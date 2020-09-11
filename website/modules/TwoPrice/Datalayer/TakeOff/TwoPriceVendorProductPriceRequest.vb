
Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceVendorProductPriceRequestRow
        Inherits TwoPriceVendorProductPriceRequestRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TwoPriceVendorProductPriceRequestID As Integer)
            MyBase.New(DB, TwoPriceVendorProductPriceRequestID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceVendorProductPriceRequestID As Integer) As TwoPriceVendorProductPriceRequestRow
            Dim row As TwoPriceVendorProductPriceRequestRow

            row = New TwoPriceVendorProductPriceRequestRow(DB, TwoPriceVendorProductPriceRequestID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TwoPriceVendorProductPriceRequestID As Integer)
            Dim row As TwoPriceVendorProductPriceRequestRow

            row = New TwoPriceVendorProductPriceRequestRow(DB, TwoPriceVendorProductPriceRequestID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceVendorProductPriceRequest"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Shared Function GetRow(ByVal DB As Database, ByVal BuilderID As Integer, ByVal VendorID As Integer, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer)
            Dim out As New TwoPriceVendorProductPriceRequestRow(DB)
            Dim sql As String = "select * from TwoPriceVendorProductPriceRequest where BuilderID=" & DB.Number(BuilderID) & " and VendorID=" & DB.Number(VendorID) & " and TwoPriceBuilderTakeOffProductPendingID=" & DB.Number(TwoPriceBuilderTakeOffProductPendingID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetPriceRequests(ByVal DB As Database, ByVal VendorID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "", Optional ByVal PageNumber As Integer = 0, Optional ByVal PageSize As Integer = 10) As DataTable
            '************* 3rd Line of sql added by Apala (Medullus) for mGuard#1086 **************
            Dim base As String = " v.TwoPriceVendorProductPriceRequestID, v.TakeoffProductID, b.CompanyName, coalesce(p.Product, s.SpecialOrderProduct) as Product, v.Created, " _
                                & " case coalesce(s.SpecialOrderProductID, 0) when 0 then 'Regular' else 'Special' end as ProductType from " _
                                & " TwoPriceVendorProductPriceRequest v inner join Builder b on v.BuilderID=b.BuilderID" _
                                & "     left outer join Product p on v.ProductID=p.ProductID" _
                                & "     left outer join SpecialOrderProduct s on v.SpecialOrderProductID=s.SpecialOrderProductID" _
                                & " where " _
                                & "     v.VendorID=" & DB.Number(VendorID)
            Dim sql As String = "select " & base
            If SortBy <> String.Empty Then
                Dim orderby As String = Core.ProtectParam(SortBy & " " & SortOrder)
                If PageNumber <> Nothing Then
                    sql = "select top " & PageSize & " * from (select Row_Number() Over(order by " & orderby & ")," & base & ") as temp where temp.RowNumber>=" & DB.Number((PageNumber - 1) * PageSize) & " order by RowNumber"
                Else
                    sql = "select " & base & " order by " & orderby
                End If
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceTakeoffPriceRequests(ByVal DB As Database, ByVal VendorID As Integer, ByVal TakeoffID As Integer) As DataTable
            Dim sql As String = "select distinct TakeoffProductID from VendorProductPriceRequest where VendorID=" & DB.Number(VendorID) & " and TakeoffProductID in (select TakeoffProductID from TakeoffProduct where TakeoffID=" & DB.Number(TakeoffID) & ") order by TakeoffProductID asc"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceTakeoffPriceRequests(ByVal DB As Database, ByVal Vendors As String, ByVal TakeoffID As Integer) As DataTable
            Dim sql As String = "select TakeoffProductID, VendorID from TwoPriceVendorPriceRequest where VendorID in (" & DB.NumberMultiple(Vendors) & ") and TakeoffProductID in (select TakeoffProductID from TakeoffProduct where TakeoffID=" & DB.Number(TakeoffID) & ") order by TakeoffProductID asc"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceRequestsByProduct(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer) As DataTable
            Dim sql As String = "select * from TwoPriceVendorProductPriceRequest where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID)
            Return DB.GetDataTable(sql)
        End Function

        '************ Following function added by Apala (Medullus) for mGuard#T-1086
        Public Shared Function GetTwoPriceRequestsBySpecialOrderProduct(ByVal DB As Database, ByVal VendorID As Integer, ByVal SpecialOrderProductID As Integer) As DataTable
            Dim sql As String = "select * from TwoPriceVendorProductPriceRequest where VendorID=" & DB.Number(VendorID) & " and SpecialOrderProductID=" & DB.Number(SpecialOrderProductID)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Sub RemoveRequests(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer)
            Dim sql As String = "delete from TwoPriceVendorProductPriceRequest where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID)
            DB.ExecuteSQL(sql)
        End Sub

        Public Shared Sub RemoveProductRequests(ByVal DB As Database, ByVal ProductID As Integer)
            Dim sql As String = "delete from TwoPriceVendorProductPriceRequest where   ProductID=" & DB.Number(ProductID)
            DB.ExecuteSQL(sql)
        End Sub
    End Class

    Public MustInherit Class TwoPriceVendorProductPriceRequestRowBase
        Private m_DB As Database
        Private m_TwoPriceVendorProductPriceRequestID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorBuilderAccountID As Integer = Nothing
        Private m_TakeoffProductID As Integer = Nothing
        Private m_TwoPriceCampaignID As Integer = Nothing
        Private m_TwoPriceOrderID As Integer = Nothing
        Private m_TwoPriceBuilderTakeOffProductPendingID As Integer = Nothing
        Public Property TwoPriceVendorProductPriceRequestID() As Integer
            Get
                Return m_TwoPriceVendorProductPriceRequestID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceVendorProductPriceRequestID = Value
            End Set
        End Property
        Public Property TwoPriceOrderID() As Integer
            Get
                Return m_TwoPriceOrderID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceOrderID = Value
            End Set
        End Property
        Public Property TwoPriceBuilderTakeOffProductPendingID() As Integer
            Get
                Return m_TwoPriceBuilderTakeOffProductPendingID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceBuilderTakeOffProductPendingID = Value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = Value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = Value
            End Set
        End Property

        Public Property ProductID() As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = Value
            End Set
        End Property

        Public Property SpecialOrderProductID() As Integer
            Get
                Return m_SpecialOrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_SpecialOrderProductID = Value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property CreatorBuilderAccountID() As Integer
            Get
                Return m_CreatorBuilderAccountID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorBuilderAccountID = Value
            End Set
        End Property

        Public Property TakeoffProductID() As Integer
            Get
                Return m_TakeoffProductID
            End Get
            Set(ByVal value As Integer)
                m_TakeoffProductID = value
            End Set
        End Property
        Public Property TwoPriceCampaignID() As Integer
            Get
                Return m_TwoPriceCampaignID
            End Get
            Set(ByVal value As Integer)
                m_TwoPriceCampaignID = value
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

        Public Sub New(ByVal DB As Database, ByVal TwoPriceVendorProductPriceRequestID As Integer)
            m_DB = DB
            m_TwoPriceVendorProductPriceRequestID = TwoPriceVendorProductPriceRequestID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceVendorProductPriceRequest WHERE TwoPriceVendorProductPriceRequestID = " & DB.Number(TwoPriceVendorProductPriceRequestID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceVendorProductPriceRequestID = Core.GetInt(r.Item("TwoPriceVendorProductPriceRequestID"))
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_BuilderID = Core.GetInt(r.Item("BuilderID"))
            m_ProductID = Core.GetInt(r.Item("ProductID"))
            m_SpecialOrderProductID = Core.GetInt(r.Item("SpecialOrderProductID"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_CreatorBuilderAccountID = Core.GetInt(r.Item("CreatorBuilderAccountID"))
            m_TakeoffProductID = Core.GetInt(r.Item("TakeoffProductID"))
            m_TwoPriceCampaignID = Core.GetInt(r.Item("TwoPriceCampaignID"))
            m_TwoPriceOrderID = Core.GetInt(r.Item("TwoPriceOrderID"))
            m_TwoPriceBuilderTakeOffProductPendingID = Core.GetInt(r.Item("TwoPriceBuilderTakeOffProductPendingID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TwoPriceVendorProductPriceRequest (" _
             & " VendorID" _
             & ",BuilderID" _
             & ",ProductID" _
             & ",SpecialOrderProductID" _
             & ",Created" _
             & ",CreatorBuilderAccountID" _
             & ",TakeoffProductID" _
              & ",TwoPriceCampaignID" _
             & ",TwoPriceOrderID" _
            & ",TwoPriceBuilderTakeOffProductPendingID" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(SpecialOrderProductID) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorBuilderAccountID) _
             & "," & m_DB.NullNumber(TakeoffProductID) _
              & "," & m_DB.NullNumber(TwoPriceCampaignID) _
               & "," & m_DB.NullNumber(TwoPriceOrderID) _
                   & "," & m_DB.NullNumber(TwoPriceBuilderTakeOffProductPendingID) _
             & ")"

            TwoPriceVendorProductPriceRequestID = m_DB.InsertSQL(SQL)

            Return TwoPriceVendorProductPriceRequestID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceVendorProductPriceRequest SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",ProductID = " & m_DB.NullNumber(ProductID) _
             & ",SpecialOrderProductID = " & m_DB.NullNumber(SpecialOrderProductID) _
             & ",TakeoffProductID = " & m_DB.NullNumber(TakeoffProductID) _
             & ",TwoPriceCampaignID = " & m_DB.NullNumber(TwoPriceCampaignID) _
             & ",TwoPriceOrderID = " & m_DB.NullNumber(TwoPriceOrderID) _
             & ",TwoPriceBuilderTakeOffProductPendingID = " & m_DB.NullNumber(TwoPriceBuilderTakeOffProductPendingID) _
             & " WHERE TwoPriceVendorProductPriceRequestID = " & m_DB.Quote(TwoPriceVendorProductPriceRequestID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceVendorProductPriceRequest  WHERE TwoPriceVendorProductPriceRequestID = " & m_DB.Number(TwoPriceVendorProductPriceRequestID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class TwoPriceVendorProductPriceRequestCollection
        Inherits GenericCollection(Of TwopriceVendorProductPriceRequestRow)
    End Class

End Namespace


