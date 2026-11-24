#nullable disable
namespace GuildlandsTree.App;

partial class ManualForm
{
    private System.ComponentModel.IContainer components = null;
    private RichTextBox rtbManual;
    private Button btnCerrar;
    private Panel pnlPie;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        rtbManual = new RichTextBox(); btnCerrar = new Button(); pnlPie = new Panel();
        pnlPie.SuspendLayout(); SuspendLayout();
        // 
        // rtbManual
        // 
        rtbManual.BackColor = Color.White;
        rtbManual.BorderStyle = BorderStyle.None;
        rtbManual.Dock = DockStyle.Fill;
        rtbManual.Font = new Font("Segoe UI", 10F);
        rtbManual.Name = "rtbManual";
        rtbManual.ReadOnly = true;
        rtbManual.Padding = new Padding(8);
        // 
        // btnCerrar
        // 
        btnCerrar.DialogResult = DialogResult.OK;
        btnCerrar.Location = new Point(618, 8);
        btnCerrar.Name = "btnCerrar";
        btnCerrar.Size = new Size(100, 30);
        btnCerrar.Text = "Cerrar";
        btnCerrar.UseVisualStyleBackColor = true;
        btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        // 
        // pnlPie
        // 
        pnlPie.Controls.Add(btnCerrar);
        pnlPie.Dock = DockStyle.Bottom;
        pnlPie.Name = "pnlPie";
        pnlPie.Size = new Size(740, 46);
        // 
        // ManualForm
        // 
        AcceptButton = btnCerrar;
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(740, 640);
        Controls.Add(rtbManual);
        Controls.Add(pnlPie);
        MinimizeBox = false;
        Name = "ManualForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Manual";
        Load += ManualForm_Load;
        pnlPie.ResumeLayout(false); ResumeLayout(false);
    }
}
