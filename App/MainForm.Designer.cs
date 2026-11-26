#nullable disable
namespace GuildlandsTree.App
{
    partial class MainForm
    {

        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuMain;
        private ToolStripMenuItem mnuArchivo, mnuEjemplo, mnuSalir, mnuAyuda, mnuComoUsarlo, mnuAcercaDe;
        private Panel pnlLeft, pnlCampos, pnlBotones, pnlRight, pnlEnlace, pnlArbol;
        private Button btnGo, btnCancel, btnEval, btnCopy, btnAbrir;
        private Label lblEnlaceIn, lblEnlaceOut, lblStatus;
        private TextBox txtUrlIn, txtUrlOut;
        private TabControl tabMain;
        private TabPage tabArbol, tabResumen, tabNodos, tabFicha;
        private Microsoft.Web.WebView2.WinForms.WebView2 wvFicha;
        private Panel pnlFicha;
        private Button btnFichaAEquipo;
        private Label lblFichaInfo;
        private RichTextBox rtb;
        private ListView lv;
        private ColumnHeader colId, colTipo, colNombre, colEfecto;
        private Label lblSecPersonaje;
        private Label lblRace;
        private ComboBox cmbRace;
        private Label lblClass;
        private ComboBox cmbClass;
        private Label lblLevel;
        private NumericUpDown nudLevel;
        private Label lblDungeons;
        private NumericUpDown nudDungeons;
        private Label lblFue;
        private NumericUpDown nudFue;
        private Label lblAgi;
        private NumericUpDown nudAgi;
        private Label lblInt;
        private NumericUpDown nudInt;
        private Label lblCar;
        private NumericUpDown nudCar;
        private Label lblVit;
        private NumericUpDown nudVit;
        private Label lblAllocCheck;
        private Label lblSecEquipo;
        private CheckBox chkEquipoFicha;
        private Label lblGFue;
        private NumericUpDown nudGFue;
        private Label lblGAgi;
        private NumericUpDown nudGAgi;
        private Label lblGInt;
        private NumericUpDown nudGInt;
        private Label lblGCar;
        private NumericUpDown nudGCar;
        private Label lblGVit;
        private NumericUpDown nudGVit;
        private Label lblGPhys;
        private NumericUpDown nudGPhys;
        private Label lblGMagic;
        private NumericUpDown nudGMagic;
        private Label lblGMana;
        private NumericUpDown nudGMana;
        private Label lblGHp;
        private NumericUpDown nudGHp;
        private Label lblGArmorPct;
        private NumericUpDown nudGArmorPct;
        private Label lblGDefPct;
        private NumericUpDown nudGDefPct;
        private Label lblGCritPct;
        private NumericUpDown nudGCritPct;
        private Label lblSecPesos;
        private Button btnPesos;
        private Label lblWPhys;
        private NumericUpDown nudWPhys;
        private Label lblWMagic;
        private NumericUpDown nudWMagic;
        private Label lblWHp;
        private NumericUpDown nudWHp;
        private Label lblWArmor;
        private NumericUpDown nudWArmor;
        private Label lblWMana;
        private NumericUpDown nudWMana;
        private Label lblWRegen;
        private NumericUpDown nudWRegen;
        private Label lblWHeal;
        private NumericUpDown nudWHeal;
        private Label lblWEvasion;
        private NumericUpDown nudWEvasion;
        private Label lblWCast;
        private NumericUpDown nudWCast;
        private Label lblWCdr;
        private NumericUpDown nudWCdr;
        private Label lblWCost;
        private NumericUpDown nudWCost;
        private Label lblWSlot;
        private NumericUpDown nudWSlot;
        private Label lblElem;
        private TextBox txtElem;
        private Label lblSecOpciones;
        private CheckBox chkForce;
        private CheckBox chkKeys;
        private Label lblGlifos;
        private CheckBox chkVapor;
        private CheckBox chkAgua;
        private CheckBox chkBrasa;
        private CheckBox chkLuz;
        private Label lblTime;
        private NumericUpDown nudTime;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            menuMain = new MenuStrip(); mnuArchivo = new ToolStripMenuItem(); mnuSalir = new ToolStripMenuItem(); mnuEjemplo = new ToolStripMenuItem();
            mnuAyuda = new ToolStripMenuItem(); mnuComoUsarlo = new ToolStripMenuItem(); mnuAcercaDe = new ToolStripMenuItem();
            menuMain.SuspendLayout();
            pnlLeft = new Panel(); pnlCampos = new Panel(); pnlBotones = new Panel();
            pnlRight = new Panel(); pnlEnlace = new Panel(); pnlArbol = new Panel();
            btnGo = new Button(); btnCancel = new Button(); btnEval = new Button(); btnCopy = new Button(); btnAbrir = new Button();
            lblEnlaceIn = new Label(); lblEnlaceOut = new Label(); lblStatus = new Label();
            txtUrlIn = new TextBox(); txtUrlOut = new TextBox();
            tabMain = new TabControl(); tabArbol = new TabPage(); tabResumen = new TabPage(); tabNodos = new TabPage(); tabFicha = new TabPage();
            wvFicha = new Microsoft.Web.WebView2.WinForms.WebView2();
            pnlFicha = new Panel(); btnFichaAEquipo = new Button(); lblFichaInfo = new Label();
            rtb = new RichTextBox(); lv = new ListView();
            colId = new ColumnHeader(); colTipo = new ColumnHeader(); colNombre = new ColumnHeader(); colEfecto = new ColumnHeader();

