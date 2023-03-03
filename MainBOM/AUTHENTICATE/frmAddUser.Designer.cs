namespace MainBOM.AUTHENTICATE
{
  partial class frmAddUser
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
      this.lbDepartment = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.lbUserName = new System.Windows.Forms.Label();
      this.txtUser = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.ultraCBDepartment = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.ultraCBEmployee = new Infragistics.Win.UltraWinGrid.UltraCombo();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBDepartment)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBEmployee)).BeginInit();
      this.SuspendLayout();
      // 
      // lbDepartment
      // 
      this.lbDepartment.AutoSize = true;
      this.lbDepartment.Location = new System.Drawing.Point(12, 9);
      this.lbDepartment.Name = "lbDepartment";
      this.lbDepartment.Size = new System.Drawing.Size(62, 13);
      this.lbDepartment.TabIndex = 0;
      this.lbDepartment.Text = "Department";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 33);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(53, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Employee";
      // 
      // lbUserName
      // 
      this.lbUserName.AutoSize = true;
      this.lbUserName.Location = new System.Drawing.Point(12, 58);
      this.lbUserName.Name = "lbUserName";
      this.lbUserName.Size = new System.Drawing.Size(57, 13);
      this.lbUserName.TabIndex = 4;
      this.lbUserName.Text = "UserName";
      // 
      // txtUser
      // 
      this.txtUser.Location = new System.Drawing.Point(80, 55);
      this.txtUser.Name = "txtUser";
      this.txtUser.Size = new System.Drawing.Size(240, 20);
      this.txtUser.TabIndex = 5;
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(164, 81);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 6;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(245, 81);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 7;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // ultraCBDepartment
      // 
      this.ultraCBDepartment.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBDepartment.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBDepartment.DisplayLayout.AutoFitColumns = true;
      this.ultraCBDepartment.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBDepartment.DisplayMember = "";
      this.ultraCBDepartment.Location = new System.Drawing.Point(80, 5);
      this.ultraCBDepartment.Name = "ultraCBDepartment";
      this.ultraCBDepartment.Size = new System.Drawing.Size(240, 21);
      this.ultraCBDepartment.TabIndex = 1;
      this.ultraCBDepartment.ValueMember = "";
      this.ultraCBDepartment.ValueChanged += new System.EventHandler(this.ultraCBDepartment_ValueChanged);
      // 
      // ultraCBEmployee
      // 
      this.ultraCBEmployee.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
      this.ultraCBEmployee.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultraCBEmployee.DisplayLayout.AutoFitColumns = true;
      this.ultraCBEmployee.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultraCBEmployee.DisplayMember = "";
      this.ultraCBEmployee.Location = new System.Drawing.Point(80, 30);
      this.ultraCBEmployee.Name = "ultraCBEmployee";
      this.ultraCBEmployee.Size = new System.Drawing.Size(240, 21);
      this.ultraCBEmployee.TabIndex = 3;
      this.ultraCBEmployee.ValueMember = "";
      this.ultraCBEmployee.ValueChanged += new System.EventHandler(this.ultraCBEmployee_ValueChanged);
      // 
      // frmAddUser
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(332, 111);
      this.Controls.Add(this.ultraCBEmployee);
      this.Controls.Add(this.ultraCBDepartment);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.txtUser);
      this.Controls.Add(this.lbUserName);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.lbDepartment);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmAddUser";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Add User";
      this.Load += new System.EventHandler(this.frmAddUser_Load);
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBDepartment)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ultraCBEmployee)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lbDepartment;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lbUserName;
    private System.Windows.Forms.TextBox txtUser;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBDepartment;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultraCBEmployee;
  }
}