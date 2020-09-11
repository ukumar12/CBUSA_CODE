Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreOrderNoteRow
        Inherits StoreOrderNoteRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal NoteId As Integer)
            MyBase.New(DB, NoteId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal NoteId As Integer) As StoreOrderNoteRow
            Dim row As StoreOrderNoteRow

            row = New StoreOrderNoteRow(DB, NoteId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal NoteId As Integer)
            Dim row As StoreOrderNoteRow

            row = New StoreOrderNoteRow(DB, NoteId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetOrderNotesByMember(ByVal DB As Database, ByVal memberid As Integer, ByVal SortBy As String, ByVal CountRows As Integer)
            Return DB.GetDataTable("select TOP " & CountRows & " son.*,so.OrderNo,a.FirstName + ' ' + a.lastName as SubmittedBy,a.Adminid from StoreOrderNote son Inner Join StoreOrder so on son.Orderid = so.OrderId Inner Join Member m on m.MemberId = so.MemberId Left Outer Join Admin a on a.AdminId = son.Adminid where so.Memberid = " & DB.Number(memberid) & " Order By " & SortBy)
        End Function

        Public Shared Function GetOrderNotesByMemberCount(ByVal DB As Database, ByVal memberid As Integer)
            Return DB.ExecuteScalar("select count(son.NoteId) from StoreOrderNote son Inner Join StoreOrder so on son.Orderid = so.OrderId Inner Join Member m on m.MemberId = so.MemberId Left Outer Join Admin a on a.AdminId = son.Adminid where so.Memberid = " & DB.Number(memberid))
        End Function
    End Class

    Public MustInherit Class StoreOrderNoteRowBase
        Private m_DB As Database
        Private m_NoteId As Integer = Nothing
        Private m_OrderId As Integer = Nothing
        Private m_NoteDate As DateTime = Nothing
        Private m_AdminId As Integer = Nothing
        Private m_Note As String = Nothing


        Public Property NoteId() As Integer
            Get
                Return m_NoteId
            End Get
            Set(ByVal Value As Integer)
                m_NoteId = value
            End Set
        End Property

        Public Property OrderId() As Integer
            Get
                Return m_OrderId
            End Get
            Set(ByVal Value As Integer)
                m_OrderId = value
            End Set
        End Property

        Public ReadOnly Property NoteDate() As DateTime
            Get
                Return m_NoteDate
            End Get
        End Property

        Public Property AdminId() As Integer
            Get
                Return m_AdminId
            End Get
            Set(ByVal Value As Integer)
                m_AdminId = value
            End Set
        End Property

        Public Property Note() As String
            Get
                Return m_Note
            End Get
            Set(ByVal Value As String)
                m_Note = value
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

        Public Sub New(ByVal DB As Database, ByVal NoteId As Integer)
            m_DB = DB
            m_NoteId = NoteId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreOrderNote WHERE NoteId = " & DB.Number(NoteId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_NoteId = Convert.ToInt32(r.Item("NoteId"))
            m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            m_NoteDate = Convert.ToDateTime(r.Item("NoteDate"))
            m_AdminId = Convert.ToInt32(r.Item("AdminId"))
            m_Note = Convert.ToString(r.Item("Note"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreOrderNote (" _
             & " OrderId" _
             & ",NoteDate" _
             & ",AdminId" _
             & ",Note" _
             & ") VALUES (" _
             & m_DB.Number(OrderId) _
             & "," & m_DB.NullQuote(Now()) _
             & "," & m_DB.Number(AdminId) _
             & "," & m_DB.Quote(Note) _
             & ")"

            NoteId = m_DB.InsertSQL(SQL)

            Return NoteId
        End Function

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreOrderNote WHERE NoteId = " & m_DB.Quote(NoteId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreOrderNoteCollection
        Inherits GenericCollection(Of StoreOrderNoteRow)
    End Class

End Namespace


