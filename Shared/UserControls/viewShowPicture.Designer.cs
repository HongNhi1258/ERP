namespace DaiCo.Shared
{
  partial class viewShowPicture
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
      this.groupBoxCompImg = new System.Windows.Forms.GroupBox();
      this.picComponent = new System.Windows.Forms.PictureBox();
      this.groupBoxCompImg.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picComponent)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBoxCompImg
      // 
      this.groupBoxCompImg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBoxCompImg.Controls.Add(this.picComponent);
      this.groupBoxCompImg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBoxCompImg.Location = new System.Drawing.Point(0, 0);
      this.groupBoxCompImg.Margin = new System.Windows.Forms.Padding(3, 3, 3, 30);
      this.groupBoxCompImg.Name = "groupBoxCompImg";
      this.groupBoxCompImg.Size = new System.Drawing.Size(501, 365);
      this.groupBoxCompImg.TabIndex = 0;
      this.groupBoxCompImg.TabStop = false;
      this.groupBoxCompImg.Text = "Component Code";
      // 
      // picComponent
      // 
      this.picComponent.Dock = System.Windows.Forms.DockStyle.Fill;
      this.picComponent.Location = new System.Drawing.Point(3, 16);
      this.picComponent.Name = "picComponent";
      this.picComponent.Size = new System.Drawing.Size(495, 346);
      this.picComponent.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picComponent.TabIndex = 0;
      this.picComponent.TabStop = false;
      this.picComponent.DoubleClick += new System.EventHandler(this.picComponent_DoubleClick);
      // 
      // viewShowPicture
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBoxCompImg);
      this.Name = "viewShowPicture";
      this.Size = new System.Drawing.Size(501, 388);
      this.Load += new System.EventHandler(this.viewShowPicture_Load);
      this.groupBoxCompImg.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.picComponent)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBoxCompImg;
    private System.Windows.Forms.PictureBox picComponent;
  }
}
