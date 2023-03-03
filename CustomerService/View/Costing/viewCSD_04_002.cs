/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 04-12-2010
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
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_002 : MainUserControl
  {
    #region Field
    public long itemPricePid = long.MinValue;
    public string des = string.Empty;
    private bool loadData = false;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    /// <summary>
    /// Init Form
    /// </summary>
    public viewCSD_04_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// From Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_002_Load(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbPriceBaseOn, 2006);
      this.LoadData();
    }
    #endregion Init

    #region LoadData

    /// <summary>
    /// Load Item Price Information
    /// </summary>
    private void LoadData()
    {
      cmbPriceBaseOn.SelectedValue = this.GetPriceBaseOn(this.itemPricePid);
      txtDescription.Text = this.des;
      DBParameter[] input = new DBParameter[] { new DBParameter("@ItemPricePid", DbType.Int64, this.itemPricePid) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemPriceInfo_Select", input);
      ultDetail.DataSource = dtSource;
      this.LoadDropDownItemCode(this.GetPriceBaseOn(this.itemPricePid));
      if (this.itemPricePid != long.MinValue)
      {
        string commandText = string.Format("SELECT Confirm FROM TblCSDItemPrice WHERE Pid = {0}", this.itemPricePid);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (obj != null)
        {
          int confirm = (int)obj;
          if (confirm == 1)
          {
            cmbPriceBaseOn.Enabled = false;
            btnSave.Enabled = false;
            chkConfirm.Checked = true;
            chkConfirm.Enabled = false;
            ultDetail.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            ultDetail.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            ultDetail.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
            txtDescription.Enabled = false;
          }
        }
      }
      this.loadData = true;
    }

    /// <summary>
    /// Get Price Base On By Item Price Pid
    /// </summary>
    /// <param name="itemPricePid"></param>
    /// <returns></returns>
    private int GetPriceBaseOn(long itemPricePid)
    {
      string commandText = string.Format("SELECT PriceBaseOn FROM TblCSDItemPrice WHERE Pid = {0}", itemPricePid);
      object objPrice = DataBaseAccess.ExecuteScalarCommandText(commandText);
      int result = int.MinValue;
      if (objPrice != null)
      {
        result = (int)objPrice;
      }
      return result;
    }

    /// <summary>
    /// Load UltraDropDown ItemCode
    /// </summary>
    /// <param name="priceBaseOn"></param>
    private void LoadDropDownItemCode(int priceBaseOn)
    {
      DBParameter[] input = new DBParameter[] { new DBParameter("@PriceBaseOn", DbType.Int32, priceBaseOn) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemInfoByPriceBaseOn_Select", input);
      udrItemCode.DataSource = dtSource;
      udrItemCode.DisplayMember = "ItemCode";
      udrItemCode.ValueMember = "ItemCode";
    }

    private void ReloadGrid( UltraGridRow row) {
      DataTable dtSource = (DataTable)udrItemCode.DataSource;
      string code = row.Cells["ItemCode"].Value.ToString().Trim();
      DataRow[] dtRows = dtSource.Select(string.Format("ItemCode = '{0}'", code));
      if (dtRows == null || dtRows.Length == 0) {
        return;
      }
      DataRow dtRow = dtRows[0];
      
      try
      {
        row.Cells["Cost"].Value = dtRow["Cost"];
      }
      catch
      {
        row.Cells["Cost"].Value = DBNull.Value;
      }
      try
      {
        row.Cells["LastestPrice"].Value = dtRow["LastestPrice"];
      }
      catch
      {
        row.Cells["LastestPrice"].Value = DBNull.Value;
      }
      try
      {
        row.Cells["LastestDate"].Value = dtRow["LastestDate"];
      }
      catch
      {
        row.Cells["LastestDate"].Value = DBNull.Value;
      }
      try
      {
        row.Cells["LastestDescription"].Value = dtRow["LastestDescription"];
      }
      catch
      {
        row.Cells["LastestDescription"].Value = DBNull.Value;
      }
    }
    #endregion LoadData

    #region SaveData

    /// <summary>
    /// Check Input Values
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      if (cmbPriceBaseOn.SelectedIndex <= 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Price Base On");
        return false;
      }

      if (txtDescription.Text.Trim().Length == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0013", "Description");
        return false;
      }

      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        long price = DBConvert.ParseLong(ultDetail.Rows[i].Cells["Price"].Value.ToString());
        if (price == long.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Price");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data Item Price
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    private bool SaveData(out long pid)
    {
      DBParameter[] input = new DBParameter[4];
      int priceBaseOn = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbPriceBaseOn));
      string des = txtDescription.Text.Trim();
      if (this.itemPricePid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.itemPricePid);
      }
      if (priceBaseOn != int.MinValue)
      {
        input[1] = new DBParameter("@PriceBaseOn", DbType.Int64, priceBaseOn);
      }

      input[2] = new DBParameter("@Description", DbType.AnsiString, 256, des);
      this.des = des;
      input[3] = new DBParameter("@AdjustBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

      DBParameter[] outputParam = new DBParameter[] {new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spCSDItemPrice_Edit", input, outputParam);
      pid = DBConvert.ParseLong(outputParam[0].Value);
      if (pid == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Data Detail Of Item Price
    /// </summary>
    /// <param name="pid"></param>
    private void SaveDataDetail(long pid)
    {
      foreach(long deletePid in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure("spCSDItemPriceDetail_Delete", inputDelete, outputDelete);
        if (outputDelete[0].ToString().Trim() == "0")
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
          return;
        }
      }

      DataTable dtSource = (DataTable)ultDetail.DataSource;
      DBParameter[] input = new DBParameter[5];
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          long detailPid = DBConvert.ParseLong(row["Pid"].ToString());

          if (detailPid != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
          }
          else
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBNull.Value);
          }
          input[1] = new DBParameter("@ItemPricePid", DbType.Int64, pid);
          string itemCode = row["ItemCode"].ToString().Trim();
          input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          long price = DBConvert.ParseLong(row["Price"].ToString());
          input[3] = new DBParameter("@Price", DbType.Double, price);
          input[4] = new DBParameter("@AdjustBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemPriceDetail_Edit", input, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value);
          if (result == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return;
          }
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.NeedToSave = false;
    }
    #endregion SaveData

    #region Event
    /// <summary>
    /// Button Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Override Function SaveAndClose
    /// </summary>
    public override void SaveAndClose()
    {
      this.btnSave_Click(null, null);
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = udrItemCode;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleCode"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Cost"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Cost"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Cost"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Cost"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["LastestPrice"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["LastestPrice"].Header.Caption = "Lastest Price";
      e.Layout.Bands[0].Columns["LastestPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LastestPrice"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LastestPrice"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["LastestDate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LastestDate"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["LastestDate"].Header.Caption = "Lastest Date";
      e.Layout.Bands[0].Columns["LastestDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["LastestDate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
      e.Layout.Bands[0].Columns["LastestDescription"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LastestDescription"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["LastestDescription"].Header.Caption = "Lastest Description";
    }

    /// <summary>
    /// ComboBox PriceBaseOn Select Index Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbPriceBaseOn_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbPriceBaseOn));
        this.LoadDropDownItemCode(value);
        string commandText = string.Format("SELECT COUNT(*) FROM TblCSDItemPrice WHERE Confirm = 0 AND PriceBaseOn = {0}", value);
        object objCount = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (objCount != null)
        {
          int count = (int)objCount;
          if (count != 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0017", "the PriceBaseOn");
            btnSave.Enabled = false;
            return;
          }
          else
          {
            btnSave.Enabled = true;
            this.NeedToSave = true;
          }
        }
        for (int i = 0; i < ultDetail.Rows.Count; i++) {
          this.ReloadGrid(ultDetail.Rows[i]);
        }
      }
    }

    /// <summary>
    /// Button Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = true;
      success = this.CheckValid();
      if (success)
      {
        long pid = long.MinValue;
        success = this.SaveData(out pid);
        if (success)
        {
          this.SaveDataDetail(pid);
          if (chkConfirm.Checked)
          {
            DBParameter[] inputConfirm = new DBParameter[2];
            inputConfirm[0] = new DBParameter("@Pid", DbType.Int64, pid);
            inputConfirm[1] = new DBParameter("@ConfirmBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            DBParameter[] outputConfirm = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spCSDItemPrice_Confirm", inputConfirm, outputConfirm);
            if (outputConfirm[0].Value.ToString().Trim() == "0")
            {
              WindowUtinity.ShowMessageError("ERR0005");
              return;
            }
          }
        }
        this.itemPricePid = pid;
        this.LoadData();
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// UltraGrid Before Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          listDeletingPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// UltraGrid After Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    /// <summary>
    /// UltraGrid After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      UltraGridRow row = e.Cell.Row;
      this.NeedToSave = true;
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "itemcode":
          try
          {
            row.Cells["Name"].Value = udrItemCode.SelectedRow.Cells["Name"].Value;
          }
          catch
          {
            row.Cells["Name"].Value = DBNull.Value;
          }
          try
          {
            row.Cells["SaleCode"].Value = udrItemCode.SelectedRow.Cells["SaleCode"].Value;
          }
          catch
          {
            row.Cells["SaleCode"].Value = DBNull.Value;
          }
          try
          {
            row.Cells["Cost"].Value = udrItemCode.SelectedRow.Cells["Cost"].Value;
          }
          catch
          {
            row.Cells["Cost"].Value = DBNull.Value;
          }
          try
          {
            row.Cells["LastestPrice"].Value = udrItemCode.SelectedRow.Cells["LastestPrice"].Value;
          }
          catch
          {
            row.Cells["LastestPrice"].Value = DBNull.Value;
          }
          try
          {
            row.Cells["LastestDate"].Value = udrItemCode.SelectedRow.Cells["LastestDate"].Value;
          }
          catch
          {
            row.Cells["LastestDate"].Value = DBNull.Value;
          }
          try
          {
            row.Cells["LastestDescription"].Value = udrItemCode.SelectedRow.Cells["LastestDescription"].Value;
          }
          catch
          {
            row.Cells["LastestDescription"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// UltraGrid Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (cmbPriceBaseOn.SelectedIndex > 0)
      {
        string columnName = e.Cell.Column.ToString().ToLower();
        switch (columnName)
        {
          case "itemcode":
            string value = e.NewValue.ToString();
            string commandText = string.Format("SELECT COUNT(*) FROM TblBOMItemBasic WHERE ItemCode = '{0}'", value);
            object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
            if (obj != null)
            {
              int count = (int)obj;
              if (count == 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0029", "ItemCode");
                e.Cancel = true;
              }
            }
            break;
          default:
            break;
        }
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0011", "The Price Base On");
        cmbPriceBaseOn.Enabled = true;
        cmbPriceBaseOn.Focus();
        e.Cancel = true;
      }
    }

    /// <summary>
    /// TextBox Description Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtDescription_TextChanged(object sender, EventArgs e)
    {
      if (this.loadData && !chkConfirm.Checked && txtDescription.Text.Trim().Length > 0)
      {
        this.NeedToSave = true;
      }
    }

    /// <summary>
    /// CheckBox Confirm Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkConfirm_CheckedChanged(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        this.NeedToSave = true;
      }
    }
    #endregion Event
  }
}
