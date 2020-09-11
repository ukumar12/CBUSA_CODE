Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyQuestionCategoryRow
        Inherits SurveyQuestionCategoryRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal CategoryId As Integer)
            MyBase.New(DB, CategoryId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal CategoryId As Integer) As SurveyQuestionCategoryRow
            Dim row As SurveyQuestionCategoryRow

            row = New SurveyQuestionCategoryRow(DB, CategoryId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal CategoryId As Integer)
            Dim row As SurveyQuestionCategoryRow

            row = New SurveyQuestionCategoryRow(DB, CategoryId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function getCategoryByQuestionId(ByVal DB As Database, ByVal QuestionId As Integer) As DataTable
            Return DB.GetDataTable("SELECT * FROM SurveyQuestionCategory WHERE QuestionId = " & DB.NullNumber(QuestionId) & " ORDER BY SortOrder")
        End Function

    End Class

    Public MustInherit Class SurveyQuestionCategoryRowBase
        Private m_DB As Database
        Private m_CategoryId As Integer = Nothing
        Private m_QuestionId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_DisplayText As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_ShowComments As Boolean = Nothing


        Public Property CategoryId() As Integer
            Get
                Return m_CategoryId
            End Get
            Set(ByVal Value As Integer)
                m_CategoryId = value
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

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property DisplayText() As String
            Get
                Return m_DisplayText
            End Get
            Set(ByVal Value As String)
                m_DisplayText = value
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

        Public Property ShowComments() As Boolean
            Get
                Return m_ShowComments
            End Get
            Set(ByVal Value As Boolean)
                m_ShowComments = value
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

        Public Sub New(ByVal DB As Database, ByVal CategoryId As Integer)
            m_DB = DB
            m_CategoryId = CategoryId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyQuestionCategory WHERE CategoryId = " & DB.Number(CategoryId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_CategoryId = Convert.ToInt32(r.Item("CategoryId"))
            m_QuestionId = Convert.ToInt32(r.Item("QuestionId"))
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("DisplayText")) Then
                m_DisplayText = Nothing
            Else
                m_DisplayText = Convert.ToString(r.Item("DisplayText"))
            End If
            m_ShowComments = Convert.ToBoolean(r.Item("ShowComments"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from SurveyQuestionCategory WHERE QuestionID = " & QuestionId & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO SurveyQuestionCategory (" _
             & " QuestionId" _
             & ",Name" _
             & ",DisplayText" _
             & ",SortOrder" _
             & ",ShowComments" _
             & ") VALUES (" _
             & m_DB.NullNumber(QuestionId) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(DisplayText) _
             & "," & MaxSortOrder _
             & "," & CInt(ShowComments) _
             & ")"

            CategoryId = m_DB.InsertSQL(SQL)

            Return CategoryId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyQuestionCategory SET " _
             & " QuestionId = " & m_DB.NullNumber(QuestionId) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",DisplayText = " & m_DB.Quote(DisplayText) _
             & ",ShowComments = " & CInt(ShowComments) _
             & " WHERE CategoryId = " & m_DB.quote(CategoryId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyQuestionCategory WHERE CategoryId = " & m_DB.Quote(CategoryId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyQuestionCategoryCollection
        Inherits GenericCollection(Of SurveyQuestionCategoryRow)
    End Class

End Namespace

