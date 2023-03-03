/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_001.cs
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
  public partial class ViewPLN_15_001 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    public long CarcassWOPid;
    private int TotalQty;
    private long TransactionPid;

    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
      this.SearchData();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      int paramNumber = 1;
      string storeName = "spPLNSelectFunitureCarcassWo";
      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@PidCarcassWO", DbType.Int64, this.CarcassWOPid);
      DataSet ds = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      ultraGridInformation.DataSource = ds.Tables[1];
      Maindata(ds.Tables[0]);

      lbCount.Text = string.Format("Count: {0}", (ds.Tables[1] != null ? ds.Tables[1].Rows.Count : 0));
      int total = 0;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow ur = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(ur.Cells["Select"].Value.ToString()) != 0)
        {
          total = total + 1;
        }
      }
      this.TotalQty = total;
      lbTotal.Text = string.Format("Total Qty: {0}", (ds.Tables[1] != null ? total : 0));

    }

    private void Maindata(DataTable dt)
    {
      txtWO.Text = dt.Rows[0]["WO"].ToString();
      txtCarcassCode.Text = dt.Rows[0]["CarcassCode"].ToString();
      txtItemCode.Text = dt.Rows[0]["ItemCode"].ToString();
      txtItemRevision.Text = dt.Rows[0]["Revision"].ToString();
      txtTotalQty.Text = dt.Rows[0]["TotalQty"].ToString();
      this.TransactionPid = DBConvert.ParseLong(dt.Rows[0]["TransactionPid"].ToString());
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

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
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (this.TotalQty != DBConvert.ParseInt(txtTotalQty.Text))
      {
        errorMessage = "Total Qty";
        return false;
      }
      return true;
    }
    private bool SaveDetail()
    {
      bool success = true;
      string storeName = "spPLNCarcassWODetailFurniture_Edit";
      // 1. Delete
      DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {

        if (DBConvert.ParseInt(row["Selected"].ToString()) > 0 && DBConvert.ParseInt(row["Select"].ToString()) == 0)
        {
          DBParameter[] inputParam = new DBParameter[1];
          long pid = DBConvert.ParseLong(row["Selected"].ToString());
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      // 2. Insert         
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Modified && DBConvert.ParseInt(row["Selected"].ToString()) == 0)
        {
          if (DBConvert.ParseInt(row["Select"].ToString()) != 0)
          {
            DBParameter[] inputParam = new DBParameter[2];
            inputParam[0] = new DBParameter("@TransactionPid", DbType.Int64, this.TransactionPid);
            inputParam[1] = new DBParameter("@FunitureCodePid", DbType.Int64, row["Pid"].ToString());

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
      }

      return success;
    }
    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = this.SaveDetail();
        // 1. Delete      

        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.SearchData();
        this.NeedToSave = false;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
        this.NeedToSave = true;
      }

    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();

    }
    #endregion function

    #region event
    public ViewPLN_15_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewPLN_15_001_Load(object sender, EventArgs e)
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
        //this.SearchData();
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
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Hidden = true;

      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["FurnitureCode"].Header.Caption = "Furniture Code";

      e.Layout.Bands[0].Columns["StandByEN"].Header.Caption = "Stand By";

      e.Layout.Bands[0].Columns["FurnitureCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StandByEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Error"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Activation = Activation.ActivateOnly;
          ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.LightGray;
        }
      }


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

    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      string colName = e.Cell.Column.ToString();
      string value = e.Cell.Value.ToString();
      switch (colName)
      {
        case "Select":

          if (DBConvert.ParseInt(value) != 0)
          {
            this.TotalQty = TotalQty + 1;
          }
          else
          {
            this.TotalQty = TotalQty - 1;
          }
          lbTotal.Text = string.Format("Total Qty: {0}", TotalQty);
          break;
        default:
          break;
      }
    }
    private void button1_Click(object sender, EventArgs e)
    {

      this.SaveData();
      if (!this.NeedToSave)
      {
        this.ConfirmToCloseTab();
      }

    }
    private void button2_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[1];
      string storeName = "spPLNRefreshfinishCarcassWO";
      inputParam[0] = new DBParameter("@CarcassWOPid", DbType.Int64, this.TransactionPid);
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam);
      this.SearchData();
    }
    #endregion event




  }
}
