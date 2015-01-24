using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonClassLib.BusinessLogic.Reports;
using CommonClassLib.Integrations.MailChimpAndConstantContact.MailChimp;
using StCore.ORM;
using TS.Common.BusinessLogic;
using TS.Common.Objects;

namespace DemoSiteApp.AdminPanel.Reports.Popups
{
    public partial class MailChimpExport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {
                     //MailChimpManager mc = new MailChimpManager(ConfigurationManager.AppSettings["MailChimpApiKey"]);
                    var mc=new MailChimpLogic();
                


            #region Bind DropDown Lists

                            //ListResult Lresult = mc.GetLists();
                            //List<ListInfo> lstInfo = Lresult.Data;
                                var lresult= mc.GetAllLists();
                             var lstInfo = lresult.Data;
                            ddlMailChimpExistingList.Items.Add(new ListItem("--Select--", "-1"));
                            foreach (var lss in lstInfo)
                            {
                                ddlMailChimpExistingList.Items.Add(new ListItem(lss.Name, lss.Id));
                   
                            }
                
                            ddlMailChimpExistingList.DataBind();


                 ddlMailChimpSegments.Items.Add(new ListItem("--Select--", "-1"));
                if(ddlMailChimpExistingList.SelectedValue!="-1")
                {
                      var sSegRes= mc.GetSegmentsForList(ddlMailChimpExistingList.SelectedValue);
                     
                      foreach(var segment in sSegRes)
                      {
                         ddlMailChimpSegments.Items.Add(new ListItem(segment.SegmentName,segment.StaticSegmentId.ToString()));
                      }
                    
                   
                    
                 }          


