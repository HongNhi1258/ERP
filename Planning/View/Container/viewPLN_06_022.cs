/*
  Author  : 
  Date    : 27/04/2012
  Description : Confirm WH CBM
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_06_022 : MainUserControl
  {
    #region Field
    public long containerPid = long.MinValue;
    private int status = 0;
    #endregion Field

    #region Init
    public viewPLN_06_022()
    {
      InitializeComponent();
    }

    /// <summary>
    /// User Control Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_021_Load(object sender, EventArgs e)
    {
      this.Search();
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Search Box Information
    /// </summary>
    private void Search()
    {
      string commandText = string.Empty;
      commandText += " SELECT ContainerNo";
      commandText += " FROM TblPLNSHPContainer";
      commandText += " WHERE Pid = " + this.containerPid;

      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      this.txtContainerList.Text = dtCheck.Rows[0][0].ToString();

      commandText = string.Empty;
      commandText += " SELECT CM.Value ItemGroup, CLD.ItemGroup ItemGroupPid, CLD.ItemCode, CLD.Revision, IB.SaleCode, ";
      commandText += "     ROUND((CAST(PAK.TotalCBM AS FLOAT)/PAK.QuantityItem) , 2) 'CBM/ItemCode', ";
      commandText += "     SUM(CLD.Qty) PlanningQty, SUM(CLD.Qty) * ROUND((CAST(PAK.TotalCBM AS FLOAT)/PAK.QuantityItem) , 2) PLNCBM, ";
      commandText += "     SUM(CLD.WHQty) WHQty, SUM(CLD.WHQty) * ROUND((CAST(PAK.TotalCBM AS FLOAT)/PAK.QuantityItem) , 2) WHCBM, ";
      commandText += "     CASE WHEN SUM(CLD.Qty) <> SUM(CLD.WHQty) THEN 1 ELSE 0 END Flag ";
      commandText += " FROM TblPLNSHPContainer CON ";
      commandText += "     INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
      commandText += "     INNER JOIN TblPLNContainerListDetail CLD ON COD.LoadingListPid = CLD.ContainerListPid ";
      commandText += "     INNER JOIN TblBOMItemBasic IB ON CLD.ItemCode = IB.ItemCode ";
      commandText += "     INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 4001 AND CM.Code = CLD.ItemGroup ";
      commandText += "     INNER JOIN TblBOMPackage PAK ON CLD.ItemCode = PAK.ItemCode ";
      commandText += " 		                      			AND CLD.Revision = PAK.Revision ";
      commandText += " 					                      AND PAK.[SetDefault] = 1 ";
      commandText += " WHERE CON.Pid = " + this.containerPid;
      commandText += " GROUP BY CM.Value, IB.SaleCode, CLD.ItemCode, CLD.Revision, PAK.TotalCBM, PAK.QuantityItem, CLD.ItemGroup ";
      commandText += " ORDER BY CM.Value, CLD.ItemCode ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDetail.DataSource = dtSource;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) != 0)
        {
          row.CellAppearance.BackColor = Color.LightBlue;
        }
      }

      commandText = string.Empty;
      commandText += " SELECT COUNT(*) ";
      commandText += " FROM TblPLNSHPContainer CO ";
      commandText += " 	INNER JOIN TblPLNSHPContainerDetails COD ON CO.Pid = COD.ContainerPid ";
      commandText += " 	INNER JOIN TblPLNContainerList CL ON COD.LoadingListPid = CL.Pid";
      commandText += " WHERE CO.Pid = " + this.containerPid;
      commandText += " 		AND CL.[Status] >= 2";

      dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dtSource.Rows[0][0].ToString()) == 0)
        {
          this.status = 0;
        }
        else
        {
          this.status = 1;
        }
      }

      if (this.status == 1)
      {
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultDetail.Rows[i];
          row.Activation = Activation.ActivateOnly;
        }

        this.chkConfirm.Enabled = false;
        this.btnSave.Enabled = false;
      }
      else
      {
        this.chkConfirm.Enabled = true;
        this.btnSave.Enabled = true;
      }
    }
    #endregion LoadData

    #region Process
    /// <summary>
    /// Check Data (Equal WH Qty & PLN Qty)
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidFinishInfo(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];

        if (DBConvert.ParseInt(row.Cells["WHQty"].Value.ToString()) == int.MinValue)
        {
          message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "WH Qty");
          return false;
        }
        if (DBConvert.ParseInt(row.Cells["WHQty"].Value.ToString())
              > DBConvert.ParseInt(row.Cells["PlanningQty"].Value.ToString()))
        {
          message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO124"), "PLN Qty", "WH Qty");
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Create DataTable Before Save
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTableBeforeSave()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("ItemGroup", typeof(System.Int32));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("WHQty", typeof(System.Int32));
      return dt;
    }

    private void SendEmail()
    {
      Email email = new Email();
      email.Key = email.KEY_PLN_007;
      ArrayList arrList = email.GetDataMain(email.Key);
      if (arrList.Count == 3)
      {
        string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
        string subject = string.Format(arrList[1].ToString(), "Container " + this.txtContainerList.Text, "confirmed", " ", userName);
        string body = string.Format(arrList[2].ToString(), "Container " + this.txtContainerList.Text, "confirmed", " ", userName);
        email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
      }
    }
    #endregion Process

    #region Event
    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";

      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemGroupPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["PlanningQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WHQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM/ItemCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["PLNCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WHCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.NoEdit;
      }

      e.Layout.Bands[0].Columns["WHQty"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["PlanningQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign
                = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["WHQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign
              = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["PLNCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign
              = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["WHCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign
              = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:###,##0.00}";
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValidFinishInfo(out message);
      if (!success)
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Create dt Before Save
      DataTable dtSource = this.CreateDataTableBeforeSave();

      // Get Main Data Issuing Detail
      DataTable dtMain = (DataTable)this.ultDetail.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        DataRow row = dtSource.NewRow();
        row["ItemGroup"] = DBConvert.ParseInt(drMain["ItemGroupPid"].ToString());
        row["ItemCode"] = drMain["ItemCode"].ToString();
        row["Revision"] = DBConvert.ParseInt(drMain["Revision"].ToString());
        row["WHQty"] = DBConvert.ParseInt(drMain["WHQty"].ToString());
        dtSource.Rows.Add(row);
      }

      SqlCommand cm = new SqlCommand("spWHDContainerWHQtyConfirmed_Update");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;
      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      // Container Pid
      para = cm.CreateParameter();
      para.ParameterName = "@ContainerPid";
      para.DbType = DbType.Int64;
      para.Value = this.containerPid;
      cm.Parameters.Add(para);

      // Confirm
      para = cm.CreateParameter();
      para.ParameterName = "@Confirm";
      para.DbType = DbType.Int32;
      if (this.chkConfirm.Checked)
      {
        para.Value = 1;
      }
      else
      {
        para.Value = 0;
      }

      cm.Parameters.Add(para);

      try
      {
        if (cm.Connection.State != ConnectionState.Open)
        {
          cm.Connection.Open();
        }
        cm.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        string a = ex.Message;
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(a);
        return;
      }
      finally
      {
        if (cm.Connection.State != ConnectionState.Closed)
        {
          cm.Connection.Close();
        }
        cm.Connection.Dispose();
        cm.Dispose();
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");

      this.SendEmail();

      this.Search();
      //this.btnSave.Enabled = false;
    }
    #endregion Event
  }
}
