namespace DaiCo.ERPProject
{
  partial class view_SaveMasterDetail
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
      this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.chkConfirm = new System.Windows.Forms.CheckBox();
      this.btnExportExcel = new System.Windows.Forms.Button();
      this.gpbMaster = new System.Windows.Forms.GroupBox();
      this.tlpMaster = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.ucbWo = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.lbWO = new System.Windows.Forms.Label();
      this.gpbDetail = new System.Windows.Forms.GroupBox();
      this.ugdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tlpForm.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.gpbMaster.SuspendLayout();
      this.tlpMaster.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbWo)).BeginInit();
      this.gpbDetail.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugdData)).BeginInit();
      this.SuspendLayout();
      // 
      // tlpForm
      // 
      this.tlpForm.ColumnCount = 1;
      this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.Controls.Add(this.tableLayoutPanel3, 0, 2);
      this.tlpForm.Controls.Add(this.gpbMaster, 0, 0);
      this.tlpForm.Controls.Add(this.gpbDetail, 0, 1);
      this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpForm.Location = new System.Drawing.Point(0, 0);
      this.tlpForm.Name = "tlpForm";
      this.tlpForm.RowCount = 3;
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 144F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpForm.Size = new System.Drawing.Size(851, 516);
      this.tlpForm.TabIndex = 0;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 5;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.chkConfirm, 1, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnExportExcel, 3, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 487);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(851, 29);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(773, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "&Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(611, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 1;
      this.btnSave.Text = "&Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // chkConfirm
      // 
      this.chkConfirm.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.chkConfirm.AutoSize = true;
      this.chkConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkConfirm.Location = new System.Drawing.Point(537, 6);
      this.chkConfirm.Name = "chkConfirm";
      this.chkConfirm.Size = new System.Drawing.Size(68, 17);
      this.chkConfirm.TabIndex = 0;
      this.chkConfirm.Text = "Confirm";
      this.chkConfirm.UseVisualStyleBackColor = true;
      // 
      // btnExportExcel
      // 
      this.btnExportExcel.AutoSize = true;
      this.btnExportExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnExportExcel.Image = global::DaiCo.ERPProject.Properties.Resources.Excel;
      this.btnExportExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnExportExcel.Location = new System.Drawing.Point(692, 3);
      this.btnExportExcel.Name = "btnExportExcel";
      this.btnExportExcel.Size = new System.Drawing.Size(75, 23);
      this.btnExportExcel.TabIndex = 4;
      this.btnExportExcel.Text = "Export";
      this.btnExportExcel.UseVisualStyleBackColor = true;
      this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
      // 
      // gpbMaster
      // 
      this.gpbMaster.Controls.Add(this.tlpMaster);
      this.gpbMaster.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gpbMaster.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gpbMaster.Location = new System.Drawing.Point(3, 3);
      this.gpbMaster.Name = "gpbMaster";
      this.gpbMaster.Size = new System.Drawing.Size(845, 138);
      this.gpbMaster.TabIndex = 0;
      this.gpbMaster.TabStop = false;
      this.gpbMaster.Text = "Master";
      // 
      // tlpMaster
      // 
      this.tlpMaster.ColumnCount = 7;
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tlpMaster.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tlpMaster.Controls.Add(this.label2, 0, 0);
      this.tlpMaster.Controls.Add(this.ucbWo, 2, 0);
      this.tlpMaster.Controls.Add(this.lbWO, 1, 0);
      this.tlpMaster.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tlpMaster.Location = new System.Drawing.Point(3, 16);
      this.tlpMaster.Name = "tlpMaster";
      this.tlpMaster.RowCount = 4;
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tlpMaster.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tlpMaster.Size = new System.Drawing.Size(839, 119);
      this.tlpMaster.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(3, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(72, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Work Order";
      // 
      // ucbWo
      // 
      this.ucbWo.Cursor = System.Windows.Forms.Cursors.Default;
      this.ucbWo.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ucbWo.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ucbWo.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ucbWo.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucbWo.Location = new System.Drawing.Point(123, 4);
      this.ucbWo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.ucbWo.Name = "ucbWo";
      this.ucbWo.Size = new System.Drawing.Size(280, 21);
      this.ucbWo.TabIndex = 2;
      // 
      // lbWO
      // 
      this.lbWO.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.lbWO.AutoSize = true;
      this.lbWO.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbWO.ForeColor = System.Drawing.Color.Red;
      this.lbWO.Location = new System.Drawing.Point(93, 7);
      this.lbWO.Name = "lbWO";
      this.lbWO.Size = new System.Drawing.Size(23, 15);
      this.lbWO.TabIndex = 17;
      this.lbWO.Text = "(*)";
      // 
      // gpbDetail
      // 
      this.gpbDetail.Controls.Add(this.ugdData);
      this.gpbDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gpbDetail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.gpbDetail.Location = new System.Drawing.Point(3, 147);
      this.gpbDetail.Name = "gpbDetail";
      this.gpbDetail.Size = new System.Drawing.Size(845, 337);
      this.gpbDetail.TabIndex = 1;
      this.gpbDetail.TabStop = false;
      this.gpbDetail.Text = "Detail";
      // 
      // ugdData
      // 
      this.ugdData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ugdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ugdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      this.ugdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ugdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ugdData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ugdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ugdData.Location = new System.Drawing.Point(3, 16);
      this.ugdData.Name = "ugdData";
      this.ugdData.Size = new System.Drawing.Size(839, 318);
      this.ugdData.TabIndex = 1;
      this.ugdData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      this.ugdData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ugdData.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultData_BeforeCellUpdate);
      this.ugdData.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultData_BeforeRowsDeleted);
      this.ugdData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ultData_MouseClick);
      // 
      // view_SaveMasterDetail
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tlpForm);
      this.Name = "view_SaveMasterDetail";
      this.Size = new System.Drawing.Size(851, 516);
      this.Load += new System.EventHandler(this.view_SaveMasterDetail_Load);
      this.tlpForm.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.gpbMaster.ResumeLayout(false);
      this.tlpMaster.ResumeLayout(false);
      this.tlpMaster.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ucbWo)).EndInit();
      this.gpbDetail.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugdData)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpForm;
    private System.Windows.Forms.TableLayoutPanel tlpMaster;
    private System.Windows.Forms.Label label2;
    private Infragistics.Win.UltraWinGrid.UltraCombo ucbWo;
    private System.Windows.Forms.Label lbWO;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.CheckBox chkConfirm;
    private Infragistics.Win.UltraWinGrid.UltraGrid ugdData;
    private System.Windows.Forms.GroupBox gpbMaster;
    private System.Windows.Forms.GroupBox gpbDetail;
    private System.Windows.Forms.Button btnExportExcel;
  }
}
