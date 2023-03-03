using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Technical.DataSetSource;
using DaiCo.Technical.Reports;
using System;
using System.Data;
using System.Drawing;
using System.IO;

namespace DaiCo.ERPProject
{
  public partial class View_BOM_0018_ItemMaterialReport : MainUserControl
  {
    #region Field

    public int iIndex = 0;
    public string itemCode = string.Empty;
    public int revision = int.MinValue;
    public int ncategory = int.MinValue;
    public DataTable dtListBox = new DataTable();
    public DataTable dtRevisionRecord = new DataTable();
    public DataTable dtSaleRelation = new DataTable();
    public int isDraft = int.MinValue;
    public string code = string.Empty;
    public string revisionRecord = string.Empty;
    public string saleRelation = string.Empty;
    public DataTable dtSource = new DataTable();
    public string packageCode = string.Empty;

    #endregion Field

    #region Init

    public View_BOM_0018_ItemMaterialReport()
    {
      InitializeComponent();
    }

    private void View_BOM_0018_ItemMaterialReport_Load(object sender, EventArgs e)
    {
      if (ncategory == 0)
      {
        this.ReportItemMaster();
      }
      if (ncategory == 1)
      {
        this.LoadDataWoodCarcass();
      }
      if (ncategory == 2)
      {
        this.ReportItemCompHardware();
      }
      if (ncategory == 3)
      {
        this.ReportItemComponentGlass();
      }
      if (ncategory == 4)
      {
        this.LoadDataSupportInfo();
      }
      if (ncategory == 5)
      {
        this.ReportItemComponentAccessory();
      }
      if (ncategory == 6)
      {
        this.ReportItemCompUpholstery();
      }
      if (ncategory == 7)
      {
        this.ReportComponentFinishing();
      }
      if (ncategory == 9)
      {
        this.ReportLabourInfo();
      }
      if (ncategory == 10)
      {
        this.LoadDataListBox();
      }
      if (ncategory == 11)
      {
        this.LoadDataCheckListBox();
      }
      if (ncategory == 12)
      {
        this.ReportFinishingInfo();
      }
      if (ncategory == 13)
      {
        this.ReportSupportMaterialInfo();
      }
      if (ncategory == 14)
      {
        this.ReportUpholteryMaterialInfo();
      }
      if (ncategory == 15)
      {
        this.ReportItemComponentMaster();
      }
      if (ncategory == 16)
      {
        this.ReportListGlassInfo();
      }
      if (ncategory == 17)
      {
        this.ReportHardwareMasterList();
      }
      if (ncategory == 18)
      {
        this.ReportPackingMaterial();
      }
      if (ncategory == 19)
      {
        this.ReportComponentMasterList();
      }
    }

    #endregion Init

    #region LoadReport

