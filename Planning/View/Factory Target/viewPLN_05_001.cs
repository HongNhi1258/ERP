/*
 * Author :
 * CreateDate : 16/06/2010
 * Description :Insert,Update,Delete Factory Target And Customer Quota
 */
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_05_001 : MainUserControl
  {
    #region Field
    private int year = 0;
    private bool flagFactory = false;
    private bool flagQuota = false;
    #endregion Field

    #region Init Data
    /// <summary>
    /// UC_PLNFactoryTarget_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_05_001_Load(object sender, EventArgs e)
    {
      LoadComboYear();
      if (year == 0)
      {
        return;
      }
      LoadDataGrid();
    }

    /// <summary>
    /// Load Year
    /// </summary>
    private void LoadComboYear()
    {
      for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year + 2; i++)
      {
        this.cmbYear.Items.Add(i.ToString());
      }
    }


    public viewPLN_05_001()
    {
      InitializeComponent();
    }
    #endregion Init Data

    #region Process
    /// <summary>
    /// Save data
    /// </summary>
    private void SaveData()
    {
      if (year == 0)
      {
        return;
      }
      //Target
      if (this.flagFactory == true)
      {
        DataTable dsData = (DataTable)this.ultData.DataSource;
        foreach (DataRow row in dsData.Rows)
        {
          for (int i = 3; i < 15; i++)
          {
            if (row[i].ToString() != string.Empty)
            {
              if (!CheckExistTarget(DBConvert.ParseInt(row["Group"].ToString()), DBConvert.ParseInt(row["Code"].ToString()), i - 2, year))
              {
                PLNFactoryTarget_Insert obj = new PLNFactoryTarget_Insert();
                obj.Group = DBConvert.ParseInt(row["Group"].ToString());
                obj.Code = DBConvert.ParseInt(row["Code"].ToString());
                obj.Month = i - 2;
                obj.Year = year;
                obj.Target = DBConvert.ParseDouble(row[i].ToString());
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreObject(obj);
              }
              else
              {
                PLNFactoryTarget_Update obj = new PLNFactoryTarget_Update();
                obj.Group = DBConvert.ParseInt(row["Group"].ToString());
                obj.Code = DBConvert.ParseInt(row["Code"].ToString());
                obj.Month = i - 2;
                obj.Year = year;
                obj.Target = DBConvert.ParseDouble(row[i].ToString());
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreObject(obj);
              }
            }
            else
            {
              if (CheckExistTarget(DBConvert.ParseInt(row["Group"].ToString()), DBConvert.ParseInt(row["Code"].ToString()), i - 2, year))
              {
                PLNFactoryTarget_Delete obj = new PLNFactoryTarget_Delete();
                obj.Group = DBConvert.ParseInt(row["Group"].ToString());
                obj.Code = DBConvert.ParseInt(row["Code"].ToString());
                obj.Month = i - 2;
                obj.Year = year;
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreObject(obj);
              }
            }
          }
        }
      }
      //Quota
      if (this.flagQuota == true)
      {
        DataTable dsQuota = (DataTable)this.ultraQuota.DataSource;
        foreach (DataRow rowQuota in dsQuota.Rows)
        {
          for (int j = 2; j < 14; j++)
          {
            if (rowQuota[j].ToString() != string.Empty)
            {
              if (!CheckExistQuota(DBConvert.ParseInt(rowQuota["Pid"].ToString()), j - 1, year))
              {
                PLNCustomerQuota_Insert obj = new PLNCustomerQuota_Insert();
                obj.Pid = DBConvert.ParseInt(rowQuota["Pid"].ToString());
                obj.Month = j - 1;
                obj.Year = year;
                obj.Quota = DBConvert.ParseDouble(rowQuota[j].ToString());
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreObject(obj);
              }
              else
              {
                PLNCustomerQuota_Update obj = new PLNCustomerQuota_Update();
                obj.Pid = DBConvert.ParseInt(rowQuota["Pid"].ToString());
                obj.Month = j - 1;
                obj.Year = year;
                obj.Target = DBConvert.ParseDouble(rowQuota[j].ToString());
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreObject(obj);
              }
            }
            else
            {
              if (CheckExistQuota(DBConvert.ParseInt(rowQuota["Pid"].ToString()), j - 1, year))
              {
                PLNCustomerQuota_Delete obj = new PLNCustomerQuota_Delete();
                obj.Pid = DBConvert.ParseInt(rowQuota["Pid"].ToString());
                obj.Month = j - 1;
                obj.Year = year;
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreObject(obj);
              }
            }
          }
        }
      }
      this.flagQuota = false;
      this.flagFactory = false;
      this.NeedToSave = false;
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
    }

    /// <summary>
    /// CheckExistTarget
    /// </summary>
    /// <param name="group"></param>
    /// <param name="code"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    private Boolean CheckExistTarget(int group, int code, int month, int year)
    {
      string commandText = "SELECT Count(*) FROM TblPLNFactoryTarget WHERE [Group] = " + group
                          + " AND Code = " + code + " AND Month = " + month + " AND Year =" + year;
      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// CheckExistTarget
    /// </summary>
    /// <param name="group"></param>
    /// <param name="code"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    private Boolean CheckExistQuota(int group, int month, int year)
    {
      string commandText = "SELECT Count(*) FROM TblPLNCustomerQuota WHERE CustomerGroupPid = " + group
                          + " AND Month = " + month + " AND Year =" + year;
      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// LoadDataGrid
    /// </summary>
    private void LoadDataGrid()
    {
      // Factory Target
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Year", DbType.Int32, year);

      DataSet dsTarget = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNFactoryTarget_Select", inputParam);
      this.ultData.DataSource = dsTarget.Tables[0];
      //Quota Target

      this.ultraQuota.DataSource = dsTarget.Tables[1];
    }
    #endregion Process

    #region Event
    /// <summary>
    /// btnSave_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
      if (year == 0)
      {
        return;
      }
      LoadDataGrid();
    }

    /// <summary>
    /// ultData_AfterCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.flagFactory = true;
      this.NeedToSave = true;
    }

    /// <summary>
    /// btnClose_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// SaveAndClose
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    /// <summary>
    /// ultData_InitializeLayout
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
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Group"].Hidden = true;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
      e.Layout.Bands[0].Columns["Area"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Area"].Header.Caption = "AREA";

      e.Layout.Bands[0].Columns["JAN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FEB"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["APR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAY"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUL"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["AUG"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SEP"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OCT"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["NOV"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["DEC"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Format Quota Factory Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraQuota_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraQuota);

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "NAME";

      e.Layout.Bands[0].Columns["JAN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FEB"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["APR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MAY"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JUL"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["AUG"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SEP"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OCT"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["NOV"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["DEC"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// ultData_BeforeCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string text = e.Cell.Text;
      if (text.Trim().Length == 0)
      {
        return;
      }
      double value = DBConvert.ParseDouble(e.Cell.Text);

      if (value < 0)
      {
        MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0024"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
      }
    }

    /// <summary>
    /// ultraQuota_BeforeCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraQuota_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string text = e.Cell.Text;
      if (text.Trim().Length == 0)
      {
        return;
      }
      double value = DBConvert.ParseDouble(e.Cell.Text);

      if (value < 0)
      {
        MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0024"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        e.Cancel = true;
      }
    }

    /// <summary>
    /// ultraQuota_AfterCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraQuota_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.flagQuota = true;
      this.NeedToSave = true;
    }

    /// <summary>
    /// cmbYear_SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbYear_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.NeedToSave == true)
      {
        DialogResult dlgr = MessageBox.Show
                  (Shared.Utility.FunctionUtility.GetMessage("MSG0008"), "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (dlgr == DialogResult.Yes)
        {
          this.SaveData();
        }
      }
      this.year = DBConvert.ParseInt(this.cmbYear.Text);
      LoadDataGrid();
      this.NeedToSave = false;
      this.flagFactory = false;
      this.flagQuota = false;
    }

    /// <summary>
    /// show screen define group customer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGroupCustomer_Click(object sender, EventArgs e)
    {
      viewPLN_05_002 view = new viewPLN_05_002();
      Shared.Utility.WindowUtinity.ShowView(view, "CUSTOMER GROUP", false, DaiCo.Shared.Utility.ViewState.Window, FormWindowState.Normal);
    }
    #endregion Event
  }
}
