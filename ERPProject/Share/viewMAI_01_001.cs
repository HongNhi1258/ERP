using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Resources;

namespace DaiCo.ERPProject
{
  public partial class viewMAI_01_001 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    #endregion field
    public viewMAI_01_001()
    {
      InitializeComponent();
    }

    private void viewMAI_01_001_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    private void LoadData()
    {
      uepDailyAtt.Text = rm.GetString("DailyAttendance", ConstantClass.CULTURE);
      this.loadAttendance();
    }

    private void loadAttendance()
    {
      int paramNumber = 3;
      string storeName = "spHRDDBMonthlyAttendance_Select";
      string eIDs = SharedObject.UserInfo.UserPid.ToString();
      DateTime toDate = DateTime.Now;
      DateTime fromDate = toDate.AddMonths(-2);


      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@FromDate", DbType.Date, fromDate.Date);
      inputParam[1] = new DBParameter("@ToDate", DbType.Date, toDate.Date);
      inputParam[2] = new DBParameter("@EIDs", DbType.String, 2048, eIDs);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      if (dsSource != null)
      {
        ugdDailyAttendance.DataSource = dsSource.Tables[0];
      }
      else
      {
        ugdDailyAttendance.DataSource = null;
      }
    }

    private void ugdDailyAttendance_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64) || colType == typeof(System.Decimal))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      for (int i = 0; i < ugdDailyAttendance.Rows.Count; i++)
      {
        ugdDailyAttendance.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
        if (DBConvert.ParseInt(ugdDailyAttendance.Rows[i].Cells["LateMinutes"].Value) > 0)
        {
          ugdDailyAttendance.Rows[i].Cells["InTime"].Appearance.BackColor = Color.LightPink;
          ugdDailyAttendance.Rows[i].Cells["LateMinutes"].Appearance.BackColor = Color.LightPink;
        }
        if (DBConvert.ParseInt(ugdDailyAttendance.Rows[i].Cells["EarlyMinutes"].Value) > 0)
        {
          ugdDailyAttendance.Rows[i].Cells["OutTime"].Appearance.BackColor = Color.LightPink;
          ugdDailyAttendance.Rows[i].Cells["EarlyMinutes"].Appearance.BackColor = Color.LightPink;
        }
      }

      // Hide columns
      e.Layout.Bands[0].Columns["OriginalInTime"].Hidden = true;
      e.Layout.Bands[0].Columns["OriginalOutTime"].Hidden = true;
      e.Layout.Bands[0].Columns["DepartmentName"].Hidden = true;
      e.Layout.Bands[0].Columns["EID"].Hidden = true;
      e.Layout.Bands[0].Columns["ShiftName"].Hidden = true;

      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["WDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["WDate"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      e.Layout.Bands[0].Columns["InTime"].Format = ConstantClass.FORMAT_HOUR;
      e.Layout.Bands[0].Columns["InTime"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      e.Layout.Bands[0].Columns["InTime"].MaskInput = ConstantClass.MASKINPUT_TIME;
      e.Layout.Bands[0].Columns["OutTime"].Format = ConstantClass.FORMAT_HOUR;
      e.Layout.Bands[0].Columns["OutTime"].FormatInfo = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER, true);
      e.Layout.Bands[0].Columns["OutTime"].MaskInput = ConstantClass.MASKINPUT_TIME;

      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = rm.GetString("EmpName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DepartmentName"].Header.Caption = rm.GetString("DepartmentName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WDate"].Header.Caption = rm.GetString("WDate", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["DayName"].Header.Caption = rm.GetString("DayName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["InTime"].Header.Caption = rm.GetString("InTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OutTime"].Header.Caption = rm.GetString("OutTime", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ShiftName"].Header.Caption = rm.GetString("ShiftName", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["LateMinutes"].Header.Caption = rm.GetString("LateMinutes", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["EarlyMinutes"].Header.Caption = rm.GetString("EarlyMinutes", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WorkingHour"].Header.Caption = rm.GetString("WorkingHour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["WorkingDay"].Header.Caption = rm.GetString("WorkingDay", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT1Hour"].Header.Caption = rm.GetString("OT1Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT2Hour"].Header.Caption = rm.GetString("OT2Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["OT3Hour"].Header.Caption = rm.GetString("OT3Hour", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["Total"].Header.Caption = rm.GetString("Total", ConstantClass.CULTURE);
    }
  }
}

