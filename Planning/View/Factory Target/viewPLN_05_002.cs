using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Text;

namespace DaiCo.Planning
{
  public partial class viewPLN_05_002 : MainUserControl
  {
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private long customerGroupPid = 0;
    int checkActiveRow = 0;

    public viewPLN_05_002()
    {
      InitializeComponent();
    }

    private void viewPLN_05_002_Load(object sender, EventArgs e)
    {
      this.LoadCustomer();
      this.LoadKindCustomer();
      this.LoadData();
    }

    private void LoadCustomer()
    {
      string commandText = "SELECT Pid, CustomerCode, Name FROM TblCSDCustomerInfo";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDCustomer.DataSource = dt;

      //ultDDCustomer.DisplayLayout.AutoFitColumns = true;
      ultDDCustomer.DisplayMember = "CustomerCode";
      ultDDCustomer.ValueMember = "Pid";
      ultDDCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDCustomer.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    private void LoadKindCustomer()
    {
      StringBuilder commandText = new StringBuilder();
      commandText.Append("select DISTINCT CS.Kind,CM.Value \n");
      commandText.Append("from   TblCSDCustomerInfo cs \n");
      commandText.Append("       inner join TblBOMCodeMaster CM ON CM.[Group] = 2008 \n");
      commandText.Append("                                         and Code = cs.Kind ");

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText.ToString());

      ultDDKindCustomer.DataSource = dt;
      ultDDKindCustomer.DisplayLayout.AutoFitColumns = true;
      ultDDKindCustomer.DisplayMember = "Value";
      ultDDKindCustomer.ValueMember = "Kind";
      ultDDKindCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDKindCustomer.DisplayLayout.Bands[0].Columns["Kind"].Hidden = true;
    }

    private void LoadData()
    {
      StringBuilder commandText = new StringBuilder();
      commandText.Append("SELECT Pid,Name,DeleteFlg, 0 Status \n");
      commandText.Append("FROM   TblPLNCustomerGroup WHERE DeleteFlg = 0");
      DataTable dt1 = DataBaseAccess.SearchCommandTextDataTable(commandText.ToString());

      StringBuilder commandText1 = new StringBuilder();
      commandText1.Append("SELECT CGD.Pid CGDPid,CustomerGroupPid,CustomerPid,kind,Distribute,CGD.DeleteFlg,Direct, 0 Status \n");
      commandText1.Append("FROM   TblPLNCustomerGroupDetail CGD WHERE DeleteFlg = 0");
      DataTable dt2 = DataBaseAccess.SearchCommandTextDataTable(commandText1.ToString());

      dsPLNCustomerGroup ds = new dsPLNCustomerGroup();

      ds.Tables["CG"].Merge(dt1);
      ds.Tables["CGD"].Merge(dt2);

      ultGrid.DataSource = ds;
      ultGrid.Rows.ExpandAll(true);
    }

