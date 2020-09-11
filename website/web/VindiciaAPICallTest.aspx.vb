Imports System.Data.SqlClient
Imports Components
Imports DataLayer
Imports Vindicia

Partial Class VindiciaAPICallTest
    Inherits SitePage

    Protected Processor As VindiciaPaymentProcessor
    Private dbBuilder As BuilderRow

    Protected currentAutoBill As Vindicia.AutoBill

    Private taxAmount As Decimal = 0.0
    Private totalAmount As Decimal = 0.0

    Protected dtCatchupStartDate As DateTime = Now.Date

    Private Sub btnRunProcess_Click(sender As Object, e As EventArgs) Handles btnRunProcess.Click

        UpdateBillingDay()

        'dbBuilder = BuilderRow.GetRow(DB, 24557)

        'Processor = New VindiciaPaymentProcessor(DB)
        'Processor.IsTestMode = SysParam.GetValue(DB, "TestMode")

        'Response.Write("Starting Process for " & dbBuilder.CompanyName & ". . . " & Now.ToLongTimeString() & "<br />")
        'Response.Write("===========================================================================================<br />")

        'FetchBillingPlans()
        'FetchBuilderAutoBills()
        'FetchBuilderTransactionHistory()
        'CalcCatchUpBilling()

    End Sub

    Private Sub UpdateBillingDay()

        Processor = New VindiciaPaymentProcessor(DB)
        Processor.IsTestMode = SysParam.GetValue(DB, "TestMode")

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, 24557)
        Dim autoBills() As Vindicia.AutoBill = Processor.GetAutobills(dbBuilder)

        If Not autoBills Is Nothing Then
            For Each ab As Vindicia.AutoBill In autoBills
                Dim Product As String = ab.items(0).product.merchantProductId
                If Product = "Subscription" Then
                    If ab.status = Vindicia.AutoBillStatus.Active Or ab.status = Vindicia.AutoBillStatus.PendingActivation Then
                        Dim Tran As Vindicia.Transaction = Nothing
                        Dim Ref As Vindicia.Refund() = Nothing

                        Dim arrABIM() As Vindicia.AutoBillItemModification = Nothing
                        ReDim arrABIM(0)
                        Dim iItemCounter As Int16 = 0

                        Dim sitem As New Vindicia.AutoBillItem

                        Dim ret As Vindicia.Return
                        Try
                            '****************************** FOR CHANGING BILLING DAY OF THE MONTH ***************************
                            ab.billingDay = CDate(Now.Date).Day
                            ret = ab.changeBillingDayOfMonth(CDate(Now.Date).Day, Now.Date.Day, Now.Date.AddMonths(1), False, Nothing, False, "")
                            '************************************************************************************************

                            Response.Write(ret.returnString)
                        Catch ex As Exception

                        End Try

                    End If
                End If
            Next
        End If

    End Sub

    Private Sub FetchBillingPlans()

        Dim startTime As DateTime = Now()
        Response.Write("Starting FetchBillingPlans(). . . " & Now.ToLongTimeString() & "<br />")

        Dim plans As Vindicia.BillingPlan() = Processor.GetBillingPlans()

        Dim endTime As DateTime = Now()
        Response.Write("FetchBillingPlans() Ended. Time Elapsed = " & DateDiff(DateInterval.Second, startTime, endTime) & " seconds <br />")
        Response.Write("===========================================================================================<br />")

    End Sub

    Private Sub FetchBuilderAutoBills()

        Dim startTime As DateTime = Now()
        Response.Write("Starting GetAutobills(). . . " & Now.ToLongTimeString() & "<br />")

        Dim autoBills() As Vindicia.AutoBill = Processor.GetAutobills(dbBuilder)

        For Each ab As AutoBill In autoBills
            Response.Write("<br />****************************************<br />")
            Response.Write(ab.merchantAutoBillId & " | ")
            Response.Write(ab.status.ToString() & " | ")
            Response.Write(ab.billingPlan.merchantBillingPlanId & " | ")
            Response.Write(ab.startTimestamp.ToString() & " | ")
            Response.Write("<br />****************************************<br />")
        Next

        Dim endTime As DateTime = Now()
        Response.Write("GetAutobills() Ended. Time Elapsed = " & DateDiff(DateInterval.Second, startTime, endTime) & " seconds <br />")
        Response.Write("===========================================================================================<br />")

    End Sub

    Private Sub FetchBuilderTransactionHistory()

        Dim startTime As DateTime = Now()
        Response.Write("Starting GetTransactionHistory(). . . " & Now.ToLongTimeString() & "<br />")

        Dim transactionHistory() As Vindicia.Transaction = Processor.GetTransactionHistory(dbBuilder)

        For Each trn As Transaction In transactionHistory
            Response.Write("<br />****************************************<br />")
            Response.Write(trn.merchantTransactionId & " | ")
            'Response.Write(trn.timestamp & " | ")
            'Response.Write(trn.merchantAutobillIdentifier & " | ")
            'Response.Write(trn.amount & " | ")
            Response.Write("<br />****************************************<br />")
        Next

        Dim endTime As DateTime = Now()
        Response.Write("GetTransactionHistory() Ended. Time Elapsed = " & DateDiff(DateInterval.Second, startTime, endTime) & " seconds <br />")
        Response.Write("===========================================================================================<br />")

    End Sub

    Private Sub CalcCatchUpBilling()

        dtCatchupStartDate = New Date(Now.Year, 4, 1)

        Dim startTime As DateTime = Now()
        Response.Write("Starting CalculateCatchUpBilling(). . . " & Now.ToLongTimeString() & "<br />")

        Dim currentAutoBill As Vindicia.AutoBill = Processor.GetAutobills(dbBuilder).LastOrDefault()

        If Not currentAutoBill Is Nothing And dtCatchupStartDate.Date < Now.Date Then
            Dim dtBackBill As DateTime = dtCatchupStartDate.Date
            Do While dtBackBill.Date < Now.Date
                Dim sPeriod As Vindicia.BillingPlanPeriod = Nothing
                For Each amount In currentAutoBill.billingPlan.periods
                    If amount.prices IsNot Nothing Then
                        sPeriod = amount
                        Exit For
                    End If
                Next
                Select Case sPeriod.type
                    Case Vindicia.BillingPeriodType.Day
                        dtBackBill = dtBackBill.AddDays(sPeriod.quantity)

                    Case Vindicia.BillingPeriodType.Month
                        dtBackBill = dtBackBill.AddMonths(sPeriod.quantity)

                    Case Vindicia.BillingPeriodType.Week
                        dtBackBill = dtBackBill.AddDays(sPeriod.quantity * 7)

                    Case Vindicia.BillingPeriodType.Year
                        dtBackBill = dtBackBill.AddYears(sPeriod.quantity)

                End Select
                taxAmount += Processor.CalculateTax(dbBuilder, 1, sPeriod.prices(0).amount)
                totalAmount += sPeriod.prices(0).amount + taxAmount
            Loop
        End If

        Dim endTime As DateTime = Now()
        Response.Write("CalculateCatchUpBilling() Ended. Time Elapsed = " & DateDiff(DateInterval.Second, startTime, endTime) & " seconds <br />")
        Response.Write("===========================================================================================<br />")

    End Sub

End Class
