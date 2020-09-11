Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorContractRow
        Inherits VendorContractRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorContractID As Integer)
            MyBase.New(DB, VendorContractID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorContractID As Integer) As VendorContractRow
            Dim row As VendorContractRow

            row = New VendorContractRow(DB, VendorContractID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorContractID As Integer)
            Dim row As VendorContractRow

            row = New VendorContractRow(DB, VendorContractID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorContract"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class VendorContractRowBase
        Private m_DB As Database
        Private m_VendorContractID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_TermsAcceptance As Boolean = Nothing
        Private m_Notes As String = Nothing
        Private m_Submitted As DateTime = Nothing


        Public Property VendorContractID() As Integer
            Get
                Return m_VendorContractID
            End Get
            Set(ByVal Value As Integer)
                m_VendorContractID = value
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

        Public Property FirstName() As String
            Get
                Return m_FirstName
            End Get
            Set(ByVal Value As String)
                m_FirstName = value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return m_LastName
            End Get
            Set(ByVal Value As String)
                m_LastName = value
            End Set
        End Property

        Public Property TermsAcceptance() As Boolean
            Get
                Return m_TermsAcceptance
            End Get
            Set(ByVal Value As Boolean)
                m_TermsAcceptance = value
            End Set
        End Property

        Public Property Notes() As String
            Get
                Return m_Notes
            End Get
            Set(ByVal Value As String)
                m_Notes = value
            End Set
        End Property

        Public Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
            Set(ByVal Value As DateTime)
                m_Submitted = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorContractID As Integer)
            m_DB = DB
            m_VendorContractID = VendorContractID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorContract WHERE VendorContractID = " & DB.Number(VendorContractID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorContractID = Convert.ToInt32(r.Item("VendorContractID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_TermsAcceptance = Convert.ToBoolean(r.Item("TermsAcceptance"))
            If IsDBNull(r.Item("Notes")) Then
                m_Notes = Nothing
            Else
                m_Notes = Convert.ToString(r.Item("Notes"))
            End If
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorContract (" _
             & " VendorID" _
             & ",FirstName" _
             & ",LastName" _
             & ",TermsAcceptance" _
             & ",Notes" _
             & ",Submitted" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & CInt(TermsAcceptance) _
             & "," & m_DB.Quote(Notes) _
             & "," & m_DB.NullQuote(Submitted) _
             & ")"

            VendorContractID = m_DB.InsertSQL(SQL)

            Return VendorContractID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorContract SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",TermsAcceptance = " & CInt(TermsAcceptance) _
             & ",Notes = " & m_DB.Quote(Notes) _
             & ",Submitted = " & m_DB.NullQuote(Submitted) _
             & " WHERE VendorContractID = " & m_DB.quote(VendorContractID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorContract WHERE VendorContractID = " & m_DB.Number(VendorContractID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorContractCollection
        Inherits GenericCollection(Of VendorContractRow)
    End Class

End Namespace


