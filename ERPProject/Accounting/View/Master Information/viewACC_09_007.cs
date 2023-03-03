/*
  Author      : Lê Thanh Long
  Date        : 13-Apr-2022
  Description : Get Payment Term
  Standard Form: viewACC_09_007.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using System.Diagnostics;


namespace DaiCo.ERPProject
{
    public partial class viewACC_09_007 : MainUserControl
    {

        #region field
        ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_SearchInfo).Assembly);
        #endregion field

        #region function
        /// <summary>
        /// Set data for controls
        /// </summary>
        private void InitData()
        {
            LoadItemKind();
            this.SetLanguage();
            chkIsActive.Checked = true;
        }
        private void LoadItemKind()
        {
            string commandText = string.Format("Select code,  Value From TblBOMCodeMaster Where [Group] = 210322 And DeleteFlag = 0 Order By Sort");
            DataTable dtItemKind = DataBaseAccess.SearchCommandTextDataTable(commandText);
            Utility.LoadUltraCombo(ucbPaymentKind, dtItemKind, "Code", "Value", false, "Code");
        }


        /// <summary>
        /// Get information from database
        /// </summary>
        private void SearchData()
        {
            string termCode = string.Empty;
            string termName = string.Empty;
            int kindCode = int.MinValue;
            Boolean isActive = true;
            if (ucbPaymentKind.SelectedRow != null)
            {
                kindCode = int.Parse(ucbPaymentKind.Value.ToString());
            }
            else
            {
                kindCode = int.MinValue;
            }
            isActive = chkIsActive.Checked ? true : false;
            termCode = txtTermCode.Text.Trim();
            termName = txtTermName.Text.Trim();
            btnSearch.Enabled = false;
            DBParameter[] inputParam = new DBParameter[4];
            if (termCode.Length > 0)
            {

                inputParam[0] = new DBParameter("@TermCode", DbType.AnsiString, 20, "%" + termCode.Replace("'", "''") + "%");
            }
            if (termName.Length > 0)
            {

                inputParam[1] = new DBParameter("@TermName", DbType.AnsiString, 20, "%" + termName.Replace("'", "''") + "%");
            }
            if (ucbPaymentKind.SelectedRow != null)
            {
                inputParam[2] = new DBParameter("@KindCode", DbType.Int32, kindCode);
            }

            inputParam[3] = new DBParameter("@IsActive", DbType.Boolean, isActive);
            DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spACCPaymentTermMain_Search", inputParam);
            ugdInformation.DataSource = dtSource;
            lbCount.Text = string.Format("Count: {0}", ugdInformation.Rows.FilteredInRowCount);
            btnSearch.Enabled = true;
        }
        /// <summary>
        /// Clear all data of search fields
        /// </summary>
        private void ClearCondition()
        {
            chkIsActive.Checked = true;
            ucbPaymentKind.Text = "";
            txtTermCode.Text = "";
            txtTermName.Text = "";

        }

        /// <summary>
        /// Set Auto Search Data When User Press Enter
        /// </summary>
        /// <param name="groupControl"></param>
        private void SetAutoSearchWhenPressEnter(Control groupControl)
        {
            // Add KeyDown even for all controls in groupBoxSearch
            foreach (Control ctr in groupControl.Controls)
            {
                if (ctr.Controls.Count == 0)
                {
                    ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
                }
                else
                {
                    this.SetAutoSearchWhenPressEnter(ctr);
                }
            }
        }

        /// <summary>
        /// Set Auto Add 4 blank before text of button
        /// </summary>
        /// <param name="groupControl"></param>
        private void SetBlankForTextOfButton(Control groupControl)
        {
            // Add KeyDown even for all controls in groupBoxSearch
            foreach (Control ctr in groupControl.Controls)
            {
                if (ctr.Controls.Count > 0)
                {
                    this.SetBlankForTextOfButton(ctr);
                }
                else if (ctr.GetType().Name == "Button")
                {
                    ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
                }
            }
        }

        private void SetLanguage()
        {
            //btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);     
            //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
            //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

            this.SetBlankForTextOfButton(this);
        }
        #endregion function

        #region event
        public viewACC_09_007()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load data for search fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void view_SearchInfo_Load(object sender, EventArgs e)
        {
            // Add KeyDown even for all controls in groupBoxSearch
            this.SetAutoSearchWhenPressEnter(uegSearch);

            //Init Data
            this.InitData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchData();

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.ClearCondition();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.CloseTab();
        }

        /// <summary>
        /// Auto search when user press Enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.SearchData();
            }
        }

        private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Utility.InitLayout_UltraGrid(ugdInformation);
            e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
            for (int i = 0; i < ugdInformation.Rows.Count; i++)
            {
                ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
            }
            e.Layout.Bands[0].Columns["TermCode"].Header.Caption = "Term Code";
            e.Layout.Bands[0].Columns["TermName"].Header.Caption = "Term Name";
            e.Layout.Bands[0].Columns["KindName"].Header.Caption = "Kind Name";
            e.Layout.Bands[0].Columns["IsActive"].Header.Caption = "Active Status";
            //e.Layout.Bands[0].Columns["TermName"].MaxWidth = 400;
            e.Layout.Bands[0].Columns["TermCode"].MaxWidth = 100;
            e.Layout.Bands[0].Columns["KindName"].MaxWidth = 100;
            e.Layout.Bands[0].Columns["IsActive"].MaxWidth = 100;

            e.Layout.Bands[0].Columns["Pid"].Hidden = true;
            e.Layout.Bands[0].Columns["IsActive"].CellActivation = Activation.AllowEdit;
            e.Layout.Bands[0].Columns["IsActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            Utility.ExportToExcelWithDefaultPath(ugdInformation, "Payment Term Main");
        }

        public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
        {
            if (popupMenu.Items[0].Selected)
            {
                //Utility.GetDataForClipboard(ugdInformation);
            }
        }

        private void ugdInformation_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
                {
                    popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
                }
            }
        }






        #endregion event

        private void ugdInformation_DoubleClick(object sender, EventArgs e)
        {
            viewACC_09_008 objItemMaster = new viewACC_09_008();
            //objItemMaster.newfr = 0;
            objItemMaster.PaymentTermPid = int.Parse(ugdInformation.Selected.Rows[0].Cells["Pid"].Value.ToString());
            Shared.Utility.WindowUtinity.ShowView(objItemMaster, "Chi Tiết Thanh Toán", false, DaiCo.Shared.Utility.ViewState.MainWindow, FormWindowState.Maximized);
        }


        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            viewACC_09_008 objItemMaster = new viewACC_09_008();
            // objItemMaster.newfr = 1;
            Shared.Utility.WindowUtinity.ShowView(objItemMaster, "Tạo Mới Thanh Toán", false, DaiCo.Shared.Utility.ViewState.MainWindow, FormWindowState.Maximized);
        }
    }
}
