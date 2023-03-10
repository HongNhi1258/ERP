/*
  Author      : Lậm Quang Hà
  Date        : 11/10/2010
  Decription  : Tìm kiếm thông tin Room
  Checked by    : Võ Hoa Lư
  Checked date  : 13/10/2010
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared.UserControls;
using VBReport;
using System.Diagnostics;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_007 : MainUserControl
  {
    #region Init Data

    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_007()
    {
      InitializeComponent();
    }
    #endregion Init Data

    #region Search
    /// <summary>
    /// Search Room infomation from name condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[1];
      string room = txtRoom.Text.Trim();
      if (room.Length > 0)
      {
        param[0] = new DBParameter("@Room", DbType.AnsiString, 130, "%" + room + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDRoom_Select", param);
      ultraGridInfomation.DataSource = dtSource;
      btnExport.Enabled = (ultraGridInfomation.Rows.Count > 0) ? true : false;
    }
    #endregion Search

    #region Event

    /// <summary>
    /// Search Room infomation from name condition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
    /// Init layout for ultragrid view Room Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInfomation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridInfomation);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Room"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].MaxWidth = 250;
      e.Layout.Bands[0].Columns["CreateBy"].MinWidth = 250;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Delete Physical/logic list Rooms which is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (ultraGridInfomation.Rows.Count > 0)
      {
        int countCheck = 0;
        for (int i = 0; i < ultraGridInfomation.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultraGridInfomation.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            countCheck++;
          }
        }
        if (countCheck == 0)
        {
          WindowUtinity.ShowMessageWarning("WRN0012");
          return;
        }
        DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
        if (result == DialogResult.Yes)
        {
          for (int i = 0; i < ultraGridInfomation.Rows.Count; i++)
          {
            int selected = DBConvert.ParseInt(ultraGridInfomation.Rows[i].Cells["Selected"].Value.ToString());
            if (selected == 1)
            {
              long pid = DBConvert.ParseLong(ultraGridInfomation.Rows[i].Cells["Pid"].Value.ToString());
              DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
              DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spCSDRoom_Delete", inputParam, outputParam);
              long success = DBConvert.ParseInt(outputParam[0].Value.ToString());
              if (success == 1)
              {
                WindowUtinity.ShowMessageSuccess("MSG0002");
              }
              else
              {
                if (success == 0)
                {
                  WindowUtinity.ShowMessageError("ERR0004");
                }
                else if (success == -1)
                {
                  string room = ultraGridInfomation.Rows[i].Cells["Room"].Value.ToString().Trim();
                  WindowUtinity.ShowMessageError("ERR0049", room);
                }
              }
            }
          }
          this.Search();
        }
      }
    }

    /// <summary>
    /// Open screen update Room, RoomLanguage(viewCSD_01_008)
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
      viewCSD_01_008 view = new viewCSD_01_008();
      view.roomPid = pid;
      WindowUtinity.ShowView(view, "ROOM INFORMATION", false, ViewState.Window);
      this.Search();
    }

    /// <summary>
    /// Open screen new Room, RoomLanguage(viewCSD_01_008)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_01_008 view = new viewCSD_01_008();
      WindowUtinity.ShowView(view,  "ROOM INFORMATION", false, ViewState.Window);
      this.Search();
    }

    /// <summary>
    /// Export Room List To Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      // Init Excel File
      string strTemplateName = "CSDMasterInfoTemplate";
      string strSheetName = "Room";
      string strOutFileName = "Room List";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Get data
      DataTable dtExport = (DataTable)ultraGridInfomation.DataSource;
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

          oXlsReport.Cell("**Room", 0, i).Value = dtRow["Room"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion Event
  }
}
