/*
  Author      : Đỗ Minh Tâm
  Date        : 16/11/2010
  Decription  : Print price list
  Update by   : Nguyễn Văn trọn
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
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.IO;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_003 : MainUserControl
  {
    #region function
    private bool CheckInvalid()
    {
      if (ucActive.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Select Active");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Get Icon for Item Kind
    /// </summary>
    /// <param name="itemKind">1: Special Order, 2: USQS, 3: UKQS, 4: Standard</param>
    /// <returns></returns>
    private byte[] GetItemKindIcon(int itemKind)
    {
      switch (itemKind)
      {
        case 1:
          return FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.SpecialOrder);          
        case 2:
          return FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.USQS);          
        case 3:
          return FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.UKQS);          
        case 4:
          return FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.Standard);          
        default:
          break;
      }
      return null;
    }

    private void ShowReport()
    {
      if (this.CheckInvalid())
      {
        long iActiveType = long.MinValue;
        string strActiveType = string.Empty;
        int iStatusItem = int.MinValue;
        string strStatusItem = string.Empty;
        // Customer Active
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

        if (rdFullDes.Checked)
        {
          DBParameter[] inputParam = new DBParameter[3];
          int iGroup;
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

          inputParam[0] = new DBParameter("@ActiveType", DbType.Int64, iActiveType);
          inputParam[1] = new DBParameter("@Group", DbType.Int32, iGroup);
          inputParam[2] = new DBParameter("@StatusItem", DbType.Int32, iStatusItem);

          DBParameter[] outputParam = new DBParameter[2];
          outputParam[0] = new DBParameter("@Footer", DbType.String, 4000, "");
          outputParam[1] = new DBParameter("@TimeOfPricelist", DbType.AnsiString, 64, "");

          DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDReportFullPriceList", 600, inputParam, outputParam);

          string strFooter = outputParam[0].Value.ToString().Trim();
          string strTime = outputParam[1].Value.ToString();
          dtSource.Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dtSource.Columns.Add("CheckImage", System.Type.GetType("System.Int32"));
          dtSource.Columns.Add("ItemKindIcon", System.Type.GetType("System.Byte[]"));
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            dtSource.Rows[i]["Image"] = FunctionUtility.ImageToByteArrayWithFormat(@dtSource.Rows[i]["img"].ToString(), 380, 1.55, "JPG");
            dtSource.Rows[i]["CheckImage"] = (dtSource.Rows[i]["Image"].ToString().Length > 0 ? 1 : 0);
            int itemKind = DBConvert.ParseInt(dtSource.Rows[i]["ItemKind"].ToString());
            dtSource.Rows[i]["ItemKindIcon"] = this.GetItemKindIcon(itemKind);            
          }
          DaiCo.CustomerService.DataSetSource.dsCSDFullDescriptionForItem dsSource = new DaiCo.CustomerService.DataSetSource.dsCSDFullDescriptionForItem();

          dsSource.Tables["dtFullDesForItem"].Merge(dtSource);

          DaiCo.CustomerService.ReportTemplate.cptCSDFullDescriptionForItem cptFullDescriptionForItem = new DaiCo.CustomerService.ReportTemplate.cptCSDFullDescriptionForItem();
          cptFullDescriptionForItem.SetDataSource(dsSource);
          string strTitle = string.Format("\"DETAILED\" WHOLE SALE PRICE LIST {0}\nIN ORDER OF \"{1}\"\n({2}) ALL \"{3}\" ITEM", strTime, strGroup, strActiveType, strStatusItem);
          cptFullDescriptionForItem.SetParameterValue("Title", strTitle.ToUpper());
          cptFullDescriptionForItem.SetParameterValue("Description", strFooter);                    
          cptFullDescriptionForItem.SetParameterValue("MoreDescription", string.Empty);
          cptFullDescriptionForItem.SetParameterValue("TimeOfPrice", string.Empty);
          cptFullDescriptionForItem.SetParameterValue("CusAdd", string.Empty);

          if (rdCode.Checked)
          {
            cptFullDescriptionForItem.GroupHeaderSection1.SectionFormat.EnableSuppress = true;
          }
          // Export report
          ControlUtility.ViewCrystalReport(cptFullDescriptionForItem);          
        }
        else
        {
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@ActiveType", DbType.Int64, iActiveType);
          inputParam[1] = new DBParameter("@StatusItem", DbType.Int32, iStatusItem);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@TimeOfPricelist", DbType.AnsiString, 64, "");

          DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDReportShortPriceList", 600, inputParam, outputParam);
          dtSource.Columns.Add("ItemKindIcon", System.Type.GetType("System.Byte[]"));
          string strTime = outputParam[0].Value.ToString().Trim();

          // Add Item Kind Icon
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            int itemKind = DBConvert.ParseInt(dtSource.Rows[i]["ItemKind"].ToString());
            dtSource.Rows[i]["ItemKindIcon"] = this.GetItemKindIcon(itemKind);
          }

          DaiCo.CustomerService.DataSetSource.dsCSDFullDescriptionForItem dsSource = new DaiCo.CustomerService.DataSetSource.dsCSDFullDescriptionForItem();
          dsSource.Tables["dtShortDesForItem"].Merge(dtSource);

          DaiCo.CustomerService.ReportTemplate.cptCSDPriceForItem cptPrice = new DaiCo.CustomerService.ReportTemplate.cptCSDPriceForItem();
          string strTitle = string.Format("\"SHORT\" WHOLE SALE PRICE LIST {0}\nIN ORDER OF \"CODE\"\n({1}) ALL \"{2}\" ITEM", strTime, strActiveType, strStatusItem);
          cptPrice.SetDataSource(dsSource);
          cptPrice.SetParameterValue("Title", strTitle.ToUpper());
          cptPrice.SetParameterValue("MoreDescription", string.Empty);
          cptPrice.SetParameterValue("TimeOfPrice", string.Empty);
          cptPrice.SetParameterValue("CusAdd", string.Empty);

          // Export report
          ControlUtility.ViewCrystalReport(cptPrice);
        }
      }
    }
    #endregion function
    public viewCSD_04_003()
    {
      InitializeComponent();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      tableLayoutPrintReport.Enabled = false;
      this.ShowReport();
      tableLayoutPrintReport.Enabled = true;
    }

    private void btnCustom_Click(object sender, EventArgs e)
    {
      this.CloseTab();
      viewCSD_04_007 view = new viewCSD_04_007();
      WindowUtinity.ShowView(view, "LIST PRICE PRINTING", false, ViewState.Window);     
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void radio_CheckedChanged(object sender, EventArgs e)
    {
      if (rdShortDes.Checked)
      {
        rdCategory.Enabled = false;
        rdCollection.Enabled = false;
        rdCode.Enabled = false;
        rdCode.Checked = true;
      }
      if (rdFullDes.Checked)
      {
        rdCategory.Enabled = true;
        rdCollection.Enabled = true;
        rdCode.Enabled = true;
      }
    }

    private void viewCSD_04_003_Load(object sender, EventArgs e)
    {
      string strsql = "	SELECT CSI.Pid, CSI.CustomerCode, CSI.Name, CSI.CustomerCode + ' - ' + CSI.Name DisplayText"
                    + " FROM (SELECT DISTINCT CUSTOMERPID FROM TblCSDItemActiveInfomation WHERE CUSTOMERPID = 11 OR CUSTOMERPID = 12) ACT "
                    + " 	INNER JOIN TblCSDCustomerInfo CSI ON ACT.CustomerPid = CSI.Pid";
      DataTable dtCus = DataBaseAccess.SearchCommandTextDataTable(strsql, null);
      ucActive.DataSource = dtCus;
      ucActive.ValueMember = "Pid";
      ucActive.DisplayMember = "DisplayText";
      ucActive.Rows.Band.Columns["Pid"].Hidden = true;
      ucActive.Rows.Band.Columns["DisplayText"].Hidden = true;
      ucActive.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ucActive.DisplayLayout.Bands[0].Columns["CustomerCode"].Width = 50;
      try
      {
        ucActive.SelectedRow = ucActive.Rows[0];
      }
      catch { }

    }
  }
}
