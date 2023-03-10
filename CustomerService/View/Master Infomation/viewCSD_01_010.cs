/*
  Author      : Nguyễn Văn Trọn
  Date        : 16/12/2010
  Decription  : Collection Info  
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
  public partial class viewCSD_01_010 : MainUserControl
  {
    #region Field
    public long collectionPid = long.MinValue;
    private bool loadingData = false;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started 
    /// 1. Init list items for ultradropdown language.
    /// 3. Load collection's information.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_01_010_Load(object sender, EventArgs e)
    {       
      this.LoadData();
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Load list collection language.
    /// </summary>
    private void LoadGrid()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CollectionPid", DbType.Int64, this.collectionPid) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCollectionLanguage_Select", inputParam);      
      ultraGridOtherLanguage.DataSource = dtSource;
    }

    /// <summary>
    /// Load collection information.
    /// </summary>
    private void LoadData()
    {
      this.loadingData = true;
      this.listDeletedPid = new ArrayList();
      if (this.collectionPid != long.MinValue)
      {
        string commandText = string.Format("Select Value, [Description], MoreDescription From TblBOMCodeMaster Where [Group] = 2 And Code = {0}", this.collectionPid);
        DataTable dtCollection = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCollection != null && dtCollection.Rows.Count > 0)
        {
          txtUSCode.Text = dtCollection.Rows[0]["Description"].ToString();
          txtCollection.Text = dtCollection.Rows[0]["Value"].ToString();
          txtDescription.Text = dtCollection.Rows[0]["MoreDescription"].ToString();
        }
      }
      this.LoadGrid();
      this.NeedToSave = false;
      this.loadingData = false;
    }

    /// <summary>
    /// Reset screen, update this.collectionPid = long.MinValue
    /// </summary>
    private void Clear()
    {
      this.collectionPid = long.MinValue;
      txtUSCode.Text = string.Empty;
      txtCollection.Text = string.Empty;      
      this.LoadGrid();
    }
    #endregion Load Data

    #region Check Invalid Data & Save
    /// <summary>
    /// Check logic : colection is required
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      string collection = txtCollection.Text.Trim();
      if (collection.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Collection" });
        return false;
      }
      else
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CollectionName", DbType.String, 128, collection) };        
        string commandText = "Select Count(Value) From TblBOMCodeMaster Where [Group] = 2 And Value = @CollectionName";
        if (this.collectionPid != long.MinValue)
        {
          commandText = string.Format("{0} And Code <> {1}", commandText, this.collectionPid);          
        }
        int count = (int)DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
        if (count > 0)
        {
          WindowUtinity.ShowMessageError("MSG0006", new string[] { "This collection name" });
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Insert/Update record tblCSDColectionLanguage in database
    /// </summary>
    /// <param name="row"></param>
    /// <param name="collectionPid">key of Colection</param>
    /// <returns>true : success, false : unsuccess</returns>
    private bool SaveCollectionLanguage(DataRow row)
    {
      DBParameter[] inputParam = new DBParameter[5];
      string storeName = string.Empty;
      long pid = DBConvert.ParseLong(row["Pid"].ToString());
      if (pid != long.MinValue) //update
      {
        inputParam[4] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
        storeName = "spCSDCollectionLanguage_Update";
      }
      else //Insert
      {
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spCSDCollectionLanguage_Insert";
      }
      inputParam[1] = new DBParameter("@CollectionPid", DbType.Int64, collectionPid);

      long languagepid = DBConvert.ParseLong(row["LanguagePid"].ToString());
      inputParam[2] = new DBParameter("@LanguagePid", DbType.Int64, languagepid);

      string collectionName = row["CollectionName"].ToString().Trim();
      inputParam[3] = new DBParameter("@CollectionName", DbType.String, 128, collectionName);

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
    /// 1. Insert/Update Colection
    /// 2. Insert/Update/Delete some record in TblCSDColectionLanguage
    /// </summary>
    /// <returns>true : success, false : unsuccess</returns>
    private bool SaveData()
    {
      long pid = long.MinValue;
      // 1.Insert/update Collection
      DBParameter[] inputParam = new DBParameter[7];
      string storeName = string.Empty;      
      if (this.collectionPid != long.MinValue) // update
      {
        inputParam[0] = new DBParameter("@Code", DbType.Int32, this.collectionPid);
        inputParam[4] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spBOMCodeMaster_Update";        
      }
      else // Insert
      {
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spBOMCodeMaster_Insert";
      }
      string collection = txtCollection.Text.Trim();
      inputParam[1] = new DBParameter("@Value", DbType.String, 128, collection);
      inputParam[2] = new DBParameter("@Group", DbType.Int32, ConstantClass.GROUP_COLLECTION);
      inputParam[3] = new DBParameter("@DeleteFlag", DbType.Int32, 0);
      inputParam[5] = new DBParameter("@Description", DbType.AnsiString, 8, txtUSCode.Text.Trim());
      inputParam[6] = new DBParameter("@MoreDescription", DbType.String, 512, txtDescription.Text.Trim());

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      pid = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
      switch (pid)
      {
        case 0:
          WindowUtinity.ShowMessageError("ERR0005");
          return false;        
        default:
          this.collectionPid = pid;
          break;
      }
      bool result = true;
      //2. Insert/update/delete TblCSDCollectionLanguage
      //2.1 Delete TblCSDCollectionLanguage
      foreach (long detailPid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
        DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDCollectionLanguage_Delete", inputParamDelete, outputParamDelete);
        long outputValue = DBConvert.ParseLong(outputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      //2.2 Insert/update TblCSDCollectionLanguage
      DataTable dtSource = (DataTable)ultraGridOtherLanguage.DataSource;
      foreach (DataRow dr in dtSource.Rows)
      {
        if ((dr.RowState == DataRowState.Modified) && (dr["CollectionName"].ToString().Trim().Length > 0))
        {
          bool success = this.SaveCollectionLanguage(dr);
          if (!success)
          {
            result = false;
          }
        }
      }
      if (!result)
      {
        WindowUtinity.ShowMessageError("WRN0004");
        this.LoadData();
        return false;
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
      return true;
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
        if (this.SaveData())
        {
          this.SaveSuccess = true;
        }
        else
        {
          this.SaveSuccess = false;
        }
      }
      else
      {
        this.SaveSuccess = false;
      }
    }
    #endregion Check Invalid Data & Save

    #region Event
    /// <summary>
    /// Init layout for ultragrid view CollectionLanguage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridOtherLanguage_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridOtherLanguage);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LanguagePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Language"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CollectionName"].Header.Caption = "Collection Name";      
    }

    /// <summary>
    /// 1. Insert/Update Collection
    /// 2. Insert/Update/Delete some record in TblCSDCollectionLanguage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {        
        this.SaveData();        
      }
    }

    /// <summary>
    /// 1. Insert/Update Collection
    /// 2. Insert/Update/Delete some record in TblCSDCollectionLanguage
    /// 3. Reset screen for register new TblCSDCollection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveContinue_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        bool sucess = this.SaveData();
        if (sucess)
        {
          this.loadingData = true;
          this.Clear();
          this.loadingData = false;
        }
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
    /// Kiểm tra và add pid vào list để delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridOtherLanguage_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (!this.loadingData)
      {
        this.NeedToSave = ((btnSave.Visible) && (btnSaveContinue.Visible));
        string collectionName = e.Cell.Value.ToString().Trim();
        long pid = DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          if ((collectionName.Length == 0) && (!this.listDeletedPid.Contains(pid)))
          {
            this.listDeletedPid.Add(pid);
          }
          else if ((collectionName.Length > 0) && (this.listDeletedPid.Contains(pid)))
          {
            this.listDeletedPid.RemoveAt(this.listDeletedPid.IndexOf(pid));
          }
        }
      }
    }

    /// <summary>
    /// Set this.NeedToSave = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Object_Changed(object sender, EventArgs e)
    {
      if (!this.loadingData)
      {
        this.NeedToSave = (btnSave.Visible) && (btnSaveContinue.Visible);
      }
    }
    #endregion Event
  }
}
