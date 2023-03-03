namespace DaiCo.FinishGoodWarehouse
{
  partial class viewFGH_03_006
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
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.ultOutStoreDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.btnGetTemplate = new System.Windows.Forms.Button();
      this.btnAddDetail = new System.Windows.Forms.Button();
      this.btnAddDetailFromFile = new System.Windows.Forms.Button();
      this.btnDeleteItem = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txtIssuingCode = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.txtNote = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.chkConfirm = new System.Windows.Forms.CheckBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnPrint = new System.Windows.Forms.Button();
      this.btnPrintDetail = new System.Windows.Forms.Button();
      this.grpSummary = new System.Windows.Forms.GroupBox();
      this.ultSummary = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultOutStoreDetail)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.grpSummary.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultSummary)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.grpSummary, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 71F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(763, 493);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.ultOutStoreDetail);
      this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox3.Location = new System.Drawing.Point(3, 230);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(757, 224);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      // 
      // ultOutStoreDetail
      // 
      this.ultOutStoreDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultOutStoreDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultOutStoreDetail.Location = new System.Drawing.Point(3, 16);
      this.ultOutStoreDetail.Name = "ultOutStoreDetail";
      this.ultOutStoreDetail.Size = new System.Drawing.Size(751, 205);
      this.ultOutStoreDetail.TabIndex = 0;
      this.ultOutStoreDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultInStoreDetail_InitializeLayout);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 7;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 79F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 2, 1);
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtIssuingCode, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.label5, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtNote, 6, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(757, 65);
      this.tableLayoutPanel2.TabIndex = 0;
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 5;
      this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel4, 5);
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 143F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
      this.tableLayoutPanel4.Controls.Add(this.btnGetTemplate, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnAddDetail, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnAddDetailFromFile, 3, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnDeleteItem, 4, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(110, 32);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(644, 30);
      this.tableLayoutPanel4.TabIndex = 2;
      // 
      // btnGetTemplate
      // 
      this.btnGetTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnGetTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnGetTemplate.Location = new System.Drawing.Point(216, 3);
      this.btnGetTemplate.Name = "btnGetTemplate";
      this.btnGetTemplate.Size = new System.Drawing.Size(95, 23);
      this.btnGetTemplate.TabIndex = 0;
      this.btnGetTemplate.Text = "Get Template";
      this.btnGetTemplate.UseVisualStyleBackColor = true;
      this.btnGetTemplate.Click += new System.EventHandler(this.btnGetTemplate_Click);
      // 
      // btnAddDetail
      // 
      this.btnAddDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAddDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAddDetail.Location = new System.Drawing.Point(317, 3);
      this.btnAddDetail.Name = "btnAddDetail";
      this.btnAddDetail.Size = new System.Drawing.Size(94, 23);
      this.btnAddDetail.TabIndex = 1;
      this.btnAddDetail.Text = "Add Detail";
      this.btnAddDetail.UseVisualStyleBackColor = true;
      this.btnAddDetail.Click += new System.EventHandler(this.btnAddDetail_Click);
      // 
      // btnAddDetailFromFile
      // 
      this.btnAddDetailFromFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAddDetailFromFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAddDetailFromFile.Location = new System.Drawing.Point(418, 3);
      this.btnAddDetailFromFile.Name = "btnAddDetailFromFile";
      this.btnAddDetailFromFile.Size = new System.Drawing.Size(136, 23);
      this.btnAddDetailFromFile.TabIndex = 2;
      this.btnAddDetailFromFile.Text = "Add Detail From File";
      this.btnAddDetailFromFile.UseVisualStyleBackColor = true;
      this.btnAddDetailFromFile.Click += new System.EventHandler(this.btnAddDetailFromFile_Click);
      // 
      // btnDeleteItem
      // 
      this.btnDeleteItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDeleteItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDeleteItem.Location = new System.Drawing.Point(560, 3);
      this.btnDeleteItem.Name = "btnDeleteItem";
      this.btnDeleteItem.Size = new System.Drawing.Size(81, 23);
      this.btnDeleteItem.TabIndex = 3;
      this.btnDeleteItem.Text = "Delete Item";
      this.btnDeleteItem.UseVisualStyleBackColor = true;
      this.btnDeleteItem.Click += new System.EventHandler(this.btnDeleteItem_Click);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(60, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "ISS Code";
      // 
      // txtIssuingCode
      // 
      this.txtIssuingCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtIssuingCode.Location = new System.Drawing.Point(110, 4);
      this.txtIssuingCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtIssuingCode.Name = "txtIssuingCode";
      this.txtIssuingCode.ReadOnly = true;
      this.txtIssuingCode.Size = new System.Drawing.Size(285, 20);
      this.txtIssuingCode.TabIndex = 0;
      // 
      // label5
      // 
      this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(421, 8);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(34, 13);
      this.label5.TabIndex = 15;
      this.label5.Text = "Note";
      // 
      // txtNote
      // 
      this.txtNote.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtNote.Location = new System.Drawing.Point(469, 4);
      this.txtNote.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
      this.txtNote.Name = "txtNote";
      this.txtNote.Size = new System.Drawing.Size(285, 20);
      this.txtNote.TabIndex = 1;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 7;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 129F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 6, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnPrint, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnPrintDetail, 5, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.chkConfirm, 2, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 460);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(757, 30);
      this.tableLayoutPanel3.TabIndex = 3;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(679, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 5;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // chkConfirm
      // 
      this.chkConfirm.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.chkConfirm.AutoSize = true;
      this.chkConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkConfirm.Location = new System.Drawing.Point(357, 6);
      this.chkConfirm.Name = "chkConfirm";
      this.chkConfirm.Size = new System.Drawing.Size(68, 17);
      this.chkConfirm.TabIndex = 0;
      this.chkConfirm.Text = "Confirm";
      this.chkConfirm.UseVisualStyleBackColor = true;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(431, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(72, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnPrint
      // 
      this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrint.Location = new System.Drawing.Point(509, 3);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(74, 23);
      this.btnPrint.TabIndex = 3;
      this.btnPrint.Text = "Print";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // btnPrintDetail
      // 
      this.btnPrintDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnPrintDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrintDetail.Location = new System.Drawing.Point(589, 3);
      this.btnPrintDetail.Name = "btnPrintDetail";
      this.btnPrintDetail.Size = new System.Drawing.Size(84, 23);
      this.btnPrintDetail.TabIndex = 4;
      this.btnPrintDetail.Text = "Print Detail";
      this.btnPrintDetail.UseVisualStyleBackColor = true;
      this.btnPrintDetail.Click += new System.EventHandler(this.btnPrintDetail_Click);
      // 
      // grpSummary
      // 
      this.grpSummary.Controls.Add(this.ultSummary);
      this.grpSummary.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpSummary.Location = new System.Drawing.Point(3, 74);
      this.grpSummary.Name = "grpSummary";
      this.grpSummary.Size = new System.Drawing.Size(757, 150);
      this.grpSummary.TabIndex = 1;
      this.grpSummary.TabStop = false;
      // 
      // ultSummary
      // 
      this.ultSummary.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultSummary.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultSummary.Location = new System.Drawing.Point(3, 16);
      this.ultSummary.Name = "ultSummary";
      this.ultSummary.Size = new System.Drawing.Size(751, 131);
      this.ultSummary.TabIndex = 0;
      this.ultSummary.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultSummary_InitializeLayout);
      // 
      // viewFGH_03_006
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewFGH_03_006";
      this.Size = new System.Drawing.Size(763, 493);
      this.Load += new System.EventHandler(this.viewFGH_03_001_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultOutStoreDetail)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.grpSummary.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultSummary)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.CheckBox chkConfirm;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtIssuingCode;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultOutStoreDetail;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox txtNote;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Button btnPrintDetail;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private System.Windows.Forms.Button btnGetTemplate;
    private System.Windows.Forms.Button btnAddDetail;
    private System.Windows.Forms.Button btnAddDetailFromFile;
    private System.Windows.Forms.Button btnDeleteItem;
    private System.Windows.Forms.GroupBox grpSummary;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultSummary;
  }
}
