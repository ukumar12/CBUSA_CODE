Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports DataLayer
Imports Components
Imports System.Data.Linq
Imports System.Linq
Imports System.Reflection
Imports System.Collections.Generic
Imports System.ComponentModel

Namespace DataLayer

    Public Class TwoPriceTakeOffRow
        Inherits TwoPriceTakeOffRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer)
            MyBase.New(DB, TwoPriceTakeOffID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer) As TwoPriceTakeOffRow
            Dim row As TwoPriceTakeOffRow

            row = New TwoPriceTakeOffRow(DB, TwoPriceTakeOffID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer)
            Dim row As TwoPriceTakeOffRow

            row = New TwoPriceTakeOffRow(DB, TwoPriceTakeOffID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceTakeOff"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetRowByTwoPriceCampaignId(ByVal DB As Database, ByVal TwoPriceCampaignId As Integer) As TwoPriceTakeOffRow
            Dim SQL As String = "select * from TwoPriceTakeOff where TwoPriceCampaignId = " & DB.Number(TwoPriceCampaignId)
            Dim row As New TwoPriceTakeOffRow(DB)
            Dim sqlr As SqlDataReader = DB.GetReader(SQL)
            If sqlr.Read Then
                row.Load(sqlr)
            End If
            sqlr.Close()
            Return row
        End Function
        Public Function AddProduct(ByVal ProductId As Integer, ByVal Quantity As Integer) As Integer
            Dim dbProduct As New TwoPriceTakeOffProductRow(DB)
            dbProduct.TwoPriceTakeOffID = TwoPriceTakeOffID
            dbProduct.ProductID = ProductId
            dbProduct.Quantity = Quantity
            Return dbProduct.Insert()
        End Function

        Public Function UpdateProduct(ByVal ProductId As Integer, ByVal Quantity As Integer, ByVal ReplaceQuantity As Boolean) As Integer
            Dim dbProduct As TwoPriceTakeOffProductRow = TwoPriceTakeOffProductRow.GetRow(DB, TwoPriceTakeOffID, ProductId)
            If dbProduct.TwoPriceTakeOffProductID = Nothing Then
                dbProduct.TwoPriceTakeOffID = TwoPriceTakeOffID
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
            Return dbProduct.TwoPriceTakeOffProductID
        End Function

        Public Function RemoveProduct(ByVal TwoPriceTakeOffProductId As Integer) As Boolean
            TwoPriceTakeOffProductRow.RemoveRow(DB, TwoPriceTakeOffProductId)
        End Function

        Public Function Copy(Optional ByVal NewTitle As String = "") As TwoPriceTakeOffRow
            DB.BeginTransaction()
            Try
                Dim dbNew As New TwoPriceTakeOffRow(DB)
                dbNew.AdminID = AdminID
                dbNew.Archived = Archived
                dbNew.BuilderID = BuilderID
                dbNew.ProjectID = ProjectID
                dbNew.Title = IIf(NewTitle = String.Empty, Title, NewTitle)
                dbNew.VendorID = VendorID
                dbNew.Insert()

                Dim sql As String = _
                    "insert into TwoPriceTakeOffProduct(" _
                    & " TwoPriceTakeOffId" _
                    & ",ProductId" _
                    & ",SpecialOrderProductId" _
                    & ",Quantity" _
                    & ",SortOrder" _
                    & ") select " _
                    & DB.Number(dbNew.TwoPriceTakeOffID) _
                    & ",ProductId" _
                    & ",SpecialOrderProductId" _
                    & ",Quantity" _
                    & ",(select max(SortOrder) + 1 from TwoPriceTakeOffProduct)" _
                    & " from TwoPriceTakeOffProduct" _
                    & " where TwoPriceTakeOffID=" & DB.Number(TwoPriceTakeOffID) _
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

        Public Shared Sub CopyToTwoPriceTakeOff(ByVal DB As Database, ByVal SrcTakeOffId As Integer, ByVal DestTwoPriceTakeOffId As Integer)
            Dim colTarget As TwoPriceTakeOffProductCollection = TwoPriceTakeOffProductRow.GetCollection(DB, "SortOrder", TwoPriceTakeOffId:=DestTwoPriceTakeOffId)
            Dim colSource As TakeOffProductCollection = TakeOffProductRow.GetCollection(DB, "SortOrder", TakeOffId:=SrcTakeOffId)
            'Update Quantity of Target with
            For Each rowSource As TakeOffProductRow In colSource
                ' Check to see if Product exists in Target
                Dim bContains As Boolean = (From row As TwoPriceTakeOffProductRow In colTarget
                                           Where row.ProductID = rowSource.ProductID
                                           Select row).Count()
                'If it exists then go throught he collection and updated the collection
                If bContains Then
                    For Each rowTarget As TwoPriceTakeOffProductRow In colTarget
                        If rowTarget.ProductID = rowSource.ProductID Then
                            rowTarget.Quantity += rowSource.Quantity
                        End If
                    Next
                Else 'Create new TargetRow aka TwoPriceTakeOffProductRow
                    Dim NewSortOrder As Integer = (From row As TwoPriceTakeOffProductRow In colTarget
                                       Select row.SortOrder Order By SortOrder Descending).Take(1)(0)
                    Dim dbProd As New TwoPriceTakeOffProductRow(DB)
                    dbProd.ProductID = rowSource.ProductID
                    dbProd.Quantity = rowSource.Quantity
                    dbProd.SortOrder = NewSortOrder + 1
                    dbProd.TwoPriceTakeOffID = DestTwoPriceTakeOffId
                    'Add it to the Target Collection.  We will update the database shortly
                    colTarget.Add(dbProd)
                End If
            Next
            'Now lets update the Target
            'Delete all existing Target Rows from DB
            DB.ExecuteSQL("Delete from TwoPriceTakeOffProduct WHERE TwoPriceTakeOffID=" & DB.Number(DestTwoPriceTakeOffId))

            'Insert all records in the new Collection
            Using BulkCopy As New SqlBulkCopy(DB.Connection.ConnectionString)
                BulkCopy.DestinationTableName = "TwoPriceTakeOffProduct"
                Dim ignorelist() As String = {"DB", "TwoPriceTakeOffProductID"}
                Dim dt As DataTable = ToDataTable(Of TwoPriceTakeOffProductRow)(colTarget, ignorelist)
                BulkCopy.ColumnMappings.Add("TwoPriceTakeOffID", "TwoPriceTakeOffID")
                BulkCopy.ColumnMappings.Add("ProductID", "ProductID")
                BulkCopy.ColumnMappings.Add("SpecialOrderProductID", "SpecialOrderProductID")
                BulkCopy.ColumnMappings.Add("Quantity", "Quantity")
                BulkCopy.ColumnMappings.Add("SortOrder", "SortOrder")
                BulkCopy.WriteToServer(dt)
            End Using
        End Sub
        'Public Shared Function ToArrayOfRows(ByVal listOfProducts As TwoPriceTakeOffProductCollection, Optional ByVal ignoreList() As String = Nothing)
        '    Dim dt As DataTable
        '    For Each prod As TwoPriceTakeOffProductRow In listOfProducts

        '    Next
        'End Function
        Public Shared Function ToDataTable(Of T)(data As IList(Of T), Optional ByVal ignoreList() As String = Nothing) As DataTable
            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(T))
            Dim dt As New DataTable()
            For i As Integer = 0 To props.Count - 1
                Dim prop As PropertyDescriptor = props(i)
                If Not ignoreList.Contains(prop.Name) Then
                    dt.Columns.Add(prop.Name, prop.PropertyType)
                End If
            Next
            For Each item As T In data
                'For i As Integer = 0 To values.Length - 1
                '    If Not ignoreList.Contains(props(i).Name) Then
                '        values(props(i).Name) = props(i).GetValue(item)
                '    End If
                'Next
                Dim row As DataRow = dt.NewRow
                For Each prop As PropertyDescriptor In props
                    If Not ignoreList.Contains(prop.Name) Then
                        row(prop.Name) = prop.GetValue(item)
                    End If
                Next
                dt.Rows.Add(row)
            Next
            Return dt
        End Function
        Public Shared Function ConvertToDataTable(Of T)(ByVal list As IList(Of T), Optional ByVal ignoreList() As String = Nothing) As DataTable
            Dim table As New DataTable()
            Dim fields() As FieldInfo = GetType(T).GetFields()
            For Each field As FieldInfo In fields
                If Not ignoreList.Contains(field.Name) Then
                    table.Columns.Add(field.Name, field.FieldType)
                End If
            Next
            For Each item As T In list
                Dim row As DataRow = table.NewRow()
                For Each field As FieldInfo In fields
                    row(field.Name) = field.GetValue(item)
                Next
                table.Rows.Add(row)
            Next
            Return table
        End Function

        Public Function GetProductLineItems(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, ByVal ProductId As Integer) As DataTable
            Dim sql As String = "select * from TwoPriceTakeOffProduct t inner join Product p on t.ProductId=p.ProductId where t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) & " and p.ProductId=" & DB.Number(ProductId)
            Return DB.GetDataTable(sql)
        End Function

        Public Function GetProductsList() As DataTable
            Return GetTwoPriceTakeOffProducts(DB, TwoPriceTakeOffID)
        End Function

        Public Shared Function GetTwoPriceTakeOffProducts(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, Optional ByVal VendorId As Integer = Nothing) As DataTable
            Dim sql As String
            If VendorId = Nothing Then
                sql = "select * from TwoPriceTakeOffProduct t left outer join Product p on t.ProductId=p.ProductId left outer join SpecialOrderProduct sp on t.SpecialOrderProductId=sp.SpecialOrderProductId where TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) & " order by SortOrder"
            Else
                sql = "select * from TwoPriceTakeOffProduct t left outer join Product p on t.ProductId=p.ProductId left outer join SpecialOrderProduct sp on t.SpecialOrderProductId=sp.SpecialOrderProductId inner join TwoPriceVendorProductPrice v on p.ProductId=v.ProductId where TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId) & " and v.VendorId=" & DB.Number(VendorId) & " order by SortOrder"
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetAwardedTwoPriceTakeOffProducts(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, Optional ByVal TwoPriceCampaignId As Integer = 0) As DataTable
            Dim sql As String
            Dim VendorId As Integer = DB.ExecuteScalar("SELECT TOP 1 AwardedVendorId FROM TwoPriceCampaign WHERE TwoPriceCampaignId IN (SELECT TOP 1 TwoPriceCampaignId FROM TwoPriceTakeOff WHERE TwoPriceTakeOffId = " & DB.Number(TwoPriceTakeOffId) & ")")

            If VendorId = Nothing Then
                Return Nothing
            Else
                sql = " select t.*, v.*, p.*, vpp.VendorPrice AS OldPrice from TwoPriceTakeOffProduct t  " & _
                      " left outer join Product p on t.ProductId=p.ProductId " & _
                      " inner join TwoPriceVendorProductPrice v on t.ProductId=v.ProductId AND v.TwoPriceCampaignID = (SELECT TwoPriceCampaignID FROM TwoPriceTakeOff WHERE TwoPriceTakeOffID = " & DB.Number(TwoPriceTakeOffId) & ")" & _
                      " left join VendorProductPrice vpp ON vpp.ProductID = t.ProductID AND vpp.VendorId = v.VendorID " & _
                      " where v.VendorId=" & DB.Number(VendorId) & " AND v.Submitted = 1 "
                If TwoPriceCampaignId <> 0 Then
                    sql &= " AND TwoPriceCampaignId = " & TwoPriceCampaignId
                End If
                sql &= " order by SortOrder"
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceTakeOffProductAverages(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, ByVal LLCID As Integer) As DataTable
            Dim sql As String = "select t.*, (select coalesce(avg(VendorPrice),-1) from VendorProductPrice vpp inner join LLCVendor lv on vpp.VendorId=lv.VendorId where vpp.VendorPrice is not null and lv.LLCID=" & DB.Number(LLCID) & " and vpp.ProductId=t.ProductId) as AvgPrice from TwoPriceTakeOffProduct t where t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffId)
            Return DB.GetDataTable(sql)
        End Function

        Public Function GetVendorPricingTable(ByVal Vendors As String) As DataTable
            Dim sql As String = "select * from TwoPriceTakeOffProduct t left outer join VendorProductPrice vp on t.ProductId=vp.ProductId inner join Vendor v on vp.VendorId=v.VendorId where v.VendorId in " & DB.NumberMultiple(Vendors) & " and t.TwoPriceTakeOffId=" & DB.Number(TwoPriceTakeOffID) & " order by t.ProductId, vp.VendorId"
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetSpecialVendorPricing(ByVal DB As Database, ByVal TwoPriceTakeOffId As Integer, ByVal VendorId As Integer) As DataTable
            Dim sql As String = String.Empty
            sql &= "select " _
                 & "    t.TwoPriceTakeOffProductId, " _
                 & "    t.ProductId, " _
                 & "    vs.VendorPrice " _
                 & "from TwoPriceTakeOffProduct t inner join SpecialOrderProduct s on t.SpecialOrderProductId=s.SpecialOrderProductId" _
                 & "    inner join VendorSpecialOrderProductPrice vs on "
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetBuilderTwoPriceTakeOffCount(ByVal DB As Database, ByVal BuilderID As Integer) As Integer
            Return DB.ExecuteScalar("select count(*) from TwoPriceTakeOff t where Title is not null and BuilderID=" & DB.Number(BuilderID) & " and not exists (select * from PriceComparison where TwoPriceTakeOffId=t.TwoPriceTakeOffId and IsAdminComparison = 1)")
        End Function

        Public Shared Function GetBuilderTwoPriceTakeOffs(ByVal DB As Database, ByVal BuilderID As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC", Optional ByVal PageNumber As Integer = 0, Optional ByVal PageSize As Integer = 10) As DataTable
            Dim sqlFields As String = "t.*,p.ProjectName as Project from TwoPriceTakeOff t left outer join Project p on t.ProjectID=p.ProjectID where t.Title is not null and t.BuilderID=" & DB.Number(BuilderID) & " and not exists (select * from PriceComparison where TwoPriceTakeOffId=t.TwoPriceTakeOffId and IsAdminComparison=1)"
            Dim sql As String = "select " & sqlFields

            If SortBy <> String.Empty Then
                Dim orderby As String = Core.ProtectParam(SortBy & " " & SortOrder)
                If PageNumber > 0 Then
                    sqlFields = "Row_Number() Over(order by " & orderby & ") as RowNumber," & sqlFields
                    'sql = "select top " & (PageNumber * PageSize) & " * from (select " & sqlFields & ") as tmp where tmp.RowNumber >=" & DB.Number(PageSize * (PageNumber - 1)) & " order by tmp.RowNumber"
                    sql = "select top " & (PageNumber * PageSize) & " * from (select " & sqlFields & ") as tmp order by tmp.RowNumber"
                Else
                    sql = "select " & sqlFields & " order by " & orderby
                End If
            End If

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceTakeOffProductPrices(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer, ByVal Vendors As String) As DataTable
            Dim sql As String = _
                  "select " _
                & "     t.TwoPriceTakeOffProductID," _
                & "     t.ProductId as ProductID, " _
                & "     v.Price AS VendorPrice, " _
                & "     v.Comments AS Comments, " _
                & "     t.Quantity," _
                & "     v.VendorID" _
                & " from " _
                & "     TwoPriceTakeOffProduct t inner join TwoPriceVendorProductPrice v on t.ProductID=v.ProductID AND v.TwoPriceCampaignID = (SELECT TwoPriceCampaignID FROM TwoPriceTakeOff WHERE TwoPriceTakeOffID = " & TwoPriceTakeOffID & ")" _
                & " where " _
                & "     v.VendorID in " & DB.NumberMultiple(Vendors) _
                & " and " _
                & "     v.Price is not null" _
                & " and " _
                & "     v.Submitted = 1"


            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceTakeOffSubstitutions(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer, ByVal Vendors As String) As DataTable
            Dim sql As String = _
                  "select " _
                & "     t.TwoPriceTakeOffProductID," _
                & "     t.ProductID," _
                & "     s.SubstituteProductID as SubProductID," _
                & "     p.VendorPrice," _
                & "     p.VendorSku," _
                & "     t.Quantity," _
                & "     (t.Quantity * s.QuantityMultiplier) as RecommendedQuantity," _
                & "     s.VendorID," _
                & "     sub.Product as SubstituteProduct" _
                & " from " _
                & "     TwoPriceTakeOffProduct t inner join VendorProductSubstitute s on t.ProductID=s.ProductID" _
                & "         inner join VendorProductPrice as p on s.SubstituteProductID=p.ProductID and s.VendorID=p.VendorID" _
                & "         inner join Product sub on s.SubstituteProductID=sub.ProductID" _
                & " where " _
                & "     t.TwoPriceTakeOffID=" & DB.Number(TwoPriceTakeOffID) _
                & " and " _
                & "     s.VendorID in " & DB.NumberMultiple(Vendors) _
                & " and " _
                & "     p.VendorPrice is not null" _
                & " union " _
                & "select " _
                & "     t.TwoPriceTakeOffProductID," _
                & "     t.ProductID," _
                & "     s.SubstituteProductID as SubProductID," _
                & "     p.VendorPrice," _
                & "     p.VendorSku," _
                & "     t.Quantity," _
                & "     s.RecommendedQuantity," _
                & "     s.VendorID," _
                & "     sub.Product as SubstituteProduct" _
                & " from " _
                & "     TwoPriceTakeOffProduct t inner join VendorTakeOffProductSubstitute s on t.TwoPriceTakeOffProductID=s.TwoPriceTakeOffProductID" _
                & "         inner join VendorProductPrice as p on s.SubstituteProductID=p.ProductID and s.VendorID=p.VendorID" _
                & "         inner join Product sub on s.SubstituteProductID=sub.ProductID" _
                & " where " _
                & "     t.TwoPriceTakeOffID=" & DB.Number(TwoPriceTakeOffID) _
                & " and " _
                & "     s.VendorID in " & DB.NumberMultiple(Vendors) _
                & " and " _
                & "     p.VendorPrice is not null"

            Return DB.GetDataTable(sql)
        End Function


        Public Shared Function GetTwoPriceBuilderTakeoffSubstitutions(ByVal DB As Database, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer, ByVal Vendors As String) As DataTable
            Dim sql As String = _
                  "select " _
                & "     t.TwoPriceBuilderTakeOffProductPendingID," _
                & "     t.ProductID," _
                & "     s.SubstituteProductID as SubProductID," _
                & "     p.VendorPrice," _
                & "     p.VendorSku," _
                & "     t.Quantity," _
                & "     (t.Quantity * s.QuantityMultiplier) as RecommendedQuantity," _
                & "     s.VendorID," _
                & "     sub.Product as SubstituteProduct" _
                & " from " _
                & "     TwoPriceBuilderTakeOffProductPending t inner join VendorProductSubstitute s on t.ProductID=s.ProductID" _
                & "         inner join VendorProductPrice as p on s.SubstituteProductID=p.ProductID and s.VendorID=p.VendorID" _
                & "         inner join Product sub on s.SubstituteProductID=sub.ProductID" _
                & " where " _
                & "     t.TwoPriceBuilderTakeOffProductPendingID=" & DB.Number(TwoPriceBuilderTakeOffProductPendingID) _
                & " and " _
                & "     s.VendorID in " & DB.NumberMultiple(Vendors) _
                & " and " _
                & "     p.VendorPrice is not null"
            Return DB.GetDataTable(sql)
        End Function


        Public Shared Function GetTwoPriceTakeOffAveragePrices(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer) As DataTable
            Dim LLCID As Integer = DB.ExecuteScalar("select LLCID from Builder where BuilderID = (select top 1 BuilderID from TwoPriceTakeOff where TwoPriceTakeOffID=" & DB.Number(TwoPriceTakeOffID) & ")")
            Dim sql As String = _
                  "select " _
                & "     t.TwoPriceTakeOffProductID," _
                & "     t.ProductID," _
                & "     coalesce(tmp.AvgPrice,0) as AvgPrice," _
                & "     t.Quantity" _
                & " from " _
                & "     TwoPriceTakeOffProduct t left outer join " _
                & "         (select ProductID, avg(Price) as AvgPrice from TwoPriceVendorProductPrice " _
                & "          where VendorID in (select VendorId from TwoPriceCampaignVendor_Rel where TwoPriceCampaignId = (SELECT TwoPriceCampaignID FROM TwoPriceTakeoff WHERE TwoPriceTakeOffId = " & TwoPriceTakeOffID & ")) GROUP BY ProductId) as tmp on tmp.ProductID=t.ProductID"

            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceTakeOffSpecialPrices(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer, ByVal Vendors As String) As DataTable
            Dim sql As String = _
                  "select " _
                & "     t.TwoPriceTakeOffProductID," _
                & "     t.SpecialOrderProductID," _
                & "     p.VendorPrice," _
                & "     p.VendorSku," _
                & "     t.Quantity," _
                & "     p.VendorID" _
                & " from " _
                & "     TwoPriceTakeOffProduct t inner join VendorSpecialOrderProductPrice p on t.SpecialOrderProductID=p.SpecialOrderProductID" _
                & " where " _
                & "     t.TwoPriceTakeOffID=" & DB.Number(TwoPriceTakeOffID) _
                & " and " _
                & "     p.VendorID in " & DB.NumberMultiple(Vendors)

            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class TwoPriceTakeOffRowBase
        Private m_DB As Database
        Private m_TwoPriceTakeOffID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_ProjectID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_Saved As DateTime = Nothing
        Private m_Archived As DateTime = Nothing

        Public Property TwoPriceCampaignId As Integer


        Public Property TwoPriceTakeOffID() As Integer
            Get
                Return m_TwoPriceTakeOffID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceTakeOffID = Value
            End Set
        End Property

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = Value
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

        Public Property AdminID() As Integer
            Get
                Return m_AdminID
            End Get
            Set(ByVal Value As Integer)
                m_AdminID = Value
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
                m_Archived = Value
            End Set
        End Property



        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TwoPriceTakeOffID As Integer)
            m_DB = DB
            m_TwoPriceTakeOffID = TwoPriceTakeOffID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceTakeOff WHERE TwoPriceTakeOffID = " & DB.Number(TwoPriceTakeOffID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_TwoPriceTakeOffID = Convert.ToInt32(r.Item("TwoPriceTakeOffID"))
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
            TwoPriceCampaignId = Core.GetString(r.Item("TwoPriceCampaignId"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TwoPriceTakeOff (" _
             & " Title" _
             & ",BuilderID" _
             & ",AdminID" _
             & ",VendorID" _
             & ",Created" _
             & ",Saved" _
             & ",Archived" _
             & ",ProjectID" _
             & ",TwoPriceCampaignId" _
             & ") VALUES (" _
             & m_DB.Quote(Title) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(AdminID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullQuote(Now()) _
             & "," & m_DB.NullQuote(Now()) _
             & "," & m_DB.NullQuote(Archived) _
             & "," & m_DB.NullNumber(ProjectID) _
             & "," & m_DB.NullNumber(TwoPriceCampaignId) _
             & ")"

            TwoPriceTakeOffID = m_DB.InsertSQL(SQL)

            Return TwoPriceTakeOffID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceTakeOff SET " _
             & " Title = " & m_DB.Quote(Title) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",AdminID = " & m_DB.NullNumber(AdminID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",Saved = " & m_DB.NullQuote(Now) _
             & ",Archived = " & m_DB.NullQuote(Archived) _
             & ",ProjectID = " & m_DB.NullNumber(ProjectID) _
             & ",TwoPriceCampaignId = " & m_DB.NullNumber(TwoPriceCampaignId) _
             & " WHERE TwoPriceTakeOffID = " & m_DB.Quote(TwoPriceTakeOffID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceTakeOffProduct WHERE TwoPriceTakeOffID = " & m_DB.Number(TwoPriceTakeOffID)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM TwoPriceTakeOff WHERE TwoPriceTakeOffID = " & m_DB.Number(TwoPriceTakeOffID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class TwoPriceTakeOffCollection
        Inherits GenericCollection(Of TwoPriceTakeOffRow)
    End Class

End Namespace


