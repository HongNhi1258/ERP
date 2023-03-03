using System;
using System.Data;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MainBOM
{
  public static class myPrinters
  {
    [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDefaultPrinter(string Name);

  }

  public partial class testbartender : Form
  {
    public testbartender()
    {
      InitializeComponent();
    }
    private string printerName;
    BarTender.Application btApp;
    BarTender.Format btFormat;
    private void testbartender_Load(object sender, EventArgs e)
    {
      btApp = new BarTender.ApplicationClass();
      printerName = GetDefaultPrinter();

      string pName = "Datamax M-4206 Mark II";
      myPrinters.SetDefaultPrinter(pName);
    }

    private string GetDefaultPrinter()
    {
      PrinterSettings settings = new PrinterSettings();
      foreach (string printer in PrinterSettings.InstalledPrinters)
      {
        settings.PrinterName = printer;
        if (settings.IsDefaultPrinter)
          return printer;
      }
      return string.Empty;
    }


    private void button1_Click(object sender, EventArgs e)
    {
      string pName = "Datamax M-4206 Mark II";
      myPrinters.SetDefaultPrinter(pName);

      DataTable dt = new DataTable();
      string commandText = "SELECT TOP 1 BoxTypeCode, BoxTypeName FROM TblBOMBoxType";
      dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);

      foreach (DataRow dr in dt.Rows)
      {
        btFormat = btApp.Formats.Open("C:\\Documents and Settings\\MinhD.DAICO-FURNITURE\\Desktop\\Format2.btw", false, "");
        //btFormat.
        btFormat.SetNamedSubStringValue("BoxCode", dr[0].ToString());
        btFormat.SetNamedSubStringValue("BoxName", dr[1].ToString());
        btFormat.SetNamedSubStringValue("Barcode", "BOX-0000113");

        btFormat.PrintOut(false, false);
        btFormat.Close(BarTender.BtSaveOptions.btDoNotSaveChanges);
      }

      myPrinters.SetDefaultPrinter(printerName);
    }

    private void testbartender_FormClosing(object sender, FormClosingEventArgs e)
    {
      //btApp.Quit(BarTender.BtSaveOptions.btDoNotSaveChanges);
    }
  }
}