namespace DaiCo.General
{
    partial class frmLoginChat
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
            if ( disposing && ( components != null ) )
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
          this.txtUsetName = new Proshot.UtilityLib.TextBox();
          this.btnEnter = new Proshot.UtilityLib.Button();
          this.btnExit = new Proshot.UtilityLib.Button();
          this.SuspendLayout();
          // 
          // txtUsetName
          // 
          this.txtUsetName.BorderWidth = 1F;
          this.txtUsetName.FloatValue = 0;
          this.txtUsetName.Location = new System.Drawing.Point(-1, -1);
          this.txtUsetName.MaxLength = 10;
          this.txtUsetName.Name = "txtUsetName";
          this.txtUsetName.ReadOnly = true;
          this.txtUsetName.Size = new System.Drawing.Size(214, 22);
          this.txtUsetName.TabIndex = 1;
          // 
          // btnEnter
          // 
          this.btnEnter.Location = new System.Drawing.Point(-1, 27);
          this.btnEnter.Name = "btnEnter";
          this.btnEnter.Size = new System.Drawing.Size(10, 10);
          this.btnEnter.TabIndex = 2;
          this.btnEnter.Text = "Enter";
          this.btnEnter.UseVisualStyleBackColor = true;
          this.btnEnter.Visible = false;
          this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
          // 
          // btnExit
          // 
          this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
          this.btnExit.Location = new System.Drawing.Point(-1, 27);
          this.btnExit.Name = "btnExit";
          this.btnExit.Size = new System.Drawing.Size(10, 10);
          this.btnExit.TabIndex = 3;
          this.btnExit.Text = "Exit";
          this.btnExit.UseVisualStyleBackColor = true;
          this.btnExit.Visible = false;
          this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
          // 
          // frmLoginChat
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.BackColor = System.Drawing.Color.White;
          this.ClientSize = new System.Drawing.Size(8, 8);
          this.ControlBox = false;
          this.Controls.Add(this.btnExit);
          this.Controls.Add(this.btnEnter);
          this.Controls.Add(this.txtUsetName);
          this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
          this.Name = "frmLoginChat";
          this.RightToLeft = System.Windows.Forms.RightToLeft.No;
          this.ShowIcon = false;
          this.ShowInTaskbar = false;
          this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
          this.Load += new System.EventHandler(this.frmLoginChat_Load);
          this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogin_FormClosing);
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private Proshot.UtilityLib.TextBox txtUsetName;
        private Proshot.UtilityLib.Button btnEnter;
        private Proshot.UtilityLib.Button btnExit;

    }
}