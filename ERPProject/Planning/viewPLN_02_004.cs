/*
   Author  : Võ Hoa Lư
   Email   : luvh_it@daico-furniture.com
   Date    : 25/06/2010
   Company :  Dai Co
*/
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using FormSerialisation;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_004 : MainUserControl
  {
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public long woPid = long.MinValue;
    private bool isBindWo = false;
    private bool canUpdate = false;
    private IList listDeleting = new ArrayList();
    private IList listDeleted = new ArrayList();
    private IList listComWorkAreaName = new ArrayList();

    public viewPLN_02_004()
    {
      InitializeComponent();
    }

    private void viewPLN_02_004_Load(object sender, EventArgs e)
    {
      this.canUpdate = (btnSave.Enabled && btnSave.Visible);
      this.LoadComboboxWo();
      this.LoadDeadlineStatus();
      this.LoadData();
      FormSerialisor.Deserialise(this, System.Windows.Forms.Application.StartupPath + @"\viewPLN_02_004.xml");
    }

    #region LoadData

    private void LoadComboboxWo()
    {
      this.isBindWo = true;
      string commandText = string.Format("SELECT Pid From VPLNLoadAllWoForControl ORDER BY Pid DESC");
      DataTable dtWo = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      cmbWo.DataSource = dtWo;
      try
      {
        cmbWo.SelectedRow.Cells["Pid"].Value = this.woPid;
      }
      catch { }
      cmbWo.DisplayLayout.Bands[0].Columns["Pid"].Width = 826;
      cmbWo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      this.isBindWo = false;
    }

    private void LoadDeadlineStatus()
    {
      string commandText = string.Format(@"select Code,Value  from TblBOMCodeMaster where [Group] = '16002'");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultdrDeadlineStatus.DataSource = dt;
      ultdrDeadlineStatus.DisplayMember = "Value";
      ultdrDeadlineStatus.ValueMember = "Code";
      ultdrDeadlineStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadGridData(UltraGrid ultData, string devision)
    {
      this.listComWorkAreaName = new ArrayList();
      this.listDeleted = new ArrayList();
      DBParameter[] input = new DBParameter[] { new DBParameter("@WoPid", DbType.Int64, this.woPid), new DBParameter("@Devision", DbType.AnsiString, devision) };
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNWorkOrderDeadlineInfo", input);
      for (int i = 1; i < dsSource.Tables.Count; i++)
      {
        long workAreaPid = 0;
        string workAreaName = string.Empty;
        if (dsSource.Tables[i].Rows.Count > 0)
        {
          workAreaPid = DBConvert.ParseLong(dsSource.Tables[i].Rows[0]["WorkAreaPid"].ToString());
          workAreaName = dsSource.Tables[i].Rows[0]["WorkAreaName"].ToString();
          dsSource.Tables[i].Rows.RemoveAt(0);
          dsSource.Tables[i].Columns.Remove("WorkAreaName");
          dsSource.Tables[i].Columns["WorkAreaPid"].DefaultValue = workAreaPid;
          this.listComWorkAreaName.Add(workAreaName);
        }
        dsSource.Tables[i].Columns.Add(new DataColumn("RowState", typeof(System.Int32)));
        DataRelation dtRelation;
        if (string.Compare(devision, Shared.Utility.ConstantClass.Devision_Component, true) == 0)
        {
          dtRelation = new DataRelation("dtRelation" + i, dsSource.Tables[0].Columns["CarcassCode"], dsSource.Tables[i].Columns["CarcassCode"], false);
        }
        else
        {
          dtRelation = new DataRelation("dtRelation" + i, dsSource.Tables[0].Columns["ItemCode"], dsSource.Tables[i].Columns["ItemCode"], false);
        }
        dsSource.Relations.Add(dtRelation);
      }
      ultData.DataSource = dsSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {

        ultData.Rows[i].Activation = Activation.ActivateOnly;
      }
      for (int a = 0; a < ultData.DisplayLayout.Bands[1].Columns.Count; a++)
      {
        string columnName = ultData.DisplayLayout.Bands[1].Columns[a].Header.Caption;
        if (string.Compare(columnName, "DeadlineHistory", true) == 0)
        {
          ultData.DisplayLayout.Bands[1].Columns[a].CellActivation = Activation.ActivateOnly;
        }
      }
      //for (int b = 0; b < ultData.DisplayLayout.Bands[2].Columns.Count; b++)
      //{
      //  string columnName = ultData.DisplayLayout.Bands[2].Columns[b].Header.Caption;
      //  if (string.Compare(columnName, "DeadlineHistory", true) == 0)
      //  {
      //    ultData.DisplayLayout.Bands[2].Columns[b].CellActivation = Activation.ActivateOnly;
      //  }
      //}
    }

    private void LoadData()
    {
      this.LoadGridData(ultGridCompWorkArea, Shared.Utility.ConstantClass.Devision_Component);
      //this.LoadGridData(ultGridCarcassWorkArea, Shared.Utility.ConstantClass.Devision_Carcass);
      //Component Work Area
      if (chkCompExpandAll.Checked)
      {
        ultGridCompWorkArea.Rows.ExpandAll(true);
      }
      else
      {
        ultGridCompWorkArea.Rows.CollapseAll(true);
      }
      //Carcass Work Area
      if (chkCarcassExpandAll.Checked)
      {
        ultGridCarcassWorkArea.Rows.ExpandAll(true);
      }
      else
      {
        ultGridCarcassWorkArea.Rows.CollapseAll(true);
      }
    }
    #endregion LoadData

    #region Check And Save

    private PLNWorkOrderDeadline LoadObject(long pid)
    {
      PLNWorkOrderDeadline obj = new PLNWorkOrderDeadline();
      if (pid == long.MinValue)
      {
        return obj;
      }
      obj.Pid = pid;
      obj = (PLNWorkOrderDeadline)Shared.DataBaseUtility.DataBaseAccess.LoadObject(obj, new string[] { "Pid" });
      return obj;
    }

    private bool IsValidData()
    {
      //Check Comp Area warning
      bool resultQuestion = false;
      for (int i = 0; i < ultGridCompWorkArea.Rows.Count; i++)
      {
        string Carcass = ultGridCompWorkArea.Rows[i].Cells["CarcassCode"].Value.ToString();
        int parentQty = DBConvert.ParseInt(ultGridCompWorkArea.Rows[i].Cells["Qty"].Value.ToString());
        ultGridCompWorkArea.Rows[i].CellAppearance.BackColor = Color.White;
        for (int n = 0; n < ultGridCompWorkArea.Rows[i].ChildBands.Count; n++)
        {
          int childQty = 0;

          for (int k = 0; k < ultGridCompWorkArea.Rows[i].ChildBands[n].Rows.Count; k++)
          {
            if (DBConvert.ParseInt(ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["RowState"].Value.ToString()) == 1)
            {
              //Dead Line
              if ((ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString().Length == 0) && (DBConvert.ParseInt(ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["DeadlineStatus"].Value.ToString()) != 1))
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { string.Format("Dead line at carcass {0} ", Carcass) });
                ultGridCompWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
                return false;
              }
              DateTime deadDate = new DateTime();
              if (ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString() != "")
              {
                deadDate = DBConvert.ParseDateTime(ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString(), formatConvert);
              }
              if (resultQuestion == false)
              {
                if ((deadDate < DateTime.Today) && (DBConvert.ParseInt(ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["DeadlineStatus"].Value.ToString()) != 1))
                {
                  DialogResult dgResult = Shared.Utility.WindowUtinity.ShowMessageConfirm("WRN0021", "Component");
                  if (dgResult == DialogResult.No)
                  {
                    return false;
                  }
                  else if (dgResult == DialogResult.Yes)
                  {
                    resultQuestion = true;
                  }
                }
              }

              if (k > 0)
              {
                int count = 0;
                DateTime date = new DateTime();
                if (ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString() != "")
                {
                  date = DBConvert.ParseDateTime(ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString(), formatConvert);
                }
                DateTime beforeDate = DateTime.MinValue;
                for (int h = 0; h < ultGridCompWorkArea.Rows[i].ChildBands[n].Rows.Count; h++)
                {
                  beforeDate = DBConvert.ParseDateTime(ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[h].Cells["Deadline"].Value.ToString(), formatConvert);
                  if (beforeDate == date)
                  {
                    count++;
                  }
                }

                if (count == 2)
                {
                  Shared.Utility.WindowUtinity.ShowMessageError("ERR0006", "Dead Line");
                  ultGridCompWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
                  return false;
                }
              }
            }
            int qty = DBConvert.ParseInt(ultGridCompWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Qty"].Value.ToString());
            if (qty != int.MinValue)
            {
              childQty += qty;
            }
          }
          //if (childQty > parentQty)
          //{
          //  Shared.Utility.WindowUtinity.ShowMessageError("ERR0026", new string[] { string.Format("Qty of carcass {0}", Carcass) });
          //  ultGridCompWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
          //  return false;
          //}
          //if (childQty < parentQty && childQty > 0 || childQty < 0)
          //{
          //  Shared.Utility.WindowUtinity.ShowMessageError("ERR0027", new string[] { string.Format("Qty of carcass {0}", Carcass) });
          //  ultGridCompWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
          //  return false;
          //}
        }
      }

      //Check Carcass Area warning
      for (int i = 0; i < ultGridCarcassWorkArea.Rows.Count; i++)
      {
        string item = ultGridCarcassWorkArea.Rows[i].Cells["ItemCode"].Value.ToString();
        int parentQty = DBConvert.ParseInt(ultGridCarcassWorkArea.Rows[i].Cells["Qty"].Value.ToString());
        ultGridCarcassWorkArea.Rows[i].CellAppearance.BackColor = Color.White;
        for (int n = 0; n < ultGridCarcassWorkArea.Rows[i].ChildBands.Count; n++)
        {
          int childQty = 0;
          for (int k = 0; k < ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows.Count; k++)
          {
            //Dead Line
            if (DBConvert.ParseInt(ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["RowState"].Value.ToString()) == 1)
            {
              if ((ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString().Length == 0) && (DBConvert.ParseInt(ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["DeadlineStatus"].Value.ToString()) != 1))
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { string.Format("Dead line at item {0} ", item) });
                ultGridCarcassWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
                return false;
              }
              DateTime deadDate = new DateTime();
              if (ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString() != "")
              {
                deadDate = DBConvert.ParseDateTime(ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString(), formatConvert);
              }
              if (resultQuestion == false)
              {
                if (deadDate < DateTime.Today)
                {
                  DialogResult dgResult = Shared.Utility.WindowUtinity.ShowMessageConfirm("WRN0021", "Carcass");
                  if (dgResult == DialogResult.No)
                  {
                    return false;
                  }
                  else if (dgResult == DialogResult.Yes)
                  {
                    resultQuestion = true;
                  }
                }
              }
              if (k > 0)
              {
                int count = 0;
                DateTime date = new DateTime();
                if (ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString() != "")
                {
                  date = DBConvert.ParseDateTime(ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Deadline"].Value.ToString(), formatConvert);
                }
                DateTime beforeDate = DateTime.MinValue;
                for (int h = 0; h < ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows.Count; h++)
                {
                  beforeDate = DBConvert.ParseDateTime(ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[h].Cells["Deadline"].Value.ToString(), formatConvert);
                  if (beforeDate == date)
                  {
                    count++;
                  }
                }

                if (count == 2)
                {
                  Shared.Utility.WindowUtinity.ShowMessageError("ERR0006", "Dead Line");
                  ultGridCarcassWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
                  return false;
                }
              }
            }
            int qty = DBConvert.ParseInt(ultGridCarcassWorkArea.Rows[i].ChildBands[n].Rows[k].Cells["Qty"].Value.ToString());
            if (qty != int.MinValue)
            {
              childQty += qty;
            }
          }
          //if (childQty > parentQty)
          //{
          //  Shared.Utility.WindowUtinity.ShowMessageError("ERR0026", new string[] { string.Format("Qty of item {0}", item) });
          //  ultGridCarcassWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
          //  return false;
          //}

          //if (childQty < parentQty && childQty > 0 || childQty < 0)
          //{
          //  Shared.Utility.WindowUtinity.ShowMessageError("ERR0027", new string[] { string.Format("Qty of item {0}", item) });
          //  ultGridCarcassWorkArea.Rows[i].CellAppearance.BackColor = Color.Yellow;
          //  return false;
          //}
        }
      }
      return true;
    }

    private bool SaveCompArea()
    {
      bool result = true;
      DateTime currentDate = DateTime.Today;
      // Insert / Update
      for (int i = 0; i < ultGridCompWorkArea.Rows.Count; i++)
      {
        UltraGridRow row = ultGridCompWorkArea.Rows[i];
        int countBand = row.ChildBands.Count;
        for (int indexBand = 0; indexBand < countBand; indexBand++)
        {
          bool state = false;
          int countRowChild = row.ChildBands[indexBand].Rows.Count;
          for (int indexState = 0; indexState < countRowChild; indexState++)
          {
            UltraGridRow rowChildState = row.ChildBands[indexBand].Rows[indexState];
            if (DBConvert.ParseInt(rowChildState.Cells["RowState"].Value.ToString()) == 1)
            {
              state = true;
            }
          }
          for (int indexRowChild = 0; indexRowChild < countRowChild; indexRowChild++)
          {
            UltraGridRow rowChild = row.ChildBands[indexBand].Rows[indexRowChild];
            int rowState = DBConvert.ParseInt(rowChild.Cells["RowState"].Value.ToString());
            DateTime date = new DateTime();
            if (rowChild.Cells["Deadline"].Value.ToString() != string.Empty)
            {
              date = DBConvert.ParseDateTime(rowChild.Cells["Deadline"].Value.ToString(), formatConvert);
            }
            long pid = DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString());
            PLNWorkOrderDeadline obj = this.LoadObject(pid);
            obj.WoPid = DBConvert.ParseLong(cmbWo.SelectedRow.Cells["Pid"].Value.ToString());
            obj.WorkStation = DBConvert.ParseLong(rowChild.Cells["WorkAreaPid"].Value.ToString());
            obj.CarcassCode = row.Cells["CarcassCode"].Value.ToString();
            obj.Qty = DBConvert.ParseInt(rowChild.Cells["Qty"].Value.ToString());
            if (rowChild.Cells["Deadline"].Value.ToString() != string.Empty)
            {
              obj.Deadline = DBConvert.ParseDateTime(rowChild.Cells["Deadline"].Value.ToString(), formatConvert);
            }
            if (rowChild.Cells["DeadlineStatus"].Value.ToString() != string.Empty)
            {
              obj.DeadlineStatus = DBConvert.ParseInt(rowChild.Cells["DeadlineStatus"].Value.ToString());
            }
            else
              obj.DeadlineStatus = 0;
            obj.FlagIdentity = 1;
            if (rowState == 1)
            {
              if (pid != long.MinValue)
              {
                obj.UpdateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
                obj.UpdateDate = currentDate;
                bool isUpdated = Shared.DataBaseUtility.DataBaseAccess.UpdateObject(obj, new string[] { "Pid" });
                if (!isUpdated)
                {
                  result = false;
                }
              }
              else
              {
                obj.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
                obj.CreateDate = currentDate;
                long OrderDeadlinePid = Shared.DataBaseUtility.DataBaseAccess.InsertObject(obj);
                if (OrderDeadlinePid <= 0)
                {
                  result = false;
                }
              }
            }
            //save history // Ha Anh them// hung moi sua lai ngay 15/11/2012
            if (indexRowChild == 0 && state == true)
            {
              SaveWorkOrderDeadLineHistory("spPLNWorkDeadLineHistory_UpdateFlagLasteHis", obj.WoPid, obj.WorkStation, obj.ItemCode, obj.Revision, obj.CarcassCode, obj.Qty, obj.Deadline);
            }
            if (state == true)
            {
              SaveWorkOrderDeadLineHistory("spPLNWorkDeadLineHistory_Insert", obj.WoPid, obj.WorkStation, obj.ItemCode, obj.Revision, obj.CarcassCode, obj.Qty, obj.Deadline);
            }
          }

        }
      }
      return result;
    }

    private void SaveWorkOrderDeadLineHistory(string storename, long Wo, long WoArea, string ItemCode, int Revision, string CarcassCode, int Qty, DateTime DeadLine)
    {
      DBParameter[] param = new DBParameter[8];
      param[0] = new DBParameter("@Wo", DbType.Int64, Wo);
      param[1] = new DBParameter("@WoArea", DbType.Int64, WoArea);
      if (ItemCode.ToString().Length > 0)
      {
        param[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ItemCode);
      }
      if (Revision != Int32.MinValue)
      {
        param[3] = new DBParameter("@Revision", DbType.Int32, Revision);
      }
      if (CarcassCode.ToString().Length > 0)
      {
        param[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, CarcassCode);
      }
      param[5] = new DBParameter("@Qty", DbType.Int32, Qty);
      if (DeadLine != DateTime.MinValue)
      {
        param[6] = new DBParameter("@DeadLine", DbType.DateTime, DeadLine);
      }
      param[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DataBaseAccess.ExecuteStoreProcedure(storename, param);
    }

    private bool SaveCarcassArea()
    {
      bool result = true;
      DateTime currentDate = DateTime.Today;
      // Insert / Update
      for (int i = 0; i < ultGridCarcassWorkArea.Rows.Count; i++)
      {
        UltraGridRow row = ultGridCarcassWorkArea.Rows[i];
        int countBand = row.ChildBands.Count;
        for (int indexBand = 0; indexBand < countBand; indexBand++)
        {
          bool state = false;
          int countRowChild = row.ChildBands[indexBand].Rows.Count;
          for (int indexState = 0; indexState < countRowChild; indexState++)
          {
            UltraGridRow rowChildState = row.ChildBands[indexBand].Rows[indexState];
            if (DBConvert.ParseInt(rowChildState.Cells["RowState"].Value.ToString()) == 1)
            {
              state = true;
            }
          }
          for (int indexRowChild = 0; indexRowChild < countRowChild; indexRowChild++)
          {
            UltraGridRow rowChild = row.ChildBands[indexBand].Rows[indexRowChild];
            int rowState = DBConvert.ParseInt(rowChild.Cells["RowState"].Value.ToString());
            long pid = DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString());
            PLNWorkOrderDeadline obj = this.LoadObject(pid);
            obj.WoPid = DBConvert.ParseLong(cmbWo.SelectedRow.Cells["Pid"].Value.ToString());
            obj.WorkStation = DBConvert.ParseLong(rowChild.Cells["WorkAreaPid"].Value.ToString());
            obj.ItemCode = row.Cells["ItemCode"].Value.ToString();
            obj.Revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
            obj.Qty = DBConvert.ParseInt(rowChild.Cells["Qty"].Value.ToString());
            DateTime deadline = new DateTime();
            if (rowChild.Cells["Deadline"].Value.ToString() != "")
            {
              deadline = DBConvert.ParseDateTime(rowChild.Cells["Deadline"].Value.ToString(), formatConvert);
            }
            obj.Deadline = deadline;
            if (rowChild.Cells["DeadlineStatus"].Value.ToString() != string.Empty)
            {
              obj.DeadlineStatus = DBConvert.ParseInt(rowChild.Cells["DeadlineStatus"].Value.ToString());
            }
            else
              obj.DeadlineStatus = 0;
            obj.FlagIdentity = 1;
            if (rowState == 1)
            {
              if (pid != long.MinValue)
              {
                obj.UpdateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
                obj.UpdateDate = currentDate;
                bool isUpdated = Shared.DataBaseUtility.DataBaseAccess.UpdateObject(obj, new string[] { "Pid" });
                if (!isUpdated)
                {
                  result = false;
                }
              }
              else
              {
                obj.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
                obj.CreateDate = currentDate;
                long OrderDeadlinePid = Shared.DataBaseUtility.DataBaseAccess.InsertObject(obj);
                if (OrderDeadlinePid <= 0)
                {
                  result = false;
                }
              }
            }
            if (indexRowChild == 0 && state == true)
            {
              SaveWorkOrderDeadLineHistory("spPLNWorkDeadLineHistory_UpdateFlagLasteHis", obj.WoPid, obj.WorkStation, obj.ItemCode, obj.Revision, obj.CarcassCode, obj.Qty, obj.Deadline);
            }
            if (state == true)
            {
              SaveWorkOrderDeadLineHistory("spPLNWorkDeadLineHistory_Insert", obj.WoPid, obj.WorkStation, obj.ItemCode, obj.Revision, obj.CarcassCode, obj.Qty, obj.Deadline);
            }
          }
        }
      }
      return result;
    }

    private bool DeleteData()
    {
      bool result = true;
      // Delete
      foreach (long pidDeleted in this.listDeleted)
      {
        PLNWorkOrderDeadline deleteObject = new PLNWorkOrderDeadline();
        deleteObject.Pid = pidDeleted;
        result = Shared.DataBaseUtility.DataBaseAccess.DeleteObject(deleteObject, new string[] { "Pid" });
      }
      return result;
    }

    private void SaveData()
    {
      bool result = true;
      if (this.IsValidData())
      {
        result = this.DeleteData();
        if (!result)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
        }
        result = this.SaveCompArea();
        if (!result)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0037", "at Component Work Area");
        }
        result = this.SaveCarcassArea();
        if (result)
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          this.NeedToSave = false;
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0037", "at Carcass Work Area");
        }
        this.LoadData();
      }
    }

    #endregion Check And Save

    #region Event

    private void btnClose_Click(object sender, EventArgs e)
    {
      FormSerialisor.Serialise(this, System.Windows.Forms.Application.StartupPath + @"\viewPLN_02_004.xml");
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (cmbWo.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Work Order");
        return;
      }
      if (this.NeedToSave)
      {
        this.SaveData();
      }
    }

    private void cmbWo_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!this.isBindWo && cmbWo.Value != null)
      {
        this.woPid = DBConvert.ParseLong(cmbWo.SelectedRow.Cells["Pid"].Value.ToString());
        this.LoadData();
      }
    }

    private void ultData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      long pid = DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString());
      e.Cell.Row.Cells["RowState"].Value = 1;
      this.NeedToSave = true;
    }

    private void ultGridCarcassWorkArea_AfterCellUpdate(object sender, CellEventArgs e)
    {
      long pid = DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString());
      e.Cell.Row.Cells["RowState"].Value = 1;
      this.NeedToSave = true;
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeleting)
      {
        this.listDeleted.Add(pid);
      }
      this.NeedToSave = true;
    }

    private void ultGridCarcassWorkArea_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeleting)
      {
        this.listDeleted.Add(pid);
      }
      this.NeedToSave = true;
    }

    private void ultData_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      this.listDeleting = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pidDelete = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pidDelete != long.MinValue)
        {
          this.listDeleting.Add(pidDelete);
        }
      }
    }

    private void ultGridCarcassWorkArea_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeleting = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pidDelete = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pidDelete != long.MinValue)
        {
          this.listDeleting.Add(pidDelete);
        }
      }
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      UltraGrid grid = (UltraGrid)sender;
      object obj = grid.DataSource;
      if (obj != null)
      {
        e.Layout.AutoFitColumns = true;
        e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.ViewStyleBand = ViewStyleBand.Horizontal;
        e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;
        e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
        e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";

        e.Layout.Bands[0].Columns["Qty"].MinWidth = 30;
        e.Layout.Bands[0].Columns["Qty"].MaxWidth = 30;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        DataSet ds = (DataSet)obj;
        for (int i = 1; i < ds.Tables.Count; i++)
        {
          e.Layout.Bands[i].HeaderVisible = true;
          e.Layout.Bands[i].Header.Caption = listComWorkAreaName[i - 1].ToString();
          e.Layout.Bands[i].Columns["DeadlineStatus"].ValueList = ultdrDeadlineStatus;
          e.Layout.Bands[i].Columns["DeadlineStatus"].MaxWidth = 70;
          e.Layout.Bands[i].Columns["DeadlineStatus"].MinWidth = 70;
          e.Layout.Bands[i].Columns["WorkAreaPid"].Hidden = true;
          e.Layout.Bands[i].Columns["Deadline"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
          e.Layout.Bands[i].Columns["Deadline"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
          e.Layout.Bands[i].Columns["RowState"].Hidden = true;
          e.Layout.Bands[i].Columns["Pid"].Hidden = true;
          e.Layout.Bands[i].Columns["CarcassCode"].Hidden = true;
          e.Layout.Bands[i].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
          e.Layout.Bands[i].Override.AllowUpdate = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
          if (i > 1)
          {
            e.Layout.Bands[i].ColHeadersVisible = false;
          }
        }
      }
    }

    private void ultGridCarcassWorkArea_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultGridCarcassWorkArea);
      UltraGrid grid = (UltraGrid)sender;
      object obj = grid.DataSource;
      if (obj != null)
      {
        e.Layout.AutoFitColumns = true;
        e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.ViewStyleBand = ViewStyleBand.Horizontal;
        e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
        e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
        e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";


        e.Layout.Bands[0].Columns["Revision"].MinWidth = 30;
        e.Layout.Bands[0].Columns["Revision"].MaxWidth = 30;
        e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Rev.";
        e.Layout.Bands[0].Columns["Qty"].MinWidth = 40;
        e.Layout.Bands[0].Columns["Qty"].MaxWidth = 40;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        DataSet ds = (DataSet)obj;
        for (int i = 1; i < ds.Tables.Count; i++)
        {
          e.Layout.Bands[i].HeaderVisible = true;
          e.Layout.Bands[i].Header.Caption = listComWorkAreaName[i - 1].ToString();
          e.Layout.Bands[i].Columns["DeadlineStatus"].ValueList = ultdrDeadlineStatus;
          e.Layout.Bands[i].Columns["DeadlineStatus"].MaxWidth = 70;
          e.Layout.Bands[i].Columns["DeadlineStatus"].MinWidth = 70;
          e.Layout.Bands[i].Columns["WorkAreaPid"].Hidden = true;
          e.Layout.Bands[i].Columns["Deadline"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
          e.Layout.Bands[i].Columns["Deadline"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
          e.Layout.Bands[i].Columns["RowState"].Hidden = true;
          e.Layout.Bands[i].Columns["Pid"].Hidden = true;
          e.Layout.Bands[i].Columns["ItemCode"].Hidden = true;
          e.Layout.Bands[i].Columns["Revision"].Hidden = true;
          e.Layout.Bands[i].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
          e.Layout.Bands[i].Override.AllowUpdate = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
          if (i > 1)
          {
            e.Layout.Bands[i].ColHeadersVisible = false;
          }
        }
      }
    }

    private void chkCompExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkCompExpandAll.Checked)
      {
        ultGridCompWorkArea.Rows.ExpandAll(true);
      }
      else
      {
        ultGridCompWorkArea.Rows.CollapseAll(true);
      }
    }

    private void chkCarcassExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkCarcassExpandAll.Checked)
      {
        ultGridCarcassWorkArea.Rows.ExpandAll(true);
      }
      else
      {
        ultGridCarcassWorkArea.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// Full Fill Qty
    /// Truong_Update 1/11/2011
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFullFillQty_Click(object sender, EventArgs e)
    {
      //Component Work Area

      DataSet ds = (DataSet)ultGridCompWorkArea.DataSource;
      for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
      {
        for (int j = 1; j < ds.Tables.Count; j++)
        {
          int totalQtyDetail = 0;
          if (ds.Tables[j].Rows.Count > 0)
          {
            for (int k = 0; k < ds.Tables[j].Rows.Count - 1; k++)
            {
              //ds.Tables[j].Rows.RemoveAt(k);
              //k--;
              totalQtyDetail += DBConvert.ParseInt(ds.Tables[j].Rows[k]["Qty"].ToString());
            }
            if (totalQtyDetail < DBConvert.ParseInt(ds.Tables[0].Rows[i]["Qty"].ToString()))
            {
              DataRow row1 = ds.Tables[j].NewRow();
              row1["CarcassCode"] = ds.Tables[0].Rows[i]["CarcassCode"].ToString();
              row1["Qty"] = DBConvert.ParseInt(ds.Tables[0].Rows[i]["Qty"].ToString()) - totalQtyDetail;
              row1["DeadlineStatus"] = 0;
              ds.Tables[j].Rows.Add(row1);
            }
          }
          else
          {
            DataRow row1 = ds.Tables[j].NewRow();
            row1["CarcassCode"] = ds.Tables[0].Rows[i]["CarcassCode"].ToString();
            row1["Qty"] = DBConvert.ParseInt(ds.Tables[0].Rows[i]["Qty"].ToString().Trim());
            row1["DeadlineStatus"] = 0;
            ds.Tables[j].Rows.Add(row1);
          }
        }
      }
      ultGridCompWorkArea.DataSource = ds;
      ultGridCompWorkArea.Rows.ExpandAll(true);

      //Carcass Work Area

      DataSet dset = (DataSet)ultGridCarcassWorkArea.DataSource;
      for (int i = 0; i < dset.Tables[0].Rows.Count; i++)
      {
        for (int j = 1; j < dset.Tables.Count; j++)
        {
          int totalQtyDetail = 0;
          if (dset.Tables[j].Rows.Count > 0)
          {
            for (int k = 0; k < dset.Tables[j].Rows.Count; k++)
            {
              //ds.Tables[j].Rows.RemoveAt(k);
              //k--;
              totalQtyDetail += DBConvert.ParseInt(dset.Tables[j].Rows[k]["Qty"].ToString());
            }
            if (totalQtyDetail < DBConvert.ParseInt(dset.Tables[0].Rows[i]["Qty"].ToString().Trim()))
            {
              DataRow row1 = dset.Tables[j].NewRow();
              row1["ItemCode"] = dset.Tables[0].Rows[i]["ItemCode"].ToString();
              row1["Revision"] = dset.Tables[0].Rows[i]["Revision"].ToString();
              row1["Qty"] = DBConvert.ParseInt(dset.Tables[0].Rows[i]["Qty"].ToString()) - totalQtyDetail;
              row1["DeadlineStatus"] = 0;
              dset.Tables[j].Rows.Add(row1);
            }
          }

          else
          {
            DataRow row1 = dset.Tables[j].NewRow();
            row1["ItemCode"] = dset.Tables[0].Rows[i]["ItemCode"].ToString();
            row1["Revision"] = dset.Tables[0].Rows[i]["Revision"].ToString();
            row1["Qty"] = DBConvert.ParseInt(dset.Tables[0].Rows[i]["Qty"].ToString());
            row1["DeadlineStatus"] = 0;
            dset.Tables[j].Rows.Add(row1);
          }
        }
      }
      ultGridCarcassWorkArea.DataSource = dset;
      ultGridCarcassWorkArea.Rows.ExpandAll(true);
    }


    #endregion Event


  }
}
