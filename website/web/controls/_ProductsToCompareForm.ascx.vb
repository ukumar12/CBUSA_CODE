Imports Components
Imports DataLayer

Partial Class controls_ProductsToCompareForm
    Inherits BaseControl

    Private BuilderId As Integer
    Private tmpCurTitle As String

    Public Event ProductsAdded As EventHandler

    Public Property VendorId() As Integer
        Get
            Return ViewState("ProductsToCompareVendorId")
        End Get
        Set(ByVal value As Integer)
            ViewState("ProductsToCompareVendorId") = value
            BindProducts()
            If ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then upProducts.Update()
        End Set
    End Property

    Private Property ActiveFilter() As String
        Get
            Return ViewState("ActiveCompareProductFilter")
        End Get
        Set(ByVal value As String)
            ViewState("ActiveCompareProductFilter") = value
        End Set
    End Property

    Private dtProducts As DataTable
    Public ReadOnly Property Products() As DataTable
        Get
            Return dtProducts
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        BuilderId = Session("BuilderId")
        If BuilderId = Nothing Then
            BuilderId = Session("TakeoffForId")
            If BuilderId = Nothing Then
                Response.Redirect("select-builder.aspx")
            End If
        End If
        acProjects.WhereClause = " BuilderId=" & DB.Number(BuilderId)
        acOrders.WhereClause = " BuilderId=" & DB.Number(BuilderId)
        acTakeoffs.WhereClause = " BuilderId=" & DB.Number(BuilderId)
    End Sub

    Protected Sub acOrders_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acOrders.ValueUpdated
        ActiveFilter = "Orders"
    End Sub

    Private Sub BindProducts()
        Select Case ActiveFilter
            Case "Projects"
                dtProducts = ProjectRow.GetProjectProducts(DB, acProjects.Value, VendorId)
            Case "Orders"

            Case "Takeoffs"
                dtProducts = TakeOffRow.GetTakeoffProducts(DB, acTakeoffs.Value, VendorId)
        End Select
        rptProducts.DataSource = dtProducts
        rptProducts.DataBind()
    End Sub

    Protected Sub acProjects_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acProjects.ValueUpdated
        ActiveFilter = "Projects"
        BindProducts()
        ltlSelected.Text = " Project '" & acProjects.Text & "'<br/>"
        If dtProducts.Rows.Count = 0 Then
            ltlSelected.Text &= "<b class=""smaller"">No Products could be found.</b>"
        End If

        upProducts.Update()
    End Sub

    Protected Sub acTakeoffs_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acTakeoffs.ValueUpdated
        ActiveFilter = "Takeoffs"
        BindProducts()
        ltlSelected.Text = " TakeOff '" & acTakeoffs.Text & "'<br/>"
        If dtProducts.Rows.Count = 0 Then
            ltlSelected.Text &= "<b class=""smaller"">No Products could be found.</b>"
        End If

        upProducts.Update()
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        BindProducts()
        RaiseEvent ProductsAdded(Me, EventArgs.Empty)
    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        If ActiveFilter = "Projects" AndAlso tmpCurTitle <> e.Item.DataItem("Title") Then
            Dim tr As HtmlTableRow = e.Item.FindControl("trTitle")
            tr.Visible = True
            Dim span As HtmlGenericControl = e.Item.FindControl("spanTitle")
            span.InnerHtml = e.Item.DataItem("Title")
            tmpCurTitle = e.Item.DataItem("Title")
        End If
    End Sub
End Class
