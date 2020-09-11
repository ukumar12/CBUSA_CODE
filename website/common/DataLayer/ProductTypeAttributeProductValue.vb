Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class ProductTypeAttributeProductValueRow
        Inherits ProductTypeAttributeProductValueRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal ProductTypeAttributeID As Integer, ByVal ProductID As Integer)
            MyBase.New(DB, ProductTypeAttributeID, ProductID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal ProductTypeAttributeID As Integer, ByVal ProductId As Integer) As ProductTypeAttributeProductValueRow
            Dim row As ProductTypeAttributeProductValueRow

            row = New ProductTypeAttributeProductValueRow(DB, ProductTypeAttributeID, ProductId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal ProductTypeAttributeID As Integer, ByVal ProductID As Integer)
            Dim row As ProductTypeAttributeProductValueRow

            row = New ProductTypeAttributeProductValueRow(DB, ProductTypeAttributeID, ProductID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from ProductTypeAttributeProductValue"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class ProductTypeAttributeProductValueRowBase
        Private m_DB As Database
        Private m_ProductTypeAttributeID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_Value As String = Nothing


        Public Property ProductTypeAttributeID() As Integer
            Get
                Return m_ProductTypeAttributeID
            End Get
            Set(ByVal Value As Integer)
                m_ProductTypeAttributeID = value
            End Set
        End Property

        Public Property ProductID() As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return m_Value
            End Get
            Set(ByVal Value As String)
                m_Value = value
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

        Public Sub New(ByVal DB As Database, ByVal ProductTypeAttributeID As Integer, ByVal ProductID As Integer)
            m_DB = DB
            m_ProductTypeAttributeID = ProductTypeAttributeID
            m_ProductID = ProductID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM ProductTypeAttributeProductValue WHERE ProductTypeAttributeID = " & DB.Number(ProductTypeAttributeID) & " AND ProductId=" & DB.Number(ProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_Value = Convert.ToString(r.Item("Value"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO ProductTypeAttributeProductValue (" _
            & " ProductTypeAttributeId" _
             & ",ProductID" _
             & ",Value" _
             & ") VALUES (" _
             & m_DB.Number(ProductTypeAttributeID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.Quote(Value) _
             & ")"

            Return IIf(DB.ExecuteSQL(SQL), ProductTypeAttributeID, Nothing)
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE ProductTypeAttributeProductValue SET " _
             & " Value = " & m_DB.Quote(Value) _
             & " WHERE ProductTypeAttributeID = " & m_DB.Quote(ProductTypeAttributeID) _
             & " AND ProductID = " & m_DB.Quote(ProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM ProductTypeAttributeProductValue WHERE ProductTypeAttributeID = " & m_DB.Number(ProductTypeAttributeID) & " AND ProductID=" & DB.Number(ProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class ProductTypeAttributeProductValueCollection
        Inherits GenericCollection(Of ProductTypeAttributeProductValueRow)
    End Class

End Namespace


