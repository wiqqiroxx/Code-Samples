using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonClassLib.BusinessLogic.Reports;
using CTCT.Components.Contacts;
using MailChimp;
using MailChimp.Lists;
using System.Configuration;
using MailChimp.Helper;
using System.Data;
using PayPal.PayPalAPIInterfaceService.Model;
using StCore.Lib.Logging;
using Contact = MailChimp.Helper.Contact;

namespace CommonClassLib.Integrations.MailChimpAndConstantContact.MailChimp
{
    public class MailChimpLogic
    {
        private MailChimpManager mc = new MailChimpManager(SiteSettings.MailChimpApiKey());

        public MailChimpLogic()
        {

        }

//This method returns the existings segments within a list
        public List<StaticSegmentResult> GetSegmentsForList(string listId)
        {
            return mc.GetStaticSegmentsForList(listId);
        }

//Following method returns the existing members with in a list via a Generic list of tuples        
 public List<Tuple<string, string, string, string, string>> GetDataForAList(string listid)
        {
            var result = new List<Tuple<string, string, string, string, string>>();
            //Gets Members for a list  ASC DESC also set here
            var allMembersOfList = mc.GetAllMembersForList(listid, "subscribed", 0, 25, "FNAME]");
            //  Write out each member's email address and name
            foreach (var member in allMembersOfList.Data)
            {

                MergeVar mvar = member.MemberMergeInfo;
                List<object> lstFnameLname = mvar.Values.ToList();
                string firstName = lstFnameLname[0] == null ? "" : lstFnameLname[0].ToString();
                string lastName = lstFnameLname[1] == null ? "" : lstFnameLname[1].ToString();

                if (allMembersOfList.Data.Count != 0)
                {
                    result.Add(Tuple.Create(firstName, lastName, member.Email, member.ListName, member.Status));
                }
            }
            result.Sort();
            return result;
        }


//      public DataTable GetTestEmails()
//       {
//
//          DataTable dt = new DataTable("Contacts");
//        	dt.Columns.Add("FirstName");
//     		dt.Columns.Add("LastName");
//            	dt.Columns.Add("Email");

//            dt.Rows.Add("Indocin", "David", "id122avid@hotmail.com");
//            dt.Rows.Add("Enebrel", "Sam", "e1sa22m@google.com");
//            dt.Rows.Add("Hydralazine", "Chri222s1toff", "hchristoff@hotmail.com");
//            dt.Rows.Add("Combivent", "Janet", "cjane122t@google.com");
//            dt.Rows.Add("Dilantin", "Melanie", "dme1222lanie@hotmail.com");

//            dt.Rows.Add("Evelyn", "Hunter", "ehun12222ter0@themeforest.net");
//            dt.Rows.Add("Craig", "Little", "clit122tle1@t.co");
//            dt.Rows.Add("Jerry", "Burke", "jbur22k1e2@aol.com");
//            dt.Rows.Add("William", "Burns", "wbu22r1ns3@cyberchimps.com");
//            dt.Rows.Add("Brenda", "Burton", "bbur221ton4@washington.edu");


//            return dt;
//           }

   
//Gets data for all the lists 
 public List<Tuple<string, string, string, string, string>> GetAllListsData(string apikey)
        {

            var result = new List<Tuple<string, string, string, string, string>>();
            var alllists = mc.GetLists();  //This will grab all the lists

            foreach (var list in alllists.Data)  //loops through lists 
            {


                //Gets Members for a list  ASC DESC also set here
                var allMembersOfList = mc.GetAllMembersForList(list.Id, "subscribed", 0, 25, "FNAME]");


                //  Write out each member's email address and name
                foreach (var member in allMembersOfList.Data)
                {

                    MergeVar mvar = member.MemberMergeInfo;
                    var lstFnameLname = mvar.Values.ToList();

                    string firstName = lstFnameLname[0] == null ? "" : lstFnameLname[0].ToString();
                    string lastName = lstFnameLname[1] == null ? "" : lstFnameLname[1].ToString();

                    if (allMembersOfList.Data.Count != 0)
                    {
                        result.Add(Tuple.Create(firstName, lastName, member.Email, member.ListName, member.Status));

                    }

                }

            }
            result.Sort();

            return result;
        }

        public ListResult GetAllLists()
        {
            return mc.GetLists();


        }





