using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using VBReport;

namespace DaiCo.General
{
  public partial class viewGNR_90_009 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewGNR_90_009()
    {
      InitializeComponent();
    }

    private void viewGNR_90_009_Load(object sender, EventArgs e)
    {
      // Init Data
    }

    #endregion Init

    #region Function
    ///// <summary>
    ///// Load UltraCombo
    ///// </summary>
    //private void LoadCombo()
    //{
    //  string commandText = string.Empty;
    //  commandText += " ....";
    //  DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //  if (dtSource != null)
    //  {
    //    ultControl.DataSource = dtSource;
    //    ultControl.DisplayMember = "...";
    //    ultControl.ValueMember = "...";
    //    // Format Grid
    //    ultControl.DisplayLayout.Bands[0].ColHeadersVisible = false;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Width = 250;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Hidden = true;
    //  }
    //}

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;

      // Control
      if (this.ultControl1.Value == null
          || DBConvert.ParseLong(this.ultControl1.Value.ToString()) == long.MinValue)
      {
        message = "...";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@...", DbType.Int64, DBConvert.ParseLong(this.ultControl1.Value.ToString()));
      //inputParam[1] = new DBParameter("@...", DbType.Int64, DBConvert.ParseLong(this.ultControl2.Value.ToString()));
      //inputParam[2] = new DBParameter("@...", DbType.String, this.ultControl3.Value.ToString().Substring(0,9));
      //inputParam[3] = new DBParameter("@...", DbType.Int32, DBConvert.ParseInt(this.ultControl3.Value.ToString().Substring(10, 1)));
      //inputParam[4] = new DBParameter("@...", DbType.Int64, DBConvert.ParseLong(this.ultControl4.Value.ToString()));

      DBParameter[] outputParam = new DBParameter[1];
      DataBaseAccess.ExecuteStoreProcedure("StoreName", inputParam, outputParam);

      int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }

      return true;
    }
    #endregion Function

    #region Event

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Valid
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      this.CloseTab();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
