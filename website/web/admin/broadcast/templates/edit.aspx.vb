Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected TemplateId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BROADCAST")

        TemplateId = Convert.ToInt32(Request("TemplateId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If TemplateId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbMailingTemplate As MailingTemplateRow = MailingTemplateRow.GetRow(DB, TemplateId)
        txtName.Text = dbMailingTemplate.Name
        txtHTMLMember.Text = dbMailingTemplate.HTMLMember
        txtTextMember.Text = dbMailingTemplate.TextMember
        txtHTMLDynamic.Text = dbMailingTemplate.HTMLDynamic
        txtTextDynamic.Text = dbMailingTemplate.TextDynamic
        fuImageName.CurrentFileName = dbMailingTemplate.ImageName
        fuImageName.DisplayImage = True
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMailingTemplate As MailingTemplateRow

            If TemplateId <> 0 Then
                dbMailingTemplate = MailingTemplateRow.GetRow(DB, TemplateId)
            Else
                dbMailingTemplate = New MailingTemplateRow(DB)
            End If
            dbMailingTemplate.Name = txtName.Text
            dbMailingTemplate.HTMLMember = txtHTMLMember.Text
            dbMailingTemplate.TextMember = txtTextMember.Text
            dbMailingTemplate.HTMLDynamic = txtHTMLDynamic.Text
            dbMailingTemplate.TextDynamic = txtTextDynamic.Text
            If fuImageName.NewFileName <> String.Empty Then
                fuImageName.SaveNewFile()
                dbMailingTemplate.ImageName = fuImageName.NewFileName
            ElseIf fuImageName.MarkedToDelete Then
                dbMailingTemplate.ImageName = Nothing
            End If

            If TemplateId <> 0 Then
                dbMailingTemplate.Update()
            Else
                TemplateId = dbMailingTemplate.Insert
            End If

            DB.CommitTransaction()

            If fuImageName.NewFileName <> String.Empty Or fuImageName.MarkedToDelete Then fuImageName.RemoveOldFile()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?TemplateId=" & TemplateId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

