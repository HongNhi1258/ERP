/*
 * Created By   : Nguyen Van Tron
 * Created Date : 06/02/2011
 * Description  : Allocate or Register detail
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_005 : MainUserControl
  {
    #region fields
    public int materialControlType = int.MinValue;
    public long wo = long.MinValue;
    public string itemCode = string.Empty;
    public int revision = int.MinValue;
    public string materialCode = string.Empty;
    #endregion fields

    #region function 
    /// <summary>
    /// Load history of allocation or register
    /// </summary>
    private void LoadHistory()
    {
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@Wo", DbType.Int64, this.wo);
      if (materialControlType == 1)
      {
        inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
        inputParam[2] = new DBParameter("@Revision", DbType.Int32, this.revision);
      }
      inputParam[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, this.materialCode);
      DataTable dtAllocationHistory = DataBaseAccess.SearchStoreProcedureDataTable("spPLNHistoryAllocatedAdvanceMaterial", inputParam);
      ultraGridAllocationHistory.DataSource = dtAllocationHistory;
    }
    #endregion function

    #region event
    public viewPLN_07_005()
    {
      InitializeComponent();
    }

    private void viewPLN_07_005_Load(object sender, EventArgs e)
    {
      this.LoadHistory();
    }

    private void ultraGridHistory_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      if (this.materialControlType == 0)
      {
        e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
        e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      }
      else if (this.materialControlType == 1)
      {
        e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
        e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
        e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
        e.Layout.Bands[0].Columns["Revision"].MinWidth = 60;
        e.Layout.Bands[0].Columns["Revision"].MaxWidth = 60;
      }

      e.Layout.Bands[0].Columns["WO"].MinWidth = 50;
      e.Layout.Bands[0].Columns["WO"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Allocated Date";
      e.Layout.Bands[0].Columns["CreateDate"].MinWidth = 160;
      e.Layout.Bands[0].Columns["CreateDate"].MaxWidth = 160;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Allocated By";
      //e.Layout.Bands[0].Columns["CreateBy"].MinWidth = 110;
      //e.Layout.Bands[0].Columns["CreateBy"].MaxWidth = 110;      
    }

    private void ultraGridHistory_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //string title = "Deduct Allocation";
      //try
      //{
      //  if (ultraGridAllocationHistory.Selected.Rows.Count > 0)
      //  {
      //    string alterMaterial = ultraGridAllocationHistory.Selected.Rows[0].Cells["MaterialCode"].Value.ToString();
      //    string mainMaterial = ultraGridAllocationHistory.Selected.Rows[0].Cells["RequestMaterialCode"].Value.ToString();
      //    viewPLN_07_006 view = new viewPLN_07_006();
      //    view.type = 0;
      //    view.wo = this.wo;
      //    view.itemCode = this.itemCode;
      //    view.revision = this.revision;
      //    view.materialCode = alterMaterial;
      //    view.requiredMaterialCode = mainMaterial;
      //    view.materialControlType = this.materialControlType;
      //    WindowUtinity.ShowView(view, title, true, ViewState.ModalWindow);
      //    this.LoadHistory();
      //    this.LoadQtyRequiredMaterialInformation();
      //  }
      //}
      //catch
      //{        
      //  WindowUtinity.ShowMessageError("MSG0011", "the row you want to " + title);
      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion event
  }
}
