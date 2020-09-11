Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceTakeOffProductRow
        Inherits TwoPriceTakeOffProductRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TwoPriceTakeOffProductID As Integer)
            MyBase.New(DB, TwoPriceTakeOffProductID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceTakeOffProductID As Integer) As TwoPriceTakeOffProductRow
            Dim row As TwoPriceTakeOffProductRow

            row = New TwoPriceTakeOffProductRow(DB, TwoPriceTakeOffProductID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TwoPriceTakeOffProductID As Integer)
            Dim row As TwoPriceTakeOffProductRow

            row = New TwoPriceTakeOffProductRow(DB, TwoPriceTakeOffProductID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceTakeOffProduct"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function
        Public Shared Function GetCollection(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC", Optional ByVal TwoPriceTakeOffId As Integer = 0) As TwoPriceTakeOffProductCollection
            Dim SQL As String = "select * from TwoPriceTakeOffProduct"
            If TwoPriceTakeOffId <> Nothing Then
                SQL &= " WHERE TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId)
            End If
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Dim col As New TwoPriceTakeOffProductCollection
            Using sqlr As SqlDataReader = DB.GetReader(SQL)
                While sqlr.Read
                    Dim dbproduct As New TwoPriceTakeOffProductRow(DB)
                    dbproduct.Load(sqlr)
                    col.Add(dbproduct)
                End While
            End Using
            Return col
        End Function

        'Custom Methods
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, ByVal ProductId As Integer) As TwoPriceTakeOffProductRow
            Dim out As New TwoPriceTakeOffProductRow(DB)
            Dim sql As String = "select * from TwoPriceTakeOffProduct where TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) & " and ProductId=" & DB.Number(ProductId)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read() Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetTwoPriceTakeOffVendorPricing(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, ByVal VendorId As Integer) As DataTable
            Dim sql As String = String.Empty
            sql &= " select" _
                 & "    t.TwoPriceTakeOffProductId, " _
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
                 & "    TwoPriceTakeOffProduct t inner join VendorProductPrice as v on t.ProductID = v.ProductID" _
                 & "        left outer join (select * from VendorProductSubstitute where VendorId=" & DB.Number(VendorId) & ") as s on t.ProductId=s.ProductID " _
                 & "        left outer join (select * from VendorProductPrice where VendorId=" & DB.Number(VendorId) & ") as vs on s.SubstituteProductID=vs.ProductID" _
                 & " where " _
                 & "    t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) _
                 & " and " _
                 & "    v.VendorID=" & DB.Number(VendorId) _
                 & " and " _
                 & "    coalesce(v.IsDiscontinued,0) = 0" _
                 & " union " _
                 & " select" _
                 & "    t.TwoPriceTakeOffProductId, " _
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
                 & "    TwoPriceTakeOffProduct t inner join (select * from VendorSpecialOrderProductPrice where VendorId=" & DB.Number(VendorId) & ") as vsp on vsp.SpecialOrderProductId=t.SpecialOrderProductId" _
                 & " where " _
                 & "    t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) _
                 & " and " _
                 & "    vsp.IsSubstitution=0" _
                 & " union " _
                 & " select " _
                 & "    t.TwoPriceTakeOffProductId, " _
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
                 & "    TwoPriceTakeOffProduct t left outer join VendorProductPrice as v on t.ProductID = v.ProductID" _
                 & "        inner join (select * from VendorTwoPriceTakeOffProductSubstitute where VendorId=" & DB.Number(VendorId) & ") as s on t.TwoPriceTakeOffProductId=s.TwoPriceTakeOffProductID " _
                 & "        left outer join (select * from VendorProductPrice where coalesce(IsDiscontinued,0)=0 and VendorId=" & DB.Number(VendorId) & ") as vs on s.SubstituteProductID=vs.ProductID" _
                 & " where " _
                 & "    t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) _
                 & " union " _
                 & " select" _
                 & "    t.TwoPriceTakeOffProductId, " _
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
                 & "    TwoPriceTakeOffProduct t " _
                 & " where " _
                 & "    t.TwoPriceTakeOffProductID not in (" _
                 & "        select TwoPriceTakeOffProductID from TwoPriceTakeOffProduct t inner join VendorProductPrice v on t.ProductID=v.ProductID where coalesce(v.IsDiscontinued,0) = 0 and v.VendorID=" & DB.Number(VendorId) _
                 & "        union select TwoPriceTakeOffProductID from TwoPriceTakeOffProduct t inner join VendorSpecialOrderProductPrice v on t.SpecialOrderProductId=v.SpecialOrderProductId where v.VendorID=" & DB.Number(VendorId) _
                 & "        union select TwoPriceTakeOffProductID from VendorTwoPriceTakeOffProductSubstitute where VendorID=" & DB.Number(VendorId) _
                 & "    )" _
                 & " and " _
                 & "    t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) _
                 & " order by TwoPriceTakeOffProductId asc"


            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceTakeOffVendorPricingOld(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, ByVal VendorId As Integer) As DataTable
            Dim sql As String = String.Empty
            sql &= " select" _
                 & "    t.TwoPriceTakeOffProductId, " _
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
                 & "    TwoPriceTakeOffProduct t left outer join (select * from VendorProductPrice where VendorId=" & DB.Number(VendorId) & ") as v on t.ProductId=v.ProductID " _
                 & "        left outer join (select * from VendorProductSubstitute where VendorId=" & DB.Number(VendorId) & ") as s on t.ProductId=s.ProductID " _
                 & "        left outer join (select * from VendorProductPrice where VendorId=" & DB.Number(VendorId) & ") as vs on s.SubstituteProductID=vs.ProductID" _
                 & "        left outer join (select * from VendorSpecialOrderProductPrice where VendorId=" & DB.Number(VendorId) & ") as vsp on vsp.SpecialOrderProductId=t.SpecialOrderProductId" _
                 & " where " _
                 & "    t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId)

            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class TwoPriceTakeOffProductRowBase
        Private m_DB As Database
        Private m_TwoPriceTakeOffProductID As Integer = Nothing
        Private m_TwoPriceTakeOffID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property TwoPriceTakeOffProductID() As Integer
            Get
                Return m_TwoPriceTakeOffProductID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceTakeOffProductID = value
            End Set
        End Property

        Public Property TwoPriceTakeOffID() As Integer
            Get
                Return m_TwoPriceTakeOffID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceTakeOffID = value
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

        Public Sub New(ByVal DB As Database, ByVal TwoPriceTakeOffProductID As Integer)
            m_DB = DB
            m_TwoPriceTakeOffProductID = TwoPriceTakeOffProductID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceTakeOffProduct WHERE TwoPriceTakeOffProductID = " & DB.Number(TwoPriceTakeOffProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceTakeOffProductID = Convert.ToInt32(r.Item("TwoPriceTakeOffProductID"))
            m_TwoPriceTakeOffID = Convert.ToInt32(r.Item("TwoPriceTakeOffID"))
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

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from TwoPriceTakeOffProduct order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO TwoPriceTakeOffProduct (" _
             & " TwoPriceTakeOffID" _
             & ",ProductID" _
             & ",SpecialOrderProductID" _
             & ",Quantity" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(TwoPriceTakeOffID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(SpecialOrderProductID) _
             & "," & m_DB.Number(Quantity) _
             & "," & MaxSortOrder _
             & ")"

            TwoPriceTakeOffProductID = m_DB.InsertSQL(SQL)

            Return TwoPriceTakeOffProductID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceTakeOffProduct SET " _
             & " TwoPriceTakeOffID = " & m_DB.NullNumber(TwoPriceTakeOffID) _
             & ",ProductID = " & m_DB.NullNumber(ProductID) _
             & ",SpecialOrderProductID = " & m_DB.NullNumber(SpecialOrderProductID) _
             & ",Quantity = " & m_DB.Number(Quantity) _
             & " WHERE TwoPriceTakeOffProductID = " & m_DB.quote(TwoPriceTakeOffProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceTakeOffProduct WHERE TwoPriceTakeOffProductID = " & m_DB.Number(TwoPriceTakeOffProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class TwoPriceTakeOffProductCollection
        Inherits List(Of TwoPriceTakeOffProductRow)
    End Class

End Namespace


