/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 14/8/2015
  Description : Update and remove carcass work
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_29_008 : MainUserControl
  {
    #region Field
    public DataTable dtTable = new DataTable();
    public bool flagSearch = false;
    #endregion Field

    #region Init

    public viewPLN_29_008()
    {
      InitializeComponent();
    }

    private void viewPLN_29_008_Load(object sender, EventArgs e)
    {
      this.LoadUltraCarcassWork();
    }

    #endregion Init

    #region Function

    private void LoadUltraCarcassWork()
    {
      string commandText = "SELECT Pid, TransactionCode FROM TblPLNCarcassWorkOrderTransaction";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCarWO, dtSource, "Pid", "TransactionCode", false, "Pid");
    }

    private bool CheckVaild()
    {
      if (rddAdd.Checked)
      {
        if (ultCarWO.SelectedRow == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Carcass Work");
          return false;
        }
        else
        {
          string cmFUR = string.Format(@"SELECT Pid FROM TblPLNCarcassWorkOrderTransaction
                                            WHERE Pid ={0}", ultCarWO.Value);
          DataTable dtFUR = DataBaseAccess.SearchCommandTextDataTable(cmFUR);
          if (dtFUR.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Carcass Work");
            return false;
          }
        }
      }
      if (txtFileImport.Text.Trim() != "")
      {
        this.dtTable.Clear();
        DataTable dtData = FunctionUtility.GetOpenOfficeCalcToDataSet(txtFileImport.Text.Trim(), "SELECT * FROM [Sheet1 (1)$A3:A500]").Tables[0];
        if (dtData != null && dtData.Rows.Count > 0)
        {
          //this.dtTable.Columns.Add("HangTag", typeof(System.String));
          for (int i = 0; i < dtData.Rows.Count; i++)
          {
            if (dtData.Rows[i]["KeyInput"].ToString().Trim() != "")
            {
              DataRow row = dtTable.NewRow();
              row["HangTag"] = dtData.Rows[i]["KeyInput"].ToString().Trim();
              this.dtTable.Rows.Add(row);
            }
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0027");
          return false;
        }
      }
      return true;
    }

    private bool SaveDetail()
    {
      bool success = true;
      SqlDBParameter[] input = new SqlDBParameter[3];
      if (ultCarWO.SelectedRow != null)
      {
        input[0] = new SqlDBParameter("@CarcassWOPid", SqlDbType.BigInt, ultCarWO.Value);
      }
      else
      {
        input[0] = new SqlDBParameter("@CarcassWOPid", SqlDbType.BigInt, DBNull.Value);
      }

      input[1] = new SqlDBParameter("@DataHangtag", SqlDbType.Structured, dtTable);
      if (rddAdd.Checked)
      {
        input[2] = new SqlDBParameter("@Type", SqlDbType.Int, 0);
      }
      else if (rddRemove.Checked)
      {
        input[2] = new SqlDBParameter("@Type", SqlDbType.Int, 1);
      }
      SqlDBParameter[] ouput = new SqlDBParameter[1];
      ouput[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);

      SqlDataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanUpdateCarcassWork", 6000, input, ouput);
      if (DBConvert.ParseLong(ouput[0].Value.ToString()) <= 0)
      {
        success = false;
      }
      return success;
    }

    #endregion Function

    #region Event

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckVaild())
      {
        bool success = this.SaveDetail();
        if (success)
        {
          this.flagSearch = true;
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.CloseTab();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void rddRemove_CheckedChanged(object sender, EventArgs e)
    {
      if (rddRemove.Checked)
      {
        ultCarWO.Enabled = false;
      }
    }

    private void rddAdd_CheckedChanged(object sender, EventArgs e)
    {
      if (rddAdd.Checked)
      {
        ultCarWO.Enabled = true;
      }
    }

    //Tien add import excel
    private void button1_Click(object sender, EventArgs e)
    {
      //Get Template
      string templateName = "TemplateUpdateCWFurniture";
      string sheetName = "Sheet1 (1)";
      string outFileName = "Template Update CWo For Furniture";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      string strInFileName = string.Format(@"{0}\{1}.xls", pathTemplate, templateName);
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, templateName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      //oXlsReport.Out.File(outFileName);
      //Process.Start(outFileName);
      System.IO.File.Copy(strInFileName, strOutFileName);
      Process.Start(strOutFileName);
    }
    private void btnBrower_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtFileImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    #endregion Event

  }
}
