<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BasicFinancials.ascx.vb" Inherits="modules_forms_BasicFinancials" %>

<table class="regform">
    <tr>
        <td>&nbsp;</td>
        <td class="fieldreq">&nbsp;</td>
        <td><span class="smaller"> indicates required field</span></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtNumYears" runat="server"># Years in Business</span></td>
        <td class="fieldreq" id="bartxtNumYears" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtNumYears" runat="server" MaxLength="4" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtNumEmployees" runat="server"># Employees:</span></td>
        <td class="fieldreq" id="bartxtNumEmployees" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtNumEmployees" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtYearStarted" runat="server"># Years in Business:</span></td>
        <td class="fieldreq" id="bartxtYearStarted" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtYearStarted" runat="server" MaxLength="4" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtNumDelivered" runat="server"># Homes Built &amp; Delivered since in Business:</span></td>
        <td class="fieldreq" id="bartxtNumDelivered" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtNumDelivered" runat="server" MaxLength="10" Columns="10" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtNumStarts" runat="server"># Home Starts in <%=Year(Now()) - 1%>:</span></td>
        <td class="fieldreq" id="bartxtNumStarts" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtNumStarts" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtNumProjected" runat="server">Projected # of Starts in <%=Year(Now())%>:</span></td>
        <td class="fieldreq" id="bartxtNumProjected" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtNumProjected" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtSizeRangeMin" runat="server">Size Range:</span></td>
        <td class="fieldreq" id="bartxtSizeRangeMin" runat="server">&nbsp;</td>
        <td class="field">
            Minimum: <asp:TextBox ID="txtSizeRangeMin" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox><br />
            Maximum: <asp:TextBox ID="txtSizeRangeMax" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtPriceRangeMin" runat="server">Price Range:</span></td>
        <td class="fieldreq" id="bartxtPriceRangeMin" runat="server">&nbsp;</td>
        <td class="field">
            Minimum: <asp:TextBox ID="txtPriceRangeMin" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox><br />
            Maximum: <asp:TextBox ID="txtPriceRangeMax" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtAvgPerFoot" runat="server">Avg $/sq ft</span></td>
        <td class="fieldreq" id="bartxtAvgPerFoot" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtAvgPerFoot" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtRevenue" runat="server">Company Revenue (<%=Year(NOw())-1 %>):</span></td>
        <td class="fieldreq" id="bartxtRevenue" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtRevenue" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtRevenueProjected" runat="server">Projected Company Revenue (<%=Year(Now()) %>):</span></td>
        <td class="fieldreq" id="bartxtRevenueProjected" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtRevenueProjected" runat="server" MaxLength="10" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtCogs" runat="server">Total COGS (<%=Year(Now())-1 %>):</span></td>
        <td class="fieldreq" id="bartxtCogs" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtCogs" runat="server" MaxLength="4" Columns="4" CssClass="regtxtshort" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl">Company Memberships and/or Affiliations:</td>
        <td>&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtMemberships" runat="server" TextMode="MultiLine" Rows="3" Columns="50" CssClass="regtxtarea" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labeltxtAreas" runat="server">Areas/Cities/Counties in Which You Build</span></td>
        <td class="fieldreq" id="bartxtAreas" runat="server">&nbsp;</td>
        <td class="field"><asp:TextBox ID="txtAreas" runat="server" Rows="3" Columns="50" CssClass="regtxtarea" ValidationGroup="BuilderReg"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labelctrlReferences" runat="server">Supplier/Trade References:</span></td>
        <td class="fieldreq" id="barctrlReferences" runat="server">&nbsp;</td>
        <td class="field">

        </td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labelctrlExpenditures" runat="server">Builder Phase Expenditures:</span></td>
        <td class="fieldreq" id="barctrlExpenditures" runat="server">&nbsp;</td>
        <td class="field">
        </td>
    </tr>
    <tr>
        <td class="fieldlbl"><span id="labelcbTerms" runat="server">Accepted Terms:</span></td>
        <td class="fieldreq" id="barcbTerms" runat="server">&nbsp;</td>
        <td class="field"><asp:CheckBox ID="cbTerms" runat="server" /></td>
    </tr>
</table>    

<p style="text-align:center;">
    <CC:OneClickButton ID="btnContinue" runat="server" Text="Continue" CssClass="btn" />
</p>

