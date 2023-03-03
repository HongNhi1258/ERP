
#region Using Directives
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Update.Commons;
#endregion

namespace MainBOM
{
  /// <summary>
  ///		Utility that provides update functionalities
  /// </summary>
  public class UpdateUtil
  {
    /// <summary>
    /// 
    /// </summary>
    private string remoteObjectUri = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    private IUpdateService remoteService;

    private IWin32Window owner;

    private string applicationName;

    /// <summary>
    ///		Creates a new instance
    /// </summary>
    public UpdateUtil(IWin32Window owner, string remoteObjectUri)
    {
      remoteService = null;
      this.owner = owner;
      this.remoteObjectUri = remoteObjectUri;
    }

    /// <summary>
    ///		Connect to the remote server
    /// </summary>
    /// <remarks>
    ///		Tries to create a WKO Instance
    /// </remarks>
    /// <returns>
    ///		<c>true</c> if the connection establishes 
    ///	successfully, <c>false</c> otherwise.
    /// </returns>
    private bool ConnectRemoteServer()
    {
      try
      {
        remoteService =
          Activator.GetObject(typeof(IUpdateService), remoteObjectUri)
          as IUpdateService;
      }
      catch (Exception remoteException)
      {
        System.Diagnostics.Trace.WriteLine(remoteException.Message);
      }
      return remoteService != null;
    }

    /// <summary>
    ///		Determine if a new version of this 
    ///	application is currently available
    /// </summary>
    /// <returns>
    ///		<c>true</c> if available, <c>false</c> otherwise
    /// </returns>
    private bool UpdateAvailable()
    {
      try
      {
        string assemblylocation
          = Assembly.GetExecutingAssembly().CodeBase;
        assemblylocation
          = assemblylocation.Substring(assemblylocation.LastIndexOf("/") + 1);
        applicationName = assemblylocation;
        AssemblyName assemblyName
          = Assembly.GetExecutingAssembly().GetName();
        string localVersion
          = assemblyName.Version.ToString();
        string remoteVersion
          = remoteService.GetCurrentVersion(applicationName);
        return IsUpdateNecessary(localVersion, remoteVersion);
      }
      catch (Exception)
      {
      }
      return false;
    }

    /// <summary>
    ///		Is update needed?
    /// </summary>
    /// <param name="localVersion"></param>
    /// <param name="remoteVersion"></param>
    /// <returns></returns>
    private bool IsUpdateNecessary(string localVersion, string remoteVersion)
    {
      try
      {
        long lcVersion = Convert.ToInt64(localVersion.Replace(".", ""));
        long rmVersion = Convert.ToInt64(remoteVersion.Replace(".", ""));

        //				#region DEBUG
        //				return lcVersion > rmVersion ;
        //				#endregion

        return lcVersion != rmVersion;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
      return false;
    }

    /// <summary>
    ///		Update the application executable
    /// </summary>
    public void Update()
    {
      //Anh Chau mo rao doan nay ra

      if (!ConnectRemoteServer())
        return;				// the remote connection was not okay
      if (UpdateAvailable())
      {
        // lets checkout if any update version available or not        
        string updateAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppUpdate.exe");

        //					#region DEBUIG
        //					updateAppPath = Path.Combine(@"D:\Programs\AppUpdateServer\AppUpdate\bin\Debug","AppUpdate.exe");
        //					#endregion					

        Process updateProcess = new Process();
        updateProcess.StartInfo =
          new ProcessStartInfo(updateAppPath, Process.GetCurrentProcess().Id.ToString() + " " + remoteObjectUri);
        updateProcess.Start();
      }

      //Het mo rao 
    }
  }
}
