using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainBOM
{
    public partial class AutoUpdate : Form
    {
        string[] _files;
        //WebClient webClient;
        string strApiUpdate;
        public AutoUpdate(string[] files)
        {
            InitializeComponent();

            this._files = files;
            //webClient = new WebClient();
            strApiUpdate = System.Configuration.ConfigurationManager.AppSettings["ApiUpdate"];
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public static void appReloader()
        {
            //Start a new instance of the current program
            

            //close the current application process
            var process = Process.GetCurrentProcess();
            //Process.GetCurrentProcess().Kill();
            process.WaitForExit(1000);

            var startInfo = new System.Diagnostics.ProcessStartInfo("ERPSoftware.exe");

            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.CreateNoWindow = false;
            startInfo.RedirectStandardOutput = true;
            System.Diagnostics.Process.Start(startInfo);

            //Process.Start(Application.ExecutablePath);
        }
        private void DownloadSingleFile(string fileName)
        {
            var webClient = new WebClient();
            if (fileName == "ERPSoftware.exe")
                webClient.DownloadFileAsync(new Uri(strApiUpdate + "Update/" + fileName), Application.StartupPath + "/ERPSoftware(1).exe");
            else
            {
                //webClient.DownloadDataCompleted += (sender, eventArgs) =>
                //{
                //    byte[] fileData = eventArgs.Result;
                //    //did you receive the data successfully? Place your own condition here. 
                //    using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                //        fileStream.Write(fileData, 0, fileData.Length);
                //};
                //webClient.DownloadFileCompleted += wc_DownloadFileCompleted;
                webClient.DownloadFileAsync(new Uri(strApiUpdate + "Update/" + fileName), fileName);
            }   
        }

        //private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    progressBar.Value = e.ProgressPercentage;
        //}

        //private void Completed(object sender, AsyncCompletedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start("ERP.Main.exe");
        //    //MessageBox.Show("Download completed!");
        //}

        private async Task DownloadUpdatefileTask(string[] files)
        {
            progressBar.Maximum = this._files.Length;
            int i = 1;
            string path = Directory.GetCurrentDirectory();
            Action downloadaction = () => {
                foreach (string item in files)
                {
                    DownloadSingleFile(item);                    
                    setLabel1TextSafe(item);
                    System.Threading.Thread.Sleep(1);                    
                    setLoading(i);
                    //progressBar.PerformStep();
                    i++;
                }
            };
            Task task = new Task(downloadaction);
            task.Start();
            await task;            
        }
        protected async override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var taskdonload = DownloadUpdatefileTask(_files);
            //..
            Console.WriteLine("Lam gi do khi file dang tai");
            //..     
            await taskdonload.ContinueWith(abcd =>
            {
                if (this.InvokeRequired)
                    this.Invoke(new Action(() => this.Hide()));
                else
                    this.Hide();

                if (System.IO.File.Exists(Application.StartupPath + "/ERPSoftware(1).exe"))
                {
                    if (System.IO.File.Exists(Application.StartupPath + "/ERPSoftware(2).exe"))
                        System.IO.File.Delete(Application.StartupPath + "/ERPSoftware(2).exe");

                    System.IO.File.Move(Application.StartupPath + "/ERPSoftware.exe", Application.StartupPath + "/ERPSoftware(2).exe");

                    System.Threading.Thread.Sleep(2000);
                    System.IO.File.Move(Application.StartupPath + "/ERPSoftware(1).exe", Application.StartupPath + "/ERPSoftware.exe");
                    //System.IO.File.Delete(Application.StartupPath + "/ERPSoftware(2).exe");
                }
                Environment.SetEnvironmentVariable("Production_RESTART", "1");
                Application.Restart();
                
            });
            //this.Hide();
        }

        private void setLabel1TextSafe(string txt)
        {
            if (lblCurrentFile.InvokeRequired)
                lblCurrentFile.Invoke(new Action(() => lblCurrentFile.Text = txt));
            else
                lblCurrentFile.Text = txt;
        }

        private void setLoading(int i)
        {
            if (progressBar.InvokeRequired)
                progressBar.Invoke(new Action(() => setLoading(i)));
            else
                progressBar.Value = i;
        }

        private void AutoUpdate_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.SetEnvironmentVariable("Production_RESTART", "1");
            Application.Restart();
        }
    }
}
