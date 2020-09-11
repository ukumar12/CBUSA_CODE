Option Strict Off
Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Partial Class modules_EnsureVendorAccess
    Inherits ModuleControl

    Protected Sub modules_EnsureVendorAccess_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsAdminDisplay Then
            If Not CType(Me.Page, SitePage).IsLoggedInVendor() Then
                Response.Redirect(AppSettings("GlobalSecureName") & "/default.aspx")
            End If
        End If
    End Sub
End Class
