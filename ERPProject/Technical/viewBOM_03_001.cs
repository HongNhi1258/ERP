using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace DaiCo.ERPProject
{
  public partial class viewBOM_03_001 : MainUserControl
  {
    private int rowIndex = int.MinValue;

    #region Init Form
    public viewBOM_03_001()
    {
      InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void viewBOM_03_001_Load(object sender, EventArgs e)
    {
      //Load data for combobox WO
      string commandText = "Select Pid From TblPLNWorkOrder";
      DataTable dtWO = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO != null)
      {
        Utility.LoadUltraCombo(ultraCBWO, dtWO, "Pid", "Pid");
        ultraCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }
      // Load data for combobox Customer
      Utility.LoadUltraCBCustomer(ucbCustomer);
      // Load data for combobox Collection
      Utility.LoadUltraCBCollection(ucbCollection);
    }
    #endregion Init Form

    #region Process
    private void Search()
    {
      btnSearch.Enabled = false;
      if (this.CheckInvalid())
      {
        DBParameter[] inputParam = new DBParameter[10];
        string text = txtCarcassCode.Text.Trim();
        string oldCode = txtOldCode.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 18, "%" + text + "%");
        }
        text = txtDescriptionEn.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[1] = new DBParameter("@Description", DbType.String, 130, "%" + text + "%");
        }
        text = txtItemCode.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + text + "%");
        }

        if (!chkInComp.Checked && chkContractOut.Checked)
        {
          inputParam[3] = new DBParameter("@ContractOut", DbType.Int32, 4, 1);
        }
        else if (chkInComp.Checked && !chkContractOut.Checked)
        {
          inputParam[3] = new DBParameter("@ContractOut", DbType.Int32, 4, 0);
        }
        inputParam[4] = new DBParameter("@DeleteFlag", DbType.Int32, 4, 0);

        if (oldCode.Length > 0)
        {
          //inputParam[5] = new DBParameter("@OldCode", DbType.AnsiString, 16, "%" + oldCode.Replace("'", "''") + "%");
          inputParam[7] = new DBParameter("@SaleCode", DbType.String, oldCode);
        }

        if (ultraCBWO.SelectedRow != null)
        {
          inputParam[6] = new DBParameter("@WO", DbType.Int64, ultraCBWO.Value);
        }
        if (ucbCustomer.SelectedRow != null)
        {
          inputParam[7] = new DBParameter("@CustomerPid", DbType.Int64, ucbCustomer.Value);
        }
        if (ucbCollection.SelectedRow != null)
        {
          inputParam[8] = new DBParameter("@Collection", DbType.Int32, ucbCollection.Value);
        }

        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListCarcass", inputParam);
        ugdInformation.DataSource = dtSource;
        lbCountCarcass.Text = string.Format("Count: {0}", dtSource.Rows.Count);
      }
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// CheckInValid
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
    #endregion Process

    #region Event
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewBOM_01_010 view = new viewBOM_01_010();
      WindowUtinity.ShowView(view, "New Carcass", false, Shared.Utility.ViewState.MainWindow);
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void btnUnLock_Click(object sender, EventArgs e)
    {
      int count = ugdInformation.Rows.Count;
      bool selected = false;
      for (int i = 0; i < count; i++)
      {
        int copy = DBConvert.ParseInt(ugdInformation.Rows[i].Cells["Select"].Value);
        if (copy == 1)
        {
          string carcassCode = ugdInformation.Rows[i].Cells["CarcassCode"].Value.ToString();
          DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode) };
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMCarcass_Unlock", inputParam, outputParam);
          int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          selected = (result == 1) ? true : false;
        }
      }
      if (!selected)
      {
        WindowUtinity.ShowMessageWarning("WRN0033");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0023");
      }
      this.Search();
    }
    #endregion Event   

    private void ultWorkOrder_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
    }

    private void chkShowWO_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowWO.Checked)
      {
        ultWorkOrder.Visible = true;
      }
      else
      {
        ultWorkOrder.Visible = false;
      }
    }

    private void ShowListWorkOder(string carcassCode)
    {
      if (chkShowWO.Checked)
      {
        string commandText = string.Format(@"SELECT WO.WoInfoPID, SUM(WO.Qty) Qty
                                             FROM TblPLNWOInfoDetailGeneral WO
                                             INNER JOIN TblBOMItemInfo ITEM ON WO.ItemCode = ITEM.ItemCode 
                                                                            AND WO.Revision = ITEM.Revision
                                                                            AND ITEM.CarcassCode = '{0}'
                                             GROUP BY WO.WoInfoPID, WO.ItemCode, WO.Revision 
                                             ORDER BY WO.WoInfoPID DESC", carcassCode);

        DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtWO != null)
        {
          ultWorkOrder.DataSource = dtWO;
          ultWorkOrder.Visible = true;
          ultWorkOrder.DisplayLayout.Bands[0].Columns["WoInfoPID"].Header.Caption = "WO List Of CarcassCode: " + carcassCode;
        }
        else
        {
          ultWorkOrder.DataSource = null;
        }
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Carcass List");
    }

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;

      // Allow update, delete, add new
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        // Read only
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Select"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

      // Set Column Style
      e.Layout.Bands[0].Columns["TECNote"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Hide column
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Description"].Hidden = true;

      for (int j = 0; j < e.Layout.Rows.Count; j++)
      {
        int color = DBConvert.ParseInt(ugdInformation.Rows[j].Cells["Status"].Value.ToString());
        switch (color)
        {
          case 1:
            e.Layout.Rows[j].Appearance.BackColor = Color.LightGray;
            e.Layout.Rows[j].Cells["Select"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            break;
          case 2:
            e.Layout.Rows[j].Appearance.BackColor = Color.LightPink;
            e.Layout.Rows[j].Cells["Select"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            break;
          case 3:
            e.Layout.Rows[j].Appearance.BackColor = Color.SkyBlue;
            e.Layout.Rows[j].Cells["Select"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            break;
          default:
            e.Layout.Rows[j].Cells["Select"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            break;
        }
      }

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      // Set caption column
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "Description";
      e.Layout.Bands[0].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[0].Columns["ContractOutData"].Header.Caption = "Contract Out\n Data";
      e.Layout.Bands[0].Columns["TECNote"].Header.Caption = "TEC \nNote";

      // Set Width
      e.Layout.Bands[0].Columns["CustomerName"].MaxWidth = 250;
      e.Layout.Bands[0].Columns["CustomerName"].MinWidth = 250;
      e.Layout.Bands[0].Columns["Collection"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Collection"].MinWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["CreateBy"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CreateBy"].MinWidth = 100;
      e.Layout.Bands[0].Columns["UpdateBy"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["UpdateBy"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Made"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Made"].MinWidth = 70;
      e.Layout.Bands[0].Columns["ContractOutData"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ContractOutData"].MinWidth = 80;
      e.Layout.Bands[0].Columns["TecNote"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["TecNote"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Select"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Select"].MinWidth = 50;
    }

    private void ugdInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (chkShowWO.Checked && ugdInformation.Selected.Rows.Count > 0)
      {
        string carcassCode = ugdInformation.Selected.Rows[0].Cells["CarcassCode"].Value.ToString();
        this.ShowListWorkOder(carcassCode);
      }
    }

    private void ugdInformation_DoubleClick(object sender, EventArgs e)
    {
      string carcassCode = string.Empty;
      if (ugdInformation.Selected.Rows.Count > 0)
      {
        carcassCode = ugdInformation.Selected.Rows[0].Cells["CarcassCode"].Value.ToString();
      }
      else
      {
        return;
      }

      if (radComponent.Checked)
      {
        viewBOM_01_010 view = new viewBOM_01_010();
        view.carcassCode = carcassCode;
        WindowUtinity.ShowView(view, carcassCode, true, Shared.Utility.ViewState.MainWindow);
      }
      else
      {
        viewBOM_01_024 view = new viewBOM_01_024();
        view.carcassCode = carcassCode;
        WindowUtinity.ShowView(view, carcassCode, true, Shared.Utility.ViewState.MainWindow);
      }
    }

    private void ugdInformation_AfterCellActivate(object sender, EventArgs e)
    {
      if (chkShowWO.Checked)
      {
        string carcassCode = ugdInformation.ActiveRow.Cells["CarcassCode"].Value.ToString();
        this.ShowListWorkOder(carcassCode);
      }
    }
  }
}
