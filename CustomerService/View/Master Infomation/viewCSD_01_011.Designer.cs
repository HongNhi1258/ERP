namespace DaiCo.CustomerService
{
  partial class viewCSD_01_011
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
      this.btnClose = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.btnSreach = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txtNameEN = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.txtNameVN = new System.Windows.Forms.TextBox();
      this.GVGroup = new System.Windows.Forms.DataGridView();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.GVGroup)).BeginInit();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(490, 383);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 7;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.btnSreach);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.txtNameEN);
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.txtNameVN);
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(5, 8);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(560, 96);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Search Information";
      // 
      // btnSreach
      // 
      this.btnSreach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSreach.Location = new System.Drawing.Point(477, 66);
      this.btnSreach.Name = "btnSreach";
      this.btnSreach.Size = new System.Drawing.Size(75, 23);
      this.btnSreach.TabIndex = 3;
      this.btnSreach.Text = "Search";
      this.btnSreach.UseVisualStyleBackColor = true;
      this.btnSreach.Click += new System.EventHandler(this.btnSreach_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(11, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(60, 13);
      this.label1.TabIndex = 101;
      this.label1.Text = "EN Name";
      // 
      // txtNameEN
      // 
      this.txtNameEN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtNameEN.Location = new System.Drawing.Point(84, 16);
      this.txtNameEN.Name = "txtNameEN";
      this.txtNameEN.Size = new System.Drawing.Size(468, 20);
      this.txtNameEN.TabIndex = 1;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(11, 44);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(60, 13);
      this.label5.TabIndex = 102;
      this.label5.Text = "VN Name";
      // 
      // txtNameVN
      // 
      this.txtNameVN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtNameVN.Location = new System.Drawing.Point(84, 40);
      this.txtNameVN.Name = "txtNameVN";
      this.txtNameVN.Size = new System.Drawing.Size(468, 20);
      this.txtNameVN.TabIndex = 2;
      // 
      // GVGroup
      // 
      this.GVGroup.AllowUserToAddRows = false;
      this.GVGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.GVGroup.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.GVGroup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.GVGroup.Location = new System.Drawing.Point(5, 108);
      this.GVGroup.Name = "GVGroup";
      this.GVGroup.ReadOnly = true;
      this.GVGroup.Size = new System.Drawing.Size(560, 270);
      this.GVGroup.TabIndex = 4;
      this.GVGroup.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.GVGroup_CellDoubleClick);
      // 
      // viewCSD_01_011
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.GVGroup);
      this.Name = "viewCSD_01_011";
      this.Size = new System.Drawing.Size(570, 409);
      this.Load += new System.EventHandler(this.viewCSD_01_011_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.GVGroup)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btnSreach;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtNameEN;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox txtNameVN;
    private System.Windows.Forms.DataGridView GVGroup;
  }
}