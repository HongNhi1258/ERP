namespace DaiCo.ERPProject
{
  partial class viewPLN_02_026
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
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.chkLock = new System.Windows.Forms.CheckBox();
      this.chkConfirm = new System.Windows.Forms.CheckBox();
      this.chkExpand = new System.Windows.Forms.CheckBox();
      this.ultraDDPartType = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.ultDDSupp = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.ultDDLocation = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraDDPartType)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDSupp)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDLocation)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(866, 537);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 3;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 508);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(866, 29);
      this.tableLayoutPanel4.TabIndex = 2;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(707, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 47;
      this.btnSave.Text = "  Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(788, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 50;
      this.btnClose.Text = "  Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.chkLock);
      this.groupBox2.Controls.Add(this.chkConfirm);
      this.groupBox2.Controls.Add(this.chkExpand);
      this.groupBox2.Controls.Add(this.ultraDDPartType);
      this.groupBox2.Controls.Add(this.ultDDSupp);
      this.groupBox2.Controls.Add(this.ultDDLocation);
      this.groupBox2.Controls.Add(this.ultData);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(860, 502);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Information";
      // 
      // chkLock
      // 
      this.chkLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.chkLock.AutoSize = true;
      this.chkLock.Location = new System.Drawing.Point(640, -1);
      this.chkLock.Name = "chkLock";
      this.chkLock.Size = new System.Drawing.Size(82, 17);
      this.chkLock.TabIndex = 4;
      this.chkLock.Text = "PLN Lock";
      this.chkLock.UseVisualStyleBackColor = true;
      this.chkLock.CheckedChanged += new System.EventHandler(this.chkLock_CheckedChanged);
      // 
      // chkConfirm
      // 
      this.chkConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.chkConfirm.AutoSize = true;
      this.chkConfirm.Location = new System.Drawing.Point(760, -1);
      this.chkConfirm.Name = "chkConfirm";
      this.chkConfirm.Size = new System.Drawing.Size(96, 17);
      this.chkConfirm.TabIndex = 4;
      this.chkConfirm.Text = "PLN Confirm";
      this.chkConfirm.UseVisualStyleBackColor = true;
      this.chkConfirm.CheckedChanged += new System.EventHandler(this.chkConfirm_CheckedChanged);
      // 
      // chkExpand
      // 
      this.chkExpand.AutoSize = true;
      this.chkExpand.Location = new System.Drawing.Point(103, -1);
      this.chkExpand.Name = "chkExpand";
      this.chkExpand.Size = new System.Drawing.Size(86, 17);
      this.chkExpand.TabIndex = 3;
      this.chkExpand.Text = "Expand All";
      this.chkExpand.UseVisualStyleBackColor = true;
      this.chkExpand.CheckedChanged += new System.EventHandler(this.chkExpand_CheckedChanged);
      // 
      // ultraDDPartType
      // 
      this.ultraDDPartType.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraDDPartType.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultraDDPartType.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraDDPartType.Location = new System.Drawing.Point(305, 346);
      this.ultraDDPartType.Name = "ultraDDPartType";
      this.ultraDDPartType.Size = new System.Drawing.Size(424, 80);
      this.ultraDDPartType.TabIndex = 2;
      this.ultraDDPartType.Visible = false;
      // 
      // ultDDSupp
      // 
      this.ultDDSupp.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDSupp.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultDDSupp.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultDDSupp.Location = new System.Drawing.Point(179, 83);
      this.ultDDSupp.Name = "ultDDSupp";
      this.ultDDSupp.Size = new System.Drawing.Size(424, 80);
      this.ultDDSupp.TabIndex = 1;
      this.ultDDSupp.Visible = false;
      // 
      // ultDDLocation
      // 
      this.ultDDLocation.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDLocation.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultDDLocation.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultDDLocation.Location = new System.Drawing.Point(192, 203);
      this.ultDDLocation.Name = "ultDDLocation";
      this.ultDDLocation.Size = new System.Drawing.Size(424, 80);
      this.ultDDLocation.TabIndex = 1;
      this.ultDDLocation.Visible = false;
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 16);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(854, 483);
      this.ultData.TabIndex = 0;
      this.ultData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_CellChange);
      this.ultData.BeforeCellActivate += new Infragistics.Win.UltraWinGrid.CancelableCellEventHandler(this.ultData_BeforeCellActivate);
      this.ultData.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultData_BeforeCellUpdate);
      // 
      // viewPLN_02_026
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_02_026";
      this.Size = new System.Drawing.Size(866, 537);
      this.Load += new System.EventHandler(this.viewPLN_02_026_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraDDPartType)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDSupp)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDLocation)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.GroupBox groupBox2;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDLocation;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultraDDPartType;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDSupp;
    private System.Windows.Forms.CheckBox chkExpand;
    private System.Windows.Forms.CheckBox chkLock;
    private System.Windows.Forms.CheckBox chkConfirm;
  }
}
