/*
  Author      : 
  Date        : 07/11/2012
  Description : List Of Item have more than 1 customer
*/
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_98_005 : MainUserControl
  {
    #region Field
    public string itemCode = string.Empty;
    public int revision = int.MinValue;
    #endregion Field

    #region Init
    public viewPLN_98_005()
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
      string commandText = string.Empty;
      commandText += " SELECT  ROW_NUMBER() OVER ( ";
      commandText += " 				ORDER BY CON.ShipDate, CLD.ItemCode, CLD.Revision) [No], ";
      commandText += " 		CUS.CustomerCode + '-' + CUS.Name CustomerName,  ";
      commandText += " 		DIR.CustomerCode + '-' + DIR.Name DirectName, ";
      commandText += " 		CON.ContainerNo, CONVERT(VARCHAR, CON.ShipDate, 103) LoadingDate, ";
      commandText += " 		CLD.ItemCode, CLD.Revision, CLD.Qty ";
      commandText += " FROM TblPLNSHPContainer CON";
      commandText += " 	INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
      commandText += " 	INNER JOIN TblPLNContainerList CL ON COD.LoadingListPid = CL.Pid ";
      commandText += " 	INNER JOIN TblPLNContainerListDetail CLD ON CL.Pid = CLD.ContainerListPid ";
      commandText += " 	INNER JOIN TblPLNSaleOrder SO ON CLD.SOPid = SO.Pid";
      commandText += " 	INNER JOIN TblCSDCustomerInfo CUS ON SO.CustomerPid = CUS.Pid ";
      commandText += " 	LEFT JOIN TblCSDCustomerInfo DIR ON SO.DirectPid = DIR.Pid ";
      commandText += " WHERE CON.Confirm <> 3 ";
      commandText += " 	AND ItemCode = '" + itemCode + "'";
      commandText += " 		AND Revision = " + revision;
      commandText += " ORDER BY [No] ";
      DataTable dtData = DataBaseAccess.SearchCommandTextDataTable(commandText);
      this.ultData.DataSource = dtData;
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

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["No"].MinWidth = 20;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 20;

      e.Layout.Bands[0].Columns["Qty"].MinWidth = 30;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 30;

      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Init

    #region Load Data
    #endregion Load Data

    #region Validation
    #endregion Validation

    #region Event
    #endregion Event
  }
}
