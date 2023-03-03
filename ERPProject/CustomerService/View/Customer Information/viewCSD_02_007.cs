/*
  Author      : Huynh Thi Bang
  Date        : 22/12/2015
  Description : Customer Contact info
  Standard Form: viewCSD_02_007
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

namespace DaiCo.ERPProject
{
  public partial class viewCSD_02_007 : MainUserControl
  {
    #region Field
    private long customerPid = long.MinValue;
    private long jobPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public viewCSD_02_007()
    {
      InitializeComponent();
    }

    private void viewCSD_02_007_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data    
      this.InitData();
    }
    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadCustomer()
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode, Name, (CustomerCode + ' | ' + Name) DisplayText 
                                           FROM TblCSDCustomerInfo 
                                           WHERE DeletedFlg = 0 
                                           AND ParentPid IS NULL 
                                           ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBCustomer, dtSource, "Pid", "DisplayText", false, "Pid");     
      //ultCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    /// <summary>
    /// Load CB Job
    /// </summary>
    private void LoadJob()
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 23");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBJob, dtSource, "Code", "Value", "Code");
      ultCBJob.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBJob.DisplayLayout.AutoFitColumns = true;
    }

    #endregion Init

    #region Function
    
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
      this.LoadCustomer();
      this.LoadJob();
    }
    //Check Search
    private bool CheckValid()
    {
      //check Customer
      if (ultCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Customer");
        return false;
      }
      //check Job
      if (ultCBJob.Value == null)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Job");
        return false;
      }
        return true;
    }
    //Check Save data
    private bool CheckValidSaveData()
    {
      //Check on grid
      //DataTable dtDetail = (DataTable)ultData.DataSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        //check name
        if (row.Cells["Name"].Value.ToString().Length == 0)
        {
          WindowUtinity.ShowMessageError("MSG0005", "Name");
          return false;
        }
        if (row.Cells["Email1"].Value.ToString().Length == 0)
        {
          WindowUtinity.ShowMessageError("MSG0005", "Email1");
          return false;
        }
      }
      return true;
    }

    private void SaveData()
    {
      if (this.CheckValidSaveData())
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDCustomerContact_Delete", deleteParam, outputParam);
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
            DBParameter[] inputParam = new DBParameter[12];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            if (DBConvert.ParseInt(row["Select"].ToString()) == 0 || row["Select"].ToString()== string.Empty)
            {
              inputParam[1] = new DBParameter("@IsSelect", DbType.Int32, 0);
            }
            else
            {
              inputParam[1] = new DBParameter("@IsSelect", DbType.Int32, 1);
            }
            inputParam[2] = new DBParameter("@Name", DbType.String, row["Name"].ToString());
            inputParam[3] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
            if(DBConvert.ParseString(row["Gender"].ToString())!= null)
            {
              inputParam[4] = new DBParameter("@Gender", DbType.String, row["Gender"].ToString());
            }
            if(DBConvert.ParseString(row["Position"].ToString())!= "")
            {
              inputParam[5] = new DBParameter("@Position", DbType.String, row["Position"].ToString());
            }
            inputParam[6] = new DBParameter("@Email1", DbType.String, 100, row["Email1"].ToString());
            if(DBConvert.ParseString(row["Email2"].ToString())!= "")
            {
             inputParam[7] = new DBParameter("@Email2", DbType.String, 100, row["Email2"].ToString());
            }
            inputParam[8] = new DBParameter("@JobPid", DbType.Int32, this.jobPid);
            if (DBConvert.ParseString(row["ExtNumber"].ToString()) != "")
            {
              inputParam[9] = new DBParameter("@ExtNumber", DbType.Int32, row["ExtNumber"].ToString());
            }
            if (DBConvert.ParseString(row["CellPhone"].ToString()) != "")
            {
              inputParam[10] = new DBParameter("@CellPhone", DbType.Int32, row["CellPhone"].ToString());
            }
            if (DBConvert.ParseString(row["Skype"].ToString()) != "")
            {
              inputParam[11] = new DBParameter("@SkypeId", DbType.String, row["Skype"].ToString());
            }

            DataBaseAccess.ExecuteStoreProcedure("spCSDCustomerContact_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
          
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.Search();
          this.NeedToSave = false;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    /// <summary>
    /// Search Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Search()
    {
      if (this.CheckValid())
      {
        if (ultCBCustomer.Value != null || ultCBJob.Value != null)
        {
          this.customerPid = DBConvert.ParseInt(ultCBCustomer.Value);
          this.jobPid = DBConvert.ParseInt(ultCBJob.Value);
        }
        else
        {
          this.customerPid = long.MinValue;
          this.jobPid = long.MinValue;
          return;
        }
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
        inputParam[1] = new DBParameter("@JobPid", DbType.Int64, this.jobPid);
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCustomerContactInfo_Search", inputParam);
        if (dtSource != null)
        {
          ultData.DataSource = dtSource;
        }
        lbCustomer.Text = string.Format("Customer Contact of: {0}, Job: {1}", ultCBCustomer.Text, ultCBJob.Text);
      }
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      //checkbox
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      for (int j = 0; j < e.Layout.Bands[0].Columns.Count - 1; j++)
      {
        //e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Select"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["Select"].CellAppearance.BackColor = Color.LightGray;
      }

    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
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
    //Search
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();

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
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    #endregion Event

  }
}
