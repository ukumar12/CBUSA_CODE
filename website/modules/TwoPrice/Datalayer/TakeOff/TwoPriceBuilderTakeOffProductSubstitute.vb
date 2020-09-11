
Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class TwoPriceBuilderTakeOffProductSubstituterow
        Inherits TwoPriceBuilderTakeOffProductSubstituteBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer)
            MyBase.New(DB, VendorID, TwoPriceBuilderTakeOffProductPendingID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer) As TwoPriceBuilderTakeOffProductSubstituterow
            Dim row As TwoPriceBuilderTakeOffProductSubstituterow

            row = New TwoPriceBuilderTakeOffProductSubstituterow(DB, VendorID, TwoPriceBuilderTakeOffProductPendingID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorID As Integer, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer)
            Dim row As TwoPriceBuilderTakeOffProductSubstituterow

            row = New TwoPriceBuilderTakeOffProductSubstituterow(DB, VendorID, TwoPriceBuilderTakeOffProductPendingID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceBuilderTakeOffProductSubstitute"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

    End Class

    Public MustInherit Class TwoPriceBuilderTakeOffProductSubstituteBase
        Private m_DB As Database
        Private m_VendorID As Integer = Nothing
        Private m_TwoPriceBuilderTakeOffProductPendingID As Integer = Nothing
        Private m_SubstituteProductID As Integer = Nothing
        Private m_RecommendedQuantity As Integer = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorVendorAccountID As Integer = Nothing


        Public Property VendorID As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = Value
            End Set
        End Property

        Public Property TwoPriceBuilderTakeOffProductPendingID As Integer
            Get
                Return m_TwoPriceBuilderTakeOffProductPendingID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceBuilderTakeOffProductPendingID = Value
            End Set
        End Property

        Public Property SubstituteProductID As Integer
            Get
                Return m_SubstituteProductID
            End Get
            Set(ByVal Value As Integer)
                m_SubstituteProductID = Value
            End Set
        End Property

        Public Property RecommendedQuantity As Integer
            Get
                Return m_RecommendedQuantity
            End Get
            Set(ByVal Value As Integer)
                m_RecommendedQuantity = Value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property

        Public Property CreatorVendorAccountID As Integer
            Get
                Return m_CreatorVendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorVendorAccountID = Value
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

        Public Sub New(ByVal DB As Database, ByVal VendorID As Integer, ByVal TwoPriceBuilderTakeOffProductPendingID As Integer)
            m_DB = DB
            m_VendorID = VendorID
            m_TwoPriceBuilderTakeOffProductPendingID = TwoPriceBuilderTakeOffProductPendingID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceBuilderTakeOffProductSubstitute WHERE VendorID = " & DB.Number(VendorID) & " and TwoPriceBuilderTakeOffProductPendingID=" & DB.Number(TwoPriceBuilderTakeOffProductPendingID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_TwoPriceBuilderTakeOffProductPendingID = Core.GetInt(r.Item("TwoPriceBuilderTakeOffProductPendingID"))
            m_SubstituteProductID = Core.GetInt(r.Item("SubstituteProductID"))
            m_RecommendedQuantity = Core.GetInt(r.Item("RecommendedQuantity"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_CreatorVendorAccountID = Core.GetInt(r.Item("CreatorVendorAccountID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TwoPriceBuilderTakeOffProductSubstitute (" _
             & " VendorID" _
             & ",TwoPriceBuilderTakeOffProductPendingID" _
             & ",SubstituteProductID" _
             & ",RecommendedQuantity" _
             & ",Created" _
             & ",CreatorVendorAccountID" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(TwoPriceBuilderTakeOffProductPendingID) _
             & "," & m_DB.NullNumber(SubstituteProductID) _
             & "," & m_DB.Number(RecommendedQuantity) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(CreatorVendorAccountID) _
             & ")"

            m_DB.ExecuteSQL(SQL)

            Return VendorID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceBuilderTakeOffProductSubstitute SET " _
             & " SubstituteProductID = " & m_DB.NullNumber(SubstituteProductID) _
             & ",RecommendedQuantity = " & m_DB.Number(RecommendedQuantity) _
             & " WHERE VendorID = " & m_DB.Quote(VendorID) _
             & " AND TwoPriceBuilderTakeOffProductPendingID = " & m_DB.Quote(TwoPriceBuilderTakeOffProductPendingID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceBuilderTakeOffProductSubstitute WHERE VendorID = " & m_DB.Number(VendorID) & " and TwoPriceBuilderTakeOffProductPendingID = " & m_DB.Number(TwoPriceBuilderTakeOffProductPendingID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class TwoPriceBuilderTakeOffProductSubstituteCollection
        Inherits GenericCollection(Of TwoPriceBuilderTakeOffProductSubstituterow)
    End Class

End Namespace

