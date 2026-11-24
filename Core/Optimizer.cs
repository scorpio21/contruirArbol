using Google.OrTools.Sat;

namespace GuildlandsTree.Core;

public sealed record Solution(HashSet<int> Ids, Dictionary<int, string> Glyphs, Eval Eval, bool ProvenOptimal);

public static class Optimizer
{
    const long Scale = 1_000_000;

    /// <summary>Una pasada: maximiza la suma lineal de pesos g con todas las reglas del árbol.</summary>
    public static (HashSet<int> Ids, Dictionary<int, string> Glyphs, bool Optimal)? SolveOnce(
        TreeData t, Params p, Weights g, HashSet<int>? hint, CancellationToken ct = default)
    {
        var (start, ok) = t.Allowed(p);
        long B = p.Budget;
        var m = new CpModel();
        var x = ok.ToDictionary(i => i, i => m.NewBoolVar($"x{i}"));
        m.Add(x[start] == 1);
        m.Add(LinearExpr.Sum(ok.Where(i => i != start).Select(i => (LinearExpr)x[i])) <= B);
        foreach (var f in p.Force) if (x.ContainsKey(f)) m.Add(x[f] == 1);

        // Piedras angulares, familias excluyentes, grupos de elección
        var keys = ok.Where(i => t.Nodes[i].Type == "key").ToList();
        m.Add(LinearExpr.Sum(keys.Select(i => (LinearExpr)x[i])) <= t.Meta.KeyMax);
        foreach (var fam in t.Meta.KeyFamilies)
        {
            var f = fam.Where(ok.Contains).ToList();
            if (f.Count > 1) m.Add(LinearExpr.Sum(f.Select(i => (LinearExpr)x[i])) <= 1);
        }
        foreach (var grp in t.Meta.ChoiceGroups)
        {
            var f = grp.Where(ok.Contains).ToList();
            if (f.Count > 1) m.Add(LinearExpr.Sum(f.Select(i => (LinearExpr)x[i])) <= 1);
        }

        // Conectividad por flujo: cada nodo tomado consume 1 unidad que sale de la raíz por nodos tomados
        var flow = new Dictionary<(int, int), IntVar>();
        foreach (var u in ok)
            foreach (var v in t.Adj[u])
                if (ok.Contains(v))
                {
                    var fv = m.NewIntVar(0, B, $"f{u}_{v}");
                    flow[(u, v)] = fv;
                    m.Add(fv <= x[u] * B);
                    m.Add(fv <= x[v] * B);
                }
        foreach (var v in ok)
        {
            if (v == start) continue;
            var inflow = t.Adj[v].Where(u => flow.ContainsKey((u, v))).Select(u => (LinearExpr)flow[(u, v)]);
            var outflow = t.Adj[v].Where(w => flow.ContainsKey((v, w))).Select(w => (LinearExpr)flow[(v, w)]);
            m.Add(LinearExpr.Sum(inflow) - LinearExpr.Sum(outflow) == x[v]);
        }

        // Glifos en nexos (cada glifo se usa una sola vez; un glifo por nexo)
        var sockets = ok.Where(i => t.Nodes[i].Type == "socket").ToList();
        var y = new Dictionary<(int, string), BoolVar>();
        foreach (var s in sockets)
        {
            foreach (var gl in p.Glyphs)
            {
                y[(s, gl)] = m.NewBoolVar($"y{s}_{gl}");
                m.Add(y[(s, gl)] <= x[s]);
            }
            if (p.Glyphs.Count > 0)
                m.Add(LinearExpr.Sum(p.Glyphs.Select(gl => (LinearExpr)y[(s, gl)])) <= 1);
        }
        foreach (var gl in p.Glyphs)
            m.Add(LinearExpr.Sum(sockets.Select(s => (LinearExpr)y[(s, gl)])) <= 1);

        // Objetivo lineal
        var vars = new List<LinearExpr>(); var coef = new List<long>();
        foreach (var i in ok)
        {
            if (i == start) continue;
            vars.Add(x[i]); coef.Add((long)Math.Round(g.OfNode(t.Nodes[i]) * Scale));
        }
        foreach (var kv in y)
        {
            vars.Add(kv.Value); coef.Add((long)Math.Round(g.Glyph(kv.Key.Item2) * Scale));
        }
        m.Maximize(LinearExpr.WeightedSum(vars, coef));

        if (hint != null) foreach (var i in ok) m.AddHint(x[i], hint.Contains(i) ? 1 : 0);

        var solver = new CpSolver { StringParameters = $"max_time_in_seconds:{p.TimeLimitSeconds} num_workers:8" };
        using var reg = ct.Register(() => solver.StopSearch());
        var st = solver.Solve(m);
        if (st != CpSolverStatus.Optimal && st != CpSolverStatus.Feasible) return null;
        var ids = ok.Where(i => i != start && solver.Value(x[i]) == 1).ToHashSet();
        var glyphs = y.Where(kv => solver.Value(kv.Value) == 1).ToDictionary(kv => kv.Key.Item1, kv => kv.Key.Item2);
        return (ids, glyphs, st == CpSolverStatus.Optimal);
    }

