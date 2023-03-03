using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_006 : MainUserControl
  {
    public string itemCode;
    public int revision;
    private string revisionFilePath = FunctionUtility.GetImagePathByPid(8);
    private int revisionPid = int.MinValue;

    public viewBOM_01_006()
    {
      InitializeComponent();
    }
    private void LoadDropdown()
    {
      Utility.LoadComboboxCodeMst(drChangeKind, ConstantClass.GROUP_CHANGEKIND);
    }
    private void LoadData()
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserName + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;
      BOMRevision objRevision = new BOMRevision();
      objRevision.ItemCode = this.itemCode;
      objRevision.Revision = this.revision;
      objRevision = (BOMRevision)DataBaseAccess.LoadObject(objRevision, new string[] { "ItemCode", "Revision" });
      if (objRevision != null)
      {
        this.revisionPid = objRevision.PID;
      }
      txtItemCode.Text = this.itemCode;
      txtRevision.Text = this.revision.ToString();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataTable dtRevisionRecord = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListRevisionRecords", inputParam);
      dgvRevisionRecord.DataSource = dtRevisionRecord;

      dgvRevisionRecord.DisplayLayout.Bands[0].Columns["Linked"].Header.Caption = "File";
      dgvRevisionRecord.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      dgvRevisionRecord.DisplayLayout.Bands[0].Columns["ChangeKind"].Hidden = true;

    }
    private void viewBOM_01_006_Load(object sender, EventArgs e)
    {
      this.LoadDropdown();
      this.LoadData();
    }

    private void btnBrowser_Click(object sender, EventArgs e)
    {
      OpenFileDialog aOpenFileDialog = new OpenFileDialog();
      aOpenFileDialog.Filter = "Files (*.pdf;*.doc;*.txt;*.jpg;*.wmf;*.xls;)|*.pdf;*.doc;*.txt;*.jpg;*.wmf;*.xls; | All Files (*.*)|*.*";
      aOpenFileDialog.ShowReadOnly = true;
      aOpenFileDialog.Multiselect = false;
      aOpenFileDialog.Title = "Open image files";
      if (aOpenFileDialog.ShowDialog() == DialogResult.OK)
      {
        //Do something useful with aOpenFileDialog.FileName 
        //or aOpenFileDialog.FileNames
        txtLinkFile.Text = aOpenFileDialog.FileName.Trim().ToString();
      }
      aOpenFileDialog.Dispose();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[6];
      int changeType = int.MinValue;
      inputParam[0] = new DBParameter("@RevisionPID", DbType.Int32, this.revisionPid);
      try
      {
        changeType = DBConvert.ParseInt(drChangeKind.SelectedValue.ToString());
        if (changeType == int.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", new string[] { "Change type" });
          return;
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "Change type" });
        return;
      }
      inputParam[1] = new DBParameter("@ChangeKind", DbType.Int32, changeType);
      //file
      string StrFileName = txtLinkFile.Text;
      string strFTpye = System.IO.Path.GetExtension(StrFileName), strFName = System.IO.Path.GetFileNameWithoutExtension(StrFileName);
      string serverPath = revisionFilePath;
      BOMCodeMaster objCMaster = new BOMCodeMaster();
      objCMaster.Code = changeType;
      objCMaster.Group = 4;
      objCMaster = (BOMCodeMaster)DataBaseAccess.LoadObject(objCMaster, new string[] { "Group", "Code" });
      System.IO.Directory.CreateDirectory(serverPath + "\\" + txtItemCode.Text);
      serverPath = serverPath + "\\" + txtItemCode.Text + "\\";
      System.IO.Directory.CreateDirectory(serverPath + txtRevision.Text.PadLeft(2, '0'));
      serverPath = serverPath + txtRevision.Text.PadLeft(2, '0') + "\\";
      StrFileName = strFName + "-" + txtItemCode.Text + "-" + txtRevision.Text.PadLeft(2, '0') + "-" + objCMaster.Value + strFTpye;

      inputParam[2] = new DBParameter("@Linked", DbType.String, 256, serverPath + StrFileName);

      inputParam[3] = new DBParameter("@Note", DbType.String, 256, txtNote.Text);
      inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spBOMRevisionDetail_Insert", inputParam, outputParam);
      if (DBConvert.ParseInt(outputParam[0].Value.ToString()) != 0)
      {
        BOMRevisionDetail obj = new BOMRevisionDetail();
        obj.PID = Convert.ToInt32(outputParam[0].Value);
        obj = (BOMRevisionDetail)DataBaseAccess.LoadObject(obj, new string[] { "PID" });
        inputParam[0] = new DBParameter("@PID", DbType.Int32, obj.PID);
        StrFileName = strFName + "-" + txtItemCode.Text + "-" + txtRevision.Text.PadLeft(2, '0') + "-" + objCMaster.Value + "-" + outputParam[0].Value.ToString() + strFTpye;
        BOMRevision objRevision = new BOMRevision();
        objRevision.ItemCode = itemCode;
        objRevision.Revision = revision;
        objRevision = (BOMRevision)DataBaseAccess.LoadObject(objRevision, new string[] { "ItemCode", "Revision" });
        inputParam[1] = new DBParameter("@RevisionPID", DbType.Int32, this.revisionPid);
        inputParam[2] = new DBParameter("@Linked", DbType.String, 256, serverPath + StrFileName);
        inputParam[3] = new DBParameter("@ChangeKind", DbType.Int32, obj.ChangeKind);
        inputParam[4] = new DBParameter("@Note", DbType.String, obj.Note);
        inputParam[5] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

        DataBaseAccess.ExecuteStoreProcedure("spBOMRevisionDetail_Update", inputParam, outputParam);
        try
        {
          System.IO.File.Copy(txtLinkFile.Text, serverPath + StrFileName, true);
        }
        catch { }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0003");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0003");
        return;
      }
      this.LoadData();
      btnSave.Enabled = true;
      btnUpdate.Enabled = false;
      btnDelete.Enabled = false;
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[6];
      inputParam[0] = new DBParameter("@PID", DbType.Int32, txtPID.Text);
      BOMRevision objRevision = new BOMRevision();
      objRevision.ItemCode = itemCode;
      objRevision.Revision = revision;
      objRevision = (BOMRevision)DataBaseAccess.LoadObject(objRevision, new string[] { "ItemCode", "Revision" });
      if (objRevision != null)
      {
        int changeType = int.MinValue;
        inputParam[1] = new DBParameter("@RevisionPID", DbType.Int32, this.revisionPid);
        try
        {
          changeType = DBConvert.ParseInt(drChangeKind.SelectedValue.ToString());
          if (changeType == int.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0001", new string[] { "Change type" });
            return;
          }
        }
        catch
        {
          WindowUtinity.ShowMessageError("ERR0001", new string[] { "Change type" });
          return;
        }
        inputParam[2] = new DBParameter("@ChangeKind", DbType.Int32, changeType);

        //file 
        string StrFileName = txtLinkFile.Text, strFile = txtLinkFile.Text;

        if (!txtFilePathOld.Text.Equals(txtLinkFile.Text))
        {
          string strFTpye = System.IO.Path.GetExtension(StrFileName), strFName = System.IO.Path.GetFileNameWithoutExtension(StrFileName);
          string serverPath = revisionFilePath;
          System.IO.Directory.CreateDirectory(serverPath + "\\" + txtItemCode.Text);
          serverPath = serverPath + "\\" + txtItemCode.Text + "\\";
          System.IO.Directory.CreateDirectory(serverPath + txtRevision.Text.PadLeft(2, '0'));
          serverPath = serverPath + txtRevision.Text.PadLeft(2, '0') + "\\";

          BOMCodeMaster objCMaster = new BOMCodeMaster();
          objCMaster.Code = Convert.ToInt32(changeType);
          objCMaster.Group = 4;
          objCMaster = (BOMCodeMaster)DataBaseAccess.LoadObject(objCMaster, new string[] { "Group", "Code" });

          StrFileName = strFName + "-" + txtItemCode.Text + "-" + txtRevision.Text.PadLeft(2, '0') + "-" + objCMaster.Value + "-" + txtPID.Text + strFTpye;
          strFile = serverPath + StrFileName;
          System.IO.File.Delete(txtFilePathOld.Text);
          System.IO.File.Copy(txtLinkFile.Text, strFile, true);
        }
        inputParam[3] = new DBParameter("@Linked", DbType.String, 256, strFile);

        inputParam[4] = new DBParameter("@Note", DbType.String, 256, txtNote.Text);
        inputParam[5] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMRevisionDetail_Update", inputParam, outputParam);
        if (DBConvert.ParseInt(outputParam[0].Value.ToString()) != 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0001");
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0002");
          return;
        }
        this.LoadData();
        btnSave.Enabled = true;
        btnUpdate.Enabled = false;
        btnDelete.Enabled = false;
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      System.IO.File.Delete(txtFilePathOld.Text);
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@PID", DbType.Int32, txtPID.Text);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spBOMRevisionDetail_Delete", inputParam, outputParam);
      if (DBConvert.ParseInt(outputParam[0].Value.ToString()) != 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0002");
      }
      this.LoadData();
      btnSave.Enabled = true;
      btnUpdate.Enabled = false;
      btnDelete.Enabled = false;
    }

    private void dgvRevisionRecord_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
      {
        return;
      }
      try
      {
        int index = 0;
        txtPID.Text = dgvRevisionRecord.Selected.Rows[index].Cells["Pid"].Value.ToString();
        txtLinkFile.Text = dgvRevisionRecord.Selected.Rows[index].Cells["Linked"].Value.ToString();
        txtFilePathOld.Text = txtLinkFile.Text;
        txtRevision.Text = dgvRevisionRecord.Selected.Rows[index].Cells["Revision"].Value.ToString();
        try
        {
          drChangeKind.SelectedValue = dgvRevisionRecord.Selected.Rows[index].Cells["ChangeKind"].Value.ToString();
        }
        catch
        {
        }
        txtNote.Text = dgvRevisionRecord.Selected.Rows[index].Cells["Note"].Value.ToString();
        btnSave.Enabled = false;
        btnUpdate.Enabled = true;
        btnDelete.Enabled = true;
      }
      catch { }

    }

    private void dgvRevisionRecord_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Bands[0].Columns["Open"].Header.Caption = "View File";
      e.Layout.Bands[0].Columns["Open"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["Open"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellAppearance.AlphaLevel = 192;

      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Override.HeaderAppearance.AlphaLevel = 192;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;

      foreach (Infragistics.Win.UltraWinGrid.UltraGridBand oBand in this.dgvRevisionRecord.DisplayLayout.Bands)
      {
        foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oColumn in oBand.Columns)
        {

          if (oColumn.DataType.ToString() == "System.Double")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          }
          if (oColumn.DataType.ToString() == "System.DateTime")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            oColumn.Format = "dd/MM/yyyy";
          }
        }
      }
    }

    private void dgvRevisionRecord_ClickCellButton(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString();
      switch (columnName)
      {
        case "Open":
          Process prc = new Process();
          prc.StartInfo = new ProcessStartInfo(dgvRevisionRecord.Rows[index].Cells["Linked"].Value.ToString());
          try
          {
            prc.Start();
          }
          catch
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
          }
          break;
      }
    }
  }
}