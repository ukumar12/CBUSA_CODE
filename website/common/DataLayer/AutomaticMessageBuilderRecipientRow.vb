Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AutomaticMessageBuilderRecipientRow
        Inherits AutomaticMessageBuilderRecipientRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AutomaticMessageID As Integer)
            MyBase.New(DB, AutomaticMessageID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AutomaticMessageID As Integer) As AutomaticMessageBuilderRecipientRow
            Dim row As AutomaticMessageBuilderRecipientRow

            row = New AutomaticMessageBuilderRecipientRow(DB, AutomaticMessageID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AutomaticMessageID As Integer)
            Dim row As AutomaticMessageBuilderRecipientRow

            row = New AutomaticMessageBuilderRecipientRow(DB, AutomaticMessageID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AutomaticMessageBuilderRecipient"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class AutomaticMessageBuilderRecipientRowBase
        Private m_DB As Database
        Private m_AutomaticMessageID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_ReadDate As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing


        Public Property AutomaticMessageID() As Integer
            Get
                Return m_AutomaticMessageID
            End Get
            Set(ByVal Value As Integer)
                m_AutomaticMessageID = value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property Created() As DateTime
            Get
                Return m_Created
            End Get
            Set(ByVal Value As DateTime)
                m_Created = value
            End Set
        End Property

        Public Property ReadDate() As DateTime
            Get
                Return m_ReadDate
            End Get
            Set(ByVal Value As DateTime)
                m_ReadDate = value
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

        Public Sub New(ByVal DB As Database, ByVal AutomaticMessageID As Integer)
            m_DB = DB
            m_AutomaticMessageID = AutomaticMessageID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AutomaticMessageBuilderRecipient WHERE AutomaticMessageID = " & DB.Number(AutomaticMessageID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AutomaticMessageID = Convert.ToInt32(r.Item("AutomaticMessageID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            m_Created = Convert.ToDateTime(r.Item("Created"))
            If IsDBNull(r.Item("ReadDate")) Then
                m_ReadDate = Nothing
            Else
                m_ReadDate = Convert.ToDateTime(r.Item("ReadDate"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
        End Sub 'Load

        Public Overridable Sub Insert()
            Dim SQL As String


            SQL = " INSERT INTO AutomaticMessageBuilderRecipient (" _
             & " BuilderID" _
             & ",AutomaticMessageId" _
             & ",Created" _
             & ",ReadDate" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullQuote(AutomaticMessageID) _
             & "," & m_DB.NullQuote(Created) _
             & "," & m_DB.NullQuote(ReadDate) _
             & "," & CInt(IsActive) _
             & ")"

            m_DB.ExecuteSQL(SQL)

        End Sub

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AutomaticMessageBuilderRecipient SET " _
             & " BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",Created = " & m_DB.NullQuote(Created) _
             & ",ReadDate = " & m_DB.NullQuote(ReadDate) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE AutomaticMessageID = " & m_DB.quote(AutomaticMessageID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AutomaticMessageBuilderRecipient WHERE AutomaticMessageID = " & m_DB.Number(AutomaticMessageID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AutomaticMessageBuilderRecipientCollection
        Inherits GenericCollection(Of AutomaticMessageBuilderRecipientRow)
    End Class

End Namespace

