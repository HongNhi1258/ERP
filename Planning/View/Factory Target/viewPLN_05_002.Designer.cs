namespace DaiCo.Planning
{
  partial class viewPLN_05_002
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.ultDDCustomer = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.ultDDKindCustomer = new Infragistics.Win.UltraWinGrid.UltraDropDown();
      this.ultGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.groupBox1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDCustomer)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDKindCustomer)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultGrid)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.tableLayoutPanel1);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(0, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(924, 512);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Group Customer";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 293F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnSave, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.ultDDCustomer, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.ultDDKindCustomer, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.ultGrid, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(918, 493);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnClose
      // 
      this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnClose.Location = new System.Drawing.Point(849, 467);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(66, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.btnSave.Location = new System.Drawing.Point(773, 467);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(70, 23);
      this.btnSave.TabIndex = 2;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // ultDDCustomer
      // 
      this.ultDDCustomer.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDCustomer.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultDDCustomer.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultDDCustomer.DisplayMember = "";
      this.ultDDCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDDCustomer.Location = new System.Drawing.Point(3, 467);
      this.ultDDCustomer.Name = "ultDDCustomer";
      this.ultDDCustomer.Size = new System.Drawing.Size(113, 23);
      this.ultDDCustomer.TabIndex = 3;
      this.ultDDCustomer.ValueMember = "";
      this.ultDDCustomer.Visible = false;
      // 
      // ultDDKindCustomer
      // 
      this.ultDDKindCustomer.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultDDKindCustomer.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultDDKindCustomer.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultDDKindCustomer.DisplayMember = "";
      this.ultDDKindCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultDDKindCustomer.Location = new System.Drawing.Point(478, 467);
      this.ultDDKindCustomer.Name = "ultDDKindCustomer";
      this.ultDDKindCustomer.Size = new System.Drawing.Size(116, 23);
      this.ultDDKindCustomer.TabIndex = 4;
      this.ultDDKindCustomer.Text = "ultraDropDown1";
      this.ultDDKindCustomer.ValueMember = "";
      this.ultDDKindCustomer.Visible = false;
      // 
      // ultGrid
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.ultGrid, 4);
      this.ultGrid.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      this.ultGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.ultGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultGrid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultGrid.Location = new System.Drawing.Point(3, 3);
      this.ultGrid.Name = "ultGrid";
      this.ultGrid.Size = new System.Drawing.Size(912, 458);
      this.ultGrid.TabIndex = 5;
      this.ultGrid.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultGrid_BeforeCellUpdate);
      this.ultGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultGrid_InitializeLayout);
      this.ultGrid.BeforeRowActivate += new Infragistics.Win.UltraWinGrid.RowEventHandler(this.ultGrid_BeforeRowActivate);
      this.ultGrid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultGrid_AfterCellUpdate);
      // 
      // viewPLN_05_002
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Name = "viewPLN_05_002";
      this.Size = new System.Drawing.Size(924, 512);
      this.Load += new System.EventHandler(this.viewPLN_05_002_Load);
      this.groupBox1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultDDCustomer)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultDDKindCustomer)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultGrid)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDCustomer;
    private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDKindCustomer;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultGrid;
  }
}
