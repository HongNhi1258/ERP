namespace MainBOM.AUTHENTICATE
{
  partial class frmSelectUser
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
      this.cmbEmployee = new System.Windows.Forms.ComboBox();
      this.cmbDepartment = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.lbDepartment = new System.Windows.Forms.Label();
      this.btnOK = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // cmbEmployee
      // 
      this.cmbEmployee.DisplayMember = "EmpName";
      this.cmbEmployee.FormattingEnabled = true;
      this.cmbEmployee.Location = new System.Drawing.Point(77, 36);
      this.cmbEmployee.Name = "cmbEmployee";
      this.cmbEmployee.Size = new System.Drawing.Size(217, 21);
      this.cmbEmployee.TabIndex = 3;
      this.cmbEmployee.ValueMember = "Pid";
      // 
      // cmbDepartment
      // 
      this.cmbDepartment.DisplayMember = "DeparmentName";
      this.cmbDepartment.FormattingEnabled = true;
      this.cmbDepartment.Location = new System.Drawing.Point(77, 12);
      this.cmbDepartment.Name = "cmbDepartment";
      this.cmbDepartment.Size = new System.Drawing.Size(217, 21);
      this.cmbDepartment.TabIndex = 2;
      this.cmbDepartment.ValueMember = "Department";
      this.cmbDepartment.SelectedIndexChanged += new System.EventHandler(this.cmbDepartment_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 39);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(53, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Employee";
      // 
      // lbDepartment
      // 
      this.lbDepartment.AutoSize = true;
      this.lbDepartment.Location = new System.Drawing.Point(12, 15);
      this.lbDepartment.Name = "lbDepartment";
      this.lbDepartment.Size = new System.Drawing.Size(62, 13);
      this.lbDepartment.TabIndex = 4;
      this.lbDepartment.Text = "Department";
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(129, 60);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(75, 23);
      this.btnOK.TabIndex = 6;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // frmSelectUser
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(302, 88);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.lbDepartment);
      this.Controls.Add(this.cmbEmployee);
      this.Controls.Add(this.cmbDepartment);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmSelectUser";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Select User";
      this.Load += new System.EventHandler(this.frmSelectUser_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox cmbEmployee;
    private System.Windows.Forms.ComboBox cmbDepartment;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lbDepartment;
    private System.Windows.Forms.Button btnOK;
  }
}