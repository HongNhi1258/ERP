/*
  Author  : 
  Date    : 01/02/2011
  Description : Process ContainerList Between Warehouse & Planning
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_06_093 : MainUserControl
  {
    #region Field
    public long containerPid = long.MinValue;
    #endregion Field

    #region Init
    public viewPLN_06_093()
    {
      InitializeComponent();
    }

    /// <summary>
    /// User Control Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_021_Load(object sender, EventArgs e)
    {
      this.LoadControl();
      this.Search();
    }
    #endregion Init

    #region LoadData
    private void LoadControl()
    {
      string commandText = string.Empty;
      commandText += " SELECT COUNT(*)";
      commandText += " FROM TblPLNSHPContainer";
      commandText += " WHERE Pid =" + this.containerPid;
      commandText += "  AND [Confirm] = 3";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 1)
        {
          this.btnFinishSpecial.Enabled = false;
          this.btnFinish.Enabled = false;
        }
        else
        {
          this.btnFinishSpecial.Enabled = true;
          this.btnFinish.Enabled = true;
        }
      }
    }

    /// <summary>
    /// Search Box Information
    /// </summary>
    private void Search()
    {
      string commandText = string.Empty;
      commandText += " SELECT ContainerNo";
      commandText += " FROM TblPLNSHPContainer";
      commandText += " WHERE Pid = " + this.containerPid;

      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      this.txtContainerList.Text = dtCheck.Rows[0][0].ToString();

      DBParameter[] input = new DBParameter[1];

      input[0] = new DBParameter("@ContainerPid", DbType.Int64, this.containerPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNProcessContainer", input);
      DataSet ds = this.DataSetSearch();

      ds.Tables[0].Merge(dsSource.Tables[0]);
      ds.Tables[1].Merge(dsSource.Tables[1]);
      ultDetail.DataSource = ds;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          row.CellAppearance.BackColor = Color.LightBlue;
        }

        for (int j = 0; j < ultDetail.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (DBConvert.ParseInt(ultDetail.Rows[i].
                    ChildBands[0].Rows[j].Cells["Errors"].Value.ToString()) != 0)
          {
            ultDetail.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Yellow;
          }
        }
      }
    }

    private DataSet DataSetSearch()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("ItemGroup", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("Issued", typeof(System.Int32));
      taParent.Columns.Add("Errors", typeof(System.Int32));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("ContainerNo", typeof(System.String));
      taChild.Columns.Add("SaleNo", typeof(System.String));
      taChild.Columns.Add("ItemGroup", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["ItemGroup"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] },
              new DataColumn[] { taChild.Columns["ItemGroup"], taChild.Columns["ItemCode"], taChild.Columns["Revision"] }));
      return ds;
    }
    #endregion LoadData

    #region Process
    /// <summary>
    /// Check Search Data (Customer,ShipDate,Container Name)
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidFinishInfo(out string message)
    {
      message = string.Empty;

      DBParameter[] input = new DBParameter[1];

      input[0] = new DBParameter("@ContainerPid", DbType.Int64, containerPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNProcessContainer", input);
      if (dsSource != null && dsSource.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in dsSource.Tables[0].Rows)
        {
          if (DBConvert.ParseInt(row["Errors"].ToString()) != 0)
          {
            message = Shared.Utility.FunctionUtility.GetMessage("ERRO118");
            return false;
          }
        }

        foreach (DataRow row in dsSource.Tables[1].Rows)
        {
          if (DBConvert.ParseInt(row["Errors"].ToString()) != 0)
          {
            message = Shared.Utility.FunctionUtility.GetMessage("ERR0147");
            return false;
          }
        }
      }
      else
      {
        message = Shared.Utility.FunctionUtility.GetMessage("ERRO118");
        return false;
      }

      return true;
    }
    #endregion Process

    #region Event
    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);

      e.Layout.Bands[0].Columns["Errors"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Errors"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.NoEdit;
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.NoEdit;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValidFinishInfo(out message);
      if (!success)
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Container", DbType.Int64, containerPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateShippedQtyContainer", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 1)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }

      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.btnFinish.Enabled = false;
      this.btnFinishSpecial.Enabled = false;
    }

    private void ultDetail_DoubleClick(object sender, EventArgs e)
    {
      viewPLN_06_015 view = new viewPLN_06_015();
      view.flagWindow = 1;
      Shared.Utility.WindowUtinity.ShowView(view, "CONTAINER LIST", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);

      this.Search();
    }

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultDetail.Rows.ExpandAll(true);
      }
      else
      {
        ultDetail.Rows.CollapseAll(true);
      }
    }

    private void btnFinishSpecial_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Container", DbType.Int64, containerPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateShippedQtyContainer", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 1)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }

      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.btnFinishSpecial.Enabled = false;
      this.btnFinish.Enabled = false;
    }
    #endregion Event
  }
}
