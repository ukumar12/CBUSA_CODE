Option Strict Off
Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Partial Class modules_EnsureBuilderAccess
    Inherits ModuleControl


    Protected Sub modules_EnsureBuilderAccess_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsAdminDisplay Then
            If Not CType(Me.Page, SitePage).IsLoggedInBuilder() Then
                Response.Redirect(AppSettings("GlobalSecureName") & "/default.aspx")
            End If
        End If
    End Sub
End Class
