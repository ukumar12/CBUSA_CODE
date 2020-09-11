Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyQuestionRow
        Inherits SurveyQuestionRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal QuestionId As Integer)
            MyBase.New(DB, QuestionId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal QuestionId As Integer) As SurveyQuestionRow
            Dim row As SurveyQuestionRow

            row = New SurveyQuestionRow(DB, QuestionId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal QuestionId As Integer)
            Dim row As SurveyQuestionRow

            row = New SurveyQuestionRow(DB, QuestionId)
            row.Remove()
        End Sub

        'Custom Methods
        ' *****
        ' ** Add this function to SurveyQuestionTypeRow class
        ' ******
        Public Shared Function GetAllSurveyQuestionTypes(ByVal DB As Database) As DataSet
            Dim ds As DataSet = DB.GetDataSet("select * from SurveyQuestionType order by Name")
            Return ds
        End Function

        Public Shared Function GetSurveyPageQuestions(ByVal DB As Database, ByVal PageId As Integer) As DataSet
            Dim ds As DataSet = DB.GetDataSet("select * from SurveyQuestion WHERE PageId = " & DB.NullNumber(PageId) & " order by SortOrder")
            Return ds
        End Function

        Public Shared Function GetAllQuestionsFromSurveyID(ByVal db As Database, ByVal SurveyId As Integer) As DataTable
            Dim sText As String = "SELECT SurveyQuestion.*, SurveyPage.SortOrder FROM SurveyQuestion, SurveyPage WHERE SurveyPage.SurveyId = " & db.NullNumber(SurveyId) & " AND SurveyPage.PageId = SurveyQuestion.PageId ORDER BY SurveyPage.SortOrder, SurveyQuestion.SortOrder "
            return db.GetDataTable(sText)
        End Function


    End Class

    Public MustInherit Class SurveyQuestionRowBase
        Private m_DB As Database
        Private m_QuestionId As Integer = Nothing
        Private m_PageId As Integer = Nothing
        Private m_QuestionTypeId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Text As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_IsRequired As Boolean = Nothing
        Private m_Choices As DataLayer.SurveyQuestionChoiceCollection = Nothing
        Private m_Categories As DataLayer.SurveyQuestionCategoryCollection = Nothing

        Public ReadOnly Property Choices(ByVal SortOrder As Integer) As SurveyQuestionChoiceRow
            Get
                If m_Choices Is Nothing Then
                    Dim dr As DataRow
                    Dim dt As DataTable = SurveyQuestionChoiceRow.getChoicesByQuestionId(DB, QuestionId)
                    For Each dr In dt.Rows
                        m_Choices.Add(SurveyQuestionChoiceRow.GetRow(DB, dr("ChoiceId")))
                    Next
                End If
                Return m_Choices.Item(SortOrder - 1)
            End Get
        End Property

        Public ReadOnly Property Categories(ByVal SortOrder As Integer) As SurveyQuestionCategoryRow
            Get
                If m_Categories Is Nothing Then
                    Dim dr As DataRow
                    Dim dt As DataTable = SurveyQuestionCategoryRow.getCategoryByQuestionId(DB, QuestionId)
                    For Each dr In dt.Rows
                        m_Categories.Add(SurveyQuestionCategoryRow.GetRow(DB, dr("CategoryId")))
                    Next
                End If
                Return m_Categories.Item(SortOrder - 1)
            End Get
        End Property

        Public Property IsRequired() As Boolean
            Get
                Return m_IsRequired
            End Get
            Set(ByVal value As Boolean)
                m_IsRequired = value
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

        Public Property PageId() As Integer
            Get
                Return m_PageId
            End Get
            Set(ByVal Value As Integer)
                m_PageId = Value
            End Set
        End Property

        Public Property QuestionTypeId() As Integer
            Get
                Return m_QuestionTypeId
            End Get
            Set(ByVal Value As Integer)
                m_QuestionTypeId = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = Value
            End Set
        End Property

        Public Property Text() As String
            Get
                Return m_Text
            End Get
            Set(ByVal Value As String)
                m_Text = Value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = Value
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

        Public Sub New(ByVal DB As Database, ByVal QuestionId As Integer)
            m_DB = DB
            m_QuestionId = QuestionId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyQuestion WHERE QuestionId = " & DB.Number(QuestionId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_QuestionId = Convert.ToInt32(r.Item("QuestionId"))
            m_PageId = Convert.ToInt32(r.Item("PageId"))
            m_QuestionTypeId = Convert.ToInt32(r.Item("QuestionTypeId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_IsRequired = Convert.ToBoolean(r.Item("IsRequired"))
            If IsDBNull(r.Item("Text")) Then
                m_Text = Nothing
            Else
                m_Text = Convert.ToString(r.Item("Text"))
            End If

           

        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from SurveyQuestion WHERE PageId = " & PageId & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO SurveyQuestion (" _
             & " PageId" _
             & ",QuestionTypeId" _
             & ",Name" _
             & ",Text" _
             & ",SortOrder" _
             & ",IsRequired" _
             & ") VALUES (" _
             & m_DB.NullNumber(PageId) _
             & "," & m_DB.NullNumber(QuestionTypeId) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Text) _
             & "," & MaxSortOrder _
             & "," & CInt(IsRequired) _
             & ")"

            QuestionId = m_DB.InsertSQL(SQL)

            Return QuestionId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyQuestion SET " _
             & " PageId = " & m_DB.NullNumber(PageId) _
             & ",QuestionTypeId = " & m_DB.NullNumber(QuestionTypeId) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",Text = " & m_DB.Quote(Text) _
             & ",IsRequired = " & CInt(IsRequired) _
             & " WHERE QuestionId = " & m_DB.Quote(QuestionId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyQuestion WHERE QuestionId = " & m_DB.Quote(QuestionId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyQuestionCollection
        Inherits GenericCollection(Of SurveyQuestionRow)
    End Class

End Namespace

