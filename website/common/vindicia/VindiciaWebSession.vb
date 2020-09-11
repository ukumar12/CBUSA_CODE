Imports System.Collections.Generic
Imports System.Net
Imports System.Runtime.InteropServices.WindowsRuntime
Imports System.Web
Imports DataLayer
Imports Vindicia

Namespace Components

    Public Enum VindiciaEnviroments
        Production = 0
        Prodtest = 1
    End Enum

    Public Enum VindiciaWebSessionMethods
        Account_update
        Account_updatePaymentMethod
        AutoBill_update
        PaymentMethod_update
        PaymentMethod_validate
        Transaction_auth
        Transaction_authCapture
    End Enum


    Public Class VindiciaWebSession

        Public Sub New(ByVal db As Database, ByVal builder As BuilderRow, ByVal action As VindiciaWebSessionMethods, ByVal context As HttpContext)
            _db = db
            _isTestMode = SysParam.GetValue(_db, "TestMode")
            _builder = builder
            _vinWebSessionName = "vin_WebSession_vid_" & _builder.HistoricID.ToString() & "_" & action.ToString()
            CurrentContext = context
            CurrentContext.Session.Remove(_vinWebSessionName)
            Call InitializeEnvironment()
        End Sub

        Public Function UpsertBuilderAccountInVindicia() As Vindicia.Account

            Dim address As New Vindicia.Address
            With address
                .name = Builder.CompanyName
                .addr1 = Builder.Address
                .district = Builder.State
                .postalCode = Builder.Zip
                .country = "US"
            End With

            Dim builderAccount As New Vindicia.Account
            Dim dbbuilderAccount As BuilderAccountRow = BuilderAccountRow.GetPrimaryAccount(_db, Builder.BuilderID)
            builderAccount.merchantAccountId = Builder.HistoricID
            builderAccount.company = Builder.CompanyName
            builderAccount.emailAddress = dbbuilderAccount.Email
            builderAccount.emailTypePreferenceSpecified = True
            builderAccount.emailTypePreference = EmailPreference.html
            builderAccount.name = Core.BuildFullName(dbbuilderAccount.FirstName, String.Empty, dbbuilderAccount.LastName)
            builderAccount.shippingAddress = address

            Dim accountCreated As Boolean = False
            Dim ret As Vindicia.Return = builderAccount.update("", accountCreated)
            Dim msg As String = "Error: UpsertBuilderAccountInVindicia "
            If ret.returnCode = 200 Then
                msg = "Websession --> Builder was" & IIf(accountCreated, " Created", " Updated")
            Else
                builderAccount = Nothing
            End If
            LogReturn(ret, msg)

            Return builderAccount
        End Function

        Public Function CheckIfSessionIdIsActiveInVindicia(ByVal sessionId As String) As Boolean

            If sessionId Is Nothing Then Return False

            Call InitializeEnvironment()
            Dim sessionReteriver As New Vindicia.WebSession
            Dim ret As Vindicia.Return = sessionReteriver.fetchByVid("", VinWebSessionVid, sessionReteriver)
            Return ret.returnCode = 200
        End Function

        Public Function UpdateCreditCardInformation() As Boolean
            If VinWebSessionVid Is Nothing AndAlso UpsertBuilderAccountInVindicia() IsNot Nothing Then
                Dim preLoadWebSessionValues() As Vindicia.NameValuePair = New Vindicia.NameValuePair() {
                New Vindicia.NameValuePair() With {.name = "Account_updatePaymentMethod_replaceOnAllAutoBills", .value = 1},
                New Vindicia.NameValuePair() With {.name = "Account_updatePaymentMethod_replaceOnAllChildAutoBills", .value = 1},
                New Vindicia.NameValuePair() With {.name = "Account_updatePaymentMethod_updateBehavior", .value = 2},
                New Vindicia.NameValuePair() With {.name = "Account_updatePaymentMethod_ignoreAvsPolicy", .value = 1},
                New Vindicia.NameValuePair() With {.name = "Account_updatePaymentMethod_ignoreAvsPolicySpecified", .value = 1},
                New Vindicia.NameValuePair() With {.name = "Account_updatePaymentMethod_ignoreCvnPolicy", .value = 1},
                New Vindicia.NameValuePair() With {.name = "Account_updatePaymentMethod_ignoreCvnPolicySpecified", .value = 1}
                }
                InitializeSession(preLoadWebSessionValues)
            End If
        End Function

