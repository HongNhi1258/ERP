using DaiCo.Application;
using DaiCo.ERPProject;
using DaiCo.Foundry;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MainBOM
{
    public partial class frmERPMain : Form
    {
        bool sliter = true;
        private long erpLog = long.MinValue;
        //private string companyName = "Lam Viet Furniture";
        private string companyName = "";
        
        public frmERPMain()
        {
            InitializeComponent();
        }

        void tvModulController_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowViewFromController();
            }
        }

        private void tvModulController_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ShowViewFromController();
        }

        private void ShowViewFromController()
        {
            MainTreeNode selectNode = (MainTreeNode)tvModulController.SelectedNode;
            if (selectNode != null && selectNode.Nodes.Count == 0)
            {
                DaiCo.Shared.Utility.SharedObject.tabContent.Visible = true;
                bool isNew = false;
                if (selectNode.Tag != null)
                {
                    isNew = (bool)selectNode.Tag;
                }
                //cay cu
                //DaiCo.Shared.MainUserControl uc = ModulController.GetUserControl(selectNode.Name);

                //cay moi
                DaiCo.Shared.MainUserControl uc = null;
                string typeName = "DaiCo." + selectNode.FullPathDomain + "." + selectNode.Name + "," + selectNode.FullPathDomain;
                Type tyView = Type.GetType(typeName, false, true);
                if (tyView != null)
                {
                    object objView = System.Activator.CreateInstance(tyView);
                    if (objView != null)
                    {
                        uc = (DaiCo.Shared.MainUserControl)objView;
                        uc.FullPath = selectNode.FullPath;
                        uc.ViewParam = selectNode.UIParam;
                    }
                }
                DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, selectNode.Text, false, selectNode.ViewState, selectNode.WindowState);
            }
        }

        private void frmERPMain_Load(object sender, EventArgs e)
        {
           
            // Check and update if having new version
            //string updatePath = System.Configuration.ConfigurationSettings.AppSettings["updatePath"];
            //new UpdateUtil(this, updatePath).Update();

            // Initialize TabContent
            DaiCo.Shared.Utility.SharedObject.tabContent = new DaiCo.Shared.MainTabControl();
            DaiCo.Shared.Utility.SharedObject.tabContent.Dock = System.Windows.Forms.DockStyle.Fill;
            DaiCo.Shared.Utility.SharedObject.tabContent.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            DaiCo.Shared.Utility.SharedObject.tabContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            DaiCo.Shared.Utility.SharedObject.tabContent.Location = new System.Drawing.Point(0, 0);
            DaiCo.Shared.Utility.SharedObject.tabContent.Name = "tabContent";
            DaiCo.Shared.Utility.SharedObject.tabContent.SelectedIndex = 0;
            DaiCo.Shared.Utility.SharedObject.tabContent.Size = new System.Drawing.Size(610, 468);
            DaiCo.Shared.Utility.SharedObject.tabContent.TabIndex = 1;
            uPanelCenter.ClientArea.Controls.Add(DaiCo.Shared.Utility.SharedObject.tabContent);
            uPanelCenter.ClientArea.Controls.Add(this.pictureBox2);

            if (SharedObject.UserInfo.UserPid == int.MinValue)
            {
                mnuLogin.Visible = true;
                mnuLogout.Visible = false;
                mnuChangePass.Visible = false;
                mnuAuthenticate.Visible = false;
                mnuAuthenticateControls.Visible = false;
            }
            DaiCo.Shared.Utility.SharedObject.tabContent.Visible = false;

            // Login
            frmLogin frm = new frmLogin();
            frm.ShowDialog();

            if (SharedObject.UserInfo.UserPid != int.MinValue)
            {
                this.Text = string.Format("{0} - Welcome to {1}", this.companyName, FunctionUtility.BoDau(SharedObject.UserInfo.EmpName));

                // Tao cay
                CreateModulContorller(SharedObject.UserInfo.UserPid);
                SetupDisplayMenu();

                if (SharedObject.UserInfo.UserPid != int.MinValue && this.erpLog == long.MinValue)
                {
                    // Insert Log
                    this.InsertLog();
                }
                this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FromClosing_Event);
            }
            this.loadHomepage();
            this.ultCBFunction.Focus();

            //Timer alertMessageTimer = new Timer();
            //alertMessageTimer.Interval = 6000;
            //alertMessageTimer.Tick += new EventHandler(AlertMessageTimer_Tick);
            //alertMessageTimer.Start();

            
        }
        void AlertMessageTimer_Tick(object sender, EventArgs e)
        {
            DBParameter[] inputParam = new DBParameter[] { new DBParameter("@EID", DbType.Int32, SharedObject.UserInfo.UserPid) };
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@MessageOut", DbType.String, 4000, SharedObject.UserInfo.UserPid) };
            DataBaseAccess.ExecuteStoreProcedure("spGNRMessageReminder", inputParam, outputParam);
            if (outputParam != null && outputParam[0].Value.ToString().Length > 0)
            {
                udaMessageReminder.Show("Reminder", outputParam[0].Value.ToString());
            }
        }
        /// <summary>
        /// Load Home page
        /// </summary>
        private void loadHomepage()
        {
            DaiCo.Shared.Utility.SharedObject.tabContent.Visible = true;

            DaiCo.Shared.MainUserControl uc = null;
            string typeName = "DaiCo.ERPProject.viewMAI_01_001,ERPProject";
            string str = this.GetType().ToString();
            Type tyView = Type.GetType(typeName, false, true);
            if (tyView != null)
            {
                object objView = System.Activator.CreateInstance(tyView);
                if (objView != null)
                {
                    uc = (DaiCo.Shared.MainUserControl)objView;
                }
            }
            DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "HOME PAGE", false, ViewState.MainWindow, FormWindowState.Normal);
        }
        /// <summary>
        /// Insert ERP Log
        /// </summary>
        private void InsertLog()
        {
            try
            {
                DBParameter[] inputParam = new DBParameter[3];
                inputParam[0] = new DBParameter("@MachineName", DbType.AnsiString, 32, System.Environment.MachineName);
                inputParam[1] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
                inputParam[2] = new DBParameter("@UserDomainName", DbType.AnsiString, 32, System.Environment.UserName);

                DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                DataBaseAccess.ExecuteStoreProcedure("spGNRERPLog_Insert", inputParam, outputParam);
                this.erpLog = DBConvert.ParseLong(outputParam[0].Value.ToString());
            }
            catch { }
        }

        /// <summary>
        /// Update ERP Log
        /// </summary>
        private void UpdateLog()
        {
            try
            {
                DBParameter[] inputParam = new DBParameter[1];
                inputParam[0] = new DBParameter("@ErpLog", DbType.Int64, this.erpLog);

                DataBaseAccess.ExecuteStoreProcedure("spGNRERPLog_Update", inputParam);
            }
            catch { }
        }

        /// <summary>
        /// Create ModulController TreeView
        /// </summary>
        /// <returns></returns>
        /// 
        private void CreateModulContorller(long userPid)
        {

            string storeName = "spGNRGroupUserNode_Union";
            DBParameter[] inputParam = new DBParameter[3];
            if (userPid == DaiCo.ERPProject.ConstantClass.UserAddmin)
            {
                inputParam[0] = new DBParameter("@IsAdmin", DbType.Int32, 1);
            }
            else
            {
                inputParam[0] = new DBParameter("@EmpPid", DbType.Int64, userPid);
                inputParam[1] = new DBParameter("@GroupMaster", DbType.Int32, DaiCo.Shared.Utility.ConstantClass.GROUP_ROLE);
            }
            inputParam[2] = new DBParameter("@CheckVersion", DbType.Int32, 0);

            DataSet ds = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);

            DataTable dt = new DataTable();
            dt.Merge(ds.Tables[0]);
            //Utility.LoadUltraCombo(ultCBFunction, ds.Tables[1], "UICode", "Title", false, "UICode");
            ultCBFunction.DataSource = ds.Tables[1];
            ultCBFunction.ValueMember = "UICode";
            ultCBFunction.DisplayMember = "Title";
            ultCBFunction.DisplayLayout.Reset();
            ultCBFunction.DisplayLayout.Bands[0].Columns["UICode"].Hidden = true;
            ultCBFunction.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultCBFunction.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            ultCBFunction.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            ultCBFunction.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            ultCBFunction.DisplayLayout.Bands[0].ColHeadersVisible = false;
            ultCBFunction.DisplayLayout.Bands[0].Columns["Title"].MaxWidth = 250;
            ultCBFunction.DisplayLayout.Bands[0].Columns["Title"].MinWidth = 250;

            if (dt != null)
            {
                DataRow[] rows = dt.Select("ParentPid IS NULL ");
                List<MainTreeNode> controller = new List<MainTreeNode>();
                foreach (DataRow row in rows)
                {
                    MainTreeNode node = new MainTreeNode(row["Title"].ToString(), "", "", 0, 1);
                    long pid = DBConvert.ParseLong(row["Pid"].ToString());
                    node.UIPid = pid;
                    tvModulController.Nodes.Add(node);
                    CreateChileNode(dt, node, pid);
                }
            }
        }

        /// <summary>
        /// Recursive to make chilenode for TreeView
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="node"></param>
        /// <param name="pid"></param>
        private void CreateChileNode(DataTable dt, MainTreeNode node, long pid)
        {
            DataRow[] chileRows = dt.Select("ParentPid = " + pid);
            if (chileRows.Length > 0)
            {
                foreach (DataRow row in chileRows)
                {
                    DataRow[] haveChile = dt.Select("ParentPid = " + row["Pid"].ToString());
                    MainTreeNode childNode;
                    if (haveChile.Length > 0)
                    {
                        childNode = new MainTreeNode(row["Title"].ToString(), "", "", 0, 1);
                        childNode.UIPid = (long)row["Pid"];
                        node.Nodes.Add(childNode);
                        long childPid = DBConvert.ParseLong(row["Pid"].ToString());
                        if (childPid != long.MinValue)
                            CreateChileNode(dt, childNode, childPid);
                    }
                    else
                    {
                        string viewName = row["UICode"].ToString();
                        string viewParam = row["UIParam"].ToString();
                        string path = row["NameSpace"].ToString();
                        int viewState = DBConvert.ParseInt(row["ViewState"].ToString());
                        childNode = new MainTreeNode(row["Title"].ToString(), viewName, path, 2, 2);
                        childNode.UIPid = (long)row["Pid"];
                        childNode.UIParam = viewParam;
                        if (viewState > 0)
                            childNode.ViewState = (ViewState)viewState;
                        int windowState = DBConvert.ParseInt(row["WindowState"].ToString());
                        if (windowState > 0)
                            childNode.WindowState = (FormWindowState)windowState;
                        node.Nodes.Add(childNode);
                    }
                }
            }
        }


        private void FromClosing_Event(object sender, FormClosingEventArgs e)
        {            
            if (MessageBox.Show("Are you sure you want to close programe...?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.CloseForm("frmMain");
                foreach (TabPage page in DaiCo.Shared.Utility.SharedObject.tabContent.TabPages)
                {
                    if (page != null)
                    {
                        DaiCo.Shared.MainUserControl uc = (DaiCo.Shared.MainUserControl)page.Controls[0];
                        if (uc != null)
                        {
                            bool result = uc.ConfirmToCloseTab();
                            if (!result)
                            {
                                e.Cancel = true;
                            }
                        }
                    }
                }
                // Update Log
                this.UpdateLog();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void CloseForm(string name)
        {
            FormCollection fc = System.Windows.Forms.Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Name == name)
                {
                    frm.Close();
                    frm.Dispose();
                    return;
                }
            }
        }

        void mnuExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        void mnuLogout_Click(object sender, System.EventArgs e)
        {
            while (Application.OpenForms.Count > 1)
            {
                Form frm = (Form)Application.OpenForms[1];
                if (frm.Name != this.Name)
                {
                    frm.Close();
                }
            }

            foreach (TabPage page in DaiCo.Shared.Utility.SharedObject.tabContent.TabPages)
            {
                MainUserControl mainUC = null;
                foreach (MainUserControl uc in page.Controls)
                {
                    mainUC = uc;
                    break;
                }
                if (mainUC != null)
                {
                    mainUC.ConfirmToCloseTab();
                }
            }
            tvModulController.Nodes.Clear();
            // Update Log
            this.UpdateLog();
            this.erpLog = long.MinValue;

            DaiCo.Shared.Utility.SharedObject.UserInfo = new BOMUserLogin();
            // Tron update (28/02/2011)      

            frmLogin frmLogin = new frmLogin();
            frmLogin.ShowDialog();
            DaiCo.Shared.Utility.SharedObject.tabContent.Visible = false;

            if (DaiCo.Shared.Utility.SharedObject.UserInfo.UserPid != int.MinValue && this.erpLog == long.MinValue)
            {
                // Insert Log
                this.InsertLog();
                this.Text = string.Format("Lam Viet - Welcome to {0}", FunctionUtility.BoDau(SharedObject.UserInfo.EmpName));
            }
            //List<MainTreeNode> controller = ModulController.InitController();
            CreateModulContorller(SharedObject.UserInfo.UserPid);
            SetupDisplayMenu();
            this.loadHomepage();
            this.ultCBFunction.Focus();
        }

        private void mnuLogin_Click(object sender, EventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.ShowDialog();
            DaiCo.Shared.Utility.SharedObject.tabContent.Visible = false;

            //List<MainTreeNode> controller = ModulController.InitController();
            CreateModulContorller(SharedObject.UserInfo.UserPid);
            SetupDisplayMenu();

            if (SharedObject.UserInfo.UserPid != int.MinValue && this.erpLog == long.MinValue)
            {
                // Insert Log
                this.InsertLog();
            }
        }

        private void mnuAuthenticate_Click(object sender, EventArgs e)
        {
            MainBOM.AUTHENTICATE.frmAuthenticate frm = new MainBOM.AUTHENTICATE.frmAuthenticate();
            frm.Show();
        }

        private void mnuChangePass_Click(object sender, EventArgs e)
        {
            frmChangePassword frm = new frmChangePassword();
            frm.ShowDialog();
        }

        private void mnuAuthenticateControls_Click(object sender, EventArgs e)
        {
            MainBOM.AUTHENTICATE.frmAuthenticateControl frm = new MainBOM.AUTHENTICATE.frmAuthenticateControl();
            frm.Show();
        }

        private void issueForProductionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewFOU_05_002 view = new viewFOU_05_002();
            view.kind = DaiCo.Shared.Utility.ConstantClass.FOU_WIP_TYPE_ISSUE_TO_PRODUCTION;
            WindowUtinity.ShowView(view, "NEW ISSUING FOR PRODUCTION", true, ViewState.Window);
        }

        private void returnToWarehouseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewFOU_06_002 view = new viewFOU_06_002();
            view.kind = DaiCo.Shared.Utility.ConstantClass.FOU_WIP_TYPE_RETURN_TO_WIP_WH;
            WindowUtinity.ShowView(view, "NEW RETURN TO WAREHOUSE", true, ViewState.Window);
        }

        #region Methods
        private void SetupDisplayMenu()
        {
            if (SharedObject.UserInfo.UserPid == int.MinValue)
            {
                mnuLogin.Visible = true;
                mnuLogout.Visible = false;
                mnuChangePass.Visible = false;
                mnuAuthenticate.Visible = false;
                mnuAuthenticateControls.Visible = false;    //Ha Anh add them
            }
            else
            {
                mnuLogin.Visible = false;
                mnuLogout.Visible = true;
                mnuChangePass.Visible = true;

                //search group
                string commandText = "SELECT	COUNT(*) FROM	TblGNRAccessGroup G " +
                                     " INNER JOIN TblGNRAccessGroupUser GU ON GU.GroupPid = G.Pid " +
                                     " INNER JOIN TblBOMUser U ON U.Pid = GU.UserPid AND U.EmployeePid =" + SharedObject.UserInfo.UserPid +
                                     " INNER JOIN TblBOMCodeMaster C ON C.Code = G.[Role] AND C.Code = 1 AND C.[Group] =" + DaiCo.Shared.Utility.ConstantClass.GROUP_ROLE;
                int count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText).ToString());

                if (SharedObject.UserInfo.UserPid == DaiCo.ERPProject.ConstantClass.UserAddmin || count > 0)
                {
                    mnuAuthenticate.Visible = true;
                    mnuAuthenticateControls.Visible = true;
                }
                else
                {
                    mnuAuthenticate.Visible = false;
                    mnuAuthenticateControls.Visible = false;
                }
            }
        }
        #endregion Methods

        string cstr = string.Empty;
        string cpath = string.Empty;
        private void FindFunction(string functionName, string despath)
        {
            if (cstr.Length > 0)
            {
                TreeNode[] nf = tvModulController.Nodes.Find(cstr, true);
                if (nf.Length > 0)
                {
                    foreach (TreeNode a in nf)
                    {
                        if (a.FullPath == cpath)
                        {
                            a.BackColor = Color.Empty;
                        }
                    }
                }
                cstr = string.Empty;
                cpath = string.Empty;
            }

            tvModulController.CollapseAll();

            TreeNode[] nodefind = tvModulController.Nodes.Find(functionName, true);

            if (nodefind.Length > 0)
            {
                cstr = functionName;
                foreach (TreeNode nd in nodefind)
                {
                    if (nd.FullPath == despath)
                    {
                        cpath = despath;
                        tvModulController.SelectedNode = nd;
                        tvModulController.SelectedNode.BackColor = Color.Yellow;
                        ShowViewFromController();
                    }
                }
            }
        }

        private void ultCBFunction_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ultCBFunction.SelectedRow != null)
                {
                    string oldstring = " --> ";
                    string newstring = @"\";
                    string path = ultCBFunction.SelectedRow.Cells["Path"].Value.ToString().Replace(oldstring, newstring);
                    this.FindFunction(Utility.GetSelectedValueUltraCombobox(ultCBFunction), path);
                }
            }
        }

        private void tvModulController_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (cstr.Length > 0)
            {
                TreeNode[] nf = tvModulController.Nodes.Find(cstr, true);
                if (nf.Length > 0)
                {
                    foreach (TreeNode a in nf)
                    {
                        if (a.FullPath == cpath)
                        {
                            a.BackColor = Color.Empty;
                        }
                    }
                }
                cstr = string.Empty;
                cpath = string.Empty;
            }

        }

        private void ultCBFunction_Enter(object sender, EventArgs e)
        {
            if (ultCBFunction.Text.Length > 0)
            {
                ultCBFunction.Text = string.Empty;
            }
        }




    }
}