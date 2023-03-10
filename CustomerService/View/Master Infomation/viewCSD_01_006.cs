/*
  Author      : Lậm Quang Hà
  Date        : 09/10/2010
  Decription  : Search Category 
  Checked by    : Võ Hoa Lư
  Checked date  : 12/10/2010
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
  public partial class viewCSD_01_006 : MainUserControl
  {
    #region Field
    public long categoryPid = long.MinValue;
    private bool loadingData = false;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_006()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started 
    /// 1. Init list items for ultradropdown language.
    /// 2. Init list items for dropdownlist parent category.
    /// 3. Load category's information.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_01_006_Load(object sender, EventArgs e)
    {
      this.LoadParentCategory();      
      this.LoadData();
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Init list items for dropdownlist parent category.
    /// </summary>
    private void LoadParentCategory()
    {
      string commandText = string.Format("SELECT Pid, Category FROM TblCSDCategory WHERE ParentPid IS NULL AND Pid <> {0} ORDER BY Category", this.categoryPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadMultiCombobox(multiComboBoxParent, dt, "Pid", "Category");
      multiComboBoxParent.ColumnWidths = "0, 450";
    }
    
    /// <summary>
    /// Load list category language.
    /// </summary>
    private void LoadGrid()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CategoryPid", DbType.Int64, this.categoryPid) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCategoryLanguage_Select", inputParam);      
      ultraGridOtherLanguage.DataSource = dtSource;
    }

    /// <summary>
    /// Load category information.
    /// </summary>
    private void LoadData()
    {
      this.loadingData = true;
      this.listDeletedPid = new ArrayList();
      if (this.categoryPid != long.MinValue)
      {
        CSDCategory category = new CSDCategory();
        category.Pid = this.categoryPid;
        category = (CSDCategory)DataBaseAccess.LoadObject(category, new string[] { "Pid" });
        if (category == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return;
        }
        txtUSCode.Text = category.USCateCode;
        txtCategory.Text = category.Category;
        txtDescription.Text = category.MoreDescription;
        try
        {
          multiComboBoxParent.SelectedValue = category.ParentPid;
        }
        catch { }        
      }
      this.LoadGrid();
      this.NeedToSave = false;
      this.loadingData = false;
    }

    /// <summary>
    /// Reset screen, update this.categoryPid = long.MinValue, 
    /// </summary>
    private void Clear()
    {
      this.categoryPid = long.MinValue;
      txtUSCode.Text = string.Empty;
      txtCategory.Text = string.Empty;      
      this.LoadGrid();
      this.LoadParentCategory();
    }
    #endregion Load Data

    #region Check Invalid Data & Save
    /// <summary>
    /// Check logic : category is required
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      //if (txtUSCode.Text.Trim().Length == 0)
      //{
      //  WindowUtinity.ShowMessageError("MSG0005", new string[] { "US Code" });
      //  return false;
      //}
      if (txtCategory.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Category" });
        return false;
      }
      return true;
    }

    /// <summary>
    /// Insert/Update record tblCSDCategoryLanguage in database
    /// </summary>
    /// <param name="row"></param>
    /// <param name="categoryPid">key of Category</param>
    /// <returns>true : success, false : unsuccess</returns>
    private bool SaveCategoryLanguage(DataRow row, long categoryPid)
    {
      DBParameter[] inputParam = new DBParameter[5];
      string storeName = string.Empty;
      long pid = DBConvert.ParseLong(row["Pid"].ToString());
      if (pid != long.MinValue) //update
      {
        inputParam[4] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
        storeName = "spCSDCategoryLanguage_Update";
      }
      else //Insert
      {
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spCSDCategoryLanguage_Insert";
      }
      inputParam[1] = new DBParameter("@CategoryPid", DbType.Int64, categoryPid);

      long languagepid = DBConvert.ParseLong(row["LanguagePid"].ToString());
      inputParam[2] = new DBParameter("@LanguagePid", DbType.Int64, languagepid);

      string categoryName = row["CategoryName"].ToString().Trim();
      inputParam[3] = new DBParameter("@CategoryName", DbType.String, 128, categoryName);

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
    /// 1. Insert/Update TblCSDCategory
    /// 2. Insert/Update/Delete some record in TblCSDCategoryLanguage
    /// </summary>
    /// <returns>true : success, false : unsuccess</returns>
    private bool SaveData()
    {
      long pid = long.MinValue;
      // 1.Insert/update TblCSDCategory
      DBParameter[] inputParam = new DBParameter[6];
      string storeName = string.Empty;      
      if (this.categoryPid != long.MinValue) // update
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.categoryPid);
        inputParam[3] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spCSDCategory_Update";        
      }
      else // Insert
      {
        inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spCSDCategory_Insert";
      }
      string category = txtCategory.Text.Trim();
      inputParam[1] = new DBParameter("@Category", DbType.String, 128, category);
         
      long parentPid = DBConvert.ParseLong(ControlUtility.GetSelectedValueMultiCombobox(multiComboBoxParent));
      if (parentPid != long.MinValue)
      {
        inputParam[2] = new DBParameter("@ParentPid", DbType.Int32, parentPid);
      }
      inputParam[4] = new DBParameter("@USCode", DbType.String, 8, txtUSCode.Text.Trim());
      inputParam[5] = new DBParameter("@Description", DbType.String, 512, txtDescription.Text.Trim());

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      pid = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
      switch (pid)
      {
        case 0:
          WindowUtinity.ShowMessageError("ERR0005");
          return false;
        case -1:
          WindowUtinity.ShowMessageError("ERR0050", "Parent category is invalid.");
          return false;
        case -2:
          WindowUtinity.ShowMessageError("ERR0050", "This category was parent category, can't not add parent category to one.");
          return false;
        default:
          this.categoryPid = pid;
          break;
      }
      bool result = true;
      //2. Insert/update/delete TblCSDCategoryLanguage
      //2.1 Delete TblCSDCategoryLanguage
      foreach (long detailPid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
        DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDCategoryLanguage_Delete", inputParamDelete, outputParamDelete);
        long outputValue = DBConvert.ParseLong(outputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      //2.2 Insert/update TblCSDCategoryLanguage
      DataTable dtSource = (DataTable)ultraGridOtherLanguage.DataSource;
      foreach (DataRow dr in dtSource.Rows)
      {
        if ((dr.RowState == DataRowState.Modified) && (dr["CategoryName"].ToString().Trim().Length > 0))
        {
          bool success = this.SaveCategoryLanguage(dr, this.categoryPid);
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
    /// Init layout for ultragrid view CategoryLanguage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridOtherLanguage_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridOtherLanguage);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LanguagePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Language"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CategoryName"].Header.Caption = "Category Name";      
    }

    /// <summary>
    /// 1. Insert/Update TblCSDCategory
    /// 2. Insert/Update/Delete some record in TblCSDCategoryLanguage
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
    /// 1. Insert/Update TblCSDCategory
    /// 2. Insert/Update/Delete some record in TblCSDCategoryLanguage
    /// 3. Reset screen for register new TblCSDCategory
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
        string categoryName = e.Cell.Value.ToString().Trim();
        long pid = DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          if ((categoryName.Length == 0) && (!this.listDeletedPid.Contains(pid)))
          {
            this.listDeletedPid.Add(pid);
          }
          else if ((categoryName.Length > 0) && (this.listDeletedPid.Contains(pid)))
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
