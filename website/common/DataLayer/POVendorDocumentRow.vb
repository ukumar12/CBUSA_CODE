Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class POVendorDocumentRow
        Inherits POVendorDocumentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorDocumentId As Integer)
            MyBase.New(DB, VendorDocumentId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorDocumentId As Integer) As POVendorDocumentRow
            Dim row As POVendorDocumentRow

            row = New POVendorDocumentRow(DB, VendorDocumentId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorDocumentId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM POVendorDocument WHERE VendorDocumentId = " & DB.Number(VendorDocumentId)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, VendorDocumentId)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from POVendorDocument"
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


