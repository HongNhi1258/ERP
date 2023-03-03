/*
 * Author       : 
 * CreateDate   : 21/05/2013
 * Description  : Re-Allocate For Department
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_102 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public viewPLN_21_102()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_21_102_Load(object sender, EventArgs e)
    {
      // Department
      this.LoadDepartment();

      // Code
      this.LoadCode();

      this.txtQty.Text = string.Empty;
      this.txtRemark.Text = string.Empty;
      this.txtRemain.Text = string.Empty;
    }
    #endregion Init

    #region Process
    /// <summary>
    /// Load Department
    /// </summary>
    private void LoadDepartment()
    {
      string commandText = string.Empty;
      commandText += " SELECT Department, DeparmentName  ";
      commandText += " FROM VHRDDepartment ";
      commandText += " WHERE DelFlag = 0";

      DataTable dtDepartment = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDepartment.DataSource = dtDepartment;
      ultDepartment.DisplayMember = "Department";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["DeparmentName"].Width = 300;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Width = 100;
    }

    /// <summary>
    /// Load Code
    /// </summary>
    private void LoadCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT MaterialCode, MaterialCode + '-' + MaterialNameEn Name ";
      commandText += " FROM VBOMMaterials ";
      commandText += " WHERE IsControl = 1 AND Warehouse = 2";
      commandText += " ORDER BY MaterialCode ";

      DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCode.DataSource = dtCode;
      ultCode.DisplayMember = "Name";
      ultCode.ValueMember = "MaterialCode";
      ultCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCode.DisplayLayout.Bands[0].Columns["Name"].Width = 400;
    }

    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Save Allocate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckError(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string storeName = "spPLNVeneerAllocateDepartment_Insert";
      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@Department", DbType.AnsiString, 50, this.ultDepartment.Value.ToString());
      inputParam[1] = new DBParameter("@MaterialCode", DbType.String, this.ultCode.Value.ToString());
      inputParam[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(this.txtQty.Text) * (-1));
      inputParam[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);
      inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

      if (DBConvert.ParseLong(outputParam[0].Value.ToString()) == -1)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0065"));
        WindowUtinity.ShowMessageErrorFromText(message);

      }
      else if (DBConvert.ParseLong(outputParam[0].Value.ToString()) == -2)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0066"));
        WindowUtinity.ShowMessageErrorFromText(message);
      }
      else if (DBConvert.ParseLong(outputParam[0].Value.ToString()) == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0003"));
        WindowUtinity.ShowMessageErrorFromText(message);
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.CloseTab();
      }
    }

    /// <summary>
    /// Check Error
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckError(out string message)
    {
      message = string.Empty;
      // Department
      if (this.ultDepartment.Value == null
          || this.ultDepartment.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Department");
        return false;
      }

      // Code
      if (this.ultCode.Value == null
          || this.ultCode.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Code");
        return false;
      }

      // Qty
      if (this.txtQty.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Qty");
        return false;
      }
      else
      {
        double qty = DBConvert.ParseDouble(this.txtQty.Text);
        if (qty <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Qty");
          return false;
        }
      }

      if (DBConvert.ParseDouble(this.txtRemain.Text) != double.MinValue)
      {
        if (DBConvert.ParseDouble(this.txtQty.Text) > DBConvert.ParseDouble(this.txtRemain.Text))
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0010"), "Qty", "Remain");
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Select Index Change
    /// </summary>
    private void SelectIndexChange()
    {
      if (this.ultDepartment.Value == null || this.ultCode.Value == null)
      {
        this.txtRemain.Text = string.Empty;
        return;
      }

      string commandText = "SELECT dbo.FPLNVeneerGetRemainQtyOfDeptAllocate(@Department, @MaterialCode)";
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@Department", DbType.AnsiString, 50, this.ultDepartment.Value.ToString());
      inputParam[1] = new DBParameter("@MaterialCode", DbType.String, this.ultCode.Value.ToString());
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
      if (obj != null)
      {
        double remain = DBConvert.ParseDouble(obj.ToString());
        txtRemain.Text = remain.ToString();
      }
      else
      {
        txtRemain.Text = string.Empty;
      }
    }

    private void ultDepartment_ValueChanged(object sender, EventArgs e)
    {
      this.SelectIndexChange();
    }

    private void ultCode_ValueChanged(object sender, EventArgs e)
    {
      this.SelectIndexChange();
    }
    #endregion Process
  }
}

