/*
  Author      : 
  Date        : 19/8/2013
  Description : Search From Grid and Add to Detail
  Standard Form : viewGNR_90_004
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;

namespace DaiCo.General
{
  public partial class viewGNR_90_004 : MainUserControl
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

    //Data Set
    DataSet dsMain = new DataSet();
    #endregion Field

    #region Init

    public viewGNR_90_004()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load ViewWHD_05_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_90_004_Load(object sender, EventArgs e)
    {
      // Load Control UltraCombo
      this.LoadCombo();

      // Load Data
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadCombo()
    {
      string commandText = string.Empty;
      commandText += " ....";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultControl.DataSource = dtSource;
        ultControl.DisplayMember = "...";
        ultControl.ValueMember = "...";
        // Format Grid
        ultControl.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultControl.DisplayLayout.Bands[0].Columns["..."].Width = 250;
        ultControl.DisplayLayout.Bands[0].Columns["..."].Hidden = true;
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@RECNo", DbType.Int64, 16, this.pid) };

      DataSet dsMain = DataBaseAccess.SearchStoreProcedure("StoreName", inputParam);
      DataTable dtMasterInfor = dsMain.Tables[0];
      // Update
      if (dtMasterInfor.Rows.Count > 0)
      {
        DataRow row = dtMasterInfor.Rows[0];
        //txtReceivingNote.Text = this.receivingNo;
        //txtTitle.Text = dtReceivingInfo.Rows[0]["Title"].ToString();
        //txtCreateBy.Text = dtReceivingInfo.Rows[0]["CreateBy"].ToString();
        //txtDate.Text = dtReceivingInfo.Rows[0]["CreateDate"].ToString();
        //ultControl.Value = dtReceivingInfo.Rows[0]["Supplier"].ToString();
        //txtControl.Text = dtReceivingInfo.Rows[0]["SupplierNote"].ToString();
        //this.status = DBConvert.ParseInt(dtReceivingInfo.Rows[0]["Posting"].ToString());
      }
      // New
      else
      {
        //DataTable dtREC = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewReceivingNoForMaterial('03REC') NewRECCode");
        //if ((dtREC != null) && (dtREC.Rows.Count > 0))
        //{
        //  this.txtReceivingNote.Text = dtREC.Rows[0]["NewRECCode"].ToString();
        //  this.txtDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        //  this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
        //  this.status = 0;
        //}
      }

      // Load Detail

      DataSet dsSource = this.CreateDataSet();
      //  dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
      //  dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
      ultDetail.DataSource = dsSource;

      // Set Status control
      this.SetStatusControl();
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      //DataTable taParent = new DataTable("dtParent");
      //taParent.Columns.Add("PID", typeof(System.Int64));
      //taParent.Columns.Add("PROnlineNo", typeof(System.String));
      //taParent.Columns.Add("Department", typeof(System.String));
      //taParent.Columns.Add("Requester", typeof(System.String));
      //taParent.Columns.Add("CreateDate", typeof(System.String));
      //taParent.Columns.Add("PurposeOfRequisition", typeof(System.String));
      //taParent.Columns.Add("Status", typeof(System.Int32));
      //taParent.Columns.Add("StatusName", typeof(System.String));
      //ds.Tables.Add(taParent);

      //DataTable taChild = new DataTable("dtChild");
      //taChild.Columns.Add("PROnlinePid", typeof(System.Int64));
      //taChild.Columns.Add("WO", typeof(System.Int32));
      //taChild.Columns.Add("CarcassCode", typeof(System.String));
      //taChild.Columns.Add("ItemCode", typeof(System.String));
      //taChild.Columns.Add("MaterialCode", typeof(System.String));
      //taChild.Columns.Add("MaterialName", typeof(System.String));
      //taChild.Columns.Add("Unit", typeof(System.String));
      //taChild.Columns.Add("Qty", typeof(System.Double));
      //taChild.Columns.Add("QtyCancel", typeof(System.Double));
      //taChild.Columns.Add("ReceiptedQty", typeof(System.Double));
      //taChild.Columns.Add("RequestDate", typeof(System.String));
      //taChild.Columns.Add("StatusDetail", typeof(System.String));
      //ds.Tables.Add(taChild);

      //ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["PROnlinePid"], false));
      return ds;
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
      // Insert
      if (this.pid == long.MinValue)
      {
        DBParameter[] inputParam = new DBParameter[8];

        //inputParam[0] = new DBParameter("@RECNo", DbType.AnsiString, 16, "03REC");
        //inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        //if (txtTitle.Text.Length > 0)
        //{
        //  inputParam[2] = new DBParameter("@Title", DbType.String, 4000, txtTitle.Text);
        //}
        //if (chkConfirm.Checked)
        //{
        //  inputParam[3] = new DBParameter("@Posting", DbType.Int32, 1);
        //}
        //else
        //{
        //  inputParam[3] = new DBParameter("@Posting", DbType.Int32, 0);
        //}
        //inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, 1);
        //inputParam[5] = new DBParameter("@Type", DbType.Int32, 1);
        //inputParam[6] = new DBParameter("@Supplier", DbType.String, 50, ultControl.Value.ToString());
        //inputParam[7] = new DBParameter("@SupplierNote", DbType.String, 255, txtControl.Text);

        DBParameter[] outputParam = new DBParameter[1];
        //outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("StoreName", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
        return true;
      }
      // Update
      else
      {
        DBParameter[] inputParam = new DBParameter[6];
        //inputParam[0] = new DBParameter("@RECNo", DbType.AnsiString, 50, this.receivingNo);
        //inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        //if (txtTitle.Text.Length > 0)
        //{
        //  inputParam[2] = new DBParameter("@Title", DbType.String, 4000, txtTitle.Text);
        //}
        //if (chkConfirm.Checked)
        //{
        //  inputParam[3] = new DBParameter("@Posting", DbType.Int32, 1);
        //}
        //else
        //{
        //  inputParam[3] = new DBParameter("@Posting", DbType.Int32, 0);
        //}
        //inputParam[4] = new DBParameter("@Supplier", DbType.String, 50, ultControl.Value.ToString());
        //inputParam[5] = new DBParameter("@SupplierNote", DbType.String, 255, txtControl.Text);
        DBParameter[] outputParam = new DBParameter[1];
        //outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("StoreName", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
        return true;
      }
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      if(ultSearchInformation.Rows.Count > 0)
      {
        for (int i = 0; i < ultSearchInformation.Rows.Count; i++)
        {
          UltraGridRow rowInfo = ultSearchInformation.Rows[i];
          if (DBConvert.ParseInt(rowInfo.Cells["Selected"].Value.ToString()) == 1)
          {
            if (DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()) != 0)
            {
              DBParameter[] inputParam = new DBParameter[6];

              //inputParam[0] = new DBParameter("@ReceivingNo", DbType.AnsiString, 50, this.receivingNo);
              //inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 255, rowInfo.Cells["MaterialCode"].Value.ToString());
              //inputParam[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowInfo.Cells["Qty"].Value.ToString()));
              //inputParam[3] = new DBParameter("@WarehousePid", DbType.Int32, 1);
              //inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              //inputParam[5] = new DBParameter("@PONo", DbType.AnsiString, 16, rowInfo.Cells["PONo"].Value.ToString());

              DBParameter[] outputParam = new DBParameter[1];
              //outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

              DataBaseAccess.ExecuteStoreProcedure("StoreName", inputParam, outputParam);
              long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
              if (result == 0)
              {
                return false;
              }
            }
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
      if (ultControl.Value == null)
      {
        message = "Control...";
        return false;
      }
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

      for (int i = 0; i < ultSearchInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultSearchInformation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) != double.MinValue)
          {
            DBParameter[] inputParam = new DBParameter[2];

            //inputParam[0] = new DBParameter("@PONo", DbType.String, row.Cells["PONo"].Value.ToString());
            //inputParam[1] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
            DataTable dtCheck = DataBaseAccess.SearchStoreProcedureDataTable("StoreName", inputParam);
            if (dtCheck != null && dtCheck.Rows.Count > 0)
            {
              if (DBConvert.ParseDouble(dtCheck.Rows[0]["Qty"].ToString())
                  < DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()))
              {
                message = "Qty <=  " + DBConvert.ParseDouble(dtCheck.Rows[0]["Qty"].ToString());
                return false;
              }
            }
            else
            {
              message = "Data is invalid";
              return false;
            }
          }
          else
          {
            message = "Qty";
            return false;
          }
        }
      }
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
      if(!success)
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
      if(!success)
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
          detailPid = DBConvert.ParseLong(row.Cells["..."].Value.ToString());
        }
        catch { }

        try
        {
          detailCode = row.Cells["..."].Value.ToString();
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

    /// <summary>
    /// Print
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      //DBParameter[] input = new DBParameter[] { new DBParameter("...", DbType.AnsiString, 48, txtReceivingNote.Text.Trim()) };
      //DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingNotePrint_Materials", input);
      //dsWHDRPTMaterialsReceivingNote dsReceiving = new dsWHDRPTMaterialsReceivingNote();
      //dsReceiving.Tables["dtReceivingInfo"].Merge(dsSource.Tables[0]);
      //dsReceiving.Tables["dtReceivingDetail"].Merge(dsSource.Tables[1]);
      //double totalQty = 0;
      //for (int i = 0; i < dsReceiving.Tables["dtReceivingDetail"].Rows.Count; i++)
      //{
      //  if (DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString()) != double.MinValue)
      //  {
      //    totalQty = totalQty + DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[i]["Qty"].ToString());
      //  }
      //}

      //ReportClass cpt = null;
      //DaiCo.Shared.View_Report report = null;

      //cpt = new cptMaterialsReceivingNote();
      //cpt.SetDataSource(dsReceiving);
      //cpt.SetParameterValue("TotalQty", totalQty);
      //cpt.SetParameterValue("Title", "MATERIALS RECEIVING NOTE");
      //cpt.SetParameterValue("Receivedby", "Received by: ");
      //report = new DaiCo.Shared.View_Report(cpt);
      //report.IsShowGroupTree = false;
      //report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }
    #endregion Event 
  }
}
