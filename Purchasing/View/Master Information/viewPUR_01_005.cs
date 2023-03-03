/*
  Author      : Vo Van Duy Qui
  Date        : 26-03-2011
  Description : List Material Information
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_01_005 : MainUserControl
  {
    #region Field
    private bool loadData = false;
    public string materialGroup = string.Empty;
    public string materialCategory = string.Empty;

    private string SP_GNR_MATERIALINFO_SELECT = "spGNRMaterialInformation_Select";
    private string SP_GNR_MATERIALINFO_CONFIRM = "spGNRMaterialInformation_Confirm";
    private string SP_GNR_MATERIALINFO_DELETE = "spGNRMaterialInformation_Delete";
    #endregion Field

    #region Init

    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_01_005()
    {
      InitializeComponent();
      ultData.MouseDoubleClick += new MouseEventHandler(ultData_MouseDoubleClick);
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_005_Load(object sender, EventArgs e)
    {
      txtMaterialGroup.Text = this.materialGroup;
      txtMaterialCategory.Text = this.materialCategory;
      this.LoadData(this.materialGroup, this.materialCategory);

      this.loadData = true;
    }

    /// <summary>
    /// Load List Material 
    /// </summary>
    /// <param name="group"></param>
    /// <param name="category"></param>
    private void LoadData(string group, string category)
    {
      DBParameter[] input = new DBParameter[2];
      if (materialGroup.Length > 0)
      {
        input[0] = new DBParameter("@MaterialGroup", DbType.AnsiString, 3, group);
      }
      if (materialCategory.Length > 0)
      {
        input[1] = new DBParameter("@MaterialCategory", DbType.AnsiString, 2, category);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(SP_GNR_MATERIALINFO_SELECT, input);
      ultData.DataSource = dtSource;
    }
    #endregion Init

    #region Event
    /// <summary>
    /// btnNew Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_01_003 uc = new viewPUR_01_003();
      uc.materialGroup = this.materialGroup;
      uc.materialCategory = this.materialCategory;
      Shared.Utility.WindowUtinity.ShowView(uc, "Material Information", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
      this.LoadData(this.materialGroup, this.materialCategory);
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// btnConfirm Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConfirm_Click(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        DataRow[] rowsConfirm = dtSource.Select("Selected = 1");
        if (rowsConfirm.Length > 0)
        {
          foreach (DataRow rowcheck in rowsConfirm)
          {
            int status = DBConvert.ParseInt(rowcheck["MStatus"].ToString().Trim());
            if (status >= 1)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0079", "confirm");
              return;
            }
          }

          DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "Confirm");
          if (dlgr == DialogResult.Yes)
          {
            foreach (DataRow row in rowsConfirm)
            {
              string materialCode = row["MaterialCode"].ToString().Trim();
              int status = DBConvert.ParseInt(row["MStatus"].ToString().Trim());

              DBParameter[] input = new DBParameter[2];
              input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
              input[1] = new DBParameter("@Status", DbType.Int32, 1);
              DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

              DataBaseAccess.ExecuteStoreProcedure(SP_GNR_MATERIALINFO_CONFIRM, input, output);
              int result = DBConvert.ParseInt(output[0].Value.ToString());
              if (result == 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0034");
                return;
              }
            }
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0012");
          }
          else if (dlgr == DialogResult.No)
          {
            return;
          }
        }
        this.LoadData(this.materialGroup, this.materialCategory);
      }
    }

    /// <summary>
    /// btnDelete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        DataRow[] rowsDelete = dtSource.Select("Selected = 1");
        if (rowsDelete.Length > 0)
        {
          foreach (DataRow rowcheck in rowsDelete)
          {
            int status = DBConvert.ParseInt(rowcheck["MStatus"].ToString().Trim());
            if (status >= 2)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0079", "delete");
              return;
            }
          }

          DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "delete");
          if (dlgr == DialogResult.Yes)
          {
            foreach (DataRow row in rowsDelete)
            {
              string materialCode = row["MaterialCode"].ToString().Trim();
              int status = DBConvert.ParseInt(row["MStatus"].ToString().Trim());

              DBParameter[] input = new DBParameter[] { new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode) };
              DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

              DataBaseAccess.ExecuteStoreProcedure(SP_GNR_MATERIALINFO_DELETE, input, output);
              int result = DBConvert.ParseInt(output[0].Value.ToString());
              if (result == 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
                return;
              }
            }
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0002");
          }
          else if (dlgr == DialogResult.No)
          {
            return;
          }
        }
        this.LoadData(this.materialGroup, this.materialCategory);
      }
    }

    /// <summary>
    /// btnDisactive Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDisactive_Click(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        DataRow[] rowsDisactive = dtSource.Select("Selected = 1");
        if (rowsDisactive.Length > 0)
        {
          foreach (DataRow rowcheck in rowsDisactive)
          {
            int status = DBConvert.ParseInt(rowcheck["MStatus"].ToString().Trim());
            if (status != 2)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0079", "disactive");
              return;
            }
          }
          DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "Disactive");
          if (dlgr == DialogResult.Yes)
          {
            foreach (DataRow row in rowsDisactive)
            {
              string materialCode = row["MaterialCode"].ToString().Trim();

              DBParameter[] input = new DBParameter[2];
              input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
              input[1] = new DBParameter("@Status", DbType.Int32, 3);
              DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

              DataBaseAccess.ExecuteStoreProcedure(SP_GNR_MATERIALINFO_CONFIRM, input, output);
              int result = DBConvert.ParseInt(output[0].Value.ToString());
              if (result == 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0034");
                return;
              }
            }
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0012");
          }
          else if (dlgr == DialogResult.No)
          {
            return;
          }
        }
        this.LoadData(this.materialGroup, this.materialCategory);
      }
    }

    /// <summary>
    /// ultData Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0)
      {
        viewPUR_01_003 uc = new viewPUR_01_003();
        uc.materialCode = ultData.Selected.Rows[0].Cells["MaterialCode"].Value.ToString().Trim();
        Shared.Utility.WindowUtinity.ShowView(uc, "Material Information", false, DaiCo.Shared.Utility.ViewState.MainWindow);
        this.LoadData(this.materialGroup, this.materialCategory);
      }
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Min-Max"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group In Charge";
      e.Layout.Bands[0].Columns["DeparmentName"].Header.Caption = "Deparment In Charge";
      e.Layout.Bands[0].Columns["MStatus"].Hidden = true;
      e.Layout.Bands[0].Columns["Group"].Hidden = true;
      e.Layout.Bands[0].Columns["Category"].Hidden = true;

      e.Layout.Bands[0].Columns["Group"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Group"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Min-Max"].MinWidth = 65;
      e.Layout.Bands[0].Columns["Min-Max"].MaxWidth = 65;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 170;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 170;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 65;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 65;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["Category"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Category"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["GroupName"].MinWidth = 160;
      e.Layout.Bands[0].Columns["GroupName"].MaxWidth = 160;
      e.Layout.Bands[0].Columns["DeparmentName"].MinWidth = 160;
      e.Layout.Bands[0].Columns["DeparmentName"].MaxWidth = 160;

      for (int i = 0; i < ultData.DisplayLayout.Bands[0].Columns.Count - 1; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }
    #endregion Event
  }
}
