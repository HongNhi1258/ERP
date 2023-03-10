/*
  Author      : Đỗ Minh Tâm
  Date        : 16/11/2010
  Decription  : Custom price list
  Update By   : Nguyễn Văn Trọn
  Update date : 18/04/2012
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.UserControls;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_007 : MainUserControl
  {
    #region function
    private bool CheckInvalid()
    {
      if (txtCompany.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "Company.");
        return false;
      }

      if (txtTimeOfPriceList.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "Time Of Price List.");
        return false;
      }

      if (txtPriceListDes.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "Price List Description.");
        return false;
      }
      //if (txtMoreDes.Text.Trim().Length == 0)
      //{
      //  WindowUtinity.ShowMessageWarning("WRN0013", "More Description.");
      //  return false;
      //}
      //if (txtCusAddress.Text.Trim().Length == 0)
      //{
      //  WindowUtinity.ShowMessageWarning("WRN0013", "Customer Address.");
      //  return false;
      //}

      double dDiscount, dMakup, dExchangeRate;
      dDiscount = DBConvert.ParseDouble(txtDiscount.Text);
      if (dDiscount == double.MinValue)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "Discount");
        return false;
      }

      dMakup = DBConvert.ParseDouble(txtMarkup.Text);
      if (dMakup == double.MinValue)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "Markup");
        return false;
      }

      if (ucActive.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Select Item Of");
        return false;
      }

      if (cmbCurrencySign.SelectedIndex < 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "CurrencySign.");
        return false;
      }

      dExchangeRate = DBConvert.ParseDouble(txtExchangeRate.Text);
      if (dExchangeRate == double.MinValue)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "ExchangeRate");
        return false;
      }

      int decimalQty = DBConvert.ParseInt(txtDecimal.Text);
      if (decimalQty < 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "Decimal");
        return false;
      }
      if (!chkGetItemFromExcel.Checked)
      {
        if (ultraCBDefaultPrice.Value == null)
        {
          WindowUtinity.ShowMessageWarning("WRN0013", "Base Price");
          return false;
        }
      }
      return true;
    }

    private void ShowReport()
    {
      if (this.CheckInvalid())
      {
        #region master data
        // Continue or Discontinue Item
        int discontinue = (rdContinueItem.Checked ? 0 : 1);

        // Customer Active
        long iActiveType = long.MinValue;
        string strActiveType = string.Empty;
        if (ucActive.SelectedRow.Index >= 0)
        {
          iActiveType = DBConvert.ParseLong(ucActive.SelectedRow.Cells["Pid"].Value.ToString());
          if (iActiveType == 11)
          {
            strActiveType = "UK";
          }
          else if (iActiveType == 12)
          {
            strActiveType = "USA";
          }          
        }

        // Active kind (Active or Quick Ship)
        int iStatusItem = int.MinValue;
        string strStatusItem = string.Empty;
        if (rdActiveItem.Checked)
        {
          iStatusItem = 0;
          strStatusItem = "ACTIVE";
        }
        else
        {
          iStatusItem = 1;
          strStatusItem = "QUICK SHIP";
        }

        string strTitle = txtCompany.Text.Trim() + " " + txtPriceListDes.Text.Trim();
        string header = txtPriceListDes.Text.Trim();
        double dDiscount = DBConvert.ParseDouble(txtDiscount.Text);
        double dMakup = DBConvert.ParseDouble(txtMarkup.Text);
        double dExchangeRate = DBConvert.ParseDouble(txtExchangeRate.Text);
        int decimalQty = DBConvert.ParseInt(txtDecimal.Text);
        long basePrice = chkGetItemFromExcel.Checked ? long.MinValue : DBConvert.ParseLong(ultraCBDefaultPrice.Value.ToString());
        string strTime = txtTimeOfPriceList.Text.Trim();
        string viewType = string.Empty;
        DataTable dtItemPriceList = new DataTable();
        if (chkGetItemFromExcel.Checked)
        {
          int maxRows = int.MinValue;
          DataSet dsMaxRows = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtExcelFilePath.Text.Trim(), "SELECT * FROM [Data$G3:G4]");
          if (dsMaxRows != null && dsMaxRows.Tables.Count > 0)
          {
            maxRows = DBConvert.ParseInt(dsMaxRows.Tables[0].Rows[0][0].ToString());
          }
          if (maxRows <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Max Rows");
            return;
          }

          DataSet dsItemPriceList = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtExcelFilePath.Text.Trim(), string.Format("SELECT * FROM [Data$C5:D{0}]", 6 + maxRows));
          dtItemPriceList = (dsItemPriceList != null && dsItemPriceList.Tables.Count > 0) ? dsItemPriceList.Tables[0] : null;
        }
        #endregion master data
        #region Full Description (15 per page or 10 per page)
        if (rdFullDes.Checked || rdFullDes10perPage.Checked)
        {
          int iGroup = int.MinValue;
          string strGroup = string.Empty;
          if (rdCategory.Checked)
          {
            iGroup = 0;
            strGroup = rdCategory.Text;
          }
          else if (rdCollection.Checked)
          {
            iGroup = 1;
            strGroup = rdCollection.Text;
          }
          else
          {
            iGroup = 2;
            strGroup = rdCode.Text;
          }
          DataTable dtSource = new DataTable();
          string strFooter = string.Empty;
          if (chkGetItemFromExcel.Checked) //Get Item From Excel
          {
            SqlDBParameter[] inputParam = new SqlDBParameter[8];
            inputParam[0] = new SqlDBParameter("@ActiveType", SqlDbType.Int, iActiveType);
            inputParam[1] = new SqlDBParameter("@Group", SqlDbType.Int, iGroup);
            inputParam[2] = new SqlDBParameter("@ItemPriceList", SqlDbType.Structured, dtItemPriceList);
            inputParam[3] = new SqlDBParameter("@Discount", SqlDbType.Float, dDiscount);
            inputParam[4] = new SqlDBParameter("@Makup", SqlDbType.Float, dMakup);
            inputParam[5] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, dExchangeRate);
            inputParam[6] = new SqlDBParameter("@CurrencySign", SqlDbType.NVarChar, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[7] = new SqlDBParameter("@Decimal", SqlDbType.Int, decimalQty);

            SqlDBParameter[] outputParam = new SqlDBParameter[1];
            outputParam[0] = new SqlDBParameter("@Footer", SqlDbType.NVarChar, 4000, "");
            dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable("spMARPriceListFullDescription_InputItem", 36000, inputParam, outputParam);
            strFooter = outputParam[0].Value.ToString().Trim();
          }
          else
          {
            DBParameter[] inputParam = new DBParameter[10];
            inputParam[0] = new DBParameter("@ActiveType", DbType.Int32, iActiveType);
            inputParam[1] = new DBParameter("@Group", DbType.Int32, iGroup);
            inputParam[2] = new DBParameter("@StatusItem", DbType.Int32, iStatusItem);
            inputParam[3] = new DBParameter("@Discount", DbType.Double, dDiscount);
            inputParam[4] = new DBParameter("@Makup", DbType.Double, dMakup);
            inputParam[5] = new DBParameter("@ExchangeRate", DbType.Double, dExchangeRate);
            inputParam[6] = new DBParameter("@CurrencySign", DbType.String, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[7] = new DBParameter("@BasePrice", DbType.Int64, basePrice);
            inputParam[8] = new DBParameter("@Decimal", DbType.Int32, decimalQty);
            inputParam[9] = new DBParameter("@Discontinue", DbType.Int32, discontinue);

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Footer", DbType.String, 4000, "");
            dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spMARPriceListFullDescription_Customize", inputParam, outputParam);
            strFooter = outputParam[0].Value.ToString().Trim();
          }

          if (dtSource != null)
          {
            dtSource.Columns.Add("Image", System.Type.GetType("System.Byte[]"));
            dtSource.Columns.Add("CheckImage", System.Type.GetType("System.Int32"));
            dtSource.Columns.Add("ItemKindIcon", System.Type.GetType("System.Byte[]"));
            double percentBetweenWithAndHeight = (rdFullDes.Checked ? 0.946 : 1.224);
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
              dtSource.Rows[i]["Image"] = FunctionUtility.ImageToByteArrayWithFormat(@dtSource.Rows[i]["img"].ToString(), 380, percentBetweenWithAndHeight, "JPG");
              dtSource.Rows[i]["CheckImage"] = (dtSource.Rows[i]["Image"].ToString().Length > 0 ? 1 : 0);
              int itemKind = DBConvert.ParseInt(dtSource.Rows[i]["ItemKind"].ToString());
              dtSource.Rows[i]["ItemKindIcon"] = FunctionUtility.GetNewItemKindIcon(itemKind);
            }

            Shared.DataSetSource.Marketing.dsMARPriceList10Items dsSource = new Shared.DataSetSource.Marketing.dsMARPriceList10Items();
            dsSource.Tables["dtPriceList10Items"].Merge(dtSource);

            CrystalDecisions.CrystalReports.Engine.ReportClass cptFullDescriptionForItem = null;            
            if (rdFullDes.Checked)
            {
              cptFullDescriptionForItem = new Shared.ReportTemplate.Marketing.cptMARPriceList15Item();
              viewType = rdFullDes.Text;
            }
            else
            {
              cptFullDescriptionForItem = new Shared.ReportTemplate.Marketing.cptMARPriceList10Item();
              viewType = rdFullDes10perPage.Text;
            }
            cptFullDescriptionForItem.SetDataSource(dsSource);
            cptFullDescriptionForItem.SetParameterValue("Title", strTitle);
            cptFullDescriptionForItem.SetParameterValue("Header", header);
            cptFullDescriptionForItem.SetParameterValue("MoreDescription", txtMoreDes.Text.Trim());
            cptFullDescriptionForItem.SetParameterValue("TimeOfPrice", strTime);
            cptFullDescriptionForItem.SetParameterValue("CusAdd", txtCusAddress.Text.Trim());
            cptFullDescriptionForItem.SetParameterValue("Description", strFooter);
            cptFullDescriptionForItem.SetParameterValue("ViewType", viewType);
            ControlUtility.ViewCrystalReport(cptFullDescriptionForItem);
          }
          else
          {
            WindowUtinity.ShowMessageErrorFromText("Data Source is null. Please check!");
          }
        }
        #endregion Full Description (15 per page)
        #region Short Description
        else if (rdShortDes.Checked)
        {
          viewType = rdShortDes.Text;
          DataTable dtSource = new DataTable();
          if (chkGetItemFromExcel.Checked) //Get Item From Excel
          {
            SqlDBParameter[] inputParam = new SqlDBParameter[7];
            inputParam[0] = new SqlDBParameter("@ActiveType", SqlDbType.Int, iActiveType);
            inputParam[1] = new SqlDBParameter("@Discount", SqlDbType.Float, dDiscount);
            inputParam[2] = new SqlDBParameter("@Makup", SqlDbType.Float, dMakup);
            inputParam[3] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, dExchangeRate);
            inputParam[4] = new SqlDBParameter("@CurrencySign", SqlDbType.NVarChar, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[5] = new SqlDBParameter("@Decimal", SqlDbType.Int, decimalQty);
            inputParam[6] = new SqlDBParameter("@ItemPriceList", SqlDbType.Structured, dtItemPriceList);
            dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable("spCSDReportShortPriceList_InputItem", inputParam);
          }
          else
          {
            DBParameter[] inputParam = new DBParameter[9];
            inputParam[0] = new DBParameter("@ActiveType", DbType.Int64, iActiveType);
            inputParam[1] = new DBParameter("@StatusItem", DbType.Int32, iStatusItem);
            inputParam[2] = new DBParameter("@Discount", DbType.Double, dDiscount);
            inputParam[3] = new DBParameter("@Makup", DbType.Double, dMakup);
            inputParam[4] = new DBParameter("@ExchangeRate", DbType.Double, dExchangeRate);
            inputParam[5] = new DBParameter("@CurrencySign", DbType.String, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[6] = new DBParameter("@BasePrice", DbType.Int64, basePrice);
            inputParam[7] = new DBParameter("@Decimal", DbType.Int32, decimalQty);
            inputParam[8] = new DBParameter("@Discontinue", DbType.Int32, discontinue);
            dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDReportShortPriceList_Customize", inputParam);
          }
          if (dtSource != null)
          {
            dtSource.Columns.Add("ItemKindIcon", System.Type.GetType("System.Byte[]"));

            // Add Item Kind Icon
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
              int itemKind = DBConvert.ParseInt(dtSource.Rows[i]["ItemKind"].ToString());
              dtSource.Rows[i]["ItemKindIcon"] = FunctionUtility.GetNewItemKindIcon(itemKind);
            }

            DaiCo.CustomerService.DataSetSource.dsCSDFullDescriptionForItem dsSource = new DaiCo.CustomerService.DataSetSource.dsCSDFullDescriptionForItem();
            dsSource.Tables.Add(dtSource.Clone());
            dsSource.Tables["dtShortDesForItem"].Merge(dtSource);

            DaiCo.CustomerService.ReportTemplate.cptCSDPriceForItem cptPrice = new DaiCo.CustomerService.ReportTemplate.cptCSDPriceForItem();
            cptPrice.SetDataSource(dsSource);
            cptPrice.SetParameterValue("Title", strTitle);
            cptPrice.SetParameterValue("MoreDescription", txtMoreDes.Text.Trim());
            cptPrice.SetParameterValue("TimeOfPrice", strTime);
            cptPrice.SetParameterValue("CusAdd", txtCusAddress.Text.Trim());
            cptPrice.SetParameterValue("ViewType", viewType);
            ControlUtility.ViewCrystalReport(cptPrice);
          }
          else
          {
            WindowUtinity.ShowMessageErrorFromText("Data Source is null. Please check!");
          }
        }
        #endregion Short Description
        #region Short Description (15 per page)
        else if (rdShortDescNew.Checked)
        {
          viewType = rdShortDescNew.Text;
          int iGroup = int.MinValue;
          string strGroup = string.Empty;
          if (rdCategory.Checked)
          {
            iGroup = 0;
            strGroup = rdCategory.Text;
          }
          else if (rdCollection.Checked)
          {
            iGroup = 1;
            strGroup = rdCollection.Text;
          }
          else
          {
            iGroup = 2;
            strGroup = rdCode.Text;
          }
          DataTable dtSource = new DataTable();
          string strFooter = string.Empty;
          if (chkGetItemFromExcel.Checked) //Get Item From Excel
          {
            SqlDBParameter[] inputParam = new SqlDBParameter[8];
            inputParam[0] = new SqlDBParameter("@ActiveType", SqlDbType.Int, iActiveType);
            inputParam[1] = new SqlDBParameter("@Group", SqlDbType.Int, iGroup);
            inputParam[2] = new SqlDBParameter("@ItemPriceList", SqlDbType.Structured, dtItemPriceList);
            inputParam[3] = new SqlDBParameter("@Discount", SqlDbType.Float, dDiscount);
            inputParam[4] = new SqlDBParameter("@Makup", SqlDbType.Float, dMakup);
            inputParam[5] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, dExchangeRate);
            inputParam[6] = new SqlDBParameter("@CurrencySign", SqlDbType.NVarChar, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[7] = new SqlDBParameter("@Decimal", SqlDbType.Int, decimalQty);

            SqlDBParameter[] outputParam = new SqlDBParameter[1];
            outputParam[0] = new SqlDBParameter("@Footer", SqlDbType.NVarChar, 4000, "");
            dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable("spMARReportShortPriceList_InputItem", inputParam, outputParam);
            strFooter = outputParam[0].Value.ToString().Trim();
          }
          else
          {
            DBParameter[] inputParam = new DBParameter[10];
            inputParam[0] = new DBParameter("@ActiveType", DbType.Int32, iActiveType);
            inputParam[1] = new DBParameter("@Group", DbType.Int32, iGroup);
            inputParam[2] = new DBParameter("@StatusItem", DbType.Int32, iStatusItem);
            inputParam[3] = new DBParameter("@Discount", DbType.Double, dDiscount);
            inputParam[4] = new DBParameter("@Makup", DbType.Double, dMakup);
            inputParam[5] = new DBParameter("@ExchangeRate", DbType.Double, dExchangeRate);
            inputParam[6] = new DBParameter("@CurrencySign", DbType.String, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[7] = new DBParameter("@BasePrice", DbType.Int64, basePrice);
            inputParam[8] = new DBParameter("@Decimal", DbType.Int32, decimalQty);
            inputParam[9] = new DBParameter("@Discontinue", DbType.Int32, discontinue);

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Footer", DbType.String, 4000, "");
            dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spMARPriceListShortDesc_Customize", inputParam, outputParam);
            strFooter = outputParam[0].Value.ToString().Trim();
          }

          dtSource.Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dtSource.Columns.Add("CheckImage", System.Type.GetType("System.Int32"));
          dtSource.Columns.Add("ItemKindIcon", System.Type.GetType("System.Byte[]"));
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            dtSource.Rows[i]["Image"] = FunctionUtility.ImageToByteArrayWithFormat(@dtSource.Rows[i]["img"].ToString(), 380, 1, "JPG");
            dtSource.Rows[i]["CheckImage"] = (dtSource.Rows[i]["Image"].ToString().Length > 0 ? 1 : 0);
            int itemKind = DBConvert.ParseInt(dtSource.Rows[i]["ItemKind"].ToString());
            dtSource.Rows[i]["ItemKindIcon"] = FunctionUtility.GetNewItemKindIcon(itemKind);
          }
          Shared.DataSetSource.Marketing.dsMARPriceListShortDescription dsSource = new Shared.DataSetSource.Marketing.dsMARPriceListShortDescription();
          dsSource.Tables["dtPriceListShortDesc"].Merge(dtSource);

          CrystalDecisions.CrystalReports.Engine.ReportClass cptFullDescriptionForItem = null;
          cptFullDescriptionForItem = new Shared.ReportTemplate.Marketing.cptMARPriceListShortDescription();
          cptFullDescriptionForItem.SetDataSource(dsSource);
          cptFullDescriptionForItem.SetParameterValue("Title", strTitle);
          cptFullDescriptionForItem.SetParameterValue("MoreDescription", txtMoreDes.Text.Trim());
          cptFullDescriptionForItem.SetParameterValue("TimeOfPrice", strTime);
          cptFullDescriptionForItem.SetParameterValue("CusAdd", txtCusAddress.Text.Trim());
          cptFullDescriptionForItem.SetParameterValue("Description", strFooter);
          cptFullDescriptionForItem.SetParameterValue("ViewType", viewType);
          ControlUtility.ViewCrystalReport(cptFullDescriptionForItem);
        }
        else if (rdFullDes20.Checked )
        {
          DataTable dtSource = new DataTable();
          string strFooter = string.Empty;
          int iGroup = int.MinValue;
          string strGroup = string.Empty;
          if (rdCategory.Checked)
          {
            iGroup = 0;
            strGroup = rdCategory.Text;
          }
          else if (rdCollection.Checked)
          {
            iGroup = 1;
            strGroup = rdCollection.Text;
          }
          else
          {
            iGroup = 2;
            strGroup = rdCode.Text;
          }
          if (chkGetItemFromExcel.Checked) //Get Item From Excel
          {
            SqlDBParameter[] inputParam = new SqlDBParameter[8];
            inputParam[0] = new SqlDBParameter("@ActiveType", SqlDbType.Int, iActiveType);
            inputParam[1] = new SqlDBParameter("@Group", SqlDbType.Int, iGroup);
            inputParam[2] = new SqlDBParameter("@ItemPriceList", SqlDbType.Structured, dtItemPriceList);
            inputParam[3] = new SqlDBParameter("@Discount", SqlDbType.Float, dDiscount);
            inputParam[4] = new SqlDBParameter("@Makup", SqlDbType.Float, dMakup);
            inputParam[5] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, dExchangeRate);
            inputParam[6] = new SqlDBParameter("@CurrencySign", SqlDbType.NVarChar, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[7] = new SqlDBParameter("@Decimal", SqlDbType.Int, decimalQty);

            SqlDBParameter[] outputParam = new SqlDBParameter[1];
            outputParam[0] = new SqlDBParameter("@Footer", SqlDbType.NVarChar, 4000, "");
            dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable("spMARPriceListFullDescription_InputItem20", 36000, inputParam, outputParam);
            strFooter = outputParam[0].Value.ToString().Trim();
          }
          else
          {
            DBParameter[] inputParam = new DBParameter[10];
            inputParam[0] = new DBParameter("@ActiveType", DbType.Int32, iActiveType);
            inputParam[1] = new DBParameter("@Group", DbType.Int32, iGroup);
            inputParam[2] = new DBParameter("@StatusItem", DbType.Int32, iStatusItem);
            inputParam[3] = new DBParameter("@Discount", DbType.Double, dDiscount);
            inputParam[4] = new DBParameter("@Makup", DbType.Double, dMakup);
            inputParam[5] = new DBParameter("@ExchangeRate", DbType.Double, dExchangeRate);
            inputParam[6] = new DBParameter("@CurrencySign", DbType.String, 8, cmbCurrencySign.Text.Split('-')[0].ToString());
            inputParam[7] = new DBParameter("@BasePrice", DbType.Int64, basePrice);
            inputParam[8] = new DBParameter("@Decimal", DbType.Int32, decimalQty);
            inputParam[9] = new DBParameter("@Discontinue", DbType.Int32, discontinue);

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Footer", DbType.String, 4000, "");
            dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spMARPriceListFullDescription_CustomizeFor20", inputParam, outputParam);
            strFooter = outputParam[0].Value.ToString().Trim();
          }

          if (dtSource != null)
          {            
            foreach (DataRow dr in dtSource.Rows)
            {
              dr["Imags"] = FunctionUtility.ImageToByteArrayWithFormat(dr["img"].ToString(), 380, 1.1794, "JPG");
              int itemKind = DBConvert.ParseInt(dr["ItemKind"].ToString());
              dr["IconList"] = FunctionUtility.GetNewItemKindIcon(itemKind);
            }            

            CrystalDecisions.CrystalReports.Engine.ReportClass cptFullDescriptionForItem = null;                        
            cptFullDescriptionForItem = new Shared.ReportTemplate.Marketing.cptUKRetailPriceListSaleCode();
            viewType = rdFullDes20.Text;

            cptFullDescriptionForItem.SetDataSource(dtSource);
            cptFullDescriptionForItem.SetParameterValue("Title", strTitle);
            cptFullDescriptionForItem.SetParameterValue("Publised", strTime);
            cptFullDescriptionForItem.SetParameterValue("MoreDescription", txtMoreDes.Text.Trim());
            cptFullDescriptionForItem.SetParameterValue("TimeOfPrice", strTime);
            cptFullDescriptionForItem.SetParameterValue("CusAdd", txtCusAddress.Text.Trim());
            cptFullDescriptionForItem.SetParameterValue("Description", strFooter);
            cptFullDescriptionForItem.SetParameterValue("ViewType", viewType);
            ControlUtility.ViewCrystalReport(cptFullDescriptionForItem);            
          }
          else
          {
            WindowUtinity.ShowMessageErrorFromText("Data Source is null. Please check!");
          }
        }
        #endregion Short Description (15 per page)
      }
    }

    private void LoadData()
    {
      //rdNone.Checked = true;
      rdFullDes.Checked = true;
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbCurrencySign, 2007);
      cmbCurrencySign.SelectedIndex = 1;
      string strTime = DataBaseAccess.ExecuteScalarCommandText("SELECT TOP 1 Value FROM TblBOMCodeMaster WHERE [Group] = 2011 AND DeleteFlag = 0").ToString();
      txtTimeOfPriceList.Text = strTime;
      txtExchangeRate.Text = "1";
      txtDiscount.Text = "0";
      txtMarkup.Text = "1";

      string strsql = "	SELECT CSI.Pid, CSI.CustomerCode, CSI.Name, CSI.CustomerCode + ' - ' + CSI.Name DisplayText"
                    + " FROM (SELECT DISTINCT CUSTOMERPID FROM TblCSDItemActiveInfomation WHERE CUSTOMERPID = 11 OR CUSTOMERPID = 12) ACT "
                    + " 	INNER JOIN TblCSDCustomerInfo CSI ON ACT.CustomerPid = CSI.Pid";
      DataTable dtCus = DataBaseAccess.SearchCommandTextDataTable(strsql, null);
      DataRow row = dtCus.NewRow();
      row["Pid"] = 0;
      row["CustomerCode"] = "Other";
      row["Name"] = "Other Items";
      row["DisplayText"] = "Other - Other Items";
      dtCus.Rows.Add(row);
      ucActive.DataSource = dtCus;
      ucActive.ValueMember = "Pid";
      ucActive.DisplayMember = "DisplayText";
      ucActive.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ucActive.DisplayLayout.Bands[0].Columns["DisplayText"].Hidden = true;
      ucActive.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ucActive.DisplayLayout.Bands[0].Columns["CustomerCode"].Width = 50;

      strsql = "Select Pid, CustomerCode, Name, CustomerCode + ' - ' + Name as DisplayText from TblCSDCustomerInfo where (Kind = 4 OR Pid = 1) Order By CustomerCode ASC";
      DataTable dtCus1 = DataBaseAccess.SearchCommandTextDataTable(strsql, null);
      ultraCBDefaultPrice.DataSource = dtCus1;
      ultraCBDefaultPrice.ValueMember = "Pid";
      ultraCBDefaultPrice.DisplayMember = "DisplayText";
      ultraCBDefaultPrice.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraCBDefaultPrice.DisplayLayout.Bands[0].Columns["DisplayText"].Hidden = true;
      ultraCBDefaultPrice.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBDefaultPrice.DisplayLayout.Bands[0].Columns["CustomerCode"].MinWidth = 60;
      ultraCBDefaultPrice.DisplayLayout.Bands[0].Columns["CustomerCode"].MaxWidth = 60;
      rdActiveItem.Checked = true;

      try
      {
        ucActive.Value = 12;
      }
      catch { }
    }
    #endregion function

    public viewCSD_04_007()
    {
      InitializeComponent();
    }

    private void rdPriceAndBarcode_CheckedChanged(object sender, EventArgs e)
    {
      //if (rdPriceAndBarcode.Checked)
      //{
      //  rdFullDes.Checked = true;
      //  grdDesType.Enabled = false;
      //}
    }

    private void rdNone_CheckedChanged(object sender, EventArgs e)
    {
      //if (rdNone.Checked)
      //{
      //  rdFullDes.Checked = true;
      //  grdDesType.Enabled = true;
      //}
    }

    private void viewCSD_04_007_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnPreview_Click(object sender, EventArgs e)
    {
      tableLayoutPrintReport.Enabled = false;
      this.ShowReport();
      tableLayoutPrintReport.Enabled = true;
    }

    private void rdShortDes_CheckedChanged(object sender, EventArgs e)
    {
      if (rdShortDes.Checked || rdShortDescNew.Checked)
      {
        groupBoxViewOption.Enabled = false;
        rdCode.Checked = true;
      }
    }

    private void rdFullDes_CheckedChanged(object sender, EventArgs e)
    {
      if (rdFullDes.Checked)
      {
        groupBoxViewOption.Enabled = true;
        rdCode.Checked = true;
      }
    }

    private void ucActive_ValueChanged(object sender, EventArgs e)
    {
      if (ucActive.Value != null)
      {
        if (DBConvert.ParseInt(ucActive.Value.ToString()) == 0)
        {
          groupBoxStatusItem.Enabled = false;
          return;
        }
      }
      groupBoxStatusItem.Enabled = true;
    }

    private void chkGetItemFromExcel_CheckedChanged(object sender, EventArgs e)
    {
      if (chkGetItemFromExcel.Checked)
      {
        tableLayoutExcelPath.Enabled = true;
        tableLayoutStatusItem.Enabled = false;
        ultraCBDefaultPrice.Enabled = false;
        lbBasePrice.Visible = false;
      }
      else
      {
        tableLayoutExcelPath.Enabled = false;
        tableLayoutStatusItem.Enabled = true;
        ultraCBDefaultPrice.Enabled = true;
        lbBasePrice.Visible = true;
      }
    }

    private void btnGetTempale_Click(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      string templateName = string.Format(@"{0}\ExcelTemplate\CustomerService\CSDItemForPrintPriceList.xls", startupPath);
      if (File.Exists(templateName))
      {
        string newFileName = string.Format(@"{0}\ItemForPrintPriceList.xls", folder);
        if (File.Exists(newFileName))
        {
          newFileName = string.Format(@"{0}\ItemForPrintPriceList{1}.xls", folder, DateTime.Now.Ticks);
        }
        File.Copy(templateName, newFileName);
        Process.Start(newFileName);
      }
      //// Delete all file in folder Report
      //foreach (string file in Directory.GetFiles(folder))
      //{
      //  try
      //  {
      //    File.Delete(file);
      //  }
      //  catch { }
      //}
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtExcelFilePath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;      
    }
  }
}
