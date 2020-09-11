Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TakeOffRow
        Inherits TakeOffRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TakeOffID As Integer)
            MyBase.New(DB, TakeOffID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TakeOffID As Integer) As TakeOffRow
            Dim row As TakeOffRow

            row = New TakeOffRow(DB, TakeOffID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TakeOffID As Integer)
            Dim row As TakeOffRow

            row = New TakeOffRow(DB, TakeOffID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TakeOff"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Function AddProduct(ByVal ProductId As Integer, ByVal Quantity As Integer) As Integer
            Dim dbProduct As New TakeOffProductRow(DB)
            dbProduct.TakeOffID = TakeOffID
            dbProduct.ProductID = ProductId
            dbProduct.Quantity = Quantity
            Return dbProduct.Insert()
        End Function

        Public Function UpdateProduct(ByVal ProductId As Integer, ByVal Quantity As Integer, ByVal ReplaceQuantity As Boolean) As Integer
            Dim dbProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, TakeOffID, ProductId)
            If dbProduct.TakeOffProductID = Nothing Then
                dbProduct.TakeOffID = TakeOffID
                dbProduct.ProductID = ProductId
                dbProduct.Quantity = Quantity
                Return dbProduct.Insert
            Else
                If ReplaceQuantity Then
                    dbProduct.Quantity = Quantity
                Else
                    dbProduct.Quantity += Quantity
                End If
                dbProduct.Update()
            End If
            Return dbProduct.TakeOffProductID
        End Function

        Public Function RemoveProduct(ByVal TakeOffProductId As Integer) As Boolean
            TakeOffProductRow.RemoveRow(DB, TakeOffProductId)
        End Function

        Public Function Copy(Optional ByVal NewTitle As String = "") As TakeOffRow
            DB.BeginTransaction()
            Try
                Dim dbNew As New TakeOffRow(DB)
                dbNew.AdminID = AdminID
                dbNew.Archived = Archived
                dbNew.BuilderID = BuilderID
                dbNew.ProjectID = ProjectID
                dbNew.Title = IIf(NewTitle = String.Empty, Title, NewTitle)
                dbNew.VendorID = VendorID
                dbNew.Insert()
               
                Dim sql As String = _
                    "insert into TakeoffProduct(" _
                    & " TakeoffId" _
                    & ",ProductId" _
                    & ",SpecialOrderProductId" _
                    & ",Quantity" _
                    & ",SortOrder" _
                    & ") select " _
                    & DB.Number(dbNew.TakeOffID) _
                    & ",ProductId" _
                    & ",SpecialOrderProductId" _
                    & ",Quantity" _
                    & ",(SELECT MAX(SortOrder) FROM TakeoffProduct) + ROW_NUMBER() OVER (ORDER BY sortorder) " _
                    & " from TakeoffProduct" _
                    & " where TakeoffID=" & DB.Number(TakeOffID) _
                    & " order by SortOrder"

                DB.ExecuteSQL(sql)

                DB.CommitTransaction()
                Return dbNew
            Catch ex As SqlException
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                Logger.Error(Logger.GetErrorMessage(ex))
                Return Nothing
            End Try
        End Function

        Public Shared Sub CopyToTakeoff(ByVal DB As Database, ByVal SrcTakeoffId As Integer, ByVal DestTakeoffId As Integer)
            Dim sql As String = _
                  "insert into TakeoffProduct (TakeoffId,ProductId,SpecialOrderProductId,Quantity,SortOrder) " _
                & " select " _
                & DB.Number(DestTakeoffId) _
                & ",t.ProductId" _
                & ",t.SpecialOrderProductId" _
                & ",t.Quantity" _
                & ",(SELECT MAX(SortOrder) FROM TakeoffProduct) + ROW_NUMBER() OVER (ORDER BY sortorder) " _
                & " from " _
                & "     TakeoffProduct t" _
                & " where " _
                & "     t.TakeoffId=" & DB.Number(SrcTakeoffId)
            '  & "     (select coalesce(max(sortorder),0) + 1 as SortOrder from TakeoffProduct where TakeoffId=" & DB.Number(DestTakeoffId) & ") as temp" _
               

            DB.ExecuteSQL(sql)
        End Sub

        Public Function GetProductLineItems(ByVal DB As Database, ByVal TakeoffId As Integer, ByVal ProductId As Integer) As DataTable
            Dim sql As String = "select * from TakeOffProduct t inner join Product p on t.ProductId=p.ProductId where t.TakeoffId=" & DB.Number(TakeoffId) & " and p.ProductId=" & DB.Number(ProductId)
            Return DB.GetDataTable(sql)
        End Function

        Public Function GetProductsList() As DataTable
            Return GetTakeoffProducts(DB, TakeOffID)
        End Function

        Public Shared Function GetTakeoffProducts(ByVal DB As Database, ByVal TakeoffId As Integer, Optional ByVal VendorId As Integer = Nothing) As DataTable
            Dim sql As String
            If VendorId = Nothing Then
                sql = "select * from TakeOffProduct t left outer join Product p on t.ProductId=p.ProductId left outer join SpecialOrderProduct sp on t.SpecialOrderProductId=sp.SpecialOrderProductId where TakeOffId=" & DB.Number(TakeoffId) & " order by SortOrder"
            Else
                sql = "select * from TakeOffProduct t left outer join Product p on t.ProductId=p.ProductId left outer join SpecialOrderProduct sp on t.SpecialOrderProductId=sp.SpecialOrderProductId inner join VendorProductPrice v on p.ProductId=v.ProductId where TakeOffId=" & DB.Number(TakeoffId) & " and v.VendorId=" & DB.Number(VendorId) & " order by SortOrder"
            End If
            Return DB.GetDataTable(sql)
        End Function

        '************ Added by Apala (Medullus) on 02.02.2018 for VSO#10701 **************************
        Public Shared Function GetTakeoffProductsWithSpecialOrderProducts(ByVal DB As Database, ByVal TakeoffId As Integer, Optional ByVal VendorId As Integer = Nothing) As DataTable
            Dim sql As String
            If VendorId = Nothing Then

                'sql = "select t.ProductID, t.Quantity, 'Regular' as ProductType from TakeOffProduct t left outer join Product p on t.ProductId=p.ProductId where TakeOffId = " & DB.Number(TakeoffId) & " and t.ProductID is not null " _
                '    & " UNION " _
                '    & "select t.SpecialOrderProductID as ProductID, t.Quantity, 'Special' as ProductType from TakeOffProduct t left outer join SpecialOrderProduct sp on t.SpecialOrderProductId=sp.SpecialOrderProductId where TakeOffId = " & DB.Number(TakeoffId) & " and t.SpecialOrderProductID is not null"
                'Added by Subhra (Medullus) on 15.03.2018 for ticket no 1119
                sql = "select ProductID,Quantity,ProductType,SortOrder from (select t.ProductID, t.Quantity, 'Regular' as ProductType,SortOrder from TakeOffProduct t left outer join Product p on t.ProductId=p.ProductId where TakeOffId = " & DB.Number(TakeoffId) & " and t.ProductID is not null and p.IsActive = 1 " _
                    & " UNION " _
                    & "select t.SpecialOrderProductID as ProductID, t.Quantity, 'Special' as ProductType,SortOrder from TakeOffProduct t left outer join SpecialOrderProduct sp on t.SpecialOrderProductId=sp.SpecialOrderProductId where TakeOffId = " & DB.Number(TakeoffId) & " and t.SpecialOrderProductID is not null) " _
                    & " TakeOffProduct order by SortOrder"
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTakeoffProductAverages(ByVal DB As Database, ByVal TakeoffId As Integer, ByVal LLCID As Integer) As DataTable
            Dim sql As String = "select t.*, (select coalesce(avg(VendorPrice),-1) from VendorProductPrice vpp inner join LLCVendor lv on vpp.VendorId=lv.VendorId where vpp.VendorPrice is not null and lv.LLCID=" & DB.Number(LLCID) & " and vpp.ProductId=t.ProductId) as AvgPrice from TakeoffProduct t where t.TakeoffId=" & DB.Number(TakeoffId)
            Return DB.GetDataTable(sql)
        End Function

        Public Function GetVendorPricingTable(ByVal Vendors As String) As DataTable
            Dim sql As String = "select * from TakeOffProduct t left outer join VendorProductPrice vp on t.ProductId=vp.ProductId inner join Vendor v on vp.VendorId=v.VendorId where v.VendorId in " & DB.NumberMultiple(Vendors) & " and t.TakeoffId=" & DB.Number(TakeOffID) & " order by t.ProductId, vp.VendorId"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetSpecialVendorPricing(ByVal DB As Database, ByVal TakeoffId As Integer, ByVal VendorId As Integer) As DataTable
            Dim sql As String = String.Empty
            sql &= "select " _
                 & "    t.TakeoffProductId, " _
                 & "    t.ProductId, " _
                 & "    vs.VendorPrice " _
                 & "from TakeoffProduct t inner join SpecialOrderProduct s on t.SpecialOrderProductId=s.SpecialOrderProductId" _
                 & "    inner join VendorSpecialOrderProductPrice vs on "
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuilderTakeoffCount(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal Project As String = "") As Integer
            ' Return DB.ExecuteScalar("select count(*) from Takeoff t where Title is not null and BuilderID=" & DB.Number(BuilderID) & " and not exists (select * from PriceComparison where TakeoffId=t.TakeoffId and IsAdminComparison = 1)")
            Return DB.ExecuteScalar("select  Count(*) from (select t.TakeOffID , t.projectID ,p .ProjectName as Project,COALESCE (p.IsArchived,0) as ArchivedProject from Takeoff t left outer join Project p on t.ProjectID=p.ProjectID where t.Title is not null and t.BuilderID=" & DB.Number(BuilderID) & " and not exists (select * from PriceComparison where TakeoffId=t.TakeoffId and IsAdminComparison=1)) as tmp  where tmp.ArchivedProject <>  1  " & IIf(Project <> Nothing, " AND  tmp.ProjectID = " & Project, ""))
        End Function

        Public Shared Function GetBuilderTakeoffs(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC", Optional ByVal PageNumber As Integer = 0, Optional ByVal PageSize As Integer = 10, Optional ByVal Project As String = "", Optional ByVal TakeOffFilter As String = "") As DataTable
            Dim sqlFields As String = "t.*,p.ProjectName as Project,COALESCE (p.IsArchived,0) as ArchivedProject from Takeoff t left outer join Project p on t.ProjectID=p.ProjectID where t.Title is not null [TakeOffFilter] and t.BuilderID=" & DB.Number(BuilderID) & " and not exists (select * from PriceComparison where TakeoffId=t.TakeoffId and IsAdminComparison=1)"
            Dim sql As String = "select " & sqlFields
            Dim sqlCondition As String = String.Empty
            If Project <> Nothing Then
                sqlCondition = " AND tmp.ProjectID = " & Project
            End If

            If TakeOffFilter <> String.Empty Then
                sqlFields = sqlFields.Replace(" [TakeOffFilter] ", " AND t.Title LIKE '" & TakeOffFilter & "%'")
            Else
                sqlFields = sqlFields.Replace(" [TakeOffFilter] ", " ")
            End If

            If SortBy <> String.Empty Then
                Dim orderby As String = Core.ProtectParam(SortBy & " " & SortOrder)
                If PageNumber > 0 Then
                    sqlFields = "Row_Number() Over(order by " & orderby & ") as RowNumber," & sqlFields
                    'sql = "select top " & (PageNumber * PageSize) & " * from (select " & sqlFields & ") as tmp where tmp.RowNumber >=" & DB.Number(PageSize * (PageNumber - 1)) & " order by tmp.RowNumber"
                    sql = "select top " & (PageNumber * PageSize) & " * from (select " & sqlFields & ") as tmp  where tmp.ArchivedProject <> 1  " _
                       & sqlCondition _
                      & " order by tmp.RowNumber"
                Else
                    sql = "select " & sqlFields & " order by " & orderby
                End If
            End If

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTakeoffProductPrices(ByVal DB As Database, ByVal TakeoffID As Integer, ByVal Vendors As String) As DataTable
            Dim sql As String = _
                  "select " _
                & "     t.TakeoffProductID," _
                & "     t.ProductId as ProductID, " _
                & "     v.VendorPrice, " _
                & "     v.VendorSku," _
                & "     t.Quantity," _
                & "     v.VendorID" _
                & " from " _
                & "     TakeoffProduct t inner join VendorProductPrice v on t.ProductID=v.ProductID" _
                & " where " _
                & "     t.TakeoffID = " & DB.Number(TakeoffID) _
                & " and " _
                & "     v.VendorID in " & DB.NumberMultiple(Vendors) _
                & " and " _
                & "     not exists (select * from VendorProductSubstitute where VendorID=v.VendorID and ProductID=t.ProductID)" _
                & " and " _
                & "     v.VendorPrice is not null"


            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTakeoffSubstitutions(ByVal DB As Database, ByVal TakeoffID As Integer, ByVal Vendors As String) As DataTable
            Dim sql As String = _
                  "select " _
                & "     t.TakeoffProductID," _
                & "     t.ProductID," _
                & "     s.SubstituteProductID as SubProductID," _
                & "     p.VendorPrice," _
                & "     p.VendorSku," _
                & "     t.Quantity," _
                & "     (t.Quantity * s.QuantityMultiplier) as RecommendedQuantity," _
                & "     s.VendorID," _
                & "     sub.Product as SubstituteProduct" _
                & " from " _
                & "     TakeoffProduct t inner join VendorProductSubstitute s on t.ProductID=s.ProductID" _
                & "         inner join VendorProductPrice as p on s.SubstituteProductID=p.ProductID and s.VendorID=p.VendorID" _
                & "         inner join Product sub on s.SubstituteProductID=sub.ProductID" _
                & " where " _
                & "     t.TakeoffID=" & DB.Number(TakeoffID) _
                & " and " _
                & "     s.VendorID in " & DB.NumberMultiple(Vendors) _
                & " and " _
                & "     p.VendorPrice is not null" _
                & " union " _
                & "select " _
                & "     t.TakeoffProductID," _
                & "     t.ProductID," _
                & "     s.SubstituteProductID as SubProductID," _
                & "     p.VendorPrice," _
                & "     p.VendorSku," _
                & "     t.Quantity," _
                & "     s.RecommendedQuantity," _
                & "     s.VendorID," _
                & "     sub.Product as SubstituteProduct" _
                & " from " _
                & "     TakeoffProduct t inner join VendorTakeoffProductSubstitute s on t.TakeoffProductID=s.TakeoffProductID" _
                & "         inner join VendorProductPrice as p on s.SubstituteProductID=p.ProductID and s.VendorID=p.VendorID" _
                & "         inner join Product sub on s.SubstituteProductID=sub.ProductID" _
                & " where " _
                & "     t.TakeoffID=" & DB.Number(TakeoffID) _
                & " and " _
                & "     s.VendorID in " & DB.NumberMultiple(Vendors) _
                & " and " _
                & "     p.VendorPrice is not null"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTakeoffAveragePrices(ByVal DB As Database, ByVal TakeoffID As Integer) As DataTable
            Dim LLCID As Integer = DB.ExecuteScalar("select LLCID from Builder where BuilderID = (select top 1 BuilderID from Takeoff where TakeoffID=" & DB.Number(TakeoffID) & ")")
            Dim sql As String = _
                  "select " _
                & "     t.TakeoffProductID," _
                & "     t.ProductID," _
                & "     coalesce(tmp.AvgPrice,0) as AvgPrice," _
                & "     t.Quantity" _
                & " from " _
                & "     TakeoffProduct t left outer join " _
                & "         (select ProductID, avg(VendorPrice) as AvgPrice from VendorProductPrice " _
                & "          where VendorID in (select lv.VendorID from LLCVendor lv Inner Join Vendor v On lv.VendorId = v.VendorId where v.IsActive = 1 And lv.LLCID=" & LLCID & ")" _
                & "          and VendorPrice is not null and VendorPrice > 0 and IsSubstitution=0" _
                & "          group by ProductID) as tmp on tmp.ProductID=t.ProductID"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTakeoffSpecialPrices(ByVal DB As Database, ByVal TakeoffID As Integer, ByVal Vendors As String) As DataTable
            Dim sql As String = _
                  "select " _
                & "     t.TakeoffProductID," _
                & "     t.SpecialOrderProductID," _
                & "     p.VendorPrice," _
                & "     p.VendorSku," _
                & "     t.Quantity," _
                & "     p.VendorID" _
                & " from " _
                & "     TakeoffProduct t inner join VendorSpecialOrderProductPrice p on t.SpecialOrderProductID=p.SpecialOrderProductID" _
                & " where " _
                & "     t.TakeoffID=" & DB.Number(TakeoffID) _
                & " and " _
                & "     p.VendorID in " & DB.NumberMultiple(Vendors)

            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class TakeOffRowBase
        Private m_DB As Database
        Private m_TakeOffID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_ProjectID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_Saved As DateTime = Nothing
        Private m_Archived As DateTime = Nothing


        Public Property TakeOffID() As Integer
            Get
                Return m_TakeOffID
            End Get
            Set(ByVal Value As Integer)
                m_TakeOffID = value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
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

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
            End Set
        End Property

        Public Property ProjectID() As Integer
            Get
                Return m_ProjectID
            End Get
            Set(ByVal value As Integer)
                m_ProjectID = value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public ReadOnly Property Saved() As DateTime
            Get
                Return m_Saved
            End Get
        End Property

        Public Property Archived() As DateTime
            Get
                Return m_Archived
            End Get
            Set(ByVal Value As DateTime)
                m_Archived = value
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

        Public Sub New(ByVal DB As Database, ByVal TakeOffID As Integer)
            m_DB = DB
            m_TakeOffID = TakeOffID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TakeOff WHERE TakeOffID = " & DB.Number(TakeOffID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TakeOffID = Convert.ToInt32(r.Item("TakeOffID"))
            If IsDBNull(r.Item("Title")) Then
                m_Title = Nothing
            Else
                m_Title = Convert.ToString(r.Item("Title"))
            End If
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
            If IsDBNull(r.Item("VendorID")) Then
                m_VendorID = Nothing
            Else
                m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            End If
            m_Created = Convert.ToDateTime(r.Item("Created"))
            If IsDBNull(r.Item("Saved")) Then
                m_Saved = Nothing
            Else
                m_Saved = Convert.ToDateTime(r.Item("Saved"))
            End If
            If IsDBNull(r.Item("Archived")) Then
                m_Archived = Nothing
            Else
                m_Archived = Convert.ToDateTime(r.Item("Archived"))
            End If
            m_ProjectID = Core.GetInt(r.Item("ProjectID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TakeOff (" _
             & " Title" _
             & ",BuilderID" _
             & ",AdminID" _
             & ",VendorID" _
             & ",Created" _
             & ",Saved" _
             & ",Archived" _
             & ",ProjectID" _
             & ") VALUES (" _
             & m_DB.Quote(Title) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(AdminID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullQuote(Now()) _
             & "," & m_DB.NullQuote(Now()) _
             & "," & m_DB.NullQuote(Archived) _
             & "," & m_DB.NullNumber(ProjectID) _
             & ")"

            TakeOffID = m_DB.InsertSQL(SQL)

            Return TakeOffID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TakeOff SET " _
             & " Title = " & m_DB.Quote(Title) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",AdminID = " & m_DB.NullNumber(AdminID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",Saved = " & m_DB.NullQuote(Now) _
             & ",Archived = " & m_DB.NullQuote(Archived) _
             & ",ProjectID = " & m_DB.NullNumber(ProjectID) _
             & " WHERE TakeOffID = " & m_DB.Quote(TakeOffID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM TakeoffProduct WHERE TakeOffID = " & m_DB.Number(TakeOffID)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM TakeOff WHERE TakeOffID = " & m_DB.Number(TakeOffID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class TakeOffCollection
        Inherits GenericCollection(Of TakeOffRow)
    End Class

End Namespace


