namespace DaiCo.Shared
{
  partial class ucUltraList
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
      this.lstLeft = new System.Windows.Forms.ListView();
      this.lstRight = new System.Windows.Forms.ListView();
      this.btnRemove = new System.Windows.Forms.Button();
      this.btnAddAll = new System.Windows.Forms.Button();
      this.txtRight = new System.Windows.Forms.TextBox();
      this.txtLeft = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnAdd = new System.Windows.Forms.Button();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.btnRemoveAll = new System.Windows.Forms.Button();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // lstLeft
      // 
      this.lstLeft.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lstLeft.FullRowSelect = true;
      this.lstLeft.Location = new System.Drawing.Point(3, 23);
      this.lstLeft.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.lstLeft.Name = "lstLeft";
      this.tableLayoutPanel1.SetRowSpan(this.lstLeft, 3);
      this.lstLeft.Size = new System.Drawing.Size(171, 128);
      this.lstLeft.TabIndex = 12;
      this.lstLeft.UseCompatibleStateImageBehavior = false;
      this.lstLeft.View = System.Windows.Forms.View.Details;
      this.lstLeft.SelectedIndexChanged += new System.EventHandler(this.lstLeft_SelectedIndexChanged);
      this.lstLeft.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstLeft_KeyDown);
      this.lstLeft.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstLeft_ColumnClick);
      this.lstLeft.DoubleClick += new System.EventHandler(this.lstLeft_DoubleClick);      
    
      // 
      // lstRight
      // 
      this.lstRight.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lstRight.FullRowSelect = true;
      this.lstRight.Location = new System.Drawing.Point(260, 23);
      this.lstRight.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.lstRight.Name = "lstRight";
      this.tableLayoutPanel1.SetRowSpan(this.lstRight, 3);
      this.lstRight.Size = new System.Drawing.Size(171, 128);
      this.lstRight.TabIndex = 18;
      this.lstRight.UseCompatibleStateImageBehavior = false;
      this.lstRight.View = System.Windows.Forms.View.Details;
      this.lstRight.SelectedIndexChanged += new System.EventHandler(this.lstRight_SelectedIndexChanged);
      this.lstRight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstRight_KeyDown);
      this.lstRight.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstRight_ColumnClick);
      this.lstRight.DoubleClick += new System.EventHandler(this.lstRight_DoubleClick);
      // 
      // btnRemove
      // 
      this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnRemove.Enabled = false;
      this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnRemove.Location = new System.Drawing.Point(12, 3);
      this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.btnRemove.Name = "btnRemove";
      this.btnRemove.Size = new System.Drawing.Size(56, 23);
      this.btnRemove.TabIndex = 16;
      this.btnRemove.Text = "&<";
      this.btnRemove.UseVisualStyleBackColor = true;
      this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
      // 
      // btnAddAll
      // 
      this.btnAddAll.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnAddAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAddAll.Location = new System.Drawing.Point(12, 29);
      this.btnAddAll.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
      this.btnAddAll.Name = "btnAddAll";
      this.btnAddAll.Size = new System.Drawing.Size(56, 23);
      this.btnAddAll.TabIndex = 14;
      this.btnAddAll.Text = ">>";
      this.btnAddAll.UseVisualStyleBackColor = true;
      this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
      // 
      // txtRight
      // 
      this.txtRight.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtRight.Location = new System.Drawing.Point(260, 0);
      this.txtRight.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.txtRight.Name = "txtRight";
      this.txtRight.Size = new System.Drawing.Size(171, 20);
      this.txtRight.TabIndex = 17;
      this.txtRight.TextChanged += new System.EventHandler(this.txtRight_TextChanged);
      this.txtRight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRight_KeyDown);
      // 
      // txtLeft
      // 
      this.txtLeft.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtLeft.Location = new System.Drawing.Point(3, 0);
      this.txtLeft.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.txtLeft.Name = "txtLeft";
      this.txtLeft.Size = new System.Drawing.Size(171, 20);
      this.txtLeft.TabIndex = 11;
      this.txtLeft.TextChanged += new System.EventHandler(this.txtLeft_TextChanged);
      this.txtLeft.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLeft_KeyDown);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel1.Controls.Add(this.txtLeft, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.txtRight, 2, 0);
      this.tableLayoutPanel1.Controls.Add(this.lstLeft, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.lstRight, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(434, 151);
      this.tableLayoutPanel1.TabIndex = 21;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.btnAddAll, 0, 2);
      this.tableLayoutPanel2.Controls.Add(this.btnAdd, 0, 1);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(177, 20);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 3;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.Size = new System.Drawing.Size(80, 55);
      this.tableLayoutPanel2.TabIndex = 19;
      // 
      // btnAdd
      // 
      this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnAdd.Enabled = false;
      this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnAdd.Location = new System.Drawing.Point(12, 3);
      this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(56, 23);
      this.btnAdd.TabIndex = 13;
      this.btnAdd.Text = "&>";
      this.btnAdd.UseVisualStyleBackColor = true;
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.btnRemove, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.btnRemoveAll, 0, 1);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(177, 96);
      this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 3;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(80, 55);
      this.tableLayoutPanel3.TabIndex = 20;
      // 
      // btnRemoveAll
      // 
      this.btnRemoveAll.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnRemoveAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnRemoveAll.Location = new System.Drawing.Point(12, 29);
      this.btnRemoveAll.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
      this.btnRemoveAll.Name = "btnRemoveAll";
      this.btnRemoveAll.Size = new System.Drawing.Size(56, 23);
      this.btnRemoveAll.TabIndex = 15;
      this.btnRemoveAll.Text = "<<";
      this.btnRemoveAll.UseVisualStyleBackColor = true;
      this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
      // 
      // ucUltraList
      // 
      //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "ucUltraList";
      this.Size = new System.Drawing.Size(434, 151);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView lstLeft;
    private System.Windows.Forms.ListView lstRight;
    private System.Windows.Forms.Button btnRemove;
    private System.Windows.Forms.Button btnAddAll;
    private System.Windows.Forms.TextBox txtRight;
    private System.Windows.Forms.TextBox txtLeft;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Button btnRemoveAll;
  }
}
