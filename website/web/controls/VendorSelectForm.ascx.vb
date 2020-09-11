Imports Components
Imports DataLayer

Partial Class controls_VendorSelectForm
    Inherits BaseControl

    Public Event VendorsChanged As EventHandler

    Public ReadOnly Property Vendors() As String
        Get
            Return mbVendors.SelectedValues
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            mbVendors.DataSource = VendorRow.GetList(DB)
            mbVendors.DataTextField = "CompanyName"
            mbVendors.DataValueField = "VendorId"
            mbVendors.DataBind()
        End If
    End Sub

    Protected Sub mbVendors_VendorsChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mbVendors.VendorsChanged
        RaiseEvent VendorsChanged(sender, e)
    End Sub
End Class
