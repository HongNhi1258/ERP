namespace DaiCo.ERPProject
{
  partial class viewBOM_03_020
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
      this.btnSearch = new System.Windows.Forms.Button();
      this.txtItemCode = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.txtPackageCode = new System.Windows.Forms.TextBox();
      this.label39 = new System.Windows.Forms.Label();
      this.txtPackageName = new System.Windows.Forms.TextBox();
      this.label37 = new System.Windows.Forms.Label();
      this.btnNew = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnUnLock = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.ultraCBWO = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label3 = new System.Windows.Forms.Label();
      this.txtBarcode = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnDelete = new System.Windows.Forms.Button();
      this.lbCount = new System.Windows.Forms.Label();
      this.btnSetDefault = new System.Windows.Forms.Button();
      this.btnExport = new System.Windows.Forms.Button();
      this.uegMain = new Infragistics.Win.Misc.UltraExpandableGroupBox();
      this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClear = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.ugrdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBWO)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uegMain)).BeginInit();
      this.uegMain.SuspendLayout();
      this.ultraExpandableGroupBoxPanel1.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugrdData)).BeginInit();
      this.SuspendLayout();
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.btnSearch.Image = global::DaiCo.ERPProject.Properties.Resources.Search;
      this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnSearch.Location = new System.Drawing.Point(505, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 1;
      this.btnSearch.Text = "    Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // txtItemCode
      // 
      this.txtItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtItemCode.Location = new System.Drawing.Point(114, 32);
      this.txtItemCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
      this.txtItemCode.Name = "txtItemCode";
      this.txtItemCode.Size = new System.Drawing.Size(464, 21);
      this.txtItemCode.TabIndex = 5;
      this.txtItemCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label1.Location = new System.Drawing.Point(3, 35);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(85, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Mã Sản Phẩm";
      // 
      // txtPackageCode
      // 
      this.txtPackageCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtPackageCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtPackageCode.Location = new System.Drawing.Point(114, 4);
      this.txtPackageCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
      this.txtPackageCode.Name = "txtPackageCode";
      this.txtPackageCode.Size = new System.Drawing.Size(464, 21);
      this.txtPackageCode.TabIndex = 1;
      this.txtPackageCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label39
      // 
      this.label39.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label39.AutoSize = true;
      this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label39.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label39.Location = new System.Drawing.Point(3, 7);
      this.label39.Name = "label39";
      this.label39.Size = new System.Drawing.Size(81, 13);
      this.label39.TabIndex = 0;
      this.label39.Text = "Mã Đóng Gói";
      // 
      // txtPackageName
      // 
      this.txtPackageName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtPackageName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtPackageName.Location = new System.Drawing.Point(736, 4);
      this.txtPackageName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
      this.txtPackageName.Name = "txtPackageName";
      this.txtPackageName.Size = new System.Drawing.Size(465, 21);
      this.txtPackageName.TabIndex = 3;
      this.txtPackageName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label37
      // 
      this.label37.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label37.AutoSize = true;
      this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label37.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label37.Location = new System.Drawing.Point(624, 7);
      this.label37.Name = "label37";
      this.label37.Size = new System.Drawing.Size(86, 13);
      this.label37.TabIndex = 2;
      this.label37.Text = "Tên Đóng Gói";
      // 
      // btnNew
      // 
      this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNew.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.btnNew.Image = global::DaiCo.ERPProject.Properties.Resources.New;
      this.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNew.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnNew.Location = new System.Drawing.Point(1049, 3);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(75, 23);
      this.btnNew.TabIndex = 4;
      this.btnNew.Text = "   Tạo Mới";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnClose.Location = new System.Drawing.Point(1131, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(76, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "    Đóng";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnUnLock
      // 
      this.btnUnLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnUnLock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnUnLock.Image = global::DaiCo.ERPProject.Properties.Resources.Unlock;
      this.btnUnLock.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnUnLock.Location = new System.Drawing.Point(968, 3);
      this.btnUnLock.Name = "btnUnLock";
      this.btnUnLock.Size = new System.Drawing.Size(75, 23);
      this.btnUnLock.TabIndex = 3;
      this.btnUnLock.Text = "    Mở Khóa";
      this.btnUnLock.UseVisualStyleBackColor = true;
      this.btnUnLock.Click += new System.EventHandler(this.btnUnLock_Click);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 64);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(30, 13);
      this.label2.TabIndex = 8;
      this.label2.Text = "LSX";
      // 
      // ultraCBWO
      // 
      this.ultraCBWO.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBWO.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultraCBWO.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBWO.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBWO.Location = new System.Drawing.Point(114, 59);
      this.ultraCBWO.Name = "ultraCBWO";
      this.ultraCBWO.Size = new System.Drawing.Size(464, 23);
      this.ultraCBWO.TabIndex = 9;
      this.ultraCBWO.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraCBWO_InitializeLayout);
      this.ultraCBWO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(624, 35);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(54, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Barcode";
      // 
      // txtBarcode
      // 
      this.txtBarcode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtBarcode.Location = new System.Drawing.Point(736, 32);
      this.txtBarcode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
      this.txtBarcode.Name = "txtBarcode";
      this.txtBarcode.Size = new System.Drawing.Size(465, 20);
      this.txtBarcode.TabIndex = 7;
      this.txtBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.uegMain, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1216, 707);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 9;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.Controls.Add(this.btnClose, 8, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnDelete, 5, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnNew, 7, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnUnLock, 6, 0);
      this.tableLayoutPanel4.Controls.Add(this.lbCount, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSetDefault, 4, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnExport, 3, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 678);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(1210, 29);
      this.tableLayoutPanel4.TabIndex = 2;
      // 
      // btnDelete
      // 
      this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDelete.Image = global::DaiCo.ERPProject.Properties.Resources.Delete;
      this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnDelete.Location = new System.Drawing.Point(887, 3);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(75, 23);
      this.btnDelete.TabIndex = 2;
      this.btnDelete.Text = "    Xóa";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // lbCount
      // 
      this.lbCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbCount.AutoSize = true;
      this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCount.Location = new System.Drawing.Point(310, 8);
      this.lbCount.Name = "lbCount";
      this.lbCount.Size = new System.Drawing.Size(55, 13);
      this.lbCount.TabIndex = 0;
      this.lbCount.Text = "Count: 0";
      // 
      // btnSetDefault
      // 
      this.btnSetDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSetDefault.Image = global::DaiCo.ERPProject.Properties.Resources.edit;
      this.btnSetDefault.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSetDefault.Location = new System.Drawing.Point(759, 3);
      this.btnSetDefault.Name = "btnSetDefault";
      this.btnSetDefault.Size = new System.Drawing.Size(122, 23);
      this.btnSetDefault.TabIndex = 1;
      this.btnSetDefault.Text = "    Đặt Tiêu Chuẩn";
      this.btnSetDefault.UseVisualStyleBackColor = true;
      this.btnSetDefault.Click += new System.EventHandler(this.btnSetDefault_Click);
      // 
      // btnExport
      // 
      this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExport.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnExport.Location = new System.Drawing.Point(678, 3);
      this.btnExport.Name = "btnExport";
      this.btnExport.Size = new System.Drawing.Size(75, 23);
      this.btnExport.TabIndex = 6;
      this.btnExport.Text = "   Xuất";
      this.btnExport.UseVisualStyleBackColor = true;
      this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
      // 
      // uegMain
      // 
      this.uegMain.Controls.Add(this.ultraExpandableGroupBoxPanel1);
      this.uegMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uegMain.ExpandedSize = new System.Drawing.Size(1210, 110);
      this.uegMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uegMain.Location = new System.Drawing.Point(3, 3);
      this.uegMain.Name = "uegMain";
      this.uegMain.Size = new System.Drawing.Size(1210, 110);
      this.uegMain.TabIndex = 0;
      this.uegMain.Text = "Thông Tin Tìm Kiếm";
      // 
      // ultraExpandableGroupBoxPanel1
      // 
      this.ultraExpandableGroupBoxPanel1.Controls.Add(this.tableLayoutPanel3);
      this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
      this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
      this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(1204, 88);
      this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 7;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel3.Controls.Add(this.label2, 0, 2);
      this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 4, 2);
      this.tableLayoutPanel3.Controls.Add(this.ultraCBWO, 2, 2);
      this.tableLayoutPanel3.Controls.Add(this.label39, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.txtPackageCode, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.label37, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.txtPackageName, 6, 0);
      this.tableLayoutPanel3.Controls.Add(this.label3, 4, 1);
      this.tableLayoutPanel3.Controls.Add(this.txtBarcode, 6, 1);
      this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.txtItemCode, 2, 1);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 4;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(1204, 88);
      this.tableLayoutPanel3.TabIndex = 0;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 3;
      this.tableLayoutPanel3.SetColumnSpan(this.tableLayoutPanel5, 3);
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.Controls.Add(this.btnSearch, 2, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnClear, 1, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(621, 56);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 1;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(583, 29);
      this.tableLayoutPanel5.TabIndex = 10;
      // 
      // btnClear
      // 
      this.btnClear.Image = global::DaiCo.ERPProject.Properties.Resources.Clear;
      this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClear.Location = new System.Drawing.Point(403, 3);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(96, 23);
      this.btnClear.TabIndex = 0;
      this.btnClear.Text = "    Làm Mới";
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.ugrdData);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 119);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(1210, 556);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Danh Sách Kết Quả";
      // 
      // ugrdData
      // 
      this.ugrdData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ugrdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ugrdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      this.ugrdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ugrdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ugrdData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ugrdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ugrdData.Location = new System.Drawing.Point(3, 16);
      this.ugrdData.Name = "ugrdData";
      this.ugrdData.Size = new System.Drawing.Size(1204, 537);
      this.ugrdData.TabIndex = 0;
      this.ugrdData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugrdData_InitializeLayout);
      this.ugrdData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ugrdData_MouseDoubleClick);
      // 
      // viewBOM_03_020
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewBOM_03_020";
      this.Size = new System.Drawing.Size(1216, 707);
      this.Load += new System.EventHandler(this.viewBOM_03_020_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBWO)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uegMain)).EndInit();
      this.uegMain.ResumeLayout(false);
      this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugrdData)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.TextBox txtItemCode;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtPackageCode;
    private System.Windows.Forms.Label label39;
    private System.Windows.Forms.TextBox txtPackageName;
    private System.Windows.Forms.Label label37;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnUnLock;
    private System.Windows.Forms.Label label2;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBWO;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtBarcode;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Infragistics.Win.Misc.UltraExpandableGroupBox uegMain;
    private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Label lbCount;
    private System.Windows.Forms.Button btnClear;
    private Infragistics.Win.UltraWinGrid.UltraGrid ugrdData;
    private System.Windows.Forms.Button btnSetDefault;
    private System.Windows.Forms.Button btnExport;
  }
}
