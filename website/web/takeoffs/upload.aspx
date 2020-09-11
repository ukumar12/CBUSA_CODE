<%@ Page Language="VB" AutoEventWireup="false" CodeFile="upload.aspx.vb" Inherits="takeoffs_upload" %>

<CT:MasterPage ID="CTMain" runat="server">
    <div class="pckggraywrpr">
        <div class="pckghdgred">Upload Takeoff Products</div>
        <div id="divStepOne" runat="server">
            <b>Step 1: Upload File</b><br />
            <CC:FileUpload ID="fuImport" runat="server" Folder="/assets/takeoffs/" DisplayImage="false" />
        </div>
        <div id="divStepTwo" runat="server">
            <b>Step 2: Select Columns</b><br />
            <table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td>CBUSA SKU</td>
                    <td>Quantity</td>
                </tr>
                <tr>
                    <td><asp:DropDownList id="drpSku" runat="server"></asp:DropDownList></td>
                    <td><asp:DropDownList id="drpQty" runat="server"></asp:DropDownList></td>
                </tr>
            </table>
        </div>
        <div id="divStepThree"
    </div>
</CT:MasterPage>