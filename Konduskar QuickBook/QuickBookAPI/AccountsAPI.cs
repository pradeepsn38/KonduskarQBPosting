﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Data;
using System.Data;
using System.Threading;
using Konduskar_QuickBook.Util;
using System.Configuration;
using Intuit.Ipp.Exception;
using Konduskar_QuickBook.QuickBookAPI;



namespace Konduskar_QuickBook

{
    public class AccountsAPI
    {

        public void Main()
        {
            //Console.Write("Start");

            DataSet ds = new DataSet();
            ds = GetMasterDataFromCRS();

            //Console.Write("DataSet");
            if (ds != null && ds.Tables.Count > 0)
            {

                #region Trip Master (Item)

                Boolean toPostTrips = Convert.ToBoolean(ConfigurationManager.AppSettings["PostTrips"].ToString());
                if(toPostTrips)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //string result = "";
                        foreach (DataRow drItemDetails in ds.Tables[0].Rows)
                        {
                            //Console.Write("Trip Start");
                            Item iPosted;
                            Item iPostedAC;
                            string status = "";
                            string statusMessage = "";
                            Int32 iID = -1;
                            Int32 iIDAC = -1;

                            if (drItemDetails["Action"].ToString() == "Insert")
                            {
                                //Post Non-AC Trip
                                try
                                {
                                    iPosted = PostItem(drItemDetails["Item"].ToString(),false);
                                    iID = Convert.ToInt32(iPosted.Id);
                                    status = "Posted";
                                    statusMessage = "Inserted";

                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                                    Logger.WriteLog("Trips", drItemDetails["Item"].ToString(), status + ":" + statusMessage, true);
                                }
                                //Post AC Trip commented from 'nov 2017'
                                //try
                                //{
                                //    iPostedAC = PostItem(drItemDetails["Item"].ToString(),true);
                                //    iIDAC = Convert.ToInt32(iPostedAC.Id);
                                //    status = "Posted";
                                //    statusMessage = "Inserted";
                                //}
                                //catch(Exception ex)
                                //{
                                //    status = "Failed";
                                //    statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                                //    Logger.WriteLog("Trips-AC", drItemDetails["Item"].ToString(), status + ":" + statusMessage, true);
                                //}
                            }
                            else
                            {
                                //Post Non-AC Trip
                                try
                                {
                                    string result = "";
                                    Item ItemData = new Item();
                                    ItemData = GetItem(drItemDetails["AccSysId"].ToString());
                                    //string trip = ItemData.FullyQualifiedName;
                                    //string b = trip.Split('_')[1];
                                    //string c = b.Split(' ')[0];
                                    //ItemData.IncomeAccountRef.name
                                    //result += ItemData.Id + ',';

                                    // result += ItemData.Id + '~' + c + '~' +  ItemData.IncomeAccountRef.Value + ',';

                                    iPosted = UpdateItem(drItemDetails["AccSysId"].ToString(), drItemDetails["Item"].ToString(), ItemData.SyncToken, false);
                                    iID = Convert.ToInt32(iPosted.Id);
                                    status = "Posted";
                                    statusMessage = "Updated";
                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = "Update- " + ex.Message + " : " + ex.Message;
                                    Logger.WriteLog("Trips", drItemDetails["AccSysId"].ToString(), status + ":" + statusMessage, true);
                                }
                                //Post AC Trip
                                //try
                                //{
                                //    Item ItemData = new Item();
                                //    ItemData = GetItem(drItemDetails["AccSysIdAC"].ToString());

                                //    iPostedAC = UpdateItem(drItemDetails["AccSysIdAC"].ToString(), drItemDetails["Item"].ToString(), ItemData.SyncToken,true);
                                //    iID = Convert.ToInt32(iPostedAC.Id);
                                //    status = "Posted";
                                //    statusMessage = "Updated";
                                //}
                                //catch(Exception ex)
                                //{
                                //    status = "Failed";
                                //    statusMessage = "Update- " + ex.Message + " : " + ex.Message;
                                //    Logger.WriteLog("Trips-AC", drItemDetails["AccSysIdAC"].ToString(), status + ":" + statusMessage, true);
                                //}
                            }

                            if (status == "Posted")
                            {
                                // Update Quickbook ID in Master Table - AccSysID,AccSysLastPostedTime
                                try
                                {
                                    UpdateTripAccSysId(Convert.ToInt32(drItemDetails["TripID"]), iID.ToString(),"");
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog("Trips", drItemDetails["TripID"].ToString(), ex.Message, true);
                                }

                            }
                        }
                    }
                }


                #endregion

                #region Bus Master (Class)

                Boolean toPostBuses = Convert.ToBoolean(ConfigurationManager.AppSettings["PostBuses"].ToString());
                if (toPostBuses)
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow drBusDetails in ds.Tables[1].Rows)
                        {
                            Class iPosted;
                            string status = "";
                            string statusMessage = "";
                            string iID = "";

                            if (drBusDetails["Action"].ToString() == "Insert")
                            {
                                try
                                {
                                    iPosted = PostBus(drBusDetails["BusNumber"].ToString());
                                    iID = iPosted.Id;
                                    status = "Posted";
                                    statusMessage = "Inserted";

                                    // Update Quickbook ID in Master Table - AccSysID,AccSysLastPostedTime
                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                                    Logger.WriteLog("Bus", drBusDetails["BusNumber"].ToString(), status + ":" + statusMessage, true);
                                }
                            }
                            //else if (drBusDetails["Action"].ToString() == "Update")
                            //{
                            //    try
                            //    {
                            //        Class BusData = new Class();
                            //        BusData = GetBus(drBusDetails["AccSysId"].ToString());

                            //        iPosted = UpdateBus(drBusDetails["AccSysId"].ToString(), drBusDetails["BusNumber"].ToString(), BusData.SyncToken);
                            //        iID = iPosted.Id;
                            //        status = "Posted";
                            //        statusMessage = "Updated";
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        status = "Failed";
                            //        statusMessage = "Update- " + ex.Message + " : " + ex.Message;
                            //        Logger.WriteLog("Bus", drBusDetails["BusNumber"].ToString(), status + ":" + statusMessage, true);
                            //    }
                            //}

                            if (status == "Posted")
                            {
                                // Update Quickbook ID in Master Table - AccSysID,AccSysLastPostedTime
                                try
                                {
                                    string busnumber = drBusDetails["BusNumber"].ToString() + '_' +  Convert.ToInt32(drBusDetails["BusMasterID"]);

                                    UpdateBusAccSysId(Convert.ToInt32(drBusDetails["BusMasterID"]), iID, drBusDetails["BusNumber"].ToString());
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog("Bus", drBusDetails["BusNumber"].ToString(), ex.Message, true);
                                }

                            }
                        }
                    }
                }


                #endregion

                #region Agent Master (Customer)

                Boolean toPostAgents = Convert.ToBoolean(ConfigurationManager.AppSettings["PostAgents"].ToString());
                if (toPostAgents)
                {
                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        foreach (DataRow drCustomerDetails in ds.Tables[2].Rows)
                        {
                            //string result = "";
                            Customer iPosted;
                            string status = "";
                            string statusMessage = "";
                            string iID = "";

                            if (drCustomerDetails["Action"].ToString() == "Insert")
                            {
                                try
                                {
                                    iPosted = PostAgent(drCustomerDetails["AgentName"].ToString(), drCustomerDetails["CompanyName"].ToString(), drCustomerDetails["AgentAddress"].ToString(), drCustomerDetails["CityName"].ToString(), drCustomerDetails["StateName"].ToString(), "India", drCustomerDetails["ContactNo1"].ToString(), drCustomerDetails["AgentID"].ToString());
                                    iID = iPosted.Id;
                                    status = "Posted";
                                    statusMessage = "Inserted";

                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                                    Logger.WriteLog("Agent", drCustomerDetails["AgentName"].ToString(), status + ":" + statusMessage, true);
                                }
                            }
                            else
                            {
                                try
                                {
                                    Customer CustomerData = new Customer();
                                    CustomerData = GetAgent(drCustomerDetails["AccSysId"].ToString());

                                    //result += CustomerData.Id + '~' + CustomerData.DisplayName + "~" + CustomerData.Balance + ",";
                                    iPosted = UpdateAgent(drCustomerDetails["AccSysId"].ToString(), drCustomerDetails["AgentName"].ToString(), drCustomerDetails["CompanyName"].ToString(), drCustomerDetails["AgentAddress"].ToString(), drCustomerDetails["CityName"].ToString(), drCustomerDetails["StateName"].ToString(), "India", drCustomerDetails["ContactNo1"].ToString(), CustomerData.SyncToken, drCustomerDetails["AgentID"].ToString());
                                    iID = iPosted.Id;
                                    status = "Posted";
                                    statusMessage = "Updated";
                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = "Update- " + ex.Message + " : " + ex.Message;
                                    Logger.WriteLog("Agent", drCustomerDetails["AgentName"].ToString(), status + ":" + statusMessage, true);
                                }
                            }

                            if (status == "Posted")
                            {
                                // Update Quickbook ID in Master Table - AccSysID,AccSysLastPostedTime
                                try
                                {
                                    UpdateAgentAccSysId(Convert.ToInt32(drCustomerDetails["AgentID"]), Convert.ToInt32(drCustomerDetails["CompanyID"]), iID);
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog("Agent", drCustomerDetails["AgentName"].ToString(), ex.Message, true);
                                }
                            }
                        }
                    }
                }


                #endregion

                

                #region Expense (Purchase) - Contra (Transfer)
                Boolean toPostExpense = Convert.ToBoolean(ConfigurationManager.AppSettings["PostExpense"].ToString());
                if (toPostExpense)
                {
                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        EntryCounter.GetInstance().ResetAllCount();
                        foreach (DataRow drExpenseDetails in ds.Tables[3].Rows)
                        {

                            string status = "";
                            string statusMessage = "";
                            Int32 iID = -1;

                            if (drExpenseDetails["doctype"].ToString() == "Expense")
                            {

                                #region Expense

                                if (drExpenseDetails["Action"].ToString() == "Insert")
                                {
                                    try
                                    {
                                        int intvoucherpaymentid = Convert.ToInt32(drExpenseDetails["voucherpaymentid"].ToString());

                                        //string VoucherNoQB = CreateExpenseNoForQB(Convert.ToInt32(drExpenseDetails["CompanyID"].ToString()), Convert.ToInt32(drExpenseDetails["divisionid"].ToString()), intvoucherpaymentid);

                                        Purchase iPosted = PostPurchase("Insert","","",intvoucherpaymentid, Convert.ToInt32(drExpenseDetails["FromAccountId"].ToString()), drExpenseDetails["FromAccount"].ToString(), Convert.ToInt32(drExpenseDetails["paymentmethodid"].ToString()), drExpenseDetails["ToAccountType"].ToString(), drExpenseDetails["Ref"].ToString(), drExpenseDetails["TxnDate"].ToString(), Convert.ToInt32(drExpenseDetails["accsysdivisionid"].ToString()), drExpenseDetails["divisionname"].ToString(), Convert.ToInt32(drExpenseDetails["Payeeid"].ToString()), drExpenseDetails["Payeename"].ToString(), Convert.ToDecimal(drExpenseDetails["TotalAmount"].ToString()), drExpenseDetails["docformattednumber"].ToString(), drExpenseDetails["narration"].ToString());
                                        iID = Convert.ToInt32(iPosted.Id);
                                        status = "Posted";
                                        statusMessage = "Inserted";
                                        EntryCounter.GetInstance().IncreaseQBCount(1);
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Failed";
                                        statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                                        Logger.WriteLog("Expense", drExpenseDetails["voucherpaymentid"].ToString(), status + ":" + statusMessage, true);
                                    }
                                }
                                else if (drExpenseDetails["Action"].ToString() == "Update")
                                {
                                    try
                                    {
                                        Purchase PurchaseData = new Purchase();
                                        PurchaseData = GetPurchase(drExpenseDetails["AccSysId"].ToString());

                                        int intvoucherpaymentid = Convert.ToInt32(drExpenseDetails["voucherpaymentid"].ToString());

                                        Purchase iPosted = PostPurchase("Update",PurchaseData.Id, PurchaseData.SyncToken, intvoucherpaymentid, Convert.ToInt32(drExpenseDetails["FromAccountId"].ToString()), drExpenseDetails["FromAccount"].ToString(), Convert.ToInt32(drExpenseDetails["paymentmethodid"].ToString()), drExpenseDetails["ToAccountType"].ToString(), drExpenseDetails["Ref"].ToString(), drExpenseDetails["TxnDate"].ToString(), Convert.ToInt32(drExpenseDetails["accsysdivisionid"].ToString()), drExpenseDetails["divisionname"].ToString(), Convert.ToInt32(drExpenseDetails["Payeeid"].ToString()), drExpenseDetails["Payeename"].ToString(), Convert.ToDecimal(drExpenseDetails["TotalAmount"].ToString()),  drExpenseDetails["docformattednumber"].ToString(), drExpenseDetails["narration"].ToString());
                                        iID = Convert.ToInt32(iPosted.Id);
                                        status = "Posted";
                                        statusMessage = "Updated";
                                        EntryCounter.GetInstance().IncreaseQBCount(1);
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Failed";
                                        statusMessage = "Update- " + ex.Message + " : " + ex.Message;
                                        Logger.WriteLog("Expense", drExpenseDetails["voucherpaymentid"].ToString(), status + ":" + statusMessage, true);
                                    }
                                }
                                else // Delete
                                {
                                    try
                                    {
                                        Purchase PurchaseData = new Purchase();
                                        if (!drExpenseDetails["AccSysId"].ToString().Equals(""))
                                        {
                                            PurchaseData = GetPurchase(drExpenseDetails["AccSysId"].ToString());

                                            if (PurchaseData != null)
                                            {
                                                Purchase iPosted = DeletePurchase(drExpenseDetails["AccSysId"].ToString(), PurchaseData.SyncToken);
                                                iID = Convert.ToInt32(iPosted.Id);
                                                status = "Posted";
                                                statusMessage = "Deleted";
                                                EntryCounter.GetInstance().IncreaseQBCount(1);
                                            }
                                           
                                        }
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Failed";
                                        statusMessage = "Delete- " + ex.Message + " : " + ex.Message;
                                        Logger.WriteLog("Expense", drExpenseDetails["AccSysId"].ToString(), status + ":" + statusMessage, true);
                                    }
                                }
                                #endregion


                            }
                            else if (drExpenseDetails["doctype"].ToString() == "Contra")
                            {

                                #region Contra
                                if (drExpenseDetails["Action"].ToString() == "Insert")
                                {
                                    try
                                    {
                                        int intvoucherpaymentid = Convert.ToInt32(drExpenseDetails["voucherpaymentid"].ToString());

                                        //string VoucherNoQB = CreateContraNoForQB(Convert.ToInt32(drExpenseDetails["CompanyID"].ToString()), Convert.ToInt32(drExpenseDetails["divisionid"].ToString()), intvoucherpaymentid);

                                        Transfer iPosted = PostTransfer("Insert","","",intvoucherpaymentid, Convert.ToInt32(drExpenseDetails["FromAccountId"].ToString()), drExpenseDetails["FromAccount"].ToString(), drExpenseDetails["TxnDate"].ToString(), Convert.ToDecimal(drExpenseDetails["TotalAmount"].ToString()), drExpenseDetails["narration"].ToString(),drExpenseDetails["docformattednumber"].ToString());
                                        iID = Convert.ToInt32(iPosted.Id);
                                        status = "Posted";
                                        statusMessage = "Inserted";
                                        EntryCounter.GetInstance().IncreaseQBCount(1);
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Failed";
                                        statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                                        Logger.WriteLog("Contra", drExpenseDetails["voucherpaymentid"].ToString(), status + ":" + statusMessage, true);
                                    }
                                }
                                else if (drExpenseDetails["Action"].ToString() == "Update")
                                {
                                    try
                                    {
                                        Transfer TransferData = new Transfer();
                                        TransferData = GetTransfer(drExpenseDetails["AccSysId"].ToString());

                                        int intvoucherpaymentid = Convert.ToInt32(drExpenseDetails["voucherpaymentid"].ToString());

                                        Transfer iPosted = PostTransfer("Update", TransferData.Id, TransferData.SyncToken, intvoucherpaymentid, Convert.ToInt32(drExpenseDetails["FromAccountId"].ToString()), drExpenseDetails["FromAccount"].ToString(), drExpenseDetails["TxnDate"].ToString(), Convert.ToDecimal(drExpenseDetails["TotalAmount"].ToString()), drExpenseDetails["narration"].ToString(),  drExpenseDetails["docformattednumber"].ToString());
                                        iID = Convert.ToInt32(iPosted.Id);
                                        status = "Posted";
                                        statusMessage = "Updated";
                                        EntryCounter.GetInstance().IncreaseQBCount(1);
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Failed";
                                        statusMessage = "Update- " + ex.Message + " : " + ex.Message;
                                        Logger.WriteLog("Contra", drExpenseDetails["voucherpaymentid"].ToString(), status + ":" + statusMessage, true);
                                    }
                                }
                                else // Delete
                                {
                                    try
                                    {
                                        Transfer TransferData = new Transfer();
                                        if (!drExpenseDetails["AccSysId"].ToString().Equals(""))
                                        {
                                            TransferData = GetTransfer(drExpenseDetails["AccSysId"].ToString());

                                            if (TransferData != null)
                                            {
                                                Transfer iPosted = DeleteTransfer(drExpenseDetails["AccSysId"].ToString(), TransferData.SyncToken);
                                                iID = Convert.ToInt32(iPosted.Id);
                                                status = "Posted";
                                                statusMessage = "Deleted";
                                                EntryCounter.GetInstance().IncreaseQBCount(1);
                                            }                                            
                                        }
                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Failed";
                                        statusMessage = "Delete- " + ex.Message + " : " + ex.Message;
                                        Logger.WriteLog("Contra", drExpenseDetails["AccSysId"].ToString(), status + ":" + statusMessage, true);
                                    }
                                }
                                #endregion


                            }

                            if (status == "Posted")
                            {
                                try
                                {
                                    // Update Quickbook ID in Master Table - AccSysID,AccSysLastPostedTime
                                    UpdateExpenseContraAccSysId(Convert.ToInt32(drExpenseDetails["voucherpaymentid"]), iID);
                                    EntryCounter.GetInstance().IncreaseCRSCount(1);
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog("ExpenseOrContra", drExpenseDetails["voucherpaymentid"].ToString(), ex.Message, true);
                                }
                            }
                        }

                        if(!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                        {
                            string msg = "ExpenseContraPosting::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                            Email.SendMail(msg);
                        }
                    }
                }


                #endregion

                //#region Expense/Contra not posted
                //if (ds.Tables[4].Rows.Count > 0)
                //{
                //    if (Convert.ToInt32(ds.Tables[4].Rows[0]["Count"]) > 0)
                //    {
                //        string VoucherIds = ds.Tables[4].Rows[0]["VoucherPaymentid"].ToString();
                //        string ErrMsg = ds.Tables[4].Rows[0]["ErrMsg"].ToString();
                //        Logger.WriteLog(ErrMsg + VoucherIds);
                //        Email.SendMail(ErrMsg + VoucherIds);

                //    }
                   
                //}
                //#endregion


                #region Franchise Master (As Customer)

                Boolean toPostFranchiseandBranches = Convert.ToBoolean(ConfigurationManager.AppSettings["PostFranchiseandBranches"].ToString());
                if (toPostFranchiseandBranches)
                {
                    if (ds.Tables[5].Rows.Count > 0)
                    {
                        foreach (DataRow drCustomerDetails in ds.Tables[5].Rows)
                        {
                            //string result = "";
                            Customer iPosted;
                            string status = "";
                            string statusMessage = "";
                            string iID = "";

                            if (drCustomerDetails["Action"].ToString() == "Insert")
                            {
                                try
                                {
                                    iPosted = PostAgent(drCustomerDetails["FranchiseName"].ToString(), drCustomerDetails["CompanyName"].ToString(), drCustomerDetails["FranchiseAddress"].ToString(), drCustomerDetails["CityName"].ToString(), drCustomerDetails["StateName"].ToString(), "India", drCustomerDetails["ContactNo1"].ToString(), drCustomerDetails["FranchiseID"].ToString());
                                    iID = iPosted.Id;
                                    status = "Posted";
                                    statusMessage = "Inserted";

                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                                    Logger.WriteLog("Franchise", drCustomerDetails["FranchiseName"].ToString(), status + ":" + statusMessage, true);
                                }
                            }
                            else
                            {
                                try
                                {
                                    Customer CustomerData = new Customer();
                                    CustomerData = GetAgent(drCustomerDetails["AccSysId"].ToString());

                                    //result += CustomerData.Id + '~' + CustomerData.DisplayName + "~" + CustomerData.Balance + ",";
                                    iPosted = UpdateAgent(drCustomerDetails["AccSysId"].ToString(), drCustomerDetails["FranchiseName"].ToString(), drCustomerDetails["CompanyName"].ToString(), drCustomerDetails["FranchiseAddress"].ToString(), drCustomerDetails["CityName"].ToString(), drCustomerDetails["StateName"].ToString(), "India", drCustomerDetails["ContactNo1"].ToString(), CustomerData.SyncToken, drCustomerDetails["FranchiseID"].ToString());
                                    iID = iPosted.Id;
                                    status = "Posted";
                                    statusMessage = "Updated";
                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = "Update- " + ex.Message + " : " + ex.Message;
                                    Logger.WriteLog("Franchise", drCustomerDetails["FranchiseName"].ToString(), status + ":" + statusMessage, true);
                                }
                            }

                            if (status == "Posted")
                            {
                                // Update Quickbook ID in Master Table - AccSysID,AccSysLastPostedTime
                                try
                                {
                                    UpdateFranchiseAccSysId(Convert.ToInt32(drCustomerDetails["FranchiseID"]), Convert.ToInt32(drCustomerDetails["CompanyID"]), iID);
                                }
                                catch (Exception ex)
                                {
                                    Logger.WriteLog("Franchise", drCustomerDetails["FranchiseName"].ToString(), ex.Message, true);
                                }
                            }
                        }
                    }
                }


                #endregion

            }
        }

        #region Fetch Data From DB
        private DataSet GetMasterDataFromCRS()
        {
            DataSet dstOutPut = null;
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_companyid", 69);

                dstOutPut = dal.ExecuteSelect("spGetDataForKonduskarQuickBooks", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                return dstOutPut;
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetMasterDataFromCRS: " + ex.Message,true);
            }
            return dstOutPut;
        }



        private DataSet GetBranchJournalEntryFromCRS()
        {
            DataSet dstOutPut = null;
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
                dstOutPut = dal.ExecuteSelect("spGetBranchJournalDataForDelete", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                return dstOutPut;
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetBranchJournalEntryFromCRS: " + ex.Message, true);
            }
            return dstOutPut;
        }


        private DataSet GetAgentPaymentEntryFromCRS()
        {
            DataSet dstOutPut = null;
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
                //dstOutPut = dal.ExecuteSelect("spGetReceiptDataForDelete", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);
                dstOutPut = dal.ExecuteSelect("spGetPaymentReceiptForQuickBooks_testutkarsh", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                return dstOutPut;
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetBranchJournalEntryFromCRS: " + ex.Message, true);
            }
            return dstOutPut;
        }


        #endregion

        #region Agent Voucher

        public void PostAgentVouchers()
        {
            #region Voucher Posting in Invoice

            List<AgentVoucherDetails> agentVoucherList = null;
            try
            {
                agentVoucherList = GetAgentVoucherDetailsFromCRS();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("PostAgentVouchers", "GetAgentVoucherDetailsFromCRS_Konduskar", ex.Message, true);
            }

            if (agentVoucherList != null && agentVoucherList.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                Logger.WriteLog("PostAgentVouchers_Konduskar", "", "No Of Agent Vouchers: " + agentVoucherList.Count, true);
                foreach (AgentVoucherDetails avDetails in agentVoucherList)
                {
                    Invoice ivPosted;
                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    try
                    {
                        string docnumber = "";
                        string docformattednumber = "";

                        CreateVoucherNoForQB(avDetails.CompanyID, Convert.ToInt32(avDetails.DivisionID), avDetails.BookingID,avDetails.GeneratedDate,ref docnumber,ref docformattednumber,"AV");

                        ivPosted = PostInvoice("Insert",avDetails.AgentID,"","", avDetails.AgentName, avDetails.QuickBookCustomerID, avDetails.FromCityName, avDetails.ToCityName, avDetails.RouteFromCityName, avDetails.RouteToCityName, avDetails.BusNumber, avDetails.AgentPhone1, avDetails.AgentPhone2, avDetails.Amount, avDetails.PNR, avDetails.PassengerName, avDetails.SeatNos, avDetails.SeatCount, avDetails.FromTo, avDetails.JDate, avDetails.JTime, avDetails.BDate, avDetails.BookingID, avDetails.VoucherDate, avDetails.TransactionID, avDetails.BusType, avDetails.TripCode, avDetails.ItemName, avDetails.ItemID, avDetails.ClassName, avDetails.ClassID, avDetails.BranchDivisionName, avDetails.BranchDivisionID, docformattednumber, avDetails.BookingStatus, avDetails.Prefix, avDetails.TotalFare, avDetails.RefundAmount,avDetails.PickUpName,avDetails.DropOffName,avDetails.GST,avDetails.AgentComm,avDetails.GSTType,avDetails.BaseFare,avDetails.PlaceOfSupply);
                        ivID = Convert.ToInt32(ivPosted.Id);
                        status = "Posted";
                        statusMessage = "";

                        EntryCounter.GetInstance().IncreaseQBCount(1);

                        InsertUpdateVoucherPostingDetailsToCRS(avDetails.AgentID, avDetails.CompanyID, avDetails.BookingID, ivID,docnumber,docformattednumber, avDetails.BusMasterID,avDetails.DivisionID);

                        EntryCounter.GetInstance().IncreaseCRSCount(1);
                    }
                    catch(IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("PostAgentVouchers_Konduskar", "", ex.Message, true);
                    }

                }

                if(!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "PostAgentVouchers_Konduskar:::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() +") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }


            #endregion

            return;
        }


        public void UpdateAgentVouchers()
        {
            #region Voucher Update in Invoice

            List<AgentVoucherDetails> agentVoucherList = null;
            try
            {
                agentVoucherList = GetAgentVoucherUpdateDetailsFromCRS();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("UpdateAgentVouchers", "GetAgentVoucherUpdateDetailsFromCRS", ex.Message, true);
            }

            if (agentVoucherList != null && agentVoucherList.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                Logger.WriteLog("UpdateAgentVouchers", "", "No Of Agent Vouchers: " + agentVoucherList.Count, true);
                foreach (AgentVoucherDetails avDetails in agentVoucherList)
                {
                    Invoice ivPosted;
                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    Invoice InvoiceData = new Invoice();
                    try
                    {
                       
                        InvoiceData = Getinvoice(avDetails.Accsysid);
                        if (InvoiceData != null)
                        {
                            ivPosted = PostInvoice("Update", avDetails.AgentID, InvoiceData.Id, InvoiceData.SyncToken, avDetails.AgentName, avDetails.QuickBookCustomerID, avDetails.FromCityName, avDetails.ToCityName, avDetails.RouteFromCityName, avDetails.RouteToCityName, avDetails.BusNumber, avDetails.AgentPhone1, avDetails.AgentPhone2, avDetails.Amount, avDetails.PNR, avDetails.PassengerName, avDetails.SeatNos, avDetails.SeatCount, avDetails.FromTo, avDetails.JDate, avDetails.JTime, avDetails.BDate, avDetails.BookingID, avDetails.GeneratedDate, avDetails.TransactionID, avDetails.BusType, avDetails.TripCode, avDetails.ItemName, avDetails.ItemID, avDetails.ClassName, avDetails.ClassID, avDetails.BranchDivisionName, avDetails.BranchDivisionID, avDetails.docformattednumber, avDetails.BookingStatus, avDetails.Prefix, avDetails.TotalFare, avDetails.RefundAmount,avDetails.PickUpName,avDetails.DropOffName,avDetails.GST,avDetails.AgentComm,avDetails.GSTType,avDetails.BaseFare,avDetails.PlaceOfSupply);
                            ivID = Convert.ToInt32(ivPosted.Id);
                            status = "Posted";
                            statusMessage = "";

                            EntryCounter.GetInstance().IncreaseQBCount(1);

                            InsertUpdateVoucherPostingDetailsToCRS(avDetails.AgentID, avDetails.CompanyID, avDetails.BookingID, ivID, avDetails.docnumber, avDetails.docformattednumber, avDetails.BusMasterID,avDetails.DivisionID);

                            EntryCounter.GetInstance().IncreaseCRSCount(1);
                        }
                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("UpdateAgentVouchers", "", ex.Message, true);
                    }

                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "UpdateAgentVouchers:::Mismatch in No Of Entries Updated to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }


            #endregion

            return;


        }

        public void PostMissingAgentVouchers()
        {
            #region Voucher Posting in Invoice

            List<AgentVoucherDetails> agentVoucherList = null;
            try
            {
                agentVoucherList = GetMissingAgentVoucherDetailsFromCRS();
                if (agentVoucherList != null && agentVoucherList.Count > 0)
                {
                    string msg = "PostMissingAgentVouchers"+ "No Of Agent Vouchers: " + agentVoucherList.Count;
                    Email.SendMail(msg);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("PostMissingAgentVouchers", "GetMissingAgentVoucherDetailsFromCRS", ex.Message, true);
            }
           Boolean toMissingAgentVouchersPostingAllow = Convert.ToBoolean(ConfigurationManager.AppSettings["MissingAgentVouchersPostingAllow"].ToString());
           if (toMissingAgentVouchersPostingAllow)
           {
                if (agentVoucherList != null && agentVoucherList.Count > 0)
                {

                EntryCounter.GetInstance().ResetAllCount();
                Logger.WriteLog("PostMissingAgentVouchers", "", "No Of Agent Vouchers: " + agentVoucherList.Count, true);

                foreach (AgentVoucherDetails avDetails in agentVoucherList)
                {
                    Invoice ivPosted;
                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    try
                    {
                        string docnumber = "";
                        string docformattednumber = "";

                        CreateVoucherNoForQB(avDetails.CompanyID, Convert.ToInt32(avDetails.DivisionID), avDetails.BookingID, avDetails.GeneratedDate, ref docnumber, ref docformattednumber,"AV");

                        ivPosted = PostInvoice("Insert", avDetails.AgentID, "", "", avDetails.AgentName, avDetails.QuickBookCustomerID, avDetails.FromCityName, avDetails.ToCityName, avDetails.RouteFromCityName, avDetails.RouteToCityName, avDetails.BusNumber, avDetails.AgentPhone1, avDetails.AgentPhone2, avDetails.Amount, avDetails.PNR, avDetails.PassengerName, avDetails.SeatNos, avDetails.SeatCount, avDetails.FromTo, avDetails.JDate, avDetails.JTime, avDetails.BDate, avDetails.BookingID, avDetails.GeneratedDate, avDetails.TransactionID, avDetails.BusType, avDetails.TripCode, avDetails.ItemName, avDetails.ItemID, avDetails.ClassName, avDetails.ClassID, avDetails.BranchDivisionName, avDetails.BranchDivisionID, docformattednumber, avDetails.BookingStatus, avDetails.Prefix, avDetails.TotalFare, avDetails.RefundAmount,avDetails.PickUpName,avDetails.DropOffName,avDetails.GST,avDetails.AgentComm,avDetails.GSTType,avDetails.BaseFare,avDetails.PlaceOfSupply);
                        ivID = Convert.ToInt32(ivPosted.Id);
                        status = "Posted";
                        statusMessage = "";

                        EntryCounter.GetInstance().IncreaseQBCount(1);

                        InsertUpdateVoucherPostingDetailsToCRS(avDetails.AgentID, avDetails.CompanyID, avDetails.BookingID, ivID, docnumber, docformattednumber, avDetails.BusMasterID,avDetails.DivisionID);

                        EntryCounter.GetInstance().IncreaseCRSCount(1);
                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("PostMissingAgentVouchers", "", ex.Message, true);
                    }

                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "PostMissingAgentVouchers:::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }

        }
            #endregion

            return;
        }


        #region Agent Voucher Deletion

        public void DeleteAgentVouchers()
        {
            #region Invoice Deletion From QB

            DataSet ds = new DataSet();

            ds = GetAgentVoucherDeleteDetailsFromCRS();
            int count = 0;

            if (ds.Tables[0].Rows.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                string result = "";
                string InvD;
                foreach (DataRow drDeleteDetails in ds.Tables[0].Rows)
                {
                 
                    string status = "";
                    string statusMessage = "";
                    Int32 iID = -1;
                    try
                    {
                        Invoice InvoiceData = new Invoice();
                        if (!drDeleteDetails["AccSysId"].ToString().Equals(""))
                        {
                            InvoiceData = Getinvoice(drDeleteDetails["AccSysId"].ToString());
                            //SalesItemLineDetail saleitem = new SalesItemLineDetail();
                         
                            if (InvoiceData != null)
                            {
                                //status = Convert.ToString(InvoiceData.status);
                                //InvD = InvoiceData.TxnDate.ToString("yyyy/MM/dd");
                                //saleitem = (SalesItemLineDetail)InvoiceData.Line[0].AnyIntuitObject;

                                //count++;
                                //result += Convert.ToString(InvoiceData.Id) + "~" + Convert.ToString(InvoiceData.CustomerRef.name) + "~" + InvD + "~" + saleitem.ItemRef.name + "~" + saleitem.ItemRef.Value + ",";

                                InvoiceData = DeleteInvoice(drDeleteDetails["AccSysId"].ToString(), InvoiceData.SyncToken);
                                iID = Convert.ToInt32(InvoiceData.Id);
                                status = "Posted";
                                statusMessage = "Deleted";
                                EntryCounter.GetInstance().IncreaseQBCount(1);
                                InsertUpdateVoucherDeleteDetailsToCRS(Convert.ToInt32(drDeleteDetails["bookingid"].ToString()));

                                EntryCounter.GetInstance().IncreaseCRSCount(1);
                            }

                        }

                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("DeleteAgentVouchers", "", ex.Message, true);
                    }

                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "DeleteAgentVouchers:::Mismatch in No Of Entries Deleted in QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }


            #endregion

            return;
        }


        public void CheckAgentVouchersInQB()
        {
            #region Voucher Posting in Invoice

            DataSet ds = new DataSet();

            ds = GetAgentVoucherDeleteDetailsFromCRS();
            int count = 0;

            if (ds.Tables[0].Rows.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                string result = "";
                string InvD;
                foreach (DataRow drCheckDetails in ds.Tables[0].Rows)
                {

                    string status = "";
                    string statusMessage = "";
                    Int32 iID = -1;
                    try
                    {
                        Invoice InvoiceData = new Invoice();
                        if (!drCheckDetails["docnumber"].ToString().Equals(""))
                        {
                            InvoiceData = Getinvoice(drCheckDetails["docnumber"].ToString());
                            SalesItemLineDetail saleitem = new SalesItemLineDetail();

                            if (InvoiceData != null)
                            {
                                status = Convert.ToString(InvoiceData.status);
                                InvD = InvoiceData.TxnDate.ToString("yyyy/MM/dd");
                                saleitem = (SalesItemLineDetail)InvoiceData.Line[0].AnyIntuitObject;
                                

                                count++;
                                //result += Convert.ToString(InvoiceData.Id) + "~" + Convert.ToString(InvoiceData.CustomerRef.name) + "~" + InvD + "~" + saleitem.ItemRef.name + "~" + saleitem.ItemRef.Value + ",";
                                //result += Convert.ToString(InvoiceData.Id) + "~" + Convert.ToString(InvoiceData.DocNumber) + ",";
                                InvoiceData = DeleteInvoice(Convert.ToString(InvoiceData.Id), InvoiceData.SyncToken);
                                
                                //InsertInvoiceItemDetails(Convert.ToInt32(InvoiceData.Id), Convert.ToInt32(saleitem.ItemRef.Value));

                            }

                        }

                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("CheckAgentVouchers", "", ex.Message, true);
                    }

                }

            }


            #endregion

            return;
        }


        public void CheckPostingInQB()
        {
            #region QB Connection

            

           
                EntryCounter.GetInstance().ResetAllCount();
                string result = "";
                string InvD;
               

                    string status = "";
                    string statusMessage = "";
                    Int32 iID = -1;
                    try
                    {
                        Logger.WriteLog("Entry");
                        Account AccountData = new Account();

                      AccountData = Getledgers("1");
                           
                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("CheckAgentVouchersInQB", "", ex.Message, true);
                    }

            #endregion

            return;
        }

        #endregion
        private List<AgentVoucherDetails> GetAgentVoucherDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();
                DateTime fromDate = new DateTime(2016, 06, 13);
                DateTime toDate = new DateTime(2016, 06, 13);
                dal.AddParameter("p_CompanyID", 69, ParameterDirection.Input);
                dal.AddParameter("p_FromDate", fromDate, ParameterDirection.Input);
                dal.AddParameter("p_ToDate", toDate, ParameterDirection.Input);
             
                DataSet dstOutPut = dal.ExecuteSelect("spGetVoucherForQuickBooks_Konduskar", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

               // List<AgentVoucherDetails> InvoiceNotPostedList = new List<AgentVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0 && dstOutPut.Tables[0].Rows[0]["BookingIDs"].ToString()!="0")
                {

                    string BookingIDs = dstOutPut.Tables[0].Rows[0]["BookingIDs"].ToString();
                    
                    Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation:: Agent Vouchers Not Posted : " + BookingIDs);
                    string msg = "GetAgentVoucherDetailsFromCRS::Validation:: Agent Vouchers Not Posted  : " + BookingIDs;
                    //Email.SendMail(msg);
                    //return InvoiceNotPostedList;

                }

                //else
                //{
                    List<AgentVoucherDetails> agentVoucherDetailsList = new List<AgentVoucherDetails>();
                    if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[1].Rows.Count > 0)
                    {

                        foreach (DataRow drAgentVoucherDetails in dstOutPut.Tables[1].Rows)
                        {
                            AgentVoucherDetails agentVoucherDetails = new AgentVoucherDetails();
                            try
                            {
                            Int32 qbCustomerId,divisionId, tripid, Merge ;
                            qbCustomerId = 0;
                            divisionId = 0;
                            tripid = 0;
                            Merge = 0;
                                if (!Int32.TryParse(drAgentVoucherDetails["QuickBookCustomerID"].ToString(), out qbCustomerId))
                                {
                                    Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                    string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                    Email.SendMail(msg);

                                }
                            else if (!Int32.TryParse(drAgentVoucherDetails["divisionid"].ToString(), out divisionId))
                            {
                                Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid DivisionId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid DivisionId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }

                            else if (!Int32.TryParse(drAgentVoucherDetails["ItemID"].ToString(), out tripid))
                                {
                                    Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                    string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                    Email.SendMail(msg);
                                }
                                else if (!Int32.TryParse(drAgentVoucherDetails["IsMerge"].ToString(), out Merge))
                                {
                                    Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid Classid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " BusId: " + drAgentVoucherDetails["BusId"].ToString());
                                    string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                    Email.SendMail(msg);
                                }
                                else
                                {
                                    agentVoucherDetails.AgentID = Convert.ToInt32(drAgentVoucherDetails["AgentID"].ToString());
                                    agentVoucherDetails.AgentName = drAgentVoucherDetails["AgentName"].ToString();
                                    agentVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drAgentVoucherDetails["QuickBookCustomerID"].ToString());
                                    agentVoucherDetails.FromCityName = drAgentVoucherDetails["FromCityName"].ToString();
                                    agentVoucherDetails.ToCityName = drAgentVoucherDetails["ToCityName"].ToString();
                                    agentVoucherDetails.RouteFromCityName = drAgentVoucherDetails["RouteFromCityName"].ToString();
                                    agentVoucherDetails.RouteToCityName = drAgentVoucherDetails["RouteToCityName"].ToString();
                                    agentVoucherDetails.BusNumber = drAgentVoucherDetails["BusNumber"].ToString();
                                    agentVoucherDetails.BusType = drAgentVoucherDetails["ChartName"].ToString();
                                    agentVoucherDetails.AgentPhone1 = drAgentVoucherDetails["ContactNo1"].ToString();
                                    agentVoucherDetails.AgentPhone2 = drAgentVoucherDetails["ContactNo2"].ToString();
                                    agentVoucherDetails.Amount = Convert.ToDecimal(drAgentVoucherDetails["NetAmt"].ToString());
                                    agentVoucherDetails.PNR = drAgentVoucherDetails["PNR"].ToString();
                                    agentVoucherDetails.PassengerName = drAgentVoucherDetails["PassengerName"].ToString();
                                    agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                    agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                    agentVoucherDetails.FromTo = drAgentVoucherDetails["FromTo"].ToString();
                                    agentVoucherDetails.JDate = drAgentVoucherDetails["JourneyDate"].ToString();
                                    agentVoucherDetails.JTime = drAgentVoucherDetails["JTime"].ToString();
                                    agentVoucherDetails.BDate = drAgentVoucherDetails["BookingDate"].ToString();
                                    agentVoucherDetails.BookingID = Convert.ToInt32(drAgentVoucherDetails["BookingID"].ToString());
                                    agentVoucherDetails.GeneratedDate = Convert.ToDateTime(drAgentVoucherDetails["GeneratedDate"].ToString());
                                    agentVoucherDetails.TransactionID = Convert.ToInt32(drAgentVoucherDetails["TransactionID"].ToString());
                                    if (drAgentVoucherDetails["ItemID"].ToString() != "")
                                        agentVoucherDetails.ItemID = Convert.ToInt32(drAgentVoucherDetails["ItemID"].ToString());
                                    agentVoucherDetails.ItemName = drAgentVoucherDetails["Item"].ToString();
                                    agentVoucherDetails.ClassID = drAgentVoucherDetails["ClassID"].ToString();
                                    agentVoucherDetails.ClassName = drAgentVoucherDetails["classname"].ToString();
                                    agentVoucherDetails.BranchDivisionID = drAgentVoucherDetails["BranchDivisionID"].ToString();
                                    agentVoucherDetails.BranchDivisionName = drAgentVoucherDetails["BranchDivisionName"].ToString();
                                    agentVoucherDetails.CompanyID = Convert.ToInt32(drAgentVoucherDetails["CompanyID"].ToString());
                                    agentVoucherDetails.VoucherNo = drAgentVoucherDetails["VoucherNo"].ToString();
                                    agentVoucherDetails.BookingStatus = drAgentVoucherDetails["BookingStatus"].ToString();
                                    agentVoucherDetails.Prefix = drAgentVoucherDetails["Prefix"].ToString();

                                    agentVoucherDetails.TotalFare = Convert.ToDecimal(drAgentVoucherDetails["TotalFare"].ToString());
                                    agentVoucherDetails.RefundAmount = Convert.ToDecimal(drAgentVoucherDetails["RefundAmount"].ToString());

                                   // agentVoucherDetails.DivisionID = Convert.ToInt32(drAgentVoucherDetails["divisionid"].ToString());
                                    agentVoucherDetails.BusMasterID = Convert.ToInt32(drAgentVoucherDetails["BusMasterId"].ToString());
                                    agentVoucherDetails.VoucherDate = Convert.ToDateTime(drAgentVoucherDetails["VoucherDate"].ToString());
                                    agentVoucherDetails.PickUpName = drAgentVoucherDetails["pickupname"].ToString();
                                    agentVoucherDetails.DropOffName = drAgentVoucherDetails["dropoffname"].ToString();
                                    agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                    agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                    agentVoucherDetails.GST = Convert.ToDecimal(drAgentVoucherDetails["GST"].ToString());
                                    agentVoucherDetails.AgentComm = Convert.ToDecimal(drAgentVoucherDetails["AgentComm"].ToString());
                                    agentVoucherDetails.GSTType = drAgentVoucherDetails["GSTType"].ToString();
                                    agentVoucherDetails.BaseFare = Convert.ToDecimal(drAgentVoucherDetails["Basefare"].ToString());
                                    agentVoucherDetails.PlaceOfSupply = drAgentVoucherDetails["agentstatename"].ToString();

                                agentVoucherDetailsList.Add(agentVoucherDetails);
                                }

                            }

                            catch (Exception ex)
                            {
                                Logger.WriteLog("GetAgentVoucherDetailsFromCRS", " AgentId: " + agentVoucherDetails.AgentID + " VoucherNo: " + agentVoucherDetails.VoucherNo, ex.Message, true);
                            }

                        }

                    }


                    return agentVoucherDetailsList;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        


        private List<AgentVoucherDetails> GetMissingAgentVoucherDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();
                DateTime fromDate = new DateTime(2016, 06, 13);
                DateTime toDate = new DateTime(2016, 06, 13);
                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
               
               
                DataSet dstOutPut = dal.ExecuteSelect("spGetMissingAgentVoucher", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

               

                 List<AgentVoucherDetails> agentVoucherDetailsList = new List<AgentVoucherDetails>();
                    if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                    {

                        foreach (DataRow drAgentVoucherDetails in dstOutPut.Tables[0].Rows)
                        {
                            AgentVoucherDetails agentVoucherDetails = new AgentVoucherDetails();
                            try
                            {
                                Int32 qbCustomerId, divisionId,tripid , Merge;
                            qbCustomerId = 0; divisionId = 0; tripid = 0; Merge = 0; 
                            if (!Int32.TryParse(drAgentVoucherDetails["QuickBookCustomerID"].ToString(), out qbCustomerId))
                                {
                                    Logger.WriteLog("spGetMissingAgentVoucher::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                    string msg = "spGetMissingAgentVoucher::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                    Email.SendMail(msg);

                                }
                                else if (!Int32.TryParse(drAgentVoucherDetails["divisionid"].ToString(), out divisionId))
                                {
                                    Logger.WriteLog("spGetMissingAgentVoucher::Validation::Invalid DivisionId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                    string msg = "spGetMissingAgentVoucher::Validation::Invalid DivisionId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                    Email.SendMail(msg);
                                }

                                else if (!Int32.TryParse(drAgentVoucherDetails["ItemID"].ToString(), out tripid))
                                {
                                    Logger.WriteLog("spGetMissingAgentVoucher::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                    string msg = "spGetMissingAgentVoucher::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                    Email.SendMail(msg);
                                }
                                else if (!Int32.TryParse(drAgentVoucherDetails["IsMerge"].ToString(), out Merge))
                                {
                                    Logger.WriteLog("spGetMissingAgentVoucher::Validation::Invalid Classid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " BusId: " + drAgentVoucherDetails["BusId"].ToString());
                                    string msg = "spGetMissingAgentVoucher::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                    Email.SendMail(msg);
                                }
                                else
                                {
                                    agentVoucherDetails.AgentID = Convert.ToInt32(drAgentVoucherDetails["AgentID"].ToString());
                                    agentVoucherDetails.AgentName = drAgentVoucherDetails["AgentName"].ToString();
                                    agentVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drAgentVoucherDetails["QuickBookCustomerID"].ToString());
                                    agentVoucherDetails.FromCityName = drAgentVoucherDetails["FromCityName"].ToString();
                                    agentVoucherDetails.ToCityName = drAgentVoucherDetails["ToCityName"].ToString();
                                    agentVoucherDetails.RouteFromCityName = drAgentVoucherDetails["RouteFromCityName"].ToString();
                                    agentVoucherDetails.RouteToCityName = drAgentVoucherDetails["RouteToCityName"].ToString();
                                    agentVoucherDetails.BusNumber = drAgentVoucherDetails["BusNumber"].ToString();
                                    agentVoucherDetails.BusType = drAgentVoucherDetails["ChartName"].ToString();
                                    agentVoucherDetails.AgentPhone1 = drAgentVoucherDetails["ContactNo1"].ToString();
                                    agentVoucherDetails.AgentPhone2 = drAgentVoucherDetails["ContactNo2"].ToString();
                                    agentVoucherDetails.Amount = Convert.ToDecimal(drAgentVoucherDetails["NetAmt"].ToString());
                                    agentVoucherDetails.PNR = drAgentVoucherDetails["PNR"].ToString();
                                    agentVoucherDetails.PassengerName = drAgentVoucherDetails["PassengerName"].ToString();
                                    agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                    agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                    agentVoucherDetails.FromTo = drAgentVoucherDetails["FromTo"].ToString();
                                    agentVoucherDetails.JDate = drAgentVoucherDetails["JourneyDate"].ToString();
                                    agentVoucherDetails.JTime = drAgentVoucherDetails["JTime"].ToString();
                                    agentVoucherDetails.BDate = drAgentVoucherDetails["BookingDate"].ToString();
                                    agentVoucherDetails.BookingID = Convert.ToInt32(drAgentVoucherDetails["BookingID"].ToString());
                                    agentVoucherDetails.GeneratedDate = Convert.ToDateTime(drAgentVoucherDetails["GeneratedDate"].ToString());
                                    agentVoucherDetails.TransactionID = Convert.ToInt32(drAgentVoucherDetails["TransactionID"].ToString());
                                    if (drAgentVoucherDetails["ItemID"].ToString() != "")
                                        agentVoucherDetails.ItemID = Convert.ToInt32(drAgentVoucherDetails["ItemID"].ToString());
                                    agentVoucherDetails.ItemName = drAgentVoucherDetails["Item"].ToString();
                                    agentVoucherDetails.ClassID = drAgentVoucherDetails["ClassID"].ToString();
                                    agentVoucherDetails.ClassName = drAgentVoucherDetails["classname"].ToString();
                                    agentVoucherDetails.BranchDivisionID = drAgentVoucherDetails["BranchDivisionID"].ToString();
                                    agentVoucherDetails.BranchDivisionName = drAgentVoucherDetails["BranchDivisionName"].ToString();
                                    agentVoucherDetails.CompanyID = Convert.ToInt32(drAgentVoucherDetails["CompanyID"].ToString());
                                    agentVoucherDetails.VoucherNo = drAgentVoucherDetails["VoucherNo"].ToString();
                                    agentVoucherDetails.BookingStatus = drAgentVoucherDetails["BookingStatus"].ToString();
                                    agentVoucherDetails.Prefix = drAgentVoucherDetails["Prefix"].ToString();

                                    agentVoucherDetails.TotalFare = Convert.ToDecimal(drAgentVoucherDetails["TotalFare"].ToString());
                                    agentVoucherDetails.RefundAmount = Convert.ToDecimal(drAgentVoucherDetails["RefundAmount"].ToString());

                                    agentVoucherDetails.DivisionID = Convert.ToInt32(drAgentVoucherDetails["divisionid"].ToString());
                                    agentVoucherDetails.BusMasterID = Convert.ToInt32(drAgentVoucherDetails["BusMasterId"].ToString());
                                    agentVoucherDetails.VoucherDate = Convert.ToDateTime(drAgentVoucherDetails["VoucherDate"].ToString());

                                agentVoucherDetailsList.Add(agentVoucherDetails);
                                }

                            }

                            catch (Exception ex)
                            {
                                Logger.WriteLog("spGetMissingAgentVoucher", " AgentId: " + agentVoucherDetails.AgentID + " VoucherNo: " + agentVoucherDetails.VoucherNo, ex.Message, true);
                            }

                        }

                    }


                    return agentVoucherDetailsList;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private DataSet GetAgentVoucherDeleteDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();
               
                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
             
                DataSet dstOutPut = dal.ExecuteSelect("spGetDataForDeletedAgentVouchers", CommandType.StoredProcedure, 0, ref strErr, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

        
                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private List<AgentVoucherDetails> GetAgentVoucherUpdateDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();
                DateTime fromDate = new DateTime(2016, 06, 13);
                DateTime toDate = new DateTime(2016, 06, 13);
                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
              
   
                DataSet dstOutPut = dal.ExecuteSelect("spGetUpdatedVoucherForQuickBooks_Konduskar", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<AgentVoucherDetails> agentVoucherDetailsList = new List<AgentVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drAgentVoucherDetails in dstOutPut.Tables[0].Rows)
                    {
                        AgentVoucherDetails agentVoucherDetails = new AgentVoucherDetails();
                        try
                        {
                            Int32 classid;
                            classid = 0;
                            if (!Int32.TryParse(drAgentVoucherDetails["classid"].ToString(), out classid))
                            {
                                Logger.WriteLog("GetAgentVoucherUpdateDetailsFromCRS::Validation::Bus is not schedule for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                //string msg = "GetAgentVoucherUpdateDetailsFromCRS::Validation::Bus is not schedule for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                //Email.SendMail(msg);

                            }


                                agentVoucherDetails.AgentID = Convert.ToInt32(drAgentVoucherDetails["AgentID"].ToString());
                                agentVoucherDetails.AgentName = drAgentVoucherDetails["AgentName"].ToString();
                                agentVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drAgentVoucherDetails["QuickBookCustomerID"].ToString());
                                agentVoucherDetails.FromCityName = drAgentVoucherDetails["FromCityName"].ToString();
                                agentVoucherDetails.ToCityName = drAgentVoucherDetails["ToCityName"].ToString();
                                agentVoucherDetails.RouteFromCityName = drAgentVoucherDetails["RouteFromCityName"].ToString();
                                agentVoucherDetails.RouteToCityName = drAgentVoucherDetails["RouteToCityName"].ToString();
                                agentVoucherDetails.BusNumber = drAgentVoucherDetails["BusNumber"].ToString();
                                agentVoucherDetails.BusType = drAgentVoucherDetails["ChartName"].ToString();
                                agentVoucherDetails.AgentPhone1 = drAgentVoucherDetails["ContactNo1"].ToString();
                                agentVoucherDetails.AgentPhone2 = drAgentVoucherDetails["ContactNo2"].ToString();
                                agentVoucherDetails.Amount = Convert.ToDecimal(drAgentVoucherDetails["NetAmt"].ToString());
                                agentVoucherDetails.PNR = drAgentVoucherDetails["PNR"].ToString();
                                agentVoucherDetails.PassengerName = drAgentVoucherDetails["PassengerName"].ToString();
                                agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                agentVoucherDetails.FromTo = drAgentVoucherDetails["FromTo"].ToString();
                                agentVoucherDetails.JDate = drAgentVoucherDetails["JourneyDate"].ToString();
                                agentVoucherDetails.JTime = drAgentVoucherDetails["JTime"].ToString();
                                agentVoucherDetails.BDate = drAgentVoucherDetails["BookingDate"].ToString();
                                agentVoucherDetails.BookingID = Convert.ToInt32(drAgentVoucherDetails["BookingID"].ToString());
                                agentVoucherDetails.GeneratedDate = Convert.ToDateTime(drAgentVoucherDetails["GeneratedDate"].ToString());
                                agentVoucherDetails.TransactionID = Convert.ToInt32(drAgentVoucherDetails["TransactionID"].ToString());
                                if (drAgentVoucherDetails["ItemID"].ToString() != "")
                                    agentVoucherDetails.ItemID = Convert.ToInt32(drAgentVoucherDetails["ItemID"].ToString());
                                agentVoucherDetails.ItemName = drAgentVoucherDetails["Item"].ToString();
                                agentVoucherDetails.ClassID = drAgentVoucherDetails["ClassID"].ToString();
                                agentVoucherDetails.ClassName = drAgentVoucherDetails["classname"].ToString();
                                agentVoucherDetails.BranchDivisionID = drAgentVoucherDetails["BranchDivisionID"].ToString();
                                agentVoucherDetails.BranchDivisionName = drAgentVoucherDetails["BranchDivisionName"].ToString();
                                agentVoucherDetails.CompanyID = Convert.ToInt32(drAgentVoucherDetails["CompanyID"].ToString());
                                agentVoucherDetails.VoucherNo = drAgentVoucherDetails["VoucherNo"].ToString();
                                agentVoucherDetails.BookingStatus = drAgentVoucherDetails["BookingStatus"].ToString();
                                agentVoucherDetails.Prefix = drAgentVoucherDetails["Prefix"].ToString();

                                agentVoucherDetails.TotalFare = Convert.ToDecimal(drAgentVoucherDetails["TotalFare"].ToString());
                                agentVoucherDetails.RefundAmount = Convert.ToDecimal(drAgentVoucherDetails["RefundAmount"].ToString());

                                agentVoucherDetails.DivisionID = Convert.ToInt32(drAgentVoucherDetails["divisionid"].ToString());
                                agentVoucherDetails.BusMasterID = Convert.ToInt32(drAgentVoucherDetails["BusMasterId"].ToString());
                                agentVoucherDetails.Accsysid = Convert.ToString(drAgentVoucherDetails["accsysinvoiceid"].ToString());
                                agentVoucherDetails.docnumber = Convert.ToString(drAgentVoucherDetails["docnumber"].ToString());
                                agentVoucherDetails.docformattednumber = Convert.ToString(drAgentVoucherDetails["docformattednumber"].ToString());
                                agentVoucherDetails.VoucherDate = Convert.ToDateTime(drAgentVoucherDetails["VoucherDate"].ToString());
                                agentVoucherDetails.PickUpName = drAgentVoucherDetails["pickupname"].ToString();
                                agentVoucherDetails.DropOffName = drAgentVoucherDetails["dropoffname"].ToString();
                                agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                agentVoucherDetails.GST = Convert.ToDecimal(drAgentVoucherDetails["GST"].ToString());
                                agentVoucherDetails.AgentComm = Convert.ToDecimal(drAgentVoucherDetails["AgentComm"].ToString());
                                agentVoucherDetails.GSTType = drAgentVoucherDetails["GSTType"].ToString();
                                agentVoucherDetails.BaseFare = Convert.ToDecimal(drAgentVoucherDetails["Basefare"].ToString());
                                agentVoucherDetails.PlaceOfSupply = drAgentVoucherDetails["agentstatename"].ToString();

                            agentVoucherDetailsList.Add(agentVoucherDetails);
                            
                          
                            

                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("GetAgentVoucherUpdateDetailsFromCRS", " AgentId: " + agentVoucherDetails.AgentID + " VoucherNo: " + agentVoucherDetails.VoucherNo, ex.Message, true);
                        }

                    }

                }

                return agentVoucherDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private void InsertUpdateVoucherPostingDetailsToCRS(Int32 AgentID, Int32 CompanyID, Int32 BookingID, Int32 AccSysID,string docnumber,string docformattednumber,int classid,int divisionid)
        {

            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_AgentID", AgentID, ParameterDirection.Input);
                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_BookingID", BookingID, ParameterDirection.Input);
                dal.AddParameter("p_AccSysID", AccSysID, ParameterDirection.Input);
                dal.AddParameter("p_docnumber", docnumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_docformattednumber", "INVCRS" + docformattednumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_classid", classid, ParameterDirection.Input);
                dal.AddParameter("p_divisionid", divisionid, ParameterDirection.Input);

                dal.ExecuteDML("spInsertUpdateQuickBookVoucherStatus", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception)
            {
                throw;
            }
        }




        private void InsertUpdateVoucherDeleteDetailsToCRS(Int32 BookingID)
        {

            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
             
               
                dal.AddParameter("p_BookingID", BookingID, ParameterDirection.Input);
               

                dal.ExecuteDML("spInsertUpdateQuickBookDeletedVoucherStatus", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreateVoucherNoForQB(Int32 CompanyID, Int32 DivisionID, Int32 BookingID,DateTime GeneratedDate, ref string docnumber,ref string docformattednumber,string Type)
        {

            try
            {
                string strErr = "";
                //string strVoucherNo = "";
                DataSet ds = null;
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_DivisionID", DivisionID, ParameterDirection.Input);
                dal.AddParameter("p_BookingID", BookingID, ParameterDirection.Input);
                dal.AddParameter("p_GeneratedDateTime", GeneratedDate, ParameterDirection.Input);

                //ds = dal.ExecuteSelect("spCreateAgentVoucherNoforQB_V2", CommandType.StoredProcedure, 0, ref strErr);

                //if (ds != null && ds.Tables.Count >= 1)
                //{
                    //docnumber = ds.Tables[0].Rows[0]["docnumber"].ToString();
                    //docformattednumber = ds.Tables[0].Rows[0]["docformattednumber"].ToString();

                    docnumber = " ";
                if (Type =="AV")
                {
                    docformattednumber = BookingID.ToString() ;
                }
                else if (Type == "CN")
                {
                    docformattednumber = BookingID.ToString();
                }
                else if (Type == "SR")
                {
                    docformattednumber = BookingID.ToString();
                }

                //}
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Invoice PostInvoice(string action,int AgentID,string invoiceid,string synctoken, string AgentName, int QuickBookCustomerID, string FromCityName, string ToCityName, string RouteFromCityName, string RouteToCityName, string BusNumber, string AgentPhone1, string AgentPhone2, decimal Amount, string PNR, string PassengerName, string SeatNos, int SeatCount, string FromTo, string JDate, string JTime, string BDate, int BookingID, DateTime VoucherDate, int TransactionID, string BusType, string TripCode, string ItemName, int intItemID, string ClassName, string ClassID, string DivisionName, string DivisionID, string VoucherNo, string BookingStatus, string Prefix, decimal TotalFare, Decimal RefundAmount,string PickUpName,string DropOffName,decimal GST,decimal AgentComm,string GSTType,decimal Basefare,string PlaceOfSupply)
       
        {
            try
            {

                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                decimal Lineamount = 0;
                int TransactionLocationTypeid = 0;
                /* Invoice Begin */
                //customer name, date, payment method, deposit to, service, description, Qty, Rate, Amout , Tax

                var invoice = new Invoice();
                {
                    invoice.TxnDate = VoucherDate;
                    invoice.TxnDateSpecified = true;


                    invoice.CustomerRef = new ReferenceType() { name = AgentName, Value = QuickBookCustomerID.ToString() };
                    if (GSTType == "IGST")
                    {
                        TransactionLocationTypeid = GetPlaceOfSupply(PlaceOfSupply);
                        invoice.TransactionLocationType = TransactionLocationTypeid.ToString();
                    }
                    

                    //invoice.SalesTermRef = new ReferenceType() { name = "Due on receipt", Value = "9" };

                    if (DivisionName != "")
                        invoice.DepartmentRef = new ReferenceType() { name = DivisionName, Value = DivisionID }; //agent area

                    List<CustomField> customFieldList = new List<CustomField>();
                    var pickupdrop = new CustomField();
                    {
                        pickupdrop.DefinitionId = "1";
                        pickupdrop.Name = "PickUp-Drop";
                        pickupdrop.AnyIntuitObject = FromCityName + "-" + ToCityName;
                        customFieldList.Add(pickupdrop);
                    };

                    var JourneyDate = new CustomField();
                    {
                        JourneyDate.DefinitionId = "2";
                        JourneyDate.Name = "Journey Date";
                        JourneyDate.AnyIntuitObject = JDate;
                        customFieldList.Add(JourneyDate);
                    }

                    CustomField PhoneNumber = new CustomField();
                    {
                        PhoneNumber.DefinitionId = "3";
                        PhoneNumber.Name = "Phone Number";
                        PhoneNumber.AnyIntuitObject = AgentPhone1 + "," + AgentPhone2;
                        customFieldList.Add(PhoneNumber);
                    }

                    invoice.CustomField = customFieldList.ToArray();

                    List<Line> invoiceLineList = new List<Line>();

                    if (BookingStatus == "C")
                    {
                        SalesItemLineDetail saleItemDetail = new SalesItemLineDetail();

                        {

                            saleItemDetail.ItemRef = new ReferenceType() { name = ItemName, Value = intItemID.ToString() };

                            if (ClassName != "")
                                saleItemDetail.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };

                        }
                        string strDesc = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                            "Seats : " + SeatNos + "\n" +
                            "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                            "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                            "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                            + ",\nPickup Name: " + PickUpName + ",\nDropOff Name: " + DropOffName;

                        if (BookingStatus == "C")
                            strDesc = "Cancelled - " + strDesc;

                        Line invoiceLine = new Line();
                        {
                            invoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                            invoiceLine.DetailTypeSpecified = true;

                            invoiceLine.Amount = Amount;
                            invoiceLine.AmountSpecified = true;
                            invoiceLine.AnyIntuitObject = saleItemDetail;
                            invoiceLine.Description = strDesc;
                        }
                        Lineamount = Amount;

                        invoiceLineList.Add(invoiceLine);
                    }
                    else
                    {
                       

                           
                            SalesItemLineDetail saleItemDetail = new SalesItemLineDetail();
                            {
                                saleItemDetail.Qty = new Decimal(SeatCount);
                                saleItemDetail.QtySpecified = true;
                              
                                saleItemDetail.AnyIntuitObject = Basefare / SeatCount; // Amount / SeatCount; // 2500m;
                               

                                saleItemDetail.ItemElementName = ItemChoiceType.UnitPrice;



                                saleItemDetail.ItemRef = new ReferenceType() { name = ItemName, Value = intItemID.ToString() };

                                if (ClassName != "")
                                    saleItemDetail.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };
                                if (GSTType == "CGST-SGST")
                                {
                                    saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "17" };
                                }
                                else if(GSTType == "IGST")
                                {
                                    saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "6" };
                                }
                                else
                                {
                                    saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "24" };
                                }
                                

                            }
                            string strDesc = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                                "Seats : " + SeatNos + "\n" +
                                "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                                "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                                "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                                + ",\nPickup Name: " + PickUpName + ",\nDropOff Name: " + DropOffName;

                            if (BookingStatus == "C")
                                strDesc = "Cancelled - " + strDesc;

                            Line invoiceLine = new Line();
                            {
                                invoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                                invoiceLine.DetailTypeSpecified = true;
                            }



                            invoiceLine.Amount = Basefare;// 1000; // Amount;
                               // Lineamount += (bkgDetails.Fare - (Commissionperseatwise + discountperseatwise) + diffamt) * bkgDetails.SeatCount;
                           

                            invoiceLine.AmountSpecified = true;
                            invoiceLine.AnyIntuitObject = saleItemDetail;
                            invoiceLine.Description = strDesc;


                            invoiceLineList.Add(invoiceLine);


                            ///////////////////////////////////// For AgentCommission Tag /////////////////////////////////////
                            SalesItemLineDetail saleItemDetail1 = new SalesItemLineDetail();
                            {
                                saleItemDetail1.Qty = new Decimal(SeatCount);
                                saleItemDetail1.QtySpecified = true;

                                saleItemDetail1.AnyIntuitObject = - (AgentComm / SeatCount); // Amount / SeatCount; // 2500m;


                                saleItemDetail1.ItemElementName = ItemChoiceType.UnitPrice;



                                saleItemDetail1.ItemRef = new ReferenceType() { name = "Commision", Value = "330" };

                                //if (ClassName != "")
                                //    saleItemDetail1.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };
                                if (GSTType == "CGST-SGST")
                                {
                                    saleItemDetail1.TaxCodeRef = new ReferenceType() { Value = "24" };
                                }
                                else if (GSTType == "IGST")
                                {
                                    saleItemDetail1.TaxCodeRef = new ReferenceType() { Value = "22" };
                                }
                                else
                                {
                                    saleItemDetail1.TaxCodeRef = new ReferenceType() { Value = "24" };
                                }
                                

                            }
                            string strDesc1 = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                                "Seats : " + SeatNos + "\n" +
                                "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                                "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                                "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                                + ",\nPickup Name: " + PickUpName + ",\nDropOff Name: " + DropOffName;

                            if (BookingStatus == "C")
                                strDesc = "Cancelled - " + strDesc1;

                            Line invoiceLine1 = new Line();
                            {
                                invoiceLine1.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                                invoiceLine1.DetailTypeSpecified = true;
                            }



                            invoiceLine1.Amount = - AgentComm;


                            invoiceLine1.AmountSpecified = true;
                            invoiceLine1.AnyIntuitObject = saleItemDetail1;
                            invoiceLine1.Description = strDesc1;


                            invoiceLineList.Add(invoiceLine1);


                        
                       
                    }
                    invoice.Line = invoiceLineList.ToArray();

                    invoice.TotalAmt = Basefare;
                    invoice.TotalAmtSpecified = true;


                    //TxnTaxDetail Details
                    
                    TxnTaxDetail txnTaxdetail = new TxnTaxDetail();

                    txnTaxdetail.TotalTaxSpecified = true;
                    txnTaxdetail.TotalTax = GST;
                    List<Line> taxlinelist = new List<Line>();

                    if (GSTType == "CGST-SGST")
                    {
                        //List<Line> taxlinelist = new List<Line>();

                        Line taxline = new Line();
                        taxline.DetailType = LineDetailTypeEnum.TaxLineDetail;
                        taxline.DetailTypeSpecified = true;
                        taxline.Amount = GST/2;
                        taxline.AmountSpecified = true;
                        TaxLineDetail taxLinedetail = new TaxLineDetail();

                        taxLinedetail.TaxRateRef = new ReferenceType() { Value = "69" };
                        taxLinedetail.NetAmountTaxable = Basefare;
                        taxLinedetail.NetAmountTaxableSpecified = true;
                        taxLinedetail.TaxPercent = 2.5M;
                        taxLinedetail.TaxPercentSpecified = true;
                        taxLinedetail.PercentBased = true;
                        taxline.AnyIntuitObject = taxLinedetail;
                        //txnTaxdetail.TaxLine = new Line[] { taxline };

                        Line taxline1 = new Line();
                        taxline1.DetailType = LineDetailTypeEnum.TaxLineDetail;
                        taxline1.DetailTypeSpecified = true;
                        taxline1.Amount = GST/2;
                        taxline1.AmountSpecified = true;
                        TaxLineDetail taxLinedetail1 = new TaxLineDetail();

                        taxLinedetail1.TaxRateRef = new ReferenceType() { Value = "70" };
                        taxLinedetail1.NetAmountTaxable = Basefare;
                        taxLinedetail1.NetAmountTaxableSpecified = true;
                        taxLinedetail1.TaxPercent = 2.5M;
                        taxLinedetail1.TaxPercentSpecified = true;
                        taxLinedetail1.PercentBased = true;
                        taxline1.AnyIntuitObject = taxLinedetail1;



                        Line taxline2 = new Line();
                        taxline2.DetailType = LineDetailTypeEnum.TaxLineDetail;
                        taxline2.DetailTypeSpecified = true;
                        taxline2.Amount = 0;
                        taxline2.AmountSpecified = true;
                        TaxLineDetail taxLinedetail2 = new TaxLineDetail();

                        taxLinedetail2.TaxRateRef = new ReferenceType() { Value = "83" };
                        taxLinedetail2.NetAmountTaxable = - AgentComm;
                        taxLinedetail2.NetAmountTaxableSpecified = true;
                        taxLinedetail2.TaxPercent = 0;
                        taxLinedetail2.TaxPercentSpecified = true;
                        taxLinedetail2.PercentBased = true;
                        taxline2.AnyIntuitObject = taxLinedetail2;



                        Line taxline3 = new Line();
                        taxline3.DetailType = LineDetailTypeEnum.TaxLineDetail;
                        taxline3.DetailTypeSpecified = true;
                        taxline3.Amount = 0;
                        taxline3.AmountSpecified = true;
                        TaxLineDetail taxLinedetail3 = new TaxLineDetail();

                        taxLinedetail3.TaxRateRef = new ReferenceType() { Value = "84" };
                        taxLinedetail3.NetAmountTaxable = - AgentComm;
                        taxLinedetail3.NetAmountTaxableSpecified = true;
                        taxLinedetail3.TaxPercent = 0;
                        taxLinedetail3.TaxPercentSpecified = true;
                        taxLinedetail3.PercentBased = true;
                        taxline3.AnyIntuitObject = taxLinedetail3;

                        taxlinelist.Add(taxline);
                        taxlinelist.Add(taxline1);
                        taxlinelist.Add(taxline2);
                        taxlinelist.Add(taxline3);
                    }
                    else if (GSTType == "IGST")
                    {

                       

                        Line taxline = new Line();
                        taxline.DetailType = LineDetailTypeEnum.TaxLineDetail;
                        taxline.DetailTypeSpecified = true;
                        taxline.Amount = GST;
                        taxline.AmountSpecified = true;
                        TaxLineDetail taxLinedetail = new TaxLineDetail();

                        taxLinedetail.TaxRateRef = new ReferenceType() { Value = "18" };
                        taxLinedetail.NetAmountTaxable = Basefare;
                        taxLinedetail.NetAmountTaxableSpecified = true;
                        taxLinedetail.TaxPercent = 5;
                        taxLinedetail.TaxPercentSpecified = true;
                        taxLinedetail.PercentBased = true;
                        taxline.AnyIntuitObject = taxLinedetail;
                        //txnTaxdetail.TaxLine = new Line[] { taxline };

                        Line taxline1 = new Line();
                        taxline1.DetailType = LineDetailTypeEnum.TaxLineDetail;
                        taxline1.DetailTypeSpecified = true;
                        taxline1.Amount = 0;
                        taxline1.AmountSpecified = true;
                        TaxLineDetail taxLinedetail1 = new TaxLineDetail();

                        taxLinedetail1.TaxRateRef = new ReferenceType() { Value = "80" };
                        taxLinedetail1.NetAmountTaxable = - AgentComm;
                        taxLinedetail1.NetAmountTaxableSpecified = true;
                        taxLinedetail1.TaxPercent = 0;
                        taxLinedetail1.TaxPercentSpecified = true;
                        taxLinedetail1.PercentBased = true;
                        taxline1.AnyIntuitObject = taxLinedetail1;

                        taxlinelist.Add(taxline);
                        taxlinelist.Add(taxline1);
                    }
                  

                    //invoice.Line = invoiceLineList.ToArray();
                    txnTaxdetail.TaxLine = taxlinelist.ToArray();

                    invoice.TxnTaxDetail = txnTaxdetail;
                    







                    invoice.GlobalTaxCalculation = GlobalTaxCalculationEnum.TaxExcluded;
                    invoice.GlobalTaxCalculationSpecified = true;


                

                    invoice.DocNumber = VoucherNo;
                    string strNo = "[Voucher No : " + VoucherNo + "]";

                    if (BookingStatus == "C")
                        invoice.PrivateNote = strNo + ", Cancelled - PNR : " + BookingID + " -- Automaticaly posted from CRS";
                    else
                        invoice.PrivateNote = strNo + ", PNR : " + BookingID + " -- Automaticaly posted from CRS";


                    if (action.Equals("Update"))
                    {
                        invoice.Id = invoiceid;
                        invoice.SyncToken = synctoken;
                    }
                }
                Invoice postedInvoice = null;

              
                
               postedInvoice = service.Add(invoice);
                
               

                return postedInvoice;
                /* Invoice End*/

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }


        public static int GetPlaceOfSupply(string PlaceOfSupply)
        {
            int LocationTypId = 0;
            switch (PlaceOfSupply)
            {

                case "ANDHRA PRADESH": LocationTypId = 37; break;
                case "ANDAMAN AND NICOBAR ISLANDS": LocationTypId = 35; break;
                case "ARUNACHAL PRADESH": LocationTypId = 12; break;
                case "ASSAM": LocationTypId = 18; break;
                case "BIHAR": LocationTypId = 10; break;
                case "CHANDIGARH": LocationTypId = 04; break;
                case "CHHATTISGARH": LocationTypId = 22; break;
                case "DADRA AND NAGAR HAVELI": LocationTypId =26; break;
                case "DAMAN AND DIU": LocationTypId = 25; break;
                case "DELHI": LocationTypId = 07; break;
                case "GOA": LocationTypId = 30; break;
                case "GUJARAT": LocationTypId = 24; break;
                case "HARYANA": LocationTypId = 06; break;
                case "HIMACHAL PRADESH": LocationTypId = 02; break;
                case "JAMMU AND KASHMIR": LocationTypId = 01; break;
                case "JHARKHAND": LocationTypId = 20; break;
                case "KARNATAKA": LocationTypId = 29; break;
                case "KERALA": LocationTypId = 32; break;
                case "LAKSHADWEEP": LocationTypId = 31; break;
                case "MADHYA PRADESH": LocationTypId = 23; break;
                case "MAHARASHTRA": LocationTypId = 27; break;
                case "MANIPUR": LocationTypId = 14; break;
                case "MEGHALAYA": LocationTypId = 17; break;
                case "MIZORAM": LocationTypId = 15; break;
                case "NAGALAND": LocationTypId = 13; break;
                case "ODISHA": LocationTypId = 21; break;
                case "PONDICHERRY": LocationTypId = 34; break;
                case "PUNJAB": LocationTypId = 03; break;
                case "RAJASTHAN": LocationTypId = 08; break;
                case "SIKKIM": LocationTypId = 11; break;
                case "TAMIL NADU": LocationTypId = 33; break;
                case "TELANGANA": LocationTypId = 36; break;
                case "TRIPURA": LocationTypId = 16; break;
                case "UTTAR_PRADESH": LocationTypId = 09; break;
                case "UTTARAKHAND": LocationTypId = 05; break;
                case "WEST_BENGAL": LocationTypId = 19; break;



            }
            return LocationTypId;
        }


        private Invoice DeleteInvoice(string QuickBookCustomerID,string SyncToken)
        {
            try
            {


                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Invoice invoice = new Invoice();

                {
                    invoice.Id = QuickBookCustomerID;
                    invoice.SyncToken = SyncToken;
                }

                Invoice postedInvoice = service.Delete(invoice);

                return postedInvoice;
               

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        private SalesReceipt PostSaleEntry(string action, int AgentID, string SalesReceiptID, string synctoken, string AgentName, int QuickBookCustomerID, string FromCityName, string ToCityName, string RouteFromCityName, string RouteToCityName, string BusNumber, string AgentPhone1, string AgentPhone2, decimal Amount, string PNR, string PassengerName, string SeatNos, int SeatCount, string FromTo, string JDate, string JTime, string BDate, int BookingID, DateTime VoucherDate, int TransactionID, string BusType, string TripCode, string ItemName, int intItemID, string ClassName, string ClassID, string DivisionName, string DivisionID, string VoucherNo, string BookingStatus, string Prefix, decimal TotalFare, Decimal RefundAmount,string PickUpname,string DropOffName,Int32 LedgerId,string LedgerName,decimal BaseFare,decimal GST,string GSTType,string PlaceOfSupply,decimal BranchComm)
        {
            try
            {

                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                decimal Lineamount = 0;
                int TransactionLocationTypeid = 0;
                /* Invoice Begin */
                //customer name, date, payment method, deposit to, service, description, Qty, Rate, Amout , Tax

                SalesReceipt Saleinvoice = new SalesReceipt();
                {
                    Saleinvoice.TxnDate = VoucherDate;
                    Saleinvoice.TxnDateSpecified = true;
                }


                Saleinvoice.CustomerRef = new ReferenceType() { name = AgentName, Value = QuickBookCustomerID.ToString() };
                Saleinvoice.DepositToAccountRef = new ReferenceType() { name = LedgerName, Value = LedgerId.ToString() };
                Saleinvoice.PaymentMethodRef = new ReferenceType() { name = "Cash", Value = "1" };
                //Saleinvoice.SalesTermRef = new ReferenceType() { name = "Due on receipt", Value = "9" };

                if (DivisionName != "")
                    Saleinvoice.DepartmentRef = new ReferenceType() { name = DivisionName, Value = DivisionID }; //agent area

                if (GSTType == "IGST")
                {
                    TransactionLocationTypeid = GetPlaceOfSupply(PlaceOfSupply);
                    Saleinvoice.TransactionLocationType = TransactionLocationTypeid.ToString();
                }


                List<CustomField> customFieldList = new List<CustomField>();
                
                    CustomField pickupdrop = new CustomField();
                { 
                    pickupdrop.DefinitionId = "1";
                    pickupdrop.Name = "PickUp-Drop";
                    pickupdrop.AnyIntuitObject = FromCityName + "-" + ToCityName;
                    customFieldList.Add(pickupdrop);
                }

                CustomField JourneyDate = new CustomField();
                {
                    JourneyDate.DefinitionId = "2";
                    JourneyDate.Name = "Journey Date";
                    JourneyDate.AnyIntuitObject = JDate;
                    customFieldList.Add(JourneyDate);
                }

                CustomField PhoneNumber = new CustomField();
                {
                    PhoneNumber.DefinitionId = "3";
                    PhoneNumber.Name = "Phone Number";
                    PhoneNumber.AnyIntuitObject = AgentPhone1 + "," + AgentPhone2;
                    customFieldList.Add(PhoneNumber);
                }

                Saleinvoice.CustomField = customFieldList.ToArray();

                List<Line> saleinvoiceLineList = new List<Line>();

                
                //else
                {
                    

                        
                        SalesItemLineDetail saleItemDetail = new SalesItemLineDetail();
                        {
                            saleItemDetail.Qty = new Decimal(SeatCount);
                            saleItemDetail.QtySpecified = true;

                            saleItemDetail.AnyIntuitObject = BaseFare/SeatCount;
                            
                      
                            

                            saleItemDetail.ItemElementName = ItemChoiceType.UnitPrice;



                            saleItemDetail.ItemRef = new ReferenceType() { name = ItemName, Value = intItemID.ToString() };

                            if (ClassName != "")
                                saleItemDetail.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };
                            if (GSTType == "CGST-SGST")
                            {
                                saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "17" };
                            }
                            else if (GSTType == "IGST")
                            {
                                saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "6" };
                            }
                            else
                            {
                                saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "24" };
                            }

                        }
                        string strDesc = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                            "Seats : " + SeatNos + "\n" +
                            "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                            "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                            "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                             + ",\nPickup Name: " + PickUpname + ",\nDropOff Name: " + DropOffName;

                        if (BookingStatus == "C")
                            strDesc = "Cancelled - " + strDesc;

                        Line saleinvoiceLine = new Line();
                        {
                            saleinvoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                            saleinvoiceLine.DetailTypeSpecified = true;

                            saleinvoiceLine.Amount = BaseFare; // 1000; // Amount;
                           // Lineamount += (bkgDetails.Fare - (Commissionperseatwise + discountperseatwise) + diffamt) * bkgDetails.SeatCount;



                            saleinvoiceLine.AmountSpecified = true;
                            saleinvoiceLine.AnyIntuitObject = saleItemDetail;
                            saleinvoiceLine.Description = strDesc;
                        }

                        saleinvoiceLineList.Add(saleinvoiceLine);



                        ///////////////////////////////////// For BranchCommission Tag /////////////////////////////////////
                        SalesItemLineDetail saleItemDetail1 = new SalesItemLineDetail();
                        {
                            saleItemDetail1.Qty = new Decimal(SeatCount);
                            saleItemDetail1.QtySpecified = true;

                            saleItemDetail1.AnyIntuitObject = (BranchComm / SeatCount); // Amount / SeatCount; // 2500m;


                            saleItemDetail1.ItemElementName = ItemChoiceType.UnitPrice;



                            saleItemDetail1.ItemRef = new ReferenceType() { name = "Branch Commission", Value = "331" };

                            //if (ClassName != "")
                            //    saleItemDetail1.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };
                            if (GSTType == "CGST-SGST")
                            {
                                saleItemDetail1.TaxCodeRef = new ReferenceType() { Value = "24" };
                            }
                            else if (GSTType == "IGST")
                            {
                                saleItemDetail1.TaxCodeRef = new ReferenceType() { Value = "22" };
                            }
                            else
                            {
                                saleItemDetail1.TaxCodeRef = new ReferenceType() { Value = "24" };
                            }


                        }
                        string strDesc1 = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                            "Seats : " + SeatNos + "\n" +
                            "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                            "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                            "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                            + ",\nPickup Name: " + PickUpname + ",\nDropOff Name: " + DropOffName;

                        if (BookingStatus == "C")
                            strDesc1 = "Cancelled - " + strDesc1;

                        Line saleinvoiceLine1 = new Line();
                        {
                          saleinvoiceLine1.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                          saleinvoiceLine1.DetailTypeSpecified = true;
                        }



                        saleinvoiceLine1.Amount = BranchComm;


                        saleinvoiceLine1.AmountSpecified = true;
                        saleinvoiceLine1.AnyIntuitObject = saleItemDetail1;
                        saleinvoiceLine1.Description = strDesc1;


                        saleinvoiceLineList.Add(saleinvoiceLine1);



                }


                Saleinvoice.TotalAmt = BaseFare;
                Saleinvoice.TotalAmtSpecified = true;


                //TxnTaxDetail
                TxnTaxDetail txnTaxdetail = new TxnTaxDetail();
                txnTaxdetail.TotalTaxSpecified = true;
                txnTaxdetail.TotalTax = GST;

                List<Line> taxlinelist = new List<Line>();

                if (GSTType == "CGST-SGST")
                {
                    //List<Line> taxlinelist = new List<Line>();

                    Line taxline = new Line();
                    taxline.DetailType = LineDetailTypeEnum.TaxLineDetail;
                    taxline.DetailTypeSpecified = true;
                    taxline.Amount = GST / 2;
                    taxline.AmountSpecified = true;
                    TaxLineDetail taxLinedetail = new TaxLineDetail();

                    taxLinedetail.TaxRateRef = new ReferenceType() { Value = "69" };
                    taxLinedetail.NetAmountTaxable = BaseFare;
                    taxLinedetail.NetAmountTaxableSpecified = true;
                    taxLinedetail.TaxPercent = 2.5M;
                    taxLinedetail.TaxPercentSpecified = true;
                    taxLinedetail.PercentBased = true;
                    taxline.AnyIntuitObject = taxLinedetail;
                    //txnTaxdetail.TaxLine = new Line[] { taxline };

                    Line taxline1 = new Line();
                    taxline1.DetailType = LineDetailTypeEnum.TaxLineDetail;
                    taxline1.DetailTypeSpecified = true;
                    taxline1.Amount = GST / 2;
                    taxline1.AmountSpecified = true;
                    TaxLineDetail taxLinedetail1 = new TaxLineDetail();

                    taxLinedetail1.TaxRateRef = new ReferenceType() { Value = "70" };
                    taxLinedetail1.NetAmountTaxable = BaseFare;
                    taxLinedetail1.NetAmountTaxableSpecified = true;
                    taxLinedetail1.TaxPercent = 2.5M;
                    taxLinedetail1.TaxPercentSpecified = true;
                    taxLinedetail1.PercentBased = true;
                    taxline1.AnyIntuitObject = taxLinedetail1;



                    Line taxline2 = new Line();
                    taxline2.DetailType = LineDetailTypeEnum.TaxLineDetail;
                    taxline2.DetailTypeSpecified = true;
                    taxline2.Amount = 0;
                    taxline2.AmountSpecified = true;
                    TaxLineDetail taxLinedetail2 = new TaxLineDetail();

                    taxLinedetail2.TaxRateRef = new ReferenceType() { Value = "83" };
                    taxLinedetail2.NetAmountTaxable = BranchComm;
                    taxLinedetail2.NetAmountTaxableSpecified = true;
                    taxLinedetail2.TaxPercent = 0;
                    taxLinedetail2.TaxPercentSpecified = true;
                    taxLinedetail2.PercentBased = true;
                    taxline2.AnyIntuitObject = taxLinedetail2;



                    Line taxline3 = new Line();
                    taxline3.DetailType = LineDetailTypeEnum.TaxLineDetail;
                    taxline3.DetailTypeSpecified = true;
                    taxline3.Amount = 0;
                    taxline3.AmountSpecified = true;
                    TaxLineDetail taxLinedetail3 = new TaxLineDetail();

                    taxLinedetail3.TaxRateRef = new ReferenceType() { Value = "84" };
                    taxLinedetail3.NetAmountTaxable = BranchComm;
                    taxLinedetail3.NetAmountTaxableSpecified = true;
                    taxLinedetail3.TaxPercent = 0;
                    taxLinedetail3.TaxPercentSpecified = true;
                    taxLinedetail3.PercentBased = true;
                    taxline3.AnyIntuitObject = taxLinedetail3;

                    taxlinelist.Add(taxline);
                    taxlinelist.Add(taxline1);
                    taxlinelist.Add(taxline2);
                    taxlinelist.Add(taxline3);
                }
                else if (GSTType == "IGST")
                {



                    Line taxline = new Line();
                    taxline.DetailType = LineDetailTypeEnum.TaxLineDetail;
                    taxline.DetailTypeSpecified = true;
                    taxline.Amount = GST;
                    taxline.AmountSpecified = true;
                    TaxLineDetail taxLinedetail = new TaxLineDetail();

                    taxLinedetail.TaxRateRef = new ReferenceType() { Value = "18" };
                    taxLinedetail.NetAmountTaxable = BaseFare;
                    taxLinedetail.NetAmountTaxableSpecified = true;
                    taxLinedetail.TaxPercent = 5;
                    taxLinedetail.TaxPercentSpecified = true;
                    taxLinedetail.PercentBased = true;
                    taxline.AnyIntuitObject = taxLinedetail;
                    //txnTaxdetail.TaxLine = new Line[] { taxline };

                    Line taxline1 = new Line();
                    taxline1.DetailType = LineDetailTypeEnum.TaxLineDetail;
                    taxline1.DetailTypeSpecified = true;
                    taxline1.Amount = 0;
                    taxline1.AmountSpecified = true;
                    TaxLineDetail taxLinedetail1 = new TaxLineDetail();

                    taxLinedetail1.TaxRateRef = new ReferenceType() { Value = "80" };
                    taxLinedetail1.NetAmountTaxable = BranchComm;
                    taxLinedetail1.NetAmountTaxableSpecified = true;
                    taxLinedetail1.TaxPercent = 0;
                    taxLinedetail1.TaxPercentSpecified = true;
                    taxLinedetail1.PercentBased = true;
                    taxline1.AnyIntuitObject = taxLinedetail1;

                    taxlinelist.Add(taxline);
                    taxlinelist.Add(taxline1);
                }


                //invoice.Line = invoiceLineList.ToArray();
                txnTaxdetail.TaxLine = taxlinelist.ToArray();

                Saleinvoice.TxnTaxDetail = txnTaxdetail;

                Saleinvoice.GlobalTaxCalculation = GlobalTaxCalculationEnum.TaxExcluded;
                Saleinvoice.GlobalTaxCalculationSpecified = true;


                Saleinvoice.Line = saleinvoiceLineList.ToArray();

                Saleinvoice.DocNumber = VoucherNo;
                string strNo = "[Voucher No : " + VoucherNo + "]";

                if (BookingStatus == "C")
                    Saleinvoice.PrivateNote = strNo + ", Cancelled - PNR : " + BookingID + " -- Automaticaly posted from CRS";
                else
                    Saleinvoice.PrivateNote = strNo + ", PNR : " + BookingID + " -- Automaticaly posted from CRS";


                if (action.Equals("Update"))
                {
                    Saleinvoice.Id = SalesReceiptID;
                    Saleinvoice.SyncToken = synctoken;
                }
                SalesReceipt postedSaleInvoice = null;

              
                  postedSaleInvoice = service.Add(Saleinvoice);
                
                return postedSaleInvoice;

               

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }


        private List<BookingDetails> GetBookingrDetailsFromCRS(int intBookingID)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_BookingID", intBookingID, ParameterDirection.Input);
                DataSet dstOutPut = dal.ExecuteSelect("spGetBookingDetailsForQuickBooksKonduskar", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false,"",false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<BookingDetails> BookingDetailsList = new List<BookingDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drBookingDetails in dstOutPut.Tables[0].Rows)
                    {
                        BookingDetails BookingDetails = new BookingDetails();
                        {
                            BookingDetails.SeatNos = drBookingDetails["SeatNos"].ToString();
                            BookingDetails.SeatCount = Convert.ToInt32(drBookingDetails["SeatCount"].ToString());
                            BookingDetails.Fare = Convert.ToDecimal(drBookingDetails["Fare"].ToString());
                            BookingDetails.Comm = Convert.ToDecimal(drBookingDetails["Comm"].ToString());
                            BookingDetails.AgentComm = Convert.ToDecimal(drBookingDetails["AgentComm"].ToString());
                            BookingDetails.Disc = Convert.ToDecimal(drBookingDetails["Disc"].ToString());
                            BookingDetails.TotalLineAmt = Convert.ToDecimal(dstOutPut.Tables[1].Rows[0]["TotalFare"].ToString());
                            BookingDetails.ActualAmt = Convert.ToDecimal(dstOutPut.Tables[1].Rows[0]["TotalVoucherAmt"].ToString());
                        }
                        BookingDetailsList.Add(BookingDetails);
                    
                    }
                   


                }
              
                return BookingDetailsList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private List<BookingDetails> GetSalesReceiptBookingrDetailsFromCRS(int intBookingID)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_BookingID", intBookingID, ParameterDirection.Input);
                DataSet dstOutPut = dal.ExecuteSelect("spGetBookingDetailsForQuickBooksKonduskarSalesReceipt", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<BookingDetails> BookingDetailsList = new List<BookingDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drBookingDetails in dstOutPut.Tables[0].Rows)
                    {
                        BookingDetails BookingDetails = new BookingDetails();
                        {
                            BookingDetails.SeatNos = drBookingDetails["SeatNos"].ToString();
                            BookingDetails.SeatCount = Convert.ToInt32(drBookingDetails["SeatCount"].ToString());
                            BookingDetails.Fare = Convert.ToDecimal(drBookingDetails["Fare"].ToString());
                            BookingDetails.Comm = Convert.ToDecimal(drBookingDetails["Comm"].ToString());
                            BookingDetails.AgentComm = Convert.ToDecimal(drBookingDetails["AgentComm"].ToString());
                            BookingDetails.Disc = Convert.ToDecimal(drBookingDetails["Disc"].ToString());
                            BookingDetails.TotalLineAmt = Convert.ToDecimal(dstOutPut.Tables[1].Rows[0]["TotalFare"].ToString());
                            BookingDetails.ActualAmt = Convert.ToDecimal(dstOutPut.Tables[1].Rows[0]["TotalVoucherAmt"].ToString());
                        }
                        BookingDetailsList.Add(BookingDetails);

                    }



                }

                return BookingDetailsList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Agent Payment

        public void PostAgentPayments(Boolean toPostAPIAgentVouchersAndPayment)
        {

            DataSet ds = null;
            DataSet dsoutput = null;
           
            try
            {
               
                    dsoutput = GetVoucherPaymentValidation(1945, toPostAPIAgentVouchersAndPayment);
                    if (dsoutput != null && dsoutput.Tables.Count > 0 && dsoutput.Tables[0].Rows.Count > 0)
                    {
                         string ErrorMsg = "";
                            if (Convert.ToInt32(dsoutput.Tables[0].Rows[0]["Count"]) > 0)
                            {
                                string VoucherIds = dsoutput.Tables[0].Rows[0]["VoucherIds"].ToString();
                                string ErrMsg = dsoutput.Tables[0].Rows[0]["ErrMsg"].ToString();
                                Logger.WriteLog(ErrMsg + VoucherIds);
                                ErrorMsg = ErrMsg + VoucherIds ;
                                
                            }
                            if (Convert.ToInt32(dsoutput.Tables[0].Rows[1]["Count"]) > 0)
                            {
                                string VoucherIds = dsoutput.Tables[0].Rows[1]["VoucherIds"].ToString();
                                string ErrMsg = dsoutput.Tables[0].Rows[1]["ErrMsg"].ToString();
                                Logger.WriteLog(ErrMsg + VoucherIds);
                                ErrorMsg += " & " + "\n" + ErrMsg + VoucherIds;
                 
                            }
                            if (Convert.ToInt32(dsoutput.Tables[0].Rows[2]["Count"]) > 0)
                            {
                                string VoucherIds = dsoutput.Tables[0].Rows[2]["VoucherIds"].ToString();
                                string ErrMsg = dsoutput.Tables[0].Rows[2]["ErrMsg"].ToString();
                                Logger.WriteLog(ErrMsg + VoucherIds);
                                ErrorMsg +=  " & " + "\n" + ErrMsg + VoucherIds ;
                       
                            }
                            if (Convert.ToInt32(dsoutput.Tables[0].Rows[3]["Count"]) > 0)
                            {
                                string VoucherIds = dsoutput.Tables[0].Rows[3]["VoucherIds"].ToString();
                                string ErrMsg = dsoutput.Tables[0].Rows[3]["ErrMsg"].ToString();
                                Logger.WriteLog(ErrMsg + VoucherIds);
                                ErrorMsg += " & " + "\n" +  ErrMsg + VoucherIds;
                      
                            }
                            if (ErrorMsg != "")
                            {
                                Email.SendMail(ErrorMsg);
                            }
                              



                }
               
                    ds = InsertVoucherPaymentReceiptsId(1945, toPostAPIAgentVouchersAndPayment);
               

            }
            catch(Exception ex)
            {
                Logger.WriteLog("PostAgentPayments", "GetVoucherPaymentReceiptsId", ex.Message, true);
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Logger.WriteLog("PostAgentPayments", "", "No Of AgentPayments: " + ds.Tables[0].Rows.Count, true);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int intAgentVoucherReceiptsID = 0;

                    intAgentVoucherReceiptsID = Convert.ToInt32(ds.Tables[0].Rows[i]["AgentVoucherReceiptsID"].ToString());

                    // Credit Memo

                    List<AgentVoucherDetails> agentVoucherListCM = null;
                    try
                    {
                        agentVoucherListCM = GetAgentVoucherCreditMemoFromCRS(1945, intAgentVoucherReceiptsID);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("PostAgentPayments", "GetAgentVoucherCreditMemoFromCRS", ex.Message, true);
                    }

                    decimal CMAmount = 0;

                    if (agentVoucherListCM != null && agentVoucherListCM.Count > 0)
                    {
                        foreach (AgentVoucherDetails avDetails in agentVoucherListCM)
                        {
                            CreditMemo ivPosted;
                            string status = "";
                            string statusMessage = "";
                            Int32 cnID = -1;
                            try
                            {
                                

                                ivPosted = PostCreditMemo(avDetails.AgentID, avDetails.AgentName, avDetails.QuickBookCustomerID,  avDetails.Amount,   avDetails.CreditNoteDateTime, avDetails.BranchDivisionName, avDetails.BranchDivisionID, avDetails.docformattednumber,avDetails.VoucherCreditNoteId);
                                cnID = Convert.ToInt32(ivPosted.Id);
                                status = "Posted";
                                statusMessage = "";

                                CMAmount = Convert.ToDecimal(avDetails.Amount);

                                UpdateQuickBookCreditNote(cnID, avDetails.VoucherCreditNoteId);
                            }
                            catch (Exception ex)
                            {
                                status = "Failed";
                                statusMessage = ex.Message;
                                Logger.WriteLog("PostAgentPayments", "PostCreditMemo-UpdateQuickBookCreditNote", ex.Message, true);
                            }
                        } 
                    }


                    // Payment 
                   
                        List<AgentPaymentDetails> agentPaymentListPD = null;
                        try
                        {
                            agentPaymentListPD = GetAgentPaymentDetailsFromCRS(1945, intAgentVoucherReceiptsID);
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("PostAgentPayments", "GetAgentPaymentDetailsFromCRS", " AgentVoucherReceiptId: " + intAgentVoucherReceiptsID + " " + ex.Message, true);
                        }

                        if (agentPaymentListPD != null && agentPaymentListPD.Count > 0)
                        {
                            EntryCounter.GetInstance().ResetAllCount();

                            foreach (AgentPaymentDetails apDetails in agentPaymentListPD)
                            {
                                Payment paymentPosted;
                                string status = "";
                                string statusMessage = "";
                                //string VoucherNoQB = "";
                                Int32 paymentID = -1;
                                try
                                {
                                    string VoucherNoQB = "";
                                    // string collboy = "";
                                    VoucherNoQB = CreateVoucherNoForPaymentQB(apDetails.CompanyID, Convert.ToInt32(apDetails.BranchDivisionID), intAgentVoucherReceiptsID, apDetails.PaymentDate);
                                    //if (dts != null && dts.Tables != null && dts.Tables.Count > 0)
                                    //{
                                    //    DataTable dtOutput = dts.Tables[0];
                                    //    if (dtOutput != null && dtOutput.Rows != null && dtOutput.Rows.Count > 0)
                                    //    {
                                    //        foreach (DataRow drDetails in dtOutput.Rows)
                                    //        {
                                    //            VoucherNoQB = drDetails["VoucherNoQB"].ToString();
                                    //            collboy = drDetails["collectionboyname"].ToString();
                                    //        }
                                    //    }
                                    //}
                                    paymentPosted = PostPayment(false,apDetails.AgentName, apDetails.QuickBookCustomerID, apDetails.inVoiceID, apDetails.TotalAmount, apDetails.PaymentType, apDetails.PaymentTypeID, apDetails.InstrumentNo, apDetails.DepositeToName, apDetails.DepositeToId, apDetails.TxnDate, apDetails.isHO, apDetails.UserLedgerName, apDetails.UserLedgerId, apDetails.PaymentDrLedgerName, apDetails.PaymentDrLedgerID, apDetails.PaymentCrLedgerName, apDetails.PaymentCrLedgerID, apDetails.BranchDivisionID, apDetails.DepositeToDivisionID, apDetails.AgentVoucherReceiptsID, apDetails.BranchDivisionNameJE, apDetails.BranchDivisionIDJE, apDetails.BranchDivisionNameJE2, apDetails.BranchDivisionIDJE2, VoucherNoQB);
                                    paymentID = Convert.ToInt32(paymentPosted.Id);
                                    status = "Posted";
                                    statusMessage = "";

                                    EntryCounter.GetInstance().IncreaseQBCount(1);

                                    InsertPaymentPostingDetailsToCRS(paymentID, apDetails.AgentVoucherReceiptsID, status, statusMessage);

                                    EntryCounter.GetInstance().IncreaseCRSCount(1);
                                }
                                catch (IdsException iex)
                                {
                                    Logger.WriteQBExceptonDetailToLog(iex);
                                }
                                catch (Exception ex)
                                {
                                    status = "Failed";
                                    statusMessage = ex.Message;
                                    Logger.WriteLog("AlertVersion 2 - PostAgentPayments", "CreateVoucherNoForPaymentQB-InsertPaymentPostingDetailsToCRS", ex.Message, true);
                                }


                            }

                            if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                            {
                                string msg = "AlertVersion 2 - PostAgentPayments::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                                Email.SendMail(msg);
                            }

                            EntryCounter.GetInstance().ResetAllCount();
                        }


//#region Journal Entry

//                    DataSet dsJournal = null;
//                    try
//                    {
//                        dsJournal = GetAgentJournalEntry(1945, intAgentVoucherReceiptsID);
//                    }
//                    catch (Exception ex)
//                    {
//                        Logger.WriteLog("PostAgentPayments", "GetAgentJournalEntry", ex.Message, true);
//                    }

//                    if (dsJournal != null && dsJournal.Tables.Count > 0 && dsJournal.Tables[0].Rows.Count > 0)
//                    {
//                        for (int j = 0; j < dsJournal.Tables[0].Rows.Count; j++)
//                        {
//                            try
//                            {
//                                int VoucherJournalId = Convert.ToInt32(dsJournal.Tables[0].Rows[j]["voucherjournalid"].ToString());
//                                decimal AmountCr = Convert.ToDecimal(dsJournal.Tables[0].Rows[j]["CreditAmount"].ToString());
//                                decimal AmountDr = Convert.ToDecimal(dsJournal.Tables[0].Rows[j]["DebitAmount"].ToString());
//                                int DebitLedgerID = Convert.ToInt32(dsJournal.Tables[0].Rows[j]["DebitId"].ToString());
//                                string DebitLedgerName = dsJournal.Tables[0].Rows[j]["DebitName"].ToString();
//                                int CreditLedgerID = Convert.ToInt32(dsJournal.Tables[0].Rows[j]["CreditId"].ToString());
//                                string CreditLedgerName = dsJournal.Tables[0].Rows[j]["CreditName"].ToString();
//                                int DivisionIdJE1 = Convert.ToInt32(dsJournal.Tables[0].Rows[j]["DivisionIdJE1"].ToString());
//                                string DivisionNameJE1 = dsJournal.Tables[0].Rows[j]["DivisionNameJE1"].ToString();
//                                int DivisionIdJE2 = Convert.ToInt32(dsJournal.Tables[0].Rows[j]["DivisionIdJE2"].ToString());
//                                string DivisionNameJE2 = dsJournal.Tables[0].Rows[j]["DivisionNameJE2"].ToString();
//                                string docnumber = dsJournal.Tables[0].Rows[j]["docformattednumber"].ToString();

//                                if (j > 0)
//                                {
//                                    DivisionIdJE1 = DivisionIdJE2;
//                                    DivisionNameJE1 = DivisionNameJE2;
//                                }
//                                JournalEntry JK = PostJournalEntry("Insert","","","","", 0, AmountCr, "", CreditLedgerName, CreditLedgerID, DebitLedgerName, DebitLedgerID, "-- Automaticaly posted from CRS", DivisionIdJE1, DivisionNameJE1, docnumber,"0","");

//                                UpdateJournalEntryPostingToCRS(VoucherJournalId, Convert.ToInt32(JK.Id));
//                            }
//                            catch(IdsException iex)
//                            {
//                                Logger.WriteQBExceptonDetailToLog(iex);
//                            }
//                            catch (Exception ex)
//                            {
//                                Logger.WriteLog("PostAgentPayments", "PostJournalEntry-UpdateJournalEntryPostingToCRS", ex.Message, true);
//                            }
//                        }
//                    }
//#endregion
                    /*
                        // 2 Journal Entry
                        int AccSysDivisionIDCR = 0;
                        int AccSysClassIDCR = 0;
                        int AccSysDivisionIDDR = 0;
                        int AccSysClassIDDR = 0;

                        JournalEntry JK = PostJournalEntry(customerDisplayName, quickbookCustomerID, amount, "", UserLedgerName, UserLedgerId, PaymentDrLedgerName, PaymentDrLedgerID, "", BranchDivisionIDJE, BranchDivisionNameJE);

                        InsertJournalEntryPostingToCRS("", "", "", "", JK.Id, DateTime.Now, "", UserLedgerId, "", AccSysDivisionIDCR, AccSysClassIDCR, PaymentDrLedgerID, "", AccSysDivisionIDDR, AccSysClassIDDR, amount);

                        // 3 Journal Entry

                        string strMemo = "Payment Mode : " + paymentType + ", Instrument No : " + InstrumentNo;
                        JournalEntry JK2 = PostJournalEntry(customerDisplayName, quickbookCustomerID, amount, "", PaymentCrLedgerName, PaymentCrLedgerID, DepositeToName, DepositeToId, strMemo, BranchDivisionIDJE2, BranchDivisionNameJE2);

                        InsertJournalEntryPostingToCRS("", "", "", "", JK2.Id, DateTime.Now, "", PaymentCrLedgerID, "", AccSysDivisionIDCR, AccSysClassIDCR, DepositeToId, "", AccSysDivisionIDDR, AccSysClassIDDR, amount);
                    */



                }
            }

            //if (ds != null && ds.Tables.Count > 0 && ds.Tables[1].Rows.Count > 0)
            //{
              
            //    string AgentVoucherReceiptsIDs = ds.Tables[1].Rows[0]["AgentVoucherReceiptsID"].ToString();
            //    if (AgentVoucherReceiptsIDs != "")
            //    {
            //        Logger.WriteLog("GetVoucherPaymentReceiptsId::Validation:: ReceiptsIds Not Posted Due To Invoice Is Not Posted: " + AgentVoucherReceiptsIDs);
            //        string msg = "Alert Version 2 : GetVoucherPaymentReceiptsId::Validation::ReceiptsIds Not Posted Due To Invoice Is Not Posted: " + AgentVoucherReceiptsIDs;
            //        Email.SendMail(msg);

            //    }
               

            //}
            return;
        }

        private DataSet GetVoucherPaymentInvoiceDetails(int AgentVoucherReceiptsid)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_AgentVoucherReceiptsid", AgentVoucherReceiptsid, ParameterDirection.Input);
                DataSet dstOutPut = dal.ExecuteSelect("spGetVoucherPaymentInvoiceDetails", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false,"",false);

                return dstOutPut;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private DataSet GetVoucherPaymentCreditNoteDetails(int AgentVoucherCreditNoteid)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_AgentVoucherCreditNoteid", AgentVoucherCreditNoteid, ParameterDirection.Input);
                DataSet dstOutPut = dal.ExecuteSelect("spGetVoucherPaymentCreditNoteDetails", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                return dstOutPut;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private DataSet InsertVoucherPaymentReceiptsId(int CompanyID,Boolean toPostAPIAgentVouchersAndPayment)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                string spName = "";
                if(toPostAPIAgentVouchersAndPayment)
                {
                    spName = "spInsertPendingQuickBooksReceiptsId_APIAgents";
                }
                else
                {
                    spName = "spInsertPendingQuickBooksReceiptsId";
                }
                DataSet dstOutPut = dal.ExecuteSelect(spName, CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", true,"", true);

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        

        private DataSet GetVoucherPaymentValidation(int CompanyID, Boolean toPostAPIAgentVouchersAndPayment)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                string spName = "";

                if (toPostAPIAgentVouchersAndPayment)
                {
                    spName = "spGetValidationQuickBooksReceiptsId_APIAgents";
                }
                else
                {
                    spName = "spGetValidationQuickBooksReceiptsId";
                }

                DataSet dstOutPut = dal.ExecuteSelect(spName, CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private DataSet GetAgentJournalEntry(int CompanyID, int agentvoucherreceiptsid)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_agentvoucherreceiptsid", agentvoucherreceiptsid, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetAgentJournalEntry", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<AgentPaymentDetails> GetAgentPaymentDetailsFromCRS(int CompanyID, int agentvoucherreceiptsid)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_agentvoucherreceiptsid", agentvoucherreceiptsid, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetVoucherPaymentForQuickBooksV2", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<AgentPaymentDetails> agentPaymentDetailsList = new List<AgentPaymentDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drAgentPaymentDetails in dstOutPut.Tables[0].Rows)
                    {
                        int qbCustomerId;
                        qbCustomerId = 0;
                        if (!Int32.TryParse(drAgentPaymentDetails["QuickBookCustomerID"].ToString(), out qbCustomerId))
                        {
                            Logger.WriteLog("GetAgentPaymentDetailsFromCRS::Validation::Invalid QuickBook CustomerId for AgentVoucherReceiptId: " + drAgentPaymentDetails["agentvoucherreceiptsid"].ToString());
                        }
                        else
                        {
                            AgentPaymentDetails agentPaymentDetails = new AgentPaymentDetails();
                            {
                                agentPaymentDetails.TransactionID = Convert.ToInt32(drAgentPaymentDetails["TransactionID"].ToString());
                                agentPaymentDetails.inVoiceID = Convert.ToInt32(drAgentPaymentDetails["AccSysID"].ToString());
                                agentPaymentDetails.ActualAmount = Convert.ToDecimal(drAgentPaymentDetails["PaidAmt"].ToString());
                                agentPaymentDetails.TotalAmount = Convert.ToDecimal(drAgentPaymentDetails["amountreceived"].ToString());
                                agentPaymentDetails.PaymentDate = Convert.ToDateTime(drAgentPaymentDetails["PaymentDate"].ToString());
                                agentPaymentDetails.AgentName = drAgentPaymentDetails["AgentName"].ToString();
                                agentPaymentDetails.QuickBookCustomerID = Convert.ToInt32(drAgentPaymentDetails["QuickBookCustomerID"].ToString());
                                agentPaymentDetails.PaymentType = drAgentPaymentDetails["PaymentType"].ToString();
                                agentPaymentDetails.PaymentTypeID = Convert.ToInt32(drAgentPaymentDetails["PaymentTypeID"].ToString());
                                agentPaymentDetails.InstrumentNo = drAgentPaymentDetails["InstrumentNo"].ToString();
                                agentPaymentDetails.DepositeToName = drAgentPaymentDetails["DepositeToName"].ToString();
                                agentPaymentDetails.DepositeToId = Convert.ToInt32(drAgentPaymentDetails["DepositeToID"].ToString());
                                agentPaymentDetails.TxnDate = drAgentPaymentDetails["PaymentDate"].ToString();
                                agentPaymentDetails.AgentVoucherReceiptsID = Convert.ToInt32(drAgentPaymentDetails["agentvoucherreceiptsid"].ToString());
                                agentPaymentDetails.isHO = Convert.ToInt32(drAgentPaymentDetails["isHO"].ToString());
                                //agentPaymentDetails.UserLedgerName = drAgentPaymentDetails["UserLedgerName"].ToString();
                                //agentPaymentDetails.UserLedgerId = Convert.ToInt32(drAgentPaymentDetails["UserLedgerID"].ToString());
                                agentPaymentDetails.PaymentDrLedgerName = drAgentPaymentDetails["PaymentDrLedgerName"].ToString();
                                agentPaymentDetails.PaymentDrLedgerID = Convert.ToInt32(drAgentPaymentDetails["PaymentDrLedgerID"].ToString());
                                agentPaymentDetails.PaymentCrLedgerName = drAgentPaymentDetails["PaymentCrLedgerName"].ToString();
                                agentPaymentDetails.PaymentCrLedgerID = Convert.ToInt32(drAgentPaymentDetails["PaymentCrLedgerID"].ToString());
                                agentPaymentDetails.BranchDivisionID = Convert.ToInt32(drAgentPaymentDetails["BranchDivision"].ToString());
                                agentPaymentDetails.DepositeToDivisionID = Convert.ToInt32(drAgentPaymentDetails["DepositeToDivision"].ToString());
                                agentPaymentDetails.BranchDivisionIDJE = Convert.ToInt32(drAgentPaymentDetails["BranchDivisionIDJE"].ToString());
                                agentPaymentDetails.BranchDivisionNameJE = drAgentPaymentDetails["BranchDivisionNameJE"].ToString();
                                agentPaymentDetails.BranchDivisionIDJE2 = Convert.ToInt32(drAgentPaymentDetails["BranchDivisionIDJE2"].ToString());
                                agentPaymentDetails.BranchDivisionNameJE2 = drAgentPaymentDetails["BranchDivisionNameJE2"].ToString();
                                agentPaymentDetails.CompanyID = Convert.ToInt32(drAgentPaymentDetails["CompanyID"].ToString());
                            }
                            agentPaymentDetailsList.Add(agentPaymentDetails);
                        }
                    }

                }

                return agentPaymentDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private String CreateVoucherNoForPaymentQB(Int32 CompanyID, Int32 DivisionID, Int32 AgentVoucherReceiptsID,DateTime PaymentDate)
        {

            try
            {
                string strErr = "";
                string strVoucherNo = "";
                DataSet ds = null;
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_DivisionID", DivisionID, ParameterDirection.Input);
                dal.AddParameter("p_AgentVoucherReceiptsID", AgentVoucherReceiptsID, ParameterDirection.Input);
                dal.AddParameter("p_PaymentDateTime", PaymentDate, ParameterDirection.Input);

                ds = dal.ExecuteSelect("spCreateAgentVoucherNoforPaymentQB_V2", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage",true);

                if (ds != null && ds.Tables.Count >= 1)
                {
                    strVoucherNo = ds.Tables[0].Rows[0]["VoucherNoQB"].ToString();
                }
                return strVoucherNo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Payment PostPayment(bool isfranchise,string customerDisplayName, int quickbookCustomerID, int invoiceID, decimal amount, string paymentType, int paymentTypeid, string InstrumentNo, string DepositeToName, int DepositeToId, string TxnDate, int isHO, string UserLedgerName, int UserLedgerId, string PaymentDrLedgerName, int PaymentDrLedgerID, string PaymentCrLedgerName, int PaymentCrLedgerID, int BranchDivisionID, int DepositeToDivisionID, int AgentVoucherReceiptsID, string BranchDivisionNameJE, int BranchDivisionIDJE, string BranchDivisionNameJE2, int BranchDivisionIDJE2,string VoucherNoQB)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Payment postedPayment = new Payment();

                // 1 Payment Entry
                #region IS Normal Branch

                Payment payment = new Payment();
                {
                    payment.TotalAmt = amount;
                    payment.TotalAmtSpecified = true;
                    payment.UnappliedAmt = 0;
                    payment.UnappliedAmtSpecified = true;
                    payment.TxnDateSpecified = true;

                    payment.CustomerRef = new ReferenceType() { name = customerDisplayName, Value = quickbookCustomerID.ToString(), };

                    //payment.DepositToAccountRef = new ReferenceType() { name = UserLedgerName, Value = UserLedgerId.ToString(), type = "Bank" };
                    //payment.DepositToAccountRef = new ReferenceType() { name = DepositeToName, Value = DepositeToId.ToString(), type = "Bank" };
                }
                //if (paymentType.Equals("Cheque"))
                //{
                //    payment.PaymentMethodRef = new ReferenceType() { name = "Cheque", Value = "9" };
                //}
                //else if (paymentType.Equals("Net Banking"))
                //{
                //    payment.PaymentMethodRef = new ReferenceType() { name = "Net Banking", Value = "11" };
                //}
                //else
                //{
                //    payment.PaymentMethodRef = new ReferenceType() { name = "Cash", Value = "8" };
                //}
                
                //payment.PaymentTypeSpecified = true;
                //payment.PaymentRefNum = InstrumentNo;
                payment.TxnDate = Convert.ToDateTime(TxnDate);


                #region Payment Posting to Invoice
                DataSet dsInvoiceList = null;

                if (isfranchise)
                {
                     dsInvoiceList = GetFranchiseVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);
                }
                else
                {
                     dsInvoiceList = GetVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);
                }

               

                if (dsInvoiceList != null && dsInvoiceList.Tables.Count > 0)
                {
                    List<Line> paymentLines = new List<Line>();

                    for (int i = 0; i < dsInvoiceList.Tables[0].Rows.Count; i++)
                    {
                        Line paymentLineOne = new Line();

                        List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                        LinkedTxn linkedTxn1 = new LinkedTxn();
                        {
                            linkedTxn1.TxnType = "Invoice";
                            linkedTxn1.TxnId = dsInvoiceList.Tables[0].Rows[i]["InvoiceID"].ToString();
                        } // invoiceID.ToString();
                        linkedPaymentTxns.Add(linkedTxn1);
                        paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                        paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[0].Rows[i]["ActualAmount"].ToString()); //amount;
                        paymentLineOne.AmountSpecified = true;

                        paymentLines.Add(paymentLineOne);
                    }

                    if (dsInvoiceList.Tables.Count > 1)
                    {
                        for (int i = 0; i < dsInvoiceList.Tables[1].Rows.Count; i++)
                        {
                            Line paymentLineOne = new Line();

                            List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                            LinkedTxn linkedTxn1 = new LinkedTxn();
                            {
                                linkedTxn1.TxnType = "CreditMemo";
                                linkedTxn1.TxnId = dsInvoiceList.Tables[1].Rows[i]["CreditNoteId"].ToString();
                            }
                            linkedPaymentTxns.Add(linkedTxn1);
                            paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                            paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[1].Rows[i]["Amount"].ToString());
                            paymentLineOne.AmountSpecified = true;

                            paymentLines.Add(paymentLineOne);
                        }
                    }
                    payment.Line = paymentLines.ToArray();
                }

                payment.PrivateNote = "[Voucher No : " + VoucherNoQB + "] -- Automaticaly posted from CRS";
                #endregion

                postedPayment = service.Add(payment);

                /*
                // 2 Journal Entry
                int AccSysDivisionIDCR = 0;
                int AccSysClassIDCR = 0;
                int AccSysDivisionIDDR = 0;
                int AccSysClassIDDR = 0;
                
                JournalEntry JK = PostJournalEntry(customerDisplayName, quickbookCustomerID, amount, "", UserLedgerName, UserLedgerId, PaymentDrLedgerName, PaymentDrLedgerID, "", BranchDivisionIDJE, BranchDivisionNameJE);

                InsertJournalEntryPostingToCRS("", "", "", "", JK.Id, DateTime.Now, "", UserLedgerId, "", AccSysDivisionIDCR, AccSysClassIDCR, PaymentDrLedgerID, "", AccSysDivisionIDDR, AccSysClassIDDR, amount);

                // 3 Journal Entry

                string strMemo = "Payment Mode : " + paymentType + ", Instrument No : " + InstrumentNo;
                JournalEntry JK2 = PostJournalEntry(customerDisplayName, quickbookCustomerID, amount, "", PaymentCrLedgerName, PaymentCrLedgerID, DepositeToName, DepositeToId, strMemo, BranchDivisionIDJE2, BranchDivisionNameJE2);

                InsertJournalEntryPostingToCRS("", "", "", "", JK2.Id, DateTime.Now, "", PaymentCrLedgerID, "", AccSysDivisionIDCR, AccSysClassIDCR, DepositeToId, "", AccSysDivisionIDDR, AccSysClassIDDR, amount);

                */

                #endregion

                return postedPayment;

                /* Payment Entry - End*/
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }


        private Payment PostPayment_Old(string customerDisplayName, int quickbookCustomerID, int invoiceID, decimal amount, string paymentType, int paymentTypeid, string InstrumentNo, string DepositeToName, int DepositeToId, string TxnDate, int isHO, string UserLedgerName, int UserLedgerId, string PaymentDrLedgerName, int PaymentDrLedgerID, string PaymentCrLedgerName, int PaymentCrLedgerID, int BranchDivisionID, int DepositeToDivisionID, int AgentVoucherReceiptsID, string BranchDivisionNameJE, int BranchDivisionIDJE, string BranchDivisionNameJE2, int BranchDivisionIDJE2)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Payment postedPayment = new Payment();

                if (paymentType == "Cash")
                {
                    //Payment Entry
                    #region Cash Payment

                    Payment payment = new Payment();
                    {
                        payment.TotalAmt = amount;
                        payment.TotalAmtSpecified = true;
                        payment.UnappliedAmt = 0;
                        payment.UnappliedAmtSpecified = true;
                        payment.TxnDate = Util.Util.GetServerDateTime();// DateTime.Now;
                        payment.TxnDateSpecified = true;

                        payment.CustomerRef = new ReferenceType() { name = customerDisplayName, Value = quickbookCustomerID.ToString(), };

                        payment.DepositToAccountRef = new ReferenceType() { name = UserLedgerName, Value = UserLedgerId.ToString() };

                        payment.PaymentMethodRef = new ReferenceType() { name = paymentType, Value = paymentTypeid.ToString() };
                        payment.PaymentTypeSpecified = true;
                        payment.PaymentRefNum = InstrumentNo;
                        payment.TxnDate = Convert.ToDateTime(TxnDate);
                    }
                    #region Payment Posting to Invoice

                    DataSet dsInvoiceList = GetVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);

                    if (dsInvoiceList != null && dsInvoiceList.Tables.Count > 0)
                    {
                        List<Line> paymentLines = new List<Line>();

                        for (int i = 0; i < dsInvoiceList.Tables[0].Rows.Count; i++)
                        {
                            Line paymentLineOne = new Line();

                            List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                            LinkedTxn linkedTxn1 = new LinkedTxn();
                            {
                                linkedTxn1.TxnType = "Invoice";
                                linkedTxn1.TxnId = dsInvoiceList.Tables[0].Rows[i]["InvoiceID"].ToString();
                            }// invoiceID.ToString();
                            linkedPaymentTxns.Add(linkedTxn1);
                            paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                            paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[0].Rows[i]["ActualAmount"].ToString()); //amount;
                            paymentLineOne.AmountSpecified = true;

                            paymentLines.Add(paymentLineOne);
                        }

                        if (dsInvoiceList.Tables.Count > 1)
                        {
                            for (int i = 0; i < dsInvoiceList.Tables[1].Rows.Count; i++)
                            {
                                Line paymentLineOne = new Line();

                                List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                                LinkedTxn linkedTxn1 = new LinkedTxn();
                                {
                                    linkedTxn1.TxnType = "CreditMemo";
                                    linkedTxn1.TxnId = dsInvoiceList.Tables[1].Rows[i]["CreditNoteId"].ToString();
                                }
                                linkedPaymentTxns.Add(linkedTxn1);
                                paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                                paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[1].Rows[i]["Amount"].ToString());
                                paymentLineOne.AmountSpecified = true;

                                paymentLines.Add(paymentLineOne);
                            }
                        }
                        payment.Line = paymentLines.ToArray();
                    }

                    #endregion

                    postedPayment = service.Add(payment);

                    #endregion
                }
                else
                {
                    if (BranchDivisionID > 0 && DepositeToDivisionID > 0 && (BranchDivisionID == DepositeToDivisionID))
                    {
                        //Payment Entry
                        #region Branch and Deposite To Division are same

                        Payment payment = new Payment();
                        {
                            payment.TotalAmt = amount;
                            payment.TotalAmtSpecified = true;
                            payment.UnappliedAmt = 0;
                            payment.UnappliedAmtSpecified = true;
                            payment.TxnDate = Util.Util.GetServerDateTime();// DateTime.Now;
                            payment.TxnDateSpecified = true;

                            payment.CustomerRef = new ReferenceType() { name = customerDisplayName, Value = quickbookCustomerID.ToString(), };

                            payment.DepositToAccountRef = new ReferenceType() { name = DepositeToName, Value = DepositeToId.ToString() };

                            payment.PaymentMethodRef = new ReferenceType() { name = paymentType, Value = paymentTypeid.ToString() };
                            payment.PaymentTypeSpecified = true;
                            payment.PaymentRefNum = InstrumentNo;
                        }
                        payment.TxnDate = Convert.ToDateTime(TxnDate);


                        #region Payment Posting to Invoice

                        DataSet dsInvoiceList = GetVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);

                        if (dsInvoiceList != null && dsInvoiceList.Tables.Count > 0)
                        {
                            List<Line> paymentLines = new List<Line>();

                            for (int i = 0; i < dsInvoiceList.Tables[0].Rows.Count; i++)
                            {
                                Line paymentLineOne = new Line();

                                List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                                LinkedTxn linkedTxn1 = new LinkedTxn();
                                {
                                    linkedTxn1.TxnType = "Invoice";
                                    linkedTxn1.TxnId = dsInvoiceList.Tables[0].Rows[i]["InvoiceID"].ToString();
                                } // invoiceID.ToString();
                                linkedPaymentTxns.Add(linkedTxn1);
                                paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                                paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[0].Rows[i]["ActualAmount"].ToString()); //amount;
                                paymentLineOne.AmountSpecified = true;

                                paymentLines.Add(paymentLineOne);
                            }

                            if (dsInvoiceList.Tables.Count > 1)
                            {
                                for (int i = 0; i < dsInvoiceList.Tables[1].Rows.Count; i++)
                                {
                                    Line paymentLineOne = new Line();

                                    List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                                    LinkedTxn linkedTxn1 = new LinkedTxn();
                                    {
                                        linkedTxn1.TxnType = "CreditMemo";
                                        linkedTxn1.TxnId = dsInvoiceList.Tables[1].Rows[i]["CreditNoteId"].ToString();
                                    }
                                    linkedPaymentTxns.Add(linkedTxn1);
                                    paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                                    paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[1].Rows[i]["Amount"].ToString());
                                    paymentLineOne.AmountSpecified = true;

                                    paymentLines.Add(paymentLineOne);
                                }
                            }
                            payment.Line = paymentLines.ToArray();
                        }

                        #endregion

                        postedPayment = service.Add(payment);

                        #endregion
                    }
                    else
                    {
                        if (isHO == 1)
                        {
                            // Payment Entry
                            #region IS HO

                            Payment payment = new Payment();
                            {
                                payment.TotalAmt = amount;
                                payment.TotalAmtSpecified = true;
                                payment.UnappliedAmt = 0;
                                payment.UnappliedAmtSpecified = true;
                                payment.TxnDate = Util.Util.GetServerDateTime();// DateTime.Now;
                                payment.TxnDateSpecified = true;

                                payment.CustomerRef = new ReferenceType() { name = customerDisplayName, Value = quickbookCustomerID.ToString(), };

                                payment.DepositToAccountRef = new ReferenceType() { name = DepositeToName, Value = DepositeToId.ToString() };
                                payment.PaymentMethodRef = new ReferenceType() { name = paymentType, Value = paymentTypeid.ToString() };
                                payment.PaymentTypeSpecified = true;
                                payment.PaymentRefNum = InstrumentNo;
                                payment.TxnDate = Convert.ToDateTime(TxnDate);
                            }

                            /*
                            if (paymentType == PaymentType.Cash)
                            {
                                payment.DepositToAccountRef = new ReferenceType() { name = "Cash at Paulo", Value = "94" };
                                payment.PaymentMethodRef = new ReferenceType() { name = "Cash", Value = "1" };
                                payment.PaymentType = PaymentTypeEnum.Cash;
                            }
                            else if (paymentType == PaymentType.Check)
                            {
                                payment.DepositToAccountRef = new ReferenceType() { name = "HDFC Bank - 123456789", Value = "93" };
                                payment.PaymentMethodRef = new ReferenceType() { name = "Check", Value = "2" };
                                payment.PaymentType = PaymentTypeEnum.Check;
                            }
                            */


                            #region Payment Posting to Journal Entry

                            //List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                            //LinkedTxn linkedTxn1 = new LinkedTxn();
                            //linkedTxn1.TxnType = "JournalEntry";
                            //linkedTxn1.TxnId = journalEntryID.ToString();
                            //linkedPaymentTxns.Add(linkedTxn1);
                            //paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                            //paymentLineOne.Amount = amount;
                            //paymentLineOne.AmountSpecified = true;

                            #endregion

                            #region Payment Posting to Invoice

                            DataSet dsInvoiceList = GetVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);

                            if (dsInvoiceList != null && dsInvoiceList.Tables.Count > 0)
                            {
                                List<Line> paymentLines = new List<Line>();

                                for (int i = 0; i < dsInvoiceList.Tables[0].Rows.Count; i++)
                                {
                                    Line paymentLineOne = new Line();

                                    List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                                    LinkedTxn linkedTxn1 = new LinkedTxn();
                                    {
                                        linkedTxn1.TxnType = "Invoice";
                                        linkedTxn1.TxnId = dsInvoiceList.Tables[0].Rows[i]["InvoiceID"].ToString(); // invoiceID.ToString();
                                        linkedPaymentTxns.Add(linkedTxn1);
                                    }
                                    paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                                    paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[0].Rows[i]["ActualAmount"].ToString()); //amount;
                                    paymentLineOne.AmountSpecified = true;

                                    paymentLines.Add(paymentLineOne);
                                }

                                if (dsInvoiceList.Tables.Count > 1)
                                {
                                    for (int i = 0; i < dsInvoiceList.Tables[1].Rows.Count; i++)
                                    {
                                        Line paymentLineOne = new Line();

                                        List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                                        LinkedTxn linkedTxn1 = new LinkedTxn();
                                        {
                                            linkedTxn1.TxnType = "CreditMemo";
                                            linkedTxn1.TxnId = dsInvoiceList.Tables[1].Rows[i]["CreditNoteId"].ToString();
                                        }
                                        linkedPaymentTxns.Add(linkedTxn1);
                                        paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                                        paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[1].Rows[i]["Amount"].ToString());
                                        paymentLineOne.AmountSpecified = true;

                                        paymentLines.Add(paymentLineOne);
                                    }
                                }
                                payment.Line = paymentLines.ToArray();
                            }

                            #endregion

                            postedPayment = service.Add(payment);

                            return postedPayment;

                            #endregion
                        }
                        else
                        {
                            // 1 Payment Entry
                            #region IS Normal Branch

                            Payment payment = new Payment();
                            {
                                payment.TotalAmt = amount;
                                payment.TotalAmtSpecified = true;
                                payment.UnappliedAmt = 0;
                                payment.UnappliedAmtSpecified = true;
                                payment.TxnDate = Util.Util.GetServerDateTime();// DateTime.Now;
                                payment.TxnDateSpecified = true;

                                payment.CustomerRef = new ReferenceType() { name = customerDisplayName, Value = quickbookCustomerID.ToString(), };

                                payment.DepositToAccountRef = new ReferenceType() { name = UserLedgerName, Value = UserLedgerId.ToString(), type = "Bank" };

                                payment.PaymentMethodRef = new ReferenceType() { name = "Cash", Value = "1" };
                                payment.PaymentTypeSpecified = true;
                                //payment.PaymentRefNum = InstrumentNo;
                                payment.TxnDate = Convert.ToDateTime(TxnDate);
                            }

                            #region Payment Posting to Invoice

                            DataSet dsInvoiceList = GetVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);

                            if (dsInvoiceList != null && dsInvoiceList.Tables.Count > 0)
                            {
                                List<Line> paymentLines = new List<Line>();

                                for (int i = 0; i < dsInvoiceList.Tables[0].Rows.Count; i++)
                                {
                                    Line paymentLineOne = new Line();

                                    List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                                    LinkedTxn linkedTxn1 = new LinkedTxn();
                                    {
                                        linkedTxn1.TxnType = "Invoice";
                                        linkedTxn1.TxnId = dsInvoiceList.Tables[0].Rows[i]["InvoiceID"].ToString();
                                    } // invoiceID.ToString();
                                    linkedPaymentTxns.Add(linkedTxn1);
                                    paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                                    paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[0].Rows[i]["ActualAmount"].ToString()); //amount;
                                    paymentLineOne.AmountSpecified = true;

                                    paymentLines.Add(paymentLineOne);
                                }

                                if (dsInvoiceList.Tables.Count > 1)
                                {
                                    for (int i = 0; i < dsInvoiceList.Tables[1].Rows.Count; i++)
                                    {
                                        Line paymentLineOne = new Line();

                                        List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                                        LinkedTxn linkedTxn1 = new LinkedTxn();
                                        {
                                            linkedTxn1.TxnType = "CreditMemo";
                                            linkedTxn1.TxnId = dsInvoiceList.Tables[1].Rows[i]["CreditNoteId"].ToString();
                                        }
                                        linkedPaymentTxns.Add(linkedTxn1);
                                        paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                                        paymentLineOne.Amount = Convert.ToDecimal(dsInvoiceList.Tables[1].Rows[i]["Amount"].ToString());
                                        paymentLineOne.AmountSpecified = true;

                                        paymentLines.Add(paymentLineOne);
                                    }
                                }
                                payment.Line = paymentLines.ToArray();
                            }

                            #endregion

                            postedPayment = service.Add(payment);

                            // 2 Journal Entry
                            int AccSysDivisionIDCR = 0;
                            int AccSysClassIDCR = 0;
                            int AccSysDivisionIDDR = 0;
                            int AccSysClassIDDR = 0;


                           // JournalEntry JK = PostJournalEntry("Insert","","","",customerDisplayName, quickbookCustomerID, amount, "", UserLedgerName, UserLedgerId, PaymentDrLedgerName, PaymentDrLedgerID, "", BranchDivisionIDJE, BranchDivisionNameJE, "","0","");

                            //InsertJournalEntryPostingToCRS("", "", "", "", JK.Id, DateTime.Now, "", UserLedgerId, "", AccSysDivisionIDCR, AccSysClassIDCR, PaymentDrLedgerID, "", AccSysDivisionIDDR, AccSysClassIDDR, amount);

                            // 3 Journal Entry

                            string strMemo = "Payment Mode : " + paymentType + ", Instrument No : " + InstrumentNo;
                            //JournalEntry JK2 = PostJournalEntry("Insert","","","",customerDisplayName, quickbookCustomerID, amount, "", PaymentCrLedgerName, PaymentCrLedgerID, DepositeToName, DepositeToId, strMemo, BranchDivisionIDJE2, BranchDivisionNameJE2, "","0","");

                            //InsertJournalEntryPostingToCRS("", "", "", "", JK2.Id, DateTime.Now, "", PaymentCrLedgerID, "", AccSysDivisionIDCR, AccSysClassIDCR, DepositeToId, "", AccSysDivisionIDDR, AccSysClassIDDR, amount);

                            #endregion
                        }
                    }
                }
                return postedPayment;

                /* Payment Entry - End*/
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        private void InsertPaymentPostingDetailsToCRS(int paymentID, int agentvoucherreceiptsid, string status, string statusMessage)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("PaymentID", paymentID, ParameterDirection.Input);
                dal.AddParameter("TransactionID", agentvoucherreceiptsid, ParameterDirection.Input);
                dal.AddParameter("STATUS", status, 100, ParameterDirection.Input);
                dal.AddParameter("StatusMessage", statusMessage, 500, ParameterDirection.Input);

                dal.ExecuteDML("spInsertQuickBookVoucherPaymentStatus", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void InsertSaleEntryPostingToCRS(string doctype, string docsubtype, string docnumber, string docformattednumber, string accsysid, int Toutledgerid_cr,string Toutdescription_cr,decimal ToutFare, DateTime journaldatetime, string narration, int accsysledgerid_cr, string description_cr, int accsysdivisionid_cr, int accsysclassid_cr, int accsysledgerid_dr, string description_dr, int accsysdivisionid_dr, int accsysclassid_dr, decimal amount, int CompanyID, int UserID, string Type, int BookingID)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_doctype", doctype, doctype.Length, ParameterDirection.Input);
                dal.AddParameter("p_docsubtype", docsubtype, docsubtype.Length, ParameterDirection.Input);
                dal.AddParameter("p_docnumber", docnumber, docnumber.Length, ParameterDirection.Input);
                dal.AddParameter("p_docformattednumber", docformattednumber, docformattednumber.Length, ParameterDirection.Input);
                dal.AddParameter("p_accsysid", accsysid, accsysid.Length, ParameterDirection.Input);

                dal.AddParameter("p_Toutledgerid_cr", Toutledgerid_cr, ParameterDirection.Input);
                dal.AddParameter("p_Toutdescription_cr", Toutdescription_cr, Toutdescription_cr.Length, ParameterDirection.Input);
                dal.AddParameter("p_ToutFare", ToutFare, ParameterDirection.Input);

                dal.AddParameter("p_saledatetime", journaldatetime, ParameterDirection.Input);
                dal.AddParameter("p_narration", narration, narration.Length, ParameterDirection.Input);
                dal.AddParameter("p_amount", amount, ParameterDirection.Input);

                dal.AddParameter("p_accsysledgerid_cr", accsysledgerid_cr, ParameterDirection.Input);
                dal.AddParameter("p_description_cr", description_cr, description_cr.Length, ParameterDirection.Input);
                dal.AddParameter("p_accsysdivisionid_cr", accsysdivisionid_cr, ParameterDirection.Input);
                dal.AddParameter("p_accsysclassid_cr", accsysclassid_cr, ParameterDirection.Input);

                dal.AddParameter("p_accsysledgerid_dr", accsysledgerid_dr, ParameterDirection.Input);
                dal.AddParameter("p_description_dr", description_dr, description_dr.Length, ParameterDirection.Input);
                dal.AddParameter("p_accsysdivisionid_dr", accsysdivisionid_dr, ParameterDirection.Input);
                dal.AddParameter("p_accsysclassid_dr", accsysclassid_dr, ParameterDirection.Input);
                //dal.AddParameter("p_agentvoucherreceiptsid", 0, ParameterDirection.Input);

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_UserID", UserID, ParameterDirection.Input);
                dal.AddParameter("p_Type", Type, Type.Length, ParameterDirection.Input);
                dal.AddParameter("p_bookingid", BookingID, ParameterDirection.Input);

                dal.ExecuteDML("spInsertQuickBookSalesEntry", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateJournalEntryPostingToCRS(int voucherjournalid, int accsysid)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_voucherjournalid", voucherjournalid, ParameterDirection.Input);
                dal.AddParameter("p_accsysid", accsysid, ParameterDirection.Input);

                dal.ExecuteDML("spUpdateQuickBookJournalEntry", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateBusNumberPostingToCRS(int voucherjournalid, int classid)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_voucherjournalid", voucherjournalid, ParameterDirection.Input);
                dal.AddParameter("p_accsysclassid", classid, ParameterDirection.Input);

                dal.ExecuteDML("spUpdateBusNumberClassForJournalEntry", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Credit Memo

        /*
        public void PostCreditMemo()
        {

            #region Posting in CreditMemo

            List<AgentVoucherDetails> agentVoucherList = GetAgentVoucherCreditMemoFromCRS();

            foreach (AgentVoucherDetails avDetails in agentVoucherList)
            {
                CreditMemo ivPosted;
                string status = "";
                string statusMessage = "";
                Int32 cnID = -1;
                try
                {
                    ivPosted = PostCreditMemo(avDetails.AgentID, avDetails.AgentName, avDetails.QuickBookCustomerID, avDetails.FromCityName, avDetails.ToCityName, avDetails.RouteFromCityName, avDetails.RouteToCityName, avDetails.BusNumber, avDetails.AgentPhone1, avDetails.AgentPhone2, avDetails.Amount, avDetails.PNR, avDetails.PassengerName, avDetails.SeatNos, avDetails.SeatCount, avDetails.FromTo, avDetails.JDate, avDetails.JTime, avDetails.BDate, avDetails.BookingID, avDetails.GeneratedDate, avDetails.TransactionID, avDetails.BusType, avDetails.TripCode, avDetails.ItemName, avDetails.ItemID, avDetails.ClassName, avDetails.ClassID, avDetails.BranchDivisionName, avDetails.BranchDivisionID);
                    cnID = Convert.ToInt32(ivPosted.Id);
                    status = "Posted";
                    statusMessage = "";
                }
                catch (Exception ex)
                {
                    status = "Failed";
                    statusMessage = ex.Message;
                }

                UpdateQuickBookCreditNote(cnID, avDetails.VoucherCreditNoteId);
            }

            #endregion

            return;
        }
        */

        private List<AgentVoucherDetails> GetAgentVoucherCreditMemoFromCRS(int CompanyID, int AgentVoucherReceiviptsID)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_agentvoucherreceiptsid", AgentVoucherReceiviptsID, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetVoucherCreditMemoForQuickBooks", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<AgentVoucherDetails> agentVoucherDetailsList = new List<AgentVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drAgentVoucherDetails in dstOutPut.Tables[0].Rows)
                    {

                        Int32 Merge = 0;
                        //if (!Int32.TryParse(drAgentVoucherDetails["IsMerge"].ToString(), out Merge))
                        //{

                        //    Logger.WriteLog("GetAgentVoucherCreditMemoFromCRS::Validation::Invalid Classid for VoucherNo: " + drAgentVoucherDetails["PNR"].ToString() + " BusId: " + drAgentVoucherDetails["BusId"].ToString());
                        // }
                        //else
                        {
                            AgentVoucherDetails agentVoucherDetails = new AgentVoucherDetails();
                            {
                                agentVoucherDetails.AgentID = Convert.ToInt32(drAgentVoucherDetails["AgentID"].ToString());
                                agentVoucherDetails.AgentName = drAgentVoucherDetails["AgentName"].ToString();
                                agentVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drAgentVoucherDetails["QuickBookCustomerID"].ToString());
                                //agentVoucherDetails.FromCityName = drAgentVoucherDetails["FromCityName"].ToString();
                                //agentVoucherDetails.ToCityName = drAgentVoucherDetails["ToCityName"].ToString();
                                //agentVoucherDetails.RouteFromCityName = drAgentVoucherDetails["RouteFromCityName"].ToString();
                                //agentVoucherDetails.RouteToCityName = drAgentVoucherDetails["RouteToCityName"].ToString();
                                //agentVoucherDetails.BusNumber = drAgentVoucherDetails["BusNumber"].ToString();
                                //agentVoucherDetails.BusType = drAgentVoucherDetails["ChartName"].ToString();
                                agentVoucherDetails.AgentPhone1 = drAgentVoucherDetails["ContactNo1"].ToString();
                                agentVoucherDetails.AgentPhone2 = drAgentVoucherDetails["ContactNo2"].ToString();
                                agentVoucherDetails.Amount = Convert.ToDecimal(drAgentVoucherDetails["amount"].ToString());
                                //agentVoucherDetails.PNR = drAgentVoucherDetails["PNR"].ToString();
                                //agentVoucherDetails.PassengerName = drAgentVoucherDetails["PassengerName"].ToString();
                                //agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                //agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                //agentVoucherDetails.FromTo = drAgentVoucherDetails["FromTo"].ToString();
                                //agentVoucherDetails.JDate = drAgentVoucherDetails["JourneyDate"].ToString();
                                //agentVoucherDetails.JTime = drAgentVoucherDetails["JTime"].ToString();
                                //agentVoucherDetails.BDate = drAgentVoucherDetails["BookingDate"].ToString();
                                //agentVoucherDetails.BookingID = Convert.ToInt32(drAgentVoucherDetails["BookingID"].ToString());
                                //agentVoucherDetails.GeneratedDate = Convert.ToDateTime(drAgentVoucherDetails["GeneratedDate"].ToString());
                                //agentVoucherDetails.TransactionID = Convert.ToInt32(drAgentVoucherDetails["TransactionID"].ToString());
                                //if (drAgentVoucherDetails["ItemID"].ToString() != "")
                                //    agentVoucherDetails.ItemID = Convert.ToInt32(drAgentVoucherDetails["ItemID"].ToString());
                                //agentVoucherDetails.ItemName = drAgentVoucherDetails["Item"].ToString();
                                //agentVoucherDetails.ClassID = drAgentVoucherDetails["ClassID"].ToString();
                                //agentVoucherDetails.ClassName = drAgentVoucherDetails["classname"].ToString();
                                agentVoucherDetails.BranchDivisionID = drAgentVoucherDetails["BranchDivisionID"].ToString();
                                agentVoucherDetails.BranchDivisionName = drAgentVoucherDetails["BranchDivisionName"].ToString();
                                agentVoucherDetails.VoucherCreditNoteId = Convert.ToInt32(drAgentVoucherDetails["VoucherCreditNoteId"].ToString());
                                agentVoucherDetails.IsDisputed = Convert.ToInt32(drAgentVoucherDetails["IsDisputed"].ToString());
                                agentVoucherDetails.docformattednumber = drAgentVoucherDetails["docformattednumber"].ToString();
                                agentVoucherDetails.CreditNoteDateTime = drAgentVoucherDetails["creditnotedatetime"].ToString();
                            }
                            agentVoucherDetailsList.Add(agentVoucherDetails);
                        }
                    }

                }

                return agentVoucherDetailsList;
            }
            catch (Exception)
            {
                throw;
            }

        }
        private CreditMemo PostCreditMemo(int AgentID, string AgentName, int QuickBookCustomerID, decimal Amount, string CreditNoteDate, string DivisionName, string DivisionID, string docformattednumber,int CreditNoteId)
        //string customerDisplayName, int quickbookCustomerID, decimal amount, string description)
        {
            try
            {

                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                CreditMemo creditMemo = new CreditMemo();


                {
                    creditMemo.CustomerRef = new ReferenceType() { name = AgentName, Value = QuickBookCustomerID.ToString() };
                    //invoice.DepositToAccountRef = new ReferenceType() { name = "Cash at Paulo", Value = "94" };

                    //invoice.DueDate = new DateTime(2016, 7, 17);
                    //invoice.DueDateSpecified = false;
                    //invoice.PaymentMethodRef = new ReferenceType() { name = "Cash", Value = "1" };
                    //invoice.PaymentType = PaymentTypeEnum.Cash;
                    //invoice.PaymentTypeSpecified = true;

                    //creditMemo.SalesTermRef = new ReferenceType() { name = "Due on receipt", Value = "1" };
                    creditMemo.SalesTermRef = new ReferenceType() { name = "Due on receipt", Value = "9" };

                    if (DivisionName != "")
                        creditMemo.DepartmentRef = new ReferenceType() { name = DivisionName, Value = DivisionID }; //agent area

                    List<CustomField> customFieldList = new List<CustomField>();
                    CustomField pickupdrop = new CustomField();
                    {
                        pickupdrop.DefinitionId = "1";
                        pickupdrop.Name = "PickUp-Drop";
                        pickupdrop.AnyIntuitObject = "" + "-" + "";
                    }
                    customFieldList.Add(pickupdrop);

                    CustomField JourneyDate = new CustomField();
                    {
                        JourneyDate.DefinitionId = "2";
                        JourneyDate.Name = "Journey Date";
                        JourneyDate.AnyIntuitObject = "";
                        customFieldList.Add(JourneyDate);
                    }

                    CustomField PhoneNumber = new CustomField();
                    {
                        PhoneNumber.DefinitionId = "3";
                        PhoneNumber.Name = "Phone Number";
                        PhoneNumber.AnyIntuitObject = "" + "," + "";
                    }
                    customFieldList.Add(PhoneNumber);

                    creditMemo.CustomField = customFieldList.ToArray();
                    creditMemo.TxnDate = Convert.ToDateTime(CreditNoteDate);
                    creditMemo.DocNumber = docformattednumber;
                    creditMemo.TxnDateSpecified = true;

                    List<Line> CreditMemoLineList = new List<Line>();

                    //List<BookingDetails> BookingDetailsList = GetBookingrDetailsFromCRS(BookingID);
                    //foreach (BookingDetails bkgDetails in BookingDetailsList)
                    //{



                    //saleItemDetail.Qty = new Decimal(2);
                    //saleItemDetail.QtySpecified = true;
                    //saleItemDetail.AnyIntuitObject = 500m;
                    //saleItemDetail.ItemElementName = ItemChoiceType.UnitPrice;

                    //saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "9" };
                    //saleItemDetail.TaxInclusiveAmtSpecified = true;
                    //saleItemDetail.TaxInclusiveAmt = 68m; // bkgDetails.Fare * bkgDetails.SeatCount;

                    //string strItemName = "";
                    //if (intItemID.ToString() != "0")
                    //    strItemName = "Ticket:" + ItemName;
                    //else
                    //{
                    //    strItemName = "Ticket:AHDGA 10.00 AM";
                    //    intItemID = 12;
                    //}

                    //saleItemDetail.ItemRef = new ReferenceType() { name = ItemName, Value = intItemID.ToString() };

                    //if (ClassName != "")
                    //    saleItemDetail.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };


                    //string strDesc = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                    //    "Seats : " + SeatNos + "\n" +
                    //    "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                    //    "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                    //    "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate;
                    #region Credit Note Posting Invocie

                    DataSet dsInvoiceList = GetVoucherPaymentCreditNoteDetails(CreditNoteId);

                    if (dsInvoiceList != null && dsInvoiceList.Tables.Count > 0)
                    {
                        //Line invoiceLine = new Line();
                        //List<Line> invoiceLine = new List<Line>();

                        for (int i = 0; i < dsInvoiceList.Tables[0].Rows.Count; i++)
                        {
                            string memo = "";
                            Line invoiceLineone = new Line();
                            SalesItemLineDetail saleItemDetail = new SalesItemLineDetail();
                            {
                                saleItemDetail.Qty = 1; // new Decimal(SeatCount); // bkgDetails.SeatCount);
                                saleItemDetail.QtySpecified = true;
                                saleItemDetail.AnyIntuitObject = Convert.ToDecimal(dsInvoiceList.Tables[0].Rows[i]["ActualAmount"].ToString()); //bkgDetails.Fare; // Amount / SeatCount; // 2500m;
                                saleItemDetail.ItemElementName = ItemChoiceType.UnitPrice;
                            }
                            invoiceLineone.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                            invoiceLineone.DetailTypeSpecified = true;

                            invoiceLineone.Amount = Convert.ToDecimal(dsInvoiceList.Tables[0].Rows[i]["ActualAmount"].ToString());// bkgDetails.Fare * bkgDetails.SeatCount; // 1000; // Amount;
                            invoiceLineone.AmountSpecified = true;
                            invoiceLineone.AnyIntuitObject = saleItemDetail;
                            if (Convert.ToString(dsInvoiceList.Tables[0].Rows[i]["IsDisputed"]) == "1")
                                memo = "Disputed -- Automaticaly posted from CRS";
                            else
                                memo = "-- Automaticaly posted from CRS";
                            invoiceLineone.Description = dsInvoiceList.Tables[0].Rows[i]["bookingid"].ToString() + "\n" + memo;
                            CreditMemoLineList.Add(invoiceLineone);
                        }

                    }

                    #endregion

                    // CreditMemoLineList.Add(invoiceLineone);

                    /*
                    DiscountLineDetail DiscLineDetail = new DiscountLineDetail();
                    DiscLineDetail.DiscountPercentSpecified = false;
                    DiscLineDetail.DiscountAccountRef = new ReferenceType() { name = "Discounts given", Value = "84" };

                    Line DiscLine = new Line();
                    DiscLine.DetailType = LineDetailTypeEnum.DiscountLineDetail;
                    DiscLine.DetailTypeSpecified = true;
                    DiscLine.Amount = bkgDetails.Comm;
                    DiscLine.AmountSpecified = true;
                    DiscLine.AnyIntuitObject = DiscLineDetail;

                    CreditMemoLineList.Add(DiscLine);
                    */
                    //}

                    creditMemo.TotalAmt = Amount;
                    creditMemo.TotalAmtSpecified = true;


                    //TxnTaxDetail
                    TxnTaxDetail txnTax = new TxnTaxDetail();
                    {
                        txnTax.TotalTaxSpecified = true;
                        txnTax.TotalTax = 0;
                    }

                    creditMemo.GlobalTaxCalculation = GlobalTaxCalculationEnum.NotApplicable;
                    creditMemo.GlobalTaxCalculationSpecified = true;

                    creditMemo.PrivateNote = "[Voucher No : " + docformattednumber + "], " + "-- Automaticaly posted from CRS";

                    /*
                    TxnTaxDetail txnTaxDetail = new TxnTaxDetail();
                    txnTaxDetail.TxnTaxCodeRef = new ReferenceType()
                    {
                        name = "Service Tax",
                        Value = "4"
                    };
                    Line taxLine = new Line();
                    taxLine.DetailType = LineDetailTypeEnum.TaxLineDetail;
                    TaxLineDetail taxLineDetail = new TaxLineDetail();
                    //Assigning the fist Tax Rate in this Tax Code
                    taxLineDetail.TaxRateRef = new ReferenceType()
                    {
                        name = "Service Tax",
                        Value = "4"
                    };

                    taxLine.AnyIntuitObject = taxLineDetail;
                    txnTaxDetail.TaxLine = new Line[] { taxLine };
                    invoice.TxnTaxDetail = txnTaxDetail;


                    invoice.ApplyTaxAfterDiscount = false;
                    invoice.ApplyTaxAfterDiscountSpecified = true;
                    */

                    //invoice.TxnTaxDetail = new TxnTaxDetail();
                    //invoice.TxnTaxDetail.TotalTax = new Decimal(0);
                    //invoice.TxnTaxDetail.TotalTaxSpecified = true;



                    //invoice.DiscountAmt = new decimal(80);
                    //invoice.DiscountAmtSpecified = true;
                    //invoice.DiscountRateSpecified = false;


                    creditMemo.Line = CreditMemoLineList.ToArray();
                }
                CreditMemo postedCreditMemo = service.Add(creditMemo);

                return postedCreditMemo;
                /* Credit Memo End*/


            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }
        private void UpdateQuickBookCreditNote(Int32 cnID, Int32 VoucherCreditNoteId)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_VoucherCreditNoteid", VoucherCreditNoteId, ParameterDirection.Input);
                dal.AddParameter("p_CreditNoteID", cnID, ParameterDirection.Input);

                dal.ExecuteDML("spUpdateQuickBookCreditNote", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Agent Cancellation Credit Note

        public void PostAgentVouchersCancellation()
        {
            #region AgentCancellation Posting in CreditNote

            List<AgentVoucherDetails> agentCreditNoteList = null;
            try
            {
                agentCreditNoteList = GetAgentCancellationDetailsFromCRS();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("PostAgentVouchers", "GetAgentVoucherDetailsFromCRS_Konduskar", ex.Message, true);
            }

            if (agentCreditNoteList != null && agentCreditNoteList.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                Logger.WriteLog("PostAgentVouchersCancellation_Konduskar", "", "No Of Agent Vouchers Cancellation: " + agentCreditNoteList.Count, true);
                foreach (AgentVoucherDetails avDetails in agentCreditNoteList)
                {
                    CreditMemo ivPosted;
                    Payment paymentPosted;
                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    Int32 ivID1 = -1;
                    try
                    {
                        string docnumber = "";
                        string docformattednumber = "";

                        CreateVoucherNoForQB(avDetails.CompanyID, Convert.ToInt32(avDetails.DivisionID), avDetails.BookingID, avDetails.GeneratedDate, ref docnumber, ref docformattednumber,"CN");
                        

                        ivPosted = PostCredtNote("Insert", avDetails.AgentID,avDetails.invoiceID.ToString(), "", avDetails.AgentName, avDetails.QuickBookCustomerID, avDetails.FromCityName, avDetails.ToCityName, avDetails.RouteFromCityName, avDetails.RouteToCityName, avDetails.BusNumber, avDetails.AgentPhone1, avDetails.AgentPhone2, avDetails.Amount, avDetails.PNR, avDetails.PassengerName, avDetails.SeatNos, avDetails.SeatCount, avDetails.FromTo, avDetails.JDate, avDetails.JTime, avDetails.BDate, avDetails.BookingID, avDetails.VoucherDate, avDetails.TransactionID, avDetails.BusType, avDetails.TripCode, avDetails.ItemName, avDetails.ItemID, avDetails.ClassName, avDetails.ClassID, avDetails.BranchDivisionName, avDetails.BranchDivisionID, docformattednumber, avDetails.BookingStatus, avDetails.Prefix, avDetails.TotalFare, avDetails.RefundAmount, avDetails.PickUpName, avDetails.DropOffName, avDetails.GST, avDetails.AgentComm, avDetails.GSTType);
                        ivID = Convert.ToInt32(ivPosted.Id);
                        InsertUpdateCreditNotePostingDetailsToCRS(avDetails.AgentID, avDetails.CompanyID, avDetails.BookingID, ivID, docnumber, docformattednumber, avDetails.BusMasterID, avDetails.DivisionID, 0, avDetails.VoucherDate, avDetails.RefundAmount);
                        if (ivID > 0)
                        {
                            paymentPosted = PostPaymentAfterCreditNote(false, avDetails.AgentName, avDetails.QuickBookCustomerID, avDetails.invoiceID, avDetails.RefundAmount, "", 0, "", "", 0, Convert.ToString(avDetails.VoucherDate),0,"", 0,"", 0, "", 0, 0, 0, avDetails.BookingID, "", 0, "", 0, docformattednumber, ivID);
                            ivID1 = Convert.ToInt32(paymentPosted.Id);
                        }
                     
                        status = "Posted";
                        statusMessage = "";

                        EntryCounter.GetInstance().IncreaseQBCount(1);

                        InsertUpdateCreditNotePostingDetailsToCRS(avDetails.AgentID, avDetails.CompanyID, avDetails.BookingID, ivID, docnumber, docformattednumber, avDetails.BusMasterID, avDetails.DivisionID,ivID1,avDetails.VoucherDate,avDetails.RefundAmount);

                        EntryCounter.GetInstance().IncreaseCRSCount(1);
                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("PostAgentVouchersCancellation_Konduskar", "", ex.Message, true);
                    }

                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "PostAgentVouchersCancellation_Konduskar:::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }


            #endregion

            return;
        }

        #endregion


        #region Branch Booking Posting

        private DataSet GetBranchBookingData()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                DateTime fromDate = new DateTime(2016, 06, 07);
                DateTime toDate = new DateTime(2016, 06, 07);
                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
                dal.AddParameter("p_FromDate", fromDate, ParameterDirection.Input);
                dal.AddParameter("p_ToDate", toDate, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetBranchBookingDataForQuickBooksKonduskar", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false,"",false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DataSet GetBranchCancellationData()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                DateTime fromDate = new DateTime(2016, 06, 07);
                DateTime toDate = new DateTime(2016, 06, 07);
                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
                dal.AddParameter("p_FromDate", fromDate, ParameterDirection.Input);
                dal.AddParameter("p_ToDate", toDate, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetBranchCancellationDataForQuickBooks", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false,"",false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<SalesReceiptDetails> GetBranchData()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 69, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetBranchSalesEntry", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false,"",false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<SalesReceiptDetails> salereceiptDetailsList = new List<SalesReceiptDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drSaleReceiptDetails in dstOutPut.Tables[0].Rows)
                    {
                        SalesReceiptDetails salereceiptDetails = new SalesReceiptDetails();
                        try
                        {
                            Int32 qbCustomerId, divisionId, tripid, Merge;
                            qbCustomerId = 0;
                            divisionId = 0;
                            tripid = 0;
                            Merge = 0;
                            if (!Int32.TryParse(drSaleReceiptDetails["QuickBookCustomerID"].ToString(), out qbCustomerId))
                            {
                                Logger.WriteLog("GetBranchData::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString());
                                string msg = "GetBranchData::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString();
                                Email.SendMail(msg);

                            }
                            else if (!Int32.TryParse(drSaleReceiptDetails["divisionid"].ToString(), out divisionId))
                            {
                                Logger.WriteLog("GetBranchData::Validation::Invalid DivisionId for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString());
                                string msg = "GetBranchData::Validation::Invalid DivisionId for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }

                            else if (!Int32.TryParse(drSaleReceiptDetails["ItemID"].ToString(), out tripid))
                            {
                                Logger.WriteLog("GetBranchData::Validation::Invalid tripid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString());
                                string msg = "GetBranchData::Validation::Invalid tripid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }
                            else if (!Int32.TryParse(drSaleReceiptDetails["IsMerge"].ToString(), out Merge))
                            {
                                Logger.WriteLog("GetBranchData::Validation::Invalid Classid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " BusId: " + drSaleReceiptDetails["BusId"].ToString());
                                string msg = "GetBranchData::Validation::Invalid tripid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }
                            else
                            {
                                salereceiptDetails.BranchID = Convert.ToInt32(drSaleReceiptDetails["BranchId"].ToString());
                                salereceiptDetails.BranchName = drSaleReceiptDetails["BranchName"].ToString();
                                salereceiptDetails.QuickBookCustomerID = Convert.ToInt32(drSaleReceiptDetails["QuickBookCustomerID"].ToString());
                                salereceiptDetails.FromCityName = drSaleReceiptDetails["FromCityName"].ToString();
                                salereceiptDetails.ToCityName = drSaleReceiptDetails["ToCityName"].ToString();
                                salereceiptDetails.RouteFromCityName = drSaleReceiptDetails["RouteFromCityName"].ToString();
                                salereceiptDetails.RouteToCityName = drSaleReceiptDetails["RouteToCityName"].ToString();
                                salereceiptDetails.BusNumber = drSaleReceiptDetails["BusNumber"].ToString();
                                salereceiptDetails.BusType = drSaleReceiptDetails["ChartName"].ToString();
                                salereceiptDetails.BranchPhone1 = drSaleReceiptDetails["ContactNo1"].ToString();
                                salereceiptDetails.BranchPhone2 = drSaleReceiptDetails["ContactNo2"].ToString();
                                salereceiptDetails.Amount = Convert.ToDecimal(drSaleReceiptDetails["NetAmt"].ToString());
                                salereceiptDetails.PNR = drSaleReceiptDetails["PNR"].ToString();
                                salereceiptDetails.PassengerName = drSaleReceiptDetails["PassengerName"].ToString();
                                salereceiptDetails.SeatNos = drSaleReceiptDetails["SeatNos"].ToString();
                                salereceiptDetails.SeatCount = Convert.ToInt32(drSaleReceiptDetails["SeatCount"].ToString());
                                salereceiptDetails.FromTo = drSaleReceiptDetails["FromTo"].ToString();
                                salereceiptDetails.JDate = drSaleReceiptDetails["JourneyDate"].ToString();
                                salereceiptDetails.JTime = drSaleReceiptDetails["JTime"].ToString();
                                salereceiptDetails.BDate = drSaleReceiptDetails["BookingDate"].ToString();
                                salereceiptDetails.BookingID = Convert.ToInt32(drSaleReceiptDetails["BookingID"].ToString());
                                //salereceiptDetails.GeneratedDate = Convert.ToDateTime(drSaleReceiptDetails["GeneratedDate"].ToString());
                                salereceiptDetails.TransactionID = Convert.ToInt32(drSaleReceiptDetails["TransactionID"].ToString());
                                if (drSaleReceiptDetails["ItemID"].ToString() != "")
                                    salereceiptDetails.ItemID = Convert.ToInt32(drSaleReceiptDetails["ItemID"].ToString());
                                salereceiptDetails.ItemName = drSaleReceiptDetails["Item"].ToString();
                                salereceiptDetails.ClassID = drSaleReceiptDetails["ClassID"].ToString();
                                salereceiptDetails.ClassName = drSaleReceiptDetails["classname"].ToString();
                                salereceiptDetails.BranchDivisionID = drSaleReceiptDetails["BranchDivisionID"].ToString();
                                salereceiptDetails.BranchDivisionName = drSaleReceiptDetails["BranchDivisionName"].ToString();
                                salereceiptDetails.CompanyID = Convert.ToInt32(drSaleReceiptDetails["CompanyID"].ToString());
                                //salereceiptDetails.VoucherNo = drSaleReceiptDetails["VoucherNo"].ToString();
                                salereceiptDetails.BookingStatus = drSaleReceiptDetails["BookingStatus"].ToString();
                                salereceiptDetails.Prefix = drSaleReceiptDetails["Prefix"].ToString();

                                salereceiptDetails.TotalFare = Convert.ToDecimal(drSaleReceiptDetails["TotalFare"].ToString());
                                salereceiptDetails.RefundAmount = Convert.ToDecimal(drSaleReceiptDetails["RefundAmount"].ToString());

                                // agentVoucherDetails.DivisionID = Convert.ToInt32(drAgentVoucherDetails["divisionid"].ToString());
                                salereceiptDetails.BusMasterID = Convert.ToInt32(drSaleReceiptDetails["BusMasterId"].ToString());
                                salereceiptDetails.SalesReceiptDate = Convert.ToDateTime(drSaleReceiptDetails["VoucherDate"].ToString());
                                salereceiptDetails.PickUpName = drSaleReceiptDetails["pickupname"].ToString();
                                salereceiptDetails.DropOffName = drSaleReceiptDetails["dropoffname"].ToString();
                                salereceiptDetails.SeatCount = Convert.ToInt32(drSaleReceiptDetails["SeatCount"].ToString());
                                salereceiptDetails.SeatNos = drSaleReceiptDetails["SeatNos"].ToString();
                                salereceiptDetails.GST = Convert.ToDecimal(drSaleReceiptDetails["GST"].ToString());
                                salereceiptDetails.BranchCommission = Convert.ToDecimal(drSaleReceiptDetails["BranchComm"].ToString());
                                salereceiptDetails.GSTType = drSaleReceiptDetails["GSTType"].ToString();
                                salereceiptDetails.BaseFare = Convert.ToDecimal(drSaleReceiptDetails["Basefare"].ToString());
                                salereceiptDetails.PlaceOfSupply = drSaleReceiptDetails["branchstatename"].ToString();
                                salereceiptDetails.DepositLedgerName = drSaleReceiptDetails["DepositLedgerName"].ToString();
                                salereceiptDetails.DepositLedgerId = Convert.ToInt32(drSaleReceiptDetails["DepositLedgerId"].ToString());
                                salereceiptDetailsList.Add(salereceiptDetails);
                            }

                        }

                        catch (Exception ex)
                        {
                            Logger.WriteLog("GetBranchData", " Branchid: " + salereceiptDetails.BranchID + " VoucherNo: " + salereceiptDetails.VoucherNo, ex.Message, true);
                        }

                    }

                }

                return salereceiptDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private List<SalesReceiptDetails> GetUpdateBranchData()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 69, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetUpdatedBranchSalesEntry", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<SalesReceiptDetails> salereceiptDetailsList = new List<SalesReceiptDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drSaleReceiptDetails in dstOutPut.Tables[0].Rows)
                    {
                        SalesReceiptDetails salereceiptDetails = new SalesReceiptDetails();
                        try
                        {
                            Int32 qbCustomerId, divisionId, tripid, Merge;
                            qbCustomerId = 0;
                            divisionId = 0;
                            tripid = 0;
                            Merge = 0;
                           
                            if (!Int32.TryParse(drSaleReceiptDetails["ClassID"].ToString(), out Merge))
                            {
                                Logger.WriteLog("GetUpdateBranchData::Validation::Invalid Classid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " BusId: " + drSaleReceiptDetails["BusId"].ToString());
                                string msg = "GetUpdateBranchData::Validation::Invalid tripid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }
                            else
                            {
                                salereceiptDetails.BranchID = Convert.ToInt32(drSaleReceiptDetails["BranchId"].ToString());
                                salereceiptDetails.BranchName = drSaleReceiptDetails["BranchName"].ToString();
                                salereceiptDetails.QuickBookCustomerID = Convert.ToInt32(drSaleReceiptDetails["QuickBookCustomerID"].ToString());
                                salereceiptDetails.FromCityName = drSaleReceiptDetails["FromCityName"].ToString();
                                salereceiptDetails.ToCityName = drSaleReceiptDetails["ToCityName"].ToString();
                                salereceiptDetails.RouteFromCityName = drSaleReceiptDetails["RouteFromCityName"].ToString();
                                salereceiptDetails.RouteToCityName = drSaleReceiptDetails["RouteToCityName"].ToString();
                                salereceiptDetails.BusNumber = drSaleReceiptDetails["BusNumber"].ToString();
                                salereceiptDetails.BusType = drSaleReceiptDetails["ChartName"].ToString();
                                salereceiptDetails.BranchPhone1 = drSaleReceiptDetails["ContactNo1"].ToString();
                                salereceiptDetails.BranchPhone2 = drSaleReceiptDetails["ContactNo2"].ToString();
                                salereceiptDetails.Amount = Convert.ToDecimal(drSaleReceiptDetails["NetAmt"].ToString());
                                salereceiptDetails.PNR = drSaleReceiptDetails["PNR"].ToString();
                                salereceiptDetails.PassengerName = drSaleReceiptDetails["PassengerName"].ToString();
                                salereceiptDetails.SeatNos = drSaleReceiptDetails["SeatNos"].ToString();
                                salereceiptDetails.SeatCount = Convert.ToInt32(drSaleReceiptDetails["SeatCount"].ToString());
                                salereceiptDetails.FromTo = drSaleReceiptDetails["FromTo"].ToString();
                                salereceiptDetails.JDate = drSaleReceiptDetails["JourneyDate"].ToString();
                                salereceiptDetails.JTime = drSaleReceiptDetails["JTime"].ToString();
                                salereceiptDetails.BDate = drSaleReceiptDetails["BookingDate"].ToString();
                                salereceiptDetails.BookingID = Convert.ToInt32(drSaleReceiptDetails["BookingID"].ToString());
                                //salereceiptDetails.GeneratedDate = Convert.ToDateTime(drSaleReceiptDetails["GeneratedDate"].ToString());
                                salereceiptDetails.TransactionID = Convert.ToInt32(drSaleReceiptDetails["TransactionID"].ToString());
                                if (drSaleReceiptDetails["ItemID"].ToString() != "")
                                    salereceiptDetails.ItemID = Convert.ToInt32(drSaleReceiptDetails["ItemID"].ToString());
                                salereceiptDetails.ItemName = drSaleReceiptDetails["Item"].ToString();
                                salereceiptDetails.ClassID = drSaleReceiptDetails["ClassID"].ToString();
                                salereceiptDetails.ClassName = drSaleReceiptDetails["classname"].ToString();
                                salereceiptDetails.BranchDivisionID = drSaleReceiptDetails["BranchDivisionID"].ToString();
                                salereceiptDetails.BranchDivisionName = drSaleReceiptDetails["BranchDivisionName"].ToString();
                                salereceiptDetails.CompanyID = Convert.ToInt32(drSaleReceiptDetails["CompanyID"].ToString());
                                //salereceiptDetails.VoucherNo = drSaleReceiptDetails["VoucherNo"].ToString();
                                salereceiptDetails.BookingStatus = drSaleReceiptDetails["BookingStatus"].ToString();
                                salereceiptDetails.Prefix = drSaleReceiptDetails["Prefix"].ToString();

                                salereceiptDetails.TotalFare = Convert.ToDecimal(drSaleReceiptDetails["TotalFare"].ToString());
                                salereceiptDetails.RefundAmount = Convert.ToDecimal(drSaleReceiptDetails["RefundAmount"].ToString());

                                // agentVoucherDetails.DivisionID = Convert.ToInt32(drAgentVoucherDetails["divisionid"].ToString());
                                salereceiptDetails.BusMasterID = Convert.ToInt32(drSaleReceiptDetails["BusMasterId"].ToString());
                                salereceiptDetails.SalesReceiptDate = Convert.ToDateTime(drSaleReceiptDetails["VoucherDate"].ToString());
                                salereceiptDetails.PickUpName = drSaleReceiptDetails["pickupname"].ToString();
                                salereceiptDetails.DropOffName = drSaleReceiptDetails["dropoffname"].ToString();
                                salereceiptDetails.SeatCount = Convert.ToInt32(drSaleReceiptDetails["SeatCount"].ToString());
                                salereceiptDetails.SeatNos = drSaleReceiptDetails["SeatNos"].ToString();
                                salereceiptDetails.GST = Convert.ToDecimal(drSaleReceiptDetails["GST"].ToString());
                                salereceiptDetails.BranchCommission = Convert.ToDecimal(drSaleReceiptDetails["BranchComm"].ToString());
                                salereceiptDetails.GSTType = drSaleReceiptDetails["GSTType"].ToString();
                                salereceiptDetails.BaseFare = Convert.ToDecimal(drSaleReceiptDetails["Basefare"].ToString());
                                salereceiptDetails.PlaceOfSupply = drSaleReceiptDetails["branchstatename"].ToString();
                                salereceiptDetails.DepositLedgerName = drSaleReceiptDetails["DepositLedgerName"].ToString();
                                salereceiptDetails.DepositLedgerId = Convert.ToInt32(drSaleReceiptDetails["DepositLedgerId"].ToString());
                                salereceiptDetails.Accsysid = drSaleReceiptDetails["SalesReceiptQBID"].ToString();
                                salereceiptDetailsList.Add(salereceiptDetails);
                            }

                        }

                        catch (Exception ex)
                        {
                            Logger.WriteLog("GetUpdateBranchData", " Branchid: " + salereceiptDetails.BranchID + " VoucherNo: " + salereceiptDetails.VoucherNo, ex.Message, true);
                        }

                    }

                }

                return salereceiptDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private List<SalesReceiptDetails> GetBranchCancellation()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 69, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetUpdatedBranchSalesEntry", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<SalesReceiptDetails> salereceiptDetailsList = new List<SalesReceiptDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drSaleReceiptDetails in dstOutPut.Tables[0].Rows)
                    {
                        SalesReceiptDetails salereceiptDetails = new SalesReceiptDetails();
                        try
                        {
                            Int32 qbCustomerId, divisionId, tripid, Merge;
                            qbCustomerId = 0;
                            divisionId = 0;
                            tripid = 0;
                            Merge = 0;

                            if (!Int32.TryParse(drSaleReceiptDetails["ClassID"].ToString(), out Merge))
                            {
                                Logger.WriteLog("GetUpdateBranchData::Validation::Invalid Classid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " BusId: " + drSaleReceiptDetails["BusId"].ToString());
                                string msg = "GetUpdateBranchData::Validation::Invalid tripid for VoucherNo: " + drSaleReceiptDetails["VoucherNo"].ToString() + " AgentId: " + drSaleReceiptDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }
                            else
                            {
                                salereceiptDetails.BranchID = Convert.ToInt32(drSaleReceiptDetails["BranchId"].ToString());
                                salereceiptDetails.BranchName = drSaleReceiptDetails["BranchName"].ToString();
                                salereceiptDetails.QuickBookCustomerID = Convert.ToInt32(drSaleReceiptDetails["QuickBookCustomerID"].ToString());
                                salereceiptDetails.FromCityName = drSaleReceiptDetails["FromCityName"].ToString();
                                salereceiptDetails.ToCityName = drSaleReceiptDetails["ToCityName"].ToString();
                                salereceiptDetails.RouteFromCityName = drSaleReceiptDetails["RouteFromCityName"].ToString();
                                salereceiptDetails.RouteToCityName = drSaleReceiptDetails["RouteToCityName"].ToString();
                                salereceiptDetails.BusNumber = drSaleReceiptDetails["BusNumber"].ToString();
                                salereceiptDetails.BusType = drSaleReceiptDetails["ChartName"].ToString();
                                salereceiptDetails.BranchPhone1 = drSaleReceiptDetails["ContactNo1"].ToString();
                                salereceiptDetails.BranchPhone2 = drSaleReceiptDetails["ContactNo2"].ToString();
                                salereceiptDetails.Amount = Convert.ToDecimal(drSaleReceiptDetails["NetAmt"].ToString());
                                salereceiptDetails.PNR = drSaleReceiptDetails["PNR"].ToString();
                                salereceiptDetails.PassengerName = drSaleReceiptDetails["PassengerName"].ToString();
                                salereceiptDetails.SeatNos = drSaleReceiptDetails["SeatNos"].ToString();
                                salereceiptDetails.SeatCount = Convert.ToInt32(drSaleReceiptDetails["SeatCount"].ToString());
                                salereceiptDetails.FromTo = drSaleReceiptDetails["FromTo"].ToString();
                                salereceiptDetails.JDate = drSaleReceiptDetails["JourneyDate"].ToString();
                                salereceiptDetails.JTime = drSaleReceiptDetails["JTime"].ToString();
                                salereceiptDetails.BDate = drSaleReceiptDetails["BookingDate"].ToString();
                                salereceiptDetails.BookingID = Convert.ToInt32(drSaleReceiptDetails["BookingID"].ToString());
                                //salereceiptDetails.GeneratedDate = Convert.ToDateTime(drSaleReceiptDetails["GeneratedDate"].ToString());
                                salereceiptDetails.TransactionID = Convert.ToInt32(drSaleReceiptDetails["TransactionID"].ToString());
                                if (drSaleReceiptDetails["ItemID"].ToString() != "")
                                    salereceiptDetails.ItemID = Convert.ToInt32(drSaleReceiptDetails["ItemID"].ToString());
                                salereceiptDetails.ItemName = drSaleReceiptDetails["Item"].ToString();
                                salereceiptDetails.ClassID = drSaleReceiptDetails["ClassID"].ToString();
                                salereceiptDetails.ClassName = drSaleReceiptDetails["classname"].ToString();
                                salereceiptDetails.BranchDivisionID = drSaleReceiptDetails["BranchDivisionID"].ToString();
                                salereceiptDetails.BranchDivisionName = drSaleReceiptDetails["BranchDivisionName"].ToString();
                                salereceiptDetails.CompanyID = Convert.ToInt32(drSaleReceiptDetails["CompanyID"].ToString());
                                //salereceiptDetails.VoucherNo = drSaleReceiptDetails["VoucherNo"].ToString();
                                salereceiptDetails.BookingStatus = drSaleReceiptDetails["BookingStatus"].ToString();
                                salereceiptDetails.Prefix = drSaleReceiptDetails["Prefix"].ToString();

                                salereceiptDetails.TotalFare = Convert.ToDecimal(drSaleReceiptDetails["TotalFare"].ToString());
                                salereceiptDetails.RefundAmount = Convert.ToDecimal(drSaleReceiptDetails["RefundAmount"].ToString());

                                // agentVoucherDetails.DivisionID = Convert.ToInt32(drAgentVoucherDetails["divisionid"].ToString());
                                salereceiptDetails.BusMasterID = Convert.ToInt32(drSaleReceiptDetails["BusMasterId"].ToString());
                                salereceiptDetails.SalesReceiptDate = Convert.ToDateTime(drSaleReceiptDetails["VoucherDate"].ToString());
                                salereceiptDetails.PickUpName = drSaleReceiptDetails["pickupname"].ToString();
                                salereceiptDetails.DropOffName = drSaleReceiptDetails["dropoffname"].ToString();
                                salereceiptDetails.SeatCount = Convert.ToInt32(drSaleReceiptDetails["SeatCount"].ToString());
                                salereceiptDetails.SeatNos = drSaleReceiptDetails["SeatNos"].ToString();
                                salereceiptDetails.GST = Convert.ToDecimal(drSaleReceiptDetails["GST"].ToString());
                                salereceiptDetails.BranchCommission = Convert.ToDecimal(drSaleReceiptDetails["BranchComm"].ToString());
                                salereceiptDetails.GSTType = drSaleReceiptDetails["GSTType"].ToString();
                                salereceiptDetails.BaseFare = Convert.ToDecimal(drSaleReceiptDetails["Basefare"].ToString());
                                salereceiptDetails.PlaceOfSupply = drSaleReceiptDetails["branchstatename"].ToString();
                                salereceiptDetails.DepositLedgerName = drSaleReceiptDetails["DepositLedgerName"].ToString();
                                salereceiptDetails.DepositLedgerId = Convert.ToInt32(drSaleReceiptDetails["DepositLedgerId"].ToString());
                                salereceiptDetails.Accsysid = drSaleReceiptDetails["SalesReceiptQBID"].ToString();
                                salereceiptDetailsList.Add(salereceiptDetails);
                            }

                        }

                        catch (Exception ex)
                        {
                            Logger.WriteLog("GetUpdateBranchData", " Branchid: " + salereceiptDetails.BranchID + " VoucherNo: " + salereceiptDetails.VoucherNo, ex.Message, true);
                        }

                    }

                }

                return salereceiptDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataSet GetBranchBookingWithBusNumber()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetBranchBookingWithBusNumber", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static SalesReceipt GetSalesReceipt(string SRID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<SalesReceipt> queryServiceForPurchase = new Intuit.Ipp.QueryFilter.QueryService<SalesReceipt>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<SalesReceipt> SRList = queryServiceForPurchase.ExecuteIdsQuery("select * from SalesReceipt  where id = '" + SRID + "'");
                SalesReceipt iv = SRList.Take(1).FirstOrDefault();
                //Purchase iPurchase = PurchaseList.Take(1).FirstOrDefault();
                return iv;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }

        private void InsertUpdateBookingSalesPostingDetailsToCRS(Int32 BranchID, Int32 CompanyID, Int32 BookingID, Int32 AccSysID, string docnumber, string docformattednumber, int classid, int divisionid,decimal Basefare,DateTime SaleReceiptDateTime,int LedgerID)
        {

            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_BranchID", BranchID, ParameterDirection.Input);
                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_BookingID", BookingID, ParameterDirection.Input);
                dal.AddParameter("p_AccSysID", AccSysID, ParameterDirection.Input);
                dal.AddParameter("p_docnumber", docnumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_docformattednumber", "SRCRS" + docformattednumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_classid", classid, ParameterDirection.Input);
                dal.AddParameter("p_divisionid", divisionid, ParameterDirection.Input);
                dal.AddParameter("p_SalesReceiptAmt", Basefare, ParameterDirection.Input);
                dal.AddParameter("p_SaleReceiptDateTime", SaleReceiptDateTime, ParameterDirection.Input);
                dal.AddParameter("p_LedgerID", LedgerID, ParameterDirection.Input);

                dal.ExecuteDML("spInsertUpdateQuickBookSalesReceipt", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsertBranchBookings()
        {
            DataSet ds = null;

            try
            {
                ds = GetBranchBookingData();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetBranchBookingData_konduskar", "", ex.Message, true);
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                Logger.WriteLog("InsertBranchBookings_konduskar", "", "No Of Bookings: " + ds.Tables[0].Rows.Count, true);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        decimal amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalFare"].ToString());
                        int creditledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["CreditLedgerId"].ToString());
                        string creditledgername = ds.Tables[0].Rows[i]["CreditLedgerName"].ToString();
                        int debitledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["DebitLedgerId"].ToString());
                        string debitledgername = ds.Tables[0].Rows[i]["DebitLedgerName"].ToString();
                        
                        int branchdivisionid = Convert.ToInt32(ds.Tables[0].Rows[i]["BranchDivisionIDJE"].ToString());
                        string branchdivisionname = ds.Tables[0].Rows[i]["BranchDivisionNameJE"].ToString();
                        int UserID = Convert.ToInt32(ds.Tables[0].Rows[i]["UserIdBooked"]);
                        int CompanyID = Convert.ToInt32(ds.Tables[0].Rows[i]["CompanyID"]);
                        int bookingid = Convert.ToInt32(ds.Tables[0].Rows[i]["bookingid"]);
                        DateTime bookingDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["bookingdate"].ToString());

                        string memo = "BookingId - " + bookingid.ToString();
                        int Toutledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["Toutledgerid"].ToString());
                        string Toutledgername = ds.Tables[0].Rows[i]["Toutledgername"].ToString();
                        decimal ToutFare = Convert.ToDecimal(ds.Tables[0].Rows[i]["ToutFare"].ToString());


                        InsertSaleEntryPostingToCRS("", "", "", "", "0", Toutledgerid, Toutledgername, ToutFare, bookingDateTime, "", creditledgerid, "", branchdivisionid, 0, debitledgerid, "", branchdivisionid, 0, amount, CompanyID, UserID, "Book", bookingid);
                    }
                    catch (Exception ex)
                    {

                        Logger.WriteLog("InsertSaleEntryPostingToCRS", "", ex.Message, true);
                    }

                }
            }
            else
            {
                Logger.WriteLog("InsertBranchBookings_konduskar", "", "No Of Bookings: 0", true);
            }
        }

        public void InsertBranchCancellations()
        {
            DataSet ds = null;
            try
            {
                ds = GetBranchCancellationData();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("InsertBranchCancellations_Konduskar", "", ex.Message, true);
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                Logger.WriteLog("InsertBranchCancellations_Konduskar", "", "No Of Cancellations: " + ds.Tables[0].Rows.Count, true);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        decimal amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["RefundAmount"].ToString());
                        int creditledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["CreditLedgerId"].ToString());
                        string creditledgername = ds.Tables[0].Rows[i]["CreditLedgerName"].ToString();
                        int debitledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["DebitLedgerId"].ToString());
                        string debitledgername = ds.Tables[0].Rows[i]["DebitLedgerName"].ToString();
                        int branchdivisionid = Convert.ToInt32(ds.Tables[0].Rows[i]["BranchDivisionIDJE"].ToString());
                        string branchdivisionname = ds.Tables[0].Rows[i]["BranchDivisionNameJE"].ToString();
                        int UserID = Convert.ToInt32(ds.Tables[0].Rows[i]["UserIdCancelled"]);
                        int CompanyID = Convert.ToInt32(ds.Tables[0].Rows[i]["CompanyID"]);
                        int bookingid = Convert.ToInt32(ds.Tables[0].Rows[i]["bookingid"]);
                        DateTime cancelDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["Canceldate"].ToString());

                        string memo = "BookingId - " + bookingid.ToString();

                        //JournalEntry JK = PostJournalEntry("", 0, amount, "", creditledgername, creditledgerid, debitledgername, debitledgerid, memo, branchdivisionid, branchdivisionname);
                        //DateTime journaldatetime = Util.Util.GetServerDateTime();

                        InsertSaleEntryPostingToCRS("", "", "", "", "0",0,"",0,cancelDate, "", creditledgerid, "", branchdivisionid, 0, debitledgerid, "", branchdivisionid, 0, amount, CompanyID, UserID, "Cancel", bookingid);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("InsertBranchCancellations_Konduskar", "InsertJournalEntryPostingToCRS", ex.Message, true);
                    }

                }
            }
        }

        public void PostBranchSalesEntry()
        {
            DataSet ds = null;
            List<SalesReceiptDetails> SaleReceiptDetailList = null;
            try
            {
                SaleReceiptDetailList = GetBranchData();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetBranchData", "", ex.Message, true);
            }

            if (SaleReceiptDetailList != null && SaleReceiptDetailList.Count > 0)
            {
              #region Booking Sale Posting
                EntryCounter.GetInstance().ResetAllCount();

                foreach (SalesReceiptDetails srdetails in SaleReceiptDetailList)
                {
                   
                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    try
                    {
                        string docnumber = "";
                        string docformattednumber = "";

                        CreateVoucherNoForQB(srdetails.CompanyID, Convert.ToInt32(srdetails.DivisionID), srdetails.BookingID, srdetails.GeneratedDate, ref docnumber, ref docformattednumber,"SR");

                        SalesReceipt JK = PostSaleEntry("Insert", srdetails.BranchID, "", "", srdetails.BranchName, srdetails.QuickBookCustomerID, srdetails.FromCityName, srdetails.ToCityName, srdetails.RouteFromCityName, srdetails.RouteToCityName, srdetails.BusNumber, srdetails.BranchPhone1, srdetails.BranchPhone2, srdetails.Amount, srdetails.PNR, srdetails.PassengerName, srdetails.SeatNos, srdetails.SeatCount, srdetails.FromTo, srdetails.JDate, srdetails.JTime, srdetails.BDate, srdetails.BookingID, srdetails.SalesReceiptDate, srdetails.TransactionID, srdetails.BusType, "", srdetails.ItemName, srdetails.ItemID, srdetails.ClassName, srdetails.ClassID, srdetails.BranchDivisionName, srdetails.BranchDivisionID, docformattednumber, srdetails.BookingStatus, srdetails.Prefix, srdetails.TotalFare, srdetails.RefundAmount, srdetails.PickUpName, srdetails.DropOffName, srdetails.DepositLedgerId, srdetails.DepositLedgerName,srdetails.BaseFare,srdetails.GST,srdetails.GSTType,srdetails.PlaceOfSupply,srdetails.BranchCommission);
                        ivID = Convert.ToInt32(JK.Id);
                        status = "Posted";
                        statusMessage = "";

                        EntryCounter.GetInstance().IncreaseQBCount(1);

                        InsertUpdateBookingSalesPostingDetailsToCRS(srdetails.BranchID, srdetails.CompanyID, srdetails.BookingID, ivID,docnumber,docformattednumber, srdetails.BusMasterID, srdetails.DivisionID,srdetails.Amount,srdetails.SalesReceiptDate,srdetails.DepositLedgerId);

                        EntryCounter.GetInstance().IncreaseCRSCount(1);
                    }
                    catch (IdsException iex)
                    {

                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                 
                }



                if(!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "PostBranchSalesEntry::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated ("+ EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
                #endregion

                #region Booking/Cancellation sale Not Posted
                //if (ds.Tables[1].Rows.Count > 0)
                //{
                //    if (Convert.ToInt32(ds.Tables[1].Rows[0]["Count"]) > 0)
                //    {
                //        string VoucherIds = ds.Tables[1].Rows[0]["voucherjournalid"].ToString();
                //        string ErrMsg = ds.Tables[1].Rows[0]["ErrMsg"].ToString();
                //        Logger.WriteLog(ErrMsg + VoucherIds);
                //        Email.SendMail(ErrMsg + VoucherIds);

                //    }

                //}

                #endregion
            }
        }



        public void UpdateBranchSalesEntry()
        {
            DataSet ds = null;
            List<SalesReceiptDetails> SaleReceiptDetailList = null;
            try
            {
                SaleReceiptDetailList = GetUpdateBranchData();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetBranchDataupdate", "", ex.Message, true);
            }

            if (SaleReceiptDetailList != null && SaleReceiptDetailList.Count > 0)
            {
                #region Booking Sale Posting
                EntryCounter.GetInstance().ResetAllCount();

                foreach (SalesReceiptDetails srdetails in SaleReceiptDetailList)
                {

                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    SalesReceipt Salesdata = new SalesReceipt();
                    try
                    {
                        SalesReceipt SR = null;
                        string docnumber = "";
                        string docformattednumber = "";
                        Salesdata = GetSalesReceipt(srdetails.Accsysid);

                        if (Salesdata != null)
                        {
                            SR = PostSaleEntry("Update", srdetails.BranchID, Salesdata.Id,Salesdata.SyncToken, srdetails.BranchName, srdetails.QuickBookCustomerID, srdetails.FromCityName, srdetails.ToCityName, srdetails.RouteFromCityName, srdetails.RouteToCityName, srdetails.BusNumber, srdetails.BranchPhone1, srdetails.BranchPhone2, srdetails.Amount, srdetails.PNR, srdetails.PassengerName, srdetails.SeatNos, srdetails.SeatCount, srdetails.FromTo, srdetails.JDate, srdetails.JTime, srdetails.BDate, srdetails.BookingID, srdetails.SalesReceiptDate, srdetails.TransactionID, srdetails.BusType, "", srdetails.ItemName, srdetails.ItemID, srdetails.ClassName, srdetails.ClassID, srdetails.BranchDivisionName, srdetails.BranchDivisionID, docformattednumber, srdetails.BookingStatus, srdetails.Prefix, srdetails.TotalFare, srdetails.RefundAmount, srdetails.PickUpName, srdetails.DropOffName, srdetails.DepositLedgerId, srdetails.DepositLedgerName, srdetails.BaseFare, srdetails.GST, srdetails.GSTType, srdetails.PlaceOfSupply, srdetails.BranchCommission);
                            ivID = Convert.ToInt32(SR.Id);
                            status = "Posted";
                            statusMessage = "";

                            EntryCounter.GetInstance().IncreaseQBCount(1);

                            InsertUpdateBookingSalesPostingDetailsToCRS(srdetails.BranchID, srdetails.CompanyID, srdetails.BookingID, ivID, docnumber, docformattednumber, srdetails.BusMasterID, srdetails.DivisionID, srdetails.Amount, srdetails.SalesReceiptDate, srdetails.DepositLedgerId);

                            EntryCounter.GetInstance().IncreaseCRSCount(1);

                        }
                     

                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("GetBranchDataupdate", "", ex.Message, true);
                    }

                }



                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "GetBranchDataupdate::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
                #endregion

            }
        }



        public void PostBranchRefundReceiptEntry()
        {
            DataSet ds = null;
            List<SalesReceiptDetails> refundreceiptdeatils = null;
            try
            {
                refundreceiptdeatils = GetBranchCancellation();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetBranchData", "", ex.Message, true);
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                #region Branch Cancellation Refund Posting
                EntryCounter.GetInstance().ResetAllCount();



                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        decimal amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["DebitAmount"].ToString());
                        int creditledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["CreditId"].ToString());
                        string creditledgername = ds.Tables[0].Rows[i]["CreditName"].ToString();
                        int debitledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["DebitId"].ToString());
                        string debitledgername = ds.Tables[0].Rows[i]["DebitName"].ToString();
                        int branchdivisionid = Convert.ToInt32(ds.Tables[0].Rows[i]["DivisionIdJE1"].ToString());
                        string branchdivisionname = ds.Tables[0].Rows[i]["DivisionNameJE1"].ToString();
                        int vouchersaleid = Convert.ToInt32(ds.Tables[0].Rows[i]["voucherjournalid"]);
                        string docnumber = ds.Tables[0].Rows[i]["docformattednumber"].ToString();
                        string memo = ds.Tables[0].Rows[i]["Desc"].ToString();
                        string saledatetime = ds.Tables[0].Rows[i]["journaldatetime"].ToString();



                       // SalesReceipt JK = PostSaleEntry("Insert", "", "", saledatetime, "", 0, amount, memo, creditledgername, creditledgerid, debitledgername, debitledgerid, memo, branchdivisionid, branchdivisionname, docnumber, "0", "");

                        EntryCounter.GetInstance().IncreaseQBCount(1);

                        UpdateJournalEntryPostingToCRS(vouchersaleid, Convert.ToInt32(0));

                        EntryCounter.GetInstance().IncreaseCRSCount(1);
                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("PostBranchSalesEntry", "", ex.Message, true);
                    }
                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "PostBranchSalesEntry::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
                #endregion

                #region Booking/Cancellation sale Not Posted
                if (ds.Tables[1].Rows.Count > 0)
                {
                    if (Convert.ToInt32(ds.Tables[1].Rows[0]["Count"]) > 0)
                    {
                        string VoucherIds = ds.Tables[1].Rows[0]["voucherjournalid"].ToString();
                        string ErrMsg = ds.Tables[1].Rows[0]["ErrMsg"].ToString();
                        Logger.WriteLog(ErrMsg + VoucherIds);
                        Email.SendMail(ErrMsg + VoucherIds);

                    }

                }

                #endregion
            }
        }

        public void PostBusNumberForBranchBookingJournalEntry()
        {
            DataSet ds = null;
            try
            {
                ds = GetBranchBookingWithBusNumber();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetBranchBookingWithBusNumber", "", ex.Message, true);
            }

            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        try
                        {
                            if (ds.Tables[0].Rows[i]["IsMerge"].ToString() == "1")
                            {
                                Logger.WriteLog("GetBranchBookingWithBusNumber::Validation::Invalid ClassId for BusNo: " + (ds.Tables[0].Rows[i]["busmasterid"].ToString()));
                            }
                            else
                            {
                                decimal amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["DebitAmount"].ToString());
                                int creditledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["CreditId"].ToString());
                                string creditledgername = ds.Tables[0].Rows[i]["CreditName"].ToString();
                                int debitledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["DebitId"].ToString());
                                string debitledgername = ds.Tables[0].Rows[i]["DebitName"].ToString();
                                int branchdivisionid = Convert.ToInt32(ds.Tables[0].Rows[i]["DivisionIdJE1"].ToString());
                                string branchdivisionname = ds.Tables[0].Rows[i]["DivisionNameJE1"].ToString();
                                int voucherjournalid = Convert.ToInt32(ds.Tables[0].Rows[i]["voucherjournalid"]);
                                string docnumber = ds.Tables[0].Rows[i]["docformattednumber"].ToString();
                                string memo = ds.Tables[0].Rows[i]["Desc"].ToString();
                                int classId = Convert.ToInt32(ds.Tables[0].Rows[i]["ClassId"].ToString());
                                string className = ds.Tables[0].Rows[i]["ClassName"].ToString();
                                string classIdQB = ds.Tables[0].Rows[i]["ClassIdQB"].ToString();
                                string accsysid = ds.Tables[0].Rows[i]["AccSysId"].ToString();
                                string journaldatetime = ds.Tables[0].Rows[i]["JournalDateTime"].ToString();

                                //decimal ToutAmount = Convert.ToDecimal(ds.Tables[0].Rows[i]["ToutFare"].ToString());
                                //int Toutledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["ToutId"].ToString());
                                //string Toutledgername = ds.Tables[0].Rows[i]["ToutName"].ToString();

                                JournalEntry je = GetJournalEntryFromQuickBook(accsysid);

                                //JournalEntry JK = PostSaleEntry("Update", je.Id, je.SyncToken, journaldatetime, "", 0,amount, memo, creditledgername, creditledgerid, debitledgername, debitledgerid, memo, branchdivisionid, branchdivisionname, docnumber, classIdQB, className);

                                UpdateBusNumberPostingToCRS(voucherjournalid, classId);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("PostBusNumberForBranchBookingJournalEntry", "", ex.Message, true);
                        }
                    }
                }
                
            }

        }
        #endregion

        #region Journal Entry (Receipt Voucher)
        public void PostJournalEntryData()
        {
            DataSet ds = null;

            try
            {
                ds = GetJournalEntryDataFromCRS();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("PostJournalEntryData", "", ex.Message, true);
            }

            if (ds != null && ds.Tables.Count > 0)
            {
              #region Regular Journal Posting
                EntryCounter.GetInstance().ResetAllCount();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {

                        int voucherjournalid = Convert.ToInt32(ds.Tables[0].Rows[i]["voucherjournalid"]);

                        //int creditledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["CreditAccountId"].ToString());
                        //string creditledgername = ds.Tables[0].Rows[i]["CreditAccountName"].ToString();
                        //decimal creditamount = Convert.ToDecimal(ds.Tables[0].Rows[i]["CreditAmount"].ToString());
                        //int creditdivisionid = Convert.ToInt32(ds.Tables[0].Rows[i]["CreditDivisionId"].ToString());
                        //string creditdivisionname = ds.Tables[0].Rows[i]["CreditDivisionName"].ToString();
                        //string creditclassid = ds.Tables[0].Rows[i]["CreditClassId"].ToString();
                        //string creditclassname = ds.Tables[0].Rows[i]["CreditClassName"].ToString();

                        //int debitledgerid = Convert.ToInt32(ds.Tables[0].Rows[i]["DebitAccountId"].ToString());
                        //string debitledgername = ds.Tables[0].Rows[i]["DebitAccountName"].ToString();
                        //decimal debitamount = Convert.ToDecimal(ds.Tables[0].Rows[i]["DebitAmount"].ToString());
                        //int debitdivisionid = Convert.ToInt32(ds.Tables[0].Rows[i]["DebitDivisionId"].ToString());
                        //string debitdivisionname = ds.Tables[0].Rows[i]["DebitDivisionName"].ToString();
                        //string debitclassid = ds.Tables[0].Rows[i]["DebitClassId"].ToString();
                        //string debitclassname = ds.Tables[0].Rows[i]["DebitClassName"].ToString();

                        string docnumber = ds.Tables[0].Rows[i]["docformattednumber"].ToString();
                        string narration = ds.Tables[0].Rows[i]["narration"].ToString();

                        string action = ds.Tables[0].Rows[i]["Action"].ToString();
                        string accsysid = ds.Tables[0].Rows[i]["AccSysId"].ToString();
                        DateTime journaldatetime = Convert.ToDateTime(ds.Tables[0].Rows[i]["journaldatetime"].ToString());

                        if (action.Equals("Insert") || action.Equals("Update"))
                        {
                            DataSet dsJEDetailData = GetJournalEntryDetailDataFromCRS(voucherjournalid);

                            if (dsJEDetailData != null && dsJEDetailData.Tables != null && dsJEDetailData.Tables.Count > 0)
                            {
                                if (dsJEDetailData.Tables[0] != null && dsJEDetailData.Tables[0].Rows != null && dsJEDetailData.Tables[0].Rows.Count > 0)
                                {
                                    List<Entity.SaleEntryDetail> jeDetailList = new List<Entity.SaleEntryDetail>();
                                    foreach (DataRow dr in dsJEDetailData.Tables[0].Rows)
                                    {
                                        Entity.SaleEntryDetail jeDetail = new Entity.SaleEntryDetail();
                                        {
                                            jeDetail.AccSysId = Convert.ToInt32(dr["accsysid"].ToString());
                                            jeDetail.LedgerName = dr["ledgername"].ToString();
                                            jeDetail.IsDebit = Convert.ToInt32(dr["isDebit"].ToString());
                                            jeDetail.Amount = Convert.ToDecimal(dr["amount"].ToString());
                                            jeDetail.Description = dr["description"].ToString();
                                            jeDetail.AccSysDivisionId = Convert.ToInt32(dr["accsysdivisionid"].ToString());
                                            jeDetail.DivisionName = dr["divisionname"].ToString();
                                            jeDetail.ClassId = Convert.ToInt32(dr["ClassId"].ToString());
                                            jeDetail.ClassName = dr["ClassName"].ToString();
                                            jeDetail.ClassIdQB = dr["ClassIdQB"].ToString();
                                            jeDetail.Payeeid = dr["Payeeid"].ToString();
                                            jeDetail.PayeeName = dr["Payeename"].ToString();
                                            jeDetail.PayeeType = dr["Payeetype"].ToString();
                                        }


                                        jeDetailList.Add(jeDetail);
                                    }

                                    JournalEntry journalEntryPosted;
                                    if (action.Equals("Insert"))
                                    {
                                        journalEntryPosted = PostJournalEntryDataToQB(action, "", "", narration, docnumber, journaldatetime, jeDetailList);
                                    }
                                    else
                                    {
                                        JournalEntry journalEntry = GetJournalEntryFromQuickBook(accsysid);

                                        journalEntryPosted = PostJournalEntryDataToQB(action, journalEntry.Id, journalEntry.SyncToken, narration, docnumber, journaldatetime, jeDetailList);
                                    }

                                    EntryCounter.GetInstance().IncreaseQBCount(1);

                                    UpdateJournalEntryPostingToCRS(voucherjournalid, Convert.ToInt32(journalEntryPosted.Id));

                                    EntryCounter.GetInstance().IncreaseCRSCount(1);
                                }
                            }


                        }
                        //else if(action.Equals("Update"))
                        //{
                        //    try
                        //    {

                        //        JournalEntry journalEntry = GetJournalEntryFromQuickBook(accsysid);

                        //        JournalEntry journalEntryPosted = PostJournalEntryDataToQB(action, journalEntry.Id, journalEntry.SyncToken, narration, docnumber, creditledgerid, creditledgername, creditamount, creditdivisionid, creditdivisionname, creditclassid, creditclassname,
                        //    debitledgerid, debitledgername, debitamount, debitdivisionid, debitdivisionname, debitclassid, debitclassname);

                        //        UpdateJournalEntryPostingToCRS(voucherjournalid, Convert.ToInt32(journalEntryPosted.Id));

                        //        Logger.WriteLog("UpdateJournalEntry", accsysid, "Success", true);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //       Logger.WriteLog("UpdateJournalEntry", accsysid, "Failed" + ":" + "Update- " + ex.Message + " : " + ex.Message, true);
                        //    }
                        //}
                        else if (action.Equals("Delete"))
                        {
                            try
                            {

                                if (!accsysid.Equals("") && !accsysid.Equals("0"))
                                {
                                    JournalEntry journalEntry = GetJournalEntryFromQuickBook(accsysid);

                                    DeleteJournalEntryFromQuickBook(journalEntry);

                                    EntryCounter.GetInstance().IncreaseQBCount(1);

                                    UpdateJournalEntryPostingToCRS(voucherjournalid, Convert.ToInt32(journalEntry.Id));

                                    EntryCounter.GetInstance().IncreaseCRSCount(1);

                                    Logger.WriteLog("DeleteJournalEntry", accsysid, "Success", true);
                                }


                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog("DeleteJournalEntry", accsysid, "Failed" + ":" + "Delete- " + ex.Message + " : " + ex.Message, true);
                            }
                        }


                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("PostJournalEntryData", "", ex.Message, true);
                    }
                }
                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "PostJournalEntryData::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }
                EntryCounter.GetInstance().ResetAllCount();
                #endregion

                #region Regular journal not posted

                if (ds.Tables[1].Rows.Count > 0)
                {
                    if (Convert.ToInt32(ds.Tables[1].Rows[0]["Count"]) > 0)
                    {
                        string VoucherIds = ds.Tables[1].Rows[0]["voucherjournalid"].ToString();
                        string ErrMsg = ds.Tables[1].Rows[0]["ErrMsg"].ToString();
                        Logger.WriteLog(ErrMsg + VoucherIds);
                        Email.SendMail(ErrMsg + VoucherIds);

                    }

                }
                #endregion
            }
        }

        private JournalEntry PostJournalEntryDataToQB(string action,string jeId, string synctoken,string narration, string docnumber,DateTime journaldatetime, List<Entity.SaleEntryDetail> jeDetailList)
        {
            try
            {

                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                /* Sample Journal Entry - Begin */

                JournalEntry je = new JournalEntry();
                List<Line> lines = new List<Line>();

               
                if(jeDetailList != null && jeDetailList.Count > 0)
                {
                    foreach(Entity.SaleEntryDetail jeDetail in jeDetailList)
                    {
                        Line line = new Line();
                        {
                            line.Amount = jeDetail.Amount;
                            line.AmountSpecified = true;
                            //creditLine.Description =  " - (Entry using API)";
                            //line.Description = "[Voucher No : " + docnumber + "], " + narration;
                            line.Description = jeDetail.Description;
                            line.DetailType = LineDetailTypeEnum.JournalEntryLineDetail;

                            line.DetailTypeSpecified = true;
                        }
                        JournalEntryLineDetail lineDetail = new JournalEntryLineDetail();
                        if (jeDetail.IsDebit == 1)
                        {
                            lineDetail.PostingType = PostingTypeEnum.Debit;
                        }
                        else
                        {
                            lineDetail.PostingType = PostingTypeEnum.Credit;
                        }

                        lineDetail.PostingTypeSpecified = true;
                        lineDetail.AccountRef = new ReferenceType() { name = jeDetail.LedgerName, Value = jeDetail.AccSysId.ToString() }; // , type = "Income" 
                        lineDetail.DepartmentRef = new ReferenceType() { name = jeDetail.DivisionName, Value = jeDetail.AccSysDivisionId.ToString() };

                        if (jeDetail.PayeeType == "Customer")
                        {
                            lineDetail.Entity = new EntityTypeRef();
                            {
                                lineDetail.Entity.Type = EntityTypeEnum.Customer;
                                lineDetail.Entity.TypeSpecified = true;

                                lineDetail.Entity.EntityRef = new ReferenceType() { name = jeDetail.PayeeName, Value = jeDetail.Payeeid };
                            }

                        }
                        else if (jeDetail.PayeeType == "Vendor") {
                            lineDetail.Entity = new EntityTypeRef();

                            {
                                lineDetail.Entity.Type = EntityTypeEnum.Vendor;
                                lineDetail.Entity.TypeSpecified = true;

                                lineDetail.Entity.EntityRef = new ReferenceType() { name = jeDetail.PayeeName, Value = jeDetail.Payeeid };
                            }
                        }
                        if (!jeDetail.ClassIdQB.Equals("0"))
                        {
                            lineDetail.ClassRef = new ReferenceType() { name = jeDetail.ClassName, Value = jeDetail.ClassIdQB };
                        }
                        line.AnyIntuitObject = lineDetail;

                        lines.Add(line);
                    }
                }

                


                //Line debitLine = new Line();
                //debitLine.Amount = debitAmount;
                //debitLine.AmountSpecified = true;
                ////debitLine.Description = " - (Entry using API)";
                //debitLine.DetailType = LineDetailTypeEnum.JournalEntryLineDetail;
                //debitLine.DetailTypeSpecified = true;

                //JournalEntryLineDetail debitLineDetail = new JournalEntryLineDetail();
                //debitLineDetail.PostingType = PostingTypeEnum.Debit;
                //debitLineDetail.PostingTypeSpecified = true;
                //debitLineDetail.AccountRef = new ReferenceType() { name = debitLedgerName, Value = debitLedgerID.ToString() }; // , type = "Accounts Receivable"
                //debitLineDetail.DepartmentRef = new ReferenceType() { name = debitDivisionName, Value = debitDivisionId.ToString() };
                //if (!debitClassId.Equals("0"))
                //{
                //    debitLineDetail.ClassRef = new ReferenceType() { name = debitClassName, Value = debitClassId };
                //}
                //debitLine.AnyIntuitObject = debitLineDetail;
                               
                //lines.Add(debitLine);

                je.Line = lines.ToArray();
                je.PrivateNote = "[Voucher No : " + docnumber + "], " + narration;
                je.TxnDate = journaldatetime;
                je.TxnDateSpecified = true;
                // as per request from candida ma'm, passing voucher no in DocNumber
                je.DocNumber = docnumber;

                if (action.Equals("Update"))
                {
                    je.Id = jeId;
                    je.SyncToken = synctoken;
                }

                JournalEntry jePosted = service.Add(je);

                return jePosted;
                /* Sample Journal Entry - End */
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        private JournalEntry GetJournalEntryFromQuickBook(string accsysid)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<JournalEntry> queryServiceForJE = new Intuit.Ipp.QueryFilter.QueryService<JournalEntry>(context);
                List<JournalEntry> jeList = queryServiceForJE.ExecuteIdsQuery("select * from journalentry where id = '" + accsysid + "'").ToList<JournalEntry>();
                JournalEntry je = jeList[0];
                return je;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        private void DeleteJournalEntryFromQuickBook(JournalEntry je)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                service.Delete(je);
                
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }
        private DataSet GetJournalEntryDataFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetJournalEntryDataForQuickBook", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false,"",false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private DataSet GetJournalEntryDetailDataFromCRS(int journalEntryId)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_JournalEntryID", journalEntryId, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetJournalEntryDataDetailForQuickBook", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Expense / Contra QuickBook Call

        private string CreateExpenseNoForQB(Int32 CompanyID, Int32 DivisionID, Int32 VoucherPaymentID)
        {

            try
            {
                string strErr = "";
                string strVoucherNo = "";
                DataSet ds = null;
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_DivisionID", DivisionID, ParameterDirection.Input);
                dal.AddParameter("p_VoucherPaymentID", VoucherPaymentID, ParameterDirection.Input);

                ds = dal.ExecuteSelect("spCreateExpenseNoforQB", CommandType.StoredProcedure, 0, ref strErr);

                if (ds != null && ds.Tables.Count >= 1)
                {
                    strVoucherNo = ds.Tables[0].Rows[0]["VoucherNoQB"].ToString();
                }
                return strVoucherNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Purchase PostPurchase(string action,string purchaseid,string synctoken,int voucherpaymentid, int fromaccountid, string fromaccount, int paymentmethodid, string toaccounttype, string refno, string txndate, int divisionid, string divisionname, int Payeeid, string Payeename, decimal totalamount,string VoucherNoQB,string narration)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Purchase ipurchase = new Purchase();

                {
                    ipurchase.AccountRef = new ReferenceType() { name = fromaccount, Value = fromaccountid.ToString() };

                    if (paymentmethodid == 1 || paymentmethodid == 5)
                    {
                        paymentmethodid = 8;
                        ipurchase.PaymentMethodRef = new ReferenceType() { name = "Cash", Value = "8" };
                    }
                    else if (paymentmethodid == 2 || paymentmethodid == 6)
                    {
                        paymentmethodid = 9;
                        ipurchase.PaymentMethodRef = new ReferenceType() { name = "Cheque", Value = "9" };
                    }
                    else if (paymentmethodid == 3)
                    {
                        paymentmethodid = 11;
                        ipurchase.PaymentMethodRef = new ReferenceType() { name = "Net Banking", Value = "11" };
                    }
                    else
                    {
                        paymentmethodid = 10;
                        ipurchase.PaymentMethodRef = new ReferenceType() { name = "Card", Value = "10" };
                    }

                    //ipurchase.PaymentMethodRef = new ReferenceType() { Value = paymentmethodid.ToString() };

                    //ipurchase.PaymentType = PaymentTypeEnum.Cash;
                    ipurchase.PaymentTypeSpecified = true;

                    if (Payeename != "")
                        ipurchase.EntityRef = new ReferenceType() { name = Payeename, Value = Payeeid.ToString(), type = toaccounttype };

                    ipurchase.TotalAmt = totalamount;

                    ipurchase.DocNumber = refno;

                    ipurchase.TxnDate = Convert.ToDateTime(txndate);
                    ipurchase.TxnDateSpecified = true;

                    ipurchase.DepartmentRef = new ReferenceType() { name = divisionname, Value = divisionid.ToString() };

                    DataSet dsVoucherDetails = GetVoucherPaymentDetails(voucherpaymentid);

                    List<Line> purchaseLineList = new List<Line>();

                    for (int i = 0; i < dsVoucherDetails.Tables[0].Rows.Count; i++)
                    {
                        Line line = new Line();

                        {
                            line.Description = dsVoucherDetails.Tables[0].Rows[i]["description"].ToString();
                            line.Amount = Convert.ToDecimal(dsVoucherDetails.Tables[0].Rows[i]["Amount"].ToString());
                            line.AmountSpecified = true;

                            line.DetailType = LineDetailTypeEnum.AccountBasedExpenseLineDetail;
                            line.DetailTypeSpecified = true;

                            AccountBasedExpenseLineDetail AccExpenseLine = new AccountBasedExpenseLineDetail();

                            if (dsVoucherDetails.Tables[0].Rows[i]["classname"].ToString() != "")
                                AccExpenseLine.ClassRef = new ReferenceType() { name = dsVoucherDetails.Tables[0].Rows[i]["classname"].ToString(), Value = dsVoucherDetails.Tables[0].Rows[i]["classid"].ToString() };

                            AccExpenseLine.AccountRef = new ReferenceType() { name = dsVoucherDetails.Tables[0].Rows[i]["ToAccount"].ToString(), Value = dsVoucherDetails.Tables[0].Rows[i]["ToAccountId"].ToString() };

                            AccExpenseLine.BillableStatus = BillableStatusEnum.NotBillable;

                            AccExpenseLine.TaxCodeRef = new ReferenceType() { Value = "NON" };

                            line.AnyIntuitObject = AccExpenseLine;
                        }

                        purchaseLineList.Add(line);

                    }

                    ipurchase.Line = purchaseLineList.ToArray();
                    ipurchase.PrivateNote = "[Voucher No: " + VoucherNoQB + "] " + narration;

                    if (action.Equals("Update"))
                    {
                        ipurchase.Id = purchaseid;
                        ipurchase.SyncToken = synctoken;
                    }
                }
                Purchase postedPurchase = service.Add(ipurchase);

                return postedPurchase;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

      
        public Purchase DeletePurchase(string purchaseid, string SyncToken)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Purchase ipurchase = new Purchase();

                {
                    ipurchase.Id = purchaseid;
                    ipurchase.SyncToken = SyncToken;
                }

                Purchase postedPurchase = service.Delete(ipurchase);

                return postedPurchase;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }


        public JournalEntry DeleteJournal(string journalid, string SyncToken)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                JournalEntry ijournal = new JournalEntry();

                {
                    ijournal.Id = journalid;
                    ijournal.SyncToken = SyncToken;
                }

                JournalEntry postedJournal = service.Delete(ijournal);

                return postedJournal;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }


        public Payment DeletePayment(string Paymentid, string SyncToken)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Payment iPayment = new Payment();

                {
                    iPayment.Id = Paymentid;
                    iPayment.SyncToken = SyncToken;
                }

                Payment postedPayment = service.Delete(iPayment);

                return postedPayment;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }


        private DataSet GetVoucherPaymentDetails(int intVoucherPaymentID)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_voucherpaymentid", intVoucherPaymentID, ParameterDirection.Input);
                DataSet dstOutPut = dal.ExecuteSelect("spGetVoucherPaymentDetails", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false,"",false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Purchase GetPurchase(string PurchaseID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Purchase> queryServiceForPurchase = new Intuit.Ipp.QueryFilter.QueryService<Purchase>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Purchase> PurchaseList = queryServiceForPurchase.ExecuteIdsQuery("select * from purchase  where id = '" + PurchaseID + "'");
                Purchase iPurchase = PurchaseList.Take(1).FirstOrDefault();
                return iPurchase;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }


        public static JournalEntry GetJournal(string JournalID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<JournalEntry> queryServiceForJournalEntry = new Intuit.Ipp.QueryFilter.QueryService<JournalEntry>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<JournalEntry> JournalList = queryServiceForJournalEntry.ExecuteIdsQuery("select * from JournalEntry   where id = '" + JournalID + "'");
                JournalEntry iJournal = JournalList.Take(1).FirstOrDefault();
                return iJournal;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }


        public static Payment GetPayment(string PaymentID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Payment> queryServiceForJournalEntry = new Intuit.Ipp.QueryFilter.QueryService<Payment>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Payment> PaymentList = queryServiceForJournalEntry.ExecuteIdsQuery("select * from payment   where id = '" + PaymentID + "'");
                Payment iPayment = PaymentList.Take(1).FirstOrDefault();
                return iPayment;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }


        public static List<Payment> GetAllPayments(int startPosition)
        {
            List<Payment> PaymentList = null;
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);
                Intuit.Ipp.QueryFilter.QueryService<Payment> queryServiceForAccount = new Intuit.Ipp.QueryFilter.QueryService<Payment>(context);
                PaymentList = queryServiceForAccount.ExecuteIdsQuery("select * from payment where TxnDate >= '2017-10-01' and  TxnDate <= '2017-10-31' STARTPOSITION " + startPosition.ToString() + "MAXRESULTS 1000").ToList<Payment>();

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                string msg = "Exception in GetBankLedgers: " + ex.Message;
               
            }
            return PaymentList;
        }


        public static List<Item> GetAllitems(int startPosition)
        {
            List<Item> itemList = null;
            try
            {
                 ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);
                Intuit.Ipp.QueryFilter.QueryService<Item> queryServiceForAccount = new Intuit.Ipp.QueryFilter.QueryService<Item>(context);
                itemList = queryServiceForAccount.ExecuteIdsQuery("select * from item STARTPOSITION " + startPosition.ToString() + "MAXRESULTS 500").ToList<Item>();

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                string msg = "Exception in GetBankLedgers: " + ex.Message;

            }
            return itemList;
        }


        public static Account Getledgers(string PaymentID)
        {
            try
            {
                Logger.WriteLog("Entry1");
                //ServiceContext context = QuickBookConnection.GetDataServiceContext();
                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Account> queryServiceForJournalEntry = new Intuit.Ipp.QueryFilter.QueryService<Account>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Account> PaymentList = queryServiceForJournalEntry.ExecuteIdsQuery("select * from account");
                Logger.WriteLog("First");
                Account iPayment = PaymentList.Take(1).FirstOrDefault();
                return iPayment;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                Logger.WriteLog("First" + ex);
                throw ex;
            }

        }

        public static Invoice Getinvoice(string InvoiceID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Invoice> queryServiceForPurchase = new Intuit.Ipp.QueryFilter.QueryService<Invoice>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Invoice> InvoiceList = queryServiceForPurchase.ExecuteIdsQuery("select * from Invoice  where id = '" + InvoiceID + "'");
                Invoice iv = InvoiceList.Take(1).FirstOrDefault();
                    //Purchase iPurchase = PurchaseList.Take(1).FirstOrDefault();
                return iv;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }

        private string CreateContraNoForQB(Int32 CompanyID, Int32 DivisionID, Int32 VoucherPaymentID)
        {

            try
            {
                string strErr = "";
                string strVoucherNo = "";
                DataSet ds = null;
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_DivisionID", DivisionID, ParameterDirection.Input);
                dal.AddParameter("p_VoucherPaymentID", VoucherPaymentID, ParameterDirection.Input);

                ds = dal.ExecuteSelect("spCreateContraNoforQB", CommandType.StoredProcedure, 0, ref strErr);

                if (ds != null && ds.Tables.Count >= 1)
                {
                    strVoucherNo = ds.Tables[0].Rows[0]["VoucherNoQB"].ToString();
                }
                return strVoucherNo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Transfer PostTransfer(string action,string transferid,string synctoken,int voucherpaymentid, int fromaccountid, string fromaccount, string txndate, decimal totalamount, string desc,string VoucherNoQB)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Transfer itransfer = new Transfer();

                {
                    itransfer.FromAccountRef = new ReferenceType() { name = fromaccount, Value = fromaccountid.ToString() };

                    DataSet dsVoucherDetails = GetVoucherPaymentDetails(voucherpaymentid);

                    if (dsVoucherDetails.Tables[0].Rows.Count > 0)
                    {
                        itransfer.ToAccountRef = new ReferenceType() { name = dsVoucherDetails.Tables[0].Rows[0]["ToAccount"].ToString(), Value = dsVoucherDetails.Tables[0].Rows[0]["ToAccountId"].ToString() };
                    }

                    itransfer.Amount = totalamount;
                    itransfer.AmountSpecified = true;

                    itransfer.TxnDate = Convert.ToDateTime(txndate);
                    itransfer.TxnDateSpecified = true;

                    itransfer.PrivateNote = "[Voucher No: " + VoucherNoQB + "]," + desc;
                    //itransfer.PrivateNote = desc;

                    if (action.Equals("Update"))
                    {
                        itransfer.Id = transferid;
                        itransfer.SyncToken = synctoken;
                    }
                }

                Transfer postedTransfer = service.Add(itransfer);

                return postedTransfer;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        

        public Transfer DeleteTransfer(string transferid, string SyncToken)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Transfer itransfer = new Transfer();

                {
                    itransfer.Id = transferid;
                    itransfer.SyncToken = SyncToken;
                }

                Transfer postedTransfer = service.Delete(itransfer);

                return postedTransfer;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        public static Transfer GetTransfer(string TransferID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Transfer> queryServiceForTransfer = new Intuit.Ipp.QueryFilter.QueryService<Transfer>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Transfer> TransferList = queryServiceForTransfer.ExecuteIdsQuery("select * from transfer where id = '" + TransferID + "'");
                Transfer iTransfer = TransferList.Take(1).FirstOrDefault();
                return iTransfer;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }

        public void UpdateExpenseContraAccSysId(int VoucherPaymentId, int AccSysID)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_VoucherPaymentID", VoucherPaymentId);
                dal.AddParameter("p_AccSysID", AccSysID);

                dal.ExecuteDML("spUpdateQuickBookExpenseContraStatus", CommandType.StoredProcedure, 0, ref strErr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Trip Master QuickBooks Call

        public Item PostItem(string ItemName,bool isAC)
        {
            try
            {
                //Console.Write("   Before QB Connection");
                //Thread.Sleep(3000);

                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                Item item = new Item();


                //item.SubItem = true;
                //item.SubItemSpecified = true;
                {
                    item.TypeSpecified = true;
                    item.Type = ItemTypeEnum.Service;
                    //item.ParentRef = new ReferenceType() { name = "Tickets", Value = "18", type = "Category" };
                    //item.ParentRef = new ReferenceType() { name = "Tickets", Value = "1239", type = "Category" };
                    //item.IncomeAccountRef = new ReferenceType() { name = "Services", Value = "126" };// { name = "Sale Of Tickets", Value = "83" };
                    if (isAC)
                    {
                        item.Name = ItemName;
                        item.IncomeAccountRef = new ReferenceType() { name = "Services", Value = "3" };
                    }
                    else
                    {
                        item.Name = ItemName;
                        item.IncomeAccountRef = new ReferenceType() { name = "Services", Value = "3" };
                    }
                }
                //Console.Write("Trip Before Post");
                //Thread.Sleep(3000);

                Item postedItem = service.Add(item);

                //Console.Write("Trip After Post");
                //Thread.Sleep(3000);

                return postedItem;

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                Thread.Sleep(3000);
                Console.Write("\n Exception  ----" + ex.Message + " -- " + ex.StackTrace);
                Thread.Sleep(3000);
                throw ex;
            }
        }

        public static Item GetItem(string ItemID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Item> queryServiceForItem = new Intuit.Ipp.QueryFilter.QueryService<Item>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Item> ItemList = queryServiceForItem.ExecuteIdsQuery("select * from item where id = '" + ItemID + "'");
                Item item = ItemList.Take(1).FirstOrDefault();
                return item;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }

        public Item UpdateItem(string itemID, string ItemName, string SyncToken,bool isAC)
        {
            try
            {

                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Item item = new Item();

                {
                    item.Id = itemID;
                    item.SyncToken = SyncToken;

                    //item.SubItem = true;
                    //item.SubItemSpecified = true;
                    item.TypeSpecified = true;
                    item.Type = ItemTypeEnum.Service;
                    //item.ParentRef = new ReferenceType() { name = "Tickets", Value = "18", type = "Category" };
                    //item.IncomeAccountRef = new ReferenceType() { name = "Services", Value = "126" };// { name = "Sale Of Tickets", Value = "83" };
                    if (isAC)
                    {
                        item.IncomeAccountRef = new ReferenceType() { name = "Sales of Tickets-AC Vehicles", Value = "2288" };
                        item.Name = ItemName + "_AC";
                    }
                    else
                    {
                        item.IncomeAccountRef = new ReferenceType() { name = "Sales of Tickets", Value = "2287" };
                        item.Name = ItemName;
                    }
                }
                Item updatedItem = service.Update(item);

                return updatedItem;

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        public void UpdateTripAccSysId(int TripID, string AccSysID,string AccSysIDAC)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_TripID", TripID);
                dal.AddParameter("p_AccSysID", AccSysID, 100);
                dal.AddParameter("p_AccSysIDAC", AccSysIDAC, 100);

                dal.ExecuteDML("spUpdateAccSysTrip", CommandType.StoredProcedure, 0, ref strErr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Bus Master QuickBooks Call

        public Class PostBus(string BusNumber)
        {
            try
            {

                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                Class c = new Class();

                {
                    c.Name = BusNumber;
                    c.SubClass = true;
                    c.SubClassSpecified = true;
                    c.ParentRef = new ReferenceType() { name = "CRSVehicles", Value = "1400000000001365986" };
                }

                Class postedClass = service.Add(c);

                return postedClass;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        public static Class GetBus(string ClassID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Class> queryServiceForItem = new Intuit.Ipp.QueryFilter.QueryService<Class>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Class> ClassList = queryServiceForItem.ExecuteIdsQuery("select * from class where id = '" + ClassID + "'");
                Class iClass = ClassList.Take(1).FirstOrDefault();
                return iClass;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw;
            }

        }

        public Class UpdateBus(string ClassID, string ClassName, string SyncToken)
        {
            try
            {

                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Class iClass = new Class();

                {
                    iClass.Id = ClassID;
                    iClass.Name = ClassName;
                    iClass.SyncToken = SyncToken;
                    iClass.SubClass = true;
                    iClass.SubClassSpecified = true;
                    iClass.ParentRef = new ReferenceType() { name = "Vehicles", Value = "3700000000000825679" };
                }

                Class updatedClass = service.Update(iClass);

                return updatedClass;

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        public void UpdateBusAccSysId(int BusMasterID, string AccSysID, string BusNumber)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_BusMasterID", BusMasterID);
                dal.AddParameter("p_AccSysID", AccSysID, 100);
                dal.AddParameter("p_ClassName", BusNumber, 200);
                dal.AddParameter("p_CompanyID", 69);

                dal.ExecuteDML("Test_utk_spUpdateAccSysBus", CommandType.StoredProcedure, 0, ref strErr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Agent Master QuickBooks Call

        public Customer PostAgent(string agentname, string companyname, string address, string city, string state, string country, string mobileno, string agentid)
        {
            try
            {

                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                Customer c = new Customer();

                string strAgentName = agentname;
                //agentname = agentid + "_" + agentname;

                if (agentname.Length > 24)
                    agentname = agentname.Substring(0, 24); // Max length 25

                c.GivenName = ""; // agentname;
                c.CompanyName = companyname;// strAgentName;
                strAgentName = strAgentName.Replace("\t", "");
                strAgentName = strAgentName.Replace("\n", "");
                strAgentName = strAgentName.Replace(":", "");
                c.DisplayName = strAgentName + "_" + agentid; // agentname;

                PhysicalAddress ph = new PhysicalAddress();
                {
                    ph.Line1 = address;
                    ph.City = city;
                    ph.CountrySubDivisionCode = state;
                    ph.Country = country;
                }

                c.BillAddr = ph;

                TelephoneNumber tn = new TelephoneNumber();
                { tn.FreeFormNumber = mobileno; }

                c.PrimaryPhone = tn;

                Customer postedCustomer = service.Add(c);

                return postedCustomer;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        public static Customer GetAgent(string CustomerID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Customer> queryServiceForItem = new Intuit.Ipp.QueryFilter.QueryService<Customer>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Customer> CustomerList = queryServiceForItem.ExecuteIdsQuery("select * from Customer where id = '" + CustomerID + "'");
                Customer iCustomer = CustomerList.Take(1).FirstOrDefault();
                return iCustomer;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw;
            }

        }

        public Customer UpdateAgent(string CustomerID, string agentname, string companyname, string address, string city, string state, string country, string mobileno, string SyncToken,string agentid)
        {
            try
            {

                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Customer iCustomer = new Customer();

                string strAgentName = agentname;
                //agentname = agentid + "_" + agentname;

                if (agentname.Length > 24)
                    agentname = agentname.Substring(0, 24); // Max length 25

                iCustomer.Id = CustomerID;
                iCustomer.GivenName = ""; // agentname; // Max length 25
                iCustomer.CompanyName = ""; // strAgentName;
                strAgentName = strAgentName.Replace("\t", "");
                strAgentName = strAgentName.Replace("\n", "");
                strAgentName = strAgentName.Replace(":", "");
                iCustomer.DisplayName = strAgentName + "_" + agentid;

                PhysicalAddress ph = new PhysicalAddress();
                {
                    ph.Line1 = address;
                    ph.City = city;
                    ph.CountrySubDivisionCode = state;
                    ph.Country = country;
                }

                iCustomer.BillAddr = ph;

                TelephoneNumber tn = new TelephoneNumber();
                { tn.FreeFormNumber = mobileno; }

                iCustomer.PrimaryPhone = tn;

                iCustomer.SyncToken = SyncToken;

                Customer updatedCustomer = service.Update(iCustomer);

                return updatedCustomer;

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        public void UpdateAgentAccSysId(int AgentID, int CompanyID, string AccSysID)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_AgentID", AgentID);
                dal.AddParameter("p_CompanyID", CompanyID);
                dal.AddParameter("p_AccSysID", AccSysID, 100);

                dal.ExecuteDML("spUpdateAccSysAgent", CommandType.StoredProcedure, 0, ref strErr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateFranchiseAccSysId(int FranchiseID, int CompanyID, string AccSysID)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_FranchiseID", FranchiseID);
                dal.AddParameter("p_CompanyID", CompanyID);
                dal.AddParameter("p_AccSysID", AccSysID, 100);

                dal.ExecuteDML("Test_utk_spUpdateAccSysFranchise", CommandType.StoredProcedure, 0, ref strErr);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        //private List<AgentVoucherDetails> ValidateAgentVoucher()
        //{
        //    return;
        //}


        public void DeleteJournalEntries()
        {
            DataSet ds = new DataSet();
            ds = GetBranchJournalEntryFromCRS();
            if (ds != null && ds.Tables.Count > 0)
            {

                string status = "";
                string statusMessage = "";
                Int32 iID = -1;
                foreach (DataRow drJournalDetails in ds.Tables[0].Rows)
                {
                    try
                    {
                        JournalEntry JournalData = new JournalEntry();
                        if (!drJournalDetails["AccSysId"].ToString().Equals(""))
                        {
                            JournalData = GetJournal(drJournalDetails["AccSysId"].ToString());

                            if (JournalData != null)
                            {
                                JournalEntry iJournal = DeleteJournal(drJournalDetails["AccSysId"].ToString(), JournalData.SyncToken);
                                iID = Convert.ToInt32(iJournal.Id);
                                status = "Posted";
                                statusMessage = "Deleted";
                                EntryCounter.GetInstance().IncreaseQBCount(1);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = "Delete- " + ex.Message + " : " + ex.Message;
                        Logger.WriteLog("JournalEntry", drJournalDetails["AccSysId"].ToString(), status + ":" + statusMessage, true);
                    }

                }


             }

          }


        public void DeleteAgentPaymentEntries()
        {
            DataSet ds = new DataSet();
            //ds = GetAgentPaymentEntryFromCRS();

           // List<Payment> paymentList = null;
            List<Item> itemList = null;


            //paymentList = GetAllPayments(2001);
            itemList = GetAllitems(1001);
            // InsertPaymentDetailToCRS(itemList);
            InsertitemDetailToCRS(itemList);
            if (ds != null && ds.Tables.Count > 0)
            {

                string status = "";
                string statusMessage = "";
                Int32 iID = -1;
                int count = 0;

               
                foreach (DataRow drPaymentDetails in ds.Tables[0].Rows)
                {
                    try
                    {
                        Payment PaymentData = new Payment();
                        if (!drPaymentDetails["AccSysId"].ToString().Equals(""))
                        {
                            PaymentData = GetPayment(drPaymentDetails["AccSysId"].ToString());

                            if (PaymentData != null)
                            {
                                count++;
                                PaymentData = DeletePayment(drPaymentDetails["AccSysId"].ToString(), PaymentData.SyncToken);
                               // iID = Convert.ToInt32(iPaymentl.Id);
                                status = "Posted";
                                //statusMessage = "Deleted";
                                //EntryCounter.GetInstance().IncreaseQBCount(1);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = "Delete- " + ex.Message + " : " + ex.Message;
                        Logger.WriteLog("JournalEntry", drPaymentDetails["AccSysId"].ToString(), status + ":" + statusMessage, true);
                    }

                }


            }

        }

        public static void InsertPaymentDetailToCRS(List<Payment> PaymentList)
        {
            try
            {
                CRSDAL dal = new CRSDAL();
                string strErr = "";
                //int Accsysid = 0;
                for (int i = 0; i < PaymentList.Count; i++)
                {
                    int Accsysid = Convert.ToInt32(PaymentList[i].Id);
                    int CustomerId = Convert.ToInt32(PaymentList[i].CustomerRef.Value);
                    //String docnumber = (PaymentList[i].PrivateNote).Substring(14,12);
                    try
                    {

                       
                        dal.AddParameter("p_AgentName", PaymentList[i].CustomerRef.name,100, ParameterDirection.Input);
                        dal.AddParameter("p_totalAmount", PaymentList[i].TotalAmt, ParameterDirection.Input);
                        dal.AddParameter("p_UnappliedAmt", PaymentList[i].UnappliedAmt, ParameterDirection.Input);
                        dal.AddParameter("p_TxnDate", PaymentList[i].TxnDate, ParameterDirection.Input);
                        dal.AddParameter("p_Accsysid", Accsysid, ParameterDirection.Input);
                        dal.AddParameter("p_PrivateNote", PaymentList[i].PrivateNote, 1000, ParameterDirection.Input);
                        dal.AddParameter("p_CreateTime", PaymentList[i].MetaData.CreateTime,  ParameterDirection.Input);
                        dal.AddParameter("p_LastUpdatedTime", PaymentList[i].MetaData.LastUpdatedTime,  ParameterDirection.Input);
                        dal.AddParameter("p_CustomerId", CustomerId, ParameterDirection.Input);
                        DataSet dsOutPut = dal.ExecuteSelect("spSet_QBPaymentDetails", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                        // int status = dal.ExecuteDML("spTallySet_UserwiseCollection_BranchTotal_V2", CommandType.StoredProcedure, 0, ref strErr);


                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("Test", PaymentList[i].Id,"", true);
                    }
                }
         }
            catch (Exception ex)
            {
                Logger.WriteLog("Test2", "", "", true);
            }
        }



        public static void InsertitemDetailToCRS(List<Item> ItemList)
        {
            try
            {
                CRSDAL dal = new CRSDAL();
                string strErr = "";
                //int Accsysid = 0;
                for (int i = 0; i < ItemList.Count; i++)
                {
                    string trip = ItemList[i].FullyQualifiedName;
                    bool containsInt = trip.Any(char.IsDigit);
                    if (containsInt)
                    {


                        int Accsysid = Convert.ToInt32(ItemList[i].Id);
                        int ledgerid = Convert.ToInt32(ItemList[i].IncomeAccountRef.Value);
                        //String docnumber = (PaymentList[i].PrivateNote).Substring(14,12);


                        string b = trip.Split('_')[1];
                        string c = b.Split(' ')[0];
                        int tripid = Convert.ToInt32(c);
                        try
                        {


                            dal.AddParameter("p_Accsysid", Accsysid, ParameterDirection.Input);
                            dal.AddParameter("p_TripId", tripid, ParameterDirection.Input);
                            dal.AddParameter("p_LedgerId", ledgerid, ParameterDirection.Input);
                            DataSet dsOutPut = dal.ExecuteSelect("spSet_QBItemDetails", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);



                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("Test", ItemList[i].Id, "", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Test2", "", "", true);
            }
        }

        public void CheckAccountLedgers()
        {
            DataSet ds = new DataSet();
            ds = GetAgentPaymentEntryFromCRS();
            if (ds != null && ds.Tables.Count > 0)
            {

                string status = "";
                string statusMessage = "";
                Int32 iID = -1;
                int count = 0;
                List<Account> ledgerList = null;
                string result = "";
                 string[] stringArray = new string[500];
                foreach (DataRow drPaymentDetails in ds.Tables[0].Rows)
                {
                    try
                    {
                        Account PaymentData = new Account();
                        if (!drPaymentDetails["AccSysId"].ToString().Equals(""))
                        {
                            PaymentData = Getledgers(drPaymentDetails["AccSysId"].ToString());

                            if (PaymentData != null)
                            {
                                count++;
                                 result += drPaymentDetails["AccSysId"].ToString() + ",";
                                stringArray[count] = (drPaymentDetails["AccSysId"].ToString());
                                //ledgerList.Add(drPaymentDetails["AccSysId"].ToString());
                                // Payment iPaymentl = DeletePayment(drPaymentDetails["AccSysId"].ToString(), PaymentData.SyncToken);
                                // iID = Convert.ToInt32(iPaymentl.Id);
                                

                                status = "Posted";
                                statusMessage = "Deleted";
                                EntryCounter.GetInstance().IncreaseQBCount(1);
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = "Delete- " + ex.Message + " : " + ex.Message;
                        Logger.WriteLog("JournalEntry", drPaymentDetails["AccSysId"].ToString(), status + ":" + statusMessage, true);
                    }

                }
                



            }

        }


        #region Credit Note Posting

        public void AgentVoucherCreditEntries()
        {
            DataSet ds = new DataSet();

            try
            {
                ds = GetExceptionalRefundData();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetExceptionalRefundData", "", ex.Message, true);
            }


            if (ds != null && ds.Tables.Count > 0)
            {
                Logger.WriteLog("InsertExceptionalRefund", "", "No Of ExceptionalRefunds: " + ds.Tables[0].Rows.Count, true);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        decimal amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["refundamount"].ToString());

                         int UserID = Convert.ToInt32(ds.Tables[0].Rows[i]["UserIdBooked"]);
                        int CompanyID = Convert.ToInt32(ds.Tables[0].Rows[i]["CompanyID"]);
                        int VoucherNo = Convert.ToInt32(ds.Tables[0].Rows[i]["vouchernumber"]);
                        int bookingid = Convert.ToInt32(ds.Tables[0].Rows[i]["BookingId"]);
                        int IsDisputed = Convert.ToInt32(ds.Tables[0].Rows[i]["isdisputed"]);
                        int divisionId = Convert.ToInt32(ds.Tables[0].Rows[i]["divisionid"]);
                        int Classid = Convert.ToInt32(ds.Tables[0].Rows[i]["accsysclassid"]);
                        DateTime CreditDateTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["entrydatetime"].ToString());

                        string memo = "BookingId - " + bookingid.ToString();


                        InsertCreditEntryPostingToCRS("ER", "", "", "", "0", CreditDateTime, "", "", divisionId, Classid, amount, CompanyID, bookingid, UserID, IsDisputed);
                    }
                    catch (Exception ex)
                    {

                        Logger.WriteLog("InsertCreditEntryPostingToCRS", "", ex.Message, true);
                    }

                }
            }
            else
            {
                Logger.WriteLog("InsertCreditEntry", "", "No Of ExceptionalRefunds: 0", true);
            }



        }


        private DataSet GetExceptionalRefundData()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

               
                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);
               

                DataSet dstOutPut = dal.ExecuteSelect("spGetExceptionalRefundDataForQuickBooks", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void PostCreditEntry()
        {
            List<AgentVoucherDetails> agentVoucherListCM = null;

            try
            {
                agentVoucherListCM = GetAgentVoucherCreditDetailsFromCRS();
            }
            catch (Exception)
            {

                throw;
            }

            if (agentVoucherListCM != null && agentVoucherListCM.Count > 0)
            {
                foreach (AgentVoucherDetails avDetails in agentVoucherListCM)
                {
                    CreditMemo ivPosted;
                    string status = "";
                    string statusMessage = "";
                    Int32 cnID = -1;
                    try
                    {
                        string memo = "";
                        if (avDetails.IsDisputed == 1)
                            memo = "Disputed -- Automaticaly posted from CRS";
                        else
                            memo = "-- Automaticaly posted from CRS";
                        ivPosted = null;
                        //  ivPosted = PostCreditMemo(avDetails.AgentID, avDetails.AgentName, avDetails.QuickBookCustomerID, avDetails.FromCityName, avDetails.ToCityName, avDetails.RouteFromCityName, avDetails.RouteToCityName, avDetails.BusNumber, avDetails.AgentPhone1, avDetails.AgentPhone2, avDetails.Amount, avDetails.PNR, avDetails.PassengerName, avDetails.SeatNos, avDetails.SeatCount, avDetails.FromTo, avDetails.JDate, avDetails.JTime, avDetails.BDate, avDetails.BookingID, avDetails.GeneratedDate, avDetails.TransactionID, avDetails.BusType, avDetails.TripCode, avDetails.ItemName, avDetails.ItemID, avDetails.ClassName, avDetails.ClassID, avDetails.BranchDivisionName, avDetails.BranchDivisionID, memo, avDetails.docformattednumber);
                        cnID = Convert.ToInt32(ivPosted.Id);
                        status = "Posted";
                        statusMessage = "";

                        // CMAmount = CMAmount + Convert.ToDecimal(avDetails.Amount);

                        UpdateQuickBookCreditNote(cnID, avDetails.VoucherCreditNoteId);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("AgentVoucherCreditEntries", "PostCredit-UpdateQuickBookCreditNote", ex.Message, true);
                    }
                }
            }

        }

        private List<AgentVoucherDetails> GetAgentVoucherCreditDetailsFromCRS()
        {

            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);


                DataSet dstOutPut = dal.ExecuteSelect("spGetAgentVoucherCreditDetailsForQuickBooks", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<AgentVoucherDetails> agentVoucherDetailsList = new List<AgentVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drAgentVoucherDetails in dstOutPut.Tables[0].Rows)
                    {

                        Int32 Merge ;
                        Merge = 0;
                        if (!Int32.TryParse(drAgentVoucherDetails["IsMerge"].ToString(), out Merge))
                        {

                            Logger.WriteLog("GetAgentVoucherCreditMemoFromCRS::Validation::Invalid Classid for VoucherNo: " + drAgentVoucherDetails["PNR"].ToString() + " BusId: " + drAgentVoucherDetails["BusId"].ToString());
                        }
                        else
                        {
                            AgentVoucherDetails agentVoucherDetails = new AgentVoucherDetails();
                            {
                                agentVoucherDetails.AgentID = Convert.ToInt32(drAgentVoucherDetails["AgentID"].ToString());
                                agentVoucherDetails.AgentName = drAgentVoucherDetails["AgentName"].ToString();
                                agentVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drAgentVoucherDetails["QuickBookCustomerID"].ToString());
                                agentVoucherDetails.FromCityName = drAgentVoucherDetails["FromCityName"].ToString();
                                agentVoucherDetails.ToCityName = drAgentVoucherDetails["ToCityName"].ToString();
                                agentVoucherDetails.RouteFromCityName = drAgentVoucherDetails["RouteFromCityName"].ToString();
                                agentVoucherDetails.RouteToCityName = drAgentVoucherDetails["RouteToCityName"].ToString();
                                agentVoucherDetails.BusNumber = drAgentVoucherDetails["BusNumber"].ToString();
                                agentVoucherDetails.BusType = drAgentVoucherDetails["ChartName"].ToString();
                                agentVoucherDetails.AgentPhone1 = drAgentVoucherDetails["ContactNo1"].ToString();
                                agentVoucherDetails.AgentPhone2 = drAgentVoucherDetails["ContactNo2"].ToString();
                                agentVoucherDetails.Amount = Convert.ToDecimal(drAgentVoucherDetails["amount"].ToString());
                                agentVoucherDetails.PNR = drAgentVoucherDetails["PNR"].ToString();
                                agentVoucherDetails.PassengerName = drAgentVoucherDetails["PassengerName"].ToString();
                                agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                agentVoucherDetails.FromTo = drAgentVoucherDetails["FromTo"].ToString();
                                agentVoucherDetails.JDate = drAgentVoucherDetails["JourneyDate"].ToString();
                                agentVoucherDetails.JTime = drAgentVoucherDetails["JTime"].ToString();
                                agentVoucherDetails.BDate = drAgentVoucherDetails["BookingDate"].ToString();
                                agentVoucherDetails.BookingID = Convert.ToInt32(drAgentVoucherDetails["BookingID"].ToString());
                                agentVoucherDetails.GeneratedDate = Convert.ToDateTime(drAgentVoucherDetails["GeneratedDate"].ToString());
                                agentVoucherDetails.TransactionID = Convert.ToInt32(drAgentVoucherDetails["TransactionID"].ToString());
                                if (drAgentVoucherDetails["ItemID"].ToString() != "")
                                    agentVoucherDetails.ItemID = Convert.ToInt32(drAgentVoucherDetails["ItemID"].ToString());
                                agentVoucherDetails.ItemName = drAgentVoucherDetails["Item"].ToString();
                                agentVoucherDetails.ClassID = drAgentVoucherDetails["ClassID"].ToString();
                                agentVoucherDetails.ClassName = drAgentVoucherDetails["classname"].ToString();
                                agentVoucherDetails.BranchDivisionID = drAgentVoucherDetails["BranchDivisionID"].ToString();
                                agentVoucherDetails.BranchDivisionName = drAgentVoucherDetails["BranchDivisionName"].ToString();
                                agentVoucherDetails.VoucherCreditNoteId = Convert.ToInt32(drAgentVoucherDetails["VoucherCreditNoteId"].ToString());
                                agentVoucherDetails.IsDisputed = Convert.ToInt32(drAgentVoucherDetails["IsDisputed"].ToString());
                                agentVoucherDetails.docformattednumber = drAgentVoucherDetails["docformattednumber"].ToString();
                            }

                            agentVoucherDetailsList.Add(agentVoucherDetails);
                        }
                    }

                }

                return agentVoucherDetailsList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void InsertCreditEntryPostingToCRS(string doctype, string docsubtype, string docnumber, string docformattednumber, string accsysid,DateTime CreditNotedatetime, string narration, string description, int accsysdivisionid, int accsysclassid, decimal amount, int CompanyID, int BookingID,int UserID,int isDisputed)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_doctype", doctype, doctype.Length, ParameterDirection.Input);
                dal.AddParameter("p_docsubtype", docsubtype, docsubtype.Length, ParameterDirection.Input);
                dal.AddParameter("p_docnumber", docnumber, docnumber.Length, ParameterDirection.Input);
                dal.AddParameter("p_docformattednumber", docformattednumber, docformattednumber.Length, ParameterDirection.Input);
                dal.AddParameter("p_accsysid", accsysid, accsysid.Length, ParameterDirection.Input);

               

                dal.AddParameter("p_creditnotedatetime", CreditNotedatetime, ParameterDirection.Input);
                //dal.AddParameter("p_narration", narration, narration.Length, ParameterDirection.Input);
                dal.AddParameter("p_amount", amount, ParameterDirection.Input);
                dal.AddParameter("p_IsDisputed", isDisputed, ParameterDirection.Input);
                // dal.AddParameter("p_accsysledgerid_cr", accsysledgerid_cr, ParameterDirection.Input);
                // dal.AddParameter("p_description_cr", description_cr, description_cr.Length, ParameterDirection.Input);
                dal.AddParameter("p_accsysdivisionid", accsysdivisionid, ParameterDirection.Input);
                dal.AddParameter("p_accsysclassid", accsysclassid, ParameterDirection.Input);

                dal.AddParameter("p_bookingid", BookingID, ParameterDirection.Input);
                // dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_UserID", UserID, ParameterDirection.Input);
               
            

                dal.ExecuteDML("spInsertQuickBookCreditNoteEntry", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

         private void InsertInvoiceItemDetails(int accsysid, int itemid)
        {

            CRSDAL dal = new CRSDAL();
            string strErr = "";
            //int Accsysid = 0;

            //String docnumber = (PaymentList[i].PrivateNote).Substring(14,12);
            try
            {



                dal.AddParameter("p_Accsysid", accsysid, ParameterDirection.Input);
                dal.AddParameter("p_itemid", itemid, ParameterDirection.Input);

                //DataSet dsOutPut = dal.ExecuteSelect("spSet_QBPaymentDetails", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                // int status = dal.ExecuteDML("spTallySet_UserwiseCollection_BranchTotal_V2", CommandType.StoredProcedure, 0, ref strErr);


            }
            catch (Exception ex)
            {
                Logger.WriteLog("Test", Convert.ToString(accsysid), "", true);
            }





        }

        #endregion


        #region CashBookRecon
           
        public void CashBookReconQB()
        {

            DataSet ds = new DataSet();

            ds = GetCRSCashierLedger(1945);

            if (ds != null && ds.Tables.Count>0)
            {

                if (ds.Tables[0].Rows.Count>0)
                {

                    foreach (DataRow drAccounts in ds.Tables[0].Rows)
                    {
                        string status = "";
                        string statusMessage = "";
                        Int32 id;
                        Decimal QBClosingBalance = 0;
                        Decimal CRSClosingBalance = 0;
                        try
                        {
                            DataSet dsout = new DataSet();
                            dsout = GetCRSCashierClosingBal(1945, Convert.ToInt32(drAccounts["AccSysLedgerId"].ToString()));
                            if (dsout != null && dsout.Tables.Count > 0 && dsout.Tables[0].Rows.Count>0)
                            {
                                Account AccountData = new Account();
                                AccountData = GetAccount(drAccounts["Accsysid"].ToString());
                                QBClosingBalance = (decimal)AccountData.CurrentBalance;
                                CRSClosingBalance = Convert.ToDecimal(dsout.Tables[0].Rows[0]["ClosingBalance"].ToString());
                                id = Convert.ToInt32(AccountData.Id);
                                status = "Success";

                            }
                            else
                            {
                                status = "Failed";
                            }
                          
                        }
                        catch (Exception ex)
                        {

                            status = "Failed";
                            statusMessage = "Insert- " + ex.Message + " : " + ex.Message;
                            Logger.WriteLog("CashBookRecon", drAccounts["AccSysLedgerId"].ToString(), status + ":" + statusMessage, true);
                        }


                        if (status == "Success")
                        {
                            InsertQBClosingBalance(Convert.ToInt32(drAccounts["AccSysLedgerId"].ToString()), QBClosingBalance, CRSClosingBalance);

                        }

                    }

                    
                }

            }



        }

        private DataSet GetCRSCashierLedger(int Companyid)
        {
            DataSet dsoutput = null;

            try
            {
                string strerror = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_companyid", Companyid, ParameterDirection.Input);
                dsoutput = dal.ExecuteSelect("spGetCashierLedgerFromCRS", CommandType.StoredProcedure, 0, ref strerror, "p_ErrMsg", false, "", false);
                return dsoutput;
            }
            catch (Exception ex)
            {

                Logger.WriteLog("GetCRSCashierLedger", Convert.ToString(Companyid), "", true);
            }

            return dsoutput;

        }


        private DataSet GetCRSCashierClosingBal(int companyid,int Ledgerid)
        {
            DataSet dsoutput = null;

            try
            {
                string strerr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_companyid",companyid);
                dal.AddParameter("p_Ledgerid",Ledgerid);
                dsoutput = dal.ExecuteSelect("spGetCashierLedgerClosingBal", CommandType.StoredProcedure, 0, ref strerr, "p_ErrMsg", false, "", false);

                return dsoutput;

            }
            catch (Exception)
            {

                throw;
            }

            return dsoutput;
        }

        public static Account GetAccount(string AccountID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                Intuit.Ipp.QueryFilter.QueryService<Account> queryServiceForItem = new Intuit.Ipp.QueryFilter.QueryService<Account>(context);
                System.Collections.ObjectModel.ReadOnlyCollection<Account> accountList = queryServiceForItem.ExecuteIdsQuery("select * from account where id = '" + AccountID + "'");
                Account account = accountList.Take(1).FirstOrDefault();
                return account;
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }

        }


        public void InsertQBClosingBalance(int Ledgerid, decimal QBClosingBal,decimal CRSClosingBal)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_Ledgerid", Ledgerid);
                dal.AddParameter("p_CompanyId", 1945);
                dal.AddParameter("p_QbClosingbal", QBClosingBal);
                dal.AddParameter("p_CRSClosingBal", CRSClosingBal);
                dal.ExecuteDML("spInsertCRSQBLedgerBalance", CommandType.StoredProcedure, 0, ref strErr);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region FranchiseVoucher

        public void PostFranchiseVouchers()
        {
            #region FRanchise Voucher Posting in Invoice

            List<FranchiseVoucherDetails> FranchiseVoucherList = null;
            try
            {
                FranchiseVoucherList = GetFranchiseVoucherDetailsFromCRS();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("PostFranchiseVouchers", "GetFranchiseVoucherDetailsFromCRS", ex.Message, true);
            }

            if (FranchiseVoucherList != null && FranchiseVoucherList.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                Logger.WriteLog("PostFranchiseVouchers", "", "No Of Franchise Vouchers: " + FranchiseVoucherList.Count, true);
                foreach (FranchiseVoucherDetails fvDetails in FranchiseVoucherList)
                {
                    Invoice ivPosted;
                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    try
                    {
                        string docnumber = "";
                        string docformattednumber = "";

                        CreateFranchiseVoucherNoForQB(fvDetails.CompanyID, Convert.ToInt32(fvDetails.DivisionID), fvDetails.BookingID, fvDetails.GeneratedDate, ref docnumber, ref docformattednumber);

                        ivPosted = PostFranchiseInvoice("Insert", fvDetails.FranchiseID, "", "", fvDetails.FranchiseName, fvDetails.QuickBookCustomerID, fvDetails.FromCityName, fvDetails.ToCityName, fvDetails.RouteFromCityName, fvDetails.RouteToCityName, fvDetails.BusNumber, fvDetails.AgentPhone1, fvDetails.AgentPhone2, fvDetails.Amount, fvDetails.PNR, fvDetails.PassengerName, fvDetails.SeatNos, fvDetails.SeatCount, fvDetails.FromTo, fvDetails.JDate, fvDetails.JTime, fvDetails.BDate, fvDetails.BookingID, fvDetails.VoucherDate, fvDetails.TransactionID, fvDetails.BusType, fvDetails.TripCode, fvDetails.ItemName, fvDetails.ItemID, fvDetails.ClassName, fvDetails.ClassID, fvDetails.BranchDivisionName, fvDetails.BranchDivisionID, docformattednumber, fvDetails.BookingStatus, fvDetails.Prefix, fvDetails.TotalFare, fvDetails.RefundAmount,fvDetails.PickUpName,fvDetails.DropOffName);
                        ivID = Convert.ToInt32(ivPosted.Id);
                        status = "Posted";
                        statusMessage = "";

                        EntryCounter.GetInstance().IncreaseQBCount(1);

                        InsertUpdateFranchiseVoucherPostingDetailsToCRS(fvDetails.FranchiseID, fvDetails.CompanyID, fvDetails.BookingID, ivID, docnumber, docformattednumber, fvDetails.BusMasterID, fvDetails.DivisionID);

                        EntryCounter.GetInstance().IncreaseCRSCount(1);
                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteLog("Exception2");

                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("PostFranchiseVouchers", "", ex.Message, true);
                    }

                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "PostFranchiseVouchers:::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }




            #endregion

            return;
        }


        private List<FranchiseVoucherDetails> GetFranchiseVoucherDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();
                DateTime fromDate = new DateTime(2019, 04, 01);
                DateTime toDate = new DateTime(2019, 04, 01);
                dal.AddParameter("p_CompanyID", 69, ParameterDirection.Input);
                dal.AddParameter("p_FromDate", fromDate, ParameterDirection.Input);
                dal.AddParameter("p_ToDate", toDate, ParameterDirection.Input);
               
               

                DataSet dstOutPut = dal.ExecuteSelect("spGetFranchiseVoucherForQuickBooks_konduskar", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

              
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0 && dstOutPut.Tables[0].Rows[0]["BookingIDs"].ToString() != "0")
                {

                    string BookingIDs = dstOutPut.Tables[0].Rows[0]["BookingIDs"].ToString();

                    Logger.WriteLog("GetFranchiseVoucherDetailsFromCRS::Validation:: Franchise Vouchers Not Posted : " + BookingIDs);
                    string msg = "GetFranchiseVoucherDetailsFromCRS::Validation:: Franchise Vouchers Not Posted  : " + BookingIDs;
                    //Email.SendMail(msg);
                  
                }

               
                List<FranchiseVoucherDetails> franchiseVoucherDetailsList = new List<FranchiseVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[1].Rows.Count > 0)
                {

                    foreach (DataRow drFranchiseVoucherDetails in dstOutPut.Tables[1].Rows)
                    {
                        FranchiseVoucherDetails franchiseVoucherDetails = new FranchiseVoucherDetails();
                        try
                        {
                            Int32 qbCustomerId,divisionId ,tripid , Merge;
                            qbCustomerId = 0;  divisionId = 0;  tripid = 0;  Merge = 0;
                            if (!Int32.TryParse(drFranchiseVoucherDetails["QuickBookCustomerID"].ToString(), out qbCustomerId))
                            {
                                Logger.WriteLog("GetFranchiseVoucherDetailsFromCRS::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString());
                                string msg = "GetFranchiseVoucherDetailsFromCRS::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString();
                                Email.SendMail(msg);

                            }
                            else if (!Int32.TryParse(drFranchiseVoucherDetails["divisionid"].ToString(), out divisionId))
                            {
                                Logger.WriteLog("GetFranchiseVoucherDetailsFromCRS::Validation::Invalid DivisionId for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString());
                                string msg = "GetFranchiseVoucherDetailsFromCRS::Validation::Invalid DivisionId for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString();
                                Email.SendMail(msg);
                            }

                            else if (!Int32.TryParse(drFranchiseVoucherDetails["ItemID"].ToString(), out tripid))
                            {
                                Logger.WriteLog("GetFranchiseVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString());
                                string msg = "GetFranchiseVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString();
                                Email.SendMail(msg);
                            }
                            else if (!Int32.TryParse(drFranchiseVoucherDetails["IsMerge"].ToString(), out Merge))
                            {
                                Logger.WriteLog("GetFranchiseVoucherDetailsFromCRS::Validation::Invalid Classid for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " BusId: " + drFranchiseVoucherDetails["BusId"].ToString());
                                string msg = "GetFranchiseVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString();
                                Email.SendMail(msg);
                            }
                            else
                            {
                                franchiseVoucherDetails.FranchiseID = Convert.ToInt32(drFranchiseVoucherDetails["FranchiseID"].ToString());
                                franchiseVoucherDetails.FranchiseName = drFranchiseVoucherDetails["BranchName"].ToString();
                                franchiseVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drFranchiseVoucherDetails["QuickBookCustomerID"].ToString());
                                franchiseVoucherDetails.FromCityName = drFranchiseVoucherDetails["FromCityName"].ToString();
                                franchiseVoucherDetails.ToCityName = drFranchiseVoucherDetails["ToCityName"].ToString();
                                franchiseVoucherDetails.RouteFromCityName = drFranchiseVoucherDetails["RouteFromCityName"].ToString();
                                franchiseVoucherDetails.RouteToCityName = drFranchiseVoucherDetails["RouteToCityName"].ToString();
                                franchiseVoucherDetails.BusNumber = drFranchiseVoucherDetails["BusNumber"].ToString();
                                franchiseVoucherDetails.BusType = drFranchiseVoucherDetails["ChartName"].ToString();
                                franchiseVoucherDetails.AgentPhone1 = drFranchiseVoucherDetails["ContactNo1"].ToString();
                                franchiseVoucherDetails.AgentPhone2 = drFranchiseVoucherDetails["ContactNo2"].ToString();
                                franchiseVoucherDetails.Amount = Convert.ToDecimal(drFranchiseVoucherDetails["NetAmt"].ToString());
                                franchiseVoucherDetails.PNR = drFranchiseVoucherDetails["PNR"].ToString();
                                franchiseVoucherDetails.PassengerName = drFranchiseVoucherDetails["PassengerName"].ToString();
                                franchiseVoucherDetails.SeatNos = drFranchiseVoucherDetails["SeatNos"].ToString();
                                franchiseVoucherDetails.SeatCount = Convert.ToInt32(drFranchiseVoucherDetails["SeatCount"].ToString());
                                franchiseVoucherDetails.FromTo = drFranchiseVoucherDetails["FromTo"].ToString();
                                franchiseVoucherDetails.JDate = drFranchiseVoucherDetails["JourneyDate"].ToString();
                                franchiseVoucherDetails.JTime = drFranchiseVoucherDetails["JTime"].ToString();
                                franchiseVoucherDetails.BDate = drFranchiseVoucherDetails["BookingDate"].ToString();
                                franchiseVoucherDetails.BookingID = Convert.ToInt32(drFranchiseVoucherDetails["BookingID"].ToString());
                                franchiseVoucherDetails.GeneratedDate = Convert.ToDateTime(drFranchiseVoucherDetails["GeneratedDate"].ToString());
                                franchiseVoucherDetails.TransactionID = Convert.ToInt32(drFranchiseVoucherDetails["TransactionID"].ToString());
                                if (drFranchiseVoucherDetails["ItemID"].ToString() != "")
                                    franchiseVoucherDetails.ItemID = Convert.ToInt32(drFranchiseVoucherDetails["ItemID"].ToString());
                                franchiseVoucherDetails.ItemName = drFranchiseVoucherDetails["Item"].ToString();
                                franchiseVoucherDetails.ClassID = drFranchiseVoucherDetails["ClassID"].ToString();
                                franchiseVoucherDetails.ClassName = drFranchiseVoucherDetails["classname"].ToString();
                                franchiseVoucherDetails.BranchDivisionID = drFranchiseVoucherDetails["BranchDivisionID"].ToString();
                                franchiseVoucherDetails.BranchDivisionName = drFranchiseVoucherDetails["BranchDivisionName"].ToString();
                                franchiseVoucherDetails.CompanyID = Convert.ToInt32(drFranchiseVoucherDetails["CompanyID"].ToString());
                                franchiseVoucherDetails.VoucherNo = drFranchiseVoucherDetails["VoucherNo"].ToString();
                                franchiseVoucherDetails.BookingStatus = drFranchiseVoucherDetails["BookingStatus"].ToString();
                                franchiseVoucherDetails.Prefix = drFranchiseVoucherDetails["Prefix"].ToString();

                                franchiseVoucherDetails.TotalFare = Convert.ToDecimal(drFranchiseVoucherDetails["TotalFare"].ToString());
                                franchiseVoucherDetails.RefundAmount = Convert.ToDecimal(drFranchiseVoucherDetails["RefundAmount"].ToString());

                                franchiseVoucherDetails.DivisionID = Convert.ToInt32(drFranchiseVoucherDetails["divisionid"].ToString());
                                franchiseVoucherDetails.BusMasterID = Convert.ToInt32(drFranchiseVoucherDetails["BusMasterId"].ToString());
                                franchiseVoucherDetails.VoucherDate = Convert.ToDateTime(drFranchiseVoucherDetails["VoucherDate"].ToString());
                                franchiseVoucherDetails.PickUpName = drFranchiseVoucherDetails["PickUpName"].ToString();
                                franchiseVoucherDetails.DropOffName = drFranchiseVoucherDetails["DropOffName"].ToString();

                                franchiseVoucherDetailsList.Add(franchiseVoucherDetails);
                            }

                        }

                        catch (Exception ex)
                        {
                            Logger.WriteLog("GetFranchiseVoucherDetailsFromCRS", " FranchiseId: " + franchiseVoucherDetails.FranchiseID + " VoucherNo: " + franchiseVoucherDetails.VoucherNo, ex.Message, true);
                        }

                    }

                }


                return franchiseVoucherDetailsList;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private void CreateFranchiseVoucherNoForQB(Int32 CompanyID, Int32 DivisionID, Int32 BookingID, DateTime GeneratedDate, ref string docnumber, ref string docformattednumber)
        {

            try
            {
                string strErr = "";
                //string strVoucherNo = "";
                DataSet ds = null;
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_DivisionID", DivisionID, ParameterDirection.Input);
                dal.AddParameter("p_BookingID", BookingID, ParameterDirection.Input);
                dal.AddParameter("p_GeneratedDateTime", GeneratedDate, ParameterDirection.Input);

                //ds = dal.ExecuteSelect("spCreateAgentVoucherNoforQB_V2", CommandType.StoredProcedure, 0, ref strErr);

                //if (ds != null && ds.Tables.Count >= 1)
                //{
                //    docnumber = ds.Tables[0].Rows[0]["docnumber"].ToString();
                //    docformattednumber = ds.Tables[0].Rows[0]["docformattednumber"].ToString();
                //}
                docnumber = "";
                docformattednumber = "HOINVCRS" + BookingID;

                return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void InsertUpdateFranchiseVoucherPostingDetailsToCRS(Int32 FranchiseID, Int32 CompanyID, Int32 BookingID, Int32 AccSysID, string docnumber, string docformattednumber, int classid, int divisionid)
        {

            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                //Logger.WriteLog("Master");
                dal.AddParameter("p_FranchiseID", FranchiseID, ParameterDirection.Input);
                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_BookingID", BookingID, ParameterDirection.Input);
                dal.AddParameter("p_AccSysID", AccSysID, ParameterDirection.Input);
                dal.AddParameter("p_docnumber", docnumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_docformattednumber", docformattednumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_classid", classid, ParameterDirection.Input);
                dal.AddParameter("p_divisionid", divisionid, ParameterDirection.Input);

                dal.ExecuteDML("spInsertUpdateQuickBookFranchiseVoucherStatus", CommandType.StoredProcedure, 0, ref strErr);
                //Logger.WriteLog("Done");
            }
            catch (Exception)
            {
                Logger.WriteLog("Exception1");
                throw;
            }
        }


        private Invoice PostFranchiseInvoice(string action, int FranchiseID, string invoiceid, string synctoken, string FranchiseName, int QuickBookCustomerID, string FromCityName, string ToCityName, string RouteFromCityName, string RouteToCityName, string BusNumber, string AgentPhone1, string AgentPhone2, decimal Amount, string PNR, string PassengerName, string SeatNos, int SeatCount, string FromTo, string JDate, string JTime, string BDate, int BookingID, DateTime VoucherDate, int TransactionID, string BusType, string TripCode, string ItemName, int intItemID, string ClassName, string ClassID, string DivisionName, string DivisionID, string VoucherNo, string BookingStatus, string Prefix, decimal TotalFare, Decimal RefundAmount,string PickName,string DropoffName)

        {
            try
            {
                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                //ServiceContext context = QuickBookConnection.GetDataServiceContext();
                var service = new DataService(context);

                decimal Lineamount = 0;
                /* Invoice Begin */
                //customer name, date, payment method, deposit to, service, description, Qty, Rate, Amout , Tax

                Invoice invoice = new Invoice();
                { 
                invoice.TxnDate = VoucherDate;
                invoice.TxnDateSpecified = true;


                invoice.CustomerRef = new ReferenceType() { name = FranchiseName, Value = QuickBookCustomerID.ToString() };

                //invoice.SalesTermRef = new ReferenceType() { name = "Due on receipt", Value = "9" };
                }
                if (DivisionName != "")
                    invoice.DepartmentRef = new ReferenceType() { name = DivisionName, Value = DivisionID }; //agent area

                List<CustomField> customFieldList = new List<CustomField>();
                CustomField pickupdrop = new CustomField();
                {
                    pickupdrop.DefinitionId = "1";
                    pickupdrop.Name = "PickUp-Drop";
                    pickupdrop.AnyIntuitObject = FromCityName + "-" + ToCityName;
                }
                customFieldList.Add(pickupdrop);

                CustomField JourneyDate = new CustomField();
                {
                    JourneyDate.DefinitionId = "2";
                    JourneyDate.Name = "Journey Date";
                    JourneyDate.AnyIntuitObject = JDate;
                }
                customFieldList.Add(JourneyDate);

                CustomField PhoneNumber = new CustomField();
                {
                    PhoneNumber.DefinitionId = "3";
                    PhoneNumber.Name = "Phone Number";
                    PhoneNumber.AnyIntuitObject = AgentPhone1 + "," + AgentPhone2;
                }
                customFieldList.Add(PhoneNumber);

                invoice.CustomField = customFieldList.ToArray();

                List<Line> invoiceLineList = new List<Line>();

                if (BookingStatus == "C")
                {
                    SalesItemLineDetail saleItemDetail = new SalesItemLineDetail();



                    {
                        saleItemDetail.ItemRef = new ReferenceType() { name = ItemName, Value = intItemID.ToString() };

                        if (ClassName != "")
                            saleItemDetail.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };

                    }
                    string strDesc = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                        "Seats : " + SeatNos + "\n" +
                        "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                        "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                        "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                         + ",\nPickUp Name : " + PickName + ",\nDropOff Name : " + DropoffName;

                    if (BookingStatus == "C")
                        strDesc = "Cancelled - " + strDesc;

                    Line invoiceLine = new Line();
                    {
                        invoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        invoiceLine.DetailTypeSpecified = true;

                        invoiceLine.Amount = Amount;
                        invoiceLine.AmountSpecified = true;
                        invoiceLine.AnyIntuitObject = saleItemDetail;
                        invoiceLine.Description = strDesc;
                    }
                    Lineamount = Amount;

                    invoiceLineList.Add(invoiceLine);
                }
                else
                {
                    List<FranciseBookingDetails> BookingDetailsList = GetFranchiseBookingDetailsFromCRS(BookingID);
                    foreach (FranciseBookingDetails bkgDetails in BookingDetailsList)
                    {

                        decimal Commissionperseatwise = 0;
                        decimal discountperseatwise = 0;
                        decimal diffamt = 0;
                        Commissionperseatwise = bkgDetails.FranchiseComm / SeatCount;
                        discountperseatwise = bkgDetails.Disc / SeatCount;
                        diffamt = bkgDetails.ActualAmt - bkgDetails.TotalLineAmt;
                        diffamt = diffamt / SeatCount;
                        SalesItemLineDetail saleItemDetail = new SalesItemLineDetail();
                        {
                            saleItemDetail.Qty = new Decimal(bkgDetails.SeatCount);
                            saleItemDetail.QtySpecified = true;
                            if (diffamt > 0)
                            {
                                saleItemDetail.AnyIntuitObject = bkgDetails.Fare - (Commissionperseatwise + discountperseatwise) + diffamt; // Amount / SeatCount; // 2500m;
                            }
                            else
                            {
                                saleItemDetail.AnyIntuitObject = bkgDetails.Fare - (Commissionperseatwise + discountperseatwise) - diffamt; // Amount / SeatCount; // 2500m;
                            }

                            saleItemDetail.ItemElementName = ItemChoiceType.UnitPrice;




                            saleItemDetail.ItemRef = new ReferenceType() { name = ItemName, Value = intItemID.ToString() };

                            if (ClassName != "")
                                saleItemDetail.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };
                        }


                        string strDesc = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                            "Seats : " + bkgDetails.SeatNos + "\n" +
                            "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                            "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                            "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                             + ",\nPickUp Name : " + PickName + ",\nDropOff Name : " + DropoffName;

                        if (BookingStatus == "C")
                            strDesc = "Cancelled - " + strDesc;

                        Line invoiceLine = new Line();
                        {
                            invoiceLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                            invoiceLine.DetailTypeSpecified = true;


                            if (diffamt > 0)
                            {
                                invoiceLine.Amount = (bkgDetails.Fare - (Commissionperseatwise + discountperseatwise) + diffamt) * bkgDetails.SeatCount; // 1000; // Amount;
                                Lineamount += (bkgDetails.Fare - (Commissionperseatwise + discountperseatwise) + diffamt) * bkgDetails.SeatCount;
                            }
                            else
                            {
                                invoiceLine.Amount = (bkgDetails.Fare - (Commissionperseatwise + discountperseatwise + diffamt)) * bkgDetails.SeatCount; // 1000; // Amount;
                                Lineamount += (bkgDetails.Fare - (Commissionperseatwise + discountperseatwise + diffamt)) * bkgDetails.SeatCount;
                            }

                            invoiceLine.AmountSpecified = true;
                            invoiceLine.AnyIntuitObject = saleItemDetail;
                            invoiceLine.Description = strDesc;
                        }

                        invoiceLineList.Add(invoiceLine);

                        
                    }
                }


                invoice.TotalAmt = Amount;
                invoice.TotalAmtSpecified = true;


                //TxnTaxDetail
                TxnTaxDetail txnTax = new TxnTaxDetail();
                {
                    txnTax.TotalTaxSpecified = true;
                    txnTax.TotalTax = 0;
                }

                invoice.GlobalTaxCalculation = GlobalTaxCalculationEnum.NotApplicable;
                invoice.GlobalTaxCalculationSpecified = true;

                invoice.Line = invoiceLineList.ToArray();

                invoice.DocNumber = VoucherNo;
                string strNo = "[Voucher No : " + VoucherNo + "]";

                if (BookingStatus == "C")
                    invoice.PrivateNote = strNo + ", Cancelled - PNR : " + BookingID + " -- Automaticaly posted from CRS";
                else
                    invoice.PrivateNote = strNo + ", PNR : " + BookingID;


                if (action.Equals("Update"))
                {
                    invoice.Id = invoiceid;
                    invoice.SyncToken = synctoken;
                }
                Invoice postedInvoice = null;

                decimal DIFF = Math.Abs(Lineamount - Amount);
                if (Convert.ToInt32(DIFF) < 1) // Not Posted if Line Amount Is Not  Matching With Net Amount
                {
                    postedInvoice = service.Add(invoice);
                }


                return postedInvoice;
                /* Invoice End*/

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }

        private List<FranciseBookingDetails> GetFranchiseBookingDetailsFromCRS(int intBookingID)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_BookingID", intBookingID, ParameterDirection.Input);
                DataSet dstOutPut = dal.ExecuteSelect("spGetFranchiseBookingDetailsForQuickBooks", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<FranciseBookingDetails> BookingDetailsList = new List<FranciseBookingDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drBookingDetails in dstOutPut.Tables[0].Rows)
                    {
                        FranciseBookingDetails BookingDetails = new FranciseBookingDetails();
                        {
                            BookingDetails.SeatNos = drBookingDetails["SeatNos"].ToString();
                            BookingDetails.SeatCount = Convert.ToInt32(drBookingDetails["SeatCount"].ToString());
                            BookingDetails.Fare = Convert.ToDecimal(drBookingDetails["Fare"].ToString());
                            BookingDetails.Comm = Convert.ToDecimal(drBookingDetails["Comm"].ToString());
                            BookingDetails.FranchiseComm = Convert.ToDecimal(drBookingDetails["FranchiseComm"].ToString());
                            BookingDetails.Disc = Convert.ToDecimal(drBookingDetails["Disc"].ToString());
                            BookingDetails.TotalLineAmt = Convert.ToDecimal(dstOutPut.Tables[1].Rows[0]["TotalFare"].ToString());
                            BookingDetails.ActualAmt = Convert.ToDecimal(dstOutPut.Tables[1].Rows[0]["TotalVoucherAmt"].ToString());
                        }
                        BookingDetailsList.Add(BookingDetails);

                    }



                }

                return BookingDetailsList;
            }
            catch (Exception)
            {
                throw;
            }

        }



        public void UpdateFranchiseVouchers()
        {
            #region Franchise Voucher Update in Invoice

            List<FranchiseVoucherDetails> FranchiseVoucherList = null;
            try
            {
                FranchiseVoucherList = GetFranchiseVoucherUpdateDetailsFromCRS();
            }
            catch (Exception ex)
            {
                Logger.WriteLog("UpdateFranchiseVouchers", "GetFranchiseVoucherUpdateDetailsFromCRS", ex.Message, true);
            }

            if (FranchiseVoucherList != null && FranchiseVoucherList.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                Logger.WriteLog("UpdateFranchiseVouchers", "", "No Of Franchise Vouchers: " + FranchiseVoucherList.Count, true);
                foreach (FranchiseVoucherDetails fvDetails in FranchiseVoucherList)
                {
                    Invoice ivPosted;
                    string status = "";
                    string statusMessage = "";
                    Int32 ivID = -1;
                    Invoice InvoiceData = new Invoice();
                    try
                    {

                        InvoiceData = Getinvoice(fvDetails.Accsysid);
                        if (InvoiceData != null)
                        {
                            ivPosted = PostFranchiseInvoice("Update", fvDetails.FranchiseID, InvoiceData.Id, InvoiceData.SyncToken, fvDetails.FranchiseName, fvDetails.QuickBookCustomerID, fvDetails.FromCityName, fvDetails.ToCityName, fvDetails.RouteFromCityName, fvDetails.RouteToCityName, fvDetails.BusNumber, fvDetails.AgentPhone1, fvDetails.AgentPhone2, fvDetails.Amount, fvDetails.PNR, fvDetails.PassengerName, fvDetails.SeatNos, fvDetails.SeatCount, fvDetails.FromTo, fvDetails.JDate, fvDetails.JTime, fvDetails.BDate, fvDetails.BookingID, fvDetails.GeneratedDate, fvDetails.TransactionID, fvDetails.BusType, fvDetails.TripCode, fvDetails.ItemName, fvDetails.ItemID, fvDetails.ClassName, fvDetails.ClassID, fvDetails.BranchDivisionName, fvDetails.BranchDivisionID, fvDetails.docformattednumber, fvDetails.BookingStatus, fvDetails.Prefix, fvDetails.TotalFare, fvDetails.RefundAmount,fvDetails.PickUpName,fvDetails.DropOffName);
                            ivID = Convert.ToInt32(ivPosted.Id);
                            status = "Posted";
                            statusMessage = "";

                            EntryCounter.GetInstance().IncreaseQBCount(1);

                            InsertUpdateFranchiseVoucherPostingDetailsToCRS(fvDetails.FranchiseID, fvDetails.CompanyID, fvDetails.BookingID, ivID, fvDetails.docnumber, fvDetails.docformattednumber, fvDetails.BusMasterID, fvDetails.DivisionID);

                            EntryCounter.GetInstance().IncreaseCRSCount(1);
                        }
                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("UpdateFranchiseVouchers", "", ex.Message, true);
                    }

                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "UpdateFranchiseVouchers:::Mismatch in No Of Entries Updated to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }


            #endregion

            return;


        }


        private List<FranchiseVoucherDetails> GetFranchiseVoucherUpdateDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();
              
                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);


                DataSet dstOutPut = dal.ExecuteSelect("spGetUpdateVoucherForQuickBooks", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<FranchiseVoucherDetails> franchiseVoucherDetailsList = new List<FranchiseVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drFranchiseVoucherDetails in dstOutPut.Tables[0].Rows)
                    {
                        FranchiseVoucherDetails franchiseVoucherDetails = new FranchiseVoucherDetails();
                        try
                        {
                            Int32 classid;
                            classid = 0;
                            if (!Int32.TryParse(drFranchiseVoucherDetails["classid"].ToString(), out classid))
                            {
                                Logger.WriteLog("GetFranchiseVoucherUpdateDetailsFromCRS::Validation::Bus is not schedule for VoucherNo: " + drFranchiseVoucherDetails["VoucherNo"].ToString() + " FranchiseID: " + drFranchiseVoucherDetails["FranchiseID"].ToString());
                                //string msg = "GetAgentVoucherUpdateDetailsFromCRS::Validation::Bus is not schedule for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                //Email.SendMail(msg);

                            }


                            franchiseVoucherDetails.FranchiseID = Convert.ToInt32(drFranchiseVoucherDetails["FranchiseID"].ToString());
                            franchiseVoucherDetails.FranchiseName = drFranchiseVoucherDetails["BranchName"].ToString();
                            franchiseVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drFranchiseVoucherDetails["QuickBookCustomerID"].ToString());
                            franchiseVoucherDetails.FromCityName = drFranchiseVoucherDetails["FromCityName"].ToString();
                            franchiseVoucherDetails.ToCityName = drFranchiseVoucherDetails["ToCityName"].ToString();
                            franchiseVoucherDetails.RouteFromCityName = drFranchiseVoucherDetails["RouteFromCityName"].ToString();
                            franchiseVoucherDetails.RouteToCityName = drFranchiseVoucherDetails["RouteToCityName"].ToString();
                            franchiseVoucherDetails.BusNumber = drFranchiseVoucherDetails["BusNumber"].ToString();
                            franchiseVoucherDetails.BusType = drFranchiseVoucherDetails["ChartName"].ToString();
                            franchiseVoucherDetails.AgentPhone1 = drFranchiseVoucherDetails["ContactNo1"].ToString();
                            franchiseVoucherDetails.AgentPhone2 = drFranchiseVoucherDetails["ContactNo2"].ToString();
                            franchiseVoucherDetails.Amount = Convert.ToDecimal(drFranchiseVoucherDetails["NetAmt"].ToString());
                            franchiseVoucherDetails.PNR = drFranchiseVoucherDetails["PNR"].ToString();
                            franchiseVoucherDetails.PassengerName = drFranchiseVoucherDetails["PassengerName"].ToString();
                            franchiseVoucherDetails.SeatNos = drFranchiseVoucherDetails["SeatNos"].ToString();
                            franchiseVoucherDetails.SeatCount = Convert.ToInt32(drFranchiseVoucherDetails["SeatCount"].ToString());
                            franchiseVoucherDetails.FromTo = drFranchiseVoucherDetails["FromTo"].ToString();
                            franchiseVoucherDetails.JDate = drFranchiseVoucherDetails["JourneyDate"].ToString();
                            franchiseVoucherDetails.JTime = drFranchiseVoucherDetails["JTime"].ToString();
                            franchiseVoucherDetails.BDate = drFranchiseVoucherDetails["BookingDate"].ToString();
                            franchiseVoucherDetails.BookingID = Convert.ToInt32(drFranchiseVoucherDetails["BookingID"].ToString());
                            franchiseVoucherDetails.GeneratedDate = Convert.ToDateTime(drFranchiseVoucherDetails["GeneratedDate"].ToString());
                            franchiseVoucherDetails.TransactionID = Convert.ToInt32(drFranchiseVoucherDetails["TransactionID"].ToString());
                            if (drFranchiseVoucherDetails["ItemID"].ToString() != "")
                                franchiseVoucherDetails.ItemID = Convert.ToInt32(drFranchiseVoucherDetails["ItemID"].ToString());
                            franchiseVoucherDetails.ItemName = drFranchiseVoucherDetails["Item"].ToString();
                            franchiseVoucherDetails.ClassID = drFranchiseVoucherDetails["ClassID"].ToString();
                            franchiseVoucherDetails.ClassName = drFranchiseVoucherDetails["classname"].ToString();
                            franchiseVoucherDetails.BranchDivisionID = drFranchiseVoucherDetails["BranchDivisionID"].ToString();
                            franchiseVoucherDetails.BranchDivisionName = drFranchiseVoucherDetails["BranchDivisionName"].ToString();
                            franchiseVoucherDetails.CompanyID = Convert.ToInt32(drFranchiseVoucherDetails["CompanyID"].ToString());
                            franchiseVoucherDetails.VoucherNo = drFranchiseVoucherDetails["VoucherNo"].ToString();
                            franchiseVoucherDetails.BookingStatus = drFranchiseVoucherDetails["BookingStatus"].ToString();
                            franchiseVoucherDetails.Prefix = drFranchiseVoucherDetails["Prefix"].ToString();

                            franchiseVoucherDetails.TotalFare = Convert.ToDecimal(drFranchiseVoucherDetails["TotalFare"].ToString());
                            franchiseVoucherDetails.RefundAmount = Convert.ToDecimal(drFranchiseVoucherDetails["RefundAmount"].ToString());

                            franchiseVoucherDetails.DivisionID = Convert.ToInt32(drFranchiseVoucherDetails["divisionid"].ToString());
                            franchiseVoucherDetails.BusMasterID = Convert.ToInt32(drFranchiseVoucherDetails["BusMasterId"].ToString());
                            franchiseVoucherDetails.Accsysid = Convert.ToString(drFranchiseVoucherDetails["accsysinvoiceid"].ToString());
                            franchiseVoucherDetails.docnumber = Convert.ToString(drFranchiseVoucherDetails["docnumber"].ToString());
                            franchiseVoucherDetails.docformattednumber = Convert.ToString(drFranchiseVoucherDetails["docformattednumber"].ToString());
                            franchiseVoucherDetails.VoucherDate = Convert.ToDateTime(drFranchiseVoucherDetails["VoucherDate"].ToString());

                            franchiseVoucherDetailsList.Add(franchiseVoucherDetails);




                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog("GetFranchiseVoucherUpdateDetailsFromCRS", " FranchiseID: " + franchiseVoucherDetails.FranchiseID + " VoucherNo: " + franchiseVoucherDetails.VoucherNo, ex.Message, true);
                        }

                    }

                }

                return franchiseVoucherDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public void DeleteFranchiseVouchers()
        {
            #region Franchise Invoice Deletion From QB

            DataSet ds = new DataSet();

            ds = GetFranchiseVoucherDeleteDetailsFromCRS();
            int count = 0;

            if (ds.Tables[0].Rows.Count > 0)
            {
                EntryCounter.GetInstance().ResetAllCount();
                string result = "";
                string InvD;
                foreach (DataRow drDeleteDetails in ds.Tables[0].Rows)
                {

                    string status = "";
                    string statusMessage = "";
                    Int32 iID = -1;
                    try
                    {
                        Invoice InvoiceData = new Invoice();
                        if (!drDeleteDetails["AccSysId"].ToString().Equals(""))
                        {
                            InvoiceData = Getinvoice(drDeleteDetails["AccSysId"].ToString());
                            //SalesItemLineDetail saleitem = new SalesItemLineDetail();

                            if (InvoiceData != null)
                            {
                               
                                InvoiceData = DeleteInvoice(drDeleteDetails["AccSysId"].ToString(), InvoiceData.SyncToken);
                                iID = Convert.ToInt32(InvoiceData.Id);
                                status = "Posted";
                                statusMessage = "Deleted";
                                EntryCounter.GetInstance().IncreaseQBCount(1);
                                InsertUpdateVoucherDeleteDetailsToCRS(Convert.ToInt32(drDeleteDetails["bookingid"].ToString()));

                                EntryCounter.GetInstance().IncreaseCRSCount(1);
                            }

                        }

                    }
                    catch (IdsException iex)
                    {
                        Logger.WriteQBExceptonDetailToLog(iex);
                    }
                    catch (Exception ex)
                    {
                        status = "Failed";
                        statusMessage = ex.Message;
                        Logger.WriteLog("DeleteFranchiseVouchers", "", ex.Message, true);
                    }

                }

                if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                {
                    string msg = "DeleteFranchiseVouchers:::Mismatch in No Of Entries Deleted in QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                    Email.SendMail(msg);
                }

                EntryCounter.GetInstance().ResetAllCount();
            }


            #endregion

            return;
        }


        private DataSet GetFranchiseVoucherDeleteDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", 1945, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetDataForDeletedFranchiseVouchers", CommandType.StoredProcedure, 0, ref strErr, "", false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;


                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion


        #region Franchise Voucher Payments

        public void PostFranchisePayments()
        {

            DataSet ds = null;
            DataSet dsoutput = null;

            try
            {

                dsoutput = GetFranchiseVoucherPaymentValidation(1945);
                if (dsoutput != null && dsoutput.Tables.Count > 0 && dsoutput.Tables[0].Rows.Count > 0)
                {
                    string ErrorMsg = "";
                    if (Convert.ToInt32(dsoutput.Tables[0].Rows[0]["Count"]) > 0)
                    {
                        string VoucherIds = dsoutput.Tables[0].Rows[0]["VoucherIds"].ToString();
                        string ErrMsg = dsoutput.Tables[0].Rows[0]["ErrMsg"].ToString();
                        Logger.WriteLog(ErrMsg + VoucherIds);
                        ErrorMsg = ErrMsg + VoucherIds;

                    }
                    if (Convert.ToInt32(dsoutput.Tables[0].Rows[1]["Count"]) > 0)
                    {
                        string VoucherIds = dsoutput.Tables[0].Rows[1]["VoucherIds"].ToString();
                        string ErrMsg = dsoutput.Tables[0].Rows[1]["ErrMsg"].ToString();
                        Logger.WriteLog(ErrMsg + VoucherIds);
                        ErrorMsg += " & " + "\n" + ErrMsg + VoucherIds;

                    }
                    if (Convert.ToInt32(dsoutput.Tables[0].Rows[2]["Count"]) > 0)
                    {
                        string VoucherIds = dsoutput.Tables[0].Rows[2]["VoucherIds"].ToString();
                        string ErrMsg = dsoutput.Tables[0].Rows[2]["ErrMsg"].ToString();
                        Logger.WriteLog(ErrMsg + VoucherIds);
                        ErrorMsg += " & " + "\n" + ErrMsg + VoucherIds;

                    }
                   
                    if (ErrorMsg != "")
                    {
                        Email.SendMail(ErrorMsg);
                    }




                }

                ds = InsertFranchiseVoucherPaymentReceiptsId(1945);


            }
            catch (Exception ex)
            {
                Logger.WriteLog("PostFranchisePayments", "GetFranchiseVoucherPaymentReceiptsId", ex.Message, true);
            }

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                Logger.WriteLog("PostFranchisePayments", "", "No Of FranchisePayments: " + ds.Tables[0].Rows.Count, true);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    int intFranchiseVoucherReceiptsID = 0;

                    intFranchiseVoucherReceiptsID = Convert.ToInt32(ds.Tables[0].Rows[i]["FranchiseVoucherReceiptsID"].ToString());

                   

                    // Payment 

                    List<FranchisePaymentDetails> franchisePaymentListPD = null;
                    try
                    {
                        franchisePaymentListPD = GetFranchisePaymentDetailsFromCRS(1945, intFranchiseVoucherReceiptsID);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("PostFranchisePayments", "GetAgentPaymentDetailsFromCRS", " FranchiseVoucherReceiptId: " + intFranchiseVoucherReceiptsID + " " + ex.Message, true);
                    }

                    if (franchisePaymentListPD != null && franchisePaymentListPD.Count > 0)
                    {
                        EntryCounter.GetInstance().ResetAllCount();

                        foreach (FranchisePaymentDetails fpDetails in franchisePaymentListPD)
                        {
                            Payment paymentPosted;
                            string status = "";
                            string statusMessage = "";
                            //string VoucherNoQB = "";
                            Int32 paymentID = -1;
                            try
                            {
                                string VoucherNoQB = "";
                              
                                VoucherNoQB = CreateVoucherNoForFranchisePaymentQB(fpDetails.CompanyID, Convert.ToInt32(fpDetails.BranchDivisionID), intFranchiseVoucherReceiptsID, fpDetails.PaymentDate);
                                
                                paymentPosted = PostPayment(true,fpDetails.FranchiseName, fpDetails.QuickBookCustomerID, fpDetails.inVoiceID, fpDetails.TotalAmount, fpDetails.PaymentType, fpDetails.PaymentTypeID, fpDetails.InstrumentNo, fpDetails.DepositeToName, fpDetails.DepositeToId, fpDetails.TxnDate, fpDetails.isHO, fpDetails.UserLedgerName, fpDetails.UserLedgerId, fpDetails.PaymentDrLedgerName, fpDetails.PaymentDrLedgerID, fpDetails.PaymentCrLedgerName, fpDetails.PaymentCrLedgerID, fpDetails.BranchDivisionID, fpDetails.DepositeToDivisionID, fpDetails.franchiseVoucherReceiptsID, fpDetails.BranchDivisionNameJE, fpDetails.BranchDivisionIDJE, fpDetails.BranchDivisionNameJE2, fpDetails.BranchDivisionIDJE2, VoucherNoQB);
                                paymentID = Convert.ToInt32(paymentPosted.Id);
                                status = "Posted";
                                statusMessage = "";

                                EntryCounter.GetInstance().IncreaseQBCount(1);

                                InsertFranchisePaymentPostingDetailsToCRS(paymentID, fpDetails.franchiseVoucherReceiptsID, status, statusMessage);

                                EntryCounter.GetInstance().IncreaseCRSCount(1);
                            }
                            catch (IdsException iex)
                            {
                                Logger.WriteQBExceptonDetailToLog(iex);
                            }
                            catch (Exception ex)
                            {
                                status = "Failed";
                                statusMessage = ex.Message;
                                Logger.WriteLog("AlertVersion 2 - PostFranchisePayments", "CreateVoucherNoForPaymentQB-InsertFranchisePaymentPostingDetailsToCRS", ex.Message, true);
                            }


                        }

                        if (!EntryCounter.GetInstance().IsQBCountEqualToCRSCount())
                        {
                            string msg = "AlertVersion 2 - PostFranchisePayments::Mismatch in No Of Entries Posted to QuickBook (" + EntryCounter.GetInstance().GetQBCount() + ") Vs Nos Of Entries Updated (" + EntryCounter.GetInstance().GetCRSCount() + ") in CRS.";
                            Email.SendMail(msg);
                        }

                        EntryCounter.GetInstance().ResetAllCount();
                    }


                    

                }
            }

      
            return;
        }


        private DataSet GetFranchiseVoucherPaymentValidation(int CompanyID)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
              
                DataSet dstOutPut = dal.ExecuteSelect("spGetValidationQuickBooksFranchiseReceiptsId", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private DataSet InsertFranchiseVoucherPaymentReceiptsId(int CompanyID)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
             
                DataSet dstOutPut = dal.ExecuteSelect("spInsertPendingQuickBooksFranchiseReceiptsId", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", true, "", true);

                return dstOutPut;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private List<FranchisePaymentDetails> GetFranchisePaymentDetailsFromCRS(int CompanyID, int franchisevoucherreceiptsid)
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_franchisevoucherreceiptsid", franchisevoucherreceiptsid, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("spGetFranchisePaymentForQuickBooksV2", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, false);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                List<FranchisePaymentDetails> franchisePaymentDetailsList = new List<FranchisePaymentDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow drFranchisePaymentDetails in dstOutPut.Tables[0].Rows)
                    {
                        int qbCustomerId;
                        qbCustomerId = 0;
                        if (!Int32.TryParse(drFranchisePaymentDetails["QuickBookCustomerID"].ToString(), out qbCustomerId))
                        {
                            Logger.WriteLog("GetFranchisePaymentDetailsFromCRS::Validation::Invalid QuickBook CustomerId for FranchiseVoucherReceiptId: " + drFranchisePaymentDetails["franchisevoucherreceiptsid"].ToString());
                        }
                        else
                        {
                            FranchisePaymentDetails franchisePaymentDetails = new FranchisePaymentDetails();
                            {
                                franchisePaymentDetails.TransactionID = Convert.ToInt32(drFranchisePaymentDetails["TransactionID"].ToString());
                                franchisePaymentDetails.inVoiceID = Convert.ToInt32(drFranchisePaymentDetails["AccSysID"].ToString());
                                franchisePaymentDetails.ActualAmount = Convert.ToDecimal(drFranchisePaymentDetails["PaidAmt"].ToString());
                                franchisePaymentDetails.TotalAmount = Convert.ToDecimal(drFranchisePaymentDetails["amountreceived"].ToString());
                                franchisePaymentDetails.PaymentDate = Convert.ToDateTime(drFranchisePaymentDetails["PaymentDate"].ToString());
                                franchisePaymentDetails.FranchiseName = drFranchisePaymentDetails["FranchiseName"].ToString();
                                franchisePaymentDetails.QuickBookCustomerID = Convert.ToInt32(drFranchisePaymentDetails["QuickBookCustomerID"].ToString());
                                franchisePaymentDetails.PaymentType = drFranchisePaymentDetails["PaymentType"].ToString();
                                franchisePaymentDetails.PaymentTypeID = Convert.ToInt32(drFranchisePaymentDetails["PaymentTypeID"].ToString());
                                franchisePaymentDetails.InstrumentNo = drFranchisePaymentDetails["InstrumentNo"].ToString();
                                franchisePaymentDetails.DepositeToName = drFranchisePaymentDetails["DepositeToName"].ToString();
                                franchisePaymentDetails.DepositeToId = Convert.ToInt32(drFranchisePaymentDetails["DepositeToID"].ToString());
                                franchisePaymentDetails.TxnDate = drFranchisePaymentDetails["PaymentDate"].ToString();
                                franchisePaymentDetails.franchiseVoucherReceiptsID = Convert.ToInt32(drFranchisePaymentDetails["franchisevoucherreceiptsid"].ToString());
                                franchisePaymentDetails.isHO = Convert.ToInt32(drFranchisePaymentDetails["isHO"].ToString());
                                //agentPaymentDetails.UserLedgerName = drAgentPaymentDetails["UserLedgerName"].ToString();
                                //agentPaymentDetails.UserLedgerId = Convert.ToInt32(drAgentPaymentDetails["UserLedgerID"].ToString());
                                franchisePaymentDetails.PaymentDrLedgerName = drFranchisePaymentDetails["PaymentDrLedgerName"].ToString();
                                franchisePaymentDetails.PaymentDrLedgerID = Convert.ToInt32(drFranchisePaymentDetails["PaymentDrLedgerID"].ToString());
                                franchisePaymentDetails.PaymentCrLedgerName = drFranchisePaymentDetails["PaymentCrLedgerName"].ToString();
                                franchisePaymentDetails.PaymentCrLedgerID = Convert.ToInt32(drFranchisePaymentDetails["PaymentCrLedgerID"].ToString());
                                franchisePaymentDetails.BranchDivisionID = Convert.ToInt32(drFranchisePaymentDetails["BranchDivision"].ToString());
                                franchisePaymentDetails.DepositeToDivisionID = Convert.ToInt32(drFranchisePaymentDetails["DepositeToDivision"].ToString());
                                franchisePaymentDetails.BranchDivisionIDJE = Convert.ToInt32(drFranchisePaymentDetails["BranchDivisionIDJE"].ToString());
                                franchisePaymentDetails.BranchDivisionNameJE = drFranchisePaymentDetails["BranchDivisionNameJE"].ToString();
                                franchisePaymentDetails.BranchDivisionIDJE2 = Convert.ToInt32(drFranchisePaymentDetails["BranchDivisionIDJE2"].ToString());
                                franchisePaymentDetails.BranchDivisionNameJE2 = drFranchisePaymentDetails["BranchDivisionNameJE2"].ToString();
                                franchisePaymentDetails.CompanyID = Convert.ToInt32(drFranchisePaymentDetails["CompanyID"].ToString());
                            }
                            franchisePaymentDetailsList.Add(franchisePaymentDetails);
                        }
                    }

                }

                return franchisePaymentDetailsList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void InsertFranchisePaymentPostingDetailsToCRS(int paymentID, int franchisevoucherreceiptsid, string status, string statusMessage)
        {
            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("PaymentID", paymentID, ParameterDirection.Input);
                dal.AddParameter("TransactionID", franchisevoucherreceiptsid, ParameterDirection.Input);
                dal.AddParameter("STATUS", status, 100, ParameterDirection.Input);
                dal.AddParameter("StatusMessage", statusMessage, 500, ParameterDirection.Input);

                dal.ExecuteDML("spInsertQuickBookFranchisePaymentStatus", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private String CreateVoucherNoForFranchisePaymentQB(Int32 CompanyID, Int32 DivisionID, Int32 FranchiseVoucherReceiptsID, DateTime PaymentDate)
        {

            try
            {
                string strErr = "";
                string strVoucherNo = "";
                DataSet ds = null;
                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_DivisionID", DivisionID, ParameterDirection.Input);
                dal.AddParameter("p_FranchiseVoucherReceiptsID", FranchiseVoucherReceiptsID, ParameterDirection.Input);
                dal.AddParameter("p_PaymentDateTime", PaymentDate, ParameterDirection.Input);

                ds = dal.ExecuteSelect("spCreateFranchiseVoucherNoforPaymentQB_V2", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", true);

                if (ds != null && ds.Tables.Count >= 1)
                {
                    strVoucherNo = ds.Tables[0].Rows[0]["VoucherNoQB"].ToString();
                }
                return strVoucherNo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DataSet GetFranchiseVoucherPaymentInvoiceDetails(int FranchsieVoucherReceiptsid)
        {
            try
            {
                string strErr = "";

                CRSDAL dal = new CRSDAL();

                dal.AddParameter("p_FranchiseVoucherReceiptsid", FranchsieVoucherReceiptsid, ParameterDirection.Input);
                DataSet dstOutPut = dal.ExecuteSelect("spGetFranchisePaymentInvoiceDetails", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false);

                return dstOutPut;
            }
            catch (Exception)
            {
                throw;
            }

        }





        #endregion


        #region CreditNote


        private List<AgentVoucherDetails> GetAgentCancellationDetailsFromCRS()
        {
            try
            {
                string strErr = "";
                string strResult = "";
                CRSDAL dal = new CRSDAL();
                DateTime fromDate = new DateTime(2016, 06, 13);
                DateTime toDate = new DateTime(2016, 06, 13);
                dal.AddParameter("p_CompanyID", 69, ParameterDirection.Input);
                dal.AddParameter("p_FromDate", fromDate, ParameterDirection.Input);
                dal.AddParameter("p_ToDate", toDate, ParameterDirection.Input);

                DataSet dstOutPut = dal.ExecuteSelect("Test_utk_spGetCancelledVoucherForQuickBooks_Konduskar", CommandType.StoredProcedure, 0, ref strErr, "p_ErrMessage", false, "", false, true);

                if (strErr != "")
                    strResult = strErr; // "Error:" + strErr;

                // List<AgentVoucherDetails> InvoiceNotPostedList = new List<AgentVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[0].Rows.Count > 0 && dstOutPut.Tables[0].Rows[0]["BookingIDs"].ToString() != "0")
                {

                    string BookingIDs = dstOutPut.Tables[0].Rows[0]["BookingIDs"].ToString();

                    Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation:: Agent Vouchers Not Posted : " + BookingIDs);
                    string msg = "GetAgentVoucherDetailsFromCRS::Validation:: Agent Vouchers Not Posted  : " + BookingIDs;
                    //Email.SendMail(msg);
                    //return InvoiceNotPostedList;

                }

                //else
                //{
                List<AgentVoucherDetails> agentVoucherDetailsList = new List<AgentVoucherDetails>();
                if (dstOutPut != null && dstOutPut.Tables.Count > 0 && dstOutPut.Tables[1].Rows.Count > 0)
                {

                    foreach (DataRow drAgentVoucherDetails in dstOutPut.Tables[1].Rows)
                    {
                        AgentVoucherDetails agentVoucherDetails = new AgentVoucherDetails();
                        try
                        {
                            Int32 qbCustomerId, divisionId, tripid, Merge;
                            qbCustomerId = 0;
                            divisionId = 0;
                            tripid = 0;
                            Merge = 0;
                            if (!Int32.TryParse(drAgentVoucherDetails["QuickBookCustomerID"].ToString(), out qbCustomerId))
                            {
                                Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid QuickBookCustomerId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                Email.SendMail(msg);

                            }
                            else if (!Int32.TryParse(drAgentVoucherDetails["divisionid"].ToString(), out divisionId))
                            {
                                Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid DivisionId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid DivisionId for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }

                            else if (!Int32.TryParse(drAgentVoucherDetails["ItemID"].ToString(), out tripid))
                            {
                                Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString());
                                string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }
                            else if (!Int32.TryParse(drAgentVoucherDetails["IsMerge"].ToString(), out Merge))
                            {
                                Logger.WriteLog("GetAgentVoucherDetailsFromCRS::Validation::Invalid Classid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " BusId: " + drAgentVoucherDetails["BusId"].ToString());
                                string msg = "GetAgentVoucherDetailsFromCRS::Validation::Invalid tripid for VoucherNo: " + drAgentVoucherDetails["VoucherNo"].ToString() + " AgentId: " + drAgentVoucherDetails["AgentID"].ToString();
                                Email.SendMail(msg);
                            }
                            else
                            {
                                agentVoucherDetails.AgentID = Convert.ToInt32(drAgentVoucherDetails["AgentID"].ToString());
                                agentVoucherDetails.AgentName = drAgentVoucherDetails["AgentName"].ToString();
                                agentVoucherDetails.QuickBookCustomerID = Convert.ToInt32(drAgentVoucherDetails["QuickBookCustomerID"].ToString());
                                agentVoucherDetails.FromCityName = drAgentVoucherDetails["FromCityName"].ToString();
                                agentVoucherDetails.ToCityName = drAgentVoucherDetails["ToCityName"].ToString();
                                agentVoucherDetails.RouteFromCityName = drAgentVoucherDetails["RouteFromCityName"].ToString();
                                agentVoucherDetails.RouteToCityName = drAgentVoucherDetails["RouteToCityName"].ToString();
                                agentVoucherDetails.BusNumber = drAgentVoucherDetails["BusNumber"].ToString();
                                agentVoucherDetails.BusType = drAgentVoucherDetails["ChartName"].ToString();
                                agentVoucherDetails.AgentPhone1 = drAgentVoucherDetails["ContactNo1"].ToString();
                                agentVoucherDetails.AgentPhone2 = drAgentVoucherDetails["ContactNo2"].ToString();
                                agentVoucherDetails.Amount = Convert.ToDecimal(drAgentVoucherDetails["NetAmt"].ToString());
                                agentVoucherDetails.PNR = drAgentVoucherDetails["PNR"].ToString();
                                agentVoucherDetails.PassengerName = drAgentVoucherDetails["PassengerName"].ToString();
                                agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                agentVoucherDetails.FromTo = drAgentVoucherDetails["FromTo"].ToString();
                                agentVoucherDetails.JDate = drAgentVoucherDetails["JourneyDate"].ToString();
                                agentVoucherDetails.JTime = drAgentVoucherDetails["JTime"].ToString();
                                agentVoucherDetails.BDate = drAgentVoucherDetails["BookingDate"].ToString();
                                agentVoucherDetails.BookingID = Convert.ToInt32(drAgentVoucherDetails["BookingID"].ToString());
                                agentVoucherDetails.GeneratedDate = Convert.ToDateTime(drAgentVoucherDetails["GeneratedDate"].ToString());
                                agentVoucherDetails.TransactionID = Convert.ToInt32(drAgentVoucherDetails["TransactionID"].ToString());
                                if (drAgentVoucherDetails["ItemID"].ToString() != "")
                                    agentVoucherDetails.ItemID = Convert.ToInt32(drAgentVoucherDetails["ItemID"].ToString());
                                agentVoucherDetails.ItemName = drAgentVoucherDetails["Item"].ToString();
                                agentVoucherDetails.ClassID = drAgentVoucherDetails["ClassID"].ToString();
                                agentVoucherDetails.ClassName = drAgentVoucherDetails["classname"].ToString();
                                agentVoucherDetails.BranchDivisionID = drAgentVoucherDetails["BranchDivisionID"].ToString();
                                agentVoucherDetails.BranchDivisionName = drAgentVoucherDetails["BranchDivisionName"].ToString();
                                agentVoucherDetails.CompanyID = Convert.ToInt32(drAgentVoucherDetails["CompanyID"].ToString());
                                agentVoucherDetails.VoucherNo = drAgentVoucherDetails["VoucherNo"].ToString();
                                agentVoucherDetails.BookingStatus = drAgentVoucherDetails["BookingStatus"].ToString();
                                agentVoucherDetails.Prefix = drAgentVoucherDetails["Prefix"].ToString();

                                agentVoucherDetails.TotalFare = Convert.ToDecimal(drAgentVoucherDetails["TotalFare"].ToString());
                                agentVoucherDetails.RefundAmount = Convert.ToDecimal(drAgentVoucherDetails["RefundAmount"].ToString());

                                // agentVoucherDetails.DivisionID = Convert.ToInt32(drAgentVoucherDetails["divisionid"].ToString());
                                agentVoucherDetails.BusMasterID = Convert.ToInt32(drAgentVoucherDetails["BusMasterId"].ToString());
                                agentVoucherDetails.VoucherDate = Convert.ToDateTime(drAgentVoucherDetails["VoucherDate"].ToString());
                                agentVoucherDetails.PickUpName = drAgentVoucherDetails["pickupname"].ToString();
                                agentVoucherDetails.DropOffName = drAgentVoucherDetails["dropoffname"].ToString();
                                agentVoucherDetails.SeatCount = Convert.ToInt32(drAgentVoucherDetails["SeatCount"].ToString());
                                agentVoucherDetails.SeatNos = drAgentVoucherDetails["SeatNos"].ToString();
                                agentVoucherDetails.GST = Convert.ToDecimal(drAgentVoucherDetails["GST"].ToString());
                                agentVoucherDetails.AgentComm = Convert.ToDecimal(drAgentVoucherDetails["AgentComm"].ToString());
                                agentVoucherDetails.GSTType = drAgentVoucherDetails["GSTType"].ToString();
                                agentVoucherDetails.invoiceID = Convert.ToInt32(drAgentVoucherDetails["AccSysId"].ToString());

                                agentVoucherDetailsList.Add(agentVoucherDetails);
                            }

                        }

                        catch (Exception ex)
                        {
                            Logger.WriteLog("GetAgentVoucherDetailsFromCRS", " AgentId: " + agentVoucherDetails.AgentID + " VoucherNo: " + agentVoucherDetails.VoucherNo, ex.Message, true);
                        }

                    }

                }


                return agentVoucherDetailsList;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private CreditMemo PostCredtNote(string action, int AgentID, string invoiceid, string synctoken, string AgentName, int QuickBookCustomerID, string FromCityName, string ToCityName, string RouteFromCityName, string RouteToCityName, string BusNumber, string AgentPhone1, string AgentPhone2, decimal Amount, string PNR, string PassengerName, string SeatNos, int SeatCount, string FromTo, string JDate, string JTime, string BDate, int BookingID, DateTime VoucherDate, int TransactionID, string BusType, string TripCode, string ItemName, int intItemID, string ClassName, string ClassID, string DivisionName, string DivisionID, string VoucherNo, string BookingStatus, string Prefix, decimal TotalFare, Decimal RefundAmount, string PickUpName, string DropOffName, decimal GST, decimal AgentComm, string GSTType)

        {
            try
            {

                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                decimal Lineamount = 0;
                /* Invoice Begin */
                //customer name, date, payment method, deposit to, service, description, Qty, Rate, Amout , Tax

                var credit = new CreditMemo();
                {
                    credit.TxnDate = VoucherDate;
                    credit.TxnDateSpecified = true;


                    credit.CustomerRef = new ReferenceType() { name = AgentName, Value = QuickBookCustomerID.ToString() };
                    credit.InvoiceRef = new ReferenceType() { name = "InvoiceRef", Value = invoiceid };
                    //credit.TransactionLocationType = "24";

                    //invoice.SalesTermRef = new ReferenceType() { name = "Due on receipt", Value = "9" };

                    if (DivisionName != "")
                        credit.DepartmentRef = new ReferenceType() { name = DivisionName, Value = DivisionID }; //agent area

                    List<CustomField> customFieldList = new List<CustomField>();
                    var pickupdrop = new CustomField();
                    {
                        pickupdrop.DefinitionId = "1";
                        pickupdrop.Name = "PickUp-Drop";
                        pickupdrop.AnyIntuitObject = FromCityName + "-" + ToCityName;
                        customFieldList.Add(pickupdrop);
                    };

                    var JourneyDate = new CustomField();
                    {
                        JourneyDate.DefinitionId = "2";
                        JourneyDate.Name = "Journey Date";
                        JourneyDate.AnyIntuitObject = JDate;
                        customFieldList.Add(JourneyDate);
                    }

                    CustomField PhoneNumber = new CustomField();
                    {
                        PhoneNumber.DefinitionId = "3";
                        PhoneNumber.Name = "Phone Number";
                        PhoneNumber.AnyIntuitObject = AgentPhone1 + "," + AgentPhone2;
                        customFieldList.Add(PhoneNumber);
                    }

                    credit.CustomField = customFieldList.ToArray();

                    List<Line> CreditLineList = new List<Line>();

                   
                    {
                       // if (GSTType == "CGST-SGST")
                        {


                            SalesItemLineDetail saleItemDetail = new SalesItemLineDetail();
                            {
                                saleItemDetail.Qty = new Decimal(SeatCount);
                                saleItemDetail.QtySpecified = true;

                                saleItemDetail.AnyIntuitObject = RefundAmount / SeatCount; // Amount / SeatCount; // 2500m;


                                saleItemDetail.ItemElementName = ItemChoiceType.UnitPrice;



                                saleItemDetail.ItemRef = new ReferenceType() { name = ItemName, Value = intItemID.ToString() };

                                if (ClassName != "")
                                    saleItemDetail.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };
                                if (GSTType == "CGST-SGST")
                                {
                                    saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "24" };

                                }
                                else if (GSTType == "IGST")
                                {
                                    saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "22" };
                                }
                                else
                                {
                                    saleItemDetail.TaxCodeRef = new ReferenceType() { Value = "24" };
                                }
                                

                            }
                            string strDesc = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                                "Seats : " + SeatNos + "\n" +
                                "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                                "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                                "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                                + ",\nPickup Name: " + PickUpName + ",\nDropOff Name: " + DropOffName;

                            if (BookingStatus == "C")
                                strDesc = "Cancelled - " + strDesc;

                            Line CreditLine = new Line();
                            {
                                CreditLine.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                                CreditLine.DetailTypeSpecified = true;
                            }



                            CreditLine.Amount = RefundAmount;// 1000; // Amount;
                                                       // Lineamount += (bkgDetails.Fare - (Commissionperseatwise + discountperseatwise) + diffamt) * bkgDetails.SeatCount;


                            CreditLine.AmountSpecified = true;
                            CreditLine.AnyIntuitObject = saleItemDetail;
                            CreditLine.Description = strDesc;


                            CreditLineList.Add(CreditLine);


                            ///////////////////////////////////// For Cancellation Charge  Tag /////////////////////////////////////
                            //SalesItemLineDetail saleItemDetail1 = new SalesItemLineDetail();
                            //{
                            //    saleItemDetail1.Qty = new Decimal(SeatCount);
                            //    saleItemDetail1.QtySpecified = true;

                            //    saleItemDetail1.AnyIntuitObject = AgentComm / SeatCount; // Amount / SeatCount; // 2500m;


                            //    saleItemDetail1.ItemElementName = ItemChoiceType.UnitPrice;



                            //    saleItemDetail1.ItemRef = new ReferenceType() { name = "Commision", Value = "330" };

                            //    //if (ClassName != "")
                            //    //    saleItemDetail1.ClassRef = new ReferenceType() { name = ClassName, Value = ClassID };
                            //    saleItemDetail1.TaxCodeRef = new ReferenceType() { Value = "24" };

                            //}
                            //string strDesc1 = "PNR : " + BookingID + ", Passenger Name : " + PassengerName + "\n" +
                            //    "Seats : " + SeatNos + "\n" +
                            //    "Trip : " + FromCityName + "-" + ToCityName + ", Route : " + RouteFromCityName + "-" + RouteToCityName + "\n" +
                            //    "Bus Code : " + TripCode + ", Bus Type : " + BusType + "\n" +
                            //    "Journey DateTime : " + JDate + " " + JTime + ",\nBooking DateTime : " + BDate
                            //    + ",\nPickup Name: " + PickUpName + ",\nDropOff Name: " + DropOffName;

                            //if (BookingStatus == "C")
                            //    strDesc = "Cancelled - " + strDesc1;

                            //Line CreditLine1 = new Line();
                            //{
                            //    CreditLine1.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                            //    CreditLine1.DetailTypeSpecified = true;
                            //}



                            //CreditLine1.Amount = AgentComm;


                            //CreditLine1.AmountSpecified = true;
                            //CreditLine1.AnyIntuitObject = saleItemDetail1;
                            //CreditLine1.Description = strDesc1;


                            //CreditLineList.Add(CreditLine1);


                        }
                       
                    }


                    credit.TotalAmt = RefundAmount;
                    credit.TotalAmtSpecified = true;


                    //TxnTaxDetail
                    TxnTaxDetail txnTaxdetail = new TxnTaxDetail();

                    txnTaxdetail.TotalTaxSpecified = true;
                  

                    credit.GlobalTaxCalculation = GlobalTaxCalculationEnum.TaxExcluded;
                    credit.GlobalTaxCalculationSpecified = true;


                    credit.Line = CreditLineList.ToArray();

                    credit.DocNumber = VoucherNo;
                    string strNo = "[Voucher No : " + VoucherNo + "]";

                    if (BookingStatus == "C")
                        credit.PrivateNote = strNo + ", Cancelled - PNR : " + BookingID + " -- Automaticaly posted from CRS";
                    else
                        credit.PrivateNote = strNo + ", PNR : " + BookingID + " -- Automaticaly posted from CRS";


                    if (action.Equals("Update"))
                    {
                        credit.Id = invoiceid;
                        credit.SyncToken = synctoken;
                    }
                }
                CreditMemo postedCreditNote = null;

               
                postedCreditNote = service.Add(credit);
                


                return postedCreditNote;
                /* Invoice End*/

            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }



        private Payment PostPaymentAfterCreditNote(bool isfranchise, string customerDisplayName, int quickbookCustomerID, int invoiceID, decimal amount, string paymentType, int paymentTypeid, string InstrumentNo, string DepositeToName, int DepositeToId, string TxnDate, int isHO, string UserLedgerName, int UserLedgerId, string PaymentDrLedgerName, int PaymentDrLedgerID, string PaymentCrLedgerName, int PaymentCrLedgerID, int BranchDivisionID, int DepositeToDivisionID, int AgentVoucherReceiptsID, string BranchDivisionNameJE, int BranchDivisionIDJE, string BranchDivisionNameJE2, int BranchDivisionIDJE2, string VoucherNoQB,int CreditNoteID)
        {
            try
            {
                ServiceContext context = QuickBookConnection.InitializeServiceContextQbo();
                var service = new DataService(context);

                Payment postedPayment = new Payment();

                // 1 Payment Entry
                #region IS Normal Branch

                Payment payment = new Payment();
                {
                    payment.TotalAmt = 0;
                    payment.TotalAmtSpecified = true;
                    payment.UnappliedAmt = 0;
                    payment.UnappliedAmtSpecified = true;
                    payment.TxnDateSpecified = true;

                    payment.CustomerRef = new ReferenceType() { name = customerDisplayName, Value = quickbookCustomerID.ToString(), };

                    //payment.DepositToAccountRef = new ReferenceType() { name = UserLedgerName, Value = UserLedgerId.ToString(), type = "Bank" };
                    //payment.DepositToAccountRef = new ReferenceType() { name = DepositeToName, Value = DepositeToId.ToString(), type = "Bank" };
                }
                //if (paymentType.Equals("Cheque"))
                //{
                //    payment.PaymentMethodRef = new ReferenceType() { name = "Cheque", Value = "9" };
                //}
                //else if (paymentType.Equals("Net Banking"))
                //{
                //    payment.PaymentMethodRef = new ReferenceType() { name = "Net Banking", Value = "11" };
                //}
                //else
                //{
                //    payment.PaymentMethodRef = new ReferenceType() { name = "Cash", Value = "8" };
                //}

                //payment.PaymentTypeSpecified = true;
                //payment.PaymentRefNum = InstrumentNo;
                payment.TxnDate = Convert.ToDateTime(TxnDate);


                #region Payment Posting to Invoice
                DataSet dsInvoiceList = null;

                //if (isfranchise)
                //{
                //    dsInvoiceList = GetFranchiseVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);
                //}
                //else
                //{
                //    dsInvoiceList = GetVoucherPaymentInvoiceDetails(AgentVoucherReceiptsID);
                //}



                //if (dsInvoiceList != null && dsInvoiceList.Tables.Count > 0)
                {
                    List<Line> paymentLines = new List<Line>();

                    //for (int i = 0; i < dsInvoiceList.Tables[0].Rows.Count; i++)
                    {
                        Line paymentLineOne = new Line();

                        List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                        LinkedTxn linkedTxn1 = new LinkedTxn();
                        {
                            linkedTxn1.TxnType = "Invoice";
                            linkedTxn1.TxnId = invoiceID.ToString();
                        } // invoiceID.ToString();
                        linkedPaymentTxns.Add(linkedTxn1);
                        paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                        paymentLineOne.Amount = amount; //amount;
                        paymentLineOne.AmountSpecified = true;

                        paymentLines.Add(paymentLineOne);
                    }

                    //if (dsInvoiceList.Tables.Count > 1)
                    {
                        //for (int i = 0; i < dsInvoiceList.Tables[1].Rows.Count; i++)
                        {
                            Line paymentLineOne = new Line();

                            List<LinkedTxn> linkedPaymentTxns = new List<LinkedTxn>();
                            LinkedTxn linkedTxn1 = new LinkedTxn();
                            {
                                linkedTxn1.TxnType = "CreditMemo";
                                linkedTxn1.TxnId = CreditNoteID.ToString();
                            }
                            linkedPaymentTxns.Add(linkedTxn1);
                            paymentLineOne.LinkedTxn = linkedPaymentTxns.ToArray();
                            paymentLineOne.Amount = amount;
                            paymentLineOne.AmountSpecified = true;

                            paymentLines.Add(paymentLineOne);
                        }
                    }
                    payment.Line = paymentLines.ToArray();
                }

                payment.PrivateNote = "[Voucher No : " + "Test" + "] -- Automaticaly posted from CRS";
                #endregion

                postedPayment = service.Add(payment);

             

                #endregion

                return postedPayment;

                /* Payment Entry - End*/
            }
            catch (Intuit.Ipp.Exception.IdsException ex)
            {
                throw ex;
            }
        }


        private void InsertUpdateCreditNotePostingDetailsToCRS(Int32 AgentID, Int32 CompanyID, Int32 BookingID, Int32 AccSysID, string docnumber, string docformattednumber, int classid, int divisionid,Int32 PaymentAccsysid,DateTime CreditNoteDate,decimal CreditAmount)
        {

            try
            {
                string strErr = "";
                CRSDAL dal = new CRSDAL();
                dal.AddParameter("p_AgentID", AgentID, ParameterDirection.Input);
                dal.AddParameter("p_CompanyID", CompanyID, ParameterDirection.Input);
                dal.AddParameter("p_BookingID", BookingID, ParameterDirection.Input);
                dal.AddParameter("p_AccSysID", AccSysID, ParameterDirection.Input);
                dal.AddParameter("p_docnumber", docnumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_docformattednumber", "CNCRS" + docformattednumber, 100, ParameterDirection.Input);
                dal.AddParameter("p_classid", classid, ParameterDirection.Input);
                dal.AddParameter("p_divisionid", divisionid, ParameterDirection.Input);
                dal.AddParameter("p_PaymentAccsysid", PaymentAccsysid, ParameterDirection.Input);
                dal.AddParameter("p_CreditNoteDate", CreditNoteDate, ParameterDirection.Input);
                dal.AddParameter("p_CreditAmount", CreditAmount, ParameterDirection.Input);

                dal.ExecuteDML("spInsertUpdateQuickBookCreditNote", CommandType.StoredProcedure, 0, ref strErr);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

    }


}

