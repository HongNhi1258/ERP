namespace DaiCo.ERPProject
{
  partial class view_SearchInfo
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
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.lbCount = new System.Windows.Forms.Label();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.uegData = new Infragistics.Win.Misc.UltraExpandableGroupBox();
      this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
      this.ugdInformation = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.uegSearch = new Infragistics.Win.Misc.UltraExpandableGroupBox();
      this.ultraExpandableGroupBoxPanel2 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
      this.tlpSearch = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.ucbSearch = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tlpForm.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uegData)).BeginInit();
      this.uegData.SuspendLayout();
      this.ultraExpandableGroupBoxPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugdInformation)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.uegSearch)).BeginInit();
      this.uegSearch.SuspendLayout();
      this.ultraExpandableGroupBoxPanel2.SuspendLayout();
      this.tlpSearch.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbSearch)).BeginInit();
      this.SuspendLayout();
      // 
      // tlpForm
      // 
      this.tlpForm.ColumnCount = 1;
      this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.Controls.Add(this.tableLayoutPanel2, 0, 2);
      this.tlpForm.Controls.Add(this.uegData, 0, 1);
      this.tlpForm.Controls.Add(this.uegSearch, 0, 0);
      this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpForm.Location = new System.Drawing.Point(0, 0);
      this.tlpForm.Name = "tlpForm";
      this.tlpForm.RowCount = 3;
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpForm.Size = new System.Drawing.Size(597, 532);
      this.tlpForm.TabIndex = 0;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 5;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.lbCount, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnExportExcel, 3, 0);
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
      this.lbCount.Location = new System.Drawing.Point(192, 8);
      this.lbCount.Name = "lbCount";
      this.lbCount.Size = new System.Drawing.Size(44, 13);
      this.lbCount.TabIndex = 1;
      this.lbCount.Text = "Count:";
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
      this.btnExportExcel.TabIndex = 2;
      this.btnExportExcel.Text = "Export";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // uegData
      // 
      this.uegData.Controls.Add(this.ultraExpandableGroupBoxPanel1);
      this.uegData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uegData.ExpandedSize = new System.Drawing.Size(591, 347);
      this.uegData.Location = new System.Drawing.Point(3, 153);
      this.uegData.Name = "uegData";
      this.uegData.Size = new System.Drawing.Size(591, 347);
      this.uegData.TabIndex = 3;
      this.uegData.Text = "Data";
      // 
      // ultraExpandableGroupBoxPanel1
      // 
      this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ugdInformation);
      this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
      this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
      this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(585, 325);
      this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
      // 
      // ugdInformation
      // 
      this.ugdInformation.Cursor = System.Windows.Forms.Cursors.Default;
      this.ugdInformation.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ugdInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ugdInformation.Location = new System.Drawing.Point(0, 0);
      this.ugdInformation.Name = "ugdInformation";
      this.ugdInformation.Size = new System.Drawing.Size(585, 325);
      this.ugdInformation.TabIndex = 0;
      this.ugdInformation.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugdInformation_InitializeLayout);
      this.ugdInformation.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ugdInformation_MouseClick);
      // 
      // uegSearch
      // 
      this.uegSearch.Controls.Add(this.ultraExpandableGroupBoxPanel2);
      this.uegSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uegSearch.ExpandedSize = new System.Drawing.Size(591, 144);
      this.uegSearch.Location = new System.Drawing.Point(3, 3);
      this.uegSearch.Name = "uegSearch";
      this.uegSearch.Size = new System.Drawing.Size(591, 144);
      this.uegSearch.TabIndex = 4;
      this.uegSearch.Text = "Search";
      // 
      // ultraExpandableGroupBoxPanel2
      // 
      this.ultraExpandableGroupBoxPanel2.Controls.Add(this.tlpSearch);
      this.ultraExpandableGroupBoxPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraExpandableGroupBoxPanel2.Location = new System.Drawing.Point(3, 19);
      this.ultraExpandableGroupBoxPanel2.Name = "ultraExpandableGroupBoxPanel2";
      this.ultraExpandableGroupBoxPanel2.Size = new System.Drawing.Size(585, 122);
      this.ultraExpandableGroupBoxPanel2.TabIndex = 0;
      // 
      // tlpSearch
      // 
      this.tlpSearch.ColumnCount = 1;
      this.tlpSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpSearch.Controls.Add(this.tableLayoutPanel4, 0, 1);
      this.tlpSearch.Controls.Add(this.tableLayoutPanel5, 0, 0);
      this.tlpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpSearch.Location = new System.Drawing.Point(0, 0);
      this.tlpSearch.Margin = new System.Windows.Forms.Padding(0);
      this.tlpSearch.Name = "tlpSearch";
      this.tlpSearch.RowCount = 2;
      this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpSearch.Size = new System.Drawing.Size(585, 122);
      this.tlpSearch.TabIndex = 0;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 3;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.Controls.Add(this.btnSearch, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnClear, 1, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 93);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(585, 29);
      this.tableLayoutPanel4.TabIndex = 0;
      // 
      // btnSearch
      // 
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Image = global::DaiCo.ERPProject.Properties.Resources.Search;
      this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSearch.Location = new System.Drawing.Point(507, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 0;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnClear
      // 
      this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClear.Image = global::DaiCo.ERPProject.Properties.Resources.Clear;
      this.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClear.Location = new System.Drawing.Point(426, 3);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(75, 23);
      this.btnClear.TabIndex = 1;
      this.btnClear.Text = "Clear";
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 2;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel5.Controls.Add(this.ucbSearch, 0, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 2;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(585, 93);
      this.tableLayoutPanel5.TabIndex = 1;
      // 
      // ucbSearch
      // 
      this.ucbSearch.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucbSearch.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbSearch.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucbSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbSearch.Location = new System.Drawing.Point(3, 3);
      this.ucbSearch.Name = "ucbSearch";
      this.ucbSearch.Size = new System.Drawing.Size(286, 22);
      this.ucbSearch.TabIndex = 0;
      // 
      // view_SearchInfo
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tlpForm);
      this.Name = "view_SearchInfo";
      this.Size = new System.Drawing.Size(597, 532);
      this.Load += new System.EventHandler(this.view_SearchInfo_Load);
      this.tlpForm.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uegData)).EndInit();
      this.uegData.ResumeLayout(false);
      this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugdInformation)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.uegSearch)).EndInit();
      this.uegSearch.ResumeLayout(false);
      this.ultraExpandableGroupBoxPanel2.ResumeLayout(false);
      this.tlpSearch.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbSearch)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpForm;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ugdInformation;
    private System.Windows.Forms.TableLayoutPanel tlpSearch;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Label lbCount;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbSearch;
    private System.Windows.Forms.Button btnExportExcel;
    private Infragistics.Win.Misc.UltraExpandableGroupBox uegData;
    private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
    private Infragistics.Win.Misc.UltraExpandableGroupBox uegSearch;
    private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel2;
  }
}
