Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MailingTemplateRow
        Inherits MailingTemplateRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal TemplateId As Integer)
            MyBase.New(database, TemplateId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal TemplateId As Integer) As MailingTemplateRow
            Dim row As MailingTemplateRow

            row = New MailingTemplateRow(_Database, TemplateId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal TemplateId As Integer)
            Dim row As MailingTemplateRow

            row = New MailingTemplateRow(_Database, TemplateId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetAllTemplates(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from MailingTemplate order by Name")
            Return dt
        End Function

        Public Function GetSlots() As MailingTemplateSlotCollection
            Dim slots As New MailingTemplateSlotCollection
            Dim dr As SqlDataReader = DB.GetReader("select * from MailingTemplateSlot where TemplateId = " & TemplateId & " order by SortOrder")
            While dr.Read
                Dim slot As New MailingTemplateSlotRow(DB)
                slot.Load(dr)
                slots.Add(slot)
            End While
            dr.Close()

            Return slots
        End Function

    End Class

    Public MustInherit Class MailingTemplateRowBase
        Private m_DB As Database
        Private m_TemplateId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_ImageName As String = Nothing
        Private m_HTMLMember As String = Nothing
        Private m_TextMember As String = Nothing
        Private m_HTMLDynamic As String = Nothing
        Private m_TextDynamic As String = Nothing


        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = value
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

        Public Property ImageName() As String
            Get
                Return m_ImageName
            End Get
            Set(ByVal Value As String)
                m_ImageName = value
            End Set
        End Property

        Public Property HTMLMember() As String
            Get
                Return m_HTMLMember
            End Get
            Set(ByVal Value As String)
                m_HTMLMember = value
            End Set
        End Property

        Public Property TextMember() As String
            Get
                Return m_TextMember
            End Get
            Set(ByVal Value As String)
                m_TextMember = value
            End Set
        End Property

        Public Property HTMLDynamic() As String
            Get
                Return m_HTMLDynamic
            End Get
            Set(ByVal Value As String)
                m_HTMLDynamic = value
            End Set
        End Property

        Public Property TextDynamic() As String
            Get
                Return m_TextDynamic
            End Get
            Set(ByVal Value As String)
                m_TextDynamic = value
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

        Public Sub New(ByVal database As Database, ByVal TemplateId As Integer)
            m_DB = database
            m_TemplateId = TemplateId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MailingTemplate WHERE TemplateId = " & DB.Quote(TemplateId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_Name = Convert.ToString(r.Item("Name"))
            m_ImageName = Convert.ToString(r.Item("ImageName"))
            m_HTMLMember = Convert.ToString(r.Item("HTMLMember"))
            If IsDBNull(r.Item("TextMember")) Then
                m_TextMember = Nothing
            Else
                m_TextMember = Convert.ToString(r.Item("TextMember"))
            End If
            If IsDBNull(r.Item("HTMLDynamic")) Then
                m_HTMLDynamic = Nothing
            Else
                m_HTMLDynamic = Convert.ToString(r.Item("HTMLDynamic"))
            End If
            If IsDBNull(r.Item("TextDynamic")) Then
                m_TextDynamic = Nothing
            Else
                m_TextDynamic = Convert.ToString(r.Item("TextDynamic"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO MailingTemplate (" _
             & " Name" _
             & ",ImageName" _
             & ",HTMLMember" _
             & ",TextMember" _
             & ",HTMLDynamic" _
             & ",TextDynamic" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(ImageName) _
             & "," & m_DB.Quote(HTMLMember) _
             & "," & m_DB.Quote(TextMember) _
             & "," & m_DB.Quote(HTMLDynamic) _
             & "," & m_DB.Quote(TextDynamic) _
             & ")"

            TemplateId = m_DB.InsertSQL(SQL)

            Return TemplateId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingTemplate SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",ImageName = " & m_DB.Quote(ImageName) _
             & ",HTMLMember = " & m_DB.Quote(HTMLMember) _
             & ",TextMember = " & m_DB.Quote(TextMember) _
             & ",HTMLDynamic = " & m_DB.Quote(HTMLDynamic) _
             & ",TextDynamic = " & m_DB.Quote(TextDynamic) _
             & " WHERE TemplateId = " & m_DB.quote(TemplateId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingTemplate WHERE TemplateId = " & m_DB.Quote(TemplateId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingTemplateCollection
        Inherits GenericCollection(Of MailingTemplateRow)
    End Class

End Namespace


