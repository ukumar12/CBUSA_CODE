Imports Components
Imports DataLayer

Partial Class substitutions_default
	Inherits SitePage


	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		EnsureBuilderAccess()

		gvList.BindList = AddressOf BindData
		If Not IsPostBack Then
			gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
			gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
			If gvList.SortBy = String.Empty Then gvList.SortBy = "ProductId"

			BindData()
		End If
	End Sub

	Private Sub BindData()
		ViewState("F_SortBy") = gvList.SortBy
		ViewState("F_SortOrder") = gvList.SortOrder

		Dim res As DataTable = BuilderRow.GetBuilderSubstitutions(DB, Session("BuilderId"), gvList.SortBy, gvList.SortOrder)
		gvList.DataSource = res.DefaultView
		gvList.DataBind()
	End Sub
End Class
