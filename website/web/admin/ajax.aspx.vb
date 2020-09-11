Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Controls
Imports Components
Imports DataLayer
Imports System.IO

Public Class adminajax
    Inherits AdminPage

    Private Function Escape(ByVal s As String)
        Dim t As String

        t = Replace(s, "'", "\'")
        t = Trim(t)

        Return "'" & t & "'"
    End Function

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()

        Dim FunctionName As String = Request("f")
        Select Case FunctionName
            Case "ExtendSession"
                ExtendSession()
        End Select
    End Sub

    Private Sub ExtendSession()
        If LoggedInAdminId > 0 Then
            Response.Write(1)
        Else
            Response.Write(0)
        End If
    End Sub
End Class