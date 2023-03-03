/*
 * Author       : Duong Minh 
 * CreateDate   : 12/05/2012
 * Description  : Add Detail From File (Adjustment Out-Finish Good Warehouse)
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using System.Collections;
using DaiCo.Shared.UserControls;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_03_008 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public long issPid = long.MinValue;
    private string pathExport = string.Empty;
    public viewFGH_03_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewFGH_04_004_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathExport = startupPath + @"\Report";
    }
    #endregion Init

    #region Process
   
    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Save Allocate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.txtFile.Text.Trim().Length == 0)
      {
        return;
      }

      try
      {
        string commandText = string.Empty;
        DataTable dtSource = new DataTable();
        DataSet dsXLS = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFile.Text.Trim(), "SELECT * FROM [ADJOUT (1)$A6:B101]");

        if (dtSource == null)
        {
          return;
        }

        DataTable dtMS = new DataTable();
        dtMS = dsXLS.Tables[0];
        int i = 0;
        foreach (DataRow drMS in dtMS.Rows)
        {
          //Check Exist
          Boolean flag = false;
          for (int j = i; j < dtMS.Rows.Count; j++)
          {
            for (int k = j + 1; k < dtMS.Rows.Count; k++)
            {
              if (dtMS.Rows[j]["SeriBoxNo"].ToString() == dtMS.Rows[k]["SeriBoxNo"].ToString())
              {
                flag = true;
                break;
              }
            }
            break;
          }
          i++;

          if (flag == true)
          {
            continue;
          }

          if (drMS["SeriBoxNo"].ToString().Length > 0)
          {
            commandText = string.Empty;
            commandText += " SELECT COUNT(*) ";
            commandText += " FROM TblWHFOutStoreDetail ISD ";
            commandText += " 	INNER JOIN TblWHFBox BOX ON ISD.BoxPID = BOX.PID ";
            commandText += " WHERE BOX.SeriBoxNo = '" + drMS["SeriBoxNo"].ToString() + "' AND ISD.OutStoreID =  " + this.issPid;
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) != 0)
              {
                continue;
              }
            }

            commandText = " SELECT COUNT(*)";
            commandText += " FROM TblWHFOutStore ISS";
            commandText += "	  INNER JOIN TblWHFOutStoreDetail ISD ON ISS.PID = ISD.OutStoreID";
            commandText += " 	  INNER JOIN TblWHFBox BOX ON ISD.BoxPID = BOX.PID ";
            commandText += " WHERE ISS.[Posting] = 0";
            commandText += "	  AND BOX.SeriBoxNo = '" + drMS["SeriBoxNo"].ToString() + "'";
            commandText += "	  AND ISD.OutStoreID != " + this.issPid;

            dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) != 0)
              {
                WindowUtinity.ShowMessageSuccess("MSG0004");
                this.CloseTab();
                return;
              }
            }

            DBParameter[] arrInputParam = new DBParameter[4];
            arrInputParam[0] = new DBParameter("@OutstorePID", DbType.Int64, this.issPid);

            long boxPid = long.MinValue;
            commandText = "SELECT PID FROM TblWHFBox WHERE SeriBoxNo = '" + drMS["SeriBoxNo"].ToString() + "' AND Status = 2";
            dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtSource.Rows.Count > 0)
            {
              boxPid = DBConvert.ParseLong(dtSource.Rows[0]["PID"].ToString());
            }
            else
            {
              continue;
            }

            arrInputParam[1] = new DBParameter("@BoxPID", DbType.Int64, boxPid);

            commandText = "SELECT Location FROM TblWHFBoxInStore WHERE BoxPID = " + boxPid ;
            dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtSource.Rows.Count > 0)
            {
              arrInputParam[2] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(dtSource.Rows[0]["Location"].ToString()));
            }
            else
            {
              continue;
            }

            arrInputParam[3] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtDescription.Text);

            DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetailAdjustment_Insert", arrInputParam);

            //TBLWHFINBOX
            DBParameter[] arrInputParamBox = new DBParameter[2];
            arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, drMS["SeriBoxNo"].ToString());
            arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 3);
            DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

            //TBLWHFINBOXINSTORE
            DBParameter[] arrInputParamBoxInStore = new DBParameter[1];
            arrInputParamBoxInStore[0] = new DBParameter("@BoxPID", DbType.Int32, boxPid);

            DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Delete", arrInputParamBoxInStore);
          }
        }
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Can not copy file! Please close file!");
        return;
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.CloseTab();
    }

    private void btnBrower_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnSave.Enabled = (txtFile.Text.Trim().Length > 0);
    }
    #endregion Process
  }
}

