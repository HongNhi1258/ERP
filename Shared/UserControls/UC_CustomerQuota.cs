using DaiCo.Application;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Shared.UserControls
{
  public partial class UC_CustomerQuota : UserControl
  {
    private long customerId;
    private long customerDirectId;
    private DateTime dateFrom;

    public long CustomerPid
    {
      get { return this.customerId; }
      set { this.customerId = value; }
    }
    public long CustomerDirectId
    {
      get { return this.customerDirectId; }
      set { this.customerDirectId = value; }
    }

    public DateTime DateFrom
    {
      get { return this.dateFrom; }
      set { this.dateFrom = value; }
    }

    public UC_CustomerQuota()
    {
      InitializeComponent();
    }

    public void ShowData()
    {
      lvData.Columns.Clear();
      lvData.Items.Clear();
      DateTime clStart = new DateTime(dateFrom.Year, dateFrom.Month, 1);
      DateTime clFinish = dateFrom.AddMonths(11);
      clFinish = new DateTime(clFinish.Year, clFinish.Month, DateTime.DaysInMonth(clFinish.Year, clFinish.Month));

      DBParameter[] param = new DBParameter[3];
      param[0] = new DBParameter("@customerPid", DbType.Int64, CustomerPid);
      param[1] = new DBParameter("@dateFrom", DbType.DateTime, clStart);
      param[2] = new DBParameter("@dateTo", DbType.DateTime, clFinish);

      DataSet dsSource = DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNGetCBMINQuota", param);
      DateTime clIndex = clStart;
      lvData.Columns.Add("");

      //so luong da co trong enquery hoac da lam PO
      DataTable pass = dsSource.Tables[0];
      DataTable quotas = dsSource.Tables[1];
      DataTable cusGroup = dsSource.Tables[2];

      int countquotas = quotas.Rows.Count;
      ListViewItem itemQuota = new ListViewItem("Quota");

      string groupName = "Order";
      if (cusGroup.Rows.Count > 0)
        groupName = DBConvert.ParseString(cusGroup.Rows[0][0]);
      ListViewItem itemGoup = new ListViewItem(groupName);

      while (clIndex <= clFinish)
      {
        int month = clIndex.Month;
        int year = clIndex.Year;

        string header = string.Format("{0}/{1}", month, year);
        lvData.Columns.Add(header, 70, HorizontalAlignment.Center);

        string fiterExpress = string.Format("Year={0} AND Month={1}", year, month);

        double quota = DBConvert.ParseDouble(quotas.Compute("SUM(Quota)", fiterExpress));
        itemQuota.SubItems.Add(DBConvert.ParseString(quota));


        long group = DBConvert.ParseLong(pass.Compute("SUM(qty)", fiterExpress).ToString());
        ListViewItem.ListViewSubItem subItem = new ListViewItem.ListViewSubItem(itemGoup, DBConvert.ParseString(group));
        if (group >= quota)
          subItem.BackColor = Color.LightBlue;
        itemGoup.SubItems.Add(subItem);


        clIndex = clIndex.AddMonths(1);
      }
      lvData.Items.Add(itemQuota);
      lvData.Items.Add(itemGoup);
    }
  }
}
