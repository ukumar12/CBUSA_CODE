Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    ''' <summary>
    ''' Represents the field-by-field internal implementation of a row in the AssetPhotoGallery table in the database.
    ''' </summary>
    ''' <remarks>This class should be regenerated automatically whenever a database change occurs on AssetPhotoGallery.</remarks>
    Public MustInherit Class AssetPhotoGalleryRowBase
        Private m_DB As Database
        Private m_AssetPhotoGalleryId As Integer = Nothing
        Private m_PhotoGalleryId As Integer = Nothing
        Private m_AssetId As Integer = Nothing
        Private m_AssetFirstId As Integer = Nothing
        Private m_AssetDimensionId As Integer = Nothing
        Private m_PictureTitle As String = Nothing
        Private m_PictureCaption As String = Nothing
        Private m_PictureCredit As String = Nothing
        Private m_AltText As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_IsFeatured As Boolean = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing

        Private m_FeaturedAssetId As Integer = Nothing
        Private m_FeaturedAssetFirstId As Integer = Nothing
        Private m_FeaturedAssetDimensionId As Integer = Nothing

        Private m_ThumbAssetId As Integer = Nothing
        Private m_ThumbAssetFirstId As Integer = Nothing
        Private m_ThumbAssetDimensionId As Integer = Nothing


        ''' <summary>
        ''' Gets or sets the value of the AssetPhotoGalleryId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetPhotoGalleryId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetPhotoGalleryId() As Integer
            Get
                Return m_AssetPhotoGalleryId
            End Get
            Set(ByVal Value As Integer)
                m_AssetPhotoGalleryId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the PhotoGalleryId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on PhotoGalleryId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property PhotoGalleryId() As Integer
            Get
                Return m_PhotoGalleryId
            End Get
            Set(ByVal Value As Integer)
                m_PhotoGalleryId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetId() As Integer
            Get
                Return m_AssetId
            End Get
            Set(ByVal Value As Integer)
                m_AssetId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetDimensionId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetDimensionId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetDimensionId() As Integer
            Get
                Return m_AssetDimensionId
            End Get
            Set(ByVal value As Integer)
                m_AssetDimensionId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AssetFirstId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AssetFirstId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AssetFirstId() As Integer
            Get
                Return m_AssetFirstId
            End Get
            Set(ByVal value As Integer)
                m_AssetFirstId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the FeaturedAssetId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on FeaturedAssetId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property FeaturedAssetId() As Integer
            Get
                Return m_FeaturedAssetId
            End Get
            Set(ByVal Value As Integer)
                m_FeaturedAssetId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the FeaturedAssetDimensionId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on FeaturedAssetDimensionId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property FeaturedAssetDimensionId() As Integer
            Get
                Return m_FeaturedAssetDimensionId
            End Get
            Set(ByVal value As Integer)
                m_FeaturedAssetDimensionId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the FeaturedAssetFirstId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on FeaturedAssetFirstId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property FeaturedAssetFirstId() As Integer
            Get
                Return m_FeaturedAssetFirstId
            End Get
            Set(ByVal value As Integer)
                m_FeaturedAssetFirstId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ThumbAssetId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ThumbAssetId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ThumbAssetId() As Integer
            Get
                Return m_ThumbAssetId
            End Get
            Set(ByVal Value As Integer)
                m_ThumbAssetId = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ThumbAssetDimensionId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ThumbAssetDimensionId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ThumbAssetDimensionId() As Integer
            Get
                Return m_ThumbAssetDimensionId
            End Get
            Set(ByVal value As Integer)
                m_ThumbAssetDimensionId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the ThumbAssetFirstId field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ThumbAssetFirstId in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property ThumbAssetFirstId() As Integer
            Get
                Return m_ThumbAssetFirstId
            End Get
            Set(ByVal value As Integer)
                m_ThumbAssetFirstId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the PictureTitle field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on PictureTitle in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property PictureTitle() As String
            Get
                Return m_PictureTitle
            End Get
            Set(ByVal Value As String)
                m_PictureTitle = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the PictureCaption field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on PictureCaption in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property PictureCaption() As String
            Get
                Return m_PictureCaption
            End Get
            Set(ByVal Value As String)
                m_PictureCaption = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the PictureCredit field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on PictureCredit in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property PictureCredit() As String
            Get
                Return m_PictureCredit
            End Get
            Set(ByVal Value As String)
                m_PictureCredit = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the AltText field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="String" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on AltText in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property AltText() As String
            Get
                Return m_AltText
            End Get
            Set(ByVal Value As String)
                m_AltText = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the SortOrder field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Integer" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on SortOrder in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the IsFeatured field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on IsFeatured in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property IsFeatured() As Boolean
            Get
                Return m_IsFeatured
            End Get
            Set(ByVal Value As Boolean)
                m_IsFeatured = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the IsActive field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="Boolean" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on IsActive in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets the value of the CreateDate field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on CreateDate in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        ''' <summary>
        ''' Gets the value of the ModifyDate field in AssetPhotoGallery in the database.
        ''' </summary>
        ''' <value>A <see cref="DateTime" /> which contains the value of the field.</value>
        ''' <remarks>For more information about this field, view the entry on ModifyDate in AssetPhotoGallery in the
        ''' database using SQL Server Management Studio.</remarks>
        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
        End Property



        ''' <summary>
        ''' Gets or sets a reference to the database for this application.
        ''' </summary>
        ''' <value>A <see cref="Database" /> object for this application.</value>
        ''' <remarks>This property should be set before any SQL commands are executed using this class.</remarks>
        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        ''' <overloads>Initializes a new instance of the <see cref="AssetPhotoGalleryRowBase" /> class.</overloads>
        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetPhotoGalleryRowBase" /> class using default settings.
        ''' </summary>
        Public Sub New()
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetPhotoGalleryRowBase" /> class with the database connection
        ''' specified by <paramref name="DB" />.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRowBase.#ctor(Database,System.Int32)">New(Database, Integer)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        ''' <summary>
        ''' Initializes a new instance of the <see cref="AssetPhotoGalleryRowBase" /> class representing the row which uses
        ''' <paramref name="AssetPhotoGalleryId" /> as its primary key and <paramref name="DB" /> as its database connection.
        ''' </summary>
        ''' <param name="DB">A reference to the <see cref="Database" /> for the application.</param>
        ''' <param name="AssetPhotoGalleryId">The primary key value of the row being referenced.</param>
        ''' <remarks>If you don't use this constructor or 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRowBase.#ctor(Database)">New(Database)</see>,
        ''' be sure to set the <see cref="DB" /> property before executing any code which must connect to the
        ''' database.</remarks>
        Public Sub New(ByVal DB As Database, ByVal AssetPhotoGalleryId As Integer)
            m_DB = DB
            m_AssetPhotoGalleryId = AssetPhotoGalleryId
        End Sub 'New

        ''' <overloads>Loads the contents of a row from the database.</overloads>
        ''' <summary>
        ''' Loads the row from the database specified by <see cref="AssetPhotoGalleryId" />.
        ''' </summary>
        ''' <remarks>This method calls 
        ''' <see cref="M:DataLayer.AssetPhotoGalleryRowBase.Load(System.Data.SqlClient.SqlDataReader)">Load(SqlDataReader)</see>.</remarks>
        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String
            SQL = "SELECT * FROM AssetPhotoGallery WHERE AssetPhotoGalleryId = " & DB.Number(AssetPhotoGalleryId)

            r = m_DB.GetReader(SQL.ToString())
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        ''' <summary>
        ''' Loads the contents of the row in the <see cref="SqlDataReader" /> into the 
        ''' <see cref="AssetPhotoGalleryRowBase" />.
        ''' </summary>
        ''' <param name="r">A <see cref="SqlDataReader" /> currently set to the row which should be loaded.</param>
        ''' <remarks>It is vital that all of the fields are properly loaded inside this method.</remarks>
        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_AssetPhotoGalleryId = Core.GetInt(r.Item("AssetPhotoGalleryId"))
            m_PhotoGalleryId = Core.GetInt(r.Item("PhotoGalleryId"))
            m_AssetId = Core.GetInt(r.Item("AssetId"))
            m_AssetFirstId = Core.GetInt(r.Item("AssetFirstId"))
            m_AssetDimensionId = Core.GetInt(r.Item("AssetDimensionId"))

            m_FeaturedAssetId = Core.GetInt(r.Item("FeaturedAssetId"))
            m_FeaturedAssetFirstId = Core.GetInt(r.Item("FeaturedAssetFirstId"))
            m_FeaturedAssetDimensionId = Core.GetInt(r.Item("FeaturedAssetDimensionId"))

            m_ThumbAssetId = Core.GetInt(r.Item("ThumbAssetId"))
            m_ThumbAssetFirstId = Core.GetInt(r.Item("ThumbAssetFirstId"))
            m_ThumbAssetDimensionId = Core.GetInt(r.Item("ThumbAssetDimensionId"))

            m_PictureTitle = Core.GetString(r.Item("PictureTitle"))
            m_PictureCaption = Core.GetString(r.Item("PictureCaption"))
            m_PictureCredit = Core.GetString(r.Item("PictureCredit"))
            m_AltText = Core.GetString(r.Item("AltText"))
            m_IsFeatured = Core.GetBoolean(r.Item("IsFeatured"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))


        End Sub 'Load

        ''' <summary>
        ''' Inserts the row into AssetPhotoGallery using the values contained in the properties set on the 
        ''' <see cref="AssetPhotoGalleryRowBase" />.
        ''' </summary>
        ''' <returns>The value of AssetPhotoGalleryId for the new row.</returns>
        ''' <remarks>It is vital that all of the fields are properly inserted with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from AssetPhotoGallery order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO AssetPhotoGallery (" _
             & " PhotoGalleryId" _
             & ",AssetId" _
             & ",AssetDimensionId" _
             & ",AssetFirstId" _
             & ",FeaturedAssetId" _
             & ",FeaturedAssetDimensionId" _
             & ",FeaturedAssetFirstId" _
             & ",ThumbAssetId" _
             & ",ThumbAssetDimensionId" _
             & ",ThumbAssetFirstId" _
             & ",PictureTitle" _
             & ",PictureCaption" _
             & ",PictureCredit" _
             & ",AltText" _
             & ",SortOrder" _
             & ",IsFeatured" _
             & ",IsActive" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(PhotoGalleryId) _
             & "," & m_DB.NullNumber(AssetId) _
             & "," & m_DB.NullNumber(AssetDimensionId) _
             & "," & m_DB.Number(AssetFirstId) _
             & "," & m_DB.NullNumber(FeaturedAssetId) _
             & "," & m_DB.NullNumber(FeaturedAssetDimensionId) _
             & "," & m_DB.Number(FeaturedAssetFirstId) _
             & "," & m_DB.NullNumber(ThumbAssetId) _
             & "," & m_DB.NullNumber(ThumbAssetDimensionId) _
             & "," & m_DB.Number(ThumbAssetFirstId) _
             & "," & m_DB.Quote(PictureTitle) _
             & "," & m_DB.Quote(PictureCaption) _
             & "," & m_DB.Quote(PictureCredit) _
             & "," & m_DB.Quote(AltText) _
             & "," & MaxSortOrder _
             & "," & CInt(IsFeatured) _
             & "," & CInt(IsActive) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            AssetPhotoGalleryId = m_DB.InsertSQL(SQL)

            Return AssetPhotoGalleryId
        End Function

        ''' <summary>
        ''' Updates the row in AssetPhotoGallery using the values contained in the properties set on the 
        ''' <see cref="AssetPhotoGalleryRowBase" />.
        ''' </summary>
        ''' <remarks>It is vital that all of the fields are properly updated with their appropriate values 
        ''' inside this method.</remarks>
        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AssetPhotoGallery WITH (ROWLOCK) SET " _
             & " PhotoGalleryId = " & m_DB.NullNumber(PhotoGalleryId) _
             & ",AssetId = " & m_DB.NullNumber(AssetId) _
             & ",AssetDimensionId = " & m_DB.NullNumber(AssetDimensionId) _
             & ",AssetFirstId = " & m_DB.Number(AssetFirstId) _
             & ",FeaturedAssetId = " & m_DB.NullNumber(FeaturedAssetId) _
             & ",FeaturedAssetDimensionId = " & m_DB.NullNumber(FeaturedAssetDimensionId) _
             & ",FeaturedAssetFirstId = " & m_DB.Number(FeaturedAssetFirstId) _
             & ",ThumbAssetId = " & m_DB.NullNumber(ThumbAssetId) _
             & ",ThumbAssetDimensionId = " & m_DB.NullNumber(ThumbAssetDimensionId) _
             & ",ThumbAssetFirstId = " & m_DB.Number(ThumbAssetFirstId) _
             & ",PictureTitle = " & m_DB.Quote(PictureTitle) _
             & ",PictureCaption = " & m_DB.Quote(PictureCaption) _
             & ",PictureCredit = " & m_DB.Quote(PictureCredit) _
             & ",AltText = " & m_DB.Quote(AltText) _
             & ",IsFeatured = " & CInt(IsFeatured) _
             & ",IsActive = " & CInt(IsActive) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & " WHERE AssetPhotoGalleryId = " & m_DB.Quote(AssetPhotoGalleryId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        ''' <summary>
        ''' Removes this row from the specified <see cref="Database" />.
        ''' </summary>
        ''' <remarks>This method removes the row from the AssetPhotoGallery table.</remarks>
        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AssetPhotoGallery WHERE AssetPhotoGalleryId = " & m_DB.Number(AssetPhotoGalleryId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    ''' <summary>
    ''' Represents a strongly-typed list of <see cref="AssetPhotoGalleryRow">AssetPhotoGalleryRows</see> that can be accessed by index.
    ''' </summary>
    ''' <remarks>This class is a wrapper for 
    ''' <see cref="T:Components.GenericCollection`1">GenericCollection(Of AssetPhotoGalleryRow)</see>.</remarks>
    Public Class AssetPhotoGalleryCollection
        Inherits GenericCollection(Of AssetPhotoGalleryRow)
    End Class

End Namespace


