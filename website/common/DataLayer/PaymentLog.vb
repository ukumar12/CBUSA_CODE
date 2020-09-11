Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PaymentLogRow
        Inherits PaymentLogRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal LogId As Integer)
            MyBase.New(DB, LogId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal LogId As Integer) As PaymentLogRow
            Dim row As PaymentLogRow

            row = New PaymentLogRow(DB, LogId)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRowByOrderNo(ByVal DB As Database, ByVal OrderNo As String) As PaymentLogRow
            Dim SQL As String = "select * from PaymentLog where OrderNo = " & DB.Quote(OrderNo)
            Dim r As SqlDataReader
            Dim row As PaymentLogRow = New PaymentLogRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal LogId As Integer)
            Dim row As PaymentLogRow

            row = New PaymentLogRow(DB, LogId)
            row.Remove()
        End Sub

        'Custom Methods
    End Class

    Public MustInherit Class PaymentLogRowBase
        Private m_DB As Database
        Private m_LogId As Integer = Nothing
        Private m_OrderNo As String = Nothing
        Private m_TransactionNo As String = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_Result As Integer = Nothing
        Private m_Amount As Double = Nothing
        Private m_Description As String = Nothing
        Private m_Response As String = Nothing
        Private m_VerificationResponse As String = Nothing
        Private m_IsHighRisk As Boolean = Nothing


        Public Property LogId() As Integer
            Get
                Return m_LogId
            End Get
            Set(ByVal Value As Integer)
                m_LogId = value
            End Set
        End Property

        Public Property OrderNo() As String
            Get
                Return m_OrderNo
            End Get
            Set(ByVal Value As String)
                m_OrderNo = Value
            End Set
        End Property

        Public Property TransactionNo() As String
            Get
                Return m_TransactionNo
            End Get
            Set(ByVal Value As String)
                m_TransactionNo = value
            End Set
        End Property

        Public Property Result() As Integer
            Get
                Return m_Result
            End Get
            Set(ByVal Value As Integer)
                m_Result = value
            End Set
        End Property

        Public Property Amount() As Double
            Get
                Return m_Amount
            End Get
            Set(ByVal Value As Double)
                m_Amount = value
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

        Public Property Response() As String
            Get
                Return m_Response
            End Get
            Set(ByVal Value As String)
                m_Response = value
            End Set
        End Property
        Public Property VerificationResponse() As String
            Get
                Return m_VerificationResponse
            End Get
            Set(ByVal Value As String)
                m_VerificationResponse = Value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property
        Public Property IsHighRisk() As Boolean
            Get
                Return m_IsHighRisk
            End Get
            Set(ByVal value As Boolean)
                m_IsHighRisk = value
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

        Public Sub New(ByVal DB As Database, ByVal LogId As Integer)
            m_DB = DB
            m_LogId = LogId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PaymentLog WHERE LogId = " & DB.Number(LogId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_LogId = Convert.ToInt32(r.Item("LogId"))
            m_OrderNo = Convert.ToString(r.Item("OrderNo"))
            m_TransactionNo = Convert.ToString(r.Item("TransactionNo"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_Result = Convert.ToInt32(r.Item("Result"))
            m_Amount = Convert.ToDouble(r.Item("Amount"))
            If IsDBNull(r.Item("Description")) Then
                m_Description = Nothing
            Else
                m_Description = Convert.ToString(r.Item("Description"))
            End If
            If IsDBNull(r.Item("Response")) Then
                m_Response = Nothing
            Else
                m_Response = Convert.ToString(r.Item("Response"))
            End If
            If IsDBNull(r.Item("VerificationResponse")) Then
                m_VerificationResponse = Nothing
            Else
                m_VerificationResponse = Convert.ToString(r.Item("VerificationResponse"))
            End If
            m_IsHighRisk = Convert.ToBoolean(r.Item("IsHighRisk"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PaymentLog (" _
             & " OrderNo" _
             & ",TransactionNo" _
             & ",CreateDate" _
             & ",Result" _
             & ",Amount" _
             & ",Description" _
             & ",Response" _
             & ",VerificationResponse" _
             & ",IsHighRisk" _
             & ") VALUES (" _
             & m_DB.Quote(OrderNo) _
             & "," & m_DB.Quote(TransactionNo) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Number(Result) _
             & "," & m_DB.Number(Amount) _
             & "," & m_DB.Quote(Description) _
             & "," & m_DB.Quote(Response) _
             & "," & m_DB.Quote(VerificationResponse) _
             & "," & CInt(IsHighRisk) _
             & ")"

            LogId = m_DB.InsertSQL(SQL)

            Return LogId
        End Function

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PaymentLog WHERE LogId = " & m_DB.Quote(LogId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PaymentLogCollection
        Inherits GenericCollection(Of PaymentLogRow)
    End Class

End Namespace


