Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class NCPContentRow
        Inherits NCPContentRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, NCPContentID As Integer)
            MyBase.New(DB, NCPContentID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal NCPContentID As Integer) As NCPContentRow
            Dim row As NCPContentRow

            row = New NCPContentRow(DB, NCPContentID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal NCPContentID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM NCPContent WHERE NCPContentID = " & DB.Number(NCPContentID)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, NCPContentID)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from NCPContent"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public ReadOnly Property GetSelectedLLCs() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select LLCID from NCPContentLLC where NCPContentID = " & NCPContentID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("LLCID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllLLCs()
            DB.ExecuteSQL("delete from NCPContentLLC where NCPContentID = " & NCPContentID)
        End Sub
        Public Sub InsertToLLCs(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToLLC(Element)
            Next
        End Sub
        Public Sub InsertToLLC(ByVal LLCID As Integer)
            Dim SQL As String = "insert into NCPContentLLC (NCPContentID, LLCID) values (" & NCPContentID & "," & LLCID & ")"
            DB.ExecuteSQL(SQL)
        End Sub
        Public ReadOnly Property GetSelectedContracts() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select ContractID from NCPContentContract where NCPContentID = " & NCPContentID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("ContractID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property
        Public Sub DeleteFromAllContracts()
            DB.ExecuteSQL("delete from NCPContentContract where NCPContentID = " & NCPContentID)
        End Sub

        Public Sub InsertToContentContracts(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                InsertToContentContract(Element)
            Next
        End Sub
        Public Sub InsertToContentContract(ByVal ContractID As Integer)
            Dim SQL As String = "insert into NCPContentContract (NCPContentID, ContractID) values (" & NCPContentID & "," & ContractID & ")"
            DB.ExecuteSQL(SQL)
        End Sub


    End Class
    Public MustInherit Class NCPContentRowBase
        Private m_DB As Database
        Private m_NCPContentID As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_Description As String = Nothing
        Private m_IsActive As Boolean = Nothing

        Public Property NCPContentID As Integer
            Get
                Return m_NCPContentID
            End Get
            Set(ByVal Value As Integer)
                m_NCPContentID = Value
            End Set
        End Property

        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(ByVal Value As String)
                m_Name = Value
            End Set
        End Property

        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(ByVal Value As String)
                m_Description = Value
            End Set
        End Property

        Public Property IsActive As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = Value
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

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, NCPContentID As Integer)
            m_DB = DB
            m_NCPContentID = NCPContentID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM NCPContent WHERE NCPContentID = " & DB.Number(NCPContentID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_NCPContentID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_NCPContentID = Core.GetInt(r.Item("NCPContentID"))
            m_Name = Core.GetString(r.Item("Name"))
            m_Description = Core.GetString(r.Item("Description"))
            m_IsActive = Core.GetBoolean(r.Item("IsActive"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO NCPContent (" _
             & " Name" _
             & ",Description" _
             & ",IsActive" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & m_DB.Quote(Description) _
             & "," & CInt(IsActive) _
             & ")"

            NCPContentID = m_DB.InsertSQL(SQL)

            Return NCPContentID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE NCPContent WITH (ROWLOCK) SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",Description = " & m_DB.Quote(Description) _
             & ",IsActive = " & CInt(IsActive) _
             & " WHERE NCPContentID = " & m_DB.Quote(NCPContentID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class NCPContentCollection
        Inherits GenericCollection(Of NCPContentRow)
    End Class

End Namespace


