using System;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace MainBOM
{
    public static class Program
    {
        public static frmERPMain mainForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool firstInstance;
            var wasRestarted = Environment.GetEnvironmentVariable("Production_RESTART");
            Mutex mutex = new Mutex(false, "ERPSystem", out firstInstance);
            if (!firstInstance && !(wasRestarted=="1"))
            {
                DaiCo.Shared.Utility.WindowUtinity.ShowMessageErrorFromText("ERP system has already run !");
                Application.Exit();
            }
            else
            {
                Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                /* ERP */

                //string[] fileEntries = System.IO.Directory.GetFiles(Environment.CurrentDirectory, "*", System.IO.SearchOption.AllDirectories);
                //System.Text.StringBuilder strFiles = new System.Text.StringBuilder();
                //foreach (string item in fileEntries)
                //{
                //    var item1 = item.Replace(Environment.CurrentDirectory+"\\", "");
                //    strFiles.AppendLine(item1);
                //}
                string[] files;
                if (ChecAppVersion4Update(out files))
                {
                    //Application.Run(new AutoUpdate(files));
                }
                else
                {
                    mainForm = new frmERPMain();
                    Application.Run(mainForm);
                }
                /* Packing */
                //Form testForm = new Form();
                //testForm.Controls.Add(new DaiCo.ERPProject.viewHRD_10_001());
                //testForm.Controls[0].Dock = DockStyle.Fill;
                //Application.Run(testForm);
            }
        }
        private static bool ChecAppVersion4Update(out string[] files)
        {
            
            //hainm 2022 - 11 - 22
            if (System.Diagnostics.Debugger.IsAttached)
            {
                files = null;
                return false;
            }

            files = null;
            return false;

            var strApiUpdate = ConfigurationManager.AppSettings["ApiUpdate"];
            string LocalVersion = Application.ProductVersion;

            WebClient client = new WebClient();
            string remoteVersion = client.DownloadString(strApiUpdate + "AppVersion.txt");
            if (LocalVersion != remoteVersion)
            {
                string strFile = client.DownloadString(strApiUpdate + "listFileDownload.txt");
                files = strFile.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                return true;
            }
            files = null;
            return false;
        }
    }
}