namespace DaiCo.Planning
{
  partial class viewPLN_07_009
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
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txtWO = new System.Windows.Forms.TextBox();
      this.cmbReport = new System.Windows.Forms.ComboBox();
      this.txtMaterial = new System.Windows.Forms.TextBox();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnExport = new System.Windows.Forms.Button();
      this.listLeft = new System.Windows.Forms.ListView();
      this.listRight = new System.Windows.Forms.ListView();
      this.btnAdd = new System.Windows.Forms.Button();
      this.btnAddAll = new System.Windows.Forms.Button();
      this.btnRemove = new System.Windows.Forms.Button();
      this.btnRemoveAll = new System.Windows.Forms.Button();
      this.chkShow = new System.Windows.Forms.CheckBox();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutMaterialGroup = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutMaterialGroup.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
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
      this.label2.Location = new System.Drawing.Point(3, 38);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(72, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Work Order";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 69);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(90, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Material Group";
      // 
      // txtWO
      // 
      this.txtWO.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtWO.Location = new System.Drawing.Point(103, 33);
      this.txtWO.Name = "txtWO";
      this.txtWO.Size = new System.Drawing.Size(599, 20);
      this.txtWO.TabIndex = 6;
      // 
      // cmbReport
      // 
      this.cmbReport.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cmbReport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cmbReport.FormattingEnabled = true;
      this.cmbReport.Items.AddRange(new object[] {
            "",
            "Close Work Order Material Information",
            "Supplement Material Information",
            "Allocate Material Information"});
      this.cmbReport.Location = new System.Drawing.Point(103, 3);
      this.cmbReport.Name = "cmbReport";
      this.cmbReport.Size = new System.Drawing.Size(599, 21);
      this.cmbReport.TabIndex = 5;
      this.cmbReport.SelectedIndexChanged += new System.EventHandler(this.cmbReport_SelectedIndexChanged);
      // 
      // txtMaterial
      // 
      this.txtMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterial.Enabled = false;
      this.txtMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMaterial.ForeColor = System.Drawing.SystemColors.InactiveBorder;
      this.txtMaterial.Location = new System.Drawing.Point(33, 3);
      this.txtMaterial.Name = "txtMaterial";
      this.txtMaterial.Size = new System.Drawing.Size(563, 20);
      this.txtMaterial.TabIndex = 4;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(627, 250);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 3;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(120, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnExport
      // 
      this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExport.Location = new System.Drawing.Point(6, 3);
      this.btnExport.Name = "btnExport";
      this.btnExport.Size = new System.Drawing.Size(108, 23);
      this.btnExport.TabIndex = 6;
      this.btnExport.Text = "Export To Excel";
      this.btnExport.UseVisualStyleBackColor = true;
      this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
      // 
      // listLeft
      // 
      this.listLeft.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listLeft.FullRowSelect = true;
      this.listLeft.Location = new System.Drawing.Point(3, 3);
      this.listLeft.Name = "listLeft";
      this.tableLayoutMaterialGroup.SetRowSpan(this.listLeft, 5);
      this.listLeft.Size = new System.Drawing.Size(249, 142);
      this.listLeft.TabIndex = 0;
      this.listLeft.UseCompatibleStateImageBehavior = false;
      this.listLeft.View = System.Windows.Forms.View.Details;
      this.listLeft.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listLeft_ColumnClick);
      // 
      // listRight
      // 
      this.listRight.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listRight.FullRowSelect = true;
      this.listRight.Location = new System.Drawing.Point(347, 3);
      this.listRight.Name = "listRight";
      this.tableLayoutMaterialGroup.SetRowSpan(this.listRight, 5);
      this.listRight.Size = new System.Drawing.Size(249, 142);
      this.listRight.TabIndex = 1;
      this.listRight.UseCompatibleStateImageBehavior = false;
      this.listRight.View = System.Windows.Forms.View.Details;
      this.listRight.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listRight_ColumnClick);
      // 
      // btnAdd
      // 
      this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAdd.Location = new System.Drawing.Point(262, 3);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 2;
      this.btnAdd.Text = ">";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // btnAddAll
      // 
      this.btnAddAll.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnAddAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAddAll.Location = new System.Drawing.Point(262, 33);
      this.btnAddAll.Name = "btnAddAll";
      this.btnAddAll.Size = new System.Drawing.Size(75, 23);
      this.btnAddAll.TabIndex = 3;
      this.btnAddAll.Text = ">>";
      this.btnAddAll.UseVisualStyleBackColor = true;
      this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
      // 
      // btnRemove
      // 
      this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnRemove.Location = new System.Drawing.Point(262, 91);
      this.btnRemove.Name = "btnRemove";
      this.btnRemove.Size = new System.Drawing.Size(75, 23);
      this.btnRemove.TabIndex = 4;
      this.btnRemove.Text = "<";
      this.btnRemove.UseVisualStyleBackColor = true;
      this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
      // 
      // btnRemoveAll
      // 
      this.btnRemoveAll.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnRemoveAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnRemoveAll.Location = new System.Drawing.Point(262, 121);
      this.btnRemoveAll.Name = "btnRemoveAll";
      this.btnRemoveAll.Size = new System.Drawing.Size(75, 23);
      this.btnRemoveAll.TabIndex = 5;
      this.btnRemoveAll.Text = "<<";
      this.btnRemoveAll.UseVisualStyleBackColor = true;
      this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
      // 
      // chkShow
      // 
      this.chkShow.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkShow.AutoSize = true;
      this.chkShow.Location = new System.Drawing.Point(3, 6);
      this.chkShow.Name = "chkShow";
      this.chkShow.Size = new System.Drawing.Size(15, 14);
      this.chkShow.TabIndex = 8;
      this.chkShow.UseVisualStyleBackColor = true;
      this.chkShow.CheckedChanged += new System.EventHandler(this.chkShow_CheckedChanged);
      // 
      // ultData
      // 
      this.ultData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 2);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.AutoFitColumns = true;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Location = new System.Drawing.Point(3, 279);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(699, 279);
      this.ultData.TabIndex = 4;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 1, 6);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutMaterialGroup, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 5);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.cmbReport, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtWO, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 7;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(705, 596);
      this.tableLayoutPanel1.TabIndex = 7;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel4.ColumnCount = 2;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.09091F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.90909F));
      this.tableLayoutPanel4.Controls.Add(this.btnClose, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnExport, 0, 0);
      this.tableLayoutPanel4.Location = new System.Drawing.Point(504, 564);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(198, 29);
      this.tableLayoutPanel4.TabIndex = 8;
      // 
      // tableLayoutMaterialGroup
      // 
      this.tableLayoutMaterialGroup.ColumnCount = 3;
      this.tableLayoutMaterialGroup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutMaterialGroup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 89F));
      this.tableLayoutMaterialGroup.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutMaterialGroup.Controls.Add(this.btnRemoveAll, 1, 4);
      this.tableLayoutMaterialGroup.Controls.Add(this.listLeft, 0, 0);
      this.tableLayoutMaterialGroup.Controls.Add(this.btnRemove, 1, 3);
      this.tableLayoutMaterialGroup.Controls.Add(this.listRight, 2, 0);
      this.tableLayoutMaterialGroup.Controls.Add(this.btnAddAll, 1, 1);
      this.tableLayoutMaterialGroup.Controls.Add(this.btnAdd, 1, 0);
      this.tableLayoutMaterialGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutMaterialGroup.Location = new System.Drawing.Point(103, 95);
      this.tableLayoutMaterialGroup.Name = "tableLayoutMaterialGroup";
      this.tableLayoutMaterialGroup.RowCount = 5;
      this.tableLayoutMaterialGroup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutMaterialGroup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutMaterialGroup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutMaterialGroup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutMaterialGroup.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutMaterialGroup.Size = new System.Drawing.Size(599, 148);
      this.tableLayoutMaterialGroup.TabIndex = 8;
      this.tableLayoutMaterialGroup.Visible = false;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.chkShow, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtMaterial, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(103, 63);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(599, 26);
      this.tableLayoutPanel2.TabIndex = 9;
      // 
      // viewPLN_07_009
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_07_009";
      this.Size = new System.Drawing.Size(705, 596);
      this.Load += new System.EventHandler(this.viewPLN_07_009_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutMaterialGroup.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtWO;
    private System.Windows.Forms.ComboBox cmbReport;
    private System.Windows.Forms.TextBox txtMaterial;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnExport;
    private System.Windows.Forms.ListView listRight;
    private System.Windows.Forms.ListView listLeft;
    private System.Windows.Forms.CheckBox chkShow;
    private System.Windows.Forms.Button btnRemoveAll;
    private System.Windows.Forms.Button btnRemove;
    private System.Windows.Forms.Button btnAddAll;
    private System.Windows.Forms.Button btnAdd;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutMaterialGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
  }
}
