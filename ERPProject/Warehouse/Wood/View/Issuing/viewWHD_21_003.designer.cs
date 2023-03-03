namespace DaiCo.ERPProject
{
  partial class viewWHD_21_003
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
      this.ultReceipt = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.chkComfirm = new System.Windows.Forms.CheckBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnPrintPreview = new System.Windows.Forms.Button();
      this.btnPrint = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.ultSupplier = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.txtIssuingNote = new System.Windows.Forms.TextBox();
      this.txtTitle = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.txtDate = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtCreateBy = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txtRemark = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnLoad = new System.Windows.Forms.Button();
      this.btnAddMultiDetail = new System.Windows.Forms.Button();
      this.ultSummary = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultReceipt)).BeginInit();
      this.tableLayoutPanel4.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultSupplier)).BeginInit();
      this.tableLayoutPanel5.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultSummary)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.ultReceipt, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultSummary, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(958, 641);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // ultReceipt
      // 
      this.ultReceipt.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultReceipt.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultReceipt.Location = new System.Drawing.Point(3, 173);
      this.ultReceipt.Name = "ultReceipt";
      this.ultReceipt.Size = new System.Drawing.Size(952, 342);
      this.ultReceipt.TabIndex = 1;
      this.ultReceipt.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultReceipt_AfterCellUpdate);
      this.ultReceipt.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultReceipt_InitializeLayout);
      this.ultReceipt.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultReceipt_BeforeRowsDeleted);
      this.ultReceipt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ultReceipt_KeyUp);
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 5;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
      this.tableLayoutPanel4.Controls.Add(this.chkComfirm, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnClose, 4, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnPrintPreview, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnPrint, 3, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 608);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(952, 30);
      this.tableLayoutPanel4.TabIndex = 3;
      // 
      // chkComfirm
      // 
      this.chkComfirm.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.chkComfirm.AutoSize = true;
      this.chkComfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkComfirm.Location = new System.Drawing.Point(553, 6);
      this.chkComfirm.Name = "chkComfirm";
      this.chkComfirm.Size = new System.Drawing.Size(68, 17);
      this.chkComfirm.TabIndex = 0;
      this.chkComfirm.Text = "Confirm";
      this.chkComfirm.UseVisualStyleBackColor = true;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(628, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "  &Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(874, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "  &Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnPrintPreview
      // 
      this.btnPrintPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrintPreview.Enabled = false;
      this.btnPrintPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrintPreview.Image = global::DaiCo.ERPProject.Properties.Resources.Search;
      this.btnPrintPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnPrintPreview.Location = new System.Drawing.Point(710, 3);
      this.btnPrintPreview.Name = "btnPrintPreview";
      this.btnPrintPreview.Size = new System.Drawing.Size(75, 23);
      this.btnPrintPreview.TabIndex = 3;
      this.btnPrintPreview.Text = "  &Preview";
      this.btnPrintPreview.UseVisualStyleBackColor = true;
      this.btnPrintPreview.Click += new System.EventHandler(this.btnPrintPreview_Click);
      // 
      // btnPrint
      // 
      this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrint.Image = global::DaiCo.ERPProject.Properties.Resources.Print;
      this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnPrint.Location = new System.Drawing.Point(791, 3);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(75, 23);
      this.btnPrint.TabIndex = 4;
      this.btnPrint.Text = "  Print";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(952, 164);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Master Information";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 7;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.ultSupplier, 2, 2);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 2, 4);
      this.tableLayoutPanel2.Controls.Add(this.txtIssuingNote, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtTitle, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label6, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtDate, 6, 0);
      this.tableLayoutPanel2.Controls.Add(this.label3, 4, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtCreateBy, 6, 1);
      this.tableLayoutPanel2.Controls.Add(this.label4, 0, 3);
      this.tableLayoutPanel2.Controls.Add(this.txtRemark, 2, 3);
      this.tableLayoutPanel2.Controls.Add(this.label9, 0, 4);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label7, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.label11, 1, 2);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 6, 3);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 6;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(946, 145);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // ultSupplier
      // 
      this.ultSupplier.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.ultSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultSupplier.Location = new System.Drawing.Point(116, 55);
      this.ultSupplier.Name = "ultSupplier";
      this.ultSupplier.Size = new System.Drawing.Size(340, 21);
      this.ultSupplier.TabIndex = 4;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 6;
      this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel5, 5);
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Controls.Add(this.textBox1, 0, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(113, 108);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 1;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(833, 26);
      this.tableLayoutPanel5.TabIndex = 1;
      // 
      // textBox1
      // 
      this.textBox1.BackColor = System.Drawing.Color.Yellow;
      this.textBox1.Enabled = false;
      this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox1.ForeColor = System.Drawing.SystemColors.WindowText;
      this.textBox1.Location = new System.Drawing.Point(3, 3);
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new System.Drawing.Size(58, 20);
      this.textBox1.TabIndex = 385;
      this.textBox1.Text = "ID Wood";
      this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // txtIssuingNote
      // 
      this.txtIssuingNote.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtIssuingNote.Enabled = false;
      this.txtIssuingNote.Location = new System.Drawing.Point(116, 3);
      this.txtIssuingNote.Name = "txtIssuingNote";
      this.txtIssuingNote.ReadOnly = true;
      this.txtIssuingNote.Size = new System.Drawing.Size(340, 20);
      this.txtIssuingNote.TabIndex = 8;
      // 
      // txtTitle
      // 
      this.txtTitle.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtTitle.Location = new System.Drawing.Point(116, 29);
      this.txtTitle.Name = "txtTitle";
      this.txtTitle.Size = new System.Drawing.Size(340, 20);
      this.txtTitle.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(78, 13);
      this.label1.TabIndex = 15;
      this.label1.Text = "Issuing Note";
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(502, 6);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(34, 13);
      this.label6.TabIndex = 5;
      this.label6.Text = "Date";
      // 
      // txtDate
      // 
      this.txtDate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtDate.Enabled = false;
      this.txtDate.Location = new System.Drawing.Point(602, 3);
      this.txtDate.Name = "txtDate";
      this.txtDate.ReadOnly = true;
      this.txtDate.Size = new System.Drawing.Size(341, 20);
      this.txtDate.TabIndex = 13;
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(502, 32);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(62, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Create By";
      // 
      // txtCreateBy
      // 
      this.txtCreateBy.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtCreateBy.Enabled = false;
      this.txtCreateBy.Location = new System.Drawing.Point(602, 29);
      this.txtCreateBy.Name = "txtCreateBy";
      this.txtCreateBy.ReadOnly = true;
      this.txtCreateBy.Size = new System.Drawing.Size(341, 20);
      this.txtCreateBy.TabIndex = 10;
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(3, 87);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(50, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Remark";
      // 
      // txtRemark
      // 
      this.txtRemark.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtRemark.Location = new System.Drawing.Point(116, 83);
      this.txtRemark.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtRemark.Name = "txtRemark";
      this.txtRemark.Size = new System.Drawing.Size(340, 20);
      this.txtRemark.TabIndex = 4;
      // 
      // label9
      // 
      this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(3, 114);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(40, 13);
      this.label9.TabIndex = 385;
      this.label9.Text = "Errors";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(32, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Title";
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(3, 59);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(53, 13);
      this.label7.TabIndex = 6;
      this.label7.Text = "Supplier";
      // 
      // label11
      // 
      this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.ForeColor = System.Drawing.Color.Red;
      this.label11.Location = new System.Drawing.Point(87, 58);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(23, 15);
      this.label11.TabIndex = 1;
      this.label11.Text = "(*)";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
      this.tableLayoutPanel3.Controls.Add(this.btnAddMultiDetail, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnLoad, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(599, 79);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(347, 29);
      this.tableLayoutPanel3.TabIndex = 386;
      // 
      // btnLoad
      // 
      this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLoad.Image = global::DaiCo.ERPProject.Properties.Resources.Load;
      this.btnLoad.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnLoad.Location = new System.Drawing.Point(269, 3);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(75, 23);
      this.btnLoad.TabIndex = 5;
      this.btnLoad.Text = "&Load";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // btnAddMultiDetail
      // 
      this.btnAddMultiDetail.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnAddMultiDetail.Image = global::DaiCo.ERPProject.Properties.Resources.Import;
      this.btnAddMultiDetail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnAddMultiDetail.Location = new System.Drawing.Point(119, 3);
      this.btnAddMultiDetail.Name = "btnAddMultiDetail";
      this.btnAddMultiDetail.Size = new System.Drawing.Size(143, 23);
      this.btnAddMultiDetail.TabIndex = 6;
      this.btnAddMultiDetail.Text = "Add Multi Detail";
      this.btnAddMultiDetail.UseVisualStyleBackColor = true;
      this.btnAddMultiDetail.Click += new System.EventHandler(this.btnAddMultiDetail_Click);
      // 
      // ultSummary
      // 
      this.ultSummary.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultSummary.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultSummary.Location = new System.Drawing.Point(3, 521);
      this.ultSummary.Name = "ultSummary";
      this.ultSummary.Size = new System.Drawing.Size(952, 81);
      this.ultSummary.TabIndex = 4;
      this.ultSummary.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultSummary_InitializeLayout);
      // 
      // viewWHD_21_003
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewWHD_21_003";
      this.Size = new System.Drawing.Size(958, 641);
      this.Load += new System.EventHandler(this.viewVEN_02_002_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultReceipt)).EndInit();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultSupplier)).EndInit();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultSummary)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultReceipt;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.CheckBox chkComfirm;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtIssuingNote;
    private System.Windows.Forms.TextBox txtTitle;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtDate;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtRemark;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtCreateBy;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.Label label11;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultSupplier;
    private System.Windows.Forms.Button btnPrintPreview;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultSummary;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnAddMultiDetail;

  }
}
