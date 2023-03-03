using System;
using System.Globalization;

namespace DaiCo.ERPProject
{
  public class ConstantClass
  {
    public const string FORMAT_DATETIME = "dd/MM/yyyy";
    public const string FORMAT_HOUR = "HH:mm";
    public const string MASKINPUT_DATETIME = "{LOC}dd/mm/yyyy hh:mm";
    public const string MASKINPUT_TIME = "hh:mm";
    public const string FORMAT_PROVIDER = "vi-VN";
    public const string FORMAT_DATETIME_WITHHOUR = "dd/MM/yyyy hh:mm";

    public const string FORMAT_DATETIME_NORMAL = "dd/MM/yyyy hh:mm:ss.fff";
    public const string NEW_FORMAT_DATETIME = "dd-MMM-yy";
    public const string PATH_LOGO = "\\logo.jpg";
    public const int UserAddmin = 485;
    // Bo vao CodeMaster Group = 1009 
    public const string ROUND_UNIT = "|sheet|pia|pc|pcs|pair|set|roll|";
    public static string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public static string PATHCOOKIE = Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
    public static CultureInfo CULTURE = CultureInfo.CreateSpecificCulture("vi-VN");
    public const string LANGUAGE_RESOURCE = "DaiCo.ERPProject.Share.Language.LangResource";

    #region ACC
    public const int Payment_Advance = 236;
    public const int Payment_Direct = 240;
    public const int Payment_Voucher = 21;
    public const int Payment_Refund = 241;
    public const int Payment_BankDebit = 242;
    public const int Payment_BankCredit = 243;
    public const int Receipt_Voucher = 18;
    public const int Loan_Receipt = 114;
    public const int Asset_Receipt = 41;
    public const int Asset_Shipment = 42;
    public const int Asset_Transfer = 43;
    public const int Invoice_Purchaser = 244;
    public const int Invoice_Seller = 248;
    #endregion ACC

    #region Barcode Carcass Component Status
    public const int BARCODE_CARCASS_COMP_NEW = 0;
    public const int BARCODE_CARCASS_COMP_RENEW = 1;
    public const int BARCODE_CARCASS_COMP_REPAIR = 2;
    #endregion Barcode Carcass Component Status

    #region Group code master

    public const int GROUP_CATEGORY = 1;
    public const int GROUP_COLLECTION = 2;
    public const int GROUP_KNOCKDOWN = 3;
    public const int GROUP_CHANGEKIND = 4;
    public const int GROUP_MOREDIMENSION = 5;
    public const int GROUP_COMPONENTSPECIFY = 6;
    public const int GROUP_COMPONENTSTATUS = 7;
    public const int GROUP_MATERIALSTYPE = 8;
    public const int GROUP_COMPONENT_ADJECTIVE = 9;
    public const int GROUP_ADJECTIVE = 10;
    public const int GROUP_GLASS = 11;
    public const int GROUP_GLASSTYPE = 12; //Mirror or Glass
    public const int GROUP_BEVELTYPE = 13;
    public const int GROUP_RDD_GROUP = 14;
    public const int GROUP_EXHIBITION = 16;
    public const int GROUP_ITEMKIND = 17;
    public const int GROUP_ITEM_PATH_FOLDER = 21;
    public const int GROUP_SALEORDERTYPE = 1001;
    public const int GROUP_CONTAINERSTYPE = 1002;
    public const int GROUP_ENQUIRY_LIMIT = 1003;
    public const int GROUP_AREA = 1004;
    public const int GROUP_WO_CHECKPOINT_DEALINE = 1005;
    public const int GROUP_ITEM = 1006;
    public const int GROUP_BILL_OF_LADING = 2001;
    public const int GROUP_CERTIFICATE = 2002;
    public const int GROUP_PACKING = 2003;
    public const int GROUP_INVOICE = 2004;
    public const int GROUP_PAYMENT_TERM = 2005;
    public const int GROUP_PRICE_BASE = 2006;
    public const int GROUP_PRICE_OPTION = 2016;
    public const int GROUP_CURRENCY_SIGN = 2007;
    public const int GROUP_CUSTOMER_KIND = 2008;
    public const int GROUP_PRICELIST_TIME = 2011;
    public const int GROUP_CATALOGUE = 2013;
    public const int GROUP_ENQUIRY_MAX_EXPIRE_DAYS = 2014;
    public const int GROUP_WIPADJUSTMENTTOWPOINT = 3001;
    public const int GROUP_WIPADJUSTMENTREASONIN = 3002;
    public const int GROUP_WIPADJUSTMENTREASONOUT = 3003;
    public const int GROUP_FINISHED_ITEM = 4001;
    public const int GROUP_EXICUSTOME_TYPE = 5001;
    public const int GROUP_DELIVERY_TERM = 5002;
    public const int GROUP_EXI_CURRENCY = 5003;
    public const int GROUP_ROLE = 10001;

