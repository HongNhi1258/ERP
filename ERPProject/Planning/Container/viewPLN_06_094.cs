/*
  Author  : 
  Date    : 01/02/2011
  Description : Process ContainerList Between Warehouse & Planning
*/

using DaiCo.Application;
using DaiCo.Application.Web.Mail;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_06_094 : MainUserControl
  {
    #region Field
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;

    public long containerPid = long.MinValue;
    #endregion Field

    #region Init
    public viewPLN_06_094()
    {
      InitializeComponent();
    }

    /// <summary>
    /// User Control Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_094_Load(object sender, EventArgs e)
    {
      this.LoadControl();
      this.Search();
    }
    #endregion Init

    #region LoadData
    private void LoadControl()
    {
      string commandText = string.Empty;
      commandText += " SELECT COUNT(*)";
      commandText += " FROM TblPLNSHPContainer";
      commandText += " WHERE Pid =" + this.containerPid;
      commandText += "  AND [Confirm] = 3";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 1)
        {
          this.btnFinishSpecial.Enabled = false;
          this.btnFinish.Enabled = false;
        }
        else
        {
          this.btnFinishSpecial.Enabled = true;
          this.btnFinish.Enabled = true;
        }
      }
    }

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

      DBParameter[] input = new DBParameter[1];

      input[0] = new DBParameter("@ContainerPid", DbType.Int64, this.containerPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNProcessContainer", input);
      DataSet ds = this.DataSetSearch();

      ds.Tables[0].Merge(dsSource.Tables[0]);
      ds.Tables[1].Merge(dsSource.Tables[1]);
      ultDetail.DataSource = ds;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          row.CellAppearance.BackColor = Color.LightBlue;
        }

        for (int j = 0; j < ultDetail.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (DBConvert.ParseInt(ultDetail.Rows[i].
                    ChildBands[0].Rows[j].Cells["Errors"].Value.ToString()) != 0)
          {
            ultDetail.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Yellow;
          }
        }
      }

      // WO - SO
      ultWOSO.DataSource = dsSource.Tables[2];
      if (btnFinish.Enabled == true)
      {
        this.CheckShippedWO();
      }
      else
      {
        ultWOSO.DisplayLayout.Bands[0].Columns["WOSOBalance"].Hidden = true;
        ultWOSO.DisplayLayout.Bands[0].Columns["ShippedWO"].CellActivation = Activation.ActivateOnly;
      }
    }

    private void CheckShippedWO()
    {
      for (int i = 0; i < ultWOSO.Rows.Count; i++)
      {
        UltraGridRow row = ultWOSO.Rows[i];
        int shippedWO = 0;
        for (int j = 0; j < ultWOSO.Rows.Count; j++)
        {
          UltraGridRow row1 = ultWOSO.Rows[j];
          if (DBConvert.ParseLong(row.Cells["SOPid"].Value.ToString()) == DBConvert.ParseLong(row1.Cells["SOPid"].Value.ToString()) &&
              row.Cells["ItemCode"].Value.ToString() == row1.Cells["ItemCode"].Value.ToString() &&
              DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(row1.Cells["Revision"].Value.ToString()))
          {
            shippedWO = shippedWO + DBConvert.ParseInt(row1.Cells["ShippedWO"].Value.ToString());
          }
        }
        if (shippedWO != DBConvert.ParseInt(row.Cells["QtySOOnContainer"].Value.ToString()))
        {
          row.Cells["Errors"].Value = 1;
          row.Appearance.BackColor = Color.Yellow;
        }
        else
        {
          row.Cells["Errors"].Value = 0;
          row.Appearance.BackColor = Color.White;
        }
      }
    }

    private DataSet DataSetSearch()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("ItemGroup", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("Issued", typeof(System.Int32));
      taParent.Columns.Add("Errors", typeof(System.Int32));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("ContainerNo", typeof(System.String));
      taChild.Columns.Add("SaleNo", typeof(System.String));
      taChild.Columns.Add("ItemGroup", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["ItemGroup"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] },
              new DataColumn[] { taChild.Columns["ItemGroup"], taChild.Columns["ItemCode"], taChild.Columns["Revision"] }));
      return ds;
    }
    #endregion LoadData

    #region Process
    /// <summary>
    /// Check Search Data (Customer,ShipDate,Container Name)
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidFinishInfo(out string message)
    {
      message = string.Empty;

      DBParameter[] input = new DBParameter[1];

      input[0] = new DBParameter("@ContainerPid", DbType.Int64, containerPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNProcessContainer", input);
      if (dsSource != null && dsSource.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in dsSource.Tables[0].Rows)
        {
          if (DBConvert.ParseInt(row["Errors"].ToString()) != 0)
          {
            message = Shared.Utility.FunctionUtility.GetMessage("ERRO118");
            return false;
          }
        }

        foreach (DataRow row in dsSource.Tables[1].Rows)
        {
          if (DBConvert.ParseInt(row["Errors"].ToString()) != 0)
          {
            message = Shared.Utility.FunctionUtility.GetMessage("ERR0147");
            return false;
          }
        }

        //for (int i = 0; i < ultWOSO.Rows.Count; i++ )
        //{
        //  if (DBConvert.ParseInt(ultWOSO.Rows[i].Cells["Ignore"].Value.ToString()) == 0 && ultWOSO.Rows[i].Cells["WoWHShipping"].Value.ToString().ToLower().IndexOf(ultWOSO.Rows[i].Cells["WO"].Value.ToString().ToLower()) == -1)
        //  {
        //    ultWOSO.Rows[i].Cells["WoWHShipping"].Appearance.BackColor = Color.Yellow;
        //    message = "Current WO mapping to container is differrent WO Issuing Note";
        //    return false;
        //  }
        //}
      }
      else
      {
        message = Shared.Utility.FunctionUtility.GetMessage("ERRO118");
        return false;
      }

      return true;
    }

    /// <summary>
    /// Check Data WO - SO
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckWOSO(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultWOSO.Rows.Count; i++)
      {
        UltraGridRow row = ultWOSO.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          message = "ShippedWO";
          return false;
        }

        if (DBConvert.ParseInt(row.Cells["Ignore"].Value.ToString()) == 0 && row.Cells["WoWHShipping"].Value.ToString().ToLower().IndexOf(row.Cells["WO"].Value.ToString().ToLower()) == -1)
        {
          row.Cells["WoWHShipping"].Appearance.BackColor = Color.Yellow;
          message = "Current WO mapping to container is differrent WO Issuing Note";
          return false;
        }

      }
      return true;
    }

    /// <summary>
    ///  Check Qty SO -  WOSO 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckQtyWOSO(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultDetail.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowSO = rowParent.ChildBands[0].Rows[j];
          int totalQtyWOSO = 0;
          for (int k = 0; k < ultWOSO.Rows.Count; k++)
          {
            UltraGridRow rowWOSO = ultWOSO.Rows[k];
            if (rowSO.Cells["SaleNo"].Value.ToString() == rowWOSO.Cells["SaleNo"].Value.ToString() &&
              rowSO.Cells["ItemCode"].Value.ToString() == rowWOSO.Cells["ItemCode"].Value.ToString() &&
              DBConvert.ParseInt(rowSO.Cells["Revision"].Value.ToString()) == DBConvert.ParseInt(rowWOSO.Cells["Revision"].Value.ToString()))
            {
              totalQtyWOSO = totalQtyWOSO + DBConvert.ParseInt(rowWOSO.Cells["ShippedWO"].Value.ToString());
            }
          }
          if (DBConvert.ParseInt(rowSO.Cells["Qty"].Value.ToString()) != totalQtyWOSO)
          {
            message = rowSO.Cells["SaleNo"].Value.ToString() + " Not Enough Qty Open WO";
            return false;
          }
        }
      }
      return true;
    }

    private bool SaveWOSO()
    {
      for (int i = 0; i < ultWOSO.Rows.Count; i++)
      {
        UltraGridRow row = ultWOSO.Rows[i];
        if (DBConvert.ParseInt(row.Cells["ShippedWO"].Value.ToString()) > 0)
        {
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@ContainerPid", DbType.Int64, this.containerPid);
          input[1] = new DBParameter("@WOLinkSODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          input[2] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row.Cells["ShippedWO"].Value.ToString()));

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spPLNContainerWOSO_Insert", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Send Email To Customer Service When Finish Container
    /// </summary>
    private void SendEmailWhenFinishContainer()
    {
      string customerName = string.Empty;
      string loadingListName = string.Empty;

      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Temporary", startupPath);

      DBParameter[] inputParent = new DBParameter[1];
      inputParent[0] = new DBParameter("@ContainerPid", DbType.Int64, this.containerPid);
      DataSet dsLoadingList = DataBaseAccess.SearchStoreProcedure("spPLNContainerListWhenFinishedContainer_Select", inputParent);
      DataTable dtLoadingList = dsLoadingList.Tables[0];
      //IList arrattachments = new ArrayList();
      if (dtLoadingList != null && dtLoadingList.Rows.Count > 0)
      {
        for (int i = 0; i < dtLoadingList.Rows.Count; i++)
        {
          IList arrattachments = new ArrayList();
          string strTemplateName = "RPT_PLN_06_094";
          string strSheetName = "Sheet1";
          string strOutFileName = dtLoadingList.Rows[i]["LoadingList"].ToString();
          string strStartupPath = System.Windows.Forms.Application.StartupPath;
          string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
          string strPathTemplate = strStartupPath + @"\ExcelTemplate";
          XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

          oXlsReport.Cell("**LoadingList").Value = dtLoadingList.Rows[i]["LoadingList"];
          oXlsReport.Cell("**Customer").Value = dtLoadingList.Rows[i]["CustomerCode"];
          oXlsReport.Cell("**Loadingdate").Value = dtLoadingList.Rows[i]["DateOutStore"];

          customerName = dtLoadingList.Rows[i]["CustomerCode"].ToString();
          loadingListName = dtLoadingList.Rows[i]["LoadingList"].ToString();

          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@ContainerPid", DbType.Int64, this.containerPid);
          input[1] = new DBParameter("@LoadingListPid", DbType.Int64, DBConvert.ParseLong(dtLoadingList.Rows[i]["LoadingListPid"].ToString()));
          DataSet dsLoadingListDetail = DataBaseAccess.SearchStoreProcedure("spPLNContainerListWhenFinishedContainer_Select", input);
          DataTable dtLoadingListDetail = dsLoadingListDetail.Tables[1];
          double total = 0;
          double totalUnit = 0;
          int totalQuan = 0;
          if (dtLoadingListDetail != null && dtLoadingListDetail.Rows.Count > 0)
          {
            for (int j = 0; j < dtLoadingListDetail.Rows.Count; j++)
            {
              DataRow dtRow = dtLoadingListDetail.Rows[j];
              if (j > 0)
              {
                oXlsReport.Cell("A7:E7").Copy();
                oXlsReport.RowInsert(6 + j);
                oXlsReport.Cell("A7:E7", 0, j).Paste();
              }
              oXlsReport.Cell("**SaleCode", 0, j).Value = dtRow["SaleCode"].ToString();
              if (DBConvert.ParseInt(dtRow["Qty"].ToString()) >= 0)
              {
                oXlsReport.Cell("**Quantity", 0, j).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
                totalQuan += DBConvert.ParseInt(dtRow["Qty"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Quantity", 0, j).Value = DBNull.Value;
              }

              if (DBConvert.ParseDouble(dtRow["PriceUnit"].ToString()) >= 0)
              {
                oXlsReport.Cell("**UnitPrice", 0, j).Value = DBConvert.ParseDouble(dtRow["PriceUnit"].ToString());
                totalUnit += DBConvert.ParseDouble(dtRow["PriceUnit"].ToString());
              }
              else
              {
                oXlsReport.Cell("**UnitPrice", 0, j).Value = DBNull.Value;
              }
              if (DBConvert.ParseDouble(dtRow["TotalPrice"].ToString()) >= 0)
              {
                oXlsReport.Cell("**TotalPrice", 0, j).Value = DBConvert.ParseDouble(dtRow["TotalPrice"].ToString());
                total += DBConvert.ParseDouble(dtRow["TotalPrice"].ToString());
              }
              else
              {
                oXlsReport.Cell("**TotalPrice", 0, j).Value = DBNull.Value;
              }
              oXlsReport.Cell("**PO", 0, j).Value = dtRow["PONo"].ToString();
            }
            oXlsReport.Cell("**TotalQuan").Value = totalQuan;
            oXlsReport.Cell("**TotalUnit").Value = totalUnit;
            oXlsReport.Cell("**Total").Value = total;
            oXlsReport.Out.File(strOutFileName);

            //// Location
            //string location = string.Empty;
            //string commandText = string.Empty;
            //commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 6);
            //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            //if (dt != null)
            //{
            //  location = dt.Rows[0]["Value"].ToString();
            //}
            //string locationNew = string.Format(@"{0}\{1}", location, System.IO.Path.GetFileName(strOutFileName));
            //File.Copy(strOutFileName, locationNew);
            //// End

            // FTP
            string fileNameCopyFTP = string.Empty;
            string locationCopyFTP = string.Empty;
            string commandTextFTP = string.Format("SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 16078");
            DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable(commandTextFTP);
            locationCopyFTP = dtLocation.Rows[0][0].ToString();
            fileNameCopyFTP = loadingListName + ".xls";
            // FTP

            string locationNew = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(strOutFileName));
            File.Copy(strOutFileName, locationNew);
            arrattachments.Add(locationNew);
            // Copy FTP
            if (customerName == "JCUSA")
            {
              string locattionCopyFTP = string.Format(@"{0}\{1}", locationCopyFTP, fileNameCopyFTP);
              File.Move(strOutFileName, locattionCopyFTP);
            }
          }

          MailMessage mailMessage = new MailMessage();
          string subject = "Actual loading list container " + loadingListName;
          string body = string.Empty;
          body += "<p><i><font color='6699CC'>";
          if (customerName == "JCUSA")
          {
            body += "Dear All,<br><br>";
            body += "Please find attached the loading list for container " + loadingListName + ". <br>";
            body += "Please ensure it is entered into your system before you run JCDL report today <br><br>";
            body += "With thanks & regards ";
          }
          else if (customerName == "JCUK")
          {
            body += "Dear All,<br><br>";
            body += "Please find attached the loading list for container UK " + loadingListName + " which was loaded today.";
            body += "Please ensure it is entered into your system before you run Intransit report today. <br><br>";
            body += "With thanks & regards ";
          }
          body += "</font></i></p> ";

          string toEmail = string.Empty;

          DBParameter[] inputMail = new DBParameter[2];
          inputMail[0] = new DBParameter("@Customer", DbType.String, customerName);
          inputMail[1] = new DBParameter("@Type", DbType.Int32, 1);
          DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNGetMailWhenShipCon", inputMail);
          if (dt.Rows.Count != 0)
          {
            toEmail = dt.Rows[0]["Email"].ToString();
          }

          mailMessage.ServerName = "10.0.0.6";
          mailMessage.Username = "dc@vfr.net.vn";
          mailMessage.Password = "dc123456";
          mailMessage.From = "dc@vfr.net.vn";

          mailMessage.To = toEmail;
          mailMessage.Subject = subject;
          mailMessage.Body = body;
          IList attachments = new ArrayList();
          for (int k = 0; k < arrattachments.Count; k++)
          {
            attachments.Add(arrattachments[k]);
          }
          mailMessage.Attachfile = attachments;
          mailMessage.SendMail(true);

          // Delete File
          if (Directory.Exists(folder))
          {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
              try
              {
                File.Delete(file);
              }
              catch { }
            }
          }
        }
      }
    }

    /// <summary>
    /// Send Email To Customer Service When Finish Container No Price
    /// </summary>
    private void SendEmailWhenFinishContainerNoPrice()
    {
      string customerName = string.Empty;
      string loadingListName = string.Empty;

      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Temporary", startupPath);

      DBParameter[] inputParent = new DBParameter[1];
      inputParent[0] = new DBParameter("@ContainerPid", DbType.Int64, this.containerPid);
      DataSet dsLoadingList = DataBaseAccess.SearchStoreProcedure("spPLNContainerListWhenFinishedContainer_Select", inputParent);
      DataTable dtLoadingList = dsLoadingList.Tables[0];
      //IList arrattachments = new ArrayList();
      if (dtLoadingList != null && dtLoadingList.Rows.Count > 0)
      {
        for (int i = 0; i < dtLoadingList.Rows.Count; i++)
        {
          IList arrattachments = new ArrayList();
          string strTemplateName = "RPT_PLN_06_094_1";
          string strSheetName = "Sheet1";
          string strOutFileName = dtLoadingList.Rows[i]["LoadingList"].ToString();
          string strStartupPath = System.Windows.Forms.Application.StartupPath;
          string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
          string strPathTemplate = strStartupPath + @"\ExcelTemplate";
          XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

          oXlsReport.Cell("**LoadingList").Value = dtLoadingList.Rows[i]["LoadingList"];
          oXlsReport.Cell("**Customer").Value = dtLoadingList.Rows[i]["CustomerCode"];
          oXlsReport.Cell("**Loadingdate").Value = dtLoadingList.Rows[i]["DateOutStore"];

          customerName = dtLoadingList.Rows[i]["CustomerCode"].ToString();
          loadingListName = dtLoadingList.Rows[i]["LoadingList"].ToString();

          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@ContainerPid", DbType.Int64, this.containerPid);
          input[1] = new DBParameter("@LoadingListPid", DbType.Int64, DBConvert.ParseLong(dtLoadingList.Rows[i]["LoadingListPid"].ToString()));
          DataSet dsLoadingListDetail = DataBaseAccess.SearchStoreProcedure("spPLNContainerListWhenFinishedContainer_Select", input);
          DataTable dtLoadingListDetail = dsLoadingListDetail.Tables[1];
          double total = 0;
          if (dtLoadingListDetail != null && dtLoadingListDetail.Rows.Count > 0)
          {
            for (int j = 0; j < dtLoadingListDetail.Rows.Count; j++)
            {
              DataRow dtRow = dtLoadingListDetail.Rows[j];
              if (j > 0)
              {
                oXlsReport.Cell("A7:C7").Copy();
                oXlsReport.RowInsert(6 + j);
                oXlsReport.Cell("A7:C7", 0, j).Paste();
              }
              oXlsReport.Cell("**SaleCode", 0, j).Value = dtRow["SaleCode"].ToString();
              if (DBConvert.ParseInt(dtRow["Qty"].ToString()) >= 0)
              {
                oXlsReport.Cell("**Quantity", 0, j).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
                total += DBConvert.ParseDouble(dtRow["Qty"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Quantity", 0, j).Value = DBNull.Value;
              }
              oXlsReport.Cell("**PO", 0, j).Value = dtRow["PONo"].ToString();
            }
            oXlsReport.Cell("**Total").Value = total;
            oXlsReport.Out.File(strOutFileName);

            //// Location
            //string location = string.Empty;
            //string commandText = string.Empty;
            //commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 6);
            //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            //if (dt != null)
            //{
            //  location = dt.Rows[0]["Value"].ToString();
            //}
            //string locationNew = string.Format(@"{0}\{1}", location, System.IO.Path.GetFileName(strOutFileName));
            //File.Copy(strOutFileName, locationNew);
            //// End

            //// FTP
            //string fileNameCopyFTP = string.Empty;
            //string locationCopyFTP = string.Empty;
            //string commandTextFTP = string.Format("SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 16078");
            //DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable(commandTextFTP);
            //locationCopyFTP = dtLocation.Rows[0][0].ToString();
            //fileNameCopyFTP = loadingListName + ".xls";
            //// FTP

            string locationNew = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(strOutFileName));
            File.Copy(strOutFileName, locationNew);
            arrattachments.Add(locationNew);
            //// Copy FTP
            //if (customerName == "JCUSA")
            //{
            //  string locattionCopyFTP = string.Format(@"{0}\{1}", locationCopyFTP, fileNameCopyFTP);
            //  File.Move(strOutFileName, locattionCopyFTP);
            //}
          }

          MailMessage mailMessage = new MailMessage();
          string subject = "Actual loading list container " + loadingListName;
          string body = string.Empty;
          body += "<p><i><font color='6699CC'>";
          if (customerName == "JCUSA")
          {
            body += "Dear All,<br><br>";
            body += "Please find attached the loading list for container " + loadingListName + ". <br>";
            body += "Please ensure it is entered into your system before you run JCDL report today <br><br>";
            body += "With thanks & regards ";
          }
          else if (customerName == "JCUK")
          {
            body += "Dear All,<br><br>";
            body += "Please find attached the loading list for container UK " + loadingListName + " which was loaded today.";
            body += "Please ensure it is entered into your system before you run Intransit report today. <br><br>";
            body += "With thanks & regards ";
          }
          body += "</font></i></p> ";

          string toEmail = string.Empty;

          DBParameter[] inputMail = new DBParameter[2];
          inputMail[0] = new DBParameter("@Customer", DbType.String, customerName);
          inputMail[1] = new DBParameter("@Type", DbType.Int32, 2);
          DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNGetMailWhenShipCon", inputMail);
          if (dt.Rows.Count != 0)
          {
            toEmail = dt.Rows[0]["Email"].ToString();
          }

          mailMessage.ServerName = "10.0.0.6";
          mailMessage.Username = "dc@vfr.net.vn";
          mailMessage.Password = "dc123456";
          mailMessage.From = "dc@vfr.net.vn";

          mailMessage.To = toEmail;
          mailMessage.Subject = subject;
          mailMessage.Body = body;
          IList attachments = new ArrayList();
          for (int k = 0; k < arrattachments.Count; k++)
          {
            attachments.Add(arrattachments[k]);
          }
          mailMessage.Attachfile = attachments;
          mailMessage.SendMail(true);

          // Delete File
          if (Directory.Exists(folder))
          {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
              try
              {
                File.Delete(file);
              }
              catch { }
            }
          }
        }
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
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      Utility.SetPropertiesUltraGrid(ultDetail);

      e.Layout.Bands[0].Columns["Errors"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Errors"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.NoEdit;
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.NoEdit;
      }
    }

    private void ultWOSO_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Errors"].Hidden = true;

      // Set color
      e.Layout.Bands[0].Columns["WOSOBalance"].CellAppearance.BackColor = Color.LightGreen;
      e.Layout.Bands[0].Columns["QtySOOnContainer"].CellAppearance.BackColor = Color.Pink;
      e.Layout.Bands[0].Columns["ShippedWO"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["WoWHShipping"].CellAppearance.BackColor = Color.White;
      e.Layout.Bands[0].Columns["Ignore"].CellAppearance.BackColor = Color.White;

      e.Layout.Bands[0].Columns["Ignore"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Read only
      e.Layout.Bands[0].Columns["SaleNo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WO"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WOSOBalance"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtySOOnContainer"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WoWHShipping"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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

      // Check WO- SO
      success = this.CheckWOSO(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Check Qty SO - WOSO
      success = this.CheckQtyWOSO(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      success = this.SaveWOSO();
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Container", DbType.Int64, containerPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateShippedQtyContainer", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 1)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }

      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.btnFinish.Enabled = false;
      this.btnFinishSpecial.Enabled = false;

      ultWOSO.DisplayLayout.Bands[0].Columns["WOSOBalance"].Hidden = true;
      ultWOSO.DisplayLayout.Bands[0].Columns["ShippedWO"].CellActivation = Activation.ActivateOnly;
      // Send Email To CSD
      this.SendEmailWhenFinishContainer();
      this.SendEmailWhenFinishContainerNoPrice();
    }

    private void ultDetail_DoubleClick(object sender, EventArgs e)
    {
      viewPLN_06_015 view = new viewPLN_06_015();
      view.flagWindow = 1;
      Shared.Utility.WindowUtinity.ShowView(view, "CONTAINER LIST", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);

      this.Search();
    }

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultDetail.Rows.ExpandAll(true);
      }
      else
      {
        ultDetail.Rows.CollapseAll(true);
      }
    }

    private void btnFinishSpecial_Click(object sender, EventArgs e)
    {
      // Check WO- SO
      string message = string.Empty;
      bool success = this.CheckWOSO(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // Check Qty SO - WOSO
      success = this.CheckQtyWOSO(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      success = this.SaveWOSO();
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Container", DbType.Int64, containerPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateShippedQtyContainer", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 1)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }

      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.btnFinishSpecial.Enabled = false;
      this.btnFinish.Enabled = false;

      ultWOSO.DisplayLayout.Bands[0].Columns["WOSOBalance"].Hidden = true;
      ultWOSO.DisplayLayout.Bands[0].Columns["ShippedWO"].CellActivation = Activation.ActivateOnly;
      // Send Email To CSD
      this.SendEmailWhenFinishContainer();
      this.SendEmailWhenFinishContainerNoPrice();
    }

    private void ultWOSO_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow rowCur = e.Cell.Row;
      switch (columnName)
      {
        case "ShippedWO":
          this.CheckShippedWO();
          break;
        default:
          break;
      }
    }

    private void ultWOSO_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "ShippedWO":
          if (DBConvert.ParseInt(text) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "ShippedWO >= 0");
            e.Cancel = true;
          }
          else
          {
            int minValue = (DBConvert.ParseInt(e.Cell.Row.Cells["WOSOBalance"].Value.ToString()) > DBConvert.ParseInt(e.Cell.Row.Cells["QtySOOnContainer"].Value.ToString())) ?
                            DBConvert.ParseInt(e.Cell.Row.Cells["QtySOOnContainer"].Value.ToString()) : DBConvert.ParseInt(e.Cell.Row.Cells["WOSOBalance"].Value.ToString());

            if (DBConvert.ParseInt(text) > minValue)
            {
              WindowUtinity.ShowMessageError("ERR0001", "ShippedWO <= " + minValue + "");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultWOSO.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        Utility.ExportToExcelWithDefaultPath(ultWOSO, "Container WOSO");
      }
    }
    #endregion Event
  }
}
