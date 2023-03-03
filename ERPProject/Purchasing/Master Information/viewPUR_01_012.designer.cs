namespace DaiCo.ERPProject
{
  partial class viewPUR_01_012
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
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label10 = new System.Windows.Forms.Label();
      this.txtMaterialCode = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.txtImport = new System.Windows.Forms.TextBox();
      this.btnImport = new System.Windows.Forms.Button();
      this.btnBrown = new System.Windows.Forms.Button();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.ultCBStatus = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultCBUsingBOM = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.btnPerPurchaser = new System.Windows.Forms.Button();
      this.btnGetTemplate = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBStatus)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBUsingBOM)).BeginInit();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(831, 576);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(80, 23);
      this.btnClose.TabIndex = 9;
      this.btnClose.Text = "   Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSearch.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Image = global::DaiCo.ERPProject.Properties.Resources.Search;
      this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSearch.Location = new System.Drawing.Point(712, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(82, 23);
      this.btnSearch.TabIndex = 2;
      this.btnSearch.Text = "   Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // label10
      // 
      this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(3, 7);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(120, 14);
      this.label10.TabIndex = 0;
      this.label10.Text = "Material Code/Name ";
      // 
      // txtMaterialCode
      // 
      this.txtMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialCode.Location = new System.Drawing.Point(134, 3);
      this.txtMaterialCode.Name = "txtMaterialCode";
      this.txtMaterialCode.Size = new System.Drawing.Size(526, 20);
      this.txtMaterialCode.TabIndex = 1;
      this.txtMaterialCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.btnSave, 2);
      this.btnSave.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(744, 576);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(80, 23);
      this.btnSave.TabIndex = 8;
      this.btnSave.Text = "   Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 6);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 91);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(908, 479);
      this.ultData.TabIndex = 7;
      this.ultData.AfterCellActivate += new System.EventHandler(this.ultData_AfterCellActivate);
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 6;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 88F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
      this.tableLayoutPanel1.Controls.Add(this.label10, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 5, 4);
      this.tableLayoutPanel1.Controls.Add(this.txtMaterialCode, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.txtImport, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnImport, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnSave, 3, 4);
      this.tableLayoutPanel1.Controls.Add(this.btnGetTemplate, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnBrown, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnExportExcel, 3, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnPerPurchaser, 0, 4);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(914, 602);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 37);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(44, 14);
      this.label1.TabIndex = 69;
      this.label1.Text = "Import";
      // 
      // txtImport
      // 
      this.txtImport.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtImport.Location = new System.Drawing.Point(134, 32);
      this.txtImport.Name = "txtImport";
      this.txtImport.ReadOnly = true;
      this.txtImport.Size = new System.Drawing.Size(526, 20);
      this.txtImport.TabIndex = 3;
      // 
      // btnImport
      // 
      this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnImport.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnImport.Image = global::DaiCo.ERPProject.Properties.Resources.Import;
      this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnImport.Location = new System.Drawing.Point(712, 32);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(82, 23);
      this.btnImport.TabIndex = 5;
      this.btnImport.Text = "   Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // btnBrown
      // 
      this.btnBrown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrown.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrown.Image = global::DaiCo.ERPProject.Properties.Resources.Browser;
      this.btnBrown.Location = new System.Drawing.Point(677, 32);
      this.btnBrown.Name = "btnBrown";
      this.btnBrown.Size = new System.Drawing.Size(29, 23);
      this.btnBrown.TabIndex = 4;
      this.btnBrown.UseVisualStyleBackColor = true;
      this.btnBrown.Click += new System.EventHandler(this.btnBrown_Click);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 4;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label3, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultCBStatus, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultCBUsingBOM, 3, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 59);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(663, 29);
      this.tableLayoutPanel2.TabIndex = 70;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(42, 14);
      this.label2.TabIndex = 0;
      this.label2.Text = "Status";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(354, 7);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(79, 14);
      this.label3.TabIndex = 1;
      this.label3.Text = "Using In BOM";
      // 
      // ultCBStatus
      // 
      this.ultCBStatus.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBStatus.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBStatus.Location = new System.Drawing.Point(134, 3);
      this.ultCBStatus.Name = "ultCBStatus";
      this.ultCBStatus.Size = new System.Drawing.Size(214, 23);
      this.ultCBStatus.TabIndex = 2;
      this.ultCBStatus.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // ultCBUsingBOM
      // 
      this.ultCBUsingBOM.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBUsingBOM.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBUsingBOM.Location = new System.Drawing.Point(445, 3);
      this.ultCBUsingBOM.Name = "ultCBUsingBOM";
      this.ultCBUsingBOM.Size = new System.Drawing.Size(215, 23);
      this.ultCBUsingBOM.TabIndex = 3;
      this.ultCBUsingBOM.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // btnExportExcel
      // 
      this.btnExportExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExportExcel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExportExcel.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnExportExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnExportExcel.Location = new System.Drawing.Point(712, 62);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(82, 23);
      this.btnExportExcel.TabIndex = 71;
      this.btnExportExcel.Text = "   Export";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // btnPerPurchaser
      // 
      this.btnPerPurchaser.Location = new System.Drawing.Point(3, 576);
      this.btnPerPurchaser.Name = "btnPerPurchaser";
      this.btnPerPurchaser.Size = new System.Drawing.Size(75, 23);
      this.btnPerPurchaser.TabIndex = 72;
      this.btnPerPurchaser.Text = "IsPurchaser";
      this.btnPerPurchaser.UseVisualStyleBackColor = true;
      this.btnPerPurchaser.Visible = false;
      // 
      // btnGetTemplate
      // 
      this.btnGetTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.btnGetTemplate, 2);
      this.btnGetTemplate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnGetTemplate.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnGetTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnGetTemplate.Location = new System.Drawing.Point(800, 32);
      this.btnGetTemplate.Name = "btnGetTemplate";
      this.btnGetTemplate.Size = new System.Drawing.Size(111, 23);
      this.btnGetTemplate.TabIndex = 6;
      this.btnGetTemplate.Text = "   Get Template";
      this.btnGetTemplate.UseVisualStyleBackColor = true;
      this.btnGetTemplate.Click += new System.EventHandler(this.btnGetTemplate_Click);
      // 
      // viewPUR_01_012
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPUR_01_012";
      this.Size = new System.Drawing.Size(914, 602);
      this.Load += new System.EventHandler(this.viewPUR_01_012_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBStatus)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBUsingBOM)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox txtMaterialCode;
    private System.Windows.Forms.Button btnSave;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtImport;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Button btnBrown;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBStatus;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBUsingBOM;
    private System.Windows.Forms.Button btnExportExcel;
    private System.Windows.Forms.Button btnPerPurchaser;
    private System.Windows.Forms.Button btnGetTemplate;
  }
}
