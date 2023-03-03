using DaiCo.Application;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace DaiCo.Shared
{
  public class ValueChangedEventArgs : EventArgs
  {
    private string value;

    public ValueChangedEventArgs(string value)
    {
      this.value = value;
    }
    public string Value
    {
      get { return value; }
    }
  }

  public delegate void ValueChangedEventHandler(object source, ValueChangedEventArgs args);

  public partial class ucUltraList : Control
  {
    private static readonly object EventValueChanged = new object();
    private DataTable dataSoure;
    private string valueMember = string.Empty;
    private string displayMember = string.Empty;
    private string columnWidths = string.Empty;
    private char separator = char.MinValue;
    private string selectedValue = string.Empty;
    private string selectedText = string.Empty;
    private string autoSearchBy = string.Empty;

    public ucUltraList()
    {
      InitializeComponent();
    }

    #region function
    public void HideButonAlls()
    {
      btnAddAll.Visible = false;
      btnRemoveAll.Visible = false;
    }

    /// <summary>
    /// Add Source For UserControl
    /// </summary>
    /// <param name="dtSource"></param>
    public void DataBind()
    {
      lstLeft.Items.Clear();
      lstLeft.Columns.Clear();
      lstRight.Items.Clear();
      lstRight.Columns.Clear();
      btnAdd.Enabled = false;
      btnRemove.Enabled = false;
      if (this.dataSoure == null || this.dataSoure.Columns.Count == 0)
      {
        return;
      }
      // Set ValueMember
      if (this.valueMember.Length == 0)
      {
        this.valueMember = this.dataSoure.Columns[0].ColumnName;
      }

      if (this.displayMember.Length == 0)
      {
        this.displayMember = this.valueMember;
      }

      // Bind Data      
      lstLeft.Columns.Add(this.valueMember);
      lstRight.Columns.Add(this.valueMember);
      int columnCount = this.dataSoure.Columns.Count;
      for (int i = 0; i < columnCount; i++)
      {
        DataColumn col = this.dataSoure.Columns[i];
        if (string.Compare(this.valueMember, this.dataSoure.Columns[i].ColumnName, true) != 0)
        {
          lstLeft.Columns.Add(col.ColumnName);
          lstRight.Columns.Add(col.ColumnName);
        }
      }
      if (this.dataSoure.Rows.Count > 0)
      {
        foreach (DataRow row in this.dataSoure.Rows)
        {
          ListViewItem item = new ListViewItem(row[this.valueMember].ToString());
          item.Name = this.valueMember;
          int itemIndex = 1;
          for (int i = 0; i < this.dataSoure.Columns.Count; i++)
          {
            if (string.Compare(this.valueMember, this.dataSoure.Columns[i].ColumnName, true) != 0)
            {
              item.SubItems.Add(row[i].ToString());
              item.SubItems[itemIndex].Name = this.dataSoure.Columns[i].ColumnName;
              itemIndex++;
            }
          }
          lstLeft.Items.Add(item);
        }
      }
      // Set Columns Width
      string[] list = this.columnWidths.Split(';');
      for (int i = 0; i < list.Length; i++)
      {
        int width = DBConvert.ParseInt(list[i]);
        if (width >= 0)
        {
          try
          {
            lstLeft.Columns[i].Width = width;
            lstRight.Columns[i].Width = width;
          }
          catch { }
        }
      }
      this.selectedValue = string.Empty;
      this.selectedText = string.Empty;
    }

    /// <summary>
    /// Set Back Color White For ListView
    /// </summary>
    /// <param name="list"></param>
    private void SetBackColorWhite(ListView list)
    {
      foreach (ListViewItem item in list.Items)
      {
        item.ForeColor = Color.Black;
        item.BackColor = Color.White;
      }
    }

    /// <summary>
    /// Move items from left list to right list
    /// </summary>
    private void AddItems()
    {
      btnAdd.Enabled = false;
      if (lstLeft.SelectedItems.Count == 0)
      {
        return;
      }
      foreach (ListViewItem item in lstLeft.SelectedItems)
      {
        item.Selected = false;
        ListViewItem copyItem = (ListViewItem)item.Clone();
        for (int i = 0; i < item.SubItems.Count; i++)
        {
          copyItem.SubItems[i].Name = item.SubItems[i].Name;
        }
        lstRight.Items.Add(copyItem);
        lstLeft.Items.Remove(item);
        this.SetBackColorWhite(lstRight);
        txtLeft.Text = string.Empty;
      }
      this.SetBackColorWhite(lstLeft);
      if (lstLeft.Items.Count > 0)
      {
        txtLeft.Focus();
      }
      else
      {
        txtRight.Focus();
      }
      this.UnselectedItem(lstRight);
      btnRemove.Enabled = false;
      this.SelectedValue = this.GetSelectedValue();
      OnValueChanged(this, new ValueChangedEventArgs(this.SelectedValue));
    }

    /// <summary>
    /// Move items from right list to left list
    /// </summary>
    private void RemoveItems()
    {
      btnRemove.Enabled = false;
      if (lstRight.SelectedItems.Count == 0)
      {
        return;
      }
      foreach (ListViewItem item in lstRight.SelectedItems)
      {
        item.Selected = false;
        lstRight.Items.Remove(item);
        lstLeft.Items.Add(item);
        this.SetBackColorWhite(lstLeft);
        txtRight.Text = string.Empty;
      }
      this.SetBackColorWhite(lstRight);
      if (lstRight.Items.Count > 0)
      {
        txtRight.Focus();
      }
      else
      {
        txtLeft.Focus();
      }
      this.UnselectedItem(lstLeft);
      btnAdd.Enabled = false;
      this.SelectedValue = this.GetSelectedValue();
      OnValueChanged(this, new ValueChangedEventArgs(this.SelectedValue));
    }

    private void SetSelectedItem(ListView lst, int itemIndex)
    {
      lst.Items[itemIndex].Selected = true;
      lst.EnsureVisible(itemIndex);
      lst.Items[itemIndex].ForeColor = Color.White;
      lst.Items[itemIndex].BackColor = Color.FromName("Highlight");
    }

    private void SetValue()
    {
      this.selectedText = string.Empty;
      this.Separator = (this.Separator == char.MinValue) ? ';' : this.Separator;
      if (this.selectedValue.Length > 0)
      {
        string value = this.selectedValue.Replace(" ", "").Replace("|", ";");
        foreach (ListViewItem item in lstLeft.Items)
        {
          if (string.Format(";{0};", value).Contains(string.Format(";{0};", item.SubItems[this.valueMember].Text)))
          {
            ListViewItem copyItem = (ListViewItem)item.Clone();
            for (int i = 0; i < item.SubItems.Count; i++)
            {
              copyItem.SubItems[i].Name = item.SubItems[i].Name;
            }
            // Set value for selectedText
            if (this.selectedText.Length > 0)
            {
              this.selectedText = string.Format("{0}{1} {2}", this.selectedText, this.Separator, item.SubItems[this.displayMember].Text);
            }
            else
            {
              this.selectedText = item.SubItems[this.displayMember].Text;
            }

            lstRight.Items.Add(copyItem);
            lstLeft.Items.Remove(item);
          }
        }
        OnValueChanged(this, new ValueChangedEventArgs(this.SelectedValue));
      }
    }
    #endregion function

    /// <summary>
    /// Right List Selected Index Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void lstRight_SelectedIndexChanged(object sender, EventArgs e)
    {
      int count = lstRight.SelectedItems.Count;
      this.SetBackColorWhite(lstRight);
      if (count > 0)
      {
        btnRemove.Enabled = true;
        for (int i = lstRight.SelectedItems[0].Index; i <= lstRight.SelectedItems[count - 1].Index; i++)
        {
          this.SetSelectedItem(lstRight, i);
        }
      }
      else
      {
        btnRemove.Enabled = false;
      }
    }

    /// <summary>
    /// Left List Selected Index Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void lstLeft_SelectedIndexChanged(object sender, EventArgs e)
    {
      int count = lstLeft.SelectedItems.Count;
      this.SetBackColorWhite(lstLeft);
      if (count > 0)
      {
        btnAdd.Enabled = true;
        for (int i = lstLeft.SelectedItems[0].Index; i <= lstLeft.SelectedItems[count - 1].Index; i++)
        {
          this.SetSelectedItem(lstLeft, i);
        }
      }
      else
      {
        btnAdd.Enabled = false;
      }
    }

    private void lstLeft_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (lstLeft.Sorting == SortOrder.Ascending)
      {
        lstLeft.Sorting = SortOrder.Descending;
      }
      else
      {
        lstLeft.Sorting = SortOrder.Ascending;
      }
    }

    private void lstLeft_DoubleClick(object sender, EventArgs e)
    {
      this.AddItems();
    }

    private void lstRight_DoubleClick(object sender, EventArgs e)
    {
      this.RemoveItems();
    }

    private void lstRight_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (lstRight.Sorting == SortOrder.Ascending)
      {
        lstRight.Sorting = SortOrder.Descending;
      }
      else
      {
        lstRight.Sorting = SortOrder.Ascending;
      }
    }

    /// <summary>
    /// Button RemoveAll Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRemoveAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem items in lstRight.Items)
      {
        lstRight.Items.Remove(items);
        lstLeft.Items.Add(items);
      }
      txtLeft.Focus();
      txtRight.Text = string.Empty;
      this.SetBackColorWhite(lstLeft);
      this.SelectedValue = this.GetSelectedValue();
      OnValueChanged(this, new ValueChangedEventArgs(this.SelectedValue));
    }

    /// <summary>
    /// Button AddAll Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem items in lstLeft.Items)
      {
        lstLeft.Items.Remove(items);
        lstRight.Items.Add(items);
      }
      txtRight.Focus();
      txtLeft.Text = string.Empty;
      this.SetBackColorWhite(lstRight);
      this.SelectedValue = this.GetSelectedValue();
      OnValueChanged(this, new ValueChangedEventArgs(this.SelectedValue));
    }

    /// <summary>
    /// Button Remove Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRemove_Click(object sender, EventArgs e)
    {
      this.RemoveItems();
    }

    /// <summary>
    /// Button Add Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      this.AddItems();
    }

    protected virtual void OnValueChanged(object source, ValueChangedEventArgs args)
    {
      ValueChangedEventHandler onValueChangedHandler = (ValueChangedEventHandler)Events[EventValueChanged];
      if (onValueChangedHandler != null)
      {
        onValueChangedHandler(this, args);
      }
    }
    public event ValueChangedEventHandler ValueChanged
    {
      add
      {
        Events.AddHandler(EventValueChanged, value);
      }
      remove
      {
        Events.RemoveHandler(EventValueChanged, value);
      }
    }

    /// <summary>
    /// Right TextBox Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtRight_TextChanged(object sender, EventArgs e)
    {
      this.autoSearchBy = (this.autoSearchBy.Length > 0 ? this.autoSearchBy : this.valueMember);
      this.UnselectedItem(lstRight);
      this.SetBackColorWhite(lstRight);
      string textName = txtRight.Text.Trim();
      int length = textName.Length;
      if (length > 0)
      {
        foreach (ListViewItem item in lstRight.Items)
        {
          string textList = item.SubItems[autoSearchBy].Text;
          textList = (textList.Length >= length) ? textList.Substring(0, length) : textList;
          if (string.Compare(textList, textName, true) == 0)
          {
            lstRight.Items[item.Index].Selected = true;
            //this.SetSelectedItem(lstRight, item.Index);            
            break;
          }
          lstRight.Items[item.Index].Selected = false;
        }
      }
    }

    /// <summary>
    /// Left TextBox Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtLeft_TextChanged(object sender, EventArgs e)
    {
      this.autoSearchBy = (this.autoSearchBy.Length > 0 ? this.autoSearchBy : this.valueMember);
      this.UnselectedItem(lstLeft);
      //this.SetBackColorWhite(lstLeft);
      string textName = this.txtLeft.Text.Trim();
      int length = textName.Length;
      if (length > 0)
      {
        foreach (ListViewItem item in lstLeft.Items)
        {
          string textList = item.SubItems[autoSearchBy].Text;
          textList = (textList.Length >= length) ? textList.Substring(0, length) : textList;

          if (string.Compare(textList, textName, true) == 0)
          {
            lstLeft.Items[item.Index].Selected = true;
            //this.SetSelectedItem(lstLeft, item.Index);
            break;
          }
          lstLeft.Items[item.Index].Selected = false;
        }
      }
    }

    private void lstLeft_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.AddItems();
      }
    }

    private void lstRight_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.RemoveItems();
      }
    }

    private void txtLeft_KeyDown(object sender, KeyEventArgs e)
    {
      int itemCount = lstLeft.Items.Count;
      if (itemCount > 0)
      {
        if (e.KeyCode == Keys.Enter)
        {
          this.AddItems();
        }
        else if ((lstLeft.SelectedItems.Count > 0) && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)))
        {
          int index = lstLeft.SelectedItems[0].Index + (e.KeyCode == Keys.Up ? -1 : 1);
          if (index < 0 || index >= itemCount)
          {
            return;
          }
          this.UnselectedItem(lstLeft);
          this.SetSelectedItem(lstLeft, index);
        }
      }
    }

    private void txtRight_KeyDown(object sender, KeyEventArgs e)
    {
      int itemCount = lstRight.Items.Count;
      if (itemCount > 0)
      {
        if (e.KeyCode == Keys.Enter)
        {
          this.RemoveItems();
        }
        else if ((lstRight.SelectedItems.Count > 0) && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)))
        {
          int index = lstRight.SelectedItems[0].Index + (e.KeyCode == Keys.Up ? -1 : 1);
          if (index < 0 || index >= itemCount)
          {
            return;
          }
          this.UnselectedItem(lstRight);
          this.SetSelectedItem(lstRight, index);
        }
      }
    }

    /// <summary>
    /// Add Column For UserControl
    /// </summary>
    /// <param name="header"></param>
    /// <param name="width"></param>
    /// <param name="align"></param>
    private void AddColumn(string header, int width, HorizontalAlignment align)
    {
      lstLeft.Columns.Add(header, width, align);
      lstRight.Columns.Add(header, width, align);
    }

    /// <summary>
    /// Unselected Items For List
    /// </summary>
    /// <param name="list"></param>
    private void UnselectedItem(ListView list)
    {
      foreach (ListViewItem item in list.SelectedItems)
      {
        item.Selected = false;
      }
    }

    /// <summary>
    /// Get Selected Value
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private string GetSelectedValue()
    {
      this.selectedText = string.Empty;
      this.displayMember = (this.displayMember.Length > 0 ? this.displayMember : this.valueMember);

      string result = string.Empty;
      this.Separator = (this.Separator == char.MinValue) ? ';' : this.Separator;
      foreach (ListViewItem item in lstRight.Items)
      {
        if (result.Length > 0)
        {
          result = string.Format("{0}{1} {2}", result, this.Separator, item.Text);
          this.selectedText = string.Format("{0}{1} {2}", this.selectedText, this.Separator, item.SubItems[this.displayMember].Text);
        }
        else
        {
          result = item.Text;
          this.selectedText = item.SubItems[this.displayMember].Text;
        }
      }
      return result;
    }

    /// <summary>
    /// Add Item
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    private void AddItem(ListView list, ListViewItem item)
    {
      list.Items.Add(item);
    }

    /// <summary>
    /// Add Item At Index
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <param name="index"></param>
    private void AddItemAt(ListView list, ListViewItem item, int index)
    {
      list.Items.Insert(index, item);
    }

    /// <summary>
    /// Remove Item
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    private void RemoveItem(ListView list, ListViewItem item)
    {
      list.Items.Remove(item);
    }

    /// <summary>
    /// Remove Item At Index
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    private void RemoveItemAt(ListView list, int index)
    {
      list.Items.RemoveAt(index);
    }

    /// <summary>
    /// Get Index Of Item
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private int GetIndexOfItem(ListView list, ListViewItem item)
    {
      return list.Items.IndexOf(item);
    }

    #region Property
    public DataTable DataSource
    {
      get { return this.dataSoure; }
      set { this.dataSoure = value; }
    }
    public string ValueMember
    {
      get { return this.valueMember; }
      set { this.valueMember = value; }
    }
    public string DisplayMember
    {
      get { return this.displayMember; }
      set { this.displayMember = value; }
    }
    public string ColumnWidths
    {
      get { return this.columnWidths; }
      set { this.columnWidths = value; }
    }
    public string SelectedValue
    {
      get { return this.selectedValue; }
      set { this.selectedValue = value; }
    }
    public string SelectedText
    {
      get { return this.selectedText; }
      set { this.selectedText = value; }
    }
    /// <summary>
    /// Set value for ucUltraList
    /// </summary>
    public string Value
    {
      set
      {
        this.selectedValue = value;
        this.SetValue();
      }
    }
    public char Separator
    {
      get { return this.separator; }
      set { this.separator = value; }
    }
    public string AutoSearchBy
    {
      get { return this.autoSearchBy; }
      set { this.autoSearchBy = value; }
    }
    #endregion Property
  }
}
