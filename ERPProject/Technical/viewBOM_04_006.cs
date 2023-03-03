/* Creator: Vo Van Duy Qui
   Date: 11/01/2013
   Description: WO Carcass component Info
*/

using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Share.DataSetSource;
using DaiCo.ERPProject.Share.ReportTemplate;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
//using DaiCo.ERPProject.DataSetSource;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_04_006 : MainUserControl
  {
    #region Field
    public long wo = long.MinValue;
    public string carcass = string.Empty;
    private double totalTime = 0;
    private int flagTEC = 0;
    public int carcassQty = int.MinValue;
    #endregion Field

    #region Init

    public viewBOM_04_006()
    {
      InitializeComponent();
    }

    private void viewBOM_04_006_Load(object sender, EventArgs e)
    {
      if (this.btnTEC.Visible)
      {
        this.flagTEC = 1;
      }
      this.btnTEC.Visible = false;

      this.LoadCBWo();
      this.LoadCBCompKind();
      this.LoadCBMake();
      this.LoadCBStatus();
      this.LoadDataPrintPermit();
      if (this.wo != long.MinValue)
      {
        ultCBWo.Value = this.wo;
      }
      if (this.carcass.Length > 0)
      {
        txtCarcass.Text = this.carcass;
      }
      if (this.wo != long.MinValue && this.carcass.Length > 0)
      {
        this.Search();
      }
      rdPrinting.Checked = true;
    }

    #endregion Init

    #region Function

    private void LoadCBWo()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 ORDER BY Pid DESC";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBWo.DataSource = dtSource;
      ultCBWo.ValueMember = "Pid";
      ultCBWo.DisplayMember = "Pid";
      ultCBWo.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadCBCompKind()
    {
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("Value", typeof(System.Int32));
      dtSource.Columns.Add("Text", typeof(System.String));

      DataRow newRow1 = dtSource.NewRow();
      newRow1["Value"] = 1;
      newRow1["Text"] = "Leaf";
      dtSource.Rows.Add(newRow1);

      DataRow newRow2 = dtSource.NewRow();
      newRow2["Value"] = 0;
      newRow2["Text"] = "Assembly";
      dtSource.Rows.Add(newRow2);

      ultCBCompKind.DataSource = dtSource;
      ultCBCompKind.ValueMember = "Value";
      ultCBCompKind.DisplayMember = "Text";
      ultCBCompKind.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCompKind.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }

    private void LoadCBMake()
    {
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("Value", typeof(System.Int32));
      dtSource.Columns.Add("Text", typeof(System.String));

      DataRow newRow1 = dtSource.NewRow();
      newRow1["Value"] = 1;
      newRow1["Text"] = "Contract Out";
      dtSource.Rows.Add(newRow1);

      DataRow newRow2 = dtSource.NewRow();
      newRow2["Value"] = 0;
      newRow2["Text"] = "Make Local";
      dtSource.Rows.Add(newRow2);
      Utility.LoadUltraCombo(ultraCBMake, dtSource, "Value", "Text", false, "Value");
    }

    private void LoadCBStatus()
    {
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("Value", typeof(System.Int32));
      dtSource.Columns.Add("Text", typeof(System.String));
      DataRow row1 = dtSource.NewRow();
      row1["Value"] = 0;
      row1["Text"] = "Not Print";
      dtSource.Rows.Add(row1);
      DataRow row2 = dtSource.NewRow();
      row2["Value"] = 1;
      row2["Text"] = "Printed";
      dtSource.Rows.Add(row2);
      Utility.LoadUltraCombo(ultraCBStatus, dtSource, "Value", "Text", "Value");
      ultraCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadDataPrintPermit()
    {
      DataTable dtPrintPermit = new DataTable();
      dtPrintPermit.Columns.Add(new DataColumn("Value", typeof(System.Int32)));
      dtPrintPermit.Columns.Add(new DataColumn("Text", typeof(System.String)));
      DataRow row0 = dtPrintPermit.NewRow();
      row0["Value"] = 0;
      row0["Text"] = "Available";
      dtPrintPermit.Rows.Add(row0);

      DataRow row1 = dtPrintPermit.NewRow();
      row1["Value"] = 1;
      row1["Text"] = "Invalid";
      dtPrintPermit.Rows.Add(row1);

      DataRow row2 = dtPrintPermit.NewRow();
      row2["Value"] = 2;
      row2["Text"] = "Requesting";
      dtPrintPermit.Rows.Add(row2);

      DataRow row3 = dtPrintPermit.NewRow();
      row3["Value"] = 3;
      row3["Text"] = "Approved";
      dtPrintPermit.Rows.Add(row3);

      DataRow row4 = dtPrintPermit.NewRow();
      row4["Value"] = 4;
      row4["Text"] = "Refused";
      dtPrintPermit.Rows.Add(row4);

      Utility.LoadUltraDropDown(ultraDDPrintPermit, dtPrintPermit, "Value", "Text", "Value");
    }

    private bool CheckValidSearch()
    {
      if (ultCBWo.SelectedRow == null && ultCBWo.Text.Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0146", "Work Order");
        return false;
      }
      return true;
    }

    private void Search()
    {
      if (this.CheckValidSearch())
      {
        chkSelectedAll.Checked = false;
        // Get input data
        long wo = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBWo));
        string carcass = txtCarcass.Text.Trim();
        string comp = txtCompCode.Text.Trim();
        int compKind = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultCBCompKind));
        string materialGroup = txtMaterialGroup.Text.Trim();

        DateTime startDateFrom = DateTime.MinValue;
        if (ultDTStartFrom.Value != null)
        {
          startDateFrom = (DateTime)ultDTStartFrom.Value;
        }
        DateTime startDateTo = DateTime.MinValue;
        if (ultDTStartTo.Value != null)
        {
          startDateTo = (DateTime)ultDTStartTo.Value;
        }
        DateTime requiredDateFrom = DateTime.MinValue;
        if (ultDTRequiredFrom.Value != null)
        {
          requiredDateFrom = (DateTime)ultDTRequiredFrom.Value;
        }
        DateTime requiredDateTo = DateTime.MinValue;
        if (ultDTRequiredTo.Value != null)
        {
          requiredDateTo = (DateTime)ultDTRequiredTo.Value;
        }

        DBParameter[] inputParam = new DBParameter[12];
        if (wo != long.MinValue)
        {
          inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
        }
        if (carcass.Length > 0)
        {
          inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, "%" + carcass + "%");
        }
        if (comp.Length > 0)
        {
          inputParam[2] = new DBParameter("@CompCode", DbType.String, 256, "%" + comp + "%");
        }
        if (startDateFrom != DateTime.MinValue)
        {
          inputParam[3] = new DBParameter("@StartDateFrom", DbType.DateTime, startDateFrom);
        }
        if (startDateTo != DateTime.MinValue)
        {
          inputParam[4] = new DBParameter("@StartDateTo", DbType.DateTime, startDateTo);
        }
        if (requiredDateFrom != DateTime.MinValue)
        {
          inputParam[5] = new DBParameter("@RequiredDateFrom", DbType.DateTime, requiredDateFrom);
        }
        if (requiredDateTo != DateTime.MinValue)
        {
          inputParam[6] = new DBParameter("@RequiredDateTo", DbType.DateTime, requiredDateTo);
        }
        if (compKind != int.MinValue)
        {
          inputParam[7] = new DBParameter("@CompKind", DbType.Int32, compKind);
        }
        if (materialGroup.Length > 0)
        {
          inputParam[8] = new DBParameter("@MaterialGroup", DbType.AnsiString, 16, materialGroup);
        }
        if (ultraCBStatus.Value != null)
        {
          inputParam[9] = new DBParameter("@PrintStatus", DbType.Int32, ultraCBStatus.Value);
        }
        if (ultraCBMake.Value != null)
        {
          inputParam[10] = new DBParameter("@Make", DbType.Int32, ultraCBMake.Value);
        }
        if (this.carcassQty != int.MinValue)
        {
          inputParam[11] = new DBParameter("@CarcassQty", DbType.Int32, this.carcassQty);
        }

        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWOCarcassComponentInfo_Select", 1800, inputParam);
        ultComponent.DataSource = dtSource;
        lbTotalLeadTime.Text = "Total lead time of selected comp: 0";
        if (dtSource != null && dtSource.Rows.Count > 0)
        {
          lbCount.Text = "Count: " + dtSource.Rows.Count;
        }
        else
        {
          lbCount.Text = "Count: 0";
        }
      }
    }

    /// <summary>
    /// Get records were seleted to print report
    /// </summary>
    /// <returns></returns>
    private dsBOMWORoutingTicket GetSelectedRoutingData()
    {
      dsBOMWORoutingTicket dsResult = new dsBOMWORoutingTicket();
      DataTable dtSource = (DataTable)ultComponent.DataSource;
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (row["Selected"].ToString() == "1")
          {
            string carcassCode = row["CarcassCode"].ToString().Trim();
            int carcassQty = DBConvert.ParseInt(row["CarcassQty"].ToString());
            string compCode = row["CompGroup"].ToString().Trim();
            int qty = DBConvert.ParseInt(row["Qty"].ToString());
            long wo = DBConvert.ParseLong(row["Wo"].ToString());

            DBParameter[] param = new DBParameter[7];
            param[0] = new DBParameter("@Wo", DbType.Int64, wo);
            param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            param[2] = new DBParameter("@Qty", DbType.Int32, carcassQty);
            param[3] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, compCode);
            param[5] = new DBParameter("@BarcodeStatus", DbType.Int32, ConstantClass.BARCODE_CARCASS_COMP_NEW);
            if (this.flagTEC == 1)
            {
              param[6] = new DBParameter("@Flag", DbType.Int32, 1);
            }
            else
            {
              param[6] = new DBParameter("@Flag", DbType.Int32, 0);
            }

            DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMGetDataRoutingTicketWO", 300, param);

            if (dsData != null)
            {
              string fileLogoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, Shared.Utility.ConstantClass.PATH_LOGO);
              byte[] bImageLogo = FunctionUtility.ImagePathToByteArray(fileLogoPath);

              foreach (DataRow rowParent in dsData.Tables[0].Rows)
              {
                DataRow rowResultParent = dsResult.Tables["dtRoutingTicketHeader"].NewRow();
                rowResultParent["Pid"] = rowParent["Pid"];
                rowResultParent["Wo"] = rowParent["Wo"];
                rowResultParent["CarcassCode"] = rowParent["CarcassCode"];
                rowResultParent["CarcassName"] = rowParent["CarcassName"];
                rowResultParent["OldCode"] = rowParent["OldCode"];
                rowResultParent["SaleCode"] = rowParent["SaleCode"];
                rowResultParent["ComponentCode"] = rowParent["ComponentCode"];
                rowResultParent["ComponentName"] = rowParent["ComponentName"];
                rowResultParent["CompPerCarcassQty"] = rowParent["CompPerCarcassQty"];
                rowResultParent["CarcassQty"] = rowParent["CarcassQty"];
                rowResultParent["CompPerWOQty"] = rowParent["CompPerWOQty"];
                rowResultParent["Size"] = rowParent["Size"];
                rowResultParent["PrepareBy"] = rowParent["PrepareBy"];
                rowResultParent["PrepareDate"] = rowParent["PrepareDate"];
                rowResultParent["UpdateBy"] = rowParent["UpdateBy"];
                rowResultParent["UpdateDate"] = rowParent["UpdateDate"];
                rowResultParent["DateStart"] = rowParent["DateStart"];
                rowResultParent["DateRequired"] = rowParent["DateRequired"];
                rowResultParent["Specify"] = rowParent["Specify"];
                rowResultParent["Status"] = rowParent["Status"];
                rowResultParent["Lamination"] = rowParent["Lamination"];
                rowResultParent["FingerJoin"] = rowParent["FingerJoin"];
                rowResultParent["ConStructionRelationship"] = rowParent["ConStructionRelationship"];
                rowResultParent["ItemReferenceInWO"] = rowParent["ItemReferenceInWO"].ToString().Replace(",", System.Environment.NewLine);
                rowResultParent["ProfileList"] = rowParent["ProfileList"];
                rowResultParent["ToolList"] = rowParent["ToolList"];
                rowResultParent["BarCode"] = rowParent["BarCode"];
                rowResultParent["Store"] = rowParent["Store"];
                rowResultParent["CriticalComponent"] = rowParent["CriticalComponent"];
                rowResultParent["Carving"] = rowParent["Carving"];
                rowResultParent["Flag"] = rowParent["Flag"];
                rowResultParent["QCHistory"] = rowParent["QCHistory"];
                rowResultParent["CustomerName"] = rowParent["CustomerName"];
                rowResultParent["Remark"] = rowParent["Remark"];
                rowResultParent["EnvironmentStatus"] = rowParent["EnvironmentStatus"];

                // Component Reference
                DBParameter[] inputParam = new DBParameter[3];
                inputParam[0] = new DBParameter("@WO", DbType.Int64, rowParent["Wo"]);
                inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, rowParent["CarcassCode"]);
                inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, rowParent["ComponentCode"]);
                DBParameter[] outputParam = new DBParameter[] { new DBParameter("@MainCompList", DbType.AnsiString, 256, string.Empty) };
                DataBaseAccess.ExecuteStoreProcedure("spPLNGetAllLevelMainCompByComponentPid", inputParam, outputParam);
                rowResultParent["ComponentReference"] = outputParam[0].Value;

                //Image logo                  
                rowResultParent["LogoImage"] = bImageLogo;

                //Image carcass
                string fileCarcassPath = FunctionUtility.BOMGetCarcassImage(rowParent["CarcassCode"].ToString());
                //rowResultParent["ImageHeader"] = FunctionUtility.ImagePathToByteArray(fileCarcassPath);
                rowResultParent["ImageHeader"] = FunctionUtility.ImageToByteArrayWithFormat(fileCarcassPath, 380, 0.94, "JPG");

                //Image component
                string fileCompPath = FunctionUtility.BOMGetCarcassComponentImage(rowParent["CarcassCode"].ToString(), rowParent["PictureCode"].ToString());
                rowResultParent["ImageFull"] = FunctionUtility.ImagePathToByteArray(fileCompPath);

                //User
                rowResultParent["User"] = Shared.Utility.SharedObject.UserInfo.EmpName;

                dsResult.Tables["dtRoutingTicketHeader"].Rows.Add(rowResultParent);
              }
              foreach (DataRow rowChild in dsData.Tables[1].Rows)
              {
                DataRow rowResultChild = dsResult.Tables["dtWorkingStep"].NewRow();
                rowResultChild["Wo"] = rowChild["Wo"];
                rowResultChild["CarcassCode"] = rowChild["CarcassCode"];
                rowResultChild["ComponentCode"] = rowChild["ComponentCode"];
                rowResultChild["Step"] = rowChild["Step"];
                rowResultChild["ProcessCode"] = rowChild["ProcessCode"];
                rowResultChild["VNDescription"] = rowChild["VNDescription"];
                rowResultChild["ENDescription"] = rowChild["ENDescription"];
                rowResultChild["MachineGroup"] = rowChild["MachineGroup"];
                rowResultChild["Profile"] = rowChild["Profile"];
                rowResultChild["SetupTime"] = rowChild["SetupTime"];
                rowResultChild["ProcessTime"] = rowChild["ProcessTime"];
                rowResultChild["LeadTime"] = rowChild["LeadTime"];
                rowResultChild["Specification"] = rowChild["Specification"];
                dsResult.Tables["dtWorkingStep"].Rows.Add(rowResultChild);
              }
              foreach (DataRow rowChild in dsData.Tables[2].Rows)
              {
                DataRow rowResultChild = dsResult.Tables["dtMaterials"].NewRow();
                rowResultChild["CompPid"] = rowChild["CompPid"];
                rowResultChild["MaterialCode"] = rowChild["MaterialCode"];
                rowResultChild["MaterialName"] = rowChild["MaterialName"];
                rowResultChild["Qty"] = rowChild["Qty"];
                rowResultChild["Alternative"] = rowChild["Alternative"];
                dsResult.Tables["dtMaterials"].Rows.Add(rowResultChild);
              }
            }
          }
        }
      }
      return dsResult;
    }

    /// <summary>
    /// Get records were seleted to print routing veneer report
    /// </summary>
    /// <returns></returns>
    private dsBOMWORoutingTicketVeneer GetSelectedRoutingVeneerData()
    {
      dsBOMWORoutingTicketVeneer dsResult = new dsBOMWORoutingTicketVeneer();
      DataTable dtSource = (DataTable)ultComponent.DataSource;

      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (row["Selected"].ToString() == "1")
          {
            string carcassCode = row["CarcassCode"].ToString().Trim();
            int carcassQty = DBConvert.ParseInt(row["CarcassQty"].ToString());
            string compCode = row["CompGroup"].ToString().Trim();
            int qty = DBConvert.ParseInt(row["Qty"].ToString());
            long wo = DBConvert.ParseLong(row["Wo"].ToString());

            DBParameter[] param = new DBParameter[6];
            param[0] = new DBParameter("@Wo", DbType.Int64, wo);
            param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            param[2] = new DBParameter("@Qty", DbType.Int32, carcassQty);
            param[3] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, compCode);
            param[5] = new DBParameter("@BarcodeStatus", DbType.Int32, ConstantClass.BARCODE_CARCASS_COMP_NEW);
            DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMGetDataRoutingTicketVeneerWO", 3000, param);

            if (dsData != null)
            {
              string fileLogoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, Shared.Utility.ConstantClass.PATH_LOGO);
              byte[] bImageLogo = FunctionUtility.ImagePathToByteArray(fileLogoPath);

              foreach (DataRow rowParent in dsData.Tables[0].Rows)
              {
                DataRow rowResultParent = dsResult.Tables["dtRoutingTicketHeader"].NewRow();
                rowResultParent["Pid"] = rowParent["Pid"];
                rowResultParent["Wo"] = rowParent["Wo"];
                rowResultParent["CarcassCode"] = rowParent["CarcassCode"];
                rowResultParent["CarcassName"] = rowParent["CarcassName"];
                rowResultParent["ComponentCode"] = rowParent["ComponentCode"];
                rowResultParent["ComponentName"] = rowParent["ComponentName"];
                rowResultParent["CompPerCarcassQty"] = rowParent["CompPerCarcassQty"];
                rowResultParent["CarcassQty"] = rowParent["CarcassQty"];
                rowResultParent["CompPerWOQty"] = rowParent["CompPerWOQty"];
                rowResultParent["Size"] = rowParent["Size"];
                rowResultParent["PrepareBy"] = rowParent["PrepareBy"];
                rowResultParent["PrepareDate"] = rowParent["PrepareDate"];
                rowResultParent["UpdateBy"] = rowParent["UpdateBy"];
                rowResultParent["UpdateDate"] = rowParent["UpdateDate"];
                rowResultParent["DateStart"] = rowParent["DateStart"];
                rowResultParent["ConStructionRelationship"] = rowParent["ConStructionRelationship"];
                rowResultParent["ItemReferenceInWO"] = rowParent["ItemReferenceInWO"];
                rowResultParent["BarCode"] = rowParent["BarCode"];

                // Component Reference
                DBParameter[] inputParam = new DBParameter[3];
                inputParam[0] = new DBParameter("@WO", DbType.Int64, rowParent["Wo"]);
                inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, rowParent["CarcassCode"]);
                inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, rowParent["ComponentCode"]);
                DBParameter[] outputParam = new DBParameter[] { new DBParameter("@MainCompList", DbType.AnsiString, 256, string.Empty) };
                DataBaseAccess.ExecuteStoreProcedure("spPLNGetAllLevelMainCompByComponentPid", inputParam, outputParam);
                rowResultParent["ComponentReference"] = outputParam[0].Value;

                //Image logo                  
                rowResultParent["LogoImage"] = bImageLogo;

                //Image carcass
                string fileCarcassPath = FunctionUtility.BOMGetCarcassImage(rowParent["CarcassCode"].ToString());
                rowResultParent["ImageHeader"] = FunctionUtility.ImageToByteArrayWithFormat(fileCarcassPath, 380, 0.865, "JPG");

                //Image component
                string fileCompPath = FunctionUtility.BOMGetCarcassComponentImage(rowParent["CarcassCode"].ToString(), rowParent["ComponentCode"].ToString());
                rowResultParent["ImageFull"] = FunctionUtility.ImagePathToByteArray(fileCompPath);

                //User
                rowResultParent["User"] = Shared.Utility.SharedObject.UserInfo.EmpName;

                dsResult.Tables["dtRoutingTicketHeader"].Rows.Add(rowResultParent);
              }
              foreach (DataRow rowChild in dsData.Tables[1].Rows)
              {
                DataRow rowResultChild = dsResult.Tables["dtMaterials"].NewRow();
                rowResultChild["CompPid"] = rowChild["CompPid"];
                rowResultChild["MaterialCode"] = rowChild["MaterialCode"];
                rowResultChild["MaterialName"] = rowChild["MaterialName"];
                rowResultChild["Qty"] = rowChild["Qty"];
                rowResultChild["Alternative"] = rowChild["Alternative"];
                dsResult.Tables["dtMaterials"].Rows.Add(rowResultChild);
              }
            }
          }
        }
      }
      return dsResult;
    }

    #endregion Function

    #region Event

    private void ultComponent_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["FlagPrint"].Hidden = true;
      e.Layout.Bands[0].Columns["Group"].Hidden = true;
      e.Layout.Bands[0].Columns["CompPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsLeaf"].Hidden = true;
      e.Layout.Bands[0].Columns["Wo"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass\nCode";
      e.Layout.Bands[0].Columns["CarcassQty"].Header.Caption = "Carcass\nQty";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Comp Qty";
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["CarcassQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Length"].Header.Caption = "FIN\nLength";
      e.Layout.Bands[0].Columns["FIN_Length"].MinWidth = 55;
      e.Layout.Bands[0].Columns["FIN_Length"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["FIN_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Width"].MinWidth = 50;
      e.Layout.Bands[0].Columns["FIN_Width"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["FIN_Width"].Header.Caption = "FIN\nWidth";
      e.Layout.Bands[0].Columns["FIN_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Thickness"].MinWidth = 60;
      e.Layout.Bands[0].Columns["FIN_Thickness"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["FIN_Thickness"].Header.Caption = "FIN\nThickness";
      e.Layout.Bands[0].Columns["StartDate"].Header.Caption = "Start\nDate";
      e.Layout.Bands[0].Columns["StartDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["StartDate"].FormatInfo = new CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["StartDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["StartDate"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["StartDate"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["FinishDate"].Header.Caption = "Finished\nDate";
      e.Layout.Bands[0].Columns["FinishDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["FinishDate"].FormatInfo = new CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["FinishDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["FinishDate"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["FinishDate"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Printed"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Printed"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["CompGroup"].Header.Caption = "Comp Code";
      e.Layout.Bands[0].Columns["CompGroup"].MinWidth = 150;
      e.Layout.Bands[0].Columns["CompGroup"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Primary"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Printed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Specify"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["Specify"].MinWidth = 200;
      e.Layout.Bands[0].Columns["Primary"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Primary"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "Vietnam Description";
      e.Layout.Bands[0].Columns["PrintedDate"].Header.Caption = "Printed\nDate";
      e.Layout.Bands[0].Columns["PrintedDate"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["PrintedDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["PrintedDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["PrintedDate"].FormatInfo = new CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["LeadTime"].Header.Caption = "Lead Time";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 55;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract\nOut";
      e.Layout.Bands[0].Columns["ContractOut"].MinWidth = 55;
      e.Layout.Bands[0].Columns["ContractOut"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["ContractOut"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PrintPermit"].ValueList = ultraDDPrintPermit;
      e.Layout.Bands[0].Columns["PrintPermit"].Header.Caption = "Status";
      e.Layout.Bands[0].Columns["PrintPermit"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["PrintPermit"].MinWidth = 70;
      e.Layout.Bands[0].Columns["IsCompStore"].Header.Caption = "Store";
      e.Layout.Bands[0].Columns["IsCompStore"].MinWidth = 55;
      e.Layout.Bands[0].Columns["IsCompStore"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["IsCompStore"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["IsCompStore"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      for (int j = 0; j < ultComponent.Rows.Count; j++)
      {
        int flag = DBConvert.ParseInt(ultComponent.Rows[j].Cells["FlagPrint"].Value.ToString());
        int leaf = DBConvert.ParseInt(ultComponent.Rows[j].Cells["IsLeaf"].Value.ToString());
        if (flag == 1)
        {
          ultComponent.Rows[j].CellAppearance.BackColor = Color.LightGray;
        }
        if (leaf == 1)
        {
          ultComponent.Rows[j].Cells["CompGroup"].Appearance.BackColor = Color.LightGreen;
        }
      }
      e.Layout.Bands[0].Columns["Selected"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Printed"].CellActivation = Activation.AllowEdit;
      if (rdPrinting.Checked)
      {
        for (int i = 0; i < ultComponent.Rows.Count; i++)
        {
          int printPermit = DBConvert.ParseInt(ultComponent.Rows[i].Cells["PrintPermit"].Value.ToString());
          if ((printPermit == 0) || (printPermit == 3))
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.AllowEdit;
          }
          else
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.ActivateOnly;
          }
        }
      }
      else
      {
        for (int i = 0; i < ultComponent.Rows.Count; i++)
        {
          int printPermit = DBConvert.ParseInt(ultComponent.Rows[i].Cells["PrintPermit"].Value.ToString());
          if ((printPermit != 0) && (printPermit != 2) && (printPermit != 3))
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.AllowEdit;
          }
          else
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.ActivateOnly;
          }
        }
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (chkSelectedAll.Checked ? 1 : 0);
      //DataTable dtSource = (DataTable)ultComponent.DataSource;
      this.totalTime = 0;

      for (int i = 0; i < ultComponent.Rows.Count; i++)
      {
        UltraGridRow row = ultComponent.Rows[i];
        if (row.IsFilteredOut == false)
        {
          int flag = DBConvert.ParseInt(ultComponent.Rows[i].Cells["FlagPrint"].Value.ToString());
          int printPermit = DBConvert.ParseInt(ultComponent.Rows[i].Cells["PrintPermit"].Value.ToString());
          if ((rdPrinting.Checked) && ((printPermit == 0) || (printPermit == 3)))
          {
            ultComponent.Rows[i].Cells["Selected"].Value = selected;
            if (flag == 0)
            {
              ultComponent.Rows[i].Cells["Printed"].Value = selected;
            }
            if (selected == 1)
            {
              double leadTime = DBConvert.ParseDouble(ultComponent.Rows[i].Cells["LeadTime"].Value.ToString()) == double.MinValue ? 0 : DBConvert.ParseDouble(ultComponent.Rows[i].Cells["LeadTime"].Value.ToString());
              this.totalTime += leadTime;
            }
          }
          else if ((rdRequesting.Checked) && (printPermit != 0) && (printPermit != 2) && (printPermit != 3))
          {
            ultComponent.Rows[i].Cells["Selected"].Value = selected;
          }
        }
      }
      if (selected == 1)
      {
        lbTotalLeadTime.Text = "Total lead time of selected comp: " + this.totalTime.ToString();
      }
      else
      {
        lbTotalLeadTime.Text = "Total lead time of selected comp: 0";
      }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      tableLayoutButton.Enabled = false;
      DaiCo.Shared.View_Report report = null;
      ReportClass cpt = null;
      // Report Routing Ticket
      dsBOMWORoutingTicket dsRoutingTicket = this.GetSelectedRoutingData();
      if (dsRoutingTicket != null)
      {
        //if (this.flagTEC == 1)
        //{
        cpt = new cptBOMWORoutingTicketTec();
        cpt.SetDataSource(dsRoutingTicket);
        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = true;
        report.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
        //}
        //else
        //{
        //  cpt = new cptBOMWORoutingTicket();
        //  cpt.SetDataSource(dsRoutingTicket);
        //  report = new DaiCo.Shared.View_Report(cpt);
        //  report.IsShowGroupTree = true;
        //  report.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
        //}
      }
      else
      {
        WindowUtinity.ShowMessageWarning("WRN0024", "Carcass");
      }
      tableLayoutButton.Enabled = true;
    }

    private void btnRoutingVeneer_Click(object sender, EventArgs e)
    {
      tableLayoutButton.Enabled = false;
      // Report Routing Ticket Veneer      
      DaiCo.Shared.View_Report report = null;
      ReportClass cpt = null;

      dsBOMWORoutingTicketVeneer dsRoutingTicketVeneer = this.GetSelectedRoutingVeneerData();

      if (dsRoutingTicketVeneer != null && dsRoutingTicketVeneer.Tables.Count > 0 && dsRoutingTicketVeneer.Tables["dtRoutingTicketHeader"].Rows.Count > 0)
      {
        cpt = new cptBOMWORoutingTicketVeneer();
        cpt.SetDataSource(dsRoutingTicketVeneer);
        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = true;
        report.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
      }
      tableLayoutButton.Enabled = true;
    }

    private void Object_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    private void ultComponent_CellChange(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      switch (colName)
      {
        case "Selected":
          int flag = DBConvert.ParseInt(e.Cell.Row.Cells["FlagPrint"].Value.ToString());
          int selected = DBConvert.ParseInt(e.Cell.Row.Cells["Selected"].Text.ToString());
          double leadTime = DBConvert.ParseDouble(e.Cell.Row.Cells["LeadTime"].Value.ToString()) == double.MinValue ? 0 : DBConvert.ParseDouble(e.Cell.Row.Cells["LeadTime"].Value.ToString());
          if (flag == 0 || flag == int.MinValue)
          {
            e.Cell.Row.Cells["Printed"].Value = selected;
          }
          if (selected == 1)
          {
            totalTime += leadTime;
          }
          else
          {
            totalTime -= leadTime;
          }
          break;
      }
      lbTotalLeadTime.Text = "Total lead time of selected comp: " + totalTime.ToString();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultComponent.DataSource;
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          int select = (DBConvert.ParseInt(row["Printed"].ToString()) == int.MinValue) ? 0 : DBConvert.ParseInt(row["Printed"].ToString());
          int isPrinted = (DBConvert.ParseInt(row["FlagPrint"].ToString()) == int.MinValue) ? 0 : DBConvert.ParseInt(row["FlagPrint"].ToString());
          if (select != isPrinted)
          {
            long compPid = DBConvert.ParseLong(row["CompPid"].ToString());
            DateTime date = DBConvert.ParseDateTime(row["PrintedDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            DBParameter[] input = new DBParameter[6];
            input[0] = new DBParameter("@Pid", DbType.Int64, compPid);
            input[1] = new DBParameter("@IsPrinted", DbType.Int32, isPrinted);
            input[2] = new DBParameter("@Select", DbType.Int32, select);
            input[3] = new DBParameter("@PrintBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            if (date != DateTime.MinValue)
            {
              input[4] = new DBParameter("@PrintedDate", DbType.DateTime, date);
            }
            input[5] = new DBParameter("@IsImport", DbType.Int32, 0);
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spPLNWOCarcassComponentPrinted_Update", input, output);
            if (DBConvert.ParseLong(output[0].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0005");
              this.SaveSuccess = false;
              return;
            }
          }
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.SaveSuccess = true;
      }
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      // Init Excel File
      string strTemplateName = "RoutingPrintedDateTemplate";
      string strSheetName = "Sheet1";
      string strOutFileName = "Routing Printed Date";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\Technical";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Get data
      DataTable dtExport = (DataTable)ultComponent.DataSource;

      if (dtExport != null && dtExport.Rows.Count > 0)
      {
        oXlsReport.Cell("**MaxRows").Value = dtExport.Rows.Count;
        for (int i = 0; i < dtExport.Rows.Count; i++)
        {
          DataRow dtRow = dtExport.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:G7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:G7", 0, i).Paste();
          }
          int printed = DBConvert.ParseInt(dtRow["Printed"].ToString());
          oXlsReport.Cell("**Wo", 0, i).Value = dtRow["Wo"];
          oXlsReport.Cell("**CarcassCode", 0, i).Value = dtRow["CarcassCode"];
          oXlsReport.Cell("**CompCode", 0, i).Value = dtRow["CompGroup"];
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["DescriptionVN"];
          oXlsReport.Cell("**Printed", 0, i).Value = dtRow["Printed"];
          if (printed == 1)
          {
            oXlsReport.Cell("**PrintedDate", 0, i).Value = dtRow["PrintedDate"];
          }
          else
          {
            oXlsReport.Cell("**PrintedDate", 0, i).Value = DBNull.Value;
          }
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      viewBOM_04_007 view = new viewBOM_04_007();
      WindowUtinity.ShowView(view, "IMPORT PRINTED DATE OF ROUTING FROM EXCEL", true, ViewState.Window, FormWindowState.Maximized);
    }

    private void btnPrintReport_Click(object sender, EventArgs e)
    {
      string name = string.Empty;
      string materialGroup = txtMaterialGroup.Text.Trim();
      if (materialGroup.Length >= 4)
      {
        string groupM = materialGroup.Substring(0, 4);
        switch (groupM)
        {
          case "010-":
            name = "MDF";
            break;
          case "011-":
            name = "Veneer";
            break;
          case "012-":
            name = "Wood";
            break;
          default:
            break;
        }
      }
      DataTable dtPrint = (DataTable)ultComponent.DataSource;
      if (dtPrint != null && dtPrint.Rows.Count > 0)
      {
        string strTemplateName = "BOMReport";
        string strSheetName = "RoutingTicket";
        string strOutFileName = "Routing " + name + " Report";
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate\Technical";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

        oXlsReport.Cell("**Date").Value = DateTime.Today.ToShortDateString();
        oXlsReport.Cell("**Title").Value = "Report routing " + name;
        for (int i = 0; i < dtPrint.Rows.Count; i++)
        {
          DataRow dtRow = dtPrint.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:K8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:K8", 0, i).Paste();
          }

          oXlsReport.Cell("**Wo", 0, i).Value = dtRow["Wo"];
          oXlsReport.Cell("**CarcassCode", 0, i).Value = dtRow["CarcassCode"];
          oXlsReport.Cell("**CarcassQty", 0, i).Value = dtRow["CarcassQty"];
          oXlsReport.Cell("**CompCode", 0, i).Value = dtRow["CompGroup"];
          oXlsReport.Cell("**Uniform", 0, i).Value = dtRow["Uniform"];
          oXlsReport.Cell("**VNDesc", 0, i).Value = dtRow["DescriptionVN"];
          oXlsReport.Cell("**Group", 0, i).Value = dtRow["Group"];
          oXlsReport.Cell("**StartDate", 0, i).Value = dtRow["StartDate"];
          oXlsReport.Cell("**FinDate", 0, i).Value = dtRow["FinishDate"];
          oXlsReport.Cell("**PrintedDate", 0, i).Value = dtRow["PrintedDate"];
        }
        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
    }

    private void WokingType_CheckedChanged(object sender, EventArgs e)
    {
      btnPrint.Enabled = rdPrinting.Checked;
      btnRequest.Enabled = rdRequesting.Checked;
      chkSelectedAll.Checked = false;

      if (rdPrinting.Checked)
      {
        for (int i = 0; i < ultComponent.Rows.Count; i++)
        {
          int printPermit = DBConvert.ParseInt(ultComponent.Rows[i].Cells["PrintPermit"].Value.ToString());
          if ((printPermit == 0) || (printPermit == 3))
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.AllowEdit;
          }
          else
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.ActivateOnly;
          }
          ultComponent.Rows[i].Cells["Selected"].Value = 0;
          int flag = DBConvert.ParseInt(ultComponent.Rows[i].Cells["FlagPrint"].Value.ToString());
          if (flag == 0)
          {
            ultComponent.Rows[i].Cells["Printed"].Value = 0;
          }
        }
      }
      else
      {
        for (int i = 0; i < ultComponent.Rows.Count; i++)
        {
          int printPermit = DBConvert.ParseInt(ultComponent.Rows[i].Cells["PrintPermit"].Value.ToString());
          if ((printPermit != 0) && (printPermit != 2) && (printPermit != 3))
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.AllowEdit;
          }
          else
          {
            ultComponent.Rows[i].Cells["Selected"].Activation = Activation.ActivateOnly;
          }
          ultComponent.Rows[i].Cells["Selected"].Value = 0;
          int flag = DBConvert.ParseInt(ultComponent.Rows[i].Cells["FlagPrint"].Value.ToString());
          if (flag == 0)
          {
            ultComponent.Rows[i].Cells["Printed"].Value = 0;
          }
        }
      }
    }

    private void btnRequest_Click(object sender, EventArgs e)
    {
      string compPidList = string.Empty;
      bool success = true;
      for (int i = 0; i < ultComponent.Rows.Count; i++)
      {
        int select = DBConvert.ParseInt(ultComponent.Rows[i].Cells["Selected"].Value.ToString());
        if (select == 1)
        {
          long pid = DBConvert.ParseLong(ultComponent.Rows[i].Cells["CompPid"].Value.ToString());
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          inputParam[1] = new DBParameter("@PrintStatus", DbType.Int32, 2);
          inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNWOCarcassComponentInfo_UpdateStatus", inputParam, outputParam);
          if (outputParam != null && outputParam[0].Value.ToString() == "0")
          {
            success = false;
          }
          else
          {
            if (compPidList.Length > 0)
            {
              compPidList += '|';
            }
            compPidList += pid.ToString();
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0001");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.Search();

      //Send mail
      if (compPidList.Length > 0)
      {
        compPidList = string.Format("|{0}|", compPidList);
        DBParameter[] inputMail = new DBParameter[2];
        inputMail[0] = new DBParameter("@MailKind", DbType.Int32, 2);
        inputMail[1] = new DBParameter("@CompPidList", DbType.AnsiString, 1024, compPidList);
        DataBaseAccess.ExecuteStoreProcedure("spBOMMailForRoutingPrinting", inputMail);
      }
    }

    private void ultComponent_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
    {
      for (int i = 0; i < ultComponent.Rows.Count; i++)
      {
        UltraGridRow row = ultComponent.Rows[i];
        if (row.IsFilteredOut == true)
        {
          row.Cells["Selected"].Value = 0;
        }
      }
      lbCount.Text = string.Format("Count: {0}", ultComponent.Rows.FilteredInRowCount);
    }
    #endregion Event    
  }
}
