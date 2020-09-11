Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PIQAdRow
        Inherits PIQAdRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PIQAdID As Integer)
            MyBase.New(DB, PIQAdID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PIQAdID As Integer) As PIQAdRow
            Dim row As PIQAdRow

            row = New PIQAdRow(DB, PIQAdID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PIQAdID As Integer)
            Dim row As PIQAdRow

            row = New PIQAdRow(DB, PIQAdID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PIQAd"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class PIQAdRowBase
        Private m_DB As Database
        Private m_PIQAdID As Integer = Nothing
        Private m_PIQID As Integer = Nothing
        Private m_AdFile As String = Nothing
        Private m_AdGUID As String = Nothing
        Private m_AltText As String = Nothing
        Private m_LinkURL As String = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing


        Public Property PIQAdID() As Integer
            Get
                Return m_PIQAdID
            End Get
            Set(ByVal Value As Integer)
                m_PIQAdID = value
            End Set
        End Property

        Public Property PIQID() As Integer
            Get
                Return m_PIQID
            End Get
            Set(ByVal Value As Integer)
                m_PIQID = value
            End Set
        End Property

        Public Property AdFile() As String
            Get
                Return m_AdFile
            End Get
            Set(ByVal Value As String)
                m_AdFile = value
            End Set
        End Property

        Public Property AdGUID() As String
            Get
                Return m_AdGUID
            End Get
            Set(ByVal Value As String)
                m_AdGUID = value
            End Set
        End Property

        Public Property AltText() As String
            Get
                Return m_AltText
            End Get
            Set(ByVal Value As String)
                m_AltText = value
            End Set
        End Property

        Public Property LinkURL() As String
            Get
                Return m_LinkURL
            End Get
            Set(ByVal Value As String)
                m_LinkURL = value
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

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
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

        Public Sub New(ByVal DB As Database, ByVal PIQAdID As Integer)
            m_DB = DB
            m_PIQAdID = PIQAdID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PIQAd WHERE PIQAdID = " & DB.Number(PIQAdID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PIQAdID = Convert.ToInt32(r.Item("PIQAdID"))
            m_PIQID = Convert.ToInt32(r.Item("PIQID"))
            m_AdFile = Convert.ToString(r.Item("AdFile"))
            If IsDBNull(r.Item("AdGUID")) Then
                m_AdGUID = Nothing
            Else
                m_AdGUID = Convert.ToString(r.Item("AdGUID"))
            End If
            m_AltText = Convert.ToString(r.Item("AltText"))
            m_LinkURL = Convert.ToString(r.Item("LinkURL"))
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
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PIQAd (" _
             & " PIQID" _
             & ",AdFile" _
             & ",AdGUID" _
             & ",AltText" _
             & ",LinkURL" _
             & ",StartDate" _
             & ",EndDate" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.NullNumber(PIQID) _
             & "," & m_DB.Quote(AdFile) _
             & "," & m_DB.Quote(AdGUID) _
             & "," & m_DB.Quote(AltText) _
             & "," & m_DB.Quote(LinkURL) _
             & "," & m_DB.NullQuote(StartDate) _
             & "," & m_DB.NullQuote(EndDate) _
             & "," & CInt(IsActive) _
             & ")"

            PIQAdID = m_DB.InsertSQL(SQL)

            Return PIQAdID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PIQAd SET " _
             & " PIQID = " & m_DB.NullNumber(PIQID) _
             & ",AdFile = " & m_DB.Quote(AdFile) _
             & ",AdGUID = " & m_DB.Quote(AdGUID) _
             & ",AltText = " & m_DB.Quote(AltText) _
             & ",LinkURL = " & m_DB.Quote(LinkURL) _
             & ",StartDate = " & m_DB.NullQuote(StartDate) _
             & ",EndDate = " & m_DB.NullQuote(EndDate) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE PIQAdID = " & m_DB.quote(PIQAdID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PIQAd WHERE PIQAdID = " & m_DB.Number(PIQAdID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PIQAdCollection
        Inherits GenericCollection(Of PIQAdRow)
    End Class

End Namespace


