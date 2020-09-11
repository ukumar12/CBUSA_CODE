Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BannerBannerGroupRow
        Inherits BannerBannerGroupRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal UniqueId As Integer)
            MyBase.New(DB, UniqueId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal UniqueId As Integer) As BannerBannerGroupRow
            Dim row As BannerBannerGroupRow

            row = New BannerBannerGroupRow(DB, UniqueId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal UniqueId As Integer)
            Dim row As BannerBannerGroupRow

            row = New BannerBannerGroupRow(DB, UniqueId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetRowByBannerAndGroup(ByVal DB As Database, ByVal BannerId As Integer, ByVal BannerGroupId As Integer) As BannerBannerGroupRow
            Dim SQL As String = "SELECT * FROM BannerBannerGroup WHERE BannerId = " & BannerId & " and BannerGroupId = " & BannerGroupId
            Dim r As SqlDataReader
            Dim row As BannerBannerGroupRow = New BannerBannerGroupRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Sub RemoveNotMatchingByBannerWidth(ByVal DB As Database, ByVal BannerId As Integer, ByVal Width As Integer)
            DB.ExecuteSQL("DELETE FROM BannerBannerGroup WHERE BannerId = " & BannerId & " and BannerGroupId IN (SELECT BannerGroupId FROM BannerGroup WHERE MinWidth > " & Width & " OR MaxWidth < " & Width & ")")
        End Sub

        Public Shared Sub RemoveNotMatchingByMinMaxWidth(ByVal DB As Database, ByVal BannerGroupId As Integer, ByVal MinWidth As Integer, ByVal MaxWidth As Integer)
            DB.ExecuteSQL("DELETE FROM BannerBannerGroup WHERE BannerGroupId = " & BannerGroupId & " and BannerId IN (SELECT BannerId FROM Banner WHERE Width < " & MinWidth & " OR Width > " & MaxWidth & ")")
        End Sub

    End Class

    Public MustInherit Class BannerBannerGroupRowBase
        Private m_DB As Database
        Private m_UniqueId As Integer = Nothing
        Private m_BannerId As Integer = Nothing
        Private m_BannerGroupId As Integer = Nothing
        Private m_DateFrom As DateTime = Nothing
        Private m_DateTo As DateTime = Nothing
        Private m_Weight As Integer = Nothing


        Public Property UniqueId() As Integer
            Get
                Return m_UniqueId
            End Get
            Set(ByVal Value As Integer)
                m_UniqueId = value
            End Set
        End Property

        Public Property BannerId() As Integer
            Get
                Return m_BannerId
            End Get
            Set(ByVal Value As Integer)
                m_BannerId = value
            End Set
        End Property

        Public Property BannerGroupId() As Integer
            Get
                Return m_BannerGroupId
            End Get
            Set(ByVal Value As Integer)
                m_BannerGroupId = value
            End Set
        End Property

        Public Property DateFrom() As DateTime
            Get
                Return m_DateFrom
            End Get
            Set(ByVal Value As DateTime)
                m_DateFrom = value
            End Set
        End Property

        Public Property DateTo() As DateTime
            Get
                Return m_DateTo
            End Get
            Set(ByVal Value As DateTime)
                m_DateTo = value
            End Set
        End Property

        Public Property Weight() As Integer
            Get
                Return m_Weight
            End Get
            Set(ByVal Value As Integer)
                m_Weight = value
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

        Public Sub New(ByVal DB As Database, ByVal UniqueId As Integer)
            m_DB = DB
            m_UniqueId = UniqueId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BannerBannerGroup WHERE UniqueId = " & DB.Number(UniqueId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_UniqueId = Convert.ToInt32(r.Item("UniqueId"))
            m_BannerId = Convert.ToInt32(r.Item("BannerId"))
            m_BannerGroupId = Convert.ToInt32(r.Item("BannerGroupId"))
            If IsDBNull(r.Item("DateFrom")) Then
                m_DateFrom = Nothing
            Else
                m_DateFrom = Convert.ToDateTime(r.Item("DateFrom"))
            End If
            If IsDBNull(r.Item("DateTo")) Then
                m_DateTo = Nothing
            Else
                m_DateTo = Convert.ToDateTime(r.Item("DateTo"))
            End If
            m_Weight = Convert.ToInt32(r.Item("Weight"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BannerBannerGroup (" _
             & " BannerId" _
             & ",BannerGroupId" _
             & ",DateFrom" _
             & ",DateTo" _
             & ",Weight" _
             & ") VALUES (" _
             & m_DB.NullNumber(BannerId) _
             & "," & m_DB.NullNumber(BannerGroupId) _
             & "," & m_DB.NullQuote(DateFrom) _
             & "," & m_DB.NullQuote(DateTo) _
             & "," & m_DB.Number(Weight) _
             & ")"

            UniqueId = m_DB.InsertSQL(SQL)

            Return UniqueId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BannerBannerGroup SET " _
             & " BannerId = " & m_DB.NullNumber(BannerId) _
             & ",BannerGroupId = " & m_DB.NullNumber(BannerGroupId) _
             & ",DateFrom = " & m_DB.NullQuote(DateFrom) _
             & ",DateTo = " & m_DB.NullQuote(DateTo) _
             & ",Weight = " & m_DB.Number(Weight) _
             & " WHERE UniqueId = " & m_DB.quote(UniqueId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BannerBannerGroup WHERE UniqueId = " & m_DB.Quote(UniqueId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BannerBannerGroupCollection
        Inherits GenericCollection(Of BannerBannerGroupRow)
    End Class

End Namespace


