using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_06_009 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewPLN_06_009()
    {
      InitializeComponent();
    }

    private void viewPLN_98_010_Load(object sender, EventArgs e)
    {
      // Load Container
      this.LoadContainer();
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Load Container
    /// </summary>
    private void LoadContainer()
    {
      string commandText = string.Empty;

      commandText += " SELECT Pid, ContainerNo ";
      commandText += " FROM TblPLNSHPContainer  ";
      commandText += " WHERE Confirm = 3 ";
      commandText += " ORDER BY ShipDate DESC ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultContainer.DataSource = dtSource;
      ultContainer.ValueMember = "Pid";
      ultContainer.DisplayMember = "ContainerNo";
      ultContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultContainer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultContainer.DisplayLayout.Bands[0].Columns["ContainerNo"].Width = 200;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;

      // Container
      if (this.ultContainer.Value == null
          || DBConvert.ParseLong(this.ultContainer.Value.ToString()) == long.MinValue)
      {
        message = "Container";
        return false;
      }

      // Current SO
      if (this.ultCurSO.Value == null
          || DBConvert.ParseLong(this.ultCurSO.Value.ToString()) == long.MinValue)
      {
        message = "Current SO";
        return false;
      }

      // ItemCode
      if (this.ultItemCode.Value == null)
      {
        message = "Item Code";
        return false;
      }

      // New SO
      if (this.ultNewSO.Value == null
          || DBConvert.ParseLong(this.ultNewSO.Value.ToString()) == long.MinValue)
      {
        message = "New SO";
        return false;
      }

      return true;
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@Container", DbType.Int64, DBConvert.ParseLong(this.ultContainer.Value.ToString()));
      inputParam[1] = new DBParameter("@CurSO", DbType.Int64, DBConvert.ParseLong(this.ultCurSO.Value.ToString()));
      inputParam[2] = new DBParameter("@ItemCode", DbType.String, this.ultItemCode.Value.ToString().Substring(0, 9));
      inputParam[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(this.ultItemCode.Value.ToString().Substring(10, 1)));
      inputParam[4] = new DBParameter("@NewSO", DbType.Int64, DBConvert.ParseLong(this.ultNewSO.Value.ToString()));

      DataBaseAccess.ExecuteStoreProcedure("spPLNSwapSOOnContainerAfterDeductContainer_Edit", inputParam);

      return true;
    }
    #endregion Function

    #region Event

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Valid
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      this.CloseTab();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultContainer_ValueChanged(object sender, EventArgs e)
    {
      if (this.ultContainer.Value != null
        && DBConvert.ParseLong(this.ultContainer.Value.ToString()) != long.MinValue)
      {
        string commandText = string.Empty;

        commandText += " SELECT DISTINCT CLD.SOPid, SO.SaleNo + '/' + SO.CustomerPONo Name ";
        commandText += " FROM TblPLNSHPContainer CON  ";
        commandText += " 	INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
        commandText += " 	INNER JOIN TblPLNContainerListDetail CLD ON COD.LoadingListPid = CLD.ContainerListPid ";
        commandText += " 	INNER JOIN TblPLNSaleOrder SO ON CLD.SOPid = SO.Pid";
        commandText += " WHERE CON.Pid = " + DBConvert.ParseLong(this.ultContainer.Value.ToString());

        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

        ultCurSO.DataSource = dtSource;
        ultCurSO.ValueMember = "SOPid";
        ultCurSO.DisplayMember = "Name";
        ultCurSO.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCurSO.DisplayLayout.Bands[0].Columns["SOPid"].Hidden = true;
        ultCurSO.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      }
      else
      {
        ultCurSO.DataSource = null;
      }
    }

    private void ultCurSO_ValueChanged(object sender, EventArgs e)
    {
      if (this.ultCurSO.Value != null
        && DBConvert.ParseLong(this.ultCurSO.Value.ToString()) != long.MinValue
        && this.ultContainer.Value != null
        && DBConvert.ParseLong(this.ultContainer.Value.ToString()) != long.MinValue)
      {
        string commandText = string.Empty;

        commandText += " SELECT CLD.ItemCode, CLD.Revision, CLD.ItemCode + '-' + CAST(CLD.Revision AS VARCHAR) Name ";
        commandText += " FROM TblPLNSHPContainer CON  ";
        commandText += " 	INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
        commandText += " 	INNER JOIN TblPLNContainerListDetail CLD ON COD.LoadingListPid = CLD.ContainerListPid ";
        commandText += " WHERE CON.Pid = " + DBConvert.ParseLong(this.ultContainer.Value.ToString());
        commandText += " 	AND CLD.SOPid = " + DBConvert.ParseLong(this.ultCurSO.Value.ToString());

        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

        ultItemCode.DataSource = dtSource;
        ultItemCode.ValueMember = "Name";
        ultItemCode.DisplayMember = "Name";
        ultItemCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Hidden = true;
        ultItemCode.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
        ultItemCode.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      }
      else
      {
        ultItemCode.DataSource = null;
      }
    }

    private void ultItemCode_ValueChanged(object sender, EventArgs e)
    {
      if (this.ultItemCode.Value != null)
      {
        string commandText = string.Empty;

        commandText += " SELECT DISTINCT SaleOrderPid SOPid, SaleNo + '/' + PONo Name ";
        commandText += " FROM VPLNMasterPlan  ";
        commandText += " WHERE Balance > 0 ";
        commandText += "  AND ItemCode = '" + this.ultItemCode.Value.ToString().Substring(0, 9) + "'";

        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

        ultNewSO.DataSource = dtSource;
        ultNewSO.ValueMember = "SOPid";
        ultNewSO.DisplayMember = "Name";
        ultNewSO.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultNewSO.DisplayLayout.Bands[0].Columns["SOPid"].Hidden = true;
        ultNewSO.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      }
      else
      {
        ultNewSO.DataSource = null;
      }
    }
    #endregion Event
  }
}
