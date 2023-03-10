/*
  Author      : Huu Phuoc
  Date        : 2016-04-04
  Description : 
  Standard Form: view_SaveMasterDetail
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_02_009 : MainUserControl
  {
    #region Field
    //Biến phân quyền CS
    public bool btCS = true;
    //Biến phân quyền QA
    public bool btQA = true;
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public viewCSD_02_009()
    {
      InitializeComponent();
    }

    private void viewCSD_02_009_Load(object sender, EventArgs e)
    {
  
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(groupBoxMaster);      
      this.InitData();
      btnSave.Enabled = false;
      this.LoadStatus();
      //this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
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
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
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
        this.LoadData();
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

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      string sql = "";
      DataTable dt = new DataTable();
      //Load dropdown : Customer
      sql = @"SELECT CUS.Pid, CUS.CustomerCode + ' - ' + CUS.Name Name 
              FROM TblCSDCustomerInfo CUS
	              LEFT JOIN TblCSDNation CONTRY ON CUS.Country = CONTRY.Pid
	              LEFT JOIN TblBOMCodeMaster REGION ON REGION.[Group] = 99004
									                AND CUS.Region = REGION.Code
	              LEFT JOIN TblBOMCodeMaster GC ON GC.[Group] = 16033
								               AND CUS.GroupCustomerPid = GC.Code
                ORDER BY CUS.CustomerCode";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      ControlUtility.LoadUltraCombo(ultCboCustomer, dt, "Pid", "Name", "Pid");
//      //Load dropdown : SaleCode
//      sql = @"SELECT SaleCode, ItemCode, CarcassCode
//              FROM TblBOMItemBasic
//              WHERE ISNULL(SaleCode, '') <> ''";
//      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
//      ControlUtility.LoadUltraDropDown(ultrDropSaleCode, dt, "SaleCode", "SaleCode");

//      //Load Dropdown : Complaint Category
//      sql = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 290316";
//      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
//      ControlUtility.LoadUltraDropDown(ultrDropComplaint, dt, "Code", "Value", "Code");
    }

    /// <summary>
    /// Load CB Status
    /// </summary>
    private void LoadStatus()
    {
      string cm = string.Format(@"SELECT 0 ID, 'CS Closed' Name
                                   UNION
                                  SELECT 1 ID, 'Pending' Name
                                   UNION
                                  SELECT 2 ID, 'Finish' Name 
                                  UNION
                                  SELECT 3 ID, 'QA Close' Name");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cm);
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
    private void SetStatusControl()
    {
      ultCboCustomer.Value = DBNull.Value;
      txtSaleCode.Text = string.Empty;
      txtTransactionCode.Text = string.Empty;
      ultrDateFrm.Value = DBNull.Value;
      ultrDateTo.Value = DBNull.Value;
      ultCBStatus.Value = DBNull.Value;
 
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
      }
    }
    
    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
       DBParameter[] inputParam = new DBParameter[6];
      //DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, long.MinValue) };
       if (txtTransactionCode.Text.ToString().Trim() != "")
       {
         inputParam[0] = new DBParameter("@TransactionCode", DbType.String, txtTransactionCode.Text.ToString().Trim());   
       }
       else
       {
         inputParam[0] = new DBParameter("@TransactionCode", DbType.String,null);
       }
      
     inputParam [1] = new DBParameter("@CustomerPid", DbType.Int64 , ultCboCustomer.Value) ;
     if (txtSaleCode.Text.ToString().Trim() !="")
     {
       inputParam[2] = new DBParameter("@SaleCode", DbType.String, txtSaleCode.Text.ToString().Trim());
     }
     else
     {
       inputParam[2] = new DBParameter("@SaleCode", DbType.String, null);
     }
     
     inputParam [3] =  new DBParameter("@DateFrm", DbType.DateTime, ultrDateFrm.Value) ;
     inputParam [4] =  new DBParameter("@DateTo", DbType.DateTime, ultrDateTo.Value) ;
     inputParam[5] = new DBParameter("@Status", DbType.Int32, ultCBStatus.Value);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDFinalCustomerComplainMaster_Search", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        //this.LoadMainData(dsSource.Tables[0]);
        dsSource.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dsSource.Tables[0].Columns["Pid"] },
                                      new DataColumn[] { dsSource.Tables[1].Columns["TransactionPid"] }, false));

        ultData.DataSource = dsSource;
      }
      if (ultData.Rows != null)
      {
        lblCount.Text = "Count: " + ultData.Rows.Count.ToString();
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["CSClosed"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["QAClosed"].Value.ToString()) == 0)
          {
            ultData.Rows[i].Appearance.BackColor = Color.White;
          }
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["QAClosed"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["CSClosed"].Value.ToString()) == 0)
          {
            ultData.Rows[i].Appearance.BackColor = Color.GreenYellow;
          }
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["CSClosed"].Value.ToString()) == 0 && DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["QAClosed"].Value.ToString()) == 0 )
          {
            ultData.Rows[i].Appearance.BackColor = Color.LightGreen;
          }
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["QAClosed"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["CSClosed"].Value.ToString()) == 1)
          {
            ultData.Rows[i].Appearance.BackColor = Color.LightSalmon;
          }
        }
      }
      //this.SetStatusControl();
      //this.NeedToSave = false;
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      //this.SaveData();
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      //e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      //e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      //e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      //e.Layout.Bands[0].Summaries.Clear();

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["TransactionPid"].Hidden = true;
      //e.Layout.Bands[0].Columns["Confirm"].Hidden = true;
      // Set caption column 
      e.Layout.Bands[0].Columns["TransactionCode"].Header.Caption = "QCD Code";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[0].Columns["UpdateDate"].Header.Caption = "Update Date";

      // set color
      //e.Layout.Bands[0].Columns["TransactionCode"].CellAppearance.BackColor = Color.LightGray;
      //e.Layout.Bands[0].Columns["CreateBy"].CellAppearance.BackColor = Color.LightGray;
      //e.Layout.Bands[0].Columns["CreateDate"].CellAppearance.BackColor = Color.LightGray;

      // Set Column Style
      e.Layout.Bands[1].Columns["CSClosed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["QAClosed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //for (int i = 0; i < ultData.Rows.Count; i++)
      //{
      //  if (ultData.Rows[i].Cells["Confirm"].Value.ToString() == "1")
      //  {
      //    //e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
      //    ultData.Rows[i].CellAppearance.BackColor = Color.LightGray;
      //  }
      //}

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;      
       
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      if (ultData.Rows.Count > 0)
      {        
        e.Layout.Bands[1].Columns["Name"].Header.Caption ="Customer Name";
        e.Layout.Bands[1].Columns["SaleCode"].Header.Caption ="Sale Code";
        e.Layout.Bands[1].Columns["ItemCode"].Header.Caption ="Item Code";
        e.Layout.Bands[1].Columns["CarcassCode"].Header.Caption ="Carcass Code";
        e.Layout.Bands[1].Columns["ItemValue"].Header.Caption = "Item Value";
        e.Layout.Bands[1].Columns["InvoiceMonth"].Header.Caption = "Invoice Month";
        e.Layout.Bands[1].Columns["InvoiceYear"].Header.Caption = "Invoice Year";        
        e.Layout.Bands[1].Columns["ClaimAmout"].Header.Caption = "Claim Amout";
        e.Layout.Bands[1].Columns["DetailOfComplaint"].Header.Caption = "Detail Of\nComplaint";
        e.Layout.Bands[1].Columns["ComplaintCategory"].Header.Caption = "Complaint\nCategory";
        e.Layout.Bands[1].Columns["ImmediateActionCustomer"].Header.Caption = "Immediate Action\nCustomer";
        e.Layout.Bands[1].Columns["PreventativeActionFactory"].Header.Caption = "Preventative\nActionFactory";
        e.Layout.Bands[1].Columns["Name"].MaxWidth = 200;
        e.Layout.Bands[1].Columns["Name"].MinWidth = 200;

        //e.Layout.Bands[1].Columns["Name"].Header.Fixed = true;
        //e.Layout.Bands[1].Columns["SaleCode"].Header.Fixed = true;
        //e.Layout.Bands[1].Columns["Qty"].Header.Fixed = true;
        //e.Layout.Bands[1].Columns["ItemCode"].Header.Fixed = true;
        //e.Layout.Bands[1].Columns["CarcassCode"].Header.Fixed = true;
        //e.Layout.Bands[1].Columns["ItemID"].Header.Fixed = true;
        //e.Layout.Bands[1].Columns["Country"].Header.Fixed = true;

        e.Layout.Bands[1].ColHeaderLines = 2;
        for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
        {
          // Set color for edit & read only cell
          e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
          // Set Align column
          Type colType = e.Layout.Bands[1].Columns[i].DataType;
          if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
          {
            e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[1].Columns[i].Format = "#,##0.##";
          }
        }
      }       
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
    
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
    
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      //Open Detail Form    
      try
      {
        UltraGridRow row = ultData.Selected.Rows[0];
        viewCSD_02_008 uc = new viewCSD_02_008();
        long pid = long.MinValue;

        try
        {
          pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        }
        catch
        {
        }
        if (pid > 0)
        {
          uc.transactionPid = pid;
          Shared.Utility.WindowUtinity.ShowView(uc, "List Final Customer Complain", false, Shared.Utility.ViewState.MainWindow);
        }
      }
      catch
      {
        viewCSD_02_008 uc1 = new viewCSD_02_008();
        uc1.transactionPid = long.MinValue;
        Shared.Utility.WindowUtinity.ShowView(uc1, "List Final Customer Complain", false, Shared.Utility.ViewState.MainWindow);
      }     
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
        btnSave.Enabled = true;
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //Open Detail Form    
      try
      {
        UltraGridRow row = ultData.Selected.Rows[0];
        viewCSD_02_008 uc = new viewCSD_02_008();
        long pid = long.MinValue;
        string TransCode = string.Empty;
        string CreateBy = string.Empty;
        DateTime CreateDate = DateTime.MinValue;
        string Remark = string.Empty;
        int Confirm = 0;
        try
        {
          pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          TransCode = DBConvert.ParseString(row.Cells["TransactionCode"].Value.ToString());
          CreateBy = DBConvert.ParseString(row.Cells["CreateBy"].Value.ToString());
          CreateDate = DBConvert.ParseDateTime(row.Cells["CreateDate"].Value.ToString(), System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
          Remark = DBConvert.ParseString(row.Cells["Remark"].Value.ToString());
        }
        catch
        {
        }
        if (pid > 0)
        {
          uc.viewPid = pid;
          uc.txtTransactionCode.Text = TransCode;
          uc.txtCreateBy.Text = CreateBy.ToString();
          uc.txtCreateDate.Text  = CreateDate.ToString();
          uc.txtRemark.Text = Remark.ToString();
          Shared.Utility.WindowUtinity.ShowView(uc, "List Final Customer Complain", false, Shared.Utility.ViewState.MainWindow);
        }
      }
      catch(Exception ex)
      {
        string a = ex.Message;
        //viewCSD_02_008 uc1 = new viewCSD_02_008();
        //uc1.viewPid = long.MinValue;
        //Shared.Utility.WindowUtinity.ShowView(uc1, "List Final Customer Complain", false, Shared.Utility.ViewState.MainWindow);
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      //this.SetNeedToSave();      
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "Data");
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDFinalCustomerComplainDelete_Master", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          MessageBox.Show("Can not delete data");
          return ;
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
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
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.SetStatusControl();
    }
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {

    }

    #endregion Event








  }
}
