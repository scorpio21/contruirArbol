using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuildlandsTree.Core;

public sealed class Node
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("t")] public string Type { get; set; } = "";
    [JsonPropertyName("x")] public double X { get; set; }
    [JsonPropertyName("y")] public double Y { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("text")] public string Text { get; set; } = "";
    [JsonPropertyName("zone")] public string? Zone { get; set; }
    [JsonPropertyName("race")] public string? Race { get; set; }
    [JsonPropertyName("fx")] public Dictionary<string, JsonElement>? Fx { get; set; }

    /// <summary>Efectos numéricos aplanados (claves fx; elemPct -> "elem_fire", ...).</summary>
    [JsonIgnore] public Dictionary<string, double> Numeric { get; } = new();

    /// <summary>Bonos "por cada N de un atributo" (perAttr), con tope.</summary>
    [JsonIgnore] public List<PerAttrEffect> PerAttr { get; } = new();
}

/// <summary>+1 stat por cada <see cref="Per"/> puntos de <see cref="Attr"/>, hasta <see cref="Cap"/>.</summary>
public sealed record PerAttrEffect(string Attr, double Per, string Stat, double Cap);

public sealed class Meta
{
    [JsonPropertyName("version")] public int Version { get; set; }
    [JsonPropertyName("keyMax")] public int KeyMax { get; set; }
    [JsonPropertyName("keyFamilies_mutuallyExclusive")] public int[][] KeyFamilies { get; set; } = Array.Empty<int[]>();
    [JsonPropertyName("choiceGroups_pickOne")] public int[][] ChoiceGroups { get; set; } = Array.Empty<int[]>();
    [JsonPropertyName("raceStart")] public Dictionary<string, int> RaceStart { get; set; } = new();
    [JsonPropertyName("combatNodeIds")] public int[] CombatNodeIds { get; set; } = Array.Empty<int>();
}

public sealed class TreeFile
{
    [JsonPropertyName("meta")] public Meta Meta { get; set; } = new();
    [JsonPropertyName("nodes")] public List<Node> Nodes { get; set; } = new();
    [JsonPropertyName("edges")] public int[][] Edges { get; set; } = Array.Empty<int[]>();
}

public sealed class TreeData
{
    public Meta Meta { get; }
    public Dictionary<int, Node> Nodes { get; } = new();
    public HashSet<int> Combat { get; }
    public Dictionary<int, List<int>> Adj { get; } = new();

    private TreeData(TreeFile f)
    {
        Meta = f.Meta;
        Combat = new HashSet<int>(f.Meta.CombatNodeIds);
        foreach (var n in f.Nodes)
        {
            if (n.Fx != null)
            {
                foreach (var kv in n.Fx)
                {
                    if (kv.Value.ValueKind == JsonValueKind.Number)
                        n.Numeric[kv.Key] = kv.Value.GetDouble();
                    else if (kv.Key == "perAttr")
                    {
                        var items = kv.Value.ValueKind == JsonValueKind.Array ? kv.Value.EnumerateArray().ToList() : new List<JsonElement> { kv.Value };
                        foreach (var it in items)
                            if (it.ValueKind == JsonValueKind.Object)
                                n.PerAttr.Add(new PerAttrEffect(it.GetProperty("attr").GetString()!, it.GetProperty("per").GetDouble(),
                                                                it.GetProperty("stat").GetString()!, it.GetProperty("cap").GetDouble()));
                    }
                    else if (kv.Key == "elemPct" && kv.Value.ValueKind == JsonValueKind.Object)
                        foreach (var e in kv.Value.EnumerateObject())
                            if (e.Value.ValueKind == JsonValueKind.Number)
                                n.Numeric["elem_" + e.Name] = e.Value.GetDouble();
                }
            }
            Nodes[n.Id] = n;
        }
        foreach (var id in Combat) Adj[id] = new List<int>();
        foreach (var e in f.Edges)
        {
            int a = e[0], b = e[1];
            if (Combat.Contains(a) && Combat.Contains(b)) { Adj[a].Add(b); Adj[b].Add(a); }
        }
    }

    public static TreeData Load(string path)
    {
        var opt = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
        var f = JsonSerializer.Deserialize<TreeFile>(File.ReadAllText(path), opt)
                ?? throw new InvalidDataException("JSON del árbol vacío");
        return new TreeData(f);
    }

    /// <summary>Nodo inicial y conjunto de nodos que esta raza puede tomar.</summary>
    public (int Start, HashSet<int> Allowed) Allowed(Params p)
    {
        int start = Meta.RaceStart[p.Race];
        var ok = new HashSet<int>();
        foreach (var i in Combat)
        {
            var n = Nodes[i];
            if (n.Type == "start" && i != start) continue;
            if (n.Race != null && n.Race != p.Race) continue;
            if (p.Forbid.Contains(i)) continue;
            if (n.Type == "key" && !p.AllowKeys && !p.Force.Contains(i)) continue;
            ok.Add(i);
        }
        return (start, ok);
    }

    /// <summary>¿El conjunto cumple presupuesto, piedras, exclusiones, grupos y conectividad?</summary>
    public bool IsValid(Params p, HashSet<int> ids)
    {
        var (start, ok) = Allowed(p);
        if (ids.Count > p.Budget || !ids.IsSubsetOf(ok)) return false;
        if (ids.Count(i => Nodes[i].Type == "key") > Meta.KeyMax) return false;
        foreach (var fam in Meta.KeyFamilies) if (fam.Count(ids.Contains) > 1) return false;
        foreach (var grp in Meta.ChoiceGroups) if (grp.Count(ids.Contains) > 1) return false;
        if (p.Force.Any(f => !ids.Contains(f))) return false;
        var all = new HashSet<int>(ids) { start };
        var seen = new HashSet<int> { start };
        var st = new Stack<int>(); st.Push(start);
        while (st.Count > 0)
        {
            int u = st.Pop();
            foreach (var v in Adj[u]) if (all.Contains(v) && seen.Add(v)) st.Push(v);
        }
        return seen.Count == all.Count;
    }
}
