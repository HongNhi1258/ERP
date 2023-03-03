namespace MainBOM.AUTHENTICATE
{
  partial class frmAddUserIntoGroup
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.ultcbGroup = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.ultGridUser = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.ultDDUser = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.tabPage2 = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.ultraCBDepartment = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label3 = new System.Windows.Forms.Label();
      this.ultraCBEmployee = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.btnSearchUser = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.txtUserName = new System.Windows.Forms.TextBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.ultraGridGroup = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSaveTab2 = new System.Windows.Forms.Button();
      this.btnCloseTab2 = new System.Windows.Forms.Button();
      this.ultDDEmpCode = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.tabControl.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultcbGroup)).BeginInit();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultGridUser)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDUser)).BeginInit();
      this.tabPage2.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.tableLayoutPanel5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBDepartment)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBEmployee)).BeginInit();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridGroup)).BeginInit();
      this.tableLayoutPanel6.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDEmpCode)).BeginInit();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.tabPage1);
      this.tabControl.Controls.Add(this.tabPage2);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(800, 502);
      this.tabControl.TabIndex = 0;
      this.tabControl.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Deselecting);
      this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.tableLayoutPanel1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(792, 476);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Group Add User";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(786, 470);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(780, 58);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Group";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel2.Controls.Add(this.btnSearch, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultcbGroup, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 17);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(774, 38);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(696, 3);
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
      // ultcbGroup
      // 
      this.ultcbGroup.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultcbGroup.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultcbGroup.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultcbGroup.DisplayMember = "";
      this.ultcbGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultcbGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultcbGroup.Location = new System.Drawing.Point(89, 4);
      this.ultcbGroup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultcbGroup.Name = "ultcbGroup";
      this.ultcbGroup.Size = new System.Drawing.Size(601, 21);
      this.ultcbGroup.TabIndex = 1;
      this.ultcbGroup.ValueMember = "";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultGridUser);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 67);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(780, 371);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Add User";
      // 
      // ultGridUser
      // 
      this.ultGridUser.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultGridUser.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultGridUser.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultGridUser.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultGridUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultGridUser.Location = new System.Drawing.Point(3, 17);
      this.ultGridUser.Name = "ultGridUser";
      this.ultGridUser.Size = new System.Drawing.Size(774, 351);
      this.ultGridUser.TabIndex = 0;
      this.ultGridUser.AfterRowsDeleted += new System.EventHandler(this.ultGridUser_AfterRowsDeleted);
      this.ultGridUser.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultGridUser_BeforeCellUpdate);
      this.ultGridUser.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultGridUser_InitializeLayout);
      this.ultGridUser.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultGridUser_BeforeRowsDeleted);
      this.ultGridUser.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultGridUser_AfterCellUpdate);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 4;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 516F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.ultDDUser, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.ultDDEmpCode, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 441);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(786, 29);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(655, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(62, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(723, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(60, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // ultDDUser
      // 
      this.ultDDUser.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDUser.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultDDUser.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultDDUser.DisplayMember = "";
      this.ultDDUser.Location = new System.Drawing.Point(3, 3);
      this.ultDDUser.Name = "ultDDUser";
      this.ultDDUser.Size = new System.Drawing.Size(121, 23);
      this.ultDDUser.TabIndex = 0;
      this.ultDDUser.ValueMember = "";
      this.ultDDUser.Visible = false;
      // 
      // tabPage2
      // 
      this.tabPage2.Controls.Add(this.tableLayoutPanel4);
      this.tabPage2.Location = new System.Drawing.Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage2.Size = new System.Drawing.Size(792, 476);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "User Add Group";
      this.tabPage2.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 1;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Controls.Add(this.groupBox3, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.groupBox4, 0, 1);
      this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 0, 2);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 3;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 117F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(786, 470);
      this.tableLayoutPanel4.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.tableLayoutPanel5);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox3.Location = new System.Drawing.Point(3, 3);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(780, 111);
      this.groupBox3.TabIndex = 0;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "User";
      // 
      // tableLayoutPanel5
      // 
      this.tableLayoutPanel5.ColumnCount = 3;
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel5.Controls.Add(this.label2, 0, 0);
      this.tableLayoutPanel5.Controls.Add(this.ultraCBDepartment, 1, 0);
      this.tableLayoutPanel5.Controls.Add(this.label3, 0, 1);
      this.tableLayoutPanel5.Controls.Add(this.ultraCBEmployee, 1, 1);
      this.tableLayoutPanel5.Controls.Add(this.btnSearchUser, 2, 2);
      this.tableLayoutPanel5.Controls.Add(this.label4, 0, 2);
      this.tableLayoutPanel5.Controls.Add(this.txtUserName, 1, 2);
      this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 17);
      this.tableLayoutPanel5.Name = "tableLayoutPanel5";
      this.tableLayoutPanel5.RowCount = 4;
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel5.Size = new System.Drawing.Size(774, 91);
      this.tableLayoutPanel5.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(72, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "Department";
      // 
      // ultraCBDepartment
      // 
      this.ultraCBDepartment.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.tableLayoutPanel5.SetColumnSpan(this.ultraCBDepartment, 2);
      this.ultraCBDepartment.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBDepartment.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraCBDepartment.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBDepartment.DisplayMember = "";
      this.ultraCBDepartment.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraCBDepartment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBDepartment.Location = new System.Drawing.Point(89, 4);
      this.ultraCBDepartment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultraCBDepartment.Name = "ultraCBDepartment";
      this.ultraCBDepartment.Size = new System.Drawing.Size(682, 21);
      this.ultraCBDepartment.TabIndex = 1;
      this.ultraCBDepartment.ValueMember = "";
      this.ultraCBDepartment.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultcbDepartment_InitializeLayout);
      this.ultraCBDepartment.ValueChanged += new System.EventHandler(this.ultraCBDepartment_ValueChanged);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 37);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(61, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Employee";
      // 
      // ultraCBEmployee
      // 
      this.ultraCBEmployee.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.tableLayoutPanel5.SetColumnSpan(this.ultraCBEmployee, 2);
      this.ultraCBEmployee.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBEmployee.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraCBEmployee.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBEmployee.DisplayMember = "";
      this.ultraCBEmployee.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.ultraCBEmployee.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraCBEmployee.Location = new System.Drawing.Point(89, 33);
      this.ultraCBEmployee.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultraCBEmployee.Name = "ultraCBEmployee";
      this.ultraCBEmployee.Size = new System.Drawing.Size(682, 21);
      this.ultraCBEmployee.TabIndex = 3;
      this.ultraCBEmployee.ValueMember = "";
      this.ultraCBEmployee.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultcbEmployee_InitializeLayout);
      // 
      // btnSearchUser
      // 
      this.btnSearchUser.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearchUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearchUser.Location = new System.Drawing.Point(696, 61);
      this.btnSearchUser.Name = "btnSearchUser";
      this.btnSearchUser.Size = new System.Drawing.Size(75, 23);
      this.btnSearchUser.TabIndex = 6;
      this.btnSearchUser.Text = "Search";
      this.btnSearchUser.UseVisualStyleBackColor = true;
      this.btnSearchUser.Click += new System.EventHandler(this.btnSearchUser_Click);
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(3, 66);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(63, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "Username";
      // 
      // txtUserName
      // 
      this.txtUserName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtUserName.Location = new System.Drawing.Point(89, 62);
      this.txtUserName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtUserName.Name = "txtUserName";
      this.txtUserName.Size = new System.Drawing.Size(601, 20);
      this.txtUserName.TabIndex = 5;
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.ultraGridGroup);
      this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox4.Location = new System.Drawing.Point(3, 120);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(780, 318);
      this.groupBox4.TabIndex = 1;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Add Group";
      // 
      // ultraGridGroup
      // 
      this.ultraGridGroup.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraGridGroup.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraGridGroup.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraGridGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraGridGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraGridGroup.Location = new System.Drawing.Point(3, 17);
      this.ultraGridGroup.Name = "ultraGridGroup";
      this.ultraGridGroup.Size = new System.Drawing.Size(774, 298);
      this.ultraGridGroup.TabIndex = 0;
      this.ultraGridGroup.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGridGroup_InitializeLayout);
      this.ultraGridGroup.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraGridGroup_AfterCellUpdate);
      // 
      // tableLayoutPanel6
      // 
      this.tableLayoutPanel6.ColumnCount = 3;
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
      this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
      this.tableLayoutPanel6.Controls.Add(this.btnSaveTab2, 1, 0);
      this.tableLayoutPanel6.Controls.Add(this.btnCloseTab2, 2, 0);
      this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel6.Location = new System.Drawing.Point(0, 441);
      this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel6.Name = "tableLayoutPanel6";
      this.tableLayoutPanel6.RowCount = 2;
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel6.Size = new System.Drawing.Size(786, 29);
      this.tableLayoutPanel6.TabIndex = 2;
      // 
      // btnSaveTab2
      // 
      this.btnSaveTab2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSaveTab2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSaveTab2.Location = new System.Drawing.Point(655, 3);
      this.btnSaveTab2.Name = "btnSaveTab2";
      this.btnSaveTab2.Size = new System.Drawing.Size(62, 23);
      this.btnSaveTab2.TabIndex = 0;
      this.btnSaveTab2.Text = "Save";
      this.btnSaveTab2.UseVisualStyleBackColor = true;
      this.btnSaveTab2.Click += new System.EventHandler(this.btnSaveTab2_Click);
      // 
      // btnCloseTab2
      // 
      this.btnCloseTab2.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnCloseTab2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCloseTab2.Location = new System.Drawing.Point(723, 3);
      this.btnCloseTab2.Name = "btnCloseTab2";
      this.btnCloseTab2.Size = new System.Drawing.Size(60, 23);
      this.btnCloseTab2.TabIndex = 1;
      this.btnCloseTab2.Text = "Close";
      this.btnCloseTab2.UseVisualStyleBackColor = true;
      this.btnCloseTab2.Click += new System.EventHandler(this.btnCloseTab2_Click);
      // 
      // ultDDEmpCode
      // 
      this.ultDDEmpCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDEmpCode.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultDDEmpCode.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultDDEmpCode.DisplayMember = "";
      this.ultDDEmpCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDDEmpCode.Location = new System.Drawing.Point(137, 3);
      this.ultDDEmpCode.Name = "ultDDEmpCode";
      this.ultDDEmpCode.Size = new System.Drawing.Size(117, 23);
      this.ultDDEmpCode.TabIndex = 3;
      this.ultDDEmpCode.ValueMember = "";
      this.ultDDEmpCode.Visible = false;
      // 
      // frmAddUserIntoGroup
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 502);
      this.Controls.Add(this.tabControl);
      this.Name = "frmAddUserIntoGroup";
      this.Text = "Add User Into Group";
      this.Load += new System.EventHandler(this.frmAddUserIntoGroup_Load);
      this.tabControl.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultcbGroup)).EndInit();
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultGridUser)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultDDUser)).EndInit();
      this.tabPage2.ResumeLayout(false);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.tableLayoutPanel5.ResumeLayout(false);
      this.tableLayoutPanel5.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBDepartment)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBEmployee)).EndInit();
      this.groupBox4.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridGroup)).EndInit();
      this.tableLayoutPanel6.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultDDEmpCode)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultGridUser;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultcbGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDUser;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
    private System.Windows.Forms.Button btnSearchUser;
    private System.Windows.Forms.Label label2;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBDepartment;
    private System.Windows.Forms.GroupBox groupBox4;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridGroup;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
    private System.Windows.Forms.Button btnSaveTab2;
    private System.Windows.Forms.Button btnCloseTab2;
    private System.Windows.Forms.Label label3;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBEmployee;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtUserName;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDEmpCode;

  }
}