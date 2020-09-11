Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports Controls

Public Class Member_OrderHistory_Default
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureMemberAccess()

        Dim dbMember As MemberRow = MemberRow.GetRow(DB, Session("memberId"))
        Dim dt As DataTable = dbMember.GetMemberOrderHistory()
        If dt.DefaultView.Count > 0 Then
            rptOrderHistory.Visible = True
            rptOrderHistory.DataSource = dt
            rptOrderHistory.DataBind()
        Else
            rptOrderHistory.Visible = False
        End If
        divNoRecords.Visible = Not rptOrderHistory.Visible
    End Sub

    Protected Sub rptOrderHistory_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptOrderHistory.ItemCommand
        If e.CommandName = "Details" Then
            Response.Redirect("/members/orders/view.aspx?OrderId=" & e.CommandArgument)
        End If
    End Sub

    Protected Sub rptOrderHistory_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOrderHistory.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem And Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If
        Dim btnDetails As OneClickButton = e.Item.FindControl("btnDetails")
        btnDetails.CommandArgument = e.Item.DataItem("OrderId")
    End Sub
End Class