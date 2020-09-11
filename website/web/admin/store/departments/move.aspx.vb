Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Public Class Move
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("STORE")

        Dim DepartmentId As Integer = CType(Request.QueryString("DepartmentId"), Integer)
        Dim ACTION As String = Request.QueryString("ACTION")

        Try
            DB.BeginTransaction()

            If StoreDepartmentRow.ChangeDepartmentSortOrder(DB, "DepartmentId", DepartmentId, ACTION) Then
                'Invalidate cached menu
                Context.Cache.Remove("HeaderMenuCache")
                DB.CommitTransaction()
            Else
                DB.RollbackTransaction()
            End If

            Response.Redirect("default.aspx?DepartmentId=" & DepartmentId)

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Catch ex As ApplicationException
            AddError(ex.Message)
        Finally
        End Try

    End Sub
End Class