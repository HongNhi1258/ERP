namespace Purchasing
{
  partial class viewPUR_21_015
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
      this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnAdjust = new System.Windows.Forms.Button();
      this.labPRNo = new System.Windows.Forms.Label();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
      this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 1, 2);
      this.tableLayoutPanel1.Controls.Add(this.btnAdjust, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.labPRNo, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(705, 255);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // ultData
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultData, 2);
      this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultData.Location = new System.Drawing.Point(3, 32);
      this.ultData.Name = "ultData";
      this.ultData.Size = new System.Drawing.Size(699, 191);
      this.ultData.TabIndex = 0;
      this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
      // 
      // btnClose
      // 
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(627, 229);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnAdjust
      // 
      this.btnAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnAdjust.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAdjust.Location = new System.Drawing.Point(546, 229);
      this.btnAdjust.Name = "btnAdjust";
      this.btnAdjust.Size = new System.Drawing.Size(75, 23);
      this.btnAdjust.TabIndex = 2;
      this.btnAdjust.Text = "Adjust";
      this.btnAdjust.UseVisualStyleBackColor = true;
      this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);
      // 
      // labPRNo
      // 
      this.labPRNo.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.labPRNo.AutoSize = true;
      this.labPRNo.Location = new System.Drawing.Point(3, 8);
      this.labPRNo.Name = "labPRNo";
      this.labPRNo.Size = new System.Drawing.Size(0, 13);
      this.labPRNo.TabIndex = 3;
      // 
      // viewPUR_21_015
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPUR_21_015";
      this.Size = new System.Drawing.Size(705, 255);
      this.Load += new System.EventHandler(this.viewPUR_21_015_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnAdjust;
    private System.Windows.Forms.Label labPRNo;
  }
}
