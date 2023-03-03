namespace DaiCo.CustomerService
{
  partial class viewCSD_04_006
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
      this.panel1 = new System.Windows.Forms.Panel();
      this.cboReport = new System.Windows.Forms.ComboBox();
      this.lblReportName = new System.Windows.Forms.Label();
      this.panel2 = new System.Windows.Forms.Panel();
      this.pWeeklyStock = new System.Windows.Forms.Panel();
      this.btnTransit = new System.Windows.Forms.Button();
      this.btnFG = new System.Windows.Forms.Button();
      this.btnPrintWeekly = new System.Windows.Forms.Button();
      this.tbnCloseWeekly = new System.Windows.Forms.Button();
      this.pDimensionInfo = new System.Windows.Forms.Panel();
      this.btnPrint = new System.Windows.Forms.Button();
      this.txtCategory = new System.Windows.Forms.TextBox();
      this.txtCollection = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.cboItemKind = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnDimensionClose = new System.Windows.Forms.Button();
      this.lblTitle = new System.Windows.Forms.Label();
      this.tableLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.pWeeklyStock.SuspendLayout();
      this.pDimensionInfo.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.lblTitle, 0, 0);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(508, 389);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.cboReport);
      this.panel1.Controls.Add(this.lblReportName);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(3, 53);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(502, 44);
      this.panel1.TabIndex = 0;
      // 
      // cboReport
      // 
      this.cboReport.FormattingEnabled = true;
      this.cboReport.Location = new System.Drawing.Point(107, 12);
      this.cboReport.Name = "cboReport";
      this.cboReport.Size = new System.Drawing.Size(350, 21);
      this.cboReport.TabIndex = 0;
      this.cboReport.SelectedIndexChanged += new System.EventHandler(this.cboReport_SelectedIndexChanged);
      // 
      // lblReportName
      // 
      this.lblReportName.AutoSize = true;
      this.lblReportName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblReportName.Location = new System.Drawing.Point(50, 14);
      this.lblReportName.Name = "lblReportName";
      this.lblReportName.Size = new System.Drawing.Size(51, 17);
      this.lblReportName.TabIndex = 0;
      this.lblReportName.Text = "Report";
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.pWeeklyStock);
      this.panel2.Controls.Add(this.pDimensionInfo);
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.Location = new System.Drawing.Point(3, 103);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(502, 283);
      this.panel2.TabIndex = 2;
      // 
      // pWeeklyStock
      // 
      this.pWeeklyStock.Controls.Add(this.btnTransit);
      this.pWeeklyStock.Controls.Add(this.btnFG);
      this.pWeeklyStock.Controls.Add(this.btnPrintWeekly);
      this.pWeeklyStock.Controls.Add(this.tbnCloseWeekly);
      this.pWeeklyStock.Location = new System.Drawing.Point(11, 3);
      this.pWeeklyStock.Name = "pWeeklyStock";
      this.pWeeklyStock.Size = new System.Drawing.Size(483, 199);
      this.pWeeklyStock.TabIndex = 8;
      this.pWeeklyStock.Visible = false;
      // 
      // btnTransit
      // 
      this.btnTransit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnTransit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnTransit.Location = new System.Drawing.Point(224, 4);
      this.btnTransit.Name = "btnTransit";
      this.btnTransit.Size = new System.Drawing.Size(123, 23);
      this.btnTransit.TabIndex = 14;
      this.btnTransit.Text = "Input Excel Transit ";
      this.btnTransit.UseVisualStyleBackColor = true;
      this.btnTransit.Click += new System.EventHandler(this.btnTransit_Click);
      // 
      // btnFG
      // 
      this.btnFG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnFG.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnFG.Location = new System.Drawing.Point(119, 4);
      this.btnFG.Name = "btnFG";
      this.btnFG.Size = new System.Drawing.Size(99, 23);
      this.btnFG.TabIndex = 13;
      this.btnFG.Text = "Input Excel FG ";
      this.btnFG.UseVisualStyleBackColor = true;
      this.btnFG.Click += new System.EventHandler(this.btnFG_Click);
      // 
      // btnPrintWeekly
      // 
      this.btnPrintWeekly.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.btnPrintWeekly.Location = new System.Drawing.Point(156, 33);
      this.btnPrintWeekly.Name = "btnPrintWeekly";
      this.btnPrintWeekly.Size = new System.Drawing.Size(62, 23);
      this.btnPrintWeekly.TabIndex = 8;
      this.btnPrintWeekly.Text = "Print";
      this.btnPrintWeekly.UseVisualStyleBackColor = true;
      this.btnPrintWeekly.Click += new System.EventHandler(this.btnPrintWeekly_Click);
      // 
      // tbnCloseWeekly
      // 
      this.tbnCloseWeekly.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.tbnCloseWeekly.Location = new System.Drawing.Point(224, 33);
      this.tbnCloseWeekly.Name = "tbnCloseWeekly";
      this.tbnCloseWeekly.Size = new System.Drawing.Size(62, 23);
      this.tbnCloseWeekly.TabIndex = 9;
      this.tbnCloseWeekly.Text = "Close";
      this.tbnCloseWeekly.UseVisualStyleBackColor = true;
      this.tbnCloseWeekly.Click += new System.EventHandler(this.tbnCloseWeekly_Click);
      // 
      // pDimensionInfo
      // 
      this.pDimensionInfo.Controls.Add(this.btnPrint);
      this.pDimensionInfo.Controls.Add(this.txtCategory);
      this.pDimensionInfo.Controls.Add(this.txtCollection);
      this.pDimensionInfo.Controls.Add(this.label3);
      this.pDimensionInfo.Controls.Add(this.label2);
      this.pDimensionInfo.Controls.Add(this.cboItemKind);
      this.pDimensionInfo.Controls.Add(this.label1);
      this.pDimensionInfo.Controls.Add(this.btnDimensionClose);
      this.pDimensionInfo.Location = new System.Drawing.Point(14, 0);
      this.pDimensionInfo.Name = "pDimensionInfo";
      this.pDimensionInfo.Size = new System.Drawing.Size(483, 202);
      this.pDimensionInfo.TabIndex = 0;
      this.pDimensionInfo.Visible = false;
      // 
      // btnPrint
      // 
      this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.btnPrint.Location = new System.Drawing.Point(164, 99);
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(62, 23);
      this.btnPrint.TabIndex = 1;
      this.btnPrint.Text = "Print";
      this.btnPrint.UseVisualStyleBackColor = true;
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // txtCategory
      // 
      this.txtCategory.Location = new System.Drawing.Point(98, 44);
      this.txtCategory.Name = "txtCategory";
      this.txtCategory.Size = new System.Drawing.Size(347, 20);
      this.txtCategory.TabIndex = 1;
      // 
      // txtCollection
      // 
      this.txtCollection.Location = new System.Drawing.Point(98, 71);
      this.txtCollection.Name = "txtCollection";
      this.txtCollection.Size = new System.Drawing.Size(346, 20);
      this.txtCollection.TabIndex = 2;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(26, 71);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(69, 17);
      this.label3.TabIndex = 5;
      this.label3.Text = "Collection";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(26, 44);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(65, 17);
      this.label2.TabIndex = 4;
      this.label2.Text = "Category";
      // 
      // cboItemKind
      // 
      this.cboItemKind.FormattingEnabled = true;
      this.cboItemKind.Location = new System.Drawing.Point(98, 18);
      this.cboItemKind.Name = "cboItemKind";
      this.cboItemKind.Size = new System.Drawing.Size(350, 21);
      this.cboItemKind.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(26, 19);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(62, 17);
      this.label1.TabIndex = 2;
      this.label1.Text = "ItemKind";
      // 
      // btnDimensionClose
      // 
      this.btnDimensionClose.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.btnDimensionClose.Location = new System.Drawing.Point(232, 99);
      this.btnDimensionClose.Name = "btnDimensionClose";
      this.btnDimensionClose.Size = new System.Drawing.Size(62, 23);
      this.btnDimensionClose.TabIndex = 7;
      this.btnDimensionClose.Text = "Close";
      this.btnDimensionClose.UseVisualStyleBackColor = true;
      this.btnDimensionClose.Click += new System.EventHandler(this.btnDimensionClose_Click);
      // 
      // lblTitle
      // 
      this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.lblTitle.AutoSize = true;
      this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTitle.ForeColor = System.Drawing.SystemColors.ActiveCaption;
      this.lblTitle.Location = new System.Drawing.Point(94, 9);
      this.lblTitle.Name = "lblTitle";
      this.lblTitle.Size = new System.Drawing.Size(319, 31);
      this.lblTitle.TabIndex = 2;
      this.lblTitle.Text = "Customer Service Report";
      // 
      // viewCSD_04_006
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "viewCSD_04_006";
      this.Size = new System.Drawing.Size(508, 389);
      this.Load += new System.EventHandler(this.viewCSD_04_006_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.pWeeklyStock.ResumeLayout(false);
      this.pDimensionInfo.ResumeLayout(false);
      this.pDimensionInfo.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button btnPrint;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label lblReportName;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.ComboBox cboReport;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel pDimensionInfo;
    private System.Windows.Forms.TextBox txtCategory;
    private System.Windows.Forms.TextBox txtCollection;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cboItemKind;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Panel pWeeklyStock;
    private System.Windows.Forms.Button btnDimensionClose;
    private System.Windows.Forms.Button btnPrintWeekly;
    private System.Windows.Forms.Button tbnCloseWeekly;
    private System.Windows.Forms.Button btnTransit;
    private System.Windows.Forms.Button btnFG;
  }
}
