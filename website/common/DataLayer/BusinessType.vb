Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BusinessTypeRow
        Inherits BusinessTypeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BusinessTypeID As Integer)
            MyBase.New(DB, BusinessTypeID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BusinessTypeID As Integer) As BusinessTypeRow
            Dim row As BusinessTypeRow

            row = New BusinessTypeRow(DB, BusinessTypeID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BusinessTypeID As Integer)
            Dim row As BusinessTypeRow

            row = New BusinessTypeRow(DB, BusinessTypeID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BusinessType"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class BusinessTypeRowBase
        Private m_DB As Database
        Private m_BusinessTypeID As Integer = Nothing
        Private m_BusinessType As String = Nothing


        Public Property BusinessTypeID() As Integer
            Get
                Return m_BusinessTypeID
            End Get
            Set(ByVal Value As Integer)
                m_BusinessTypeID = value
            End Set
        End Property

        Public Property BusinessType() As String
            Get
                Return m_BusinessType
            End Get
            Set(ByVal Value As String)
                m_BusinessType = value
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

        Public Sub New(ByVal DB As Database, ByVal BusinessTypeID As Integer)
            m_DB = DB
            m_BusinessTypeID = BusinessTypeID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BusinessType WHERE BusinessTypeID = " & DB.Number(BusinessTypeID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BusinessTypeID = Core.GetInt(r.Item("BusinessTypeID"))
            m_BusinessType = Core.GetString(r.Item("BusinessType"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BusinessType (" _
             & " BusinessType" _
             & ") VALUES (" _
             & m_DB.Quote(BusinessType) _
             & ")"

            BusinessTypeID = m_DB.InsertSQL(SQL)

            Return BusinessTypeID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BusinessType SET " _
             & " BusinessType = " & m_DB.Quote(BusinessType) _
             & " WHERE BusinessTypeID = " & m_DB.quote(BusinessTypeID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BusinessType WHERE BusinessTypeID = " & m_DB.Number(BusinessTypeID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BusinessTypeCollection
        Inherits GenericCollection(Of BusinessTypeRow)
    End Class

End Namespace


