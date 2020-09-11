Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports System.Net.Mail
Imports Components

Namespace DataLayer

    Public Class StoreItemRow
        Inherits StoreItemRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ItemId As Integer)
            MyBase.New(DB, ItemId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ItemId As Integer) As StoreItemRow
            Dim row As StoreItemRow

            row = New StoreItemRow(DB, ItemId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ItemId As Integer)
            Dim row As StoreItemRow

            row = New StoreItemRow(DB, ItemId)
            row.Remove()
        End Sub

        'Custom Methods
		Public Shared Function GetAlternateImages(ByVal DB As Database, ByVal ItemId As Integer, Optional ByVal TopRecords As Integer = Nothing) As DataTable
			Dim SQL As String = "select " & IIf(TopRecords > 0, "top " & TopRecords, "") & " * from storeitemimage where itemid = " & ItemId & " order by sortorder"
			Return DB.GetDataTable(SQL)
		End Function

		Public Shared Function GetAlternateImagesCount(ByVal DB As Database, ByVal ItemId As Integer, Optional ByVal TopRecords As Integer = Nothing) As Integer
			Dim SQL As String = "select count(*) from storeitemimage where itemid = " & ItemId
			Return DB.ExecuteScalar(SQL)
		End Function

		Public Shared Function GetRowBySKU(ByVal DB As Database, ByVal SKU As String) As StoreItemRow
			Dim SQL As String = "SELECT * FROM StoreItem WHERE SKU = " & DB.Quote(SKU)
			Dim r As SqlDataReader
			Dim row As StoreItemRow = New StoreItemRow(DB)
			r = DB.GetReader(SQL)
			If r.Read Then
				row.Load(r)
			End If
			r.Close()
			Return row
		End Function

        Public Shared Function GetRowByCustomURL(ByVal DB As Database, ByVal Url As String) As StoreItemRow
            Dim SQL As String = "SELECT * FROM StoreItem WHERE CustomURL = " & DB.Quote(Url)
            Dim r As SqlDataReader
            Dim row As StoreItemRow = New StoreItemRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

		Public Shared Function GetItemAttributes(ByVal DB As Database, ByVal TemplateAttributeId As Integer, ByVal ItemId As Integer) As DataTable
			Return DB.GetDataTable("SELECT * FROM StoreItemAttribute WHERE TemplateAttributeId=" & TemplateAttributeId & " AND ItemId=" & ItemId & " ORDER BY SortOrder")
		End Function

		Public Sub InsertRelatedItem(ByVal ChildItemId As Integer)
			If ChildItemId = Nothing Then
				Exit Sub
			End If
			Dim SQL As String = "INSERT INTO StoreRelatedItem (ParentId, ItemId, SortOrder) Select " & DB.Quote(ItemId) & ", ItemId, coalesce((select Max(SortOrder) from StoreRelatedItem where ParentId = " & DB.Quote(ItemId) & "),0) + 1 FROM StoreItem WHERE ItemId = " & DB.Quote(ChildItemId)
			DB.ExecuteSQL(SQL)
		End Sub

        Public Sub RemoveRelatedItem(ByVal ItemId As Integer)
            Dim SQL As String = "delete from StoreRelatedItem where ParentId = " & Me.ItemId & " and ItemId = " & ItemId
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub RemoveDepartmentItems()
            Dim SQL As String = "delete from StoreDepartmentItem where ItemId = " & DB.Quote(ItemId.ToString)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub InsertDepartmentItems(ByVal DepartmentListSeparatedByComma As String)
            If DepartmentListSeparatedByComma = String.Empty Then
                Exit Sub
            End If
            Dim SQL As String = String.Empty

            SQL &= " INSERT INTO StoreDepartmentItem (ItemId, DepartmentId) Select " & ItemId & ", DepartmentId FROM StoreDepartment WHERE DepartmentId IN " & DB.NumberMultiple(DepartmentListSeparatedByComma)
            SQL &= " and DepartmentId not in (select DepartmentId from StoreDepartmentItem where ItemId = " & ItemId & ")"

            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetActiveItemsCount(ByVal DB As Database, ByVal Filter As DepartmentFilterField) As Integer
            Dim SQL As String = String.Empty
			SQL &= " select count(*) from StoreItem si where si.IsActive = 1 "

            If Filter.IncludeItemsFromSubdepartments Then
                If Not Filter.DepartmentId = 0 Then
                    SQL &= " and si.ItemId in ("
                    SQL &= " select ItemId from StoreDepartmentItem sdi, StoreDepartment sd, StoreDepartment p "
                    SQL &= " where p.DepartmentId = " & Filter.DepartmentId
                    SQL &= " and sdi.DepartmentId = sd.DepartmentId and p.lft <= sd.Lft and p.rgt >= sd.rgt"
                    SQL &= " )"
                End If
            Else
                If Not Filter.DepartmentId = 0 Then
                    SQL &= " and si.ItemId in ("
                    SQL &= " select ItemId from StoreDepartmentItem sdi where sdi.DepartmentId = " & Filter.DepartmentId
                    SQL &= " )"
                End If
            End If
            If Filter.IsFeatured Then
                SQL &= " and si.IsFeatured = 1"
            End If
            If Filter.IsOnSale Then
                SQL &= " and si.IsOnSale = 1"
            End If
            If Not Filter.BrandId = 0 Then
                SQL &= " and si.BrandId = " & DB.Number(Filter.BrandId)
            End If
            If Not Filter.Keyword = String.Empty Then
				SQL &= " and (si.ItemId in ("
				SQL &= " select [KEY] from CONTAINSTABLE(StoreItem, * , " & DB.Quote(Filter.Keyword) & ")"
				SQL &= " )"
				SQL &= " or si.ItemId in (select itemid from storeitemattribute where finalsku like " & DB.FilterQuote(Filter.RawKeyword) & "))"
			End If
            Return DB.ExecuteScalar(SQL)
        End Function

        Public Shared Function GetActiveItems(ByVal DB As Database, ByVal Filter As DepartmentFilterField) As StoreItemCollection
            Dim c As New StoreItemCollection
            Dim SQL As String = String.Empty
            Dim counter As Integer = 0

            If filter.MaxPerPage <> -1 Then
				SQL &= " select top " & Filter.MaxPerPage * Filter.pg & " si.* from StoreItem si where si.IsActive = 1 "
            Else
				SQL &= " select si.* from StoreItem si where si.IsActive = 1 "
            End If

            If Filter.IncludeItemsFromSubdepartments Then
                If Not Filter.DepartmentId = 0 Then
                    SQL &= " and si.ItemId in ("
                    SQL &= " select ItemId from StoreDepartmentItem sdi, StoreDepartment sd, StoreDepartment p "
                    SQL &= " where p.DepartmentId = " & Filter.DepartmentId
                    SQL &= " and sdi.DepartmentId = sd.DepartmentId and p.lft <= sd.Lft and p.rgt >= sd.rgt"
                    SQL &= " )"
                End If
            Else
                If Not Filter.DepartmentId = 0 Then
                    SQL &= " and si.ItemId in ("
                    SQL &= " select ItemId from StoreDepartmentItem sdi where sdi.DepartmentId = " & Filter.DepartmentId
                    SQL &= " )"
                End If
            End If
            If Filter.IsFeatured Then
                SQL &= " and si.IsFeatured = 1"
            End If
            If filter.IsOnSale Then
                SQL &= " and si.IsOnSale = 1"
            End If
            If Not filter.BrandId = 0 Then
                SQL &= " and si.BrandId = " & DB.Number(filter.BrandId)
            End If

            If Not Filter.Keyword = String.Empty Then
				SQL &= " and (si.ItemId in ("
                SQL &= " select [KEY] from CONTAINSTABLE(StoreItem, * , " & DB.Quote(Filter.Keyword) & ")"
				SQL &= " )"
				SQL &= " or si.ItemId in (select itemid from storeitemattribute where finalsku like " & DB.FilterQuote(Filter.RawKeyword) & "))"
            End If

            If filter.SortBy = String.Empty Then
                filter.SortBy = "case when si.IsOnSale  = 1 then si.SalePrice else si.Price end"
                filter.SortOrder = "desc"
            End If

            If Filter.SortBy.Contains("Price Asc") Then
                Filter.SortBy = "case when si.IsOnSale  = 1 then si.SalePrice else si.Price end asc"
            ElseIf Filter.SortBy.Contains("Price Desc") Then
                Filter.SortBy = "case when si.IsOnSale  = 1 then si.SalePrice else si.Price end desc"
            End If

            SQL &= " order by " & Core.ProtectParam(filter.SortBy) & " " & Core.ProtectParam(filter.SortOrder)
            If Filter.SortBy <> "ItemName" Then SQL &= ",ItemName ASC, ItemId asc" Else SQL &= ", ItemId asc"

            Dim dr As SqlDataReader = DB.GetReader(SQL)
            While dr.Read
                counter += 1

                'skip first (pg-1) * maxperpage records
                If (counter > filter.MaxPerPage * (filter.pg - 1)) Or filter.MaxPerPage = -1 Then
                    Dim item As New StoreItemRow(DB)
                    item.Load(dr)
                    c.Add(item)
                End If
            End While
            dr.Close()
            dr = Nothing
            Return c
        End Function

        Public Shared Function GetRelatedItems(ByVal DB As Database, ByVal ItemId As Integer, Optional ByVal count As Integer = 4) As DataTable
            Dim SQL As String = "select top " & count & " i.itemid, i.CustomURL, i.itemname, i.price, i.saleprice, i.isonsale, i.image from StoreItem i inner join StoreRelatedItem r on i.itemid = r.itemid where i.isactive = 1 and r.parentid = " & ItemId
            Return DB.GetDataTable(SQL)
        End Function

        Public ReadOnly Property GetSelectedStoreFeatures() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select FeatureId from StoreItemFeature where ItemId = " & ItemId)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("FeatureId")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllStoreFeatures()
            DB.ExecuteSQL("delete from StoreItemFeature where ItemId = " & ItemId)
        End Sub

        Public Sub DeleteUniqueFeatures(ByVal Elements As String)
            DB.ExecuteSQL("DELETE FROM StoreItemFeature WHERE FeatureId In " & DB.NumberMultiple(Elements) & " AND NOT ItemId = " & ItemId & " AND FeatureId IN (SELECT FeatureId FROM StoreFeature WHERE StoreFeature.FeatureId = FeatureId AND IsUnique=1)")
        End Sub

        Public Sub InsertToStoreFeatures(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToStoreFeature(Element)
            Next
        End Sub

        Public Sub InsertToStoreFeature(ByVal FeatureId As Integer)
            Dim SQL As String = "insert into StoreItemFeature (ItemId, FeatureId) values (" & ItemId & "," & FeatureId & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub InsertRecentlyViewedItem(ByVal DepartmentId As Integer, ByVal BrandId As Integer, ByVal MemberId As Integer, ByVal SessionId As String)
            DB.ExecuteSQL("INSERT INTO StoreRecentlyViewed (ItemId, DepartmentId, BrandId, SessionNo, MemberId, CreateDate) VALUES (" & ItemId & "," & DB.NullNumber(DepartmentId) & "," & DB.NullNumber(BrandId) & "," & DB.Quote(SessionId) & "," & DB.NullNumber(MemberId) & "," & DB.Quote(Now) & ")")
        End Sub

        Public Shared Function GetRecentlyViewedItems(ByVal DB As Database, ByVal Count As Integer, ByVal ItemId As Integer, ByVal MemberId As Integer, ByVal SessionId As String) As DataTable
            Dim dt As New DataTable

            Dim prams(3) As SqlParameter
            prams(0) = New SqlParameter("@Howmany", Count)
            prams(1) = New SqlParameter("@CurrentItemId", ItemId)
            prams(2) = New SqlParameter("@MemberId", MemberId)
            prams(3) = New SqlParameter("@SessionNo", SessionId)

            DB.RunProc("sp_GetRecentlyViewedItems", prams, dt)

            Return dt
		End Function

		Public Shared Function IsValidAttributes(ByVal DB As Database, ByVal ItemId As Integer, ByVal col As ItemAttributeCollection) As Boolean
			Dim SQL As String = "select templateattributeid from storeitemtemplateattribute where templateid = (select top 1 templateid from storeitem where itemid = " & ItemId & ")"
			Dim dt As DataTable = DB.GetDataTable(SQL)
			If dt.Rows.Count > 0 AndAlso (col Is Nothing OrElse Not col.Count = dt.Rows.Count) Then
				Return False
			End If

			Dim AttributeIds As String = String.Empty
			For Each a As ItemAttribute In col
				AttributeIds &= IIf(AttributeIds = String.Empty, "", ",") & a.ItemAttributeId
			Next

			SQL = "select itemattributeid, templateattributeid from storeitemattribute where itemid = " & ItemId & " and ItemAttributeId in " & DB.NumberMultiple(AttributeIds)
			Dim dtAtt As DataTable = DB.GetDataTable(SQL)

			Dim ids As New ArrayList
			For Each row As DataRow In dt.Rows
				For Each a As ItemAttribute In col
					If row("TemplateAttributeId") = a.TemplateAttributeId Then
						dtAtt.DefaultView.RowFilter = "ItemAttributeId = " & a.ItemAttributeId & " and TemplateAttributeId = " & a.TemplateAttributeId
						If ids.IndexOf(row("TemplateAttributeId")) = -1 AndAlso dtAtt.DefaultView.Count > 0 Then
							ids.Add(row("TemplateAttributeId"))
						End If
					End If
				Next
			Next

			Return ids.Count = dt.Rows.Count
		End Function
	End Class

    Public MustInherit Class StoreItemRowBase
        Private m_DB As Database
        Private m_ItemId As Integer = Nothing
        Private m_BrandId As Integer = Nothing
        Private m_TemplateId As Integer = Nothing
		Private m_DisplayMode As String = Nothing
        Private m_ItemName As String = Nothing
        Private m_SKU As String = Nothing
        Private m_PageTitle As String = Nothing
        Private m_MetaDescription As String = Nothing
        Private m_MetaKeywords As String = Nothing
        Private m_Image As String = Nothing
        Private m_Weight As Double = Nothing
        Private m_Width As Double = Nothing
		Private m_OriginalInventoryQty As Integer = Nothing
		Private m_InventoryQty As Integer = Nothing
		Private m_InventoryActionThreshold As Integer = Nothing
		Private m_InventoryWarningThreshold As Integer = Nothing
		Private m_InventoryAction As String = Nothing
		Private m_BackorderDate As DateTime = Nothing
        Private m_Height As Double = Nothing
        Private m_ThumbnailWidth As Integer = Nothing
        Private m_ThumbnailHeight As Integer = Nothing
        Private m_Thickness As Double = Nothing
        Private m_Price As Double = Nothing
        Private m_ItemUnit As String = Nothing
        Private m_CustomURL As String = Nothing
        Private m_SalePrice As Double = Nothing
        Private m_Shipping1 As Double = Nothing
        Private m_Shipping2 As Double = Nothing
        Private m_CountryUnit As Integer = Nothing
        Private m_IsOnSale As Boolean = Nothing
        Private m_IsTaxFree As Boolean = Nothing
        Private m_IsActive As Boolean = Nothing
		Private m_IsFeatured As Boolean = Nothing
		Private m_IsGiftWrap As Boolean = Nothing
		Private m_SendNotification As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_LongDescription As String = Nothing
        Private m_ShortDescription As String = Nothing
		Private OriginalCustomURL As String = Nothing

        Public Property ItemId() As Integer
            Get
                Return m_ItemId
            End Get
            Set(ByVal Value As Integer)
                m_ItemId = Value
            End Set
        End Property

        Public Property ItemName() As String
            Get
                Return m_ItemName
            End Get
            Set(ByVal Value As String)
                m_ItemName = Value
            End Set
        End Property

        Public Property SKU() As String
            Get
                Return m_SKU
            End Get
            Set(ByVal Value As String)
                m_SKU = Value
            End Set
        End Property

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = Value
            End Set
        End Property

		Public Property DisplayMode() As String
			Get
				Return m_DisplayMode
			End Get
			Set(ByVal value As String)
				m_DisplayMode = value
			End Set
		End Property

        Public Property BrandId() As Integer
            Get
                Return m_BrandId
            End Get
            Set(ByVal Value As Integer)
                m_BrandId = Value
            End Set
        End Property

        Public Property PageTitle() As String
            Get
                Return m_PageTitle
            End Get
            Set(ByVal Value As String)
                m_PageTitle = Value
            End Set
        End Property

        Public Property MetaDescription() As String
            Get
                Return m_MetaDescription
            End Get
            Set(ByVal Value As String)
                m_MetaDescription = Value
            End Set
        End Property

        Public Property MetaKeywords() As String
            Get
                Return m_MetaKeywords
            End Get
            Set(ByVal Value As String)
                m_MetaKeywords = Value
            End Set
        End Property

        Public Property Image() As String
            Get
                Return m_Image
            End Get
            Set(ByVal Value As String)
                m_Image = Value
            End Set
        End Property

        Public Property Weight() As Double
            Get
                Return m_Weight
            End Get
            Set(ByVal Value As Double)
                m_Weight = Value
            End Set
        End Property

		Public Property InventoryQty() As Integer
			Get
				Return m_InventoryQty
			End Get
			Set(ByVal value As Integer)
				m_InventoryQty = value
			End Set
		End Property

		Public Property InventoryActionThreshold() As Integer
			Get
				Return m_InventoryActionThreshold
			End Get
			Set(ByVal value As Integer)
				m_InventoryActionThreshold = value
			End Set
		End Property

		Public Property InventoryAction() As String
			Get
				Return m_InventoryAction
			End Get
			Set(ByVal value As String)
				m_InventoryAction = value
			End Set
		End Property

		Public Property InventoryWarningThreshold() As Integer
			Get
				Return m_InventoryWarningThreshold
			End Get
			Set(ByVal value As Integer)
				m_InventoryWarningThreshold = value
			End Set
		End Property

		Public Property BackorderDate() As DateTime
			Get
				Return m_BackorderDate
			End Get
			Set(ByVal value As DateTime)
				m_BackorderDate = value
			End Set
		End Property

        Public Property Width() As Double
            Get
                Return m_Width
            End Get
            Set(ByVal Value As Double)
                m_Width = Value
            End Set
        End Property

        Public Property Height() As Double
            Get
                Return m_Height
            End Get
            Set(ByVal Value As Double)
                m_Height = Value
            End Set
        End Property

        Public Property ThumbnailWidth() As Integer
            Get
                Return m_ThumbnailWidth
            End Get
            Set(ByVal Value As Integer)
                m_ThumbnailWidth = Value
            End Set
        End Property

        Public Property ThumbnailHeight() As Integer
            Get
                Return m_ThumbnailHeight
            End Get
            Set(ByVal Value As Integer)
                m_ThumbnailHeight = Value
            End Set
        End Property

        Public Property Thickness() As Double
            Get
                Return m_Thickness
            End Get
            Set(ByVal Value As Double)
                m_Thickness = Value
            End Set
        End Property

        Public Property Price() As Double
            Get
                Return m_Price
            End Get
            Set(ByVal Value As Double)
                m_Price = Value
            End Set
        End Property

        Public Property ItemUnit() As String
            Get
                Return m_ItemUnit
            End Get
            Set(ByVal Value As String)
                m_ItemUnit = Value
            End Set
        End Property

        Public Property CustomURL() As String
            Get
                Return m_CustomURL
            End Get
            Set(ByVal Value As String)
                m_CustomURL = Value
            End Set
        End Property

        Public Property SalePrice() As Double
            Get
                Return m_SalePrice
            End Get
            Set(ByVal Value As Double)
                m_SalePrice = Value
            End Set
        End Property

        Public Property Shipping1() As Double
            Get
                Return m_Shipping1
            End Get
            Set(ByVal Value As Double)
                m_Shipping1 = Value
            End Set
        End Property

        Public Property Shipping2() As Double
            Get
                Return m_Shipping2
            End Get
            Set(ByVal Value As Double)
                m_Shipping2 = Value
            End Set
        End Property

        Public Property CountryUnit() As Integer
            Get
                Return m_CountryUnit
            End Get
            Set(ByVal Value As Integer)
                m_CountryUnit = Value
            End Set
        End Property

        Public Property IsOnSale() As Boolean
            Get
                Return m_IsOnSale
            End Get
            Set(ByVal Value As Boolean)
                m_IsOnSale = Value
            End Set
        End Property

        Public Property IsTaxFree() As Boolean
            Get
                Return m_IsTaxFree
            End Get
            Set(ByVal Value As Boolean)
                m_IsTaxFree = Value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = Value
            End Set
        End Property

		Public Property IsFeatured() As Boolean
			Get
				Return m_IsFeatured
			End Get
			Set(ByVal Value As Boolean)
				m_IsFeatured = Value
			End Set
		End Property

        Public Property IsGiftWrap() As Boolean
            Get
                Return m_IsGiftWrap
            End Get
            Set(ByVal Value As Boolean)
                m_IsGiftWrap = Value
            End Set
        End Property

        Public Property LongDescription() As String
            Get
                Return m_LongDescription
            End Get
            Set(ByVal Value As String)
                m_LongDescription = Value
            End Set
        End Property

        Public Property ShortDescription() As String
            Get
                Return m_ShortDescription
            End Get
            Set(ByVal Value As String)
                m_ShortDescription = Value
            End Set
		End Property

		Public Property SendNotification() As Boolean
			Get
				Return m_SendNotification
			End Get
			Set(ByVal value As Boolean)
				m_SendNotification = value
			End Set
		End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
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
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ItemId As Integer)
            m_DB = DB
            m_ItemId = ItemId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreItem WHERE ItemId = " & DB.Number(ItemId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_ItemId = Convert.ToInt32(r.Item("ItemId"))
            m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            m_ItemName = Convert.ToString(r.Item("ItemName"))
            m_SKU = Convert.ToString(r.Item("SKU"))

			If IsDBNull(r.Item("DisplayMode")) Then m_DisplayMode = Nothing Else m_DisplayMode = Convert.ToString(r.Item("DisplayMode"))
            If IsDBNull(r.Item("BrandId")) Then m_BrandId = Nothing Else m_BrandId = Convert.ToInt32(r.Item("BrandId"))
            If IsDBNull(r.Item("PageTitle")) Then m_PageTitle = Nothing Else m_PageTitle = Convert.ToString(r.Item("PageTitle"))
            If IsDBNull(r.Item("MetaDescription")) Then m_MetaDescription = Nothing Else m_MetaDescription = Convert.ToString(r.Item("MetaDescription"))
            If IsDBNull(r.Item("MetaKeywords")) Then m_MetaKeywords = Nothing Else m_MetaKeywords = Convert.ToString(r.Item("MetaKeywords"))
            If IsDBNull(r.Item("Image")) Then m_Image = Nothing Else m_Image = Convert.ToString(r.Item("Image"))
            If IsDBNull(r.Item("Weight")) Then m_Weight = Nothing Else m_Weight = Convert.ToDouble(r.Item("Weight"))
			m_InventoryQty = Convert.ToInt32(r.Item("InventoryQty"))
			m_OriginalInventoryQty = m_InventoryQty
			If IsDBNull(r.Item("InventoryActionThreshold")) Then m_InventoryActionThreshold = Nothing Else m_InventoryActionThreshold = Convert.ToInt32(r.Item("InventoryActionThreshold"))
			If IsDBNull(r.Item("InventoryWarningThreshold")) Then m_InventoryWarningThreshold = Nothing Else m_InventoryWarningThreshold = Convert.ToInt32(r.Item("InventoryWarningThreshold"))
			m_InventoryAction = Convert.ToString(r.Item("InventoryAction"))
			If IsDBNull(r.Item("BackorderDate")) Then m_BackorderDate = Nothing Else m_BackorderDate = Convert.ToDateTime(r.Item("BackorderDate"))
            If IsDBNull(r.Item("Width")) Then m_Width = Nothing Else m_Width = Convert.ToDouble(r.Item("Width"))
            If IsDBNull(r.Item("Height")) Then m_Height = Nothing Else m_Height = Convert.ToDouble(r.Item("Height"))
            If IsDBNull(r.Item("Thickness")) Then m_Thickness = Nothing Else m_Thickness = Convert.ToDouble(r.Item("Thickness"))
            If IsDBNull(r.Item("ThumbnailWidth")) Then
                m_ThumbnailWidth = Nothing
            Else
                m_ThumbnailWidth = Convert.ToInt32(r.Item("ThumbnailWidth"))
            End If
            If IsDBNull(r.Item("ThumbnailHeight")) Then
                m_ThumbnailHeight = Nothing
            Else
                m_ThumbnailHeight = Convert.ToInt32(r.Item("ThumbnailHeight"))
            End If
            m_Price = Convert.ToDouble(r.Item("Price"))
            If IsDBNull(r.Item("ItemUnit")) Then m_ItemUnit = Nothing Else m_ItemUnit = Convert.ToString(r.Item("ItemUnit"))
            If IsDBNull(r.Item("CustomURL")) Then
                m_CustomURL = Nothing
                OriginalCustomURL = Nothing
            Else
                m_CustomURL = Convert.ToString(r.Item("CustomURL"))
                OriginalCustomURL = Convert.ToString(r.Item("CustomURL"))
            End If
            If IsDBNull(r.Item("SalePrice")) Then m_SalePrice = Nothing Else m_SalePrice = Convert.ToDouble(r.Item("SalePrice"))
            If IsDBNull(r.Item("Shipping1")) Then m_Shipping1 = Nothing Else m_Shipping1 = Convert.ToDouble(r.Item("Shipping1"))
            If IsDBNull(r.Item("Shipping2")) Then m_Shipping2 = Nothing Else m_Shipping2 = Convert.ToDouble(r.Item("Shipping2"))
            If IsDBNull(r.Item("CountryUnit")) Then m_CountryUnit = Nothing Else m_CountryUnit = Convert.ToInt32(r.Item("CountryUnit"))
            m_IsOnSale = Convert.ToBoolean(r.Item("IsOnSale"))
            m_IsTaxFree = Convert.ToBoolean(r.Item("IsTaxFree"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
			m_IsFeatured = Convert.ToBoolean(r.Item("IsFeatured"))
			m_IsGiftWrap = Convert.ToBoolean(r.Item("IsGiftWrap"))
			m_SendNotification = Convert.ToBoolean(r.Item("SendNotification"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            If IsDBNull(r.Item("ShortDescription")) Then m_ShortDescription = Nothing Else m_ShortDescription = Convert.ToString(r.Item("ShortDescription"))
            If IsDBNull(r.Item("LongDescription")) Then m_LongDescription = Nothing Else m_LongDescription = Convert.ToString(r.Item("LongDescription"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

			ValidateInventory()

			SQL = " INSERT INTO StoreItem (" _
			 & " ItemName" _
			 & ",DisplayMode" _
			 & ",SKU" _
			 & ",TemplateId" _
			 & ",BrandId" _
			 & ",PageTitle" _
			 & ",MetaDescription" _
			 & ",MetaKeywords" _
			 & ",Image" _
			 & ",Weight" _
			 & ",InventoryQty" _
			 & ",InventoryActionThreshold" _
			 & ",InventoryWarningThreshold" _
			 & ",InventoryAction" _
			 & ",BackorderDate" _
			 & ",Width" _
			 & ",Height" _
			 & ",Thickness" _
			 & ",ThumbnailWidth" _
			 & ",ThumbnailHeight" _
			 & ",Price" _
			 & ",ItemUnit" _
			 & ",CustomURL" _
			 & ",SalePrice" _
			 & ",Shipping1" _
			 & ",Shipping2" _
			 & ",CountryUnit" _
			 & ",IsOnSale" _
			 & ",IsTaxFree" _
			 & ",IsActive" _
			 & ",IsFeatured" _
			 & ",IsGiftWrap" _
			 & ",SendNotification" _
			 & ",CreateDate" _
			 & ",ModifyDate" _
			 & ",ShortDescription" _
			 & ",LongDescription" _
			 & ") VALUES (" _
			 & m_DB.Quote(ItemName) _
			 & "," & m_DB.Quote(DisplayMode) _
			 & "," & m_DB.Quote(SKU) _
			 & "," & m_DB.NullNumber(TemplateId) _
			 & "," & m_DB.NullNumber(BrandId) _
			 & "," & m_DB.Quote(PageTitle) _
			 & "," & m_DB.Quote(MetaDescription) _
			 & "," & m_DB.Quote(MetaKeywords) _
			 & "," & m_DB.Quote(Image) _
			 & "," & m_DB.Number(Weight) _
			 & "," & m_DB.Number(InventoryQty) _
			 & "," & m_DB.NullNumber(InventoryActionThreshold) _
			 & "," & m_DB.NullNumber(InventoryWarningThreshold) _
			 & "," & m_DB.Quote(InventoryAction) _
			 & "," & m_DB.NullQuote(BackorderDate) _
			 & "," & m_DB.Number(Width) _
			 & "," & m_DB.Number(Height) _
			 & "," & m_DB.Number(Thickness) _
			 & "," & m_DB.NullNumber(ThumbnailWidth) _
			 & "," & m_DB.NullNumber(ThumbnailHeight) _
			 & "," & m_DB.Number(Price) _
			 & "," & m_DB.Quote(ItemUnit) _
			 & "," & m_DB.Quote(CustomURL) _
			 & "," & m_DB.NullNumber(SalePrice) _
			 & "," & m_DB.NullNumber(Shipping1) _
			 & "," & m_DB.NullNumber(Shipping2) _
			 & "," & m_DB.NullNumber(CountryUnit) _
			 & "," & CInt(IsOnSale) _
			 & "," & CInt(IsTaxFree) _
			 & "," & CInt(IsActive) _
			 & "," & CInt(IsFeatured) _
			 & "," & CInt(IsGiftWrap) _
			 & "," & CInt(SendNotification) _
			 & "," & m_DB.NullQuote(Now) _
			 & "," & m_DB.NullQuote(Now) _
			 & "," & m_DB.Quote(ShortDescription) _
			 & "," & m_DB.Quote(LongDescription) _
			 & ")"

            ItemId = m_DB.InsertSQL(SQL)
            Return ItemId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

			ValidateInventory()

            If OriginalCustomURL <> String.Empty And OriginalCustomURL <> m_CustomURL Then
                If m_CustomURL = "" Then
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, "/store/item.aspx?itemid=" & m_ItemId.ToString)
                Else
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, m_CustomURL)
                End If
            End If
			SQL = " UPDATE StoreItem SET " _
			 & " ItemName = " & m_DB.Quote(ItemName) _
			 & ",DisplayMode = " & m_DB.Quote(DisplayMode) _
			 & ",SKU = " & m_DB.Quote(SKU) _
			 & ",TemplateId = " & m_DB.NullNumber(TemplateId) _
			 & ",BrandId = " & m_DB.NullNumber(BrandId) _
			 & ",PageTitle = " & m_DB.Quote(PageTitle) _
			 & ",MetaDescription = " & m_DB.Quote(MetaDescription) _
			 & ",MetaKeywords = " & m_DB.Quote(MetaKeywords) _
			 & ",Image = " & m_DB.Quote(Image) _
			 & ",Weight = " & m_DB.Number(Weight) _
			 & ",InventoryQty = " & m_DB.Number(InventoryQty) _
			 & ",InventoryActionThreshold = " & m_DB.NullNumber(InventoryActionThreshold) _
			 & ",InventoryWarningThreshold = " & m_DB.NullNumber(InventoryWarningThreshold) _
			 & ",InventoryAction = " & m_DB.Quote(InventoryAction) _
			 & ",BackorderDate = " & m_DB.NullQuote(BackorderDate) _
			 & ",Width = " & m_DB.Number(Width) _
			 & ",Height = " & m_DB.Number(Height) _
			 & ",Thickness = " & m_DB.Number(Thickness) _
			 & ",ThumbnailWidth = " & m_DB.NullNumber(ThumbnailWidth) _
			 & ",ThumbnailHeight = " & m_DB.NullNumber(ThumbnailHeight) _
			 & ",Price = " & m_DB.Number(Price) _
			 & ",ItemUnit = " & m_DB.Quote(ItemUnit) _
			 & ",CustomURL = " & m_DB.Quote(CustomURL) _
			 & ",SalePrice = " & m_DB.NullNumber(SalePrice) _
			 & ",Shipping1 = " & m_DB.NullNumber(Shipping1) _
			 & ",Shipping2 = " & m_DB.NullNumber(Shipping2) _
			 & ",CountryUnit = " & m_DB.NullNumber(CountryUnit) _
			 & ",IsOnSale = " & CInt(IsOnSale) _
			 & ",IsTaxFree = " & CInt(IsTaxFree) _
			 & ",IsActive = " & CInt(IsActive) _
			 & ",IsFeatured = " & CInt(IsFeatured) _
			 & ",IsGiftWrap = " & CInt(IsGiftWrap) _
			 & ",SendNotification = " & CInt(SendNotification) _
			 & ",ModifyDate = " & m_DB.NullQuote(Now) _
			 & ",ShortDescription = " & m_DB.Quote(ShortDescription) _
			 & ",LongDescription = " & m_DB.Quote(LongDescription) _
			 & " WHERE ItemId = " & m_DB.Quote(ItemId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

		Private Sub ValidateInventory()
			If Not SysParam.GetValue(DB, "EnableInventoryManagement") = 1 Then
				Exit Sub
			End If

			Dim SQL As String = "SELECT TOP 1 TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = " & TemplateId
			If InventoryAction = "Disable" AndAlso InventoryQty <= 0 AndAlso DB.ExecuteScalar(SQL) = Nothing Then
				IsActive = False
			End If

			If m_OriginalInventoryQty <= IIf(InventoryActionThreshold = Nothing, SysParam.GetValue(DB, "InventoryActionThreshold"), InventoryActionThreshold) AndAlso InventoryQty > InventoryActionThreshold Then
				SendNotification = True
			End If
		End Sub

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreItemAttributeTemp WHERE ItemId = " & m_DB.Quote(ItemId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM StoreItemAttribute WHERE ItemId = " & m_DB.Quote(ItemId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM StoreDepartmentItem WHERE ItemId = " & m_DB.Quote(ItemId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM StoreItemFeature WHERE ItemId = " & m_DB.Quote(ItemId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM StoreItemImage WHERE ItemId = " & m_DB.Quote(ItemId)
			m_DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM StorePromotionItem WHERE ItemId = " & m_DB.Quote(ItemId)
			m_DB.ExecuteSQL(SQL)

			SQL = "DELETE FROM MemberWishlistItem WHERE ItemId = " & m_DB.Quote(ItemId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM StoreItem WHERE ItemId = " & m_DB.Quote(ItemId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

	Public Class StoreItemCollection
		Inherits GenericCollection(Of StoreItemRow)
	End Class

    Public Class DepartmentFilterField
        Public BrandId As Integer
        Public DepartmentId As Integer
        Public SortBy As String
        Public SortOrder As String
        Public IsFeatured As Boolean = False
        Public IsNew As Boolean = False
        Public IsOnSale As Boolean = False
        Public pg As Integer
        Public MaxPerPage As Integer
		Public Keyword As String
		Public RawKeyword As String
        Public IncludeItemsFromSubdepartments As Boolean
	End Class

End Namespace

