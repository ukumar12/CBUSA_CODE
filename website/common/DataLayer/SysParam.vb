Option Explicit On 

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports System.Web
Imports Components

Namespace DataLayer

    Public Class SysParam
        Private Const CacheKey As String = "SysparamCollection"

        Public Shared Function GetValue(ByVal DB As Database, ByVal ParamName As String) As String
            Dim ParamCollection As SysparamCollection = Nothing

            Dim context As HttpContext = HttpContext.Current
            If Not context Is Nothing Then
                ParamCollection = CType(context.Cache(SysParam.CacheKey), SysparamCollection)
            End If
            If ParamCollection Is Nothing Then
                ParamCollection = SysparamRow.GetCollection(DB)
                If Not context Is Nothing Then
                    context.Cache.Insert(SysParam.CacheKey, ParamCollection, Nothing, DateTime.Now.AddSeconds(10), TimeSpan.Zero)
                End If
            End If
            For Each row As SysparamRow In ParamCollection
                If row.Name = ParamName Then
                    Return row.Value
                End If
            Next
            Return Nothing
        End Function
    End Class

    Public Class SysparamRow
        Inherits SysparamRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ParamId As Integer)
            MyBase.New(DB, ParamId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ParamId As Integer) As SysparamRow
            Dim row As SysparamRow

            row = New SysparamRow(DB, ParamId)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRow(ByVal DB As Database, ByVal ParamName As String) As SysparamRow
            Dim row As SysparamRow = New SysparamRow(DB)
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Sysparam WHERE Name = " & DB.Quote(ParamName)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ParamId As Integer)
            Dim row As SysparamRow

            row = New SysparamRow(DB, ParamId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetList(ByVal DB As Database, ByVal IsInternal As Boolean) As DataTable
            Dim SQL As String = "SELECT * FROM sysparam " & IIf(IsInternal, "", "WHERE IsInternal = 0 ") & " ORDER BY GroupName, SortOrder"
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetShippingList(ByVal DB As Database, ByVal IsInternal As Boolean) As DataTable
            Dim SQL As String = "SELECT * FROM sysparam where GroupName='Shipping' " & IIf(IsInternal, "", "And IsInternal = 0 ") & " ORDER BY GroupName, SortOrder"
            Return DB.GetDataTable(SQL)
        End Function

        Public Shared Function GetCollection(ByVal DB As Database) As SysparamCollection
            Dim SQL As String
            Dim r As SqlDataReader
            Dim collection As New SysparamCollection
            Dim row As SysparamRow

            SQL = "select * from Sysparam"
            r = DB.GetReader(SQL)
            While r.Read()
                row = New SysparamRow(DB)
                row.Load(r)
                collection.Add(row)
            End While
            r.Close()
            Return collection
        End Function

    End Class

    Public MustInherit Class SysparamRowBase
        Private m_DB As Database
        Private m_ParamId As Integer = Nothing
        Private m_GroupName As String = Nothing
        Private m_Name As String = Nothing
        Private m_Value As String = Nothing
        Private m_Type As String = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_IsInternal As Boolean = Nothing
        Private m_IsEncrypted As Boolean = Nothing
        Private m_Comments As String = Nothing

        Public Property ParamId() As Integer
            Get
                Return m_ParamId
            End Get
            Set(ByVal Value As Integer)
                m_ParamId = Value
            End Set
        End Property

        Public Property GroupName() As String
            Get
                Return m_GroupName
            End Get
            Set(ByVal Value As String)
                m_GroupName = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = Value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return m_Value
            End Get
            Set(ByVal Value As String)
                m_Value = Value
            End Set
        End Property

        Public ReadOnly Property EncryptedValue() As String
            Get
                If IsEncrypted Then
                    Return Utility.Crypt.EncryptTripleDes(Value)
                End If
                Return Value
            End Get
        End Property

        Public Property Type() As String
            Get
                Return m_Type
            End Get
            Set(ByVal Value As String)
                m_Type = Value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = Value
            End Set
        End Property

        Public Property IsInternal() As Boolean
            Get
                Return m_IsInternal
            End Get
            Set(ByVal Value As Boolean)
                m_IsInternal = Value
            End Set
        End Property

        Public Property IsEncrypted() As Boolean
            Get
                Return m_IsEncrypted
            End Get
            Set(ByVal Value As Boolean)
                m_IsEncrypted = Value
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return m_Comments
            End Get
            Set(ByVal Value As String)
                m_Comments = Value
            End Set
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

        Public Sub New(ByVal database As Database)
            m_DB = database
        End Sub 'New

        Public Sub New(ByVal database As Database, ByVal ParamId As Integer)
            m_DB = database
            m_ParamId = ParamId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM Sysparam WHERE ParamId = " & DB.Quote(ParamId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_ParamId = Convert.ToInt32(r.Item("ParamId"))
            m_GroupName = Convert.ToString(r.Item("GroupName"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_Type = Convert.ToString(r.Item("Type"))
            m_SortOrder = Convert.ToInt32(r.Item("SortOrder"))
            m_IsInternal = Convert.ToBoolean(r.Item("IsInternal"))
            m_IsEncrypted = Convert.ToBoolean(r.Item("IsEncrypted"))
            m_Comments = Convert.ToString(r.Item("Comments"))
            If IsEncrypted Then
                m_Value = Utility.Crypt.DecryptTripleDes(IIf(IsDBNull(r.Item("Value")), String.Empty, r.Item("Value")))
            Else
                m_Value = Convert.ToString(r.Item("Value"))
            End If
        End Sub 'Load

        Public Overridable Sub Insert()
            Dim SQL As String

            SQL = " INSERT INTO Sysparam (" _
             & " GroupName" _
             & ",Name" _
             & ",Value" _
             & ",Type" _
             & ",SortOrder" _
             & ",IsInternal" _
             & ",IsEncrypted" _
             & ",Comments" _
             & ") VALUES (" _
             & m_DB.Quote(GroupName) _
             & "," & m_DB.Quote(Name) _
             & "," & m_DB.Quote(EncryptedValue) _
             & "," & m_DB.Quote(Type) _
             & "," & m_DB.Quote(SortOrder) _
             & "," & CInt(IsInternal) _
             & "," & CInt(IsEncrypted) _
             & "," & m_DB.Quote(Comments) _
             & ")"

            m_DB.ExecuteSQL(SQL)
        End Sub 'Insert

        Function AutoInsert() As Integer
            Dim SQL As String = "SELECT SCOPE_IDENTITY()"

            Insert()
            Return m_DB.ExecuteScalar(SQL)
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE Sysparam SET " _
             & " GroupName = " & m_DB.Quote(GroupName) _
             & ",Name = " & m_DB.Quote(Name) _
             & ",Value = " & m_DB.Quote(EncryptedValue) _
             & ",Type = " & m_DB.Quote(Type) _
             & ",SortOrder = " & m_DB.Quote(SortOrder) _
             & ",IsInternal = " & CInt(IsInternal) _
             & ",IsEncrypted = " & CInt(IsEncrypted) _
             & ",Comments = " & m_DB.Quote(Comments) _
             & " WHERE ParamId = " & m_DB.Quote(ParamId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM Sysparam WHERE ParamId = " & m_DB.Quote(ParamId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SysparamCollection
        Inherits GenericCollection(Of SysparamRow)
    End Class

End Namespace


