namespace DaiCo.Planning
{
  partial class viewPLN_06_008
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
      this.txtContainerID = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.ultContainerDetails = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.txtDescription = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label9 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.ultCBContainerType = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label5 = new System.Windows.Forms.Label();
      this.txtVehicle = new System.Windows.Forms.TextBox();
      this.ultdtShipDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.txtContainerNumber = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.ultCBDistributor = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.txtHardwarePri = new System.Windows.Forms.TextBox();
      this.label12 = new System.Windows.Forms.Label();
      this.ultOriginalShipDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.btnDeliveryContainer = new System.Windows.Forms.Button();
      this.btnChecking = new System.Windows.Forms.Button();
      this.btnWHCBM = new System.Windows.Forms.Button();
      this.drpLoadingList = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.chkShipment = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.ultContainerDetails)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBContainerType)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultdtShipDate)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBDistributor)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultOriginalShipDate)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpLoadingList)).BeginInit();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtContainerID
      // 
      this.txtContainerID.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtContainerID.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtContainerID.Location = new System.Drawing.Point(117, 4);
      this.txtContainerID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtContainerID.MaxLength = 50;
      this.txtContainerID.Name = "txtContainerID";
      this.txtContainerID.Size = new System.Drawing.Size(134, 20);
      this.txtContainerID.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(74, 14);
      this.label1.TabIndex = 0;
      this.label1.Text = "Container ID";
      // 
      // ultContainerDetails
      // 
      this.ultContainerDetails.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultContainerDetails.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultContainerDetails.Location = new System.Drawing.Point(3, 144);
      this.ultContainerDetails.Name = "ultContainerDetails";
      this.ultContainerDetails.Size = new System.Drawing.Size(803, 301);
      this.ultContainerDetails.TabIndex = 2;
      this.ultContainerDetails.AfterRowsDeleted += new System.EventHandler(this.ultContainerDetails_AfterRowsDeleted);
      this.ultContainerDetails.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultContainerDetails_BeforeCellUpdate);
      this.ultContainerDetails.BeforeRowUpdate += new Infragistics.Win.UltraWinGrid.CancelableRowEventHandler(this.ultContainerDetails_BeforeRowUpdate);
      this.ultContainerDetails.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultContainerDetails_InitializeLayout);
      this.ultContainerDetails.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultContainerDetails_BeforeRowsDeleted);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(732, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(68, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(291, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(67, 23);
      this.btnSave.TabIndex = 4;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // txtDescription
      // 
      this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtDescription.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtDescription.Location = new System.Drawing.Point(653, 4);
      this.txtDescription.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtDescription.MaxLength = 512;
      this.txtDescription.Name = "txtDescription";
      this.txtDescription.Size = new System.Drawing.Size(141, 20);
      this.txtDescription.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(539, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(70, 14);
      this.label2.TabIndex = 0;
      this.label2.Text = "Description";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 8;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 114F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
      this.tableLayoutPanel2.Controls.Add(this.label9, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.label8, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label7, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.label3, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultCBContainerType, 5, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtContainerID, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label5, 3, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtVehicle, 5, 1);
      this.tableLayoutPanel2.Controls.Add(this.ultdtShipDate, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.txtDescription, 7, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 6, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtContainerNumber, 7, 1);
      this.tableLayoutPanel2.Controls.Add(this.label6, 6, 1);
      this.tableLayoutPanel2.Controls.Add(this.label4, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultCBDistributor, 2, 2);
      this.tableLayoutPanel2.Controls.Add(this.label10, 1, 2);
      this.tableLayoutPanel2.Controls.Add(this.label11, 3, 2);
      this.tableLayoutPanel2.Controls.Add(this.txtHardwarePri, 5, 2);
      this.tableLayoutPanel2.Controls.Add(this.label12, 6, 2);
      this.tableLayoutPanel2.Controls.Add(this.ultOriginalShipDate, 7, 2);
      this.tableLayoutPanel2.Controls.Add(this.chkShipment, 0, 3);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 4;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(797, 116);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // label9
      // 
      this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.Location = new System.Drawing.Point(3, 65);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(66, 14);
      this.label9.TabIndex = 11;
      this.label9.Text = "Distributor";
      // 
      // label8
      // 
      this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(3, 36);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(78, 14);
      this.label8.TabIndex = 13;
      this.label8.Text = "Loading Date";
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.ForeColor = System.Drawing.Color.Red;
      this.label7.Location = new System.Drawing.Point(87, 7);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(20, 15);
      this.label7.TabIndex = 11;
      this.label7.Text = "(*)";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(257, 7);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(90, 14);
      this.label3.TabIndex = 2;
      this.label3.Text = "Container Type";
      // 
      // ultCBContainerType
      // 
      this.ultCBContainerType.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultCBContainerType.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBContainerType.DisplayMember = "";
      this.ultCBContainerType.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBContainerType.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBContainerType.Location = new System.Drawing.Point(399, 4);
      this.ultCBContainerType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultCBContainerType.Name = "ultCBContainerType";
      this.ultCBContainerType.Size = new System.Drawing.Size(134, 21);
      this.ultCBContainerType.TabIndex = 3;
      this.ultCBContainerType.ValueMember = "";
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(257, 36);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(94, 14);
      this.label5.TabIndex = 6;
      this.label5.Text = "Vehicle Number";
      // 
      // txtVehicle
      // 
      this.txtVehicle.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtVehicle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtVehicle.Location = new System.Drawing.Point(399, 33);
      this.txtVehicle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtVehicle.MaxLength = 50;
      this.txtVehicle.Name = "txtVehicle";
      this.txtVehicle.Size = new System.Drawing.Size(134, 20);
      this.txtVehicle.TabIndex = 8;
      // 
      // ultdtShipDate
      // 
      this.ultdtShipDate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultdtShipDate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultdtShipDate.FormatString = "dd/MM/yyyy";
      this.ultdtShipDate.Location = new System.Drawing.Point(117, 33);
      this.ultdtShipDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultdtShipDate.Name = "ultdtShipDate";
      this.ultdtShipDate.Size = new System.Drawing.Size(134, 21);
      this.ultdtShipDate.TabIndex = 15;
      this.ultdtShipDate.ValueChanged += new System.EventHandler(this.ultdtShipDate_ValueChanged);
      // 
      // txtContainerNumber
      // 
      this.txtContainerNumber.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtContainerNumber.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtContainerNumber.Location = new System.Drawing.Point(653, 33);
      this.txtContainerNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtContainerNumber.MaxLength = 50;
      this.txtContainerNumber.Name = "txtContainerNumber";
      this.txtContainerNumber.Size = new System.Drawing.Size(141, 20);
      this.txtContainerNumber.TabIndex = 9;
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(539, 36);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(108, 14);
      this.label6.TabIndex = 7;
      this.label6.Text = "Container Number";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.ForeColor = System.Drawing.Color.Red;
      this.label4.Location = new System.Drawing.Point(370, 7);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(20, 15);
      this.label4.TabIndex = 12;
      this.label4.Text = "(*)";
      // 
      // ultCBDistributor
      // 
      this.ultCBDistributor.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultCBDistributor.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBDistributor.DisplayMember = "";
      this.ultCBDistributor.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBDistributor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBDistributor.Location = new System.Drawing.Point(117, 62);
      this.ultCBDistributor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultCBDistributor.Name = "ultCBDistributor";
      this.ultCBDistributor.Size = new System.Drawing.Size(134, 21);
      this.ultCBDistributor.TabIndex = 16;
      this.ultCBDistributor.ValueMember = "";
      // 
      // label10
      // 
      this.label10.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.ForeColor = System.Drawing.Color.Red;
      this.label10.Location = new System.Drawing.Point(87, 65);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(20, 15);
      this.label10.TabIndex = 17;
      this.label10.Text = "(*)";
      // 
      // label11
      // 
      this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(257, 65);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(103, 14);
      this.label11.TabIndex = 18;
      this.label11.Text = "Hardware Priority";
      // 
      // txtHardwarePri
      // 
      this.txtHardwarePri.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtHardwarePri.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtHardwarePri.Location = new System.Drawing.Point(399, 61);
      this.txtHardwarePri.Name = "txtHardwarePri";
      this.txtHardwarePri.Size = new System.Drawing.Size(134, 20);
      this.txtHardwarePri.TabIndex = 19;
      // 
      // label12
      // 
      this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label12.AutoSize = true;
      this.label12.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label12.Location = new System.Drawing.Point(539, 58);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(99, 28);
      this.label12.TabIndex = 7;
      this.label12.Text = "Original Loading Date";
      // 
      // ultOriginalShipDate
      // 
      this.ultOriginalShipDate.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultOriginalShipDate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultOriginalShipDate.FormatString = "dd/MM/yyyy";
      this.ultOriginalShipDate.Location = new System.Drawing.Point(653, 61);
      this.ultOriginalShipDate.Name = "ultOriginalShipDate";
      this.ultOriginalShipDate.Size = new System.Drawing.Size(141, 21);
      this.ultOriginalShipDate.TabIndex = 20;
      // 
      // btnDeliveryContainer
      // 
      this.btnDeliveryContainer.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnDeliveryContainer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDeliveryContainer.Location = new System.Drawing.Point(564, 3);
      this.btnDeliveryContainer.Name = "btnDeliveryContainer";
      this.btnDeliveryContainer.Size = new System.Drawing.Size(162, 23);
      this.btnDeliveryContainer.TabIndex = 6;
      this.btnDeliveryContainer.Text = "Report Delivery Container";
      this.btnDeliveryContainer.UseVisualStyleBackColor = true;
      this.btnDeliveryContainer.Click += new System.EventHandler(this.btnDeliveryContainer_Click);
      // 
      // btnChecking
      // 
      this.btnChecking.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnChecking.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnChecking.Location = new System.Drawing.Point(484, 3);
      this.btnChecking.Name = "btnChecking";
      this.btnChecking.Size = new System.Drawing.Size(74, 23);
      this.btnChecking.TabIndex = 7;
      this.btnChecking.Text = "Checking";
      this.btnChecking.UseVisualStyleBackColor = true;
      this.btnChecking.Click += new System.EventHandler(this.btnChecking_Click);
      // 
      // btnWHCBM
      // 
      this.btnWHCBM.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.btnWHCBM.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnWHCBM.Location = new System.Drawing.Point(364, 3);
      this.btnWHCBM.Name = "btnWHCBM";
      this.btnWHCBM.Size = new System.Drawing.Size(113, 23);
      this.btnWHCBM.TabIndex = 8;
      this.btnWHCBM.Text = "WH Confirm CBM";
      this.btnWHCBM.UseVisualStyleBackColor = true;
      this.btnWHCBM.Click += new System.EventHandler(this.btnWHCBM_Click);
      // 
      // drpLoadingList
      // 
      this.drpLoadingList.Cursor = System.Windows.Forms.Cursors.Default;
      this.drpLoadingList.DisplayMember = "";
      this.drpLoadingList.Location = new System.Drawing.Point(206, 3);
      this.drpLoadingList.Name = "drpLoadingList";
      this.drpLoadingList.Size = new System.Drawing.Size(79, 23);
      this.drpLoadingList.TabIndex = 9;
      this.drpLoadingList.ValueMember = "";
      this.drpLoadingList.Visible = false;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 7;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 85F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 119F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 168F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
      this.tableLayoutPanel4.Controls.Add(this.btnClose, 6, 0);
      this.tableLayoutPanel4.Controls.Add(this.drpLoadingList, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnDeliveryContainer, 5, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnChecking, 4, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnWHCBM, 3, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSave, 2, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 451);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(803, 30);
      this.tableLayoutPanel4.TabIndex = 10;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel4, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.ultContainerDetails, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 141F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(809, 484);
      this.tableLayoutPanel1.TabIndex = 11;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel2);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(803, 135);
      this.groupBox1.TabIndex = 12;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Master Information";
      // 
      // chkShipment
      // 
      this.chkShipment.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkShipment.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.chkShipment, 3);
      this.chkShipment.Location = new System.Drawing.Point(3, 92);
      this.chkShipment.Name = "chkShipment";
      this.chkShipment.Size = new System.Drawing.Size(137, 18);
      this.chkShipment.TabIndex = 21;
      this.chkShipment.Text = "Shipment Ex Factory";
      this.chkShipment.UseVisualStyleBackColor = true;
      // 
      // viewPLN_06_008
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_06_008";
      this.Size = new System.Drawing.Size(809, 484);
      this.Load += new System.EventHandler(this.viewPLN_06_008_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultContainerDetails)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBContainerType)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultdtShipDate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBDistributor)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultOriginalShipDate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpLoadingList)).EndInit();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox txtContainerID;
    private System.Windows.Forms.Label label1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultContainerDetails;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TextBox txtDescription;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label3;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBContainerType;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtVehicle;
    private System.Windows.Forms.TextBox txtContainerNumber;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label8;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultdtShipDate;
    private System.Windows.Forms.Button btnDeliveryContainer;
    private System.Windows.Forms.Button btnChecking;
    private System.Windows.Forms.Button btnWHCBM;
    private Infragistics.Win.UltraWinGrid.UltraDropDown drpLoadingList;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Label label9;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBDistributor;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.TextBox txtHardwarePri;
    private System.Windows.Forms.Label label12;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultOriginalShipDate;
    private System.Windows.Forms.CheckBox chkShipment;
  }
}
