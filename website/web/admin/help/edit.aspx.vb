Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected TextId As Integer
    Private previousurl As String = String.Empty

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckInternalAccess("CONTENT_TOOL")
        TextId = Convert.ToInt32(Request("TextId"))
        If TextId <> 0 Then
            txtCode.Visible = False
            rfvCode.Visible = False
            lblCode.Visible = True
        End If
        If Not IsPostBack Then
            ViewState("RefURL") = Request.QueryString("RedirectUrl") & "&" & GetPageParams(FilterFieldType.All)
            LoadFromDB()
        End If
        previousurl = ViewState("RefURL").ToString()
        If previousurl.Contains("/admin/help/") Then
            btnDelete.Visible = True
        Else
            btnDelete.Visible = False
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dbCustomText As CustomTextRow = CustomTextRow.GetRow(DB, TextId)
        If TextId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If
        lblCode.Text = dbCustomText.Code
        txtTitle.Text = dbCustomText.Title
        txtValue.Value = dbCustomText.Value
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbCustomText As CustomTextRow

            If TextId <> 0 Then
                dbCustomText = CustomTextRow.GetRow(DB, TextId)
            Else
                dbCustomText = New CustomTextRow(DB)
                dbCustomText.Code = txtCode.Text
            End If

            dbCustomText.Title = txtTitle.Text
            dbCustomText.Value = txtValue.Value
            dbCustomText.IsHelpTag = 1

            If TextId <> 0 Then
                dbCustomText.Update()
            Else
                TextId = dbCustomText.Insert
            End If

            DB.CommitTransaction()

            If previousurl <> String.Empty Then
                Response.Redirect(previousurl)
            Else
                Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
            End If
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If previousurl <> String.Empty Then
            Response.Redirect(previousurl)
        Else
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?TextId=" & TextId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

