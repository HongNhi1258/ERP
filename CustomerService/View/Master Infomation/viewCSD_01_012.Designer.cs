namespace DaiCo.CustomerService
{
    partial class viewCSD_01_012
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
          this.GVCodeMst = new System.Windows.Forms.DataGridView();
          this.cmbGroup = new System.Windows.Forms.ComboBox();
          this.label9 = new System.Windows.Forms.Label();
          this.btnClose = new System.Windows.Forms.Button();
          this.btnSave = new System.Windows.Forms.Button();
          ((System.ComponentModel.ISupportInitialize)(this.GVCodeMst)).BeginInit();
          this.SuspendLayout();
          // 
          // GVCodeMst
          // 
          this.GVCodeMst.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                      | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.GVCodeMst.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
          this.GVCodeMst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
          this.GVCodeMst.Location = new System.Drawing.Point(4, 48);
          this.GVCodeMst.Name = "GVCodeMst";
          this.GVCodeMst.Size = new System.Drawing.Size(600, 352);
          this.GVCodeMst.TabIndex = 2;
          this.GVCodeMst.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.GVCodeMst_UserDeletingRow);
          // 
          // cmbGroup
          // 
          this.cmbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
          this.cmbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
          this.cmbGroup.Enabled = false;
          this.cmbGroup.FormattingEnabled = true;
          this.cmbGroup.Items.AddRange(new object[] {
            "Waiting",
            "Finished"});
          this.cmbGroup.Location = new System.Drawing.Point(48, 16);
          this.cmbGroup.Name = "cmbGroup";
          this.cmbGroup.Size = new System.Drawing.Size(555, 21);
          this.cmbGroup.TabIndex = 1;
          this.cmbGroup.SelectedIndexChanged += new System.EventHandler(this.cmbGroup_SelectedIndexChanged);
          // 
          // label9
          // 
          this.label9.AutoSize = true;
          this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.label9.Location = new System.Drawing.Point(4, 20);
          this.label9.Name = "label9";
          this.label9.Size = new System.Drawing.Size(41, 13);
          this.label9.TabIndex = 24;
          this.label9.Text = "Group";
          // 
          // btnClose
          // 
          this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.btnClose.Location = new System.Drawing.Point(529, 405);
          this.btnClose.Name = "btnClose";
          this.btnClose.Size = new System.Drawing.Size(75, 23);
          this.btnClose.TabIndex = 4;
          this.btnClose.Text = "Close";
          this.btnClose.UseVisualStyleBackColor = true;
          this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
          // 
          // btnSave
          // 
          this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
          this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.btnSave.Location = new System.Drawing.Point(448, 405);
          this.btnSave.Name = "btnSave";
          this.btnSave.Size = new System.Drawing.Size(75, 23);
          this.btnSave.TabIndex = 3;
          this.btnSave.Text = "Save";
          this.btnSave.UseVisualStyleBackColor = true;
          this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
          // 
          // viewCSD_01_012
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.btnClose);
          this.Controls.Add(this.btnSave);
          this.Controls.Add(this.cmbGroup);
          this.Controls.Add(this.label9);
          this.Controls.Add(this.GVCodeMst);
          this.Name = "viewCSD_01_012";
          this.Size = new System.Drawing.Size(608, 433);
          this.Load += new System.EventHandler(this.viewCSD_01_012_Load);
          ((System.ComponentModel.ISupportInitialize)(this.GVCodeMst)).EndInit();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView GVCodeMst;
        private System.Windows.Forms.ComboBox cmbGroup;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Button btnSave;
    }
}