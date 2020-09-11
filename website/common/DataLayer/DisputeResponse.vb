Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components
	
Namespace DataLayer

	Public Class DisputeResponseRow
		Inherits DisputeResponseRowBase
	
		Public Sub New()
			MyBase.New
		End Sub 'New
	
		Public Sub New(ByVal DB As Database)
			MyBase.New(DB)
		End Sub 'New
	
		Public Sub New(ByVal DB As Database, DisputeResponseID as Integer)
			MyBase.New(DB, DisputeResponseID)
		End Sub 'New
		
		'Shared function to get one row
		Public Shared Function GetRow(ByVal DB as Database, ByVal DisputeResponseID As Integer) As DisputeResponseRow
			Dim row as DisputeResponseRow 
			
			row = New DisputeResponseRow(DB, DisputeResponseID)
			row.Load()
			
			Return row
		End Function

		Public Shared Sub RemoveRow(ByVal DB as Database, ByVal DisputeResponseID As Integer)
			Dim row as DisputeResponseRow 
			
			row = New DisputeResponseRow(DB, DisputeResponseID)
			row.Remove()
		End Sub

		Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder as String = "ASC") As DataTable
			Dim SQL As String = "select * from DisputeResponse"
			if not SortBy = String.Empty then
				SortBy = Core.ProtectParam(SortBy)
				SortOrder = Core.ProtectParam(SortOrder)

				SQL &= " order by " & SortBy & " " & SortOrder
			End if
			Return DB.GetDataTable(SQL)
		End Function

		'Custom Methods

	End Class
	
	Public MustInherit Class DisputeResponseRowBase
		Private m_DB as Database
		Private m_DisputeResponseID As Integer = nothing
        Private m_DisputeResponse As String = Nothing


        Public Property DisputeResponseID() As Integer
            Get
                Return m_DisputeResponseID
            End Get
            Set(ByVal Value As Integer)
                m_DisputeResponseID = Value
            End Set
        End Property

        Public Property DisputeResponse() As String
            Get
                Return m_DisputeResponse
            End Get
            Set(ByVal Value As String)
                m_DisputeResponse = Value
            End Set
        End Property


        Public Property DB() As Database
            Get
                DB = m_DB
            End Get
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal DisputeResponseID As Integer)
            m_DB = DB
            m_DisputeResponseID = DisputeResponseID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM DisputeResponse WHERE DisputeResponseID = " & DB.Number(DisputeResponseID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_DisputeResponseID = Core.GetInt(r.Item("DisputeResponseID"))
            m_DisputeResponse = Core.GetString(r.Item("DisputeResponse"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO DisputeResponse (" _
             & " DisputeResponse" _
             & ") VALUES (" _
             & m_DB.Quote(DisputeResponse) _
             & ")"

            DisputeResponseID = m_DB.InsertSQL(SQL)

            Return DisputeResponseID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE DisputeResponse SET " _
             & " DisputeResponse = " & m_DB.Quote(DisputeResponse) _
             & " WHERE DisputeResponseID = " & m_DB.Quote(DisputeResponseID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
	
		Public Sub Remove()
			Dim SQL As String
	
			SQL = "DELETE FROM DisputeResponse WHERE DisputeResponseID = " & m_DB.Number(DisputeResponseID)
			m_DB.ExecuteSQL(SQL)
		End Sub 'Remove
	End Class
	
	Public Class DisputeResponseCollection
		Inherits GenericCollection(Of DisputeResponseRow)
	End Class

End Namespace

