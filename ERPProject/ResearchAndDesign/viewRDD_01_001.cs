using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewRDD_01_001 : MainUserControl
  {
    public viewRDD_01_001()
    {
      InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void UC_RDDListItemInfo_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserPid + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;
      this.LoadStatus();
      Utility.LoadItemImageFolder(ultraCBItemImageFolder);
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
      Utility.LoadUltraCombo(ultraCBStatus, dtStatus, "Value", "Text", "Value");
      ultraCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void Search()
    {
      btnSearch.Enabled = false;
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
        inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 64, "%" + saleCode + "%");
      }
      if (carcassCode.Length > 0)
      {
        inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, "%" + carcassCode + "%");
      }
      if (oldCode.Length > 0)
      {
        inputParam[3] = new DBParameter("@OldCode", DbType.AnsiString, 32, "%" + oldCode + "%");
      }
      if (ultraCBStatus.Value != null)
      {
        inputParam[4] = new DBParameter("@Status", DbType.Int32, ultraCBStatus.Value);
      }
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spRDDListItemInfo", inputParam);
      ultraGridItemList.DataSource = dtSource;
      lbCount.Text = string.Format("Count: {0}", ultraGridItemList.Rows.Count);
      btnSearch.Enabled = true;
    }

    private void btnNewItem_Click(object sender, EventArgs e)
    {
      viewRDD_01_002 objItemMaster = new viewRDD_01_002();
      Shared.Utility.WindowUtinity.ShowView(objItemMaster, "Item Master 1st", false, Shared.Utility.ViewState.ModalWindow);
      this.Search();
      this.Show();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnGetData_Click(object sender, EventArgs e)
    {
      bool successful = true;
      DataTable dtItemList = (DataTable)ultraGridItemList.DataSource;
      DataRow[] itemRows = dtItemList.Select("Selected = 1");
      for (int i = 0; i < itemRows.Length; i++)
      {
        string itemCode = itemRows[i]["ItemCode"].ToString();
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spRDDCopyItemInfoToBOM", inputParam, outputParam);
        int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
        if (result != 1)
        {
          MessageBox.Show("Error happen When Copy The Item " + itemCode);
          successful = false;
        }

        // Begin Auto Copy Image Item (Tron update 17/07/2012)
        string rddImage = FunctionUtility.RDDGetItemImage(itemCode);
        string techPath = FunctionUtility.GetImagePathByPid(1);
        string techImage = string.Format(@"{0}\{1}-{2}.jpg", techPath, itemCode, "01");
        if (File.Exists(rddImage) && !File.Exists(techImage))
        {
          File.Copy(rddImage, techImage);
        }
        // End Auto Copy Image Item (Tron update 17/07/2012)
      }
      if (itemRows.Length == 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0014");
      }
      else if (successful)
      {
        WindowUtinity.ShowMessageSuccess("MSG0022");
      }
      this.Search();
    }

    private void btnUnlock_Click(object sender, EventArgs e)
    {
      DataTable dtItemList = (DataTable)ultraGridItemList.DataSource;
      DataRow[] itemRows = dtItemList.Select("Selected = 1");
      for (int i = 0; i < itemRows.Length; i++)
      {
        string itemCode = itemRows[i]["ItemCode"].ToString();
        Shared.Utility.FunctionUtility.UnlockRDDItemInfo(itemCode);
      }
      if (itemRows.Length == 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0014");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0023");
      }
      this.Search();
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void ultraGridItemList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Confirm"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["MainFinish"].Header.Caption = "Main Finishing";

      //Set columns width
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 100;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Selected"].CellActivation = Activation.AllowEdit;

      // Set color
      for (int i = 0; i < e.Layout.Rows.Count; i++)
      {
        int confirm = DBConvert.ParseInt(e.Layout.Rows[i].Cells["Confirm"].Value);
        e.Layout.Rows[i].Cells["Selected"].Activation = Activation.ActivateOnly;
        if (confirm == 1)
        {
          e.Layout.Rows[i].Appearance.BackColor = Color.Pink;
          e.Layout.Rows[i].Cells["Selected"].Activation = Activation.AllowEdit;
        }
        else if (confirm == 2)
        {
          e.Layout.Rows[i].Appearance.BackColor = Color.Yellow;
        }
      }
    }

    private void ultraGridItemList_DoubleClick(object sender, EventArgs e)
    {
      try
      {
        string itemCode = ultraGridItemList.Selected.Rows[0].Cells["ItemCode"].Value.ToString();
        if (chkShowFolder.Checked)
        {
          if (ultraCBItemImageFolder.SelectedRow != null)
          {
            string outFolder = string.Format("{0}\\{1}", ultraCBItemImageFolder.SelectedRow.Cells["Value"].Value.ToString(), itemCode);
            try
            {
              Process.Start(outFolder);
            }
            catch
            {
              WindowUtinity.ShowMessageError("ERR0029", "Item Folder");
            }
          }
          else
          {
            WindowUtinity.ShowMessageErrorFromText("Please choose item image folder.");
          }
        }
        else
        {
          viewRDD_01_002 objItemMaster = new viewRDD_01_002();
          objItemMaster.itemCode = itemCode;
          Shared.Utility.WindowUtinity.ShowView(objItemMaster, objItemMaster.itemCode, true, Shared.Utility.ViewState.Window, FormWindowState.Maximized);
        }
      }
      catch { }
    }

    private void ultraGridItemList_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultraGridItemList, grpItemPicture, picItem, chkShowItem.Checked);
    }

    private void chkShowItem_CheckedChanged(object sender, EventArgs e)
    {
      grpItemPicture.Visible = chkShowItem.Checked;
      Utility.BOMShowItemImage(ultraGridItemList, grpItemPicture, picItem, chkShowItem.Checked);
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtSaleCode.Text = string.Empty;
      txtItemCode.Text = string.Empty;
      txtOldCode.Text = string.Empty;
      txtCarcassCode.Text = string.Empty;
      ultraCBStatus.Value = null;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultraGridItemList, "Item List");
    }
  }
}
