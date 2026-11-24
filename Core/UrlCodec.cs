using System.Text;

namespace GuildlandsTree.Core;

public static class UrlCodec
{
    const string Digits = "0123456789abcdefghijklmnopqrstuvwxyz";

    public static string B36(int n)
    {
        if (n == 0) return "0";
        var sb = new StringBuilder();
        while (n > 0) { sb.Insert(0, Digits[n % 36]); n /= 36; }
        return sb.ToString();
    }

    /// <summary>Convert.ToInt32 no admite base 36, así que se parsea a mano.</summary>
    public static int FromB36(string s)
    {
        int n = 0;
        foreach (char ch in s.Trim().ToLowerInvariant())
        {
            int d = Digits.IndexOf(ch);
            if (d < 0) throw new FormatException($"Carácter no válido en base 36: '{ch}'");
            n = checked(n * 36 + d);
        }
        return n;
    }

    public static string ToRc(IEnumerable<int> ids) => string.Join(".", ids.OrderBy(i => i).Select(B36));

    public static HashSet<int> FromRc(string rc) =>
        new(rc.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(FromB36));

    /// <summary>at = FUE.AGI.INT.CAR.VIT en base 36 (verificado contra el código del planificador).</summary>
    public static string ToAt(int[] alloc) => string.Join(".", alloc.Select(B36));

    public static string BuildUrl(Params p, string clase, IEnumerable<int> ids, int treeVersion) =>
        $"https://tree.guildlands.com/es/#raza={p.Race}&clase={clase}&v={treeVersion}&nivel={p.Level}" +
        $"&mazmorras={p.Dungeons}&rc={ToRc(ids)}&at={ToAt(p.Alloc)}";

    public sealed record Parsed(string Race, string Class, int Version, int Level, int Dungeons, HashSet<int> Ids, int[]? Alloc);

    public static Parsed ParseUrl(string url)
    {
        int h = url.IndexOf('#');
        var q = (h >= 0 ? url[(h + 1)..] : url).Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(kv => kv.Split('=', 2)).Where(a => a.Length == 2).ToDictionary(a => a[0], a => a[1]);
        string Get(string k, string d = "") => q.TryGetValue(k, out var v) ? v : d;
        int[]? alloc = null;
        if (q.TryGetValue("at", out var at) && at.Length > 0)
            alloc = at.Split('.').Select(FromB36).ToArray();
        return new Parsed(Get("raza", "human"), Get("clase", "warrior"), int.Parse(Get("v", "0")),
            int.Parse(Get("nivel", "1")), int.Parse(Get("mazmorras", "0")),
            FromRc(Get("rc")), alloc);
    }
}
