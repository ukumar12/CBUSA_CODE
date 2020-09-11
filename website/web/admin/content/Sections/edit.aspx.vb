Imports System.Data.Sqlclient
Imports Components
Imports DataLayer
Imports System.IO

Partial Class edit
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then
            Exit Sub
        End If

        'Make sure that the path has been entered propery
        If Not Core.FolderExists(Server.MapPath(Folder.Text)) Then
            AddError("Entered folder doesn't exist")
            Exit Sub
        End If
        Dim FullPath As String = Server.MapPath(Folder.Text)
        FullPath = Replace(FullPath, Server.MapPath("/"), "").ToLower()
        FullPath = Replace(FullPath, "\", "/")
        FullPath = "/" & FullPath
        If Not URLMappingManager.IsValidFolder(FullPath) Then
            AddError("Cannot register system folder. Please try another folder")
            Exit Sub
        End If

        Dim dbSection As ContentToolSectionRow
        dbSection = New ContentToolSectionRow(DB)
        dbSection.SectionName = SectionName.Text
        dbSection.Folder = Folder.Text

        Try
            DB.BeginTransaction()
            ViewState("SectionId") = dbSection.AutoInsert()
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
			AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ViewState("SectionId") = Nothing
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
