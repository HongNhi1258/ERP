/*
  Author      : Duong Minh
  Date        : 28/03/2012
  Description : Create Receving From Production
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Configuration; 
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.FinishGoodWarehouse;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using Infragistics.Win;
using System.Diagnostics;
using VBReport;
using System.IO;
using System.Data.SqlClient;
using DaiCo.Shared.DataSetSource.FinishGoodWarehouse;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_04_002 : MainUserControl
  {
    #region Field
    public long inStorePid = long.MinValue;
    private string rcCode = string.Empty;
    private int status = 0;
    private DataTable dt;
    #endregion Field

    #region Init
    public viewFGH_04_002()
    {
      InitializeComponent();
    }

    private void viewFGH_03_001_Load(object sender, EventArgs e)
    {
      // Check Summary MonthLy Duong Minh 10/10/2011 START
      bool result = this.CheckSummary();
      if (result == false)
      {
        this.CloseTab();
        return;
      }
      // Check Summary MonthLy Duong Minh 10/10/2011 END

      this.LoadDeparment();
      this.LoadPackingIssue();
      this.LoadDropdownLocation(this.ultLocation);
      this.LoadDropdownRemark(this.ultRemark);

      this.LoadData();
    }

    /// <summary>
    /// Check Summary PreMonth && PreYear
    /// </summary>
    /// <returns></returns>
    private bool CheckSummary()
    {
      DateTime firstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      int result = DateTime.Compare(firstDate, DateTime.Today);

      if (result <= 0)
      {
        int preMonth = 0;
        int preYear = 0;
        if (DateTime.Today.Month == 1)
        {
          preMonth = 12;
          preYear = DateTime.Today.Year - 1;
        }
        else
        {
          preMonth = DateTime.Today.Month - 1;
          preYear = DateTime.Today.Year;
        }

        string commandText = "SELECT COUNT(*) FROM TblWHFBalance WHERE Month = " + preMonth + " AND Year = " + preYear;
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if ((dtCheck == null) || (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0))
        {
          WindowUtinity.ShowMessageError("ERR0303", preMonth.ToString(), preYear.ToString());
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Load Department
    /// </summary>
    private void LoadDeparment()
    {
      string commandText = " SELECT Department, DeparmentName ";
      commandText += "       FROM VHRDDepartment  ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "DeparmentName";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["DeparmentName"].Width = 500;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load Packing Issue
    /// </summary>
    private void LoadPackingIssue()
    {
      string commandText = " SELECT DISTINCT PISS.Pid, PISS.IssuingNote ";
      commandText += "       FROM TblBOMPackingIssuingNote PISS  ";
      commandText += "       	INNER JOIN TblWHFBox BOX ON PISS.Pid = BOX.PackingIssue  ";
      commandText += "       WHERE PISS.[Status] = 0  ";
      commandText += "       ORDER BY PISS.Pid DESC  ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultPackingIssue.DataSource = dtSource;
      ultPackingIssue.DisplayMember = "IssuingNote";
      ultPackingIssue.ValueMember = "Pid";
      ultPackingIssue.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultPackingIssue.DisplayLayout.Bands[0].Columns["IssuingNote"].Width = 300;
      ultPackingIssue.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// LoadDropdownLocation
    /// </summary> 
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownLocation(UltraDropDown udrpLocation)
    {
      string commandText = " SELECT PID, Location ";
      commandText += "       FROM TblWHFLocation  ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpLocation.DataSource = dtSource;
      udrpLocation.ValueMember = "PID";
      udrpLocation.DisplayMember = "Location";
      udrpLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpLocation.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
      udrpLocation.DisplayLayout.Bands[0].Columns["Location"].Width = 200;
    }

    /// <summary>
    /// LoadDropdownRemark
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownRemark(UltraDropDown udrpRemark)
    {
      string commandText = " SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 1008 ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      udrpRemark.DataSource = dtSource;
      udrpRemark.ValueMember = "Code";
      udrpRemark.DisplayMember = "Value";
      udrpRemark.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpRemark.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      udrpRemark.DisplayLayout.Bands[0].Columns["Value"].Width = 200;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      // Insert
      if (this.inStorePid == long.MinValue)
      {

        string inStoreID = string.Empty;
        string commandText = string.Empty;
        string inStoreCode = string.Empty;
        DataSet ds = new DataSet();

        string time = DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0');
        commandText = "Select Max(SUBSTRING(InStoreCode,7,11)) From TblWHFInStore Where InStoreCode like '09RTW-" + time + "%'";
        ds = DataBaseAccess.SearchCommandText(commandText);
        if (ds.Tables[0].Rows[0][0].ToString() == "")
        {
          inStoreID = DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0') + "-0001";
        }
        else
        {
          int max = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString().Substring(7, 4)) + 1;
          inStoreID = DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0') + "-" + max.ToString().PadLeft(4, '0');
        }
        inStoreCode = "09RTW" + "-" + inStoreID;

        this.txtReceivingCode.Text = inStoreCode;

        this.ultDepartment.Value = "PACK";
      }
      // Update
      else
      {
        string strCommandText = "SELECT InStoreCode FROM TblWHFInStore WHERE PID = " + this.inStorePid;
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(strCommandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          this.rcCode = dt.Rows[0][0].ToString();
        }
        else
        {
          return;
        }

        this.txtReceivingCode.Text = this.rcCode;
        strCommandText = "SELECT UserWH,Posting,Note,Department FROM TblWHFInStore WHERE InStoreCode = '" + this.rcCode + "'";
        dt = DataBaseAccess.SearchCommandTextDataTable(strCommandText);
        if (dt.Rows.Count != 1)
        {
          return;
        }

        this.ultDepartment.Value = dt.Rows[0]["Department"].ToString();
        this.txtNote.Text = dt.Rows[0]["Note"].ToString();

        this.status = DBConvert.ParseInt(dt.Rows[0]["Posting"].ToString());

        strCommandText = "      SELECT ROW_NUMBER() OVER ( ORDER BY BOX.SeriBoxNo) [No], PISS.IssuingNote PackingIssue, CM.Value ProductType, BOX.SeriBoxNo, BOX.SeriBoxNo AS PackingBox, ";
        strCommandText += "                 BOT.BoxTypeCode,BOT.BoxTypeName,BOX.WorkOrder,DIM.Length,DIM.Width,DIM.Height, BOX.[Set], IND.Location, ";
        strCommandText += "                 REK.Value Remark, REK.Code, 0 [Check] ";
        strCommandText += "      FROM		    TblWHFInStore			    INS ";
        strCommandText += "      INNER JOIN	TblWHFInStoreDetail		IND		ON	INS.PID     =   IND.InStorePID ";
        strCommandText += "      INNER JOIN	TblWHFBox				      BOX		ON	IND.BoxPID  =   BOX.PID";
        strCommandText += "      INNER JOIN	TblBOMBoxType			    BOT		ON	BOT.Pid	    =   BOX.BoxTypePID";
        strCommandText += "      LEFT  JOIN	TblWHFDimension			  DIM		ON	DIM.Pid     =   BOX.DimensionPID";
        strCommandText += "      LEFT  JOIN	TblBOMPackingIssuingNote PISS ON	PISS.Pid  =   BOX.PackingIssue";
        strCommandText += "      LEFT  JOIN	TblBOMCodeMaster CM ON	CM.Code = BOX.ProductType AND CM.[Group] = 4001";
        strCommandText += "      LEFT JOIN TblBOMCodeMaster REK ON REK.Code = IND.Remark AND REK.[Group] = 1008";
        strCommandText += "      WHERE INS.InStoreCode = '" + this.rcCode + "'";
        strCommandText += "      ORDER BY PISS.IssuingNote, BOX.SeriBoxNo";

        dt = DataBaseAccess.SearchCommandTextDataTable(strCommandText);

        this.ultInStoreDetail.DataSource = dt;
      
        //Set Control
        this.SetStatusControl();
      }
    }

    private void SetStatusControl()
    {
      if (this.status == 1)
      {
        this.txtNote.ReadOnly = true;
        this.txtNote.Enabled = false;
        this.btnLoad.Enabled = false; 
        this.chkConfirm.Enabled = false;
        this.btnSave.Enabled = false;
        this.ultPackingIssue.Enabled = false;
        this.ultDepartment.Enabled = false;
        for (int i = 0; i < ultInStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultInStoreDetail.Rows[i];
          row.Activation = Activation.ActivateOnly;
        }
      }
      else
      {
        this.txtNote.ReadOnly = false;
        this.txtNote.Enabled = true;
        this.chkConfirm.Enabled = true;
        this.btnSave.Enabled = true;
        this.btnLoad.Enabled = true;
        this.ultPackingIssue.Enabled = true;
        this.ultDepartment.Enabled = true;
      }
    }

    private DataTable createTableBox()
    {
      DataTable dt = new DataTable();

      DataColumn no = new DataColumn("No");
      no.DataType = System.Type.GetType("System.Int32");
      dt.Columns.Add(no);

      DataColumn packingIssue = new DataColumn("PackingIssue");
      packingIssue.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(packingIssue);

      DataColumn productType = new DataColumn("ProductType");
      productType.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(productType);

      DataColumn packingBox = new DataColumn("PackingBox");
      packingBox.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(packingBox);

      DataColumn seriBoxNo = new DataColumn("SeriBoxNo");
      seriBoxNo.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(seriBoxNo);

      DataColumn boxType = new DataColumn("BoxTypeCode");
      boxType.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(boxType);

      DataColumn boxTypeName = new DataColumn("BoxTypeName");
      boxTypeName.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(boxTypeName);

      DataColumn workOrder = new DataColumn("WorkOrder");
      workOrder.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(workOrder);

      DataColumn length = new DataColumn("Length");
      length.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(length);

      DataColumn width = new DataColumn("Width");
      width.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(width);

      DataColumn height = new DataColumn("Height");
      height.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(height);

      DataColumn set = new DataColumn("Set");
      set.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(set);

      DataColumn location = new DataColumn("Location");
      location.DataType = System.Type.GetType("System.Int32");
      dt.Columns.Add(location);

      DataColumn remark = new DataColumn("Remark");
      remark.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(remark);

      DataColumn code = new DataColumn("Code");
      code.DataType = System.Type.GetType("System.Int32");
      dt.Columns.Add(code);

      DataColumn checkBox = new DataColumn("Check");
      checkBox.DataType = System.Type.GetType("System.Int32");
      dt.Columns.Add(checkBox);

      return dt;
    }
    #endregion Function

    #region Event 
    private void ultOutStoreDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Set"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["BoxTypeName"].Header.Caption = "Box Name";
      e.Layout.Bands[0].Columns["BoxTypeCode"].Header.Caption = "Box Code";
      e.Layout.Bands[0].Columns["SeriBoxNo"].Header.Caption = "Box Id";

      e.Layout.Bands[0].Columns["Location"].ValueList = ultLocation;
      e.Layout.Bands[0].Columns["Remark"].ValueList = ultRemark;
      e.Layout.Bands[0].Columns["Check"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["No"].MinWidth = 25;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 25;

      e.Layout.Bands[0].Columns["Remark"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Remark"].MaxWidth = 70;

      e.Layout.Bands[0].Columns["Location"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Check"].MinWidth = 25;
      e.Layout.Bands[0].Columns["Check"].MaxWidth = 25;

      e.Layout.Bands[0].Columns["Length"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Length"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Width"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Width"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Height"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Height"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 50;
      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Set"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Set"].MaxWidth = 50;
     
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      if (this.ultPackingIssue.Value == null || DBConvert.ParseLong(this.ultPackingIssue.Value.ToString()) == long.MinValue)
      {
        string message = "Please fill Packing Issue";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a;
      DriveInfo[] allDrives = DriveInfo.GetDrives();
      string stringName = string.Empty;
      string path = "PhanmemDENSOBHT8000";
      int flagDrive = 0;
      foreach (DriveInfo d in allDrives)
      {
        stringName = d.Name;
        path = stringName + path;
        if (Directory.Exists(path))
        {
          flagDrive = 1;
          break;
        }
        path = "PhanmemDENSOBHT8000";
        stringName = string.Empty;
      }

      if (flagDrive == 0)
      {
        return;
      }

      // Get path from Folder
      
      //path = Path.GetFullPath(path);
      string pathBarCode = path + @"\THONGTIN.txt";
      try
      {
        a = File.ReadAllLines(pathBarCode);
      }
      catch ( Exception ex)
      {
        MessageBox.Show(ex.ToString());
        //MessageBox.Show(pathBarCode);

        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (a.Length == 0)
      {
        MessageBox.Show("2");
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      //MessageBox.Show(pathBarCode);

      string strCommandText = "   SELECT SeriBoxNo" +
                              "   FROM TblWHFBox box" +
                              "   INNER JOIN TblBOMPackingIssuingNote packing ON box.PackingIssue = packing.Pid " +
                              "   WHERE box.Status = 1 AND packing.Status = 0 AND packing.Pid =" + DBConvert.ParseLong(this.ultPackingIssue.Value.ToString());

      DataSet dsPacking = DataBaseAccess.SearchCommandText(strCommandText);

      if (dt == null)
      {
        dt = this.createTableBox();
      }

      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (String.Compare(dt.Rows[i]["PackingIssue"].ToString(), ultPackingIssue.Text) == 0)
        {
          dt.Rows.RemoveAt(i);
          i--;
        }
      }

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      if (index != -1)
      {
        for (int i = 0; i < a.Length; i++)
        {
          if (a[i].Trim().ToString() != string.Empty)
          {
            index = a[i].IndexOf("*");
            a[i] = a[i].Substring(0, index).Trim().ToString();
          }
        }
      }

      int k = 0;
      DataRow[] foundRow;
      for (int i = 0; i < a.Length; i++)
      {
        //check duplicate
        k = 0;
        for (int j = i + 1; j < a.Length; j++)
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
        //
        if (a[i].ToString().Length == 0)
        {
          continue;
        }

        DBParameter[] arrInputParam = new DBParameter[2];
        arrInputParam[0] = new DBParameter("@SeriBoxNo", DbType.String, a[i].ToString());
        arrInputParam[1] = new DBParameter("@Status", DbType.Int16, 1);

        DataTable dtBox = DataBaseAccess.SearchStoreProcedure("spWHFBoxBySeriBoxNo", arrInputParam).Tables[0];
        
        DataRow row = dt.NewRow();

        if (dtBox.Rows.Count == 0)
        {
          row["PackingBox"] = "";
          row["PackingIssue"] = this.ultPackingIssue.Text.ToString();
          row["SeriBoxNo"] = a[i].ToString();
          row["Check"] = 0;

          foundRow = dt.Select("SeriBoxNo ='" + a[i].ToString() + "'");
          if (foundRow.Length > 0)
          {
            continue;
          }

          dt.Rows.Add(row);
          continue;
        }

        Boolean flagCheck = false;
        foreach (DataRow drPacking in dsPacking.Tables[0].Rows)
        {
          if (drPacking["SeriBoxNo"].ToString() == dtBox.Rows[0]["SeriBoxNO"].ToString())
          {
            flagCheck = true;
            row["PackingBox"] = dtBox.Rows[0]["SeriBoxNO"].ToString();
            row["PackingIssue"] = this.ultPackingIssue.Text.ToString();

            for (int mm = 0; mm < dt.Rows.Count; mm++)
            {
              if (dt.Rows[mm]["SeriBoxNo"].ToString() == dtBox.Rows[0]["SeriBoxNO"].ToString())
              {
                dt.Rows[mm].Delete();
                break;
              }
            }

            break;
          }
        }

        if (flagCheck == false)
        {
          row["PackingBox"] = "";
        }
        row["PackingIssue"] = this.ultPackingIssue.Text.ToString();
        row["ProductType"] = dtBox.Rows[0]["ProductType"].ToString();
        row["SeriBoxNo"] = dtBox.Rows[0]["SeriBoxNO"].ToString();
        row["BoxTypeCode"] = dtBox.Rows[0]["BoxTypeCode"].ToString();
        row["BoxTypeName"] = dtBox.Rows[0]["BoxTypeName"].ToString();
        row["WorkOrder"] = dtBox.Rows[0]["WorkOrder"].ToString();
        row["Length"] = dtBox.Rows[0]["Length"].ToString();
        row["Width"] = dtBox.Rows[0]["Width"].ToString();
        row["Height"] = dtBox.Rows[0]["Height"].ToString();
        row["Set"] = dtBox.Rows[0]["Set"].ToString();
        row["Location"] = 20;
        row["Remark"] = dtBox.Rows[0]["Value"].ToString();
        row["Code"] = DBConvert.ParseInt(dtBox.Rows[0]["Code"].ToString());
        row["Check"] = 0;

        foundRow = dt.Select("SeriBoxNo ='" + dtBox.Rows[0]["SeriBoxNO"].ToString() + "'");
        if (foundRow.Length > 0)
        {
          continue;
        }

        dt.Rows.Add(row);
      }

      DataTable dtSource = dt;
      foreach (DataRow drPacking in dsPacking.Tables[0].Rows)
      {
        Boolean flag = false;
        foreach (DataRow dr in dt.Rows)
        {
          if (drPacking["SeriBoxNo"].ToString() == dr["SeriBoxNo"].ToString())
          {
            flag = true;
            break;
          }
        }
        if (flag == false)
        {
          DataRow row = dtSource.NewRow();
          row["PackingIssue"] = this.ultPackingIssue.Text.ToString();
          row["PackingBox"] = drPacking["SeriBoxNo"].ToString();
          row["SeriBoxNo"] = "";
          row["BoxTypeCode"] = "";
          row["BoxTypeName"] = "";
          row["WorkOrder"] = "";
          row["Length"] = "";
          row["Width"] = "";
          row["Height"] = "";
          row["Set"] = "";
          row["Location"] = 20;
          row["Check"] = 0;

          foundRow = dtSource.Select("SeriBoxNo ='" + drPacking["SeriBoxNo"].ToString() + "'");
          if (foundRow.Length > 0)
          {
            continue;
          }

          dtSource.Rows.Add(row);
        }
      }
      DataView dtView = dtSource.DefaultView;
      dtView.Sort = "PackingIssue,PackingBox";
      dtSource = dtView.ToTable();
      int count = 1;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        row["No"] = count.ToString();
        count++;
      }
      dt = dtSource;

      this.ultInStoreDetail.DataSource = dt;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      int count = 0;
      Boolean flag = false;

      if (this.ultDepartment.Value == null || this.ultDepartment.Value.ToString().Length == 0)
      {
        string message = "Please fill Department";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (this.ultInStoreDetail.Rows.Count == 0)
      {
        string message = "Data Box ID Is Wrong";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      for (count = 0; count < this.ultInStoreDetail.Rows.Count; count++)
      {
        UltraGridRow row = this.ultInStoreDetail.Rows[count];
        if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 0)
        {
          if (row.Cells["PackingBox"].Value.ToString().Length == 0
              && row.Cells["SeriBoxNo"].Value.ToString().Length > 0)
          {
            string message = "Box Is Wrong";
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }

          if (row.Cells["ProductType"].Value.ToString().Length == 0)
          {
            string message = "Product Type Is Wrong";
            WindowUtinity.ShowMessageErrorFromText(message);
            return;

          }

          if (DBConvert.ParseInt(row.Cells["Location"].Value.ToString()) == int.MinValue)
          {
            string message = "Please chosen location!";
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }

          if (row.Cells["PackingBox"].Value.ToString().Length > 0 &&
                        row.Cells["SeriBoxNo"].Value.ToString().Length == 0)
          {
            flag = true;
          }
        }
      }

      if (flag == true)
      {
        string message = "Box in store is not enough with " + this.ultPackingIssue.Text;
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      //update TblBOMPackingIssuingNote
      DBParameter[] arrInputParam = new DBParameter[5];

      int j = 0;
      if (this.chkConfirm.Checked)
      {
        j = 1;
      }
      else
      {
        j = 0;
      }

      string race = "";
      for (int mm = 0; mm < this.ultInStoreDetail.Rows.Count; mm++)
      {
        UltraGridRow row = this.ultInStoreDetail.Rows[mm];
        if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 0)
        {
          if (race.IndexOf(row.Cells["PackingIssue"].Value.ToString()) != -1)
          {
            continue;
          }

          DBParameter[] arrInputUpdateParam = new DBParameter[2];

          arrInputUpdateParam[0] = new DBParameter("@IssuingNote", DbType.String, row.Cells["PackingIssue"].Value.ToString());
          arrInputUpdateParam[1] = new DBParameter("@Status", DbType.Int32, 1);

          DataBaseAccess.ExecuteStoreProcedure("spWHFPackingIssuingNote_UpdateStatus", arrInputUpdateParam);
          race += row.Cells["PackingIssue"].Value.ToString();
        }
      }

      string inStoreID = string.Empty;
      string commandText = string.Empty;
      string inStoreCode = string.Empty;
      DataSet ds = new DataSet();

      string time = DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0');
      commandText = "Select Max(SUBSTRING(InStoreCode,7,11)) From TblWHFInStore Where InStoreCode like '09RTW-" + time + "%'";
      ds = DataBaseAccess.SearchCommandText(commandText);
      if (ds.Tables[0].Rows[0][0].ToString() == "")
      {
        inStoreID = DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0') + "-0001";
      }
      else
      {
        int max = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString().Substring(7, 4)) + 1;
        inStoreID = DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString().PadLeft(2, '0') + "-" + max.ToString().PadLeft(4, '0');
      }
      inStoreCode = "09RTW" + "-" + inStoreID;

      arrInputParam[0] = new DBParameter("@InStoreCode", DbType.String, inStoreCode);
      arrInputParam[1] = new DBParameter("@UserWH", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      arrInputParam[2] = new DBParameter("@Department", DbType.String, this.ultDepartment.Value.ToString());
      arrInputParam[3] = new DBParameter("@Posting", DbType.Int32, j.ToString());
      arrInputParam[4] = new DBParameter("@Note", DbType.String, txtNote.Text.ToString());

      DBParameter[] arrOutputParam = new DBParameter[1];
      arrOutputParam[0] = new DBParameter("@Result", DbType.Int32, 0);

      DataBaseAccess.ExecuteStoreProcedure("spWHFInStore_Insert", arrInputParam, arrOutputParam);

      long iResult = DBConvert.ParseLong(arrOutputParam[0].Value.ToString());

      this.inStorePid = iResult;
      this.rcCode = inStoreCode;

      for (int i = 0; i < this.ultInStoreDetail.Rows.Count; i++)
      {
        UltraGridRow row = this.ultInStoreDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 0)
        {
          DBParameter[] arrInputParamInStore = new DBParameter[4];

          arrInputParamInStore[0] = new DBParameter("@InstorePID", DbType.Int64, iResult);

          commandText = "Select PID From TblWHFBox Where SeriBoxNo = '" + row.Cells["SeriBoxNo"].Value.ToString() + "' AND [Status] = 2";
          ds = DataBaseAccess.SearchCommandText(commandText);
          if (ds.Tables[0].Rows.Count == 1)
          {
            continue;
          }

          commandText = "Select PID From TblWHFBox Where SeriBoxNo = '" + row.Cells["SeriBoxNo"].Value.ToString() + "'";
          ds = DataBaseAccess.SearchCommandText(commandText);
          if (ds.Tables[0].Rows.Count == 0)
          {
            continue;
          }

          arrInputParamInStore[1] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(ds.Tables[0].Rows[0][0].ToString()));
          arrInputParamInStore[2] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(row.Cells["Location"].Value.ToString()));

          if (DBConvert.ParseInt(row.Cells["Code"].Value.ToString()) != int.MinValue)
          {
            arrInputParamInStore[3] = new DBParameter("@Remark", DbType.Int32, DBConvert.ParseInt(row.Cells["Code"].Value.ToString()));
          }

          DataBaseAccess.ExecuteStoreProcedure("spWHFInStoreDetail_Insert", arrInputParamInStore);

          DBParameter[] arrInputParamBox = new DBParameter[2];
          arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["SeriBoxNo"].Value.ToString());
          arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 2);
          DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);
          
          DBParameter[] arrInputParamBoxInStore = new DBParameter[2];
          arrInputParamBoxInStore[0] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(ds.Tables[0].Rows[0][0].ToString()));
          arrInputParamBoxInStore[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(row.Cells["Location"].Value.ToString()));

          DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Insert", arrInputParamBoxInStore);
        }
      }

      DBParameter[] arrInputUpdatePackedItem = new DBParameter[1];
      arrInputUpdatePackedItem[0] = new DBParameter("@issPid", DbType.Int64, this.inStorePid);
      DataBaseAccess.ExecuteStoreProcedure("spWHFMiniusPackedItem_Edit", arrInputUpdatePackedItem);

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.inStoreCode = this.rcCode;
      view.ncategory = 3;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void btnPrintDetail_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.inStoreCode = this.rcCode;
      view.ncategory = 4;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }
    #endregion Event
  }
}
