namespace GuildlandsTree.Core;

public sealed class Params
{
    public string Class = "wizard";
    public string Race = "gnome";
    public int Level = 36;
    public int Dungeons = 5;
    /// <summary>Puntos repartidos a mano, orden del enlace (at): FUE, AGI, INT, CAR, VIT.</summary>
    public int[] Alloc = { 0, 0, 0, 0, 0 };

    // Equipo (suma de tus piezas)
    public double[] GearAttr = new double[5];                                // atributos de equipo FUE, AGI, INT, CAR, VIT
    public Dictionary<string, double> GearFlat = new();                      // maxHp, maxMana, physicalDamage, magicDamage
    public Dictionary<string, double> GearPct = new();                       // armorPct, defensePct, critChancePct, ...

    /// <summary>Pesos del objetivo RELATIVOS al daño principal (=1). SUPOSICIONES editables, no datos del juego.</summary>
    public Dictionary<string, double> W = StatModel.DefaultWeights("wizard");
    public Dictionary<string, double> ElemShare = new();                     // {"fire": 1.0}
    public List<string> Glyphs = new();
    public HashSet<int> Force = new();
    public HashSet<int> Forbid = new();
    public bool AllowKeys = true;
    public Dictionary<int, double> KeyValue = new();
    public int TimeLimitSeconds = 60;

    public int Budget => (Level - 1) + Dungeons;
}

public sealed class Eval
{
    public double Score;
    public Dictionary<string, double> Stats = new();      // estadísticas finales (maxHp, physicalDamage, armor, ...)
    public Dictionary<string, double> Attr = new();       // atributos totales
    public Dictionary<string, double> Sums = new();       // suma de efectos del árbol
    public double S(string k) => Stats.TryGetValue(k, out var v) ? v : 0;
}

/// <summary>Pesos lineales (gradiente en el punto actual) para el solver.</summary>
public sealed class Weights
{
    public Dictionary<string, double> G = new();
    public Dictionary<int, double> Extra = new();
    Params _p = null!;
    public static Weights Create(Params p, Dictionary<string, double> g, Dictionary<int, double> extra) => new() { G = g, Extra = extra, _p = p };

    public double OfNode(Node n)
    {
        double w = 0;
        foreach (var kv in n.Numeric) w += (G.TryGetValue(kv.Key, out var gv) ? gv : 0) * kv.Value;
        if (Extra.TryGetValue(n.Id, out var e)) w += e;
        if (_p.KeyValue.TryGetValue(n.Id, out var kvv)) w += kvv;
        return w;
    }
    public double Glyph(string gl) => StatModel.Glyphs[gl].Sum(e => (G.TryGetValue(e.Key, out var gv) ? gv : 0) * e.Value);
}

public static class StatModel
{
    public static readonly string[] AttrOrder = { "strength", "agility", "intelligence", "charisma", "stamina" };
    static readonly HashSet<string> Ib = new() { "strength", "intelligence", "stamina", "agility" };
    static readonly HashSet<string> AdditiveGear = new()
    { "critChance", "evasion", "critDamage", "cooldownPct", "block", "elementalRes", "healIncrease", "vampirism",
      "goldBonus", "expBonus", "rarity", "manaCostPct", "potionBonus", "restBonus", "steadfast" };
    static readonly Dictionary<string, string[]> GearPctKey = new()
    {
        ["armor"] = new[] { "armorPct", "defensePct" }, ["critChance"] = new[] { "critChancePct" }, ["critDamage"] = new[] { "critDamagePct" },
        ["evasion"] = new[] { "evasionPct" }, ["block"] = new[] { "blockPct" }, ["attackSpeed"] = new[] { "attackSpeedPct" },
        ["hpRegen"] = new[] { "hpRegenPct" }, ["manaRegen"] = new[] { "manaRegenPct" }, ["castSpeed"] = new[] { "castSpeedPct" },
        ["manaCostPct"] = new[] { "manaCostPct" }, ["cooldownPct"] = new[] { "cooldownPct" }, ["healIncrease"] = new[] { "healIncreasePct" },
    };
    static readonly HashSet<string> Phys = new() { "warrior", "paladin", "rogue", "bandit", "hunter", "beastmaster", "pirate", "guardian", "avenger", "warlord", "duelist" };
    static readonly HashSet<string> Magic = new() { "wizard", "necromancer", "cleric", "artificer", "apothecary", "bard" };
    static readonly HashSet<string> Evasive = new() { "rogue", "bandit", "duelist", "hunter", "ranger" };
    static readonly HashSet<string> Tank = new() { "warrior", "paladin", "guardian", "avenger", "warlord" };