    /// <summary>Linealización sucesiva: calcula pesos en el punto actual, resuelve, repite hasta que no cambie.</summary>
    public static Solution Optimize(TreeData t, Params p, IEnumerable<int>? startIds = null,
                                    int iterations = 8, Action<string>? log = null, CancellationToken ct = default)
    {
        var cur = new HashSet<int>(startIds ?? Array.Empty<int>());
        IEnumerable<string> glyphCur = Array.Empty<string>();
        Solution? best = null;
        var seen = new List<HashSet<int>>();
        for (int it = 0; it < iterations && !ct.IsCancellationRequested; it++)
        {
            var g = StatModel.MakeWeights(t, p, cur, glyphCur);
            var res = SolveOnce(t, p, g, cur, ct) ?? throw new InvalidOperationException("Sin solución con estas restricciones.");
            var ev = StatModel.Evaluate(t, p, res.Ids, res.Glyphs.Values);
            log?.Invoke($"iteración {it + 1}: score {ev.Score:F4}, {res.Ids.Count} nodos, óptimo lineal: {res.Optimal}");
            if (best == null || ev.Score > best.Eval.Score + 1e-9) best = new Solution(res.Ids, res.Glyphs, ev, res.Optimal);
            if (seen.Any(s => s.SetEquals(res.Ids))) break;
            seen.Add(res.Ids);
            cur = res.Ids; glyphCur = res.Glyphs.Values.ToList();
        }
        return best ?? throw new OperationCanceledException();
    }

    /// <summary>Mejor asignación de glifos (sobre los nexos ya tomados) para un build dado.</summary>
    public static (Eval Eval, Dictionary<int, string> Glyphs) BestGlyphs(TreeData t, Params p, HashSet<int> ids)
    {
        var socks = ids.Where(i => t.Nodes[i].Type == "socket").OrderBy(i => i).ToList();
        var bestEval = StatModel.Evaluate(t, p, ids);
        var bestMap = new Dictionary<int, string>();
        void Rec(int idx, HashSet<string> used, Dictionary<int, string> map)
        {
            if (idx == socks.Count)
            {
                var e = StatModel.Evaluate(t, p, ids, map.Values);
                if (e.Score > bestEval.Score) { bestEval = e; bestMap = new Dictionary<int, string>(map); }
                return;
            }
            Rec(idx + 1, used, map);                                   // nexo vacío
            foreach (var gl in p.Glyphs)
            {
                if (used.Contains(gl)) continue;
                used.Add(gl); map[socks[idx]] = gl;
                Rec(idx + 1, used, map);
                used.Remove(gl); map.Remove(socks[idx]);
            }
        }
        Rec(0, new HashSet<string>(), new Dictionary<int, string>());
        return (bestEval, bestMap);
    }
}
