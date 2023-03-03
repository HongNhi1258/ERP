/*
  Author      : 
  Date        : 
  Description : 
  Standard Code: view_MasterDetail.cs
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
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_028 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int checkDCConfirm = 0;
    private bool isConfirm = false;
    #endregion Field

    #region Init

    public viewPLN_02_028()
    {
      InitializeComponent();
    }

    private void viewPLN_02_028_Load(object sender, EventArgs e)
    {
      isConfirm = btnConfirm.Visible;
      this.LoadInit();
      this.LoadData();
      chkConfirm.Visible = btnConfirm.Visible;
      //txtWo.Enabled = !isConfirm;
      //txtRemark.Enabled = !isConfirm;
      //txtSubName.Enabled = !isConfirm;
      this.panel1.Visible = false;
      txtID.Text = this.pid.ToString();
    }

    #endregion Init

    #region Function

    private void LoadInit()
    {
      this.LoadSupplier();
    }

    private void LoadData()
    {
      if (this.pid > 0)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@WoCarcassContractOutPid", DbType.Int64, this.pid) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNWOCarcassContractOut_Select", inputParam);
        if (dsSource != null)
        {
          DataTable dtInfo = dsSource.Tables[0];
          if (dtInfo.Rows.Count > 0)
          {
            DataRow row = dtInfo.Rows[0];
            this.status = DBConvert.ParseInt(row["Confirm"].ToString());
            txtWo.Text = row["WoPid"].ToString();
            if (DBConvert.ParseDateTime(row["CreateDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) > DateTime.MinValue)
            {
              ultdCreatedDate.Value = DBConvert.ParseDateTime(row["CreateDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            this.txtCreatedBy.Text = row["CreatedBy"].ToString();
            this.txtRemark.Text = row["Remark"].ToString();
            this.txtSubName.Text = row["SubconName"].ToString();
            if (this.status > 0)
            {
              chkConfirm.Checked = true;
              this.SetStatusControl(true);
            }
          }
          if (dsSource.Tables[2] != null && dsSource.Tables[2].Rows.Count > 0)
          {
            Shared.Utility.ControlUtility.LoadUltraCombo(ultcSubName, dsSource.Tables[2], "Pid", "DisplayText", "DisplayText");
          }
          // Load Detail
          ultData.DataSource = dsSource.Tables[1];
          try
          {
            string carcass = "";
            int colorFlag = 0;
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              if (i == 0)
              {
                ultData.Rows[i].Cells["CarcassCode"].Appearance.BackColor = Color.White;
                colorFlag = 0;
              }
              else
              {
                if (carcass != ultData.Rows[i].Cells["CarcassCode"].Value.ToString())
                {
                  if (colorFlag == 0)
                  {
                    ultData.Rows[i].Cells["CarcassCode"].Appearance.BackColor = Color.SkyBlue;
                    colorFlag = 1;
                  }
                  else
                  {
                    ultData.Rows[i].Cells["CarcassCode"].Appearance.BackColor = Color.White;
                    colorFlag = 0;
                  }
                }
              }
              carcass = ultData.Rows[i].Cells["CarcassCode"].Value.ToString();
            }
          }
          catch
          { }
        }
      }
      else
      {
        this.ultdCreatedDate.Value = DateTime.Now;
        this.txtCreatedBy.Text = SharedObject.UserInfo.EmpName;
      }
      if (this.status > 0)
      {
        btnDelete.Visible = false;
      }
    }

    private void SetStatusControl(bool Confirm)
    {
      if (Confirm)
      {
        btnSave.Visible = false;
        chkConfirm.Enabled = false;

        txtWo.Enabled = false;
        txtRemark.Enabled = false;
        txtSubName.Enabled = false;
      }
    }

    private void LoadSupplier()
    {
      string commandText = string.Format(@"SELECT Pid, ID_NhaCC, TenNhaCCVN, TenNhaCCEN, (ID_NhaCC + ' - ' + ISNULL(TenNhaCCVN, '')) DisplayText FROM VWHFSupplier Order By ID_NhaCC");
      DataTable dtSupplier = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraCombo(ultcSubName, dtSupplier, "Pid", "DisplayText", "DisplayText");
    }

    private bool CheckVaild()
    {
      string message = string.Empty;

      // Check Info
      if (txtWo.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Wo");
        return false;
      }

      //string ExitsWo = DataBaseAccess.ExecuteScalarCommandText("SELECT COUNT(Pid) FROM TblPLNWoCarcassContractOutInfo where WoPid = " + txtWo.Text.Trim() + " and Pid <> " + this.pid.ToString()).ToString();
      //if (ExitsWo != "0")
      //{
      //  WindowUtinity.ShowMessageError("ERR0006", "Wo");
      //  return false;
      //}
      //// Check Detail
      //if (chkConfirm.Checked)
      //{
      //  for (int i = 0; i < ultData.Rows.Count; i++)
      //  {
      //    if (DBConvert.ParseDateTime(ultData.Rows[i].Cells["DCConfirm"].Value) <= DateTime.MinValue)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0029", "Daico Confirm Date");
      //      return false;
      //    }
      //  }
      //}
      return true;
    }

    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      return success;
    }

    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[7];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }
      inputParam[1] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(txtWo.Text));
      inputParam[2] = new DBParameter("@Confirm", DbType.Int64, chkConfirm.Checked ? 1 : 0);
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@Remark", DbType.String, 256, this.txtRemark.Text);
      }
      inputParam[4] = new DBParameter("@AdjustBy", DbType.Int64, SharedObject.UserInfo.UserPid);

      string DCConfrim = string.Empty; ;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string carcassCode = string.Empty;
        string dcDate = string.Empty;
        string netPrice = string.Empty;
        string SubSupplierPid = string.Empty;
        string RemarkMakeSample = string.Empty;
        string SamplePrice = string.Empty;
        string ContractOut = string.Empty;

        carcassCode = ultData.Rows[i].Cells["CarcassCode"].Value.ToString();
        if (ultData.Rows[i].Cells["DCConfirm"].Value.ToString().Length > 0)
        {
          dcDate = DBConvert.ParseDateTime(ultData.Rows[i].Cells["DCConfirm"].Value).ToString("dd-MMM-yyyy");
          //dcDate = DBConvert.ParseDateTime(ultData.Rows[i].Cells["DCConfirm"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME).ToString();
        }
        else
        {
          dcDate = "NULL";
        }
        if (DBConvert.ParseDouble(ultData.Rows[i].Cells["NetPrice"].Value.ToString()) > 0)
        {
          netPrice = ultData.Rows[i].Cells["NetPrice"].Value.ToString();
        }
        else
        {
          netPrice = "NULL";
        }
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["SubSupplierPid"].Value.ToString()) > 0)
        {
          SubSupplierPid = ultData.Rows[i].Cells["SubSupplierPid"].Value.ToString();
        }
        else
        {
          SubSupplierPid = "NULL";
        }
        if (ultData.Rows[i].Cells["Remark Make Sample"].Value.ToString().Trim().Length > 0)
        {
          RemarkMakeSample = ultData.Rows[i].Cells["Remark Make Sample"].Value.ToString();
        }
        else
        {
          RemarkMakeSample = "NULL";
        }
        if (DBConvert.ParseDouble(ultData.Rows[i].Cells["Sample Price"].Value.ToString()) > 0)
        {
          SamplePrice = ultData.Rows[i].Cells["Sample Price"].Value.ToString();
        }
        else
        {
          SamplePrice = "NULL";
        }
        if (ultData.Rows[i].Cells["ContractOut"].Value.ToString().Trim().Length > 0)
        {
          ContractOut = ultData.Rows[i].Cells["ContractOut"].Value.ToString();
        }
        else
        {
          ContractOut = "NULL";
        }
        DCConfrim += carcassCode + "^" + dcDate + "^" + netPrice + "^" + SubSupplierPid + "^" + RemarkMakeSample + "^" + SamplePrice + "^" + ContractOut + "#";

        //DCConfrim += ultData.Rows[i].Cells["CarcassCode"].Value.ToString() + "^" + 
        //        //DBConvert.ParseDateTime(ultData.Rows[i].Cells["DCConfirm"].Value).ToString("dd/MM/yyyy") + "^" +
        //        DBConvert.ParseDouble(ultData.Rows[i].Cells["NetPrice"].Value.ToString()).ToString() + "^" +
        //        DBConvert.ParseLong(ultData.Rows[i].Cells["SubSupplierPid"].Value.ToString()).ToString() + "#";
      }
      inputParam[5] = new DBParameter("@DCConfirmParam", DbType.String, 4000, DCConfrim);

      if (txtSubName.Text.Trim().Length > 0)
      {
        inputParam[6] = new DBParameter("@SubName", DbType.String, 256, this.txtSubName.Text);
      }

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNWoCarcassContractOutInfo_Edit", inputParam, ouputParam);
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.pid <= 0)
      {
        return false;
      }
      return true;
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["DCConfirm"].Header.Caption = "Required\n Delivery Date";
      e.Layout.Bands[0].Columns["NetPrice"].Header.Caption = "Net Price";
      e.Layout.Bands[0].Columns["DeliveryDate"].Header.Caption = "Subcon\n Delivery Date";
      e.Layout.Bands[0].ColHeaderLines = 2;
      for (int i = 0; i < ultData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        if ((ultData.DisplayLayout.Bands[0].Columns[i].Header.Caption != "Required\n Delivery Date" && ultData.DisplayLayout.Bands[0].Columns[i].Header.Caption != "Net Price" && ultData.DisplayLayout.Bands[0].Columns[i].Header.Caption != "Remark Make Sample" && ultData.DisplayLayout.Bands[0].Columns[i].Header.Caption != "Sample Price"))
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.White;
        }
        Type colType = ultData.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      if (chkConfirm.Checked)
      {
        e.Layout.Bands[0].Columns["DCConfirm"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["NetPrice"].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["WoPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SubSupplierPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Price Of Material Supply"].Header.Caption = "Material Supply";
      e.Layout.Bands[0].Columns["GrossPrice"].Header.Caption = "Gross Price";
      e.Layout.Bands[0].Columns["Other Price"].Header.Caption = "Other";
      e.Layout.Bands[0].Columns["NetPrice"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Price Of Material Supply"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Other Price"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["GrossPrice"].CellAppearance.BackColor = Color.Yellow;
      //e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Supplier Name"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Default"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      if (chkConfirm.Checked == false)
      {
        e.Layout.Bands[0].Columns["DCConfirm"].CellAppearance.BackColor = Color.LightBlue;
        e.Layout.Bands[0].Columns["NetPrice"].CellAppearance.BackColor = Color.LightBlue;
      }
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Bands[0].Columns["Price Of Material Supply"].Format = "###,###";
      e.Layout.Bands[0].Columns["NetPrice"].Format = "###,###";
      e.Layout.Bands[0].Columns["GrossPrice"].Format = "###,###";
      e.Layout.Bands[0].Columns["Other Price"].Format = "###,###";
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      if (this.CheckVaild())
      {
        // Save Data
        bool success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }

        //Load Data
        this.LoadData();
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      DataTable dt;
      switch (columnName)
      {
        case "dcconfirm":
          try
          {

            if (checkDCConfirm == 1)
            {
              return;
            }
            string strCarcassCode = e.Cell.Row.Cells["CarcassCode"].Value.ToString();
            DateTime DCConfrim = DBConvert.ParseDateTime(e.Cell.Row.Cells["DCConfirm"].Value);
            if (DCConfrim > DateTime.MinValue)
            {
              for (int i = 0; i < ultData.Rows.Count; i++)
              {
                if (ultData.Rows[i].Cells["CarcassCode"].Value.ToString() == strCarcassCode)
                {
                  checkDCConfirm = 1;
                  ultData.Rows[i].Cells["DCConfirm"].Value = DCConfrim;
                }
              }
            }
            checkDCConfirm = 0;
          }
          catch
          {
          }
          break;

        default:
          break;
      }
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0)
      {
        string commandText = "SELECT [Confirm] FROM TblPLNWoCarcassContractOutInfo WHERE Pid = " + this.pid + "";
        DataTable dtCheckConfirm = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheckConfirm != null && dtCheckConfirm.Rows.Count > 0 && DBConvert.ParseInt(dtCheckConfirm.Rows[0]["Confirm"].ToString()) == 0)
        {
          long WoPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["WoPid"].Value.ToString());
          string CarcassCode = ultData.Selected.Rows[0].Cells["CarcassCode"].Value.ToString();
          viewPLN_02_030 view = new viewPLN_02_030();
          view.WoPid = WoPid;
          view.CarcassCode = CarcassCode;
          Shared.Utility.WindowUtinity.ShowView(view, "SUMMARY COMPARISION PRICE", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
          //Load Data
          this.LoadData();
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[1];
      if (this.pid > 0)
      {
        inputParam[0] = new DBParameter("@WoCarcassContractOutPid", DbType.Int64, this.pid);
      }
      else
      {
        return;
      }
      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNWoCarcassContractOutInfo_Delete", inputParam, ouputParam);
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.pid <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0004", "");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
        this.CloseTab();
      }
    }
    #endregion Event   

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowImage.Checked)
      {
        grpBoxCarcassCode.Visible = true;
      }
      else
      {
        grpBoxCarcassCode.Visible = false;
      }
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (chkShowImage.Checked == true)
      {
        try
        {
          grpBoxCarcassCode.Visible = true;
          string carcassCode = ultData.Selected.Rows[0].Cells["CarcassCode"].Value.ToString().Trim();
          if (carcassCode.Length > 0)
          {
            picCarcassCode.ImageLocation = FunctionUtility.BOMGetCarcassImage(carcassCode);
            grpBoxCarcassCode.Text = string.Format("{0}", carcassCode);
          }
        }
        catch { }
      }
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      bool result = true;
      result = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = '" + this.pid.ToString() + "' WHERE [Group] = 16023 AND Code = 4", null);
      if (result)
      {
        Process.Start(DataBaseAccess.ExecuteScalarCommandText("SELECT [Description] FROM TblBOMCodeMaster WHERE [Group] = 16023 AND Code = 5", null).ToString());
      }
    }
  }
}
