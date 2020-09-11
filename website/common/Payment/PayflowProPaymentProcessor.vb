Imports System.Collections.Specialized
Imports System.Reflection

Namespace Components

    Public Class PayflowProPaymentProcessor
        Inherits PaymentProcessor

        Private Function SubmitTransaction(ByVal Input As StringDictionary) As Integer
            Dim Host As String

            If TestMode Then Host = "test-payflow.verisign.com" Else Host = "payflow.verisign.com"

            Input("USER") = Username
            Input("PWD") = Password
            Input("PARTNER") = Custom1
            Input("VENDOR") = Custom2

            Dim lateBoundType As Type = Type.GetTypeFromProgID("PFProCOMControl.PFProCOMControl.1")
            Dim lateBoundObject As Object = Activator.CreateInstance(lateBoundType)

            Dim args As Object() = New Object() {Host, 443, Timeout, "", 0, "", ""}
            Dim context As Integer = Int(lateBoundType.InvokeMember("CreateContext", BindingFlags.Public Or BindingFlags.InvokeMethod, Nothing, lateBoundObject, args))

            Dim Params As String = GetParams(Input)
            args = New Object() {context, Params, Params.Length}
            Dim Results As String = CStr(lateBoundType.InvokeMember("SubmitTransaction", BindingFlags.Public Or BindingFlags.InvokeMethod, Nothing, lateBoundObject, args))

            args = New Object() {context}
            lateBoundType.InvokeMember("DestroyContext", BindingFlags.Public Or BindingFlags.InvokeMethod, Nothing, lateBoundObject, args)

            Output = ParseResults(Results)

            Return Int32.Parse(Output("RESULT"))
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
            Dim Buffer() As String = Result.Split("&")
            For Each pairs As String In buffer
                Try
                    Dim tmp() As String = pairs.Split("=")
                    Dictionary(tmp(0)) = tmp(1)
                Catch ex As Exception
                End Try
            Next
            Return Dictionary
        End Function
        Public Overrides Function GetVerificationResponse() As String
            Dim ErrorMessage As String = String.Empty
            Dim Buffer() As String = FullResponse.Split("&")
            For Each pairs As String In Buffer
                Try
                    Dim tmp() As String = pairs.Split("=")
                    Select Case tmp(0)
                        Case "AvsAddr"
                            If tmp(1) = "N" Then
                                ErrorMessage &= "Address does not match. "
                            End If
                        Case "AvsZip"
                            If tmp(1) = "N" Then
                                ErrorMessage &= "Zip Code does not match. "
                            End If
                        Case "CVV2Match"
                            If tmp(1) = "N" Then
                                ErrorMessage = "CVV code does not match. "
                            End If
                        Case Else
                    End Select
                Catch ex As Exception
                End Try
            Next
            Return ErrorMessage
        End Function

        Public Overrides Function SubmitSale(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean
            Dim Input As New StringDictionary

            Input("TRXTYPE") = "S"
            Input("TENDER") = "C"
            Input("ACCT") = RemoveSpecialCharacters(cc.CreditCardNumber)
            Input("EXPDATE") = ConvertExpDate(cc.ExpMonth, cc.ExpYear)
            Input("AMT") = Amount.ToString
            Input("NAME") = cc.CardHolderName
            Input("CVV2") = cc.CID
            Input("ZIP") = RemoveSpecialCharacters(addr.Zip)
            Input("STREET") = addr.AddressLn1
            Input("COMMENT1") = Unique
            Input("COMMENT2") = addr.City

            Return 0 = SubmitTransaction(Input)
        End Function

        Private Function ConvertExpDate(ByVal m As Integer, ByVal y As Integer) As String
            Dim result As String = m & Right(y, 2)
            If Len(result) = 3 Then result = "0" & result
            Return result
        End Function

        Public Overrides Function SubmitAuthorization(ByVal Unique As String, ByVal cc As CreditCardInfo, ByVal addr As AddressInfo, ByVal Amount As Double) As Boolean
            Dim Input As New StringDictionary

            Input("TRXTYPE") = "A"
            Input("TENDER") = "C"
            Input("ACCT") = RemoveSpecialCharacters(cc.CreditCardNumber)
            Input("EXPDATE") = ConvertExpDate(cc.ExpMonth, cc.ExpYear)
            Input("AMT") = Amount.ToString
            Input("NAME") = cc.CardHolderName
            Input("CVV2") = cc.CID
            Input("ZIP") = RemoveSpecialCharacters(addr.Zip)
            Input("STREET") = addr.AddressLn1
            Input("COMMENT1") = Unique
            Input("COMMENT2") = addr.City

            Return 0 = SubmitTransaction(Input)
        End Function

        Public Overrides Function SubmitDelayedCapture(ByVal TransactionNo As String, ByVal Amount As Double) As Boolean
            Dim Input As New StringDictionary
            Input("TRXTYPE") = "D"
            Input("TENDER") = "C"
            Input("ORIGID") = TransactionNo
            Input("AMT") = Amount

            Return 0 = SubmitTransaction(Input)
        End Function

        Public Overrides Function SubmitCredit(ByVal TransactionNo As String, ByVal Amount As Double, Optional ByVal cc As CreditCardInfo = Nothing) As Boolean
            Dim Input As New StringDictionary

            Input("TRXTYPE") = "C"
            Input("TENDER") = "C"
            Input("ORIGID") = TransactionNo
            Input("AMT") = Amount

            Return 0 = SubmitTransaction(Input)
        End Function


        Public Overrides Function SubmitVoid(ByVal TransactionNo As String) As Boolean
            Dim Input As New StringDictionary

            Input("TRXTYPE") = "V"
            Input("ORIGID") = TransactionNo
            Input("TENDER") = "C"

            Return 0 = SubmitTransaction(Input)
        End Function

        Public Overrides ReadOnly Property ErrorMessage() As String
            Get
                Return Output("RESPMSG")
            End Get
        End Property

        Public Overrides ReadOnly Property TransactionNo() As String
            Get
                Return Output("PNREF")
            End Get
        End Property

        Public Overrides ReadOnly Property Result() As Integer
            Get
                Return Output("RESULT")
            End Get
        End Property

        Public Overrides ReadOnly Property FullResponse() As String
            Get
                Dim Result As String = String.Empty
                For Each key As String In Output.Keys
                    Dim k As String = key
                    Dim v As String = Output(key)
                    Result = Result & k & "=" & v & vbCrLf
                Next
                Return Result
            End Get
        End Property

    End Class

End Namespace