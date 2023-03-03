/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: view_SearchInfo.cs
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.Shared.Utility;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_08_002 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
      this.LoadCbStatus();
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

    private void LoadCbStatus()
    {
      DataTable dtSourceReport = new DataTable();
      dtSourceReport.Columns.Add("value", typeof(System.Int32));
      dtSourceReport.Columns.Add("text", typeof(System.String));

      DataRow row1 = dtSourceReport.NewRow();
      row1["value"] = 0;
      row1["text"] = "Not Confirmed";
      dtSourceReport.Rows.Add(row1);

      DataRow row2 = dtSourceReport.NewRow();
      row2["Value"] = 1;
      row2["Text"] = "Confirmed";
      dtSourceReport.Rows.Add(row2);
      Utility.LoadUltraCombo(ultcbStatus, dtSourceReport, "value", "text", false, "value");
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 4;
      string storeName = "spBOMColor_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];

      if (txtColor.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@ColorCode", DbType.String, txtColor.Text.Trim());
      }

      if (txtName.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@Name", DbType.String, txtName.Text.Trim());
      }

      if (txtMaterial.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@MaterialCode", DbType.String, txtMaterial.Text.Trim());
      }

      if (ultcbStatus.Value != null)
      {
        inputParam[3] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultcbStatus.Value.ToString()));
      }

      DataSet dtSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      dtSource.Relations.Add(new DataRelation("Parent_Child", dtSource.Tables[0].Columns["Pid"], dtSource.Tables[1].Columns["ColorPid"], false));
      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Tables[0].Rows.Count : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtColor.Text = "";
      txtMaterial.Text = "";
      txtName.Text = "";
      txtColor.Focus();
    }
    #endregion function

    #region event
    public viewBOM_08_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_08_002_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      //foreach (Control ctr in groupBoxSearch.Controls)
      //{
      //  ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      //}
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);
      //Init Data
      this.InitData();
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

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Selected"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      }
      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        Type colType = e.Layout.Bands[1].Columns[j].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[1].Columns[j].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ColorPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Remark"].Hidden = true;
      e.Layout.Bands[1].Columns["Consumption"].Hidden = true;
      e.Layout.Bands[1].Columns["MaterialRemark"].Hidden = true;

      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["MaterialNameEn"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["MaterialRemark"].Header.Caption = "Material Remark";
      e.Layout.Bands[1].Columns["Consumption"].Header.Caption = "Estimated Consumption (g/m2)";

      e.Layout.Bands[0].Columns["ColorCode"].Header.Caption = "Chemical Code";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["Confirm"].Header.Caption = "Status";

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;


      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewBOM_08_001 view = new viewBOM_08_001();
      WindowUtinity.ShowView(view, "New Chemical", true, ViewState.MainWindow);
    }

    private void ultraGridInformation_DoubleClick(object sender, EventArgs e)
    {
      if (ultraGridInformation.Selected.Rows.Count > 0 && ultraGridInformation.Selected.Rows[0].Band.ParentBand == null)
      {
        long pid = DBConvert.ParseLong(ultraGridInformation.Selected.Rows[0].Cells["Pid"].Value.ToString());
        viewBOM_08_001 view = new viewBOM_08_001();
        view.pid = pid;
        WindowUtinity.ShowView(view, "Chemical Detail", true, ViewState.MainWindow);
      }
    }

    private void btnUnlock_Click(object sender, EventArgs e)
    {
      int count = ultraGridInformation.Rows.Count;
      int result = int.MinValue;
      bool selected = false;
      for (int i = 0; i < count; i++)
      {
        int check = int.MinValue;
        try
        {
          check = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Selected"].Value.ToString());
        }
        catch { }
        if (check == 1)
        {
          long colorPid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["Pid"].Value.ToString());
          DBParameter[] input = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, colorPid) };
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMFinishingColor_Unlock", input, outputParam);
          result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          selected = (result == 1) ? true : false;
        }
      }
      if (!selected)
      {
        WindowUtinity.ShowMessageWarning("WRN0014");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0023");
      }
      this.SearchData();
    }

    #endregion event


  }
}
