Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyQuestionChoiceRow
        Inherits SurveyQuestionChoiceRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ChoiceId As Integer)
            MyBase.New(DB, ChoiceId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ChoiceId As Integer) As SurveyQuestionChoiceRow
            Dim row As SurveyQuestionChoiceRow

            row = New SurveyQuestionChoiceRow(DB, ChoiceId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ChoiceId As Integer)
            Dim row As SurveyQuestionChoiceRow

            row = New SurveyQuestionChoiceRow(DB, ChoiceId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function getChoicesByQuestionId(ByVal DB As Database, ByVal QuestionId As Integer) As DataTable
            Return DB.GetDataTable("SELECT * FROM SurveyQuestionChoice WHERE QuestionId = " & DB.NullNumber(QuestionId) & " ORDER BY SortOrder")
        End Function

    End Class

    Public MustInherit Class SurveyQuestionChoiceRowBase
        Private m_DB As Database
        Private m_ChoiceId As Integer = Nothing
        Private m_QuestionId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_DisplayText As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_SkipToPageId As Integer = Nothing
        Private m_ChildQuestionId As Integer = Nothing
        Private m_ShowResponseField As Boolean = Nothing

        Public Property ShowResponseField() As Boolean
            Get
                return m_ShowResponseField 
            End Get
            Set(ByVal value As Boolean)
                m_ShowResponseField = value
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

        Public Property SkipToPageId() As Integer
            Get
                Return m_SkipToPageId
            End Get
            Set(ByVal Value As Integer)
                m_SkipToPageId = value
            End Set
        End Property

        Public Property ChildQuestionId() As Integer
            Get
                Return m_ChildQuestionId
            End Get
            Set(ByVal Value As Integer)
                m_ChildQuestionId = value
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

        Public Sub New(ByVal DB As Database, ByVal ChoiceId As Integer)
            m_DB = DB
            m_ChoiceId = ChoiceId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyQuestionChoice WHERE ChoiceId = " & DB.Number(ChoiceId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ChoiceId = Convert.ToInt32(r.Item("ChoiceId"))
            m_QuestionId = Convert.ToInt32(r.Item("QuestionId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_DisplayText = Convert.ToString(r.Item("DisplayText"))
            If IsDBNull(r.Item("SkipToPageId")) Then
                m_SkipToPageId = Nothing
            Else
                m_SkipToPageId = Convert.ToInt32(r.Item("SkipToPageId"))
            End If
            If IsDBNull(r.Item("ChildQuestionId")) Then
                m_ChildQuestionId = Nothing
            Else
                m_ChildQuestionId = Convert.ToInt32(r.Item("ChildQuestionId"))
            End If
            m_ShowResponseField = Convert.ToBoolean(r.Item("ShowResponseField"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from SurveyQuestionChoice WHERE QuestionID = " & QuestionId & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO SurveyQuestionChoice (" _
             & " QuestionId" _
             & ",Name" _
             & ",DisplayText" _
             & ",SortOrder" _
             & ",SkipToPageId" _
             & ",ChildQuestionId" _
             & ",ShowResponseField" _
             & ") VALUES (" _
             & m_DB.NullNumber(QuestionId) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(DisplayText) _
             & "," & MaxSortOrder _
             & "," & m_DB.NullNumber(SkipToPageId) _
             & "," & m_DB.NullNumber(ChildQuestionId) _
             & "," & CInt(ShowResponseField) _
             & ")"

            ChoiceId = m_DB.InsertSQL(SQL)

            Return ChoiceId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyQuestionChoice SET " _
             & " QuestionId = " & m_DB.NullNumber(QuestionId) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",DisplayText = " & m_DB.Quote(DisplayText) _
             & ",SkipToPageId = " & m_DB.NullNumber(SkipToPageId) _
             & ",ChildQuestionId = " & m_DB.NullNumber(ChildQuestionId) _
             & ",ShowResponseField = " & CInt(ShowResponseField) _
             & " WHERE ChoiceId = " & m_DB.Quote(ChoiceId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyQuestionChoice WHERE ChoiceId = " & m_DB.Quote(ChoiceId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyQuestionChoiceCollection
        Inherits GenericCollection(Of SurveyQuestionChoiceRow)
    End Class

End Namespace

