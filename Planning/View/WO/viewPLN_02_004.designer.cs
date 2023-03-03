namespace DaiCo.Planning
{
  partial class viewPLN_02_004
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
      Infragistics.Win.UltraWinGrid.UltraGridLayout ultraGridLayout1 = new Infragistics.Win.UltraWinGrid.UltraGridLayout();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.ultGridCompWorkArea = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.ultdrDeadlineStatus = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.chkCompExpandAll = new System.Windows.Forms.CheckBox();
      this.btnFullFillQty = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.ultGridCarcassWorkArea = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.chkCarcassExpandAll = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.cmbWo = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.ultGridCompWorkArea)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultdrDeadlineStatus)).BeginInit();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultGridCarcassWorkArea)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.cmbWo)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(770, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(851, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(62, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "WO NO #";
      // 
      // ultGridCompWorkArea
      // 
      this.ultGridCompWorkArea.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultGridCompWorkArea.DisplayLayout.AutoFitColumns = true;
      this.ultGridCompWorkArea.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultGridCompWorkArea.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultGridCompWorkArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultGridCompWorkArea.Location = new System.Drawing.Point(3, 17);
      this.ultGridCompWorkArea.Name = "ultGridCompWorkArea";
      this.ultGridCompWorkArea.Size = new System.Drawing.Size(452, 557);
      this.ultGridCompWorkArea.TabIndex = 0;
      this.ultGridCompWorkArea.AfterRowsDeleted += new System.EventHandler(this.ultData_AfterRowsDeleted);
      this.ultGridCompWorkArea.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultGridCompWorkArea.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultData_BeforeRowsDeleted);
      this.ultGridCompWorkArea.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.ultdrDeadlineStatus);
      this.groupBox1.Controls.Add(this.ultGridCompWorkArea);
      this.groupBox1.Controls.Add(this.chkCompExpandAll);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 32);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(458, 577);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Component Work Area";
      // 
      // ultdrDeadlineStatus
      // 
      this.ultdrDeadlineStatus.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultdrDeadlineStatus.DisplayMember = "";
      this.ultdrDeadlineStatus.Location = new System.Drawing.Point(274, 461);
      this.ultdrDeadlineStatus.Name = "ultdrDeadlineStatus";
      this.ultdrDeadlineStatus.Size = new System.Drawing.Size(424, 80);
      this.ultdrDeadlineStatus.TabIndex = 2;
      this.ultdrDeadlineStatus.Text = "ultraDropDown1";
      this.ultdrDeadlineStatus.ValueMember = "";
      this.ultdrDeadlineStatus.Visible = false;
      // 
      // chkCompExpandAll
      // 
      this.chkCompExpandAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.chkCompExpandAll.AutoSize = true;
      this.chkCompExpandAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkCompExpandAll.Location = new System.Drawing.Point(324, 0);
      this.chkCompExpandAll.Name = "chkCompExpandAll";
      this.chkCompExpandAll.Size = new System.Drawing.Size(86, 17);
      this.chkCompExpandAll.TabIndex = 1;
      this.chkCompExpandAll.Text = "Expand All";
      this.chkCompExpandAll.UseVisualStyleBackColor = true;
      this.chkCompExpandAll.CheckedChanged += new System.EventHandler(this.chkCompExpandAll_CheckedChanged);
      // 
      // btnFullFillQty
      // 
      this.btnFullFillQty.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnFullFillQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnFullFillQty.Location = new System.Drawing.Point(351, 3);
      this.btnFullFillQty.Name = "btnFullFillQty";
      this.btnFullFillQty.Size = new System.Drawing.Size(110, 23);
      this.btnFullFillQty.TabIndex = 2;
      this.btnFullFillQty.Text = "Default Fill Qty";
      this.btnFullFillQty.UseVisualStyleBackColor = true;
      this.btnFullFillQty.Click += new System.EventHandler(this.btnFullFillQty_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultGridCarcassWorkArea);
      this.groupBox2.Controls.Add(this.chkCarcassExpandAll);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(467, 32);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(459, 577);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Carcass Work Area";
      // 
      // ultGridCarcassWorkArea
      // 
      this.ultGridCarcassWorkArea.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultGridCarcassWorkArea.DisplayLayout.AutoFitColumns = true;
      this.ultGridCarcassWorkArea.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultGridCarcassWorkArea.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultGridCarcassWorkArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultGridCarcassWorkArea.Location = new System.Drawing.Point(3, 17);
      this.ultGridCarcassWorkArea.Name = "ultGridCarcassWorkArea";
      this.ultGridCarcassWorkArea.Size = new System.Drawing.Size(453, 557);
      this.ultGridCarcassWorkArea.TabIndex = 0;
      this.ultGridCarcassWorkArea.AfterRowsDeleted += new System.EventHandler(this.ultGridCarcassWorkArea_AfterRowsDeleted);
      this.ultGridCarcassWorkArea.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultGridCarcassWorkArea_InitializeLayout);
      this.ultGridCarcassWorkArea.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultGridCarcassWorkArea_BeforeRowsDeleted);
      this.ultGridCarcassWorkArea.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultGridCarcassWorkArea_AfterCellUpdate);
      // 
      // chkCarcassExpandAll
      // 
      this.chkCarcassExpandAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.chkCarcassExpandAll.AutoSize = true;
      this.chkCarcassExpandAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkCarcassExpandAll.Location = new System.Drawing.Point(327, 0);
      this.chkCarcassExpandAll.Name = "chkCarcassExpandAll";
      this.chkCarcassExpandAll.Size = new System.Drawing.Size(86, 17);
      this.chkCarcassExpandAll.TabIndex = 1;
      this.chkCarcassExpandAll.Text = "Expand All";
      this.chkCarcassExpandAll.UseVisualStyleBackColor = true;
      this.chkCarcassExpandAll.CheckedChanged += new System.EventHandler(this.chkCarcassExpandAll_CheckedChanged);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(929, 641);
      this.tableLayoutPanel1.TabIndex = 12;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116F));
      this.tableLayoutPanel2.Controls.Add(this.btnFullFillQty, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.cmbWo, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(464, 29);
      this.tableLayoutPanel2.TabIndex = 2;
      // 
      // cmbWo
      // 
      this.cmbWo.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.cmbWo.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.cmbWo.DisplayMember = "";
      this.cmbWo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cmbWo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      ultraGridLayout1.AutoFitColumns = true;
      this.cmbWo.Layouts.Add(ultraGridLayout1);
      this.cmbWo.Location = new System.Drawing.Point(77, 2);
      this.cmbWo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.cmbWo.Name = "cmbWo";
      this.cmbWo.Size = new System.Drawing.Size(268, 23);
      this.cmbWo.TabIndex = 20;
      this.cmbWo.ValueMember = "";
      this.cmbWo.ValueChanged += new System.EventHandler(this.cmbWo_SelectedIndexChanged);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel3, 2);
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 0, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 612);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(929, 29);
      this.tableLayoutPanel3.TabIndex = 3;
      // 
      // viewPLN_02_004
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_02_004";
      this.Size = new System.Drawing.Size(929, 641);
      this.Load += new System.EventHandler(this.viewPLN_02_004_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultGridCompWorkArea)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultdrDeadlineStatus)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultGridCarcassWorkArea)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.cmbWo)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultGridCompWorkArea;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultGridCarcassWorkArea;
    private System.Windows.Forms.CheckBox chkCompExpandAll;
    private System.Windows.Forms.CheckBox chkCarcassExpandAll;
    private System.Windows.Forms.Button btnFullFillQty;
    private Infragistics.Win.UltraWinGrid.UltraCombo cmbWo;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultdrDeadlineStatus;
  }
}