#Region "Vindicia Web Session"
        Private Sub InitializeSession(Optional fieldsCollection As Vindicia.NameValuePair() = Nothing)
            _vindiciaSession = New Vindicia.WebSession()
            'TODO: Make below as System Paramaters

            Dim URL As String = Configuration.WebConfigurationManager.AppSettings("GlobalRefererName")

            _vindiciaSession.errorURL = URL & "/forms/builder-registration/VinHoaError.aspx"
            _vindiciaSession.returnURL = URL & "/forms/builder-registration/VinHoaSuccess.aspx" ' 
            _vindiciaSession.ipAddress = GetIpAddress()
            _vindiciaSession.version = Vindicia.Environment.soapVersion

            _vindiciaSession.method = "Account_updatePaymentMethod"

            Dim preLoadWebSessionValues() As Vindicia.NameValuePair = New Vindicia.NameValuePair() {
               New Vindicia.NameValuePair() With {.name = "vin_Account_merchantAccountId", .value = Builder.HistoricID.ToString()},
               New Vindicia.NameValuePair() With {.name = "vin_PaymentMethod_type", .value = "CreditCard"}}
            _vindiciaSession.privateFormValues = preLoadWebSessionValues

            _vindiciaSession.methodParamValues = fieldsCollection

            Dim response As Vindicia.Return = _vindiciaSession.initialize("")
            LogReturn(response)
            If response.returnCode = "200" Then
                VinWebSessionVid = _vindiciaSession.VID
            End If
        End Sub


        Public Function GetIpAddress() As String

            Dim sIPAddress As String = CurrentContext.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(sIPAddress) Then
                Return CurrentContext.Request.ServerVariables("REMOTE_ADDR")
            Else
                Dim ipArray As String() = sIPAddress.Split(New [Char]() {","c})
                Return ipArray(0)
            End If
        End Function

        Public Function GetPostValuesFromWebSession() As Vindicia.NameValuePair()
            Return CurrentVindiciaWebSession.postValues()
        End Function

        Public Function GetApiReturnValues() As Vindicia.WebSessionMethodReturn
            Return CurrentVindiciaWebSession.apiReturnValues
        End Function


        Private Function SecurityCallback() As Boolean
            Return True
        End Function
        Private Sub InitializeEnvironment()

            System.Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf SecurityCallback

            '********** Added following line (by Apala - Medullus) on 19.12.2017 for PCI Compliance
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Vindicia.Environment.SetEndpoint(GetVindicaEnvironment(_isTestMode))
            Vindicia.Environment.SetAuth(SysParam.GetValue(_db, "VindiciaLogin"), SysParam.GetValue(_db, "VindiciaPassword"))
        End Sub

        Private Function GetVindicaEnvironment(ByVal currentEnviroment As VindiciaEnviroments) As String
            Dim serviceAddress As String = String.Empty
            Select Case currentEnviroment
                Case VindiciaEnviroments.Production
                    serviceAddress = SysParam.GetValue(_db, "VindiciaServiceAddress")
                Case VindiciaEnviroments.Prodtest
                    serviceAddress = "https://soap.prodtest.sj.vindicia.com"
            End Select

            Return serviceAddress
        End Function

        Public Sub LogReturn(ByVal ret As Vindicia.Return, Optional ByVal customMsg As String = "")
            If customMsg = "" Then customMsg = "WebSession -> " & _vindiciaMethod.ToString()

            Dim dbLog As New VindiciaSoapLogRow(_db)
            dbLog.SoapId = ret.soapId
            dbLog.ReturnCode = ret.returnCode
            dbLog.ReturnString = ret.returnString
            dbLog.SoapMethod = customMsg
            dbLog.BuilderGUID = Builder.HistoricID
            dbLog.Insert()
        End Sub


        Public Sub LogCVVReturn(ByVal ret As Vindicia.Return, ByVal tran As Vindicia.Transaction)

            Dim dbLog As New VindiciaSoapLogRow(_db)
            dbLog.SoapId = ret.soapId
            dbLog.ReturnCode = ret.returnCode
            dbLog.ReturnString = ret.returnString & vbCrLf & vbCrLf
            'get transaction details in try/catch block in case null from invalid transaction
            Try
                dbLog.ReturnString &= "CVV2 Return Code: " & tran.statusLog(0).status.ToString & vbCrLf
                dbLog.ReturnString &= "AVS Return Code: " & tran.statusLog(0).creditCardStatus.avsCode & vbCrLf
                dbLog.ReturnString &= "Auth Return Code: " & tran.statusLog(0).creditCardStatus.authCode & vbCrLf
                dbLog.ReturnString &= "Verification Code: " & tran.statusLog(0).creditCardStatus.cvnCode & vbCrLf
            Catch ex As Exception
                Logger.Error(Logger.GetErrorMessage(ex))
            End Try

            If CurrentContext IsNot Nothing Then
                dbLog.BuilderGUID = CurrentContext.Session("BuilderId")
            End If
            dbLog.Insert()
        End Sub


#End Region


#Region "Properties"


        Public Property Builder As BuilderRow
            Get
                Return _builder
            End Get
            Set(value As BuilderRow)
                _builder = value
            End Set
        End Property

        Public Property VinWebSessionVid As String
            Get
                If (CurrentContext.Session(_vinWebSessionName) IsNot Nothing) Then
                    _vinWebSessionVid = CStr(CurrentContext.Session(_vinWebSessionName))
                End If
                Return _vinWebSessionVid
            End Get
            Set
                _vinWebSessionVid = Value
                CurrentContext.Session.Add(_vinWebSessionName, _vinWebSessionVid)
            End Set
        End Property

        Public ReadOnly Property CurrentVindiciaWebSession() As Vindicia.WebSession
            Get
                Return _vindiciaSession
            End Get
        End Property

        Public Property CurrentContext() As HttpContext

        Private _vindiciaSession As Vindicia.WebSession
        Private _db As Database
        Private _isTestMode As Integer
        Private _builder As BuilderRow
        Private _vinWebSessionName As String = Nothing
        Private _vinWebSessionVid As String = Nothing
        Private _vindiciaMethod As VindiciaWebSessionMethods


#End Region
    End Class
End Namespace