        //}


// Add members to a segment with in a list and return the success count.
        public int AddMembersToStaticSegment(string listId, int segmentId, List<EmailParameter> lstEmails)
        {
            int emailsAdded;
            try
            {

                var ssmar = mc.AddStaticSegmentMembers(listId, segmentId, lstEmails);
                emailsAdded = ssmar.successCount;
            }
            catch (Exception ex)
            {
                StLogger.Error("Add Member to Static Segment Failed", ex);
                return 0;
            }
            return emailsAdded;

        }

//Add new dynamic segments to a list which auto populates the data when a new contact is added
        public int AddNewDynamicSegmentToList(string listId, string segName)
        {
            int newSegmentId;
            try
            {

                var segOptions = new AddCampaignSegmentOptions();
                segOptions.Name = segName;
                segOptions.SegmentType = "static";
                SegmentAddResult sar = mc.AddSegment(listId, segOptions);
                newSegmentId = sar.NewSegmentID;
            }
            catch (Exception ex)
            {
                // TODO LOG
                StLogger.Error("AddNewDynamicSegmentToList Failed", ex);
                return -1;
            }
            return newSegmentId;
        }


//Add members to a static segment in which further members will be added forcefully
        public int AddNewStaticSegmentToList(string listId, string segName)
        {
            int newSegmentId;
            try
            {


                StaticSegmentAddResult sar = mc.AddStaticSegment(listId, segName);
                newSegmentId = sar.NewStaticSegmentID;
            }
            catch (Exception ex)
            {
                StLogger.Error("AddNewStaticSegmentToList Failed", ex);
                return -1;
            }
            return newSegmentId;
        }


//Make list of type UserContact(Class) from Datatable
        public List<UserContact> MakeListOfContactFromDatatable(DataTable dt)
        {

            var lstContact = new List<UserContact>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                lstContact.Add(new UserContact((string)row[0], (string)row[1], (string)row[2]));
            }
            return lstContact;

        }

//Create a list of UserContact class type from a Hashtable
        public List<UserContact> MakeListOfContactFromReportFunctionsResponse(Hashtable lstReponse)
        {
            var lstContact = new List<UserContact>(lstReponse.Count);
            //DictionaryEntry entry in hashtable
            foreach (DictionaryEntry response in lstReponse)
            {
               // lstContact.Add(new UserContact((string)row[0], (string)row[1], (string)row[2]));
               // var cntct = new Contact();
              //  var emllst = new List<EmailAddress>();

                var FirstName = response.Key.ToString().Split(' ')[0];
                var LastName = response.Key.ToString().Split(' ')[1];


                // emllst.Add(new EmailAddress(reMakeListOfContactFromReportFunctionsResponsesponse.Value.ToString()));

                    lstContact.Add(new UserContact((FirstName),LastName, response.Value.ToString()));
               

            }

            return lstContact;
        }

//Create a generic list of UserContact from a different type of list
        public List<UserContact> MakeListOfContactFromReportData(List<OrderListReportResponse> lstReponse)
        {

            var lstContact = new List<UserContact>(lstReponse.Count);
            foreach (var response in lstReponse)
            {
                var nameArray = response.BuyerFullName.Split(' ');
                
                string firstName = nameArray[0]!=string.Empty?nameArray[0]:"-";
                string lastName = string.Empty;
                if(nameArray.Length==2)
                { lastName = nameArray[1];}

                if(!string.IsNullOrEmpty(response.BuyerEmail))
                {
                    lstContact.Add(new UserContact(firstName, lastName, response.BuyerEmail));
                }
                
            }

            return lstContact;

        }

//Make list of EmailParameters(Mailchimp class) from Datatable     
   public List<EmailParameter> MakeListOfEmailsFromDatatable(DataTable dt)
        {

            var lstEmail = new List<EmailParameter>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                lstEmail.Add(new EmailParameter() { Email = (string)row[0] }); //herenew Email { Email = contact.Email }
            }
            return lstEmail;

        }

//Subscribe a member to a list in a segment and return the success count
        public int ListSubscribeMember(List<UserContact> lstContact, string listId, int segmentId)
        {
            int count = 0;
            var emailParam = new List<EmailParameter>();
            if (listId != string.Empty)
            {
                // MailChimpManager mc = new MailChimpManager(SiteSettings.MailChimpApiKey());

                foreach (var contact in lstContact)
                {

                    var mergeVars = new CustomMergeVars
                    {
                        FirstName = contact.FirstName,
                        LastName = contact.Lastname
                    };

                    var match = mc.SearchMembers(contact.Email, listId: listId);


                    if (match.ExactMatches.Members.Count == 0)
                    {
                        //if not subscribed so subscribe them
                        var emResul = mc.Subscribe(listId: listId, emailParam: new EmailParameter() { Email = contact.Email }, mergeVars: mergeVars, doubleOptIn: false);
                        emailParam.Add(emResul);

                        count++;
                    }


                }

            }
            var segmentCount = AddMembersToStaticSegment(listId, segmentId, emailParam);
            return count;


        }



    }
}
