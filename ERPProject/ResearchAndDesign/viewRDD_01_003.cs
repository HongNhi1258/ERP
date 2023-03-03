using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewRDD_01_003 : MainUserControl
  {
    private bool loaddingItemCode = false;
    public string itemCode = string.Empty;
    private DataTable dataSource;
    private IList listPidDeleted = new ArrayList();
    private IList listPidDeleting = new ArrayList();
    private bool isDuplicateCompo = false;
    private bool canUpdate = false;

    public viewRDD_01_003()
    {
      InitializeComponent();
    }

    private void UC_RDDListItemComponent_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserName + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;
      this.listPidDeleted = new ArrayList();
      this.LoadDropDownRDDItemCode();
      this.LoadDropdownComponentCode();
      this.LoadComboItemReference();
      this.LoadComGroup();

      if (this.itemCode.Length > 0)
      {
        try
        {
          ultItemCode.Value = this.itemCode;
          this.LoadData();
        }
        catch { }
      }
      else
      {
        btnSave.Enabled = false;
        this.canUpdate = false;
      }
    }

    #region InitData

    private void LoadDropDownRDDItemCode()
    {
      this.loaddingItemCode = true;
      string commandText = string.Format(@"SELECT ItemCode, Name FROM TblRDDItemInfo ORDER BY ItemCode DESC");
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultItemCode, dtSource, "ItemCode", "ItemCode");
      this.loaddingItemCode = false;
    }
    //Tien Add
    private void LoadComGroup()
    {
      string cmText = string.Empty;
      if (ultraCbItemReference.Value == null)
      {
        cmText = string.Format(@" SELECT Code,Value 
                                  FROM TblBOMCodeMaster 
                                  WHERE [Group] = 9
                                  AND Code IN (1,2,4,5,6)");

      }
      else
      {
        string itemcode = ultraCbItemReference.SelectedRow.Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(ultraCbItemReference.SelectedRow.Cells["Revision"].Value.ToString());
        cmText = string.Format(@" SELECT DISTINCT Code,Value
                                  FROM  TblBOMCodeMaster CMT
	                                    INNER JOIN TblBOMItemComponent ITC	ON CMT.Code = ITC.CompGroup
										                                                  AND CMT.[Group] = 9
	                                    INNER JOIN TblBOMItemInfo ITF ON ITC.ItemCode = ITF.ItemCode
								                                                    AND ITC.Revision = ITF.Revision
                                      WHERE ITF.ItemCode = '{0}' AND ITF.Revision = {1}", itemcode, revision);
      }
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cmText);
      Utility.LoadUltraCombo(ultraCbComGroup, dtSource, "Code", "Value", "Code");
      ultraCbComGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    //Tien Add
    private void GetDataComponentReference()
    {
      string storeName = string.Empty;
      if (ultraCbItemReference.Value != null)
      {
        storeName = "spRDDGetDataComponentReference";
        string itemrefer = ultraCbItemReference.SelectedRow.Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(ultraCbItemReference.SelectedRow.Cells["Revision"].Value.ToString());
        DBParameter[] input = new DBParameter[3];
        input[0] = new DBParameter("@ItemCode", DbType.String, itemrefer);
        input[1] = new DBParameter("@Revision", DbType.Int32, revision);
        if (ultraCbComGroup.Value != null)
        {
          input[2] = new DBParameter("@ComGroup", DbType.Int32, DBConvert.ParseInt(ultraCbComGroup.Value.ToString()));
        }
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, input);
        if (dtSource != null)
        {
          DataTable dtSourceOnGrid = (DataTable)ultListComponent.DataSource;
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            DataRow row = dtSourceOnGrid.NewRow();
            row["DisplayComponentCode"] = dtSource.Rows[i]["DisplayComponentCode"];
            row["ComponentCode"] = dtSource.Rows[i]["ComponentCode"];
            row["Revision"] = dtSource.Rows[i]["Revision"];
            row["Name"] = dtSource.Rows[i]["Name"];
            row["Qty"] = dtSource.Rows[i]["Qty"];
            row["Length"] = dtSource.Rows[i]["Length"];
            row["Width"] = dtSource.Rows[i]["Width"];
            row["Thickness"] = dtSource.Rows[i]["Thickness"];
            row["CompGroup"] = dtSource.Rows[i]["CompGroup"];
            row["Group"] = dtSource.Rows[i]["Group"];
            dtSourceOnGrid.Rows.Add(row);
          }
          lbRowcount.Text = this.CountRowOnGird();
          this.CheckComponentDuplicate();
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0125", "Item Reference");
      }
    }
    //Tien Add
    private void CheckComponentDuplicate()
    {
      isDuplicateCompo = false;
      for (int k = 0; k < ultListComponent.Rows.Count; k++)
      {
        ultListComponent.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultListComponent.Rows.Count; i++)
      {
        string comcode = ultListComponent.Rows[i].Cells["ComponentCode"].Value.ToString();
        for (int j = i + 1; j < ultListComponent.Rows.Count; j++)
        {
          string comcodeCompare = ultListComponent.Rows[j].Cells["ComponentCode"].Value.ToString();
          if (string.Compare(comcode, comcodeCompare, true) == 0)
          {
            ultListComponent.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultListComponent.Rows[j].CellAppearance.BackColor = Color.Yellow;
            this.isDuplicateCompo = true;
          }
        }
      }
    }
    //Tien Add
    private string CountRowOnGird()
    {
      int rows = ultListComponent.Rows.Count;
      string rowcount = string.Format("Count: {0}", rows);
      return rowcount;
    }

    private void LoadDropdownComponentCode()
    {
      string commandText = string.Format(@"SELECT Code + '|' + ISNULL(CAST(Revision AS VARCHAR), '') Value, 
                                                  Code, Revision, Name, NameVn, Length, Width, Thickness, CompGroup, [Group],
                                                  (Code + ' | ' + Name) DisplayText
                                          FROM VRDDComponent
                                          WHERE CompGroup != 999 
                                          ORDER BY CompGroup, Code, Revision");
      DataTable dtCompList = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBComponentCode, dtCompList, "Value", "DisplayText", true, new string[] { "CompGroup", "Value", "DisplayText" });
      ultraCBComponentCode.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["Code"].Width = 80;
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["Revision"].Width = 60;
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["NameVn"].Width = 300;
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["NameVn"].Header.Caption = "Vietnamese Name";
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["Length"].Width = 50;
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["Width"].Width = 50;
      ultraCBComponentCode.DisplayLayout.Bands[0].Columns["Thickness"].Width = 60;
    }

    private void LoadGrid()
    {
      // Data
      DBParameter[] param = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode) };
      this.dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spRDDListItemComponentInfo", param);
      this.dataSource.Columns["ComponentCode"].AllowDBNull = false;
      ultListComponent.DataSource = this.dataSource;
      lbRowcount.Text = this.CountRowOnGird();
    }

    private void LoadData()
    {
      this.listPidDeleted = new ArrayList();
      RDDItemInfo item = new RDDItemInfo();
      item.ItemCode = this.itemCode;
      item = (RDDItemInfo)Shared.DataBaseUtility.DataBaseAccess.LoadObject(item, new string[] { "ItemCode" });
      if (item != null)
      {
        txtSaleCode.Text = item.SaleCode;
        image.ImageLocation = Shared.Utility.FunctionUtility.RDDGetItemImage(item.ItemCode);
        txtName.Text = item.Name;
        txtItemSize.Text = string.Format("{0} X {1} X {2}", DBConvert.ParseString(item.WidthDefault), DBConvert.ParseString(item.DepthDefault), DBConvert.ParseString(item.HighDefault));
        txtCBM.Text = DBConvert.ParseString(item.CBM);
        if (item.KD == 0)
        {
          rbtNo.Select();
        }
        else
        {
          rbtYes.Select();
        }
        if (item.Confirm != 0)
        {
          btnSave.Enabled = false;
        }
        else
        {
          btnSave.Enabled = true;
        }
        this.canUpdate = (btnSave.Enabled) && (btnSave.Visible);
      }
      else
      {
        txtSaleCode.Text = string.Empty;
        txtName.Text = string.Empty;
        txtItemSize.Text = string.Empty;
        txtCBM.Text = string.Empty;
        btnSave.Enabled = false;
        this.canUpdate = false;
      }
      this.LoadGrid();
      this.NeedToSave = false;
    }

    #endregion InitData

    #region Process

    private bool CheckComponentCode(string code)
    {
      string strCommandText = " SELECT dbo.FRDDCheckComponent(@Code)";
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 50, code) };
      return ((int)Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) >= 1);
    }

    private DBParameter[] GetParamater(DataRow dtRow)
    {
      DBParameter[] param = new DBParameter[10];

      // Pid
      long pid = DBConvert.ParseLong(dtRow["Pid"].ToString());
      if (pid != long.MinValue)
      {
        param[0] = new DBParameter("@Pid", DbType.Int64, pid);
      }

      // Item Code
      param[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);

      // ComponentCode
      param[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, dtRow["ComponentCode"].ToString().Trim());

      // Component Revision
      int revision = DBConvert.ParseInt(dtRow["Revision"].ToString().Trim());
      if (revision != int.MinValue)
      {
        param[3] = new DBParameter("@CompRevision", DbType.Int32, revision);
      }

      // Component Group
      int group = DBConvert.ParseInt(dtRow["CompGroup"].ToString().Trim());
      if (group != int.MinValue && group != 999)
      {
        param[4] = new DBParameter("@CompGroup", DbType.Int32, group);
      }

      // Qty
      double value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
      if (value != double.MinValue)
      {
        param[5] = new DBParameter("@Qty", DbType.Double, value);
      }

      // Length
      value = DBConvert.ParseDouble(dtRow["Length"].ToString().Trim());
      if (value != double.MinValue)
      {
        param[6] = new DBParameter("@Length", DbType.Double, value);
      }

      // Width
      value = DBConvert.ParseDouble(dtRow["Width"].ToString());
      if (value != double.MinValue)
      {
        param[7] = new DBParameter("@Width", DbType.Double, value);
      }

      // Thickness
      value = DBConvert.ParseDouble(dtRow["Thickness"].ToString());
      if (value != double.MinValue)
      {
        param[8] = new DBParameter("@Thickness", DbType.Double, value);
      }

      if (dtRow.RowState == DataRowState.Added)
      {
        param[9] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      else
      {
        param[9] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      return param;
    }

    private bool CheckInvalid()
    {
      for (int i = 0; i < ultListComponent.Rows.Count; i++)
      {
        double qty = DBConvert.ParseDouble(ultListComponent.Rows[i].Cells["Qty"].Value.ToString());
        if (qty == double.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", string.Format("Qty at row {0}", i + 1));
          return false;
        }
      }
      return true;
    }

    private bool SaveData()
    {
      bool result = true;
      int outputResult = 0;
      // 1. Delete
      foreach (long pid in this.listPidDeleted)
      {
        DBParameter[] inputParamDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spRDDItemComponent_Delete", inputParamDelete, OutputParamDelete);
        outputResult = DBConvert.ParseInt(OutputParamDelete[0].Value.ToString());
        if (outputResult == 0)
        {
          result = false;
          break;
        }
      }

      // 2. Insert/Update
      string storeName = string.Empty;
      foreach (DataRow dtRow in this.dataSource.Rows)
      {
        storeName = string.Empty;
        if (dtRow.RowState == DataRowState.Added)
        {
          storeName = "spRDDItemComponent_Insert";
        }
        else if (dtRow.RowState == DataRowState.Modified)
        {
          // Update
          storeName = "spRDDItemComponent_Update";
        }
        if (storeName.Length > 0)
        {
          DBParameter[] inputParamDetail = this.GetParamater(dtRow);
          DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
          outputResult = DBConvert.ParseInt(outputParamDetail[0].Value.ToString());
          if (outputResult == 0)
          {
            result = false;
            break;
          }
        }
      }
      return result;
    }

    #endregion Process

    #region Others Event

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.isDuplicateCompo)
        {
          WindowUtinity.ShowMessageError("ERR0013", "Component Code");
          return;
        }
        bool success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
        this.LoadData();
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.SaveSuccess = true;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
        this.SaveSuccess = false;
        this.LoadData();
      }
    }

    #endregion Others Event

    #region UltraGrid Event

    private void ultListComponent_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      this.listPidDeleting = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listPidDeleting.Add(pid);
        }
      }
    }

    private void ultListComponent_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = true;
      this.CheckComponentDuplicate();
      lbRowcount.Text = this.CountRowOnGird();
      foreach (long pid in this.listPidDeleting)
      {
        this.listPidDeleted.Add(pid);

      }
    }

    private void ultListComponent_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "displaycomponentcode":
          string code = string.Empty;
          if (ultraCBComponentCode.SelectedRow != null)
          {
            code = ultraCBComponentCode.SelectedRow.Cells["Code"].Value.ToString();
          }
          bool isComponent = this.CheckComponentCode(code);
          if (!isComponent)
          {
            MessageBox.Show(string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "ComponentCode"));
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultListComponent_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "displaycomponentcode":
          try
          {
            ultListComponent.Rows[index].Cells["Name"].Value = ultraCBComponentCode.SelectedRow.Cells["Name"].Value;
          }
          catch
          {
            ultListComponent.Rows[index].Cells["Name"].Value = DBNull.Value;
          }
          try
          {
            ultListComponent.Rows[index].Cells["Group"].Value = ultraCBComponentCode.SelectedRow.Cells["Group"].Value;
          }
          catch
          {
            ultListComponent.Rows[index].Cells["Group"].Value = DBNull.Value;
          }
          try
          {
            ultListComponent.Rows[index].Cells["ComponentCode"].Value = ultraCBComponentCode.SelectedRow.Cells["Code"].Value;
          }
          catch
          {
            ultListComponent.Rows[index].Cells["ComponentCode"].Value = DBNull.Value;
          }
          int group = int.MinValue;
          try
          {
            group = DBConvert.ParseInt(ultraCBComponentCode.SelectedRow.Cells["CompGroup"].Value.ToString());
            ultListComponent.Rows[index].Cells["CompGroup"].Value = group;
          }
          catch
          {
            ultListComponent.Rows[index].Cells["CompGroup"].Value = DBNull.Value;
          }
          if (group != 999)
          {
            ultListComponent.Rows[index].Cells["Length"].Activation = Activation.ActivateOnly;
            ultListComponent.Rows[index].Cells["Width"].Activation = Activation.ActivateOnly;
            ultListComponent.Rows[index].Cells["Thickness"].Activation = Activation.ActivateOnly;

            try
            {
              ultListComponent.Rows[index].Cells["Length"].Value = ultraCBComponentCode.SelectedRow.Cells["Length"].Value;
            }
            catch
            {
              ultListComponent.Rows[index].Cells["Length"].Value = DBNull.Value;
            }

            try
            {
              ultListComponent.Rows[index].Cells["Width"].Value = ultraCBComponentCode.SelectedRow.Cells["Width"].Value;
            }
            catch
            {
              ultListComponent.Rows[index].Cells["Width"].Value = DBNull.Value;
            }

            try
            {
              ultListComponent.Rows[index].Cells["Thickness"].Value = ultraCBComponentCode.SelectedRow.Cells["Thickness"].Value;
            }
            catch
            {
              ultListComponent.Rows[index].Cells["Thickness"].Value = DBNull.Value;
            }

            try
            {
              ultListComponent.Rows[index].Cells["Revision"].Value = ultraCBComponentCode.SelectedRow.Cells["Revision"].Value;
            }
            catch
            {
              ultListComponent.Rows[index].Cells["Revision"].Value = DBNull.Value;
            }
            //Tien Add
            this.CheckComponentDuplicate();
            //Tien add
            lbRowcount.Text = this.CountRowOnGird();
          }
          else
          {
            ultListComponent.Rows[index].Cells["Length"].Activation = Activation.AllowEdit;
            ultListComponent.Rows[index].Cells["Length"].Value = DBNull.Value;
            ultListComponent.Rows[index].Cells["Width"].Activation = Activation.AllowEdit;
            ultListComponent.Rows[index].Cells["Width"].Value = DBNull.Value;
            ultListComponent.Rows[index].Cells["Thickness"].Activation = Activation.AllowEdit;
            ultListComponent.Rows[index].Cells["Thickness"].Value = DBNull.Value;
            ultListComponent.Rows[index].Cells["Revision"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    //Tien Add
    private void LoadComboItemReference()
    {
      DataTable dtIte1 = DataBaseAccess.SearchCommandTextDataTable(string.Format(@" SELECT INF.ItemCode, INF.Revision,
                                                                                    (INF.ItemCode + ' | ' + CAST(INF.Revision as varchar) + ' | ' + BS.Name) DisplayText
                                                                                    FROM	TblBOMItemInfo INF
                                                                                          INNER JOIN TblBOMItemBasic BS ON 
                                                                                                    (INF.ItemCode = BS.ItemCode)
                                                                                     ORDER BY INF.ItemCode DESC"));
      Utility.LoadUltraCombo(ultraCbItemReference, dtIte1, "ItemCode", "DisplayText", "DisplayText");
    }


    #endregion UltraGrid Event

    private void ultraCbItemReference_ValueChanged(object sender, EventArgs e)
    {
      this.LoadComGroup();
    }

    private void btnLoadRefer_Click(object sender, EventArgs e)
    {
      this.GetDataComponentReference();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (!loaddingItemCode)
      {
        this.itemCode = Utility.GetSelectedValueUltraCombobox(ultItemCode);
        this.LoadData();
      }
    }

    private void ultItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        if (!loaddingItemCode)
        {
          this.itemCode = Utility.GetSelectedValueUltraCombobox(ultItemCode);
          this.LoadData();
        }
      }
    }

    private void chkShowCompPicture_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowGroupboxOnGrid(ultListComponent, "DisplayComponentCode", 0, groupItemPicture, chkShowCompPicture.Checked);
    }

    private void ultListComponent_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      try
      {
        string compCode = ultListComponent.Selected.Rows[0].Cells["ComponentCode"].Value.ToString().Trim();
        pictureComp.ImageLocation = FunctionUtility.BOMGetItemComponentImage(compCode);
        groupItemPicture.Text = compCode;
        Utility.BOMShowGroupboxOnGrid(ultListComponent, "DisplayComponentCode", ultListComponent.Selected.Rows[0].Index, groupItemPicture, chkShowCompPicture.Checked);
      }
      catch { }
    }

    private void ultListComponent_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DisplayComponentCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DisplayComponentCode"].AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;

      // Hide column
      e.Layout.Bands[0].Columns["Name"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CompGroup"].Hidden = true;
      e.Layout.Bands[0].Columns["ComponentCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Name"].Hidden = true;

      e.Layout.Bands[0].Columns["DisplayComponentCode"].ValueList = ultraCBComponentCode;
      e.Layout.Bands[0].Columns["DisplayComponentCode"].Header.Caption = "Component";
      e.Layout.Bands[0].Columns["DisplayComponentCode"].Width = 400;
      e.Layout.Bands[0].Columns["Revision"].Width = 50;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Group"].CellActivation = Activation.ActivateOnly;
      e.Layout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      if (!this.canUpdate)
      {
        int count = ultListComponent.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultListComponent.Rows[i].Activation = Activation.ActivateOnly;
        }
      }
      else
      {
        for (int i = 0; i < ultListComponent.Rows.Count; i++)
        {
          int compGroup = DBConvert.ParseInt(ultListComponent.Rows[i].Cells["CompGroup"].Value.ToString());
          if (compGroup != 999)
          {
            ultListComponent.Rows[i].Cells["Length"].Activation = Activation.ActivateOnly;
            ultListComponent.Rows[i].Cells["Width"].Activation = Activation.ActivateOnly;
            ultListComponent.Rows[i].Cells["Thickness"].Activation = Activation.ActivateOnly;
          }
        }
      }
    }
  }
}
