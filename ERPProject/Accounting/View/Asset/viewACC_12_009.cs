/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : asset stop depreciation
  Standard Form: view_SaveMasterDetail
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
  public partial class viewACC_12_009 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Asset_Transfer;
    public int actionCode = 1;
    private int currency = int.MinValue;
    public int objectTye = int.MinValue;
    public int creditPid = int.MinValue;
    private bool isLoadedDetail = false;
    private bool isLoadedPostTransaction = false;
    #endregion Field

    #region Init
    public viewACC_12_009()
    {
      InitializeComponent();
    }

    private void viewACC_12_009_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(ugbInformation);
      this.SetBlankForTextOfButton(this);
      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
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

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetStopDepreciation_Init");

      //Init dropdown
      Utility.LoadUltraCombo(ucbStatus, dsInit.Tables[0], "StatusCode", "StatusName", false, "StatusCode");
      Utility.LoadUltraCombo(ucbAsset, dsInit.Tables[1], "Pid", "AssetCode", true, "Pid");
      ucbAsset.DisplayLayout.Bands[0].Columns["AssetCode"].Header.Caption = "Mã Tài Sản";
      ucbAsset.DisplayLayout.Bands[0].Columns["AssetName"].Header.Caption = "Tên Tài Sản";
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      //if (this.status > 0)
      //{
      //  txtAssetStopCode.ReadOnly = true;
      //  udtAssetStopDate.ReadOnly = true;
      //  txtAssetTransferDesc.ReadOnly = true;
      //  btnSave.Enabled = false;
      //}
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtAssetStopCode.Text = dtMain.Rows[0]["StopCode"].ToString();
        txtAssetStopDesc.Text = dtMain.Rows[0]["StopDesc"].ToString();
        udtAssetStopDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["StopDate"]);
        ucbStatus.Value = DBConvert.ParseInt(dtMain.Rows[0]["StopStatus"]);
        ucbAsset.Value = DBConvert.ParseInt(dtMain.Rows[0]["AssetPid"]);
      }
      else
      {
        string cmd = string.Format(@"SELECT dbo.FACCAssetStopDepreciationNo() StopCode");

        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);

        txtAssetStopCode.Text = dt.Rows[0]["StopCode"].ToString();
        udtAssetStopDate.Value = DateTime.Now;
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCAssetStopDepreciation_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.LoadMainData(dsSource.Tables[0]);
      }
      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (udtAssetStopDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtAssetStopDate.Focus();
        return false;
      }

      //check detail

      //for (int i = 0; i < ugdData.Rows.Count; i++)
      //{
      //  UltraGridRow row = ugdData.Rows[i];
      //  row.Selected = false;
      //  if (row.Cells["MaterialCode"].Value.ToString().Length <= 0)
      //  {
      //    row.Cells["MaterialCode"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Mã SP không được để trống.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }

      //  if (DBConvert.ParseDouble(row.Cells["OriginalAmount"].Value) <= 0)
      //  {
      //    row.Cells["OriginalAmount"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Nguyên giá không được để trống và phải lớn hơn 0.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }
      //  if (DBConvert.ParseInt(row.Cells["DepreciationMonth"].Value) < 0)
      //  {
      //    row.Cells["DepreciationMonth"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Số tháng khấu hao không được để trống.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }
      //  if (DBConvert.ParseDouble(row.Cells["DepreciationAmount"].Value) < 0)
      //  {
      //    row.Cells["DepreciationAmount"].Appearance.BackColor = Color.Yellow;
      //    WindowUtinity.ShowMessageErrorFromText("Giá trị khấu hao không được để trống.");
      //    row.Selected = true;
      //    ugdData.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }

      //}
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[6];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtAssetStopDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@StopDesc", SqlDbType.NVarChar, txtAssetStopDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@StopDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtAssetStopDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@Status", SqlDbType.Int, DBConvert.ParseInt(ucbStatus.Value));
      inputParam[4] = new SqlDBParameter("@AssetPid", SqlDbType.BigInt, DBConvert.ParseLong(ucbAsset.Value));
      inputParam[5] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCAssetStopDepreciation_Save", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
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
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }

    #endregion Function

    #region Event

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
      }
    }
    #endregion Event

  }
}