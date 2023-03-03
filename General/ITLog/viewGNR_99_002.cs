/*
  Author      : 
  Date        : 18/01/2013
  Description : Register Function Base On Department
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.General
{
  public partial class viewGNR_99_002 : MainUserControl
  {
    #region variable 
    public string department = string.Empty;
    public int kind = int.MinValue;

    private IList listDeletingCategory = new ArrayList();
    private IList listDeletedCategory = new ArrayList();
    #endregion variable

    #region Init
    public viewGNR_99_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_002_Load(object sender, EventArgs e)
    {
      txtDepartment.Text = this.department;
      this.LoadData(this.kind);
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].DefaultCellValue = 0;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string value = ultData.Rows[i].Cells["Value"].Value.ToString();
        if (value.Length > 0)
        {
          ultData.Rows[i].Cells["Value"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }

      e.Layout.Bands[0].Columns["Value"].Header.Caption = "Function";
      e.Layout.Bands[0].Columns["Code"].Hidden = true;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Init

    #region Load Data
    /// <summary>
    /// Load Data
    /// </summary>
    /// <param name="group"></param>
    private void LoadData(int code)
    {
      string commandText = string.Empty;
      commandText += " SELECT Code, Value, [Description], 0 [Selected] ";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 99002 ";
      commandText += "  	AND DeleteFlag = 0 ";
      commandText += "  	AND Kind = " + code;

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dtSource;
    }
    #endregion Load Data

    #region Validation
    /// <summary>
    /// Check Input
    /// </summary>
    /// <param name="warehouse"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private bool ValidationInput(DataTable dtSource)
    {
      bool result = true;
      string commandText = string.Empty;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row["Value"].ToString().Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Function");
          result = false;
          return result;
        }

        if (row.RowState == DataRowState.Added)
        {
          commandText += " SELECT COUNT(*) ";
          commandText += " FROM TblBOMCodeMaster ";
          commandText += " WHERE [Group] = 99002 ";
          commandText += "  	AND DeleteFlag = 0 ";
          commandText += "  	AND Kind = " + kind;
          commandText += "  	AND Value = '" + row["Value"].ToString() + "'";

          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) > 0)
          {
            WindowUtinity.ShowMessageError("ERR0028", "Function");
            result = false;
            return result;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Check Delete
    /// </summary>
    /// <returns></returns>
    private bool ValidateDelete()
    {
      bool result = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
        if (selected == 1)
        {
          string commandText = string.Empty;
          commandText += " SELECT COUNT(*) ";
          commandText += " FROM TblGNRITLogDaily ";
          commandText += " WHERE [Function] = " + DBConvert.ParseInt(ultData.Rows[i].Cells["Code"].Value.ToString());
          commandText += "     AND Kind =" + kind;
          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) > 0)
          {
            WindowUtinity.ShowMessageError("ERR0054", ultData.Rows[i].Cells["Value"].Value.ToString());
            result = false;
            break;
          }
        }
      }
      return result;
    }
    #endregion Validation

    #region Event
    /// <summary>
    /// Close tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        int countCheck = 0;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            countCheck++;
          }
        }

        if (countCheck == 0)
        {
          return;
        }

        DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
        if (result == DialogResult.Yes)
        {
          bool check = ValidateDelete();
          if (check)
          {
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
              if (selected == 1)
              {
                int code = DBConvert.ParseInt(ultData.Rows[i].Cells["Code"].Value.ToString());
                DBParameter[] input = new DBParameter[2];
                
                input[0] = new DBParameter("@Code", DbType.Int32, code);
                input[1] = new DBParameter("@Kind", DbType.Int32, kind);

                DataBaseAccess.ExecuteStoreProcedure("spGNRITLogFunction_Delete", input);
              }
            }
            WindowUtinity.ShowMessageSuccess("MSG0002");
          }

          // Load Grid
          this.LoadData(kind);
        }
      }
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      bool checkSave = ValidationInput(dtSource);

      if (checkSave)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          string storeName = string.Empty;
          DBParameter[] input = new DBParameter[4];
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            string value = row["Value"].ToString().Trim();
            string remark = row["Description"].ToString().Trim();

            input[0] = new DBParameter("@Value", DbType.String, 128, value);
            input[1] = new DBParameter("@Remark", DbType.String, 128, remark);
            input[3] = new DBParameter("@Kind", DbType.Int32, kind);

            if (row.RowState == DataRowState.Added)
            {
              input[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              storeName = "spGNRITLogFunction_Insert";
            }
            else if (row.RowState == DataRowState.Modified)
            {
              storeName = "spGNRITLogFunction_Update";
            }

            if (storeName.Length > 0)
            {
              DataBaseAccess.ExecuteStoreProcedure(storeName, input);
            }
          }
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");

        this.LoadData(kind);
      }   
    }
    #endregion Event

    #region UtraGridView Handle Event      
    #endregion UtraGridView Handle Event
  }
}
