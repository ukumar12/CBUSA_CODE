Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Controls
Imports Components
Imports DataLayer
Imports System.IO
Imports System.Configuration.ConfigurationManager

Public Class ajax
	Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()

	Dim FunctionName As String = Request("f")
        Select Case FunctionName
      Case "508Validation"
          Validate508()
        End Select
    End Sub

    Public Shared Function GetRenderedHtml(ByVal Url As String) As String
        Dim Results As String = String.Empty
        Dim req As Net.HttpWebRequest = Net.WebRequest.Create(Url)
        Dim sr As New StreamReader(req.GetResponse().GetResponseStream())
        Results = sr.ReadToEnd
        sr.Close()
        Return Results
    End Function

    Private Sub Validate508()
	Response.Write(GetRenderedHtml("https://accverify.americaneagle.com/ajax.aspx?f=508Validation&html=" & Request("html")))
    End Sub

End Class