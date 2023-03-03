namespace DaiCo.ERPProject
{
  partial class ProgressForm
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
      this.progress = new System.Windows.Forms.ProgressBar();
      this.labelStatus = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // progress
      // 
      this.progress.Location = new System.Drawing.Point(12, 34);
      this.progress.Name = "progress";
      this.progress.Size = new System.Drawing.Size(460, 23);
      this.progress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.progress.TabIndex = 0;
      // 
      // labelStatus
      // 
      this.labelStatus.AutoSize = true;
      this.labelStatus.Location = new System.Drawing.Point(10, 12);
      this.labelStatus.Name = "labelStatus";
      this.labelStatus.Size = new System.Drawing.Size(73, 13);
      this.labelStatus.TabIndex = 1;
      this.labelStatus.Text = "Please Wait...";
      // 
      // ProgressForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(484, 65);
      this.Controls.Add(this.labelStatus);
      this.Controls.Add(this.progress);
      this.Name = "ProgressForm";
      this.Text = "ProgressForm";
      this.Load += new System.EventHandler(this.ProgressForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ProgressBar progress;
    private System.Windows.Forms.Label labelStatus;
  }
}