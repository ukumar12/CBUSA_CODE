Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreShippingMethodRow
        Inherits StoreShippingMethodRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal MethodId As Integer)
            MyBase.New(DB, MethodId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal MethodId As Integer) As StoreShippingMethodRow
            Dim row As StoreShippingMethodRow

            row = New StoreShippingMethodRow(DB, MethodId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal MethodId As Integer)
            Dim row As StoreShippingMethodRow

            row = New StoreShippingMethodRow(DB, MethodId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetAllStoreShippingMethods(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from StoreShippingMethod order by SortOrder")
            Return dt
        End Function

        Public Shared Function GetDomesticShippingMethods(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("select * from StoreShippingMethod WHERE IsInternational = 0 order by SortOrder")
        End Function

        Public Shared Function GetInternationalShippingMethods(ByVal DB As Database) As DataTable
            Return DB.GetDataTable("select * from StoreShippingMethod WHERE IsInternational = 1 order by SortOrder")
        End Function
    End Class

    Public MustInherit Class StoreShippingMethodRowBase
        Private m_DB As Database
        Private m_MethodId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_UPSCode As String = Nothing
        Private m_FedExCode As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_IsInternational As Boolean = Nothing

        Public Property MethodId() As Integer
            Get
                Return m_MethodId
            End Get
            Set(ByVal Value As Integer)
                m_MethodId = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property UPSCode() As String
            Get
                Return m_UPSCode
            End Get
            Set(ByVal Value As String)
                m_UPSCode = value
            End Set
        End Property

        Public Property FedExCode() As String
            Get
                Return m_FedExCode
            End Get
            Set(ByVal Value As String)
                m_FedExCode = value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property

        Public Property IsInternational() As Boolean
            Get
                Return m_IsInternational
            End Get
            Set(ByVal value As Boolean)
                m_IsInternational = value
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

        Public Sub New(ByVal DB As Database, ByVal MethodId As Integer)
            m_DB = DB
            m_MethodId = MethodId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreShippingMethod WHERE MethodId = " & DB.Number(MethodId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_MethodId = Convert.ToInt32(r.Item("MethodId"))
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("UPSCode")) Then
                m_UPSCode = Nothing
            Else
                m_UPSCode = Convert.ToString(r.Item("UPSCode"))
            End If
            If IsDBNull(r.Item("FedExCode")) Then
                m_FedExCode = Nothing
            Else
                m_FedExCode = Convert.ToString(r.Item("FedExCode"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_IsInternational = Convert.ToBoolean(r.Item("IsInternational"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from StoreShippingMethod order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO StoreShippingMethod (" _
             & " Name" _
             & ",UPSCode" _
             & ",FedExCode" _
             & ",SortOrder" _
             & ",IsActive" _
             & ",IsInternational" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(UPSCode) _
             & "," & m_DB.Quote(FedExCode) _
             & "," & MaxSortOrder _
             & "," & CInt(IsActive) _
             & "," & CInt(IsInternational) _
             & ")"

            MethodId = m_DB.InsertSQL(SQL)

            Return MethodId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreShippingMethod SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",UPSCode = " & m_DB.Quote(UPSCode) _
             & ",FedExCode = " & m_DB.Quote(FedExCode) _
             & ",IsActive = " & CInt(IsActive) _
             & ",IsInternational = " & CInt(IsInternational) _
             & " WHERE MethodId = " & m_DB.Quote(MethodId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreShippingMethod WHERE MethodId = " & m_DB.Quote(MethodId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreShippingMethodCollection
        Inherits GenericCollection(Of StoreShippingMethodRow)
    End Class

End Namespace


