/*
  Author      : Nguyen Thanh Binh
  Date        : 28/03/2021
  Description : Add account
  Standard Form: view_SearchSave.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Collections;
using System.Data;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_09_002 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    private IList listDeletedPid = new ArrayList();
    public long pidMainAccount = long.MinValue;
    public long pidAccount = long.MinValue;
    private long error = 0;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadType();
      this.LoadPostingRule();

      // Set Language
      this.SetLanguage();
    }
    /// <summary>
    /// Load Type
    /// </summary>
    private void LoadType()
    {
      string cmd = string.Format(@"
                                SELECT Code, [Value]
                                FROM TblBOMCodeMaster
                                WhERE [Group] = 6002");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      Utility.LoadUltraCombo(ucbType, dt, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Load Type
    /// </summary>
    private void LoadPostingRule()
    {
      string cmd = string.Format(@"
                                SELECT Code, [Value]
                                FROM TblBOMCodeMaster
                                WhERE [Group] = 6003");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      Utility.LoadUltraCombo(ucbPostingRule, dt, "Code", "Value", false, "Code");
    }


    /// <summary>
    /// Load data
    /// </summary>
    private void LoadData()
    {
      string cmd = string.Format(@"SELECT PId, ParentPid, AccountCode, AccountName, [Type], PostingRule, ISNULL(IsActive, 0) IsActive
                                    FROM TblACCAccount PR
                                    WHERE IsActive = 1
                                    AND Pid = {0}
                                 ", this.pidAccount);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      if (dt.Rows.Count > 0)
      {
        txtAccountCode.Text = DBConvert.ParseString(dt.Rows[0]["AccountCode"]);
        txtAccountName.Text = DBConvert.ParseString(dt.Rows[0]["AccountName"]);
        if (DBConvert.ParseInt(dt.Rows[0]["Type"]) != int.MinValue)
        {
          ucbType.Value = DBConvert.ParseInt(dt.Rows[0]["Type"]);
        }
        if (DBConvert.ParseInt(dt.Rows[0]["PostingRule"]) != int.MinValue)
        {
          ucbPostingRule.Value = DBConvert.ParseInt(dt.Rows[0]["PostingRule"]);
        }
        chkActive.Checked = (Boolean)dt.Rows[0]["IsActive"];
      }
      else
      {

      }

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
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

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
      if (txtAccountCode.Text.Trim().Length <= 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Mã tài khoản không được để trống!!!");
        return false;
      }
      if (txtAccountName.Text.Trim().Length <= 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Tên tài khoản không được để trống!!!");
        return false;
      }
      if (ucbType.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Tính chất của tài khoản không được để trống!!!");
        return false;
      }
      if (ucbPostingRule.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Định khoản của tài khoản không được để trống!!!");
        return false;
      }
      return true;
    }


    private bool SaveMain()
    {
      string storeName = "spACCAccount_Edit";
      int paramNumber = 8;
      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      if (this.pidAccount != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.pidAccount);
      }
      if (this.pidMainAccount != long.MinValue)
      {
        inputParam[1] = new SqlDBParameter("@ParentPid", SqlDbType.BigInt, this.pidMainAccount);
      }
      inputParam[2] = new SqlDBParameter("@AccountCode", SqlDbType.VarChar, txtAccountCode.Text.Trim().ToString());
      inputParam[3] = new SqlDBParameter("@AccountName", SqlDbType.NVarChar, txtAccountName.Text.Trim().ToString());
      inputParam[4] = new SqlDBParameter("@Type", SqlDbType.Int, DBConvert.ParseInt(ucbType.Value));
      inputParam[5] = new SqlDBParameter("@PostingRule", SqlDbType.Int, DBConvert.ParseInt(ucbPostingRule.Value));
      inputParam[6] = new SqlDBParameter("@IsActive", SqlDbType.Bit, (Boolean)chkActive.Checked ? true : false);
      inputParam[7] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };

      SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.pidAccount = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      else if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) == -1)
      {
        this.error = -1;
      }
      return false;
    }


    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = true;
        success = this.SaveMain();
        if (success)
        {
          WindowUtinity.ShowMessageSuccessFromText("Lưu thành công!!!");
          this.CloseTab();
        }
        else
        {
          if (this.error == -1)
          {
            WindowUtinity.ShowMessageErrorFromText("Mã tài khoản đã tồn tại trong hệ thống.");
          }
          else
          {
            WindowUtinity.ShowMessageErrorFromText("Lỗi khi lưu!!!");
          }
        }

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


      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewACC_09_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_09_002_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(gpbAccountDetail);

      //Init Data
      this.InitData();
      this.LoadData();
    }



    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
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

      }
    }


    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }


    #endregion event
  }
}
