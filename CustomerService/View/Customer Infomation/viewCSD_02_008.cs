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
using System.Diagnostics;
using VBReport;
using Infragistics.Win;
using System.IO;
using DaiCo.Application.Web.Mail;



namespace DaiCo.CustomerService
{
  public partial class viewCSD_02_008 : MainUserControl
  {
    #region Field
    //Biến phân quyền CS
    private bool btCS = true;
    //Biến phân quyền QA
    private bool btQA = true;
    public long transactionPid = long.MinValue; 
    public long viewPid = long.MinValue;
    private string sourseFile = string.Empty;
    private string destFile = string.Empty;

    private IList listDeletedPid = new ArrayList();
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    public int Confirm = 0;
    #endregion Field

    #region Init
    public viewCSD_02_008()
    {
      InitializeComponent();
    }

    private void viewCSD_02_008_Load(object sender, EventArgs e)
    {
      ///Phan quyen
      this.btCS = btnCSClose.Visible;
      this.btQA = btnQAClose.Visible;
      //Hide nut phan quyen
      this.pnlAccess.Visible = false;
      // Add ask before closing form even if user change data
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";
      this.SetAutoAskSaveWhenCloseForm(groupBoxMaster);      
      this.InitData();
      this.LoadData();
      //if (Confirm == 1)
      //{
      //  this.EnableContrl();
      //}
      if (this.viewPid  < 0)
      {
        this.NewSetupControl();
      }
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
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }
    private void EnableContrl()
    {
      //chkConfirm.Checked = true;
      //chkConfirm.Enabled = false;
      btnSave.Enabled = false;
      txtRemark.ReadOnly = true;
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
      sql = @"SELECT CUS.Pid,CUS.CustomerCode +' - '+ CUS.Name Name, CONTRY.NationEN Country, REGION.Value Region, GC.Value CustomerGroup
              FROM TblCSDCustomerInfo CUS
	              LEFT JOIN TblCSDNation CONTRY ON CUS.Country = CONTRY.Pid
	              LEFT JOIN TblBOMCodeMaster REGION ON REGION.[Group] = 99004
									                AND CUS.Region = REGION.Code
	              LEFT JOIN TblBOMCodeMaster GC ON GC.[Group] = 16033
								               AND CUS.GroupCustomerPid = GC.Code
                ORDER BY CUS.CustomerCode";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      ControlUtility.LoadUltraDropDown(ultrDropCustomer, dt, "Pid", "Name", "Pid");
      //Load dropdown : SaleCode
      sql = @"SELECT SaleCode, ItemCode, CarcassCode
              FROM TblBOMItemBasic
              WHERE ISNULL(SaleCode, '') <> ''";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      ControlUtility.LoadUltraDropDown(ultrDropSaleCode, dt, "SaleCode", "SaleCode");

      //Load Dropdown : Complaint Category
      sql = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 290316";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      ControlUtility.LoadUltraDropDown(ultrDropComplaint, dt, "Code", "Value", "Code");
    }
    private void NewSetupControl()
    {
      string sql = "";
      sql = " SELECT dbo.fn_CSDFinalCustomerComplainGenCode('QCD') CODE ";
      DataTable dt = new DataTable();
      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      if (dt!=null)
      {
        txtTransactionCode.Text = dt.Rows[0][0].ToString();
      }
      sql = "SELECT EmpName  FROM VHRMEmployee WHERE Pid = "+ SharedObject.UserInfo.UserPid +"";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      if (dt != null)
      {
        txtCreateBy.Text = dt.Rows[0][0].ToString();
      }
      txtCreateDate.Text = DateTime.Now.ToString();
    }
    private void SetStatusControl()
    {

    }
    /// <summary>
    /// Create Datatable
    /// </summary>
    /// <returns></returns>
    private DataTable AddDataTable()
    {
      DataTable dtlist = new DataTable();
      dtlist.Columns.Add("Pid", typeof(System.Int64));
      dtlist.Columns.Add("TransactionPid", typeof(System.Int64));
      //dtlist.Columns.Add("QCDCode", typeof(System.String));
      dtlist.Columns.Add("CustomerPid", typeof(System.Int64));
      dtlist.Columns.Add("SaleCode", typeof(System.String));
      dtlist.Columns.Add("Qty", typeof(System.Int32));
      dtlist.Columns.Add("ItemCode", typeof(System.String));
      dtlist.Columns.Add("CarcassCode", typeof(System.String));
      dtlist.Columns.Add("ItemID", typeof(System.String));
      dtlist.Columns.Add("Country", typeof(System.String));
      dtlist.Columns.Add("Region", typeof(System.String));
      dtlist.Columns.Add("CustomerGroup", typeof(System.String));
      dtlist.Columns.Add("ItemValue", typeof(System.Int64));
      dtlist.Columns.Add("InvoiceMonth", typeof(System.Int32));
      dtlist.Columns.Add("InvoiceYear", typeof(System.Int32));
      dtlist.Columns.Add("ClaimAmout", typeof(System.Int64));
      dtlist.Columns.Add("ClaimValue", typeof(System.Double));
      dtlist.Columns.Add("DetailOfComplaint", typeof(System.String));
      dtlist.Columns.Add("ComplaintCategory", typeof(System.Int32));
      dtlist.Columns.Add("ImmediateActionCustomer", typeof(System.String));
      dtlist.Columns.Add("PreventativeActionFactory", typeof(System.String));
      dtlist.Columns.Add("IsStatus", typeof(System.Int32));
      dtlist.Columns.Add("CSClosed", typeof(System.Int32));
      dtlist.Columns.Add("CSRemark", typeof(System.String));
      dtlist.Columns.Add("QAClosed", typeof(System.Int32));
      dtlist.Columns.Add("QARemark", typeof(System.String));
      return dtlist;
    }
    /// <summary>
    /// Get Cutomer's Information by Name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private string GetCustomerString(string name, string field)
    {
      string sql = "";
      DataTable dt = new DataTable();
      string fl = "";
      sql =  " SELECT "+ field +" ";      
      sql = sql + "          FROM ";
      sql = sql + "          ( ";
      sql = sql + "            SELECT CUS.Pid, CUS.CustomerCode, CUS.Name, CONTRY.NationEN Country, REGION.Value Region, GC.Value CustomerGroup ";
      sql = sql + "            FROM TblCSDCustomerInfo CUS ";
      sql = sql + "              LEFT JOIN TblCSDNation CONTRY ON CUS.Country = CONTRY.Pid ";
      sql = sql + "              LEFT JOIN TblBOMCodeMaster REGION ON REGION.[Group] = 99004 ";
      sql = sql + "							                  AND CUS.Region = REGION.Code ";
      sql = sql + "              LEFT JOIN TblBOMCodeMaster GC ON GC.[Group] = 16033 ";
      sql = sql + "						                 AND CUS.GroupCustomerPid = GC.Code	 ";
      sql = sql + "          )CUS ";
      sql = sql + "         WHERE CUS.CustomerCode = '" + name + "'";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql,90);
      if (dt != null && dt.Rows.Count >0)
      {
        fl = DBConvert.ParseString(dt.Rows[0][0].ToString());
      }
      else
      {
        fl = string.Empty ;
      }
      return fl;
    }
    /// <summary>
    /// Get SaleCode's Information
    /// </summary>
    /// <param name="salecode"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    private string GetSaleCodeString(string salecode, string field)
    {
      string sql = "";
      string fl = "";
      DataTable dt = new DataTable();
      sql = " SELECT "+ field +" ";
      sql = sql +" FROM ( ";
	    sql = sql +"   SELECT SaleCode, ItemCode, CarcassCode ";
	    sql = sql +"   FROM TblBOMItemBasic " ;
	    sql = sql +"   WHERE ISNULL(SaleCode, '') <> '' ";
      sql = sql +" )SL ";
      sql = sql + " WHERE SaleCode ='"+ salecode +"' ";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql, 90);
      if (dt != null && dt.Rows.Count >0)
      {
        fl = DBConvert.ParseString(dt.Rows[0][0].ToString());
      }
      else
      {
        fl = "";
      }
      return fl;
    }
    private int CheckExistingSaleCode(string salecode, string field)
    {
      string sql = "";
      int fl = 0;
      DataTable dt = new DataTable();
      sql = " SELECT " + field + " ";
      sql = sql + " FROM ( ";
      sql = sql + "   SELECT SaleCode, ItemCode, CarcassCode ";
      sql = sql + "   FROM TblBOMItemBasic ";
      sql = sql + "   WHERE ISNULL(SaleCode, '') <> '' ";
      sql = sql + " )SL ";
      sql = sql + " WHERE SaleCode ='" + salecode + "' ";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql, 90);
      if (salecode.ToString().Trim() != "")
      {
        if (dt != null && dt.Rows.Count > 0)
        {
          fl = 0;
        }
        else
        {
          fl = 1;
        }
      }
      return fl;
    }
    /// <summary>
    /// Check list database
    /// </summary>
    private void CheckListDatabase()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["IsStatus"].Value.ToString()) == 1)
        {
          row.Cells["SaleCode"].Appearance.BackColor = Color.Yellow;
        }

        if (DBConvert.ParseString(row.Cells["TransactionPid"].Value.ToString()) == "")
        {
          row.Cells["TransactionPid"].Appearance.BackColor = Color.Blue;
        }

      }
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

      //DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, long.MinValue) };
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@TransactionPid", DbType.Int64 , this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDFinalCustomerComplain", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        //this.LoadMainData(dsSource.Tables[0]);
        ultData.DataSource = dsSource.Tables[0];
      }
      // Upload File
      this.ultUploadFile.DataSource = dsSource.Tables[1];
      if (this.ultUploadFile.Rows.Count > 0)
      {
        this.chkUpload.Checked = true;
        this.ultUploadFile.Visible = true;
      }
      else
      {
        this.ultUploadFile.Visible = false;
      }
      for (int i = 0; i < ultUploadFile.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultUploadFile.Rows[i];
        if (File.Exists(rowGrid.Cells["File"].Value.ToString()))
        {
          rowGrid.Cells["Type"].Appearance.Image = Image.FromFile(rowGrid.Cells["File"].Value.ToString());
        }
      }
      //Phan quyen
      for (int j = 0; j < ultData.Rows.Count; j++)
      {

        if (DBConvert.ParseInt(ultData.Rows[j].Cells["CSClosed"].Value.ToString()) == 1 || DBConvert.ParseInt(ultData.Rows[j].Cells["QAClosed"].Value.ToString()) == 1)
        {
          for (int k = 0; k < ultData.Rows.Band.Columns.Count; k++)
          {
            ultData.Rows[j].Cells[k].Activation = Activation.ActivateOnly;
          }
        }
        if (btCS == true)
        {
          if (DBConvert.ParseInt(ultData.Rows[j].Cells["CSClosed"].Value.ToString()) == 0 )
          {
            ultData.Rows[j].Activation = Activation.AllowEdit;
          }
        }
        if (btQA == true)
        {
          if (DBConvert.ParseInt(ultData.Rows[j].Cells["QAClosed"].Value.ToString()) == 0)
          {
            ultData.Rows[j].Cells["QAClosed"].Activation = Activation.AllowEdit;
            ultData.Rows[j].Cells["QARemark"].Activation = Activation.AllowEdit;
          }
        }
      }

      //Set Color
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["CSClosed"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].Cells["QAClosed"].Value.ToString()) == 0)
          {
            ultData.Rows[i].Cells["No"].Appearance.BackColor = Color.White;
          }
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["QAClosed"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].Cells["CSClosed"].Value.ToString()) == 0)
          {
            ultData.Rows[i].Cells["No"].Appearance.BackColor = Color.GreenYellow;
          }
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["CSClosed"].Value.ToString()) == 0 && DBConvert.ParseInt(ultData.Rows[i].Cells["QAClosed"].Value.ToString()) == 0)
          {
            ultData.Rows[i].Cells["No"].Appearance.BackColor = Color.LightGreen;
          }
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["QAClosed"].Value.ToString()) == 1 && DBConvert.ParseInt(ultData.Rows[i].Cells["CSClosed"].Value.ToString()) == 1)
          {
            ultData.Rows[i].Cells["No"].Appearance.BackColor = Color.LightSalmon;
          }
      }
      //this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (row.Cells["SaleCode"].Appearance.BackColor == Color.Yellow)
        {
          return false ;
        }
        if (row.Cells["CustomerPid"].Appearance.BackColor == Color.Yellow)
        {
          return false ;
        }
        if (DBConvert.ParseLong(row.Cells["CustomerPid"].Value.ToString()) < 0)
        {
          row.Cells["CustomerPid"].Appearance.BackColor = Color.Yellow; 
          return false;
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      string storeName = "spCSDFinalCustomerComplainMaster_Update";
      int paramNumber = 4;
      DBParameter[] inputParam = new DBParameter[paramNumber];
      //if (this.transactionPid > 0)
      //{
        
      //}
      //else
      //{
      //  inputParam[0] = new DBParameter("@Pid", DbType.Int64, null);
      //}
      //int confirm;
      //if (chkConfirm.Checked)
      //{
      //  confirm = 1;
      //}
      //else
      //{
      //  confirm = 0;
      //}
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.viewPid);
      inputParam[1] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
      inputParam[2] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      //inputParam[3] = new DBParameter("@Confirm", DbType.Int16, confirm);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }
    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDFinalCustomerComplainDelete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ultData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[19];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
        
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          inputParam[1] = new DBParameter("@TransactionPid", DbType.Int64, this.viewPid);
          //inputParam[2] = new DBParameter("@QCDCode", DbType.String, row["QCDCode"].ToString());
          if (DBConvert.ParseString(row["CustomerPid"].ToString()) !="")
          {
            inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, DBConvert.ParseLong(row["CustomerPid"].ToString()));  
          }
          else
          {
            inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, null);
          }
          inputParam[3] = new DBParameter("@SaleCode", DbType.String, row["SaleCode"].ToString());
          if (DBConvert.ParseInt(row["Qty"].ToString()) > 0)
          {
            inputParam[4] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row["Qty"].ToString()));  
          }
          else
          {
            inputParam[4] = new DBParameter("@Qty", DbType.Int32, 0);
          }          
          inputParam[5] = new DBParameter("@ItemID", DbType.String, row["ItemID"].ToString());
          if (DBConvert.ParseDouble(row["ItemValue"].ToString()) >= 0)
          {
            inputParam[6] = new DBParameter("@ItemValue", DbType.Double , DBConvert.ParseDouble(row["ItemValue"].ToString()));
          }
          else
          {            
            inputParam[6] = new DBParameter("@ItemValue", DbType.Int64, 0);
          }
          if (DBConvert.ParseInt(row["InvoiceMonth"].ToString()) > 0)
          {
            inputParam[7] = new DBParameter("@InvoiceMonth", DbType.Int32, DBConvert.ParseInt(row["InvoiceMonth"].ToString()));  
          }
          else
          {
            inputParam[7] = new DBParameter("@InvoiceMonth", DbType.Int32, 0);
          }
          if (DBConvert.ParseInt(row["InvoiceYear"].ToString()) > 0)
          {
            inputParam[8] = new DBParameter("@InvoiceYear", DbType.Int32, DBConvert.ParseInt(row["InvoiceYear"].ToString()));  
          }
          else
          {
            inputParam[8] = new DBParameter("@InvoiceYear", DbType.Int32, 0);
          }
          if (DBConvert.ParseDouble(row["ClaimValue"].ToString()) > 0)
          {
            inputParam[9] = new DBParameter("@ClaimValue", DbType.Double, DBConvert.ParseDouble(row["ClaimValue"].ToString()));
          }
          else
          {
            inputParam[9] = new DBParameter("@ClaimValue", DbType.Int32, 0);
          }
          inputParam[10] = new DBParameter("@DetailOfComplaint", DbType.String, row["DetailOfComplaint"].ToString());
          if (DBConvert.ParseInt(row["ComplaintCategory"].ToString()) > 0 )
          {
            inputParam[11] = new DBParameter("@ComplaintCategory", DbType.Int32, DBConvert.ParseInt(row["ComplaintCategory"].ToString()));   
          }
          else
          {
            inputParam[11] = new DBParameter("@ComplaintCategory", DbType.Int32, null);
          }
         
          inputParam[12] = new DBParameter("@ImmediateActionCustomer", DbType.String, row["ImmediateActionCustomer"].ToString());
          inputParam[13] = new DBParameter("@PreventativeActionFactory", DbType.String, row["PreventativeActionFactory"].ToString());
          if (DBConvert.ParseInt(row["CSClosed"].ToString()) == 1)
          {
            inputParam[14] = new DBParameter("@CSClosed", DbType.Int32, DBConvert.ParseInt(row["CSClosed"].ToString()));
          }
          else
          {
            inputParam[14] = new DBParameter("@CSClosed", DbType.Int32, 0);
          }
          if (DBConvert.ParseInt(row["QAClosed"].ToString()) == 1)
          {
            inputParam[15] = new DBParameter("@QAClosed", DbType.Int32, DBConvert.ParseInt(row["QAClosed"].ToString()));
          }
          else
          {
            inputParam[15] = new DBParameter("@QAClosed", DbType.Int32, 0);
          }
          inputParam[16] = new DBParameter("@CSRemark", DbType.String, row["CSRemark"].ToString());
          inputParam[17] = new DBParameter("@QARemark", DbType.String, row["QARemark"].ToString());
          inputParam[18] = new DBParameter("@Location", DbType.String, row["Location"].ToString());
          DataBaseAccess.ExecuteStoreProcedure("spCSDFinalCustomerComplainDetail_Update", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    /// <summary>
    /// Save Upload File 
    /// </summary>
    /// <returns></returns>
    private bool SaveUploadFile()
    {
      //Copy File 
      //System.IO.File.Copy(sourseFile, destFile, true);

      string storeName = string.Empty;
      DataTable dtMain = (DataTable)this.ultUploadFile.DataSource;
      foreach (DataRow row in dtMain.Rows)       if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row.RowState == DataRowState.Added)
          {
            //Copy File
            System.IO.File.Copy(row["LocationFileLocal"].ToString(), row["LocationFile"].ToString(), true);
          }
          storeName = "spCSDFinalCustomerComplainUpload_Edit";
          DBParameter[] inputParam = new DBParameter[6];

          //Pid
          if (DBConvert.ParseLong(row["Pid"].ToString()) >= 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
         
          inputParam[1] = new DBParameter("@TranSactionDetailPid", DbType.Int64, this.viewPid);

          inputParam[2] = new DBParameter("@FileName", DbType.String, 512, row["FileName"].ToString());

          inputParam[3] = new DBParameter("@LocationFile", DbType.String, 512, row["LocationFile"].ToString());

          inputParam[4] = new DBParameter("@Remark", DbType.String, 4000, row["Remark"].ToString());

          inputParam[5] = new DBParameter("@File", DbType.String, row["File"].ToString());


          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      return true;
    }      
    
    /// <summary>
    /// Save Data
    /// </summary>
    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        if (this.SaveMain())
        {
          if (this.SaveDetail())
          {
            success = this.SaveUploadFile();
          }
          else
          {
            success = false;
          }
        }
        else
        {
          success = false;
        }
        
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
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
      e.Layout.AutoFitColumns = false;
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Bands[0].Summaries.Clear();
      
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Hide column
      e.Layout.Bands[0].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsStatus"].Hidden = true;

      // Set caption column 
      e.Layout.Bands[0].Columns["CustomerPid"].Header.Caption = "Customer Name";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["ItemValue"].Header.Caption = "Item Value";
      e.Layout.Bands[0].Columns["ClaimAmout"].Header.Caption = "Claim Amout";
      e.Layout.Bands[0].Columns["ClaimValue"].Header.Caption = "Claim Value";
      e.Layout.Bands[0].Columns["ComplaintCategory"].Header.Caption = "Complaint\nCategory";
      e.Layout.Bands[0].Columns["DetailOfComplaint"].Header.Caption = "Detail Of \nComplaint";
      e.Layout.Bands[0].Columns["ImmediateActionCustomer"].Header.Caption = "Immediate Action \nCustomer";
      e.Layout.Bands[0].Columns["PreventativeActionFactory"].Header.Caption = "Preventative Action \nFactory";
      //Set Caption
      e.Layout.Bands[0].Columns["CSClosed"].Header.Caption = "CS Closed";
      e.Layout.Bands[0].Columns["QAClosed"].Header.Caption = "QA Closed";
      e.Layout.Bands[0].Columns["CSRemark"].Header.Caption = "CS Remark";
      e.Layout.Bands[0].Columns["QARemark"].Header.Caption = "QA Remark";
      e.Layout.Bands[0].Columns["CSClosed"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["QAClosed"].DefaultCellValue = 0;
      // Set Column Style
      e.Layout.Bands[0].Columns["CSClosed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["QAClosed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CustomerPid"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Qty"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemID"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Location"].Header.Fixed = true;

      // Ready only
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Country"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Region"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CustomerGroup"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ClaimAmout"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ClaimAmout"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ClaimValue"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      //e.Layout.Bands[0].Columns["QCDCode"].CellAppearance.BackColor = Color.LightGreen;
      //if (chkConfirm.Checked || Confirm == 1)
      //{
      //  for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      //  {
      //    e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //  }
      //  this.EnableContrl();
      //}
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        else
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
        }
      }
      e.Layout.Bands[0].Columns["CustomerPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

       // Set dropdownlist for column
      e.Layout.Bands[0].Columns["CustomerPid"].ValueList = ultrDropCustomer;
      e.Layout.Bands[0].Columns["SaleCode"].ValueList = ultrDropSaleCode;
      e.Layout.Bands[0].Columns["ComplaintCategory"].ValueList = ultrDropComplaint;
      e.Layout.Bands[0].Columns["CustomerPid"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["CustomerPid"].MinWidth = 200;
      if (btQA == true)
      {
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      }
      if (btCS == true)
      {
        e.Layout.Bands[0].Columns["QAClosed"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["QARemark"].CellActivation = Activation.ActivateOnly;
      }
      
    }

    private void ultUploadFile_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultUploadFile);
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      // Hide column
      e.Layout.Bands[0].Columns["TranSactionDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFile"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFileLocal"].Hidden = true;
      e.Layout.Bands[0].Columns["File"].Hidden = true;

      e.Layout.Bands[0].Columns["Type"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Type"].MinWidth = 40;
      e.Layout.Bands[0].Columns["FileName"].MaxWidth = 300;
      e.Layout.Bands[0].Columns["FileName"].MinWidth = 300;

    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      switch (colName)
      {
        case "CustomerPid":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["CustomerPid"].Value.ToString()) > 0)
          {            
            e.Cell.Row.Cells["CustomerPid"].Appearance.BackColor = Color.White;
            e.Cell.Row.Cells["Country"].Value = ultrDropCustomer.SelectedRow.Cells["Country"].Value;
            e.Cell.Row.Cells["Region"].Value = ultrDropCustomer.SelectedRow.Cells["Region"].Value;
            e.Cell.Row.Cells["CustomerGroup"].Value = ultrDropCustomer.SelectedRow.Cells["CustomerGroup"].Value;
            if (DBConvert.ParseInt(e.Cell.Row.Cells["CSClosed"].Value.ToString()) == int.MinValue)
            {
              e.Cell.Row.Cells["CSClosed"].Value = 0;
            }
            if (DBConvert.ParseInt(e.Cell.Row.Cells["QAClosed"].Value.ToString()) == int.MinValue)
            {
              e.Cell.Row.Cells["QAClosed"].Value = 0;
            }
          }
          else
          {
            e.Cell.Row.Cells["CustomerPid"].Appearance.BackColor = Color.Yellow;
          }                   
          break;
        case "SaleCode":
          if (DBConvert.ParseInt(this.CheckExistingSaleCode(e.Cell.Row.Cells["SaleCode"].Value.ToString(), "SaleCode")) == 0)
          {
            e.Cell.Row.Cells["SaleCode"].Appearance.BackColor = Color.White;
            e.Cell.Row.Cells["ItemCode"].Value = ultrDropSaleCode.SelectedRow.Cells["ItemCode"].Value;
            e.Cell.Row.Cells["CarcassCode"].Value = ultrDropSaleCode.SelectedRow.Cells["CarcassCode"].Value;
          }
          else
          {
            e.Cell.Row.Cells["SaleCode"].Appearance.BackColor = Color.Yellow;
          }
          if (DBConvert.ParseInt(e.Cell.Row.Cells["CustomerPid"].Value.ToString()) < 0)
          {
            e.Cell.Row.Cells["CustomerPid"].Appearance.BackColor = Color.Yellow;
          }
          break;
        case "Qty":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["CustomerPid"].Value.ToString()) < 0)
          {
            e.Cell.Row.Cells["CustomerPid"].Appearance.BackColor = Color.Yellow;
          }
          e.Cell.Row.Cells["ClaimAmout"].Value = (DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value)> 0 ? DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value) : 0) * (DBConvert.ParseDouble(e.Cell.Row.Cells["ItemValue"].Value) > 0 ? DBConvert.ParseDouble(e.Cell.Row.Cells["ItemValue"].Value) : 0);
          break;
        case "ItemValue":
          e.Cell.Row.Cells["ClaimAmout"].Value = (DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value) > 0 ? DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value) : 0) * (DBConvert.ParseDouble(e.Cell.Row.Cells["ItemValue"].Value) > 0 ? DBConvert.ParseDouble(e.Cell.Row.Cells["ItemValue"].Value) : 0);
          break;

        default:
          break;
      }     
      this.SetNeedToSave();      
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
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
        int csClose = DBConvert.ParseInt(row.Cells["CSClosed"].Value.ToString());
        int qaClose = DBConvert.ParseInt(row.Cells["QAClosed"].Value.ToString());
        if (csClose == 0 && qaClose == 0)
        {
          if (pid != long.MinValue)
          {
            this.listDeletedPid.Add(pid);
          }
        }
        else
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0093"), "CS Close or QA Close");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      //this.SetNeedToSave();      
    }
    /// <summary>
    /// Get Customer Infromation
    /// </summary>
    /// <param name="Where"></param>
    /// <param name="Value"></param>
    /// <param name="Filed"></param>
    /// <returns></returns>
    private string GetInforCustomerOfGridview(string Where, string Value, string Filed)
    {
      DataTable dt = new DataTable();
      string fl = "";
      string sql = "";
      sql = "SELECT " + Filed + " FROM TblCSDCustomerInfo WHERE " + "CAST (" + Where + " AS VARCHAR(50))" + " = " + "'" + Value + "'";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql, 90);
      if (dt != null && dt.Rows.Count > 0)
      {
        fl = DBConvert.ParseString(dt.Rows[0][0].ToString());
      }
      else
      {
        fl = string.Empty;
      }
      return fl;
    }

    private string GetInforComplaintOfGridview(string Where, string Value, string Filed)
    {
      DataTable dt = new DataTable();
      string fl = "";
      string sql = "";
      sql = "SELECT " + Filed + " FROM TblBOMCodeMaster WHERE [Group] = 290316 AND  " + "CAST (" + Where + " AS VARCHAR(50))" + " = " + "'" + Value + "'";
      dt = DataBaseAccess.SearchCommandTextDataTable(sql, 90);
      if (dt != null && dt.Rows.Count > 0)
      {
        fl = DBConvert.ParseString(dt.Rows[0][0].ToString());
      }
      else
      {
        fl = string.Empty;
      }
      return fl;
    }
    /// <summary>
    /// Add datatable Mail
    /// </summary>
    /// <returns></returns>
    private DataTable GetDataTableEmail()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Customer", typeof(System.String));
      dt.Columns.Add("SaleCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("Location", typeof(System.String));
      dt.Columns.Add("Country", typeof(System.String));
      dt.Columns.Add("Region", typeof(System.String));
      dt.Columns.Add("ClaimAmout", typeof(System.String));
      dt.Columns.Add("DetailOfComplaint", typeof(System.String));
      dt.Columns.Add("ComplaintCategory", typeof(System.String));
      DataTable dtgr = (DataTable)ultData.DataSource;
      foreach (DataRow dr in dtgr.Rows)
      {
        DataRow row = dt.NewRow();
        row["Customer"] = this.GetInforCustomerOfGridview("Pid",dr["CustomerPid"].ToString(), "Name");
        row["SaleCode"] = dr["SaleCode"].ToString();
        row["ItemCode"] = dr["ItemCode"].ToString();
        row["CarcassCode"] = dr["CarcassCode"].ToString();
        row["Location"] = dr["Location"].ToString();
        row["Country"] = dr["Country"].ToString();
        row["Region"] = dr["Region"].ToString();
        row["ClaimAmout"] = dr["ClaimAmout"].ToString();
        row["DetailOfComplaint"] = dr["DetailOfComplaint"].ToString();
        row["ComplaintCategory"] = this.GetInforComplaintOfGridview("Code",dr["ComplaintCategory"].ToString(),"Value");
        
        dt.Rows.Add(row);
      }

      return dt;
    }
    private string GetAdressByEmail(string st)
    {
      DataTable dt = new DataTable();
      string fl = "";
      string sql = "";

      if (st == "1")
      {
        sql = "SELECT Value  FROM TblBOMCodeMaster WHERE [Group]  ='010616' AND Code = 1";
      }
      else
      {
        sql = "SELECT Value  FROM TblBOMCodeMaster WHERE [Group]  ='010616' AND Code = 2";
      }
      dt = DataBaseAccess.SearchCommandTextDataTable(sql, 90);
      if (dt != null && dt.Rows.Count > 0)
      {
        fl = DBConvert.ParseString(dt.Rows[0][0].ToString());
      }
      else
      {
        fl = string.Empty;
      }
      return fl;

    }
    private void btSendEmail_Click(object sender, EventArgs e)
    {
      DataTable dt =this.GetDataTableEmail();
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      MailMessage mailMessage = new MailMessage();            
      
      string mailTo = string.Empty;
      string mailCC = string.Empty;
      mailTo = this.GetAdressByEmail("1");
      mailCC = this.GetAdressByEmail("2");
      mailMessage.ServerName = "10.0.0.5";
      mailMessage.Username = "dc@daico-furniture.com";
      mailMessage.Password = "dc123456";
      mailMessage.From = "dc@daico-furniture.com";
      mailMessage.To = mailTo;
      mailMessage.Cc = mailCC;
      IList attachments = new ArrayList();
      mailMessage.Attachfile = attachments;

      mailMessage.Subject = "Customer Complaint";

      string tab = "\t";
      sb.AppendLine("<html>");
      sb.AppendLine(tab + "<body>");
      sb.AppendFormat("Have already issued:  {0}, Create Date: {1} , Create By: {2}  <b><b/>  <br/><br/>", txtTransactionCode.Text.ToString() + "  -- ", txtCreateDate.Text.ToString().Substring(0,10) +" -- ", txtCreateBy.Text.ToString());
      sb.AppendLine(tab + tab + "<table border= 1 style= width:100% >");

      // headers.
      sb.Append(tab + tab + tab + "<tr>");

      foreach (DataColumn dc in dt.Columns)
      {
        sb.AppendFormat("<td style=font-weight:bold bgcolor = GreenYellow> {0}</td>", dc.ColumnName);
      }

      sb.AppendLine("</tr>");

      // data rows
      foreach (DataRow dr in dt.Rows)
      {
        sb.Append(tab + tab + tab + "<tr>");

        foreach (DataColumn dc in dt.Columns)
        {
          string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
          sb.AppendFormat("<td> <font size=2> {0} </font></td>", cellValue);
        }

        sb.AppendLine("</tr>");
      }

      sb.AppendLine(tab + tab + "</table>");
      sb.AppendLine(tab + "</body>");
      sb.AppendLine("</html>");
      mailMessage.Body = sb.ToString();      
      mailMessage.SendMail(true);
      MessageBox.Show("Data Send Successfully !"); 

    }
    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "Data");
    }
    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtFile.Text.Trim().Length > 0);
    }

    private void btnGetTamplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_CSD_02_008_001";
      string sheetName = "csdcomplain";
      string outFileName = "RPT_CSD_02_008_001";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      int customerPid = 0;
      if (this.txtFile.Text.Trim().Length == 0)
      {
        return;
      }
      DataSet dsMaxRows = new DataSet();
      try
      {
        int maxRows = int.MinValue;
        try
        {
          dsMaxRows = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFile.Text.Trim(), string.Format(@"SELECT * FROM [csdcomplain (1)$M2:M3]"));     
        }
        catch
        {
           dsMaxRows = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFile.Text.Trim(), string.Format(@"SELECT * FROM [csdcomplain$M2:M3]"));
        }

        if (dsMaxRows != null && dsMaxRows.Tables.Count > 0)
        {
          maxRows = DBConvert.ParseInt(dsMaxRows.Tables[0].Rows[0][0].ToString());
        }

        if (maxRows > 0)
        {
          DataTable dtSource = new DataTable();
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFile.Text.Trim(), string.Format(@"SELECT * FROM [csdcomplain (1)$A4:L{0}]", maxRows + 3)).Tables[0];
            
          }
          catch
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFile.Text.Trim(), string.Format(@"SELECT * FROM [csdcomplain$A4:L{0}]", maxRows + 3)).Tables[0];
          }

          if (dtSource == null)
          {
            return;
          }
          DataTable dt = this.AddDataTable();
          foreach (DataRow dr in dtSource.Rows)
          {
            DataRow row = dt.NewRow();
            row["Pid"] = long.MinValue;
            row["TransactionPid"] = this.viewPid;
            //row["QCDCode"] = dr["QCDCode"].ToString();
            if (DBConvert.ParseString(dr["CustomerCode"].ToString()) != "")
            {
              row["CustomerPid"] = DBConvert.ParseLong(this.GetCustomerString(dr["CustomerCode"].ToString(), "Pid"));  
            }
            //else
            //{
            //  row["CustomerPid"] = DBConvert.ParseLong(this.GetCustomerString(dr["CustomerPid"].ToString(), "Pid"));
            //}
            row["SaleCode"] = dr["SaleCode"].ToString();
            if (dr["Qty"].ToString() != "")
            {
              row["Qty"] = DBConvert.ParseInt(dr["Qty"].ToString());
            }
            else
            {
              row["Qty"] = 0;
            }
            if (DBConvert.ParseString(dr["SaleCode"].ToString()) !="" )
            {
               row["IsStatus"] = DBConvert.ParseInt(this.CheckExistingSaleCode(dr["SaleCode"].ToString(), "SaleCode"));
               row["ItemCode"] = this.GetSaleCodeString(dr["SaleCode"].ToString(), "ItemCode");
               row["CarcassCode"] = this.GetSaleCodeString(dr["SaleCode"].ToString(), "CarcassCode");
            }
            else
            {
              row["IsStatus"] = DBConvert.ParseInt(this.CheckExistingSaleCode(dr["SaleCode"].ToString(), "SaleCode"));
            }
            row["ItemID"] = dr["ItemID"].ToString();
            if (DBConvert.ParseString(dr["CustomerCode"].ToString()) != "")
            {
              row["Country"] = DBConvert.ParseString(this.GetCustomerString(dr["CustomerCode"].ToString(), "Country"));
              row["Region"] = DBConvert.ParseString(this.GetCustomerString(dr["CustomerCode"].ToString(), "Region"));
              row["CustomerGroup"] = DBConvert.ParseString(this.GetCustomerString(dr["CustomerCode"].ToString(), "CustomerGroup"));
            }
            if (dr["ItemValue"].ToString() !="")
            {
              row["ItemValue"] = DBConvert.ParseDouble(dr["ItemValue"].ToString());
            }
            else
            {
              row["ItemValue"] = 0;
            }
            if (dr["InvoiceMonth"].ToString() !="")
            {
              row["InvoiceMonth"] = DBConvert.ParseInt(dr["InvoiceMonth"].ToString());
            }
            else
            {
              row["InvoiceMonth"] = 0;
            }
            if (dr["InvoiceYear"].ToString() !="")
            {
              row["InvoiceYear"] = DBConvert.ParseInt(dr["InvoiceYear"].ToString());
            }
            else
            {
              row["InvoiceYear"] = 0;
            }
            row["ClaimAmout"] = (dr["Qty"].ToString()!= "" ? DBConvert.ParseInt(dr["Qty"].ToString()): 0) * (dr["ItemValue"].ToString()!= "" ? DBConvert.ParseDouble(dr["ItemValue"].ToString()): 0);
            if (dr["ClaimValue"].ToString() != "")
            {
              row["ClaimValue"] = DBConvert.ParseInt(dr["ClaimValue"].ToString());
            }
            else
            {
              row["ClaimValue"] = 0;
            }
 
            row["DetailOfComplaint"] = dr["DetailOfComplaint"].ToString();
            if (dr["ComplaintCategory"].ToString() !="")
            {
              row["ComplaintCategory"] = DBConvert.ParseInt(dr["ComplaintCategory"].ToString()); 
            }
            else
            {
              row["ComplaintCategory"] = 0;
            }
            row["ImmediateActionCustomer"] = dr["ImmediateActionCustomer"].ToString();
            row["PreventativeActionFactory"] = dr["PreventativeActionFactory"].ToString();
       
            dt.Rows.Add(row);
          }
          DataTable dtultr = (DataTable)ultData.DataSource;
          if (dtultr != null)
          {
            foreach (DataRow drw in dtultr.Rows)
            {
              DataRow row = dt.NewRow();
              row["Pid"] = DBConvert.ParseLong(drw["Pid"].ToString());
              row["TransactionPid"] = DBConvert.ParseLong(drw["TransactionPid"].ToString());
              //row["QCDCode"] = drw["QCDCode"].ToString();
              row["CustomerPid"] = drw["CustomerPid"].ToString();
              row["SaleCode"] = drw["SaleCode"].ToString();
              if (drw["Qty"].ToString() !="")
              {
                row["Qty"] = DBConvert.ParseInt(drw["Qty"].ToString()); 
              }
              else
              {
                row["Qty"] = 0;
              }
              row["ItemCode"] = drw["ItemCode"].ToString();
              row["CarcassCode"] = drw["CarcassCode"].ToString();
              row["ItemID"] = drw["ItemID"].ToString();
              row["Country"] = drw["Country"].ToString();
              row["Region"] = drw["Region"].ToString();
              row["CustomerGroup"] = drw["CustomerGroup"].ToString();
              if (drw["ItemValue"].ToString() !="")
              {
                row["ItemValue"] = DBConvert.ParseDouble(drw["ItemValue"].ToString());
              }
              else
              {
                row["ItemValue"] = 0;
              }
              if (drw["ClaimAmout"].ToString() != "")
              {
                row["ClaimAmout"] = DBConvert.ParseDouble(drw["ClaimAmout"].ToString());
              }
              else
              {
                row["ClaimAmout"] = 0;
              }
              if (drw["ClaimValue"].ToString() != "")
              {
                row["ClaimValue"] = DBConvert.ParseDouble(drw["ClaimValue"].ToString());
              }
              else
              {
                row["ClaimValue"] = 0;
              }
              if (drw["InvoiceMonth"].ToString() !="")
              {
                row["InvoiceMonth"] = DBConvert.ParseInt(drw["InvoiceMonth"].ToString());  
              }
              else
              {
                row["InvoiceMonth"] = 0;
              }
              if (drw["InvoiceYear"].ToString() !="")
              {
                row["InvoiceYear"] = DBConvert.ParseInt(drw["InvoiceYear"].ToString());
              }
              else
              {
                row["InvoiceYear"] = 0;
              }
              row["DetailOfComplaint"] = drw["DetailOfComplaint"].ToString();
              if (drw["ComplaintCategory"].ToString() !="")
              {
                row["ComplaintCategory"] = DBConvert.ParseInt(drw["ComplaintCategory"].ToString());
              }
              else
              {
                row["ComplaintCategory"] = 0;
              }
              row["ImmediateActionCustomer"] = drw["ImmediateActionCustomer"].ToString();
              row["PreventativeActionFactory"] = drw["PreventativeActionFactory"].ToString();
              row["IsStatus"] = drw["IsStatus"].ToString();
              row["CSClosed"] = drw["CSClosed"].ToString();
              row["CSRemark"] = drw["CSRemark"].ToString();
              row["QARemark"] = drw["QARemark"].ToString();
              dt.Rows.Add(row);
            }
          }
          ultData.DataSource = dt;
        }
      }
      catch (Exception ex)
      {
        //MessageBox.Show("Can't Import data to List !");       
      }
      this.CheckListDatabase();
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
    /// <summary>
    /// Browse Upload
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrowseUpload_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnUpload.Enabled = (txtLocation.Text.Trim().Length > 0);
    }
    /// <summary>
    /// Add file upload
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnUpload_Click(object sender, EventArgs e)
    {
      if (this.txtLocation.Text.Trim().Length > 0)
      {
        string file = txtLocation.Text;
        FileInfo f = new FileInfo(file);
        long fLength = f.Length;
        //if (fLength < 5120000)
        //{
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
              this.chkUpload.Checked = true;
            //}
            //else
            //{
            //  WindowUtinity.ShowMessageError("ERR0001", "Type File Not UPload");
            //}
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", "File Upload < 5Mb");
        }
      }
    }
    /// <summary>
    /// Double Clock
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultUploadFile_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultUploadFile.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultUploadFile.Selected.Rows[0];
      Process prc = new Process();

      if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) == int.MinValue)
      {
        prc.StartInfo = new ProcessStartInfo(row.Cells["LocationFileLocal"].Value.ToString());
      }
      else
      {
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string folder = string.Format(@"{0}\Temporary", startupPath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
        string locationFile = row.Cells["LocationFile"].Value.ToString();
        if (File.Exists(locationFile))
        {
          string newLocationFile = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["LocationFile"].Value.ToString()));
          if (File.Exists(newLocationFile))
          {
            try
            {
              File.Delete(newLocationFile);
            }
            catch
            {
              WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
              return;
            }
          }
          File.Copy(locationFile, newLocationFile);
          prc.StartInfo = new ProcessStartInfo(newLocationFile);
        }
      }
      try
      {
        prc.Start();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }
    /// <summary>
    /// Check Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkUpload_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkUpload.Checked)
      {
        this.ultUploadFile.Visible = true;
      }
      else
      {
        this.ultUploadFile.Visible = false;
      }
    }

    /// <summary>
    /// Delete row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultUploadFile_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.viewPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@UploadPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            string storeName = string.Empty;
            storeName = "spCSDFinalCustomerComplainUpload_Delete";
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
            if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
    }
    #endregion Event



    


  }
}