    // Add Group General  15/10/2011 START
    public const int GROUP_GNR_PROGRAMMODULE = 9001;
    public const int GROUP_GNR_URGENTLEVEL = 9002;
    public const int GROUP_GNR_TYPE = 9003;
    public const int GROUP_GNR_ITTYPE = 9004;
    public const int GROUP_GNR_PATHFILEUPLOAD = 9005;
    public const int GROUP_GNR_TYPEFILEUPLOAD = 9006;
    public const int GROUP_GNR_TYPEFILEITUPLOAD = 9007;
    // Add Group General  15/10/2011 END

    public const int GROUP_FOUNDY_COMPONENT_KIND = 13001;
    public const int GROUP_FOUNDY_ISCASTING = 13002;
    public const int GROUP_FOUNDY_MISC_REASON = 13003;
    public const int GROUP_COMP_NO_COUNT = 13006;
    public const int GROUP_FOUNDY_WORK_AREA = 13007;
    public const int GROUP_FOUNDRY_COMPONENT_STATUS = 13008;

    #endregion Group code master

    #region Group Purchase Trade Type
    public const int GROUP_PUR_TRADETYPE = 7002;
    public const int GROUP_PUR_DEBIT = 7001;

    #endregion Group Purchase Trade Type

    #region Group code component item

    public const int COMP_HARDWARE = 1;
    public const int COMP_GLASS = 2;
    public const int COMP_SUPPORT = 3;
    public const int COMP_ACCESSORY = 4;
    public const int COMP_UPHOLSTERY = 5;
    public const int COMP_FINISHING = 6;
    public const int COMP_PACKING = 7;
    public const int COMP_DIRECT_LABOUR = 8;
    public const int COMP_CARCASS = 9;

    #endregion Group code component item

    #region Devision

    public const string Devision_Component = "COM";
    public const string Devision_Carcass = "CAR";

    #endregion Devision

    #region WorkArea

    public const int WorkArea_Other = 65;
    public const int WorkArea_PrimaryCut = 1;
    public const int WorkArea_Veneering = 4;
    public const int WorkArea_ComponentStore = 31;
    public const int WorkArea_Assembly = 32;
    public const int WorkArea_ITW = 33;
    public const int WorkArea_Sanding = 34;
    public const int WorkArea_Finishing = 35;
    public const int WorkArea_Finishing2 = 41;
    public const int WorkArea_FinalFinishing = 36;
    public const int WorkArea_FinalQC = 38;
    public const int WorkArea_Pack = 37;
    public const int SubCon = 39;
    public const int WorkArea_QCToCST = 30;
    public const int WorkArea_COM2 = 65;
    #endregion WorkArea

    #region Department

    public const string PLANNING_DEPT = "PLA";
    public const string WAREHOUSE_DEPT = "WHD";
    public const string TECHICAL_DEPT = "TEC";
    public const string RD1_DEPT = "RD1";
    public const string RD2_DEPT = "RD2";

    #endregion Department

    #region Warehouse

    public const int MATERIALS_WAREHOUSE = 1;
    public const int VENEER_WAREHOUSE = 2;
    public const int WOOD_WAREHOUSE = 3;
    public const int SERVICE_WAREHOUSE = 4;

    public const int TRANSFER_TO_WIP_WAREHOUSE = 0;
    public const int ISSUE_TO_PRODUCTION = 1;
    public const int RETURN_TO_SUPPLIER = 2;
    public const int ADJUSTMENT_OUT = 3;
    public const int ISSUE_TO_SUBCON = 4;

    public const int RECEIPT_FROM_SUPPLIER = 1;
    public const int RETURN_FROM_PRODUCTION = 2;
    public const int ADJUSTMENT_IN = 3;

    #endregion Warehouse

    #region prefix
    public const int PREFIX_ITEM = 1;
    public const int PREFIX_CARCASS = 2;
    #endregion prefix

