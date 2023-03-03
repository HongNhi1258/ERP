namespace MainBOM
{
  partial class frmChangePassword
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
      this.label1 = new System.Windows.Forms.Label();
      this.txtCurrentPass = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txtNewPass = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtConfirmPass = new System.Windows.Forms.TextBox();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnChange = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 11);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(89, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Current password";
      // 
      // txtCurrentPass
      // 
      this.txtCurrentPass.Location = new System.Drawing.Point(107, 8);
      this.txtCurrentPass.Name = "txtCurrentPass";
      this.txtCurrentPass.PasswordChar = '*';
      this.txtCurrentPass.Size = new System.Drawing.Size(176, 20);
      this.txtCurrentPass.TabIndex = 0;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(77, 13);
      this.label2.TabIndex = 0;
      this.label2.Text = "New password";
      // 
      // txtNewPass
      // 
      this.txtNewPass.Location = new System.Drawing.Point(107, 31);
      this.txtNewPass.Name = "txtNewPass";
      this.txtNewPass.PasswordChar = '*';
      this.txtNewPass.Size = new System.Drawing.Size(176, 20);
      this.txtNewPass.TabIndex = 1;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 57);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(90, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "Confirm password";
      // 
      // txtConfirmPass
      // 
      this.txtConfirmPass.Location = new System.Drawing.Point(107, 54);
      this.txtConfirmPass.Name = "txtConfirmPass";
      this.txtConfirmPass.PasswordChar = '*';
      this.txtConfirmPass.Size = new System.Drawing.Size(176, 20);
      this.txtConfirmPass.TabIndex = 2;
      // 
      // btnCancel
      // 
      this.btnCancel.Location = new System.Drawing.Point(208, 79);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 4;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnChange
      // 
      this.btnChange.Location = new System.Drawing.Point(127, 79);
      this.btnChange.Name = "btnChange";
      this.btnChange.Size = new System.Drawing.Size(75, 23);
      this.btnChange.TabIndex = 3;
      this.btnChange.Text = "Change";
      this.btnChange.UseVisualStyleBackColor = true;
      this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
      // 
      // frmChangePassword
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(295, 112);
      this.Controls.Add(this.btnChange);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.txtConfirmPass);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.txtNewPass);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txtCurrentPass);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmChangePassword";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Change Password";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtCurrentPass;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtNewPass;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtConfirmPass;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnChange;
  }
}