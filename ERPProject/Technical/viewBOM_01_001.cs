using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_001 : MainUserControl
  {
    public viewBOM_01_001()
    {
      InitializeComponent();
    }

    private void viewBOM_01_001_Load(object sender, EventArgs e)
    {

      //Load data for combobox WO
      string commandText = "Select Pid From TblPLNWorkOrder";
      DataTable dtWO = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO != null)
      {
        Utility.LoadUltraCombo(ultraCBWO, dtWO, "Pid", "Pid");
      }

      // Load data for combobox Customer
      Utility.LoadUltraCBCustomer(ultraCBCustomer);
      // Load data for combobox Collection
      Utility.LoadUltraCBCollection(ultCBCollection);
      Utility.LoadItemImageFolder(ultraCBItemImageFolder);
      Utility.LoadUltraCBCategory(ultraCBCategory);
    }

    private void Search()
    {
      btnSearch.Enabled = false;
      if (this.CheckInvalid())
      {
        string itemCode = txtItemCode.Text.Trim();
        string saleCode = txtSaleCode.Text.Trim();
        string itemName = txtItemName.Text.Trim();
        string carcassCode = txtCarcassCode.Text.Trim();
        string component = txtComponent.Text.Trim();

        DBParameter[] inputParam = new DBParameter[10];
        if (itemCode.Length > 0)
        {
          inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode.Replace("'", "''") + "%");
        }
        if (saleCode.Length > 0)
        {
          inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 64, "%" + saleCode.Replace("'", "''") + "%");
        }
        if (carcassCode.Length > 0)
        {
          inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, "%" + carcassCode.Replace("'", "''") + "%");
        }
        if (ultraCBWO.SelectedRow != null)
        {
          inputParam[3] = new DBParameter("@WO", DbType.Int32, ultraCBWO.Value);
        }
        if (itemName.Length > 0)
        {
          inputParam[4] = new DBParameter("@ItemName", DbType.String, 512, "%" + itemName + "%");
        }
        if (component.Length > 0)
        {
          inputParam[5] = new DBParameter("@Component", DbType.AnsiString, 34, "%" + component.Replace("'", "''") + "%");
        }
        if (chkActiveRevision.Checked)
        {
          inputParam[6] = new DBParameter("@ActiveRevision", DbType.Int32, 1);
        }
        if (ultraCBCustomer.SelectedRow != null)
        {
          inputParam[7] = new DBParameter("@CustomerPid", DbType.Int64, ultraCBCustomer.Value);
        }
        if (ultCBCollection.SelectedRow != null)
        {
          inputParam[8] = new DBParameter("@Collection", DbType.Int32, ultCBCollection.Value);
        }
        if (ultraCBCategory.SelectedRow != null)
        {
          inputParam[9] = new DBParameter("@Category", DbType.Int64, ultraCBCategory.Value);
        }
        DataTable dtItemList = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMListItemInfo", 300, inputParam);
        ultData.DataSource = dtItemList;
        lbCountItem.Text = string.Format("Count: {0}", dtItemList.Rows.Count);

        // Change color the items confirmed
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          //string confirm = dgvItemList.Rows[i].Cells["Status"].Value.ToString();
          string confirm = ultData.Rows[i].Cells["Status"].Value.ToString();
          if (string.Compare(confirm, "Confirmed", true) == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.LightGray;
          }
        }
      }
      btnSearch.Enabled = true;
    }

    private void UnlockItem()
    {
      bool success = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int select = 0;
        try
        {
          select = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value);
        }
        catch { }
        if (select == 1)
        {
          string itemCode = ultData.Rows[i].Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString());

          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
          inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.SearchStoreProcedure("spBOMItemInfo_Unlock", inputParam, outputParam);
          if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
          //string commandUnlockItem = string.Format("Update TblBOMItemInfo Set Confirm = 0, UpdateBy = {0}, UpdateDate = '{1}' Where ItemCode = '{2}' And Revision = {3}", Shared.Utility.SharedObject.UserInfo.UserPid, DateTime.Now, itemCode, revision);
          //if (!Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText(commandUnlockItem))
          {
            success = false;
          }
        }
      }
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0053", "some item");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0023");
      }
      this.Search();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnUnlock_Click(object sender, EventArgs e)
    {
      this.UnlockItem();
    }

    private void ultraCBWO_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }

    //Truong_Update(07/11/2011)
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }
    /// <summary>
    /// Check In Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      if (ultraCBWO.Text.Trim().Length > 0 && ultraCBWO.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "WO");
        return false;
      }
      if (ultraCBCustomer.Text.Trim().Length > 0 && ultraCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        return false;
      }
      return true;
    }

    private void chkShowWO_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowWO.Checked)
      {
        grShowWO.Visible = true;
      }
      else
      {
        grShowWO.Visible = false;
      }
    }

    private void ultShowWO_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      // Set Width
      e.Layout.Bands[0].Columns["WO"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["WO"].MinWidth = 50;
    }

    private void ShowListWorkOder()
    {
      if (chkShowWO.Checked)
      {
        UltraGridRow row = ultData.Selected.Rows[0];
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultData.Selected.Rows[0].Cells["ItemCode"].Value);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, ultData.Selected.Rows[0].Cells["Revision"].Value);
        DataTable dtWO = DataBaseAccess.SearchStoreProcedureDataTable("spBOMGetWOListByItemCode", inputParam);

        Point xy = new Point();
        int yMax = ultData.Location.Y + ultData.Height;
        xy.X = ultData.Location.X + row.Cells["ItemCode"].Width + row.Cells["Revision"].Width + 250;
        xy.Y = ultData.Location.Y + (row.Cells["ItemCode"].Height * (row.Index + 2));
        if (xy.Y + grShowWO.Height > yMax)
        {
          xy.Y = yMax - grShowWO.Height;
        }
        grShowWO.Location = xy;
        if (dtWO != null)
        {
          grShowWO.Text = "ItemCode: " + ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString();
          ultShowWO.DataSource = dtWO;//
          ultShowWO.Visible = true;
        }
        else
        {
          grShowWO.Text = string.Empty;
          ultShowWO.DataSource = null;
        }
      }
    }

    private void ultraCBCustomer_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Display"].Hidden = true;
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Code"].MaxWidth = 100;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        Utility.ExportToExcelWithDefaultPath(ultData, "Item List");
      }
    }

    private void chkShowItem_CheckedChanged(object sender, EventArgs e)
    {
      grpItemPicture.Visible = chkShowItem.Checked;
      Utility.BOMShowItemImage(ultData, grpItemPicture, picItem, chkShowItem.Checked);
    }

    private void btnPrintCheckList_Click(object sender, EventArgs e)
    {
      int revision = DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["Revision"].Value.ToString());
      string itemCode = DBConvert.ParseString(ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString());
      Shared.Utility.FunctionUtility.ViewReportCheckListForQC(itemCode, revision);
      Shared.Utility.FunctionUtility.ViewReportCheckListForQC(itemCode, revision);
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ultData);
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitStyle = AutoFitStyle.None;

      e.Layout.Bands[0].Columns["Selected"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Set column header
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Item Name";

      //Hide column
      e.Layout.Bands[0].Columns["NameVN"].Hidden = true;      
      e.Layout.Bands[0].Columns["Category"].Hidden = true;
      e.Layout.Bands[0].Columns["Collection"].Hidden = true;

      //Set width
      e.Layout.Bands[0].Columns["ItemCode"].Width = 80;
      e.Layout.Bands[0].Columns["SaleCode"].Width = 100;
      e.Layout.Bands[0].Columns["OldCode"].Width = 90;
      e.Layout.Bands[0].Columns["CarcassCode"].Width = 90;
      e.Layout.Bands[0].Columns["Status"].Width = 85;
      e.Layout.Bands[0].Columns["Selected"].Width = 80;
      e.Layout.Bands[0].Columns["Description"].Width = 150;
      e.Layout.Bands[0].Columns["Cus.Name"].Width = 150;
      e.Layout.Bands[0].Columns["Name"].Width = 150;
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, grpItemPicture, picItem, chkShowItem.Checked);
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      try
      {
        if (chkShowFolder.Checked)
        {
          if (ultraCBItemImageFolder.SelectedRow != null)
          {
            string outFolder = string.Format("{0}\\{1}", ultraCBItemImageFolder.SelectedRow.Cells["Value"].Value.ToString(), ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString());
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

          viewBOM_01_002 objItemMaster = new viewBOM_01_002();
          objItemMaster.itemCode = ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString();
          objItemMaster.revision = DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["Revision"].Value.ToString());
          Shared.Utility.WindowUtinity.ShowView(objItemMaster, "Item Master 1st Level", false, ViewState.Window, FormWindowState.Maximized);
        }
      }
      catch { }
    }

    private void ultData_Click(object sender, EventArgs e)
    {
      if (chkShowWO.Checked)
      {
        this.ShowListWorkOder();
      }
    }
  }
}
