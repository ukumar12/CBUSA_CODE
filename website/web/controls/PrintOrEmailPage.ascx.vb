Imports Components
Imports Utility

Partial Class PrintOrEmailPage
    Inherits System.Web.UI.UserControl

    Protected PrintURL As String = String.Empty
    Protected SendURL As String = String.Empty

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim qs As URLParameters = New URLParameters(Request.QueryString, "print")
        PrintURL = Request.Path & qs.ToString("print", "y")
        SendURL = Request.Path & qs.ToString
    End Sub
End Class
