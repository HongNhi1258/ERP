/*
  Author      : Huynh Thi Bang
  Date        : 18/12/2017
  Description : 
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewHRD_02_001 : MainUserControl
  {
    #region field
    private int eid = int.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      udtBirthday.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
      udtBirthday.FormatString = ConstantClass.FORMAT_DATETIME;

      udtStartDate.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
      udtStartDate.FormatString = ConstantClass.FORMAT_DATETIME;

      udtIDIssuedDate.FormatProvider = new System.Globalization.CultureInfo(ConstantClass.FORMAT_PROVIDER);
      udtIDIssuedDate.FormatString = ConstantClass.FORMAT_DATETIME;

      // Load Employee
      this.LoadEmployee();

      // Load Race 
      Utility.LoadUltraCBMasterData(ucbRace, 1);

      // Load Religion 
      Utility.LoadUltraCBMasterData(ucbReligion, 2);

      // Load Education
      Utility.LoadUltraCBMasterData(ucbEducation, 3);

      // Load Material Status
      Utility.LoadUltraCBMasterData(ucbMaritalStatus, 4);

      // Load Gender
      Utility.LoadUltraCBMasterData(ucbGender, 7);

      // Load Employee type
      Utility.LoadUltraCBMasterData(ucbEmpType, 12);

      // Load Employee group
      Utility.LoadUltraCBMasterData(ucbEmpGroup, 13);

      //Load Country
      this.LoadUltraCBCountry();

      //Load City
      this.LoadUltraCBCity();

      //Load District
      this.LoadUltraDistrict();

      //Load Current country
      this.LoadUltraCBCurrentCountry();

      //Load Current City
      this.LoadUltraCBCurrentCity();

      //Load current District
      this.LoadUltraCurrentDistrict();

      //Load Birthday Place
      this.LoadUltraCBBirthdayPlace();

      //Load Issued By
      this.LoadUltraCBIssuedBy();

    }
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      this.NeedToSave = false;
      int paramNumber = 1;
      string storeName = "spHRDDBEmployee_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@Eid", DbType.Int32, this.eid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        DataRow row = dtSource.Rows[0];
        utxtEmpID.Value = row["EID"];
        utxtEmpName.Text = row["EmpName"].ToString();
        utxtWorkingStatus.Text = row["WorkingStatus"].ToString();
        if (row["BirthPlace"] != DBNull.Value)
        {
          ucbBirthPlace.Value = row["BirthPlace"];
        }
        else
        {
          ucbBirthPlace.Value = null;
        }
        if (row["Birthday"] != DBNull.Value)
        {
          udtBirthday.Value = DateTime.Parse(row["Birthday"].ToString());
        }
        else
        {
          udtBirthday.Value = null;
        }
        if (row["StartDate"] != DBNull.Value)
        {
          udtStartDate.Value = DateTime.Parse(row["StartDate"].ToString());
        }
        else
        {
          udtStartDate.Value = null;
        }
        if (row["IDCardIssuedDate"] != DBNull.Value)
        {
          udtIDIssuedDate.Value = DateTime.Parse(row["IDCardIssuedDate"].ToString());
        }
        else
        {
          udtIDIssuedDate.Value = null;
        }
        if (row["GenderPid"] != DBNull.Value)
        {
          ucbGender.Value = row["GenderPid"];
        }
        else
        {
          ucbGender.Value = null;
        }
        if (row["RacePid"] != DBNull.Value)
        {
          ucbRace.Value = row["RacePid"];
        }
        else
        {
          ucbRace.Value = null;
        }
        if (row["ReligionPid"] != DBNull.Value)
        {
          ucbReligion.Value = row["ReligionPid"];
        }
        else
        {
          ucbReligion.Value = null;
        }
        if (row["IDCardNo"] != DBNull.Value)
        {
          utxtIDCardNo.Text = row["IDCardNo"].ToString();
        }
        else
        {
          utxtIDCardNo.Text = string.Empty;
        }
        if (row["IDCardIssuedBy"] != DBNull.Value)
        {
          ucbIDIssuedBy.Value = row["IDCardIssuedBy"];
        }
        else
        {
          ucbIDIssuedBy.Value = null;
        }
        if (row["EducationPid"] != DBNull.Value)
        {
          ucbEducation.Value = row["EducationPid"];
        }
        else
        {
          ucbEducation.Value = null;
        }
        if (row["MaritalStatusPid"] != DBNull.Value)
        {
          ucbMaritalStatus.Value = row["MaritalStatusPid"];
        }
        else
        {
          ucbMaritalStatus.Value = null;
        }
        if (row["TaxCode"] != DBNull.Value)
        {
          utxtTaxCode.Text = row["TaxCode"].ToString();
        }
        else
        {
          utxtTaxCode.Text = string.Empty;
        }
        if (row["AttendanceCardNo"] != DBNull.Value)
        {
          utxtAttCardNo.Text = row["AttendanceCardNo"].ToString();
        }
        else
        {
          utxtAttCardNo.Text = string.Empty;
        }
        if (row["EmpTypePid"] != DBNull.Value)
        {
          ucbEmpType.Value = row["EmpTypePid"];
        }
        else
        {
          ucbEmpType.Value = null;
        }
        if (row["EmpGroupPid"] != DBNull.Value)
        {
          ucbEmpGroup.Value = row["EmpGroupPid"];
        }
        else
        {
          ucbEmpGroup.Value = null;
        }
        if (row["HomePhone"] != DBNull.Value)
        {
          utxtHomePhone.Text = row["HomePhone"].ToString();
        }
        else
        {
          utxtHomePhone.Text = string.Empty;
        }
        if (row["MobilePhone"] != DBNull.Value)
        {
          utxtMobilePhone.Text = row["MobilePhone"].ToString();
        }
        else
        {
          utxtMobilePhone.Text = string.Empty;
        }
        if (row["PersonalEmail"] != DBNull.Value)
        {
          utxtPersonalEmail.Text = row["PersonalEmail"].ToString();
        }
        else
        {
          utxtPersonalEmail.Text = string.Empty;
        }
        if (row["CompanyEmail"] != DBNull.Value)
        {
          utxtCompanyEmail.Text = row["CompanyEmail"].ToString();
        }
        else
        {
          utxtCompanyEmail.Text = string.Empty;
        }
        if (row["PerHouseNo"] != DBNull.Value)
        {
          utxtPerHouseNo.Text = row["PerHouseNo"].ToString();
        }
        else
        {
          utxtPerHouseNo.Text = string.Empty;
        }
        if (row["PerStreet"] != DBNull.Value)
        {
          utxtPerStreet.Text = row["PerStreet"].ToString();
        }
        else
        {
          utxtPerStreet.Text = string.Empty;
        }
        if (row["PerWard"] != DBNull.Value)
        {
          utxtPerWard.Text = row["PerWard"].ToString();
        }
        else
        {
          utxtPerWard.Text = string.Empty;
        }
        if (row["PerCountryPid"] != DBNull.Value)
        {
          ucbPerCountry.Value = row["PerCountryPid"];
        }
        else
        {
          ucbPerCountry.Value = null;
        }
        if (row["PerCityPid"] != DBNull.Value)
        {
          ucbPerCity.Value = row["PerCityPid"];
        }
        else
        {
          ucbPerCity.Value = null;
        }
        if (row["PerDistrictPid"] != DBNull.Value)
        {
          ucbPerDistrict.Value = row["PerDistrictPid"];
        }
        else
        {
          ucbPerDistrict.Value = null;
        }
        if (row["CurAddressNo"] != DBNull.Value)
        {
          utxtCurHouseNo.Text = row["CurAddressNo"].ToString();
        }
        else
        {
          utxtCurHouseNo.Text = string.Empty;
        }
        if (row["CurStreet"] != DBNull.Value)
        {
          utxtCurStreet.Text = row["CurStreet"].ToString();
        }
        else
        {
          utxtCurStreet.Text = string.Empty;
        }
        if (row["CurWard"] != DBNull.Value)
        {
          utxtCurWard.Text = row["CurWard"].ToString();
        }
        else
        {
          utxtCurWard.Text = string.Empty;
        }
        if (row["CurCountryPid"] != DBNull.Value)
        {
          ucbCurCountry.Value = row["CurCountryPid"];
        }
        else
        {
          ucbCurCountry.Value = null;
        }
        if (row["CurCityPid"] != DBNull.Value)
        {
          ucbCurCity.Value = row["CurCityPid"];
        }
        else
        {
          ucbCurCity.Value = null;
        }
        if (row["CurDistrictPid"] != DBNull.Value)
        {
          ucbCurDistrict.Value = row["CurDistrictPid"];
        }
        else
        {
          ucbCurDistrict.Value = null;
        }
        if (row["BankName"] != DBNull.Value)
        {
          utxtBankName.Text = row["BankName"].ToString();
        }
        else
        {
          utxtBankName.Text = string.Empty;
        }
        if (row["BankBranch"] != DBNull.Value)
        {
          utxtBankBranch.Text = row["BankBranch"].ToString();
        }
        else
        {
          utxtBankBranch.Text = string.Empty;
        }
        if (row["BankAccountNo"] != DBNull.Value)
        {
          utxtBankAccountNo.Text = row["BankAccountNo"].ToString();
        }
        else
        {
          utxtBankAccountNo.Text = string.Empty;
        }
      }


      btnSave.Enabled = true;
    }
    private void LoadEmployee()
    {
      string commandEmployee = "SELECT EID, CAST(EID AS varchar) +' - '+ EmpName Name FROM TblHRDDBEmployee";
      DataTable data = DataBaseAccess.SearchCommandTextDataTable(commandEmployee);
      Utility.LoadUltraCombo(ucbEmployeeList, data, "EID", "Name", false, "EID");
      ucbEmployeeList.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
    }
    /// <summary>
    /// Load Country
    /// </summary>
    private void LoadUltraCBCountry()
    {
      string commandText = string.Format(@"SELECT Pid, CountryCode +' - '+ CountryName Name FROM TblHRDDBCountry WHERE IsDeleted = 0");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbPerCountry, dtSource, "Pid", "Name", false, "Pid");
      ucbPerCountry.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
    }
    /// <summary>
    /// Load City
    /// </summary>
    private void LoadUltraCBCity()
    {
      if (ucbPerCountry.Value != null)
      {
        string commandText = string.Format(@"SELECT Pid, CityName FROM TblHRDDBCity WHERE IsDeleted = 0 AND CountryPid = {0}", ucbPerCountry.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbPerCity, dtSource, "Pid", "CityName", false, "Pid");
        ucbPerCity.DisplayLayout.Bands[0].Columns["CityName"].Width = 200;
      }
    }
    /// <summary>
    /// Load District
    /// </summary>
    private void LoadUltraDistrict()
    {
      if (ucbPerCountry.Value != null)
      {
        string commandText = string.Format(@"SELECT Pid, DistrictCode +' - '+ DistrictName Name FROM TblHRDDBDistrict WHERE IsDeleted = 0 AND CityPid = {0}", ucbPerCity.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbPerDistrict, dtSource, "Pid", "Name", false, "Pid");
        ucbPerDistrict.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      }
    }
    /// <summary>
    /// Load Current Country
    /// </summary>
    private void LoadUltraCBCurrentCountry()
    {
      string commandText = string.Format(@"SELECT Pid, CountryCode +' - '+ CountryName Name FROM TblHRDDBCountry WHERE IsDeleted = 0");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCurCountry, dtSource, "Pid", "Name", false, "Pid");
      ucbCurCountry.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
    }
    /// <summary>
    /// Load City
    /// </summary>
    private void LoadUltraCBCurrentCity()
    {
      if (ucbCurCountry.Value != null)
      {
        string commandText = string.Format(@"SELECT Pid, CityName FROM TblHRDDBCity WHERE IsDeleted = 0 AND CountryPid = {0}", ucbCurCountry.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbCurCity, dtSource, "Pid", "CityName", false, "Pid");
        ucbCurCity.DisplayLayout.Bands[0].Columns["CityName"].Width = 200;
      }
    }
    /// <summary>
    /// Load District
    /// </summary>
    private void LoadUltraCurrentDistrict()
    {
      if (ucbCurCity.Value != null)
      {
        string commandText = string.Format(@"SELECT Pid, DistrictCode +' - '+ DistrictName Name FROM TblHRDDBDistrict WHERE IsDeleted = 0 AND CityPid = {0}", ucbCurCity.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbCurDistrict, dtSource, "Pid", "Name", false, "Pid");
        ucbCurDistrict.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      }
    }

    /// <summary>
    /// Load Birthday Place
    /// </summary>
    private void LoadUltraCBBirthdayPlace()
    {
      string commandText = string.Format(@"SELECT Pid, CityName FROM TblHRDDBCity WHERE IsDeleted = 0");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbBirthPlace, dtSource, "Pid", "CityName", false, "Pid");
      ucbBirthPlace.DisplayLayout.Bands[0].Columns["CityName"].Width = 200;
    }
    /// <summary>
    /// Load Birthday Place
    /// </summary>
    private void LoadUltraCBIssuedBy()
    {
      string commandText = string.Format(@"SELECT Pid, CityName FROM TblHRDDBCity WHERE IsDeleted = 0");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbIDIssuedBy, dtSource, "Pid", "CityName", false, "Pid");
      ucbIDIssuedBy.DisplayLayout.Bands[0].Columns["CityName"].Width = 200;
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      if (ucbEmployeeList.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Employee");
        return;
      }
      else
      {
        eid = DBConvert.ParseInt(ucbEmployeeList.Value);
      }
      btnSearch.Enabled = false;
      this.LoadData();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
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
    /// Check valid
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (utxtEmpID.Text.Trim().Length == 0)
      {
        errorMessage = "EID is invalid";
        utxtEmpID.Focus();
        return false;
      }
      if (utxtEmpName.Text.Trim().Length == 0)
      {
        errorMessage = "Employee is invalid";
        utxtEmpName.Focus();
        return false;
      }
      if (utxtWorkingStatus.Text.Trim().Length == 0)
      {
        errorMessage = "Working Status is invalid";
        utxtWorkingStatus.Focus();
        return false;
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

        // Insert / Update
        DBParameter[] inputParam = new DBParameter[38];

        //Basic Infomation
        int eid = DBConvert.ParseInt(utxtEmpID.Text.Trim());
        string employee = utxtEmpName.Text.Trim();
        string cardNo = utxtIDCardNo.Text.Trim();
        int birthdayPlace = DBConvert.ParseInt(ucbBirthPlace.Value);
        int cardIssuedBy = DBConvert.ParseInt(ucbIDIssuedBy.Value);
        int gender = DBConvert.ParseInt(ucbGender.Value);
        int race = DBConvert.ParseInt(ucbRace.Value);
        int religion = DBConvert.ParseInt(ucbReligion.Value);
        int education = DBConvert.ParseInt(ucbEducation.Value);
        int materialStatus = DBConvert.ParseInt(ucbMaritalStatus.Value);
        string taxCode = utxtTaxCode.Text.Trim();
        string attCardNo = utxtAttCardNo.Text.Trim();
        int employeeType = DBConvert.ParseInt(ucbEmpType.Value);
        int employeeGroup = DBConvert.ParseInt(ucbEmpGroup.Value);

        //Contact Information
        string homePhone = utxtHomePhone.Text.Trim();
        string mobilePhone = utxtMobilePhone.Text.Trim();
        string personalEmail = utxtPersonalEmail.Text.Trim();
        string companyEmail = utxtCompanyEmail.Text.Trim();
        string perHouseNo = utxtPerHouseNo.Text.Trim();
        string perWard = utxtPerWard.Text.Trim();
        string perStreet = utxtPerStreet.Text.Trim();
        int perCountry = DBConvert.ParseInt(ucbPerCountry.Value);
        int perCity = DBConvert.ParseInt(ucbPerCity.Value);
        int perDistrict = DBConvert.ParseInt(ucbPerDistrict.Value);
        string curHouseNo = utxtCurHouseNo.Text.Trim();
        string curWard = utxtCurWard.Text.Trim();
        string curStreet = utxtCurStreet.Text.Trim();
        int curCoutry = DBConvert.ParseInt(ucbCurCountry.Value);
        int curCity = DBConvert.ParseInt(ucbCurCity.Value);
        int curDistrist = DBConvert.ParseInt(ucbCurDistrict.Value);
        //Bank Account
        string bankName = utxtBankName.Text.Trim();
        string bankBranch = utxtBankBranch.Text.Trim();
        string bankAccount = utxtBankAccountNo.Text.Trim();

        inputParam[0] = new DBParameter("@Eid", DbType.Int32, eid);
        inputParam[1] = new DBParameter("@EmpName", DbType.String, 50, employee);
        if (udtBirthday.Value != null)
        {
          inputParam[3] = new DBParameter("@Birthday", DbType.DateTime, DBConvert.ParseDateTime(udtBirthday.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
        }
        if (birthdayPlace != int.MinValue)
        {
          inputParam[4] = new DBParameter("@BirthPlace", DbType.Int32, birthdayPlace);
        }
        if (udtStartDate.Value != null)
        {
          inputParam[5] = new DBParameter("@StartDate", DbType.DateTime, DBConvert.ParseDateTime(udtStartDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
        }
        if (gender != int.MinValue)
        {
          inputParam[6] = new DBParameter("@Gender", DbType.Int32, gender);
        }
        if (race != int.MinValue)
        {
          inputParam[7] = new DBParameter("@Race", DbType.Int32, race);
        }
        if (religion != int.MinValue)
        {
          inputParam[8] = new DBParameter("@Religion", DbType.Int32, religion);
        }
        if (cardNo.Length > 0)
        {
          inputParam[9] = new DBParameter("@IDCardNo", DbType.AnsiString, 50, cardNo);
        }
        if (cardIssuedBy != int.MinValue)
        {
          inputParam[10] = new DBParameter("@IDCardIssuedBy", DbType.Int32, cardIssuedBy);
        }
        if (udtIDIssuedDate.Value != null)
        {
          inputParam[11] = new DBParameter("@IDCardIssuedDate", DbType.DateTime, DBConvert.ParseDateTime(udtIDIssuedDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
        }
        if (education != int.MinValue)
        {
          inputParam[12] = new DBParameter("@Education", DbType.Int32, education);
        }
        if (materialStatus != int.MinValue)
        {
          inputParam[13] = new DBParameter("@MaritalStatus", DbType.Int32, materialStatus);
        }
        if (taxCode.Length > 0)
        {
          inputParam[14] = new DBParameter("@TaxCode", DbType.String, taxCode);
        }
        if (taxCode.Length > 0)
        {
          inputParam[15] = new DBParameter("@AttendanceCardNo", DbType.AnsiString, attCardNo);
        }
        if (employeeType != int.MinValue)
        {
          inputParam[16] = new DBParameter("@EmpType", DbType.Int32, employeeType);
        }
        if (employeeGroup != int.MinValue)
        {
          inputParam[17] = new DBParameter("@EmpGroup", DbType.Int32, employeeGroup);
        }
        if (homePhone.Length > 0)
        {
          inputParam[18] = new DBParameter("@HomePhone", DbType.AnsiString, homePhone);
        }
        if (mobilePhone.Length > 0)
        {
          inputParam[19] = new DBParameter("@MobilePhone", DbType.AnsiString, mobilePhone);
        }
        if (personalEmail.Length > 0)
        {
          inputParam[20] = new DBParameter("@PersonalEmail", DbType.AnsiString, personalEmail);
        }
        if (companyEmail.Length > 0)
        {
          inputParam[21] = new DBParameter("@CompanyEmail", DbType.AnsiString, companyEmail);
        }
        if (perHouseNo.Length > 0)
        {
          inputParam[22] = new DBParameter("@PerHouseNo", DbType.AnsiString, perHouseNo);
        }
        if (perStreet.Length > 0)
        {
          inputParam[23] = new DBParameter("@PerStreet", DbType.String, perStreet);
        }
        if (perWard.Length > 0)
        {
          inputParam[24] = new DBParameter("@PerWard", DbType.String, perWard);
        }
        if (perCountry != int.MinValue)
        {
          inputParam[25] = new DBParameter("@PerCountry", DbType.Int32, perCountry);
        }
        if (perCity != int.MinValue)
        {
          inputParam[26] = new DBParameter("@PerCityPid", DbType.Int32, perCity);
        }
        if (perDistrict != int.MinValue)
        {
          inputParam[27] = new DBParameter("@PerDistrict", DbType.Int32, perDistrict);
        }
        if (curHouseNo.Length > 0)
        {
          inputParam[28] = new DBParameter("@CurAddressNo", DbType.String, curHouseNo);
        }
        if (curStreet.Length > 0)
        {
          inputParam[29] = new DBParameter("@CurStreet", DbType.String, curStreet);
        }
        if (curWard.Length > 0)
        {
          inputParam[30] = new DBParameter("@CurWard", DbType.String, curWard);
        }
        if (curCoutry != int.MinValue)
        {
          inputParam[31] = new DBParameter("@CurCountry", DbType.Int32, curCoutry);
        }
        if (curCity != int.MinValue)
        {
          inputParam[32] = new DBParameter("@CurCityPid", DbType.Int32, curCity);
        }
        if (curDistrist != int.MinValue)
        {
          inputParam[33] = new DBParameter("@CurDistrict", DbType.Int32, curDistrist);
        }
        if (bankName.Length > 0)
        {
          inputParam[34] = new DBParameter("@BankName", DbType.String, bankName);
        }
        if (bankBranch.Length > 0)
        {
          inputParam[35] = new DBParameter("@BankBranch", DbType.String, bankBranch);
        }
        if (bankAccount.Length > 0)
        {
          inputParam[36] = new DBParameter("@BankAccountNo", DbType.AnsiString, bankAccount);
        }
        inputParam[37] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        DataBaseAccess.ExecuteStoreProcedure("spHRDDBEmployee_Edit", inputParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          if (this.eid == int.MinValue)
          {
            this.eid = DBConvert.ParseInt(outputParam[0].Value);
            this.LoadEmployee();
            ucbEmployeeList.Value = this.eid;
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    private void ClearData()
    {
      //Basic Information
      utxtEmpID.Text = string.Empty;
      ucbEmployeeList.Value = null;
      utxtEmpName.Text = string.Empty;
      utxtWorkingStatus.Text = string.Empty;
      udtBirthday.Value = null;
      ucbBirthPlace.Value = null;
      utxtIDCardNo.Text = string.Empty;
      udtIDIssuedDate.Value = null;
      ucbIDIssuedBy.Value = null;
      ucbGender.Value = null;
      ucbRace.Value = null;
      ucbReligion.Value = null;
      ucbEducation.Value = null;
      ucbMaritalStatus.Value = null;
      utxtTaxCode.Text = string.Empty;
      utxtAttCardNo.Text = string.Empty;
      ucbEmpType.Value = null;
      ucbEmpGroup.Value = null;
      //Contact Informaton
      utxtHomePhone.Text = string.Empty;
      utxtMobilePhone.Text = string.Empty;
      utxtPersonalEmail.Text = string.Empty;
      utxtCompanyEmail.Text = string.Empty;
      utxtPerHouseNo.Text = string.Empty;
      utxtPerWard.Text = string.Empty;
      utxtPerStreet.Text = string.Empty;
      ucbPerCountry.Value = null;
      ucbPerCity.Value = null;
      ucbPerDistrict.Value = null;
      utxtCurHouseNo.Text = string.Empty;
      utxtCurWard.Text = string.Empty;
      utxtCurStreet.Text = string.Empty;
      ucbCurCountry.Value = null;
      ucbCurCity.Value = null;
      ucbCurDistrict.Value = null;
      //Bank Account
      utxtBankName.Text = string.Empty;
      utxtBankBranch.Text = string.Empty;
      utxtBankAccountNo.Text = string.Empty;
      this.NeedToSave = false;

    }
    #endregion function

    #region event
    public viewHRD_02_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewHRD_02_001_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(tableLayoutSearch);

      //Init Data
      this.InitData();
      this.ClearData();
    }
    /// <summary>
    /// Search Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void ucbPerCountry_ValueChanged(object sender, EventArgs e)
    {
      this.LoadUltraCBCity();
    }

    private void ucbPerCity_ValueChanged(object sender, EventArgs e)
    {
      this.LoadUltraDistrict();
    }

    private void ucbCurCountry_ValueChanged(object sender, EventArgs e)
    {
      this.LoadUltraCBCurrentCity();
    }

    private void ucbCurCity_ValueChanged(object sender, EventArgs e)
    {
      this.LoadUltraCurrentDistrict();
    }
    private void btnNew_Click(object sender, EventArgs e)
    {
      this.ClearData();
      btnSave.Enabled = true;
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    #endregion event

  }
}
