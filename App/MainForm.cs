using System.Globalization;
using System.Text;
using GuildlandsTree.Core;

namespace GuildlandsTree.App;

// El diseño (controles, posiciones, tamaños) está en MainForm.Designer.cs: se edita en el diseñador de Visual Studio.
public partial class MainForm : Form
{
    sealed record ClassItem(string Key, string Text) { public override string ToString() => Text; }

    readonly TreeData _tree = null!;
    CancellationTokenSource? _cts;
    readonly NumericUpDown[] nudAlloc;          // FUE, AGI, INT, CAR, VIT (puntos repartidos)
    readonly NumericUpDown[] nudGearAttr;       // FUE, AGI, INT, CAR, VIT de equipo
    readonly CheckBox[] chkGlyph;               // vapor, agua, brasa, luz
    readonly Dictionary<string, NumericUpDown> _w;   // pesos del objetivo por clave
    TreeCanvas? _canvas;
    string _clase = "wizard";                   // clase del último enlace/árbol mostrado
    bool _loading = true;

    public MainForm()
    {
        InitializeComponent();
        nudAlloc = new[] { nudFue, nudAgi, nudInt, nudCar, nudVit };
        nudGearAttr = new[] { nudGFue, nudGAgi, nudGInt, nudGCar, nudGVit };
        chkGlyph = new[] { chkVapor, chkAgua, chkBrasa, chkLuz };
        _w = new Dictionary<string, NumericUpDown>
        {
            ["phys"] = nudWPhys, ["magic"] = nudWMagic, ["hp"] = nudWHp, ["armor"] = nudWArmor, ["mana"] = nudWMana,
            ["regen"] = nudWRegen, ["heal"] = nudWHeal, ["evasion"] = nudWEvasion, ["cast"] = nudWCast,
            ["cdr"] = nudWCdr, ["cost"] = nudWCost, ["slots"] = nudWSlot
        };

        // En tiempo de diseño no hay datos ni gráficos: solo se cargan al ejecutar.
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;

        var dir = AppContext.BaseDirectory;
        var json = Path.Combine(dir, "guildlands_tree_v5.json");
        var cls = Path.Combine(dir, "guildlands_classes.json");
        if (!File.Exists(json) || !File.Exists(cls))
        { MessageBox.Show("Faltan guildlands_tree_v5.json o guildlands_classes.json junto al .exe"); Environment.Exit(1); }
        _tree = TreeData.Load(json);
        var cd = ClassData.Load(cls);

        cmbClass.Items.Clear();
        foreach (var kv in cd.Classes.OrderBy(k => k.Value.Name))
            cmbClass.Items.Add(new ClassItem(kv.Key, $"{kv.Value.Name} — {kv.Value.Role}"));
        cmbRace.SelectedItem = "gnome";
        SelectClass("wizard");
        _loading = false;
        ApplyDefaultWeights();

        var render = Path.Combine(dir, "guildlands_render.json");
        if (File.Exists(render))
        {
            _canvas = new TreeCanvas(render, _tree.Combat, TooltipFor) { Dock = DockStyle.Fill };
            _canvas.StartRaces = _tree.Meta.RaceStart.ToDictionary(kv => kv.Value, kv => kv.Key);
            pnlArbol.Controls.Add(_canvas);
            if (_canvas.Mensaje.Length > 0) lblStatus.Text = _canvas.Mensaje;
        }
        else lblStatus.Text = "Falta guildlands_render.json: no se dibujará el árbol.";
        CheckAlloc();
    }

    string? TooltipFor(int id)
    {
        if (!_tree.Nodes.TryGetValue(id, out var n)) return null;
        return string.IsNullOrEmpty(n.Name) ? $"[{id}] {n.Text}" : $"{n.Name}  [{id}]\n{n.Text}";
    }

    string CurrentClass => (cmbClass.SelectedItem as ClassItem)?.Key ?? "wizard";
    void SelectClass(string key)
    {
        for (int i = 0; i < cmbClass.Items.Count; i++)
            if (cmbClass.Items[i] is ClassItem ci && ci.Key == key) { cmbClass.SelectedIndex = i; return; }
    }

    void ApplyDefaultWeights()
    {
        var w = StatModel.DefaultWeights(CurrentClass);
        foreach (var kv in _w) kv.Value.Value = (decimal)(w.TryGetValue(kv.Key, out var v) ? v : 0);
    }

