namespace MainBOM.AUTHENTICATE
{
  partial class frmNodeProperties
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.txtOderBy = new System.Windows.Forms.TextBox();
      this.chxIsActive = new System.Windows.Forms.CheckBox();
      this.cmbViewState = new System.Windows.Forms.ComboBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.txtOrtherInfo = new System.Windows.Forms.TextBox();
      this.txtDescription = new System.Windows.Forms.TextBox();
      this.txtTitle = new System.Windows.Forms.TextBox();
      this.txtUICode = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.label8 = new System.Windows.Forms.Label();
      this.cmbWindowState = new System.Windows.Forms.ComboBox();
      this.label10 = new System.Windows.Forms.Label();
      this.cmbNameSpace = new System.Windows.Forms.ComboBox();
      this.label11 = new System.Windows.Forms.Label();
      this.txtUIParam = new System.Windows.Forms.TextBox();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnDelete = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtOderBy
      // 
      this.txtOderBy.Location = new System.Drawing.Point(78, 194);
      this.txtOderBy.MaxLength = 2;
      this.txtOderBy.Name = "txtOderBy";
      this.txtOderBy.Size = new System.Drawing.Size(105, 20);
      this.txtOderBy.TabIndex = 18;
      this.txtOderBy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // chxIsActive
      // 
      this.chxIsActive.AutoSize = true;
      this.chxIsActive.Location = new System.Drawing.Point(234, 197);
      this.chxIsActive.Name = "chxIsActive";
      this.chxIsActive.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.chxIsActive.Size = new System.Drawing.Size(67, 17);
      this.chxIsActive.TabIndex = 19;
      this.chxIsActive.Text = "Is Active";
      this.chxIsActive.UseVisualStyleBackColor = true;
      // 
      // cmbViewState
      // 
      this.cmbViewState.FormattingEnabled = true;
      this.cmbViewState.Items.AddRange(new object[] {
            "",
            "Main Window",
            "Modal Window",
            "Window"});
      this.cmbViewState.Location = new System.Drawing.Point(78, 78);
      this.cmbViewState.Name = "cmbViewState";
      this.cmbViewState.Size = new System.Drawing.Size(105, 21);
      this.cmbViewState.TabIndex = 10;
      this.cmbViewState.SelectedIndexChanged += new System.EventHandler(this.cmbViewState_SelectedIndexChanged);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(8, 82);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(58, 13);
      this.label6.TabIndex = 9;
      this.label6.Text = "View State";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(8, 130);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(53, 13);
      this.label5.TabIndex = 15;
      this.label5.Text = "Other info";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(8, 107);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(60, 13);
      this.label4.TabIndex = 13;
      this.label4.Text = "Description";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(8, 56);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(27, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Title";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(8, 31);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(64, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Namespace";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(8, 197);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(45, 13);
      this.label7.TabIndex = 17;
      this.label7.Text = "Oder By";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(8, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(43, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "UICode";
      // 
      // txtOrtherInfo
      // 
      this.txtOrtherInfo.Location = new System.Drawing.Point(78, 127);
      this.txtOrtherInfo.Multiline = true;
      this.txtOrtherInfo.Name = "txtOrtherInfo";
      this.txtOrtherInfo.Size = new System.Drawing.Size(319, 63);
      this.txtOrtherInfo.TabIndex = 16;
      // 
      // txtDescription
      // 
      this.txtDescription.Location = new System.Drawing.Point(78, 103);
      this.txtDescription.MaxLength = 256;
      this.txtDescription.Name = "txtDescription";
      this.txtDescription.Size = new System.Drawing.Size(319, 20);
      this.txtDescription.TabIndex = 14;
      // 
      // txtTitle
      // 
      this.txtTitle.Location = new System.Drawing.Point(78, 53);
      this.txtTitle.MaxLength = 128;
      this.txtTitle.Name = "txtTitle";
      this.txtTitle.Size = new System.Drawing.Size(319, 20);
      this.txtTitle.TabIndex = 8;
      // 
      // txtUICode
      // 
      this.txtUICode.Location = new System.Drawing.Point(78, 4);
      this.txtUICode.MaxLength = 128;
      this.txtUICode.Name = "txtUICode";
      this.txtUICode.Size = new System.Drawing.Size(127, 20);
      this.txtUICode.TabIndex = 1;
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(160, 223);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 20;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(189, 82);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(74, 13);
      this.label8.TabIndex = 11;
      this.label8.Text = "Window State";
      // 
      // cmbWindowState
      // 
      this.cmbWindowState.FormattingEnabled = true;
      this.cmbWindowState.Items.AddRange(new object[] {
            "",
            "Normal",
            "Minimized",
            "Maximized"});
      this.cmbWindowState.Location = new System.Drawing.Point(269, 78);
      this.cmbWindowState.Name = "cmbWindowState";
      this.cmbWindowState.Size = new System.Drawing.Size(128, 21);
      this.cmbWindowState.TabIndex = 12;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.ForeColor = System.Drawing.Color.Red;
      this.label10.Location = new System.Drawing.Point(62, 57);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(14, 16);
      this.label10.TabIndex = 7;
      this.label10.Text = "*";
      // 
      // cmbNameSpace
      // 
      this.cmbNameSpace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbNameSpace.FormattingEnabled = true;
      this.cmbNameSpace.Items.AddRange(new object[] {
            "",
            "ResearchAndDesign",
            "Technical",
            "Planning",
            "CustomerService",
            "Accounting",
            "QualityControl",
            "EXIM",
            "Warehouse",
            "Purchasing",
            "Foundry",
            "Packing",
            "VeneerWarehouse",
            "General",
            "WorkInProccess",
            "FinishGoodWarehouse",
            "WoodsWarehouse",
            "MaterialWarehouse",
            "WorkInProcessCarcass",
            "SubCon",
            "HumanResource",
            "MainBOM",
            "ERPProject"});
      this.cmbNameSpace.Location = new System.Drawing.Point(78, 28);
      this.cmbNameSpace.Name = "cmbNameSpace";
      this.cmbNameSpace.Size = new System.Drawing.Size(319, 21);
      this.cmbNameSpace.TabIndex = 5;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(211, 8);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(51, 13);
      this.label11.TabIndex = 2;
      this.label11.Text = "UI Param";
      // 
      // txtUIParam
      // 
      this.txtUIParam.Location = new System.Drawing.Point(265, 4);
      this.txtUIParam.MaxLength = 256;
      this.txtUIParam.Name = "txtUIParam";
      this.txtUIParam.Size = new System.Drawing.Size(132, 20);
      this.txtUIParam.TabIndex = 3;
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(322, 223);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 22;
      this.btnClose.Text = "Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnDelete
      // 
      this.btnDelete.ForeColor = System.Drawing.Color.Red;
      this.btnDelete.Location = new System.Drawing.Point(241, 223);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(75, 23);
      this.btnDelete.TabIndex = 21;
      this.btnDelete.Text = "Delete";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // frmNodeProperties
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(401, 250);
      this.Controls.Add(this.btnDelete);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.cmbNameSpace);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.txtOderBy);
      this.Controls.Add(this.chxIsActive);
      this.Controls.Add(this.cmbWindowState);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.cmbViewState);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtOrtherInfo);
      this.Controls.Add(this.txtDescription);
      this.Controls.Add(this.txtTitle);
      this.Controls.Add(this.txtUIParam);
      this.Controls.Add(this.txtUICode);
      this.Controls.Add(this.btnSave);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmNodeProperties";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Node Properties";
      this.Load += new System.EventHandler(this.frmNodeProperties_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtOderBy;
    private System.Windows.Forms.CheckBox chxIsActive;
    private System.Windows.Forms.ComboBox cmbViewState;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtOrtherInfo;
    private System.Windows.Forms.TextBox txtDescription;
    private System.Windows.Forms.TextBox txtTitle;
    private System.Windows.Forms.TextBox txtUICode;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ComboBox cmbWindowState;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.ComboBox cmbNameSpace;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.TextBox txtUIParam;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Button btnDelete;
  }
}