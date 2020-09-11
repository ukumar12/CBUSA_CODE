Imports Components
Imports DataLayer
Imports System.Data
Imports System.Configuration.ConfigurationManager

Partial Class admin_CKEditor
    Inherits AdminPage

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not IsPostBack Then
            Dim s As String = ck.ID
            btnSave.Attributes.Add("onclick", "window.opener.document.getElementById('" & Request("ClientID") & "').value = document.getElementById('divCK').innerHTML;window.close();return false;")
            ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType, System.Guid.NewGuid.ToString, "document.getElementById('ck_ccEditor').value = window.opener.document.getElementById('" & Request("ClientID") & "').value;", True)
            phPreview.Visible = False
        End If
    End Sub

    Protected Sub btnPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPreview.Click
        phEdit.Visible = False
        phPreview.Visible = True
        ltl.Text = ck.Value
    End Sub

    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        phEdit.Visible = True
        phPreview.Visible = False
        ck.Value = ltl.Text
    End Sub
End Class
