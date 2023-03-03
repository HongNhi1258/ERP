/*
  Author      : Dang 
  Date        : 12/07/2012
  Description : Cancel PR
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
using System.Drawing;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_03_006 : MainUserControl
  {
    #region Field
    public ArrayList arrList = new ArrayList();
    #endregion Field

    #region Init
    public viewPUR_03_006()
    {
      InitializeComponent();
    }

    private void viewPUR_03_006_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function

    private void LoadData()
    {
      DataTable dtData = this.DataShow();
      for (int i = 0; i < arrList.Count; i++)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@PRDetailPid", DbType.Int64, arrList[i]);
        DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURListPRNeedCancel_Select", input);

        if (dt != null && dt.Rows.Count == 1)
        {
          DataRow row = dtData.NewRow();
          row["Pid"] = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
          row["PRNo"] = dt.Rows[0]["PRNo"].ToString();
          row["MaterialCode"] = dt.Rows[0]["MaterialCode"].ToString();
          row["NameEN"] = dt.Rows[0]["NameEN"].ToString();
          row["Symbol"] = dt.Rows[0]["Symbol"].ToString();
          row["Status"] = dt.Rows[0]["Status"].ToString();
          row["Quantity"] = DBConvert.ParseDouble(dt.Rows[0]["Quantity"].ToString());
          row["Flag"] = DBConvert.ParseInt(dt.Rows[0]["Flag"].ToString());

          dtData.Rows.Add(row);
        }
      }
      ultData.DataSource = dtData;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) == 1)
        {
          row.CellAppearance.BackColor = Color.Yellow;
        }
      }

      if (this.ultData.Rows.Count == 0)
      {
        this.btnCancel.Enabled = false;
      }
    }

    private DataTable DataShow()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("PRNo", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("Symbol", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Quantity", typeof(System.Double));
      taParent.Columns.Add("Flag", typeof(System.Int32));
      return taParent;
    }

    /// <summary>
    ///  Cancel PO
    /// </summary>
    /// <returns></returns>
    private bool CancelPR()
    {
      string storeName = "spPURStatusPRPOWhenCancelPR_Update";
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@PRNo", DbType.String, row.Cells["PRNo"].Value.ToString());
        inputParam[1] = new DBParameter("@PRDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.AutoFitColumns = true;

      e.Layout.Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) == 1)
        {
          WindowUtinity.ShowMessageError("WRN0004");
          return;
        }
      }

      bool success = this.CancelPR();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.CloseTab();
    }
    private void btnCLose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event  
  }
}
