namespace DaiCo.ERPProject
{
  partial class viewWHD_05_002
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.ucbActionCode = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnPrint = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.lbCount = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.ultCBWarehouse = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label10 = new System.Windows.Forms.Label();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.txtRecNoFrom = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtRecNoTo = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
      this.ultDateFrom = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.label4 = new System.Windows.Forms.Label();
      this.ultDateTo = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.label8 = new System.Windows.Forms.Label();
      this.ultCBMaterialCode = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.ultCBRecType = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label5 = new System.Windows.Forms.Label();
      this.ultCBPosting = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label14 = new System.Windows.Forms.Label();
      this.ultCBSupplier = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbActionCode)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBWarehouse)).BeginInit();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel7.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDateFrom)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDateTo)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBMaterialCode)).BeginInit();
      this.tableLayoutPanel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBRecType)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBPosting)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBSupplier)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 142F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(953, 600);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.ultData);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 145);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(947, 417);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Danh sách phiếu nhập";
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 16);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(941, 398);
      this.ultData.TabIndex = 0;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.DoubleClick += new System.EventHandler(this.ultData_DoubleClick);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 7;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.Controls.Add(this.ucbActionCode, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 6, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnPrint, 5, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnNew, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.lbCount, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 568);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(947, 29);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // ucbActionCode
      // 
      this.ucbActionCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucbActionCode.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbActionCode.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucbActionCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbActionCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucbActionCode.Location = new System.Drawing.Point(556, 3);
      this.ucbActionCode.Name = "ucbActionCode";
      this.ucbActionCode.Size = new System.Drawing.Size(144, 23);
      this.ucbActionCode.TabIndex = 1;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(868, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 4;
      this.btnClose.Text = "   Đóng";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnPrint
      // 
      this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrint.Image = global::DaiCo.ERPProject.Properties.Resources.Print;
      this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnPrint.Location = new System.Drawing.Point(787, 3);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(75, 23);
      this.btnPrint.TabIndex = 3;
      this.btnPrint.Text = "   In";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // btnNew
      // 
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.Image = global::DaiCo.ERPProject.Properties.Resources.New;
      this.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNew.Location = new System.Drawing.Point(706, 3);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(75, 23);
      this.btnNew.TabIndex = 2;
      this.btnNew.Text = "   Tạo Mới";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // lbCount
      // 
      this.lbCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbCount.AutoSize = true;
      this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCount.Location = new System.Drawing.Point(336, 8);
      this.lbCount.Name = "lbCount";
      this.lbCount.Size = new System.Drawing.Size(47, 13);
      this.lbCount.TabIndex = 0;
      this.lbCount.Text = "Đếm: 0";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.tableLayoutPanel2);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(947, 136);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Điều kiện tìm kiếm";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 8;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.ultCBWarehouse, 1, 2);
      this.tableLayoutPanel2.Controls.Add(this.label10, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel7, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.label8, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultCBMaterialCode, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 7, 3);
      this.tableLayoutPanel2.Controls.Add(this.label6, 3, 1);
      this.tableLayoutPanel2.Controls.Add(this.ultCBRecType, 4, 1);
      this.tableLayoutPanel2.Controls.Add(this.label5, 6, 1);
      this.tableLayoutPanel2.Controls.Add(this.ultCBPosting, 7, 1);
      this.tableLayoutPanel2.Controls.Add(this.label14, 3, 2);
      this.tableLayoutPanel2.Controls.Add(this.ultCBSupplier, 4, 2);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 5;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(941, 117);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(94, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Mã Phiếu Nhập";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(70, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Ngày Nhập";
      // 
      // ultCBWarehouse
      // 
      this.ultCBWarehouse.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBWarehouse.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultCBWarehouse.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBWarehouse.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBWarehouse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBWarehouse.Location = new System.Drawing.Point(103, 57);
      this.ultCBWarehouse.Name = "ultCBWarehouse";
      this.ultCBWarehouse.Size = new System.Drawing.Size(211, 21);
      this.ultCBWarehouse.TabIndex = 13;
      // 
      // label10
      // 
      this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(3, 61);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(29, 13);
      this.label10.TabIndex = 12;
      this.label10.Text = "Kho";
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 3;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.Controls.Add(this.txtRecNoFrom, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.label3, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.txtRecNoTo, 2, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(100, 0);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(217, 27);
      this.tableLayoutPanel4.TabIndex = 22;
      // 
      // txtRecNoFrom
      // 
      this.txtRecNoFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtRecNoFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtRecNoFrom.Location = new System.Drawing.Point(3, 4);
      this.txtRecNoFrom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
      this.txtRecNoFrom.Name = "txtRecNoFrom";
      this.txtRecNoFrom.Size = new System.Drawing.Size(92, 20);
      this.txtRecNoFrom.TabIndex = 1;
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(101, 7);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(15, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "~";
      // 
      // txtRecNoTo
      // 
      this.txtRecNoTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtRecNoTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtRecNoTo.Location = new System.Drawing.Point(122, 4);
      this.txtRecNoTo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
      this.txtRecNoTo.Name = "txtRecNoTo";
      this.txtRecNoTo.Size = new System.Drawing.Size(92, 20);
      this.txtRecNoTo.TabIndex = 3;
      // 
      // tableLayoutPanel7
      // 
      this.tableLayoutPanel7.ColumnCount = 3;
      this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel7.Controls.Add(this.ultDateFrom, 0, 0);
      this.tableLayoutPanel7.Controls.Add(this.label4, 1, 0);
      this.tableLayoutPanel7.Controls.Add(this.ultDateTo, 2, 0);
      this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel7.Location = new System.Drawing.Point(100, 27);
      this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel7.Name = "tableLayoutPanel7";
      this.tableLayoutPanel7.RowCount = 1;
      this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel7.Size = new System.Drawing.Size(217, 27);
      this.tableLayoutPanel7.TabIndex = 23;
      // 
      // ultDateFrom
      // 
      this.ultDateFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDateFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDateFrom.FormatString = "dd/MM/yyyy";
      this.ultDateFrom.Location = new System.Drawing.Point(3, 3);
      this.ultDateFrom.Name = "ultDateFrom";
      this.ultDateFrom.Size = new System.Drawing.Size(92, 21);
      this.ultDateFrom.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(101, 7);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(15, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "~";
      // 
      // ultDateTo
      // 
      this.ultDateTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDateTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDateTo.FormatString = "dd/MM/yyyy";
      this.ultDateTo.Location = new System.Drawing.Point(122, 3);
      this.ultDateTo.Name = "ultDateTo";
      this.ultDateTo.Size = new System.Drawing.Size(92, 21);
      this.ultDateTo.TabIndex = 9;
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(340, 7);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(64, 13);
      this.label8.TabIndex = 16;
      this.label8.Text = "Sản Phẩm";
      // 
      // ultCBMaterialCode
      // 
      this.tableLayoutPanel2.SetColumnSpan(this.ultCBMaterialCode, 4);
      this.ultCBMaterialCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBMaterialCode.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
      this.ultCBMaterialCode.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBMaterialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBMaterialCode.Location = new System.Drawing.Point(444, 3);
      this.ultCBMaterialCode.Name = "ultCBMaterialCode";
      this.ultCBMaterialCode.Size = new System.Drawing.Size(494, 21);
      this.ultCBMaterialCode.TabIndex = 17;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 3;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.Controls.Add(this.btnSearch, 2, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnClear, 1, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(756, 81);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 1;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(182, 29);
      this.tableLayoutPanel5.TabIndex = 21;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Image = global::DaiCo.ERPProject.Properties.Resources.Search;
      this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSearch.Location = new System.Drawing.Point(104, 4);
      this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 1;
      this.btnSearch.Text = "Tìm";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnClear
      // 
      this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClear.Image = global::DaiCo.ERPProject.Properties.Resources.Clear;
      this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClear.Location = new System.Drawing.Point(23, 4);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(75, 23);
      this.btnClear.TabIndex = 0;
      this.btnClear.Text = "   Xóa";
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(340, 34);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(98, 13);
      this.label6.TabIndex = 10;
      this.label6.Text = "Loại phiếu nhập";
      // 
      // ultCBRecType
      // 
      this.ultCBRecType.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBRecType.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultCBRecType.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBRecType.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBRecType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBRecType.Location = new System.Drawing.Point(444, 30);
      this.ultCBRecType.Name = "ultCBRecType";
      this.ultCBRecType.Size = new System.Drawing.Size(211, 21);
      this.ultCBRecType.TabIndex = 11;
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(681, 34);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(69, 13);
      this.label5.TabIndex = 4;
      this.label5.Text = "Trạng Thái";
      // 
      // ultCBPosting
      // 
      this.ultCBPosting.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBPosting.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultCBPosting.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBPosting.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBPosting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBPosting.Location = new System.Drawing.Point(756, 30);
      this.ultCBPosting.Name = "ultCBPosting";
      this.ultCBPosting.Size = new System.Drawing.Size(182, 21);
      this.ultCBPosting.TabIndex = 5;
      // 
      // label14
      // 
      this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(340, 61);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(89, 13);
      this.label14.TabIndex = 18;
      this.label14.Text = "Nhà Cung Cấp";
      // 
      // ultCBSupplier
      // 
      this.tableLayoutPanel2.SetColumnSpan(this.ultCBSupplier, 4);
      this.ultCBSupplier.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBSupplier.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
      this.ultCBSupplier.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBSupplier.Location = new System.Drawing.Point(444, 57);
      this.ultCBSupplier.Name = "ultCBSupplier";
      this.ultCBSupplier.Size = new System.Drawing.Size(494, 21);
      this.ultCBSupplier.TabIndex = 19;
      // 
      // viewWHD_05_002
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewWHD_05_002";
      this.Size = new System.Drawing.Size(953, 600);
      this.Load += new System.EventHandler(this.viewWHD_05_002_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbActionCode)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBWarehouse)).EndInit();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.tableLayoutPanel7.ResumeLayout(false);
      this.tableLayoutPanel7.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDateFrom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDateTo)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBMaterialCode)).EndInit();
      this.tableLayoutPanel5.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultCBRecType)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBPosting)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBSupplier)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.GroupBox groupBox1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox txtRecNoFrom;
    private System.Windows.Forms.TextBox txtRecNoTo;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBPosting;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultDateFrom;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultDateTo;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBRecType;
    private System.Windows.Forms.Label label10;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBMaterialCode;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBWarehouse;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label14;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBSupplier;
    private System.Windows.Forms.Button btnPrint;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbActionCode;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Label lbCount;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
  }
}
