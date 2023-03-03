namespace DaiCo.ERPProject
{
  partial class viewPLN_05_009
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
      this.components = new System.ComponentModel.Container();
      this.chkConfirm = new System.Windows.Forms.CheckBox();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnPrint = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.ultDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnAddItem = new System.Windows.Forms.Button();
      this.cmbDirectCustomer = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.label9 = new System.Windows.Forms.Label();
      this.txtCustomerPoCancelNo = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.txtPoCancelNo = new System.Windows.Forms.TextBox();
      this.cmbCustomer = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.label3 = new System.Windows.Forms.Label();
      this.dtp_date = new DaiCo.Shared.UserControls.uc_DateTimePicker();
      this.label10 = new System.Windows.Forms.Label();
      this.chkDirect = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.txtRemark = new System.Windows.Forms.TextBox();
      this.txtRef = new System.Windows.Forms.TextBox();
      this.txtContract = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnPrintPDF = new System.Windows.Forms.Button();
      this.btnSaveConfirm = new System.Windows.Forms.Button();
      this.chkPLNcomfirm = new System.Windows.Forms.CheckBox();
      this.groupDetail = new System.Windows.Forms.GroupBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.textBox4 = new System.Windows.Forms.TextBox();
      this.chkSwapWO = new System.Windows.Forms.CheckBox();
      this.groupItemImage = new System.Windows.Forms.GroupBox();
      this.pictureItem = new System.Windows.Forms.PictureBox();
      this.chkShowImage = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.ultContainerEffect = new Infragistics.Win.UltraWinGrid.UltraGrid();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.groupDetail.SuspendLayout();
      this.groupItemImage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureItem)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultContainerEffect)).BeginInit();
      this.SuspendLayout();
      // 
      // chkConfirm
      // 
      this.chkConfirm.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.chkConfirm.AutoSize = true;
      this.chkConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkConfirm.Location = new System.Drawing.Point(338, 5);
      this.chkConfirm.Name = "chkConfirm";
      this.chkConfirm.Size = new System.Drawing.Size(68, 17);
      this.chkConfirm.TabIndex = 0;
      this.chkConfirm.Text = "Confirm";
      this.chkConfirm.UseVisualStyleBackColor = true;
      this.chkConfirm.Visible = false;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(888, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(94, 22);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnPrint
      // 
      this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnPrint.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrint.Location = new System.Drawing.Point(688, 3);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(94, 22);
      this.btnPrint.TabIndex = 2;
      this.btnPrint.Text = "Export Exel";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(412, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(94, 22);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Visible = false;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // ultraGrid1
      // 
      this.ultraGrid1.Location = new System.Drawing.Point(0, 0);
      this.ultraGrid1.Name = "ultraGrid1";
      this.ultraGrid1.Size = new System.Drawing.Size(384, 80);
      this.ultraGrid1.TabIndex = 0;
      // 
      // ultDetail
      // 
      this.ultDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDetail.Location = new System.Drawing.Point(3, 16);
      this.ultDetail.Name = "ultDetail";
      this.ultDetail.Size = new System.Drawing.Size(979, 354);
      this.ultDetail.TabIndex = 1;
      this.ultDetail.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultDetail_ClickCellButton);
      this.ultDetail.AfterRowsDeleted += new System.EventHandler(this.ultDetail_AfterRowsDeleted);
      this.ultDetail.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultDetail_BeforeCellUpdate);
      this.ultDetail.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultDetail_MouseDoubleClick);
      this.ultDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultDetail_InitializeLayout);
      this.ultDetail.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultDetail_BeforeRowsDeleted);
      this.ultDetail.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultDetail_AfterCellUpdate);
      this.ultDetail.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ultDetail_InitializeRow);
      this.ultDetail.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ultDetail_MouseClick);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(985, 126);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Sale Order Cancel Information";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 8;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel1.Controls.Add(this.btnAddItem, 7, 3);
      this.tableLayoutPanel1.Controls.Add(this.cmbDirectCustomer, 6, 1);
      this.tableLayoutPanel1.Controls.Add(this.label9, 5, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtCustomerPoCancelNo, 6, 0);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.label7, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label8, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtPoCancelNo, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.cmbCustomer, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.dtp_date, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.label10, 5, 1);
      this.tableLayoutPanel1.Controls.Add(this.chkDirect, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.label1, 4, 0);
      this.tableLayoutPanel1.Controls.Add(this.label6, 4, 3);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.label11, 4, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtRemark, 6, 3);
      this.tableLayoutPanel1.Controls.Add(this.txtRef, 6, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtContract, 2, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(979, 107);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnAddItem
      // 
      this.btnAddItem.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnAddItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnAddItem.Location = new System.Drawing.Point(900, 84);
      this.btnAddItem.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
      this.btnAddItem.Name = "btnAddItem";
      this.btnAddItem.Size = new System.Drawing.Size(79, 23);
      this.btnAddItem.TabIndex = 5;
      this.btnAddItem.Text = "Add Item";
      this.btnAddItem.UseVisualStyleBackColor = true;
      this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
      // 
      // cmbDirectCustomer
      // 
      this.cmbDirectCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cmbDirectCustomer.Enabled = false;
      this.cmbDirectCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cmbDirectCustomer.FormattingEnabled = true;
      this.cmbDirectCustomer.Location = new System.Drawing.Point(649, 30);
      this.cmbDirectCustomer.Name = "cmbDirectCustomer";
      this.cmbDirectCustomer.Size = new System.Drawing.Size(247, 21);
      this.cmbDirectCustomer.TabIndex = 2;
      this.cmbDirectCustomer.SelectedIndexChanged += new System.EventHandler(this.cmbDirectCustomer_SelectedIndexChanged);
      this.cmbDirectCustomer.TextChanged += new System.EventHandler(this.Object_Changed);
      // 
      // label9
      // 
      this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.ForeColor = System.Drawing.Color.Red;
      this.label9.Location = new System.Drawing.Point(619, 6);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(23, 15);
      this.label9.TabIndex = 58;
      this.label9.Text = "(*)";
      // 
      // txtCustomerPoCancelNo
      // 
      this.txtCustomerPoCancelNo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtCustomerPoCancelNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtCustomerPoCancelNo.Location = new System.Drawing.Point(649, 3);
      this.txtCustomerPoCancelNo.Name = "txtCustomerPoCancelNo";
      this.txtCustomerPoCancelNo.Size = new System.Drawing.Size(247, 20);
      this.txtCustomerPoCancelNo.TabIndex = 0;
      this.txtCustomerPoCancelNo.TextChanged += new System.EventHandler(this.Object_Changed);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(59, 13);
      this.label2.TabIndex = 1004;
      this.label2.Text = "Customer";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(3, 61);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(112, 13);
      this.label5.TabIndex = 1008;
      this.label5.Text = "Cancellation  Date";
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.ForeColor = System.Drawing.Color.Red;
      this.label7.Location = new System.Drawing.Point(141, 33);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(23, 15);
      this.label7.TabIndex = 1012;
      this.label7.Text = "(*)";
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.ForeColor = System.Drawing.Color.Red;
      this.label8.Location = new System.Drawing.Point(141, 60);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(23, 15);
      this.label8.TabIndex = 1013;
      this.label8.Text = "(*)";
      // 
      // txtPoCancelNo
      // 
      this.txtPoCancelNo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtPoCancelNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtPoCancelNo.Location = new System.Drawing.Point(170, 3);
      this.txtPoCancelNo.Name = "txtPoCancelNo";
      this.txtPoCancelNo.ReadOnly = true;
      this.txtPoCancelNo.Size = new System.Drawing.Size(247, 20);
      this.txtPoCancelNo.TabIndex = 0;
      // 
      // cmbCustomer
      // 
      this.cmbCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cmbCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cmbCustomer.FormattingEnabled = true;
      this.cmbCustomer.Location = new System.Drawing.Point(170, 30);
      this.cmbCustomer.Name = "cmbCustomer";
      this.cmbCustomer.Size = new System.Drawing.Size(247, 21);
      this.cmbCustomer.TabIndex = 1;
      this.cmbCustomer.SelectedIndexChanged += new System.EventHandler(this.cmbCustomer_SelectedIndexChanged);
      this.cmbCustomer.TextChanged += new System.EventHandler(this.Object_Changed);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 7);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(130, 13);
      this.label3.TabIndex = 22;
      this.label3.Text = "Sale Order Cancel No";
      // 
      // dtp_date
      // 
      this.dtp_date.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dtp_date.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dtp_date.Location = new System.Drawing.Point(170, 57);
      this.dtp_date.Name = "dtp_date";
      this.dtp_date.Size = new System.Drawing.Size(247, 21);
      this.dtp_date.TabIndex = 3;
      this.dtp_date.Value = new System.DateTime(2010, 6, 25, 10, 27, 49, 626);
      // 
      // label10
      // 
      this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.ForeColor = System.Drawing.Color.Red;
      this.label10.Location = new System.Drawing.Point(619, 33);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(23, 15);
      this.label10.TabIndex = 1014;
      this.label10.Text = "(*)";
      // 
      // chkDirect
      // 
      this.chkDirect.AutoSize = true;
      this.chkDirect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.chkDirect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.chkDirect.Location = new System.Drawing.Point(469, 30);
      this.chkDirect.Name = "chkDirect";
      this.chkDirect.Size = new System.Drawing.Size(129, 19);
      this.chkDirect.TabIndex = 4;
      this.chkDirect.Text = "Direct Customer";
      this.chkDirect.UseVisualStyleBackColor = true;
      this.chkDirect.CheckedChanged += new System.EventHandler(this.chkDirect_CheckedChanged);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(469, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(115, 13);
      this.label1.TabIndex = 1002;
      this.label1.Text = "Cus\' SO Cancel No";
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(469, 88);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(50, 13);
      this.label6.TabIndex = 1011;
      this.label6.Text = "Remark";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(3, 88);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(67, 13);
      this.label4.TabIndex = 1011;
      this.label4.Text = "Contract #";
      // 
      // label11
      // 
      this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(469, 61);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(43, 13);
      this.label11.TabIndex = 1011;
      this.label11.Text = "REF #";
      // 
      // txtRemark
      // 
      this.txtRemark.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtRemark.Location = new System.Drawing.Point(649, 84);
      this.txtRemark.Name = "txtRemark";
      this.txtRemark.Size = new System.Drawing.Size(247, 20);
      this.txtRemark.TabIndex = 4;
      this.txtRemark.TextChanged += new System.EventHandler(this.Object_Changed);
      // 
      // txtRef
      // 
      this.txtRef.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtRef.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtRef.Location = new System.Drawing.Point(649, 57);
      this.txtRef.Name = "txtRef";
      this.txtRef.Size = new System.Drawing.Size(247, 20);
      this.txtRef.TabIndex = 4;
      this.txtRef.TextChanged += new System.EventHandler(this.Object_Changed);
      // 
      // txtContract
      // 
      this.txtContract.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtContract.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtContract.Location = new System.Drawing.Point(170, 84);
      this.txtContract.Name = "txtContract";
      this.txtContract.Size = new System.Drawing.Size(247, 20);
      this.txtContract.TabIndex = 4;
      this.txtContract.TextChanged += new System.EventHandler(this.Object_Changed);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 7;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.Controls.Add(this.btnPrint, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 6, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnPrintPDF, 5, 0);
      this.tableLayoutPanel2.Controls.Add(this.chkConfirm, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnSave, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnSaveConfirm, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.chkPLNcomfirm, 2, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 600);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(985, 28);
      this.tableLayoutPanel2.TabIndex = 2;
      // 
      // btnPrintPDF
      // 
      this.btnPrintPDF.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnPrintPDF.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrintPDF.Location = new System.Drawing.Point(788, 3);
      this.btnPrintPDF.Name = "btnPrintPDF";
      this.btnPrintPDF.Size = new System.Drawing.Size(94, 22);
      this.btnPrintPDF.TabIndex = 4;
      this.btnPrintPDF.Text = "Print";
      this.btnPrintPDF.UseVisualStyleBackColor = true;
      this.btnPrintPDF.Click += new System.EventHandler(this.btnPrintPDF_Click);
      // 
      // btnSaveConfirm
      // 
      this.btnSaveConfirm.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSaveConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSaveConfirm.Location = new System.Drawing.Point(588, 3);
      this.btnSaveConfirm.Name = "btnSaveConfirm";
      this.btnSaveConfirm.Size = new System.Drawing.Size(94, 22);
      this.btnSaveConfirm.TabIndex = 5;
      this.btnSaveConfirm.Text = "Save";
      this.btnSaveConfirm.UseVisualStyleBackColor = true;
      this.btnSaveConfirm.Click += new System.EventHandler(this.btnSaveConfirm_Click);
      // 
      // chkPLNcomfirm
      // 
      this.chkPLNcomfirm.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.chkPLNcomfirm.AutoSize = true;
      this.chkPLNcomfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkPLNcomfirm.Location = new System.Drawing.Point(512, 5);
      this.chkPLNcomfirm.Name = "chkPLNcomfirm";
      this.chkPLNcomfirm.Size = new System.Drawing.Size(70, 17);
      this.chkPLNcomfirm.TabIndex = 6;
      this.chkPLNcomfirm.Text = "Comfirm";
      this.chkPLNcomfirm.UseVisualStyleBackColor = true;
      // 
      // groupDetail
      // 
      this.groupDetail.Controls.Add(this.textBox1);
      this.groupDetail.Controls.Add(this.textBox4);
      this.groupDetail.Controls.Add(this.chkSwapWO);
      this.groupDetail.Controls.Add(this.groupItemImage);
      this.groupDetail.Controls.Add(this.chkShowImage);
      this.groupDetail.Controls.Add(this.ultDetail);
      this.groupDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupDetail.Location = new System.Drawing.Point(3, 135);
      this.groupDetail.Name = "groupDetail";
      this.groupDetail.Size = new System.Drawing.Size(985, 373);
      this.groupDetail.TabIndex = 13;
      this.groupDetail.TabStop = false;
      this.groupDetail.Text = "SOC Detail";
      // 
      // textBox1
      // 
      this.textBox1.BackColor = System.Drawing.Color.LightBlue;
      this.textBox1.Enabled = false;
      this.textBox1.Location = new System.Drawing.Point(452, -4);
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(173, 20);
      this.textBox1.TabIndex = 44;
      this.textBox1.Text = "Sale Order Swap Cancel";
      this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // textBox4
      // 
      this.textBox4.BackColor = System.Drawing.Color.Wheat;
      this.textBox4.Enabled = false;
      this.textBox4.Location = new System.Drawing.Point(321, -4);
      this.textBox4.Name = "textBox4";
      this.textBox4.ReadOnly = true;
      this.textBox4.Size = new System.Drawing.Size(125, 20);
      this.textBox4.TabIndex = 44;
      this.textBox4.Text = "Sale Order Cancel";
      this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // chkSwapWO
      // 
      this.chkSwapWO.AutoSize = true;
      this.chkSwapWO.Location = new System.Drawing.Point(188, -1);
      this.chkSwapWO.Name = "chkSwapWO";
      this.chkSwapWO.Size = new System.Drawing.Size(82, 17);
      this.chkSwapWO.TabIndex = 4;
      this.chkSwapWO.Text = "Swap WO";
      this.chkSwapWO.UseVisualStyleBackColor = true;
      // 
      // groupItemImage
      // 
      this.groupItemImage.Controls.Add(this.pictureItem);
      this.groupItemImage.Location = new System.Drawing.Point(325, 87);
      this.groupItemImage.Name = "groupItemImage";
      this.groupItemImage.Size = new System.Drawing.Size(239, 197);
      this.groupItemImage.TabIndex = 3;
      this.groupItemImage.TabStop = false;
      this.groupItemImage.Text = "Item Image";
      this.groupItemImage.Visible = false;
      // 
      // pictureItem
      // 
      this.pictureItem.BackColor = System.Drawing.Color.Transparent;
      this.pictureItem.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureItem.ImageLocation = "";
      this.pictureItem.Location = new System.Drawing.Point(3, 16);
      this.pictureItem.Name = "pictureItem";
      this.pictureItem.Size = new System.Drawing.Size(233, 178);
      this.pictureItem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureItem.TabIndex = 12;
      this.pictureItem.TabStop = false;
      // 
      // chkShowImage
      // 
      this.chkShowImage.AutoSize = true;
      this.chkShowImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkShowImage.Location = new System.Drawing.Point(87, -1);
      this.chkShowImage.Name = "chkShowImage";
      this.chkShowImage.Size = new System.Drawing.Size(95, 17);
      this.chkShowImage.TabIndex = 0;
      this.chkShowImage.Text = "Show Image";
      this.chkShowImage.UseVisualStyleBackColor = true;
      this.chkShowImage.CheckedChanged += new System.EventHandler(this.chkShowImage_CheckedChanged);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 0, 3);
      this.tableLayoutPanel3.Controls.Add(this.groupDetail, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.ultContainerEffect, 0, 2);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 4;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 132F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(991, 631);
      this.tableLayoutPanel3.TabIndex = 14;
      // 
      // ultContainerEffect
      // 
      this.ultContainerEffect.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultContainerEffect.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultContainerEffect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultContainerEffect.Location = new System.Drawing.Point(3, 514);
      this.ultContainerEffect.Name = "ultContainerEffect";
      this.ultContainerEffect.Size = new System.Drawing.Size(985, 80);
      this.ultContainerEffect.TabIndex = 14;
      this.ultContainerEffect.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultContainerEffect_InitializeLayout);
      // 
      // viewPLN_05_009
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel3);
      this.Name = "viewPLN_05_009";
      this.Size = new System.Drawing.Size(991, 631);
      this.Load += new System.EventHandler(this.viewPLN_05_009_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.groupDetail.ResumeLayout(false);
      this.groupDetail.PerformLayout();
      this.groupItemImage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureItem)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultContainerEffect)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultDetail;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtPoCancelNo;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnAddItem;
    private System.Windows.Forms.GroupBox groupDetail;
    private System.Windows.Forms.GroupBox groupItemImage;
    private System.Windows.Forms.PictureBox pictureItem;
    private System.Windows.Forms.CheckBox chkShowImage;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.CheckBox chkSwapWO;
    public System.Windows.Forms.Button btnSave;
    public System.Windows.Forms.CheckBox chkConfirm;
    public System.Windows.Forms.Button btnSaveConfirm;
    public System.Windows.Forms.CheckBox chkPLNcomfirm;
    public System.Windows.Forms.Button btnPrint;
    public System.Windows.Forms.Button btnPrintPDF;
    public DaiCo.Shared.DaiCoComboBox cmbCustomer;
    public System.Windows.Forms.CheckBox chkDirect;
    public System.Windows.Forms.TextBox txtRemark;
    public System.Windows.Forms.Label label6;
    public DaiCo.Shared.DaiCoComboBox cmbDirectCustomer;
    public System.Windows.Forms.TextBox txtCustomerPoCancelNo;
    public System.Windows.Forms.Label label1;
    public System.Windows.Forms.Label label4;
    public System.Windows.Forms.Label label11;
    public System.Windows.Forms.TextBox txtRef;
    public System.Windows.Forms.TextBox txtContract;
    public System.Windows.Forms.Label label9;
    public System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox textBox4;
    private System.Windows.Forms.TextBox textBox1;
    public DaiCo.Shared.UserControls.uc_DateTimePicker dtp_date;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultContainerEffect;
  }
}
