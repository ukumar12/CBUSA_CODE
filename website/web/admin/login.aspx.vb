Imports Components
Imports DataLayer

Partial Class Login
    Inherits BasePage
    Private PageUrl As String = ""
    Private CurrentUserId As String = ""
    Private CurrentUserName As String = ""
    Private IsExternalAuthentication As Boolean = False

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Make sure that the page is displayed in https://

        EnsureSSL()
        Dim IsCallingForAutoAuthenticate As Boolean = False

        Dim parameters As System.Collections.Specialized.NameValueCollection
        parameters = Request.QueryString
        Dim key As String
        Dim values() As String
        Dim ExternalAuthToken As String = ""
        Dim StrUserName As String = ""
        Dim StrPasswd As String = ""
        'System.Diagnostics.Debug.Print("Number of parameters: " & parameters.Count)
        Dim Ctr As Integer = 0
        For Each key In parameters.Keys
            Ctr += 1
            values = parameters.GetValues(key)
            For Each value As String In values
                'System.Diagnostics.Debug.Print(key & " - " & value)
                If key.Equals("CBUSAMobAppCall") Then
                    IsCallingForAutoAuthenticate = True
                    ExternalAuthToken = value
                End If
                If key.Equals("UserName") Then
                    StrUserName = value
                End If
                If key.Equals("Password") Then
                    StrPasswd = value
                End If
            Next
        Next
        If IsCallingForAutoAuthenticate AndAlso Not String.IsNullOrEmpty(StrUserName) AndAlso Not String.IsNullOrEmpty(StrPasswd) Then
            Me.Username.Value = StrUserName
            Me.UserPass.Value = StrPasswd
            Me.PageUrl = Request.Url.ToString()
            AuthenticateExternalLogin()
        End If

    End Sub

    Private Function AuthenticateExternalLogin() As String
        Dim IsAuthenticated = False
        ' Validate Login
        Dim resp As ValidateCredentialsResponse
        Dim newUser As AdminPrincipal = AdminPrincipal.ValidateLogin(DB, Username.Value, UserPass.Value, System.Configuration.ConfigurationManager.AppSettings("DomainGroupNames"), resp)
        'CurrentUserName = newUser.Identity.
        'Core.DataLog("Mobile App Login", PageUrl, Username.Value, "External Admin Login", "", "", "", "", CurrentUserName)
        Session("AdminExpiredPassword") = False
        Session("AdminIsNew") = False
        Dim StrReturnKey As String = "IsAuthenticated"
        Dim StrReturnValue As String = ""
        If Not IsNothing(newUser) Then
            If newUser.Identity.IsAuthenticated = True Then
                Dim json As String = "{""IsAuthentication"":""true""}"
                Response.Clear()
                Response.ContentType = "application/json; charset=utf-8"
                Response.Write(json)
                Response.[End]()
            Else
                Dim json As String = "{""IsAuthentication"":""Invalid user name / password""}"
                Response.Clear()
                Response.ContentType = "application/json; charset=utf-8"
                Response.Write(json)
                Response.[End]()
            End If
        Else
            Dim json As String = "{""IsAuthentication"":""Invalid user name / password""}"
            Response.Clear()
            Response.ContentType = "application/json; charset=utf-8"
            Response.Write(json)
            Response.[End]()
        End If

        Return IsAuthenticated
    End Function

    Protected Sub Login_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        If Not IsValid Then Exit Sub

        ' Validate Login
        Dim resp As ValidateCredentialsResponse
        Dim newUser As AdminPrincipal = AdminPrincipal.ValidateLogin(DB, Username.Value, UserPass.Value, System.Configuration.ConfigurationManager.AppSettings("DomainGroupNames"), resp)

        Session("AdminExpiredPassword") = False
        Session("AdminIsNew") = False

        ' If Login/Password correct then replace Context.User
        If newUser Is Nothing Then

            Select Case resp
                Case ValidateCredentialsResponse.LockedUser
                    Msg.Text = "Your account is locked. Please contact a site administrator for assistance."
                    Core.LogEvent("Access was denied to user """ & Username.Value & """ as account was locked.", Diagnostics.EventLogEntryType.Warning)
                Case ValidateCredentialsResponse.WrongPassword
                    Msg.Text = "Invalid Credentials. Please try again."
                    Core.LogEvent("Invalid credentials for user """ & Username.Value & """", Diagnostics.EventLogEntryType.Warning)
                Case ValidateCredentialsResponse.WrongUsername
                    Msg.Text = "Invalid Credentials. Please try again."
                    Core.LogEvent("Invalid credentials for user """ & Username.Value & """", Diagnostics.EventLogEntryType.Warning)
            End Select

            'Increase number of login attempts
            ViewState("LoginCount") = IIf(ViewState("LoginCount") Is Nothing, 0, ViewState("LoginCount")) + 1

            If Not ViewState("LoginCount") Is Nothing AndAlso ViewState("LoginCount") >= 3 Then
                ViewState("LoginCount") = 0
                AdminRow.LockUser(DB, Username.Value)

                Msg.Text = "Because of three failed login attempts, the user account <b>" & Username.Value & "</b> has been locked.<br />Please contact a system administrator for assistance."
                Core.LogEvent("Account was locked for user """ & Username.Value & """", Diagnostics.EventLogEntryType.Warning)
            End If

            AdminRow.InsertAdminLog(DB, Nothing, Session.SessionID, Username.Value, Request.ServerVariables("REMOTE_ADDR"), False)
        Else
            Context.User = newUser

            'There is a documented BUG in ASP.NET 2.0 related with persistent cookies, 
            'therefore we need to set the cookie ourself
            'http://lab.msdn.microsoft.com/productfeedback/ViewWorkaround.aspx?FeedbackID=FDBK31991#1
            SetAuthCookie(newUser.Username, False)

            Session("Username") = Username.Value

            AdminRow.InsertAdminLog(DB, CType(newUser.Identity, AdminIdentity).AdminId, Session.SessionID, Username.Value, Request.ServerVariables("REMOTE_ADDR"), True)

            Session("AdminId") = CType(newUser.Identity, AdminIdentity).AdminId

            'Password is not set or expired, in this case we need to redirect the user to password page
            'and force him/her to change password
            If Not CType(newUser.Identity, AdminIdentity).Username = "admin" Then
                Select Case resp
                    Case ValidateCredentialsResponse.ExpiredPassword
                        Session("AdminExpiredPassword") = True
                    Case ValidateCredentialsResponse.NewUser
                        Session("AdminIsNew") = True
                End Select
            End If
            Core.LogEvent("Admin """ & Username.Value & """ Logged in", Diagnostics.EventLogEntryType.Information)
            'log admin login
            PageUrl = Request.Url.ToString()
            CurrentUserId = Session("AdminId")
            CurrentUserName = Session("Username")
            Core.DataLog("Login", PageUrl, CurrentUserId, "Admin Login", "", "", "", "", CurrentUserName)
            'End log
            Response.Redirect("/admin/")
        End If
    End Sub

    Sub SetAuthCookie(ByVal username As String, ByVal Persist As Boolean)
        If Persist Then
            Dim ticket As FormsAuthenticationTicket = New FormsAuthenticationTicket(2, username, DateTime.Now, DateTime.Now.AddMonths(1), True, "", FormsAuthentication.FormsCookiePath)
            Dim cookie As HttpCookie = New HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))

            cookie.HttpOnly = True
            cookie.Path = FormsAuthentication.FormsCookiePath
            cookie.Secure = FormsAuthentication.RequireSSL
            cookie.Expires = ticket.Expiration

            Response.Cookies.Add(cookie)
        Else
            FormsAuthentication.SetAuthCookie(username, False)
        End If
    End Sub

End Class
