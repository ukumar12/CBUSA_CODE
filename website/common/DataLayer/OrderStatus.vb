Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

    Public Enum OrderStatus
        Unprocessed = 0
        Processing = 1
        Shipped = 2
        OnHold = 3
        Backordered = 4
        Complete = 5
        Cancelled = 6
    End Enum

	Public Class OrderStatusRow
		Inherits OrderStatusRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, OrderStatusID as Integer)
			MyBase.New(DB, OrderStatusID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal OrderStatusID As Integer) As OrderStatusRow
			Dim row as OrderStatusRow 
			
			row = New OrderStatusRow(DB, OrderStatusID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal OrderStatusID As Integer)
			Dim row as OrderStatusRow 
			
			row = New OrderStatusRow(DB, OrderStatusID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from OrderStatus"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

        'Custom Methods
        Public Shared Function GetRow(ByVal DB As Database, ByVal Status As OrderStatus) As OrderStatusRow
            Dim out As New OrderStatusRow(DB)
            Dim sql As String = "select * from OrderStatus where StatusCode=" & DB.Number(Status)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function

        Public Shared Function GetDefaultStatusId(ByVal DB As Database) As Integer
            Dim sql As String = "select top 1 OrderStatusID from OrderStatus order by StatusCode"
            Return DB.ExecuteScalar(sql)
        End Function

        Public Shared Function GetStatusByName(ByVal DB As Database, ByVal OrderStatus As String) As OrderStatusRow
            Dim out As New OrderStatusRow(DB)
            Dim sql As String = "select * from OrderStatus where OrderStatus=" & DB.Quote(OrderStatus)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                out.Load(sdr)
            End If
            sdr.Close()
            Return out
        End Function
    End Class
	
	Public MustInherit Class OrderStatusRowBase
		Private m_DB as Database
		Private m_OrderStatusID As Integer = nothing
		Private m_OrderStatus As String = nothing
        Private m_StatusCode As OrderStatus = Nothing
	
	
		Public Property OrderStatusID As Integer
			Get
				Return m_OrderStatusID
			End Get
			Set(ByVal Value As Integer)
				m_OrderStatusID = value
			End Set
		End Property
	
		Public Property OrderStatus As String
			Get
				Return m_OrderStatus
			End Get
			Set(ByVal Value As String)
				m_OrderStatus = value
			End Set
		End Property
	
        Public ReadOnly Property StatusCode() As OrderStatus
            Get
                Return m_StatusCode
            End Get
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
	
		Public Sub New(ByVal DB As Database, OrderStatusID as Integer)
			m_DB = DB
			m_OrderStatusID = OrderStatusID
		End Sub 'New
		
		Protected Overridable Sub Load()
			Dim r As SqlDataReader
			Dim SQL As String
	
			SQL = "SELECT * FROM OrderStatus WHERE OrderStatusID = " & DB.Number(OrderStatusID)
			r = m_DB.GetReader(SQL)
			If r.Read Then
				Me.Load(r)
			End If
			r.Close()
		End Sub
	
	
		Protected Overridable Sub Load(ByVal r as sqlDataReader)
			m_OrderStatusID = Convert.ToInt32(r.Item("OrderStatusID"))
            m_OrderStatus = Convert.ToString(r.Item("OrderStatus"))
            m_StatusCode = Core.GetInt(r.Item("StatusCode"))
		End Sub 'Load
	
		Public Overridable Function Insert() as Integer
			Dim SQL as String
	
            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 StatusCode from OrderStatus order by StatusCode desc")
			MaxSortOrder += 1
	
            SQL = " INSERT INTO OrderStatus (" _
             & " OrderStatus" _
             & ",StatusCode" _
             & ") VALUES (" _
             & m_DB.Quote(OrderStatus) _
             & "," & MaxSortOrder _
             & ")"

			OrderStatusID = m_DB.InsertSQL(SQL)
			
			Return OrderStatusID
		End Function
	
		Public Overridable Sub Update()
			Dim SQL As String
	
			SQL = " UPDATE OrderStatus SET " _
				& " OrderStatus = " & m_DB.Quote(OrderStatus) _
				& " WHERE OrderStatusID = " & m_DB.quote(OrderStatusID)
	
			m_DB.ExecuteSQL(SQL)
	
		End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM OrderStatus WHERE OrderStatusID = " & m_DB.Number(OrderStatusID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class OrderStatusCollection
		Inherits GenericCollection(Of OrderStatusRow)
	End Class

End Namespace

