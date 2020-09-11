Imports Components

Partial Class Confirm
    Inherits AdminPage

    Protected Confirmation As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Confirmation = Session("Confirmation").ToString
    End Sub
End Class
