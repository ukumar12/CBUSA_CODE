Option Strict Off

Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports Components

Partial Class AddModule
    Inherits AdminPage

    Protected ContentId As Integer
    Protected dbContent As ContentToolContentRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        If Not IsPostBack Then
            'Read ContentId from Request (it may be blank when inserting new item)
            ContentId = Request("ContentId")
        Else
            'On postback, we have ContentId saved in the Viewstate
            ContentId = ViewState("ContentId")
        End If

        'Initialize object
        dbContent = ContentToolContentRow.GetRow(DB, ContentId)
        If dbContent Is Nothing Then
            dbContent = New ContentToolContentRow(DB)
        End If

        If Not IsPostBack Then
            FCKeditor.Value = dbContent.Content
        End If
    End Sub

    Protected Sub btnPreview_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPreview.Click
        divEditor.Visible = False
        divPreview.Visible = True

        If drpPadding.SelectedValue = "1" Then
            dbContent.Content = "<div class=""cblock"">" & FCKeditor.Value & "</div>"
        Else
            dbContent.Content = FCKeditor.Value
        End If

        'Insert only when the preview button is submitted first time
        'Each next attempt should update the same record via Update() function
        If ViewState("ContentId") Is Nothing Then
            dbContent.AutoInsert()
            ViewState("ContentId") = dbContent.ContentId
        Else
            dbContent.Update()
        End If

        'Generate onClick javascript code
        btnInsertModule.Attributes("onClick") = "window.opener.document.getElementById('__CommandArgument').value = '" & dbContent.ContentId & "'; window.opener.__doPostBack('" & Request("ClientId") & "',''); window.close();"
    End Sub

    Protected Sub btnMakeChanges_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnMakeChanges.Click
        divEditor.Visible = True
        divPreview.Visible = False
    End Sub
End Class
