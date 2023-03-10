/*
  Author      : Lậm Quang Hà
  Date        : 07/10/2010
  Decription  : Search collection
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
using System.Diagnostics;
using VBReport;


namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_009 : MainUserControl
  {
    #region Field
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_01_009_Load(object sender, EventArgs e)
    {
      btnExport.Enabled = false;
    }
    #endregion Init Data

    #region Load Data
   
    #endregion Load Data

    #region Search
    /// <summary>
    /// Search collection infomation from collection name
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[2];
      string collection = txtCollection.Text.Trim();
      string usCode = txtUSCode.Text.Trim();
      if (collection.Length > 0)
      {
        param[0] = new DBParameter("@Collection", DbType.AnsiString, 130, "%" + collection + "%");
      }
      if (usCode.Length > 0)
      {
        param[1] = new DBParameter("@USCode", DbType.AnsiString, 8, "%" + usCode + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCollection_Select", param);
      ultraGridInfomation.DataSource = dtSource;
      btnExport.Enabled = (ultraGridInfomation.Rows.Count > 0) ? true : false;
    }
    #endregion Search

    #region Event
    /// <summary>
    /// Search collection infomation from collection name
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
            DataBaseAccess.ExecuteStoreProcedure("spCSDCollection_Delete", inputParam, outputParam);
            int pidResult = DBConvert.ParseInt(outputParam[0].Value.ToString());

            if (pidResult == -1)
            {
              success = false;
              WindowUtinity.ShowMessageError("ERR0054", "the collection");
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
    /// Init layout for ultragrid view cellection Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInfomation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridInfomation);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["USCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["USCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["USCode"].Header.Caption = "Code";
      e.Layout.Bands[0].Columns["USCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Collection"].CellActivation = Activation.ActivateOnly;      
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;            
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Open screen update collection(viewCSD_01_006)
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
      viewCSD_01_010 view = new viewCSD_01_010();
      view.collectionPid = pid;
      WindowUtinity.ShowView(view, "Collection Information", false, ViewState.Window);
      this.Search();
    }

    /// <summary>
    /// Open screen register collection(viewCSD_01_006)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_01_010 view = new viewCSD_01_010();
      WindowUtinity.ShowView(view, "Collection Information", false, ViewState.Window);
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
    /// Export Collection List To Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      //// Init Excel File
      //string strTemplateName = "CSDMasterInfoTemplate";
      //string strSheetName = "Collection";
      //string strOutFileName = "Collection List";
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //// Get data
      //DataTable dtExport = (DataTable)ultraGridInfomation.DataSource;
      //if (dtExport != null && dtExport.Rows.Count > 0)
      //{
      //  for (int i = 0; i < dtExport.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dtExport.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("C7:D7").Copy();
      //      oXlsReport.RowInsert(6 + i);
      //      oXlsReport.Cell("C7:D7", 0, i).Paste();
      //    }

      //    oXlsReport.Cell("**USCode", 0, i).Value = dtRow["USCode"];
      //    oXlsReport.Cell("**Name", 0, i).Value = dtRow["Collection"];
      //  }
      //}
      //oXlsReport.Out.File(strOutFileName);
      //Process.Start(strOutFileName);
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInfomation, "Collection List");
    }
    #endregion Event
  }
}
