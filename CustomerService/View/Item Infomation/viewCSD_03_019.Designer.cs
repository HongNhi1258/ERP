namespace DaiCo.CustomerService
{
  partial class viewCSD_03_019
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txtTransaction = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txtName = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.txtCreateDate = new System.Windows.Forms.TextBox();
      this.txtRemark = new System.Windows.Forms.TextBox();
      this.txtStatus = new System.Windows.Forms.TextBox();
      this.ultDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnExportToExcel = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.chkConfirm = new System.Windows.Forms.CheckBox();
      this.tableImportFromExcel = new System.Windows.Forms.TableLayoutPanel();
      this.txtExcelFilePath = new System.Windows.Forms.TextBox();
      this.btnBrowse = new System.Windows.Forms.Button();
      this.btnImportFromExcel = new System.Windows.Forms.Button();
      this.label7 = new System.Windows.Forms.Label();
      this.ultraCBCategory = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultddReason = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableImportFromExcel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBCategory)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultddReason)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(736, 130);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Master Information";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 5;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtTransaction, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label5, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.label6, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.txtCreateDate, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.txtRemark, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtStatus, 4, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(730, 111);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(107, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Transaction Code";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 66);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(50, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Remark";
      // 
      // txtTransaction
      // 
      this.txtTransaction.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtTransaction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtTransaction.Location = new System.Drawing.Point(123, 4);
      this.txtTransaction.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtTransaction.Name = "txtTransaction";
      this.txtTransaction.ReadOnly = true;
      this.txtTransaction.Size = new System.Drawing.Size(219, 20);
      this.txtTransaction.TabIndex = 13;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 37);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(62, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Create By";
      // 
      // txtName
      // 
      this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtName.Location = new System.Drawing.Point(123, 33);
      this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtName.Name = "txtName";
      this.txtName.ReadOnly = true;
      this.txtName.Size = new System.Drawing.Size(219, 20);
      this.txtName.TabIndex = 3;
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(388, 8);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(43, 13);
      this.label5.TabIndex = 14;
      this.label5.Text = "Status";
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(388, 37);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(75, 13);
      this.label6.TabIndex = 16;
      this.label6.Text = "Create Date";
      // 
      // txtCreateDate
      // 
      this.txtCreateDate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtCreateDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtCreateDate.Location = new System.Drawing.Point(508, 33);
      this.txtCreateDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtCreateDate.Name = "txtCreateDate";
      this.txtCreateDate.ReadOnly = true;
      this.txtCreateDate.Size = new System.Drawing.Size(219, 20);
      this.txtCreateDate.TabIndex = 17;
      // 
      // txtRemark
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.txtRemark, 4);
      this.txtRemark.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtRemark.Location = new System.Drawing.Point(123, 62);
      this.txtRemark.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtRemark.Name = "txtRemark";
      this.txtRemark.Size = new System.Drawing.Size(604, 20);
      this.txtRemark.TabIndex = 18;
      // 
      // txtStatus
      // 
      this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtStatus.Location = new System.Drawing.Point(508, 4);
      this.txtStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtStatus.Name = "txtStatus";
      this.txtStatus.ReadOnly = true;
      this.txtStatus.Size = new System.Drawing.Size(219, 20);
      this.txtStatus.TabIndex = 19;
      // 
      // ultDetail
      // 
      this.ultDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDetail.Location = new System.Drawing.Point(3, 16);
      this.ultDetail.Name = "ultDetail";
      this.ultDetail.Size = new System.Drawing.Size(730, 380);
      this.ultDetail.TabIndex = 3;
      this.ultDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultDetail_InitializeLayout);
      this.ultDetail.AfterCellActivate += new System.EventHandler(this.ultDetail_AfterCellActivate);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(658, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(577, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnExportToExcel
      // 
      this.btnExportToExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExportToExcel.AutoSize = true;
      this.btnExportToExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExportToExcel.Location = new System.Drawing.Point(390, 3);
      this.btnExportToExcel.Name = "btnExportToExcel";
      this.btnExportToExcel.Size = new System.Drawing.Size(107, 23);
      this.btnExportToExcel.TabIndex = 0;
      this.btnExportToExcel.Text = "Export To Excel";
      this.btnExportToExcel.UseVisualStyleBackColor = true;
      this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultddReason);
      this.groupBox2.Controls.Add(this.ultDetail);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(0, 159);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(736, 399);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Detail Information";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.groupBox2, 0, 2);
      this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 3);
      this.tableLayoutPanel3.Controls.Add(this.tableImportFromExcel, 0, 1);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 4;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(736, 587);
      this.tableLayoutPanel3.TabIndex = 7;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 5;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.Controls.Add(this.btnClose, 4, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnExportToExcel, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 3, 0);
      this.tableLayoutPanel4.Controls.Add(this.chkConfirm, 2, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 558);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(736, 29);
      this.tableLayoutPanel4.TabIndex = 3;
      // 
      // chkConfirm
      // 
      this.chkConfirm.AutoSize = true;
      this.chkConfirm.Dock = System.Windows.Forms.DockStyle.Fill;
      this.chkConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkConfirm.Location = new System.Drawing.Point(503, 3);
      this.chkConfirm.Name = "chkConfirm";
      this.chkConfirm.Size = new System.Drawing.Size(68, 23);
      this.chkConfirm.TabIndex = 3;
      this.chkConfirm.Text = "Confirm";
      this.chkConfirm.UseVisualStyleBackColor = true;
      // 
      // tableImportFromExcel
      // 
      this.tableImportFromExcel.ColumnCount = 4;
      this.tableImportFromExcel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
      this.tableImportFromExcel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableImportFromExcel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableImportFromExcel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableImportFromExcel.Controls.Add(this.txtExcelFilePath, 1, 0);
      this.tableImportFromExcel.Controls.Add(this.btnBrowse, 2, 0);
      this.tableImportFromExcel.Controls.Add(this.btnImportFromExcel, 3, 0);
      this.tableImportFromExcel.Controls.Add(this.label7, 0, 0);
      this.tableImportFromExcel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableImportFromExcel.Location = new System.Drawing.Point(3, 130);
      this.tableImportFromExcel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.tableImportFromExcel.Name = "tableImportFromExcel";
      this.tableImportFromExcel.RowCount = 1;
      this.tableImportFromExcel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableImportFromExcel.Size = new System.Drawing.Size(730, 29);
      this.tableImportFromExcel.TabIndex = 1;
      // 
      // txtExcelFilePath
      // 
      this.txtExcelFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtExcelFilePath.Location = new System.Drawing.Point(78, 5);
      this.txtExcelFilePath.Margin = new System.Windows.Forms.Padding(3, 5, 3, 4);
      this.txtExcelFilePath.Name = "txtExcelFilePath";
      this.txtExcelFilePath.ReadOnly = true;
      this.txtExcelFilePath.Size = new System.Drawing.Size(487, 20);
      this.txtExcelFilePath.TabIndex = 1;
      // 
      // btnBrowse
      // 
      this.btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrowse.Location = new System.Drawing.Point(571, 3);
      this.btnBrowse.Name = "btnBrowse";
      this.btnBrowse.Size = new System.Drawing.Size(75, 23);
      this.btnBrowse.TabIndex = 2;
      this.btnBrowse.Text = "...";
      this.btnBrowse.UseVisualStyleBackColor = true;
      this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
      // 
      // btnImportFromExcel
      // 
      this.btnImportFromExcel.Enabled = false;
      this.btnImportFromExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnImportFromExcel.Location = new System.Drawing.Point(652, 3);
      this.btnImportFromExcel.Name = "btnImportFromExcel";
      this.btnImportFromExcel.Size = new System.Drawing.Size(75, 23);
      this.btnImportFromExcel.TabIndex = 3;
      this.btnImportFromExcel.Text = "Import";
      this.btnImportFromExcel.UseVisualStyleBackColor = true;
      this.btnImportFromExcel.Click += new System.EventHandler(this.btnImportFromExcel_Click);
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(3, 8);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(57, 13);
      this.label7.TabIndex = 0;
      this.label7.Text = "File Path";
      // 
      // ultraCBCategory
      // 
      this.ultraCBCategory.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBCategory.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBCategory.DisplayLayout.AutoFitColumns = true;
      this.ultraCBCategory.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBCategory.DisplayMember = "";
      this.ultraCBCategory.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBCategory.Location = new System.Drawing.Point(489, 56);
      this.ultraCBCategory.Name = "ultraCBCategory";
      this.ultraCBCategory.Size = new System.Drawing.Size(238, 21);
      this.ultraCBCategory.TabIndex = 11;
      this.ultraCBCategory.ValueMember = "";
      // 
      // ultddReason
      // 
      this.ultddReason.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultddReason.DisplayLayout.AutoFitColumns = true;
      this.ultddReason.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultddReason.DisplayMember = "";
      this.ultddReason.Location = new System.Drawing.Point(9, 19);
      this.ultddReason.Name = "ultddReason";
      this.ultddReason.Size = new System.Drawing.Size(235, 35);
      this.ultddReason.TabIndex = 4;
      this.ultddReason.ValueMember = "";
      this.ultddReason.Visible = false;
      // 
      // viewCSD_03_019
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel3);
      this.Name = "viewCSD_03_019";
      this.Size = new System.Drawing.Size(736, 587);
      this.Load += new System.EventHandler(this.viewCSD_03_019_Load);
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.tableImportFromExcel.ResumeLayout(false);
      this.tableImportFromExcel.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBCategory)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultddReason)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultDetail;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnExportToExcel;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.TableLayoutPanel tableImportFromExcel;
    private System.Windows.Forms.TextBox txtExcelFilePath;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.Button btnImportFromExcel;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtTransaction;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtCreateDate;
    private System.Windows.Forms.TextBox txtRemark;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBCategory;
    private System.Windows.Forms.TextBox txtStatus;
    private System.Windows.Forms.CheckBox chkConfirm;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultddReason;
  }
}
