using System;
using System.Windows.Forms;

namespace DaiCo.Shared.Utility
{

  public partial class ReportViewer : Form
  {

    public CrystalDecisions.CrystalReports.Engine.ReportDocument objReport;
    public ReportViewer()
    {
      InitializeComponent();
    }


    private void ReportViewer_Load(object sender, EventArgs e)
    {

    }

    public void PrintReport(CrystalDecisions.CrystalReports.Engine.ReportDocument objReport)
    {
      MessageBox.Show("Not init");
    }
    public void PrintReport2Excel(CrystalDecisions.CrystalReports.Engine.ReportDocument objReport)
    {
      MessageBox.Show("Not init");
    }
    public void ViewReport(CrystalDecisions.CrystalReports.Engine.ReportDocument objReport)
    {
      this.Viewer.ReportSource = objReport;
      Viewer.Show();
    }

  }
}