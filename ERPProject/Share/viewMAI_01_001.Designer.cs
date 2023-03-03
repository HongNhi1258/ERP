using Infragistics.Win.UltraWinGrid;

namespace DaiCo.ERPProject
{
  partial class viewMAI_01_001
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
      this.colIcon = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.colUserName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.txtMarquee = new System.Windows.Forms.TextBox();
      this.uepDailyAtt = new Infragistics.Win.Misc.UltraExpandableGroupBox();
      this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
      this.ugdDailyAttendance = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uepDailyAtt)).BeginInit();
      this.uepDailyAtt.SuspendLayout();
      this.ultraExpandableGroupBoxPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ugdDailyAttendance)).BeginInit();
      this.SuspendLayout();
      // 
      // colIcon
      // 
      this.colIcon.Text = "";
      this.colIcon.Width = 23;
      // 
      // colUserName
      // 
      this.colUserName.Text = "";
      this.colUserName.Width = 85;
      // 
      // columnHeader1
      // 
      this.columnHeader1.Text = "";
      this.columnHeader1.Width = 23;
      // 
      // columnHeader2
      // 
      this.columnHeader2.Text = "";
      this.columnHeader2.Width = 85;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.txtMarquee, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.uepDailyAtt, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(1024, 768);
      this.tableLayoutPanel2.TabIndex = 4;
      // 
      // txtMarquee
      // 
      this.txtMarquee.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
      this.txtMarquee.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtMarquee.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtMarquee.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMarquee.ForeColor = System.Drawing.Color.DarkGreen;
      this.txtMarquee.Location = new System.Drawing.Point(3, 0);
      this.txtMarquee.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.txtMarquee.Multiline = true;
      this.txtMarquee.Name = "txtMarquee";
      this.txtMarquee.ReadOnly = true;
      this.txtMarquee.Size = new System.Drawing.Size(1018, 32);
      this.txtMarquee.TabIndex = 4;
      this.txtMarquee.Text = "Welcome to ERP system";
      this.txtMarquee.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // uepDailyAtt
      // 
      this.uepDailyAtt.Controls.Add(this.ultraExpandableGroupBoxPanel1);
      this.uepDailyAtt.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uepDailyAtt.ExpandedSize = new System.Drawing.Size(1018, 730);
      this.uepDailyAtt.Location = new System.Drawing.Point(3, 35);
      this.uepDailyAtt.Name = "uepDailyAtt";
      this.uepDailyAtt.Size = new System.Drawing.Size(1018, 730);
      this.uepDailyAtt.TabIndex = 6;
      this.uepDailyAtt.Text = "Daily Attendance";
      // 
      // ultraExpandableGroupBoxPanel1
      // 
      this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ugdDailyAttendance);
      this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
      this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
      this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(1012, 708);
      this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
      // 
      // ugdDailyAttendance
      // 
      this.ugdDailyAttendance.Cursor = System.Windows.Forms.Cursors.Default;
      this.ugdDailyAttendance.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ugdDailyAttendance.Location = new System.Drawing.Point(0, 0);
      this.ugdDailyAttendance.Name = "ugdDailyAttendance";
      this.ugdDailyAttendance.Size = new System.Drawing.Size(1012, 708);
      this.ugdDailyAttendance.TabIndex = 5;
      this.ugdDailyAttendance.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ugdDailyAttendance_InitializeLayout);
      // 
      // viewMAI_01_001
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel2);
      this.Name = "viewMAI_01_001";
      this.Size = new System.Drawing.Size(1024, 768);
      this.Load += new System.EventHandler(this.viewMAI_01_001_Load);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.uepDailyAtt)).EndInit();
      this.uepDailyAtt.ResumeLayout(false);
      this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.ugdDailyAttendance)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.ColumnHeader colIcon;
    private System.Windows.Forms.ColumnHeader colUserName;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ColumnHeader columnHeader2;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.TextBox txtMarquee;
    private UltraGrid ugdDailyAttendance;
    private Infragistics.Win.Misc.UltraExpandableGroupBox uepDailyAtt;
    private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
  }
}
