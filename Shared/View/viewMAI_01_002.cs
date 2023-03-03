/*
  Author      : 
  Date        : 19/8/2013
  Description : Search From Grid and Add to Detail
  Standard Form : viewMAI_01_002
*/
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.Shared
{
  public partial class viewMAI_01_002 : MainUserControl
  {
    #region Field
    //REC
    public long pid = long.MinValue;
    // Status
    int status = int.MinValue;
    // Delete
    private IList listDeletingDetailPid = new ArrayList();
    private IList listDeletedDetailPid = new ArrayList();
    private IList listDetailDeletingPid = new ArrayList();
    private IList listDetailDeletedPid = new ArrayList();
    private string sourseFile = string.Empty;
    private string destFile = string.Empty;

    //Data Set
    DataSet dsMain = new DataSet();
    #endregion Field

    #region Init

    public viewMAI_01_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load ViewWHD_05_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewMAI_01_002_Load(object sender, EventArgs e)
    {
      // Load Control UltraCombo
      //this.LoadCombo();

      // Load Data
      DaiCo.Shared.Utility.ControlUtility.LoadUltraComboDepartment(ultraCBDepartment);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraComboEmployee(ultraCBEmployee, string.Empty);
      this.LoadData();
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, 16, this.pid) };

      DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spGNRTaskTransfer_List", inputParam);
      DataTable dtMasterInfor = dsMain.Tables[0];
      // Update
      if (dtMasterInfor.Rows.Count > 0)
      {
        DataRow row = dtMasterInfor.Rows[0];
        txtTitle.Text = dtMasterInfor.Rows[0]["Title"].ToString();
        txtDescription.Text = dtMasterInfor.Rows[0]["Description"].ToString();
      }
      // Load Detail
      ultDetail.DataSource = dsMain.Tables[1];
      ultUploadFile.DataSource = dsMain.Tables[2];

      this.SetStatusControl();
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      // Set Status Control
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool success = this.SaveMasterInformation();
      if (!success)
      {
        return false;
      }

      success = this.SaveDetail();
      if (!success)
      {
        return false;
      }
      return success;
    }

    /// <summary>
    /// Save Receiving Info
    /// </summary>
    /// <returns></returns>
    private bool SaveMasterInformation()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
      inputParam[1] = new DBParameter("@Description", DbType.String, 1000, txtDescription.Text.Trim());
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spGNRTaskTransfer_Update", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result == 0)
      {
        return false;
      }
      else
      {
        this.pid = result;
      }
      return true;
    }

    private bool DeleteDetail()
    {
      try
      {
        string strCode = "";
        foreach (string detailCode in this.listDetailDeletedPid)
        {
          strCode += "," + detailCode;
        }
        strCode += ",";
        DBParameter[] inputParam = new DBParameter[1];

        inputParam[0] = new DBParameter("@StringPid", DbType.String, 4000, strCode);

        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spGNRTaskTransferDetail_Delete", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      if (ultSearchInformation.Rows.Count > 0)
      {
        for (int i = 0; i < ultSearchInformation.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultSearchInformation.Rows[i];
          if (DBConvert.ParseInt(rowInfo.Cells["Selected"].Value.ToString()) == 1)
          {
            //if (DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()) != 0)
            //{
            DBParameter[] inputParam = new DBParameter[5];
            if (DBConvert.ParseInt(rowInfo.Cells["EmployeePid"].Value.ToString()) > 0)
            {
              inputParam[1] = new DBParameter("@EID", DbType.Int32, DBConvert.ParseInt(rowInfo.Cells["EmployeePid"].Value.ToString()));
            }
            //if (DBConvert.ParseLong(rowInfo.Cells["GroupPid"].Value.ToString()) > 0)
            //{
            //  inputParam[2] = new DBParameter("@GroupPid", DbType.Int64, DBConvert.ParseLong(rowInfo.Cells["GroupPid"].Value.ToString()));
            //}
            inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[4] = new DBParameter("@TaskTransferPid", DbType.Int64, this.pid);

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spGNRTaskTransferDetail_Update", inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (result == 0)
            {
              return false;
            }
            //}
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check ValidInfo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidInfo(out string message)
    {
      message = string.Empty;
      return true;
    }

    /// <summary>
    /// Check Valid Info PO
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidBefore(out string message)
    {
      message = string.Empty;

      //for (int i = 0; i < ultSearchInformation.Rows.Count; i++)
      //{
      //  UltraGridRow row = ultSearchInformation.Rows[i];
      //  if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
      //  {
      //    if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) != double.MinValue)
      //    {
      //      DBParameter[] inputParam = new DBParameter[2];

      //      //inputParam[0] = new DBParameter("@PONo", DbType.String, row.Cells["PONo"].Value.ToString());
      //      //inputParam[1] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
      //      DataTable dtCheck = DataBaseAccess.SearchStoreProcedureDataTable("StoreName", inputParam);
      //      if (dtCheck != null && dtCheck.Rows.Count > 0)
      //      {
      //        if (DBConvert.ParseDouble(dtCheck.Rows[0]["Qty"].ToString())
      //            < DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()))
      //        {
      //          message = "Qty <=  " + DBConvert.ParseDouble(dtCheck.Rows[0]["Qty"].ToString());
      //          return false;
      //        }
      //      }
      //      else
      //      {
      //        message = "Data is invalid";
      //        return false;
      //      }
      //    }
      //    else
      //    {
      //      message = "Qty";
      //      return false;
      //    }
      //  }
      //}
      return true;
    }

    /// <summary>
    ///  Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidAfter(out string message)
    {
      message = string.Empty;
      //for (int i = 0; i < ultDetail.Rows.Count; i++)
      //{
      //  UltraGridRow row = ultDetail.Rows[i];
      //  if (DBConvert.ParseInt(row.Cells["Location"].Value.ToString()) == int.MinValue)
      //  {
      //    message = "Location";
      //    return false;
      //  }
      //}
      return true;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Init Search Information Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearchInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].Key != "Selected")
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
       Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Detail Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["EID"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupPid"].Hidden = true;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      /*
      // Allow update, delete, add new
     
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    ///  Add
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.CheckValidBefore(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        this.LoadData();
        return;
      }

      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      this.LoadData();
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValidInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Check Valid
      success = this.CheckValidAfter(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Info
      success = this.SaveMasterInformation();
      success = this.SaveFileUpload();
      success = this.DeleteDetail();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// After Cell Update Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearchInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "...":
          //if (DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()) == double.MinValue)
          //{
          //  WindowUtinity.ShowMessageError("ERR0001", "Qty");
          //  return;
          //}
          break;
        default:
          break;
      }
    }

    /// <summary>
    ///  Before Cell Update Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearchInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "...":
          if (text.Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(text) == double.MinValue)
            {
              WindowUtinity.ShowMessageError("ERR0001", "...");
              e.Cancel = true;
            }
            else if (DBConvert.ParseDouble(text) < 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "...");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Check Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelect_CheckedChanged(object sender, EventArgs e)
    {
      bool flagSelect = false;
      for (int i = 0; i < ultSearchInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultSearchInformation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 0)
        {
          flagSelect = true;
          break;
        }
      }
      for (int i = 0; i < ultSearchInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultSearchInformation.Rows[i];
        if (flagSelect == true)
        {
          row.Cells["Selected"].Value = 1;
        }
        else
        {
          row.Cells["Selected"].Value = 0;
        }
      }
    }

    /// <summary>
    /// Check Change hide 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkHide.Checked)
      {
        this.grpSearchInfomation.Visible = false;
      }
      else
      {
        this.grpSearchInfomation.Visible = true;
      }
    }

    /// <summary>
    /// Before Delete Row of Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingDetailPid = new ArrayList();
      this.listDetailDeletingPid = new ArrayList();

      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = long.MinValue;
        string detailCode = string.Empty;
        try
        {
          detailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        }
        catch { }

        try
        {
          detailCode = row.Cells["Pid"].Value.ToString();
        }
        catch { }

        if (detailPid != long.MinValue)
        {
          this.listDeletingDetailPid.Add(detailPid);
        }
        if (detailCode != string.Empty)
        {
          this.listDetailDeletingPid.Add(detailCode);
        }
      }
    }

    /// <summary>
    /// After Delete Row of Receiving
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetailInfomation_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long detailpid in this.listDeletingDetailPid)
      {
        this.listDeletedDetailPid.Add(detailpid);
      }
      foreach (string detailCode in this.listDetailDeletingPid)
      {
        this.listDetailDeletedPid.Add(detailCode);
      }
    }

    /// <summary>
    /// After Cell Update Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetailInfomation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "...":
          //if (DBConvert.ParseLong(e.Cell.Row.Cells["Location"].Value.ToString()) == long.MinValue)
          //{
          //  WindowUtinity.ShowMessageError("ERR0001", "Location");
          //  return;
          //}
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Change Info PO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearchInformation_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "qty":
          e.Cell.Row.Cells["Selected"].Value = 1;
          break;
        default:
          break;
      }
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
    /// <summary>
    /// Save RequestITUpload
    /// </summary>
    /// <returns></returns>
    private bool SaveFileUpload()
    {
      //Copy File 
      //System.IO.File.Copy(sourseFile, destFile, true);

      string storeName = string.Empty;
      DataTable dtMain = (DataTable)this.ultUploadFile.DataSource;
      foreach (DataRow row in dtMain.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row.RowState == DataRowState.Added)
          {
            //Copy File
            System.IO.File.Copy(row["LocationFileLocal"].ToString(), row["LocationFile"].ToString(), true);
          }
          storeName = "spGNRTaskTransferUpload_Edit";
          DBParameter[] inputParam = new DBParameter[7];

          //Pid
          if (DBConvert.ParseLong(row["Pid"].ToString()) >= 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          //RequestITPid
          if (this.pid != long.MinValue)
          {
            inputParam[1] = new DBParameter("@TaskTransferPid", DbType.Int64, this.pid);
          }

          inputParam[2] = new DBParameter("@FileName", DbType.String, 512, row["FileName"].ToString());

          inputParam[3] = new DBParameter("@LocationFile", DbType.String, 512, row["LocationFile"].ToString());

          inputParam[4] = new DBParameter("@Remark", DbType.String, 4000, row["Remark"].ToString());

          inputParam[5] = new DBParameter("@File", DbType.String, row["File"].ToString());

          inputParam[6] = new DBParameter("@TypeUpload", DbType.Int32, 0);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Key up
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearchInfomation_KeyUp(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
        {
          return;
        }
        int rowIndex = (e.KeyCode == Keys.Down) ? ultSearchInformation.ActiveCell.Row.Index + 1 : ultSearchInformation.ActiveCell.Row.Index - 1;
        int cellIndex = ultSearchInformation.ActiveCell.Column.Index;
        try
        {
          ultSearchInformation.Rows[rowIndex].Cells[cellIndex].Activate();
          ultSearchInformation.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }
        catch
        {
        }
      }
      catch
      {
      }
    }
    #endregion Event 

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }
    private void Search()
    {
      string deptCode = string.Empty;
      int userPid = int.MinValue;
      string userName = string.Empty;
      DBParameter[] inputParam = new DBParameter[3];
      if (ultraCBDepartment.SelectedRow != null)
      {
        deptCode = ultraCBDepartment.SelectedRow.Cells["Department"].Value.ToString();
        inputParam[0] = new DBParameter("@DeptCode", DbType.AnsiString, 50, deptCode);
      }
      if (ultraCBEmployee.SelectedRow != null)
      {
        userPid = DBConvert.ParseInt(ultraCBEmployee.SelectedRow.Cells["Pid"].Value.ToString());
        inputParam[1] = new DBParameter("@EmpPid", DbType.Int32, userPid);
      }
      if (txtUserName.Text.Trim().Length > 0)
      {
        userName = txtUserName.Text.Trim();
        inputParam[2] = new DBParameter("@UserName", DbType.AnsiString, 255, "%" + userName + "%");
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRListUser", inputParam);
      DataColumn c = new DataColumn("Selected", typeof(Int32));
      c.DefaultValue = 0;
      ds.Tables[0].Columns.Add(c);
      ultSearchInformation.DataSource = ds.Tables[0];
    }

    private void ultraCBDepartment_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Department"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Department"].MaxWidth = 70;
    }

    private void ultraCBEmployee_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Pid"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Pid"].MaxWidth = 70;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      ultraCBDepartment.Text = "";
      ultraCBEmployee.Text = "";
      txtUserName.Text = "";
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnUpload.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    private void ultUploadFile_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      DaiCo.Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultUploadFile);
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["TaskTransferPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFile"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFileLocal"].Hidden = true;
      e.Layout.Bands[0].Columns["File"].Hidden = true;

      e.Layout.Bands[0].Columns["FileName"].Header.Caption = "File Name";

      e.Layout.Bands[0].Columns["Type"].MaxWidth = 25;
      e.Layout.Bands[0].Columns["Type"].MinWidth = 25;
      e.Layout.Bands[0].Columns["FileName"].MaxWidth = 250;
      e.Layout.Bands[0].Columns["FileName"].MinWidth = 250;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnUpload_Click(object sender, EventArgs e)
    {
      if (this.txtLocation.Text.Trim().Length > 0)
      {
        string file = txtLocation.Text;
        FileInfo f = new FileInfo(file);
        long fLength = f.Length;
        if (fLength < 5120000)
        {
          string extension = System.IO.Path.GetExtension(file).ToLower();
          string typeFile = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE Value = '" + extension + "' AND [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPEFILEUPLOAD;
          DataTable dtTypeFile = DataBaseAccess.SearchCommandTextDataTable(typeFile);
          if (dtTypeFile != null && dtTypeFile.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtTypeFile.Rows[0][0].ToString()) > 0)
            {
              string fileName1 = System.IO.Path.GetFileName(file).ToString();
              string fileName = System.IO.Path.GetFileNameWithoutExtension(file).ToString()
                                      + DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd"))
                                      + DBConvert.ParseString(DateTime.Now.Ticks)
                                      + System.IO.Path.GetExtension(file);

              string sourcePath = System.IO.Path.GetDirectoryName(file);
              string commandText = string.Empty;
              commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 1);
              DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              string targetPath = string.Empty;
              if (dt != null && dt.Rows.Count > 0)
              {
                targetPath = dt.Rows[0][0].ToString();
              }

              sourseFile = System.IO.Path.Combine(sourcePath, fileName1);
              destFile = System.IO.Path.Combine(targetPath, fileName);
              if (!System.IO.Directory.Exists(targetPath))
              {
                System.IO.Directory.CreateDirectory(targetPath);
              }
              DataTable dtSource = (DataTable)ultUploadFile.DataSource;
              int i = dtSource.Rows.Count;
              foreach (DataRow row1 in dtSource.Rows)
              {
                if (row1.RowState == DataRowState.Deleted)
                {
                  i = i - 1;
                }
              }
              DataRow row = dtSource.NewRow();
              row["FileName"] = fileName1;
              row["LocationFile"] = destFile;
              row["LocationFileLocal"] = sourseFile;
              dtSource.Rows.Add(row);
              if (String.Compare(extension, ".docx") == 0 || String.Compare(extension, ".doc") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "word.bmp");
                row["File"] = targetPath + "word.bmp";
              }
              else if (string.Compare(extension, ".xls") == 0 || string.Compare(extension, ".xlsx") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "xls.bmp");
                row["File"] = targetPath + "xls.bmp";
              }
              else if (string.Compare(extension, ".pdf") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "pdf.bmp");
                row["File"] = targetPath + "pdf.bmp";
              }
              else if (string.Compare(extension, ".txt") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "notepad.bmp");
                row["File"] = targetPath + "notepad.bmp";
              }
              else if (string.Compare(extension, ".gif") == 0
                        || string.Compare(extension, ".jpg") == 0
                        || string.Compare(extension, ".bmp") == 0
                        || string.Compare(extension, ".png") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "image.bmp");
                row["File"] = targetPath + "image.bmp";
              }
              this.btnUpload.Enabled = false;
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0001", "Type File Not UPload");
            }
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", "File Upload < 5Mb");
        }
      }
    }
  }
}
