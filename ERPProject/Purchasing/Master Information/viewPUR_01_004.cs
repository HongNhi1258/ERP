/*
  Author      : Ha Anh
  Date        : 27/04/2011
  Description : Material Unit
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
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_01_004 : MainUserControl
  {
    #region variable
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool checkDelete = false;
    private bool checkDuplicate = false;
    #endregion variable

    #region Init Data
    public viewPUR_01_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_004_Load(object sender, EventArgs e)
    {
      this.LoadMaterialUnit();
    }

    /// <summary>
    /// Init list material
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultMaterialUnit_InittializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Load Material Unit
    /// </summary>
    private void LoadMaterialUnit()
    {
      DBParameter[] inputParam = new DBParameter[2];
      string text = string.Empty;
      text = txtSymbol.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[0] = new DBParameter("@Symbol", DbType.AnsiString, 8, "%" + text.Replace("'", "''") + "%");
      }
      text = txtDescription.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[1] = new DBParameter("@Description", DbType.String, 64, "%" + text.Replace("'", "''") + "%");
      }
      string storename = "spPURListMaterialUnit";
      DataSet dataset = DataBaseAccess.SearchStoreProcedure(storename, inputParam);
      DataTable dataTableMaterialUnit = dataset.Tables[0];
      ultMaterialUnit.DataSource = dataset;
      this.listDeletedPid = new ArrayList();
      this.listDeletingPid = new ArrayList();
      for (int i = 0; i < ultMaterialUnit.Rows.Count; i++)
      {
        long value = DBConvert.ParseLong(ultMaterialUnit.Rows[i].Cells["Pid"].Value.ToString());
        string sql = "SELECT * FROM TblGNRMaterialInformation WHERE Unit = " + value;
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(sql);
        if (dt.Rows.Count > 0)
        {
          ultMaterialUnit.Rows[i].Cells["Symbol"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
          ultMaterialUnit.Rows[i].Cells["Description"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
    }
    #endregion Load Data

    #region Event
    /// <summary>
    /// Search Data Material Unit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.LoadMaterialUnit();
      btnSearch.Enabled = true;
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.LoadMaterialUnit();
        btnSearch.Enabled = true;
      }
    }
    /// <summary>
    /// Save Material Unit Include Insert , Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check input before save
      bool check = ValidationInput(out message);
      if (!check)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      check = this.SaveData();
      if (check)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        if (this.checkDelete)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0054"), "Symbol");
          WindowUtinity.ShowMessageErrorFromText(message);
        }
        if (this.checkDuplicate)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Unit");
          WindowUtinity.ShowMessageErrorFromText(message);
        }
        //WindowUtinity.ShowMessageError("ERR0005");
      }

      // Load Material Unit
      this.txtSymbol.Text = string.Empty;
      this.txtDescription.Text = string.Empty;
      this.listDeletedPid = new ArrayList();
      this.listDeletingPid = new ArrayList();
      this.LoadMaterialUnit();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

    #region Method
    /// <summary>
    /// Save Data 
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;
      int count = ultMaterialUnit.Rows.Count;
      string storename = string.Empty;
      string text = string.Empty;
      long outputValue = long.MinValue;
      if (this.listDeletedPid != null)
      {
        foreach (long pid in this.listDeletedPid)
        {
          DBParameter[] inputParamDelete = new DBParameter[1];
          inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, pid);
          DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialUnit_Delete", inputParamDelete, OutputParamDelete);
          outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
          if (outputValue == 0)
          {
            result = false;
          }
          else if (outputValue == 2)
          {
            this.checkDelete = true;
            result = false;
          }
        }
      }
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultMaterialUnit.Rows[i];
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        DBParameter[] inputParam = new DBParameter[4];
        if (pid == long.MinValue)
        {
          storename = "spGNRMaterialUnit_Insert";
        }
        else
        {
          storename = "spGNRMaterialUnit_Update";
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
        }
        text = row.Cells["Symbol"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParam[1] = new DBParameter("@Symbol", DbType.AnsiString, 8, text);
        }
        text = row.Cells["Description"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParam[2] = new DBParameter("@Description", DbType.String, 64, text);
        }
        text = row.Cells["Remark"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParam[3] = new DBParameter("@Remark", DbType.String, 128, text);
        }
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outputParam);
        long outputvalue = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (outputvalue == 0)
        {
          result = false;
        }
        else if (outputvalue == 2)
        {
          result = false;
          this.checkDuplicate = true;
        }
      }
      return result;
    }

    /// <summary>
    /// Check input before save
    /// </summary>
    /// <returns></returns>
    private bool ValidationInput(out string message)
    {
      message = string.Empty;
      DataSet ds_grid = (DataSet)ultMaterialUnit.DataSource;
      if (ds_grid != null)
      {
        foreach (DataRow row_grid in ds_grid.Tables[0].Rows)
        {
          if (row_grid.RowState != DataRowState.Deleted)
          {
            if (row_grid["Symbol"].ToString().Length == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Symbol");
              return false;
            }
            if (row_grid["Description"].ToString().Length == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Description");
              return false;
            }
            DataRow[] foundRow_grid = ds_grid.Tables[0].Select("Symbol = '" + row_grid["Symbol"].ToString() + "' AND Description = '" + row_grid["Description"].ToString() + "'");
            if (foundRow_grid.Length > 1)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0006"), "The Gird");
              return false;
            }
          }
        }
      }
      return true;
    }
    #endregion Method

    #region UtraGridView Handle Event
    /// <summary>
    /// Add List Pid To Arraylist for delete material unit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultMaterialUnit_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// Delete Material Unit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultMaterialUnit_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }
    #endregion UtraGridView Handle Event
  }
}
