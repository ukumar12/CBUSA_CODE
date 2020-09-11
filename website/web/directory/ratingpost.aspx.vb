Imports Components
Imports DataLayer
Imports System.net.Mail

Partial Class _default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim buffer As String = String.Empty
        Dim VendorRatingCategoryRating As New DataLayer.VendorRatingCategoryRatingRow(Me.DB)

        Try

            VendorRatingCategoryRating.BuilderID = Convert.ToInt32(Request("builderid"))
            VendorRatingCategoryRating.VendorID = Convert.ToInt32(Request("vendorid"))
            VendorRatingCategoryRating.RatingCategoryID = Convert.ToInt32(Request("categoryid"))
            VendorRatingCategoryRating.Rating = Convert.ToInt32(Request("rating"))

            VendorRatingCategoryRating.Insert()

            buffer = "INSERTED"

        Catch ex As Exception
            buffer = "ERRORED"
        End Try

        MyBase.Response.Write(buffer)
        MyBase.Response.Flush()

    End Sub

End Class
