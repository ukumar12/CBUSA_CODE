<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMasterNotes.master" Title="Store Order Notes" CodeFile="notes.aspx.vb" Inherits="Notes"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<CC:GridView id="gvList" CellSpacing="2" CellPadding="2" runat="server" AllowPaging="False" AllowSorting="True" HeaderText="" EmptyDataText="There are no notes for this order" AutoGenerateColumns="False" BorderWidth="0" PagerSettings-Position="Bottom" Width="100%">
	<Columns>
		<asp:BoundField HTMLEncode="False" DataField="NoteDate" HeaderText="Date"></asp:BoundField>
		<asp:BoundField DataField="FullName" HeaderText="Name"></asp:BoundField>
		<asp:BoundField DataField="Note" HeaderText="Note" htmlEncode="false"></asp:BoundField>
	</Columns>
</CC:GridView>

</asp:content>

