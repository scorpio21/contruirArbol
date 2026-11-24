#!/usr/bin/env python3
"""Optimizador del árbol de pasivas de Guildlands (v5) para un mago.

Referencia validada del algoritmo que luego se porta a C#.
  - Datos: guildlands_tree_v5.json (nodos, aristas, reglas)
  - Stats: fórmulas extraídas del propio planificador (Td, bb, kb, Ib, Gs...)
  - Solver: CP-SAT, conectividad por flujo, linealización sucesiva del log-score
"""
import json, math, sys
from collections import defaultdict
from ortools.sat.python import cp_model

DATA = json.load(open('../guildlands_tree_v5.json', encoding='utf-8'))
NODES = {n['id']: n for n in DATA['nodes']}
EDGES = DATA['edges']
META = DATA['meta']
COMBAT = set(META['combatNodeIds'])

ADJ = defaultdict(set)
for a, b in EDGES:
    if a in COMBAT and b in COMBAT:
        ADJ[a].add(b); ADJ[b].add(a)

# ---------------- constantes del motor del juego (sacadas del bundle) ----------------
FOCUS = 1.3                      # Gs: multiplicador del atributo foco
INT_MAGIC = 0.6 * FOCUS + 0.3    # Td.magicDamage.intelligence*focus + Ib.wizard.magicDamage
INT_MANA = 7 * FOCUS             # Td.maxMana.intelligence*focus
INT_REGEN = 0.045 * FOCUS        # Td.manaRegen.intelligence*focus
CLASS_BASE = {'strength': 3, 'agility': 5, 'intelligence': 15, 'charisma': 3, 'stamina': 8}
RACE_MOD = {'gnome': {'strength': -3, 'intelligence': 3, 'charisma': 2, 'stamina': -2}}
ATTR_ORDER = ['strength', 'agility', 'intelligence', 'charisma', 'stamina']  # orden de "at"
CAPS = {'castSpeedPct': 20, 'cdrPct': 30, 'spellCostPct': 30}                # wb del bundle

GLYPHS = {   # efectos de los glifos (bundle)
    'vapor': {'magicPct': 8, 'manaRegenPct': 6},
    'agua':  {'manaPct': 10, 'manaRegenPct': 8},
    'brasa': {'allDmgPct': 5},
    'luz':   {'healDonePct': 10, 'intelligence': 2},
}


class Params:
    def __init__(self, **kw):
        self.level = 36
        self.dungeons = 5
        self.race = 'gnome'
        self.alloc = {'strength': 0, 'agility': 0, 'intelligence': 44, 'charisma': 0, 'stamina': 26}
        # equipo (desconocido -> editable)
        self.gear_int = 0; self.gear_magic = 0; self.gear_mana = 0; self.gear_hp = 0
        # elasticidades del score: cuánto pesa cada cosa respecto al daño (suposiciones editables)
        self.a_mana = 0.30; self.a_regen = 0.30; self.a_cost = 0.60
        self.a_cast = 0.50; self.a_cdr = 0.50; self.a_hp = 0.15
        self.slot_value = 0.10                       # valor (en log-score) de +1 ranura de hechizo
        self.elem_share = {}                         # {'fire':1.0} fracción de tu daño por elemento
        self.glyphs = ['vapor', 'agua']              # glifos que puedes fabricar
        self.force = [564]                           # nodos obligatorios
        self.forbid = []                             # nodos prohibidos
        self.allow_keys = True                       # permitir piedras angulares (máx 2)
        self.key_value = {}                          # valor extra por piedra {id: logscore}
        self.time_limit = 60
        self.__dict__.update(kw)

    @property
    def budget(self):
        return (self.level - 1) + self.dungeons


def tree_sums(ids, glyph_choice=()):
    """Suma numérica de fx de los nodos (+ glifos elegidos)."""
    s = defaultdict(float)
    for i in ids:
        for k, v in NODES[i].get('fx', {}).items():
            if isinstance(v, (int, float)) and not isinstance(v, bool):
                s[k] += v
            elif k == 'elemPct':
                for el, p in v.items():
                    s['elem_' + el] += p
    for g in glyph_choice:
        for k, v in GLYPHS[g].items():
            s[k] += v
    return s


def attrs_total(P, s):
    a = {k: CLASS_BASE[k] + RACE_MOD[P.race].get(k, 0) + P.alloc.get(k, 0) for k in ATTR_ORDER}
    for k in ATTR_ORDER:
        a[k] += s.get(k, 0) + s.get('allAttributes', 0)
    a['intelligence'] += P.gear_int
    return a


