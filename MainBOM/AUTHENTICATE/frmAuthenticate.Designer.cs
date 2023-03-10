namespace MainBOM.AUTHENTICATE
{
  partial class frmAuthenticate
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("", -1);
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAuthenticate));
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.uEGSearch = new Infragistics.Win.Misc.UltraExpandableGroupBox();
      this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.ultraCBDepartment = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.txtUserName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.ultraCBEmployee = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultEmpList = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.ultGroup = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnAddEmployee = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnAddGroup = new System.Windows.Forms.Button();
      this.btnResetPass = new System.Windows.Forms.Button();
      this.btnSameRight = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tvAuthenticate = new System.Windows.Forms.TreeView();
      this.ctmController = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mnuNewChildNode = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuProperties = new System.Windows.Forms.ToolStripMenuItem();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.btnCheckRight = new System.Windows.Forms.Button();
      this.chxCarcass = new System.Windows.Forms.CheckBox();
      this.chxComponent = new System.Windows.Forms.CheckBox();
      this.btnAddModult = new System.Windows.Forms.Button();
      this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uEGSearch)).BeginInit();
      this.uEGSearch.SuspendLayout();
      this.ultraExpandableGroupBoxPanel1.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBDepartment)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBEmployee)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultEmpList)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultGroup)).BeginInit();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.ctmController.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.AutoScroll = true;
      this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
      this.splitContainer1.Size = new System.Drawing.Size(929, 579);
      this.splitContainer1.SplitterDistance = 504;
      this.splitContainer1.TabIndex = 0;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.uEGSearch, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultEmpList, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.ultGroup, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 0, 3);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 4;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(504, 579);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // uEGSearch
      // 
      this.uEGSearch.Controls.Add(this.ultraExpandableGroupBoxPanel1);
      this.uEGSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uEGSearch.ExpandedSize = new System.Drawing.Size(498, 110);
      this.uEGSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uEGSearch.Location = new System.Drawing.Point(3, 3);
      this.uEGSearch.Name = "uEGSearch";
      this.uEGSearch.Size = new System.Drawing.Size(498, 110);
      this.uEGSearch.TabIndex = 0;
      this.uEGSearch.Text = "Search";
      // 
      // ultraExpandableGroupBoxPanel1
      // 
      this.ultraExpandableGroupBoxPanel1.Controls.Add(this.tableLayoutPanel3);
      this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
      this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
      this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(492, 88);
      this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 4;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.ultraCBDepartment, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSearch, 3, 2);
      this.tableLayoutPanel3.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel3.Controls.Add(this.txtUserName, 1, 2);
      this.tableLayoutPanel3.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.ultraCBEmployee, 1, 1);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 4;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(492, 88);
      this.tableLayoutPanel3.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Department";
      // 
      // ultraCBDepartment
      // 
      this.tableLayoutPanel3.SetColumnSpan(this.ultraCBDepartment, 3);
      this.ultraCBDepartment.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBDepartment.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultraCBDepartment.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraCBDepartment.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBDepartment.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBDepartment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBDepartment.Location = new System.Drawing.Point(103, 3);
      this.ultraCBDepartment.Name = "ultraCBDepartment";
      this.ultraCBDepartment.Size = new System.Drawing.Size(386, 22);
      this.ultraCBDepartment.TabIndex = 1;
      this.ultraCBDepartment.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraCBDepartment_InitializeLayout);
      this.ultraCBDepartment.ValueChanged += new System.EventHandler(this.ultraCBDepartment_ValueChanged);
      // 
      // btnSearch
      // 
      this.btnSearch.Image = global::MainBOM.Properties.Resources.search;
      this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSearch.Location = new System.Drawing.Point(414, 59);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 6;
      this.btnSearch.Text = "   Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 64);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(69, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "User Name";
      // 
      // txtUserName
      // 
      this.txtUserName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtUserName.Location = new System.Drawing.Point(103, 60);
      this.txtUserName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtUserName.Name = "txtUserName";
      this.txtUserName.Size = new System.Drawing.Size(285, 20);
      this.txtUserName.TabIndex = 5;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 35);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(61, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Employee";
      // 
      // ultraCBEmployee
      // 
      this.tableLayoutPanel3.SetColumnSpan(this.ultraCBEmployee, 3);
      this.ultraCBEmployee.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.ultraCBEmployee.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultraCBEmployee.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraCBEmployee.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBEmployee.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBEmployee.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBEmployee.Location = new System.Drawing.Point(103, 31);
      this.ultraCBEmployee.Name = "ultraCBEmployee";
      this.ultraCBEmployee.Size = new System.Drawing.Size(386, 22);
      this.ultraCBEmployee.TabIndex = 3;
      this.ultraCBEmployee.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraCBEmployee_InitializeLayout);
      // 
      // ultEmpList
      // 
      this.ultEmpList.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultEmpList.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      ultraGridBand1.Override.MaxSelectedRows = 1;
      this.ultEmpList.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
      this.ultEmpList.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      this.ultEmpList.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      this.ultEmpList.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      this.ultEmpList.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultEmpList.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultEmpList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultEmpList.Location = new System.Drawing.Point(3, 119);
      this.ultEmpList.Name = "ultEmpList";
      this.ultEmpList.Size = new System.Drawing.Size(498, 211);
      this.ultEmpList.TabIndex = 1;
      this.ultEmpList.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultEmpList_InitializeLayout);
      this.ultEmpList.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.ultEmpList_AfterSelectChange);
      this.ultEmpList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultEmpList_MouseDoubleClick);
      // 
      // ultGroup
      // 
      this.ultGroup.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultGroup.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultGroup.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultGroup.Location = new System.Drawing.Point(3, 336);
      this.ultGroup.Name = "ultGroup";
      this.ultGroup.Size = new System.Drawing.Size(498, 211);
      this.ultGroup.TabIndex = 2;
      this.ultGroup.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultGroup_InitializeLayout);
      this.ultGroup.DoubleClick += new System.EventHandler(this.ultGroup_DoubleClick);
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 6;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.Controls.Add(this.btnAddEmployee, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 5, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnAddGroup, 3, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnResetPass, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSameRight, 2, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 550);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(504, 29);
      this.tableLayoutPanel4.TabIndex = 3;
      // 
      // btnAddEmployee
      // 
      this.btnAddEmployee.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnAddEmployee.Location = new System.Drawing.Point(3, 3);
      this.btnAddEmployee.Name = "btnAddEmployee";
      this.btnAddEmployee.Size = new System.Drawing.Size(75, 23);
      this.btnAddEmployee.TabIndex = 0;
      this.btnAddEmployee.Text = "Add User";
      this.btnAddEmployee.UseVisualStyleBackColor = true;
      this.btnAddEmployee.Click += new System.EventHandler(this.btnAddEmployee_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Location = new System.Drawing.Point(384, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(117, 23);
      this.btnSave.TabIndex = 4;
      this.btnSave.Text = "Save Authenticate";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnAddGroup
      // 
      this.btnAddGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnAddGroup.Location = new System.Drawing.Point(266, 3);
      this.btnAddGroup.Name = "btnAddGroup";
      this.btnAddGroup.Size = new System.Drawing.Size(77, 23);
      this.btnAddGroup.TabIndex = 3;
      this.btnAddGroup.Text = "Add Group";
      this.btnAddGroup.UseVisualStyleBackColor = true;
      this.btnAddGroup.Click += new System.EventHandler(this.btnModuleCodeMaster_Click);
      // 
      // btnResetPass
      // 
      this.btnResetPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnResetPass.Location = new System.Drawing.Point(84, 3);
      this.btnResetPass.Name = "btnResetPass";
      this.btnResetPass.Size = new System.Drawing.Size(75, 23);
      this.btnResetPass.TabIndex = 1;
      this.btnResetPass.Text = "Reset Pass";
      this.btnResetPass.UseVisualStyleBackColor = true;
      this.btnResetPass.Click += new System.EventHandler(this.btnResetPass_Click);
      // 
      // btnSameRight
      // 
      this.btnSameRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnSameRight.Location = new System.Drawing.Point(165, 3);
      this.btnSameRight.Name = "btnSameRight";
      this.btnSameRight.Size = new System.Drawing.Size(95, 23);
      this.btnSameRight.TabIndex = 2;
      this.btnSameRight.Text = "Same Right With";
      this.btnSameRight.UseVisualStyleBackColor = true;
      this.btnSameRight.Click += new System.EventHandler(this.btnSameRight_Click);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnAddModult, 1, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(421, 579);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
      this.groupBox1.Controls.Add(this.tvAuthenticate);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(415, 534);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Tree Nodes";
      // 
      // tvAuthenticate
      // 
      this.tvAuthenticate.CheckBoxes = true;
      this.tvAuthenticate.ContextMenuStrip = this.ctmController;
      this.tvAuthenticate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tvAuthenticate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tvAuthenticate.FullRowSelect = true;
      this.tvAuthenticate.ImageIndex = 0;
      this.tvAuthenticate.ImageList = this.imageList1;
      this.tvAuthenticate.Location = new System.Drawing.Point(3, 16);
      this.tvAuthenticate.Name = "tvAuthenticate";
      this.tvAuthenticate.SelectedImageIndex = 0;
      this.tvAuthenticate.Size = new System.Drawing.Size(409, 515);
      this.tvAuthenticate.TabIndex = 0;
      this.tvAuthenticate.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvAuthenticate_AfterCheck);
      this.tvAuthenticate.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvAuthenticate_NodeMouseClick);
      // 
      // ctmController
      // 
      this.ctmController.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNewChildNode,
            this.mnuProperties});
      this.ctmController.Name = "ctmController";
      this.ctmController.Size = new System.Drawing.Size(158, 48);
      // 
      // mnuNewChildNode
      // 
      this.mnuNewChildNode.Name = "mnuNewChildNode";
      this.mnuNewChildNode.Size = new System.Drawing.Size(157, 22);
      this.mnuNewChildNode.Text = "New child node";
      this.mnuNewChildNode.Click += new System.EventHandler(this.mnuNewChildNode_Click);
      // 
      // mnuProperties
      // 
      this.mnuProperties.Name = "mnuProperties";
      this.mnuProperties.Size = new System.Drawing.Size(157, 22);
      this.mnuProperties.Text = "Properties";
      this.mnuProperties.Click += new System.EventHandler(this.mnuProperties_Click);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "view.PNG");
      this.imageList1.Images.SetKeyName(1, "control.PNG");
      this.imageList1.Images.SetKeyName(2, "group.PNG");
      this.imageList1.Images.SetKeyName(3, "inActive.ico");
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.btnCheckRight);
      this.groupBox2.Controls.Add(this.chxCarcass);
      this.groupBox2.Controls.Add(this.chxComponent);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Location = new System.Drawing.Point(3, 543);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(328, 33);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "WIP";
      // 
      // btnCheckRight
      // 
      this.btnCheckRight.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.btnCheckRight.Location = new System.Drawing.Point(249, 7);
      this.btnCheckRight.Name = "btnCheckRight";
      this.btnCheckRight.Size = new System.Drawing.Size(75, 23);
      this.btnCheckRight.TabIndex = 2;
      this.btnCheckRight.Text = "Check Right";
      this.btnCheckRight.UseVisualStyleBackColor = true;
      this.btnCheckRight.Click += new System.EventHandler(this.btnCheckRight_Click);
      // 
      // chxCarcass
      // 
      this.chxCarcass.AutoSize = true;
      this.chxCarcass.Location = new System.Drawing.Point(144, 14);
      this.chxCarcass.Name = "chxCarcass";
      this.chxCarcass.Size = new System.Drawing.Size(64, 17);
      this.chxCarcass.TabIndex = 1;
      this.chxCarcass.Text = "Carcass";
      this.chxCarcass.UseVisualStyleBackColor = true;
      // 
      // chxComponent
      // 
      this.chxComponent.AutoSize = true;
      this.chxComponent.Location = new System.Drawing.Point(34, 14);
      this.chxComponent.Name = "chxComponent";
      this.chxComponent.Size = new System.Drawing.Size(80, 17);
      this.chxComponent.TabIndex = 0;
      this.chxComponent.Text = "Component";
      this.chxComponent.UseVisualStyleBackColor = true;
      // 
      // btnAddModult
      // 
      this.btnAddModult.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnAddModult.Location = new System.Drawing.Point(343, 551);
      this.btnAddModult.Margin = new System.Windows.Forms.Padding(9, 9, 3, 3);
      this.btnAddModult.Name = "btnAddModult";
      this.btnAddModult.Size = new System.Drawing.Size(75, 23);
      this.btnAddModult.TabIndex = 2;
      this.btnAddModult.Text = "Add Module";
      this.btnAddModult.UseVisualStyleBackColor = true;
      this.btnAddModult.Click += new System.EventHandler(this.btnAddModult_Click);
      // 
      // contextMenuStrip1
      // 
      this.contextMenuStrip1.Name = "contextMenuStrip1";
      this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
      // 
      // frmAuthenticate
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(929, 579);
      this.Controls.Add(this.splitContainer1);
      this.Name = "frmAuthenticate";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Authenticate";
      this.Load += new System.EventHandler(this.frmAuthenticate_Load);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.uEGSearch)).EndInit();
      this.uEGSearch.ResumeLayout(false);
      this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBDepartment)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBEmployee)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultEmpList)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultGroup)).EndInit();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.ctmController.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TreeView tvAuthenticate;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnAddEmployee;
    private System.Windows.Forms.Button btnSave;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultEmpList;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.Button btnSameRight;
    private System.Windows.Forms.Button btnResetPass;
    private System.Windows.Forms.ContextMenuStrip ctmController;
    private System.Windows.Forms.ToolStripMenuItem mnuNewChildNode;
    private System.Windows.Forms.ToolStripMenuItem mnuProperties;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.CheckBox chxCarcass;
    private System.Windows.Forms.CheckBox chxComponent;
    private System.Windows.Forms.Button btnAddModult;
    private System.Windows.Forms.TextBox txtUserName;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBEmployee;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBDepartment;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnAddGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultGroup;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    private System.Windows.Forms.Button btnCheckRight;
    private Infragistics.Win.Misc.UltraExpandableGroupBox uEGSearch;
    private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
  }
}