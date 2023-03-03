/*
 * Author       : Duong Minh 
 * CreateDate   : 07/05/2012
 * Description  : Add Detail From File (Adjustment In-Finish Good Warehouse)
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
  public partial class viewFGH_04_005 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public long receiptPid = long.MinValue;
    private string pathExport = string.Empty;
    public int flagReturn = 0;
    public viewFGH_04_005()
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
        DataSet dsXLS = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFile.Text.Trim(), "SELECT * FROM [ADJIN (1)$A6:E501]");

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
            if (drMS["BoxCode"].ToString().Length > 0)
            {
              commandText = "SELECT COUNT(*) FROM TblBOMBoxType WHERE BoxTypeCode = '" + drMS["BoxCode"].ToString() + "'";
              dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtSource != null && dtSource.Rows.Count > 0)
              {
                if (DBConvert.ParseInt(dtSource.Rows[0][0].ToString()) == 0)
                {
                  continue;
                }

                DBParameter[] arrInputBox = new DBParameter[4];
                arrInputBox[0] = new DBParameter("@SeriBoxNo", DbType.AnsiString, 24, drMS["SeriBoxNo"].ToString());
                arrInputBox[1] = new DBParameter("@BoxTypeCode", DbType.AnsiString, 24, drMS["BoxCode"].ToString());
                if (this.flagReturn == 0)
                {
                  arrInputBox[2] = new DBParameter("@ProductType", DbType.Int32, 1);
                }
                else
                {
                  arrInputBox[2] = new DBParameter("@ProductType", DbType.Int32, 5);
                }

                if (DBConvert.ParseInt(drMS["WO"].ToString()) != int.MinValue)
                {
                  arrInputBox[3] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(drMS["WO"].ToString()));
                }
                DataBaseAccess.ExecuteStoreProcedure("spWHFCreateBoxReturn_Insert", arrInputBox);
              }
            }

            commandText = string.Empty;
            commandText += " SELECT COUNT(*) ";
            commandText += " FROM TblWHFInStoreDetail ISD ";
            commandText += " 	INNER JOIN TblWHFBox BOX ON ISD.BoxPID = BOX.PID ";
            commandText += " WHERE BOX.SeriBoxNo = '" + drMS["SeriBoxNo"].ToString() + "' AND ISD.InStorePID =  " + this.receiptPid;
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) != 0)
              {
                continue;
              }
            }

            commandText = " SELECT COUNT(*)";
            commandText += " FROM TblWHFInStore ISS";
            commandText += "	  INNER JOIN TblWHFInStoreDetail ISD ON ISS.PID = ISD.InStorePID";
            commandText += " 	  INNER JOIN TblWHFBox BOX ON ISD.BoxPID = BOX.PID ";
            commandText += " WHERE ISS.[Posting] = 0";
            commandText += "	  AND BOX.SeriBoxNo = '" + drMS["SeriBoxNo"].ToString() + "'";
            commandText += "	  AND ISD.InStorePID != " + this.receiptPid;

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
            arrInputParam[0] = new DBParameter("@InstorePID", DbType.Int64, this.receiptPid);

            long boxPid = long.MinValue;
            commandText = "SELECT PID FROM TblWHFBox WHERE SeriBoxNo = '" + drMS["SeriBoxNo"].ToString() + "' AND Status != 2";
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

            commandText = "SELECT PID FROM TblWHFLocation WHERE Location = '" + drMS["Location"].ToString() + "'";
            dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtSource.Rows.Count > 0)
            {
              arrInputParam[2] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(dtSource.Rows[0]["PID"].ToString()));
            }
            else
            {
              continue;
            }

            arrInputParam[3] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtDescription.Text);

            DataBaseAccess.ExecuteStoreProcedure("spWHFInStoreDetailAdjustment_Insert", arrInputParam);
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

