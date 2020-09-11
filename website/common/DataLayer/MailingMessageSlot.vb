Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    <Serializable()> _
    Public Class MailingMessageSlot
        Public Headline As String
        Public MessageId As Integer
        Public Slot As String
        Public SortOrder As Integer
    End Class

    Public Class MailingMessageSlotRow
        Inherits MailingMessageSlotRowBase

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
        Public Shared Function GetRow(ByVal _Database As Database, ByVal SlotId As Integer) As MailingMessageSlotRow
            Dim row As MailingMessageSlotRow

            row = New MailingMessageSlotRow(_Database, SlotId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal SlotId As Integer)
            Dim row As MailingMessageSlotRow

            row = New MailingMessageSlotRow(_Database, SlotId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Sub RemoveByMessage(ByVal DB As Database, ByVal MessageId As Integer)
            Dim SQL As String = "DELETE FROM MailingMessageSlot WHERE MessageId = " & MessageId
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Function GetSlotsByMessage(ByVal db As Database, ByVal Templateid As Integer, ByVal MessageId As Integer) As DataTable
            Dim SQL As String = String.Empty
            If MessageId = 0 Then
                SQL = "SELECT mts.SlotId, mts.TemplateId, mts.SlotName, mts.ImageName, mts.SortOrder, null as Headline, null as Slot FROM MailingTemplateSlot AS mts WHERE mts.TemplateId = " & Templateid
            Else
                SQL = "SELECT mts.SlotId, mts.TemplateId, mts.SlotName, mts.ImageName, mts.SortOrder, mms.Headline, mms.Slot FROM MailingTemplateSlot AS mts LEFT OUTER JOIN MailingMessageSlot AS mms ON mts.SortOrder = mms.SortOrder WHERE mts.TemplateId = " & Templateid & " AND mms.MessageId = " & MessageId
            End If
            Return db.GetDataTable(SQL)
        End Function

    End Class

    Public MustInherit Class MailingMessageSlotRowBase
        Private m_DB As Database
        Private m_SlotId As Integer = Nothing
        Private m_MessageId As Integer = Nothing
        Private m_Headline As String = Nothing
        Private m_Slot As String = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property SlotId() As Integer
            Get
                Return m_SlotId
            End Get
            Set(ByVal Value As Integer)
                m_SlotId = Value
            End Set
        End Property

        Public Property MessageId() As Integer
            Get
                Return m_MessageId
            End Get
            Set(ByVal Value As Integer)
                m_MessageId = Value
            End Set
        End Property

        Public Property Headline() As String
            Get
                Return m_Headline
            End Get
            Set(ByVal Value As String)
                m_Headline = Value
            End Set
        End Property

        Public Property Slot() As String
            Get
                Return m_Slot
            End Get
            Set(ByVal Value As String)
                m_Slot = Value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = Value
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

            SQL = "SELECT * FROM MailingMessageSlot WHERE SlotId = " & DB.Quote(SlotId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Public Overridable Sub Load(ByVal r As SqlDataReader)
            m_SlotId = Convert.ToInt32(r.Item("SlotId"))
            m_MessageId = Convert.ToInt32(r.Item("MessageId"))
            If IsDBNull(r.Item("Headline")) Then
                m_Headline = Nothing
            Else
                m_Headline = Convert.ToString(r.Item("Headline"))
            End If
            If IsDBNull(r.Item("Slot")) Then
                m_Slot = Nothing
            Else
                m_Slot = Convert.ToString(r.Item("Slot"))
            End If
            m_SortOrder = Convert.ToInt32(r.Item("SortOrder"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO MailingMessageSlot (" _
             & " MessageId" _
             & ",Headline" _
             & ",Slot" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.Quote(MessageId) _
             & "," & m_DB.Quote(Headline) _
             & "," & m_DB.Quote(Slot) _
             & "," & m_DB.Quote(SortOrder) _
             & ")"

            SlotId = m_DB.InsertSQL(SQL)

            Return SlotId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingMessageSlot SET " _
             & " MessageId = " & m_DB.Quote(MessageId) _
             & ",Headline = " & m_DB.Quote(Headline) _
             & ",Slot = " & m_DB.Quote(Slot) _
             & ",SortOrder = " & m_DB.Quote(SortOrder) _
             & " WHERE SlotId = " & m_DB.Quote(SlotId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingMessageSlot WHERE SlotId = " & m_DB.Quote(SlotId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingMessageSlotCollection
        Inherits GenericCollection(Of MailingMessageSlotRow)
    End Class

End Namespace


