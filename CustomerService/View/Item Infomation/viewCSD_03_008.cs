using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System.IO;
using DaiCo.Shared.ReportTemplate.CustomerService;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_008 : MainUserControl
  {
    #region fields
    
    #endregion fields

    #region function
    private void Search()
    {
      string itemCode = txtItemCode.Text.Trim();
      string saleCode = txtSaleCode.Text.Trim();
      string carcassCode = txtCarcassCode.Text.Trim();
      string oldCode = txtOldCode.Text.Trim();
      DBParameter[] inputParam = new DBParameter[5];
      if (itemCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode + "%");
      }
      if (saleCode.Length > 0)
      {
        inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode + "%");
      }
      if (carcassCode.Length > 0)
      {
        inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, "%" + carcassCode + "%");
      }
      if (oldCode.Length > 0)
      {
        inputParam[3] = new DBParameter("@OldCode", DbType.AnsiString, 16, "%" + oldCode + "%");
      }
      if (ultraCBStatus.Value != null)
      {
        inputParam[4] = new DBParameter("@Status", DbType.Int32, ultraCBStatus.Value);
      }
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spRDDListItemInfo", inputParam);
      dtSource.Columns.Add("Select", typeof(System.Int32));      
      foreach (DataRow row in dtSource.Rows)
      {
        row["Select"] = 0;
      }
      ultraGridInformation.DataSource = dtSource;      
    }

    private void PrintDataSheet()
    {
      DataSet dsDataSheet = new Shared.DataSetSource.CustomerService.dsCSDSampleDataSheet();
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        int select = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Select"].Value.ToString());
        if (select == 1)
        {
          string itemCode = ultraGridInformation.Rows[i].Cells["ItemCode"].Value.ToString();
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spRDDSampleDataSheetReport", inputParam);
          dsSource.Tables[0].Columns.Add("Picture", typeof(System.Byte[]));
          dsSource.Tables[1].Columns.Add("ComponentPicture", typeof(System.Byte[]));
          dsSource.Tables[2].Columns.Add("ComponentPicture", typeof(System.Byte[]));

          if (dsSource.Tables[0].Rows.Count > 0)
          {
            string imgPath = FunctionUtility.RDDGetItemImage(itemCode);
            dsSource.Tables[0].Rows[0]["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.02, "JPG");
          }

          foreach (DataRow rowGlass in dsSource.Tables[1].Rows)
          {
            try
            {
              string imgPath = FunctionUtility.BOMGetItemComponentImage(rowGlass["ComponentCode"].ToString().Trim());
              FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
              BinaryReader br = new BinaryReader(fs);
              byte[] imgbyte = new byte[fs.Length + 1];
              imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
              rowGlass["ComponentPicture"] = imgbyte;
              br.Close();
              fs.Close();
            }
            catch { }
          }

          foreach (DataRow row in dsSource.Tables[2].Rows)
          {
            try
            {

              string imgPath = FunctionUtility.BOMGetItemComponentImage(row["ComponentCode"].ToString().Trim());
              FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
              BinaryReader br = new BinaryReader(fs);
              byte[] imgbyte = new byte[fs.Length + 1];
              imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
              row["ComponentPicture"] = imgbyte;
              br.Close();
              fs.Close();
            }
            catch { }
          }

          if (dsSource.Tables.Count >= 3)
          {
            dsDataSheet.Tables["dtItemInfo"].Merge(dsSource.Tables[0]);
            dsDataSheet.Tables["dtItemGlass"].Merge(dsSource.Tables[1]);
            dsDataSheet.Tables["dtItemDetail"].Merge(dsSource.Tables[2]);
          }
          string commandText = string.Format(@"SELECT DMS.ItemCode, DimensionKind, MST.[Description], DMS.[Values] mm, dbo.FCSDConvert_mm_to_inches(DMS.[Values]) Inches
	                                            FROM TblRDDMoreDimension DMS
		                                            LEFT JOIN TblBOMCodeMaster MST ON (DMS.DimensionKind = MST.Code AND MST.[Group] = 5)
	                                            WHERE ItemCode = @ItemCode");
          DataTable dtMoreDimention = DataBaseAccess.SearchCommandTextDataTable(commandText, inputParam);
          if (dtMoreDimention != null && dtMoreDimention.Rows.Count > 0)
          {
            dsDataSheet.Tables["dtMoreDimention"].Merge(dtMoreDimention);
          }
        }
      }      
      
      //ReportClass cpt = null;
      DaiCo.Shared.View_Report report = null;

      cptCSDSampleDataSheet cpt = new cptCSDSampleDataSheet();
      cpt.SetDataSource(dsDataSheet);
      cpt.Subreports["cptCSDSubGlass.rpt"].SetDataSource(dsDataSheet.Tables["dtItemGlass"]);
      cpt.Subreports["cptCSDSubComponent.rpt"].SetDataSource(dsDataSheet.Tables["dtItemDetail"]);
      cpt.Subreports["cptSubOtherDimention.rpt"].SetDataSource(dsDataSheet.Tables["dtMoreDimention"]);

      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.Window, FormWindowState.Maximized);
    }

    private void Object_Change(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void LoadStatus()
    {
      // Load UltraCBStatus
      DataTable dtStatus = new DataTable();
      dtStatus.Columns.Add("Value");
      dtStatus.Columns.Add("Text");
      DataRow row1 = dtStatus.NewRow();
      row1["Value"] = 0;
      row1["Text"] = "Not Confirmed";
      dtStatus.Rows.Add(row1);
      DataRow row2 = dtStatus.NewRow();
      row2["Value"] = 1;
      row2["Text"] = "Confirmed";
      dtStatus.Rows.Add(row2);
      DataRow row3 = dtStatus.NewRow();
      row3["Value"] = 2;
      row3["Text"] = "Loaded";
      dtStatus.Rows.Add(row3);
      ControlUtility.LoadUltraCombo(ultraCBStatus, dtStatus, "Value", "Text", "Value");
      ultraCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    #endregion function

    #region event
    public viewCSD_03_008()
    {
      InitializeComponent();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Confirm"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["OldCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["OldCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["MainFinish"].Header.Caption = "Main Finishing";
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Select"].MaxWidth = 50;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }

    private void btnDataSheet_Click(object sender, EventArgs e)
    {
      btnDataSheet.Enabled = false;
      this.PrintDataSheet();
      btnDataSheet.Enabled = true;
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      int select = (chkSelectAll.Checked ? 1 : 0);
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        ultraGridInformation.Rows[i].Cells["Select"].Value = select;
      }
    }

    private void viewCSD_03_008_Load(object sender, EventArgs e)
    {
      this.LoadStatus();
    }
    #endregion event
  }
}
