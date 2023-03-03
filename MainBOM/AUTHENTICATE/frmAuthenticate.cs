/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 30-09-2010
  Company : Dai Co 
*/

using DaiCo.Application;
using DaiCo.ERPProject;
using DaiCo.General;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace MainBOM.AUTHENTICATE
{
  public partial class frmAuthenticate : Form
  {
    #region Init    
    private enum TreeNodeType { View = 0, Control = 1, Group = 2, InActive = 3 };
    private bool loadingUnchecked = false;
    public frmAuthenticate()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAuthenticate_Load(object sender, EventArgs e)
    {
      Utility.LoadUltraComboDepartment(ultraCBDepartment);
      Utility.LoadUltraComboEmployee(ultraCBEmployee, string.Empty);
      btnResetPass.Enabled = false;
      btnSameRight.Enabled = false;
      InitTreeView();

      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uEGSearch);
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
    #endregion Init

    #region LoadData
    private void SearchData()
    {
      string deptCode = string.Empty;
      int userPid = int.MinValue;
      string userName = string.Empty;
      DBParameter[] inputParam = new DBParameter[3];
      if (ultraCBDepartment.SelectedRow != null)
      {
        deptCode = ultraCBDepartment.SelectedRow.Cells["Department"].Value.ToString();
        if (deptCode.Length > 0)
        {
          inputParam[0] = new DBParameter("@DeptCode", DbType.AnsiString, 50, deptCode);
        }
      }
      if (ultraCBEmployee.SelectedRow != null)
      {
        userPid = DBConvert.ParseInt(ultraCBEmployee.SelectedRow.Cells["Pid"].Value.ToString());
        if (userPid > 0)
        {
          inputParam[1] = new DBParameter("@EmpPid", DbType.Int32, userPid);
        }
      }
      if (txtUserName.Text.Trim().Length > 0)
      {
        userName = txtUserName.Text.Trim();
        inputParam[2] = new DBParameter("@UserName", DbType.AnsiString, 255, string.Format("%{0}%", userName));
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRListUser", inputParam);
      ultEmpList.DataSource = ds.Tables[0];
      ultGroup.DataSource = ds.Tables[1];
    }

    private void InitTreeView()
    {
      tvAuthenticate.Nodes.Clear();
      string cmd = "SELECT * FROM TblGNRDefineUI ORDER BY OrderBy";
      DataTable dtUIDefine = DataBaseAccess.SearchCommandTextDataTable(cmd);

      string commandText = string.Format("SELECT ControlName, [Description], UICode FROM TblGNRDefineUIControl");
      DataTable dtControl = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DataRow[] rows = dtUIDefine.Select("ParentPid IS NULL ");
      List<MainTreeNode> controller = new List<MainTreeNode>();
      foreach (DataRow row in rows)
      {
        int iconTree = (DBConvert.ParseInt(row["IsActive"].ToString()) == 1 ? (int)TreeNodeType.Group : (int)TreeNodeType.InActive);
        MainTreeNode node = new MainTreeNode(row["Title"].ToString(), "", "", iconTree, iconTree);
        long pid = DBConvert.ParseLong(row["Pid"].ToString());
        node.UIPid = pid;
        node.Tag = TreeNodeType.Group;
        tvAuthenticate.Nodes.Add(node);
        CreateChildNode(dtUIDefine, dtControl, node, pid);
      }
    }

    /// <summary>
    /// Recursive to make chilenode for TreeView
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="node"></param>
    /// <param name="pid"></param>
    private void CreateChildNode(DataTable dtUIDefine, DataTable dtControl, MainTreeNode node, long pid)
    {
      int iconTree = 0;
      DataRow[] chileRows = dtUIDefine.Select("ParentPid = " + pid);
      if (chileRows.Length > 0)
      {
        foreach (DataRow row in chileRows)
        {
          DataRow[] haveChile = dtUIDefine.Select("ParentPid = " + row["Pid"].ToString());
          string viewName = row["UICode"].ToString();
          string path = row["NameSpace"].ToString();
          MainTreeNode childNode;
          if (haveChile.Length > 0)
          {
            iconTree = (DBConvert.ParseInt(row["IsActive"].ToString()) == 1 ? (int)TreeNodeType.Group : (int)TreeNodeType.InActive);
            childNode = new MainTreeNode(row["Title"].ToString(), viewName, path, iconTree, iconTree);
            childNode.UIPid = (long)row["Pid"];
            childNode.Tag = TreeNodeType.Group;
            node.Nodes.Add(childNode);
            long childPid = DBConvert.ParseLong(row["Pid"].ToString());
            if (childPid != long.MinValue)
              CreateChildNode(dtUIDefine, dtControl, childNode, childPid);
          }
          else
          {
            iconTree = (DBConvert.ParseInt(row["IsActive"].ToString()) == 1 ? (int)TreeNodeType.View : (int)TreeNodeType.InActive);
            childNode = new MainTreeNode(row["Title"].ToString(), viewName, path, iconTree, iconTree);
            childNode.UIPid = (long)row["Pid"];
            childNode.Tag = TreeNodeType.View;
            node.Nodes.Add(childNode);
          }

          // Add control to view
          DataRow[] rowsControl = dtControl.Select(string.Format("UICode = '{0}'", viewName));
          foreach (DataRow controlName in rowsControl)
          {
            MainTreeNode contrNode = new MainTreeNode(string.Format("{0} ({1})", controlName["Description"], controlName["ControlName"]), controlName["ControlName"].ToString(), "", (int)TreeNodeType.Control, (int)TreeNodeType.Control);
            contrNode.Tag = TreeNodeType.Control;
            contrNode.ImageIndex = (int)TreeNodeType.Control;
            contrNode.SelectedImageIndex = (int)TreeNodeType.Control;
            childNode.Nodes.Add(contrNode);
          }
        }
      }
    }

    /// <summary>
    /// Set Check Status of ChildNodes
    /// </summary>
    /// <param name="treeNode"></param>
    /// <param name="checkedState"></param>
    private void SetChildrenChecked(TreeNode treeNode)
    {
      foreach (TreeNode item in treeNode.Nodes)
      {
        if ((TreeNodeType)item.Tag != TreeNodeType.Control || !treeNode.Checked)
        {
          item.Checked = treeNode.Checked;
          this.SetChildrenChecked(item);
        }
      }
    }

    /// <summary>
    /// Set Check Status of ParentNodes
    /// </summary>
    /// <param name="node"></param>
    private void SetParentChecked(TreeNode node)
    {
      if (node.Parent != null)
      {
        bool parentChecked = false;
        foreach (TreeNode n in node.Parent.Nodes)
        {
          if (n.Checked)
          {
            parentChecked = true;
            break;
          }
        }
        node.Parent.Checked = parentChecked;
        this.SetParentChecked(node.Parent);
      }
    }

    private void SetUncheckedNode(TreeNode node)
    {
      node.Checked = false;
      foreach (TreeNode childNode in node.Nodes)
      {
        if (childNode.Nodes.Count > 0)
        {
          SetUncheckedNode(childNode);
        }
        else
        {
          childNode.Checked = false;
        }
      }
    }

    /// <summary>
    /// Set Check Status Of Employee
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="dtcommand"></param>
    private void SetCheckedNode(TreeNodeCollection nodes, DataTable dtcommand, long userPid)
    {
      foreach (MainTreeNode node in nodes)
      {
        if (dtcommand != null)
        {
          DataRow[] rows = dtcommand.Select(string.Format("UIPid = {0} ", node.UIPid));
          if (rows.Length > 0)
          {
            node.Checked = true;
            if ((TreeNodeType)node.Tag != TreeNodeType.Control)
            {
              string cmd = string.Format("Select ControlName FROM TblGNRUserUIControl WHERE UserPid = {0} AND UIPid = {1}", userPid, node.UIPid);
              DataTable dtAllowControl = DataBaseAccess.SearchCommandTextDataTable(cmd);
              if (dtAllowControl != null && dtAllowControl.Rows.Count > 0)
              {
                foreach (TreeNode controlNode in node.Nodes)
                {
                  DataRow[] allowNode = dtAllowControl.Select(string.Format("ControlName = '{0}'", controlNode.Name));
                  controlNode.Checked = (allowNode.Length > 0);
                }
              }
            }
          }
          else
          {
            if ((TreeNodeType)node.Tag == TreeNodeType.Control)
            {
              MainTreeNode parentNode = (MainTreeNode)node.Parent;
              string commandtext = string.Format("Select ControlName From TblGNRUserUIControl Where UserPid = {0} And UIPid = {1} And ControlName = '{2}'", userPid, parentNode.UIPid, node.Name);
              DataTable dtControl = DataBaseAccess.SearchCommandTextDataTable(commandtext);
              if (dtControl != null && dtControl.Rows.Count > 0)
              {
                node.Checked = true;
              }
              else
              {
                node.Checked = false;
              }
            }
            else
            {
              this.loadingUnchecked = true;
              this.SetUncheckedNode(node);
              this.loadingUnchecked = false;
            }
          }
        }
        else
        {
          node.Checked = false;
        }

        if (node.Nodes.Count > 0 && node.Checked && (TreeNodeType)node.Tag != TreeNodeType.Control)
        {
          this.SetCheckedNode(node.Nodes, dtcommand, userPid);
        }
      }
    }

    #endregion LoadData

    #region CheckData
    /// <summary>
    /// Check Input Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      if (ultEmpList.Selected.Rows.Count == 0)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0011", "User");
        return false;
      }
      return true;
    }
    #endregion CheckData

    #region Event

    /// <summary>
    /// Event Button Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool result = this.CheckValid();
      if (result)
      {
        long userPid = DBConvert.ParseLong(ultEmpList.Selected.Rows[0].Cells["EmployeePid"].Value.ToString());
        IList nodeUNSelect = new ArrayList();
        foreach (MainTreeNode node in tvAuthenticate.Nodes)
        {
          SaveAuthenticateUIAndControl(node, userPid, nodeUNSelect);
        }
        if (nodeUNSelect.Count > 0)
        {
          string beDeleted = string.Empty;
          foreach (long s in nodeUNSelect)
            beDeleted += "," + s;
          beDeleted = beDeleted.Remove(0, 1);
          string cmdDeleteView = string.Format("DELETE TblGNRUserUI WHERE UserPid = {0} AND UIPid IN ({1})", userPid, beDeleted);
          DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText(cmdDeleteView);

          string cmdDeleteControl = string.Format("DELETE TblGNRUserUIControl WHERE UserPid = {0} AND UIPid IN ({1})", userPid, beDeleted);
          DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText(cmdDeleteControl);
        }

        int right = 0;
        if (chxComponent.Checked && !chxCarcass.Checked)
          right = 1;
        else if (!chxComponent.Checked && chxCarcass.Checked)
          right = 2;
        else if (chxComponent.Checked && chxCarcass.Checked)
          right = 3;

        DBParameter[] param = new DBParameter[2];
        param[0] = new DBParameter("@UserPid", DbType.Int64, userPid);
        param[1] = new DBParameter("@Right", DbType.Int32, right);
        DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spWIPUpdateRightToUser", param);

        DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }

    private void SaveAuthenticateUIAndControl(MainTreeNode selectNode, long userPid, IList nodeUnselect)
    {
      if (selectNode.Checked)
      {
        long userUIPid = long.MinValue;
        if ((TreeNodeType)selectNode.Tag != TreeNodeType.Control)
        {
          string cmd = string.Format("Select Pid FROM TblGNRUserUI WHERE UserPid = {0} AND UIPid = '{1}'", userPid, selectNode.UIPid);
          object objUserUI = DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(cmd);
          if (objUserUI == null)
          {
            GNRUserUI userUI = new GNRUserUI();
            userUI.UICode = selectNode.Name;
            userUI.UIPid = selectNode.UIPid;
            userUI.UserPid = userPid;
            userUIPid = DataBaseAccess.InsertObject(userUI);
          }
          else
          {
            userUIPid = (long)objUserUI;
          }

          if (userUIPid > 0)
          {
            string controlUNSelect = string.Empty;
            foreach (MainTreeNode control in selectNode.Nodes)
            {
              if (control.ImageIndex == (int)TreeNodeType.Control)
              {
                if (control.Checked)
                {
                  string cmdUserUIControl = string.Format("Select Pid FROM TblGNRUserUIControl WHERE UserPid = {0} AND UICode = '{1}' AND ControlName = '{2}'", userPid, selectNode.Name, control.Name);
                  object objUserUIControl = DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(cmdUserUIControl);
                  if (objUserUIControl == null)
                  {
                    GNRUserUIControl userUIControl = new GNRUserUIControl();
                    userUIControl.UserPid = userPid;
                    userUIControl.UICode = selectNode.Name;
                    userUIControl.UIPid = selectNode.UIPid;
                    userUIControl.ControlName = control.Name;
                    DataBaseAccess.InsertObject(userUIControl);
                  }
                }
                else
                {
                  controlUNSelect += ",'" + control.Name + "'";
                }
              }
            }
            if (controlUNSelect != string.Empty)
            {
              controlUNSelect = controlUNSelect.Remove(0, 1);
              string cmdDeleteControl = string.Format("DELETE TblGNRUserUIControl WHERE UserPid = {0} AND UICode = '{1}' AND ControlName IN ({2})", userPid, selectNode.Name, controlUNSelect);
              DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText(cmdDeleteControl);
            }
          }
        }
      }
      else
      {
        nodeUnselect.Add(selectNode.UIPid);
      }
      if ((TreeNodeType)selectNode.Tag == TreeNodeType.Group)
        foreach (MainTreeNode chileNode in selectNode.Nodes)
        {
          SaveAuthenticateUIAndControl(chileNode, userPid, nodeUnselect);
        }
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultEmpList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["CreateDate"].Format = DaiCo.Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
    }

    /// <summary>
    /// Event Node TreeView Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tvAuthenticate_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      TreeNode node = e.Node;
      tvAuthenticate.SelectedNode = node;
      //this.SetChildrenChecked(node);
      this.SetParentChecked(node);
    }

    /// <summary>
    /// Event Select Change In UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultEmpList_AfterSelectChange(object sender, Infragistics.Win.UltraWinGrid.AfterSelectChangeEventArgs e)
    {
      if (ultEmpList.Selected.Rows.Count > 0)
      {
        long userPid = DBConvert.ParseLong(ultEmpList.Selected.Rows[0].Cells["EmployeePid"].Value.ToString());
        string commandText = string.Format("SELECT UIPid FROM TblGNRUserUI WHERE UserPid = {0}", userPid);
        DataTable dtcommand = DataBaseAccess.SearchCommandTextDataTable(commandText);
        btnResetPass.Enabled = true;
        btnSameRight.Enabled = true;
        this.SetCheckedNode(tvAuthenticate.Nodes, dtcommand, userPid);

        chxComponent.Checked = false;
        chxCarcass.Checked = false;
        string cmdWIPRight = "SELECT AccessRight FROM TblWIPUserAccessRight WHERE UserPid = " + userPid;
        object objRight = DataBaseAccess.ExecuteScalarCommandText(cmdWIPRight);

        if (objRight != null)
        {
          int right = (int)objRight;
          if (right == 1)//Component
          {
            chxComponent.Checked = true;
            chxCarcass.Checked = false;
          }
          else if (right == 2)//Carcass
          {
            chxComponent.Checked = false;
            chxCarcass.Checked = true;
          }
          else if (right == 3)//Component And Carcass
          {
            chxComponent.Checked = true;
            chxCarcass.Checked = true;
          }
        }
      }
    }

    /// <summary>
    /// Event Button Add Employee Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddEmployee_Click(object sender, EventArgs e)
    {
      frmAddUser frm = new frmAddUser();
      string dept = string.Empty;
      if (ultraCBDepartment.SelectedRow != null)
      {
        dept = ultraCBDepartment.SelectedRow.Cells["Department"].Value.ToString();
      }
      frm.department = dept;
      frm.ShowDialog();
      this.SearchData();
    }

    private void btnResetPass_Click(object sender, EventArgs e)
    {
      if (ultEmpList.Selected.Rows.Count > 0)
      {
        long employeePid = DBConvert.ParseLong(ultEmpList.Selected.Rows[0].Cells["EmployeePid"].Value.ToString());
        string defaultPass = DaiCo.Shared.Utility.FunctionUtility.EncodePassword("123456");
        string cmd = string.Format("UPDATE TblBOMUser SET PasswordI = '{0}' WHERE EmployeePid = {1}", defaultPass, employeePid);
        bool result = DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText(cmd);
        if (result)
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0016");
        else
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0047");
      }
      else
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("MSG0011", "the user");
      }
    }

    private void btnSameRight_Click(object sender, EventArgs e)
    {
      long employeePid = DBConvert.ParseLong(ultEmpList.Selected.Rows[0].Cells["EmployeePid"].Value.ToString());
      if (employeePid != long.MinValue)
      {
        frmSelectUser frm = new frmSelectUser();
        frm.toUserPid = employeePid;
        frm.ShowDialog();

        long userPid = DBConvert.ParseLong(ultEmpList.Selected.Rows[0].Cells["EmployeePid"].Value.ToString());
        string commandText = string.Format("SELECT UIPid FROM TblGNRUserUI WHERE UserPid = {0}", userPid);
        DataTable dtcommand = DataBaseAccess.SearchCommandTextDataTable(commandText);
        btnResetPass.Enabled = true;
        btnSameRight.Enabled = true;
        this.SetCheckedNode(tvAuthenticate.Nodes, dtcommand, userPid);
      }
    }

    private void mnuNewChildNode_Click(object sender, EventArgs e)
    {
      MainTreeNode selectNode = (MainTreeNode)tvAuthenticate.SelectedNode;
      if (selectNode != null && (TreeNodeType)selectNode.Tag != TreeNodeType.Control)
      {
        frmNodeProperties frm = new frmNodeProperties();
        frm.UIparentPid = selectNode.UIPid;
        frm.ShowDialog();
        InitTreeView();
      }
    }

    private void mnuProperties_Click(object sender, EventArgs e)
    {
      MainTreeNode selectNode = (MainTreeNode)tvAuthenticate.SelectedNode;
      if (selectNode != null && (TreeNodeType)selectNode.Tag != TreeNodeType.Control)
      {
        frmNodeProperties frm = new frmNodeProperties();
        frm.UIPid = selectNode.UIPid;
        TreeNode parentNode = selectNode.Parent;
        if (parentNode != null)
        {
          frm.UIparentPid = ((MainTreeNode)parentNode).UIPid;
        }
        frm.ShowDialog();
        InitTreeView();
      }
    }

    private void btnAddModult_Click(object sender, EventArgs e)
    {
      frmNodeProperties frm = new frmNodeProperties();
      frm.UIPid = long.MinValue;
      frm.UIparentPid = long.MinValue;
      frm.ShowDialog();
      InitTreeView();
    }

    private void tvAuthenticate_AfterCheck(object sender, TreeViewEventArgs e)
    {
      if (!this.loadingUnchecked)
      {
        if (!e.Node.Checked)
        {
          foreach (TreeNode node in e.Node.Nodes)
          {
            if (node.Checked)
            {
              e.Node.Checked = true;
              break;
            }
          }
        }
      }
    }

    private void ultraCBDepartment_ValueChanged(object sender, EventArgs e)
    {
      string dept = string.Empty;
      if (ultraCBDepartment.SelectedRow != null)
      {
        dept = ultraCBDepartment.SelectedRow.Cells["Department"].Value.ToString();
      }
      Utility.LoadUltraComboEmployee(ultraCBEmployee, dept);
    }

    private void ultraCBDepartment_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Department"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Department"].MaxWidth = 70;
    }

    private void ultraCBEmployee_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Pid"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Pid"].MaxWidth = 70;
    }

    private void ultGroup_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Value"].Header.Caption = "Role";
      e.Layout.Bands[0].Override.CellClickAction = CellClickAction.RowSelect;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Search user information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void ultEmpList_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultEmpList.Selected != null && ultEmpList.Selected.Rows.Count > 0)
      {
        int userPid = DBConvert.ParseInt(ultEmpList.Selected.Rows[0].Cells["EmployeePid"].Value.ToString());
        frmAddUser frm = new frmAddUser();
        frm.userPid = userPid;
        frm.ShowDialog();
      }
      this.SearchData();
    }

    private void ultGroup_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultGroup.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultGroup.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      frmAddUserIntoGroup form = new frmAddUserIntoGroup();
      form.group = pid;
      form.ShowDialog();
    }

    /// <summary>
    /// Add Group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnModuleCodeMaster_Click(object sender, EventArgs e)
    {
      frmAddGroup frmAddGroup = new frmAddGroup();
      frmAddGroup.ShowDialog();
      this.SearchData();
    }
    #endregion Event

    private void btnCheckRight_Click(object sender, EventArgs e)
    {

      frmSearchUICode frm = new frmSearchUICode();
      frm.Show();

      //DaiCo.Shared.Utility.WindowUtinity.ShowView(frm, "INFOR UNICODE", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }
  }
}