/*
  Author      : Nguyen Thanh Binh
  Date        : 27/03/2021
  Description : List of type account
  Standard Form: view_ExtraControl.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_09_001 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_ExtraControl).Assembly);
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function

    /// <summary>
    /// Load tab data
    /// </summary>
    private void LoadTabData()
    {
      // Load Tab Data Component
      string tabPageName = utabAccount.SelectedTab.TabPage.Name;
      switch (tabPageName)
      {
        case "utpcAccountList":
          this.LoadDataAccountList();
          break;
        case "utpcAccountStruct":
          this.LoadDataAccountStruct(tvAccountDetail);
          break;
        default:
          break;
      }
    }


    private void LoadDataAccountList()
    {
      DataSet ds = SqlDataBaseAccess.SearchStoreProcedure("spACCAccount_List");
      ds.Relations.Add(new DataRelation("LV0_LV1", new DataColumn[] { ds.Tables[0].Columns["Pid"] },
                                                new DataColumn[] { ds.Tables[1].Columns["ParentPid"] }, false));
      ds.Relations.Add(new DataRelation("LV1_LV2", new DataColumn[] { ds.Tables[1].Columns["Pid"] },
                                              new DataColumn[] { ds.Tables[2].Columns["ParentPid"] }, false));
      ds.Relations.Add(new DataRelation("LV2_LV3", new DataColumn[] { ds.Tables[2].Columns["Pid"] },
                                            new DataColumn[] { ds.Tables[3].Columns["ParentPid"] }, false));
      ds.Relations.Add(new DataRelation("LV3_LV4", new DataColumn[] { ds.Tables[3].Columns["Pid"] },
                                            new DataColumn[] { ds.Tables[4].Columns["ParentPid"] }, false));
      ugrdAccoutList.DataSource = ds;
    }

    /// <summary>
    /// Load account Struct
    /// </summary>
    private void LoadDataAccountStruct(TreeView treeViewAccountStruct)
    {
      treeViewAccountStruct.Nodes.Clear();
      string commandTextRootComp = string.Format(@"SELECT PId, AccountCode, AccountName
                                                  FROM TblACCAccount PR
                                                  WHERE ParentPid = 1"
                                                  );
      DataTable dtRootAccount = DataBaseAccess.SearchCommandTextDataTable(commandTextRootComp);
      string commandTextSubComp = string.Format(@"SELECT PR.Pid MainAccountPid, CL.Pid SubAccountPid, CL.AccountCode SubAccountCode, CL.AccountName SubAccountName
                                              FROM TblACCAccount PR
                                              INNER JOIN TblACCAccount CL ON PR.Pid = CL.ParentPid
							                                              AND PR.IsActive = 1
							                                              AND CL.IsActive = 1");
      DataTable dtSubAccount = DataBaseAccess.SearchCommandTextDataTable(commandTextSubComp);
      foreach (DataRow rootRow in dtRootAccount.Rows)
      {
        TreeNode node = new TreeNode();
        node.Name = rootRow["Pid"].ToString();
        node.Text = string.Format("{0} - {1}", rootRow["AccountCode"], rootRow["AccountName"]);
        treeViewAccountStruct.Nodes.Add(node);
        LoadDataSubAccount(node, dtSubAccount);
      }
    }

    /// <summary>
    /// Load sub account on tree view
    /// </summary>
    private void LoadDataSubAccount(TreeNode mainNode, DataTable dtSubAccount)
    {
      long pidMainAccount = DBConvert.ParseLong(mainNode.Name.ToString());
      DataRow[] subRows = dtSubAccount.Select(string.Format("MainAccountPid = {0}", pidMainAccount));
      for (int i = 0; i < subRows.Length; i++)
      {
        TreeNode subNode = new TreeNode();
        subNode.Name = subRows[i]["SubAccountPid"].ToString();
        subNode.Text = string.Format("{0} - {1}", subRows[i]["SubAccountCode"], subRows[i]["SubAccountName"]);
        mainNode.Nodes.Add(subNode);
        LoadDataSubAccount(subNode, dtSubAccount);
      }
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadType();
      this.LoadPostingRule();

      // Set Language
      //this.SetLanguage();
    }

    /// <summary>
    /// Load Type
    /// </summary>
    private void LoadType()
    {
      string cmd = string.Format(@"
                                SELECT Code, [Value]
                                FROM TblBOMCodeMaster
                                WhERE [Group] = 6002");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      Utility.LoadUltraCombo(ucbType, dt, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Load Type
    /// </summary>
    private void LoadPostingRule()
    {
      string cmd = string.Format(@"
                                SELECT Code, [Value]
                                FROM TblBOMCodeMaster
                                WhERE [Group] = 6003");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      Utility.LoadUltraCombo(ucbPostingRule, dt, "Code", "Value", false, "Code");
    }

    private void SetNeedToSave()
    {
      if (btnNew.Enabled && btnNew.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private void LoadData()
    {
      this.LoadTabData();
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
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewACC_09_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_09_001_Load(object sender, EventArgs e)
    {
      this.SetBlankForTextOfButton(this);
      //Init Data
      this.InitData();
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ugdInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }


    private void mnuAddCount_Click(object sender, EventArgs e)
    {
      TreeNode selectNode = (TreeNode)tvAccountDetail.SelectedNode;
      if (selectNode != null)
      {
        long pidMainAccount = DBConvert.ParseLong(selectNode.Name.ToString());
        viewACC_09_002 view = new viewACC_09_002();
        view.pidMainAccount = DBConvert.ParseLong(selectNode.Name);
        Shared.Utility.WindowUtinity.ShowView(view, "Tạo tài khoản mới", true, ViewState.ModalWindow);
        this.LoadDataAccountStruct(tvAccountDetail);
      }
    }

    private void mnuProperties_Click(object sender, EventArgs e)
    {
      TreeNode selectNode = (TreeNode)tvAccountDetail.SelectedNode;
      if (selectNode != null)
      {
        long pidAccount = DBConvert.ParseLong(selectNode.Name.ToString());
        viewACC_09_002 view = new viewACC_09_002();
        view.pidAccount = pidAccount;
        TreeNode parentNode = selectNode.Parent;
        if (parentNode != null)
        {
          view.pidMainAccount = DBConvert.ParseLong(((TreeNode)parentNode).Name);
        }
        Shared.Utility.WindowUtinity.ShowView(view, "Chi tiết tài khoản", false, ViewState.ModalWindow);
        this.LoadDataAccountStruct(tvAccountDetail);
      }
    }


    private void tvAccountDetail_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      TreeNode node = e.Node;
      tvAccountDetail.SelectedNode = node;
    }


    private void btnNew_Click(object sender, EventArgs e)
    {
      viewACC_09_002 view = new viewACC_09_002();
      Shared.Utility.WindowUtinity.ShowView(view, "Tạo tài khoản mới", false, ViewState.ModalWindow);
      this.LoadTabData();
    }


    private void tsmiAddNew_Click(object sender, EventArgs e)
    {
      //int i = ugrdAccoutList.ActiveRow.Cells["Pid"].Value;
      //UltraGridRow rowSelected = null;  
      //rowSelected = ugrdAccoutList.Selected.Rows[0];  
      try
      {
        viewACC_09_002 view = new viewACC_09_002();
        view.pidMainAccount = DBConvert.ParseLong(ugrdAccoutList.ActiveRow.Cells["Pid"].Value);
        Shared.Utility.WindowUtinity.ShowView(view, "Tạo tài khoản mới", false, ViewState.ModalWindow);
        this.LoadTabData();
      }
      catch
      {
        viewACC_09_002 view = new viewACC_09_002();
        Shared.Utility.WindowUtinity.ShowView(view, "Tạo tài khoản mới", false, ViewState.ModalWindow);
        this.LoadTabData();
      }
    }

    private void tsmiProperties_Click(object sender, EventArgs e)
    {
      //UltraGridRow rowSelected = null;
      //rowSelected = ugrdAccoutList.Selected.Rows[0];
      try
      {
        viewACC_09_002 view = new viewACC_09_002();
        view.pidAccount = DBConvert.ParseLong(ugrdAccoutList.ActiveRow.Cells["Pid"].Value);
        Shared.Utility.WindowUtinity.ShowView(view, "Chi tiết tài khoản", false, ViewState.ModalWindow);
        this.LoadTabData();
      }
      catch
      {
        this.LoadTabData();
      }
    }

    private void ugrdAccoutList_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ugrdAccoutList);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      // Set Align
      for (int i = 0; i < ugrdAccoutList.DisplayLayout.Bands[1].Columns.Count; i++)
      {
        Type colType = ugrdAccoutList.DisplayLayout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ugrdAccoutList.DisplayLayout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          ugrdAccoutList.DisplayLayout.Bands[1].Columns[i].Format = "#,##0.##";
        }
      }

      // Set Align
      for (int i = 0; i < ugrdAccoutList.DisplayLayout.Bands[2].Columns.Count; i++)
      {
        Type colType = ugrdAccoutList.DisplayLayout.Bands[2].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ugrdAccoutList.DisplayLayout.Bands[2].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          ugrdAccoutList.DisplayLayout.Bands[2].Columns[i].Format = "#,##0.##";
        }
      }

      // Set Align
      for (int i = 0; i < ugrdAccoutList.DisplayLayout.Bands[3].Columns.Count; i++)
      {
        Type colType = ugrdAccoutList.DisplayLayout.Bands[3].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ugrdAccoutList.DisplayLayout.Bands[3].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          ugrdAccoutList.DisplayLayout.Bands[3].Columns[i].Format = "#,##0.##";
        }
      }

      // Set Align
      for (int i = 0; i < ugrdAccoutList.DisplayLayout.Bands[4].Columns.Count; i++)
      {
        Type colType = ugrdAccoutList.DisplayLayout.Bands[4].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ugrdAccoutList.DisplayLayout.Bands[4].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          ugrdAccoutList.DisplayLayout.Bands[4].Columns[i].Format = "#,##0.##";
        }
      }

      ugrdAccoutList.DisplayLayout.Bands[0].Columns["Type"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[0].Columns["PostingRule"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[1].Columns["Type"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[1].Columns["PostingRule"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[2].Columns["Type"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[2].Columns["PostingRule"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[3].Columns["Type"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[3].Columns["PostingRule"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[4].Columns["Type"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      ugrdAccoutList.DisplayLayout.Bands[4].Columns["PostingRule"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;


      //
      e.Layout.Bands[1].ColHeadersVisible = false;
      e.Layout.Bands[2].ColHeadersVisible = false;
      e.Layout.Bands[3].ColHeadersVisible = false;
      e.Layout.Bands[4].ColHeadersVisible = false;
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["Type"].ValueList = ucbType;
      e.Layout.Bands[0].Columns["PostingRule"].ValueList = ucbType;
      e.Layout.Bands[1].Columns["Type"].ValueList = ucbType;
      e.Layout.Bands[1].Columns["PostingRule"].ValueList = ucbType;
      e.Layout.Bands[2].Columns["Type"].ValueList = ucbType;
      e.Layout.Bands[2].Columns["PostingRule"].ValueList = ucbType;
      e.Layout.Bands[3].Columns["Type"].ValueList = ucbType;
      e.Layout.Bands[3].Columns["PostingRule"].ValueList = ucbType;
      e.Layout.Bands[4].Columns["Type"].ValueList = ucbType;
      e.Layout.Bands[4].Columns["PostingRule"].ValueList = ucbType;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[2].Columns["Pid"].Hidden = true;
      e.Layout.Bands[2].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[3].Columns["Pid"].Hidden = true;
      e.Layout.Bands[3].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[4].Columns["Pid"].Hidden = true;
      e.Layout.Bands[4].Columns["ParentPid"].Hidden = true;

      //Caption
      e.Layout.Bands[0].Columns["AccountCode"].Header.Caption = "Mã tài khoản";
      e.Layout.Bands[0].Columns["AccountName"].Header.Caption = "Tên tài khoản";
      e.Layout.Bands[0].Columns["Type"].Header.Caption = "Tính chất";
      e.Layout.Bands[0].Columns["PostingRule"].Header.Caption = "Các định khoản";
      e.Layout.Bands[0].Columns["IsActive"].Header.Caption = "Hoạt đông";

    }
    #endregion event

    private void utabAccount_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }
  }
}
