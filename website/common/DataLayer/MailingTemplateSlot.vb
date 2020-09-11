Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MailingTemplateSlotRow
        Inherits MailingTemplateSlotRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal SlotId As Integer)
            MyBase.New(database, SlotId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal SlotId As Integer) As MailingTemplateSlotRow
            Dim row As MailingTemplateSlotRow

            row = New MailingTemplateSlotRow(_Database, SlotId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal SlotId As Integer)
            Dim row As MailingTemplateSlotRow

            row = New MailingTemplateSlotRow(_Database, SlotId)
            row.Remove()
        End Sub

        'Custom Methods
    End Class

    Public MustInherit Class MailingTemplateSlotRowBase
        Private m_DB As Database
        Private m_SlotId As Integer = Nothing
        Private m_TemplateId As Integer = Nothing
        Private m_SlotName As String = Nothing
        Private m_ImageName As String = Nothing

        Public Property SlotId() As Integer
            Get
                Return m_SlotId
            End Get
            Set(ByVal Value As Integer)
                m_SlotId = value
            End Set
        End Property

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = value
            End Set
        End Property

        Public Property SlotName() As String
            Get
                Return m_SlotName
            End Get
            Set(ByVal Value As String)
                m_SlotName = value
            End Set
        End Property

        Public Property ImageName() As String
            Get
                Return m_ImageName
            End Get
            Set(ByVal Value As String)
                m_ImageName = value
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal SlotId As Integer)
            m_DB = database
            m_SlotId = SlotId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MailingTemplateSlot WHERE SlotId = " & DB.Quote(SlotId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Public Overridable Sub Load(ByVal r As SqlDataReader)
            m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            m_SlotName = Convert.ToString(r.Item("SlotName"))
            m_ImageName = Convert.ToString(r.Item("ImageName"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String
            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from MailingTemplateSlot where TemplateId = " & TemplateId & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO MailingTemplateSlot (" _
             & " TemplateId" _
             & ",SlotName" _
             & ",ImageName" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.Quote(TemplateId) _
             & "," & m_DB.Quote(SlotName) _
             & "," & m_DB.Quote(ImageName) _
             & "," & m_DB.Quote(MaxSortOrder) _
             & ")"

            SlotId = m_DB.InsertSQL(SQL)

            Return SlotId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingTemplateSlot SET " _
             & " TemplateId = " & m_DB.Quote(TemplateId) _
             & ",SlotName = " & m_DB.Quote(SlotName) _
             & ",ImageName = " & m_DB.Quote(ImageName) _
             & " WHERE SlotId = " & m_DB.Quote(SlotId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingTemplateSlot WHERE SlotId = " & m_DB.Quote(SlotId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingTemplateSlotCollection
        Inherits GenericCollection(Of MailingTemplateSlotRow)
    End Class

End Namespace

