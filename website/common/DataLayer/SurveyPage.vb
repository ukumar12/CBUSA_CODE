Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyPageRow
        Inherits SurveyPageRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PageId As Integer)
            MyBase.New(DB, PageId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PageId As Integer) As SurveyPageRow
            Dim row As SurveyPageRow

            row = New SurveyPageRow(DB, PageId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PageId As Integer)
            Dim row As SurveyPageRow

            row = New SurveyPageRow(DB, PageId)
            row.Remove()
        End Sub

        'Custom Methods' 
        ' *****
        ' ** Add this function to SurveyPageRow class
        ' ******
        Public Shared Function GetAllSurveyPages(ByVal DB As Database, ByVal SurveyId As Integer) As DataSet
            Dim ds As DataSet = DB.GetDataSet("select * from SurveyPage WHERE SurveyId = " & DB.NullNumber(SurveyId) & " order by SortOrder")
            Return ds
        End Function


        ' *****
        ' ** Add this function to SurveyPageRow class
        ' ******
        Public Shared Function GetSurveyPages(ByVal DB As Database, ByVal SurveyId As Integer, ByVal GreaterThanSortOrder As Integer) As DataSet
            Dim ds As DataSet = DB.GetDataSet("select * from SurveyPage WHERE SurveyId = " & DB.NullNumber(SurveyId) & " AND SortOrder > " & DB.NullNumber(GreaterThanSortOrder) & " order by SortOrder")
            Return ds
        End Function

		Public Shared Function getFirstPageId(ByVal DB As Database, ByVal SurveyId As Integer) As Integer
			Return DB.ExecuteScalar("SELECT TOP 1 PageId FROM SurveyPage WHERE SurveyId = " & DB.NullNumber(SurveyId) & " ORDER BY SortOrder")

		End Function
		Public Shared Function getLastPageId(ByVal DB As Database, ByVal SurveyId As Integer) As Integer
			Return DB.ExecuteScalar("SELECT TOP 1 PageId FROM SurveyPage WHERE SurveyId = " & DB.NullNumber(SurveyId) & " ORDER BY SortOrder desc")

		End Function



        Public Shared Function GetNextPage(ByVal DB As Database, ByVal PageId As Integer, ByVal SurveyID As Integer) As Integer
            Dim dt As DataTable = DB.GetDataTable("SELECT PageId FROM SurveyPAge WHERE SurveyId = " & DB.NullNumber(SurveyID) & " ORDER BY SortOrder")
            Dim dr As DataRow, iLastId As Integer = 0
            For Each dr In dt.Rows
                If iLastId = PageId Then
                    iLastId = dr("PageId")
                    Exit For
                End If
                iLastId = dr("PageId")
            Next

            If iLastId = PageId Then iLastId = 0 ' Completed

            Return iLastId

        End Function

        Public Shared Function GetPreviousPage(ByVal DB As Database, ByVal PageId As Integer, ByVal SurveyID As Integer) As Integer
            Dim dt As DataTable = DB.GetDataTable("SELECT PageId FROM SurveyPAge WHERE SurveyId = " & DB.NullNumber(SurveyID) & " ORDER BY SortOrder DESC")
            Dim dr As DataRow, iLastId As Integer = 0
            For Each dr In dt.Rows
                If iLastId = PageId Then
                    iLastId = dr("PageId")
                    Exit For
                End If
                iLastId = dr("PageId")
            Next

            If iLastId = PageId Then iLastId = 0 ' is first page

            Return iLastId

        End Function

    End Class

    Public MustInherit Class SurveyPageRowBase
        Private m_DB As Database
        Private m_PageId As Integer = Nothing
        Private m_SurveyId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_DisplayName As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_Questions As DataLayer.SurveyQuestionCollection = Nothing

        Public ReadOnly Property Questions(ByVal Index As Integer) As SurveyQuestionRow
            Get
                If m_Questions Is Nothing Then
                    Dim dr As DataRow
                    Dim dt As DataTable = SurveyQuestionRow.GetSurveyPageQuestions(DB, PageId).Tables(0)
                    For Each dr In dt.Rows
                        m_Questions.Add(SurveyQuestionRow.GetRow(DB, dr("QuestionId")))
                    Next
                End If
                Return m_Questions(Index)
            End Get
        End Property


        Public Property PageId() As Integer
            Get
                Return m_PageId
            End Get
            Set(ByVal Value As Integer)
                m_PageId = value
            End Set
        End Property

        Public ReadOnly Property IsFirstPage() As Boolean
            Get
                Dim i As Integer = DB.ExecuteScalar("SELECT TOP 1 PageId FROM SurveyPage WHERE SurveyId = " & DB.NullNumber(SurveyId) & " ORDER BY SortOrder")
                If i = PageId Then Return True
                Return False
            End Get 
        End Property

        Public Property SurveyId() As Integer
            Get
                Return m_SurveyId
            End Get
            Set(ByVal Value As Integer)
                m_SurveyId = value
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

        Public Property DisplayName() As String
            Get
                Return m_DisplayName
            End Get
            Set(ByVal Value As String)
                m_DisplayName = value
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

        Public Sub New(ByVal DB As Database, ByVal PageId As Integer)
            m_DB = DB
            m_PageId = PageId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SurveyPage WHERE PageId = " & DB.Number(PageId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PageId = Convert.ToInt32(r.Item("PageId"))
            m_SurveyId = Convert.ToInt32(r.Item("SurveyId"))
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("DisplayName")) Then
                m_DisplayName = Nothing
            Else
                m_DisplayName = Convert.ToString(r.Item("DisplayName"))
            End If
            
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from SurveyPage WHERE SurveyId = " & SurveyId & " order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO SurveyPage (" _
             & " SurveyId" _
             & ",Name" _
             & ",DisplayName" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.NullNumber(SurveyId) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(DisplayName) _
             & "," & MaxSortOrder _
             & ")"

            PageId = m_DB.InsertSQL(SQL)

            Return PageId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SurveyPage SET " _
             & " SurveyId = " & m_DB.NullNumber(SurveyId) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",DisplayName = " & m_DB.Quote(DisplayName) _
             & " WHERE PageId = " & m_DB.quote(PageId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SurveyPage WHERE PageId = " & m_DB.Quote(PageId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyPageCollection
        Inherits GenericCollection(Of SurveyPageRow)
    End Class

End Namespace

