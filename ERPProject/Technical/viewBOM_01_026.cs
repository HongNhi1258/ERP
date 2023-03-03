/*
  Author      : NGUYEN HUU DUC
  Date        : 22-11-2022
  Description : TblBOMMaterialRawDimensionFormula
  Standard Form: view_SearchSave.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_026 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewBOM_01_026).Assembly);
    private IList listDeletedMaterial = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Utility.LoadUltraCombo();
      //Utility.LoadUltraDropDown();

      //string cmd = string.Format(@"SELECT Pid, [Group], Category, MaterialCode, RawLength, RawWidth, RawThickness
      //                            FROM TblBOMMaterialRawDimensionFormula");
      //DataTable dataSource = DataBaseAccess.SearchCommandTextDataTable(cmd);
      //ultData.DataSource = dataSource;
      this.LoadGroup();
      this.LoadCategory(ultddCategory);
      this.LoadMaterial(ultddMaterial);
      ultData.BeforeCellActivate += new Infragistics.Win.UltraWinGrid.CancelableCellEventHandler(this.ultData_BeforeCellActivate);
    }

    private void LoadGroup()
    {
      string commandText = @"SELECT [Group], [Group] + ' | ' + [Name] AS [Name]  FROM TblGNRMaterialGroup";
      var dtGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtGroup == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultddGroup, dtGroup, "Group", "Name", true);
      ultddGroup.DropDownWidth = 200;
      ultddGroup.DisplayLayout.Bands[0].Columns["Group"].Width = 80;
      ultddGroup.DisplayLayout.Bands[0].Columns["Name"].Width = 100;
    }
    private UltraCombo LoadCategory(UltraCombo ult, string group = null)
    {
      if(ult == null)
        ult = new UltraCombo();

      string commandText = string.Format(@"SELECT Category, Category + ' | ' + [Name] AS Name 
                              FROM TblGNRMaterialCategory
                              WHERE [Group]={0}", group);
      var dtCategory = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCategory == null || group == null)
      {
        return ult;
      }
      Utility.LoadUltraCombo(ult, dtCategory, "Category", "Name", true);
      ult.DropDownWidth = 300;
      ult.DisplayLayout.Bands[0].Columns["Category"].Width = 150;
      ult.DisplayLayout.Bands[0].Columns["Name"].Width = 150;

      return ult;
    }
    private UltraCombo LoadMaterial(UltraCombo ult, string group = null, string category = null)
    {
      if (ult == null)
        ult = new UltraCombo();

      string commandText = string.Empty;
      commandText += string.Format(@"SELECT MaterialCode, MaterialCode + ' | ' + NameEN MaterialNameVn, NameVN MaterialNameEn, Unit, LeadTime
                                    FROM TblGNRMaterialInformation
                                    WHERE ([Group]='{0}' OR '{0}' = '') AND (Category ='{1}' OR '{1}' = '')", group, category);
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return ult;
      }
      Utility.LoadUltraCombo(ult, dtSource, "MaterialCode", "MaterialNameVn", true);
      ult.DropDownWidth = 600;
      ult.DisplayLayout.Bands[0].Columns["MaterialCode"].Width = 100;
      ult.DisplayLayout.Bands[0].Columns["MaterialNameVn"].Width = 250;
      ult.DisplayLayout.Bands[0].Columns["MaterialNameEn"].Width = 250;
      ult.DisplayLayout.Bands[0].Columns["Unit"].Width = 50;
      ult.DisplayLayout.Bands[0].Columns["LeadTime"].Width = 50;
      return ult;
    }

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

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      int paramNumber = 1;
      string storeName = "spBOMMaterialRawDimensionFormula_Select";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@MaterialCode", DbType.String, txtMaterialCode.Text.Trim());

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultData.DataSource = dtSource;

      lblCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
      LoadData();
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

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

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //if (ultddGroup.SelectedRow == "" && ultddMaterial.SelectedText =="")
      //{
      //  errorMessage = "Nhóm hoặc mã sản phẩm";
      //  return false;
      //}
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        int index = -1;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedMaterial.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.String, listDeletedMaterial[i]) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMMaterialRawDimensionFormula_Delete", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ultData.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          index++;
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            string group = row["Group"].ToString();
            string category = row["Category"].ToString();
            string material = row["MaterialCode"].ToString();
            double rawLength = DBConvert.ParseDouble(row["RawLength"].ToString());
            double rawWidth = DBConvert.ParseDouble(row["RawWidth"].ToString());
            double rawThickness = DBConvert.ParseDouble(row["RawThickness"].ToString());
            if (string.IsNullOrEmpty(group) && string.IsNullOrEmpty(material))
            {
              WindowUtinity.ShowMessageError("ERR0343");
              return;
            }
            DBParameter[] inputParam = new DBParameter[8];
            if(group != "")
            {
              inputParam[0] = new DBParameter("@Group", DbType.String, group);
            }  
            if (category != "")
            {
              inputParam[1] = new DBParameter("@Category", DbType.String, category);
            }  
            if (material != "")
            {
              inputParam[2] = new DBParameter("@MaterialCode", DbType.String, material);
            }  
            if (rawLength > 0)
            {
              inputParam[3] = new DBParameter("@RawLength", DbType.Double, rawLength);
            }  
            else if (rawLength < 0 && rawLength != double.MinValue)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Chiều dài thô");
              ultData.Rows[index].Cells["RawLength"].Selected = true;
              return;
            }
            if (rawWidth > 0)
            {
              inputParam[4] = new DBParameter("@RawWidth", DbType.Double, rawWidth);
            }  
            else if (rawWidth < 0 && rawWidth != double.MinValue)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Chiều rộng thô");
              ultData.Rows[index].Cells["RawWidth"].Selected = true;
              return;
            }
            if (rawThickness > 0)
            {
              inputParam[5] = new DBParameter("@RawThickness", DbType.Double, rawThickness);
            }  
            else if(rawThickness < 0 && rawThickness != double.MinValue)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Độ dày thô");
              ultData.Rows[index].Cells["RawThickness"].Selected = true;
              return;
            }  

            if (row.RowState == DataRowState.Modified) // Update
            {
              long pid = DBConvert.ParseLong(row["Pid"].ToString());
              inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              inputParam[7] = new DBParameter("@Pid", DbType.Int64, pid);
              DataBaseAccess.ExecuteStoreProcedure("spBOMMaterialRawDimensionFormula_Update", inputParam, outputParam);
            }
            else if (row.RowState == DataRowState.Added)
            {
              inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              DataBaseAccess.ExecuteStoreProcedure("spBOMMaterialRawDimensionFormula_Insert", inputParam, outputParam);
            }

            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.SearchData();
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
      //lblCount.Text = rm.GetString("Count", ConstantClass.CULTURE) + ":";
      //btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);      
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewBOM_01_026()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_01_026_Load(object sender, EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(gpbSearch);
      //Init Data
      this.InitData();
      this.LoadData();
      // Set Language
      this.SetLanguage();
    }

    private void LoadData()
    {
      foreach (UltraGridRow row in ultData.Rows)
      {
        UltraGridCell cell = row.Cells["Category"];
        CancelableCellEventArgs e = new CancelableCellEventArgs(cell);
        object sender = new object();
        this.ultData_BeforeCellActivate(sender, e);

      }
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
      this.ConfirmToCloseTab();
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

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Group"].Header.Caption = "Nhóm sản phẩm\n(*)";
      e.Layout.Bands[0].Columns["Category"].Header.Caption = "Loại sản phẩm";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[0].Columns["RawLength"].Header.Caption = "Chiều dài thô";
      e.Layout.Bands[0].Columns["RawWidth"].Header.Caption = "Chiều rộng thô";
      e.Layout.Bands[0].Columns["RawThickness"].Header.Caption = "Độ dày thô";

      e.Layout.Bands[0].Columns["Group"].Width = 150;
      e.Layout.Bands[0].Columns["Category"].Width = 150;
      e.Layout.Bands[0].Columns["MaterialCode"].Width = 300;
      e.Layout.Bands[0].Columns["RawLength"].Width = 100;
      e.Layout.Bands[0].Columns["RawWidth"].Width = 100;
      e.Layout.Bands[0].Columns["RawThickness"].Width = 100;

      e.Layout.Bands[0].Columns["Group"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["Category"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["RawLength"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["RawWidth"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["RawThickness"].CellAppearance.TextHAlign = HAlign.Center;

      e.Layout.Bands[0].Columns["Group"].ValueList = ultddGroup;
      e.Layout.Bands[0].Columns["Category"].ValueList = ultddCategory;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ultddMaterial;


      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }
    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      if (row.ParentRow == null)
      {
        string group = row.Cells["Group"].Value.ToString();
        string category = row.Cells["Category"].Value.ToString();
        switch (columnName.ToLower())
        {
          case "category":
            UltraCombo ultc = (UltraCombo)e.Cell.Row.Cells["Category"].ValueList;
            row.Cells["Category"].ValueList = LoadCategory(ultc, group);
            break;
          case "materialcode":
            UltraCombo ultcMaterial = (UltraCombo)e.Cell.Row.Cells["MaterialCode"].ValueList;
            row.Cells["MaterialCode"].ValueList = LoadMaterial(ultcMaterial, group, category);
            break;
          default:
            break;
        }
      }
    }
    private void ultData_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
        long pid = DBConvert.ParseLong( row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedMaterial.Add(pid);
        }
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string group = row.Cells["Group"].Value.ToString();
      string category = row.Cells["Category"].Value.ToString();
      switch (colName.ToLower())
      {
        case "group":
          row.Cells["Category"].Value = null;
          row.Cells["MaterialCode"].Value = null;
          break;
        case "category":
          UltraCombo ultc = (UltraCombo)e.Cell.Row.Cells["Category"].ValueList;
          row.Cells["Category"].ValueList = LoadCategory(ultc, group);
          row.Cells["MaterialCode"].Value = null;
          break;
        case "materialcode":
          UltraCombo ultcMaterial = (UltraCombo)e.Cell.Row.Cells["MaterialCode"].ValueList;
          row.Cells["MaterialCode"].ValueList = LoadMaterial(ultcMaterial, group, category);
          break;
        default:
          break;
      }


      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ultData);
      }
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultData.Selected.Rows.Count > 0 || ultData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultData, new Point(e.X, e.Y));
        }
      }
    }
    #endregion event


  }
}
