import sys, itertools; sys.path.insert(0,'/home/claude/gl_solver')
from gl_solver import *

def best_glyphs(P, ids):
    socks=[i for i in ids if NODES[i]['t']=='socket']
    best=(evaluate(P,ids,()),{})
    for r in range(1,min(len(socks),len(P.glyphs))+1):
        for ss in itertools.combinations(socks,r):
            for gs in itertools.permutations(P.glyphs,r):
                e=evaluate(P,ids,gs)
                if e['score']>best[0]['score']: best=(e,dict(zip(ss,gs)))
    return best

def valid(P, ids):
    start,ok=allowed_nodes(P)
    if len(ids)>P.budget or not ids<=ok: return False
    if sum(1 for i in ids if NODES[i]['t']=='key')>META['keyMax']: return False
    for fam in META['keyFamilies_mutuallyExclusive']:
        if len(ids&set(fam))>1: return False
    for grp in META['choiceGroups_pickOne']:
        if len(ids&set(grp))>1: return False
    if any(f not in ids for f in P.force): return False
    S=ids|{start}; seen={start}; st=[start]
    while st:
        u=st.pop()
        for v in ADJ[u]:
            if v in S and v not in seen: seen.add(v); st.append(v)
    return seen==S

def local_search(P, ids):
    cur=set(ids); ev,gl=best_glyphs(P,cur); cur_s=ev['score']; start=META['raceStart'][P.race]
    improved=True; n=0
    while improved:
        improved=False
        S=cur|{start}
        cand={v for u in S for v in ADJ[u]}-S
        cand&=allowed_nodes(P)[1]
        # swap 1-1 (quita nodo, añade vecino) y swap 2-2 simple por quitar 2 hojas
        for r in list(cur):
            for c in cand|{None}:
                new=(cur-{r})|({c} if c else set())
                if not valid(P,new): continue
                e,_=best_glyphs(P,new)
                if e['score']>cur_s+1e-9:
                    cur,cur_s=new,e['score']; improved=True; n+=1; break
            if improved: break
        if not improved and len(cur)<P.budget:
            for c in cand:
                new=cur|{c}
                if valid(P,new):
                    e,_=best_glyphs(P,new)
                    if e['score']>cur_s+1e-9: cur,cur_s=new,e['score']; improved=True; n+=1; break
    return cur,cur_s,n

def show(tag,P,ids):
    e,gl=best_glyphs(P,ids)
    print(f"{tag:<34} sc={e['score']:.3f} n={len(ids)} D={e['D']:.0f} mana={e['mana']:.0f} reg={e['regen']:.1f} hp={e['hp']:.0f} INT={e['INT']:.0f} mag%={e['magicPct']:.0f} mana%={e['manaPct']:.0f} reg%={e['regenPct']:.0f} cdr={e['cdr']:.0f} cost={e['cost']:.0f} cast={e['cast']:.0f} slot={e['slots']:.0f}")
    return e,gl

user=from_rc("s.z.10.16.17.18.19.1a.1b.1c.1d.1e.1f.1g.1h.1i.2s.2t.2u.2v.2w.48.49.6q.6r.6s.6u.6v.72.73.9b.9c.9d.9e.9g.9k.9l.fo.h0")
base=Params()
print("== tu build vs óptimo base (glifos óptimos asignados a ambos) ==")
show('tu build',base,user)
ev,ids,gl=optimize(base,start_ids=user,verbose=False)
show('óptimo CP-SAT (base)',base,ids)
ls,ls_s,n=local_search(base,ids)
print(f'búsqueda local desde el óptimo: {n} mejoras, score {ls_s:.4f}')
ls2,ls2_s,n2=local_search(base,user)
print(f'búsqueda local desde TU build: {n2} mejoras, score {ls2_s:.4f}')

print("\n== sensibilidad a los pesos (¿cambia mucho el árbol?) ==")
sc={
 'solo daño+maná (cast/cdr/cost=0)': dict(a_cast=0,a_cdr=0,a_cost=0),
 'maná importa mucho (0.6/0.6)':      dict(a_mana=0.6,a_regen=0.6,a_cost=1.0),
 'maná importa poco (0.1/0.1)':       dict(a_mana=0.1,a_regen=0.1,a_cost=0.2),
 'con equipo (INT+12, magia+30)':     dict(gear_int=12,gear_magic=30,gear_mana=60,gear_hp=120),
 'más vida (a_hp=0.5)':               dict(a_hp=0.5),
 'hechizos de fuego (100%)':          dict(elem_share={'fire':1.0}),
 'sin glifos':                        dict(glyphs=[]),
}
results={}
for name,kw in sc.items():
    P=Params(**kw)
    ev,ids2,gl2=optimize(P,start_ids=ids,verbose=False)
    results[name]=ids2
    show(name,P,ids2)
    print(f"      nodos distintos vs óptimo base: +{len(ids2-ids)} / -{len(ids-ids2)} ; observatorio={'sí' if any(NODES[i].get('zone')=='observatorio' for i in ids2) else 'no'}")
json.dump({'base':sorted(ids),**{k:sorted(v) for k,v in results.items()}},open('scenarios.json','w'))
