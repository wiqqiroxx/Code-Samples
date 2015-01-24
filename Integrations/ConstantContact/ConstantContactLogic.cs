using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonClassLib.BusinessLogic.Reports;
using CTCT;
using CTCT.Components;
using CTCT.Components.Contacts;
using CTCT.Components.EmailCampaigns;
using CTCT.Exceptions;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using MailChimp.Helper;
using StCore.Lib.Logging;
using Telerik.OpenAccess;
using TS.Common.BusinessLogic;
using Contact = CTCT.Components.Contacts.Contact;


namespace CommonClassLib.Integrations
{
    public class ConstantContactLogic
    {
        ConstantContact constantContact = new ConstantContact(SiteSettings.ConstantContactApiKey(), "30c666e0-138d-42d2-ac2b-6b487f6cfb89");
        public ConstantContactLogic()
        {
            //
            // TODO: Add constructor logic here
            //var pc = new ParticipantsController();
            //var tbl = pc.GetContactsPerOpenTicketSectionId("12052");
            //
        }


//This function takes a Datatable as input and converts it to a generic list of Contact class type and returns it
        public List<Contact> MakeListOfContactFromDatatable(DataTable dt)
        {

            var lstContact = new List<Contact>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                var cntct = new Contact();
                var emllst = new List<EmailAddress>();
                emllst.Add(new EmailAddress((string)row[2]));
                cntct.FirstName = (string)row[0];
                cntct.LastName = (string)row[1];
                cntct.EmailAddresses = (emllst);
                lstContact.Add(cntct);
            }
            return lstContact;

        }


//This function is used to add a new list to the existing contact lists
        public ContactList AddList(string listName)
        {
            //OAuth.GetAccessTokenByCode(HttpContext.Current, "ok");
            //30c666e0-138d-42d2-ac2b-6b487f6cfb89
            var ctl = new ContactList();
            ctl.Name = listName;
            ctl.Status = "ACTIVE";
            return constantContact.AddList(ctl);


        }

//This method takes a hashtable as input and returns a generic list of Contact type
        public List<Contact> MakeListOfContactFromReportFunctionsResponse(Hashtable lstReponse)
        {
            var lstContact = new List<Contact>(lstReponse.Count);
            //DictionaryEntry entry in hashtable
            foreach (DictionaryEntry response in lstReponse)
            {
               
                var cntct = new Contact();
                var emllst = new List<EmailAddress>();

                cntct.FirstName = response.Key.ToString().Split(' ')[0];
                cntct.LastName = response.Key.ToString().Split(' ')[1];


                emllst.Add(new EmailAddress(response.Value.ToString()));
                cntct.EmailAddresses = emllst;
                if (emllst.Count != 0)
                {
                    lstContact.Add(cntct);
                }

            }

            return lstContact;
        }

//This method takes a generic list as input and converts it to another list of type Contact
        public List<Contact> MakeListOfContactFromReportData(List<OrderListReportResponse> lstReponse)
        {

            var lstContact = new List<Contact>(lstReponse.Count);
            foreach (var response in lstReponse)
            {

                var cntct = new Contact();
                var emllst = new List<EmailAddress>();
                var nameArray = response.BuyerFullName.Split(' ');
                string firstName = nameArray[0] != string.Empty ? nameArray[0] : "-";
                string lastName = string.Empty;
                if (nameArray.Length == 2)
                {
                    lastName = nameArray[1];
                }

                
                emllst.Add(new EmailAddress(response.BuyerEmail));

                if (emllst.Count!=0)
                {
                    lstContact.Add(cntct);
                }

            }

            return lstContact;

        }


        public IList<ContactList> GetAllList()
        {
            return constantContact.GetLists(System.DateTime.Now);


        }


//This method subscribes a list of members to a Constant Contact list and returns the successfull number of people added
        public int ListSubscribeMember(List<Contact> lstContact, string listId)
        {
            int count = 0;
            if (listId != string.Empty)
            {
               // ConstantContact constantContact = new ConstantContact(SiteSettings.ConstantContactApiKey(), "30c666e0-138d-42d2-ac2b-6b487f6cfb89");
                var list = new List<ContactList>();
                
                var ctl = new ContactList();
                ctl.Id = listId;
                ctl.Status = "ACTIVE";

                list.Add(ctl);
                foreach (var contact in lstContact)
                {
                    var cntctemail = contact.EmailAddresses;
              if(cntctemail.Count>0)
              { 
                    bool contains = false;
                    var match = constantContact.GetContacts(DateTime.Now.AddYears(-10));
                    try
                    {
                        foreach (var mtch in match.Results)
                        {
                            var eml = mtch.EmailAddresses;
                            contains =
                                Regex.Match(eml[0].EmailAddr, cntctemail[0].EmailAddr,
                                    RegexOptions.IgnoreCase).Success;
                            if (contains == true)
                           break;

                        }

                    }
                    catch (CTCT.Exceptions.CtctException ex)
                    {

                        StLogger.Error("Email Already Exists", ex);
                    }
                    if (!contains)
                    {
                        contact.Lists = list;

                        try
                        {
                           
                            constantContact.AddContact(contact, false);
                            count++;
                        }

                        catch (CTCT.Exceptions.CtctException ex)
                        {
                            if (ex.Message.Contains("http.status.email_address.conflict:Email address"))
                            {
                                //wasntadded
                                StLogger.Error("Email Already Exists", ex);
                            }
                        }
                    }

                }
                }
            }
            return count;


        }
    }
}