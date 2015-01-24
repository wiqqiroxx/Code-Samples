<%@ Page Title="" Language="C#" MasterPageFile="~/PopUp.master" AutoEventWireup="true" CodeBehind="MailChimpExport.aspx.cs" Inherits="DemoSiteApp.AdminPanel.Reports.Popups.MailChimpExport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    
    
         <asp:ScriptManagerProxy runat="server" ID="scriptMgrAmiando" />
    <script type="text/javascript"> 
     function returnToParent() {
            GetRadWindow().BrowserWindow.ReloadTab("2");
            GetRadWindow().close();
        }

        function closeWindow() {
            GetRadWindow().close();

        }
        function Validate() {

           
            if (googleAnalytics.match(/([\<])([^\>]{1,})*([\>])/i) != null) {
                alert("No html tags are allowed");
                return false;
            }
            return true;
        }
     </script>
     <div style="padding-left: 10px;">
        <div runat="server" id="dvInputs">
            <table>
                <tr>
                    <td colspan="2">
                        <asp:Image ID="imgLogo" runat="server" ImageUrl="~/AdminPanel/Shows/ImportEvents/Images/mailchimp-logo.png" Width="64" />
                    </td>
                </tr>
                <tr runat="server" id="trAPIKey">
                    <td>
                        <asp:Label ID="lblConstantContact" runat="server">Select Existing Lists </asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMailChimpExistingList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMailChimpExistingList_SelectedIndexChanged"></asp:DropDownList>
                       
                    
                    </td>
                       <td>
                        <asp:DropDownList ID="ddlMailChimpSegments" runat="server" AutoPostBack="false" ></asp:DropDownList>
                       
                    
                    </td>
                    </tr>
                <tr><td align="center" colspan="2">OR</td></tr>
                  <tr> <td>
                        <asp:Label ID="Label1" runat="server">Add New Segment </asp:Label> 
                    </td>
                       <td>
                       <asp:TextBox runat="server" ID="txtAddSegment"/>
                    </td>
                      <td>
                           <ST:StButton IconType="Submit" ID="btnAddSegment" Text="<%$ Resources:LocalizedText, Add Segment %>"
                            runat="server" OnClientClicked="Validate" OnClick="btnAddSegment_Click">
                        </ST:StButton>

                      </td>
                      </tr> 
                    <tr>
                        
                        <td>
                             <asp:Label ID="lblCount" runat="server" Text=""></asp:Label> 
                            
                        </td>
                    </tr>
               
                <tr>
                    <td style="text-align: left;">
                        <ST:StButton IconType="Submit" ID="btnSubmit" Text="<%$ Resources:LocalizedText, Submit %>"
                            runat="server" OnClientClicked="Validate" OnClick="btnSubmit_Click">
                        </ST:StButton>
                    </td>
                    <td style="text-align: left;" class="pl">
                        <ST:StButton IconType="Cancel" ID="btnBack" runat="server" Text="<%$ Resources:LocalizedText, Cancel %>"
                            CausesValidation="false" OnClientClicked="closeWindow">
                        </ST:StButton>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />

                        
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                       <asp:HyperLink ID="HyperLink1" runat="server"
                            NavigateUrl="https://login.mailchimp.com/signup" Target="_blank">Sign Up for the MailChimp</asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <p>
                            
                            Online email marketing solution to manage contacts, send emails and track results. Offers plug-ins for other programs.
                        </p>
                    </td>
                </tr>

            </table>
        </div>
    </div>
    
</asp:Content>
