Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class CustomURLHistoryRow
        Inherits CustomURLHistoryRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal CustomURLHistoryId As Integer)
            MyBase.New(DB, CustomURLHistoryId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal CustomURLHistoryId As Integer) As CustomURLHistoryRow
            Dim row As CustomURLHistoryRow

            row = New CustomURLHistoryRow(DB, CustomURLHistoryId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal CustomURLHistoryId As Integer)
            Dim row As CustomURLHistoryRow

            row = New CustomURLHistoryRow(DB, CustomURLHistoryId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from CustomURLHistory"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Sub AddToHistory(ByVal DB As Database, ByVal OldURL As String, ByVal NewURL As String)
            If OldURL = String.Empty Then Exit Sub
            DB.ExecuteSQL("UPDATE CustomURLHistory SET RedirectURL = " & DB.Quote(NewURL) & " WHERE RedirectURL = " & DB.Quote(OldURL))
            Dim row As CustomURLHistoryRow = New CustomURLHistoryRow(DB)
            row.OldCustomURL = OldURL
            row.RedirectURL = NewURL
            row.Insert()
        End Sub

        Public Shared Function GetFromCustomURL(ByVal DB As Database, ByVal CustomURL As String) As CustomURLHistoryRow
            Dim CustomURLHistoryId As Integer = DB.ExecuteScalar("SELECT CustomURLHistoryId FROM CustomURLHistory WHERE OldCustomURL = " & DB.Quote(CustomURL))
            Dim row As CustomURLHistoryRow

            row = New CustomURLHistoryRow(DB, CustomURLHistoryId)
            row.Load()

            Return row
        End Function

    End Class

    Public MustInherit Class CustomURLHistoryRowBase
        Private m_DB As Database
        Private m_CustomURLHistoryId As Integer = Nothing
        Private m_OldCustomURL As String = Nothing
        Private m_RedirectURL As String = Nothing
        Private m_CreateDate As DateTime = Nothing


        Public Property CustomURLHistoryId() As Integer
            Get
                Return m_CustomURLHistoryId
            End Get
            Set(ByVal Value As Integer)
                m_CustomURLHistoryId = value
            End Set
        End Property

        Public Property OldCustomURL() As String
            Get
                Return m_OldCustomURL
            End Get
            Set(ByVal Value As String)
                m_OldCustomURL = value
            End Set
        End Property

        Public Property RedirectURL() As String
            Get
                Return m_RedirectURL
            End Get
            Set(ByVal Value As String)
                m_RedirectURL = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
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

        Public Sub New(ByVal DB As Database, ByVal CustomURLHistoryId As Integer)
            m_DB = DB
            m_CustomURLHistoryId = CustomURLHistoryId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM CustomURLHistory WHERE CustomURLHistoryId = " & DB.Number(CustomURLHistoryId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_CustomURLHistoryId = Convert.ToInt32(r.Item("CustomURLHistoryId"))
            m_OldCustomURL = Convert.ToString(r.Item("OldCustomURL"))
            m_RedirectURL = Convert.ToString(r.Item("RedirectURL"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO CustomURLHistory (" _
             & " OldCustomURL" _
             & ",RedirectURL" _
             & ",CreateDate" _
             & ") VALUES (" _
             & m_DB.Quote(OldCustomURL) _
             & "," & m_DB.Quote(RedirectURL) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            CustomURLHistoryId = m_DB.InsertSQL(SQL)

            Return CustomURLHistoryId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE CustomURLHistory SET " _
             & " OldCustomURL = " & m_DB.Quote(OldCustomURL) _
             & ",RedirectURL = " & m_DB.Quote(RedirectURL) _
             & " WHERE CustomURLHistoryId = " & m_DB.quote(CustomURLHistoryId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM CustomURLHistory WHERE CustomURLHistoryId = " & m_DB.Number(CustomURLHistoryId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class CustomURLHistoryCollection
        Inherits GenericCollection(Of CustomURLHistoryRow)
    End Class

End Namespace

