namespace DaiCo.CustomerService
{
  partial class viewCSD_02_001
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
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnExport = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txtCustomerCode = new System.Windows.Forms.TextBox();
      this.multiCBParentCustomer = new DaiCo.Shared.UserControls.MultiColumnComboBox();
      this.btnSearch = new System.Windows.Forms.Button();
      this.multiCBResponsiblePerson = new DaiCo.Shared.UserControls.MultiColumnComboBox();
      this.txtName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.multiCBKind = new DaiCo.Shared.UserControls.MultiColumnComboBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.ultraGridInformation = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridInformation)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(706, 450);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 4;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnNew, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnDelete, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnExport, 3, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 420);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(706, 30);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(632, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(71, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnNew
      // 
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.Location = new System.Drawing.Point(555, 3);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(71, 23);
      this.btnNew.TabIndex = 1;
      this.btnNew.Text = "New";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // btnDelete
      // 
      this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDelete.Location = new System.Drawing.Point(474, 3);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(75, 23);
      this.btnDelete.TabIndex = 0;
      this.btnDelete.Text = "Delete";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // btnExport
      // 
      this.btnExport.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.btnExport.Enabled = false;
      this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExport.Location = new System.Drawing.Point(361, 3);
      this.btnExport.Name = "btnExport";
      this.btnExport.Size = new System.Drawing.Size(107, 23);
      this.btnExport.TabIndex = 3;
      this.btnExport.Text = "Export To Excel";
      this.btnExport.UseVisualStyleBackColor = true;
      this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(700, 104);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Search";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 5;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 126F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtCustomerCode, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.multiCBParentCustomer, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.btnSearch, 4, 2);
      this.tableLayoutPanel2.Controls.Add(this.multiCBResponsiblePerson, 4, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtName, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.label4, 3, 1);
      this.tableLayoutPanel2.Controls.Add(this.label5, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.multiCBKind, 1, 2);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 4;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(694, 85);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(36, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Code";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 33);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(61, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Distribute";
      // 
      // txtCustomerCode
      // 
      this.txtCustomerCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtCustomerCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtCustomerCode.Location = new System.Drawing.Point(93, 3);
      this.txtCustomerCode.Name = "txtCustomerCode";
      this.txtCustomerCode.Size = new System.Drawing.Size(213, 20);
      this.txtCustomerCode.TabIndex = 1;
      // 
      // multiCBParentCustomer
      // 
      this.multiCBParentCustomer.AutoComplete = false;
      this.multiCBParentCustomer.AutoDropdown = false;
      this.multiCBParentCustomer.BackColorEven = System.Drawing.Color.White;
      this.multiCBParentCustomer.BackColorOdd = System.Drawing.Color.White;
      this.multiCBParentCustomer.ColumnNames = "";
      this.multiCBParentCustomer.ColumnWidthDefault = 75;
      this.multiCBParentCustomer.ColumnWidths = "";
      this.multiCBParentCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.multiCBParentCustomer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.multiCBParentCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.multiCBParentCustomer.FormattingEnabled = true;
      this.multiCBParentCustomer.LinkedColumnIndex = 0;
      this.multiCBParentCustomer.LinkedTextBox = null;
      this.multiCBParentCustomer.Location = new System.Drawing.Point(93, 29);
      this.multiCBParentCustomer.Name = "multiCBParentCustomer";
      this.multiCBParentCustomer.Size = new System.Drawing.Size(213, 21);
      this.multiCBParentCustomer.TabIndex = 5;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Location = new System.Drawing.Point(617, 55);
      this.btnSearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 1);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 10;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // multiCBResponsiblePerson
      // 
      this.multiCBResponsiblePerson.AutoComplete = false;
      this.multiCBResponsiblePerson.AutoDropdown = false;
      this.multiCBResponsiblePerson.BackColorEven = System.Drawing.Color.White;
      this.multiCBResponsiblePerson.BackColorOdd = System.Drawing.Color.White;
      this.multiCBResponsiblePerson.ColumnNames = "";
      this.multiCBResponsiblePerson.ColumnWidthDefault = 75;
      this.multiCBResponsiblePerson.ColumnWidths = "";
      this.multiCBResponsiblePerson.Dock = System.Windows.Forms.DockStyle.Fill;
      this.multiCBResponsiblePerson.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.multiCBResponsiblePerson.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.multiCBResponsiblePerson.FormattingEnabled = true;
      this.multiCBResponsiblePerson.LinkedColumnIndex = 0;
      this.multiCBResponsiblePerson.LinkedTextBox = null;
      this.multiCBResponsiblePerson.Location = new System.Drawing.Point(478, 29);
      this.multiCBResponsiblePerson.Name = "multiCBResponsiblePerson";
      this.multiCBResponsiblePerson.Size = new System.Drawing.Size(213, 21);
      this.multiCBResponsiblePerson.TabIndex = 7;
      // 
      // txtName
      // 
      this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtName.Location = new System.Drawing.Point(478, 3);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(213, 20);
      this.txtName.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(352, 6);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(39, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Name";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(352, 33);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(119, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Responsible Person";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 60);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(32, 13);
      this.label5.TabIndex = 8;
      this.label5.Text = "Kind";
      // 
      // multiCBKind
      // 
      this.multiCBKind.AutoComplete = false;
      this.multiCBKind.AutoDropdown = false;
      this.multiCBKind.BackColorEven = System.Drawing.Color.White;
      this.multiCBKind.BackColorOdd = System.Drawing.Color.White;
      this.multiCBKind.ColumnNames = "";
      this.multiCBKind.ColumnWidthDefault = 75;
      this.multiCBKind.ColumnWidths = "";
      this.multiCBKind.Dock = System.Windows.Forms.DockStyle.Fill;
      this.multiCBKind.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.multiCBKind.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.multiCBKind.FormattingEnabled = true;
      this.multiCBKind.LinkedColumnIndex = 0;
      this.multiCBKind.LinkedTextBox = null;
      this.multiCBKind.Location = new System.Drawing.Point(93, 56);
      this.multiCBKind.Name = "multiCBKind";
      this.multiCBKind.Size = new System.Drawing.Size(213, 21);
      this.multiCBKind.TabIndex = 9;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultraGridInformation);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 113);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(700, 304);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Information";
      // 
      // ultraGridInformation
      // 
      this.ultraGridInformation.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraGridInformation.DisplayLayout.AutoFitColumns = true;
      this.ultraGridInformation.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraGridInformation.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraGridInformation.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraGridInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraGridInformation.Location = new System.Drawing.Point(3, 16);
      this.ultraGridInformation.Name = "ultraGridInformation";
      this.ultraGridInformation.Size = new System.Drawing.Size(694, 285);
      this.ultraGridInformation.TabIndex = 0;
      this.ultraGridInformation.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultraGridInformation_MouseDoubleClick);
      this.ultraGridInformation.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGridInformation_InitializeLayout);
      // 
      // viewCSD_02_001
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewCSD_02_001";
      this.Size = new System.Drawing.Size(706, 450);
      this.Load += new System.EventHandler(this.viewCSD_02_001_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridInformation)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtCustomerCode;
    private System.Windows.Forms.TextBox txtName;
    private DaiCo.Shared.UserControls.MultiColumnComboBox multiCBParentCustomer;
    private DaiCo.Shared.UserControls.MultiColumnComboBox multiCBResponsiblePerson;
    private System.Windows.Forms.Button btnSearch;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridInformation;
    private System.Windows.Forms.Label label5;
    private DaiCo.Shared.UserControls.MultiColumnComboBox multiCBKind;
    private System.Windows.Forms.Button btnExport;
  }
}
