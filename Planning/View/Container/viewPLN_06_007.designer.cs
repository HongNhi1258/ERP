namespace DaiCo.Planning
{
  partial class viewPLN_06_007
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
      this.btnClose = new System.Windows.Forms.Button();
      this.btnNew = new System.Windows.Forms.Button();
      this.ultContainerList = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.txtContainerId = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.ultCBShipType = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultDatetimeShipFrom = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.ultDatetimeShipTo = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
      this.label5 = new System.Windows.Forms.Label();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClear = new System.Windows.Forms.Button();
      this.btnSearch = new System.Windows.Forms.Button();
      this.ultCBLoadingList = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.btnSwap = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultContainerList)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBShipType)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDatetimeShipFrom)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDatetimeShipTo)).BeginInit();
      this.tableLayoutPanel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBLoadingList)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 7;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 123F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 6, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnNew, 5, 2);
      this.tableLayoutPanel1.Controls.Add(this.ultContainerList, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnSwap, 4, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnDelete, 3, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 109F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(665, 547);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(588, 520);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(74, 24);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnNew
      // 
      this.btnNew.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.Location = new System.Drawing.Point(507, 520);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(74, 24);
      this.btnNew.TabIndex = 2;
      this.btnNew.Text = "New";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // ultContainerList
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultContainerList, 7);
      this.ultContainerList.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultContainerList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultContainerList.Location = new System.Drawing.Point(3, 112);
      this.ultContainerList.Name = "ultContainerList";
      this.ultContainerList.Size = new System.Drawing.Size(659, 402);
      this.ultContainerList.TabIndex = 1;
      this.ultContainerList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultContainerList_MouseDoubleClick);
      this.ultContainerList.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultContainerList_InitializeLayout);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 4;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 7);
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 97F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Controls.Add(this.label2, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtContainerId, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.label4, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.ultCBShipType, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.ultDatetimeShipFrom, 1, 1);
      this.tableLayoutPanel2.Controls.Add(this.ultDatetimeShipTo, 3, 1);
      this.tableLayoutPanel2.Controls.Add(this.label5, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 3, 2);
      this.tableLayoutPanel2.Controls.Add(this.ultCBLoadingList, 1, 2);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 4;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(659, 103);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(345, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(60, 14);
      this.label2.TabIndex = 2;
      this.label2.Text = "Ship Type";
      // 
      // txtContainerId
      // 
      this.txtContainerId.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtContainerId.Location = new System.Drawing.Point(100, 4);
      this.txtContainerId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtContainerId.Name = "txtContainerId";
      this.txtContainerId.Size = new System.Drawing.Size(239, 20);
      this.txtContainerId.TabIndex = 1;
      this.txtContainerId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtContainerId_KeyDown);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(74, 14);
      this.label1.TabIndex = 0;
      this.label1.Text = "Container ID";
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 39);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(87, 14);
      this.label3.TabIndex = 4;
      this.label3.Text = "ShipDate From";
      // 
      // label4
      // 
      this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(345, 39);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(20, 14);
      this.label4.TabIndex = 6;
      this.label4.Text = "To";
      // 
      // ultCBShipType
      // 
      this.ultCBShipType.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultCBShipType.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBShipType.DisplayMember = "";
      this.ultCBShipType.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBShipType.Location = new System.Drawing.Point(417, 4);
      this.ultCBShipType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultCBShipType.Name = "ultCBShipType";
      this.ultCBShipType.Size = new System.Drawing.Size(239, 21);
      this.ultCBShipType.TabIndex = 3;
      this.ultCBShipType.ValueMember = "";
      this.ultCBShipType.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ultCBShipType_KeyDown);
      // 
      // ultDatetimeShipFrom
      // 
      this.ultDatetimeShipFrom.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDatetimeShipFrom.FormatString = "dd/MM/yyyy";
      this.ultDatetimeShipFrom.Location = new System.Drawing.Point(100, 35);
      this.ultDatetimeShipFrom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultDatetimeShipFrom.Name = "ultDatetimeShipFrom";
      this.ultDatetimeShipFrom.Size = new System.Drawing.Size(239, 21);
      this.ultDatetimeShipFrom.TabIndex = 5;
      this.ultDatetimeShipFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ultDatetimeShipFrom_KeyDown);
      // 
      // ultDatetimeShipTo
      // 
      this.ultDatetimeShipTo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultDatetimeShipTo.FormatString = "dd/MM/yyyy";
      this.ultDatetimeShipTo.Location = new System.Drawing.Point(417, 35);
      this.ultDatetimeShipTo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ultDatetimeShipTo.Name = "ultDatetimeShipTo";
      this.ultDatetimeShipTo.Size = new System.Drawing.Size(239, 21);
      this.ultDatetimeShipTo.TabIndex = 7;
      this.ultDatetimeShipTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ultDatetimeShipTo_KeyDown);
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(3, 70);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(75, 14);
      this.label5.TabIndex = 10;
      this.label5.Text = "Loading List";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
      this.tableLayoutPanel3.Controls.Add(this.btnClear, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSearch, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(414, 62);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(245, 31);
      this.tableLayoutPanel3.TabIndex = 9;
      // 
      // btnClear
      // 
      this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClear.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClear.Location = new System.Drawing.Point(105, 3);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(63, 25);
      this.btnClear.TabIndex = 1;
      this.btnClear.Text = "Clear";
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(174, 3);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(68, 25);
      this.btnSearch.TabIndex = 0;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // ultCBLoadingList
      // 
      this.ultCBLoadingList.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultCBLoadingList.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBLoadingList.DisplayMember = "";
      this.ultCBLoadingList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBLoadingList.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBLoadingList.Location = new System.Drawing.Point(100, 65);
      this.ultCBLoadingList.Name = "ultCBLoadingList";
      this.ultCBLoadingList.Size = new System.Drawing.Size(239, 21);
      this.ultCBLoadingList.TabIndex = 11;
      this.ultCBLoadingList.ValueMember = "";
      // 
      // btnSwap
      // 
      this.btnSwap.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSwap.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSwap.Location = new System.Drawing.Point(407, 520);
      this.btnSwap.Name = "btnSwap";
      this.btnSwap.Size = new System.Drawing.Size(94, 24);
      this.btnSwap.TabIndex = 4;
      this.btnSwap.Text = "Swap SO Deducted";
      this.btnSwap.UseVisualStyleBackColor = true;
      this.btnSwap.Click += new System.EventHandler(this.btnSwap_Click);
      // 
      // btnDelete
      // 
      this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnDelete.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDelete.Location = new System.Drawing.Point(328, 520);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(73, 24);
      this.btnDelete.TabIndex = 5;
      this.btnDelete.Text = "Delete";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // viewPLN_06_007
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_06_007";
      this.Size = new System.Drawing.Size(665, 547);
      this.Load += new System.EventHandler(this.viewPLN_06_007_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultContainerList)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBShipType)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDatetimeShipFrom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDatetimeShipTo)).EndInit();
      this.tableLayoutPanel3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultCBLoadingList)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtContainerId;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnNew;
      private Infragistics.Win.UltraWinGrid.UltraGrid ultContainerList;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private Infragistics.Win.UltraWinGrid.UltraCombo ultCBShipType;
      private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultDatetimeShipFrom;
      private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ultDatetimeShipTo;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnSwap;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Label label5;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBLoadingList;
  }
}
