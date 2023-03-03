namespace DaiCo.Shared.Utility
{
  partial class ReportViewer
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.Viewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
      this.SuspendLayout();
      // 
      // Viewer
      // 
      this.Viewer.ActiveViewIndex = -1;
      this.Viewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Viewer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.Viewer.Location = new System.Drawing.Point(0, 0);
      this.Viewer.Name = "Viewer";
      this.Viewer.SelectionFormula = "";
      this.Viewer.Size = new System.Drawing.Size(409, 265);
      this.Viewer.TabIndex = 1;
      this.Viewer.ViewTimeSelectionFormula = "";
      // 
      // ReportViewer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(409, 265);
      this.Controls.Add(this.Viewer);
      this.Name = "ReportViewer";
      this.Text = "ReportViewer";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.ResumeLayout(false);

    }

    #endregion

    private CrystalDecisions.Windows.Forms.CrystalReportViewer Viewer;
  }
}