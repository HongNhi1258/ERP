/*
  Author      :   Ha Anh
  Date        :   07/01/2014
  Description :   Create Option Set group
  Standard Form: view_SearchSave.cs
  Update By   : Huynh Thi Bang (25/09/2015)
*/
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
  public partial class viewCSD_07_005 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    #endregion field
    #region Init
    public viewCSD_07_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_07_005_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
      this.LoadFinishingCode();
    }
    #endregion Init
    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      string commandText = string.Format(@" SELECT	Pid, OPG.Code, OPG.Name, OPG.FinCode , OPG.[Description], ISNULL(NV.HoNV, '') + ' ' + ISNULL(NV.TenNV, '')                                                     CreateBy, OPG.CreateDate
                                            FROM  TblCSDOptionCodeForEcat OPG
                                              INNER JOIN	VHRNhanVien NV ON NV.ID_NhanVien = OPG.CreateBy
                                            ORDER BY OPG.Code ");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultraGridInformation.DataSource = dtSource;
      lbCount.Text = string.Format("Count: {0}", dtSource.Rows.Count);
    }

    private void LoadFinishingCode()
    {
      string commandText = "SELECT FinCode, [Description] FROM	TblBOMFinishingInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultraDropFinishingCode.DataSource = dtSource;
      ultraDropFinishingCode.ValueMember = "FinCode";
      ultraDropFinishingCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDropFinishingCode.DisplayLayout.Bands[0].Columns["Description"].Hidden = true;
      ultraDropFinishingCode.DisplayLayout.AutoFitColumns = true;
      //ultraGridInformation.DataSource = dtSource;
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
    /// Save data before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //if (ultCBWo.Text.Length == 0)
      //{
      //  errorMessage = "Work Order";      
      //  return false;
      //}
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[6];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@Code", DbType.String, 128, row["Code"].ToString());
            inputParam[2] = new DBParameter("@Name", DbType.String, 128, row["Name"].ToString());
            inputParam[3] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[4] = new DBParameter("@FinCode", DbType.String, row["FinCode"].ToString());
            inputParam[5] = new DBParameter("@Description", DbType.String, row["Description"].ToString());
            DataBaseAccess.ExecuteStoreProcedure("spCSDOptionSetGroupEcat_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.InitData();
          this.NeedToSave = false;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }
    #endregion function

    #region event
   
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;


      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      
      // Read only
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["FinCode"].ValueList = ultraDropFinishingCode;
      e.Layout.Bands[0].Columns["Description"].Header.Caption = "Marketing Name";
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      int count = 0;
      if (colName == "Code")
      {
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (e.Cell.Row.Cells["Code"].Text == ultraGridInformation.Rows[i].Cells["Code"].Text)
          {
            count++;
            if (count == 2)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Code");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
              break;
            }
          }
        }
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      string colName = e.Cell.Column.ToString();
      switch (colName)
      {
        case "FinCode":
          if (ultraDropFinishingCode.SelectedRow.Cells["FinCode"].Value.ToString() != null)
          {
            e.Cell.Row.Cells["Description"].Value = ultraDropFinishingCode.SelectedRow.Cells["Description"].Value.ToString();
          }
          else
          {
            e.Cell.Row.Cells["Description"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }
    #endregion event

    private void btnExport_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }
  }
}
