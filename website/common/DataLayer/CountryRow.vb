Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports System.Web
Imports Components

Namespace DataLayer

    Public Class CountryRow
        Inherits CountryRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal CountryId As Integer)
            MyBase.New(DB, CountryId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal CountryId As Integer) As CountryRow
            Dim row As CountryRow

            row = New CountryRow(DB, CountryId)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRow(ByVal DB As Database, ByVal CountryCode As String) As CountryRow
            Dim row As CountryRow
            Dim CountryId As Integer = DB.ExecuteScalar("SELECT CountryId FROM Country WHERE countryCode=" & DB.Quote(CountryCode))
            row = New CountryRow(DB, CountryId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal CountryId As Integer)
            Dim row As CountryRow

            row = New CountryRow(DB, CountryId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetRowByCode(ByVal DB As Database, ByVal CountryCode As String) As CountryRow
            Dim SQL As String = "SELECT * FROM Country WHERE CountryCode = " & DB.Quote(CountryCode)
            Dim r As SqlDataReader
            Dim row As CountryRow = New CountryRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Function GetCountryList(ByVal DB As Database) As DataTable
            Dim dt As DataTable = CType(HttpContext.Current.Cache("CountryDataTable"), DataTable)
            If dt Is Nothing Then
                dt = DB.GetDataTable("SELECT * FROM Country ORDER BY CountryName ASC")

                'Save in the cache object
                HttpContext.Current.Cache.Insert("CountryDataTable", dt, Nothing, DateAdd(DateInterval.Second, 15, Now), Nothing)
            End If
            Return dt
        End Function
    End Class

    Public MustInherit Class CountryRowBase
        Private m_DB As Database
        Private m_CountryId As Integer = Nothing
        Private m_CountryCode As String = Nothing
        Private m_CountryName As String = Nothing
        Private m_Shipping As Double = 0


        Public Property CountryId() As Integer
            Get
                Return m_CountryId
            End Get
            Set(ByVal Value As Integer)
                m_CountryId = value
            End Set
        End Property

        Public Property CountryCode() As String
            Get
                Return m_CountryCode
            End Get
            Set(ByVal Value As String)
                m_CountryCode = value
            End Set
        End Property

        Public Property CountryName() As String
            Get
                Return m_CountryName
            End Get
            Set(ByVal Value As String)
                m_CountryName = value
            End Set
        End Property

        Public Property Shipping() As Double
            Get
                Return m_Shipping
            End Get
            Set(ByVal Value As Double)
                m_Shipping = Value
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

        Public Sub New(ByVal DB As Database, ByVal CountryId As Integer)
            m_DB = DB
            m_CountryId = CountryId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Country WHERE CountryId = " & DB.Number(CountryId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_CountryId = Convert.ToInt32(r.Item("CountryId"))
            m_CountryCode = Convert.ToString(r.Item("CountryCode"))
            m_CountryName = Convert.ToString(r.Item("CountryName"))
            m_Shipping = Convert.ToDouble(r.Item("Shipping"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Country (" _
             & " CountryCode" _
             & ",CountryName" _
             & ",Shipping" _
             & ") VALUES (" _
             & m_DB.Quote(CountryCode) _
             & "," & m_DB.Quote(CountryName) _
             & "," & m_DB.Number(Shipping) _
             & ")"

            CountryId = m_DB.InsertSQL(SQL)

            Return CountryId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Country SET " _
             & " CountryCode = " & m_DB.Quote(CountryCode) _
             & ",CountryName = " & m_DB.Quote(CountryName) _
             & ",Shipping = " & m_DB.Number(Shipping) _
             & " WHERE CountryId = " & m_DB.Quote(CountryId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Country WHERE CountryId = " & m_DB.Quote(CountryId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class CountryCollection
        Inherits GenericCollection(Of CountryRow)
    End Class

End Namespace