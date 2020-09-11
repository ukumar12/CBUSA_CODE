Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports System.Collections.Generic

Public Class Edit
    Inherits AdminPage

	Protected ProductID As Integer

	Public Property selected() As List(Of String)
		Get
			Return ViewState("selected")
		End Get
		Set(ByVal value As List(Of String))
			ViewState("selected") = value
		End Set
	End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PRODUCTS")
        'ctlAttributes.ProductTypeId = 2
		'ProductID = Convert.ToInt32(Request("ProductID"))
		If selected Is Nothing Then selected = New List(Of String)()
		gvList.BindList = AddressOf BindList
		gvList2.BindList = AddressOf BindList2
		LoadFromDB()
		If Not IsPostBack Then
			F_Product.Text = Request("F_Product")
			F_SKU.Text = Request("F_SKU")
			F_IsActive.Text = Request("F_IsActive")
			F_Manufacturer.Text = Request("F_Manufacturer")

			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			gvList2.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList2.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "ProductID"
			If gvList2.SortBy = String.Empty Then gvList2.SortBy = "ProductID"

			BindList()
			BindList2()
		End If
    End Sub

    Private Sub LoadFromDB()
		ctvSupplyPhase.DataSource = SupplyPhaseRow.GetList(DB)
        ctvSupplyPhase.DataTextName = "SupplyPhase"
        ctvSupplyPhase.DataValueName = "SupplyPhaseId"
        ctvSupplyPhase.ParentFieldName = "ParentSupplyPhaseId"
        ctvSupplyPhase.DataBind()
	End Sub

	Private Sub BindList()
		Dim SQLFields, SQL As String
		Dim Conn As String = " AND "

		ViewState("F_SortBy") = gvList.SortBy
		ViewState("F_SortOrder") = gvList.SortOrder

		SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " p.*, m.Manufacturer "
		SQL = " FROM Product p left outer join Manufacturer m on p.ManufacturerID=m.ManufacturerID "
		SQL &= " WHERE ProductId IN " & DB.NumberMultiple(Join(selected.ToArray(), ","))

		gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
		gvList.DataKeyNames = New String() {"ProductId"}

		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub

	Private Sub BindList2()
		Dim SQLFields, SQL As String
		Dim Conn As String = " where "

		ViewState("F_SortBy") = gvList2.SortBy
		ViewState("F_SortOrder") = gvList2.SortOrder

		SQLFields = "SELECT TOP " & (gvList2.PageIndex + 1) * gvList2.PageSize & " p.*, m.Manufacturer "
		SQL = " FROM Product p left outer join Manufacturer m on p.ManufacturerID=m.ManufacturerID "
		If selected.Count > 0 Then
			SQL &= Conn & "ProductId NOT IN " & DB.NumberMultiple(Join(selected.ToArray(), ","))
			Conn = " AND "
		End If


		If Not F_Product.Text = String.Empty Then
			SQL = SQL & Conn & "p.Product LIKE " & DB.FilterQuote(F_Product.Text)
			Conn = " AND "
		End If
		If Not F_SKU.Text = String.Empty Then
			SQL = SQL & Conn & "p.SKU LIKE " & DB.FilterQuote(F_SKU.Text)
			Conn = " AND "
		End If
		If Not F_IsActive.SelectedValue = String.Empty Then
			SQL = SQL & Conn & "p.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
			Conn = " AND "
		End If
		If Not F_Manufacturer.Value = Nothing Then
			SQL = SQL & Conn & "p.ManufacturerId=" & DB.Number(F_Manufacturer.Value)
			Conn = " AND "
		End If
		gvList2.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
		gvList2.DataKeyNames = New String() {"ProductId"}

		Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList2.SortByAndOrder)
		gvList2.DataSource = res.DefaultView
		gvList2.DataBind()
	End Sub

	Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
		If Not IsValid Then Exit Sub

		gvList2.PageIndex = 0
		BindList2()
	End Sub


	Protected Sub btnUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUp.Click
		For Each row As GridViewRow In gvList2.Rows
			Dim chkSelected As CheckBox = row.FindControl("chkSelected")
			If chkSelected.Checked Then
				selected.Add(gvList2.DataKeys(row.RowIndex).Value)
			End If
		Next
		BindList()
		BindList2()
	End Sub

	Protected Sub btnDown_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDown.Click
		For Each row As GridViewRow In gvList.Rows
			Dim chkSelected As CheckBox = row.FindControl("chkSelected")
			If chkSelected.Checked Then
				selected.Remove(gvList.DataKeys(row.RowIndex).Value)
			End If
		Next
		BindList()
		BindList2()
	End Sub

	Protected Sub Categorize_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Categorize.Click
		For Each item As String In selected
			Dim ID As Integer = Int32.Parse(item)
			Dim dbProduct As ProductRow = ProductRow.GetRow(DB, ID)
			dbProduct.DeleteFromAllSupplyPhases()
			dbProduct.InsertToSupplyPhases(ctvSupplyPhase.Value)
		Next

		Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub
End Class

