Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports System.Web

Namespace DataLayer

    Public Class StoreBrandRow
        Inherits StoreBrandRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BrandId As Integer)
            MyBase.New(DB, BrandId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BrandId As Integer) As StoreBrandRow
            Dim row As StoreBrandRow

            row = New StoreBrandRow(DB, BrandId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BrandId As Integer)
            Dim row As StoreBrandRow

            row = New StoreBrandRow(DB, BrandId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetRowByCustomURL(ByVal DB As Database, ByVal Url As String) As StoreBrandRow
            Dim SQL As String = "SELECT * FROM StoreBrand WHERE CustomURL = " & DB.Quote(Url)
            Dim r As SqlDataReader
            Dim row As StoreBrandRow = New StoreBrandRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Function GetBrands(ByVal DB As Database) As DataTable
            Dim dt As DataTable = CType(HttpContext.Current.Cache("StoreBrandDataTable"), DataTable)
            If HttpContext.Current Is Nothing Then
                dt = DB.GetDataTable("SELECT * FROM StoreBrand ORDER BY SortOrder ASC")
            Else
                dt = CType(HttpContext.Current.Cache("StoreBrandDataTable"), DataTable)
                If dt Is Nothing Then
                    dt = DB.GetDataTable("SELECT * FROM StoreBrand ORDER BY SortOrder ASC")
                    HttpContext.Current.Cache.Insert("StoreBrandDataTable", dt, Nothing, DateAdd(DateInterval.Second, 15, Now), Nothing)
                End If
            End If
            Return dt
        End Function

        Public Shared Function GetActiveBrands(ByVal DB As Database) As DataTable
            Dim SQL As String = "select distinct sb.Name, sb.Image, sb.Logo, sb.ThumbnailWidth, sb.ThumbnailHeight, sb.BrandId, sb.SortOrder, sb.CustomURL from StoreBrand sb inner join StoreItem si on sb.BrandId = si.BrandId inner join StoreDepartmentItem sdi on si.ItemId = sdi.ItemId inner join StoreDepartment sd on sdi.DepartmentId = sd.DepartmentId where si.IsActive = 1 and sd.IsInActive = 0 and sb.IsActive = 1 order by SortOrder"
            Return DB.GetDataTable(SQL)
        End Function

		Public Shared Function DisplayBreadCrumb(ByVal DB As Database, ByVal BrandId As Integer, ByVal IsLastLink As Boolean, Optional ByVal ItemName As String = Nothing)
			Dim Result As String = String.Empty
			Dim dbBrand As StoreBrandRow = StoreBrandRow.GetRow(DB, BrandId)

			If Not ItemName = Nothing Then
				'if the itemname exists then override the IsLastLink to True
				IsLastLink = True
			End If

			Result &= "<a href=""/"">Home</a> &gt;"
			Result &= "<a href=""/store/"">Store</a> &gt;"

			If dbBrand.BrandId > 0 Then
				Result &= "<a href=""/store/brand.aspx"">Brands</a>"
				If IsLastLink Then
                    Result &= " &gt; <a href=""" & IIf(dbBrand.CustomURL = String.Empty, "/store/brand.aspx?BrandId=" & BrandId, dbBrand.CustomURL) & """>" & dbBrand.Name & "</a>"
				Else
					Result &= " &gt; <span>" & dbBrand.Name & "</span>"
				End If
			Else
				Result &= "<span>Brands</span>"
			End If
			If IsLastLink AndAlso Not ItemName = Nothing Then Result &= " &gt; " & ItemName
			Return Result
		End Function
    End Class

    Public MustInherit Class StoreBrandRowBase
        Private m_DB As Database
        Private m_BrandId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Image As String = Nothing
        Private m_Logo As String = Nothing
        Private m_ThumbnailWidth As Integer = Nothing
        Private m_ThumbnailHeight As Integer = Nothing
        Private m_PageTitle As String = Nothing
        Private m_MetaDescription As String = Nothing
        Private m_MetaKeywords As String = Nothing
        Private m_CustomURL As String = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_DateModified As DateTime = Nothing
        Private m_Description As String = Nothing

        Private OriginalCustomURL As String = Nothing 'Stores the original value of customurl from the database


        Public Property BrandId() As Integer
            Get
                Return m_BrandId
            End Get
            Set(ByVal Value As Integer)
                m_BrandId = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property Image() As String
            Get
                Return m_Image
            End Get
            Set(ByVal Value As String)
                m_Image = value
            End Set
        End Property

        Public Property Logo() As String
            Get
                Return m_Logo
            End Get
            Set(ByVal Value As String)
                m_Logo = value
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

        Public Property PageTitle() As String
            Get
                Return m_PageTitle
            End Get
            Set(ByVal Value As String)
                m_PageTitle = value
            End Set
        End Property

        Public Property MetaDescription() As String
            Get
                Return m_MetaDescription
            End Get
            Set(ByVal Value As String)
                m_MetaDescription = value
            End Set
        End Property

        Public Property MetaKeywords() As String
            Get
                Return m_MetaKeywords
            End Get
            Set(ByVal Value As String)
                m_MetaKeywords = value
            End Set
        End Property

        Public Property CustomURL() As String
            Get
                Return m_CustomURL
            End Get
            Set(ByVal Value As String)
                m_CustomURL = value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
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

        Public Property DateModified() As DateTime
            Get
                Return m_DateModified
            End Get
            Set(ByVal Value As DateTime)
                m_DateModified = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
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

        Public Sub New(ByVal DB As Database, ByVal BrandId As Integer)
            m_DB = DB
            m_BrandId = BrandId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreBrand WHERE BrandId = " & DB.Number(BrandId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BrandId = Convert.ToInt32(r.Item("BrandId"))
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("Image")) Then
                m_Image = Nothing
            Else
                m_Image = Convert.ToString(r.Item("Image"))
            End If
            If IsDBNull(r.Item("Logo")) Then
                m_Logo = Nothing
            Else
                m_Logo = Convert.ToString(r.Item("Logo"))
            End If
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
            If IsDBNull(r.Item("PageTitle")) Then
                m_PageTitle = Nothing
            Else
                m_PageTitle = Convert.ToString(r.Item("PageTitle"))
            End If
            If IsDBNull(r.Item("MetaDescription")) Then
                m_MetaDescription = Nothing
            Else
                m_MetaDescription = Convert.ToString(r.Item("MetaDescription"))
            End If
            If IsDBNull(r.Item("MetaKeywords")) Then
                m_MetaKeywords = Nothing
            Else
                m_MetaKeywords = Convert.ToString(r.Item("MetaKeywords"))
            End If
            If IsDBNull(r.Item("CustomURL")) Then
                m_CustomURL = Nothing
                OriginalCustomURL = Nothing
            Else
                m_CustomURL = Convert.ToString(r.Item("CustomURL"))
                OriginalCustomURL = Convert.ToString(r.Item("CustomURL"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(r.Item("DateModified")) Then
                m_DateModified = Nothing
            Else
                m_DateModified = Convert.ToDateTime(r.Item("DateModified"))
            End If
            If IsDBNull(r.Item("Description")) Then
                m_Description = Nothing
            Else
                m_Description = Convert.ToString(r.Item("Description"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from StoreBrand order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO StoreBrand (" _
             & " Name" _
             & ",Image" _
             & ",Logo" _
             & ",ThumbnailWidth" _
             & ",ThumbnailHeight" _
             & ",PageTitle" _
             & ",MetaDescription" _
             & ",MetaKeywords" _
             & ",CustomURL" _
             & ",IsActive" _
             & ",SortOrder" _
             & ",CreateDate" _
             & ",DateModified" _
             & ",Description" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Image) _
             & "," & m_DB.Quote(Logo) _
             & "," & m_DB.NullNumber(ThumbnailWidth) _
             & "," & m_DB.NullNumber(ThumbnailHeight) _
             & "," & m_DB.Quote(PageTitle) _
             & "," & m_DB.Quote(MetaDescription) _
             & "," & m_DB.Quote(MetaKeywords) _
             & "," & m_DB.Quote(CustomURL) _
             & "," & CInt(IsActive) _
             & "," & MaxSortOrder _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(DateModified) _
             & "," & m_DB.Quote(Description) _
             & ")"

            BrandId = m_DB.InsertSQL(SQL)

            Return BrandId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            If OriginalCustomURL <> String.Empty And OriginalCustomURL <> m_CustomURL Then
                If m_CustomURL = "" Then
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, "/store/brand.aspx?brandid=" & m_BrandId)
                Else
                    CustomURLHistoryRow.AddToHistory(DB, OriginalCustomURL, m_CustomURL)
                End If
            End If

            SQL = " UPDATE StoreBrand SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Image = " & m_DB.Quote(Image) _
             & ",Logo = " & m_DB.Quote(Logo) _
             & ",ThumbnailWidth = " & m_DB.NullNumber(ThumbnailWidth) _
             & ",ThumbnailHeight = " & m_DB.NullNumber(ThumbnailHeight) _
             & ",PageTitle = " & m_DB.Quote(PageTitle) _
             & ",MetaDescription = " & m_DB.Quote(MetaDescription) _
             & ",MetaKeywords = " & m_DB.Quote(MetaKeywords) _
             & ",CustomURL = " & m_DB.Quote(CustomURL) _
             & ",IsActive = " & CInt(IsActive) _
             & ",DateModified = " & m_DB.NullQuote(DateModified) _
             & ",Description = " & m_DB.Quote(Description) _
             & " WHERE BrandId = " & m_DB.Quote(BrandId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreBrand WHERE BrandId = " & m_DB.Quote(BrandId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreBrandCollection
        Inherits GenericCollection(Of StoreBrandRow)
    End Class

End Namespace


