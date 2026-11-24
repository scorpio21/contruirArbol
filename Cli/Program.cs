using GuildlandsTree.Core;

// Prueba de humo. Sin argumentos: comprueba que el modelo reproduce los números del prototipo Python.
//   dotnet run --project Cli          -> solo evaluación (segundos)
//   dotnet run --project Cli opt      -> además optimiza el paladín (tarda ~1-3 min)
var dir = AppContext.BaseDirectory;
var t = TreeData.Load(Path.Combine(dir, "guildlands_tree_v5.json"));
ClassData.Load(Path.Combine(dir, "guildlands_classes.json"));
Console.WriteLine($"Árbol v{t.Meta.Version}: {t.Nodes.Count} nodos, {t.Combat.Count} de combate · {ClassData.Current.Classes.Count} clases");

static void Print(string tag, Eval e) =>
    Console.WriteLine($"{tag,-10} score {e.Score:F3} | físico {e.S("physicalDamage") * 1.14:F0} mágico {e.S("magicDamage") * 1.14:F0} vida {e.S("maxHp"):F0} " +
                      $"maná {e.S("maxMana"):F0} armadura {e.S("armor"):F1} crit {e.S("critChance"):F1}% x{e.S("critDamage"):F0} " +
                      $"velAtq {e.S("attackSpeed"):F0} curación {e.S("healIncrease"):F1}% evasión {e.S("evasion"):F1}");

// ---- Paladín humano nivel 38 (capturas del jugador) ----
const string urlP = "https://tree.guildlands.com/es/#raza=human&clase=paladin&v=5&nivel=38&mazmorras=5&rc=1y.22.23.24.25.26.27.28.3r.3y.3z.40.41.4x.52.53.54.55.56.5q.5r.5s.5t.5u.5v.5w.5x.60.61.62.64.65.6c.6d.6f.80.81.83.8k.8l.8n.8w&at=19.3.0.2.o";
var u = UrlCodec.ParseUrl(urlP);
var p = new Params { Class = u.Class, Race = u.Race, Level = u.Level, Dungeons = u.Dungeons, Alloc = u.Alloc!, W = StatModel.DefaultWeights(u.Class) };
p.GearAttr = new double[] { 5, 8, 5, 11, 5 };
p.GearFlat["maxHp"] = 176; p.GearFlat["physicalDamage"] = 38; p.GearFlat["maxMana"] = 58; p.GearFlat["magicDamage"] = 29;
p.GearPct["armorPct"] = 18; p.GearPct["defensePct"] = 9; p.GearPct["critChancePct"] = 4;
Console.WriteLine($"Paladín: {u.Ids.Count} nodos, válido: {t.IsValid(p, u.Ids)}, presupuesto {p.Budget}");
Print("tu build", StatModel.Evaluate(t, p, u.Ids));
Console.WriteLine("  esperado: score 12.713 | físico 123 mágico 49 vida 944 maná 284 armadura 83.8 crit 21.6% x154 velAtq 122 curación 14.3% evasión 7.6");

// ---- Mago gnomo nivel 36 (sin equipo, sin glifos) ----
var uw = UrlCodec.ParseUrl("https://tree.guildlands.com/es/#raza=gnome&clase=wizard&v=5&nivel=36&mazmorras=5&rc=" +
    "s.z.10.16.17.18.19.1a.1b.1c.1d.1e.1f.1g.1h.1i.2s.2t.2u.2v.2w.48.49.6q.6r.6s.6u.6v.72.73.9b.9c.9d.9e.9g.9k.9l.fo.h0&at=0.0.18.0.q");
var pw = new Params { Class = "wizard", Race = uw.Race, Level = uw.Level, Dungeons = uw.Dungeons, Alloc = uw.Alloc!, W = StatModel.DefaultWeights("wizard") };
Print("mago", StatModel.Evaluate(t, pw, uw.Ids));
Console.WriteLine("  esperado: score 8.726 | mágico 139 vida 393 maná 884 armadura 48.6 crit 7.5% velAtq 101");

if (args.Contains("opt"))
{
    p.TimeLimitSeconds = 25;
    var sol = Optimizer.Optimize(t, p, u.Ids, iterations: 4, log: Console.WriteLine);
    Print("óptimo", sol.Eval);
    Console.WriteLine("  esperado (Python, datos del bundle 5912u1nv): score 13.110 | físico 137 vida 1039 armadura 81.6 crit 31% velAtq 149");
    Console.WriteLine("rc = " + UrlCodec.ToRc(sol.Ids));
    foreach (var i in sol.Ids.OrderBy(i => i))
    {
        var n = t.Nodes[i];
        if (n.Type is "notable" or "key" or "choice") Console.WriteLine($"  {i,5} {n.Type,-8} {n.Name,-22} {n.Text}");
    }
    Console.WriteLine(UrlCodec.BuildUrl(p, p.Class, sol.Ids, t.Meta.Version));
}
