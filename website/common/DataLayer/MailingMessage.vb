Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MailingMessageRow
        Inherits MailingMessageRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal database As Database)
            MyBase.New(database)
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal MessageId As Integer)
            MyBase.New(database, MessageId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal _Database As Database, ByVal MessageId As Integer) As MailingMessageRow
            Dim row As MailingMessageRow

            row = New MailingMessageRow(_Database, MessageId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal _Database As Database, ByVal MessageId As Integer)
            Dim SQL As String = "update MailingMessage set Status = 'DELETED' where MessageId = " & MessageId
            _Database.ExecuteSQL(SQL)
        End Sub

        'Custom Methods
        Public Shared Function GetPastNewsletters(ByVal DB As Database) As DataTable
            Dim SQL As String = "SELECT Messageid, case when TargetType = 'MEMBER' then Name else Name + ' (custom e-mail list)' end as Name  FROM MailingMessage WHERE Status NOT IN ('NEW','DELETED') AND ParentId IS NULL ORDER BY Name ASC"
            Dim dt As DataTable = DB.GetDataTable(SQL)
            Return dt
        End Function

        Public Shared Function DuplicateMessage(ByVal DB As Database, ByVal SourceId As Integer, ByVal AdminId As Integer) As Integer

            Dim SQL As String = " Insert Into MailingMessage (" _
              & " ListPrefix, NewsletterDate, Step1, Step2, Name, TemplateId, MimeType," _
              & " FromEmail, FromName, ReplyEmail, Subject, Status, TargetType, CreateDate, " _
              & " CreateAdminId, ModifyDate, ModifyAdminId," _
              & " MessageHtml, MessageText, SavedText )" _
              & " Select ListPrefix, NewsletterDate, Step1, Step2, 'copy Of ' + Name, TemplateId, MimeType," _
              & " FromEmail, FromName, ReplyEmail, Subject, 'SAVED', TargetType," & DB.Quote(Now()) & "," _
              & AdminId & "," & DB.Quote(Now()) & "," & AdminId & "," _
              & " MessageHtml, MessageText, SavedText From MailingMessage where MessageId = " & SourceId

            Dim MessageId As Integer = DB.InsertSQL(SQL)

            SQL = "insert into MailingMessageSlot ( MessageId, Headline, Slot, SortOrder ) select " & MessageId & ", Headline, Slot, SortOrder from MailingMessageSlot Where MessageId = " & SourceId
            DB.ExecuteSQL(SQL)

            Return MessageId
        End Function

        Public Function GetSlots() As GenericSerializableCollection(Of MailingMessageSlot)
            Dim slots As New GenericSerializableCollection(Of MailingMessageSlot)
            Dim dr As SqlDataReader = DB.GetReader("select * from MailingMessageSlot where MessageId = " & MessageId & " order by SortOrder")
            While dr.Read
                Dim dbSlot As New MailingMessageSlotRow(DB)
                dbSlot.Load(dr)

                Dim slot As New MailingMessageSlot
                slot.Headline = dbSlot.Headline
                slot.MessageId = dbSlot.MessageId
                slot.SortOrder = dbSlot.SortOrder
                slot.Slot = dbSlot.Slot

                slots.Add(slot)
            End While
            dr.Close()

            Return slots
        End Function

    End Class

    Public MustInherit Class MailingMessageRowBase
        Private m_DB As Database
        Private m_MessageId As Integer = Nothing
        Private m_ParentId As Integer = Nothing
        Private m_ListPrefix As String = Nothing
        Private m_ListHTMLId As Integer = Nothing
        Private m_ListTextId As Integer = Nothing
        Private m_NewsletterDate As DateTime = Nothing
        Private m_Step1 As Boolean = Nothing
        Private m_Step2 As Boolean = Nothing
        Private m_Step3 As Boolean = Nothing
        Private m_GroupId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_TemplateId As Integer = Nothing
        Private m_MimeType As String = Nothing
        Private m_FromEmail As String = Nothing
        Private m_FromName As String = Nothing
        Private m_ReplyEmail As String = Nothing
        Private m_SentDate As DateTime = Nothing
        Private m_Subject As String = Nothing
        Private m_ScheduledDate As DateTime = Nothing
        Private m_Status As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_CreateAdminId As Integer = Nothing
        Private m_ModifyDate As DateTime = Nothing
        Private m_ModifyAdminId As Integer = Nothing
        Private m_HTMLQuery As String = Nothing
        Private m_HTMLLyrisQuery As String = Nothing
        Private m_TextQuery As String = Nothing
        Private m_TextLyrisQuery As String = Nothing
        Private m_TargetType As String = Nothing
        Private m_MessageHTML As String = Nothing
        Private m_MessageText As String = Nothing
        Private m_SavedText As String = Nothing


        Public Property MessageId() As Integer
            Get
                Return m_MessageId
            End Get
            Set(ByVal Value As Integer)
                m_MessageId = value
            End Set
        End Property

        Public Property ParentId() As Integer
            Get
                Return m_ParentId
            End Get
            Set(ByVal Value As Integer)
                m_ParentId = value
            End Set
        End Property

        Public Property ListPrefix() As String
            Get
                Return m_ListPrefix
            End Get
            Set(ByVal Value As String)
                m_ListPrefix = value
            End Set
        End Property

        Public Property ListHTMLId() As Integer
            Get
                Return m_ListHTMLId
            End Get
            Set(ByVal Value As Integer)
                m_ListHTMLId = value
            End Set
        End Property

        Public Property ListTextId() As Integer
            Get
                Return m_ListTextId
            End Get
            Set(ByVal Value As Integer)
                m_ListTextId = value
            End Set
        End Property

        Public Property NewsletterDate() As DateTime
            Get
                Return m_NewsletterDate
            End Get
            Set(ByVal Value As DateTime)
                m_NewsletterDate = value
            End Set
        End Property

        Public Property Step1() As Boolean
            Get
                Return m_Step1
            End Get
            Set(ByVal Value As Boolean)
                m_Step1 = Value
            End Set
        End Property

        Public Property Step2() As Boolean
            Get
                Return m_Step2
            End Get
            Set(ByVal Value As Boolean)
                m_Step2 = Value
            End Set
        End Property

        Public Property Step3() As Boolean
            Get
                Return m_Step3
            End Get
            Set(ByVal Value As Boolean)
                m_Step3 = Value
            End Set
        End Property

        Public Property GroupId() As Integer
            Get
                Return m_GroupId
            End Get
            Set(ByVal Value As Integer)
                m_GroupId = value
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

        Public Property TemplateId() As Integer
            Get
                Return m_TemplateId
            End Get
            Set(ByVal Value As Integer)
                m_TemplateId = value
            End Set
        End Property

        Public Property MimeType() As String
            Get
                Return m_MimeType
            End Get
            Set(ByVal Value As String)
                m_MimeType = value
            End Set
        End Property

        Public Property FromEmail() As String
            Get
                Return m_FromEmail
            End Get
            Set(ByVal Value As String)
                m_FromEmail = value
            End Set
        End Property

        Public Property FromName() As String
            Get
                Return m_FromName
            End Get
            Set(ByVal Value As String)
                m_FromName = value
            End Set
        End Property

        Public Property ReplyEmail() As String
            Get
                Return m_ReplyEmail
            End Get
            Set(ByVal Value As String)
                m_ReplyEmail = value
            End Set
        End Property

        Public Property SentDate() As DateTime
            Get
                Return m_SentDate
            End Get
            Set(ByVal Value As DateTime)
                m_SentDate = value
            End Set
        End Property

        Public Property Subject() As String
            Get
                Return m_Subject
            End Get
            Set(ByVal Value As String)
                m_Subject = value
            End Set
        End Property

        Public Property ScheduledDate() As DateTime
            Get
                Return m_ScheduledDate
            End Get
            Set(ByVal Value As DateTime)
                m_ScheduledDate = value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal Value As String)
                m_Status = value
            End Set
        End Property

        Public Readonly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        Public Property CreateAdminId() As Integer
            Get
                Return m_CreateAdminId
            End Get
            Set(ByVal Value As Integer)
                m_CreateAdminId = value
            End Set
        End Property

        Public Readonly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
        End Property

        Public Property ModifyAdminId() As Integer
            Get
                Return m_ModifyAdminId
            End Get
            Set(ByVal Value As Integer)
                m_ModifyAdminId = value
            End Set
        End Property

        Public Property HTMLQuery() As String
            Get
                Return m_HTMLQuery
            End Get
            Set(ByVal Value As String)
                m_HTMLQuery = value
            End Set
        End Property

        Public Property HTMLLyrisQuery() As String
            Get
                Return m_HTMLLyrisQuery
            End Get
            Set(ByVal Value As String)
                m_HTMLLyrisQuery = value
            End Set
        End Property

        Public Property TextQuery() As String
            Get
                Return m_TextQuery
            End Get
            Set(ByVal Value As String)
                m_TextQuery = value
            End Set
        End Property

        Public Property TextLyrisQuery() As String
            Get
                Return m_TextLyrisQuery
            End Get
            Set(ByVal Value As String)
                m_TextLyrisQuery = value
            End Set
        End Property

        Public Property TargetType() As String
            Get
                Return m_TargetType
            End Get
            Set(ByVal Value As String)
                m_TargetType = value
            End Set
        End Property

        Public Property MessageHTML() As String
            Get
                Return m_MessageHTML
            End Get
            Set(ByVal Value As String)
                m_MessageHTML = value
            End Set
        End Property

        Public Property MessageText() As String
            Get
                Return m_MessageText
            End Get
            Set(ByVal Value As String)
                m_MessageText = value
            End Set
        End Property

        Public Property SavedText() As String
            Get
                Return m_SavedText
            End Get
            Set(ByVal Value As String)
                m_SavedText = value
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

        Public Sub New(ByVal database As Database, ByVal MessageId As Integer)
            m_DB = database
            m_MessageId = MessageId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MailingMessage WHERE MessageId = " & DB.Quote(MessageId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_MessageId = Convert.ToInt32(r.Item("MessageId"))
            If IsDBNull(r.Item("ParentId")) Then
                m_ParentId = Nothing
            Else
                m_ParentId = Convert.ToInt32(r.Item("ParentId"))
            End If
            If IsDBNull(r.Item("ListPrefix")) Then
                m_ListPrefix = Nothing
            Else
                m_ListPrefix = Convert.ToString(r.Item("ListPrefix"))
            End If
            If IsDBNull(r.Item("ListHTMLId")) Then
                m_ListHTMLId = Nothing
            Else
                m_ListHTMLId = Convert.ToInt32(r.Item("ListHTMLId"))
            End If
            If IsDBNull(r.Item("ListTextId")) Then
                m_ListTextId = Nothing
            Else
                m_ListTextId = Convert.ToInt32(r.Item("ListTextId"))
            End If
            If IsDBNull(r.Item("NewsletterDate")) Then
                m_NewsletterDate = Nothing
            Else
                m_NewsletterDate = Convert.ToDateTime(r.Item("NewsletterDate"))
            End If
            If IsDBNull(r.Item("Step1")) Then
                m_Step1 = False
            Else
                m_Step1 = Convert.ToBoolean(r.Item("Step1"))
            End If
            If IsDBNull(r.Item("Step2")) Then
                m_Step2 = False
            Else
                m_Step2 = Convert.ToBoolean(r.Item("Step2"))
            End If
            If IsDBNull(r.Item("Step3")) Then
                m_Step3 = False
            Else
                m_Step3 = Convert.ToBoolean(r.Item("Step3"))
            End If
            If IsDBNull(r.Item("GroupId")) Then
                m_GroupId = Nothing
            Else
                m_GroupId = Convert.ToInt32(r.Item("GroupId"))
            End If
            m_Name = Convert.ToString(r.Item("Name"))
            m_TemplateId = Convert.ToInt32(r.Item("TemplateId"))
            m_MimeType = Convert.ToString(r.Item("MimeType"))
            If IsDBNull(r.Item("FromEmail")) Then
                m_FromEmail = Nothing
            Else
                m_FromEmail = Convert.ToString(r.Item("FromEmail"))
            End If
            If IsDBNull(r.Item("FromName")) Then
                m_FromName = Nothing
            Else
                m_FromName = Convert.ToString(r.Item("FromName"))
            End If
            If IsDBNull(r.Item("ReplyEmail")) Then
                m_ReplyEmail = Nothing
            Else
                m_ReplyEmail = Convert.ToString(r.Item("ReplyEmail"))
            End If
            If IsDBNull(r.Item("SentDate")) Then
                m_SentDate = Nothing
            Else
                m_SentDate = Convert.ToDateTime(r.Item("SentDate"))
            End If
            If IsDBNull(r.Item("Subject")) Then
                m_Subject = Nothing
            Else
                m_Subject = Convert.ToString(r.Item("Subject"))
            End If
            If IsDBNull(r.Item("ScheduledDate")) Then
                m_ScheduledDate = Nothing
            Else
                m_ScheduledDate = Convert.ToDateTime(r.Item("ScheduledDate"))
            End If
            m_Status = Convert.ToString(r.Item("Status"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_CreateAdminId = Convert.ToInt32(r.Item("CreateAdminId"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            m_ModifyAdminId = Convert.ToInt32(r.Item("ModifyAdminId"))
            If IsDBNull(r.Item("HTMLQuery")) Then
                m_HTMLQuery = Nothing
            Else
                m_HTMLQuery = Convert.ToString(r.Item("HTMLQuery"))
            End If
            If IsDBNull(r.Item("HTMLLyrisQuery")) Then
                m_HTMLLyrisQuery = Nothing
            Else
                m_HTMLLyrisQuery = Convert.ToString(r.Item("HTMLLyrisQuery"))
            End If
            If IsDBNull(r.Item("TextQuery")) Then
                m_TextQuery = Nothing
            Else
                m_TextQuery = Convert.ToString(r.Item("TextQuery"))
            End If
            If IsDBNull(r.Item("TextLyrisQuery")) Then
                m_TextLyrisQuery = Nothing
            Else
                m_TextLyrisQuery = Convert.ToString(r.Item("TextLyrisQuery"))
            End If
            If IsDBNull(r.Item("TargetType")) Then
                m_TargetType = Nothing
            Else
                m_TargetType = Convert.ToString(r.Item("TargetType"))
            End If
            If IsDBNull(r.Item("MessageHTML")) Then
                m_MessageHTML = Nothing
            Else
                m_MessageHTML = Convert.ToString(r.Item("MessageHTML"))
            End If
            If IsDBNull(r.Item("MessageText")) Then
                m_MessageText = Nothing
            Else
                m_MessageText = Convert.ToString(r.Item("MessageText"))
            End If
            If IsDBNull(r.Item("SavedText")) Then
                m_SavedText = Nothing
            Else
                m_SavedText = Convert.ToString(r.Item("SavedText"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO MailingMessage (" _
             & " ParentId" _
             & ",ListPrefix" _
             & ",ListHTMLId" _
             & ",ListTextId" _
             & ",NewsletterDate" _
             & ",Step1" _
             & ",Step2" _
             & ",Step3" _
             & ",GroupId" _
             & ",Name" _
             & ",TemplateId" _
             & ",MimeType" _
             & ",FromEmail" _
             & ",FromName" _
             & ",ReplyEmail" _
             & ",SentDate" _
             & ",Subject" _
             & ",ScheduledDate" _
             & ",Status" _
             & ",CreateDate" _
             & ",CreateAdminId" _
             & ",ModifyDate" _
             & ",ModifyAdminId" _
             & ",HTMLQuery" _
             & ",HTMLLyrisQuery" _
             & ",TextQuery" _
             & ",TextLyrisQuery" _
             & ",TargetType" _
             & ",MessageHTML" _
             & ",MessageText" _
             & ",SavedText" _
             & ") VALUES (" _
             & m_DB.NullQuote(ParentId) _
             & "," & m_DB.Quote(ListPrefix) _
             & "," & m_DB.NullQuote(ListHTMLId) _
             & "," & m_DB.NullQuote(ListTextId) _
             & "," & m_DB.Quote(NewsletterDate) _
             & "," & CInt(Step1) _
             & "," & CInt(Step2) _
             & "," & CInt(Step3) _
             & "," & m_DB.NullQuote(GroupId) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(TemplateId) _
             & "," & m_DB.Quote(MimeType) _
             & "," & m_DB.Quote(FromEmail) _
             & "," & m_DB.Quote(FromName) _
             & "," & m_DB.Quote(ReplyEmail) _
             & "," & m_DB.Quote(SentDate) _
             & "," & m_DB.Quote(Subject) _
             & "," & m_DB.Quote(ScheduledDate) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.Quote(CreateAdminId) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.Quote(ModifyAdminId) _
             & "," & m_DB.Quote(HTMLQuery) _
             & "," & m_DB.Quote(HTMLLyrisQuery) _
             & "," & m_DB.Quote(TextQuery) _
             & "," & m_DB.Quote(TextLyrisQuery) _
             & "," & m_DB.Quote(TargetType) _
             & "," & m_DB.Quote(MessageHTML) _
             & "," & m_DB.Quote(MessageText) _
             & "," & m_DB.Quote(SavedText) _
             & ")"

            MessageId = m_DB.InsertSQL(SQL)

            Return MessageId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE MailingMessage SET " _
             & " ParentId = " & m_DB.NullQuote(ParentId) _
             & ",ListPrefix = " & m_DB.Quote(ListPrefix) _
             & ",ListHTMLId = " & m_DB.NullQuote(ListHTMLId) _
             & ",ListTextId = " & m_DB.NullQuote(ListTextId) _
             & ",NewsletterDate = " & m_DB.Quote(NewsletterDate) _
             & ",Step1 = " & CInt(Step1) _
             & ",Step2 = " & CInt(Step2) _
             & ",Step3 = " & CInt(Step3) _
             & ",GroupId = " & m_DB.NullQuote(GroupId) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",TemplateId = " & m_DB.Quote(TemplateId) _
             & ",MimeType = " & m_DB.Quote(MimeType) _
             & ",FromEmail = " & m_DB.Quote(FromEmail) _
             & ",FromName = " & m_DB.Quote(FromName) _
             & ",ReplyEmail = " & m_DB.Quote(ReplyEmail) _
             & ",SentDate = " & m_DB.Quote(SentDate) _
             & ",Subject = " & m_DB.Quote(Subject) _
             & ",ScheduledDate = " & m_DB.Quote(ScheduledDate) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",ModifyDate = " & m_DB.Quote(Now) _
             & ",ModifyAdminId = " & m_DB.Quote(ModifyAdminId) _
             & ",HTMLQuery = " & m_DB.Quote(HTMLQuery) _
             & ",HTMLLyrisQuery = " & m_DB.Quote(HTMLLyrisQuery) _
             & ",TextQuery = " & m_DB.Quote(TextQuery) _
             & ",TextLyrisQuery = " & m_DB.Quote(TextLyrisQuery) _
             & ",TargetType = " & m_DB.Quote(TargetType) _
             & ",MessageHTML = " & m_DB.Quote(MessageHTML) _
             & ",MessageText = " & m_DB.Quote(MessageText) _
             & ",SavedText = " & m_DB.Quote(SavedText) _
             & " WHERE MessageId = " & m_DB.Quote(MessageId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM MailingMessage WHERE MessageId = " & m_DB.Quote(MessageId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MailingMessageCollection
        Inherits GenericCollection(Of MailingMessageRow)
    End Class

End Namespace


