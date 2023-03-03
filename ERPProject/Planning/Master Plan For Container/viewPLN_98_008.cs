/*
  Author      : 
  Date        : 07/11/2012
  Description : Change ShipDate For Container
*/
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
  public partial class viewPLN_98_008 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private DataTable dtContainer = new DataTable();
    #endregion Field

    #region Init
    public viewPLN_98_008()
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
      // Load Customer
      this.LoadContainerNo(udrpContainer);

      // Load Data
      this.LoadData();
    }

    private void LoadData()
    {
      this.ultData.DataSource = this.TableInit();
    }

    /// <summary>
    /// Get Main Structure Datatable Init
    /// </summary>
    /// <returns></returns>
    private DataTable TableInit()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("ContainerPid", typeof(System.Int64));
      dt.Columns.Add("ContainerNo", typeof(System.String));
      dt.Columns.Add("CurrentShipDate", typeof(System.String));
      dt.Columns.Add("NewShipDate", typeof(System.DateTime));
      dt.Columns.Add("CurrentOriginalShipDate", typeof(System.String));
      dt.Columns.Add("NewOriginalShipDate", typeof(System.DateTime));
      return dt;
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["ContainerPid"].Hidden = true;

      e.Layout.Bands[0].Columns["NewShipDate"].Header.Caption = "NewLoadingDate";
      e.Layout.Bands[0].Columns["CurrentShipDate"].Header.Caption = "CurrentLoadingDate";
      e.Layout.Bands[0].Columns["CurrentOriginalShipDate"].Header.Caption = "CurrentOriginalLoadingDate";
      e.Layout.Bands[0].Columns["NewOriginalShipDate"].Header.Caption = "NewOriginalLoadingDate";

      e.Layout.Bands[0].Columns["NewShipDate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["ContainerNo"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["NewOriginalShipDate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["ContainerNo"].ValueList = udrpContainer;

      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["CurrentShipDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CurrentOriginalShipDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NewShipDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["NewOriginalShipDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    #endregion Init

    #region Load Data
    /// <summary>
    /// Load Container No
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="ultRevision"></param>
    /// <returns></returns>
    private UltraDropDown LoadContainerNo(UltraDropDown ultContainer)
    {
      if (ultContainer == null)
      {
        ultContainer = new UltraDropDown();
        this.Controls.Add(ultContainer);
      }
      string commandText = string.Empty;
      commandText = @"SELECT CON.Pid ContainerPid, CON.ContainerNo, CONVERT(VARCHAR, CON.ShipDate, 103) ShipDateStr, CON.ShipDate, 
                             CONVERT(VARCHAR, CON.OriginalShipDate, 103) OriginalShipDate
                      FROM TblPLNSHPContainer CON 
                      WHERE CON.Confirm <> 3 
                      ORDER BY CON.ShipDate";

      //commandText += " SELECT CON.Pid ContainerPid, CON.ContainerNo, CONVERT(VARCHAR, CON.ShipDate, 103) ShipDateStr,";
      //commandText += "          CON.ShipDate";
      //commandText += " FROM TblPLNSHPContainer CON ";
      //commandText += " WHERE CON.Confirm <> 3 ";
      //commandText += " ORDER BY CON.ShipDate";

      dtContainer = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultContainer.DataSource = dtContainer;
      ultContainer.ValueMember = "ContainerPid";
      ultContainer.DisplayMember = "ContainerNo";
      ultContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultContainer.DisplayLayout.Bands[0].Columns["ContainerPid"].Hidden = true;
      ultContainer.DisplayLayout.Bands[0].Columns["ShipDate"].Hidden = true;
      ultContainer.DisplayLayout.Bands[0].Columns["ShipDateStr"].Header.Caption = "Ship Date";
      ultContainer.DisplayLayout.Bands[0].Columns["OriginalShipDate"].Header.Caption = "Original Ship Date";
      ultContainer.Visible = false;
      return ultContainer;
    }
    #endregion Load Data

    #region Validation
    #endregion Validation

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();

      switch (columnName)
      {
        case "containerno":
          int containerPid = DBConvert.ParseInt(e.Cell.Row.Cells["ContainerNo"].Value.ToString());
          DataRow[] foundRow = dtContainer.Select("ContainerPid =" + containerPid);
          try
          {
            e.Cell.Row.Cells["CurrentShipDate"].Value = foundRow[0]["ShipDateStr"].ToString();
          }
          catch
          {
            e.Cell.Row.Cells["CurrentShipDate"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["CurrentOriginalShipDate"].Value = foundRow[0]["OriginalShipDate"].ToString();
            e.Cell.Row.Cells["NewOriginalShipDate"].Value = foundRow[0]["OriginalShipDate"].ToString();
          }
          catch
          {
            e.Cell.Row.Cells["CurrentOriginalShipDate"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["ContainerPid"].Value = containerPid;
          }
          catch
          {
            e.Cell.Row.Cells["ContainerPid"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check holiday
      bool isHoliday = false;
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        string shipDate = DBConvert.ParseString(DBConvert.ParseDateTime(rowGrid.Cells["NewShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
        string originalshipdate = DBConvert.ParseString(DBConvert.ParseDateTime(rowGrid.Cells["NewOriginalShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
        if (shipDate.Length > 0 || originalshipdate.Length > 0)
        {
          string ccommand = "SELECT Holiday FROM VHRDWorkingDay WHERE Holiday = -1 AND (CONVERT(varchar, WDate, 103) = '" + shipDate + "' OR CONVERT(varchar, WDate, 103) = '" + originalshipdate + "') ";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(ccommand);
          if (dt.Rows.Count > 0)
          {
            isHoliday = true;
          }
        }
      }
      if (isHoliday)
      {
        if (WindowUtinity.ShowMessageConfirm("WRN0034", "Ship Date OR Original Ship Date") == DialogResult.No)
        {
          return;
        }
      }

      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        if (DBConvert.ParseLong(rowGrid.Cells["ContainerPid"].Value.ToString()) != long.MinValue)
        {
          long containerPid = DBConvert.ParseLong(rowGrid.Cells["ContainerPid"].Value.ToString());
          DateTime shipDate = DBConvert.ParseDateTime(rowGrid.Cells["NewShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
          string originalshipdate = DBConvert.ParseString(DBConvert.ParseDateTime(rowGrid.Cells["NewOriginalShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
          string currentoriginalshipdate = DBConvert.ParseString(DBConvert.ParseDateTime(rowGrid.Cells["CurrentOriginalShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@ContainerPid", DbType.Int64, containerPid);
          inputParam[1] = new DBParameter("@ShipDate", DbType.DateTime, shipDate);
          if (originalshipdate.Length > 0)
          {
            inputParam[2] = new DBParameter("@OriginalShipDate", DbType.DateTime, DBConvert.ParseDateTime(rowGrid.Cells["NewOriginalShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
          }
          else
          {
            if (currentoriginalshipdate.Length > 0)
            {
              inputParam[2] = new DBParameter("@OriginalShipDate", DbType.DateTime, DBConvert.ParseDateTime(rowGrid.Cells["CurrentOriginalShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
            }
          }
          DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerUpdateShipDate_Update", inputParam);
        }
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.CloseTab();
    }
    #endregion Event
  }
}
