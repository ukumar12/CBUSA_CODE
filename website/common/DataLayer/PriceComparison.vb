Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PriceComparisonRow
        Inherits PriceComparisonRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PriceComparisonID As Integer)
            MyBase.New(DB, PriceComparisonID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PriceComparisonID As Integer) As PriceComparisonRow
            Dim row As PriceComparisonRow

            row = New PriceComparisonRow(DB, PriceComparisonID)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRow(ByVal DB As Database, ByVal TakeoffID As Integer, ByVal Vendors As Generic.List(Of String)) As PriceComparisonRow
            Dim row As New PriceComparisonRow(DB)
            Dim sVendors As New StringBuilder
            For Each s As String In Vendors
                sVendors.Append(IIf(sVendors.Length = 0, s, "," & s))
            Next
            Dim sql As String = "select pc.* from PriceComparison pc where TakeoffID=" & DB.Number(TakeoffID) & " and (select count(*) from PriceComparisonVendorProductPrice p where p.PriceComparisonId=pc.PriceComparisonId) = (select count(*) from PriceComparisonVendorProductPrice where VendorID in " & DB.NumberMultiple(sVendors.ToString) & ") order by IsSaved, IsDashboard"
            'Dim sql As String = "select pc.* from PriceComparison pc where AdminID=null and TakeoffID=" & DB.Number(TakeoffID) & " and (select count(*) from PriceComparisonVendorProductPrice p where p.PriceComparisonId=pc.PriceComparisonId) = (select count(*) from PriceComparisonVendorProductPrice where VendorID in " & DB.NumberMultiple(Vendors.Aggregate(Function(sum, app) IIf(sum = "", app, sum & "," & app))) & ") order by IsSaved, IsDashboard"
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                row.Load(sdr)
            End If
            sdr.Close()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PriceComparisonID As Integer)
            Dim row As PriceComparisonRow

            row = New PriceComparisonRow(DB, PriceComparisonID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PriceComparison"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Sub UpdateVendor(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, ByVal dtTakeoffProducts As DataTable, ByVal dtPrices As DataTable)
            Dim dbSummary As PriceComparisonVendorSummaryRow = PriceComparisonVendorSummaryRow.GetRow(DB, PriceComparisonId, VendorId)
            If dbSummary.Subtotal > 0 Then
                dbSummary.Subtotal = 0
            End If

            If dbSummary.CreateDate = Nothing Then
                Try
                    dbSummary.Insert()
                Catch ex As SqlException
                    dbSummary.Update()
                End Try
            Else
                dbSummary.Update()
            End If

            Dim e As IEnumerable = _
                From prod As DataRow In dtTakeoffProducts.AsEnumerable _
                Join price As DataRow In dtPrices.AsEnumerable _
                On prod("TakeoffProductId") Equals price("TakeoffProductId") _
                Select New With {.product = prod, .price = price}

            For Each item As Object In e
                Dim dbRow As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(DB, PriceComparisonId, VendorId, item.product("TakeoffProductId"))
                dbRow.PriceComparisonID = PriceComparisonId
                dbRow.VendorID = VendorId
                dbRow.Quantity = item.product("Quantity")
                dbRow.State = Controls.ProductState.Init
                If IsDBNull(item.price("SubstituteQuantityMultiplier")) Then
                    dbRow.RecommendedQuantity = dbRow.Quantity
                Else
                    dbRow.RecommendedQuantity = dbRow.Quantity * item.price("SubstituteQuantityMultiplier")
                End If
                If Not IsDBNull(item.price("ProductID")) Then
                    dbRow.ProductID = item.price("ProductID")
                End If
                If Not IsDBNull(item.product("SpecialOrderProductId")) Then
                    dbRow.SpecialOrderProductID = item.product("SpecialOrderProductId")
                End If
                If Not IsDBNull(item.price("SubProductId")) Then
                    dbRow.SubstituteProductID = item.price("SubProductId")
                End If
                If Not IsDBNull(item.price("VendorPrice")) Then
                    dbRow.UnitPrice = item.price("VendorPrice")
                    'ElseIf Not IsDBNull(item.product("AvgPrice")) Then
                    '    dbRow.UnitPrice = item.product("AvgPrice")
                Else
                    dbRow.UnitPrice = -1
                End If
                dbRow.Total = IIf(dbRow.UnitPrice > 0, dbRow.UnitPrice * dbRow.Quantity, 0)

                dbSummary.Subtotal += dbRow.Total

                If dbRow.IsNew Then
                    dbRow.Insert()
                Else
                    dbRow.Update()
                End If
            Next

            'dbSummary.Tax = CalculateTax()

            dbSummary.Total = dbSummary.Subtotal + dbSummary.Tax
            dbSummary.Update()
        End Sub

        Public Shared Sub RemoveVendor(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer)
            Dim sql As String = "delete from PriceComparisonVendorSummary where PriceComparisonId=" & DB.Quote(PriceComparisonId) & " and VendorId=" & DB.Quote(VendorId)
            DB.ExecuteSQL(sql)
        End Sub

        Public Shared Function GetSavedVendors(ByVal DB As Database, ByVal PriceComparisonId As Integer) As String
            Dim sql As String = "select distinct pcvpp.VendorId from PriceComparisonVendorProductPrice pcvpp Inner Join Vendor v On pcvpp.VendorId = v.VendorId where v.IsActive = 1 And pcvpp.PriceComparisonId=" & DB.Number(PriceComparisonId)
            Dim out As New StringBuilder()
            Dim conn As String = String.Empty
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                out.Append(conn & sdr.Item("VendorId"))
                conn = ","
            End While
            sdr.Close()
            Return out.ToString
        End Function

        Public Shared Function GetVendorProducts(ByVal DB As Database, ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, Optional ByVal IncludeProduct As Boolean = False) As DataTable
            Dim sql As String
            If IncludeProduct Then
                sql = _
                      " select v.*," _
                      & "   p.ProductID as DBProductID," _
                      & "   p.Product as Product,p.SKU as CBUSASKU," _
                      & "   p.Description as Description," _
                      & "   vp.VendorSku" _
                      & " from PriceComparisonVendorProductPrice v " _
                      & "   inner join TakeoffProduct tp on tp.TakeoffProductID=v.TakeoffProductID" _
                      & "   inner join Product p on v.ProductID=p.ProductID" _
                      & "   left outer join (select * from VendorProductPrice where VendorID=" & DB.Number(VendorId) & ") as vp on vp.ProductID=p.ProductID" _
                      & " where" _
                      & "   v.VendorID=" & DB.Number(VendorId) _
                      & " and " _
                      & "   v.PriceComparisonID=" & DB.Number(PriceComparisonId) _
                      & " and " _
                      & "   v.SpecialOrderProductID is null" _
                      & " and " _
                      & "   v.SubstituteProductId is null" _
                      & " union " _
                      & " select v.*," _
                      & "   sp.ProductID as DBProductId," _
                      & "   sp.Product as Product,sp.SKU  AS CBUSASKU," _
                      & "   sp.Description as Description," _
                      & "   vp.VendorSku" _
                      & " from PriceComparisonVendorProductPrice v" _
                      & "   inner join TakeoffProduct tp on tp.TakeoffProductID=v.TakeoffProductID" _
                      & "   inner join Product sp on v.SubstituteProductID=sp.ProductID" _
                      & "   left outer join (select * from VendorProductPrice where VendorID=" & DB.Number(VendorId) & ") as vp on vp.ProductID=sp.ProductID" _
                      & " where " _
                      & "   v.VendorId=" & DB.Number(VendorId) _
                      & " and " _
                      & "   v.PriceComparisonId=" & DB.Number(PriceComparisonId) _
                      & " and " _
                      & "   v.SpecialOrderProductID is null" _
                      & " union " _
                      & " select v.*," _
                      & "   sp.SpecialOrderProductID as DBProductID," _
                      & "   sp.SpecialOrderProduct as Product, NULL  AS CBUSASKU," _
                      & "   sp.Description as Description," _
                      & "   vp.VendorSku" _
                      & " from PriceComparisonVendorProductPrice v " _
                      & "   inner join TakeoffProduct tp on tp.TakeoffProductID=v.TakeoffProductID" _
                      & "   inner join SpecialOrderProduct sp on v.SpecialOrderProductID=sp.SpecialOrderProductID" _
                      & "   left outer join (select * from VendorSpecialOrderProductPrice where VendorID=" & DB.Number(VendorId) & ") as vp on vp.SpecialOrderProductID=sp.SpecialOrderProductID" _
                      & " where" _
                      & "   v.VendorID=" & DB.Number(VendorId) _
                      & " and " _
                      & "   v.PriceComparisonID=" & DB.Number(PriceComparisonId)
            Else
                sql = " select p.* " _
                    & " from PriceComparisonVendorProductPrice p inner join TakeoffProduct tp on p.TakeoffProductID=tp.TakeoffProductID" _
                    & " where PriceComparisonId=" & DB.Number(PriceComparisonId) _
                    & " and VendorId=" & DB.Number(VendorId)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Sub UpdateAll()
            DB.BeginTransaction()
            Try
                Dim dtVendors As DataTable = DB.GetDataTable("select VendorID, ModifyDate from PriceComparisonVendorSummary where PriceComparisonID=" & DB.Number(PriceComparisonID))
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, BuilderID)
                Dim dtProducts As DataTable = TakeOffRow.GetTakeoffProductAverages(DB, TakeoffID, dbBuilder.LLCID)

                For Each row As DataRow In dtVendors.Rows
                    'only update price info once per day
                    If Core.GetDate(row("ModifyDate")).Date <> Now.Date Then
                        Dim dtPrices As DataTable = TakeOffProductRow.GetTakeoffVendorPricing(DB, TakeoffID, row("VendorID"))

                        UpdateVendor(DB, PriceComparisonID, row("VendorID"), dtProducts, dtPrices)
                    End If
                Next

                DB.CommitTransaction()
            Catch ex As SqlException
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                Throw ex
            End Try
        End Sub

        Public Function GetTopVendors(ByVal cnt As Integer) As DataTable
            Dim sql As String = _
                  "select top " & cnt & " s.*, v.CompanyName " _
                & "from Vendor v inner join PriceComparisonVendorSummary s on v.VendorID=s.VendorID " _
                & "where v.IsActive = 1 and s.PriceComparisonID=" & DB.Number(PriceComparisonID) _
                & " and s.SubTotal > 0" _
                & " order by SubTotal Asc"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetDashboardComparisonIds(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = "select p.PriceComparisonId from PriceComparison p where IsAdminComparison = 0 and IsDashboard = 1 and BuilderID=" & DB.Number(BuilderID)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetDashboardComparisons(ByVal DB As Database, ByVal BuilderID As Integer) As PriceComparisonCollection
            Dim out As New PriceComparisonCollection
            Dim sql As String = "select * from PriceComparison where IsAdminComparison = 0 and IsDashboard = 1 and BuilderID=" & DB.Number(BuilderID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                Dim c As PriceComparisonRow = New PriceComparisonRow(DB)
                c.Load(sdr)
                out.Add(c)
            End While
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetSavedComparisons(ByVal DB As Database, ByVal BuilderID As Integer) As DataTable
            Dim sql As String = "select * from PriceComparison where IsSaved=1 and BuilderID=" & DB.Number(BuilderID)
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetLatestSavedComparison(ByVal DB As Database, ByVal BuilderID As Integer, ByVal TakeOffId As Integer) As DataTable
            Dim sql As String = "select Top 1 * from PriceComparison where IsSaved=1 and BuilderID=" & DB.Number(BuilderID) & " and TakeOffId=" & DB.Number(TakeOffId) & " Order By Created Desc"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetLastComparison(ByVal DB As Database, ByVal TakeoffID As Integer) As DataTable
            Dim sql As String = "select top 1 * from PriceComparison where TakeoffID=" & DB.Number(TakeoffID) & " order by Created desc"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAdminComparionIds(ByVal DB As Database, ByVal LLCId As Integer) As DataTable
            Dim sql As String = "select PriceComparisonId from PriceComparison p where IsAdminComparison = 1 and BuilderID in (select BuilderID from Builder where LLCId=" & DB.Number(LLCId) & ") and exists (select * from Takeoff where TakeoffId=p.TakeoffId)"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAdminComparisons(ByVal DB As Database, ByVal LLCId As Integer) As PriceComparisonCollection
            Dim out As New PriceComparisonCollection
            Dim sql As String = "select * from PriceComparison p where IsAdminComparison = 1 and BuilderID in (select BuilderID from Builder where LLCId=" & DB.Number(LLCId) & ") and exists (select * from Takeoff where TakeoffId=p.TakeoffId)"
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                Dim temp As New PriceComparisonRow(DB)
                temp.Load(sdr)
                out.Add(temp)
            End While
            sdr.Close()

            Return out
        End Function

        Public Shared Sub AddToDashboard(ByVal DB As Database, ByVal PriceComparisonID As Integer, Optional ByVal BuilderID As Integer = Nothing)
            If BuilderID = Nothing Then
                Dim dbComparison As PriceComparisonRow = GetRow(DB, PriceComparisonID)
                BuilderID = dbComparison.BuilderID
            End If
            DB.ExecuteSQL("insert into BuilderDashboardComparison(BuilderID, PriceComparisonID) values (" & DB.Number(BuilderID) & "," & DB.Number(PriceComparisonID) & ")")
        End Sub

        Public Shared Function GetRecentJobs(ByVal DB As Database, ByVal BuilderID As Integer, ByVal Count As Integer) As DataTable
            Dim sql As String = "select top " & Count & " p.ProjectName, t.* from Takeoff t " _
                 & "    left outer join Project p on t.ProjectID=p.ProjectID" _
                 & " where t.BuilderID=" & DB.Number(BuilderID) _
                 & " and (t.Title is not null or exists (select * from PriceComparison where TakeoffID=t.TakeoffID))" _
                 & " and not exists (select * from PriceComparison where TakeoffId=t.TakeoffId and IsAdminComparison=1)" _
                 & " order by t.Saved Desc"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetRowByTakeoff(ByVal DB As Database, ByVal TakeoffID As Integer) As PriceComparisonRow
            Dim out As New PriceComparisonRow(DB)
            Dim sql As String = "select * from PriceComparison where TakeoffID=" & DB.Number(TakeoffID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Sub InitVendor(ByVal DB As Database, ByVal PriceComparisonID As Integer, ByVal VendorID As Integer)
            Dim TakeoffID As Integer = DB.ExecuteScalar("select TakeoffID from PriceComparison where PriceComparisonID=" & DB.Number(PriceComparisonID))

            Dim dbSummary As PriceComparisonVendorSummaryRow = PriceComparisonVendorSummaryRow.GetRow(DB, PriceComparisonID, VendorID)
            If dbSummary.CreateDate = Nothing Then
                dbSummary.PriceComparisonID = PriceComparisonID
                dbSummary.Subtotal = 0
                dbSummary.Tax = 0
                dbSummary.Total = 0
                dbSummary.VendorID = VendorID
                dbSummary.Insert()
            End If

            Dim dtProducts As DataTable = TakeOffRow.GetTakeoffProducts(DB, TakeoffID)
            For Each row As DataRow In dtProducts.Rows
                Dim dbPrice As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(DB, PriceComparisonID, VendorID, row("TakeoffProductID"))
                dbPrice.PriceComparisonID = PriceComparisonID
                If dbPrice.State <> Controls.ProductState.Accepted Or dbPrice.Quantity = Nothing Then
                    dbPrice.Quantity = row("Quantity")
                End If
                dbPrice.TakeoffProductId = row("TakeoffProductID")
                dbPrice.Total = 0
                dbPrice.UnitPrice = 0
                dbPrice.VendorID = VendorID
                If dbPrice.IsNew Then
                    dbPrice.Insert()
                Else
                    dbPrice.Update()
                End If
            Next
        End Sub
    End Class

    Public MustInherit Class PriceComparisonRowBase
        Private m_DB As Database
        Private m_PriceComparisonID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_TakeoffID As Integer = Nothing
        Private m_IsSaved As Boolean = Nothing
        Private m_IsDashboard As Boolean = Nothing
        Private m_IsAdminComparison As Boolean = Nothing
        Private m_ModifyDate As DateTime = Nothing


        Public Property PriceComparisonID() As Integer
            Get
                Return m_PriceComparisonID
            End Get
            Set(ByVal Value As Integer)
                m_PriceComparisonID = value
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

        Public Property AdminID() As Integer
            Get
                Return m_AdminID
            End Get
            Set(ByVal Value As Integer)
                m_AdminID = value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property TakeoffID() As Integer
            Get
                Return m_TakeoffID
            End Get
            Set(ByVal value As Integer)
                m_TakeoffID = value
            End Set
        End Property

        Public Property IsSaved() As Boolean
            Get
                Return m_IsSaved
            End Get
            Set(ByVal value As Boolean)
                m_IsSaved = value
            End Set
        End Property

        Public Property IsDashboard() As Boolean
            Get
                Return m_IsDashboard
            End Get
            Set(ByVal value As Boolean)
                m_IsDashboard = value
            End Set
        End Property

        Public Property IsAdminComparison() As Boolean
            Get
                Return m_IsAdminComparison
            End Get
            Set(ByVal value As Boolean)
                m_IsAdminComparison = value
            End Set
        End Property

        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
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

        Public Sub New(ByVal DB As Database, ByVal PriceComparisonID As Integer)
            m_DB = DB
            m_PriceComparisonID = PriceComparisonID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PriceComparison WHERE PriceComparisonID = " & DB.Number(PriceComparisonID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PriceComparisonID = Convert.ToInt32(r.Item("PriceComparisonID"))
            If IsDBNull(r.Item("BuilderID")) Then
                m_BuilderID = Nothing
            Else
                m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            End If
            If IsDBNull(r.Item("AdminID")) Then
                m_AdminID = Nothing
            Else
                m_AdminID = Convert.ToInt32(r.Item("AdminID"))
            End If
            m_Created = Convert.ToDateTime(r.Item("Created"))
            m_TakeoffID = Core.GetInt(r.Item("TakeoffID"))
            m_IsSaved = Core.GetBoolean(r.Item("IsSaved"))
            m_IsDashboard = Core.GetBoolean(r.Item("IsDashboard"))
            m_IsAdminComparison = Core.GetBoolean(r.Item("IsAdminComparison"))
            m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))

        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PriceComparison (" _
             & " BuilderID" _
             & ",AdminID" _
             & ",Created" _
             & ",TakeoffID" _
             & ",IsSaved" _
             & ",IsDashboard" _
             & ",IsAdminComparison" _
             & ",ModifyDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(AdminID) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Number(TakeoffID) _
             & "," & CInt(IsSaved) _
             & "," & CInt(IsDashboard) _
             & "," & CInt(IsAdminComparison) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            PriceComparisonID = m_DB.InsertSQL(SQL)

            Return PriceComparisonID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PriceComparison SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",AdminID = " & m_DB.NullNumber(AdminID) _
             & ",TakeoffID = " & m_DB.Number(TakeoffID) _
             & ",IsSaved = " & CInt(IsSaved) _
             & ",IsDashboard = " & CInt(IsDashboard) _
             & ",IsAdminComparison = " & CInt(IsAdminComparison) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & " WHERE PriceComparisonID = " & m_DB.Quote(PriceComparisonID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PriceComparison WHERE PriceComparisonID = " & m_DB.Number(PriceComparisonID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PriceComparisonCollection
        Inherits GenericCollection(Of PriceComparisonRow)
    End Class

End Namespace


