
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_030 : MainUserControl
  {
    #region Field
    public long WoPid = long.MinValue;
    public string CarcassCode = "";
    private int checkDefault = 0;
    private int DefaultIndex = 0;
    private int Default = 0;

    #endregion Field

    #region Init
    public viewPLN_02_030()
    {
      InitializeComponent();
    }

    private void viewPLN_02_030_Load(object sender, EventArgs e)
    {
      // Load ItemCode
      this.LoadItemCode();
      this.Search();
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT CarcassCode, [Description]  ";
      commandText += " FROM TblBOMCarcass  ";
      commandText += " WHERE ContractOut = 1 ORDER BY CarcassCode";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtItem;
      ucItemCode.ColumnWidths = "200; 500";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "CarcassCode";
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;
      string storeName = "spPLNWOCarcassContractOutCheckingPrice_Select";

      DBParameter[] param = new DBParameter[3];
      param[0] = new DBParameter("@Wo", DbType.Int64, this.WoPid);
      param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.CarcassCode);
      if (txtItemCode.Text.Trim().Length > 0)
      {
        param[2] = new DBParameter("@CarcassRelative", DbType.String, 4000, txtItemCode.Text.Trim().Replace("; ", ","));
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, param);
      ultData.DataSource = dtSource;
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      int countCheck = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["SetDefault"].Value.ToString()) == 1)
        {
          countCheck = countCheck + 1;
        }
      }

      if (countCheck > 1)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Set Default");
        return false;
      }
      return true;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Init Layout 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = false;
      for (int i = 0; i < ultData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        if (ultData.DisplayLayout.Bands[0].Columns[i].Header.Caption != "SetDefault")
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        Type colType = ultData.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["IsMain"].Value.ToString() == "1")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else
        {
          ultData.Rows[i].Cells["SetDefault"].Activation = Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[0].Columns["BOM Gross Price"].Header.Caption = "DC Gross Price";
      e.Layout.Bands[0].Columns["SetDefault"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["SupplierPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsMain"].Hidden = true;
      e.Layout.Bands[0].Columns["WoPid"].Header.Caption = "Wo";
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Bands[0].Columns["DC Net Price"].Format = "###,###";
      e.Layout.Bands[0].Columns["Old Gross Price"].Format = "###,###";
      e.Layout.Bands[0].Columns["New Gross Price"].Format = "###,###";
      e.Layout.Bands[0].Columns["BOM Gross Price"].Format = "###,###";
      e.Layout.Bands[0].Columns["Old Net Price"].Format = "###,###";
      e.Layout.Bands[0].Columns["New Net Price"].Format = "###,###";

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

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool flagSetDefault = false;

      bool success = this.CheckValid();
      if (success)
      {
        DBParameter[] inputParam = new DBParameter[3];
        DBParameter[] ouputParam = new DBParameter[1];
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].Cells["IsMain"].Value.ToString() == "1" && ultData.Rows[i].Cells["SetDefault"].Value.ToString() == "1")
          {
            inputParam[0] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["SupplierPid"].Value.ToString()));
            inputParam[1] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["WoPid"].Value.ToString()));
            inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultData.Rows[i].Cells["CarcassCode"].Value.ToString());

            ouputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutDetailSetDefafault_Edit", inputParam, ouputParam);
            int result = DBConvert.ParseInt(ouputParam[0].Value.ToString());
            if (result <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0037", "Data");
              return;
            }
            flagSetDefault = true;
          }
        }
        if (flagSetDefault == false)
        {
          inputParam[0] = new DBParameter("@WoPid", DbType.Int64, this.WoPid);
          inputParam[1] = new DBParameter("@CarcassCode", DbType.String, this.CarcassCode);

          ouputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutDetailSetDefafault_Edit", inputParam, ouputParam);
          int result = DBConvert.ParseInt(ouputParam[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0037", "Data");
            return;
          }
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }

    private void ucItemCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtItemCode.Text = this.ucItemCode.SelectedValue;
    }

    private void chkItemCode_CheckedChanged(object sender, EventArgs e)
    {
      ucItemCode.Visible = chkItemCode.Checked;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      //this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      DataTable dt;
      switch (columnName)
      {
        case "setdefault":
          try
          {

            if (checkDefault == 1)
            {
              return;
            }
            string strCarcassCode = e.Cell.Row.Cells["CarcassCode"].Value.ToString();
            int isMain = DBConvert.ParseInt(e.Cell.Row.Cells["IsMain"].Value.ToString());
            Default = DBConvert.ParseInt(e.Cell.Row.Cells["SetDefault"].Value.ToString());
            DefaultIndex = index;

            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              if (ultData.Rows[i].Cells["CarcassCode"].Value.ToString() == strCarcassCode && isMain == 1)
              {
                checkDefault = 1;
                if (Default == 1 && DefaultIndex != i && DBConvert.ParseInt(ultData.Rows[i].Cells["SetDefault"].Value.ToString()) == 1)
                {
                  ultData.Rows[i].Cells["SetDefault"].Value = 0;
                }
              }
            }

            checkDefault = 0;
          }
          catch
          {
          }
          break;

        default:
          break;
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0)
      {
        long Wo = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["WoPid"].Value.ToString());
        string CarcassCode = ultData.Selected.Rows[0].Cells["CarcassCode"].Value.ToString();
        bool result = true;
        result = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = '" + Wo.ToString() + "' WHERE [Group] = 16023 AND Value = 1", null);
        result = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = '" + CarcassCode + "' WHERE [Group] = 16023 AND Value = 2", null);
        Process.Start(DataBaseAccess.ExecuteScalarCommandText("SELECT [Description] FROM TblBOMCodeMaster WHERE [Group] = 16023 AND Value = 3", null).ToString());
        Thread.Sleep(2000);
        result = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = NULL WHERE [Group] = 16023 AND Value = 1", null);
        result = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = NULL WHERE [Group] = 16023 AND Value = 2", null);
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtItemCode.Text = "";
      this.LoadItemCode();
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowImage.Checked)
      {
        grpBoxCarcassCode.Visible = true;
      }
      else
      {
        grpBoxCarcassCode.Visible = false;
      }
    }
    #endregion Event

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (chkShowImage.Checked == true)
      {
        try
        {
          grpBoxCarcassCode.Visible = true;
          string carcassCode = ultData.Selected.Rows[0].Cells["CarcassCode"].Value.ToString().Trim();
          if (carcassCode.Length > 0)
          {
            picCarcassCode.ImageLocation = FunctionUtility.BOMGetCarcassImage(carcassCode);
            grpBoxCarcassCode.Text = string.Format("{0}", carcassCode);
          }
        }
        catch { }
      }
    }


  }
}
