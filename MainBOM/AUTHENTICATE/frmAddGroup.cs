/*
  Author      : Ha Anh
  Description : Add Group
  Date        : 05-10-2011
*/

using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace MainBOM.AUTHENTICATE
{
  public partial class frmAddGroup : Form
  {
    #region variable
    private enum TreeNodeType { View = 0, Control = 1, Group = 2, InActive = 3 };
    private bool loadingUnchecked = false;

    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool checkDelete = false;
    private bool checkDuplicate = false;
    private long groupPid = long.MinValue;
    private long copyGroupPid = long.MinValue;
    #endregion variable

    #region Load Data
    /// <summary>
    /// init form
    /// </summary>
    public frmAddGroup()
    {
      InitializeComponent();
    }

    /// <summary>
    /// load form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAddGroup_Load(object sender, EventArgs e)
    {
      this.radUser.Checked = true;
      this.LoadData();
      this.InitTreeView();
      this.LoadcbCopyGroup();
      this.LoadDDRole();
    }

    /// <summary>
    /// load down drop role
    /// </summary>
    private void LoadDDRole()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] =" + DaiCo.Shared.Utility.ConstantClass.GROUP_ROLE + " ORDER BY Sort";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDRole.DataSource = dt;
      ultDDRole.DisplayMember = "Value";
      ultDDRole.ValueMember = "Code";
      ultDDRole.DisplayLayout.AutoFitColumns = true;
      ultDDRole.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDRole.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load Group copy structure
    /// </summary>
    private void LoadcbCopyGroup()
    {
      string commandText = "SELECT Pid, NameEN, [Description] FROM TblGNRAccessGroup ORDER BY NameEN";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultGroupCopy.DataSource = dt;
      ultGroupCopy.DisplayMember = "NameEN";
      ultGroupCopy.ValueMember = "Pid";
      ultGroupCopy.DisplayLayout.AutoFitColumns = true;
      ultGroupCopy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultGroupCopy.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// init tree view
    /// </summary>
    private void InitTreeView()
    {
      tvAuthenticate.Nodes.Clear();
      tvAuthenticate.CheckBoxes = true;
      string cmd = "SELECT * FROM TblGNRDefineUI ORDER BY OrderBy";
      DataTable dtUIDefine = DataBaseAccess.SearchCommandTextDataTable(cmd);

      string commandText = string.Format("SELECT ControlName, UICode FROM TblGNRDefineUIControl");
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
    /// function load data
    /// </summary>
    private void LoadData()
    {
      this.Search();
    }
    #endregion variable

    #region Event
    /// <summary>
    /// ultragird init layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Role"].ValueList = ultDDRole;
    }

    /// <summary>
    /// button close click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// button save click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      if (ultGroupCopy.Text.Trim().Length > 0 && txGroupControl.Text.Trim().Length > 0)
      {
        if (WindowUtinity.ShowMessageConfirm("MSG0048", ultGroupCopy.Text, txGroupControl.Text).ToString() == "No")
        {
          return;
        }
      }
      bool check = ValidationInput(out message);
      if (!check)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      check = this.Save();
      message = string.Empty;
      if (check)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        if (this.checkDelete)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0054"), "Group");
          WindowUtinity.ShowMessageErrorFromText(message);
        }
        if (this.checkDuplicate)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Group");
          WindowUtinity.ShowMessageErrorFromText(message);
        }
      }
      this.ReloadData();
    }

    /// <summary>
    /// reload data
    /// </summary>
    private void ReloadData()
    {
      //copy group structure
      this.CopyStructure();
      ultGroupCopy.Text = string.Empty;
      txGroupControl.Text = string.Empty;
      this.LoadcbCopyGroup();
      this.Search();
    }

    /// <summary>
    /// after row delete 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    /// <summary>
    /// before row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        string commandText = "SELECT	COUNT(Pid) FROM	TblGNRAccessGroupUser WHERE	GroupPid =" + pid;
        int count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText));
        if (count > 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0054"), "Group");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// btn search click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// before cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      int count = 0;
      if (colName == "nameen")
      {
        for (int i = 0; i < ultraGrid.Rows.Count; i++)
        {
          if (e.Cell.Row.Cells["NameEN"].Text == ultraGrid.Rows[i].Cells["NameEN"].Text)
          {
            count++;
            if (count == 2)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Username");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
              break;
            }
          }
        }
      }
      count = 0;
      if (colName == "role")
      {
        string role = e.Cell.Row.Cells["Role"].Text;
        if (role.Length == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Role");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
        for (int i = 0; i < ultDDRole.Rows.Count; i++)
        {
          if (ultDDRole.Rows[i].Cells["Value"].Text == e.Cell.Row.Cells["Role"].Text)
          {
            count = 1;
            break;
          }
        }
        if (count == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Role");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
        }
      }
    }

    /// <summary>
    /// before row update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_BeforeRowUpdate(object sender, CancelableRowEventArgs e)
    {
      string message = string.Empty;
      if (e.Row.Cells["NameEN"].Text.Trim().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "NameEN");
        WindowUtinity.ShowMessageErrorFromText(message);
        e.Cancel = true;
      }
      else if (e.Row.Cells["Role"].Text.Trim().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Role");
        WindowUtinity.ShowMessageErrorFromText(message);
        e.Cancel = true;
      }
    }

    /// <summary>
    /// add user click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddUser_Click(object sender, EventArgs e)
    {
      frmAddUserIntoGroup form = new frmAddUserIntoGroup();
      form.ShowDialog();
    }

    /// <summary>
    /// button code master click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnControlCodeMaster_Click(object sender, EventArgs e)
    {
      frmCodeMasterControl form = new frmCodeMasterControl();
      form.ShowDialog();
    }

    /// <summary>
    /// event double click row grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_DoubleClick(object sender, EventArgs e)
    {
      if (radCode.Checked)
      {
        bool selected = false;
        try
        {
          selected = ultraGrid.Selected.Rows[0].Selected;
        }
        catch
        {
          selected = false;
        }
        if (!selected)
        {
          return;
        }
        UltraGridRow row = ultraGrid.Selected.Rows[0];
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        frmCodeMasterControl form = new frmCodeMasterControl();
        form.group = pid;
        form.ShowDialog();
      }
      else
      {
        bool selected = false;
        try
        {
          selected = ultraGrid.Selected.Rows[0].Selected;
        }
        catch
        {
          selected = false;
        }
        if (!selected)
        {
          return;
        }
        UltraGridRow row = ultraGrid.Selected.Rows[0];
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        frmAddUserIntoGroup form = new frmAddUserIntoGroup();
        form.group = pid;
        form.ShowDialog();
      }
    }

    /// <summary>
    /// after select
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tvAuthenticate_AfterSelect(object sender, TreeViewEventArgs e)
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

    /// <summary>
    /// Click node 
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
    /// Save click Auth Group - UI
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveAuth_Click(object sender, EventArgs e)
    {
      if (groupPid == long.MinValue)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0011", "Group");
        return;
      }

      IList nodeUNSelect = new ArrayList();
      foreach (MainTreeNode node in tvAuthenticate.Nodes)
      {
        SaveAuthenticateUIAndControl(node, nodeUNSelect);
      }
      if (nodeUNSelect.Count > 0)
      {
        string beDeleted = string.Empty;
        foreach (long s in nodeUNSelect)
          beDeleted += "," + s;
        beDeleted = beDeleted.Remove(0, 1);
        string cmdDeleteView = string.Format("DELETE TblGNRGroupUI WHERE GroupPid = {0} AND UIPid IN ({1})", groupPid, beDeleted);
        DataBaseAccess.ExecuteCommandText(cmdDeleteView);

        string cmdDeleteControl = string.Format("DELETE TblGNRGroupUIControl WHERE GroupPid = {0} AND UIPid IN ({1})", groupPid, beDeleted);
        DataBaseAccess.ExecuteCommandText(cmdDeleteControl);
      }
      //int right = 0;
      //if (chxComponent.Checked && !chxCarcass.Checked)
      //  right = 1;
      //else if (!chxComponent.Checked && chxCarcass.Checked)
      //  right = 2;
      //else if (chxComponent.Checked && chxCarcass.Checked)
      //  right = 3;

      //DBParameter[] param = new DBParameter[2];
      //param[0] = new DBParameter("@UserPid", DbType.Int64, userPid);
      //param[1] = new DBParameter("@Right", DbType.Int32, right);
      //DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spWIPUpdateRightToUser", param);

      DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
    }

    /// <summary>
    /// mouse click so that choose group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_MouseClick(object sender, MouseEventArgs e)
    {
      if (ultraGrid.Selected.Rows.Count > 0)
      {
        string group = ultraGrid.Selected.Rows[0].Cells["NameEN"].Text.Trim();
        txGroupControl.Text = group;

        if (DBConvert.ParseLong(ultraGrid.Selected.Rows[0].Cells["Pid"].Text.Trim()) != long.MinValue)
        {
          groupPid = DBConvert.ParseLong(ultraGrid.Selected.Rows[0].Cells["Pid"].Text.Trim());
        }
      }
    }

    /// <summary>
    /// group text change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txGroupControl_TextChanged(object sender, EventArgs e)
    {
      if (ultraGrid.Selected.Rows.Count > 0)
      {
        if (DBConvert.ParseLong(ultraGrid.Selected.Rows[0].Cells["Pid"].Text.Trim()) != long.MinValue)
        {
          groupPid = DBConvert.ParseLong(ultraGrid.Selected.Rows[0].Cells["Pid"].Text.Trim());
        }
        else
        {
          groupPid = long.MinValue;
        }

        string commandText = string.Format("SELECT UIPid FROM TblGNRGroupUI WHERE GroupPid = {0}", groupPid);
        DataTable dtcommand = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.SetCheckedNode(tvAuthenticate.Nodes, dtcommand);
      }
    }

    /// <summary>
    /// group copy select and value change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGroupCopy_ValueChanged(object sender, EventArgs e)
    {
      if (ultGroupCopy.Text.Trim().Length == 0 || ultGroupCopy.Value == null)
      {
        copyGroupPid = 0;
      }
      else if (ultGroupCopy.Text.Trim().Length > 0 && ultGroupCopy.Value != null)
      {
        copyGroupPid = DBConvert.ParseLong(ultGroupCopy.Value.ToString());
      }
    }

    private void ultDDRole_RowSelected(object sender, RowSelectedEventArgs e)
    {
      for (int i = 0; i < ultraGrid.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGrid.Rows[i].Cells["Role"].Value.ToString()) == 1)
        {
          if (ultDDRole.Rows.Count >= 2)
          {
            ultDDRole.SelectedRow = ultDDRole.Rows[1];
          }
        }
      }
    }
    #endregion Event

    #region Function

    /// <summary>
    /// search event
    /// </summary>
    private void Search()
    {
      string group = txtGroup.Text.Trim();
      string commandText = string.Empty;
      if (group.Length > 0)
      {
        commandText = " SELECT Pid, NameEN, Role, Description FROM TblGNRAccessGroup WHERE " +
                        " NameEN LIKE '%" + group + "%'  ORDER BY NameEN ASC";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        ultraGrid.DataSource = dt;
      }
      else
      {
        commandText = "SELECT Pid, NameEN, Role, Description FROM TblGNRAccessGroup ORDER BY NameEN ASC";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        ultraGrid.DataSource = dt;
      }
    }

    /// <summary>
    /// check data khi insert group
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool ValidationInput(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultraGrid.Rows.Count; i++)
      {
        if (ultraGrid.Rows[i].Cells["NameEN"].Text.Trim().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Group");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save data input
    /// </summary>
    /// <returns></returns>
    private bool Save()
    {
      //delete group
      bool result = true;
      long outputValue = long.MinValue;
      if (this.listDeletedPid != null)
      {
        foreach (long pid in this.listDeletedPid)
        {
          DBParameter[] inputParamDelete = new DBParameter[2];
          inputParamDelete[0] = new DBParameter("@GroupPid", DbType.Int64, pid);
          inputParamDelete[1] = new DBParameter("@IsDelete", DbType.Int32, 1);

          DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spGNRAccessGroup_Edit", inputParamDelete, OutputParamDelete);
          outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
          if (outputValue == 0)
          {
            result = false;
          }
          else if (outputValue == 2)
          {
            this.checkDelete = true;
            result = false;
          }
        }
      }

      // insert update
      DataTable dt = (DataTable)ultraGrid.DataSource;
      if (dt != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState == DataRowState.Modified || dt.Rows[i].RowState == DataRowState.Added)
          {
            if (dt.Rows[i]["NameEN"].ToString().Trim().Length > 0)
            {
              string storename = "spGNRAccessGroup_Edit";
              long GroupPid = DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString());
              DBParameter[] inputParam = new DBParameter[4];

              if (GroupPid != long.MinValue)
              {
                inputParam[0] = new DBParameter("@GroupPid", DbType.Int64, GroupPid);
              }
              inputParam[1] = new DBParameter("@NameEN", DbType.AnsiString, 256, dt.Rows[i]["NameEN"].ToString());
              inputParam[2] = new DBParameter("@Role", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Role"].ToString()));
              inputParam[3] = new DBParameter("@Description", DbType.String, 256, dt.Rows[i]["Description"].ToString());

              DBParameter[] outParam = new DBParameter[1];
              outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);
              long outResult = DBConvert.ParseLong(outParam[0].Value.ToString());
              if (outResult == 0 && outResult == long.MinValue)
              {
                return false;
              }
              if (outResult == 2)
              {
                this.checkDuplicate = true;
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// save authen Group - UI
    /// </summary>
    /// <param name="selectNode"></param>
    /// <param name="nodeUnselect"></param>
    private void SaveAuthenticateUIAndControl(MainTreeNode selectNode, IList nodeUnselect)
    {
      if (selectNode.Checked)
      {
        long groupUIPid = long.MinValue;
        if ((TreeNodeType)selectNode.Tag != TreeNodeType.Control)
        {
          string cmd = string.Format("SELECT Pid FROM TblGNRGroupUI WHERE GroupPid = {0} AND UIPid = '{1}'", groupPid, selectNode.UIPid);
          object objGroupUI = DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(cmd);
          if (objGroupUI == null)
          {
            GNRGroupUI groupUI = new GNRGroupUI();
            groupUI.UICode = selectNode.Name;
            groupUI.UIPid = selectNode.UIPid;
            groupUI.GroupPid = groupPid;
            groupUIPid = DataBaseAccess.InsertObject(groupUI);
          }
          else
          {
            groupUIPid = (long)objGroupUI;
          }

          if (groupUIPid > 0)
          {
            string controlUNSelect = string.Empty;
            foreach (MainTreeNode control in selectNode.Nodes)
            {
              if (control.ImageIndex == (int)TreeNodeType.Control)
              {
                if (control.Checked)
                {
                  string cmdGroupUIControl = string.Format("SELECT Pid FROM TblGNRGroupUIControl WHERE GroupPid = {0} AND UIPid = '{1}' AND ControlName = '{2}'", groupPid, selectNode.UIPid, control.Name);
                  object objGroupUIControl = DataBaseAccess.ExecuteScalarCommandText(cmdGroupUIControl);
                  if (objGroupUIControl == null)
                  {
                    GNRGroupUIControl groupUIControl = new GNRGroupUIControl();
                    groupUIControl.GroupPid = groupPid;
                    groupUIControl.UICode = selectNode.Name;
                    groupUIControl.UIPid = selectNode.UIPid;
                    groupUIControl.ControlName = control.Name;
                    DataBaseAccess.InsertObject(groupUIControl);
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
              string cmdDeleteControl = string.Format("DELETE TblGNRGroupUIControl WHERE GroupPid = {0} AND UIPid = '{1}' AND ControlName IN ({2})", groupPid, selectNode.UIPid, controlUNSelect);
              DataBaseAccess.ExecuteCommandText(cmdDeleteControl);
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
          SaveAuthenticateUIAndControl(chileNode, nodeUnselect);
        }
    }

    /// <summary>
    /// click copy structure of group for a new group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyStructure()
    {
      if (ultGroupCopy.Text == txGroupControl.Text)
      {
        return;
      }
      if (copyGroupPid > 0 && txGroupControl.Text.Trim().Length > 0)
      {
        string query = "SELECT Pid FROM TblGNRAccessGroup WHERE NameEN ='" + txGroupControl.Text.Trim() + "'";
        object obj = DataBaseAccess.ExecuteScalarCommandText(query);
        if (obj == null)
        {
          return;
        }
        long group = DBConvert.ParseLong(obj.ToString());
        if (group > 0)
        {
          string storeName = "spGNRGroupStructure_Copy";
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@Group", DbType.Int64, group);
          inputParam[1] = new DBParameter("@GroupCopy", DbType.Int64, copyGroupPid);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          long result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          if (result == 0)
          {
            WindowUtinity.ShowMessageError("ERR0108");
          }
        }
      }
    }

    /// <summary>
    /// check tree nodes
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="dtcommand"></param>
    private void SetCheckedNode(TreeNodeCollection nodes, DataTable dtcommand)
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
              string cmd = string.Format("SELECT ControlName FROM TblGNRGroupUIControl WHERE GroupPid = {0} AND UIPid = {1}", groupPid, node.UIPid);
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
              string commandtext = string.Format("SELECT ControlName From TblGNRGroupUIControl Where GroupPid = {0} And UIPid = {1} And ControlName = '{2}'", groupPid, parentNode.UIPid, node.Name);
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
          this.SetCheckedNode(node.Nodes, dtcommand);
        }
      }
    }

    /// <summary>
    /// uncheck tree nodes
    /// </summary>
    /// <param name="node"></param>
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

    /// <summary>
    /// create child Nodes
    /// </summary>
    /// <param name="dtUIDefine"></param>
    /// <param name="dtControl"></param>
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
            MainTreeNode contrNode = new MainTreeNode(controlName["ControlName"].ToString(), controlName["ControlName"].ToString(), "", (int)TreeNodeType.Control, (int)TreeNodeType.Control);
            contrNode.Tag = TreeNodeType.Control;
            contrNode.ImageIndex = (int)TreeNodeType.Control;
            contrNode.SelectedImageIndex = (int)TreeNodeType.Control;
            childNode.Nodes.Add(contrNode);
          }
        }
      }
    }
    #endregion Function
  }
}