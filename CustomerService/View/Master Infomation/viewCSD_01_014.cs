using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.IO;
using System.Collections;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_014 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeletedPid = new ArrayList();

    #endregion Field

    #region Init
    public viewCSD_01_014()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_01_014_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[4];
      if (ultCBFinishingCode.Value != null)
      {
        input[0] = new DBParameter("@FinishingCode", DbType.AnsiString, 16, ultCBFinishingCode.Value.ToString() );
      }

      if (txtEnglishName.Text.Length > 0)
      {
        input[1] = new DBParameter("@NameEN", DbType.AnsiString, 128, "%" + txtEnglishName.Text.Trim() + "%");
      }
      if (txtVNName.Text.Length > 0)
      {
        input[3] = new DBParameter("@NameVN", DbType.String, 128, "%" + txtVNName.Text.Trim() + "%");
      }

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDFinishingCode_Search", input);
      if (dt != null)
      {
        ultData.DataSource = dt;
      }
      // Enable button search
      btnSearch.Enabled = true;
      this.NeedToSave = false;
    }

    /// <summary>
    /// Load All Data For Search Information
    /// </summary>
    private void LoadData()
    {
      this.LoadFinishingCode();
    }

    private void LoadFinishingCode()
    {
      string commandText = "SELECT FinCode, FinCode + ' - ' + Name DisPlay FROM	TblBOMFinishingInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBFinishingCode.DataSource = dtSource;
      ultCBFinishingCode.DisplayMember = "DisPlay";
      ultCBFinishingCode.ValueMember = "FinCode";
      ultCBFinishingCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBFinishingCode.DisplayLayout.Bands[0].Columns["FinCode"].Hidden = true;
      ultCBFinishingCode.DisplayLayout.AutoFitColumns = true;
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
    #endregion Function

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["FinishingCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["EnglishName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["VietnameseName"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Description"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Description"].Header.Caption = "Marketing Name";
 
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++ )
      {
        if (ultData.Rows[i].Cells["Code"].Value.ToString().Trim() == string.Empty || ultData.Rows[i].Cells["Group"].Value.ToString() == string.Empty)
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          ultData.Rows[i].Selected = true;
          ultData.ActiveRowScrollRegion.ScrollRowIntoView(ultData.Rows[i]);

          errorMessage = "Data Input Error";
          return false;
        }
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      bool success = true;

      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ultData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[5];
          inputParam[0] = new DBParameter("@FinishingCode", DbType.AnsiString, 16, row["FinishingCode"].ToString());
          inputParam[1] = new DBParameter("@Description", DbType.String, 512, row["Description"].ToString());
          inputParam[2] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spCSDFinishingInfo_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.Search();
        this.NeedToSave = false;
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
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
    /// Save data before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtEnglishName.Text = string.Empty;
      txtVNName.Text = string.Empty;
      ultCBFinishingCode.Text = string.Empty;
    }
    #endregion Event

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "FinishingList");
    }
  }
}
