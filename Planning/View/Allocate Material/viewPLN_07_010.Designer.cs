namespace DaiCo.Planning
{
  partial class viewPLN_07_010
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
      this.label3 = new System.Windows.Forms.Label();
      this.ucmbGroup = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.txtMaterialCode = new System.Windows.Forms.TextBox();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.ultddDept = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.chkAll = new System.Windows.Forms.CheckBox();
      this.udrpControlType = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucmbGroup)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultddDept)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.udrpControlType)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.ucmbGroup, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtMaterialCode, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 2, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnSave, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(847, 575);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(90, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Material Group";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 34);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(85, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Material Code";
      // 
      // ucmbGroup
      // 
      this.ucmbGroup.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ucmbGroup.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucmbGroup.DisplayLayout.AutoFitColumns = true;
      this.ucmbGroup.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ucmbGroup.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucmbGroup.DisplayMember = "";
      this.ucmbGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucmbGroup.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
      this.ucmbGroup.Location = new System.Drawing.Point(123, 3);
      this.ucmbGroup.Name = "ucmbGroup";
      this.ucmbGroup.Size = new System.Drawing.Size(636, 21);
      this.ucmbGroup.TabIndex = 3;
      this.ucmbGroup.ValueMember = "";
      // 
      // txtMaterialCode
      // 
      this.txtMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialCode.Location = new System.Drawing.Point(123, 30);
      this.txtMaterialCode.Name = "txtMaterialCode";
      this.txtMaterialCode.Size = new System.Drawing.Size(636, 20);
      this.txtMaterialCode.TabIndex = 5;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(767, 29);
      this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 6;
      this.btnSearch.Text = "S&earch";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(767, 549);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "&Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(684, 549);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 0;
      this.btnSave.Text = "&Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBox1
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
      this.groupBox1.Controls.Add(this.ultddDept);
      this.groupBox1.Controls.Add(this.chkAll);
      this.groupBox1.Controls.Add(this.udrpControlType);
      this.groupBox1.Controls.Add(this.ultData);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 57);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(841, 486);
      this.groupBox1.TabIndex = 10;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Information";
      // 
      // ultddDept
      // 
      this.ultddDept.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultddDept.DisplayLayout.AutoFitColumns = true;
      this.ultddDept.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultddDept.DisplayMember = "";
      this.ultddDept.Location = new System.Drawing.Point(267, 174);
      this.ultddDept.Name = "ultddDept";
      this.ultddDept.Size = new System.Drawing.Size(221, 35);
      this.ultddDept.TabIndex = 11;
      this.ultddDept.ValueMember = "";
      this.ultddDept.Visible = false;
      // 
      // chkAll
      // 
      this.chkAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.chkAll.AutoSize = true;
      this.chkAll.Location = new System.Drawing.Point(702, -1);
      this.chkAll.Name = "chkAll";
      this.chkAll.Size = new System.Drawing.Size(80, 17);
      this.chkAll.TabIndex = 10;
      this.chkAll.Text = "Select All";
      this.chkAll.UseVisualStyleBackColor = true;
      this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
      // 
      // udrpControlType
      // 
      this.udrpControlType.Cursor = System.Windows.Forms.Cursors.Default;
      this.udrpControlType.DisplayMember = "";
      this.udrpControlType.Location = new System.Drawing.Point(559, 174);
      this.udrpControlType.Name = "udrpControlType";
      this.udrpControlType.Size = new System.Drawing.Size(223, 35);
      this.udrpControlType.TabIndex = 9;
      this.udrpControlType.ValueMember = "";
      this.udrpControlType.Visible = false;
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.AutoFitColumns = true;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 16);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(835, 467);
      this.ultData.TabIndex = 8;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_CellChange);
      // 
      // viewPLN_07_010
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_07_010";
      this.Size = new System.Drawing.Size(847, 575);
      this.Load += new System.EventHandler(this.viewPLN_07_010_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucmbGroup)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultddDept)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.udrpControlType)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucmbGroup;
    private System.Windows.Forms.TextBox txtMaterialCode;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private Infragistics.Win.UltraWinGrid.UltraDropDown udrpControlType;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckBox chkAll;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultddDept;

  }
}
