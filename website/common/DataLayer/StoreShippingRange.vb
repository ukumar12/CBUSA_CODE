Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreShippingRangeRow
        Inherits StoreShippingRangeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal RangeId As Integer)
            MyBase.New(DB, RangeId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal RangeId As Integer) As StoreShippingRangeRow
            Dim row As StoreShippingRangeRow

            row = New StoreShippingRangeRow(DB, RangeId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal RangeId As Integer)
            Dim row As StoreShippingRangeRow

            row = New StoreShippingRangeRow(DB, RangeId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetRanges(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("select * from StoreShippingRange")
        End Function

    End Class

    Public MustInherit Class StoreShippingRangeRowBase
        Private m_DB As Database
        Private m_RangeId As Integer = Nothing
        Private m_ShippingFrom As Double = Nothing
        Private m_ShippingTo As Double = Nothing
        Private m_ShippingValue As Double = Nothing


        Public Property RangeId() As Integer
            Get
                Return m_RangeId
            End Get
            Set(ByVal Value As Integer)
                m_RangeId = value
            End Set
        End Property

        Public Property ShippingFrom() As Double
            Get
                Return m_ShippingFrom
            End Get
            Set(ByVal Value As Double)
                m_ShippingFrom = value
            End Set
        End Property

        Public Property ShippingTo() As Double
            Get
                Return m_ShippingTo
            End Get
            Set(ByVal Value As Double)
                m_ShippingTo = value
            End Set
        End Property

        Public Property ShippingValue() As Double
            Get
                Return m_ShippingValue
            End Get
            Set(ByVal Value As Double)
                m_ShippingValue = value
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

        Public Sub New(ByVal DB As Database, ByVal RangeId As Integer)
            m_DB = DB
            m_RangeId = RangeId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreShippingRange WHERE RangeId = " & DB.Number(RangeId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_RangeId = Convert.ToInt32(r.Item("RangeId"))
            If IsDBNull(r.Item("ShippingFrom")) Then
                m_ShippingFrom = Nothing
            Else
                m_ShippingFrom = Convert.ToDouble(r.Item("ShippingFrom"))
            End If
            If IsDBNull(r.Item("ShippingTo")) Then
                m_ShippingTo = Nothing
            Else
                m_ShippingTo = Convert.ToDouble(r.Item("ShippingTo"))
            End If
            If IsDBNull(r.Item("ShippingValue")) Then
                m_ShippingValue = Nothing
            Else
                m_ShippingValue = Convert.ToDouble(r.Item("ShippingValue"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreShippingRange (" _
             & " ShippingFrom" _
             & ",ShippingTo" _
             & ",ShippingValue" _
             & ") VALUES (" _
             & m_DB.Number(ShippingFrom) _
             & "," & m_DB.Number(ShippingTo) _
             & "," & m_DB.Number(ShippingValue) _
             & ")"

            RangeId = m_DB.InsertSQL(SQL)

            Return RangeId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreShippingRange SET " _
             & " ShippingFrom = " & m_DB.Number(ShippingFrom) _
             & ",ShippingTo = " & m_DB.Number(ShippingTo) _
             & ",ShippingValue = " & m_DB.Number(ShippingValue) _
             & " WHERE RangeId = " & m_DB.quote(RangeId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreShippingRange WHERE RangeId = " & m_DB.Quote(RangeId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreShippingRangeCollection
        Inherits GenericCollection(Of StoreShippingRangeRow)
    End Class

End Namespace


