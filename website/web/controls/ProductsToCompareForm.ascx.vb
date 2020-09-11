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
        End Set
    End Property

    Private m_OpenSpecialScript As String
    Public Property OpenSpecialScript() As String
        Get
            Return m_OpenSpecialScript
        End Get
        Set(ByVal value As String)
            m_OpenSpecialScript = value
        End Set
    End Property

    Private dtProducts As DataTable = Nothing
    Public ReadOnly Property Products() As DataTable
        Get
            If dtProducts Is Nothing Then
                If acTakeoffs.Value <> Nothing Then
                    dtProducts = TakeOffRow.GetTakeoffProducts(DB, acTakeoffs.Value)
                ElseIf acOrders.Value <> Nothing Then
                    dtProducts = OrderRow.GetOrderProducts(DB, acOrders.Value)
                ElseIf hlProjects.SelectedValue <> Nothing Then
                    dtProducts = ProjectRow.GetProjectProducts(DB, hlProjects.SelectedValue)
                End If
            End If
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
        If Not IsPostBack Then
            hlProjects.DataSource = ProjectRow.GetBuilderProjects(DB, Session("BuilderId"), "ProjectName")
            hlProjects.DataTextField = "ProjectName"
            hlProjects.DataValueField = "ProjectId"
            hlProjects.DataBind()
        End If
        acOrders.WhereClause = " BuilderId=" & DB.Number(BuilderId)
        acTakeoffs.WhereClause = " BuilderId=" & DB.Number(BuilderId)

        btnSpecial.OnClientClick = OpenSpecialScript
    End Sub

    'Protected Sub acOrders_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acOrders.ValueUpdated
    'End Sub

    'Protected Sub acProjects_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acProjects.ValueUpdated
    '    acOrders.WhereClause = " BuilderId=" & DB.Number(BuilderId) & " and ProjectId=" & DB.Number(acProjects.Value)
    '    acTakeoffs.WhereClause = " BuilderId=" & DB.Number(BuilderId) & " and ProjectId=" & DB.Number(acProjects.Value)
    '    'upFilterBar.Update()
    'End Sub

    'Protected Sub acTakeoffs_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acTakeoffs.ValueUpdated
    'End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnAdd.Click
        RaiseEvent ProductsAdded(Me, EventArgs.Empty)
        hlProjects.SelectedValue = Nothing
        acTakeoffs.Text = ""
        acOrders.Text = ""
        upFilterBar.Update()
    End Sub
End Class
