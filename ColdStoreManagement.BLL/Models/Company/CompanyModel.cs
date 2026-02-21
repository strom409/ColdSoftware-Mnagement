using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColdStoreManagement.BLL.Models.Company
{
    public class CompanyModel
    {

        [DisplayFormat(DataFormatString = "{0:000.000}", ApplyFormatInEditMode = true)]

        public int Mid { get; set; }
        public int? UserId { get; set; } = 1;

        public int? DockPost { get; set; }
        public string? Ratetype { get; set; }


        public int? Id { get; set; }
        public string? MaccGrp { get; set; }
        public bool? MacStatus { get; set; }
        public DateTime? MacCreatedDate { get; set; }
        public DateTime? draftdatefrom { get; set; } = new DateTime(2025, 10, 1);
        public DateTime? draftdateto { get; set; }
        public string? MaccDetails { get; set; }
        public string? DraftRemarks { get; set; }
        public DateTime? calendardate { get; set; }
        public DateTime? calendartime { get; set; }


        public int Pid { get; set; }
        public string? PurchGrp { get; set; }
        public bool? PurchStatus { get; set; }
        public DateTime? PurchCreatedDate { get; set; }

        public string? PurchDetails { get; set; }


        public int Itemid { get; set; }
        public string? PurchaseItemName { get; set; }
        public bool? ItemStatus { get; set; }
        public DateTime? ItemCreatedDate { get; set; }

        public string? ItemGst { get; set; }

        public string? ItemUom { get; set; }
        public string? ItemHsn { get; set; }
        public string? Vehntype { get; set; }
        public bool? VehStatus { get; set; }


        public string? Scabename { get; set; }




        public int SubAccid { get; set; }
        public string? SubaccGrp { get; set; }
        public bool? SubAccStatus { get; set; }
        public DateTime? SubaccCreatedDate { get; set; }

        public string? SubaccDetails { get; set; }
        public DateTime? Slotime { get; set; }

        public int Slotno { get; set; }

        public int SlotQty { get; set; }

        public int SubGroupId { get; set; }
        public string? SubgroupName { get; set; }
        public bool? SubGroupStatus { get; set; }
        public DateTime? SubGroupCreated { get; set; }

        public string? SubGroupDetails { get; set; }




        public string? OldPassword { get; set; }






















        public int? Msid { get; set; }
        public string? MsGrp { get; set; }

        public string? MsGdetails { get; set; }

        public int? Sid { get; set; }
        public string? Subgroup { get; set; }

        public string? Subgdetails { get; set; }





        public string? Name { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string? Gst { get; set; }
        public string? Phone { get; set; }
        public string? Regno { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }

        public string? Pan { get; set; }
        public string? Accno { get; set; }


        public int? Bid { get; set; }



        public string? Bname { get; set; }

        //[Required(ErrorMessage = "Address is required")]
        public string? Caddress { get; set; }


        public string? Branch { get; set; }


        public string? Ifsc { get; set; }

        public string? Accname { get; set; }



        public int? Uid { get; set; }



        public string? Unitname { get; set; }

        //[Required(ErrorMessage = "Address is required")]
        public string? Ucode { get; set; }
        public string? Ustat { get; set; }
        public string? Unitdetails { get; set; }



        public int? Buid { get; set; }



        public string? Buildname { get; set; }

        //[Required(ErrorMessage = "Address is required")]
        public string? Bucode { get; set; }
        public string? Bustat { get; set; }
        public string? Budetails { get; set; }



        public int Prodid { get; set; }
        public string? Prodname { get; set; }
        public string? Prodetails { get; set; }



        public int Qid { get; set; }
        public string? Qname { get; set; }
        public string? Qdetails { get; set; }



        public int Pkid { get; set; }
        public string? Pkname { get; set; }
        public string? Pkdetails { get; set; }

        public int Skid { get; set; }
        public string? Stname { get; set; }
        public string? Stdetails { get; set; }

        public int Crid { get; set; }
        public string? Crname { get; set; }
        public decimal Crqty { get; set; }

        public int Cfid { get; set; }
        public string? Cfname { get; set; }
        public string? Cfstat { get; set; }

        public string? Cflag { get; set; }


        public int chid { get; set; }
        public string? Chname { get; set; }
        public string? Chtype { get; set; }

        public string? Chbcode { get; set; }
        public string? Chformat { get; set; }

        public decimal Floorn { get; set; }

        public decimal Cft { get; set; }
        public decimal Asize { get; set; }

        public decimal Usize { get; set; }
        public decimal Sft { get; set; }

        public string? Chstatus { get; set; }



        public int id { get; set; }
        public int Growerid { get; set; }
        public string? GrowerGroupName { get; set; }
        public string? GrowerName { get; set; }
        public string? GrowerContact { get; set; }
        public string? GrowerCity { get; set; }
        public string? GrowerState { get; set; }
        public string? GrowerEmail { get; set; } = "";
        public string? GrowerVillage { get; set; } = string.Empty;
        public string? GrowerRemarks { get; set; } = string.Empty;

        public string? GrowerGrace { get; set; } = string.Empty;




        public string? GrowerAddress { get; set; } = string.Empty;

        public string? Cdated { get; set; }

        public string? Grstatus { get; set; }
        public string? Grtype { get; set; }

        public decimal GrowerCrateissue { get; set; } = 0;
        public decimal GrowerCratelimit { get; set; } = 0;

        public string? GrowerLock { get; set; }
        public string? GrowerFlexi { get; set; }

        public string? GrowerActive { get; set; }

        public string? GrowerGst { get; set; }
        public string? GrowerCountry { get; set; } = "India";

        public string? GrowerPan { get; set; }
        public string? GrowerUser { get; set; }
        public string? GrowerPincode { get; set; }
        public string? GrowerStatecode { get; set; }
        public string? GrowerGroupSelectName { get; set; }

        public string? RetMessage { get; set; }

        public string? RetFlag { get; set; }

        public int SubGrowerId { get; set; } = 0;


        public bool GrowerRetLock { get; set; } = false;

        public bool GrowerRetFlexi { get; set; } = false;

        public bool GrowerRetAcitve { get; set; } = false;



        public bool AccountRetLock { get; set; } = false;


        public string? BillGrowerGroup { get; set; }
        public string? BillChamber { get; set; }
        public decimal? BillAmount { get; set; }


        public bool AccountRetActive { get; set; } = false;






        public int InvoiceNo { get; set; }
        public string? InvoiceIrn { get; set; }
        public DateTime? Billdate { get; set; } = DateTime.Now;
        public string? Invdated { get; set; }
        public DateTime? BillCreatedate { get; set; } = DateTime.Now;

        public string? Billtype { get; set; }
        public decimal? TransferValue { get; set; }
        public string? BillGrower { get; set; }

        public string? Billstatus { get; set; }
        public string? Billcurrentstatus { get; set; }


        public string? ChallanName { get; set; }

        public string? ChallanAddress { get; set; }

        public string? ChallanVillage { get; set; }
        public string? vehname { get; set; }
        public string? ChallanCity { get; set; }
        public int ChallanId { get; set; }

        public string? ChallanPhone1 { get; set; }


        public string? ChallanPhone2 { get; set; } = "";


        public string? ChallanSMS1 { get; set; }

        public string? ChallanSMS2 { get; set; }
        public DateTime GrowerRepdate { get; set; } = DateTime.Today;

        public DateTime GrowerRepdateto { get; set; } = DateTime.Today;


        public DateTime Adated { get; set; } = DateTime.Today;

        public DateTime Idated { get; set; } = DateTime.Today;
        public DateTime Instadate { get; set; } = DateTime.Today;
        public decimal? Instamount { get; set; }
        public decimal? Netweight { get; set; }
        public decimal? TotalNetweight { get; set; }
        public decimal? TotalAgrade { get; set; }
        public string? Avgsizename { get; set; }
        public decimal? TotalBgrade { get; set; }
        public decimal? TotalAgradePercent { get; set; }
        public decimal? TotalBgradePercent { get; set; }
        public decimal? TotalAgradepercrate { get; set; }
        public decimal? TotalBgradepercrate { get; set; }
        public int aid { get; set; } = 0;
        public int AgreementId { get; set; } = 0;

        public string Fyear { get; set; } = "25-26";

        public decimal? Aqty { get; set; }
        public string Orderby { get; set; } = "";
        public string Itemname { get; set; } = "";

        public string Atype { get; set; } = "";
        public string RentType { get; set; } = "";
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? StoreRent { get; set; }

        public decimal? CrateRate { get; set; }

        public decimal? LabourRate { get; set; }


        public decimal? GradingRent { get; set; }

        public decimal? BinRent { get; set; }

        public decimal? PackingRent { get; set; }
        public string Achambers { get; set; } = "";

        public string SalesType { get; set; } = "NA";




        public decimal? InvestQty { get; set; } = 0;

        public decimal? InvestRate { get; set; } = 0;

        public decimal? InvtotalRate { get; set; }

        public decimal? InvtotalComm { get; set; }
        public decimal? Invpercent { get; set; }
        public decimal? Totalcomm { get; set; }


        public decimal? TotalReturn { get; set; }

        public string InvestType { get; set; } = "";

        public string InvestFlag { get; set; } = "0";


        // public decimal Total => InvestQty * InvestRate; // Optional: for display
        public decimal? TotalValue { get; set; }

        public decimal? DoubleValue { get; set; }

        public decimal? percentvalue { get; set; }

        public decimal? Returnvalue { get; set; }



        public bool Investmentflag { get; set; } = false;



        public decimal? Iamount { get; set; }

        public decimal? AccountBalance { get; set; }
        public string? BalanceType { get; set; }


        public int? Inid { get; set; }


        public DateTime Indated { get; set; } = DateTime.Today;

        public string? AccountType { get; set; }
        public string? AccountGroup { get; set; }

        public int Partyid { get; set; }
        public int Accountid { get; set; }
        public string? AccountName { get; set; }
        public string? AccountContact { get; set; }
        public string? AccountCity { get; set; }
        public string? AccountState { get; set; }
        public string? AccountEmail { get; set; } = "";
        public string? AccountVillage { get; set; } = string.Empty;
        public string? AccountRemarks { get; set; } = string.Empty;
        public string? AccountGrace { get; set; } = string.Empty;




        public string? AccountAddress { get; set; } = string.Empty;

        public string? AccountCdated { get; set; }

        public string? Accountstatus { get; set; }

        public string? AccountLock { get; set; }

        public string? AccountActive { get; set; }

        public string? AccountGst { get; set; }
        public string? AccountCountry { get; set; } = "India";

        public string? AccountPan { get; set; }
        public string? AccountPincode { get; set; }
        public string? AccountStatecode { get; set; }

        public string AccountApproval { get; set; } = "No";
        public string? VoucherRemarks { get; set; }
        public string? VoucherName { get; set; }

        public int VoucherId { get; set; }


        public bool? VoucherStatus { get; set; }
        public DateTime? VoucherCreatedDate { get; set; }



        public int TransactionId { get; set; }
        public DateTime? TransactionDated { get; set; } = DateTime.Today;

        public string? TransactionPaymentType { get; set; }
        public string? TransactionCredit { get; set; }

        public string? TransactionDebit { get; set; }
        public string? TransactionRemarks { get; set; }
        public string? TransactionName { get; set; }
        public string? TransactionType { get; set; }
        public string? TransactionChequeno { get; set; }
        public string? TransactionRefno { get; set; }


        public decimal? TransactionAmount { get; set; }
        public string? TotalDebit { get; set; }
        public string? TotalCredit { get; set; }
        public decimal? TransactionDebitAmt { get; set; }
        public decimal? TransactionCreditAmt { get; set; }
        public decimal? TransactionRunning { get; set; }

        public string? TotalBalance { get; set; }

        public int PurchaseOrderItemId { get; set; }
        public int PurchaseOrderId { get; set; }
        public DateTime? PurchaseOrderDated { get; set; } = DateTime.Today;

        public decimal? PurchaseOrderqty { get; set; }

        public string? PurchaseOrderRemarks { get; set; }
        public string? PurchaseOrderBillto { get; set; }
        public string? PurchaseOrderDel { get; set; }
        public string? PurchaseOrderStatus { get; set; }
        public string? PurchaseOrderItemDescrip { get; set; }

        public string? Vehno { get; set; }
        public string? Vehtype { get; set; }

        public string? VehDriver { get; set; }
        public string? VehContact { get; set; }

        public int Vehid { get; set; }


        public int CrissueId { get; set; }
        public int CrissueretId { get; set; }
        public string? CrissueRemarks { get; set; }

        public decimal? CrissueQty { get; set; }
        public string? CrateCrn { get; set; }
        public string? CrissueFlag { get; set; }
        public DateTime? CrateIssueDated { get; set; } = DateTime.Today;
        public string? CrissueMark { get; set; }
        public string? CrTransferGrowerGroup { get; set; }

        public string? CrTransferGrower { get; set; }

        public decimal? CrateBalance { get; set; }
        public decimal? GrowerBalance { get; set; }
        public decimal? PettyBalance { get; set; }
        public decimal? CrateAgreement { get; set; }
        public decimal? CrateIssue { get; set; }
        public decimal? CrateReceive { get; set; }
        public decimal? EmptyReceive { get; set; }

        public decimal? PartyPending { get; set; }

        public decimal? GrowerPending { get; set; }
        public decimal? CrateAvailable { get; set; }
        public DateTime? CrateAdjDate { get; set; } = DateTime.Today;
        public DateTime? CrateDatefrom { get; set; } = new DateTime(2025, 09, 1);
        public DateTime? CrateDateto { get; set; } = DateTime.Today;
        public DateTime? Outdatefrom { get; set; } = new DateTime(2025, 09, 1);
        public DateTime? OutdateTo { get; set; } = DateTime.Today;


        public decimal? SelfCrateReceive { get; set; }
        public decimal? EmptyReturn { get; set; }
        public decimal? SelfCrateBalance { get; set; }
        public decimal? PettyReceive { get; set; }
        public decimal? PettyReturn { get; set; }

        public decimal? CrateAdjustmentTaken { get; set; }
        public decimal? CrateAdjustmentGiven { get; set; }
        public decimal? TotalInwardQty { get; set; }

        public decimal? TempInsertedQty { get; set; }
        public int Trid { get; set; }
        public int GlobalUnitId { get; set; }
        [Required(ErrorMessage = "Please select a unit.")]
        public string? GlobalUnitName { get; set; }

        public int TempGateInId { get; set; }
        public string? TempGateInIrn { get; set; }

        public DateTime? PreinwardDate { get; set; } = new DateTime(2025, 09, 1);
        public DateTime? PreinwardDateTo { get; set; } = DateTime.Today;

        public int PreInwardId { get; set; }
        public int Lotno { get; set; }
        public int ChamberId { get; set; } = 0;
        public int YearId { get; set; } = 0;

        public int NewchamberId { get; set; } = 0;
        public decimal? ChamberAvailQty { get; set; }


        public int VarietyQty { get; set; }

        public int VarietyRate { get; set; }

        public int VarietyValue { get; set; }



        public decimal VarietyQtyAgrade { get; set; }

        public decimal VarietyQtyBgrade { get; set; }
        public decimal VarietyRateAgrade { get; set; }

        public decimal VarietyRateBgrade { get; set; }
        public decimal VarietyValueAgrade { get; set; }

        public decimal VarietyValueBgrade { get; set; }


        public int Brandid { get; set; }

        public int FlexiChamberId { get; set; }
        public string? ServiceId { get; set; }
        public string? ProductId { get; set; }
        public string? VarietyId { get; set; }
        public string? PreCrateType { get; set; }
        public decimal? PreInwardQty { get; set; }

        public decimal? SplitlotQty { get; set; }
        public string? Mixlotkhata { get; set; }

        public string? PreInIrn { get; set; }
        public int Mixlotno { get; set; }

        public string? PackingGrower { get; set; }
        public string? QualityRemarks { get; set; }
        public string? Lotstatus { get; set; }
        public string? LotIrn { get; set; }

        public decimal? OwnQty { get; set; }
        public decimal? StoreQty { get; set; }
        public decimal? PrebookQty { get; set; }
        public string? PreInwardRemarks { get; set; }
        public string? PreInwardStatus { get; set; }
        public string? PreInwardUom { get; set; }

        public string? PreInwardKhata { get; set; }
        public string? UserName { get; set; }
        public string? ChallanNo { get; set; }

        //////////quality
        public string? PreInwardCode { get; set; }
        public string? LotNo { get; set; }
        public string? PartyGroup { get; set; }
        public string? PartyName { get; set; }
        public string? ItemName { get; set; }
        public string? BrandName { get; set; }
        public string? CrateMarka { get; set; }
        public string? Stickerprinted { get; set; }
        public decimal? VerfiedOut { get; set; }
        public decimal? VerfiedAvailable { get; set; }


        public string? Templocation { get; set; }
        public decimal? VerfiedQty { get; set; }
        public decimal? VerfiedCompanyCrates { get; set; }
        public decimal? VerfiedOwnCrates { get; set; }
        public decimal? Verfiedbins { get; set; }

        public decimal? Verfiedpetties { get; set; }
        public decimal? Verfiedpallets { get; set; }
        public int? StickerNo { get; set; }
        public string? Sprinter { get; set; }
        public DateTime PreInwardDate { get; set; } = DateTime.Now;
        public bool IsInvestor { get; set; }
        public bool IsGrading { get; set; }
        public string? InvestorText { get; set; }
        public string? FloorName { get; set; }
        public string? MatrixName { get; set; }
                     
        public string? ChamberType { get; set; }
        public string? Password { get; set; }
                     
        public string? stockType { get; set; }
        public string? RowName { get; set; }
        public string? ColumName { get; set; }
        public decimal? CrateNos { get; set; }



        // Images
        public string? Image1Path { get; set; }
        public string? Image2Path { get; set; }
        public string? Image3Path { get; set; }

        // Pressure
        public decimal? Pressure1 { get; set; }
        public decimal? Pressure2 { get; set; }
        public decimal? Pressure3 { get; set; }
        public decimal? AvgPressure { get; set; }

        // Weight
        public decimal? AvgWeight1 { get; set; }
        public decimal? AvgWeight2 { get; set; }
        public decimal? AvgWeight3 { get; set; }
        public decimal? AvgWeight { get; set; }
        public decimal? Pporderqty { get; set; }

        public string? PPdescrip { get; set; }
        // B-Grade
        public string? BGradeAvgType { get; set; }
        public decimal? BGradeAvg1 { get; set; }
        public decimal? BGradeAvg2 { get; set; }
        public decimal? BGradeAvgValue { get; set; }

        // Other fields
        public string? LessColorPercentage { get; set; }
        public string? AvgSize { get; set; }
        public string? Remarks { get; set; }
        public string? PreinwardStatus { get; set; }
        // Defects
        public bool HasWaterCore { get; set; }
        public bool HasRusting { get; set; }
        public bool HasTouch { get; set; }
        public bool HasScab { get; set; }
        public bool HasBitterPit { get; set; }
        public bool HasSunburn { get; set; }
        public bool HasBitterRot { get; set; }
        public bool HasBlouch { get; set; }
        public bool HasInsectHole { get; set; }
        public bool HasOlla { get; set; }
        public bool HasSanjose { get; set; }
        public bool HasScar { get; set; }
        public bool HasFlySpeck { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string GlobalUserName { get; set; } = "";
        public string? GlobalUserGroup { get; set; } = "";

        public int Userid { get; set; } = 0;
        public string? UserStatus { get; set; } = "";

        public string? Useremail { get; set; } = "";

        [Required(ErrorMessage = "Password is required.")]
        public string UserPassword { get; set; } = "";
        public string? retypepassword { get; set; } = "";
        public bool Unit1 { get; set; }
        public bool Unit2 { get; set; }
        public bool Unit3 { get; set; }
        public bool Unit4 { get; set; }
        public bool Unit5 { get; set; }
        public string UserGroupName { get; set; } = "";
        public int UserGroupid { get; set; } = 0;
        public string UserGroupRemarks { get; set; } = "";
        public string UserGroupStatus { get; set; } = "";

        public string UserPrivname { get; set; } = "";
        public bool Pageview { get; set; }

        public bool AddVal { get; set; }
        public bool EditVal { get; set; }

        public bool ViewVal { get; set; }


        public bool Delval { get; set; }
        public bool Appval { get; set; }

        public bool AllocationAdd { get; set; }
        public bool AllocationView { get; set; }
        public bool AllocationDel { get; set; }
        public bool AllocationEdit { get; set; }

        public bool CrateAdd { get; set; }
        public bool CrateView { get; set; }
        public bool CrateDel { get; set; }
        public bool CrateEdit { get; set; }


        public bool DispatchAdd { get; set; }
        public bool DispatchView { get; set; }
        public bool DispatchDel { get; set; }
        public bool DispatchEdit { get; set; }



        public bool TradeAdd { get; set; }
        public bool TradeView { get; set; }
        public bool TradeDel { get; set; }
        public bool TradeEdit { get; set; }
        public bool TradeApp { get; set; }














        public bool PreAdd { get; set; }
        public bool PreView { get; set; }
        public bool PreDel { get; set; }
        public bool PreEdit { get; set; }

        public bool DockAdd { get; set; }
        public bool DockView { get; set; }
        public bool DockEdit { get; set; }
        public bool DockDel { get; set; }

        public bool QcAdd { get; set; }
        public bool QcView { get; set; }
        public bool QcEdit { get; set; }
        public bool QckDel { get; set; }
        public bool BillAdd { get; set; }
        public bool BillView { get; set; }
        public bool BillEdit { get; set; }
        public bool BillDel { get; set; }
        public bool BillApp { get; set; }

        public bool LocAdd { get; set; }
        public bool LocView { get; set; }
        public bool LocEdit { get; set; }
        public bool LocDel { get; set; }

        public bool GroAdd { get; set; }
        public bool GroView { get; set; }
        public bool GroEdit { get; set; }
        public bool GroDel { get; set; }

        public bool FoAdd { get; set; }
        public bool FoView { get; set; }
        public bool FoEdit { get; set; }
        public bool FoDel { get; set; }
        public bool FoApp { get; set; }

        public bool AgreeAdd { get; set; }
        public bool AgreeDel { get; set; }
        public bool AgreeView { get; set; }
        public bool AgreeEdit { get; set; }



        public bool DemAdd { get; set; }
        public bool DemView { get; set; }
        public bool DemDel { get; set; }
        public bool DemEdit { get; set; }

        public bool DemApp { get; set; }



        public bool PackingOrderAdd { get; set; }
        public bool PackingOrderView { get; set; }
        public bool PackingOrderDel { get; set; }
        public bool PackingOrderEdit { get; set; }

        public bool PackingOrderApp { get; set; }




        public bool PackingDraftAdd { get; set; }
        public bool PackingDraftView { get; set; }
        public bool PackingDraftDel { get; set; }
        public bool PackingDraftEdit { get; set; }

        public bool PackingDraftApp { get; set; }



















        public bool StoreoutAdd { get; set; }
        public bool StoreoutView { get; set; }
        public bool StoreoutDel { get; set; }
        public bool StoreoutEdit { get; set; }

        public bool StoreoutApp { get; set; }
















        ////chamber models
        ///

        public int chambercapacitytotal { get; set; } = 0;
        public int chamberallotedtotal { get; set; } = 0;

        public int chambercapacitytotalgrower { get; set; } = 0;
        public int chamberallotedtotalgrower { get; set; } = 0;

        public int chambercapcity { get; set; } = 0;


        public bool chamberstatus { get; set; }


        public string? TransferType { get; set; }
        public decimal? TransferRate { get; set; }
        public DateTime? TransferDate { get; set; } = DateTime.Now;
        public string? TransferTo { get; set; }


        public int DraftWeight { get; set; }

        public string? ChamberName { get; set; }
        public int ChamberRemaining { get; set; } = 0;
        public int Allocationsno { get; set; } = 0;
        public int Chamberalloted { get; set; } = 0;
        public int AllocationNo { get; set; } = 0;
        public int Capacity { get; set; }
        public int AvailableQty { get; set; }
        public string? Status { get; set; }
        public int AllocatedQty { get; set; }
        public int RemainingQty { get; set; }

        public bool IsLocked { get; set; }
        public decimal? ChamberAllocation { get; set; }
        public decimal? ChamberAllocationqty { get; set; }

        //public double UtilizationRate => Capacity > 0 ? (double)AllocatedQty / Capacity : 0;
        //public double AvailabilityRate => Capacity > 0 ? (double)RemainingQty / Capacity : 0;
        public double UtilizationRate = 0;
        public double AvailabilityRat = 0;
        public string? ReportType { get; set; }
        public string CardColorClass => GetCardColorClass();
        public string TextColorClass => GetTextColorClass();

        private string GetCardColorClass()
        {
            var rate = UtilizationRate;
            return rate switch
            {
                >= 0.8 => "high-utilization",    // 80%+ utilized
                >= 0.6 => "medium-utilization",  // 60-79% utilized
                >= 0.4 => "low-utilization",     // 40-59% utilized
                _ => "very-low-utilization"      // Below 40% utilized
            };
        }

        private string GetTextColorClass()
        {
            var rate = UtilizationRate;
            return rate switch
            {
                >= 0.6 => "text-white",  // Dark background, light text
                _ => "text-dark"         // Light background, dark text
            };
        }
        public int PackingId { get; set; }

        /////////////////OUT PART
        ///
        public string? DemandIrn { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.Now;
        public string? DemandStatus { get; set; }
        public DateTime? OrderDatefrom { get; set; } = new DateTime(2025, 10, 1);

        public DateTime? OrderDateto { get; set; } = DateTime.Now;
        public DateTime? DeliveryDate { get; set; } = DateTime.Now;
        public string? OrderType { get; set; }

        public string? GrowerCombine { get; set; }
        public int DemandNo { get; set; }
        public int Lotbalance { get; set; }
        public int Alotbal { get; set; }
        public int OutQty { get; set; }
        public bool IsSelected { get; set; }
        public bool IsReturned { get; set; }


        public bool IsSelectedEdit { get; set; } = true;
        public int? OrderQty { get; set; }

        public int? TransferInQty { get; set; }
        public int? TransferOutQty { get; set; }
        public string? DemandContact { get; set; }

        public string? DemandRemarks { get; set; }
        public string? OrderBy { get; set; }

        public int? Netqty { get; set; }
        public string? TempOrderIrn { get; set; }
        public int TempOrderId { get; set; }

        public int Outwardno { get; set; }

        public string? TempOutwardIrn { get; set; }
        public int TempOutwardId { get; set; }
        public int OutwardFixid { get; set; }
        public int ChamberInqty { get; set; } = 0;

        public int ChamberOutqty { get; set; } = 0;
        public int ChamberBalance { get; set; }


        public DateTime? MaxChamberDate { get; set; } = DateTime.Now;
        public DateTime? MinChamberDate { get; set; } = DateTime.Now;

        public DateTime? DraftDatefrom { get; set; } = new DateTime(2025, 10, 1);
        public DateTime? DraftDateTo { get; set; } = DateTime.Now;
        public string? StoreoutDate { get; set; }
        public DateTime? DraftDate { get; set; } = DateTime.Now;

        public string? ChamberinDate { get; set; }
        public string? ChamberOutDate { get; set; }
        public string? Draftstat { get; set; }

        public string? PackingDraftDate { get; set; }
        public int DanaQty { get; set; }
        public int Storestatus { get; set; } = 0;
        public bool IsuserAssigned { get; set; }
        public int TotalOrderQty { get; set; }
        public int TotalStoreOut { get; set; }
        public int PreStoreOut { get; set; }
        public string? StoreStat { get; set; }
        public int DraftedQty { get; set; }


        public string? ForceRemarks { get; set; }
        public string? DraftIrn { get; set; }
        public int TempDraftId { get; set; }
        public int GradingQty { get; set; }
        public int DraftId { get; set; }
        public int PelletQty { get; set; }
        //Grading Model
        public string? ReceipeName { get; set; }
        public string? Repackremarks { get; set; }
        public string HCASE { get; set; } = "HCASE";

        public string BOX { get; set; } = "BOX";
        public string? HcaseName { get; set; }

        public string? BoxName { get; set; }

        //public String ReportType { get; set; }
        public string? CommonSearch { get; set; }
        public int ReceipeId { get; set; }
        public int DraftIdentity { get; set; }
        public string? BoxBrand { get; set; }
        public string? ReceipeMarka { get; set; }
        public string? ReceipeUom { get; set; }
        public int SachetQty { get; set; }
        public int TotalReceipeQty { get; set; }
        public string? EntryDate { get; set; }
        public int Storeintid { get; set; }
        public decimal? ReceipeRate { get; set; }
        public string? ReceipeLocation { get; set; }
        public int ReceipeQty { get; set; }
        public string? ReceipeGrade { get; set; }
        public int PalletId { get; set; }
        public string? PalletName { get; set; }
        public int Gradeid { get; set; }

        //agstock
        public decimal? TotalPreQty { get; set; }
        public decimal? TotalPrecompanyQty { get; set; }
        public decimal? TotalPreownQty { get; set; }

        public decimal? TotalPrepettyQty { get; set; }


        public decimal? TotalInQty { get; set; }
        public decimal? TotalIncompanyQty { get; set; }
        public decimal? TotalInownQty { get; set; }

        public decimal? TotalInpettyQty { get; set; }


        public int Packingorderid { get; set; }

        //Grading Model
        public string? PackingOrderIrn { get; set; }
        //public String ReportType { get; set; }
        public DateTime? PackingOrderDate { get; set; } = DateTime.Now;


        public int PackingQty { get; set; }

        public string? PackingOrderStatus { get; set; }
        public int Packingbalance { get; set; }
        public int ActualPackingbalance { get; set; }
        public int Recipebalance { get; set; }
        public int DispatchQty { get; set; }
        public int totavqty { get; set; }
        public int selectedPackingQty { get; set; }
        public DateTime? PackingOrderDatefrom { get; set; } = new DateTime(2025, 10, 1);

        public DateTime? PackingOrderDateto { get; set; } = DateTime.Now;
        public int Outwardid { get; set; }

        public string? OutwardIrn { get; set; }

        public string? cname { get; set; }
        public DateTime? OutwardDate { get; set; } = DateTime.Now;
        public string Outwardtype { get; set; } = "Grading";

        public int OutwardQty { get; set; }

        public string OutwardStatus { get; set; } = "Unapproved";
        public int Outwardbalance { get; set; }
        public int OutwardPackingbalance { get; set; }
        public int DispatcherId { get; set; }
        public int TotInQty { get; set; }
        public int TotOutQty { get; set; }
        public int TotBalQty { get; set; }


        public int Chamberoutqty { get; set; }
        public int Chamberbalqty { get; set; }
        public DateTime? OutwardDatefrom { get; set; } = new DateTime(2025, 10, 1);

        public DateTime? OutwardDateto { get; set; } = DateTime.Now;
        public string? OutwardGrowerGroup { get; set; }

        public string? OutwardGrowerName { get; set; }
        public string? OutwardChallanName { get; set; }




    }





}
