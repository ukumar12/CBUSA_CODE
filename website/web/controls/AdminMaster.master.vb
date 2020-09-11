Partial Class AdminMaster
    Inherits System.Web.UI.MasterPage

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        MyBase.OnInit(e)
        Dim sm As ScriptManager = ScriptManager.GetCurrent(Me.Page)
        If sm Is Nothing Then
            sm = New ScriptManager
            sm.ID = "AjaxManager"
            Dim oForm As Control = Me.FindControl("main")
            If oForm IsNot Nothing AndAlso (TypeOf oForm Is Controls.Form Or TypeOf oForm Is HtmlForm) Then
                oForm.Controls.AddAt(0, sm)
            End If
        End If
    End Sub
End Class

