Imports Components
Imports System.IO
Imports DataLayer

Public Class Banner
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		Dim BannerId As Integer = IIf(IsNumeric(Request("BannerId")), Request("BannerId"), 0)
        Dim dbBanner As BannerRow = BannerRow.GetRow(DB, BannerId)
        Dim OrderId As Integer = IIf(Session("OrderId") Is Nothing, 0, Session("OrderId"))

        If dbBanner.IsOrderTracking Then
            If OrderId = 0 Then
                Dim Conn As String = IIf(Session("BannerOrderTracking") Is Nothing, String.Empty, ",")
                Session("BannerOrderTracking") = Session("BannerOrderTracking") & Conn & BannerId
            Else
                BannerOrderTrackingRow.AddClick(DB, BannerId, OrderId)
            End If
        End If
        BannerTrackingRow.AddClick(DB, BannerId, Now())
        Response.Redirect(dbBanner.Link)
    End Sub
End Class
