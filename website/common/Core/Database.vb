Imports System
Imports System.ComponentModel
Imports System.Collections
Imports System.Diagnostics
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports Components

Public Class Database
    Private con As SqlConnection = Nothing
    Private tran As SqlTransaction = Nothing
    Private RefCount As Integer = 0

    Public Sub New()
    End Sub

    Public Sub New(ByVal connection As SqlConnection, ByVal transaction As SqlTransaction)
        con = connection
        tran = transaction
    End Sub

    Public Property Connection() As SqlConnection
        Get
            Return con
        End Get
        Set(ByVal value As SqlConnection)
            con = value
        End Set
    End Property

    Public Property Transaction() As SqlTransaction
        Get
            Return tran
        End Get
        Set(ByVal value As SqlTransaction)
            tran = value
        End Set
    End Property

    Public Sub Open(ByVal connectionstring As String)
        RefCount = RefCount + 1

        If con Is Nothing Then
            con = New SqlConnection(connectionstring)
        End If
        If Not IsOpen() Then
            con.Open()
        End If
    End Sub

    Public Sub Close()
        If Not con Is Nothing Then
            RefCount = RefCount - 1
            If RefCount = 0 Then
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
                con = Nothing
            End If
        End If
    End Sub

    Public Sub Dispose()
        RefCount = 0
        If Not con Is Nothing Then
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            con = Nothing
        End If
    End Sub

    Public Function IsOpen() As Boolean
        If Not con Is Nothing Then
            If con.State = ConnectionState.Open Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function Quote(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            Return "'" + input.Replace("'", "''") + "'"
        End If
    End Function

    Public Function NQuote(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            Return "N'" + input.Replace("'", "''") + "'"
        End If
    End Function

    Public Function Number(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            Try
                Return CDbl(input)
            Catch ex As Exception
                Return "NULL"
            End Try
        End If
    End Function

    Public Function NullNumber(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            If input = 0 Then
                Return "NULL"
            Else
                Return input
            End If
        End If
    End Function


    Public Function NullNumber(ByVal input As Integer) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            If input = 0 Then
                Return "NULL"
            Else
                Return input
            End If
        End If
    End Function

    Public Function NullNumber(ByVal input As Double) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            If input = 0 Then
                Return "NULL"
            Else
                Return input
            End If
        End If
    End Function

    Public Function NullQuote(ByVal input As Date) As String
        If input = Nothing Then
            Return "NULL"
        Else
            Return Quote(input.ToString())
        End If
    End Function

    Public Function NullQuote(ByVal input As Integer) As String
        If input = 0 Then
            Return "NULL"
        Else
            Return Quote(input.ToString())
        End If
    End Function

    Public Function Quote(ByVal input As DateTime) As String
        If input = DateTime.MinValue Then
            Return "NULL"
        Else
            Return Quote(input.ToString())
        End If
    End Function

    Public Function QuoteMultiple(ByVal input As String()) As String
        If input.Length = 0 Then
            Return "(NULL)"
        Else
            Dim i As Integer
            For i = 0 To input.Length - 1 Step i + 1
                input(i) = Quote(input(i))
            Next
            Return "(" + String.Join(",", input) + ")"
        End If
    End Function

    Public Function QuoteMultiple(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "(NULL)"
        Else
            Dim aMultiple() As String = input.Split(","c)
            Dim i As Integer
            For i = 0 To aMultiple.Length - 1 Step i + 1
                aMultiple(i) = Quote(aMultiple(i))
            Next
            Return "(" + String.Join(",", aMultiple) + ")"
        End If
    End Function

    Public Function NQuoteMultiple(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "(NULL)"
        Else
            Dim aMultiple() As String = input.Split(","c)
            Dim i As Integer
            For i = 0 To aMultiple.Length - 1 Step i + 1
                aMultiple(i) = NQuote(aMultiple(i))
            Next
            Return "(" + String.Join(",", aMultiple) + ")"
        End If
    End Function

    Public Function NumberMultiple(ByVal input As String) As String
        Return QuoteMultiple(input)
    End Function

    Public Function NumberMultiple(ByVal input() As String) As String
        If input.Length = 0 Then
            Return "(NULL)"
        Else
            Dim i As Integer
            For i = 0 To input.Length - 1 Step i + 1
                input(i) = Number(input(i))
            Next
            Return "(" + String.Join(",", input) + ")"
        End If
    End Function

    Public Function FilterQuote(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            Return "'%" + input.Replace("'", "''") + "%'"
        End If
    End Function

    Public Function StartsWith(ByVal input As String) As String
        If IsEmpty(input) Then
            Return "NULL"
        Else
            Return "'" + input.Replace("'", "''") + "%'"
        End If
    End Function

    Public Function IsEmpty(ByVal input As String) As Boolean
        If input = String.Empty Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub BeginTransaction()
        If tran Is Nothing Then
            tran = con.BeginTransaction()
        End If
    End Sub

    Public Sub CommitTransaction()
        If Not tran Is Nothing Then
            tran.Commit()
            tran = Nothing
        End If
    End Sub

    Public Sub RollbackTransaction()
        If Not tran Is Nothing Then
            tran.Rollback()
            tran = Nothing
        End If
    End Sub

    Public Function RunProc(ByVal procName As String) As Integer
        Logger.Info(procName)
        Dim cmd As SqlCommand = CreateCommand(procName, Nothing)
        cmd.ExecuteNonQuery()
        Return CType(cmd.Parameters("ReturnValue").Value, Integer)
    End Function

    Public Function RunProc(ByVal procName As String, ByVal prams() As SqlParameter) As Integer
        Logger.Info(procName)
        Dim cmd As SqlCommand = CreateCommand(procName, prams)
        cmd.ExecuteNonQuery()
        Return CType(cmd.Parameters("ReturnValue").Value, Integer)
    End Function

    Public Sub RunProc(ByVal procName As String, ByRef dataReader As SqlDataReader)
        Logger.Info(procName)
        Dim cmd As SqlCommand = CreateCommand(procName, Nothing)
        dataReader = cmd.ExecuteReader()
    End Sub

    Public Sub RunProc(ByVal procName As String, ByVal prams() As SqlParameter, ByRef dataReader As SqlDataReader)
        Logger.Info(procName)
        Dim cmd As SqlCommand = CreateCommand(procName, prams)
        dataReader = cmd.ExecuteReader()
    End Sub

    Public Sub RunProc(ByVal procName As String, ByRef dsDataSet As DataSet)
        Logger.Info(procName)
        Dim cmd As SqlCommand = CreateCommand(procName, Nothing)
        Dim daDataAdapter As SqlDataAdapter = New SqlDataAdapter(cmd)
        dsDataSet = New DataSet()
        daDataAdapter.Fill(dsDataSet)
    End Sub

    Public Sub RunProc(ByVal procName As String, ByVal prams() As SqlParameter, ByRef dsDataSet As DataSet)
        Logger.Info(procName)
        Dim cmd As SqlCommand = CreateCommand(procName, prams)
        Dim daDataAdapter As SqlDataAdapter = New SqlDataAdapter(cmd)
        dsDataSet = New DataSet()
        daDataAdapter.Fill(dsDataSet)
    End Sub

    Public Sub RunProc(ByVal procName As String, ByVal prams() As SqlParameter, ByRef dt As DataTable)
        Logger.Info(procName)
        Dim cmd As SqlCommand = CreateCommand(procName, prams)
        If Not tran Is Nothing Then
            cmd.Transaction = tran
        End If
        Dim myReader As SqlDataReader = cmd.ExecuteReader()
        dt.Load(myReader)
    End Sub

    Public Function GetReader(ByVal SQL As String) As SqlDataReader
        Logger.Info(SQL)
        Dim cmd As SqlCommand
        If Not tran Is Nothing Then
            cmd = New SqlCommand(SQL, con, tran)
        Else
            cmd = New SqlCommand(SQL, con)
        End If
        Dim myReader As SqlDataReader = cmd.ExecuteReader()
        Return myReader
    End Function
    Public Function GetReader(ByVal SQL As String, ByVal Timeout As Integer) As SqlDataReader
        Logger.Info("Begin GetReader: " & SQL)
        Dim cmd As SqlCommand = Nothing
        Dim myReader As SqlDataReader = Nothing

        If Not tran Is Nothing Then
            cmd = New SqlCommand(SQL, con, tran)
        Else
            cmd = New SqlCommand(SQL, con)
        End If
        cmd.CommandTimeout = Timeout
        myReader = cmd.ExecuteReader()



        Return myReader
    End Function
    Public Function ExecuteScalar(ByVal SQL As String) As Object
        Logger.Info(SQL)
        Dim cmd As SqlCommand
        If Not tran Is Nothing Then
            cmd = New SqlCommand(SQL, con, tran)
        Else
            cmd = New SqlCommand(SQL, con)
        End If
        Return cmd.ExecuteScalar()
    End Function

    Public Function ExecuteScalar(ByVal SQL As String, ByVal Timeout As Integer) As Object
        Logger.Info(SQL)
        Dim cmd As SqlCommand
        If Not tran Is Nothing Then
            cmd = New SqlCommand(SQL, con, tran)
        Else
            cmd = New SqlCommand(SQL, con)
        End If
        cmd.CommandTimeout = Timeout
        Return cmd.ExecuteScalar()
    End Function

    Public Function GetDataSet(ByVal SQL As String) As DataSet
        Dim ds As New DataSet
        ds.Tables.Add(GetDataTable(SQL))
        Return ds
    End Function

    Public Function GetDataTable(ByVal SQL As String) As DataTable
        Dim dt As DataTable = New DataTable()
        Dim dr As SqlDataReader = GetReader(SQL)
        dt.Load(dr)
        dr.Close()
        Return dt
    End Function
    Public Function GetDataTable(ByVal SQL As String, ByVal Timeout As Integer) As DataTable
        Dim dt As DataTable = New DataTable()
        Dim dr As SqlDataReader = GetReader(SQL, Timeout)
        dt.Load(dr)
        dr.Close()
        Return dt
    End Function
    Public Function InsertSQL(ByVal SQL As String) As Integer
        SQL = "SET NOCOUNT ON; " + SQL + "; SELECT SCOPE_IDENTITY() AS NewId; SET NOCOUNT OFF;"
        Logger.Info(SQL)
        Dim cmd As SqlCommand
        If Not tran Is Nothing Then
            cmd = New SqlCommand(SQL, con, tran)
        Else
            cmd = New SqlCommand(SQL, con)
        End If
        Dim result As Integer
        Dim myReader As SqlDataReader = cmd.ExecuteReader()
        If myReader.Read() Then
            result = Convert.ToInt32(myReader("NewId"))
        Else
            result = 0
        End If
        myReader.Close()

        Return result
    End Function

    Public Function ExecuteSQL(ByVal SQL As String) As Integer
        Logger.Info(SQL)
        Dim cmd As SqlCommand
        If Not tran Is Nothing Then
            cmd = New SqlCommand(SQL, con, tran)
        Else
            cmd = New SqlCommand(SQL, con)
        End If
        Return cmd.ExecuteNonQuery()
    End Function

    Public Function ExecuteSQL(ByVal SQL As String, ByVal Timeout As Integer) As Integer
        Logger.Info(SQL)
        Dim cmd As SqlCommand
        If Not tran Is Nothing Then
            cmd = New SqlCommand(SQL, con, tran)
        Else
            cmd = New SqlCommand(SQL, con)
        End If
        cmd.CommandTimeout = Timeout
        Return cmd.ExecuteNonQuery()
    End Function

    Private Function CreateCommand(ByVal procName As String, ByVal prams() As SqlParameter) As SqlCommand
        Dim cmd As SqlCommand
        If Not tran Is Nothing Then
            cmd = New SqlCommand(procName, con, tran)
        Else
            cmd = New SqlCommand(procName, con)
        End If
        cmd.CommandType = CommandType.StoredProcedure

        ' add proc parameters
        If Not prams Is Nothing Then
            Dim parameter As SqlParameter
            For Each parameter In prams
                cmd.Parameters.Add(parameter)
            Next
        End If

        cmd.Parameters.Add(New SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, False, 0, 0, String.Empty, DataRowVersion.Default, Nothing))
        Return cmd
    End Function

    Public Function InParam(ByVal ParamName As String, ByVal DbType As SqlDbType, ByVal Size As Integer, ByVal Value As Object) As SqlParameter
        Return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value)
    End Function

    Public Function OutParam(ByVal ParamName As String, ByVal DbType As SqlDbType, ByVal Size As Integer) As SqlParameter
        Return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, Nothing)
    End Function

    Public Function MakeParam(ByVal ParamName As String, ByVal DbType As SqlDbType, ByVal Size As Int32, ByVal Direction As ParameterDirection, ByVal Value As Object) As SqlParameter
        Dim param As SqlParameter

        If Size > 0 Then
            param = New SqlParameter(ParamName, DbType, Size)
        Else
            param = New SqlParameter(ParamName, DbType)
        End If

        param.Direction = Direction
        If Not (Direction = ParameterDirection.Output And Value Is Nothing) Then
            param.Value = Value
        End If

        Return param
    End Function

    Public Shared Function GetRowsRange(ByVal dt As DataTable, ByVal First As Integer, ByVal Last As Integer) As DataTable
        Dim table As DataTable = dt.Clone()
        Dim i As Integer
        For i = First To Last
            If i >= dt.Rows.Count Then
                Exit For
            Else
                table.ImportRow(dt.Rows(i))
            End If
        Next
        Return table
    End Function
End Class