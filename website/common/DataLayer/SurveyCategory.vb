Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyCategoryRow
        Inherits SurveyCategoryRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal SurveyCategoryId As Integer)
            MyBase.New(DB, SurveyCategoryId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal SurveyCategoryId As Integer) As SurveyCategoryRow
            Dim row As SurveyCategoryRow

            row = New SurveyCategoryRow(DB, SurveyCategoryId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SurveyCategoryId As Integer)
            Dim row As SurveyCategoryRow

            row = New SurveyCategoryRow(DB, SurveyCategoryId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetAllSurveyCategories(ByVal DB As Database) As DataSet
            Return DB.GetDataSet("SELECT * FROM SurveyCategory ORDER BY Description")
        End Function
    End Class

    Public MustInherit Class SurveyCategoryRowBase
        Private m_DB As Database
        Private m_SurveyCategoryId As Integer = Nothing
        Private m_Description As String = Nothing


        Public Property SurveyCategoryId() As Integer
            Get
                Return m_SurveyCategoryId
            End Get
            Set(ByVal Value As Integer)
                m_SurveyCategoryId = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = value
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

        Public Sub New(ByVal DB As Database, ByVal SurveyCategoryId As Integer)
            m_DB = DB
            m_SurveyCategoryId = SurveyCategoryId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyCategory WHERE SurveyCategoryId = " & DB.Number(SurveyCategoryId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_SurveyCategoryId = Convert.ToInt32(r.Item("SurveyCategoryId"))
            m_Description = Convert.ToString(r.Item("Description"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO SurveyCategory (" _
             & " Description" _
             & ") VALUES (" _
             & m_DB.Quote(Description) _
             & ")"

            SurveyCategoryId = m_DB.InsertSQL(SQL)

            Return SurveyCategoryId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyCategory SET " _
             & " Description = " & m_DB.Quote(Description) _
             & " WHERE SurveyCategoryId = " & m_DB.quote(SurveyCategoryId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyCategory WHERE SurveyCategoryId = " & m_DB.Quote(SurveyCategoryId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyCategoryCollection
        Inherits GenericCollection(Of SurveyCategoryRow)
    End Class

End Namespace

