<%@ Page Language="vb" AutoEventWireup="true" Inherits="Components.BasePage" %> 
<% @Import Namespace="System.Data" %>
<% @Import Namespace="System.Data.SqlClient" %>

<script language="vb" runat="server">
Sub Page_Load(sender as Object, e as EventArgs)
    Dim dr As SqlDataReader = DB.GetReader("select top 1 * from Sysparam")
    If dr.Read Then
        Response.Write("Good")
    End If
    dr.Close()
End Sub
</script>
