Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
Imports System.Web

Namespace DataLayer

    Public Class StateRow
        Inherits StateRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal StateId As Integer)
            MyBase.New(DB, StateId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal StateId As Integer) As StateRow
            Dim row As StateRow

            row = New StateRow(DB, StateId)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRowByCode(ByVal DB As Database, ByVal StateCode As String) As StateRow
            Dim SQL As String = "SELECT * FROM State WHERE StateCode = " & DB.Quote(StateCode)
            Dim r As SqlDataReader
            Dim row As StateRow = New StateRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal StateId As Integer)
            Dim row As StateRow

            row = New StateRow(DB, StateId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetStateList(ByVal DB As Database) As DataTable
            Dim dt As DataTable = CType(HttpContext.Current.Cache("StatesDataTable"), DataTable)
            If dt Is Nothing Then
                dt = DB.GetDataTable("select StateCode, StateName from State order by StateName")

                'Save in the cache object
                HttpContext.Current.Cache.Insert("StatesDataTable", dt, Nothing, DateAdd(DateInterval.Second, 15, Now), Nothing)
            End If
            Return dt
        End Function
    End Class

    Public MustInherit Class StateRowBase
        Private m_DB As Database
        Private m_StateId As Integer = Nothing
        Private m_StateCode As String = Nothing
        Private m_StateName As String = Nothing
        Private m_TaxRate As Double = Nothing
        Private m_TaxShipping As Boolean = Nothing
        Private m_TaxGiftWrap As Boolean = Nothing


        Public Property StateId() As Integer
            Get
                Return m_StateId
            End Get
            Set(ByVal Value As Integer)
                m_StateId = value
            End Set
        End Property

        Public Property StateCode() As String
            Get
                Return m_StateCode
            End Get
            Set(ByVal Value As String)
                m_StateCode = value
            End Set
        End Property

        Public Property StateName() As String
            Get
                Return m_StateName
            End Get
            Set(ByVal Value As String)
                m_StateName = value
            End Set
        End Property

        Public Property TaxRate() As Double
            Get
                Return m_TaxRate
            End Get
            Set(ByVal Value As Double)
                m_TaxRate = value
            End Set
        End Property

        Public Property TaxShipping() As Boolean
            Get
                Return m_TaxShipping
            End Get
            Set(ByVal Value As Boolean)
                m_TaxShipping = value
            End Set
        End Property

        Public Property TaxGiftWrap() As Boolean
            Get
                Return m_TaxGiftWrap
            End Get
            Set(ByVal Value As Boolean)
                m_TaxGiftWrap = value
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

        Public Sub New(ByVal DB As Database, ByVal StateId As Integer)
            m_DB = DB
            m_StateId = StateId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM State WHERE StateId = " & DB.Number(StateId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_StateId = Convert.ToInt32(r.Item("StateId"))
            m_StateCode = Convert.ToString(r.Item("StateCode"))
            m_StateName = Convert.ToString(r.Item("StateName"))
            m_TaxRate = Convert.ToDouble(r.Item("TaxRate"))
            m_TaxShipping = Convert.ToBoolean(r.Item("TaxShipping"))
            m_TaxGiftWrap = Convert.ToBoolean(r.Item("TaxGiftWrap"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO State (" _
             & " StateCode" _
             & ",StateName" _
             & ",TaxRate" _
             & ",TaxShipping" _
             & ",TaxGiftWrap" _
             & ") VALUES (" _
             & m_DB.Quote(StateCode) _
             & "," & m_DB.Quote(StateName) _
             & "," & m_DB.Number(TaxRate) _
             & "," & CInt(TaxShipping) _
             & "," & CInt(TaxGiftWrap) _
             & ")"

            StateId = m_DB.InsertSQL(SQL)

            Return StateId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE State SET " _
             & " StateCode = " & m_DB.Quote(StateCode) _
             & ",StateName = " & m_DB.Quote(StateName) _
             & ",TaxRate = " & m_DB.Number(TaxRate) _
             & ",TaxShipping = " & CInt(TaxShipping) _
             & ",TaxGiftWrap = " & CInt(TaxGiftWrap) _
             & " WHERE StateId = " & m_DB.quote(StateId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM State WHERE StateId = " & m_DB.Quote(StateId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StateCollection
        Inherits GenericCollection(Of StateRow)
    End Class

End Namespace


