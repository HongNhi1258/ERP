using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_10_001 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private DataTable dtSource = new DataTable();
    private int pid = int.MinValue;
    private int whPid = int.MinValue;
    #endregion Field

    #region Init
    public viewWHD_10_001()
    {
      InitializeComponent();
    }

    #endregion Init
    private void viewWHD_10_001_Load(object sender, EventArgs e)
    {
      this.InitData();
      this.LoadData();
    }

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }

    private void SetNeedToSave()
    {
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDWarehouse_InitData");
      Utility.LoadUltraCombo(ucboStockAccount, ds.Tables[1], "Value", "Display", false, new string[] { "Value", "Display" });
      ucboStockAccount.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
      Utility.LoadUltraCombo(ucboType, ds.Tables[0], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucboWarehouse, ds.Tables[2], "Value", "Display", false, new string[] { "Value", "Display" });
      ucboWarehouse.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
    }

    private void SetStatusControl()
    {

    }

    private void PopulateTreeView(DataTable dtParent, int parentId, TreeNode treeNode)
    {
      foreach (DataRow row in dtParent.Rows)
      {
        TreeNode child = new TreeNode
        {
          Text = row["Name"].ToString(),
          Tag = row["Pid"]
        };
        if (treeNode == null)
        {
          treeViewData.Nodes.Add(child);
        }
        else
        {
          treeNode.Nodes.Add(child);
        }
        string cm = string.Format(@"SELECT Pid, Code + ' | ' + Name Name, ParentPid
                              FROM TblWHDWarehouse
                              WHERE ISNULL(ParentPid,0)='{0}' ", child.Tag);
        DataTable dtChild = DataBaseAccess.SearchCommandTextDataTable(cm);
        PopulateTreeView(dtChild, Convert.ToInt32(child.Tag), child);
      }
    }
    private void LoadData()
    {
      treeViewData.Nodes.Clear();
      dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDWarehouse_Select");
      this.PopulateTreeView(dtSource, 0, null);
    }
    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //Check code
      if (txtCode.Text.Length == 0)
      {
        errorMessage = "Code";
        return false;
      }

      if(this.pid != int.MinValue)
      {
        string codeE = string.Empty;
        string code = txtCode.Text.Trim().ToString();
        string cmd = string.Format(@"SELECT Code
	                                      FROM TblWHDWarehouse
                                        WHERE Pid = {0}", this.pid);
        DataTable dtChkCode = DataBaseAccess.SearchCommandTextDataTable(cmd);
        codeE = dtChkCode.Rows[0]["Code"].ToString();
        if(code != codeE)
        {
          cmd = string.Format(@"SELECT Code
	                                      FROM TblWHDWarehouse
                                        WHERE Code = '{0}'", code);
          DataTable dtChkCodeExists = DataBaseAccess.SearchCommandTextDataTable(cmd);
          if (dtChkCodeExists.Rows.Count > 0)
          {
            WindowUtinity.ShowMessageErrorFromText(string.Format("Code {0} has been existed.", code));
            errorMessage = "Code";
            return false;
          }  
        }  
      }
      else
      {
        string cmd = string.Format(@"SELECT Code
	                                      FROM TblWHDWarehouse
                                        WHERE Code = '{0}'", txtCode.Text.Trim().ToString());
        DataTable dtChkCodeExists = DataBaseAccess.SearchCommandTextDataTable(cmd);
        if (dtChkCodeExists.Rows.Count > 0)
        {
          WindowUtinity.ShowMessageErrorFromText(string.Format("Code {0} has been existed.", txtCode.Text.Trim().ToString()));
          errorMessage = "Code";
          return false;
        }
      }



      //Check name
      if (txtName.Text.Length == 0)
      {
        errorMessage = "Name";
        return false;
      }
      //Check type
      if (ucboType.SelectedRow == null)
      {
        errorMessage = "Type";
        return false;
      }

      //Check stock 
      int stockAccountPid = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucboStockAccount));
      if (ucboStockAccount.Text.Length > 0)
      {
        if (stockAccountPid != int.MinValue)
        {
          string cm1 = string.Format(@"SELECT A.Pid
		                                      FROM TblACCAccount A
		                                      LEFT JOIN (
							                                        SELECT DISTINCT  ParentPid
							                                        FROM TblACCAccount 
							                                        ) B ON A.Pid = B.ParentPid 
		                                      WHERE B.ParentPid IS NULL AND A.Pid ='{0}'", stockAccountPid);
          DataTable dtStock = DataBaseAccess.SearchCommandTextDataTable(cm1);
          if (dtStock.Rows.Count <= 0)
          {
            errorMessage = "Stock Account is not exists";
            return false;
          }
        }
        else
        {
          errorMessage = "Stock Account is not exists";
          return false;
        }
      }
      //Check wh Type 
      int whTypePid = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucboType));
      if (ucboType.Text.Length > 0)
      {
        if (whTypePid != int.MinValue)
        {
          string cm2 = string.Format(@"SELECT Code, Name
                                        FROM VWHDWarehouseType
                                         WHERE Code ='{0}'", whTypePid);
          DataTable dttype = DataBaseAccess.SearchCommandTextDataTable(cm2);
          if (dttype.Rows.Count <= 0)
          {
            errorMessage = "WH Type is not exists";
            return false;
          }
        }
        else
        {
          errorMessage = "WH Type is not exists";
          return false;
        }
      }
      //check warehouse
      int warehousePid = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucboWarehouse));
      if (ucboWarehouse.Text.Length > 0)
      {
        if (warehousePid != int.MinValue)
        {
          string cm3 = string.Format(@"SELECT Pid, Name
	                                      FROM TblWHDWarehouse
                                        WHERE Pid ='{0}'", warehousePid);
          DataTable dtwarehouse = DataBaseAccess.SearchCommandTextDataTable(cm3);
          if (dtwarehouse.Rows.Count <= 0)
          {
            errorMessage = "Parent Warehouse is not exists";
            return false;
          }
        }
        else
        {
          errorMessage = "Parent Warehouse is not exists";
          return false;
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      string storeName = "spWHDWarehouse_Edit";
      int paramNumber = 10;
      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (this.pid != int.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int32, this.pid);
      }
      if (txtCode.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@Code", DbType.String, txtCode.Text.ToString());
      }
      inputParam[2] = new DBParameter("@Name", DbType.String, txtName.Text.ToString());
      if (ucboStockAccount.Text.Length > 0)
      {
        inputParam[3] = new DBParameter("@StockAccount", DbType.Int32, DBConvert.ParseInt(ucboStockAccount.Value.ToString()));
      }
      if (ucboType.Text.Length > 0)
      {
        inputParam[4] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(ucboType.Value.ToString()));
      }
      if (ucboWarehouse.Text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Warehouse", DbType.Int32, DBConvert.ParseInt(ucboWarehouse.Value.ToString()));
      }
      if (txtDescription.Text.Length > 0)
      {
        inputParam[6] = new DBParameter("@Description", DbType.String, txtDescription.Text.ToString());
      }
      if (txtSymbol.Text.Length > 0)
      {
        inputParam[7] = new DBParameter("@Symbol", DbType.String, txtSymbol.Text.ToString());
      }
      inputParam[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (chkActive.Checked)
      {
        inputParam[9] = new DBParameter("@IsDelete", DbType.Int32, 0);
      }
      else
      {
        inputParam[9] = new DBParameter("@IsDelete", DbType.Int32, 1);
      }
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        this.pid = DBConvert.ParseInt(outputParam[0].Value.ToString());
        this.LoadData();
        return true;

      }
      return false;
    }


    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        success = this.SaveMain();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.InitData();
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion Function

    #region Event

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ugrdData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      //Utility.ExportToExcelWithDefaultPath(ugdData, "Data");
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      this.pid = int.MinValue;
      txtCode.Text = string.Empty;
      txtName.Text = string.Empty;
      txtDescription.Text = string.Empty;
      txtSymbol.Text = string.Empty;
      if (this.whPid != int.MinValue)
      {
        ucboWarehouse.Value = this.whPid;
      }
      else
      {
        ucboWarehouse.Value = null;
      }
      ucboType.Value = null;
      ucboStockAccount.Value = null;
      chkActive.Checked = true;
    }

    private void treeViewData_AfterSelect(object sender, TreeViewEventArgs e)
    {
      whPid = DBConvert.ParseInt(e.Node.Tag.ToString());
      string cm = string.Format(@"SELECT Pid, Code, Name, WHDesc, Symbol, WHType, StockAccountPid, ParentPid, ISNULL(IsDeleted,0) IsDeleted
	                                FROM TblWHDWarehouse 
	                                WHERE Pid = {0}", whPid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cm);
      if (dtSource.Rows.Count > 0)
      {
        DataRow rowGrid = dtSource.Rows[0];
        this.pid = DBConvert.ParseInt(rowGrid["Pid"].ToString());
        txtCode.Text = rowGrid["Code"].ToString();
        txtName.Text = rowGrid["Name"].ToString();
        txtDescription.Text = rowGrid["WHDesc"].ToString();
        txtSymbol.Text = rowGrid["Symbol"].ToString();
        ucboStockAccount.Value = rowGrid["StockAccountPid"];
        ucboType.Value = rowGrid["WHType"];
        ucboWarehouse.Value = rowGrid["ParentPid"];
        string active = rowGrid["IsDeleted"].ToString();
        //bool active = DBConvert.ParseInt(rowGrid["IsDeleted"].ToString()) == 1 ? true : false;
        if (active == "False")
        {
          chkActive.Checked = true;
        }
        else
        {
          chkActive.Checked = false;
        }
      }
    }
    #endregion Event
  }
}
