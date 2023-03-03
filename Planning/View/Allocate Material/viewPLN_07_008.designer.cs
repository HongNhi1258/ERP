namespace DaiCo.Planning
{
  partial class viewPLN_07_008
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
      this.label1 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.ultraComboWO = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.txtMaterialGroup = new System.Windows.Forms.TextBox();
      this.chkSelectedAll = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnClosedMaterialCode = new System.Windows.Forms.Button();
      this.btnReOpenMaterialCode = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraComboWO)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(28, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "WO";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.ultraComboWO, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.txtMaterialGroup, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.chkSelectedAll, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 4);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(593, 671);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(90, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Material Group";
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
      this.ultraComboWO.Enabled = false;
      this.ultraComboWO.Location = new System.Drawing.Point(110, 3);
      this.ultraComboWO.Name = "ultraComboWO";
      this.ultraComboWO.Size = new System.Drawing.Size(480, 21);
      this.ultraComboWO.TabIndex = 0;
      this.ultraComboWO.ValueMember = "";
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 2);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 84);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(587, 557);
      this.ultData.TabIndex = 2;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // txtMaterialGroup
      // 
      this.txtMaterialGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialGroup.Enabled = false;
      this.txtMaterialGroup.Location = new System.Drawing.Point(110, 30);
      this.txtMaterialGroup.Name = "txtMaterialGroup";
      this.txtMaterialGroup.Size = new System.Drawing.Size(480, 20);
      this.txtMaterialGroup.TabIndex = 1;
      // 
      // chkSelectedAll
      // 
      this.chkSelectedAll.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.chkSelectedAll.AutoSize = true;
      this.chkSelectedAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkSelectedAll.Location = new System.Drawing.Point(496, 59);
      this.chkSelectedAll.Name = "chkSelectedAll";
      this.chkSelectedAll.Size = new System.Drawing.Size(94, 17);
      this.chkSelectedAll.TabIndex = 4;
      this.chkSelectedAll.Text = "Selected All";
      this.chkSelectedAll.UseVisualStyleBackColor = true;
      this.chkSelectedAll.CheckedChanged += new System.EventHandler(this.chkSelectedAll_CheckedChanged);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnClosedMaterialCode, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnReOpenMaterialCode, 0, 0);
      this.tableLayoutPanel2.Location = new System.Drawing.Point(283, 644);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(310, 27);
      this.tableLayoutPanel2.TabIndex = 5;
      // 
      // btnClose
      // 
      this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(251, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(56, 21);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnClosedMaterialCode
      // 
      this.btnClosedMaterialCode.AutoSize = true;
      this.btnClosedMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnClosedMaterialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClosedMaterialCode.Location = new System.Drawing.Point(127, 3);
      this.btnClosedMaterialCode.Name = "btnClosedMaterialCode";
      this.btnClosedMaterialCode.Size = new System.Drawing.Size(118, 21);
      this.btnClosedMaterialCode.TabIndex = 4;
      this.btnClosedMaterialCode.Text = "Close Material";
      this.btnClosedMaterialCode.UseVisualStyleBackColor = true;
      this.btnClosedMaterialCode.Click += new System.EventHandler(this.btnClosedMaterialCode_Click);
      // 
      // btnReOpenMaterialCode
      // 
      this.btnReOpenMaterialCode.AutoSize = true;
      this.btnReOpenMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnReOpenMaterialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnReOpenMaterialCode.Location = new System.Drawing.Point(3, 3);
      this.btnReOpenMaterialCode.Name = "btnReOpenMaterialCode";
      this.btnReOpenMaterialCode.Size = new System.Drawing.Size(118, 21);
      this.btnReOpenMaterialCode.TabIndex = 5;
      this.btnReOpenMaterialCode.Text = "Re-Open Material";
      this.btnReOpenMaterialCode.UseVisualStyleBackColor = true;
      this.btnReOpenMaterialCode.Click += new System.EventHandler(this.btnReOpenMaterialCode_Click);
      // 
      // viewPLN_07_008
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_07_008";
      this.Size = new System.Drawing.Size(593, 671);
      this.Load += new System.EventHandler(this.viewPLN_07_008_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraComboWO)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label2;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraComboWO;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TextBox txtMaterialGroup;
    private System.Windows.Forms.CheckBox chkSelectedAll;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClosedMaterialCode;
    private System.Windows.Forms.Button btnReOpenMaterialCode;
  }
}
