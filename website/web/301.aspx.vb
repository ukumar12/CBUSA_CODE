Imports System.Data.SqlClient
Imports Controls
Imports Components

Partial Class _301
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim dbCustomURLHistory As DataLayer.CustomURLHistoryRow = DataLayer.CustomURLHistoryRow.GetFromCustomURL(DB, Request.RawUrl)
        If dbCustomURLHistory.CustomURLHistoryId <> 0 Then
            Response.Status = "301 Permanently Moved"
            Response.AddHeader("Location", dbCustomURLHistory.RedirectURL)
        End If
    End Sub
End Class
