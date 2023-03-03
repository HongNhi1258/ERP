using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_012 : MainUserControl
  {
    private bool loadingFlg = false;
    private string listDeleteCodeMstKey = string.Empty;
    public int group;
    private DataTable dtSource;
    public viewCSD_01_012()
    {
      InitializeComponent();
    }
    private void viewCSD_01_012_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + DaiCo.Shared.Utility.SharedObject.UserInfo.UserName + " | " + DaiCo.Shared.Utility.SharedObject.UserInfo.LoginDate;
      this.loadingFlg = true;
      Shared.Utility.ControlUtility.LoadComboboxMasterName(cmbGroup);
      this.loadingFlg = false;
      try
      {
        cmbGroup.SelectedValue = group;
      }
      catch { }
      this.LoadGrid();
    }
    private void LoadGrid()
    {
      DBParameter[] inputParam = new DBParameter[1];
      this.group = DBConvert.ParseInt(cmbGroup.SelectedValue.ToString());
      inputParam[0] = new DBParameter("@Group", DbType.Int32, group);
      this.dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListCodeMasterByGroup", inputParam);
      GVCodeMst.DataSource = this.dtSource;
      GVCodeMst.Columns["Code"].Visible = false;
    }
    private void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!this.loadingFlg)
      {
        this.LoadGrid();
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      int count = this.dtSource.Rows.Count;
      bool successful = true;
      int result;
      DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      for (int i = 0; i < count; i++)
      {
        DataRow dtRow = this.dtSource.Rows[i];
        DBParameter[] inputParams = new DBParameter[7];
        if (dtRow.RowState == DataRowState.Added || dtRow.RowState == DataRowState.Modified)
        {
          inputParams[0] = new DBParameter("@Group", DbType.Int32, this.group);          
          inputParams[2] = new DBParameter("@Value", DbType.String, 1024, dtRow["Value"].ToString());
          inputParams[3] = new DBParameter("@DeleteFlag", DbType.Int32, 0);
          inputParams[4] = new DBParameter("@Sort", DbType.Int32, DBConvert.ParseInt(dtRow["Sort"].ToString()));
          inputParams[5] = new DBParameter("@Description", DbType.String, 512, dtRow["Description"].ToString());

          if (dtRow.RowState == DataRowState.Added)
          {
            inputParams[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spBOMCodeMaster_Insert", inputParams, outputParams);
            result = DBConvert.ParseInt(outputParams[0].Value.ToString());
            if (result == 0)
            {
              successful = false;
            }
          }
          else
          {
            inputParams[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            inputParams[1] = new DBParameter("@Code", DbType.Int32, DBConvert.ParseInt(dtRow["Code"].ToString()));
            DataBaseAccess.ExecuteStoreProcedure("spBOMCodeMaster_Update", inputParams, outputParams);
            result = DBConvert.ParseInt(outputParams[0].Value.ToString());
            if (result == 0)
            {
              successful = false;
            }
          }
        }
      }
      if (this.listDeleteCodeMstKey.Length > 0)
      {
        string[] deleteKeys = this.listDeleteCodeMstKey.Split('|');
        foreach (string deleteKey in deleteKeys)
        {
          if (deleteKey.Length > 1)
          {
            int group = DBConvert.ParseInt(deleteKey.Split('@')[0]);
            int code = DBConvert.ParseInt(deleteKey.Split('@')[1]);
            if (group != int.MinValue && code != int.MinValue)
            {
              DBParameter[] inputDeleteParams = new DBParameter[2];
              inputDeleteParams[0] = new DBParameter("@Group", DbType.Int32, group);
              inputDeleteParams[1] = new DBParameter("@Code", DbType.Int32, code);
              DataBaseAccess.ExecuteStoreProcedure("spBOMCodeMaster_Delete", inputDeleteParams, outputParams);
              result = DBConvert.ParseInt(outputParams[0].Value.ToString());
              if (result != 1)
              {
                successful = false;
              }
            }
          }
        }
      }
      if (successful)
      {
        MessageBox.Show("Save data successful!");
      }
      else
      {
        MessageBox.Show("Error!");
      }
      this.LoadGrid();
    }

    private void GVCodeMst_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
    {
      if (listDeleteCodeMstKey.Length > 0)
      {
        listDeleteCodeMstKey += "|";
      }
      listDeleteCodeMstKey += this.group.ToString() + "@" + this.GVCodeMst.Rows[e.Row.Index].Cells["Code"].Value.ToString();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
  }
}