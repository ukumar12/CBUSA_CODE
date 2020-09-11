Imports System.Collections.Specialized
Imports System.Reflection
Imports System.Net
Imports System.Web
Imports System.IO
Imports VindiciaTest

Namespace Components

    Public Class VindiciaPaymentProcessor

        Protected Output As StringDictionary



        Private Function SubmitTransaction(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As VindiciaAddressInfo, ByVal Amount As Double) As Integer
            Dim account As New Transaction.Account
            Dim transaction As New Transaction.Transaction
            Dim payment As New Transaction.PaymentMethod
            Dim auth As New Transaction.Authentication

            auth.login = "cbusa_soap"
            auth.password = "YJYv2c1AeCZR56xdWTRknPeZXpFmrWmY"
            auth.version = "1.1"

            account.company = addr.Company
            account.emailAddress = addr.Email
            account.emailTypePreferenceSpecified = False
            account.merchantAccountId = addr.Guid
            account.name = Core.BuildFullName(addr.FirstName, String.Empty, addr.LastName)
            account.shippingAddress = New Transaction.Address
            With account.shippingAddress
                .addr1 = addr.AddressLn1
                .addr2 = addr.AddressLn2
                .city = addr.City
                .country = addr.Country
                .name = Core.BuildFullName(addr.FirstName, String.Empty, addr.LastName)
                .postalCode = addr.Zip
            End With

            payment.accountHolderName = cc.CardHolderName
            payment.billingAddress = account.shippingAddress
            payment.creditCard = New Transaction.CreditCard
            With payment.creditCard
                .account = cc.CreditCardNumber
                .expirationDate = cc.ExpYear.ToString & cc.ExpMonth.ToString
            End With

            transaction.account = account
            transaction.amount = Amount
            transaction.merchantTransactionId = Unique
            transaction.shippingAddress = account.shippingAddress
            transaction.sourceIp = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            transaction.sourcePaymentMethod = payment

            Dim client As New Transaction.TransactionPortTypeClient
            Dim ret As Transaction.Return = client.auth(auth, transaction, False, 100)
            Return ret.returnCode
        End Function

        Public Function SubmitSale(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As VindiciaAddressInfo, ByVal Amount As Double) As Boolean


        End Function

        Private Function ConvertExpDate(ByVal m As Integer, ByVal y As Integer) As String
            Dim result As String = m & Right(y, 2)
            If Len(result) = 3 Then result = "0" & result
            Return result
        End Function

        Public Function SubmitAuthorization(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean

        End Function

        Private Function GetParams(ByVal Input As StringDictionary) As String
            Dim Result As String = String.Empty
            For Each key As String In Input.Keys
                Dim k As String = key
                Dim v As String = Input(key)

                If Result.Length > 0 Then
                    Result = Result & "&"
                End If
                Result = Result & k.ToUpper() & "=" & v
            Next
            Return Result
        End Function

        Private Function ParseResults(ByVal Result As String) As StringDictionary
            Dim Dictionary As StringDictionary = New StringDictionary


            Return Dictionary
        End Function

        Public Function GetVerificationResponse() As String
            Dim ErrorMessage As String = String.Empty
            Select Case Output("AVS Code")
                Case "A"
                    ErrorMessage = "Address (Street) matches, ZIP does not."
                Case "N"
                    ErrorMessage = " No Match on Address (Street) or ZIP."
                Case "W"
                    ErrorMessage = " 9 digit ZIP matches, Address (Street) does not."
                Case "Z"
                    ErrorMessage = " 5 digit ZIP matches, Address (Street) does not."
                Case Else
            End Select
            Select Case Output("Card Code Response Code")
                Case "N"
                    ErrorMessage &= " No Match on Card Code"
            End Select
            Return ErrorMessage
        End Function

        Public Function SubmitCredit(ByVal TransactionNo As String, ByVal Amount As Double, Optional ByVal cc As CreditCardInfo = Nothing) As Boolean

        End Function

        Public Function SubmitVoid(ByVal TransactionNo As String) As Boolean

        End Function

        Public ReadOnly Property ErrorMessage() As String
            Get
                Return Output("Response Reason Text")
            End Get
        End Property

        Public ReadOnly Property TransactionNo() As String
            Get
                Return Output("Transaction ID")
            End Get
        End Property

        Public ReadOnly Property Result() As Integer
            Get
                Return Output("Response Code")
            End Get
        End Property

        Public ReadOnly Property FullResponse() As String
            Get
                Dim Result As String = String.Empty
                For Each key As String In Output.Keys
                    Dim k As String = key
                    Dim v As String = Output(key)
                    If Not v = String.Empty Then
                        Result = Result & k & "=" & v & vbCrLf
                    End If
                Next
                Return Result
            End Get
        End Property
    End Class

    Public Class VindiciaAddressInfo
        Public Guid As String
        Public Company As String
        Public FirstName As String
        Public LastName As String
        Public AddressLn1 As String
        Public AddressLn2 As String
        Public City As String
        Public State As String
        Public Zip As String
        Public Country As String
        Public Email As String
    End Class

End Namespace