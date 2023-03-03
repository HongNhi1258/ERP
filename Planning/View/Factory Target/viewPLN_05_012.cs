/*
 * Author :
 * CreateDate : 16/06/2010
 * Description :Insert,Update,Delete Factory Target And Customer Quota
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_05_012 : MainUserControl
  {
    #region Field
    private int year = 0;
    private bool flagFactory = false;
    private bool flagQuota = false;

    public Double totalDistrubutor = 0;

    #endregion Field

    #region Init Data
    /// <summary>
    /// UC_PLNFactoryTarget_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_05_001_Load(object sender, EventArgs e)
    {
      LoadComboYear();
      if (year == 0)
      {
        return;
      }
      LoadDataGrid();
    }

    /// <summary>
    /// Load Year
    /// </summary>
    private void LoadComboYear()
    {
      for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year + 2; i++)
      {
        this.cmbYear.Items.Add(i.ToString());
      }
    }


    public viewPLN_05_012()
    {
      InitializeComponent();
    }
    #endregion Init Data

    #region Process
    /// <summary>
    /// Save data
    /// </summary>
    private bool SaveData()
    {
      bool isFinish = true;
      // 1: Save Quota
      DataTable dtQuota = (DataTable)ultraQuota.DataSource;
      for (int i = 0; i < dtQuota.Rows.Count; i++)
      {
        DataRow rowQuota = dtQuota.Rows[i];
        if (rowQuota.RowState != DataRowState.Deleted && rowQuota.RowState == DataRowState.Modified)
        {
          isFinish = this.UpdateRow(rowQuota, 0);
        }
      }

      // 2: Save Data
      DataTable dtData = (DataTable)ultData.DataSource;
      for (int j = 0; j < dtData.Rows.Count; j++)
      {
        DataRow rowData = dtData.Rows[j];
        if (rowData.RowState != DataRowState.Deleted && rowData.RowState == DataRowState.Modified)
        {
          isFinish = this.UpdateRow(rowData, 1);
        }
      }

      // 3: Save CBM Target 
      DataTable dtCBMTarget = (DataTable)ultCBMTarget.DataSource;
      for (int k = 0; k < dtCBMTarget.Rows.Count; k++)
      {
        DataRow rowCBMTarget = dtCBMTarget.Rows[k];
        if (rowCBMTarget.RowState != DataRowState.Deleted && rowCBMTarget.RowState == DataRowState.Modified)
        {
          isFinish = this.UpdateRow(rowCBMTarget, 2);
        }
      }
      return isFinish;

    }
    /// <summary>
    /// Update Row
    /// </summary>
    /// <param name="rowQuota"></param>
    /// <param name="Type"></param>
    private bool UpdateRow(DataRow rowQuota, int Type)
    {
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@DivisionCode", DbType.AnsiString, 12, rowQuota["Department"].ToString());
      string strData = "";
      for (int c = (Type == 0) ? 2 : 1; c < rowQuota.Table.Columns.Count; c++)
      {
        if (c != rowQuota.Table.Columns.Count - 1)
        {
          if (DBConvert.ParseDouble(rowQuota[c].ToString()) > 0)
          {
            strData = strData + rowQuota[c].ToString() + "#";
          }
          else
          {
            strData = strData + "0#";
          }
        }
        else
        {
          if (DBConvert.ParseDouble(rowQuota[c].ToString()) > 0)
          {
            strData = strData + rowQuota[c].ToString();
          }
          else
          {
            strData = strData + "0";
          }
        }
      }
      inputParam[1] = new DBParameter("@StringData", DbType.String, strData);
      inputParam[2] = new DBParameter("@YearNo", DbType.Int32, DBConvert.ParseInt(this.cmbYear.Text));
      inputParam[3] = new DBParameter("@Type", DbType.Int32, Type);

      //Code
      //Code

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      // Gan Lai Pid
      DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNDivisionTarget_Edit", inputParam, ouputParam);
      long Result = long.MinValue;
      Result = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (Result <= 0)
      {
        return false;
      }
      return true;
    }
    /// <summary>
    /// LoadDataGrid
    /// </summary>
    private void LoadDataGrid()
    {
      // Factory Target
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Year", DbType.Int32, year);

      DataSet dsTarget = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNDivisionTarget_Select", inputParam);
      this.ultraQuota.DataSource = dsTarget.Tables[0];
      //Quota Target
      this.ultData.DataSource = dsTarget.Tables[2];
      this.ultCBMTarget.DataSource = dsTarget.Tables[1];

      this.SetStatusControl();
    }

    private void SetStatusControl()
    {
      for (int i = 0; i < ultraQuota.Rows.Count; i++)
      {
        UltraGridRow row = ultraQuota.Rows[i];
        if (string.Compare(row.Cells["Department"].Value.ToString(), "FEUS", true) == 0)
        {
          row.Activation = Activation.ActivateOnly;
        }
      }
    }
    #endregion Process

    #region Event
    /// <summary>
    /// btnSave_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.SaveData())
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      if (year == 0)
      {
        return;
      }
      LoadDataGrid();
      this.NeedToSave = false;
    }

    /// <summary>
    /// ultData_AfterCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.flagFactory = true;
      if (btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// btnClose_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      if (btnSave.Visible == true)
      {
        this.ConfirmToCloseTab();
      }
      else
      {
        this.CloseTab();
      }
    }

    /// <summary>
    /// SaveAndClose
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    /// <summary>
    /// ultData_InitializeLayout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Department"].CellActivation = Activation.ActivateOnly;

      int yearNow = DateTime.Now.Year;
      int monthMow = DateTime.Now.Month;

      //for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      //{
      //  if (yearNow == DBConvert.ParseInt(cmbYear.Text))
      //  {
      //    if (i < monthMow)
      //    {
      //      e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //    }
      //    else
      //    {
      //      e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
      //    }
      //  }
      //  else if (yearNow > DBConvert.ParseInt(cmbYear.Text))
      //  {
      //    e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //  }
      //}

      e.Layout.Bands[0].Columns["JAN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FEB"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["APR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAY"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUL"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["AUG"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SEP"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OCT"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["NOV"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["DEC"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["JAN"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["FEB"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["MAR"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["APR"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["MAY"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["JUN"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["JUL"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["AUG"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["SEP"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["OCT"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["NOV"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["DEC"].Format = "###,###.##";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Format Quota Factory Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraQuota_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraQuota);

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Department"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;

      int yearNow = DateTime.Now.Year;
      int monthMow = DateTime.Now.Month;

      //for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      //{
      //  if (yearNow == DBConvert.ParseInt(cmbYear.Text))
      //  {
      //    if (i < monthMow + 1)
      //    {
      //      e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //    }
      //    else
      //    {
      //      e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
      //    }
      //  }
      //  else if (yearNow > DBConvert.ParseInt(cmbYear.Text))
      //  {
      //    e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //  }
      //}

      e.Layout.Bands[0].Columns["JAN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FEB"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["APR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAY"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUL"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["AUG"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SEP"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OCT"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["NOV"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["DEC"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["JAN"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["FEB"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["MAR"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["APR"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["MAY"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["JUN"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["JUL"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["AUG"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["SEP"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["OCT"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["NOV"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["DEC"].Format = "###,###.##";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// ultData_BeforeCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string text = e.Cell.Text;
      if (text.Trim().Length == 0)
      {
        return;
      }
      double value = DBConvert.ParseDouble(e.Cell.Text);

      if (value < 0)
      {
        MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0024"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
      }
    }

    /// <summary>
    /// ultraQuota_BeforeCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraQuota_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string text = e.Cell.Text;
      if (text.Trim().Length == 0)
      {
        return;
      }
      double value = DBConvert.ParseDouble(e.Cell.Text);

      if (value < 0)
      {
        MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0024"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
      }
    }

    /// <summary>
    /// ultraQuota_AfterCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraQuota_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.flagQuota = true;
      if (btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// cmbYear_SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbYear_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.NeedToSave == true)
      {
        DialogResult dlgr = MessageBox.Show
                  (Shared.Utility.FunctionUtility.GetMessage("MSG0008"), "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (dlgr == DialogResult.Yes)
        {
          this.SaveData();
        }
      }
      this.year = DBConvert.ParseInt(this.cmbYear.Text);
      LoadDataGrid();
      this.NeedToSave = false;
      this.flagFactory = false;
      this.flagQuota = false;
    }

    private void ultCBMTarget_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultCBMTarget);

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Department"].CellActivation = Activation.ActivateOnly;

      int yearNow = DateTime.Now.Year;
      int monthMow = DateTime.Now.Month;

      //for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      //{
      //  if (yearNow == DBConvert.ParseInt(cmbYear.Text))
      //  {
      //    if (i < monthMow)
      //    {
      //      e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //    }
      //    else
      //    {
      //      e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
      //    }
      //  }
      //  else if (yearNow > DBConvert.ParseInt(cmbYear.Text))
      //  {
      //    e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //  }
      //}

      e.Layout.Bands[0].Columns["JAN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FEB"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["APR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAY"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUL"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["AUG"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SEP"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OCT"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["NOV"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["DEC"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["JAN"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["FEB"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["MAR"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["APR"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["MAY"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["JUN"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["JUL"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["AUG"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["SEP"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["OCT"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["NOV"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["DEC"].Format = "###,###.##";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultCBMTarget_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string text = e.Cell.Text;
      if (text.Trim().Length == 0)
      {
        return;
      }
      double value = DBConvert.ParseDouble(e.Cell.Text);

      if (value < 0)
      {
        MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0024"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
      }
    }

    private void ultCBMTarget_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.flagFactory = true;
      if (btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private void ultraQuota_AfterCellActivate(object sender, EventArgs e)
    {
      UltraGridRow row = ultraQuota.ActiveRow;
      if (row.Cells["Department"].Text.ToLower() == "feus")
      {
        UltraGridCell cell = ultraQuota.ActiveCell;
        int month = 0;
        int year = DBConvert.ParseInt(cmbYear.Text);
        if (cell.Column.Header.Caption == "JAN") month = 1;
        else if (cell.Column.Header.Caption == "FEB") month = 2;
        else if (cell.Column.Header.Caption == "MAR") month = 3;
        else if (cell.Column.Header.Caption == "APR") month = 4;
        else if (cell.Column.Header.Caption == "MAY") month = 5;
        else if (cell.Column.Header.Caption == "JUN") month = 6;
        else if (cell.Column.Header.Caption == "JUL") month = 7;
        else if (cell.Column.Header.Caption == "AUG") month = 8;
        else if (cell.Column.Header.Caption == "SEP") month = 9;
        else if (cell.Column.Header.Caption == "OCT") month = 10;
        else if (cell.Column.Header.Caption == "NOV") month = 11;
        else month = 12;

        if ((year == DateTime.Now.Year && month >= DateTime.Now.Month) || year > DateTime.Now.Year)
        {
          viewPLN_05_014 uc = new viewPLN_05_014();
          uc.month = month;
          uc.year = year;
          uc.frmMaster = this;

          DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE TARGET DISTRIBUTOR", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
          //if (totalDistrubutor >= 0)
          //{
          //  cell.Value = totalDistrubutor;
          //}

          // Truong Add
          string commandText = string.Empty;
          commandText = " SELECT TG.MonthNo, TG.YearNo, SUM(ISNULL(TG.FEUs, 0)) TotalFEUs";
          commandText += " FROM TblBOMCodeMaster CM";
          commandText += "    LEFT JOIN TblPLNDivisionTargetDistributor TG ON CM.Value = TG.Distributor";
          commandText += " WHERE CM.[Group] = 16033";
          commandText += "   AND TG.[MonthNo] = " + month + "";
          commandText += "   AND TG.[YearNo] = " + year + "";
          commandText += " GROUP BY TG.MonthNo, TG.YearNo";

          DataTable dtResult = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtResult != null && dtResult.Rows.Count > 0)
          {
            double totalFEUs = DBConvert.ParseDouble(dtResult.Rows[0]["TotalFEUs"].ToString());
            if (totalFEUs > 0)
            {
              cell.Value = totalFEUs;
            }
            else
            {
              cell.Value = DBNull.Value;
            }
          }
          else
          {
            cell.Value = DBNull.Value;
          }
          // End
        }
      }
    }
    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraQuota, "Data");
    }
    #endregion Event

  }
}
