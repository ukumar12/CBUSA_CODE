Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PriceComparisonVendorProductPriceRow
        Inherits PriceComparisonVendorProductPriceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, ByVal TakeoffProductId As Integer)
            MyBase.New(DB, PriceComparisonId, VendorId, TakeoffProductId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, ByVal TakeoffProductId As Integer) As PriceComparisonVendorProductPriceRow
            Dim row As PriceComparisonVendorProductPriceRow

            row = New PriceComparisonVendorProductPriceRow(DB, PriceComparisonId, VendorId, TakeoffProductId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, ByVal TakeoffProductId As Integer)
            Dim row As PriceComparisonVendorProductPriceRow

            row = New PriceComparisonVendorProductPriceRow(DB, PriceComparisonId, VendorId, TakeoffProductId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PriceComparisonVendorProductPrice"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        'Public Shared Function GetRow(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, ByVal ProductId As Integer) As PriceComparisonVendorProductPriceRow
        '    Dim ret As New PriceComparisonVendorProductPriceRow(DB)
        '    Dim sql As String = "select * from PriceComparisonVendorProductPrice where PriceComparisonId=" & DB.Number(PriceComparisonId) & " and VendorId=" & DB.Number(VendorId) & " and ProductId=" & DB.Number(ProductId)
        '    Dim sdr As SqlDataReader = DB.GetReader(sql)
        '    If sdr.Read() Then
        '        ret.Load(sdr)
        '    End If
        '    sdr.Close()
        '    Return ret
        'End Function

        Public Shared Function GetVendorRows(ByVal DB As Database, ByVal PriceComparisonID As Integer, ByVal VendorID As Integer) As PriceComparisonVendorProductPriceCollection
            Dim out As New PriceComparisonVendorProductPriceCollection
            Dim sql As String = "select * from PriceComparisonVendorProductPrice where PriceCOmparisonID=" & DB.Number(PriceComparisonID) & " and VendorID=" & DB.Number(VendorID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                Dim dbRow As New PriceComparisonVendorProductPriceRow(DB)
                dbRow.Load(sdr)
                out.Add(dbRow)
            End While
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetVendorProductState(ByVal DB As Database, ByVal PriceComparisonID As Integer)
            Dim sql As String = _
                  "select " _
                & "     p.*," _
                & "     r.VendorProductPriceRequestID" _
                & " from PriceComparisonVendorProductPrice p Inner Join Vendor v On p.VendorId = v.VendorId left outer join VendorProductPriceRequest r on p.TakeoffProductID=r.TakeoffProductID and p.VendorID=r.VendorID " _
                & "  where v.IsActive = 1 " _
                & "   And  p.PriceComparisonID=" & DB.Number(PriceComparisonID)

            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class PriceComparisonVendorProductPriceRowBase
        Private m_DB As Database
        Private m_TakeoffProductID As Integer = Nothing
        Private m_PriceComparisonID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_SubstituteProductID As Integer = Nothing
        Private m_SupplyPhaseID As Integer = Nothing
        Private m_RecommendedQuantity As Integer = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_UnitPrice As Double = Nothing
        Private m_Total As Double = Nothing
        Private m_State As Controls.ProductState = Nothing
        Private m_IsNew As Boolean = True

        Public ReadOnly Property IsNew() As Boolean
            Get
                Return m_IsNew
            End Get
        End Property

        Public Property TakeoffProductId() As Integer
            Get
                Return m_TakeoffProductID
            End Get
            Set(ByVal Value As Integer)
                m_TakeoffProductID = Value
            End Set
        End Property

        Public Property PriceComparisonID() As Integer
            Get
                Return m_PriceComparisonID
            End Get
            Set(ByVal Value As Integer)
                m_PriceComparisonID = value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
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

        Public Property SpecialOrderProductID() As Integer
            Get
                Return m_SpecialOrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_SpecialOrderProductID = value
            End Set
        End Property

        Public Property SubstituteProductID() As Integer
            Get
                Return m_SubstituteProductID
            End Get
            Set(ByVal Value As Integer)
                m_SubstituteProductID = value
            End Set
        End Property

        Public Property SupplyPhaseID() As Integer
            Get
                Return m_SupplyPhaseID
            End Get
            Set(ByVal Value As Integer)
                m_SupplyPhaseID = value
            End Set
        End Property

        Public Property RecommendedQuantity() As Integer
            Get
                Return m_RecommendedQuantity
            End Get
            Set(ByVal Value As Integer)
                m_RecommendedQuantity = value
            End Set
        End Property

        Public Property Quantity() As Integer
            Get
                Return m_Quantity
            End Get
            Set(ByVal Value As Integer)
                m_Quantity = value
            End Set
        End Property

        Public Property UnitPrice() As Double
            Get
                Return m_UnitPrice
            End Get
            Set(ByVal Value As Double)
                m_UnitPrice = value
            End Set
        End Property

        Public Property Total() As Double
            Get
                Return m_Total
            End Get
            Set(ByVal Value As Double)
                m_Total = value
            End Set
        End Property

        Public Property State() As Controls.ProductState
            Get
                Return m_State
            End Get
            Set(ByVal value As Controls.ProductState)
                m_State = value
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

        Public Sub New(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, ByVal TakeoffProductId As Integer)
            m_DB = DB
            m_PriceComparisonID = PriceComparisonId
            m_VendorID = VendorId
            m_TakeoffProductID = TakeoffProductId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PriceComparisonVendorProductPrice WHERE TakeoffProductId = " & DB.Number(TakeoffProductId) & " AND PriceComparisonId=" & DB.Number(PriceComparisonID) & " AND VendorId=" & DB.Number(VendorID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                m_IsNew = False
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TakeoffProductID = Convert.ToInt32(r.Item("TakeoffProductId"))
            m_PriceComparisonID = Convert.ToInt32(r.Item("PriceComparisonID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            If IsDBNull(r.Item("ProductID")) Then
                m_ProductID = Nothing
            Else
                m_ProductID = Convert.ToInt32(r.Item("ProductID"))
            End If
            If IsDBNull(r.Item("SpecialOrderProductID")) Then
                m_SpecialOrderProductID = Nothing
            Else
                m_SpecialOrderProductID = Convert.ToInt32(r.Item("SpecialOrderProductID"))
            End If
            If IsDBNull(r.Item("SubstituteProductID")) Then
                m_SubstituteProductID = Nothing
            Else
                m_SubstituteProductID = Convert.ToInt32(r.Item("SubstituteProductID"))
            End If
            If IsDBNull(r.Item("SupplyPhaseID")) Then
                m_SupplyPhaseID = Nothing
            Else
                m_SupplyPhaseID = Convert.ToInt32(r.Item("SupplyPhaseID"))
            End If
            If IsDBNull(r.Item("RecommendedQuantity")) Then
                m_RecommendedQuantity = Nothing
            Else
                m_RecommendedQuantity = Convert.ToInt32(r.Item("RecommendedQuantity"))
            End If
            m_Quantity = Convert.ToInt32(r.Item("Quantity"))
            m_UnitPrice = Convert.ToDouble(r.Item("UnitPrice"))
            m_Total = Convert.ToDouble(r.Item("Total"))
            If IsDBNull(r.Item("State")) Then
                m_State = Nothing
            Else
                m_State = Convert.ToInt32(r.Item("State"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PriceComparisonVendorProductPrice (" _
             & " PriceComparisonID" _
             & ",VendorID" _
             & ",TakeoffProductID" _
             & ",ProductID" _
             & ",SpecialOrderProductID" _
             & ",SubstituteProductID" _
             & ",SupplyPhaseID" _
             & ",RecommendedQuantity" _
             & ",Quantity" _
             & ",UnitPrice" _
             & ",Total" _
             & ",State" _
             & ") VALUES (" _
             & m_DB.NullNumber(PriceComparisonID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(TakeoffProductId) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(SpecialOrderProductID) _
             & "," & m_DB.NullNumber(SubstituteProductID) _
             & "," & m_DB.NullNumber(SupplyPhaseID) _
             & "," & m_DB.Number(RecommendedQuantity) _
             & "," & m_DB.Number(Quantity) _
             & "," & m_DB.Number(UnitPrice) _
             & "," & m_DB.Number(Total) _
             & "," & m_DB.Number(State) _
             & ")"

            m_DB.ExecuteSQL(SQL)

            Return Nothing
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PriceComparisonVendorProductPrice SET " _
             & " ProductID = " & m_DB.NullNumber(ProductID) _
             & ",SpecialOrderProductID = " & m_DB.NullNumber(SpecialOrderProductID) _
             & ",SubstituteProductID = " & m_DB.NullNumber(SubstituteProductID) _
             & ",SupplyPhaseID = " & m_DB.NullNumber(SupplyPhaseID) _
             & ",RecommendedQuantity = " & m_DB.Number(RecommendedQuantity) _
             & ",Quantity = " & m_DB.Number(Quantity) _
             & ",UnitPrice = " & m_DB.Number(UnitPrice) _
             & ",Total = " & m_DB.Number(Total) _
             & ",State = " & m_DB.Number(State) _
             & " WHERE PriceComparisonID = " & m_DB.Quote(PriceComparisonID) _
             & " AND VendorID = " & DB.Quote(VendorID) _
             & " AND TakeoffProductID = " & DB.Quote(TakeoffProductId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PriceComparisonVendorProductPrice WHERE PriceComparisonID = " & m_DB.Number(PriceComparisonID) & " AND VendorID = " & m_DB.Number(VendorID) & " AND TakeoffProductId = " & m_DB.Number(TakeoffProductId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PriceComparisonVendorProductPriceCollection
        Inherits GenericCollection(Of PriceComparisonVendorProductPriceRow)
    End Class

End Namespace


