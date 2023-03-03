using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Shared;
using Infragistics.Win;
using VBReport;
using DaiCo.Shared.DataBaseUtility;
using System.Diagnostics;
using DaiCo.Shared.Utility;
using DaiCo.CustomerService.DataSetSource;
using DaiCo.Shared.DataSetSource.CustomerService;
using DaiCo.Shared.DataSetSource;
using System;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_02_006 : MainUserControl
  {
    public viewCSD_02_006()
    {
      InitializeComponent();
    }
    private void LoadCmbGroupCode()
    {
      string commandText = string.Format(@"SELECT Pid, GroupCode+' - '+ GroupName GroupName FROM tblCSDCustomerGroup WHERE ISNULL(Active,0) = 0");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcmbGroupCode.DataSource = dtSource;
      ultcmbGroupCode.DisplayLayout.AutoFitColumns = true;
      ultcmbGroupCode.DisplayMember = "GroupName";
      ultcmbGroupCode.ValueMember = "Pid";
      ultcmbGroupCode.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }
    private void LoadDropdownGroupCode()
    {
      string commandText = string.Format(@"SELECT Pid, GroupCode+' - '+ GroupName GroupName FROM tblCSDCustomerGroup WHERE ISNULL(Active,0) = 0");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultdrGroupCode.DataSource = dt;
      DataRow dr = dt.NewRow();
      dt.Rows.InsertAt(dr, 0);
      ultdrGroupCode.DisplayMember = "GroupName";
      ultdrGroupCode.ValueMember = "Pid";
      ultdrGroupCode.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultdrGroupCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    private void btnSearch_Click(object sender, EventArgs e)
    {
      DBParameter[] param = new DBParameter[3];
      string storeName = "spCSDCustomerInfo_AddGroup";
      if (txtCustomerCode.Text.Trim().Length > 0)
      {
        param[0] = new DBParameter("@CustomerCode", DbType.AnsiString, 30, txtCustomerCode.Text.Trim() );
      }
      if (txtCustomerName.Text.Trim().Length > 0)
      {
        param[1] = new DBParameter("@CustomerName", DbType.AnsiString, 200, txtCustomerName.Text.Trim());
      }
      if (ultcmbGroupCode.Text.Trim().Length > 0)
      {
        param[2] = new DBParameter("@GroupCodePid", DbType.AnsiString, 32, ultcmbGroupCode.Value.ToString());
      }
      DataSet dsource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      if (dsource != null)
      {
        ultCustomerInformaition.DataSource = dsource.Tables[0];
      }
    }

    private void ultCustomerInformaition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultCustomerInformaition);
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerCode"].Header.Caption  = "Customer Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Customer Name";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Customer Name VN";
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["GroupName"].ValueList = ultdrGroupCode;
      e.Layout.Bands[0].Columns["GroupName"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["GroupName"].MinWidth = 100;
      for (int i = 0; i < ultCustomerInformaition.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = ultCustomerInformaition.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if (string.Compare(columnName, "Group Name", true) == 0 || string.Compare(columnName, "Selected", true) == 0)
        {
          ultCustomerInformaition.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          ultCustomerInformaition.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
    }

    private void viewCSD_02_006_Load(object sender, EventArgs e)
    {
      LoadCmbGroupCode();
      LoadDropdownGroupCode();
    }

    private void btnAddGroup_Click(object sender, EventArgs e)
    {
      DBParameter[] param = new DBParameter[4];
      if (txtGroupcode.Text.Trim().Length > 0)
      {
        param[0] = new DBParameter("@GroupCode", DbType.AnsiString, 20, txtGroupcode.Text.Trim());
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005","Group Code");
        return;
      }
      if (txtGroupName.Text.Trim().Length > 0)
      {
        param[1] = new DBParameter("@GroupName", DbType.AnsiString, 20, txtGroupName.Text.Trim());
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "Group Name");
        return;
      }
      if (chkActive.Checked)
      {
        param[2] = new DBParameter("@Active", DbType.Int32, 1);
      }
      else
      {
        param[2] = new DBParameter("@Active", DbType.Int32, 0);
      }
      param[3] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      DBParameter[] outparam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDCustomerGroup_Insert", param, outparam);
      long output = DBConvert.ParseLong(outparam[0].Value.ToString().Trim());
      if (output == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
      }
      else if (output == -1)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0028","Group Code "+txtGroupcode.Text.Trim());
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      this.LoadCmbGroupCode();
      this.LoadDropdownGroupCode();
    }

    private void ultcmbGroupCode_ValueChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultCustomerInformaition.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultCustomerInformaition.Rows[i].Cells["Selected"].Value.ToString()) == 1)
        {
          ultCustomerInformaition.Rows[i].Cells["GroupName"].Value = ultcmbGroupCode.Value;
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dt = (DataTable)ultCustomerInformaition.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified)
        {
          DBParameter[] param = new DBParameter[2];
          param[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));
          param[1] = new DBParameter("@GroupCodePid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["GroupName"].ToString()));
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDCustomerInfo_UpdateGroupCode", param);
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      btnSearch_Click(sender, e);
    }
  }
}
