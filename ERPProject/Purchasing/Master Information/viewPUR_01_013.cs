/*
  Author      : Huynh Thi Bang
  Date        : 25/10/2016
  Description : Upload file contractno
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace DaiCo.ERPProject
{
  public partial class viewPUR_01_013 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;
    public long viewPid = long.MinValue;
    private string sourseFile = string.Empty;
    private string destFile = string.Empty;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public viewPUR_01_013()
    {
      InitializeComponent();
    }

    private void viewPUR_01_013_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@TransactionPid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURContractNoFileUpload", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        DataTable dtMain = dsSource.Tables[0];
        txtMaterialCode.Text = dtMain.Rows[0]["MaterialCode"].ToString().Trim();
        txtSupplier.Text = dtMain.Rows[0]["EnglishName"].ToString().Trim();
        txtContractNo.Text = dtMain.Rows[0]["ContractNo"].ToString().Trim();
      }
      this.ultData.DataSource = dsSource.Tables[1];
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultData.Rows[i];
        if (File.Exists(rowGrid.Cells["File"].Value.ToString()))
        {
          rowGrid.Cells["Type"].Appearance.Image = Image.FromFile(rowGrid.Cells["File"].Value.ToString());
        }
      }
    }
    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnUpload.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    private void btnUpload_Click(object sender, EventArgs e)
    {
      if (this.txtLocation.Text.Trim().Length > 0)
      {
        string file = txtLocation.Text;
        FileInfo f = new FileInfo(file);
        long fLength = f.Length;
        //if (fLength < 5120000)
        //{
        string extension = System.IO.Path.GetExtension(file).ToLower();
        string typeFile = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE Value = '" + extension + "' AND [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPEFILEUPLOAD;
        DataTable dtTypeFile = DataBaseAccess.SearchCommandTextDataTable(typeFile);
        if (dtTypeFile != null && dtTypeFile.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtTypeFile.Rows[0][0].ToString()) > 0)
          {
            string fileName1 = System.IO.Path.GetFileName(file).ToString();
            string fileName = System.IO.Path.GetFileNameWithoutExtension(file).ToString()
                                    + DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd"))
                                    + DBConvert.ParseString(DateTime.Now.Ticks)
                                    + System.IO.Path.GetExtension(file);

            string sourcePath = System.IO.Path.GetDirectoryName(file);
            string commandText = string.Empty;
            commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 1);
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            string targetPath = string.Empty;
            if (dt != null && dt.Rows.Count > 0)
            {
              targetPath = dt.Rows[0][0].ToString();
            }

            sourseFile = System.IO.Path.Combine(sourcePath, fileName1);
            destFile = System.IO.Path.Combine(targetPath, fileName);
            if (!System.IO.Directory.Exists(targetPath))
            {
              System.IO.Directory.CreateDirectory(targetPath);
            }
            DataTable dtSource = (DataTable)ultData.DataSource;
            int i = dtSource.Rows.Count;
            foreach (DataRow row1 in dtSource.Rows)
            {
              if (row1.RowState == DataRowState.Deleted)
              {
                i = i - 1;
              }
            }
            DataRow row = dtSource.NewRow();
            row["FileName"] = fileName1;
            row["LocationFile"] = destFile;
            row["LocationFileLocal"] = sourseFile;
            dtSource.Rows.Add(row);
            if (String.Compare(extension, ".docx") == 0 || String.Compare(extension, ".doc") == 0)
            {
              this.ultData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "word.bmp");
              row["File"] = targetPath + "word.bmp";
            }
            else if (string.Compare(extension, ".xls") == 0 || string.Compare(extension, ".xlsx") == 0)
            {
              this.ultData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "xls.bmp");
              row["File"] = targetPath + "xls.bmp";
            }
            else if (string.Compare(extension, ".pdf") == 0)
            {
              this.ultData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "pdf.bmp");
              row["File"] = targetPath + "pdf.bmp";
            }
            else if (string.Compare(extension, ".txt") == 0)
            {
              this.ultData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "notepad.bmp");
              row["File"] = targetPath + "notepad.bmp";
            }
            else if (string.Compare(extension, ".gif") == 0
                      || string.Compare(extension, ".jpg") == 0
                      || string.Compare(extension, ".bmp") == 0
                      || string.Compare(extension, ".png") == 0)
            {
              this.ultData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "image.bmp");
              row["File"] = targetPath + "image.bmp";
            }
            this.btnUpload.Enabled = false;
          }
        }
      }
    }
    private DataTable dtWOIncreaseResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int16));
      dt.Columns.Add("SaleNo", typeof(System.String));
      dt.Columns.Add("QtyIncrease", typeof(System.Int16));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }
    /// <summary>
    /// Check Valaid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private bool CheckData()
    {
      DataTable dt = (DataTable)ultData.DataSource;
      if (dt.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          string file = ultData.Rows[i].Cells["FileName"].Value.ToString();
          for (int j = i + 1; j < ultData.Rows.Count; j++)
          {
            string file2 = ultData.Rows[j].Cells["FileName"].Value.ToString();
            if (file == file2)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
              ultData.Rows[j].CellAppearance.BackColor = Color.Yellow;
              MessageBox.Show("This file is already added", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
              this.SaveSuccess = false;
              return false;
            }
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private bool SaveUploadFile()
    {
      string storeName = string.Empty;
      DataTable dtMain = (DataTable)this.ultData.DataSource;
      foreach (DataRow row in dtMain.Rows)
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row.RowState == DataRowState.Added)
          {
            //Copy File
            System.IO.File.Copy(row["LocationFileLocal"].ToString(), row["LocationFile"].ToString(), true);
          }
          storeName = "spPURContractNoFileUpload_Edit";
          DBParameter[] inputParam = new DBParameter[6];

          //Pid
          if (DBConvert.ParseLong(row["Pid"].ToString()) >= 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }

          inputParam[1] = new DBParameter("@TranSactionDetailPid", DbType.Int64, this.viewPid);

          inputParam[2] = new DBParameter("@FileName", DbType.String, 512, row["FileName"].ToString());

          inputParam[3] = new DBParameter("@LocationFile", DbType.String, 512, row["LocationFile"].ToString());

          inputParam[4] = new DBParameter("@Remark", DbType.String, 4000, row["Remark"].ToString());

          inputParam[5] = new DBParameter("@File", DbType.String, row["File"].ToString());


          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      return true;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckData())
      {
        bool success = true;
        success = this.SaveUploadFile();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }
    /// <summary>
    /// Save Transaction Master
    /// </summary>
    /// <returns></returns>


    #endregion Function

    #region Event

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
          DBParameter[] inputParams = new DBParameter[1];
          inputParams[0] = new DBParameter("@UploadPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string storeName = string.Empty;
          storeName = "spPURContractUploadFile_Delete";
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            return;
          }
        }
      }
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultData.Selected.Rows[0];
      Process prc = new Process();

      if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) == int.MinValue)
      {
        prc.StartInfo = new ProcessStartInfo(row.Cells["LocationFileLocal"].Value.ToString());
      }
      else
      {
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string folder = string.Format(@"{0}\Temporary", startupPath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
        string locationFile = row.Cells["LocationFile"].Value.ToString();
        if (File.Exists(locationFile))
        {
          string newLocationFile = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["LocationFile"].Value.ToString()));
          if (File.Exists(newLocationFile))
          {
            try
            {
              File.Delete(newLocationFile);
            }
            catch
            {
              WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
              return;
            }
          }
          File.Copy(locationFile, newLocationFile);
          prc.StartInfo = new ProcessStartInfo(newLocationFile);
        }
      }
      try
      {
        prc.Start();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      Utility.SetPropertiesUltraGrid(ultData);
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      // Hide column
      //e.Layout.Bands[0].Columns["TranSactionDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContracNoPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFile"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFileLocal"].Hidden = true;
      e.Layout.Bands[0].Columns["File"].Hidden = true;

      e.Layout.Bands[0].Columns["Type"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Type"].MinWidth = 50;
      e.Layout.Bands[0].Columns["FileName"].MaxWidth = 300;
      e.Layout.Bands[0].Columns["FileName"].MinWidth = 300;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Event

  }
}
