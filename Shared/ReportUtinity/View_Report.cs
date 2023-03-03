using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Shared.Utility;
using System.Windows.Forms;

namespace DaiCo.Shared
{
  public partial class View_Report : MainUserControl
  {
    private bool isShowGroupTree = false;
    public bool IsShowGroupTree
    {
      get { return this.isShowGroupTree; }
      set { this.isShowGroupTree = value; }
    }

    private ReportClass cptTemplate;
    public View_Report(ReportClass _cptTemplate)
    {
      this.cptTemplate = _cptTemplate;
      InitializeComponent();
    }
    public void ShowReport(ViewState viewState)
    {
      crystalReportViewer1.ShowGroupTreeButton = IsShowGroupTree;
      //crystalReportViewer1.DisplayGroupTree = IsShowGroupTree;
      crystalReportViewer1.ReportSource = cptTemplate;
      crystalReportViewer1.Refresh();
      WindowUtinity.ShowView(this, "Report", false, viewState);
    }

    public void ShowReport(ViewState viewState, FormWindowState windownState)
    {
      crystalReportViewer1.ShowGroupTreeButton = IsShowGroupTree;
      //crystalReportViewer1.DisplayGroupTree = IsShowGroupTree;
      crystalReportViewer1.ReportSource = cptTemplate;
      crystalReportViewer1.Refresh();
      WindowUtinity.ShowView(this, "Report", false, viewState, windownState);
    }

    public void ShowReport(ViewState viewState, bool isNewForm, FormWindowState windownState)
    {
      crystalReportViewer1.ShowGroupTreeButton = IsShowGroupTree;
      //crystalReportViewer1.DisplayGroupTree = IsShowGroupTree;
      crystalReportViewer1.ReportSource = cptTemplate;
      crystalReportViewer1.Refresh();
      WindowUtinity.ShowView(this, "Report", isNewForm, viewState, windownState);
    }
  }
}
