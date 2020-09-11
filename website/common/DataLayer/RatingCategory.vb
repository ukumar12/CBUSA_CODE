Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class RatingCategoryRow
        Inherits RatingCategoryRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal RatingCategoryID As Integer)
            MyBase.New(DB, RatingCategoryID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal RatingCategoryID As Integer) As RatingCategoryRow
            Dim row As RatingCategoryRow

            row = New RatingCategoryRow(DB, RatingCategoryID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal RatingCategoryID As Integer)
            Dim row As RatingCategoryRow

            row = New RatingCategoryRow(DB, RatingCategoryID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from RatingCategory"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class RatingCategoryRowBase
        Private m_DB As Database
        Private m_RatingCategoryID As Integer = Nothing
        Private m_RatingCategory As String = Nothing


        Public Property RatingCategoryID() As Integer
            Get
                Return m_RatingCategoryID
            End Get
            Set(ByVal Value As Integer)
                m_RatingCategoryID = value
            End Set
        End Property

        Public Property RatingCategory() As String
            Get
                Return m_RatingCategory
            End Get
            Set(ByVal Value As String)
                m_RatingCategory = value
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

        Public Sub New(ByVal DB As Database, ByVal RatingCategoryID As Integer)
            m_DB = DB
            m_RatingCategoryID = RatingCategoryID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM RatingCategory WHERE RatingCategoryID = " & DB.Number(RatingCategoryID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_RatingCategoryID = Convert.ToInt32(r.Item("RatingCategoryID"))
            m_RatingCategory = Convert.ToString(r.Item("RatingCategory"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO RatingCategory (" _
             & " RatingCategory" _
             & ") VALUES (" _
             & m_DB.Quote(RatingCategory) _
             & ")"

            RatingCategoryID = m_DB.InsertSQL(SQL)

            Return RatingCategoryID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE RatingCategory SET " _
             & " RatingCategory = " & m_DB.Quote(RatingCategory) _
             & " WHERE RatingCategoryID = " & m_DB.quote(RatingCategoryID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM RatingCategory WHERE RatingCategoryID = " & m_DB.Number(RatingCategoryID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class RatingCategoryCollection
        Inherits GenericCollection(Of RatingCategoryRow)
    End Class

End Namespace


