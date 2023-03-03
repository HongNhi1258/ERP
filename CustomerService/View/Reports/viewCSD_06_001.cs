/*
  Author      : Nguyen Van Tron
  Date        : 26/12/2011
  Description : Report for Customer Service
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using DaiCo.Shared.Utility;
using VBReport;
using System.Diagnostics;
using DaiCo.Shared.DataBaseUtility;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_06_001 : MainUserControl
  {
    #region fields
    private int totalColumnsQty = 0;
    private int columnsQty = 0;
    // Format Date User
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    string columnName = string.Empty;
    string columnValue = string.Empty;
    #endregion fields

    #region function
    /// <summary>
    /// Load Type Report
    /// </summary>
    private void LoadReportSoure()
    {
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("Value", typeof(System.Int32));
      dtSource.Columns.Add("Display", typeof(System.String));

      DataRow row1 = dtSource.NewRow();
      row1["Value"] = 1;
      row1["Display"] = "Get Picture Report for Multi-purposes";
      dtSource.Rows.Add(row1);

      DataRow row2 = dtSource.NewRow();
      row2["Value"] = 2;
      row2["Display"] = "Get List Hardware";
      dtSource.Rows.Add(row2);

      ultraCBReports.DataSource = dtSource;
      ultraCBReports.ValueMember = "Value";
      ultraCBReports.DisplayMember = "Display";      
      ultraCBReports.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBReports.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }
    /// <summary>
    /// Input by
    /// </summary>
    private void LoadSearchBySoure()
    {
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("Value", typeof(System.Int32));
      dtSource.Columns.Add("Display", typeof(System.String));

      DataRow row1 = dtSource.NewRow();
      row1["Value"] = 1;
      row1["Display"] = "Sale Code";
      dtSource.Rows.Add(row1);

      DataRow row2 = dtSource.NewRow();
      row2["Value"] = 2;
      row2["Display"] = "Item Code";
      dtSource.Rows.Add(row2);
      
      DataRow row3 = dtSource.NewRow();
      row3["Value"] = 3;
      row3["Display"] = "Component Code";
      dtSource.Rows.Add(row3);

      ultraCBSearchBy.DataSource = dtSource;
      ultraCBSearchBy.ValueMember = "Value";
      ultraCBSearchBy.DisplayMember = "Display";      
      ultraCBSearchBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBSearchBy.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }

    private void LoadUltraCBCustomer()
    {
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
      ultraCBCustomer.DataSource = dtCus;
      ultraCBCustomer.ValueMember = "Pid";
      ultraCBCustomer.DisplayMember = "DisplayText";
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["DisplayText"].Hidden = true;
      ultraCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["CustomerCode"].Width = 50;
      try
      {
        ultraCBCustomer.Value = 12;
      }
      catch { }
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      if (ultraCBReports.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return false;
      }
      if (DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 1)
      {
        if (ultraCBSearchBy.Value == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Search By");
          return false;
        }
        if (ultraCBCustomer.Value == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Customer");
          return false;
        }
        if (DBConvert.ParseInt(txtColumnsQty.Text) <= 0 || DBConvert.ParseInt(txtColumnsQty.Text) > 9)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Columns Qty");
          return false;
        }
        if (txtFilePath.Text.Trim().Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "File Path");
          return false;
        }
      }
      else if(DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 2)
      {
        if(ultcbHardWareFrom.Value == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "HardWare From");
          return false;
        }
        if(ultcbHardWareTo.Value == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "HardWare To");
          return false;
        }
      }
      return true;
    }
    /// <summary>
    /// Set Column Landscape
    /// </summary>
    /// <param name="cpt"></param>
    private void SetTotalColumnLandscape(Shared.ReportTemplate.CustomerService.cptCSDGetPicture_Landscape cpt)
    {
      if (columnsQty == 1)
      {
        // Header1
        cpt.Header1.SectionFormat.EnableSuppress = false;

        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail1
        cpt.Detail1.SectionFormat.EnableSuppress = false;

        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer1
        cpt.Footer1.SectionFormat.EnableSuppress = false;

        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 2)
      {
        // Header2
        cpt.Header2.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail2
        cpt.Detail2.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer2
        cpt.Footer2.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 3)
      {
        // Header3
        cpt.Header3.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail3
        cpt.Detail3.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer3
        cpt.Footer3.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 4)
      {
        // Header4
        cpt.Header4.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail4
        cpt.Detail4.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer4
        cpt.Footer4.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 5)
      {
        // Header5
        cpt.Header5.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail5
        cpt.Detail5.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer5
        cpt.Footer5.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if(columnsQty == 6)
      {
        // Header6
        cpt.Header6.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail6
        cpt.Detail6.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer6
        cpt.Footer6.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if(columnsQty == 7)
      {
        // Header7
        cpt.Header7.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail7
        cpt.Detail7.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer7
        cpt.Footer7.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if(columnsQty == 8)
      {
        // Header8
        cpt.Header8.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header9.SectionFormat.EnableSuppress = true;

        // Detail8
        cpt.Detail8.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail9.SectionFormat.EnableSuppress = true;

        // Footer8
        cpt.Footer8.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer9.SectionFormat.EnableSuppress = true;
      }
      else if(columnsQty == 9)
      {
        // Header9
        cpt.Header9.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;
        cpt.Header6.SectionFormat.EnableSuppress = true;
        cpt.Header7.SectionFormat.EnableSuppress = true;
        cpt.Header8.SectionFormat.EnableSuppress = true;

        // Detail9
        cpt.Detail9.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;
        cpt.Detail6.SectionFormat.EnableSuppress = true;
        cpt.Detail7.SectionFormat.EnableSuppress = true;
        cpt.Detail8.SectionFormat.EnableSuppress = true;

        // Footer9
        cpt.Footer9.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
        cpt.Footer6.SectionFormat.EnableSuppress = true;
        cpt.Footer7.SectionFormat.EnableSuppress = true;
        cpt.Footer8.SectionFormat.EnableSuppress = true;
      }
    }
    /// <summary>
    /// Set Column Portrail
    /// </summary>
    /// <param name="cpt"></param>
    private void SetTotalColumn(Shared.ReportTemplate.CustomerService.cptCUSGetPicture cpt)
    {
      if (columnsQty == 1)
      {
        // Header1
        cpt.Header1.SectionFormat.EnableSuppress = false;

        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;

        // Detail1
        cpt.Detail1.SectionFormat.EnableSuppress = false;

        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;

        // Footer1
        cpt.Footer1.SectionFormat.EnableSuppress = false;

        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 2)
      {
        // Header2
        cpt.Header2.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;

        // Detail2
        cpt.Detail2.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;

        // Footer2
        cpt.Footer2.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 3)
      {
        // Header3
        cpt.Header3.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;

        // Detail3
        cpt.Detail3.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;

        // Footer3
        cpt.Footer3.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 4)
      {
        // Header4
        cpt.Header4.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header5.SectionFormat.EnableSuppress = true;

        // Detail4
        cpt.Detail4.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail5.SectionFormat.EnableSuppress = true;

        // Footer4
        cpt.Footer4.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer5.SectionFormat.EnableSuppress = true;
      }
      else if (columnsQty == 5)
      {
        // Header5
        cpt.Header5.SectionFormat.EnableSuppress = false;

        cpt.Header1.SectionFormat.EnableSuppress = true;
        cpt.Header2.SectionFormat.EnableSuppress = true;
        cpt.Header3.SectionFormat.EnableSuppress = true;
        cpt.Header4.SectionFormat.EnableSuppress = true;

        // Detail5
        cpt.Detail5.SectionFormat.EnableSuppress = false;

        cpt.Detail1.SectionFormat.EnableSuppress = true;
        cpt.Detail2.SectionFormat.EnableSuppress = true;
        cpt.Detail3.SectionFormat.EnableSuppress = true;
        cpt.Detail4.SectionFormat.EnableSuppress = true;

        // Footer5
        cpt.Footer5.SectionFormat.EnableSuppress = false;

        cpt.Footer1.SectionFormat.EnableSuppress = true;
        cpt.Footer2.SectionFormat.EnableSuppress = true;
        cpt.Footer3.SectionFormat.EnableSuppress = true;
        cpt.Footer4.SectionFormat.EnableSuppress = true;
      }
    }
    /// <summary>
    /// Load Data Report Hardware
    /// </summary>
    private void LoadHardWareList()
    {
      string commandText = "SELECT DISTINCT Code, Code + ' - ' + Name AS Display FROM VBOMComponent WHERE CompGroup = 1";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if(dtSource != null && dtSource.Rows.Count > 0)
      {
        // HardWare From
        ultcbHardWareFrom.DataSource = dtSource;
        ultcbHardWareFrom.ValueMember = "Code";
        ultcbHardWareFrom.DisplayMember = "Display";

        ultcbHardWareFrom.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultcbHardWareFrom.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;

        // HardWare To
        ultcbHardWareTo.DataSource = dtSource;
        ultcbHardWareTo.ValueMember = "Code";
        ultcbHardWareTo.DisplayMember = "Display";

        ultcbHardWareTo.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultcbHardWareTo.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      }
    }

    /// <summary>
    /// Get Report Picture
    /// </summary>
    private void GetPictureReport()
    {
      Shared.DataSetSource.CustomerService.dsCUSGetPicture dsSource = new Shared.DataSetSource.CustomerService.dsCUSGetPicture();

      if (radLandscape.Checked || columnsQty > 5)
      {
        columnName = "dtColumnName_Landscape";
        columnValue = "dtColumnValue_Landscape";
      }
      else
      {
        columnName = "dtColumnName";
        columnValue = "dtColumnValue";
      }
      // 1. Header
      DataTable dtTittle = dsSource.Tables["dtTittle"];
      DataRow row = dtTittle.NewRow();

      DataSet dsTittleExcel = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), "SELECT * FROM [PictureReport (1)$B1:B3]");
      if (dsTittleExcel != null && dsTittleExcel.Tables.Count > 0)
      {
        DataTable dtTittleExcel = dsTittleExcel.Tables[0];
        // Title
        row["Tittle"] = txtTittle.Text;
        if (rdJCLogo.Checked)
        {
          row["LogoImage"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.Logo);
        }
        else
        {
          row["LogoImage"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFR);
        }
        if (dtTittleExcel != null && dtTittleExcel.Rows.Count > 1)
        {
          // Report by
          row["ReportedBy"] = Shared.Utility.SharedObject.UserInfo.EmpName;
          //row["ReportedBy"] = dtTittleExcel.Rows[0][0];
          // Report date
          row["ReportedDate"] = dtTittleExcel.Rows[1][0];
        }
      }
      // Description
      DataSet dsDescriptionExcel = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), "SELECT * FROM [PictureReport (1)$N1:N21]");
      if (dsDescriptionExcel != null && dsDescriptionExcel.Tables.Count > 0)
      {
        DataTable dtDescriptionExcel = dsDescriptionExcel.Tables[0];
        if (dtDescriptionExcel != null && dtDescriptionExcel.Rows.Count > 0)
        {
          foreach (DataRow rowDescription in dtDescriptionExcel.Rows)
          {
            if (rowDescription["F1"].ToString().Length > 0)
            {
              row["Description"] += "<br/>" + rowDescription["F1"].ToString().Replace(" ", "&nbsp;");
            }
          }
        }
      }
      dtTittle.Rows.Add(row);

      // 2. Columns Tittle
      long number = 65 + DBConvert.ParseInt(txtColumnsQty.Text) + 1;
      string colName = System.Convert.ToChar(number).ToString();
      DataTable dtColumnName = dsSource.Tables[columnName];
      DataRow rowColumn = dtColumnName.NewRow();

      DataSet dsColumnNameExcel = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), String.Format(@"SELECT * FROM [PictureReport (1)$A4:{0}5]", colName));
      if (dsColumnNameExcel != null && dsColumnNameExcel.Tables.Count > 0)
      {
        DataTable dtColumnNameExcel = dsColumnNameExcel.Tables[0];
        if (dtColumnNameExcel != null && dtColumnNameExcel.Rows.Count > 0)
        {
          for (int i = 1; i < DBConvert.ParseInt(txtColumnsQty.Text) + 1; i++)
          {
            rowColumn[string.Format(@"Column{0}", i)] = dtColumnNameExcel.Rows[0][i];
          }
          rowColumn["Remark"] = dtColumnNameExcel.Rows[0][DBConvert.ParseInt(txtColumnsQty.Text) + 1];
        }
      }
      dtColumnName.Rows.Add(rowColumn);

      // 3. Detail
      // Format Date & Max Rows Count
      int maxRows = int.MinValue;
      string formatDate = "dd/MM/yy";
      DataSet dsMaxRows = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), string.Format(@"SELECT * FROM [PictureReport (1)$F1:F3]"));
      if (dsMaxRows != null && dsMaxRows.Tables.Count > 0)
      {
        DataTable dtMaxRows = dsMaxRows.Tables[0];
        if (dtMaxRows != null && dtMaxRows.Rows.Count > 1)
        {
          if (dtMaxRows.Rows[0][0].ToString().Trim().Length > 0)
          {
            formatDate = dtMaxRows.Rows[0][0].ToString().Trim();
          }
          if (DBConvert.ParseInt(dtMaxRows.Rows[1][0].ToString()) != int.MinValue)
          {
            maxRows = DBConvert.ParseInt(dtMaxRows.Rows[1][0].ToString()) + 5;
          }
          else
          {
            btnPrint.Enabled = true;
            WindowUtinity.ShowMessageError("ERR0001", "MaxRows in Excel");
            return;
          }
        }
      }
      // Reading Info From File Excel
      DataSet dsInfoDetailExcel = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), String.Format(@"SELECT * FROM [PictureReport (1)$A5:{0}{1}]", colName, maxRows));
      if (dsInfoDetailExcel != null && dsInfoDetailExcel.Tables.Count > 0)
      {
        DataTable dtInfoDetailExcel = dsInfoDetailExcel.Tables[0];
        if (dtInfoDetailExcel != null && dtInfoDetailExcel.Rows.Count > 0)
        {
          DataTable dtColumnValue = dsSource.Tables[columnValue];
          for (int j = 0; j < dtInfoDetailExcel.Rows.Count; j++)
          {
            if (dtInfoDetailExcel.Rows[j][0].ToString().Length > 0)
            {
              DataRow rowInfoDetail = dtColumnValue.NewRow();
              int searchBy = DBConvert.ParseInt(ultraCBSearchBy.Value.ToString());
              DBParameter[] input = new DBParameter[3];
              input[0] = new DBParameter("@SearchBy", DbType.Int32, searchBy);
              input[1] = new DBParameter("@Value", DbType.AnsiString, 16, dtInfoDetailExcel.Rows[j][0].ToString());
              input[2] = new DBParameter("@CustomerPid", DbType.Int64, ultraCBCustomer.Value);
              DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMRPTPictureReport", input);
              if (dtSource != null)
              {
                if (dtSource.Rows.Count > 0)
                {
                  rowInfoDetail["SaleCodeValue"] = dtSource.Rows[0]["SaleCodeValue"].ToString();
                  rowInfoDetail["ItemCodeValue"] = dtSource.Rows[0]["ItemCodeValue"].ToString();
                  rowInfoDetail["ShortNameValue"] = dtSource.Rows[0]["ShortNameValue"].ToString();                  
                  // Get Picture
                  try
                  {
                    string imgPath = string.Empty;
                    if (searchBy == 3)
                    {
                      imgPath = FunctionUtility.BOMGetItemComponentImage(dtSource.Rows[0]["SaleCodeValue"].ToString());
                      rowInfoDetail["PictureValue"] = FunctionUtility.ImagePathToByteArray(imgPath);
                    }
                    else
                    {
                      rowInfoDetail["CBMValue"] = DBConvert.ParseDouble(dtSource.Rows[0]["CBMValue"].ToString());
                      imgPath = FunctionUtility.BOMGetItemImage(dtSource.Rows[0]["ItemCodeValue"].ToString(), DBConvert.ParseInt(dtSource.Rows[0]["Revision"].ToString()));
                      rowInfoDetail["PictureValue"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.81, "JPG");
                    }
                    
                  }
                  catch { }
                  int status = DBConvert.ParseInt(dtSource.Rows[0]["Status"].ToString());
                  rowInfoDetail["StatusIcon"] = FunctionUtility.GetLocalItemKindIcon(status, DBConvert.ParseLong(ultraCBCustomer.Value.ToString()));                  
                }
                else
                {
                  if (searchBy == 1)
                  {
                    rowInfoDetail["SaleCodeValue"] = dtInfoDetailExcel.Rows[j][0].ToString();
                  }
                  else
                  {
                    rowInfoDetail["ItemCodeValue"] = dtInfoDetailExcel.Rows[j][0].ToString();
                  }
                }
                // Column Value
                for (int k = 1; k < DBConvert.ParseInt(txtColumnsQty.Text) + 1; k++)
                {
                  string colValue = dtInfoDetailExcel.Rows[j][k].ToString();
                  if (DBConvert.ParseDouble(colValue) == double.MinValue && DBConvert.ParseDateTime(colValue, formatConvert) != DateTime.MinValue)
                  {
                    colValue = DBConvert.ParseString(DBConvert.ParseDateTime(colValue, formatConvert), formatDate);
                  }
                  rowInfoDetail[string.Format(@"Col{0}Value", k)] = colValue;
                }
                rowInfoDetail["RemarkValue"] = dtInfoDetailExcel.Rows[j][DBConvert.ParseInt(txtColumnsQty.Text) + 1];
              }
              // Add Datatable
              dtColumnValue.Rows.Add(rowInfoDetail);
            }
          }
        }
      }
      // Total Column
      for (int column = 1; column < DBConvert.ParseInt(txtColumnsQty.Text) + 1; column++)
      {
        // Check Total Column
        if (chkTotalColumn.Checked)
        {
          bool flag = false;
          double total = 0;
          foreach (DataRow rowcol in dsSource.Tables[columnValue].Rows)
          {
            if (rowcol[1].ToString().Length > 0 && rowcol[string.Format(@"Col{0}Value", column)].ToString().Length > 0)
            {
              if (DBConvert.ParseDouble(rowcol[string.Format(@"Col{0}Value", column)].ToString()) != Double.MinValue)
              {
                total += DBConvert.ParseDouble(rowcol[string.Format(@"Col{0}Value", column)].ToString());
              }
              else
              {
                flag = true;
                break;
              }
            }
          }
          if (flag == false && total > 0)
          {
            rowColumn[string.Format(@"ToalCol{0}", column)] = total;
          }
        }
        else
        {
          DataSet dsTotalColumn = FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), String.Format(@"SELECT * FROM [PictureReport (1)$B3:{0}4]", colName));   
          if (dsTotalColumn != null && dsTotalColumn.Tables.Count > 0)
          {
            DataTable dtTotalColumn = dsTotalColumn.Tables[0];
            if (dtTotalColumn.Rows[0][column - 1].ToString().Length > 0 && DBConvert.ParseDouble(dtTotalColumn.Rows[0][column - 1].ToString()) != double.MinValue)
            {
              rowColumn[string.Format(@"ToalCol{0}", column)] = dtTotalColumn.Rows[0][column - 1];
            }
            else
            {
              rowColumn[string.Format(@"ToalCol{0}", column)] = DBNull.Value;
            }
          }
        }
      }
      this.SetPrintReport(dsSource);
    }
    /// <summary>
    /// Set Type Print Report
    /// </summary>
    /// <param name="dsSource"></param>
    private void SetPrintReport(DataSet dsSource)
    {
      if (radLandscape.Checked || columnsQty > 5)
      {
        Shared.ReportTemplate.CustomerService.cptCSDGetPicture_Landscape cpt = new DaiCo.Shared.ReportTemplate.CustomerService.cptCSDGetPicture_Landscape();
        cpt.SetDataSource(dsSource);
        // Set Type Reprt Landscape
        this.SetTotalColumnLandscape(cpt);
        ControlUtility.ViewCrystalReport(cpt);
      }
      else
      {
        Shared.ReportTemplate.CustomerService.cptCUSGetPicture cpt = new DaiCo.Shared.ReportTemplate.CustomerService.cptCUSGetPicture();
        cpt.SetDataSource(dsSource);
        // Set Type Report Portrait
        this.SetTotalColumn(cpt);
        ControlUtility.ViewCrystalReport(cpt);
      }
    }
    /// <summary>
    /// Report HareWare
    /// </summary>
    private void GetHardWareListReport()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@HareWareFrom", DbType.AnsiString, 16, ultcbHardWareFrom.Value.ToString());
      inputParam[1] = new DBParameter("@HareWareTo", DbType.AnsiString, 16, ultcbHardWareTo.Value.ToString());

      DataTable dtListHareWare = DataBaseAccess.SearchStoreProcedureDataTable("spCSDRPTGetListHardWare", inputParam);
      Shared.DataSetSource.CustomerService.dsCSDGetListHardWare dsSource = new Shared.DataSetSource.CustomerService.dsCSDGetListHardWare();
      if(dtListHareWare != null)
      {
        DataTable dtSource = dsSource.Tables["dtListHardWare"];
        for (int i = 0; i < dtListHareWare.Rows.Count; i++)
        {
          DataRow drSource = dtSource.NewRow();
          drSource["Code"] = dtListHareWare.Rows[i]["Code"].ToString();
          drSource["Name"] = dtListHareWare.Rows[i]["Name"].ToString();
          drSource["SaleCode"] = dtListHareWare.Rows[i]["SaleCode"].ToString();
          try
          {
            string imgPath = FunctionUtility.BOMGetItemComponentImage(drSource["Code"].ToString());
            FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            byte[] imgbyte = new byte[fs.Length + 1];
            imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
            drSource["Picture"] = imgbyte;
            br.Close();
            fs.Close();
          }
          catch { }
          dtSource.Rows.Add(drSource);
        }
      }
      Shared.ReportTemplate.CustomerService.cptCSDGetListHardWare cpt = new DaiCo.Shared.ReportTemplate.CustomerService.cptCSDGetListHardWare();
      cpt.SetDataSource(dsSource);
      cpt.SetParameterValue("Reportby", SharedObject.UserInfo.EmpName.ToString());
      cpt.SetParameterValue("Reportdate", DBConvert.ParseString(DateTime.Now));

      // Export Report
      SaveFileDialog f = new SaveFileDialog();
      f.Filter = "PDF file (*.rpt)|*.rpt";
      if (f.ShowDialog() == DialogResult.OK)
      {
        string strName = f.FileName;
        try
        {
          File.Open(f.FileName, FileMode.OpenOrCreate).Close();
        }
        catch
        {
          MessageBox.Show("Already Opened:" + f.FileName + "\nPlease find and close it", "Can not save!");
        }

        cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.CrystalReport, strName);
        Process.Start(@strName);
      }
    }
    #endregion function

    #region event
    public viewCSD_06_001()
    {
      InitializeComponent();
    }

    private void viewCSD_06_001_Load(object sender, EventArgs e)
    {
      this.LoadReportSoure();
      ultraCBReports.Value = 1;
      if(DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 1)
      {
        this.LoadSearchBySoure();
        this.LoadUltraCBCustomer();
      }
      else if (DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 2)
      {
        this.LoadHardWareList();
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      // Check Invalid
      int columnsQty = DBConvert.ParseInt(txtColumnsQty.Text);
      if (columnsQty <= 0 || columnsQty > 10)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Columns Qty");
        btnGetTemplate.Enabled = false;
        return;
      }
      this.totalColumnsQty = columnsQty;      
      DataTable dtSource = new DataTable();
      for (int i = 0; i < columnsQty; i++)
      {
        dtSource.Columns.Add(string.Format("Col{0}-Tittle", i + 1));        
      }      
      dtSource.Rows.Add(dtSource.NewRow());

      ultraGridColumnsTittle.DataSource = dtSource;      
      if (ultraCBSearchBy.Value != null)
      {
        btnGetTemplate.Enabled = true;
      }
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "CSDGetPictureReport";
      string sheetName = "PictureReport";
      string outFileName = "Picture Report Template";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      //string searchBy = (DBConvert.ParseInt(ultraCBSearchBy.Value.ToString()) == 1 ? "Sale Code" : "Item Code");
      string searchBy = ultraCBSearchBy.SelectedRow.Cells["Display"].Value.ToString();
      oXlsReport.Cell("**SearchBy").Value = searchBy;
      DataRow row = ((DataTable)ultraGridColumnsTittle.DataSource).Rows[0];
      for (int i = 0; i < this.totalColumnsQty; i++)
      {
        oXlsReport.Cell(string.Format("**Col{0}-Tittle", i + 1)).Value = row[string.Format("Col{0}-Tittle", i + 1)];
      }
      oXlsReport.Cell(string.Format("**Col{0}-Tittle", this.totalColumnsQty + 1)).Value = "Remark";
      for (int k = this.totalColumnsQty + 1; k < 5; k++)
      {
        oXlsReport.Cell(string.Format("**Col{0}-Tittle", k + 1)).Value = null;
      }
      oXlsReport.Cell("**ReportedBy").Value = Shared.Utility.SharedObject.UserInfo.EmpName;
      oXlsReport.Cell("**ReportedDate").Value = DBConvert.ParseString(DateTime.Now, "MM/dd/yyyy");
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void ultraCBSearchBy_ValueChanged(object sender, EventArgs e)
    {
      if (ultraCBSearchBy.Value != null && this.totalColumnsQty > 0)
      {
        btnGetTemplate.Enabled = true;
      }
      else
      {
        btnGetTemplate.Enabled = false;
      }
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = folder;
      dialog.Title = "Select a Excel file";
      txtFilePath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      btnPrint.Enabled = false;
      radPortrail.Enabled = false;
      radLandscape.Enabled = false;
      chkTotalColumn.Enabled = false;
      if (this.CheckInvalid())
      {
        if(DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 1)
        {
          columnsQty = DBConvert.ParseInt(txtColumnsQty.Text);
          this.GetPictureReport();
        }
        else if(DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 2)
        {
          this.GetHardWareListReport();
        }
      }
      btnPrint.Enabled = true;
      radPortrail.Enabled = true;
      radLandscape.Enabled = true;
      chkTotalColumn.Enabled = true;
    }

    private void ultraCBReports_TextChanged(object sender, EventArgs e)
    {
      if (ultraCBReports.Value != null)
      {
        if (DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 1)
        {
          tableLayoutPictureReport.Visible = true;
          tableLayoutHardWareList.Visible = false;
          radPortrail.Visible = true;
          radLandscape.Visible = true;
          chkTotalColumn.Visible = true;
        }
        else if (DBConvert.ParseInt(ultraCBReports.Value.ToString()) == 2)
        {
          // Load HardWare List
          this.LoadHardWareList();

          tableLayoutPictureReport.Visible = false;
          radPortrail.Visible = false;
          radLandscape.Visible = false;
          chkTotalColumn.Visible = false;
          tableLayoutHardWareList.Visible = true;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return ;
      }
    }

    private void txtColumnsQty_TextChanged(object sender, EventArgs e)
    {
      if (DBConvert.ParseInt(txtColumnsQty.Text) != int.MinValue)
      {
        int qty = DBConvert.ParseInt(txtColumnsQty.Text);
        if (qty > 5 && qty < 10)
        {
          radLandscape.Checked = true;
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion event
  }
}
