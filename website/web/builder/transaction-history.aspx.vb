Imports Components
Imports DataLayer
Imports Vindicia
Imports Vindicia_OLD

Partial Class builder_transaction_history
    Inherits SitePage


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        EnsureBuilderAccess()

        If Not IsPostBack Then
            BindTransactions()
        End If
    End Sub

    Private Sub BindTransactions()
        'Dim p As New VindiciaPaymentProcessor(DB)
        'p.IsTestMode = SysParam.GetValue(DB, "TestMode")

        'Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

        'Dim t As AllDataTypes.Transaction() = p.GetTransactionHistory(dbBuilder)
        'gvTransactions.DataSource = t
        'gvTransactions.DataBind()
    End Sub
End Class
