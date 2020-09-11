Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected GiftMessageId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        GiftMessageId = Convert.ToInt32(Request("GiftMessageId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If GiftMessageId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbGiftMessage As GiftMessageRow = GiftMessageRow.GetRow(DB, GiftMessageId)
        txtGiftMessage.Text = Server.HtmlDecode(dbGiftMessage.GiftMessage)
        txtGiftMessageLabel.Text = dbGiftMessage.GiftMessageLabel
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbGiftMessage As GiftMessageRow

            If GiftMessageId <> 0 Then
                dbGiftMessage = GiftMessageRow.GetRow(DB, GiftMessageId)
            Else
                dbGiftMessage = New GiftMessageRow(DB)
            End If
            dbGiftMessage.GiftMessage = Server.HtmlEncode(txtGiftMessage.Text)
            dbGiftMessage.GiftMessageLabel = txtGiftMessageLabel.Text

            If GiftMessageId <> 0 Then
                dbGiftMessage.Update()
            Else
                GiftMessageId = dbGiftMessage.Insert
            End If

            DB.CommitTransaction()


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
        Response.Redirect("delete.aspx?GiftMessageId=" & GiftMessageId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

