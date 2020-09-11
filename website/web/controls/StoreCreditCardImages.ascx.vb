Imports Components
Imports DataLayer

Partial Class StoreCreditCardImages
    Inherits BaseControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        Dim dt As DataTable = CreditCardTypeRow.GetAllCardTypes(DB)
        rptImages.DataSource = dt
        rptImages.DataBind()
    End Sub
End Class
