/*
  Author      : Nguyen Van Tron
  Date        : 08/08/2022
  Description : Customs Declaration list
  Standard Form: view_SearchInfo.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_16_001 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCCustomsDeclarationList_Init");
      Utility.LoadUltraCombo(ucbCreateBy, dsInit.Tables[0], "EmployeePid", "Employee", false, "Employee");
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "Pid", "Object", false, new string[] { "Pid", "Object", "ObjectType" });
      Utility.LoadUltraCombo(ucbStatus, dsInit.Tables[2], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbActionDoc, dsInit.Tables[3], "Value", "Display", false, "Value");
      ucbActionDoc.Value = 1;
      udtFromDate.Value = null;
      udtToDate.Value = null;      
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 7;
      string storeName = "spACCCustomsDeclarationList_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtDeclarationCode.Text.ToString().Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@DeclarationCode", DbType.String, txtDeclarationCode.Text.ToString());
      }
      if (DBConvert.ParseInt(ucbCreateBy.Value) > 0)
      {
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ucbCreateBy.Value));
      }
      if (udtFromDate.Value != null)
      {
        inputParam[2] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(udtFromDate.Value));
      }
      if (udtToDate.Value != null)
      {
        inputParam[3] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(udtToDate.Value));
      }
      if (DBConvert.ParseInt(ucbObject.Value) > 0)
      {
        inputParam[4] = new DBParameter("@Object", DbType.Int32, DBConvert.ParseInt(ucbObject.Value));
        inputParam[5] = new DBParameter("@ObjectType", DbType.Int32, DBConvert.ParseInt(ucbStatus.SelectedRow.Cells["ObjectType"].Value));

      }
      if (DBConvert.ParseInt(ucbStatus.Value) >= 0)
      {
        inputParam[6] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ucbStatus.Value));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        int status = DBConvert.ParseInt(ugdInformation.Rows[i].Cells["Status"].Value);
        switch (status)
        {
          case 1:
            ugdInformation.Rows[i].Appearance.BackColor = Color.LightGray;
            break;
          case 2:
            ugdInformation.Rows[i].Appearance.BackColor = Color.LightCyan;
            break;
          default:
            break;
        }        
      }
      lbCount.Text = string.Format(String.Format("Đếm: {0}", ugdInformation.Rows.FilteredInRowCount > 0 ? ugdInformation.Rows.FilteredInRowCount : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtDeclarationCode.Text = string.Empty;
      ucbCreateBy.Value = null;
      udtFromDate.Value = null;
      udtToDate.Value = null;
      ucbObject.Value = null;
      ucbStatus.Value = null;
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

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }

    private void SetLanguage()
    {
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewACC_16_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_16_001_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegSearch);      
      //Init Data
      this.InitData();
      // Set Language
      this.SetLanguage();
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
      this.CloseTab();
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

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdInformation);      
      e.Layout.Override.RowSelectorWidth = 32;
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

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;

      e.Layout.Bands[0].Columns["DeclarationCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["DeclarationDate"].Header.Caption = "Ngày chứng từ";
      e.Layout.Bands[0].Columns["Object"].Header.Caption = "Đối tượng";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Tiền tệ";      
      e.Layout.Bands[0].Columns["TotalTaxAmount"].Header.Caption = "Tổng tiền thuế";
      e.Layout.Bands[0].Columns["DeclarationDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Người tạo";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Ngày tạo";
      e.Layout.Bands[0].Columns["StatusRemark"].Header.Caption = "Tình trạng";
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdInformation);
      }
    }

    private void ugdInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
        }
      }
    }


    private void btnNew_Click(object sender, EventArgs e)
    {
      if (ucbActionDoc.SelectedRow != null)
      {
        viewACC_16_002 view = new viewACC_16_002();
        view.actionCode = DBConvert.ParseInt(ucbActionDoc.Value);
        view.actionName = ucbActionDoc.Text;
        Shared.Utility.WindowUtinity.ShowView(view, "Tạo mới tờ khai", false, ViewState.MainWindow);
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText("Vui lòng chọn loại tờ khai.");
      }
    }


    private void ugdInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      viewACC_16_002 view = new viewACC_16_002();
      view.viewPid = DBConvert.ParseLong(ugdInformation.ActiveRow.Cells["Pid"].Value);
      Shared.Utility.WindowUtinity.ShowView(view, "CT Tờ Khai", false, ViewState.MainWindow);
    }
    #endregion event
  }
}
