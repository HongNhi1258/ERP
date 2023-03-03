/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewBOM_50_001.cs
*/
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_50_001 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Load data for combobox WO
      string commandText = "Select Pid From TblPLNWorkOrder ORDER BY Pid DESC";
      DataTable dtWO = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO != null)
      {
        Utility.LoadUltraCombo(ultraCBWO, dtWO, "Pid", "Pid");
        ultraCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      string item = string.Format("%{0}%", txtItem.Text.Trim());
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@Item", DbType.AnsiString, 512, item);
      if (ultraCBWO.SelectedRow != null)
      {
        inputParam[1] = new DBParameter("@WO", DbType.Int64, ultraCBWO.Value);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMCarcassListForGetOldBOM", inputParam);
      ugdInformation.DataSource = dtSource;

      lblCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtItem.Text = string.Empty;
      ultraCBWO.Value = null;
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
      DataTable dtDetail = (DataTable)ugdInformation.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Modified && DBConvert.ParseInt(row["Select"]) == 1)
        {
          string carcassCode = row["CarcassCode"].ToString();
          string commandText = string.Format(@"SELECT COUNT(*) CompCount FROM TblBOMCarcassComponent WHERE CarcassCode = '{0}'", carcassCode);
          DataTable dtCompCount = DataBaseAccess.SearchCommandTextDataTable(commandText);
          int compCount = DBConvert.ParseInt(dtCompCount.Rows[0]["CompCount"]);
          if (compCount > 0)
          {
            errorMessage = string.Format("Error! Please delete current BOM of carcass {0}", carcassCode);
            return false;
          }
        }
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        string oldItemCodes = string.Empty;
        bool success = true;

        //1. Get Old Item List
        DataTable dtDetail = (DataTable)ugdInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Modified && DBConvert.ParseInt(row["Select"]) == 1)
          {
            string oldCode = row["OldCode"].ToString();
            if (oldItemCodes.Length > 0 && oldCode.Length > 0)
            {
              oldItemCodes += ",";
            }
            oldItemCodes += (oldCode);
          }
        }

        //2.1. Delete Data Carcass Component struct & material
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@OldItemCodes", DbType.AnsiString, 1024, oldItemCodes) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponentForUpload_Delete", inputDelete, outputDelete);
        if ((outputDelete == null) || (outputDelete[0].Value.ToString() == "0"))
        {
          success = false;
        }

        //2.2. Insert Data Into TblBOMCarcassComponentForUpload
        if (oldItemCodes.Length > 0)
        {
          DataSet dsOldBOM = this.GetOldBOM(oldItemCodes);

          // Carcass Component Struct
          DataTable dtOldBOMStruct = dsOldBOM.Tables["Struct"];
          foreach (DataRow row in dtOldBOMStruct.Rows)
          {
            DBParameter[] inputParam = new DBParameter[11];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 32, row["ItemCode"]);
            inputParam[1] = new DBParameter("@KeyCode", DbType.AnsiString, 32, row["KeyCode"]);
            inputParam[2] = new DBParameter("@CompCode", DbType.AnsiString, 32, row["CompCode"]);
            inputParam[3] = new DBParameter("@CompName", DbType.String, 512, row["CompName"]);
            inputParam[4] = new DBParameter("@ParentCompCode", DbType.AnsiString, 32, row["ParentCompCode"]);
            inputParam[5] = new DBParameter("@Qty", DbType.Double, row["Qty"]);
            inputParam[6] = new DBParameter("@Length", DbType.Double, row["Length"]);
            inputParam[7] = new DBParameter("@Width", DbType.Double, row["Width"]);
            inputParam[8] = new DBParameter("@Thickness", DbType.Double, row["Thickness"]);
            inputParam[9] = new DBParameter("@Remark", DbType.String, 512, row["Remark"]);

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponentForUpload_Insert", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }

          // Carcass Component Material
          DataTable dtOldBOMMaterial = dsOldBOM.Tables["Material"];
          foreach (DataRow row in dtOldBOMMaterial.Rows)
          {
            DBParameter[] inputParam = new DBParameter[11];
            inputParam[0] = new DBParameter("@KeyCode", DbType.AnsiString, 32, row["KeyCode"]);
            inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 32, row["MaterialCode"]);
            inputParam[2] = new DBParameter("@Length", DbType.Double, row["Length"]);
            inputParam[3] = new DBParameter("@Width", DbType.Double, row["Width"]);
            inputParam[4] = new DBParameter("@Thickness", DbType.Double, row["Thickness"]);
            inputParam[5] = new DBParameter("@Qty", DbType.Double, row["Qty"]);

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponentMaterialForUpload_Insert", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }

        //3. Upload data for new BOM
        if (success)
        {
          DataTable dtOldItemCode = (DataTable)ugdInformation.DataSource;
          foreach (DataRow rowOld in dtOldItemCode.Rows)
          {
            if (rowOld.RowState == DataRowState.Modified && DBConvert.ParseInt(rowOld["Select"]) == 1)
            {
              string oldCode = rowOld["OldCode"].ToString();
              DBParameter[] inputParamUpload = new DBParameter[2];
              inputParamUpload[0] = new DBParameter("@OldItemCode", DbType.AnsiString, 32, oldCode);
              inputParamUpload[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              DBParameter[] outputParamUpload = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spBOMUploadFromOldToNewBOM", inputParamUpload, outputParamUpload);
              if ((outputParamUpload == null) || (outputParamUpload[0].Value.ToString() == "0"))
              {
                success = false;
              }
            }
          }
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
            this.SearchData();
          }
          else
          {
            WindowUtinity.ShowMessageError("WRN0004");
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", errorMessage);
          this.SaveSuccess = false;
        }
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText(errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    public async Task<BYSCarcassCompStruct[]> RunBYSCarcassCompStructAsync(string oldItemCode)
    {
      BYSCarcassCompStruct[] carcassCompStruct = new BYSCarcassCompStruct[0];
      try
      {
        string url = string.Format("{0}{1}", Utility.GetImagePathByPid(26), oldItemCode);

        // Get the product        
        var result = string.Empty;
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
          result = await response.Content.ReadAsStringAsync();

          carcassCompStruct = (JsonConvert.DeserializeObject<BYSCarcassCompStructResult>(result, new JsonSerializerSettings())).result;
        }
      }
      catch (Exception e)
      {
        WindowUtinity.ShowMessageErrorFromText(e.Message);
      }
      return carcassCompStruct;
    }

    public async Task<BYSCarcassCompMaterial[]> RunBYSCarcassCompMaterialAsync(string oldItemCode)
    {
      BYSCarcassCompMaterial[] carcassCompMaterial = new BYSCarcassCompMaterial[0];
      try
      {
        string url = string.Format("{0}{1}", Utility.GetImagePathByPid(27), oldItemCode);

        // Get the product        
        var result = string.Empty;
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
          result = await response.Content.ReadAsStringAsync();

          carcassCompMaterial = (JsonConvert.DeserializeObject<BYSCarcassCompMaterialResult>(result, new JsonSerializerSettings())).result;
        }
      }
      catch (Exception e)
      {
        WindowUtinity.ShowMessageErrorFromText(e.Message);
      }
      return carcassCompMaterial;
    }

    private DataSet GetOldBOM(string oldItemCodes)
    {
      DataSet dsBOMCarcass = new DataSet();
      DataTable dtBYSCarcassCompStruct = new DataTable("Struct");
      DataTable dtBYSCarcassCompMaterial = new DataTable("Material");
      dsBOMCarcass.Tables.Add(dtBYSCarcassCompStruct);
      dsBOMCarcass.Tables.Add(dtBYSCarcassCompMaterial);

      dtBYSCarcassCompStruct.Columns.AddRange(new DataColumn[] { new DataColumn("ItemCode", typeof(string)), new DataColumn("KeyCode", typeof(string)),
          new DataColumn("CompCode", typeof(string)), new DataColumn("CompName", typeof(string)), new DataColumn("ParentCompCode", typeof(string)),
          new DataColumn("Qty", typeof(float)), new DataColumn("Length", typeof(float)), new DataColumn("Width", typeof(float)),
          new DataColumn("Thickness", typeof(float)), new DataColumn("Remark", typeof(string))});
      dtBYSCarcassCompMaterial.Columns.AddRange(new DataColumn[] { new DataColumn("KeyCode", typeof(string)), new DataColumn("MaterialCode", typeof(string)),
          new DataColumn("Length", typeof(float)), new DataColumn("Width", typeof(float)), new DataColumn("Thickness", typeof(float)),
          new DataColumn("Qty", typeof(float))});

      string[] arrOldItemCode = oldItemCodes.Split(',');
      for (int i = 0; i < arrOldItemCode.Length; i++)
      {
        string oldItemCode = arrOldItemCode[i];

        // Carcass Component Struct
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ProductNo", DbType.AnsiString, 50, oldItemCode) };
        DataTable dtCompStruct = BYSDataBaseAccess.SearchStoreProcedureDataTable("GetCompStructByBOM", inputParam);
        foreach (DataRow compRow in dtCompStruct.Rows)
        {
          DataRow row = dtBYSCarcassCompStruct.NewRow();
          row["ItemCode"] = compRow["ItemCode"];
          row["KeyCode"] = compRow["KeyCode"];
          row["CompCode"] = compRow["CompCode"];
          row["CompName"] = compRow["CompName"];
          row["ParentCompCode"] = compRow["ParentCompCode"];
          row["Qty"] = compRow["Qty"];
          row["Length"] = compRow["Length"];
          row["Width"] = compRow["Width"];
          row["Thickness"] = compRow["Thickness"];
          row["Remark"] = compRow["Remark"];
          dtBYSCarcassCompStruct.Rows.Add(row);
        }

        // Carcass Component Material
        DataTable dtCompMaterial = BYSDataBaseAccess.SearchStoreProcedureDataTable("GetMaterialByBOM", inputParam);
        foreach (DataRow materialRow in dtCompMaterial.Rows)
        {
          DataRow row = dtBYSCarcassCompMaterial.NewRow();
          row["KeyCode"] = materialRow["KeyCode"];
          row["MaterialCode"] = materialRow["MaterialCode"];
          row["Length"] = materialRow["Length"];
          row["Width"] = materialRow["Width"];
          row["Thickness"] = materialRow["Thickness"];
          row["Qty"] = materialRow["Qty"];
          dtBYSCarcassCompMaterial.Rows.Add(row);
        }

        //// Carcass Component Struct
        //BYSCarcassCompStruct[] compStruct = Task.Run(async () => await RunBYSCarcassCompStructAsync(oldItemCode)).GetAwaiter().GetResult();
        //foreach (var comp in compStruct)
        //{
        //  DataRow row = dtBYSCarcassCompStruct.NewRow();
        //  row["ItemCode"] = comp.ItemCode;
        //  row["KeyCode"] = comp.KeyCode;
        //  row["CompCode"] = comp.CompCode;
        //  row["CompName"] = comp.CompName;
        //  row["ParentCompCode"] = comp.ParentCompCode;
        //  row["Qty"] = comp.Qty;
        //  row["Length"] = comp.Length;
        //  row["Width"] = comp.Width;
        //  row["Thickness"] = comp.Thickness;
        //  row["Remark"] = comp.Remark;
        //  dtBYSCarcassCompStruct.Rows.Add(row);
        //}

        //// Carcass Component Material
        //BYSCarcassCompMaterial[] compMaterial = Task.Run(async () => await RunBYSCarcassCompMaterialAsync(oldItemCode)).GetAwaiter().GetResult();
        //foreach (var material in compMaterial)
        //{
        //  DataRow row = dtBYSCarcassCompMaterial.NewRow();          
        //  row["KeyCode"] = material.KeyCode;
        //  row["MaterialCode"] = material.MaterialCode;                    
        //  row["Length"] = material.Length;
        //  row["Width"] = material.Width;
        //  row["Thickness"] = material.Thickness;
        //  row["Qty"] = material.Qty;
        //  dtBYSCarcassCompMaterial.Rows.Add(row);
        //}
      }

      return dsBOMCarcass;
    }
    #endregion function

    #region event
    public viewBOM_50_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_50_001_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(gpbSearch);

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
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {

    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
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

      // Set Width
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["OldCode"].MaxWidth = 130;
      e.Layout.Bands[0].Columns["OldCode"].MinWidth = 130;
      e.Layout.Bands[0].Columns["Select"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Select"].MinWidth = 70;

      // Set caption column
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Item Name";

      // Set Column Style
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            
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

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
        }
      }
    }
    #endregion event
  }
}
