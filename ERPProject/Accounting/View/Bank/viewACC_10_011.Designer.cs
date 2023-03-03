namespace DaiCo.ERPProject
{
  partial class viewACC_10_011
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
      this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.ugbInformation = new Infragistics.Win.Misc.UltraGroupBox();
      this.tlpMaster = new System.Windows.Forms.TableLayoutPanel();
      this.lbEffectDate = new System.Windows.Forms.Label();
      this.lbEndDate = new System.Windows.Forms.Label();
      this.lbCompanyBank = new System.Windows.Forms.Label();
      this.lbDescription = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.txtContractCode = new System.Windows.Forms.TextBox();
      this.txtContractDesc = new System.Windows.Forms.TextBox();
      this.ucbCompanyBank = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ucbCurrency = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.udtEffectDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.lbContractCode = new System.Windows.Forms.Label();
      this.lbRemainAmount = new System.Windows.Forms.Label();
      this.lbTotalAmount = new System.Windows.Forms.Label();
      this.lbCurrency = new System.Windows.Forms.Label();
      this.lbReceiptAmount = new System.Windows.Forms.Label();
      this.udtEndDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.label2 = new System.Windows.Forms.Label();
      this.uneTotalAmount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
      this.uneRemainAmount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
      this.uneReceiptAmount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
      this.label3 = new System.Windows.Forms.Label();
      this.tlpForm.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugbInformation)).BeginInit();
      this.ugbInformation.SuspendLayout();
      this.tlpMaster.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbCompanyBank)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ucbCurrency)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.udtEffectDate)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.udtEndDate)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.uneTotalAmount)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.uneRemainAmount)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.uneReceiptAmount)).BeginInit();
      this.SuspendLayout();
      // 
      // tlpForm
      // 
      this.tlpForm.ColumnCount = 1;
      this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.Controls.Add(this.tableLayoutPanel3, 0, 1);
      this.tlpForm.Controls.Add(this.ugbInformation, 0, 0);
      this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpForm.Location = new System.Drawing.Point(0, 0);
      this.tlpForm.Name = "tlpForm";
      this.tlpForm.RowCount = 2;
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpForm.Size = new System.Drawing.Size(758, 222);
      this.tlpForm.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 3;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 193);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(758, 29);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(680, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 4;
      this.btnClose.Text = "Đóng";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(599, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 2;
      this.btnSave.Text = "Lưu";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // ugbInformation
      // 
      this.ugbInformation.Controls.Add(this.tlpMaster);
      this.ugbInformation.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ugbInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ugbInformation.Location = new System.Drawing.Point(3, 3);
      this.ugbInformation.Name = "ugbInformation";
      this.ugbInformation.Size = new System.Drawing.Size(752, 187);
      this.ugbInformation.TabIndex = 0;
      this.ugbInformation.Text = "Thông tin";
      // 
      // tlpMaster
      // 
      this.tlpMaster.ColumnCount = 7;
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tlpMaster.Controls.Add(this.lbEffectDate, 4, 0);
      this.tlpMaster.Controls.Add(this.lbEndDate, 4, 1);
      this.tlpMaster.Controls.Add(this.lbCompanyBank, 0, 1);
      this.tlpMaster.Controls.Add(this.lbDescription, 0, 4);
      this.tlpMaster.Controls.Add(this.label1, 1, 1);
      this.tlpMaster.Controls.Add(this.txtContractCode, 2, 0);
      this.tlpMaster.Controls.Add(this.txtContractDesc, 2, 4);
      this.tlpMaster.Controls.Add(this.ucbCompanyBank, 2, 1);
      this.tlpMaster.Controls.Add(this.ucbCurrency, 6, 3);
      this.tlpMaster.Controls.Add(this.udtEffectDate, 6, 0);
      this.tlpMaster.Controls.Add(this.lbContractCode, 0, 0);
      this.tlpMaster.Controls.Add(this.lbRemainAmount, 0, 3);
      this.tlpMaster.Controls.Add(this.lbTotalAmount, 0, 2);
      this.tlpMaster.Controls.Add(this.lbCurrency, 4, 3);
      this.tlpMaster.Controls.Add(this.lbReceiptAmount, 4, 2);
      this.tlpMaster.Controls.Add(this.udtEndDate, 6, 1);
      this.tlpMaster.Controls.Add(this.label2, 1, 0);
      this.tlpMaster.Controls.Add(this.uneTotalAmount, 2, 2);
      this.tlpMaster.Controls.Add(this.uneRemainAmount, 2, 3);
      this.tlpMaster.Controls.Add(this.uneReceiptAmount, 6, 2);
      this.tlpMaster.Controls.Add(this.label3, 5, 3);
      this.tlpMaster.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpMaster.Location = new System.Drawing.Point(3, 16);
      this.tlpMaster.Name = "tlpMaster";
      this.tlpMaster.RowCount = 6;
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpMaster.Size = new System.Drawing.Size(746, 168);
      this.tlpMaster.TabIndex = 0;
      // 
      // lbEffectDate
      // 
      this.lbEffectDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbEffectDate.AutoSize = true;
      this.lbEffectDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbEffectDate.Location = new System.Drawing.Point(371, 8);
      this.lbEffectDate.Name = "lbEffectDate";
      this.lbEffectDate.Size = new System.Drawing.Size(94, 13);
      this.lbEffectDate.TabIndex = 2;
      this.lbEffectDate.Text = "Ngày hợp đồng";
      // 
      // lbEndDate
      // 
      this.lbEndDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbEndDate.AutoSize = true;
      this.lbEndDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbEndDate.Location = new System.Drawing.Point(371, 37);
      this.lbEndDate.Name = "lbEndDate";
      this.lbEndDate.Size = new System.Drawing.Size(145, 13);
      this.lbEndDate.TabIndex = 10;
      this.lbEndDate.Text = "Ngày kết thúc hợp đồng";
      // 
      // lbCompanyBank
      // 
      this.lbCompanyBank.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbCompanyBank.AutoSize = true;
      this.lbCompanyBank.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCompanyBank.Location = new System.Drawing.Point(3, 37);
      this.lbCompanyBank.Name = "lbCompanyBank";
      this.lbCompanyBank.Size = new System.Drawing.Size(87, 13);
      this.lbCompanyBank.TabIndex = 7;
      this.lbCompanyBank.Text = "TK ngân hàng";
      // 
      // lbDescription
      // 
      this.lbDescription.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbDescription.AutoSize = true;
      this.lbDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbDescription.Location = new System.Drawing.Point(3, 138);
      this.lbDescription.Name = "lbDescription";
      this.lbDescription.Size = new System.Drawing.Size(59, 13);
      this.lbDescription.TabIndex = 22;
      this.lbDescription.Text = "Diễn Giải";
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.Red;
      this.label1.Location = new System.Drawing.Point(133, 36);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(23, 15);
      this.label1.TabIndex = 8;
      this.label1.Text = "(*)";
      // 
      // txtContractCode
      // 
      this.txtContractCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtContractCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtContractCode.Location = new System.Drawing.Point(163, 4);
      this.txtContractCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtContractCode.Name = "txtContractCode";
      this.txtContractCode.Size = new System.Drawing.Size(182, 20);
      this.txtContractCode.TabIndex = 1;
      // 
      // txtContractDesc
      // 
      this.tlpMaster.SetColumnSpan(this.txtContractDesc, 5);
      this.txtContractDesc.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtContractDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtContractDesc.Location = new System.Drawing.Point(163, 119);
      this.txtContractDesc.Multiline = true;
      this.txtContractDesc.Name = "txtContractDesc";
      this.txtContractDesc.Size = new System.Drawing.Size(580, 52);
      this.txtContractDesc.TabIndex = 23;
      // 
      // ucbCompanyBank
      // 
      this.ucbCompanyBank.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucbCompanyBank.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbCompanyBank.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ucbCompanyBank.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucbCompanyBank.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbCompanyBank.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucbCompanyBank.Location = new System.Drawing.Point(163, 32);
      this.ucbCompanyBank.Name = "ucbCompanyBank";
      this.ucbCompanyBank.Size = new System.Drawing.Size(182, 23);
      this.ucbCompanyBank.TabIndex = 9;
      this.ucbCompanyBank.ValueChanged += new System.EventHandler(this.ucbCurrency_ValueChanged);
      // 
      // ucbCurrency
      // 
      this.ucbCurrency.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucbCurrency.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbCurrency.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ucbCurrency.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucbCurrency.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbCurrency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucbCurrency.Location = new System.Drawing.Point(561, 90);
      this.ucbCurrency.Name = "ucbCurrency";
      this.ucbCurrency.Size = new System.Drawing.Size(182, 23);
      this.ucbCurrency.TabIndex = 19;
      // 
      // udtEffectDate
      // 
      this.udtEffectDate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.udtEffectDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.udtEffectDate.Location = new System.Drawing.Point(561, 4);
      this.udtEffectDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.udtEffectDate.Name = "udtEffectDate";
      this.udtEffectDate.Size = new System.Drawing.Size(182, 21);
      this.udtEffectDate.TabIndex = 4;
      // 
      // lbContractCode
      // 
      this.lbContractCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbContractCode.AutoSize = true;
      this.lbContractCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbContractCode.Location = new System.Drawing.Point(3, 8);
      this.lbContractCode.Name = "lbContractCode";
      this.lbContractCode.Size = new System.Drawing.Size(80, 13);
      this.lbContractCode.TabIndex = 0;
      this.lbContractCode.Text = "Số hợp đồng";
      // 
      // lbRemainAmount
      // 
      this.lbRemainAmount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbRemainAmount.AutoSize = true;
      this.lbRemainAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbRemainAmount.Location = new System.Drawing.Point(3, 95);
      this.lbRemainAmount.Name = "lbRemainAmount";
      this.lbRemainAmount.Size = new System.Drawing.Size(64, 13);
      this.lbRemainAmount.TabIndex = 15;
      this.lbRemainAmount.Text = "Số còn lại";
      // 
      // lbTotalAmount
      // 
      this.lbTotalAmount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbTotalAmount.AutoSize = true;
      this.lbTotalAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbTotalAmount.Location = new System.Drawing.Point(3, 66);
      this.lbTotalAmount.Name = "lbTotalAmount";
      this.lbTotalAmount.Size = new System.Drawing.Size(112, 13);
      this.lbTotalAmount.TabIndex = 24;
      this.lbTotalAmount.Text = "Tổng hạn mức vay";
      // 
      // lbCurrency
      // 
      this.lbCurrency.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbCurrency.AutoSize = true;
      this.lbCurrency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCurrency.Location = new System.Drawing.Point(371, 95);
      this.lbCurrency.Name = "lbCurrency";
      this.lbCurrency.Size = new System.Drawing.Size(71, 13);
      this.lbCurrency.TabIndex = 18;
      this.lbCurrency.Text = "Loại tiền tệ";
      // 
      // lbReceiptAmount
      // 
      this.lbReceiptAmount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbReceiptAmount.AutoSize = true;
      this.lbReceiptAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbReceiptAmount.Location = new System.Drawing.Point(371, 66);
      this.lbReceiptAmount.Name = "lbReceiptAmount";
      this.lbReceiptAmount.Size = new System.Drawing.Size(72, 13);
      this.lbReceiptAmount.TabIndex = 25;
      this.lbReceiptAmount.Text = "Số nhận nợ";
      // 
      // udtEndDate
      // 
      this.udtEndDate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.udtEndDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.udtEndDate.Location = new System.Drawing.Point(561, 33);
      this.udtEndDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.udtEndDate.Name = "udtEndDate";
      this.udtEndDate.Size = new System.Drawing.Size(182, 21);
      this.udtEndDate.TabIndex = 27;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Red;
      this.label2.Location = new System.Drawing.Point(133, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(23, 15);
      this.label2.TabIndex = 28;
      this.label2.Text = "(*)";
      // 
      // uneTotalAmount
      // 
      this.uneTotalAmount.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uneTotalAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uneTotalAmount.FormatString = "##,##0.##";
      this.uneTotalAmount.Location = new System.Drawing.Point(163, 62);
      this.uneTotalAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.uneTotalAmount.Name = "uneTotalAmount";
      this.uneTotalAmount.Size = new System.Drawing.Size(182, 21);
      this.uneTotalAmount.TabIndex = 29;
      // 
      // uneRemainAmount
      // 
      this.uneRemainAmount.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uneRemainAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uneRemainAmount.FormatString = "##,##0.##";
      this.uneRemainAmount.Location = new System.Drawing.Point(163, 91);
      this.uneRemainAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.uneRemainAmount.Name = "uneRemainAmount";
      this.uneRemainAmount.ReadOnly = true;
      this.uneRemainAmount.Size = new System.Drawing.Size(182, 21);
      this.uneRemainAmount.TabIndex = 29;
      // 
      // uneReceiptAmount
      // 
      this.uneReceiptAmount.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uneReceiptAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uneReceiptAmount.FormatString = "##,##0.##";
      this.uneReceiptAmount.Location = new System.Drawing.Point(561, 62);
      this.uneReceiptAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.uneReceiptAmount.Name = "uneReceiptAmount";
      this.uneReceiptAmount.ReadOnly = true;
      this.uneReceiptAmount.Size = new System.Drawing.Size(182, 21);
      this.uneReceiptAmount.TabIndex = 29;
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.ForeColor = System.Drawing.Color.Red;
      this.label3.Location = new System.Drawing.Point(531, 94);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(23, 15);
      this.label3.TabIndex = 8;
      this.label3.Text = "(*)";
      // 
      // viewACC_10_011
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tlpForm);
      this.Name = "viewACC_10_011";
      this.Size = new System.Drawing.Size(758, 222);
      this.Load += new System.EventHandler(this.viewACC_10_011_Load);
      this.tlpForm.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugbInformation)).EndInit();
      this.ugbInformation.ResumeLayout(false);
      this.tlpMaster.ResumeLayout(false);
      this.tlpMaster.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbCompanyBank)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ucbCurrency)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.udtEffectDate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.udtEndDate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.uneTotalAmount)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.uneRemainAmount)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.uneReceiptAmount)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpForm;
    private System.Windows.Forms.TableLayoutPanel tlpMaster;
    private System.Windows.Forms.Label lbEffectDate;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbCompanyBank;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Label lbContractCode;
    private System.Windows.Forms.Label lbEndDate;
    private System.Windows.Forms.Label lbCompanyBank;
    private System.Windows.Forms.Label lbRemainAmount;
    private System.Windows.Forms.Label lbCurrency;
    private System.Windows.Forms.Label lbDescription;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtContractCode;
    private System.Windows.Forms.TextBox txtContractDesc;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbCurrency;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udtEffectDate;
    private Infragistics.Win.Misc.UltraGroupBox ugbInformation;
    private System.Windows.Forms.Label lbTotalAmount;
    private System.Windows.Forms.Label lbReceiptAmount;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udtEndDate;
    private System.Windows.Forms.Label label2;
    private Infragistics.Win.UltraWinEditors.UltraNumericEditor uneTotalAmount;
    private Infragistics.Win.UltraWinEditors.UltraNumericEditor uneRemainAmount;
    private Infragistics.Win.UltraWinEditors.UltraNumericEditor uneReceiptAmount;
    private System.Windows.Forms.Label label3;
  }
}
