using DaiCo.Application;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DaiCo.Shared.UserControls
{
  public partial class UC_ListViewExt : UserControl
  {
    public List<ColumnType> colums;
    public IList DataSource;
    public UC_ListViewExt()
    {
      InitializeComponent();
      lvContent.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(lvContent_ItemSelectionChanged);
      this.colums = new List<ColumnType>();
      this.DataSource = new List<object>();
    }

    void lvContent_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
    {
      int index = e.ItemIndex;
      object obj = this.DataSource[index];
      foreach (ColumnType col in colums)
      {
        Control colInfo = col.ColumnControl;
        if (obj != null)
        {
          if (colInfo.GetType().Name == "TextBox")
          {
            colInfo.Text = GetObjectValue(col.FieldName, obj).ToString();
          }
          else if (colInfo.GetType().Name == "ComboBox")
          {
            ComboBox comb = (ComboBox)colInfo;
            comb.SelectedValue = GetObjectValue(col.FieldName, obj);
          }
          else if (colInfo.GetType().Name == "uc_DateTimePicker")
          {
            uc_DateTimePicker dtp = (uc_DateTimePicker)colInfo;
            DateTime dt = (DateTime)GetObjectValue(col.FieldName, obj);
            dtp.Value = dt;
          }
          else if (colInfo.GetType().Name == "CheckBox")
          {
            //if (isFirstColumn)
            //  item.Text = objValue.ToString();
            //else
            //  item.SubItems.Add(objValue.ToString());
          }
        }
      }
    }

    private Type type;
    public void DesignView<T>()
    {
      type = typeof(T);
      int intColumn = colums.Count;
      tableLayoutPanel1.ColumnStyles[0].SizeType = SizeType.Absolute;
      tableLayoutPanel1.ColumnStyles[0].Width = colums[0].Width;
      int colShowCount = 0;
      for (int i = 0; i < intColumn; i++)
      {
        ColumnType col = colums[i];
        if (!col.IsHide)
        {
          colShowCount++;
          Label lab = new Label();
          lab.Text = col.Header;
          lab.TextAlign = ContentAlignment.MiddleCenter;

          col.ColumnControl.Margin = new Padding(0);
          col.ColumnControl.Dock = DockStyle.Fill;
          lab.Margin = new Padding(0);
          lab.Dock = DockStyle.Fill;
          if (i > 0)
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, col.Width));
          tableLayoutPanel1.Controls.Add(lab, i, 0);
          tableLayoutPanel1.Controls.Add(col.ColumnControl, i, 1);
          ColumnHeader colHeader = new ColumnHeader();
          colHeader.Width = col.Width;
          lvContent.Columns.Add(colHeader);
        }
      }
      Button btnOK = new Button();
      btnOK.Text = "OK";
      btnOK.Width = 30;
      btnOK.Margin = new Padding(0);
      btnOK.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
      btnOK.Click += new EventHandler(btnOK_Click);
      tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, btnOK.Width));
      tableLayoutPanel1.Controls.Add(btnOK, colShowCount, 1);

      tableLayoutPanel1.ColumnCount = colShowCount + 1;
      tableLayoutPanel1.SetColumnSpan(lvContent, colShowCount + 1);
      //Init ListView
      UpdateListView();
    }

    void btnOK_Click(object sender, EventArgs e)
    {
      object objNew = System.Activator.CreateInstance(type, true);
      foreach (ColumnType col in colums)
      {
        FieldInfo info = this.type.GetField(col.FieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
        if (info != null)
        {
          Control colInfo = col.ColumnControl;
          if (colInfo.GetType().Name == "TextBox")
          {
            TextBox txt = (TextBox)colInfo;
            info.SetValue(objNew, Convert.ChangeType(txt.Text, info.FieldType));
          }
          else if (colInfo.GetType().Name == "ComboBox")
          {
            ComboBox comb = (ComboBox)col.ColumnControl;
            info.SetValue(objNew, Convert.ChangeType(comb.SelectedValue, info.FieldType));

          }
          else if (colInfo.GetType().Name == "uc_DateTimePicker")
          {
            uc_DateTimePicker dtp = (uc_DateTimePicker)col.ColumnControl;
            info.SetValue(objNew, dtp.Value);
          }
          else if (colInfo.GetType().Name == "CheckBox")
          {
            //if (isFirstColumn)
            //  item.Text = objValue.ToString();
            //else
            //  item.SubItems.Add(objValue.ToString());
          }
        }
      }
      this.DataSource.Add(objNew);
      UpdateListView();
    }



    private void UpdateListView()
    {
      lvContent.Items.Clear();
      if (DataSource != null && DataSource.Count > 0)
      {
        bool isFirstColumn = false;
        foreach (object obj in DataSource)
        {
          isFirstColumn = true;
          ListViewItem item = new ListViewItem();
          foreach (ColumnType info in colums)
          {
            Control colInfo = info.ColumnControl;
            object objValue = GetObjectValue(info.FieldName, obj);
            if (objValue != null)
            {
              switch (colInfo.GetType().Name)
              {
                case "TextBox":
                  {
                    if (isFirstColumn)
                      item.Text = objValue.ToString();
                    else
                      item.SubItems.Add(objValue.ToString());
                    break;
                  }
                case "ComboBox":
                  {
                    string v = objValue.ToString();
                    ComboBox comb = (ComboBox)info.ColumnControl;
                    if (comb.DataSource != null)
                    {
                      object objD = MatchKeyInList((IList)comb.DataSource, v, comb.ValueMember);
                      object objV = GetObjectValue(comb.DisplayMember, objD);
                      if (isFirstColumn)
                        item.Text = objV != null ? objV.ToString() : "";
                      else
                        item.SubItems.Add(objV != null ? objV.ToString() : "");
                    }
                    break;
                  }
                case "uc_DateTimePicker":
                  {
                    uc_DateTimePicker dtp = (uc_DateTimePicker)info.ColumnControl;
                    DateTime dt = (DateTime)objValue;
                    item.SubItems.Add(DBConvert.ParseString(dt, "dd/MM/yyyy"));
                    break;
                  }
                case "CheckBox":
                  {
                    break;
                  }
                default:
                  break;
              }
            }
            isFirstColumn = false;
          }
          lvContent.Items.Add(item);
        }
      }
    }

    private object GetObjectValue(string fieldName, object obj)
    {
      if (obj == null)
        return null;
      Type ty = obj.GetType();
      FieldInfo info = ty.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
      if (info != null)
        return info.GetValue(obj);
      else
        return null;
    }

    private object MatchKeyInList(IList dataSource, string key, string field)
    {
      foreach (object obj in dataSource)
      {
        object obv = GetObjectValue(field, obj);
        if (obv != null && string.Equals(obv.ToString(), key, StringComparison.OrdinalIgnoreCase))
          return obj;
      }
      return null;
    }
  }

  public class ColumnType
  {
    private string header;
    private string fieldName;
    private Control columnControl;
    private int width;
    private bool isHide;
    public ColumnType()
    {
      this.header = string.Empty;
      this.fieldName = string.Empty;
      this.columnControl = null;
      this.width = 0;
      this.isHide = false;
    }
    public ColumnType(string header, string fieldName, Control columnControl, int width, bool isHide)
    {
      this.header = header;
      this.fieldName = fieldName;
      this.columnControl = columnControl;
      this.width = width;
      this.isHide = isHide;
    }
    public ColumnType(string header, string fieldName, Control columnControl, int width)
    {
      this.header = header;
      this.fieldName = fieldName;
      this.columnControl = columnControl;
      this.width = width;
      this.isHide = false;
    }
    public ColumnType(string header, string fieldName, int width)
    {
      this.header = header;
      this.fieldName = fieldName;
      this.columnControl = new TextBox();
      this.width = width;
      this.isHide = false;
    }
    public string Header
    {
      get { return this.header; }
      set { this.header = value; }
    }
    public string FieldName
    {
      get { return this.fieldName; }
      set { this.fieldName = value; }
    }
    public Control ColumnControl
    {
      get { return this.columnControl; }
      set { this.columnControl = value; }
    }
    public int Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    public bool IsHide
    {
      get { return this.isHide; }
      set { this.isHide = value; }
    }
  }

  public class TestDataSource
  {
    private string name;
    private int birthPlace;
    private string district;
    private DateTime birthDate;
    public TestDataSource()
    {
      this.name = string.Empty;
      this.birthPlace = int.MinValue;
      this.district = string.Empty;
      this.birthDate = DateTime.MinValue;
    }
    public TestDataSource(string name, int birthPlace, string district, DateTime birthDate)
    {
      this.name = name;
      this.birthPlace = birthPlace;
      this.district = district;
      this.birthDate = birthDate;
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
    public int BirthPlace
    {
      get { return this.birthPlace; }
      set { this.birthPlace = value; }
    }
    public string District
    {
      get { return this.district; }
      set { this.district = value; }
    }
    public DateTime BirthDate
    {
      get { return this.birthDate; }
      set { this.birthDate = value; }
    }
  }

  public class Province
  {
    int pid;
    string name;
    public Province(int pid, string name)
    {
      this.pid = pid;
      this.name = name;
    }
    public int Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
  }

  public class District
  {
    int pid;
    int provincePid;
    string name;
    public District(int pid, int provincePid, string name)
    {
      this.pid = pid;
      this.provincePid = provincePid;
      this.name = name;
    }
    public int Pid
    {
      get { return this.pid; }
      set { this.pid = value; }
    }
    public int ProvincePid
    {
      get { return this.provincePid; }
      set { this.provincePid = value; }
    }
    public string Name
    {
      get { return this.name; }
      set { this.name = value; }
    }
  }
}
