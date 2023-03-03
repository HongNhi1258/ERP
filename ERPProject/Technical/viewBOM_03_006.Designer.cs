namespace DaiCo.ERPProject
{
  partial class viewBOM_03_006
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
      this.components = new System.ComponentModel.Container();
      this.groupBox6 = new System.Windows.Forms.GroupBox();
      this.txtPrefix = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.txtRemark = new System.Windows.Forms.TextBox();
      this.txtNameVN = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.txtSupCode = new System.Windows.Forms.TextBox();
      this.txtNameEN = new System.Windows.Forms.TextBox();
      this.label16 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.chkLock = new System.Windows.Forms.CheckBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.ultSupportDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.udrpMaterialsCode = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.udpAlternative = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.gbReference = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label6 = new System.Windows.Forms.Label();
      this.btnCopyFromSupport = new System.Windows.Forms.Button();
      this.cmbSupport = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.btnPrint = new System.Windows.Forms.Button();
      this.lbConfirm = new System.Windows.Forms.Label();
      this.groupImportFromExcel = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.label8 = new System.Windows.Forms.Label();
      this.txtImportExcelFile = new System.Windows.Forms.TextBox();
      this.btnGetTemplate = new System.Windows.Forms.Button();
      this.btnBrowseItem = new System.Windows.Forms.Button();
      this.btnImport = new System.Windows.Forms.Button();
      this.lbCount = new System.Windows.Forms.Label();
      this.groupBox6.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultSupportDetail)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.udrpMaterialsCode)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.udpAlternative)).BeginInit();
      this.gbReference.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupImportFromExcel.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox6
      // 
      this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox6.Controls.Add(this.txtPrefix);
      this.groupBox6.Controls.Add(this.label2);
      this.groupBox6.Controls.Add(this.label10);
      this.groupBox6.Controls.Add(this.txtRemark);
      this.groupBox6.Controls.Add(this.txtNameVN);
      this.groupBox6.Controls.Add(this.label3);
      this.groupBox6.Controls.Add(this.label1);
      this.groupBox6.Controls.Add(this.txtSupCode);
      this.groupBox6.Controls.Add(this.txtNameEN);
      this.groupBox6.Controls.Add(this.label16);
      this.groupBox6.Controls.Add(this.label18);
      this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.groupBox6.Location = new System.Drawing.Point(4, 8);
      this.groupBox6.Name = "groupBox6";
      this.groupBox6.Size = new System.Drawing.Size(1008, 137);
      this.groupBox6.TabIndex = 0;
      this.groupBox6.TabStop = false;
      this.groupBox6.Text = "Support Materials";
      // 
      // txtPrefix
      // 
      this.txtPrefix.Location = new System.Drawing.Point(124, 24);
      this.txtPrefix.Name = "txtPrefix";
      this.txtPrefix.ReadOnly = true;
      this.txtPrefix.Size = new System.Drawing.Size(48, 21);
      this.txtPrefix.TabIndex = 46;
      this.txtPrefix.Text = "SUP";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(177, 27);
      this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 1);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(12, 15);
      this.label2.TabIndex = 45;
      this.label2.Text = "-";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.ForeColor = System.Drawing.Color.Red;
      this.label10.Location = new System.Drawing.Point(94, 53);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(23, 15);
      this.label10.TabIndex = 44;
      this.label10.Text = "(*)";
      // 
      // txtRemark
      // 
      this.txtRemark.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtRemark.Location = new System.Drawing.Point(124, 102);
      this.txtRemark.Name = "txtRemark";
      this.txtRemark.Size = new System.Drawing.Size(876, 21);
      this.txtRemark.TabIndex = 3;
      this.txtRemark.TextChanged += new System.EventHandler(this.Object_TextChanged);
      // 
      // txtNameVN
      // 
      this.txtNameVN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtNameVN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtNameVN.Location = new System.Drawing.Point(124, 76);
      this.txtNameVN.Name = "txtNameVN";
      this.txtNameVN.Size = new System.Drawing.Size(876, 21);
      this.txtNameVN.TabIndex = 2;
      this.txtNameVN.TextChanged += new System.EventHandler(this.Object_TextChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label3.Location = new System.Drawing.Point(16, 105);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(57, 15);
      this.label3.TabIndex = 5;
      this.label3.Text = "Remark";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label1.Location = new System.Drawing.Point(15, 79);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(67, 15);
      this.label1.TabIndex = 5;
      this.label1.Text = "Name VN";
      // 
      // txtSupCode
      // 
      this.txtSupCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSupCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.txtSupCode.Location = new System.Drawing.Point(195, 24);
      this.txtSupCode.Name = "txtSupCode";
      this.txtSupCode.Size = new System.Drawing.Size(805, 21);
      this.txtSupCode.TabIndex = 0;
      this.txtSupCode.TextChanged += new System.EventHandler(this.Object_TextChanged);
      this.txtSupCode.Leave += new System.EventHandler(this.txtSupCode_Leave);
      // 
      // txtNameEN
      // 
      this.txtNameEN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtNameEN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtNameEN.Location = new System.Drawing.Point(124, 50);
      this.txtNameEN.Name = "txtNameEN";
      this.txtNameEN.Size = new System.Drawing.Size(876, 21);
      this.txtNameEN.TabIndex = 1;
      this.txtNameEN.TextChanged += new System.EventHandler(this.Object_TextChanged);
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label16.Location = new System.Drawing.Point(15, 53);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(68, 15);
      this.label16.TabIndex = 4;
      this.label16.Text = "Name EN";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label18.Location = new System.Drawing.Point(15, 25);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(94, 15);
      this.label18.TabIndex = 3;
      this.label18.Text = "Support Code";
      // 
      // chkLock
      // 
      this.chkLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.chkLock.AutoSize = true;
      this.chkLock.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.chkLock.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.chkLock.Location = new System.Drawing.Point(680, 524);
      this.chkLock.Name = "chkLock";
      this.chkLock.Size = new System.Drawing.Size(76, 19);
      this.chkLock.TabIndex = 3;
      this.chkLock.Text = "Confirm";
      this.chkLock.UseVisualStyleBackColor = true;
      this.chkLock.CheckedChanged += new System.EventHandler(this.Object_TextChanged);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnSave.Location = new System.Drawing.Point(848, 523);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(80, 23);
      this.btnSave.TabIndex = 5;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnClose.Location = new System.Drawing.Point(934, 523);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(80, 23);
      this.btnClose.TabIndex = 6;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // ultSupportDetail
      // 
      this.ultSupportDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ultSupportDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultSupportDetail.DisplayLayout.AutoFitColumns = true;
      this.ultSupportDetail.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      this.ultSupportDetail.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultSupportDetail.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultSupportDetail.Location = new System.Drawing.Point(2, 202);
      this.ultSupportDetail.Name = "ultSupportDetail";
      this.ultSupportDetail.Size = new System.Drawing.Size(1010, 315);
      this.ultSupportDetail.TabIndex = 2;
      this.ultSupportDetail.AfterRowsDeleted += new System.EventHandler(this.ultSupportDetail_AfterRowsDeleted);
      this.ultSupportDetail.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultSupportDetail_BeforeCellUpdate);
      this.ultSupportDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultSupportDetail_InitializeLayout);
      this.ultSupportDetail.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultSupportDetail_BeforeRowsDeleted);
      this.ultSupportDetail.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultSupportDetail_AfterCellUpdate);
      // 
      // udrpMaterialsCode
      // 
      this.udrpMaterialsCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.udrpMaterialsCode.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.udrpMaterialsCode.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.udrpMaterialsCode.DisplayMember = "";
      this.udrpMaterialsCode.Location = new System.Drawing.Point(23, 239);
      this.udrpMaterialsCode.Name = "udrpMaterialsCode";
      this.udrpMaterialsCode.Size = new System.Drawing.Size(600, 52);
      this.udrpMaterialsCode.TabIndex = 5;
      this.udrpMaterialsCode.ValueMember = "";
      this.udrpMaterialsCode.Visible = false;
      // 
      // udpAlternative
      // 
      this.udpAlternative.Cursor = System.Windows.Forms.Cursors.Default;
      this.udpAlternative.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.udpAlternative.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.udpAlternative.DisplayMember = "";
      this.udpAlternative.Location = new System.Drawing.Point(629, 239);
      this.udpAlternative.Name = "udpAlternative";
      this.udpAlternative.Size = new System.Drawing.Size(256, 52);
      this.udpAlternative.TabIndex = 6;
      this.udpAlternative.ValueMember = "";
      this.udpAlternative.Visible = false;
      // 
      // gbReference
      // 
      this.gbReference.Controls.Add(this.tableLayoutPanel1);
      this.gbReference.Controls.Add(this.checkBox1);
      this.gbReference.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gbReference.Location = new System.Drawing.Point(4, 146);
      this.gbReference.Name = "gbReference";
      this.gbReference.Size = new System.Drawing.Size(343, 50);
      this.gbReference.TabIndex = 1;
      this.gbReference.TabStop = false;
      this.gbReference.Text = "Reference";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnCopyFromSupport, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.cmbSupport, 1, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(337, 30);
      this.tableLayoutPanel1.TabIndex = 5;
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label6.Location = new System.Drawing.Point(3, 7);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(94, 15);
      this.label6.TabIndex = 4;
      this.label6.Text = "Support Code";
      // 
      // btnCopyFromSupport
      // 
      this.btnCopyFromSupport.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnCopyFromSupport.Enabled = false;
      this.btnCopyFromSupport.Location = new System.Drawing.Point(259, 3);
      this.btnCopyFromSupport.Name = "btnCopyFromSupport";
      this.btnCopyFromSupport.Size = new System.Drawing.Size(75, 23);
      this.btnCopyFromSupport.TabIndex = 1;
      this.btnCopyFromSupport.Text = "Copy";
      this.btnCopyFromSupport.UseVisualStyleBackColor = true;
      this.btnCopyFromSupport.Click += new System.EventHandler(this.btnCopyFromSupport_Click);
      // 
      // cmbSupport
      // 
      this.cmbSupport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbSupport.FormattingEnabled = true;
      this.cmbSupport.Location = new System.Drawing.Point(103, 3);
      this.cmbSupport.Name = "cmbSupport";
      this.cmbSupport.Size = new System.Drawing.Size(150, 23);
      this.cmbSupport.TabIndex = 0;
      this.cmbSupport.SelectedIndexChanged += new System.EventHandler(this.cmbSupport_SelectedIndexChanged);
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.checkBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.checkBox1.Location = new System.Drawing.Point(147, 88);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(105, 19);
      this.checkBox1.TabIndex = 3;
      this.checkBox1.Text = "Contract Out";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // btnPrint
      // 
      this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrint.Enabled = false;
      this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrint.Location = new System.Drawing.Point(762, 523);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(80, 23);
      this.btnPrint.TabIndex = 4;
      this.btnPrint.Text = "Print";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // lbConfirm
      // 
      this.lbConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lbConfirm.AutoSize = true;
      this.lbConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbConfirm.ForeColor = System.Drawing.Color.Red;
      this.lbConfirm.Location = new System.Drawing.Point(550, 522);
      this.lbConfirm.Name = "lbConfirm";
      this.lbConfirm.Size = new System.Drawing.Size(120, 24);
      this.lbConfirm.TabIndex = 45;
      this.lbConfirm.Text = "Not Confirm";
      // 
      // groupImportFromExcel
      // 
      this.groupImportFromExcel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupImportFromExcel.Controls.Add(this.tableLayoutPanel4);
      this.groupImportFromExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupImportFromExcel.Location = new System.Drawing.Point(379, 146);
      this.groupImportFromExcel.Name = "groupImportFromExcel";
      this.groupImportFromExcel.Size = new System.Drawing.Size(633, 50);
      this.groupImportFromExcel.TabIndex = 46;
      this.groupImportFromExcel.TabStop = false;
      this.groupImportFromExcel.Text = "Import From Excel";
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 5;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.Controls.Add(this.label8, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.txtImportExcelFile, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnGetTemplate, 4, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnBrowseItem, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnImport, 3, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(627, 31);
      this.tableLayoutPanel4.TabIndex = 8;
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(3, 8);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(57, 13);
      this.label8.TabIndex = 1;
      this.label8.Text = "File Path";
      // 
      // txtImportExcelFile
      // 
      this.txtImportExcelFile.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtImportExcelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtImportExcelFile.Location = new System.Drawing.Point(83, 5);
      this.txtImportExcelFile.Margin = new System.Windows.Forms.Padding(3, 5, 3, 4);
      this.txtImportExcelFile.Name = "txtImportExcelFile";
      this.txtImportExcelFile.Size = new System.Drawing.Size(294, 20);
      this.txtImportExcelFile.TabIndex = 2;
      // 
      // btnGetTemplate
      // 
      this.btnGetTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGetTemplate.AutoSize = true;
      this.btnGetTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnGetTemplate.Location = new System.Drawing.Point(520, 3);
      this.btnGetTemplate.Name = "btnGetTemplate";
      this.btnGetTemplate.Size = new System.Drawing.Size(104, 23);
      this.btnGetTemplate.TabIndex = 5;
      this.btnGetTemplate.Text = "Get Template";
      this.btnGetTemplate.UseVisualStyleBackColor = true;
      this.btnGetTemplate.Click += new System.EventHandler(this.btnGetTemplate_Click);
      // 
      // btnBrowseItem
      // 
      this.btnBrowseItem.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnBrowseItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrowseItem.Location = new System.Drawing.Point(383, 3);
      this.btnBrowseItem.Name = "btnBrowseItem";
      this.btnBrowseItem.Size = new System.Drawing.Size(50, 23);
      this.btnBrowseItem.TabIndex = 3;
      this.btnBrowseItem.Text = "...";
      this.btnBrowseItem.UseVisualStyleBackColor = true;
      this.btnBrowseItem.Click += new System.EventHandler(this.btnBrowseItem_Click);
      // 
      // btnImport
      // 
      this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnImport.Location = new System.Drawing.Point(439, 3);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(75, 23);
      this.btnImport.TabIndex = 4;
      this.btnImport.Text = "Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // lbCount
      // 
      this.lbCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.lbCount.AutoSize = true;
      this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCount.Location = new System.Drawing.Point(440, 529);
      this.lbCount.Name = "lbCount";
      this.lbCount.Size = new System.Drawing.Size(44, 13);
      this.lbCount.TabIndex = 47;
      this.lbCount.Text = "Count:";
      // 
      // viewBOM_03_006
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lbCount);
      this.Controls.Add(this.groupImportFromExcel);
      this.Controls.Add(this.lbConfirm);
      this.Controls.Add(this.btnPrint);
      this.Controls.Add(this.gbReference);
      this.Controls.Add(this.udpAlternative);
      this.Controls.Add(this.udrpMaterialsCode);
      this.Controls.Add(this.ultSupportDetail);
      this.Controls.Add(this.chkLock);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.groupBox6);
      this.Controls.Add(this.btnSave);
      this.Name = "viewBOM_03_006";
      this.Size = new System.Drawing.Size(1018, 551);
      this.Load += new System.EventHandler(this.viewBOM_03_006_Load);
      this.groupBox6.ResumeLayout(false);
      this.groupBox6.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultSupportDetail)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.udrpMaterialsCode)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.udpAlternative)).EndInit();
      this.gbReference.ResumeLayout(false);
      this.gbReference.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.groupImportFromExcel.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox6;
    private System.Windows.Forms.TextBox txtNameEN;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.TextBox txtSupCode;
    private System.Windows.Forms.CheckBox chkLock;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TextBox txtNameVN;
    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultSupportDetail;
    private Infragistics.Win.UltraWinGrid.UltraDropDown udrpMaterialsCode;
    private Infragistics.Win.UltraWinGrid.UltraDropDown udpAlternative;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox txtRemark;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox gbReference;
    private System.Windows.Forms.Button btnCopyFromSupport;
    private DaiCo.Shared.DaiCoComboBox cmbSupport;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtPrefix;
    private System.Windows.Forms.Label lbConfirm;
    private System.Windows.Forms.GroupBox groupImportFromExcel;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox txtImportExcelFile;
    private System.Windows.Forms.Button btnGetTemplate;
    private System.Windows.Forms.Button btnBrowseItem;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label lbCount;
  }
}