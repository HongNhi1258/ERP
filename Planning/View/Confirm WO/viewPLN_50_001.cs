/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 10/5/2016 
  Description : Confirm WO Online
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_50_001 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    public long transactionPid = long.MinValue;
    public int type = int.MinValue;
    private IList listDeletedPid = new ArrayList();
    private DataTable dtConfirm = new DataTable();
    private DataTable dtKind = new DataTable();
    private DataTable dtDraft = new DataTable();
    private int isFinishedByPLN = 0;

    private string department = string.Empty;

    bool isDraft = false;
    bool isConfirm = false;
    bool isDraftAll = false;
    bool isConfirmAll = false;
    int status = 0;
    int flagConfirm = 0;
    // Lock
    bool isDraftCOM1 = false;
    bool isDraftASSY = false;
    bool isDraftSUB = false;
    bool isDraftCAR = false;
    bool isDraftCOM2 = false;
    bool isPLNSuggest = false;
    bool isDraftSAM = false;
    bool isDraftASSHW = false;
    bool isDraftFFHW = false;
    bool isDraftMAT = false;

    // Confirm
    bool isConfirmCOM1 = false;
    bool isConfirmASSY = false;
    bool isConfirmSUB = false;
    bool isConfirmCAR = false;
    bool isConfirmCOM2 = false;
    bool isConfirmSAM = false;
    bool isConfirmASSHW = false;
    bool isConfirmFFHW = false;
    bool isConfirmMAT = false;

    #endregion Field

    #region Init
    public viewPLN_50_001()
    {
      InitializeComponent();
    }

    private void viewPLN_50_001_Load(object sender, EventArgs e)
    {
      this.dtDraft.Columns.Add("value", typeof(System.Int32));
      this.dtDraft.Columns.Add("text", typeof(System.String));

      this.dtConfirm.Columns.Add("value", typeof(System.Int32));
      this.dtConfirm.Columns.Add("text", typeof(System.String));

      this.dtKind.Columns.Add("value", typeof(System.Int32));
      this.dtKind.Columns.Add("text", typeof(System.String));

      //--- User Login
      // PLN
      if (btnPerPLN.Visible == true)
      {
        this.department = "PLN";
        btnPerPLN.Visible = false;
      }
      // COM1
      if (btnPerCOM1.Visible == true)
      {
        this.department = "COM1";
        //chkConfirm.Enabled = false;
        //btnReturn.Enabled = false;
        //btnAdd.Enabled = false;
        //ultCBWo.Enabled = false;
        //ultcbItem.Enabled = false;
        //txtRemark.Enabled = false;
        btnPerCOM1.Visible = false;
      }
      // ASSY
      if (btnPerASSY.Visible == true)
      {
        this.department = "ASSY";
        //chkConfirm.Enabled = false;
        //btnReturn.Enabled = false;
        //btnAdd.Enabled = false;
        //ultCBWo.Enabled = false;
        //ultcbItem.Enabled = false;
        //txtRemark.Enabled = false;
        btnPerASSY.Visible = false;
      }
      // SUBCON
      if (btnPerSUB.Visible == true)
      {
        this.department = "SUBCON";
        //chkConfirm.Enabled = false;
        //btnReturn.Enabled = false;
        //btnAdd.Enabled = false;
        //ultCBWo.Enabled = false;
        //ultcbItem.Enabled = false;
        //txtRemark.Enabled = false;
        btnPerSUB.Visible = false;
      }
      // CAR
      if (btnPerCAR.Visible == true)
      {
        this.department = "CAR";
        //chkConfirm.Enabled = false;
        //btnReturn.Enabled = false;
        //btnAdd.Enabled = false;
        //ultCBWo.Enabled = false;
        //ultcbItem.Enabled = false;
        //txtRemark.Enabled = false;
        btnPerCAR.Visible = false;
      }
      // PPD
      if (btnPerPPD.Visible == true)
      {
        this.department = "PPD";
        //btnReturn.Enabled = false;
        //btnAdd.Enabled = false;
        //ultCBWo.Enabled = false;
        //ultcbItem.Enabled = false;
        //txtRemark.Enabled = false;
        btnPerPPD.Visible = false;
      }
      // SAM
      if (btnPerSAM.Visible == true)
      {
        this.department = "SAMPLE";
        //chkConfirm.Enabled = false;
        //btnReturn.Enabled = false;
        //btnAdd.Enabled = false;
        //ultCBWo.Enabled = false;
        //ultcbItem.Enabled = false;
        //txtRemark.Enabled = false;
        btnPerSAM.Visible = false;
      }

      // QA
      if (btnPerQA.Visible == true)
      {
        this.department = "QAD";
        //chkConfirm.Enabled = false;
        //btnReturn.Enabled = false;
        //btnAdd.Enabled = false;
        //ultCBWo.Enabled = false;
        //ultcbItem.Enabled = false;
        //btnInputDL.Enabled = false;
        //txtRemark.Enabled = false;
        btnPerQA.Visible = false;
      }

      // Draft
      if (btnDraft.Visible == true)
      {
        isDraft = true;
        btnDraft.Visible = false;

        DataRow row1 = this.dtKind.NewRow();
        row1["value"] = 0;
        row1["text"] = "Draft Deadline";
        this.dtKind.Rows.Add(row1);
      }

      // Confirm
      if (btnConfirm.Visible == true)
      {
        isConfirm = true;
        btnConfirm.Visible = false;

        DataRow row1 = this.dtKind.NewRow();
        row1["value"] = 1;
        row1["text"] = "Confirm Deadline";
        this.dtKind.Rows.Add(row1);
      }

      //--- DRAFT
      // PLN Suggest
      if (btnPLNSuggest.Visible == true)
      {
        isPLNSuggest = true;
        btnPLNSuggest.Visible = false;

        DataRow row1 = this.dtDraft.NewRow();
        row1["value"] = 1;
        row1["text"] = "PLN Suggest Deadline";
        this.dtDraft.Rows.Add(row1);

      }

      //Draft ALL
      if (btnDraftAll.Visible == true)
      {
        isDraftAll = true;
        //chkCOM1.Visible = true;
        btnDraftAll.Visible = false;

        DataRow row1 = this.dtDraft.NewRow();
        row1["value"] = 0;
        row1["text"] = "All";
        this.dtDraft.Rows.Add(row1);

        DataRow row2 = this.dtDraft.NewRow();
        row2["value"] = 2;
        row2["text"] = "COM1 Draft Deadline";
        this.dtDraft.Rows.Add(row2);

        DataRow row3 = this.dtDraft.NewRow();
        row3["value"] = 3;
        row3["text"] = "ASSY Draft Deadline";
        this.dtDraft.Rows.Add(row3);

        DataRow row4 = this.dtDraft.NewRow();
        row4["value"] = 4;
        row4["text"] = "SUB Draft Deadline";
        this.dtDraft.Rows.Add(row4);

        DataRow row5 = this.dtDraft.NewRow();
        row5["value"] = 5;
        row5["text"] = "CAR Draft Deadline";
        this.dtDraft.Rows.Add(row5);

        DataRow row6 = this.dtDraft.NewRow();
        row6["value"] = 6;
        row6["text"] = "ASSHW Draft Deadline";
        this.dtDraft.Rows.Add(row6);

        DataRow row7 = this.dtDraft.NewRow();
        row7["value"] = 7;
        row7["text"] = "FFHW Draft Deadline";
        this.dtDraft.Rows.Add(row7);

        DataRow row8 = this.dtDraft.NewRow();
        row8["value"] = 8;
        row8["text"] = "MAT Draft Deadline";
        this.dtDraft.Rows.Add(row8);

        DataRow row9 = this.dtDraft.NewRow();
        row9["value"] = 9;
        row9["text"] = "Sample Draft Deadline";
        this.dtDraft.Rows.Add(row9);
      }

      //Confirm ALL
      if (btnConfirmAll.Visible == true)
      {
        isDraftAll = true;
        //chkCOM1.Visible = true;
        btnConfirmAll.Visible = false;

        DataRow row1 = this.dtConfirm.NewRow();
        row1["value"] = 0;
        row1["text"] = "All";
        this.dtConfirm.Rows.Add(row1);

        DataRow row2 = this.dtConfirm.NewRow();
        row2["value"] = 2;
        row2["text"] = "COM1 Confirmed Deadline";
        this.dtConfirm.Rows.Add(row2);

        DataRow row3 = this.dtConfirm.NewRow();
        row3["value"] = 3;
        row3["text"] = "ASSY Confirmed Deadline";
        this.dtConfirm.Rows.Add(row3);

        DataRow row4 = this.dtConfirm.NewRow();
        row4["value"] = 4;
        row4["text"] = "SUB Confirmed Deadline";
        this.dtConfirm.Rows.Add(row4);

        DataRow row5 = this.dtConfirm.NewRow();
        row5["value"] = 5;
        row5["text"] = "CAR Confirmed Deadline";
        this.dtConfirm.Rows.Add(row5);

        DataRow row6 = this.dtDraft.NewRow();
        row6["value"] = 6;
        row6["text"] = "ASSHW Confirmed Deadline";
        this.dtDraft.Rows.Add(row6);

        DataRow row7 = this.dtDraft.NewRow();
        row7["value"] = 7;
        row7["text"] = "FFHW Confirmed Deadline";
        this.dtDraft.Rows.Add(row7);

        DataRow row8 = this.dtDraft.NewRow();
        row8["value"] = 8;
        row8["text"] = "MAT Confirmed Deadline";
        this.dtDraft.Rows.Add(row8);

        DataRow row9 = this.dtConfirm.NewRow();
        row9["value"] = 9;
        row9["text"] = "Sample Confirmed Deadline";
        this.dtConfirm.Rows.Add(row9);
      }

      // COM1
      if (btnDraftCOM1.Visible == true)
      {
        isDraftCOM1 = true;
        //chkCOM1.Visible = true;
        btnDraftCOM1.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 2));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 2;
          row1["text"] = "Deadline Draft COM1";
          this.dtDraft.Rows.Add(row1);
        }
      }

      // ASSY
      if (btnDraftASSY.Visible == true)
      {
        isDraftASSY = true;
        //chkASSY.Visible = true;
        btnDraftASSY.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 3));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 3;
          row1["text"] = "Deadline Draft ASSY";
          this.dtDraft.Rows.Add(row1);
        }
      }

      // SUBCON
      if (btnDraftSUB.Visible == true)
      {
        isDraftSUB = true;
        //chkSUBCON.Visible = true;
        btnDraftSUB.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 4));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 4;
          row1["text"] = "Deadline Draft SUB";
          this.dtDraft.Rows.Add(row1);
        }
      }

      // Carcass
      if (btnDraftCAR.Visible == true)
      {
        isDraftCAR = true;
        //chkCAR.Visible = true;
        btnDraftCAR.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 5));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 5;
          row1["text"] = "Deadline Draft CAR";
          this.dtDraft.Rows.Add(row1);
        }
      }

      // ASSHW
      if (btnDraftASSHW.Visible == true)
      {
        isDraftASSHW = true;
        btnDraftASSHW.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 6));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 6;
          row1["text"] = "Deadline Draft ASSHW";
          this.dtDraft.Rows.Add(row1);
        }
      }

      //FFHW
      if (btnDraftFFHW.Visible == true)
      {
        isDraftFFHW = true;
        btnDraftFFHW.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 7));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 7;
          row1["text"] = "Deadline Draft FFHW";
          this.dtDraft.Rows.Add(row1);
        }
      }

      //MAT
      if (btnDraftMAT.Visible == true)
      {
        isDraftMAT = true;
        btnDraftMAT.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 8));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 8;
          row1["text"] = "Deadline Draft MAT";
          this.dtDraft.Rows.Add(row1);
        }
      }

      // SAM
      if (btnDraftSAM.Visible == true)
      {
        isDraftSAM = true;
        btnDraftSAM.Visible = false;

        DataRow[] dr = this.dtDraft.Select(string.Format("value = {0}", 9));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtDraft.NewRow();
          row1["value"] = 9;
          row1["text"] = "Deadline Draft Sample";
          this.dtDraft.Rows.Add(row1);
        }
      }

      //--- Confirm
      // COM1
      if (btnConfirmCOM1.Visible == true)
      {
        isConfirmCOM1 = true;
        btnConfirmCOM1.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 2));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 2;
          row1["text"] = "Deadline Confirmed COM1";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // ASSY
      if (btnConfirmASSY.Visible == true)
      {
        isConfirmASSY = true;
        btnConfirmASSY.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 3));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 3;
          row1["text"] = "Deadline Confirmed ASSY";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // SUBCON
      if (btnConfirmSUB.Visible == true)
      {
        isConfirmSUB = true;
        btnConfirmSUB.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 4));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 4;
          row1["text"] = "Deadline Confirmed SUB";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // CAR
      if (btnConfirmCAR.Visible == true)
      {
        isConfirmCAR = true;
        //chkCARConfirm.Visible = true;
        btnConfirmCAR.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 5));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 5;
          row1["text"] = "Deadline Confirmed CAR";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // ASSHW
      if (btnConfirmASSHW.Visible == true)
      {
        isConfirmASSHW = true;
        btnConfirmASSHW.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 6));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 6;
          row1["text"] = "Deadline Confirmed ASSHW";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // FFHW
      if (btnConfirmFFHW.Visible == true)
      {
        isConfirmFFHW = true;
        btnConfirmFFHW.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 7));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 7;
          row1["text"] = "Deadline Confirmed FFHW";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // MAT
      if (btnConfirmMAT.Visible == true)
      {
        isConfirmMAT = true;
        btnConfirmMAT.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 8));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 8;
          row1["text"] = "Deadline Confirmed MAT";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // SAM
      if (btnConfirmSAM.Visible == true)
      {
        isConfirmSAM = true;
        btnConfirmSAM.Visible = false;

        DataRow[] dr = this.dtConfirm.Select(string.Format("value = {0}", 9));
        if (dr.Length == 0)
        {
          DataRow row1 = this.dtConfirm.NewRow();
          row1["value"] = 9;
          row1["text"] = "Deadline Confirmed Sample";
          this.dtConfirm.Rows.Add(row1);
        }
      }

      // Add ask before closing form even if user change data
      //this.SetAutoAskSaveWhenCloseForm(groupBoxMaster);

      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
    ///// <summary>
    ///// Set Auto Ask Save Data When User Close Form
    ///// </summary>
    ///// <param name="groupControl"></param>
    //private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    //{
    //  // Add KeyDown even for all controls in groupBoxSearch
    //  foreach (Control ctr in groupControl.Controls)
    //  {
    //    if (ctr.Controls.Count == 0)
    //    {
    //      ctr.TextChanged += new System.EventHandler(this.Object_Changed);
    //    }
    //    else
    //    {
    //      this.SetAutoAskSaveWhenCloseForm(ctr);
    //    }
    //  }
    //}

    private void LoadCbWO()
    {
      string cm = string.Format(@"SELECT DISTINCT WorkOrderPid WorkOrder
                                    FROM TblPLNWorkOrderConfirmedDetails
                                    ORDER BY WorkOrderPid");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultCBWo, dt, "WorkOrder", "WorkOrder", false);
    }

    private void LoadCbItem(long wo)
    {
      string cm = String.Empty;
      if (this.type == 0)
      {
        cm = string.Format(@"SELECT WO.ItemCode, WO.Revision, WO.ItemCode + ' - '+CAST(WO.Revision AS VARCHAR(16)) Display
                                  FROM TblPLNWorkOrderConfirmedDetails WO
                                      LEFT JOIN TblPLNWODraftDeadlineDetail WDT ON WO.WorkOrderPid = WDT.WO
													                                      AND WO.ItemCode = WDT.ItemCode
													                                      AND WO.Revision = WDT.Revision
													                                      AND WO.CarcassCode = WDT.CarcassCode
                                  WHERE WDT.ItemCode IS NULL AND WO.WorkOrderPid = {0}
                                  ORDER BY WO.ItemCode, WO.Revision", wo);
      }
      else
      {
        cm = string.Format(@"SELECT WO.ItemCode, WO.Revision, WO.ItemCode + ' - '+CAST(WO.Revision AS VARCHAR(16)) Display
                                  FROM TblPLNWorkOrderConfirmedDetails WO
                                      LEFT JOIN TblPLNWODraftDeadlineDetail WDT ON WO.WorkOrderPid = WDT.WO
													                                      AND WO.ItemCode = WDT.ItemCode
													                                      AND WO.Revision = WDT.Revision
													                                      AND WO.CarcassCode = WDT.CarcassCode
                                  WHERE WO.WorkOrderPid = {0}
                                  ORDER BY WO.ItemCode, WO.Revision", wo);
      }
      //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      string[] hiden = new string[2];
      hiden[0] = "ItemCode";
      hiden[1] = "Revision";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultcbItem, dt, "ItemCode", "Display", false, hiden);
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

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadCbWO();
    }

    /// <summary>
    /// READ ONLY ACCESS
    /// </summary>
    private void ReadOnlyAccess()
    {
      //PLN
      if (this.department == "PLN")
      {
        if (this.status >= 1 && this.status < 2)
        {
          chkConfirm.Checked = true;
          //btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
      }

      //PPD
      if (this.department == "PPD")
      {
        if (this.status >= 2)
        {
          chkConfirm.Checked = true;
          //btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
      }
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.department == "PLN")
      {
        if (this.status == 0) // New
        {
          btnReturn.Enabled = false;
        }
        else if (this.status == 1) // PLN Confired
        {
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
          btnSave.Text = "Save";
          btnAdd.Enabled = false;
          if (this.isFinishedByPLN == 1)
          {
            chkConfirm.Enabled = false;
            btnSave.Enabled = true;
            btnSave.Text = "Finish";
            btnReturn.Enabled = true;
            btnAdd.Enabled = false;
          }
        }
        else if (this.status == 2) // PPD Confirmed
        {
          chkConfirm.Enabled = false;
          btnSave.Enabled = true;
          btnSave.Text = "Finish";
          btnReturn.Enabled = true;
          btnAdd.Enabled = false;
        }
        else if (this.status == 3) // Finished
        {
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
          btnInputDL.Enabled = false;
          btnAdd.Enabled = false;
        }
      }
      else if (this.department == "PPD")
      {
        if (this.status == 0)
        {
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
          btnInputDL.Enabled = false;
        }
        else if (this.status == 1) // PLN Confired
        {
          chkConfirm.Enabled = true;
          btnSave.Enabled = true;
          btnReturn.Enabled = true;
          btnInputDL.Enabled = true;
        }
        else if (this.status >= 2) // PPD Confirmed, Finished
        {
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
          btnInputDL.Enabled = false;
        }
      }
      else
      {
        chkConfirm.Enabled = false;
        if (this.status <= 1) // New , PLN Confirmed
        {
          btnInputDL.Enabled = true;
        }
        else
        {
          btnInputDL.Enabled = false;
        }
      }
    }

    /// <summary>
    /// Load Main Data
    /// </summary>
    /// <param name="dtMain"></param>
    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        DataRow row = dtMain.Rows[0];
        txtTransaction.Text = row["TransactionCode"].ToString();
        txtCreateBy.Text = row["CreateBy"].ToString();
        txtRemark.Text = row["Remark"].ToString();
        this.status = DBConvert.ParseInt(row["Status"].ToString());
        this.flagConfirm = DBConvert.ParseInt(row["Status"].ToString());
        this.isFinishedByPLN = DBConvert.ParseInt(row["IsFinishedByPLN"].ToString());
      }
      else
      {
        System.Data.DataTable dtTranNo = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNGetNewTransactionCodeConfirmWOL() NewTranNo");
        if ((dtTranNo != null) && (dtTranNo.Rows.Count > 0))
        {
          txtTransaction.Text = dtTranNo.Rows[0]["NewTranNo"].ToString();
        }
        txtCreateBy.Text = SharedObject.UserInfo.EmpName;
      }

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Load data
    /// </summary>
    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.transactionPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNConfirmWOL_LoadData", 600, inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        // Load Main
        this.LoadMainData(dsSource.Tables[0]);

        // Load Detail
        ultData.DataSource = dsSource.Tables[1];

        // Set Status Gird
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (this.department == "QAD")
          {
            ultData.Rows[i].Cells["PLNRemark"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["PPDRemark"].Activation = Activation.ActivateOnly;
          }
          else if (this.department == "PLN")
          {
            ultData.Rows[i].Cells["QARemark"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["PPDRemark"].Activation = Activation.ActivateOnly;
          }
          else if (this.department == "PPD")
          {
            ultData.Rows[i].Cells["PLNRemark"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["QARemark"].Activation = Activation.ActivateOnly;
          }

          if (ultData.Rows[i].Cells["COM1"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["COM1DraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["COM1ConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["COM1Remark"].Appearance.BackColor = Color.LightGray;
          }

          if (ultData.Rows[i].Cells["ASSY"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["ASSYDraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["ASSYConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["ASSYRemark"].Appearance.BackColor = Color.LightGray;
          }

          if (ultData.Rows[i].Cells["SUB"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["SUBDraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["SUBConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["SUBRemark"].Appearance.BackColor = Color.LightGray;
          }

          if (ultData.Rows[i].Cells["CAR"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["PAKDraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["PAKConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["PAKRemark"].Appearance.BackColor = Color.LightGray;
          }

          if (ultData.Rows[i].Cells["ASSHW"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["ASSYHWDraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["ASSYHWConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["ASSYHWRemark"].Appearance.BackColor = Color.LightGray;
          }

          if (ultData.Rows[i].Cells["FFHW"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["FFHWDraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["FFHWConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["FFHWRemark"].Appearance.BackColor = Color.LightGray;
          }

          if (ultData.Rows[i].Cells["MAT"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["MATDraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["MATConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["MATRemark"].Appearance.BackColor = Color.LightGray;
          }

          if (ultData.Rows[i].Cells["SAM"].Value.ToString() == "1")
          {
            ultData.Rows[i].Cells["SAMDraftDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["SAMConfirmedDeadline"].Appearance.BackColor = Color.LightGray;
            ultData.Rows[i].Cells["SAMRemark"].Appearance.BackColor = Color.LightGray;
          }
        }
      }
      this.NeedToSave = false;
    }

    /// <summary>
    ///  Check Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        long wo = DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString());
        string item = ultData.Rows[i].Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString());
        string cmtext = string.Format(@"SELECT WorkOrderPid, ItemCode, Revision 
                                        FROM TblPLNWorkOrderConfirmedDetails
                                        WHERE WorkOrderPid = {0} 
                                        AND ItemCode = '{1}' 
                                        AND Revision = {2} ", wo, item, revision);
        DataTable dtWOInfo = DataBaseAccess.SearchCommandTextDataTable(cmtext);
        if (dtWOInfo == null || dtWOInfo.Rows.Count <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0029", "WorkOrder: " + wo.ToString() + ", ItemCode: " + item + ", Rev: " + revision.ToString());
          return false;
        }
        string cm = String.Empty;
        if (this.type == 0 && this.transactionPid < 0)
        {
          cm = string.Format(@"SELECT WO.ItemCode, WO.Revision
                               FROM TblPLNWorkOrderConfirmedDetails WO
                                    INNER JOIN TblPLNWODraftDeadlineDetail WDT ON WO.WorkOrderPid = WDT.WO
											                                        AND WO.ItemCode = WDT.ItemCode
											                                        AND WO.Revision = WDT.Revision
											                                        AND WO.CarcassCode = WDT.CarcassCode
                               WHERE WO.WorkOrderPid = {0} AND WO.ItemCode = '{1}' AND WO.Revision = {2}", wo, item, revision);
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
          if (dt != null && dt.Rows.Count > 0)
          {
            //ultData.Rows[i].Cells["WorkOrder"].Appearance.BackColor = Color.Yellow;
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0028", "WorkOrder: " + wo.ToString() + ", ItemCode: " + item + ", Rev: " + revision.ToString());
            return false;
          }
        }
      }

      return true;
    }

    /// <summary>
    /// Save Main Data
    /// </summary>
    /// <returns></returns>
    private bool SaveMain()
    {
      bool result = false;
      string storeName = "spPLNConfirmWOLInfo_Edit";
      int paramNumber = 7;
      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (this.transactionPid > 0)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
      }

      inputParam[1] = new DBParameter("@TransactionCode", DbType.String, txtTransaction.Text);

      // Status
      if (this.department == "PLN")
      {
        if (this.flagConfirm == 0 && chkConfirm.Checked)
        {
          this.status = 1;
        }
        else if (this.flagConfirm == 2 || this.isFinishedByPLN == 1)
        {
          this.status = 3;
        }
      }
      else if (this.department == "PPD")
      {
        if (this.flagConfirm == 1 && chkConfirm.Checked)
        {
          this.status = 2;
        }
      }
      else
      {
        this.status = this.flagConfirm;
      }
      inputParam[2] = new DBParameter("@Status", DbType.Int32, this.status);
      // End Status

      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@Remark", DbType.String, 4000, txtRemark.Text.Trim());
      }

      inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[5] = new DBParameter("@Type", DbType.Int32, this.type);
      inputParam[6] = new DBParameter("@Dept", DbType.String, this.department);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.transactionPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        result = this.SaveDetail(this.transactionPid);
      }
      return result;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <param name="transactionPid"></param>
    /// <returns></returns>
    private bool SaveDetail(long transactionPid)
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWODraftDeadline_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update  
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (row.Cells["isUpdate"].Value.ToString() == "1")
        {
          DBParameter[] inputParam = new DBParameter[10];
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pid > 0) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[1] = new DBParameter("@TransactionPid", DbType.Int64, transactionPid);
          inputParam[2] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["WorkOrder"].Value.ToString()));
          inputParam[3] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
          inputParam[4] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
          inputParam[5] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
          inputParam[6] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()));
          inputParam[7] = new DBParameter("@PLNRemark", DbType.String, row.Cells["PLNRemark"].Value.ToString());
          inputParam[8] = new DBParameter("@PPDRemark", DbType.String, row.Cells["PPDRemark"].Value.ToString());
          inputParam[9] = new DBParameter("@QARemark", DbType.String, row.Cells["QARemark"].Value.ToString());
          DataBaseAccess.ExecuteStoreProcedure("spPLNConfirmWOLDetail_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }

      if (this.department == "PLN" && this.status == 1 && chkConfirm.Checked)
      {
        DBParameter[] inputConfirmParam = new DBParameter[2];
        inputConfirmParam[0] = new DBParameter("@TransactionPid", DbType.Int64, transactionPid);
        inputConfirmParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        DataBaseAccess.ExecuteStoreProcedure("spPLNConfirmWOLConfirmDeadline_Edit", inputConfirmParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }

      if (this.department == "PLN" && this.status == 3)
      {
        DBParameter[] inputFinishParam = new DBParameter[2];
        inputFinishParam[0] = new DBParameter("@TransactionPid", DbType.Int64, transactionPid);
        inputFinishParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        DataBaseAccess.ExecuteStoreProcedure("spPLNConfirmedWODraftDeadline_Finished", 600, inputFinishParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      return success;
    }


    /// <summary>
    /// Save Data
    /// </summary>
    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = this.SaveMain();
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
      //else
      //{
      //  WindowUtinity.ShowMessageError("ERR0005");
      //  this.SaveSuccess = false;
      //}
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Bands[0].ColHeaderLines = 3;
      e.Layout.UseFixedHeaders = true;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["PLNRemark"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["PPDRemark"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["QARemark"].CellActivation = Activation.AllowEdit;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["isUpdate"].Hidden = true;
      e.Layout.Bands[0].Columns["COM1"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSY"].Hidden = true;
      e.Layout.Bands[0].Columns["SUB"].Hidden = true;
      e.Layout.Bands[0].Columns["CAR"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSHW"].Hidden = true;
      e.Layout.Bands[0].Columns["FFHW"].Hidden = true;
      e.Layout.Bands[0].Columns["MAT"].Hidden = true;
      e.Layout.Bands[0].Columns["SAM"].Hidden = true;

      e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["SaleCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Yellow;

      // CAR
      e.Layout.Bands[0].Columns["PAKDraftDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["PAKConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["PAKRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

      //COM1
      e.Layout.Bands[0].Columns["COM1DraftDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1ConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1Remark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

      // SUBCON
      e.Layout.Bands[0].Columns["SUBDraftDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

      // ASSY
      e.Layout.Bands[0].Columns["ASSYDraftDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYRemark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

      // SAM
      e.Layout.Bands[0].Columns["SAMDraftDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SAMConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SAMRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

      // ASSYHW
      e.Layout.Bands[0].Columns["ASSYHWDraftDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYHWConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYHWRemark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

      // FFHW
      e.Layout.Bands[0].Columns["FFHWDraftDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

      // MAT
      e.Layout.Bands[0].Columns["MATDraftDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATConfirmedDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATRemark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

      e.Layout.Bands[0].Columns["WorkOrder"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Revision"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Qty"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["PAKDraftDeadline"].Header.Caption = "PAK\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["PAKConfirmedDeadline"].Header.Caption = "PAK\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["PAKRemark"].Header.Caption = "PAK\nRemark";
      e.Layout.Bands[0].Columns["CreateByPAK"].Header.Caption = "PAK\nCreateBy";
      e.Layout.Bands[0].Columns["CreateDatePAK"].Header.Caption = "PAK\nCreateDate";

      e.Layout.Bands[0].Columns["COM1DraftDeadline"].Header.Caption = "COM1\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["COM1ConfirmedDeadline"].Header.Caption = "COM1\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["COM1Remark"].Header.Caption = "COM1\nRemark";
      e.Layout.Bands[0].Columns["CreateByCOM1"].Header.Caption = "COM1\nCreateBy";
      e.Layout.Bands[0].Columns["CreateDateCOM1"].Header.Caption = "COM1\nCreateDate";

      e.Layout.Bands[0].Columns["ASSYHWDraftDeadline"].Header.Caption = "ASSHW\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["ASSYHWConfirmedDeadline"].Header.Caption = "ASSHW\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["ASSYRemark"].Header.Caption = "ASSY\nRemark";
      e.Layout.Bands[0].Columns["CreateByASSY"].Header.Caption = "ASSY\nCreateBy";
      e.Layout.Bands[0].Columns["CreateDateASSY"].Header.Caption = "ASSY\nCreateDate";

      e.Layout.Bands[0].Columns["FFHWDraftDeadline"].Header.Caption = "FFHW\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["FFHWConfirmedDeadline"].Header.Caption = "FFHW\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["FFHWRemark"].Header.Caption = "FFHW\nRemark";
      e.Layout.Bands[0].Columns["CreateByFFHW"].Header.Caption = "FFHW\nCreateBy";
      e.Layout.Bands[0].Columns["CreateDateFFHW"].Header.Caption = "FFHW\nCreateDate";

      e.Layout.Bands[0].Columns["MATDraftDeadline"].Header.Caption = "MAT\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["MATConfirmedDeadline"].Header.Caption = "MAT\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["MATRemark"].Header.Caption = "MAT\nRemark";
      e.Layout.Bands[0].Columns["CreateByMAT"].Header.Caption = "MAT\nCreateBy";
      e.Layout.Bands[0].Columns["CreateDateMAT"].Header.Caption = "MAT\nCreateDate";

      e.Layout.Bands[0].Columns["SUBDraftDeadline"].Header.Caption = "SUB\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["SUBConfirmedDeadline"].Header.Caption = "SUB\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["SUBRemark"].Header.Caption = "SUB\nRemark";
      e.Layout.Bands[0].Columns["CreateBySUB"].Header.Caption = "SUB\nCreateBy";
      e.Layout.Bands[0].Columns["CreateDateSUB"].Header.Caption = "SUB\nCreateDate";

      e.Layout.Bands[0].Columns["ASSYDraftDeadline"].Header.Caption = "ASSY_SAN\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["ASSYConfirmedDeadline"].Header.Caption = "ASSY_SAN\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["ASSYRemark"].Header.Caption = "ASSY_SAN\nRemark";
      e.Layout.Bands[0].Columns["CreateByASSY"].Header.Caption = "ASSY_SAN\nCreateBy";
      e.Layout.Bands[0].Columns["CreateDateASSY"].Header.Caption = "ASSY_SAN\nCreateDate";

      e.Layout.Bands[0].Columns["SAMDraftDeadline"].Header.Caption = "SAM\nDraft\nDeadline";
      e.Layout.Bands[0].Columns["SAMConfirmedDeadline"].Header.Caption = "SAM\nConfirmed\nDeadline";
      e.Layout.Bands[0].Columns["SAMRemark"].Header.Caption = "SAM\nRemark";
      e.Layout.Bands[0].Columns["CreateBySAM"].Header.Caption = "SAM\nCreateBy";
      e.Layout.Bands[0].Columns["CreateBySAM"].Header.Caption = "SAM\nCreateBy";
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      string colName = e.Cell.Column.ToString().Trim();
      switch (colName)
      {
        case "PLNRemark":
          e.Cell.Row.Cells["isUpdate"].Value = 1;
          break;
        case "PPDRemark":
          e.Cell.Row.Cells["isUpdate"].Value = 1;
          break;
        case "QARemark":
          e.Cell.Row.Cells["isUpdate"].Value = 1;
          break;
      }
    }


    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "Data");
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultData);
      }
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultData.Selected.Rows.Count > 0 || ultData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultData, new Point(e.X, e.Y));
        }
      }
      Shared.Utility.ControlUtility.BOMShowItemImage(ultData, grpItemPicture, picItem, chkShowImage.Checked);
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (ultCBWo.Value != null)
      {
        DBParameter[] inputParam = new DBParameter[4];
        inputParam[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWo.Value.ToString()));
        if (ultcbItem.Value != null)
        {
          inputParam[1] = new DBParameter("@ItemCode", DbType.String, ultcbItem.Value.ToString());
          inputParam[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultcbItem.SelectedRow.Cells["Revision"].Value.ToString()));
        }
        if (this.type == 0)
        {
          inputParam[2] = new DBParameter("@Type", DbType.Int32, 0);
        }
        else
        {
          inputParam[2] = new DBParameter("@Type", DbType.Int32, 1);
        }
        DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNConfirmWOL_Add", inputParam);
        DataTable dtMain = (DataTable)ultData.DataSource;
        if (dt != null)
        {
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            DataRow row = dtMain.NewRow();
            DataRow rowAdd = dt.Rows[i];
            long wo = DBConvert.ParseLong(rowAdd["WorkOrder"].ToString());
            string saleCode = rowAdd["SaleCode"].ToString();
            string item = rowAdd["ItemCode"].ToString();
            int revision = DBConvert.ParseInt(rowAdd["Revision"].ToString());
            string carcass = rowAdd["CarcassCode"].ToString();
            DataRow[] dr = dtMain.Select(string.Format("WorkOrder = {0} AND CarcassCode = '{1}' AND SaleCode = '{2}' AND ItemCode = '{3}' AND Revision = {4}", wo, carcass, saleCode, item, revision));
            if (dr.Length < 1)
            {
              row["Pid"] = DBConvert.ParseLong(rowAdd["Pid"].ToString());
              row["TransactionPid"] = DBConvert.ParseLong(rowAdd["TransactionPid"].ToString());
              row["WorkOrder"] = DBConvert.ParseLong(rowAdd["WorkOrder"].ToString());
              row["CarcassCode"] = rowAdd["CarcassCode"].ToString();
              row["SaleCode"] = rowAdd["SaleCode"].ToString();
              row["ItemCode"] = rowAdd["ItemCode"].ToString();
              row["Revision"] = DBConvert.ParseInt(rowAdd["Revision"].ToString());
              row["Qty"] = DBConvert.ParseInt(rowAdd["Qty"].ToString());
              if (row["PAKDraftDeadline"].ToString().Trim().Length > 0)
              {
                row["PAKDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["PAKDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["PAKConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["PAKConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["PAKConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              row["Unit"] = rowAdd["Unit"].ToString();
              row["CBM"] = DBConvert.ParseDouble(rowAdd["CBM"].ToString());
              row["TotalCBM"] = DBConvert.ParseDouble(rowAdd["TotalCBM"].ToString());
              row["VP"] = DBConvert.ParseDouble(rowAdd["VP"].ToString());
              row["TotalVP"] = DBConvert.ParseDouble(rowAdd["TotalVP"].ToString());
              if (row["COM1DraftDeadline"].ToString().Trim().Length > 0)
              {
                row["COM1DraftDeadline"] = DBConvert.ParseDateTime(rowAdd["COM1DraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["COM1ConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["COM1ConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["COM1ConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["ASSYHWDraftDeadline"].ToString().Trim().Length > 0)
              {
                row["ASSYHWDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSHWDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["ASSYHWConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["ASSYHWConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSHWConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["SUBDraftDeadline"].ToString().Trim().Length > 0)
              {
                row["SUBDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["SUBDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["SUBConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["SUBConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["SUBConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["ASSYDraftDeadline"].ToString().Trim().Length > 0)
              {
                row["ASSYDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSY_SandingDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["ASSYConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["ASSYConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSY_SandingConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["FFHWDraftDeadline"].ToString().Trim().Length > 0)
              {
                row["FFHWDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["FFHWDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["FFHWConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["FFHWConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["FFHWConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["MATDraftDeadline"].ToString().Trim().Length > 0)
              {
                row["MATDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["MATDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["MATConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["MATConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["MATConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["SAMDraftDeadline"].ToString().Trim().Length > 0)
              {
                row["SAMDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["SAMDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              if (row["SAMConfirmedDeadline"].ToString().Trim().Length > 0)
              {
                row["SAMConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["SAMConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
              }
              row["isUpdate"] = DBConvert.ParseInt(rowAdd["isUpdate"].ToString());
              dtMain.Rows.Add(row);
            }
          }
        }
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (this.department == "QAD")
          {
            ultData.Rows[i].Cells["PLNRemark"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["PPDRemark"].Activation = Activation.ActivateOnly;
          }
          else if (this.department == "PLN")
          {
            ultData.Rows[i].Cells["QARemark"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["PPDRemark"].Activation = Activation.ActivateOnly;
          }
          else if (this.department == "PPD")
          {
            ultData.Rows[i].Cells["PLNRemark"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["QARemark"].Activation = Activation.ActivateOnly;
          }
        }
      }
    }
    private DataTable dtWOConfirmResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int16));
      dt.Columns.Add("PLNRemark", typeof(System.String));
      return dt;
    }
    private void btnInputDL_Click(object sender, EventArgs e)
    {
      viewPLN_50_002 view = new viewPLN_50_002();
      view.dtDraft = this.dtDraft;
      view.dtConfirm = this.dtConfirm;
      view.dept = this.department;
      view.dtKind = this.dtKind;
      view.transactionPid = this.transactionPid;
      WindowUtinity.ShowView(view, "Input Deadline", true, ViewState.ModalWindow, FormWindowState.Maximized);
      if (view.flagSearch == true)
      {
        this.LoadData();
      }
    }

    private void btnReturn_Click(object sender, EventArgs e)
    {
      string storeName = "spPLNConfirmWOLInfo_Return";
      int paramNumber = 4;
      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (this.transactionPid > 0)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
      }

      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@Remark", DbType.String, 4000, txtRemark.Text.Trim());
      }

      inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[3] = new DBParameter("@Type", DbType.String, this.department);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
        if (this.department == "PLN")
        {
          chkConfirm.Enabled = true;
          btnSave.Enabled = true;
          btnReturn.Enabled = true;
          btnAdd.Enabled = true;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.BOMShowItemImage(ultData, grpItemPicture, picItem, chkShowImage.Checked);
    }

    private void btnSendMail_Click(object sender, EventArgs e)
    {
      viewPLN_50_006 view = new viewPLN_50_006();
      view.pid = this.transactionPid;
      WindowUtinity.ShowView(view, "Send Mail", true, ViewState.Window);
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    /// <summary>
    /// Import Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get Items Count

      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [WOConfirm (1)$F3:F4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "WO Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "WO Count");
          return;
        }
      }
      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [WOConfirm (1)$B5:E{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      //input data
      DataSet dt = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [WOConfirm (1)$B5:E{0}]", itemCount + 5));
      DataTable dtSource = new DataTable();
      dtSource = dt.Tables[0];

      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtInput = this.dtWOConfirmResult();
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (DBConvert.ParseString(dtSource.Rows[i][1].ToString()) != "" && DBConvert.ParseString(dtSource.Rows[i][2].ToString()) != "")
        {
          if (DBConvert.ParseString(row["WO"].ToString()) != "")
          {
            rowadd["WO"] = DBConvert.ParseLong(row["WO"]);
          }
          if (DBConvert.ParseString(row["ItemCode"].ToString()) != "")
          {
            rowadd["ItemCode"] = DBConvert.ParseString(row["ItemCode"]);
          }
          if (DBConvert.ParseString(row["Revision"].ToString()) != "")
          {
            rowadd["Revision"] = DBConvert.ParseInt(row["Revision"]);
          }
          if (DBConvert.ParseString(row["PLNRemark"].ToString()) != "")
          {
            rowadd["PLNRemark"] = DBConvert.ParseString(row["PLNRemark"]);
          }
          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dtInput);
      DataTable dtSource1 = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNConfirmWO_ImportData", sqlinput);
      DataTable dtMain = (DataTable)ultData.DataSource;
      if (dtSource1 != null)
      {
        for (int i = 0; i < dtSource1.Rows.Count; i++)
        {
          DataRow row = dtMain.NewRow();
          DataRow rowAdd = dtSource1.Rows[i];
          long wo = DBConvert.ParseLong(rowAdd["WorkOrder"].ToString());
          string saleCode = rowAdd["SaleCode"].ToString();
          string item = rowAdd["ItemCode"].ToString();
          int revision = DBConvert.ParseInt(rowAdd["Revision"].ToString());
          string carcass = rowAdd["CarcassCode"].ToString();
          DataRow[] dr = dtMain.Select(string.Format("WorkOrder = {0} AND CarcassCode = '{1}' AND SaleCode = '{2}' AND ItemCode = '{3}' AND Revision = {4}", wo, carcass, saleCode, item, revision));
          if (dr.Length < 1)
          {
            row["Pid"] = DBConvert.ParseLong(rowAdd["Pid"].ToString());
            row["TransactionPid"] = DBConvert.ParseLong(rowAdd["TransactionPid"].ToString());
            row["WorkOrder"] = DBConvert.ParseLong(rowAdd["WorkOrder"].ToString());
            row["CarcassCode"] = rowAdd["CarcassCode"].ToString();
            row["SaleCode"] = rowAdd["SaleCode"].ToString();
            row["ItemCode"] = rowAdd["ItemCode"].ToString();
            row["Revision"] = DBConvert.ParseInt(rowAdd["Revision"].ToString());
            row["Qty"] = DBConvert.ParseInt(rowAdd["Qty"].ToString());
            if (row["PAKDraftDeadline"].ToString().Trim().Length > 0)
            {
              row["PAKDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["PAKDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["PAKConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["PAKConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["PAKConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            row["Unit"] = rowAdd["Unit"].ToString();
            row["CBM"] = DBConvert.ParseDouble(rowAdd["CBM"].ToString());
            row["TotalCBM"] = DBConvert.ParseDouble(rowAdd["TotalCBM"].ToString());
            row["VP"] = DBConvert.ParseDouble(rowAdd["VP"].ToString());
            row["TotalVP"] = DBConvert.ParseDouble(rowAdd["TotalVP"].ToString());
            row["PLNRemark"] = rowAdd["PLNRemark"].ToString();
            if (row["COM1DraftDeadline"].ToString().Trim().Length > 0)
            {
              row["COM1DraftDeadline"] = DBConvert.ParseDateTime(rowAdd["COM1DraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["COM1ConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["COM1ConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["COM1ConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["ASSYHWDraftDeadline"].ToString().Trim().Length > 0)
            {
              row["ASSYHWDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSHWDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["ASSYHWConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["ASSYHWConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSHWConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["SUBDraftDeadline"].ToString().Trim().Length > 0)
            {
              row["SUBDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["SUBDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["SUBConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["SUBConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["SUBConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["ASSYDraftDeadline"].ToString().Trim().Length > 0)
            {
              row["ASSYDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSY_SandingDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["ASSYConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["ASSYConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["ASSY_SandingConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["FFHWDraftDeadline"].ToString().Trim().Length > 0)
            {
              row["FFHWDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["FFHWDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["FFHWConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["FFHWConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["FFHWConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["MATDraftDeadline"].ToString().Trim().Length > 0)
            {
              row["MATDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["MATDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["MATConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["MATConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["MATConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["SAMDraftDeadline"].ToString().Trim().Length > 0)
            {
              row["SAMDraftDeadline"] = DBConvert.ParseDateTime(rowAdd["SAMDraftDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["SAMConfirmedDeadline"].ToString().Trim().Length > 0)
            {
              row["SAMConfirmedDeadline"] = DBConvert.ParseDateTime(rowAdd["SAMConfirmedDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            row["isUpdate"] = DBConvert.ParseInt(rowAdd["isUpdate"].ToString());
            dtMain.Rows.Add(row);
          }
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (this.department == "QAD")
        {
          ultData.Rows[i].Cells["PLNRemark"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["PPDRemark"].Activation = Activation.ActivateOnly;
        }
        else if (this.department == "PLN")
        {
          ultData.Rows[i].Cells["QARemark"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["PPDRemark"].Activation = Activation.ActivateOnly;
        }
        else if (this.department == "PPD")
        {
          ultData.Rows[i].Cells["PLNRemark"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["QARemark"].Activation = Activation.ActivateOnly;
        }
      }
    }

    /// <summary>
    /// Get template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "PLN_50_001";
      string sheetName = "WOConfirm";
      string outFileName = "WOConfirm";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void ultCBWo_ValueChanged(object sender, EventArgs e)
    {
      ultcbItem.Value = null;
      if (ultCBWo.Value != null)
      {
        this.LoadCbItem(DBConvert.ParseLong(ultCBWo.Value.ToString()));
      }
      else
      {
        this.LoadCbItem(-1);
      }
    }

    #endregion Event


  }
}
