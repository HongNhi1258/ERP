namespace DaiCo.Purchasing
{
  partial class viewPUR_11_001
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
      this.grpSearch = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label8 = new System.Windows.Forms.Label();
      this.ultMaterialCode = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label26 = new System.Windows.Forms.Label();
      this.ultSupplier = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label1 = new System.Windows.Forms.Label();
      this.ultLeadTime = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.bnExport = new System.Windows.Forms.Button();
      this.grpSearch.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultMaterialCode)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultSupplier)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultLeadTime)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // grpSearch
      // 
      this.grpSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpSearch.Controls.Add(this.tableLayoutPanel1);
      this.grpSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpSearch.Location = new System.Drawing.Point(3, 3);
      this.grpSearch.Name = "grpSearch";
      this.grpSearch.Size = new System.Drawing.Size(658, 74);
      this.grpSearch.TabIndex = 1;
      this.grpSearch.TabStop = false;
      this.grpSearch.Text = "Search Information";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 8;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 99F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 11F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
      this.tableLayoutPanel1.Controls.Add(this.bnExport, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 7, 1);
      this.tableLayoutPanel1.Controls.Add(this.label8, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultMaterialCode, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label26, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultSupplier, 4, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 6, 0);
      this.tableLayoutPanel1.Controls.Add(this.ultLeadTime, 7, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(652, 55);
      this.tableLayoutPanel1.TabIndex = 14;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(583, 30);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(66, 21);
      this.btnSearch.TabIndex = 12;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(3, 7);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(85, 13);
      this.label8.TabIndex = 14;
      this.label8.Text = "Material Code";
      // 
      // ultMaterialCode
      // 
      this.ultMaterialCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultMaterialCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultMaterialCode.DisplayMember = "";
      this.ultMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultMaterialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultMaterialCode.Location = new System.Drawing.Point(102, 3);
      this.ultMaterialCode.Name = "ultMaterialCode";
      this.ultMaterialCode.Size = new System.Drawing.Size(120, 21);
      this.ultMaterialCode.TabIndex = 15;
      this.ultMaterialCode.ValueMember = "";
      // 
      // label26
      // 
      this.label26.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label26.AutoSize = true;
      this.label26.Location = new System.Drawing.Point(243, 7);
      this.label26.Name = "label26";
      this.label26.Size = new System.Drawing.Size(53, 13);
      this.label26.TabIndex = 35;
      this.label26.Text = "Supplier";
      // 
      // ultSupplier
      // 
      this.ultSupplier.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultSupplier.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.ultSupplier.DisplayMember = "";
      this.ultSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultSupplier.Location = new System.Drawing.Point(317, 3);
      this.ultSupplier.Name = "ultSupplier";
      this.ultSupplier.Size = new System.Drawing.Size(120, 21);
      this.ultSupplier.TabIndex = 36;
      this.ultSupplier.ValueMember = "";
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(454, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(66, 13);
      this.label1.TabIndex = 37;
      this.label1.Text = "Lead Time";
      // 
      // ultLeadTime
      // 
      this.ultLeadTime.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultLeadTime.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.ultLeadTime.DisplayMember = "";
      this.ultLeadTime.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultLeadTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultLeadTime.Location = new System.Drawing.Point(528, 3);
      this.ultLeadTime.Name = "ultLeadTime";
      this.ultLeadTime.Size = new System.Drawing.Size(121, 21);
      this.ultLeadTime.TabIndex = 38;
      this.ultLeadTime.ValueMember = "";
      // 
      // ultDetail
      // 
      this.ultDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ultDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDetail.Location = new System.Drawing.Point(3, 84);
      this.ultDetail.Name = "ultDetail";
      this.ultDetail.Size = new System.Drawing.Size(658, 417);
      this.ultDetail.TabIndex = 2;
      this.ultDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultDetail_InitializeLayout);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.grpSearch, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultDetail, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(664, 504);
      this.tableLayoutPanel2.TabIndex = 3;
      // 
      // bnExport
      // 
      this.bnExport.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.bnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.bnExport.Location = new System.Drawing.Point(30, 30);
      this.bnExport.Name = "bnExport";
      this.bnExport.Size = new System.Drawing.Size(66, 21);
      this.bnExport.TabIndex = 39;
      this.bnExport.Text = "Export";
      this.bnExport.UseVisualStyleBackColor = true;
      this.bnExport.Click += new System.EventHandler(this.bnExport_Click);
      // 
      // viewPUR_10_005
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel2);
      this.Name = "viewPUR_11_001";
      this.Size = new System.Drawing.Size(664, 504);
      this.Load += new System.EventHandler(this.viewPLN_06_006_Load);
      this.grpSearch.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultMaterialCode)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultSupplier)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultLeadTime)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox grpSearch;
    private System.Windows.Forms.Button btnSearch;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultDetail;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label8;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultMaterialCode;
    private System.Windows.Forms.Label label26;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultSupplier;
    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultLeadTime;
    private System.Windows.Forms.Button bnExport;
  }
}
