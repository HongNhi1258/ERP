/*
  Author      : 
  Date        : 09/05/2013
  Description : Tool Change Folder
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;
using System.IO;

namespace DaiCo.General
{
  public partial class viewGNR_04_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public viewGNR_04_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_001_Load(object sender, EventArgs e)
    {
    }
    #endregion Init

    private void button1_Click(object sender, EventArgs e)
    {
      string folderPath = "";
      FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
      if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
      {
        folderPath = folderBrowserDialog1.SelectedPath;
      }
      textBox1.Text = folderPath;
      button2.Enabled = (textBox1.Text.Trim().Length > 0);
    }

    private void button2_Click(object sender, EventArgs e)
    {
      string path = this.textBox1.Text;
      string[] directories = Directory.GetDirectories(path);
      try
      {
        foreach (string strName in directories)
        {
          if (strName.IndexOf("WW") != -1 || strName.IndexOf("ww") != -1)
          {
            string[] direcChild = Directory.GetDirectories(strName);
            foreach (string strChildName in direcChild)
            {
              int lastIndex = strChildName.LastIndexOf("\\");
              string nameOld = string.Empty;
              try
              {
                nameOld = strChildName.Substring(lastIndex + 1, strChildName.Length - lastIndex - 1).Trim();
              }
              catch
              {
                continue;
              }

              string commandText = string.Empty;
              commandText += " SELECT ItemCode ";
              commandText += " FROM TblBOMItemBasic ";
              commandText += " WHERE LOWER(OldCode) =  '" + nameOld.ToLower() + "'";

              DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              try
              {
                if (dt.Rows.Count >= 1)
                {
                  try
                  {
                    string nameNew = strName + "\\" + dt.Rows[0][0].ToString();
                    System.IO.Directory.Move(strChildName, nameNew);
                  }
                  catch
                  {
                    continue;
                  }
                }
              }
              catch
              {
                continue;
              }
            }
          }
        }
      }
      catch
      { 
      }

      WindowUtinity.ShowMessageSuccessFromText("Successfully!");
    }

    private void button3_Click(object sender, EventArgs e)
    {
      string folderPath = "";
      FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
      if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
      {
        folderPath = folderBrowserDialog1.SelectedPath;
      }
      textBox2.Text = folderPath;
      button3.Enabled = (textBox2.Text.Trim().Length > 0);
    }

    private void button4_Click(object sender, EventArgs e)
    {
      string path = this.textBox2.Text;
      string[] directories = Directory.GetFiles(path);
      try
      {
        foreach (string strName in directories)
        {
          int lastIndex = strName.LastIndexOf("\\");
          string abc = Path.GetFileNameWithoutExtension(strName);
          string commandText = string.Empty;
          commandText += " SELECT ItemCode ";
          commandText += " FROM TblBOMItemBasic ";
          commandText += " WHERE LOWER(OldCode) =  '" + abc.ToLower() + "'";

          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt.Rows.Count >= 1)
          {
            try
            {
              string nameNew = path + "\\" + dt.Rows[0][0].ToString() + ".jpg";
              System.IO.File.Move(strName, nameNew);
            }
            catch
            {
              continue;
            }
          }
        }
      }
      catch
      {
      }

      WindowUtinity.ShowMessageSuccessFromText("Successfully!");
    }
  }
}
