namespace DaiCo.ERPProject
{
  partial class viewPUR_02_001
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
      this.ultListSupplier = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnSearch = new System.Windows.Forms.Button();
      this.txtEnglishName = new System.Windows.Forms.TextBox();
      this.txtSupplierCode = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtVietnameseName = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.ucbCountry = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label6 = new System.Windows.Forms.Label();
      this.ultCBStatus = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label7 = new System.Windows.Forms.Label();
      this.txtTaxNo = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnNotActived = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.ultListSupplier)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbCountry)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBStatus)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // ultListSupplier
      // 
      this.ultListSupplier.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultListSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultListSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultListSupplier.Location = new System.Drawing.Point(3, 16);
      this.ultListSupplier.Name = "ultListSupplier";
      this.ultListSupplier.Size = new System.Drawing.Size(732, 219);
      this.ultListSupplier.TabIndex = 0;
      this.ultListSupplier.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultListSupplier_InitializeLayout);
      this.ultListSupplier.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultListSupplier_MouseDoubleClick);
      // 
      // btnSearch
      // 
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Image = global::DaiCo.ERPProject.Properties.Resources.Search;
      this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSearch.Location = new System.Drawing.Point(650, 79);
      this.btnSearch.Margin = new System.Windows.Forms.Padding(0);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(82, 25);
      this.btnSearch.TabIndex = 5;
      this.btnSearch.Text = "   Tìm Kiếm";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // txtEnglishName
      // 
      this.tableLayoutPanel3.SetColumnSpan(this.txtEnglishName, 2);
      this.txtEnglishName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtEnglishName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtEnglishName.Location = new System.Drawing.Point(501, 3);
      this.txtEnglishName.Name = "txtEnglishName";
      this.txtEnglishName.Size = new System.Drawing.Size(228, 20);
      this.txtEnglishName.TabIndex = 1;
      this.txtEnglishName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // txtSupplierCode
      // 
      this.txtSupplierCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtSupplierCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtSupplierCode.Location = new System.Drawing.Point(123, 3);
      this.txtSupplierCode.Name = "txtSupplierCode";
      this.txtSupplierCode.Size = new System.Drawing.Size(222, 20);
      this.txtSupplierCode.TabIndex = 0;
      this.txtSupplierCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(53, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Mã NCC";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(381, 6);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(91, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Tên Tiếng Anh";
      // 
      // txtVietnameseName
      // 
      this.txtVietnameseName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtVietnameseName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtVietnameseName.Location = new System.Drawing.Point(123, 29);
      this.txtVietnameseName.Name = "txtVietnameseName";
      this.txtVietnameseName.Size = new System.Drawing.Size(222, 20);
      this.txtVietnameseName.TabIndex = 2;
      this.txtVietnameseName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 32);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(91, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Tên Tiếng Việt";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(3, 59);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(60, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Quốc Gia";
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(661, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(74, 25);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "   Đóng";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnNew
      // 
      this.btnNew.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.Image = global::DaiCo.ERPProject.Properties.Resources.New;
      this.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNew.Location = new System.Drawing.Point(581, 3);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(74, 25);
      this.btnNew.TabIndex = 0;
      this.btnNew.Text = "   Tạo Mới";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 6;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
      this.tableLayoutPanel3.Controls.Add(this.label5, 0, 2);
      this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.txtSupplierCode, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.label2, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.txtVietnameseName, 1, 1);
      this.tableLayoutPanel3.Controls.Add(this.label3, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.txtEnglishName, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.ucbCountry, 1, 2);
      this.tableLayoutPanel3.Controls.Add(this.btnSearch, 5, 3);
      this.tableLayoutPanel3.Controls.Add(this.label6, 3, 1);
      this.tableLayoutPanel3.Controls.Add(this.ultCBStatus, 4, 1);
      this.tableLayoutPanel3.Controls.Add(this.label7, 3, 2);
      this.tableLayoutPanel3.Controls.Add(this.txtTaxNo, 4, 2);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 4;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(732, 140);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // ucbCountry
      // 
      this.ucbCountry.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucbCountry.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbCountry.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucbCountry.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbCountry.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucbCountry.Location = new System.Drawing.Point(123, 55);
      this.ucbCountry.Name = "ucbCountry";
      this.ucbCountry.Size = new System.Drawing.Size(222, 21);
      this.ucbCountry.TabIndex = 12;
      this.ucbCountry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(381, 32);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(43, 13);
      this.label6.TabIndex = 13;
      this.label6.Text = "Status";
      // 
      // ultCBStatus
      // 
      this.tableLayoutPanel3.SetColumnSpan(this.ultCBStatus, 2);
      this.ultCBStatus.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBStatus.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBStatus.Location = new System.Drawing.Point(501, 29);
      this.ultCBStatus.Name = "ultCBStatus";
      this.ultCBStatus.Size = new System.Drawing.Size(228, 20);
      this.ultCBStatus.TabIndex = 14;
      this.ultCBStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(381, 59);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(76, 13);
      this.label7.TabIndex = 16;
      this.label7.Text = "Mã Số Thuế";
      // 
      // txtTaxNo
      // 
      this.tableLayoutPanel3.SetColumnSpan(this.txtTaxNo, 2);
      this.txtTaxNo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtTaxNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtTaxNo.Location = new System.Drawing.Point(501, 55);
      this.txtTaxNo.Name = "txtTaxNo";
      this.txtTaxNo.Size = new System.Drawing.Size(228, 20);
      this.txtTaxNo.TabIndex = 17;
      this.txtTaxNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel3);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(738, 159);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Information";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultListSupplier);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 168);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(738, 238);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Contact Person";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 165F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(744, 446);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Controls.Add(this.btnNew, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnNotActived, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 412);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(738, 31);
      this.tableLayoutPanel2.TabIndex = 2;
      // 
      // btnNotActived
      // 
      this.btnNotActived.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnNotActived.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNotActived.Image = global::DaiCo.ERPProject.Properties.Resources.Delete;
      this.btnNotActived.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNotActived.Location = new System.Drawing.Point(436, 3);
      this.btnNotActived.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.btnNotActived.Name = "btnNotActived";
      this.btnNotActived.Size = new System.Drawing.Size(139, 25);
      this.btnNotActived.TabIndex = 2;
      this.btnNotActived.Text = "   Không Hoạt Động";
      this.btnNotActived.UseVisualStyleBackColor = true;
      this.btnNotActived.Click += new System.EventHandler(this.btnNotActived_Click);
      // 
      // viewPUR_02_001
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPUR_02_001";
      this.Size = new System.Drawing.Size(744, 446);
      this.Load += new System.EventHandler(this.viewPUR_02_001_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultListSupplier)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbCountry)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBStatus)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private Infragistics.Win.UltraWinGrid.UltraGrid ultListSupplier;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox txtVietnameseName;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox txtSupplierCode;
      private System.Windows.Forms.TextBox txtEnglishName;
      private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbCountry;
    private System.Windows.Forms.Button btnNotActived;
    private System.Windows.Forms.Label label6;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBStatus;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox txtTaxNo;

}
}
