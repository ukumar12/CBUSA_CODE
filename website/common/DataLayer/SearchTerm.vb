Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SearchTermRow
        Inherits SearchTermRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal TermId As Integer)
            MyBase.New(DB, TermId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TermId As Integer) As SearchTermRow
            Dim row As SearchTermRow

            row = New SearchTermRow(DB, TermId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TermId As Integer)
            Dim row As SearchTermRow

            row = New SearchTermRow(DB, TermId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function InsertSearchTerm(ByVal DB As Database, ByVal Term As String, ByVal RemoteIP As String, ByVal OrderId As String, ByVal MemberId As String, ByVal NumberResults As Integer)
            Dim row As SearchTermRow = New SearchTermRow(DB)
            row.Term = Term
            row.RemoteIP = RemoteIP
            row.OrderId = OrderId
            row.MemberId = MemberId
            row.NumberResults = NumberResults
            Dim TermId As Integer = row.Insert()
            Return TermId
        End Function

    End Class

    Public MustInherit Class SearchTermRowBase
        Private m_DB As Database
        Private m_TermId As Integer = Nothing
        Private m_Term As String = Nothing
        Private m_RemoteIP As String = Nothing
        Private m_Memberid As Integer = Nothing
        Private m_OrderId As Integer = Nothing
        Private m_NumberResults As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing


        Public Property TermId() As Integer
            Get
                Return m_TermId
            End Get
            Set(ByVal Value As Integer)
                m_TermId = value
            End Set
        End Property

        Public Property Term() As String
            Get
                Return m_Term
            End Get
            Set(ByVal Value As String)
                m_Term = value
            End Set
        End Property

        Public Property RemoteIP() As String
            Get
                Return m_RemoteIP
            End Get
            Set(ByVal Value As String)
                m_RemoteIP = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property
        Public Property MemberId() As Integer
            Get
                Return m_Memberid
            End Get
            Set(ByVal Value As Integer)
                m_Memberid = Value
            End Set
        End Property
        Public Property OrderId() As Integer
            Get
                Return m_OrderId
            End Get
            Set(ByVal Value As Integer)
                m_OrderId = Value
            End Set
        End Property
        Public Property NumberResults() As Integer
            Get
                Return m_NumberResults
            End Get
            Set(ByVal Value As Integer)
                m_NumberResults = Value
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

        Public Sub New(ByVal DB As Database, ByVal TermId As Integer)
            m_DB = DB
            m_TermId = TermId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SearchTerm WHERE TermId = " & DB.Number(TermId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TermId = Convert.ToInt32(r.Item("TermId"))
            m_Term = Convert.ToString(r.Item("Term"))
            m_RemoteIP = Convert.ToString(r.Item("RemoteIP"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(m_OrderId) Then
                m_OrderId = 0
            Else
                m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            End If
            If IsDBNull(m_Memberid) Then
                m_Memberid = 0
            Else
                m_Memberid = Convert.ToInt32(r.Item("MemberId"))
            End If
            If IsDBNull(m_NumberResults) Then
                m_NumberResults = 0
            Else
                m_NumberResults = Convert.ToInt32(r.Item("NumberResults"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO SearchTerm (" _
             & " Term" _
             & ",RemoteIP" _
             & ",CreateDate" _
             & ",OrderId" _
             & ",MemberId" _
             & ",NumberResults" _
             & ") VALUES (" _
             & m_DB.Quote(Term) _
             & "," & m_DB.Quote(RemoteIP) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Number(OrderId) _
             & "," & m_DB.Number(MemberId) _
             & "," & m_DB.Number(NumberResults) _
             & ")"

            TermId = m_DB.InsertSQL(SQL)

            Return TermId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SearchTerm SET " _
             & " Term = " & m_DB.Quote(Term) _
             & ",RemoteIP = " & m_DB.Quote(RemoteIP) _
             & ",OrderId = " & m_DB.Number(OrderId) _
             & ",MemberId = " & m_DB.Number(MemberId) _
             & ",NumberResults = " & m_DB.Number(NumberResults) _
             & " WHERE TermId = " & m_DB.Quote(TermId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SearchTerm WHERE TermId = " & m_DB.Quote(TermId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SearchTermCollection
        Inherits GenericCollection(Of SearchTermRow)
    End Class

End Namespace


