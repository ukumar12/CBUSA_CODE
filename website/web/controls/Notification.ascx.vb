Imports Components
Imports DataLayer
Imports System.Web

Partial Class Notification
    Inherits BaseControl

    Private dt As DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindRepeater()
        End If
    End Sub

    Private Sub BindRepeater()
        Dim IdValue As Integer = 0
        Dim IdField As String = String.Empty
        If CType(Me.Page, SitePage).IsLoggedInBuilder AndAlso Not Session("BuilderAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("BuilderAccountId"))
            IdField = "BuilderAccountId"
        ElseIf CType(Me.Page, SitePage).IsLoggedInVendor AndAlso Not Session("VendorAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("VendorAccountId"))
            IdField = "VendorAccountId"
        ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ AndAlso Not Session("PIQAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("PIQAccountId"))
            IdField = "PIQAccountId"
        End If
        If IdValue <= 0 Then Exit Sub
        dt = NotificationRow.GetActiveListByAccountId(DB, IdValue, IdField)
        rptNotification.DataSource = dt
        rptNotification.DataBind()
    End Sub

    Protected Sub rptNotification_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptNotification.ItemCommand
        If e.CommandName = "Close" Then
            Dim dbNotification As NotificationRow = NotificationRow.GetRow(DB, e.CommandArgument)
            dbNotification.ProcessDate = Now
            dbNotification.Update()
            BindRepeater()
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not dt Is Nothing AndAlso dt.Rows.Count = 0 Then divNotification.Visible = False
    End Sub
End Class
