using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Proshot.CommandClient;
using System.Net;
using System.Runtime.InteropServices;


namespace DaiCo.General
{
    internal enum FlashMode
    {
        FLASHW_CAPTION = 0x1 ,
        FLASHW_TRAY = 0x2 ,
        FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY
    }

    internal struct FlashInfo
    {
        public int cdSize;
        public System.IntPtr hwnd;
        public int dwFlags;
        public int uCount;
        public int dwTimeout;

    }

    public partial class frmPrivate : Form
    {

        private CMDClient remoteClient;
        private IPAddress targetIP;
        private string remoteName;
        private bool activated;
        public string RemoteName
        {
            get { return this.remoteName; }
        }
        protected override bool ProcessCmdKey(ref Message msg , Keys keyData)
        {
            if ( keyData == Keys.Enter )
                this.SendMessage();
            if ( this.txtMessages.Focused && !ShareUtils.IsValidKeyForReadOnlyFields(keyData) )
                return true;
            return base.ProcessCmdKey(ref msg , keyData);
        }
      //bool isInit = true;
        public frmPrivate(CMDClient cmdClient,IPAddress friendIP,string name,string initialMessage)
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Minimized;
            this.remoteClient = cmdClient;
            this.targetIP = friendIP;
            this.remoteName = name;
            this.Text += " With " + name;
            this.txtMessages.Text = this.remoteName + ": " + initialMessage + Environment.NewLine;
            this.Flash(this.Handle, FlashMode.FLASHW_ALL, 3);  
          //if (this.isInit)
            //{
            //  this.WindowState = FormWindowState.Normal;
            //  this.WindowState = FormWindowState.Minimized;
            //}
            //else
            //{
            //  this.WindowState = FormWindowState.Normal;
            //}
            this.remoteClient.CommandReceived += new CommandReceivedEventHandler(private_CommandReceived);
            //this.isInit = false;
        }

        public frmPrivate(CMDClient cmdClient , IPAddress friendIP , string name)
        {
            InitializeComponent();
            this.remoteClient = cmdClient;
            this.targetIP = friendIP;
            this.remoteName = name;
            this.Text += " With " + name;
            this.remoteClient.CommandReceived += new CommandReceivedEventHandler(private_CommandReceived);
        }

        private void private_CommandReceived(object sender , CommandEventArgs e)
        {
            switch ( e.Command.CommandType )
            {
                case ( CommandType.Message ):
                    if ( !e.Command.Target.Equals(IPAddress.Broadcast) && e.Command.SenderIP.Equals(this.targetIP))
                    {
                      Font f = new Font(new FontFamily("Tahoma"), float.Parse("8.25", null), FontStyle.Bold);
                      this.AppendText(txtMessages, e.Command.SenderName.ToString(), Color.DarkBlue, f);
                      f = new Font(new FontFamily("Tahoma"), float.Parse("8.25", null), FontStyle.Regular);
                        //this.txtMessages.Text += ": " + e.Command.MetaData + Environment.NewLine;
                      this.AppendText(txtMessages, ": " + e.Command.MetaData + Environment.NewLine, Color.Black, f);

                        if ( !this.activated)
                        {
                          if (this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
                            ShareUtils.PlaySound(ShareUtils.SoundType.NewMessageReceived);
                          else
                            ShareUtils.PlaySound(ShareUtils.SoundType.NewMessageWithPow);
                            this.Flash(this.Handle , FlashMode.FLASHW_ALL , 3);
                        }
                    }
                    break;
            }    
        }

        [DllImport("user32.dll")]
        private static extern int FlashWindowEx(ref FlashInfo pfwi);
        private void Flash(System.IntPtr hwnd , FlashMode flashMode , int times)
        {
            //unsafe
            //{
            //    FlashInfo FlashInf = new FlashInfo();
            //    FlashInf.cdSize = sizeof(FlashInfo);
            //    FlashInf.dwFlags = (int)flashMode;
            //    FlashInf.dwTimeout = 0;
            //    FlashInf.hwnd = hwnd;
            //    FlashInf.uCount = times;
            //    FlashWindowEx(ref FlashInf);
            //}
        }

        private void btnSend_Click(object sender , EventArgs e)
        {
            this.SendMessage();
        }

      private void SendMessage()
      {
        if (this.remoteClient.Connected && this.txtNewMessage.Text.Trim() != "")
        {
          this.remoteClient.SendCommand(new Proshot.CommandClient.Command(Proshot.CommandClient.CommandType.Message, this.targetIP, this.txtNewMessage.Text));
          //this.txtMessages.Text += this.remoteClient.NetworkName + ": " + this.txtNewMessage.Text.Trim() + Environment.NewLine;
          Font f = new Font(new FontFamily("Tahoma"), float.Parse("8.25", null), FontStyle.Bold);
          this.AppendText(txtMessages, this.remoteClient.NetworkName, Color.Gray, f);
          f = new Font(new FontFamily("Tahoma"), float.Parse("8.25", null), FontStyle.Regular);
          this.AppendText(txtMessages, ": " + this.txtNewMessage.Text.Trim() + Environment.NewLine, Color.Black, f);

          this.txtNewMessage.Text = "";
          this.txtNewMessage.Focus();
        }
      }

        private void frmPrivate_FormClosing(object sender , FormClosingEventArgs e)
        {
            this.remoteClient.CommandReceived -= new CommandReceivedEventHandler(private_CommandReceived);
        }

        private void mniExit_Click(object sender , EventArgs e)
        {
            this.Close();
        }

        private void mniSave_Click(object sender , EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "HTML Files(*.HTML;*.HTM)|*.html;*.htm";
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            saveDlg.CheckPathExists = true;
            saveDlg.OverwritePrompt = true;
            saveDlg.FileName = this.Text;
            if ( saveDlg.ShowDialog() == DialogResult.OK )
                ShareUtils.SaveAsHTML(saveDlg.FileName , this.txtMessages.Lines , this.Text);
        }

        private void frmPrivate_Load(object sender , EventArgs e)
        {
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width , Screen.PrimaryScreen.WorkingArea.Height - this.DesktopBounds.Height);
        }

        private void frmPrivate_Activated(object sender , EventArgs e)
        {
            this.activated = true;
            this.CenterToParent();   
        }

        private void frmPrivate_Deactivate(object sender , EventArgs e)
        {
          this.activated = false;
        }
      private void AppendText(RichTextBox box, string text, Color color, Font f)
      {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;
        //StringBuilder str = new StringBuilder();
        //String.Format( 
        box.SelectionFont = f;
        box.SelectionColor = color;
        box.AppendText(text);
        //box.SelectionColor = box.ForeColor;
      }
    }
}