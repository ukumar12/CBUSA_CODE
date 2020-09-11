Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceDocumentRow
        Inherits TwoPriceDocumentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TwoPriceDocumentId As Integer)
            MyBase.New(DB, TwoPriceDocumentId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal DocumentId As Integer) As TwoPriceDocumentRow
            Dim row As TwoPriceDocumentRow

            row = New TwoPriceDocumentRow(DB, DocumentId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal DocumentId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM   TwoPriceDocument WHERE  DocumentId = " & DB.Number(DocumentId)
            DB.ExecuteSQL(SQL)
        End Sub
        Public Shared Sub RemoveByCampaignId(ByVal DB As Database, ByVal TwoPriceCampaignId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM   TwoPriceDocument WHERE  twopriceCampaignId = " & DB.Number(TwoPriceCampaignId)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, DocumentId)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from  TwoPriceDocument"
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


