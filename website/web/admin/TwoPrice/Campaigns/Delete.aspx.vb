Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports TwoPrice.DataLayer

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("TWO_PRICE_CAMPAIGNS")

        Dim TwoPriceCampaignId As Integer = Convert.ToInt32(Request("TwoPriceCampaignId"))
        Try
            DB.BeginTransaction()
            TwoPriceDocumentRow.RemoveByCampaignId(DB, TwoPriceCampaignId)
            TwoPriceCampaignRow.RemoveRow(DB, TwoPriceCampaignId)

            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class