    // ---- eventos (enlazados desde el diseñador) ----
    void MainForm_Load(object? sender, EventArgs e) { }

    /// <summary>Rellena todos los campos con el personaje de la solapa «Ejemplo»: Scorpio, paladín humano nivel 38.</summary>
    void mnuEjemplo_Click(object? sender, EventArgs e)
    {
        _loading = true;
        cmbRace.SelectedItem = "human"; SelectClass("paladin");
        nudLevel.Value = 38; nudDungeons.Value = 5;
        int[] reparto = { 45, 3, 0, 2, 24 };                 // FUE AGI INT CAR VIT puestos (suman 2·(38−1) = 74)
        for (int i = 0; i < 5; i++) nudAlloc[i].Value = reparto[i];
        int[] eq = { 5, 8, 5, 11, 5 };                       // atributos de equipo de la ficha (base + equipo)
        for (int i = 0; i < 5; i++) nudGearAttr[i].Value = eq[i];
        nudGPhys.Value = 38; nudGMagic.Value = 29; nudGMana.Value = 58; nudGHp.Value = 176;
        nudGArmorPct.Value = 18; nudGDefPct.Value = 9; nudGCritPct.Value = 4;
        _loading = false;
        ApplyDefaultWeights();
        PushFicha();
        tabMain.SelectedTab = tabResumen;
        MessageBox.Show("Cargado: Scorpio (Paladín humano, nivel 38, 5 mazmorras) con su equipo.\nPulsa «Generar árbol» para calcular el árbol.", "Personaje de ejemplo");
    }

    // ---- Solapa «Ejemplo del personaje»: ficha.html (HTML/CSS original del juego) dentro de un WebView2 ----
    bool _fichaListo, _fichaIniciando;
    const double FichaAncho = 1400, FichaAlto = 830;      // tamaño de referencia con el que se ve entera

