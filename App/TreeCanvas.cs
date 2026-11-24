using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GuildlandsTree.App;

/// <summary>Control que dibuja el árbol de pasivas con los gráficos originales (arrastrar = mover, rueda = zoom, doble clic = ajustar).</summary>
public sealed class TreeCanvas : Control
{
    // ---- datos de render (guildlands_render.json) ----
    sealed class RNode
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("t")] public string T { get; set; } = "";
        [JsonPropertyName("x")] public double X { get; set; }
        [JsonPropertyName("y")] public double Y { get; set; }
        [JsonPropertyName("a")] public string? Atlas { get; set; }   // "passives" | "icons"
        [JsonPropertyName("c")] public int Cell { get; set; }
    }
    sealed class RBg
    {
        [JsonPropertyName("file")] public string File { get; set; } = "";
        [JsonPropertyName("x")] public double X { get; set; }
        [JsonPropertyName("y")] public double Y { get; set; }
        [JsonPropertyName("lado")] public double Lado { get; set; }
        [JsonPropertyName("unidades")] public double Unidades { get; set; }
    }
    sealed class RLabel
    {
        [JsonPropertyName("n")] public string N { get; set; } = "";
        [JsonPropertyName("x")] public double X { get; set; }
        [JsonPropertyName("y")] public double Y { get; set; }
        [JsonPropertyName("k")] public string K { get; set; } = "z";   // z=zona, cumbre, umbral, o=confín
    }
    sealed class RFile
    {
        [JsonPropertyName("etiquetas")] public List<RLabel> Etiquetas { get; set; } = new();
        [JsonPropertyName("escala")] public double Escala { get; set; } = 2.4;
        [JsonPropertyName("ladoZona")] public double LadoZona { get; set; } = 2;
        [JsonPropertyName("radios")] public Dictionary<string, double> Radios { get; set; } = new();
        [JsonPropertyName("fondoOpacidad")] public double FondoOpacidad { get; set; } = 0.5;
        [JsonPropertyName("mosaico")] public double Mosaico { get; set; } = 512;
        [JsonPropertyName("nodos")] public List<RNode> Nodos { get; set; } = new();
        [JsonPropertyName("aristas")] public int[][] Aristas { get; set; } = Array.Empty<int[]>();
        [JsonPropertyName("fondos")] public List<RBg> Fondos { get; set; } = new();
    }

    readonly RFile _r;
    readonly Dictionary<int, RNode> _byId = new();
    readonly HashSet<int> _combat;
    readonly string? _assets;
    readonly Func<int, string?> _tooltip;
    readonly Dictionary<string, Bitmap?> _img = new();
    readonly ToolTip _tt = new() { InitialDelay = 250, ReshowDelay = 100, AutoPopDelay = 20000 };

    HashSet<int> _chosen = new();
    double _zoom = 0.25, _cx, _cy;          // _cx/_cy: punto del mundo en el centro de la vista
    bool _drag; Point _last; bool _fitted;
    int _hover = -1;

    public string Mensaje { get; private set; } = "";

    // Cabeza del nodo de inicio: hoja de cabezas (4 vistas de 23x30) + cuerpo (armor000_01, celdas de 32x64).
    static readonly string[] RaceOrder = { "human", "elf", "orc", "drow", "dwarf", "gnome" };
    static readonly int[] HeadSheet = { 1, 21, 41, 61, 81, 101 };
    string _race = "human";
    Bitmap? _startSprite; string? _startSpriteRace;
    public string Race { get => _race; set { _race = value; Invalidate(); } }

    Bitmap? StartSprite()
    {
        if (_startSprite != null && _startSpriteRace == _race) return _startSprite;
        _startSprite = null; _startSpriteRace = _race;
        int i = Array.IndexOf(RaceOrder, _race); if (i < 0) i = 0;
        var head = Img($"head/head{HeadSheet[i]}.png");
        var body = Img("body/armor000_01.png");
        if (head == null) return null;
        var bmp = new Bitmap(32, 32, PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.NearestNeighbor; g.PixelOffsetMode = PixelOffsetMode.Half;
        const int cropY = 9;                                   // recorte: cabeza + hombros
        if (body != null)
            g.DrawImage(body, new Rectangle(0, -cropY, 32, 64), new Rectangle(0, 0, 32, 64), GraphicsUnit.Pixel);
        g.DrawImage(head, new Rectangle(4, 7 - cropY, 23, 30), new Rectangle(0, 0, 23, 30), GraphicsUnit.Pixel); // vista frontal
        _startSprite = bmp;
        return bmp;
    }

    public TreeCanvas(string renderJson, IEnumerable<int> combatIds, Func<int, string?> tooltip)
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer
                 | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
        BackColor = Color.FromArgb(10, 11, 16);
        _tooltip = tooltip;
        _combat = new HashSet<int>(combatIds);
        _r = JsonSerializer.Deserialize<RFile>(File.ReadAllText(renderJson)) ?? throw new InvalidDataException("render vacío");
        foreach (var n in _r.Nodos) _byId[n.Id] = n;
        _assets = FindAssets();
        if (_assets == null)
            Mensaje = "No encuentro la carpeta 'guildlands_assets' (la busco subiendo desde el .exe). Dibujo sin imágenes.";
    }

    static string? FindAssets()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var p = Path.Combine(dir.FullName, "guildlands_assets");
            if (Directory.Exists(p)) return p;
            dir = dir.Parent;
        }
        return null;
    }

    // ---- API pública ----
    public void SetChosen(IEnumerable<int> ids)
    {
        _chosen = new HashSet<int>(ids);
        FitToChosen();
        Invalidate();
    }

    public void FitToChosen()
    {
        var pts = (_chosen.Count > 0 ? _chosen : _combat).Where(_byId.ContainsKey).Select(i => _byId[i]).ToList();
        if (pts.Count == 0 || Width < 10 || Height < 10) return;
        double k = _r.Escala, pad = 60;
        double minX = pts.Min(n => n.X) * k - pad, maxX = pts.Max(n => n.X) * k + pad;
        double minY = pts.Min(n => n.Y) * k - pad, maxY = pts.Max(n => n.Y) * k + pad;
        _cx = (minX + maxX) / 2; _cy = (minY + maxY) / 2;
        _zoom = Math.Max(0.05, Math.Min(Width / (maxX - minX), Height / (maxY - minY)));
        _fitted = true;
    }

    // ---- imágenes ----
    Bitmap? Img(string relPath)
    {
        if (_assets == null) return null;
        if (_img.TryGetValue(relPath, out var b)) return b;
        try
        {
            var f = Path.Combine(_assets, relPath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(f))
            {
                using var fs = File.OpenRead(f);
                using var tmp = new Bitmap(fs);
                b = new Bitmap(tmp);          // copia: no deja el archivo bloqueado
            }
        }
        catch { b = null; }
        _img[relPath] = b;
        return b;
    }

    // ---- dibujo ----
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (!_fitted) FitToChosen();
    }

    PointF ToScreen(double wx, double wy) =>
        new((float)((wx - _cx) * _zoom + Width / 2.0), (float)((wy - _cy) * _zoom + Height / 2.0));
    (double, double) ToWorld(Point p) => ((p.X - Width / 2.0) / _zoom + _cx, (p.Y - Height / 2.0) / _zoom + _cy);

    protected override void OnPaint(PaintEventArgs pe)
    {
        var g = pe.Graphics;
        g.Clear(BackColor);
        if (_r == null) return;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        double k = _r.Escala;

        // vista en coordenadas de mundo
        double wl = _cx - Width / 2.0 / _zoom, wr = _cx + Width / 2.0 / _zoom;
        double wt = _cy - Height / 2.0 / _zoom, wb = _cy + Height / 2.0 / _zoom;

        g.TranslateTransform((float)(Width / 2.0 - _cx * _zoom), (float)(Height / 2.0 - _cy * _zoom));
        g.ScaleTransform((float)_zoom, (float)_zoom);

        DrawBackground(g, wl, wr, wt, wb, k);
        DrawEdges(g, k, wl, wr, wt, wb);
        DrawNodes(g, k, wl, wr, wt, wb);
        DrawLabels(g, k, wl, wr, wt, wb);
        g.ResetTransform();

        if (_hover >= 0) DrawCartel(g);
        if (Mensaje.Length > 0)
            g.DrawString(Mensaje, Font, Brushes.OrangeRed, 8, 8);
        g.DrawString($"zoom {_zoom:P0}  ·  arrastra para mover · rueda = zoom · doble clic = ajustar", Font, Brushes.Gray, 8, Height - 20);
    }

    void DrawBackground(Graphics g, double wl, double wr, double wt, double wb, double k)
    {
        var tile = Img("ui/passive-tree/background.png");
        double ts = _r.Mosaico, lim = 3200;
        var old = g.InterpolationMode; var oldPix = g.PixelOffsetMode;
        g.InterpolationMode = InterpolationMode.NearestNeighbor; g.PixelOffsetMode = PixelOffsetMode.Half;
        if (tile != null)
        {
            for (double y = Math.Floor(Math.Max(wt, -lim) / ts) * ts; y < Math.Min(wb, lim); y += ts)
                for (double x = Math.Floor(Math.Max(wl, -lim) / ts) * ts; x < Math.Min(wr, lim); x += ts)
                    g.DrawImage(tile, new RectangleF((float)x, (float)y, (float)ts + 0.5f, (float)ts + 0.5f));
        }
        // viñeta oscura hacia los bordes
        using (var path = new GraphicsPath())
        {
            double R = 900 * k;
            path.AddEllipse((float)-R, (float)-R, (float)(2 * R), (float)(2 * R));
            using var br = new PathGradientBrush(path)
            {
                CenterColor = Color.FromArgb(0, 13, 14, 21),
                SurroundColors = new[] { Color.FromArgb(215, 6, 7, 11) },
                CenterPoint = new PointF(0, 0)
            };
            br.FocusScales = new PointF(0.7f, 0.7f);
            g.FillEllipse(br, (float)-R, (float)-R, (float)(2 * R), (float)(2 * R));
        }
        // imágenes de zona (semitransparentes)
        using var ia = new ImageAttributes();
        ia.SetColorMatrix(new ColorMatrix { Matrix33 = (float)_r.FondoOpacidad }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        foreach (var f in _r.Fondos)
        {
            var bmp = Img($"ui/passive-tree/{f.File}.png");
            if (bmp == null) continue;
            double d = f.Unidades > 0 ? f.Unidades * k : f.Lado * _r.LadoZona;
            double x = f.X * k - d / 2, y = f.Y * k - d / 2;
            if (x > wr || x + d < wl || y > wb || y + d < wt) continue;
            g.DrawImage(bmp, new Rectangle((int)x, (int)y, (int)d, (int)d), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
        }
        g.InterpolationMode = old; g.PixelOffsetMode = oldPix;
    }

    void DrawEdges(Graphics g, double k, double wl, double wr, double wt, double wb)
    {
        bool any = _chosen.Count > 0;
        using var dim = new Pen(Color.FromArgb(any ? 70 : 140, 150, 140, 120), 2.2f);
        using var on = new Pen(Color.FromArgb(255, 232, 190, 90), 4.5f);
        foreach (var e in _r.Aristas)
        {
            int a = e[0], b = e[1];
            if (!_combat.Contains(a) || !_combat.Contains(b)) continue;
            var na = _byId[a]; var nb = _byId[b];
            double ax = na.X * k, ay = na.Y * k, bx = nb.X * k, by = nb.Y * k;
            if ((ax < wl && bx < wl) || (ax > wr && bx > wr) || (ay < wt && by < wt) || (ay > wb && by > wb)) continue;
            bool lit = _chosen.Contains(a) && _chosen.Contains(b);
            g.DrawLine(lit ? on : dim, (float)ax, (float)ay, (float)bx, (float)by);
        }
    }

    // Tamaños de icono del original (Z5) y radios (ks)
    static readonly Dictionary<string, double> IconSize = new()
    { ["travel"] = 10, ["minor"] = 11, ["notable"] = 17, ["key"] = 22, ["socket"] = 13, ["start"] = 20, ["mstart"] = 15, ["choice"] = 10, ["bigsocket"] = 15 };
    static readonly Dictionary<string, double> Radii = new()
    { ["minor"] = 8, ["travel"] = 7, ["notable"] = 13, ["key"] = 17, ["socket"] = 9.5, ["start"] = 21, ["mstart"] = 11, ["choice"] = 8, ["bigsocket"] = 15 };
    static double RadiusOf(string t) => Radii.TryGetValue(t, out var r) ? r : 8;

    /// <summary>Id del nodo de salida → raza (lo rellena MainForm desde el meta del árbol).</summary>
    public Dictionary<int, string> StartRaces { get; set; } = new();
    static readonly Dictionary<string, Color> RaceTint = new()
    { ["human"] = Color.FromArgb(240, 200, 90), ["orc"] = Color.FromArgb(200, 90, 70), ["dwarf"] = Color.FromArgb(225, 160, 60),
      ["gnome"] = Color.FromArgb(90, 190, 200), ["elf"] = Color.FromArgb(120, 200, 90), ["drow"] = Color.FromArgb(170, 100, 210) };

    readonly Dictionary<string, Bitmap?> _faces = new();
    /// <summary>Cara de la raza: solo la cabeza (vista frontal), recortada al rectángulo de píxeles no transparentes, como hace el original.</summary>
    Bitmap? Face(string race)
    {
        if (_faces.TryGetValue(race, out var f)) return f;
        int i = Array.IndexOf(RaceOrder, race); if (i < 0) i = 0;
        Bitmap? res = null;
        var sheet = Img($"head/head{HeadSheet[i]}.png");
        if (sheet != null)
        {
            int minX = 99, minY = 99, maxX = -1, maxY = -1;
            for (int y = 0; y < Math.Min(30, sheet.Height); y++)
                for (int x = 0; x < Math.Min(23, sheet.Width); x++)
                    if (sheet.GetPixel(x, y).A > 0) { minX = Math.Min(minX, x); maxX = Math.Max(maxX, x); minY = Math.Min(minY, y); maxY = Math.Max(maxY, y); }
            if (maxX >= 0)
                res = sheet.Clone(new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1), PixelFormat.Format32bppArgb);
        }
        _faces[race] = res;
        return res;
    }

    static PointF[] Star(double cx, double cy, int points, double outer, double inner, double rot = 0)
    {
        var p = new PointF[points * 2];
        for (int i = 0; i < points * 2; i++)
        {
            double a = rot + Math.PI * i / points, r = i % 2 == 1 ? inner : outer;
            p[i] = new PointF((float)(cx + r * Math.Cos(a)), (float)(cy + r * Math.Sin(a)));
        }
        return p;
    }
    static PointF[] Hexagon(double cx, double cy, double r)
    {
        var p = new PointF[6];
        for (int i = 0; i < 6; i++) { double a = -Math.PI / 2 + Math.PI / 3 * i; p[i] = new PointF((float)(cx + r * Math.Cos(a)), (float)(cy + r * Math.Sin(a))); }
        return p;
    }

    void DrawStart(Graphics g, RNode n, double x, double y, bool on, bool faded)
    {
        string race = StartRaces.TryGetValue(n.Id, out var rr) ? rr : "human";
        bool own = race == _race;
        var tint = RaceTint.TryGetValue(race, out var tc) ? tc : Color.Gold;
        int al = own ? 255 : (on ? 255 : 130);
        double i = RadiusOf("start");
        // luz
        using (var gp = new GraphicsPath())
        {
            gp.AddEllipse((float)(x - (i + 22)), (float)(y - (i + 22)), (float)(2 * (i + 22)), (float)(2 * (i + 22)));
            using var pb = new PathGradientBrush(gp)
            { CenterColor = Color.FromArgb(own ? 150 : 70, tint), SurroundColors = new[] { Color.FromArgb(0, tint) } };
            g.FillPath(pb, gp);
        }
        // rayos
        using (var br = new SolidBrush(Color.FromArgb((int)(al * 0.55), tint)))
            g.FillPolygon(br, Star(x, y, 12, i + 17, i + 4));
        using (var br = new SolidBrush(Color.FromArgb((int)(al * 0.8), tint)))
            g.FillPolygon(br, Star(x, y, 8, i + 11, i + 3, Math.PI / 8));
        // aro + disco
        float r1 = (float)(i + 1), r2 = (float)(i - 2);
        using (var br = new SolidBrush(Color.FromArgb(255, 18, 17, 24))) g.FillEllipse(br, (float)(x - r2), (float)(y - r2), 2 * r2, 2 * r2);
        // cara recortada en círculo
        var face = Face(race);
        if (face != null)
        {
            var st = g.Save();
            using (var clip = new GraphicsPath()) { clip.AddEllipse((float)(x - r2), (float)(y - r2), 2 * r2, 2 * r2); g.SetClip(clip, CombineMode.Intersect); }
            var oi = g.InterpolationMode; var op = g.PixelOffsetMode;
            g.InterpolationMode = InterpolationMode.NearestNeighbor; g.PixelOffsetMode = PixelOffsetMode.Half;
            double box = 2 * i - 6, sc = Math.Min(box / face.Width, box / face.Height);
            double w = face.Width * sc, h = face.Height * sc;
            g.DrawImage(face, new RectangleF((float)(x - w / 2), (float)(y - h / 2), (float)w, (float)h));
            g.InterpolationMode = oi; g.PixelOffsetMode = op;
            g.Restore(st);
        }
        using (var pen = new Pen(Color.FromArgb(al, own || on ? Color.FromArgb(250, 235, 170) : Color.FromArgb(200, 195, 185)), own ? 3f : 2.2f))
            g.DrawEllipse(pen, (float)(x - r1), (float)(y - r1), 2 * r1, 2 * r1);
        // 4 gemas en rombo
        using (var br = new SolidBrush(Color.FromArgb(al, tint)))
            for (int k = 0; k < 4; k++)
            {
                double m = Math.PI / 2 * k - Math.PI / 2, hx = x + (i + 1) * Math.Cos(m), hy = y + (i + 1) * Math.Sin(m);
                g.FillPolygon(br, new[] { new PointF((float)hx, (float)(hy - 4.5)), new PointF((float)(hx + 4.5), (float)hy), new PointF((float)hx, (float)(hy + 4.5)), new PointF((float)(hx - 4.5), (float)hy) });
            }
        if (n.Id == _hover) { using var ph = new Pen(Color.White, 2f); g.DrawEllipse(ph, (float)(x - r1 - 3), (float)(y - r1 - 3), 2 * (r1 + 3), 2 * (r1 + 3)); }
    }

    void DrawNodes(Graphics g, double k, double wl, double wr, double wt, double wb)
    {
        var atlasP = Img("ui/icons/atlas-passives.png");
        var atlasI = Img("ui/icons/atlas-icons.png");
        bool any = _chosen.Count > 0;
        var oldI = g.InterpolationMode;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        using var dimAttr = new ImageAttributes();
        dimAttr.SetColorMatrix(new ColorMatrix { Matrix33 = 0.45f }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        foreach (var n in _r.Nodos)
        {
            if (!_combat.Contains(n.Id)) continue;
            double x = n.X * k, y = n.Y * k, rad = RadiusOf(n.T);
            if (x + rad + 45 < wl || x - rad - 45 > wr || y + rad + 45 < wt || y - rad - 45 > wb) continue;
            bool on = _chosen.Contains(n.Id), faded = any && !on;

            if (n.T == "start") { DrawStart(g, n, x, y, on, faded); continue; }

            Color ring = n.T switch
            {
                "key" => Color.FromArgb(255, 255, 150, 60),
                "notable" => Color.FromArgb(255, 205, 175, 95),
                "socket" or "bigsocket" => Color.FromArgb(255, 120, 200, 230),
                "choice" => Color.FromArgb(255, 170, 130, 220),
                "mstart" => Color.FromArgb(255, 200, 150, 235),
                _ => Color.FromArgb(255, 150, 145, 135)
            };
            if (on) ring = Color.FromArgb(255, 245, 205, 100);
            else if (faded) ring = Color.FromArgb(120, ring);
            float rw = on ? 2.6f : (n.T is "notable" or "key" ? 2.2f : 1.5f);
            int fillA = faded ? 170 : 240;

            // forma según tipo (como el original)
            using var path = new GraphicsPath();
            float fx = (float)x, fy = (float)y, fr = (float)rad;
            switch (n.T)
            {
                case "socket": path.AddPolygon(new[] { new PointF(fx, fy - fr), new PointF(fx + fr, fy), new PointF(fx, fy + fr), new PointF(fx - fr, fy) }); break;
                case "mstart": path.AddRectangle(new RectangleF(fx - fr, fy - fr, 2 * fr, 2 * fr)); break;
                case "bigsocket": path.AddPolygon(Hexagon(x, y, rad)); break;
                default: path.AddEllipse(fx - fr, fy - fr, 2 * fr, 2 * fr); break;
            }
            if (n.T == "key")                                   // aro exterior de la piedra angular
                using (var po = new Pen(Color.FromArgb(faded ? 90 : 230, on ? Color.FromArgb(245, 205, 100) : Color.FromArgb(255, 150, 60)), 1.6f))
                    g.DrawEllipse(po, fx - fr - 6, fy - fr - 6, 2 * (fr + 6), 2 * (fr + 6));
            if (n.T == "bigsocket")
                using (var po = new Pen(Color.FromArgb(faded ? 90 : 230, ring), 1.6f))
                    g.DrawPolygon(po, Hexagon(x, y, rad + 6));
            using (var fill = new SolidBrush(Color.FromArgb(fillA, 20, 21, 29))) g.FillPath(fill, path);

            var atlas = n.Atlas == "passives" ? atlasP : atlasI;
            if (atlas != null && n.Cell >= 0)
            {
                int cols = 16, side = 32;
                var src = new Rectangle(n.Cell % cols * side, n.Cell / cols * side, side, side);
                if (src.Right <= atlas.Width && src.Bottom <= atlas.Height)
                {
                    double isz = IconSize.TryGetValue(n.T, out var z) ? z : 11;
                    var dst = new Rectangle((int)Math.Round(x - isz / 2), (int)Math.Round(y - isz / 2), (int)Math.Round(isz), (int)Math.Round(isz));
                    if (faded) g.DrawImage(atlas, dst, src.X, src.Y, src.Width, src.Height, GraphicsUnit.Pixel, dimAttr);
                    else g.DrawImage(atlas, dst, src, GraphicsUnit.Pixel);
                }
            }
            using (var pen = new Pen(ring, rw)) g.DrawPath(pen, path);
            if (n.Id == _hover) { using var ph = new Pen(Color.White, 2f); g.DrawEllipse(ph, fx - fr - 3, fy - fr - 3, 2 * (fr + 3), 2 * (fr + 3)); }
        }
        g.InterpolationMode = oldI;
    }

    // Nombres de zona (LA CRUZADA, EL COTO...) en tipografía pixel con letras espaciadas
    void DrawLabels(Graphics g, double k, double wl, double wr, double wt, double wb)
    {
        LoadFonts();
        var oldT = g.TextRenderingHint; g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        var sf = StringFormat.GenericTypographic; sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
        foreach (var l in _r.Etiquetas)
        {
            double x = l.X * k, y = l.Y * k;
            float size = l.K switch { "cumbre" => 17f, "umbral" => 15f, "o" => 12f, _ => 14f };
            float gap = size * 0.28f;
            Color col = l.K switch
            {
                "cumbre" => Color.FromArgb(235, 232, 200, 120), "umbral" => Color.FromArgb(225, 190, 170, 220),
                "o" => Color.FromArgb(170, 185, 180, 170), _ => Color.FromArgb(235, 232, 226, 205)
            };
            string txt = l.N.ToUpperInvariant();
            using var font = _ffTipo != null ? new Font(_ffTipo, size, FontStyle.Bold, GraphicsUnit.Pixel) : new Font("Consolas", size, FontStyle.Bold, GraphicsUnit.Pixel);
            var widths = txt.Select(c => g.MeasureString(c.ToString(), font, 1000, sf).Width).ToArray();
            double total = widths.Sum() + gap * (txt.Length - 1);
            if (x + total / 2 < wl || x - total / 2 > wr || y + size < wt || y - size > wb) continue;
            double cx = x - total / 2;
            using var sh = new SolidBrush(Color.FromArgb(230, 8, 8, 12));
            using var br = new SolidBrush(col);
            for (int i = 0; i < txt.Length; i++)
            {
                float px = (float)cx, py = (float)(y - size / 2);
                g.DrawString(txt[i].ToString(), font, sh, px + 1.5f, py + 1.5f, sf);
                g.DrawString(txt[i].ToString(), font, br, px, py, sf);
                cx += widths[i] + gap;
            }
        }
        g.TextRenderingHint = oldT;
    }

    // ---- cartel (popup) con el estilo pergamino del original ----
    Point _mouse;
    PrivateFontCollection? _fonts; FontFamily? _ffTitulo, _ffTipo;
    static readonly Color Pergamino = Color.FromArgb(246, 230, 184), PergaminoOsc = Color.FromArgb(232, 211, 160),
        Marco = Color.FromArgb(104, 66, 34), Cobre = Color.FromArgb(205, 134, 58), Tinta = Color.FromArgb(52, 38, 26), Tenue = Color.FromArgb(118, 100, 80);

    void LoadFonts()
    {
        if (_fonts != null || _assets == null) { _fonts ??= new PrivateFontCollection(); return; }
        _fonts = new PrivateFontCollection();
        var dir = Path.Combine(_assets, "fonts");
        if (Directory.Exists(dir))
            foreach (var f in Directory.GetFiles(dir, "*.ttf")) { try { _fonts.AddFontFile(f); } catch { } }
        foreach (var ff in _fonts.Families)
        {
            if (ff.Name.StartsWith("Jersey 20")) _ffTitulo = ff;
            else if (ff.Name.StartsWith("Silkscreen")) _ffTipo = ff;
        }
    }

    static string TipoNodo(string t) => t switch
    {
        "start" => "Salida", "key" => "Piedra angular", "notable" => "Notable", "choice" => "Elección",
        "socket" or "bigsocket" => "Nexo", "mstart" => "Maestría", _ => "Pasiva"
    };

    /// <summary>Dibuja una imagen como marco 9-slice: esquinas de 'slice' px escaladas ×scale, bordes estirados, centro de color liso.</summary>
    static void NineSlice(Graphics g, Bitmap img, Rectangle d, int slice, int scale)
    {
        var oi = g.InterpolationMode; var op = g.PixelOffsetMode;
        g.InterpolationMode = InterpolationMode.NearestNeighbor; g.PixelOffsetMode = PixelOffsetMode.Half;
        int w = img.Width, h = img.Height, c = slice * scale;
        var cen = img.GetPixel(w / 2 - 6, h / 2 + 6);
        using (var b = new SolidBrush(cen)) g.FillRectangle(b, d.X + c, d.Y + c, d.Width - 2 * c, d.Height - 2 * c);
        void P(int sx, int sy, int sw, int sh, int dx, int dy, int dw, int dh) =>
            g.DrawImage(img, new Rectangle(dx, dy, dw, dh), sx, sy, sw, sh, GraphicsUnit.Pixel);
        int mw = w - 2 * slice, mh = h - 2 * slice;
        P(0, 0, slice, slice, d.X, d.Y, c, c);
        P(w - slice, 0, slice, slice, d.Right - c, d.Y, c, c);
        P(0, h - slice, slice, slice, d.X, d.Bottom - c, c, c);
        P(w - slice, h - slice, slice, slice, d.Right - c, d.Bottom - c, c, c);
        P(slice, 0, mw, slice, d.X + c, d.Y, d.Width - 2 * c, c);
        P(slice, h - slice, mw, slice, d.X + c, d.Bottom - c, d.Width - 2 * c, c);
        P(0, slice, slice, mh, d.X, d.Y + c, c, d.Height - 2 * c);
        P(w - slice, slice, slice, mh, d.Right - c, d.Y + c, c, d.Height - 2 * c);
        g.InterpolationMode = oi; g.PixelOffsetMode = op;
    }

    void DrawCartel(Graphics g)
    {
        var raw = _tooltip(_hover); if (string.IsNullOrEmpty(raw) || !_byId.TryGetValue(_hover, out var n)) return;
        LoadFonts();
        var lines = raw.Split('\n', 2);
        string title = lines.Length > 1 ? lines[0] : TipoNodo(n.T), body = lines.Length > 1 ? lines[1] : lines[0];
        var oldS = g.SmoothingMode; var oldT = g.TextRenderingHint;
        g.SmoothingMode = SmoothingMode.None; g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        using var fTitle = _ffTitulo != null ? new Font(_ffTitulo, 15f, FontStyle.Regular, GraphicsUnit.Pixel) : new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
        using var fTipo = _ffTipo != null ? new Font(_ffTipo, 10f, FontStyle.Bold, GraphicsUnit.Pixel) : new Font("Segoe UI", 10.5f, FontStyle.Bold, GraphicsUnit.Pixel);
        using var fBody = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Pixel);
        const int pad = 16, icon = 40, w = 310;
        int textW = w - 2 * pad;
        var bs = g.MeasureString(body, fBody, textW);
        int h = (int)(pad + icon + 8 + 1 + 8 + bs.Height + pad);
        int x = _mouse.X + 18, y = _mouse.Y + 18;
        if (x + w > Width - 4) x = _mouse.X - w - 12;
        if (y + h > Height - 4) y = Height - h - 4;
        if (x < 4) x = 4; if (y < 4) y = 4;
        var r = new Rectangle(x, y, w, h);

        using (var sh = new SolidBrush(Color.FromArgb(90, 0, 0, 0))) g.FillRectangle(sh, r.X + 4, r.Y + 5, r.Width, r.Height);
        var frame = Img("ui/frames/parchment-small.png");   // 48x48: marco de 6 px con remaches de cobre (9-slice)
        if (frame != null) NineSlice(g, frame, r, 6, 2);
        else
        {
            using (var br = new LinearGradientBrush(r, Pergamino, PergaminoOsc, 90f)) g.FillRectangle(br, r);
            using (var p = new Pen(Marco, 3)) g.DrawRectangle(p, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
        }

        // icono enmarcado
        var ir = new Rectangle(r.X + pad, r.Y + pad, icon, icon);
        using (var b = new SolidBrush(Color.FromArgb(24, 22, 28))) g.FillRectangle(b, ir);
        var oldI = g.InterpolationMode; g.InterpolationMode = InterpolationMode.NearestNeighbor; g.PixelOffsetMode = PixelOffsetMode.Half;
        var sp = n.T == "start" ? Face(StartRaces.TryGetValue(n.Id, out var rc) ? rc : "human") : null;
        if (sp != null)
        {
            double sc = Math.Min((ir.Width - 6.0) / sp.Width, (ir.Height - 6.0) / sp.Height);
            int dw = (int)(sp.Width * sc), dh = (int)(sp.Height * sc);
            g.DrawImage(sp, new Rectangle(ir.X + (ir.Width - dw) / 2, ir.Y + (ir.Height - dh) / 2, dw, dh));
        }
        else
        {
            var atlas = Img(n.Atlas == "passives" ? "ui/icons/atlas-passives.png" : "ui/icons/atlas-icons.png");
            if (atlas != null && n.Cell >= 0)
            {
                var src = new Rectangle(n.Cell % 16 * 32, n.Cell / 16 * 32, 32, 32);
                if (src.Right <= atlas.Width && src.Bottom <= atlas.Height) g.DrawImage(atlas, ir, src, GraphicsUnit.Pixel);
            }
        }
        g.InterpolationMode = oldI;
        using (var p = new Pen(Cobre, 2)) g.DrawRectangle(p, ir.X, ir.Y, ir.Width - 1, ir.Height - 1);

        // título y tipo
        int tx = ir.Right + 10;
        var tipo = TipoNodo(n.T);
        var ts = g.MeasureString(tipo, fTipo);
        using (var b = new SolidBrush(Tenue)) g.DrawString(tipo, fTipo, b, r.Right - pad - ts.Width, ir.Y + 4);
        var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
        using (var b = new SolidBrush(Tinta))
            g.DrawString(title, fTitle, b, new RectangleF(tx, ir.Y + 2, r.Right - pad - ts.Width - 8 - tx, 22), sf);

        // separador y descripción
        int ly = ir.Bottom + 8;
        using (var p = new Pen(Color.FromArgb(140, Marco), 1)) g.DrawLine(p, r.X + pad, ly, r.Right - pad, ly);
        using (var b = new SolidBrush(Tinta)) g.DrawString(body, fBody, b, new RectangleF(r.X + pad, ly + 8, textW, bs.Height + 2));
        g.SmoothingMode = oldS; g.TextRenderingHint = oldT;
    }

    // ---- interacción ----
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e); Focus();
        if (e.Button == MouseButtons.Left) { _drag = true; _last = e.Location; Cursor = Cursors.SizeAll; }
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e); _drag = false; Cursor = Cursors.Default;
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _mouse = e.Location;
        if (_drag)
        {
            _cx -= (e.X - _last.X) / _zoom; _cy -= (e.Y - _last.Y) / _zoom; _last = e.Location;
            Invalidate(); return;
        }
        var (wx, wy) = ToWorld(e.Location);
        double k = _r.Escala, best = double.MaxValue; int hit = -1;
        foreach (var n in _r.Nodos)
        {
            if (!_combat.Contains(n.Id)) continue;
            double dx = n.X * k - wx, dy = n.Y * k - wy, d = dx * dx + dy * dy;
            double rad = RadiusOf(n.T) + 5;
            if (d <= rad * rad && d < best) { best = d; hit = n.Id; }
        }
        if (hit != _hover || hit >= 0)
        {
            _hover = hit; Invalidate();
        }
    }
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        var (wx, wy) = ToWorld(e.Location);
        double f = e.Delta > 0 ? 1.2 : 1 / 1.2;
        _zoom = Math.Min(3, Math.Max(0.05, _zoom * f));
        // mantener el punto bajo el cursor fijo
        _cx = wx - (e.X - Width / 2.0) / _zoom; _cy = wy - (e.Y - Height / 2.0) / _zoom;
        Invalidate();
    }
    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
        base.OnMouseDoubleClick(e); FitToChosen(); Invalidate();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) { foreach (var b in _img.Values) b?.Dispose(); _tt.Dispose(); }
        base.Dispose(disposing);
    }
}
