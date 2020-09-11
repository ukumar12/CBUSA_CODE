Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ReferralRow
        Inherits ReferralRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ReferralId As Integer)
            MyBase.New(DB, ReferralId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ReferralId As Integer) As ReferralRow
            Dim row As ReferralRow

            row = New ReferralRow(DB, ReferralId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ReferralId As Integer)
            Dim row As ReferralRow

            row = New ReferralRow(DB, ReferralId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Sub AddClick(ByVal DB As Database, ByVal Code As String, ByVal RemoteIP As String)
            Dim row As ReferralClickRow = New ReferralClickRow(DB)
            row.ClickDate = Now
            row.RemoteIP = RemoteIP
            row.Code = Code
            row.Insert()
        End Sub

        Public Shared Function GetAllReferrals(ByVal DB As Database) As DataTable
            Dim ds As DataTable = DB.GetDataTable("select * from Referral order by Name")
            Return ds
        End Function
    End Class

    Public MustInherit Class ReferralRowBase
        Private m_DB As Database
        Private m_ReferralId As Integer = Nothing
        Private m_PromotionId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Code As String = Nothing


        Public Property ReferralId() As Integer
            Get
                Return m_ReferralId
            End Get
            Set(ByVal Value As Integer)
                m_ReferralId = value
            End Set
        End Property

        Public Property PromotionId() As Integer
            Get
                Return m_PromotionId
            End Get
            Set(ByVal value As Integer)
                m_PromotionId = value
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

        Public Property Code() As String
            Get
                Return m_Code
            End Get
            Set(ByVal Value As String)
                m_Code = value
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

        Public Sub New(ByVal DB As Database, ByVal ReferralId As Integer)
            m_DB = DB
            m_ReferralId = ReferralId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Referral WHERE ReferralId = " & DB.Number(ReferralId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_ReferralId = Convert.ToInt32(r.Item("ReferralId"))
            If IsDBNull(r.Item("PromotionId")) Then
                m_PromotionId = Nothing
            Else
                m_PromotionId = Convert.ToInt32(r.Item("PromotionId"))
            End If
            m_Name = Convert.ToString(r.Item("Name"))
            m_Code = Convert.ToString(r.Item("Code"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO Referral (" _
             & " Name" _
             & ",PromotionId" _
             & ",Code" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.NullNumber(PromotionId) _
             & "," & m_DB.Quote(Code) _
             & ")"

            ReferralId = m_DB.InsertSQL(SQL)

            Return ReferralId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Referral SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",PromotionId = " & m_DB.NullNumber(PromotionId) _
             & ",Code = " & m_DB.Quote(Code) _
             & " WHERE ReferralId = " & m_DB.Quote(ReferralId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE From ReferralClick where code in (select code from Referral WHERE ReferralId = " & m_DB.Number(ReferralId) & ")"
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM Referral WHERE ReferralId = " & m_DB.Quote(ReferralId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ReferralCollection
        Inherits GenericCollection(Of ReferralRow)
    End Class

End Namespace


