using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_10_002 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRAccessGroupWarehouse_InitData");
      Utility.LoadUltraCombo(ucbUserGroup, ds.Tables[0], "Value", "Display", false, new string[] { "Value", "Display" });

    }
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void LoadData()
    {
      this.NeedToSave = false;
      int paramNumber = 1;
      string storeName = "spGNRAccessGroupWarehouse_Load ";

      int userGroup = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbUserGroup));
      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@UserGroup", DbType.Int32, userGroup);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugrdData.DataSource = dtSource;

      lblCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

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

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (ucbUserGroup.SelectedRow == null)
      {
        errorMessage = "User Group";
        return false;
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        //Insert/Delete     
        DataTable dtDetail = (DataTable)ugrdData.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[5];
            int pid = DBConvert.ParseInt(row["Pid"].ToString());
            int agwPid = DBConvert.ParseInt(row["AGWPid"].ToString());
            int select = DBConvert.ParseInt(row["Selected"].ToString());

            inputParam[0] = new DBParameter("@WarehousePid", DbType.Int64, pid);

            if (ucbUserGroup.Text.Length > 0)
            {
              inputParam[1] = new DBParameter("@UserGroup", DbType.Int32, DBConvert.ParseInt(ucbUserGroup.Value.ToString()));
            }

            inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[3] = new DBParameter("@Selected", DbType.Int32, select);
            if (agwPid > 0)
            {
              inputParam[4] = new DBParameter("@Pid", DbType.Int32, agwPid);
            }

            DataBaseAccess.ExecuteStoreProcedure("spGNRAccessGroupWarehouse_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public viewWHD_10_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_10_002_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegMain);

      //Init Data
      this.InitData();
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.LoadData();
      }
    }

    private void ugrdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ugrdData.SyncWithCurrencyManager = false;
      ugrdData.StyleSetName = "Excel2013";

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {
        ugrdData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      // Hide column 
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["AGWPid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["Code"].Header.Caption = "Warehouse Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Warehouse Name";
      e.Layout.Bands[0].Columns["Selected"].Header.Caption = "Select";

      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Set Width
      e.Layout.Bands[0].Columns["Code"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Name"].MinWidth = 120;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 100;

      //read only



      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Code"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      /*
			// Set auto complete combo in grid
			e.Layout.Bands[0].Columns[""].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
			e.Layout.Bands[0].Columns[""].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

			// Enable support for displaying errors through IDataErrorInfo interface. By default
				  // the functionality is disabled.
				  e.Layout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

				  // Set data error related appearances.
				  e.Layout.Override.DataErrorCellAppearance.ForeColor = Color.Red;
				  e.Layout.Override.DataErrorRowAppearance.BackColor = Color.LightYellow;
				  e.Layout.Override.DataErrorRowSelectorAppearance.BackColor = Color.Green;

				  // Make the row selectors bigger so they can accomodate the data error icon as 
				  // well active row indicator.
				  e.Layout.Override.RowSelectorWidth = 32;

			// Allow update, delete, add new
			e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
			e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
			e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

			// Hide column
			e.Layout.Bands[0].Columns[""].Hidden = true;

			// Set caption column
			e.Layout.Bands[0].Columns[""].Header.Caption = "\n";

			// Set dropdownlist for column
			e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;

			// Set Align
			e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
			e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;

			// Set Width
			e.Layout.Bands[0].Columns[""].MaxWidth = 100;
			e.Layout.Bands[0].Columns[""].MinWidth = 100;

			// Set Column Style
			e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

			// Add Column Selected
			if (!e.Layout.Bands[0].Columns.Exists("Select"))
			{
			  UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
			  checkedCol.Key = "Select";
			  checkedCol.Header.Caption = string.Empty;
			  checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;        
			  checkedCol.DataType = typeof(bool);
			  checkedCol.Header.VisiblePosition = 0;
			} 

			// Set color
			ugrdData.Rows[0].Appearance.BackColor = Color.Yellow;
			ugrdData.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
			e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;

			// Set header height
			e.Layout.Bands[0].ColHeaderLines = 2;

			// Read only
			e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
			ugrdData.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
			ugrdData.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

			// Format date (dd/MM/yy)
			e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
			e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

			// Set language
			e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
			 //Cell multi line
			e.Layout.Bands[0].Columns["Remark"].CellMultiLine = DefaultableBoolean.True;
			*/
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ucbUserGroup_ValueChanged(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void ugrdData_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void ugrdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void ugrdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      //Utility.ExportToExcelWithDefaultPath(ugrdData, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugrdData);
      }
    }

    private void ugrdData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugrdData.Selected.Rows.Count > 0 || ugrdData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugrdData, new Point(e.X, e.Y));
        }
      }
    }
    #endregion event


  }
}
