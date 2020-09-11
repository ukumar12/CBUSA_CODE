Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PriceComparisonVendorSummaryRow
        Inherits PriceComparisonVendorSummaryRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PriceComparisonID As Integer, ByVal VendorId As Integer)
            MyBase.New(DB, PriceComparisonID, VendorId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PriceComparisonID As Integer, ByVal VendorId As Integer) As PriceComparisonVendorSummaryRow
            Dim row As PriceComparisonVendorSummaryRow

            row = New PriceComparisonVendorSummaryRow(DB, PriceComparisonID, VendorId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PriceComparisonID As Integer, ByVal VendorId As Integer)
            Dim row As PriceComparisonVendorSummaryRow

            row = New PriceComparisonVendorSummaryRow(DB, PriceComparisonID, VendorID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PriceComparisonVendorSummary"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetVendors(ByVal DB As Database, ByVal PriceComparisonID As Integer) As PriceComparisonVendorSummaryCollection
            Dim out As New PriceComparisonVendorSummaryCollection()
            Dim sql As String = "select pcvs.* from PriceComparisonVendorSummary pcvs Inner Join Vendor v On pcvs.VendorId = v.VendorId where v.IsActive = 1 And pcvs.PriceComparisonID=" & DB.Number(PriceComparisonID)
            Dim sdr As SqlDataReader = DB.GetReader(sql)
            While sdr.Read
                Dim row As New PriceComparisonVendorSummaryRow(DB)
                row.Load(sdr)
                out.Add(row)
            End While
            sdr.Close()
            Return out
        End Function
    End Class

    Public MustInherit Class PriceComparisonVendorSummaryRowBase
        Private m_DB As Database
        Private m_PriceComparisonID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_Subtotal As Double = Nothing
        Private m_Tax As Double = Nothing
        Private m_Total As Double = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing

        Public Property PriceComparisonID() As Integer
            Get
                Return m_PriceComparisonID
            End Get
            Set(ByVal Value As Integer)
                m_PriceComparisonID = value
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

        Public Property Subtotal() As Double
            Get
                Return m_Subtotal
            End Get
            Set(ByVal Value As Double)
                m_Subtotal = value
            End Set
        End Property

        Public Property Tax() As Double
            Get
                Return m_Tax
            End Get
            Set(ByVal Value As Double)
                m_Tax = value
            End Set
        End Property

        Public Property Total() As Double
            Get
                Return m_Total
            End Get
            Set(ByVal Value As Double)
                m_Total = value
            End Set
        End Property


        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
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
            Set(ByVal Value As DataBase)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PriceComparisonID As Integer, ByVal VendorId As Integer)
            m_DB = DB
            m_PriceComparisonID = PriceComparisonID
            m_VendorID = VendorId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PriceComparisonVendorSummary WHERE PriceComparisonID = " & DB.Number(PriceComparisonID) & " and VendorId=" & DB.Number(VendorID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_VendorID = Core.GetInt(r("VendorID"))
            m_PriceComparisonID = Core.GetInt(r("PriceComparisonID"))
            m_Subtotal = Convert.ToDouble(r.Item("Subtotal"))
            m_Tax = Convert.ToDouble(r.Item("Tax"))
            m_Total = Convert.ToDouble(r.Item("Total"))
            m_CreateDate = Core.GetDate(r.Item("CreateDate"))
            m_ModifyDate = Core.GetDate(r.Item("ModifyDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            SQL = " INSERT INTO PriceComparisonVendorSummary (" _
             & " VendorId" _
             & ",PriceComparisonId" _
             & ",Subtotal" _
             & ",Tax" _
             & ",Total" _
             & ",ModifyDate" _
             & ",CreateDate" _
             & ") VALUES (" _
             & m_DB.Quote(VendorID) _
             & "," & m_DB.Quote(PriceComparisonID) _
             & "," & m_DB.Number(Subtotal) _
             & "," & m_DB.Number(Tax) _
             & "," & m_DB.Number(Total) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & ")"

            m_DB.ExecuteSQL(SQL)

            Return PriceComparisonID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PriceComparisonVendorSummary SET " _
             & " Subtotal = " & m_DB.Number(Subtotal) _
             & ",Tax = " & m_DB.Number(Tax) _
             & ",Total = " & m_DB.Number(Total) _
             & ",ModifyDate = " & m_DB.NullQuote(Now) _
             & " WHERE PriceComparisonID = " & m_DB.Quote(PriceComparisonID) _
             & " AND VendorId = " & m_DB.Quote(VendorID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PriceComparisonVendorProductPrice WHERE PriceComparisonID=" & m_DB.Number(PriceComparisonID) & " and VendorID=" & DB.Number(VendorID)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM PriceComparisonVendorSummary WHERE PriceComparisonID = " & m_DB.Number(PriceComparisonID) & " and VendorID=" & DB.Number(VendorID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class PriceComparisonVendorSummaryCollection
        Inherits GenericCollection(Of PriceComparisonVendorSummaryRow)
    End Class

End Namespace


