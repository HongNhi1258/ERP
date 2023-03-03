/*
 * Author       :  
 * CreateDate   : 08/02/2011
 * Description  : Re-Allocate For Department
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_102 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public viewPLN_07_102()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_102_Load(object sender, EventArgs e)
    {
      this.LoadCombo();
    }
    #endregion Init

    #region Process
    /// <summary>
    /// Load Combo
    /// </summary>
    private void LoadCombo()
    {
      // Load Data For Department
      string commandText = "SELECT Department, Department + ' | ' +  DeparmentName Name FROM VHRDDepartment";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      multiCBDepartment.DataSource = dtSource;
      if (dtSource != null)
      {
        multiCBDepartment.ValueMember = "Department";
        multiCBDepartment.DisplayMember = "Name";
      }
      //ControlUtility.LoadMultiCombobox(multiCBDepartment, dtSource, "Department", "Name");
      //multiCBDepartment.ColumnWidths = "100, 500";

      // Material Code
      commandText = "SELECT MaterialCode, MaterialCode + ' | '+ MaterialNameEn  Name FROM VBOMMaterials WHERE IsControl = 1";
      dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      multiCBMaterials.DataSource = dtSource;
      if (dtSource != null)
      {
        multiCBMaterials.ValueMember = "MaterialCode";
        multiCBMaterials.DisplayMember = "Name";
      }
      //ControlUtility.LoadMultiCombobox(multiCBMaterials, dtSource, "MaterialCode", "Name");
      //multiCBDepartment.ColumnWidths = "100, 500";

      this.txtQty.Text = string.Empty;
      this.txtRemain.Text = string.Empty;
      this.txtRemark.Text = string.Empty;
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

      string storeName = "spPLNAllocateForDepartment_Insert";
      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@Department", DbType.AnsiString, 50, this.multiCBDepartment.Value.ToString());
      inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, this.multiCBMaterials.Value.ToString());
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
      if (this.multiCBDepartment.Value == null)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Department");
        return false;
      }

      // Materials
      if (this.multiCBMaterials.Value == null)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Materials");
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
        if (qty == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Qty");
          return false;
        }

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
      if (this.multiCBDepartment.Value == null || this.multiCBMaterials.Value == null)
      {
        this.txtRemain.Text = string.Empty;
        return;
      }

      string commandText = "Select dbo.FPLNGetRemainQtyOfDeptAllocate(@Department, @MaterialCode)";
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@Department", DbType.AnsiString, 50, multiCBDepartment.Value);
      inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 50, multiCBMaterials.Value);
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

    /// <summary>
    /// Department Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void multiCBDepartment_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SelectIndexChange();
    }

    /// <summary>
    /// Material Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void multiCBMaterials_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SelectIndexChange();
    }
    #endregion Process
  }
}

