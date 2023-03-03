namespace DaiCo.CustomerService
{
  partial class viewCSD_03_006
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
      this.label1 = new System.Windows.Forms.Label();
      this.ucmbCustomer = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.label2 = new System.Windows.Forms.Label();
      this.txtItemCode = new System.Windows.Forms.TextBox();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.chkEnable = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnImport = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnFocus = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucmbCustomer)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ucmbCustomer, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.txtItemCode, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnRefresh, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.chkEnable, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnFocus, 3, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(572, 429);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(59, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Customer";
      // 
      // ucmbCustomer
      // 
      this.ucmbCustomer.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ucmbCustomer.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucmbCustomer.DisplayLayout.AutoFitColumns = true;
      this.ucmbCustomer.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ucmbCustomer.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucmbCustomer.DisplayMember = "";
      this.ucmbCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucmbCustomer.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
      this.ucmbCustomer.Location = new System.Drawing.Point(83, 4);
      this.ucmbCustomer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ucmbCustomer.Name = "ucmbCustomer";
      this.ucmbCustomer.Size = new System.Drawing.Size(331, 21);
      this.ucmbCustomer.TabIndex = 0;
      this.ucmbCustomer.ValueMember = "";
      this.ucmbCustomer.ValueChanged += new System.EventHandler(this.ucmbCustomer_ValueChanged);
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 4);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.AutoFitColumns = true;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 61);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(566, 336);
      this.ultData.TabIndex = 4;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      this.ultData.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_CellChange);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 37);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(64, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Item Code";
      // 
      // txtItemCode
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.txtItemCode, 2);
      this.txtItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtItemCode.Location = new System.Drawing.Point(83, 34);
      this.txtItemCode.Margin = new System.Windows.Forms.Padding(3, 5, 3, 4);
      this.txtItemCode.Name = "txtItemCode";
      this.txtItemCode.Size = new System.Drawing.Size(406, 20);
      this.txtItemCode.TabIndex = 1;
      this.txtItemCode.TextChanged += new System.EventHandler(this.txtItemCode_TextChanged);
      this.txtItemCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemCode_KeyDown);
      // 
      // btnRefresh
      // 
      this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnRefresh.Location = new System.Drawing.Point(495, 3);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(74, 23);
      this.btnRefresh.TabIndex = 8;
      this.btnRefresh.Text = "Refresh";
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
      // 
      // chkEnable
      // 
      this.chkEnable.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkEnable.AutoSize = true;
      this.chkEnable.Checked = true;
      this.chkEnable.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkEnable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkEnable.Location = new System.Drawing.Point(420, 7);
      this.chkEnable.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
      this.chkEnable.Name = "chkEnable";
      this.chkEnable.Size = new System.Drawing.Size(65, 17);
      this.chkEnable.TabIndex = 9;
      this.chkEnable.Text = "Enable";
      this.chkEnable.UseVisualStyleBackColor = true;
      this.chkEnable.CheckedChanged += new System.EventHandler(this.chkEnable_CheckedChanged);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 3);
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.Controls.Add(this.btnImport, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnSave, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel2.Location = new System.Drawing.Point(263, 400);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(309, 29);
      this.tableLayoutPanel2.TabIndex = 7;
      // 
      // btnImport
      // 
      this.btnImport.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnImport.Location = new System.Drawing.Point(3, 3);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(143, 23);
      this.btnImport.TabIndex = 1;
      this.btnImport.Text = "Import From Excel File";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Enabled = false;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(152, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(74, 23);
      this.btnSave.TabIndex = 5;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(232, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(74, 23);
      this.btnClose.TabIndex = 6;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnFocus
      // 
      this.btnFocus.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnFocus.Enabled = false;
      this.btnFocus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnFocus.Location = new System.Drawing.Point(495, 33);
      this.btnFocus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 2);
      this.btnFocus.Name = "btnFocus";
      this.btnFocus.Size = new System.Drawing.Size(74, 23);
      this.btnFocus.TabIndex = 2;
      this.btnFocus.Text = "Focus";
      this.btnFocus.UseVisualStyleBackColor = true;
      this.btnFocus.Click += new System.EventHandler(this.btnFocus_Click);
      // 
      // viewCSD_03_006
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewCSD_03_006";
      this.Size = new System.Drawing.Size(572, 429);
      this.Load += new System.EventHandler(this.viewCSD_03_006_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucmbCustomer)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucmbCustomer;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtItemCode;
    private System.Windows.Forms.Button btnFocus;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnImport;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.CheckBox chkEnable;
  }
}
