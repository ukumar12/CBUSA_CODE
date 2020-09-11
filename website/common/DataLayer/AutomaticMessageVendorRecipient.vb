Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AutomaticMessageVendorRecipientRow
        Inherits AutomaticMessageVendorRecipientRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AutomaticMessageID As Integer, ByVal VendorID As Integer)
            MyBase.New(DB, AutomaticMessageID, VendorID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AutomaticMessageID As Integer, ByVal VendorID As Integer) As AutomaticMessageVendorRecipientRow
            Dim row As AutomaticMessageVendorRecipientRow

            row = New AutomaticMessageVendorRecipientRow(DB, AutomaticMessageID, VendorID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AutomaticMessageID As Integer, ByVal VendorID As Integer)
            Dim row As AutomaticMessageVendorRecipientRow

            row = New AutomaticMessageVendorRecipientRow(DB, AutomaticMessageID, VendorID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AutomaticMessageVendorRecipient"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class AutomaticMessageVendorRecipientRowBase
        Private m_DB As Database
        Private m_AutomaticMessageID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_ReadDate As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing


        Public Property AutomaticMessageID() As Integer
            Get
                Return m_AutomaticMessageID
            End Get
            Set(ByVal Value As Integer)
                m_AutomaticMessageID = value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
            End Set
        End Property

        Public Property Created() As DateTime
            Get
                Return m_Created
            End Get
            Set(ByVal Value As DateTime)
                m_Created = value
            End Set
        End Property

        Public Property ReadDate() As DateTime
            Get
                Return m_ReadDate
            End Get
            Set(ByVal Value As DateTime)
                m_ReadDate = value
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

        Public Sub New(ByVal DB As Database, ByVal AutomaticMessageID As Integer, ByVal VendorID As Integer)
            m_DB = DB
            m_AutomaticMessageID = AutomaticMessageID
            m_VendorID = VendorID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AutomaticMessageVendorRecipient WHERE AutomaticMessageID = " & DB.Number(AutomaticMessageID) & " and VendorID=" & DB.Number(VendorID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AutomaticMessageID = Core.GetInt(r.Item("AutomaticMessageID"))
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_ReadDate = Core.GetDate(r.Item("ReadDate"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AutomaticMessageVendorRecipient (" _
             & " AutomaticMessageID" _
             & ",VendorID" _
             & ",Created" _
             & ",ReadDate" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.NullNumber(AutomaticMessageID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullQuote(Created) _
             & "," & m_DB.NullQuote(ReadDate) _
             & "," & CInt(IsActive) _
             & ")"

            AutomaticMessageID = m_DB.InsertSQL(SQL)

            Return AutomaticMessageID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AutomaticMessageVendorRecipient SET " _
             & " Created = " & m_DB.NullQuote(Created) _
             & ",ReadDate = " & m_DB.NullQuote(ReadDate) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE AutomaticMessageID = " & m_DB.Quote(AutomaticMessageID) _
             & " AND VendorID= " & DB.Number(VendorID)


            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AutomaticMessageVendorRecipient WHERE AutomaticMessageID = " & m_DB.Number(AutomaticMessageID) & " AND VendorID=" & DB.Number(VendorID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AutomaticMessageVendorRecipientCollection
        Inherits GenericCollection(Of AutomaticMessageVendorRecipientRow)
    End Class

End Namespace


