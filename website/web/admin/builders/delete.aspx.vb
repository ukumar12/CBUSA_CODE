Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports Utilities

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDERS")

        Dim BuilderID As Integer = Convert.ToInt32(Request("BuilderID"))
        'Try

        DB.BeginTransaction()
        Dim dbBuilderRow As BuilderRow = BuilderRow.GetRow(DB, BuilderID)
        If dbBuilderRow.BuilderID > 0 Then
            dbBuilderRow.IsActive = False
            dbBuilderRow.Update()
            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If sfHelper.DeleteBuilderRow(dbBuilderRow) = False Then
            '        'throw error
            '    End If
            'End If
        End If
        DB.CommitTransaction()

        'DB.BeginTransaction()
        'BuilderRow.RemoveRow(DB, BuilderID)
        'DB.CommitTransaction()

        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        'Catch ex As SqlException
        'If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
        'AddError(ErrHandler.ErrorText(ex))
        'End Try
    End Sub
End Class
