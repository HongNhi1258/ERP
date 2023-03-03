using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_010 : MainUserControl
  {
    #region Field
    public DataTable dtDetail = new DataTable();
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init

    public viewPLN_98_010()
    {
      InitializeComponent();
    }

    private void viewPLN_98_010_Load(object sender, EventArgs e)
    {
      // Load Customer
      this.LoadCustomer();

      // Load Distributor
      this.LoadDistributor();

      // Load Type Of Container
      this.LoadTypeOfContainer();
    }

    #endregion Init

    #region Function

    private void LoadTypeOfContainer()
    {
      string commandText = "SELECT Code, Value, [Description] FROM TblBOMCodeMaster WHERE [Group] = 11001";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBContainerType, dt, "Code", "Description", false, "Code");
    }

    /// <summary>
    /// Load Distributor
    /// </summary>
    private void LoadDistributor()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 99003 ORDER BY Sort";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultDistributor, dt, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadCustomer()
    {
      string commandText = string.Empty;

      commandText += " SELECT Pid CustomerPid, CustomerCode, CustomerCode + '-' + Name Name ";
      commandText += " FROM TblCSDCustomerInfo ";
      commandText += " ORDER BY Name ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCustomer, dtSource, "CustomerPid", "Name", false, new string[] { "CustomerPid", "CustomerCode" });
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;

      // Check Container
      string container = ultContainer.Text.Trim();
      if (container.Length == 0)
      {
        message = "Container";
        return false;
      }
      string commandText = string.Format(@"SELECT ContainerNo FROM TblPLNSHPContainer WHERE ContainerNo = '{0}'", container);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count >= 1)
      {
        message = "Container Duplication";
        return false;
      }

      // Check ShipDate
      if (ultShipDate.Value != null)
      {
        string shipDate = DBConvert.ParseString(DBConvert.ParseDateTime(ultShipDate.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
        string ccommand = "SELECT Holiday FROM VHRDWorkingDay WHERE Holiday = -1 AND CONVERT(varchar, WDate, 103) = '" + shipDate + "' ";
        DataTable dtdata = DataBaseAccess.SearchCommandTextDataTable(ccommand);
        if (dtdata.Rows.Count > 0)
        {
          message = "Shipdate is Holiday";
          return false;
        }
      }
      else
      {
        message = "Shipdate";
        return false;
      }

      // Check Customer
      if (ultCustomer.Value == null || ultCustomer.Value.ToString().Length == 0)
      {
        message = "Customer";
        return false;
      }

      // Check Distributor
      if (ultDistributor.Value == null || ultDistributor.Value.ToString().Length == 0)
      {
        message = "Distributor";
        return false;
      }

      // Check Type Of Container
      if (ultCBContainerType.Value == null || ultCBContainerType.Value.ToString().Length == 0)
      {
        message = "Container Type";
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
      DBParameter[] inputParam = new DBParameter[6];
      inputParam[0] = new DBParameter("@ContainerNo", DbType.String, ultContainer.Text.Trim());
      inputParam[1] = new DBParameter("@ShipDate", DbType.DateTime, DBConvert.ParseDateTime(ultShipDate.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
      inputParam[2] = new DBParameter("@Customer", DbType.Int64, DBConvert.ParseLong(ultCustomer.Value.ToString()));
      inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[4] = new DBParameter("@Distributor", DbType.Int32, DBConvert.ParseInt(ultDistributor.Value.ToString()));
      inputParam[5] = new DBParameter("@ContainerType", DbType.Int32, DBConvert.ParseInt(ultCBContainerType.Value.ToString()));

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int32, Int32.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNAddContainer_Insert", inputParam, outputParam);
      int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
      if (result == 0)
      {
        return false;
      }

      // Load Detail
      string commandText = string.Format(@"SELECT CON.ContainerNo, CONVERT(VARCHAR, CON.ShipDate, 103) LoadingDate, CL.Pid ContainerListPid,
		                                       SUM(ISNULL(CLD.Qty, 0)) LoadingQty, ROUND(SUM(ISNULL(CLD.Qty, 0) * ISNULL(IIF.CBM, 0)), 3) LoadingCBM, CL.CustomerPid
	                                         FROM TblPLNSHPContainer CON
		                                          INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid
		                                          INNER JOIN TblPLNContainerList CL ON COD.LoadingListPid = CL.Pid
		                                          LEFT JOIN TblPLNContainerListDetail CLD ON CL.Pid = CLD.ContainerListPid
		                                          LEFT JOIN TblBOMItemInfo IIF ON CLD.ItemCode = IIF.ItemCode
									                                          AND CLD.Revision = IIF.Revision
	                                          WHERE CON.Confirm <> 3 AND CON.ContainerNo = '{0}'
	                                          GROUP BY CON.ContainerNo, CON.ShipDate, CL.Pid, CL.CustomerPid
	                                          ORDER BY CON.ShipDate DESC", ultContainer.Text);
      dtDetail = DataBaseAccess.SearchCommandTextDataTable(commandText);

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

    private void ultCustomer_Leave(object sender, EventArgs e)
    {
      if (this.ultCustomer.Value != null && DBConvert.ParseLong(this.ultCustomer.Value.ToString()) != long.MinValue)
      {
        string commandText = string.Empty;

        commandText += " SELECT CON.ContainerNo, CONVERT(VARCHAR, CON.ShipDate, 6) ShipDateStr ";
        commandText += " FROM TblPLNSHPContainer CON ";
        commandText += " 	INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
        commandText += " 	INNER JOIN TblPLNContainerList CL ON COD.LoadingListPid = CL.Pid ";
        commandText += " WHERE CL.CustomerPid = " + DBConvert.ParseLong(this.ultCustomer.Value.ToString());
        commandText += " ORDER BY CON.ShipDate DESC ";

        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

        ultContainer.DataSource = dtSource;
        ultContainer.ValueMember = "ContainerNo";
        ultContainer.DisplayMember = "ContainerNo";
        ultContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultContainer.DisplayLayout.Bands[0].Columns["ShipDateStr"].Header.Caption = "Ship Date";
      }
    }
      #endregion Event
  }
}
