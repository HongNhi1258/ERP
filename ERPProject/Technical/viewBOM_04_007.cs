using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_04_007 : MainUserControl
  {
    #region Field
    private IList listInvalid = new ArrayList();
    #endregion Field

    #region Inint
    public viewBOM_04_007()
    {
      InitializeComponent();
    }

    private void viewBOM_04_007_Load(object sender, EventArgs e)
    {

    }
    #endregion Init

    #region Function
    private void RemoveInvalidRows(DataTable dtSource, IList lst)
    {
      foreach (int index in lst)
      {
        dtSource.Rows.RemoveAt(index);
      }
      lst.Clear();
    }

    private void SaveData()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          long wo = DBConvert.ParseLong(row["WorkOrder"].ToString());
          string carcass = row["CarcassCode"].ToString().Trim();
          string comp = row["CompCode"].ToString().Trim();
          DateTime date = DBConvert.ParseDateTime(row["PrintedDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          int select = (DBConvert.ParseInt(row["Printed"].ToString()) == int.MinValue) ? 0 : DBConvert.ParseInt(row["Printed"].ToString());
          DBParameter[] input = new DBParameter[7];
          input[0] = new DBParameter("@Wo", DbType.Int64, wo);
          input[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcass);
          input[2] = new DBParameter("@CompCode", DbType.AnsiString, 32, comp);
          input[3] = new DBParameter("@Select", DbType.Int32, select);
          if (date != DateTime.MinValue)
          {
            input[4] = new DBParameter("@PrintedDate", DbType.DateTime, date);
          }
          input[5] = new DBParameter("@PrintBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          input[6] = new DBParameter("@IsImport", DbType.Int32, 1);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spPLNWOCarcassComponentPrinted_Update", input, output);
          if (DBConvert.ParseLong(output[0].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERR0005");
            this.SaveSuccess = false;
            return;
          }
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.SaveSuccess = true;
      }
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 80;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["CompCode"].Header.Caption = "Component Code";
      e.Layout.Bands[0].Columns["CompCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["CompCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["Printed"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Printed"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Printed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 60;
      e.Layout.Bands[0].Columns["PrintedDate"].Header.Caption = "Printed Date";
      e.Layout.Bands[0].Columns["PrintedDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["PrintedDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["PrintedDate"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["PrintedDate"].MinWidth = 120;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
      txtFilePath.Text = string.Empty;
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.InitialDirectory = folder;
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      string importFileName = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      txtFilePath.Text = importFileName;
      btnImport.Enabled = (importFileName.Length > 0);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.listInvalid = new ArrayList();
      // Get max row
      int maxRow = int.MinValue;
      DataSet dsMaxRows = Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), "SELECT * FROM [Sheet1 (1)$J5:J6]");
      if (dsMaxRows != null && dsMaxRows.Tables.Count > 0)
      {
        DataTable dtMaxRows = dsMaxRows.Tables[0];
        if (dtMaxRows != null && dtMaxRows.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtMaxRows.Rows[0][0].ToString()) != int.MinValue)
          {
            maxRow = DBConvert.ParseInt(dtMaxRows.Rows[0][0].ToString()) + 6;
          }
          else
          {
            btnImport.Enabled = true;
            WindowUtinity.ShowMessageError("ERR0001", "MaxRows in Excel");
            return;
          }
        }
      }

      // Get info from Excel file
      DataSet dsInput = Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), string.Format("SELECT * FROM [Sheet1 (1)$B6:G{0}]", maxRow));
      if (dsInput != null && dsInput.Tables.Count > 0)
      {
        DataTable dt = dsInput.Tables[0];
        if (!dt.Columns.Contains("Status"))
        {
          dt.Columns.Add("Status", typeof(System.String));
        }
        if (!dt.Columns.Contains("Name"))
        {
          dt.Columns.Add("Name", typeof(System.String));
          dt.Columns["Name"].SetOrdinal(3);
        }
        if (dt != null && dt.Rows.Count > 0)
        {
          DataTable dtSourceUltra = (DataTable)ultData.DataSource;
          if (dtSourceUltra != null)
          {
            dtSourceUltra.Clear();
          }
          foreach (DataRow row in dt.Rows)
          {
            // Get input data
            long wo = DBConvert.ParseLong(row["WorkOrder"].ToString());
            string carcass = row["CarcassCode"].ToString().Trim();
            string compCode = row["CompCode"].ToString().Trim();
            string commandText = string.Format("SELECT COUNT(*) FROM VBOMCarcassComponentGroup WHERE Wo = {0} AND CarcassCode = '{1}' AND CompGroup = '{2}'", wo, carcass, compCode);
            object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
            int count = obj != null ? DBConvert.ParseInt(obj.ToString()) : 0;
            if (count == 0)
            {
              row["Status"] = "Invalid";
              listInvalid.Add(row.Table.Rows.IndexOf(row));
            }
            else
            {
              row["Status"] = "Normal";
            }
          }
          dtSourceUltra = dt.Copy();
          ultData.DataSource = dtSourceUltra;
          foreach (int index in listInvalid)
          {
            ultData.Rows[index].CellAppearance.BackColor = Color.Yellow;
          }
          btnRemove.Enabled = (this.listInvalid.Count > 0) ? true : false;
          btnSave.Enabled = (this.listInvalid.Count > 0) ? false : true;
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0024");
      btnImport.Enabled = false;
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {
      if (this.listInvalid.Count > 0)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        this.RemoveInvalidRows(dtSource, this.listInvalid);
        btnRemove.Enabled = (this.listInvalid.Count > 0) ? true : false;
        btnSave.Enabled = (this.listInvalid.Count > 0) ? false : true;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event
  }
}
