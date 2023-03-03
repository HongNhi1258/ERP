namespace DaiCo.ERPProject
{
  partial class viewPLN_22_001
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
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.ultCBReport = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.labMonth = new System.Windows.Forms.Label();
      this.labMonthFrom = new System.Windows.Forms.Label();
      this.txtMonth = new System.Windows.Forms.TextBox();
      this.txtMonthFrom = new System.Windows.Forms.TextBox();
      this.btnExport = new System.Windows.Forms.Button();
      this.labYear = new System.Windows.Forms.Label();
      this.txttYearTo = new System.Windows.Forms.TextBox();
      this.txtYear = new System.Windows.Forms.TextBox();
      this.labYearTo = new System.Windows.Forms.Label();
      this.labMaterialCode = new System.Windows.Forms.Label();
      this.txtMaterialCode = new System.Windows.Forms.TextBox();
      this.labMonthTo = new System.Windows.Forms.Label();
      this.labYearFrom = new System.Windows.Forms.Label();
      this.txtMonthTo = new System.Windows.Forms.TextBox();
      this.txtYearFrom = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.btnClose = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBReport)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(851, 400);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 69F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultCBReport, 2, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 1);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(845, 27);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(45, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Report";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Red;
      this.label2.Location = new System.Drawing.Point(72, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(20, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "(*)";
      // 
      // ultCBReport
      // 
      this.ultCBReport.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBReport.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBReport.Location = new System.Drawing.Point(103, 3);
      this.ultCBReport.Name = "ultCBReport";
      this.ultCBReport.Size = new System.Drawing.Size(739, 21);
      this.ultCBReport.TabIndex = 0;
      this.ultCBReport.ValueChanged += new System.EventHandler(this.ultCBReport_ValueChanged);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 9;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel3.Controls.Add(this.labMonth, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.labMonthFrom, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.txtMonth, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.txtMonthFrom, 1, 1);
      this.tableLayoutPanel3.Controls.Add(this.btnExport, 8, 2);
      this.tableLayoutPanel3.Controls.Add(this.labYear, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.txttYearTo, 8, 1);
      this.tableLayoutPanel3.Controls.Add(this.txtYear, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.labYearTo, 7, 1);
      this.tableLayoutPanel3.Controls.Add(this.labMaterialCode, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.txtMaterialCode, 6, 0);
      this.tableLayoutPanel3.Controls.Add(this.labMonthTo, 4, 1);
      this.tableLayoutPanel3.Controls.Add(this.labYearFrom, 2, 1);
      this.tableLayoutPanel3.Controls.Add(this.txtMonthTo, 6, 1);
      this.tableLayoutPanel3.Controls.Add(this.txtYearFrom, 3, 1);
      this.tableLayoutPanel3.Controls.Add(this.label3, 5, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 32);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 3;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(845, 336);
      this.tableLayoutPanel3.TabIndex = 1;
      // 
      // labMonth
      // 
      this.labMonth.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labMonth.AutoSize = true;
      this.labMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labMonth.Location = new System.Drawing.Point(3, 8);
      this.labMonth.Name = "labMonth";
      this.labMonth.Size = new System.Drawing.Size(42, 13);
      this.labMonth.TabIndex = 1;
      this.labMonth.Text = "Month";
      // 
      // labMonthFrom
      // 
      this.labMonthFrom.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labMonthFrom.AutoSize = true;
      this.labMonthFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labMonthFrom.Location = new System.Drawing.Point(3, 37);
      this.labMonthFrom.Name = "labMonthFrom";
      this.labMonthFrom.Size = new System.Drawing.Size(73, 13);
      this.labMonthFrom.TabIndex = 2;
      this.labMonthFrom.Text = "Month From";
      // 
      // txtMonth
      // 
      this.txtMonth.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMonth.Location = new System.Drawing.Point(103, 3);
      this.txtMonth.Name = "txtMonth";
      this.txtMonth.Size = new System.Drawing.Size(97, 20);
      this.txtMonth.TabIndex = 0;
      // 
      // txtMonthFrom
      // 
      this.txtMonthFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMonthFrom.Location = new System.Drawing.Point(103, 32);
      this.txtMonthFrom.Name = "txtMonthFrom";
      this.txtMonthFrom.Size = new System.Drawing.Size(97, 20);
      this.txtMonthFrom.TabIndex = 3;
      // 
      // btnExport
      // 
      this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExport.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnExport.Location = new System.Drawing.Point(770, 61);
      this.btnExport.Name = "btnExport";
      this.btnExport.Size = new System.Drawing.Size(72, 23);
      this.btnExport.TabIndex = 7;
      this.btnExport.Text = "   Export";
      this.btnExport.UseVisualStyleBackColor = true;
      this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
      // 
      // labYear
      // 
      this.labYear.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labYear.AutoSize = true;
      this.labYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labYear.Location = new System.Drawing.Point(206, 8);
      this.labYear.Name = "labYear";
      this.labYear.Size = new System.Drawing.Size(33, 13);
      this.labYear.TabIndex = 3;
      this.labYear.Text = "Year";
      // 
      // txttYearTo
      // 
      this.txttYearTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txttYearTo.Location = new System.Drawing.Point(742, 32);
      this.txttYearTo.Name = "txttYearTo";
      this.txttYearTo.Size = new System.Drawing.Size(100, 20);
      this.txttYearTo.TabIndex = 6;
      // 
      // txtYear
      // 
      this.txtYear.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtYear.Location = new System.Drawing.Point(306, 3);
      this.txtYear.Name = "txtYear";
      this.txtYear.Size = new System.Drawing.Size(97, 20);
      this.txtYear.TabIndex = 1;
      // 
      // labYearTo
      // 
      this.labYearTo.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labYearTo.AutoSize = true;
      this.labYearTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labYearTo.Location = new System.Drawing.Point(642, 37);
      this.labYearTo.Name = "labYearTo";
      this.labYearTo.Size = new System.Drawing.Size(52, 13);
      this.labYearTo.TabIndex = 12;
      this.labYearTo.Text = "Year To";
      // 
      // labMaterialCode
      // 
      this.labMaterialCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labMaterialCode.AutoSize = true;
      this.labMaterialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labMaterialCode.Location = new System.Drawing.Point(409, 8);
      this.labMaterialCode.Name = "labMaterialCode";
      this.labMaterialCode.Size = new System.Drawing.Size(85, 13);
      this.labMaterialCode.TabIndex = 13;
      this.labMaterialCode.Text = "Material Code";
      // 
      // txtMaterialCode
      // 
      this.tableLayoutPanel3.SetColumnSpan(this.txtMaterialCode, 3);
      this.txtMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialCode.Location = new System.Drawing.Point(539, 3);
      this.txtMaterialCode.Name = "txtMaterialCode";
      this.txtMaterialCode.Size = new System.Drawing.Size(303, 20);
      this.txtMaterialCode.TabIndex = 2;
      // 
      // labMonthTo
      // 
      this.labMonthTo.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labMonthTo.AutoSize = true;
      this.labMonthTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labMonthTo.Location = new System.Drawing.Point(409, 37);
      this.labMonthTo.Name = "labMonthTo";
      this.labMonthTo.Size = new System.Drawing.Size(61, 13);
      this.labMonthTo.TabIndex = 4;
      this.labMonthTo.Text = "Month To";
      // 
      // labYearFrom
      // 
      this.labYearFrom.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labYearFrom.AutoSize = true;
      this.labYearFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labYearFrom.Location = new System.Drawing.Point(206, 37);
      this.labYearFrom.Name = "labYearFrom";
      this.labYearFrom.Size = new System.Drawing.Size(64, 13);
      this.labYearFrom.TabIndex = 11;
      this.labYearFrom.Text = "Year From";
      // 
      // txtMonthTo
      // 
      this.txtMonthTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMonthTo.Location = new System.Drawing.Point(539, 32);
      this.txtMonthTo.Name = "txtMonthTo";
      this.txtMonthTo.Size = new System.Drawing.Size(97, 20);
      this.txtMonthTo.TabIndex = 5;
      // 
      // txtYearFrom
      // 
      this.txtYearFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtYearFrom.Location = new System.Drawing.Point(306, 32);
      this.txtYearFrom.Name = "txtYearFrom";
      this.txtYearFrom.Size = new System.Drawing.Size(97, 20);
      this.txtYearFrom.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.ForeColor = System.Drawing.Color.Red;
      this.label3.Location = new System.Drawing.Point(509, 8);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(20, 13);
      this.label3.TabIndex = 14;
      this.label3.Text = "(*)";
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(773, 374);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 0;
      this.btnClose.Text = "   Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // viewPLN_22_001
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_22_001";
      this.Size = new System.Drawing.Size(851, 400);
      this.Load += new System.EventHandler(this.viewPLN_22_001_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBReport)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnExport;
    private System.Windows.Forms.Label labMonth;
    private System.Windows.Forms.Label labMonthFrom;
    private System.Windows.Forms.Label labYear;
    private System.Windows.Forms.Label labMonthTo;
    private System.Windows.Forms.TextBox txtMonth;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBReport;
    private System.Windows.Forms.TextBox txtMonthFrom;
    private System.Windows.Forms.TextBox txtYear;
    private System.Windows.Forms.TextBox txtMonthTo;
    private System.Windows.Forms.TextBox txtYearFrom;
    private System.Windows.Forms.TextBox txttYearTo;
    private System.Windows.Forms.Label labYearFrom;
    private System.Windows.Forms.Label labYearTo;
    private System.Windows.Forms.Label labMaterialCode;
    private System.Windows.Forms.TextBox txtMaterialCode;
    private System.Windows.Forms.Label label3;
  }
}
