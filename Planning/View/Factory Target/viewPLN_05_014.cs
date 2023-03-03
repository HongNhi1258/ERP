/*
  Author        :   Ha Anh
  Date          :   07/01/2014
  Description   :   Input Target Distributor FEUS
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_05_014 : MainUserControl
  {
    #region field
    public int year;
    public int month;
    public viewPLN_05_012 frmMaster = null;

    #endregion field

    #region function
    public viewPLN_05_014()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_05_014_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void LoadData()
    {
      string commandText = string.Format(@" SELECT	TD.Pid, {0} [Month], {1} [Year], CM.Value Distributor, TD.FEUs
                                            FROM	TblBOMCodeMaster CM
	                                            LEFT JOIN	TblPLNDivisionTargetDistributor TD ON TD.MonthNo = {0} 
                                                        AND TD.YearNo = {1}
				                                                AND TD.Distributor = CM.Value
                                            WHERE	[Group] = 16033", month, year);
      DataTable dtGrid = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraGridInformation.DataSource = dtGrid;
    }

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary> 
    /// Save data before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;

        DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          //if (row.RowState == DataRowState.Modified)
          //{
          DBParameter[] inputParam = new DBParameter[5];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (pid > 0) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[1] = new DBParameter("@Month", DbType.Int32, DBConvert.ParseInt(row["Month"].ToString()));
          inputParam[2] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(row["Year"].ToString()));
          inputParam[3] = new DBParameter("@Distributor", DbType.AnsiString, 16, row["Distributor"].ToString());
          if (DBConvert.ParseDouble(row["FEUs"].ToString()) > 0)
          {
            inputParam[4] = new DBParameter("@FEUs", DbType.Double, DBConvert.ParseDouble(row["FEUs"].ToString()));
          }

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNTargetDistributor_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
          //}
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
          this.NeedToSave = false;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }
    #endregion function

    #region event
    private void btnClose_Click(object sender, EventArgs e)
    {
      //Double totalFEUs = 0;
      //for (int i = 0; i < ultraGridInformation.Rows.Count; i++ )
      //{
      //  if (DBConvert.ParseDouble(ultraGridInformation.Rows[i].Cells["FEUs"].Value.ToString()) > 0)
      //  {
      //    totalFEUs += DBConvert.ParseDouble(ultraGridInformation.Rows[i].Cells["FEUs"].Value.ToString());
      //  }
      //}
      //frmMaster.totalDistrubutor = totalFEUs;

      this.ConfirmToCloseTab();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;


      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowAddNew = AllowAddNew.No;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Read only
      e.Layout.Bands[0].Columns["Month"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Year"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Distributor"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName == "feus")
      {
        if (e.Cell.Row.Cells["FEUs"].Text.Length > 0 && DBConvert.ParseDouble(e.Cell.Row.Cells["FEUs"].Text) <= 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "FEUs");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }
    #endregion event
  }
}
