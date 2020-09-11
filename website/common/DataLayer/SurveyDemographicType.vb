Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyDemographicFieldTypeRow
        Inherits SurveyDemographicFieldTypeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal DemographicFieldTypeId As Integer)
            MyBase.New(DB, DemographicFieldTypeId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal DemographicFieldTypeId As Integer) As SurveyDemographicFieldTypeRow
            Dim row As SurveyDemographicFieldTypeRow

            row = New SurveyDemographicFieldTypeRow(DB, DemographicFieldTypeId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal DemographicFieldTypeId As Integer)
            Dim row As SurveyDemographicFieldTypeRow

            row = New SurveyDemographicFieldTypeRow(DB, DemographicFieldTypeId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetAllSurveyDemographicFieldTypes(ByVal DB As Database) As DataSet
            Dim ds As DataSet = DB.GetDataSet("select * from SurveyDemographicFieldType order by Name")
            Return ds
        End Function

    End Class

    Public MustInherit Class SurveyDemographicFieldTypeRowBase
        Private m_DB As Database
        Private m_DemographicFieldTypeId As Integer = Nothing
        Private m_Name As String = Nothing


        Public Property DemographicFieldTypeId() As Integer
            Get
                Return m_DemographicFieldTypeId
            End Get
            Set(ByVal Value As Integer)
                m_DemographicFieldTypeId = value
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

        Public Sub New(ByVal DB As Database, ByVal DemographicFieldTypeId As Integer)
            m_DB = DB
            m_DemographicFieldTypeId = DemographicFieldTypeId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyDemographicFieldType WHERE DemographicFieldTypeId = " & DB.Number(DemographicFieldTypeId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_DemographicFieldTypeId = Convert.ToInt32(r.Item("DemographicFieldTypeId"))
            m_Name = Convert.ToString(r.Item("Name"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO SurveyDemographicFieldType (" _
             & " Name" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & ")"

            DemographicFieldTypeId = m_DB.InsertSQL(SQL)

            Return DemographicFieldTypeId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyDemographicFieldType SET " _
             & " Name = " & m_DB.Quote(Name) _
             & " WHERE DemographicFieldTypeId = " & m_DB.quote(DemographicFieldTypeId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyDemographicFieldType WHERE DemographicFieldTypeId = " & m_DB.Quote(DemographicFieldTypeId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyDemographicFieldTypeCollection
        Inherits GenericCollection(Of SurveyDemographicFieldTypeRow)
    End Class

End Namespace

