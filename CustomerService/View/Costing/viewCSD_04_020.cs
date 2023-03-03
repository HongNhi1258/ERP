using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System.Collections;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_020 : MainUserControl
  {
    #region field
    public string itemCode = string.Empty;
    public long capturePid = long.MinValue;
    public string bomVersion = string.Empty;

    #endregion field
    #region Init
    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_020_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
      this.LoadBOMVersion();
    }
    #endregion Init

    #region function

    /// <summary>
    /// Load BOM Version
    /// </summary>
    private void LoadBOMVersion()
    {
      string cmmd = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 33";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmmd);
      ControlUtility.LoadUltraCombo(ultraDropBOMVersion, dt, "Code", "Value", "Code");
      ultraDropBOMVersion.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultraDropBOMVersion.DisplayLayout.AutoFitColumns = true;
      ultraDropBOMVersion.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      txtItemCode.Text = itemCode;
      //ultraDropBOMVersion.Value = bomVersion;
      if (this.capturePid > 0)
      {
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(string.Format("SELECT BOMVersion, CaptureRemark FROM TblCSDItemCosting WHERE Pid = {0}", this.capturePid));
        if (dt != null)
        {
          txtRemark.Text = dt.Rows[0][1].ToString();
          ultraDropBOMVersion.Value = dt.Rows[0][0].ToString();
        }
//        if(ultraDropBOMVersion.Value != null)
//        {
//        ultraDropBOMVersion.Value = DataBaseAccess.ExecuteScalarCommandText(string.Format(@"SELECT CM.Value 
//                                                                                            FROM TblCSDItemCosting CT
//                                                                                            INNER JOIN TblBOMCodeMaster CM ON CT.BOMVersion = CM.Code 
//                                                                                                                           AND [Group] = 33
//                                                                                            WHERE Pid = {0}", this.capturePid)).ToString();
//        }
//        else
//        {
//          ultraDropBOMVersion.Value = DBNull.Value;
//        }
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

    private void SaveData()
    {
      DBParameter[] input = new DBParameter[5];
     
      if (this.capturePid > 0)
      {
        input[0] = new DBParameter("@CapturePid", DbType.Int64, this.capturePid);
      }
      input[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      if (txtRemark.Text.Trim().Length > 0)
      {
        input[2] = new DBParameter("@CaptureRemark", DbType.String, 512, txtRemark.Text.Trim());
      }
      input[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (ultraDropBOMVersion.SelectedRow != null)
      {
        input[4] = new DBParameter("@BOMVersion", DbType.Int32, ultraDropBOMVersion.Value);
      }
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spCSDItemCosting_Capture", 600, input, output);

      long result = DBConvert.ParseLong(output[0].Value.ToString());

      if (result > 0)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        btnSave.Enabled = false;
      }
      else if (result == -1) 
      {
        WindowUtinity.ShowMessageErrorFromText("Can't capture Item Costing because Item Costing data in the last capture and now not different");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      this.SaveSuccess = false;
    }

    #endregion function

    #region event
    public viewCSD_04_020()
    {
      InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }
    #endregion event
  }
}
