Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreItemTemplateRow
        Inherits StoreItemTemplateRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TemplateId As Integer)
            MyBase.New(DB, TemplateId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TemplateId As Integer) As StoreItemTemplateRow
            Dim row As StoreItemTemplateRow

            row = New StoreItemTemplateRow(DB, TemplateId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TemplateId As Integer)
            Dim row As StoreItemTemplateRow

            row = New StoreItemTemplateRow(DB, TemplateId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetTemplates(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("SELECT * FROM StoreItemTemplate ORDER BY TemplateName")
        End Function

		Public Shared Function GetValidTemplates(ByVal DB As Database) As DataTable
			Return DB.GetDataTable("SELECT * FROM (SELECT *, CASE WHEN IsAttributes = 1 THEN COALESCE((SELECT TOP 1 TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = StoreItemTemplate.TemplateId),0) ELSE 1 END AS IsValid FROM StoreItemTemplate) AS tmp WHERE IsValid > 0 ORDER BY TemplateName")
		End Function

        Public Shared Function GetTemplateAttributes(ByVal DB As Database, ByVal TemplateId As Integer) As DataTable
            Return DB.GetDataTable("SELECT * FROM StoreItemTemplateAttribute WHERE TemplateId=" & TemplateId & " ORDER BY SortOrder ASC")
        End Function
    End Class

    Public MustInherit Class StoreItemTemplateRowBase
        Private m_DB As Database
        Private m_TemplateId As Integer = Nothing
        Private m_TemplateName As String = Nothing
        Private m_IsAttributes As Boolean = Nothing
		Private m_DisplayMode As String = Nothing
        Private m_IsToAndFrom As Boolean = Nothing
        Private m_IsGiftMessage As Boolean = Nothing


        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = Value
            End Set
        End Property

        Public Property TemplateName() As String
            Get
                Return m_TemplateName
            End Get
            Set(ByVal Value As String)
                m_TemplateName = Value
            End Set
        End Property

        Public Property IsAttributes() As Boolean
            Get
                Return m_IsAttributes
            End Get
            Set(ByVal Value As Boolean)
                m_IsAttributes = Value
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

        Public Property IsToAndFrom() As Boolean
            Get
                Return m_IsToAndFrom
            End Get
            Set(ByVal Value As Boolean)
                m_IsToAndFrom = Value
            End Set
        End Property

        Public Property IsGiftMessage() As Boolean
            Get
                Return m_IsGiftMessage
            End Get
            Set(ByVal Value As Boolean)
                m_IsGiftMessage = Value
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

        Public Sub New(ByVal DB As Database, ByVal TemplateId As Integer)
            m_DB = DB
            m_TemplateId = TemplateId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreItemTemplate WHERE TemplateId = " & DB.Number(TemplateId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            m_TemplateName = Convert.ToString(r.Item("TemplateName"))
            m_IsAttributes = Convert.ToBoolean(r.Item("IsAttributes"))
			m_DisplayMode = Convert.ToString(r.Item("DisplayMode"))
            m_IsToAndFrom = Convert.ToBoolean(r.Item("IsToAndFrom"))
            m_IsGiftMessage = Convert.ToBoolean(r.Item("IsGiftMessage"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreItemTemplate (" _
             & " TemplateName" _
			 & ",DisplayMode" _
             & ",IsAttributes" _
             & ",IsToAndFrom" _
             & ",IsGiftMessage" _
             & ") VALUES (" _
             & m_DB.Quote(TemplateName) _
			 & "," & m_DB.Quote(DisplayMode) _
             & "," & CInt(IsAttributes) _
             & "," & CInt(IsToAndFrom) _
             & "," & CInt(IsGiftMessage) _
             & ")"

            TemplateId = m_DB.InsertSQL(SQL)

            Return TemplateId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreItemTemplate SET " _
             & " TemplateName = " & m_DB.Quote(TemplateName) _
			 & ",DisplayMode = " & m_DB.Quote(DisplayMode) _
             & ",IsAttributes = " & CInt(IsAttributes) _
             & ",IsToAndFrom = " & CInt(IsToAndFrom) _
             & ",IsGiftMessage = " & CInt(IsGiftMessage) _
             & " WHERE TemplateId = " & m_DB.Quote(TemplateId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreItemTemplate WHERE TemplateId = " & m_DB.Quote(TemplateId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreItemTemplateCollection
        Inherits GenericCollection(Of StoreItemTemplateRow)
    End Class

End Namespace

