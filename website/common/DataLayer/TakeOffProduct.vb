Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TakeOffProductRow
        Inherits TakeOffProductRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TakeOffProductID As Integer)
            MyBase.New(DB, TakeOffProductID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TakeOffProductID As Integer) As TakeOffProductRow
            Dim row As TakeOffProductRow

            row = New TakeOffProductRow(DB, TakeOffProductID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TakeOffProductID As Integer)
            Dim row As TakeOffProductRow

            row = New TakeOffProductRow(DB, TakeOffProductID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TakeOffProduct"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetCollection(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC", Optional ByVal TakeOffId As Integer = 0) As TakeOffProductCollection
            Dim SQL As String = "select * from TakeOffProduct TkP LEFT JOIN Product P ON TkP.ProductId = P.ProductId WHERE P.IsActive = 1"
            If TakeOffId <> Nothing Then
                SQL &= " AND TakeOffID=" & DB.Number(TakeOffId)
            End If
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Dim col As New TakeOffProductCollection
            Using sqlr As SqlDataReader = DB.GetReader(SQL)
                While sqlr.Read
                    Dim dbto As New TakeOffProductRow(DB)
                    dbto.Load(sqlr)
                    col.Add(dbto)
                End While
            End Using
            Return col
        End Function
        Public Shared Function GetRow(ByVal DB As Database, ByVal TakeOffId As Integer, ByVal ProductId As Integer) As TakeOffProductRow
            Dim out As New TakeOffProductRow(DB)
            Dim sql As String = "select * from TakeOffProduct where TakeOffId=" & DB.Number(TakeOffId) & " and ProductId=" & DB.Number(ProductId)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read() Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetTakeoffVendorPricing(ByVal DB As Database, ByVal TakeOffId As Integer, ByVal VendorId As Integer) As DataTable
            Dim sql As String = String.Empty
            sql &= " select" _
                 & "    t.TakeOffProductId, " _
                 & "    vs.ProductID as SubProductId," _
                 & "    t.ProductId as ProductID, " _
                 & "    t.SpecialOrderProductID, " _
                 & "    coalesce(vs.VendorPrice,v.VendorPrice) as VendorPrice, " _
                 & "    coalesce(vs.VendorSku,v.VendorSku) as VendorSku," _
                 & "    v.IsSubstitution, " _
                 & "    v.SubstituteQuantityMultiplier," _
                 & "    null as VendorSpecialPrice," _
                 & "    null as VendorSpecialSku," _
                 & "    t.Quantity" _
                 & " from " _
                 & "    TakeOffProduct t inner join VendorProductPrice as v on t.ProductID = v.ProductID" _
                 & "        left outer join (select * from VendorProductSubstitute where VendorId=" & DB.Number(VendorId) & ") as s on t.ProductId=s.ProductID " _
                 & "        left outer join (select * from VendorProductPrice where VendorId=" & DB.Number(VendorId) & ") as vs on s.SubstituteProductID=vs.ProductID" _
                 & " where " _
                 & "    t.TakeOffId=" & DB.Number(TakeOffId) _
                 & " and " _
                 & "    v.VendorID=" & DB.Number(VendorId) _
                 & " and " _
                 & "    coalesce(v.IsDiscontinued,0) = 0" _
                 & " union " _
                 & " select" _
                 & "    t.TakeOffProductId, " _
                 & "    NULL as SubProductId," _
                 & "    t.ProductId as ProductID, " _
                 & "    t.SpecialOrderProductID, " _
                 & "    vsp.VendorPrice, " _
                 & "    vsp.VendorSku," _
                 & "    1 as IsSubstitution, " _
                 & "    1 as SubstituteQuantityMultiplier," _
                 & "    vsp.VendorPrice as VendorSpecialPrice," _
                 & "    vsp.VendorSku as VendorSpecialSku," _
                 & "    t.Quantity" _
                 & " from " _
                 & "    TakeOffProduct t inner join (select * from VendorSpecialOrderProductPrice where VendorId=" & DB.Number(VendorId) & ") as vsp on vsp.SpecialOrderProductId=t.SpecialOrderProductId" _
                 & " where " _
                 & "    t.TakeOffId=" & DB.Number(TakeOffId) _
                 & " and " _
                 & "    vsp.IsSubstitution=0" _
                 & " union " _
                 & " select " _
                 & "    t.TakeOffProductId, " _
                 & "    vs.ProductID as SubProductId," _
                 & "    t.ProductId as ProductID, " _
                 & "    t.SpecialOrderProductID, " _
                 & "    vs.VendorPrice, " _
                 & "    vs.VendorSku," _
                 & "    1 as IsSubstitution, " _
                 & "    s.RecommendedQuantity as SubstituteQuantityMultiplier," _
                 & "    null as VendorSpecialPrice," _
                 & "    null as VendorSpecialSku," _
                 & "    t.Quantity" _
                 & " from " _
                 & "    TakeOffProduct t left outer join VendorProductPrice as v on t.ProductID = v.ProductID" _
                 & "        inner join (select * from VendorTakeoffProductSubstitute where VendorId=" & DB.Number(VendorId) & ") as s on t.TakeoffProductId=s.TakeoffProductID " _
                 & "        left outer join (select * from VendorProductPrice where coalesce(IsDiscontinued,0)=0 and VendorId=" & DB.Number(VendorId) & ") as vs on s.SubstituteProductID=vs.ProductID" _
                 & " where " _
                 & "    t.TakeOffId=" & DB.Number(TakeOffId) _
                 & " union " _
                 & " select" _
                 & "    t.TakeOffProductId, " _
                 & "    null as SubProductId," _
                 & "    t.ProductId as ProductID, " _
                 & "    t.SpecialOrderProductID, " _
                 & "    null as VendorPrice, " _
                 & "    null as VendorSku," _
                 & "    0 as IsSubstitution, " _
                 & "    null as SubstituteQuantityMultiplier," _
                 & "    null as VendorSpecialPrice," _
                 & "    null as VendorSpecialSku," _
                 & "    t.Quantity" _
                 & " from " _
                 & "    TakeOffProduct t " _
                 & " where " _
                 & "    t.TakeoffProductID not in (" _
                 & "        select TakeoffProductID from TakeoffProduct t inner join VendorProductPrice v on t.ProductID=v.ProductID where coalesce(v.IsDiscontinued,0) = 0 and v.VendorID=" & DB.Number(VendorId) _
                 & "        union select TakeoffProductID from TakeoffProduct t inner join VendorSpecialOrderProductPrice v on t.SpecialOrderProductId=v.SpecialOrderProductId where v.VendorID=" & DB.Number(VendorId) _
                 & "        union select TakeoffProductID from VendorTakeoffProductSubstitute where VendorID=" & DB.Number(VendorId) _
                 & "    )" _
                 & " and " _
                 & "    t.TakeOffId=" & DB.Number(TakeOffId) _
                 & " order by TakeoffProductId asc"


            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTakeoffVendorPricingOld(ByVal DB As Database, ByVal TakeOffId As Integer, ByVal VendorId As Integer) As DataTable
            Dim sql As String = String.Empty
            sql &= " select" _
                 & "    t.TakeOffProductId, " _
                 & "    vs.ProductID as SubProductId," _
                 & "    t.ProductId as ProductID, " _
                 & "    t.SpecialOrderProductID, " _
                 & "    coalesce(vs.VendorPrice,v.VendorPrice) as VendorPrice, " _
                 & "    coalesce(vs.VendorSku,v.VendorSku) as VendorSku," _
                 & "    v.IsSubstitution, " _
                 & "    v.SubstituteQuantityMultiplier," _
                 & "    vsp.VendorPrice as VendorSpecialPrice," _
                 & "    vsp.VendorSku as VendorSpecialSku," _
                 & "    t.Quantity" _
                 & " from " _
                 & "    TakeOffProduct t left outer join (select * from VendorProductPrice where VendorId=" & DB.Number(VendorId) & ") as v on t.ProductId=v.ProductID " _
                 & "        left outer join (select * from VendorProductSubstitute where VendorId=" & DB.Number(VendorId) & ") as s on t.ProductId=s.ProductID " _
                 & "        left outer join (select * from VendorProductPrice where VendorId=" & DB.Number(VendorId) & ") as vs on s.SubstituteProductID=vs.ProductID" _
                 & "        left outer join (select * from VendorSpecialOrderProductPrice where VendorId=" & DB.Number(VendorId) & ") as vsp on vsp.SpecialOrderProductId=t.SpecialOrderProductId" _
                 & " where " _
                 & "    t.TakeOffId=" & DB.Number(TakeOffId)

            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class TakeOffProductRowBase
        Private m_DB As Database
        Private m_TakeOffProductID As Integer = Nothing
        Private m_TakeOffID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property TakeOffProductID() As Integer
            Get
                Return m_TakeOffProductID
            End Get
            Set(ByVal Value As Integer)
                m_TakeOffProductID = value
            End Set
        End Property

        Public Property TakeOffID() As Integer
            Get
                Return m_TakeOffID
            End Get
            Set(ByVal Value As Integer)
                m_TakeOffID = value
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

        Public Property Quantity() As Integer
            Get
                Return m_Quantity
            End Get
            Set(ByVal Value As Integer)
                m_Quantity = value
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

        Public Sub New(ByVal DB As Database, ByVal TakeOffProductID As Integer)
            m_DB = DB
            m_TakeOffProductID = TakeOffProductID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TakeOffProduct WHERE TakeOffProductID = " & DB.Number(TakeOffProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TakeOffProductID = Convert.ToInt32(r.Item("TakeOffProductID"))
            m_TakeOffID = Convert.ToInt32(r.Item("TakeOffID"))
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
            m_Quantity = Convert.ToInt32(r.Item("Quantity"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from TakeOffProduct order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO TakeOffProduct (" _
             & " TakeOffID" _
             & ",ProductID" _
             & ",SpecialOrderProductID" _
             & ",Quantity" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(TakeOffID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(SpecialOrderProductID) _
             & "," & m_DB.Number(Quantity) _
             & "," & MaxSortOrder _
             & ")"

            TakeOffProductID = m_DB.InsertSQL(SQL)

            Return TakeOffProductID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TakeOffProduct SET " _
             & " TakeOffID = " & m_DB.NullNumber(TakeOffID) _
             & ",ProductID = " & m_DB.NullNumber(ProductID) _
             & ",SpecialOrderProductID = " & m_DB.NullNumber(SpecialOrderProductID) _
             & ",Quantity = " & m_DB.Number(Quantity) _
             & " WHERE TakeOffProductID = " & m_DB.quote(TakeOffProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM TakeOffProduct WHERE TakeOffProductID = " & m_DB.Number(TakeOffProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class TakeOffProductCollection
        Inherits GenericCollection(Of TakeOffProductRow)
    End Class

End Namespace


