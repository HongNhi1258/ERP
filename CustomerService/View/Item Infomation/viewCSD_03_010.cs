/*
  Author        : Vo Van Duy Qui
  Create date   : 05/09/2012
  Decription    : Item spare part info
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Application.Web.Mail;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_010 : MainUserControl
  {
    #region Field
    public string itemCode = string.Empty;
    private IList listCompDeleted = new ArrayList();
    private bool isNew = false;
    #endregion Field

    #region Init
    /// <summary>
    /// Init form
    /// </summary>
    public viewCSD_03_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_010_Load(object sender, EventArgs e)
    {
      this.LoadPrefix();
      ControlUtility.LoadUltraCBJC_OEM_Customer(ultCBCustomer);
      ControlUtility.LoadUltraCBCategory(ultCBCategory);
      ControlUtility.LoadUltraCBCollection(ultCBCollection);
      this.LoadItemKind();
      this.LoadUnit();
      this.LoadComponent();
      this.LoadItemInfo();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load item spare part info
    /// </summary>
    private void LoadItemInfo()
    {
      this.listCompDeleted = new ArrayList();
      int confirmed = 0;
      if (this.itemCode.Length > 0)
      {
        btnGetCode.Enabled = false;
        ultCBPrefix.ReadOnly = true;
        ultCBType.ReadOnly = true;
      }
      else
      {
        btnGetCode.Enabled = true;
        ultCBPrefix.ReadOnly = false;
        ultCBType.ReadOnly = false;
      }
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      
      //Master
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemSparepartInfo", inputParam);
      if (dtData.Rows.Count > 0)
      {
        ultCBPrefix.Value = this.itemCode.Substring(0, 6);
        ultCBType.Value = dtData.Rows[0]["ItemKind"];
        txtItemCode.Text = this.itemCode;
        txtSaleCode.Text = dtData.Rows[0]["SaleCode"].ToString();
        if (txtSaleCode.Text.Trim().Length > 0)
        {
          txtSaleCode.ReadOnly = true;
        }
        else
        {
          txtSaleCode.ReadOnly = false;
        }
        txtItemName.Text = dtData.Rows[0]["Name"].ToString();
        txtShortName.Text = dtData.Rows[0]["ShortName"].ToString();
        ultCBUnit.Value = dtData.Rows[0]["Unit"];
        ultCBCustomer.Value = dtData.Rows[0]["CustomerPid"];
        ultCBCategory.Value = dtData.Rows[0]["Category"];
        ultCBCollection.Value = dtData.Rows[0]["Collection"];
        txtDesc.Text = dtData.Rows[0]["Description"].ToString();
        txtWidthmm.Text = dtData.Rows[0]["MMWidth"].ToString();
        txtDepthmm.Text = dtData.Rows[0]["MMDepth"].ToString();
        txtHeightmm.Text = dtData.Rows[0]["MMHigh"].ToString();
        txtWidthinch.Text = FunctionUtility.ConverMilimetToInch(DBConvert.ParseInt(dtData.Rows[0]["MMWidth"].ToString()));
        txtDepthinch.Text = FunctionUtility.ConverMilimetToInch(DBConvert.ParseInt(dtData.Rows[0]["MMDepth"].ToString()));
        txtHeightinch.Text = FunctionUtility.ConverMilimetToInch(DBConvert.ParseInt(dtData.Rows[0]["MMHigh"].ToString()));
        confirmed = DBConvert.ParseInt(dtData.Rows[0]["Confirm"].ToString());
      }
      // Item Component
      DataTable dtItemComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMItemComp_Select", inputParam);
      if (dtItemComp != null)
      {
        dtItemComp.PrimaryKey = new DataColumn[] { dtItemComp.Columns["ComponentCode"] };
        ultComp.DataSource = dtItemComp;
      }
      if (confirmed == 1)
      { 
        txtItemCode.ReadOnly = true;
        txtSaleCode.ReadOnly = true;
        txtItemName.ReadOnly = true;
        txtShortName.ReadOnly = true;
        ultCBUnit.ReadOnly = true;
        ultCBCustomer.ReadOnly = true;
        ultCBCategory.ReadOnly = true;
        ultCBCollection.ReadOnly = true;
        txtDesc.ReadOnly = true;
        txtWidthmm.ReadOnly = true;
        txtDepthmm.ReadOnly = true;
        txtHeightmm.ReadOnly = true;
        txtWidthinch.ReadOnly = true;
        txtDepthinch.ReadOnly = true;
        txtHeightinch.ReadOnly = true;
        btnClear.Enabled = false;
        btnSave.Enabled = false;
        btnSaveContinue.Enabled = false;

        ultComp.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
        ultComp.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
        ultComp.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
      }
      this.NeedToSave = false;
    }

    /// <summary>
    /// Load combo prefix code
    /// </summary>
    private void LoadPrefix()
    {
      string commandText = string.Format("SELECT DISTINCT LEFT(ItemCode, 6) PrefixCode FROM TblRDDItemInfo ORDER BY LEFT(ItemCode, 6) DESC");
      DataTable dtPrefixCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultCBPrefix, dtPrefixCode, "PrefixCode", "PrefixCode", false);
    }

    private void LoadItemKind()
    {
      string commandText = string.Format("SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND DeleteFlag = 0 ORDER BY Sort", ConstantClass.GROUP_ITEMKIND);
      DataTable dtItemKind = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtItemKind != null)
      {
        ControlUtility.LoadUltraCombo(ultCBType, dtItemKind, "Code", "Value", "Code");
        ultCBType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }
    }

    /// <summary>
    /// Load combo Unit
    /// </summary>
    private void LoadUnit()
    {
      string commandText = string.Format("SELECT UnitPid FROM TblBOMUnit ORDER BY UnitPid");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultCBUnit, dtSource, "UnitPid", "UnitPid", false);
    }

    /// <summary>
    /// Load dropdown Component
    /// </summary>
    private void LoadComponent()
    {
      string commandText = string.Format(@"SELECT COM.Code + ISNULL('|' + CAST(COM.Revision AS VARCHAR), '') Value, 
                                                COM.Code, COM.Revision, COM.Name, COM.NameVn, COM.Length, 
                                                COM.Width, COM.Thickness, COM.ContractOut, COM.CompGroup, MST.Value [Group]
                                          FROM VBOMComponent COM LEFT JOIN TblBOMCodeMaster MST ON (COM.CompGroup = MST.Code AND MST.[Group] = 9)
                                          WHERE COM.CompGroup <> 3
                                          ORDER BY CompGroup, Code, Revision");
      ultDDCompCode.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDCompCode.ValueMember = "Value";
      ultDDCompCode.DisplayMember = "Code";
      ultDDCompCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["CompGroup"].Hidden = true;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["Code"].Width = 80;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["Revision"].Width = 60;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["NameVn"].Width = 300;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["Length"].Width = 50;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["Width"].Width = 50;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["Thickness"].Width = 60;
      ultDDCompCode.DisplayLayout.Bands[0].Columns["ContractOut"].Hidden = true;
      ultDDCompCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Get new Item code
    /// </summary>
    /// <param name="prefixCode"></param>
    /// <returns></returns>
    private string GetNewCode(string prefixCode, int itemKind)
    {
      string newCode = string.Empty;
      string commandExec = "SELECT dbo.FRDDGetAutoItemCode(@PrefixCode, @ItemKind)";
      DBParameter[] inputParam = new DBParameter[2];
      if (prefixCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@PrefixCode", DbType.AnsiString, 8, prefixCode);
      }
      else
      {
        inputParam[0] = new DBParameter("@PrefixCode", DbType.AnsiString, 8, DBNull.Value);
      }
      inputParam[1] = new DBParameter("@ItemKind", DbType.Int32, itemKind);
      
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandExec, inputParam);
      if (obj != null && obj.ToString().Trim().Length > 0)
      {
        newCode = obj.ToString().Trim();
      }
      return newCode;
    }

    /// <summary>
    /// Check valid data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    { 
      // Get input data
      string itemCode = txtItemCode.Text.Trim();
      string itemName = txtItemName.Text.Trim();
      string saleCode = txtSaleCode.Text.Trim();

      if (itemCode.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Item Code");
        txtItemCode.Focus();
        return false;
      }

      if (itemName.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Item Name");
        txtItemName.Focus();
        return false;
      }

      int width = DBConvert.ParseInt(txtWidthmm.Text);
      if (width == int.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Width");
        txtWidthmm.Focus();
        return false;
      }
      int depth = DBConvert.ParseInt(txtDepthmm.Text);
      if (depth == int.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Depth");
        txtDepthmm.Focus();
        return false;
      }

      int high = DBConvert.ParseInt(txtHeightmm.Text);
      if (high == int.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0005", "High");
        txtHeightmm.Focus();
        return false;
      }

      if (saleCode.Length > 0)
      {
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, saleCode);
        string commandTextSaleCode = "SELECT dbo.FBOMCheckSaleCode(@ItemCode, @SaleCode)";
        int countSaleCode = (int)DataBaseAccess.ExecuteScalarCommandText(commandTextSaleCode, inputParam);

        if (countSaleCode > 0)
        {
          WindowUtinity.ShowMessageError("MSG0006", "Sale Code");
          txtSaleCode.Focus();
          return false;
        }
      }
      if (ultCBCustomer.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        ultCBCustomer.Focus();
        return false;
      }
      if (ultCBCategory.SelectedRow == null && ultCBCategory.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0011", "Category");
        ultCBCategory.Focus();
        return false;
      }
      if (ultCBCollection.SelectedRow == null && ultCBCollection.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0011", "Collection");
        ultCBCollection.Focus();
        return false;
      }
      if (ultCBUnit.SelectedRow == null && ultCBUnit.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0011", "Unit");
        ultCBUnit.Focus();
        return false;
      }
      // Item lv2
      DataTable dtCheck = (DataTable)ultComp.DataSource;
      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        foreach (DataRow rowCheck in dtCheck.Rows)
        {
          int rowNumber = rowCheck.Table.Rows.IndexOf(rowCheck) + 1;
          if (rowCheck.RowState == DataRowState.Added || rowCheck.RowState == DataRowState.Modified)
          {
            string compCode = rowCheck["ComponentCode"].ToString().Trim();
            double qty = DBConvert.ParseDouble(rowCheck["Qty"].ToString());
            if (compCode.Length == 0)
            {
              WindowUtinity.ShowMessageError("MSG0011", "Component Code");
              this.SaveSuccess = false;
              return false;
            }
            if (qty <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0130", rowNumber.ToString(), "Qty");
              this.SaveSuccess = false;
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Exist ItemCode
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns> true :  Not exist item code
    ///           false:  Exist item code
    /// </returns>
    private bool CheckExistItemCode(string itemCode)
    {
      string commandText = string.Format("SELECT COUNT(*) FROM TblRDDItemInfo WHERE ItemCode = '{0}'", itemCode);
      int count = (int)DataBaseAccess.ExecuteScalarCommandText(commandText);
      return count > 0 ? false : true;
    }

    /// <summary>
    /// Save data
    /// </summary>
    private void SaveItem()
    {
      if (this.CheckValid())
      {
        // Get input data
        bool resultEditGrid = true;
        string itemCode = txtItemCode.Text.Trim();
        string saleCode = txtSaleCode.Text.Trim();
        string itemName = txtItemName.Text.Trim();
        string shortName = txtShortName.Text.Trim();
        int customerPid = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBCustomer));
        string unit = ControlUtility.GetSelectedValueUltraCombobox(ultCBUnit);
        int category = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBCategory));
        int collection = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBCollection));
        string desc = txtDesc.Text.Trim();
        int width = DBConvert.ParseInt(txtWidthmm.Text);
        int depth = DBConvert.ParseInt(txtDepthmm.Text);
        int hight = DBConvert.ParseInt(txtHeightmm.Text);
        int itemKind = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBType));

        // Edit data
        string storeName = string.Empty;
        DBParameter[] input = new DBParameter[14];
        if (this.CheckExistItemCode(itemCode))
        {
          storeName = "spCSDItemSparepart_Insert";
          input[12] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          this.isNew = true;
        }
        else
        {
          storeName = "spCSDItemSparepart_Update";
          input[12] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          this.isNew = false;
        }
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        if (saleCode.Length > 0)
        {
          input[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, saleCode);
        }
        input[2] = new DBParameter("@Name", DbType.AnsiString, 256, itemName);
        if (shortName.Length > 0)
        {
          input[3] = new DBParameter("@ShortName", DbType.AnsiString, 128, shortName);
        }
        if (customerPid != int.MinValue)
        {
          input[4] = new DBParameter("@CustomerPid", DbType.Int32, customerPid);
        }
        if (unit.Length > 0)
        {
          input[5] = new DBParameter("@Unit", DbType.AnsiString, 8, unit);
        }
        if (category != int.MinValue)
        {
          input[6] = new DBParameter("@Category", DbType.Int32, category);
        }
        if (collection != int.MinValue)
        {
          input[7] = new DBParameter("@Collection", DbType.Int32, collection);
        }
        if (desc.Length > 0)
        {
          input[8] = new DBParameter("@Description", DbType.AnsiString, 1024, desc);
        }
        if (width != int.MinValue)
        {
          input[9] = new DBParameter("@Width", DbType.Int32, width);
        }
        if (depth != int.MinValue)
        {
          input[10] = new DBParameter("@Depth", DbType.Int32, depth);
        }
        if (hight != int.MinValue)
        {
          input[11] = new DBParameter("@High", DbType.Int32, hight);
        }
        input[13] = new DBParameter("@ItemKind", DbType.Int32, itemKind);
        
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
        int resultEdit = DBConvert.ParseInt(output[0].Value.ToString());
        if (resultEdit != 1)
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
          return;
        }
        else
        {
          this.itemCode = itemCode;
          DataTable dtComp = (DataTable)ultComp.DataSource;
          if (dtComp != null && dtComp.Rows.Count > 0)
          {
            foreach (string compCode in this.listCompDeleted)
            {
              string compCodeDel = compCode.Split('|')[0];
              int compRev = DBConvert.ParseInt(compCode.Split('|')[1]);
              int compGroup = DBConvert.ParseInt(compCode.Split('|')[2]);

              DBParameter[] inputDel = new DBParameter[4];
              inputDel[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
              inputDel[1] = new DBParameter("@CompCode", DbType.AnsiString, 16, compCodeDel);
              if (compRev != int.MinValue)
              {
                inputDel[2] = new DBParameter("@CompRev", DbType.Int32, compRev);
              }
              inputDel[3] = new DBParameter("@CompGroup", DbType.Int32, compGroup);

              DBParameter[] outputDel = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spCSDItemCompSparepart_Delete", inputDel, outputDel);
              long resultDel = DBConvert.ParseLong(outputDel[0].Value.ToString());
              if (resultDel != 1)
              {
                resultEditGrid = false;
              }
            }
            foreach (DataRow row in dtComp.Rows)
            {
              if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
              {
                // Get input data
                string storeNameComp = string.Empty;
                string compCode = row["ComponentCode"].ToString().Trim().Split('|')[0];
                string oldCompCode = row["OldCompCode"].ToString().Trim();
                int compRev = DBConvert.ParseInt(row["CompRevision"].ToString());
                int oldCompRev = DBConvert.ParseInt(row["OldCompRev"].ToString());
                int compGroup = DBConvert.ParseInt(row["CompGroup"].ToString());
                double qty = DBConvert.ParseDouble(row["Qty"].ToString());
                double lengthComp = DBConvert.ParseDouble(row["Length"].ToString());
                double widthComp = DBConvert.ParseDouble(row["Width"].ToString());
                double thicknessComp = DBConvert.ParseDouble(row["Thickness"].ToString());

                DBParameter[] inputComp = new DBParameter[11];
                if (row.RowState == DataRowState.Added)
                {
                  storeNameComp = "spCSDItemCompSparepart_Insert";
                  inputComp[10] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
                }
                else if (row.RowState == DataRowState.Modified)
                {
                  storeNameComp = "spCSDItemCompSparepart_Update";
                  inputComp[1] = new DBParameter("@OldCompCode", DbType.AnsiString, 16, oldCompCode);
                  if (oldCompRev != int.MinValue)
                  {
                    inputComp[3] = new DBParameter("@OldCompRev", DbType.Int32, oldCompRev);
                  }
                  inputComp[10] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
                }
                inputComp[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
                inputComp[2] = new DBParameter("@CompCode", DbType.AnsiString, 16, compCode);
                if (compRev != int.MinValue)
                {
                  inputComp[4] = new DBParameter("@CompRev", DbType.Int32, compRev);
                }
                if (compGroup != int.MinValue)
                {
                  inputComp[5] = new DBParameter("@CompGroup", DbType.Int32, compGroup);
                }
                inputComp[6] = new DBParameter("@Qty", DbType.Double, qty);
                if (lengthComp != double.MinValue)
                {
                  inputComp[7] = new DBParameter("@Length", DbType.Double, lengthComp);
                }
                if (widthComp != double.MinValue)
                {
                  inputComp[8] = new DBParameter("@Width", DbType.Double, widthComp);
                }
                if (thicknessComp != double.MinValue)
                {
                  inputComp[9] = new DBParameter("@Thickness", DbType.Double, thicknessComp);
                }

                DBParameter[] outputComp = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                DataBaseAccess.ExecuteStoreProcedure(storeNameComp, inputComp, outputComp);
                long resultEditComp = DBConvert.ParseLong(outputComp[0].Value.ToString());
                if (resultEditComp != 1)
                {
                  resultEditGrid = false;
                }
              }
            }
          }
        }
        if (!resultEditGrid)
        {
          WindowUtinity.ShowMessageError("WRN0004");
          this.SaveSuccess = false;
        }
        else
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
          if (this.isNew)
          {
            this.SendEmail(saleCode, itemCode);
          }
        }
        this.LoadItemInfo();
      }
    }

    /// <summary>
    /// Send Email
    /// </summary>
    private void SendEmail(string saleCode, string itemCode)
    {
      // Mail
      MailMessage mailMessage = new MailMessage();
      string body = string.Empty;
      saleCode = (saleCode.Length > 0) ? saleCode + "/" : "";
      try
      {
        body += "<p><i><font color='6699CC'>";
        body += "Dear all, <br><br>";
        body += "We'd like to inform that " + saleCode + itemCode + " has just created by Customer Service... <br>";
        body += "Please arrange your time to do next steps ASAP! <br><br>";
        body += "For Customer Service team.";
      }
      catch (Exception e)
      {
        MessageBox.Show("Message 1: " + e.Message);
      }
      string mailTo = string.Empty;
      try
      {
        mailTo = "tu@daico-furniture.com, chau_rd@daico-furniture.com";
        mailMessage.ServerName = "10.0.0.5";
        mailMessage.Username = "dc@daico-furniture.com";
        mailMessage.Password = "dc123456";
        mailMessage.From = "dc@daico-furniture.com";
        mailMessage.To = mailTo;
        mailMessage.Subject = "NEW CUSTOM/SPAREPART CODE";
        mailMessage.Body = body;
        mailMessage.Bcc = "thuy_cs@daico-furniture.com, ngan_cs@daico-furniture.com, trang.hoang@daico-furniture.com, thu_cs@daico-furniture.com, quoc_tech@daico-furniture.com, trung@daico-furniture.com";
        IList attachments = new ArrayList();
        mailMessage.Attachfile = attachments;
        mailMessage.SendMail(true);
      }
      catch (Exception e)
      {
        MessageBox.Show("Message 2:" + e.Message);
      }
    }

    /// <summary>
    /// Clear input data
    /// </summary>
    private void ClearInputData()
    {
      ultCBPrefix.ReadOnly = false;
      ultCBPrefix.Value = string.Empty;
      ultCBType.Value = string.Empty;
      btnGetCode.Enabled = true;
      txtItemCode.Text = string.Empty;
      txtSaleCode.Text = string.Empty;
      txtItemName.Text = string.Empty;
      txtShortName.Text = string.Empty;
      ultCBUnit.Value = string.Empty;
      ultCBCustomer.Value = string.Empty;
      ultCBCategory.Value = string.Empty;
      ultCBCollection.Value = string.Empty;
      txtDesc.Text = string.Empty;
      txtWidthmm.Text = string.Empty;
      txtWidthinch.Text = string.Empty;
      txtDepthmm.Text = string.Empty;
      txtDepthinch.Text = string.Empty;
      txtHeightmm.Text = string.Empty;
      txtHeightinch.Text = string.Empty;
      DataTable dtSource = (DataTable)ultComp.DataSource;
      dtSource.Rows.Clear();
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Event button 'GetCode' click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetCode_Click(object sender, EventArgs e)
    {
      string prefix = ControlUtility.GetSelectedValueUltraCombobox(ultCBPrefix).Trim();
      int itemKind = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBType));
      if (prefix.Length > 0 && itemKind != int.MinValue)
      {
        string newItemCode = this.GetNewCode(prefix, itemKind);
        txtItemCode.Text = newItemCode;
      }
    }

    /// <summary>
    /// Event button 'SaveContinue' click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveContinue_Click(object sender, EventArgs e)
    {
      this.SaveItem();
      this.ClearInputData();
    }

    /// <summary>
    /// Event button 'Save' click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveItem();
    }

    /// <summary>
    /// Event button 'Clear' click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearInputData();
    }

    /// <summary>
    /// Event button 'Close' click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Initialize layout of ultragrid ultComp
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComp_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ComponentCode"].ValueList = ultDDCompCode;
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["CompGroup"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCompCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCompRev"].Hidden = true;
      e.Layout.Bands[0].Columns["CompRevision"].Header.Caption = "Comp Revision";
      e.Layout.Bands[0].Columns["CompRevision"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["CompRevision"].MinWidth = 90;
      e.Layout.Bands[0].Columns["CompRevision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Length"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Thickness"].MinWidth = 60;

      e.Layout.Bands[0].Columns["CompName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CompName"].Header.Caption = "Name";
      e.Layout.Bands[0].Columns["CompName"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["ContractOut"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["ContractOut"].MinWidth = 90;
      e.Layout.Bands[0].Columns["Length"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Width"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Thickness"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["ContractOut"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ContractOut"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["CompRevision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CompRevision"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Group"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Group"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Group"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Group"].MinWidth = 90;
    }

    /// <summary>
    /// Event After Cell Update of ultragrid ultComp
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComp_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.NeedToSave = true;
      string columnName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      if (ultDDCompCode.SelectedRow != null)
      {
        switch (columnName)
        {
          case "componentcode":
            ultComp.Rows[index].Cells["CompName"].Value = ultDDCompCode.SelectedRow.Cells["Name"].Value;
            ultComp.Rows[index].Cells["Length"].Value = ultDDCompCode.SelectedRow.Cells["Length"].Value;
            ultComp.Rows[index].Cells["Width"].Value = ultDDCompCode.SelectedRow.Cells["Width"].Value;
            ultComp.Rows[index].Cells["Thickness"].Value = ultDDCompCode.SelectedRow.Cells["Thickness"].Value;
            ultComp.Rows[index].Cells["CompGroup"].Value = ultDDCompCode.SelectedRow.Cells["CompGroup"].Value;
            ultComp.Rows[index].Cells["Group"].Value = ultDDCompCode.SelectedRow.Cells["Group"].Value;
            ultComp.Rows[index].Cells["ContractOut"].Value = ultDDCompCode.SelectedRow.Cells["ContractOut"].Value;
            ultComp.Rows[index].Cells["CompRevision"].Value = ultDDCompCode.SelectedRow.Cells["Revision"].Value;
            break;
          default:
            break;
        }
      }
    }

    /// <summary>
    /// Event textbox Dimention Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Object_Leave(object sender, EventArgs e)
    {
      Control ctrl = (TextBox)sender;
      string value = ctrl.Text.Trim();
      if (value.Length > 0)
      {
        int mm = DBConvert.ParseInt(value);
        string inches = FunctionUtility.ConverMilimetToInch(mm);
        if (ctrl.Name.CompareTo("txtWidthmm") == 0)
        {
          txtWidthinch.Text = inches;
        }
        else if (ctrl.Name.CompareTo("txtDepthmm") == 0)
        {
          txtDepthinch.Text = inches;
        }
        else if (ctrl.Name.CompareTo("txtHeightmm") == 0)
        {
          txtHeightinch.Text = inches;
        }
      }
    }

    /// <summary>
    /// Event before rows deleted of ultragird ultComp
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComp_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.NeedToSave = true;
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        string compCode = row.Cells["ComponentCode"].Value.ToString().Trim() + "|" + row.Cells["CompRevision"].Value.ToString().Trim() + "|" + row.Cells["CompGroup"].Value.ToString();
        if (pid != long.MinValue && compCode.Length > 0)
        {
          this.listCompDeleted.Add(compCode);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.NeedToSave = true;
    }
    #endregion Event

    private void txtSaleCode_Leave(object sender, EventArgs e)
    {
      if (txtSaleCode.Text.Trim().Length > 16)
      {
        WindowUtinity.ShowMessageErrorFromText("NewSaleCode length must be less than 16 characters");
        txtSaleCode.Text = "";
        txtSaleCode.Focus();
      }
    }
  }
}
