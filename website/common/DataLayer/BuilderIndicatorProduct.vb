Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderIndicatorProductRow
        Inherits BuilderIndicatorProductRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderIndicatorProductID As Integer)
            MyBase.New(DB, BuilderIndicatorProductID)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer)
            MyBase.New(DB, BuilderID, SortIndex)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderIndicatorProductID As Integer) As BuilderIndicatorProductRow
            Dim row As BuilderIndicatorProductRow

            row = New BuilderIndicatorProductRow(DB, BuilderIndicatorProductID)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderID As Integer, ByVal ProductID As Integer) As BuilderIndicatorProductRow
            Dim row As New BuilderIndicatorProductRow(DB)
            Dim sql As String = "select * from BuilderIndicatorProduct where BUilderID=" & DB.Number(BuilderID) & " and ProductID=" & DB.Number(ProductID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                row.Load(sdr)
            End If
            sdr.Close()
            Return row
        End Function

        'Shared function to get one row by builder and sort order
        Public Shared Function GetRowByBuilder(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer) As BuilderIndicatorProductRow
            Dim row As BuilderIndicatorProductRow

            row = New BuilderIndicatorProductRow(DB, BuilderID, SortIndex)
            row.LoadByBuilderIndex()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderIndicatorProductID As Integer)
            Dim row As BuilderIndicatorProductRow

            row = New BuilderIndicatorProductRow(DB, BuilderIndicatorProductID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderIndicatorProduct"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Sub DeleteByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            Dim SQL As String = "DELETE FROM BuilderIndicatorProduct WHERE BuilderID = " & BuilderID
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetBuilderRows(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = "select p.Product, bip.* from BuilderIndicatorProduct bip inner join Product p on bip.ProductID=p.ProductID where bip.BuilderID=" & DB.Number(BuilderID)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTopPrices(ByVal DB As Database, ByVal BuilderIndicatorProductID As Integer) As DataTable
            Dim sql As String = _
                  " select top 3" _
                & " v.CompanyName as Vendor" _
                & ",vp.ProductID as ProductID" _
                & ",vs.ProductID as SubProductID " _
                & ",case vp.IsSubstitution when 0 then vp.VendorPrice else vs.VendorPrice end as VendorPrice" _
                & ",case vp.IsSubstitution " _
                & "     when 0 then (select Product from Product where ProductID=vp.ProductID)" _
                & "     when 1 then (select Product from Product where ProductID=vs.ProductID)" _
                & " end as Product" _
                & ",vp.IsSubstitution" _
                & ",case vp.IsSubstitution " _
                & "     when 0 then (select top 1 VendorPrice from VendorProductPriceHistory where ProductID=vp.ProductID and VendorID=v.VendorID) " _
                & "     when 1 then (select top 1 VendorPrice from VendorProductPriceHistory where ProductID=vs.ProductID and VendorID=v.VendorID) " _
                & " end as LastPrice" _
                & " from " _
                & "     VendorProductPrice vp left outer join (select vps.ProductID as OriginalProductID, vpsp.* from VendorProductSubstitute vps inner join VendorProductPrice vpsp on vps.SubstituteProductID=vpsp.ProductID where vpsp.VendorID=vps.VendorID) as vs on vs.OriginalProductID=vp.ProductID and vs.VendorId=vp.VendorId   " _
                & "         inner join (select ProductID, BuilderID from BuilderIndicatorProduct where BuilderIndicatorProductID=" & DB.Number(BuilderIndicatorProductID) & ") as temp on temp.ProductID=vp.ProductID" _
                & "         inner join Vendor v on v.VendorID = vp.VendorID" _
                & " where " _
                & "  v.IsActive =1 and v.VendorID in (select VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID where b.BuilderID=temp.BuilderID)" _
                & " order by VendorPrice Asc"

            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class BuilderIndicatorProductRowBase
        Private m_DB As Database
        Private m_BuilderIndicatorProductID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property BuilderIndicatorProductID() As Integer
            Get
                Return m_BuilderIndicatorProductID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderIndicatorProductID = value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property ProductID() As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
            End Set
        End Property


        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderIndicatorProductID As Integer)
            m_DB = DB
            m_BuilderIndicatorProductID = BuilderIndicatorProductID
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderID As Integer, ByVal SortIndex As Integer)
            m_DB = DB
            m_BuilderID = BuilderID
            m_SortOrder = SortIndex
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderIndicatorProduct WHERE BuilderIndicatorProductID = " & DB.Number(BuilderIndicatorProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Sub LoadByBuilderIndex()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT TOP 1 * FROM BuilderIndicatorProduct WHERE BuilderID = " & DB.Number(BuilderID) & " AND SortOrder = " & DB.Number(SortOrder)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderIndicatorProductID = Convert.ToInt32(r.Item("BuilderIndicatorProductID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_ProductID = Convert.ToInt32(r.Item("ProductID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from BuilderIndicatorProduct WHERE BuilderId = " & BuilderID & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO BuilderIndicatorProduct (" _
             & " BuilderID" _
             & ",ProductID" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & MaxSortOrder _
             & ")"

            BuilderIndicatorProductID = m_DB.InsertSQL(SQL)

            Return BuilderIndicatorProductID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderIndicatorProduct SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",ProductID = " & m_DB.NullNumber(ProductID) _
             & " WHERE BuilderIndicatorProductID = " & m_DB.quote(BuilderIndicatorProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderIndicatorProduct WHERE BuilderIndicatorProductID = " & m_DB.Number(BuilderIndicatorProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderIndicatorProductCollection
        Inherits GenericCollection(Of BuilderIndicatorProductRow)
    End Class

End Namespace