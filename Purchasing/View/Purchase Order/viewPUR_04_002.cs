/*
  Author      : Dang 
  Date        : 11/07/2012
  Description : Cancel PO And End Receiving
              : Flag = 0(Cancel PO)
              : Flag = 1(End Receiving)
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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_04_002 : MainUserControl
  {
    #region Field
    public ArrayList arrList = new ArrayList();
    public int flag = int.MinValue;
    #endregion Field

    #region Init
    public viewPUR_04_002()
    {
      InitializeComponent();
    }

    private void viewPUR_04_002_Load(object sender, EventArgs e)
    {
      if (this.flag == 0)
      {
        btnEndReceiving.Visible = false;
        btnCancel.Visible = true;
      }
      else
      {
        btnCancel.Visible = false;
        btnEndReceiving.Visible = true;
      }
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
        input[0] = new DBParameter("@PODetailPid", DbType.Int64, arrList[i]);
        DataTable dt = new DataTable();
        if (flag == 1)
        {
          dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURListPONeedEndReceiving_Select", input);
        }
        else if (flag == 0)
        {
          dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURListPONeedCancel_Select", input);
        }
        if (dt != null && dt.Rows.Count == 1)
        {
          DataRow row = dtData.NewRow();
          row["Pid"] = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
          row["PONo"] = dt.Rows[0]["PONo"].ToString();
          row["PRNo"] = dt.Rows[0]["PRNo"].ToString();
          row["MaterialCode"] = dt.Rows[0]["MaterialCode"].ToString();
          row["NameEN"] = dt.Rows[0]["NameEN"].ToString();
          row["Symbol"] = dt.Rows[0]["Symbol"].ToString();
          row["Status"] = dt.Rows[0]["Status"].ToString();
          row["Quantity"] = DBConvert.ParseDouble(dt.Rows[0]["Quantity"].ToString());
          row["ReceiptedQty"] = DBConvert.ParseDouble(dt.Rows[0]["ReceiptedQty"].ToString());
          row["QtyCancel"] = DBConvert.ParseDouble(dt.Rows[0]["QtyCancel"].ToString());
          if (flag == 0)
          {
            row["Flag"] = DBConvert.ParseInt(dt.Rows[0]["Flag"].ToString());
            row["FlagCancelPR"] = DBConvert.ParseInt(dt.Rows[0]["FlagCancelPR"].ToString());
            row["FlagPROnline"] = DBConvert.ParseInt(dt.Rows[0]["FlagPROnline"].ToString());
          }
          dtData.Rows.Add(row);
        }
      }
      ultData.DataSource = dtData;
      if (flag == 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) == 1)
          {
            row.CellAppearance.BackColor = Color.Yellow;
          }

          if (DBConvert.ParseInt(row.Cells["FlagPROnline"].Value.ToString()) == 1)
          {
            row.Cells["FlagCancelPR"].Activation = Activation.ActivateOnly;
          }
          else
          {
            row.Cells["FlagCancelPR"].Activation = Activation.AllowEdit;
          }
        }
      }

      if (this.ultData.Rows.Count == 0)
      {
        this.btnCancel.Enabled = false;
        this.btnEndReceiving.Enabled = false;
      }
    }

    private DataTable DataShow()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("PRNo", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("Symbol", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Quantity", typeof(System.Double));
      taParent.Columns.Add("ReceiptedQty", typeof(System.Double));
      taParent.Columns.Add("QtyCancel", typeof(System.Double));
      if (flag == 0)
      {
        taParent.Columns.Add("Flag", typeof(System.Int32));
        taParent.Columns.Add("FlagCancelPR", typeof(System.Int32));
        taParent.Columns.Add("FlagPROnline", typeof(System.Int32));
      }
      return taParent;
    }

    /// <summary>
    ///  Cancel PO
    /// </summary>
    /// <returns></returns>
    private bool UpdatePRPO()
    {
      string storeName = string.Empty;
      if (flag == 0)
      {
        storeName = "spPURStatusPRPOWhenCancelPO_Update";
      }
      else
      {
        storeName = "spPURStatusPRPOWhenEndReceiving_Update";
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
        if (flag == 0)
        {
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@PONo", DbType.String, row.Cells["PONo"].Value.ToString());
          inputParam[1] = new DBParameter("@PODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          inputParam[2] = new DBParameter("@CancelPR", DbType.Int32, DBConvert.ParseInt(row.Cells["FlagCancelPR"].Value.ToString()));
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        }
        else if (flag == 1)
        {
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@PONo", DbType.String, row.Cells["PONo"].Value.ToString());
          inputParam[1] = new DBParameter("@PODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          inputParam[2] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        }

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
      if (flag == 1)
      {
        e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      // Cancel
      if (flag == 0)
      {
        e.Layout.Bands[0].Columns["Flag"].Hidden = true;
        //e.Layout.Bands[0].Columns["FlagCancelPR"].Hidden = true;
        e.Layout.Bands[0].Columns["FlagPROnline"].Hidden = true;
        e.Layout.Bands[0].Columns["FlagCancelPR"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        e.Layout.Bands[0].Columns["FlagCancelPR"].Header.Caption = "Cancel PR";
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ReceiptedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

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

      bool success = this.UpdatePRPO();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      //this.LoadData();
      this.CloseTab();
    }

    private void btnEndReceiving_Click(object sender, EventArgs e)
    {
      bool success = this.UpdatePRPO();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      //this.LoadData();
      this.CloseTab();
    }

    private void btnCLose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event  
  }
}
