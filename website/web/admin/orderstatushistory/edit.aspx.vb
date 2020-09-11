Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected OrderStatusHistoryID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("ORDER_STATUS_HISTORYS")

        OrderStatusHistoryID = Convert.ToInt32(Request("OrderStatusHistoryID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If OrderStatusHistoryID = 0 Then
            Response.Redirect("default.aspx")
        End If

        Dim dbOrderStatusHistory As OrderStatusHistoryRow = OrderStatusHistoryRow.GetRow(DB, OrderStatusHistoryID)
        Dim dbOrder As OrderRow = OrderRow.GetRow(DB, dbOrderStatusHistory.OrderID)
        Dim dbStatus As OrderStatusRow = OrderStatusRow.GetRow(DB, dbOrderStatusHistory.OrderStatusID)
        Dim dbAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, dbOrderStatusHistory.CreatorVendorAccountID)
        ltlOrderID.Text = dbOrder.OrderNumber & " (Created: " & FormatDateTime(dbOrder.Created, DateFormat.ShortDate) & ")"
        ltlStatus.Text = dbStatus.OrderStatus
        ltlAccount.Text = dbAccount.Username & "(" & Core.BuildFullName(dbAccount.FirstName, String.Empty, dbAccount.LastName) & ")"
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    'Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
    '    Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    'End Sub

    'Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
    '    Response.Redirect("delete.aspx?OrderStatusHistoryID=" & OrderStatusHistoryID & "&" & GetPageParams(FilterFieldType.All))
    'End Sub
End Class

