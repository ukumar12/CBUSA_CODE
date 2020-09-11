Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class SpecificItems
    Inherits AdminPage

    Protected PromotionId As Integer
    Protected dbPromotionRow As StorePromotionRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MARKETING_TOOLS")

        PromotionId = CInt(Request("PromotionId"))
        dbPromotionRow = StorePromotionRow.GetRow(DB, PromotionId)
        ltlTitle.Text = "Edit Specific Items/Departments for " & dbPromotionRow.PromotionCode

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            gvList.SortBy = "ItemName"
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim sSQL As String

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        sSQL = ""
        sSQL &= " SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize
        sSQL &= " spi.ID, spi.PromotionId, si.ItemId As RecordId, si.ItemName AS ItemName, 'Item' As ItemType FROM StoreItem si, StorePromotionItem spi"
        sSQL &= " where si.ItemId = spi.ItemId and spi.PromotionId = " & DB.Quote(PromotionId)
        sSQL &= " UNION "
        sSQL &= " SELECT spi.ID, spi.PromotionId, sd.DepartmentId As RecordId, sd.Name As ItemName, 'Department' As ItemType FROM StoreDepartment sd, StorePromotionItem spi "
        sSQL &= " where spi.DepartmentId = sd.DepartmentId and spi.PromotionId = " & DB.Quote(PromotionId)
        sSQL &= " ORDER BY " & gvList.SortByAndOrder

        Dim res As DataTable = DB.GetDataTable(sSQL)
        gvList.DataSource = res.DefaultView
        gvList.Pager.NofRecords = res.DefaultView.Count
        gvList.DataBind()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            DB.BeginTransaction()
            Dim dbPromotion As StorePromotionRow = StorePromotionRow.GetRow(DB, PromotionId)
            dbPromotion.InsertPromotionItem(Request.Form("ItemId"))
            DB.CommitTransaction()
            Response.Redirect("specific-items.aspx?PromotionId=" & PromotionId & "&" & GetPageParams(FilterFieldType.All))
        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            AddError(ex.Message)
        Finally
            BindList()
        End Try
    End Sub
End Class