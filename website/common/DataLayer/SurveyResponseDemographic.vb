Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyResponseDemographicRow
        Inherits SurveyResponseDemographicRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ResponseDemographicId As Integer)
            MyBase.New(DB, ResponseDemographicId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ResponseDemographicId As Integer) As SurveyResponseDemographicRow
            Dim row As SurveyResponseDemographicRow

            row = New SurveyResponseDemographicRow(DB, ResponseDemographicId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ResponseDemographicId As Integer)
            Dim row As SurveyResponseDemographicRow

            row = New SurveyResponseDemographicRow(DB, ResponseDemographicId)
            row.Remove()
        End Sub

        'Custom Methods

        Public Shared Sub DeleteDemographicFromResponseId(ByVal db As Database, ByVal ResponseId As Integer)
            db.ExecuteSQL("DELETE FROM SurveyResponseDemographic WHERE responseId = " & db.NullNumber(ResponseId))
        End Sub
        Public Shared Function GetDemographicsFromResponseId(ByVal DB As Database, ByVal ResponseId As Integer) As DataTable
            Dim sText As String = ""
            sText &= "SELECT SRD.*, SDF.TypeId, SDFT.Name "
            sText &= "FROM SurveyResponseDemographic SRD, SurveyDemographicField SDF, SurveyDemographicFieldType SDFT "
            sText &= " WHERE SRD.DemographicId = SDF.DemographicId AND "
            sText &= " SDF.TypeId = SDFT.DemographicFieldTypeId AND "
            sText &= " SRD.ResponseId = " & DB.NullNumber(ResponseId)
            sText &= " "

            Return DB.GetDataTable(sText)
        End Function

        Public Shared Function GetAllSurveyDemographics(ByVal DB As Database, ByVal SurveyId As Integer) As DataTable
            Dim sText As String = ""
            sText &= ""

            sText &= " SELECT * FROM SurveyQuestionDemographic "
            sText &= " LEFT OUTER JOIN dbo.SurveyResponseDemographic ON "
            sText &= "      SurveyResponseDemographic.DemographicID = SurveyQuestionDemographic.DemographicId "
            sText &= " INNER JOIN SurveyResponse ON SurveyResponse.ResponseId = SurveyResponseDemographic.ResponseId "
            sText &= " INNER JOIN SurveyDemographicField ON SurveyDemographicField.DemographicId = SurveyQuestionDemographic.DemographicId "
            sText &= " WHERE SurveyResponse.SurveyId =  " & DB.NullNumber(SurveyId)
            sText &= " ORDER BY SurveyResponseDemographic.ResponseId,SurveyQuestionDemographic.SortOrder "

            Return DB.GetDataTable(sText)

        End Function

    End Class

    Public MustInherit Class SurveyResponseDemographicRowBase
        Private m_DB As Database
        Private m_ResponseDemographicId As Integer = Nothing
        Private m_ResponseId As Integer = Nothing
        Private m_DemographicId As Integer = Nothing
        Private m_SurveyQuestionDemographicId As Integer = Nothing
        Private m_Value As String = Nothing


        Public Property ResponseDemographicId() As Integer
            Get
                Return m_ResponseDemographicId
            End Get
            Set(ByVal Value As Integer)
                m_ResponseDemographicId = value
            End Set
        End Property

        Public Property ResponseId() As Integer
            Get
                Return m_ResponseId
            End Get
            Set(ByVal Value As Integer)
                m_ResponseId = Value
            End Set
        End Property

        Public Property SurveyQuestionDemographicId() As Integer
            Get
                Return m_SurveyQuestionDemographicId
            End Get
            Set(ByVal Value As Integer)
                m_SurveyQuestionDemographicId = Value
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

        Public Property Value() As String
            Get
                Return m_Value
            End Get
            Set(ByVal Value As String)
                m_Value = value
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

        Public Sub New(ByVal DB As Database, ByVal ResponseDemographicId As Integer)
            m_DB = DB
            m_ResponseDemographicId = ResponseDemographicId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyResponseDemographic WHERE ResponseDemographicId = " & DB.Number(ResponseDemographicId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ResponseDemographicId = Convert.ToInt32(r.Item("ResponseDemographicId"))
            m_ResponseId = Convert.ToInt32(r.Item("ResponseId"))
            m_SurveyQuestionDemographicId = Convert.ToInt32(r.Item("SurveyQuestionDemographicId"))
            m_DemographicId = Convert.ToInt32(r.Item("DemographicId"))
            m_Value = Convert.ToString(r.Item("Value"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO SurveyResponseDemographic (" _
             & " ResponseId" _
             & ",SurveyQuestionDemographicId" _
             & ",DemographicId" _
             & ",Value" _
             & ") VALUES (" _
             & m_DB.NullNumber(ResponseId) _
             & "," & m_DB.NullNumber(SurveyQuestionDemographicId) _
             & "," & m_DB.NullNumber(DemographicId) _
             & "," & m_DB.Quote(Value) _
             & ")"

            ResponseDemographicId = m_DB.InsertSQL(SQL)

            Return ResponseDemographicId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyResponseDemographic SET " _
             & " ResponseId = " & m_DB.NullNumber(ResponseId) _
             & ",SurveyQuestionDemographicId = " & m_DB.NullNumber(SurveyQuestionDemographicId) _
             & ",DemographicId = " & m_DB.NullNumber(DemographicId) _
             & ",Value = " & m_DB.Quote(Value) _
             & " WHERE ResponseDemographicId = " & m_DB.Quote(ResponseDemographicId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyResponseDemographic WHERE ResponseDemographicId = " & m_DB.Quote(ResponseDemographicId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyResponseDemographicCollection
        Inherits GenericCollection(Of SurveyResponseDemographicRow)
    End Class

End Namespace

