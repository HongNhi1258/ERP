namespace DaiCo.Planning
{
  partial class viewPLN_05_013
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
      this.components = new System.ComponentModel.Container();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.cmbYear = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.label1 = new System.Windows.Forms.Label();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.ultraQuota = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.GbSkillWorker = new System.Windows.Forms.GroupBox();
      this.ultSkillWorker = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.groupBox1.SuspendLayout();
      this.groupBox5.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraQuota)).BeginInit();
      this.tableLayoutPanel1.SuspendLayout();
      this.GbSkillWorker.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultSkillWorker)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 5);
      this.groupBox1.Controls.Add(this.cmbYear);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.groupBox1.Size = new System.Drawing.Size(1174, 54);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Search Information";
      // 
      // cmbYear
      // 
      this.cmbYear.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cmbYear.FormattingEnabled = true;
      this.cmbYear.Location = new System.Drawing.Point(75, 19);
      this.cmbYear.Name = "cmbYear";
      this.cmbYear.Size = new System.Drawing.Size(174, 21);
      this.cmbYear.TabIndex = 1;
      this.cmbYear.SelectedIndexChanged += new System.EventHandler(this.cmbYear_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(20, 25);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(33, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Year";
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(1112, 604);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(65, 23);
      this.btnClose.TabIndex = 2;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Location = new System.Drawing.Point(1042, 604);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(64, 23);
      this.btnSave.TabIndex = 4;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBox5
      // 
      this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox5, 5);
      this.groupBox5.Controls.Add(this.ultraQuota);
      this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.groupBox5.Location = new System.Drawing.Point(3, 63);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(1174, 264);
      this.groupBox5.TabIndex = 3;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Manhour Budget";
      // 
      // ultraQuota
      // 
      this.ultraQuota.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ultraQuota.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraQuota.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultraQuota.Location = new System.Drawing.Point(3, 16);
      this.ultraQuota.Name = "ultraQuota";
      this.ultraQuota.Size = new System.Drawing.Size(1168, 242);
      this.ultraQuota.TabIndex = 0;
      this.ultraQuota.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultraQuota_BeforeCellUpdate);
      this.ultraQuota.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultraQuota_InitializeLayout);
      this.ultraQuota.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultraQuota_AfterCellUpdate);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 5;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 119F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.groupBox5, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.btnClose, 4, 3);
      this.tableLayoutPanel1.Controls.Add(this.btnSave, 3, 3);
      this.tableLayoutPanel1.Controls.Add(this.GbSkillWorker, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(1180, 630);
      this.tableLayoutPanel1.TabIndex = 5;
      // 
      // GbSkillWorker
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.GbSkillWorker, 5);
      this.GbSkillWorker.Controls.Add(this.ultSkillWorker);
      this.GbSkillWorker.Dock = System.Windows.Forms.DockStyle.Fill;
      this.GbSkillWorker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.GbSkillWorker.Location = new System.Drawing.Point(3, 333);
      this.GbSkillWorker.Name = "GbSkillWorker";
      this.GbSkillWorker.Size = new System.Drawing.Size(1174, 264);
      this.GbSkillWorker.TabIndex = 5;
      this.GbSkillWorker.TabStop = false;
      this.GbSkillWorker.Text = "Skill Worker Budget";
      // 
      // ultSkillWorker
      // 
      this.ultSkillWorker.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultSkillWorker.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultSkillWorker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultSkillWorker.Location = new System.Drawing.Point(3, 16);
      this.ultSkillWorker.Name = "ultSkillWorker";
      this.ultSkillWorker.Size = new System.Drawing.Size(1168, 245);
      this.ultSkillWorker.TabIndex = 0;
      this.ultSkillWorker.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultSkillWorker_BeforeCellUpdate);
      this.ultSkillWorker.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultSkillWorker_InitializeLayout);
      // 
      // viewPLN_05_013
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewPLN_05_013";
      this.Size = new System.Drawing.Size(1180, 630);
      this.Load += new System.EventHandler(this.viewPLN_05_001_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox5.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultraQuota)).EndInit();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.GbSkillWorker.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ultSkillWorker)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnSave;
    private DaiCo.Shared.DaiCoComboBox cmbYear;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox5;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultraQuota;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox GbSkillWorker;
    private Infragistics.Win.UltraWinGrid.UltraGrid ultSkillWorker;
  }
}
