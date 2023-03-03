namespace DaiCo.Shared.UserControls
{
  partial class UC_ListViewExt
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
      this.lvContent = new System.Windows.Forms.ListView();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // lvContent
      // 
      this.lvContent.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvContent.FullRowSelect = true;
      this.lvContent.GridLines = true;
      this.lvContent.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.lvContent.Location = new System.Drawing.Point(0, 36);
      this.lvContent.Margin = new System.Windows.Forms.Padding(0);
      this.lvContent.Name = "lvContent";
      this.lvContent.Size = new System.Drawing.Size(317, 82);
      this.lvContent.TabIndex = 0;
      this.lvContent.UseCompatibleStateImageBehavior = false;
      this.lvContent.View = System.Windows.Forms.View.Details;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.lvContent, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(317, 118);
      this.tableLayoutPanel1.TabIndex = 1;
      // 
      // UC_ListViewExt
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "UC_ListViewExt";
      this.Size = new System.Drawing.Size(317, 118);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView lvContent;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
  }
}
