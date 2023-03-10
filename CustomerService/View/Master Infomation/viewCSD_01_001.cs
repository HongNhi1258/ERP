/*
  Author        : Lâm Quang Hà
  Create date   : 06/10/2010
  Decription    : Search and display forwarder from name and address
  Checked by    : Võ Hoa Lư
  Checked date  : 12/10/2010
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared.UserControls;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_001 : MainUserControl
  {
    
    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_001()
    {
      InitializeComponent();
    }
    #endregion Init Data

    #region GetData
    /// <summary>
    /// Search Forwarder infomation from name and address condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[2];
      string code = txtCode.Text.Trim();
      if (code.Length > 0)
      {
        param[0] = new DBParameter("@Code", DbType.String, 8, "%" + code + "%");
      }
      string name = txtName.Text.Trim();
      if (name.Length > 0)
      {
        param[1] = new DBParameter("@Name", DbType.String, 128, "%" + name + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDForwarder_Select", param);
      ultraGridInfomation.DataSource = dtSource;      
    }
    #endregion GetData

    #region Event
    /// <summary>
    /// Search Forwarder infomation from name and address condition
    /// </summary>
    /// <param name="sender">button Search</param>
    /// <param name="e">Click</param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Init layout for ultragrid view Forwarder Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInfomation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridInfomation);
      e.Layout.Bands[0].Columns["ForwarderCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ForwarderCode"].MinWidth = 80;        
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ForwarderCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ForwarderCode"].Header.Caption = "Code";
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["Address"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Tel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Fax"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Email"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ContactPerson"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ContactPerson"].Header.Caption = "Contact Person";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }
    
    /// <summary>
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    
    /// <summary>
    /// Delete Physical/logic list forwarders which is selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (ultraGridInfomation.Rows.Count > 0)
      {
        int countCheck = 0;
        for (int i = 0; i < ultraGridInfomation.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultraGridInfomation.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            countCheck++;
          }
        }
        if (countCheck == 0)
        {
          WindowUtinity.ShowMessageWarning("WRN0012");
          return;
        }
        DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
        if (result == DialogResult.Yes)
        {
          for (int i = 0; i < ultraGridInfomation.Rows.Count; i++)
          {
            int selected = DBConvert.ParseInt(ultraGridInfomation.Rows[i].Cells["Selected"].Value.ToString());
            if (selected == 1)
            {
              long pid = DBConvert.ParseLong(ultraGridInfomation.Rows[i].Cells["Pid"].Value.ToString());
              DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
              DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spCSDForwarder_Delete", inputParam, outputParam);
              long success = DBConvert.ParseInt(outputParam[0].Value.ToString());
              if (success == 1)
              {
                WindowUtinity.ShowMessageSuccess("MSG0002");
              }
              else if (success == 0)
              {
                WindowUtinity.ShowMessageError("ERR0005");
              }
            }
          }
          this.Search();
        }
      }
    }
    
    /// <summary>
    /// Open screen update forwarder(viewCSD_01_002)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInfomation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridInfomation.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultraGridInfomation.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      viewCSD_01_002 view = new viewCSD_01_002();
      view.forwardPid = pid;
      WindowUtinity.ShowView(view, "FORWARDER INFORMATION", false, ViewState.ModalWindow);
      this.Search();

    }
    
    /// <summary>
    /// Open screen new forwarder(viewCSD_01_002)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_01_002 view = new viewCSD_01_002();
      WindowUtinity.ShowView(view, "FORWARDER INFORMATION", false, ViewState.Window);
      this.Search();
    }
    #endregion Event    
  }
}   