            lblSecPersonaje = new Label();
            lblRace = new Label();
            cmbRace = new ComboBox();
            lblClass = new Label();
            cmbClass = new ComboBox();
            lblLevel = new Label();
            nudLevel = new NumericUpDown();
            lblDungeons = new Label();
            nudDungeons = new NumericUpDown();
            lblFue = new Label();
            nudFue = new NumericUpDown();
            lblAgi = new Label();
            nudAgi = new NumericUpDown();
            lblInt = new Label();
            nudInt = new NumericUpDown();
            lblCar = new Label();
            nudCar = new NumericUpDown();
            lblVit = new Label();
            nudVit = new NumericUpDown();
            lblAllocCheck = new Label();
            lblSecEquipo = new Label();
            chkEquipoFicha = new CheckBox();
            lblGFue = new Label();
            nudGFue = new NumericUpDown();
            lblGAgi = new Label();
            nudGAgi = new NumericUpDown();
            lblGInt = new Label();
            nudGInt = new NumericUpDown();
            lblGCar = new Label();
            nudGCar = new NumericUpDown();
            lblGVit = new Label();
            nudGVit = new NumericUpDown();
            lblGPhys = new Label();
            nudGPhys = new NumericUpDown();
            lblGMagic = new Label();
            nudGMagic = new NumericUpDown();
            lblGMana = new Label();
            nudGMana = new NumericUpDown();
            lblGHp = new Label();
            nudGHp = new NumericUpDown();
            lblGArmorPct = new Label();
            nudGArmorPct = new NumericUpDown();
            lblGDefPct = new Label();
            nudGDefPct = new NumericUpDown();
            lblGCritPct = new Label();
            nudGCritPct = new NumericUpDown();
            lblSecPesos = new Label();
            btnPesos = new Button();
            lblWPhys = new Label();
            nudWPhys = new NumericUpDown();
            lblWMagic = new Label();
            nudWMagic = new NumericUpDown();
            lblWHp = new Label();
            nudWHp = new NumericUpDown();
            lblWArmor = new Label();
            nudWArmor = new NumericUpDown();
            lblWMana = new Label();
            nudWMana = new NumericUpDown();
            lblWRegen = new Label();
            nudWRegen = new NumericUpDown();
            lblWHeal = new Label();
            nudWHeal = new NumericUpDown();
            lblWEvasion = new Label();
            nudWEvasion = new NumericUpDown();
            lblWCast = new Label();
            nudWCast = new NumericUpDown();
            lblWCdr = new Label();
            nudWCdr = new NumericUpDown();
            lblWCost = new Label();
            nudWCost = new NumericUpDown();
            lblWSlot = new Label();
            nudWSlot = new NumericUpDown();
            lblElem = new Label();
            txtElem = new TextBox();
            lblSecOpciones = new Label();
            chkForce = new CheckBox();
            chkKeys = new CheckBox();
            lblGlifos = new Label();
            chkVapor = new CheckBox();
            chkAgua = new CheckBox();
            chkBrasa = new CheckBox();
            chkLuz = new CheckBox();
            lblTime = new Label();
            nudTime = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)nudLevel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudDungeons).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAgi).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudInt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudCar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudVit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGFue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGAgi).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGInt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGCar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGVit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGPhys).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGMagic).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGMana).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGHp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGArmorPct).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGDefPct).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudGCritPct).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWPhys).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWMagic).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWHp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWArmor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWMana).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWRegen).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWHeal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWEvasion).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWCast).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWCdr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWCost).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudWSlot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudTime).BeginInit();
            pnlLeft.SuspendLayout(); pnlCampos.SuspendLayout(); pnlBotones.SuspendLayout();
            pnlRight.SuspendLayout(); pnlEnlace.SuspendLayout(); tabMain.SuspendLayout();
            tabArbol.SuspendLayout(); tabResumen.SuspendLayout(); tabNodos.SuspendLayout(); tabFicha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)wvFicha).BeginInit();
            SuspendLayout();

            // 
            // lblSecPersonaje
            // 
            lblSecPersonaje.AutoSize = true;
            lblSecPersonaje.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSecPersonaje.Location = new Point(12, 14);
            lblSecPersonaje.Name = "lblSecPersonaje";
            lblSecPersonaje.Size = new Size(100, 15);
            lblSecPersonaje.TabIndex = 0;
            lblSecPersonaje.Text = "Personaje";
            // 
            // lblRace
            // 
            lblRace.Location = new Point(12, 42);
            lblRace.Name = "lblRace";
            lblRace.Size = new Size(232, 20);
            lblRace.Text = "Raza";
            // 
            // cmbRace
            // 
            cmbRace.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRace.Enabled = true;
            cmbRace.FormattingEnabled = true;
            cmbRace.Items.AddRange(new object[] { "human", "elf", "drow", "orc", "dwarf", "gnome" });
            cmbRace.Location = new Point(252, 38);
            cmbRace.Name = "cmbRace";
            cmbRace.SelectedIndexChanged += cmbRace_SelectedIndexChanged;
            cmbRace.Size = new Size(120, 23);
            cmbRace.TabIndex = 1;
            // 
            // lblClass
            // 
            lblClass.Location = new Point(12, 72);
            lblClass.Name = "lblClass";
            lblClass.Size = new Size(232, 20);
            lblClass.Text = "Clase";
            // 
            // cmbClass
            // 
            cmbClass.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbClass.Enabled = true;
            cmbClass.FormattingEnabled = true;
            cmbClass.Items.AddRange(new object[] { "wizard" });
            cmbClass.Location = new Point(252, 68);
            cmbClass.Name = "cmbClass";
            cmbClass.SelectedIndexChanged += cmbClass_SelectedIndexChanged;
            cmbClass.Size = new Size(120, 23);
            cmbClass.TabIndex = 2;
            // 
            // lblLevel
            // 
            lblLevel.Location = new Point(12, 102);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(232, 20);
            lblLevel.Text = "Nivel";
            // 
            // nudLevel
            // 
            nudLevel.Location = new Point(252, 98);
            nudLevel.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudLevel.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            nudLevel.Name = "nudLevel";
            nudLevel.Size = new Size(120, 23);
            nudLevel.TabIndex = 3;
            nudLevel.Value = new decimal(new int[] { 36, 0, 0, 0 });
            // 
            // lblDungeons
            // 
            lblDungeons.Location = new Point(12, 132);
            lblDungeons.Name = "lblDungeons";
            lblDungeons.Size = new Size(232, 20);
            lblDungeons.Text = "Mazmorras completadas";
            // 
            // nudDungeons
            // 
            nudDungeons.Location = new Point(252, 128);
            nudDungeons.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            nudDungeons.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudDungeons.Name = "nudDungeons";
            nudDungeons.Size = new Size(120, 23);
            nudDungeons.TabIndex = 4;
            nudDungeons.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblFue
            // 
            lblFue.Location = new Point(12, 162);
            lblFue.Name = "lblFue";
            lblFue.Size = new Size(232, 20);
            lblFue.Text = "Puntos repartidos FUE";
            // 
            // nudFue
            // 
            nudFue.Location = new Point(252, 158);
            nudFue.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudFue.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudFue.Name = "nudFue";
            nudFue.Size = new Size(120, 23);
            nudFue.TabIndex = 5;
            nudFue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblAgi
            // 
            lblAgi.Location = new Point(12, 192);
            lblAgi.Name = "lblAgi";
            lblAgi.Size = new Size(232, 20);
            lblAgi.Text = "Puntos repartidos AGI";
            // 
            // nudAgi
            // 
            nudAgi.Location = new Point(252, 188);
            nudAgi.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudAgi.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudAgi.Name = "nudAgi";
            nudAgi.Size = new Size(120, 23);
            nudAgi.TabIndex = 6;
            nudAgi.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblInt
            // 
            lblInt.Location = new Point(12, 222);
            lblInt.Name = "lblInt";
            lblInt.Size = new Size(232, 20);
            lblInt.Text = "Puntos repartidos INT";
            // 
            // nudInt
            // 
            nudInt.Location = new Point(252, 218);
            nudInt.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudInt.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudInt.Name = "nudInt";
            nudInt.Size = new Size(120, 23);
            nudInt.TabIndex = 7;
            nudInt.Value = new decimal(new int[] { 44, 0, 0, 0 });
            // 
            // lblCar
            // 
            lblCar.Location = new Point(12, 252);
            lblCar.Name = "lblCar";
            lblCar.Size = new Size(232, 20);
            lblCar.Text = "Puntos repartidos CAR";
            // 
            // nudCar
            // 
            nudCar.Location = new Point(252, 248);
            nudCar.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudCar.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudCar.Name = "nudCar";
            nudCar.Size = new Size(120, 23);
            nudCar.TabIndex = 8;
            nudCar.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblVit
            // 
            lblVit.Location = new Point(12, 282);
            lblVit.Name = "lblVit";
            lblVit.Size = new Size(232, 20);
            lblVit.Text = "Puntos repartidos VIT";
            // 
            // nudVit
            // 
            nudVit.Location = new Point(252, 278);
            nudVit.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudVit.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudVit.Name = "nudVit";
            nudVit.Size = new Size(120, 23);
            nudVit.TabIndex = 9;
            nudVit.Value = new decimal(new int[] { 26, 0, 0, 0 });
            // 
            // lblAllocCheck
            // 
            lblAllocCheck.AutoSize = true;
            lblAllocCheck.Location = new Point(12, 308);
            lblAllocCheck.Name = "lblAllocCheck";
            lblAllocCheck.Size = new Size(100, 15);
            lblAllocCheck.Text = "Reparto";
            // 
            // lblSecEquipo
            // 
            lblSecEquipo.AutoSize = true;
            lblSecEquipo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSecEquipo.Location = new Point(12, 338);
            lblSecEquipo.Name = "lblSecEquipo";
            lblSecEquipo.Size = new Size(100, 15);
            lblSecEquipo.TabIndex = 10;
            lblSecEquipo.Text = "Equipo (suma de todas tus piezas)";
            // 
            // chkEquipoFicha  (rellena el equipo solo desde la ficha del personaje)
            // 
            chkEquipoFicha.AutoSize = true;
            chkEquipoFicha.Location = new Point(262, 336);
            chkEquipoFicha.Name = "chkEquipoFicha";
            chkEquipoFicha.Size = new Size(110, 19);
            chkEquipoFicha.TabIndex = 9;
            chkEquipoFicha.Text = "Auto (ficha)";
            chkEquipoFicha.UseVisualStyleBackColor = true;
            chkEquipoFicha.CheckedChanged += chkEquipoFicha_CheckedChanged;
            // 
            // lblGFue
            // 
            lblGFue.Location = new Point(12, 366);
            lblGFue.Name = "lblGFue";
            lblGFue.Size = new Size(232, 20);
            lblGFue.Text = "FUE de equipo";
            // 
            // nudGFue
            // 
            nudGFue.Location = new Point(252, 362);
            nudGFue.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudGFue.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGFue.Name = "nudGFue";
            nudGFue.Size = new Size(120, 23);
            nudGFue.TabIndex = 11;
            nudGFue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGAgi
            // 
            lblGAgi.Location = new Point(12, 396);
            lblGAgi.Name = "lblGAgi";
            lblGAgi.Size = new Size(232, 20);
            lblGAgi.Text = "AGI de equipo";
            // 
            // nudGAgi
            // 
            nudGAgi.Location = new Point(252, 392);
            nudGAgi.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudGAgi.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGAgi.Name = "nudGAgi";
            nudGAgi.Size = new Size(120, 23);
            nudGAgi.TabIndex = 12;
            nudGAgi.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGInt
            // 
            lblGInt.Location = new Point(12, 426);
            lblGInt.Name = "lblGInt";
            lblGInt.Size = new Size(232, 20);
            lblGInt.Text = "INT de equipo";
            // 
            // nudGInt
            // 
            nudGInt.Location = new Point(252, 422);
            nudGInt.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudGInt.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGInt.Name = "nudGInt";
            nudGInt.Size = new Size(120, 23);
            nudGInt.TabIndex = 13;
            nudGInt.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGCar
            // 
            lblGCar.Location = new Point(12, 456);
            lblGCar.Name = "lblGCar";
            lblGCar.Size = new Size(232, 20);
            lblGCar.Text = "CAR de equipo";
            // 
            // nudGCar
            // 
            nudGCar.Location = new Point(252, 452);
            nudGCar.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudGCar.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGCar.Name = "nudGCar";
            nudGCar.Size = new Size(120, 23);
            nudGCar.TabIndex = 14;
            nudGCar.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGVit
            // 
            lblGVit.Location = new Point(12, 486);
            lblGVit.Name = "lblGVit";
            lblGVit.Size = new Size(232, 20);
            lblGVit.Text = "VIT de equipo";
            // 
            // nudGVit
            // 
            nudGVit.Location = new Point(252, 482);
            nudGVit.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            nudGVit.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGVit.Name = "nudGVit";
            nudGVit.Size = new Size(120, 23);
            nudGVit.TabIndex = 15;
            nudGVit.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGPhys
            // 
            lblGPhys.Location = new Point(12, 516);
            lblGPhys.Name = "lblGPhys";
            lblGPhys.Size = new Size(232, 20);
            lblGPhys.Text = "Daño físico (arma, piezas)";
            // 
            // nudGPhys
            // 
            nudGPhys.Location = new Point(252, 512);
            nudGPhys.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            nudGPhys.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGPhys.Name = "nudGPhys";
            nudGPhys.Size = new Size(120, 23);
            nudGPhys.TabIndex = 16;
            nudGPhys.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGMagic
            // 
            lblGMagic.Location = new Point(12, 546);
            lblGMagic.Name = "lblGMagic";
            lblGMagic.Size = new Size(232, 20);
            lblGMagic.Text = "Daño mágico (arma, piezas)";
            // 
            // nudGMagic
            // 
            nudGMagic.Location = new Point(252, 542);
            nudGMagic.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            nudGMagic.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGMagic.Name = "nudGMagic";
            nudGMagic.Size = new Size(120, 23);
            nudGMagic.TabIndex = 17;
            nudGMagic.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGMana
            // 
            lblGMana.Location = new Point(12, 576);
            lblGMana.Name = "lblGMana";
            lblGMana.Size = new Size(232, 20);
            lblGMana.Text = "Maná";
            // 
            // nudGMana
            // 
            nudGMana.Location = new Point(252, 572);
            nudGMana.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            nudGMana.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGMana.Name = "nudGMana";
            nudGMana.Size = new Size(120, 23);
            nudGMana.TabIndex = 18;
            nudGMana.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGHp
            // 
            lblGHp.Location = new Point(12, 606);
            lblGHp.Name = "lblGHp";
            lblGHp.Size = new Size(232, 20);
            lblGHp.Text = "Vida";
            // 
            // nudGHp
            // 
            nudGHp.Location = new Point(252, 602);
            nudGHp.Maximum = new decimal(new int[] { 8000, 0, 0, 0 });
            nudGHp.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGHp.Name = "nudGHp";
            nudGHp.Size = new Size(120, 23);
            nudGHp.TabIndex = 19;
            nudGHp.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGArmorPct
            // 
            lblGArmorPct.Location = new Point(12, 636);
            lblGArmorPct.Name = "lblGArmorPct";
            lblGArmorPct.Size = new Size(232, 20);
            lblGArmorPct.Text = "% Armadura";
            // 
            // nudGArmorPct
            // 
            nudGArmorPct.Location = new Point(252, 632);
            nudGArmorPct.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudGArmorPct.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGArmorPct.Name = "nudGArmorPct";
            nudGArmorPct.Size = new Size(120, 23);
            nudGArmorPct.TabIndex = 20;
            nudGArmorPct.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGDefPct
            // 
            lblGDefPct.Location = new Point(12, 666);
            lblGDefPct.Name = "lblGDefPct";
            lblGDefPct.Size = new Size(232, 20);
            lblGDefPct.Text = "% Defensa";
            // 
            // nudGDefPct
            // 
            nudGDefPct.Location = new Point(252, 662);
            nudGDefPct.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudGDefPct.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGDefPct.Name = "nudGDefPct";
            nudGDefPct.Size = new Size(120, 23);
            nudGDefPct.TabIndex = 21;
            nudGDefPct.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblGCritPct
            // 
            lblGCritPct.Location = new Point(12, 696);
            lblGCritPct.Name = "lblGCritPct";
            lblGCritPct.Size = new Size(232, 20);
            lblGCritPct.Text = "% Chance de crítico";
            // 
            // nudGCritPct
            // 
            nudGCritPct.Location = new Point(252, 692);
            nudGCritPct.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            nudGCritPct.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            nudGCritPct.Name = "nudGCritPct";
            nudGCritPct.Size = new Size(120, 23);
            nudGCritPct.TabIndex = 22;
            nudGCritPct.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblSecPesos
            // 
            lblSecPesos.AutoSize = true;
            lblSecPesos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSecPesos.Location = new Point(12, 728);
            lblSecPesos.Name = "lblSecPesos";
            lblSecPesos.Size = new Size(100, 15);
            lblSecPesos.TabIndex = 23;
            lblSecPesos.Text = "Pesos del objetivo (suposiciones tuyas)";
            // 
            // btnPesos
            // 
            btnPesos.Location = new Point(12, 752);
            btnPesos.Name = "btnPesos";
            btnPesos.Size = new Size(360, 26);
            btnPesos.TabIndex = 24;
            btnPesos.Text = "Pesos por defecto de la clase";
            btnPesos.UseVisualStyleBackColor = true;
            btnPesos.Click += btnPesos_Click;
            // 
            // lblWPhys
            // 
            lblWPhys.Location = new Point(12, 788);
            lblWPhys.Name = "lblWPhys";
            lblWPhys.Size = new Size(232, 20);
            lblWPhys.Text = "Daño físico (principal)";
            // 
            // nudWPhys
            // 
            nudWPhys.DecimalPlaces = 2;
            nudWPhys.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWPhys.Location = new Point(252, 784);
            nudWPhys.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWPhys.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWPhys.Name = "nudWPhys";
            nudWPhys.Size = new Size(120, 23);
            nudWPhys.TabIndex = 25;
            nudWPhys.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWMagic
            // 
            lblWMagic.Location = new Point(12, 818);
            lblWMagic.Name = "lblWMagic";
            lblWMagic.Size = new Size(232, 20);
            lblWMagic.Text = "Daño mágico (principal)";
            // 
            // nudWMagic
            // 
            nudWMagic.DecimalPlaces = 2;
            nudWMagic.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWMagic.Location = new Point(252, 814);
            nudWMagic.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWMagic.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWMagic.Name = "nudWMagic";
            nudWMagic.Size = new Size(120, 23);
            nudWMagic.TabIndex = 26;
            nudWMagic.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWHp
            // 
            lblWHp.Location = new Point(12, 848);
            lblWHp.Name = "lblWHp";
            lblWHp.Size = new Size(232, 20);
            lblWHp.Text = "Vida";
            // 
            // nudWHp
            // 
            nudWHp.DecimalPlaces = 2;
            nudWHp.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWHp.Location = new Point(252, 844);
            nudWHp.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWHp.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWHp.Name = "nudWHp";
            nudWHp.Size = new Size(120, 23);
            nudWHp.TabIndex = 27;
            nudWHp.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWArmor
            // 
            lblWArmor.Location = new Point(12, 878);
            lblWArmor.Name = "lblWArmor";
            lblWArmor.Size = new Size(232, 20);
            lblWArmor.Text = "Armadura";
            // 
            // nudWArmor
            // 
            nudWArmor.DecimalPlaces = 2;
            nudWArmor.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWArmor.Location = new Point(252, 874);
            nudWArmor.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWArmor.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWArmor.Name = "nudWArmor";
            nudWArmor.Size = new Size(120, 23);
            nudWArmor.TabIndex = 28;
            nudWArmor.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWMana
            // 
            lblWMana.Location = new Point(12, 908);
            lblWMana.Name = "lblWMana";
            lblWMana.Size = new Size(232, 20);
            lblWMana.Text = "Maná máximo";
            // 
            // nudWMana
            // 
            nudWMana.DecimalPlaces = 2;
            nudWMana.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWMana.Location = new Point(252, 904);
            nudWMana.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWMana.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWMana.Name = "nudWMana";
            nudWMana.Size = new Size(120, 23);
            nudWMana.TabIndex = 29;
            nudWMana.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWRegen
            // 
            lblWRegen.Location = new Point(12, 938);
            lblWRegen.Name = "lblWRegen";
            lblWRegen.Size = new Size(232, 20);
            lblWRegen.Text = "Regeneración de maná";
            // 
            // nudWRegen
            // 
            nudWRegen.DecimalPlaces = 2;
            nudWRegen.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWRegen.Location = new Point(252, 934);
            nudWRegen.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWRegen.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWRegen.Name = "nudWRegen";
            nudWRegen.Size = new Size(120, 23);
            nudWRegen.TabIndex = 30;
            nudWRegen.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWHeal
            // 
            lblWHeal.Location = new Point(12, 968);
            lblWHeal.Name = "lblWHeal";
            lblWHeal.Size = new Size(232, 20);
            lblWHeal.Text = "Curación hecha";
            // 
            // nudWHeal
            // 
            nudWHeal.DecimalPlaces = 2;
            nudWHeal.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWHeal.Location = new Point(252, 964);
            nudWHeal.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWHeal.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWHeal.Name = "nudWHeal";
            nudWHeal.Size = new Size(120, 23);
            nudWHeal.TabIndex = 31;
            nudWHeal.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWEvasion
            // 
            lblWEvasion.Location = new Point(12, 998);
            lblWEvasion.Name = "lblWEvasion";
            lblWEvasion.Size = new Size(232, 20);
            lblWEvasion.Text = "Evasión";
            // 
            // nudWEvasion
            // 
            nudWEvasion.DecimalPlaces = 2;
            nudWEvasion.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWEvasion.Location = new Point(252, 994);
            nudWEvasion.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWEvasion.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWEvasion.Name = "nudWEvasion";
            nudWEvasion.Size = new Size(120, 23);
            nudWEvasion.TabIndex = 32;
            nudWEvasion.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWCast
            // 
            lblWCast.Location = new Point(12, 1028);
            lblWCast.Name = "lblWCast";
            lblWCast.Size = new Size(232, 20);
            lblWCast.Text = "Velocidad de lanzamiento";
            // 
            // nudWCast
            // 
            nudWCast.DecimalPlaces = 2;
            nudWCast.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWCast.Location = new Point(252, 1024);
            nudWCast.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWCast.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWCast.Name = "nudWCast";
            nudWCast.Size = new Size(120, 23);
            nudWCast.TabIndex = 33;
            nudWCast.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWCdr
            // 
            lblWCdr.Location = new Point(12, 1058);
            lblWCdr.Name = "lblWCdr";
            lblWCdr.Size = new Size(232, 20);
            lblWCdr.Text = "Enfriamiento";
            // 
            // nudWCdr
            // 
            nudWCdr.DecimalPlaces = 2;
            nudWCdr.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWCdr.Location = new Point(252, 1054);
            nudWCdr.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWCdr.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWCdr.Name = "nudWCdr";
            nudWCdr.Size = new Size(120, 23);
            nudWCdr.TabIndex = 34;
            nudWCdr.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWCost
            // 
            lblWCost.Location = new Point(12, 1088);
            lblWCost.Name = "lblWCost";
            lblWCost.Size = new Size(232, 20);
            lblWCost.Text = "Ahorro de coste";
            // 
            // nudWCost
            // 
            nudWCost.DecimalPlaces = 2;
            nudWCost.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWCost.Location = new Point(252, 1084);
            nudWCost.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWCost.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWCost.Name = "nudWCost";
            nudWCost.Size = new Size(120, 23);
            nudWCost.TabIndex = 35;
            nudWCost.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblWSlot
            // 
            lblWSlot.Location = new Point(12, 1118);
            lblWSlot.Name = "lblWSlot";
            lblWSlot.Size = new Size(232, 20);
            lblWSlot.Text = "Valor de +1 ranura";
            // 
            // nudWSlot
            // 
            nudWSlot.DecimalPlaces = 2;
            nudWSlot.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            nudWSlot.Location = new Point(252, 1114);
            nudWSlot.Maximum = new decimal(new int[] { 300, 0, 0, 131072 });
            nudWSlot.Minimum = new decimal(new int[] { 0, 0, 0, 131072 });
            nudWSlot.Name = "nudWSlot";
            nudWSlot.Size = new Size(120, 23);
            nudWSlot.TabIndex = 36;
            nudWSlot.Value = new decimal(new int[] { 0, 0, 0, 131072 });
            // 
            // lblElem
            // 
            lblElem.Location = new Point(12, 1148);
            lblElem.Name = "lblElem";
            lblElem.Size = new Size(232, 20);
            lblElem.Text = "Elementos (fire=1;ice=0.5)";
            // 
            // txtElem
            // 
            txtElem.Location = new Point(252, 1144);
            txtElem.Name = "txtElem";
            txtElem.Size = new Size(120, 23);
            txtElem.TabIndex = 37;
            // 
            // lblSecOpciones
            // 
            lblSecOpciones.AutoSize = true;
            lblSecOpciones.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSecOpciones.Location = new Point(12, 1180);
            lblSecOpciones.Name = "lblSecOpciones";
            lblSecOpciones.Size = new Size(100, 15);
            lblSecOpciones.TabIndex = 38;
            lblSecOpciones.Text = "Opciones";
            // 
            // chkForce
            // 
            chkForce.AutoSize = true;
            chkForce.Checked = false;
            chkForce.CheckState = CheckState.Unchecked;
            chkForce.Location = new Point(12, 1204);
            chkForce.Name = "chkForce";
            chkForce.Size = new Size(100, 19);
            chkForce.TabIndex = 39;
            chkForce.Text = "Forzar Mente de Engranajes (gnomo)";
            chkForce.UseVisualStyleBackColor = true;
            // 
            // chkKeys
            // 
            chkKeys.AutoSize = true;
            chkKeys.Checked = true;
            chkKeys.CheckState = CheckState.Checked;
            chkKeys.Location = new Point(12, 1230);
            chkKeys.Name = "chkKeys";
            chkKeys.Size = new Size(100, 19);
            chkKeys.TabIndex = 40;
            chkKeys.Text = "Permitir piedras angulares";
            chkKeys.UseVisualStyleBackColor = true;
            // 
            // lblGlifos
            // 
            lblGlifos.AutoSize = true;
            lblGlifos.Location = new Point(12, 1259);
            lblGlifos.Name = "lblGlifos";
            lblGlifos.Size = new Size(100, 15);
            lblGlifos.Text = "Glifos que puedes fabricar:";
            // 
            // chkVapor
            // 
            chkVapor.AutoSize = true;
            chkVapor.Checked = false;
            chkVapor.CheckState = CheckState.Unchecked;
            chkVapor.Location = new Point(22, 1282);
            chkVapor.Name = "chkVapor";
            chkVapor.Size = new Size(100, 19);
            chkVapor.TabIndex = 41;
            chkVapor.Text = "vapor";
            chkVapor.UseVisualStyleBackColor = true;
            // 
            // chkAgua
            // 
            chkAgua.AutoSize = true;
            chkAgua.Checked = false;
            chkAgua.CheckState = CheckState.Unchecked;
            chkAgua.Location = new Point(102, 1282);
            chkAgua.Name = "chkAgua";
            chkAgua.Size = new Size(100, 19);
            chkAgua.TabIndex = 42;
            chkAgua.Text = "agua";
            chkAgua.UseVisualStyleBackColor = true;
            // 
            // chkBrasa
            // 
            chkBrasa.AutoSize = true;
            chkBrasa.Checked = false;
            chkBrasa.CheckState = CheckState.Unchecked;
            chkBrasa.Location = new Point(182, 1282);
            chkBrasa.Name = "chkBrasa";
            chkBrasa.Size = new Size(100, 19);
            chkBrasa.TabIndex = 43;
            chkBrasa.Text = "brasa";
            chkBrasa.UseVisualStyleBackColor = true;
            // 
            // chkLuz
            // 
            chkLuz.AutoSize = true;
            chkLuz.Checked = false;
            chkLuz.CheckState = CheckState.Unchecked;
            chkLuz.Location = new Point(262, 1282);
            chkLuz.Name = "chkLuz";
            chkLuz.Size = new Size(100, 19);
            chkLuz.TabIndex = 44;
            chkLuz.Text = "luz";
            chkLuz.UseVisualStyleBackColor = true;
            // 
            // lblTime
            // 
            lblTime.Location = new Point(12, 1316);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(232, 20);
            lblTime.Text = "Tiempo máximo (s)";
            // 
            // nudTime
            // 
            nudTime.Location = new Point(252, 1312);
            nudTime.Maximum = new decimal(new int[] { 600, 0, 0, 0 });
            nudTime.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            nudTime.Name = "nudTime";
            nudTime.Size = new Size(120, 23);
            nudTime.TabIndex = 45;
            nudTime.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // pnlCampos  (campos del personaje; con desplazamiento si no caben)
            // 
            pnlCampos.AutoScroll = true;
            pnlCampos.AutoScrollMinSize = new Size(0, 1352);
            pnlCampos.Dock = DockStyle.Fill;
            pnlCampos.Name = "pnlCampos";
            pnlCampos.Controls.Add(lblSecPersonaje);
            pnlCampos.Controls.Add(lblRace);
            pnlCampos.Controls.Add(cmbRace);
            pnlCampos.Controls.Add(lblClass);
            pnlCampos.Controls.Add(cmbClass);
            pnlCampos.Controls.Add(lblLevel);
            pnlCampos.Controls.Add(nudLevel);
            pnlCampos.Controls.Add(lblDungeons);
            pnlCampos.Controls.Add(nudDungeons);
            pnlCampos.Controls.Add(lblFue);
            pnlCampos.Controls.Add(nudFue);
            pnlCampos.Controls.Add(lblAgi);
            pnlCampos.Controls.Add(nudAgi);
            pnlCampos.Controls.Add(lblInt);
            pnlCampos.Controls.Add(nudInt);
            pnlCampos.Controls.Add(lblCar);
            pnlCampos.Controls.Add(nudCar);
            pnlCampos.Controls.Add(lblVit);
            pnlCampos.Controls.Add(nudVit);
            pnlCampos.Controls.Add(lblAllocCheck);
            pnlCampos.Controls.Add(lblSecEquipo);
            pnlCampos.Controls.Add(lblGFue);
            pnlCampos.Controls.Add(nudGFue);
            pnlCampos.Controls.Add(chkEquipoFicha);
            pnlCampos.Controls.Add(lblGAgi);
            pnlCampos.Controls.Add(nudGAgi);
            pnlCampos.Controls.Add(lblGInt);
            pnlCampos.Controls.Add(nudGInt);
            pnlCampos.Controls.Add(lblGCar);
            pnlCampos.Controls.Add(nudGCar);
            pnlCampos.Controls.Add(lblGVit);
            pnlCampos.Controls.Add(nudGVit);
            pnlCampos.Controls.Add(lblGPhys);
            pnlCampos.Controls.Add(nudGPhys);
            pnlCampos.Controls.Add(lblGMagic);
            pnlCampos.Controls.Add(nudGMagic);
            pnlCampos.Controls.Add(lblGMana);
            pnlCampos.Controls.Add(nudGMana);
            pnlCampos.Controls.Add(lblGHp);
            pnlCampos.Controls.Add(nudGHp);
            pnlCampos.Controls.Add(lblGArmorPct);
            pnlCampos.Controls.Add(nudGArmorPct);
            pnlCampos.Controls.Add(lblGDefPct);
            pnlCampos.Controls.Add(nudGDefPct);
            pnlCampos.Controls.Add(lblGCritPct);
            pnlCampos.Controls.Add(nudGCritPct);
            pnlCampos.Controls.Add(lblSecPesos);
            pnlCampos.Controls.Add(btnPesos);
            pnlCampos.Controls.Add(lblWPhys);
            pnlCampos.Controls.Add(nudWPhys);
            pnlCampos.Controls.Add(lblWMagic);
            pnlCampos.Controls.Add(nudWMagic);
            pnlCampos.Controls.Add(lblWHp);
            pnlCampos.Controls.Add(nudWHp);
            pnlCampos.Controls.Add(lblWArmor);
            pnlCampos.Controls.Add(nudWArmor);
            pnlCampos.Controls.Add(lblWMana);
            pnlCampos.Controls.Add(nudWMana);
            pnlCampos.Controls.Add(lblWRegen);
            pnlCampos.Controls.Add(nudWRegen);
            pnlCampos.Controls.Add(lblWHeal);
            pnlCampos.Controls.Add(nudWHeal);
            pnlCampos.Controls.Add(lblWEvasion);
            pnlCampos.Controls.Add(nudWEvasion);
            pnlCampos.Controls.Add(lblWCast);
            pnlCampos.Controls.Add(nudWCast);
            pnlCampos.Controls.Add(lblWCdr);
            pnlCampos.Controls.Add(nudWCdr);
            pnlCampos.Controls.Add(lblWCost);
            pnlCampos.Controls.Add(nudWCost);
            pnlCampos.Controls.Add(lblWSlot);
            pnlCampos.Controls.Add(nudWSlot);
            pnlCampos.Controls.Add(lblElem);
            pnlCampos.Controls.Add(txtElem);
            pnlCampos.Controls.Add(lblSecOpciones);
            pnlCampos.Controls.Add(chkForce);
            pnlCampos.Controls.Add(chkKeys);
            pnlCampos.Controls.Add(lblGlifos);
            pnlCampos.Controls.Add(chkVapor);
            pnlCampos.Controls.Add(chkAgua);
            pnlCampos.Controls.Add(chkBrasa);
            pnlCampos.Controls.Add(chkLuz);
            pnlCampos.Controls.Add(lblTime);
            pnlCampos.Controls.Add(nudTime);
            // 
            // btnGo
            // 
            btnGo.Location = new Point(12, 8);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(130, 32);
            btnGo.TabIndex = 100;
            btnGo.Text = "Generar árbol";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += btnGo_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(150, 8);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 32);
            btnCancel.TabIndex = 101;
            btnCancel.Text = "Cancelar";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // pnlBotones
            // 
            pnlBotones.Controls.Add(btnGo);
            pnlBotones.Controls.Add(btnCancel);
            pnlBotones.Dock = DockStyle.Bottom;
            pnlBotones.Name = "pnlBotones";
            pnlBotones.Size = new Size(400, 48);
            // 
            // pnlLeft
            // 
            pnlLeft.Controls.Add(pnlCampos);
            pnlLeft.Controls.Add(pnlBotones);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(400, 700);
            // 
            // lblEnlaceIn
            // 
            lblEnlaceIn.AutoSize = true;
            lblEnlaceIn.Location = new Point(8, 6);
            lblEnlaceIn.Name = "lblEnlaceIn";
            lblEnlaceIn.Text = "Pega aquí un enlace del planificador para evaluarlo:";
            // 
            // txtUrlIn
            // 
            txtUrlIn.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrlIn.Location = new Point(8, 26);
            txtUrlIn.Name = "txtUrlIn";
            txtUrlIn.Size = new Size(800, 23);
            // 
            // btnEval
            // 
            btnEval.Location = new Point(8, 54);
            btnEval.Name = "btnEval";
            btnEval.Size = new Size(110, 30);
            btnEval.Text = "Evaluar enlace";
            btnEval.UseVisualStyleBackColor = true;
            btnEval.Click += btnEval_Click;
            // 
            // btnCopy
            // 
            btnCopy.Location = new Point(188, 142);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(110, 30);
            btnCopy.Text = "Copiar enlace";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // btnAbrir
            // 
            btnAbrir.Location = new Point(8, 142);
            btnAbrir.Name = "btnAbrir";
            btnAbrir.Size = new Size(170, 30);
            btnAbrir.Text = "Abrir en el navegador";
            btnAbrir.UseVisualStyleBackColor = true;
            btnAbrir.Click += btnAbrir_Click;
            // 
            // lblEnlaceOut
            // 
            lblEnlaceOut.AutoSize = true;
            lblEnlaceOut.Location = new Point(8, 92);
            lblEnlaceOut.Name = "lblEnlaceOut";
            lblEnlaceOut.Text = "Enlace generado:";
            // 
            // txtUrlOut
            // 
            txtUrlOut.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUrlOut.Location = new Point(8, 112);
            txtUrlOut.Name = "txtUrlOut";
            txtUrlOut.ReadOnly = true;
            txtUrlOut.Size = new Size(800, 23);
            // 
            // pnlEnlace
            // 
            pnlEnlace.Controls.Add(lblEnlaceIn);
            pnlEnlace.Controls.Add(txtUrlIn);
            pnlEnlace.Controls.Add(btnEval);
            pnlEnlace.Controls.Add(btnCopy);
            pnlEnlace.Controls.Add(btnAbrir);
            pnlEnlace.Controls.Add(lblEnlaceOut);
            pnlEnlace.Controls.Add(txtUrlOut);
            pnlEnlace.Dock = DockStyle.Top;
            pnlEnlace.Name = "pnlEnlace";
            pnlEnlace.Size = new Size(840, 182);
            // 
            // pnlArbol  (aquí se inserta el dibujo del árbol, TreeCanvas, al arrancar)
            // 
            pnlArbol.BackColor = Color.FromArgb(10, 11, 16);
            pnlArbol.Dock = DockStyle.Fill;
            pnlArbol.Name = "pnlArbol";
            // 
            // tabArbol
            // 
            tabArbol.Controls.Add(pnlArbol);
            tabArbol.Name = "tabArbol";
            tabArbol.Padding = new Padding(0);
            tabArbol.Text = "Árbol";
            // 
            // rtb
            // 
            rtb.Dock = DockStyle.Fill;
            rtb.Font = new Font("Consolas", 9.5F);
            rtb.Name = "rtb";
            rtb.ReadOnly = true;
            // 
            // tabResumen
            // 
            tabResumen.Controls.Add(rtb);
            tabResumen.Name = "tabResumen";
            tabResumen.Text = "Resumen";
            // 
            // lv
            // 
            lv.Columns.AddRange(new ColumnHeader[] { colId, colTipo, colNombre, colEfecto });
            lv.Dock = DockStyle.Fill;
            lv.FullRowSelect = true;
            lv.Name = "lv";
            lv.UseCompatibleStateImageBehavior = false;
            lv.View = View.Details;
            colId.Text = "Id"; colId.Width = 50;
            colTipo.Text = "Tipo"; colTipo.Width = 70;
            colNombre.Text = "Nombre"; colNombre.Width = 170;
            colEfecto.Text = "Efecto"; colEfecto.Width = 520;
            // 
            // tabNodos
            // 
            tabNodos.Controls.Add(lv);
            tabNodos.Name = "tabNodos";
            tabNodos.Text = "Nodos elegidos";
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabArbol);
            tabMain.Controls.Add(tabResumen);
            tabMain.Controls.Add(tabNodos);
            tabMain.Controls.Add(tabFicha);
            // 
            // wvFicha  (la ficha del juego se muestra con el HTML/CSS original: ficha.html)
            // 
            wvFicha.AllowExternalDrop = false;
            wvFicha.BackColor = Color.FromArgb(20, 18, 15);
            wvFicha.CreationProperties = null;
            wvFicha.DefaultBackgroundColor = Color.FromArgb(20, 18, 15);
            wvFicha.Dock = DockStyle.Fill;
            wvFicha.Name = "wvFicha";
            wvFicha.ZoomFactor = 1D;
            wvFicha.SizeChanged += wvFicha_SizeChanged;
            // 
            // pnlFicha  (barra superior de la solapa: botón «Pasar a Equipo»)
            // 
            pnlFicha.Controls.Add(lblFichaInfo);
            pnlFicha.Controls.Add(btnFichaAEquipo);
            pnlFicha.Dock = DockStyle.Top;
            pnlFicha.Height = 40;
            pnlFicha.Name = "pnlFicha";
            btnFichaAEquipo.Dock = DockStyle.Left;
            btnFichaAEquipo.Name = "btnFichaAEquipo";
            btnFichaAEquipo.Text = "Pasar a Equipo (generar con este personaje)";
            btnFichaAEquipo.Width = 300;
            btnFichaAEquipo.UseVisualStyleBackColor = true;
            btnFichaAEquipo.Click += btnFichaAEquipo_Click;
            lblFichaInfo.Dock = DockStyle.Fill;
            lblFichaInfo.Name = "lblFichaInfo";
            lblFichaInfo.Padding = new Padding(8, 0, 0, 0);
            lblFichaInfo.TextAlign = ContentAlignment.MiddleLeft;
            lblFichaInfo.Text = "Suma los objetos equipados (tooltips) y los atributos de la hoja, y los copia a la pestaña Equipo.";
            // 
            // tabFicha
            // 
            tabFicha.Controls.Add(wvFicha);
            tabFicha.Controls.Add(pnlFicha);
            tabFicha.Name = "tabFicha";
            tabFicha.Text = "Ejemplo del personaje";
            tabMain.Dock = DockStyle.Fill;
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(tabMain);
            pnlRight.Controls.Add(pnlEnlace);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Name = "pnlRight";
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(1280, 24);
            lblStatus.Text = "Listo.";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // menuMain
            // 
            menuMain.Items.AddRange(new ToolStripItem[] { mnuArchivo, mnuAyuda });
            menuMain.Location = new Point(0, 0);
            menuMain.Name = "menuMain";
            menuMain.Size = new Size(1280, 24);
            // 
            // mnuArchivo
            // 
            mnuArchivo.DropDownItems.AddRange(new ToolStripItem[] { mnuEjemplo, mnuSalir });
            mnuArchivo.Name = "mnuArchivo";
            mnuArchivo.Text = "&Archivo";
            // 
            // mnuEjemplo
            // 
            mnuEjemplo.Name = "mnuEjemplo";
            mnuEjemplo.Text = "Cargar personaje de ejemplo (Scorpio)";
            mnuEjemplo.Click += mnuEjemplo_Click;
            // 
            // mnuSalir
            // 
            mnuSalir.Name = "mnuSalir";
            mnuSalir.ShortcutKeys = Keys.Alt | Keys.F4;
            mnuSalir.Text = "&Salir";
            mnuSalir.Click += mnuSalir_Click;
            // 
            // mnuAyuda  (alineado a la derecha)
            // 
            mnuAyuda.Alignment = ToolStripItemAlignment.Right;
            mnuAyuda.DropDownItems.AddRange(new ToolStripItem[] { mnuComoUsarlo, mnuAcercaDe });
            mnuAyuda.Name = "mnuAyuda";
            mnuAyuda.Text = "A&yuda";
            // 
            // mnuComoUsarlo
            // 
            mnuComoUsarlo.Name = "mnuComoUsarlo";
            mnuComoUsarlo.ShortcutKeys = Keys.F1;
            mnuComoUsarlo.Text = "Manual";
            mnuComoUsarlo.Click += mnuComoUsarlo_Click;
            // 
            // mnuAcercaDe
            // 
            mnuAcercaDe.Name = "mnuAcercaDe";
            mnuAcercaDe.Text = "Acerca de...";
            mnuAcercaDe.Click += mnuAcercaDe_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 820);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            Controls.Add(lblStatus);
            Controls.Add(menuMain);
            MainMenuStrip = menuMain;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Guildlands · Optimizador del árbol de pasivas";
            Load += MainForm_Load;
            tabMain.SelectedIndexChanged += tabMain_SelectedIndexChanged;
            nudLevel.ValueChanged += nudAlloc_ValueChanged;
            ((System.ComponentModel.ISupportInitialize)nudLevel).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudDungeons).EndInit();
            nudFue.ValueChanged += nudAlloc_ValueChanged;
            ((System.ComponentModel.ISupportInitialize)nudFue).EndInit();
            nudAgi.ValueChanged += nudAlloc_ValueChanged;
            ((System.ComponentModel.ISupportInitialize)nudAgi).EndInit();
            nudInt.ValueChanged += nudAlloc_ValueChanged;
            ((System.ComponentModel.ISupportInitialize)nudInt).EndInit();
            nudCar.ValueChanged += nudAlloc_ValueChanged;
            ((System.ComponentModel.ISupportInitialize)nudCar).EndInit();
            nudVit.ValueChanged += nudAlloc_ValueChanged;
            ((System.ComponentModel.ISupportInitialize)nudVit).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGFue).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGAgi).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGInt).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGCar).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGVit).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGPhys).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGMagic).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGMana).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGHp).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGArmorPct).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGDefPct).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudGCritPct).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWPhys).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWMagic).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWHp).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWArmor).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWMana).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWRegen).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWHeal).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWEvasion).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWCast).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWCdr).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWCost).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudWSlot).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudTime).EndInit();
            pnlLeft.ResumeLayout(false); pnlCampos.ResumeLayout(false); pnlCampos.PerformLayout(); pnlBotones.ResumeLayout(false);
            pnlRight.ResumeLayout(false); pnlEnlace.ResumeLayout(false); pnlEnlace.PerformLayout(); tabMain.ResumeLayout(false);
            tabArbol.ResumeLayout(false); tabResumen.ResumeLayout(false); tabNodos.ResumeLayout(false); tabFicha.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)wvFicha).EndInit();
            menuMain.ResumeLayout(false);
            menuMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
    }
}
