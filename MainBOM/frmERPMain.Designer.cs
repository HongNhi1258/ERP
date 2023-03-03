namespace MainBOM
{
  partial class frmERPMain
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmERPMain));
      this.mainMenu = new System.Windows.Forms.MenuStrip();
      this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuLogin = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuLogout = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuChangePass = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuAuthenticate = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuAuthenticateControls = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuExist = new System.Windows.Forms.ToolStripMenuItem();
      this.foundryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.wIPWarehouseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.issueForProductionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.returnToWarehouseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.tvModulController = new System.Windows.Forms.TreeView();
      this.imageList_Controller = new System.Windows.Forms.ImageList(this.components);
      this.pictureBox2 = new System.Windows.Forms.PictureBox();
      this.tlpSearchFunction = new System.Windows.Forms.TableLayoutPanel();
      this.ultCBFunction = new Infragistics.Win.UltraWinGrid.UltraCombo();
      this.lblShowSearch = new System.Windows.Forms.Label();
      this.mnuAuthenticateViews = new System.Windows.Forms.ToolStripMenuItem();
      this.uPanelTop = new Infragistics.Win.Misc.UltraPanel();
      this.uSplitterTop = new Infragistics.Win.Misc.UltraSplitter();
      this.uPanelLeft = new Infragistics.Win.Misc.UltraPanel();
      this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
      this.uPanelCenter = new Infragistics.Win.Misc.UltraPanel();
      this.udaMessageReminder = new Infragistics.Win.Misc.UltraDesktopAlert(this.components);
      this.mainMenu.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
      this.tlpSearchFunction.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBFunction)).BeginInit();
      this.uPanelTop.ClientArea.SuspendLayout();
      this.uPanelTop.SuspendLayout();
      this.uPanelLeft.ClientArea.SuspendLayout();
      this.uPanelLeft.SuspendLayout();
      this.uPanelCenter.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.udaMessageReminder)).BeginInit();
      this.SuspendLayout();
      // 
      // mainMenu
      // 
      this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemToolStripMenuItem,
            this.foundryToolStripMenuItem});
      this.mainMenu.Location = new System.Drawing.Point(0, 0);
      this.mainMenu.Name = "mainMenu";
      this.mainMenu.Size = new System.Drawing.Size(1170, 24);
      this.mainMenu.TabIndex = 0;
      this.mainMenu.Text = "menuStrip1";
      // 
      // systemToolStripMenuItem
      // 
      this.systemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLogin,
            this.mnuLogout,
            this.mnuChangePass,
            this.mnuAuthenticate,
            this.mnuAuthenticateControls,
            this.mnuExist});
      this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
      this.systemToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
      this.systemToolStripMenuItem.Text = "System";
      // 
      // mnuLogin
      // 
      this.mnuLogin.Name = "mnuLogin";
      this.mnuLogin.Size = new System.Drawing.Size(142, 22);
      this.mnuLogin.Text = "Login";
      this.mnuLogin.Click += new System.EventHandler(this.mnuLogin_Click);
      // 
      // mnuLogout
      // 
      this.mnuLogout.Name = "mnuLogout";
      this.mnuLogout.Size = new System.Drawing.Size(142, 22);
      this.mnuLogout.Text = "Logout";
      this.mnuLogout.Click += new System.EventHandler(this.mnuLogout_Click);
      // 
      // mnuChangePass
      // 
      this.mnuChangePass.Name = "mnuChangePass";
      this.mnuChangePass.Size = new System.Drawing.Size(142, 22);
      this.mnuChangePass.Text = "Change Pass";
      this.mnuChangePass.Click += new System.EventHandler(this.mnuChangePass_Click);
      // 
      // mnuAuthenticate
      // 
      this.mnuAuthenticate.Name = "mnuAuthenticate";
      this.mnuAuthenticate.Size = new System.Drawing.Size(142, 22);
      this.mnuAuthenticate.Text = "Authenticate";
      this.mnuAuthenticate.Click += new System.EventHandler(this.mnuAuthenticate_Click);
      // 
      // mnuAuthenticateControls
      // 
      this.mnuAuthenticateControls.Name = "mnuAuthenticateControls";
      this.mnuAuthenticateControls.Size = new System.Drawing.Size(142, 22);
      this.mnuAuthenticateControls.Text = "Controls";
      this.mnuAuthenticateControls.Click += new System.EventHandler(this.mnuAuthenticateControls_Click);
      // 
      // mnuExist
      // 
      this.mnuExist.Name = "mnuExist";
      this.mnuExist.Size = new System.Drawing.Size(142, 22);
      this.mnuExist.Text = "Exit";
      this.mnuExist.Click += new System.EventHandler(this.mnuExit_Click);
      // 
      // foundryToolStripMenuItem
      // 
      this.foundryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wIPWarehouseToolStripMenuItem});
      this.foundryToolStripMenuItem.Name = "foundryToolStripMenuItem";
      this.foundryToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
      this.foundryToolStripMenuItem.Text = "Foundry";
      this.foundryToolStripMenuItem.Visible = false;
      // 
      // wIPWarehouseToolStripMenuItem
      // 
      this.wIPWarehouseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.issueForProductionToolStripMenuItem,
            this.returnToWarehouseToolStripMenuItem});
      this.wIPWarehouseToolStripMenuItem.Name = "wIPWarehouseToolStripMenuItem";
      this.wIPWarehouseToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
      this.wIPWarehouseToolStripMenuItem.Text = "WIP Warehouse";
      // 
      // issueForProductionToolStripMenuItem
      // 
      this.issueForProductionToolStripMenuItem.Name = "issueForProductionToolStripMenuItem";
      this.issueForProductionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.U)));
      this.issueForProductionToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
      this.issueForProductionToolStripMenuItem.Text = "Issue for Production";
      this.issueForProductionToolStripMenuItem.Click += new System.EventHandler(this.issueForProductionToolStripMenuItem_Click);
      // 
      // returnToWarehouseToolStripMenuItem
      // 
      this.returnToWarehouseToolStripMenuItem.Name = "returnToWarehouseToolStripMenuItem";
      this.returnToWarehouseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
      this.returnToWarehouseToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
      this.returnToWarehouseToolStripMenuItem.Text = "Return to Warehouse";
      this.returnToWarehouseToolStripMenuItem.Click += new System.EventHandler(this.returnToWarehouseToolStripMenuItem_Click);
      // 
      // tvModulController
      // 
      this.tvModulController.BackColor = System.Drawing.Color.White;
      this.tvModulController.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tvModulController.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tvModulController.ForeColor = System.Drawing.Color.Black;
      this.tvModulController.ImageIndex = 0;
      this.tvModulController.ImageList = this.imageList_Controller;
      this.tvModulController.Location = new System.Drawing.Point(0, 0);
      this.tvModulController.Margin = new System.Windows.Forms.Padding(0);
      this.tvModulController.Name = "tvModulController";
      this.tvModulController.SelectedImageIndex = 0;
      this.tvModulController.Size = new System.Drawing.Size(280, 619);
      this.tvModulController.TabIndex = 1;
      this.tvModulController.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvModulController_NodeMouseClick);
      this.tvModulController.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvModulController_NodeMouseDoubleClick);
      this.tvModulController.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvModulController_KeyUp);
      // 
      // imageList_Controller
      // 
      this.imageList_Controller.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Controller.ImageStream")));
      this.imageList_Controller.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList_Controller.Images.SetKeyName(0, "folder.ico");
      this.imageList_Controller.Images.SetKeyName(1, "folderopen.ico");
      this.imageList_Controller.Images.SetKeyName(2, "AddRoot.ico");
      // 
      // pictureBox2
      // 
      this.pictureBox2.BackColor = System.Drawing.SystemColors.Window;
      this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox2.Image = global::MainBOM.Properties.Resources.ERPBackground;
      this.pictureBox2.Location = new System.Drawing.Point(0, 0);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new System.Drawing.Size(883, 617);
      this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pictureBox2.TabIndex = 2;
      this.pictureBox2.TabStop = false;
      // 
      // tlpSearchFunction
      // 
      this.tlpSearchFunction.ColumnCount = 2;
      this.tlpSearchFunction.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlpSearchFunction.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpSearchFunction.Controls.Add(this.ultCBFunction, 1, 0);
      this.tlpSearchFunction.Controls.Add(this.lblShowSearch, 0, 0);
      this.tlpSearchFunction.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.tlpSearchFunction.Location = new System.Drawing.Point(0, 28);
      this.tlpSearchFunction.Name = "tlpSearchFunction";
      this.tlpSearchFunction.RowCount = 1;
      this.tlpSearchFunction.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlpSearchFunction.Size = new System.Drawing.Size(1170, 28);
      this.tlpSearchFunction.TabIndex = 2;
      // 
      // ultCBFunction
      // 
      this.ultCBFunction.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      this.ultCBFunction.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
      this.ultCBFunction.Cursor = System.Windows.Forms.Cursors.Default;
      this.ultCBFunction.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      this.ultCBFunction.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      this.ultCBFunction.DisplayLayout.UseFixedHeaders = true;
      this.ultCBFunction.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ultCBFunction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ultCBFunction.Location = new System.Drawing.Point(93, 2);
      this.ultCBFunction.Margin = new System.Windows.Forms.Padding(2);
      this.ultCBFunction.Name = "ultCBFunction";
      this.ultCBFunction.Size = new System.Drawing.Size(1075, 24);
      this.ultCBFunction.TabIndex = 2;
      this.ultCBFunction.Enter += new System.EventHandler(this.ultCBFunction_Enter);
      this.ultCBFunction.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ultCBFunction_KeyDown);
      // 
      // lblShowSearch
      // 
      this.lblShowSearch.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.lblShowSearch.AutoSize = true;
      this.lblShowSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblShowSearch.Location = new System.Drawing.Point(3, 7);
      this.lblShowSearch.Name = "lblShowSearch";
      this.lblShowSearch.Size = new System.Drawing.Size(85, 13);
      this.lblShowSearch.TabIndex = 3;
      this.lblShowSearch.Text = "Search Function";
      // 
      // mnuAuthenticateViews
      // 
      this.mnuAuthenticateViews.Name = "mnuAuthenticateViews";
      this.mnuAuthenticateViews.Size = new System.Drawing.Size(152, 22);
      this.mnuAuthenticateViews.Text = "Views";
      // 
      // uPanelTop
      // 
      // 
      // uPanelTop.ClientArea
      // 
      this.uPanelTop.ClientArea.Controls.Add(this.mainMenu);
      this.uPanelTop.ClientArea.Controls.Add(this.tlpSearchFunction);
      this.uPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
      this.uPanelTop.Location = new System.Drawing.Point(0, 0);
      this.uPanelTop.MaximumSize = new System.Drawing.Size(2000, 56);
      this.uPanelTop.Name = "uPanelTop";
      this.uPanelTop.Size = new System.Drawing.Size(1170, 56);
      this.uPanelTop.TabIndex = 3;
      // 
      // uSplitterTop
      // 
      this.uSplitterTop.BackColor = System.Drawing.SystemColors.Control;
      this.uSplitterTop.Dock = System.Windows.Forms.DockStyle.Top;
      this.uSplitterTop.Location = new System.Drawing.Point(0, 56);
      this.uSplitterTop.Name = "uSplitterTop";
      this.uSplitterTop.RestoreExtent = 100;
      this.uSplitterTop.Size = new System.Drawing.Size(1170, 6);
      this.uSplitterTop.TabIndex = 4;
      // 
      // uPanelLeft
      // 
      // 
      // uPanelLeft.ClientArea
      // 
      this.uPanelLeft.ClientArea.Controls.Add(this.tvModulController);
      this.uPanelLeft.Dock = System.Windows.Forms.DockStyle.Left;
      this.uPanelLeft.Location = new System.Drawing.Point(0, 62);
      this.uPanelLeft.Name = "uPanelLeft";
      this.uPanelLeft.Size = new System.Drawing.Size(280, 619);
      this.uPanelLeft.TabIndex = 5;
      // 
      // ultraSplitter1
      // 
      this.ultraSplitter1.Location = new System.Drawing.Point(280, 62);
      this.ultraSplitter1.Name = "ultraSplitter1";
      this.ultraSplitter1.RestoreExtent = 0;
      this.ultraSplitter1.Size = new System.Drawing.Size(10, 619);
      this.ultraSplitter1.TabIndex = 6;
      // 
      // uPanelCenter
      // 
      this.uPanelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.uPanelCenter.Location = new System.Drawing.Point(290, 62);
      this.uPanelCenter.Name = "uPanelCenter";
      this.uPanelCenter.Size = new System.Drawing.Size(880, 619);
      this.uPanelCenter.TabIndex = 7;
      // 
      // frmERPMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1170, 681);
      this.Controls.Add(this.uPanelCenter);
      this.Controls.Add(this.ultraSplitter1);
      this.Controls.Add(this.uPanelLeft);
      this.Controls.Add(this.uSplitterTop);
      this.Controls.Add(this.uPanelTop);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.mainMenu;
      this.Name = "frmERPMain";
      this.Text = "ERP System";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      
      this.Load += new System.EventHandler(this.frmERPMain_Load);
      this.mainMenu.ResumeLayout(false);
      this.mainMenu.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
      this.tlpSearchFunction.ResumeLayout(false);
      this.tlpSearchFunction.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.ultCBFunction)).EndInit();
      this.uPanelTop.ClientArea.ResumeLayout(false);
      this.uPanelTop.ClientArea.PerformLayout();
      this.uPanelTop.ResumeLayout(false);
      this.uPanelLeft.ClientArea.ResumeLayout(false);
      this.uPanelLeft.ResumeLayout(false);
      this.uPanelCenter.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.udaMessageReminder)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.MenuStrip mainMenu;
    private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mnuLogin;
    private System.Windows.Forms.ToolStripMenuItem mnuChangePass;
    private System.Windows.Forms.ToolStripMenuItem mnuExist;
    public System.Windows.Forms.TreeView tvModulController;
    private System.Windows.Forms.ImageList imageList_Controller;
    private System.Windows.Forms.PictureBox pictureBox2;
    private System.Windows.Forms.ToolStripMenuItem mnuAuthenticate;
    private System.Windows.Forms.ToolStripMenuItem mnuLogout;
    private System.Windows.Forms.ToolStripMenuItem mnuAuthenticateViews;
    private System.Windows.Forms.ToolStripMenuItem mnuAuthenticateControls;
    private System.Windows.Forms.ToolStripMenuItem foundryToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem wIPWarehouseToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem issueForProductionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem returnToWarehouseToolStripMenuItem;
    private System.Windows.Forms.TableLayoutPanel tlpSearchFunction;
    private Infragistics.Win.UltraWinGrid.UltraCombo ultCBFunction;
    private System.Windows.Forms.Label lblShowSearch;
    private Infragistics.Win.Misc.UltraPanel uPanelTop;
    private Infragistics.Win.Misc.UltraSplitter uSplitterTop;
    private Infragistics.Win.Misc.UltraPanel uPanelLeft;
    private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
    private Infragistics.Win.Misc.UltraPanel uPanelCenter;
    private Infragistics.Win.Misc.UltraDesktopAlert udaMessageReminder;
  }
}