<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VendorRatingSummary.ascx.vb" Inherits="modules_VendorRatingSummary" %>
<%@Register TagName="StarRatingDisplay" TagPrefix="CC" Src="~/controls/StarRatingDisplay.ascx" %>

<%--<div class="pckggraywrpr">
    <div class="pckghdgblue">Ratings &amp; Comments</div>
<asp:UpdatePanel ID="upRatings" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <CC:GridView ID="gvRatings" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowSorting="false">
            <Columns>
                <asp:TemplateField HeaderText="Ratings">
                    <ItemTemplate>
                        <a style="cursor:pointer;" onclick='<%# "ToggleRatings("""& Container.FindControl("divRatings").ClientID &""");" %>'>Show/Hide Ratings</a>
                        <div id="divRatings" runat="server" style="background-color:#fff;border:1px solid #666;">
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Comment" HeaderText="Comments" />
            </Columns>
        </CC:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
</div>--%>