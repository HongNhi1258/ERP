/*
  Author      : 
  Date        : 02/11/2012
  Description : Container Priority
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_003 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    public viewPLN_98_003()
    {
      InitializeComponent();
    }

    private void viewWHD_05_002_Load(object sender, EventArgs e)
    {
      this.Search();
    }

    void StartForm(ProgressForm form)
    {
      DialogResult result = form.ShowDialog();
      if (result == DialogResult.Cancel)
        MessageBox.Show("Operation has been cancelled");
      if (result == DialogResult.Abort)
        MessageBox.Show("Exception:" + Environment.NewLine + form.Result.Error.Message);
    }

    void form_DoWork(ProgressForm sender, DoWorkEventArgs e)
    {
      bool throwException = (bool)e.Argument;

      for (int i = 0; i < 100; i++)
      {
        System.Threading.Thread.Sleep(600);
        sender.SetProgress(i, "Step " + i.ToString() + " %");
        if (sender.CancellationPending)
        {
          e.Cancel = true;
          return;
        }
      }

    }

    private void Progessbar()
    {

      ProgressForm form = new ProgressForm();
      form.Text = "Please Waiting";
      form.Argument = false;
      form.DoWork += new ProgressForm.DoWorkEventHandler(form_DoWork);
      StartForm(form);
      form.FormBorderStyle = FormBorderStyle.None;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[5];

      // SaleCode
      string text = string.Empty;
      text = this.txtSaleCode.Text;
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@SaleCode", DbType.String, text);
      }

      // Carcass Code
      text = this.txtCarcassCode.Text;
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@CarcassCode", DbType.String, text);
      }

      // Item Code
      text = this.txtItemCode.Text;
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@ItemCode", DbType.String, text);
      }

      // Old Code
      text = this.txtOldCode.Text;
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@OldCode", DbType.String, text);
      }

      // WO
      text = this.txtWO.Text;
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@WO", DbType.String, text);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanContainerContainerPriority_Select", 5000, param);
      if (dtSource != null)
      {
        this.ultData.DataSource = dtSource;
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int type = DBConvert.ParseInt(ultData.Rows[i].Cells["CustomerPriority"].Value.ToString());
        if (type == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }

        ultData.Rows[i].Cells["Priority"].Appearance.BackColor = Color.LightBlue;
      }
    }

    /// <summary>
    /// Check valid before save
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;

      DataTable dt = (DataTable)this.ultData.DataSource;
      foreach (DataRow row in dt.Rows)
      {
        int wo = DBConvert.ParseInt(row["WO"].ToString());
        string itemCode = row["ItemCode"].ToString();
        int revision = DBConvert.ParseInt(row["Revision"].ToString());
        int priority = DBConvert.ParseInt(row["Priority"].ToString());
        // Check Priority
        if (priority <= 0)
        {
          message = "Priority";
          return false;
        }

        DateTime shipDate = DBConvert.ParseDateTime(row["ShipDate"].ToString()
                                    , USER_COMPUTER_FORMAT_DATETIME);

        // Check Duplicate
        string sql = "WO =" + wo + " AND ItemCode ='" + itemCode + "' AND Revision =" + revision + " AND Priority =" + priority;
        sql += " AND MonthNo =" + shipDate.Month + " AND YearNo =" + shipDate.Year;
        DataRow[] foundRow = dt.Select(sql);
        if (foundRow.Length > 1)
        {
          message = "WO =" + wo + ";ItemCode ='" + itemCode + "';Revision=" + revision;
          return false;
        }
      }

      return true;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Init Layout 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerPriority"].Hidden = true;
      e.Layout.Bands[0].Columns["MonthNo"].Hidden = true;
      e.Layout.Bands[0].Columns["YearNo"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipDate"].Hidden = true;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Priority"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      try
      {
        DataTable dtData = (DataTable)this.ultData.DataSource;
        foreach (DataRow row in dtData.Rows)
        {
          if (row.RowState == DataRowState.Modified)
          {
            long pid = DBConvert.ParseLong(row["PID"].ToString());
            int priority = DBConvert.ParseInt(row["Priority"].ToString());
            DBParameter[] inputParam = new DBParameter[2];
            inputParam[0] = new DBParameter("@PID", DbType.Int64, pid);
            inputParam[1] = new DBParameter("@Priority", DbType.Int32, priority);

            DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerUpdatePriority_Update", inputParam);
          }
        }

        // Refresh Data Container Priority
        DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");

        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.Search();
    }

    //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    //{
    //  if (keyData == Keys.Enter)
    //  {
    //    this.Search();
    //    return true;
    //  }
    //  return base.ProcessCmdKey(ref msg, keyData);
    //}

    //private void btnRefreshData_Click(object sender, EventArgs e)
    //{
    //  System.Threading.Thread Thead2 = new System.Threading.Thread(new System.Threading.ThreadStart(Progessbar));
    //  Thead2.Start();
    //  DBParameter[] inputParam = new DBParameter[2];
    //  DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerSummarizeTotalData_Insert", 300, inputParam);
    //  WindowUtinity.ShowMessageSuccess("MSG0059");
    //  this.Search();
    //}

    private void btnDefault_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[2];
      DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriority_Insert", 300, inputParam);
      WindowUtinity.ShowMessageSuccess("MSG0059");
      this.Search();
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtCarcassCode.Text = string.Empty;
      txtItemCode.Text = string.Empty;
      txtOldCode.Text = string.Empty;
      txtSaleCode.Text = string.Empty;
      txtWO.Text = string.Empty;
    }
    #endregion Event
  }
}
