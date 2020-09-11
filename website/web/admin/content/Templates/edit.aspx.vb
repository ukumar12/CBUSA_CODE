Imports Components
Imports DataLayer
Imports System.Data.Sqlclient

Partial Class edit
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        If Not IsPostBack Then
            If Not Request("TemplateId") = Nothing Then
                ViewState("TemplateId") = Convert.ToInt32(Request("TemplateId"))
                LoadFromDB()
            End If
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dbTemplate As ContentToolTemplateRow
        If Not ViewState("TemplateId") Is Nothing Then
            dbTemplate = ContentToolTemplateRow.GetRow(DB, ViewState("TemplateId"))
        Else
            dbTemplate = New ContentToolTemplateRow(DB)
        End If
        TemplateName.Text = dbTemplate.TemplateName
        Content.Text = dbTemplate.TemplateHTML
        Print.Text = dbTemplate.PrintHTML
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim dbTemplate As ContentToolTemplateRow

        If ViewState("TemplateId") Is Nothing Then
            dbTemplate = New ContentToolTemplateRow(Me.DB)
        Else
            dbTemplate = ContentToolTemplateRow.GetRow(DB, ViewState("TemplateId"))
        End If
        dbTemplate.TemplateName = TemplateName.Text
        dbTemplate.TemplateHTML = Content.Text
        dbTemplate.PrintHTML = Print.Text
        Try
            DB.BeginTransaction()
            If ViewState("TemplateId") Is Nothing Then
                ViewState("TemplateId") = dbTemplate.AutoInsert()
            Else
                dbTemplate.Update()
            End If
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ex.Message + "<br/>" + ex.ToString)
        End Try
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

End Class