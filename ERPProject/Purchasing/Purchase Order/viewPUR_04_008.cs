/*
  Author      : Huynh Thi Bang
  Date        : 12/04/2016
  Description : 
  Standard Form: viewPUR_04_008.cs
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

namespace DaiCo.ERPProject
{
  public partial class viewPUR_04_008 : MainUserControl
  {
    #region field
    //private IList listDeletedPid = new ArrayList();
    private bool checkGrid = false;
    #endregion field

    #region Init
    public viewPUR_04_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_04_008_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);
      this.LoadSupplier();
    }

    #endregion Init

    #region function

    /// <summary>
    /// Load Supplier 
    /// </summary>
    private void LoadSupplier()
    {
      string cmText = @"SELECT Pid Value, EnglishName + ' - ' + SupplierCode  Display
                        FROM TblPURSupplierInfo";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultraCBSupplier.DataSource = dt;
      ultraCBSupplier.DisplayMember = "Display";
      ultraCBSupplier.ValueMember = "Value";
      ultraCBSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBSupplier.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      string storeName = "spPURPODateToFactory_Select";
      string po = txtPO.Text.Trim().ToString();
      string materialCode = txtMaterialCode.Text.Trim().ToString();
      DBParameter[] inputParam = new DBParameter[3];
      if (po.Length > 0)
      {
        inputParam[0] = new DBParameter("@PONo", DbType.String, po);
      }
      if (materialCode.Length > 0)
      {
        inputParam[1] = new DBParameter("@Material", DbType.String, materialCode);
      }
      if (ultraCBSupplier.SelectedRow != null)
      {
        inputParam[2] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(ultraCBSupplier.Value.ToString()));
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsActive"].Value.ToString()) == 0)
        {
          ultraGridInformation.Rows[i].Activation = Activation.ActivateOnly;
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Select"].Value.ToString()) == 1)
          {
            ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.Moccasin;
          }
        }
      }
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtPO.Text = string.Empty;
      txtMaterialCode.Text = string.Empty;
      ultraCBSupplier.Value = DBNull.Value;
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
    /// Save data
    /// </summary>
    private void SaveData()
    {
      bool success = true;
      DataTable dt = (DataTable)ultraGridInformation.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dr = dt.Rows[i];
        if (dr["IsModified"].ToString() == "1")
        {
          if (DBConvert.ParseInt(dr["Select"].ToString()) == 1)
          {
            long pid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["Pid"].Value);
            DBParameter[] inputParam = new DBParameter[2];
            if (pid > 0)
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@CreateByToFactory", DbType.Int32, SharedObject.UserInfo.UserPid);
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spPURPODateToFactory_Edit", inputParam, outputParam);
            if (outputParam == null || DBConvert.ParseInt(outputParam[0].Value.ToString()) <= 0)
            {
              this.SaveSuccess = false;
              success = false;
            }
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.SearchData();
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
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
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsModified"].Hidden = true;
      e.Layout.Bands[0].Columns["IsActive"].Hidden = true;

      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].CellActivation = Activation.AllowEdit;

    }
    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "Select":
          if (e.Cell.Row.Cells["Select"].Value.ToString() == "1")
          {
            e.Cell.Row.Cells["IsModified"].Value = 1;
          }
          else
          {
            e.Cell.Row.Cells["IsModified"].Value = 0;
          }
          break;
        default:
          break;
      }
      this.NeedToSave = true;
    }

    /// <summary>
    /// Check box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAuto_Click(object sender, EventArgs e)
    {
      int selected = (chkAuto.Checked ? 1 : 0);
      checkGrid = false;
      if (!checkGrid)
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsActive"].Value.ToString()) == 1)
          {
            ultraGridInformation.Rows[i].Cells["Select"].Value = selected;
          }
        }
    }

    /// <summary>
    /// Cell change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInformation_CellChange(object sender, CellEventArgs e)
    {
      DataTable dt = (DataTable)ultraGridInformation.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dr = dt.Rows[i];
        if (dr.RowState == DataRowState.Modified)
        {
          checkGrid = true;
          if (chkAuto.Checked == true)
          {
            chkAuto.Checked = false;
          }
        }
      }
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion event

  }
}
