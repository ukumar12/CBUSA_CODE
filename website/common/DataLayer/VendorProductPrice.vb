Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorProductPriceRow
        Inherits VendorProductPriceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductId As Integer)
            MyBase.New(DB, VendorID, ProductId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductId As Integer) As VendorProductPriceRow
            Dim row As VendorProductPriceRow

            row = New VendorProductPriceRow(DB, VendorID, ProductId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductId As Integer)
            Dim row As VendorProductPriceRow

            row = New VendorProductPriceRow(DB, VendorID, ProductId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorProductPrice"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetRowByVendorSku(ByVal DB As Database, ByVal VendorID As Integer, ByVal Sku As String, Optional ByVal SkipProductId As Integer = Nothing) As VendorProductPriceRow
            Dim out As New VendorProductPriceRow(DB)
            Dim sql As String = "select * from VendorProductPrice where VendorId=" & DB.Number(VendorID) & " and VendorSku=" & DB.Quote(Sku)
            If SkipProductId <> Nothing Then
                sql &= " and ProductId <> " & DB.Number(SkipProductId)
            End If
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetAllVendorPrices(ByVal DB As Database, ByVal VendorID As Integer, Optional ByVal UseSubstitutions As Boolean = False) As DataTable
            Dim sql As String = _
                  "select " _
                & " vpp.*, " _
                & " vs.SubstituteProductID, " _
                & " vs.VendorSKU as SubstituteSku, " _
                & " vs.VendorPrice as SubstitutePrice, " _
                & " vs.QuantityMultiplier as SubstituteQuantityMultiplier," _
                & " vs.Product as SubstituteProduct " _
                & "from " _
                & " VendorProductPrice vpp left outer join " _
                & "     (select vps.*, vppi.VendorPrice, vppi.VendorSKU, p.Product " _
                & "      from Product p inner join VendorProductSubstitute vps on p.ProductID=vps.SubstituteProductID " _
                & "         inner join VendorProductPrice vppi on vps.SubstituteProductID=vppi.ProductID " _
                & "      where vps.VendorID=" & DB.Number(VendorID) _
                & "      and vppi.VendorID=" & DB.Number(VendorID) & ") as vs " _
                & "     on vpp.ProductID=vs.ProductID where vpp.VendorID=" & DB.Number(VendorID) _
                & " order by ProductID asc"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAllProductPrices(ByVal DB As Database, ByVal ProductID As Integer, Optional ByVal LLCID As Integer = 0) As DataTable
            Dim sql As String = _
                  "select " _
                & " vpp.*, " _
                & " vs.SubstituteProductID, " _
                & " vs.VendorSKU as SubstituteSku, " _
                & " vs.VendorPrice as SubstitutePrice, " _
                & " vs.QuantityMultiplier as SubstituteQuantityMultiplier," _
                & " vs.Product as SubstituteProduct, " _
                & " v.CompanyName," _
                & " (select top 1 Submitted from VendorProductPriceHistory where VendorID=v.VendorID and ProductID=Coalesce(vs.ProductID,vpp.ProductID) order by Submitted desc) as LastUpdate " _
                & "from " _
                & " VendorProductPrice vpp left outer join " _
                & "     (select vps.*, vppi.VendorPrice, vppi.VendorSKU, p.Product " _
                & "      from Product p inner join VendorProductSubstitute vps on p.ProductID=vps.SubstituteProductID " _
                & "         inner join VendorProductPrice vppi on vps.SubstituteProductID=vppi.ProductID " _
                & "      where vps.ProductID=" & DB.Number(ProductID) _
                & "      and vps.VendorID=vppi.VendorID" _
                & "      ) as vs " _
                & "     on vpp.VendorID=vs.VendorID " _
                & " inner join Vendor v on vpp.VendorID = v.VendorID" _
                & " where v.IsActive =1 and vpp.ProductID=" & DB.Number(ProductID)

            If LLCID <> Nothing Then
                sql &= " and v.VendorID in (select VendorID from LLCVendor where LLCID=" & DB.Number(LLCID) & ")"
            End If

            sql &= " order by v.CompanyName"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Sub UpdateSubstitutions(ByVal DB As Database, ByVal VendorID As Integer)
            DB.BeginTransaction()
            Try
                Dim sql As String = "delete from VendorProductSubstitute where VendorID=" & DB.Number(VendorID) & " and SubstituteProductID not in (select ProductID from VendorProductPrice where VendorID=" & DB.Number(VendorID) & " and VendorPrice is not null)"
                DB.ExecuteSQL(sql)

                sql = "update VendorProductPrice set IsSubstitution=0 where VendorID=" & DB.Number(VendorID) & " and ProductID not in (select ProductID from VendorProductSubstitute where VendorID=" & DB.Number(VendorID) & ")"
                DB.ExecuteSQL(sql)

                DB.CommitTransaction()
            Catch ex As SqlException
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                're-throw for page-level error handling
                Throw ex
            End Try
        End Sub

        Public Shared Function GetCurrentPrice(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer) As Double
            Dim sql As String = _
                  " select " _
                & " case IsSubstitution when 0 then vp.VendorPrice else vs.VendorPrice end as VendorPrice" _
                & " from " _
                & "     VendorProductPrice vp left outer join (select vps.ProductID as OriginalProductID, vpsp.* from VendorProductSubstitute vps inner join VendorProductPrice vpsp on vps.SubstituteProductID=vpsp.ProductID where vpsp.VendorID=vps.VendorID and vps.VendorID=" & DB.Number(VendorID) & ") as vs on vs.OriginalProductID=vp.ProductID" _
                & " where " _
                & "     vp.ProductID=" & DB.Number(ProductID) _
                & " and " _
                & "     vp.VendorID=" & DB.Number(VendorID)
            Dim out As Object = DB.ExecuteScalar(sql)
            Return Core.GetDouble(out)
        End Function

        Public Shared Function GetHistoricalPrice(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer, ByVal PriceDate As DateTime) As DataRow
            Dim dt As DataTable = DB.GetDataTable("select * from VendorProductPrice where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID))
            If dt.Rows.Count > 0 Then
                If (Not IsDBNull(dt.Rows(0)("Updated")) AndAlso Core.GetDate(dt.Rows(0)("Updated")) <= PriceDate) _
                    OrElse (IsDBNull(dt.Rows(0)("Updated")) AndAlso Core.GetDate(dt.Rows(0)("Submitted")) <= PriceDate) Then
                    Return dt.Rows(0)
                End If
            End If

            dt = DB.GetDataTable("select top 1 * from VendorProductPriceHistory where VendorID=" & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID) & " and Submitted <=" & DB.Quote(PriceDate) & " order by Submitted Desc")
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0)
            End If
            Return Nothing
        End Function
    End Class

    Public MustInherit Class VendorProductPriceRowBase
        Private m_DB As Database
        Private m_VendorID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_VendorSKU As String = Nothing
        Private m_VendorPrice As Double = Nothing
        Private m_IsSubstitution As Boolean = Nothing
        Private m_IsDiscontinued As Boolean = Nothing
        Private m_SubstituteQuantityMultiplier As Integer = Nothing
        Private m_IsUpload As Boolean = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_SubmitterVendorAccountID As Integer = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_UpdaterVendorAccountID As Integer = Nothing
        Private m_NextPrice As Double = Nothing
        Private m_NextPriceApplies As Date = Nothing

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

        Public Property VendorSKU() As String
            Get
                Return m_VendorSKU
            End Get
            Set(ByVal Value As String)
                m_VendorSKU = value
            End Set
        End Property

        Public Property VendorPrice() As Double
            Get
                Return m_VendorPrice
            End Get
            Set(ByVal Value As Double)
                m_VendorPrice = value
            End Set
        End Property

        Public Property IsSubstitution() As Boolean
            Get
                Return m_IsSubstitution
            End Get
            Set(ByVal Value As Boolean)
                m_IsSubstitution = value
            End Set
        End Property

        Public Property IsDiscontinued() As Boolean
            Get
                Return m_IsDiscontinued
            End Get
            Set(ByVal Value As Boolean)
                m_IsDiscontinued = value
            End Set
        End Property

        Public Property SubstituteQuantityMultiplier() As Integer
            Get
                Return m_SubstituteQuantityMultiplier
            End Get
            Set(ByVal Value As Integer)
                m_SubstituteQuantityMultiplier = value
            End Set
        End Property

        Public Property IsUpload() As Boolean
            Get
                Return m_IsUpload
            End Get
            Set(ByVal Value As Boolean)
                m_IsUpload = value
            End Set
        End Property

        Public Property NextPrice() As Double
            Get
                Return m_NextPrice
            End Get
            Set(ByVal value As Double)
                m_NextPrice = value
            End Set
        End Property

        Public Property NextPriceApplies() As Date
            Get
                Return m_NextPriceApplies
            End Get
            Set(ByVal value As Date)
                m_NextPriceApplies = value
            End Set
        End Property

        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
        End Property

        Public Property SubmitterVendorAccountID() As Integer
            Get
                Return m_SubmitterVendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_SubmitterVendorAccountID = value
            End Set
        End Property

        Public ReadOnly Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
        End Property

        Public Property UpdaterVendorAccountID() As Integer
            Get
                Return m_UpdaterVendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_UpdaterVendorAccountID = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductId As Integer)
            m_DB = DB
            m_VendorID = VendorID
            m_ProductID = ProductId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorProductPrice WHERE VendorID = " & DB.Number(VendorID) & " and ProductId=" & DB.Number(ProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_ProductID = Core.GetInt(r.Item("ProductID"))
            If IsDBNull(r.Item("VendorSKU")) Then
                m_VendorSKU = Nothing
            Else
                m_VendorSKU = Core.GetString(r.Item("VendorSKU"))
            End If
            If IsDBNull(r.Item("VendorPrice")) Then
                m_VendorPrice = Nothing
            Else
                m_VendorPrice = Core.GetDouble(r.Item("VendorPrice"))
            End If
            m_IsSubstitution = Core.GetBoolean(r.Item("IsSubstitution"))
            m_IsDiscontinued = Core.GetBoolean(r.Item("IsDiscontinued"))
            If IsDBNull(r.Item("SubstituteQuantityMultiplier")) Then
                m_SubstituteQuantityMultiplier = Nothing
            Else
                m_SubstituteQuantityMultiplier = Core.GetInt(r.Item("SubstituteQuantityMultiplier"))
            End If
            m_IsUpload = Core.GetBoolean(r.Item("IsUpload"))
            m_Submitted = Core.GetDate(r.Item("Submitted"))
            m_SubmitterVendorAccountID = Core.GetInt(r.Item("SubmitterVendorAccountID"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Core.GetDate(r.Item("Updated"))
            End If
            If IsDBNull(r.Item("UpdaterVendorAccountID")) Then
                m_UpdaterVendorAccountID = Nothing
            Else
                m_UpdaterVendorAccountID = Convert.ToInt32(r.Item("UpdaterVendorAccountID"))
            End If
            m_NextPrice = Core.GetDouble(r.Item("NextPrice"))
            m_NextPriceApplies = Core.GetDate(r.Item("NextPriceApplies"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorProductPrice (" _
             & " ProductID" _
             & ",VendorID" _
             & ",VendorSKU" _
             & ",VendorPrice" _
             & ",IsSubstitution" _
             & ",IsDiscontinued" _
             & ",SubstituteQuantityMultiplier" _
             & ",IsUpload" _
             & ",Submitted" _
             & ",SubmitterVendorAccountID" _
             & ",Updated" _
             & ",UpdaterVendorAccountID" _
             & ",NextPrice" _
             & ",NextPriceApplies" _
             & ") VALUES (" _
             & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Quote(VendorSKU) _
             & "," & m_DB.NullNumber(VendorPrice) _
             & "," & CInt(IsSubstitution) _
             & "," & CInt(IsDiscontinued) _
             & "," & m_DB.NullNumber(SubstituteQuantityMultiplier) _
             & "," & CInt(IsUpload) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(SubmitterVendorAccountID) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(UpdaterVendorAccountID) _
             & "," & m_DB.NullNumber(NextPrice) _
             & "," & m_DB.NullQuote(NextPriceApplies) _
             & ")"

            m_DB.ExecuteSQL(SQL)

            Return VendorID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorProductPrice SET " _
             & " VendorSKU = " & m_DB.Quote(VendorSKU) _
             & ",VendorPrice = " & m_DB.NullNumber(VendorPrice) _
             & ",IsSubstitution = " & CInt(IsSubstitution) _
             & ",IsDiscontinued = " & CInt(IsDiscontinued) _
             & ",SubstituteQuantityMultiplier = " & m_DB.NullNumber(SubstituteQuantityMultiplier) _
             & ",IsUpload = " & CInt(IsUpload) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",UpdaterVendorAccountID = " & m_DB.NullNumber(UpdaterVendorAccountID) _
             & ",NextPrice = " & m_DB.NullNumber(NextPrice) _
             & ",NextPriceApplies = " & m_DB.NullQuote(NextPriceApplies) _
             & " WHERE VendorID = " & m_DB.Quote(VendorID) _
             & " AND ProductID = " & m_DB.Quote(ProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorProductPrice WHERE VendorID = " & m_DB.Number(VendorID) & " and ProductID = " & m_DB.Number(ProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorProductPriceCollection
        Inherits GenericCollection(Of VendorProductPriceRow)
    End Class

End Namespace


