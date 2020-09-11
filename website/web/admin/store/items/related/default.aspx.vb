Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class related
    Inherits AdminPage

    Protected F_ItemId As Integer
    Protected dbStoreItem As StoreItemRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        F_ItemId = CInt(Request("F_ItemId"))
        dbStoreItem = StoreItemRow.GetRow(DB, F_ItemId)

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            If gvList.SortBy = String.Empty Then gvList.SortBy = " ri.SortOrder"
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM StoreRelatedItem ri, StoreItem si where ri.ItemId = si.ItemId and ri.ParentId = " & F_ItemId
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            DB.BeginTransaction()
            Dim dbItem As StoreItemRow = StoreItemRow.GetRow(DB, F_ItemId)
            dbItem.InsertRelatedItem(Request.Form("ItemId"))
            DB.CommitTransaction()
            Response.Redirect("default.aspx?ItemId=" & F_ItemId & "&" & GetPageParams(FilterFieldType.All))
        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            AddError(ex.Message)
        Finally
            BindList()
        End Try
    End Sub
End Class