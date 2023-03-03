namespace DaiCo.Purchasing
{
  partial class viewPUR_21_009
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.chkSelectAll = new System.Windows.Forms.CheckBox();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.grpSearch = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.ucCarcassCode = new DaiCo.Shared.ucUltraList();
      this.txtCarcassCode = new System.Windows.Forms.TextBox();
      this.btnAdd = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.chkItemCode = new System.Windows.Forms.CheckBox();
      this.chkCarcassCode = new System.Windows.Forms.CheckBox();
      this.txtItemCode = new System.Windows.Forms.TextBox();
      this.ucItemCode = new DaiCo.Shared.ucUltraList();
      this.ultCBWO = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.btnCopy = new System.Windows.Forms.Button();
      this.ultDDRequestDateCopy = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.label5 = new System.Windows.Forms.Label();
      this.ultCBUrgentCopy = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label4 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.txtPurposeOfRequisition = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.ultDDUrgent = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.ultCBSupplier = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.grpSearch.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBWO)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDRequestDateCopy)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBUrgentCopy)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDUrgent)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBSupplier)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.grpSearch, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1161, 552);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.chkSelectAll);
      this.groupBox1.Controls.Add(this.ultData);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 427);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(1155, 91);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Information";
      // 
      // chkSelectAll
      // 
      this.chkSelectAll.AutoSize = true;
      this.chkSelectAll.Location = new System.Drawing.Point(86, -1);
      this.chkSelectAll.Name = "chkSelectAll";
      this.chkSelectAll.Size = new System.Drawing.Size(80, 17);
      this.chkSelectAll.TabIndex = 1;
      this.chkSelectAll.Text = "Select All";
      this.chkSelectAll.UseVisualStyleBackColor = true;
      this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.AutoFitColumns = true;
      this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 16);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(1149, 72);
      this.ultData.TabIndex = 0;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // grpSearch
      // 
      this.grpSearch.AutoSize = true;
      this.grpSearch.Controls.Add(this.tableLayoutPanel4);
      this.grpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpSearch.Location = new System.Drawing.Point(3, 3);
      this.grpSearch.Name = "grpSearch";
      this.grpSearch.Size = new System.Drawing.Size(1155, 418);
      this.grpSearch.TabIndex = 3;
      this.grpSearch.TabStop = false;
      this.grpSearch.Text = "Search Information";
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.AutoSize = true;
      this.tableLayoutPanel4.ColumnCount = 12;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 119F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 123F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 205F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
      this.tableLayoutPanel4.Controls.Add(this.ucCarcassCode, 2, 2);
      this.tableLayoutPanel4.Controls.Add(this.txtCarcassCode, 2, 1);
      this.tableLayoutPanel4.Controls.Add(this.btnAdd, 11, 6);
      this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.label2, 0, 1);
      this.tableLayoutPanel4.Controls.Add(this.label3, 0, 3);
      this.tableLayoutPanel4.Controls.Add(this.chkItemCode, 1, 3);
      this.tableLayoutPanel4.Controls.Add(this.chkCarcassCode, 1, 1);
      this.tableLayoutPanel4.Controls.Add(this.txtItemCode, 2, 3);
      this.tableLayoutPanel4.Controls.Add(this.ucItemCode, 2, 4);
      this.tableLayoutPanel4.Controls.Add(this.ultCBWO, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnCopy, 9, 6);
      this.tableLayoutPanel4.Controls.Add(this.ultDDRequestDateCopy, 6, 6);
      this.tableLayoutPanel4.Controls.Add(this.label5, 5, 6);
      this.tableLayoutPanel4.Controls.Add(this.ultCBUrgentCopy, 4, 6);
      this.tableLayoutPanel4.Controls.Add(this.label6, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.label7, 7, 6);
      this.tableLayoutPanel4.Controls.Add(this.txtPurposeOfRequisition, 8, 6);
      this.tableLayoutPanel4.Controls.Add(this.label8, 0, 5);
      this.tableLayoutPanel4.Controls.Add(this.label9, 1, 5);
      this.tableLayoutPanel4.Controls.Add(this.ultCBSupplier, 2, 5);
      this.tableLayoutPanel4.Controls.Add(this.label4, 3, 6);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 7;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(1149, 399);
      this.tableLayoutPanel4.TabIndex = 4;
      // 
      // ucCarcassCode
      // 
      this.ucCarcassCode.AutoSearchBy = "";
      this.tableLayoutPanel4.SetColumnSpan(this.ucCarcassCode, 10);
      this.ucCarcassCode.ColumnWidths = "";
      this.ucCarcassCode.DataSource = null;
      this.ucCarcassCode.DisplayMember = "";
      this.ucCarcassCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucCarcassCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucCarcassCode.Location = new System.Drawing.Point(130, 56);
      this.ucCarcassCode.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.ucCarcassCode.Name = "ucCarcassCode";
      this.ucCarcassCode.SelectedText = "";
      this.ucCarcassCode.SelectedValue = "";
      this.ucCarcassCode.Separator = '\0';
      this.ucCarcassCode.Size = new System.Drawing.Size(1019, 127);
      this.ucCarcassCode.TabIndex = 1;
      this.ucCarcassCode.Text = "ucUltraList2";
      this.ucCarcassCode.ValueMember = "";
      this.ucCarcassCode.Visible = false;
      this.ucCarcassCode.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucCarcassCode_ValueChanged);
      // 
      // txtCarcassCode
      // 
      this.tableLayoutPanel4.SetColumnSpan(this.txtCarcassCode, 10);
      this.txtCarcassCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtCarcassCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtCarcassCode.Location = new System.Drawing.Point(133, 30);
      this.txtCarcassCode.Name = "txtCarcassCode";
      this.txtCarcassCode.ReadOnly = true;
      this.txtCarcassCode.Size = new System.Drawing.Size(1013, 20);
      this.txtCarcassCode.TabIndex = 8;
      // 
      // btnAdd
      // 
      this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAdd.Location = new System.Drawing.Point(1071, 374);
      this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(75, 23);
      this.btnAdd.TabIndex = 3;
      this.btnAdd.Text = "Add";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(28, 13);
      this.label1.TabIndex = 47;
      this.label1.Text = "WO";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 33);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(81, 13);
      this.label2.TabIndex = 48;
      this.label2.Text = "CarcassCode";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 192);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(60, 13);
      this.label3.TabIndex = 49;
      this.label3.Text = "ItemCode";
      // 
      // chkItemCode
      // 
      this.chkItemCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkItemCode.AutoSize = true;
      this.chkItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkItemCode.Location = new System.Drawing.Point(103, 192);
      this.chkItemCode.Name = "chkItemCode";
      this.chkItemCode.Size = new System.Drawing.Size(15, 14);
      this.chkItemCode.TabIndex = 7;
      this.chkItemCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.chkItemCode.UseVisualStyleBackColor = true;
      this.chkItemCode.CheckedChanged += new System.EventHandler(this.chkItemCode_CheckedChanged);
      // 
      // chkCarcassCode
      // 
      this.chkCarcassCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkCarcassCode.AutoSize = true;
      this.chkCarcassCode.Location = new System.Drawing.Point(103, 31);
      this.chkCarcassCode.Name = "chkCarcassCode";
      this.chkCarcassCode.Size = new System.Drawing.Size(24, 17);
      this.chkCarcassCode.TabIndex = 50;
      this.chkCarcassCode.Text = "checkBox1";
      this.chkCarcassCode.UseVisualStyleBackColor = true;
      this.chkCarcassCode.CheckedChanged += new System.EventHandler(this.chkCarcassCode_CheckedChanged);
      // 
      // txtItemCode
      // 
      this.tableLayoutPanel4.SetColumnSpan(this.txtItemCode, 10);
      this.txtItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtItemCode.Location = new System.Drawing.Point(133, 189);
      this.txtItemCode.Name = "txtItemCode";
      this.txtItemCode.ReadOnly = true;
      this.txtItemCode.Size = new System.Drawing.Size(1013, 20);
      this.txtItemCode.TabIndex = 51;
      // 
      // ucItemCode
      // 
      this.ucItemCode.AutoSearchBy = "";
      this.tableLayoutPanel4.SetColumnSpan(this.ucItemCode, 10);
      this.ucItemCode.ColumnWidths = "";
      this.ucItemCode.DataSource = null;
      this.ucItemCode.DisplayMember = "";
      this.ucItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucItemCode.Location = new System.Drawing.Point(130, 215);
      this.ucItemCode.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.ucItemCode.Name = "ucItemCode";
      this.ucItemCode.SelectedText = "";
      this.ucItemCode.SelectedValue = "";
      this.ucItemCode.Separator = '\0';
      this.ucItemCode.Size = new System.Drawing.Size(1019, 127);
      this.ucItemCode.TabIndex = 2;
      this.ucItemCode.Text = "ucUltraList2";
      this.ucItemCode.ValueMember = "";
      this.ucItemCode.Visible = false;
      this.ucItemCode.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucItemCode_ValueChanged);
      // 
      // ultCBWO
      // 
      this.ultCBWO.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.tableLayoutPanel4.SetColumnSpan(this.ultCBWO, 10);
      this.ultCBWO.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBWO.DisplayMember = "";
      this.ultCBWO.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBWO.Location = new System.Drawing.Point(133, 3);
      this.ultCBWO.Name = "ultCBWO";
      this.ultCBWO.Size = new System.Drawing.Size(1013, 21);
      this.ultCBWO.TabIndex = 0;
      this.ultCBWO.ValueMember = "";
      this.ultCBWO.ValueChanged += new System.EventHandler(this.ultCBWO_ValueChanged);
      // 
      // btnCopy
      // 
      this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCopy.Location = new System.Drawing.Point(840, 374);
      this.btnCopy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.btnCopy.Name = "btnCopy";
      this.btnCopy.Size = new System.Drawing.Size(73, 23);
      this.btnCopy.TabIndex = 7;
      this.btnCopy.Text = "Copy";
      this.btnCopy.UseVisualStyleBackColor = true;
      this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
      // 
      // ultDDRequestDateCopy
      // 
      this.ultDDRequestDateCopy.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDDRequestDateCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDDRequestDateCopy.Location = new System.Drawing.Point(411, 375);
      this.ultDDRequestDateCopy.Name = "ultDDRequestDateCopy";
      this.ultDDRequestDateCopy.Size = new System.Drawing.Size(117, 21);
      this.ultDDRequestDateCopy.TabIndex = 5;
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(324, 379);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(81, 13);
      this.label5.TabIndex = 1;
      this.label5.Text = "RequestDate";
      // 
      // ultCBUrgentCopy
      // 
      this.ultCBUrgentCopy.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultCBUrgentCopy.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBUrgentCopy.DisplayMember = "";
      this.ultCBUrgentCopy.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBUrgentCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBUrgentCopy.Location = new System.Drawing.Point(205, 375);
      this.ultCBUrgentCopy.Name = "ultCBUrgentCopy";
      this.ultCBUrgentCopy.Size = new System.Drawing.Size(113, 21);
      this.ultCBUrgentCopy.TabIndex = 4;
      this.ultCBUrgentCopy.ValueMember = "";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(149, 379);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(45, 13);
      this.label4.TabIndex = 0;
      this.label4.Text = "Urgent";
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.ForeColor = System.Drawing.Color.Red;
      this.label6.Location = new System.Drawing.Point(103, 7);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(20, 13);
      this.label6.TabIndex = 54;
      this.label6.Text = "(*)";
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(534, 379);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(91, 13);
      this.label7.TabIndex = 55;
      this.label7.Text = "Purpose Of PR";
      // 
      // txtPurposeOfRequisition
      // 
      this.txtPurposeOfRequisition.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtPurposeOfRequisition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtPurposeOfRequisition.Location = new System.Drawing.Point(635, 375);
      this.txtPurposeOfRequisition.Name = "txtPurposeOfRequisition";
      this.txtPurposeOfRequisition.Size = new System.Drawing.Size(199, 20);
      this.txtPurposeOfRequisition.TabIndex = 6;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 7;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 6, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 5, 0);
      this.tableLayoutPanel3.Controls.Add(this.ultDDUrgent, 0, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 524);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(1155, 25);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(1077, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(996, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 0;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // ultDDUrgent
      // 
      this.ultDDUrgent.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDUrgent.DisplayMember = "";
      this.ultDDUrgent.Location = new System.Drawing.Point(3, 3);
      this.ultDDUrgent.Name = "ultDDUrgent";
      this.ultDDUrgent.Size = new System.Drawing.Size(44, 23);
      this.ultDDUrgent.TabIndex = 2;
      this.ultDDUrgent.Text = "ultraDropDown1";
      this.ultDDUrgent.ValueMember = "";
      this.ultDDUrgent.Visible = false;
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(3, 352);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(53, 13);
      this.label8.TabIndex = 56;
      this.label8.Text = "Supplier";
      // 
      // label9
      // 
      this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label9.AutoSize = true;
      this.label9.ForeColor = System.Drawing.Color.Red;
      this.label9.Location = new System.Drawing.Point(103, 352);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(20, 13);
      this.label9.TabIndex = 57;
      this.label9.Text = "(*)";
      // 
      // ultCBSupplier
      // 
      this.ultCBSupplier.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.tableLayoutPanel4.SetColumnSpan(this.ultCBSupplier, 10);
      this.ultCBSupplier.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBSupplier.DisplayMember = "";
      this.ultCBSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBSupplier.Location = new System.Drawing.Point(133, 348);
      this.ultCBSupplier.Name = "ultCBSupplier";
      this.ultCBSupplier.Size = new System.Drawing.Size(1013, 21);
      this.ultCBSupplier.TabIndex = 58;
      this.ultCBSupplier.ValueMember = "";
      // 
      // viewPUR_21_009
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPUR_21_009";
      this.Size = new System.Drawing.Size(1161, 552);
      this.Load += new System.EventHandler(this.viewPUR_21_009_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.grpSearch.ResumeLayout(false);
      this.grpSearch.PerformLayout();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBWO)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDRequestDateCopy)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBUrgentCopy)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultDDUrgent)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBSupplier)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.GroupBox grpSearch;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.CheckBox chkItemCode;
    private System.Windows.Forms.TextBox txtCarcassCode;
    private DaiCo.Shared.ucUltraList ucCarcassCode;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox chkCarcassCode;
    private System.Windows.Forms.TextBox txtItemCode;
    private DaiCo.Shared.ucUltraList ucItemCode;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBWO;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDUrgent;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultDDRequestDateCopy;
    private System.Windows.Forms.Button btnCopy;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBUrgentCopy;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.CheckBox chkSelectAll;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox txtPurposeOfRequisition;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBSupplier;
  }
}
