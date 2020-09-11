Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyResponseRow
        Inherits SurveyResponseRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ResponseId As Integer)
            MyBase.New(DB, ResponseId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ResponseId As Integer) As SurveyResponseRow
            Dim row As SurveyResponseRow

            row = New SurveyResponseRow(DB, ResponseId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ResponseId As Integer)
            Dim row As SurveyResponseRow

            row = New SurveyResponseRow(DB, ResponseId)
            row.Remove()
        End Sub

        'Custom Methods

        Public Shared Function GetResponses(ByVal DB As Database, ByVal SurveyId As Integer) As DataTable
            Return DB.GetDataTable("SELECT ResponseId FROM SurveyResponse WHERE SurveyId= " & DB.NullNumber(SurveyId) & " AND Status = 2 ORDER BY CompleteDate")
        End Function

        Public Shared Function getTotalResponses(ByVal DB As Database, ByVal SurveyId As Integer) As Integer
            Return DB.ExecuteScalar("SELECT COUNT(*) FROM SurveyResponse WHERE SurveyId= " & DB.NullNumber(SurveyId) & " AND Status = 2")

        End Function

        Public Shared Function GetSurveyId(ByVal DB As Database, ByVal ResponseId As Integer) As Integer
            Return DB.ExecuteScalar("SELECT SurveyId FROM SurveyResponse WHERE ResponseId = " & DB.NullNumber(ResponseId) & " AND Status < 2 ")
        End Function


    End Class

    Public MustInherit Class SurveyResponseRowBase
        Private m_DB As Database
        Private m_ResponseId As Integer = Nothing
        Private m_SurveyId As Integer = Nothing
        Private m_Status As Integer = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_CompleteDate As DateTime = Nothing
        Private m_SurveyPath As String = Nothing
        Private m_RemoteIP As String = Nothing
		Private m_OrderId As Integer = Nothing

		Public Property OrderId() As Integer
			Get
				Return m_OrderId
			End Get
			Set(ByVal value As Integer)
				m_OrderId = value
			End Set
		End Property

        Public Property ResponseId() As Integer
            Get
                Return m_ResponseId
            End Get
            Set(ByVal Value As Integer)
                m_ResponseId = value
            End Set
        End Property


        Public Property SurveyId() As Integer
            Get
                Return m_SurveyId
            End Get
            Set(ByVal Value As Integer)
                m_SurveyId = Value
            End Set
        End Property

        Public Property Status() As Integer
            Get
                Return m_Status
            End Get
            Set(ByVal Value As Integer)
                m_Status = Value
            End Set
        End Property

        Public Property StartDate() As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = Value
            End Set
        End Property

        Public Property CompleteDate() As DateTime
            Get
                Return m_CompleteDate
            End Get
            Set(ByVal Value As DateTime)
                m_CompleteDate = value
            End Set
        End Property

        Public Property SurveyPath() As String
            Get
                Return m_SurveyPath
            End Get
            Set(ByVal Value As String)
                m_SurveyPath = value
            End Set
        End Property

        Public Property RemoteIP() As String
            Get
                Return m_RemoteIP
            End Get
            Set(ByVal Value As String)
                m_RemoteIP = value
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

        Public Sub New(ByVal DB As Database, ByVal ResponseId As Integer)
            m_DB = DB
            m_ResponseId = ResponseId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyResponse WHERE ResponseId = " & DB.Number(ResponseId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ResponseId = Convert.ToInt32(r.Item("ResponseId"))
            
            m_SurveyId = Convert.ToInt32(r.Item("SurveyId"))
            If IsDBNull(r.Item("StartDate")) Then
                m_StartDate = Nothing
            Else
                m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            End If
            If IsDBNull(r.Item("CompleteDate")) Then
                m_CompleteDate = Nothing
            Else
                m_CompleteDate = Convert.ToDateTime(r.Item("CompleteDate"))
            End If
            If IsDBNull(r.Item("SurveyPath")) Then
                m_SurveyPath = Nothing
            Else
                m_SurveyPath = Convert.ToString(r.Item("SurveyPath"))
            End If
			If IsDBNull(r.Item("RemoteIP")) Then
				m_RemoteIP = Nothing
			Else
				m_RemoteIP = Convert.ToString(r.Item("RemoteIP"))
			End If
			If IsDBNull(r.Item("OrderId")) Then
				m_OrderId = Nothing
			Else
				m_OrderId = Convert.ToInt32(r.Item("OrderId"))
			End If
			m_Status = Convert.ToInt32(r.Item("Status"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


			SQL = " INSERT INTO SurveyResponse (" _
			 & "SurveyId" _
			 & ",Status" _
			 & ",OrderId" _
			 & ",StartDate" _
			 & ",CompleteDate" _
			 & ",SurveyPath" _
			 & ",RemoteIP" _
			 & ") VALUES (" _
			 & "" & m_DB.NullNumber(SurveyId) _
			 & "," & m_DB.NullNumber(Status) _
			 & "," & m_DB.NullNumber(OrderId) _
			 & "," & m_DB.NullQuote(StartDate) _
			 & "," & m_DB.NullQuote(CompleteDate) _
			 & "," & m_DB.Quote(SurveyPath) _
			 & "," & m_DB.Quote(RemoteIP) _
			 & ")"

            ResponseId = m_DB.InsertSQL(SQL)

            Return ResponseId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

			SQL = " UPDATE SurveyResponse SET " _
			 & "SurveyId = " & m_DB.NullNumber(SurveyId) _
			 & ",Status = " & m_DB.NullNumber(Status) _
				& ",OrderId = " & m_DB.NullNumber(OrderId) _
			 & ",StartDate = " & m_DB.NullQuote(StartDate) _
			 & ",CompleteDate = " & m_DB.NullQuote(CompleteDate) _
			 & ",SurveyPath = " & m_DB.Quote(SurveyPath) _
			 & ",RemoteIP = " & m_DB.Quote(RemoteIP) _
			 & " WHERE ResponseId = " & m_DB.Quote(ResponseId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyResponse WHERE ResponseId = " & m_DB.Quote(ResponseId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyResponseCollection
        Inherits GenericCollection(Of SurveyResponseRow)
    End Class

End Namespace

