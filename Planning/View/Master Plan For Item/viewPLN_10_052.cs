/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewPLN_29_009.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_052 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    public long pidEnDetail = long.MinValue;
    private int qtySelect = 0;
    private int qtyRemain = int.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@EnDetailPid", DbType.Int64, this.pidEnDetail);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNAllocateFurnitureforEnquiry_LoadData", input);
      if (ds != null)
      {
        txtShowENNo.Text = ds.Tables[0].Rows[0]["EnquiryNo"].ToString();
        txtShowPONo.Text = ds.Tables[0].Rows[0]["PONo"].ToString();
        txtShowCUS.Text = ds.Tables[0].Rows[0]["CustomerName"].ToString();
        txtShowItemCode.Text = ds.Tables[0].Rows[0]["ItemCode"].ToString();
        txtShowRevision.Text = ds.Tables[0].Rows[0]["Revision"].ToString();
        txtShowRequiredDate.Text = ds.Tables[0].Rows[0]["RequestDate"].ToString();
        txtShowSpecial.Text = ds.Tables[0].Rows[0]["SpecialInstruction"].ToString();
        txtShowRemark.Text = ds.Tables[0].Rows[0]["Remark"].ToString();
        txtShowENQty.Text = ds.Tables[0].Rows[0]["Qty"].ToString();
        txtShowFurQty.Text = ds.Tables[0].Rows[0]["QtyFur"].ToString();
        this.qtyRemain = DBConvert.ParseInt(ds.Tables[0].Rows[0]["Qty"].ToString()) - DBConvert.ParseInt(ds.Tables[0].Rows[0]["QtyFur"].ToString());

        ControlUtility.LoadUltraCombo(ultCBInputCUS, ds.Tables[1], "CustomerPid", "CustomerName", "CustomerPid");
        ControlUtility.LoadUltraCombo(ultCBInputWorkArea, ds.Tables[2], "Pid", "StandByEN", "Pid");
      }
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 5;
      string storeName = "spPLNAllocateFurnitureforEnquiry_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtInputItemCode.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.String, txtInputItemCode.Text.Trim());
      }
      if (ultCBInputCUS.SelectedRow != null)
      {
        inputParam[1] = new DBParameter("@CustomerPid", DbType.Int64, DBConvert.ParseLong(ultCBInputCUS.Value.ToString()));
      }
      if (txtInputSO.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@SaleOrder", DbType.String, txtInputSO.Text.Trim());
      }
      if (txtInputCarcass.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@CarcassCode", DbType.String, txtInputCarcass.Text.Trim());
      }
      if (ultCBInputWorkArea.SelectedRow != null)
      {
        inputParam[4] = new DBParameter("@WorkArea", DbType.Int64, DBConvert.ParseLong(ultCBInputWorkArea.Value.ToString()));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, 600, inputParam);
      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
      this.qtySelect = 0;
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //if (ultCBWo.Text.Length == 0)
      //{
      //  errorMessage = "Work Order";      
      //  return false;
      //}
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 2. Insert/Update      
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Allocate"].Value.ToString()) == 1)
          {
            DBParameter[] inputParam = new DBParameter[5];
            long furniturePid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["FurniturePid"].Value.ToString());
            long sOPid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["SOPid"].Value.ToString());
            string remark = ultraGridInformation.Rows[i].Cells["Remark"].Value.ToString();
            inputParam[0] = new DBParameter("@ENDTPid", DbType.Int64, this.pidEnDetail);
            inputParam[1] = new DBParameter("@FURPid", DbType.Int64, furniturePid);
            inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[3] = new DBParameter("@Remark", DbType.String, remark);
            inputParam[4] = new DBParameter("@SOPid", DbType.Int64, sOPid);
            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPLNAllocateFurForEnquiry_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.CloseTab();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      //this.SaveData();
    }
    #endregion function

    #region event
    public viewPLN_10_052()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_052_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      //this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      // Hide column
      e.Layout.Bands[0].Columns["FurniturePid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["PAKDeadline"].Header.Caption = "PAK\nDeadline";
      e.Layout.Bands[0].Columns["SUBCONDeadline"].Header.Caption = "SUBCON\nDeadline";
      e.Layout.Bands[0].Columns["ASSYSANDDeadline"].Header.Caption = "ASSY + SAN\nDeadline";
      e.Layout.Bands[0].Columns["CustomerCode"].Header.Caption = "Customer\nCode";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Rev.";
      e.Layout.Bands[0].Columns["ASSHWStatus"].Header.Caption = "ASSY HW\nStatus";
      e.Layout.Bands[0].Columns["FFHWStatus"].Header.Caption = "FF HW\nStatus";
      e.Layout.Bands[0].Columns["MATStatus"].Header.Caption = "MAT\nStatus";
      e.Layout.Bands[0].Columns["ItemRemark"].Header.Caption = "Item\nRemark";

      // Set dropdownlist for column
      //e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;

      // Set Align
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      //e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;

      // Set Width

      e.Layout.Bands[0].Columns["FurnitureCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["FurnitureCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["WO"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["WO"].MinWidth = 60;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 50;
      e.Layout.Bands[0].Columns["WorkArea"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["WorkArea"].MinWidth = 60;
      e.Layout.Bands[0].Columns["PAKDeadline"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["PAKDeadline"].MinWidth = 80;
      e.Layout.Bands[0].Columns["SUBCONDeadline"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["SUBCONDeadline"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ASSYSANDDeadline"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ASSYSANDDeadline"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ASSHWStatus"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["ASSHWStatus"].MinWidth = 70;
      e.Layout.Bands[0].Columns["FFHWStatus"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["FFHWStatus"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MATStatus"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["MATStatus"].MinWidth = 70;
      e.Layout.Bands[0].Columns["CustomerCode"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["CustomerCode"].MinWidth = 70;
      e.Layout.Bands[0].Columns["SaleNo"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["SaleNo"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Allocate"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Allocate"].MinWidth = 50;
      e.Layout.Bands[0].Columns["ItemRemark"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemRemark"].MinWidth = 100;

      // Set Column Style
      e.Layout.Bands[0].Columns["Allocate"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      // Read only
      e.Layout.Bands[0].Columns["Allocate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "Allocate":
          {
            if (value == "1")
            {
              this.qtySelect = this.qtySelect + 1;
              if (this.qtySelect > this.qtyRemain)
              {
                WindowUtinity.ShowMessageErrorFromText("Invaild value!. Allocate Qty More Than Remain Qty");
                e.Cancel = true;
                this.qtySelect = this.qtySelect - 1;
              }
            }
            else
            {
              this.qtySelect = this.qtySelect - 1;
            }
          }
          break;
        default:
          break;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
        }
      }
    }
    #endregion event
  }
}
