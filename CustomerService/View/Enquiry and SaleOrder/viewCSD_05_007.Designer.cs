namespace DaiCo.CustomerService
{
  partial class viewCSD_05_007
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
      this.btnSave = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.dt_OrderDateTo = new DaiCo.Shared.UserControls.uc_DateTimePicker();
      this.txtEnquiryFrom = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.dt_OrderDateFrom = new DaiCo.Shared.UserControls.uc_DateTimePicker();
      this.btnSearch = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.txtEnquiryTo = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.ultraGridEnquiryDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.chkSelectedAll = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridEnquiryDetail)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(866, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 3;
      this.btnSave.Text = "Add";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 7);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(65, 14);
      this.label3.TabIndex = 6;
      this.label3.Text = "Enquiry No";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 34);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(1018, 84);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Search information";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 9;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.Controls.Add(this.dt_OrderDateTo, 8, 0);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtEnquiryFrom, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.label7, 7, 0);
      this.tableLayoutPanel1.Controls.Add(this.dt_OrderDateFrom, 6, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 8, 1);
      this.tableLayoutPanel1.Controls.Add(this.label6, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtEnquiryTo, 3, 0);
      this.tableLayoutPanel1.Controls.Add(this.label4, 5, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 17);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1012, 64);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // dt_OrderDateTo
      // 
      this.dt_OrderDateTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dt_OrderDateTo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dt_OrderDateTo.Location = new System.Drawing.Point(821, 3);
      this.dt_OrderDateTo.Name = "dt_OrderDateTo";
      this.dt_OrderDateTo.Size = new System.Drawing.Size(188, 23);
      this.dt_OrderDateTo.TabIndex = 3;
      this.dt_OrderDateTo.Value = new System.DateTime(2010, 6, 23, 10, 50, 29, 802);
      // 
      // txtEnquiryFrom
      // 
      this.txtEnquiryFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtEnquiryFrom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtEnquiryFrom.Location = new System.Drawing.Point(82, 3);
      this.txtEnquiryFrom.Name = "txtEnquiryFrom";
      this.txtEnquiryFrom.Size = new System.Drawing.Size(186, 20);
      this.txtEnquiryFrom.TabIndex = 0;
      // 
      // label7
      // 
      this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(801, 7);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(13, 14);
      this.label7.TabIndex = 28;
      this.label7.Text = "~";
      // 
      // dt_OrderDateFrom
      // 
      this.dt_OrderDateFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dt_OrderDateFrom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dt_OrderDateFrom.Location = new System.Drawing.Point(609, 3);
      this.dt_OrderDateFrom.Name = "dt_OrderDateFrom";
      this.dt_OrderDateFrom.Size = new System.Drawing.Size(186, 23);
      this.dt_OrderDateFrom.TabIndex = 2;
      this.dt_OrderDateFrom.Value = new System.DateTime(2010, 6, 23, 10, 50, 29, 802);
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSearch.Location = new System.Drawing.Point(934, 32);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 4;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // label6
      // 
      this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(274, 7);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(13, 14);
      this.label6.TabIndex = 25;
      this.label6.Text = "~";
      // 
      // txtEnquiryTo
      // 
      this.txtEnquiryTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtEnquiryTo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtEnquiryTo.Location = new System.Drawing.Point(294, 3);
      this.txtEnquiryTo.Name = "txtEnquiryTo";
      this.txtEnquiryTo.Size = new System.Drawing.Size(186, 20);
      this.txtEnquiryTo.TabIndex = 1;
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(526, 7);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(75, 14);
      this.label4.TabIndex = 19;
      this.label4.Text = "Enquiry Date";
      // 
      // ultraGridEnquiryDetail
      // 
      this.ultraGridEnquiryDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraGridEnquiryDetail.DisplayLayout.AutoFitColumns = true;
      this.ultraGridEnquiryDetail.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      this.ultraGridEnquiryDetail.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      this.ultraGridEnquiryDetail.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      this.ultraGridEnquiryDetail.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultraGridEnquiryDetail.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraGridEnquiryDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraGridEnquiryDetail.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraGridEnquiryDetail.Location = new System.Drawing.Point(3, 17);
      this.ultraGridEnquiryDetail.Name = "ultraGridEnquiryDetail";
      this.ultraGridEnquiryDetail.Size = new System.Drawing.Size(1012, 419);
      this.ultraGridEnquiryDetail.TabIndex = 2;
      this.ultraGridEnquiryDetail.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultraGridEnquiryDetail_BeforeCellUpdate);
      this.ultraGridEnquiryDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraGridEnquiryDetail_InitializeLayout);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(947, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(74, 23);
      this.btnClose.TabIndex = 4;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // chkSelectedAll
      // 
      this.chkSelectedAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.chkSelectedAll.AutoSize = true;
      this.chkSelectedAll.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkSelectedAll.Location = new System.Drawing.Point(901, 0);
      this.chkSelectedAll.Name = "chkSelectedAll";
      this.chkSelectedAll.Size = new System.Drawing.Size(91, 18);
      this.chkSelectedAll.TabIndex = 1;
      this.chkSelectedAll.Text = "Selected All";
      this.chkSelectedAll.UseVisualStyleBackColor = true;
      this.chkSelectedAll.CheckedChanged += new System.EventHandler(this.chkSelectedAll_CheckedChanged);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.groupBox2, 0, 2);
      this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 0, 3);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 4;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(1024, 595);
      this.tableLayoutPanel3.TabIndex = 11;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.Red;
      this.label1.Location = new System.Drawing.Point(388, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(247, 24);
      this.label1.TabIndex = 1;
      this.label1.Text = "Add Items For Sale Order";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.ultraGridEnquiryDetail);
      this.groupBox2.Controls.Add(this.chkSelectedAll);
      this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox2.Location = new System.Drawing.Point(3, 124);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(1018, 439);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Detail";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.Controls.Add(this.btnSave, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 566);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(1024, 29);
      this.tableLayoutPanel2.TabIndex = 3;
      // 
      // viewCSD_05_007
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel3);
      this.Name = "viewCSD_05_007";
      this.Size = new System.Drawing.Size(1024, 595);
      this.Load += new System.EventHandler(this.viewCSD_05_007_Load);
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraGridEnquiryDetail)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnSearch;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultraGridEnquiryDetail;
    private System.Windows.Forms.TextBox txtEnquiryTo;
    private System.Windows.Forms.TextBox txtEnquiryFrom;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button btnClose;
    private Shared.UserControls.uc_DateTimePicker dt_OrderDateTo;
    private Shared.UserControls.uc_DateTimePicker dt_OrderDateFrom;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.CheckBox chkSelectedAll;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
  }
}