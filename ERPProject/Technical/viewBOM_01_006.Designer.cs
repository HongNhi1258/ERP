namespace DaiCo.ERPProject
{
  partial class viewBOM_01_006
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
      this.components = new System.ComponentModel.Container();
      this.txtRevision = new System.Windows.Forms.TextBox();
      this.txtItemCode = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.drChangeKind = new DaiCo.Shared.DaiCoComboBox(this.components);
      this.label7 = new System.Windows.Forms.Label();
      this.txtNote = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtLinkFile = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnUpdate = new System.Windows.Forms.Button();
      this.txtPID = new System.Windows.Forms.TextBox();
      this.txtFilePathOld = new System.Windows.Forms.TextBox();
      this.dgvRevisionRecord = new Infragistics.Win.UltraWinGrid.UltraGrid();
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnBrowser = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.dgvRevisionRecord)).BeginInit();
      this.SuspendLayout();
      // 
      // txtRevision
      // 
      this.txtRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtRevision.Location = new System.Drawing.Point(100, 33);
      this.txtRevision.Name = "txtRevision";
      this.txtRevision.ReadOnly = true;
      this.txtRevision.Size = new System.Drawing.Size(671, 20);
      this.txtRevision.TabIndex = 1;
      // 
      // txtItemCode
      // 
      this.txtItemCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtItemCode.Location = new System.Drawing.Point(100, 9);
      this.txtItemCode.Name = "txtItemCode";
      this.txtItemCode.ReadOnly = true;
      this.txtItemCode.Size = new System.Drawing.Size(671, 20);
      this.txtItemCode.TabIndex = 0;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(4, 34);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(62, 15);
      this.label4.TabIndex = 25;
      this.label4.Text = "Revision";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(4, 10);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(72, 15);
      this.label3.TabIndex = 24;
      this.label3.Text = "Item Code";
      // 
      // drChangeKind
      // 
      this.drChangeKind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.drChangeKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.drChangeKind.FormattingEnabled = true;
      this.drChangeKind.Location = new System.Drawing.Point(100, 83);
      this.drChangeKind.Name = "drChangeKind";
      this.drChangeKind.Size = new System.Drawing.Size(671, 21);
      this.drChangeKind.TabIndex = 2;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(4, 84);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(90, 15);
      this.label7.TabIndex = 28;
      this.label7.Text = "Change Type";
      // 
      // txtNote
      // 
      this.txtNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtNote.Location = new System.Drawing.Point(100, 109);
      this.txtNote.Name = "txtNote";
      this.txtNote.Size = new System.Drawing.Size(671, 20);
      this.txtNote.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(4, 110);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(37, 15);
      this.label1.TabIndex = 29;
      this.label1.Text = "Note";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(4, 59);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(31, 15);
      this.label2.TabIndex = 31;
      this.label2.Text = "File";
      // 
      // txtLinkFile
      // 
      this.txtLinkFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtLinkFile.Enabled = false;
      this.txtLinkFile.Location = new System.Drawing.Point(100, 58);
      this.txtLinkFile.Name = "txtLinkFile";
      this.txtLinkFile.Size = new System.Drawing.Size(588, 20);
      this.txtLinkFile.TabIndex = 0;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::DaiCo.ERPProject.Properties.Resources.New;
      this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSave.Location = new System.Drawing.Point(534, 132);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 4;
      this.btnSave.Text = "    Insert";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnClose.Image = global::DaiCo.ERPProject.Properties.Resources.Close;
      this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClose.Location = new System.Drawing.Point(696, 483);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 8;
      this.btnClose.Text = "    Close";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
      // 
      // btnUpdate
      // 
      this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnUpdate.Enabled = false;
      this.btnUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnUpdate.Image = global::DaiCo.ERPProject.Properties.Resources.Save;
      this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnUpdate.Location = new System.Drawing.Point(615, 132);
      this.btnUpdate.Name = "btnUpdate";
      this.btnUpdate.Size = new System.Drawing.Size(75, 23);
      this.btnUpdate.TabIndex = 5;
      this.btnUpdate.Text = "    Update";
      this.btnUpdate.UseVisualStyleBackColor = true;
      this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
      // 
      // txtPID
      // 
      this.txtPID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPID.Location = new System.Drawing.Point(292, 132);
      this.txtPID.Name = "txtPID";
      this.txtPID.Size = new System.Drawing.Size(120, 20);
      this.txtPID.TabIndex = 39;
      this.txtPID.Visible = false;
      // 
      // txtFilePathOld
      // 
      this.txtFilePathOld.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFilePathOld.Location = new System.Drawing.Point(164, 132);
      this.txtFilePathOld.Name = "txtFilePathOld";
      this.txtFilePathOld.Size = new System.Drawing.Size(120, 20);
      this.txtFilePathOld.TabIndex = 40;
      this.txtFilePathOld.Visible = false;
      // 
      // dgvRevisionRecord
      // 
      this.dgvRevisionRecord.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dgvRevisionRecord.Cursor = System.Windows.Forms.Cursors.Default;
      this.dgvRevisionRecord.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.dgvRevisionRecord.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      this.dgvRevisionRecord.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.dgvRevisionRecord.Location = new System.Drawing.Point(4, 156);
      this.dgvRevisionRecord.Name = "dgvRevisionRecord";
      this.dgvRevisionRecord.Size = new System.Drawing.Size(768, 324);
      this.dgvRevisionRecord.TabIndex = 7;
      this.dgvRevisionRecord.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.dgvRevisionRecord_InitializeLayout);
      this.dgvRevisionRecord.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.dgvRevisionRecord_ClickCellButton);
      this.dgvRevisionRecord.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgvRevisionRecord_MouseDoubleClick);
      // 
      // btnDelete
      // 
      this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnDelete.Enabled = false;
      this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDelete.Image = global::DaiCo.ERPProject.Properties.Resources.Delete;
      this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnDelete.Location = new System.Drawing.Point(696, 132);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(75, 23);
      this.btnDelete.TabIndex = 6;
      this.btnDelete.Text = "    Delete";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // btnBrowser
      // 
      this.btnBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBrowser.Image = global::DaiCo.ERPProject.Properties.Resources.Browser;
      this.btnBrowser.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnBrowser.Location = new System.Drawing.Point(694, 56);
      this.btnBrowser.Name = "btnBrowser";
      this.btnBrowser.Size = new System.Drawing.Size(77, 23);
      this.btnBrowser.TabIndex = 1;
      this.btnBrowser.Text = "    Browser";
      this.btnBrowser.UseVisualStyleBackColor = true;
      this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
      // 
      // viewBOM_01_006
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dgvRevisionRecord);
      this.Controls.Add(this.txtFilePathOld);
      this.Controls.Add(this.txtPID);
      this.Controls.Add(this.btnDelete);
      this.Controls.Add(this.btnUpdate);
      this.Controls.Add(this.btnClose);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.btnBrowser);
      this.Controls.Add(this.txtLinkFile);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txtNote);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.drChangeKind);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.txtRevision);
      this.Controls.Add(this.txtItemCode);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Name = "viewBOM_01_006";
      this.Size = new System.Drawing.Size(777, 509);
      this.Load += new System.EventHandler(this.viewBOM_01_006_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dgvRevisionRecord)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRevision;
        private System.Windows.Forms.TextBox txtItemCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private DaiCo.Shared.DaiCoComboBox drChangeKind;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLinkFile;
      private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.Button btnUpdate;
      private System.Windows.Forms.Button btnDelete;
      private System.Windows.Forms.TextBox txtPID;
      private System.Windows.Forms.TextBox txtFilePathOld;
      private Infragistics.Win.UltraWinGrid.UltraGrid dgvRevisionRecord;
    }
}