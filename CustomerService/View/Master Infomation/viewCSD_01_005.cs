/*
  Author      : Lậm Quang Hà
  Date        : 07/10/2010
  Decription  : Search Category
  Checked by    : Võ Hoa Lư
  Checked date  : 12/10/2010
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.UserControls;
using DaiCo.CustomerService.DataSetSource;
using VBReport;
using System.Diagnostics;


namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_005 : MainUserControl
  {
    #region Field
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_01_005_Load(object sender, EventArgs e)
    {
      this.LoadParentCategory();
      btnExport.Enabled = false;
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Load dropdownlist parent category
    /// </summary>
    private void LoadParentCategory()
    {
      string commandText = "SELECT Pid, Category FROM TblCSDCategory WHERE ParentPid IS NULL ORDER BY Category";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadMultiCombobox(multiCBParentCategory, dt, "Pid", "Category");
      multiCBParentCategory.ColumnWidths = "0, 150";
    }    
    #endregion Load Data

    #region Search
    /// <summary>
    /// Search Category infomation from Category name and Parent Category
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[3];
      string category = txtCategory.Text.Trim();
      string usCode = txtUSCode.Text.Trim();
      if (category.Length > 0)
      {
        param[0] = new DBParameter("@Category", DbType.AnsiString, 130, "%" + category + "%");
      }
      if (multiCBParentCategory.SelectedIndex > 0)
      {
        int parent = DBConvert.ParseInt(multiCBParentCategory.SelectedValue.ToString());
        param[1] = new DBParameter("@ParentPid", DbType.Int64, parent);
      }
      if (usCode.Length > 0)
      {
        param[2] = new DBParameter("@USCode", DbType.AnsiString, 8, "%" + usCode + "%");
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDCategory_Select", param);
      dsCSDListCategory dsListCategory = new dsCSDListCategory();
      dsListCategory.Tables["TblCSDMainCategory"].Merge(dsSource.Tables[0]);
      dsListCategory.Tables["TblCSDSubCategory"].Merge(dsSource.Tables[1]);
      ultraGridInfomation.DataSource = dsListCategory;
      btnExport.Enabled = (ultraGridInfomation.Rows.Count > 0) ? true : false;
    }
    #endregion Search

    #region Event
    /// <summary>
    /// Search Category infomation from Category name and Parent Category
    /// </summary>
    /// <param name="sender">button Search</param>
    /// <param name="e">Click</param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Delete physical list forwarders which is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {      
      if (ultraGridInfomation.Rows.Count > 0)
      {
        DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
        if (result == DialogResult.Yes)
        {
          IList deleteList = new ArrayList();
          for (int i = 0; i < ultraGridInfomation.Rows.Count; i++)
          {
            UltraGridRow row = ultraGridInfomation.Rows[i];
            int selected = DBConvert.ParseInt(row.Cells["Selected"].Value.ToString());
            //Nếu thằng cha bị delete thì không cần duyệt thằng con vì mặc định xóa cha là xóa con
            if (selected == 1)
            {
              long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
              deleteList.Add(pid);
            }
            else
            {
              for (int k = 0; k < row.ChildBands[0].Rows.Count; k++)              
              {
                UltraGridRow childRow = row.ChildBands[0].Rows[k];
                selected = DBConvert.ParseInt(childRow.Cells["Selected"].Value.ToString());
                if (selected == 1)
                {
                  long pid = DBConvert.ParseLong(childRow.Cells["Pid"].Value.ToString());
                  deleteList.Add(pid);
                }
              }
            }
          }

          if (deleteList.Count == 0)
          {
            WindowUtinity.ShowMessageWarning("WRN0012");
            return;
          }

          bool success = true;
          foreach (long pid in deleteList)
          {
            DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spCSDCategory_Delete", inputParam, outputParam);
            int pidResult = DBConvert.ParseInt(outputParam[0].Value.ToString());

            if (pidResult == -1)
            {
              success = false;
              WindowUtinity.ShowMessageError("ERR0048");
            }
            else if (pidResult == 0)
            {
              success = false;
              WindowUtinity.ShowMessageError("ERR0004");
            }
          }
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0002");
          }
          this.Search();
        }
      }
    }

    /// <summary>
    /// Init layout for ultragrid view category Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInfomation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridInfomation);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["HSCode"].Hidden = true;
      e.Layout.Bands[0].Columns["USCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["USCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["USCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["USCode"].Header.Caption = "Code";
      e.Layout.Bands[0].Columns["Category"].CellActivation = Activation.ActivateOnly;      
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["USCode"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["USCode"].MinWidth = 100;
      e.Layout.Bands[1].Columns["USCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Category"].CellActivation = Activation.ActivateOnly;  
      e.Layout.Bands[1].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[1].Columns["Selected"].MinWidth = 70;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Open screen update category(viewCSD_01_006)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInfomation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridInfomation.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultraGridInfomation.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      viewCSD_01_006 view = new viewCSD_01_006();
      view.categoryPid = pid;
      WindowUtinity.ShowView(view, "CATEGORY INFORMATION", false, ViewState.Window);
      this.Search();
    }

    /// <summary>
    /// Open screen register category(viewCSD_01_006)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_01_006 view = new viewCSD_01_006();
      WindowUtinity.ShowView(view, "CATEGORY INFORMATION", false, ViewState.Window);
      this.Search();
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }
    /// <summary>
    /// Export Category List To Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      // Init Excel File
      string strTemplateName = "CSDMasterInfoTemplate";
      string strSheetName = "Category";
      string strOutFileName = "Category List";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Get data
      DataSet dsExport = (DataSet)ultraGridInfomation.DataSource;
      DataTable dtExport = dsExport.Tables[0];
      if (dtExport != null && dtExport.Rows.Count > 0)
      {
        for (int i = 0; i < dtExport.Rows.Count; i++)
        {
          DataRow dtRow = dtExport.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("C7:D7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("C7:D7", 0, i).Paste();
          }

          oXlsReport.Cell("**USCode", 0, i).Value = dtRow["USCode"];
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["Category"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion Event
  }
}
