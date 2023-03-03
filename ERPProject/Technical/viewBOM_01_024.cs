/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewBOM_01_024
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Technical.DataSetSource;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class
    viewBOM_01_024 : MainUserControl
  {
    #region Field
    private bool confirmed = false;
    public string carcassCode = string.Empty;
    public long viewPid = long.MinValue;
    #endregion Field

    #region Init
    public viewBOM_01_024()
    {
      InitializeComponent();
    }

    private void viewBOM_01_024_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(groupBoxMaster);
      this.LoadData();
    }

    private void LoadDD()
    {
      Utility.LoadUltraComboFormula(ucbListPaintA);
      ucbListPaintA.DisplayLayout.Bands[0].Columns["FaceQty"].Hidden = true;
      ucbListPaintA.DisplayLayout.Bands[0].Columns["ShortEdgeQty"].Hidden = true;
      ucbListPaintA.DisplayLayout.Bands[0].Columns["LongEdgeQty"].Hidden = true;

      Utility.LoadUltraComboFormula(ucbListPaintB);
      ucbListPaintB.DisplayLayout.Bands[0].Columns["FaceQty"].Hidden = true;
      ucbListPaintB.DisplayLayout.Bands[0].Columns["ShortEdgeQty"].Hidden = true;
      ucbListPaintB.DisplayLayout.Bands[0].Columns["LongEdgeQty"].Hidden = true;

      Utility.LoadUltraComboFormula(ucbListPaintC);
      ucbListPaintC.DisplayLayout.Bands[0].Columns["FaceQty"].Hidden = true;
      ucbListPaintC.DisplayLayout.Bands[0].Columns["ShortEdgeQty"].Hidden = true;
      ucbListPaintC.DisplayLayout.Bands[0].Columns["LongEdgeQty"].Hidden = true;
    }

    #endregion Init

    #region Function

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

    private void SetStatusControl()
    {
      chkConfirm.Checked = this.confirmed;
      btnSave.Enabled = !this.confirmed;
      chkConfirm.Enabled = !this.confirmed;
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

    private void LoadDataComponentList()
    {
      //dsBOMCarcassCompMultiLevel dsComponent = new dsBOMCarcassCompMultiLevel();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListCarcassCompFinishInfo", inputParam);
      if (dsSource != null)
      {
        dsSource.Relations.Add(dsSource.Tables[0].Columns["Pid"], dsSource.Tables[1].Columns["ComponentPid"]);
        //dsComponent.Tables["ComponentInfo"].Merge(dsSource.Tables[0]);
        //dsComponent.Tables["ComponentMaterial"].Merge(dsSource.Tables[1]);
      }
      ultData.DataSource = dsSource;
    }

    private void LoadData()
    {
      this.LoadDataCarcassInfo();
      this.LoadDataComponentList();
      this.LoadDD();
      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private DBParameter[] SetCarcassComponentParam(DBParameter[] param, UltraGridRow row)
    {
      param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
      string text = row.Cells["ComponentCode"].Value.ToString().Trim();
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, text);
      }
      text = row.Cells["DescriptionVN"].Value.ToString().Trim();
      param[3] = new DBParameter("@DescriptionVN", DbType.String, 256, text);
      double waste = DBConvert.ParseDouble(row.Cells["Waste"].Value.ToString());
      if (waste != double.MinValue)
      {
        param[4] = new DBParameter("@Waste", DbType.Double, waste);
      }
      double dimesion = DBConvert.ParseDouble(row.Cells["FIN_Length"].Value.ToString());
      if (dimesion != double.MinValue)
      {
        param[5] = new DBParameter("@Length", DbType.Double, dimesion);
      }
      dimesion = DBConvert.ParseDouble(row.Cells["FIN_Width"].Value.ToString());
      if (dimesion != double.MinValue)
      {
        param[6] = new DBParameter("@Width", DbType.Double, dimesion);
      }
      dimesion = DBConvert.ParseDouble(row.Cells["FIN_Thickness"].Value.ToString());
      if (dimesion != double.MinValue)
      {
        param[7] = new DBParameter("@Thickness", DbType.Double, dimesion);
      }
      int value = DBConvert.ParseInt(row.Cells["Lamination"].Value.ToString());
      if (value != int.MinValue)
      {
        param[8] = new DBParameter("@Lamination", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["FingerJoin"].Value.ToString());
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@FingerJoin", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["Specify"].Value.ToString());
      if (value != int.MinValue)
      {
        param[10] = new DBParameter("@Specify", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["Status"].Value.ToString());
      if (value != int.MinValue)
      {
        param[11] = new DBParameter("@Status", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["ContractOut"].Value.ToString());
      if (value != int.MinValue)
      {
        param[12] = new DBParameter("@ContractOut", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["No"].Value.ToString());
      if (value != int.MinValue)
      {
        param[13] = new DBParameter("@No", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["Primary"].Value.ToString());
      if (value != int.MinValue)
      {
        param[14] = new DBParameter("@Primary", DbType.Int32, value);
      }
      int isMainComp = DBConvert.ParseInt(row.Cells["IsMainComp"].Value.ToString());
      isMainComp = (isMainComp == 1 ? isMainComp : 0);
      param[15] = new DBParameter("@IsMainComp", DbType.Int32, isMainComp);
      if (isMainComp == 1)
      {
        value = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
        if (value != int.MinValue)
        {
          param[16] = new DBParameter("@Qty", DbType.Int32, value);
        }
      }
      int isCompStore = DBConvert.ParseInt(row.Cells["isCompStore"].Value.ToString());
      isCompStore = (isCompStore == 1 ? isCompStore : 0);
      param[18] = new DBParameter("@isCompStore", DbType.Int32, isCompStore);
      int value1 = DBConvert.ParseInt(row.Cells["CriticalComponent"].Value.ToString());
      if (value1 != int.MinValue)
      {
        param[19] = new DBParameter("@Critical", DbType.Int32, value1);
      }
      return param;
    }

    private void LoadDataCarcassInfo()
    {
      string commandText = string.Format("Select CarcassCode, [Description], DescriptionVN, TechConfirm Confirm FROM TblBOMCarcass WHERE CarcassCode = '{0}'", this.carcassCode);
      DataTable dtCarcassInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCarcassInfo != null && dtCarcassInfo.Rows.Count > 0)
      {
        DataRow row = dtCarcassInfo.Rows[0];
        txtCarcassCode.Text = this.carcassCode;
        picCarcass.ImageLocation = FunctionUtility.BOMGetCarcassImage(this.carcassCode);        
        txtVNDescription.Text = row["DescriptionVN"].ToString();
        this.confirmed = (DBConvert.ParseInt(row["Confirm"].ToString()) == 1);
      }
    }

    private bool SaveDetail()
    {
      bool success = true;
      // 2. Update      
      //DataTable dtDetail = ((DataSet)ultData.DataSource).Tables[0];
      DataSet dtDetail = (DataSet)ultData.DataSource;
      foreach (DataRow row in dtDetail.Tables[0].Rows)
      {
        if (row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[7];
          long Pid = DBConvert.ParseLong(row["Pid"].ToString());
          int PaintA = DBConvert.ParseInt(row["APaintFormulaPid"].ToString());
          int PaintB = DBConvert.ParseInt(row["BPaintFormulaPid"].ToString());
          int PaintC = DBConvert.ParseInt(row["CPaintFormulaPid"].ToString());

          inputParam[0] = new DBParameter("@Pid", DbType.Int64, Pid);
          if (PaintA >= 0)
          {
            inputParam[1] = new DBParameter("@APaintFormulaPid", DbType.Int64, PaintA);
          }
          if (PaintB >= 0)
          {
            inputParam[2] = new DBParameter("@BPaintFormulaPid", DbType.Int64, PaintB);
          }
          if (PaintC >= 0)
          {
            inputParam[3] = new DBParameter("@CPaintFormulaPid", DbType.Int64, PaintC);
          }

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponent_UpdateFinishing", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool CheckValid()
    {
      return true;
    }

    private void SaveData()
    {
      bool success = true;
      if (this.CheckValid())
      {
        if (this.SaveDetail())
        {

          string componentNotUsed = this.ConfirmToCarcass();
          if (componentNotUsed.Length > 0)
          {
            success = false;
            this.SaveSuccess = false;
            WindowUtinity.ShowMessageError("ERR0074", componentNotUsed);
          }
          else
          {
            if (success)
            {
              WindowUtinity.ShowMessageSuccess("MSG0004");
              this.SaveSuccess = true;
            }
            else
            {
              WindowUtinity.ShowMessageError("WRN0004");
              this.SaveSuccess = false;
            }
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
    }

    private string ConfirmToCarcass()
    {
      if (chkConfirm.Checked)
      {
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
        inputParam[1] = new DBParameter("@Confirm", DbType.Int32, 1);
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 256, string.Empty) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcass_Confirm", 300, inputParam, outputParam);
        return outputParam[0].Value.ToString();
      }
      return string.Empty;
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private void ShowImageComp()
    {
      if (chkShowImageComp.Checked)
      {
        if (ultData.Selected.Rows.Count > 0 && ultData.Selected.Rows[0].ParentRow == null)
        {
          string componentCode = ultData.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
          string imagePath = FunctionUtility.BOMGetCarcassComponentImage(this.carcassCode, componentCode);
          FunctionUtility.ShowImagePopup(imagePath);
        }
      }
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      // Hidden column
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ComponentPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ComponentCode"].Hidden = true;

      // Rename caption header
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";     
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "VN Description";      
      e.Layout.Bands[0].Columns["FIN_Length"].Header.Caption = "Length";
      e.Layout.Bands[0].Columns["FIN_Width"].Header.Caption = "Width";
      e.Layout.Bands[0].Columns["FIN_Thickness"].Header.Caption = "Thick";
      e.Layout.Bands[0].Columns["APaintFormulaPid"].Header.Caption = "Paint A";      
      e.Layout.Bands[0].Columns["BPaintFormulaPid"].Header.Caption = "Paint B";
      e.Layout.Bands[0].Columns["CPaintFormulaPid"].Header.Caption = "Paint C";
      e.Layout.Bands[0].Columns["APaintValue"].Header.Caption = "Sqm A\n(m2)";
      e.Layout.Bands[0].Columns["BPaintValue"].Header.Caption = "Sqm B\n(m2)";
      e.Layout.Bands[0].Columns["CPaintValue"].Header.Caption = "Sqm C\n(m2)";
      e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["FactoryUnit"].Header.Caption = "Factory Unit";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["QtyCombine"].Header.Caption = "Qty Combine";
      e.Layout.Bands[1].Columns["RAW_Length"].Header.Caption = "Length";
      e.Layout.Bands[1].Columns["RAW_Width"].Header.Caption = "Width";
      e.Layout.Bands[1].Columns["RAW_Thickness"].Header.Caption = "Thick";

      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Read only
      e.Layout.Bands[0].Columns["No"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ComponentCode"].CellActivation = Activation.ActivateOnly;      
      e.Layout.Bands[0].Columns["DescriptionVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["IsMainComp"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FIN_Length"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FIN_Width"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FIN_Thickness"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["APaintValue"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["BPaintValue"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CPaintValue"].CellActivation = Activation.ActivateOnly;

      if (this.confirmed)
      {
        e.Layout.Bands[0].Columns["APaintFormulaPid"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["BPaintFormulaPid"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["CPaintFormulaPid"].CellActivation = Activation.ActivateOnly;
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.#####";
        }
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      // assign value for columns in grid
      e.Layout.Bands[0].Columns["APaintFormulaPid"].ValueList = ucbListPaintA;
      e.Layout.Bands[0].Columns["BPaintFormulaPid"].ValueList = ucbListPaintB;
      e.Layout.Bands[0].Columns["CPaintFormulaPid"].ValueList = ucbListPaintC;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["APaintFormulaPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["APaintFormulaPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;      
      e.Layout.Bands[0].Columns["BPaintFormulaPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["BPaintFormulaPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;      
      e.Layout.Bands[0].Columns["CPaintFormulaPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CPaintFormulaPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      // Set Width
      e.Layout.Bands[0].Columns["No"].MaxWidth = 30;
      e.Layout.Bands[0].Columns["No"].MinWidth = 30;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["APaintFormulaPid"].Width = 100;      
      e.Layout.Bands[0].Columns["APaintFormulaPid"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["BPaintFormulaPid"].Width = 100;
      e.Layout.Bands[0].Columns["BPaintFormulaPid"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["CPaintFormulaPid"].Width = 100;
      e.Layout.Bands[0].Columns["CPaintFormulaPid"].CellAppearance.TextHAlign = HAlign.Left;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        int isMainComp = DBConvert.ParseInt(ultData.Rows[k].Cells["IsMainComp"].Value.ToString());
        if (isMainComp == 1)
        {
          ultData.Rows[k].CellAppearance.BackColor = Color.LightGray;
        }
        else
          ultData.Rows[k].CellAppearance.BackColor = Color.White;
      }
      // set number line header
      e.Layout.Bands[0].ColHeaderLines = 2;

      // set don't allow delete
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      //Summary
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["APaintValue"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["BPaintValue"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CPaintValue"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:#,##0.#####}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:#,##0.#####}";
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:#,##0.#####}";
      e.Layout.Bands[0].SummaryFooterCaption = "Total:";
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      string colError = string.Empty;
      switch (colName)
      {
        case "apaintformulapid":
          colError = "Paint A";
          break;
        case "bpaintformulapid":
          colError = "Paint B";
          break;
        case "cpaintformulapid":
          colError = "Paint C";
          break;
        default:
          break;
      }
      //if colName exist
      if (colError.Trim().Length > 0)
      {
        // Check Data
        string value = e.NewValue.ToString();

        if (value.Trim().Length == 0)
        {
          DataTable dtCheck = (DataTable)ucbListPaintA.DataSource;
          DataRow[] row = dtCheck.Select("");
        }
        else
        {
          DataTable dtCheck = (DataTable)ucbListPaintA.DataSource;
          DataRow[] row = dtCheck.Select(string.Format("Pid = {0}", DBConvert.ParseInt(value)));
          if (row.Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", colError);
            e.Cancel = true;
          }
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
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

    private void chkShowImageComp_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowImageComp();
    }
    #endregion Event
  }
}
