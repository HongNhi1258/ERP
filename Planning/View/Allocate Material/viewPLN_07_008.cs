/*
  Author      : Vo Van Duy Qui
  Date        : 25/02/2011
  Description : Re-Open Work Order Material Code
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_008 : MainUserControl
  {
    #region Field
    public long wo = long.MinValue;
    public string materialGroup = string.Empty;
    #endregion Field

    #region Init
    public viewPLN_07_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_008_Load(object sender, EventArgs e)
    {
      txtMaterialGroup.Text = this.materialGroup;
      this.LoadComboBoxWorkOrder();
      ultraComboWO.Value = this.wo;
      this.LoadMaterialCode(this.wo, this.materialGroup);
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load Data For UltraComboBox
    /// </summary>
    private void LoadComboBoxWorkOrder()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder ORDER BY Pid DESC";
      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraComboWO.DataSource = dtWO;
      ultraComboWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Material Code With WOPid AND MaterialGroup
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="groupMaterial"></param>
    private void LoadMaterialCode(long wo, string groupMaterial)
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@WO", DbType.Int64, wo);
      inputParam[1] = new DBParameter("@MaterialGroup", DbType.AnsiString, 3, groupMaterial);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNListReOpenMaterialCode", inputParam);
      ultData.DataSource = dtSource;
      chkSelectedAll.Checked = false;
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Status"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
      e.Layout.Bands[0].Columns["MaterialNameEn"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialNameEn"].MinWidth = 250;
      e.Layout.Bands[0].Columns["MaterialNameEn"].MaxWidth = 250;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["ReOpen"].Hidden = true;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// CheckBox Selected All Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      if (dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (chkSelectedAll.Checked)
          {
            row["Selected"] = 1;
          }
          else
          {
            row["Selected"] = 0;
          }
        }
      }
    }

    /// <summary>
    /// btnReOpenMaterialCode Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReOpenMaterialCode_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      DataRow[] rowCollection = dtSource.Select("Selected = 1 AND ReOpen = 0");
      if (rowCollection.Length > 0)
      {
        foreach (DataRow row in rowCollection)
        {
          string materialCode = row["MaterialCode"].ToString().Trim();
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@WO", DbType.Int64, this.wo);
          inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
          inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.SearchStoreProcedure("spPLNReOpenWoMaterial_Insert", inputParam, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0070", "Re-Open");
            return;
          }
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0028", "Re-Open");
      }
      this.LoadMaterialCode(this.wo, this.materialGroup);
    }

    /// <summary>
    /// btnClosedMaterialCode Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClosedMaterialCode_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      DataRow[] rowCollection = dtSource.Select("Selected = 1 AND ReOpen = 1");
      if (rowCollection.Length > 0)
      {
        foreach (DataRow row in rowCollection)
        {
          string materialCode = row["MaterialCode"].ToString().Trim();
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@WO", DbType.Int64, this.wo);
          inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.SearchStoreProcedure("spPLNReOpenWoMaterial_Delete", inputParam, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0070", "Closed");
            return;
          }
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0028", "Closed");
      }
      this.LoadMaterialCode(this.wo, this.materialGroup);
    }
    #endregion Event
  }
}
