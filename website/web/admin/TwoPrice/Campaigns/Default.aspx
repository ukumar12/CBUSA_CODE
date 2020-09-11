<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Committed Purchase Event" CodeFile="default.aspx.vb" Inherits="Index" %>

<%@ Register TagName="Search" Src="~/controls/SearchSql.ascx" TagPrefix="CC" %>

<asp:Content ContentPlaceHolderID="ph" ID="Content" runat="server">
    <%--<link rel="stylesheet" href="/includes/style.css" type="text/css">--%>
    <%--<asp:ScriptManager ID="sc" runat="server"></asp:ScriptManager>--%>
    <style type="text/css">
        div.pform {
            display: none;
            position: absolute;
            z-index: 10;
            background-color: #fff;
            border: 1px solid #333;
        }

        .btnGray {
            overflow: visible;
            color: #656263;
            font-size: 10px;
            text-transform: uppercase;
            border-style: none;
            padding: 3px 10px;
            border-radius: 5px;
            border: 2px solid #c2c2c2;
            font-weight: bold;
            background: #d7d7d7;
            cursor: pointer;
        }

        .copyCpheader {
            text-align: left;
            background-color: #0e2d50;
            font-size: 12px;
            padding: 10px 15px;
            color: #FFF;
        }
       table[id$=gvList] a[disabled]{
            color:#c4c0c0;
        }
    </style>
    <h4>Committed Purchase Event Administration</h4>
    <span class="smaller">Please provide search criteria below</span>
    <asp:Panel ID="pnlSearch" DefaultButton="btnSearch" runat="server">
        <table cellpadding="2" cellspacing="2">
            <tr>
                <th valign="top">Name:</th>
                <td valign="top" class="field">
                    <asp:TextBox ID="F_Name" runat="server" Columns="50" MaxLength="50"></asp:TextBox></td>
            </tr>
            <tr>
                <th valign="top">Status:</th>
                <td valign="top" class="field">
                    <asp:DropDownList ID="F_Status" runat="server" Columns="50" MaxLength="50"></asp:DropDownList></td>
            </tr>
            <tr>
                <th valign="top"><b>Start Date:</b></th>
                <td valign="top" class="field">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="smaller">From
                                <CC:DatePicker ID="F_StartDateLbound" runat="server" /></td>
                            <td>&nbsp;</td>
                            <td class="smaller">To 
                                <CC:DatePicker ID="F_StartDateUbound" runat="server" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th valign="top"><b>End Date:</b></th>
                <td valign="top" class="field">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="smaller">From
                                <CC:DatePicker ID="F_EndDateLbound" runat="server" /></td>
                            <td>&nbsp;</td>
                            <td class="smaller">To 
                                <CC:DatePicker ID="F_EndDateUbound" runat="server" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th valign="top"><b>Create Date:</b></th>
                <td valign="top" class="field">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td class="smaller">From
                                <CC:DatePicker ID="F_CreateDateLbound" runat="server" /></td>
                            <td>&nbsp;</td>
                            <td class="smaller">To 
                                <CC:DatePicker ID="F_CreateDateUbound" runat="server" /></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <th valign="top"><b>Is Active:</b></th>
                <td valign="top" class="field">
                    <asp:DropDownList ID="F_IsActive" runat="server">
                        <asp:ListItem Value="">-- ALL --</asp:ListItem>
                        <asp:ListItem Selected="True" Value="1">Yes</asp:ListItem>
                        <asp:ListItem Value="0">No</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right">
                    <CC:OneClickButton ID="btnSearch" runat="server" Text="Search" CssClass="btn" />
                    <input class="btn" type="submit" value="Clear" onclick="window.location = 'default.aspx'; return false;" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <p></p>
    <CC:OneClickButton ID="AddNew" runat="server" Text="Add New Committed Purchase Event" CssClass="btn"></CC:OneClickButton>
    <input type="button" id="btnCopyExistingCP" value="Copy Existing Committed Purchase Event" class="btn" onclick="OpenCpEventPopUp()" />
    <%--<CC:OneClickButton ID="CopyExisting" runat="server" Text="Copy Existing Committed Purchase Event" CssClass="btn"></CC:OneClickButton>--%>
    <p></p>

    <CC:PopupForm ID="frmSelectCPEvent" runat="server" OpenMode="MoveToCenter" CloseTriggerId="btnCancel" CssClass="pform" Animate="true" Width="350px" Height="200px" ShowVeil="true" VeilCloses="false">
        <FormTemplate>
            <div class="copyCpheader">Copy CP Event</div>
            <div class="pckghdggray" style="margin-top: 40px;">
                <%--<div style="background-color:#fff;width:200px;height:80px;text-align:center;padding:30px 10px;">
            <img src="/images/loading.gif" alt="Processing..." /><br /><br />
            <h1 class="largest">Processing... Please Wait</h1>
        </div>--%>

                <div style="text-align: left; margin-bottom: 10px; font-size: 12px; margin-left: 15px; font-weight: bold;">Select CP Event:</div>

                <asp:DropDownList ID="ddlCpEvent" CssClass="form-control" Style="margin-left: 15px; height: 30px; width: 320px;" runat="server"></asp:DropDownList>
                <br />
                <div class="clearfix"></div>
                <div style="margin-top: 20px; text-align: center;">
                    <%--<input type="button" id="btnNavExistingCP" class="btn" value="Copy CP Event" onclick="NavigateToCpEventDetails()" />--%>
                    <CC:OneClickButton ID="btnNavExistingCP" runat="server" Text="Copy CP Event" CssClass="btnGray"></CC:OneClickButton>
                    <CC:OneClickButton ID="btnCancel" runat="server" Text="Close" CssClass="btnGray"></CC:OneClickButton>
                </div>
            </div>
        </FormTemplate>
    </CC:PopupForm>
    <CC:GridView ID="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
        <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>
        <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink EnableViewState="False" runat="server" NavigateUrl='<%# "edit.aspx?TwoPriceCampaignId=" & DataBinder.Eval(Container.DataItem, "TwoPriceCampaignId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/edit.gif" ID="lnkEdit">Edit</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <CC:ConfirmLink EnableViewState="False" Message="Are you sure that you want to remove this Committed Purchase Event?" runat="server" NavigateUrl='<%# "delete.aspx?TwoPriceCampaignId=" & DataBinder.Eval(Container.DataItem, "TwoPriceCampaignId") & "&" & GetPageParams(Components.FilterFieldType.All) %>' ImageUrl="/images/admin/delete.gif" ID="lnkDelete">Delete</CC:ConfirmLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Name"></asp:BoundField>
            <asp:BoundField SortExpression="Status" DataField="StatusName" HeaderText="Status"></asp:BoundField>
            <asp:BoundField SortExpression="StartDate" DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:d}" HtmlEncode="False"></asp:BoundField>
            <asp:BoundField SortExpression="EndDate" DataField="EndDate" HeaderText="End Date" DataFormatString="{0:d}" HtmlEncode="False"></asp:BoundField>
            <asp:BoundField SortExpression="CreateDate" DataField="CreateDate" HeaderText="Create Date" DataFormatString="{0:d}" HtmlEncode="False"></asp:BoundField>
            <asp:ImageField SortExpression="IsActive" DataImageUrlField="IsActive" HeaderText="Is Active" DataImageUrlFormatString="/images/admin/{0}.gif" ReadOnly="true" ItemStyle-HorizontalAlign="Center" />
            <%--<asp:HyperLinkField HeaderText="Participating Builders" Text="Select Builders" DataNavigateUrlFields="TwoPriceCampaignId" DataNavigateUrlFormatString="/admin/twoprice/campaigns/builders.aspx?TwoPriceCampaignId={0}" />--%>

            <asp:HyperLinkField HeaderText="Participating Builders" Text="Invite Builders (New)" DataNavigateUrlFields="TwoPriceCampaignId" DataNavigateUrlFormatString="/admin/twoprice/campaigns/InviteBuilder.aspx?TwoPriceCampaignId={0}" />
            <asp:HyperLinkField HeaderText="Participating Builders" Text="Select Builders" DataNavigateUrlFields="TwoPriceCampaignId" DataNavigateUrlFormatString="/admin/twoprice/campaigns/builders.aspx?TwoPriceCampaignId={0}" />

            <asp:HyperLinkField HeaderText="Bid List" Text="Assemble Bid List" DataNavigateUrlFields="TwoPriceCampaignId" DataNavigateUrlFormatString="/admin/twoprice/campaigns/takeoffs/edit.aspx?TwoPriceCampaignId={0}" />
            <asp:HyperLinkField HeaderText="Participating Vendors" Text="Select Vendors" DataNavigateUrlFields="TwoPriceCampaignId" DataNavigateUrlFormatString="/admin/twoprice/campaigns/Vendors.aspx?TwoPriceCampaignId={0}" />
            <asp:HyperLinkField HeaderText="Analysis" Text="Analysis" DataNavigateUrlFields="TwoPriceCampaignId" DataNavigateUrlFormatString="/admin/twoprice/Analysis/Edit.aspx?TwoPriceCampaignId={0}" />
            <%--<asp:HyperLinkField HeaderText="Post Deadline Enrollment"
                Visible='<%# If ( DateTime.Now.Date > DateTime.Parse(Eval("ResponseDeadline")).Date And Val(Eval("AwardedVendorId")) = 0 And DateTime.Now.Date < DateTime.Parse(Eval("EndDate")).Date, true, false) %>' Text="Post Deadline Enrollment" DataNavigateUrlFields="TwoPriceCampaignId" DataNavigateUrlFormatString="/admin/twoprice/campaigns/PostDeadlineEnrollment.aspx?TwoPriceCampaignId={0}" />--%>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkbtnPostDeadLineEnrollment" runat="server"
                        Enabled='<%# If(DateTime.Now.Date > DateTime.Parse(Eval("ResponseDeadline")).Date And Val(Eval("AwardedVendorId")) = 0 And Convert.ToInt32(Eval("InvitationStatus")) = 1 And DateTime.Now.Date < DateTime.Parse(Eval("EndDateFormatted")).Date, True, False) %>'
                        Style="text-decoration: underline;" Text="Post Deadline Enrollment" CommandArgument='<%# Eval("TwoPriceCampaignId") %>' onclick="RedirectToPostDeadlineEnrollment"></asp:LinkButton>
                </ItemTemplate>
                <HeaderTemplate>
                    Post Deadline Enrollment
                </HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </CC:GridView>

    <script type="text/javascript">
        function OpenCpEventPopUp() {
            var form = $get('<%=frmSelectCPEvent.ClientID %>').control;
            form._doMoveToCenter();
            form.Open();
        }
        function NavigateToCpEventDetails() {
            var ddlCpEvent = '<%=frmSelectCPEvent.FindControl("ddlCpEvent") %>';
            alert('<%= ddlCpEvent.SelectedValue%>');
        }
    </script>
</asp:Content>
