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
    void nudAlloc_ValueChanged(object? sender, EventArgs e) => CheckAlloc();
    void cmbRace_SelectedIndexChanged(object? sender, EventArgs e)
    {
        bool gnome = (string?)cmbRace.SelectedItem == "gnome";
        chkForce.Enabled = gnome;
        if (!gnome) chkForce.Checked = false;
    }
    void cmbClass_SelectedIndexChanged(object? sender, EventArgs e) { if (!_loading) ApplyDefaultWeights(); }
    void btnPesos_Click(object? sender, EventArgs e) => ApplyDefaultWeights();
    async void btnGo_Click(object? sender, EventArgs e) => await GenerateAsync();
    void btnCancel_Click(object? sender, EventArgs e) => _cts?.Cancel();
    void btnEval_Click(object? sender, EventArgs e) => EvaluateLink();
    void mnuSalir_Click(object? sender, EventArgs e) => Close();
    void mnuComoUsarlo_Click(object? sender, EventArgs e) => MessageBox.Show(
        "GENERAR UN ÁRBOL (cualquier clase)\n" +
        "1. Elige raza, clase, nivel, mazmorras y los puntos repartidos (FUE/AGI/INT/CAR/VIT).\n" +
        "2. Rellena la SUMA de lo que dan tus piezas (atributos, daño, vida, % armadura...).\n" +
        "3. Ajusta los pesos: son suposiciones tuyas sobre qué vale más. «Pesos por defecto» los restablece según la clase.\n" +
        "4. Pulsa «Generar árbol». El resultado aparece en la pestaña Árbol y el enlace se copia con «Copiar enlace».\n\n" +
        "EVALUAR UN ENLACE\n" +
        "Pega un enlace del planificador arriba y pulsa «Evaluar enlace»: carga raza, clase, nivel y reparto, comprueba si es válido y lo dibuja.\n\n" +
        "ÁRBOL: arrastra para mover, rueda para zoom, doble clic para ajustar, ratón encima de un nodo para ver su efecto.\n\n" +
        "LÍMITES: las cifras de daño incluyen un factor ×1,14 que se observa en el juego y cuyo origen no conozco. " +
        "Los efectos condicionales (contra bichos con poca vida, armas concretas, auras...) no se valoran.",
        "Cómo usarlo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    void mnuAcercaDe_Click(object? sender, EventArgs e) => MessageBox.Show(
        "Guildlands · Optimizador del árbol de pasivas\n" +
        $"Datos del árbol: versión {_tree.Meta.Version} ({_tree.Nodes.Count} nodos), {ClassData.Current.Classes.Count} clases\n" +
        "Herramienta no oficial. Las ponderaciones del score son suposiciones editables, no datos del juego.",
        "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
    void btnCopy_Click(object? sender, EventArgs e) { if (txtUrlOut.Text.Length > 0) Clipboard.SetText(txtUrlOut.Text); }

    void CheckAlloc()
    {
        int sum = nudAlloc.Sum(n => (int)n.Value), expect = 2 * ((int)nudLevel.Value - 1);
        lblAllocCheck.Text = sum == expect ? $"Reparto: {sum} / {expect} ✓" : $"Reparto: {sum} / {expect} (el juego espera {expect})";
        lblAllocCheck.ForeColor = sum == expect ? Color.DarkGreen : Color.Firebrick;
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
