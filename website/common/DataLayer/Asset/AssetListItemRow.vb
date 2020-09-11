Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class AssetListItemRow
        Inherits AssetListItemRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AssetListItemId As Integer)
            MyBase.New(DB, AssetListItemId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AssetListItemId As Integer) As AssetListItemRow
            Dim row As AssetListItemRow

            row = New AssetListItemRow(DB, AssetListItemId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AssetListItemId As Integer)
            Dim row As AssetListItemRow

            row = New AssetListItemRow(DB, AssetListItemId)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from AssetListItem"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        
        Public Shared Sub DeleteByListItemId(ByVal DB As Database, ByVal ListItemId As Integer)
            DB.ExecuteSQL("Delete From dbo.AssetListItem Where [ListItemId] = " & DB.Number(ListItemId))
        End Sub

        Public Shared Function GetAssetDirectory(ByVal DB As Database, ByVal ListItemId As Integer) As DataTable
            Dim dt As DataTable
            Dim sql As New StringBuilder()

            sql.Append("Select a.OriginalFile, a.AssetDirectory, a.FileName From dbo.Asset a Inner Join dbo.AssetListItem al On al.AssetId = a.AssetId ")
            sql.Append("Inner Join dbo.ListItem li On li.ListItemId = al.ListItemId And li.ListItemId = " & DB.Number(ListItemId))

            dt = DB.GetDataTable(sql.ToString())

            Return dt
        End Function
    End Class

    Public MustInherit Class AssetListItemRowBase
        Private m_DB As Database
        Private m_AssetListItemId As Integer = Nothing
        Private m_AssetId As Integer = Nothing
        Private m_ListItemId As Integer = Nothing


        Public Property AssetListItemId() As Integer
            Get
                Return m_AssetListItemId
            End Get
            Set(ByVal Value As Integer)
                m_AssetListItemId = value
            End Set
        End Property

        Public Property AssetId() As Integer
            Get
                Return m_AssetId
            End Get
            Set(ByVal Value As Integer)
                m_AssetId = value
            End Set
        End Property

        Public Property ListItemId() As Integer
            Get
                Return m_ListItemId
            End Get
            Set(ByVal Value As Integer)
                m_ListItemId = value
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

        Public Sub New(ByVal DB As Database, ByVal AssetListItemId As Integer)
            m_DB = DB
            m_AssetListItemId = AssetListItemId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM AssetListItem WHERE AssetListItemId = " & DB.Number(AssetListItemId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AssetListItemId = Convert.ToInt32(r.Item("AssetListItemId"))
            m_AssetId = Convert.ToInt32(r.Item("AssetId"))
            m_ListItemId = Convert.ToInt32(r.Item("ListItemId"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO AssetListItem (" _
             & " AssetId" _
             & ",ListItemId" _
             & ") VALUES (" _
             & m_DB.NullNumber(AssetId) _
             & "," & m_DB.NullNumber(ListItemId) _
             & ")"

            AssetListItemId = m_DB.InsertSQL(SQL)

            Return AssetListItemId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE AssetListItem SET " _
             & " AssetId = " & m_DB.NullNumber(AssetId) _
             & ",ListItemId = " & m_DB.NullNumber(ListItemId) _
             & " WHERE AssetListItemId = " & m_DB.quote(AssetListItemId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM AssetListItem WHERE AssetListItemId = " & m_DB.Number(AssetListItemId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class AssetListItemCollection
        Inherits GenericCollection(Of AssetListItemRow)
    End Class

End Namespace


