Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer

Partial Class move
    Inherits AdminPage

    Private objTemplate As StoreItemTemplateRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("STORE")

        Dim TemplateAttributeId As Integer = Convert.ToInt32(Request("TemplateAttributeId"))
        Dim Action As String = Request("ACTION")
        objTemplate = StoreItemTemplateRow.GetRow(DB, Request("TemplateId"))
        If objTemplate.TemplateId = Nothing Then
            DB.Close()
            Response.Redirect("/admin/store/template/")
        End If

        Try
            DB.BeginTransaction()
            If Core.ChangeSortOrder(DB, "TemplateAttributeId", "StoreItemTemplateAttribute", "SortOrder", "TemplateId=" & objTemplate.TemplateId.ToString, TemplateAttributeId, Action) Then
                DB.CommitTransaction()
            Else
                DB.RollbackTransaction()
            End If
            Response.Redirect("default.aspx?TemplateId=" & objTemplate.TemplateId & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