def evaluate(P, ids, glyph_choice=()):
    """Stats finales y log-score exacto de un build."""
    s = tree_sums(ids, glyph_choice)
    a = attrs_total(P, s)
    L = P.level
    I = a['intelligence']
    magic_pct = s['magicPct'] + s['allDmgPct']
    D = (2 + INT_MAGIC * I + P.gear_magic) * (1 + magic_pct / 100) * 1.10
    mana = (15 + 2 * (L - 1) + INT_MANA * I + P.gear_mana) * (1 + s['manaPct'] / 100)
    regen = (1 + 0.1 * (L - 1) + INT_REGEN * I) * (1 + s['manaRegenPct'] / 100)
    cs = min(s['castSpeedPct'], CAPS['castSpeedPct'])
    cdr = min(s['cdrPct'], CAPS['cdrPct'])
    cost = min(s['spellCostPct'], CAPS['spellCostPct'])
    hp = (40 + 4 * (L - 1) + 3 * CLASS_BASE['strength'] + 6 * a['stamina'] + P.gear_hp) * (1 + s['hpPct'] / 100)
    elem = 0.0
    for el, sh in P.elem_share.items():
        elem += sh * math.log(1 + s['elem_' + el] / 100)
    keyv = sum(P.key_value.get(i, 0) for i in ids)
    score = (math.log(D)
             + P.a_mana * math.log(mana) + P.a_regen * math.log(regen)
             + P.a_cast * math.log(1 + cs / 100) - P.a_cdr * math.log(1 - cdr / 100)
             - P.a_cost * math.log(1 - cost / 100)
             + P.a_hp * math.log(hp) + elem
             + P.slot_value * s['spellSlots'] + keyv)
    return dict(score=score, D=D, mana=mana, regen=regen, hp=hp, INT=I, VIT=a['stamina'],
                magicPct=magic_pct, manaPct=s['manaPct'], regenPct=s['manaRegenPct'],
                cast=cs, cdr=cdr, cost=cost, slots=s['spellSlots'], sums=dict(s))


def gradient(P, ids, glyph_choice=()):
    """Derivadas parciales del log-score respecto a cada clave de fx, en el punto actual."""
    e = evaluate(P, ids, glyph_choice)
    s = e['sums']; L = P.level
    I = e['INT']
    Dpre = 2 + INT_MAGIC * I + P.gear_magic
    Mpre = 15 + 2 * (L - 1) + INT_MANA * I + P.gear_mana
    Rpre = 1 + 0.1 * (L - 1) + INT_REGEN * I
    Hpre = 40 + 4 * (L - 1) + 9 + 6 * e['VIT'] + P.gear_hp
    dI = INT_MAGIC / Dpre + P.a_mana * INT_MANA / Mpre + P.a_regen * INT_REGEN / Rpre
    g = defaultdict(float)
    g['intelligence'] = dI
    g['stamina'] = P.a_hp * 6 / Hpre
    mp = 1.0 / (100 + e['magicPct'])
    g['magicPct'] = mp; g['allDmgPct'] = mp
    g['manaPct'] = P.a_mana / (100 + e['manaPct'])
    g['manaRegenPct'] = P.a_regen / (100 + e['regenPct'])
    g['hpPct'] = P.a_hp / (100 + s['hpPct'])
    g['castSpeedPct'] = 0 if e['cast'] >= CAPS['castSpeedPct'] else P.a_cast / (100 + e['cast'])
    g['cdrPct'] = 0 if e['cdr'] >= CAPS['cdrPct'] else P.a_cdr / (100 - e['cdr'])
    g['spellCostPct'] = 0 if e['cost'] >= CAPS['spellCostPct'] else P.a_cost / (100 - e['cost'])
    g['spellSlots'] = P.slot_value
    g['allAttributes'] = dI + g['stamina']
    for el, sh in P.elem_share.items():
        g['elem_' + el] = sh / (100 + s['elem_' + el])
    return g


def node_weight(P, n, g):
    w = 0.0
    for k, v in n.get('fx', {}).items():
        if isinstance(v, (int, float)) and not isinstance(v, bool):
            w += g.get(k, 0) * v
        elif k == 'elemPct':
            for el, p in v.items():
                w += g.get('elem_' + el, 0) * p
    return w + P.key_value.get(n['id'], 0)


def allowed_nodes(P):
    start = META['raceStart'][P.race]
    ok = set()
    for i in COMBAT:
        n = NODES[i]
        if n['t'] == 'start' and i != start:
            continue
        if n.get('race') and n['race'] != P.race:
            continue
        if i in P.forbid:
            continue
        if n['t'] == 'key' and not P.allow_keys and i not in P.force:
            continue
        ok.add(i)
    return start, ok


