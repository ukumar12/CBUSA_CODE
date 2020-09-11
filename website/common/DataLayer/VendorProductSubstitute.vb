Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorProductSubstituteRow
        Inherits VendorProductSubstituteRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer)
            MyBase.New(DB, VendorID, ProductID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer) As VendorProductSubstituteRow
            Dim row As VendorProductSubstituteRow

            row = New VendorProductSubstituteRow(DB, VendorID, ProductID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer)
            Dim row As VendorProductSubstituteRow

            row = New VendorProductSubstituteRow(DB, VendorID, ProductID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorProductSubstitute"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class VendorProductSubstituteRowBase
        Private m_DB As Database
        Private m_VendorID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SubstituteProductID As Integer = Nothing
        Private m_QuantityMultiplier As Double = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorVendorAccountID As Integer = Nothing


        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
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

        Public Property SubstituteProductID() As Integer
            Get
                Return m_SubstituteProductID
            End Get
            Set(ByVal Value As Integer)
                m_SubstituteProductID = value
            End Set
        End Property

        Public Property QuantityMultiplier() As Double
            Get
                Return m_QuantityMultiplier
            End Get
            Set(ByVal Value As Double)
                m_QuantityMultiplier = Value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property CreatorVendorAccountID() As Integer
            Get
                Return m_CreatorVendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorVendorAccountID = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal ProductID As Integer)
            m_DB = DB
            m_VendorID = VendorID
            m_ProductID = ProductID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorProductSubstitute WHERE VendorID = " & DB.Number(VendorID) & " and ProductID=" & DB.Number(ProductID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_ProductID = Core.GetInt(r.Item("ProductID"))
            m_SubstituteProductID = Core.GetInt(r.Item("SubstituteProductID"))
            m_QuantityMultiplier = Core.GetDouble(r.Item("QuantityMultiplier"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_CreatorVendorAccountID = Core.GetInt(r.Item("CreatorVendorAccountID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorProductSubstitute (" _
             & " VendorID" _
             & ",ProductID" _
             & ",SubstituteProductID" _
             & ",QuantityMultiplier" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(SubstituteProductID) _
             & "," & m_DB.Number(QuantityMultiplier) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & ")"

            DB.ExecuteSQL(SQL)

            Return VendorID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorProductSubstitute SET " _
             & " SubstituteProductID = " & m_DB.NullNumber(SubstituteProductID) _
             & ",QuantityMultiplier = " & m_DB.Number(QuantityMultiplier) _
             & " WHERE VendorID = " & m_DB.Quote(VendorID) _
             & " AND ProductID = " & m_DB.Quote(ProductID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorProductSubstitute WHERE VendorID = " & m_DB.Number(VendorID) & " AND ProductID=" & m_DB.Number(ProductID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorProductSubstituteCollection
        Inherits GenericCollection(Of VendorProductSubstituteRow)
    End Class

End Namespace


