Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

	BindList()
    End Sub

    Private Sub BindList()
	If Not Session("QueryStr") Is Nothing Then
	        Dim res As DataTable = DB.GetDataTable("Select " & Session("QueryStr").ToString().Substring(Session("QueryStr").ToString().IndexOf("*") - 1))
        	gvList.DataSource = res.DefaultView
	        gvList.DataBind()

	        Response.Clear()
		'might cause problems depending on the version of ie. (http://classicasp.aspfaq.com/files/directories-fso/how-do-i-send-the-correct-filename-with-binarywrite.html)
		Response.AddHeader("Content-Disposition", "attachment;filename=export.xls")
	        Response.ContentType = "application/vnd.ms-excel"

	End If
    End Sub

End Class

