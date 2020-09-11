Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyAnswerRow
        Inherits SurveyAnswerRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AnswerId As Integer)
            MyBase.New(DB, AnswerId)
        End Sub 'New


        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AnswerId As Integer) As SurveyAnswerRow
            Dim row As SurveyAnswerRow

            row = New SurveyAnswerRow(DB, AnswerId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AnswerId As Integer)
            Dim row As SurveyAnswerRow

            row = New SurveyAnswerRow(DB, AnswerId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function RemoveQuestionAnswers(ByVal DB As Database, ByVal QuestionId As Integer, ByVal ResponseId As Integer)
            Return DB.ExecuteScalar("DELETE FROM SurveyAnswer WHERE QuestionId = " & DB.NullNumber(QuestionId) & " AND ResponseId = " & DB.NullNumber(ResponseId))
        End Function

        Public Shared Function GetChoiceResponses(ByVal DB As Database, ByVal questionid As Integer, ByVal ChoiceId As Integer) As DataTable
            Dim sText As String
            sText = "SELECT SurveyAnswer.*, SurveyQuestionChoice.Name FROM SurveyAnswer, SurveyQuestionChoice WHERE SurveyAnswer.Response <> '' AND SurveyAnswer.ChoiceId = SurveyQuestionChoice.ChoiceId AND SurveyAnswer.ChoiceId = " & DB.Quote(ChoiceId) & " AND SurveyAnswer.QuestionId = " & DB.NullNumber(questionid) & " ORDER BY surveyanswer.CreateDate "
            Return DB.GetDataTable(sText)
        End Function

		Public Shared Function GetAnswers(ByVal DB As Database, ByVal questionid As Integer, Optional ByVal ResponseId As Integer = Nothing) As DataTable
			Dim sText As String
			If ResponseId = Nothing Then
				sText = "SELECT *, SurveyAnswer.CreateDate FROM SurveyAnswer WHERE QuestionId = " & DB.NullNumber(questionid) & " ORDER BY surveyanswer.CreateDate "
			Else
				sText = "SELECT * FROM SurveyAnswer WHERE QuestionId = " & DB.NullNumber(questionid) & " AND ResponseId = " & DB.NullNumber(ResponseId)
			End If
			Return DB.GetDataTable(sText)
		End Function

        Public Shared Function GetChoiceCounts(ByVal DB As Database, ByVal QuestionId As Integer) As DataTable
            Dim sText As String = "SELECT *, (SELECT COUNT(*) FROM SurveyAnswer SA WHERE QuestionId = " & DB.NullNumber(QuestionId) & " AND SA.ChoiceId = SQC.ChoiceId)  as AnswerCount FROM SurveyQuestionChoice SQC WHERE SQC.QuestionId = " & DB.NullNumber(QuestionId) & " ORDER BY SQC.SortOrder "
            Return DB.GetDataTable(sText)
        End Function
        Public Shared Function GetStandardRankCounts(ByVal DB As Database, ByVal QuestionId As Integer) As DataTable
            Return DB.GetDataTable("SELECT SurveyAnswer.ChoiceId, SurveyQuestionChoice.SortOrder, SurveyAnswer.Value, Count(SurveyAnswer.Value) as RankCount FROM SurveyAnswer, SurveyQuestionChoice WHERE SurveyQuestionChoice.ChoiceId = SurveyAnswer.ChoiceId AND SurveyAnswer.QuestionId = " & DB.NullNumber(QuestionId) & " GROUP BY SurveyAnswer.ChoiceID, SurveyQuestionChoice.SortOrder,Value ORDER BY SurveyQuestionChoice.SortOrder")
        End Function

        Public Shared Function GetPercentRankCounts(ByVal DB As Database, ByVal QuestionId As Integer) As DataTable
            Return DB.GetDataTable("SELECT SurveyAnswer.ChoiceId, SurveyQuestionChoice.ShowResponseField, SurveyQuestionChoice.SortOrder, SUM(CAST(Value as int)) as ChoiceSum, Name FROM SurveyAnswer,SurveyQuestionChoice WHERE SurveyQuestionChoice.ChoiceId = SurveyAnswer.ChoiceId AND SurveyAnswer.QuestionID = " & DB.NullNumber(QuestionId) & " GROUP BY SurveyAnswer.ChoiceID, SurveyQuestionChoice.SortOrder, Name, SurveyQuestionChoice.ShowResponseField ORDER BY SurveyQuestionChoice.SortOrder")
        End Function


    End Class

    Public MustInherit Class SurveyAnswerRowBase
        Private m_DB As Database
        Private m_AnswerId As Integer = Nothing
        Private m_ResponseId As Integer = Nothing
        Private m_SurveyId As Integer = Nothing
        Private m_QuestionId As Integer = Nothing
        Private m_CategoryId As Integer = Nothing
        Private m_ChoiceId As Integer = Nothing
        Private m_Selected As Boolean = Nothing
        Private m_Value As String = Nothing
        Private m_Response As String = Nothing


        Public Property AnswerId() As Integer
            Get
                Return m_AnswerId
            End Get
            Set(ByVal Value As Integer)
                m_AnswerId = value
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
                m_SurveyId = value
            End Set
        End Property

        Public Property QuestionId() As Integer
            Get
                Return m_QuestionId
            End Get
            Set(ByVal Value As Integer)
                m_QuestionId = value
            End Set
        End Property

        Public Property CategoryId() As Integer
            Get
                Return m_CategoryId
            End Get
            Set(ByVal Value As Integer)
                m_CategoryId = value
            End Set
        End Property

        Public Property ChoiceId() As Integer
            Get
                Return m_ChoiceId
            End Get
            Set(ByVal Value As Integer)
                m_ChoiceId = value
            End Set
        End Property

        Public Property Selected() As Boolean
            Get
                Return m_Selected
            End Get
            Set(ByVal Value As Boolean)
                m_Selected = value
            End Set
        End Property

        Public Property Response() As String
            Get
                Return m_Response
            End Get
            Set(ByVal Value As String)
                m_Response = value
            End Set
        End Property


        Public Property Value() As String
            Get
                Return m_Value
            End Get
            Set(ByVal Value As String)
                m_Value = Value
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

        Public Sub New(ByVal DB As Database, ByVal AnswerId As Integer)
            m_DB = DB
            m_AnswerId = AnswerId
        End Sub 'New
        Public Sub New(ByVal DB As Database, ByVal QuestionId As Integer, ByVal ResponseId As Integer)
            m_DB = DB
            m_QuestionId = QuestionId
            m_ResponseId = ResponseId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyAnswer WHERE AnswerId = " & DB.Number(AnswerId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AnswerId = Convert.ToInt32(r.Item("AnswerId"))
            m_ResponseId = Convert.ToInt32(r.Item("ResponseId"))
            m_SurveyId = Convert.ToInt32(r.Item("SurveyId"))
            m_QuestionId = Convert.ToInt32(r.Item("QuestionId"))
            If IsDBNull(r.Item("CategoryId")) Then
                m_CategoryId = Nothing
            Else
                m_CategoryId = Convert.ToInt32(r.Item("CategoryId"))
            End If
            If IsDBNull(r.Item("ChoiceId")) Then
                m_ChoiceId = Nothing
            Else
                m_ChoiceId = Convert.ToInt32(r.Item("ChoiceId"))
            End If
            m_Selected = Convert.ToBoolean(r.Item("Selected"))
            If IsDBNull(r.Item("Response")) Then
                m_Response = Nothing
            Else
                m_Response = Convert.ToString(r.Item("Response"))
            End If
            m_Value = Convert.ToString(r.Item("Value"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO SurveyAnswer (" _
             & " ResponseId" _
             & ",SurveyId" _
             & ",QuestionId" _
             & ",CategoryId" _
             & ",ChoiceId" _
             & ",Selected" _
             & ",Value" _
             & ",Response" _
             & ") VALUES (" _
             & m_DB.NullNumber(ResponseId) _
             & "," & m_DB.NullNumber(SurveyId) _
             & "," & m_DB.NullNumber(QuestionId) _
             & "," & m_DB.NullNumber(CategoryId) _
             & "," & m_DB.NullNumber(ChoiceId) _
             & "," & CInt(Selected) _
             & "," & m_DB.Quote(Value) _
             & "," & m_DB.Quote(Response) _
             & ")"

            AnswerId = m_DB.InsertSQL(SQL)

            Return AnswerId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyAnswer SET " _
             & " ResponseId = " & m_DB.NullNumber(ResponseId) _
             & ",SurveyId = " & m_DB.NullNumber(SurveyId) _
             & ",QuestionId = " & m_DB.NullNumber(QuestionId) _
             & ",CategoryId = " & m_DB.NullNumber(CategoryId) _
             & ",ChoiceId = " & m_DB.NullNumber(ChoiceId) _
             & ",Selected = " & CInt(Selected) _
             & ",Value = " & m_DB.Quote(Value) _
             & ",Response = " & m_DB.Quote(Response) _
             & " WHERE AnswerId = " & m_DB.Quote(AnswerId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyAnswer WHERE AnswerId = " & m_DB.Quote(AnswerId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyAnswerCollection
        Inherits GenericCollection(Of SurveyAnswerRow)
    End Class

End Namespace