    private void ultGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["DeleteFlg"].Header.Caption = "Delete";
      e.Layout.Bands[0].Columns["DeleteFlg"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["CGDPid"].Hidden = true;
      e.Layout.Bands[1].Columns["CustomerGroupPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Kind"].ValueList = ultDDKindCustomer;
      e.Layout.Bands[1].Columns["Direct"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["DeleteFlg"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["CustomerPid"].ValueList = ultDDCustomer;
      e.Layout.Bands[1].Columns["Distribute"].ValueList = ultDDCustomer;
      e.Layout.Bands[1].Columns["CustomerPid"].Header.Caption = "Customer";
      e.Layout.Bands[1].Columns["Status"].Hidden = true;
      e.Layout.Bands[1].Columns["DeleteFlg"].Header.Caption = "Delete";
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultGrid_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName == "customerpid" && e.Cell.Row.Cells["CustomerPid"].Text.Trim().Length > 0)
      {
        string customer = e.Cell.Row.Cells["CustomerPid"].Text;
        int count = 0;
        for (int i = 0; i < ultDDCustomer.Rows.Count; i++)
        {
          if (ultDDCustomer.Rows[i].Cells["CustomerCode"].Text == customer)
          {
            count = 1;
            break;
          }
        }
        if (count == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Customer");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }

        //duplicate tren luoi
        //count = 0;
        //for (int i = 0; i < ultGrid.Rows.Count; i++)
        //{
        //  if (ultGrid.Rows[i].Cells["CustomerPid"].Text == e.Cell.Row.Cells["CustomerPid"].Text)
        //  {
        //    count++;
        //  }
        //  if (count == 2)
        //  {
        //    string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Group - Customer");
        //    WindowUtinity.ShowMessageErrorFromText(message);
        //    e.Cancel = true;
        //    break;
        //  }
        //}
      }

      if (colName == "distribute" && e.Cell.Row.Cells["Distribute"].Text.Trim().Length > 0)
      {
        string Distribute = e.Cell.Row.Cells["Distribute"].Text;
        int count = 0;
        for (int i = 0; i < ultDDCustomer.Rows.Count; i++)
        {
          if (ultDDCustomer.Rows[i].Cells["CustomerCode"].Text == Distribute)
          {
            count = 1;
            break;
          }
        }
        if (count == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Distribute");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }

        //duplicate tren luoi
        //count = 0;
        //for (int i = 0; i < ultGrid.Rows.Count; i++)
        //{
        //  if (ultGrid.Rows[i].Cells["Distribute"].Text == e.Cell.Row.Cells["Distribute"].Text)
        //  {
        //    count++;
        //  }
        //  if (count == 2)
        //  {
        //    string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Group - Distribute");
        //    WindowUtinity.ShowMessageErrorFromText(message);
        //    e.Cancel = true;
        //    break;
        //  }
        //}
      }

      if (colName == "kind" && e.Cell.Row.Cells["Kind"].Text.Trim().Length > 0)
      {
        string Kind = e.Cell.Row.Cells["Kind"].Text;
        int count = 0;
        for (int i = 0; i < ultDDKindCustomer.Rows.Count; i++)
        {
          if (ultDDKindCustomer.Rows[i].Cells["Value"].Text == Kind)
          {
            count = 1;
            break;
          }
        }
        if (count == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Kind");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }

        //duplicate tren luoi
        //count = 0;
        //for (int i = 0; i < ultGrid.Rows.Count; i++)
        //{
        //  if (ultGrid.Rows[i].Cells["Kind"].Text == e.Cell.Row.Cells["Kind"].Text)
        //  {
        //    count++;
        //  }
        //  if (count == 2)
        //  {
        //    string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Kind");
        //    WindowUtinity.ShowMessageErrorFromText(message);
        //    e.Cancel = true;
        //    break;
        //  }
        //}
      }
    }

    private void ultGrid_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (checkActiveRow == 1)
      {
        checkActiveRow = 0;
        return;
      }
      string colName = e.Cell.Column.ToString().ToUpper();
      if (e.Cell.Text.Length > 0)
      {
        if (e.Cell.Row.Cells.Exists("Name"))
        {
          if (e.Cell.Row.Cells["Pid"].Value.ToString().Length == 0)
          {
            e.Cell.Row.Cells["Pid"].Value = customerGroupPid--;
          }
        }
      }
      e.Cell.Row.Cells["Status"].Value = 1;
      try
      {
        e.Cell.Row.ParentRow.Cells["Status"].Value = 1;
      }
      catch { }
    }

    private void ultGrid_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    private void ultGrid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool result = this.save();
      if (result)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
    }

    private bool save()
    {
      for (int i = 0; i < ultGrid.Rows.Count; i++)
      {
        if (ultGrid.Rows[i].Cells["Status"].Value.ToString() == "1")
        {
          long customerGroupPid = long.MinValue;
          string storename = "spPLNCustomerGroupParent_Edit";
          DBParameter[] inputParam1 = new DBParameter[3];

          if (DBConvert.ParseLong(ultGrid.Rows[i].Cells["Pid"].Value.ToString()) > 0)
          {
            inputParam1[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultGrid.Rows[i].Cells["Pid"].Value.ToString()));
          }
          inputParam1[1] = new DBParameter("@Name", DbType.AnsiString, 128, ultGrid.Rows[i].Cells["Name"].Value.ToString());
          inputParam1[2] = new DBParameter("@Delete", DbType.Int32, DBConvert.ParseInt(ultGrid.Rows[i].Cells["DeleteFlg"].Value.ToString()));

          DBParameter[] outParam1 = new DBParameter[1];
          outParam1[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(storename, inputParam1, outParam1);
          if (DBConvert.ParseLong(outParam1[0].Value.ToString()) == 0 || DBConvert.ParseLong(outParam1[0].Value.ToString()) == long.MinValue)
          {
            return false;
          }
          else
          {
            customerGroupPid = DBConvert.ParseLong(outParam1[0].Value.ToString());
            for (int j = 0; j < ultGrid.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              UltraGridRow row = ultGrid.Rows[i].ChildBands[0].Rows[j];
              storename = "spPLNCustomerGroup_Edit";
              DBParameter[] inputParam = new DBParameter[7];
              if (DBConvert.ParseLong(row.Cells["CGDPid"].Value.ToString()) > 0)
              {
                inputParam[0] = new DBParameter("@CGDPid", DbType.Int64, DBConvert.ParseLong(row.Cells["CGDPid"].Value.ToString()));
              }
              inputParam[1] = new DBParameter("@CustomerGroupPid", DbType.Int64, customerGroupPid);
              if (DBConvert.ParseLong(row.Cells["CustomerPid"].Value.ToString()) != long.MinValue)
              {
                inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, DBConvert.ParseLong(row.Cells["CustomerPid"].Value.ToString()));
              }
              if (DBConvert.ParseInt(row.Cells["Direct"].Value.ToString()) == 1)
              {
                inputParam[3] = new DBParameter("@Direct", DbType.Int32, 1);
              }
              else
              {
                inputParam[3] = new DBParameter("@Direct", DbType.Int32, 0);
              }
              if (DBConvert.ParseLong(row.Cells["Distribute"].Value.ToString()) != long.MinValue)
              {
                inputParam[4] = new DBParameter("@Distribute", DbType.Int64, DBConvert.ParseLong(row.Cells["Distribute"].Value.ToString()));
              }
              if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) != int.MinValue)
              {
                inputParam[5] = new DBParameter("@Kind", DbType.Int32, DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()));
              }
              inputParam[6] = new DBParameter("@Delete", DbType.Int32, DBConvert.ParseInt(row.Cells["DeleteFlg"].Value.ToString()));

              DBParameter[] outParam = new DBParameter[1];
              outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);
              if (DBConvert.ParseLong(outParam[0].Value.ToString()) == 0 || DBConvert.ParseLong(outParam[0].Value.ToString()) == long.MinValue)
              {
                return false;
              }
            }
          }
        }
      }

      return true;
    }

    private void ultGrid_BeforeRowActivate(object sender, RowEventArgs e)
    {
      checkActiveRow = 1;
      e.Row.Cells["DeleteFlg"].Value = 0;
      if (e.Row.Cells.Exists("Direct"))
      {
        e.Row.Cells["Direct"].Value = 0;
      }
    }
  }
}
