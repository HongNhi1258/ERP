/*
  Author      : Đỗ Minh Tâm
  Date        : 16/11/2010
  Decription  : Custom price list Detail
  Checked by  :
  Checked date:
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
using System.IO;
using System.Threading;
using Microsoft.VisualBasic;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_005 : MainUserControl
  {
    public long outItemPid = long.MinValue;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    public viewCSD_04_005()
    {
      InitializeComponent();
    }
    
    private void btlClose_Click(object sender, EventArgs e)
    {
      //if (!this.CheckValid())
      //{
      //  return;
      //}
      this.ConfirmToCloseTab();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      btnSave_Click(sender, e);
      FileStream fs;
      BinaryReader br;
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.outItemPid);
      DBParameter[] outputParam = new DBParameter[2] ;
      outputParam[0] = new DBParameter("@Fontsize", DbType.Int32,0);
      outputParam[1] = new DBParameter("@Description", DbType.String , 512,"");
      DataTable dtReport = DataBaseAccess.SearchStoreProcedureDataTable("spCSDReportOutputItemForCustomer", inputParam, outputParam);
      int a = DBConvert.ParseInt(outputParam[0].Value.ToString());
      string strDes = outputParam[1].Value.ToString();

      dtReport.Columns.Add("Image", System.Type.GetType("System.Byte[]"));
      for (int i = 0; i < dtReport.Rows.Count; i++)
      {
          dtReport.Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dtReport.Rows[i]["img"].ToString());
      }
      DaiCo.CustomerService.DataSetSource.dsCSDListOutItem dsSource1 = new DaiCo.CustomerService.DataSetSource.dsCSDListOutItem();
      dsSource1.Tables.Add(dtReport.Clone());
      dsSource1.Tables["dtListOutItem"].Merge(dtReport);
      DaiCo.CustomerService.ReportTemplate.cptCSDListOutItem cptListOut = new DaiCo.CustomerService.ReportTemplate.cptCSDListOutItem();
      cptListOut.SetDataSource(dsSource1);
      DataTable dtval = DataBaseAccess.SearchCommandTextDataTable("Select Fontsize,[Description] from TblCSDOutputItem where Pid = " + outItemPid.ToString(), null);
      int iFontSize = 10;
      string strDescription = "";
      if (dtval.Rows.Count > 0)
      {
        strDescription = dtval.Rows[0][1].ToString();
        try
        {
          iFontSize = DBConvert.ParseInt(dtval.Rows[0][0].ToString());
        }
        catch { }
      }
      cptListOut.SetParameterValue("Title", strDescription);
      cptListOut.SetParameterValue("FontSize", iFontSize);
      Shared.View_Report frm = new View_Report(cptListOut);
      frm.ShowReport(ViewState.Window, FormWindowState.Maximized);


      //crystalReportViewer1.ReportSource = rpt;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.NeedToSave)
      {
          this.SaveData();
      }
    }

    private void viewCSD_04_005_Load(object sender, EventArgs e)
    {
      string strsql = "Select Code as Val,Value as Tex from TblBOMCodeMaster where [Group] = 2006";
      DataTable dtPrice = DataBaseAccess.SearchCommandTextDataTable(strsql, null);
      cboPrice.DataSource = dtPrice;
      cboPrice.DisplayMember = "Tex";
      cboPrice.ValueMember = "Val";
      LoadData();
      txtFont.Text = "10";
    }

    private void LoadData()
    {
      string commandText = "select ItemCode,SaleCode from VCSDItemInfo";
      ultraDropDownItemCode.DataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDropDownItemCode.ValueMember = "ItemCode";
      ultraDropDownItemCode.DisplayMember = "ItemCode";
      DBParameter[] inputParam = new DBParameter[1];
      if (this.outItemPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@pid", DbType.Int64, outItemPid);
      }
      else
      {
        inputParam[0] = new DBParameter("@pid", DbType.Int64, Int64.MinValue);
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDOutputItemInfoByPid", inputParam);

      if (this.outItemPid != long.MinValue)
      {
        
        DataTable dtParent = dsSource.Tables[0];
        txtListCode.Text = dtParent.Rows[0]["OutputCode"].ToString();
        txtDes.Text = dtParent.Rows[0]["Description"].ToString();
        txtFont.Text = dtParent.Rows[0]["Fontsize"].ToString();
        try
        {
          cboPrice.SelectedValue = dtParent.Rows[0]["BasePrice"].ToString();
        }
        catch { }
      }
      else
      {
        object obOutCode = DataBaseAccess.ExecuteScalarCommandText("Select dbo.FCSDGetNewOutputCode('CUS')", null);
        txtListCode.Text = obOutCode.ToString();
      }
      DataTable dtChild = dsSource.Tables[1];
      ultOutItemDetail.DataSource = dtChild;
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      string value = txtListCode.Text.Trim();
      if (value.Length == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "List Code");
        return false;
      }

      int fontSize = DBConvert.ParseInt(txtFont.Text.Trim());
      if (fontSize == int.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Font Size");
        return false;
      }

      value = txtDes.Text.Trim();
      if (value.Length == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Description");
        return false;
      }

      int basePrice = int.MinValue;
      if (cboPrice.SelectedIndex >= 0)
      {
        basePrice = DBConvert.ParseInt(cboPrice.SelectedValue.ToString().Trim());
      }
      if (basePrice == int.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Base Price");
        return false;
      }

      for (int i = 0; i < ultOutItemDetail.Rows.Count; i++)
      {
        int qty = DBConvert.ParseInt(ultOutItemDetail.Rows[i].Cells["Qty"].Value.ToString());
        if (qty == int.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Qty");
          return false;
        }
        double time = DBConvert.ParseDouble(ultOutItemDetail.Rows[i].Cells["Times"].Value.ToString());
        if (time == double.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Times");
          return false;
        }
      }
      return true;
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveSuccess = this.SaveData();
    }

    private bool SaveData()
    {
      if (!this.CheckValid())
      {
        return false;
      }
      bool result = true;
      string storeName = string.Empty;
      long pidParent = long.MinValue;
      // 1.Insert/update TblCSDOutputItemDetail
      DBParameter[] inputParamParent = new DBParameter[5];
      storeName = "spCSDOutputItem_Edit";
      if (outItemPid != long.MinValue) // update
      {
        inputParamParent[0] = new DBParameter("@Pid", DbType.Int64, outItemPid);
      }
      inputParamParent[4] = new DBParameter("@AdjustBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      int iFontSize = int.MinValue;
      try
      {
        iFontSize = DBConvert.ParseInt(txtFont.Text);
      }
      catch
      { }
      int iBasePrice = int.MinValue;
      try
      {
        iBasePrice = DBConvert.ParseInt(cboPrice.SelectedValue.ToString());
      }
      catch
      {}
      inputParamParent[1] = new DBParameter("@Fontsize", DbType.Int32, iFontSize);
      inputParamParent[2] = new DBParameter("@Description", DbType.String, txtDes.Text);
      inputParamParent[3] = new DBParameter("@BasePrice", DbType.Int32, iBasePrice);
      DBParameter[] outputParamParent = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamParent, outputParamParent);
      pidParent = DBConvert.ParseLong(outputParamParent[0].Value.ToString().Trim());
      if (pidParent == 0)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        result = false;
      }
      else
      {
        this.outItemPid = pidParent;
      }

      // 2.Insert/update TblCSDOutputItemDetail
      storeName = "spCSDOutputItemDetail_Edit";
      

      DataTable dsSource = (DataTable) ultOutItemDetail.DataSource;
      for (int i = 0; i < dsSource.Rows.Count; i++)
      {
        
        if (dsSource.Rows[i].RowState == DataRowState.Added || dsSource.Rows[i].RowState == DataRowState.Modified)
        {
          long pid = long.MinValue;
          DBParameter[] inputParam = new DBParameter[6];
          long lPidDetail = long.MinValue;
          try
          {
            lPidDetail = DBConvert.ParseLong(dsSource.Rows[i]["Pid"].ToString());
          }
          catch
          {

          }
          if (lPidDetail != long.MinValue) // update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, lPidDetail);
          }
          inputParam[5] = new DBParameter("@AdjustBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[1] = new DBParameter("@OutputItemPid", DbType.Int64, this.outItemPid);
          inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, dsSource.Rows[i]["ItemCode"].ToString());
          int iQty = int.MinValue;
          try
          {
            iQty = DBConvert.ParseInt(dsSource.Rows[i]["Qty"].ToString());
          }
          catch
          { }
          double dTimes = double.MinValue;
          try
          {
            dTimes = DBConvert.ParseInt(dsSource.Rows[i]["Times"].ToString());
          }
          catch
          { }
          inputParam[3] = new DBParameter("@Qty", DbType.Int32, iQty);
          inputParam[4] = new DBParameter("@Times", DbType.Double, dTimes);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          pid = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
          if (pid == 0)
          {
            WindowUtinity.ShowMessageError("ERR0005");
            result = false;
          }
          else
          {
            //this.this.outItemPid = pid;
          }
        }
      }
      //3 Delete TblCSDOutputItemDetail
      foreach (long detailPid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
        DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDOutputItemDetail_Delete", inputParamDelete, outputParamDelete);
        long outputValue = DBConvert.ParseLong(outputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      
      if (!result)
      {
        WindowUtinity.ShowMessageError("WRN0004");
        this.LoadData();
        return false;
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
      return true;
    }
    string strSaleCode = "";
    private void ultOutItemDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultOutItemDetail);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultraDropDownItemCode;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Times"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
    }

    private void ultOutItemDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
      if (string.Compare(e.Cell.Column.ToString(), "ItemCode", true) == 0)
      {
        e.Cell.Row.Cells["SaleCode"].Value = strSaleCode;
      }
    }
   
    private void ultOutItemDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (e.Cell.Text.ToString().Trim().Length > 0)
      {
        string strColName = e.Cell.Column.ToString();
        if (string.Compare(strColName, "ItemCode", true) == 0)
        {
          string ItemCode = e.Cell.Text.ToString().Trim();
          
          DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ItemCode", DbType.String, 128, ItemCode) };
          string commandText = "select Count(*) from VCSDItemInfo Where ItemCode = @ItemCode";
          object obj = DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
          int count = (int)obj;
          if (count == 0)
          {
            WindowUtinity.ShowMessageWarning("ERR0001", "ItemCode");
            e.Cancel = true;
          }
          else
          {
            object objSaleCode = DataBaseAccess.ExecuteScalarCommandText("select top 1 SaleCode from VCSDItemInfo Where ItemCode = @ItemCode", inputParam);
            strSaleCode = objSaleCode.ToString();
          }
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
    }

    private void ultOutItemDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long Pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (Pid != long.MinValue)
        {
          listDeletingPid.Add(Pid);
        }
      }
    }

    private void ultOutItemDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    private void btnInsertExcel_Click(object sender, EventArgs e)
    {
      try
      {
        string strtmp = Clipboard.GetText();
        strtmp = strtmp.Replace("\r\n", string.Format("{0}", (char)16));
        strtmp = strtmp.Replace("\t", string.Format("{0}", (char)15));
        DataTable dt = (DataTable)ultOutItemDetail.DataSource;
        bool chkInsert = false;
        string[] a;
        string[] b;
        b = strtmp.Split((char)16);
        for (int i = 0; i < b.Length - 1; i++)
        {
          DataRow dr = dt.NewRow();
          a = b.GetValue(i).ToString().Split((char)15);
          for (int j = 0; j < a.Length; j++)
          {
            if (a[j].ToString() == "")
            {
              dr[j] = DBNull.Value;
            }
            else
            {
              if (dt.Columns[j].DataType == typeof(DateTime) || dt.Columns[j].DataType == typeof(Int64) || dt.Columns[j].DataType == typeof(Double) || dt.Columns[j].DataType == typeof(Int32) || dt.Columns[j].DataType == typeof(float))
              {
                a.SetValue(a.GetValue(j).ToString().Replace(",", ""), j);
              }
              if ((dt.Columns[j].DataType == typeof(DateTime) || dt.Columns[j].DataType == typeof(Int32) || dt.Columns[j].DataType == typeof(Int64) || dt.Columns[j].DataType == typeof(Double) || dt.Columns[j].DataType == typeof(float)) && (a.GetValue(j).ToString() == "NULL" || a.GetValue(j).ToString() == ""))
              {
                dr[j + 1] = DBNull.Value;
              }
              else
              {
                if (j == 0)
                {
                  DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ItemCode", DbType.String, 128, a.GetValue(j).ToString()) };
                  DataTable dtcheck = DataBaseAccess.SearchCommandTextDataTable("select Distinct top 1 ItemCode, SaleCode from VCSDItemInfo Where ItemCode = @ItemCode", inputParam);
                  if (dtcheck.Rows[0][0].ToString() != "")
                  {
                    dr[1] = dtcheck.Rows[0][0].ToString();
                    dr[2] = dtcheck.Rows[0][1].ToString();
                  }
                }
                else
                {
                  dr[j + 2] = a.GetValue(j).ToString();
                }
              }
            }
          }
          dt.Rows.Add(dr);
        }
        ultOutItemDetail.DataSource = dt;
        this.NeedToSave = true;
      }
      catch { }
    }

  }
}
