namespace DaiCo.Planning
{
  partial class viewPLN_07_103
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
      this.txtTime = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.txtDepartment = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.txtMaterials = new System.Windows.Forms.TextBox();
      this.txtBOH = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.SuspendLayout();
      // 
      // txtTime
      // 
      this.txtTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtTime.Location = new System.Drawing.Point(96, 9);
      this.txtTime.MaxLength = 50;
      this.txtTime.Name = "txtTime";
      this.txtTime.ReadOnly = true;
      this.txtTime.Size = new System.Drawing.Size(430, 20);
      this.txtTime.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(34, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Time";
      // 
      // ultData
      // 
      this.ultData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Location = new System.Drawing.Point(0, 110);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(526, 208);
      this.ultData.TabIndex = 2;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(457, 324);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // txtDepartment
      // 
      this.txtDepartment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtDepartment.Location = new System.Drawing.Point(96, 34);
      this.txtDepartment.MaxLength = 512;
      this.txtDepartment.Name = "txtDepartment";
      this.txtDepartment.ReadOnly = true;
      this.txtDepartment.Size = new System.Drawing.Size(430, 20);
      this.txtDepartment.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 37);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(72, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Department";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 62);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(52, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Material";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(4, 87);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(33, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "BOH";
      // 
      // txtMaterials
      // 
      this.txtMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMaterials.Location = new System.Drawing.Point(96, 59);
      this.txtMaterials.MaxLength = 512;
      this.txtMaterials.Name = "txtMaterials";
      this.txtMaterials.ReadOnly = true;
      this.txtMaterials.Size = new System.Drawing.Size(430, 20);
      this.txtMaterials.TabIndex = 8;
      // 
      // txtBOH
      // 
      this.txtBOH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtBOH.Location = new System.Drawing.Point(96, 84);
      this.txtBOH.MaxLength = 512;
      this.txtBOH.Name = "txtBOH";
      this.txtBOH.ReadOnly = true;
      this.txtBOH.Size = new System.Drawing.Size(430, 20);
      this.txtBOH.TabIndex = 9;
      // 
      // viewPLN_07_103
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.txtBOH);
      this.Controls.Add(this.txtMaterials);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.ultData);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtDepartment);
      this.Controls.Add(this.txtTime);
      this.Name = "viewPLN_07_103";
      this.Size = new System.Drawing.Size(532, 350);
      this.Load += new System.EventHandler(this.viewPLN_07_103_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtTime;
    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TextBox txtDepartment;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtMaterials;
    private System.Windows.Forms.TextBox txtBOH;
  }
}