    public static readonly Dictionary<string, Dictionary<string, double>> Glyphs = new()
    {
        ["vapor"] = new() { ["magicPct"] = 8, ["manaRegenPct"] = 6 },
        ["agua"]  = new() { ["manaPct"] = 10, ["manaRegenPct"] = 8 },
        ["brasa"] = new() { ["allDmgPct"] = 5 },
        ["luz"]   = new() { ["healDonePct"] = 10, ["intelligence"] = 2 },
    };

    static double G(Dictionary<string, double> d, string k) => d.TryGetValue(k, out var v) ? v : 0;
    static double K(string k) => ClassData.Current.Constants[k];

    /// <summary>Pesos por defecto según el rol de la clase (SUPOSICIONES editables).</summary>
    public static Dictionary<string, double> DefaultWeights(string cls)
    {
        var w = new Dictionary<string, double>();
        if (Magic.Contains(cls))
        { w["magic"] = 1; w["mana"] = .30; w["regen"] = .30; w["cost"] = .60; w["cast"] = .50; w["cdr"] = .50; w["hp"] = .15; }
        else if (Phys.Contains(cls))
        {
            w["phys"] = 1; w["hp"] = Tank.Contains(cls) ? .8 : .45; w["armor"] = Tank.Contains(cls) ? .45 : .15;
            w["heal"] = cls == "paladin" ? .2 : .05; w["evasion"] = Evasive.Contains(cls) ? .35 : 0;
        }
        else
        { w["phys"] = .6; w["magic"] = .6; w["hp"] = .4; w["armor"] = .15; w["mana"] = .1; w["evasion"] = cls == "ranger" ? .2 : 0; w["cast"] = .2; w["cdr"] = .2; }
        w["slots"] = .10;
        return w;
    }

    public static Dictionary<string, double> Sum(TreeData t, IEnumerable<int> ids, IEnumerable<string>? glyphs = null)
    {
        var s = new Dictionary<string, double>();
        foreach (var i in ids)
            foreach (var kv in t.Nodes[i].Numeric) s[kv.Key] = G(s, kv.Key) + kv.Value;
        if (glyphs != null)
            foreach (var g in glyphs)
                foreach (var kv in Glyphs[g]) s[kv.Key] = G(s, kv.Key) + kv.Value;
        return s;
    }

    static double Ub(double e) => e > 0 ? K("atkSpdMax") * e * e / (e * e + K("atkSpdK") * K("atkSpdK")) : 0;

    public static double[] BaseAttrs(Params p)
    {
        var cd = ClassData.Current;
        var ca = cd.Classes[p.Class].Attributes; var rm = cd.RaceMod[p.Race];
        var r = new double[5];
        for (int i = 0; i < 5; i++) r[i] = Math.Max(1, ca[AttrOrder[i]] + G(rm, AttrOrder[i])) + p.Alloc[i];
        return r;
    }

