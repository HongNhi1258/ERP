/*
  Author      : Nguyen Ngoc Tien
  Date        : 21-Mar-2016
  Description : Compare Seri Box Of Container With Box Scan
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
using System.IO;
using System.Data.SqlClient;
using System.Collections;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_03_011 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadContainer();
    }
    private void LoadContainer()
    {
      string cm = string.Format(@"SELECT Pid, ContainerNo 
                                  FROM TblPLNSHPContainer
                                  WHERE Confirm <> 3
                                  ORDER BY Pid DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultContainer, dt, "Pid", "ContainerNo", "Pid");
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 1;
      string storeName = "spFGWInfoBoxOfContainer";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@ContainerPid", DbType.Int64, DBConvert.ParseLong(ultContainer.Value.ToString()));

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      ultraGridInformation.DataSource = dsSource.Tables[0];
      ultGridData.DataSource = dsSource.Tables[1];
      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void LoadDataScan()
    {
      if (this.ultContainer.Value == null || this.ultContainer.Value.ToString().Length == 0 || ultraGridInformation.Rows.Count == 0)
      {
        string message = "Please Search Data First!";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a;
      DriveInfo[] allDrives = DriveInfo.GetDrives();
      string stringName = string.Empty;
      string path = "PhanmemDENSOBHT8000";
      int flagDrive = 0;
      foreach (DriveInfo d in allDrives)
      {
        stringName = d.Name;
        path = stringName + path;
        if (Directory.Exists(path))
        {
          flagDrive = 1;
          break;
        }
        path = "PhanmemDENSOBHT8000";
        stringName = string.Empty;
      }

      if (flagDrive == 0)
      {
        return;
      }

      //// Get path from Folder
      //string path = @"\PhanmemDENSOBHT8000";
      //path = Path.GetFullPath(path);
      string pathBarCode = path + @"\THONGTIN.txt";
      try
      {
        a = File.ReadAllLines(pathBarCode);
      }
      catch
      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (a.Length == 0)
      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      if (index != -1)
      {
        for (int i = 0; i < a.Length; i++)
        {
          if (a[i].Trim().ToString() != string.Empty)
          {
            index = a[i].IndexOf("*");
            a[i] = a[i].Substring(0, index).Trim().ToString();
          }
        }
      }

      DataTable dtBoxScan = new DataTable();
      dtBoxScan.Columns.Add("BoxScan", typeof(System.String));

      int k = 0;
      for (int i = 0; i < a.Length; i++)
      {
        //check duplicate
        k = 0;
        for (int j = i + 1; j < a.Length; j++)
        {
          if (a[i].ToString() == a[j].ToString())
          {
            k++;
          }
        }
        if (k > 0)
        {
          continue;
        }
        DataRow dr = dtBoxScan.NewRow();
        dr["BoxScan"] = a[i].ToString().Trim();
        dtBoxScan.Rows.Add(dr);
      }

      DataTable dtSeriBox = (DataTable)ultraGridInformation.DataSource;
      SqlDBParameter[] input = new SqlDBParameter[2];
      input[0] = new SqlDBParameter("@SeriBox", SqlDbType.Structured, dtSeriBox);
      input[1] = new SqlDBParameter("@BoxScan", SqlDbType.Structured, dtBoxScan);
      DataTable dtAfterCompare = SqlDataBaseAccess.SearchStoreProcedureDataTable("spFGWCompareSeriBoxWithBoxScan", input);
      ultraGridInformation.DataSource = dtAfterCompare;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Flag"].Value.ToString()) == 0)
        {
          ultraGridInformation.Rows[i].Cells["SeriBoxNo"].Appearance.BackColor = Color.Yellow;
          ultraGridInformation.Rows[i].Cells["BoxScanned"].Appearance.BackColor = Color.Yellow;
        }
        else
        {
          ultraGridInformation.Rows[i].Cells["SeriBoxNo"].Appearance.BackColor = Color.Transparent;
          ultraGridInformation.Rows[i].Cells["BoxScanned"].Appearance.BackColor = Color.Transparent;
        }
      }
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
    #endregion function

    #region event
    public viewFGH_03_011()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewFGH_03_011_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (ultContainer.SelectedRow != null)
      {
        this.SearchData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0115", "Container");
      }
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      this.LoadDataScan();
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
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

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
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      
      // Hide column
      e.Layout.Bands[0].Columns["ContainerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["BOXPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns["BoxTypeCode"].Header.Caption = "Box Type";
      e.Layout.Bands[0].Columns["QtyFurniturePerBox"].Header.Caption = "Qty Furniture \n Per Box";
      
      // Set dropdownlist for column
      //e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      //e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Wo"].MinWidth = 70;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 70;
      e.Layout.Bands[0].Columns["ItemGroup"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemGroup"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["BoxTypeCode"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["BoxTypeCode"].MinWidth = 200;
      e.Layout.Bands[0].Columns["BoxTypeName"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["BoxTypeName"].MinWidth = 100;
      e.Layout.Bands[0].Columns["QtyFurniturePerBox"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["QtyFurniturePerBox"].MinWidth = 150;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 100;

      // Set Column Style
      //e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      //ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      //ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      //e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      //e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      //ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      //ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      //e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      //e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
      ControlUtility.ExportToExcelWithDefaultPath(ultGridData, "Data Scaned");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
        }
      }
    }
    #endregion event

  }
}
