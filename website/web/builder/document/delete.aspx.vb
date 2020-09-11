Imports Components
Imports DataLayer
Imports System.Net.Mail

Partial Class _default
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim BuilderDocumentID As Integer
        Dim Builderdocument As DataLayer.BuilderDocumentRow

        Try

            BuilderDocumentID = Convert.ToInt16(Request("ID"))
            Builderdocument = DataLayer.BuilderDocumentRow.GetRow(Me.DB, BuilderDocumentID)
            Builderdocument.IsApproved = False
            Builderdocument.Update()

            'log Delete Document
            PageURL = Request.Url.ToString()
            CurrentUserId = Session("VendorId")
            UserName = Session("Username")
            Core.DataLog("PlansOnline", PageURL, CurrentUserId, "Delete Document", BuilderDocumentID, "", "", "", UserName)
            'end log

            Response.Redirect("../default.aspx")

        Catch ex As Exception

        End Try

    End Sub

End Class
