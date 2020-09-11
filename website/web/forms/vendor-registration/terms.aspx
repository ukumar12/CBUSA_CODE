<%@ Page Language="VB" AutoEventWireup="false" CodeFile="terms.aspx.vb" Inherits="forms_vendor_registration_terms" %>

<CT:MasterPage ID="CTMain" runat="server">
<script type="text/javascript">
<!--

    function CheckTerms() {
    
        var rdoAgree = document.getElementById('rdoAgree');
        var btnSubmit = document.getElementById('btnSubmit');

        if (rdoAgree.checked)
            btnSubmit.disabled = false;
        else
            btnSubmit.disabled = true;

    }
    	
//-->
</script>
<br />

<table class="termsform">
    <col width="200px" />
    <col width="" />
    <tr>
        <td class="fieldlbl" colspan="2" >
            <div align="left">
                Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Ut iaculis. Nulla pharetra tempus pede. Fusce vel tortor. Suspendisse potenti. In hac habitasse platea dictumst. Curabitur convallis erat quis massa. Donec condimentum dui vel est auctor lobortis. Maecenas aliquet semper tellus. Quisque fermentum mollis justo. Proin libero velit, porttitor at, placerat a, lobortis eget, sem. Cras id purus. Sed semper vulputate nibh. Maecenas quis lectus. Morbi ac eros. Sed luctus lacinia dui. Etiam vel lacus at mauris fermentum adipiscing. Maecenas aliquam interdum pede. In sed lorem. Nullam in nulla sed urna cursus hendrerit. Duis vel urna eu ligula tincidunt semper. 
                <br />
                <br />
                Suspendisse potenti. Nam mattis, sapien in iaculis lobortis, dolor tortor rhoncus risus, vitae dictum lectus leo nec nisi. Integer eu magna ut augue imperdiet pretium. Ut tempus bibendum nisl. Etiam est. In sit amet ipsum. Maecenas facilisis tincidunt magna. Aenean sagittis, nibh vitae fringilla interdum, nunc diam auctor tellus, quis vestibulum lacus enim id ipsum. Nullam quis dolor. Sed non ipsum. Nam vel turpis. 
                <br />
                <br />
                Nullam justo. Ut auctor lacinia libero. Integer accumsan lorem in lacus. Maecenas elit quam, bibendum nec, viverra a, vehicula vitae, augue. In semper diam id magna. Nam est. Sed odio. In condimentum turpis eu nisl. Vestibulum interdum sapien id quam. Sed eget risus et quam lobortis iaculis. Nulla volutpat, metus id dapibus placerat, odio neque interdum dui, adipiscing placerat lorem erat vitae eros. Nulla a metus nec sapien varius semper. Proin sed leo. Praesent id turpis varius dui dictum aliquet. Praesent sodales, est ac hendrerit dictum, neque erat accumsan erat, sit amet interdum nulla mauris imperdiet odio. Duis et augue. Phasellus lobortis convallis nulla. Nullam vehicula scelerisque nunc. In eu nulla ac felis lacinia sollicitudin. Quisque lectus. 
                <br />
                <br />
                Aliquam erat volutpat. Curabitur diam. Cras viverra. Vivamus vitae lacus eu velit pretium venenatis. Integer sit amet lectus. Duis est. Donec ut lorem. Aliquam ut sapien eget nisi adipiscing suscipit. Nunc porttitor gravida dui. Donec tortor. Proin feugiat, sapien non condimentum eleifend, massa quam faucibus arcu, non ultrices velit sapien nec enim. Nam et leo at tortor bibendum commodo. Fusce nulla. Cras mauris odio, viverra ac, interdum ac, placerat vitae, nisi. Etiam ipsum odio, vehicula et, tristique et, luctus quis, mi. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. 
                <br />
                <br />
                Cras sagittis quam eu urna. Maecenas est urna, viverra a, feugiat eu, semper vel, odio. Aenean volutpat lectus sit amet magna. Phasellus convallis. Mauris hendrerit pellentesque enim. Praesent quam urna, fermentum accumsan, varius in, pellentesque interdum, augue. Nullam at massa a elit accumsan varius. Ut auctor nisl ut arcu. Praesent tellus dolor, dignissim vel, malesuada vitae, vestibulum a, nunc. Curabitur non tortor. Vivamus faucibus sodales dui. 
                <br />
                <br />
            </div>
        </td>
    </tr>
    <tr>
        <td><asp:RadioButton runat="server" id="rdoAgree" onclick="CheckTerms()" GroupName="TermsAgree" Value="1" Text="I agree to the following terms" ></asp:RadioButton></td>
        <td><asp:RadioButton runat="server" id="rdoNotAgree" onclick="CheckTerms()" GroupName="TermsAgree" Value="0" Checked="true" Text="I do not agree to the following terms" ></asp:RadioButton></td>
    </tr>
    <tr>
        
    </tr>
    <tr>
        <td class="fieldlbl" colspan="2">
            <p style="text-align:center;">
                <CC:OneClickButton ID="btnSubmit" runat="server" Text="Submit" Enabled="false" />
            </p>
        </td>
    </tr>

</CT:MasterPage>