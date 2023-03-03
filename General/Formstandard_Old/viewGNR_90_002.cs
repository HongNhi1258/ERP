/*
  Author      : 
  Date        : 17/8/2013
  Description : Template Search Info
  Standard Form: view_GNR_90_002
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.Shared.Utility;

namespace DaiCo.General.FormTemplate
{
  public partial class viewGNR_90_002 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      string storeName = string.Empty;

      //DBParameter[] param = new DBParameter[11];
      //param[5] = new DBParameter("@OldCode", DbType.String, text);

      //DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      //ultraGridInformation.DataSource = dtSource;

      //DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURPROnlineListPRInformation_Select", input);
      //if (ds != null)
      //{
      //  DataSet dsSource = this.CreateDataSet();
      //  dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
      //  dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
      //  ultData.DataSource = dsSource;

      //  for (int i = 0; i < ultData.Rows.Count; i++)
      //  {
      //    UltraGridRow row = ultData.Rows[i];
      //    if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 0)
      //    {
      //      row.Appearance.BackColor = Color.White;
      //    }
      //    else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 1)
      //    {
      //      row.Appearance.BackColor = Color.LightGreen;
      //    }
      //    else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 2)
      //    {
      //      row.Appearance.BackColor = Color.Pink;
      //    }
      //    else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 3)
      //    {
      //      row.Appearance.BackColor = Color.Yellow;
      //    }
      //    else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 4)
      //    {
      //      row.Appearance.BackColor = Color.LightSkyBlue;
      //    }
      //    else if (DBConvert.ParseInt(row.Cells["Status"].Value.ToString()) == 5)
      //    {
      //      row.Appearance.BackColor = Color.Gray;
      //    }
      //  }
      //}

      //lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      //txtCusNoTo.Text = string.Empty;
      //dt_EnquiryFrom.Value = DateTime.MinValue;
      //this.txtCusNoTo.Focus();
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      //DataTable taParent = new DataTable("dtParent");
      //taParent.Columns.Add("PID", typeof(System.Int64));
      //taParent.Columns.Add("PROnlineNo", typeof(System.String));
      //taParent.Columns.Add("Department", typeof(System.String));
      //taParent.Columns.Add("Requester", typeof(System.String));
      //taParent.Columns.Add("CreateDate", typeof(System.String));
      //taParent.Columns.Add("PurposeOfRequisition", typeof(System.String));
      //taParent.Columns.Add("Status", typeof(System.Int32));
      //taParent.Columns.Add("StatusName", typeof(System.String));
      //ds.Tables.Add(taParent);

      //DataTable taChild = new DataTable("dtChild");
      //taChild.Columns.Add("PROnlinePid", typeof(System.Int64));
      //taChild.Columns.Add("WO", typeof(System.Int32));
      //taChild.Columns.Add("CarcassCode", typeof(System.String));
      //taChild.Columns.Add("ItemCode", typeof(System.String));
      //taChild.Columns.Add("MaterialCode", typeof(System.String));
      //taChild.Columns.Add("MaterialName", typeof(System.String));
      //taChild.Columns.Add("Unit", typeof(System.String));
      //taChild.Columns.Add("Qty", typeof(System.Double));
      //taChild.Columns.Add("QtyCancel", typeof(System.Double));
      //taChild.Columns.Add("ReceiptedQty", typeof(System.Double));
      //taChild.Columns.Add("RequestDate", typeof(System.String));
      //taChild.Columns.Add("StatusDetail", typeof(System.String));
      //ds.Tables.Add(taChild);

      //ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["PROnlinePid"], false));
      return ds;
    }
    #endregion function

    #region event
    public viewGNR_90_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_90_002_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupBoxSearch.Controls)
      {
        ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      }

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
      this.CloseTab();
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

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {        
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion event
  }
}
