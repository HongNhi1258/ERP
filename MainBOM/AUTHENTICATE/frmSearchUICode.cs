/*
  Author      : Huynh Thi Bang  
  Date        : 14/08/2015
  Description : Search info UICode
  Standard Form: frmSearchUICode.cs
*/
using DaiCo.Application;
using DaiCo.ERPProject;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.General
{
  public partial class frmSearchUICode : Form
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {

    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      DBParameter[] inputParam = new DBParameter[2];
      if (txtUICode.Text.Length > 0)
      {
        inputParam[0] = new DBParameter("@UICode", DbType.String, txtUICode.Text);
      }
      if (txtControName.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@ControlName", DbType.String, txtControName.Text);
      }
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spGNRSearchInfoUICode", inputParam);
      ultraGridInformation.DataSource = dt;
      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.Count.ToString());
      btnSearch.Enabled = true;

    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtControName.Text = string.Empty;
      txtUICode.Text = string.Empty;
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }
    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }
    #endregion function

    #region event
    public frmSearchUICode()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmSearchUICode_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        //if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        //{
        //  e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        //}
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          //e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      e.Layout.Bands[0].Columns["EmployeePid"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["EmployeePid"].MinWidth = 120;

      e.Layout.Bands[0].Columns["Department"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["Department"].MinWidth = 120;

    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }

    //public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    //{
    //  if (popupMenu.Items[0].Selected)
    //  {
    //    ControlUtility.GetDataForClipboard(ultraGridInformation);
    //  }
    //}

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      //if (e.Button == MouseButtons.Right)
      //{
      //  if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
      //  {
      //    popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
      //  }
      //}
    }
    #endregion event

  }
}
