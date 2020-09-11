Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AdminMessageRow
        Inherits AdminMessageRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AdminMessageID As Integer)
            MyBase.New(DB, AdminMessageID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AdminMessageID As Integer) As AdminMessageRow
            Dim row As AdminMessageRow

            row = New AdminMessageRow(DB, AdminMessageID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AdminMessageID As Integer)
            Dim row As AdminMessageRow

            row = New AdminMessageRow(DB, AdminMessageID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AdminMessage"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Sub MarkUnReadByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            If BuilderID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessageBuilderRecipient SET ReadDate = Null WHERE ReadDate IS NOT NULL AND BuilderID = " & BuilderID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub
        Public Sub MarkReadByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            If BuilderID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessageBuilderRecipient SET ReadDate = getdate() WHERE ReadDate IS NULL AND BuilderID = " & BuilderID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

        Public Sub MarkDeletedByBuilder(ByVal DB As Database, ByVal BuilderID As Integer)
            If BuilderID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessageBuilderRecipient SET IsActive = 0 WHERE BuilderID = " & BuilderID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

        Public Sub MarkUnReadByVendor(ByVal DB As Database, ByVal VendorID As Integer)
            If VendorID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessageVendorRecipient SET ReadDate = Null WHERE ReadDate IS NOT NULL AND VendorID = " & VendorID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

        Public Sub MarkReadByVendor(ByVal DB As Database, ByVal VendorID As Integer)
            If VendorID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessageVendorRecipient SET ReadDate = getdate() WHERE ReadDate IS NULL AND VendorID = " & VendorID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

        Public Sub MarkDeletedByVendor(ByVal DB As Database, ByVal VendorID As Integer)
            If VendorID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessageVendorRecipient SET IsActive = 0 WHERE VendorID = " & VendorID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

        Public Sub MarkUnReadByPIQ(ByVal DB As Database, ByVal PIQID As Integer)
            If PIQID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessagePIQRecipient SET ReadDate = Null WHERE ReadDate IS NOT NULL AND PIQID = " & PIQID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

        Public Sub MarkReadByPIQ(ByVal DB As Database, ByVal PIQID As Integer)
            If PIQID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessagePIQRecipient SET ReadDate = getdate() WHERE ReadDate IS NULL AND PIQID = " & PIQID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

        Public Sub MarkDeletedByPIQ(ByVal DB As Database, ByVal PIQID As Integer)
            If PIQID <> 0 And Me.AdminMessageID <> 0 Then
                DB.ExecuteSQL("UPDATE AdminMessagePIQRecipient SET IsActive = 0 WHERE PIQID = " & PIQID & " AND AdminMessageID = " & Me.AdminMessageID)
            End If
        End Sub

    End Class

    Public MustInherit Class AdminMessageRowBase
        Private m_DB As Database
        Private m_AdminMessageID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_Subject As String = Nothing
        Private m_Title As String = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_Message As String = Nothing
        Private m_SendEmailCopy As Boolean = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_IsAlert As Boolean = Nothing


        Public Property AdminMessageID() As Integer
            Get
                Return m_AdminMessageID
            End Get
            Set(ByVal Value As Integer)
                m_AdminMessageID = value
            End Set
        End Property

        Public Property AdminID() As Integer
            Get
                Return m_AdminID
            End Get
            Set(ByVal Value As Integer)
                m_AdminID = value
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

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property StartDate() As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return m_EndDate
            End Get
            Set(ByVal Value As DateTime)
                m_EndDate = value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal Value As String)
                m_Message = value
            End Set
        End Property

        Public Property SendEmailCopy() As Boolean
            Get
                Return m_SendEmailCopy
            End Get
            Set(ByVal Value As Boolean)
                m_SendEmailCopy = value
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

        Public Property IsAlert() As Boolean
            Get
                Return m_IsAlert
            End Get
            Set(ByVal Value As Boolean)
                m_IsAlert = value
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

        Public Sub New(ByVal DB As Database, ByVal AdminMessageID As Integer)
            m_DB = DB
            m_AdminMessageID = AdminMessageID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminMessage WHERE AdminMessageID = " & DB.Number(AdminMessageID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AdminMessageID = Convert.ToInt32(r.Item("AdminMessageID"))

            m_AdminID = Convert.ToInt32(r.Item("AdminID"))
            m_Subject = Convert.ToString(r.Item("Subject"))
            m_Title = Convert.ToString(r.Item("Title"))
            m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            If IsDBNull(r.Item("EndDate")) Then
                m_EndDate = Nothing
            Else
                m_EndDate = Convert.ToDateTime(r.Item("EndDate"))
            End If
            m_Message = Convert.ToString(r.Item("Message"))
            m_SendEmailCopy = Convert.ToBoolean(r.Item("SendEmailCopy"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_IsAlert = Convert.ToBoolean(r.Item("IsAlert"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AdminMessage (" _
             & " AdminID" _
             & ",Subject" _
             & ",Title" _
             & ",StartDate" _
             & ",EndDate" _
             & ",Message" _
             & ",SendEmailCopy" _
             & ",IsActive" _
             & ",IsAlert" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminID) _
             & "," & m_DB.Quote(Subject) _
             & "," & m_DB.Quote(Title) _
             & "," & m_DB.NullQuote(StartDate) _
             & "," & m_DB.NullQuote(EndDate) _
             & "," & m_DB.Quote(Message) _
             & "," & CInt(SendEmailCopy) _
             & "," & CInt(IsActive) _
             & "," & CInt(IsAlert) _
             & ")"

            AdminMessageID = m_DB.InsertSQL(SQL)

            Return AdminMessageID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AdminMessage SET " _
             & " AdminID = " & m_DB.NullNumber(AdminID) _
             & ",Subject = " & m_DB.Quote(Subject) _
             & ",Title = " & m_DB.Quote(Title) _
             & ",StartDate = " & m_DB.NullQuote(StartDate) _
             & ",EndDate = " & m_DB.NullQuote(EndDate) _
             & ",Message = " & m_DB.Quote(Message) _
             & ",SendEmailCopy = " & CInt(SendEmailCopy) _
             & ",IsActive = " & CInt(IsActive) _
             & ",IsAlert = " & CInt(IsAlert) _
             & " WHERE AdminMessageID = " & m_DB.Quote(AdminMessageID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminMessage WHERE AdminMessageID = " & m_DB.Number(AdminMessageID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminMessageCollection
        Inherits GenericCollection(Of AdminMessageRow)
    End Class

End Namespace

