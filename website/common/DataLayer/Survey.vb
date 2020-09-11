Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SurveyRow
        Inherits SurveyRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal SurveyId As Integer)
            MyBase.New(DB, SurveyId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal SurveyId As Integer) As SurveyRow
            Dim row As SurveyRow

            row = New SurveyRow(DB, SurveyId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SurveyId As Integer)
            Dim row As SurveyRow

            row = New SurveyRow(DB, SurveyId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function IsLive(ByVal db As Database, ByVal SurveyId As Integer) As Boolean
            Dim b As Boolean = False
            Dim i As Integer = db.ExecuteScalar("SELECT SurveyId " & _
                                  " FROM Survey " & _
                                  " WHERE SurveyId =  " & db.NullNumber(SurveyId) & _
                                  " AND Survey.IsActive = 1 " & _
                                  " AND ((Survey.StartDate IS NOT NULL AND Survey.StartDate <= " & db.Quote(Now) & ") OR (Survey.StartDate IS NULL))" & _
                                  " AND ((Survey.EndDate IS NOT NULL AND Survey.EndDate >= " & db.Quote(Now) & ") OR (Survey.EndDate IS NULL))" & _
                                  " AND Survey.IsDeleted <> 1 ")
            If i > 0 Then b = True
            Return b
        End Function

		Public Shared Function GetFollowUpSurvey(ByVal DB As Database) As SurveyRow
            Dim SurveyId As Integer = DB.ExecuteScalar("select top 1 surveyid from survey where IsFollowUp = 1 and IsActive = 1")
			Return SurveyRow.GetRow(DB, SurveyId)
		End Function
    End Class

    Public MustInherit Class SurveyRowBase
        Private m_DB As Database
        Private m_SurveyId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_DisplayTitle As String = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_Description As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_IsDeleted As Boolean = Nothing
        Private m_IsFollowUp As Boolean = Nothing
        Private m_IsBuilder As Boolean = Nothing
        Private m_IsVendor As Boolean = Nothing
        Private m_IsPIQ As Boolean = Nothing

        Public Property IsBuilder() As Boolean
            Get
                Return m_IsBuilder
            End Get
            Set(ByVal value As Boolean)
                m_IsBuilder = value
            End Set
        End Property
        Public Property IsVendor() As Boolean
            Get
                Return m_IsVendor
            End Get
            Set(ByVal value As Boolean)
                m_IsVendor = value
            End Set
        End Property
        Public Property IsPIQ() As Boolean
            Get
                Return m_IsPIQ
            End Get
            Set(ByVal value As Boolean)
                m_IsPIQ = value
            End Set
        End Property
		Public Property IsFollowUp() As Boolean
			Get
				Return m_IsFollowUp
			End Get
			Set(ByVal value As Boolean)
				m_IsFollowUp = value
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

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property DisplayTitle() As String
            Get
                Return m_DisplayTitle
            End Get
            Set(ByVal Value As String)
                m_DisplayTitle = value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
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

        Public Property StartDate() As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return m_EndDate
            End Get
            Set(ByVal Value As DateTime)
                m_EndDate = value
            End Set
        End Property

        Public Property IsDeleted() As Boolean
            Get
                Return m_IsDeleted
            End Get
            Set(ByVal Value As Boolean)
                m_IsDeleted = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
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

        Public Sub New(ByVal DB As Database, ByVal SurveyId As Integer)
            m_DB = DB
            m_SurveyId = SurveyId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Survey WHERE SurveyId = " & DB.Number(SurveyId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_SurveyId = Convert.ToInt32(r.Item("SurveyId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_DisplayTitle = Convert.ToString(r.Item("DisplayTitle"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            If IsDBNull(r.Item("Description")) Then
                m_Description = Nothing
            Else
                m_Description = Convert.ToString(r.Item("Description"))
            End If
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(r.Item("StartDate")) Then
                m_StartDate = Nothing
            Else
                m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            End If
            If IsDBNull(r.Item("EndDate")) Then
                m_EndDate = Nothing
            Else
                m_EndDate = Convert.ToDateTime(r.Item("EndDate"))
            End If
			m_IsDeleted = Convert.ToBoolean(r.Item("IsDeleted"))
            m_IsFollowUp = Convert.ToBoolean(r.Item("IsFollowUp"))
            m_IsPIQ = Convert.ToBoolean(r.Item("IsPIQ"))
            m_IsVendor = Convert.ToBoolean(r.Item("IsVendor"))
            m_IsBuilder = Convert.ToBoolean(r.Item("IsBuilder"))
		End Sub	'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Survey (" _
             & " Name" _
             & ",DisplayTitle" _
             & ",IsActive" _
             & ",Description" _
             & ",CreateDate" _
             & ",StartDate" _
             & ",EndDate" _
             & ",IsDeleted" _
             & ",IsFollowUp" _
             & ",IsBuilder" _
             & ",IsVendor" _
             & ",IsPIQ" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(DisplayTitle) _
             & "," & CInt(IsActive) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(StartDate) _
             & "," & m_DB.NullQuote(EndDate) _
             & "," & CInt(IsDeleted) _
             & "," & CInt(IsFollowUp) _
             & "," & CInt(IsBuilder) _
             & "," & CInt(IsVendor) _
             & "," & CInt(IsPIQ) _
             & ")"

            SurveyId = m_DB.InsertSQL(SQL)

            Return SurveyId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Survey SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",DisplayTitle = " & m_DB.Quote(DisplayTitle) _
             & ",IsActive = " & CInt(IsActive) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",StartDate = " & m_DB.NullQuote(StartDate) _
             & ",EndDate = " & m_DB.NullQuote(EndDate) _
             & ",IsDeleted = " & CInt(IsDeleted) _
             & ",IsFollowUp = " & CInt(IsFollowUp) _
             & ",IsBuilder = " & CInt(IsBuilder) _
             & ",IsVendor = " & CInt(IsVendor) _
             & ",IsPIQ = " & CInt(IsPIQ) _
             & " WHERE SurveyId = " & m_DB.Quote(SurveyId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Survey WHERE SurveyId = " & m_DB.Quote(SurveyId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SurveyCollection
        Inherits GenericCollection(Of SurveyRow)
    End Class

End Namespace

