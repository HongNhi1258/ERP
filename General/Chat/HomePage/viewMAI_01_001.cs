using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.IO;
using DaiCo.Shared.Utility;
using VBReport;
using System.Diagnostics;
using System.Reflection;
using FormSerialisation;
using DaiCo.Shared.DataBaseUtility;
namespace DaiCo.General
{
  public partial class viewMAI_01_001 : MainUserControl
  {
    private int xPos = 0;


    public viewMAI_01_001()
    {
      InitializeComponent();
    }

    private void viewMAI_01_001_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    private void LoadData()
    {
      DaiCo.Shared.Utility.ControlUtility.LoadUltraDropDownEmployee(ultTransferEID, SharedObject.UserInfo.Department);
      chkRead.Checked = false;
      this.LoadTask(false);
      this.loadBirthday();
      this.loadAttendance();
      this.loadLeave();
      this.loadKPI();
      Timer timer = new Timer();
      timer.Interval = 100;
      timer.Tick += new EventHandler(timer_Tick);
      timer.Start();

      Timer timer1 = new Timer();
      timer1.Interval = 300000;

      timer1.Tick += new EventHandler(timer1_Tick);
      timer1.Start();
    }

    void timer1_Tick(object sender, EventArgs e)
    {
      this.LoadTask(chkRead.Checked);
    }

