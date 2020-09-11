<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" CodeFile="PostDeadlineEnrollment.aspx.vb" Inherits="admin_TwoPrice_Campaigns_PostDeadlineEnrollment" %>

<asp:Content ContentPlaceHolderID="ph" ID="Content" runat="server">

<script type="text/javascript">
    function ParticipationMessage() {
        var x = confirm('Are you sure that you want to assign this builder to the corresponding CP event?');

        if (x == true)
            return true;
        else
            return false;
    }
</script>
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
h4.campaignTitle{
    background: #0E2D50 !important;
}
.deadline-form .campaignBtn{
    color: #656263;
    font-size: 10px;
    font-family: Arial,Helvetica,Verdana,sans-serif;
    text-transform: uppercase;
    line-height: 24px;
    text-decoration: none;
    padding: 4px 14px;
    border-radius: 5px;
    border: 1px solid #656263;
    font-weight: bold;
    background: transparent !important;
}
.deadline-form {
    padding: 0 10px;
}
.deadline-form div {
    height: 100%;
    overflow: auto;
}
.deadline-form div table{
    width:100%;
}
.deadline-form.tbl-width-adjustment div table {
    width: 65%;
    padding: 0px;
}
.deadline-form.tbl-width-adjustment div table td{
    text-align:center;
}
.deadline-form.tbl-width-adjustment div table td input[type="radio"]{
  float:left; margin:0px;
}
</style>

<h4>Committed Purchase Event - Post Deadline Enrollment</h4>
 <h4 class="campaignTitle">
            <asp:Literal ID="lblEventName" runat="server"></asp:Literal></h4>

   <div class="form deadline-form tbl-width-adjustment">
            <label>
                <a href="/admin/twoprice/campaigns/default.aspx" target="main" class="btn campaignBtn" style="text-decoration:none;color:black;">Return to event management</a>
               </label>
				<div style="clear: both;"></div>

<CC:GridView ID="gvList" CellSpacing="2" CellPadding="2" runat="server" PageSize="50" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records founds." AutoGenerateColumns="False" BorderWidth="0px" PagerSettings-Position="Bottom" DragAndDropColumnIndex="1" DragAndDropIDColumnName="" EnableDragAndDrop="False" SortImageAsc="/images/admin/asc3.gif" SortImageDesc="/images/admin/desc3.gif" SortOrder="ASC" CausesValidation="False">
        <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top"></AlternatingRowStyle>

<HeaderStyle VerticalAlign="Top"></HeaderStyle>

        <RowStyle CssClass="row" VerticalAlign="Top"></RowStyle>
        <Columns>
            <asp:BoundField SortExpression="BuilderID" DataField="BuilderID" HeaderText="Builder ID"></asp:BoundField>
            <asp:BoundField SortExpression="CompanyName" DataField="CompanyName" HeaderText="Builder Name"></asp:BoundField>
            <asp:BoundField SortExpression="ResponseDeadline" DataField="ResponseDeadline" HeaderText="Response Deadline"></asp:BoundField>
            <asp:BoundField SortExpression="ResponseStatus" DataField="ResponseStatus" HeaderText="Response Status"></asp:BoundField>
            
             <asp:TemplateField HeaderText="Enrollment">
                <ItemTemplate>
                    <asp:RadioButtonList ID="rblYesNo" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rblYesNo_SelectedIndexChanged" RepeatDirection="Horizontal">
                        <asp:ListItem>Yes</asp:ListItem>
                    </asp:RadioButtonList>
                </ItemTemplate>
            </asp:TemplateField>
            
            
        </Columns>
    </CC:GridView>











        </div>



</asp:Content> 