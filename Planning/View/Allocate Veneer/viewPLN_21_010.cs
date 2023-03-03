/*
  Author        : 
  Create date   : 06/03/2013
  Decription    : Control Group - Category
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_010 : MainUserControl
  {
    #region Field
    private bool loadCheck = true;
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewPLN_21_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewPLN_21_010_Load(object sender, EventArgs e)
    {

    }
    #endregion Init Data

    #region Process Data
    /// <summary>
    /// Find and show data result
    /// </summary>
    private void Search()
    {
      string commandText = string.Empty;
      commandText += "SELECT MaterialCode, MaterialNameEn, MaterialNameVn, ISNULL(IsControl, 0) IsControl,";
      commandText += " 		CASE WHEN ISNULL(IsControl, 0) != 0 THEN 'WO && Carcass' ELSE '' END ControlType ";
      commandText += " FROM VBOMMaterials";
      commandText += " WHERE Warehouse = 2 ";
      if (txtGroupCategory.Text.Length > 0)
      {
        commandText += "    AND (MaterialCode LIKE '%" + txtGroupCategory.Text + "%'";
        commandText += "      OR MaterialNameEn LIKE '%" + txtGroupCategory.Text + "%'";
        commandText += "      OR MaterialNameVn LIKE '%" + txtGroupCategory.Text + "%')";
      }

      DataTable dataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dataSource;
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

      e.Layout.Bands[0].Columns["IsControl"].Header.Caption = "Control";
      e.Layout.Bands[0].Columns["IsControl"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["ControlType"].Header.Caption = "Control Type";
      e.Layout.Bands[0].Columns["ControlType"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ControlType"].MaxWidth = 100;

      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
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
