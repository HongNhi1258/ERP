/*
  Author        : Võ Hoa Lư
  Create date   : 16/06/2011
  Decription    : Import item price from excel file  
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System.Diagnostics;
using System.IO;
using System.Data.OleDb;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_013 : MainUserControl
  {
    #region Fields
    public viewCSD_04_011 parent;
    #endregion Fields

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_04_013()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewCSD_04_013_Load(object sender, EventArgs e)
    {
      parent.customerPid = long.MinValue;
      this.LoadData();
    }
    #endregion Init Form

    #region Load Data
    /// <summary>
    /// Bind data
    /// </summary>
    private void LoadData() {
//      string commandText = string.Format(@"SELECT Pid,  CustomerCode Code, Name, 0 Selected
//                                           FROM TblCSDCustomerInfo
//                                           WHERE ParentPid is null AND Pid not in (1,2,3, 11, 12, 27) AND DeletedFlg = 0
//                                           ORDER BY Name");
      string commandText = string.Format(@"SELECT Pid,  CustomerCode Code, Name, 0 Selected
                                           FROM TblCSDCustomerInfo
                                           WHERE ParentPid is null AND Pid > 3 AND Kind = 5 AND DeletedFlg = 0
                                           ORDER BY Name");
      DataTable dataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ugidDistribute.DataSource = dataSource;
    }
    #endregion Load Data

    #region Events
    /// <summary>
    /// Uncheck another rows
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ugidDistribute_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      if(columnName.Equals("Selected", StringComparison.OrdinalIgnoreCase)){
        int rowIndex = e.Cell.Row.Index;
        string text = e.Cell.Text;
        if (text.Equals("1"))
        {
          for (int i = 0; i < ugidDistribute.Rows.Count; i++)
          {
            if(i != rowIndex){
              ugidDistribute.Rows[i].Cells["Selected"].Value = 0;
            }
          }
        }
      }
    }

    /// <summary>
    /// Initialize Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ugidDistribute_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ugidDistribute);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Code"].MinValue = 70;
      e.Layout.Bands[0].Columns["Code"].MaxValue = 70;
      e.Layout.Bands[0].Columns["Code"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Selected"].MinValue = 50;
      e.Layout.Bands[0].Columns["Selected"].MaxValue = 50;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      parent.customerPid = long.MinValue;
      this.CloseTab();
    }

    /// <summary>
    /// Close and set customerPid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnChoose_Click(object sender, EventArgs e)
    {
      int count = ugidDistribute.Rows.Count;
      string customerCode = string.Empty;
      long selectedPid = long.MinValue;
      for (int i = 0; i < count; i++) { 
        int selected = DBConvert.ParseInt(ugidDistribute.Rows[i].Cells["Selected"].Value.ToString());
        if (selected == 1) {
          selectedPid = DBConvert.ParseLong(ugidDistribute.Rows[i].Cells["Pid"].Value.ToString());
          customerCode = ugidDistribute.Rows[i].Cells["Code"].Value.ToString();
          break;
        }
      }
      if (selectedPid == long.MinValue) {
        WindowUtinity.ShowMessageError("MSG0043");
        return;
      }
      parent.customerPid = selectedPid;
      parent.customerCode = customerCode;
      this.CloseTab();
    }
    #endregion Events
  }
}