using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.Shared.Utility
{
  public class ControlUtility
  {
    private static List<Control> _pagesIndexed;
    /// <summary>
    /// This method is used to Permision.
    /// (Hàm này dùng d? phân quy?n, hi?n th? các comp user có quy?n truy c?p)
    /// </summary>
    public static void UserAccessRight(int userPid, Control control)
    {
      #region InVisible Control
      string strCmdCtr = "Select ControlName From TblGNRDefineUIControl ";
      strCmdCtr += string.Format("Where UICode = '{0}'", control.Name);
      DataTable dtCtr = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(strCmdCtr);

      //search permision of group
      string commandText = "SELECT	COUNT(*) FROM	TblGNRAccessGroup G " +
                           " INNER JOIN TblGNRAccessGroupUser GU ON GU.GroupPid = G.Pid " +
                           " INNER JOIN TblBOMUser U ON U.Pid = GU.UserPid AND U.EmployeePid =" + SharedObject.UserInfo.UserPid +
                           " INNER JOIN TblBOMCodeMaster C ON C.Code = G.[Role] AND C.Code = 1 AND C.[Group] =" + DaiCo.Shared.Utility.ConstantClass.GROUP_ROLE;
      int count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText).ToString());

      for (int i = 0; i < dtCtr.Rows.Count; i++)
      {
        if (SharedObject.UserInfo.UserPid != ConstantClass.UserAddmin && count == 0)
          AccessControl(dtCtr.Rows[i]["ControlName"].ToString(), control, false);
      }
      #endregion InVisible Control

      #region Visible Control
      string storeName = "spGNRGroupUIControl_Access";

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@EmpPid", DbType.Int32, userPid);
      inputParam[1] = new DBParameter("@UICode", DbType.AnsiString, 128, control.Name);

      DataTable dtCompUser = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      if (dtCompUser != null)
      {
        for (int i = 0; i < dtCompUser.Rows.Count; i++)
        {
          AccessControl(dtCompUser.Rows[i]["ControlName"].ToString(), control, true);
        }
      }
      #endregion Visible Control


      #region RemoveTabPage
      DataTable dtControlNotSetRight = new DataTable();
      dtControlNotSetRight.Columns.Add("ControlName");
      foreach (DataRow row in dtCtr.Rows)
      {
        if (dtCompUser.Select(string.Format("ControlName = '{0}'", row["ControlName"])).Length == 0)
        {
          DataRow rowNotSetRight = dtControlNotSetRight.NewRow();
          rowNotSetRight["ControlName"] = row["ControlName"];
          dtControlNotSetRight.Rows.Add(rowNotSetRight);
        }
      }
      _pagesIndexed = new List<Control>();
      GetAllControl(control);

      for (int i = 0; i < _pagesIndexed.Count; i++)
      {
        if (_pagesIndexed[i].GetType() == typeof(TabPage))
        {
          string tabName = _pagesIndexed[i].Name;
          if (dtControlNotSetRight.Select(string.Format("ControlName = '{0}'", tabName)).Length > 0)
          {
            ((TabControl)((TabPage)_pagesIndexed[i]).Parent).TabPages.RemoveByKey(tabName);
          }
        }
      }
      #endregion RemoveTabPage
    }

    public static void GetAllControl(Control viewName)
    {

      foreach (Control ctr in viewName.Controls)
      {
        _pagesIndexed.Add(ctr);
        if (ctr.Controls.Count > 0)
        {
          GetAllControl(ctr);
        }
      }
    }

    /// <summary>
    /// Recursive to Visible or Invisible a control and some chile its controls
    /// </summary>
    /// <param name="strControlName"></param>
    /// <param name="control"></param>
    /// <param name="bAccess"></param>
    public static void AccessControl(string strControlName, Control control, bool bAccess)
    {
      foreach (Control ctr in control.Controls)
      {
        if (string.Compare(strControlName, ctr.Name, true) == 0)
        {
          ctr.Visible = bAccess;
        }
        else
        {
          if (ctr.Controls.Count > 0)
          {
            AccessControl(strControlName, ctr, bAccess);
          }
        }
      }
    }

    public static void LoadCombobox(ComboBox cmb, DataTable dtSoure, string columnValue, string columnText)
    {
      if (dtSoure != null)
      {
        DataTable dt = dtSoure.Clone();
        dt.Merge(dtSoure);
        DataRow row = dt.NewRow();
        dt.Rows.InsertAt(row, 0);
        cmb.DataSource = dt;
        cmb.DisplayMember = columnText;
        cmb.ValueMember = columnValue;
      }
    }

    public static void LoadMultiCombobox(UserControls.MultiColumnComboBox multiCB, DataTable dtSoure, string columnValue, string columnText)
    {
      if (dtSoure != null)
      {
        DataTable dt = dtSoure.Clone();
        dt.Merge(dtSoure);
        DataRow row = dt.NewRow();
        dt.Rows.InsertAt(row, 0);
        multiCB.DataSource = dt;
        multiCB.DisplayMember = columnText;
        multiCB.ValueMember = columnValue;
        multiCB.ColumnWidths = "100, 200";
      }
    }

    public static void LoadComboboxUnit(ComboBox cmbUnit)
    {
      string commandText = string.Format("SELECT UnitPid FROM TblBOMUnit ORDER BY UnitPid");
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbUnit, dtSource, "UnitPid", "UnitPid");
    }

    public static void LoadComboboxCodeMst(ComboBox cmbCodeMst, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbCodeMst, dtSource, "Code", "Value");
    }
    /// <summary>
    /// Tien add
    /// Load combo KnockDown
    /// </summary>
    /// <param name="cmbCodeMst"></param>
    /// <param name="group"></param>
    public static void LoadComboboxKnockDown(ComboBox cmbCodeMst, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbCodeMst, dtSource, "Code", "Value");
    }

    public static void LoadUltraComboCodeMst(UltraCombo ultCombo, int group, params int[] kind)
    {
      string commandText = string.Format(@"SELECT Code, Value, Description
                                           FROM TblBOMCodeMaster 
                                           WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      if (kind.Length > 0)
      {
        commandText = string.Format(commandText + " AND (Kind = {0})", kind[0]);
      }
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCombo.DataSource = dtSource;
      ultCombo.ValueMember = "Code";
      ultCombo.DisplayMember = "Value";
      ultCombo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCombo.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCombo.DisplayLayout.Bands[0].Columns["Value"].Width = 100;
      ultCombo.DisplayLayout.Bands[0].Columns["Description"].Width = 250;
    }
    /// <summary>
    /// Tien add
    /// Load Combo box Main Material
    /// </summary>
    /// <param name="ultCombo"></param>
    /// <param name="group"></param>
    /// <param name="kind"></param>
    public static void LoadUltraMainMaterial(UltraCombo ultCombo, int group, params int[] kind)
    {
      string commandText = string.Format(@"SELECT Code, Value, Description
                                           FROM TblBOMCodeMaster 
                                           WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      if (kind.Length > 0)
      {
        commandText = string.Format(commandText + " AND (Kind = {0})", kind[0]);
      }
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCombo.DataSource = dtSource;
      ultCombo.ValueMember = "Code";
      ultCombo.DisplayMember = "Value";
      ultCombo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCombo.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCombo.DisplayLayout.Bands[0].Columns["Value"].Width = 100;
      ultCombo.DisplayLayout.Bands[0].Columns["Description"].Width = 250;
    }
    public static void LoadComboboxCodeMstMaterialStype(ComboBox cmbCodeMst)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Value", Shared.Utility.ConstantClass.GROUP_MATERIALSTYPE);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbCodeMst, dtSource, "Code", "Value");
    }

    #region Load Data For ucUltraList
    public static void LoaducUltraListMaterialGroup(ucUltraList ucUltraListMaterialGroup)
    {
      // Load ListView Material Group
      string commandText = "Select [Group], Description From VBOMMaterialGroup Order By [Group] ASC";
      DataTable dtMaterialGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      // Add columns to ListView
      ucUltraListMaterialGroup.DataSource = dtMaterialGroup;
      ucUltraListMaterialGroup.ColumnWidths = "50; 150";
      ucUltraListMaterialGroup.DataBind();
      ucUltraListMaterialGroup.ValueMember = "Group";
    }

    /// <summary>
    /// Load Data for usercontrol ultralist
    /// </summary>
    /// <param name="uc"></param>
    /// <param name="dtSource"></param>
    /// <param name="colsWidth"></param>
    /// <param name="valueMember"> </param>
    public static void LoaducUltraList(ucUltraList uc, DataTable dtSource, string colsWidth, string valueMember, string displayMember)
    {
      if (dtSource != null)
      {
        // Add columns to ListView
        uc.DataSource = dtSource;
        uc.ValueMember = valueMember;
        uc.DisplayMember = displayMember;
        uc.ColumnWidths = colsWidth;
        uc.DataBind();
      }
    }
    #endregion Load Data For ucUltraList

    public static void LoadUltraDropdownCodeMst(UltraDropDown udrpDropDown, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value, Description FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Code";
      udrpDropDown.DisplayMember = "Value";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Value"].Width = 100;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Description"].Width = 250;
    }
    public static void LoadUltraDropdownCodeMstDefault(UltraDropDown udrpDropDown, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Code";
      udrpDropDown.DisplayMember = "Value";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Value"].Width = 100;
    }

    public static void LoadComboboxMasterName(ComboBox cmbMaster)
    {
      string commandText = string.Format(@"SELECT [Group], NameEn FROM TblBOMMasterName ORDER BY [Group]");
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbMaster, dtSource, "Group", "NameEn");
    }

    public static string GetSelectedValueCombobox(ComboBox cmb)
    {
      string value = string.Empty;
      try
      {
        value = cmb.SelectedValue.ToString();
      }
      catch { }
      return value;
    }

    public static void LoadCustomer(ComboBox cmb)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmb, dtSource, "Pid", "Customer");
    }

    public static void LoadCustomerVersion2(ComboBox cmb)
    {
      string commandText = string.Format(@"SELECT Pid, Customer FROM VPLNLoadCustomerToCompobox ORDER BY Customer");
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmb, dtSource, "Pid", "Customer");
    }
    /// <summary>
    /// Load all distribute Customer For UltraCombo
    /// </summary>
    /// <param name="cmb"></param>
    public static void LoadUltraCBDistributeCustomer(UltraCombo ultraCBCustomer)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode Code, Name , CustomerCode + ' | ' + Name Customer 
                                           FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBCustomer, dtSource, "Pid", "Customer", new string[] { "Pid", "Customer" });
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 100;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 100;
    }

    public static void LoadDirectCustomer(ComboBox cmb, long parentCustomerPid)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE ParentPid = {0}", parentCustomerPid);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmb, dtSource, "Pid", "Customer");
    }

    public static void LoadEmployee(ComboBox cmb, string deparment)
    {
      string whereClause = (deparment.Length > 0) ? string.Format(@"Where Department = '{0}'", deparment) : string.Empty;
      string commandText = string.Format(@" SELECT Pid, dbo.FSYSPadLeft(CAST(Pid as varchar), '0', 4) + ' - ' + EmpName EmpName FROM VHRMEmployee {0} ORDER BY Pid", whereClause);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmb, dtSource, "Pid", "EmpName");
    }

    /// <summary>
    /// Load dropdown language. 
    /// If english = true then the English language is included in DataSource else
    /// the English language is not included in DataSource
    /// </summary>
    /// <param name="cmb"></param>
    /// <param name="english"></param>
    public static void LoadLanguage(ComboBox cmbLanguage, bool english)
    {
      string commandText = "SELECT Pid, NameEN FROM VCSDOtherLanguage ORDER BY NameEN";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbLanguage, dtSource, "Pid", "NameEN");
    }

    /// <summary>
    /// Load dropdown language. 
    /// If english = true then the English language is included in DataSource else
    /// the English language is not included in DataSource
    /// </summary>
    /// <param name="cmb"></param>
    /// <param name="english"></param>
    public static void LoadLanguage(UltraDropDown ultraDropDownLanguage, bool english)
    {
      string commandText = "SELECT Pid, NameEN FROM VCSDOtherLanguage ORDER BY NameEN";
      ultraDropDownLanguage.DataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDropDownLanguage.ValueMember = "Pid";
      ultraDropDownLanguage.DisplayMember = "NameEN";
      ultraDropDownLanguage.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDropDownLanguage.DisplayLayout.Bands[0].Columns["NameEN"].Width = 150;
      ultraDropDownLanguage.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    #region LoadUltraCombo Data
    public static void LoadUltraCombo(UltraCombo ultraCB, DataTable dtSource, string columnValue, string columnText, params string[] columnHidden)
    {
      ultraCB.DataSource = dtSource;
      if (dtSource != null)
      {
        ultraCB.ValueMember = columnValue;
        ultraCB.DisplayMember = columnText;
        for (int i = 0; i < columnHidden.Length; i++)
        {
          ultraCB.DisplayLayout.Bands[0].Columns[columnHidden[i]].Hidden = true;
        }
      }
    }

    public static void LoadUltraCombo(UltraCombo ultraCB, DataTable dtSource, string columnValue, string columnText, bool colHeadersVisible, params string[] columnHidden)
    {
      ultraCB.DataSource = dtSource;
      if (dtSource != null)
      {
        ultraCB.ValueMember = columnValue;
        ultraCB.DisplayMember = columnText;
        for (int i = 0; i < columnHidden.Length; i++)
        {
          ultraCB.DisplayLayout.Bands[0].Columns[columnHidden[i]].Hidden = true;
        }
      }
      ultraCB.DisplayLayout.Bands[0].ColHeadersVisible = colHeadersVisible;
    }

    public static void LoadUltraDropDown(UltraDropDown ultraDD, DataTable dtSource, string columnValue, string columnText, params string[] columnHidden)
    {
      ultraDD.DataSource = dtSource;
      if (dtSource != null)
      {
        ultraDD.ValueMember = columnValue;
        ultraDD.DisplayMember = columnText;
        for (int i = 0; i < columnHidden.Length; i++)
        {
          ultraDD.DisplayLayout.Bands[0].Columns[columnHidden[i]].Hidden = true;
        }
      }
    }

    /// <summary>
    /// Load Data To UltraCombo Department
    /// </summary>
    public static void LoadUltraComboDepartment(UltraCombo ultraCBDepartment)
    {
      string commandText = "SELECT Department, DeparmentName, Department + ' | ' + DeparmentName Display FROM VHRDDepartment ORDER BY Department";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBDepartment, dtSource, "Department", "Display", "Display");
    }
    /// <summary>
    /// Load Data To UltraCombo Section
    /// </summary>
    public static void LoadUltraComboSection(UltraCombo ultraCBSection, string departmentID)
    {
      string commandText = "Select Section, FullName, Section + ' | ' + FullName Display from FMISDB.dbo.V_SHRSection ";
      commandText += " WHERE ISNULL(Section, '') <> '' ";
      if (departmentID.ToString().Trim().Length > 0)
      {
        commandText += " AND ISNULL(Department, '') = '" + departmentID + "'";
      }
      commandText += " ORDER BY Section ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBSection, dtSource, "Section", "Display", "Display");
    }
    /// <summary>
    /// Load Data To UltraCombo Leave Type
    /// </summary>
    public static void LoadUltraComboLeaveType(UltraCombo ultraCBLeaveType)
    {
      string commandText = "Select LID, VNName, LID + ' | ' + VNName Display from FMISDB.dbo.V_SHRLeaveKind ORDER BY LID";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBLeaveType, dtSource, "LID", "Display", "Display");
    }

    /// <summary>
    /// Load Data To UltraCombo Employee
    /// </summary>
    public static void LoadUltraComboEmployee(UltraCombo ultraCBEmployee, string department)
    {
      string commandText = string.Empty;
      if (department.Length > 0)
      {
        commandText = string.Format("Select Pid, EmpName, Cast(Pid as varchar) + ' | ' + EmpName Display From VHRMEmployee Where Department = '{0}'", department);
        ultraCBEmployee.Text = string.Empty;
      }
      else
      {
        commandText = "Select Pid, EmpName, Cast(Pid as varchar) + ' - ' + EmpName Display From VHRMEmployee";
      }
      if (commandText.Length > 0)
      {
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        LoadUltraCombo(ultraCBEmployee, dtSource, "Pid", "Display", "Display");
      }
    }
    /// <summary>
    /// Load Data To UltraDropDown Employee
    /// </summary>
    public static void LoadUltraDropDownEmployee(UltraDropDown ultraDDEmployee, string department)
    {
      string commandText = string.Empty;
      if (department.Length > 0)
      {
        commandText = string.Format("Select Pid, EmpName, Cast(Pid as varchar) + ' | ' + EmpName Display From VHRMEmployee Where Department = '{0}'", department);
        ultraDDEmployee.Text = string.Empty;
      }
      else
      {
        commandText = "Select Pid, EmpName, Cast(Pid as varchar) + ' - ' + EmpName Display From VHRMEmployee";
      }
      if (commandText.Length > 0)
      {
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        LoadUltraDropDown(ultraDDEmployee, dtSource, "Pid", "Display", "Display");
      }
    }
    public static void LoadUltraCBExhibition(UltraCombo ultraCBExhibition)
    {
      string commandText = string.Format("Select Code, Value From TblBOMCodeMaster Where [Group] = {0} Order By Sort", ConstantClass.GROUP_EXHIBITION);
      DataTable dtExhibition = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBExhibition.DataSource = dtExhibition;
      ultraCBExhibition.ValueMember = "Code";
      ultraCBExhibition.DisplayMember = "Value";
    }

    /// <summary>
    /// Load ultraComboCustomer for item (distribute & Pid > 3)
    /// </summary>
    public static void LoadUltraCBCustomer(UltraCombo ultraCBCustomer)
    {
      //Load ultraComboCustomer (distribute)
      //string commandText = @"Select Pid, CustomerCode Code, Name, (CustomerCode + ' - ' + Name) Display 
      //                      From TblCSDCustomerInfo 
      //                      Where ParentPid Is Null And Pid > 3 and (Kind = 5 or CustomerCode = 'JC' OR CustomerCode = 'BLANK')
      //                      Order By CustomerCode";
      string commandText = @"Select Pid, CustomerCode Code, Name, (CustomerCode + ' - ' + Name) Display 
                            From TblCSDCustomerInfo 
                            Where ParentPid Is Null And Pid > 3
                            Order By CustomerCode";
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBCustomer.DataSource = dtCustomer;
      ultraCBCustomer.ValueMember = "Pid";
      ultraCBCustomer.DisplayMember = "Display";
      ultraCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
    }

    /// <summary>
    /// Load data sale through for ultraCombo
    /// </summary>
    public static void LoadUltraCBSaleThrough(UltraCombo ultraCBSaleThrough)
    {
      string commandText = "SELECT Pid, Name FROM VCSDSaleThrought";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBSaleThrough, dt, "Pid", "Name", false, "Pid");
    }

    /// <summary>
    /// Load JC, OEM Customer List
    /// </summary>
    /// <param name="ultraCBCustomer"></param>
    public static void LoadUltraCBJC_OEM_Customer(UltraCombo ultraCBCustomer)
    {
      string commandText = @"SELECT	Pid, CustomerCode, CustomerCode + ' - ' + Name Display



                             FROM	TblCSDCustomerInfo
                             WHERE	ParentPid IS NULL AND (Pid = 27 OR Kind = 5) ORDER BY CustomerCode";
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBCustomer.DataSource = dtCustomer;
      ultraCBCustomer.ValueMember = "Pid";
      ultraCBCustomer.DisplayMember = "Display";
      ultraCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["CustomerCode"].MaxWidth = 80;
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["CustomerCode"].MinWidth = 80;
    }
    #endregion LoadUltraCombo Data
    /// <summary>
    /// Load data combobox category from Customer service
    /// </summary>
    /// <param name="cmbCategory"></param>
    public static void LoadComboboxCategory(ComboBox cmbCategory)
    {
      string commandText = "Select Pid Code, Category Value From TblCSDCategory Order By Category";
      DataTable dtCategory = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbCategory, dtCategory, "Code", "Value");
    }

    /// <summary>
    /// Load data ultra combobox category from Customer service
    /// </summary>
    /// <param name="cmbCategory"></param>
    public static void LoadUltraCBCategory(UltraCombo ultCBCategory)
    {
      string commandText = @"SELECT CH.Pid Code, PR.USCateCode + ' - ' + PR.Category + ' | ' + CH.Category Value
                            FROM
                            (
                                SELECT *
                                FROM TblCSDCategory
                                WHERE ParentPid IS NULL
                            )PR
                            LEFT JOIN 
                            (
	                            SELECT *
	                            FROM TblCSDCategory
	                            WHERE ParentPid IS NOT NULL
                            )CH ON PR.Pid = CH.ParentPid
                            ORDER BY PR.Category";
      DataTable dtCategory = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCBCategory, dtCategory, "Code", "Value", "Code");
    }

    /// <summary>
    /// Load data combobox collection from Customer service
    /// </summary>
    /// <param name="cmbCategory"></param>
    public static void LoadComboboxCollection(ComboBox cmbCollection)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Value", ConstantClass.GROUP_COLLECTION);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbCollection, dtSource, "Code", "Value");
    }

    /// <summary>
    /// Load data ultra combo collection from Customer service
    /// </summary>
    /// <param name="cmbCategory"></param>
    public static void LoadUltraCBCollection(UltraCombo ultCBCollection)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Value", ConstantClass.GROUP_COLLECTION);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCBCollection, dtSource, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Show Image of Item on the grid data. Notice: the columns name of grid must be "ItemCode" and "Revision"
    /// </summary>
    /// <param name="ultGridData"></param>
    /// <param name="pictureItem"></param>
    /// <param name="showImage"></param>
    /// <returns></returns>
    public static void BOMShowItemImage(UltraGrid ultGridData, GroupBox groupItemImage, PictureBox pictureItem, bool showImage)
    {
      try
      {
        if (showImage)
        {
          UltraGridRow row = ultGridData.Selected.Rows[0];
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          if (itemCode.Length > 0 && revision != int.MinValue)
          {
            groupItemImage.Text = string.Format("Item: {0}, Revision: {1}", itemCode, revision);
            pictureItem.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
            Point xy = new Point();
            int yMax = ultGridData.Location.Y + ultGridData.Height;
            xy.X = ultGridData.Location.X + row.Cells["ItemCode"].Width + row.Cells["Revision"].Width;
            xy.Y = ultGridData.Location.Y + (row.Cells["ItemCode"].Height * (row.Index + 2));
            if (xy.Y + groupItemImage.Height > yMax)
            {
              xy.Y = yMax - groupItemImage.Height;
            }
            groupItemImage.Location = xy;
            groupItemImage.Visible = true;
          }
          else
          {
            groupItemImage.Text = string.Empty;
          }
        }
        else
        {
          groupItemImage.Visible = false;
        }
      }
      catch
      {
        groupItemImage.Text = string.Empty;
      }
    }

    /// <summary>
    /// Show groupbox on grid
    /// </summary>
    /// <param name="ultGridData"></param>
    /// <param name="groupItemImage"></param>
    /// <param name="pictureItem"></param>
    /// <param name="showImage"></param>
    public static void BOMShowGroupboxOnGrid(UltraGrid ultGridData, string colName, int rowIndex, GroupBox groupDetail, bool show)
    {
      try
      {
        if (show)
        {
          UltraGridRow row = ultGridData.Rows[rowIndex];
          Point xy = new Point();
          int xMax = ultGridData.Location.X + ultGridData.Width;
          int yMax = ultGridData.Location.Y + ultGridData.Height;
          int colLocationX = 0;
          for (int i = 0; i <= row.Cells[colName].Column.Index; i++)
          {
            colLocationX += row.Cells[i].Column.Width;
          }
          xy.X = ultGridData.Location.X + colLocationX;
          xy.Y = ultGridData.Location.Y + (row.Cells[colName].Height * (row.Index + 2));
          if (xy.Y + groupDetail.Height > yMax)
          {
            xy.Y = yMax - groupDetail.Height;
          }
          if (xy.X + groupDetail.Width > xMax)
          {
            xy.X = xMax - groupDetail.Width;
          }
          groupDetail.Location = xy;
          groupDetail.Visible = true;
        }
        else
        {
          groupDetail.Visible = false;
        }
      }
      catch
      {
        groupDetail.Text = string.Empty;
      }
    }

    /// <summary>
    /// Show Image of Item on the grid data. Notice: the columns name of grid must be "ItemCode" and "Revision"
    /// </summary>
    /// <param name="ultGridData"></param>
    /// <param name="pictureItem"></param>
    /// <param name="showImage"></param>
    /// <returns></returns>
    public static void BOMShowItemImage(DataGridView dgvData, int indexRow, GroupBox groupItemImage, PictureBox pictureItem, bool showImage)
    {
      try
      {
        DataGridViewRow row = dgvData.Rows[indexRow];
        if (showImage)
        {
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          if (itemCode.Length > 0 && revision != int.MinValue)
          {
            groupItemImage.Text = string.Format("Item: {0}, Revision: {1}", itemCode, revision);
            pictureItem.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
            Point xy = new Point();
            int yMax = dgvData.Location.Y + dgvData.Height;
            xy.X = dgvData.Location.X + dgvData.Columns["ItemCode"].Width + dgvData.Columns["ItemCode"].Width;
            xy.Y = dgvData.Location.Y + (row.Height * (row.Index + 2));
            if (xy.Y + groupItemImage.Height > yMax)
            {
              xy.Y = yMax - groupItemImage.Height;
            }
            groupItemImage.Location = xy;
            groupItemImage.Visible = true;
          }
          else
          {
            groupItemImage.Text = string.Empty;
          }
        }
        else
        {
          groupItemImage.Visible = false;
        }
      }
      catch
      {
        groupItemImage.Text = string.Empty;
      }
    }

    public static string GetSelectedValueMultiCombobox(Shared.UserControls.MultiColumnComboBox multiCB)
    {
      string value = string.Empty;
      try
      {
        value = multiCB.SelectedValue.ToString();
      }
      catch { }
      return value;
    }

    public static string GetValueCheckBoxComboBox(PresentationControls.CheckBoxComboBox chkCB)
    {
      string checkedValue = string.Empty;
      for (int i = 0; i < chkCB.Items.Count; i++)
      {
        if (chkCB.CheckBoxItems[i].Checked)
        {
          if (checkedValue != string.Empty)
          {
            checkedValue += "|";
          }
          checkedValue += chkCB.Items[i].ToString();
        }
      }
      return checkedValue;
    }

    public static void LoadDropdownFinishsingCode(Shared.UserControls.MultiColumnComboBox cmbFinishingCode, string condition)
    {
      string commandText = string.Format(@"SELECT FinCode, Name as FinName, (FinCode + ' | ' + Name) as FinCodeName  
                                           FROM TblBOMFinishingInfo 
                                           WHERE {0}", condition);
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dataSource.NewRow();
      dataSource.Rows.InsertAt(row, 0);
      cmbFinishingCode.DataSource = dataSource;
      cmbFinishingCode.ValueMember = "FinCode";
      cmbFinishingCode.DisplayMember = "FinCodeName";
    }

    public static void LoadUltraDropdownFinishsingCode(UltraDropDown udrpFinishingCode, string condition)
    {
      string commandText = string.Format(@"SELECT FinCode, Name as FinName FROM TblBOMFinishingInfo WHERE {0}", condition);
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpFinishingCode.DataSource = dataSource;
      udrpFinishingCode.DisplayLayout.Bands[0].Columns["FinCode"].Width = 100;
      udrpFinishingCode.DisplayLayout.Bands[0].Columns["FinName"].Width = 250;
    }

    public static void LoadCheckBoxComboBox(PresentationControls.CheckBoxComboBox chkCB, DataTable dtSoure, string value, string text)
    {
      chkCB.Items.Clear();
      chkCB.CheckBoxItems.Clear();
      chkCB.Clear();
      int i = 1;
      foreach (DataRow row in dtSoure.Rows)
      {
        chkCB.Items.Add(row[value]);
        chkCB.CheckBoxItems[i].Text = row[text].ToString();
        i++;
      }
    }

    public static bool CheckBOMMaterialCode(string code, int group)
    {
      if (code.Trim().Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckMaterialCode(@Code, @Group)";
      DBParameter[] arrInput = new DBParameter[2];
      arrInput[0] = new DBParameter("@Code", DbType.AnsiString, 50, code);
      arrInput[1] = new DBParameter("@Group", DbType.Int32, group);
      return Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText(strCommandText, arrInput);
    }

    public static void CheckedCheckBoxComboBox(PresentationControls.CheckBoxComboBox chkCB, string checkedValue)
    {
      checkedValue = string.Format("|{0}|", checkedValue);
      for (int i = 0; i < chkCB.Items.Count; i++)
      {
        chkCB.CheckBoxItems[i].Checked = false;
        if (checkedValue.IndexOf(string.Format("|{0}|", chkCB.Items[i].ToString())) >= 0)
        {
          chkCB.CheckBoxItems[i].Checked = true;
        }
      }
    }

    public static void LoadComboboxPrefix(ComboBox cmbPrefix, int group)
    {
      DataTable dtPrefix = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(string.Format(@"Select PrefixCode From TblBOMPrefix Where [Group] = {0} ORDER BY No", group));
      LoadCombobox(cmbPrefix, dtPrefix, "PrefixCode", "PrefixCode");
    }

    public static void LoadComboboxPrefix(UltraCombo UltraCBPrefix, int group)
    {
      DataTable dtPrefix = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(string.Format(@"Select PrefixCode From TblBOMPrefix Where [Group] = {0} ORDER BY No", group));
      LoadUltraCombo(UltraCBPrefix, dtPrefix, "PrefixCode", "PrefixCode");
    }

    public static void LoadItemImageFolder(UltraCombo ultraCB)
    {
      string commandText = string.Format(@"SELECT Value, Description
                                           FROM TblBOMCodeMaster
                                           WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", ConstantClass.GROUP_ITEM_PATH_FOLDER);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultraCB, dtSource, "Value", "Description", false, "Value");
    }

    public static void LoadItemComboBox(Shared.UserControls.MultiColumnComboBox multiCBItem)
    {
      string commandText = "SELECT ItemCode, Name as ItemName, (ItemCode + ' | ' + Name) as ItemCodeName FROM TblBOMItemBasic";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dtSource.NewRow();
      dtSource.Rows.InsertAt(row, 0);
      multiCBItem.DataSource = dtSource;
      multiCBItem.ValueMember = "ItemCode";
      multiCBItem.DisplayMember = "ItemCodeName";
      multiCBItem.ColumnWidths = "100, 200, 0";
    }

    public static void LoadItemComboBox(UltraCombo ultraCBItem)
    {
      string commandText = "SELECT ItemCode, Name as ItemName, (ItemCode + ' | ' + Name) as ItemCodeName FROM TblBOMItemBasic";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBItem, dtSource, "ItemCode", "ItemCodeName", "ItemCodeName");
    }

    public static void LoadRevisionByItemCode(ComboBox cmbRevision, string itemCode)
    {
      string commandText = string.Format("Select Revision From TblBOMRevision Where ItemCode = '{0}'", itemCode);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbRevision, dtSource, "Revision", "Revision");
    }

    public static void LoadRevisionByItemCode(UltraCombo ultraCBRevision, string itemCode)
    {
      string commandText = string.Format("Select Revision From TblBOMRevision Where ItemCode = '{0}'", itemCode);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBRevision, dtSource, "Revision", "Revision");
    }

    public static void LoadDropDownCarcass(Shared.UserControls.MultiColumnComboBox cmbCarcass)
    {
      string commandText = "Select CarcassCode, Description, (CarcassCode + ' | ' + Description) DescCarcass From TblBOMCarcass Where DeleteFlag = 0 Order By CarcassCode Desc";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbCarcass, dtSource, "CarcassCode", "DescCarcass");
      cmbCarcass.ColumnWidths = "110, 350, 0";
    }

    public static void LoadUltraCBCarcass(UltraCombo ultCBCarcass)
    {
      string commandText = "Select CarcassCode, Description, (CarcassCode + ' | ' + Description) DescCarcass From TblBOMCarcass Where DeleteFlag = 0 Order By CarcassCode Desc";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCBCarcass, dtSource, "CarcassCode", "DescCarcass", false, "DescCarcass");
    }

    public static void LoadOtherMaterials(PresentationControls.CheckBoxComboBox chkOthermaterials)
    {
      string commandText = string.Format(@"SELECT Code, (Value + ISNULL(' - ' + Description, '')) Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Value", Shared.Utility.ConstantClass.GROUP_MATERIALSTYPE);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCheckBoxComboBox(chkOthermaterials, dtSource, "Code", "Value");
    }

    public static string GetSelectedValueUltraCombobox(UltraCombo cmb)
    {
      string value = string.Empty;
      try
      {
        value = cmb.Value.ToString();
      }
      catch { }
      return value;
    }

    public static void LoadDropDownBOMGroupProcess(ComboBox cmbGroupProcess)
    {
      string commandText = string.Format(@"SELECT Pid, ISNULL(GroupProcessName, '') + ' - ' + ISNULL(GroupProcessNameVN, '') GroupProcessName  FROM TblBOMGroupProcess WHERE DeleteFlag = 0 ");
      DataTable dtGropProcess = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbGroupProcess, dtGropProcess, "Pid", "GroupProcessName");
    }

    public static void LoadDropDownBOMGroupProcess(UltraCombo cmbGroupProcess)
    {
      string commandText = string.Format(@"SELECT Pid, GroupProcessNameVN, GroupProcessName, (GroupProcessNameVN + ' | ' + GroupProcessName) DisplayText FROM TblBOMGroupProcess WHERE DeleteFlag = 0 Order By GroupProcessNameVN");
      DataTable dtGropProcess = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      cmbGroupProcess.DataSource = dtGropProcess;
      cmbGroupProcess.ValueMember = "Pid";
      cmbGroupProcess.DisplayMember = "DisplayText";
      cmbGroupProcess.DisplayLayout.Bands[0].ColHeadersVisible = false;
      cmbGroupProcess.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      cmbGroupProcess.DisplayLayout.Bands[0].Columns["DisplayText"].Hidden = true;
    }

    public static void LoadUltraDropdownProcessInfo(UltraDropDown ultraDDProcess)
    {
      string commandText = "Select Pid, ProcessCode, ENDescription, VNDescription, STUFF((SELECT ', ' + NameEn FROM TblBOMMachine WHERE ('|' + PROCESS.MachineGroup + '|') LIKE ('%|' + CAST(Pid as varchar ) + '|%') FOR XML PATH('')),1,2,'') MachineGroup From TblBOMProcessInfo PROCESS";
      DataTable dtProcessInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraDropDown(ultraDDProcess, dtProcessInfo, "Pid", "ProcessCode", "Pid");
    }

    public static void LoadDropdownProfile(UltraDropDown udrpDropDown)
    {
      string commandText = "SELECT Pid, ProfileCode, Description, DescriptionVN FROM TblBOMProfile";
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Pid";
      udrpDropDown.DisplayMember = "ProfileCode";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Description"].Width = 200;
      udrpDropDown.DisplayLayout.Bands[0].Columns["DescriptionVN"].Width = 200;
    }

    public static void LoadDropdownWorkStation(UltraDropDown udrpDropDown)
    {
      string commandText = "SELECT Pid, Station, Team, CASE WHEN [CheckPoint] = 1 THEN 'CheckPoint' END [Checked], [CheckPoint] FROM VBOMWorkStation";
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Pid";
      udrpDropDown.DisplayMember = "Station";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["CheckPoint"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Station"].Width = 200;
    }

    public static void LoadDropdownWorkAreaComponent(UltraDropDown udrpDropDown)
    {
      string commandText = string.Format("SELECT Pid, WorkAreaCode, WorkAreaName FROM TblWIPWorkArea Where IsDeleted = 0 And DevisionCode = '{0}' And Pid <> {1}", Shared.Utility.ConstantClass.Devision_Component, Shared.Utility.ConstantClass.WorkArea_ComponentStore);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Pid";
      udrpDropDown.DisplayMember = "WorkAreaCode";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["WorkAreaCode"].Width = 70;
    }

    public static void LoadDropdownItem(UltraDropDown ultraDDItem)
    {
      string commandText = "Select ItemCode, Name ItemName From TblBOMItemBasic";
      DataTable dtItemInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDItem.DataSource = dtItemInfo;
      ultraDDItem.DisplayLayout.Bands[0].HeaderVisible = false;
      ultraDDItem.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultraDDItem.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
    }

    public static UltraDropDown LoadDropDownTeamByWorkAreaPid(long workAreaPid, UltraDropDown ultraTeam, Control ctr)
    {
      if (ultraTeam == null)
      {
        ultraTeam = new UltraDropDown();
        ctr.Controls.Add(ultraTeam);
      }
      string commandText = "Select STRUCT.Team TeamCode, STRUCT.TeamName From VHRMCompanyStructure STRUCT Inner Join TblWIPWorkArea WORK On ";
      commandText += string.Format("STRUCT.Section = WORK.WorkAreaCode And WORK.Pid = {0}", workAreaPid);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraTeam.DataSource = dtSource;
      ultraTeam.ValueMember = "TeamCode";
      ultraTeam.DisplayMember = "TeamCode";
      ultraTeam.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraTeam.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      return ultraTeam;
    }

    public static void LoadMultiComboEmployeeInSection(DaiCo.Shared.UserControls.MultiColumnComboBox comboBox, string section)
    {
      string sql = string.Format("Select EID AS Pid, EName AS Description From V_SHREmpFollowTeam WHERE (Section LIKE '{0}%' OR TeamCode LIKE '{0}') order by REVERSE(EName)", section);
      DataTable dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(sql);
      DataRow newRow = dt.NewRow();
      dt.Rows.InsertAt(newRow, 0);
      comboBox.DataSource = dt;
      comboBox.SelectedIndex = 0;
      comboBox.ValueMember = "Pid";
      comboBox.DisplayMember = "Description";
    }

    public static void LoadMultiComboEmployeeInDepartment(DaiCo.Shared.UserControls.MultiColumnComboBox comboBox, string dept)
    {
      DataTable dt = null;
      if (dept != string.Empty)
      {
        string sql = string.Format("Select EID AS Pid, RIGHT(REPLICATE('0', 4) + CAST(EID as varchar), 4), (ShortName + ' ' + OtherName) Description, (RIGHT(REPLICATE('0', 4) + CAST(EID as varchar), 4) + ' | ' + ShortName + ' ' + OtherName) TextShow From FMISDB.dbo.V_SHREmpInfo_Out WHERE Department LIKE '{0}%' AND Resigned = 0 ORDER BY OtherName", dept);
        dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(sql);
        DataRow newRow = dt.NewRow();
        dt.Rows.InsertAt(newRow, 0);
      }
      comboBox.DataSource = dt;
      comboBox.SelectedIndex = 0;
      comboBox.ValueMember = "Pid";
      comboBox.DisplayMember = "TextShow";
      comboBox.ColumnWidths = "0, 50, 150, 0";
    }

    public static void LoadComboBoxEmployeeByDept(ComboBox cmb, string dept)
    {
      DataTable dt = null;
      if (dept.Length > 0)
      {
        string sql = string.Format("Select EID, (ShortName + ' ' + OtherName) Name From FMISDB.dbo.V_SHREmpInfo_Out WHERE Department = '{0}' AND Resigned = 0 ORDER BY OtherName", dept);
        dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(sql);
      }
      LoadCombobox(cmb, dt, "EID", "Name");
    }

    /// <summary>
    /// Load Combobox nation
    /// </summary>
    public static void LoadNation(ComboBox cmbNation)
    {
      string commandText = "SELECT Pid, NationEN FROM TblCSDNation";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadCombobox(cmbNation, dt, "Pid", "NationEN");
    }

    public static void LoadUltCBNation(UltraCombo ultCBNation)
    {
      string commandText = "SELECT Pid, NationEN FROM TblCSDNation";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultCBNation, dt, "Pid", "NationEN");
    }

    /// <summary>
    /// Disable all controls in view, except control in conEnables
    /// </summary>
    /// <param name="container">View need disable</param>
    /// <param name="conEnables">Array of control enable</param>
    public static void LockControl(Control container, params Control[] conEnables)
    {
      foreach (Control control in container.Controls)
      {
        LockControlRecursive(control, conEnables);
      }
    }

    /// <summary>
    /// Recursive disable controls
    /// </summary>
    /// <param name="control"></param>
    /// <param name="conEnables"></param>
    public static void LockControlRecursive(Control control, params Control[] conEnables)
    {
      foreach (Control conTrack in control.Controls)
      {
        bool isAllow = false;
        foreach (Control conEnable in conEnables)
        {
          if (conEnable.Name == conTrack.Name)
          {
            conEnable.Enabled = true;
            isAllow = true;
            break;
          }
        }
        if (!isAllow)
        {
          if (conTrack.Controls.Count > 0)
            LockControlRecursive(conTrack, conEnables);
          else if (conTrack.GetType() != typeof(Label))
            conTrack.Enabled = false;
        }
      }
    }

    #region Load data carcass
    public static void LoadDataCarcassComponentStruct(TreeView treeViewComponentStruct, string carcassCode)
    {
      treeViewComponentStruct.Nodes.Clear();
      string commandTextRootComp = string.Format(@"Select Pid, ComponentCode, DescriptionVN, Qty From TblBOMCarcassComponent Where CarcassCode = '{0}' And IsMainComp = 1", carcassCode);
      DataTable dtRootComp = DataBaseAccess.SearchCommandTextDataTable(commandTextRootComp);
      string commandTextSubComp = string.Format(@"SELECT STRUCT.MainCompPid, STRUCT.SubCompPid, SUBCOMP.ComponentCode SubCompCode, 
                                                  SUBCOMP.DescriptionVN SubCompDescription, STRUCT.Qty
                                               FROM TblBOMCarcassComponentStruct STRUCT
  	                                              INNER JOIN TblBOMCarcassComponent SUBCOMP ON SUBCOMP.Pid = STRUCT.SubCompPid AND SUBCOMP.CarcassCode = '{0}' ORDER BY STRUCT.No ASC", carcassCode);
      DataTable dtSubComp = DataBaseAccess.SearchCommandTextDataTable(commandTextSubComp);
      foreach (DataRow rootRow in dtRootComp.Rows)
      {
        TreeNode node = new TreeNode();
        node.Name = rootRow["Pid"].ToString();
        node.Text = string.Format("{0} - {1}, qty: {2}", rootRow["ComponentCode"], rootRow["DescriptionVN"], rootRow["Qty"]);
        treeViewComponentStruct.Nodes.Add(node);
        LoadDataCarcassSubComponent(node, dtSubComp);
      }
    }

    public static void LoadDataCarcassSubComponent(TreeNode mainNode, DataTable dtSubComp)
    {
      long pidMainComp = DBConvert.ParseLong(mainNode.Name.ToString());
      DataRow[] subRows = dtSubComp.Select(string.Format("MainCompPid = {0}", pidMainComp));
      for (int i = 0; i < subRows.Length; i++)
      {
        TreeNode subNode = new TreeNode();
        subNode.Name = subRows[i]["SubCompPid"].ToString();
        subNode.Text = string.Format("{0} - {1}, qty: {2}", subRows[i]["SubCompCode"], subRows[i]["SubCompDescription"], subRows[i]["Qty"]);
        mainNode.Nodes.Add(subNode);
        LoadDataCarcassSubComponent(subNode, dtSubComp);
      }
    }
    #endregion Load data carcass

    #region Warehouse
    public static void LoadUltraSupplier(UltraCombo drpSupplier, bool isAddNewRow)
    {
      string newRow = isAddNewRow ? "SELECT NULL Code, NULL Name UNION" : string.Empty;
      string commandText = string.Format(@"{0} SELECT SupplierCode Code, EnglishName Name
                                          FROM VPURSupplierInfo_PMISDB
                                          WHERE EnglishName IS NOT NULL AND REPLACE(EnglishName, ' ', '') <> ''
                                          ORDER BY Name", newRow);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpSupplier.DataSource = dtSource;
      drpSupplier.ValueMember = "Code";
      drpSupplier.DisplayMember = "Name";
      drpSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      drpSupplier.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 80;
      drpSupplier.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 80;
    }

    public static void LoadUltraDepartment(UltraCombo drpDepartment, bool isAddNewRow)
    {
      string newRow = isAddNewRow ? "SELECT NULL Code, NULL Name UNION" : string.Empty;
      string commandText = string.Format(@"{0} SELECT Code, Name 
                                          FROM VHRDDepartmentInfo DEP
	                                            INNER JOIN VHRMEmployee EMP ON (DEP.Code = EMP.Department)
                                          GROUP BY Code, Name 
                                          ORDER BY Name", newRow);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpDepartment.DataSource = dtSource;
      drpDepartment.ValueMember = "Code";
      drpDepartment.DisplayMember = "Name";
      drpDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      drpDepartment.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 50;
      drpDepartment.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 50;
      drpDepartment.DisplayLayout.Bands[0].Columns["Name"].MinWidth = 200;
      drpDepartment.DisplayLayout.Bands[0].Columns["Name"].MaxWidth = 200;
    }

    public static void LoadUltraEmployeeByDeparment(UltraCombo drpEmployee, string department, bool isAddNewRow)
    {
      string whereClause = (department.Length > 0) ? string.Format(@"Where Department = '{0}'", department) : string.Empty;
      string newRow = isAddNewRow ? "SELECT NULL Pid, NULL EmpName UNION" : string.Empty;
      string commandText = string.Format(@" {0} SELECT Pid, EmpName FROM VHRMEmployee {1} ORDER BY Pid", newRow, whereClause);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpEmployee.DataSource = dtSource;
      drpEmployee.ValueMember = "Pid";
      drpEmployee.DisplayMember = "EmpName";
      drpEmployee.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    public static void LoadUltraMaterialCode(UltraCombo drpMaterialCode, int warehouse)
    {
      string commandText = string.Format(@"SELECT MaterialCode Code, MaterialNameEn EnglishName, IsControl, ControlType,
                                                MaterialCode + ' - ' + MaterialNameEn DisplayName
                                           FROM VBOMMaterials
                                           WHERE Warehouse = {0}
                                           ORDER BY MaterialCode", warehouse);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpMaterialCode.DataSource = dtSource;
      drpMaterialCode.ValueMember = "Code";
      drpMaterialCode.DisplayMember = "DisplayName";
      drpMaterialCode.DisplayLayout.Bands[0].Columns["IsControl"].Hidden = true;
      drpMaterialCode.DisplayLayout.Bands[0].Columns["ControlType"].Hidden = true;
      drpMaterialCode.DisplayLayout.Bands[0].Columns["DisplayName"].Hidden = true;
      drpMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    public static void LoadListMaterials(ucUltraList ultMaterials, int warehouse, int recovery)
    {
      string commandText = string.Format(@"SELECT DISTINCT MAT.MaterialCode [Material Code], MAT.MaterialNameEn [Material Name] 
                                           FROM VWHDMaterialStockBalance BL
	                                              INNER JOIN VBOMMaterials MAT ON (BL.MaterialCode = MAT.MaterialCode)
                                           WHERE Warehouse = {0} AND BL.Recovery = {1}", warehouse, recovery);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultMaterials.DataSource = dtSource;
      ultMaterials.ValueMember = "Material Code";
      ultMaterials.ColumnWidths = "100; 320";
      ultMaterials.DataBind();
    }
    #endregion Warehouse

    #region Foundry
    public static void LoadUltraComboboxComponentGroup(UltraCombo ultCmbGroup, int kind)
    {
      string commandText = string.Format(@"SELECT Pid, GroupCode, GroupNameEN, GroupNameVN, GroupCode + ' - ' + ISNULL(GroupNameEN, '') DisplayMember 
                                           FROM TblFOUComponentGroup 
                                           WHERE Kind = {0}", kind);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCmbGroup.DataSource = dtSource;
      ultCmbGroup.ValueMember = "Pid";
      ultCmbGroup.DisplayMember = "DisplayMember";
      ultCmbGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCmbGroup.DisplayLayout.Bands[0].Columns["GroupCode"].MinWidth = 80;
      ultCmbGroup.DisplayLayout.Bands[0].Columns["GroupCode"].MaxWidth = 80;
      ultCmbGroup.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCmbGroup.DisplayLayout.Bands[0].Columns["DisplayMember"].Hidden = true;
    }

    public static void LoadUltraComboboxFinishingStyle(UltraCombo ultCmbFinishingStyle, bool filterBrassType)
    {
      string commandText = string.Format(@"SELECT FinCode Code, Name, NameVN, FinCode + ' - ' + ISNULL(Name, '')  DisplayMember 
                                           FROM TblBOMFinishingInfo
                                           WHERE DeleteFlag = 0");
      if (filterBrassType)
      {
        commandText = string.Format("{0}  AND BrassStyle = 1", commandText);
      }
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCmbFinishingStyle.DataSource = dtSource;
      ultCmbFinishingStyle.ValueMember = "Code";
      ultCmbFinishingStyle.DisplayMember = "DisplayMember";
      ultCmbFinishingStyle.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCmbFinishingStyle.DisplayLayout.Bands[0].Columns["DisplayMember"].Hidden = true;
      ultCmbFinishingStyle.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 100;
      ultCmbFinishingStyle.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 100;
    }

    /// <summary>
    /// type = 0: all
    /// type = 1: semi component (BRC, IRC, ...)
    /// type = 2: component (DCB, DCA, ...)
    /// </summary>
    /// <param name="ultDDComp"></param>
    /// <param name="type"></param>
    public static void LoadUltraDropDownHardwareComponent(UltraDropDown ultDDComp, int type, params int[] confirm)
    {
      string commandText = string.Format(@"Select CompCode, CompNameEN, CompNameVN, Length, Width, Thickness, Weight, CASE WHEN Confirm = 1 THEN 'Confirmed' ELSE 'Not Confirm' END Confirm 
                                           From TblFOUComponentInfo Where ((Kind = {0}) Or ({0} = 0))", type);
      if (confirm.Length > 0)
      {
        commandText = string.Format(commandText + " AND (Confirm = {0})", confirm[0]);
      }
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDComp.DataSource = dtSource;
      ultDDComp.ValueMember = "CompCode";
      ultDDComp.DisplayMember = "CompCode";
      ultDDComp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDComp.DisplayLayout.Bands[0].Columns["CompCode"].MaxWidth = 100;
      ultDDComp.DisplayLayout.Bands[0].Columns["CompCode"].MinWidth = 100;
      ultDDComp.DisplayLayout.Bands[0].Columns["CompNameEN"].MaxWidth = 200;
      ultDDComp.DisplayLayout.Bands[0].Columns["CompNameEN"].MinWidth = 200;
      ultDDComp.DisplayLayout.Bands[0].Columns["Length"].Hidden = true;
      ultDDComp.DisplayLayout.Bands[0].Columns["Width"].Hidden = true;
      ultDDComp.DisplayLayout.Bands[0].Columns["Thickness"].Hidden = true;
      ultDDComp.DisplayLayout.Bands[0].Columns["Weight"].Hidden = true;
      ultDDComp.DisplayLayout.Bands[0].Columns["CompNameVN"].Hidden = true;
      ultDDComp.DisplayLayout.Bands[0].Columns["Confirm"].Hidden = true;
    }

    /// <summary>
    /// type = 0: all
    /// type = 1: semi component (BRC, IRC, ...)
    /// type = 2: component (DCB, DCA, ...)
    /// </summary>
    /// <param name="ultDDComp"></param>
    /// <param name="type"></param>
    public static void LoadUltraCBHardwareComponent(UltraCombo ultCBComp, int type, params int[] confirm)
    {
      string commandText = string.Format(@"Select CompCode, CompNameEN, CompNameVN, (CompCode + ' | ' + CompNameEN) DisplayText
                                           From TblFOUComponentInfo Where ((Kind = {0}) Or ({0} = 0))", type);
      if (confirm.Length > 0)
      {
        commandText = string.Format(commandText + " AND (Confirm = {0})", confirm[0]);
      }
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCBComp, dtSource, "CompCode", "DisplayText", "DisplayText");
      ultCBComp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBComp.DisplayLayout.Bands[0].Columns["CompCode"].MaxWidth = 100;
      ultCBComp.DisplayLayout.Bands[0].Columns["CompCode"].MinWidth = 100;
    }

    public static void LoadUltraDropDownMaterialForFoundry(UltraDropDown ultDDMaterial)
    {
      string commandText = @"Select MaterialCode, MaterialName, MaterialNameVn, FactoryUnit, IDFactoryUnit From VFOUMaterials Where Used = 1";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDMaterial.DataSource = dtSource;
      ultDDMaterial.ValueMember = "MaterialCode";
      ultDDMaterial.DisplayMember = "MaterialCode";
      ultDDMaterial.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      ultDDMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      ultDDMaterial.DisplayLayout.Bands[0].Columns["MaterialName"].MaxWidth = 300;
      ultDDMaterial.DisplayLayout.Bands[0].Columns["MaterialName"].MinWidth = 300;
      ultDDMaterial.DisplayLayout.Bands[0].Columns["MaterialNameVn"].Hidden = true;
      ultDDMaterial.DisplayLayout.Bands[0].Columns["FactoryUnit"].Hidden = true;
      ultDDMaterial.DisplayLayout.Bands[0].Columns["IDFactoryUnit"].Hidden = true;
    }

    /// <summary>
    /// kind = 0: process for component
    /// kind = 1: process for im-ex WH
    /// </summary>
    /// <param name="ultDDProcess"></param>
    /// <param name="kind"></param>
    public static void LoadUltraDropDownProcess(UltraDropDown ultDDProcess, int kind)
    {
      string commandText = string.Format("Select Pid, ProcessCode, ProcessNameEN, ProcessNameVN From TblFOUProcessInfo WHERE Kind = {0}", kind);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDProcess.DataSource = dtSource;
      ultDDProcess.ValueMember = "Pid";
      ultDDProcess.DisplayMember = "ProcessCode";
      ultDDProcess.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessCode"].MaxWidth = 70;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessCode"].MinWidth = 70;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessNameEN"].MaxWidth = 100;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessNameEN"].MinWidth = 100;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessNameVN"].MaxWidth = 100;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessNameVN"].MinWidth = 100;
    }

    /// <summary>
    /// Load ultra combo Mould
    /// </summary>
    /// <param name="ultDDProcess"></param>
    /// <param name="kind"></param>
    public static void LoadUltraCBMould(UltraCombo ultraCBMould)
    {
      string commandText = string.Format(@"SELECT Pid, MouldCode, MouldName, MouldCode + ' - ' + ISNULL(MouldName, '') DisplayMember
                                          FROM TblFOUMould");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBMould, dtSource, "Pid", "DisplayMember", false, new string[] { "Pid", "DisplayMember" });
    }

    /// <summary>
    /// Load prefix code of component code
    /// kind = 1: semi component.
    /// kind = 2: component.
    /// </summary>
    public static void LoadPrefixCodeForFoundry(UltraCombo ult, int kind)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value 
                                           FROM TblBOMCodeMaster WHERE [Group] = {0} AND Kind = {1} AND DeleteFlag = 0 ORDER BY Sort", DaiCo.Shared.Utility.ConstantClass.GROUP_FOUNDY_COMPONENT_KIND, kind);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ult, dtSource, "Value", "Value", false);
      ult.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }
    /// <summary>
    /// Built tree component struct
    /// </summary>
    /// <param name="note"></param>
    /// <param name="componentCode"></param>
    /// <param name="subcomponentCode"></param>
    /// <param name="revision"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static TreeNode GetTreeComponentStruct(TreeNode note, string componentCode, string subcomponentCode, int revision, int level)
    {
      string commandText = string.Format(@"SELECT ST.SemiComponentCode, COM.CompNameEN, ST.QtyPerParent
                                           FROM TblFOUComponentStructRevision ST 
                                               LEFT JOIN TblFOUComponentInfo COM ON (ST.SemiComponentCode = COM.CompCode)
                                           WHERE ST.ComponentCode='{0}' AND ST.ParentSemiComponent = '{1}' 
                                               AND ST.Revision = {2} AND ST.[Level]= {3} ", componentCode, subcomponentCode, revision, level);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      foreach (DataRow row in dtSource.Rows)
      {
        string root = string.Format("{0} | {1} (Qty: {2})", row["SemiComponentCode"].ToString(), row["CompNameEN"].ToString(), row["QtyPerParent"].ToString());
        TreeNode childNode = new TreeNode(root);
        note.Nodes.Add(childNode);
        GetTreeComponentStruct(childNode, componentCode, row["SemiComponentCode"].ToString(), revision, level + 1);
      }
      return note;
    }

    /// <summary>
    /// Show struct of component selected in grid
    /// </summary>
    /// <param name="ultData"> UltraGird for select data </param>
    /// <param name="treeViewStruct"> Treeview show struct </param>
    /// <param name="compCode"> Component need get struct </param>
    /// <param name="compName"> Name of component </param>
    /// <param name="revision"> Revision of component </param>
    /// <param name="curMousePoint"> Position of mouse click </param>
    /// <param name="oldPoint"> Old position of mouse click </param>
    /// <param name="startX"> Position X of treeview </param>
    /// <returns></returns>
    public static Point ShowStruct(UltraGrid ultData, TreeView treeViewStruct, string compCode, string compName, int revision, Point curMousePoint, Point oldPoint, int startX)
    {
      if (ultData.Selected.Rows.Count > 0)
      {
        UltraGridRow row = ultData.Selected.Rows[0];
        string rev = (revision != int.MinValue) ? revision.ToString() : string.Empty;
        treeViewStruct.Nodes.Clear();
        string root = string.Format("{0} | {1} (Revision: {2})", compCode, compName, rev);
        treeViewStruct.Nodes.Add(compCode, root);
        TreeNode tree = treeViewStruct.Nodes[compCode];
        treeViewStruct.Nodes.Clear();
        treeViewStruct.Nodes.Add(GetTreeComponentStruct(tree, compCode, compCode, revision, 1));

        curMousePoint = ultData.PointToClient(curMousePoint);
        curMousePoint.X += startX + 15;
        if ((curMousePoint.Y + treeViewStruct.Height) <= (ultData.Location.Y + ultData.Height))
        {
          if (curMousePoint.Y < 0)
          {
            curMousePoint = oldPoint;
          }
          else
          {
            curMousePoint.Y += row.Height;
          }
        }
        else
        {
          int h = (curMousePoint.Y + treeViewStruct.Height) - (ultData.Location.Y + ultData.Height);
          curMousePoint.Y -= h;
        }
        oldPoint = curMousePoint;
        treeViewStruct.Location = curMousePoint;
      }
      return oldPoint;
    }

    /// <summary>
    /// Change Header Caption
    /// </summary>
    /// <param name="uc"></param>
    /// <param name="newHeader"></param>
    public static void ChangedHeaderCaption(MainUserControl uc, string newHeader)
    {
      TabControl tabContent = DaiCo.Shared.Utility.SharedObject.tabContent;
      tabContent.SelectTab(uc.Name);
      TabPage activeTab = tabContent.SelectedTab;
      activeTab.Text = newHeader + "     ";
    }
    #endregion Foundry

    #region CustomerService
    public static void LoadUltraComboCustomer(UltraCombo ucmb, string whereClause, bool isAddRowNull)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer 
                                           FROM TblCSDCustomerInfo 
                                           {0}
                                           ORDER BY Customer", whereClause);
      if (isAddRowNull)
      {
        commandText = string.Format("SELECT NULL Pid, NULL Customer UNION {0}", commandText);
      }
      DataTable dataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ucmb, dataSource, "Pid", "Customer", new string[] { "Pid" });
      ucmb.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    #endregion CustomerService

    #region General
    public static void LoadUltraComboMaterialGroup(UltraCombo ucmbGroup, bool isAddRowNull)
    {
      string rowNull = isAddRowNull ? "SELECT NULL [Group], NULL Description,  NULL Display UNION" : string.Empty;
      string commandText = string.Format(@"{0} 
                                           SELECT [Group], Description, [Group] + ' - ' + ISNULL(Description, '') Display
                                           FROM VBOMMaterialGroup ORDER BY [Group]", rowNull);
      DataTable dtSource = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucmbGroup.DataSource = dtSource;
      ucmbGroup.ValueMember = "Group";
      ucmbGroup.DisplayMember = "Display";
      ucmbGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ucmbGroup.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
      ucmbGroup.DisplayLayout.Bands[0].Columns["Group"].MinWidth = 60;
      ucmbGroup.DisplayLayout.Bands[0].Columns["Group"].MaxWidth = 60;
    }

    public static void ViewCrystalReport(CrystalDecisions.CrystalReports.Engine.ReportClass cpt)
    {
      SaveFileDialog f = new SaveFileDialog();
      f.Filter = "Report files (*.rpt)|*.rpt";
      if (f.ShowDialog() == DialogResult.OK)
      {
        string strName = f.FileName;
      GoBack:;
        try
        {
          File.Open(f.FileName, FileMode.OpenOrCreate).Close();
        }
        catch
        {
          MessageBox.Show("Already Opened:" + f.FileName + "\nPlease find and close it", "Can not save!");
          goto GoBack;
        }
        cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.CrystalReport, strName);
        System.Diagnostics.Process.Start(@strName);
      }
    }

    public static void ExportToExcel(UltraGrid ultraGridSource, string pathOutputFile, params int[] startRow)
    {
      Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExport = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
      excelExport.Export(ultraGridSource, pathOutputFile);

      // Add new row
      if (startRow.Length > 0)
      {
        object missing = System.Reflection.Missing.Value;
        Excel.Application excelApp = new Excel.Application();
        Excel.Workbook xlBook = excelApp.Workbooks.Open(pathOutputFile, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
        Excel.Sheets xlSheets = xlBook.Worksheets;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlSheets.get_Item(1);

        Excel.Range rng = (Excel.Range)xlSheet.Cells[1, 1];
        Excel.Range row = rng.EntireRow;
        for (int i = 1; i < startRow[0]; i++)
        {
          row.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);
        }
        xlBook.Close(true, missing, missing);
      }
      Process.Start(pathOutputFile);
    }

    /// <summary>
    /// Workbook return is not closed. Please close it before viewing for user.
    /// Example: xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="pathOutputFile"></param>
    /// <param name="startRow"></param>
    /// <returns></returns>
    public static void ExportToExcel(UltraGrid ultraGridSource, out Excel.Workbook xlBook, string pathOutputFile, params int[] startRow)
    {
      Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExport = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
      excelExport.Export(ultraGridSource, pathOutputFile);

      object missing = System.Reflection.Missing.Value;
      Excel.Application excelApp = new Excel.Application();
      xlBook = excelApp.Workbooks.Open(pathOutputFile, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
      Excel.Sheets xlSheets = xlBook.Worksheets;
      Excel.Worksheet xlSheet = (Excel.Worksheet)xlSheets.get_Item(1);

      Excel.Range rng = (Excel.Range)xlSheet.Cells[1, 1];
      Excel.Range row = rng.EntireRow;
      // Add new row
      if (startRow.Length > 0)
      {
        for (int i = 1; i < startRow[0]; i++)
        {
          row.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);
        }
      }
      //xlBook.Close(true, missing, missing);      
    }

    /// <summary>
    /// Default Path: StartupPath\Report\
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="pathOutputFile"></param>
    public static void ExportToExcelWithDefaultPath(UltraGrid ultraGridSource, string fileName)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      if (!Directory.Exists(pathOutputFile))
      {
        Directory.CreateDirectory(pathOutputFile);
      }
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExport = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
      excelExport.Export(ultraGridSource, strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Default Path: StartupPath\Report\
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="pathOutputFile"></param>
    public static void ExportToExcelWithDefaultPath(UltraGrid ultraGridSource, string fileName, int startRow)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      if (!Directory.Exists(pathOutputFile))
      {
        Directory.CreateDirectory(pathOutputFile);
      }
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      ExportToExcel(ultraGridSource, strOutFileName, startRow);
    }

    /// <summary>    
    /// Workbook return is not closed. Please close it before viewing for user.
    /// Example: xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="fileName"></param>
    /// <param name="startRow"></param>
    /// <returns></returns>
    public static void ExportToExcelWithDefaultPath(UltraGrid ultraGridSource, out Excel.Workbook xlBook, string fileName, params int[] startRow)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      if (!Directory.Exists(pathOutputFile))
      {
        Directory.CreateDirectory(pathOutputFile);
      }
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      ExportToExcel(ultraGridSource, out xlBook, strOutFileName, startRow);
    }
    #endregion General

    #region UltraGrid Properties
    public static void SetPropertiesUltraGrid(UltraGrid ultraGridData)
    {
      ultraGridData.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      ultraGridData.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
    }

    public static void GetDataForClipboard(UltraGrid ultraGridData)
    {
      string newLine = System.Environment.NewLine;
      string tab = "\t";
      string clipboard_string = "";
      if (ultraGridData.Selected.Rows.Count > 0)
      {
        // Get Caption
        for (int i = 0; i < ultraGridData.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          if (!ultraGridData.DisplayLayout.Bands[0].Columns[i].Hidden)
          {
            string header = ultraGridData.DisplayLayout.Bands[0].Columns[i].Header.Caption.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            clipboard_string = string.Format("{0}{1}{2}", clipboard_string, header, tab);
          }
        }

        // Get Data
        for (int nRow = 0; nRow < ultraGridData.Selected.Rows.Count; nRow++)
        {
          clipboard_string = string.Format("{0}{1}", clipboard_string, newLine);
          Infragistics.Win.UltraWinGrid.UltraGridRow row = ultraGridData.Selected.Rows[nRow];
          for (int i = 0; i < row.Cells.Count; i++)
          {
            if (!row.Cells[i].Column.Hidden)
            {
              clipboard_string = string.Format("{0}{1}{2}", clipboard_string, row.Cells[i].Text, tab);
            }
          }
        }
      }
      else if (ultraGridData.Selected.Columns.Count > 0)
      {
        // Get Caption
        for (int i = 0; i < ultraGridData.Selected.Columns.Count; i++)
        {
          string header = ultraGridData.Selected.Columns[i].Caption.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
          clipboard_string = string.Format("{0}{1}{2}", clipboard_string, header, tab);
        }

        // Get Data
        for (int nRow = 0; nRow < ultraGridData.Rows.Count; nRow++)
        {
          Infragistics.Win.UltraWinGrid.UltraGridRow row = ultraGridData.Rows[nRow];
          if (row.IsFilteredOut == false)
          {
            clipboard_string = string.Format("{0}{1}", clipboard_string, newLine);

            for (int i = 0; i < ultraGridData.Selected.Columns.Count; i++)
            {
              int colIndex = ultraGridData.Selected.Columns[i].Column.Index;
              clipboard_string = string.Format("{0}{1}{2}", clipboard_string, row.Cells[colIndex].Text, tab);
            }
          }
        }
      }
      Clipboard.SetText(clipboard_string);
    }

    #endregion UltraGrid Properties

    public static void LoadItemComboBox()
    {
      throw new Exception("The method or operation is not implemented.");
    }
  }
}

