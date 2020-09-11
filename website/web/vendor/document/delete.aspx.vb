Imports Components
Imports DataLayer
Imports System.Net.Mail

Partial Class _default
    Inherits SitePage

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim VendorDocumentID As Integer
        Dim VendorDocument As DataLayer.VendorDocumentRow

        Try

            VendorDocumentID = Convert.ToInt16(Request("ID"))
            VendorDocument = DataLayer.VendorDocumentRow.GetRow(Me.DB, VendorDocumentID)
            VendorDocument.IsApproved = False
            VendorDocument.Update()

            Response.Redirect("../default.aspx")

        Catch ex As Exception

        End Try

    End Sub

End Class
