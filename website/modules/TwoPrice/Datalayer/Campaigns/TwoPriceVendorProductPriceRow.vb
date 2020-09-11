Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceVendorProductPriceRow
        Inherits TwoPriceVendorProductPriceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, VendorID As Integer, ByVal ProductID As Integer, ByVal TwoPriceCampaignId As Integer)
            MyBase.New(DB, VendorID, ProductID, TwoPriceCampaignId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer, ByVal TwoPriceCampaignId As Integer, Optional ByVal Submitted As Boolean = Nothing) As TwoPriceVendorProductPriceRow
            Dim row As New TwoPriceVendorProductPriceRow(DB)

            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceVendorProductPrice WHERE VendorID = " & DB.Number(VendorID) & " AND ProductID = " & DB.Number(ProductID) & " AND TwoPriceCampaignID = " & DB.Number(TwoPriceCampaignId) & IIf(Submitted <> Nothing, " AND Submitted = " & DB.NQuote(Submitted.ToString), String.Empty)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            Else
                row.TwoPriceProductPriceID = Nothing
            End If
            r.Close()

            Return row
        End Function

        Public Shared Function GetRow(ByVal DB As Database, ByVal ID As Integer) As TwoPriceVendorProductPriceRow
            Dim row As New TwoPriceVendorProductPriceRow(DB)

            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceVendorProductPrice WHERE TwoPriceProductPriceID = " & DB.Number(ID)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            Else
                row.TwoPriceProductPriceID = Nothing
            End If
            r.Close()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceVendorProductPrice WHERE VendorID = " & DB.Number(VendorID)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, VendorID)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceVendorProductPrice"
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
