/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewPLN_98_024.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_024 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    public DataTable dtSource;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.SearchData();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      ultraGridInformation.DataSource = this.dtSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow dr = ultraGridInformation.Rows[i];

        //New 
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) == 0)
        {
          dr.CellAppearance.BackColor = Color.White;
        }
        else
        {
          dr.CellAppearance.BackColor = Color.Yellow;
        }

      }
      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
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

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow dr = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) > 0)
        {
          errorMessage = "Data";
          return false;
        }
      }
      return true;

    }

    private DataTable ContainerMovingInformation()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("SaleOrderPid", typeof(System.Int64));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("LoadingFromPid", typeof(System.Int64));
      taParent.Columns.Add("LoadingToPid", typeof(System.Int64));
      taParent.Columns.Add("MovingQty", typeof(System.String));
      taParent.Columns.Add("MovingReason", typeof(System.Int32));
      taParent.Columns.Add("OldConfirmShipdate", typeof(System.DateTime));
      taParent.Columns.Add("NewConfirmShipdate", typeof(System.DateTime));
      taParent.Columns.Add("ReasonChangeShipDate", typeof(System.Int32));
      taParent.Columns.Add("Remarks", typeof(System.String));
      taParent.Columns.Add("ContainerRemark", typeof(System.String));
      return taParent;
    }
    private DataTable CreateTableExport()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("SaleOrder", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.String));
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.String));
      taParent.Columns.Add("MoveQty", typeof(System.String));
      taParent.Columns.Add("LoadingListFrom", typeof(System.String));
      taParent.Columns.Add("MoveToLoadingList", typeof(System.String));
      taParent.Columns.Add("MovingReason", typeof(System.String));
      taParent.Columns.Add("OldConfirmShipdate", typeof(System.String));
      taParent.Columns.Add("NewConfirmShipdate", typeof(System.String));
      taParent.Columns.Add("ReasonChangeShipDate", typeof(System.String));
      taParent.Columns.Add("Remarks", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      return taParent;
    }



    private bool Savedetail()
    {


      // Update New

      DataTable dt = this.ContainerMovingInformation();
      foreach (DataRow row in dtSource.Rows)
      {
        DataRow dr = dt.NewRow();
        dr["SaleOrderPid"] = DBConvert.ParseLong(row["SOPid"].ToString());
        dr["ItemCode"] = row["ItemCode"].ToString();
        dr["Revision"] = DBConvert.ParseInt(row["Revision"].ToString());
        dr["LoadingFromPid"] = DBConvert.ParseLong(row["ContainerPid"].ToString());
        dr["LoadingToPid"] = DBConvert.ParseLong(row["MoveContainerPid"].ToString());
        dr["MovingQty"] = DBConvert.ParseInt(row["MoveQty"].ToString());
        dr["MovingReason"] = DBConvert.ParseInt(row["ReasonMoveContainer"].ToString());
        if (row["OldShipDate"].ToString().Length > 0)
        {
          dr["OldConfirmShipdate"] = DBConvert.ParseDateTime(row["OldShipDate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        }
        if (row["NewShipDate"].ToString().Length > 0)
        {
          dr["NewConfirmShipdate"] = DBConvert.ParseDateTime(row["NewShipDate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        }
        if (row["OldShipDate"].ToString().Length > 0 || row["NewShipDate"].ToString().Length > 0)
        {
          dr["ReasonChangeShipDate"] = DBConvert.ParseInt(row["ReasonChangeShipDate"].ToString());
        }
        dr["Remarks"] = row["Remarks"].ToString();
        dr["ContainerRemark"] = row["ContainerRemark"].ToString();
        dt.Rows.Add(dr);
      }

      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      inputParam[1] = new SqlDBParameter("@CurrentPid", SqlDbType.BigInt, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[1];
      outputParam[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
      SqlDataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanHistory_Insert", 900, inputParam, outputParam);
      if (outputParam[0].Value == null || DBConvert.ParseLong(outputParam[0].Value.ToString()) < 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Save History");
        return false;
      }
      return true;

    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = this.Savedetail();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.CloseTab();
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

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public viewPLN_98_024()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_98_024_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      e.Layout.Bands[0].Columns["TextRevision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TextReasonMove"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TextReasonChangeShipdate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["LoadingListFrom"].Header.Caption = "Loading List From";
      e.Layout.Bands[0].Columns["MoveToLoadingList"].Header.Caption = "Move To Loading List";
      e.Layout.Bands[0].Columns["TextRevision"].Header.Caption = "Revision";

      e.Layout.Bands[0].Columns["TextOldShipDate"].Header.Caption = "Old ShipDate";
      e.Layout.Bands[0].Columns["TextNewShipDate"].Header.Caption = "New ShipDate";
      e.Layout.Bands[0].Columns["TextReasonMove"].Header.Caption = "ReasonMove";

      e.Layout.Bands[0].Columns["TextReasonChangeShipdate"].Header.Caption = "Reason Change Shipdate";

      e.Layout.Bands[0].Columns["ContainerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["MoveContainerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["OldShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["NewShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["ReasonMoveContainer"].Hidden = true;
      e.Layout.Bands[0].Columns["ReasonChangeShipDate"].Hidden = true;

      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["MoveContainerPid"].Hidden = true;
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
    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (ultraGridInformation.Rows.Count > 0)
      {


        Utility.ExportToExcelWithDefaultPath(ultraGridInformation, "Master Plan For Container", 7);

      }
    }
    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }
    #endregion event


  }
}
