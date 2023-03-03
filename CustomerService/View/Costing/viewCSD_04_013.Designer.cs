namespace DaiCo.CustomerService
{
  partial class viewCSD_04_013
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
      this.grpDistribute = new System.Windows.Forms.GroupBox();
      this.ugidDistribute = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnChoose = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.grpDistribute.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugidDistribute)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.grpDistribute, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 461);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // grpDistribute
      // 
      this.grpDistribute.Controls.Add(this.ugidDistribute);
      this.grpDistribute.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpDistribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grpDistribute.Location = new System.Drawing.Point(3, 3);
      this.grpDistribute.Name = "grpDistribute";
      this.grpDistribute.Size = new System.Drawing.Size(494, 426);
      this.grpDistribute.TabIndex = 0;
      this.grpDistribute.TabStop = false;
      this.grpDistribute.Text = "Choose distibute";
      // 
      // ugidDistribute
      // 
      this.ugidDistribute.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ugidDistribute.Cursor = System.Windows.Forms.Cursors.Default;
      this.ugidDistribute.DisplayLayout.AutoFitColumns = true;
      this.ugidDistribute.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ugidDistribute.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ugidDistribute.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ugidDistribute.Location = new System.Drawing.Point(3, 16);
      this.ugidDistribute.Name = "ugidDistribute";
      this.ugidDistribute.Size = new System.Drawing.Size(488, 407);
      this.ugidDistribute.TabIndex = 0;
      this.ugidDistribute.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugidDistribute_InitializeLayout);
      this.ugidDistribute.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ugidDistribute_CellChange);
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel2.Controls.Add(this.btnClose, 2, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnChoose, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 432);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(500, 29);
      this.tableLayoutPanel2.TabIndex = 1;
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(424, 3);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(73, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnChoose
      // 
      this.btnChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnChoose.Location = new System.Drawing.Point(345, 3);
      this.btnChoose.Name = "btnChoose";
      this.btnChoose.Size = new System.Drawing.Size(73, 23);
      this.btnChoose.TabIndex = 0;
      this.btnChoose.Text = "Choose";
      this.btnChoose.UseVisualStyleBackColor = true;
      this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
      // 
      // viewCSD_04_013
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "viewCSD_04_013";
      this.Size = new System.Drawing.Size(500, 461);
      this.Load += new System.EventHandler(this.viewCSD_04_013_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.grpDistribute.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugidDistribute)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox grpDistribute;
    private Infragistics.Win.UltraWinGrid.UltraGrid ugidDistribute;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnChoose;

  }
}
