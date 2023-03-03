/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 14-08-2008
   Company :  Dai Co   
 */
using DaiCo.Application;
using DaiCo.Transaction;
using DaiCo.TransactionImplement;
using System;
using System.Data.SqlClient;

namespace DaiCo.ProjectImplement
{

  public class ProjectApplication
  {

    #region Fields
    static private ILogging logging;
    static private string strLogFolder;
    private SqlFactory factory = SqlFactory.Default;
    private ITransaction trans;
    private string strConnectionString;
    #endregion Fields

    #region Contructors
    public ProjectApplication(string strConnectionString)
    {
      this.strConnectionString = strConnectionString;
    }
    #endregion Contructors

    #region Methods
    public static void SetLogFolder(string strPath)
    {
      strLogFolder = strPath;
      logging = new DailyLogging(strLogFolder, new TraceLogCreator());
    }

    public SqlConnection Init()
    {
      TransactionFactory.SetFactory(new TransactionFactoryImpl());
      return factory.MakeConnection(this.strConnectionString);
    }

    public void Idle()
    {
      //Debug.Assert(logging != null, "logging is null", "Invoke static method SetLogFolder first.");
      //ILog log = null;
      try
      {
        trans.GetFactory(factory);
        trans.Execute();
      }
      catch (TransactionException te)
      {
        //log = logging.GetLog();
        //lock (log)
        //{
        //  log.Open();
        //  log.WriteLine(te.Message);
        //  log.Write(te.InnerException);
        //  log.WriteLine("---------------------\n");
        //  log.Close();
        //  throw;
        //}
      }
      catch (Exception e)
      {
        //log = logging.GetLog();
        //lock (log)
        //{
        //  log.Open();
        //  log.WriteLine("Unknow transaction");
        //  log.Write(e);
        //  log.WriteLine("---------------------\n");
        //  log.Close();
        //  throw;
        //}
      }
      catch
      {
        throw;
      }
    }

    public void Execute(ITransaction trans)
    {
      this.trans = trans;
      Idle();
    }

    public void Pause()
    {
      factory.Release();
    }

    public void Restart()
    {
      factory.MakeConnection(this.strConnectionString);
    }

    public void Restart(string strConnectionString)
    {
      factory.MakeConnection(strConnectionString);
      this.strConnectionString = strConnectionString;
    }
    #endregion Methods
  }
}
