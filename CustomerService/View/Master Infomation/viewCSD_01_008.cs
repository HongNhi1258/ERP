/*
  Author      : Lậm Quang Hà
  Date        : 11/10/2010
  Decription  : Insert, Update Room & Room Language
  Checked by    : Võ Hoa Lư
  Checked date  : 13/10/2010
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.UserControls;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_008 : MainUserControl
  {
    #region Field
    public long roomPid = long.MinValue;
    private bool loadingData = false;
    private IList listDeletedPid = new ArrayList();    
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_01_008_Load(object sender, EventArgs e)
    {      
      //Load Room's information
      this.LoadData();
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Load OtherLanguage
    /// </summary>
    private void LoadGrid()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@RoomPid", DbType.Int64, this.roomPid) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDRoomLanguage_Select", inputParam);      
      ultraGridOtherLanguage.DataSource = dtSource;
    }

    /// <summary>
    /// Load Room's information
    /// </summary>
    private void LoadData()
    {
      this.loadingData = true;
      this.listDeletedPid = new ArrayList();
      if (this.roomPid != long.MinValue)
      {
        CSDRoom roomObj = new CSDRoom();
        roomObj.Pid = this.roomPid;
        roomObj = (CSDRoom)DataBaseAccess.LoadObject(roomObj, new string[] { "Pid" });
        if (roomObj == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return;
        }
        txtRoom.Text = roomObj.Room;        
      }
      this.LoadGrid();
      this.NeedToSave = false;
      this.loadingData = false;
    }

    /// <summary>
    /// Clear screen
    /// </summary>
    private void Clear()
    {
      this.NeedToSave = false;
      this.roomPid = long.MinValue;
      txtRoom.Text = string.Empty;
      DataTable dtSource = (DataTable)ultraGridOtherLanguage.DataSource;
      dtSource.Rows.Clear();
    }
    #endregion Load Data

    #region Check & Save Data
    /// <summary>
    /// Check logic : Room are required
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      if (txtRoom.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Room" });
        return false;
      }
      return true;
    }
    /// <summary>
    /// Save Data : insert new record or update one record TblCSDRoomLanguage in database
    /// </summary>
    /// <param name="row"></param>
    /// <param name="parentPid"></param>
    /// <returns>true : success; fase : unsuccess</returns>
    private bool SaveRoomLanguage(DataRow row, long parentPid)
    {
      DBParameter[] inputParam = new DBParameter[5];
      string storeName = string.Empty;
      long pid = DBConvert.ParseLong(row["Pid"].ToString());
      if (pid != long.MinValue) //update
      {
        inputParam[4] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
        storeName = "spCSDRoomLanguage_Update";
      }
      else //Insert
      {
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spCSDRoomLanguage_Insert";
      }
      inputParam[1] = new DBParameter("@RoomPid", DbType.Int64, parentPid);

      long languagepid = DBConvert.ParseLong(row["LanguagePid"].ToString());
      inputParam[2] = new DBParameter("@LanguagePid", DbType.Int64, languagepid);

      string categoryName = row["RoomName"].ToString().Trim();
      inputParam[3] = new DBParameter("@RoomName", DbType.String, 128, categoryName);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Data : insert new record or update one record TblCSDRoom in database
    /// </summary>
    /// <param name="pid"></param>
    /// <returns>true : success; fase : unsuccess</returns>
    private bool SaveData(out long pid)
    {
      bool result = true;
      //1.Insert, update Room
      DBParameter[] inputParam = new DBParameter[3];
      string storeName = string.Empty;
      if (this.roomPid != long.MinValue) // update
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.roomPid);
        inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spCSDRoom_Update";
        pid = this.roomPid;
      }
      else // Insert
      {
        inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spCSDRoom_Insert";
      }
      string room = txtRoom.Text.Trim();
      inputParam[1] = new DBParameter("@Room", DbType.AnsiString, 128, room);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      pid = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
      if (pid <= 0)
      {
        return false;
      }
      //2.1.Delete RoomLanguage
      foreach (long detailPid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
        DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDRoomLanguage_Delete", inputParamDelete, outputParamDelete);
        long outputValue = DBConvert.ParseLong(outputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      //2.2 Insert/update TblCSDRoomLanguage
      DataTable dtSource = (DataTable)ultraGridOtherLanguage.DataSource;
      foreach (DataRow dr in dtSource.Rows)
      {
        if ((dr.RowState == DataRowState.Modified) && (dr["RoomName"].ToString().Trim().Length > 0))
        {
          bool success = this.SaveRoomLanguage(dr, this.roomPid);
          if (!success)
          {
            result = false;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Confirm before close
    /// YES : Save and close
    /// No  : Close without save
    /// Cancle : nothing
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      if (this.CheckInvalid())
      {
        long pid = long.MinValue;
        bool success = this.SaveData(out pid);
        if (success)
        {          
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
        }        
      }
      else
      {
        this.SaveSuccess = false;
      }
    }
    #endregion Check & Save Data

    #region Event


    /// <summary>
    /// Insert/update some record in screen into table TblRoom in database
    /// Insert/update/Delete some record in screen into table TblRoomLanguage in database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = this.CheckInvalid();
      if (!success)
      {
        return;
      }
      long pid = long.MinValue;
      success = this.SaveData(out pid);
      if (success)
      {
        this.roomPid = pid;
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");    
      }
    }

    /// <summary>
    /// Description : 
    ///   1/Insert/update some record in screen into table TblRoom in database
    ///   2/Insert/update/Delete some record in screen into table TblRoomLanguage in database
    ///   3/Reset screen for save register new  TblCSDRoom & TblCSDRoomLanguage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveContinue_Click(object sender, EventArgs e)
    {
      bool success = this.CheckInvalid();
      if (!success)
      {
        return;
      }
      long pid = long.MinValue;
      success = this.SaveData(out pid);
      if (success)
      {
        this.roomPid = pid;
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
        this.loadingData = true;
        this.Clear();
        this.loadingData = false;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }    
    }

    /// <summary>
    /// Confirm Save and Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Init layout for ultragrid view RoomLanguage Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridOtherLanguage_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridOtherLanguage);
      e.Layout.Bands[0].Columns["LanguagePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;            
      e.Layout.Bands[0].Columns["RoomName"].Header.Caption = "Room Name";      
    }

    /// <summary>
    /// Set this.NeedToSave = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtRoom_TextChanged(object sender, EventArgs e)
    {
      if (!this.loadingData)
      {
        this.NeedToSave = (btnSave.Visible) && (btnSaveContinue.Visible);
      }      
    }

    /// <summary>
    /// Set this.NeedToSave = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridOtherLanguage_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (!this.loadingData)
      {
        this.NeedToSave = ((btnSave.Visible) && (btnSaveContinue.Visible));
        string roomName = e.Cell.Value.ToString().Trim();
        long pid = DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          if ((roomName.Length == 0) && (!this.listDeletedPid.Contains(pid)))
          {
            this.listDeletedPid.Add(pid);
          }
          else if ((roomName.Length > 0) && (this.listDeletedPid.Contains(pid)))
          {
            this.listDeletedPid.RemoveAt(this.listDeletedPid.IndexOf(pid));
          }
        }
      }
    }
    #endregion Event
  }
}
