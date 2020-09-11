Imports Components
Imports DataLayer
Imports Utility
Imports System.Data.SqlClient

Public Class logout
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CookieUtil.SetTripleDESEncryptedCookie("MemberId", Nothing)
        Session("MemberId") = Nothing
        Session("MemberName") = Nothing


        CookieUtil.SetTripleDESEncryptedCookie("OrderId", Nothing)
        Session("OrderID") = Nothing

		Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & "/")
    End Sub
End Class
