namespace DaiCo.Planning
{
  partial class viewPLN_06_015
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
      this.label1 = new System.Windows.Forms.Label();
      this.txtContName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.cmbStatus = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.cmbCustomer = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.label4 = new System.Windows.Forms.Label();
      this.dtpShipDateFrom = new DaiCo.Shared.UserControls.uc_DateTimePicker();
      this.label5 = new System.Windows.Forms.Label();
      this.dtpShipDateTo = new DaiCo.Shared.UserControls.uc_DateTimePicker();
      this.grpSearchInfo = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.txtItemCode = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.txtSaleOrderCustomerPONo = new System.Windows.Forms.TextBox();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.ultDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnUnlock = new System.Windows.Forms.Button();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.grpSearchInfo.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(85, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Container List";
      // 
      // txtContName
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.txtContName, 2);
      this.txtContName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtContName.Location = new System.Drawing.Point(123, 3);
      this.txtContName.Name = "txtContName";
      this.txtContName.Size = new System.Drawing.Size(282, 20);
      this.txtContName.TabIndex = 0;
      this.txtContName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContName_KeyDown);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(411, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(43, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Status";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 61);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(59, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Customer";
      // 
      // cmbStatus
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.cmbStatus, 2);
      this.cmbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cmbStatus.FormattingEnabled = true;
      this.cmbStatus.Items.AddRange(new object[] {
            "",
            "New",
            "Planning Temporary Confirm",
            "WH Confirm",
            "Planning Confirm",
            "Issued For Container"});
      this.cmbStatus.Location = new System.Drawing.Point(481, 3);
      this.cmbStatus.Name = "cmbStatus";
      this.cmbStatus.Size = new System.Drawing.Size(230, 21);
      this.cmbStatus.TabIndex = 1;
      this.cmbStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbStatus_KeyDown);
      // 
      // cmbCustomer
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.cmbCustomer, 2);
      this.cmbCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cmbCustomer.FormattingEnabled = true;
      this.cmbCustomer.Location = new System.Drawing.Point(123, 57);
      this.cmbCustomer.Name = "cmbCustomer";
      this.cmbCustomer.Size = new System.Drawing.Size(282, 21);
      this.cmbCustomer.TabIndex = 4;
      this.cmbCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbCustomer_KeyDown);
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(3, 34);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(65, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Date From";
      // 
      // dtpShipDateFrom
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.dtpShipDateFrom, 2);
      this.dtpShipDateFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dtpShipDateFrom.Location = new System.Drawing.Point(123, 30);
      this.dtpShipDateFrom.Name = "dtpShipDateFrom";
      this.dtpShipDateFrom.Size = new System.Drawing.Size(282, 21);
      this.dtpShipDateFrom.TabIndex = 2;
      this.dtpShipDateFrom.Value = new System.DateTime(((long)(0)));
      this.dtpShipDateFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpShipDateFrom_KeyDown);
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(411, 34);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(22, 13);
      this.label5.TabIndex = 8;
      this.label5.Text = "To";
      // 
      // dtpShipDateTo
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.dtpShipDateTo, 2);
      this.dtpShipDateTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dtpShipDateTo.Location = new System.Drawing.Point(481, 30);
      this.dtpShipDateTo.Name = "dtpShipDateTo";
      this.dtpShipDateTo.Size = new System.Drawing.Size(230, 21);
      this.dtpShipDateTo.TabIndex = 3;
      this.dtpShipDateTo.Value = new System.DateTime(((long)(0)));
      this.dtpShipDateTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtpShipDateTo_KeyDown);
      // 
      // grpSearchInfo
      // 
      this.grpSearchInfo.Controls.Add(this.tableLayoutPanel1);
      this.grpSearchInfo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpSearchInfo.Location = new System.Drawing.Point(3, 3);
      this.grpSearchInfo.Name = "grpSearchInfo";
      this.grpSearchInfo.Size = new System.Drawing.Size(720, 129);
      this.grpSearchInfo.TabIndex = 0;
      this.grpSearchInfo.TabStop = false;
      this.grpSearchInfo.Text = "Search Information";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 6;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.dtpShipDateTo, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.label5, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.cmbStatus, 4, 0);
      this.tableLayoutPanel1.Controls.Add(this.label4, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 5, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnClear, 4, 3);
      this.tableLayoutPanel1.Controls.Add(this.label6, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.txtItemCode, 4, 2);
      this.tableLayoutPanel1.Controls.Add(this.label7, 3, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtContName, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.dtpShipDateFrom, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.cmbCustomer, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtSaleOrderCustomerPONo, 2, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(714, 110);
      this.tableLayoutPanel1.TabIndex = 15;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(636, 84);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 8;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnClear
      // 
      this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClear.Location = new System.Drawing.Point(552, 84);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(75, 23);
      this.btnClear.TabIndex = 7;
      this.btnClear.Text = "Clear";
      this.btnClear.UseVisualStyleBackColor = true;
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.tableLayoutPanel1.SetColumnSpan(this.label6, 2);
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(3, 89);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(170, 13);
      this.label6.TabIndex = 9;
      this.label6.Text = "Sale Order / Customer PONo";
      // 
      // txtItemCode
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.txtItemCode, 2);
      this.txtItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtItemCode.Location = new System.Drawing.Point(481, 57);
      this.txtItemCode.Name = "txtItemCode";
      this.txtItemCode.Size = new System.Drawing.Size(230, 20);
      this.txtItemCode.TabIndex = 5;
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(411, 61);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(64, 13);
      this.label7.TabIndex = 12;
      this.label7.Text = "Item Code";
      // 
      // txtSaleOrderCustomerPONo
      // 
      this.txtSaleOrderCustomerPONo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtSaleOrderCustomerPONo.Location = new System.Drawing.Point(183, 84);
      this.txtSaleOrderCustomerPONo.Name = "txtSaleOrderCustomerPONo";
      this.txtSaleOrderCustomerPONo.Size = new System.Drawing.Size(222, 20);
      this.txtSaleOrderCustomerPONo.TabIndex = 6;
      // 
      // btnDelete
      // 
      this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDelete.Location = new System.Drawing.Point(480, 3);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(75, 22);
      this.btnDelete.TabIndex = 1;
      this.btnDelete.Text = "Delete";
      this.btnDelete.UseVisualStyleBackColor = true;
      // 
      // btnNew
      // 
      this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.Location = new System.Drawing.Point(561, 3);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(75, 22);
      this.btnNew.TabIndex = 2;
      this.btnNew.Text = "New";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // ultDetail
      // 
      this.ultDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDetail.Location = new System.Drawing.Point(3, 138);
      this.ultDetail.Name = "ultDetail";
      this.ultDetail.Size = new System.Drawing.Size(720, 318);
      this.ultDetail.TabIndex = 11;
      this.ultDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultDetail_InitializeLayout);
      this.ultDetail.DoubleClick += new System.EventHandler(this.ultDetail_DoubleClick);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(642, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 22);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      // 
      // btnUnlock
      // 
      this.btnUnlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnUnlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnUnlock.Location = new System.Drawing.Point(399, 3);
      this.btnUnlock.Name = "btnUnlock";
      this.btnUnlock.Size = new System.Drawing.Size(75, 22);
      this.btnUnlock.TabIndex = 0;
      this.btnUnlock.Text = "Unlock";
      this.btnUnlock.UseVisualStyleBackColor = true;
      this.btnUnlock.Click += new System.EventHandler(this.btnUnlock_Click);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.grpSearchInfo, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultDetail, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 3;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(726, 489);
      this.tableLayoutPanel2.TabIndex = 13;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 4;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnUnlock, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnNew, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnDelete, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 460);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(720, 28);
      this.tableLayoutPanel3.TabIndex = 12;
      // 
      // viewPLN_06_015
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel2);
      this.Name = "viewPLN_06_015";
      this.Size = new System.Drawing.Size(726, 489);
      this.Load += new System.EventHandler(this.viewPLN_06_015_Load);
      this.grpSearchInfo.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtContName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private DaiCo.Shared.DaiCoComboBox cmbStatus;
    private DaiCo.Shared.DaiCoComboBox cmbCustomer;
    private System.Windows.Forms.Label label4;
    private DaiCo.Shared.UserControls.uc_DateTimePicker dtpShipDateFrom;
    private System.Windows.Forms.Label label5;
    private DaiCo.Shared.UserControls.uc_DateTimePicker dtpShipDateTo;
    private System.Windows.Forms.GroupBox grpSearchInfo;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Button btnSearch;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultDetail;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button btnUnlock;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtSaleOrderCustomerPONo;
    private System.Windows.Forms.TextBox txtItemCode;
    private System.Windows.Forms.Label label7;
  }
}
