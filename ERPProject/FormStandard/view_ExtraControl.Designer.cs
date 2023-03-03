namespace DaiCo.ERPProject
{
  partial class view_ExtraControl
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
      this.btnAddItem = new System.Windows.Forms.Button();
      this.groupBoxMaster = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.label8 = new System.Windows.Forms.Label();
      this.txtImportExcelFile = new System.Windows.Forms.TextBox();
      this.btnGetTemplate = new System.Windows.Forms.Button();
      this.btnBrowseItem = new System.Windows.Forms.Button();
      this.btnImport = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.ugdInformation = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.lbCount = new System.Windows.Forms.Label();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.tlpForm.SuspendLayout();
      this.groupBoxMaster.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugdInformation)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tlpForm
      // 
      this.tlpForm.ColumnCount = 1;
      this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.Controls.Add(this.btnAddItem, 0, 1);
      this.tlpForm.Controls.Add(this.groupBoxMaster, 0, 0);
      this.tlpForm.Controls.Add(this.groupBox2, 0, 2);
      this.tlpForm.Controls.Add(this.tableLayoutPanel2, 0, 3);
      this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpForm.Location = new System.Drawing.Point(0, 0);
      this.tlpForm.Name = "tlpForm";
      this.tlpForm.RowCount = 4;
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tlpForm.Size = new System.Drawing.Size(597, 532);
      this.tlpForm.TabIndex = 0;
      // 
      // btnAddItem
      // 
      this.btnAddItem.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnAddItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAddItem.Image = global::DaiCo.ERPProject.Properties.Resources.Load;
      this.btnAddItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnAddItem.Location = new System.Drawing.Point(502, 63);
      this.btnAddItem.Name = "btnAddItem";
      this.btnAddItem.Size = new System.Drawing.Size(92, 23);
      this.btnAddItem.TabIndex = 4;
      this.btnAddItem.Text = "   Thêm SP";
      this.btnAddItem.UseVisualStyleBackColor = true;
      this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
      // 
      // groupBoxMaster
      // 
      this.groupBoxMaster.Controls.Add(this.tableLayoutPanel4);
      this.groupBoxMaster.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBoxMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBoxMaster.Location = new System.Drawing.Point(3, 3);
      this.groupBoxMaster.Name = "groupBoxMaster";
      this.groupBoxMaster.Size = new System.Drawing.Size(591, 54);
      this.groupBoxMaster.TabIndex = 3;
      this.groupBoxMaster.TabStop = false;
      this.groupBoxMaster.Text = "Import From Excel";
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
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(585, 35);
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
      this.txtImportExcelFile.Size = new System.Drawing.Size(271, 20);
      this.txtImportExcelFile.TabIndex = 2;
      // 
      // btnGetTemplate
      // 
      this.btnGetTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGetTemplate.AutoSize = true;
      this.btnGetTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnGetTemplate.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnGetTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnGetTemplate.Location = new System.Drawing.Point(497, 3);
      this.btnGetTemplate.Name = "btnGetTemplate";
      this.btnGetTemplate.Size = new System.Drawing.Size(85, 23);
      this.btnGetTemplate.TabIndex = 5;
      this.btnGetTemplate.Text = "Template";
      this.btnGetTemplate.UseVisualStyleBackColor = true;
      this.btnGetTemplate.Click += new System.EventHandler(this.btnGetTemplate_Click);
      // 
      // btnBrowseItem
      // 
      this.btnBrowseItem.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnBrowseItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrowseItem.Image = global::DaiCo.ERPProject.Properties.Resources.Browser;
      this.btnBrowseItem.Location = new System.Drawing.Point(360, 3);
      this.btnBrowseItem.Name = "btnBrowseItem";
      this.btnBrowseItem.Size = new System.Drawing.Size(50, 23);
      this.btnBrowseItem.TabIndex = 3;
      this.btnBrowseItem.UseVisualStyleBackColor = true;
      this.btnBrowseItem.Click += new System.EventHandler(this.btnBrowseItem_Click);
      // 
      // btnImport
      // 
      this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnImport.Image = global::DaiCo.ERPProject.Properties.Resources.Import;
      this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnImport.Location = new System.Drawing.Point(416, 3);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(75, 23);
      this.btnImport.TabIndex = 4;
      this.btnImport.Text = "Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ugdInformation);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 92);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(591, 408);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Information";
      // 
      // ugdInformation
      // 
      this.ugdInformation.Cursor = System.Windows.Forms.Cursors.Default;
      this.ugdInformation.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ugdInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ugdInformation.Location = new System.Drawing.Point(3, 16);
      this.ugdInformation.Name = "ugdInformation";
      this.ugdInformation.Size = new System.Drawing.Size(585, 389);
      this.ugdInformation.TabIndex = 0;
      this.ugdInformation.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ugdInformation_AfterCellUpdate);
      this.ugdInformation.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugdInformation_InitializeLayout);
      this.ugdInformation.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ugdInformation_BeforeCellUpdate);
      this.ugdInformation.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ugdInformation_BeforeRowsDeleted);
      this.ugdInformation.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ugdInformation_MouseClick);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 6;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 5, 0);
      this.tableLayoutPanel2.Controls.Add(this.lbCount, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnSave, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnExportExcel, 4, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 503);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(591, 29);
      this.tableLayoutPanel2.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(512, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 0;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // lbCount
      // 
      this.lbCount.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.lbCount.AutoSize = true;
      this.lbCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbCount.Location = new System.Drawing.Point(181, 8);
      this.lbCount.Name = "lbCount";
      this.lbCount.Size = new System.Drawing.Size(44, 13);
      this.lbCount.TabIndex = 1;
      this.lbCount.Text = "Count:";
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(350, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 2;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnExportExcel
      // 
      this.btnExportExcel.AutoSize = true;
      this.btnExportExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExportExcel.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnExportExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnExportExcel.Location = new System.Drawing.Point(431, 3);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(75, 23);
      this.btnExportExcel.TabIndex = 3;
      this.btnExportExcel.Text = "Export";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // view_ExtraControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tlpForm);
      this.Name = "view_ExtraControl";
      this.Size = new System.Drawing.Size(597, 532);
      this.Load += new System.EventHandler(this.view_ExtraControl_Load);
      this.tlpForm.ResumeLayout(false);
      this.groupBoxMaster.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugdInformation)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpForm;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ugdInformation;
    private System.Windows.Forms.Label lbCount;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.GroupBox groupBoxMaster;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox txtImportExcelFile;
    private System.Windows.Forms.Button btnGetTemplate;
    private System.Windows.Forms.Button btnBrowseItem;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Button btnExportExcel;
    private System.Windows.Forms.Button btnAddItem;
  }
}
