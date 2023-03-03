namespace DaiCo.CustomerService
{
  partial class viewCSD_01_016
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
      this.btnClose = new System.Windows.Forms.Button();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.label10 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.rdbtJCRU = new System.Windows.Forms.RadioButton();
      this.rdbtUK = new System.Windows.Forms.RadioButton();
      this.rdbtCn = new System.Windows.Forms.RadioButton();
      this.rdbtAll = new System.Windows.Forms.RadioButton();
      this.rdbtInt = new System.Windows.Forms.RadioButton();
      this.txtSaleCode = new System.Windows.Forms.TextBox();
      this.txtImport = new System.Windows.Forms.TextBox();
      this.btnBrown = new System.Windows.Forms.Button();
      this.btnImport = new System.Windows.Forms.Button();
      this.btnSearch = new System.Windows.Forms.Button();
      this.lblCount = new System.Windows.Forms.Label();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.btnGetTemplate = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.rdbtME = new System.Windows.Forms.RadioButton();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(836, 576);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 9;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 6);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 84);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(908, 486);
      this.ultData.TabIndex = 7;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 6;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.label10, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtSaleCode, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtImport, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnBrown, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnImport, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 5, 0);
      this.tableLayoutPanel1.Controls.Add(this.lblCount, 1, 4);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 5, 4);
      this.tableLayoutPanel1.Controls.Add(this.btnExportExcel, 3, 4);
      this.tableLayoutPanel1.Controls.Add(this.btnGetTemplate, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(914, 602);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // label10
      // 
      this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(3, 7);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(65, 13);
      this.label10.TabIndex = 0;
      this.label10.Text = "Sale Code";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 61);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(59, 13);
      this.label2.TabIndex = 69;
      this.label2.Text = "Customer";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 6;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.rdbtJCRU, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.rdbtUK, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.rdbtCn, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.rdbtAll, 5, 0);
      this.tableLayoutPanel2.Controls.Add(this.rdbtInt, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.rdbtME, 4, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(100, 54);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(601, 27);
      this.tableLayoutPanel2.TabIndex = 71;
      // 
      // rdbtJCRU
      // 
      this.rdbtJCRU.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.rdbtJCRU.AutoSize = true;
      this.rdbtJCRU.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.rdbtJCRU.Location = new System.Drawing.Point(61, 5);
      this.rdbtJCRU.Name = "rdbtJCRU";
      this.rdbtJCRU.Size = new System.Drawing.Size(53, 17);
      this.rdbtJCRU.TabIndex = 0;
      this.rdbtJCRU.TabStop = true;
      this.rdbtJCRU.Text = "JCRU";
      this.rdbtJCRU.UseVisualStyleBackColor = true;
      // 
      // rdbtUK
      // 
      this.rdbtUK.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.rdbtUK.AutoSize = true;
      this.rdbtUK.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.rdbtUK.Location = new System.Drawing.Point(3, 5);
      this.rdbtUK.Name = "rdbtUK";
      this.rdbtUK.Size = new System.Drawing.Size(52, 17);
      this.rdbtUK.TabIndex = 0;
      this.rdbtUK.TabStop = true;
      this.rdbtUK.Text = "JCUK";
      this.rdbtUK.UseVisualStyleBackColor = true;
      // 
      // rdbtCn
      // 
      this.rdbtCn.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.rdbtCn.AutoSize = true;
      this.rdbtCn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.rdbtCn.Location = new System.Drawing.Point(120, 5);
      this.rdbtCn.Name = "rdbtCn";
      this.rdbtCn.Size = new System.Drawing.Size(40, 17);
      this.rdbtCn.TabIndex = 0;
      this.rdbtCn.TabStop = true;
      this.rdbtCn.Text = "CN";
      this.rdbtCn.UseVisualStyleBackColor = true;
      // 
      // rdbtAll
      // 
      this.rdbtAll.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.rdbtAll.AutoSize = true;
      this.rdbtAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.rdbtAll.Checked = true;
      this.rdbtAll.Location = new System.Drawing.Point(562, 5);
      this.rdbtAll.Name = "rdbtAll";
      this.rdbtAll.Size = new System.Drawing.Size(36, 17);
      this.rdbtAll.TabIndex = 0;
      this.rdbtAll.TabStop = true;
      this.rdbtAll.Text = "All";
      this.rdbtAll.UseVisualStyleBackColor = true;
      // 
      // rdbtInt
      // 
      this.rdbtInt.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.rdbtInt.AutoSize = true;
      this.rdbtInt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.rdbtInt.Location = new System.Drawing.Point(166, 5);
      this.rdbtInt.Name = "rdbtInt";
      this.rdbtInt.Size = new System.Drawing.Size(43, 17);
      this.rdbtInt.TabIndex = 0;
      this.rdbtInt.TabStop = true;
      this.rdbtInt.Text = "INT";
      this.rdbtInt.UseVisualStyleBackColor = true;
      // 
      // txtSaleCode
      // 
      this.txtSaleCode.Dock = System.Windows.Forms.DockStyle.Top;
      this.txtSaleCode.Location = new System.Drawing.Point(103, 3);
      this.txtSaleCode.Name = "txtSaleCode";
      this.txtSaleCode.Size = new System.Drawing.Size(595, 20);
      this.txtSaleCode.TabIndex = 1;
      this.txtSaleCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // txtImport
      // 
      this.txtImport.Dock = System.Windows.Forms.DockStyle.Top;
      this.txtImport.Location = new System.Drawing.Point(103, 30);
      this.txtImport.Name = "txtImport";
      this.txtImport.ReadOnly = true;
      this.txtImport.Size = new System.Drawing.Size(595, 20);
      this.txtImport.TabIndex = 3;
      // 
      // btnBrown
      // 
      this.btnBrown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrown.Location = new System.Drawing.Point(704, 30);
      this.btnBrown.Name = "btnBrown";
      this.btnBrown.Size = new System.Drawing.Size(27, 21);
      this.btnBrown.TabIndex = 4;
      this.btnBrown.Text = "...";
      this.btnBrown.UseVisualStyleBackColor = true;
      this.btnBrown.Click += new System.EventHandler(this.btnBrown_Click);
      // 
      // btnImport
      // 
      this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnImport.Location = new System.Drawing.Point(737, 30);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(73, 21);
      this.btnImport.TabIndex = 5;
      this.btnImport.Text = "Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // btnSearch
      // 
      this.btnSearch.Dock = System.Windows.Forms.DockStyle.Top;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(836, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 21);
      this.btnSearch.TabIndex = 2;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // lblCount
      // 
      this.lblCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblCount.AutoSize = true;
      this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblCount.Location = new System.Drawing.Point(103, 581);
      this.lblCount.Name = "lblCount";
      this.lblCount.Size = new System.Drawing.Size(40, 13);
      this.lblCount.TabIndex = 72;
      this.lblCount.Text = "Count";
      // 
      // btnExportExcel
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.btnExportExcel, 2);
      this.btnExportExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExportExcel.Location = new System.Drawing.Point(737, 576);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(93, 23);
      this.btnExportExcel.TabIndex = 70;
      this.btnExportExcel.Text = "Export Excel";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // btnGetTemplate
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.btnGetTemplate, 2);
      this.btnGetTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnGetTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnGetTemplate.Location = new System.Drawing.Point(816, 30);
      this.btnGetTemplate.Name = "btnGetTemplate";
      this.btnGetTemplate.Size = new System.Drawing.Size(95, 21);
      this.btnGetTemplate.TabIndex = 6;
      this.btnGetTemplate.Text = "Get Template";
      this.btnGetTemplate.UseVisualStyleBackColor = true;
      this.btnGetTemplate.Click += new System.EventHandler(this.btnGetTemplate_Click);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 34);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(33, 13);
      this.label1.TabIndex = 73;
      this.label1.Text = "Path";
      // 
      // rdbtME
      // 
      this.rdbtME.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.rdbtME.AutoSize = true;
      this.rdbtME.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.rdbtME.Location = new System.Drawing.Point(215, 5);
      this.rdbtME.Name = "rdbtME";
      this.rdbtME.Size = new System.Drawing.Size(41, 17);
      this.rdbtME.TabIndex = 1;
      this.rdbtME.TabStop = true;
      this.rdbtME.Text = "ME";
      this.rdbtME.UseVisualStyleBackColor = true;
      // 
      // viewCSD_01_016
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewCSD_01_016";
      this.Size = new System.Drawing.Size(914, 602);
      this.Load += new System.EventHandler(this.viewCSD_01_016_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TextBox txtImport;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Button btnGetTemplate;
    private System.Windows.Forms.Button btnBrown;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox txtSaleCode;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnExportExcel;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.RadioButton rdbtUK;
    private System.Windows.Forms.RadioButton rdbtJCRU;
    private System.Windows.Forms.Label lblCount;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.RadioButton rdbtCn;
    private System.Windows.Forms.RadioButton rdbtInt;
    private System.Windows.Forms.RadioButton rdbtAll;
    private System.Windows.Forms.RadioButton rdbtME;
  }
}