            #endregion
                }
                catch (MailChimp.Errors.MailChimpAPIException ex)
                {
                    Response.Write("<script language=javascript>alert('" + ex.Message + "');</script>");

                }
            }
        }

        //private void SetTimeIdsFromSession(out List<int> orderOriginIds)
        //{
        //    orderOriginIds = new List<int>() { -1 };

        //    #region Purchase Point
        //    var orderListReportPurchasePointTypes = Enum.GetValues(typeof(OrderOrigin)).Cast<OrderOrigin>();

        //    foreach (var orderListReportPurchasePointType in orderListReportPurchasePointTypes)
        //    {
        //        orderOriginIds.Add((int)orderListReportPurchasePointType);

        //    }
        //    #endregion




        //}

        public string Url
        {
            get
            {
                // var orderBy = OrderListReportOrderByType.OrderDateDesc;
                var url = "";

                try
                {
                    url = Request.QueryString["url"];
                }
                catch (Exception)
                {
                }

                return url;
            }
        }

        private void SetTimeIdsFromSession(out List<int> showTimeIds, out List<int> eventSectionTimeIds, out List<int> openTicketSectionIds)
        {
            showTimeIds = new List<int>() { -1 };
            eventSectionTimeIds = new List<int>() { -1 };
            openTicketSectionIds = new List<int>() { -1 };

            if ((Session["OrderList_showTimeIds"] as List<int>) != null)
                showTimeIds = Session["OrderList_showTimeIds"] as List<int>;

            if ((Session["OrderList_eventSectionTimeIds"] as List<int>) != null)
                eventSectionTimeIds = Session["OrderList_eventSectionTimeIds"] as List<int>;

            if ((Session["OrderList_openTicketSectionIds"] as List<int>) != null)
                openTicketSectionIds = Session["OrderList_openTicketSectionIds"] as List<int>;

        }

        private List<OrderListReportResponse> GetReportListData()
        {
            #region Get Time Ids from Session

            List<int> showTimeIds;
            List<int> eventSectionTimeIds;
            List<int> openTicketSectionIds;

            SetTimeIdsFromSession(out showTimeIds, out eventSectionTimeIds, out openTicketSectionIds);

            #endregion


            var utcOffsetMinutes = VenueController.GetUtcOffSetByTimeZone(SiteSettings.DefaultTimeZone());

            DateTime rdpStartDate =DateTime.Now.AddYears(-1);
            DateTime rdpEndDate = DateTime.Now.AddYears(1);
            if (string.IsNullOrEmpty(Request.QueryString["startDate"]) == false)
            {
                try
                {
                    rdpStartDate = Convert.ToDateTime(Request.QueryString["startDate"], CultureInfo.GetCultureInfo("en-US"));
                }
                catch (Exception)
                {

                }
            }


            var data = OrderListReportHelper.GetOrderListReport(
                          startDateLocal: rdpStartDate,
                          endDateLocal: rdpEndDate,
                          showTimeIds: showTimeIds,
                          eventSectionTimeIds: eventSectionTimeIds,
                          openTicketSectionIds: openTicketSectionIds,
                          orderBy: OrderListReportOrderByType.OrderDateDesc,
                          utcOffsetMinutes: utcOffsetMinutes,
                          applicationId: SiteSettings.ApplicationId());

            return data;
        }

        protected void btnAddSegment_Click(object sender, EventArgs e)
        {
            var mcLogic = new MailChimpLogic();
           
      
            if (ddlMailChimpExistingList.SelectedValue != "-1")
            {
                int newsegmentid = mcLogic.AddNewStaticSegmentToList(ddlMailChimpExistingList.SelectedValue, txtAddSegment.Text);
               var SSegRes = mcLogic.GetSegmentsForList(ddlMailChimpExistingList.SelectedValue);

                ddlMailChimpSegments.Items.Clear();
                foreach (var segment in SSegRes)
                {
                    ddlMailChimpSegments.Items.Add(new ListItem(segment.SegmentName, segment.StaticSegmentId.ToString()));
                }

                ddlMailChimpSegments.Items.Add(new ListItem("--Select--", "-1"));
                ddlMailChimpSegments.SelectedValue = newsegmentid == -1 ? "-1" : newsegmentid.ToString();
                ddlMailChimpSegments.DataBind();
            }     

        }

        protected void ddlMailChimpExistingList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var mcLogic = new MailChimpLogic();
            ddlMailChimpSegments.Items.Clear();
         
            if (ddlMailChimpExistingList.SelectedValue != "-1")
            {
                var sSegRes = mcLogic.GetSegmentsForList(ddlMailChimpExistingList.SelectedValue);

                foreach (var segment in sSegRes)
                {
                    ddlMailChimpSegments.Items.Add(new ListItem(segment.SegmentName, segment.StaticSegmentId.ToString()));
                }
                ddlMailChimpSegments.Items.Add(new ListItem("--Select--", "-1"));
                ddlMailChimpSegments.SelectedValue="-1";
                ddlMailChimpSegments.DataBind();

            }     

        }


        public string Parameter
        {
            get
            {
                // var orderBy = OrderListReportOrderByType.OrderDateDesc;
                var id = "";

                try
                {
                    switch (Url)
                    {

                        case "ParticipantPerEventTime":
                            id = Request.QueryString["EventId"];
                            // var pc = new ParticipantsController();
                            //var tbl = pc.GetContactsPerEvent("12006");
                            // count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportData(), ddlConstantContactExistingList.SelectedValue);
                            break;
                        case "ParticipantsPerShowTime":
                            id = Request.QueryString["ShowId"];
                            // count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportData(GetReportListData()), ddlConstantContactExistingList.SelectedValue);
                            break;
                        case "ParticipantsPerOpenTicketSection":
                            id = Request.QueryString["ShowId"];
                            // count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportData(GetReportListData()), ddlConstantContactExistingList.SelectedValue);
                            break;
                        default:
                            // count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportData(GetReportListData()), ddlConstantContactExistingList.SelectedValue);
                            break;
                    }
                }
                catch (Exception)
                {
                }

                return id;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var mcLogic = new MailChimpLogic();
                // GetReportListData()
                //int count = 0;
                //switch (Url)
                //{

                //    case "ParticipantPerEventTime":
                //        //var dic1 = list.ToDictionary(item => item.Id,
                //        //                            item => item.Name);
                //         count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportData(), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                //        break;
                //    case "ParticipantsPerShowTime":
                //         count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportData(GetReportListData()), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                //        break;
                //    case "ParticipantsPerOpenTicketSection":
                //         count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportData(GetReportListData()), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                //        break;
                //    default:
                //         count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportData(GetReportListData()), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                //        break;
                //}
                int count = 0;
                var pc = new ParticipantsController();
                var tbl = new Hashtable();
                switch (Url)
                {

                    case "ParticipantPerEventTime":

                        tbl = pc.GetContactsPerEvent(Parameter);
                        count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportFunctionsResponse(tbl), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                        break;
                    case "ParticipantsPerShowTime":

                        tbl = pc.GetContactsPerShow(Parameter);
                        count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportFunctionsResponse(tbl), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                        break;
                    case "ParticipantsPerOpenTicketSection":

                        tbl = pc.GetContactsPerOpenTicketSectionId(Parameter);
                        count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportFunctionsResponse(tbl), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                        break;
                    default:
                        count = mcLogic.ListSubscribeMember(mcLogic.MakeListOfContactFromReportData(GetReportListData()), ddlMailChimpExistingList.SelectedValue, Convert.ToInt32(ddlMailChimpSegments.SelectedValue));
                        break;
                }

            

                    if (count == 0) { lblCount.Text = count.ToString() + " Rows Added. Either All members already present on the list or No list Exists"; }
                    else { lblCount.Text = count.ToString() + " Rows Added Successfully"; }
                    lblCount.ForeColor = System.Drawing.Color.Red;
               
            }

            catch (MailChimp.Errors.MailChimpAPIException ex)
            {

                Response.Write("<script language=javascript>alert('" + ex.Message + "');</script>");
            }
        }
    }
}