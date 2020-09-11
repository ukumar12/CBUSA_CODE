Imports System.Web
Imports log4net
Imports System.Configuration.ConfigurationManager
Imports System.Web.SessionState

Namespace Components

    Public MustInherit Class Logger

        Public Shared Function GetErrorMessage(ByVal ex As Exception) As String
            Dim baseex As Exception = ex.GetBaseException()
            Dim Context As HttpContext = HttpContext.Current
            Dim Request As HttpRequest = Nothing
            Dim Session As HttpSessionState = Nothing

            If Not Context Is Nothing Then Request = Context.Request
            If Not Context Is Nothing Then Session = Context.Session

            Dim ErrorMessage As String = String.Empty
            ErrorMessage &= "*** Error Message ***" & vbCrLf & "---------------------" & vbCrLf & baseex.Message & vbCrLf & vbCrLf
            ErrorMessage &= "*** Error Source ***" & vbCrLf & "---------------------" & vbCrLf & baseex.Source & vbCrLf & vbCrLf
            If baseex.TargetSite IsNot Nothing Then ErrorMessage &= "*** Error Target Site ***" & vbCrLf & "---------------------" & vbCrLf & baseex.TargetSite.ToString & vbCrLf & vbCrLf
            ErrorMessage &= "*** Date/Time ***" & vbCrLf & "---------------------" & vbCrLf & DateTime.Now.ToString() & vbCrLf & vbCrLf
            ErrorMessage &= "*** Stack Trace ***" & vbCrLf & "---------------------" & vbCrLf & ex.StackTrace.ToString & vbCrLf & vbCrLf

            If Not Request Is Nothing Then
                If Not Request.ServerVariables("HTTP_REFERER") Is Nothing Then
                    ErrorMessage &= "*** Referrer ***" & vbCrLf & "---------------------" & vbCrLf & Request.ServerVariables("HTTP_REFERER").ToString & vbCrLf & vbCrLf
                End If
                If Not Request.QueryString Is Nothing Then
                    ErrorMessage &= "*** Query String ***" & vbCrLf & "---------------------" & vbCrLf & Request.QueryString.ToString & vbCrLf & vbCrLf
                End If
                If Not Request.Form Is Nothing Then
                    If Request.Form.Count > 0 Then
                        ErrorMessage &= "*** Form Fields ***" & vbCrLf & "---------------------" & vbCrLf
                    End If
                    For i As Integer = 0 To Request.Form.Count - 1
						If Not Request.Form.Keys(i) Is Nothing AndAlso Request.Form.Keys(i).ToString <> "__VIEWSTATE" Then
                            Dim value As String = Request.Form(i)
                            If IsProtectedParamName(Request.Form.Keys(i).ToString) Then
                                value = "*** protected ***"
                            End If
                            ErrorMessage &= Request.Form.Keys(i) & "=" & value & vbCrLf
                        End If
                    Next
                End If
            End If

            If Not Session Is Nothing Then
                Dim keys As ICollection = Session.Keys
                If keys.Count > 0 Then
                    ErrorMessage &= vbCrLf & "*** Session ***" & vbCrLf & "---------------------" & vbCrLf
                End If

                For Each key As String In keys
                    'Dont assume we only store strings in session
                    If TypeOf Session(key) Is String Then
                        Dim value As String = CStr(Session(key))
                        If IsProtectedParamName(key) Then
                            value = "*** protected ***"
                        End If
                        ErrorMessage &= key & "=" & value & vbCrLf
                    End If
                Next


            End If
            Return ErrorMessage
        End Function

        Private Shared Function IsProtectedParamName(ByVal FieldName As String) As Boolean
            Dim fields() As String = {"password", "pwd", "pass", "creditcard", "credit_card", "ccnumber", "cid", "card_number", "cardnumber"}
            FieldName = LCase(FieldName)
            For i As Integer = 0 To fields.Length - 1
                If FieldName.IndexOf(fields(i)) >= 0 Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Shared Function GetLoggerObject() As ILog
            Dim log As ILog = LogManager.GetLogger(ConfigurationManager.AppSettings("GlobalWebsiteKey"))
            If Not LogManager.GetRepository().Configured Then
                log4net.Config.XmlConfigurator.Configure()
                Dim hier As log4net.Repository.Hierarchy.Hierarchy = log4net.LogManager.GetRepository()
                Dim adoAppender As log4net.Appender.AdoNetAppender = hier.Root.GetAppender("AdoNetAppender")
                If Not adoAppender Is Nothing Then
                    Dim ConnectionString As String = DBConnectionString.GetConnectionString(AppSettings("LoggerConnectionString"), AppSettings("LoggerConnectionStringUsername"), AppSettings("LoggerConnectionStringPassword"))
                    adoAppender.ConnectionString = ConnectionString
                    adoAppender.ActivateOptions()
                End If
            End If
            Return log
        End Function

        Public Shared Sub Fatal(ByVal Message As String)
            Dim log As ILog = GetLoggerObject()
            If (log.IsFatalEnabled) Then
                UpdateCustomFields()
                log.Fatal(Message)
            End If
        End Sub

        Public Shared Sub [Debug](ByVal Message As String)
            Dim log As ILog = GetLoggerObject()
            If log.IsDebugEnabled Then
                UpdateCustomFields()
                log.Debug(Message)
            End If
        End Sub

        Public Shared Sub [Error](ByVal Message As String)
            Dim log As ILog = GetLoggerObject()
            If log.IsErrorEnabled Then
                UpdateCustomFields()
                log.Error(Message)
            End If
        End Sub

        Public Shared Sub [Auto](ByVal Message As String)
            If IsSoft(Message) Then
                [Warning](Message)
            Else
                [Error](Message)
            End If
        End Sub

        Public Shared Sub Info(ByVal Message As String)
            Dim log As ILog = GetLoggerObject()
            If log.IsInfoEnabled Then
                UpdateCustomFields()
                log.Info(Message)
            End If
        End Sub

        Public Shared Sub Warning(ByVal Message As String)
            Dim log As ILog = GetLoggerObject()
            If log.IsWarnEnabled Then
                UpdateCustomFields()
                log.Warn(Message)
            End If
        End Sub

        Private Shared Sub UpdateCustomFields()
            If HttpContext.Current Is Nothing Then
                Exit Sub
            End If
            MDC.Set("host", HttpContext.Current.Request.ServerVariables("HTTP_HOST"))
            MDC.Set("server_name", HttpContext.Current.Server.MachineName)
            MDC.Set("user_agent", HttpContext.Current.Request.ServerVariables("HTTP_USER_AGENT"))
            MDC.Set("remote_addr", HttpContext.Current.Request.ServerVariables("REMOTE_ADDR"))
            MDC.Set("url", HttpContext.Current.Request.RawUrl)
        End Sub

        Private Shared Function IsSoft(ByVal Msg As String) As Boolean
            If InStr(Msg, "A potentially dangerous Request.Form value was detected from the client") > 0 Then
                Return True
            ElseIf InStr(Msg, "A potentially dangerous Request.Cookies value was detected from the client") > 0 Then
                Return True
            ElseIf InStr(Msg, "A potentially dangerous Request.QueryString value was detected from the client") > 0 Then
                Return True
            ElseIf InStr(Msg, "Input string was not in a correct format") > 0 Then
                Return True
            ElseIf InStr(Msg, "Invalid length for a Base-64 char array") > 0 Then
                Return True
            ElseIf InStr(Msg, "Invalid character in a Base-64 string") > 0 Then
                Return True
            ElseIf InStr(Msg, "Invalid postback or callback argument") > 0 Then
                Return True
            ElseIf InStr(Msg, "Unable to validate data") > 0 Then
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace

