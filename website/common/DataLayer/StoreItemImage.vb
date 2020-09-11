Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreItemImageRow
        Inherits StoreItemImageRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ImageId As Integer)
            MyBase.New(DB, ImageId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ImageId As Integer) As StoreItemImageRow
            Dim row As StoreItemImageRow

            row = New StoreItemImageRow(DB, ImageId)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRowByItemIdAndImage(ByVal DB As Database, ByVal ItemId As Integer, ByVal Image As String) As StoreItemImageRow
            Dim row As New StoreItemImageRow(DB)
            Dim Sql As String = "SELECT * FROM StoreItemImage WHERE ItemId = " & DB.Number(ItemId) & " and Image = " & DB.Quote(Image)
            Dim r As SqlDataReader = DB.GetReader(Sql)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Function GetRowByItemAndType(ByVal DB As Database, ByVal ItemId As Integer, ByVal ImageType As String) As StoreItemImageRow
            Dim row As StoreItemImageRow
            Dim ImageId As Integer
            ImageId = DB.ExecuteScalar("SELECT ImageId FROM StoreItemImage WHERE ItemId=" & ItemId & " AND ImageType = " & DB.Quote(ImageType))
            row = New StoreItemImageRow(DB, ImageId)
            row.Load()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ImageId As Integer)
            Dim row As StoreItemImageRow

            row = New StoreItemImageRow(DB, ImageId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class StoreItemImageRowBase
        Private m_DB As Database
        Private m_ImageId As Integer = Nothing
        Private m_ItemId As Integer = Nothing
        Private m_Image As String = Nothing
        Private m_ImageAltTag As String = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property ImageId() As Integer
            Get
                Return m_ImageId
            End Get
            Set(ByVal Value As Integer)
                m_ImageId = Value
            End Set
        End Property

        Public Property ItemId() As Integer
            Get
                Return m_ItemId
            End Get
            Set(ByVal Value As Integer)
                m_ItemId = value
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

        Public Property ImageAltTag() As String
            Get
                Return m_ImageAltTag
            End Get
            Set(ByVal Value As String)
                m_ImageAltTag = value
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

        Public Sub New(ByVal DB As Database, ByVal ImageId As Integer)
            m_DB = DB
            m_ImageId = ImageId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreItemImage WHERE ImageId = " & DB.Number(ImageId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ImageId = Convert.ToInt32(r.Item("ImageId"))
            If IsDBNull(r.Item("ItemId")) Then
                m_ItemId = Nothing
            Else
                m_ItemId = Convert.ToInt32(r.Item("ItemId"))
            End If
            If IsDBNull(r.Item("Image")) Then
                m_Image = Nothing
            Else
                m_Image = Convert.ToString(r.Item("Image"))
            End If
            If IsDBNull(r.Item("ImageAltTag")) Then
                m_ImageAltTag = Nothing
            Else
                m_ImageAltTag = Convert.ToString(r.Item("ImageAltTag"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from StoreItemImage order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO StoreItemImage (" _
             & " ItemId" _
             & ",Image" _
             & ",ImageAltTag" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.Number(ItemId) _
             & "," & m_DB.Quote(Image) _
             & "," & m_DB.Quote(ImageAltTag) _
             & "," & MaxSortOrder _
             & ")"

            ImageId = m_DB.InsertSQL(SQL)

            Return ImageId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreItemImage SET " _
             & " ItemId = " & m_DB.Number(ItemId) _
             & ",Image = " & m_DB.Quote(Image) _
             & ",ImageAltTag = " & m_DB.Quote(ImageAltTag) _
             & " WHERE ImageId = " & m_DB.Quote(ImageId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreItemImage WHERE ImageId = " & m_DB.Quote(ImageId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreItemImageCollection
        Inherits GenericCollection(Of StoreItemImageRow)
    End Class
End Namespace


