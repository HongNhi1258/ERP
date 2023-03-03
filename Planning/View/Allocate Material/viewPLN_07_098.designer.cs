namespace DaiCo.Planning
{
  partial class viewPLN_07_098
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
      this.components = new System.ComponentModel.Container();
      this.btnSearch = new System.Windows.Forms.Button();
      this.txtMaterial = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.txtSuppNo = new System.Windows.Forms.TextBox();
      this.label39 = new System.Windows.Forms.Label();
      this.txtWO = new System.Windows.Forms.TextBox();
      this.label37 = new System.Windows.Forms.Label();
      this.btnNew = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.cmbStatus = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.multiCBCreateBy = new DaiCo.Shared.UserControls.MultiColumnComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.drpDateFrom = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.drpDateTo = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpDateFrom)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpDateTo)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnSearch.Location = new System.Drawing.Point(858, 56);
      this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 12;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // txtMaterial
      // 
      this.txtMaterial.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtMaterial.Location = new System.Drawing.Point(545, 30);
      this.txtMaterial.Name = "txtMaterial";
      this.txtMaterial.Size = new System.Drawing.Size(256, 21);
      this.txtMaterial.TabIndex = 7;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label1.Location = new System.Drawing.Point(425, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(43, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Status";
      // 
      // txtSuppNo
      // 
      this.txtSuppNo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtSuppNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtSuppNo.Location = new System.Drawing.Point(113, 3);
      this.txtSuppNo.Name = "txtSuppNo";
      this.txtSuppNo.Size = new System.Drawing.Size(256, 21);
      this.txtSuppNo.TabIndex = 1;
      // 
      // label39
      // 
      this.label39.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label39.AutoSize = true;
      this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label39.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label39.Location = new System.Drawing.Point(3, 7);
      this.label39.Name = "label39";
      this.label39.Size = new System.Drawing.Size(56, 13);
      this.label39.TabIndex = 0;
      this.label39.Text = "Supp.No";
      // 
      // txtWO
      // 
      this.txtWO.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
      this.txtWO.Location = new System.Drawing.Point(113, 30);
      this.txtWO.Name = "txtWO";
      this.txtWO.Size = new System.Drawing.Size(256, 21);
      this.txtWO.TabIndex = 5;
      // 
      // label37
      // 
      this.label37.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label37.AutoSize = true;
      this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label37.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label37.Location = new System.Drawing.Point(3, 34);
      this.label37.Name = "label37";
      this.label37.Size = new System.Drawing.Size(28, 13);
      this.label37.TabIndex = 4;
      this.label37.Text = "WO";
      // 
      // btnNew
      // 
      this.btnNew.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnNew.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnNew.Location = new System.Drawing.Point(305, 3);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(125, 23);
      this.btnNew.TabIndex = 0;
      this.btnNew.Text = "New Supplement";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnClose.Location = new System.Drawing.Point(436, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label2.Location = new System.Drawing.Point(425, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(52, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Material";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label3.Location = new System.Drawing.Point(3, 61);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(62, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Create By";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label4.Location = new System.Drawing.Point(425, 61);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(75, 13);
      this.label4.TabIndex = 10;
      this.label4.Text = "Create Date";
      // 
      // cmbStatus
      // 
      this.cmbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
      this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbStatus.FormattingEnabled = true;
      this.cmbStatus.Location = new System.Drawing.Point(545, 3);
      this.cmbStatus.Name = "cmbStatus";
      this.cmbStatus.Size = new System.Drawing.Size(256, 21);
      this.cmbStatus.TabIndex = 3;
      // 
      // multiCBCreateBy
      // 
      this.multiCBCreateBy.AutoComplete = false;
      this.multiCBCreateBy.AutoDropdown = false;
      this.multiCBCreateBy.BackColorEven = System.Drawing.Color.White;
      this.multiCBCreateBy.BackColorOdd = System.Drawing.Color.White;
      this.multiCBCreateBy.ColumnNames = "";
      this.multiCBCreateBy.ColumnWidthDefault = 75;
      this.multiCBCreateBy.ColumnWidths = "";
      this.multiCBCreateBy.Dock = System.Windows.Forms.DockStyle.Fill;
      this.multiCBCreateBy.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
      this.multiCBCreateBy.FormattingEnabled = true;
      this.multiCBCreateBy.LinkedColumnIndex = 0;
      this.multiCBCreateBy.LinkedTextBox = null;
      this.multiCBCreateBy.Location = new System.Drawing.Point(113, 57);
      this.multiCBCreateBy.Name = "multiCBCreateBy";
      this.multiCBCreateBy.Size = new System.Drawing.Size(256, 21);
      this.multiCBCreateBy.TabIndex = 9;
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(124, 8);
      this.label5.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(14, 13);
      this.label5.TabIndex = 1;
      this.label5.Text = "~";
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 9);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 84);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(930, 409);
      this.ultData.TabIndex = 13;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.DoubleClick += new System.EventHandler(this.ultData_DoubleClick);
      // 
      // drpDateFrom
      // 
      this.drpDateFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.drpDateFrom.Location = new System.Drawing.Point(3, 3);
      this.drpDateFrom.Name = "drpDateFrom";
      this.drpDateFrom.Size = new System.Drawing.Size(115, 21);
      this.drpDateFrom.TabIndex = 0;
      this.drpDateFrom.Value = null;
      // 
      // drpDateTo
      // 
      this.drpDateTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.drpDateTo.Location = new System.Drawing.Point(144, 3);
      this.drpDateTo.Name = "drpDateTo";
      this.drpDateTo.Size = new System.Drawing.Size(115, 21);
      this.drpDateTo.TabIndex = 2;
      this.drpDateTo.Value = null;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 9;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 6, 2);
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 3);
      this.tableLayoutPanel1.Controls.Add(this.label39, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnSearch, 8, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtSuppNo, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 4, 0);
      this.tableLayoutPanel1.Controls.Add(this.cmbStatus, 6, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtWO, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.label4, 4, 2);
      this.tableLayoutPanel1.Controls.Add(this.multiCBCreateBy, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.label37, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 4, 1);
      this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.txtMaterial, 6, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 4, 4);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(936, 525);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.drpDateFrom, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label5, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.drpDateTo, 2, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(542, 54);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(262, 27);
      this.tableLayoutPanel2.TabIndex = 11;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 3;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel3, 5);
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnNew, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(422, 496);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(514, 29);
      this.tableLayoutPanel3.TabIndex = 14;
      // 
      // viewPLN_07_098
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_07_098";
      this.Size = new System.Drawing.Size(936, 525);
      this.Load += new System.EventHandler(this.viewPLN_07_098_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpDateFrom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.drpDateTo)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.TextBox txtMaterial;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtSuppNo;
    private System.Windows.Forms.Label label39;
    private System.Windows.Forms.TextBox txtWO;
    private System.Windows.Forms.Label label37;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private DaiCo.Shared.DaiCoComboBox cmbStatus;
    private DaiCo.Shared.UserControls.MultiColumnComboBox multiCBCreateBy;
    private System.Windows.Forms.Label label5;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor drpDateFrom;
    private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor drpDateTo;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;

  }
}
