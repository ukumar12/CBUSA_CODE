Imports Components
Imports DataLayer

Partial Class directory_piq
    Inherits SitePage

    Private PIQId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PIQId = Request("piqid")
        Dim dbPiq As PIQRow = PIQRow.GetRow(DB, PIQId)
        If dbPiq.PIQID = Nothing Then
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
        End If

        If Not IsPostBack Then
            divProgram.InnerHtml = dbPiq.IncentivePrograms
        End If
    End Sub

    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
