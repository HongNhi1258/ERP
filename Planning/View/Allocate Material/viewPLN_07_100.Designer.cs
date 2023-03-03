namespace DaiCo.Planning
{
  partial class viewPLN_07_100
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
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnReAllocate = new System.Windows.Forms.Button();
      this.btnAllocate = new System.Windows.Forms.Button();
      this.btnAdjustByExcel = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
      this.chkShowMaterialListBox = new System.Windows.Forms.CheckBox();
      this.txtMaterialGroup = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.lblMonth = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.chkShowMaterialCodeListBox = new System.Windows.Forms.CheckBox();
      this.txtMaterialCode = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.multiCBMonth = new DaiCo.Shared.UserControls.MultiColumnComboBox();
      this.multiCBDepartment = new DaiCo.Shared.UserControls.MultiColumnComboBox();
      this.multiCBYear = new DaiCo.Shared.UserControls.MultiColumnComboBox();
      this.ucUltraListMaterialGroup = new DaiCo.Shared.ucUltraList();
      this.ucUltraListMaterial = new DaiCo.Shared.ucUltraList();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.ultraGridWOMaterialDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel12.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridWOMaterialDetail)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(642, 555);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 6;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Controls.Add(this.btnClose, 0, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnReAllocate, 2, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnAllocate, 3, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnAdjustByExcel, 4, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnExportExcel, 5, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 525);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.tableLayoutPanel5.RowCount = 1;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(642, 30);
      this.tableLayoutPanel5.TabIndex = 4;
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(564, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnReAllocate
      // 
      this.btnReAllocate.Location = new System.Drawing.Point(473, 3);
      this.btnReAllocate.Name = "btnReAllocate";
      this.btnReAllocate.Size = new System.Drawing.Size(85, 23);
      this.btnReAllocate.TabIndex = 1;
      this.btnReAllocate.Text = "Decrease";
      this.btnReAllocate.UseVisualStyleBackColor = true;
      this.btnReAllocate.Click += new System.EventHandler(this.btnReAllocate_Click);
      // 
      // btnAllocate
      // 
      this.btnAllocate.Location = new System.Drawing.Point(392, 3);
      this.btnAllocate.Name = "btnAllocate";
      this.btnAllocate.Size = new System.Drawing.Size(75, 23);
      this.btnAllocate.TabIndex = 0;
      this.btnAllocate.Text = "Increase";
      this.btnAllocate.UseVisualStyleBackColor = true;
      this.btnAllocate.Click += new System.EventHandler(this.btnAllocate_Click);
      // 
      // btnAdjustByExcel
      // 
      this.btnAdjustByExcel.Location = new System.Drawing.Point(266, 3);
      this.btnAdjustByExcel.Name = "btnAdjustByExcel";
      this.btnAdjustByExcel.Size = new System.Drawing.Size(120, 23);
      this.btnAdjustByExcel.TabIndex = 4;
      this.btnAdjustByExcel.Text = "Adjust By Excel";
      this.btnAdjustByExcel.UseVisualStyleBackColor = true;
      this.btnAdjustByExcel.Click += new System.EventHandler(this.btnAdjustByExcel_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.AutoSize = true;
      this.groupBox1.Controls.Add(this.tableLayoutPanel2);
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(636, 481);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Search";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.AutoSize = true;
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel12, 2, 3);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.lblMonth, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 3);
      this.tableLayoutPanel2.Controls.Add(this.btnSearch, 2, 7);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 5);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 2, 5);
      this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.multiCBMonth, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.multiCBDepartment, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.multiCBYear, 2, 2);
      this.tableLayoutPanel2.Controls.Add(this.ucUltraListMaterialGroup, 2, 4);
      this.tableLayoutPanel2.Controls.Add(this.ucUltraListMaterial, 2, 6);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 8;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(630, 462);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // tableLayoutPanel12
      // 
      this.tableLayoutPanel12.ColumnCount = 2;
      this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
      this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel12.Controls.Add(this.chkShowMaterialListBox, 0, 0);
      this.tableLayoutPanel12.Controls.Add(this.txtMaterialGroup, 1, 0);
      this.tableLayoutPanel12.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel12.Location = new System.Drawing.Point(105, 72);
      this.tableLayoutPanel12.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel12.Name = "tableLayoutPanel12";
      this.tableLayoutPanel12.RowCount = 1;
      this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel12.Size = new System.Drawing.Size(525, 24);
      this.tableLayoutPanel12.TabIndex = 3;
      // 
      // chkShowMaterialListBox
      // 
      this.chkShowMaterialListBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkShowMaterialListBox.AutoSize = true;
      this.chkShowMaterialListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkShowMaterialListBox.Location = new System.Drawing.Point(3, 5);
      this.chkShowMaterialListBox.Name = "chkShowMaterialListBox";
      this.chkShowMaterialListBox.Size = new System.Drawing.Size(15, 14);
      this.chkShowMaterialListBox.TabIndex = 0;
      this.chkShowMaterialListBox.UseVisualStyleBackColor = true;
      this.chkShowMaterialListBox.CheckedChanged += new System.EventHandler(this.chkShowMaterialListBox_CheckedChanged);
      // 
      // txtMaterialGroup
      // 
      this.txtMaterialGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMaterialGroup.Location = new System.Drawing.Point(28, 3);
      this.txtMaterialGroup.Name = "txtMaterialGroup";
      this.txtMaterialGroup.ReadOnly = true;
      this.txtMaterialGroup.Size = new System.Drawing.Size(494, 20);
      this.txtMaterialGroup.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 5);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Department";
      // 
      // lblMonth
      // 
      this.lblMonth.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblMonth.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.lblMonth, 2);
      this.lblMonth.Location = new System.Drawing.Point(3, 29);
      this.lblMonth.Name = "lblMonth";
      this.lblMonth.Size = new System.Drawing.Size(42, 13);
      this.lblMonth.TabIndex = 7;
      this.lblMonth.Text = "Month";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.label3, 2);
      this.label3.Location = new System.Drawing.Point(3, 77);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(90, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Material Group";
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Location = new System.Drawing.Point(552, 437);
      this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 5;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.label2, 2);
      this.label2.Location = new System.Drawing.Point(3, 258);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(85, 13);
      this.label2.TabIndex = 9;
      this.label2.Text = "Material Code";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.chkShowMaterialCodeListBox, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.txtMaterialCode, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(105, 253);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(525, 24);
      this.tableLayoutPanel3.TabIndex = 4;
      // 
      // chkShowMaterialCodeListBox
      // 
      this.chkShowMaterialCodeListBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkShowMaterialCodeListBox.AutoSize = true;
      this.chkShowMaterialCodeListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkShowMaterialCodeListBox.Location = new System.Drawing.Point(3, 5);
      this.chkShowMaterialCodeListBox.Name = "chkShowMaterialCodeListBox";
      this.chkShowMaterialCodeListBox.Size = new System.Drawing.Size(15, 14);
      this.chkShowMaterialCodeListBox.TabIndex = 0;
      this.chkShowMaterialCodeListBox.UseVisualStyleBackColor = true;
      this.chkShowMaterialCodeListBox.CheckedChanged += new System.EventHandler(this.chkShowMaterialCodeListBox_CheckedChanged);
      // 
      // txtMaterialCode
      // 
      this.txtMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMaterialCode.Location = new System.Drawing.Point(28, 3);
      this.txtMaterialCode.Name = "txtMaterialCode";
      this.txtMaterialCode.ReadOnly = true;
      this.txtMaterialCode.Size = new System.Drawing.Size(494, 20);
      this.txtMaterialCode.TabIndex = 0;
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.label4, 2);
      this.label4.Location = new System.Drawing.Point(3, 53);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(33, 13);
      this.label4.TabIndex = 12;
      this.label4.Text = "Year";
      // 
      // multiCBMonth
      // 
      this.multiCBMonth.AutoComplete = false;
      this.multiCBMonth.AutoDropdown = false;
      this.multiCBMonth.BackColorEven = System.Drawing.Color.White;
      this.multiCBMonth.BackColorOdd = System.Drawing.Color.White;
      this.multiCBMonth.ColumnNames = "";
      this.multiCBMonth.ColumnWidthDefault = 75;
      this.multiCBMonth.ColumnWidths = "";
      this.multiCBMonth.Dock = System.Windows.Forms.DockStyle.Fill;
      this.multiCBMonth.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.multiCBMonth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.multiCBMonth.FormattingEnabled = true;
      this.multiCBMonth.LinkedColumnIndex = 0;
      this.multiCBMonth.LinkedTextBox = null;
      this.multiCBMonth.Location = new System.Drawing.Point(108, 27);
      this.multiCBMonth.Name = "multiCBMonth";
      this.multiCBMonth.Size = new System.Drawing.Size(519, 21);
      this.multiCBMonth.TabIndex = 1;
      // 
      // multiCBDepartment
      // 
      this.multiCBDepartment.AutoComplete = false;
      this.multiCBDepartment.AutoDropdown = false;
      this.multiCBDepartment.BackColorEven = System.Drawing.Color.White;
      this.multiCBDepartment.BackColorOdd = System.Drawing.Color.White;
      this.multiCBDepartment.ColumnNames = "";
      this.multiCBDepartment.ColumnWidthDefault = 75;
      this.multiCBDepartment.ColumnWidths = "";
      this.multiCBDepartment.Dock = System.Windows.Forms.DockStyle.Fill;
      this.multiCBDepartment.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.multiCBDepartment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.multiCBDepartment.FormattingEnabled = true;
      this.multiCBDepartment.LinkedColumnIndex = 0;
      this.multiCBDepartment.LinkedTextBox = null;
      this.multiCBDepartment.Location = new System.Drawing.Point(108, 3);
      this.multiCBDepartment.Name = "multiCBDepartment";
      this.multiCBDepartment.Size = new System.Drawing.Size(519, 21);
      this.multiCBDepartment.TabIndex = 0;
      // 
      // multiCBYear
      // 
      this.multiCBYear.AutoComplete = false;
      this.multiCBYear.AutoDropdown = false;
      this.multiCBYear.BackColorEven = System.Drawing.Color.White;
      this.multiCBYear.BackColorOdd = System.Drawing.Color.White;
      this.multiCBYear.ColumnNames = "";
      this.multiCBYear.ColumnWidthDefault = 75;
      this.multiCBYear.ColumnWidths = "";
      this.multiCBYear.Dock = System.Windows.Forms.DockStyle.Fill;
      this.multiCBYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.multiCBYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.multiCBYear.FormattingEnabled = true;
      this.multiCBYear.LinkedColumnIndex = 0;
      this.multiCBYear.LinkedTextBox = null;
      this.multiCBYear.Location = new System.Drawing.Point(108, 51);
      this.multiCBYear.Name = "multiCBYear";
      this.multiCBYear.Size = new System.Drawing.Size(519, 21);
      this.multiCBYear.TabIndex = 2;
      // 
      // ucUltraListMaterialGroup
      // 
      this.ucUltraListMaterialGroup.AutoSearchBy = "";
      this.ucUltraListMaterialGroup.ColumnWidths = "";
      this.ucUltraListMaterialGroup.DataSource = null;
      this.ucUltraListMaterialGroup.DisplayMember = "";
      this.ucUltraListMaterialGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucUltraListMaterialGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucUltraListMaterialGroup.Location = new System.Drawing.Point(108, 99);
      this.ucUltraListMaterialGroup.Name = "ucUltraListMaterialGroup";
      this.ucUltraListMaterialGroup.SelectedText = "";
      this.ucUltraListMaterialGroup.SelectedValue = "";
      this.ucUltraListMaterialGroup.Separator = '\0';
      this.ucUltraListMaterialGroup.Size = new System.Drawing.Size(519, 151);
      this.ucUltraListMaterialGroup.TabIndex = 13;
      this.ucUltraListMaterialGroup.ValueMember = "";
      this.ucUltraListMaterialGroup.Visible = false;
      this.ucUltraListMaterialGroup.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucUltraListMaterialGroup_ValueChanged);
      // 
      // ucUltraListMaterial
      // 
      this.ucUltraListMaterial.AutoSearchBy = "";
      this.ucUltraListMaterial.ColumnWidths = "";
      this.ucUltraListMaterial.DataSource = null;
      this.ucUltraListMaterial.DisplayMember = "";
      this.ucUltraListMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucUltraListMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucUltraListMaterial.Location = new System.Drawing.Point(108, 280);
      this.ucUltraListMaterial.Name = "ucUltraListMaterial";
      this.ucUltraListMaterial.SelectedText = "";
      this.ucUltraListMaterial.SelectedValue = "";
      this.ucUltraListMaterial.Separator = '\0';
      this.ucUltraListMaterial.Size = new System.Drawing.Size(519, 151);
      this.ucUltraListMaterial.TabIndex = 14;
      this.ucUltraListMaterial.ValueMember = "";
      this.ucUltraListMaterial.Visible = false;
      this.ucUltraListMaterial.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucUltraListMaterial_ValueChanged);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultraGridWOMaterialDetail);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(3, 490);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(636, 32);
      this.groupBox2.TabIndex = 5;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Information";
      // 
      // ultraGridWOMaterialDetail
      // 
      this.ultraGridWOMaterialDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraGridWOMaterialDetail.DisplayLayout.AutoFitColumns = true;
      this.ultraGridWOMaterialDetail.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      this.ultraGridWOMaterialDetail.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
      this.ultraGridWOMaterialDetail.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraGridWOMaterialDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraGridWOMaterialDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraGridWOMaterialDetail.Location = new System.Drawing.Point(3, 16);
      this.ultraGridWOMaterialDetail.Name = "ultraGridWOMaterialDetail";
      this.ultraGridWOMaterialDetail.Size = new System.Drawing.Size(630, 13);
      this.ultraGridWOMaterialDetail.TabIndex = 0;
      this.ultraGridWOMaterialDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGridWOMaterialDetail_InitializeLayout);
      this.ultraGridWOMaterialDetail.DoubleClick += new System.EventHandler(this.ultraGridWOMaterialDetail_DoubleClick);
      // 
      // btnExportExcel
      // 
      this.btnExportExcel.AutoSize = true;
      this.btnExportExcel.Location = new System.Drawing.Point(172, 3);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(88, 23);
      this.btnExportExcel.TabIndex = 5;
      this.btnExportExcel.Text = "Export Excel";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // viewPLN_07_100
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_07_100";
      this.Size = new System.Drawing.Size(642, 555);
      this.Load += new System.EventHandler(this.viewPLN_07_100_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.tableLayoutPanel12.ResumeLayout(false);
      this.tableLayoutPanel12.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridWOMaterialDetail)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.GroupBox groupBox2;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridWOMaterialDetail;
    private System.Windows.Forms.Label lblMonth;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
    private System.Windows.Forms.CheckBox chkShowMaterialListBox;
    private System.Windows.Forms.TextBox txtMaterialGroup;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.CheckBox chkShowMaterialCodeListBox;
    private System.Windows.Forms.TextBox txtMaterialCode;
    private System.Windows.Forms.Button btnAllocate;
    private System.Windows.Forms.Button btnReAllocate;
    private System.Windows.Forms.Label label4;
    private DaiCo.Shared.UserControls.MultiColumnComboBox multiCBMonth;
    private DaiCo.Shared.UserControls.MultiColumnComboBox multiCBDepartment;
    private DaiCo.Shared.UserControls.MultiColumnComboBox multiCBYear;
    private DaiCo.Shared.ucUltraList ucUltraListMaterialGroup;
    private DaiCo.Shared.ucUltraList ucUltraListMaterial;
    private System.Windows.Forms.Button btnAdjustByExcel;
    private System.Windows.Forms.Button btnExportExcel;

  }
}
