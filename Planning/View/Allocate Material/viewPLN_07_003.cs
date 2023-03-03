/*
 * Created By   : Nguyen Van Tron
 * Created Date : 02/02/2011
 * Description  : Planning Materials
 * Truong Update: Thay Doi Control Item, Material Group
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_003 : MainUserControl
  {
    #region function
    private void LoadDataForItemListView()
    {
      // Truong Add
      txtItemCode.Text = string.Empty;
      string woPids = string.Empty;
      woPids = txtWO.Text;
      string commandText = string.Empty;
      woPids = woPids.Replace(" ", "").Replace("'", "");
      // Load ListView Item
      if (woPids.Length > 0)
      {
        commandText = string.Format("Select DISTINCT ITEM.ItemCode, ITEM.Name ItemName From TblBOMItemBasic ITEM INNER JOIN TblPLNWOInfoDetailGeneral WO On ITEM.ItemCode = WO.ItemCode And ';{0};' LIKE ('%;' + CAST(WO.WoInfoPID as varchar) + ';%')", woPids);
      }
      else
      {
        commandText = "Select DISTINCT ITEM.ItemCode, ITEM.Name ItemName From TblBOMItemBasic ITEM INNER JOIN TblPLNWOInfoDetailGeneral WO On ITEM.ItemCode = WO.ItemCode";
      }
      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucUltraListItem.DataSource = dtItem;
      ucUltraListItem.ColumnWidths = "100; 200";
      ucUltraListItem.DataBind();
      ucUltraListItem.ValueMember = "ItemCode";
    }

    private string GetSelectedListView(ListView lst)
    {
      string result = string.Empty;
      foreach (ListViewItem item in lst.Items)
      {
        if (result.Length > 0)
        {
          result += "; ";
        }
        result += item.Text;
      }
      return result;
    }

    private void Search()
    {
      btnSearch.Enabled = false;
      DBParameter[] inputParam = new DBParameter[4];
      if (txtWO.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@Wos", DbType.AnsiString, 4000, txtWO.Text.Trim());
      }
      else
      {
        btnSearch.Enabled = true;
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Work Order");
        return;
      }
      if (txtItemCode.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@ItemCodes", DbType.AnsiString, 4000, txtItemCode.Text.Trim());
      }
      if (txtMaterialGroup.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text.Trim());
      }
      int dataChange = chkDataChange.Checked ? 1 : 0;
      inputParam[3] = new DBParameter("@DataChange", DbType.Int32, dataChange);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMaterialPlanning_Select", inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;
      btnSearch.Enabled = true;
    }

    private bool SaveData()
    {
      bool success = true;
      DataTable dtSource = (DataTable)ultraGridWOMaterialDetail.DataSource;
      if (dtSource != null)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState != DataRowState.Deleted && DBConvert.ParseInt(row["Selected"].ToString()) == 1)
          {
            DBParameter[] inputParam = new DBParameter[6];
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            string storeName = string.Empty;
            double consQty = DBConvert.ParseDouble(row["ConsExist"].ToString());
            double newQty = DBConvert.ParseDouble(row["NewCons"].ToString());
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            int userPid = Shared.Utility.SharedObject.UserInfo.UserPid;
            if (consQty == double.MinValue && newQty != double.MinValue) //New
            {
              storeName = "spPLNWOMaterialsPlanning_Insert";
              inputParam[0] = new DBParameter("@WoPid", DbType.Int64, row["Wo"]);
              inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, row["ItemCode"]);
              inputParam[2] = new DBParameter("@Revision", DbType.Int32, row["Revision"]);
              inputParam[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, row["MaterialCode"]);
              inputParam[4] = new DBParameter("@Qty", DbType.Double, row["NewCons"]);
              inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, userPid);
            }
            else if (consQty != double.MinValue && newQty == double.MinValue) //Delete
            {
              storeName = "spPLNWOMaterialsPlanning_Delete";
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            else if (consQty != newQty) //Change
            {
              storeName = "spPLNWOMaterialsPlanning_Update";
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
              inputParam[1] = new DBParameter("@Qty", DbType.Double, row["NewCons"]);
              inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, userPid);
            }
            if (storeName.Length > 0)
            {
              DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
              if (DBConvert.ParseLong(outputParam[0].Value.ToString()) == 0)
              {
                success = false;
              }
            }
          }
        }
      }
      return success;
    }
    #endregion function

    #region event
    public viewPLN_07_003()
    {
      InitializeComponent();
    }

    private void viewPLN_07_003_Load(object sender, EventArgs e)
    {
      // Truong Add
      this.LoadDataForItemListView();
      ControlUtility.LoaducUltraListMaterialGroup(ucUltraListMaterialGroup);
    }

    private void chkShowItemListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListItem.Visible = chkShowItemListBox.Checked;
    }

    private void chkShowMaterialListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterialGroup.Visible = chkShowMaterialListBox.Checked;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridWOMaterialDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridWOMaterialDetail);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Wo"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["ItemQty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["ItemQty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["ItemQty"].Header.Caption = "Item Qty";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["ConsExist"].MinWidth = 60;
      e.Layout.Bands[0].Columns["ConsExist"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["ConsExist"].Header.Caption = "Cons Exist";
      e.Layout.Bands[0].Columns["NewCons"].MinWidth = 60;
      e.Layout.Bands[0].Columns["NewCons"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["NewCons"].Header.Caption = "New Cons";
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 50;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Selected"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      DataTable dtSource = (DataTable)ultraGridWOMaterialDetail.DataSource;
      if (dtSource != null)
      {
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          UltraGridRow row = ultraGridWOMaterialDetail.Rows[i];
          double consQty = DBConvert.ParseDouble(row.Cells["ConsExist"].Value.ToString());
          double newQty = DBConvert.ParseDouble(row.Cells["NewCons"].Value.ToString());
          if (consQty == double.MinValue && newQty != double.MinValue) //New
          {
            row.Appearance.BackColor = Color.LightBlue;
          }
          else if (consQty != double.MinValue && newQty == double.MinValue) //Delete
          {
            row.Appearance.BackColor = Color.Pink;
          }
          else if (consQty != newQty) //Change
          {
            row.Appearance.BackColor = Color.Yellow;
          }
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      btnSave.Enabled = false;
      if (this.SaveData())
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("WRN0004");
      }
      this.Search();
      btnSave.Enabled = true;
    }

    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = 0;
      if (chkSelectedAll.Checked)
      {
        selected = 1;
      }
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        ultraGridWOMaterialDetail.Rows[i].Cells["Selected"].Value = selected;
      }
    }
    #endregion event

    private void btnPrint_Click(object sender, EventArgs e)
    {
      string strTemplateName = "PlanningReport";
      string strSheetName = "PlanningMaterial";
      string strOutFileName = "Planning Material Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Search :
      //DBParameter[] inputParam = new DBParameter[4];
      //if (txtWO.Text.Trim().Length > 0)
      //{
      //  inputParam[0] = new DBParameter("@Wos", DbType.AnsiString, 4000, txtWO.Text.Trim());
      //}
      //else
      //{
      //  Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Work Order");
      //  return;
      //}
      //if (txtItemCode.Text.Trim().Length > 0)
      //{
      //  inputParam[1] = new DBParameter("@ItemCodes", DbType.AnsiString, 4000, txtItemCode.Text.Trim());
      //}
      //if (txtMaterialGroup.Text.Trim().Length > 0)
      //{
      //  inputParam[2] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text.Trim());
      //}
      //int dataChange = chkDataChange.Checked ? 1 : 0;
      //inputParam[3] = new DBParameter("@DataChange", DbType.Int32, dataChange);      
      //DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMaterialPlanning_Select", inputParam);
      DataTable dtData = (DataTable)ultraGridWOMaterialDetail.DataSource;
      oXlsReport.Cell("**PrinDate").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          double consQty = DBConvert.ParseDouble(dtData.Rows[i]["ConsExist"].ToString());
          double newQty = DBConvert.ParseDouble(dtData.Rows[i]["NewCons"].ToString());
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:M7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:M7", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**WO", 0, i).Value = dtRow["Wo"];
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**Revision", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**ItemQty", 0, i).Value = dtRow["ItemQty"];
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"];
          oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialName"];
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**Waste", 0, i).Value = dtRow["Waste"];
          oXlsReport.Cell("**ConsExist", 0, i).Value = dtRow["ConsExist"];
          oXlsReport.Cell("**NewCons", 0, i).Value = dtRow["NewCons"];
          oXlsReport.Cell("**TotalQty", 0, i).Value = dtRow["TotalQty"];
          oXlsReport.Cell("B7:L7", 0, i).Attr.FontColor = xlColor.xcDefault;
          if (consQty == double.MinValue && newQty != double.MinValue) //New
          {
            oXlsReport.Cell("K7:L7", 0, i).Attr.FontColor = xlColor.xcBlue;
          }
          else if (consQty != double.MinValue && newQty == double.MinValue) //Delete
          {
            oXlsReport.Cell("K7:L7", 0, i).Attr.FontColor = xlColor.xcRed;
          }
          else if (consQty != newQty) //Change
          {
            oXlsReport.Cell("K7:L7", 0, i).Attr.FontColor = xlColor.xcPink;
          }
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void txtWO_Leave(object sender, EventArgs e)
    {
      this.LoadDataForItemListView();
    }

    private void txtWO_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void ucUltraListItem_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtItemCode.Text = ucUltraListItem.SelectedValue;
    }

    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialGroup.Text = ucUltraListMaterialGroup.SelectedValue;
    }
  }
}