<CC:RequiredFieldValidatorFront ID="rfvNumYears" runat="server" ControlToValidate="txtNumYears" ErrorMessage="Field 'Years in Business' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivNumYears" runat="server" ControlToValidate="txtNumYears" ErrorMessage="Field 'Years in Business' is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvNumEmployees" runat="server" ControlToValidate="txtNumEmployees" ErrorMessage="Field 'Number of Employees' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivNumEmployees" runat="server" ControlToValidate="txtNumEmployees" ErrorMessage="Field 'Number of Employees' is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvNumDelivered" runat="server" ControlToValidate="txtNumDelivered" ErrorMessage="Field 'Number of Homes Built and Delivered' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivNumDelivered" runat="server" ControlToValidate="txtNumDelivered" ErrorMessage="Field 'Number of Homes Built and Delivered' is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvNumStarts" runat="server" ControlToValidate="txtNumStarts" ErrorMessage="Field 'Number of Home Starts' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivNumStarts" runat="server" ControlToValidate="txtNumStarts" ErrorMessage="Field 'Number of Home Starts' is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:IntegerValidatorFront ID="ivNumProjected" runat="server" ControlToValidate="txtNumProjected" ErrorMessage="Field 'Projected Home Starts' is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvSizeRangeMin" runat="server" ControlToValidate="txtSizeRangeMin" ErrorMessage="Field 'Size Range: Minimum' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivSizeRangeMin" runat="server" ControlToValidate="txtSizeRangeMin" ErrorMessage="Field 'Size Range: Minimum' is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvSizeRangeMax" runat="server" ControlToValidate="txtSizeRangeMax" ErrorMessage="Field 'Size Range: Maximum' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:IntegerValidatorFront ID="ivSizeRangeMax" runat="server" ControlToValidate="txtSizeRangeMax" ErrorMessage="Field 'Size Range: Maximum' is invalid" ValidationGroup="BuilderFinance"></CC:IntegerValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvPriceRangeMin" runat="server" ControlToValidate="txtPriceRangeMin" ErrorMessage="Field 'Price Range: Minimum' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:FloatValidator ID="fvPriceRangeMin" runat="server" ControlToValidate="txtPriceRangeMin" ErrorMessage="Field 'Price Range: Minimum' is invalid" ValidationGroup="BuilderFinance"></CC:FloatValidator>
<CC:RequiredFieldValidatorFront ID="rfvPriceRangeMax" runat="server" ControlToValidate="txtPriceRangeMax" ErrorMessage="Field 'Price Range: Maximum' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:FloatValidator ID="fvPriceRangeMax" runat="server" ControlToValidate="txtPriceRangeMax" ErrorMessage="Field 'Price Range: Maximum' is invalid" ValidationGroup="BuilderFinance"></CC:FloatValidator>
<CC:RequiredFieldValidatorFront ID="rfvAvgPerFoot" runat="server" ControlToValidate="txtAvgPerFoot" ErrorMessage="Field 'Average Price/sqft' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:FloatValidator ID="fvAvgPerFoot" runat="server" ControlToValidate="txtAvgPerFoot" ErrorMessage="Field 'Average Price/sqft' is invalid" ValidationGroup="BuilderFinance"></CC:FloatValidator>
<CC:RequiredFieldValidatorFront ID="rfvRevenue" runat="server" ControlToValidate="txtRevenue" ErrorMessage="Field 'Company Revenue' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:FloatValidator ID="fvRevenue" runat="server" ControlToValidate="txtRevenue" ErrorMessage="Field 'Company Revenue' is invalid" ValidationGroup="BuilderFinance"></CC:FloatValidator>
<CC:RequiredFieldValidatorFront ID="rfvRevenueProjected" runat="server" ControlToValidate="txtRevenueProjected" ErrorMessage="Field 'Projected Company Revenue' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:FloatValidator ID="fvRevenueProjected" runat="server" ControlToValidate="txtRevenueProjected" ErrorMessage="Field 'Projected Company Revenue' is invalid" ValidationGroup="BuilderFinance"></CC:FloatValidator>
<CC:RequiredFieldValidatorFront ID="rfvCogs" runat="server" ControlToValidate="txtCogs" ErrorMessage="Field 'Total COGS' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:FloatValidator ID="fvCogs" runat="server" ControlToValidate="txtCogs" ErrorMessage="Field 'Total COGS' is invalid" ValidationGroup="BuilderFinance"></CC:FloatValidator>
<CC:RequiredFieldValidatorFront ID="rfvAreas" runat="server" ControlToValidate="txtAreas" ErrorMessage="Field 'Areas in which you build' is empty" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>
<CC:RequiredFieldValidatorFront ID="rfvTerms" runat="server" ControlToValidate="cbTerms" ErrorMessage="Field 'Accepted Terms' is invalid" ValidationGroup="BuilderFinance"></CC:RequiredFieldValidatorFront>










