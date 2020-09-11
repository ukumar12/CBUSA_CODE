Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class POQuoteRequestRow
        Inherits POQuoteRequestRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal QuoteRequestId As Integer)
            MyBase.New(DB, QuoteRequestId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal QuoteRequestId As Integer) As POQuoteRequestRow
            Dim row As POQuoteRequestRow

            row = New POQuoteRequestRow(DB, QuoteRequestId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal QuoteRequestId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM POQuoteRequest WHERE QuoteRequestId = " & DB.Number(QuoteRequestId)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, QuoteRequestId)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from POQuoteRequest"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

End Namespace


