Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class CustomRebateRow
        Inherits CustomRebateRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal CustomRebateID As Integer)
            MyBase.New(DB, CustomRebateID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal CustomRebateID As Integer) As CustomRebateRow
            Dim row As CustomRebateRow

            row = New CustomRebateRow(DB, CustomRebateID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal CustomRebateID As Integer)
            Dim row As CustomRebateRow

            row = New CustomRebateRow(DB, CustomRebateID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from CustomRebate"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Function GetListByVendor(ByVal DB As Database, ByVal VendorID As Integer) As DataTable
            Dim sql As String = "select * from CustomRebate where VendorID=" & DB.Number(VendorID)
            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class CustomRebateRowBase
        Private m_DB As Database
        Private m_CustomRebateID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_CustomRebateProgramID As Integer = Nothing
        Private m_RebateYear As Integer = Nothing
        Private m_RebateQuarter As Integer = Nothing
        Private m_MinimumPurchase As Double = Nothing
        Private m_RebatePercentage As Integer = Nothing
        Private m_ApplicablePurchaseAmount As Double = Nothing
        Private m_RebateAmount As Double = Nothing
        Private m_Details As String = Nothing


        Public Property CustomRebateID() As Integer
            Get
                Return m_CustomRebateID
            End Get
            Set(ByVal Value As Integer)
                m_CustomRebateID = value
            End Set
        End Property

        Public Property VendorID() As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property CustomRebateProgramID() As Integer
            Get
                Return m_CustomRebateProgramID
            End Get
            Set(ByVal Value As Integer)
                m_CustomRebateProgramID = value
            End Set
        End Property

        Public Property RebateYear() As Integer
            Get
                Return m_RebateYear
            End Get
            Set(ByVal Value As Integer)
                m_RebateYear = value
            End Set
        End Property

        Public Property RebateQuarter() As Integer
            Get
                Return m_RebateQuarter
            End Get
            Set(ByVal Value As Integer)
                m_RebateQuarter = value
            End Set
        End Property

        Public Property MinimumPurchase() As Double
            Get
                Return m_MinimumPurchase
            End Get
            Set(ByVal Value As Double)
                m_MinimumPurchase = value
            End Set
        End Property

        Public Property RebatePercentage() As Integer
            Get
                Return m_RebatePercentage
            End Get
            Set(ByVal Value As Integer)
                m_RebatePercentage = value
            End Set
        End Property

        Public Property ApplicablePurchaseAmount() As Double
            Get
                Return m_ApplicablePurchaseAmount
            End Get
            Set(ByVal Value As Double)
                m_ApplicablePurchaseAmount = value
            End Set
        End Property

        Public Property RebateAmount() As Double
            Get
                Return m_RebateAmount
            End Get
            Set(ByVal Value As Double)
                m_RebateAmount = value
            End Set
        End Property

        Public Property Details() As String
            Get
                Return m_Details
            End Get
            Set(ByVal Value As String)
                m_Details = value
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

        Public Sub New(ByVal DB As Database, ByVal CustomRebateID As Integer)
            m_DB = DB
            m_CustomRebateID = CustomRebateID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM CustomRebate WHERE CustomRebateID = " & DB.Number(CustomRebateID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_CustomRebateID = Core.GetInt(r.Item("CustomRebateID"))
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_BuilderID = Core.GetInt(r.Item("BuilderID"))
            m_CustomRebateProgramID = Core.GetInt(r.Item("CustomRebateProgramID"))
            m_RebateYear = Core.GetInt(r.Item("RebateYear"))
            m_RebateQuarter = Core.GetInt(r.Item("RebateQuarter"))
            m_MinimumPurchase = Core.GetDouble(r.Item("MinimumPurchase"))
            m_RebatePercentage = Core.GetInt(r.Item("RebatePercentage"))
            m_ApplicablePurchaseAmount = Core.GetDouble(r.Item("ApplicablePurchaseAmount"))
            m_RebateAmount = Core.GetDouble(r.Item("RebateAmount"))
            m_Details = Core.GetString(r.Item("Details"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO CustomRebate (" _
             & " VendorID" _
             & ",BuilderID" _
             & ",CustomRebateProgramID" _
             & ",RebateYear" _
             & ",RebateQuarter" _
             & ",MinimumPurchase" _
             & ",RebatePercentage" _
             & ",ApplicablePurchaseAmount" _
             & ",RebateAmount" _
             & ",Details" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(CustomRebateProgramID) _
             & "," & m_DB.Number(RebateYear) _
             & "," & m_DB.Number(RebateQuarter) _
             & "," & m_DB.Number(MinimumPurchase) _
             & "," & m_DB.Number(RebatePercentage) _
             & "," & m_DB.Number(ApplicablePurchaseAmount) _
             & "," & m_DB.Number(RebateAmount) _
             & "," & m_DB.Quote(Details) _
             & ")"

            CustomRebateID = m_DB.InsertSQL(SQL)

            Return CustomRebateID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE CustomRebate SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",CustomRebateProgramID = " & m_DB.NullNumber(CustomRebateProgramID) _
             & ",RebateYear = " & m_DB.Number(RebateYear) _
             & ",RebateQuarter = " & m_DB.Number(RebateQuarter) _
             & ",MinimumPurchase = " & m_DB.Number(MinimumPurchase) _
             & ",RebatePercentage = " & m_DB.Number(RebatePercentage) _
             & ",ApplicablePurchaseAmount = " & m_DB.Number(ApplicablePurchaseAmount) _
             & ",RebateAmount = " & m_DB.Number(RebateAmount) _
             & ",Details = " & m_DB.Quote(Details) _
             & " WHERE CustomRebateID = " & m_DB.quote(CustomRebateID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM CustomRebate WHERE CustomRebateID = " & m_DB.Number(CustomRebateID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class CustomRebateCollection
        Inherits GenericCollection(Of CustomRebateRow)
    End Class

End Namespace


