Imports DataLayer
Imports Components
Imports System.Configuration.ConfigurationManager

Partial Class Confirm
    Inherits AdminPage

    Protected dbPage As ContentToolPageRow
    Protected GlobalRefererName As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        dbPage = ContentToolPageRow.GetRow(DB, CInt(Request("PageId")))
        GlobalRefererName = AppSettings("GlobalRefererName")
    End Sub
End Class
