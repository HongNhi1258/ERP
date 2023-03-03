/*
 * Author       : 
 * CreateDate   : 21/05/2013
 * Description  : History Department Allocation
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_103 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public long departmentAllocatePid = long.MinValue;

    public viewPLN_21_103()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, inid data inform
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_21_103_Load(object sender, EventArgs e)
    {
      if (departmentAllocatePid != long.MinValue)
      {
        string month = string.Empty;
        string year = string.Empty;

        string commandText = string.Empty;
        commandText += " SELECT SU.Department, MAT.MaterialCode, MAT.MaterialNameEn, MAT.MaterialNameVn, SU.BOH, SU.Month, SU.Year";
        commandText += " FROM TblPLNVeneerAllocateDepartmentSummary SU ";
        commandText += " 	  LEFT JOIN VBOMMaterials MAT ON SU.MaterialCode = MAT.MaterialCode ";
        commandText += " WHERE SU.Pid = " + this.departmentAllocatePid;
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt.Rows.Count > 0)
        {
          this.txtDepartment.Text = dt.Rows[0]["Department"].ToString();
          this.txtCode.Text = dt.Rows[0]["MaterialCode"].ToString() + " - "
                        + dt.Rows[0]["MaterialNameEn"].ToString();
          this.txtBOH.Text = dt.Rows[0]["BOH"].ToString();
          month = dt.Rows[0]["Month"].ToString();
          year = dt.Rows[0]["Year"].ToString();
          this.txtTime.Text = month + '/' + year;

          commandText = string.Empty;

          commandText += " SELECT CONVERT(varchar, AD.CreateDate, 103) CreateDate, ";
          commandText += "		NV.HoNV + ' ' + NV.TenNV CreateBy, AD.Qty, AD.Remark";
          commandText += " FROM TblPLNVeneerAllocateDepartment AD";
          commandText += "    LEFT JOIN VHRNhanVien NV ON AD.CreateBy = NV.ID_NhanVien";
          commandText += " WHERE AD.Department = '" + this.txtDepartment.Text + "' ";
          commandText += "    AND AD.MaterialCode = '" + dt.Rows[0]["MaterialCode"].ToString() + "' ";
          commandText += "    AND MONTH(AD.CreateDate) =" + DBConvert.ParseInt(month);
          commandText += "    AND YEAR(AD.CreateDate) =" + DBConvert.ParseInt(year);

          dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          this.ultData.DataSource = dt;
        }
      }
    }

    /// <summary>
    /// Init Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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
    #endregion Init
  }
}

