Imports System.Collections.Specialized
Imports System.Reflection
Imports System.Net
Imports System.Web
Imports System.IO

Namespace Components

    'Test Cards (success): 
    '370000000000002  - AmEx
    '6011000000000012 - Discover
    '5424000000000015 - MC
    '4007000000027    - Visa

    'Test Card (failure): 
    '4222222222222 and use the response reason code desired as the x_amount in currency format
    Public Class AuthorizeNetPaymentProcessor
        Inherits PaymentProcessor

        Private Function SubmitTransaction(ByVal Input As StringDictionary) As Integer
            Dim Host As String

            'populate the rest of the input dictionary with values that dont need to change
            Input("x_login") = Username
            Input("x_tran_key") = Password
            Input("x_version") = "3.1"

            If TestMode Then
                Host = "https://test.authorize.net/gateway/transact.dll"
                Input("x_test_request") = "TRUE"
            Else
                Host = "https://secure.authorize.net/gateway/transact.dll"
                Input("x_test_request") = "FALSE"
            End If

            Input("x_delim_data") = "TRUE"
            Input("x_delim_char") = "|"
            Input("x_relay_response") = "FALSE"
            Input("x_customer_ip") = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR").ToString

            Dim Params As String = GetParams(Input)

            Dim objRequest As HttpWebRequest = CType(WebRequest.Create(Host), HttpWebRequest)
            objRequest.Method = "POST"
            objRequest.ContentType = "application/x-www-form-urlencoded"
            objRequest.ContentLength = Params.Length

            Dim myWriter As StreamWriter = Nothing
            Try
                myWriter = New StreamWriter(objRequest.GetRequestStream())
                myWriter.Write(Params)
            Catch ex As WebException
                Throw New ApplicationException("The website timed out when attempting to connect payment processor. Please try again.")
            Finally
                myWriter.Close()
            End Try

            Dim objResponse As HttpWebResponse = CType(objRequest.GetResponse(), HttpWebResponse)
            Dim sr As New StreamReader(objResponse.GetResponseStream())
            Dim Results As String = sr.ReadToEnd()

            ' Close and clean up the StreamReader
            sr.Close()

            'parse returned results and return the response code
            If Results.ToLower.IndexOf("<html") = 0 Then
                'this is incase there was a connection error
                Output = New StringDictionary
                Output.Add("Response Reason Text", Results)
                Output.Add("Response Code", "3")
            Else
                Output = ParseResults(Results)
            End If
            Return Int32.Parse(Output("Response Code"))
        End Function

        Public Overrides Function SubmitSale(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean
            Dim Input As New StringDictionary

            Input("x_card_num") = RemoveSpecialCharacters(cc.CreditCardNumber)
            Input("x_exp_date") = ConvertExpDate(cc.ExpMonth, cc.ExpYear)
            Input("x_amount") = Amount.ToString
            Input("x_first_name") = cc.CardHolderName
            Input("x_card_code") = cc.CID
            Input("x_address") = addr.AddressLn1
            Input("x_city") = addr.City
            Input("x_state") = addr.State
            Input("x_zip") = RemoveSpecialCharacters(addr.Zip)
            Input("x_country") = addr.Country
            Input("x_po_num") = Unique
            Input("x_type") = "AUTH_CAPTURE"
            Input("x_method") = "CC"

            Return 1 = SubmitTransaction(Input)
        End Function

        Private Function ConvertExpDate(ByVal m As Integer, ByVal y As Integer) As String
            Dim result As String = m & Right(y, 2)
            If Len(result) = 3 Then result = "0" & result
            Return result
        End Function

        Public Overrides Function SubmitAuthorization(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean
            Dim Input As New StringDictionary

            Input("x_card_num") = RemoveSpecialCharacters(cc.CreditCardNumber)
            Input("x_exp_date") = ConvertExpDate(cc.ExpMonth, cc.ExpYear)
            Input("x_amount") = Amount.ToString
            Input("x_first_name") = cc.CardHolderName
            Input("x_card_code") = cc.CID
            Input("x_address") = addr.AddressLn1
            Input("x_city") = addr.City
            Input("x_state") = addr.State
            Input("x_zip") = RemoveSpecialCharacters(addr.Zip)
            Input("x_country") = addr.Country
            Input("x_po_num") = Unique
            Input("x_type") = "AUTH_ONLY"
            Input("x_method") = "CC"

            Return 1 = SubmitTransaction(Input)
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
            Dim Buffer() As String = Result.Split("|")

            Dictionary.Add("Response Code", Buffer(0))
            Dictionary.Add("Response Sub-Code", Buffer(1))
            Dictionary.Add("Response Reason Code", Buffer(2))
            Dictionary.Add("Response Reason Text", Buffer(3))
            Dictionary.Add("Approval Code", Buffer(4))
            Dictionary.Add("AVS Code", Buffer(5))
            Dictionary.Add("Transaction ID", Buffer(6))
            Dictionary.Add("x_invoice_num", Buffer(7))
            Dictionary.Add("x_description", Buffer(8))
            Dictionary.Add("x_amount", Buffer(9))
            Dictionary.Add("x_method", Buffer(10))
            Dictionary.Add("x_type", Buffer(11))
            Dictionary.Add("x_cust_id", Buffer(12))
            Dictionary.Add("x_first_name", Buffer(13))
            Dictionary.Add("x_last_name", Buffer(14))
            Dictionary.Add("x_company", Buffer(15))
            Dictionary.Add("x_address", Buffer(16))
            Dictionary.Add("x_city", Buffer(17))
            Dictionary.Add("x_state", Buffer(18))
            Dictionary.Add("x_zip", Buffer(19))
            Dictionary.Add("x_country", Buffer(20))
            Dictionary.Add("x_phone", Buffer(21))
            Dictionary.Add("x_fax", Buffer(22))
            Dictionary.Add("x_email", Buffer(23))
            Dictionary.Add("x_ship_to_first_name", Buffer(24))
            Dictionary.Add("x_ship_to_last_name", Buffer(25))
            Dictionary.Add("x_ship_to_company", Buffer(26))
            Dictionary.Add("x_ship_to_address", Buffer(27))
            Dictionary.Add("x_ship_to_city", Buffer(28))
            Dictionary.Add("x_ship_to_state", Buffer(29))
            Dictionary.Add("x_ship_to_zip", Buffer(30))
            Dictionary.Add("x_ship_to_country", Buffer(31))
            Dictionary.Add("x_tax", Buffer(32))
            Dictionary.Add("x_duty", Buffer(33))
            Dictionary.Add("x_freight", Buffer(34))
            Dictionary.Add("x_tax_exempt", Buffer(35))
            Dictionary.Add("x_po_num", Buffer(36))
            Dictionary.Add("x_md5_hash", Buffer(37))
            Dictionary.Add("Card Code Response Code", Buffer(38))
            Dictionary.Add("CAVV Response Code", Buffer(39))

            Return Dictionary
        End Function

        Public Overrides Function GetVerificationResponse() As String
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

        Public Overrides Function SubmitCredit(ByVal TransactionNo As String, ByVal Amount As Double, Optional ByVal cc As CreditCardInfo = Nothing) As Boolean
            Dim Input As New StringDictionary

            Input("x_amount") = Amount.ToString
            Input("x_trans_id") = TransactionNo
            Input("x_type") = "CREDIT"

            If Not cc Is Nothing Then
                'auth.net requires the last 4 digits of the credit card number in order to do a credit
                'auth.net api standard is 4 X's and last 4 digits of the cc number
                Input("x_card_num") = "XXXX" & Right(RemoveSpecialCharacters(cc.CreditCardNumber), 4)
            End If

            Return 1 = SubmitTransaction(Input)
        End Function

        Public Overrides Function SubmitVoid(ByVal TransactionNo As String) As Boolean
            Dim Input As New StringDictionary

            Input("x_trans_id") = TransactionNo
            Input("x_type") = "VOID"

            Return 1 = SubmitTransaction(Input)
        End Function

        Public Overrides ReadOnly Property ErrorMessage() As String
            Get
                Return Output("Response Reason Text")
            End Get
        End Property

        Public Overrides ReadOnly Property TransactionNo() As String
            Get
                Return Output("Transaction ID")
            End Get
        End Property

        Public Overrides ReadOnly Property Result() As Integer
            Get
                Return Output("Response Code")
            End Get
        End Property

        Public Overrides ReadOnly Property FullResponse() As String
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

        Public ReadOnly Property ResponseCode() As AuthorizeNetResponseCode
            Get
                If Output("Response Code") = String.Empty Then
                    Return AuthorizeNetResponseCode.TransactionError
                Else
                    Return Int32.Parse(Output("Response Code"))
                End If
            End Get
        End Property
    End Class

    Public Enum AuthorizeNetResponseCode
        Approved = 1
        Declined = 2
        TransactionError = 3
        TransactionHeld = 4
    End Enum

End Namespace