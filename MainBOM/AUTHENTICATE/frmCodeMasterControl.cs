/*
  Author      : Ha Anh
  Description : Relation Module - Code Master
  Date        : 05-09-2011
*/

using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.General;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace MainBOM.AUTHENTICATE
{
  public partial class frmCodeMasterControl : Form
  {
    #region variable
    private int rowFocus = 0;
    public long group = long.MinValue;
    #endregion variable

    #region Load Data
    public frmCodeMasterControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// load form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmCodeMasterControl_Load(object sender, EventArgs e)
    {
      this.LoadModule();
      this.LoadGroup();
      if (group != long.MinValue)
      {
        cbModule.Value = group;
      }
      this.Search();
    }

    /// <summary>
    /// Load Module
    /// </summary>
    private void LoadModule()
    {
      string commandText = "SELECT Pid, NameEN, Description FROM TblGNRAccessGroup";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      cbModule.DataSource = dt;
      cbModule.DisplayMember = "NameEN";
      cbModule.ValueMember = "Pid";
      cbModule.DisplayLayout.AutoFitColumns = true;
      cbModule.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      cbModule.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Group
    /// </summary>
    private void LoadGroup()
    {
      string commandText = "SELECT NameEn, NameVn FROM TblBOMMasterName WHERE [Group] <> 15001";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      cbGroup.DataSource = dt;
      cbGroup.ValueMember = "NameEn";
      cbGroup.DisplayMember = "NameEn";
      cbGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      cbGroup.DisplayLayout.AutoFitColumns = true;
    }
    #endregion Load Data

    #region Event
    /// <summary>
    /// search click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// function
    /// </summary>
    private void Search()
    {
      chkExpandAll.Checked = false;
      DBParameter[] inputParam = new DBParameter[2];
      if (cbModule.Value != null && cbModule.Text.Length > 0)
      {
        group = DBConvert.ParseLong(cbModule.Value.ToString());
      }
      if (group != long.MinValue)
      {
        inputParam[1] = new DBParameter("@Group", DbType.Int64, group);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRCodeMasterControl_Select", inputParam);
      dsGNRGroupRelationCodeMaster dsRelation = new dsGNRGroupRelationCodeMaster();
      if (ds != null)
      {
        dsRelation.Tables["Group"].Merge(ds.Tables[0]);
        dsRelation.Tables["CodeMaster"].Merge(ds.Tables[1]);
      }
      ultraGrid.DataSource = dsRelation;

      //scroll grid
      int countRow = 0;
      rowFocus++;
      string message = string.Empty;

      if (cbGroup.Text.ToString().Trim().Length > 0)
      {
        for (int i = 0; i < ultraGrid.Rows.Count; i++)
        {
          if (ultraGrid.Rows[i].Cells["NameEn"].Value.ToString() == cbGroup.Text.ToString())
          {
            countRow++;
            if (countRow == rowFocus)
            {
              ultraGrid.Rows[i].Selected = true;
              ultraGrid.ActiveRowScrollRegion.ScrollRowIntoView(ultraGrid.Rows[i]);
            }
          }
        }
        if (rowFocus >= countRow)
        {
          rowFocus = 0;
        }
      }
    }

    /// <summary>
    /// click close tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Init ultraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Group"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["NameEn"].Header.Caption = "Name En";
      e.Layout.Bands[0].Columns["NameVn"].Header.Caption = "Name Vn";
      e.Layout.Bands[0].Columns["Group"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[1].Columns["Group"].Hidden = true;
      e.Layout.Bands[1].Columns["IsDeleted"].Hidden = true;
      e.Layout.Bands[1].Columns["Kind"].Hidden = true;
      e.Layout.Bands[1].Columns["Code"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.DarkSeaGreen;
    }

    /// <summary>
    /// save click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool check = this.save();
      if (check == true)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      this.Search();
    }

    /// <summary>
    /// function save khi click button save
    /// </summary>
    /// <returns></returns>
    private bool save()
    {
      DataSet ds = (DataSet)ultraGrid.DataSource;
      if (ds != null)
      {
        DataTable dt = ds.Tables[0];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState == DataRowState.Modified)
          {
            string storename = "spGNRCodeMasterControl_Edit";
            DBParameter[] inputParam = new DBParameter[4];
            if (DBConvert.ParseInt(dt.Rows[i]["Select"].ToString()) == 0)
            {
              inputParam[0] = new DBParameter("@Select", DbType.Int32, 0);
            }
            else
            {
              inputParam[0] = new DBParameter("@Select", DbType.Int32, 1);
            }

            inputParam[2] = new DBParameter("@GroupPid", DbType.Int64, group);
            inputParam[3] = new DBParameter("@GroupMaster", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Group"].ToString()));

            DBParameter[] outParam = new DBParameter[1];
            outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);

            if (DBConvert.ParseLong(outParam[0].Value.ToString()) == 0 && DBConvert.ParseLong(outParam[0].Value.ToString()) == long.MinValue)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Expand All click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultraGrid.Rows.ExpandAll(true);
      }
      else
      {
        ultraGrid.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// Group value change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbGroup_ValueChanged(object sender, EventArgs e)
    {
      rowFocus = 0;
    }

    /// <summary>
    /// module value change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cbModule_ValueChanged(object sender, EventArgs e)
    {
      rowFocus = 0;
    }
    #endregion Event
  }
}