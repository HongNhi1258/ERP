/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace DaiCo.Application.Web.Mail
{
  /// <summary>
  /// Summary description for MailMessage.
  /// </summary>
  public class MailMessage
  {
    #region Fields

    private string serverName;
    private string username;
    private string password;
    private string subject;
    private string body;
    private string from;
    private string to;
    private string cc;
    private string bcc;
    private string logFile;
    private string filepath;
    private IList attachfile;

    #endregion Fields

    #region Contructors
    public MailMessage()
    {
      this.serverName = string.Empty;
      this.username = string.Empty;
      this.password = string.Empty;
      this.body = string.Empty;
      this.from = string.Empty;
      this.to = string.Empty;
      this.cc = string.Empty;
      this.bcc = string.Empty;
      this.subject = string.Empty;
      this.logFile = string.Empty;
      this.filepath = string.Empty;
      this.attachfile = null;
    }
    public MailMessage(string serverName, string username, string password, string from, string to, string cc, string bcc, string subject, string body, string filepath, IList attachfile)
    {
      this.serverName = serverName;
      this.username = username;
      this.password = password;
      this.body = body;
      this.from = from;
      this.to = to;
      this.cc = cc;
      this.bcc = bcc;
      this.subject = subject;
      this.filepath = filepath;
      this.attachfile = attachfile;
    }
    #endregion Contructors

    #region Methods

    public bool SendMail()
    {
      this.ServerName = "10.0.0.6";
      this.Username = "dc@vfr.net.vn";
      this.Password = "dc123456";
      this.From = "dc@vfr.net.vn";

      SmtpClient client = new SmtpClient();
      client.Timeout = 60000;
      client.Host = this.serverName;
      client.Port = 25;
      NetworkCredential cred = new NetworkCredential(this.username, this.password);
      client.Credentials = cred;

      // Make an mail message to send over smtp client
      System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
      if (this.from != string.Empty)
        mailMessage.From = new MailAddress(this.from);
      if (this.to != string.Empty)
        mailMessage.To.Add(this.to);
      if (this.cc != string.Empty)
        mailMessage.CC.Add(this.cc);
      if (this.bcc != string.Empty)
        mailMessage.Bcc.Add(this.bcc);
      mailMessage.Subject = this.subject;
      mailMessage.Body = this.body;

      for (int i = 0; i < attachfile.Count; i++)
      {
        //Attachment attachFile11 = new Attachment(attachfile[i].ToString());
        //mailMessage.Attachments.Add(attachFile11);

        // Create  the file attachment for this e-mail message.
        Attachment data = new Attachment(attachfile[i].ToString(), MediaTypeNames.Application.Octet);
        // Add time stamp information for the file.
        ContentDisposition disposition = data.ContentDisposition;
        disposition.CreationDate = System.IO.File.GetCreationTime(attachfile[i].ToString());
        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachfile[i].ToString());
        disposition.ReadDate = System.IO.File.GetLastAccessTime(attachfile[i].ToString());
        // Add the file attachment to this e-mail message.
        mailMessage.Attachments.Add(data);

      }

      //mailMessage.Attachments.Add(attachfile);

      mailMessage.IsBodyHtml = false;
      mailMessage.Priority = MailPriority.High;
      mailMessage.BodyEncoding = Encoding.UTF8;
      mailMessage.Headers.Add("X-Company", "K&G's Company");
      mailMessage.Headers.Add("X-Location", "Vietnamese");
      try
      {
        client.Send(mailMessage);
        mailMessage.Dispose();
        return true;
      }
      catch (Exception e)
      {
        TransactionException te = new TransactionException("Send mail", e);
        ILogging logging = new DailyLogging(this.logFile, new TraceLogCreator()); ;
        ILog log = logging.GetLog();
        lock (log)
        {
          log.Open();
          log.WriteLine("Send Mail To <" + mailMessage.To + "> failed.");
          log.Write(te.InnerException);
          log.WriteLine("---------------------\n");
          log.Close();
        }
        return false;
      }
    }

    public bool SendMail(bool isIsBodyHtml)
    {
      this.ServerName = "10.0.0.6";
      this.Username = "dc@vfr.net.vn";
      this.Password = "dc123456";
      this.From = "dc@vfr.net.vn";

      SmtpClient client = new SmtpClient();
      client.Timeout = 60000;
      client.Host = this.serverName;
      client.Port = 25;
      NetworkCredential cred = new NetworkCredential(this.username, this.password);
      client.Credentials = cred;

      // Make an mail message to send over smtp client
      System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
      if (this.from != string.Empty)
        mailMessage.From = new MailAddress(this.from);
      if (this.to != string.Empty)
        mailMessage.To.Add(this.to);
      if (this.cc != string.Empty)
        mailMessage.CC.Add(this.cc);
      if (this.bcc != string.Empty)
        mailMessage.Bcc.Add(this.bcc);
      mailMessage.Subject = this.subject;
      mailMessage.Body = this.body;

      for (int i = 0; i < attachfile.Count; i++)
      {
        //Attachment attachFile11 = new Attachment(attachfile[i].ToString());
        //mailMessage.Attachments.Add(attachFile11);

        // Create  the file attachment for this e-mail message.
        Attachment data = new Attachment(attachfile[i].ToString(), MediaTypeNames.Application.Octet);
        // Add time stamp information for the file.
        ContentDisposition disposition = data.ContentDisposition;
        disposition.CreationDate = System.IO.File.GetCreationTime(attachfile[i].ToString());
        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachfile[i].ToString());
        disposition.ReadDate = System.IO.File.GetLastAccessTime(attachfile[i].ToString());
        // Add the file attachment to this e-mail message.
        mailMessage.Attachments.Add(data);

      }

      //mailMessage.Attachments.Add(attachfile);

      mailMessage.IsBodyHtml = isIsBodyHtml;
      mailMessage.Priority = MailPriority.High;
      mailMessage.BodyEncoding = Encoding.UTF8;
      //mailMessage.Headers.Add("X-Company", "K&G's Company");
      //mailMessage.Headers.Add("X-Location", "Vietnamese");
      try
      {
        client.Send(mailMessage);
        mailMessage.Dispose();
        return true;
      }
      catch (Exception e)
      {
        TransactionException te = new TransactionException("Send mail", e);
        ILogging logging = new DailyLogging(this.logFile, new TraceLogCreator()); ;
        ILog log = logging.GetLog();
        lock (log)
        {
          log.Open();
          log.WriteLine("Send Mail To <" + mailMessage.To + "> failed.");
          log.Write(te.InnerException);
          log.WriteLine("---------------------\n");
          log.Close();
        }
        return false;
      }
    }
    #endregion Methods

    #region Properties
    public string ServerName
    {
      get
      {
        return this.serverName;
      }
      set
      {
        this.serverName = value;
      }
    }
    public string Username
    {
      get
      {
        return this.username;
      }
      set
      {
        this.username = value;
      }
    }
    public IList Attachfile
    {
      get
      {
        return this.attachfile;
      }
      set
      {
        this.attachfile = value;
      }
    }
    public string Password
    {
      get
      {
        return this.password;
      }
      set
      {
        this.password = value;
      }
    }
    public string From
    {
      get
      {
        return this.from;
      }
      set
      {
        this.from = value;
      }
    }
    public string To
    {
      get
      {
        return this.to;
      }
      set
      {
        this.to = value;
      }
    }
    public string Subject
    {
      get
      {
        return this.subject;
      }
      set
      {
        this.subject = value;
      }
    }
    public string Body
    {
      get
      {
        return this.body;
      }
      set
      {
        this.body = value;
      }
    }
    public string Bcc
    {
      get
      {
        return this.bcc;
      }
      set
      {
        this.bcc = value;
      }
    }
    public string Cc
    {
      get
      {
        return this.cc;
      }
      set
      {
        this.cc = value;
      }
    }
    public string LogFile
    {
      get
      {
        return this.logFile;
      }
      set
      {
        this.logFile = value;
      }
    }
    public string FilePath
    {
      get
      {
        return this.filepath;
      }
      set
      {
        this.filepath = value;
      }
    }
    #endregion Properties
  }
}
