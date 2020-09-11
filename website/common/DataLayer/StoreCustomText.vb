Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreCustomTextRow
        Inherits StoreCustomTextRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TextId As Integer)
            MyBase.New(DB, TextId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TextId As Integer) As StoreCustomTextRow
            Dim row As StoreCustomTextRow

            row = New StoreCustomTextRow(DB, TextId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TextId As Integer)
            Dim row As StoreCustomTextRow

            row = New StoreCustomTextRow(DB, TextId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetRowByCode(ByVal DB As Database, ByVal Code As String) As StoreCustomTextRow
            Dim SQL As String = "SELECT * FROM StoreCustomText WHERE Code = " & DB.Quote(Code)
            Dim r As SqlDataReader
            Dim row As StoreCustomTextRow = New StoreCustomTextRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

    End Class

    Public MustInherit Class StoreCustomTextRowBase
        Private m_DB As Database
        Private m_TextId As Integer = Nothing
        Private m_Code As String = Nothing
        Private m_Title As String = Nothing
        Private m_Value As String = Nothing
        Private m_IsHelpTag As Boolean = Nothing



        Public Property TextId() As Integer
            Get
                Return m_TextId
            End Get
            Set(ByVal Value As Integer)
                m_TextId = value
            End Set
        End Property

        Public Property Code() As String
            Get
                Return m_Code
            End Get
            Set(ByVal Value As String)
                m_Code = value
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

        Public Property Value() As String
            Get
                Return m_Value
            End Get
            Set(ByVal Value As String)
                m_Value = value
            End Set
        End Property

        Public Property IsHelpTag() As Boolean
            Get
                Return m_IsHelpTag
            End Get
            Set(ByVal Value As Boolean)
                m_IsHelpTag = Value
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

        Public Sub New(ByVal DB As Database, ByVal TextId As Integer)
            m_DB = DB
            m_TextId = TextId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreCustomText WHERE TextId = " & DB.Number(TextId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TextId = Convert.ToInt32(r.Item("TextId"))
            m_Code = Convert.ToString(r.Item("Code"))
            m_Title = Convert.ToString(r.Item("Title"))
            If IsDBNull(r.Item("Value")) Then
                m_Value = Nothing
            Else
                m_Value = Convert.ToString(r.Item("Value"))
            End If
            m_IsHelpTag = Convert.ToBoolean(r.Item("IsHelpTag"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreCustomText (" _
             & " Code" _
             & ",Title" _
             & ",Value" _
             & ",IsHelpTag" _
             & ") VALUES (" _
             & m_DB.Quote(Code) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.Quote(Value) _
             & "," & CInt(IsHelpTag) _
             & ")"

            TextId = m_DB.InsertSQL(SQL)

            Return TextId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreCustomText SET " _
             & " Code = " & m_DB.Quote(Code) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",Value = " & m_DB.Quote(Value) _
             & ",IsHelpTag = " & CInt(IsHelpTag) _
             & " WHERE TextId = " & m_DB.Quote(TextId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreCustomText WHERE TextId = " & m_DB.Quote(TextId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreCustomTextCollection
        Inherits GenericCollection(Of StoreCustomTextRow)
    End Class

End Namespace