    private void ReportListGlassInfo()
    {
      dsGlassMasterList dsGlassMasterList = new dsGlassMasterList();
      int no = 1;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, row["Pid"]) };
          DataTable dtGlassInfo = DataBaseAccess.SearchStoreProcedureDataTable("spBOMGlassInfo", inputParam);
          if (dtGlassInfo != null && dtGlassInfo.Rows.Count > 0)
          {
            string glassCode = dtGlassInfo.Rows[0]["GlassCode"].ToString();
            DataRow newRow = dsGlassMasterList.Tables["TblGlassMasterList"].NewRow();
            newRow["No"] = no;
            newRow["GlassCode"] = glassCode;
            newRow["MaterialCode"] = dtGlassInfo.Rows[0]["MaterialCode"];
            newRow["Description"] = dtGlassInfo.Rows[0]["Description"];
            newRow["Unit"] = dtGlassInfo.Rows[0]["Unit"];
            newRow["Dimension"] = dtGlassInfo.Rows[0]["Dimension"];
            newRow["Image"] = this.ImagePathToByteArray(FunctionUtility.BOMGetItemComponentImage(glassCode));

            //Item referrence
            string commandTextItem = string.Format("Select ItemCode, Revision From TblBOMItemComponent Where ComponentCode = '{0}'", glassCode);
            DataTable dtItemReference = DataBaseAccess.SearchCommandTextDataTable(commandTextItem);
            if (dtItemReference != null)
            {
              string itemReference = string.Empty;
              foreach (DataRow itemRow in dtItemReference.Rows)
              {
                if (itemReference.Length > 0)
                {
                  itemReference += ", ";
                }
                itemReference += string.Format("{0}({1})", itemRow["ItemCode"], itemRow["Revision"]);
              }
              newRow["ItemReference"] = itemReference;
            }
            newRow["UserName"] = SharedObject.UserInfo.UserName;
            string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
            newRow["LogoImage"] = this.ImagePathToByteArray(logoPath);
            no++;
            dsGlassMasterList.Tables["TblGlassMasterList"].Rows.Add(newRow);
          }
        }
      }
      cptBOMListGlassInfo cpt = new cptBOMListGlassInfo();
      cpt.SetDataSource(dsGlassMasterList);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataHardwareInfor()
    {
      string commandText = string.Format("Select (INFO.ItemCode + '|' + BS.Name) as [ItemCode], INFO.Confirm, INFO.CarcassCode, BS.Name as Description, INFO.Revision From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
      DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);
      DataTable dtCompDetail = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfoDetail", inputParam);

      DataSet dsSource = CreateDataSet.HardwareInfoReport();
      //dsSource.Tables.Add(dtInfo.Clone());
      dsSource.Tables["TblItemInfo"].Merge(dtInfo);
      //dsSource.Tables.Add(dtComp.Clone());
      dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);
      //dsSource.Tables.Add(dtCompDetail.Clone());
      dsSource.Tables["TblBOMComponentInfoDetail"].Merge(dtCompDetail);

      Technical.Reports.cptHardwareInfoReport cpt = new DaiCo.Technical.Reports.cptHardwareInfoReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptHardwareInfoReport.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    /// <summary>
    ///  REPORT COMPONENT MASTER LIST LEVEL 2nd (Danh sach tat ca component cua item)
    /// 
    /// </summary>
    /// 

    private void ReportItemComponentMaster()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListItemComponentMaster", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[1].Columns.Add("Image", typeof(System.Byte[]));
      try
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        string imgPath = FunctionUtility.BOMGetItemImage(this.itemCode, this.revision);
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        row["Image"] = imgbyte;
        br.Close();
        fs.Close();
      }
      catch
      {
      }
      DataSet dsItemMaster = CreateDataSet.ItemMasterInfo();
      dsSource.Tables[0].Columns.Add("Title", typeof(string));
      dsSource.Tables[0].Rows[0]["Title"] = "Component Master List 2nd Level";
      dsItemMaster.Tables["DataTable1"].Merge(dsSource.Tables[0]);
      DataSet dsHardware = CreateDataSet.ItemComPonentHadware();
      DataRow rowMaster = dsHardware.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch
      {
      }
      dsHardware.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      foreach (DataRow row in dsSource.Tables[1].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(row["ComponentCode"].ToString());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          row["Image"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch
        {
        }
      }
      dsHardware.Tables["TblHardwareList"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMListItemComponent cpt = new DaiCo.Technical.Reports.cptBOMListItemComponent();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMListItemComponent.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsItemMaster);
      cpt.SetDataSource(dsHardware);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void ReportItemCompHardware()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListItemCompHardware", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[1].Columns.Add("Image", typeof(System.Byte[]));

      try
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        string imgPath = FunctionUtility.BOMGetItemImage(this.itemCode, this.revision);
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        row["Image"] = imgbyte;
        br.Close();
        fs.Close();
      }
      catch { }

      DataSet dsItemMaster = CreateDataSet.ItemMasterInfo();
      dsSource.Tables[0].Columns.Add("Title", typeof(string));
      dsSource.Tables[0].Rows[0]["Title"] = "Hardware Master List 2nd Level";
      //dsItemMaster.Tables.Add(dsSource.Tables[0].Clone());
      dsItemMaster.Tables["DataTable1"].Merge(dsSource.Tables[0]);

      DataSet dsHardware = CreateDataSet.ItemComPonentHadware();
      DataRow rowMaster = dsHardware.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch { }
      dsHardware.Tables["TblMasterInfo"].Rows.Add(rowMaster);

      foreach (DataRow row in dsSource.Tables[1].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(row["ComponentCode"].ToString());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          row["Image"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch
        { }
      }

      //dsHardware.Tables.Add(dsSource.Tables[1].Clone());
      dsHardware.Tables["TblHardwareList"].Merge(dsSource.Tables[1]);

      Technical.Reports.cptBOMItemComponentHardware cpt = new DaiCo.Technical.Reports.cptBOMItemComponentHardware();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMItemComponentHardware.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsItemMaster);
      cpt.SetDataSource(dsHardware);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataUpholsteryInfo()
    {
      string commandText = string.Format("Select (INFO.ItemCode + '|' + BS.Name) as [ItemCode], INFO.Confirm, INFO.CarcassCode, BS.Name as Description, INFO.Revision From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
      DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);

      DataSet dsSource = CreateDataSet.HardwareInfoReport();
      //dsSource.Tables.Add(dtInfo.Clone());
      dsSource.Tables["TblItemInfo"].Merge(dtInfo);
      //dsSource.Tables.Add(dtComp.Clone());
      dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);

      Technical.Reports.cptUpholsteryInforReport cpt = new DaiCo.Technical.Reports.cptUpholsteryInforReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptUpholsteryInforReport.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void ReportItemCompUpholstery()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListItemCompUpholstery", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[0].Columns.Add("Title", typeof(string));
      dsSource.Tables[0].Rows[0]["Title"] = "Upholstery Master List 2nd Level";

      DataSet dsUpholstery = CreateDataSet.ItemComponentUpholstery();
      DataRow rowMaster = dsUpholstery.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch { }
      dsUpholstery.Tables["TblMasterInfo"].Rows.Add(rowMaster);

      try
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        string imgPath = FunctionUtility.BOMGetItemImage(this.itemCode, this.revision);
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        row["Image"] = imgbyte;
        br.Close();
        fs.Close();
      }
      catch { }

      DataSet dsItemMaster = CreateDataSet.ItemMasterInfo();
      dsItemMaster.Tables[0].Merge(dsSource.Tables[0]);
      dsUpholstery.Tables["TblUpholsteryList"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMItemComponentUpholstery cpt = new DaiCo.Technical.Reports.cptBOMItemComponentUpholstery();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMItemComponentUpholstery.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsItemMaster);
      cpt.SetDataSource(dsUpholstery);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private string GetItemReferenceUpholtery(string upholtery)
    {
      string commandText = string.Format("SELECT DISTINCT TOP 4 ItemCode, MAX(Revision) Revision FROM TblBOMItemComponent WHERE CompGroup = 5 AND ComponentCode = '{0}' GROUP BY ItemCode", upholtery);
      DataTable dtTemp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      string itemReference = string.Empty;
      if (dtTemp.Rows.Count > 0)
      {
        foreach (DataRow rowTemp in dtTemp.Rows)
        {
          itemReference += ", " + rowTemp["ItemCode"].ToString().Trim() + string.Format("({0})", rowTemp["Revision"].ToString().Trim());
        }
      }
      itemReference = itemReference.TrimStart(',').Trim();
      return itemReference;
    }

    private void ReportUpholteryMaterialInfo()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@UpholsteryCode", DbType.AnsiString, 16, code);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListUpholsteryMaterial", inputParam);

      DataSet dsUlpholteryInfo = CreateDataSet.UpholsteryInfo();
      DataRow rowMaster = dsUlpholteryInfo.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch { }
      dsUlpholteryInfo.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsUlpholteryInfo.Tables["TblUpholsteryInfo"].Merge(dsSource.Tables[0]);
      foreach (DataRow row in dsUlpholteryInfo.Tables["TblUpholsteryInfo"].Rows)
      {
        string upholsteryCode = row["UpholsteryCode"].ToString().Trim();
        string itemReference = this.GetItemReferenceUpholtery(upholsteryCode);
        row["ItemReference"] = itemReference;
      }

      dsUlpholteryInfo.Tables["TblUpholsteryDetail"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMUpholsteryInfo cpt = new DaiCo.Technical.Reports.cptBOMUpholsteryInfo();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMUpholsteryInfo.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsUlpholteryInfo);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataAccessoryInfo()
    {
      string commandText = string.Format("Select (INFO.ItemCode + '|' + BS.Name) as [ItemCode], INFO.Confirm, INFO.CarcassCode, BS.Name as Description, INFO.Revision From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
      DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);

      DataSet dsSource = CreateDataSet.HardwareInfoReport();
      //dsSource.Tables.Add(dtInfo.Clone());
      dsSource.Tables["TblItemInfo"].Merge(dtInfo);
      //dsSource.Tables.Add(dtComp.Clone());
      dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);

      Technical.Reports.cptAccessoryInforReport cpt = new DaiCo.Technical.Reports.cptAccessoryInforReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptAccessoryInforReport.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataFinishingInfo()
    {
      string commandText = string.Format("Select (INFO.ItemCode + '|' + BS.Name) as [ItemCode], INFO.Confirm, INFO.CarcassCode, BS.Name as Description, INFO.Revision From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
      DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);

      DataSet dsSource = CreateDataSet.HardwareInfoReport();
      //dsSource.Tables.Add(dtInfo.Clone());
      dsSource.Tables["TblItemInfo"].Merge(dtInfo);
      //dsSource.Tables.Add(dtComp.Clone());
      dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);

      Technical.Reports.cptFinishingInfoReport cpt = new DaiCo.Technical.Reports.cptFinishingInfoReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptFinishingInfoReport.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSource);
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataSupportInfo()
    {
      string commandText = string.Format("Select (INFO.ItemCode + '|' + BS.Name) as [ItemCode], INFO.Confirm, INFO.CarcassCode, BS.Name as Description, INFO.Revision, INFO.SupCode, SUBINFO.Description as SupDescription From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1} Left join TblBOMSupportInfo SUBINFO on INFO.SupCode = SUBINFO.SupCode", itemCode, revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      string supCode = string.Empty;
      try
      {
        supCode = dtInfo.Rows[0]["SupCode"].ToString().Trim();
      }
      catch { }

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SupCode", DbType.AnsiString, 16, supCode) };
      DataTable dtSup = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListSupportDetailBySupCode", inputParam);

      DataSet dsSource = CreateDataSet.SupportInfoReport();
      //dsSource.Tables.Add(dtInfo.Clone());
      dsSource.Tables["TblItemInfo"].Merge(dtInfo);
      //dsSource.Tables.Add(dtSup.Clone());
      dsSource.Tables["TblBOMSupportDetail"].Merge(dtSup);

      Technical.Reports.cptSupportInfoReport cpt = new DaiCo.Technical.Reports.cptSupportInfoReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptSupportInfoReport.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private string GetItemReference(string supCode)
    {
      string commandText = string.Empty;
      if (itemCode.Length > 0)
      {
        commandText = string.Format("SELECT DISTINCT TOP 3 ItemCode, MAX(Revision) Revision FROM TblBOMItemInfo WHERE SupCode = '{0}' AND ItemCode <> '{1}' AND Revision <> {2} GROUP BY ItemCode", supCode, itemCode, this.revision);
      }
      else
      {
        commandText = string.Format("SELECT DISTINCT TOP 4 ItemCode, MAX(Revision) Revision FROM TblBOMItemInfo WHERE SupCode = '{0}' GROUP BY ItemCode", supCode);
      }
      DataTable dtTemp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      string itemReference = string.Empty;
      if (dtTemp.Rows.Count > 0)
      {
        foreach (DataRow rowTemp in dtTemp.Rows)
        {
          itemReference += ", " + rowTemp["ItemCode"].ToString().Trim() + string.Format("({0})", rowTemp["Revision"].ToString().Trim());
        }
      }
      if (itemCode.Length > 0)
      {
        itemReference = itemCode + string.Format("({0})", this.revision) + itemReference;
      }
      itemReference = itemReference.TrimStart(',').Trim();
      return itemReference;
    }

    private void ReportSupportMaterialInfo()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@SupCode", DbType.AnsiString, 16, code);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListSupportMaterial", inputParam);

      DataSet dsSupportInfo = CreateDataSet.SupportMaterialInfo();
      DataRow rowMaster = dsSupportInfo.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch { }
      dsSupportInfo.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsSupportInfo.Tables["TblSupportInfo"].Merge(dsSource.Tables[0]);
      foreach (DataRow row in dsSupportInfo.Tables["TblSupportInfo"].Rows)
      {
        string supCode = row["SupCode"].ToString().Trim();
        string itemReference = this.GetItemReference(supCode);
        row["ItemReference"] = itemReference;
      }

      //if (dsSource.Tables[1].Rows.Count == 0)
      //{
      //  DataRow row = dsSource.Tables[1].NewRow();
      //  dsSource.Tables[1].Rows.Add(row);
      //}
      dsSupportInfo.Tables["TblSupportDetail"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMSupportMaterialInfo cpt = new DaiCo.Technical.Reports.cptBOMSupportMaterialInfo();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMSupportMaterialInfo.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSupportInfo);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataWoodCarcass()
    {
      dsCarcassMaterialCutting dsSource = new dsCarcassMaterialCutting();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@itemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@revision", DbType.Int32, this.revision);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spBOMCarcassCuttingListReport", inputParam);
      dsSource.Tables["dtMaster"].Merge(ds.Tables[0]);
      dsSource.Tables["dtDetail"].Merge(ds.Tables[1]);
      dsSource.Tables["dtMaterialSummary"].Merge(ds.Tables[2]);
      string carcassCode = dsSource.Tables["dtMaster"].Rows[0]["CarcassCode"].ToString();
      string carcassPath = FunctionUtility.BOMGetCarcassImage(carcassCode);
      Image image = FunctionUtility.GetThumbnailImage(carcassPath, 5, 2.5);
      byte[] byData = FunctionUtility.ImageToByteArray(image);
      dsSource.Tables["dtMaster"].Rows[0]["CarcassImage"] = byData;
      Technical.Reports.cptBOMCarcassInfo cpt = new Technical.Reports.cptBOMCarcassInfo();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMCarcassInfo.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsSource.Tables["dtMaterial"]);
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataLabourInfo()
    {
      string commandText = string.Format("Select (INFO.ItemCode + '|' + BS.Name) as [ItemCode], INFO.Confirm, INFO.CarcassCode, BS.Name as Description, INFO.Revision From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataTable dtDirectLabour = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabDirectLabourInfo", inputParam);

      DataSet dsSource = CreateDataSet.LabourInfoReport();
      //dsSource.Tables.Add(dtInfo.Clone());
      dsSource.Tables["TblItemInfo"].Merge(dtInfo);
      //dsSource.Tables.Add(dtDirectLabour.Clone());
      dsSource.Tables["TblLabourInfo"].Merge(dtDirectLabour);

      Technical.Reports.cptLabourInfoReport cpt = new DaiCo.Technical.Reports.cptLabourInfoReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptLabourInfoReport.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void ReportLabourInfo()
    {
      // Load Data Sub Item Info
      string commandTextItemInfo = string.Format(@" SELECT BS.SaleCode, BS.ItemCode, @Revision Revision, BS.Name [ItemName], BS.[Description], CM1.Category, 
		                                                  CM2.Value [Collection], (FI.FinCode + ' / ' + FI.Name) [MainFinishing], INFO.KD
	                                                  FROM TblBOMItemInfo INFO
		                                                  INNER JOIN TblBOMItemBasic BS ON (INFO.ItemCode = BS.ItemCode)
		                                                  LEFT JOIN TblBOMFinishingInfo FI ON (INFO.MainFinish = FI.FinCode)
		                                                  LEFT JOIN TblCSDCategory CM1 ON (BS.Category = CM1.Pid)
		                                                  LEFT JOIN TblBOMCodeMaster CM2 ON (BS.[Collection] = CM2.Code AND CM2.[Group] = 2)		
	                                                  WHERE INFO.ItemCode = '{0}' AND INFO.Revision = {1}", this.itemCode, this.revision);
      DataTable dtItemInfo = DataBaseAccess.SearchCommandTextDataTable(commandTextItemInfo);
      dtItemInfo.Columns.Add("Title", typeof(string));
      dtItemInfo.Rows[0]["Title"] = "Direct Labor 2nd Level";
      DataSet dsItemMaster = CreateDataSet.ItemMasterInfo();
      dsItemMaster.Tables["DataTable1"].Merge(dtItemInfo);

      try
      {
        DataRow row = dsItemMaster.Tables[0].Rows[0];
        string imgPath = FunctionUtility.BOMGetItemImage(this.itemCode, this.revision);
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        row["Image"] = imgbyte;
        br.Close();
        fs.Close();
      }
      catch { }

      //Load Detail
      string commandTextDetail = string.Format(@"SELECT SectionCode, VS.NameEN SectionName, Qty [ManHour], Remark Remarks
	                                              FROM TblBOMDirectLabour Labour LEFT JOIN VBOMSection VS ON (Labour.SectionCode = VS.Code)
	                                              Where ItemCode = '{0}' AND Revision = {1}", this.itemCode, this.revision);
      DataTable dtLabourDetail = DataBaseAccess.SearchCommandTextDataTable(commandTextDetail);
      DataSet dsLabourInfo = CreateDataSet.DitectLaborInfo();
      DataRow rowMaster = dsLabourInfo.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      dsLabourInfo.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsLabourInfo.Tables["TblDirectLaborInfo"].Merge(dtLabourDetail);

      Technical.Reports.cptBOMDirectLaborInfo cpt = new DaiCo.Technical.Reports.cptBOMDirectLaborInfo();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMDirectLaborInfo.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsItemMaster);

      cpt.SetDataSource(dsLabourInfo);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataGlassInfo()
    {
      string commandText = string.Format("Select (INFO.ItemCode + '|' + BS.Name) as [ItemCode], INFO.Confirm, INFO.CarcassCode, BS.Name as Description, INFO.Revision From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataTable dtGlassInfo = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListItemGlassInfo", inputParam);

      string commandTextAdj = "Select Code, Value From TblBOMCodeMaster Where [Group] = 11";
      DataTable dtAdj = DataBaseAccess.SearchCommandTextDataTable(commandTextAdj);

      //Them va gan gia tri 0 cho cac cot Adjective
      foreach (DataRow row in dtAdj.Rows)
      {
        dtGlassInfo.Columns.Add(row["Value"].ToString(), typeof(int));
        foreach (DataRow rowGlass in dtGlassInfo.Rows)
        {
          rowGlass[row["Value"].ToString()] = 0;
        }
      }
      //Load gia tri vao cac cot Adjective
      for (int i = 0; i < dtGlassInfo.Rows.Count; i++)
      {
        string[] arrAdj = dtGlassInfo.Rows[i]["Adjective"].ToString().Split('|');
        for (int k = 0; k < arrAdj.Length; k++)
        {
          if (arrAdj[k].Length > 0)
          {
            try
            {
              dtGlassInfo.Rows[i][arrAdj[k]] = 1;
            }
            catch { }
          }
        }
      }

      DataSet dsSource = CreateDataSet.GlassInfoReport();
      dsSource.Tables["TblItemInfo"].Merge(dtInfo);
      dsSource.Tables["TblBOMGlassInfo"].Merge(dtGlassInfo);

      Technical.Reports.cptGlassInfoReport cpt = new DaiCo.Technical.Reports.cptGlassInfoReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptGlassInfoReport.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsSource);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void LoadDataListBox()
    {
      Technical.Reports.cptCreateListBoxReport rpt = new DaiCo.Technical.Reports.cptCreateListBoxReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptCreateListBoxReport.rpt";
      rpt.FileName = filePath;
      rpt.Load();
      DataSet dsCreateListBox = CreateDataSet.FinBox();
      //dsCreateListBox.Tables.Add(dtListBox.Clone());
      dsCreateListBox.Tables["dtListBox"].Merge(dtListBox);
      rpt.SetDataSource(dsCreateListBox);
      if (isDraft == 0)
      {
        rpt.SetParameterValue("Draft", "Draft");
      }
      else
      {
        rpt.SetParameterValue("Draft", "");
      }
      cptItemMaterialViewer.ReportSource = rpt;
    }

    private void LoadDataCheckListBox()
    {
      Technical.Reports.cptCheckFinishBoxInfoReport rpt = new DaiCo.Technical.Reports.cptCheckFinishBoxInfoReport();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptCheckFinishBoxInfoReport.rpt";
      rpt.FileName = filePath;
      rpt.Load();
      DataSet dsCreateListBox = CreateDataSet.FinBox();
      //dsCreateListBox.Tables.Add(dtListBox.Clone());
      dsCreateListBox.Tables["dtPackage"].Merge(dtListBox);
      rpt.SetDataSource(dsCreateListBox);
      rpt.SetParameterValue("PackingCode", this.itemCode);
      cptItemMaterialViewer.ReportSource = rpt;
    }

    /// <summary>
    /// Report Danh Sach nguyen lieu cua packing theo ItemCode
    /// </summary>
    ///
    private void ReportPackingMaterial()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@PackageCode", DbType.AnsiString, 50, this.packageCode);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListPackingMaterial", inputParam);

      DataSet dsPackingMaterial = PackingMaterialInfo();

      DataRow rowMaster = dsPackingMaterial.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch { }
      dsPackingMaterial.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsPackingMaterial.Tables["TblPackingInfo"].Merge(dsSource.Tables[0]);

      if (dsSource.Tables[1].Rows.Count == 0)
      {
        for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
        {
          DataRow newRow = dsSource.Tables[1].NewRow();
          newRow["BoxTypePid"] = dsSource.Tables[0].Rows[i]["BoxTypePid"];
          dsSource.Tables[1].Rows.Add(newRow);
        }
      }
      else
      {
        for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
        {
          long boxTypePid = DBConvert.ParseLong(dsSource.Tables[0].Rows[i]["BoxTypePid"].ToString());
          DataRow[] row = dsSource.Tables[1].Select(string.Format("BoxTypePid = {0}", boxTypePid));
          if (row.Length > 0)
          {
            continue;
          }
          else
          {
            DataRow newRow = dsSource.Tables[1].NewRow();
            newRow["BoxTypePid"] = dsSource.Tables[0].Rows[i]["BoxTypePid"];
            dsSource.Tables[1].Rows.Add(newRow);
          }
        }
      }

      dsPackingMaterial.Tables["TblMaterialInfo"].Merge(dsSource.Tables[1]);
      cptBOMPackingMaterialInfo cpt = new cptBOMPackingMaterialInfo();
      cpt.SetDataSource(dsPackingMaterial);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    public DataSet PackingMaterialInfo()
    {
      DataSet ds = new DataSet();

      DataTable tblMasterInfo = new DataTable("TblMasterInfo");
      tblMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      tblMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(tblMasterInfo);

      DataTable taParent = new DataTable("TblPackingInfo");

      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("PackageCode", typeof(System.String));
      taParent.Columns.Add("BoxTypePid", typeof(System.Int64));
      taParent.Columns.Add("BoxTypeCode", typeof(System.String));
      taParent.Columns.Add("EnglishName", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("CartonSize", typeof(System.String));
      taParent.Columns.Add("ItemPerBox", typeof(System.Int32));
      taParent.Columns.Add("BoxPerItem", typeof(System.Int32));
      taParent.Columns.Add("ItemReference", typeof(System.String));

      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblMaterialInfo");
      taChild.Columns.Add("BoxTypePid", typeof(System.Int64));
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("EnglishName", typeof(System.String));
      taChild.Columns.Add("VietnameseName", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("Remarks", typeof(System.String));

      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblPackingInfo_TblMaterialInfo", taParent.Columns["BoxTypePid"], taChild.Columns["BoxTypePid"], false));
      return ds;
    }

    /// <summary>
    /// Report Hardware Master List
    /// </summary>
    /// 
    private void ReportHardwareMasterList()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Revision", DbType.Int32, DBNull.Value);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListHarwareMaster", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[0].Columns.Add("ItemReference", typeof(System.String));
      foreach (DataRow row in dsSource.Tables[0].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(row["HardwareCode"].ToString());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          row["Image"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch
        {
        }
      }
      for (int i = 0; i <= dsSource.Tables[0].Rows.Count - 1; i++)
      {
        //ItemReference
        string harwareCode = dsSource.Tables[0].Rows[i]["HardwareCode"].ToString();
        string commandText = string.Format("Select ItemCode, Revision From TblBOMItemComponent Where ComponentCode = '{0}'", harwareCode);
        DataTable dtItemReference = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtItemReference != null)
        {
          string itemReference = string.Empty;
          foreach (DataRow itemRow in dtItemReference.Rows)
          {
            if (itemReference.Length > 0)
            {
              itemReference += ", ";
            }
            itemReference += string.Format("{0}", itemRow["ItemCode"]);
          }
          dsSource.Tables[0].Rows[i]["ItemReference"] = itemReference;
        }


      }
      dsBOMHardwareMasterList dsHardware = new dsBOMHardwareMasterList();
      DataRow rowMaster = dsHardware.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch
      {
      }
      dsHardware.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsHardware.Tables["TblHardwareMasterList"].Merge(dsSource.Tables[0]);
      cptBOMListHardware cpt = new cptBOMListHardware();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMListHardware.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsHardware);
      cpt.SetParameterValue("Title", "Hardware Code Master Report");
      cptItemMaterialViewer.ReportSource = cpt;
    }

    /// <summary>
    /// Report Component Master List
    /// </summary>
    /// 
    private void ReportComponentMasterList()
    {
      string compCode = string.Empty;
      if (this.dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in this.dtSource.Rows)
        {
          compCode += row["ComponentCode"].ToString() + ",";
        }
        compCode = compCode.Trim(',');
      }
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CompCode", DbType.AnsiString, 8000, compCode) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spRDDComponentMasterGroupReport", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[0].Columns.Add("ItemReference", typeof(System.String));
      foreach (DataRow row in dsSource.Tables[0].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(row["HardwareCode"].ToString());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          row["Image"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch
        {
        }
      }
      for (int i = 0; i <= dsSource.Tables[0].Rows.Count - 1; i++)
      {
        //ItemReference
        string componentCode = dsSource.Tables[0].Rows[i]["HardwareCode"].ToString();
        string commandText = string.Format("Select ItemCode, Revision From TblBOMItemComponent Where ComponentCode = '{0}'", componentCode);
        DataTable dtItemReference = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtItemReference != null)
        {
          string itemReference = string.Empty;
          foreach (DataRow itemRow in dtItemReference.Rows)
          {
            if (itemReference.Length > 0)
            {
              itemReference += ", ";
            }
            itemReference += string.Format("{0}", itemRow["ItemCode"]);
          }
          dsSource.Tables[0].Rows[i]["ItemReference"] = itemReference;
        }
      }
      dsBOMHardwareMasterList dsHardware = new dsBOMHardwareMasterList();
      DataRow rowMaster = dsHardware.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch
      {
      }
      dsHardware.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsHardware.Tables["TblHardwareMasterList"].Merge(dsSource.Tables[0]);
      cptBOMListHardware cpt = new cptBOMListHardware();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMListHardware.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsHardware);
      cpt.SetParameterValue("Title", "Component Master Report");
      cptItemMaterialViewer.ReportSource = cpt;
    }

    private void ReportItemComponentAccessory()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListAccessoryMaster", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[0].Columns.Add("Title", typeof(string));
      dsSource.Tables[0].Rows[0]["Title"] = "Accessory Master List 2nd Level";
      try
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        string imgPath = FunctionUtility.BOMGetItemImage(itemCode, revision);
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        row["Image"] = imgbyte;
        br.Close();
        fs.Close();
      }
      catch
      {
      }
      dsSource.Tables[1].Columns.Add("Image", typeof(System.Byte[]));
      foreach (DataRow row in dsSource.Tables[1].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(row["ComponentCode"].ToString());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          row["Image"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch
        {
        }
      }
      DataSet dsMasterInfo = CreateDataSet.ItemMasterInfo();
      dsMasterInfo.Tables["DataTable1"].Merge(dsSource.Tables[0]);
      DataSet dsAccessory = CreateDataSet.ItemComponentAccessory();
      DataRow rowMaster = dsAccessory.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch
      {
      }
      dsAccessory.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsAccessory.Tables["TblComponentInfo"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMItemComponentAccessory cpt = new Technical.Reports.cptBOMItemComponentAccessory();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMItemComponentAccessory.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsMasterInfo);
      cpt.SetDataSource(dsAccessory);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    /// <summary>
    /// REPORT FINISHING MATERIAL LIST LEVEL 3rd
    /// </summary>
    /// 
    private void ReportFinishingInfo()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@FinCode", DbType.AnsiString, 16, code);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListFinishingMaterial", inputParam);
      DataSet dsFinishingInfo = CreateDataSet.FinishingInfo();
      DataRow rowMaster = dsFinishingInfo.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch
      {
      }
      dsFinishingInfo.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsFinishingInfo.Tables["TblFinishingInfo"].Merge(dsSource.Tables[0]);
      if (dsSource.Tables[1].Rows.Count == 0)
      {
        DataRow row = dsSource.Tables[1].NewRow();
        dsSource.Tables[1].Rows.Add(row);
      }
      dsFinishingInfo.Tables["TblFinishingDetail"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMFinishingInfo cpt = new Technical.Reports.cptBOMFinishingInfo();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMFinishingInfo.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.SetDataSource(dsFinishingInfo);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    /// <summary>
    /// REPORT FINISHING MASTER LIST LEVEL 2nd
    /// </summary>
    /// 
    private void ReportComponentFinishing()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListFinishingMaster", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[0].Columns.Add("Title", typeof(string));
      dsSource.Tables[0].Rows[0]["Title"] = "Finishing Master List 2nd Level";
      try
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        string imgPath = FunctionUtility.BOMGetItemImage(itemCode, revision);
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        row["Image"] = imgbyte;
        br.Close();
        fs.Close();
      }
      catch
      {
      }
      DataSet dsMasterInfo = CreateDataSet.ItemMasterInfo();
      dsMasterInfo.Tables["DataTable1"].Merge(dsSource.Tables[0]);

      DataSet dsItemComponentFinishing = CreateDataSet.ItemComponentFinishing();
      DataRow rowMaster = dsItemComponentFinishing.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch
      {
      }
      dsItemComponentFinishing.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsItemComponentFinishing.Tables["TblItemComponent"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMItemComponentFinishing cpt = new DaiCo.Technical.Reports.cptBOMItemComponentFinishing();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMItemComponentFinishing.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsMasterInfo);
      cpt.SetDataSource(dsItemComponentFinishing);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    /// <summary>
    ///  REPORT GLASS MASTER LIST LEVEL 2nd 
    /// </summary>
    /// 
    private void ReportItemComponentGlass()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListGlassMaster", inputParam);
      dsSource.Tables[0].Columns.Add("Image", typeof(System.Byte[]));
      dsSource.Tables[0].Columns.Add("Title", typeof(string));
      dsSource.Tables[0].Rows[0]["Title"] = "Glass Master List 2nd Level";
      try
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        string imgPath = FunctionUtility.BOMGetItemImage(itemCode, revision);
        FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        row["Image"] = imgbyte;
        br.Close();
        fs.Close();
      }
      catch
      {
      }
      dsSource.Tables[1].Columns.Add("Image", typeof(System.Byte[]));
      foreach (DataRow row in dsSource.Tables[1].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(row["ComponentCode"].ToString());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          row["Image"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch
        {
        }
      }

      DataSet dsMasterInfo = CreateDataSet.ItemMasterInfo();
      dsMasterInfo.Tables["DataTable1"].Merge(dsSource.Tables[0]);
      DataSet dsItemComponentGlass = CreateDataSet.ItemComponentGLass();
      DataRow rowMaster = dsItemComponentGlass.Tables["TblMasterInfo"].NewRow();
      rowMaster["UserName"] = Shared.Utility.SharedObject.UserInfo.UserName;
      try
      {
        string logoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, ConstantClass.PATH_LOGO);
        FileStream fslogo = new FileStream(logoPath, FileMode.Open, FileAccess.Read);
        BinaryReader brlogo = new BinaryReader(fslogo);
        byte[] imgbytelogo = new byte[fslogo.Length + 1];
        imgbytelogo = brlogo.ReadBytes(Convert.ToInt32((fslogo.Length)));
        rowMaster["LogoImage"] = imgbytelogo;
        brlogo.Close();
        fslogo.Close();
      }
      catch
      {
      }
      dsItemComponentGlass.Tables["TblMasterInfo"].Rows.Add(rowMaster);
      dsItemComponentGlass.Tables["TblComponentInfo"].Merge(dsSource.Tables[1]);
      Technical.Reports.cptBOMItemComponentGlass cpt = new DaiCo.Technical.Reports.cptBOMItemComponentGlass();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMItemComponentGlass.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.Subreports[0].SetDataSource(dsMasterInfo);
      cpt.SetDataSource(dsItemComponentGlass);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    /// <summary>
    /// Load Data to cptBOMItemInfo
    /// </summary>
    private void ReportItemMaster()
    {
      // Load Data Sub Item Info
      string commandTextItemInfo = string.Format(@"SELECT CUS.Name Customer, BS.SaleCode, ITEM.ItemCode, ITEM.Revision, BS.Name [ItemName], BS.[Description], CM1.Category, CM2.Value [Collection], (FI.FinCode + ' / ' + FI.Name) [MainFinishing], ITEM.KD
                                                   FROM TblBOMItemInfo ITEM 
	                                                    INNER JOIN TblBOMItemBasic BS ON ITEM.ItemCode = BS.ItemCode AND (ITEM.ItemCode = '{0}') AND (ITEM.Revision = {1})
                                                      LEFT JOIN TblCSDCustomerInfo CUS ON BS.CustomerPid = CUS.Pid
	                                                    LEFT JOIN TblBOMFinishingInfo FI	ON (ITEM.MainFinish = FI.FinCode)
	                                                    LEFT JOIN TblCSDCategory CM1		ON (BS.Category = CM1.Pid)
	                                                    LEFT JOIN TblBOMCodeMaster CM2		ON (BS.[Collection] = CM2.Code AND CM2.[Group] = 2)", this.itemCode, this.revision);
      DataTable dtItemInfo = DataBaseAccess.SearchCommandTextDataTable(commandTextItemInfo);
      dtItemInfo.Columns.Add("Title", typeof(string));
      dtItemInfo.Rows[0]["Title"] = "ITEM MASTER LEVEL 1st";
      dsBOMItemMasterInfo dsItemMaster = new dsBOMItemMasterInfo();
      dsItemMaster.Tables["DataTable1"].Merge(dtItemInfo);
      dsItemMaster.Tables["DataTable1"].Rows[0]["Image"] = FunctionUtility.ImagePathToByteArray(FunctionUtility.BOMGetItemImage(this.itemCode, this.revision));

      //Load Sub Item Other Size
      string commandTextOtherSize = string.Format(@"SELECT Code.[Description] [Type], More.[Values] as mm
                                                    FROM TblBOMMoreDimension More 
                                                            INNER JOIN TblBOMCodeMaster Code ON (More.DimensionKind = Code.Code)
                                                    WHERE ItemCode = '{0}' AND Revision = {1} AND Code.[Group] = 5 AND Code.DeleteFlag = 0", this.itemCode, this.revision);
      DataTable dtOtherDimension = DataBaseAccess.SearchCommandTextDataTable(commandTextOtherSize);
      dtOtherDimension.Columns.Add("Inch", typeof(string));
      for (int j = 0; j < dtOtherDimension.Rows.Count; j++)
      {
        dtOtherDimension.Rows[j]["Inch"] = FunctionUtility.ConverMilimetToInch(DBConvert.ParseInt(dtOtherDimension.Rows[j]["mm"].ToString()));
      }
      DataSet dsItemOtherSize = CreateDataSet.ItemInfoSupReportItemSize();
      dsItemOtherSize.Tables["dtSubReportOtherSize"].Merge(dtOtherDimension);

      //Load Sub Carton Box
      string commandTextCartonBox = string.Format(@"Select Dimension.BoxTypeCode, WHFD.[Length], WHFD.Width, WHFD.Height, Dimension.GWeight, Dimension.NWeight
                                                    From TblWHFDimension WHFD INNER JOIN (Select BType.BoxTypeCode, BType.DimensionPid, BType.GWeight, BType.NWeight
										                                                                      From TblBOMBoxType BType INNER JOIN TblBOMPackage Pack 
											                                                                                             ON (Pack.Pid = BType.PackagePid)
										                                                                      Where Pack.ItemCode = '{0}' AND Pack.Revision = {1}) Dimension
							                                                                ON (WHFD.Pid = Dimension.DimensionPid)", this.itemCode, this.revision);
      DataTable dtCartonBox = DataBaseAccess.SearchCommandTextDataTable(commandTextCartonBox);

      dsBOMCartonBoxOfItem dsCartonBox = new dsBOMCartonBoxOfItem();
      dsCartonBox.Tables["dtCartonBoxOfItem"].Merge(dtCartonBox);

      // Load Item Size (mm, inch)
      string commandTextItemSize = string.Format(@"Select Info.ItemCode, Info.Revision, Info.Width MMWidth, Info.Depth MMDepth, Info.High MMHight, 
		                                                         Package.PackageCode, Package.QuantityBox, Package.QuantityItem, Package.TotalCBM
                                                    From TblBOMItemInfo Info LEFT JOIN TblBOMPackage Package 
				                                                        ON (Info.PackageCode = Package.PackageCode)
                                                    Where Info.ItemCode = '{0}' AND Info.Revision = {1}", this.itemCode, this.revision);

      DataTable dtItemSize = DataBaseAccess.SearchCommandTextDataTable(commandTextItemSize);
      dtItemSize.Columns.Add("RevisionRecord", typeof(string));
      dtItemSize.Columns.Add("Relative", typeof(string));
      dtItemSize.Columns.Add("InchWidth", typeof(string));
      dtItemSize.Columns.Add("InchDepth", typeof(string));
      dtItemSize.Columns.Add("InchHight", typeof(string));
      string strRevisionRecord = string.Empty;
      string strSaleRelation = string.Empty;

      if (dtRevisionRecord != null)
      {
        for (int i = 0; i < dtRevisionRecord.Rows.Count; i++)
        {
          int length = dtRevisionRecord.Rows[i]["Linked"].ToString().Split('\\').Length;
          string strfileName = dtRevisionRecord.Rows[i]["Linked"].ToString().Split('\\')[length - 1].Trim();
          strRevisionRecord += dtRevisionRecord.Rows[i]["Note"].ToString() + " Reference to " + strfileName + System.Environment.NewLine;
        }
      }

      if (dtSaleRelation != null)
      {
        for (int i = 0; i < dtSaleRelation.Rows.Count; i++)
        {
          if (dtSaleRelation.Rows[i]["Pid"].ToString().Length != 0)
          {
            string strName = dtSaleRelation.Rows[i]["Name"].ToString();
            strSaleRelation += dtSaleRelation.Rows[i]["RelativeItem"].ToString() + " " + strName + ", ";
          }
          else
          {
            string strName = dtSaleRelation.Rows[i]["Name"].ToString();
            strSaleRelation += dtSaleRelation.Rows[i]["RelativeItem"].ToString() + " " + strName + ".";
          }
        }
        strSaleRelation = strSaleRelation.Trim().TrimEnd(',') + ".";
      }

      if (dtItemSize != null && dtItemSize.Rows.Count > 0)
      {
        dtItemSize.Rows[0]["Relative"] = strSaleRelation;
        dtItemSize.Rows[0]["RevisionRecord"] = strRevisionRecord;
        dtItemSize.Rows[0]["InchWidth"] = FunctionUtility.ConverMilimetToInch(DBConvert.ParseInt(dtItemSize.Rows[0]["MMWidth"].ToString()));
        dtItemSize.Rows[0]["InchDepth"] = FunctionUtility.ConverMilimetToInch(DBConvert.ParseInt(dtItemSize.Rows[0]["MMDepth"].ToString()));
        dtItemSize.Rows[0]["InchHight"] = FunctionUtility.ConverMilimetToInch(DBConvert.ParseInt(dtItemSize.Rows[0]["MMHight"].ToString()));
      }
      else
      {
        DataRow row = dtItemSize.NewRow();
        dtItemSize.Rows.Add(row);
      }
      DataSet dsItemInfoSize = CreateDataSet.ItemSize();
      dsItemInfoSize.Tables["dtItemSize"].Merge(dtItemSize);

      Technical.Reports.cptBOMItemInfo cpt = new DaiCo.Technical.Reports.cptBOMItemInfo();
      string filePath = System.Windows.Forms.Application.StartupPath + @"\Reports\cptBOMItemInfo.rpt";
      cpt.FileName = filePath;
      cpt.Load();
      cpt.OpenSubreport("cptItemMasterInfo.rpt").SetDataSource(dsItemMaster);
      cpt.OpenSubreport("cptBOMItemInfoSupReportItemSize.rpt").SetDataSource(dsItemOtherSize);
      cpt.OpenSubreport("cptBOMItemInfoSupReportCartonBox.rpt").SetDataSource(dsCartonBox);

      cpt.SetDataSource(dsItemInfoSize);
      cpt.SetParameterValue("User", Shared.Utility.SharedObject.UserInfo.UserName);
      cptItemMaterialViewer.ReportSource = cpt;
    }

    #endregion LoadReport

    #region more function
    private Byte[] ImagePathToByteArray(string imagePath)
    {
      try
      {
        FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        br.Close();
        fs.Close();
        return imgbyte;
      }
      catch { }
      return null;
    }
    #endregion more function
  }
}