    /// <summary>Estadísticas finales (misma cadena de cálculo que el planificador).</summary>
    public static Dictionary<string, double> Compute(Params p, Dictionary<string, double> sIn, IEnumerable<PerAttrEffect> per,
                                                     out Dictionary<string, double> attrOut)
    {
        var C = ClassData.Current;
        var s = new Dictionary<string, double>(sIn);
        var ba = BaseAttrs(p);
        var d = new Dictionary<string, double>();
        for (int i = 0; i < 5; i++)
        {
            var a = AttrOrder[i];
            d[a] = ba[i] + p.GearAttr[i] + G(s, a) + G(s, "allAttributes");
        }
        foreach (var e in per)
            s[e.Stat] = G(s, e.Stat) + Math.Min(e.Cap, Math.Floor(Math.Max(0, G(d, e.Attr)) / e.Per));
        var cl = C.Classes[p.Class]; int L = p.Level;
        bool spread = G(s, "focusSpread") > 0;
        double Pf(string a) => spread ? K("spread") : (Ib.Contains(a) && cl.Focus.Contains(a) ? K("focus") : 1.0);
        double m = spread ? K("spread") : K("focus");

        var o = new Dictionary<string, double>(); var pre = new Dictionary<string, double>();
        foreach (var st in C.Stats)
        {
            double q = G(C.Base, st);
            if (C.AttrCoef.TryGetValue(st, out var coefs))
                foreach (var kv in coefs)
                {
                    double co = kv.Value;
                    if (st == "hpRegen" && kv.Key == "stamina" && C.Kb.TryGetValue(p.Class, out var kb)) co = kb;
                    q += d[kv.Key] * co * Pf(kv.Key);
                }
            if (st == "attackSpeed") q += Ub(d["agility"] * Pf("agility"));
            if (C.Ob.TryGetValue(p.Class, out var ob) && ob.TryGetValue(st, out var b1)) q += d["charisma"] * b1 * m;
            if (C.Nb.TryGetValue(p.Class, out var nb) && nb.TryGetValue(st, out var b2)) q += d["stamina"] * b2;
            if (C.Fb.TryGetValue(p.Class, out var fb) && fb.TryGetValue(st, out var b3)) q += d["intelligence"] * b3;
            if (C.Vb.TryGetValue(p.Class, out var vb) && vb.TryGetValue(st, out var b4)) q += d["agility"] * b4;
            if (st == "healIncrease" && d["charisma"] > 0) q += K("healMax") * d["charisma"] / (d["charisma"] + K("healK"));
            if (st == "maxHp")
                q += 3 * cl.Attributes["strength"] + (cl.Focus.Contains("strength") ? K("classHpPerLevelIfStrFocus") * (Math.Max(1, L) - 1) : 0);
            if (L > 1 && C.PerLevel.TryGetValue(st, out var pl)) q += pl * (L - 1);
            q += G(p.GearFlat, st);
            double g = q;
            if (GearPctKey.TryGetValue(st, out var keys))
                foreach (var gk in keys)
                {
                    double R = G(p.GearPct, gk);
                    if (R == 0) continue;
                    if (AdditiveGear.Contains(st)) g += R; else g *= 1 + R / 100;
                }
            pre[st] = g;
            if (C.PassiveAdd.TryGetValue(st, out var pa) && G(s, pa) != 0) g += s[pa];
            if (st == "healIncrease") g += G(s, "healTakenPct");
            if (C.PassiveMul.TryGetValue(st, out var pm) && G(s, pm) != 0) g *= 1 + s[pm] / 100;
            o[st] = g;
        }
        double extra = G(s, "allDmgPct");
        o["physicalDamage"] *= 1 + extra / 100; o["magicDamage"] *= 1 + extra / 100;
        if (C.Perk.TryGetValue(p.Class, out var pk)) o[pk.Stat] *= 1 + pk.Pct / 100;
        if (C.Mb.TryGetValue(p.Class, out var mb)) { o["maxHp"] *= 1 - mb / 100; o["armor"] *= 1 - mb / 100; }
        if (G(s, "noEvasion") > 0) o["evasion"] = 0;
        if (G(s, "noCrit") > 0 || G(s, "critEvery") > 0) o["critChance"] = 0;
        if (G(s, "critCap") > 0) o["critChance"] = Math.Min(o["critChance"], s["critCap"]);
        if (G(s, "armorMult") > 0) o["armor"] *= s["armorMult"];
        if (G(s, "manaRegenMult") > 0) o["manaRegen"] *= s["manaRegenMult"];
        if (G(s, "noBlock") > 0) o["block"] = 0;
        if (G(s, "ironSpells") > 0) o["magicDamage"] = 0;
        if (G(s, "overheal") > 0) o["hpRegen"] = 0;
        if (G(s, "overkillCarry") > 0) o["vampirism"] = 0;
        foreach (var kv in C.Caps)
            if (o.TryGetValue(kv.Key, out var v) && v > kv.Value) o[kv.Key] = Math.Max(kv.Value, pre[kv.Key]);
        o["critChance"] = Math.Min(100, o["critChance"]);
        attrOut = d;
        return o;
    }

