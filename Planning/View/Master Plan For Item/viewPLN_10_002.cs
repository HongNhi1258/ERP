/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 11-10-2010
  Company : Dai Co 
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_002 : MainUserControl
  {
    #region Field
    public long ContainerListDetail = long.MinValue;
    public long containerListPid = long.MinValue;

    public long Wo = long.MinValue;
    public string Itemcode = "";
    public int Revision = int.MinValue;
    #endregion Field

    #region Init

    public viewPLN_10_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// User Control Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UC_PLNShipmentRequest_Load(object sender, EventArgs e)
    {
      ultCBWork.Value = 241;
      this.LoadData();
    }
    #endregion Init

    #region LoadData

    private void LoadWo()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 and Status = 0 ORDER BY Pid DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBWork.DataSource = dt;
      ultCBWork.DisplayMember = "Pid";
      ultCBWork.ValueMember = "Pid";
      ultCBWork.DisplayLayout.AutoFitColumns = true;
      ultCBWork.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadItem(long workOrderPid)
    {
      string commandText = string.Empty;
      if (workOrderPid != long.MinValue)
      {
        commandText = string.Format("SELECT DISTINCT WID.ItemCode, IB.OldCode FROM TblPLNWOInfoDetailGeneral WID LEFT JOIN TblBOMItemBasic IB ON IB.ItemCode = WID.ItemCode AND IB.RevisionActive = WID.Revision WHERE WoInfoPID = {0}", workOrderPid);
      }
      else
      {
        commandText = "SELECT ItemCode, OldCode FROM TblBOMItemBasic";
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBItem.DataSource = dt;
      ultCBItem.DisplayMember = "ItemCode";
      ultCBItem.ValueMember = "ItemCode";
      ultCBItem.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load Data For Grid
    /// </summary>
    private void LoadData()
    {
      this.LoadItem(long.MinValue);
      this.LoadWo();
      this.Search();
    }
    #endregion LoadData

    #region Event


    /// <summary>
    /// Event Button Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }



    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void Search()
    {
      if (ultCBWork.Value != null)
      {
        this.Wo = DBConvert.ParseLong(ultCBWork.Value.ToString());
      }
      if (ultCBItem.Value != null)
      {
        this.Itemcode = ultCBItem.Value.ToString();
      }
      if ((txtRevision.Text.Length > 0 && DBConvert.ParseInt(txtRevision.Text.Trim()) != int.MinValue))
      {
        this.Revision = DBConvert.ParseInt(txtRevision.Text.Trim());
      }
      DataSet ds = new DataSet();
      if (this.ContainerListDetail > long.MinValue)
      {
        DBParameter[] input = new DBParameter[4];
        input[0] = new DBParameter("@ContainerListDetail", DbType.Int64, this.ContainerListDetail);
        if (ultCBWork.Value != null)
        {
          input[1] = new DBParameter("@Wo", DbType.Int64, this.Wo);
        }
        if (ultCBItem.Value != null)
        {
          input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.Itemcode);
        }
        if ((txtRevision.Text.Length > 0 && DBConvert.ParseInt(txtRevision.Text.Trim()) != int.MinValue))
        {
          input[3] = new DBParameter("@Revision", DbType.Int32, this.Revision);
        }

        ds = DataBaseAccess.SearchStoreProcedure("spPLNWorkInProcessStatusForPopup", input);
        ds.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { ds.Tables[0].Columns["ItemCode"],
                                                                                 ds.Tables[0].Columns["Revision"]},
                                                  new DataColumn[] { ds.Tables[1].Columns["ItemCode"],
                                                                                ds.Tables[1].Columns["Revision"]}, false));
        ultAfter.DataSource = ds;
      }
      else if (this.containerListPid > long.MinValue)
      {
        DBParameter[] input = new DBParameter[4];
        input[0] = new DBParameter("@ContainerList", DbType.Int64, this.containerListPid);
        if (ultCBWork.Value != null)
        {
          input[1] = new DBParameter("@Wo", DbType.Int64, this.Wo);
        }
        if (ultCBItem.Value != null)
        {
          input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.Itemcode);
        }
        if ((txtRevision.Text.Length > 0 && DBConvert.ParseInt(txtRevision.Text.Trim()) != int.MinValue))
        {
          input[3] = new DBParameter("@Revision", DbType.Int32, this.Revision);
        }
        ds = DataBaseAccess.SearchStoreProcedure("spPLNWorkInProcessStatusForPopup", input);
        ds.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { ds.Tables[0].Columns["SOPid"],ds.Tables[0].Columns["ItemCode"],
                                                                                 ds.Tables[0].Columns["Revision"]},
                                                  new DataColumn[] { ds.Tables[1].Columns["SOPid"],ds.Tables[1].Columns["ItemCode"],
                                                                                ds.Tables[1].Columns["Revision"]}, false));
        ultAfter.DataSource = ds;
        ultAfter.Rows.ExpandAll(true);

      }
      else
      {
        DBParameter[] input = new DBParameter[4];
        if (ultCBWork.Value != null)
        {
          input[1] = new DBParameter("@Wo", DbType.Int64, this.Wo);
        }
        if (ultCBItem.Value != null)
        {
          input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.Itemcode);
        }
        if ((txtRevision.Text.Length > 0 && DBConvert.ParseInt(txtRevision.Text.Trim()) != int.MinValue))
        {
          input[3] = new DBParameter("@Revision", DbType.Int32, this.Revision);
        }
        ds = DataBaseAccess.SearchStoreProcedure("spPLNWorkInProcessStatusForPopup", input);
        ds.Relations.Add(new DataRelation("TblParent_TblChild1", new DataColumn[] {
                                                                                 ds.Tables[0].Columns["Wo"],ds.Tables[0].Columns["ItemCode"],
                                                                                 ds.Tables[0].Columns["Revision"]},
                                                  new DataColumn[] { ds.Tables[1].Columns["Wo"],ds.Tables[1].Columns["ItemCode"],
                                                                                ds.Tables[1].Columns["Revision"]}, false));
        ultAfter.DataSource = ds;
        ultAfter.Rows.ExpandAll(true);
      }
    }

    private void ultAfter_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultAfter);
      try
      {
        e.Layout.Bands[0].Columns["ContainerListPid1"].Hidden = true;
        e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.LightSkyBlue;
        e.Layout.Bands[1].Override.RowAppearance.ForeColor = Color.Black;
      }
      catch
      {
      }
      try
      {
        e.Layout.Bands[0].Columns["ContainerListPid"].Hidden = true;
        e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
        e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
        e.Layout.Bands[1].Columns["SOPid"].Hidden = true;
        e.Layout.Bands[0].Columns["WO"].Hidden = true;
        e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.LightSkyBlue;
        e.Layout.Bands[1].Override.RowAppearance.ForeColor = Color.Black;
      }
      catch
      { }
      try
      {
        e.Layout.Bands[1].Columns["Wo"].Hidden = true;
        e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
        e.Layout.Bands[1].Columns["Revision"].Hidden = true;
        e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.LightSkyBlue;
        e.Layout.Bands[1].Override.RowAppearance.ForeColor = Color.Black;
      }
      catch
      { }
    }

    private void ultCBWork_ValueChanged(object sender, EventArgs e)
    {
      long wo = long.MinValue;
      this.ultCBItem.Text = string.Empty;
      if (ultCBWork.Value != null)
      {
        wo = DBConvert.ParseLong(ultCBWork.Value.ToString());
        this.LoadItem(wo);
      }
      else
      {
        this.LoadItem(wo);
      }
    }
    // Add Finish 08/07/2011 Minh-d END
    #endregion Event
  }
}
