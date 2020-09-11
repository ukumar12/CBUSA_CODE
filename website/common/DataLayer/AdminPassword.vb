Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports Utility

Namespace DataLayer

    Public Class AdminPasswordRow
        Inherits AdminPasswordRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PasswordId As Integer)
            MyBase.New(DB, PasswordId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PasswordId As Integer) As AdminPasswordRow
            Dim row As AdminPasswordRow

            row = New AdminPasswordRow(DB, PasswordId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PasswordId As Integer)
            Dim row As AdminPasswordRow

            row = New AdminPasswordRow(DB, PasswordId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function PasswordUsedBefore(ByVal DB As Database, ByVal AdminId As Integer, ByVal Password As String, ByVal NumRecords As Integer)
            Dim UsedBefore As Boolean = False
            Dim SQL As String = "SELECT PasswordId, Password FROM AdminPassword WHERE PasswordId IN (Select TOP " & NumRecords & " PasswordId FROM AdminPassword WHERE AdminId = " & AdminId & " ORDER BY PasswordDate DESC)"
            Dim r As SqlDataReader = DB.GetReader(SQL)
            While r.Read
                Dim PasswordInDatabase As String = Crypt.DecryptTripleDes(r("Password"))
                If Password = PasswordInDatabase Then
                    UsedBefore = True
                End If
            End While
            r.Close()
            Return UsedBefore
        End Function
    End Class

    Public MustInherit Class AdminPasswordRowBase
        Private m_DB As Database
        Private m_PasswordId As Integer = Nothing
        Private m_AdminId As Integer = Nothing
        Private m_Password As String = Nothing
        Private m_PasswordDate As DateTime = Nothing


        Public Property PasswordId() As Integer
            Get
                Return m_PasswordId
            End Get
            Set(ByVal Value As Integer)
                m_PasswordId = value
            End Set
        End Property

        Public Property AdminId() As Integer
            Get
                Return m_AdminId
            End Get
            Set(ByVal Value As Integer)
                m_AdminId = value
            End Set
        End Property

        Public Property Password() As String
            Get
                Return m_Password
            End Get
            Set(ByVal Value As String)
                m_Password = value
            End Set
        End Property

        Public Property PasswordDate() As DateTime
            Get
                Return m_PasswordDate
            End Get
            Set(ByVal Value As DateTime)
                m_PasswordDate = value
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

        Public Sub New(ByVal DB As Database, ByVal PasswordId As Integer)
            m_DB = DB
            m_PasswordId = PasswordId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AdminPassword WHERE PasswordId = " & DB.Number(PasswordId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PasswordId = Convert.ToInt32(r.Item("PasswordId"))
            m_AdminId = Convert.ToInt32(r.Item("AdminId"))
            m_Password = Convert.ToString(r.Item("Password"))
            m_PasswordDate = Convert.ToDateTime(r.Item("PasswordDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AdminPassword (" _
             & " AdminId" _
             & ",Password" _
             & ",PasswordDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminId) _
             & "," & m_DB.Quote(Password) _
             & "," & m_DB.NullQuote(PasswordDate) _
             & ")"

            PasswordId = m_DB.InsertSQL(SQL)

            Return PasswordId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AdminPassword SET " _
             & " AdminId = " & m_DB.NullNumber(AdminId) _
             & ",Password = " & m_DB.Quote(Password) _
             & ",PasswordDate = " & m_DB.NullQuote(PasswordDate) _
             & " WHERE PasswordId = " & m_DB.quote(PasswordId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AdminPassword WHERE PasswordId = " & m_DB.Quote(PasswordId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AdminPasswordCollection
        Inherits GenericCollection(Of AdminPasswordRow)
    End Class

End Namespace


