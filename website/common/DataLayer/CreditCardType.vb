Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class CreditCardTypeRow
        Inherits CreditCardTypeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal CardTypeId As Integer)
            MyBase.New(DB, CardTypeId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal CardTypeId As Integer) As CreditCardTypeRow
            Dim row As CreditCardTypeRow

            row = New CreditCardTypeRow(DB, CardTypeId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal CardTypeId As Integer)
            Dim row As CreditCardTypeRow

            row = New CreditCardTypeRow(DB, CardTypeId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetAllCardTypes(ByVal db As Database) As DataTable
            Return db.GetDataTable("select * from creditcardtype where isactive = 1 order by sortorder")
        End Function

    End Class

    Public MustInherit Class CreditCardTypeRowBase
        Private m_DB As Database
        Private m_CardTypeId As Integer = Nothing
        Private m_Code As String = Nothing
        Private m_Name As String = Nothing
        Private m_ImageName As String = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_SortOrder As Integer = Nothing


        Public Property CardTypeId() As Integer
            Get
                Return m_CardTypeId
            End Get
            Set(ByVal Value As Integer)
                m_CardTypeId = value
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

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = value
            End Set
        End Property

        Public Property ImageName() As String
            Get
                Return m_ImageName
            End Get
            Set(ByVal Value As String)
                m_ImageName = value
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

        Public Sub New(ByVal DB As Database, ByVal CardTypeId As Integer)
            m_DB = DB
            m_CardTypeId = CardTypeId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM CreditCardType WHERE CardTypeId = " & DB.Number(CardTypeId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_CardTypeId = Convert.ToInt32(r.Item("CardTypeId"))
            m_Code = Convert.ToString(r.Item("Code"))
            m_Name = Convert.ToString(r.Item("Name"))
            If IsDBNull(r.Item("ImageName")) Then
                m_ImageName = Nothing
            Else
                m_ImageName = Convert.ToString(r.Item("ImageName"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from CreditCardType order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO CreditCardType (" _
             & " Code" _
             & ",Name" _
             & ",ImageName" _
             & ",IsActive" _
             & ",SortOrder" _
             & ") VALUES (" _
             & m_DB.Quote(Code) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(ImageName) _
             & "," & CInt(IsActive) _
             & "," & MaxSortOrder _
             & ")"

            CardTypeId = m_DB.InsertSQL(SQL)

            Return CardTypeId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE CreditCardType SET " _
             & " Code = " & m_DB.Quote(Code) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",ImageName = " & m_DB.Quote(ImageName) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE CardTypeId = " & m_DB.quote(CardTypeId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM CreditCardType WHERE CardTypeId = " & m_DB.Quote(CardTypeId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class CreditCardTypeCollection
        Inherits GenericCollection(Of CreditCardTypeRow)
    End Class

End Namespace


