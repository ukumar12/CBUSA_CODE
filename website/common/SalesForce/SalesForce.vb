Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports sforce
Imports DataLayer
Imports System.ServiceModel
Imports System.Net

Namespace SalesForce
    Public Class SalesForce



        Sub New(ByRef DB As Database)


            username = SysParam.GetValue(DB, "SalesForceLogin")
            password = SysParam.GetValue(DB, "SalesForcePassword")
            token = SysParam.GetValue(DB, "SalesForceAPI")
            'Make sure to change endpoints in web.config as well
            serviceurl = IIf(CInt(SysParam.GetValue(DB, "TestMode")) = 1, SysParam.GetValue(DB, "SalesForceHostTest"), SysParam.GetValue(DB, "SalesForceHostLive"))


            mClient = New SoapClient()

        End Sub

        Public Property serviceurl() As String
            Get
                Return m_serviceurl
            End Get
            Set(value As String)
                m_serviceurl = value
            End Set
        End Property
        Private m_serviceurl As String

        Public Property username() As String
            Get
                Return m_username
            End Get
            Set(value As String)
                m_username = value
            End Set
        End Property
        Private m_username As String
        Public Property password() As String
            Get
                Return m_password
            End Get
            Set(value As String)
                m_password = value
            End Set
        End Property
        Private m_password As String
        Public Property token() As String
            Get
                Return m_token
            End Get
            Set(value As String)
                m_token = value
            End Set
        End Property
        Private m_token As String
        Private Property CurrentLoginResult() As LoginResult
            Get
                Return m_CurrentLoginResult
            End Get
            Set(value As LoginResult)
                m_CurrentLoginResult = value
            End Set
        End Property
        Private m_CurrentLoginResult As LoginResult
        'Private Property SfdcBinding() As sforce.SforceService
        '    Get
        '        Return m_SfdcBinding
        '    End Get
        '    Set(value As sforce.SforceService)
        '        m_SfdcBinding = value
        '    End Set
        ''End Property
        'Private m_SfdcBinding As sforce.SforceService
        Private Property saveResults() As SaveResult()
            Get
                Return m_saveResults
            End Get
            Set(value As SaveResult())
                m_saveResults = value
            End Set
        End Property
        Private m_saveResults As SaveResult()

        Private Property mSHeader() As SessionHeader
            Get
                Return m_mSHeader
            End Get
            Set(value As SessionHeader)
                m_mSHeader = value
            End Set
        End Property
        Private m_mSHeader As SessionHeader


        Private Property mClient() As SoapClient
            Get
                Return m_mClient
            End Get
            Set(value As SoapClient)
                m_mClient = value
            End Set
        End Property
        Private m_mClient As SoapClient


        Public Function login() As Boolean
            Try

                Dim callopts As CallOptions = New CallOptions
                callopts.client = ""
                callopts.defaultNamespace = ""
                Dim loginopts As LoginScopeHeader = New LoginScopeHeader
                loginopts.portalId = ""
                loginopts.organizationId = ""
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11
                mClient.Open()
                CurrentLoginResult = mClient.login(loginopts, callopts, username, password & token)
                mSHeader = New SessionHeader()
                mSHeader.sessionId = CurrentLoginResult.sessionId
                mClient.Close()

                mClient = New SoapClient("Soap", CurrentLoginResult.serverUrl())
                mClient.Open()
                serviceurl = CurrentLoginResult.serverUrl()
                Return True
            Catch e As System.Web.Services.Protocols.SoapException
                mClient = Nothing
                mSHeader = Nothing
                Throw (e)
            Catch e As Exception
                mClient = Nothing
                mSHeader = Nothing
                Throw (e)
            End Try
        End Function

        Public Function upsert(ObjectToCreate As sObject(), ByVal ExternalID As String) As sforce.UpsertResult()
            Dim mUpsertResult As sforce.UpsertResult()
            mClient.upsert(mSHeader, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, ExternalID, ObjectToCreate, Nothing, mUpsertResult)
            Return mUpsertResult
        End Function
        Public Function update(ObjectToCreate As sObject()) As sforce.SaveResult()
            Dim mUpdateResult As sforce.SaveResult()
            mClient.update(mSHeader, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, ObjectToCreate, Nothing, mUpdateResult)
            Return mUpdateResult
        End Function

        'Public Function create(ObjectToCreate As sObject()) As Boolean
        '    saveResults = SfdcBinding.create(ObjectToCreate)
        '    Return saveResults(0).success
        'End Function

        'Public Function update(ObjectToCreate As sObject()) As Boolean
        '    saveResults = SfdcBinding.update(ObjectToCreate)
        '    Return saveResults(0).success
        'End Function

        Public Function getLastError() As String
            Return saveResults(0).errors(0).message
        End Function

    End Class
End Namespace