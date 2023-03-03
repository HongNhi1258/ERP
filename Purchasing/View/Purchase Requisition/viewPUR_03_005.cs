/*
 * Author       :  
 * CreateDate   : 18/04/2011
 * Description  : List Approved Note For 1 PR
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_03_005 : DaiCo.Shared.MainUserControl
  {
    #region Field
    // PR No
    public string prNo = string.Empty;
    public double totalPrice = double.MinValue;
    //public string remark = string.Empty;
    // Approved
    private long approvePid = long.MinValue;
    #endregion Field

    #region Init
    public viewPUR_03_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_03_005_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserName + " | " + SharedObject.UserInfo.LoginDate;
      this.txtPRNo.Text = this.prNo;
      //this.txtRemark.Text = this.remark;
      // Load Data
      this.LoadData();
    }

    /// <summary>
    ///  Load Data
    /// </summary>
    private void LoadData()
    {
      // Load Detail
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PRNo", DbType.AnsiString, this.prNo) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURListPRDetailApproved_Select", inputParam);
      if (dsSource != null)
      {
        // Load Info
        txtTotalPrice.Text = dsSource.Tables[0].Rows[0]["ScheduleTotalMoney"].ToString();
        txtTotalAddition.Text = dsSource.Tables[0].Rows[0]["TotalAddition"].ToString();
        // Load Detail
        ultData.DataSource = dsSource.Tables[1];
      }
      // Format Luoi
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
        {
          row.Cells["Finished"].Activation = Activation.ActivateOnly;
          row.Cells["Selected"].Activation = Activation.ActivateOnly;
        }
      }
    }
    #endregion Init

    #region Process
    /// <summary>
    /// Add New PR Approved
    /// </summary>
    /// <returns></returns>
    private bool SaveUpdate()
    {
      bool result = true;

      // TblPURPRApprove
      this.approvePid = this.SaveUpdateNewPrApprove();
      if (this.approvePid == long.MinValue)
      {
        return false;
      }

      // TblPURPRApprove
      result = this.SavePrApproveDetail();
      if (result == false)
      {
        return false;
      }

      // Update Status TblPURPRInformation
      string no = this.UpdateStatusPRInformation();
      if (no.Length == 0)
      {
        return false;
      }
      return result;
    }

    /// <summary>
    /// Save PR Approve Detail
    /// </summary>
    /// <returns></returns>
    private bool SavePrApproveDetail()
    {
      bool flag = true;

      DBParameter[] inputParam = new DBParameter[4];

      string storeName = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];

        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1
            || DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
        {
          if (DBConvert.ParseLong(row.Cells["ApproveDetailPid"].Value.ToString()) != long.MinValue)
          {
            storeName = "spPURPRAppDetailFinished_Update";
            inputParam[0] = new DBParameter("@AppDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["ApproveDetailPid"].Value.ToString()));
            if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
            {
              inputParam[1] = new DBParameter("@Finished", DbType.Int32, 1);
            }
            else
            {
              inputParam[1] = new DBParameter("@Finished", DbType.Int32, 0);
            }
            inputParam[2] = new DBParameter("@PrDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
            {
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 8);
            }
            else
            {
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 5);
            }

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (DBConvert.ParseLong(result) == 0)
            {
              return false;
            }
          }
          else
          {
            storeName = "spPURPRApproveDetail_Insert";
            inputParam[0] = new DBParameter("@PrDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            inputParam[1] = new DBParameter("@PrApprovePid", DbType.Int64, this.approvePid);
            if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
            {
              inputParam[2] = new DBParameter("@Finished", DbType.Int32, 1);
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 8);
            }
            else
            {
              inputParam[2] = new DBParameter("@Finished", DbType.Int32, 0);
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 5);
            }

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@ResultPid", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (DBConvert.ParseLong(result) == 0)
            {
              return false;
            }
          }
        }
      }
      return flag;
    }

    /// <summary>
    /// Update PR Approve Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveUpdatePrApproveDetail()
    {
      bool flag = true;

      DBParameter[] inputParam = new DBParameter[4];

      string storeName = string.Empty;
      storeName = "spPURPRAppDetailFinished_Update";

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];

        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1
            || DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
        {
          if (DBConvert.ParseLong(row.Cells["ApproveDetailPid"].Value.ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@AppDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["ApproveDetailPid"].Value.ToString()));
            if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
            {
              inputParam[1] = new DBParameter("@Finished", DbType.Int32, 1);
            }
            else
            {
              inputParam[1] = new DBParameter("@Finished", DbType.Int32, 0);
            }
            inputParam[2] = new DBParameter("@PrDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
            {
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 8);
            }
            else
            {
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 5);
            }

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (DBConvert.ParseLong(result) == 0)
            {
              return false;
            }
          }
        }
      }
      return flag;
    }

    /// <summary>
    /// Add New PR Approved
    /// </summary>
    /// <returns></returns>
    private bool SaveAddNew()
    {
      bool result = true;

      // TblPURPRApprove
      this.approvePid = this.SaveInsertNewPrApprove();
      if (this.approvePid == long.MinValue)
      {
        return false;
      }

      // TblPURPRApproveDetail
      result = this.SavePrApproveDetail();
      if (result == false)
      {
        return false;
      }

      // Update Status TblPURPRInformation
      string no = this.UpdateStatusPRInformation();
      if (no.Length == 0)
      {
        return false;
      }
      return result;
    }

    /// <summary>
    /// Update Status PR Information ==> Pending
    /// </summary>
    /// <returns></returns>
    private string UpdateStatusPRInformation()
    {
      string result = string.Empty;
      DBParameter[] inputParam = new DBParameter[4];

      string storeName = string.Empty;
      storeName = "spPURPRInformationStatus_Update";

      inputParam[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, this.prNo);
      inputParam[1] = new DBParameter("@StatusPRDetail", DbType.Int32, 8);
      inputParam[2] = new DBParameter("@StatusPR1", DbType.Int32, 7);
      inputParam[3] = new DBParameter("@StatusPR2", DbType.Int32, 6);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, String.Empty) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = outputParam[0].Value.ToString();

      return result;
    }

    /// <summary>
    /// Save Insert New Pr Approve Detail
    /// </summary>
    /// <returns></returns>
    private Boolean SaveInsertNewPrApproveDetail()
    {
      long result = long.MinValue;
      DBParameter[] inputParam = new DBParameter[4];
      string commandText = string.Empty;
      DataTable dt = new DataTable();

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];

        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1
            || DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
        {
          string storeName = string.Empty;
          storeName = "spPURPRApproveDetail_Insert";
          if (DBConvert.ParseLong(row.Cells["ApproveDetailPid"].Value.ToString()) == long.MinValue)
          {
            inputParam[0] = new DBParameter("@PrDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            inputParam[1] = new DBParameter("@PrApprovePid", DbType.Int64, this.approvePid);
            if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
            {
              inputParam[2] = new DBParameter("@Finished", DbType.Int32, 1);
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 8);
            }
            else
            {
              inputParam[2] = new DBParameter("@Finished", DbType.Int32, 0);
              inputParam[3] = new DBParameter("@Status", DbType.Int32, 5);
            }

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@ResultPid", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (DBConvert.ParseLong(result) == 0)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Insert New Pr Information
    /// </summary>
    /// <returns></returns>
    private long SaveInsertNewPrApprove()
    {
      DBParameter[] inputParam = new DBParameter[1];
      string commandText = string.Empty;
      DataTable dt = new DataTable();

      string storeName = string.Empty;
      storeName = "spPURPRApprove_Insert";

      inputParam[0] = new DBParameter("@ApprovedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      //inputParam[1] = new DBParameter("@Remark", DbType.AnsiString, 128, this.txtRemark.Text);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      approvePid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return approvePid;
    }

    /// <summary>
    /// Save Insert New Pr Information
    /// </summary>
    /// <returns></returns>
    private long SaveUpdateNewPrApprove()
    {
      DBParameter[] inputParam = new DBParameter[2];
      string commandText = string.Empty;
      DataTable dt = new DataTable();

      string storeName = string.Empty;
      storeName = "spPURPRApprove_Update";

      inputParam[0] = new DBParameter("@ApprovedPid", DbType.Int64, this.approvePid);
      inputParam[1] = new DBParameter("@ApprovedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      approvePid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return approvePid;
    }
    #endregion Process

    #region Event
    /// <summary>
    /// Init Layout Data On Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();

      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsFinished"].Hidden = true;
      e.Layout.Bands[0].Columns["PRDTStatus"].Hidden = true;

      //if (this.approvePid != long.MinValue)
      //{
      //  e.Layout.Bands[0].Columns["ApproveDetailPid"].Hidden = true;
      //}
      e.Layout.Bands[0].Columns["ApproveDetailPid"].Hidden = true;

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Finished"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].Header.Caption = "PM Approved";
      e.Layout.Bands[0].Columns["Finished"].Header.Caption = "BOD Approved";

      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["LastPurchasePrice"].Header.Caption = "Last Purchase Price";
      e.Layout.Bands[0].Columns["DeparmentName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["DeparmentName"].Header.Caption = "Department";
      e.Layout.Bands[0].Columns["Quantity"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Amount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Currency"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Urgent"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["VAT"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LastPurchasePrice"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Amount"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["VAT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LastPurchasePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["LastPurchasePrice"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Amount"].Format = "###,###.##";

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Approve
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool flagErr = true;
      string message = string.Empty;

      //// Check Budget
      //DBParameter[] input = new DBParameter[1];
      //input[0] = new DBParameter("@PRNo", DbType.String, txtPRNo.Text);
      //DBParameter[] output = new DBParameter[2];
      //output[0] = new DBParameter("@Result", DbType.Int64, 0);
      //output[1] = new DBParameter("@BudgetCode", DbType.String, 128, string.Empty);
      //DataBaseAccess.ExecuteStoreProcedure("spPURPRCheckingBudgetOverForPurchasing", input, output);
      //long result = DBConvert.ParseLong(output[0].Value.ToString());
      //if (result == 0)
      //{
      //  message = "Budget request Over";
      //  WindowUtinity.ShowMessageErrorFromText(message);
      //  return;
      //}
      //// End

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1
            || DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 1)
        {
          if (row.Cells["Currency"].Value.ToString().Length == 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Currency");
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }

          // Quantity
          if (row.Cells["Price"].Value.ToString().Trim().Length == 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Price");
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }

          double price = DBConvert.ParseDouble(row.Cells["Price"].Value.ToString().Trim());
          if (price == double.MinValue)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Price");
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }

          flagErr = false;
          break;
        }
      }

      if (flagErr == true)
      {
        return;
      }

      bool success;
      //insert
      if (this.approvePid == long.MinValue)
      {
        success = this.SaveAddNew();
      }
      else
      {
        success = this.SaveUpdate();
      }

      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }

      this.LoadData();
    }

    /// <summary>
    ///  Double Click ==> Open 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected != null && ultData.Selected.Rows.Count > 0)
      {
        if (DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) != 5 &&
            DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) < 8)
        {
          viewPUR_03_002 view = new viewPUR_03_002();
          view.prDetailPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Pid"].Value.ToString());
          Shared.Utility.WindowUtinity.ShowView(view, "Update PR Detail", false, DaiCo.Shared.Utility.ViewState.ModalWindow);

          this.LoadData();
        }
      }
    }

    /// <summary>
    /// Close Tab Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void chkSelect_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];
        if (row.Cells["Selected"].Activation == Activation.AllowEdit)
        {
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 0)
          {
            row.Cells["Selected"].Value = 1;
          }
          else
          {
            row.Cells["Selected"].Value = 0;
          }
        }
      }
    }

    private void chkFinish_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];
        if (row.Cells["Finished"].Activation == Activation.AllowEdit)
        {
          if (DBConvert.ParseInt(row.Cells["Finished"].Value.ToString()) == 0)
          {
            row.Cells["Finished"].Value = 1;
          }
          else
          {
            row.Cells["Finished"].Value = 0;
          }
        }
      }
    }

    /// <summary>
    /// Format Numeric
    /// </summary>
    /// <param name="number"></param>
    /// <param name="phanLe"></param>
    /// <returns></returns>
    private string NumericFormat(double number, int phanLe)
    {
      if (number == double.MinValue)
      {
        return string.Empty;
      }
      if (phanLe < 0)
      {
        return number.ToString();
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      double t = Math.Truncate(number);
      formatInfo.NumberDecimalDigits = phanLe;
      return number.ToString("N", formatInfo);
    }

    private void txtTotalPrice_TextChanged(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtTotalPrice.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtTotalPrice.Text = numberRead;
    }

    private void txtTotalAddition_TextChanged(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtTotalAddition.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtTotalAddition.Text = numberRead;
    }
    #endregion Event
  }
}

