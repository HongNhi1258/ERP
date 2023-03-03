/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 27/5/2016
  Description : 
  Standard Form: viewPLN_50_006.cs
*/
using DaiCo.Application;
using DaiCo.Application.Web.Mail;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;


namespace DaiCo.Planning
{
  public partial class viewPLN_50_006 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    public long pid = long.MinValue;
    private string _paraBreak = "\r\n\r\n";
    private string _link = "<a href=\"{0}\">{1}</a>";
    private string _linkNoFollow = "<a href=\"{0}\" rel=\"nofollow\">{1}</a>";
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
      this.LoadUltraCBGroup();
    }

    private void LoadUltraCBGroup()
    {
      string cm = "SELECT Pid,[Group] FROM TblPLNMailInfo";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultcbGroup, dt, "Group", "Group", "Pid");
      ultcbGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[1];
      if (ultcbGroup.Value != null)
      {
        inputParam[0] = new DBParameter("@Group", DbType.String, ultcbGroup.Value.ToString());
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMailInfo_Search", inputParam);
        if (dtSource != null && dtSource.Rows.Count > 0)
        {
          DataRow row = dtSource.Rows[0];
          txtMailCC.Text = row["MailCC"].ToString();
          txtMailTo.Text = row["MailTo"].ToString();
          txtBody.Text = row["Body"].ToString();
          txtSubject.Text = row["Subject"].ToString();
          txtGroup.Text = row["Group"].ToString();
        }
      }
      else
      {
        txtMailCC.Text = "";
        txtMailTo.Text = "";
        txtBody.Text = "";
        txtSubject.Text = "";
        txtGroup.Text = "";
      }
      this.NeedToSave = false;
    }



    /// <summary>
    /// Returns a copy of this string converted to HTML markup.
    /// </summary>
    private string ToHtml(string s)
    {
      return ToHtml(s, false);
    }

    /// <summary>
    /// Returns a copy of this string converted to HTML markup.
    /// </summary>
    /// <param name="nofollow">If true, links are given "nofollow"
    /// attribute</param>
    private string ToHtml(string s, bool nofollow)
    {
      StringBuilder sb = new StringBuilder();

      int pos = 0;
      while (pos < s.Length)
      {
        // Extract next paragraph
        int start = pos;
        pos = s.IndexOf(_paraBreak, start);
        if (pos < 0)
          pos = s.Length;
        string para = s.Substring(start, pos - start).Trim();

        // Encode non-empty paragraph
        if (para.Length > 0)
          EncodeParagraph(para, sb, nofollow);

        // Skip over paragraph break
        pos += _paraBreak.Length;
      }
      // Return result
      return sb.ToString();
    }

    /// <summary>
    /// Encodes a single paragraph to HTML.
    /// </summary>
    /// <param name="s">Text to encode</param>
    /// <param name="sb">StringBuilder to write results</param>
    /// <param name="nofollow">If true, links are given "nofollow"
    /// attribute</param>
    private void EncodeParagraph(string s, StringBuilder sb, bool nofollow)
    {
      // Start new paragraph
      sb.AppendLine("<p>");

      // HTML encode text
      s = System.Web.HttpUtility.HtmlEncode(s);

      // Convert single newlines to <br>
      s = s.Replace(Environment.NewLine, "<br />\r\n");

      // Encode any hyperlinks
      EncodeLinks(s, sb, nofollow);

      // Close paragraph
      sb.AppendLine("\r\n</p>");
    }

    /// <summary>
    /// Encodes [[URL]] and [[Text][URL]] links to HTML.
    /// </summary>
    /// <param name="text">Text to encode</param>
    /// <param name="sb">StringBuilder to write results</param>
    /// <param name="nofollow">If true, links are given "nofollow"
    /// attribute</param>
    private void EncodeLinks(string s, StringBuilder sb, bool nofollow)
    {
      // Parse and encode any hyperlinks
      int pos = 0;
      while (pos < s.Length)
      {
        // Look for next link
        int start = pos;
        pos = s.IndexOf("[[", pos);
        if (pos < 0)
          pos = s.Length;
        // Copy text before link
        sb.Append(s.Substring(start, pos - start));
        if (pos < s.Length)
        {
          string label, link;

          start = pos + 2;
          pos = s.IndexOf("]]", start);
          if (pos < 0)
            pos = s.Length;
          label = s.Substring(start, pos - start);
          int i = label.IndexOf("][");
          if (i >= 0)
          {
            link = label.Substring(i + 2);
            label = label.Substring(0, i);
          }
          else
          {
            link = label;
          }
          // Append link
          sb.Append(String.Format(nofollow ? _linkNoFollow : _link, link, label));

          // Skip over closing "]]"
          pos += 2;
        }
      }
    }


    #endregion function

    #region event
    public viewPLN_50_006()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_50_006_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      //if (ultcbGroup.Value == null)
      //{
      //  WindowUtinity.ShowMessageError("ERR0001", "Group");
      //  return;
      //}
      if (WindowUtinity.ShowMessageConfirmFromText("Do You Want To Send Email?").ToString() == "No")
      {
        return;
      }
      int flag = 0;
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNConfirmWOL_Report", inputParam);
      ds.Tables[0].Columns.Add("ImageLogo", typeof(System.Byte[]));
      ds.Tables[0].Rows[0]["ImageLogo"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFRLogo_New);
      DaiCo.Planning.DataSetFile.dsPLNConfirmWOL dsSource = new DaiCo.Planning.DataSetFile.dsPLNConfirmWOL();
      dsSource.Tables["ConfirmWOLInfo"].Merge(ds.Tables[0]);
      dsSource.Tables["ConfirmWOLDetail"].Merge(ds.Tables[1]);
    GoBack:
      for (int i = 0; i < dsSource.Tables["ConfirmWOLDetail"].Rows.Count; i++)
      {
        string imgPath = string.Empty;
        imgPath = FunctionUtility.BOMGetItemImage(dsSource.Tables["ConfirmWOLDetail"].Rows[i]["ItemCode"].ToString(), DBConvert.ParseInt(dsSource.Tables["ConfirmWOLDetail"].Rows[i]["Revision"].ToString()));
        if (flag == 0)
        {
          dsSource.Tables["ConfirmWOLDetail"].Rows[i]["Picture"] = FunctionUtility.ImagePathToByteArray(imgPath);
        }
        else
        {
          dsSource.Tables["ConfirmWOLDetail"].Rows[i]["Picture"] = DBNull.Value;
        }
      }
      string transationCode = dsSource.Tables["ConfirmWOLInfo"].Rows[0]["TransactionCode"].ToString();
      DaiCo.Planning.Reports.cptPLNConfirmWOL cpt = new DaiCo.Planning.Reports.cptPLNConfirmWOL();
      cpt.SetDataSource(dsSource);
      cpt.SetParameterValue("User", SharedObject.UserInfo.EmpName);
      //View_Report frm = new View_Report(cpt);
      //frm.ShowReport(ViewState.Window, FormWindowState.Maximized);

      string time = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\Reports";
      // Check Exist Folder
      if (!Directory.Exists(pathTemplate))
      {
        Directory.CreateDirectory(pathTemplate);
      }
      string path = string.Format(pathOutputFile + String.Format("\\TransactionReport{0}{1}.pdf", transationCode, DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd")) + time));
      string pathNew = string.Format(pathOutputFile + String.Format("\\TransactionReport{0}{1}.pdf", transationCode, DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd")) + time));
      try
      {
        cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path);
      }
      catch
      {
        flag = 1;
        goto GoBack;
      }

      MailMessage mailMessage = new MailMessage();

      string mailTo = string.Empty;
      string mailCC = string.Empty;

      if (txtMailTo.Text.Trim().Length > 0)
      {
        mailTo = txtMailTo.Text;
        mailTo = mailTo.Replace(";", ",");
      }

      if (txtMailCC.Text.Trim().Length > 0)
      {
        mailCC = txtMailCC.Text;
        mailCC = mailCC.Replace(";", ",");
      }

      //mailMessage.ServerName = "10.0.0.5";
      //mailMessage.Username = "dc@daico-furniture.com";
      //mailMessage.Password = "dc123456";
      //mailMessage.From = "dc@daico-furniture.com";

      mailMessage.ServerName = "10.0.0.6";
      mailMessage.Username = "dc@vfr.net.vn";
      mailMessage.Password = "dc123456";
      mailMessage.From = "dc@vfr.net.vn";

      mailMessage.To = mailTo;
      mailMessage.Cc = mailCC;
      if (txtSubject.Text.Trim().Length > 0)
      {
        mailMessage.Subject = txtSubject.Text;
      }

      string myString = txtBody.Text;

      if (txtBody.Text.Trim().Length > 0)
      {
        mailMessage.Body = this.ToHtml(myString);
      }

      IList attachments = new ArrayList();

      attachments.Add(pathNew);

      mailMessage.Attachfile = attachments;
      mailMessage.SendMail(true);

      //File.Delete(path);
      //File.Delete(pathNew);

      WindowUtinity.ShowMessageSuccess("MSG0052");
      this.CloseTab();
    }

    private void ultcbGroup_ValueChanged(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_50_005 view = new viewPLN_50_005();
      WindowUtinity.ShowView(view, "New Mail Info", true, ViewState.Window);
    }
    private void btnEdit_Click(object sender, EventArgs e)
    {
      if (ultcbGroup.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Group");
        return;
      }
      viewPLN_50_005 view = new viewPLN_50_005();
      view.pid = DBConvert.ParseLong(ultcbGroup.SelectedRow.Cells["Pid"].Value.ToString());
      WindowUtinity.ShowView(view, "Update Mail Info", true, ViewState.Window);
      this.CloseTab();
    }
    #endregion event
  }
}
