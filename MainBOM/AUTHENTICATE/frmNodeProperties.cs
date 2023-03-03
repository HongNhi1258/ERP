/*
  Author  : Nguyen Phuoc Vinh
  Email   : vinh_it@daico-furniture.com
  Date    : 01-11-2010
  Company : Dai Co 
*/
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;
using System.Windows.Forms;

namespace MainBOM.AUTHENTICATE
{
  public partial class frmNodeProperties : Form
  {
    public long UIPid = long.MinValue;
    public long UIparentPid = long.MinValue;
    public frmNodeProperties()
    {
      InitializeComponent();
    }

    private void LoadData()
    {
      if (this.UIPid > 0)
      {
        string cmt = "SELECT * FROM TblGNRDefineUI WHERE Pid = " + UIPid.ToString();
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmt);
        if (dt.Rows.Count > 0)
        {
          txtUICode.Text = DBConvert.ParseString(dt.Rows[0]["UICode"]);
          txtUIParam.Text = DBConvert.ParseString(dt.Rows[0]["UIParam"]);
          if (txtUIParam.Text.Trim().Length > 0)
            txtUIParam.Enabled = false;
          txtTitle.Text = DBConvert.ParseString(dt.Rows[0]["Title"]);
          try
          {
            cmbNameSpace.SelectedItem = DBConvert.ParseString(dt.Rows[0]["NameSpace"]);
          }
          catch { }
          int viewState = DBConvert.ParseInt(dt.Rows[0]["ViewState"]);
          int windowState = DBConvert.ParseInt(dt.Rows[0]["WindowState"]);
          cmbViewState.SelectedIndex = viewState > 0 ? viewState : -1;
          cmbWindowState.SelectedIndex = windowState >= 0 ? windowState + 1 : 0;
          txtDescription.Text = DBConvert.ParseString(dt.Rows[0]["Description"]);
          txtOrtherInfo.Text = DBConvert.ParseString(dt.Rows[0]["OtherInfo"]);
          txtOderBy.Text = DBConvert.ParseString(dt.Rows[0]["OrderBy"]);
          chxIsActive.Checked = (dt.Rows[0]["IsActive"] == DBNull.Value || ((int)dt.Rows[0]["IsActive"]) == 0) ? false : true;
        }
      }
      else
      {
        btnDelete.Enabled = false;
        chxIsActive.Checked = true;
      }
    }

    /// <summary>
    /// Load form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmNodeProperties_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// Combobox Viewtate selectechange
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbViewState_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cmbViewState.SelectedIndex == 0 || cmbViewState.SelectedIndex == 1)
      {
        cmbWindowState.SelectedIndex = 0;
        cmbWindowState.Enabled = false;
      }
      else
        cmbWindowState.Enabled = true;
    }

    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (CheckValid())
      {
        GNRDefineUI ui = new GNRDefineUI();
        ui.UICode = txtUICode.Text.Trim();
        ui.UIParam = txtUIParam.Text.Trim();
        ui.Title = txtTitle.Text.Trim();
        if (cmbNameSpace.SelectedIndex > 0)
          ui.NameSpace = cmbNameSpace.SelectedItem.ToString();
        if (cmbViewState.SelectedIndex > 0)
          ui.ViewState = cmbViewState.SelectedIndex;
        if (cmbWindowState.SelectedIndex > 0)
          ui.WindowState = cmbWindowState.SelectedIndex - 1;
        ui.Description = txtDescription.Text.Trim();
        ui.OtherInfo = txtOrtherInfo.Text.Trim();
        int orderBy = DBConvert.ParseInt(txtOderBy.Text.Trim());
        if (orderBy >= 0)
          ui.OrderBy = orderBy;
        ui.IsActive = chxIsActive.Checked ? 1 : 0;
        ui.ParentPid = UIparentPid;

        if (UIPid > 0)
        {
          ui.Pid = UIPid;
          bool result = DaiCo.Shared.DataBaseUtility.DataBaseAccess.UpdateObject(ui, new string[] { "Pid" });
          if (result)
          {
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0001");
            this.LoadData();
          }
          else
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0002");
        }
        else
        {
          long result = DaiCo.Shared.DataBaseUtility.DataBaseAccess.InsertObject(ui);
          if (result > 0)
          {
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0003");
            this.UIPid = result;
            this.LoadData();
          }
          else
          {
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0003");
          }
        }
      }
    }

    /// <summary>
    /// Check valid
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      if (txtTitle.Text.Trim().Length == 0)
      {
        return false;
      }
      else if (txtUIParam.Text.Trim().Length > 0)
      {
        if (!CheckUniqueUIParam(txtUICode.Text.Trim(), txtUIParam.Text.Trim(), this.UIPid))
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0058");
          txtUIParam.Focus();
          return false;
        }
        else
        {
          return true;
        }
      }
      else
      {
        return true;
      }
    }
    private bool CheckUniqueUIParam(string UICode, string param, long UIPid)
    {
      string cmdCheck = string.Format("SELECT COUNT(*) FROM TblGNRDefineUI WHERE UICode = '{0}' AND UIParam = '{1}'", UICode, param);
      int count = (int)DataBaseAccess.ExecuteScalarCommandText(cmdCheck);

      if (UIPid > 0 && count > 1)
      {
        return false;
      }
      else if (UIPid < 0 && count >= 1)
      {
        return false;
      }
      return true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (DialogResult.Yes == DaiCo.Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0007", txtTitle.Text))
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@UIPid", DbType.Int64, this.UIPid) };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spGNRDefineUI_Delete", inputParam, outputParam);
        int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
        switch (result)
        {
          case -1:
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0093", string.Format("{0}. {1}", txtTitle.Text, "You have to delete sub node first"));
            break;
          case 0:
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
            break;
          case 1:
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0002");
            this.Close();
            break;
        }
      }
    }
  }
}
