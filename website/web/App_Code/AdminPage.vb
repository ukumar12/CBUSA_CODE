Imports Microsoft.VisualBasic
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Namespace Components

	Public Class AdminPage
		Inherits BasePage

        Public LoggedInIsInternal As Boolean
        Public LoggedInAdminId As Integer
        Public LoggedInUsername As String
        Public LoggedInFullName As String

		Protected Sub CheckAccess(ByVal action As String)
			If Not HasRights(action) Then
				Response.Redirect("/admin/Unauthorized.aspx")
			End If

			'CLEAR FrameURL FROM SESSION
			If Not LCase(Request.ServerVariables("URL")) = "/admin/default.aspx" Then
				Session("FrameURL") = String.Empty
			End If
		End Sub

        Public Function HasRights(ByVal action As String) As Boolean

            If Context.User.Identity.IsAuthenticated Then
                Dim principal As AdminPrincipal = CType(Context.User, AdminPrincipal)
                Dim aAction() As String
                Dim bHasPermission As Boolean

                aAction = action.Split(","c)
                For Each s As String In aAction
                    bHasPermission = principal.HasPermission(s)
                    If bHasPermission Then Exit For
                Next
                Return bHasPermission
            Else
                Return False
            End If

        End Function

		Protected Sub CheckInternalAccess(ByVal action As String)
			If Not HasRights(action) OrElse Not LoggedInIsInternal Then
				Response.Redirect("/admin/Unauthorized.aspx")
			End If
		End Sub

		Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			'Make sure that the page is displayed in https://
			EnsureSSL()

			'Save FrameURL in the session
			If Not Request("FrameURL") = String.Empty Then Session("FrameURL") = Request("FrameURL")

			'If not logged in then redirect to login page
			If Not Context.User.Identity.IsAuthenticated Then
                Response.Redirect(AppSettings("GlobalSecureName") & "/admin/Login.aspx")
				Exit Sub
			End If

            Dim newUser As AdminPrincipal = Nothing
			If TypeOf Context.User Is AdminPrincipal Then
				newUser = CType(Context.User, AdminPrincipal)
			Else
                Try
                    'check if the user is not locked
                    Dim dbAdmin As AdminRow = AdminRow.GetRowByUsername(DB, Context.User.Identity.Name)
                    If dbAdmin.IsLocked Then
                        Response.Redirect(AppSettings("GlobalSecureName") & "/admin/logout.aspx")
                        Exit Sub
                    End If

                    'check if the password has not expired, skip Active Directory authentication
                    If dbAdmin.Username <> "admin" Then
                        If dbAdmin.PasswordDate = Nothing Then
                            Session("AdminIsNew") = True
                        End If
                        If Not dbAdmin.PasswordDate = Nothing AndAlso DateDiff("d", dbAdmin.PasswordDate, Now()) > 90 Then
                            Session("AdminExpiredPassword") = True
                        End If
                    End If

                    newUser = New AdminPrincipal(DB, Context.User.Identity.Name)
                    Context.User = newUser

                Catch ex As Exception
                    Response.Redirect(AppSettings("GlobalSecureName") & "/admin/logout.aspx")
                End Try

			End If
			LoggedInAdminId = CType(newUser.Identity, AdminIdentity).AdminId
			LoggedInIsInternal = CType(newUser.Identity, AdminIdentity).IsInternal
            LoggedInUsername = CType(newUser.Identity, AdminIdentity).Username
            LoggedInFullName = CType(newUser.Identity, AdminIdentity).Name

            If (Session("AdminExpiredPassword") Or Session("AdminIsNew")) And LCase(Request.ServerVariables("URL")) <> "/admin/password/default.aspx" Then
                Response.Redirect(AppSettings("GlobalSecureName") & "/admin/password/default.aspx")
            End If

            

			If IsPostBack AndAlso Not Request.Path.ToLower = "/admin/login.aspx" Then
				ScriptManager.RegisterStartupScript(Me, Me.GetType, "ClearTimer", "if(typeof extendSession=='function') {clearTimeout(loginTimer); clearTimeout(sessionTimer); sessionTimer = setTimeout(""extendSession()"", timeoutInterval);}", True)
			End If

           
        End Sub
	End Class

End Namespace