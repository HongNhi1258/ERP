namespace MainBOM.AUTHENTICATE
{
  partial class frmAddGroup
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddGroup));
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.radUser = new System.Windows.Forms.RadioButton();
      this.radCode = new System.Windows.Forms.RadioButton();
      this.ultraGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnAddUser = new System.Windows.Forms.Button();
      this.btnControlCodeMaster = new System.Windows.Forms.Button();
      this.ultDDRole = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
      this.tvAuthenticate = new System.Windows.Forms.TreeView();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.txGroupControl = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSaveAuth = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txtGroup = new System.Windows.Forms.TextBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.ultGroupCopy = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label3 = new System.Windows.Forms.Label();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGrid)).BeginInit();
      this.tableLayoutPanel4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDRole)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel6.SuspendLayout();
      this.tableLayoutPanel7.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.groupBox4.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultGroupCopy)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 1, 3);
      this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(952, 586);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.radUser);
      this.groupBox1.Controls.Add(this.radCode);
      this.groupBox1.Controls.Add(this.ultraGrid);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 63);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(470, 433);
      this.groupBox1.TabIndex = 3;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Detail";
      // 
      // radUser
      // 
      this.radUser.AutoSize = true;
      this.radUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radUser.Location = new System.Drawing.Point(247, 0);
      this.radUser.Name = "radUser";
      this.radUser.Size = new System.Drawing.Size(77, 17);
      this.radUser.TabIndex = 2;
      this.radUser.TabStop = true;
      this.radUser.Text = "Add User";
      this.radUser.UseVisualStyleBackColor = true;
      // 
      // radCode
      // 
      this.radCode.AutoSize = true;
      this.radCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.radCode.Location = new System.Drawing.Point(92, 0);
      this.radCode.Name = "radCode";
      this.radCode.Size = new System.Drawing.Size(140, 17);
      this.radCode.TabIndex = 1;
      this.radCode.TabStop = true;
      this.radCode.Text = "Control Code Master";
      this.radCode.UseVisualStyleBackColor = true;
      // 
      // ultraGrid
      // 
      this.ultraGrid.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraGrid.Location = new System.Drawing.Point(3, 17);
      this.ultraGrid.Name = "ultraGrid";
      this.ultraGrid.Size = new System.Drawing.Size(464, 413);
      this.ultraGrid.TabIndex = 0;
      this.ultraGrid.AfterRowsDeleted += new System.EventHandler(this.ultraGrid_AfterRowsDeleted);
      this.ultraGrid.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultraGrid_BeforeCellUpdate);
      this.ultraGrid.BeforeRowUpdate += new Infragistics.Win.UltraWinGrid.CancelableRowEventHandler(this.ultraGrid_BeforeRowUpdate);
      this.ultraGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGrid_InitializeLayout);
      this.ultraGrid.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultraGrid_BeforeRowsDeleted);
      this.ultraGrid.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ultraGrid_MouseClick);
      this.ultraGrid.DoubleClick += new System.EventHandler(this.ultraGrid_DoubleClick);
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 5;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 4, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnAddUser, 3, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnControlCodeMaster, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.ultDDRole, 1, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 557);
      this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 2;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(476, 29);
      this.tableLayoutPanel4.TabIndex = 2;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(387, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(86, 23);
      this.btnSave.TabIndex = 2;
      this.btnSave.Text = "Save Group";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnAddUser
      // 
      this.btnAddUser.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnAddUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAddUser.Location = new System.Drawing.Point(309, 3);
      this.btnAddUser.Name = "btnAddUser";
      this.btnAddUser.Size = new System.Drawing.Size(72, 23);
      this.btnAddUser.TabIndex = 1;
      this.btnAddUser.Text = "Add User";
      this.btnAddUser.UseVisualStyleBackColor = true;
      this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
      // 
      // btnControlCodeMaster
      // 
      this.btnControlCodeMaster.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnControlCodeMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnControlCodeMaster.Location = new System.Drawing.Point(166, 3);
      this.btnControlCodeMaster.Name = "btnControlCodeMaster";
      this.btnControlCodeMaster.Size = new System.Drawing.Size(137, 23);
      this.btnControlCodeMaster.TabIndex = 0;
      this.btnControlCodeMaster.Text = "Control Code Master";
      this.btnControlCodeMaster.UseVisualStyleBackColor = true;
      this.btnControlCodeMaster.Click += new System.EventHandler(this.btnControlCodeMaster_Click);
      // 
      // ultDDRole
      // 
      this.ultDDRole.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.ultDDRole.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDRole.DisplayMember = "";
      this.ultDDRole.Location = new System.Drawing.Point(52, 3);
      this.ultDDRole.Name = "ultDDRole";
      this.ultDDRole.Size = new System.Drawing.Size(107, 23);
      this.ultDDRole.TabIndex = 3;
      this.ultDDRole.Text = "ultraDropDown1";
      this.ultDDRole.ValueMember = "";
      this.ultDDRole.Visible = false;
      this.ultDDRole.RowSelected += new Infragistics.Win.UltraWinGrid.RowSelectedEventHandler(this.ultDDRole_RowSelected);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.tableLayoutPanel6);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(479, 3);
      this.groupBox2.Name = "groupBox2";
      this.tableLayoutPanel1.SetRowSpan(this.groupBox2, 3);
      this.groupBox2.Size = new System.Drawing.Size(470, 551);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Tree Nodes";
      // 
      // tableLayoutPanel6
      // 
      this.tableLayoutPanel6.ColumnCount = 1;
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel6.Controls.Add(this.tvAuthenticate, 0, 1);
      this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 0);
      this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 17);
      this.tableLayoutPanel6.Name = "tableLayoutPanel6";
      this.tableLayoutPanel6.RowCount = 2;
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel6.Size = new System.Drawing.Size(464, 531);
      this.tableLayoutPanel6.TabIndex = 0;
      // 
      // tvAuthenticate
      // 
      this.tvAuthenticate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tvAuthenticate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tvAuthenticate.ImageIndex = 1;
      this.tvAuthenticate.ImageList = this.imageList1;
      this.tvAuthenticate.Location = new System.Drawing.Point(3, 32);
      this.tvAuthenticate.Name = "tvAuthenticate";
      this.tvAuthenticate.SelectedImageIndex = 0;
      this.tvAuthenticate.Size = new System.Drawing.Size(458, 496);
      this.tvAuthenticate.TabIndex = 0;
      this.tvAuthenticate.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvAuthenticate_AfterSelect);
      this.tvAuthenticate.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvAuthenticate_NodeMouseClick);
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
      // tableLayoutPanel7
      // 
      this.tableLayoutPanel7.ColumnCount = 2;
      this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
      this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel7.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel7.Controls.Add(this.txGroupControl, 1, 0);
      this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tableLayoutPanel7.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel7.Name = "tableLayoutPanel7";
      this.tableLayoutPanel7.RowCount = 2;
      this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel7.Size = new System.Drawing.Size(464, 29);
      this.tableLayoutPanel7.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(125, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Group Control Nodes";
      // 
      // txGroupControl
      // 
      this.txGroupControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txGroupControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txGroupControl.Location = new System.Drawing.Point(134, 4);
      this.txGroupControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txGroupControl.Name = "txGroupControl";
      this.txGroupControl.ReadOnly = true;
      this.txGroupControl.Size = new System.Drawing.Size(327, 20);
      this.txGroupControl.TabIndex = 0;
      this.txGroupControl.TextChanged += new System.EventHandler(this.txGroupControl_TextChanged);
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 2;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
      this.tableLayoutPanel5.Controls.Add(this.btnSaveAuth, 0, 0);
      this.tableLayoutPanel5.Controls.Add(this.btnClose, 1, 0);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Location = new System.Drawing.Point(476, 557);
      this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 2;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(476, 29);
      this.tableLayoutPanel5.TabIndex = 4;
      // 
      // btnSaveAuth
      // 
      this.btnSaveAuth.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSaveAuth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSaveAuth.Location = new System.Drawing.Point(275, 3);
      this.btnSaveAuth.Name = "btnSaveAuth";
      this.btnSaveAuth.Size = new System.Drawing.Size(128, 23);
      this.btnSaveAuth.TabIndex = 4;
      this.btnSaveAuth.Text = "Save Authenticate";
      this.btnSaveAuth.UseVisualStyleBackColor = true;
      this.btnSaveAuth.Click += new System.EventHandler(this.btnSaveAuth_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(409, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(64, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.tableLayoutPanel2);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox3.Location = new System.Drawing.Point(3, 3);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(470, 54);
      this.groupBox3.TabIndex = 5;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Search";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel2.Controls.Add(this.btnSearch, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtGroup, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 17);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(464, 34);
      this.tableLayoutPanel2.TabIndex = 1;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(386, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 2;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(77, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Group Name";
      // 
      // txtGroup
      // 
      this.txtGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtGroup.Location = new System.Drawing.Point(89, 4);
      this.txtGroup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtGroup.Name = "txtGroup";
      this.txtGroup.Size = new System.Drawing.Size(291, 20);
      this.txtGroup.TabIndex = 1;
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.tableLayoutPanel3);
      this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox4.Location = new System.Drawing.Point(3, 502);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(470, 52);
      this.groupBox4.TabIndex = 6;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Copy Structure";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Controls.Add(this.ultGroupCopy, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 17);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(464, 32);
      this.tableLayoutPanel3.TabIndex = 6;
      // 
      // ultGroupCopy
      // 
      this.ultGroupCopy.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultGroupCopy.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultGroupCopy.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultGroupCopy.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultGroupCopy.DisplayMember = "";
      this.ultGroupCopy.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultGroupCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultGroupCopy.Location = new System.Drawing.Point(107, 4);
      this.ultGroupCopy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultGroupCopy.Name = "ultGroupCopy";
      this.ultGroupCopy.Size = new System.Drawing.Size(354, 21);
      this.ultGroupCopy.TabIndex = 4;
      this.ultGroupCopy.ValueMember = "";
      this.ultGroupCopy.ValueChanged += new System.EventHandler(this.ultGroupCopy_ValueChanged);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 8);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(97, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Group Structure";
      // 
      // frmAddGroup
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(952, 586);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "frmAddGroup";
      this.Text = "Add Group";
      this.Load += new System.EventHandler(this.frmAddGroup_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGrid)).EndInit();
      this.tableLayoutPanel4.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultDDRole)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.tableLayoutPanel6.ResumeLayout(false);
      this.tableLayoutPanel7.ResumeLayout(false);
      this.tableLayoutPanel7.PerformLayout();
      this.tableLayoutPanel5.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.groupBox4.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultGroupCopy)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid;
    private System.Windows.Forms.Button btnAddUser;
    private System.Windows.Forms.Button btnControlCodeMaster;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.RadioButton radUser;
    private System.Windows.Forms.RadioButton radCode;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TreeView tvAuthenticate;
    private System.Windows.Forms.ImageList imageList1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Button btnSaveAuth;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
    private System.Windows.Forms.TextBox txGroupControl;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label label3;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultGroupCopy;
    private System.Windows.Forms.GroupBox groupBox4;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDRole;
  }
}