using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_03_020 : MainUserControl
  {
    private bool addCheckBox = false;
    #region Init Form
    public viewBOM_03_020()
    {
      InitializeComponent();
    }

    /// <summary>
    /// frmBOMListPackage_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_03_020_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegMain);

      this.LoadWO();
      this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserName + " | " + SharedObject.UserInfo.LoginDate;
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
      }
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
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
    #endregion Init Form

    #region Process
    private void Search()
    {
      if (this.CheckInvalid())
      {
        DBParameter[] inputParam = new DBParameter[5];
        string text = txtPackageCode.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[0] = new DBParameter("@PackageCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
        }
        text = txtPackageName.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[1] = new DBParameter("@PackageName", DbType.AnsiString, 130, "%" + text.Replace("'", "''") + "%");
        }
        text = txtItemCode.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[2] = new DBParameter("@ItemName", DbType.String, 130, "%" + text.Replace("'", "''") + "%");
        }
        if (ultraCBWO.SelectedRow != null)
        {
          inputParam[3] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultraCBWO.Value.ToString()));
        }
        text = txtBarcode.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[4] = new DBParameter("@Barcode", DbType.AnsiString, 50, "%" + text.Replace("'", "''") + "%");
        }

        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListPackage", inputParam);
        ugrdData.DataSource = dtSource;
        lbCount.Text = string.Format("Count: {0}", (ugrdData.Rows.Count > 0 ? ugrdData.Rows.FilteredInRowCount : 0));
      }
    }

    private void Clear()
    {
      txtItemCode.Text = string.Empty;
      txtPackageCode.Text = string.Empty;
      txtPackageName.Text = string.Empty;
      txtBarcode.Text = string.Empty;
      ultraCBWO.Value = null;
    }

    private bool CheckDeletePackage(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {
        UltraGridRow row = ugrdData.Rows[i];
        if ((bool)ugrdData.Rows[i].Cells["Select"].Value)
        {
          string packageCode = row.Cells["PackageCode"].Value.ToString().Trim();
          string cmd = string.Format(@"SELECT COUNT(*)
                                    FROM TblWHFBox BOX
                                    INNER JOIN TblBOMBoxType BT ON BOX.BoxTypePID = BT.Pid 
                                    INNER JOIN TblBOMPackage PAK ON BT.PackagePid = PAK.Pid
                                    WHERE PAK.PackageCode = '{0}'", packageCode);
          DataTable dtCheckBox = DataBaseAccess.SearchCommandTextDataTable(cmd);
          if (DBConvert.ParseInt(dtCheckBox.Rows[0][0].ToString()) > 0)
          {
            message = string.Format(@"Can't delete because any boxes are using package {0}. Please check again.", packageCode);
            row.Selected = true;
            ugrdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }

          cmd = string.Format(@"SELECT COUNT(*)
                              FROM TblBOMItemInfo AS tbi
                              INNER JOIN TblBOMPackage AS tb ON tb.ItemCode = tbi.ItemCode
								                              AND tb.Revision = tbi.Revision
								                              AND tb.SetDefault = 1
                              WHERE tb.PackageCode = 	'{0}'", packageCode);
          DataTable checkStandard = DataBaseAccess.SearchCommandTextDataTable(cmd);
          if (DBConvert.ParseInt(checkStandard.Rows[0][0].ToString()) > 0)
          {
            message = string.Format(@"Can't delete because package {0} is standard package. Please check again.", packageCode);
            row.Selected = true;
            ugrdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
        }
      }
      return true;
    }


    private bool CheckStandardPackage(out string message)
    {
      message = string.Empty;
      DataTable dtSelected = new DataTable();
      dtSelected.Columns.Add("PackageCode", typeof(System.String));
      dtSelected.Columns.Add("ItemCode", typeof(System.String));
      dtSelected.Columns.Add("Revision", typeof(System.Int32));
      dtSelected.Columns.Add("Standard", typeof(System.String));
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {
        UltraGridRow row = ugrdData.Rows[i];
        if ((bool)ugrdData.Rows[i].Cells["Select"].Value)
        {
          if (row.Cells["Standard"].Value.ToString() == "YES")
          {
            message = string.Format(@"Currently, package {0} is standard. Please check again.", row.Cells["PackageCode"].Value.ToString());
            row.Selected = true;
            ugrdData.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
          DataRow rowSelected = dtSelected.NewRow();
          rowSelected["PackageCode"] = row.Cells["PackageCode"].Value.ToString();
          rowSelected["ItemCode"] = row.Cells["ItemCode"].Value.ToString();
          rowSelected["Revision"] = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          rowSelected["Standard"] = row.Cells["Standard"].Value.ToString();
          dtSelected.Rows.Add(rowSelected);
        }
      }
      if (dtSelected.Rows.Count > 0)
      {
        for (int j = 0; j < dtSelected.Rows.Count; j++)
        {
          string packageA = dtSelected.Rows[j]["PackageCode"].ToString();
          string itemCodeA = dtSelected.Rows[j]["ItemCode"].ToString();
          int revA = DBConvert.ParseInt(dtSelected.Rows[j]["Revision"].ToString());
          for (int k = j + 1; k < dtSelected.Rows.Count; k++)
          {
            string packageB = dtSelected.Rows[k]["PackageCode"].ToString();
            string itemCodeB = dtSelected.Rows[k]["ItemCode"].ToString();
            int revB = DBConvert.ParseInt(dtSelected.Rows[k]["Revision"].ToString());
            if (itemCodeA == itemCodeB && revA == revB)
            {
              message = string.Format(@"Please select one package between {0} and {1} because they are same item {2} (revision {3}).",
                                packageA, packageB, itemCodeB, revB);
              return false;
            }
          }
        }
      }
      else
      {
        message = string.Format(@"Please select package!!!");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Delete package which haven't been used
    /// </summary>
    private void Delete()
    {

      int count = ugrdData.Rows.Count;
      bool success = false;
      for (int i = 0; i < count; i++)
      {
        if ((bool)ugrdData.Rows[i].Cells["Select"].Value)
        {
          string packageCode = ugrdData.Rows[i].Cells["PackageCode"].Value.ToString().Trim();
          string commandText = string.Empty;
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@PackageCode", DbType.AnsiString, 16, packageCode);
          inputParam[1] = new DBParameter("@DeletedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMPackage_Delete", inputParam, outputParam);
          int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          success = (result == 1) ? true : false;
          if (!success)
          {
            WindowUtinity.ShowMessageWarning("WRN0014");
            return;
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      this.Search();
    }


    /// <summary>
    /// set default and package standard for item
    /// </summary>
    private void SetPackageDefault()
    {
      int count = ugrdData.Rows.Count;
      bool success = false;
      for (int i = 0; i < count; i++)
      {
        if ((bool)ugrdData.Rows[i].Cells["Select"].Value)
        {
          string packageCode = ugrdData.Rows[i].Cells["PackageCode"].Value.ToString().Trim();
          string commandText = string.Empty;
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@PackageCode", DbType.AnsiString, 16, packageCode);
          inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfoPackageCode_UpdateSetDefault", inputParam, outputParam);
          int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          success = (result == 1) ? true : false;
          if (!success)
          {
            WindowUtinity.ShowMessageWarning("WRN0014");
            return;
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      this.Search();
    }
    #endregion Process

    #region Event
    /// <summary>
    /// btnSearch_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// btnNew_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewBOM_03_021 view = new viewBOM_03_021();
      Shared.Utility.WindowUtinity.ShowView(view, "Package Information", false, Shared.Utility.ViewState.Window, FormWindowState.Normal);
      //this.Search();

    }


    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnUnLock_Click(object sender, EventArgs e)
    {
      int count = ugrdData.Rows.Count;
      bool success = false;
      for (int i = 0; i < count; i++)
      {
        bool copy = false;
        try
        {
          copy = (bool)ugrdData.Rows[i].Cells["Select"].Value;
        }
        catch
        {
        }
        if (copy)
        {
          string packageCode = ugrdData.Rows[i].Cells["PackageCode"].Value.ToString().Trim();

          DBParameter[] inputParam = new DBParameter[2];

          inputParam[0] = new DBParameter("@PackageCode", DbType.AnsiString, 16, packageCode);

          //string commandText = string.Empty;

          //commandText += " SELECT COUNT(*) ";
          //commandText += " FROM TblWHFBox BOX ";
          //commandText += " INNER JOIN TblBOMBoxType BT ON BOX.BoxTypePID = BT.Pid ";
          //commandText += " INNER JOIN TblBOMPackage PAK ON BT.PackagePid = PAK.Pid ";
          //commandText += " WHERE PAK.PackageCode = '" + packageCode + "'";

          //DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          //if (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) > 0)
          //{
          //  inputParam[1] = new DBParameter("@Confirm", DbType.Int32, 1);
          //  WindowUtinity.ShowMessageWarningFromText(string.Format("This Package {0} was used for some boxes. Please unlock other!!!", packageCode));
          //  return;
          //}
          //else
          //{
          //  inputParam[1] = new DBParameter("@Confirm", DbType.Int32, 0);
          //}
          inputParam[1] = new DBParameter("@Confirm", DbType.Int32, 0);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMPackage_Unlock", inputParam, outputParam);
          int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          success = (result == 1) ? true : false;
        }
      }
      if (!success)
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

    private void ultraCBWO_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }


    private void ugrdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      //ugrdData.SyncWithCurrencyManager = false;
      //ugrdData.StyleSetName = "Excel2013";
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      // Add Column Selected
      if (!e.Layout.Bands[0].Columns.Exists("Select"))
      {
        UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
        checkedCol.Key = "Select";
        checkedCol.Header.Caption = "Chọn";
        //checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;
        checkedCol.DataType = typeof(bool);
        checkedCol.Header.VisiblePosition = 0;
      }

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Select"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

      // Set caption column
      e.Layout.Bands[0].Columns["PackageCode"].Header.Caption = "Mã đóng gói";
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Mã cũ";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã sản phẩn";
      e.Layout.Bands[0].Columns["QuantityBox"].Header.Caption = "Số lượng thùng";
      e.Layout.Bands[0].Columns["QuantityItem"].Header.Caption = "Số lương SP";
      e.Layout.Bands[0].Columns["Status"].Header.Caption = "Trạng thái";
      e.Layout.Bands[0].Columns["Standard"].Header.Caption = "Tiêu chuẩn";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
    }

    private void ugrdData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      string packageCode = string.Empty;
      try
      {
        packageCode = ugrdData.Selected.Rows[0].Cells["PackageCode"].Value.ToString();
      }
      catch { }
      if (packageCode.Trim().Length == 0)
      {
        return;
      }
      viewBOM_03_021 view = new viewBOM_03_021();
      view.packageCode = packageCode;
      Shared.Utility.WindowUtinity.ShowView(view, "Package Information", false, Shared.Utility.ViewState.Window, FormWindowState.Normal);
      this.Search();
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      btnDelete.Enabled = false;
      string message = string.Empty;

      bool success = this.CheckDeletePackage(out message);
      if (success)
      {
        this.Delete();
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText(message);
      }
      btnDelete.Enabled = true;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      btnClear.Enabled = false;
      this.Clear();
      btnClear.Enabled = true;
    }

    private void btnSetDefault_Click(object sender, EventArgs e)
    {
      btnSetDefault.Enabled = false;
      string message = string.Empty;

      bool success = this.CheckStandardPackage(out message);
      if (success)
      {
        this.SetPackageDefault();
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText(message);
      }
      btnSetDefault.Enabled = true;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugrdData, "Package List");
    }
    #endregion Event    
  }
}
