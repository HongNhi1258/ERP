using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_21_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewWHD_21_001()
    {
      InitializeComponent();
    }


    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewVEN_02_001_Load(object sender, EventArgs e)
    {
      drpDateFrom.Value = DateTime.Today.AddDays(-7);

      // Load All Data For Search Information
      this.LoadData();

      // Set Focus
      this.txtNoFrom.Focus();
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[14];

      // Receiving No
      string text = txtNoFrom.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 24, text);
      }

      text = txtNoTo.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 24, text);
      }

      text = txtNoSet.Text.Trim();
      string[] listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1024, text);
      }

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (drpDateTo.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        param[3] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        param[4] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // Type

      int value = int.MinValue;

      if (this.ultType.Value != null)
      {
        value = DBConvert.ParseInt(this.ultType.Value.ToString());
        if (value != int.MinValue)
        {
          param[5] = new DBParameter("@Type", DbType.Int32, value);
        }
      }

      // Status
      if (this.ultStatus.Value != null)
      {
        value = DBConvert.ParseInt(this.ultStatus.Value.ToString());
        if (value != int.MinValue)
        {
          param[6] = new DBParameter("@Status", DbType.Int32, value);
        }
      }

      // Department
      if (this.ultDepartment.Value != null)
      {
        text = this.ultDepartment.Value.ToString();
        if (text.Length > 0)
        {
          param[7] = new DBParameter("@Department", DbType.AnsiString, 256, text);
        }
      }

      // Create By
      if (this.ultCreateBy.Value != null)
      {
        value = DBConvert.ParseInt(this.ultCreateBy.Value.ToString());
        if (value != int.MinValue)
        {
          param[8] = new DBParameter("@CreateBy", DbType.Int32, value);
        }
      }

      // Location
      if (this.ultLocation.Value != null)
      {
        value = DBConvert.ParseInt(this.ultLocation.Value.ToString());
        if (value != int.MinValue)
        {
          param[9] = new DBParameter("@LocationPid", DbType.Int32, value);
        }
      }

      // Approved By
      if (this.ultApprovedBy.Value != null)
      {
        value = DBConvert.ParseInt(this.ultApprovedBy.Value.ToString());
        if (value != int.MinValue)
        {
          param[10] = new DBParameter("@ApprovedBy", DbType.Int32, value);
        }
      }

      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        text = this.ultMaterialCode.Value.ToString();
        if (text.Length > 0)
        {
          param[11] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
        }
      }

      // Supplier
      if (this.ultSupplier.Value != null)
      {
        text = this.ultSupplier.Value.ToString();
        if (text.Length > 0)
        {
          param[12] = new DBParameter("@Supplier", DbType.AnsiString, 16, text);
        }
      }

      // Receiver
      if (this.ultReceiver.Value != null)
      {
        value = DBConvert.ParseInt(this.ultReceiver.Value.ToString());
        if (value != int.MinValue)
        {
          param[13] = new DBParameter("@Receiver", DbType.Int32, value);
        }
      }

      string storeName = "spWHDListIssuingWoods_Select";
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, 300, param);

      DataSet dsData = Shared.Utility.CreateDataSet.ListIssuingVeneer();
      dsData.Tables["TblIssuingVeneer"].Merge(dsSource.Tables[0]);
      dsData.Tables["TblIssuingDetailVeneer"].Merge(dsSource.Tables[1]);

      ultData.DataSource = dsData;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int type = DBConvert.ParseInt(ultData.Rows[i].Cells["Type"].Value.ToString());
        if (type == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == 2)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
        else if (type == 3)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (type == 4)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightSkyBlue;
        }
        else if (type == 5) // Issue Request Online Special
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == 6) // Issue Request Online 
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == 7) // Issue Paper
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
      }
    }

    /// <summary>
    /// Load All Data For Search Information
    /// </summary>
    private void LoadData()
    {
      // Load UltraCombo Type (Issue To Production/ Return To Supplier/ Adjustment/ Issue To Supcon)
      this.LoadComboType();

      // Load UltraCombo Status (0: New / 1: Confirm)
      this.LoadComboStatus();

      // Load UltraCombo Department
      this.LoadComboDepartment();

      // Load UltraCombo Create By
      this.LoadComboCreateBy();

      // Load UltraCombo Location
      this.LoadComboLocation();

      // Load UltraCombo Approved By
      this.LoadComboApprovedBy();

      // Load UltraCombo Material Code
      this.LoadComboMaterialCode();

      // Load UltraCombo Supplier
      this.LoadComboSupplier();

      // Load UltraCombo Receiver
      this.LoadComboReceiver();

      // Set Focus
      this.txtNoFrom.Focus();
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboReceiver()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultSupplier, dtSource, "ID_NhanVien", "Name", false, "ID_NhanVien");
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT ID_NhaCC, ID_NhaCC + ' - ' + TenNhaCCEN AS Name FROM VWHDSupplier";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultSupplier, dtSource, "ID_NhaCC", "Name", false, "ID_NhaCC");
    }

    /// <summary>
    /// Load UltraCombo Material Code
    /// </summary>
    private void LoadComboMaterialCode()
    {
      string commandText = string.Empty;
      commandText += "  SELECT MaterialCode ID_SanPham, MaterialCode + ' - ' + MaterialNameEn AS Name";
      commandText += "  FROM VBOMMaterials";
      commandText += "  WHERE Warehouse = 3";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultMaterialCode, dtSource, "ID_SanPham", "Name", false, "ID_SanPham");
    }

    /// <summary>
    /// Load UltraCombo Approved By
    /// </summary>
    private void LoadComboApprovedBy()
    {
      string commandText = string.Empty;
      commandText += "  SELECT DEP.Manager, CONVERT(varchar, DEP.Manager) + ' - ' + NV.HoNV + ' ' + NV.TenNV Name";
      commandText += "  FROM VHRDDepartmentInfo DEP";
      commandText += "  LEFT JOIN VHRNhanVien NV ON DEP.Manager = NV.ID_NhanVien";
      commandText += "  WHERE DEP.Code = 'WHD'";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultApprovedBy, dtSource, "Manager", "Name", false, "Manager");
    }

    /// <summary>
    /// Load UltraCombo Location
    /// </summary>
    private void LoadComboLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_Position, Name";
      commandText += " FROM VWHDLocationWoods ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultLocation, dtSource, "ID_Position", "Name", false, "ID_Position");
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Department = 'WHD' AND Resigned = 0";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCreateBy, dtSource, "ID_NhanVien", "Name", false, "ID_NhanVien");
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultDepartment, dtSource, "Department", "Name", false, "Department");
    }

    /// <summary>
    /// Load UltraCombo Status (0: New / 1: Confirm)
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Confirmed' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultStatus, dtSource, "ID", "Name", false, "ID");
    }

    /// <summary>
    /// Load UltraCombo Type (Receiving Note / Return From Production / Adjustment In)
    /// </summary>
    private void LoadComboType()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'Issue To Production' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Return To Supplier' Name";
      commandText += " UNION ";
      commandText += " SELECT 3 ID, 'Adjustment' Name";
      commandText += " UNION ";
      commandText += " SELECT 4 ID, 'Issue To Subcon' Name";
      commandText += " UNION ";
      commandText += " SELECT 5 ID, 'Issue SPECIAL ID' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultType, dtSource, "ID", "Name", false, "ID");
    }
    #endregion LoadData

    #region Event
    /// <summary>
    /// Clear Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      //Format
      this.txtNoFrom.Text = string.Empty;
      this.txtNoTo.Text = string.Empty;
      this.txtNoSet.Text = string.Empty;
      this.drpDateFrom.Value = DateTime.Today;
      this.drpDateTo.Value = DateTime.Today;
      this.ultType.Text = string.Empty;
      this.ultStatus.Text = string.Empty;
      this.ultDepartment.Text = string.Empty;
      this.ultCreateBy.Text = string.Empty;
      this.ultLocation.Text = string.Empty;
      this.ultApprovedBy.Text = string.Empty;
      this.ultMaterialCode.Text = string.Empty;
      this.ultSupplier.Text = string.Empty;
      this.ultReceiver.Text = string.Empty;

      // Load All Data For Search Information
      this.LoadData();
    }

    /// <summary>
    /// Search Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      // Search Information
      this.Search();
    }

    /// <summary>
    /// Format Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      //e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[1].Columns["IssuingNotePid"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Print"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Band.Columns["IssuingCode"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["Status"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["Title"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["Source"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["ApprovedPerson"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["Remark"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].Band.Columns["Print"].CellActivation = Activation.AllowEdit;
        ultData.Rows[i].ChildBands[0].Band.Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].ChildBands[0].Band.Columns["Location"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].ChildBands[0].Band.Columns["Unit"].CellActivation = Activation.ActivateOnly;
        ultData.Rows[i].ChildBands[0].Band.Columns["Qty"].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["IssuingCode"].Header.Caption = "Issuing Code";
      e.Layout.Bands[0].Columns["ApprovedPerson"].Header.Caption = "Approved Person";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";

      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Mat.Code";
      e.Layout.Bands[1].Columns["Location"].Header.Caption = "Package";
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Open Sceen when Double
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;

      long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
      int type = DBConvert.ParseInt(row.Cells["Type"].Value.ToString());

      if (type == 1)
      {
        // Issuing Note
        viewWHD_21_002 uc = new viewWHD_21_002();
        uc.issuingPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE ISSUING NOTE", false, ViewState.MainWindow);
      }
      else if (type == 2)
      {
        // Return To Supplier
        viewWHD_21_003 uc = new viewWHD_21_003();
        uc.issuingPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE RETURN TO SUPPLIER", false, ViewState.MainWindow);
      }
      else if (type == 3)
      {
        // Adjustment Out
        viewWHD_21_004 uc = new viewWHD_21_004();
        uc.issuingPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE ADJUSTMENT OUT", false, ViewState.MainWindow);
      }
      else if (type == 4)
      {
        // Issue To Subcon
        viewWHD_21_005 uc = new viewWHD_21_005();
        uc.issuingPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE ISSUE TO SUBCON", false, ViewState.MainWindow);
      }
      else if (type == 5)
      {
        // Issue Special ID
        viewWHD_21_007 uc = new viewWHD_21_007();
        uc.pid = pid;
        WindowUtinity.ShowView(uc, "UPDATE ISSUE SPECIAL ID", false, ViewState.MainWindow);
      }
      else if (type == 6)
      {
        // Issue Request Online
        viewWHD_21_008 uc = new viewWHD_21_008();
        uc.pid = pid;
        WindowUtinity.ShowView(uc, "UPDATE ISSUE REQUEST ONLINE", false, ViewState.MainWindow);
      }
      else if (type == 7)
      {
        // Issue Request Online
        viewWHD_21_009 uc = new viewWHD_21_009();
        uc.pid = pid;
        WindowUtinity.ShowView(uc, "UPDATE ISSUE PAPER", false, ViewState.MainWindow);
      }

      // Search Grid Again 
      this.Search();
    }

    /// <summary>
    /// Print Issuing Note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      bool flag = false;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Print"].Value.ToString()) == 1)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["Type"].Value.ToString()) < 5)
          {
            flag = true;
            long issuingNote = DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString());
            viewWHD_99_001 view = new viewWHD_99_001();
            view.ncategory = 1;
            view.issuingNotePid = issuingNote;
            Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
            break;
          }
          else
          {
            flag = true;
            long issuingNote = DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString());
            viewWHD_99_001 view = new viewWHD_99_001();
            view.ncategory = 3;
            view.issuingNotePid = issuingNote;
            Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
            break;
          }
        }
      }

      if (flag == false)
      {
        WindowUtinity.ShowMessageErrorFromText("Please Select Issuing Note");
        return;
      }
    }

    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion Event
  }
}
