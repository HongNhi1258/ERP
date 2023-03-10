/*
  Author      : Duong Minh
  Date        : 14/05/2012
  Description : StockCount Location
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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.IO;
using System.Diagnostics;
using VBReport;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_06_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewFGH_06_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewVEN_01_005_Load(object sender, EventArgs e)
    {
      // Load UltraCombo Location
      this.LoadComboLocation();
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Load UltraCombo Location
    /// </summary>
    private void LoadComboLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT PID, Location";
      commandText += " FROM TblWHFLocation ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLocation.DataSource = dtSource;
      ultLocation.DisplayMember = "Location";
      ultLocation.ValueMember = "PID";
      ultLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocation.DisplayLayout.Bands[0].Columns["Location"].Width = 150;
      ultLocation.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
    }
    #endregion LoadData

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      if (this.ultLocation.Value == null || DBConvert.ParseLong(this.ultLocation.Value.ToString()) == long.MinValue)
      {
        string message = "Please Fill Location";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string commandText = string.Empty;
      commandText = "  SELECT BOX.SeriBoxNo BoxId, BBT.BoxTypeCode BoxCode, DIM.[Length], DIM.Width, DIM.Height,";
      commandText += "    CASE WHEN BOX.[Weight] < 0 THEN 0 ELSE BOX.[Weight] END [Weight]";
      commandText += " FROM TblWHFBoxInStore BIS";
      commandText += " INNER JOIN TblWHFBox BOX ON BIS.BoxPID = BOX.PID";
      commandText += " INNER JOIN TblBOMBoxType BBT ON BOX.BoxTypePID = BBT.Pid";
      commandText += " INNER JOIN TblWHFDimension DIM ON BOX.DimensionPID = DIM.Pid";
      commandText += " INNER JOIN TblWHFLocation LOC ON BIS.Location = LOC.PID ";
      commandText += " WHERE BIS.Location = " + DBConvert.ParseLong(this.ultLocation.Value.ToString());

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      this.ultraDetail.DataSource = dt;
    }

    private void ultraDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Weight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Length"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Length"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["Width"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Width"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["Height"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Height"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["Weight"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Weight"].MaxWidth = 80;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnExportBox_Click(object sender, EventArgs e)
    {
      if (this.ultLocation.Value == null || DBConvert.ParseLong(this.ultLocation.Value.ToString()) == long.MinValue)
      {
        string message = "Please Fill Location";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a;

      // Get path from Folder
      //string path = @"\PhanmemDENSOBHT8000";
      //path = Path.GetFullPath(path);
      //string pathBarCode = path + @"\THONGTIN.txt";
      string pathBarCode = txtImportExcelFile.Text.Trim();
      try
      {
        a = File.ReadAllLines(pathBarCode);
      }
      catch
      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (a.Length == 0)
      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      DataTable dtScan = new DataTable();

      DataColumn boxId = new DataColumn("BoxId");
      boxId.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(boxId);

      DataColumn itemGroup = new DataColumn("ItemGroup");
      itemGroup.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(itemGroup);

      DataColumn revision = new DataColumn("Revision");
      revision.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(revision);

      DataColumn itemCode = new DataColumn("ItemCode");
      itemCode.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(itemCode);

      DataColumn saleCode = new DataColumn("SaleCode");
      saleCode.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(saleCode);

      DataColumn boxCode = new DataColumn("BoxCode");
      boxCode.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(boxCode);

      DataColumn eoh = new DataColumn("EOH");
      eoh.DataType = System.Type.GetType("System.Int32");
      dtScan.Columns.Add(eoh);

      DataColumn qty = new DataColumn("Qty");
      qty.DataType = System.Type.GetType("System.Int32");
      dtScan.Columns.Add(qty);

      DataColumn diff = new DataColumn("Diff");
      diff.DataType = System.Type.GetType("System.Int32");
      dtScan.Columns.Add(diff);

      int index = int.MinValue;
      int i = 0;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      if (index != -1)
      {
        for (i = 0; i < a.Length; i++)
        {
          if (a[i].Trim().ToString() != string.Empty)
          {
            index = a[i].IndexOf("*");
            a[i] = a[i].Substring(0, index).Trim().ToString();
          }
        }
      }

      int j, k;
      string commandText = string.Empty;
      for (i = 0; i < a.Length; i++)
      {
        //check duplicate
        k = 0;
        for (j = i + 1; j < a.Length; j++)
        {
          if (a[i].ToString() == a[j].ToString())
          {
            k++;
          }
        }
        if (k > 0)
        {
          continue;
        }
        else
        {
          commandText = string.Empty;
          commandText += " SELECT BBT.BoxTypeCode, CM.Value [ItemGroup], PAK.Revision, PAK.ItemCode, IB.SaleCode,";
          commandText += "            CASE WHEN BIS.PID IS NULL THEN 0 ELSE 1 END [HaveLocation], PAK.QuantityItem";
          commandText += " FROM TblWHFBox BOX  ";
          commandText += " 	INNER JOIN TblBOMBoxType BBT ON BOX.BoxTypePID = BBT.Pid";
          commandText += " 	LEFT JOIN TblBOMCodeMaster CM ON BOX.ProductType = CM.Code AND CM.[Group] = 4001";
          commandText += " 	LEFT JOIN TblBOMPackage PAK ON BBT.PackagePid = PAK.Pid";
          commandText += "	LEFT JOIN TblBOMItemBasic IB ON PAK.ItemCode = IB.ItemCode";
          commandText += "	LEFT JOIN TblWHFBoxInStore BIS ON BOX.Pid = BIS.BoxPID AND BIS.Location = " + DBConvert.ParseLong(this.ultLocation.Value.ToString());
          commandText += " WHERE BOX.SeriBoxNo = '" + a[i].ToString() + "'";

          DataTable dtChk = DataBaseAccess.SearchCommandTextDataTable(commandText);

          DataRow drScan = dtScan.NewRow();
          drScan["BoxId"] = a[i].ToString().Trim();
          if (dtChk.Rows.Count > 0)
          {
            drScan["BoxCode"] = dtChk.Rows[0]["BoxTypeCode"].ToString();
            drScan["ItemGroup"] = dtChk.Rows[0]["ItemGroup"].ToString();
            drScan["Revision"] = dtChk.Rows[0]["Revision"].ToString();
            drScan["ItemCode"] = dtChk.Rows[0]["ItemCode"].ToString();
            drScan["SaleCode"] = dtChk.Rows[0]["SaleCode"].ToString();
            if (DBConvert.ParseInt(dtChk.Rows[0]["HaveLocation"].ToString()) == 0)
            {
              drScan["EOH"] = 0;
              drScan["Qty"] = DBConvert.ParseInt(dtChk.Rows[0]["QuantityItem"].ToString());
            }
            else
            {
              drScan["EOH"] = DBConvert.ParseInt(dtChk.Rows[0]["QuantityItem"].ToString());
              drScan["Qty"] = DBConvert.ParseInt(dtChk.Rows[0]["QuantityItem"].ToString());
            }

          }
          else
          {
            drScan["BoxCode"] = "";
            drScan["ItemGroup"] = "";
            drScan["Revision"] = "";
            drScan["ItemCode"] = "";
            drScan["SaleCode"] = "";
            drScan["EOH"] = 0;
            drScan["Qty"] = 1;
          }

          drScan["Diff"] = DBConvert.ParseInt(drScan["Qty"].ToString()) - DBConvert.ParseInt(drScan["EOH"].ToString());

          dtScan.Rows.Add(drScan);
        }
      }

      commandText = string.Empty;
      commandText += " SELECT BOX.SeriBoxNo, BBT.BoxTypeCode, CM.Value [ItemGroup], PAK.Revision, PAK.ItemCode, IB.SaleCode, PAK.QuantityItem ";
      commandText += " FROM TblWHFBoxInStore BIS";
      commandText += "  INNER JOIN TblWHFBox BOX ON BIS.BoxPID = BOX.PID";
      commandText += "  INNER JOIN TblBOMBoxType BBT ON BOX.BoxTypePID = BBT.Pid";
      commandText += "  INNER JOIN TblWHFLocation LOC ON BIS.Location = LOC.PID ";
      commandText += " 	LEFT JOIN TblBOMCodeMaster CM ON BOX.ProductType = CM.Code AND CM.[Group] = 4001";
      commandText += " 	LEFT JOIN TblBOMPackage PAK ON BBT.PackagePid = PAK.Pid";
      commandText += "	LEFT JOIN TblBOMItemBasic IB ON PAK.ItemCode = IB.ItemCode";
      commandText += " WHERE BIS.Location = " + DBConvert.ParseLong(this.ultLocation.Value.ToString());

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt.Rows.Count > 0)
      {
        DataTable dtTemp = dt.Copy();
        for (j = 0; j < dtTemp.Rows.Count; j++)
        {
          for (int m = 0; m < dtScan.Rows.Count; m++)
          {
            if (string.Compare(dtTemp.Rows[j]["SeriBoxNo"].ToString(), dtScan.Rows[m]["BoxId"].ToString().Trim()) == 0)
            {
              dt.Rows[j].Delete();
            }
          }
        }
      }

      foreach (DataRow drScan in dt.Rows)
      {
        if (drScan.RowState != DataRowState.Deleted)
        {
          DataRow drMain = dtScan.NewRow();
          drMain["BoxId"] = drScan["SeriBoxNo"].ToString();
          drMain["BoxCode"] = drScan["BoxTypeCode"].ToString();
          drMain["ItemGroup"] = drScan["ItemGroup"].ToString();
          drMain["Revision"] = drScan["Revision"].ToString();
          drMain["ItemCode"] = drScan["ItemCode"].ToString();
          drMain["SaleCode"] = drScan["SaleCode"].ToString();
          drMain["Qty"] = 0;
          drMain["EOH"] = DBConvert.ParseInt(drScan["QuantityItem"].ToString());
          drMain["Diff"] = DBConvert.ParseInt(drMain["Qty"].ToString()) - DBConvert.ParseInt(drMain["EOH"].ToString());
          dtScan.Rows.Add(drMain);
        }
      }

      string strTemplateName = "FGH_06_001_01";
      string strSheetName = "StockCount";
      string strOutFileName = "StockCountLocationBoxId";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport XlsReport1 = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (dtScan.Rows.Count > 0)
      {
        int totalEOH = 0;
        int totalQty = 0;
        for (i = 0; i < dtScan.Rows.Count; i++)
        {
          DataRow dtRow = dtScan.Rows[i];
          if (i > 0)
          {
            XlsReport1.Cell("B8:M8").Copy();
            XlsReport1.RowInsert(7 + i);
            XlsReport1.Cell("B8:M8", 0, i).Paste();
          }
          XlsReport1.Cell("**No", 0, i).Value = i + 1;
          XlsReport1.Cell("**BoxId", 0, i).Value = dtRow["BoxId"].ToString();
          XlsReport1.Cell("**ItemGroup", 0, i).Value = dtRow["ItemGroup"].ToString();
          XlsReport1.Cell("**Revision", 0, i).Value = dtRow["Revision"].ToString();
          XlsReport1.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          XlsReport1.Cell("**SaleCode", 0, i).Value = dtRow["SaleCode"].ToString();
          XlsReport1.Cell("**BoxCode", 0, i).Value = dtRow["BoxCode"].ToString();
          XlsReport1.Cell("**EOH", 0, i).Value = DBConvert.ParseInt(dtRow["EOH"].ToString());
          XlsReport1.Cell("**Qty", 0, i).Value = DBConvert.ParseInt(dtRow["Qty"].ToString());
          XlsReport1.Cell("**Diff", 0, i).Value = DBConvert.ParseInt(dtRow["Diff"].ToString());

          if (DBConvert.ParseInt(dtRow["Diff"].ToString()) < 0)
          {
            XlsReport1.Cell("**BoxId", 0, i).Attr.FontColor = VBReport.xlColor.xcBlue;
          }
          else if (DBConvert.ParseInt(dtRow["Diff"].ToString()) > 0)
          {
            XlsReport1.Cell("**BoxId", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
          }
          else
          {
            XlsReport1.Cell("**BoxId", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
          }
          totalEOH += DBConvert.ParseInt(dtRow["EOH"].ToString());
          totalQty += DBConvert.ParseInt(dtRow["Qty"].ToString());
        }

        XlsReport1.Cell("**TotalEOH").Value = totalEOH;
        XlsReport1.Cell("**TotalQty").Value = totalQty;
        XlsReport1.Cell("**TotalDif").Value = totalQty - totalEOH;

      }
      //Header
      XlsReport1.Cell("**Location").Value = this.ultLocation.Text.ToString();
      XlsReport1.Cell("**Date").Value = string.Format(@"Date : {0}", DBConvert.ParseString(DateTime.Now, "dd/MM/yyyy"));

      XlsReport1.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void btnExportItemCode_Click(object sender, EventArgs e)
    {
      if (this.ultLocation.Value == null || DBConvert.ParseLong(this.ultLocation.Value.ToString()) == long.MinValue)
      {
        string message = "Please Fill Location";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a;

      // Get path from Folder
      string path = @"\PhanmemDENSOBHT8000";
      path = Path.GetFullPath(path);
      string pathBarCode = path + @"\THONGTIN.txt";
      try
      {
        a = File.ReadAllLines(pathBarCode);
      }
      catch
      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (a.Length == 0)
      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      DataTable dtScan = new DataTable();

      DataColumn boxId = new DataColumn("BoxId");
      boxId.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(boxId);

      DataColumn boxCode = new DataColumn("BoxCode");
      boxCode.DataType = System.Type.GetType("System.String");
      dtScan.Columns.Add(boxCode);

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }
      int i = 0;
      if (index != -1)
      {
        for (i = 0; i < a.Length; i++)
        {
          if (a[i].Trim().ToString() != string.Empty)
          {
            index = a[i].IndexOf("*");
            a[i] = a[i].Substring(0, index).Trim().ToString();
          }
        }
      }

      int j, k;
      string commandText = string.Empty;
      for (i = 0; i < a.Length; i++)
      {
        //check duplicate
        k = 0;
        for (j = i + 1; j < a.Length; j++)
        {
          if (a[i].ToString() == a[j].ToString())
          {
            k++;
          }
        }
        if (k > 0)
        {
          continue;
        }
        else
        {
          commandText = "SELECT BBT.BoxTypeCode FROM TblWHFBox BOX INNER JOIN TblBOMBoxType BBT ON BOX.BoxTypePID = BBT.Pid WHERE SeriBoxNo = '" + a[i].ToString() + "' AND Status = 2";
          DataTable dtChk = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtChk.Rows.Count == 0)
          {
            continue;
          }

          DataRow drScan = dtScan.NewRow();
          drScan["BoxId"] = a[i].ToString();
          drScan["BoxCode"] = dtChk.Rows[0]["BoxTypeCode"].ToString();
          dtScan.Rows.Add(drScan);
        }
      }

      foreach (DataRow drScan in dtScan.Rows)
      {
        //temp
        DBParameter[] arrInput = new DBParameter[1];
        arrInput[0] = new DBParameter("@SeriBox", DbType.AnsiString, 16, drScan["BoxId"].ToString());

        DataBaseAccess.ExecuteStoreProcedure("spWHFTempGroupCode_Insert", arrInput);
      }

      //bl
      DBParameter[] arrbl = new DBParameter[1];
      arrbl[0] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(this.ultLocation.Value.ToString()));

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spRPTStockBalanceScan_Location", arrbl);

      string strTemplateName = "FGH_06_001_02";
      string strSheetName = "StockCount";
      string strOutFileName = "StockCountLocationItemCode";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport XlsReport1 = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      long blCtns = 0;
      long blQty = 0;
      long snCtns = 0;
      long snQty = 0;

      if (ds.Tables[0].Rows.Count > 0)
      {
        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
          DataRow dtRow = ds.Tables[0].Rows[i];
          if (i > 0)
          {
            XlsReport1.Cell("B9:L9").Copy();
            XlsReport1.RowInsert(8 + i);
            XlsReport1.Cell("B9:L9", 0, i).Paste();
          }
          XlsReport1.Cell("**No", 0, i).Value = i + 1;
          XlsReport1.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          XlsReport1.Cell("**Revision", 0, i).Value = dtRow["Revision"];
          XlsReport1.Cell("**BoxCode", 0, i).Value = dtRow["BoxTypeCode"].ToString();
          XlsReport1.Cell("**BLCTNS", 0, i).Value = dtRow["BL CTNS"];
          if (dtRow["BL CTNS"].ToString().Trim().Length > 0)
          {
            blCtns += DBConvert.ParseLong(dtRow["BL CTNS"].ToString());
          }
          XlsReport1.Cell("**BLQty", 0, i).Value = dtRow["BL Qty"];
          if (dtRow["BL Qty"].ToString().Trim().Length > 0)
          {
            blQty += DBConvert.ParseLong(dtRow["BL Qty"].ToString());
          }
          XlsReport1.Cell("**SCANCTNS", 0, i).Value = dtRow["SCAN CTNS"];
          if (dtRow["SCAN CTNS"].ToString().Trim().Length > 0)
          {
            snCtns += DBConvert.ParseLong(dtRow["SCAN CTNS"].ToString());
          }
          XlsReport1.Cell("**SCANQty", 0, i).Value = dtRow["SCAN Qty"];
          if (dtRow["SCAN Qty"].ToString().Trim().Length > 0)
          {
            snQty += DBConvert.ParseLong(dtRow["SCAN Qty"].ToString());
          }

          if (dtRow["ItemCode"].ToString().Length == 0)
          {
            if (dtRow["BL CTNS"].ToString().Length > 0)
            {
              XlsReport1.Cell("**BLQty", 0, i).Value = dtRow["BL CTNS"];
              blQty += DBConvert.ParseLong(dtRow["BL CTNS"].ToString());
            }

            if (dtRow["SCAN CTNS"].ToString().Length > 0)
            {
              XlsReport1.Cell("**SCANQty", 0, i).Value = dtRow["SCAN CTNS"];
              snQty += DBConvert.ParseLong(dtRow["SCAN CTNS"].ToString());
            }
          }

          long dCtns = long.MinValue;
          long dQty = long.MinValue;
          if (DBConvert.ParseInt(dtRow["SCAN CTNS"].ToString()) != int.MinValue
                && DBConvert.ParseInt(dtRow["BL CTNS"].ToString()) != int.MinValue)
          {
            dCtns = DBConvert.ParseInt(dtRow["SCAN CTNS"].ToString()) - DBConvert.ParseInt(dtRow["BL CTNS"].ToString());
          }

          if (DBConvert.ParseInt(dtRow["SCAN Qty"].ToString()) != int.MinValue
                && DBConvert.ParseInt(dtRow["BL Qty"].ToString()) != int.MinValue)
          {
            dQty = DBConvert.ParseInt(dtRow["SCAN Qty"].ToString()) - DBConvert.ParseInt(dtRow["BL Qty"].ToString());
          }

          if (dCtns != long.MinValue)
          {
            XlsReport1.Cell("**DCTNS", 0, i).Value = dCtns;
          }

          if (dQty != long.MinValue)
          {
            XlsReport1.Cell("**DQty", 0, i).Value = dQty;
          }

          if (dCtns != 0 || dQty != 0)
          {
            XlsReport1.Cell("**No", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**ItemCode", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**Revision", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**BoxCode", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**BLCTNS", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**BLQty", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**SCANCTNS", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**SCANQty", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**DCTNS", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
            XlsReport1.Cell("**DQty", 0, i).Attr.FontColor = VBReport.xlColor.xcRed;
          }
          else
          {
            XlsReport1.Cell("**No", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**ItemCode", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**Revision", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**BoxCode", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**BLCTNS", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**BLQty", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**SCANCTNS", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**SCANQty", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**DCTNS", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
            XlsReport1.Cell("**DQty", 0, i).Attr.FontColor = VBReport.xlColor.xcDefault;
          }
        }
      }

      //Header
      XlsReport1.Cell("**Location").Value = this.ultLocation.Text.ToString();
      XlsReport1.Cell("**Date").Value = string.Format(@"Date : {0}", DBConvert.ParseString(DateTime.Now, "dd/MM/yyyy"));

      XlsReport1.Cell("**TBLCTNS").Value = blCtns;
      XlsReport1.Cell("**TBLQty").Value = blQty;
      XlsReport1.Cell("**TSCCTNS").Value = snCtns;
      XlsReport1.Cell("**TSCQty").Value = snQty;

      XlsReport1.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Text files (*.txt)|*.txt| All files (*.*)|*.*";
      dialog.Title = "Select a text file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    #endregion Event
  }
}
