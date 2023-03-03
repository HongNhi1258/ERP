namespace DaiCo.Planning
{
  partial class viewPLN_06_012
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
      this.ultraCBReports = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label1 = new System.Windows.Forms.Label();
      this.btnPrint = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.lbContainer = new System.Windows.Forms.Label();
      this.ultCBContainer = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.grpItemsCustomerAllocation = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label5 = new System.Windows.Forms.Label();
      this.ultraCBKind = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultraCBItemCode = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultraCBCustomer = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label3 = new System.Windows.Forms.Label();
      this.chkPrintWithSAP = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBReports)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBContainer)).BeginInit();
      this.grpItemsCustomerAllocation.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBKind)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBItemCode)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBCustomer)).BeginInit();
      this.SuspendLayout();
      // 
      // ultraCBReports
      // 
      this.ultraCBReports.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.tableLayoutPanel1.SetColumnSpan(this.ultraCBReports, 2);
      this.ultraCBReports.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBReports.DisplayMember = "";
      this.ultraCBReports.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBReports.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
      this.ultraCBReports.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBReports.Location = new System.Drawing.Point(129, 3);
      this.ultraCBReports.Name = "ultraCBReports";
      this.ultraCBReports.Size = new System.Drawing.Size(659, 21);
      this.ultraCBReports.TabIndex = 33;
      this.ultraCBReports.ValueMember = "";
      this.ultraCBReports.ValueChanged += new System.EventHandler(this.ultraCBReports_ValueChanged);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(51, 13);
      this.label1.TabIndex = 32;
      this.label1.Text = "Reports";
      // 
      // btnPrint
      // 
      this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrint.Location = new System.Drawing.Point(645, 426);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(80, 23);
      this.btnPrint.TabIndex = 4;
      this.btnPrint.Text = "Print";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(731, 426);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(69, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(64, 13);
      this.label2.TabIndex = 34;
      this.label2.Text = "Item Code";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel1);
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(797, 87);
      this.groupBox1.TabIndex = 35;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Reports";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 126F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 114F));
      this.tableLayoutPanel1.Controls.Add(this.chkPrintWithSAP, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultraCBReports, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.lbContainer, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.ultCBContainer, 1, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(791, 68);
      this.tableLayoutPanel1.TabIndex = 36;
      // 
      // lbContainer
      // 
      this.lbContainer.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbContainer.AutoSize = true;
      this.lbContainer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbContainer.Location = new System.Drawing.Point(3, 37);
      this.lbContainer.Name = "lbContainer";
      this.lbContainer.Size = new System.Drawing.Size(81, 13);
      this.lbContainer.TabIndex = 34;
      this.lbContainer.Text = "Container No";
      // 
      // ultCBContainer
      // 
      this.ultCBContainer.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultCBContainer.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBContainer.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultCBContainer.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBContainer.DisplayMember = "";
      this.ultCBContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBContainer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBContainer.Location = new System.Drawing.Point(129, 32);
      this.ultCBContainer.Name = "ultCBContainer";
      this.ultCBContainer.Size = new System.Drawing.Size(545, 21);
      this.ultCBContainer.TabIndex = 35;
      this.ultCBContainer.ValueMember = "";
      // 
      // grpItemsCustomerAllocation
      // 
      this.grpItemsCustomerAllocation.Controls.Add(this.tableLayoutPanel2);
      this.grpItemsCustomerAllocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpItemsCustomerAllocation.Location = new System.Drawing.Point(3, 96);
      this.grpItemsCustomerAllocation.Name = "grpItemsCustomerAllocation";
      this.grpItemsCustomerAllocation.Size = new System.Drawing.Size(797, 100);
      this.grpItemsCustomerAllocation.TabIndex = 36;
      this.grpItemsCustomerAllocation.TabStop = false;
      this.grpItemsCustomerAllocation.Text = "Search Items/Customer Allocation Reports";
      this.grpItemsCustomerAllocation.Visible = false;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 127F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.label5, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.ultraCBKind, 1, 2);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultraCBItemCode, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultraCBCustomer, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 3;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(791, 81);
      this.tableLayoutPanel2.TabIndex = 37;
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(3, 60);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(32, 13);
      this.label5.TabIndex = 39;
      this.label5.Text = "Kind";
      // 
      // ultraCBKind
      // 
      this.ultraCBKind.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBKind.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.ultraCBKind.DisplayMember = "";
      this.ultraCBKind.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBKind.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBKind.Location = new System.Drawing.Point(130, 56);
      this.ultraCBKind.Name = "ultraCBKind";
      this.ultraCBKind.Size = new System.Drawing.Size(658, 21);
      this.ultraCBKind.TabIndex = 39;
      this.ultraCBKind.ValueMember = "";
      // 
      // ultraCBItemCode
      // 
      this.ultraCBItemCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBItemCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBItemCode.DisplayMember = "";
      this.ultraCBItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBItemCode.Location = new System.Drawing.Point(130, 3);
      this.ultraCBItemCode.Name = "ultraCBItemCode";
      this.ultraCBItemCode.Size = new System.Drawing.Size(658, 21);
      this.ultraCBItemCode.TabIndex = 35;
      this.ultraCBItemCode.ValueMember = "";
      // 
      // ultraCBCustomer
      // 
      this.ultraCBCustomer.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBCustomer.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.ultraCBCustomer.DisplayMember = "";
      this.ultraCBCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBCustomer.Location = new System.Drawing.Point(130, 30);
      this.ultraCBCustomer.Name = "ultraCBCustomer";
      this.ultraCBCustomer.Size = new System.Drawing.Size(658, 21);
      this.ultraCBCustomer.TabIndex = 37;
      this.ultraCBCustomer.ValueMember = "";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 33);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(59, 13);
      this.label3.TabIndex = 36;
      this.label3.Text = "Customer";
      // 
      // chkPrintWithSAP
      // 
      this.chkPrintWithSAP.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkPrintWithSAP.AutoSize = true;
      this.chkPrintWithSAP.Location = new System.Drawing.Point(680, 35);
      this.chkPrintWithSAP.Name = "chkPrintWithSAP";
      this.chkPrintWithSAP.Size = new System.Drawing.Size(107, 17);
      this.chkPrintWithSAP.TabIndex = 37;
      this.chkPrintWithSAP.Text = "Print with SAP";
      this.chkPrintWithSAP.UseVisualStyleBackColor = true;
      // 
      // viewPLN_06_012
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.grpItemsCustomerAllocation);
      this.Controls.Add(this.btnPrint);
      this.Controls.Add(this.groupBox1);
      this.Name = "viewPLN_06_012";
      this.Size = new System.Drawing.Size(803, 452);
      this.Load += new System.EventHandler(this.viewPLN_06_012_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBReports)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBContainer)).EndInit();
      this.grpItemsCustomerAllocation.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBKind)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBItemCode)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBCustomer)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBReports;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox grpItemsCustomerAllocation;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBItemCode;
    private System.Windows.Forms.Label label3;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBCustomer;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBKind;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label lbContainer;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBContainer;
    private System.Windows.Forms.CheckBox chkPrintWithSAP;
  }
}
