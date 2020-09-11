Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class VendorPhotoRowBase
        Private m_DB As Database
        Private m_PhotoId As Integer = Nothing
        Private m_VendorId As Integer = Nothing
        Private m_Photo As String = Nothing
        Private m_Caption As String = Nothing
        Private m_AltText As String = Nothing

        Public Property PhotoId() As Integer
            Get
                Return m_PhotoId
            End Get
            Set(ByVal Value As Integer)
                m_PhotoId = value
            End Set
        End Property

        Public Property VendorId() As Integer
            Get
                Return m_VendorId
            End Get
            Set(ByVal Value As Integer)
                m_VendorId = value
            End Set
        End Property

        Public Property Photo() As String
            Get
                Return m_Photo
            End Get
            Set(ByVal Value As String)
                m_Photo = value
            End Set
        End Property

        Public Property Caption() As String
            Get
                Return m_Caption
            End Get
            Set(ByVal Value As String)
                m_Caption = value
            End Set
        End Property

        Public Property AltText() As String
            Get
                Return m_AltText
            End Get
            Set(ByVal Value As String)
                m_AltText = value
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

        Public Sub New(ByVal DB As Database, ByVal PhotoId As Integer)
            m_DB = DB
            m_PhotoId = PhotoId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorPhoto WHERE PhotoId = " & DB.Number(PhotoId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_PhotoId = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PhotoId = Core.GetInt(r.Item("PhotoId"))
            m_VendorId = Core.GetInt(r.Item("VendorId"))
            m_Photo = Core.GetString(r.Item("Photo"))
            m_Caption = Core.GetString(r.Item("Caption"))
            m_AltText = Core.GetString(r.Item("AltText"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorPhoto (" _
             & " VendorId" _
             & ",Photo" _
             & ",Caption" _
             & ",AltText" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorId) _
             & "," & m_DB.Quote(Photo) _
             & "," & m_DB.Quote(Caption) _
             & "," & m_DB.Quote(AltText) _
             & ")"

            PhotoId = m_DB.InsertSQL(SQL)

            Return PhotoId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorPhoto WITH (ROWLOCK) SET " _
             & " VendorId = " & m_DB.NullNumber(VendorId) _
             & ",Photo = " & m_DB.Quote(Photo) _
             & ",Caption = " & m_DB.Quote(Caption) _
             & ",AltText = " & m_DB.Quote(AltText) _
             & " WHERE PhotoId = " & m_DB.quote(PhotoId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class VendorPhotoCollection
        Inherits GenericCollection(Of VendorPhotoRow)
    End Class

End Namespace


