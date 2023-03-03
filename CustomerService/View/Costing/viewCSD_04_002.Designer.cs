namespace DaiCo.CustomerService
{
  partial class viewCSD_04_002
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
      this.label1 = new System.Windows.Forms.Label();
      this.grpInfo = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txtDescription = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.cmbPriceBaseOn = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.ultDetail = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.udrItemCode = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.btnSave = new System.Windows.Forms.Button();
      this.chkConfirm = new System.Windows.Forms.CheckBox();
      this.grpInfo.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.udrItemCode)).BeginInit();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 27);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(88, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Price Base On";
      // 
      // grpInfo
      // 
      this.grpInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grpInfo.Controls.Add(this.label4);
      this.grpInfo.Controls.Add(this.txtDescription);
      this.grpInfo.Controls.Add(this.label3);
      this.grpInfo.Controls.Add(this.cmbPriceBaseOn);
      this.grpInfo.Controls.Add(this.label2);
      this.grpInfo.Controls.Add(this.label1);
      this.grpInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpInfo.Location = new System.Drawing.Point(3, 3);
      this.grpInfo.Name = "grpInfo";
      this.grpInfo.Size = new System.Drawing.Size(479, 79);
      this.grpInfo.TabIndex = 1;
      this.grpInfo.TabStop = false;
      this.grpInfo.Text = "Information";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.ForeColor = System.Drawing.Color.Red;
      this.label4.Location = new System.Drawing.Point(100, 51);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(23, 15);
      this.label4.TabIndex = 5;
      this.label4.Text = "(*)";
      // 
      // txtDescription
      // 
      this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtDescription.Location = new System.Drawing.Point(126, 51);
      this.txtDescription.Name = "txtDescription";
      this.txtDescription.Size = new System.Drawing.Size(345, 20);
      this.txtDescription.TabIndex = 4;
      this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 53);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(71, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Description";
      // 
      // cmbPriceBaseOn
      // 
      this.cmbPriceBaseOn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.cmbPriceBaseOn.FormattingEnabled = true;
      this.cmbPriceBaseOn.Location = new System.Drawing.Point(126, 24);
      this.cmbPriceBaseOn.Name = "cmbPriceBaseOn";
      this.cmbPriceBaseOn.Size = new System.Drawing.Size(345, 21);
      this.cmbPriceBaseOn.TabIndex = 2;
      this.cmbPriceBaseOn.SelectedIndexChanged += new System.EventHandler(this.cmbPriceBaseOn_SelectedIndexChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Red;
      this.label2.Location = new System.Drawing.Point(100, 25);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(23, 15);
      this.label2.TabIndex = 1;
      this.label2.Text = "(*)";
      // 
      // ultDetail
      // 
      this.ultDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ultDetail.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDetail.Location = new System.Drawing.Point(3, 88);
      this.ultDetail.Name = "ultDetail";
      this.ultDetail.Size = new System.Drawing.Size(479, 329);
      this.ultDetail.TabIndex = 2;
      this.ultDetail.AfterRowsDeleted += new System.EventHandler(this.ultDetail_AfterRowsDeleted);
      this.ultDetail.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultDetail_BeforeCellUpdate);
      this.ultDetail.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultDetail_InitializeLayout);
      this.ultDetail.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultDetail_BeforeRowsDeleted);
      this.ultDetail.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultDetail_AfterCellUpdate);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(407, 423);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // udrItemCode
      // 
      this.udrItemCode.Cursor = System.Windows.Forms.Cursors.Default;
      this.udrItemCode.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.udrItemCode.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.udrItemCode.DisplayMember = "";
      this.udrItemCode.Location = new System.Drawing.Point(12, 98);
      this.udrItemCode.Name = "udrItemCode";
      this.udrItemCode.Size = new System.Drawing.Size(145, 37);
      this.udrItemCode.TabIndex = 5;
      this.udrItemCode.Text = "ultraDropDown1";
      this.udrItemCode.ValueMember = "";
      this.udrItemCode.Visible = false;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(326, 423);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 6;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // chkConfirm
      // 
      this.chkConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.chkConfirm.AutoSize = true;
      this.chkConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkConfirm.Location = new System.Drawing.Point(252, 427);
      this.chkConfirm.Name = "chkConfirm";
      this.chkConfirm.Size = new System.Drawing.Size(68, 17);
      this.chkConfirm.TabIndex = 7;
      this.chkConfirm.Text = "Confirm";
      this.chkConfirm.UseVisualStyleBackColor = true;
      this.chkConfirm.CheckedChanged += new System.EventHandler(this.chkConfirm_CheckedChanged);
      // 
      // viewCSD_04_002
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chkConfirm);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.udrItemCode);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.ultDetail);
      this.Controls.Add(this.grpInfo);
      this.Name = "viewCSD_04_002";
      this.Size = new System.Drawing.Size(485, 449);
      this.Load += new System.EventHandler(this.viewCSD_04_002_Load);
      this.grpInfo.ResumeLayout(false);
      this.grpInfo.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDetail)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.udrItemCode)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox grpInfo;
    private System.Windows.Forms.TextBox txtDescription;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultDetail;
    private System.Windows.Forms.Button btnClose;
    private Infragistics.Win.UltraWinGrid.UltraDropDown udrItemCode;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.CheckBox chkConfirm;
    private System.Windows.Forms.ComboBox cmbPriceBaseOn;
    private System.Windows.Forms.Label label4;
  }
}
