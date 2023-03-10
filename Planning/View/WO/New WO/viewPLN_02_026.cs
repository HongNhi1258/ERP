using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_026 : MainUserControl
  {
    private string insertPid = string.Empty;
    public long workOrder = long.MinValue;
    private bool chklock = true;
    private bool chkconfirm = true;
    public viewPLN_02_026()
    {
      InitializeComponent();
    }

    /// <summary>
    /// load view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_026_Load(object sender, EventArgs e)
    {
      this.LoadDDLocation();

      this.Search();

      this.LoadPartType();
    }
    private void LoadDDLocation()
    {
      string cm = @"SELECT WA.Pid, WA.StandByEN
                    FROM TblBOMCodeMaster CM 
	                    INNER JOIN TblWIPWorkArea WA ON CM.[Group] = 81915
								                    AND CM.Code = WA.Pid
                    ORDER BY WA.OrderBy";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      Shared.Utility.ControlUtility.LoadUltraDropDown(ultDDLocation, dt, "Pid", "StandByEN", "Pid");
    }
    //Load Part Type
    private void LoadPartType()
    {
      string cmText = "SELECT Code,Value FROM TblBOMCodeMaster WHERE [GROUP] = 1992";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultraDDPartType.DataSource = dt;
      ultraDDPartType.DisplayMember = "Value";
      ultraDDPartType.ValueMember = "Code";
      ultraDDPartType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDPartType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }
    private UltraDropDown LoadSupplier(string item, int rev, long partgroup, UltraDropDown ultSupp)
    {
      if (ultSupp == null)
      {
        ultSupp = new UltraDropDown();
        this.Controls.Add(ultSupp);
      }
      DBParameter[] input = new DBParameter[3];
      if (item.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.String, item);
      }
      if (rev != int.MinValue)
      {
        input[1] = new DBParameter("@Revision", DbType.Int32, rev);
      }
      if (partgroup != long.MinValue)
      {
        input[2] = new DBParameter("@PartGroup", DbType.Int64, partgroup);
      }
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNLoadSupplierDefaultForPart", input);
      if (dt != null && dt.Rows.Count > 0)
      {
        ControlUtility.LoadUltraDropDown(ultSupp, dt, "Value", "Display", "Value");
        ultSupp.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultSupp.Visible = false;
        ultSupp.DisplayLayout.AutoFitColumns = true;
        ultSupp.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      }
      return ultSupp;
    }
    /// <summary>
    /// Search Data
    /// </summary>
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@WO", DbType.Int32, this.workOrder);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNWODetailConfirm_Select", inputParam);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", ds.Tables[0].Columns["ItemCode"], ds.Tables[1].Columns["ItemCode"], false));
      DataColumn[] Parent = new DataColumn[2];
      Parent[0] = ds.Tables[0].Columns["ItemCode"];
      Parent[1] = ds.Tables[0].Columns["Revision"];
      DataColumn[] Child = new DataColumn[2];
      Child[0] = ds.Tables[2].Columns["ItemCode"];
      Child[1] = ds.Tables[2].Columns["Revision"];
      ds.Relations.Add(new DataRelation("dtParent_dtSDChild", Parent, Child, false));
      ultData.DataSource = ds;
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];

        if (DBConvert.ParseInt(row.Cells["PLNConfirm"].Value.ToString()) == 1)
        {
          row.Cells["PLNConfirm"].Activation = Activation.ActivateOnly;
          row.Cells["Delete"].Activation = Activation.AllowEdit;
        }
        else
        {
          row.Cells["PLNConfirm"].Activation = Activation.AllowEdit;
          row.Cells["Delete"].Activation = Activation.ActivateOnly;
        }
        int childcount = this.ultData.Rows[i].ChildBands[1].Rows.Count;
        if (childcount <= 0)
        {
          row.Cells["LocationDefault"].Value = 0;
        }
        for (int j = 0; j < ultData.Rows[i].ChildBands[1].Rows.Count; j++)
        {
          UltraGridRow rowchild = this.ultData.Rows[i].ChildBands[1].Rows[j];

          if (rowchild.Cells["Location"].Value.ToString() != string.Empty)
          {
            row.Cells["LocationDefault"].Value = 1;
          }
          else
          {
            row.Cells["LocationDefault"].Value = 0;
          }
        }
      }
    }

    /// <summary>
    /// close form click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// init ultra grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      for (int i = 0; i < e.Layout.Bands[2].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[2].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[2].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[2].Columns["PartType"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[2].Columns["Location"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[2].Columns["Supplier"].CellAppearance.TextHAlign = HAlign.Left;
      // Activation
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      for (int k = 0; k < e.Layout.Bands[2].Columns.Count - 1; k++)
      {
        e.Layout.Bands[2].Columns[k].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[2].Columns["Location"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[2].Columns["Location"].ValueList = this.ultDDLocation;
      e.Layout.Bands[2].Columns["PartType"].ValueList = this.ultraDDPartType;
      e.Layout.Bands[2].Columns["Supplier"].CellActivation = Activation.AllowEdit;

      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["WOType"].Header.Caption = "WO \nType";

      e.Layout.Bands[1].Columns["SaleNo"].Header.Caption = "Sale Order No";
      e.Layout.Bands[1].Columns["CustomerPONo"].Header.Caption = "Customer PO No";
      e.Layout.Bands[1].Columns["ScheduleDelivery"].Header.Caption = "Confirmed Ship Date";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["QtyShipped"].Header.Caption = "Shipped";

      e.Layout.Bands[0].Columns["PLNConfirm"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["PLNLock"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Delete"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["PLNConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["WIPRun"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PLNLock"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ItemConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CARConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CBMExisted"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ProcessItemCode"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Delete"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["LocationDefault"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[2].Columns["Supplier"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
      UltraDropDown ultSup = (UltraDropDown)e.Layout.Bands[2].Columns["Supplier"].ValueList;
      e.Layout.Bands[2].Columns["Supplier"].ValueList = this.LoadSupplier("", int.MinValue, long.MinValue, ultSup);

      e.Layout.Bands[0].Columns["CMDOW"].Hidden = true;
      e.Layout.Bands[0].Columns["OldPLNLock"].Hidden = true;
      e.Layout.Bands[0].Columns["ConflictCarcass"].Hidden = true;
      e.Layout.Bands[0].Columns["OldPLNConfirm"].Hidden = true;
      e.Layout.Bands[0].Columns["ConfirmDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Delete"].Hidden = true;
      e.Layout.Bands[2].Columns["PartGroupDetailPid"].Hidden = true;

      e.Layout.Bands[0].Columns["PLNConfirm"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["PLNLock"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Delete"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["WIPRun"].CellAppearance.BackColor = Color.LightCyan;
      e.Layout.Bands[2].Columns["Location"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["ItemConfirm"].CellAppearance.BackColor = Color.LightCyan;
      e.Layout.Bands[0].Columns["CARConfirm"].CellAppearance.BackColor = Color.LightCyan;
      e.Layout.Bands[0].Columns["CBMExisted"].CellAppearance.BackColor = Color.LightCyan;
      e.Layout.Bands[0].Columns["ProcessItemCode"].CellAppearance.BackColor = Color.LightCyan;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      // Set caption column
      e.Layout.Bands[0].Columns["WIPRun"].Header.Caption = "WIP \nRun";
      e.Layout.Bands[0].Columns["ItemConfirm"].Header.Caption = "Item \nConfirm";
      e.Layout.Bands[0].Columns["CARConfirm"].Header.Caption = "Carcass \nConfirm";
      e.Layout.Bands[0].Columns["CBMExisted"].Header.Caption = "Existed \nCBM";
      e.Layout.Bands[0].Columns["PLNConfirm"].Header.Caption = "PLN \nConfirm";
      e.Layout.Bands[0].Columns["PLNLock"].Header.Caption = "PLN \nLock";
      e.Layout.Bands[0].Columns["LocationDefault"].Header.Caption = "Default \nLocation";

      e.Layout.Bands[2].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[2].Columns["PartCode"].Header.Caption = "Part Code";
      e.Layout.Bands[2].Columns["PartName"].Header.Caption = "Part Name";
      e.Layout.Bands[2].Columns["PartType"].Header.Caption = "Part Type";
      e.Layout.Bands[2].Columns["PartPercent"].Header.Caption = "Part Percent";
      e.Layout.Bands[0].Columns["ProcessItemCode"].Hidden = true;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// grid cell change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      int select = 0;
      if (string.Compare("PLNLock", colName, true) == 0)
      {
        select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.chklock = false;
          chkLock.Checked = false;
          this.chklock = true;
        }
      }
      if (string.Compare("PLNConfirm", colName, true) == 0)
      {
        select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.chkconfirm = false;
          chkConfirm.Checked = false;
          this.chkconfirm = true;
        }
      }
      //if (string.Compare("PLNLock", colName, true) == 0)
      //{
      //  select = DBConvert.ParseInt(e.Cell.Text.ToString());
      //  if (select == 1)
      //  {
      //    // Carcass Confirm
      //    if (DBConvert.ParseInt(e.Cell.Row.Cells["CARConfirm"].Value.ToString()) == 0)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0001", "CAR Confirm");
      //      e.Cell.CancelUpdate();
      //      return;
      //    }
      //    // ItemConfirm
      //    if (DBConvert.ParseInt(e.Cell.Row.Cells["ItemConfirm"].Value.ToString()) == 0)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0001", "Item Confirm");
      //      e.Cell.CancelUpdate();
      //      return;
      //    }
      //  }
      //}
      if (string.Compare("PLNConfirm", colName, true) == 0)
      {
        select = DBConvert.ParseInt(e.Cell.Text.ToString());
        if (select == 1)
        {
          if (DBConvert.ParseInt(e.Cell.Row.Cells["CMDOW"].Value.ToString()) == 1)
          {
            // Carcass Confirm
            if (DBConvert.ParseInt(e.Cell.Row.Cells["CARConfirm"].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "CAR Confirm");
              e.Cell.CancelUpdate();
              return;
            }
          }
          if (DBConvert.ParseInt(e.Cell.Row.Cells["CMDOW"].Value.ToString()) == 1 || DBConvert.ParseInt(e.Cell.Row.Cells["CMDOW"].Value.ToString()) == 2)
          {
            // ItemConfirm
            if (DBConvert.ParseInt(e.Cell.Row.Cells["ItemConfirm"].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Item Confirm");
              e.Cell.CancelUpdate();
              return;
            }

            // CBM Existed
            if (DBConvert.ParseInt(e.Cell.Row.Cells["CBMExisted"].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "CBM Existed");
              e.Cell.CancelUpdate();
              return;
            }

            // Process ItemCode
            if (DBConvert.ParseInt(e.Cell.Row.Cells["ProcessItemCode"].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Process ItemCode");
              e.Cell.CancelUpdate();
              return;
            }
          }
          //Tien Add
          if (DBConvert.ParseInt(e.Cell.Row.Cells["LocationDefault"].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Location Default");
            e.Cell.CancelUpdate();
            return;
          }
        }

        if (string.Compare("Delete", colName, true) == 0)
        {
          select = DBConvert.ParseInt(e.Cell.Text.ToString());
          if (select == 1)
          {
            // Wip Run
            if (DBConvert.ParseInt(e.Cell.Row.Cells["WipRun"].Value.ToString()) == 1)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Wip Run");
              e.Cell.CancelUpdate();
              return;
            }
          }
        }
      }
    }
    private bool CheckValid()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["PLNConfirm"].Value.ToString()) == 1
            && DBConvert.ParseInt(row.Cells["OldPLNConfirm"].Value.ToString()) == 0)
        {
          long supp1 = long.MinValue;
          long supp2 = long.MinValue;
          for (int j = 0; j < ultData.Rows[i].ChildBands[1].Rows.Count; j++)
          {
            if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Location"].Value.ToString()) == long.MinValue)
            {
              WindowUtinity.ShowMessageError("ERR0115", "Location");
              return false;
            }
            if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Location"].Value.ToString()) == 39)
            {
              if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Supplier"].Value.ToString()) == long.MinValue)
              {
                WindowUtinity.ShowMessageError("ERR0115", "Supplier");
                return false;
              }
              else
              {
                if (supp1 == long.MinValue)
                {
                  supp1 = DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Supplier"].Value.ToString());
                }
                else
                {
                  supp2 = DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Supplier"].Value.ToString());
                }
              }
              if (supp1 != long.MinValue && supp2 != long.MinValue)
              {
                if (supp1 != supp2)
                {
                  WindowUtinity.ShowMessageError("ERR0080");
                  return false;
                }
              }
            }
          }
        }
      }
      return true;
    }
    /// <summary>
    /// save click - save confirm or delete item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.insertPid = string.Empty;
      bool result = true;
      result = this.CheckValid();
      if (result == false)
      {
        return;
      }
      // Check Part When Delete WO Confirm
      string message = string.Empty;
      result = this.CheckPartWhenDeleteWOConfirm(out message);
      if (result == false)
      {
        WindowUtinity.ShowMessageError("ERR0123", message, "Exist PartCode");
        return;
      }
      // End
      try
      {
        DataTable dt = ((DataSet)ultData.DataSource).Tables[0];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState == DataRowState.Modified)
          {
            long lPid = 0;
            if (DBConvert.ParseLong(dt.Rows[i]["ConfirmDetailPid"].ToString()) > 0)
            {
              lPid = DBConvert.ParseLong(dt.Rows[i]["ConfirmDetailPid"].ToString());
            }
            bool istrue = DataBaseAccess.ExecuteCommandText(String.Format("UPDATE TblPLNWorkOrderConfirmedDetails SET Remark = '{0}' WHERE Pid = {1}", dt.Rows[i]["Remark"].ToString().Trim(), lPid.ToString()));
          }
        }
      }
      catch
      { }
      // Delete WO Confirm
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Delete"].Value.ToString()) == 1)
        {
          if (DBConvert.ParseLong(ultData.Rows[i].Cells["ConfirmDetailPid"].Value.ToString()) >= 0)
          {
            result = DeleteConfirmWork(DBConvert.ParseLong(row.Cells["ConfirmDetailPid"].Value.ToString()),
                    DBConvert.ParseInt(row.Cells["CMDOW"].Value.ToString()));
            if (result == false)
            {
              break;
            }
          }
        }
      }

      // Confirm WO
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("Wo", typeof(Int64));
      dtSource.Columns.Add("ItemCode", typeof(String));
      dtSource.Columns.Add("Revision", typeof(Int32));
      dtSource.Columns.Add("Location", typeof(Int64));
      dtSource.Columns.Add("PartGroupPid", typeof(Int64));
      dtSource.Columns.Add("Supplier", typeof(Int64));
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["PLNConfirm"].Value.ToString()) == 1
            && DBConvert.ParseInt(row.Cells["OldPLNConfirm"].Value.ToString()) == 0)
        {
          long wo = DBConvert.ParseLong(row.Cells["WO"].Value.ToString());
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(ultData.Rows[i].Cells["revision"].Value.ToString());
          int dlBOM = DBConvert.ParseInt(row.Cells["CMDOW"].Value.ToString());
          string Remark = row.Cells["Remark"].Value.ToString();

          for (int j = 0; j < ultData.Rows[i].ChildBands[1].Rows.Count; j++)
          {
            DataRow dr = dtSource.NewRow();
            dr["Wo"] = wo;
            dr["ItemCode"] = itemCode;
            dr["Revision"] = revision;
            dr["Location"] = DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Location"].Value.ToString());
            dr["PartGroupPid"] = DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["PartGroupDetailPid"].Value.ToString());
            if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Supplier"].Value.ToString()) != long.MinValue)
            {
              dr["Supplier"] = DBConvert.ParseLong(ultData.Rows[i].ChildBands[1].Rows[j].Cells["Supplier"].Value.ToString());
            }
            dtSource.Rows.Add(dr);
          }

          result = ConfirmWorkByItem(wo, itemCode, revision, dlBOM, Remark, dtSource);
        }
        if (DBConvert.ParseInt(row.Cells["PLNLock"].Value.ToString()) != DBConvert.ParseInt(row.Cells["OldPLNLock"].Value.ToString()))
        {
          long wo = DBConvert.ParseLong(row.Cells["WO"].Value.ToString());
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(ultData.Rows[i].Cells["revision"].Value.ToString());
          result = LockWorkByItem(wo, itemCode, revision, DBConvert.ParseInt(row.Cells["PLNLock"].Value.ToString()));
        }
      }
      if (insertPid.Length > 0)
      {
        result = UnlockCarcass(this.insertPid);
      }

      if (result)
      {
        WindowUtinity.ShowMessageSuccessFromText(FunctionUtility.GetMessage("MSG0004"));
      }
      else
      {
        WindowUtinity.ShowMessageError(FunctionUtility.GetMessage("ERR0005"));
      }
      this.Search();
    }

    /// <summary>
    /// Unlock Carcass After Confirm Wo
    /// </summary>
    /// <param name="Wo"></param>
    /// <returns></returns>
    private bool UnlockCarcass(string stringPid)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@StringPid", DbType.AnsiString, 4000, stringPid) };
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNUnlockCarcassAfterConfirmWo", inputParam, outputParam);
      int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
      return (success == 1 ? true : false);
    }

    /// <summary>
    /// Check Part When Delete WO Confirm Detail
    /// </summary>
    /// <returns></returns>
    private bool CheckPartWhenDeleteWOConfirm(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Delete"].Value) == 1 && DBConvert.ParseLong(row.Cells["ConfirmDetailPid"].Value) > 0)
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@WOConfirmDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["ConfirmDetailPid"].Value));
          DataTable dtCheck = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckWOConfirmDetailWhenDelete_Select", input);
          if (dtCheck != null && dtCheck.Rows.Count > 0)
          {
            message = row.Cells["WO"].Value.ToString() + " - " + row.Cells["CarcassCode"].Value.ToString();
            return false;
          }
        }
      }
      return true;
    }
    /// <summary>
    /// delete work confirm
    /// </summary>
    /// <param name="ConfimPid"></param>
    /// <returns></returns>
    private bool DeleteConfirmWork(long confimPid, int dlBOM)
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ConfimPid", DbType.Int64, confimPid);
      inputParam[1] = new DBParameter("@DLBOM", DbType.Int32, dlBOM);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNWOConfirmDetail_Delete", inputParam, outputParam);
      int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
      return (success == 1 ? true : false);
    }

    /// <summary>
    /// Confirm Item
    /// </summary>
    /// <param name="Wo"></param>
    /// <param name="ItemCode"></param>
    /// <param name="Revision"></param>
    /// <returns></returns>
    private bool ConfirmWorkByItem(long wo, string itemCode, int revision, int dlBOM, string Remark, DataTable dtsource)
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[7];
      inputParam[0] = new SqlDBParameter("@Wo", SqlDbType.BigInt, wo);
      inputParam[1] = new SqlDBParameter("@ItemCode", SqlDbType.VarChar, 16, itemCode);
      inputParam[2] = new SqlDBParameter("@Revision", SqlDbType.Int, revision);
      inputParam[3] = new SqlDBParameter("@ConfirmBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[4] = new SqlDBParameter("@DlBOM", SqlDbType.Int, dlBOM);
      inputParam[5] = new SqlDBParameter("@Remark", SqlDbType.VarChar, 256, Remark);
      inputParam[6] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtsource);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      Shared.DataBaseUtility.SqlDataBaseAccess.ExecuteStoreProcedure("spPLNWOConfirmDetail_Insert", 7200, inputParam, outputParam);
      long success = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (success > 0)
      {
        insertPid += success.ToString().Trim() + ",";
        return true;
      }
      else
      {
        return false;
      }
    }

    private bool LockWorkByItem(long wo, string itemCode, int revision, int isLock)
    {
      string storeName = "";
      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
      if (isLock == 1)
      {
        inputParam[3] = new DBParameter("@ConfirmBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spPLNWOConfirmLockHistory_Insert";
      }
      else
      {
        storeName = "spPLNWOConfirmUnLockHistory_Insert";
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, 90, inputParam, outputParam);
      long success = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (success > 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string values = e.NewValue.ToString();
      switch (colName)
      {
        case "Location":
          {
            if (values.Length == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Location");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string itemCode;
      try
      {
        itemCode = e.Cell.Row.Cells["ItemCode"].Value.ToString();
      }
      catch
      {
        itemCode = "";
      }
      int rev;
      try
      {
        rev = DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString());
      }
      catch
      {
        rev = int.MinValue;
      }
      long partgroup;
      try
      {
        partgroup = DBConvert.ParseLong(e.Cell.Row.Cells["PartGroupDetailPid"].Value.ToString());
      }
      catch
      {
        partgroup = long.MinValue;
      }
      switch (columnName)
      {
        case "supplier":
          if (DBConvert.ParseLong(e.Cell.Row.Cells["Location"].Value.ToString()) == 39)
          {
            e.Cell.Row.Cells["Supplier"].Activation = Activation.AllowEdit;
            UltraDropDown ultSup = (UltraDropDown)e.Cell.Row.Cells["Supplier"].ValueList;
            e.Cell.Row.Cells["Supplier"].ValueList = this.LoadSupplier(itemCode, rev, partgroup, ultSup);
            e.Cell.Row.Cells["Supplier"].Appearance.BackColor = Color.LightBlue;
          }
          else
          {
            e.Cell.Row.Cells["Supplier"].Activation = Activation.ActivateOnly;
            e.Cell.Row.Cells["Supplier"].Appearance.BackColor = Color.Empty;
          }
          break;
        default:
          break;
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      switch (columnName)
      {
        case "location":
          {
            if (DBConvert.ParseLong(value) != 39)
            {
              if (e.Cell.Row.Cells["Supplier"].Value != null || e.Cell.Row.Cells["Supplier"].Text.Length > 0)
              {
                e.Cell.Row.Cells["Supplier"].Value = DBNull.Value;
                e.Cell.Row.Cells["Supplier"].Appearance.BackColor = Color.Empty;
              }
            }
          }
          break;
        default:
          break;

      }
    }

    private void chkExpand_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpand.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }
    //Check All PLN Lock
    private void chkLock_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chklock)
      {
        int checkAll = (chkLock.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false && ultData.Rows[i].Cells["PLNLock"].Activation != Activation.ActivateOnly)
          {
            ultData.Rows[i].Cells["PLNLock"].Value = checkAll;
          }
        }
      }
    }
    //Check All PLN Confirm
    private void chkConfirm_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkconfirm)
      {
        int checkAll = (chkConfirm.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false && ultData.Rows[i].Cells["PLNConfirm"].Activation != Activation.ActivateOnly)
          {
            ultData.Rows[i].Cells["PLNConfirm"].Value = checkAll;
          }
        }
      }
    }
  }
}
