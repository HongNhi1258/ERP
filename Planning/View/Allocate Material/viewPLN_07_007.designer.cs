namespace DaiCo.Planning
{
  partial class viewPLN_07_007
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
      this.label1 = new System.Windows.Forms.Label();
      this.btnClose = new System.Windows.Forms.Button();
      this.ultraComboWO = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.label2 = new System.Windows.Forms.Label();
      this.ultCBStatus = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraComboWO)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBStatus)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 5;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 4, 2);
      this.tableLayoutPanel1.Controls.Add(this.ultraComboWO, 4, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultCBStatus, 1, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(611, 503);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(318, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(28, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "WO";
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(530, 476);
      this.btnClose.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(81, 25);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // ultraComboWO
      // 
      this.ultraComboWO.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraComboWO.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraComboWO.DisplayLayout.AutoFitColumns = true;
      this.ultraComboWO.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraComboWO.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraComboWO.DisplayMember = "";
      this.ultraComboWO.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraComboWO.Location = new System.Drawing.Point(378, 3);
      this.ultraComboWO.Name = "ultraComboWO";
      this.ultraComboWO.Size = new System.Drawing.Size(230, 21);
      this.ultraComboWO.TabIndex = 0;
      this.ultraComboWO.ValueMember = "";
      this.ultraComboWO.ValueChanged += new System.EventHandler(this.ultraComboWO_ValueChanged);
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 5);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 30);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(605, 443);
      this.ultData.TabIndex = 1;
      this.ultData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultData_MouseDoubleClick);
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_CellChange);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(43, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Status";
      // 
      // ultCBStatus
      // 
      this.ultCBStatus.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultCBStatus.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBStatus.DisplayLayout.AutoFitColumns = true;
      this.ultCBStatus.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultCBStatus.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBStatus.DisplayMember = "";
      this.ultCBStatus.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBStatus.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
      this.ultCBStatus.Location = new System.Drawing.Point(63, 3);
      this.ultCBStatus.Name = "ultCBStatus";
      this.ultCBStatus.Size = new System.Drawing.Size(229, 21);
      this.ultCBStatus.TabIndex = 4;
      this.ultCBStatus.ValueMember = "";
      this.ultCBStatus.ValueChanged += new System.EventHandler(this.ultCBStatus_ValueChanged);
      // 
      // viewPLN_07_007
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_07_007";
      this.Size = new System.Drawing.Size(611, 503);
      this.Load += new System.EventHandler(this.viewPLN_07_007_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraComboWO)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBStatus)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraComboWO;
    private System.Windows.Forms.Label label2;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBStatus;

  }
}
