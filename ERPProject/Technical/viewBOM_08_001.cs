/*
  Author      : 
  Date        : 
  Description : 
  Standard Code: view_MasterDetail.cs
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_08_001 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedPid = new ArrayList();
    private bool isDuplicateProcess = false;
    #endregion Field

    #region Init

    public viewBOM_08_001()
    {
      InitializeComponent();
    }

    private void viewBOM_08_001_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
      this.SetStatusControl();
    }

    #endregion Init

    #region Function

    private void LoadInit()
    {
      this.LoadUltraDDMaterial();
      this.LoadUltraCBColor();
    }

    //private void LoadUltraComboDepartment(UltraCombo ultraCBDepartment)
    //{
    //  string commandText = "SELECT Department, DeparmentName, Department + ' | ' + DeparmentName Display FROM VHRDDepartment ORDER BY Department";
    //  DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //  ControlUtility.LoadUltraCombo(ultraCBDepartment, dtSource, "Department", "Display", "Display");
    //}

    private void LoadUltraDDMaterial()
    {
      string commandText = "SELECT MaterialCode, MaterialName, Unit, (MaterialCode + ' | ' + MaterialName) DisplayText FROM VBOMMaterialsForFinishingInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBMaterial, dtSource, "MaterialCode", "DisplayText", false, "DisplayText");
      ultCBMaterial.DisplayLayout.AutoFitStyle = AutoFitStyle.None;
      ultCBMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].Width = 80;
      ultCBMaterial.DisplayLayout.Bands[0].Columns["MaterialName"].Width = 300;
      ultCBMaterial.DisplayLayout.Bands[0].Columns["Unit"].Width = 50;
    }

    private void LoadUltraCBColor()
    {
      string commandText = "SELECT Pid, ColorCode FROM TblBOMColor";
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultcbChemicalCopy, dataSource, "Pid", "ColorCode", "Pid");
      ultcbChemicalCopy.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMColorLoadData", inputParam);
      if (dsSource != null)
      {
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          this.status = DBConvert.ParseInt(row["Confirm"].ToString());
          txtCreateBy.Text = row["CreateBy"].ToString();
          txtCreateDate.Text = row["CreateDate"].ToString();
          txtColor.Text = row["ColorCode"].ToString();
          txtName.Text = row["Name"].ToString();
          //Code
          //Code
          if (this.status > 0)
          {
            chkConfirm.Checked = true;
          }
        }
        else
        {
          txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          txtColor.Focus();
        }

        // Load Detail
        ultData.DataSource = dsSource.Tables[1];
        // Set Status Control
        this.SetStatusControl();
      }
    }

    private void SetStatusControl()
    {
      txtCreateBy.ReadOnly = true;
      txtCreateDate.ReadOnly = true;
      if (this.status > 0)
      {
        txtColor.ReadOnly = true;
        txtName.ReadOnly = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Activation = Activation.ActivateOnly; 
        }
        //ultData.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
        //ultData.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
      }
    }

    private void CheckMaterialDuplicate()
    {
      isDuplicateProcess = false;
      for (int x = 0; x < ultData.Rows.Count; x++)
      {
        UltraGridRow rowcurentA1 = ultData.Rows[x];
        for (int z = x + 1; z < ultData.Rows.Count; z++)
        {
          UltraGridRow rowcurentB1 = ultData.Rows[z];
          rowcurentA1.CellAppearance.BackColor = Color.White;
          rowcurentB1.CellAppearance.BackColor = Color.White;
        }
      }
      
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowcurentA = ultData.Rows[i];
        string materialCodeA = rowcurentA.Cells["MaterialCode"].Value.ToString();
        for (int j = i + 1; j < ultData.Rows.Count; j++)
        {
          UltraGridRow rowcurentB = ultData.Rows[j];
          string materialCodeB = rowcurentB.Cells["MaterialCode"].Value.ToString();
          if (string.Compare(materialCodeA, materialCodeB) == 0)
          {
            rowcurentA.CellAppearance.BackColor = Color.Yellow;
            rowcurentB.CellAppearance.BackColor = Color.Yellow;
            isDuplicateProcess = true;
          }
        }
      }
    }

    private bool CheckVaild(out string message)
    {
      message = string.Empty;

      if (this.isDuplicateProcess)
      {
        message = "MaterialCode is duplicate";
        return false;
      }
      
      if (txtColor.Text.Trim().Length > 0)
      {
        if (this.pid < 0)
        {
          string cm = String.Format(@"SELECT ColorCode
                                      FROM TblBOMColor
                                      WHERE ColorCode = '{0}'", txtColor.Text.Trim());
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
          if (dt.Rows.Count > 0 && dt != null)
          {
            message = "Color Code has been exists";
            return false;
          }
        }
      }
      else
      {
        message = "Color Code";
        return false;
      }

      if (txtName.Text.Trim().Length == 0)
      {
        message = "Description";
        return false;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string cm = string.Format(@"SELECT MaterialCode FROM VBOMMaterialsForFinishingInfo WHERE MaterialCode = '{0}'", ultData.Rows[i].Cells["MaterialCode"].Value.ToString().Trim());
        DataTable dtMaterial = DataBaseAccess.SearchCommandTextDataTable(cm);
        if (dtMaterial.Rows.Count == 0 || dtMaterial == null)
        //if (ultData.Rows[i].Cells["MaterialCode"].Value.ToString().Trim().Length == 0)
        {
          message = "MaterialCode";
          return false;
        }
        if (DBConvert.ParseDouble(ultData.Rows[i].Cells["Qty"].Value.ToString()) < 0)
        {
          message = "Qty";
          return false;
        }
      }
      // Check Info

      // Check Detail

      return true;
    }

    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      if (success)
      {
        // Save detail
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }

    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[5];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }

      if (txtColor.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@ColorCode", DbType.String, txtColor.Text.Trim());
      }

      if (txtName.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@Name", DbType.String, txtName.Text.Trim());
      }

      if (chkConfirm.Checked)
      {
        inputParam[3] = new DBParameter("@Confirm", DbType.Int32, 1);
      }
      else
      {
        inputParam[3] = new DBParameter("@Confirm", DbType.Int32, 0);
      }

      inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spBOMColor_Edit", inputParam, ouputParam);
      // Gan Lai Pid
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if(this.pid == long.MinValue)
      {
        return false;
      }
      return true;
    }

    private bool SaveDetail()
    {
      // Delete Row In grid
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spBOMColor_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      // Save Detail
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if(row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[5];
          if(DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }

          if (this.pid != long.MinValue)
          {
            inputParam[1] = new DBParameter("@ColorPid", DbType.Int64, this.pid);
          }

          if (row["MaterialCode"].ToString().Trim().Length > 0)
          {
            inputParam[2] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString().Trim());
          }

          if (DBConvert.ParseDouble(row["Qty"].ToString()) != double.MinValue)
          {
            inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row["Qty"].ToString()));
          }

          if (DBConvert.ParseDouble(row["Consumption"].ToString()) != double.MinValue)
          {
            inputParam[4] = new DBParameter("@Consumption", DbType.Double, DBConvert.ParseDouble(row["Consumption"].ToString()));
          }

          DBParameter[] outPutParam = new DBParameter[1];
          outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spBOMColorDetail_Edit", inputParam, outPutParam);
          if(DBConvert.ParseLong(outPutParam[0].Value.ToString()) == long.MinValue)
          {
            return false;
          }
        }
      }
      // End
      return true;
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ColorPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Consumption"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialRemark"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ultCBMaterial;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["MaterialCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["MaterialCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material";      
      e.Layout.Bands[0].Columns["MaterialRemark"].Header.Caption = "Material Remark";
      e.Layout.Bands[0].Columns["Consumption"].Header.Caption = "Estimated\n Consumption (g/m2)";

      e.Layout.Bands[0].Columns["MaterialCode"].Width = 400;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "materialcode":
          if (ultCBMaterial.SelectedRow != null)
          {
            e.Cell.Row.Cells["Unit"].Value = ultCBMaterial.SelectedRow.Cells["Unit"].Value.ToString();
          }
          else
          {
            e.Cell.Row.Cells["Unit"].Value = DBNull.Value;
          }
          this.CheckMaterialDuplicate();
          break;
        default:
          break;
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString();
      //string text = e.Cell.Text.Trim();
      //switch (columnName.ToLower())
      //{
      //  case "qty":
      //    if (text.Length > 0)
      //    {
      //      if (DBConvert.ParseDouble(text) <= 0)
      //      {
      //        WindowUtinity.ShowMessageError("ERR0001", "Qty");
      //        e.Cancel = true;
      //      }
      //    }
      //    break;
      //  default:
      //    break;
      //}
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
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckVaild(out message);
      if(!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
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

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.CheckMaterialDuplicate();
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (ultcbChemicalCopy.SelectedRow != null)
      {
        this.NeedToSave = true;
        DataTable dtSource = (DataTable)ultData.DataSource;
        DataTable dtProcess = new DataTable();
        long colorPid = DBConvert.ParseInt(ultcbChemicalCopy.Value.ToString());
        if (colorPid != long.MinValue)
        {
          DBParameter[] inputParamProcess = new DBParameter[] { new DBParameter("@Pid", DbType.String, colorPid) };
          DataSet ds = DataBaseAccess.SearchStoreProcedure("spBOMColorLoadData", inputParamProcess);
          if (ds != null)
          {
            dtProcess = (DataTable)(ds.Tables[1]);
          }
        }
        foreach (DataRow drRow in dtProcess.Rows)
        {
          DataRow row = dtSource.NewRow();
          row["ColorPid"] = this.pid;
          row["MaterialCode"] = drRow["MaterialCode"];
          row["MaterialRemark"] = drRow["MaterialRemark"];
          row["Unit"] = drRow["Unit"];
          row["Qty"] = drRow["Qty"];
          row["Consumption"] = drRow["Consumption"];
          dtSource.Rows.Add(row);
        }
      }
    }
    #endregion Event
  }
}
