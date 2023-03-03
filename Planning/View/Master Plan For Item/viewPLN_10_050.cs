/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewPLN_10_050.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_050 : MainUserControl
  {
    #region field
    public long enquiryPid = long.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.SearchData();
    }

    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("SaleCode", typeof(System.String));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("QtyFur", typeof(System.Int32));
      taParent.Columns.Add("RequiredShipDate", typeof(System.DateTime));
      taParent.Columns.Add("SpecialInstruction", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("LeadTimeCOM1", typeof(System.Double));
      taParent.Columns.Add("LeadTimeASSY", typeof(System.Double));
      taParent.Columns.Add("LeadTimeSUB", typeof(System.Double));
      taParent.Columns.Add("LeadTimeCAR", typeof(System.Double));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("FurnitureCode", typeof(System.String));
      taChild.Columns.Add("SaleNo", typeof(System.String));
      taChild.Columns.Add("CustomerCode", typeof(System.String));
      taChild.Columns.Add("WorkOrder", typeof(System.Int64));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("WorkAreaCode", typeof(System.String));
      taChild.Columns.Add("PAKDeadline", typeof(System.DateTime));
      taChild.Columns.Add("ASSYDeadline", typeof(System.DateTime));
      taChild.Columns.Add("SUBCONDeadline", typeof(System.DateTime));
      taChild.Columns.Add("COM1Deadline", typeof(System.DateTime));
      taChild.Columns.Add("ASSHWStatus", typeof(System.String));
      taChild.Columns.Add("FFHWStatus", typeof(System.String));
      taChild.Columns.Add("MATStatus", typeof(System.String));
      taChild.Columns.Add("Re_Allocate", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("Parent_Child", taParent.Columns["Pid"], taChild.Columns["Pid"]));
      return ds;
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnAutoAllocate.Enabled = false;
      int paramNumber = 1;
      string storeName = "spPLNAllocateFurforEnquiry_LoadData";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@EnquiryPid", DbType.Int64, this.enquiryPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, 600, inputParam);
      if (dsSource != null && dsSource.Tables[0].Rows.Count > 0)
      {
        txtEnquiry.Text = dsSource.Tables[0].Rows[0]["EnquiryNo"].ToString();
        txtPONo.Text = dsSource.Tables[0].Rows[0]["CustomerEnquiryNo"].ToString();
        txtCustom.Text = dsSource.Tables[0].Rows[0]["Name"].ToString();
        if (DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["AllocateStatus"].ToString()) > 0)
        {
          ultCBStatus.Value = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["AllocateStatus"].ToString());
        }
        else
        {
          ultCBStatus.Value = null;
        }
        DataSet ds = this.CreateDataSet();
        ds.Tables[0].Merge(dsSource.Tables[1]);
        if (dsSource.Tables[2].Rows.Count > 0)
        {
          ds.Tables[1].Merge(dsSource.Tables[2]);
        }
        ultraGridInformation.DataSource = ds;
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          ultraGridInformation.Rows[i].Cells["Qty"].Appearance.BackColor = Color.Empty;
          ultraGridInformation.Rows[i].Cells["QtyFur"].Appearance.BackColor = Color.Empty;
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Qty"].Value.ToString()) > DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["QtyFur"].Value.ToString()))
          {
            ultraGridInformation.Rows[i].Cells["Qty"].Appearance.BackColor = Color.YellowGreen;
            ultraGridInformation.Rows[i].Cells["QtyFur"].Appearance.BackColor = Color.YellowGreen;
          }
        }

      }
      btnAutoAllocate.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

    }

    private bool ReAllocate()
    {
      DataTable dt = (DataTable)((DataSet)ultraGridInformation.DataSource).Tables[1];
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(dt.Rows[i]["Re_Allocate"].ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@AllocatePid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["AllocatePid"].ToString()));
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPLNReAllocateFurForEnquiry_Delete", input, output);
          if (DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    private bool AutoAllocate()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@EnPid", DbType.Int64, this.enquiryPid);
      input[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNAutoAllocateFurnitureForEnquiry_Auto", input, output);
      if (DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
      {
        return false;
      }
      return true;
    }

    private bool SaveAllocateStatus()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@EnPid", DbType.Int64, this.enquiryPid);
      if (ultCBStatus.Value != null)
      {
        input[1] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNSaveAllocateStatus", input, output);
      if (DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
      {
        return false;
      }
      return true;
    }

    private void LoadCbStatus()
    {
      DataTable dtSourceReport = new DataTable();
      dtSourceReport.Columns.Add("value", typeof(System.Int32));
      dtSourceReport.Columns.Add("text", typeof(System.String));

      DataRow row1 = dtSourceReport.NewRow();
      row1["value"] = 0;
      row1["text"] = "Not Yet Allocate";
      dtSourceReport.Rows.Add(row1);

      DataRow row2 = dtSourceReport.NewRow();
      row2["Value"] = 1;
      row2["Text"] = "Allocated";
      dtSourceReport.Rows.Add(row2);

      DataRow row3 = dtSourceReport.NewRow();
      row3["Value"] = 2;
      row3["Text"] = "Pending";
      dtSourceReport.Rows.Add(row3);

      ControlUtility.LoadUltraCombo(ultCBStatus, dtSourceReport, "value", "text", false, "value");
    }

    #endregion function

    #region event
    public viewPLN_10_050()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_050_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
      this.LoadCbStatus();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnAutoAllocate.Enabled = false;
      bool success = this.AutoAllocate();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.SearchData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      btnAutoAllocate.Enabled = true;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;


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

      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        Type colType = e.Layout.Bands[1].Columns[j].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[1].Columns[j].Format = "#,##0.##";
        }
        e.Layout.Bands[1].Columns[j].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        e.Layout.Bands[1].Columns["Re_Allocate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      }
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      DataTable dt = (DataTable)((DataSet)ultraGridInformation.DataSource).Tables[1];
      if (dt.Rows.Count > 0 && dt != null)
      {
        e.Layout.Bands[1].Columns["AllocatePid"].Hidden = true;
      }

      e.Layout.Bands[1].Columns["Re_Allocate"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["RequiredShipDate"].Header.Caption = "Required \nShip Date";
      e.Layout.Bands[0].Columns["QtyFur"].Header.Caption = "Qty \nFurniture";
      e.Layout.Bands[0].Columns["LeadTimeCOM1"].Header.Caption = "Lead Time COM1\n(days)";
      e.Layout.Bands[0].Columns["LeadTimeASSY"].Header.Caption = "Lead Time ASSY\n(days)";
      e.Layout.Bands[0].Columns["LeadTimeSUB"].Header.Caption = "Lead Time SUB\n(days)";
      e.Layout.Bands[0].Columns["LeadTimeCAR"].Header.Caption = "Lead Time CAR\n(days)";

      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[1].Columns["FurnitureCode"].Header.Caption = "Furniture Code";
      e.Layout.Bands[1].Columns["WorkOrder"].Header.Caption = "Work Order";
      e.Layout.Bands[1].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["WorkAreaCode"].Header.Caption = "Work Area \nCode";
      e.Layout.Bands[1].Columns["PAKDeadline"].Header.Caption = "PAK \nDeadline";
      e.Layout.Bands[1].Columns["ASSYDeadline"].Header.Caption = "ASSY \nDeadline";
      e.Layout.Bands[1].Columns["SUBCONDeadline"].Header.Caption = "SUBCON \nDeadline";
      e.Layout.Bands[1].Columns["COM1Deadline"].Header.Caption = "COM1 \nDeadline";
      e.Layout.Bands[1].Columns["ASSHWStatus"].Header.Caption = "ASSHW \nStatus";
      e.Layout.Bands[1].Columns["FFHWStatus"].Header.Caption = "FFHW \nStatus";
      e.Layout.Bands[1].Columns["MATStatus"].Header.Caption = "MAT \nStatus";
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

    private void btnReAllocate_Click(object sender, EventArgs e)
    {
      btnReAllocate.Enabled = false;
      bool success = this.ReAllocate();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.SearchData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      btnReAllocate.Enabled = true;
    }
    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      FunctionUtility.ExportToOpenOfficeCalc(ultraGridInformation, "Data Allocate Furniture For Enquiry");
    }
    private void ultraGridInformation_DoubleClick(object sender, EventArgs e)
    {
      bool select = false;
      try
      {
        select = ultraGridInformation.Selected.Rows[0].Selected;
      }
      catch
      {
        select = false;
      }
      if (!select)
      {
        return;
      }
      UltraGridRow row = (ultraGridInformation.Selected.Rows[0].ParentRow == null) ? ultraGridInformation.Selected.Rows[0] : ultraGridInformation.Selected.Rows[0].ParentRow;
      long EnDetailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      int qtyEN = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
      int qtyFur = DBConvert.ParseInt(row.Cells["QtyFur"].Value.ToString());
      if (qtyFur < qtyEN)
      {
        viewPLN_10_052 uc = new viewPLN_10_052();
        uc.pidEnDetail = EnDetailPid;
        WindowUtinity.ShowView(uc, "ALLOCATE FURNITURE FOR ENQUIRY MANUAL", false, ViewState.ModalWindow, FormWindowState.Maximized);
        this.SearchData();
      }
    }

    private void btnSaveStatus_Click(object sender, EventArgs e)
    {
      bool success = true;
      if (ultCBStatus.Value == null)
      {
        success = false;
      }
      if (success)
      {
        btnSaveStatus.Enabled = false;
        success = this.SaveAllocateStatus();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
        btnSaveStatus.Enabled = true;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Allocate Status");
      }
    }
    #endregion event


  }
}
