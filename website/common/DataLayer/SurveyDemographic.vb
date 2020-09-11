Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyDemographicFieldRow
        Inherits SurveyDemographicFieldRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal DemographicId As Integer)
            MyBase.New(DB, DemographicId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal DemographicId As Integer) As SurveyDemographicFieldRow
            Dim row As SurveyDemographicFieldRow

            row = New SurveyDemographicFieldRow(DB, DemographicId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal DemographicId As Integer)
            Dim row As SurveyDemographicFieldRow

            row = New SurveyDemographicFieldRow(DB, DemographicId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetUnusedSurveyDemographicFields(ByVal DB As Database, ByVal QuestionId As Integer) As DataSet
            Dim ds As DataSet = DB.GetDataSet("select * from SurveyDemographicField WHERE DemographicId NOT IN (SELECT DemographicId FROM SurveyQuestionDemographic WHERE QuestionId = " & DB.NullNumber(QuestionId) & ") order by DemographicId")
            Return ds
        End Function

        Public Shared Function GetAllSurveyDemographicFields(ByVal DB As Database) As DataSet
            Dim ds As DataSet = DB.GetDataSet("select * from SurveyDemographicField order by DemographicId")
            Return ds
        End Function

    End Class

    Public MustInherit Class SurveyDemographicFieldRowBase
        Private m_DB As Database
        Private m_DemographicId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_TypeId As Integer = Nothing


        Public Property DemographicId() As Integer
            Get
                Return m_DemographicId
            End Get
            Set(ByVal Value As Integer)
                m_DemographicId = value
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

        Public Property TypeId() As Integer
            Get
                Return m_TypeId
            End Get
            Set(ByVal Value As Integer)
                m_TypeId = value
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

        Public Sub New(ByVal DB As Database, ByVal DemographicId As Integer)
            m_DB = DB
            m_DemographicId = DemographicId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyDemographicField WHERE DemographicId = " & DB.Number(DemographicId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_DemographicId = Convert.ToInt32(r.Item("DemographicId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_TypeId = Convert.ToInt32(r.Item("TypeId"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO SurveyDemographicField (" _
             & " Name" _
             & ",TypeId" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.NullNumber(TypeId) _
             & ")"

            DemographicId = m_DB.InsertSQL(SQL)

            Return DemographicId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyDemographicField SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",TypeId = " & m_DB.NullNumber(TypeId) _
             & " WHERE DemographicId = " & m_DB.quote(DemographicId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyDemographicField WHERE DemographicId = " & m_DB.Quote(DemographicId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyDemographicFieldCollection
        Inherits GenericCollection(Of SurveyDemographicFieldRow)
    End Class

End Namespace

