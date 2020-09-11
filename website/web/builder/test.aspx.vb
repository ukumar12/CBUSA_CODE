Imports Components
Imports DataLayer

Partial Class test
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            
        End If
    End Sub

    'Protected Sub frmSubstitute_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmSubstitute.Callback
    '    Dim json As New Web.Script.Serialization.JavaScriptSerializer
    '    Dim ret As String = json.Serialize(args.Data)
    '    frmSubstitute.CallbackResult = ret
    'End Sub
End Class
