Imports Components
Imports DataLayer

Partial Class controls_VendorInvoices
    Inherits BaseControl

    Public Property SalesReportId() As Integer
        Get
            Return ViewState("SalesReportId")
        End Get
        Set(ByVal value As Integer)
            ViewState("SalesReportId") = value
        End Set
    End Property

    Public Property BuilderId() As Integer
        Get
            Return ViewState("BuilderId")
        End Get
        Set(ByVal value As Integer)
            ViewState("BuilderId") = value
        End Set
    End Property

    Public Property EditMode() As Boolean
        Get
            Return ViewState("EditMode")
        End Get
        Set(ByVal value As Boolean)
            ViewState("EditMode") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub DataBind()
        MyBase.DataBind()

        Dim dt
    End Sub
End Class
