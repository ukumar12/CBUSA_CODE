<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="productimport.aspx.vb" Inherits="admin_products_productimport" %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<h4>Import Products</h4>
<p></p>
 <CC:FileUpload ID="fulDocument" runat="server" />
 <p><a href="ProductTemplate.csv" target="_blank">Get Product File Template</a> <span class="smallest">(Uploaded file must match the template exactly including the header line)</span></p>
  <p></p>
 <CC:OneClickButton  runat="server" Text="Import" ID="btnImport" CssClass="btn" />
 <CC:OneClickButton runat="server"  Text="Cancel" ID="btnCancel" CssClass="btn" />
 <p></p>
  <asp:Literal runat="server" ID="ltlMessage"></asp:Literal><br/>
  <p></p>
  <asp:Literal runat="server" ID="ltlErrorFileLink"></asp:Literal>
</asp:content>
