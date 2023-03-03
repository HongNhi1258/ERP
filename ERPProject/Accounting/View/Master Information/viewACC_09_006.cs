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
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_09_006 : MainUserControl
  {
    public int iIndex = 0;
    private string itemCode = string.Empty;
    private IList listCusDeletedPid = new ArrayList();
    private IList listSupDeletedPid = new ArrayList();
    private IList listAssetDeletedPid = new ArrayList();
    private IList listEquipmentDeletedPid = new ArrayList();
    private IList listLoanDeletedPid = new ArrayList();
    private IList listProductDeletedPid = new ArrayList();
    private IList listProcessDeletedPid = new ArrayList();

    private bool isCusDuplicateProcess = false;
    private bool isSupDuplicateProcess = false;
    private bool isAssetDuplicateProcess = false;
    private bool isEquipmentDuplicateProcess = false;
    private bool isLoanDuplicateProcess = false;
    private bool isProductDuplicateProcess = false;

    private DataSet dsLoad = new DataSet();
    private DataSet dsInitData = new DataSet();
    private bool flagCusSup = false;
    private bool flagProduct = false;
    private bool flagAssetEquip = false;
    private bool flagLoan = false;

    private DataTable dtSourceMaterials = new DataTable();

    public viewACC_09_006()
    {
      InitializeComponent();
    }

    private void viewBOM_01_003_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserName + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;
      this.InitTabData();
    }



    #region LoadInit

    private void InitTabData()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Index", DbType.Int64, this.iIndex);
      dsLoad = DataBaseAccess.SearchStoreProcedure("spACCObjectAccountConfiguration_Select", inputParam);
      dsInitData = DataBaseAccess.SearchStoreProcedure("spACCObjectAccountConfiguration_InitData");
      switch (iIndex)
      {
        case 0:
          this.InitDataCusSup();
          ultCustomer.DataSource = dsLoad.Tables[0];
          ultSupplier.DataSource = dsLoad.Tables[1];
          break;
        case 1:
          this.InitDataProduct();
          ultraProduct.DataSource = dsLoad.Tables[0];
          break;
        case 2:
          this.InitDataAssetEquipment();
          ultraAsset.DataSource = dsLoad.Tables[0];
          ultraEquipment.DataSource = dsLoad.Tables[1];
          break;
        case 3:
          this.InitDataLoanType();
          ultraLoanType.DataSource = dsLoad.Tables[0];
          break;
        case 4:
          this.InitDataProcess();
          ultraProcess.DataSource = dsLoad.Tables[0];
          break;
        default:
          break;
      }
    }
    private void InitDataCusSup()
    {
      if (flagCusSup == false)
      {
        // Account List
        Utility.LoadUltraCombo(ucbAccountList, dsInitData.Tables[0], "Value", "AccountCode", false, "Value");
        ucbAccountList.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
        flagCusSup = true;
      }
    }
    private void InitDataProduct()
    {

    }
    private void InitDataAssetEquipment()
    {
      if (flagAssetEquip == false)
      {
        this.flagAssetEquip = true;
      }
    }
    private void InitDataLoanType()
    {
      if (flagLoan == false)
      {
        this.flagLoan = true;
      }
    }
    private void InitDataProcess()
    {
      Utility.LoadUltraCombo(ucbProcessList, dsInitData.Tables[1], "Value", "Display", false, "Value");
      ucbProcessList.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

      Utility.LoadUltraCombo(ucbWorkshopList, dsInitData.Tables[2], "Value", "Display", false, "Value");
      ucbWorkshopList.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
    }
    #endregion LoadInt

    #region CheckSaveData
    private bool CheckSupValid(out string errorMessage)
    {
      errorMessage = string.Empty;

      for (int i = 0; i < ultSupplier.Rows.Count; i++)
      {
        string groupname = ultSupplier.Rows[i].Cells["GroupName"].Value.ToString();
        string saleAcc = ultSupplier.Rows[i].Cells["SaleACCPid"].Value.ToString();
        string depositAcc = ultSupplier.Rows[i].Cells["DepositACCPid"].Value.ToString();
        string purAcc = ultSupplier.Rows[i].Cells["PurchaseACCPid"].Value.ToString();
        int saleaccountPid = DBConvert.ParseInt(ultSupplier.Rows[i].Cells["SaleACCPid"].Value);
        int depositaccountPid = DBConvert.ParseInt(ultSupplier.Rows[i].Cells["DepositACCPid"].Value);
        int puraccountPid = DBConvert.ParseInt(ultSupplier.Rows[i].Cells["PurchaseACCPid"].Value);
        if (groupname.Length <= 0)
        {
          //WindowUtinity.ShowMessageError("MSG0005", string.Format("Group Name at row {0}", i + 1));
          errorMessage = "Group Name";
          ultSupplier.Rows[i].Cells["GroupName"].Selected = true;
          ultSupplier.ActiveRowScrollRegion.FirstRow = ultSupplier.Rows[i];
          return false;
        }
        //Check Sale Acc
        if (saleAcc.Length <= 0)
        {
          errorMessage = "Sale Account";
          ultSupplier.Rows[i].Cells["SaleACCPid"].Selected = true;
          ultSupplier.ActiveRowScrollRegion.FirstRow = ultSupplier.Rows[i];
          return false;
        }

        if (saleAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", saleaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Sale Account  is not exists";
            ultSupplier.Rows[i].Cells["SaleACCPid"].Selected = true;
            return false;
          }
        }

        //Check Deposit Acc
        if (depositAcc.Length <= 0)
        {
          errorMessage = "Deposit Account";
          ultSupplier.Rows[i].Cells["DepositACCPid"].Selected = true;
          ultSupplier.ActiveRowScrollRegion.FirstRow = ultSupplier.Rows[i];
          return false;
        }

        if (depositAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", depositaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Deposit Account  is not exists";
            ultSupplier.Rows[i].Cells["DepositACCPid"].Selected = true;
            return false;
          }
        }

        //Check pur Acc
        if (depositAcc.Length <= 0)
        {
          errorMessage = "Purchase Account";
          ultSupplier.Rows[i].Cells["PurchaseACCPid"].Selected = true;
          ultSupplier.ActiveRowScrollRegion.FirstRow = ultSupplier.Rows[i];
          return false;
        }

        if (depositAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", puraccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Deposit Account  is not exists";
            ultSupplier.Rows[i].Cells["PurchaseACCPid"].Selected = true;
            return false;
          }
        }
      }
      //Check trùng Group Name
      this.CheckProcessSupDuplicate();
      if (this.isSupDuplicateProcess == true)
      {
        errorMessage = "Row duplicate"; ;
        return false;
      }
      return true;
    }

    private bool CheckCusValid(out string errorMessage)
    {
      errorMessage = string.Empty;

      for (int i = 0; i < ultCustomer.Rows.Count; i++)
      {

        string groupname = ultCustomer.Rows[i].Cells["GroupName"].Value.ToString();
        string saleAcc = ultCustomer.Rows[i].Cells["SaleACCPid"].Value.ToString();
        string depositAcc = ultCustomer.Rows[i].Cells["DepositACCPid"].Value.ToString();
        string purAcc = ultCustomer.Rows[i].Cells["PurchaseACCPid"].Value.ToString();
        int saleaccountPid = DBConvert.ParseInt(ultCustomer.Rows[i].Cells["SaleACCPid"].Value);
        int depositaccountPid = DBConvert.ParseInt(ultCustomer.Rows[i].Cells["DepositACCPid"].Value);
        int puraccountPid = DBConvert.ParseInt(ultCustomer.Rows[i].Cells["PurchaseACCPid"].Value);
        if (groupname.Length <= 0)
        {
          //WindowUtinity.ShowMessageError("MSG0005", string.Format("Group Name at row {0}", i + 1));
          errorMessage = "Group Name";
          ultCustomer.Rows[i].Cells["GroupName"].Selected = true;
          ultCustomer.ActiveRowScrollRegion.FirstRow = ultCustomer.Rows[i];
          return false;
        }
        //Check Sale Acc
        if (saleAcc.Length <= 0)
        {
          errorMessage = "Sale Account";
          ultCustomer.Rows[i].Cells["SaleACCPid"].Selected = true;
          ultCustomer.ActiveRowScrollRegion.FirstRow = ultCustomer.Rows[i];
          return false;
        }

        if (saleAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", saleaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Sale Account  is not exists";
            ultCustomer.Rows[i].Cells["SaleACCPid"].Selected = true;
            return false;
          }
        }

        //Check Deposit Acc
        if (depositAcc.Length <= 0)
        {
          errorMessage = "Deposit Account";
          ultCustomer.Rows[i].Cells["DepositACCPid"].Selected = true;
          ultCustomer.ActiveRowScrollRegion.FirstRow = ultCustomer.Rows[i];
          return false;
        }

        if (depositAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", depositaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Deposit Account  is not exists";
            ultCustomer.Rows[i].Cells["DepositACCPid"].Selected = true;
            return false;
          }
        }

        //Check pur Acc
        if (depositAcc.Length <= 0)
        {
          errorMessage = "Purchase Account";
          ultCustomer.Rows[i].Cells["PurchaseACCPid"].Selected = true;
          ultCustomer.ActiveRowScrollRegion.FirstRow = ultCustomer.Rows[i];
          return false;
        }

        if (depositAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", puraccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Deposit Account  is not exists";
            ultCustomer.Rows[i].Cells["PurchaseACCPid"].Selected = true;
            return false;
          }
        }

      }
      //Check trùng Group Name
      this.CheckProcessCusDuplicate();
      if (this.isCusDuplicateProcess == true)
      {
        errorMessage = "Row duplicate";
        return false;
      }
      return true;
    }

    private bool CheckProductValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultraProduct.Rows.Count; i++)
      {
        string groupname = ultraProduct.Rows[i].Cells["GroupName"].Value.ToString();
        string stockAcc = ultraProduct.Rows[i].Cells["StockACCPid"].Value.ToString();
        string revenueInternalAcc = ultraProduct.Rows[i].Cells["RevenueInternalACCPid"].Value.ToString();
        string costPriceAcc = ultraProduct.Rows[i].Cells["CostPriceACCPid"].Value.ToString();
        string exportRevenueAcc = ultraProduct.Rows[i].Cells["ExportRevenueACCPid"].Value.ToString();
        string saleReturnAcc = ultraProduct.Rows[i].Cells["ACSaleReturnPid"].Value.ToString();
        string discountAcc = ultraProduct.Rows[i].Cells["DiscountACCPid"].Value.ToString();
        string localRevenueAcc = ultraProduct.Rows[i].Cells["LocalRevenueACCPid"].Value.ToString();
        string outsourcingRevenueAcc = ultraProduct.Rows[i].Cells["OutsourcingRevenueACCPid"].Value.ToString();

        int stockaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["StockACCPid"].Value);
        int revenueInternalaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["RevenueInternalACCPid"].Value);
        int costPriceaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["CostPriceACCPid"].Value);
        int exportRevenueaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["ExportRevenueACCPid"].Value);
        int saleReturnaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["ACSaleReturnPid"].Value);
        int localRevenueaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["LocalRevenueACCPid"].Value);
        int discountaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["DiscountACCPid"].Value);
        int outsourcingRevenueaccountPid = DBConvert.ParseInt(ultraProduct.Rows[i].Cells["OutsourcingRevenueACCPid"].Value);

        if (groupname.Length <= 0)
        {
          //WindowUtinity.ShowMessageError("MSG0005", string.Format("Group Name at row {0}", i + 1));
          errorMessage = "Group Name";
          ultraProduct.Rows[i].Cells["GroupName"].Selected = true;
          ultraProduct.ActiveRowScrollRegion.FirstRow = ultraProduct.Rows[i];
          return false;
        }
        //Check Sale Acc        
        if (stockAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", stockaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Stock Account is not exists";
            ultraProduct.Rows[i].Cells["StockACCPid"].Selected = true;
            return false;
          }
        }
        //Check revenue internal acc     
        if (revenueInternalAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", revenueInternalaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Revenue Internal Account is not exists";
            ultraProduct.Rows[i].Cells["RevenueInternalACCPid"].Selected = true;
            return false;
          }
        }

        //Check cost price Acc
        if (costPriceAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", costPriceaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Cost Price Account  is not exists";
            ultraProduct.Rows[i].Cells["CostPriceACCPid"].Selected = true;
            return false;
          }
        }

        //Check ExportRevenueACCPid Acc      
        if (exportRevenueAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", exportRevenueaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Export Revenue Account  is not exists";
            ultraProduct.Rows[i].Cells["ExportRevenueACCPid"].Selected = true;
            return false;
          }
        }

        //Check ACSaleReturnPid Acc       
        if (saleReturnAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", saleReturnaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Sale Return Account  is not exists";
            ultraProduct.Rows[i].Cells["ACSaleReturnPid"].Selected = true;
            return false;
          }
        }

        //Check LocalRevenueACCPid Acc       
        if (localRevenueAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", localRevenueaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Local Revenue Account is not exists";
            ultraProduct.Rows[i].Cells["LocalRevenueACCPid"].Selected = true;
            return false;
          }
        }

        //Check DiscountACCPid Acc        
        if (discountAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", discountaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Discount Account  is not exists";
            ultraProduct.Rows[i].Cells["DiscountACCPid"].Selected = true;
            return false;
          }
        }

        //Check OutsourcingRevenueACCPid Acc       
        if (outsourcingRevenueAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", outsourcingRevenueaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "OutSourcing Revenue Account  is not exists";
            ultraProduct.Rows[i].Cells["OutsourcingRevenueACCPid"].Selected = true;
            return false;
          }
        }
      }
      //Check trùng Group Name
      this.CheckProcessProductDuplicate();
      if (this.isProductDuplicateProcess == true)
      {
        errorMessage = "Row duplicate";
        return false;
      }
      return true;
    }

    private bool CheckAssetValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultraAsset.Rows.Count; i++)
      {
        //GroupName , AccountPid , DepreciationACCPid, CostACCPid, DepreciationMonth 
        string groupname = ultraAsset.Rows[i].Cells["GroupName"].Value.ToString();
        string account = ultraAsset.Rows[i].Cells["AccountPid"].Value.ToString();
        string depreciationAcc = ultraAsset.Rows[i].Cells["DepreciationACCPid"].Value.ToString();
        string costAcc = ultraAsset.Rows[i].Cells["CostACCPid"].Value.ToString();

        int accountPid = DBConvert.ParseInt(ultraAsset.Rows[i].Cells["AccountPid"].Value);
        int depreciationaccountPid = DBConvert.ParseInt(ultraAsset.Rows[i].Cells["DepreciationACCPid"].Value);
        int costaccountPid = DBConvert.ParseInt(ultraAsset.Rows[i].Cells["CostACCPid"].Value);

        if (groupname.Length <= 0)
        {
          //WindowUtinity.ShowMessageError("MSG0005", string.Format("Group Name at row {0}", i + 1));
          errorMessage = "Group Name";
          ultraAsset.Rows[i].Cells["GroupName"].Selected = true;
          ultraAsset.ActiveRowScrollRegion.FirstRow = ultraAsset.Rows[i];
          return false;
        }
        //Check Acc
        //if (account.Length <= 0)
        //{
        //  errorMessage = "Account";
        //  ultraAsset.Rows[i].Cells["AccountPid"].Selected = true;
        //  ultraAsset.ActiveRowScrollRegion.FirstRow = ultraAsset.Rows[i];
        //  return false;
        //}
        if (account.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", accountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Account is not exists";
            ultraAsset.Rows[i].Cells["AccountPid"].Selected = true;
            return false;
          }
        }
        //Check depreciation acc
        //if (depreciationAcc.Length <= 0)
        //{
        //  errorMessage = "Depreciation Account";
        //  ultraAsset.Rows[i].Cells["DepreciationACCPid"].Selected = true;
        //  ultraAsset.ActiveRowScrollRegion.FirstRow = ultraAsset.Rows[i];
        //  return false;
        //}
        if (depreciationAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", depreciationaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Depreciation Account is not exists";
            ultraAsset.Rows[i].Cells["DepreciationACCPid"].Selected = true;
            return false;
          }
        }

        //Check CostACCPid Acc
        //if (costAcc.Length <= 0)
        //{
        //  errorMessage = "Cost Account";
        //  ultraAsset.Rows[i].Cells["CostACCPid"].Selected = true;
        //  ultraAsset.ActiveRowScrollRegion.FirstRow = ultraAsset.Rows[i];
        //  return false;
        //}

        if (costAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", costaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "CostAccount  is not exists";
            ultraAsset.Rows[i].Cells["CostACCPid"].Selected = true;
            return false;
          }
        }
      }
      //Check trùng Group Name
      this.CheckProcessAssetDuplicate();
      if (this.isAssetDuplicateProcess == true)
      {
        errorMessage = "Row duplicate";
        return false;
      }
      return true;
    }

    private bool CheckEquipmentValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultraEquipment.Rows.Count; i++)
      {
        //GroupName , AccountPid , DepreciationACCPid, CostACCPid, DepreciationMonth 
        string groupname = ultraEquipment.Rows[i].Cells["GroupName"].Value.ToString();
        string account = ultraEquipment.Rows[i].Cells["AccountPid"].Value.ToString();
        string depreciationAcc = ultraEquipment.Rows[i].Cells["DepreciationACCPid"].Value.ToString();
        string costAcc = ultraEquipment.Rows[i].Cells["CostACCPid"].Value.ToString();

        int accountPid = DBConvert.ParseInt(ultraEquipment.Rows[i].Cells["AccountPid"].Value);
        int depreciationaccountPid = DBConvert.ParseInt(ultraEquipment.Rows[i].Cells["DepreciationACCPid"].Value);
        int costaccountPid = DBConvert.ParseInt(ultraEquipment.Rows[i].Cells["CostACCPid"].Value);

        if (groupname.Length <= 0)
        {
          //WindowUtinity.ShowMessageError("MSG0005", string.Format("Group Name at row {0}", i + 1));
          errorMessage = "Group Name";
          ultraEquipment.Rows[i].Cells["GroupName"].Selected = true;
          ultraEquipment.ActiveRowScrollRegion.FirstRow = ultraEquipment.Rows[i];
          return false;
        }
        //Check Acc
        //if (account.Length <= 0)
        //{
        //  errorMessage = "Account";
        //  ultraEquipment.Rows[i].Cells["AccountPid"].Selected = true;
        //  ultraEquipment.ActiveRowScrollRegion.FirstRow = ultraEquipment.Rows[i];
        //  return false;
        //}
        if (account.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", accountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Account is not exists";
            ultraEquipment.Rows[i].Cells["AccountPid"].Selected = true;
            return false;
          }
        }
        //Check depreciation acc
        //if (depreciationAcc.Length <= 0)
        //{
        //  errorMessage = "Depreciation Account";
        //  ultraEquipment.Rows[i].Cells["DepreciationACCPid"].Selected = true;
        //  ultraEquipment.ActiveRowScrollRegion.FirstRow = ultraEquipment.Rows[i];
        //  return false;
        //}
        if (depreciationAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", depreciationaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Depreciation Account is not exists";
            ultraEquipment.Rows[i].Cells["DepreciationACCPid"].Selected = true;
            return false;
          }
        }

        //Check CostACCPid Acc
        //if (costAcc.Length <= 0)
        //{
        //  errorMessage = "Cost Account";
        //  ultraEquipment.Rows[i].Cells["CostACCPid"].Selected = true;
        //  ultraEquipment.ActiveRowScrollRegion.FirstRow = ultraEquipment.Rows[i];
        //  return false;
        //}

        if (costAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", costaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Cost Account  is not exists";
            ultraEquipment.Rows[i].Cells["CostACCPid"].Selected = true;
            return false;
          }
        }
      }

      //Check trùng Group Name
      this.CheckProcessEquimentDuplicate();
      if (this.isEquipmentDuplicateProcess == true)
      {
        errorMessage = "Row duplicate";
        return false;
      }
      return true;
    }

    private bool CheckLoanValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultraLoanType.Rows.Count; i++)
      {
        // LoanTypeName, LoanACCPid,InterestExpenseACCPid,OverduePenaltyACCPid
        string groupname = ultraLoanType.Rows[i].Cells["LoanTypeName"].Value.ToString();
        string loanaccount = ultraLoanType.Rows[i].Cells["LoanACCPid"].Value.ToString();
        string interestExpenseAcc = ultraLoanType.Rows[i].Cells["InterestExpenseACCPid"].Value.ToString();
        string overDuePenaltyAcc = ultraLoanType.Rows[i].Cells["OverduePenaltyACCPid"].Value.ToString();

        int loanaccountPid = DBConvert.ParseInt(ultraLoanType.Rows[i].Cells["LoanACCPid"].Value);
        int interestExpenseaccountPid = DBConvert.ParseInt(ultraLoanType.Rows[i].Cells["InterestExpenseACCPid"].Value);
        int overDuepenaltyaccountPid = DBConvert.ParseInt(ultraLoanType.Rows[i].Cells["OverduePenaltyACCPid"].Value);

        if (groupname.Length <= 0)
        {
          //WindowUtinity.ShowMessageError("MSG0005", string.Format("Loan Type Name at row {0}", i + 1));
          errorMessage = "Loan Type Name";
          ultraLoanType.Rows[i].Cells["LoanTypeName"].Selected = true;
          ultraLoanType.ActiveRowScrollRegion.FirstRow = ultraLoanType.Rows[i];
          return false;
        }
        //Check LoanACCPid
        if (loanaccount.Length <= 0)
        {
          errorMessage = "Loan Account";
          ultraLoanType.Rows[i].Cells["LoanACCPid"].Selected = true;
          ultraLoanType.ActiveRowScrollRegion.FirstRow = ultraLoanType.Rows[i];
          return false;
        }
        if (loanaccount.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", loanaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Loan Account is not exists";
            ultraLoanType.Rows[i].Cells["LoanACCPid"].Selected = true;
            return false;
          }
        }
        //Check InterestExpenseACCPid acc
        if (interestExpenseAcc.Length <= 0)
        {
          errorMessage = "Interest Expense Account";
          ultraLoanType.Rows[i].Cells["InterestExpenseACCPid"].Selected = true;
          ultraLoanType.ActiveRowScrollRegion.FirstRow = ultraLoanType.Rows[i];
          return false;
        }
        if (interestExpenseAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", interestExpenseaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Interest Expense Account is not exists";
            ultraLoanType.Rows[i].Cells["InterestExpenseACCPid"].Selected = true;
            return false;
          }
        }

        //Check OverduePenaltyACCPid Acc
        if (overDuePenaltyAcc.Length <= 0)
        {
          errorMessage = "Cost Account";
          ultraLoanType.Rows[i].Cells["OverduePenaltyACCPid"].Selected = true;
          ultraLoanType.ActiveRowScrollRegion.FirstRow = ultraLoanType.Rows[i];
          return false;
        }

        if (overDuePenaltyAcc.Length > 0)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", overDuepenaltyaccountPid);
          DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dSource.Rows.Count <= 0)
          {
            errorMessage = "Over Due Penalty Account  is not exists";
            ultraLoanType.Rows[i].Cells["OverduePenaltyACCPid"].Selected = true;
            return false;
          }
        }
      }
      //Check trùng Group Name
      this.CheckProcessLoanDuplicate();
      if (this.isLoanDuplicateProcess == true)
      {
        errorMessage = "Row duplicate";
        return false;
      }
      return true;
    }
    private bool CheckProcessValid(out string errorMessage, DataRow row, int index)
    {
      errorMessage = string.Empty;
      int processPid = DBConvert.ParseInt(row["ProcessPid"].ToString());
      int workshopPid = DBConvert.ParseInt(row["WorkshopPid"].ToString());
      int acCostPid = DBConvert.ParseInt(row["ACCostPid"].ToString());
      int acUnfinishPid = DBConvert.ParseInt(row["ACUnfinishPid"].ToString());

      //Check process       
      if (processPid > 0)
      {
        string cm1 = string.Format(@"SELECT ProcessPid FROM VBOMMainProcess WHERE ProcessPid = '{0}'", processPid);
        DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
        if (dSource.Rows.Count <= 0)
        {
          errorMessage = "Process";
          ultraProcess.Rows[index].Cells["ProcessPid"].Selected = true;
          return false;
        }
      }
      //Check workshop       
      if (workshopPid > 0)
      {
        string cm1 = string.Format(@"SELECT WorkshopPid FROM VBOMWorkshop WHERE WorkshopPid = '{0}'", workshopPid);
        DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
        if (dSource.Rows.Count <= 0)
        {
          errorMessage = "Workshop";
          ultraProcess.Rows[index].Cells["WorkshopPid"].Selected = true;
          return false;
        }
      }
      //Check cost account       
      if (acCostPid > 0)
      {
        string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", acCostPid);
        DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
        if (dSource.Rows.Count <= 0)
        {
          errorMessage = "Cost Account";
          ultraProcess.Rows[index].Cells["ACCostPid"].Selected = true;
          return false;
        }
      }
      //Check unfinish account
      if (acUnfinishPid > 0)
      {
        string cm1 = string.Format(@"SELECT A.Pid
		                                    FROM TblACCAccount A
		                                    LEFT JOIN (
							                                      SELECT DISTINCT  ParentPid
							                                      FROM TblACCAccount 
							                                      ) B ON A.Pid = B.ParentPid 
		                                    WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", acUnfinishPid);
        DataTable dSource = DataBaseAccess.SearchCommandTextDataTable(cm1);
        if (dSource.Rows.Count <= 0)
        {
          errorMessage = "Unfinish Account";
          ultraProcess.Rows[index].Cells["ACUnfinishPid"].Selected = true;
          return false;
        }
      }
      return true;
    }

    #endregion CheckSaveData

    #region SaveData
    private bool SaveCus()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listCusDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listCusDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCSupCusAccount_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ultCustomer.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[7];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (row["GroupName"].ToString().Length > 0)
          {
            inputParam[1] = new DBParameter("@GroupName", DbType.String, row["GroupName"].ToString());
          }
          if (DBConvert.ParseInt(row["SaleACCPid"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@SaleACCPid", DbType.Int32, DBConvert.ParseInt(row["SaleACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["PurchaseACCPid"].ToString()) != int.MinValue)
          {
            inputParam[3] = new DBParameter("@PurchaseACCPid", DbType.Int32, DBConvert.ParseInt(row["PurchaseACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepositACCPid"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@DepositACCPid", DbType.Int32, DBConvert.ParseInt(row["DepositACCPid"].ToString()));
          }
          inputParam[5] = new DBParameter("@ObjectType", DbType.Int32, 1);
          inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCSupCusAccount_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveSup()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listSupDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listSupDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCSupCusAccount_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ultSupplier.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[7];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (row["GroupName"].ToString().Length > 0)
          {
            inputParam[1] = new DBParameter("@GroupName", DbType.String, row["GroupName"].ToString());
          }
          if (DBConvert.ParseInt(row["SaleACCPid"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@SaleACCPid", DbType.Int32, DBConvert.ParseInt(row["SaleACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["PurchaseACCPid"].ToString()) != int.MinValue)
          {
            inputParam[3] = new DBParameter("@PurchaseACCPid", DbType.Int32, DBConvert.ParseInt(row["PurchaseACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepositACCPid"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@DepositACCPid", DbType.Int32, DBConvert.ParseInt(row["DepositACCPid"].ToString()));
          }
          inputParam[5] = new DBParameter("@ObjectType", DbType.Int32, 2);
          inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCSupCusAccount_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveProduct()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listProductDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listProductDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCProductAccount_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ultraProduct.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[11];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[1] = new DBParameter("@GroupName", DbType.String, row["GroupName"].ToString());
          if (DBConvert.ParseInt(row["StockACCPid"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@StockACCPid", DbType.Int32, DBConvert.ParseInt(row["StockACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["RevenueInternalACCPid"].ToString()) != int.MinValue)
          {
            inputParam[3] = new DBParameter("@RevenueInternalACCPid", DbType.Int32, DBConvert.ParseInt(row["RevenueInternalACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CostPriceACCPid"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@CostPriceACCPid", DbType.Int32, DBConvert.ParseInt(row["CostPriceACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["ExportRevenueACCPid"].ToString()) != int.MinValue)
          {
            inputParam[5] = new DBParameter("@RevenueExportACCPid", DbType.Int32, DBConvert.ParseInt(row["ExportRevenueACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["ACSaleReturnPid"].ToString()) != int.MinValue)
          {
            inputParam[6] = new DBParameter("@SaleReturnACCPid", DbType.Int32, DBConvert.ParseInt(row["ACSaleReturnPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DiscountACCPid"].ToString()) != int.MinValue)
          {
            inputParam[7] = new DBParameter("@DiscountACCPid", DbType.Int32, DBConvert.ParseInt(row["DiscountACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["LocalRevenueACCPid"].ToString()) != int.MinValue)
          {
            inputParam[8] = new DBParameter("@RevenueLocalACCPid", DbType.Int32, DBConvert.ParseInt(row["LocalRevenueACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["OutsourcingRevenueACCPid"].ToString()) != int.MinValue)
          {
            inputParam[9] = new DBParameter("@RevenueOutsourcingACCPid", DbType.Int32, DBConvert.ParseInt(row["OutsourcingRevenueACCPid"].ToString()));
          }
          inputParam[10] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCProductAccount_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveAsset()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listAssetDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listAssetDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCAssetEquipAccount_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update   
      DataTable dtDetail = (DataTable)ultraAsset.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[8];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (row["GroupName"].ToString().Length > 0)
          {
            inputParam[1] = new DBParameter("@GroupName", DbType.String, row["GroupName"].ToString());
          }
          if (DBConvert.ParseInt(row["AccountPid"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@AcountPid", DbType.Int32, DBConvert.ParseInt(row["AccountPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepreciationACCPid"].ToString()) != int.MinValue)
          {
            inputParam[3] = new DBParameter("@DepreciationACCPid", DbType.Int32, DBConvert.ParseInt(row["DepreciationACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CostACCPid"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@CostACCPid", DbType.Int32, DBConvert.ParseInt(row["CostACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepreciationMonth"].ToString()) != int.MinValue)
          {
            inputParam[5] = new DBParameter("@DepreciationMonth", DbType.Int32, DBConvert.ParseInt(row["DepreciationMonth"].ToString()));
          }
          inputParam[6] = new DBParameter("@GroupType", DbType.Int32, 1);
          inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCAssetEquipAccount_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveEquipment()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listEquipmentDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listEquipmentDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCAssetEquipAccount_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update   
      DataTable dtDetail = (DataTable)ultraEquipment.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[8];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (row["GroupName"].ToString().Length > 0)
          {
            inputParam[1] = new DBParameter("@GroupName", DbType.String, row["GroupName"].ToString());
          }
          if (DBConvert.ParseInt(row["AccountPid"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@AcountPid", DbType.Int32, DBConvert.ParseInt(row["AccountPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepreciationACCPid"].ToString()) != int.MinValue)
          {
            inputParam[3] = new DBParameter("@DepreciationACCPid", DbType.Int32, DBConvert.ParseInt(row["DepreciationACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CostACCPid"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@CostACCPid", DbType.Int32, DBConvert.ParseInt(row["CostACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DepreciationMonth"].ToString()) != int.MinValue)
          {
            inputParam[5] = new DBParameter("@DepreciationMonth", DbType.Int32, DBConvert.ParseInt(row["DepreciationMonth"].ToString()));
          }
          inputParam[6] = new DBParameter("@GroupType", DbType.Int32, 2);
          inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCAssetEquipAccount_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveLoanType()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listLoanDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listLoanDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCLoanAccount_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update   
      DataTable dtDetail = (DataTable)ultraLoanType.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[6];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (row["LoanTypeName"].ToString().Length > 0)
          {
            inputParam[1] = new DBParameter("@GroupName", DbType.String, row["LoanTypeName"].ToString());
          }
          if (DBConvert.ParseInt(row["LoanACCPid"].ToString()) != int.MinValue)
          {
            inputParam[2] = new DBParameter("@LoanACCPid", DbType.Int32, DBConvert.ParseInt(row["LoanACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["InterestExpenseACCPid"].ToString()) != int.MinValue)
          {
            inputParam[3] = new DBParameter("@InterestExpenseACCPid", DbType.Int32, DBConvert.ParseInt(row["InterestExpenseACCPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["OverduePenaltyACCPid"].ToString()) != int.MinValue)
          {
            inputParam[4] = new DBParameter("@OverduePenaltyACCPid", DbType.Int32, DBConvert.ParseInt(row["OverduePenaltyACCPid"].ToString()));
          }
          inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spACCLoanAccount_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveProcess(out string errorMessage)
    {
      bool success = true;
      errorMessage = string.Empty;
      int index = -1;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listProcessDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listProcessDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spACCProcessAccount_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ultraProcess.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        index++;
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (CheckProcessValid(out errorMessage, row, index))
          {
            DBParameter[] inputParam = new DBParameter[8];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }

            if (DBConvert.ParseInt(row["ProcessPid"].ToString()) != int.MinValue)
            {
              inputParam[1] = new DBParameter("@ProcessPid", DbType.Int32, DBConvert.ParseInt(row["ProcessPid"].ToString()));
            }
            if (DBConvert.ParseInt(row["WorkshopPid"].ToString()) != int.MinValue)
            {
              inputParam[2] = new DBParameter("@WorkshopPid", DbType.Int32, DBConvert.ParseInt(row["WorkshopPid"].ToString()));
            }
            if (DBConvert.ParseInt(row["ACCostPid"].ToString()) != int.MinValue)
            {
              inputParam[3] = new DBParameter("@ACCostPid", DbType.Int32, DBConvert.ParseInt(row["ACCostPid"].ToString()));
            }
            if (DBConvert.ParseInt(row["ACUnfinishPid"].ToString()) != int.MinValue)
            {
              inputParam[4] = new DBParameter("@ACUnfinishPid", DbType.Int32, DBConvert.ParseInt(row["ACUnfinishPid"].ToString()));
            }
            if (string.IsNullOrEmpty(row["IsForWO"].ToString()))
            {
              inputParam[5] = new DBParameter("@IsForWO", DbType.Boolean, false);
            }
            else
            {
              inputParam[5] = new DBParameter("@IsForWO", DbType.Boolean, row["IsForWO"]);
            }
            if (string.IsNullOrEmpty(row["IsForWO"].ToString()))
            {
              inputParam[6] = new DBParameter("@IsForMainMaterial", DbType.Boolean, false);
            }
            else
            {
              inputParam[6] = new DBParameter("@IsForMainMaterial", DbType.Boolean, row["IsForMainMaterial"]);
            }

            inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spACCProcessAccount_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
          else
          {
            return success = false;
          }
        }
      }
      return success;
    }

    #endregion SaveData

    #region Save
    private void SaveDataCusSup()
    {
      string errorMessage;
      if (this.CheckCusValid(out errorMessage))
      {
        if (this.CheckSupValid(out errorMessage))
        {
          bool success = true;
          if (this.SaveCus())
          {
            success = this.SaveSup();

          }
          else
          {
            success = false;
          }
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            WindowUtinity.ShowMessageError("WRN0004");
          }
          this.InitTabData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", errorMessage);
          this.SaveSuccess = false;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void SaveDataProduct()
    {
      string errorMessage;
      if (this.CheckProductValid(out errorMessage))
      {
        bool success = true;
        success = this.SaveProduct();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.InitTabData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void SaveDataAssetEquipment()
    {
      string errorMessage;
      if (this.CheckAssetValid(out errorMessage))
      {
        if (this.CheckEquipmentValid(out errorMessage))
        {
          bool success = true;
          if (this.SaveAsset())
          {
            success = this.SaveEquipment();

          }
          else
          {
            success = false;
          }
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            WindowUtinity.ShowMessageError("WRN0004");
          }
          this.InitTabData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", errorMessage);
          this.SaveSuccess = false;
        }

      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void SaveDataLoanType()
    {
      string errorMessage;
      if (this.CheckLoanValid(out errorMessage))
      {
        bool success = true;
        success = this.SaveLoanType();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.InitTabData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void SaveDataProcess()
    {
      string errorMessage;
      bool success = this.SaveProcess(out errorMessage);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.InitTabData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    #endregion Save

    #region ButtonSave
    private void btnCusSupSave_Click(object sender, EventArgs e)
    {
      this.SaveDataCusSup();
    }

    private void btnProductSave_Click(object sender, EventArgs e)
    {
      this.SaveDataProduct();
    }

    private void btnAssetEquipSave_Click(object sender, EventArgs e)
    {
      this.SaveDataAssetEquipment();
    }

    private void btnLoanTypeSave_Click(object sender, EventArgs e)
    {
      this.SaveDataLoanType();
    }

    private void btnProcesSave_Click(object sender, EventArgs e)
    {
      this.SaveDataProcess();
    }
    #endregion ButtonSave

    #region InitializeLayout
    private void ultraProduct_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ultraProduct.SyncWithCurrencyManager = false;
      ultraProduct.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      // Set caption column
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Tên nhóm TK";
      e.Layout.Bands[0].Columns["StockACCPid"].Header.Caption = "TK tồn kho";
      e.Layout.Bands[0].Columns["CostPriceACCPid"].Header.Caption = "TK giá vốn";
      e.Layout.Bands[0].Columns["ExportRevenueACCPid"].Header.Caption = "TK doanh thu\nxuất khẩu";
      e.Layout.Bands[0].Columns["LocalRevenueACCPid"].Header.Caption = "TK doanh thu\nnội địa";
      e.Layout.Bands[0].Columns["ACSaleReturnPid"].Header.Caption = "TK hàng bán\ntrả lại";
      e.Layout.Bands[0].Columns["DiscountACCPid"].Header.Caption = "TK chiết khấu";
      e.Layout.Bands[0].Columns["RevenueInternalACCPid"].Header.Caption = "TK doanh thu\nnội bộ";
      e.Layout.Bands[0].Columns["OutsourcingRevenueACCPid"].Header.Caption = "TK doanh thu\ngia công";

      // Set dropdownlist for column      
      e.Layout.Bands[0].Columns["StockACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["RevenueInternalACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["CostPriceACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["ExportRevenueACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["ACSaleReturnPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["DiscountACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["LocalRevenueACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["OutsourcingRevenueACCPid"].ValueList = ucbAccountList;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["GroupName"].Width = 120;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["StockACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["StockACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["RevenueInternalACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["RevenueInternalACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["CostPriceACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CostPriceACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["ExportRevenueACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ExportRevenueACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["ACSaleReturnPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ACSaleReturnPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["DiscountACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DiscountACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["LocalRevenueACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["LocalRevenueACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["OutsourcingRevenueACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["OutsourcingRevenueACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
    }

    private void ultraAsset_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ultraAsset.SyncWithCurrencyManager = false;
      ultraAsset.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      // GroupName , AccountPid , ACDepreciationPid DepreciationACCPid, ACCostPid  CostACCPid, DepreciationMonth 
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["AccountPid"].Header.Caption = "Account";
      e.Layout.Bands[0].Columns["DepreciationACCPid"].Header.Caption = "Depreciation Account";
      e.Layout.Bands[0].Columns["CostACCPid"].Header.Caption = "Cost Account";
      e.Layout.Bands[0].Columns["DepreciationMonth"].Header.Caption = "Depreciation Month";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["AccountPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["DepreciationACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["CostACCPid"].ValueList = ucbAccountList;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["GroupName"].Width = 120;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["AccountPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AccountPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["CostACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CostACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["DepreciationACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DepreciationACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
    }

    private void ultraEquipment_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ultraEquipment.SyncWithCurrencyManager = false;
      ultraEquipment.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      // GroupName , AccountPid , ACDepreciationPid DepreciationACCPid, ACCostPid  CostACCPid, DepreciationMonth 
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["AccountPid"].Header.Caption = "Account";
      e.Layout.Bands[0].Columns["DepreciationACCPid"].Header.Caption = "Depreciation Account";
      e.Layout.Bands[0].Columns["CostACCPid"].Header.Caption = "Cost Account";
      e.Layout.Bands[0].Columns["DepreciationMonth"].Header.Caption = "Depreciation Month";
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["AccountPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["DepreciationACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["CostACCPid"].ValueList = ucbAccountList;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["GroupName"].Width = 120;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["GroupName"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["GroupName"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["AccountPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["AccountPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["CostACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CostACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["DepreciationACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DepreciationACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

    }

    private void ultraLoanType_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ultraLoanType.SyncWithCurrencyManager = false;
      ultraLoanType.StyleSetName = "Excel2013";

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      // LoanTypeName , ACLoanPid LoanACCPid, ACInterestExpensePid InterestExpenseACCPid, ACOverduePenaltyPid OverduePenaltyACCPid
      e.Layout.Bands[0].Columns["LoanTypeName"].Header.Caption = "Loan Type Name";
      e.Layout.Bands[0].Columns["LoanACCPid"].Header.Caption = "Loan Account";
      e.Layout.Bands[0].Columns["InterestExpenseACCPid"].Header.Caption = "Interest Expense\n Account";
      e.Layout.Bands[0].Columns["OverduePenaltyACCPid"].Header.Caption = "Overdue Penalty\n Account";
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["LoanACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["InterestExpenseACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["OverduePenaltyACCPid"].ValueList = ucbAccountList;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["LoanTypeName"].Width = 120;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["LoanACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["LoanACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["InterestExpenseACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["InterestExpenseACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["OverduePenaltyACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["OverduePenaltyACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
    }

    private void ultCustomer_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ultCustomer.SyncWithCurrencyManager = false;
      ultCustomer.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      //ACGroupName GroupName, ACSalePid SaleACCPid, ACDepositPid DepositACCPid, ACPurchasePid PurchaseACCPid
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["SaleACCPid"].Header.Caption = "Sale Account";
      e.Layout.Bands[0].Columns["PurchaseACCPid"].Header.Caption = "Purchase Account";
      e.Layout.Bands[0].Columns["DepositACCPid"].Header.Caption = "Deposit Account";
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["SaleACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["PurchaseACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["DepositACCPid"].ValueList = ucbAccountList;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["GroupName"].Width = 120;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["SaleACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["SaleACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["PurchaseACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["PurchaseACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["DepositACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DepositACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

    }

    private void ultSupplier_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ultSupplier.SyncWithCurrencyManager = false;
      ultSupplier.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      //ACGroupName GroupName, ACSalePid SaleACCPid, ACDepositPid DepositACCPid, ACPurchasePid PurchaseACCPid
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["SaleACCPid"].Header.Caption = "Sale Account";
      e.Layout.Bands[0].Columns["PurchaseACCPid"].Header.Caption = "Purchase Account";
      e.Layout.Bands[0].Columns["DepositACCPid"].Header.Caption = "Deposit Account";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["SaleACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["PurchaseACCPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["DepositACCPid"].ValueList = ucbAccountList;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["GroupName"].MinWidth = 120;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["SaleACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["SaleACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["PurchaseACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["PurchaseACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["DepositACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DepositACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
    }
    #endregion InitializeLayout

    private void ultraProcess_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ultraProcess.SyncWithCurrencyManager = false;
      ultraProcess.StyleSetName = "Excel2013";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      // Set caption column
      e.Layout.Bands[0].Columns["ProcessPid"].Header.Caption = "Process";
      e.Layout.Bands[0].Columns["WorkshopPid"].Header.Caption = "Workshop";
      e.Layout.Bands[0].Columns["ACCostPid"].Header.Caption = "Cost Account";
      e.Layout.Bands[0].Columns["ACUnfinishPid"].Header.Caption = "Unfinish Account";
      e.Layout.Bands[0].Columns["IsForWO"].Header.Caption = "For Work Order";
      e.Layout.Bands[0].Columns["IsForMainMaterial"].Header.Caption = "For Main Material";


      // Set dropdownlist for column      
      e.Layout.Bands[0].Columns["ProcessPid"].ValueList = ucbProcessList;
      e.Layout.Bands[0].Columns["WorkshopPid"].ValueList = ucbWorkshopList;
      e.Layout.Bands[0].Columns["ACCostPid"].ValueList = ucbAccountList;
      e.Layout.Bands[0].Columns["ACUnfinishPid"].ValueList = ucbAccountList;

      // Set Checkbox
      e.Layout.Bands[0].Columns["IsForWO"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsForMainMaterial"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;


      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Width
      e.Layout.Bands[0].Columns["WorkshopPid"].Width = 120;

      // Set auto complete combo in grid
      //e.Layout.Bands[0].Columns["StockACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      //e.Layout.Bands[0].Columns["StockACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      //e.Layout.Bands[0].Columns["RevenueInternalACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      //e.Layout.Bands[0].Columns["RevenueInternalACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      //e.Layout.Bands[0].Columns["CostPriceACCPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      //e.Layout.Bands[0].Columns["CostPriceACCPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["ACCostPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ACCostPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["ACUnfinishPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ACUnfinishPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
    }

    #region Mouse Click Grid
    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Mouse Click Grids

    #region Duplicate
    private void CheckProcessCusDuplicate()
    {
      isCusDuplicateProcess = false;
      for (int k = 0; k < ultCustomer.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultCustomer.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultCustomer.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ultCustomer.Rows[i];
        string seriesNo = rowcurentA.Cells["GroupName"].Value.ToString().ToLower();
        if (seriesNo.Length > 0)
        {
          for (int j = i + 1; j < ultCustomer.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ultCustomer.Rows[j];

            string seriesNocom = rowcurentB.Cells["GroupName"].Value.ToString().ToLower();
            if (seriesNo == seriesNocom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isCusDuplicateProcess = true;
            }
          }
        }
      }
    }

    private void CheckProcessSupDuplicate()
    {
      isSupDuplicateProcess = false;
      for (int k = 0; k < ultSupplier.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultSupplier.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultSupplier.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ultSupplier.Rows[i];
        string seriesNo = rowcurentA.Cells["GroupName"].Value.ToString().ToLower();
        if (seriesNo.Length > 0)
        {
          for (int j = i + 1; j < ultSupplier.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ultSupplier.Rows[j];

            string seriesNocom = rowcurentB.Cells["GroupName"].Value.ToString().ToLower();
            if (seriesNo == seriesNocom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isSupDuplicateProcess = true;
            }
          }
        }
      }
    }

    private void CheckProcessProductDuplicate()
    {
      isProductDuplicateProcess = false;
      for (int k = 0; k < ultraProduct.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultraProduct.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultraProduct.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ultraProduct.Rows[i];
        string seriesNo = rowcurentA.Cells["GroupName"].Value.ToString().ToLower();
        if (seriesNo.Length > 0)
        {
          for (int j = i + 1; j < ultraProduct.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ultraProduct.Rows[j];

            string seriesNocom = rowcurentB.Cells["GroupName"].Value.ToString().ToLower();
            if (seriesNo == seriesNocom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isProductDuplicateProcess = true;
            }
          }
        }
      }
    }

    private void CheckProcessAssetDuplicate()
    {
      isAssetDuplicateProcess = false;
      for (int k = 0; k < ultraAsset.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultraAsset.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultraAsset.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ultraAsset.Rows[i];
        string seriesNo = rowcurentA.Cells["GroupName"].Value.ToString().ToLower();
        if (seriesNo.Length > 0)
        {
          for (int j = i + 1; j < ultraAsset.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ultraAsset.Rows[j];

            string seriesNocom = rowcurentB.Cells["GroupName"].Value.ToString().ToLower();
            if (seriesNo == seriesNocom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isAssetDuplicateProcess = true;
            }
          }
        }
      }
    }

    private void CheckProcessEquimentDuplicate()
    {
      isEquipmentDuplicateProcess = false;
      for (int k = 0; k < ultraEquipment.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultraEquipment.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultraEquipment.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ultraEquipment.Rows[i];
        string seriesNo = rowcurentA.Cells["GroupName"].Value.ToString().ToLower();
        if (seriesNo.Length > 0)
        {
          for (int j = i + 1; j < ultraEquipment.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ultraEquipment.Rows[j];

            string seriesNocom = rowcurentB.Cells["GroupName"].Value.ToString().ToLower();
            if (seriesNo == seriesNocom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isEquipmentDuplicateProcess = true;
            }
          }
        }
      }
    }

    private void CheckProcessLoanDuplicate()
    {
      isLoanDuplicateProcess = false;
      for (int k = 0; k < ultraLoanType.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultraLoanType.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultraLoanType.Rows.Count; i++)
      {

        UltraGridRow rowcurentA = ultraLoanType.Rows[i];
        string seriesNo = rowcurentA.Cells["LoanTypeName"].Value.ToString().ToLower();
        if (seriesNo.Length > 0)
        {
          for (int j = i + 1; j < ultraLoanType.Rows.Count; j++)
          {
            UltraGridRow rowcurentB = ultraLoanType.Rows[j];

            string seriesNocom = rowcurentB.Cells["LoanTypeName"].Value.ToString().ToLower();
            if (seriesNo == seriesNocom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isLoanDuplicateProcess = true;
            }
          }
        }
      }
    }
    #endregion Duplicate

    #region BeforeRowDelete

    private void ultraLoanType_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listLoanDeletedPid.Add(pid);
        }
      }
    }

    private void ultCustomer_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listCusDeletedPid.Add(pid);
        }
      }
    }

    private void ultSupplier_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listSupDeletedPid.Add(pid);
        }
      }
    }

    private void ultraProduct_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listProductDeletedPid.Add(pid);
        }
      }
    }

    private void ultraAsset_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listAssetDeletedPid.Add(pid);
        }
      }
    }

    private void ultraEquipment_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listEquipmentDeletedPid.Add(pid);
        }
      }
    }

    private void ultraProcess_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listProcessDeletedPid.Add(pid);
        }
      }
    }

    #endregion BeforeRowDelete

    #region AfterCellUpdate
    private void ultraLoanType_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("loantypename"))
      {
        try
        {
          this.CheckProcessLoanDuplicate();
        }
        catch { }

      }
    }

    private void ultraAsset_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("groupname"))
      {
        try
        {
          this.CheckProcessAssetDuplicate();
        }
        catch { }

      }
    }

    private void ultraProduct_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("groupname"))
      {
        try
        {
          this.CheckProcessProductDuplicate();
        }
        catch { }

      }
    }

    private void ultCustomer_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("groupname"))
      {
        try
        {
          this.CheckProcessCusDuplicate();
        }
        catch { }

      }
    }

    private void ultSupplier_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("groupname"))
      {
        try
        {
          this.CheckProcessSupDuplicate();
        }
        catch { }

      }
    }

    private void ultraEquipment_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName.Equals("groupname"))
      {
        try
        {
          this.CheckProcessEquimentDuplicate();
        }
        catch { }

      }
    }

    private void ultraProcess_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      //if (columnName.Equals("groupname"))
      //{
      //  try
      //  {
      //    this.CheckProcessProductDuplicate();
      //  }
      //  catch { }

      //}
    }

    #endregion AfterCellUpdate

    private void utcObjectConfig_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      iIndex = e.Tab.Index;
      InitTabData();
    }
  }
}