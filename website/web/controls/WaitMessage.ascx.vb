Imports Components

Partial Class controls_WaitMessage
    Inherits BaseControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.ClientScript.IsClientScriptBlockRegistered("WaitMessage") Then
            Dim s As String = _
                  " function OpenWait(ctl,msg) {" _
                & "     " _
                & " }"
        End If
    End Sub
End Class
