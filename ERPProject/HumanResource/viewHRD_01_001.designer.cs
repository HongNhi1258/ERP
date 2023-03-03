namespace DaiCo.ERPProject
{
  partial class viewHRD_01_001
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
      this.gpbSearch = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClear = new System.Windows.Forms.Button();
      this.btnSearch = new System.Windows.Forms.Button();
      this.ucbBranch = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ucbDivision = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.btnClose = new System.Windows.Forms.Button();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.ucbManger = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.gpbSearch.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbBranch)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ucbDivision)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbManger)).BeginInit();
      this.SuspendLayout();
      // 
      // gpbSearch
      // 
      this.gpbSearch.Controls.Add(this.tableLayoutPanel1);
      this.gpbSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gpbSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gpbSearch.Location = new System.Drawing.Point(3, 3);
      this.gpbSearch.Name = "gpbSearch";
      this.gpbSearch.Size = new System.Drawing.Size(789, 90);
      this.gpbSearch.TabIndex = 0;
      this.gpbSearch.TabStop = false;
      this.gpbSearch.Text = "Search Information";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 5;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label9, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.ucbBranch, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.ucbDivision, 4, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(783, 71);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(47, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Branch";
      // 
      // label9
      // 
      this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(407, 8);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(52, 13);
      this.label9.TabIndex = 46;
      this.label9.Text = "Division";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel2.Controls.Add(this.btnClear, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnSearch, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(462, 29);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(321, 29);
      this.tableLayoutPanel2.TabIndex = 50;
      // 
      // btnClear
      // 
      this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClear.Location = new System.Drawing.Point(165, 3);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(72, 22);
      this.btnClear.TabIndex = 0;
      this.btnClear.Text = "Clear";
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(246, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(72, 22);
      this.btnSearch.TabIndex = 1;
      this.btnSearch.Text = "S&earch";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      this.btnSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // ucbBranch
      // 
      this.ucbBranch.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      this.ucbBranch.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
      this.ucbBranch.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbBranch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbBranch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucbBranch.Location = new System.Drawing.Point(56, 3);
      this.ucbBranch.Name = "ucbBranch";
      this.ucbBranch.Size = new System.Drawing.Size(315, 23);
      this.ucbBranch.TabIndex = 0;
      this.ucbBranch.ValueChanged += new System.EventHandler(this.ucbBranch_ValueChanged);
      // 
      // ucbDivision
      // 
      this.ucbDivision.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      this.ucbDivision.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
      this.ucbDivision.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbDivision.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbDivision.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucbDivision.Location = new System.Drawing.Point(465, 3);
      this.ucbDivision.Name = "ucbDivision";
      this.ucbDivision.Size = new System.Drawing.Size(315, 23);
      this.ucbDivision.TabIndex = 1;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(712, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(74, 23);
      this.btnClose.TabIndex = 0;
      this.btnClose.Text = "&Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 99);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(789, 322);
      this.ultData.TabIndex = 0;
      this.ultData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.AfterRowsDeleted += new System.EventHandler(this.ultData_AfterRowsDeleted);
      this.ultData.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultData_BeforeRowsDeleted);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.ultData, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.gpbSearch, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 2);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 3;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(795, 459);
      this.tableLayoutPanel3.TabIndex = 4;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 4;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 97F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel4.Controls.Add(this.btnClose, 3, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnExportExcel, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.ucbManger, 0, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 427);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(789, 29);
      this.tableLayoutPanel4.TabIndex = 2;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(632, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(74, 23);
      this.btnSave.TabIndex = 3;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnExportExcel
      // 
      this.btnExportExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExportExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExportExcel.Location = new System.Drawing.Point(534, 3);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(91, 23);
      this.btnExportExcel.TabIndex = 2;
      this.btnExportExcel.Text = "Export Excel";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // ucbManger
      // 
      this.ucbManger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ucbManger.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      this.ucbManger.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
      this.ucbManger.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbManger.Enabled = false;
      this.ucbManger.Location = new System.Drawing.Point(264, 3);
      this.ucbManger.Name = "ucbManger";
      this.ucbManger.Size = new System.Drawing.Size(264, 22);
      this.ucbManger.TabIndex = 52;
      this.ucbManger.Visible = false;
      // 
      // viewHRD_01_001
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel3);
      this.Name = "viewHRD_01_001";
      this.Size = new System.Drawing.Size(795, 459);
      this.Load += new System.EventHandler(this.viewHRD_01_001_Load);
      this.gpbSearch.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ucbBranch)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ucbDivision)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbManger)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox gpbSearch;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnExportExcel;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbBranch;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbDivision;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbManger;
  }
}
