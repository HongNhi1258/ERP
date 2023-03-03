namespace DaiCo.ERPProject
{
  partial class viewPUR_03_004
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.label1 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.lblPRNO = new System.Windows.Forms.Label();
      this.ultLocalImport = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultGroupInCharge = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultProjectCode = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.drpRequestDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.ultUrgentLevel = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultLocalImport)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultGroupInCharge)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultProjectCode)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpRequestDate)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultUrgentLevel)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(46, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "PR NO";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.lblPRNO, 2, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(853, 370);
      this.tableLayoutPanel1.TabIndex = 421;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnNew, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(153, 338);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(697, 29);
      this.tableLayoutPanel2.TabIndex = 422;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnClose.Location = new System.Drawing.Point(600, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(94, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "   Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnNew
      // 
      this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
      this.btnNew.Image = global::DaiCo.ERPProject.Properties.Resources.New;
      this.btnNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNew.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnNew.Location = new System.Drawing.Point(500, 3);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(94, 23);
      this.btnNew.TabIndex = 0;
      this.btnNew.Text = "   New";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 3);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 30);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(847, 302);
      this.ultData.TabIndex = 0;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.DoubleClick += new System.EventHandler(this.ultData_DoubleClick);
      // 
      // lblPRNO
      // 
      this.lblPRNO.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblPRNO.AutoSize = true;
      this.lblPRNO.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblPRNO.ForeColor = System.Drawing.Color.Red;
      this.lblPRNO.Location = new System.Drawing.Point(153, 7);
      this.lblPRNO.Name = "lblPRNO";
      this.lblPRNO.Size = new System.Drawing.Size(0, 13);
      this.lblPRNO.TabIndex = 423;
      // 
      // ultLocalImport
      // 
      this.ultLocalImport.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultLocalImport.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultLocalImport.Location = new System.Drawing.Point(560, 111);
      this.ultLocalImport.Name = "ultLocalImport";
      this.ultLocalImport.Size = new System.Drawing.Size(222, 22);
      this.ultLocalImport.TabIndex = 7;
      // 
      // ultGroupInCharge
      // 
      this.ultGroupInCharge.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultGroupInCharge.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultGroupInCharge.Enabled = false;
      this.ultGroupInCharge.Location = new System.Drawing.Point(560, 192);
      this.ultGroupInCharge.Name = "ultGroupInCharge";
      this.ultGroupInCharge.Size = new System.Drawing.Size(222, 22);
      this.ultGroupInCharge.TabIndex = 12;
      // 
      // ultProjectCode
      // 
      this.ultProjectCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultProjectCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultProjectCode.Location = new System.Drawing.Point(153, 192);
      this.ultProjectCode.Name = "ultProjectCode";
      this.ultProjectCode.Size = new System.Drawing.Size(221, 22);
      this.ultProjectCode.TabIndex = 11;
      // 
      // drpRequestDate
      // 
      this.drpRequestDate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.drpRequestDate.Location = new System.Drawing.Point(557, 138);
      this.drpRequestDate.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
      this.drpRequestDate.Name = "drpRequestDate";
      this.drpRequestDate.Size = new System.Drawing.Size(225, 21);
      this.drpRequestDate.TabIndex = 9;
      // 
      // ultUrgentLevel
      // 
      this.ultUrgentLevel.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultUrgentLevel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultUrgentLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultUrgentLevel.Location = new System.Drawing.Point(153, 138);
      this.ultUrgentLevel.Name = "ultUrgentLevel";
      this.ultUrgentLevel.Size = new System.Drawing.Size(221, 22);
      this.ultUrgentLevel.TabIndex = 8;
      // 
      // viewPUR_03_004
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPUR_03_004";
      this.Size = new System.Drawing.Size(853, 370);
      this.Load += new System.EventHandler(this.viewPUR_03_004_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultLocalImport)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultGroupInCharge)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultProjectCode)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpRequestDate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultUrgentLevel)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultLocalImport;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultGroupInCharge;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultProjectCode;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor drpRequestDate;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultUrgentLevel;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Label lblPRNO;

  }
}
