/*
 * Author       : 
 * CreateDate   : 10/02/2011
 * Description  : Report
 */
using DaiCo.Application;
using DaiCo.Planning.DataSetFile;
using DaiCo.Planning.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class ViewPLN_99_001 : MainUserControl
  {
    #region Field
    public int ncategory = int.MinValue;
    public string suppNo = string.Empty;
    #endregion Field

    #region Init

    public ViewPLN_99_001()
    {
      InitializeComponent();
    }

    private void ViewPLN_99_001_Load(object sender, EventArgs e)
    {
      if (ncategory == 1)
      {
        this.SupplementReport();
      }
    }

    #endregion Init

    #region LoadReport
    private void SupplementReport()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@SupplementNo", DbType.AnsiString, 16, this.suppNo);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spRPTSupplement", inputParam);
      dsPLNSupplementNew dsSource = new dsPLNSupplementNew();
      dsSource.Tables["TblWO"].Merge(ds.Tables[0]);
      dsSource.Tables["TblWODetail"].Merge(ds.Tables[1]);

      cptPLNSupplement rpt = new cptPLNSupplement();

      rpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = rpt;
    }
    #endregion LoadReport

    #region more function

    #endregion more function
  }
}