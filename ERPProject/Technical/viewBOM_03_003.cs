using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_03_003 : MainUserControl
  {
    #region Field
    private IList listUnlockFin = new ArrayList();
    #endregion Field

    public viewBOM_03_003()
    {
      InitializeComponent();
    }

    private void viewBOM_03_003_Load(object sender, EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(groupBox1);
      this.LoadWO();
      if (string.Compare(this.ViewParam, "BOM_03_003_02", true) == 0)
      {
        btnUnLock.Visible = false;
      }
      btnPrint.Enabled = false;
    }

    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
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
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void Search()
    {
      btnSearch.Enabled = false;
      if (this.CheckInvalid())
      {
        string finCode = txtFINCode.Text.Trim().Replace("'", "''");
        string name = txtFINDescr.Text.Trim().Replace("'", "''");
        string materialCode = txtMaterial.Text.Trim().Replace("'", "''");
        int compGroup = Shared.Utility.ConstantClass.COMP_FINISHING;
        DBParameter[] inputParam = new DBParameter[5];
        if (finCode.Length > 0)
        {
          inputParam[0] = new DBParameter("@FINCode", DbType.AnsiString, 16, "%" + finCode + "%");
        }
        if (name.Length > 0)
        {
          inputParam[1] = new DBParameter("@Name", DbType.String, 128, "%" + name + "%");
        }
        if (materialCode.Length > 0)
        {
          inputParam[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, "%" + materialCode + "%");
        }
        if (ultraCBWO.SelectedRow != null)
        {
          inputParam[3] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultraCBWO.Value.ToString()));
        }
        if (compGroup != int.MinValue)
        {
          inputParam[4] = new DBParameter("@CompGroup", DbType.Int32, compGroup);
        }
        DataTable dtFinishingList = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListFinishingInfo", inputParam);
        ultListFinishingInfo.DataSource = dtFinishingList;
      }
      btnPrint.Enabled = (ultListFinishingInfo.Rows.Count > 0) ? true : false;
      btnSearch.Enabled = true;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      DataTable a = new DataTable();
      a.Columns.Add(new DataColumn("Name"));
      a.Columns.Add(new DataColumn("Select"));
      viewBOM_03_004 view = new viewBOM_03_004();
      foreach (Control con in view.Controls)
      {
        if (con.GetType() == typeof(Button))
        {
          DataRow row = a.NewRow();
          row[0] = con.Name;
          a.Rows.Add(row);
          if (true)
            row[1] = 1;
          else
            row[1] = 1;
        }
      }

      view.finishingCode = string.Empty;
      view.ViewParam = this.ViewParam;
      Shared.Utility.WindowUtinity.ShowView(view, "FINISHING INFORMATION", false, Shared.Utility.ViewState.MainWindow);
      this.btnSearch_Click(sender, e);
    }

    private void ultListFinishingInfo_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
      {
        return;
      }
      try
      {
        string strCode = ultListFinishingInfo.Selected.Rows[0].Cells[0].Value.ToString();
        viewBOM_03_004 view = new viewBOM_03_004();
        view.finishingCode = strCode;
        view.ViewParam = this.ViewParam;
        Shared.Utility.WindowUtinity.ShowView(view, "FINISHING INFORMATION", false, Shared.Utility.ViewState.MainWindow);
        //this.btnSearch_Click(sender, e);
      }
      catch { }
    }

    private void ultListFinishingInfo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ultListFinishingInfo);
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;

      for (int i = 0; i < ultListFinishingInfo.Rows.Count; i++)
      {
        ultListFinishingInfo.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      ////   Hide the identity columns
      ////e.Layout.Bands["TblBOMFinishingInfo"].Columns["Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ////e.Layout.Bands[1].Columns["FinCode"].Hidden = true;
      //e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["Confirm"].Header.Caption = "Status";
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FinCode"].Header.Caption = "Finishing Code";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["LastCarcassProcess"].Header.Caption = "Last Carcass Process";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      ////e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      ////e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
      ////e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ////e.Layout.Bands[1].Override.CellClickAction = CellClickAction.RowSelect;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      for (int j = 0; j < ultListFinishingInfo.Rows.Count; j++)
      {
        string confirm = ultListFinishingInfo.Rows[j].Cells["Confirm"].Value.ToString().Trim();
        if (confirm == "Not Confirmed")
        {
          ultListFinishingInfo.Rows[j].Activation = Activation.ActivateOnly;
        }
      }

      //FormatCurrencyColumns();
    }

    private void FormatCurrencyColumns()
    {

      foreach (Infragistics.Win.UltraWinGrid.UltraGridBand oBand in this.dgvListFinishingInfo.DisplayLayout.Bands)
      {
        foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oColumn in oBand.Columns)
        {

          if (oColumn.DataType.ToString() == "System.Double")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            //oColumn.Format = "###,##,###.00";
          }
          if (oColumn.DataType.ToString() == "System.DateTime")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            oColumn.Format = "dd/MM/yyyy";
          }
        }
      }

    }
    // Qui, 24/11/2010 Add Button Unlock
    private void btnUnLock_Click(object sender, EventArgs e)
    {
      int count = ultListFinishingInfo.Rows.Count;
      int result = int.MinValue;
      bool selected = false;
      for (int i = 0; i < count; i++)
      {
        int check = int.MinValue;
        try
        {
          check = DBConvert.ParseInt(ultListFinishingInfo.Rows[i].Cells["Selected"].Value.ToString());
        }
        catch { }
        if (check == 1)
        {
          string finCode = ultListFinishingInfo.Rows[i].Cells["FinCode"].Value.ToString();
          DBParameter[] input = new DBParameter[] { new DBParameter("@FinCode", DbType.AnsiString, 16, finCode) };
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMFinishingInfo_Unlock", input, outputParam);
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

    private void ultraCBWO_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (ultListFinishingInfo.Rows.Count > 0)
      {
        Utility.ExportToExcelWithDefaultPath(ultListFinishingInfo, "BOMFinishingList");
      }
      // Init Excel File
      //string strTemplateName = "BOMFinishingList";
      //string strSheetName = "Finishing";
      //string strOutFileName = "Finishing List";
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate\Technical";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //// Get data
      //DataSet dsExport = (DataSet)ultListFinishingInfo.DataSource;
      //DataTable dtExport = dsExport.Tables[0];

      //if (dtExport != null && dtExport.Rows.Count > 0)
      //{
      //  for (int i = 0; i < dtExport.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dtExport.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("B7:I7").Copy();
      //      oXlsReport.RowInsert(6 + i);
      //      oXlsReport.Cell("B7:I7", 0, i).Paste();
      //    }

      //    oXlsReport.Cell("**FinCode", 0, i).Value = dtRow["FinCode"];
      //    oXlsReport.Cell("**Name", 0, i).Value = dtRow["Name"];
      //    oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"];
      //    oXlsReport.Cell("**Waste", 0, i).Value = dtRow["Waste"];
      //    oXlsReport.Cell("**SheenLevel", 0, i).Value = dtRow["SheenLevel"];
      //    oXlsReport.Cell("**CreateBy", 0, i).Value = dtRow["CreateBy"];
      //    oXlsReport.Cell("**CreateDate", 0, i).Value = dtRow["CreateDate"];
      //    oXlsReport.Cell("**Status", 0, i).Value = dtRow["Confirm"];
      //  }
      //}
      //oXlsReport.Out.File(strOutFileName);
      //Process.Start(strOutFileName);
    }

    private void ultListFinishingInfo_MouseClick(object sender, MouseEventArgs e)
    {
      if (ultListFinishingInfo.Selected != null && ultListFinishingInfo.Selected.Rows.Count > 0)
      {
        string code = ultListFinishingInfo.Selected.Rows[0].Cells["FinCode"].Value.ToString();
        //string code = ultraGridFinishingInfo.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
        PictureFinishing.ImageLocation = FunctionUtility.BOMGetItemComponentImage(code);
        gbPicture.Text = code;
      }
    }

    private void PictureFinishing_DoubleClick(object sender, EventArgs e)
    {
      //if (PictureFinishing.ImageLocation != null)
      //{
      //  viewSHR_01_001 view = new viewSHR_01_001();
      //  view.path = PictureFinishing.ImageLocation;
      //  view.compCode = PictureFinishing.Text;
      //  WindowUtinity.ShowView(view, "Finishing Image", true, ViewState.Window, FormWindowState.Normal);
      //}
    }
  }
}
