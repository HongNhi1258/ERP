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
using DaiCo.Shared.Utility;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_011 : MainUserControl
  {
    public viewCSD_01_011()
    {
      InitializeComponent();
    }
    private void viewCSD_01_011_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + DaiCo.Shared.Utility.SharedObject.UserInfo.UserName + " | " + DaiCo.Shared.Utility.SharedObject.UserInfo.LoginDate;
      this.Search();
    }
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[4];
      if (SharedObject.UserInfo.UserPid == Shared.Utility.ConstantClass.UserAddmin)
      {
        inputParam[0] = new DBParameter("@Admin", DbType.Int32, 1);
      }
      else
      {
        string text = txtNameEN.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[0] = new DBParameter("@NameEn", DbType.AnsiString, 66, "%" + text.Replace("'", "''") + "%");
        }
        text = txtNameVN.Text.Trim();
        if (text.Length > 0)
        {
          inputParam[1] = new DBParameter("@NameVn", DbType.String, 258, "%" + text.Replace("'", "''") + "%");
        }
        inputParam[2] = new DBParameter("@UserCode", DbType.Int32, SharedObject.UserInfo.UserPid);
        inputParam[3] = new DBParameter("@GroupMaster", DbType.Int32, DaiCo.Shared.Utility.ConstantClass.GROUP_ROLE);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListMasterName", inputParam);
      this.GVGroup.DataSource = dtSource;
      this.GVGroup.Columns[0].Visible = false;
      this.GVGroup.Columns["NameEn"].HeaderText = "EN Name";
      this.GVGroup.Columns["NameVn"].HeaderText = "VN Name";
    }    
    private void btnSreach_Click(object sender, EventArgs e)
    {
      this.Search();
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void GVGroup_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex >= 0)
      {
        viewCSD_01_012 view = new viewCSD_01_012();
        view.group = DBConvert.ParseInt(GVGroup.Rows[e.RowIndex].Cells["Group"].Value.ToString());
        WindowUtinity.ShowView(view, "Master Information", false, ViewState.Window);
      }
    }
  }
}