namespace DaiCo.ERPProject
{
  partial class viewBOM_03_005
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
      this.btnClose = new System.Windows.Forms.Button();
      this.txtSuppCode = new System.Windows.Forms.TextBox();
      this.label18 = new System.Windows.Forms.Label();
      this.txtNameEn = new System.Windows.Forms.TextBox();
      this.label16 = new System.Windows.Forms.Label();
      this.txtMaterial = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btnSearch = new System.Windows.Forms.Button();
      this.utlSuppList = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnNew = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.ultraCBWO = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.label2 = new System.Windows.Forms.Label();
      this.btnUnLock = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.utlSuppList)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBWO)).BeginInit();
      this.SuspendLayout();
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Location = new System.Drawing.Point(858, 510);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 3;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // txtSuppCode
      // 
      this.txtSuppCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSuppCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtSuppCode.Location = new System.Drawing.Point(141, 19);
      this.txtSuppCode.Name = "txtSuppCode";
      this.txtSuppCode.Size = new System.Drawing.Size(788, 20);
      this.txtSuppCode.TabIndex = 0;
      this.txtSuppCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label18.Location = new System.Drawing.Point(9, 20);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(84, 13);
      this.label18.TabIndex = 26;
      this.label18.Text = "Support Code";
      // 
      // txtNameEn
      // 
      this.txtNameEn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtNameEn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtNameEn.Location = new System.Drawing.Point(141, 43);
      this.txtNameEn.Name = "txtNameEn";
      this.txtNameEn.Size = new System.Drawing.Size(788, 20);
      this.txtNameEn.TabIndex = 1;
      this.txtNameEn.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label16.Location = new System.Drawing.Point(9, 45);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(115, 13);
      this.label16.TabIndex = 28;
      this.label16.Text = "Description EN/VN";
      // 
      // txtMaterial
      // 
      this.txtMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtMaterial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtMaterial.Location = new System.Drawing.Point(141, 66);
      this.txtMaterial.Name = "txtMaterial";
      this.txtMaterial.Size = new System.Drawing.Size(788, 20);
      this.txtMaterial.TabIndex = 2;
      this.txtMaterial.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.label1.Location = new System.Drawing.Point(10, 69);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(85, 13);
      this.label1.TabIndex = 30;
      this.label1.Text = "Material Code";
      // 
      // btnSearch
      // 
      this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnSearch.Location = new System.Drawing.Point(854, 117);
      this.btnSearch.Name = "btnSearch";
      this.btnSearch.Size = new System.Drawing.Size(75, 23);
      this.btnSearch.TabIndex = 3;
      this.btnSearch.Text = "Search";
      this.btnSearch.UseVisualStyleBackColor = true;
      this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
      // 
      // utlSuppList
      // 
      this.utlSuppList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.utlSuppList.Cursor = System.Windows.Forms.Cursors.Default;
      this.utlSuppList.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      this.utlSuppList.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.utlSuppList.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.utlSuppList.Location = new System.Drawing.Point(7, 157);
      this.utlSuppList.Name = "utlSuppList";
      this.utlSuppList.Size = new System.Drawing.Size(930, 346);
      this.utlSuppList.TabIndex = 1;
      this.utlSuppList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.utlSuppList_MouseDoubleClick);
      this.utlSuppList.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.utlSuppList_InitializeLayout);
      // 
      // btnNew
      // 
      this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnNew.AutoSize = true;
      this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNew.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.btnNew.Location = new System.Drawing.Point(762, 510);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(90, 23);
      this.btnNew.TabIndex = 2;
      this.btnNew.Text = "New Support";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add(this.ultraCBWO);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.txtNameEn);
      this.groupBox1.Controls.Add(this.label18);
      this.groupBox1.Controls.Add(this.txtSuppCode);
      this.groupBox1.Controls.Add(this.btnSearch);
      this.groupBox1.Controls.Add(this.label16);
      this.groupBox1.Controls.Add(this.txtMaterial);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Location = new System.Drawing.Point(4, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(936, 148);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Search Information";
      // 
      // ultraCBWO
      // 
      this.ultraCBWO.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.ultraCBWO.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBWO.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBWO.DisplayMember = "";
      this.ultraCBWO.Location = new System.Drawing.Point(141, 90);
      this.ultraCBWO.Name = "ultraCBWO";
      this.ultraCBWO.Size = new System.Drawing.Size(788, 21);
      this.ultraCBWO.TabIndex = 32;
      this.ultraCBWO.ValueMember = "";
      this.ultraCBWO.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(10, 98);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(28, 13);
      this.label2.TabIndex = 31;
      this.label2.Text = "WO";
      // 
      // btnUnLock
      // 
      this.btnUnLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnUnLock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnUnLock.Location = new System.Drawing.Point(681, 510);
      this.btnUnLock.Name = "btnUnLock";
      this.btnUnLock.Size = new System.Drawing.Size(75, 23);
      this.btnUnLock.TabIndex = 4;
      this.btnUnLock.Text = "UnLock";
      this.btnUnLock.UseVisualStyleBackColor = true;
      this.btnUnLock.Click += new System.EventHandler(this.btnUnLock_Click);
      // 
      // viewBOM_03_005
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.btnUnLock);
      this.Controls.Add(this.utlSuppList);
      this.Controls.Add(this.btnNew);
      this.Controls.Add(this.btnClose);
      this.Name = "viewBOM_03_005";
      this.Size = new System.Drawing.Size(943, 540);
      this.Load += new System.EventHandler(this.viewBOM_03_005_Load);
      ((System.ComponentModel.ISupportInitialize)(this.utlSuppList)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBWO)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.TextBox txtSuppCode;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.TextBox txtNameEn;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.TextBox txtMaterial;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnSearch;
    private Infragistics.Win.UltraWinGrid.UltraGrid utlSuppList;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btnUnLock;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBWO;
    private System.Windows.Forms.Label label2;
  }
}
