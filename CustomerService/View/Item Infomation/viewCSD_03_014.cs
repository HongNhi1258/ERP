using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_014 : MainUserControl
  {
    #region Field
    public int iIndex = 0;
    private bool loaded = false;
    #endregion Field

    #region Init

    public viewCSD_03_014()
    {
      InitializeComponent();
    }

    private void viewCSD_03_014_Load(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.LoadUltraCBCategory(ultCategory);
      ultCategory.DisplayLayout.AutoFitColumns = true;

      Shared.Utility.ControlUtility.LoadUltraCBCollection(ultCollection);
      ultCollection.DisplayLayout.AutoFitColumns = true;

      Shared.Utility.ControlUtility.LoadUltraCBCustomer(ultCustomer);
      ultCustomer.DisplayLayout.AutoFitColumns = true;
      this.InitTabData();
    }

    #endregion Init

    #region Function

    private void LoadData(int index)
    {
      switch (index)
      {
        case 1:
          { 

          }

          break;
        case 2:
          { 
            
          }

          break;
        default:
          break;
      }
    }

    private void InitTabData()
    {
      this.loaded = false;
      switch (iIndex)
      {
        case 1:
          this.LoadData(1);
          break;
        case 2:
          this.LoadData(2);
          break;
        default:
          break;
      }
      this.loaded = true;
    }

    private bool CheckInvalid(int index)
    {
      return true;
    }

    /// <summary>
    /// save data
    /// </summary>
    /// <param name="index"></param>
    private void SaveData(int index)
    {
      btnSaveRelative.Enabled = false;
      btnSaveAlternative.Enabled = false;
      if (this.CheckInvalid(index))
      {
        if(tabControl.SelectedIndex == 0)
        {
          this.SaveRelative();
        }
        else if (tabControl.SelectedIndex == 1)
        {
          this.SaveAlternative();
        }
        this.LoadData(index);
      }
      this.btnSearch_Click(null, null);
      btnSaveRelative.Enabled = true;
      btnSaveAlternative.Enabled = true;
    }

    /// <summary>
    /// Delete Relative of distribute item
    /// </summary>
    private void SaveRelative()
    {
      DataTable dtDetail = (DataTable)ultGridRelative.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Modified && DBConvert.ParseInt(row["Select"].ToString()) == 1)
        {
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spCSDItemRelative_Delete", inputParam, outputParam);

          if (DBConvert.ParseLong((outputParam[0].Value.ToString())) <= 0)
          {
            WindowUtinity.ShowMessageError("WRN0004");
          }
        }
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.NeedToSave = false;

    }


    /// <summary>
    /// Delete alternative of distribute item
    /// </summary>
    private void SaveAlternative()
    {
      DataTable dtDetail = (DataTable)ultGridAlternative.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Modified && DBConvert.ParseInt(row["Select"].ToString()) == 1)
        {
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spCSDItemAlternative_Delete", inputParam, outputParam);

          if (DBConvert.ParseLong((outputParam[0].Value.ToString())) <= 0)
          {
            WindowUtinity.ShowMessageError("WRN0004");
          }
        }
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.NeedToSave = false;
    }

    #endregion Function

    #region Event

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (tabControl.SelectedIndex)
      {
        case 0:
          iIndex = 1;
          break;
        default:
          iIndex = tabControl.SelectedIndex;
          break;
      }
      this.InitTabData();
    }

    private void ultGridRelative_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultGridAlternative_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSaveRelative_Click(object sender, EventArgs e)
    {
      
      
      this.SaveData(1);
    }

    private void btnSaveAlternative_Click(object sender, EventArgs e)
    {
      this.SaveData(2);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[5];
      if (txtItemCode.Text.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + txtItemCode.Text.Trim() + "%");
      }

      if (txtSaleCode.Text.Length > 0)
      {
        input[1] = new DBParameter("@SaleCode", DbType.String, 128, "%" + txtSaleCode.Text.Trim() + "%");
      }
      if (ultCategory.Value != null)
      {
        input[2] = new DBParameter("@Category", DbType.Int64, DBConvert.ParseLong(ultCategory.Value.ToString()));
      }
      if (ultCollection.Value != null)
      {
        input[3] = new DBParameter("@Collection", DbType.Int32, DBConvert.ParseInt(ultCollection.Value.ToString()));
      }
      if (ultCustomer.Value != null)
      {
        input[4] = new DBParameter("@Customer", DbType.Int64, DBConvert.ParseLong(ultCustomer.Value.ToString()));
      }

      if (tabControl.SelectedIndex == 0)
      {
        DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemRelative_Search", input);
        if (dt != null)
        {
          ultGridRelative.DataSource = dt;
          lbCountRelative.Text = string.Format("Count: {0}", ultGridRelative.Rows.FilteredInRowCount);
        }
      }
      else if (tabControl.SelectedIndex == 1)
      {
        DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemAlternative_Search", input);
        if (dt != null)
        {
          ultGridAlternative.DataSource = dt;
          lbcountAlternative.Text = string.Format("Count: {0}", ultGridAlternative.Rows.FilteredInRowCount);
        }
      }

      
      // Enable button search
      btnSearch.Enabled = true;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.btnSearch_Click(null, null);
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    
    private void chkSelectAllRelative_CheckedChanged(object sender, EventArgs e)
    {
      if (chkSelectAllRelative.Checked)
      {
        for (int i = 0; i < ultGridRelative.Rows.Count; i++)
        {
          ultGridRelative.Rows[i].Cells["Select"].Value = 1;
        }
      }
      else 
      {
        for (int i = 0; i < ultGridRelative.Rows.Count; i++)
        {
          ultGridRelative.Rows[i].Cells["Select"].Value = 0;
        }
      }
    }

    private void chkSelectAllAlternative_CheckedChanged(object sender, EventArgs e)
    {
      if (chkSelectAllAlternative.Checked)
      {
        for (int i = 0; i < ultGridAlternative.Rows.Count; i++)
        {
          ultGridAlternative.Rows[i].Cells["Select"].Value = 1;
        }
      }
      else
      {
        for (int i = 0; i < ultGridAlternative.Rows.Count; i++)
        {
          ultGridAlternative.Rows[i].Cells["Select"].Value = 0;
        }
      }
    }
    #endregion Event

  }
}
