/*
  Author      : Vo Van Duy Qui
  Date        : 10/03/2011
  Description : Material Category
  Update By   :  (04/05/2011)
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_01_002 : MainUserControl
  {
    #region variable 
    public string materialGroup = string.Empty;
    public long wareHouse = long.MinValue;

    private IList listDeletingCategory = new ArrayList();
    private IList listDeletedCategory = new ArrayList();
    #endregion variable

    #region Init
    public viewPUR_01_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_002_Load(object sender, EventArgs e)
    {
      btnControl.Visible = false;
      this.LoadCBMaterialGroup();
      txtGroup.Text = this.materialGroup;

      this.LoadData(this.materialGroup);
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["WarningPricePercent"].Hidden = true;
      e.Layout.Bands[0].Columns["LeadTimeDeadLinePacking"].Hidden = true;
      e.Layout.Bands[0].Columns["FormulaStandardCostPercent"].Hidden = true;
      e.Layout.Bands[0].Columns["Control"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Control"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Control"].MinWidth = 70;
      //e.Layout.Bands[0].Columns["UnitPrice"].MaxWidth = 90;
      //e.Layout.Bands[0].Columns["UnitPrice"].MinWidth = 90;
      e.Layout.Bands[0].Columns["Control"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["WarningPricePercent"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTimeDeadLinePacking"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["FormulaStandardCostPercent"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["UnitPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Category"].Header.Caption = "Ngành hàng";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Tên ngành hàng";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["Warehouses"].Header.Caption = "Kho";
      e.Layout.Bands[0].Columns["Warehouses"].Header.Caption = "Chọn";

      e.Layout.Bands[0].Columns["WarningPricePercent"].Header.Caption = "WarningPricePercent(%)";
      e.Layout.Bands[0].Columns["FormulaStandardCostPercent"].Header.Caption = "FormulaStandardCostPercent(%)";
      //e.Layout.Bands[0].Columns["UnitPrice"].Header.Caption = "UnitPrice (VND)";
      e.Layout.Bands[0].Columns["WarningPricePercent"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["WarningPricePercent"].MinWidth = 150;
      //e.Layout.Bands[0].Columns["UnitPrice"].Format = "###,###.##";
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string value = ultData.Rows[i].Cells["Category"].Value.ToString();
        if (value.Length > 0)
        {
          ultData.Rows[i].Cells["Category"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
          ultData.Rows[i].Cells["Name"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[0].Columns["WHPids"].Hidden = true;

      ucbeMaterialGroup.CheckedListSettings.CheckBoxStyle = Infragistics.Win.CheckStyle.CheckBox;
      ucbeMaterialGroup.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
      ucbeMaterialGroup.CheckedListSettings.ItemCheckArea = Infragistics.Win.ItemCheckArea.Item;
      ucbeMaterialGroup.CheckedListSettings.ListSeparator = ",";
      e.Layout.Bands[0].Columns["Warehouses"].EditorComponent = ucbeMaterialGroup;
    }
    #endregion Init

    #region Load Data

    private void LoadCBMaterialGroup()
    {
      string cmd = string.Format(@"SELECT Pid Value, DisplayText Display
              FROM VWHDWarehouseList
              ORDER BY DisplayText");
      DataTable dtMaterialGroup = DataBaseAccess.SearchCommandTextDataTable(cmd);

      this.ucbeMaterialGroup.DataSource = dtMaterialGroup;
      this.ucbeMaterialGroup.DisplayMember = "Display";
      ucbeMaterialGroup.ValueMember = "Value";
      ucbeMaterialGroup.CheckedListSettings.CheckBoxStyle = Infragistics.Win.CheckStyle.CheckBox;
      ucbeMaterialGroup.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
      ucbeMaterialGroup.CheckedListSettings.ItemCheckArea = Infragistics.Win.ItemCheckArea.Item;
      ucbeMaterialGroup.CheckedListSettings.ListSeparator = ",";
    }

    /// <summary>
    /// Load Data
    /// </summary>
    /// <param name="group"></param>
    private void LoadData(string group)
    {
      string value = txtGroup.Text.Trim();
      string commandText = string.Format(@"SELECT Category, Name, Remark, LeadTimeDeadLinePacking, WarningPricePercent, FormulaStandardCostPercent, WHPids, REPLACE(WHPids, '|', ',') Warehouses, ISNULL(PurControl, 0) [Control], 0 AS Selected 
                                FROM TblGNRMaterialCategory WHERE [Group] = {0}", value);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      dtSource.Columns["Selected"].DefaultValue = 0; // Add HangNguyen
      ultData.DataSource = dtSource;
    }
    #endregion Load Data

    #region Validation
    /// <summary>
    /// Check Input
    /// </summary>
    /// <param name="group"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private bool ValidationInput(string group, DataTable dtSource)
    {
      bool result = true;
      foreach (DataRow row in dtSource.Rows)
      {
        string storeName = string.Empty;
        DBParameter[] input = new DBParameter[2];
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

        //// Check Warning Price Percent
        //if(DBConvert.ParseDouble(row["WarningPricePercent"].ToString()) == double.MinValue)
        //{
        //  WindowUtinity.ShowMessageError("WRN0013", "Warning Price Percent");
        //  result = false;
        //  return result;
        //}


        //// Check LeadTime DeadLine Packing
        //if (row["LeadTimeDeadLinePacking"].ToString().Length > 0 && DBConvert.ParseDouble(row["LeadTimeDeadLinePacking"].ToString()) <= 0)
        //{
        //  WindowUtinity.ShowMessageError("WRN0013", "LeadTime DeadLine Packing");
        //  result = false;
        //  return result;
        //}

        // End
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          string category = string.Empty;
          long check = long.MinValue;
          category = row["Category"].ToString().Trim();
          if (category.Length == 0)
          {
            WindowUtinity.ShowMessageError("WRN0013", "Category");
            result = false;
            return result;
          }
          else
          {
            try
            {
              check = DBConvert.ParseLong(category);
            }
            catch { }
            if (check == long.MinValue || category.Length < 2)
            {
              WindowUtinity.ShowMessageError("ERR0073", category);
              result = false;
              return result;
            }
          }
          input[0] = new DBParameter("@Group", DbType.AnsiString, 3, group);
          input[1] = new DBParameter("@Category", DbType.AnsiString, 2, category);
          int count = int.MinValue;
          count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText("SELECT dbo.FPURCheckCategoryInMaterialCategory(@Group,@Category)", input));

          if (row.RowState == DataRowState.Added)
          {
            if (count == 1)
            {
              WindowUtinity.ShowMessageError("ERR0028", group);
              result = false;
              return result;
            }
          }
          else
          {
            if (count > 1)
            {
              WindowUtinity.ShowMessageError("ERR0028", group);
              result = false;
              return result;
            }
          }
          // Check Name
          string name = string.Empty;
          name = row["Name"].ToString().Trim();
          if (name.Length == 0)
          {
            WindowUtinity.ShowMessageError("WRN0013", "Name");
            result = false;
            return result;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Check Delete
    /// </summary>
    /// <returns></returns>
    private bool ValidateDelete(string materialGroup)
    {
      bool result = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
        if (selected == 1)
        {
          string category = ultData.Rows[i].Cells["Category"].Value.ToString();
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@MaterialGroup", DbType.String, materialGroup);
          inputParam[1] = new DBParameter("@Category", DbType.String, category);
          int count = int.MinValue;
          count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText("SELECT dbo.FPURCheckCategoryInMaterialInformation(@MaterialGroup, @Category)", inputParam));
          if (count == 1)
          {
            WindowUtinity.ShowMessageError("ERR0054", category);
            result = false;
            break;
          }
        }
      }
      return result;
    }
    #endregion Validation

    #region Event
    /// <summary>
    /// Close tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        int countCheck = 0;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            countCheck++;
          }
        }
        if (countCheck == 0)
        {
          return;
        }
        DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
        if (result == DialogResult.Yes)
        {
          bool check = ValidateDelete(this.materialGroup);
          string group = txtGroup.Text.Trim();
          if (check)
          {
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
              if (selected == 1)
              {
                string category = ultData.Rows[i].Cells["Category"].Value.ToString();
                DBParameter[] inputParam = new DBParameter[2];
                inputParam[0] = new DBParameter("@Group", DbType.AnsiString, 3, group);
                inputParam[1] = new DBParameter("@Category", DbType.AnsiString, 2, category);
                DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialCategory_Delete", inputParam);

              }
            }
            WindowUtinity.ShowMessageSuccess("MSG0002");
            // Load Data
            this.LoadData(group);
          }
        }
      }
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string group = txtGroup.Text.Trim();
      DataTable dtSource = (DataTable)ultData.DataSource;
      bool checkSave = ValidationInput(group, dtSource);
      if (checkSave)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          string storeName = string.Empty;
          string category = row["Category"].ToString().Trim();
          string name = row["Name"].ToString().Trim();
          string remark = row["Remark"].ToString().Trim();
          //double price = DBConvert.ParseDouble(row["UnitPrice"].ToString().Trim());
          double warningPricePercent = DBConvert.ParseDouble(row["WarningPricePercent"].ToString());
          double leadTimeDeadLinePacking = DBConvert.ParseDouble(row["LeadTimeDeadLinePacking"].ToString());
          double formulaStandardCostPercent = DBConvert.ParseDouble(row["FormulaStandardCostPercent"].ToString());
          int purcontrol = DBConvert.ParseInt(row["Control"].ToString());
          string whPids = row["WHPids"].ToString();

          DBParameter[] input = new DBParameter[9];
          input[0] = new DBParameter("@Group", DbType.AnsiString, 3, group);
          input[1] = new DBParameter("@Category", DbType.AnsiString, 2, category);
          input[2] = new DBParameter("@Name", DbType.String, 128, name);
          input[3] = new DBParameter("@Remark", DbType.String, 512, remark);
          if (warningPricePercent != double.MinValue)
          {
            input[4] = new DBParameter("@WarningPricePercent", DbType.Double, warningPricePercent);
          }
          if (leadTimeDeadLinePacking != double.MinValue)
          {
            input[5] = new DBParameter("@LeadTimeDeadLinePacking", DbType.Double, leadTimeDeadLinePacking);
          }
          if (formulaStandardCostPercent != double.MinValue)
          {
            input[6] = new DBParameter("@FormulaStandardCostPercent", DbType.Double, formulaStandardCostPercent);
          }
          input[7] = new DBParameter("@PurControl", DbType.Int32, purcontrol);
          input[8] = new DBParameter("@WHPids", DbType.AnsiString, 64, whPids);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          if (row.RowState == DataRowState.Added)
          {
            storeName = "spGNRMaterialCategory_Insert";
          }
          else if (row.RowState == DataRowState.Modified)
          {
            storeName = "spGNRMaterialCategory_Update";
          }
          if (storeName.Length > 0)
          {
            DataBaseAccess.ExecuteStoreProcedure(storeName, input);
          }
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData(group);
      }
    }

    private void btnControl_Click(object sender, EventArgs e)
    {
      string group = txtGroup.Text.Trim();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Control"].Value.ToString());
        string category = ultData.Rows[i].Cells["Category"].Value.ToString();
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@Group", DbType.AnsiString, 3, group);
        inputParam[1] = new DBParameter("@Category", DbType.AnsiString, 2, category);
        inputParam[2] = new DBParameter("@Control", DbType.Int32, selected);
        DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialCategoryPurControl_Update", inputParam);
      }
      WindowUtinity.ShowMessageSuccess("MSG0001");
      // Load Data
      this.LoadData(group);
    }
    #endregion Event

    #region UtraGridView Handle Event
    /// <summary>
    /// Double Click==> Open Form 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
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

      if (ultData.Rows.Count > 0 && ultData.Selected != null)
      {
        string category = ultData.Selected.Rows[0].Cells["Category"].Value.ToString().Trim();
        if (category.Length > 0)
        {
          viewPUR_01_005 uc = new viewPUR_01_005();
          uc.materialGroup = this.materialGroup;
          uc.materialCategory = category;
          Shared.Utility.WindowUtinity.ShowView(uc, "MATERIAL LIST", false, DaiCo.Shared.Utility.ViewState.MainWindow);
        }
      }
    }

    /// <summary>
    /// Format 2 char
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowInsert(object sender, Infragistics.Win.UltraWinGrid.BeforeRowInsertEventArgs e)
    {
      e.Band.Columns["Category"].FieldLen = 2;
    }
   

    private void ultData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      switch (colName)
      {
        case "Warehouses":
          {
            CheckedValueListItemsCollection chekedItems = ucbeMaterialGroup.CheckedItems;
            DataTable dt = new DataTable();
            dt.Columns.Add("WHPids", typeof(String));
            foreach (ValueListItem item in chekedItems)
            {

              DataRow row_ = dt.NewRow();
              row_[0] = item.DataValue;
              dt.Rows.Add(row_);
            }
            string warehouses = string.Empty;
            for (int l = 0; l < dt.Rows.Count; l++)
            {
              if (l == 0)
              {
                warehouses = dt.Rows[l]["WHPids"].ToString();
              }
              else
              {
                warehouses = warehouses + "|" + dt.Rows[l]["WHPids"].ToString();
              }

            }
            e.Cell.Row.Cells["WHPids"].Value = warehouses;
            break;
          }
        default:
          break;
      }
    }
   

    private void ultData_BeforeCellActivate(object sender, Infragistics.Win.UltraWinGrid.CancelableCellEventArgs e)
    {
      this.LoadCBMaterialGroup();
    }
    #endregion UtraGridView Handle Event
  }
}
