namespace DaiCo.ERPProject
{
  partial class viewPUR_02_003
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
      this.groupBoxExcel = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.label8 = new System.Windows.Forms.Label();
      this.txtImportExcelFile = new System.Windows.Forms.TextBox();
      this.btnGetTemplate = new System.Windows.Forms.Button();
      this.btnBrowseItem = new System.Windows.Forms.Button();
      this.btnImport = new System.Windows.Forms.Button();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBoxExcel.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBoxExcel, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1035, 516);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBoxExcel
      // 
      this.groupBoxExcel.Controls.Add(this.tableLayoutPanel4);
      this.groupBoxExcel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBoxExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBoxExcel.Location = new System.Drawing.Point(3, 3);
      this.groupBoxExcel.Name = "groupBoxExcel";
      this.groupBoxExcel.Size = new System.Drawing.Size(1029, 54);
      this.groupBoxExcel.TabIndex = 4;
      this.groupBoxExcel.TabStop = false;
      this.groupBoxExcel.Text = "Import From Excel";
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
      this.tableLayoutPanel4.Size = new System.Drawing.Size(1023, 35);
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
      this.txtImportExcelFile.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtImportExcelFile.Location = new System.Drawing.Point(83, 5);
      this.txtImportExcelFile.Margin = new System.Windows.Forms.Padding(3, 5, 3, 4);
      this.txtImportExcelFile.Name = "txtImportExcelFile";
      this.txtImportExcelFile.ReadOnly = true;
      this.txtImportExcelFile.Size = new System.Drawing.Size(689, 20);
      this.txtImportExcelFile.TabIndex = 2;
      // 
      // btnGetTemplate
      // 
      this.btnGetTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGetTemplate.AutoSize = true;
      this.btnGetTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnGetTemplate.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnGetTemplate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnGetTemplate.Location = new System.Drawing.Point(915, 3);
      this.btnGetTemplate.Name = "btnGetTemplate";
      this.btnGetTemplate.Size = new System.Drawing.Size(105, 23);
      this.btnGetTemplate.TabIndex = 5;
      this.btnGetTemplate.Text = "   Get Template";
      this.btnGetTemplate.UseVisualStyleBackColor = true;
      this.btnGetTemplate.Click += new System.EventHandler(this.btnGetTemplate_Click);
      // 
      // btnBrowseItem
      // 
      this.btnBrowseItem.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnBrowseItem.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrowseItem.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnBrowseItem.Location = new System.Drawing.Point(778, 3);
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
      this.btnImport.Location = new System.Drawing.Point(834, 3);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(75, 23);
      this.btnImport.TabIndex = 4;
      this.btnImport.Text = "   Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 3;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 487);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(1035, 29);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(964, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(68, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "   &Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(881, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 5;
      this.btnSave.Text = "   Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultData);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 63);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(1029, 421);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Detail";
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 16);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(1023, 402);
      this.ultData.TabIndex = 1;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultData_BeforeRowsDeleted);
      // 
      // viewPUR_02_003
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPUR_02_003";
      this.Size = new System.Drawing.Size(1035, 516);
      this.Load += new System.EventHandler(this.viewPUR_02_003_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBoxExcel.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBoxExcel;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox txtImportExcelFile;
    private System.Windows.Forms.Button btnGetTemplate;
    private System.Windows.Forms.Button btnBrowseItem;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Button btnSave;
  }
}
