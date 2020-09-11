Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ExportQueueRow
        Inherits ExportQueueRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ExportQueueId As Integer)
            MyBase.New(DB, ExportQueueId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ExportQueueId As Integer) As ExportQueueRow
            Dim row As ExportQueueRow

            row = New ExportQueueRow(DB, ExportQueueId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ExportQueueId As Integer)
            Dim row As ExportQueueRow

            row = New ExportQueueRow(DB, ExportQueueId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from ExportQueue"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetActiveListByAccountId(ByVal DB As Database, ByVal IdValue As Integer, ByVal IdField As String) As DataTable
            Dim SQL As String = "select * from ExportQueue where " & Core.ProtectParam(IdField) & " = " & DB.Number(IdValue) & " and ProcessDate is null order by CreateDate desc"
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function Add(ByVal DB As Database, ByVal IdValue As Integer, ByVal IdField As String, ByVal ExportCode As String, ByVal Parameters As String) As Integer
            Dim dbExportQueue As New ExportQueueRow(DB)
            Select Case IdField
                Case "AdminId"
                    dbExportQueue.AdminId = IdValue
                Case "VendorAccountId"
                    dbExportQueue.VendorAccountId = IdValue
                Case "BuilderAccountId"
                    dbExportQueue.BuilderAccountId = IdValue
                Case "PIQAccountId"
                    dbExportQueue.PIQAccountId = IdValue
            End Select
            dbExportQueue.ExportCode = ExportCode
            dbExportQueue.Parameters = Parameters
            Return dbExportQueue.Insert
        End Function

    End Class

    Public MustInherit Class ExportQueueRowBase
        Private m_DB As Database
        Private m_ExportQueueId As Integer = Nothing
        Private m_AdminId As Integer = Nothing
        Private m_VendorAccountId As Integer = Nothing
        Private m_BuilderAccountId As Integer = Nothing
        Private m_PIQAccountId As Integer = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ExportCode As String = Nothing
        Private m_Parameters As String = Nothing
        Private m_ProcessDate As DateTime = Nothing


        Public Property ExportQueueId() As Integer
            Get
                Return m_ExportQueueId
            End Get
            Set(ByVal Value As Integer)
                m_ExportQueueId = Value
            End Set
        End Property

        Public Property AdminId() As Integer
            Get
                Return m_AdminId
            End Get
            Set(ByVal Value As Integer)
                m_AdminId = Value
            End Set
        End Property

        Public Property VendorAccountId() As Integer
            Get
                Return m_VendorAccountId
            End Get
            Set(ByVal Value As Integer)
                m_VendorAccountId = Value
            End Set
        End Property

        Public Property BuilderAccountId() As Integer
            Get
                Return m_BuilderAccountId
            End Get
            Set(ByVal Value As Integer)
                m_BuilderAccountId = Value
            End Set
        End Property

        Public Property PIQAccountId() As Integer
            Get
                Return m_PIQAccountId
            End Get
            Set(ByVal Value As Integer)
                m_PIQAccountId = Value
            End Set
        End Property

        Public Property ExportCode() As String
            Get
                Return m_ExportCode
            End Get
            Set(ByVal Value As String)
                m_ExportCode = Value
            End Set
        End Property

        Public Property Parameters() As String
            Get
                Return m_Parameters
            End Get
            Set(ByVal Value As String)
                m_Parameters = Value
            End Set
        End Property

        Public Property ProcessDate() As DateTime
            Get
                Return m_ProcessDate
            End Get
            Set(ByVal Value As DateTime)
                m_ProcessDate = Value
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
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ExportQueueId As Integer)
            m_DB = DB
            m_ExportQueueId = ExportQueueId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ExportQueue WHERE ExportQueueId = " & DB.Number(ExportQueueId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_ExportQueueId = Convert.ToInt32(r.Item("ExportQueueId"))
            If IsDBNull(r.Item("AdminId")) Then
                m_AdminId = Nothing
            Else
                m_AdminId = Convert.ToInt32(r.Item("AdminId"))
            End If
            If IsDBNull(r.Item("VendorAccountID")) Then
                m_VendorAccountId = Nothing
            Else
                m_VendorAccountId = Convert.ToInt32(r.Item("VendorAccountID"))
            End If
            If IsDBNull(r.Item("BuilderAccountID")) Then
                m_BuilderAccountId = Nothing
            Else
                m_BuilderAccountId = Convert.ToInt32(r.Item("BuilderAccountID"))
            End If
            If IsDBNull(r.Item("PIQAccountID")) Then
                m_PIQAccountId = Nothing
            Else
                m_PIQAccountId = Convert.ToInt32(r.Item("PIQAccountID"))
            End If
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(r.Item("ExportCode")) Then
                m_ExportCode = Nothing
            Else
                m_ExportCode = Convert.ToString(r.Item("ExportCode"))
            End If
            If IsDBNull(r.Item("Parameters")) Then
                m_Parameters = Nothing
            Else
                m_Parameters = Convert.ToString(r.Item("Parameters"))
            End If
            If IsDBNull(r.Item("ProcessDate")) Then
                m_ProcessDate = Nothing
            Else
                m_ProcessDate = Convert.ToDateTime(r.Item("ProcessDate"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO ExportQueue (" _
             & " AdminId" _
             & ",VendorAccountId" _
             & ",BuilderAccountId" _
             & ",PIQAccountId" _
             & ",CreateDate" _
             & ",ExportCode" _
             & ",Parameters" _
             & ",ProcessDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(AdminId) _
             & "," & m_DB.NullNumber(VendorAccountId) _
             & "," & m_DB.NullNumber(BuilderAccountId) _
             & "," & m_DB.NullNumber(PIQAccountId) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.Quote(ExportCode) _
             & "," & m_DB.Quote(Parameters) _
             & "," & m_DB.NullQuote(ProcessDate) _
             & ")"

            ExportQueueId = m_DB.InsertSQL(SQL)

            Return ExportQueueId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ExportQueue SET " _
             & " AdminId = " & m_DB.NullNumber(AdminId) _
             & ",VendorAccountId = " & m_DB.NullNumber(VendorAccountId) _
             & ",BuilderAccountId = " & m_DB.NullNumber(BuilderAccountId) _
             & ",PIQAccountId = " & m_DB.NullNumber(PIQAccountId) _
             & ",ExportCode = " & m_DB.Quote(ExportCode) _
             & ",Parameters = " & m_DB.Quote(Parameters) _
             & ",ProcessDate = " & m_DB.NullQuote(ProcessDate) _
             & " WHERE ExportQueueId = " & m_DB.Quote(ExportQueueId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ExportQueue WHERE ExportQueueId = " & m_DB.Number(ExportQueueId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ExportQueueCollection
        Inherits GenericCollection(Of ExportQueueRow)
    End Class

End Namespace