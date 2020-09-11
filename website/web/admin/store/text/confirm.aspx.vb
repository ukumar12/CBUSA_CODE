Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Confirm
    Inherits AdminPage

    Protected Text As String = String.Empty
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Text = Request("Text")
    End Sub
End Class

