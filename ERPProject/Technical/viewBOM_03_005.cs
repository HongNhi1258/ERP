using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_03_005 : MainUserControl
  {
    #region Field
    private string dept = string.Empty;
    #endregion Field

    public viewBOM_03_005()
    {
      InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    private void viewBOM_03_005_Load(object sender, EventArgs e)
    {
      this.LoadWO();
      if (string.Compare(this.ViewParam, "BOM_03_005_02", true) == 0)
      {
        btnUnLock.Visible = false;
      }
      //this.Search();
    }

    private void LoadWO()
    {
      //Load data for combobox WO
      string commandText = "Select Pid From TblPLNWorkOrder";
      DataTable dtWO = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO != null)
      {
        Utility.LoadUltraCombo(ultraCBWO, dtWO, "Pid", "Pid");
        ultraCBWO.DisplayLayout.Bands[0].Columns["Pid"].Width = 300;
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
      return true;
    }
    private void Search()
    {
      if (this.CheckInvalid())
      {
        string supCode = txtSuppCode.Text.Trim();
        string description = txtNameEn.Text.Trim();
        string materialCode = txtMaterial.Text.Trim();
        DBParameter[] inputParam = new DBParameter[4];
        if (supCode.Length > 0)
        {
          inputParam[0] = new DBParameter("@SupCode", DbType.AnsiString, 16, "%" + supCode + "%");
        }
        if (description.Length > 0)
        {
          inputParam[1] = new DBParameter("@Description", DbType.String, 128, "%" + description + "%");
        }
        if (materialCode.Length > 0)
        {
          inputParam[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, "%" + materialCode + "%");
        }
        if (ultraCBWO.SelectedRow != null)
        {
          inputParam[3] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultraCBWO.Value.ToString()));
        }
        
        DataSet ds = DataBaseAccess.SearchStoreProcedure("spBOMListSupportInfo", inputParam);
        DataSet dsList = CreateDataSet.Support();
        DataColumn dtColumn = new DataColumn("Selected", typeof(System.Int32));
        dtColumn.DefaultValue = 0;
        dsList.Tables["TblBOMSupportInfo"].Merge(ds.Tables[0]);
        dsList.Tables["TblBOMSupportInfo"].Columns.Add(dtColumn);
        dsList.Tables["TblBOMSupportDetail"].Merge(ds.Tables[1]);
        utlSuppList.DataSource = dsList;
      }
    }
  
    private void utlSuppList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {      
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      //   Set some scrolling related properties.
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      //   Hide the identity columns
      e.Layout.Bands[1].Columns["SupCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Description"].Header.Caption = "Description EN";
      e.Layout.Bands[0].Columns["SupCode"].Header.Caption = "Support Code";
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "Description VN";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Depth"].Header.Caption = "Length";
      e.Layout.Bands[1].Columns["Depth"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Height"].Header.Caption = "Thickness";
      e.Layout.Bands[1].Columns["Height"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Override.CellClickAction = CellClickAction.RowSelect;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      for (int j = 0; j < utlSuppList.Rows.Count; j++)
      {
        string confirm = utlSuppList.Rows[j].Cells["Status"].Value.ToString().Trim();
        if (confirm == "Not Confirmed")
        {
          utlSuppList.Rows[j].Activation = Activation.ActivateOnly;
        }
      }
      FormatCurrencyColumns();
    }
    private void FormatCurrencyColumns()
    {
      foreach (Infragistics.Win.UltraWinGrid.UltraGridBand oBand in this.utlSuppList.DisplayLayout.Bands)
      {
        foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oColumn in oBand.Columns)
        {

          if (oColumn.DataType.ToString() == "System.Double")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          }
          if (oColumn.DataType.ToString() == "System.DateTime")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
          }
        }
      }
    }
    private void utlSuppList_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
      {
        return;
      }
      bool selected = false;
      try
      {
        selected = utlSuppList.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (utlSuppList.Selected.Rows[0].ParentRow == null) ? utlSuppList.Selected.Rows[0] : utlSuppList.Selected.Rows[0].ParentRow;
      try
      {
        string strCode = row.Cells[0].Value.ToString();
        viewBOM_03_006 objSupGroup = new viewBOM_03_006();
        objSupGroup.supCode = strCode;
        objSupGroup.ViewParam = this.ViewParam;
        Shared.Utility.WindowUtinity.ShowView(objSupGroup, "SUPPORT INFORMATION", false, Shared.Utility.ViewState.MainWindow);        
      }
      catch
      {
      }
    }

 
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewBOM_03_006 objSupp = new viewBOM_03_006();
      objSupp.supCode = string.Empty;
      objSupp.ViewParam = this.ViewParam;
      Shared.Utility.WindowUtinity.ShowView(objSupp, "SUPPORT INFORMATION", false, Shared.Utility.ViewState.MainWindow);      
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        btnSearch_Click(sender, e);
      else
        return;
    }

    private void btnUnLock_Click(object sender, EventArgs e)
    {
      int count = utlSuppList.Rows.Count;
      int result = int.MinValue;
      bool selected = false;
      for (int i = 0; i < count; i++)
      {
        int check = int.MinValue;
        try
        {
          check = DBConvert.ParseInt(utlSuppList.Rows[i].Cells["Selected"].Value.ToString());
        }
        catch { }
        if (check == 1)
        {
          string supCode = utlSuppList.Rows[i].Cells["SupCode"].Value.ToString();
          DBParameter[] input = new DBParameter[] { new DBParameter("@SupCode", DbType.AnsiString, 16, supCode) };
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMSupportInfo_Unlock", input, outputParam);
          result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          selected = (result == 1) ? true : false;
        }
      }
      if (!selected)
      {
        WindowUtinity.ShowMessageWarning("WRN0014");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0023");
      }
      this.Search();
    }
  }
}
