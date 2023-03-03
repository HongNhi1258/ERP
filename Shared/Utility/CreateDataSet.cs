using System.Data;

namespace DaiCo.Shared.Utility
{
  public class CreateDataSet
  {
    public static DataSet SaleOrder_Enquiry()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("No", typeof(System.String));
      taParent.Columns.Add("CusNo", typeof(System.String));
      taParent.Columns.Add("OrderDate", typeof(System.String));
      taParent.Columns.Add("Customer", typeof(System.String));
      taParent.Columns.Add("Direct", typeof(System.String));
      taParent.Columns.Add("Type", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("ParentPid", typeof(System.Int64));
      taChild.Columns.Add("PO Number", typeof(System.String));
      taChild.Columns.Add("SaleCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("CBM", typeof(System.Double));
      taChild.Columns.Add("TotalCBM", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["ParentPid"], false));
      return ds;
    }

    public static DataSet Load_List()
    {
      DataSet ds = new DataSet();

      DataTable taRoot = new DataTable("dtRoot");
      taRoot.Columns.Add("Pid", typeof(System.Int64));
      taRoot.Columns.Add("Item Code", typeof(System.String));
      taRoot.Columns.Add("Revision", typeof(System.String));
      taRoot.Columns.Add("Item Group", typeof(System.String));
      taRoot.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taRoot);

      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("Item Code", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.String));
      taParent.Columns.Add("Package Code", typeof(System.String));
      taParent.Columns.Add("Package Name", typeof(System.String));
      taParent.Columns.Add("Quantity Item", typeof(System.String));
      taParent.Columns.Add("Quantity Box", typeof(System.String));
      taParent.Columns.Add("Total CBM", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("Must Ship Qty", typeof(System.Int32));
      taParent.Columns.Add("Check Box", typeof(System.Int32));
      ds.Tables.Add(taParent);


      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("Item Code", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.String));
      taChild.Columns.Add("Box Type", typeof(System.String));
      taChild.Columns.Add("BoxTypePid", typeof(System.Int64));
      taChild.Columns.Add("Item Group", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      taChild.Columns.Add("Must Ship Qty", typeof(System.Int32));
      taChild.Columns.Add("Remark", typeof(System.String));
      taChild.Columns.Add("Check Box", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtRoot_dtParent", taRoot.Columns["Pid"], taParent.Columns["Pid"], false));
      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["Pid"], false));
      return ds;
    }

    public static DataSet EnquiryConfirm()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Select", typeof(System.Int32));
      taParent.Columns.Add("No", typeof(System.Int64));
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("SaleCode", typeof(System.String));
      //taParent.Columns.Add("EnquirySale", typeof(System.Int32));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Name", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("MOQQty", typeof(System.Int32));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("CBM", typeof(System.Double));
      taParent.Columns.Add("TotalCBM", typeof(System.Double));
      taParent.Columns.Add("RequestDate", typeof(System.DateTime));
      taParent.Columns.Add("Price", typeof(System.Double));
      taParent.Columns.Add("Amount", typeof(System.Double));
      taParent.Columns.Add("SecondPrice", typeof(System.Double));
      taParent.Columns.Add("SecondAmount", typeof(System.Double));
      taParent.Columns.Add("RequiredShipDate", typeof(System.DateTime));
      taParent.Columns.Add("SpecialInstruction", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("EnquiryDetailPid", typeof(System.Int64));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("ScheduleDate", typeof(System.DateTime));
      taChild.Columns.Add("Expire", typeof(System.Int32));
      taChild.Columns.Add("Keep", typeof(System.Int32));
      taChild.Columns.Add("KeepDays", typeof(System.Int32));
      taChild.Columns.Add("NonPlan", typeof(System.Int32));
      taChild.Columns.Add("Remark", typeof(System.String));

      ds.Tables.Add(taChild);

      taChild.Columns["Expire"].DefaultValue = 0;
      taChild.Columns["Keep"].DefaultValue = 0;
      taChild.Columns["NonPlan"].DefaultValue = 0;
      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["EnquiryDetailPid"], false));
      return ds;
    }
    public static DataSet EnquiryConfirmAnswer()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Select", typeof(System.Int32));
      taParent.Columns.Add("No", typeof(System.Int64));
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("SaleCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Name", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("CBM", typeof(System.Double));
      taParent.Columns.Add("TotalCBM", typeof(System.Double));
      taParent.Columns.Add("RequestDate", typeof(System.DateTime));
      taParent.Columns.Add("Price", typeof(System.Double));
      taParent.Columns.Add("Amount", typeof(System.Double));
      taParent.Columns.Add("SecondPrice", typeof(System.Double));
      taParent.Columns.Add("SecondAmount", typeof(System.Double));
      taParent.Columns.Add("RequiredShipDate", typeof(System.DateTime));
      taParent.Columns.Add("SpecialInstruction", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("CarcassSUB", typeof(System.String));

      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("EnquiryDetailPid", typeof(System.Int64));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("ScheduleDate", typeof(System.DateTime));
      taChild.Columns.Add("Expire", typeof(System.Int32));
      taChild.Columns.Add("Keep", typeof(System.Int32));
      taChild.Columns.Add("KeepDays", typeof(System.Int32));
      taChild.Columns.Add("NonPlan", typeof(System.Int32));
      taChild.Columns.Add("Remark", typeof(System.String));

      ds.Tables.Add(taChild);

      taChild.Columns["Expire"].DefaultValue = 0;
      taChild.Columns["Keep"].DefaultValue = 0;
      taChild.Columns["NonPlan"].DefaultValue = 0;
      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["EnquiryDetailPid"], false));

      //Differnt Enquiry
      DataTable taChildDiff = new DataTable("dtChildDiffEn");
      taChildDiff.Columns.Add("EnquiryPid", typeof(System.Int64));
      taChildDiff.Columns.Add("EnquiryNo", typeof(System.String));
      taChildDiff.Columns.Add("CustomerCode", typeof(System.String));
      taChildDiff.Columns.Add("ItemCode", typeof(System.String));
      taChildDiff.Columns.Add("Revision", typeof(System.Int32));
      taChildDiff.Columns.Add("Qty", typeof(System.Double));
      taChildDiff.Columns.Add("ScheduleDate", typeof(System.DateTime));
      taChildDiff.Columns.Add("Expire", typeof(System.Int32));
      taChildDiff.Columns.Add("Keep", typeof(System.Int32));
      taChildDiff.Columns.Add("KeepDays", typeof(System.Int32));
      taChildDiff.Columns.Add("NonPlan", typeof(System.Int32));
      taChildDiff.Columns.Add("Remark", typeof(System.String));

      ds.Tables.Add(taChildDiff);

      taChildDiff.Columns["Expire"].DefaultValue = 0;
      taChildDiff.Columns["Keep"].DefaultValue = 0;
      taChildDiff.Columns["NonPlan"].DefaultValue = 0;
      ds.Relations.Add(new DataRelation("dtParent_dtChildDiff", new DataColumn[] { taParent.Columns["ItemCode"], taParent.Columns["Revision"] }, new DataColumn[] { taChildDiff.Columns["ItemCode"], taChildDiff.Columns["Revision"] }, false));

      DataTable taChildTwo = new DataTable("dtChildTwo");
      taChildTwo.Columns.Add("No", typeof(System.Int64));
      taChildTwo.Columns.Add("SaleCode", typeof(System.String));
      taChildTwo.Columns.Add("CarcassCode", typeof(System.String));
      taChildTwo.Columns.Add("OldCode", typeof(System.String));
      taChildTwo.Columns.Add("ItemCode", typeof(System.String));
      taChildTwo.Columns.Add("Revision", typeof(System.Int32));
      taChildTwo.Columns.Add("CarcassSUB", typeof(System.String));
      taChildTwo.Columns.Add("UCBM", typeof(System.Double));
      taChildTwo.Columns.Add("CustCode", typeof(System.String));
      taChildTwo.Columns.Add("SaleNo", typeof(System.String));
      taChildTwo.Columns.Add("PONo", typeof(System.String));
      taChildTwo.Columns.Add("PODate", typeof(System.String));
      taChildTwo.Columns.Add("SpecialRemark", typeof(System.String));
      taChildTwo.Columns.Add("PackingNote", typeof(System.String));
      taChildTwo.Columns.Add("ConfirmedShipDate", typeof(System.String));
      taChildTwo.Columns.Add("UrgentNote", typeof(System.String));
      taChildTwo.Columns.Add("OrderQty", typeof(System.Int32));
      taChildTwo.Columns.Add("ShippedQty", typeof(System.Int32));
      taChildTwo.Columns.Add("CancelledQty", typeof(System.Int32));
      taChildTwo.Columns.Add("Balance", typeof(System.Int32));
      taChildTwo.Columns.Add("TotalBalance", typeof(System.Int32));
      taChildTwo.Columns.Add("TotalWIP", typeof(System.Int32));
      taChildTwo.Columns.Add("TotalUnrelease", typeof(System.Int32));
      taChildTwo.Columns.Add("TotalUnReleaseSameCarcass", typeof(System.Int32));
      taChildTwo.Columns.Add("House/Sub", typeof(System.String));
      taChildTwo.Columns.Add("WO", typeof(System.String));
      taChildTwo.Columns.Add("StatusWIP", typeof(System.String));
      taChildTwo.Columns.Add("Item/Box", typeof(System.String));
      taChildTwo.Columns.Add("ContainerNo", typeof(System.String));
      taChildTwo.Columns.Add("LoadingDate", typeof(System.String));
      taChildTwo.Columns.Add("LoadingQty", typeof(System.Int32));
      taChildTwo.Columns.Add("LoadingCBM", typeof(System.Double));
      taChildTwo.Columns.Add("PackingQty", typeof(System.String));
      taChildTwo.Columns.Add("PackingCBM", typeof(System.Double));
      taChildTwo.Columns.Add("WIPStatusForContainer", typeof(System.String));
      taChildTwo.Columns.Add("RepairQty", typeof(System.Int32));
      taChildTwo.Columns.Add("StatusRepair", typeof(System.String));
      taChildTwo.Columns.Add("FGWReceivedDate", typeof(System.DateTime));
      taChildTwo.Columns.Add("MCHDeadline", typeof(System.String));
      taChildTwo.Columns.Add("SUBDeadline", typeof(System.String));
      taChildTwo.Columns.Add("FOUDeadline", typeof(System.String));
      taChildTwo.Columns.Add("USStock", typeof(System.Int32));
      taChildTwo.Columns.Add("AVEUS6M", typeof(System.Double));
      taChildTwo.Columns.Add("AVEUS12M", typeof(System.Double));
      ds.Tables.Add(taChildTwo);
      ds.Relations.Add(new DataRelation("dtParent_dtChildTwo", new DataColumn[] { taParent.Columns["CarcassSUB"] }, new DataColumn[] { taChildTwo.Columns["CarcassSUB"] }, false));
      return ds;

    }

    public static DataSet SaleOrderCancel()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("No", typeof(System.String));
      taParent.Columns.Add("CancelDate", typeof(System.String));
      taParent.Columns.Add("CusCancelNo", typeof(System.String));
      taParent.Columns.Add("Customer", typeof(System.String));
      taParent.Columns.Add("Direct", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("ParentPid", typeof(System.Int64));
      taChild.Columns.Add("SaleCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("CBM", typeof(System.Double));
      taChild.Columns.Add("TotalCBM", typeof(System.Double));
      ds.Tables.Add(taChild);
      DataTable taChildSwap = new DataTable("dtChildSwap");
      taChildSwap.Columns.Add("ParentDetailPid", typeof(System.Int64));
      taChildSwap.Columns.Add("SaleCode", typeof(System.String));
      taChildSwap.Columns.Add("ItemCode", typeof(System.String));
      taChildSwap.Columns.Add("Revision", typeof(System.Int32));
      taChildSwap.Columns.Add("Name", typeof(System.String));
      taChildSwap.Columns.Add("Qty", typeof(System.Double));
      ds.Tables.Add(taChildSwap);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["ParentPid"], false));
      ds.Relations.Add(new DataRelation("dtChild_dtChildSwap", taChild.Columns["Pid"], taChildSwap.Columns["ParentDetailPid"], false));
      return ds;
    }

    public static DataSet SaleOrderCancelDetail()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("SaleOrderPid", typeof(System.Int64));
      taParent.Columns.Add("SaleNo", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("TotalQty", typeof(System.Int32));
      taParent.Columns.Add("OpenWO", typeof(System.Int32));
      taParent.Columns.Add("Cancelled", typeof(System.Int32));
      taParent.Columns.Add("Remain", typeof(System.Int32));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("POCancelDetailPid", typeof(System.Int64));
      taChild.Columns.Add("SaleOrderDetailPid", typeof(System.Int64));
      taChild.Columns.Add("ScheduleDate", typeof(System.DateTime));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("SO Qty", typeof(System.Int32));
      taChild.Columns.Add("OpenWO", typeof(System.Int32));
      taChild.Columns.Add("Cancelled", typeof(System.Int32));
      taChild.Columns.Add("Remain", typeof(System.Int32));
      taChild.Columns.Add("NewCancelQty", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("taParent_taChild", new DataColumn[] { taParent.Columns["ItemCode"], taParent.Columns["Revision"], taParent.Columns["Pid"] }, new DataColumn[] { taChild.Columns["ItemCode"], taChild.Columns["Revision"], taChild.Columns["POCancelDetailPid"] }));
      return ds;
    }    

    public static DataSet WorkDeadLine()
    {
      DataSet ds = new DataSet();
      DataTable dtWorkOderInfomation = new DataTable("dtWorkOderInfomation");
      dtWorkOderInfomation.Columns.Add("ItemCode", typeof(System.String));
      dtWorkOderInfomation.Columns.Add("Revision", typeof(System.Int32));
      dtWorkOderInfomation.Columns.Add("NameEN", typeof(System.String));
      dtWorkOderInfomation.Columns.Add("NameVN", typeof(System.String));
      dtWorkOderInfomation.Columns.Add("Qty", typeof(System.Int32));
      dtWorkOderInfomation.Columns.Add("RowState", typeof(System.Int32));
      ds.Tables.Add(dtWorkOderInfomation);

      DataTable dtMCHDeadline = new DataTable("dtMCHDeadline");
      dtMCHDeadline.Columns.Add("Pid", typeof(System.Int64));
      dtMCHDeadline.Columns.Add("ItemCode", typeof(System.String));
      dtMCHDeadline.Columns.Add("Revision", typeof(System.Int32));
      dtMCHDeadline.Columns.Add("Deadline", typeof(System.DateTime));
      dtMCHDeadline.Columns.Add("Qty", typeof(System.Int32));
      dtMCHDeadline.Columns.Add("RowState", typeof(System.Int32));
      ds.Tables.Add(dtMCHDeadline);

      DataTable tdASSDeadline = new DataTable("dtASSDeadline");
      tdASSDeadline.Columns.Add("Pid", typeof(System.Int64));
      tdASSDeadline.Columns.Add("ItemCode", typeof(System.String));
      tdASSDeadline.Columns.Add("Revision", typeof(System.Int32));
      tdASSDeadline.Columns.Add("Deadline", typeof(System.DateTime));
      tdASSDeadline.Columns.Add("Qty", typeof(System.Int32));
      tdASSDeadline.Columns.Add("RowState", typeof(System.Int32));
      ds.Tables.Add(tdASSDeadline);

      DataTable tdSUBDeadline = new DataTable("dtSUBDeadline");
      tdSUBDeadline.Columns.Add("Pid", typeof(System.Int64));
      tdSUBDeadline.Columns.Add("ItemCode", typeof(System.String));
      tdSUBDeadline.Columns.Add("Revision", typeof(System.Int32));
      tdSUBDeadline.Columns.Add("Deadline", typeof(System.DateTime));
      tdSUBDeadline.Columns.Add("Qty", typeof(System.Int32));
      tdSUBDeadline.Columns.Add("RowState", typeof(System.Int32));
      ds.Tables.Add(tdSUBDeadline);

      DataTable tdPACDeadline = new DataTable("dtPACDeadline");
      tdPACDeadline.Columns.Add("Pid", typeof(System.Int64));
      tdPACDeadline.Columns.Add("ItemCode", typeof(System.String));
      tdPACDeadline.Columns.Add("Revision", typeof(System.Int32));
      tdPACDeadline.Columns.Add("Deadline", typeof(System.DateTime));
      tdPACDeadline.Columns.Add("Qty", typeof(System.Int32));
      tdPACDeadline.Columns.Add("RowState", typeof(System.Int32));
      ds.Tables.Add(tdPACDeadline);

      DataRelation dtWorkOderInfomation_dtMCHDeadline = new DataRelation("dtWorkOderInfomation_dtMCHDeadline", new DataColumn[] { dtWorkOderInfomation.Columns["ItemCode"], dtWorkOderInfomation.Columns["Revision"] }, new DataColumn[] { dtMCHDeadline.Columns["ItemCode"], dtMCHDeadline.Columns["Revision"] }, false);

      DataRelation dtWorkOderInfomation_tdASSDeadline = new DataRelation("dtWorkOderInfomation_tdASSDeadline", new DataColumn[] { dtWorkOderInfomation.Columns["ItemCode"], dtWorkOderInfomation.Columns["Revision"] }, new DataColumn[] { tdASSDeadline.Columns["ItemCode"], tdASSDeadline.Columns["Revision"] }, false);

      DataRelation dtWorkOderInfomation_tdSUBDeadline = new DataRelation("dtWorkOderInfomation_tdSUBDeadline", new DataColumn[] { dtWorkOderInfomation.Columns["ItemCode"], dtWorkOderInfomation.Columns["Revision"] }, new DataColumn[] { tdSUBDeadline.Columns["ItemCode"], tdSUBDeadline.Columns["Revision"] }, false);

      DataRelation dtWorkOderInfomation_tdPACDeadline = new DataRelation("dtWorkOderInfomation_tdPACDeadline", new DataColumn[] { dtWorkOderInfomation.Columns["ItemCode"], dtWorkOderInfomation.Columns["Revision"] }, new DataColumn[] { tdPACDeadline.Columns["ItemCode"], tdPACDeadline.Columns["Revision"] }, false);

      ds.Relations.Add(dtWorkOderInfomation_dtMCHDeadline);
      ds.Relations.Add(dtWorkOderInfomation_tdASSDeadline);
      ds.Relations.Add(dtWorkOderInfomation_tdSUBDeadline);
      ds.Relations.Add(dtWorkOderInfomation_tdPACDeadline);

      return ds;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static DataSet CarcassDetail()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblRDDCarcassInfo");
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("Description", typeof(System.String));
      taParent.Columns.Add("DescriptionVN", typeof(System.String));
      taParent.Columns.Add("ImageDaiCo", typeof(System.Byte[]));

      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("TblRDDCarcassDetail");
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialName", typeof(System.String));
      taChild.Columns.Add("Description", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("Waste", typeof(System.Double));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("Thickness", typeof(System.Double));
      taChild.Columns.Add("Alternative", typeof(System.String));
      ds.Tables.Add(taChild);

      return ds;
    }

    ////////Technical
    public static DataSet DitectLaborInfo()
    {
      DataSet ds = new DataSet();
      DataTable taDirectLaborInfo = new DataTable("TblDirectLaborInfo");
      taDirectLaborInfo.Columns.Add("SectionCode", typeof(System.String));
      taDirectLaborInfo.Columns.Add("SectionName", typeof(System.String));
      taDirectLaborInfo.Columns.Add("ManHour", typeof(System.Double));
      taDirectLaborInfo.Columns.Add("Remarks", typeof(System.String));
      ds.Tables.Add(taDirectLaborInfo);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;
    }

    public static DataSet FinishingInfo()
    {
      DataSet ds = new DataSet();
      DataTable taFinishingInfo = new DataTable("TblFinishingInfo");
      taFinishingInfo.Columns.Add("FinCode", typeof(System.String));
      taFinishingInfo.Columns.Add("Name", typeof(System.String));
      taFinishingInfo.Columns.Add("Unit", typeof(System.String));
      ds.Tables.Add(taFinishingInfo);
      DataTable taFinishingDetail = new DataTable("TblFinishingDetail");
      taFinishingDetail.Columns.Add("MaterialCode", typeof(System.String));
      taFinishingDetail.Columns.Add("MaterialName", typeof(System.String));
      taFinishingDetail.Columns.Add("MaterialNameVn", typeof(System.String));
      taFinishingDetail.Columns.Add("Unit", typeof(System.String));
      taFinishingDetail.Columns.Add("Qty", typeof(System.Double));
      taFinishingDetail.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taFinishingDetail);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;
    }

    public static DataSet GlassMasterList()
    {
      DataSet ds = new DataSet();
      DataTable taGlassMasterList = new DataTable("TblGlassMasterList");
      taGlassMasterList.Columns.Add("No", typeof(System.Int64));
      taGlassMasterList.Columns.Add("GlassCode", typeof(System.String));
      taGlassMasterList.Columns.Add("MaterialCode", typeof(System.String));
      taGlassMasterList.Columns.Add("Description", typeof(System.String));
      taGlassMasterList.Columns.Add("Unit", typeof(System.String));
      taGlassMasterList.Columns.Add("Dimension", typeof(System.String));
      taGlassMasterList.Columns.Add("Image", typeof(System.Byte[]));
      taGlassMasterList.Columns.Add("ItemReference", typeof(System.String));
      ds.Tables.Add(taGlassMasterList);

      return ds;
    }

    public static DataSet HardwareMasterList()
    {
      DataSet ds = new DataSet();
      DataTable taHardwareMasterList = new DataTable("TblHardwareMasterList");
      taHardwareMasterList.Columns.Add("No", typeof(System.Int64));
      taHardwareMasterList.Columns.Add("HardwareCode", typeof(System.String));
      taHardwareMasterList.Columns.Add("Revision", typeof(System.Int32));
      taHardwareMasterList.Columns.Add("DescriptionEN", typeof(System.String));
      taHardwareMasterList.Columns.Add("DescriptionVN", typeof(System.String));
      taHardwareMasterList.Columns.Add("Unit", typeof(System.String));
      taHardwareMasterList.Columns.Add("Dimension", typeof(System.String));
      taHardwareMasterList.Columns.Add("Image", typeof(System.Byte[]));
      taHardwareMasterList.Columns.Add("ItemReference", typeof(System.String));
      ds.Tables.Add(taHardwareMasterList);

      return ds;
    }

    public static DataSet ItemComponentAccessory()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("SaleCode", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      taItemInfo.Columns.Add("ItemName", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("Category", typeof(System.String));
      taItemInfo.Columns.Add("Collection", typeof(System.String));
      taItemInfo.Columns.Add("MainFinishing", typeof(System.String));
      taItemInfo.Columns.Add("KD", typeof(System.Int32));
      taItemInfo.Columns.Add("Image", typeof(System.Byte[]));
      ds.Tables.Add(taItemInfo);
      DataTable taComponentInfo = new DataTable("TblComponentInfo");
      taComponentInfo.Columns.Add("ComponentCode", typeof(System.String));
      taComponentInfo.Columns.Add("MaterialCode", typeof(System.String));
      taComponentInfo.Columns.Add("Desription", typeof(System.String));
      taComponentInfo.Columns.Add("Qty", typeof(System.Double));
      taComponentInfo.Columns.Add("Unit", typeof(System.String));
      taComponentInfo.Columns.Add("Length", typeof(System.Double));
      taComponentInfo.Columns.Add("Width", typeof(System.Double));
      taComponentInfo.Columns.Add("Thickness", typeof(System.Double));
      taComponentInfo.Columns.Add("Remark", typeof(System.String));
      taComponentInfo.Columns.Add("ContractOut", typeof(System.Int32));
      taComponentInfo.Columns.Add("Image", typeof(System.Byte[]));
      ds.Tables.Add(taComponentInfo);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;
    }

    public static DataSet ItemComponentFinishing()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("SaleCode", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      taItemInfo.Columns.Add("ItemName", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("Category", typeof(System.String));
      taItemInfo.Columns.Add("Collection", typeof(System.String));
      taItemInfo.Columns.Add("MainFinishing", typeof(System.String));
      taItemInfo.Columns.Add("KD", typeof(System.Int32));
      taItemInfo.Columns.Add("Image", typeof(System.Byte[]));
      ds.Tables.Add(taItemInfo);
      DataTable taItemComponent = new DataTable("TblItemComponent");
      taItemComponent.Columns.Add("FinishingCode", typeof(System.String));
      taItemComponent.Columns.Add("Description", typeof(System.String));
      taItemComponent.Columns.Add("Qty", typeof(System.Double));
      taItemComponent.Columns.Add("Unit", typeof(System.String));
      taItemComponent.Columns.Add("Specification", typeof(System.String));
      taItemComponent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taItemComponent);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;
    }

    public static DataSet ItemComponentGLass()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("SaleCode", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      taItemInfo.Columns.Add("ItemName", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("Category", typeof(System.String));
      taItemInfo.Columns.Add("Collection", typeof(System.String));
      taItemInfo.Columns.Add("MainFinishing", typeof(System.String));
      taItemInfo.Columns.Add("KD", typeof(System.Int32));
      taItemInfo.Columns.Add("Image", typeof(System.Byte[]));
      ds.Tables.Add(taItemInfo);
      DataTable taComponentInfo = new DataTable("TblComponentInfo");
      taComponentInfo.Columns.Add("ComponentCode", typeof(System.String));
      taComponentInfo.Columns.Add("MaterialCode", typeof(System.String));
      taComponentInfo.Columns.Add("Desription", typeof(System.String));
      taComponentInfo.Columns.Add("Qty", typeof(System.Double));
      taComponentInfo.Columns.Add("Unit", typeof(System.String));
      taComponentInfo.Columns.Add("Length", typeof(System.Double));
      taComponentInfo.Columns.Add("Width", typeof(System.Double));
      taComponentInfo.Columns.Add("Thickness", typeof(System.Double));
      taComponentInfo.Columns.Add("Remark", typeof(System.String));
      taComponentInfo.Columns.Add("ContractOut", typeof(System.Int32));
      taComponentInfo.Columns.Add("Image", typeof(System.Byte[]));
      ds.Tables.Add(taComponentInfo);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;
    }

    public static DataSet ItemMasterInfo()
    {
      DataSet ds = new DataSet();
      DataTable dt = new DataTable("DataTable1");
      dt.Columns.Add("Title", typeof(System.String));
      dt.Columns.Add("SaleCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("ItemName", typeof(System.String));
      dt.Columns.Add("Description", typeof(System.String));
      dt.Columns.Add("Category", typeof(System.String));
      dt.Columns.Add("Collection", typeof(System.String));
      dt.Columns.Add("MainFinishing", typeof(System.String));
      dt.Columns.Add("KD", typeof(System.Int32));
      dt.Columns.Add("Image", typeof(System.Byte[]));
      dt.Columns.Add("Logo", typeof(System.Byte[]));
      ds.Tables.Add(dt);

      return ds;
    }

    public static DataSet PackingMaterialInfo()
    {
      DataSet ds = new DataSet();
      DataTable taPackingInfo = new DataTable("TblPackingInfo");
      taPackingInfo.Columns.Add("PackingStyle", typeof(System.String));
      taPackingInfo.Columns.Add("PackingCode", typeof(System.String));
      taPackingInfo.Columns.Add("EnglishName", typeof(System.String));
      taPackingInfo.Columns.Add("Unit", typeof(System.String));
      taPackingInfo.Columns.Add("CartonSize", typeof(System.String));
      taPackingInfo.Columns.Add("ItemPerBox", typeof(System.Int32));
      taPackingInfo.Columns.Add("BoxPerItem", typeof(System.Int32));
      taPackingInfo.Columns.Add("ItemReference", typeof(System.String));
      taPackingInfo.Columns.Add("Pid", typeof(System.Int64));
      ds.Tables.Add(taPackingInfo);
      DataTable taMaterialInfo = new DataTable();
      taMaterialInfo.Columns.Add("Pid", typeof(System.Int64));
      taMaterialInfo.Columns.Add("MaterialCode", typeof(System.String));
      taMaterialInfo.Columns.Add("EnglishName", typeof(System.String));
      taMaterialInfo.Columns.Add("VietnameseName", typeof(System.String));
      taMaterialInfo.Columns.Add("Unit", typeof(System.String));
      taMaterialInfo.Columns.Add("Qty", typeof(System.Double));
      taMaterialInfo.Columns.Add("Remarks", typeof(System.String));
      ds.Tables.Add(taMaterialInfo);
      ds.Relations.Add(new DataRelation("TblPackingInfo_TblMaterialInfo", taPackingInfo.Columns["Pid"], taMaterialInfo.Columns["Pid"], false));

      return ds;
    }

    public static DataSet ItemComPonentHadware()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("SaleCode", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      taItemInfo.Columns.Add("Name", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("Category", typeof(System.String));
      taItemInfo.Columns.Add("Collection", typeof(System.String));
      taItemInfo.Columns.Add("MainFinishing", typeof(System.String));
      taItemInfo.Columns.Add("KD", typeof(System.Int32));
      taItemInfo.Columns.Add("Image", typeof(System.Byte[]));
      ds.Tables.Add(taItemInfo);
      DataTable taHardwareList = new DataTable("TblHardwareList");
      taHardwareList.Columns.Add("ComponentCode", typeof(System.String));
      taHardwareList.Columns.Add("Description", typeof(System.String));
      taHardwareList.Columns.Add("Qty", typeof(System.Double));
      taHardwareList.Columns.Add("Unit", typeof(System.String));
      taHardwareList.Columns.Add("Length", typeof(System.Double));
      taHardwareList.Columns.Add("Width", typeof(System.Double));
      taHardwareList.Columns.Add("Thickness", typeof(System.Double));
      taHardwareList.Columns.Add("Image", typeof(System.Byte[]));
      taHardwareList.Columns.Add("ContractOut", typeof(System.Int32));
      taHardwareList.Columns.Add("Finishing", typeof(System.String));
      taHardwareList.Columns.Add("CompRevision", typeof(System.Int32));
      ds.Tables.Add(taHardwareList);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;
    }

    public static DataSet ItemComponentUpholstery()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("SaleCode", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      taItemInfo.Columns.Add("ItemName", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("Category", typeof(System.String));
      taItemInfo.Columns.Add("Collection", typeof(System.String));
      taItemInfo.Columns.Add("MainFinishing", typeof(System.String));
      taItemInfo.Columns.Add("KD", typeof(System.Int32));
      taItemInfo.Columns.Add("Image", typeof(System.Byte[]));
      ds.Tables.Add(taItemInfo);
      DataTable taUpholsteryList = new DataTable("TblUpholsteryList");
      taUpholsteryList.Columns.Add("Upholstery code", typeof(System.String));
      taUpholsteryList.Columns.Add("Description", typeof(System.String));
      taUpholsteryList.Columns.Add("Qty", typeof(System.Double));
      taUpholsteryList.Columns.Add("Unit", typeof(System.String));
      taUpholsteryList.Columns["Unit"].DefaultValue = "pcs";
      taUpholsteryList.Columns.Add("Remark", typeof(System.String));
      taUpholsteryList.Columns.Add("Specification", typeof(System.String));
      ds.Tables.Add(taUpholsteryList);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;

    }

    public static DataSet ItemInfoSupReportCartonBox()
    {
      DataSet ds = new DataSet();
      DataTable taItemSupReportCartonBox = new DataTable("dtItemSupReportCartonBox");
      taItemSupReportCartonBox.Columns.Add("BoxTypeCode", typeof(System.String));
      taItemSupReportCartonBox.Columns.Add("DimensionPid", typeof(System.Int32));
      taItemSupReportCartonBox.Columns.Add("Length", typeof(System.Double));
      taItemSupReportCartonBox.Columns.Add("Width", typeof(System.Double));
      taItemSupReportCartonBox.Columns.Add("Height", typeof(System.Double));
      taItemSupReportCartonBox.Columns.Add("Weight", typeof(System.Double));
      ds.Tables.Add(taItemSupReportCartonBox);

      return ds;
    }

    public static DataSet ItemInfoSupReportItemSize()
    {
      DataSet ds = new DataSet();
      DataTable taSubReportOtherSize = new DataTable("dtSubReportOtherSize");
      taSubReportOtherSize.Columns.Add("Type", typeof(System.String));
      taSubReportOtherSize.Columns.Add("mm", typeof(System.Int32));
      taSubReportOtherSize.Columns.Add("Inch", typeof(System.String));
      ds.Tables.Add(taSubReportOtherSize);

      return ds;
    }

    public static DataSet ItemSize()
    {
      DataSet ds = new DataSet();
      DataTable taItemSize = new DataTable("dtItemSize");
      taItemSize.Columns.Add("ItemCode", typeof(System.String));
      taItemSize.Columns.Add("Revision", typeof(System.Int32));
      taItemSize.Columns.Add("MMWidth", typeof(System.Double));
      taItemSize.Columns.Add("MMDepth", typeof(System.Double));
      taItemSize.Columns.Add("MMHight", typeof(System.Double));
      taItemSize.Columns.Add("InchWidth", typeof(System.String));
      taItemSize.Columns.Add("InchDepth", typeof(System.String));
      taItemSize.Columns.Add("InchHight", typeof(System.String));
      taItemSize.Columns.Add("QuantityBox", typeof(System.Int32));
      taItemSize.Columns.Add("QuantityItem", typeof(System.Int32));
      taItemSize.Columns.Add("TotalCBM", typeof(System.String));
      taItemSize.Columns.Add("RevisionRecord", typeof(System.String));
      taItemSize.Columns.Add("Relative", typeof(System.String));
      taItemSize.Columns.Add("PackageCode", typeof(System.String));
      ds.Tables.Add(taItemSize);

      return ds;
    }

    public static DataSet SupportMaterialInfo()
    {
      DataSet ds = new DataSet();
      DataTable taSupportInfo = new DataTable("TblSupportInfo");
      taSupportInfo.Columns.Add("SupCode", typeof(System.String));
      taSupportInfo.Columns.Add("EnglishName", typeof(System.String));
      taSupportInfo.Columns.Add("ItemReference", typeof(System.String));
      ds.Tables.Add(taSupportInfo);
      DataTable taSupportDetail = new DataTable("TblSupportDetail");
      taSupportDetail.Columns.Add("MaterialCode", typeof(System.String));
      taSupportDetail.Columns.Add("EnglishName", typeof(System.String));
      taSupportDetail.Columns.Add("VietnameseName", typeof(System.String));
      taSupportDetail.Columns.Add("Unit", typeof(System.String));
      taSupportDetail.Columns.Add("Qty", typeof(System.Double));
      taSupportDetail.Columns.Add("Remarks", typeof(System.String));
      ds.Tables.Add(taSupportDetail);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;

    }

    public static DataSet UpholsteryInfo()
    {
      DataSet ds = new DataSet();
      DataTable taUpholsteryInfo = new DataTable("TblUpholsteryInfo");
      taUpholsteryInfo.Columns.Add("UpholsteryCode", typeof(System.String));
      taUpholsteryInfo.Columns.Add("EnglishName", typeof(System.String));
      taUpholsteryInfo.Columns.Add("ItemReference", typeof(System.String));
      ds.Tables.Add(taUpholsteryInfo);
      DataTable taUpholsteryDetail = new DataTable("TblUpholsteryDetail");
      taUpholsteryDetail.Columns.Add("MaterialCode", typeof(System.String));
      taUpholsteryDetail.Columns.Add("EnglishName", typeof(System.String));
      taUpholsteryDetail.Columns.Add("VietnameseName", typeof(System.String));
      taUpholsteryDetail.Columns.Add("Unit", typeof(System.String));
      taUpholsteryDetail.Columns.Add("Qty", typeof(System.Double));
      taUpholsteryDetail.Columns.Add("Remarks", typeof(System.String));
      ds.Tables.Add(taUpholsteryDetail);
      DataTable taMasterInfo = new DataTable("TblMasterInfo");
      taMasterInfo.Columns.Add("LogoImage", typeof(System.Byte[]));
      taMasterInfo.Columns.Add("UserName", typeof(System.String));
      ds.Tables.Add(taMasterInfo);

      return ds;
    }

    public static DataSet BoxTypeInfo()
    {
      DataSet ds = new DataSet();
      DataTable taBoxType = new DataTable("dtBoxType");
      taBoxType.Columns.Add("Pid", typeof(System.Int64));
      taBoxType.Columns.Add("BoxTypePid", typeof(System.Int64));
      taBoxType.Columns.Add("BoxCode", typeof(System.String));
      taBoxType.Columns.Add("BoxName", typeof(System.String));
      taBoxType.Columns["BoxName"].AllowDBNull = false;
      taBoxType.Columns.Add("Width", typeof(System.Double));
      taBoxType.Columns["Width"].AllowDBNull = false;
      taBoxType.Columns.Add("Length", typeof(System.Double));
      taBoxType.Columns["Length"].AllowDBNull = false;
      taBoxType.Columns.Add("Height", typeof(System.Double));
      taBoxType.Columns["Height"].AllowDBNull = false;
      taBoxType.Columns.Add("No", typeof(System.Int32));
      taBoxType.Columns["No"].AllowDBNull = false;
      taBoxType.Columns.Add("GWeight", typeof(System.Double));
      taBoxType.Columns.Add("NWeight", typeof(System.Double));
      taBoxType.Columns.Add("Child", typeof(System.String));
      taBoxType.Columns["Child"].AllowDBNull = false;
      taBoxType.Columns["Child"].DefaultValue = 0;
      ds.Tables.Add(taBoxType);
      DataTable taBoxTypeDetail = new DataTable("dtBoxTypeDetail");
      taBoxTypeDetail.Columns.Add("Pid", typeof(System.Int64));
      taBoxTypeDetail.Columns.Add("BoxCode", typeof(System.String));
      taBoxTypeDetail.Columns.Add("BoxTypePid", typeof(System.Int64));
      taBoxTypeDetail.Columns.Add("BoxNo", typeof(System.Int32));
      taBoxTypeDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBoxTypeDetail.Columns["MaterialCode"].AllowDBNull = false;
      taBoxTypeDetail.Columns.Add("MaterialName", typeof(System.String));
      taBoxTypeDetail.Columns.Add("FactoryUnit", typeof(System.String));
      taBoxTypeDetail.Columns.Add("IDFactoryUnit", typeof(System.Int32));
      taBoxTypeDetail.Columns.Add("Qty", typeof(System.Double));
      taBoxTypeDetail.Columns.Add("RAW_Length", typeof(System.Double));
      taBoxTypeDetail.Columns.Add("RAW_Width", typeof(System.Double));
      taBoxTypeDetail.Columns.Add("RAW_Thickness", typeof(System.Double));
      taBoxTypeDetail.Columns.Add("TotalQty", typeof(System.Double));
      taBoxTypeDetail.Columns.Add("Alternative", typeof(System.String));
      taBoxTypeDetail.Columns.Add("Waste", typeof(System.Double));
      taBoxTypeDetail.Columns.Add("Child", typeof(System.String));
      taBoxTypeDetail.Columns["Child"].DefaultValue = 1;
      ds.Tables.Add(taBoxTypeDetail);

      DataTable taDirectLabour = new DataTable("dtDirectLabour");
      taDirectLabour.Columns.Add("SectionCode", typeof(System.String));
      taDirectLabour.Columns.Add("NameEN", typeof(System.String));
      taDirectLabour.Columns.Add("Qty", typeof(System.Double));
      taDirectLabour.Columns.Add("Description", typeof(System.String));
      taDirectLabour.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taDirectLabour);

      ds.Relations.Add(new DataRelation("dtBoxType_dtBoxTypeDetail", taBoxType.Columns["BoxCode"], taBoxTypeDetail.Columns["BoxCode"], false));
      return ds;
    }

    public static DataSet DirectLabourInfo()
    {
      DataSet ds = new DataSet();
      DataTable taDirectLabour = new DataTable("dtDirectLabour");
      taDirectLabour.Columns.Add("SectionCode", typeof(System.String));
      taDirectLabour.Columns["SectionCode"].AllowDBNull = false;
      taDirectLabour.Columns.Add("NameEN", typeof(System.String));
      taDirectLabour.Columns.Add("Qty", typeof(System.Double));
      taDirectLabour.Columns.Add("Description", typeof(System.String));
      taDirectLabour.Columns.Add("Remark", typeof(System.String));
      taDirectLabour.PrimaryKey = new DataColumn[] { taDirectLabour.Columns["SectionCode"] };
      ds.Tables.Add(taDirectLabour);
      return ds;
    }

    public static DataSet SupplementInfo()
    {
      DataSet ds = new DataSet();
      DataTable taSupp = new DataTable("dtSupp");
      taSupp.Columns.Add("Pid", typeof(System.Int64));
      taSupp.Columns.Add("MaterialCode", typeof(System.String));
      taSupp.Columns.Add("ControlType", typeof(System.Int32));
      taSupp.Columns.Add("MaterialNameEn", typeof(System.String));
      taSupp.Columns.Add("Unit", typeof(System.String));
      taSupp.Columns.Add("WoPid", typeof(System.Int64));
      taSupp.Columns.Add("ItemCode", typeof(System.String));
      taSupp.Columns.Add("Revision", typeof(System.Int32));
      taSupp.Columns.Add("Issued", typeof(System.Double));
      taSupp.Columns.Add("StockQty", typeof(System.Double));
      taSupp.Columns.Add("SuppQty", typeof(System.Double));
      taSupp.Columns.Add("Reason", typeof(System.Int32));
      ds.Tables.Add(taSupp);
      return ds;
    }

    public static DataSet CarcassInfo()
    {
      DataSet ds = new DataSet();
      DataTable taCarcassComponent = new DataTable("CarcassComponent");
      taCarcassComponent.Columns.Add("Pid", typeof(System.Int64));
      taCarcassComponent.Columns.Add("No", typeof(System.Int32));
      taCarcassComponent.Columns.Add("ComponentCode", typeof(System.String));
      taCarcassComponent.Columns["ComponentCode"].AllowDBNull = false;
      taCarcassComponent.Columns.Add("DescriptionVN", typeof(System.String));
      taCarcassComponent.Columns["DescriptionVN"].AllowDBNull = false;
      taCarcassComponent.Columns.Add("Qty", typeof(System.Double));
      taCarcassComponent.Columns["Qty"].AllowDBNull = false;
      taCarcassComponent.Columns.Add("FIN_Length", typeof(System.Double));
      taCarcassComponent.Columns.Add("FIN_Width", typeof(System.Double));
      taCarcassComponent.Columns.Add("FIN_Thickness", typeof(System.Double));
      taCarcassComponent.Columns.Add("Lamination", typeof(System.Int32));
      taCarcassComponent.Columns["Lamination"].AllowDBNull = false;
      taCarcassComponent.Columns["Lamination"].DefaultValue = 0;
      taCarcassComponent.Columns.Add("FingerJoin", typeof(System.Int32));
      taCarcassComponent.Columns["FingerJoin"].AllowDBNull = false;
      taCarcassComponent.Columns["FingerJoin"].DefaultValue = 0;
      taCarcassComponent.Columns.Add("Specify", typeof(System.Int32));
      taCarcassComponent.Columns.Add("Status", typeof(System.Int32));
      taCarcassComponent.Columns.Add("ContractOut", typeof(System.Int32));
      taCarcassComponent.Columns["ContractOut"].AllowDBNull = false;
      taCarcassComponent.Columns["ContractOut"].DefaultValue = 0;
      taCarcassComponent.Columns.Add("MainCop", typeof(System.Int32));
      taCarcassComponent.Columns["MainCop"].DefaultValue = 1;
      taCarcassComponent.Columns.Add("Primary", typeof(System.Int32));
      taCarcassComponent.Columns["Primary"].DefaultValue = 1;
      taCarcassComponent.Columns.Add("RowState", typeof(System.Int32));
      taCarcassComponent.Columns["RowState"].DefaultValue = 0;
      taCarcassComponent.Columns.Add("Child", typeof(System.Int32));
      taCarcassComponent.Columns["Child"].AllowDBNull = false;
      taCarcassComponent.Columns["Child"].DefaultValue = 0;
      taCarcassComponent.Columns.Add("Waste", typeof(System.Double));
      taCarcassComponent.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(taCarcassComponent);
      DataTable taCarcassComponentDetail = new DataTable("CarcassComponentDetail");
      taCarcassComponentDetail.Columns.Add("Pid", typeof(System.Int64));
      taCarcassComponentDetail.Columns.Add("ComponentCode", typeof(System.String));
      taCarcassComponentDetail.Columns["ComponentCode"].AllowDBNull = false;
      taCarcassComponentDetail.Columns.Add("MaterialCode", typeof(System.String));
      taCarcassComponentDetail.Columns["MaterialCode"].AllowDBNull = false;
      taCarcassComponentDetail.Columns.Add("MaterialName", typeof(System.String));
      taCarcassComponentDetail.Columns.Add("FactoryUnit", typeof(System.String));
      taCarcassComponentDetail.Columns.Add("QtyCombine", typeof(System.Double));
      taCarcassComponentDetail.Columns["QtyCombine"].AllowDBNull = false;
      taCarcassComponentDetail.Columns["QtyCombine"].DefaultValue = 1;
      taCarcassComponentDetail.Columns.Add("RAW_Length", typeof(System.Double));
      taCarcassComponentDetail.Columns.Add("RAW_Width", typeof(System.Double));
      taCarcassComponentDetail.Columns.Add("RAW_Thickness", typeof(System.Double));
      taCarcassComponentDetail.Columns.Add("Alternative", typeof(System.String));
      taCarcassComponentDetail.Columns.Add("RowState", typeof(System.Int32));
      taCarcassComponentDetail.Columns["RowState"].DefaultValue = 0;
      taCarcassComponentDetail.Columns.Add("Child", typeof(System.Int32));
      taCarcassComponentDetail.Columns["Child"].AllowDBNull = false;
      taCarcassComponentDetail.Columns["Child"].DefaultValue = 1;
      taCarcassComponentDetail.Columns.Add("IDFactoryUnit", typeof(System.Int32));
      taCarcassComponentDetail.Columns.Add("Waste", typeof(System.Double));
      ds.Tables.Add(taCarcassComponentDetail);
      ds.Relations.Add(new DataRelation("CarcassComponent_CarcassComponentDetail", taCarcassComponent.Columns["ComponentCode"], taCarcassComponentDetail.Columns["ComponentCode"], false));

      return ds;


    }

    public static DataSet FinBox()
    {
      DataSet ds = new DataSet();
      DataTable taPackage = new DataTable("dtPackage");
      taPackage.Columns.Add("No", typeof(System.Int64));
      taPackage.Columns.Add("SeriBox", typeof(System.String));
      taPackage.Columns.Add("BoxTypeCode", typeof(System.String));
      taPackage.Columns.Add("Length", typeof(System.Double));
      taPackage.Columns.Add("Width", typeof(System.Double));
      taPackage.Columns.Add("Height", typeof(System.Double));
      taPackage.Columns.Add("Set", typeof(System.Int32));
      ds.Tables.Add(taPackage);
      DataTable taSearchBox = new DataTable("dtSearchBox");
      taSearchBox.Columns.Add("SeriBox", typeof(System.String));
      taSearchBox.Columns.Add("BoxTypeCode", typeof(System.String));
      taSearchBox.Columns.Add("WorkOrder", typeof(System.String));
      taSearchBox.Columns.Add("Date", typeof(System.DateTime));
      taSearchBox.Columns.Add("BoxSet", typeof(System.Int32));
      taSearchBox.Columns.Add("Check", typeof(System.Int16));
      ds.Tables.Add(taSearchBox);
      DataTable taFinBox = new DataTable("dtFinBox");
      taFinBox.Columns.Add("SeriBox", typeof(System.String));
      taFinBox.Columns["SeriBox"].AllowDBNull = false;
      taFinBox.Columns.Add("BoxCode", typeof(System.String));
      taFinBox.Columns["BoxCode"].AllowDBNull = false;
      taFinBox.Columns.Add("WorkOrder", typeof(System.String));
      taFinBox.Columns.Add("Length", typeof(System.Double));
      taFinBox.Columns.Add("Width", typeof(System.Double));
      taFinBox.Columns.Add("Height", typeof(System.Double));
      taFinBox.Columns.Add("Set", typeof(System.Int32));
      taFinBox.Columns.Add("Check", typeof(System.Int16));
      taFinBox.Columns.Add("Pid", typeof(System.Int64));
      ds.Tables.Add(taFinBox);
      DataTable taListBox = new DataTable();
      taListBox.Columns.Add("ItemCode", typeof(System.String));
      taListBox.Columns.Add("Name", typeof(System.String));
      taListBox.Columns.Add("Revision", typeof(System.String));
      taListBox.Columns.Add("No", typeof(System.String));
      taListBox.Columns.Add("BoxPerItem", typeof(System.String));
      taListBox.Columns.Add("ItemPerBox", typeof(System.String));
      taListBox.Columns.Add("ItemDimension", typeof(System.String));
      taListBox.Columns.Add("GWeight", typeof(System.String));
      taListBox.Columns.Add("NWeight", typeof(System.String));
      taListBox.Columns.Add("BoxDimension", typeof(System.String));
      taListBox.Columns.Add("WorkOrder", typeof(System.String));
      taListBox.Columns.Add("BarCode", typeof(System.String));
      taListBox.Columns.Add("SaleCode", typeof(System.String));
      taListBox.Columns.Add("Image", typeof(System.Byte[]));
      taListBox.Columns.Add("Set", typeof(System.String));
      taListBox.Columns.Add("Image1", typeof(System.Byte[]));
      taListBox.Columns.Add("Length", typeof(System.Double));
      taListBox.Columns.Add("Width", typeof(System.Double));
      taListBox.Columns.Add("Height", typeof(System.Double));
      taListBox.Columns.Add("BoxTypeCode", typeof(System.String));
      taListBox.Columns["BoxTypeCode"].AllowDBNull = false;
      ds.Tables.Add(taListBox);

      return ds;
    }

    public static DataSet Finnishing()
    {
      DataSet ds = new DataSet();
      DataTable taBOMFinishingInfo = new DataTable("TblBOMFinishingInfo");
      taBOMFinishingInfo.Columns.Add("FinCode", typeof(System.String));
      taBOMFinishingInfo.Columns["FinCode"].AllowDBNull = false;
      taBOMFinishingInfo.Columns.Add("NameVN", typeof(System.String));
      taBOMFinishingInfo.Columns.Add("Name", typeof(System.String));
      taBOMFinishingInfo.Columns.Add("Waste", typeof(System.Double));
      taBOMFinishingInfo.Columns.Add("SheenLevel", typeof(System.String));
      taBOMFinishingInfo.Columns.Add("CreateDate", typeof(System.DateTime));
      taBOMFinishingInfo.Columns.Add("CreateBy", typeof(System.String));
      taBOMFinishingInfo.Columns.Add("Confirm", typeof(System.String));
      ds.Tables.Add(taBOMFinishingInfo);
      DataTable taBOMFinishingDetail = new DataTable("TblBOMFinishingDetail");
      taBOMFinishingDetail.Columns.Add("FinCode", typeof(System.String));
      taBOMFinishingDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBOMFinishingDetail.Columns.Add("MaterialName", typeof(System.String));
      taBOMFinishingDetail.Columns.Add("Qty", typeof(System.Double));
      taBOMFinishingDetail.Columns.Add("Unit", typeof(System.String));
      taBOMFinishingDetail.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taBOMFinishingDetail);
      ds.Relations.Add(new DataRelation("TblBOMFinishingInfo_TblBOMFinishingDetail", taBOMFinishingInfo.Columns["FinCode"], taBOMFinishingDetail.Columns["FinCode"], false));

      return ds;
    }

    public static DataSet GlassInfoReport()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("Confirm", typeof(System.Int32));
      taItemInfo.Columns.Add("CarcassCode", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      ds.Tables.Add(taItemInfo);
      DataTable taBOMGlassInfo = new DataTable();
      taBOMGlassInfo.Columns.Add("PID", typeof(System.Int64));
      taBOMGlassInfo.Columns.Add("ComponentCode", typeof(System.String));
      taBOMGlassInfo.Columns.Add("ComponentName", typeof(System.String));
      taBOMGlassInfo.Columns.Add("MaterialCode", typeof(System.String));
      taBOMGlassInfo.Columns.Add("Qty", typeof(System.Double));
      taBOMGlassInfo.Columns.Add("Waste", typeof(System.Double));
      taBOMGlassInfo.Columns.Add("TotalQty", typeof(System.Double));
      taBOMGlassInfo.Columns.Add("Length", typeof(System.Double));
      taBOMGlassInfo.Columns.Add("Width", typeof(System.Double));
      taBOMGlassInfo.Columns.Add("Thickness", typeof(System.Double));
      taBOMGlassInfo.Columns.Add("Adjective", typeof(System.String));
      taBOMGlassInfo.Columns.Add("Link", typeof(System.String));
      taBOMGlassInfo.Columns.Add("Remark", typeof(System.String));
      taBOMGlassInfo.Columns.Add("Types", typeof(System.String));
      taBOMGlassInfo.Columns.Add("Bevel", typeof(System.Int32));
      taBOMGlassInfo.Columns.Add("TP", typeof(System.Int32));
      taBOMGlassInfo.Columns.Add("GR", typeof(System.Int32));
      taBOMGlassInfo.Columns.Add("EG", typeof(System.Int32));
      taBOMGlassInfo.Columns.Add("DS", typeof(System.Int32));
      taBOMGlassInfo.Columns.Add("CU", typeof(System.Int32));
      taBOMGlassInfo.Columns.Add("CO", typeof(System.Int32));
      ds.Tables.Add(taBOMGlassInfo);

      return ds;
    }

    public static DataSet Handle()
    {
      DataSet ds = new DataSet();
      DataTable taBOMItemComponent = new DataTable("TblBOMItemComponent");
      taBOMItemComponent.Columns.Add("ComponentCode", typeof(System.String));
      taBOMItemComponent.Columns.Add("ComponentName", typeof(System.String));
      taBOMItemComponent.Columns.Add("Qty", typeof(System.Double));
      taBOMItemComponent.Columns.Add("Waste", typeof(System.Double));
      taBOMItemComponent.Columns.Add("Alternative", typeof(System.String));
      taBOMItemComponent.Columns.Add("Length", typeof(System.Double));
      taBOMItemComponent.Columns.Add("Width", typeof(System.Double));
      taBOMItemComponent.Columns.Add("Thickness", typeof(System.Double));
      taBOMItemComponent.Columns.Add("Link", typeof(System.String));
      taBOMItemComponent.Columns.Add("CompGroup", typeof(System.Int32));
      taBOMItemComponent.Columns.Add("ContractOut", typeof(System.Int32));
      taBOMItemComponent.Columns.Add("CompRevision", typeof(System.Int32));
      ds.Tables.Add(taBOMItemComponent);
      DataTable taBOMComponentInfoDetail = new DataTable("TblBOMComponentInfoDetail");
      taBOMComponentInfoDetail.Columns.Add("ComponentCode", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("MaterialName", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("Unit", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("Qty", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Length", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Width", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Thickness", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Waste", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Alternative", typeof(System.String));
      ds.Tables.Add(taBOMComponentInfoDetail);
      DataTable taBOMSupportDetail = new DataTable();
      taBOMSupportDetail.Columns.Add("SupCode", typeof(System.String));
      taBOMSupportDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBOMSupportDetail.Columns.Add("MaterialName", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Unit", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Qty", typeof(System.Double));
      taBOMSupportDetail.Columns.Add("Width", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Depth", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Height", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Waste", typeof(System.Double));
      taBOMSupportDetail.Columns.Add("Alternative", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taBOMSupportDetail);

      return ds;


    }

    public static DataSet HardwareInfoReport()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("Confirm", typeof(System.Int32));
      taItemInfo.Columns.Add("CarcassCode", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      ds.Tables.Add(taItemInfo);
      DataTable taBOMComponentInfo = new DataTable("TblBOMComponentInfo");
      taBOMComponentInfo.Columns.Add("PID", typeof(System.Int64));
      taBOMComponentInfo.Columns.Add("ComponentCode", typeof(System.String));
      taBOMComponentInfo.Columns["ComponentCode"].AllowDBNull = false;
      taBOMComponentInfo.Columns.Add("ComponentName", typeof(System.String));
      taBOMComponentInfo.Columns.Add("Revision", typeof(System.Int32));
      taBOMComponentInfo.Columns.Add("Length", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Width", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Thickness", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("ContractOut", typeof(System.Int32));
      taBOMComponentInfo.Columns.Add("Qty", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Waste", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("TotalQty", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Material_Alter", typeof(System.String));
      taBOMComponentInfo.Columns.Add("Comp_Alter", typeof(System.String));
      taBOMComponentInfo.Columns.Add("AlterRevision", typeof(System.Int32));
      ds.Tables.Add(taBOMComponentInfo);
      DataTable taBOMComponentInfoDetail = new DataTable();
      taBOMComponentInfoDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("Qty", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Waste", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("TotalQty", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Length", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Width", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Thickness", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Alternative", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("ComponentCode", typeof(System.String));
      ds.Tables.Add(taBOMComponentInfoDetail);

      return ds;
    }

    public static DataSet ItemComponent()
    {
      DataSet ds = new DataSet();
      DataTable taBOMComponentInfo = new DataTable("TblBOMComponentInfo");
      taBOMComponentInfo.Columns.Add("ComponentCode", typeof(System.String));
      taBOMComponentInfo.Columns["ComponentCode"].AllowDBNull = false;
      taBOMComponentInfo.Columns.Add("ComponentName", typeof(System.String));
      taBOMComponentInfo.Columns.Add("Qty", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Waste", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Material", typeof(System.String));
      taBOMComponentInfo.Columns.Add("Length", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Width", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Thickness", typeof(System.Double));
      taBOMComponentInfo.Columns.Add("Link", typeof(System.String));
      taBOMComponentInfo.Columns.Add("CompGroup", typeof(System.String));
      taBOMComponentInfo.Columns.Add("ContractOut", typeof(System.Int32));
      taBOMComponentInfo.Columns.Add("Pid", typeof(System.Int64));
      taBOMComponentInfo.Columns.Add("Confirm", typeof(System.String));
      ds.Tables.Add(taBOMComponentInfo);
      DataTable taBOMComponentInfoDetail = new DataTable("TblBOMComponentInfoDetail");
      taBOMComponentInfoDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("Qty", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Waste", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Length", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Width", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Thickness", typeof(System.Double));
      taBOMComponentInfoDetail.Columns.Add("Alternative", typeof(System.String));
      taBOMComponentInfoDetail.Columns.Add("ComponentPid", typeof(System.Int64));
      ds.Tables.Add(taBOMComponentInfoDetail);
      ds.Relations.Add(new DataRelation("TblBOMComponentInfo_TblBOMComponentInfoDetail", taBOMComponentInfo.Columns["Pid"], taBOMComponentInfoDetail.Columns["ComponentPid"], false));

      return ds;
    }

    public static DataSet LabourInfoReport()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("Confirm", typeof(System.Int32));
      taItemInfo.Columns.Add("CarcassCode", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      ds.Tables.Add(taItemInfo);
      DataTable taLabourInfo = new DataTable("TblLabourInfo");
      taLabourInfo.Columns.Add("SectionCode", typeof(System.String));
      taLabourInfo.Columns.Add("NameEN", typeof(System.String));
      taLabourInfo.Columns.Add("Qty", typeof(System.Double));
      taLabourInfo.Columns.Add("Description", typeof(System.String));
      taLabourInfo.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taLabourInfo);

      return ds;


    }

    public static DataSet Machine()
    {
      DataSet ds = new DataSet();
      DataTable taBOMMachine = new DataTable("TblBOMMachine");
      taBOMMachine.Columns.Add("Pid", typeof(System.Int64));
      taBOMMachine.Columns.Add("NameEn", typeof(System.String));
      taBOMMachine.Columns.Add("NameVn", typeof(System.String));
      taBOMMachine.Columns.Add("EmpName", typeof(System.String));
      taBOMMachine.Columns.Add("CreateDate", typeof(System.DateTime));
      taBOMMachine.Columns.Add("DeleteFlag", typeof(System.Int32));
      ds.Tables.Add(taBOMMachine);
      DataTable taBOMMachineDetails = new DataTable("TblBOMMachineDetails");
      taBOMMachineDetails.Columns.Add("MachinePid", typeof(System.Int64));
      taBOMMachineDetails.Columns.Add("MachineCode", typeof(System.String));
      taBOMMachineDetails.Columns.Add("MachineNameEN", typeof(System.String));
      taBOMMachineDetails.Columns.Add("MachineNameVN", typeof(System.String));
      ds.Tables.Add(taBOMMachineDetails);
      ds.Relations.Add(new DataRelation("TblBOMMachine_TblBOMMachineDetails", taBOMMachine.Columns["Pid"], taBOMMachineDetails.Columns["MachinePid"], false));

      return ds;

    }

    public static DataSet MasterProcessInfo()
    {
      DataSet ds = new DataSet();
      DataTable taMasterProcessInfo = new DataTable("TblMasterProcessInfo");
      taMasterProcessInfo.Columns.Add("Pid", typeof(System.Int32));
      taMasterProcessInfo.Columns.Add("Ordinal", typeof(System.Int32));
      taMasterProcessInfo.Columns["Ordinal"].AllowDBNull = false;
      taMasterProcessInfo.Columns.Add("DescriptionVN", typeof(System.String));
      taMasterProcessInfo.Columns.Add("Description", typeof(System.String));
      taMasterProcessInfo.Columns["Description"].AllowDBNull = false;
      taMasterProcessInfo.Columns.Add("WorkStation", typeof(System.Int64));
      taMasterProcessInfo.Columns.Add("WorkAreaName", typeof(System.String));
      taMasterProcessInfo.Columns.Add("TeamCode", typeof(System.String));
      taMasterProcessInfo.Columns.Add("TeamName", typeof(System.String));
      taMasterProcessInfo.Columns.Add("MachineGroupPid", typeof(System.Int64));
      taMasterProcessInfo.Columns.Add("Profile", typeof(System.String));
      taMasterProcessInfo.Columns.Add("ProfilePid", typeof(System.String));
      ds.Tables.Add(taMasterProcessInfo);
      DataTable taProfileInfo = new DataTable("TblProfileInfo");
      taProfileInfo.Columns.Add("ProcessPid", typeof(System.Int32));
      taProfileInfo.Columns.Add("ProfilePid", typeof(System.Int64));
      taProfileInfo.Columns.Add("ProfileCode", typeof(System.String));
      taProfileInfo.Columns["ProfileCode"].AllowDBNull = false;
      taProfileInfo.Columns.Add("Description", typeof(System.String));
      ds.Tables.Add(taProfileInfo);
      ds.Relations.Add(new DataRelation("TblMasterProcessInfo_TblProfileInfo", taMasterProcessInfo.Columns["Pid"], taProfileInfo.Columns["ProcessPid"], false));

      return ds;
    }

    public static DataSet TabComponent()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblBOMComponentInfo");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("ComponentCode", typeof(System.String));
      taParent.Columns.Add("ComponentName", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("Waste", typeof(System.Double));
      taParent.Columns.Add("TotalQty", typeof(System.Double));
      taParent.Columns.Add("WorkAreaPid", typeof(System.Int64));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Length", typeof(System.Double));
      taParent.Columns.Add("Width", typeof(System.Double));
      taParent.Columns.Add("Thickness", typeof(System.Double));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("ContractOut", typeof(System.Int32));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("TblBOMComponentInfoDetail");
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialName", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("Waste", typeof(System.Double));
      taChild.Columns.Add("TotalQty", typeof(System.Double));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("Thickness", typeof(System.Double));
      taChild.Columns.Add("Alternative", typeof(System.String));
      taChild.Columns.Add("ComponentCode", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblBOMComponentInfo_TblBOMComponentInfoDetail", taParent.Columns["ComponentCode"], taChild.Columns["ComponentCode"], false));
      return ds;
    }

    public static DataSet WoodCarcass()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblItemInfo");
      taParent.Columns.Add("Confirm", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("Description", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblBOMCarcassComponent");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("ComponentCode", typeof(System.String));
      taChild.Columns.Add("Description", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("Waste", typeof(System.Double));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("Thickness", typeof(System.Double));
      taChild.Columns.Add("Lamination", typeof(System.Int32));
      taChild.Columns.Add("FingerJoin", typeof(System.Int32));
      taChild.Columns.Add("Specify", typeof(System.String));
      taChild.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taChild);
      return ds;
    }

    public static DataSet ProfileTool()
    {
      DataSet ds = new DataSet();
      DataTable taBOMProfile = new DataTable("TblBOMProfile");
      taBOMProfile.Columns.Add("Pid", typeof(System.Int64));
      taBOMProfile.Columns.Add("ProfileCode", typeof(System.String));
      taBOMProfile.Columns.Add("Description", typeof(System.String));
      taBOMProfile.Columns.Add("DescriptionVN", typeof(System.String));
      taBOMProfile.Columns.Add("CreateBy", typeof(System.String));
      taBOMProfile.Columns.Add("CreateDate", typeof(System.DateTime));
      taBOMProfile.Columns.Add("Confirm", typeof(System.String));
      ds.Tables.Add(taBOMProfile);
      DataTable taBOMProfileDetail = new DataTable("TblBOMProfileDetail");
      taBOMProfileDetail.Columns.Add("ProfilePid", typeof(System.Int64));
      taBOMProfileDetail.Columns.Add("ProfileCode", typeof(System.String));
      taBOMProfileDetail.Columns.Add("ToolCode", typeof(System.String));
      taBOMProfileDetail.Columns.Add("Ordinal", typeof(System.Int32));
      taBOMProfileDetail.Columns.Add("Description", typeof(System.String));
      taBOMProfileDetail.Columns.Add("DescriptionVN", typeof(System.String));
      taBOMProfileDetail.Columns.Add("Machine", typeof(System.String));
      ds.Tables.Add(taBOMProfileDetail);
      ds.Relations.Add(new DataRelation("TblBOMProfile_TblBOMProfileDetail", taBOMProfile.Columns["Pid"], taBOMProfileDetail.Columns["ProfilePid"], false));

      return ds;
    }

    public static DataSet RoutingTicket()
    {
      DataSet ds = new DataSet();
      DataTable taProcessInfo = new DataTable("tblProcessInfo");
      taProcessInfo.Columns.Add("Pid", typeof(System.Int64));
      taProcessInfo.Columns.Add("Ordinal", typeof(System.Int32));
      taProcessInfo.Columns.Add("ProcessPid", typeof(System.Int64));
      taProcessInfo.Columns.Add("ENDescription", typeof(System.String));
      taProcessInfo.Columns.Add("VNDescription", typeof(System.String));
      taProcessInfo.Columns.Add("MachineGroup", typeof(System.String));
      taProcessInfo.Columns.Add("ENMoreDescription", typeof(System.String));
      taProcessInfo.Columns.Add("VNMoreDescription", typeof(System.String));
      taProcessInfo.Columns.Add("Profile", typeof(System.String));
      taProcessInfo.Columns.Add("ProfilePid", typeof(System.String));
      taProcessInfo.Columns.Add("ChangeProfile", typeof(System.Int32));
      ds.Tables.Add(taProcessInfo);
      DataTable taProfile = new DataTable("tblProfile");
      taProfile.Columns.Add("ProcessPid", typeof(System.Int64));
      taProfile.Columns.Add("ProfilePid", typeof(System.Int64));
      taProfile.Columns.Add("ProfileCode", typeof(System.String));
      taProfile.Columns["ProfileCode"].AllowDBNull = false;
      taProfile.Columns.Add("Description", typeof(System.String));
      ds.Tables.Add(taProfile);
      ds.Relations.Add(new DataRelation("tblProcessInfo_tblProfile", taProcessInfo.Columns["Pid"], taProfile.Columns["ProcessPid"], false));

      return ds;
    }


    public static DataSet Support()
    {
      DataSet ds = new DataSet();
      DataTable taBOMSupportInfo = new DataTable("TblBOMSupportInfo");
      taBOMSupportInfo.Columns.Add("SupCode", typeof(System.String));
      taBOMSupportInfo.Columns.Add("Description", typeof(System.String));
      taBOMSupportInfo.Columns.Add("DescriptionVN", typeof(System.String));
      taBOMSupportInfo.Columns.Add("CreateDate", typeof(System.DateTime));
      taBOMSupportInfo.Columns.Add("CreateBy", typeof(System.String));
      ds.Tables.Add(taBOMSupportInfo);
      DataTable taBOMSupportDetail = new DataTable("TblBOMSupportDetail");
      taBOMSupportDetail.Columns.Add("SupCode", typeof(System.String));
      taBOMSupportDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Qty", typeof(System.Double));
      taBOMSupportDetail.Columns.Add("Unit", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Depth", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Width", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Height", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Waste", typeof(System.Double));
      taBOMSupportDetail.Columns.Add("Remark", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Alternative", typeof(System.String));
      ds.Tables.Add(taBOMSupportDetail);
      ds.Relations.Add(new DataRelation("TblBOMSupportInfo_TblBOMSupportDetail", taBOMSupportInfo.Columns["SupCode"], taBOMSupportDetail.Columns["SupCode"], false));

      return ds;

    }

    public static DataSet TabCarcass()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblBOMCarcassComponent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("ComponentCode", typeof(System.String));
      taParent.Columns.Add("Description", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("Length", typeof(System.Double));
      taParent.Columns.Add("Width", typeof(System.Double));
      taParent.Columns.Add("Thickness", typeof(System.Double));
      taParent.Columns.Add("Lamination", typeof(System.Int32));
      taParent.Columns.Add("FingerJoin", typeof(System.Int32));
      taParent.Columns.Add("Specify", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("ContractOut", typeof(System.Int32));
      taParent.Columns.Add("Child", typeof(System.Int32));
      taParent.Columns["Child"].DefaultValue = 0;
      taParent.Columns.Add("Primary", typeof(System.Int32));
      taParent.Columns.Add("Waste", typeof(System.Double));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblBOMCarcassComponentProcess");
      taChild.Columns.Add("ComponentPid", typeof(System.Int64));
      taChild.Columns.Add("ProcessID", typeof(System.Int32));
      taChild.Columns.Add("Description", typeof(System.String));
      taChild.Columns.Add("DescriptionVN", typeof(System.String));
      taChild.Columns.Add("WorkStation", typeof(System.String));
      taChild.Columns.Add("Profile", typeof(System.String));
      taChild.Columns.Add("Child", typeof(System.Int32));
      taChild.Columns["Child"].DefaultValue = 1;
      ds.Tables.Add(taChild);

      DataTable taChild1 = new DataTable("TblBOMCarcassComponentDetail");
      taChild1.Columns.Add("ComponentPid", typeof(System.Int64));
      taChild1.Columns.Add("MaterialCode", typeof(System.String));
      taChild1.Columns.Add("MaterialName", typeof(System.String));
      taChild1.Columns.Add("Unit", typeof(System.String));
      taChild1.Columns.Add("QtyCombine", typeof(System.Double));
      taChild1.Columns.Add("RAW_Length", typeof(System.Double));
      taChild1.Columns.Add("RAW_Width", typeof(System.Double));
      taChild1.Columns.Add("RAW_Thickness", typeof(System.Double));
      taChild1.Columns.Add("Alternative", typeof(System.String));
      taChild1.Columns.Add("Child", typeof(System.Int32));
      taChild1.Columns["Child"].DefaultValue = 1;
      taChild1.Columns.Add("Waste", typeof(System.Double));
      ds.Tables.Add(taChild1);

      ds.Relations.Add(new DataRelation("TblBOMCarcassComponent_TblBOMCarcassComponentProcess", taParent.Columns["Pid"], taChild.Columns["ComponentPid"], false));
      ds.Relations.Add(new DataRelation("TblBOMCarcassComponent_TblBOMCarcassComponentDetail", taParent.Columns["Pid"], taChild1.Columns["ComponentPid"], false));
      return ds;
    }

    public static DataSet SupportInfoReport()
    {
      DataSet ds = new DataSet();
      DataTable taItemInfo = new DataTable("TblItemInfo");
      taItemInfo.Columns.Add("Confirm", typeof(System.Int32));
      taItemInfo.Columns.Add("CarcassCode", typeof(System.String));
      taItemInfo.Columns.Add("Description", typeof(System.String));
      taItemInfo.Columns.Add("ItemCode", typeof(System.String));
      taItemInfo.Columns.Add("Revision", typeof(System.Int32));
      taItemInfo.Columns.Add("SupCode", typeof(System.String));
      taItemInfo.Columns.Add("SupDescription", typeof(System.String));
      ds.Tables.Add(taItemInfo);
      DataTable taBOMSupportDetail = new DataTable("TblBOMSupportDetail");
      taBOMSupportDetail.Columns.Add("Pid", typeof(System.Int64));
      taBOMSupportDetail.Columns.Add("MaterialCode", typeof(System.String));
      taBOMSupportDetail.Columns.Add("MaterialName", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Unit", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Qty", typeof(System.Double));
      taBOMSupportDetail.Columns.Add("Width", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Depth", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Height", typeof(System.Int32));
      taBOMSupportDetail.Columns.Add("Waste", typeof(System.Double));
      taBOMSupportDetail.Columns.Add("Remark", typeof(System.String));
      taBOMSupportDetail.Columns.Add("Alternative", typeof(System.String));
      ds.Tables.Add(taBOMSupportDetail);
      return ds;

    }

    public static DataSet ListSupplement()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblWO");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("SupplementNo", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblWODetail");
      taChild.Columns.Add("SupplementPid", typeof(System.Int64));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialNameEn", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("WoPid", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Issued", typeof(System.Double));
      taChild.Columns.Add("Supplement", typeof(System.Double));
      taChild.Columns.Add("Reason", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblWO_TblWODetail", taParent.Columns["Pid"], taChild.Columns["SupplementPid"], false));
      return ds;
    }
    // Add HangNguyen 09-03-2011
    public static DataSet Supplier_Contact()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("SupplierCode", typeof(System.String));
      taParent.Columns.Add("EnglishName", typeof(System.String));
      taParent.Columns.Add("VietnameseName", typeof(System.String));
      taParent.Columns.Add("TradeName", typeof(System.String));
      taParent.Columns.Add("TradeType", typeof(System.String));
      taParent.Columns.Add("TradeCommodity", typeof(System.String));
      taParent.Columns.Add("PersonInChange", typeof(System.String));
      taParent.Columns.Add("IntroducePerson", typeof(System.String));
      taParent.Columns.Add("Confirm", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("SupplierID", typeof(System.Int64));
      taChild.Columns.Add("ContactName", typeof(System.String));
      taChild.Columns.Add("ContactPostion", typeof(System.String));
      taChild.Columns.Add("ContactMobile", typeof(System.String));
      taChild.Columns.Add("ContactEmail", typeof(System.String));
      taChild.Columns.Add("Sex", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["SupplierID"], false));
      return ds;
    }
    // End Add HangNguyen

    public static DataSet ListPR()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblPR");
      taParent.Columns.Add("Selected", typeof(System.Int32));
      taParent.Columns.Add("PRNo", typeof(System.String));
      taParent.Columns.Add("DeparmentName", typeof(System.String));
      taParent.Columns.Add("HeadDepartmentApproved", typeof(System.String));
      taParent.Columns.Add("RequestBy", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("TypeOfRequest", typeof(System.String));
      taParent.Columns.Add("PurposeOfRequisition", typeof(System.String));
      taParent.Columns.Add("TotalAmountValue", typeof(System.Double));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblPRDetail");
      taChild.Columns.Add("PRNo", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("NameEN", typeof(System.String));
      taChild.Columns.Add("Status", typeof(System.String));
      taChild.Columns.Add("Quantity", typeof(System.Double));
      taChild.Columns.Add("Price", typeof(System.Double));
      taChild.Columns.Add("Currency", typeof(System.String));
      taChild.Columns.Add("Urgent", typeof(System.String));
      taChild.Columns.Add("RequestDate", typeof(System.String));
      taChild.Columns.Add("ExpectedBrand", typeof(System.String));
      taChild.Columns.Add("VAT", typeof(System.String));
      taChild.Columns.Add("Imported", typeof(System.String));
      taChild.Columns.Add("ProjectCode", typeof(System.String));
      taChild.Columns.Add("GroupInCharge", typeof(System.Int64));
      taChild.Columns.Add("GroupName", typeof(System.String));
      taChild.Columns.Add("ConfirmBy", typeof(System.String));
      taChild.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblPR_TblPRDetail", taParent.Columns["PRNo"], taChild.Columns["PRNo"], false));
      return ds;
    }

    public static DataSet ListPO()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblPO");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("PidLink", typeof(System.Int64));
      taParent.Columns.Add("PRDTPid", typeof(System.Int64));
      taParent.Columns.Add("PRNo", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("RequestDate", typeof(System.String));
      taParent.Columns.Add("Quantity", typeof(System.Double));
      taParent.Columns.Add("Currency", typeof(System.Int64));
      taParent.Columns.Add("Price", typeof(System.Double));
      taParent.Columns.Add("VAT", typeof(System.Double));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("TblPODetail");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PODetatlPid", typeof(System.Int64));
      taChild.Columns.Add("PRDTPid", typeof(System.Int64));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("ReceiptedQty", typeof(System.Double));
      taChild.Columns.Add("ExpectDate", typeof(System.DateTime));
      taChild.Columns.Add("ConfirmExpectDate", typeof(System.DateTime));
      taChild.Columns.Add("LatestDeliveryDate", typeof(System.DateTime));
      taChild.Columns.Add("ContractNo", typeof(System.String));
      taChild.Columns.Add("NameOfGoods", typeof(System.String));
      taChild.Columns.Add("InvoiceNo", typeof(System.String));
      taChild.Columns.Add("ETD1", typeof(System.Int32));
      taChild.Columns.Add("ETD2", typeof(System.DateTime));
      taChild.Columns.Add("ETA", typeof(System.DateTime));
      taChild.Columns.Add("TimeOfReceivingDoc", typeof(System.DateTime));
      taChild.Columns.Add("TimeOfReceivingOriginal", typeof(System.DateTime));
      taChild.Columns.Add("ArrivalTimeToPort", typeof(System.DateTime));
      taChild.Columns.Add("BLDate", typeof(System.DateTime));
      taChild.Columns.Add("DocumentAt", typeof(System.String));
      ds.Tables.Add(taChild);
      ds.Relations.Add(new DataRelation("TblPO_TblPODetail", new DataColumn[] { taParent.Columns["PidLink"], taParent.Columns["PRDTPid"] }, new DataColumn[] { taChild.Columns["PODetatlPid"], taChild.Columns["PRDTPid"] }));
      return ds;
    }

    public static DataSet ListPONo()
    {
      DataSet ds = new DataSet();

      // PO Information
      DataTable taParent = new DataTable("TblPO");
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("SupplierName", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("TotalMoney", typeof(System.Double));
      taParent.Columns.Add("ApprovedBy", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.DateTime));
      ds.Tables.Add(taParent);

      // PO Detail
      DataTable taChild = new DataTable("TblPODetail");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PONo", typeof(System.String));
      taChild.Columns.Add("PRNo", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialName", typeof(System.String));
      taChild.Columns.Add("Status", typeof(System.String));
      taChild.Columns.Add("Quantity", typeof(System.Double));
      taChild.Columns.Add("Price", typeof(System.Double));
      taChild.Columns.Add("Currency", typeof(System.String));
      taChild.Columns.Add("VAT", typeof(System.String));
      taChild.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taChild);

      //PO Detail Schedule
      DataTable taSchedule = new DataTable("TblPODetailSchedule");
      taSchedule.Columns.Add("Pid", typeof(System.Int64));
      taSchedule.Columns.Add("PODetailPid", typeof(System.Int64));
      taSchedule.Columns.Add("Quantity", typeof(System.Double));
      taSchedule.Columns.Add("ReceiptedQty", typeof(System.Double));
      taSchedule.Columns.Add("ExpectDate", typeof(System.DateTime));
      taSchedule.Columns.Add("ConfirmExpectDate", typeof(System.DateTime));
      taSchedule.Columns.Add("DeliveryDate", typeof(System.DateTime));
      taSchedule.Columns.Add("ContractNo", typeof(System.String));
      taSchedule.Columns.Add("NameOfGoods", typeof(System.String));
      taSchedule.Columns.Add("InvoiceNo", typeof(System.String));
      taSchedule.Columns.Add("TypeOfETD", typeof(System.String));
      taSchedule.Columns.Add("ETD2", typeof(System.DateTime));
      taSchedule.Columns.Add("ETA", typeof(System.DateTime));
      taSchedule.Columns.Add("TimeOfReceivingDoc", typeof(System.DateTime));
      taSchedule.Columns.Add("TimeOfReceivingOriginal", typeof(System.DateTime));
      taSchedule.Columns.Add("ArrivalTimeToPort", typeof(System.DateTime));
      taSchedule.Columns.Add("BLDate", typeof(System.DateTime));
      taSchedule.Columns.Add("DocumentAt", typeof(System.String));
      ds.Tables.Add(taSchedule);

      ds.Relations.Add(new DataRelation("TblPO_TblPODetail", taParent.Columns["PONo"], taChild.Columns["PONo"]));
      ds.Relations.Add(new DataRelation("TblPODetail_TblPODetailSchedule", taChild.Columns["Pid"], taSchedule.Columns["PODetailPid"]));
      return ds;
    }

    /// <summary>
    /// ItemCode Allocation
    /// </summary>
    /// <returns></returns>
    public static DataSet ListItemAllocate()
    {
      DataSet ds = new DataSet();

      // ItemCode
      DataTable taParent = new DataTable("TblItemCode");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Name", typeof(System.String));
      taParent.Columns.Add("PCS/Box", typeof(System.String));
      taParent.Columns.Add("QtyItems", typeof(System.Int32));
      taParent.Columns.Add("QtyBoxes", typeof(System.Int32));
      taParent.Columns.Add("QtyAllocate", typeof(System.Int32));
      taParent.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(taParent);

      // BoxCode
      DataTable taChild = new DataTable("TblBoxId");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("SeriBoxNo", typeof(System.String));
      taChild.Columns.Add("BoxCode", typeof(System.String));
      taChild.Columns.Add("BoxName", typeof(System.String));
      taChild.Columns.Add("FurnitureCode", typeof(System.String));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("Set", typeof(System.String));
      taChild.Columns.Add("DateInStore", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblItemCode_TblBoxId", taParent.Columns["Pid"], taChild.Columns["Pid"], false));
      return ds;
    }

    /// <summary>
    /// List Receiving Veneer
    /// </summary>
    /// <returns></returns>
    public static DataSet ListReceivingVeneer()
    {
      DataSet ds = new DataSet();

      // Parent
      DataTable taParent = new DataTable("TblReceivingVeneer");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("ReceivingCode", typeof(System.String));
      taParent.Columns.Add("Title", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Source", typeof(System.String));
      taParent.Columns.Add("ApprovedPerson", typeof(System.String));
      taParent.Columns.Add("Type", typeof(System.Int32));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("Print", typeof(System.Int32));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("TblReceivingDetailVeneer");
      taChild.Columns.Add("ReceivingNotePid", typeof(System.Int64));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("Location", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblReceivingVeneer_TblReceivingDetailVeneer", taParent.Columns["PID"], taChild.Columns["ReceivingNotePid"], false));
      return ds;
    }

    /// <summary>
    /// List Receiving Veneer
    /// </summary>
    /// <returns></returns>
    public static DataSet ListIssuingVeneer()
    {
      DataSet ds = new DataSet();

      // Parent
      DataTable taParent = new DataTable("TblIssuingVeneer");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("IssuingCode", typeof(System.String));
      taParent.Columns.Add("Title", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Source", typeof(System.String));
      taParent.Columns.Add("ApprovedPerson", typeof(System.String));
      taParent.Columns.Add("Type", typeof(System.Int32));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("Print", typeof(System.Int32));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("TblIssuingDetailVeneer");
      taChild.Columns.Add("IssuingNotePid", typeof(System.Int64));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("Location", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblIssuingVeneer_TblIssuingDetailVeneer", taParent.Columns["PID"], taChild.Columns["IssuingNotePid"], false));
      return ds;
    }

    /// <summary>
    /// List Tran Location Veneer
    /// </summary>
    /// <returns></returns>
    public static DataSet ListTranLocationVeneer()
    {
      DataSet ds = new DataSet();

      // Parent
      DataTable taParent = new DataTable("TblTranLocationVeneer");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("TrNo", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("TblTranLocationDetailVeneer");
      taChild.Columns.Add("TrNoPid", typeof(System.Int64));
      taChild.Columns.Add("LotNoId", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("Length", typeof(System.Double));
      taChild.Columns.Add("Width", typeof(System.Double));
      taChild.Columns.Add("TotalCBM", typeof(System.Double));
      taChild.Columns.Add("LocationFrom", typeof(System.String));
      taChild.Columns.Add("LocationTo", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblTranLocationVeneer_TblTranLocationDetailVeneer", taParent.Columns["PID"], taChild.Columns["TrNoPid"], false));
      return ds;
    }
  }
}