    void timer_Tick(object sender, EventArgs e)
    {
      this.txtMarquee.Text = " " + txtMarquee.Text;
      xPos++;
      if (xPos >= 220)
      {
        xPos = 0;
        this.txtMarquee.Text = this.txtMarquee.Text.Trim();
      }
    }
    private void loadBirthday()
    {
      string storeName = "spGNRBirthday_Select";
      DBParameter[] param = new DBParameter[2];
      string Dep = SharedObject.UserInfo.Department;
      param[0] = new DBParameter("@Department", DbType.AnsiString, 20, Dep);
      param[1] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataSet dsSource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      ultBirthday.DataSource = dsSource;
      Bitmap b = new Bitmap(FunctionUtility.GetEmailIcon(4));
      for (int i = 0; i < ultBirthday.Rows.Count; i++)
      {
        if (ultBirthday.Rows[i].Cells["IsPic"].Value.ToString() == "0")
        {
          ultBirthday.Rows[i].Cells["Pic"].Appearance.Image = b;
          ultBirthday.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
        else
        {
          ultBirthday.Rows[i].CellAppearance.BackColor = Color.White;
        }
      }
    }
    private void loadLeave()
    {
      string storeName = "spGNRLeaveInfo_Select";
      DBParameter[] param = new DBParameter[1];
      int EID = SharedObject.UserInfo.UserPid;
      param[0] = new DBParameter("@EID", DbType.Int32, EID);
      DataSet dsSource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);

      ultLeave.DataSource = dsSource.Tables[0];
      ultAL.DataSource = dsSource.Tables[1];
      ultCom.DataSource = dsSource.Tables[2];
      ultSo.DataSource = dsSource.Tables[3];
      ultAL.Rows.Band.Columns["Val"].CellAppearance.ForeColor = Color.Red;
      ultCom.Rows.Band.Columns["Val"].CellAppearance.ForeColor = Color.Blue;
      ultSo.Rows.Band.Columns["Val"].CellAppearance.ForeColor = Color.DarkGreen;

      //Bitmap b = new Bitmap(FunctionUtility.GetEmailIcon(4));
      //for (int i = 0; i < ultAtt.Rows.Count; i++)
      //{
      //  if (ultAtt.Rows[i].Cells["Holiday"].Value.ToString() != "0")
      //  {
      //    ultAtt.Rows[i].CellAppearance.BackColor = Color.Yellow;
      //  }
      //  else
      //  {
      //    //ultAtt.Rows[i].CellAppearance.BackColor = Color.White;
      //  }
      //}
    }
    private void loadAttendance()
    {
      string storeName = "spGNRDailyAttendance_Select";
      DBParameter[] param = new DBParameter[1];
      int EID = SharedObject.UserInfo.UserPid;
      param[0] = new DBParameter("@EID", DbType.Int32, EID);
      DataSet dsSource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      dsSource.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dsSource.Tables[0].Columns["WDate"],
                                                                                 dsSource.Tables[0].Columns["EID"]},
                                                  new DataColumn[] { dsSource.Tables[1].Columns["WDate"],  
                                                                                dsSource.Tables[1].Columns["EID"]}, false));
      ultAtt.DataSource = dsSource;
      Bitmap b = new Bitmap(FunctionUtility.GetEmailIcon(4));
      for (int i = 0; i < ultAtt.Rows.Count; i++)
      {
        if (ultAtt.Rows[i].Cells["Holiday"].Value.ToString() != "0" )
        {
          ultAtt.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (DBConvert.ParseDouble(ultAtt.Rows[i].Cells["NormalHour"].Value.ToString()) <= 0)
        {
          ultAtt.Rows[i].CellAppearance.BackColor = Color.LightGray;
        }
        else
        {
          //ultAtt.Rows[i].CellAppearance.BackColor = Color.White;
        }
      }
    }
    private void loadKPI()
    {
      string storeName = "spGNRKPIInfo_Select";
      DBParameter[] param = new DBParameter[1];
      int EID = SharedObject.UserInfo.UserPid;
      param[0] = new DBParameter("@EID", DbType.Int32, EID);
      DataSet dsSource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      ultKPI.DataSource = dsSource;
      //Bitmap b = new Bitmap(FunctionUtility.GetEmailIcon(4));
      //for (int i = 0; i < ultAtt.Rows.Count; i++)
      //{
      //  if (ultAtt.Rows[i].Cells["Holiday"].Value.ToString() != "0")
      //  {
      //    ultAtt.Rows[i].CellAppearance.BackColor = Color.Yellow;
      //  }
      //  else
      //  {
      //    //ultAtt.Rows[i].CellAppearance.BackColor = Color.White;
      //  }
      //}
    }
    private void LoadTask(bool IsRead)
    {
      try
      {
        string storeName = "spGNRTaskTransfer_Select";
        DBParameter[] param = new DBParameter[2];
        int EID = SharedObject.UserInfo.UserPid;
        param[0] = new DBParameter("@EID", DbType.Int32, EID);
        if (@IsRead)
        {
          param[1] = new DBParameter("@IsRead", DbType.Int32, 1);
        }
        else
        {
          param[1] = new DBParameter("@IsRead", DbType.Int32, 0);

        }
        DataSet dsSource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
        ultTaskSchedule.DataSource = dsSource;
        Bitmap b = new Bitmap(FunctionUtility.GetEmailIcon(3));
        for (int i = 0; i < ultTaskSchedule.Rows.Count; i++)
        {
          if (ultTaskSchedule.Rows[i].Cells["IsAttach"].Value.ToString() == "1")
          {
            ultTaskSchedule.Rows[i].Cells["Att"].Appearance.Image = b;
          }
        }
      }
      catch
      { }
    }
    //public void databaseFilePut(string varFilePath, long Pid)
    //{
    //  byte[] file;
    //  using (FileStream stream = new FileStream(varFilePath, FileMode.Open, FileAccess.Read))
    //  {
    //    using (BinaryReader reader = new BinaryReader(stream))
    //    {
    //      file = reader.ReadBytes((int)stream.Length);
    //    }
    //  }
    //  DBParameter[] param = new DBParameter[2];
    //  param[0] = new DBParameter("@File", DbType.Binary, file.Length, file);
    //  param[1] = new DBParameter("@Pid", DbType.Int64, Pid);

    //  bool result = DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText("Update TblGNRTaskTransfer set FileData = @File WHERE Pid = @Pid", param);
    //}
    public MemoryStream databaseFileRead(string Pid)
    {
      string str = "Select Filedata from TblGNRTaskTransfer WHERE Pid = '"+ Pid +"'";
      DataTable dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(str, null);
      byte[] file = (byte[])dt.Rows[0][0];
      MemoryStream memoryStream = new MemoryStream();
      memoryStream.Write(file, 0, file.Length);
      return memoryStream;
    }
    private void ultTaskSchedule_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        UltraGridRow row = ultTaskSchedule.Selected.Rows[0];
        //cay moi
        DaiCo.Shared.MainUserControl uc = null;
        string typeName = row.Cells["TypeObject"].Value.ToString();
        Type tyView = Type.GetType(typeName, false, true);
        if (tyView != null)
        {
          object objView = System.Activator.CreateInstance(tyView);
          if (objView != null)
          {
            uc = (DaiCo.Shared.MainUserControl)objView;
          }
        }

        DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, row.Cells["Title"].Value.ToString(), false, ViewState.MainWindow, FormWindowState.Normal);
        FormSerialisor.Deserialise(uc, this.databaseFileRead(row.Cells["Pid"].Value.ToString()));
        Type type = uc.GetType();
        foreach (MethodInfo f in type.GetMethods())
        {
          if (f.Name == "Search")
          {
            object result = null;
            result = f.Invoke(uc, null);

          }
        }
        bool success = this.SaveTask(DBConvert.ParseLong(row.Cells["TaskTransferDetailPid"].Value.ToString()));
        if (success)
        {
          this.LoadTask(false);
        }
      }
      catch
      { 
      
      }
    }


    private void chkRead_CheckedChanged(object sender, EventArgs e)
    {
      this.LoadTask(chkRead.Checked);
      this.loadBirthday();
      this.loadAttendance();
      this.loadLeave();
      this.loadKPI();
    }
    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveTask()
    {
      if (ultTaskSchedule.Rows.Count > 0)
      {
        DataTable dtTask = ((DataSet) ultTaskSchedule.DataSource).Tables[0];
        for (int i = 0; i < dtTask.Rows.Count; i++)
        {
          DataRow rowInfo = dtTask.Rows[i];
          if (rowInfo.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[4];
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(rowInfo["TaskTransferDetailPid"].ToString()));
            inputParam[1] = new DBParameter("@IsRead", DbType.Int32, DBConvert.ParseInt(rowInfo["read"].ToString()));
            inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            if (DBConvert.ParseInt(rowInfo["TransferTo"].ToString()) > 0)
            {
              inputParam[3] = new DBParameter("@TransferEID", DbType.Int32, DBConvert.ParseInt(rowInfo["TransferTo"].ToString()));
            }
            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spGNRTaskTransferDetail_Update", inputParam, outputParam);
            long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
            if (result == 0)
            {
              return false;
            }
            //}
          }
        }
      }
      return true;
    }
    private bool SaveTask(long TransactionPid)
    {
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, TransactionPid);
      inputParam[1] = new DBParameter("@IsRead", DbType.Int32, 1);
      inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spGNRTaskTransferDetail_Update", inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result == 0)
      {
        return false;
      }
      return true;
    }
    private void ultTaskSchedule_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      try
      {
        e.Layout.ScrollStyle = ScrollStyle.Immediate;
        e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
        e.Layout.Scrollbars = Scrollbars.Both;
        e.Layout.AutoFitColumns = true;
        // Set Align
        for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          Type colType = e.Layout.Bands[0].Columns[i].DataType;
          if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
          {
            e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          }
        }
        for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          if (!(e.Layout.Bands[0].Columns[i].Key == "Read" || e.Layout.Bands[0].Columns[i].Key == "TransferTo"))
          {
            e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
          }
        }
        e.Layout.Bands[0].Columns["Pid"].Hidden = true;
        e.Layout.Bands[0].Columns["IsAttach"].Hidden = true;
        e.Layout.Bands[0].Columns["TaskTransferDetailPid"].Hidden = true;
        e.Layout.Bands[0].Columns["TypeObject"].Hidden = true;
        e.Layout.Bands[0].Columns["FileName"].Hidden = true;
        e.Layout.Bands[0].Columns["TransferTo"].ValueList = ultTransferEID;
        e.Layout.Bands[0].Columns["TransferTo"].CellAppearance.BackColor = Color.Yellow;
        e.Layout.Bands[0].Columns["Read"].CellAppearance.BackColor = Color.Yellow;
        Bitmap b;
        if (chkRead.Checked)
        {
          b = new Bitmap(FunctionUtility.GetEmailIcon(1));
        }
        else
        {
          b = new Bitmap(FunctionUtility.GetEmailIcon(2));
        }
        e.Layout.Bands[0].Columns["img"].CellAppearance.Image = b;
        e.Layout.Bands[0].Columns["img"].Width = 30;
        e.Layout.Bands[0].Columns["Att"].Width = 70;
        e.Layout.Bands[0].Columns["img"].Header.Caption = "";
        e.Layout.Bands[0].Columns["Att"].Header.Caption = "Attached";
        e.Layout.Bands[0].Columns["Read"].Width = 70;

        e.Layout.Bands[0].Columns["Read"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

        e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
        e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
        e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
        e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      }
      catch
      { 
        
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = this.SaveTask();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.LoadTask(chkRead.Checked);
    }

    private void ultTaskSchedule_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;

      if (columnName == "att")
      {
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0 && DBConvert.ParseInt(row.Cells["IsAttach"].Value.ToString()) > 0)
        {
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

          DataTable dtLocationFile = DataBaseAccess.SearchCommandTextDataTable("SELECT LocationFile	FROM TblGNRTaskTransferUpload WHERE TaskTransferPid = @Pid AND TypeUpload = 0", inputParam);
          if (dtLocationFile != null || dtLocationFile.Rows.Count > 0)
          {
            //
            for (int i = 0; i < dtLocationFile.Rows.Count; i++)
            {
              Process prc = new Process();
              string startupPath = System.Windows.Forms.Application.StartupPath;
              string folder = string.Format(@"{0}\Temporary", startupPath);
              if (!Directory.Exists(folder))
              {
                Directory.CreateDirectory(folder);
              }
              string locationFile = dtLocationFile.Rows[i]["LocationFile"].ToString();
              if (File.Exists(locationFile))
              {
                string newLocationFile = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(dtLocationFile.Rows[i]["LocationFile"].ToString()));
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


              try
              {
                prc.Start();
              }
              catch
              {
                DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
              }
            }
            e.Cancel = true;
            //
          }
        }
      }
      //
    }

    private void ultTaskSchedule_MouseClick(object sender, MouseEventArgs e)
    {
      try
      {
        UltraGridRow row = ultTaskSchedule.Selected.Rows[0];
        long lTaskTransfer = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, 16, lTaskTransfer) };

        DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spGNRTaskTransfer_List", inputParam);
        ultCommend.DataSource = dsMain.Tables[1];
        ultAttached.DataSource = dsMain.Tables[2];
       
      }
      catch
      {

      }
    }

    private void ultAttached_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Bands[0].Columns["FileName"].CellMultiLine = DefaultableBoolean.True;
      e.Layout.Override.RowSizing = RowSizing.AutoFree;
      e.Layout.AutoFitColumns = true;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].Key != "FileName")
        {
          e.Layout.Bands[0].Columns[i].Hidden = true;
        }
      }
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultCommend_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Bands[0].Columns["Description"].CellMultiLine = DefaultableBoolean.True;
      e.Layout.Override.RowSizing = RowSizing.AutoFree;
      e.Layout.AutoFitColumns = true;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].Key != "Description")
        {
          e.Layout.Bands[0].Columns[i].Hidden = true;
        }
      }
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultBirthday_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["IsPic"].Hidden = true;
      e.Layout.Bands[0].Columns["Pic"].Header.Caption = "";
      e.Layout.Bands[0].Columns["Pic"].AutoSizeMode = ColumnAutoSizeMode.None;
      e.Layout.Bands[0].Columns["Pic"].Width = 30;
      e.Layout.Bands[0].Columns["Birth"].AutoSizeMode = ColumnAutoSizeMode.None;
      e.Layout.Bands[0].Columns["Birth"].Width = 50;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultAtt_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["Holiday"].Hidden = true;
      e.Layout.Bands[0].Columns["EID"].Hidden = true;
      e.Layout.Bands[1].Columns["WDate"].Hidden = true;
      e.Layout.Bands[1].Columns["EID"].Hidden = true;

      e.Layout.Bands[0].Columns["NormalHour"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["Overtime"].CellAppearance.ForeColor = Color.DarkGreen;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultAL_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["Val"].AutoSizeMode = ColumnAutoSizeMode.None;
      e.Layout.Bands[0].Columns["Val"].Width = 40;
      e.Layout.Bands[0].Columns["WDate"].AutoSizeMode = ColumnAutoSizeMode.None;
      e.Layout.Bands[0].Columns["WDate"].Width = 70;
      e.Layout.Bands[0].Columns["WDate"].Format = ConstantClass.FORMAT_DATETIME;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultLeave_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["EID"].Hidden = true;
      e.Layout.Bands[0].Columns["Veteran"].CellAppearance.ForeColor = Color.Brown;
      e.Layout.Bands[0].Columns["TAnnual"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["TAnnual"].Header.Caption = "Total";
      e.Layout.Bands[0].Columns["TUAL"].Header.Caption = "Leave";
      e.Layout.Bands[0].Columns["TUAL"].CellAppearance.ForeColor = Color.DarkGreen;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.ForeColor = Color.Red;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultKPI_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.AutoFitColumns = true;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnChat_Click(object sender, EventArgs e)
    {
      if (!this.CheckOpened("frmMain"))
      {
        string IP = "";
        try
        {
          DBParameter[] arrParam1 = new DBParameter[1];
          IP = DataBaseAccess.ExecuteScalarCommandText("SELECT VALUE FROM TblBOMCodeMaster WHERE [GROUP] = 16017 AND CODE = 1", arrParam1).ToString();
        }
        catch
        {
          IP = "10.0.8.161";
        }
        frmMain frm = new frmMain();
        frm.UserName = SharedObject.UserInfo.EmpName.Split(' ')[SharedObject.UserInfo.EmpName.Split(' ').Length - 1] + "-" + SharedObject.UserInfo.Department + "-" + SharedObject.UserInfo.UserPid.ToString();
        frm.IP = IP;
        frm.Show();
      }
      this.CloseForm("frmLoginChat");
    }
    private void CloseForm(string name)
    {
      FormCollection fc = System.Windows.Forms.Application.OpenForms;

      foreach (Form frm in fc)
      {
        if (frm.Name == name)
        {
          frm.Close();
          return;
        }
      }
    }
    private bool CheckOpened(string name)
    {
      FormCollection fc = System.Windows.Forms.Application.OpenForms;

      foreach (Form frm in fc)
      {
        if (frm.Name == name)
        {
          frm.Activate();
          frm.WindowState = FormWindowState.Normal;
          return true;
        }
      }
      return false;
    }

    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      ultTaskSchedule.Visible = !chkHide.Checked;
    }
  }
}
