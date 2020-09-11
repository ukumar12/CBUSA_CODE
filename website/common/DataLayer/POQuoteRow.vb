Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class POQuoteRow
        Inherits POQuoteRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal QuoteId As Integer)
            MyBase.New(DB, QuoteId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal QuoteId As Integer) As POQuoteRow
            Dim row As POQuoteRow

            row = New POQuoteRow(DB, QuoteId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal QuoteId As Integer)
            Dim SQL As String

            SQL = "DELETE FROM POQuote WHERE QuoteId = " & DB.Number(QuoteId)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, QuoteId)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from POQuote"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Function GetListByBuilder(ByVal DB As Database, ByVal BuilderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from POQuote Where ProjectId In (select ProjectId from Project Where BuilderId = " & DB.Number(BuilderId) & ")"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        Public ReadOnly Property GetSelectedVendorCategories() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select VendorCategoryID from POQuoteVendorCategory where QuoteId = " & QuoteId)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("VendorCategoryID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllVendorCategories()
            DB.ExecuteSQL("delete from POQuoteVendorCategory where QuoteId = " & QuoteId)
        End Sub

        Public Sub InsertToVendorCategories(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToVendorCategory(Element)
            Next
        End Sub

        Public Sub InsertToVendorCategory(ByVal VendorCategoryID As Integer)
            Dim SQL As String = "insert into POQuoteVendorCategory (QuoteId, VendorCategoryID) values (" & QuoteId & "," & VendorCategoryID & ")"
            DB.ExecuteSQL(SQL)
        End Sub
        Public Sub DeleteFromAllPOQuoteVendors()
            DB.ExecuteSQL("delete from POQuoteVendor where QuoteId = " & QuoteId)
        End Sub

        Public Sub InsertToPOQuoteVendors(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                If Not String.IsNullOrEmpty(Element) Then
                    InsertToPOQuoteVendors(Core.GetInt(Element))
                End If
            Next
        End Sub

        Public Sub InsertToPOQuoteVendors(ByVal VendorID As Integer)
            Dim SQL As String = "insert into POQuoteVendor (QuoteId, VendorId) values (" & QuoteId & "," & VendorID & ")"
            DB.ExecuteSQL(SQL)
        End Sub
    End Class

End Namespace


