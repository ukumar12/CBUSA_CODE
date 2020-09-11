Option Strict Off

Imports System.Data
Imports System.Data.Sqlclient
Imports Components
Imports DataLayer
Imports System.IO

Partial Class register
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        LoadDropDownList()
    End Sub

    Private Sub LoadDropDownList()
        Dim SQL As String = ""

        If IsPostBack Then
            Exit Sub
        End If

        'Bind templates
        SQL = "SELECT TemplateId, TemplateName FROM ContentToolTemplate order by TemplateName"
        Dim dsTemplates As DataTable = DB.GetDataTable(SQL)
        TemplateId.DataValueField = "TemplateId"
        TemplateId.DataTextField = "TemplateName"
        TemplateId.DataSource = dsTemplates
        TemplateId.DataBind()
        TemplateId.Items.Insert(0, New ListItem("", ""))
    End Sub

    Public Function FileExists(ByVal FileFullPath As String) As Boolean
        Dim f As New IO.FileInfo(FileFullPath)
        Return f.Exists
    End Function

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then
            Exit Sub
        End If

        Dim FullPath As String = Server.MapPath(PageURL.Text)

        'Make sure that the path has been entered propery
        If Not FileExists(FullPath) Then
            AddError("Entered file doesn't exist")
            Exit Sub
        End If

        FullPath = Replace(FullPath, Server.MapPath("/"), "").ToLower()
        FullPath = Replace(FullPath, "\", "/")
        FullPath = "/" & FullPath

        Dim SectionId As Integer = GetSectionId(FullPath)
        If SectionId = 0 Then
            AddError("Cannot find section for the registered page. Please register the section and try again.")
            Exit Sub
        End If
        If Not URLMappingManager.IsValidFolder(FullPath) Then
            AddError("Cannot register new content tool page to system folder. Please try another folder")
            Exit Sub
        End If

        Try
            DB.BeginTransaction()

            Dim dbPage As New ContentToolPageRow(DB)
            dbPage.PageURL = FullPath
            dbPage.TemplateId = CInt(TemplateId.SelectedValue)
            dbPage.SectionId = SectionId
            dbPage.Name = PageTitle.Text
            dbPage.Title = PageTitle.Text
            dbPage.MetaKeywords = SysParam.GetValue(DB, "DefaultMetaKeywords")
            dbPage.MetaDescription = SysParam.GetValue(DB, "DefaultMetaDescription")
            dbPage.IsIndexed = True
            dbPage.IsFollowed = True
            dbPage.AutoInsert()
            DB.CommitTransaction()

            Response.Redirect("/admin/content/edit.aspx?PageId=" & dbPage.PageId)

        Catch ex As SqlException
			AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Function GetSectionId(ByVal FullPath As String) As Integer
        Dim bExit As Boolean = False
        Dim Index As Integer = InStrRev(FullPath, "/")

        While Not bExit
            Dim Folder As String = Mid(FullPath, 1, Index)
            Dim SQL As String = "select SectionId from ContentToolSection where Folder = " & DB.Quote(Folder)
            Dim SectionId As Integer = DB.ExecuteScalar(SQL)

            If Not SectionId = Nothing Then
                Return SectionId
            End If

            If Index > 1 Then
                Index = InStrRev(FullPath, "/", Index - 1)
            Else
                bExit = True
            End If
        End While

        Return 0
    End Function

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
