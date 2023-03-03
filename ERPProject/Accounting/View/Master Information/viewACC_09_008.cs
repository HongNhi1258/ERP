/*
  Author      : Le Thanh Long
  Date        : 22/03/2022
  Description : Get Payment Term Detail
  Standard Form: viewACC_09_008
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{

    public partial class viewACC_09_008 : MainUserControl
    {
        //tranfer value form
        public int PaymentTermPid;
        #region field
        ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_SearchSave).Assembly);
        private IList listDeletedPid = new ArrayList();
        #endregion field    
        #region function
        /// <summary>
        /// Set data for controls
        /// </summary>
        private void InitData()
        {
            ucbddPaymentKind.Visible = false;
            ucbddPaymentTime.Visible = false;
            string sqlGetPaymentItemKind = @"Select code, Value From TblBOMCodeMaster Where[Group] = 210322 And DeleteFlag = 0 Order By Sort";
            string sqlGetPaymentKindCode = @"select PaymentKindCode,PaymentKindName from VACCPaymentKind order by PaymentKindCode";
            string sqlGetPaymentTimeCode = @"select PaymentTimeCode,PaymentTimeName from VACCPaymentTime order by PaymentTimeCode";
            this.LoadPaymentCombo(ucbPaymentKind, sqlGetPaymentItemKind, "code", "value");
            this.LoadPaymentCombo(ucbddPaymentKind, sqlGetPaymentKindCode, "PaymentKindCode", "PaymentKindName");
            this.LoadPaymentCombo(ucbddPaymentTime, sqlGetPaymentTimeCode, "PaymentTimeCode", "PaymentTimeName");
            // txttermcode.ReadOnly = PaymentTermPid > 0 ? true : false;
            txtTermCode.ReadOnly = true;
            GetNewTermCode();
            this.SetLanguage();
        }
        //tạo function get new code để check lại
        private void GetNewTermCode()
        {
            string cmd = string.Format(@"select dbo.FACCGetNewPaymentTermCode()");
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                txtTermCode.Text = dt.Rows[0][0].ToString();
            }
        }
        private void LoadPaymentCombo(UltraCombo ucb, string sql, string code, string name)
        {
            string commandText = string.Format(sql);
            DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
            Utility.LoadUltraCombo(ucb, dtItem, code, name, false, code);
        }
        /// <summary>
        /// Set Auto Ask Save Data When User Close Form
        /// </summary>
        /// <param name="groupControl"></param>
        private void SetAutoAskSaveWhenCloseForm(Control groupControl)
        {
            // Add KeyDown even for all controls in groupBoxSearch
            foreach (Control ctr in groupControl.Controls)
            {
                if (ctr.Controls.Count == 0)
                {
                    ctr.TextChanged += new System.EventHandler(this.Object_Changed);
                }
                else
                {
                    this.SetAutoAskSaveWhenCloseForm(ctr);
                }
            }
        }

        private void Object_Changed(object sender, EventArgs e)
        {
            this.SetNeedToSave();
        }

        private void LoadMainData(DataTable dtMain)
        {
            if (dtMain.Rows.Count > 0)
            {
                txtTermCode.Text = dtMain.Rows[0]["TermCode"].ToString();
                txtTermName.Text = dtMain.Rows[0]["TermName"].ToString();
                ucbPaymentKind.Value = DBConvert.ParseInt(dtMain.Rows[0]["Kind"].ToString());
                chkIsActive.Checked = (bool)dtMain.Rows[0]["IsActive"];
            }
        }

        private void LoadData()
        {
            this.listDeletedPid = new ArrayList();

            DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PaymentTermPid", DbType.Int64, PaymentTermPid) };
            DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCPaymentTermDetail_Load", inputParam);
            if (dsSource != null && dsSource.Tables.Count > 1)
            {
                LoadMainData(dsSource.Tables[0]);
                ugdInformation.DataSource = dsSource.Tables[1];
                lblCount.Text = string.Format("Count: {0}", (dsSource.Tables[0] != null ? dsSource.Tables[0].Rows.Count : 0));
            }
            //this.SetStatusControl();
            this.NeedToSave = false;
        }

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
        private void SetNeedToSave()
        {
            if (btnSave.Enabled && btnSave.Visible)
            {
                this.NeedToSave = true;
            }
            else
            {
                this.NeedToSave = false;
            }
        }
        private bool CheckValid()
        {
            double countRate = 0;
            if (txtTermCode.Text == "" || txtTermName.Text == "" || ucbPaymentKind.Value.ToString() == "")
            {
                WindowUtinity.ShowMessageErrorFromText(" Đảm bảo các dữ liệu nhập trên text vào đã đủ!!!");
                return false;
            }
            for (int i = 0; i < ugdInformation.Rows.Count; i++)
            {
                UltraGridRow row = ugdInformation.Rows[i];
                row.Selected = false;
                if (row.Cells["PaymentKindCode"].Value.ToString() == "")
                {
                    row.Cells["PaymentKindCode"].Appearance.BackColor = Color.Yellow;
                    WindowUtinity.ShowMessageErrorFromText(" Cột Tên Thanh Toán không được bỏ trống !!!");
                    row.Selected = true;
                    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
                    return false;
                }

                if (DBConvert.ParseDouble(row.Cells["PaymentPercent"].Value) < 0 || DBConvert.ParseDouble(row.Cells["PaymentPercent"].Value) > 100)
                {
                    row.Cells["PaymentPercent"].Appearance.BackColor = Color.Yellow;
                    WindowUtinity.ShowMessageErrorFromText(" Phần trăm thanh toán phải điền đẩy đủ và đúng định dạng từ 0 tới 100.");
                    row.Selected = true;
                    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
                    return false;
                }
                //count %
                countRate = countRate + DBConvert.ParseDouble(row.Cells["PaymentPercent"].Value);
                if (countRate > 100)
                {
                    row.Cells["PaymentPercent"].Appearance.BackColor = Color.Yellow;
                    WindowUtinity.ShowMessageErrorFromText(" Tổng số phần trăm thanh toán của mã " + txtTermCode.Text + " phải nhỏ hơn hoặc bằng 100%");
                    row.Selected = true;
                    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
                    return false;
                }

                if (row.Cells["MonthDay"].Value.ToString() != "")
                {
                    if (DBConvert.ParseDouble(row.Cells["MonthDay"].Value) < 0 || DBConvert.ParseDouble(row.Cells["MonthDay"].Value) > 31)
                    {
                        row.Cells["MonthDay"].Appearance.BackColor = Color.Yellow;
                        WindowUtinity.ShowMessageErrorFromText(" Cột ngày trong tháng bị sai định dạng ngày trong tháng từ 1 tới 31 !");
                        row.Selected = true;
                        ugdInformation.ActiveRowScrollRegion.FirstRow = row;
                        return false;
                    }
                }
                if (DBConvert.ParseDouble(row.Cells["DayNumber"].Value) > 0 && DBConvert.ParseDouble(row.Cells["MonthDay"].Value) > 0)
                {
                    row.Cells["DayNumber"].Appearance.BackColor = Color.Yellow;
                    row.Cells["MonthDay"].Appearance.BackColor = Color.Yellow;
                    WindowUtinity.ShowMessageErrorFromText("Cột Số Ngày Và  Ngày Trong Tháng không thể đồng thời có dữ liệu!!!");
                    row.Selected = true;
                    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
                    return false;
                }

                if (DBConvert.ParseDouble(row.Cells["DayNumber"].Value) <= 0 && DBConvert.ParseDouble(row.Cells["MonthDay"].Value) <= 0)
                {
                    row.Cells["DayNumber"].Appearance.BackColor = Color.Yellow;
                    row.Cells["MonthDay"].Appearance.BackColor = Color.Yellow;
                    WindowUtinity.ShowMessageErrorFromText("Cột Số Ngày Và  Ngày Trong Tháng không thể đồng thời bị trống!!!");
                    row.Selected = true;
                    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
                    return false;
                }
                if (row.Cells["PaymentTimeCode"].Value.ToString() == "")
                {
                    row.Cells["PaymentTimeCode"].Appearance.BackColor = Color.Yellow;
                    WindowUtinity.ShowMessageErrorFromText(" Cột Điểm  Thanh Toán không được bỏ trống");
                    row.Selected = true;
                    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
                    return false;
                }
            }
            return true;
        }

        bool insertMain()
        {
            Boolean isActive = chkIsActive.Checked ? true : false;
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DBParameter[] inputParam = new DBParameter[5];
            inputParam[0] = new DBParameter("@TermCode", DbType.String, txtTermCode.Text.Trim().ToString());
            inputParam[1] = new DBParameter("@TermName", DbType.String, txtTermName.Text.Trim().ToString());
            inputParam[2] = new DBParameter("@IsActive", DbType.Boolean, isActive);
            if (ucbPaymentKind.SelectedRow != null)
            {
                inputParam[3] = new DBParameter("@KindCode", DbType.Int32, DBConvert.ParseInt(ucbPaymentKind.Value));
            }
            inputParam[4] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spACCPaymentTerm_Edit", inputParam, outputParam);
            if (outputParam != null && DBConvert.ParseInt(outputParam[0].Value.ToString()) > 0)
            {
                this.PaymentTermPid = DBConvert.ParseInt(outputParam[0].Value.ToString());
                return true;
            }
            return false;
        }
        bool SaveDetailData()
        {
            bool success = true;
            // 1. Delete      
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            for (int i = 0; i < listDeletedPid.Count; i++)
            {
                DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
                DataBaseAccess.ExecuteStoreProcedure("spACCPaymentTermDetail_Delete", deleteParam, outputParam);
                if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
                {
                    success = false;
                }
            }
            // 2. Insert/Update      
            DataTable dtDetail = (DataTable)ugdInformation.DataSource;
            foreach (DataRow row in dtDetail.Rows)
            {
                if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                {
                    DBParameter[] inputParam = new DBParameter[9];
                    long pid = DBConvert.ParseLong(row["Pid"].ToString());
                    if (row.RowState == DataRowState.Modified && row["Pid"].ToString() != "") // Update
                    {
                        inputParam[0] = new DBParameter("@Pid", DbType.Int32, pid);
                    }
                    inputParam[1] = new DBParameter("@PaymentTermPid", DbType.Int32, (DBConvert.ParseInt(PaymentTermPid)));

                    if (DBConvert.ParseInt(row["PaymentKindCode"].ToString()) != int.MinValue)
                    {
                        inputParam[2] = new DBParameter("@PaymentKindCode", DbType.Int32, DBConvert.ParseInt(row["PaymentKindCode"].ToString()));
                    }

                    inputParam[3] = new DBParameter("@PaymentPercent", DbType.Int32, DBConvert.ParseInt(row["PaymentPercent"].ToString()));
                    if (row["MonthDay"].ToString() != "")
                    {
                        inputParam[4] = new DBParameter("@MonthDay", DbType.Int32, DBConvert.ParseInt(row["MonthDay"].ToString()));
                    }
                    else
                    {
                        inputParam[4] = new DBParameter("@MonthDay", DbType.Int32, 0);
                    }
                    if (row["DayNumber"].ToString() != "")
                    {
                        inputParam[5] = new DBParameter("@DayNumber", DbType.Int32, DBConvert.ParseInt(row["DayNumber"].ToString()));
                    }
                    else
                    {
                        inputParam[5] = new DBParameter("@DayNumber", DbType.Int32, 0);
                    }
                    //cần check combox select item . nhưng chưa chay                       
                    if (DBConvert.ParseInt(row["PaymentTimeCode"].ToString()) != int.MinValue)
                    {
                        inputParam[6] = new DBParameter("@PaymentTimeCode", DbType.Int32, DBConvert.ParseInt(row["PaymentTimeCode"].ToString()));
                    }

                    inputParam[7] = new DBParameter("@PaymentDesc", DbType.String, row["PaymentDesc"].ToString());
                    inputParam[8] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
                    DataBaseAccess.ExecuteStoreProcedure("spACCPaymentTermDetail_Edit", inputParam, outputParam);
                    if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
                    {
                        success = false;
                    }
                }
            }

            return success;
        }
        private void SaveData()
        {
            if (this.CheckValid())
            {
                bool success = true;
                if (this.insertMain())
                {
                    success = this.SaveDetailData();
                }
                else
                {
                    success = false;
                }
                if (success)
                {
                    WindowUtinity.ShowMessageSuccess("MSG0004");
                }
                else
                {
                    WindowUtinity.ShowMessageError("WRN0004");
                }
                this.LoadData();
            }
            else
            {
                this.SaveSuccess = false;
            }
        }
        public override void SaveAndClose()
        {
            base.SaveAndClose();
            this.SaveData();
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
            //lblCount.Text = rm.GetString("Count", ConstantClass.CULTURE) + ":";
            //btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);      
            //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
            //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
            //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

            this.SetBlankForTextOfButton(this);
        }
        #endregion function

        #region event
        public viewACC_09_008()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load data for search fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void view_SearchSave_Load(object sender, EventArgs e)
        {
            // Add KeyDown even for all controls in groupBoxSearch
            this.SetAutoSearchWhenPressEnter(gpbSearch);
            //Init Data
            this.InitData();
            LoadData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //  this.SearchData();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.ConfirmToCloseTab();
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
                //    this.SearchData();
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
            e.Layout.Bands[0].Columns["PaymentKindCode"].Header.Caption = "Payment Kind Name";
            e.Layout.Bands[0].Columns["PaymentPercent"].Header.Caption = "Payment Percent";
            e.Layout.Bands[0].Columns["MonthDay"].Header.Caption = "Month Day";
            e.Layout.Bands[0].Columns["DayNumber"].Header.Caption = "Day Number";
            e.Layout.Bands[0].Columns["PaymentTimeCode"].Header.Caption = "Payment Time Name";
            e.Layout.Bands[0].Columns["PaymentDesc"].Header.Caption = "Payment Descistion";


            e.Layout.Bands[0].Columns["Pid"].Hidden = true;
            e.Layout.Bands[0].Columns["PaymentKindCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            e.Layout.Bands[0].Columns["PaymentTimeCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            e.Layout.Bands[0].Columns["PaymentKindCode"].ValueList = ucbddPaymentKind;
            e.Layout.Bands[0].Columns["PaymentTimeCode"].ValueList = ucbddPaymentTime;
            e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveData();
        }
        private void ugdInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
        {
            e.DisplayPromptMsg = false;
            if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            foreach (UltraGridRow row in e.Rows)
            {
                this.SetNeedToSave();
                long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
                if (pid != long.MinValue)
                {
                    this.listDeletedPid.Add(pid);
                }
            }
        }

        private void ugdInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            string colName = e.Cell.Column.ToString();
            string value = e.NewValue.ToString();
            switch (colName)
            {
                case "PaymentKindCode":

                    if (ucbddPaymentKind.SelectedRow == null)
                    {
                        WindowUtinity.ShowMessageErrorFromText("Cột Thanh Toán không đúng định dạng");
                        e.Cancel = true;
                    }

                    break;
                case "PaymentTimeCode":

                    if (ucbddPaymentTime.SelectedRow == null)
                    {
                        WindowUtinity.ShowMessageErrorFromText("Cột Thời gian không đúng định dạng");
                        e.Cancel = true;
                    }

                    break;
                default:
                    break;
            }
        }

        private void ugdInformation_AfterCellUpdate(object sender, CellEventArgs e)
        {
            this.SetNeedToSave();

        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            //Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
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


    }
}