    #region Foundry
    #region Foundry WH
    public const string FOU_WIP_PREFIX_ISSUE_TO_PRODUCTION = "08ISS";
    public const string FOU_WIP_PREFIX_TRANSFER_TO_FIN_WH = "08TFW";
    public const string FOU_WIP_PREFIX_ADJUSTMENT_OUT = "08ADO";
    public const string FOU_WIP_PREFIX_MISC_ISSUE = "08MIS";
    public const string FOU_WIP_PREFIX_TRANS_LOCATION = "08TR";

    public const int FOU_WIP_TYPE_ISSUE_TO_PRODUCTION = 1;
    public const int FOU_WIP_TYPE_TRANSFER_TO_FIN_WH = 2;
    public const int FOU_WIP_TYPE_ADJUSTMENT_OUT = 3;
    public const int FOU_WIP_TYPE_MISC_ISSUE = 4;

    public const string FOU_WIP_PREFIX_RECEIVING_NOTE = "08REC";
    public const string FOU_WIP_PREFIX_RETURN_TO_WIP_WH = "08RTW";
    public const string FOU_WIP_PREFIX_ADJUSTMENT_IN = "08ADI";
    public const string FOU_WIP_PREFIX_MISC_RECEIVE = "08MRC";

    public const int FOU_WIP_TYPE_RECEIVING_NOTE = 1;
    public const int FOU_WIP_TYPE_RETURN_TO_WIP_WH = 2;
    public const int FOU_WIP_TYPE_ADJUSTMENT_IN = 3;
    public const int FOU_WIP_TYPE_MISC_RECEIVE = 4;

    public const string FOU_FIN_PREFIX_RECEIVING_NOTE = "07REC";
    public const string FOU_FIN_PREFIX_ADJUSTMENT_IN = "07ADI";
    public const string FOU_FIN_PREFIX_TRANS_LOCATION = "07TR";

    public const int FOU_FIN_TYPE_RECEIVING_NOTE = 1;
    public const int FOU_FIN_TYPE_ADJUSTMENT_IN = 2;

    public const string FOU_FIN_PREFIX_ISSUING_NOTE = "07ISS";
    public const string FOU_FIN_PREFIX_ADJUSTMENT_OUT = "07ADO";

    public const int FOU_FIN_TYPE_ISSUING_NOTE = 1;
    public const int FOU_FIN_TYPE_SPECIAL_ISSUE = 2;
    public const int FOU_FIN_TYPE_ADJUSTMENT_OUT = 3;
    public const int FOU_FIN_TYPE_MISC_ISSUE = 4;
    #endregion Foundry WH

    #region Foundry Component
    public const int FOU_COMP_GROUP = 1;
    public const int FOU_COMP_SAMPLE_PROCESS_GROUP = 2;
    public const int FOU_COMP_PRODUCTION_GROUP = 3;
    #endregion Foundry Component

    #region Foundry Planning
    public const string FOU_PLN_PREFIX_SUPPLEMENT = "FSUP";
    #endregion Foundry Planning



    #endregion Foundry
    #region Planning

    #region Planning CarcassWO
    public const string PLN_PREFIX_CARCASSWO = "CWO";
    #endregion Planning CarcassWO

    #region Planning Transaction

    public const string PLN_PREFIX_CARCASSWODETAIL = "CARWO";
    #endregion Planning Transaction

    #region Planning AdJust
    public const string PLN_PREFIX_ADJUST = "ADJ";
    #endregion Planning AdJust

    #endregion Planning

    #region Accounting AR
    public const string ACC_AR_PREFIX_PRO_FORMA_INVOICE = "PI";
    public const string ACC_AR_PREFIX_COMMERCIAL_INVOICE = "CI";
    public const string ACC_AR_PREFIX_RECEIVING_NOTE = "RE";
    public const string ACC_AR_PREFIX_CREDIT_NOTE = "CR";
    public const string ACC_AR_PREFIX_DEBIT_NOTE = "DB";
    public const string ACC_AR_PREFIX_OVERPAY_NOTE = "OP";
    #endregion Accounting AR

    #region Reason Unlock Kind
    public const int WIP_CARCASS_PPUNLOCK = 1;
    public const int FOUNDRY_COMPONENT_PPUNLOCK = 2;
    #endregion Reason Unlock Kind
  }
}
