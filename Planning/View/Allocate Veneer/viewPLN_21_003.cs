/*
 * Author       :  
 * CreateDate   : 11/6/2013
 * Description  : Decrease Allocation WO & Item
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_003 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public viewPLN_21_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_101_Load(object sender, EventArgs e)
    {
      // Wo
      this.LoadWO();

      this.txtDecreaseQty.Text = string.Empty;
      this.txtRemark.Text = string.Empty;
      this.txtAllocatedQty.Text = string.Empty;
    }
    #endregion Init

    #region Process

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 AND Status != 1 ORDER BY Pid DESC";

      DataTable dtWo = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultWO.DataSource = dtWo;
      ultWO.DisplayMember = "Pid";
      ultWO.ValueMember = "Pid";
      ultWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultWO.DisplayLayout.Bands[0].Columns["Pid"].Width = 200;
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
      string altCode = this.ultAltCode.Value.ToString();
      string mainCode = this.ultMainCode.Value.ToString();

      string storeName = "spPLNVeneerAllocateForWODecrease_Insert";
      DBParameter[] inputParam = new DBParameter[7];
      inputParam[0] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(this.ultWO.Value.ToString()));
      inputParam[1] = new DBParameter("@CarcassCode", DbType.String, this.ultCarcassCode.Value.ToString());
      inputParam[2] = new DBParameter("@MainCode", DbType.String, mainCode);
      inputParam[3] = new DBParameter("@AltCode", DbType.String, altCode);
      inputParam[4] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(this.txtDecreaseQty.Text));
      inputParam[5] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);
      inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

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
      if (this.ultWO.Value == null
          || this.ultWO.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "WO");
        return false;
      }

      // Carcass Code
      if (this.ultCarcassCode.Value == null
          || this.ultCarcassCode.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "CarcassCode");
        return false;
      }

      // Main Code
      if (this.ultMainCode.Value == null
          || this.ultMainCode.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Main Code");
        return false;
      }

      // Allocated Qty
      if (this.txtAllocatedQty.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Allocated Qty");
        return false;
      }

      // Decrease Qty
      if (this.txtDecreaseQty.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Decrease Qty");
        return false;
      }
      else
      {
        double qty = DBConvert.ParseDouble(this.txtDecreaseQty.Text);
        if (qty <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Decrease Qty");
          return false;
        }

        if (qty > DBConvert.ParseDouble(this.txtAllocatedQty.Text))
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0010"), "Decrease Qty", "Allocated Qty");
          return false;
        }
      }

      return true;
    }

    private void ultWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultWO.Value != null && DBConvert.ParseInt(this.ultWO.Value.ToString()) != int.MinValue)
      {
        string commandText = string.Empty;
        commandText += " SELECT DISTINCT CarcassCode  ";
        commandText += " FROM TblPLNVeneerAllocateWorkOrderSummary  ";
        commandText += " WHERE IsCloseWO = 0 ";
        commandText += " 	AND Allocated - Issued > 0 ";
        commandText += " 	AND WO = " + DBConvert.ParseInt(this.ultWO.Value.ToString());
        commandText += " ORDER BY CarcassCode ";


        DataTable dtCarcassCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.ultCarcassCode.Text = string.Empty;
        ultCarcassCode.DataSource = dtCarcassCode;
        ultCarcassCode.DisplayMember = "CarcassCode";
        ultCarcassCode.ValueMember = "CarcassCode";
        ultCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCarcassCode.DisplayLayout.Bands[0].Columns["CarcassCode"].Width = 200;
      }
      else
      {
        this.ultCarcassCode.Text = string.Empty;
        ultCarcassCode.DataSource = null;
      }
      this.ultMainCode.Text = string.Empty;
      this.ultMainCode.DataSource = null;
      this.ultAltCode.Text = string.Empty;
      this.ultAltCode.DataSource = null;
      this.txtAllocatedQty.Text = string.Empty;
      this.txtDecreaseQty.Text = string.Empty;
      this.txtRemark.Text = string.Empty;
    }

    private void ultCarcassCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultWO.Value != null && DBConvert.ParseInt(this.ultWO.Value.ToString()) != int.MinValue
          && ultCarcassCode.Value != null && this.ultCarcassCode.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText += " SELECT DISTINCT ALLO.MaterialCode MainCode, MAT.NameEN MainName  ";
        commandText += " FROM TblPLNVeneerAllocateWorkOrderSummary ALLO ";
        commandText += " 	INNER JOIN TblGNRMaterialInformation MAT ON ALLO.MaterialCode = MAT.MaterialCode ";
        commandText += " WHERE IsCloseWO = 0 ";
        commandText += " 	AND Allocated - Issued > 0 ";
        commandText += " 	AND WO = " + DBConvert.ParseInt(this.ultWO.Value.ToString());
        commandText += " 	AND CarcassCode = '" + this.ultCarcassCode.Value.ToString() + "'";
        commandText += " ORDER BY MainCode ";

        DataTable dtMainCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.ultMainCode.Text = string.Empty;
        ultMainCode.DataSource = dtMainCode;
        ultMainCode.DisplayMember = "MainCode";
        ultMainCode.ValueMember = "MainCode";
        ultMainCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultMainCode.DisplayLayout.Bands[0].Columns["MainCode"].Width = 200;
        ultMainCode.DisplayLayout.Bands[0].Columns["MainName"].Width = 400;
      }
      else
      {
        this.ultMainCode.Text = string.Empty;
        ultMainCode.DataSource = null;
      }

      this.ultAltCode.Text = string.Empty;
      this.ultAltCode.DataSource = null;
      this.txtAllocatedQty.Text = string.Empty;
      this.txtDecreaseQty.Text = string.Empty;
      this.txtRemark.Text = string.Empty;
    }

    private void ultMainCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultWO.Value != null && DBConvert.ParseInt(this.ultWO.Value.ToString()) != int.MinValue
          && ultCarcassCode.Value != null && this.ultCarcassCode.Value.ToString().Length > 0
          && ultMainCode.Value != null)
      {
        string commandText = string.Empty;
        commandText += " SELECT DISTINCT ALLO.AlternativeCode AltCode, MAT.NameEN AltName  ";
        commandText += " FROM TblPLNVeneerAllocateWorkOrderSummary ALLO ";
        commandText += " 		INNER JOIN TblGNRMaterialInformation MAT ON ISNULL(ALLO.AlternativeCode, '') = ALLO.AlternativeCode ";
        commandText += " WHERE IsCloseWO = 0 ";
        commandText += " 	AND Allocated - Issued > 0 ";
        commandText += " 	AND WO = " + DBConvert.ParseInt(this.ultWO.Value.ToString());
        commandText += " 	AND CarcassCode = '" + this.ultCarcassCode.Value.ToString() + "'";
        commandText += " 	AND ALLO.MaterialCode = '" + this.ultMainCode.Value.ToString() + "'";
        commandText += " ORDER BY AltCode ";

        DataTable dtAltCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.ultAltCode.Text = string.Empty;
        ultAltCode.DataSource = dtAltCode;
        ultAltCode.DisplayMember = "AltCode";
        ultAltCode.ValueMember = "AltCode";
        ultAltCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultAltCode.DisplayLayout.Bands[0].Columns["AltCode"].Width = 200;
        ultAltCode.DisplayLayout.Bands[0].Columns["AltName"].Width = 400;

        if (dtAltCode.Rows.Count > 0)
        {
          this.ultAltCode.Value = dtAltCode.Rows[0][0].ToString();
        }
        else
        {
          this.ultAltCode.Value = "";
          this.ultAltCode_ValueChanged(null, null);
        }
      }
      else
      {
        this.ultAltCode.Text = string.Empty;
        this.txtAllocatedQty.Text = string.Empty;
        ultAltCode.DataSource = null;
      }

      this.txtDecreaseQty.Text = string.Empty;
      this.txtRemark.Text = string.Empty;
    }

    private void ultAltCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultWO.Value != null && DBConvert.ParseInt(this.ultWO.Value.ToString()) != int.MinValue
                && ultCarcassCode.Value != null && this.ultCarcassCode.Value.ToString().Length > 0
                && ultMainCode.Value != null && this.ultMainCode.Value.ToString().Length > 0
                && ultAltCode.Value != null)
      {
        string altCode = this.ultAltCode.Value.ToString();
        string mainCode = this.ultMainCode.Value.ToString();

        string commandText = string.Empty;
        commandText += " SELECT ALLO.Allocated - ISNULL(ALLO.Issued, 0) - ISNULL(MRN.Qty, 0) StockAllocateQty ";
        commandText += " FROM TblPLNVeneerAllocateWorkOrderSummary ALLO ";
        commandText += " 	LEFT JOIN VGNRVeneerRequisitionNoteInfo MRN ON (MRN.[Type] = 1) ";
        commandText += "                  AND MRN.MainCode = ALLO.MaterialCode ";
        commandText += "                  AND ISNULL(MRN.AlternativeCode, '') = ISNULL(ALLO.AlternativeCode, '') ";
        commandText += " WHERE ALLO.IsCloseWO = 0 ";
        commandText += " 	AND ALLO.Allocated - ALLO.Issued - ISNULL(MRN.Qty, 0) > 0 ";
        commandText += " 	AND ALLO.WO = " + DBConvert.ParseInt(this.ultWO.Value.ToString());
        commandText += " 	AND ALLO.CarcassCode = '" + this.ultCarcassCode.Value.ToString() + "'";
        commandText += " 	AND ALLO.MaterialCode = '" + mainCode + "'";
        commandText += " 	AND ISNULL(ALLO.AlternativeCode, '') = '" + altCode + "'";

        DataTable dtAllocate = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtAllocate != null && dtAllocate.Rows.Count > 0)
        {
          this.txtAllocatedQty.Text = dtAllocate.Rows[0][0].ToString();
        }
        else
        {
          this.txtAllocatedQty.Text = "0";
        }
      }
      else
      {
        this.txtAllocatedQty.Text = "0";
      }

      this.txtDecreaseQty.Text = string.Empty;
      this.txtRemark.Text = string.Empty;
    }
    #endregion Process
  }
}

