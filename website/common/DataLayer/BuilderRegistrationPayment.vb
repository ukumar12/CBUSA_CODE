Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class BuilderRegistrationPaymentRow
        Inherits BuilderRegistrationPaymentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal BuilderRegistrationPaymentID As Integer)
            MyBase.New(DB, BuilderRegistrationPaymentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal BuilderRegistrationPaymentID As Integer) As BuilderRegistrationPaymentRow
            Dim row As BuilderRegistrationPaymentRow

            row = New BuilderRegistrationPaymentRow(DB, BuilderRegistrationPaymentID)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRowByRegistration(ByVal DB As Database, ByVal BuilderRegistrationID As Integer) As BuilderRegistrationPaymentRow
            Dim row As BuilderRegistrationPaymentRow

            row = New BuilderRegistrationPaymentRow(DB)
            row.LoadByRegistration(BuilderRegistrationID)

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal BuilderRegistrationPaymentID As Integer)
            Dim row As BuilderRegistrationPaymentRow

            row = New BuilderRegistrationPaymentRow(DB, BuilderRegistrationPaymentID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from BuilderRegistrationPayment"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class BuilderRegistrationPaymentRowBase
        Private m_DB As Database
        Private m_BuilderRegistrationPaymentID As Integer = Nothing
        Private m_BuilderRegistrationID As Integer = Nothing
        Private m_CardholderName As String = Nothing
        Private m_CardTypeID As Integer = Nothing
        Private m_CardNumber As String = Nothing
        Private m_CIDNumber As String = Nothing
        Private m_ExpirationDate As String = Nothing
        Private m_ReferenceNumber As String = Nothing
        Private m_PaymentStatusID As Integer = Nothing
        Private m_Submitted As DateTime = Nothing


        Public Property BuilderRegistrationPaymentID() As Integer
            Get
                Return m_BuilderRegistrationPaymentID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderRegistrationPaymentID = value
            End Set
        End Property

        Public Property BuilderRegistrationID() As Integer
            Get
                Return m_BuilderRegistrationID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderRegistrationID = value
            End Set
        End Property

        Public Property CardholderName() As String
            Get
                Return m_CardholderName
            End Get
            Set(ByVal Value As String)
                m_CardholderName = value
            End Set
        End Property

        Public Property CardTypeID() As Integer
            Get
                Return m_CardTypeID
            End Get
            Set(ByVal Value As Integer)
                m_CardTypeID = value
            End Set
        End Property

        Public Property CardNumber() As String
            Get
                Return m_CardNumber
            End Get
            Set(ByVal Value As String)
                m_CardNumber = value
            End Set
        End Property

        Public Property CIDNumber() As String
            Get
                Return m_CIDNumber
            End Get
            Set(ByVal value As String)
                m_CIDNumber = value
            End Set
        End Property

        Public Property ExpirationDate() As String
            Get
                Return m_ExpirationDate
            End Get
            Set(ByVal Value As String)
                m_ExpirationDate = value
            End Set
        End Property

        Public Property ReferenceNumber() As String
            Get
                Return m_ReferenceNumber
            End Get
            Set(ByVal Value As String)
                m_ReferenceNumber = value
            End Set
        End Property

        Public Property PaymentStatusID() As Integer
            Get
                Return m_PaymentStatusID
            End Get
            Set(ByVal Value As Integer)
                m_PaymentStatusID = value
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

        Public Sub New(ByVal DB As Database, ByVal BuilderRegistrationPaymentID As Integer)
            m_DB = DB
            m_BuilderRegistrationPaymentID = BuilderRegistrationPaymentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM BuilderRegistrationPayment WHERE BuilderRegistrationPaymentID = " & DB.Number(BuilderRegistrationPaymentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub LoadByRegistration(ByVal BuilderRegistrationId As Integer)
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT Top 1 * FROM BuilderRegistrationPayment WHERE BuilderRegistrationID = " & DB.Number(BuilderRegistrationId) & " Order By Submitted DESC"
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_BuilderRegistrationPaymentID = Convert.ToInt32(r.Item("BuilderRegistrationPaymentID"))
            m_BuilderRegistrationID = Convert.ToInt32(r.Item("BuilderRegistrationID"))
            m_CardholderName = Convert.ToString(r.Item("CardholderName"))
            m_CardTypeID = Convert.ToInt32(r.Item("CardTypeID"))
            m_CardNumber = Convert.ToString(r.Item("CardNumber"))
            m_CIDNumber = Convert.ToString(r.Item("CIDNumber"))
            m_ExpirationDate = Convert.ToString(r.Item("ExpirationDate"))
            If IsDBNull(r.Item("ReferenceNumber")) Then
                m_ReferenceNumber = Nothing
            Else
                m_ReferenceNumber = Convert.ToString(r.Item("ReferenceNumber"))
            End If
            m_PaymentStatusID = Convert.ToInt32(r.Item("PaymentStatusID"))
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO BuilderRegistrationPayment (" _
             & " BuilderRegistrationID" _
             & ",CardholderName" _
             & ",CardTypeID" _
             & ",CardNumber" _
             & ",CIDNumber" _
             & ",ExpirationDate" _
             & ",ReferenceNumber" _
             & ",PaymentStatusID" _
             & ",Submitted" _
             & ") VALUES (" _
             & m_DB.NullNumber(BuilderRegistrationID) _
             & "," & m_DB.Quote(CardholderName) _
             & "," & m_DB.NullNumber(CardTypeID) _
             & "," & m_DB.Quote(CardNumber) _
             & "," & m_DB.Quote(CIDNumber) _
             & "," & m_DB.Quote(ExpirationDate) _
             & "," & m_DB.Quote(ReferenceNumber) _
             & "," & m_DB.NullNumber(PaymentStatusID) _
             & "," & m_DB.NullQuote(Submitted) _
             & ")"

            BuilderRegistrationPaymentID = m_DB.InsertSQL(SQL)

            Return BuilderRegistrationPaymentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE BuilderRegistrationPayment SET " _
             & " BuilderRegistrationID = " & m_DB.NullNumber(BuilderRegistrationID) _
             & ",CardholderName = " & m_DB.Quote(CardholderName) _
             & ",CardTypeID = " & m_DB.NullNumber(CardTypeID) _
             & ",CardNumber = " & m_DB.Quote(CardNumber) _
             & ",CIDNumber = " & m_DB.Quote(CIDNumber) _
             & ",ExpirationDate = " & m_DB.Quote(ExpirationDate) _
             & ",ReferenceNumber = " & m_DB.Quote(ReferenceNumber) _
             & ",PaymentStatusID = " & m_DB.NullNumber(PaymentStatusID) _
             & ",Submitted = " & m_DB.NullQuote(Submitted) _
             & " WHERE BuilderRegistrationPaymentID = " & m_DB.Quote(BuilderRegistrationPaymentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM BuilderRegistrationPayment WHERE BuilderRegistrationPaymentID = " & m_DB.Number(BuilderRegistrationPaymentID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class BuilderRegistrationPaymentCollection
        Inherits GenericCollection(Of BuilderRegistrationPaymentRow)
    End Class

End Namespace


