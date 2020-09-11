Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ExcludedSearchWordsRow
        Inherits ExcludedSearchWordsRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ExcludeSearchWordId As Integer)
            MyBase.New(DB, ExcludeSearchWordId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ExcludeSearchWordId As Integer) As ExcludedSearchWordsRow
            Dim row As ExcludedSearchWordsRow

            row = New ExcludedSearchWordsRow(DB, ExcludeSearchWordId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ExcludeSearchWordId As Integer)
            Dim row As ExcludedSearchWordsRow

            row = New ExcludedSearchWordsRow(DB, ExcludeSearchWordId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from ExcludedSearchWords"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Function CleanInput(ByVal DB As Database, ByVal sKeyword As String) As String
            Dim drWords As SqlDataReader = DB.GetReader("SELECT ExcludeSearchWord FROM ExcludedSearchWords")
            While drWords.Read
                sKeyword = Replace(sKeyword, " " & drWords("ExcludeSearchWord") & " ", "")
                sKeyword = Replace(sKeyword, " " & drWords("ExcludeSearchWord"), "")
                sKeyword = Replace(sKeyword, drWords("ExcludeSearchWord") & " ", "")
                If drWords("ExcludeSearchWord") = sKeyword Then sKeyword = ""
            End While
            drWords.Close()
            Return sKeyword
        End Function

    End Class

    Public MustInherit Class ExcludedSearchWordsRowBase
        Private m_DB As Database
        Private m_ExcludeSearchWordId As Integer = Nothing
        Private m_ExcludeSearchWord As String = Nothing


        Public Property ExcludeSearchWordId() As Integer
            Get
                Return m_ExcludeSearchWordId
            End Get
            Set(ByVal Value As Integer)
                m_ExcludeSearchWordId = value
            End Set
        End Property

        Public Property ExcludeSearchWord() As String
            Get
                Return m_ExcludeSearchWord
            End Get
            Set(ByVal Value As String)
                m_ExcludeSearchWord = value
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

        Public Sub New(ByVal DB As Database, ByVal ExcludeSearchWordId As Integer)
            m_DB = DB
            m_ExcludeSearchWordId = ExcludeSearchWordId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ExcludedSearchWords WHERE ExcludeSearchWordId = " & DB.Number(ExcludeSearchWordId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ExcludeSearchWordId = Convert.ToInt32(r.Item("ExcludeSearchWordId"))
            m_ExcludeSearchWord = Convert.ToString(r.Item("ExcludeSearchWord"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO ExcludedSearchWords (" _
             & " ExcludeSearchWord" _
             & ") VALUES (" _
             & m_DB.Quote(ExcludeSearchWord) _
             & ")"

            ExcludeSearchWordId = m_DB.InsertSQL(SQL)

            Return ExcludeSearchWordId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ExcludedSearchWords SET " _
             & " ExcludeSearchWord = " & m_DB.Quote(ExcludeSearchWord) _
             & " WHERE ExcludeSearchWordId = " & m_DB.quote(ExcludeSearchWordId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ExcludedSearchWords WHERE ExcludeSearchWordId = " & m_DB.Number(ExcludeSearchWordId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ExcludedSearchWordsCollection
        Inherits GenericCollection(Of ExcludedSearchWordsRow)
    End Class

End Namespace