def solve(P, g, time_limit=60, hint=None):
    start, ok = allowed_nodes(P)
    B = P.budget
    m = cp_model.CpModel()
    x = {i: m.NewBoolVar(f'x{i}') for i in ok}
    m.Add(x[start] == 1)
    m.Add(sum(x[i] for i in ok if i != start) <= B)
    for i in P.force:
        m.Add(x[i] == 1)
    # piedras angulares
    keys = [i for i in ok if NODES[i]['t'] == 'key']
    m.Add(sum(x[i] for i in keys) <= META['keyMax'])
    for fam in META['keyFamilies_mutuallyExclusive']:
        f = [i for i in fam if i in ok]
        if len(f) > 1:
            m.Add(sum(x[i] for i in f) <= 1)
    for grp in META['choiceGroups_pickOne']:
        f = [i for i in grp if i in ok]
        if len(f) > 1:
            m.Add(sum(x[i] for i in f) <= 1)
    # conectividad por flujo (una unidad por nodo tomado, desde la raíz)
    arcs = [(u, v) for u in ok for v in ADJ[u] if v in ok]
    f = {(u, v): m.NewIntVar(0, B, f'f{u}_{v}') for u, v in arcs}
    for (u, v), var in f.items():
        m.Add(var <= B * x[u]); m.Add(var <= B * x[v])
    for v in ok:
        if v == start:
            continue
        inflow = sum(f[(u, v)] for u in ADJ[v] if (u, v) in f)
        outflow = sum(f[(v, w)] for w in ADJ[v] if (v, w) in f)
        m.Add(inflow - outflow == x[v])
    # glifos en nexos
    ysock = {}
    gl = [gname for gname in P.glyphs]
    sockets = [i for i in ok if NODES[i]['t'] == 'socket']
    for sck in sockets:
        for gname in gl:
            ysock[(sck, gname)] = m.NewBoolVar(f'y{sck}_{gname}')
            m.Add(ysock[(sck, gname)] <= x[sck])
        m.Add(sum(ysock[(sck, gname)] for gname in gl) <= 1)
    for gname in gl:
        m.Add(sum(ysock[(sck, gname)] for sck in sockets) <= 1)
    SC = 1_000_000
    obj = []
    for i in ok:
        if i == start:
            continue
        w = node_weight(P, NODES[i], g)
        obj.append(int(round(w * SC)) * x[i])
    for (sck, gname), y in ysock.items():
        w = sum(g.get(k, 0) * v for k, v in GLYPHS[gname].items())
        obj.append(int(round(w * SC)) * y)
    m.Maximize(sum(obj))
    if hint:
        for i in ok:
            m.AddHint(x[i], 1 if i in hint else 0)
    sv = cp_model.CpSolver()
    sv.parameters.max_time_in_seconds = time_limit
    sv.parameters.num_workers = 8
    st = sv.Solve(m)
    if st not in (cp_model.OPTIMAL, cp_model.FEASIBLE):
        return None
    ids = {i for i in ok if sv.Value(x[i]) and i != start}
    glyphs = {sck: gname for (sck, gname), y in ysock.items() if sv.Value(y)}
    return ids, glyphs, st == cp_model.OPTIMAL


def optimize(P, start_ids=None, iters=8, verbose=True):
    start = META['raceStart'][P.race]
    cur = set(start_ids or [])
    glyph_cur = ()
    best = None
    seen = []
    for it in range(iters):
        g = gradient(P, cur, glyph_cur) if cur else gradient(P, {564}, ())
        res = solve(P, g, time_limit=P.time_limit, hint=cur)
        if res is None:
            raise RuntimeError('sin solución')
        ids, glyphs, opt = res
        gch = tuple(glyphs.values())
        ev = evaluate(P, ids, gch)
        if verbose:
            print(f'  it{it}: score={ev["score"]:.4f} nodos={len(ids)} óptimo_lineal={opt}')
        if best is None or ev['score'] > best[0]['score'] + 1e-9:
            best = (ev, ids, glyphs)
        key = frozenset(ids)
        if key in seen:
            break
        seen.append(key)
        cur, glyph_cur = set(ids), gch
    return best


def to_rc(ids):
    def b36(n):
        d = '0123456789abcdefghijklmnopqrstuvwxyz'
        s = ''
        while True:
            s = d[n % 36] + s
            n //= 36
            if n == 0:
                return s
    return '.'.join(b36(i) for i in sorted(ids))


def from_rc(rc):
    return {int(x, 36) for x in rc.split('.')}


def describe(P, ids, glyphs):
    rows = []
    for i in sorted(ids):
        n = NODES[i]
        if n['t'] in ('notable', 'key', 'choice', 'socket'):
            extra = f' [glifo {glyphs[i]}]' if i in glyphs else ''
            rows.append(f"    {i:>5} {n['t']:<8} {n.get('name', ''):<22} {n['text']}{extra}")
    return '\n'.join(rows)
