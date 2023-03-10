/*
  Author      : Lậm Quang Hà
  Date        : 07/10/2010
  Decription  : Insert, Update Nation
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
  public partial class viewCSD_01_004 : MainUserControl
  {
    #region Field
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init Data

    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_004()
    {
      InitializeComponent();
    }
    #endregion Init Data

    #region Search

    /// <summary>
    /// Search Nation infomation from name and zipcode condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[2];
      string zipCode = txtZipCode.Text.Trim();
      if (zipCode.Length > 0)
      {
        param[0] = new DBParameter("@ZipCode", DbType.AnsiString, 10, "%" + zipCode + "%");
      }
      string name = txtNameNation.Text.Trim();
      if (name.Length > 0)
      {
        param[0] = new DBParameter("@Name", DbType.String, 130, "%" + name + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDNation_Select", param);
      ultraGridNation.DataSource = dtSource;
      this.NeedToSave = false;
    }
    #endregion Search

    #region Save Data
    private bool CheckInValid()
    {
      bool result = true;
      DataTable dt = (DataTable)ultraGridNation.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow row = dt.Rows[i];
        //PartCode
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          string terrior = row["NationCode"].ToString();
          //string cmpart = string.Format("SELECT Pid FROM TblWIPFurniturePartcode WHERE Pid = {0}", part);
          string cm = string.Format(@"SELECT Value , Value Display
                                    FROM TblBOMCodeMaster CM 
                                    WHERE  CM.[Group] = 1011 AND Value ='{0}'", terrior);
          DataTable dt1 = DataBaseAccess.SearchCommandTextDataTable(cm);
          if (dt1.Rows.Count <= 0 || dt1 == null)
          {
            WindowUtinity.ShowMessageError("ERR0115", "TerriorCode");
            result = false;
          }
        }
      }
      return result;
    }


    /// <summary>
    /// Invert/update/delete some record in screen into table TblCSDNation in database
    /// </summary>
    private void SaveData()
    {
      //1.delete
      foreach (long pid in this.listDeletedPid)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDNation_Delete", inputParam, outputParam);
        long success = DBConvert.ParseInt(outputParam[0].Value.ToString());
        if (success == 0)
        {
          WindowUtinity.ShowMessageError("ERR0005");
          return;
        }
      }
      //2.Insert, UpDate
      DataTable dtNation = (DataTable)ultraGridNation.DataSource;
      string storeName = string.Empty;
      foreach (DataRow row in dtNation.Rows)
      {
        if ((row.RowState == DataRowState.Added) || (row.RowState == DataRowState.Modified))
        {
          DBParameter[] inputParam = new DBParameter[7];
          if (row.RowState == DataRowState.Modified)
          {
            storeName = "spCSDNation_Update";
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            inputParam[5] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          else
          {
            storeName = "spCSDNation_Insert";
            inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          string zipCode = row["ZipCode"].ToString();
          string nationCode = row["NationCode"].ToString();
          string nameEN = row["NationEN"].ToString();
          string nameVN = row["NationVN"].ToString();
          int orderby = DBConvert.ParseInt(row["OrderBy"].ToString());

          inputParam[1] = new DBParameter("@ZipCode", DbType.String, 8, zipCode);
          inputParam[2] = new DBParameter("@NationEN", DbType.String, 256, nameEN);
          inputParam[3] = new DBParameter("@NationVN", DbType.String, 256, nameVN);
          if (orderby != int.MinValue)
          {
            inputParam[4] = new DBParameter("@OrderBy", DbType.Int32, orderby);
          }
          inputParam[6] = new DBParameter("@NationCode", DbType.String, 16, nationCode);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0005");
            return;
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
    }

    private void LoadTerriorCode()
    {

      string commandText = string.Format(@"   SELECT Value , Value Display
                                            FROM TblBOMCodeMaster CM 
                                            WHERE  CM.[Group] = 1011");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraDropDown(ultddTerrior, dt, "Value", "Display", "Value");
      ultddTerrior.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Confirm before close
    /// YES : Save and close
    /// No  : Close without save
    /// Cancle : Nothing
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion Save Data

    #region Event

    /// <summary>
    /// Search Nation infomation from name and zipcode condition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Confirm before close
    /// YES : Save and close
    /// No  : Close without save
    /// Cancle : Nothing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Invert/update/delete some record in screen into table TblCSDNation in database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInValid())
      {
        this.SaveData();
      }
    }

    /// <summary>
    /// Init layout for ultragrid view Nation Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridNation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridNation);
      e.Layout.Bands[0].Columns["NationCode"].ValueList = this.ultddTerrior;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["OrderBy"].Header.Caption = "No";
      e.Layout.Bands[0].Columns["OrderBy"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["OrderBy"].MinWidth = 60;
      e.Layout.Bands[0].Columns["OrderBy"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ZipCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ZipCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ZipCode"].Header.Caption = "Phone Code";
      e.Layout.Bands[0].Columns["NationCode"].Header.Caption = "Terrior Code";
      e.Layout.Bands[0].Columns["ZipCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["NationEN"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["NationVN"].Header.Caption = "Vietnamese Name";
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
    }

    /// <summary>
    /// Get list pids of nations which are deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridNation_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    /// <summary>
    /// Get list pids of nations which are deleting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridNation_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
    /// Set this.NeedToSave = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridNation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
    }
    #endregion Event

    private void viewCSD_01_004_Load(object sender, EventArgs e)
    {
      this.LoadTerriorCode();
    }
  }
}
