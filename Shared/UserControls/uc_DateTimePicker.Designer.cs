using DaiCo.Shared.Utility;
namespace DaiCo.Shared.UserControls
{
  partial class uc_DateTimePicker
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.ultraDateTimeEditor1 = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      ((System.ComponentModel.ISupportInitialize)(this.ultraDateTimeEditor1)).BeginInit();
      this.SuspendLayout();
      // 
      // ultraDateTimeEditor1
      // 
      this.ultraDateTimeEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraDateTimeEditor1.FormatProvider = new System.Globalization.CultureInfo("vi-VN");
      this.ultraDateTimeEditor1.Location = new System.Drawing.Point(0, 0);
      this.ultraDateTimeEditor1.Name = "ultraDateTimeEditor1";
      this.ultraDateTimeEditor1.Size = new System.Drawing.Size(205, 21);
      this.ultraDateTimeEditor1.TabIndex = 0;
      this.ultraDateTimeEditor1.Value = null;
      this.ultraDateTimeEditor1.ValueChanged += new System.EventHandler(this.ultraDateTimeEditor1_ValueChanged);
      // 
      // uc_DateTimePicker
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.ultraDateTimeEditor1);
      this.Name = "uc_DateTimePicker";
      this.Size = new System.Drawing.Size(205, 21);
      ((System.ComponentModel.ISupportInitialize)(this.ultraDateTimeEditor1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultraDateTimeEditor1;


  }
}
