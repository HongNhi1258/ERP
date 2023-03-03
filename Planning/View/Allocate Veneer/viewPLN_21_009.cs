/*
  Author        : 
  Create date   : 30/05/2013
  Decription    : Material Relation
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
  public partial class viewPLN_21_009 : MainUserControl
  {
    #region Field
    public string materialCode = string.Empty;
    public string materialName = string.Empty;
    #endregion Field

    #region Init Data

    public viewPLN_21_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load From
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_21_009_Load(object sender, EventArgs e)
    {
      this.LoadInit();
    }

    /// <summary>
    /// Load Init Data
    /// </summary>
    private void LoadInit()
    {
      txtMaterial.Text = this.materialCode + " - " + this.materialName;
      // Load Grid
      this.LoadData();
      // Load Group
      this.LoadMaterial();
    }

    /// <summary>
    /// Load MaterialCode
    /// </summary>
    private void LoadMaterial()
    {
      string commandText = string.Empty;
      commandText = "SELECT MAT.MaterialCode FROM VBOMMaterials MAT WHERE MAT.Warehouse = 2";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultDDMaterial.DataSource = dt;
      ultDDMaterial.DisplayMember = "MaterialCode";
      ultDDMaterial.ValueMember = "MaterialCode";
      ultDDMaterial.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Grid
    /// </summary>
    private void LoadData()
    {
      string commandText = string.Empty;
      commandText += " SELECT MRE.Pid, MRE.MaterialCode, MAT.MaterialNameEn ,MAT.MaterialNameVn, MAT.Unit, MRE.Coefficient, 0 [Select]";
      commandText += " FROM tblPLNVeneerMaterialRelation MRE";
      commandText += "    LEFT JOIN VBOMMaterials MAT ON MAT.MaterialCode = MRE.MaterialCode";
      commandText += " WHERE MRE.ParentMaterialCode = '" + this.materialCode + "'";
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
        // Check MaterialCode
        if (row.Cells["MaterialCode"].Value.ToString().Trim().Length == 0)
        {
          message = "MaterialCode";
          return false;
        }
        // Check Coefficient
        if (DBConvert.ParseDouble(row.Cells["Coefficient"].Value.ToString()) <= 0)
        {
          message = "Coefficient";
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
          DBParameter[] input = new DBParameter[6];
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          input[1] = new DBParameter("@ParentMaterialCode", DbType.String, this.materialCode);
          input[2] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
          input[3] = new DBParameter("@Coefficient", DbType.String, DBConvert.ParseDouble(row["Coefficient"].ToString()));
          input[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          input[5] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          // Output
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.SearchStoreProcedure("spPLNVeneerMaterialRelation_Edit", input, output);
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
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = this.ultDDMaterial;
      e.Layout.Bands[0].Columns["MaterialNameEn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Coefficient"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.Gainsboro;
      e.Layout.Bands[0].Columns["Coefficient"].CellAppearance.BackColor = Color.Gainsboro;

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

      switch (columnName)
      {
        case "materialcode":
          row.Cells["MaterialNameEn"].Value = null;
          row.Cells["MaterialNameVn"].Value = null;
          row.Cells["Unit"].Value = null;

          commandText = "SELECT MaterialCode, MaterialNameEn, MaterialNameVn, Unit FROM VBOMMaterials WHERE MaterialCode = '" + row.Cells["MaterialCode"].Value.ToString() + "'";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt == null || dt.Rows.Count == 0)
          {
            return;
          }
          row.Cells["MaterialNameEn"].Value = dt.Rows[0]["MaterialNameEn"].ToString();
          row.Cells["MaterialNameVn"].Value = dt.Rows[0]["MaterialNameVn"].ToString();
          row.Cells["Unit"].Value = dt.Rows[0]["Unit"].ToString();
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
        case "materialcode":
          commandText += "  SELECT MaterialCode FROM VBOMMaterials WHERE MaterialCode = '" + text + "'";
          DataTable dt1 = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt1 == null || dt1.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "MaterialCode");
            e.Cancel = true;
          }

          // Check Trung Cha
          if (string.Compare(e.Cell.Row.Cells["MaterialCode"].Text, this.materialCode, true) == 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "MaterialCode");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }

          // Check Trung Luoi
          string select = string.Format(@"MaterialCode = '{0}'", e.Cell.Row.Cells["MaterialCode"].Text);
          DataTable dt2 = (DataTable)ultData.DataSource;
          DataRow[] foundRow = dt2.Select(select);
          if (foundRow.Length >= 1)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "MaterialCode");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        case "cofficient":
          if (DBConvert.ParseDouble(e.Cell.Row.Cells["Coefficient"].Text) <= 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Coefficient");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        default:
          break;
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
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "MaterialCode Relation").ToString() == "No")
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

          DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerMaterialRelation_Delete", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0093", "Material Relation");
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
