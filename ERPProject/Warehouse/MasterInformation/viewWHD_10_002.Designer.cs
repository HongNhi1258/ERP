namespace DaiCo.ERPProject
{
  partial class viewWHD_10_002
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
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.ugrdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.lblCount = new System.Windows.Forms.Label();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.uegMain = new Infragistics.Win.Misc.UltraExpandableGroupBox();
      this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.ucbUserGroup = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label1 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugrdData)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uegMain)).BeginInit();
      this.uegMain.SuspendLayout();
      this.ultraExpandableGroupBoxPanel1.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbUserGroup)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.uegMain, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(597, 532);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ugrdData);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 64);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(591, 436);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Information";
      // 
      // ugrdData
      // 
      this.ugrdData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ugrdData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ugrdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ugrdData.Location = new System.Drawing.Point(3, 16);
      this.ugrdData.Name = "ugrdData";
      this.ugrdData.Size = new System.Drawing.Size(585, 417);
      this.ugrdData.TabIndex = 2;
      this.ugrdData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ugrdData_AfterCellUpdate);
      this.ugrdData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugrdData_InitializeLayout);
      this.ugrdData.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ugrdData_BeforeCellUpdate);
      this.ugrdData.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ugrdData_BeforeRowsDeleted);
      this.ugrdData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ugrdData_MouseClick);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 6;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 5, 0);
      this.tableLayoutPanel2.Controls.Add(this.lblCount, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnSave, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnExportExcel, 4, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 503);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(591, 29);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(513, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 4;
      this.btnClose.Text = "   Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // lblCount
      // 
      this.lblCount.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.lblCount.AutoSize = true;
      this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblCount.Location = new System.Drawing.Point(152, 8);
      this.lblCount.Name = "lblCount";
      this.lblCount.Size = new System.Drawing.Size(44, 13);
      this.lblCount.TabIndex = 0;
      this.lblCount.Text = "Count:";
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(351, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 3;
      this.btnSave.Text = "   Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnExportExcel
      // 
      this.btnExportExcel.AutoSize = true;
      this.btnExportExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExportExcel.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnExportExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnExportExcel.Location = new System.Drawing.Point(432, 3);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(75, 23);
      this.btnExportExcel.TabIndex = 0;
      this.btnExportExcel.Text = "    Export";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Visible = false;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // uegMain
      // 
      this.uegMain.Controls.Add(this.ultraExpandableGroupBoxPanel1);
      this.uegMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uegMain.ExpandedSize = new System.Drawing.Size(591, 55);
      this.uegMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uegMain.Location = new System.Drawing.Point(3, 3);
      this.uegMain.Name = "uegMain";
      this.uegMain.Size = new System.Drawing.Size(591, 55);
      this.uegMain.TabIndex = 0;
      this.uegMain.Text = "Main";
      // 
      // ultraExpandableGroupBoxPanel1
      // 
      this.ultraExpandableGroupBoxPanel1.Controls.Add(this.tableLayoutPanel3);
      this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
      this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
      this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(585, 33);
      this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 0, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(585, 33);
      this.tableLayoutPanel3.TabIndex = 0;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 2;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Controls.Add(this.ucbUserGroup, 1, 0);
      this.tableLayoutPanel5.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 2;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(585, 29);
      this.tableLayoutPanel5.TabIndex = 0;
      // 
      // ucbUserGroup
      // 
      this.ucbUserGroup.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucbUserGroup.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbUserGroup.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucbUserGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbUserGroup.Location = new System.Drawing.Point(113, 3);
      this.ucbUserGroup.Name = "ucbUserGroup";
      this.ucbUserGroup.Size = new System.Drawing.Size(469, 22);
      this.ucbUserGroup.TabIndex = 1;
      this.ucbUserGroup.ValueChanged += new System.EventHandler(this.ucbUserGroup_ValueChanged);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(71, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "User Group";
      // 
      // viewWHD_10_002
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewWHD_10_002";
      this.Size = new System.Drawing.Size(597, 532);
      this.Load += new System.EventHandler(this.viewWHD_10_002_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugrdData)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uegMain)).EndInit();
      this.uegMain.ResumeLayout(false);
      this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbUserGroup)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ugrdData;
    private System.Windows.Forms.Label lblCount;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnExportExcel;
    private Infragistics.Win.Misc.UltraExpandableGroupBox uegMain;
    private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbUserGroup;
    private System.Windows.Forms.Label label1;
  }
}
