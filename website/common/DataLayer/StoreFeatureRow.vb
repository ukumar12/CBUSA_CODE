Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreFeatureRow
        Inherits StoreFeatureRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal FeatureId As Integer)
            MyBase.New(DB, FeatureId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal FeatureId As Integer) As StoreFeatureRow
            Dim row As StoreFeatureRow

            row = New StoreFeatureRow(DB, FeatureId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal FeatureId As Integer)
            Dim row As StoreFeatureRow

            row = New StoreFeatureRow(DB, FeatureId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetAllStoreFeatures(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select *, case when IsUnique  = 1 then Name + ' (unique)' else Name end as NameWithUniqueText from StoreFeature order by Name")
            Return dt
        End Function

    End Class

    Public MustInherit Class StoreFeatureRowBase
        Private m_DB As Database
        Private m_FeatureId As Integer = Nothing
        Private m_Name As String = Nothing
        Private m_IsUnique As Boolean = Nothing


        Public Property FeatureId() As Integer
            Get
                Return m_FeatureId
            End Get
            Set(ByVal Value As Integer)
                m_FeatureId = value
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

        Public Property IsUnique() As Boolean
            Get
                Return m_IsUnique
            End Get
            Set(ByVal Value As Boolean)
                m_IsUnique = value
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

        Public Sub New(ByVal DB As Database, ByVal FeatureId As Integer)
            m_DB = DB
            m_FeatureId = FeatureId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreFeature WHERE FeatureId = " & DB.Number(FeatureId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_FeatureId = Convert.ToInt32(r.Item("FeatureId"))
            m_Name = Convert.ToString(r.Item("Name"))
            m_IsUnique = Convert.ToBoolean(r.Item("IsUnique"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreFeature (" _
             & " Name" _
             & ",IsUnique" _
             & ") VALUES (" _
             & m_DB.Quote(Name) _
             & "," & CInt(IsUnique) _
             & ")"

            FeatureId = m_DB.InsertSQL(SQL)

            Return FeatureId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreFeature SET " _
             & " Name = " & m_DB.Quote(Name) _
             & ",IsUnique = " & CInt(IsUnique) _
             & " WHERE FeatureId = " & m_DB.quote(FeatureId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreFeature WHERE FeatureId = " & m_DB.Quote(FeatureId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreFeatureCollection
        Inherits GenericCollection(Of StoreFeatureRow)
    End Class

End Namespace


