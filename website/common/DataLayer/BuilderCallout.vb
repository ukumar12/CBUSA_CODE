Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderCallOutRow
        Inherits BuilderCallOutRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderCallOutID As Integer)
            MyBase.New(DB, BuilderCallOutID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderCallOutID As Integer) As BuilderCallOutRow
            Dim row As BuilderCallOutRow

            row = New BuilderCallOutRow(DB, BuilderCallOutID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderCallOutID As Integer)
            Dim row As BuilderCallOutRow

            row = New BuilderCallOutRow(DB, BuilderCallOutID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderCallOut"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class BuilderCallOutRowBase
        Private m_DB As Database
        Private m_BuilderCallOutID As Integer = Nothing
        Private m_AdminID As Integer = Nothing
        Private m_CallOut1 As String = Nothing
        Private m_CallOut2 As String = Nothing
        Private m_Updated As DateTime = Nothing


        Public Property BuilderCallOutID() As Integer
            Get
                Return m_BuilderCallOutID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderCallOutID = value
            End Set
        End Property

        Public Property AdminID() As Integer
            Get
                Return m_AdminID
            End Get
            Set(ByVal Value As Integer)
                m_AdminID = value
            End Set
        End Property

        Public Property CallOut1() As String
            Get
                Return m_CallOut1
            End Get
            Set(ByVal Value As String)
                m_CallOut1 = value
            End Set
        End Property

        Public Property CallOut2() As String
            Get
                Return m_CallOut2
            End Get
            Set(ByVal Value As String)
                m_CallOut2 = value
            End Set
        End Property

        Public Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
            Set(ByVal Value As DateTime)
                m_Updated = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderCallOutID As Integer)
            m_DB = DB
            m_BuilderCallOutID = BuilderCallOutID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderCallOut WHERE BuilderCallOutID = " & DB.Number(BuilderCallOutID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderCallOutID = Convert.ToInt32(r.Item("BuilderCallOutID"))
            m_AdminID = Convert.ToInt32(r.Item("AdminID"))
            If IsDBNull(r.Item("CallOut1")) Then
                m_CallOut1 = Nothing
            Else
                m_CallOut1 = Convert.ToString(r.Item("CallOut1"))
            End If
            If IsDBNull(r.Item("CallOut2")) Then
                m_CallOut2 = Nothing
            Else
                m_CallOut2 = Convert.ToString(r.Item("CallOut2"))
            End If
            m_Updated = Convert.ToDateTime(r.Item("Updated"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BuilderCallOut (" _
             & " AdminID" _
             & ",CallOut1" _
             & ",CallOut2" _
             & ",Updated" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminID) _
             & "," & m_DB.Quote(CallOut1) _
             & "," & m_DB.Quote(CallOut2) _
             & "," & m_DB.NullQuote(Updated) _
             & ")"

            BuilderCallOutID = m_DB.InsertSQL(SQL)

            Return BuilderCallOutID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderCallOut SET " _
             & " AdminID = " & m_DB.NullNumber(AdminID) _
             & ",CallOut1 = " & m_DB.Quote(CallOut1) _
             & ",CallOut2 = " & m_DB.Quote(CallOut2) _
             & ",Updated = " & m_DB.NullQuote(Updated) _
             & " WHERE BuilderCallOutID = " & m_DB.quote(BuilderCallOutID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderCallOut WHERE BuilderCallOutID = " & m_DB.Number(BuilderCallOutID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderCallOutCollection
        Inherits GenericCollection(Of BuilderCallOutRow)
    End Class

End Namespace

