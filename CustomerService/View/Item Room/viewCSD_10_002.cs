/*
  Author      : Huu Phuoc
  Date        : 17/03/2016
  Description : Create Item Room
  Standard Form: viewCSD_10_002
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using System.IO;
using System.Diagnostics;
using Infragistics.Win.UltraWinGrid;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_10_002 : MainUserControl
  {
    #region Field
    private IList listDeletedPid = new ArrayList();

    #endregion Field

    #region Init
    public viewCSD_10_002()
    {
      InitializeComponent();
    }

    private void viewCSD_10_002_Load(object sender, EventArgs e)
    {
      this.LoadCbItem();
      this.LoadCustomer();
      btnSave.Enabled = false; 
    }
    #endregion Init

    #region Function
    private void ClearCotrol()
    {
      ultraCustomer.Text = string.Empty;
      ultraItem.Text = string.Empty;
      txtSetCode.Text = string.Empty;
      txtSetCollection.Text = string.Empty;
      txtSetRoom.Text = string.Empty;
      
    }
    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    
    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadCustomer()
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode +'  -  '+ Name FullName
                                           FROM TblCSDCustomerInfo 
                                           WHERE DeletedFlg = 0 
                                           AND ParentPid IS NULL 
                                           ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultraCustomer, dtSource, "Pid", "FullName","Pid");
      ControlUtility.LoadUltraDropDown(ultraDropCustomer, dtSource, "Pid", "FullName", "Pid");

      ultraDropCustomer.DisplayLayout.Bands[0].Columns["FullName"].CellActivation  = Activation.ActivateOnly ;
 
    }
    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadCbItem()
    {

      string commandText = string.Empty;
      commandText = string.Format(@"SELECT ItemCode FROM TblBOMItemBasic ORDER BY ItemCode");

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultraItem, dtSource, "ItemCode", "ItemCode");
      ControlUtility.LoadUltraDropDown(ultraDropItemCode, dtSource, "ItemCode", "ItemCode");
     
    }

    /// <summary>
    /// Search Data
    /// </summary>
    private void Search()
    {
      //btnSearch.Enabled = false;
      string itemCode = "";
      if (ultraItem.Value != null && ultraItem.Text !="")
      {
        itemCode = ultraItem.Value.ToString();
      }
      else
      {
        itemCode = null;
      }
      string setCode = "";
      if (txtSetCode.Text.Trim() !="")
      {
        setCode = txtSetCode.Text.Trim();
      }
      else
      {
        setCode = null;
      }
      string setCollection = "";
      if (txtSetCollection.Text.Trim() !="")
      {
        setCollection = txtSetCollection.Text.Trim();
      }
      else
	    {
        setCollection = null;
	    }
      string setRoom = "";
      if (txtSetRoom.Text.Trim() !="")
      {
        setRoom = txtSetRoom.Text.Trim();
      }
      else
      {
        setRoom = null;
      }
      DBParameter[] inputParam = new DBParameter[5];
 
       inputParam[0] = new DBParameter("@ItemCode", DbType.String ,  itemCode);       
     
       if (ultraCustomer.Value != null && ultraCustomer.Text != "")       {
         
         inputParam[1] = new DBParameter("@CustomerPid", DbType.Int32, Convert.ToInt16(ultraCustomer.Value));
       }
       else
       {
         inputParam[1] = new DBParameter("@CustomerPid", DbType.Int32, null);
       }

       inputParam[2] = new DBParameter("@SetCode", DbType.String , setCode );   
       inputParam[3] = new DBParameter("@SetCollection", DbType.String , setCollection);
       inputParam[4] = new DBParameter("@SetRoom", DbType.String, setRoom);   
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemCodeForRoom", inputParam);
      ultData.DataSource = dtSource;
    }

    /// <summary>
    /// Check data before Save in database
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (row.Cells["SetQuantity"].Appearance.BackColor == Color.Yellow)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          return false;
        }
      }

      return true;
    }
    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>

    private bool CheckExistingItemCode(string ItemCode, int Pid, int CustomerCode)
    {
      string sql = "";
      sql = string.Format("SELECT ItemCode  FROM TblCSDRoomSetting WHERE ItemCode = '{0}' AND Pid !={1} AND CustomerPid ={2}", ItemCode, Pid, CustomerCode);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      if (dt.Rows.Count > 0)
      {
        return false;
      }
      return true;
    }

    private bool CheckExistingItemCodeForUltraCombo(string ItemCode)
    {
      string sql = "";
      sql = string.Format("SELECT ItemCode  FROM TblBOMItemBasic WHERE ItemCode = '{0}'", ItemCode);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(sql);
      if (dt.Rows.Count <= 0 || dt == null)
      {
        return false;
      }
      return true;
    }

    private bool SaveData()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      DBParameter[] input = new DBParameter[7];
      //1. Delete Row     
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spCSDItemCodeRoomDelete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          return false;
        }
      }

      //2. Update and Insert data
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          long detailPid = DBConvert.ParseLong(row["Pid"].ToString());

          if (detailPid != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
          }
          else
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBNull.Value);
          }
          int CustomerPid = DBConvert.ParseInt(row["CustomerPid"].ToString());
          input[1] = new DBParameter("@CustomerPid", DbType.Int64, CustomerPid);
          string itemCode = row["ItemCode"].ToString().Trim();
          input[2] = new DBParameter("@ItemCode", DbType.String, itemCode);
          long SetQuantity = DBConvert.ParseLong(row["SetQuantity"].ToString());
          input[3] = new DBParameter("@SetQuantity", DbType.Double, SetQuantity);
          string SetRoom = row["SetRoom"].ToString().Trim();
          input[4] = new DBParameter("@SetRoom", DbType.String, SetRoom);
          string SetCode = row["SetCode"].ToString().Trim();
          input[5] = new DBParameter("@SetCode", DbType.String, SetCode);
          string SetColletion = row["SetColletion"].ToString().Trim();
          input[6] = new DBParameter("@SetCollection", DbType.String, SetColletion);

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          if (itemCode == "" || itemCode == null)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return false;
          }
          if (CustomerPid < 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return false;
          }
          if (!CheckExistingItemCodeForUltraCombo(itemCode))
          {
            MessageBox.Show("Item Code: " + itemCode.ToString() + " Not Existing !");
            return false;
          }
          if (SetQuantity <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return false;
          }
          if (!this.CheckExistingItemCode(itemCode, DBConvert.ParseInt(detailPid), CustomerPid))
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return false;
          }
          else
          {
            DataBaseAccess.ExecuteStoreProcedure("spCSDItemCodeRoomInsert", input, outputParam);            

          }
          long result = DBConvert.ParseLong(outputParam[0].Value);
          if (result == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            return false;
          }
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
      this.btnSave.Enabled = false;
      this.SetNeedToSave();
      return true;
    }
    #endregion Function


    #region Event
    /// <summary>
    /// Search Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }
    /// <summary>
    /// Clear Control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCotrol();
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if(CheckValid())
      {
        this.SaveData();
      }      
    }
    private void ultData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();      
      switch (colName)
      {
        case "SetQuantity":
          if (DBConvert.ParseLong(e.Cell.Row.Cells["SetQuantity"].Value) < 0)
          {
            e.Cell.Row.Cells["SetQuantity"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            e.Cell.Row.Cells["SetQuantity"].Appearance.BackColor = Color.White;
          }
          if (!CheckExistingItemCode(e.Cell.Row.Cells["ItemCode"].Value.ToString(), DBConvert.ParseInt(e.Cell.Row.Cells["Pid"].Value), DBConvert.ParseInt(e.Cell.Row.Cells["CustomerPid"].Value)))
          {
            MessageBox.Show("Item Code is Existing already !");
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.White;
          } 
          break;
        case "CustomerPid":
          if (DBConvert.ParseLong(e.Cell.Row.Cells["CustomerPid"].Value) < 0)
          {
            MessageBox.Show("Customer can't Empty !"); 
            e.Cell.Row.Cells["CustomerPid"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            e.Cell.Row.Cells["CustomerPid"].Appearance.BackColor = Color.White;
          }
          if (!CheckExistingItemCode(e.Cell.Row.Cells["ItemCode"].Value.ToString(), DBConvert.ParseInt(e.Cell.Row.Cells["Pid"].Value), DBConvert.ParseInt(e.Cell.Row.Cells["CustomerPid"].Value)))
          {
            MessageBox.Show("Item Code is Existing already !");
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.White;
          } 
          break;
        case "ItemCode":
          if (DBConvert.ParseString(e.Cell.Row.Cells["ItemCode"].Value) =="" || !(CheckExistingItemCodeForUltraCombo(DBConvert.ParseString(e.Cell.Row.Cells["ItemCode"].Value))))
          {
            MessageBox.Show("Item Code can't Empty or Not Existing !");
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.White;
          }
          if (!CheckExistingItemCode(e.Cell.Row.Cells["ItemCode"].Value.ToString(), DBConvert.ParseInt(e.Cell.Row.Cells["Pid"].Value), DBConvert.ParseInt(e.Cell.Row.Cells["CustomerPid"].Value)))
          {
            MessageBox.Show("Item Code is Existing already !");
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow; 
          }
          else
          {
            e.Cell.Row.Cells["ItemCode"].Appearance.BackColor = Color.White;
          }                         
          break;
          
        default:
          break;
      }
      btnSave.Enabled = true;
      this.SetNeedToSave();
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      this.btnSave.Enabled = true;
      this.SetNeedToSave();
    }
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.AutoFitColumns = true;  
      e.Layout.Bands[0].Columns["CustomerPid"].ValueList = ultraDropCustomer;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultraDropItemCode;
      e.Layout.Bands[0].Columns["CustomerPid"].Header.Caption = "Customer Name";
      e.Layout.Bands[0].Columns["SetQuantity"].Header.Caption = "Quantity";
      e.Layout.Bands[0].Columns["SetRoom"].Header.Caption = "Set Room";
      e.Layout.Bands[0].Columns["SetCode"].Header.Caption = "Set Code";
      e.Layout.Bands[0].Columns["SetColletion"].Header.Caption = "Collection";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      //e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly; 
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        else
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
        }
      }
      //Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;      

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      btnSave.Enabled = true;
      this.SetNeedToSave();
    }
    /// <summary>
    /// Bofore Row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    #endregion Event


  }
}