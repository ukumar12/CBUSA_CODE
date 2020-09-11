Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected BuilderRegistrationPaymentID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDER_REGISTRATIONS")

        BuilderRegistrationPaymentID = Convert.ToInt32(Request("BuilderRegistrationPaymentID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If BuilderRegistrationPaymentID = 0 Then
            Exit Sub
        End If

        Dim dbBuilderRegistrationPayment As BuilderRegistrationPaymentRow = BuilderRegistrationPaymentRow.GetRow(DB, BuilderRegistrationPaymentID)

        Dim dbCardType As CreditCardTypeRow = CreditCardTypeRow.GetRow(DB, dbBuilderRegistrationPayment.CardTypeID)
        ltlCardType.Text = dbCardType.Name

        Dim dbpaymentstatus As PaymentStatusRow = PaymentStatusRow.GetRow(DB, dbBuilderRegistrationPayment.PaymentStatusID)
        ltlPaymentStatus.Text = dbpaymentstatus.PaymentStatus

        ltlSubmitted.Text = dbBuilderRegistrationPayment.Submitted

        txtCardholderName.Text = dbBuilderRegistrationPayment.CardholderName
        txtCardNumber.Text = dbBuilderRegistrationPayment.CardNumber
        txtExpirationDate.Text = dbBuilderRegistrationPayment.ExpirationDate
        txtReferenceNumber.Text = dbBuilderRegistrationPayment.ReferenceNumber
    End Sub


    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

