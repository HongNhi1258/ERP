namespace DaiCo.ERPProject
{
  partial class viewPUR_01_005
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtMaterialGroup = new System.Windows.Forms.TextBox();
      this.txtMaterialCategory = new System.Windows.Forms.TextBox();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.btnConfirm = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnDisactive = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 5;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtMaterialGroup, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtMaterialCategory, 4, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(693, 500);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(90, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Material Group";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(352, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(106, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Material Category";
      // 
      // txtMaterialGroup
      // 
      this.txtMaterialGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialGroup.Enabled = false;
      this.txtMaterialGroup.Location = new System.Drawing.Point(101, 3);
      this.txtMaterialGroup.Name = "txtMaterialGroup";
      this.txtMaterialGroup.Size = new System.Drawing.Size(225, 20);
      this.txtMaterialGroup.TabIndex = 0;
      // 
      // txtMaterialCategory
      // 
      this.txtMaterialCategory.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialCategory.Enabled = false;
      this.txtMaterialCategory.Location = new System.Drawing.Point(465, 3);
      this.txtMaterialCategory.Name = "txtMaterialCategory";
      this.txtMaterialCategory.Size = new System.Drawing.Size(225, 20);
      this.txtMaterialCategory.TabIndex = 1;
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 5);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 30);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(687, 440);
      this.ultData.TabIndex = 2;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel2.ColumnCount = 5;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 5);
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnNew, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnConfirm, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnDelete, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnDisactive, 0, 0);
      this.tableLayoutPanel2.Location = new System.Drawing.Point(293, 473);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(400, 27);
      this.tableLayoutPanel2.TabIndex = 3;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(326, 2);
      this.btnClose.Margin = new System.Windows.Forms.Padding(0);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(74, 23);
      this.btnClose.TabIndex = 4;
      this.btnClose.Text = "   Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnNew
      // 
      this.btnNew.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.Image = global::DaiCo.ERPProject.Properties.Resources.New;
      this.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNew.Location = new System.Drawing.Point(174, 2);
      this.btnNew.Margin = new System.Windows.Forms.Padding(0);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(74, 23);
      this.btnNew.TabIndex = 2;
      this.btnNew.Text = "   New";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // btnConfirm
      // 
      this.btnConfirm.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnConfirm.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnConfirm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnConfirm.Location = new System.Drawing.Point(250, 2);
      this.btnConfirm.Margin = new System.Windows.Forms.Padding(0);
      this.btnConfirm.Name = "btnConfirm";
      this.btnConfirm.Size = new System.Drawing.Size(74, 23);
      this.btnConfirm.TabIndex = 3;
      this.btnConfirm.Text = "   Confirm";
      this.btnConfirm.UseVisualStyleBackColor = true;
      this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
      // 
      // btnDelete
      // 
      this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDelete.Image = global::DaiCo.ERPProject.Properties.Resources.Delete;
      this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnDelete.Location = new System.Drawing.Point(98, 2);
      this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(74, 23);
      this.btnDelete.TabIndex = 1;
      this.btnDelete.Text = "   Delete";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // btnDisactive
      // 
      this.btnDisactive.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnDisactive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDisactive.Image = global::DaiCo.ERPProject.Properties.Resources.cancel;
      this.btnDisactive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnDisactive.Location = new System.Drawing.Point(0, 2);
      this.btnDisactive.Margin = new System.Windows.Forms.Padding(0);
      this.btnDisactive.Name = "btnDisactive";
      this.btnDisactive.Size = new System.Drawing.Size(96, 23);
      this.btnDisactive.TabIndex = 0;
      this.btnDisactive.Text = "   Discontinue";
      this.btnDisactive.UseVisualStyleBackColor = true;
      this.btnDisactive.Click += new System.EventHandler(this.btnDisactive_Click);
      // 
      // viewPUR_01_005
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPUR_01_005";
      this.Size = new System.Drawing.Size(693, 500);
      this.Load += new System.EventHandler(this.viewPUR_01_005_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtMaterialGroup;
    private System.Windows.Forms.TextBox txtMaterialCategory;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Button btnConfirm;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Button btnDisactive;
  }
}
