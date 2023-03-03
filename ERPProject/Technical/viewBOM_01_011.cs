using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_011 : MainUserControl
  {
    #region fields
    public string carcassCode = string.Empty;
    #endregion fields

    #region function
    private bool CheckInvalid()
    {
      if (ultraCBRootComponent.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Root Component");
        return false;
      }
      int qty = DBConvert.ParseInt(txtQty.Text);
      if (qty <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Qty");
        return false;
      }
      return true;
    }

    private void SaveData()
    {
      if (this.CheckInvalid())
      {
        long pidRootComp = DBConvert.ParseLong(ultraCBRootComponent.SelectedRow.Cells["Pid"].Value.ToString());
        int qty = DBConvert.ParseInt(txtQty.Text);
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@PidRootComp", DbType.Int64, pidRootComp);
        inputParam[1] = new DBParameter("@IsMainComp", DbType.Int32, 1);
        inputParam[2] = new DBParameter("@Qty", DbType.Int32, qty);
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponent_UpdateRootComp", inputParam, outputParam);
        if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
        else
        {
          this.CloseTab();
        }
      }
    }

    private void LoadUltraCBRootComponent()
    {
      string commandText = string.Format(@"SELECT COMP.Pid, COMP.ComponentCode, COMP.DescriptionVN, (COMP.ComponentCode + ' - ' + COMP.DescriptionVN) DisplayText
                                          FROM TblBOMCarcassComponent COMP
                                            LEFT JOIN TblBOMCarcassComponentStruct STRUCT ON (COMP.Pid = STRUCT.SubCompPid) 
                                          WHERE STRUCT.Pid IS NULL AND ISNULL(COMP.IsMainComp, 0) = 0 AND COMP.CarcassCode = '{0}'", this.carcassCode);
      DataTable dtRootComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBRootComponent.DataSource = dtRootComp;
      ultraCBRootComponent.ValueMember = "Pid";
      ultraCBRootComponent.DisplayMember = "DisplayText";
    }
    #endregion function

    #region event
    public viewBOM_01_011()
    {
      InitializeComponent();
    }

    private void ultraCBRootComponent_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["DisplayText"].Hidden = true;
    }

    private void viewBOM_01_011_Load(object sender, EventArgs e)
    {
      this.LoadUltraCBRootComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }
    #endregion event
  }
}
