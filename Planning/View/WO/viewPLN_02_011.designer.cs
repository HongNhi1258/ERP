namespace Planning.View
{
  partial class viewPLN_02_011
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
      this.groupInformation = new System.Windows.Forms.GroupBox();
      this.groupItemImage = new System.Windows.Forms.GroupBox();
      this.pictureItem = new System.Windows.Forms.PictureBox();
      this.chkShowImage = new System.Windows.Forms.CheckBox();
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnSwapWo = new System.Windows.Forms.Button();
      this.chkExpandAll = new System.Windows.Forms.CheckBox();
      this.groupInformation.SuspendLayout();
      this.groupItemImage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureItem)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupInformation
      // 
      this.groupInformation.Controls.Add(this.chkExpandAll);
      this.groupInformation.Controls.Add(this.groupItemImage);
      this.groupInformation.Controls.Add(this.chkShowImage);
      this.groupInformation.Controls.Add(this.ultData);
      this.groupInformation.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupInformation.Location = new System.Drawing.Point(3, 3);
      this.groupInformation.Name = "groupInformation";
      this.groupInformation.Size = new System.Drawing.Size(1059, 641);
      this.groupInformation.TabIndex = 2;
      this.groupInformation.TabStop = false;
      this.groupInformation.Text = "Information";
      // 
      // groupItemImage
      // 
      this.groupItemImage.Controls.Add(this.pictureItem);
      this.groupItemImage.Location = new System.Drawing.Point(300, 85);
      this.groupItemImage.Name = "groupItemImage";
      this.groupItemImage.Size = new System.Drawing.Size(239, 197);
      this.groupItemImage.TabIndex = 80;
      this.groupItemImage.TabStop = false;
      this.groupItemImage.Text = "Item Image";
      this.groupItemImage.Visible = false;
      // 
      // pictureItem
      // 
      this.pictureItem.BackColor = System.Drawing.Color.Transparent;
      this.pictureItem.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureItem.ImageLocation = "";
      this.pictureItem.Location = new System.Drawing.Point(3, 16);
      this.pictureItem.Name = "pictureItem";
      this.pictureItem.Size = new System.Drawing.Size(233, 178);
      this.pictureItem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.pictureItem.TabIndex = 12;
      this.pictureItem.TabStop = false;
      // 
      // chkShowImage
      // 
      this.chkShowImage.AutoSize = true;
      this.chkShowImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkShowImage.Location = new System.Drawing.Point(116, -1);
      this.chkShowImage.Name = "chkShowImage";
      this.chkShowImage.Size = new System.Drawing.Size(95, 17);
      this.chkShowImage.TabIndex = 80;
      this.chkShowImage.Text = "Show Image";
      this.chkShowImage.UseVisualStyleBackColor = true;
      // 
      // ultData
      // 
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.DisplayLayout.AutoFitColumns = true;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultData.Location = new System.Drawing.Point(3, 16);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(1053, 622);
      this.ultData.TabIndex = 2;
      this.ultData.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ultData_KeyUp);
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      this.ultData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(988, 5);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(74, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.groupInformation, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(1065, 680);
      this.tableLayoutPanel2.TabIndex = 4;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 4;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
      this.tableLayoutPanel3.Controls.Add(this.btnClose, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSave, 2, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnSwapWo, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 647);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(1065, 33);
      this.tableLayoutPanel3.TabIndex = 3;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(905, 5);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(71, 23);
      this.btnSave.TabIndex = 4;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnSwapWo
      // 
      this.btnSwapWo.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSwapWo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSwapWo.Location = new System.Drawing.Point(809, 5);
      this.btnSwapWo.Name = "btnSwapWo";
      this.btnSwapWo.Size = new System.Drawing.Size(90, 23);
      this.btnSwapWo.TabIndex = 5;
      this.btnSwapWo.Text = "SwapWO";
      this.btnSwapWo.UseVisualStyleBackColor = true;
      this.btnSwapWo.Click += new System.EventHandler(this.btnSwapWo_Click);
      // 
      // chkExpandAll
      // 
      this.chkExpandAll.AutoSize = true;
      this.chkExpandAll.Location = new System.Drawing.Point(240, -1);
      this.chkExpandAll.Name = "chkExpandAll";
      this.chkExpandAll.Size = new System.Drawing.Size(86, 17);
      this.chkExpandAll.TabIndex = 81;
      this.chkExpandAll.Text = "Expand All";
      this.chkExpandAll.UseVisualStyleBackColor = true;
      this.chkExpandAll.CheckedChanged += new System.EventHandler(this.chkExpandAll_CheckedChanged);
      // 
      // viewPLN_02_011
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel2);
      this.Name = "viewPLN_02_011";
      this.Size = new System.Drawing.Size(1065, 680);
      this.Load += new System.EventHandler(this.viewPLN_02_011_Load);
      this.groupInformation.ResumeLayout(false);
      this.groupInformation.PerformLayout();
      this.groupItemImage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureItem)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupInformation;
    private System.Windows.Forms.GroupBox groupItemImage;
    private System.Windows.Forms.PictureBox pictureItem;
    private System.Windows.Forms.CheckBox chkShowImage;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnSwapWo;
    private System.Windows.Forms.CheckBox chkExpandAll;
  }
}
