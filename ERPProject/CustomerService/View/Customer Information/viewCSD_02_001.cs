/*
  Author        : Lâm Quang Hà
  Create date   : 013/10/2010
  Decription    : Search and display Customer info from code, name, Distribute, ResponsiblePerson And Kind
  Checked by    : Võ Hoa Lư
  Checked date  : 
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
using DaiCo.ERPProject.CustomerService.DataSetSource;
using VBReport;
using System.Diagnostics;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_02_001 : MainUserControl
  {
    #region Init Data

    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_02_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_02_001_Load(object sender, EventArgs e)
    {
      //Load dropdownlist
      this.LoadDropDownList();
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Load dropdownlist
    /// </summary>
    private void LoadDropDownList()
    {
      //load dropdownlist Distribute
      this.LoadDistribute();

      //Load dropdownlist ResponsiblePerson
      this.LoadResponsiblePerson();

      //Load dropdownlist Kind
      this.LoadKind();
    }

    /// <summary>
    /// Load dropdownlist Distribute
    /// </summary>
    private void LoadDistribute()
    {
      string commandText = "SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE ParentPid IS NULL AND DeletedFlg = 0";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DaiCo.Shared.Utility.ControlUtility.LoadMultiCombobox(multiCBParentCustomer, dt, "Pid", "Customer");
      multiCBParentCustomer.ColumnWidths = "0, 150";
    }

    /// <summary>
    /// Load dropdownlist ResponsiblePerson
    /// </summary>
    private void LoadResponsiblePerson()
    {
      string commandText = "SELECT Pid, EmpName FROM VHRMEmployee WHERE Department = 'CSD'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DaiCo.Shared.Utility.ControlUtility.LoadMultiCombobox(multiCBResponsiblePerson, dt, "Pid", "EmpName");
      multiCBResponsiblePerson.ColumnWidths = "0, 150";
    }

    /// <summary>
    /// Load dropdownlist Kind
    /// </summary>
    private void LoadKind()
    {
      string commandText = "SELECT Code, [Description] FROM TblBOMCodeMaster WHERE [Group] = 2008";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DaiCo.Shared.Utility.ControlUtility.LoadMultiCombobox(multiCBKind, dt, "Code", "Description");
      multiCBKind.ColumnWidths = "0, 150";
    }
    #endregion Load Data

    #region Search
    /// <summary>
    /// Search  Customer infomation from code, name, Distribute, ResponsiblePerson And Kind condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[5];
      string code = txtCustomerCode.Text.Trim();
      if (code.Length > 0)
      {
        param[0] = new DBParameter("@CustomerCode", DbType.AnsiString, 10, "%" + code + "%");
      }
      string name = txtName.Text.Trim();
      if (name.Length > 0)
      {
        param[1] = new DBParameter("@Name", DbType.String, 130, "%" + name + "%");
      }
      if (multiCBParentCustomer.SelectedIndex > 0)
      {
        long parentPid = DBConvert.ParseLong(multiCBParentCustomer.SelectedValue.ToString());
        param[2] = new DBParameter("@ParentPid", DbType.Int64, parentPid);
      }
      if (multiCBResponsiblePerson.SelectedIndex > 0)
      {
        int responsiblePerson = DBConvert.ParseInt(multiCBResponsiblePerson.SelectedValue.ToString());
        param[3] = new DBParameter("@ResponsiblePerson", DbType.Int32, responsiblePerson);
      }
      if (multiCBKind.SelectedIndex > 0)
      {
        long kind = DBConvert.ParseLong(multiCBKind.SelectedValue.ToString());
        param[4] = new DBParameter("@Kind", DbType.Int64, kind);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCustomerInfo_Select", param);
      ultraGridInformation.DataSource = dtSource;
      if (dtSource != null)
      {
        int count = ultraGridInformation.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          int locked = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Locked"].Value.ToString());
          if (locked == 1)
          {
            ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.DarkGray;
            ultraGridInformation.Rows[i].Activation = Activation.ActivateOnly;
          }
        }
        btnExport.Enabled = (count > 0) ? true : false;
      }
    }
    #endregion Search

    #region Event
    /// <summary>
    /// Search  Customer infomation from code, name, Distribute, ResponsiblePerson And Kind
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
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Delete physical list Customers which is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (ultraGridInformation.Rows.Count > 0)
      {
        int countCheck = 0;
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Selected"].Value.ToString());
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
          bool deleteSuccess = true;
          for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
          {
            int selected = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Selected"].Value.ToString());
            if (selected == 1)
            {
              long pid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["Pid"].Value.ToString());
              DBParameter[] inputParam = new DBParameter[2];
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
              inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spCSDCustomerInfo_DeleteLogic", inputParam, outputParam);
              long success = DBConvert.ParseInt(outputParam[0].Value.ToString());
              if(success == 0)
              {
                deleteSuccess = false;                
              }
            }
          }
          if (deleteSuccess)
          {
            WindowUtinity.ShowMessageSuccess("MSG0002");
          }
          else
          {
            WindowUtinity.ShowMessageError("WRN0004");
          }
          this.Search();
        }
      }
    }

    /// <summary>
    /// Init layout for ultragrid view Customers Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultraGridInformation);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerCode"].Header.Caption = "Customer Code";
      e.Layout.Bands[0].Columns["CustomerCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CustomerCode"].MaxWidth = 85;
      e.Layout.Bands[0].Columns["CustomerCode"].MinValue = 85;
      e.Layout.Bands[0].Columns["Distribute"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleThrough"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["SaleThrough"].MaxWidth = 50;
      //e.Layout.Bands[0].Columns["SaleThrough"].MinValue = 50;
      e.Layout.Bands[0].Columns["SaleThrough"].Header.Caption = "Through";
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Kind"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Kind"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Kind"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["ResponsiblePerson"].Header.Caption = "Responsible Person";
      e.Layout.Bands[0].Columns["ResponsiblePerson"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Region"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Region"].Header.Caption = "Country";
      e.Layout.Bands[0].Columns["City"].Hidden = true;
      e.Layout.Bands[0].Columns["StreetAddress"].Hidden = true;
      //e.Layout.Bands[0].Columns["StreetAddress"].Header.Caption = "Street Address";
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["Selected"].Hidden = true;
      e.Layout.Bands[0].Columns["Locked"].Hidden = true;
      e.Layout.Bands[0].Columns["TradeTerm"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TradeTerm"].Header.Caption = "Trade Term";
      e.Layout.Bands[0].Columns["PaymentTerm"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PaymentTerm"].Header.Caption = "Payment Term";
      e.Layout.Bands[0].Columns["OpenDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["OpenDate"].Header.Caption = "Open Date";
      e.Layout.Bands[0].Columns["CloseDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CloseDate"].Header.Caption = "Close Date";
      e.Layout.Bands[0].Columns["ResponsibleSale"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ResponsibleSale"].Header.Caption = "Res.Sale";
      e.Layout.Bands[0].Columns["ResponsiblePerson"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ResponsiblePerson"].Header.Caption = "Res.CS";
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_02_002 view = new viewCSD_02_002();
      WindowUtinity.ShowView(view, "NEW CUSTOMER", false, ViewState.Window);
      this.Search();
    }

    private void ultraGridInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridInformation.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultraGridInformation.Selected.Rows[0];
      int locked = DBConvert.ParseInt(row.Cells["Locked"].Value.ToString());
      if (locked == 1) {
        return;
      }      
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      viewCSD_02_002 view = new viewCSD_02_002();
      view.customerPid = pid;
      WindowUtinity.ShowView(view, "CUSTOMER INFORMATION", false, ViewState.MainWindow);
      this.Search();
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (ultraGridInformation.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        ultraGridInformation.Rows.Band.Columns["Selected"].Hidden = true;
        Utility.ExportToExcelWithDefaultPath(ultraGridInformation, out xlBook, "CUSTOMER LIST", 5);
        ultraGridInformation.Rows.Band.Columns["Selected"].Hidden = false;

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "CUSTOMER LIST";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlSheet.Cells[5, 1] = "Note: ";
        //r.Font.Bold = true;

        //xlSheet.Cells[5, 2] = "On Time";

        //xlSheet.Cells[5, 3] = "Late";
        //r = xlSheet.get_Range("C5", "C5");
        //r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0));

        //xlSheet.Cells[5, 4] = "Early ";
        //r = xlSheet.get_Range("D5", "D5");
        //r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(144, 238, 144));

        xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }

      //// Init Excel File
      //string strTemplateName = "CSDCustomerList";
      //string strSheetName = "Template";
      //string strOutFileName = "Customer List";
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //// Get data
      //DataTable dtExport = (DataTable)ultraGridInformation.DataSource;

      //if (dtExport != null && dtExport.Rows.Count > 0)
      //{
      //  for (int i = 0; i < dtExport.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dtExport.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("B6:O6").Copy();
      //      oXlsReport.RowInsert(5 + i);
      //      oXlsReport.Cell("B6:O6", 0, i).Paste();
      //    }

      //    oXlsReport.Cell("**No", 0, i).Value = i + 1;
      //    oXlsReport.Cell("**CusCode", 0, i).Value = dtRow["CustomerCode"];
      //    oXlsReport.Cell("**CusName", 0, i).Value = dtRow["Name"];
      //    oXlsReport.Cell("**Distribute", 0, i).Value = dtRow["Distribute"];
      //    oXlsReport.Cell("**Through", 0, i).Value = dtRow["SaleThrough"];
      //    oXlsReport.Cell("**Kind", 0, i).Value = dtRow["Kind"];
      //    oXlsReport.Cell("**Country", 0, i).Value = dtRow["Region"];
      //    oXlsReport.Cell("**TradeTerm", 0, i).Value = dtRow["TradeTerm"];
      //    oXlsReport.Cell("**PaymentTerm", 0, i).Value = dtRow["PaymentTerm"];
      //    oXlsReport.Cell("**OpenDate", 0, i).Value = dtRow["OpenDate"];
      //    oXlsReport.Cell("**CloseDate", 0, i).Value = dtRow["CloseDate"];
      //    oXlsReport.Cell("**Status", 0, i).Value = dtRow["Status"];
      //    oXlsReport.Cell("**ResSale", 0, i).Value = dtRow["ResponsibleSale"];
      //    oXlsReport.Cell("**ResCS", 0, i).Value = dtRow["ResponsiblePerson"];
      //  }
      //}
      //oXlsReport.Out.File(strOutFileName);
      //Process.Start(strOutFileName);
    }
    #endregion Event
  }
}