    void tabMain_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (tabMain.SelectedTab == tabFicha && !_fichaListo && !_fichaIniciando) _ = IniciarFichaAsync();
    }

    static string? BuscarRaizFicha()
    {
        for (var d = new DirectoryInfo(AppContext.BaseDirectory); d != null; d = d.Parent)
            if (File.Exists(Path.Combine(d.FullName, "ficha.html")) && Directory.Exists(Path.Combine(d.FullName, "guildlands_assets")))
                return d.FullName;
        return null;
    }

    async Task IniciarFichaAsync()
    {
        _fichaIniciando = true;
        try
        {
            var raiz = BuscarRaizFicha();
            if (raiz == null)
            {
                MessageBox.Show("No encuentro 'ficha.html' junto a la carpeta 'guildlands_assets' (la busco subiendo desde el .exe).", "Ejemplo del personaje");
                return;
            }
            var env = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(null, Path.Combine(Path.GetTempPath(), "GuildlandsTreeOptimizer_WebView2"));
            await wvFicha.EnsureCoreWebView2Async(env);
            var cw = wvFicha.CoreWebView2;
            cw.SetVirtualHostNameToFolderMapping("guildlands.local", raiz, Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            cw.Settings.AreDefaultContextMenusEnabled = false;
            cw.Settings.AreDevToolsEnabled = false;
            cw.Settings.IsZoomControlEnabled = false;
            cw.Settings.IsStatusBarEnabled = false;
            cw.WebMessageReceived += cw_WebMessageReceived;
            cw.NavigationCompleted += (_, _) => { PushFicha(); if (chkEquipoFicha.Checked) _ = RellenarDesdeFicha(false, true); };
            cw.Navigate("https://guildlands.local/ficha.html");
            _fichaListo = true;
            AjustarZoomFicha();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo iniciar el visor de la ficha (WebView2).\n\n" + ex.Message +
                "\n\nInstala el «WebView2 Runtime» de Microsoft: https://go.microsoft.com/fwlink/p/?LinkId=2124703", "Ejemplo del personaje");
        }
        finally { _fichaIniciando = false; }
    }

    static string NombreRaza(string r) => r switch
    {
        "human" => "Humano", "elf" => "Elfo", "orc" => "Orco", "drow" => "Drow", "dwarf" => "Enano", "gnome" => "Gnomo", _ => r
    };

    bool _fichaSync;   // evita el bucle ficha → campos → ficha

    /// <summary>App → ficha: base de clase/raza, puntos repartidos y nivel actuales.</summary>
    void PushFicha()
    {
        if (!_fichaListo || _fichaSync || _loading || wvFicha.CoreWebView2 == null) return;
        try
        {
            var bp = BuildParams(); var ba = GuildlandsTree.Core.StatModel.BaseAttrs(bp);
            string js = string.Format(CultureInfo.InvariantCulture, "GLInit({{base:[{0}],alloc:[{1}],nivel:{2},clase:'{3}',raza:'{4}'}})",
                string.Join(",", Enumerable.Range(0, 5).Select(i => (ba[i] - bp.Alloc[i]).ToString(CultureInfo.InvariantCulture))),
                string.Join(",", bp.Alloc), (int)nudLevel.Value,
                GuildlandsTree.Core.ClassData.Current.Classes[bp.Class].Name.Replace("'", ""), NombreRaza(bp.Race));
            _ = wvFicha.CoreWebView2.ExecuteScriptAsync(js);
        }
        catch { /* combinación raza/clase aún sin elegir */ }
    }

    /// <summary>Ficha → app: al pulsar +/− en un atributo, los «Puntos repartidos» se actualizan solos.</summary>
    void cw_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(e.WebMessageAsJson);
            if (doc.RootElement.TryGetProperty("equipo", out _))
            {
                if (chkEquipoFicha.Checked) _ = RellenarDesdeFicha(false, true);     // el equipo cambió en la ficha
                return;
            }
            var arr = doc.RootElement.GetProperty("alloc");
            _fichaSync = true;
            int i = 0;
            foreach (var x in arr.EnumerateArray())
            {
                if (i < 5) nudAlloc[i].Value = Math.Clamp(x.GetInt32(), (int)nudAlloc[i].Minimum, (int)nudAlloc[i].Maximum);
                i++;
            }
        }
        catch { }
        finally { _fichaSync = false; }
        CheckAlloc();
    }

    void btnFichaAEquipo_Click(object? sender, EventArgs e) => _ = RellenarDesdeFicha(true, false);

    /// <summary>Lee la ficha (objetos + atributos) y rellena Equipo; con conAlloc también deduce el reparto de puntos.</summary>
    async Task RellenarDesdeFicha(bool conAlloc, bool silencioso)
    {
        if (!_fichaListo || wvFicha.CoreWebView2 == null) { if (!silencioso) MessageBox.Show("La ficha aún no está cargada.", "Pasar a Equipo"); return; }
        try
        {
            string raw = await wvFicha.CoreWebView2.ExecuteScriptAsync("JSON.stringify(window.GLEquipo())");
            using var doc = System.Text.Json.JsonDocument.Parse(System.Text.Json.JsonSerializer.Deserialize<string>(raw)!);
            var r = doc.RootElement; var a = r.GetProperty("attrs");
            decimal D(System.Text.Json.JsonElement x, string n) => x.GetProperty(n).GetDecimal();
            _loading = true;
            // orden de los campos: FUE AGI INT CAR VIT
            string[] k = { "fue", "agi", "int", "car", "vit" };
            for (int i = 0; i < 5; i++) nudGearAttr[i].Value = Math.Clamp(D(a, k[i]), nudGearAttr[i].Minimum, nudGearAttr[i].Maximum);
            nudGPhys.Value = Math.Clamp(D(r, "phys"), nudGPhys.Minimum, nudGPhys.Maximum);
            nudGMagic.Value = Math.Clamp(D(r, "magic"), nudGMagic.Minimum, nudGMagic.Maximum);
            nudGMana.Value = Math.Clamp(D(r, "mana"), nudGMana.Minimum, nudGMana.Maximum);
            nudGHp.Value = Math.Clamp(D(r, "hp"), nudGHp.Minimum, nudGHp.Maximum);
            nudGArmorPct.Value = Math.Clamp(D(r, "armorPct"), nudGArmorPct.Minimum, nudGArmorPct.Maximum);
            nudGDefPct.Value = Math.Clamp(D(r, "defPct"), nudGDefPct.Minimum, nudGDefPct.Maximum);
            nudGCritPct.Value = Math.Clamp(D(r, "critPct"), nudGCritPct.Minimum, nudGCritPct.Maximum);
            // puntos repartidos = valor mostrado en la hoja − equipo − base de clase/raza
            if (!conAlloc) { _loading = false; if (silencioso) return; }
            var tot = r.GetProperty("tot");
            var bp = BuildParams(); var ba = GuildlandsTree.Core.StatModel.BaseAttrs(bp);
            var alloc = new int[5]; int sumA = 0;
            for (int i = 0; i < 5; i++)
            {
                double baseI = ba[i] - bp.Alloc[i];
                alloc[i] = Math.Max(0, (int)Math.Round((double)D(tot, k[i]) - (double)D(a, k[i]) - baseI));
                sumA += alloc[i];
            }
            for (int i = 0; i < 5; i++) nudAlloc[i].Value = Math.Clamp(alloc[i], (int)nudAlloc[i].Minimum, (int)nudAlloc[i].Maximum);
            _loading = false;
            CheckAlloc();
            var av = new List<string>();
            if (sumA != 2 * ((int)nudLevel.Value - 1)) av.Add($"• Reparto deducido = {sumA}, el juego espera {2 * ((int)nudLevel.Value - 1)}: comprueba que Raza, Clase y Nivel coinciden con la ficha.");
            foreach (var x in r.GetProperty("avisos").EnumerateArray()) av.Add("• " + x.GetString());
            if (D(r, "resPct") > 0) av.Add($"• Resistencia elemental +{D(r, "resPct")} %: el generador no la usa.");
            if (!silencioso) MessageBox.Show("Cargados el reparto de puntos y el equipo de la ficha." +
                (av.Count > 0 ? "\n\nAvisos:\n" + string.Join("\n", av) : ""), "Pasar a Equipo");
        }
        catch (Exception ex) { _loading = false; if (!silencioso) MessageBox.Show("No se pudo leer el equipo de la ficha.\n\n" + ex.Message, "Pasar a Equipo"); }
    }

    IEnumerable<NumericUpDown> CamposEquipo => nudGearAttr.Concat(new[] { nudGPhys, nudGMagic, nudGMana, nudGHp, nudGArmorPct, nudGDefPct, nudGCritPct });

    void chkEquipoFicha_CheckedChanged(object? sender, EventArgs e)
    {
        foreach (var n in CamposEquipo) n.Enabled = !chkEquipoFicha.Checked;
        if (!chkEquipoFicha.Checked) return;
        if (_fichaListo) _ = RellenarDesdeFicha(false, true);
        else tabMain.SelectedTab = tabFicha;               // al cargar la ficha se rellena sola (NavigationCompleted)
    }

    void wvFicha_SizeChanged(object? sender, EventArgs e) => AjustarZoomFicha();

    void AjustarZoomFicha()
    {
        if (!_fichaListo || wvFicha.Width < 50 || wvFicha.Height < 50) return;
        wvFicha.ZoomFactor = Math.Clamp(Math.Min(wvFicha.Width / FichaAncho, wvFicha.Height / FichaAlto), 0.3, 2.0);
    }
    void nudAlloc_ValueChanged(object? sender, EventArgs e) => CheckAlloc();
    void cmbRace_SelectedIndexChanged(object? sender, EventArgs e)
    {
        bool gnome = (string?)cmbRace.SelectedItem == "gnome";
        chkForce.Enabled = gnome;
        if (!gnome) chkForce.Checked = false;
        PushFicha();
    }
    void cmbClass_SelectedIndexChanged(object? sender, EventArgs e) { if (!_loading) ApplyDefaultWeights(); PushFicha(); }
    void btnPesos_Click(object? sender, EventArgs e) => ApplyDefaultWeights();
    async void btnGo_Click(object? sender, EventArgs e) => await GenerateAsync();
    void btnCancel_Click(object? sender, EventArgs e) => _cts?.Cancel();
    void btnEval_Click(object? sender, EventArgs e) => EvaluateLink();
    void mnuSalir_Click(object? sender, EventArgs e) => Close();
    void mnuComoUsarlo_Click(object? sender, EventArgs e) { using var f = new ManualForm(); f.ShowDialog(this); }
    void mnuAcercaDe_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Guildlands · Optimizador del árbol de pasivas\n" +
        $"Datos del árbol: versión {_tree.Meta.Version} ({_tree.Nodes.Count} nodos), {ClassData.Current.Classes.Count} clases\n" +
        "Herramienta no oficial. Las ponderaciones del score son suposiciones editables, no datos del juego.",
        "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
    void btnAbrir_Click(object? sender, EventArgs e)
    {
        var url = txtUrlOut.Text.Trim();
        if (url.Length == 0) { MessageBox.Show("Todavía no hay enlace generado. Pulsa «Generar árbol» o «Evaluar enlace» primero."); return; }
        try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true }); }
        catch (Exception ex) { MessageBox.Show("No pude abrir el navegador: " + ex.Message); }
    }
    void btnCopy_Click(object? sender, EventArgs e) { if (txtUrlOut.Text.Length > 0) Clipboard.SetText(txtUrlOut.Text); }

    void CheckAlloc()
    {
        int sum = nudAlloc.Sum(n => (int)n.Value), expect = 2 * ((int)nudLevel.Value - 1);
        lblAllocCheck.Text = sum == expect ? $"Reparto: {sum} / {expect} ✓" : $"Reparto: {sum} / {expect} (el juego espera {expect})";
        lblAllocCheck.ForeColor = sum == expect ? Color.DarkGreen : Color.Firebrick;
        PushFicha();
    }

    Params BuildParams()
    {
        var p = new Params
        {
            Class = CurrentClass, Race = (string)cmbRace.SelectedItem!, Level = (int)nudLevel.Value, Dungeons = (int)nudDungeons.Value,
            Alloc = nudAlloc.Select(n => (int)n.Value).ToArray(),
            GearAttr = nudGearAttr.Select(n => (double)n.Value).ToArray(),
            AllowKeys = chkKeys.Checked, TimeLimitSeconds = (int)nudTime.Value,
            Glyphs = chkGlyph.Where(c => c.Checked).Select(c => c.Text).ToList(),
            Force = chkForce.Checked ? new HashSet<int> { 564 } : new HashSet<int>(),
            W = _w.ToDictionary(kv => kv.Key, kv => (double)kv.Value.Value)
        };
        p.GearFlat["physicalDamage"] = (double)nudGPhys.Value; p.GearFlat["magicDamage"] = (double)nudGMagic.Value;
        p.GearFlat["maxMana"] = (double)nudGMana.Value; p.GearFlat["maxHp"] = (double)nudGHp.Value;
        p.GearPct["armorPct"] = (double)nudGArmorPct.Value; p.GearPct["defensePct"] = (double)nudGDefPct.Value;
        p.GearPct["critChancePct"] = (double)nudGCritPct.Value;
        foreach (var part in txtElem.Text.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split('=');
            if (kv.Length == 2 && double.TryParse(kv[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                p.ElemShare[kv[0].Trim()] = v;
        }
        return p;
    }

    async Task GenerateAsync()
    {
        var p = BuildParams();
        btnGo.Enabled = false; btnCancel.Enabled = true;
        _cts = new CancellationTokenSource();
        lblStatus.Text = "Optimizando… (puede tardar hasta el tiempo máximo por iteración)";
        try
        {
            var ct = _cts.Token;
            var sol = await Task.Run(() => Optimizer.Optimize(_tree, p, null, 5,
                msg => BeginInvoke(new Action(() => lblStatus.Text = msg)), ct), ct);
            var (ev, gl) = Optimizer.BestGlyphs(_tree, p, sol.Ids);
            _clase = p.Class;
            ShowResult(p, sol.Ids, ev, gl, null);
            lblStatus.Text = sol.ProvenOptimal ? "Hecho (óptimo del modelo lineal)." : "Hecho (mejor solución encontrada en el tiempo dado; sin prueba de optimalidad).";
        }
        catch (OperationCanceledException) { lblStatus.Text = "Cancelado."; }
        catch (Exception ex) { lblStatus.Text = "Error: " + ex.Message; }
        finally { btnGo.Enabled = true; btnCancel.Enabled = false; }
    }

    void EvaluateLink()
    {
        try
        {
            var u = UrlCodec.ParseUrl(txtUrlIn.Text.Trim());
            if (!ClassData.Current.Classes.ContainsKey(u.Class)) throw new InvalidDataException($"clase desconocida «{u.Class}»");
            bool cambia = CurrentClass != u.Class;
            _loading = true; SelectClass(u.Class); _loading = false;
            if (cambia) ApplyDefaultWeights();          // los pesos deben corresponder a la clase del enlace
            cmbRace.SelectedItem = u.Race; nudLevel.Value = u.Level; nudDungeons.Value = u.Dungeons;
            if (u.Alloc is { Length: 5 }) for (int i = 0; i < 5; i++) nudAlloc[i].Value = u.Alloc[i];
            var p = BuildParams();
            if (u.Version != _tree.Meta.Version)
                MessageBox.Show($"El enlace es de la versión {u.Version} del árbol y los datos son de la {_tree.Meta.Version}. Los IDs pueden no coincidir.");
            _clase = u.Class;
            var (ev, gl) = Optimizer.BestGlyphs(_tree, p, u.Ids);
            ShowResult(p, u.Ids, ev, gl, _tree.IsValid(p, u.Ids));
            lblStatus.Text = "Enlace evaluado.";
        }
        catch (Exception ex) { lblStatus.Text = "No pude leer el enlace: " + ex.Message; }
    }

    void ShowResult(Params p, HashSet<int> ids, Eval e, Dictionary<int, string> gl, bool? valid)
    {
        double m = ClassData.Current.Constants["globalDamageMult"];
        var sb = new StringBuilder();
        sb.AppendLine($"Clase: {ClassData.Current.Classes[_clase].Name}   ·   Raza: {p.Race}   ·   Nivel {p.Level}");
        sb.AppendLine($"Nodos: {ids.Count} / {p.Budget} puntos" + (valid == null ? "" : valid.Value ? "   (build válido)" : "   (¡build NO válido con las reglas!)"));
        sb.AppendLine($"Atributos  FUE {e.Attr["strength"]:F0}  AGI {e.Attr["agility"]:F0}  INT {e.Attr["intelligence"]:F0}  CAR {e.Attr["charisma"]:F0}  VIT {e.Attr["stamina"]:F0}");
        sb.AppendLine($"Daño físico {e.S("physicalDamage") * m,8:F0}   Daño mágico {e.S("magicDamage") * m,8:F0}   (incluye ×{m:0.00} supuesto)");
        sb.AppendLine($"Vida {e.S("maxHp"),8:F0}   Maná {e.S("maxMana"),8:F0}   Armadura {e.S("armor"),7:F1}");
        sb.AppendLine($"Crítico {e.S("critChance"),6:F1}% (x{e.S("critDamage"):F0})   Vel. ataque {e.S("attackSpeed"):F0}   Vel. lanzamiento {e.S("castSpeed"):F0}");
        sb.AppendLine($"Regen. maná {e.S("manaRegen"):F2}   Curación hecha {e.S("healIncrease"):F1}%   Evasión {e.S("evasion"):F1}   Bloqueo {e.S("block"):F0}");
        sb.AppendLine($"Enfriamiento −{e.S("cooldownPct"):F0}%   Coste de maná −{e.S("manaCostPct"):F0}%   Ranuras +{e.Sums.GetValueOrDefault("spellSlots"):F0}");
        sb.AppendLine("Pesos usados: " + string.Join("  ", p.W.Where(kv => kv.Value > 0).Select(kv => $"{kv.Key} {kv.Value:0.##}")));
        sb.AppendLine($"Equipo: FUE {p.GearAttr[0]:0} AGI {p.GearAttr[1]:0} INT {p.GearAttr[2]:0} CAR {p.GearAttr[3]:0} VIT {p.GearAttr[4]:0}, daño fís. +{p.GearFlat.GetValueOrDefault("physicalDamage"):0}, vida +{p.GearFlat.GetValueOrDefault("maxHp"):0}");
        sb.AppendLine($"Score del modelo: {e.Score:F3}  (solo sirve para comparar builds con los mismos pesos)");
        if (gl.Count > 0) sb.AppendLine("Glifos: " + string.Join(", ", gl.Select(kv => $"nexo {kv.Key} = {kv.Value}")));
        rtb.Text = sb.ToString();
        if (_canvas != null)
        {
            var shown = new HashSet<int>(ids) { _tree.Meta.RaceStart[p.Race] };
            _canvas.Race = p.Race;
            _canvas.SetChosen(shown);
            tabMain.SelectedTab = tabArbol;
        }
        txtUrlOut.Text = UrlCodec.BuildUrl(p, _clase, ids, _tree.Meta.Version);
        lv.Items.Clear();
        foreach (var i in ids.OrderBy(i => i))
        {
            var n = _tree.Nodes[i];
            if (n.Type is "notable" or "key" or "choice" or "socket")
                lv.Items.Add(new ListViewItem(new[] { i.ToString(), n.Type, n.Name ?? "", n.Text + (gl.TryGetValue(i, out var g) ? $"  [glifo {g}]" : "") }));
        }
    }
}
