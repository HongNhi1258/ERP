namespace DaiCo.Planning
{
  partial class viewPLN_07_014
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
      this.btnSave = new System.Windows.Forms.Button();
      this.btnExportToExcel = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.chkShowMaterialCodeListBox = new System.Windows.Forms.CheckBox();
      this.chkShowMaterialListBox = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.lbItemCode = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.ultraCBWO = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label2 = new System.Windows.Forms.Label();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSearch = new System.Windows.Forms.Button();
      this.ucUltraListItem = new DaiCo.Shared.ucUltraList();
      this.ucUltraListMaterialGroup = new DaiCo.Shared.ucUltraList();
      this.ucUltraListMaterial = new DaiCo.Shared.ucUltraList();
      this.chkShowItemListBox = new System.Windows.Forms.CheckBox();
      this.txtItemCode = new System.Windows.Forms.TextBox();
      this.txtMaterialGroup = new System.Windows.Forms.TextBox();
      this.txtMaterialCode = new System.Windows.Forms.TextBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.chkSelectedAll = new System.Windows.Forms.CheckBox();
      this.ultraGridWOMaterialDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBWO)).BeginInit();
      this.tableLayoutPanel4.SuspendLayout();
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
      this.tableLayoutPanel1.Size = new System.Drawing.Size(642, 634);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 4;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel5.Controls.Add(this.btnClose, 1, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnSave, 2, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnExportToExcel, 3, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 604);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.tableLayoutPanel5.RowCount = 1;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(642, 30);
      this.tableLayoutPanel5.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(564, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 0;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(483, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnExportToExcel
      // 
      this.btnExportToExcel.Location = new System.Drawing.Point(368, 3);
      this.btnExportToExcel.Name = "btnExportToExcel";
      this.btnExportToExcel.Size = new System.Drawing.Size(109, 23);
      this.btnExportToExcel.TabIndex = 2;
      this.btnExportToExcel.Text = "Export To Excel";
      this.btnExportToExcel.UseVisualStyleBackColor = true;
      this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
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
      this.groupBox1.Size = new System.Drawing.Size(636, 561);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Search";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.AutoSize = true;
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.chkShowMaterialCodeListBox, 1, 5);
      this.tableLayoutPanel2.Controls.Add(this.chkShowMaterialListBox, 1, 3);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.lbItemCode, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 3);
      this.tableLayoutPanel2.Controls.Add(this.ultraCBWO, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 0, 5);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 2, 7);
      this.tableLayoutPanel2.Controls.Add(this.ucUltraListItem, 2, 2);
      this.tableLayoutPanel2.Controls.Add(this.ucUltraListMaterialGroup, 2, 4);
      this.tableLayoutPanel2.Controls.Add(this.ucUltraListMaterial, 2, 6);
      this.tableLayoutPanel2.Controls.Add(this.chkShowItemListBox, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtItemCode, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtMaterialGroup, 2, 3);
      this.tableLayoutPanel2.Controls.Add(this.txtMaterialCode, 2, 5);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 8;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(630, 542);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // chkShowMaterialCodeListBox
      // 
      this.chkShowMaterialCodeListBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkShowMaterialCodeListBox.AutoSize = true;
      this.chkShowMaterialCodeListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkShowMaterialCodeListBox.Location = new System.Drawing.Point(103, 357);
      this.chkShowMaterialCodeListBox.Name = "chkShowMaterialCodeListBox";
      this.chkShowMaterialCodeListBox.Size = new System.Drawing.Size(15, 14);
      this.chkShowMaterialCodeListBox.TabIndex = 0;
      this.chkShowMaterialCodeListBox.UseVisualStyleBackColor = true;
      this.chkShowMaterialCodeListBox.CheckedChanged += new System.EventHandler(this.chkShowMaterialCodeListBox_CheckedChanged);
      // 
      // chkShowMaterialListBox
      // 
      this.chkShowMaterialListBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkShowMaterialListBox.AutoSize = true;
      this.chkShowMaterialListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkShowMaterialListBox.Location = new System.Drawing.Point(103, 195);
      this.chkShowMaterialListBox.Name = "chkShowMaterialListBox";
      this.chkShowMaterialListBox.Size = new System.Drawing.Size(15, 14);
      this.chkShowMaterialListBox.TabIndex = 0;
      this.chkShowMaterialListBox.UseVisualStyleBackColor = true;
      this.chkShowMaterialListBox.CheckedChanged += new System.EventHandler(this.chkShowMaterialListBox_CheckedChanged);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Work Order";
      // 
      // lbItemCode
      // 
      this.lbItemCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lbItemCode.AutoSize = true;
      this.lbItemCode.Location = new System.Drawing.Point(3, 33);
      this.lbItemCode.Name = "lbItemCode";
      this.lbItemCode.Size = new System.Drawing.Size(64, 13);
      this.lbItemCode.TabIndex = 3;
      this.lbItemCode.Text = "Item Code";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 195);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(90, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Material Group";
      // 
      // ultraCBWO
      // 
      this.ultraCBWO.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBWO.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBWO.DisplayLayout.AutoFitColumns = true;
      this.ultraCBWO.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBWO.DisplayMember = "";
      this.ultraCBWO.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBWO.Location = new System.Drawing.Point(128, 3);
      this.ultraCBWO.Name = "ultraCBWO";
      this.ultraCBWO.Size = new System.Drawing.Size(499, 21);
      this.ultraCBWO.TabIndex = 2;
      this.ultraCBWO.ValueMember = "";
      this.ultraCBWO.ValueChanged += new System.EventHandler(this.ultraCBWO_ValueChanged);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 357);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(85, 13);
      this.label2.TabIndex = 9;
      this.label2.Text = "Material Code";
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 2;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel4.Controls.Add(this.btnSearch, 1, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(125, 513);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(505, 29);
      this.tableLayoutPanel4.TabIndex = 13;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Location = new System.Drawing.Point(427, 2);
      this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 12;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // ucUltraListItem
      // 
      this.ucUltraListItem.ColumnWidths = "";
      this.ucUltraListItem.DataSource = null;
      this.ucUltraListItem.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucUltraListItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucUltraListItem.Location = new System.Drawing.Point(125, 56);
      this.ucUltraListItem.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.ucUltraListItem.Name = "ucUltraListItem";
      this.ucUltraListItem.SelectedValue = "";
      this.ucUltraListItem.Separator = '\0';
      this.ucUltraListItem.Size = new System.Drawing.Size(505, 130);
      this.ucUltraListItem.TabIndex = 14;
      this.ucUltraListItem.Text = "ucUltraList1";
      this.ucUltraListItem.ValueMember = "";
      this.ucUltraListItem.Visible = false;
      this.ucUltraListItem.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucUltraListItem_ValueChanged);
      // 
      // ucUltraListMaterialGroup
      // 
      this.ucUltraListMaterialGroup.ColumnWidths = "";
      this.ucUltraListMaterialGroup.DataSource = null;
      this.ucUltraListMaterialGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucUltraListMaterialGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucUltraListMaterialGroup.Location = new System.Drawing.Point(125, 218);
      this.ucUltraListMaterialGroup.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.ucUltraListMaterialGroup.Name = "ucUltraListMaterialGroup";
      this.ucUltraListMaterialGroup.SelectedValue = "";
      this.ucUltraListMaterialGroup.Separator = '\0';
      this.ucUltraListMaterialGroup.Size = new System.Drawing.Size(505, 130);
      this.ucUltraListMaterialGroup.TabIndex = 15;
      this.ucUltraListMaterialGroup.Text = "ucUltraList2";
      this.ucUltraListMaterialGroup.ValueMember = "";
      this.ucUltraListMaterialGroup.Visible = false;
      this.ucUltraListMaterialGroup.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucUltraListMaterialGroup_ValueChanged);
      // 
      // ucUltraListMaterial
      // 
      this.ucUltraListMaterial.ColumnWidths = "";
      this.ucUltraListMaterial.DataSource = null;
      this.ucUltraListMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucUltraListMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucUltraListMaterial.Location = new System.Drawing.Point(125, 380);
      this.ucUltraListMaterial.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.ucUltraListMaterial.Name = "ucUltraListMaterial";
      this.ucUltraListMaterial.SelectedValue = "";
      this.ucUltraListMaterial.Separator = '\0';
      this.ucUltraListMaterial.Size = new System.Drawing.Size(505, 130);
      this.ucUltraListMaterial.TabIndex = 16;
      this.ucUltraListMaterial.Text = "ucUltraList3";
      this.ucUltraListMaterial.ValueMember = "";
      this.ucUltraListMaterial.Visible = false;
      this.ucUltraListMaterial.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucUltraListMaterial_ValueChanged);
      // 
      // chkShowItemListBox
      // 
      this.chkShowItemListBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkShowItemListBox.AutoSize = true;
      this.chkShowItemListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkShowItemListBox.Location = new System.Drawing.Point(103, 33);
      this.chkShowItemListBox.Name = "chkShowItemListBox";
      this.chkShowItemListBox.Size = new System.Drawing.Size(15, 14);
      this.chkShowItemListBox.TabIndex = 0;
      this.chkShowItemListBox.UseVisualStyleBackColor = true;
      this.chkShowItemListBox.CheckedChanged += new System.EventHandler(this.chkShowItemListBox_CheckedChanged);
      // 
      // txtItemCode
      // 
      this.txtItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtItemCode.Location = new System.Drawing.Point(128, 30);
      this.txtItemCode.Name = "txtItemCode";
      this.txtItemCode.ReadOnly = true;
      this.txtItemCode.Size = new System.Drawing.Size(499, 20);
      this.txtItemCode.TabIndex = 1;
      // 
      // txtMaterialGroup
      // 
      this.txtMaterialGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMaterialGroup.Location = new System.Drawing.Point(128, 192);
      this.txtMaterialGroup.Name = "txtMaterialGroup";
      this.txtMaterialGroup.ReadOnly = true;
      this.txtMaterialGroup.Size = new System.Drawing.Size(499, 20);
      this.txtMaterialGroup.TabIndex = 1;
      // 
      // txtMaterialCode
      // 
      this.txtMaterialCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterialCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMaterialCode.Location = new System.Drawing.Point(128, 354);
      this.txtMaterialCode.Name = "txtMaterialCode";
      this.txtMaterialCode.ReadOnly = true;
      this.txtMaterialCode.Size = new System.Drawing.Size(499, 20);
      this.txtMaterialCode.TabIndex = 1;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.chkSelectedAll);
      this.groupBox2.Controls.Add(this.ultraGridWOMaterialDetail);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(3, 570);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(636, 31);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "List Materials were deleted";
      // 
      // chkSelectedAll
      // 
      this.chkSelectedAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.chkSelectedAll.AutoSize = true;
      this.chkSelectedAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkSelectedAll.Location = new System.Drawing.Point(500, -1);
      this.chkSelectedAll.Name = "chkSelectedAll";
      this.chkSelectedAll.Size = new System.Drawing.Size(62, 17);
      this.chkSelectedAll.TabIndex = 1;
      this.chkSelectedAll.Text = "Auto All";
      this.chkSelectedAll.UseVisualStyleBackColor = true;
      this.chkSelectedAll.CheckedChanged += new System.EventHandler(this.chkSelectedAll_CheckedChanged);
      // 
      // ultraGridWOMaterialDetail
      // 
      this.ultraGridWOMaterialDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraGridWOMaterialDetail.DisplayLayout.AutoFitColumns = true;
      this.ultraGridWOMaterialDetail.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      this.ultraGridWOMaterialDetail.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      this.ultraGridWOMaterialDetail.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
      this.ultraGridWOMaterialDetail.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraGridWOMaterialDetail.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraGridWOMaterialDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraGridWOMaterialDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraGridWOMaterialDetail.Location = new System.Drawing.Point(3, 16);
      this.ultraGridWOMaterialDetail.Name = "ultraGridWOMaterialDetail";
      this.ultraGridWOMaterialDetail.Size = new System.Drawing.Size(630, 12);
      this.ultraGridWOMaterialDetail.TabIndex = 0;
      this.ultraGridWOMaterialDetail.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultraGridWOMaterialDetail_MouseDoubleClick);
      this.ultraGridWOMaterialDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGridWOMaterialDetail_InitializeLayout);
      this.ultraGridWOMaterialDetail.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraGridWOMaterialDetail_AfterCellUpdate);
      this.ultraGridWOMaterialDetail.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraGridWOMaterialDetail_CellChange);
      // 
      // viewPLN_07_014
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_07_014";
      this.Size = new System.Drawing.Size(642, 634);
      this.Load += new System.EventHandler(this.viewPLN_07_014_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBWO)).EndInit();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
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
    private System.Windows.Forms.Label lbItemCode;
    private System.Windows.Forms.CheckBox chkShowItemListBox;
    private System.Windows.Forms.TextBox txtItemCode;
    private System.Windows.Forms.CheckBox chkShowMaterialListBox;
    private System.Windows.Forms.TextBox txtMaterialGroup;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBWO;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox chkShowMaterialCodeListBox;
    private System.Windows.Forms.TextBox txtMaterialCode;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.CheckBox chkSelectedAll;
    private DaiCo.Shared.ucUltraList ucUltraListItem;
    private DaiCo.Shared.ucUltraList ucUltraListMaterialGroup;
    private DaiCo.Shared.ucUltraList ucUltraListMaterial;
    private System.Windows.Forms.Button btnExportToExcel;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;

  }
}
