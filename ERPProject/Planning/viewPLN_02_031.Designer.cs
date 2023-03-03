namespace DaiCo.ERPProject
{
  partial class viewPLN_02_031
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
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.chkExpandAll = new System.Windows.Forms.CheckBox();
      this.grpBoxCarcassCode = new System.Windows.Forms.GroupBox();
      this.picCarcassCode = new System.Windows.Forms.PictureBox();
      this.ultddSupplier = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.chkShowImage = new System.Windows.Forms.CheckBox();
      this.btnClose = new System.Windows.Forms.Button();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtLocation = new System.Windows.Forms.TextBox();
      this.btnBrown = new System.Windows.Forms.Button();
      this.btnImport = new System.Windows.Forms.Button();
      this.btnGetTemplate = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.labErrors = new System.Windows.Forms.Label();
      this.btnTransfer = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.grpBoxCarcassCode.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picCarcassCode)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultddSupplier)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel1.Controls.Add(this.btnSave, 2, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 2);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnTransfer, 1, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(826, 459);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(667, 432);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 0;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBox1
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 4);
      this.groupBox1.Controls.Add(this.chkExpandAll);
      this.groupBox1.Controls.Add(this.grpBoxCarcassCode);
      this.groupBox1.Controls.Add(this.ultddSupplier);
      this.groupBox1.Controls.Add(this.ultData);
      this.groupBox1.Controls.Add(this.chkShowImage);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 68);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(820, 357);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Carcass Contract Out";
      // 
      // chkExpandAll
      // 
      this.chkExpandAll.AutoSize = true;
      this.chkExpandAll.Location = new System.Drawing.Point(144, -1);
      this.chkExpandAll.Name = "chkExpandAll";
      this.chkExpandAll.Size = new System.Drawing.Size(86, 17);
      this.chkExpandAll.TabIndex = 4;
      this.chkExpandAll.Text = "Expand All";
      this.chkExpandAll.UseVisualStyleBackColor = true;
      this.chkExpandAll.CheckedChanged += new System.EventHandler(this.chkExpandAll_CheckedChanged);
      // 
      // grpBoxCarcassCode
      // 
      this.grpBoxCarcassCode.Controls.Add(this.picCarcassCode);
      this.grpBoxCarcassCode.Location = new System.Drawing.Point(232, 23);
      this.grpBoxCarcassCode.Name = "grpBoxCarcassCode";
      this.grpBoxCarcassCode.Size = new System.Drawing.Size(390, 321);
      this.grpBoxCarcassCode.TabIndex = 3;
      this.grpBoxCarcassCode.TabStop = false;
      this.grpBoxCarcassCode.Visible = false;
      // 
      // picCarcassCode
      // 
      this.picCarcassCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.picCarcassCode.Location = new System.Drawing.Point(3, 16);
      this.picCarcassCode.Name = "picCarcassCode";
      this.picCarcassCode.Size = new System.Drawing.Size(384, 302);
      this.picCarcassCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picCarcassCode.TabIndex = 0;
      this.picCarcassCode.TabStop = false;
      // 
      // ultddSupplier
      // 
      this.ultddSupplier.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultddSupplier.Location = new System.Drawing.Point(12, 31);
      this.ultddSupplier.Name = "ultddSupplier";
      this.ultddSupplier.Size = new System.Drawing.Size(121, 48);
      this.ultddSupplier.TabIndex = 2;
      this.ultddSupplier.Text = "ultraDropDown1";
      this.ultddSupplier.Visible = false;
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 16);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(814, 338);
      this.ultData.TabIndex = 1;
      this.ultData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.BeforeCellActivate += new Infragistics.Win.UltraWinGrid.CancelableCellEventHandler(this.ultData_BeforeCellActivate);
      this.ultData.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultData_BeforeCellUpdate);
      this.ultData.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultData_BeforeRowsDeleted);
      this.ultData.DoubleClick += new System.EventHandler(this.ultData_DoubleClick);
      this.ultData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ultData_MouseClick);
      // 
      // chkShowImage
      // 
      this.chkShowImage.AutoSize = true;
      this.chkShowImage.Location = new System.Drawing.Point(260, 0);
      this.chkShowImage.Name = "chkShowImage";
      this.chkShowImage.Size = new System.Drawing.Size(95, 17);
      this.chkShowImage.TabIndex = 0;
      this.chkShowImage.Text = "Show Image";
      this.chkShowImage.UseVisualStyleBackColor = true;
      this.chkShowImage.CheckedChanged += new System.EventHandler(this.chkShowImage_CheckedChanged);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(748, 432);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 6;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 4);
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.label2, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.txtLocation, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnBrown, 3, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnImport, 4, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnGetTemplate, 5, 0);
      this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.labErrors, 2, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(820, 59);
      this.tableLayoutPanel2.TabIndex = 4;
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
      this.label1.Text = "Import Excel";
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Red;
      this.label2.Location = new System.Drawing.Point(103, 8);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(20, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "(*)";
      // 
      // txtLocation
      // 
      this.txtLocation.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtLocation.Location = new System.Drawing.Point(133, 3);
      this.txtLocation.Name = "txtLocation";
      this.txtLocation.ReadOnly = true;
      this.txtLocation.Size = new System.Drawing.Size(470, 20);
      this.txtLocation.TabIndex = 2;
      // 
      // btnBrown
      // 
      this.btnBrown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrown.Location = new System.Drawing.Point(609, 3);
      this.btnBrown.Name = "btnBrown";
      this.btnBrown.Size = new System.Drawing.Size(27, 23);
      this.btnBrown.TabIndex = 3;
      this.btnBrown.Text = "...";
      this.btnBrown.UseVisualStyleBackColor = true;
      this.btnBrown.Click += new System.EventHandler(this.btnBrown_Click);
      // 
      // btnImport
      // 
      this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnImport.Location = new System.Drawing.Point(642, 3);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(75, 23);
      this.btnImport.TabIndex = 4;
      this.btnImport.Text = "Import";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.Import_Click);
      // 
      // btnGetTemplate
      // 
      this.btnGetTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnGetTemplate.Location = new System.Drawing.Point(723, 3);
      this.btnGetTemplate.Name = "btnGetTemplate";
      this.btnGetTemplate.Size = new System.Drawing.Size(94, 23);
      this.btnGetTemplate.TabIndex = 5;
      this.btnGetTemplate.Text = "Get Template";
      this.btnGetTemplate.UseVisualStyleBackColor = true;
      this.btnGetTemplate.Click += new System.EventHandler(this.btnGetTemplate_Click);
      // 
      // label3
      // 
      this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.label3, 2);
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(3, 37);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(40, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Errors";
      // 
      // labErrors
      // 
      this.labErrors.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labErrors.AutoSize = true;
      this.tableLayoutPanel2.SetColumnSpan(this.labErrors, 3);
      this.labErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labErrors.Location = new System.Drawing.Point(133, 37);
      this.labErrors.Name = "labErrors";
      this.labErrors.Size = new System.Drawing.Size(0, 13);
      this.labErrors.TabIndex = 7;
      // 
      // btnTransfer
      // 
      this.btnTransfer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnTransfer.Location = new System.Drawing.Point(586, 431);
      this.btnTransfer.Name = "btnTransfer";
      this.btnTransfer.Size = new System.Drawing.Size(75, 23);
      this.btnTransfer.TabIndex = 5;
      this.btnTransfer.Text = "Lock";
      this.btnTransfer.UseVisualStyleBackColor = true;
      this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
      // 
      // viewPLN_02_031
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_02_031";
      this.Size = new System.Drawing.Size(826, 459);
      this.Load += new System.EventHandler(this.viewPLN_02_031_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.grpBoxCarcassCode.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.picCarcassCode)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultddSupplier)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.GroupBox groupBox1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.CheckBox chkShowImage;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtLocation;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Button btnGetTemplate;
    private System.Windows.Forms.Button btnBrown;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultddSupplier;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label labErrors;
    private System.Windows.Forms.GroupBox grpBoxCarcassCode;
    private System.Windows.Forms.PictureBox picCarcassCode;
    private System.Windows.Forms.CheckBox chkExpandAll;
    private System.Windows.Forms.Button btnTransfer;
  }
}
