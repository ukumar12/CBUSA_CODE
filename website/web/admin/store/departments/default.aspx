<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/controls/AdminMaster.master" Title="" CodeFile="default.aspx.vb" Inherits="_default"  %>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">

<script language="JavaScript" type="text/javascript">
<!--
    var ns6=document.getElementById&&!document.all?1:0

    var head="display:''"
    var department=''

    function expandit(prefix, obj, objid){
        department=document.getElementById(prefix + 'SPAN' + objid).style;
        img = document.getElementById(prefix + 'IMG' + objid);
        fld = document.getElementById(prefix + 'FLD' + objid);
    	
        if (department.display=="none") {
	        department.display="block"
	        img.src = img.src.replace(/plus/i, "minus");
	        fld.src = fld.src.replace(/plus/i, "minus");
        } else {	
	        department.display="none"
	        img.src = img.src.replace(/minus/i, "plus");
	        fld.src = fld.src.replace(/minus/i, "plus");
        }	
    }

    function Add() {
        document.aspnetForm.ACTION.value = 'ADD';
        if (ValidateForm(document.aspnetForm)) return true;
        return false;
    }

    function Rename() {
        document.aspnetForm.ACTION.value = 'RENAME';
        if (ValidateForm(document.aspnetForm)) return true;
        return false;
    }

    function Delete() {
        document.aspnetForm.ACTION.value = 'DELETE';
        if (ValidateForm(document.aspnetForm)) return true;
        return false;
    }

    function Move() {
        document.aspnetForm.ACTION.value = 'MOVE';
        if (ValidateForm(document.aspnetForm)) return true;
        return false;
    }

    function ValidateForm(oForm) {
        // Validate form here
        var sAction = oForm.ACTION.value;

        if (sAction == 'ADD') {
	        if (isEmptyField(oForm.DepartmentId)) {
		        alert("Please select the Parent Department");
		        return false;
	        }
	        if (isEmptyField(document.getElementById('<%= NewDepartment.ClientId%>'))) {
		        alert("Please specify in the New Department Name");
		        document.getElementById('<%= NewDepartment.ClientId%>').focus();
		        return false;
	        }
        } else if (sAction == 'RENAME') {
	        if (isEmptyField(oForm.DepartmentId)) {
		        alert("Please select the Department to Rename");
		        return false;
	        }
	        if (isEmptyField(document.getElementById('<%= NAME.ClientId%>'))) {
		        alert("Please provide new Department Name");
		        document.getElementById('<%= NAME.ClientId%>').focus();
		        return false;
	        }
        } else if (sAction == 'DELETE') {
	        if (isEmptyField(oForm.DepartmentId)) {
		        alert("Please select the Department to Delete");
		        return false;
	        }
	        if (!confirm("Are you sure you want to delete this Department?")) return false;

        } else if (sAction == 'MOVE') {
    		
	        if(!eval(oForm.SourceId)) {
		        alert("There is no department to move (Main department cannot be moved)");
		        return false;
	        }
	        if (isEmptyField(oForm.SourceId)) {
		        alert("Please select Source Department to Move");
		        return false;
	        }
	        if (isEmptyField(oForm.TargetId)) {
		        alert("Please select Destination Department/Class");
		        return false;
	        }

        } else {
	        alert("Please chose one: Add/Rename/Delete or Move action");
	        return false;
        }
        return true;
    }
//-->
</script>

<input type="hidden" name="ACTION" />
<h4>Store Department/Class Administration</h4>
<p>
    <b>Add/Rename/Delete Departments/Classes</b>
    <br/>
    <font class="desc">* Departments with number in parentheses next to the name cannot be deleted, because contain items. </font></p>

    <table width="100%" border="0">
        <tr>
            <td valign="top" width="50%">
                <asp:Literal ID="F" Runat="server" />
            </td>
            <td valign="top" width="50%">
                <table border="0" cellpadding="2" cellspacing="0" class="view" width="280">
                    <tr>
                        <td class="row" colspan="2"><b>Add New Department/Class</b>&nbsp;<br>
                            <font class="desc">(Please select the parent department on the left)</font></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td><asp:TextBox Text="" id="NewDepartment" Runat="server" />
                        &nbsp;
                        <CC:OneClickButton runat="server" ID="btnAdd" Text="Add" cssclass="btn" />
                        </td>
                    </tr>
                </table>
                <p>
                    <table border="0" cellpadding="2" cellspacing="0" class="view" width="280">
                        <tr class="row">
                            <td colspan="2"><b>Rename Department/Class</b>&nbsp;<br>
                                <font class="desc">(Please select the department on the left)</font></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>Department/Class Name:<br>
                                <asp:TextBox Text="" id="NAME" columns="20" maxlength="50" Runat="server" />
                                &nbsp;
                                <CC:OneClickButton id="btnRename" cssclass="btn" Text="Rename" Runat="server" />
                            </td>
                        </tr>
                    </table>
                </p><p>
                    <table border="0" cellpadding="2" cellspacing="0" class="view" width="280">
                        <tr class="row">
                            <td colspan="2"><b>Delete Department/Class</b>&nbsp;<br>
                                <font class="desc">(Please select the department on the left)</font></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>Delete selected department/class&nbsp;
                                <CC:OneClickButton id="btnDelete" cssclass="btn" Text="Delete" Runat="server" />
                            </td>
                        </tr>
                    </table>
                </p>
            </td>
        </tr>
    </table>

    <p/>
    <hr size=1 color=black>
    <p/>
    <b>Move Departments/Classes</b>
    <br>
    <font class="desc">Please select source and destination department/class and then click 
        the </font>&nbsp;<CC:OneClickButton ID="btnMove" cssclass="btn" value="Move" Runat="server" Text="Move" />
    <font class="desc">button</font>
<p>
    <table width="100%" border="0">
        <tr>
            <td valign="top" width="50%">
                <b>Source Department/Class</b><br>
                <asp:Literal ID="S" Runat="server" />
            </td>
            <td valign="top" width="50%">
                <b>Destination Department/Class</b><br>
                <asp:Literal ID="T" Runat="server" />
            </td>
        </tr>
    </table>
    <br />
    &nbsp; </TD></TR></TABLE></p>
</asp:content>
