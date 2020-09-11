<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="Banner" CodeFile="banners.aspx.vb" Inherits="banners"  %>
<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<h4>Banners</h4>

<p>
Banner Group Association for <b><% =dbBannerGroup.Name%></b> | <a href="/admin/banners/groups/default.aspx?<%= GetPageParams(Components.FilterFieldType.All,"F_BannerGroupId") %>">&laquo; Go Back To Banner Group List</a>
</p>
	
<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="False" HeaderText="" EmptyDataText="There are no groups in which this image will fit." AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom">
	<AlternatingRowStyle CssClass="alternate" VerticalAlign="Bottom"></AlternatingRowStyle>
	<RowStyle CssClass="row" VerticalAlign="Bottom"></RowStyle>
	<Columns>
		<asp:TemplateField>
		<ItemTemplate>
		<input type="hidden" id="BannerId" runat="server" value='<%#Container.DataItem("BannerId")%>' />
		<input type="hidden" id="UniqueId" runat="server" value='' />
		<asp:Checkbox id="chkGroup" runat="server"></asp:Checkbox>
		</ItemTemplate>
		</asp:TemplateField>
        <asp:BoundField SortExpression="Name" DataField="Name" HeaderText="Banner Name"></asp:BoundField>
		<asp:TemplateField>
		<HeaderTemplate>
		Date Range
		</HeaderTemplate>
		<ItemTemplate>
        <table border="0" cellpadding="0" cellspacing="0">
        <tr><td class="smaller">From <CC:DatePicker id="DateLbound" runat="server" /></td><td style="vertical-align:middle;"><CC:DateValidator ID="vDateLBound" runat="server" Display="Dynamic" ControlToValidate="DateLbound" ErrorMessage="Invalid 'From date'" EnableClientScript="False" Text="*"></CC:DateValidator></td><td>&nbsp;</td><td class="smaller">To <CC:DatePicker id="DateUbound" runat="server" /></td><td><CC:DateValidator ID="vDateUBound" runat="server" Display="Dynamic" ControlToValidate="DateUbound" ErrorMessage="Invalid 'To date'" EnableClientScript="False" Text="*"></CC:DateValidator></td></tr>
        </table> 	
		</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField>
		<HeaderTemplate>
		Weight
		</HeaderTemplate>
		<ItemTemplate>
		<asp:Textbox Columns="2" MaxLenght="3" id="txtWeight" runat="server"></asp:Textbox>&nbsp;(Total: <asp:Literal id="ltlTotal" runat="server"></asp:Literal>)
		<asp:RequiredFieldValidator ID="rvtxtWeight" runat="server" Display="Dynamic" ControlToValidate="txtWeight" ErrorMessage="Field 'Weight' cannot be blank"  EnableClientScript="False" Text="*"/>
		<CC:IntegerValidator ID="ivtxtWeight" runat="server" Display="Dynamic" ControlToValidate="txtWeight" ErrorMessage="Field 'Weight' must contain valid integer value" EnableClientScript="False" Text="*"/>
		</ItemTemplate>
		</asp:TemplateField>
		
	</Columns>
</CC:GridView>

<p></p>
<CC:OneClickButton id="btnSave" runat="server" Text="Save" cssClass="btn"></CC:OneClickButton>
<CC:OneClickButton id="btnCancel" runat="server" Text="Cancel" cssClass="btn" CausesValidation="False"></CC:OneClickButton>


</asp:content>

