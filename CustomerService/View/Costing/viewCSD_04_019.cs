/*
  Author        : Ha Anh
  Create date   : 08/1/2014
  Decription    : Searching item cost price order to capture 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataSetSource.CustomerService;
using DaiCo.Shared.ReportTemplate.CustomerService;
using Infragistics.Win;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_019 : MainUserControl
  {
    #region Init
    /// <summary>
    /// Init view
    /// </summary>
    public viewCSD_04_019()
    {
      InitializeComponent();
    }

    /// <summary>
    /// View Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_019_Load(object sender, EventArgs e)
    {
      this.LoadData();
      
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load data for ultracombo Customer and Status
    /// </summary>
    private void LoadData()
    {
      // Load data for ultra combo Customer
      ControlUtility.LoadUltraCBCustomer(ultCBCustomer);

      // Load HP Exhibition
      this.LoadExhibition();

      //Load Capture
      this.LoadCapture();

      //Create data for ultra combo Status
      DataTable dt = new DataTable();
      dt.Columns.Add("Value", typeof(System.Int32));
      dt.Columns.Add("Text", typeof(System.String));

      DataRow row1 = dt.NewRow();
      row1["Value"] = 0;
      row1["Text"] = "Not Confirmed";
      dt.Rows.Add(row1);

      DataRow row2 = dt.NewRow();
      row2["Value"] = 1;
      row2["Text"] = "Confirmed";
      dt.Rows.Add(row2);
      ultCBStatus.DataSource = dt;
      ultCBStatus.ValueMember = "Value";
      ultCBStatus.DisplayMember = "Text";
      ultCBStatus.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;

      // Load data for item kind
      DataTable dtKind = new DataTable();
      dtKind.Columns.Add("Value", typeof(System.Int32));
      dtKind.Columns.Add("Text", typeof(System.String));
      DataRow rowKind1 = dtKind.NewRow();
      rowKind1["Value"] = 0;
      rowKind1["Text"] = "JC";
      dtKind.Rows.Add(rowKind1);
      DataRow rowKind2 = dtKind.NewRow();
      rowKind2["Value"] = 1;
      rowKind2["Text"] = "OEM";
      dtKind.Rows.Add(rowKind2);
      ControlUtility.LoadUltraCombo(ultraCBKind, dtKind, "Value", "Text", false, "Value");
    }
    /// <summary>
    /// Load Capture
    /// </summary>
    private void LoadCapture()
    {
      string cm = "";
      cm = @"  SELECT 1 Value, 'Yes' Label
                UNION
                SELECT 2 Value, 'No' Label";
              
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultCBCapture, dt, "Value", "Label", false, "Value");
    }
    
    /// <summary>
    /// Load Exhibition
    /// </summary>
    private void LoadExhibition()
    {
      string commandText = string.Format("Select Code, Value From TblBOMCodeMaster Where [Group] = 16");
      DataTable dtExhibition = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBExhibition.DataSource = dtExhibition;
      ultraCBExhibition.ValueMember = "Code";
      ultraCBExhibition.DisplayMember = "Value";
      ultraCBExhibition.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      //ultraCBExhibition.DisplayLayout.AutoFitColumns = true;
      ultraCBExhibition.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    /// <summary>
    /// Check valid input data
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      if (ultCBCustomer.Text.Trim().Length > 0 && ultCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        return false;
      }      
      if (ultCBStatus.Value == null && ultCBStatus.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Status");
        return false;
      }
      return true;
    }

    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("SaleCode", typeof(System.String));
      taParent.Columns.Add("OldCode", typeof(System.String));
      taParent.Columns.Add("Name", typeof(System.String));
      taParent.Columns.Add("NameVN", typeof(System.String));
      taParent.Columns.Add("Description", typeof(System.String));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("CustomerPid", typeof(System.Int64));
      taParent.Columns.Add("Cus.Name", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Confirm", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("CapturePid", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("No", typeof(System.Int64));
      taChild.Columns.Add("CaptureRemark", typeof(System.String));
      taChild.Columns.Add("CreateDate", typeof(System.DateTime));
      taChild.Columns.Add("BOMVersion", typeof(System.String));
      taChild.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["ItemCode"], taChild.Columns["ItemCode"], false));
      return ds;
    }

    /// <summary>
    /// Search information with conditionals 
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;
      if (this.CheckInvalid())
      {
        string itemCode = txtItemCode.Text.Trim();
        string saleCode = txtSaleCode.Text.Trim();        
        string carcassCode = txtCarCode.Text.Trim();
        long cusPid = long.MinValue;
        try 
        { 
          cusPid = DBConvert.ParseLong(ultCBCustomer.Value.ToString()); 
        }
        catch { }
        int status = int.MinValue;
        try
        {
          status = DBConvert.ParseInt(ultCBStatus.Value.ToString());
        }
        catch { }
        int capture = int.MinValue;
        try
        {
          capture = DBConvert.ParseInt(ultCBCapture.Value.ToString());
        }
        catch { }
        int exhibition = int.MinValue;
        try
        {
          exhibition = DBConvert.ParseInt(ultraCBExhibition.Value.ToString());
        }
        catch { }
        DBParameter[] inputParam = new DBParameter[8];
        if (itemCode.Length > 0)
        {
          inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode.Replace("'", "''") + "%");
        }
        if (saleCode.Length > 0)
        {
          inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode.Replace("'", "''") + "%");
        }
        if (carcassCode.Length > 0)
        {
          inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, "%" + carcassCode.Replace("'", "''") + "%");
        }
        if (cusPid != long.MinValue)
        {
          inputParam[3] = new DBParameter("@CustomerPid", DbType.Int64, cusPid);
        }
        if (ultraCBKind.Value != null)
        {
          inputParam[4] = new DBParameter("@ItemKind", DbType.Int32, ultraCBKind.Value);
        }
        if (status != int.MinValue)
        {
          inputParam[5] = new DBParameter("@Status", DbType.Int32, status);
        }
        if (ultCBCapture.Value != null)
        {
          inputParam[6] = new DBParameter("@Capture", DbType.Int32, capture);
        }
        if (ultraCBExhibition.Value != null)
        {
          inputParam[7] = new DBParameter("@Exhibition", DbType.Int32, exhibition);
        }
        DataSet dsSearch = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDItemCostingCapture_Select", inputParam);
        lbCount.Text = string.Format("Count: {0}", dsSearch.Tables[0].Rows.Count);

        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(dsSearch.Tables[0]);
        dsSource.Tables["dtChild"].Merge(dsSearch.Tables[1]);
        ultData.DataSource = dsSource;
 
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int confirm = DBConvert.ParseInt(ultData.Rows[i].Cells["Confirm"].Value.ToString());
          if (confirm == 1)
          {
            ultData.Rows[i].Appearance.BackColor = Color.LightGray;
          }
        }
      }
      btnSearch.Enabled = true;
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Initialize layout of ultra grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      //e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      //e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Confirm"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Vietnamese Name";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count - 2; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["CapturePid"].Hidden = true;

      e.Layout.Bands[1].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["No"].MaxWidth = 50;
      e.Layout.Bands[1].Columns["No"].MinWidth = 50;
      e.Layout.Bands[1].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
    }

    /// <summary>
    /// Show item cost price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected.Rows != null && ultData.Selected.Rows.Count > 0) 
      { 
        UltraGridRow row = ultData.Selected.Rows[0];
        string itemCode = row.Cells["ItemCode"].Value.ToString().Trim();

        long capturePid = long.MinValue;
        if (row.ParentRow != null)
        {
          capturePid = DBConvert.ParseLong(row.Cells["CapturePid"].Value);
        }        
        if (itemCode.Length > 0)
        {       
          viewCSD_04_020 uc = new viewCSD_04_020();
          uc.itemCode = itemCode;
          //uc.bomVersion = bomVersion;
          if (row.Cells.Exists("CapturePid"))
          {
              uc.capturePid = DBConvert.ParseLong(row.Cells["CapturePid"].Value);
          }

          WindowUtinity.ShowView(uc, "CAPTURE ITEM COSTING", false, ViewState.ModalWindow);
          //this.Search();
        }
      }
    }

    /// <summary>
    /// Search information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }
    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
        //bool success = true;
        ////  Insert/Update                 
        //for (int i = 0; i < ultData.Rows.Count; i++)
        //{
        //  int isUpdate = DBConvert.ParseInt(ultData.Rows[i].Cells["IsUpdate"].Value);
        //  DBParameter[] inputParam = new DBParameter[2];
        //  if (isUpdate == 1)
        //  {
        //    for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        //    {
        //      int bomVersion = DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["BOMVersion"].Value);
        //      long detailPid = DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["CapturePid"].Value.ToString());
        //      if (pid > 0) // Update
        //      {
        //        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
        //      }
        //      inputParam[1] = new DBParameter("@BOMVersion", DbType.AnsiString, 16, bomVersion);
        //      DataBaseAccess.ExecuteStoreProcedure("spCSDOptionSetForEcat_Edit", inputParam, outputParam);
        //      if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        //      {
        //        success = false;
        //      }
        //    }
        //  }
        //}
        //if (success)
        //{
        //  WindowUtinity.ShowMessageSuccess("MSG0004");
        //  this.Search();
        //}
        //else
        //{
        //  WindowUtinity.ShowMessageError("WRN0004");
        //}
    }
    /// <summary>
    /// Close view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    
    /// <summary>
    /// Event when key Enter down
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Object_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      //int viewType = -1;
      //if (rdMakeLocal.Checked)
      //{
      //  viewType = 0;
      //}
      //else if (rdContractOut.Checked)
      //{
      //  viewType = 1;
      //}
      int viewType = -1;
      if (rdMakeLocal.Checked && rdinhousehw.Checked)
      {
        viewType = 0;
      }
      else if (rdContractOut.Checked && rdsubhw.Checked)
      {
        viewType = 1;
      }
      else if (rdMakeLocal.Checked && rdsubhw.Checked)
      {
        viewType = 2;
      }
      else if (rdContractOut.Checked && rdinhousehw.Checked)
      {
        viewType = 3;
      }

      for (int i = 0; i < ultData.Rows.Count; i++ )
      {
        if(DBConvert.ParseInt(ultData.Rows[i].Cells["Select"].Value.ToString()) == 1)
        {
          string itemCode = ultData.Rows[i].Cells["ItemCode"].Value.ToString().Trim();
          if (itemCode.Length > 0)
          {
            FunctionUtility.ViewItemCosting(itemCode, 0, viewType);
          }
          break;
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j ++ )
        {
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Select"].Value.ToString()) == 1)
          {
            try
            {
              //DBParameter[] inputParamCosting = new DBParameter[2];
              //inputParamCosting[0] = new DBParameter("@CapturePid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["CapturePid"].Value.ToString()));

              //if (rdContractOut.Checked)
              //{
              //  inputParamCosting[1] = new DBParameter("@ViewType", DbType.Int32, 1);
              //}
              //else 
              //{
              //  inputParamCosting[1] = new DBParameter("@ViewType", DbType.Int32, 0);
              //}

              //DBParameter[] outputParamCosting = new DBParameter[5];
              //outputParamCosting[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
              //outputParamCosting[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
              //outputParamCosting[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
              //outputParamCosting[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, string.Empty);
              //outputParamCosting[4] = new DBParameter("@ContractOut", DbType.Int32, int.MinValue);

              //DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDItemCostingCapture_Report", inputParamCosting, outputParamCosting);
              //if (dsSource != null && dsSource.Tables.Count > 1)
              //{
              //  dsSource.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
              //  dsSource.Tables[0].Columns.Add("CheckImage", typeof(String));

              //  string pathImate = FunctionUtility.BOMGetItemImage(dsSource.Tables[0].Rows[0]["ItemCode"].ToString(), DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["RevisionActive"].ToString()));

              //  dsSource.Tables[0].Rows[0]["Image"] = Shared.Utility.FunctionUtility.ImageToByteArrayWithFormat(pathImate, 380, 1.74, "JPG");
              //  dsSource.Tables[0].Rows[0]["CheckImage"] = dsSource.Tables[0].Rows[0]["Image"].ToString();

              //  dsSource.Tables[1].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
              //  dsSource.Tables[1].Columns.Add("CheckImage", typeof(String));
              //  for (int k = 0; k < dsSource.Tables[1].Rows.Count; k++)
              //  {
              //    dsSource.Tables[1].Rows[k]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(dsSource.Tables[1].Rows[k]["Picture"].ToString());
              //    dsSource.Tables[1].Rows[k]["CheckImage"] = dsSource.Tables[1].Rows[k]["Image"].ToString();
              //  }

              //  double totalVNDPrice = DBConvert.ParseDouble(dsSource.Tables[2].Rows[0]["TotalBOMPrice_VND"].ToString());
              //  double totalUSDPrice = DBConvert.ParseDouble(dsSource.Tables[2].Rows[0]["TotalBOMPrice_USD"].ToString());

              //  dsCSDItemCostPrice ds = new dsCSDItemCostPrice();
              //  ds.Tables["dtItemCostMaster"].Merge(dsSource.Tables[0]);
              //  ds.Tables["dtItemCostDetail"].Merge(dsSource.Tables[1]);

              //  cptCSDItemCostPrice cptItemCostPrice = new cptCSDItemCostPrice();

              //  cptItemCostPrice.SetDataSource(ds);
              //  double dExchange = DBConvert.ParseDouble(outputParamCosting[0].Value.ToString());
              //  double dFOH = DBConvert.ParseDouble(outputParamCosting[1].Value.ToString());
              //  double dProfit = DBConvert.ParseDouble(outputParamCosting[2].Value.ToString());
              //  string remark = outputParamCosting[3].Value.ToString();
              //  cptItemCostPrice.SetParameterValue("ExchangeRate", dExchange);
              //  cptItemCostPrice.SetParameterValue("FOH", dFOH);
              //  cptItemCostPrice.SetParameterValue("Profit", dProfit);
              //  cptItemCostPrice.SetParameterValue("Remark", remark);
              //  cptItemCostPrice.SetParameterValue("TotalBOMPrice_VND", totalVNDPrice);
              //  cptItemCostPrice.SetParameterValue("TotalBOMPrice_USD", totalUSDPrice);
              //  int contractOut = DBConvert.ParseInt(outputParamCosting[4].Value);
              //  cptItemCostPrice.SetParameterValue("ViewType", (contractOut == 1 ? "Subcon" : "Inhouse"));
              //  cptItemCostPrice.SetParameterValue("User", SharedObject.UserInfo.EmpName);

              //  //Carcass Summary
              //  DataTable dtCarcassSummary = dsSource.Tables[2];
              //  if (dtCarcassSummary.Rows.Count > 0)
              //  {
              //    cptItemCostPrice.SetParameterValue("TotalLabor", DBConvert.ParseDouble(dtCarcassSummary.Rows[0]["TotalLabor"].ToString()));
              //    cptItemCostPrice.SetParameterValue("TotalMainMaterial", DBConvert.ParseDouble(dtCarcassSummary.Rows[0]["TotalMainMaterial"].ToString()));
              //    cptItemCostPrice.SetParameterValue("OtherMaterial", DBConvert.ParseDouble(dtCarcassSummary.Rows[0]["OtherMaterial"].ToString()));
              //    cptItemCostPrice.SetParameterValue("SubconNetPrice", DBConvert.ParseDouble(dtCarcassSummary.Rows[0]["SubconNetPrice"].ToString()));
              //    cptItemCostPrice.SetParameterValue("MaterialSupplied", DBConvert.ParseDouble(dtCarcassSummary.Rows[0]["MaterialSupplied"].ToString()));
              //  }

              //  // Contract out
              //  if (viewType >= 0)
              //  {
              //    if (viewType == 1)
              //    {
              //      cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = false;
              //      cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;
              //    }
              //    else
              //    {
              //      cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
              //      cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = false;
              //    }
              //  }
              //  else
              //  {
              //    DataRow[] contractOutRow = dsSource.Tables[1].Select("Group = 6");
              //    if (contractOutRow.Length > 0)
              //    {
              //      cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = false;
              //      cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;
              //    }
              //    else
              //    {
              //      cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
              //      cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = false;
              //    }
              //  }

              //  //ControlUtility.ViewCrystalReport(cptItemCostPrice);
              //  View_Report frm = new View_Report(cptItemCostPrice);
              //  frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
              //}

              // Calculate BOM Price
              double totalVNDPrice = 0;
              double totalUSDPrice = 0;
              // End Calculate BOM Price
              DBParameter[] inputParamCosting = new DBParameter[2];
              inputParamCosting[0] = new DBParameter("@CapturePid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["CapturePid"].Value.ToString()));

              int viewTypeDT = -1;
              if (rdMakeLocal.Checked && rdinhousehw.Checked)
              {
                viewTypeDT = 0;
              }
              else if (rdContractOut.Checked && rdsubhw.Checked)
              {
                viewTypeDT = 1;
              }
              else if (rdMakeLocal.Checked && rdsubhw.Checked)
              {
                viewTypeDT = 2;
              }
              else if (rdContractOut.Checked && rdinhousehw.Checked)
              {
                viewTypeDT = 3;
              }

             
              inputParamCosting[1] = new DBParameter("@ViewType", DbType.Int32, viewTypeDT);

              DBParameter[] outputParamCosting = new DBParameter[11];
              outputParamCosting[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
              outputParamCosting[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
              outputParamCosting[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
              outputParamCosting[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, string.Empty);
              outputParamCosting[4] = new DBParameter("@ContractOut", DbType.Int32, int.MinValue);
              outputParamCosting[5] = new DBParameter("@CountPur", DbType.Int32, int.MinValue);
              outputParamCosting[6] = new DBParameter("@ContractOutHW", DbType.Int32, int.MinValue);
              outputParamCosting[7] = new DBParameter("@ExFactoryContractOutVND", DbType.Double, double.MinValue);
              outputParamCosting[8] = new DBParameter("@ExFactoryContractOutUSD", DbType.Double, double.MinValue);
              outputParamCosting[9] = new DBParameter("@ExFactoryInhouseVND", DbType.Double, double.MinValue);
              outputParamCosting[10] = new DBParameter("@ExFactoryInhouseUSD", DbType.Double, double.MinValue);


              DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDItemCostingCapture_Report", 3600, inputParamCosting, outputParamCosting);
              if (dsSource != null && dsSource.Tables.Count > 1)
              {
                dsSource.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
                dsSource.Tables[0].Columns.Add("CheckImage", typeof(String));
                for (int k = 0; k < dsSource.Tables[0].Rows.Count; k++)
                {
                  dsSource.Tables[0].Rows[k]["Image"] = Shared.Utility.FunctionUtility.ImageToByteArrayWithFormat(@dsSource.Tables[0].Rows[k]["img"].ToString(), 380, 1.74, "JPG");
                  dsSource.Tables[0].Rows[k]["CheckImage"] = dsSource.Tables[0].Rows[k]["Image"].ToString();
                }
                dsSource.Tables[1].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
                dsSource.Tables[1].Columns.Add("CheckImage", typeof(String));
                for (int k = 0; k < dsSource.Tables[1].Rows.Count; k++)
                {
                  dsSource.Tables[1].Rows[k]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dsSource.Tables[1].Rows[k]["Picture"].ToString());
                  dsSource.Tables[1].Rows[k]["CheckImage"] = dsSource.Tables[1].Rows[k]["Image"].ToString();
                }

                dsCSDItemCostPriceSummary ds = new dsCSDItemCostPriceSummary();
                ds.Tables["dtItemCostMaster"].Merge(dsSource.Tables[0]);
                ds.Tables["dtItemCostDetail"].Merge(dsSource.Tables[1]);
                ds.Tables["dtACTSalePrice"].Merge(dsSource.Tables[3]);
                ds.Tables["dtTotal"].Merge(dsSource.Tables[7]);
                ds.Tables["dtCostSummary"].Merge(dsSource.Tables[4]);
                ds.Tables["dtPurchaseCost"].Merge(dsSource.Tables[5]);
                ds.Tables["dtMaterialGroupSummary"].Merge(dsSource.Tables[6]);
                cptCSDItemCosrPriceSummary cptItemCostPrice = new cptCSDItemCosrPriceSummary();

                cptItemCostPrice.SetDataSource(ds);
                double dExchange = DBConvert.ParseDouble(outputParamCosting[0].Value.ToString());
                double dFOH = DBConvert.ParseDouble(outputParamCosting[1].Value.ToString());
                double dProfit = DBConvert.ParseDouble(outputParamCosting[2].Value.ToString());
                string remark = outputParamCosting[3].Value.ToString();
                int contractOut = DBConvert.ParseInt(outputParamCosting[4].Value);
                int countPur = DBConvert.ParseInt(outputParamCosting[5].Value);
                double exFactoryContractOutVND = DBConvert.ParseDouble(outputParamCosting[7].Value);
                double exFactoryContractOutUSD = DBConvert.ParseDouble(outputParamCosting[8].Value);
                double exFactoryInhouseVND = DBConvert.ParseDouble(outputParamCosting[9].Value);
                double exFactoryInhouseUSD = DBConvert.ParseDouble(outputParamCosting[10].Value);
                double differentVND = exFactoryContractOutVND - exFactoryInhouseVND;
                double differentUSD = exFactoryContractOutUSD - exFactoryInhouseUSD;
                double percent1 = double.MinValue;
                string percent = string.Empty;
                if (exFactoryInhouseVND > 0)
                {
                  percent1 = Math.Round(differentVND / exFactoryInhouseVND * 100, 2);
                  percent = percent1.ToString() + " %";
                }
                else
                {
                  percent = "";
                }
                cptItemCostPrice.SetParameterValue("ExchangeRate", dExchange);
                cptItemCostPrice.SetParameterValue("FOH", dFOH);
                cptItemCostPrice.SetParameterValue("Profit", dProfit);
                cptItemCostPrice.SetParameterValue("TotalBOMPrice_VND", totalVNDPrice);
                cptItemCostPrice.SetParameterValue("TotalBOMPrice_USD", totalUSDPrice);
                cptItemCostPrice.SetParameterValue("User", SharedObject.UserInfo.EmpName);
                if (exFactoryContractOutVND > 0)
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryContractOutVND", exFactoryContractOutVND);
                }
                else
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryContractOutVND", 0);
                }

                if (exFactoryContractOutUSD > 0)
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryContractOutUSD", exFactoryContractOutUSD);
                }
                else
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryContractOutUSD", 0);
                }

                if (exFactoryInhouseVND > 0)
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryInhouseVND", exFactoryInhouseVND);
                }
                else
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryInhouseVND", 0);
                }

                if (exFactoryInhouseUSD > 0)
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryInhouseUSD", exFactoryInhouseUSD);
                }
                else
                {
                  cptItemCostPrice.SetParameterValue("ExFactoryInhouseUSD", 0);
                }

                if (exFactoryContractOutVND != double.MinValue || exFactoryInhouseVND != double.MinValue)
                {
                  cptItemCostPrice.SetParameterValue("DifferentVND", differentVND);
                }
                else
                {
                  cptItemCostPrice.SetParameterValue("DifferentVND", 0);
                }

                if (exFactoryContractOutUSD != double.MinValue || exFactoryInhouseUSD != double.MinValue)
                {
                  cptItemCostPrice.SetParameterValue("DifferentUSD", differentUSD);
                }
                else
                {
                  cptItemCostPrice.SetParameterValue("DifferentUSD", 0);
                }

                if (percent.ToString().Trim().Length > 0)
                {
                  cptItemCostPrice.SetParameterValue("Percent", percent);
                }
                else
                {
                  cptItemCostPrice.SetParameterValue("Percent", percent);
                }


                //Carcass Summary
                DataTable dtCarcassSummary = dsSource.Tables[2];
                if (dtCarcassSummary.Rows.Count > 0)
                {
                  cptItemCostPrice.SetParameterValue("TotalLabor", dtCarcassSummary.Rows[0]["TotalLabor"]);
                  cptItemCostPrice.SetParameterValue("TotalMainMaterial", dtCarcassSummary.Rows[0]["TotalMainMaterial"]);
                  cptItemCostPrice.SetParameterValue("OtherMaterial", dtCarcassSummary.Rows[0]["OtherMaterial"]);
                  cptItemCostPrice.SetParameterValue("SubconNetPrice", dtCarcassSummary.Rows[0]["SubconNetPrice"]);
                  cptItemCostPrice.SetParameterValue("MaterialSupplied", dtCarcassSummary.Rows[0]["MaterialSupplied"]);
                }

                // New price format (26/8/2016)
                cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
                cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;

                if (exFactoryContractOutVND > 0)
                {
                  cptItemCostPrice.ReportFooterSection1.SectionFormat.EnableSuppress = false;
                  cptItemCostPrice.ReportFooterSection7.SectionFormat.EnableSuppress = true;
                }
                else
                {
                  cptItemCostPrice.ReportFooterSection1.SectionFormat.EnableSuppress = true;
                  cptItemCostPrice.ReportFooterSection7.SectionFormat.EnableSuppress = false;
                }

                //hide/show purchase cost
                if (countPur > 0)
                {
                  cptItemCostPrice.PurchaseCost.SectionFormat.EnableSuppress = false;
                }
                else
                {
                  cptItemCostPrice.PurchaseCost.SectionFormat.EnableSuppress = true;
                }

                //Hide/Show Sale Price
                //if (priceType == 0)
                //{
                  cptItemCostPrice.SalePrice.SectionFormat.EnableSuppress = true;
                //}
                //else
                //{
                //  cptItemCostPrice.SalePrice.SectionFormat.EnableSuppress = false;
                //}

                View_Report frm = new View_Report(cptItemCostPrice);
                frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
              }
            }
            catch (Exception ex) 
            { 
              MessageBox.Show(ex.Message); 
            }
            return;
          }
        }
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {

    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();

      int flag = 1;

      if (colName == "select")
      {
        if (flag == 0)
        {
          return;
        }
        if (flag == 1)
        {
          flag = 0;

          DataSet ds = (DataSet)ultData.DataSource;
          for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
          {
            ds.Tables[0].Rows[i]["Select"] = 0;
          }

          for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
          {
            ds.Tables[1].Rows[i]["Select"] = 0;
          }

          flag = 1;
        }
      }
    }
    #endregion Event

  }
}
