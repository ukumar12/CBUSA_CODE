Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected PaymentStatusID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("REGISTRATIONS")

        PaymentStatusID = Convert.ToInt32(Request("PaymentStatusID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If PaymentStatusID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPaymentStatus As PaymentStatusRow = PaymentStatusRow.GetRow(DB, PaymentStatusID)
        txtPaymentStatus.Text = dbPaymentStatus.PaymentStatus
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbPaymentStatus As PaymentStatusRow

            If PaymentStatusID <> 0 Then
                dbPaymentStatus = PaymentStatusRow.GetRow(DB, PaymentStatusID)
            Else
                dbPaymentStatus = New PaymentStatusRow(DB)
            End If
            dbPaymentStatus.PaymentStatus = txtPaymentStatus.Text

            If PaymentStatusID <> 0 Then
                dbPaymentStatus.Update()
            Else
                PaymentStatusID = dbPaymentStatus.Insert
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
        Response.Redirect("delete.aspx?PaymentStatusID=" & PaymentStatusID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

