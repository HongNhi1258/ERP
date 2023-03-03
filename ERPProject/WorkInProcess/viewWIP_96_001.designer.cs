namespace DaiCo.ERPProject
{
    partial class viewWIP_96_001
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
          this.ultData = new Infragistics.Win.UltraWinGrid.UltraGrid();
          this.btnClose = new System.Windows.Forms.Button();
          this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
          this.btnSave = new System.Windows.Forms.Button();
          this.ultDDTeam = new Infragistics.Win.UltraWinGrid.UltraDropDown();
          this.ultDDTeamCode = new Infragistics.Win.UltraWinGrid.UltraDropDown();
          this.btnExport = new System.Windows.Forms.Button();
          this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
          this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
          this.btnSearch = new System.Windows.Forms.Button();
          this.txtProcess = new System.Windows.Forms.TextBox();
          ((System.ComponentModel.ISupportInitialize)(this.ultData)).BeginInit();
          this.tableLayoutPanel3.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.ultDDTeam)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.ultDDTeamCode)).BeginInit();
          this.tableLayoutPanel1.SuspendLayout();
          this.tableLayoutPanel2.SuspendLayout();
          this.SuspendLayout();
          // 
          // label1
          // 
          this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
          this.label1.AutoSize = true;
          this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.label1.Location = new System.Drawing.Point(3, 8);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(52, 13);
          this.label1.TabIndex = 0;
          this.label1.Text = "Process";
          // 
          // ultData
          // 
          this.ultData.Cursor = System.Windows.Forms.Cursors.Default;
          this.ultData.DisplayLayout.AutoFitColumns = true;
          this.ultData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
          this.ultData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
          this.ultData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
          this.ultData.Dock = System.Windows.Forms.DockStyle.Fill;
          this.ultData.Location = new System.Drawing.Point(3, 32);
          this.ultData.Name = "ultData";
          this.ultData.Size = new System.Drawing.Size(564, 347);
          this.ultData.TabIndex = 2;
          this.ultData.AfterRowsDeleted += new System.EventHandler(this.ultData_AfterRowsDeleted);
          this.ultData.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.ultData_BeforeCellUpdate);
          this.ultData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ultData_InitializeLayout);
          this.ultData.BeforeRowsDeleted += new Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventHandler(this.ultData_BeforeRowsDeleted);
          this.ultData.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.ultData_AfterCellUpdate);
          // 
          // btnClose
          // 
          this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.btnClose.Location = new System.Drawing.Point(492, 3);
          this.btnClose.Name = "btnClose";
          this.btnClose.Size = new System.Drawing.Size(75, 23);
          this.btnClose.TabIndex = 0;
          this.btnClose.Text = "Close";
          this.btnClose.UseVisualStyleBackColor = true;
          this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
          // 
          // tableLayoutPanel3
          // 
          this.tableLayoutPanel3.ColumnCount = 5;
          this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
          this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 178F));
          this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 74F));
          this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
          this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
          this.tableLayoutPanel3.Controls.Add(this.btnClose, 4, 0);
          this.tableLayoutPanel3.Controls.Add(this.btnSave, 3, 0);
          this.tableLayoutPanel3.Controls.Add(this.ultDDTeam, 0, 0);
          this.tableLayoutPanel3.Controls.Add(this.ultDDTeamCode, 1, 0);
          this.tableLayoutPanel3.Controls.Add(this.btnExport, 2, 0);
          this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 382);
          this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
          this.tableLayoutPanel3.Name = "tableLayoutPanel3";
          this.tableLayoutPanel3.RowCount = 1;
          this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
          this.tableLayoutPanel3.Size = new System.Drawing.Size(570, 29);
          this.tableLayoutPanel3.TabIndex = 1;
          // 
          // btnSave
          // 
          this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.btnSave.Location = new System.Drawing.Point(411, 3);
          this.btnSave.Name = "btnSave";
          this.btnSave.Size = new System.Drawing.Size(75, 23);
          this.btnSave.TabIndex = 1;
          this.btnSave.Text = "Save";
          this.btnSave.UseVisualStyleBackColor = true;
          this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
          // 
          // ultDDTeam
          // 
          this.ultDDTeam.Cursor = System.Windows.Forms.Cursors.Default;
          this.ultDDTeam.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
          this.ultDDTeam.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
          this.ultDDTeam.DisplayMember = "";
          this.ultDDTeam.Location = new System.Drawing.Point(3, 3);
          this.ultDDTeam.Name = "ultDDTeam";
          this.ultDDTeam.Size = new System.Drawing.Size(91, 23);
          this.ultDDTeam.TabIndex = 2;
          this.ultDDTeam.ValueMember = "";
          this.ultDDTeam.Visible = false;
          // 
          // ultDDTeamCode
          // 
          this.ultDDTeamCode.Cursor = System.Windows.Forms.Cursors.Default;
          this.ultDDTeamCode.DisplayLayout.AutoFitColumns = true;
          this.ultDDTeamCode.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
          this.ultDDTeamCode.DisplayMember = "";
          this.ultDDTeamCode.Location = new System.Drawing.Point(159, 3);
          this.ultDDTeamCode.Name = "ultDDTeamCode";
          this.ultDDTeamCode.Size = new System.Drawing.Size(172, 23);
          this.ultDDTeamCode.TabIndex = 3;
          this.ultDDTeamCode.ValueMember = "";
          this.ultDDTeamCode.Visible = false;
          // 
          // btnExport
          // 
          this.btnExport.Dock = System.Windows.Forms.DockStyle.Fill;
          this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.btnExport.Location = new System.Drawing.Point(337, 3);
          this.btnExport.Name = "btnExport";
          this.btnExport.Size = new System.Drawing.Size(68, 23);
          this.btnExport.TabIndex = 4;
          this.btnExport.Text = "Export";
          this.btnExport.UseVisualStyleBackColor = true;
          this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
          // 
          // tableLayoutPanel1
          // 
          this.tableLayoutPanel1.ColumnCount = 1;
          this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
          this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
          this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
          this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
          this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
          this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
          this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
          this.tableLayoutPanel1.Controls.Add(this.ultData, 0, 1);
          this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
          this.tableLayoutPanel1.Name = "tableLayoutPanel1";
          this.tableLayoutPanel1.RowCount = 3;
          this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
          this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
          this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
          this.tableLayoutPanel1.Size = new System.Drawing.Size(570, 411);
          this.tableLayoutPanel1.TabIndex = 1;
          // 
          // tableLayoutPanel2
          // 
          this.tableLayoutPanel2.ColumnCount = 3;
          this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
          this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
          this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
          this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
          this.tableLayoutPanel2.Controls.Add(this.btnSearch, 2, 0);
          this.tableLayoutPanel2.Controls.Add(this.txtProcess, 1, 0);
          this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
          this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
          this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
          this.tableLayoutPanel2.Name = "tableLayoutPanel2";
          this.tableLayoutPanel2.RowCount = 1;
          this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
          this.tableLayoutPanel2.Size = new System.Drawing.Size(570, 29);
          this.tableLayoutPanel2.TabIndex = 0;
          // 
          // btnSearch
          // 
          this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.btnSearch.Location = new System.Drawing.Point(492, 3);
          this.btnSearch.Name = "btnSearch";
          this.btnSearch.Size = new System.Drawing.Size(75, 23);
          this.btnSearch.TabIndex = 1;
          this.btnSearch.Text = "Search";
          this.btnSearch.UseVisualStyleBackColor = true;
          this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
          this.btnSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnSearch_KeyDown);
          // 
          // txtProcess
          // 
          this.txtProcess.Dock = System.Windows.Forms.DockStyle.Fill;
          this.txtProcess.Location = new System.Drawing.Point(103, 5);
          this.txtProcess.Margin = new System.Windows.Forms.Padding(3, 5, 3, 4);
          this.txtProcess.Name = "txtProcess";
          this.txtProcess.Size = new System.Drawing.Size(383, 20);
          this.txtProcess.TabIndex = 2;
          this.txtProcess.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProcess_KeyDown);
          // 
          // viewWIP_96_001
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.AutoSize = true;
          this.Controls.Add(this.tableLayoutPanel1);
          this.Name = "viewWIP_96_001";
          this.Size = new System.Drawing.Size(570, 411);
          this.Load += new System.EventHandler(this.ViewWIP_96_001_Load);
          ((System.ComponentModel.ISupportInitialize)(this.ultData)).EndInit();
          this.tableLayoutPanel3.ResumeLayout(false);
          ((System.ComponentModel.ISupportInitialize)(this.ultDDTeam)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.ultDDTeamCode)).EndInit();
          this.tableLayoutPanel1.ResumeLayout(false);
          this.tableLayoutPanel2.ResumeLayout(false);
          this.tableLayoutPanel2.PerformLayout();
          this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultData;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnSave;
        private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDTeam;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtProcess;
      private Infragistics.Win.UltraWinGrid.UltraDropDown ultDDTeamCode;
      private System.Windows.Forms.Button btnExport;

    }
}
