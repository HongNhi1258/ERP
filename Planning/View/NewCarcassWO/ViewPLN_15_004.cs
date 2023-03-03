/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_004.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class ViewPLN_15_004 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      int paramNumber = 4;
      string storeName = "spPLNSelectAdjustWoCarcassMaster";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtWO.Text.Length > 0)
      {
        inputParam[0] = new DBParameter("@WO", DbType.Int32, txtWO.Text.ToString());
      }
      if (txtCarcassCode.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@AdjusNo", DbType.String, txtAdjustNo.Text.ToString());
      }
      if (txtCarcassCode.Text.Length > 0)
      {
        inputParam[2] = new DBParameter("@CarcassCode", DbType.String, txtCarcassCode.Text.ToString());
      }
      if (txtItemCode.Text.Length > 0)
      {
        inputParam[3] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text.ToString());
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);

      DataSet ds = this.CreateDataSet();
      ds.Tables["dtParent"].Merge(dsSource.Tables[0]);
      ds.Tables["dtChild"].Merge(dsSource.Tables[1]);
      ultraGridInformation.DataSource = ds;
      lbCount.Text = string.Format("Count: {0}", (ds != null ? ultraGridInformation.Rows.Count : 0));
      btnSearch.Enabled = true;

      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow ur = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(ur.Cells["Status"].Value.ToString()) == 1)
        {
          ur.CellAppearance.BackColor = Color.LightGreen;
        }
        else if (DBConvert.ParseInt(ur.Cells["Status"].Value.ToString()) == 2)
        {
          ur.CellAppearance.BackColor = Color.Yellow;
        }
        else if (DBConvert.ParseInt(ur.Cells["Status"].Value.ToString()) == 3)
        {
          ur.CellAppearance.BackColor = Color.Pink;
        }
      }
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      this.txtAdjustNo.Text = "";
      this.txtCarcassCode.Text = "";
      this.txtItemCode.Text = "";
      this.txtWO.Text = "";



    }
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("AdjCarcassWONo", typeof(System.String));
      taParent.Columns.Add("HoTen", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("AdjustPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("ConfirmQty", typeof(System.Int32));
      ds.Tables.Add(taChild);
      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["Pid"] }, new DataColumn[] { taChild.Columns["AdjustPid"] }, false));
      return ds;
    }
    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }

    private void SetNeedToSave()
    {
      //if (btnSave.Enabled && btnSave.Visible)
      //{
      //  this.NeedToSave = true;
      //}
      //else
      //{
      //  this.NeedToSave = false;
      //}
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //if (ultCBWo.Text.Length == 0)
      //{
      //  errorMessage = "Work Order";      
      //  return false;
      //}
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[7];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[6] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.SearchData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public ViewPLN_15_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewPLN_15_004_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["AdjustPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;

      e.Layout.Bands[0].Columns["Hoten"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["AdjCarcassWONo"].Header.Caption = "Adjust NO";


    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }
    private void button1_Click(object sender, EventArgs e)
    {
      ViewPLN_15_005 view = new ViewPLN_15_005();
      WindowUtinity.ShowView(view, "Carcass WO Detail", true, ViewState.MainWindow);
    }
    private void ultraGridInformation_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridInformation.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (!ultraGridInformation.Selected.Rows[0].HasParent()) ? ultraGridInformation.Selected.Rows[0] : ultraGridInformation.Selected.Rows[0].ParentRow;
      long Pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      ViewPLN_15_005 view = new ViewPLN_15_005();
      view.AdjustPid = Pid;
      WindowUtinity.ShowView(view, "Carcass WO Detail", true, ViewState.MainWindow);
    }
    #endregion event


  }
}
