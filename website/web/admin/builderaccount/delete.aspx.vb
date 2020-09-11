Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports Utilities

Partial Class delete
    Inherits AdminPage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private UpdatedBuilderAccountId As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDER_ACCOUNTS")

        Dim BuilderAccountID As Integer = Convert.ToInt32(Request("BuilderAccountID"))
        Try
            DB.BeginTransaction()
            Dim dbBuilderAccountRow As BuilderAccountRow = BuilderAccountRow.GetRow(DB, BuilderAccountID)
            If dbBuilderAccountRow.BuilderAccountID > 0 Then
                dbBuilderAccountRow.IsActive = False
                dbBuilderAccountRow.Update()

                PageURL = Request.Url.ToString()
                CurrentUserId = Session("AdminId")
                UserName = Session("Username")
                UpdatedBuilderAccountId = BuilderAccountID
                Core.DataLog("Admin", PageURL, CurrentUserId, "Delete Builder Account", UpdatedBuilderAccountId, "", "", "", UserName)

                '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.DeleteBuilderAccount(dbBuilderAccountRow) = False Then
                '        'throw error
                '    End If
                'End If
                '===============================================================================================
            End If
            DB.CommitTransaction()

            'DB.BeginTransaction()
            'BuilderAccountRow.RemoveRow(DB, BuilderAccountID)
            'DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
