/*
  Author        : 
  Create date   : 02/03/2013
  Decription    : Group Relation
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.Planning
{
  public partial class viewPLN_20_009 : MainUserControl
  {
    #region Field
    public string groupParent = string.Empty;
    public string categoryParent = string.Empty;
    public string categoryName = string.Empty;

    bool changeGroup = false;
    bool changeCategory = false;
    #endregion Field

    #region Init Data

    public viewPLN_20_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load From
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_20_009_Load(object sender, EventArgs e)
    {
      this.LoadInit();
    }

    /// <summary>
    /// Load Init Data
    /// </summary>
    private void LoadInit()
    {
      txtGroupCategory.Text = this.groupParent + " - " + this.categoryParent;
      txtName.Text = this.categoryName;

      // Load Grid
      this.LoadData();

      // Load Group
      this.LoadGroup();
    }

    /// <summary>
    /// Load Group
    /// </summary>
    private void LoadGroup()
    {
      string commandText = string.Empty;
      commandText = "SELECT [Group] FROM TblGNRMaterialGroup WHERE Warehouse = 3";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultDDGroup.DataSource = dt;
      ultDDGroup.DisplayMember = "Group";
      ultDDGroup.ValueMember = "Group";
      ultDDGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Category
    /// </summary>
    /// <param name="group"></param>
    private void LoadCategory(string group)
    {
      if (group.Length > 0)
      {
        string commandText = string.Empty;
        commandText += "  SELECT CAT.Category";
        commandText += "  FROM TblGNRMaterialGroup GRP";
        commandText += "      INNER JOIN TblGNRMaterialCategory CAT ON GRP.[Group] = CAT.[Group]";
        commandText += "  WHERE GRP.Warehouse = 3 AND GRP.[Group] = '" + group + "'";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt == null)
        {
          return;
        }
        ultDDCategory.DataSource = dt;
      }
      else
      {
        ultDDCategory.DataSource = null;
      }
    }

    /// <summary>
    /// Load Grid
    /// </summary>
    private void LoadData()
    {
      string commandText = string.Empty;
      commandText += " SELECT REL.Pid, REL.[Group], GRP.Name GroupName, REL.Category, CAT.Name CategoryName, REL.Cofficient, 0 [Select]";
      commandText += " FROM TblPLNWoodsGroupRelation REL";
      commandText += "    LEFT JOIN TblGNRMaterialGroup GRP ON GRP.[Group] = REL.[Group]";
      commandText += "    LEFT JOIN TblGNRMaterialCategory CAT ON CAT.[Group] = REL.[Group] AND CAT.Category = REL.Category";
      commandText += " WHERE GRP.Warehouse = 3 AND REL.ParentGroup = '" + this.groupParent + "' AND REL.ParentCategory = '" + this.categoryParent + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultData.DataSource = dt;
    }

    #endregion Init Data

    #region Function

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        // Check Group
        if (row.Cells["Group"].Value.ToString().Trim().Length == 0)
        {
          message = "Group";
          return false;
        }
        // Check Category
        if (row.Cells["Category"].Value.ToString().Trim().Length == 0)
        {
          message = "Category";
          return false;
        }
        // Check Cofficient
        if (DBConvert.ParseDouble(row.Cells["Cofficient"].Value.ToString()) <= 0)
        {
          message = "Cofficient";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (row.RowState != DataRowState.Deleted &&
          (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added))
        {
          DBParameter[] input = new DBParameter[8];
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          input[1] = new DBParameter("@ParentGroup", DbType.String, this.groupParent);
          input[2] = new DBParameter("@ParentCatagory", DbType.String, this.categoryParent);
          input[3] = new DBParameter("@Group", DbType.String, row["Group"].ToString());
          input[4] = new DBParameter("@Catagory", DbType.String, row["Category"].ToString());
          input[5] = new DBParameter("@Cofficient", DbType.String, DBConvert.ParseDouble(row["Cofficient"].ToString()));
          input[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          input[7] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          // Output
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.SearchStoreProcedure("spPLNWoodsGroupRelation_Edit", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Group"].ValueList = this.ultDDGroup;
      e.Layout.Bands[0].Columns["Category"].ValueList = this.ultDDCategory;
      e.Layout.Bands[0].Columns["GroupName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CategoryName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Cofficient"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["Group"].CellAppearance.BackColor = Color.Gainsboro;
      e.Layout.Bands[0].Columns["Category"].CellAppearance.BackColor = Color.Gainsboro;
      e.Layout.Bands[0].Columns["Cofficient"].CellAppearance.BackColor = Color.Gainsboro;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      string commandText = string.Empty;
      //DataTable dt = new DataTable();

      switch (columnName)
      {
        case "group":
          this.changeGroup = false;
          this.changeCategory = false;
          row.Cells["GroupName"].Value = null;
          row.Cells["Category"].Value = null;
          row.Cells["CategoryName"].Value = null;

          commandText = "SELECT [Group], Name FROM TblGNRMaterialGroup WHERE Warehouse = 3 AND [Group] = '" + row.Cells["Group"].Value.ToString() + "'";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt == null || dt.Rows.Count == 0)
          {
            return;
          }
          row.Cells["GroupName"].Value = dt.Rows[0]["Name"].ToString();
          this.LoadCategory(dt.Rows[0]["Group"].ToString());
          break;
        case "category":
          this.changeCategory = true;
          row.Cells["categoryName"].Value = null;
          commandText += "  SELECT CAT.Name";
          commandText += "  FROM TblGNRMaterialGroup GRP";
          commandText += "      INNER JOIN TblGNRMaterialCategory CAT ON GRP.[Group] = CAT.[Group]";
          commandText += "  WHERE GRP.Warehouse = 3 AND GRP.[Group] = '" + row.Cells["Group"].Value.ToString() + "'";
          commandText += "        AND CAT.Category = '" + row.Cells["Category"].Value.ToString() + "'";
          DataTable dt1 = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt1 == null || dt1.Rows.Count == 0)
          {
            return;
          }
          row.Cells["CategoryName"].Value = dt1.Rows[0]["Name"].ToString();
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string text = e.Cell.Text.Trim();
      string commandText = string.Empty;

      switch (columnName.ToLower())
      {
        case "group":
          this.changeGroup = true;
          commandText = "SELECT [Group], Name FROM TblGNRMaterialGroup WHERE Warehouse = 3 AND [Group] = '" + text + "'";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt == null || dt.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Group");
            this.changeGroup = false;
            e.Cancel = true;
          }
          break;
        case "category":
          if (this.changeGroup == false && this.changeCategory == true)
          {
            commandText += "  SELECT CAT.Name";
            commandText += "  FROM TblGNRMaterialGroup GRP";
            commandText += "      INNER JOIN TblGNRMaterialCategory CAT ON GRP.[Group] = CAT.[Group]";
            commandText += "  WHERE GRP.Warehouse = 3 AND GRP.[Group] = '" + e.Cell.Row.Cells["Group"].Value.ToString() + "'";
            commandText += "        AND CAT.Category = '" + text + "'";
            DataTable dt1 = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt1 == null || dt1.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Category");
              e.Cancel = true;
            }

            // Check Trung Cha
            if (string.Compare(e.Cell.Row.Cells["Group"].Value.ToString(), this.groupParent, true) == 0 &&
                string.Compare(e.Cell.Row.Cells["Category"].Text.ToString(), this.categoryParent, true) == 0)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "Category");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }

            // Check Trung Luoi
            string select = string.Format(@"Group = '{0}' AND Category = '{1}'", e.Cell.Row.Cells["Group"].Value.ToString(), e.Cell.Row.Cells["Category"].Text);
            DataTable dt2 = (DataTable)ultData.DataSource;
            DataRow[] foundRow = dt2.Select(select);
            if (foundRow.Length >= 1)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "Category");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }
          }
          break;
        case "cofficient":
          if (DBConvert.ParseDouble(e.Cell.Row.Cells["Cofficient"].Text) <= 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Cofficient");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before Cell Actice
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string commandText = string.Empty;

      if (e.Cell.Row.Cells.Exists("Group"))
      {
        string group = e.Cell.Row.Cells["Group"].Value.ToString();
        this.LoadCategory(group);
      }
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      this.LoadData();
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Group Relation").ToString() == "No")
      {
        return;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1 &&
            DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spPLNWoodsGroupRelation_Delete", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0093", "Group Relation");
            return;
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0002");

      this.LoadData();
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event    
  }
}
