/*
  Author      : Lậm Quang Hà
  Date        : 07/10/2010
  Decription  : Insert, Update Forwarder
  Checked by    : Võ Hoa Lư
  Checked date  : 12/10/2010
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
namespace DaiCo.ERPProject
{
  public partial class viewCSD_01_002 : MainUserControl
  {
    #region Field
    public long forwardPid = long.MinValue;
    #endregion Field

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_01_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_01_002_Load(object sender, EventArgs e)
    {
      ControlUtility.LoadNation(cmbCountry);
      this.LoadData();
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Load forwarder's information
    /// </summary>
    private void LoadData()
    {
      if (this.forwardPid != long.MinValue)
      {
        CSDForwarder forwarder = new CSDForwarder();
        forwarder.Pid = this.forwardPid;
        forwarder = (CSDForwarder)DataBaseAccess.LoadObject(forwarder, new string[] { "Pid" });
        if (forwarder == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return;
        }
        txtCode.Text = forwarder.ForwarderCode;
        txtName.Text = forwarder.Name;
        txtTel.Text = forwarder.Tel;
        txtFax.Text = forwarder.Fax;
        txtEmail.Text = forwarder.Email;
        txtContactPerson.Text = forwarder.ContactPerson;
        try
        {
          cmbCountry.SelectedValue = forwarder.Country;
        }
        catch { }
        txtPostalCode.Text = forwarder.PostalCode;
        txtRegion.Text = forwarder.Region;
        txtCity.Text = forwarder.City;
        txtStreetAddress.Text = forwarder.StreetAdress;
        txtPOBox.Text = forwarder.POBox;
      }
      else
      {
        txtCode.Text = this.GetNewCode();
      }
      this.NeedToSave = false;
    }

    private string GetNewCode() {
      return DataBaseAccess.ExecuteScalarCommandText("Select dbo.FCSDGetNewForwardCode('FW')").ToString();
    }

    /// <summary>
    /// Clear screen, update this.forwardPid = long.MinValue, 
    /// </summary>
    private void Clear()
    {
      this.forwardPid = long.MinValue;
      txtCode.Text = this.GetNewCode();
      txtName.Text = string.Empty;
      txtTel.Text = string.Empty;
      txtFax.Text = string.Empty;
      txtEmail.Text = string.Empty;
      txtContactPerson.Text = string.Empty;
      cmbCountry.SelectedIndex = 0;
      txtPostalCode.Text = string.Empty;
      txtRegion.Text = string.Empty;
      txtCity.Text = string.Empty;
      txtStreetAddress.Text = string.Empty;
      txtPOBox.Text = string.Empty;
      this.NeedToSave = false;
    }
    #endregion Load Data

    #region Check & Save Data


    /// <summary>
    /// Check logic : forwarderCode and name are required
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      if (txtName.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Name" });
        txtName.Focus();
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Data : insert new record or update one record TblCSDForwarder in database
    /// </summary>
    private bool SaveData()
    {
      DBParameter[] inputParam = new DBParameter[15];
      string storeName = string.Empty;
      if (this.forwardPid != long.MinValue)
      {
        //Update
        storeName = "spCSDForwarder_Update";
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.forwardPid);
        inputParam[14] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }
      else
      {
        //Insert
        string strForwardCode = DataBaseAccess.ExecuteScalarCommandText("Select dbo.FCSDGetNewForwardCode('FW')", null).ToString();
        txtCode.Text = strForwardCode;
        storeName = "spCSDForwarder_Insert";
        inputParam[14] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }

      string text = txtCode.Text.Trim();
      inputParam[1] = new DBParameter("@ForwarderCode", DbType.AnsiString, 8, text);

      text = txtName.Text.Trim();
      inputParam[2] = new DBParameter("@Name", DbType.AnsiString, 128, text);

      text = txtTel.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[3] = new DBParameter("@Tel", DbType.AnsiString, 32, text);
      }

      text = txtFax.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[4] = new DBParameter("@Fax", DbType.AnsiString, 32, text);
      }

      text = txtEmail.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Email", DbType.AnsiString, 128, text);
      }

      text = txtContactPerson.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[6] = new DBParameter("@ContactPerson", DbType.AnsiString, 128, text);
      }

      inputParam[7] = new DBParameter("@DeleteFlag", DbType.Int32, 0);

      long country = DBConvert.ParseLong(ControlUtility.GetSelectedValueCombobox(cmbCountry));
      inputParam[8] = new DBParameter("@Country", DbType.Int64, country);

      text = txtPostalCode.Text.Trim();
      inputParam[9] = new DBParameter("@PostalCode", DbType.AnsiString, 16, text);

      text = txtRegion.Text.Trim();
      inputParam[10] = new DBParameter("@Region", DbType.String, 128, text);

      text = txtCity.Text.Trim();
      inputParam[11] = new DBParameter("@City", DbType.String, 128, text);

      text = txtStreetAddress.Text.Trim();
      inputParam[12] = new DBParameter("@StreetAdress", DbType.String, 256, text);

      text = txtPOBox.Text.Trim();
      if (text.Length > 0) {
        inputParam[13] = new DBParameter("@POBox", DbType.AnsiString, 8, text);
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return false;
      }
      this.forwardPid = result;
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
      return true;
    }

    /// <summary>
    /// Confirm before close
    /// YES : Save and close
    /// No  : Close without save
    /// Cancle : nothing
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      if (this.CheckInvalid())
      {
        this.SaveData();
      }
    }
    #endregion Check & Save Data

    #region Event
    /// <summary>
    /// Save Data : insert new record or update one record TblCSDForwarder in database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        this.SaveData();
      }  
    }

    /// <summary>
    /// Confirm Save and Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Description : 
    ///   1/ Insert new record or update one record TblCSDForwarder in database.
    ///   2/ Reset screen for save register new  TblCSDForwarder
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveContinute_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        bool sucess = this.SaveData();
        if (sucess)
        {
          this.Clear();
        }
      }
    }

    /// <summary>
    /// Warning if dupplicate name
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtName_Leave(object sender, EventArgs e)
    {
      string name = txtName.Text.Trim();
      if (name.Length == 0) {
        return;
      }
      string commandText = "SELECT Pid FROM TblCSDForwarder WHERE Name = @Name";
      DBParameter[] input = new DBParameter[]{ new DBParameter("@Name", DbType.AnsiString, 128, name)};
      object result = DataBaseAccess.ExecuteScalarCommandText(commandText, input);
      if (result == null) {
        return;
      }
      long pid = DBConvert.ParseLong(result.ToString());
      if(pid != long.MinValue && pid != this.forwardPid){
        WindowUtinity.ShowMessageWarning("WRN0018");
      }
    }

    /// <summary>
    /// Set flg NeedToSave = btnSave.Visible when another object change value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Obj_TextChanged(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
    }
    #endregion Event
  }
}
