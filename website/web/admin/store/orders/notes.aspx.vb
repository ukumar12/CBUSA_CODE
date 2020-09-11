Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO
Imports System.Data.SqlClient

Partial Class Notes
    Inherits AdminPage

    Private OrderId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("ORDERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            OrderId = Request("OrderId")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "NoteDate"
                gvList.SortOrder = "DESC"
            End If
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT son.*, a.LastName + ' ' + a.FirstName as FullName"
        SQL = " FROM StoreOrderNote son, Admin a where son.OrderId = " & DB.Number(OrderId) & " and son.AdminId = a.AdminId"

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

End Class

