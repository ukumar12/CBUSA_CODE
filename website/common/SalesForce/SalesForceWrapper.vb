Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports sforce

Public Class SalesForceWrapperClient
    Private _binding As SforceService
    Private _username As String
    Private _password As String
    Private _host As String
    Private _querySize As Integer
    Private _sessionlength As Integer
    Private _sessionId As String
    Private _serverURL As String
    Private _nextLoginTime As DateTime

    'Initialize private variables for the class
    Sub New()
        Me._binding = New SforceService
        Me._querySize = 500
        Me._sessionlength = 29
        Me._host = "https://www.salesforce.com/services/Soap/c/15.0"
    End Sub

    Public Property username() As String
        'Allows calling class to get values
        Get
            Return Me._username
        End Get
        'Allows calling class to set value
        Set(ByVal Value As String)
            Me._username = Value
        End Set
    End Property
    Public Property password() As String
        Get
            Return Me._password
        End Get
        Set(ByVal Value As String)
            Me._password = Value
        End Set
    End Property
    Public Property host() As String
        Get
            Return Me._host
        End Get
        Set(ByVal Value As String)
            Me._host = Value
        End Set
    End Property
    Public ReadOnly Property serverURL() As String
        Get
            Return Me._serverURL
        End Get
    End Property
    Public Property querySize() As Integer
        Get
            Return Me._querySize
        End Get
        Set(ByVal Value As Integer)
            Me._querySize = Value
        End Set
    End Property
    Public Property sessionlength() As Integer
        Get
            Return Me._sessionlength
        End Get
        Set(ByVal Value As Integer)
            Me._sessionlength = Value
        End Set
    End Property
    'In case of proxy server...
    Public Property proxy() As System.Net.WebProxy
        Get
            Return Me._binding.Proxy
        End Get
        Set(ByVal Value As System.Net.WebProxy)
            Me._binding.Proxy = Value
        End Set
    End Property

    Public Sub Login()
        Dim lr As sforce.LoginResult
        Me._binding.Url = Me._host
        lr = Me._binding.login(username, password)
        Me._nextLoginTime = Now().AddMinutes(Me.sessionlength)
        'Reset the SOAP endpoint to the returned server URL
        Me._binding.Url = lr.serverUrl
        Me._binding.SessionHeaderValue = New sforce.SessionHeader
        Me._binding.SessionHeaderValue.sessionId = lr.sessionId
        Me._sessionId = lr.sessionId
        Me._serverURL = lr.serverUrl
    End Sub

    Public Sub loginBySessionId(ByVal sid As String, ByVal sURL As String)
        Me._nextLoginTime = Now().AddMinutes(Me.sessionlength)
        Me._binding.Url = sURL
        Me._binding.SessionHeaderValue = New sforce.SessionHeader
        Me._binding.SessionHeaderValue.sessionId = sid
        Me._sessionId = sid
        Me._serverURL = sURL
    End Sub

    Public Function isConnected() As Boolean
        If _sessionId <> "" And _sessionId <> Nothing Then
            If Now() > Me._nextLoginTime Then
                isConnected = False
            End If
            isConnected = True
        Else
            isConnected = False
        End If
    End Function

    Private Function loginRequired() As Boolean
        loginRequired = Not (isConnected())
    End Function

    Public Function executeQuery(ByVal strSOQLStmt As String, Optional ByVal queryBatchSize As Integer = -1) As sforce.QueryResult
        If queryBatchSize = -1 Then
            queryBatchSize = _querySize
        End If
        If (Me.loginRequired()) Then
            Login()
        End If
        _binding.QueryOptionsValue = New sforce.QueryOptions
        _binding.QueryOptionsValue.batchSizeSpecified = True
        _binding.QueryOptionsValue.batchSize = queryBatchSize
        executeQuery = _binding.query(strSOQLStmt)
    End Function

    Public Function executeQueryMore(ByVal queryLocator As String) As sforce.QueryResult
        If loginRequired() Then Login()
        Return _binding.queryMore(queryLocator)
    End Function

    Public Sub setAssignmentRuleHeaderId(ByVal ruleId As String)
        _binding.AssignmentRuleHeaderValue = New AssignmentRuleHeader
        _binding.AssignmentRuleHeaderValue.assignmentRuleId = ruleId
    End Sub

    Public Sub setAssignmentRuleHeaderToDefault(ByVal runDefaultRule As Boolean)
        _binding.AssignmentRuleHeaderValue = New AssignmentRuleHeader
        _binding.AssignmentRuleHeaderValue.useDefaultRule = runDefaultRule
    End Sub

    Public ReadOnly Property Binding() As sforce.SforceService
        Get
            Return _binding
        End Get
    End Property

    Public Function create(ByVal records() As sObject, Optional ByVal batchSize As Integer = 200) As sforce.SaveResult()
        Return batch(records, batchSize, New CreateBatcher)
    End Function

    Public Function update(ByVal records() As sObject, Optional ByVal batchSize As Integer = 200) As sforce.SaveResult()
        Return batch(records, batchSize, New UpdateBatcher)
    End Function

    Private Function batch(ByVal records() As sObject, ByVal batchSize As Integer, ByVal oper As Batcher) As sforce.SaveResult()
        If (records.Length <= batchSize) Then
            batch = oper.perform(Binding, records)
            Exit Function
        End If
        Dim saveResults(records.Length - 1) As sforce.SaveResult
        Dim thisBatch As sforce.sObject()
        Dim pos As Integer = 0
        Dim thisBatchSize As Integer
        While (pos < records.Length)

            thisBatchSize = Math.Min(batchSize, records.Length - pos)
            ReDim thisBatch(thisBatchSize)
            System.Array.Copy(records, pos, thisBatch, 0, thisBatchSize)
            Dim sr As sforce.SaveResult() = oper.perform(Binding, thisBatch)
            System.Array.Copy(sr, 0, saveResults, pos, thisBatchSize)
            pos += sr.Length
        End While
        batch = saveResults
    End Function

    Private Class Batcher

        Public Function perform(ByVal binding As sforce.SforceService, ByVal records As sforce.sObject()) As sforce.SaveResult()
            perform = Nothing
        End Function
    End Class

    Private Class CreateBatcher
        Inherits Batcher

        Public Overloads Function perform(ByVal binding As sforce.SforceService, ByVal records As sforce.sObject()) As sforce.SaveResult()
            perform = binding.create(records)
        End Function
    End Class

    Private Class UpdateBatcher
        Inherits Batcher
        Public Overloads Function perform(ByVal binding As sforce.SforceService, ByVal records As sforce.sObject()) As sforce.SaveResult()
            perform = binding.update(records)
        End Function
    End Class
End Class