Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyQuestionDemographicRow
        Inherits SurveyQuestionDemographicRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal SurveyQuestionDemographicId As Integer)
            MyBase.New(DB, SurveyQuestionDemographicId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal SurveyQuestionDemographicId As Integer) As SurveyQuestionDemographicRow
            Dim row As SurveyQuestionDemographicRow

            row = New SurveyQuestionDemographicRow(DB, SurveyQuestionDemographicId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SurveyQuestionDemographicId As Integer)
            Dim row As SurveyQuestionDemographicRow

            row = New SurveyQuestionDemographicRow(DB, SurveyQuestionDemographicId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetQuestionDemographics(ByVal DB As Database, ByVal QuestionId As Integer) As DataTable
            Return DB.GetDataTable("SELECT SQD.*, SD.TypeId, SD.Name FROM SurveyQuestionDemographic SQD, SurveyDemographicField SD, SurveyDemographicFieldType SDT WHERE SD.DemographicId = SQD.DemographicID AND SD.TypeId = SDT.DemographicFieldTypeId AND QuestionID = " & DB.NullNumber(QuestionId) & " ORDER BY SortOrder")
        End Function

    End Class

    Public MustInherit Class SurveyQuestionDemographicRowBase
        Private m_DB As Database
        Private m_SurveyQuestionDemographicId As Integer = Nothing
        Private m_QuestionId As Integer = Nothing
        Private m_DemographicId As Integer = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_DisplayText As String = Nothing
        Private m_IsRequired As Boolean = Nothing


        Public Property SurveyQuestionDemographicId() As Integer
            Get
                Return m_SurveyQuestionDemographicId
            End Get
            Set(ByVal Value As Integer)
                m_SurveyQuestionDemographicId = value
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

        Public Property DemographicId() As Integer
            Get
                Return m_DemographicId
            End Get
            Set(ByVal Value As Integer)
                m_DemographicId = value
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

        Public Property DisplayText() As String
            Get
                Return m_DisplayText
            End Get
            Set(ByVal Value As String)
                m_DisplayText = value
            End Set
        End Property

        Public Property IsRequired() As Boolean
            Get
                Return m_IsRequired
            End Get
            Set(ByVal Value As Boolean)
                m_IsRequired = value
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

        Public Sub New(ByVal DB As Database, ByVal SurveyQuestionDemographicId As Integer)
            m_DB = DB
            m_SurveyQuestionDemographicId = SurveyQuestionDemographicId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyQuestionDemographic WHERE SurveyQuestionDemographicId = " & DB.Number(SurveyQuestionDemographicId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_SurveyQuestionDemographicId = Convert.ToInt32(r.Item("SurveyQuestionDemographicId"))
            m_QuestionId = Convert.ToInt32(r.Item("QuestionId"))
            m_DemographicId = Convert.ToInt32(r.Item("DemographicId"))
            m_DisplayText = Convert.ToString(r.Item("DisplayText"))
            m_IsRequired = Convert.ToBoolean(r.Item("IsRequired"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from SurveyQuestionDemographic WHERE QuestionID = " & DB.NullNumber(QuestionId) & "  order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO SurveyQuestionDemographic (" _
             & " QuestionId" _
             & ",DemographicId" _
             & ",SortOrder" _
             & ",DisplayText" _
             & ",IsRequired" _
             & ") VALUES (" _
             & m_DB.NullNumber(QuestionId) _
             & "," & m_DB.NullNumber(DemographicId) _
             & "," & MaxSortOrder _
             & "," & m_DB.Quote(DisplayText) _
             & "," & CInt(IsRequired) _
             & ")"

            SurveyQuestionDemographicId = m_DB.InsertSQL(SQL)

            Return SurveyQuestionDemographicId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyQuestionDemographic SET " _
             & " QuestionId = " & m_DB.NullNumber(QuestionId) _
             & ",DemographicId = " & m_DB.NullNumber(DemographicId) _
             & ",DisplayText = " & m_DB.Quote(DisplayText) _
             & ",IsRequired = " & CInt(IsRequired) _
             & " WHERE SurveyQuestionDemographicId = " & m_DB.quote(SurveyQuestionDemographicId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyQuestionDemographic WHERE SurveyQuestionDemographicId = " & m_DB.Quote(SurveyQuestionDemographicId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyQuestionDemographicCollection
        Inherits GenericCollection(Of SurveyQuestionDemographicRow)
    End Class

End Namespace

