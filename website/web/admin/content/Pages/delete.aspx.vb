Imports Components
Imports System.Data
Imports System.Data.SqlClient
Imports DataLayer
Imports System.IO

Partial Class delete
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("CONTENT_TOOL")

        Dim PageId As Integer = Convert.ToInt32(Request("PageId"))
        Dim dbContentToolPage As ContentToolPageRow = ContentToolPageRow.GetRow(DB, PageId)

        'Don't delte page if permanent
        If dbContentToolPage.IsPermanent Then
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
        End If

        Try
            If Not dbContentToolPage.PageURL = String.Empty Then
                Dim FullPath As String = Server.MapPath(dbContentToolPage.PageURL)

                'If file has been generated and not changes, then remove file as well
                Try
                Dim sr As StreamReader = File.OpenText(FullPath)
                Dim fc As String = sr.ReadToEnd
                sr.Close()
                If fc = MasterPages.MasterPage.BlankFileContent Then
                    File.Delete(FullPath)
                End If
                Catch ex As System.IO.FileNotFoundException
                    Logger.Warning("Tried to delete file: " & dbContentToolPage.PageURL & " which does not exist.")
                End Try
            End If

            DB.BeginTransaction()
            ContentToolPageRow.RemoveRow(DB, PageId)
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