    public static double ScoreOf(Params p, Dictionary<string, double> o, Dictionary<string, double> s, IEnumerable<int>? ids)
    {
        var w = p.W; double mult = K("globalDamageMult");
        double W(string k) => G(w, k);
        double cc = o["critChance"] / 100, cdm = o["critDamage"] / 100;
        double physEff = o["physicalDamage"] + G(s, "weaponMagicPct") / 100 * o["magicDamage"];   // Arma Bendita
        double physDps = physEff * mult * (1 + cc * (cdm - 1)) * o["attackSpeed"] / 100;
        double sc = 0;
        if (W("phys") != 0) sc += W("phys") * Math.Log(Math.Max(physDps, 1e-9));
        if (W("magic") != 0) sc += W("magic") * Math.Log(Math.Max(o["magicDamage"] * mult, 1e-9));
        if (W("hp") != 0) sc += W("hp") * Math.Log(Math.Max(o["maxHp"], 1e-9));
        if (W("armor") != 0) sc += W("armor") * Math.Log(Math.Max(o["armor"], 1));
        if (W("mana") != 0) sc += W("mana") * Math.Log(Math.Max(o["maxMana"], 1e-9));
        if (W("regen") != 0) sc += W("regen") * Math.Log(Math.Max(o["manaRegen"], 1e-9));
        if (W("heal") != 0) sc += W("heal") * Math.Log(1 + o["healIncrease"] / 100);
        if (W("evasion") != 0) sc += W("evasion") * Math.Log(1 + o["evasion"] / 100);
        if (W("cast") != 0) sc += W("cast") * Math.Log(Math.Max(o["castSpeed"], 1) / 100);
        if (W("cdr") != 0) sc -= W("cdr") * Math.Log(1 - Math.Min(o["cooldownPct"], 95) / 100);
        if (W("cost") != 0) sc -= W("cost") * Math.Log(1 - Math.Min(o["manaCostPct"], 95) / 100);
        sc += W("slots") * G(s, "spellSlots");
        foreach (var kv in p.ElemShare) sc += kv.Value * Math.Log(1 + G(s, "elem_" + kv.Key) / 100);
        if (ids != null) foreach (var i in ids) if (p.KeyValue.TryGetValue(i, out var v)) sc += v;
        return sc;
    }

    static List<PerAttrEffect> PerOf(TreeData t, IEnumerable<int> ids) => ids.SelectMany(i => t.Nodes[i].PerAttr).ToList();

    public static Eval Evaluate(TreeData t, Params p, IEnumerable<int> ids, IEnumerable<string>? glyphs = null)
    {
        var idl = ids as ICollection<int> ?? ids.ToList();
        var s = Sum(t, idl, glyphs);
        var o = Compute(p, s, PerOf(t, idl), out var d);
        return new Eval { Score = ScoreOf(p, o, s, idl), Stats = o, Attr = d, Sums = s };
    }

    /// <summary>Gradiente numérico del score respecto a cada clave fx + efecto exacto de los nodos perAttr.</summary>
    public static Weights MakeWeights(TreeData t, Params p, IEnumerable<int> ids, IEnumerable<string>? glyphs = null)
    {
        var idl = ids.ToList();
        var s = Sum(t, idl, glyphs);
        var per = PerOf(t, idl);
        double Sc(Dictionary<string, double> ss, IEnumerable<PerAttrEffect> pp) =>
            ScoreOf(p, Compute(p, ss, pp, out _), ss, null);
        double baseScore = Sc(s, per);
        var keys = new HashSet<string>();
        foreach (var i in t.Combat) foreach (var k in t.Nodes[i].Numeric.Keys) keys.Add(k);
        foreach (var g in Glyphs.Values) foreach (var k in g.Keys) keys.Add(k);
        var grad = new Dictionary<string, double>();
        foreach (var k in keys)
        {
            var s2 = new Dictionary<string, double>(s) { [k] = G(s, k) + 1.0 };
            grad[k] = Sc(s2, per) - baseScore;
        }
        var extra = new Dictionary<int, double>();
        var idset = new HashSet<int>(idl);
        foreach (var i in t.Combat)
        {
            var n = t.Nodes[i];
            if (n.PerAttr.Count == 0) continue;
            if (idset.Contains(i))
            {
                var others = idl.Where(j => j != i).SelectMany(j => t.Nodes[j].PerAttr).ToList();
                extra[i] = baseScore - Sc(s, others);
            }
            else extra[i] = Sc(s, per.Concat(n.PerAttr).ToList()) - baseScore;
        }
        return Weights.Create(p, grad, extra);
    }
}
