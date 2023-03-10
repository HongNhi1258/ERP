/*
  Author      : Lậm Quang Hà
  Date        : 07/10/2010
  Decription  : Insert/Update/Delete Language
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
  public partial class viewCSD_01_003 : MainUserControl
  {
    #region Field
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_003()
    {
      InitializeComponent();
    }
    #endregion Init Data

    #region Search
    /// <summary>
    /// Search Language infomation from name condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[1];
      string name = txtName.Text.Trim();
      if (name.Length > 0)
      {
        param[0] = new DBParameter("@Name", DbType.String, 130, "%" + name + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDLanguage_Select", param);
      ultraGridLanguage.DataSource = dtSource;
      this.NeedToSave = false;
    }
    #endregion Search

    #region Save Data
    /// <summary>
    /// Invert/update/delete some record in screen into table TblCSDLanguage in database
    /// </summary>
    private void SaveData()
    {
      foreach (long pid in this.listDeletedPid)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDLanguage_Delete", inputParam, outputParam);
        long success = DBConvert.ParseInt(outputParam[0].Value.ToString());
        if (success == 0)
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
      }
      //2.Insert, UpDate
      DataTable dtLanguage = (DataTable)ultraGridLanguage.DataSource;
      string storeName = string.Empty;
      foreach (DataRow row in dtLanguage.Rows)
      {
        if ((row.RowState == DataRowState.Added) || (row.RowState == DataRowState.Modified))
        {
          DBParameter[] inputParam = new DBParameter[5];
          if (row.RowState == DataRowState.Modified)
          {
            storeName = "spCSDLanguage_Update";
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            inputParam[4] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          else
          {
            storeName = "spCSDLanguage_Insert";
            inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          string nameEN = row["NameEN"].ToString();
          string nameVN = row["NameVN"].ToString();
          int orderby = DBConvert.ParseInt(row["OrderBy"].ToString());

          inputParam[1] = new DBParameter("@NameEN", DbType.String, 128, nameEN);
          inputParam[2] = new DBParameter("@NameVN", DbType.String, 128, nameVN);
          if (orderby != int.MinValue)
          {
            inputParam[3] = new DBParameter("@OrderBy", DbType.Int32, orderby);
          }
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0005");
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
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
      this.SaveData();
    }
    #endregion Save Data

    #region Event
    /// <summary>
    /// Search Language infomation from name condition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
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
    /// Invert/update/delete some record in screen into table TblCSDLanguage in database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      //Save Data
      this.SaveData();
    }

    /// <summary>
    /// Init layout for ultragrid view Languege Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridLanguage_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["OrderBy"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["OrderBy"].MinWidth = 60;
      e.Layout.Bands[0].Columns["OrderBy"].Header.Caption = "No";
      e.Layout.Bands[0].Columns["OrderBy"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Vietnamese Name";
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
    }

    /// <summary>
    /// Get list pids of languages which are deleting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridLanguage_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long Pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (Pid != long.MinValue)
        {
          listDeletingPid.Add(Pid);
        }
      }
    }

    /// <summary>
    /// Get list pids of languages which are deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridLanguage_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    /// <summary>
    /// Set this.NeedToSave = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridLanguage_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
    }    
    #endregion Event    
  }
}