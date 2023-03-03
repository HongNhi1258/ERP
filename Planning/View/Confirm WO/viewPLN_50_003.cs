/*
   Author      : Nguyen Huynh Quoc Tuan
  Date        : 27/5/2016
  Description : 
  Standard Form: viewPLN_50_006.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_50_003 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
      this.LoadCBStatus();
    }

    private void LoadCBStatus()
    {
      string cm = @"SELECT 0 value, 'New' [text]
                    UNION
                    SELECT 1, 'PLN Confirmed'
                    UNION
                    SELECT 2, 'PPD Confirmed'
                    UNION
                    SELECT 3, 'PLN Finished'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultcbStatus, dt, "value", "text", false, "value");
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 5;
      string storeName = "spPLNConfirmWOL_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (DBConvert.ParseLong(txtWO.Text.Trim()) >= 0)
      {
        inputParam[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(txtWO.Text.Trim()));
      }

      if (txtItemCode.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text.Trim());
      }

      if (ultcbStatus.Value != null)
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultcbStatus.Value.ToString()));
      }

      if (ultdtDateFrom.Value != null)
      {
        inputParam[3] = new DBParameter("@DateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultdtDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }

      if (ultdtDateTo.Value != null)
      {
        inputParam[4] = new DBParameter("@DateTo", DbType.DateTime, DBConvert.ParseDateTime(ultdtDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      dsSource.Relations.Add(new DataRelation("Parent_Child", dsSource.Tables[0].Columns["Pid"], dsSource.Tables[1].Columns["TransactionPid"], false));
      ultraGridInformation.DataSource = dsSource;

      if (dsSource != null)
      {
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (ultraGridInformation.Rows[i].Cells["Status"].Value.ToString() == "1")
          {
            ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.YellowGreen;
            ultraGridInformation.Rows[i].Activation = Activation.ActivateOnly;
          }
          else if (ultraGridInformation.Rows[i].Cells["Status"].Value.ToString() == "2")
          {
            ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.SandyBrown;
            ultraGridInformation.Rows[i].Activation = Activation.ActivateOnly;
          }
          else if (ultraGridInformation.Rows[i].Cells["Status"].Value.ToString() == "3")
          {
            ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.Violet;
            ultraGridInformation.Rows[i].Activation = Activation.ActivateOnly;
          }
          else
          {
            ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.Empty;
            ultraGridInformation.Rows[i].Activation = Activation.AllowEdit;
          }
        }
      }

      lbCount.Text = string.Format("Count: {0}", (dsSource != null ? dsSource.Tables[0].Rows.Count : 0));
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtWO.Text = string.Empty;
      txtItemCode.Text = string.Empty;
      ultcbStatus.Value = null;
      ultdtDateFrom.Value = string.Empty;
      ultdtDateFrom.Value = null;
      ultdtDateTo.Value = null;
      txtWO.Focus();
    }
    #endregion function

    #region event
    public viewPLN_50_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_50_003_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupBoxSearch.Controls)
      {
        ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      }
      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.SearchData();
      btnSearch.Enabled = true;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
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
      //e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["isDelete"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["isDelete"].CellActivation = Activation.AllowEdit;

      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        Type colType = e.Layout.Bands[1].Columns[j].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[1].Columns[j].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["TransactionPid"].Hidden = true;

      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[1].ColHeaderLines = 3;
      e.Layout.Bands[1].Columns["PAKConfirmedDeadline"].Header.Caption = "PAK\nConfirmed\nDeadline";
      e.Layout.Bands[1].Columns["COM1ConfirmedDeadline"].Header.Caption = "COM1\nConfirmed\nDeadline";
      e.Layout.Bands[1].Columns["SUBConfirmedDeadline"].Header.Caption = "SUB\nConfirmed\nDeadline";
      e.Layout.Bands[1].Columns["ASSY_SandingConfirmedDeadline"].Header.Caption = "ASSY_Sanding\nConfirmed\nDeadline";
      e.Layout.Bands[1].Columns["SAMConfirmedDeadline"].Header.Caption = "SAM\nConfirmed\nDeadline";
      e.Layout.Bands[1].Columns["ASSHWDeadline"].Header.Caption = "ASSYHW\nDeadline";
      e.Layout.Bands[1].Columns["FFHWDeadline"].Header.Caption = "FFHW\nDeadline";
      e.Layout.Bands[1].Columns["MATDeadline"].Header.Caption = "MAT\nDeadline";
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_50_001 view = new viewPLN_50_001();
      if (rdWONew.Checked)
      {
        view.type = 0;
      }
      else if (rdWORevise.Checked)
      {
        view.type = 1;
      }
      WindowUtinity.ShowView(view, "New Confirm WO Online", true, ViewState.MainWindow);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {

    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridInformation_DoubleClick(object sender, EventArgs e)
    {
      if (ultraGridInformation.Selected.Rows.Count > 0 && ultraGridInformation.Selected.Rows[0].Band.ParentBand == null)
      {
        long pid = DBConvert.ParseLong(ultraGridInformation.Selected.Rows[0].Cells["Pid"].Value.ToString());
        viewPLN_50_001 view = new viewPLN_50_001();
        view.transactionPid = pid;
        view.type = DBConvert.ParseInt(ultraGridInformation.Selected.Rows[0].Cells["Type"].Value.ToString());
        WindowUtinity.ShowView(view, "Update Confirm WO Online", true, ViewState.MainWindow);
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      string strDelete = string.Empty;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridInformation.Rows[i];
        if (row.Cells["isDelete"].Value.ToString() == "1")
        {
          if (strDelete == string.Empty)
          {
            strDelete = row.Cells["Pid"].Value.ToString();
          }
          else
          {
            strDelete += ',' + row.Cells["Pid"].Value.ToString();
          }
        }
      }
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@StrPid", DbType.String, strDelete);

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNConfirmWODraftDeadline_Delete", input, output);
      int result = DBConvert.ParseInt(output[0].Value.ToString());
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0004");
        this.SaveSuccess = false;
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
        this.SaveSuccess = true;
      }
      this.SearchData();
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.BOMShowItemImage(ultraGridInformation, grpItemPicture, picItem, chkShowImage.Checked);
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      Shared.Utility.ControlUtility.BOMShowItemImage(ultraGridInformation, grpItemPicture, picItem, chkShowImage.Checked);
    }

    #endregion event
  }
}
