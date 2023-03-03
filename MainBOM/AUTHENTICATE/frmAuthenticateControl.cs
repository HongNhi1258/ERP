/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 07-10-2010
  Company : Dai Co 
*/

using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace MainBOM.AUTHENTICATE
{
  public partial class frmAuthenticateControl : Form
  {
    #region Init
    private enum TreeNodeType { View = 0, Control = 1, Group = 2, InActive = 3 };

    public frmAuthenticateControl()
    {
      InitializeComponent();
      ultControls.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(ultControls_InitializeLayout);
      btnSave.Click += new EventHandler(btnSave_Click);
      btnClose.Click += new EventHandler(btnClose_Click);
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAuthenticateControl_Load(object sender, EventArgs e)
    {
      this.InitTreeView();
      this.LoadControlType();
    }
    #endregion Init

    #region LoadData

    /// <summary>
    /// Load List Button Of View
    /// </summary>
    /// <param name="commandName"></param>
    private void LoadControlsOfView(MainTreeNode selectNode)
    {
      grpControls.Text = string.Format("Controls List Of View {0}", selectNode.Text);
      if (ucbControlType.SelectedRow != null)
      {
        int controlType = DBConvert.ParseInt(ucbControlType.Value);
        DataTable dtSource = new DataTable();
        dtSource.Columns.Add("ControlName", typeof(string));
        dtSource.Columns.Add("ControlText", typeof(string));
        dtSource.Columns.Add("Selected", typeof(int));
        dtSource.Columns.Add("RowState", typeof(int));
        dtSource.Columns["Selected"].DefaultValue = 0;
        dtSource.Columns["RowState"].DefaultValue = 0;
        try
        {
          string typeName = "DaiCo." + selectNode.FullPathDomain + "." + selectNode.Name + "," + selectNode.FullPathDomain;
          DaiCo.Shared.MainUserControl uc = (DaiCo.Shared.MainUserControl)System.Activator.CreateInstance(Type.GetType(typeName, false, true));

          if (uc != null)
          {
            string commandText = string.Format(@"SELECT ControlName FROM TblGNRDefineUIControl WHERE UICode = '{0}'", selectNode.Name);
            DataTable dtExistingControl = DataBaseAccess.SearchCommandTextDataTable(commandText);

            foreach (Control ctr in uc.Controls)
            {
              this.GetControlList(dtSource, ctr, dtExistingControl, controlType);
            }
          }
        }
        catch (Exception e)
        { }
        ultControls.DataSource = dtSource;
      }
    }

    /// <summary>
    /// Get all button controls in a form
    /// </summary>
    private void GetControlList(DataTable dtSource, Control mainCtr, DataTable dtExistingControl, int controlType)
    {
      DataRow row = dtSource.NewRow();
      if ((controlType == 1 && mainCtr.GetType() == typeof(Button)) || (controlType == 2 && mainCtr.GetType() == typeof(Label)) || (controlType == 3 && mainCtr.GetType() == typeof(TextBox)) || (controlType == 4 && mainCtr.GetType() == typeof(TabPage)) || (controlType == 0))
      {
        row["ControlName"] = mainCtr.Name;
        row["ControlText"] = mainCtr.Text;

        DataRow[] rows = dtExistingControl.Select(string.Format("ControlName = '{0}'", mainCtr.Name));
        if (rows.Length > 0)
        {
          row["Selected"] = 1;
          row["RowState"] = 1;
        }
        dtSource.Rows.Add(row);
      }
      if (mainCtr.Controls.Count > 0)
      {
        foreach (Control subCtr in mainCtr.Controls)
        {
          GetControlList(dtSource, subCtr, dtExistingControl, controlType);
        }
      }
    }

    private void InitTreeView()
    {
      tvAuthenticate.Nodes.Clear();
      string cmd = "SELECT * FROM TblGNRDefineUI ORDER BY OrderBy";
      DataTable dtUIDefine = DataBaseAccess.SearchCommandTextDataTable(cmd);

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
        this.CreateChileNode(dtUIDefine, node, pid);
      }
    }

    private void LoadControlType()
    {
      string commandText = "SELECT Code, Name, (CAST(Code as varchar) + ' - ' + Name) DisplayText FROM VGNRControlTypeList";
      DataTable dtControlType = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DaiCo.ERPProject.Utility.LoadUltraCombo(ucbControlType, dtControlType, "Code", "DisplayText", false, "DisplayText");
      ucbControlType.Value = 1;
    }

    /// <summary>
    /// Recursive to make chilenode for TreeView
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="node"></param>
    /// <param name="pid"></param>
    private void CreateChileNode(DataTable dtUIDefine, MainTreeNode node, long pid)
    {
      int iconTree = 0;
      DataRow[] chileRows = dtUIDefine.Select("ParentPid = " + pid);
      if (chileRows.Length > 0)
      {
        foreach (DataRow row in chileRows)
        {
          DataRow[] haveChile = dtUIDefine.Select("ParentPid = " + row["Pid"].ToString());
          MainTreeNode childNode;
          if (haveChile.Length > 0)
          {
            string viewName = row["UICode"].ToString();
            string path = row["NameSpace"].ToString();
            iconTree = (DBConvert.ParseInt(row["IsActive"].ToString()) == 1 ? (int)TreeNodeType.Group : (int)TreeNodeType.InActive);
            childNode = new MainTreeNode(row["Title"].ToString(), viewName, path, iconTree, iconTree);
            childNode.UIPid = (long)row["Pid"];
            childNode.Tag = TreeNodeType.Group;
            node.Nodes.Add(childNode);
            long childPid = DBConvert.ParseLong(row["Pid"].ToString());
            if (childPid != long.MinValue)
              CreateChileNode(dtUIDefine, childNode, childPid);
          }
          else
          {
            string viewName = row["UICode"].ToString();
            string path = row["NameSpace"].ToString();
            int viewState = DBConvert.ParseInt(row["ViewState"].ToString());
            iconTree = (DBConvert.ParseInt(row["IsActive"].ToString()) == 1 ? (int)TreeNodeType.View : (int)TreeNodeType.InActive);
            childNode = new MainTreeNode(row["Title"].ToString(), viewName, path, iconTree, iconTree);
            childNode.UIPid = (long)row["Pid"];
            childNode.Tag = TreeNodeType.View;
            node.Nodes.Add(childNode);
          }
        }
      }
    }
    #endregion LoadData

    #region SaveData

    /// <summary>
    /// Save Data
    /// </summary>
    private void SaveData()
    {
      MainTreeNode selectNode = (MainTreeNode)tvAuthenticate.SelectedNode;
      DataTable dtSource = (DataTable)ultControls.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        int select = DBConvert.ParseInt(row["Selected"].ToString());
        int rowState = DBConvert.ParseInt(row["RowState"].ToString());

        string controlName = row["ControlName"].ToString().Trim();
        string controlText = row["ControlText"].ToString().Trim();
        if (select == 1 && rowState == 0)
        {
          GNRDefineUIControl controlAuthen = new GNRDefineUIControl();
          controlAuthen.UICode = selectNode.Name;
          controlAuthen.ControlName = controlName;
          controlAuthen.Description = controlText;
          long controlPid = DataBaseAccess.InsertObject(controlAuthen);

          if (controlPid == long.MinValue)
          {
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return;
          }
          row["RowState"] = 1;
        }
        else if (select == 0 && rowState == 1)
        {
          string commandText = string.Format("DELETE FROM TblGNRDefineUIControl WHERE UICode = '{0}' AND ControlName = '{1}'", selectNode.Name, controlName);
          bool result = DataBaseAccess.ExecuteCommandText(commandText);
          if (result == false)
          {
            DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return;
          }
          row["RowState"] = 0;
        }
      }
      DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
    }
    #endregion SaveData

    #region Event

    /// <summary>
    /// Event Button Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    /// <summary>
    /// Event Button Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultControls_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["ControlName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ControlText"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;
    }

    private void tvAuthenticate_AfterSelect(object sender, TreeViewEventArgs e)
    {
      this.LoadControlsOfView((MainTreeNode)e.Node);
    }

    private void ucbControlType_ValueChanged(object sender, EventArgs e)
    {
      if (tvAuthenticate.SelectedNode != null)
      {
        this.LoadControlsOfView((MainTreeNode)tvAuthenticate.SelectedNode);
      }
    }
    #endregion Event
  }
}