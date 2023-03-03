/*
 * Created by:    Nguyen Van Tron
 * Created date:  29/01/2011
 * Description:   Danh sach du doan vat tu cua planning
 */

using DaiCo.Application;
using DaiCo.Planning.DataSetFile;
using DaiCo.Planning.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_001 : MainUserControl
  {
    #region Function
    /// <summary>
    /// Load data for combobox employee
    /// </summary>
    private void LoadData()
    {
      ControlUtility.LoadComboBoxEmployeeByDept(cmbCreateBy, ConstantClass.PLANNING_DEPT);
    }

    #endregion Function

    #region event
    public viewPLN_07_001()
    {
      InitializeComponent();
      ultraDateTimeFromDate.FormatString = ConstantClass.FORMAT_DATETIME;
      ultraDateTimeToDate.FormatString = ConstantClass.FORMAT_DATETIME;
      ultraDateTimeFromDate.Value = null;
      ultraDateTimeToDate.Value = null;
    }

    private void viewPLN_07_001_Load(object sender, EventArgs e)
    {
      this.LoadListViewMaterialGroup();
      this.LoadData();
    }

    private void LoadListViewMaterialGroup()
    {
      // Load ListView Material Group
      string commandText = "Select [Group] [Material Group], Description From VBOMMaterialGroup";
      DataTable dtMaterialGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      // Add columns to ListView
      listViewSource.DataSource = dtMaterialGroup;
      listViewSource.ColumnWidths = "110; 170";
      listViewSource.DataBind();
    }
    /// <summary>
    /// Search list forecast material
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[4];
      if (txtTittle.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@Tittle", DbType.String, 256, "%" + txtTittle.Text.Trim() + "%");
      }
      if (cmbCreateBy.SelectedIndex > 0)
      {
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, cmbCreateBy.SelectedValue);
      }
      if (ultraDateTimeFromDate.Value != null)
      {
        inputParam[2] = new DBParameter("@FromCreateDate", DbType.DateTime, ultraDateTimeFromDate.Value);
      }
      if (ultraDateTimeToDate.Value != null)
      {
        inputParam[3] = new DBParameter("@ToCreateDate", DbType.DateTime, ((DateTime)ultraDateTimeToDate.Value).AddDays(1));
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spListPLNForecastMaterial", inputParam);
      ultraGridForcastMaterial.DataSource = dtSource;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultraGridForcastMaterial_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridForcastMaterial);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Tittle"].Header.Caption = "Title";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateBy"].MinWidth = 150;
      e.Layout.Bands[0].Columns["CreateBy"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateDate"].MinWidth = 120;
      e.Layout.Bands[0].Columns["CreateDate"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Selected"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_07_002 view = new viewPLN_07_002();
      WindowUtinity.ShowView(view, "New Forecast Material", true, ViewState.Window);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridForcastMaterial_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultraGridForcastMaterial.Selected.Rows.Count > 0)
      {
        long pid = DBConvert.ParseLong(ultraGridForcastMaterial.Selected.Rows[0].Cells["Pid"].Value.ToString());
        viewPLN_07_002 view = new viewPLN_07_002();
        view.pid = pid;
        WindowUtinity.ShowView(view, "Forecast Material Detail", true, ViewState.Window, FormWindowState.Maximized);
        this.Search();
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      bool success = true;
      for (int i = 0; i < ultraGridForcastMaterial.Rows.Count; i++)
      {
        int selected = DBConvert.ParseInt(ultraGridForcastMaterial.Rows[i].Cells["Selected"].Value.ToString());
        if (selected == 1)
        {
          long pid = DBConvert.ParseLong(ultraGridForcastMaterial.Rows[i].Cells["Pid"].Value.ToString());
          DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNForecastMaterial_Delete", inputParam, outputParam);
          if (inputParam[0].Value.ToString() == "0")
          {
            success = false;
          }
        }
      }
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0004");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      this.Search();
    }

    /// <summary>
    /// Print Report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrintReport_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultraGridForcastMaterial.DataSource;
      DataRow[] rows = dtSource.Select("Selected = 1");
      if (rows.Length > 0)
      {
        DataSet dsSource = new dsPLNForecastMaterial();
        foreach (DataRow row in rows)
        {
          long forecastPid = DBConvert.ParseLong(row["Pid"].ToString());
          string title = row["Tittle"].ToString().Trim();
          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@ForcastPid", DbType.Int64, forecastPid);
          // List group materials
          string itemGroup = listViewSource.SelectedValue.Replace(',', ';');
          input[1] = new DBParameter("@GroupMaterials", DbType.String, 4000, itemGroup);
          DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNForecastMaterial_Select", input);
          DataRow newRow = dsSource.Tables["dtForecastInfo"].NewRow();
          newRow["Pid"] = forecastPid;
          newRow["Title"] = title;
          dsSource.Tables["dtForecastInfo"].Rows.Add(newRow);
          dsSource.Tables["dtForecastMaterial"].Merge(dtData);
        }
        DaiCo.Shared.View_Report report = null;
        cptPLNForecastMaterial cpt = new cptPLNForecastMaterial();
        cpt.SetDataSource(dsSource);

        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = true;
        report.ShowReport(Shared.Utility.ViewState.ModalWindow);
      }
      else
      {
        WindowUtinity.ShowMessageWarning("MSG0011", "Forecast Material");
        return;
      }
    }

    /// <summary>
    /// Show/Hide Group Box Material Group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkGroup_CheckedChanged(object sender, EventArgs e)
    {
      if (chkGroup.Checked)
      {
        grpGroupMaterial.Visible = true;
      }
      else
      {
        grpGroupMaterial.Visible = false;
      }
    }
    #endregion event
  }
}
