Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PortfolioRow
        Inherits PortfolioRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PortfolioID As Integer)
            MyBase.New(DB, PortfolioID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PortfolioID As Integer) As PortfolioRow
            Dim row As PortfolioRow

            row = New PortfolioRow(DB, PortfolioID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PortfolioID As Integer)
            Dim row As PortfolioRow

            row = New PortfolioRow(DB, PortfolioID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from Portfolio"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class PortfolioRowBase
        Private m_DB As Database
        Private m_PortfolioID As Integer = Nothing
        Private m_Portfolio As String = Nothing


        Public Property PortfolioID() As Integer
            Get
                Return m_PortfolioID
            End Get
            Set(ByVal Value As Integer)
                m_PortfolioID = value
            End Set
        End Property

        Public Property Portfolio() As String
            Get
                Return m_Portfolio
            End Get
            Set(ByVal Value As String)
                m_Portfolio = value
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

        Public Sub New(ByVal DB As Database, ByVal PortfolioID As Integer)
            m_DB = DB
            m_PortfolioID = PortfolioID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Portfolio WHERE PortfolioID = " & DB.Number(PortfolioID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PortfolioID = Convert.ToInt32(r.Item("PortfolioID"))
            m_Portfolio = Convert.ToString(r.Item("Portfolio"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Portfolio (" _
             & " Portfolio" _
             & ") VALUES (" _
             & m_DB.Quote(Portfolio) _
             & ")"

            PortfolioID = m_DB.InsertSQL(SQL)

            Return PortfolioID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Portfolio SET " _
             & " Portfolio = " & m_DB.Quote(Portfolio) _
             & " WHERE PortfolioID = " & m_DB.quote(PortfolioID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Portfolio WHERE PortfolioID = " & m_DB.Number(PortfolioID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PortfolioCollection
        Inherits GenericCollection(Of PortfolioRow)
    End Class

End Namespace


