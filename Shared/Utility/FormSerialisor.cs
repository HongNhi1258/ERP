using DaiCo.Application;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace FormSerialisation
{
  public static class FormSerialisor
  {
    /*
     * Drop this class into your project, and add the following line at the top of any class/form that wishes to use it...
       using FormSerialisation;
       To use the code, simply call FormSerialisor.Serialise(FormOrControlToBeSerialised, FullPathToXMLFile)
     * 
     * For more details, see http://www.codeproject.com/KB/dialog/SavingTheStateOfAForm.aspx
     * 
     * Last updated 13th June '10 to account for the odd behaviour of the two Panel controls in a SplitContainer (see the article)
     */
    public static void Serialise(Control c, string XmlFileName)
    {
      XmlTextWriter xmlSerialisedForm = new XmlTextWriter(XmlFileName, System.Text.UnicodeEncoding.Unicode);
      xmlSerialisedForm.Formatting = Formatting.Indented;
      xmlSerialisedForm.WriteStartDocument();
      xmlSerialisedForm.WriteStartElement("ChildForm");
      // enumerate all controls on the form, and serialise them as appropriate
      AddChildControls(xmlSerialisedForm, c);
      xmlSerialisedForm.WriteEndElement(); // ChildForm
      xmlSerialisedForm.WriteEndDocument();
      xmlSerialisedForm.Flush();
      xmlSerialisedForm.Close();
    }
    public static MemoryStream Serialise(Control c)
    {
      MemoryStream stream = new MemoryStream();
      XmlTextWriter xmlSerialisedForm = new XmlTextWriter(stream, System.Text.UnicodeEncoding.Unicode);
      xmlSerialisedForm.Formatting = Formatting.Indented;
      xmlSerialisedForm.WriteStartDocument();
      xmlSerialisedForm.WriteStartElement("ChildForm");
      // enumerate all controls on the form, and serialise them as appropriate
      AddChildControls(xmlSerialisedForm, c);
      xmlSerialisedForm.WriteEndElement(); // ChildForm
      xmlSerialisedForm.WriteEndDocument();
      xmlSerialisedForm.Flush();
      xmlSerialisedForm.Close();
      return stream;
    }
    private static string UnicodeString(string text)
    {
      return System.Text.Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(text));
    }
    private static void AddChildControls(XmlTextWriter xmlSerialisedForm, Control c)
    {
      Type type = c.GetType();
      //try
      //{
      //    FieldInfo field = type.GetField("Pid");
      //    field.SetValue(c, 3566);
      //}
      //catch
      //{ }

      foreach (FieldInfo f in type.GetFields())
      {
        // serialise this control
        xmlSerialisedForm.WriteStartElement("Control");
        xmlSerialisedForm.WriteAttributeString("Type", f.FieldType.ToString());
        xmlSerialisedForm.WriteAttributeString("Name", f.Name);
        if (f.FieldType == typeof(System.Int64))
        {
          long value = (long)f.GetValue(c);
          xmlSerialisedForm.WriteElementString("Text", value.ToString());
        }
        if (f.FieldType == typeof(System.String))
        {
          string value = f.GetValue(c).ToString();
          xmlSerialisedForm.WriteElementString("Text", value.Trim());
        }
        xmlSerialisedForm.WriteEndElement(); // Control

      }
      foreach (Control childCtrl in c.Controls)
      {
        if (!(childCtrl is Label))
        {
          // serialise this control
          xmlSerialisedForm.WriteStartElement("Control");
          xmlSerialisedForm.WriteAttributeString("Type", childCtrl.GetType().ToString());
          xmlSerialisedForm.WriteAttributeString("Name", childCtrl.Name);
          if (childCtrl is TextBox)
          {
            xmlSerialisedForm.WriteElementString("Text", ((TextBox)childCtrl).Text);
          }
          if (childCtrl is RichTextBox)
          {
            xmlSerialisedForm.WriteElementString("Text", ((RichTextBox)childCtrl).Text);
          }
          else if (childCtrl is ComboBox)
          {
            xmlSerialisedForm.WriteElementString("Text", ((ComboBox)childCtrl).Text);
            xmlSerialisedForm.WriteElementString("SelectedIndex", ((ComboBox)childCtrl).SelectedIndex.ToString());
          }
          else if (childCtrl is Infragistics.Win.UltraWinGrid.UltraCombo)
          {
            xmlSerialisedForm.WriteElementString("Text", ((Infragistics.Win.UltraWinGrid.UltraCombo)childCtrl).Text);
            try
            {
              xmlSerialisedForm.WriteElementString("SelectedIndex", ((Infragistics.Win.UltraWinGrid.UltraCombo)childCtrl).SelectedRow.Index.ToString());
            }
            catch
            {
              xmlSerialisedForm.WriteElementString("SelectedIndex", "-1");
            }
          }
          else if (childCtrl is DateTimePicker)
          {
            xmlSerialisedForm.WriteElementString("Text", ((DateTimePicker)childCtrl).Value.ToString("dd/MM/yyyy"));
          }
          else if (childCtrl is Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)
          {
            xmlSerialisedForm.WriteElementString("Text", DBConvert.ParseDateTime(((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)childCtrl).Value).ToString("dd/MM/yyyy"));
          }

          else if (childCtrl is ListBox)
          {
            // need to account for multiply selected items
            ListBox lst = (ListBox)childCtrl;
            if (lst.SelectedIndex == -1)
            {
              xmlSerialisedForm.WriteElementString("SelectedIndex", "-1");
            }
            else
            {
              for (int i = 0; i < lst.SelectedIndices.Count; i++)
              {
                xmlSerialisedForm.WriteElementString("SelectedIndex", (lst.SelectedIndices[i].ToString()));
              }
            }
          }
          else if (childCtrl is CheckBox)
          {
            xmlSerialisedForm.WriteElementString("Checked", ((CheckBox)childCtrl).Checked.ToString());
          }
          // this next line was taken from http://stackoverflow.com/questions/391888/how-to-get-the-real-value-of-the-visible-property
          // which dicusses the problem of child controls claiming to have Visible=false even when they haven't, based on the parent
          // having Visible=true
          bool visible = (bool)typeof(Control).GetMethod("GetState", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(childCtrl, new object[] { 2 });
          xmlSerialisedForm.WriteElementString("Visible", visible.ToString());
          // see if this control has any children, and if so, serialise them
          if (childCtrl.HasChildren)
          {
            if (childCtrl is SplitContainer)
            {
              // handle this one as a special case
              AddChildControls(xmlSerialisedForm, ((SplitContainer)childCtrl).Panel1);
              AddChildControls(xmlSerialisedForm, ((SplitContainer)childCtrl).Panel2);
            }
            else
            {
              AddChildControls(xmlSerialisedForm, childCtrl);
            }
          }
          xmlSerialisedForm.WriteEndElement(); // Control
        }
      }
    }

    public static void Deserialise(Control c, string XmlFileName)
    {
      if (File.Exists(XmlFileName))
      {
        XmlDocument xmlSerialisedForm = new XmlDocument();
        xmlSerialisedForm.Load(XmlFileName);
        XmlNode topLevel = xmlSerialisedForm.ChildNodes[1];
        foreach (XmlNode n in topLevel.ChildNodes)
        {
          SetControlProperties((Control)c, n);
        }
      }
    }
    public static void Deserialise(Control c, MemoryStream Streamfile)
    {
      if (Streamfile.Length > 0)
      {
        Streamfile.Position = 0;
        XmlDocument xmlSerialisedForm = new XmlDocument();
        xmlSerialisedForm.Load(Streamfile);
        XmlNode topLevel = xmlSerialisedForm.ChildNodes[1];
        foreach (XmlNode n in topLevel.ChildNodes)
        {
          SetControlProperties((Control)c, n);
        }
      }
    }

    private static void SetControlProperties(Control currentCtrl, XmlNode n)
    {
      try
      {
        // get the control's name and type
        string controlName = n.Attributes["Name"].Value;
        string controlType = n.Attributes["Type"].Value;
        // find the control
        Control[] ctrl = currentCtrl.Controls.Find(controlName, true);
        Type type = currentCtrl.GetType();
        //try
        //{
        //    FieldInfo field = type.GetField("Pid");
        //    field.SetValue(c, 3566);
        //}
        //catch
        //{ }

        foreach (FieldInfo f in type.GetFields())
        {
          if (f.FieldType == typeof(System.Int64))
          {
            //long value = (long)f.GetValue(c);
            //xmlSerialisedForm.WriteElementString("Text", value.ToString());
            FieldInfo field = type.GetField(n.Attributes["Name"].Value.ToString());
            field.SetValue(currentCtrl, DBConvert.ParseLong(n["Text"].InnerText.ToString()));
          }
          if (f.FieldType == typeof(System.String))
          {
            //string value = f.GetValue(c).ToString();
            //xmlSerialisedForm.WriteElementString("Text", value.Trim());
            FieldInfo field = type.GetField(n.Attributes["Name"].Value.ToString());
            field.SetValue(currentCtrl, n["Text"].InnerText.ToString());
          }
        }
        if (ctrl.Length == 0)
        {
          // can't find the control
        }
        else
        {
          Control ctrlToSet = GetImmediateChildControl(ctrl, currentCtrl);

          if (ctrlToSet != null)
          {
            if (ctrlToSet.GetType().ToString() == controlType)
            {
              // the right type too ;-)
              switch (controlType)
              {
                case "System.Windows.Forms.TextBox":
                  ((System.Windows.Forms.TextBox)ctrlToSet).Text = n["Text"].InnerText;
                  break;
                case "System.Windows.Forms.RichTextBox":
                  ((System.Windows.Forms.RichTextBox)ctrlToSet).Text = n["Text"].InnerText;
                  break;
                case "System.Windows.Forms.DateTimePicker":
                  ((System.Windows.Forms.DateTimePicker)ctrlToSet).Value = DateTime.ParseExact(n["Text"].InnerText, "dd/MM/yyyy", null);
                  break;
                case "Infragistics.Win.UltraWinEditors.UltraDateTimeEditor":
                  ((Infragistics.Win.UltraWinEditors.UltraDateTimeEditor)ctrlToSet).Value = DateTime.ParseExact(n["Text"].InnerText, "dd/MM/yyyy", null);
                  break;
                case "Infragistics.Win.UltraWinGrid.UltraCombo":
                  ((Infragistics.Win.UltraWinGrid.UltraCombo)ctrlToSet).Text = n["Text"].InnerText;
                  ((Infragistics.Win.UltraWinGrid.UltraCombo)ctrlToSet).Rows[Convert.ToInt32(n["SelectedIndex"].InnerText)].Selected = true;
                  break;
                case "DaiCo.Shared.DaiCoComboBox":
                  ((DaiCo.Shared.DaiCoComboBox)ctrlToSet).Text = n["Text"].InnerText;
                  ((DaiCo.Shared.DaiCoComboBox)ctrlToSet).SelectedIndex = Convert.ToInt32(n["SelectedIndex"].InnerText);
                  break;
                case "System.Windows.Forms.ComboBox":
                  ((System.Windows.Forms.ComboBox)ctrlToSet).Text = n["Text"].InnerText;
                  ((System.Windows.Forms.ComboBox)ctrlToSet).SelectedIndex = Convert.ToInt32(n["SelectedIndex"].InnerText);
                  break;
                case "System.Windows.Forms.ListBox":
                  // need to account for multiply selected items
                  ListBox lst = (ListBox)ctrlToSet;
                  XmlNodeList xnlSelectedIndex = n.SelectNodes("SelectedIndex");
                  for (int i = 0; i < xnlSelectedIndex.Count; i++)
                  {
                    lst.SelectedIndex = Convert.ToInt32(xnlSelectedIndex[i].InnerText);
                  }
                  break;
                case "System.Windows.Forms.CheckBox":
                  ((System.Windows.Forms.CheckBox)ctrlToSet).Checked = Convert.ToBoolean(n["Checked"].InnerText);
                  break;
              }
              ctrlToSet.Visible = Convert.ToBoolean(n["Visible"].InnerText);
              // if n has any children that are controls, deserialise them as well
              if (n.HasChildNodes && ctrlToSet.HasChildren)
              {
                XmlNodeList xnlControls = n.SelectNodes("Control");
                foreach (XmlNode n2 in xnlControls)
                {
                  try
                  {
                    SetControlProperties(ctrlToSet, n2);
                  }
                  catch
                  { }
                }
              }
            }
            else
            {
              // not the right type
            }
          }
          else
          {
            // can't find a control whose parent is the current control
          }
        }
      }
      catch
      {

      }
    }

    private static Control GetImmediateChildControl(Control[] ctrl, Control currentCtrl)
    {
      Control c = null;
      for (int i = 0; i < ctrl.Length; i++)
      {
        if ((ctrl[i].Parent.Name == currentCtrl.Name) || (currentCtrl is SplitContainer && ctrl[i].Parent.Parent.Name == currentCtrl.Name))
        {
          c = ctrl[i];
          break;
        }
      }
      return c;
    }

  }
}
