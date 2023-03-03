namespace DaiCo.Planning
{
  partial class viewPLN_02_030
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
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.grpSearch = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.label2 = new System.Windows.Forms.Label();
      this.ucItemCode = new DaiCo.Shared.ucUltraList();
      this.txtItemCode = new System.Windows.Forms.TextBox();
      this.chkItemCode = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnSearch = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.chkShowImage = new System.Windows.Forms.CheckBox();
      this.grpBoxCarcassCode = new System.Windows.Forms.GroupBox();
      this.picCarcassCode = new System.Windows.Forms.PictureBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.grpSearch.SuspendLayout();
      this.tableLayoutPanel4.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.grpBoxCarcassCode.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.picCarcassCode)).BeginInit();
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
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(953, 600);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.grpBoxCarcassCode);
      this.groupBox1.Controls.Add(this.chkShowImage);
      this.groupBox1.Controls.Add(this.ultData);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 217);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(947, 345);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Information";
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
      this.ultData.Size = new System.Drawing.Size(941, 326);
      this.ultData.TabIndex = 0;
      this.ultData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ultData_MouseDoubleClick);
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      this.ultData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ultData_MouseClick);
      // 
      // grpSearch
      // 
      this.grpSearch.AutoSize = true;
      this.grpSearch.Controls.Add(this.tableLayoutPanel4);
      this.grpSearch.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpSearch.Location = new System.Drawing.Point(3, 3);
      this.grpSearch.Name = "grpSearch";
      this.grpSearch.Size = new System.Drawing.Size(947, 208);
      this.grpSearch.TabIndex = 3;
      this.grpSearch.TabStop = false;
      this.grpSearch.Text = "Search Information";
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.AutoSize = true;
      this.tableLayoutPanel4.ColumnCount = 15;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
      this.tableLayoutPanel4.Controls.Add(this.label2, 0, 2);
      this.tableLayoutPanel4.Controls.Add(this.ucItemCode, 2, 1);
      this.tableLayoutPanel4.Controls.Add(this.txtItemCode, 2, 0);
      this.tableLayoutPanel4.Controls.Add(this.chkItemCode, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel4.Controls.Add(this.btnSearch, 14, 2);
      this.tableLayoutPanel4.Controls.Add(this.btnClear, 13, 2);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 3;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(941, 189);
      this.tableLayoutPanel4.TabIndex = 4;
      // 
      // label2
      // 
      this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label2.AutoSize = true;
      this.label2.BackColor = System.Drawing.Color.Yellow;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Black;
      this.label2.Location = new System.Drawing.Point(3, 167);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(83, 13);
      this.label2.TabIndex = 18;
      this.label2.Text = "Main Carcass";
      // 
      // ucItemCode
      // 
      this.ucItemCode.AutoSearchBy = "";
      this.tableLayoutPanel4.SetColumnSpan(this.ucItemCode, 13);
      this.ucItemCode.ColumnWidths = "";
      this.ucItemCode.DataSource = null;
      this.ucItemCode.DisplayMember = "";
      this.ucItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ucItemCode.Location = new System.Drawing.Point(137, 29);
      this.ucItemCode.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
      this.ucItemCode.Name = "ucItemCode";
      this.ucItemCode.SelectedText = "";
      this.ucItemCode.SelectedValue = "";
      this.ucItemCode.Separator = '\0';
      this.ucItemCode.Size = new System.Drawing.Size(804, 127);
      this.ucItemCode.TabIndex = 16;
      this.ucItemCode.Text = "ucUltraList2";
      this.ucItemCode.ValueMember = "";
      this.ucItemCode.Visible = false;
      this.ucItemCode.ValueChanged += new DaiCo.Shared.ValueChangedEventHandler(this.ucItemCode_ValueChanged);
      // 
      // txtItemCode
      // 
      this.tableLayoutPanel4.SetColumnSpan(this.txtItemCode, 13);
      this.txtItemCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtItemCode.Location = new System.Drawing.Point(140, 3);
      this.txtItemCode.Name = "txtItemCode";
      this.txtItemCode.ReadOnly = true;
      this.txtItemCode.Size = new System.Drawing.Size(798, 20);
      this.txtItemCode.TabIndex = 8;
      // 
      // chkItemCode
      // 
      this.chkItemCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.chkItemCode.AutoSize = true;
      this.chkItemCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkItemCode.Location = new System.Drawing.Point(110, 6);
      this.chkItemCode.Name = "chkItemCode";
      this.chkItemCode.Size = new System.Drawing.Size(15, 14);
      this.chkItemCode.TabIndex = 7;
      this.chkItemCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.chkItemCode.UseVisualStyleBackColor = true;
      this.chkItemCode.CheckedChanged += new System.EventHandler(this.chkItemCode_CheckedChanged);
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(81, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "CarcassCode";
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.Location = new System.Drawing.Point(880, 162);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(58, 23);
      this.btnSearch.TabIndex = 17;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // btnClear
      // 
      this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClear.Location = new System.Drawing.Point(816, 162);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(58, 23);
      this.btnClear.TabIndex = 19;
      this.btnClear.Text = "Clear";
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 5;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 3, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 568);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(947, 29);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(869, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "&Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(796, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(67, 23);
      this.btnSave.TabIndex = 0;
      this.btnSave.Text = "Approve";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // chkShowImage
      // 
      this.chkShowImage.AutoSize = true;
      this.chkShowImage.Location = new System.Drawing.Point(97, -1);
      this.chkShowImage.Name = "chkShowImage";
      this.chkShowImage.Size = new System.Drawing.Size(95, 17);
      this.chkShowImage.TabIndex = 5;
      this.chkShowImage.Text = "Show Image";
      this.chkShowImage.UseVisualStyleBackColor = true;
      this.chkShowImage.CheckedChanged += new System.EventHandler(this.chkShowImage_CheckedChanged);
      // 
      // grpBoxCarcassCode
      // 
      this.grpBoxCarcassCode.Controls.Add(this.picCarcassCode);
      this.grpBoxCarcassCode.Location = new System.Drawing.Point(299, 19);
      this.grpBoxCarcassCode.Name = "grpBoxCarcassCode";
      this.grpBoxCarcassCode.Size = new System.Drawing.Size(328, 264);
      this.grpBoxCarcassCode.TabIndex = 6;
      this.grpBoxCarcassCode.TabStop = false;
      this.grpBoxCarcassCode.Visible = false;
      // 
      // picCarcassCode
      // 
      this.picCarcassCode.Dock = System.Windows.Forms.DockStyle.Fill;
      this.picCarcassCode.Location = new System.Drawing.Point(3, 16);
      this.picCarcassCode.Name = "picCarcassCode";
      this.picCarcassCode.Size = new System.Drawing.Size(322, 245);
      this.picCarcassCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picCarcassCode.TabIndex = 0;
      this.picCarcassCode.TabStop = false;
      // 
      // viewPLN_02_030
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_02_030";
      this.Size = new System.Drawing.Size(953, 600);
      this.Load += new System.EventHandler(this.viewPLN_02_030_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.grpSearch.ResumeLayout(false);
      this.grpSearch.PerformLayout();
      this.tableLayoutPanel4.ResumeLayout(false);
      this.tableLayoutPanel4.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.grpBoxCarcassCode.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.picCarcassCode)).EndInit();
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
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox chkItemCode;
    private System.Windows.Forms.TextBox txtItemCode;
    private DaiCo.Shared.ucUltraList ucItemCode;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.CheckBox chkShowImage;
    private System.Windows.Forms.GroupBox grpBoxCarcassCode;
    private System.Windows.Forms.PictureBox picCarcassCode;
  }
}
