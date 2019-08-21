﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konduskar_QuickBook
{
    public class SalesReceiptDetails
    {

        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int QuickBookCustomerID { get; set; }
        public string FromCityName { get; set; }
        public string ToCityName { get; set; }
        public string RouteFromCityName { get; set; }
        public string RouteToCityName { get; set; }
        public string BusNumber { get; set; }
        public string BusType { get; set; }
        public string TripCode { get; set; }
        public string BranchPhone1 { get; set; }
        public string BranchPhone2 { get; set; }
        public decimal Amount { get; set; }
        public string PNR { get; set; }
        public string PassengerName { get; set; }
        public string SeatNos { get; set; }
        public int SeatCount { get; set; }
        public string FromTo { get; set; }
        public string JDate { get; set; }
        public string JTime { get; set; }
        public string BTime { get; set; }
        public string BDate { get; set; }
        public int BookingID { get; set; }
        public DateTime GeneratedDate { get; set; }
        public DateTime SalesReceiptDate { get; set; }
        public int TransactionID { get; set; }
        public string ItemName { get; set; }
        public int ItemID { get; set; }
        public string ClassName { get; set; }
        public string ClassID { get; set; }
        public string BranchDivisionName { get; set; }
        public string BranchDivisionID { get; set; }
        public int VoucherCreditNoteId { get; set; }
        public int IsDisputed { get; set; }
        public string VoucherNo { get; set; }
        public string BookingStatus { get; set; }
        public string Prefix { get; set; }
        public decimal BaseFare { get; set; }
        public decimal TotalFare { get; set; }
        public decimal RefundAmount { get; set; }
        public string docformattednumber { get; set; }
        public string docnumber { get; set; }
        public int DivisionID { get; set; }
        public int BusMasterID { get; set; }
        public string Accsysid { get; set; }
        public string CreditNoteDateTime { get; set; }
        public string PickUpName { get; set; }
        public string DropOffName { get; set; }
        public decimal GST { get; set; }
        public decimal BranchCommission { get; set; }
        public string GSTType { get; set; }
        public int invoiceID { get; set; }
        public string PlaceOfSupply { get; set; }
        public int DepositLedgerId { get; set; }
        public string DepositLedgerName { get; set; }
        ///public string Accsysid { get; set; }
    }
}
