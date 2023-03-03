namespace DaiCo.ERPProject
{
  partial class viewWHD_99_002
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
      this.cptItemMaterialViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
      this.SuspendLayout();
      // 
      // cptItemMaterialViewer
      // 
      this.cptItemMaterialViewer.ActiveViewIndex = -1;
      this.cptItemMaterialViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.cptItemMaterialViewer.DisplayGroupTree = false;
      this.cptItemMaterialViewer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cptItemMaterialViewer.Location = new System.Drawing.Point(0, 0);
      this.cptItemMaterialViewer.Name = "cptItemMaterialViewer";
      this.cptItemMaterialViewer.SelectionFormula = "";
      this.cptItemMaterialViewer.Size = new System.Drawing.Size(789, 434);
      this.cptItemMaterialViewer.TabIndex = 0;
      this.cptItemMaterialViewer.ViewTimeSelectionFormula = "";
      // 
      // viewWHD_99_002
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.cptItemMaterialViewer);
      this.Name = "viewWHD_99_002";
      this.Size = new System.Drawing.Size(789, 434);
      this.Load += new System.EventHandler(this.ViewVEN_99_001_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private CrystalDecisions.Windows.Forms.CrystalReportViewer cptItemMaterialViewer;
  }
}