using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms.VisualStyles;
using CommonClassLib;
using CommonClassLib.BusinessLogic.Reports;
using CommonClassLib.BusinessLogic.Reports.BoxOffice;
using CommonClassLib.Integrations;
using CommonClassLib.PermissionsLogic;
using CommonClassLib.Utility;
using CTCT;
using CTCT.Components.Contacts;
using StCore.Lib.Logging;
using StCore.ORM;
using Telerik.Web.UI;
using TS.Common.BusinessLogic;
using TS.Common.Objects;


namespace DemoSiteApp.AdminPanel.Reports.Popups
{
    public partial class ConstantContactExport : System.Web.UI.Page
    {
        public int? QsShowId
        {
            get
            {
                int id;

                int.TryParse(Request.QueryString["EventId"], out id);

                if (id <= 0)
                    return null;
                else
                    return id;
            }
        }

        public int? QsEventSectionTimeId
        {
            get
            {
                int id;

                int.TryParse(Request.QueryString["EventTime"], out id);

                if (id <= 0)
                    return null;
                else
                    return id;
            }
        }


        string _accessToken = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            PermissionController.Setting.Update.IsAuthorized();
            if (!Page.IsPostBack)
            {
                if (HttpContext.Current.Request.QueryString["code"] != null || HttpContext.Current.Request.QueryString["code"] != string.Empty)
                {
                    _accessToken = OAuth.GetAccessTokenByCode(HttpContext.Current, HttpContext.Current.Request.QueryString["code"]);
                }

               // BindGrid();
                try
                {
                    var ccLogic = new ConstantContactLogic();
                    var cntctList = ccLogic.GetAllList();
                    foreach (var list in cntctList)
                    {
                        ddlConstantContactExistingList.Items.Add(new ListItem(list.Name, list.Id));

                        //do whatever you want to do.
                    }

                    ddlConstantContactExistingList.DataBind();

                }
                catch (Exception ex)
                {
                   // lblCount.Text = ex.Message; 
                    StLogger.Error("Error Binding Constant Contact", ex);
                    Response.Write("<script language=javascript>alert('" + ex.Message + "');</script>");
                    throw;
                } 
                
            }




        }


        private DataTable GetTestEmails()
        {

            DataTable dt=new DataTable("Contacts");
            dt.Columns.Add("FirstName");
            dt.Columns.Add("LastName");
            dt.Columns.Add("Email");

            dt.Rows.Add("Indocin", "David","i22d1avid@test.com" );
            dt.Rows.Add("Enebrel", "Sam", "e1s22am@test.com");
            dt.Rows.Add("Hydralazine", "Chris1toff", "hch222ristoff@test.com");
            dt.Rows.Add("Combivent", "Janet", "cj33ane1t@test.com");
            dt.Rows.Add("Dilantin", "Melanie", "dm33e1lanie@test.com");

            dt.Rows.Add("Evelyn", "Hunter", "ehun331ter0@themeforest.net");
            dt.Rows.Add("Craig", "Little", "clit133tle1@t.co");
            dt.Rows.Add("Jerry", "Burke", "jburk3331e2@desdev.cn");
            dt.Rows.Add("William", "Burns", "wbu33r1ns3@cyberchimps.com");
            dt.Rows.Add("Brenda", "Burton", "bbur331ton4@washington.edu");

            
            return dt;

          

        }



        
        protected void ddlList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddlConstantContactExistingList.Items.Clear();
                var ccLogic = new ConstantContactLogic();
                var cntctList = ccLogic.GetAllList();
                foreach (var list in cntctList)
                {
                    ddlConstantContactExistingList.Items.Add(new ListItem(list.Name, list.Id));

                    //do whatever you want to do.
                }

                ddlConstantContactExistingList.DataBind();
            }
            catch (Exception ex)
            {
                StLogger.Error("Error Binding Constant Contact ddlConstantContactExistingList", ex);
                Response.Write("<script language=javascript>alert('" + ex.Message + "');</script>");
                //lblCount.Text = ex.Message; 
                throw;
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

            DateTime rdpStartDate = DateTime.Now.AddYears(-1);
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
            if (!Page.IsValid)
            {
                return;
            }
            try
            {
                var ccLogic = new ConstantContactLogic();

                int count = 0;
                var pc = new ParticipantsController();
                var tbl=new Hashtable();
                switch (Url)
                {

                    case "ParticipantPerEventTime":
                       
                         tbl = pc.GetContactsPerEvent(Parameter);
                         count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportFunctionsResponse(tbl), ddlConstantContactExistingList.SelectedValue);
                        break;
                    case "ParticipantsPerShowTime":
                            
                              tbl = pc.GetContactsPerShow(Parameter);
                              count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportFunctionsResponse(tbl), ddlConstantContactExistingList.SelectedValue);
                        break;
                    case "ParticipantsPerOpenTicketSection":
                          
                            tbl = pc.GetContactsPerOpenTicketSectionId(Parameter);
                            count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportFunctionsResponse(tbl), ddlConstantContactExistingList.SelectedValue);
                        break;
                    default:
                        count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportData(GetReportListData()), ddlConstantContactExistingList.SelectedValue);
                        break;
                }
                if (count == 0) { lblCount.Text = count.ToString() + " Rows Added. Either All members already present on the list or No list Exists"; }
                else { lblCount.Text = count.ToString() + " Rows Added Successfully"; }
                lblCount.ForeColor = System.Drawing.Color.Red;
                   // DataTableHelper = new DataTableHelper();

               // int Count = ccLogic.ListSubscribeMember(ccLogic.MakeListOfContactFromReportData(GetReportListData()), ddlConstantContactExistingList.SelectedValue);
                    //rbHDR.SelectedItem.Text

               
                
            }

            catch (CTCT.Exceptions.CtctException ex)
            {
                StLogger.Error("Error Adding Members to the List", ex);
                Response.Write("<script language=javascript>alert('" + ex.Message + "');</script>");
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClosePopup", "javascript:returnToParent();", true);
            return;

        }

        protected void btnAddList_Click(object sender, EventArgs e)
        {
            var ccLogic = new ConstantContactLogic();
           var newlist= ccLogic.AddList(txtAddList.Text);

            ddlConstantContactExistingList.Items.Clear();
            var cntctList = ccLogic.GetAllList();
            foreach (var list in cntctList)
            {
                ddlConstantContactExistingList.Items.Add(new ListItem(list.Name, list.Id));

            }
            ddlConstantContactExistingList.SelectedValue = newlist.Id;
            txtAddList.Text = "";
            ddlConstantContactExistingList.DataBind();

        }
    }
}