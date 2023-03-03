/***************************************
 *Author: Nguyen Ngoc Tien             *    
 *Create day: 17/10/2014               *
 *Description: List Item with routing  *
 ***************************************/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_96_005 : MainUserControl
  {
    /// <summary>
    /// 
    /// </summary>
    public viewWIP_96_005()
    {
      InitializeComponent();
    }
    #region Init
    private bool BCar = true;
    private bool BPln = true;
    private bool BTec = true;
    private int isConfirm = int.MinValue;
    #endregion

    #region Function

    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();

      ////Table Master
      DataTable dtMaster = new DataTable();
      dtMaster.Columns.Add("PidMaster", typeof(System.Int64));
      dtMaster.Columns.Add("ItemCode", typeof(System.String));
      dtMaster.Columns.Add("Revision", typeof(System.Int32));
      dtMaster.Columns.Add("PartGroupPid", typeof(System.Int64));
      dtMaster.Columns.Add("PartGroup", typeof(System.String));
      dtMaster.Columns.Add("IsDefault", typeof(System.Int32));
      dtMaster.Columns.Add("IsConfirm", typeof(System.Int32));
      dtMaster.Columns.Add("SDefault", typeof(System.String));
      dtMaster.Columns.Add("SStatus", typeof(System.String));
      dtMaster.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(dtMaster);

      //Table Detail

      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("PidDetail", typeof(System.Int64));
      dtDetail.Columns.Add("MasterPid", typeof(System.Int64));
      dtDetail.Columns.Add("PartCodePid", typeof(System.Int64));
      dtDetail.Columns.Add("PartName", typeof(System.String));
      dtDetail.Columns.Add("PartType", typeof(System.Int32));
      dtDetail.Columns.Add("PartPercent", typeof(System.Double));
      dtDetail.Columns.Add("LocationDefault", typeof(System.Int64));
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
      ds.Tables.Add(dtComp);

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

      ds.Tables.Add(dtDescr);

      ds.Relations.Add(new DataRelation("dsMaster_dsDetail", new DataColumn[] { ds.Tables[0].Columns["PidMaster"] }, new DataColumn[] { ds.Tables[1].Columns["MasterPid"] }, false));
      ds.Relations.Add(new DataRelation("dsDetail_dsComp", new DataColumn[1] { ds.Tables[1].Columns["PidDetail"] }, new DataColumn[1] { ds.Tables[2].Columns["PidDetail"] }, false));
      ds.Relations.Add(new DataRelation("dsDetail_dsDescrip", new DataColumn[] { ds.Tables[1].Columns["PidDetail"] }, new DataColumn[] { ds.Tables[3].Columns["DetailPid"] }, false));

      return ds;
    }
    //Load Part Group
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

    //Load Status
    private void LoadStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'PLN Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'TEC Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'CAR Confirmed' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBStatus.DataSource = dtSource;
      ultCBStatus.DisplayMember = "Name";
      ultCBStatus.ValueMember = "ID";
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBStatus.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadDDProcess()
    {
      string commandText = @"SELECT Pid Value, ProcessCode + ' | ' + ProcessNameEN Display
                             FROM TblPLNProcessCarcass_ProcessInfo ";
      DataTable dtSourcePro = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBProcess.DataSource = dtSourcePro;
      ultraCBProcess.DisplayMember = "Display";
      ultraCBProcess.ValueMember = "Value";
      ultraCBProcess.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBProcess.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }

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
    /// Load Location Default
    /// </summary>
    private void LoadLocationDefault()
    {
      string cmText = @"SELECT WA.Pid Value, WA.StandByEN Display
                        FROM TblBOMCodeMaster CM 
                              INNER JOIN TblWIPWorkArea WA ON CM.Code = WA.Pid
                        WHERE [Group] = 81915
                        ORDER BY Sort";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultraDDLocation.DataSource = dt;
      ultraDDLocation.DisplayMember = "Display";
      ultraDDLocation.ValueMember = "Value";
      ultraDDLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDLocation.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }
    /// <summary>
    /// hàm Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Search()
    {
      btnSearch.Enabled = false;
      DBParameter[] input = new DBParameter[6];
      if (txtItemCode.Text.Trim().Length > 0)
      {
        input[1] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text.Trim());
      }
      if (ultraCBPartGroup.Text.Length > 0)
      {
        input[2] = new DBParameter("@PartGroupPid", DbType.Int64, DBConvert.ParseLong(ultraCBPartGroup.Value.ToString()));
      }
      if (ultCBStatus.Text.Length > 0)
      {
        input[3] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }
      if (ChkDefault.Checked == true)
      {
        input[4] = new DBParameter("@IsDefault", DbType.Int32, 1);
      }
      if (ultraCBProcess.Text.Length > 0)
      {
        input[5] = new DBParameter("@ProcessPid", DbType.Int64, DBConvert.ParseLong(ultraCBProcess.Value.ToString()));
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNRoutingDefaultMaster_Select", 500, input);
      DataSet dsData = this.CreateDataSet();
      dsData.Tables[0].Merge(dsSource.Tables[0]);
      dsData.Tables[1].Merge(dsSource.Tables[1]);
      dsData.Tables[2].Merge(dsSource.Tables[2]);
      dsData.Tables[3].Merge(dsSource.Tables[3]);
      ultraGridInformation.DataSource = dsData;
      this.SetColor();
      btnSearch.Enabled = true;
    }

    //Set Color
    private void SetColor()
    {
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["IsConfirm"].Value.ToString()) == 1)
        {
          row.CellAppearance.BackColor = Color.PaleGreen;
        }
        if (DBConvert.ParseInt(row.Cells["IsConfirm"].Value.ToString()) == 2)
        {
          row.CellAppearance.BackColor = Color.LightSteelBlue;
        }
        if (DBConvert.ParseInt(row.Cells["IsConfirm"].Value.ToString()) == 3)
        {
          row.CellAppearance.BackColor = Color.LightPink;
        }
      }
    }

    private void LoadPartType()
    {
      string cmText = "SELECT Code,Value FROM TblBOMCodeMaster WHERE [GROUP]=1992";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultraPartType.DataSource = dt;
      ultraPartType.DisplayMember = "Value";
      ultraPartType.ValueMember = "Code";
      ultraPartType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraPartType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }


    private void ExportExcel()
    {
      if (ultraGridInformation.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        Utility.ExportToExcelWithDefaultPath(ultraGridInformation, out xlBook, "ProcessCarcassList", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Process Carcass List";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }


    #endregion

    #region Events

    private void viewWIP_96_005_Load(object sender, EventArgs e)
    {
      BCar = btnCAR.Visible;
      BPln = btnPLN.Visible;
      BTec = btnTEC.Visible;
      panel1.Visible = false;
      btnNew.Visible = false;
      btnImportEX.Visible = false;
      if (BPln == true)
      {
        btnNew.Visible = true;
        btnImportEX.Visible = true;
      }
      LoadDDProcess();
      LoadPartGroup();
      LoadStatus();
      LoadPartType();
      LoadLocationDefault();
      LoadSupplier();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultraGridInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Columns["IsConfirm"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["IsConfirm"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["IsDefault"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["IsDefault"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["PartGroupPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsConfirm"].Hidden = true;
      e.Layout.Bands[0].Columns["IsDefault"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsDefault"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["IsDefault"].Header.Caption = "Default";
      e.Layout.Bands[0].Columns["SStatus"].Header.Caption = "Confirm";
      e.Layout.Bands[0].Columns["SDefault"].Hidden = true;
      e.Layout.Bands[0].Columns["PidMaster"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["PidDetail"].Hidden = true;
      e.Layout.Bands[1].Columns["MasterPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PartCodePid"].Hidden = true;
      e.Layout.Bands[1].Columns["PartType"].ValueList = ultraPartType;
      e.Layout.Bands[1].Columns["PartType"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["LocationDefault"].ValueList = ultraDDLocation;
      e.Layout.Bands[1].Columns["Supplier1"].ValueList = ultddSup1;
      e.Layout.Bands[1].Columns["Supplier2"].ValueList = ultddSup1;
      e.Layout.Bands[1].Columns["Supplier3"].ValueList = ultddSup1;
      e.Layout.Bands[1].Columns["Supplier4"].ValueList = ultddSup1;

      e.Layout.Bands[2].Columns["PidComp"].Hidden = true;
      e.Layout.Bands[2].Columns["PidDetail"].Hidden = true;
      e.Layout.Bands[2].Columns["CompGroup"].Hidden = true;

      e.Layout.Bands[3].Columns["LeadTime1"].Header.Caption = "Lead time <= 2\n(days)";
      e.Layout.Bands[3].Columns["LeadTime2"].Header.Caption = "2 < Lead time <= 6\n(days)";
      e.Layout.Bands[3].Columns["LeadTime3"].Header.Caption = "6 < Lead time <= 12\n(days)";
      e.Layout.Bands[3].Columns["LeadTime4"].Header.Caption = "Lead time > 12\n(days)";
      e.Layout.Bands[3].Columns["PidDescription"].Hidden = true;
      e.Layout.Bands[3].Columns["DetailPid"].Hidden = true;
      e.Layout.Bands[3].Columns["ProcessCodePid"].ValueList = ultraDropDown1;
      e.Layout.Bands[3].Columns["ProcessCodePid"].CellActivation = Activation.ActivateOnly;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[3].ColHeaderLines = 2;
      e.Layout.Bands[2].ColHeaderLines = 2;
      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      for (int i = 0; i < e.Layout.Bands[3].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[3].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[3].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[2].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[3].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[3].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[3].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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
      UltraGridRow row = (ultraGridInformation.Selected.Rows[0].ParentRow == null) ? ultraGridInformation.Selected.Rows[0] : ((ultraGridInformation.Selected.Rows[0].ParentRow.ParentRow == null) ? ultraGridInformation.Selected.Rows[0].ParentRow : ultraGridInformation.Selected.Rows[0].ParentRow.ParentRow);
      long MasterPid = DBConvert.ParseLong(row.Cells["PidMaster"].Value.ToString());
      viewWIP_96_004 uc = new viewWIP_96_004();
      uc.pidItem = MasterPid;
      WindowUtinity.ShowView(uc, "SET TIME FOR PART OF ITEM", false, ViewState.MainWindow);
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewWIP_96_004 uc = new viewWIP_96_004();
      WindowUtinity.ShowView(uc, "SET TIME FOR PART OF ITEM", false, ViewState.MainWindow);
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtItemCode.Text = string.Empty;
      ultCBStatus.Value = DBNull.Value;
      ultraCBPartGroup.Value = DBNull.Value;
      ChkDefault.Checked = false;
    }

    private void btnUnlock_Click(object sender, EventArgs e)
    {
      bool flag = false;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          flag = true;
          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["PidMaster"].Value.ToString()));

          if (BTec == true && BCar == false && BPln == false)
          {
            this.isConfirm = 1;
          }
          if (BCar == true && BTec == false && BPln == false)
          {
            this.isConfirm = 2;
          }
          if (BPln == true && BTec == false && BCar == false)
          {
            this.isConfirm = 0;
          }
          if (BPln == true && BCar == true && BTec == true)
          {
            this.isConfirm = 0;
          }
          if (this.isConfirm != int.MinValue)
          {
            input[1] = new DBParameter("@IsConfirm", DbType.Int32, this.isConfirm);
          }
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPLNRoutingMasterUnlock", input, output);
          if ((output == null) || DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
          {
            flag = false;
          }
          else
          {
            flag = true;
          }
        }
      }
      if (flag == true)
      {
        WindowUtinity.ShowMessageSuccess("MSG0023");
        this.Search();
      }
    }

    private void ultraGridInformation_CellChange(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare("Select", column, true) == 0)
      {

        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 1)
        {
          if (BTec == true)
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["IsConfirm"].Value.ToString()) != 2)
            {
              e.Cell.Row.Cells["Select"].Value = 0;
            }
          }
          if (BCar == true)
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["IsConfirm"].Value.ToString()) != 3)
            {
              e.Cell.Row.Cells["Select"].Value = 0;
            }
          }
          if (BPln == true)
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["IsConfirm"].Value.ToString()) > 1)
            {
              e.Cell.Row.Cells["Select"].Value = 0;
            }
          }
          if (BPln == true && BCar == true && BTec == true)
          {
            e.Cell.Row.Cells["Select"].Value = 1;
          }
        }
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void btnImportEX_Click(object sender, EventArgs e)
    {
      viewWIP_96_007 uc = new viewWIP_96_007();
      WindowUtinity.ShowView(uc, "IMPORT ITEM PART", false, ViewState.MainWindow);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion
  }
}
