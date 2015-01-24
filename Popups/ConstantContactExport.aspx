<%@ Page Title="" Language="C#" MasterPageFile="~/PopUp.master" AutoEventWireup="true" CodeBehind="ConstantContactExport.aspx.cs" Inherits="DemoSiteApp.AdminPanel.Reports.Popups.ConstantContactExport" %>
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
                        <asp:Image ID="imgLogo" runat="server" ImageUrl="~/AdminPanel/Shows/ImportEvents/Images/Constant_Contact.jpg" Width="64" />
                    </td>
                </tr>
                <tr runat="server" id="trAPIKey">
                    <td>
                        <asp:Label ID="lblConstantContact" runat="server">Select Existing Lists </asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlConstantContactExistingList" runat="server" AutoPostBack="false" OnSelectedIndexChanged="ddlList_SelectedIndexChanged"></asp:DropDownList>
                       
                    
                    </td>
                    </tr>
                <tr><td align="center" colspan="2">OR</td></tr>
                  <tr> <td>
                        <asp:Label ID="Label1" runat="server">Add New List </asp:Label> 
                    </td>
                       <td>
                       <asp:TextBox runat="server" ID="txtAddList"/>
                    </td>
                      <td>
                           <ST:StButton IconType="Submit" ID="btnAddList" Text="<%$ Resources:LocalizedText, Add %>"
                            runat="server" OnClientClicked="Validate" OnClick="btnAddList_Click">
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
                            NavigateUrl="http://www.constantcontact.com/features/signup.jsp" Target="_blank">Sign Up for the Constant Contact</asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <p>
                            
                            One tool, for all your marketing. That’s the Constant Contact Toolkit. Our customers get real results through marketing campaigns like email newsletters, surveys, events, Facebook promotions, online listings, and more.
                        </p>
                    </td>
                </tr>

            </table>
        </div>
    </div>
    

</asp:Content>
