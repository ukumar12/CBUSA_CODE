Imports Components
Imports DataLayer
Imports System.net.Mail

Partial Class _default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim buffer As String = String.Empty
        Dim VendorComment As New DataLayer.VendorCommentRow(Me.DB)

        Try

            VendorComment.BuilderID = Convert.ToInt32(Request("builderid"))
            VendorComment.VendorID = Convert.ToInt32(Request("vendorid"))
            VendorComment.Comment = Request("comment")
            'VendorComment.Submitted = Now

            VendorComment.Insert()

            buffer = "INSERTED"

        Catch ex As Exception
            buffer = "ERRORED"
        End Try

        MyBase.Response.Write(buffer)
        MyBase.Response.Flush()

    End Sub

End Class
