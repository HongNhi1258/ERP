/*****************************************
 * Author: Nguyen Ngoc Tien              *
 * Create Date: 18/10/2014               *
 * Description: Define item with process *
 * ***************************************/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_96_004 : MainUserControl
  {

    public viewWIP_96_004()
    {
      InitializeComponent();
    }

    #region Init
    //Biến phân quyền Planing
    private bool bNew = true;
    //Biến phân quyền Carcass
    private bool bConfirm = true;
    //Bien phan quen Tec
    private bool bTec = true;
    //Biến chứa Pid Item
    public long pidItem = long.MinValue;
    //Biến chứa Item Code
    private string itemCode;
    //Biến giữ giá trị revision
    private int revision;
    //Biến giữ giá trị Pid Detail
    private long pidDetail = long.MinValue;
    //Biến giữ giá trị Default
    private int isDefault = 1;
    //Biến giữ giá trị Confirm
    private int isConfirm = 0;
    private int flagConfirm = 0;
    //Mảng giữ giá trị pid process để delete
    private IList listDeletedPid = new ArrayList();
    private IList listDeleteComp = new ArrayList();
    //
    private long pidtemp = long.MinValue + 1;
    //
    private bool isDuplicateProcess = false;
    //
    private int totalPartPercent;
    //
    private bool isReference = false;

    private int location = int.MinValue;
    /// <summary>
    /// Load View
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewWIP_96_004_Load(object sender, EventArgs e)
    {
      ///Phan quyen
      this.bNew = btnNew.Visible;
      this.bConfirm = btnConfirm.Visible;
      this.bTec = btTec.Visible;
      //Hide nut phan quyen
      this.pnlAccess.Visible = false;
      /////////////////////
      ultraCBPartGroup.ReadOnly = true;
      CBRevision.Enabled = false;
      if (bNew == true && bTec == true && bConfirm == true)
      {
        label9.Visible = true;
        ultItemReference.Visible = true;
        btnCopy.Visible = true;
      }
      ///////////////////
      LoadItem();
      LoadPartGroup();
      LoadDDProcess();
      LoadPartType();
      LoadLocationDefault();
      this.LoadSupplier();
      LoadItemReference();
      //Load du lieu từ list khi double item từ màn hình list                       
      if (pidItem != long.MinValue)
      {
        LoadDataFromList(pidItem);
      }
    }

    #endregion

    #region Function

    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();

      //Table Detail

      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("PidDetail", typeof(System.Int64));
      dtDetail.Columns.Add("MasterPid", typeof(System.Int64));
      dtDetail.Columns.Add("PartCodePid", typeof(System.Int64));
      dtDetail.Columns.Add("PartCode", typeof(System.String));
      dtDetail.Columns.Add("PartName", typeof(System.String));
      dtDetail.Columns.Add("PartType", typeof(System.Int32));
      dtDetail.Columns.Add("PartPercent", typeof(System.Double));
      dtDetail.Columns.Add("LocationDefault", typeof(System.Int64));
      dtDetail.Columns.Add("Supplier1", typeof(System.Int64));
      dtDetail.Columns.Add("SupLeadtime1", typeof(System.Double));
      dtDetail.Columns.Add("Supplier2", typeof(System.Int64));
      dtDetail.Columns.Add("SupLeadtime2", typeof(System.Double));
      dtDetail.Columns.Add("Supplier3", typeof(System.Int64));
      dtDetail.Columns.Add("SupLeadtime3", typeof(System.Double));
      dtDetail.Columns.Add("Supplier4", typeof(System.Int64));
      dtDetail.Columns.Add("SupLeadtime4", typeof(System.Double));
      dtDetail.Columns.Add("StatusComp", typeof(System.String));
      dtDetail.Columns.Add("AddGroup", typeof(System.String));
      dtDetail.Columns.Add("CopyProcess", typeof(System.String));
      dtDetail.Columns.Add("FlagComp", typeof(System.Int32));
      dtDetail.Columns.Add("isRefer", typeof(System.Int32));
      ds.Tables.Add(dtDetail);

      //Table Component
      DataTable dtComp = new DataTable();
      dtComp.Columns.Add("PidComp", typeof(System.Int64));
      dtComp.Columns.Add("PidDetail", typeof(System.Int64));
      dtComp.Columns.Add("ComponentCode", typeof(System.String));
      dtComp.Columns.Add("CompRev", typeof(System.Int32));
      dtComp.Columns.Add("CompName", typeof(System.String));
      dtComp.Columns.Add("Qty", typeof(System.Int32));
      dtComp.Columns.Add("CompGroup", typeof(System.Int32));
      dtComp.Columns.Add("TotalQty", typeof(System.Int32));
      dtComp.Columns.Add("IsSave", typeof(System.Int32));
      ds.Tables.Add(dtComp);

      ds.Relations.Add(new DataRelation("dsDetail_dsComp", new DataColumn[1] { ds.Tables[0].Columns["PidDetail"] }, new DataColumn[1] { ds.Tables[1].Columns["PidDetail"] }, false));

      //Table Description
      DataTable dtDescr = new DataTable();
      dtDescr.Columns.Add("PidDescription", typeof(System.Int64));
      dtDescr.Columns.Add("DetailPid", typeof(System.Int64));
      dtDescr.Columns.Add("Priority", typeof(System.Int32));
      dtDescr.Columns.Add("ProcessCodePid", typeof(System.Int64));
      dtDescr.Columns.Add("ProcessNameEN", typeof(System.String));
      dtDescr.Columns.Add("Capacity", typeof(System.Double));
      dtDescr.Columns.Add("SetupTime", typeof(System.Double));
      dtDescr.Columns.Add("ProcessTime", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime1", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime2", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime3", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime4", typeof(System.Double));
      dtDescr.Columns.Add("Notation", typeof(System.String));
      dtDescr.Columns.Add("NonCalculate", typeof(System.Int32));
      dtDescr.Columns["NonCalculate"].DefaultValue = 0;
      dtDescr.Columns.Add("isRefer", typeof(System.Int32));
      ds.Tables.Add(dtDescr);

      ds.Relations.Add(new DataRelation("dsDetail_dsDescrip", new DataColumn[1] { ds.Tables[0].Columns["PidDetail"] }, new DataColumn[1] { ds.Tables[2].Columns["DetailPid"] }, false));

      return ds;
    }

    private void LoadItemReference()
    {
      string cm = string.Format(@"SELECT Pid, ItemCode, Revision, ItemCode + ' - ' + CAST(Revision AS VARCHAR) Display
                                  FROM TblPLNProcessCarcass_RoutingDefaultMaster
                                  WHERE IsConfirm > 0
	                                  AND IsDefault = 1");
      DataTable dtItemRefer = DataBaseAccess.SearchCommandTextDataTable(cm);
      ultItemReference.DataSource = dtItemRefer;
      ultItemReference.ValueMember = "Pid";
      ultItemReference.DisplayMember = "Display";
      ultItemReference.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultItemReference.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultItemReference.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
    }

    /// <summary>
    /// HÀM LOAD COMBOBOX LIST ITEM
    /// </summary>
    private void LoadItem()
    {
      string cmtext = @"SELECT DISTINCT B.RevisionActive, A.ItemCode,A.ItemCode + ' - '+ Name AS Name
                        FROM TblBOMItemInfo A 
                          LEFT JOIN TblBOMItemBasic B ON A.ItemCode = B.ItemCode 
                        ORDER BY A.ItemCode ASC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmtext);
      Utility.LoadUltraCombo(ultraCBItemCode, dt, "ItemCode", "Name", false, new string[] { "RevisionActive", "ItemCode" });
    }

    /// <summary>
    ///  HÀM LOAD COMBOBOX REVISION
    /// </summary>
    private void LoadRevision()
    {
      //List Revision Item
      string strCmdRevision = string.Format("Select Revision From TblBOMItemInfo Where ItemCode = '{0}' Order By Revision ASC", itemCode);
      CBRevision.DataSource = DataBaseAccess.SearchCommandTextDataTable(strCmdRevision);
      CBRevision.DisplayMember = "Revision";
      CBRevision.ValueMember = "Revision";
      try
      {
        string cmRevisionActive = string.Format("select RevisionActive from TblBOMItemBasic where ItemCode='{0}'", itemCode);
        CBRevision.SelectedValue = DBConvert.ParseInt(DataBaseAccess.SearchCommandTextDataTable(cmRevisionActive).Rows[0]["RevisionActive"].ToString());
        revision = DBConvert.ParseInt(CBRevision.Text);
      }
      catch
      {
      }
    }

    /// <summary>
    /// HÀM LOAD PART TYPE
    /// </summary>
    private void LoadPartType()
    {
      string cmText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [GROUP] = 1992";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultraPartType.DataSource = dt;
      ultraPartType.DisplayMember = "Value";
      ultraPartType.ValueMember = "Code";
      ultraPartType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraPartType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load Location Default
    /// </summary>
    private void LoadLocationDefault()
    {
      string cmText = @"SELECT WA.Pid, WA.StandByEN
                        FROM TblBOMCodeMaster CM 
                              INNER JOIN TblWIPWorkArea WA ON CM.Code = WA.Pid
                        WHERE [Group] = 81915
                        ORDER BY Sort";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultraDDLocation.DataSource = dt;
      ultraDDLocation.DisplayMember = "StandByEN";
      ultraDDLocation.ValueMember = "Pid";
      ultraDDLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDLocation.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load Supplier 1
    /// </summary>
    private void LoadSupplier()
    {
      string cmText = @"SELECT Pid Value, DefineCode + ' - ' + EnglishName Display
                        FROM TblPURSupplierInfo
                        WHERE DefineCode IS NOT NULL";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultddSup1.DataSource = dt;
      ultddSup1.DisplayMember = "Display";
      ultddSup1.ValueMember = "Value";
      ultddSup1.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddSup1.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }

    /// <summary>
    /// Hàm load partgroup
    /// </summary>
    private void LoadPartGroup()
    {
      string cmtext = "SELECT Pid,PartGroup FROM TblWIPCARParGroup";
      ultraCBPartGroup.DataSource = DataBaseAccess.SearchCommandTextDataTable(cmtext);
      ultraCBPartGroup.DisplayMember = "PartGroup";
      ultraCBPartGroup.ValueMember = "Pid";
      ultraCBPartGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBPartGroup.DisplayLayout.Bands[0].Columns["Pid"].MaxWidth = 30;
      ultraCBPartGroup.DisplayLayout.Bands[0].Columns["Pid"].MinWidth = 30;
    }

    /// <summary>
    /// HÀM LOAD PROCESS
    /// </summary>
    private void LoadDDProcess()
    {
      string commandText = @"
        SELECT DISTINCT PC.Pid , PC.ProcessCode, PC.ProcessNameEN, 0 AS Priority, PC.SetupTime,PC.ProcessTime,
          PC.Capacity, PC.LeadTime1, PC.LeadTime2, PC.LeadTime3, PC.LeadTime4, PC.Notation
        FROM TblPLNProcessCarcass_ProcessInfo PC";
      DataTable dtSourcePro = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDProcess.DataSource = dtSourcePro;
      ultDDProcess.DisplayMember = "ProcessCode";
      ultDDProcess.ValueMember = "Pid";
      ultDDProcess.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Priority"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["SetupTime"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessTime"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Capacity"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime1"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime2"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime3"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime4"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Notation"].Hidden = true;
    }

    /// <summary>
    /// HÀM LOAD PARTCODE THEO PARTGROUP
    /// </summary>
    /// <param name="PartGroupPid"></param>
    private void LoadListPartCode(long PartGroupPid)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PartGroupPid", DbType.Int64, PartGroupPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNLoadRoutingDefaultForItems", input);
      DataSet dsData = this.CreateDataSet();
      dsData.Tables[0].Merge(dsSource.Tables[0]);
      dsData.Tables[1].Merge(dsSource.Tables[1]);
      dsData.Tables[2].Merge(dsSource.Tables[2]);
      ultData.DataSource = dsData;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["Supplier1"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["Supplier2"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["Supplier3"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["Supplier4"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime1"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime2"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime3"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime4"].Activation = Activation.ActivateOnly;
      }

    }


    /// <summary>
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
    private void CheckProcessDuplicate()
    {
      isDuplicateProcess = false;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        for (int n = 0; n < ultData.Rows[k].ChildBands[1].Rows.Count; n++)
        {
          UltraGridRow rowcurent = ultData.Rows[k].ChildBands[1].Rows[n];
          rowcurent.CellAppearance.BackColor = Color.Empty;
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {

        for (int j = 0; j < ultData.Rows[i].ChildBands[1].Rows.Count; j++)
        {
          UltraGridRow rowcurentA = ultData.Rows[i].ChildBands[1].Rows[j];
          int priority = DBConvert.ParseInt(rowcurentA.Cells["Priority"].Value.ToString());
          for (int x = j + 1; x < ultData.Rows[i].ChildBands[1].Rows.Count; x++)
          {
            UltraGridRow rowcurentB = ultData.Rows[i].ChildBands[1].Rows[x];
            int priorityCom = DBConvert.ParseInt(rowcurentB.Cells["Priority"].Value.ToString());
            if (priority == priorityCom)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isDuplicateProcess = true;
            }
          }
        }
      }
    }
    /// <summary>
    /// HÀM CHECK COMPONENT DUPLICATE
    /// </summary>
    private void CheckCompDuplicate()
    {
      isDuplicateProcess = false;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        for (int n = 0; n < ultData.Rows[k].ChildBands[0].Rows.Count; n++)
        {
          UltraGridRow rowcurent = ultData.Rows[k].ChildBands[0].Rows[n];
          rowcurent.CellAppearance.BackColor = Color.Empty;
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["PartType"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].Cells["FlagComp"].Value.ToString()) != 0)
        {
          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowcurentA = ultData.Rows[i].ChildBands[0].Rows[j];
            string compCode = rowcurentA.Cells["ComponentCode"].Value.ToString();
            for (int x = j + 1; x < ultData.Rows[i].ChildBands[0].Rows.Count; x++)
            {
              UltraGridRow rowcurentB = ultData.Rows[i].ChildBands[0].Rows[x];
              string compCodeCom = rowcurentB.Cells["ComponentCode"].Value.ToString();
              if (string.Compare(compCode, compCodeCom) == 0)
              {
                rowcurentA.CellAppearance.BackColor = Color.Yellow;
                rowcurentB.CellAppearance.BackColor = Color.Yellow;
                isDuplicateProcess = true;
              }
            }
          }
        }
      }
    }
    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      //Check lưới cha
      if (ultData.Rows.Count == 0)
      {
        message = "No data";
        return false;
      }
      // Check Item Code
      if (ultraCBItemCode.Text.Trim().Length == 0 || ultraCBItemCode.Value == null)
      {
        message = "ItemCode";
        return false;
      }

      // Check Revision
      if (CBRevision.Text.Trim().Length == 0 || CBRevision.SelectedIndex < 0)
      {
        message = "Revision";
        return false;
      }
      // Check Part Group
      if (ultraCBPartGroup.Text.Trim().Length == 0 || ultraCBPartGroup.Value == null)
      {
        message = "Part Group";
        return false;
      }
      // Check Total Percent = 100
      if (DBConvert.ParseInt(txtTotalPercent.Text.Trim()) != 100)
      {
        message = "Total Percent";
        return false;
      }

      // Check Detail gridview
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        long lcdefault = DBConvert.ParseLong(rowParent.Cells["LocationDefault"].Value.ToString());
        long sup1 = DBConvert.ParseLong(rowParent.Cells["Supplier1"].Value.ToString());
        long sup2 = DBConvert.ParseLong(rowParent.Cells["Supplier2"].Value.ToString());
        long sup3 = DBConvert.ParseLong(rowParent.Cells["Supplier3"].Value.ToString());
        long sup4 = DBConvert.ParseLong(rowParent.Cells["Supplier4"].Value.ToString());
        string supp1 = string.Format(@"SELECT Pid Value, EnglishName Display
                                        FROM TblPURSupplierInfo
                                        WHERE DefineCode IS NOT NULL AND Pid = {0}", sup1);
        DataTable dtsup1 = DataBaseAccess.SearchCommandTextDataTable(supp1);

        string supp2 = string.Format(@"SELECT Pid Value, EnglishName Display
                                        FROM TblPURSupplierInfo
                                        WHERE DefineCode IS NOT NULL AND Pid = {0}", sup2);
        DataTable dtsup2 = DataBaseAccess.SearchCommandTextDataTable(supp2);

        string supp3 = string.Format(@"SELECT Pid Value, EnglishName Display
                                        FROM TblPURSupplierInfo
                                        WHERE DefineCode IS NOT NULL AND Pid = {0}", sup3);
        DataTable dtsup3 = DataBaseAccess.SearchCommandTextDataTable(supp3);

        string supp4 = string.Format(@"SELECT Pid Value, EnglishName Display
                                        FROM TblPURSupplierInfo
                                        WHERE DefineCode IS NOT NULL AND Pid = {0}", sup4);
        DataTable dtsup4 = DataBaseAccess.SearchCommandTextDataTable(supp4);

        if (lcdefault == 39 && rowParent.Cells["Supplier1"].Text.Length > 0 && dtsup1.Rows.Count == 0)
        {
          message = "Supplier 1";
          return false;
        }
        if (lcdefault == 39 && rowParent.Cells["Supplier2"].Text.Length > 0 && dtsup2.Rows.Count == 0)
        {
          message = "Supplier 2";
          return false;
        }

        if (lcdefault == 39 && rowParent.Cells["Supplier3"].Text.Length > 0 && dtsup3.Rows.Count == 0)
        {
          message = "Supplier 3";
          return false;
        }
        if (lcdefault == 39 && rowParent.Cells["Supplier4"].Text.Length > 0 && dtsup4.Rows.Count == 0)
        {
          message = "Supplier 4";
          return false;
        }

        if (lcdefault == 39 &&
          (
          (rowParent.Cells["Supplier1"].Text == rowParent.Cells["Supplier2"].Text && rowParent.Cells["Supplier1"].Text.Length > 0 && rowParent.Cells["Supplier2"].Text.Length > 0) ||
          (rowParent.Cells["Supplier1"].Text == rowParent.Cells["Supplier3"].Text && rowParent.Cells["Supplier1"].Text.Length > 0 && rowParent.Cells["Supplier3"].Text.Length > 0) ||
          (rowParent.Cells["Supplier1"].Text == rowParent.Cells["Supplier4"].Text && rowParent.Cells["Supplier1"].Text.Length > 0 && rowParent.Cells["Supplier4"].Text.Length > 0) ||
          (rowParent.Cells["Supplier2"].Text == rowParent.Cells["Supplier3"].Text && rowParent.Cells["Supplier2"].Text.Length > 0 && rowParent.Cells["Supplier3"].Text.Length > 0) ||
          (rowParent.Cells["Supplier2"].Text == rowParent.Cells["Supplier4"].Text && rowParent.Cells["Supplier2"].Text.Length > 0 && rowParent.Cells["Supplier4"].Text.Length > 0) ||
          (rowParent.Cells["Supplier3"].Text == rowParent.Cells["Supplier4"].Text && rowParent.Cells["Supplier3"].Text.Length > 0 && rowParent.Cells["Supplier4"].Text.Length > 0)
          ))
        {
          message = "Supplier";
          return false;
        }

        if (DBConvert.ParseInt(rowParent.Cells["PartPercent"].Value.ToString()) < 0)
        {
          message = "Part Percent";
          return false;
        }
        if (rowParent.Cells["LocationDefault"].Value.ToString().Length == 0)
        {
          message = "Location";
          return false;
        }
        if (bTec == true || bConfirm == true)
        {
          for (int j = 0; j < rowParent.ChildBands[1].Rows.Count; j++)
          {
            UltraGridRow row = rowParent.ChildBands[1].Rows[j];
            // SetupTime > 0
            if (DBConvert.ParseDouble(row.Cells["SetupTime"].Value.ToString()) != double.MinValue && DBConvert.ParseDouble(row.Cells["SetupTime"].Value.ToString()) <= 0)
            {
              message = "SetupTime";
              return false;
            }
            // ProcessTime > 0
            if (DBConvert.ParseDouble(row.Cells["ProcessTime"].Value.ToString()) != double.MinValue && DBConvert.ParseDouble(row.Cells["ProcessTime"].Value.ToString()) <= 0)
            {
              message = "ProcessTime";
              return false;
            }
            // LeadTime1 > 0
            if (DBConvert.ParseDouble(row.Cells["LeadTime1"].Value.ToString()) != double.MinValue && DBConvert.ParseDouble(row.Cells["LeadTime1"].Value.ToString()) <= 0)
            {
              message = "LeadTime1";
              return false;
            }
            // LeadTime2 > 0
            if (DBConvert.ParseDouble(row.Cells["LeadTime2"].Value.ToString()) != double.MinValue && DBConvert.ParseDouble(row.Cells["LeadTime2"].Value.ToString()) <= 0)
            {
              message = "LeadTime2";
              return false;
            }
            // LeadTime3 > 0
            if (DBConvert.ParseDouble(row.Cells["LeadTime3"].Value.ToString()) != double.MinValue && DBConvert.ParseDouble(row.Cells["LeadTime3"].Value.ToString()) <= 0)
            {
              message = "LeadTime3";
              return false;
            }
            // LeadTime4 > 0
            if (DBConvert.ParseDouble(row.Cells["LeadTime4"].Value.ToString()) != double.MinValue && DBConvert.ParseDouble(row.Cells["LeadTime4"].Value.ToString()) <= 0)
            {
              message = "LeadTime4";
              return false;
            }
          }

          for (int h = 0; h < rowParent.ChildBands[0].Rows.Count; h++)
          {
            UltraGridRow row1 = rowParent.ChildBands[0].Rows[h];
            if (DBConvert.ParseInt(row1.Cells["CompGroup"].Value.ToString()) == 1)
            {
              if (DBConvert.ParseInt(row1.Cells["Qty"].Value.ToString()) <= 0 || DBConvert.ParseInt(row1.Cells["Qty"].Value.ToString()) > DBConvert.ParseInt(row1.Cells["TotalQty"].Value.ToString()))
              {
                message = "Qty";
                return false;
              }
            }
          }
        }
      }
      DataTable dt1 = ((DataSet)ultData.DataSource).Tables[1];
      for (int l = 0; l < dt1.Rows.Count; l++)
      {
        int qty = 0;
        int qtystock = 0;
        string compcode = string.Empty;

        if (dt1.Rows[l].RowState != DataRowState.Deleted)
        {
          compcode = dt1.Rows[l]["ComponentCode"].ToString();
          int comre = DBConvert.ParseInt(dt1.Rows[l]["CompRev"].ToString());
          DataRow[] comp = dt1.Select(string.Format("CompGroup = 1 and ComponentCode = '{0}' and CompRev = {1}", compcode, comre));
          if (comp.Length > 1)
          {
            qtystock = DBConvert.ParseInt(comp[0]["TotalQty"].ToString());
            for (int i = 0; i < comp.Length; i++)
            {
              qty += DBConvert.ParseInt(comp[i]["Qty"].ToString());
            }
            if (qty > qtystock)
            {
              if (qty > qtystock)
              {
                message = string.Format("Component {0}: Qty > Total Qty", compcode);
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// SAVE MASTER
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      bool result = true;

      //delete stucture if copied
      if (pidItem != long.MinValue && this.isReference == true)
      {
        DBParameter[] inp = new DBParameter[1];
        inp[0] = new DBParameter("@Pid", DbType.Int64, pidItem);
        DBParameter[] otp = new DBParameter[1];
        otp[0] = new DBParameter("@Result", DbType.Int32, 0);
        DataBaseAccess.ExecuteStoreProcedure("spPLNDeleteStuctureItemPart", inp, otp);
      }
      DBParameter[] input = new DBParameter[7];
      if (pidItem != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, pidItem);
      }
      if (ultraCBItemCode.Value.ToString().Length > 0)
      {
        input[1] = new DBParameter("@ItemCode", DbType.String, ultraCBItemCode.Value.ToString());
      }
      input[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(CBRevision.SelectedValue.ToString()));
      input[3] = new DBParameter("@PartGroupPid", DbType.Int64, DBConvert.ParseLong(ultraCBPartGroup.Value.ToString()));
      input[4] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
      input[5] = new DBParameter("@IsDefault", DbType.Int32, this.isDefault);
      input[6] = new DBParameter("@IsConfirm", DbType.Int32, this.isConfirm);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNRoutingMaster_Edit", input, output);

      if ((output == null) || DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
      {
        //Nếu insert/update có lỗi thì trả về false
        result = false;
      }

      else
      {
        //Nếu insert/update thành công thì trả về Pid Item
        this.pidItem = DBConvert.ParseLong(output[0].Value.ToString());
        result = true;
        //Sau đó gọi hàm save Routing Detail với giá trị truyền vào là Pid Item
        result = SaveDetail(pidItem);
      }
      return result;
    }

    /// <summary>
    /// SAVE DETAIL
    /// </summary>
    /// <param name="MasterPid"></param>
    /// <returns></returns>
    private bool SaveDetail(long MasterPid)
    {
      bool result = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];

        DBParameter[] input = new DBParameter[12];

        if (DBConvert.ParseLong(rowParent.Cells["PidDetail"].Value.ToString()) > 0 && DBConvert.ParseInt(rowParent.Cells["isRefer"].Value.ToString()) != 1)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["PidDetail"].Value.ToString()));
        }

        if (MasterPid != long.MinValue)
        {
          input[1] = new DBParameter("@MasterPid", DbType.Int64, MasterPid);
        }

        if (DBConvert.ParseLong(rowParent.Cells["PartCodePid"].Value.ToString()) != long.MinValue)
        {
          input[2] = new DBParameter("@PartCodePid", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["PartCodePid"].Value.ToString()));
        }

        if (DBConvert.ParseDouble(rowParent.Cells["PartPercent"].Value.ToString()) >= 0)
        {
          input[3] = new DBParameter("@PartPercent", DbType.Double, DBConvert.ParseDouble(rowParent.Cells["PartPercent"].Value.ToString()));
        }

        if (DBConvert.ParseInt(rowParent.Cells["PartType"].Value.ToString()) != int.MinValue)
        {
          input[4] = new DBParameter("@PartType", DbType.Int32, DBConvert.ParseInt(rowParent.Cells["PartType"].Value.ToString()));
        }

        if (DBConvert.ParseLong(rowParent.Cells["LocationDefault"].Value.ToString()) != long.MinValue)
        {
          input[5] = new DBParameter("@LocationDefault", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["LocationDefault"].Value.ToString()));
        }

        if (DBConvert.ParseLong(rowParent.Cells["Supplier1"].Value.ToString()) != long.MinValue)
        {
          input[6] = new DBParameter("@Sup1", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["Supplier1"].Value.ToString()));
        }
        if (DBConvert.ParseLong(rowParent.Cells["Supplier2"].Value.ToString()) != long.MinValue)
        {
          input[7] = new DBParameter("@Sup2", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["Supplier2"].Value.ToString()));
        }
        if (DBConvert.ParseLong(rowParent.Cells["Supplier3"].Value.ToString()) != long.MinValue)
        {
          input[8] = new DBParameter("@Sup3", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["Supplier3"].Value.ToString()));
        }
        if (DBConvert.ParseLong(rowParent.Cells["Supplier4"].Value.ToString()) != long.MinValue)
        {
          input[9] = new DBParameter("@Sup4", DbType.Int64, DBConvert.ParseLong(rowParent.Cells["Supplier4"].Value.ToString()));
        }
        input[10] = new DBParameter("@PartName", DbType.String, rowParent.Cells["PartName"].Value.ToString());
        input[11] = new DBParameter("@PartCode", DbType.String, rowParent.Cells["PartCode"].Value.ToString());
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, 0);
        DataBaseAccess.ExecuteStoreProcedure("spPLNRoutingDetail_Edit", input, output);
        if ((output == null) || DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
        {
          //Nếu insert/update có lỗi thì trả về false
          result = false;
        }
        else
        {
          //Nếu insert/update thành công thì trả về Pid Detail
          this.pidDetail = DBConvert.ParseLong(output[0].Value.ToString());

          //delete
          //Nếu có dòng bị delete trên lưới thì mới delete
          if (this.listDeletedPid.Count > 0)
          {
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            for (int k = 0; k < listDeletedPid.Count; k++)
            {
              DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[k]) };
              DataBaseAccess.ExecuteStoreProcedure("spPLNDeleteProcessInPart", deleteParam, outputParam);
              if (DBConvert.ParseLong(outputParam[0].Value) <= 0)
              {
                result = false;
              }
            }
          }

          if (this.listDeleteComp.Count > 0)
          {
            DBParameter[] outputParam1 = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            for (int k = 0; k < listDeleteComp.Count; k++)
            {
              DBParameter[] deleteParam1 = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeleteComp[k]) };
              DataBaseAccess.ExecuteStoreProcedure("spPLNDeleteComponentInPart", deleteParam1, outputParam1);
              if (DBConvert.ParseLong(outputParam1[0].Value) <= 0)
              {
                result = false;
              }
            }
          }
          for (int k = 0; k < ultData.Rows[i].ChildBands[0].Rows.Count; k++)
          {
            //if (DBConvert.ParseInt(ultData.Rows[i].Cells["PartType"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].Cells["FlagComp"].Value.ToString()) != 0)
            //{
            if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[k].Cells["IsSave"].Value.ToString()) == 1)
            {
              DBParameter[] inputComp = new DBParameter[6];
              inputComp[1] = new DBParameter("@DetailPid", DbType.Int64, this.pidDetail);
              if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[k].Cells["PidComp"].Value.ToString()) != long.MinValue)
              {
                inputComp[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[k].Cells["PidComp"].Value.ToString()));
              }
              if (ultData.Rows[i].ChildBands[0].Rows[k].Cells["ComponentCode"].Value.ToString().Length > 0)
              {
                inputComp[2] = new DBParameter("@ComponentCode", DbType.String, ultData.Rows[i].ChildBands[0].Rows[k].Cells["ComponentCode"].Value.ToString());
              }
              if (ultData.Rows[i].ChildBands[0].Rows[k].Cells["CompRev"].Value.ToString().Length > 0)
              {
                inputComp[3] = new DBParameter("@CompRevision", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[k].Cells["CompRev"].Value.ToString()));
              }
              if (ultData.Rows[i].ChildBands[0].Rows[k].Cells["Qty"].Value.ToString().Length > 0)
              {
                inputComp[4] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[k].Cells["Qty"].Value.ToString()));
              }
              inputComp[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              DBParameter[] outputComp = new DBParameter[1];
              outputComp[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure("spPLNRoutingComponentForCarcassProcess_Edit", inputComp, outputComp);
              long rsc = DBConvert.ParseLong(outputComp[0].Value.ToString());
              if (rsc <= 0)
              {
                result = false;
              }
            }
            //}
          }

          for (int j = 0; j < ultData.Rows[i].ChildBands[1].Rows.Count; j++)
          {
            DBParameter[] inputChild = new DBParameter[13];
            inputChild[1] = new DBParameter("@DetailPid", DbType.Int64, this.pidDetail);
            if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["PidDescription"].Value.ToString()) != long.MinValue && DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["isRefer"].Value.ToString()) != 1)
            {
              inputChild[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["PidDescription"].Value.ToString()));
            }

            if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Priority"].Value.ToString()) != int.MinValue)
            {
              inputChild[2] = new DBParameter("@Priority", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Priority"].Value.ToString()));
            }

            if (ultData.Rows[i].ChildBands[1].Rows[j].Cells["ProcessCodePid"].Value != DBNull.Value)
            {
              inputChild[3] = new DBParameter("@ProcessCodePid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["ProcessCodePid"].Value.ToString()));
            }

            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Capacity"].Value.ToString()) != double.MinValue)
            {
              inputChild[4] = new DBParameter("@Capacity", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Capacity"].Value.ToString()));
            }

            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["SetupTime"].Value.ToString()) != double.MinValue)
            {
              inputChild[5] = new DBParameter("@SetupTime", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["SetupTime"].Value.ToString()));
            }

            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["ProcessTime"].Value.ToString()) != double.MinValue)
            {
              inputChild[6] = new DBParameter("@ProcessTime", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["ProcessTime"].Value.ToString()));
            }

            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime1"].Value.ToString()) != double.MinValue)
            {
              inputChild[7] = new DBParameter("@LeadTime1", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime1"].Value.ToString()));
            }

            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime2"].Value.ToString()) != double.MinValue)
            {
              inputChild[8] = new DBParameter("@LeadTime2", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime2"].Value.ToString()));
            }

            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime3"].Value.ToString()) != double.MinValue)
            {
              inputChild[9] = new DBParameter("@LeadTime3", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime3"].Value.ToString()));
            }

            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime4"].Value.ToString()) != double.MinValue)
            {
              inputChild[10] = new DBParameter("@LeadTime4", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[1].Rows[j].Cells["LeadTime4"].Value.ToString()));
            }

            if (ultData.Rows[i].ChildBands[1].Rows[j].Cells["Notation"].Value.ToString().Length > 0)
            {
              inputChild[11] = new DBParameter("@Notation", DbType.String, ultData.Rows[i].ChildBands[1].Rows[j].Cells["Notation"].Value.ToString());
            }
            if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[1].Rows[j].Cells["NonCalculate"].Value.ToString()) != int.MinValue)
            {
              inputChild[12] = new DBParameter("@NonCalculate", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[1].Rows[j].Cells["NonCalculate"].Value.ToString()));
            }
            DBParameter[] outputChild = new DBParameter[1];
            outputChild[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPLNRoutingDescription_Edit", inputChild, outputChild);
            long rs = DBConvert.ParseLong(outputChild[0].Value.ToString());
            if (rs <= 0)
            {
              result = false;
            }
          }
        }
      }
      return result;
    }

    /// <summary>
    /// SAVE DATA
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool TotalResult = true;
      if (SaveMaster())
      {
        TotalResult = true;
      }
      else
      {
        TotalResult = false;
      }
      return TotalResult;
    }

    /// <summary>
    /// READ ONLY ACCESS
    /// </summary>
    private void ReadOnlyAccess()
    {
      if (isConfirm > 0)
      {
        ChkDefault.Enabled = false;
        ultItemReference.Enabled = false;
        btnCopy.Enabled = false;
      }
      //PLN
      if (bNew == true && bTec == false && bConfirm == false)
      {
        if (isConfirm > 0)
        {
          chkConfirm.Checked = true;
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
      }

      //TEC
      if (bTec == true && bNew == false && bConfirm == false)
      {
        if (isConfirm >= 2)
        {
          chkConfirm.Checked = true;
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
        else if (isConfirm == 1)
        {
          btnSave.Enabled = true;
        }
        else
        {
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
      }

      //CAR
      if (bConfirm == true && bNew == false && bTec == false)
      {
        if (isConfirm == 3)
        {
          chkConfirm.Checked = true;
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
        else if (isConfirm == 2)
        {
          btnSave.Enabled = true;
        }
        else
        {
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
      }

      //ADMIN
      if (bConfirm == true && bNew == true && bTec == true)
      {
        if (isConfirm == 3)
        {
          chkConfirm.Checked = true;
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
        else
        {
          ChkDefault.Enabled = true;
          ultItemReference.Enabled = true;
          btnCopy.Enabled = true;
        }
      }
    }

    /// <summary>
    /// HÀM LOAD DATA KHI DOUBLE CLICK ITEM TỪ DANH SÁCH
    /// </summary>
    /// <param name="masterpid"></param>
    private void LoadDataFromList(long masterpid)
    {
      DBParameter[] input = new DBParameter[4];
      input[0] = new DBParameter("@MasterPid", DbType.Int64, masterpid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNRoutingDefaultMaster_Select", 600, input);
      DataSet dsData = this.CreateDataSet();
      dsData.Tables[0].Merge(dsSource.Tables[1]);
      dsData.Tables[1].Merge(dsSource.Tables[2]);
      dsData.Tables[2].Merge(dsSource.Tables[3]);

      this.location = DBConvert.ParseInt(dsSource.Tables[1].Rows[0]["LocationDefault"].ToString());
      ultraCBItemCode.Value = dsSource.Tables[0].Rows[0]["ItemCode"].ToString();
      CBRevision.SelectedValue = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["Revision"].ToString());
      ultraCBPartGroup.Value = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["PartGroupPid"].ToString());
      isConfirm = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["IsConfirm"].ToString());
      flagConfirm = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["IsConfirm"].ToString());
      isDefault = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["IsDefault"].ToString());

      if (isDefault == 1)
      {
        ChkDefault.Checked = true;
      }
      else
      {
        ChkDefault.Checked = false;
      }

      this.ReadOnlyAccess();

      ultData.DataSource = dsData;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["Supplier1"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["Supplier2"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["Supplier3"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["Supplier4"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime1"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime2"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime3"].Activation = Activation.ActivateOnly;
        ultData.Rows[i].Cells["SupLeadtime4"].Activation = Activation.ActivateOnly;

        if (ultData.Rows[i].ChildBands[0].Rows.Count > 0)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[0].Cells["IsSave"].Value.ToString()) == 0)
          {
            ultData.Rows[i].Cells["StatusComp"].Value = "Link from BOM";
            ultData.Rows[i].Cells["StatusComp"].Appearance.BackColor = Color.LightGreen;
            ultData.Rows[i].Cells["FlagComp"].Value = 0;
            for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              ultData.Rows[i].ChildBands[0].Rows[j].Activation = Activation.ActivateOnly;
            }
          }
          else
          {
            ultData.Rows[i].Cells["StatusComp"].Value = "Allocate Manual";
            ultData.Rows[i].Cells["StatusComp"].Appearance.BackColor = Color.LightCyan;
            ultData.Rows[i].Cells["FlagComp"].Value = 1;
          }

        }
      }

      totalPartPercent = 0;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        totalPartPercent += DBConvert.ParseInt(ultData.Rows[i].Cells["PartPercent"].Value.ToString());
      }
      txtTotalPercent.Text = totalPartPercent.ToString();

      //Không cho chỉnh sửa thông tin item

      ultraCBItemCode.ReadOnly = true;
      ultraCBPartGroup.ReadOnly = true;
      //CBRevision.Enabled = false;

      this.isReference = false;
    }

    private void CopyItemReference(long pidItem)
    {
      DBParameter[] input = new DBParameter[4];
      input[0] = new DBParameter("@MasterPid", DbType.Int64, pidItem);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNRoutingDefaultMaster_Select", input);
      DataSet dsData = this.CreateDataSet();
      dsData.Tables[0].Merge(dsSource.Tables[1]);
      //dsData.Tables[1].Merge(dsSource.Tables[2]);
      dsData.Tables[2].Merge(dsSource.Tables[3]);
      ultraCBPartGroup.Value = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["PartGroupPid"].ToString());
      ultData.DataSource = dsData;
      totalPartPercent = 0;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        totalPartPercent += DBConvert.ParseInt(ultData.Rows[i].Cells["PartPercent"].Value.ToString());
        ultData.Rows[i].Cells["isRefer"].Value = 1;
        for (int j = 0; j < ultData.Rows[i].ChildBands[1].Rows.Count; j++)
        {
          ultData.Rows[i].ChildBands[1].Rows[j].Cells["isRefer"].Value = 1;
        }
      }
      txtTotalPercent.Text = totalPartPercent.ToString();
      this.isReference = true;
    }
    private UltraDropDown LoadComponentHW(string item, int rev, int compgroup, int type, UltraDropDown ultComp)
    {
      if (ultComp == null)
      {
        ultComp = new UltraDropDown();
        this.Controls.Add(ultComp);
      }
      DBParameter[] input = new DBParameter[4];
      input[0] = new DBParameter("@ItemCode", DbType.String, item);
      input[1] = new DBParameter("@Revision", DbType.Int32, rev);
      input[2] = new DBParameter("@CompGroup", DbType.Int32, compgroup);
      input[3] = new DBParameter("@Type", DbType.Int32, type);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNLoadComponentForPart", input);
      if (dt != null && dt.Rows.Count > 0)
      {
        ultComp.DataSource = dt;
        ultComp.DisplayMember = "ComponentCode";
        ultComp.ValueMember = "ComponentCode";
        //ultComp.DisplayLayout.Bands[0].Columns["CompNameEN"].Hidden = true;
        ultComp.DisplayLayout.Bands[0].Columns["Qty"].Hidden = true;
        ultComp.DisplayLayout.Bands[0].Columns["CompGroup"].Hidden = true;
        ultComp.DisplayLayout.Bands[0].Columns["TotalQty"].Hidden = true;
        ultComp.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultComp.Visible = false;
        ultComp.DisplayLayout.AutoFitColumns = true;
        ultComp.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      }
      return ultComp;
    }

    #endregion

    #region Events

    /// <summary>
    /// SỰ KIỆN KHI CHỌN ITEM
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraCBItemCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultraCBItemCode.SelectedRow != null)
      {
        itemCode = ultraCBItemCode.Value.ToString();
        LoadRevision();
        PicItem.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
        ultraCBPartGroup.ReadOnly = false;
        CBRevision.Enabled = true;
      }
    }

    /// <summary>
    /// SỰ KIỆN KHI CHỌN REVISION
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CBRevision_SelectedIndexChanged(object sender, EventArgs e)
    {
      revision = DBConvert.ParseInt(CBRevision.Text.ToString());
      PicItem.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
    }

    /// <summary>
    /// SỰ KIỆN KHI CHỌN PART GROUP
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraCBPartGroup_ValueChanged(object sender, EventArgs e)
    {
      //Just only effect when create new
      if (this.pidItem == long.MinValue)
      {
        LoadListPartCode(DBConvert.ParseLong(ultraCBPartGroup.Value.ToString()));
      }
    }



    /// <summary>
    /// SỰ KIỆN FORMAT LƯỚI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      //e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      //e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      if (bNew == true && bTec == false)
      {
        e.Layout.Bands[0].Columns["AddGroup"].Hidden = true;
        e.Layout.Bands[0].Columns["CopyProcess"].Hidden = true;
        e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      }
      else
      {
        e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      }
      //Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        if ((bTec == true && bNew == false) || (bConfirm == true && bNew == false))
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
          e.Layout.Bands[0].Columns["PartName"].CellActivation = Activation.AllowEdit;
        }
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      for (int i = 0; i < e.Layout.Bands[2].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[2].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[2].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["PartType"].ValueList = ultraPartType;
      e.Layout.Bands[0].Columns["PartType"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["LocationDefault"].ValueList = ultraDDLocation;
      e.Layout.Bands[0].Columns["Supplier1"].ValueList = ultddSup1;
      e.Layout.Bands[0].Columns["Supplier2"].ValueList = ultddSup1;
      e.Layout.Bands[0].Columns["Supplier3"].ValueList = ultddSup1;
      e.Layout.Bands[0].Columns["Supplier4"].ValueList = ultddSup1;
      e.Layout.Bands[0].Columns["PartCode"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["PartName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Supplier1"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Supplier2"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Supplier3"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Supplier4"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["SupLeadtime1"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SupLeadtime2"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SupLeadtime3"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SupLeadtime4"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["PidDetail"].Hidden = true;
      e.Layout.Bands[0].Columns["isRefer"].Hidden = true;
      e.Layout.Bands[0].Columns["PartCodePid"].Hidden = true;
      e.Layout.Bands[0].Columns["MasterPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagComp"].Hidden = true;

      e.Layout.Bands[0].Columns["AddGroup"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
      e.Layout.Bands[0].Columns["AddGroup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
      e.Layout.Bands[0].Columns["CopyProcess"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
      e.Layout.Bands[0].Columns["CopyProcess"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
      e.Layout.Bands[0].Columns["LocationDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["PartType"].Header.Caption = "Part Type";
      e.Layout.Bands[0].Columns["PartPercent"].Header.Caption = "Part Percent";
      e.Layout.Bands[0].Columns["LocationDefault"].Header.Caption = "Location\nDefault";
      e.Layout.Bands[0].Columns["PartName"].Header.Caption = "Part Name";
      e.Layout.Bands[0].Columns["PartCode"].Header.Caption = "Part Code";

      e.Layout.Bands[1].Columns["PidComp"].Hidden = true;
      e.Layout.Bands[1].Columns["PidDetail"].Hidden = true;
      e.Layout.Bands[1].Columns["CompGroup"].Hidden = true;
      e.Layout.Bands[1].Columns["IsSave"].Hidden = true;
      e.Layout.Bands[1].Columns["CompRev"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["CompName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["TotalQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["TotalQty"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[1].Columns["CompName"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[1].Columns["CompRev"].CellAppearance.BackColor = Color.LightGray;



      e.Layout.Bands[2].Columns["PidDescription"].Hidden = true;
      e.Layout.Bands[2].Columns["DetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["isRefer"].Hidden = true;
      e.Layout.Bands[2].Columns["ProcessCodePid"].ValueList = ultDDProcess;
      e.Layout.Bands[2].Columns["Capacity"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["Notation"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["Capacity"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[2].Columns["Notation"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[2].Columns["ProcessNameEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["ProcessNameEN"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[2].Columns["NonCalculate"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[2].ColHeaderLines = 2;
      e.Layout.Bands[2].Columns["LeadTime1"].Header.Caption = "Lead Time\n<= 2pcs";
      e.Layout.Bands[2].Columns["LeadTime2"].Header.Caption = "Lead Time\n<= 6pcs";
      e.Layout.Bands[2].Columns["LeadTime3"].Header.Caption = "Lead Time\n<= 12pcs";
      e.Layout.Bands[2].Columns["LeadTime4"].Header.Caption = "Lead Time\n> 12pcs";
      e.Layout.Bands[2].Columns["ProcessCodePid"].Header.Caption = "Process\nCode";
      e.Layout.Bands[2].Columns["ProcessNameEN"].Header.Caption = "Process\nName";
      e.Layout.Bands[2].Columns["ProcessTime"].Header.Caption = "Process\nTime";
      e.Layout.Bands[2].Columns["SetupTime"].Header.Caption = "Setup\nTime";
      e.Layout.Bands[2].Columns["NonCalculate"].Header.Caption = "Non\nCalculate";
      e.Layout.Bands[2].Columns["ProcessCodePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      //Set Width
      e.Layout.Bands[0].Columns["LocationDefault"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LocationDefault"].MinWidth = 70;
      e.Layout.Bands[0].Columns["PartType"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["PartType"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Supplier1"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Supplier1"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Supplier2"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Supplier2"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Supplier3"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Supplier3"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Supplier4"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Supplier4"].MinWidth = 100;
      e.Layout.Bands[0].Columns["PartPercent"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["PartPercent"].MinWidth = 70;

      e.Layout.Bands[2].Columns["Priority"].MaxWidth = 50;
      e.Layout.Bands[2].Columns["Priority"].MinWidth = 50;
      e.Layout.Bands[2].Columns["ProcessCodePid"].MaxWidth = 100;
      e.Layout.Bands[2].Columns["ProcessCodePid"].MinWidth = 100;
      e.Layout.Bands[2].Columns["Capacity"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["Capacity"].MinWidth = 70;
      e.Layout.Bands[2].Columns["SetupTime"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["SetupTime"].MinWidth = 70;
      e.Layout.Bands[2].Columns["ProcessTime"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["ProcessTime"].MinWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime1"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime1"].MinWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime2"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime2"].MinWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime3"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime3"].MinWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime4"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["LeadTime4"].MinWidth = 70;
      e.Layout.Bands[2].Columns["Notation"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["Notation"].MinWidth = 70;
      e.Layout.Bands[2].Columns["NonCalculate"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["NonCalculate"].MinWidth = 70;

    }

    /// <summary>
    /// SỰ KIỆN FORMAT DÒNG TRÊN LƯỚI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeRow(object sender, InitializeRowEventArgs e)
    {
      try
      {
        if (e.ReInitialize == false)

        {
          e.Row.Cells["AddGroup"].Value = "Add Group";
          e.Row.Cells["AddGroup"].ButtonAppearance.ForeColor = Color.Blue;
          e.Row.Cells["CopyProcess"].Value = "Copy Process";
          e.Row.Cells["CopyProcess"].ButtonAppearance.ForeColor = Color.Blue;
        }
      }
      catch
      {
      }
    }

    /// <summary>
    /// SỰ KIỆN CLICK BUTTON TRÊN LƯỚI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_ClickCellButton(object sender, CellEventArgs e)
    {
      string value = e.Cell.Value.ToString();
      if (value == "Add Group")
      {
        DataSet ds = (DataSet)ultData.DataSource;
        viewWIP_96_003 uc = new viewWIP_96_003();
        uc.RuleShow = 1;
        uc.Index = DBConvert.ParseInt(e.Cell.Row.Index.ToString()) + 1;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "ADD GROUP TO PART CODE DESCRIPTION", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
        UltraGridRow rowparent = e.Cell.Row;

        if (DBConvert.ParseLong(e.Cell.Row.Cells["PidDetail"].Value.ToString()) == long.MinValue)
        {
          e.Cell.Row.Cells["PidDetail"].Value = pidtemp;
          pidtemp++;
        }

        for (int i = 0; i < uc.dtNewSource.Rows.Count; i++)
        {
          rowparent.ChildBands[1].Band.AddNew();
          int rowindex = DBConvert.ParseInt(rowparent.ChildBands[1].Rows.Count) - 1;
          rowparent.ChildBands[1].Rows[rowindex].Cells["ProcessCodePid"].Value = DBConvert.ParseLong(uc.dtNewSource.Rows[i]["ProcessCodePid"].ToString());
          rowparent.ChildBands[1].Rows[rowindex].Cells["Priority"].Value = rowindex + 1;
          rowparent.ChildBands[1].Rows[rowindex].Cells["ProcessNameEN"].Value = uc.dtNewSource.Rows[i]["ProcessNameEN"].ToString();
          if (DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["Capacity"].ToString()) != double.MinValue)
          {
            rowparent.ChildBands[1].Rows[rowindex].Cells["Capacity"].Value = DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["Capacity"].ToString());
          }
          if (DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["SetupTime"].ToString()) != double.MinValue)
          {
            rowparent.ChildBands[1].Rows[rowindex].Cells["SetupTime"].Value = DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["SetupTime"].ToString());
          }
          rowparent.ChildBands[1].Rows[rowindex].Cells["ProcessTime"].Value = DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["ProcessTime"].ToString());
          rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime1"].Value = DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["LeadTime1"].ToString());
          rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime2"].Value = DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["LeadTime2"].ToString());
          rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime3"].Value = DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["LeadTime3"].ToString());
          rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime4"].Value = DBConvert.ParseDouble(uc.dtNewSource.Rows[i]["LeadTime4"].ToString());
          rowparent.ChildBands[1].Rows[rowindex].Cells["Notation"].Value = uc.dtNewSource.Rows[i]["Notation"].ToString();
        }
        this.CheckProcessDuplicate();
      }
      else
      {
        int partType = DBConvert.ParseInt(e.Cell.Row.Cells["PartType"].Value.ToString());
        if (partType > 0)
        {
          viewWIP_96_008 uc1 = new viewWIP_96_008();
          uc1.partType = partType;
          DaiCo.Shared.Utility.WindowUtinity.ShowView(uc1, "COPY PROCESS FROM ITEM", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
          UltraGridRow rowparent = e.Cell.Row;
          for (int i = 0; i < uc1.dtNewSource.Rows.Count; i++)
          {
            rowparent.ChildBands[1].Band.AddNew();
            int rowindex = DBConvert.ParseInt(rowparent.ChildBands[1].Rows.Count) - 1;
            rowparent.ChildBands[1].Rows[rowindex].Cells["Priority"].Value = DBConvert.ParseInt(uc1.dtNewSource.Rows[i]["Priority"].ToString());
            rowparent.ChildBands[1].Rows[rowindex].Cells["ProcessCodePid"].Value = DBConvert.ParseLong(uc1.dtNewSource.Rows[i]["ProcessCodePid"].ToString());
            rowparent.ChildBands[1].Rows[rowindex].Cells["ProcessNameEN"].Value = uc1.dtNewSource.Rows[i]["ProcessNameEN"].ToString();
            if (DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["Capacity"].ToString()) != double.MinValue)
            {
              rowparent.ChildBands[1].Rows[rowindex].Cells["Capacity"].Value = DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["Capacity"].ToString());
            }
            if (DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["SetupTime"].ToString()) != double.MinValue)
            {
              rowparent.ChildBands[1].Rows[rowindex].Cells["SetupTime"].Value = DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["SetupTime"].ToString());
            }
            rowparent.ChildBands[1].Rows[rowindex].Cells["ProcessTime"].Value = DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["ProcessTime"].ToString());
            rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime1"].Value = DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["LeadTime1"].ToString());
            rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime2"].Value = DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["LeadTime2"].ToString());
            rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime3"].Value = DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["LeadTime3"].ToString());
            rowparent.ChildBands[1].Rows[rowindex].Cells["LeadTime4"].Value = DBConvert.ParseDouble(uc1.dtNewSource.Rows[i]["LeadTime4"].ToString());
            rowparent.ChildBands[1].Rows[rowindex].Cells["Notation"].Value = uc1.dtNewSource.Rows[i]["Notation"].ToString();
            rowparent.ChildBands[1].Rows[rowindex].Cells["NonCalculate"].Value = DBConvert.ParseInt(uc1.dtNewSource.Rows[i]["NonCalculate"].ToString());
          }
          this.CheckProcessDuplicate();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0125", "Part Type");
        }
      }
    }

    /// <summary>
    /// SỰ KIỆN CLICK NÚT SAVE
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // CHECK DUPLICATE PROCESS
      if (this.isDuplicateProcess)
      {
        WindowUtinity.ShowMessageError("ERR0013", "Process Code or Priority");
        return;
      }

      //CHECK VALID
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadDataFromList(pidItem);
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
    }

    /// <summary>
    /// SỰ KIỆN KHI THAY ĐỔI CHECKBOX CONFIRM
    /// Nếu là PLN khi check = 1
    /// Nếu là TEC khi check = 2
    /// Nếu là CAR khi check = 3
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkConfirm_CheckedChanged(object sender, EventArgs e)
    {
      if (bNew == true && bTec == false)
      {
        if (chkConfirm.Checked)
        {
          isConfirm = 1;
        }
        else
        {
          isConfirm = this.flagConfirm;
        }
      }

      if (bTec == true && bConfirm == false)
      {
        if (chkConfirm.Checked)
        {
          isConfirm = 2;
        }
        else
        {
          isConfirm = this.flagConfirm;
        }
      }

      if (bConfirm == true && bNew == false)
      {
        if (chkConfirm.Checked)
        {
          isConfirm = 3;
        }
        else
        {
          isConfirm = this.flagConfirm;
        }
      }
      if (bNew == true && bTec == true && bConfirm == true)
      {
        if (chkConfirm.Checked)
        {
          isConfirm = 3;
        }
        else
        {
          isConfirm = this.flagConfirm;
        }
      }
    }

    /// <summary>
    /// SỰ KIỆN CLICK NÚT CLOSE
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// SỰ KIỆN SAU KHI UPDATE TRÊN LƯỚI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      DataTable dt = (DataTable)((DataSet)ultData.DataSource).Tables[1];
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "PartPercent":
          totalPartPercent = 0;
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            totalPartPercent += DBConvert.ParseInt(ultData.Rows[i].Cells["PartPercent"].Value.ToString());
          }
          txtTotalPercent.Text = totalPartPercent.ToString();
          break;
        case "ProcessCodePid":
          if (DBConvert.ParseLong(e.Cell.Row.ParentRow.Cells["PidDetail"].Value.ToString()) == long.MinValue)
          {
            e.Cell.Row.ParentRow.Cells["PidDetail"].Value = pidtemp;
            if (e.Cell.Row.ParentRow.ChildBands[0].Rows.Count > 0)
            {
              for (int k = 0; k < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; k++)
              {
                e.Cell.Row.ParentRow.ChildBands[0].Rows[k].Cells["PidDetail"].Value = pidtemp;
              }
            }
            e.Cell.Row.Cells["DetailPid"].Value = pidtemp;
            pidtemp++;
          }
          //Load Process
          if (ultDDProcess.SelectedRow != null)
          {
            //e.Cell.Row.Cells["ProcessCode"].Value = DBConvert.ParseLong(ultDDProcess.SelectedRow.Cells["Pid"].Value.ToString());
            e.Cell.Row.Cells["ProcessNameEN"].Value = ultDDProcess.SelectedRow.Cells["ProcessNameEN"].Value;
            e.Cell.Row.Cells["Priority"].Value = e.Cell.Row.Index + 1;
            try
            {
              e.Cell.Row.Cells["SetupTime"].Value = ultDDProcess.SelectedRow.Cells["SetupTime"].Value;
            }
            catch
            {
              e.Cell.Row.Cells["SetupTime"].Value = DBNull.Value;
            }
            e.Cell.Row.Cells["ProcessTime"].Value = ultDDProcess.SelectedRow.Cells["ProcessTime"].Value;
            try
            {
              e.Cell.Row.Cells["Capacity"].Value = ultDDProcess.SelectedRow.Cells["Capacity"].Value;
            }
            catch
            {
              e.Cell.Row.Cells["Capacity"].Value = DBNull.Value;
            }
            e.Cell.Row.Cells["LeadTime1"].Value = ultDDProcess.SelectedRow.Cells["LeadTime1"].Value;
            e.Cell.Row.Cells["LeadTime2"].Value = ultDDProcess.SelectedRow.Cells["LeadTime2"].Value;
            e.Cell.Row.Cells["LeadTime3"].Value = ultDDProcess.SelectedRow.Cells["LeadTime3"].Value;
            e.Cell.Row.Cells["LeadTime4"].Value = ultDDProcess.SelectedRow.Cells["LeadTime4"].Value;
            e.Cell.Row.Cells["Notation"].Value = ultDDProcess.SelectedRow.Cells["Notation"].Value;
          }

          //Check Process Duplicate
          this.CheckProcessDuplicate();
          //Auto priority
          //this.AutoPriority();
          break;
        case "LocationDefault":
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            if (ultData.Rows[i].Cells["LocationDefault"].Text == "SUB")
            {
              ultData.Rows[i].Cells["Supplier1"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
              ultData.Rows[i].Cells["Supplier2"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
              ultData.Rows[i].Cells["Supplier3"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
              ultData.Rows[i].Cells["Supplier4"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
            }
            else
            {
              ultData.Rows[i].Cells["Supplier1"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
              ultData.Rows[i].Cells["Supplier2"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
              ultData.Rows[i].Cells["Supplier3"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
              ultData.Rows[i].Cells["Supplier4"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
              ultData.Rows[i].Cells["Supplier1"].Value = DBNull.Value;
              ultData.Rows[i].Cells["Supplier2"].Value = DBNull.Value;
              ultData.Rows[i].Cells["Supplier3"].Value = DBNull.Value;
              ultData.Rows[i].Cells["Supplier4"].Value = DBNull.Value;
            }
          }
          break;
        case "Priority":
          this.CheckProcessDuplicate();
          break;
        case "ComponentCode":
          {
            //if (DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["PartType"].Value.ToString()) == 1)
            //{
            UltraDropDown ultc = (UltraDropDown)e.Cell.Row.Cells["ComponentCode"].ValueList;
            if (ultc.SelectedRow != null)
            {
              int compRevision = DBConvert.ParseInt(ultc.SelectedRow.Cells["CompRevision"].Value.ToString());
              if (compRevision < 0)
              {
                e.Cell.Row.Cells["CompRev"].Value = DBNull.Value;
              }
              else
              {
                e.Cell.Row.Cells["CompRev"].Value = compRevision;
              }

              e.Cell.Row.Cells["Qty"].Value = DBConvert.ParseInt(ultc.SelectedRow.Cells["Qty"].Value.ToString());
              e.Cell.Row.Cells["CompName"].Value = ultc.SelectedRow.Cells["CompNameEN"].Value.ToString();
              e.Cell.Row.Cells["TotalQty"].Value = DBConvert.ParseInt(ultc.SelectedRow.Cells["TotalQty"].Value.ToString());
              e.Cell.Row.Cells["CompGroup"].Value = DBConvert.ParseInt(ultc.SelectedRow.Cells["CompGroup"].Value.ToString());
              e.Cell.Row.Cells["IsSave"].Value = 1;
              if (e.Cell.Row.Cells["PidDetail"].Value == DBNull.Value)
              {
                if (e.Cell.Row.ParentRow.Cells["PidDetail"].Value == DBNull.Value)
                {
                  e.Cell.Row.ParentRow.Cells["PidDetail"].Value = e.Cell.Row.ParentRow.Index * (-1);
                }
                e.Cell.Row.Cells["PidDetail"].Value = e.Cell.Row.ParentRow.Cells["PidDetail"].Value;
              }
            }
            this.CheckCompDuplicate();
            //}
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// SU KIỆN KHI THAY ĐỔI 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChkDefault_CheckedChanged(object sender, EventArgs e)
    {
      if (ChkDefault.Checked == true)
      {
        isDefault = 1;
      }
      else
      {
        isDefault = 0;
      }
    }

    /// <summary>
    /// SỰ KIỆN TRƯỚC KHI DELETE DÒNG TRÊN LƯỚI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        if (row.Band.Index == 2)
        {
          long pid = DBConvert.ParseLong(row.Cells["PidDescription"].Value.ToString());
          if (pid != long.MinValue)
          {
            this.listDeletedPid.Add(pid);
          }
        }
        else
        {
          long pid = DBConvert.ParseLong(row.Cells["PidComp"].Value.ToString());
          int flag = DBConvert.ParseInt(row.Cells["IsSave"].Value.ToString());
          if (flag == 1)
          {
            if (pid != long.MinValue)
            {
              this.listDeleteComp.Add(pid);
            }
          }
        }
      }
    }

    /// <summary>
    /// SỰ KIỆN TRƯỚC KHI UPDATE DÒNG TRÊN LƯỚI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string values = e.NewValue.ToString();
      switch (colName)
      {
        case "PartPercent":
          {
            if (DBConvert.ParseInt(values) < 0 || DBConvert.ParseInt(values) > 100)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Part Percent");
              e.Cancel = true;
            }
          }
          break;
        case "Priority":
          {
            if (DBConvert.ParseInt(values) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Priority");
              e.Cancel = true;
            }
          }
          break;
        case "PartType":
          {
            if (values.Length > 0)
            {
              DataTable dt = ((DataSet)ultData.DataSource).Tables[0];
              DataRow[] parttype = dt.Select(string.Format("PartType NOT IN (1, 3) AND PartType = {0}", DBConvert.ParseInt(values)));
              if (parttype.Length > 0)
              {
                WindowUtinity.ShowMessageError("ERR0006", "This Part Type");
                e.Cancel = true;
              }
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// SỰ KIỆN SAU KHI DELETE DÒNG TRÊN LƯỚI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.CheckProcessDuplicate();
      this.CheckCompDuplicate();
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      switch (columnName)
      {
        //case "Supplier1":
        //  if (DBConvert.ParseLong(e.Cell.Row.Cells["LocationDefault"].Value.ToString()) == 39)
        //  {
        //    e.Cell.Row.Cells["Supplier1"].Activation = Activation.AllowEdit;
        //  }
        //  else
        //  {
        //    e.Cell.Row.Cells["Supplier1"].Activation = Activation.ActivateOnly;
        //    e.Cell.Row.Cells["Supplier1"].Value = DBNull.Value;
        //  }
        //  break;
        //case "Supplier2":
        //  if (DBConvert.ParseLong(e.Cell.Row.Cells["LocationDefault"].Value.ToString()) == 39)
        //  {
        //    e.Cell.Row.Cells["Supplier2"].Activation = Activation.AllowEdit;
        //  }
        //  else
        //  {
        //    e.Cell.Row.Cells["Supplier2"].Activation = Activation.ActivateOnly;
        //    e.Cell.Row.Cells["Supplier2"].Value = DBNull.Value;
        //  }
        //  break;
        //case "Supplier3":
        //  if (DBConvert.ParseLong(e.Cell.Row.Cells["LocationDefault"].Value.ToString()) == 39)
        //  {
        //    e.Cell.Row.Cells["Supplier3"].Activation = Activation.AllowEdit;
        //  }
        //  else
        //  {
        //    e.Cell.Row.Cells["Supplier3"].Activation = Activation.ActivateOnly;
        //    e.Cell.Row.Cells["Supplier3"].Value = DBNull.Value;
        //  }
        //  break;
        //case "Supplier4":
        //  if (DBConvert.ParseLong(e.Cell.Row.Cells["LocationDefault"].Value.ToString()) == 39)
        //  {
        //    e.Cell.Row.Cells["Supplier4"].Activation = Activation.AllowEdit;
        //  }
        //  else
        //  {
        //    e.Cell.Row.Cells["Supplier4"].Activation = Activation.ActivateOnly;
        //    e.Cell.Row.Cells["Supplier4"].Value = DBNull.Value;
        //  }
        //  break;
        case "ComponentCode":
          {
            //if (DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["PartType"].Value.ToString()) == 1 && DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["FlagComp"].Value.ToString()) != 0)
            //{
            e.Cell.Row.Cells["ComponentCode"].Activation = Activation.AllowEdit;
            e.Cell.Row.Cells["Qty"].Activation = Activation.AllowEdit;
            UltraDropDown ultc = (UltraDropDown)e.Cell.Row.Cells["ComponentCode"].ValueList;
            e.Cell.Row.Cells["ComponentCode"].ValueList = this.LoadComponentHW(ultraCBItemCode.Value.ToString(), DBConvert.ParseInt(CBRevision.SelectedValue.ToString()), 1, DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["PartType"].Value.ToString()), ultc);
            //}
            //else
            //{
            //  e.Cell.Row.Cells["ComponentCode"].Activation = Activation.ActivateOnly;
            //  e.Cell.Row.Cells["Qty"].Activation = Activation.ActivateOnly;
            //}
          }
          break;
        default:
          break;
      }
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      //UltraGridRow row = (!ultData.Selected.Rows[0].HasParent()) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      //int parttype = DBConvert.ParseInt(row.Cells["PartType"].Value.ToString());
      //string itemcode = ultraCBItemCode.Value.ToString();
      //int rev = DBConvert.ParseInt(CBRevision.SelectedValue.ToString());
      //if (parttype == 2)
      //{
      //  Technical.viewBOM_01_003 objITM = new Technical.viewBOM_01_003();
      //  objITM.iIndex = 1;
      //  objITM.currItemCode = itemCode;
      //  objITM.currRevision = rev;
      //  objITM.tabControl1.SelectTab(objITM.tabHardware);
      //  Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP LEVEL 2ND", false, ViewState.Window);
      //}
    }
    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (ultItemReference.SelectedRow != null)
      {
        long pidItem = DBConvert.ParseLong(ultItemReference.Value);
        this.CopyItemReference(pidItem);
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0115", "Item Reference");
      }
    }
    #endregion
  }
}
