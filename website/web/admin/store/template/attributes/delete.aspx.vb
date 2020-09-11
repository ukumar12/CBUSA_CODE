Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class delete
    Inherits AdminPage

    Private objTemplate As StoreItemTemplateRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        Dim TemplateAttributeId As Integer = Convert.ToInt32(Request("TemplateAttributeId"))
        objTemplate = StoreItemTemplateRow.GetRow(DB, Request("TemplateId"))
        If objTemplate.TemplateId = Nothing Then
            DB.Close()
            Response.Redirect("/admin/store/template/")
        End If

        Try
            DB.BeginTransaction()
            StoreItemTemplateAttributeRow.RemoveRow(DB, TemplateAttributeId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?TemplateId=" & objTemplate.TemplateId.ToString & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
