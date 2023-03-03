/*
  Author        : 
  Create date   : 06/03/2013
  Decription    : Control Group - Category
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_20_010 : MainUserControl
  {
    #region Field
    private bool loadCheck = true;
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewPLN_20_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewPLN_07_010_Load(object sender, EventArgs e)
    {
      // Load Control Type (WO & Item)
      this.LoadControlType();
    }
    #endregion Init Data

    #region Process Data
    /// <summary>
    /// Find and show data result
    /// </summary>
    private void Search()
    {
      string commandText = string.Empty;
      commandText += " SELECT GRP.[Group], GRP.Name GroupName, CAT.Category, CAT.Name CategoryName, ";
      commandText += " 		GRP.[Group] + '-' + CAT.Category GroupCategory, ISNULL(CAT.IsControl, 0) IsControl, ";
      commandText += " 		CASE WHEN ISNULL(CAT.IsControl, 0) != 0 THEN ISNULL(CAT.ControlType, 0) ELSE NULL END ControlType ";
      commandText += " FROM TblGNRMaterialGroup GRP ";
      commandText += " 	INNER JOIN TblGNRMaterialCategory CAT ON GRP.[Group] = CAT.[Group] ";
      commandText += " WHERE Warehouse = 3 ";
      commandText += "  AND GRP.[Group] + '-' + CAT.Category LIKE '%" + this.txtGroupCategory.Text + "%'";

      DataTable dataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dataSource;

      int count = ultData.DisplayLayout.Bands[0].Columns.Count;
      for (int i = 0; i < count; i++)
      {
        string name = ultData.DisplayLayout.Bands[0].Columns[i].ToString();
        if (string.Compare(name, "IsControl", true) != 0 && string.Compare(name, "ControlType", true) != 0)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        else
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightCyan;
        }
      }

      count = ultData.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        int control = DBConvert.ParseInt(row.Cells["IsControl"].Value.ToString());
        row.Cells["ControlType"].Activation = (control == 1) ? Activation.AllowEdit : Activation.ActivateOnly;
      }
    }
    /// <summary>
    /// Bind control type
    /// </summary>
    private void LoadControlType()
    {
      DataTable dataSource = new DataTable();
      dataSource.Columns.Add("Value", typeof(System.Int32));
      dataSource.Columns.Add("Text", typeof(System.String));

      DataRow row = dataSource.NewRow();
      row["Value"] = 1;
      row["Text"] = "WO & Item";
      dataSource.Rows.Add(row);
      udrpControlType.DataSource = dataSource;

      udrpControlType.ValueMember = "Value";
      udrpControlType.DisplayMember = "Text";
      udrpControlType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpControlType.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckValidData()
    {
      int count = ultData.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        int control = DBConvert.ParseInt(row.Cells["IsControl"].Value.ToString());
        if (control == 1)
        {
          int controlType = DBConvert.ParseInt(row.Cells["ControlType"].Value.ToString());
          if (controlType != 1)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Control Type");
            row.Selected = true;
            return false;
          }
        }
      }
      return true;
    }

    private bool SaveData()
    {
      DataTable dtSource = (DataTable)this.ultData.DataSource;

      foreach (DataRow row in dtSource.Rows)
      {
        string storeName = string.Empty;
        DBParameter[] input = new DBParameter[3];
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

        input[0] = new DBParameter("@Name", DbType.String, row["GroupCategory"].ToString());
        input[1] = new DBParameter("@IsControl", DbType.Int32, DBConvert.ParseInt(row["IsControl"].ToString()));
        if (DBConvert.ParseInt(row["ControlType"].ToString()) == 1)
        {
          input[2] = new DBParameter("@ControlType", DbType.Int32, DBConvert.ParseInt(row["ControlType"].ToString()));
        }
        else
        {
          input[2] = new DBParameter("@ControlType", DbType.Int32, 0);
        }
        DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialCategoryControl_Update", input, output);

        if (DBConvert.ParseInt(output[0].Value.ToString()) == 0)
        {
          return false;
        }
      }
      return true;
    }
    #endregion Process Data

    #region Events
    /// <summary>
    /// Find and show result data from condition group, category, material code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;

      // Search Data
      this.Search();

      this.btnSearch.Enabled = true;
    }

    /// <summary>
    /// Hiden/show, format and resize column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["GroupCategory"].Header.Caption = "Group-Category";
      e.Layout.Bands[0].Columns["IsControl"].Header.Caption = "Control";
      e.Layout.Bands[0].Columns["IsControl"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["ControlType"].Header.Caption = "Control Type";
      e.Layout.Bands[0].Columns["ControlType"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ControlType"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ControlType"].ValueList = udrpControlType;

      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Change control --> change activation control type 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      int value = DBConvert.ParseInt(e.Cell.Text.ToString());
      if (string.Compare(columnName, "IsControl", true) == 0)
      {
        if (value == 1)
        {
          e.Cell.Row.Cells["ControlType"].Activation = Activation.AllowEdit;
          e.Cell.Row.Cells["ControlType"].Value = 1;
        }
        else
        {
          e.Cell.Row.Cells["ControlType"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["ControlType"].Value = DBNull.Value;
          this.loadCheck = false;
          chkAll.Checked = false;
          this.loadCheck = true;
        }
      }
    }

    /// <summary>
    /// Save data on screen in to data base
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = false;
      if (!this.CheckValidData())
      {
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
      this.Search();
    }

    /// <summary>
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Selected all Material
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.loadCheck)
      {
        int check = (chkAll.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Cells["IsControl"].Value = check;
          if (check == 0)
          {
            ultData.Rows[i].Cells["ControlType"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["ControlType"].Value = DBNull.Value;
          }
          else
          {
            ultData.Rows[i].Cells["ControlType"].Activation = Activation.AllowEdit;
            ultData.Rows[i].Cells["ControlType"].Value = 1;
          }
        }
      }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion Events
  }
}
