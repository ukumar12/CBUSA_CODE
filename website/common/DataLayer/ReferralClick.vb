Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ReferralClickRow
        Inherits ReferralClickRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ClickId As Integer)
            MyBase.New(DB, ClickId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ClickId As Integer) As ReferralClickRow
            Dim row As ReferralClickRow

            row = New ReferralClickRow(DB, ClickId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ClickId As Integer)
            Dim row As ReferralClickRow

            row = New ReferralClickRow(DB, ClickId)
            row.Remove()
        End Sub

        'Custom Methods

    End Class

    Public MustInherit Class ReferralClickRowBase
        Private m_DB As Database
        Private m_ClickId As Integer = Nothing
        Private m_Code As String = Nothing
        Private m_RemoteIP As String = Nothing
        Private m_ClickDate As DateTime = Nothing


        Public Property ClickId() As Integer
            Get
                Return m_ClickId
            End Get
            Set(ByVal Value As Integer)
                m_ClickId = value
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

        Public Property RemoteIP() As String
            Get
                Return m_RemoteIP
            End Get
            Set(ByVal Value As String)
                m_RemoteIP = value
            End Set
        End Property

        Public Property ClickDate() As DateTime
            Get
                Return m_ClickDate
            End Get
            Set(ByVal Value As DateTime)
                m_ClickDate = value
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

        Public Sub New(ByVal DB As Database, ByVal ClickId As Integer)
            m_DB = DB
            m_ClickId = ClickId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ReferralClick WHERE ClickId = " & DB.Number(ClickId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ClickId = Convert.ToInt32(r.Item("ClickId"))
            If IsDBNull(r.Item("Code")) Then
                m_Code = Nothing
            Else
                m_Code = Convert.ToString(r.Item("Code"))
            End If
            If IsDBNull(r.Item("RemoteIP")) Then
                m_RemoteIP = Nothing
            Else
                m_RemoteIP = Convert.ToString(r.Item("RemoteIP"))
            End If
            If IsDBNull(r.Item("ClickDate")) Then
                m_ClickDate = Nothing
            Else
                m_ClickDate = Convert.ToDateTime(r.Item("ClickDate"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO ReferralClick (" _
             & " Code" _
             & ",RemoteIP" _
             & ",ClickDate" _
             & ") VALUES (" _
             & m_DB.Quote(Code) _
             & "," & m_DB.Quote(RemoteIP) _
             & "," & m_DB.NullQuote(ClickDate) _
             & ")"

            ClickId = m_DB.InsertSQL(SQL)

            Return ClickId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ReferralClick SET " _
             & " Code = " & m_DB.Quote(Code) _
             & ",RemoteIP = " & m_DB.Quote(RemoteIP) _
             & ",ClickDate = " & m_DB.NullQuote(ClickDate) _
             & " WHERE ClickId = " & m_DB.quote(ClickId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ReferralClick WHERE ClickId = " & m_DB.Quote(ClickId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ReferralClickCollection
        Inherits GenericCollection(Of ReferralClickRow)
    End Class

End Namespace


