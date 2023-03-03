using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class ProgressForm : Form
  {
    BackgroundWorker worker;
    int lastPercent;
    string lastStatus;
    public ProgressBar ProgressBar
    {
      get
      {
        return progress;
      }
    }
    public object Argument;// { get; set; }
    public RunWorkerCompletedEventArgs Result;// { get; private set; }
    public bool CancellationPending
    {
      get
      {
        return worker.CancellationPending;
      }
    }
    public string CancellingText;//{ get; set; }
    public string DefaultStatusText;// { get; set; }
    public delegate void DoWorkEventHandler(ProgressForm sender, DoWorkEventArgs e);
    public event DoWorkEventHandler DoWork;
    public ProgressForm()
    {
      InitializeComponent();

      DefaultStatusText = "Please wait...";
      CancellingText = "Cancelling operation...";

      worker = new BackgroundWorker();
      worker.WorkerReportsProgress = true;
      worker.WorkerSupportsCancellation = true;
      worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
      worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
      worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
    }
    public void SetProgress(string status)
    {
      //do not update the text if it didn't change
      //or if a cancellation request is pending
      if (status != lastStatus && !worker.CancellationPending)
      {
        lastStatus = status;
        worker.ReportProgress(progress.Minimum - 1, status);
      }
    }
    public void SetProgress(int percent)
    {
      //do not update the progress bar if the value didn't change
      if (percent != lastPercent)
      {
        lastPercent = percent;
        worker.ReportProgress(percent);
      }
    }
    public void SetProgress(int percent, string status)
    {
      //update the form is at least one of the values need to be updated
      if (percent != lastPercent || (status != lastStatus && !worker.CancellationPending))
      {
        lastPercent = percent;
        lastStatus = status;
        worker.ReportProgress(percent, status);
      }
    }
    private void ProgressForm_Load(object sender, EventArgs e)
    {
      //reset to defaults just in case the user wants to reuse the form
      Result = null;
      //buttonCancel.Enabled = true;
      progress.Value = progress.Minimum;
      labelStatus.Text = DefaultStatusText;
      lastStatus = DefaultStatusText;
      lastPercent = progress.Minimum;
      //start the background worker as soon as the form is loaded
      worker.RunWorkerAsync(Argument);
    }

    void worker_DoWork(object sender, DoWorkEventArgs e)
    {
      //the background worker started
      //let's call the user's event handler
      if (DoWork != null)
        DoWork(this, e);
    }
    void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      //make sure the new value is valid for the progress bar and update it
      if (e.ProgressPercentage >= progress.Minimum && e.ProgressPercentage <= progress.Maximum)
        progress.Value = e.ProgressPercentage;
      //do not update the text if a cancellation request is pending
      if (e.UserState != null && !worker.CancellationPending)
        labelStatus.Text = e.UserState.ToString();
    }
    void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      //the background worker completed
      //keep the resul and close the form
      Result = e;
      if (e.Error != null)
        DialogResult = DialogResult.Abort;
      else if (e.Cancelled)
        DialogResult = DialogResult.Cancel;
      else
        DialogResult = DialogResult.OK;
      Close();
    }

  }
}
