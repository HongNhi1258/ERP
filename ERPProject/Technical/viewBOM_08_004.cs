using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_08_004 : MainUserControl
  {
    #region Field
    private IList listUnlockFin = new ArrayList();
    #endregion Field

    public viewBOM_08_004()
    {
      InitializeComponent();
    }

    private void viewBOM_08_004_Load(object sender, EventArgs e)
    {
    }

    /// <summary>
    /// Check In Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      return true;
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void Search()
    {
      btnSearch.Enabled = false;
      string finCode = txtFINCode.Text.Trim().Replace("'", "''");
      string name = txtFINDescr.Text.Trim().Replace("'", "''");
      string desc = txtDescription.Text.Trim().Replace("'", "''");
      string process = txtProcessCode.Text.Trim().Replace("'", "''");
      DBParameter[] inputParam = new DBParameter[4];
      if (finCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@FINCode", DbType.AnsiString, 16, "%" + finCode + "%");
      }
      if (name.Length > 0)
      {
        inputParam[1] = new DBParameter("@Name", DbType.String, 512, "%" + name + "%");
      }
      if (desc.Length > 0)
      {
        inputParam[2] = new DBParameter("@Description", DbType.String, 512, "%" + desc + "%");
      }
      if (process.Length > 0)
      {
        inputParam[3] = new DBParameter("@ProcessCode", DbType.AnsiString, 64, "%" + process + "%");
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spBOMFinishingProcessCode_Search", inputParam);

      ultListFinishingInfo.DataSource = ds.Tables[0];
      lbCount.Text = string.Format(@"Count:{0}", ultListFinishingInfo.Rows.FilteredInRowCount > 0 ? ultListFinishingInfo.Rows.FilteredInRowCount : 0);

      btnSearch.Enabled = true;
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch_Click(sender, e);
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewBOM_08_005 view = new viewBOM_08_005();
      view.finishingProceesPid = long.MinValue;
      WindowUtinity.ShowView(view, "New Finishing Process", true, ViewState.MainWindow);
    }

    private void ultListFinishingInfo_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultListFinishingInfo.Selected != null && ultListFinishingInfo.Selected.Rows.Count > 0)
      {
        long finishingProcessPid = DBConvert.ParseLong(ultListFinishingInfo.Selected.Rows[0].Cells["Pid"].Value.ToString());
        viewBOM_08_005 view = new viewBOM_08_005();
        view.finishingProceesPid = finishingProcessPid;
        view.ViewParam = this.ViewParam;
        Shared.Utility.WindowUtinity.ShowView(view, "FINISHING PROCESS INFORMATION", false, Shared.Utility.ViewState.MainWindow);        
      }
    }

    private void ultListFinishingInfo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ultListFinishingInfo);      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      //for (int i = 0; i < ultListFinishingInfo.Rows.Count; i++)
      //{
      //  ultListFinishingInfo.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      //}
      //   Set some scrolling related properties.
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      //   Hide the identity columns
      //e.Layout.Bands["TblBOMFinishingInfo"].Columns["Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ProcessCode"].Header.Caption = "Process Code";
      e.Layout.Bands[0].Columns["FinishingCode"].Header.Caption = "Finishing Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Finishing\nEnglish Name";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Finishing\nVietnamese Name";
      ////e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      ////e.Layout.Bands["TblBOMFinishingInfo"].Columns["Name"].Header.Caption = "Name EN";
      ////e.Layout.Bands["TblBOMFinishingInfo"].Columns["Confirm"].Header.Caption = "Status";
      ////e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;

      //e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Name VN";
      //e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      ////e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      //e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      ////e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ////e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      //e.Layout.Bands[1].ColHeaderLines = 2;
      //e.Layout.Bands[1].Columns["ProcessDescription"].Header.Caption = "Process\n Description";
      //e.Layout.Bands[1].Columns["ChemicalCode"].Header.Caption = "Chemical Code";
      //e.Layout.Bands[1].Columns["HoldTime"].Header.Caption = "Hold Time (h)";
      ////e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[1].Override.CellClickAction = CellClickAction.RowSelect;

      //	use the same appearance for alternate rows
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].Width = 60;

      e.Layout.Bands[0].Columns["ProcessCode"].Width = 70;
      e.Layout.Bands[0].Columns["FinishingCode"].Width = 80;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Selected"].CellActivation = Activation.AllowEdit;


      //for (int j = 0; j < ultListFinishingInfo.Rows.Count; j++)
      //{
      //  string confirm = ultListFinishingInfo.Rows[j].Cells["Confirm"].Value.ToString().Trim();
      //  if (confirm == "Not Confirmed")
      //  {
      //    ultListFinishingInfo.Rows[j].Activation = Activation.ActivateOnly;
      //  }
      //}

      FormatCurrencyColumns();
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
          long finishingProcessPid = DBConvert.ParseLong(ultListFinishingInfo.Rows[i].Cells["Pid"].Value.ToString());
          DBParameter[] input = new DBParameter[] { new DBParameter("@FinishingProcessPid", DbType.Int64, finishingProcessPid) };
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMFinishingProcessInfo_Unlock", input, outputParam);
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
