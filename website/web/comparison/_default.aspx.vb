Imports Components
Imports DataLayer

Partial Class comparison_default
    Inherits SitePage

    Protected ReadOnly Property PriceComparisonId() As Integer
        Get
            If Session("PriceComparisonId") Is Nothing Then
                Session("PriceComparisonId") = Comparison.PriceComparisonID
            End If
            Return Session("PriceComparisonId")
        End Get
    End Property

    Private m_Comparison As PriceComparisonRow
    Public ReadOnly Property Comparison() As PriceComparisonRow
        Get
            If m_Comparison Is Nothing OrElse m_Comparison.PriceComparisonID = Nothing Then
                If Session("PriceComparisonId") = Nothing Then
                    m_Comparison = New PriceComparisonRow(DB)
                    m_Comparison.BuilderID = IIf(Session("TakeoffForId") Is Nothing, Session("BuilderId"), Session("TakeoffForId"))
                    m_Comparison.AdminID = Session("AdminId")
                    m_Comparison.Insert()
                Else
                    m_Comparison = New PriceComparisonRow(DB, PriceComparisonId)
                End If
            End If
            Return m_Comparison
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not ScriptManager.GetCurrent(Page).IsInAsyncPostBack And Not IsPostBack Then
            lsVendors.DataSource = VendorRow.GetList(DB)
            lsVendors.DataTextField = "CompanyName"
            lsVendors.DataValueField = "VendorId"
            lsVendors.DataBind()

            Dim dt As DataTable = TakeOffRow.GetTakeoffProducts(DB, Session("CurrentTakeoffID"))
            rptMain.DataSource = dt
            rptMain.DataBind()
        End If
    End Sub

    Protected Sub lsVendors_SelectedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lsVendors.SelectedChanged
    End Sub

    Protected Sub rptMain_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMain.ItemCreated
        If lsVendors.SelectedValues = String.Empty Then
            Exit Sub
        End If
        If e.Item.ItemType = ListItemType.Header Then
            Dim rptHeader As Repeater = e.Item.FindControl("rptHeader")
            'rptHeader.DataSource = VendorRow.GetVendorNames(DB, lsVendors.SelectedValues)
        ElseIf e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then

        End If
    End Sub
End Class
