/*
  Author      : 
  Date        : 13/11/2010
  Description : Report Planning Container Management
 */
using DaiCo.Application;
using DaiCo.Planning.DataSetFile;
using DaiCo.Planning.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_06_012 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_06_012()
    {
      InitializeComponent();
    }

    private void viewPLN_06_012_Load(object sender, EventArgs e)
    {
      // Load List Reports
      this.LoadReports();

      // Load Search Condition Item.Customer Allocation
      this.LoadSearchItemCustomerAllocation();
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Load List Report For Allocate Management
    /// </summary>
    private void LoadReports()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Name", typeof(System.String));
      dt.Columns.Add("Value", typeof(System.Int32));

      DataRow row = dt.NewRow();
      row["Name"] = "";
      row["Value"] = 0;

      dt.Rows.Add(row);

      row = dt.NewRow();
      row["Name"] = "ITEMS/CUSTOMER ALLOCATION REPORT";
      row["Value"] = 1;
      dt.Rows.Add(row);

      row = dt.NewRow();
      row["Name"] = "CONTAINER/ITEM REPORT";
      row["Value"] = 2;
      dt.Rows.Add(row);

      this.ultraCBReports.DataSource = dt;
      ultraCBReports.ValueMember = "Value";
      ultraCBReports.DisplayMember = "Name";
      ultraCBReports.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultraCBReports.DisplayLayout.Bands[0].Columns["Name"].Width = 500;
      ultraCBReports.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Check Valid Form Before Print
    /// </summary>
    /// 
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidInfo(out string message)
    {
      message = string.Empty;
      // Reports
      if (this.ultraCBReports.Value == null || DBConvert.ParseInt(this.ultraCBReports.Value.ToString()) < 1)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Reports");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Load Data For Category Information Report
    /// </summary>
    private void LoadItemsAndCustomerInformationReport()
    {
      DBParameter[] input = new DBParameter[3];
      if (ultraCBItemCode.Value != null && ultraCBItemCode.Value.ToString().Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.String, ultraCBItemCode.Value.ToString());
      }

      if (ultraCBCustomer.Value != null && ultraCBCustomer.Value.ToString().Length > 0)
      {
        input[1] = new DBParameter("@CustomerPid", DbType.Int64, DBConvert.ParseLong(ultraCBCustomer.Value.ToString()));
      }

      if (ultraCBKind.Value != null && ultraCBKind.Value.ToString().Length > 0
                  && DBConvert.ParseInt(ultraCBKind.Value.ToString()) > 0)
      {
        input[2] = new DBParameter("@Kind", DbType.Int32, DBConvert.ParseInt(ultraCBKind.Value.ToString()));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNItemAllocationReport_Select", input);

      if (dtSource != null)
      {
        dtSource.Columns.Add("Picture", typeof(System.Byte[]));
        foreach (DataRow row in dtSource.Rows)
        {
          try
          {
            string imgPath = FunctionUtility.RDDGetItemImage(row["ItemCode"].ToString().Trim());
            FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] imgbyte = new byte[fs.Length + 1];
            imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
            row["Picture"] = imgbyte;
            br.Close();
            fs.Close();
          }
          catch { }
        }

        dsPLNItemsCustomerAllocation dsSource = new dsPLNItemsCustomerAllocation();
        dsSource.Tables[0].Merge(dtSource);

        DaiCo.Shared.View_Report report = null;
        cptPLNItemsCustomerAllocation cpt = new cptPLNItemsCustomerAllocation();
        cpt.SetDataSource(dsSource);

        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(Shared.Utility.ViewState.ModalWindow);
      }
    }

    /// <summary>
    /// Load Data Item Customer Allocation
    /// </summary>
    private void LoadSearchItemCustomerAllocation()
    {
      // Load Data For ItemCode
      this.LoadComboItemCode();

      // Load Customer
      this.LoadCustomer();

      // Load Kind (Allocate Or Non Allocate)
      this.LoadKind();
    }

    /// <summary>
    /// Load Kind (Allocate Or Non Allocate)
    /// </summary>
    private void LoadKind()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Name", typeof(System.String));
      dt.Columns.Add("Value", typeof(System.Int32));

      DataRow row = dt.NewRow();
      row["Name"] = "";
      row["Value"] = 0;

      dt.Rows.Add(row);

      row = dt.NewRow();
      row["Name"] = "Allocated";
      row["Value"] = 1;
      dt.Rows.Add(row);

      row = dt.NewRow();
      row["Name"] = "Non-Allocate";
      row["Value"] = 2;
      dt.Rows.Add(row);

      this.ultraCBKind.DataSource = dt;
      ultraCBKind.ValueMember = "Value";
      ultraCBKind.DisplayMember = "Name";
      ultraCBKind.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultraCBKind.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Data For Customer
    /// </summary>
    private void LoadCustomer()
    {
      // Load data for Customer
      string commandText = "SELECT Pid, CustomerCode + '-' + Name Name FROM TblCSDCustomerInfo";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBCustomer.DataSource = dt;
      ultraCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBCustomer.DisplayMember = "Name";
      ultraCBCustomer.ValueMember = "Pid";
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Name"].Width = 500;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load Data For ItemCode
    /// </summary>
    private void LoadComboItemCode()
    {
      // Load data for ItemCode
      string commandText = "SELECT ItemCode FROM TblBOMItemInfo Order By ItemCode";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBItemCode.DataSource = dt;
      ultraCBItemCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBItemCode.DisplayMember = "ItemCode";
      ultraCBItemCode.ValueMember = "ItemCode";
      ultraCBItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 500;
    }

    private void LoadComboContainer()
    {
      StringBuilder varname1 = new StringBuilder();
      varname1.Append("SELECT Pid, \n");
      varname1.Append("       Containerno, \n");
      varname1.Append("       CM.[Description] \n");
      varname1.Append("FROM   TblPLNSHPContainer CON \n");
      varname1.Append("       INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 11001 \n");
      varname1.Append("                                         AND CM.code = CON.[Type] ORDER BY Containerno");

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(varname1.ToString());
      ultCBContainer.DataSource = dt;
      ultCBContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBContainer.DisplayMember = "Containerno";
      ultCBContainer.ValueMember = "Pid";
      ultCBContainer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBContainer.DisplayLayout.Bands[0].Columns["Description"].Width = 250;
    }
    #endregion LoadData

    #region Event
    /// <summary>
    /// Close Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Print Report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (DBConvert.ParseInt(this.ultraCBReports.Value.ToString()) == 1)
      {
        this.LoadItemsAndCustomerInformationReport();
      }
      if (DBConvert.ParseInt(this.ultraCBReports.Value.ToString()) == 2)
      {
        if (ultCBContainer.Value != null)
        {
          string storeName = "spPLNContainerItemNeedPack";

          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@ContainerPid", DbType.Int64, DBConvert.ParseLong(ultCBContainer.Value.ToString()));

          DataSet dsSearch = DataBaseAccess.SearchStoreProcedure(storeName, 600, input);

          int totalQty = 0;
          double totalCBM = 0;
          int totalNeedPack = 0;
          if (dsSearch != null)
          {
            dsSearch.Tables[0].Columns.Add("Picture", typeof(System.Byte[]));
            foreach (DataRow row in dsSearch.Tables[0].Rows)
            {
              try
              {
                totalQty += DBConvert.ParseInt(row["Qty"].ToString());
                totalNeedPack += DBConvert.ParseInt(row["NeedPack"].ToString());
                totalCBM += DBConvert.ParseDouble(row["TotalCBM"].ToString());

                string imgPath = FunctionUtility.BOMGetItemImage(row["ItemCode"].ToString().Trim(), DBConvert.ParseInt(row["Revision"].ToString()));
                FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] imgbyte = new byte[fs.Length + 1];
                imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
                row["Picture"] = imgbyte;
                br.Close();
                fs.Close();
              }
              catch { }
            }

            dsPLNNeedPackOfContainer dsShare = new dsPLNNeedPackOfContainer();
            dsShare.Tables["ItemList"].Merge(dsSearch.Tables[0]);
            dsShare.Tables["Status"].Merge(dsSearch.Tables[1]);

            DaiCo.Shared.View_Report report = null;
            cptPLNNeedPackOfContainer cpt = new cptPLNNeedPackOfContainer();
            cpt.SetDataSource(dsShare);
            cpt.SetParameterValue("ContainerNo", ultCBContainer.Text);
            cpt.SetParameterValue("Printer", SharedObject.UserInfo.EmpName);

            if (chkPrintWithSAP.Checked)
            {
              ControlUtility.ViewCrystalReport(cpt);
            }
            else
            {
              report = new DaiCo.Shared.View_Report(cpt);
              report.ShowReport(Shared.Utility.ViewState.Window, FormWindowState.Maximized);
            }
          }
        }
      }
    }

    /// <summary>
    /// Show Group Search Condition Reports
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraCBReports_ValueChanged(object sender, EventArgs e)
    {
      if (DBConvert.ParseInt(this.ultraCBReports.Value.ToString()) == 1)
      {
        this.grpItemsCustomerAllocation.Visible = true;
      }
      else if (DBConvert.ParseInt(this.ultraCBReports.Value.ToString()) == 2)
      {
        this.grpItemsCustomerAllocation.Visible = false;
        this.LoadComboContainer();
      }
      else
      {
        this.grpItemsCustomerAllocation.Visible = false;
        // Load Search Condition Item.Customer Allocation
        this.LoadSearchItemCustomerAllocation();
      }
    }
    #endregion Event
  }
}
