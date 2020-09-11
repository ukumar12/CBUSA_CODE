Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PaymentStatusRow
        Inherits PaymentStatusRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PaymentStatusID As Integer)
            MyBase.New(DB, PaymentStatusID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PaymentStatusID As Integer) As PaymentStatusRow
            Dim row As PaymentStatusRow

            row = New PaymentStatusRow(DB, PaymentStatusID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PaymentStatusID As Integer)
            Dim row As PaymentStatusRow

            row = New PaymentStatusRow(DB, PaymentStatusID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PaymentStatus"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class PaymentStatusRowBase
        Private m_DB As Database
        Private m_PaymentStatusID As Integer = Nothing
        Private m_PaymentStatus As String = Nothing


        Public Property PaymentStatusID() As Integer
            Get
                Return m_PaymentStatusID
            End Get
            Set(ByVal Value As Integer)
                m_PaymentStatusID = value
            End Set
        End Property

        Public Property PaymentStatus() As String
            Get
                Return m_PaymentStatus
            End Get
            Set(ByVal Value As String)
                m_PaymentStatus = value
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

        Public Sub New(ByVal DB As Database, ByVal PaymentStatusID As Integer)
            m_DB = DB
            m_PaymentStatusID = PaymentStatusID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PaymentStatus WHERE PaymentStatusID = " & DB.Number(PaymentStatusID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PaymentStatusID = Convert.ToInt32(r.Item("PaymentStatusID"))
            m_PaymentStatus = Convert.ToString(r.Item("PaymentStatus"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PaymentStatus (" _
             & " PaymentStatus" _
             & ") VALUES (" _
             & m_DB.Quote(PaymentStatus) _
             & ")"

            PaymentStatusID = m_DB.InsertSQL(SQL)

            Return PaymentStatusID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PaymentStatus SET " _
             & " PaymentStatus = " & m_DB.Quote(PaymentStatus) _
             & " WHERE PaymentStatusID = " & m_DB.quote(PaymentStatusID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PaymentStatus WHERE PaymentStatusID = " & m_DB.Number(PaymentStatusID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PaymentStatusCollection
        Inherits GenericCollection(Of PaymentStatusRow)
    End Class

End Namespace


