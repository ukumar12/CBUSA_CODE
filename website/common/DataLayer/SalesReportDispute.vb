Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class SalesReportDisputeRow
        Inherits SalesReportDisputeRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal SalesReportDisputeID As Integer)
            MyBase.New(DB, SalesReportDisputeID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal SalesReportDisputeID As Integer) As SalesReportDisputeRow
            Dim row As SalesReportDisputeRow

            row = New SalesReportDisputeRow(DB, SalesReportDisputeID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal SalesReportDisputeID As Integer)
            Dim row As SalesReportDisputeRow

            row = New SalesReportDisputeRow(DB, SalesReportDisputeID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from SalesReportDispute"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetListByVendor(ByVal DB As Database, ByVal VendorID As Integer, ByVal PeriodQuarter As Integer, ByVal PeriodYear As Integer, Optional ByVal IncludeResponses As Boolean = True) As DataTable
            Dim sql As String = "select d.*, b.CompanyName as BuilderCompany from SalesReportDispute d inner join SalesReport r on d.SalesReportId=r.SalesReportID inner join Builder b on d.BuilderID=b.BuilderID where r.PeriodQuarter=" & PeriodQuarter & " and r.PeriodYear=" & PeriodYear & " and r.VendorID=" & DB.Number(VendorID)
            If Not IncludeResponses Then
                sql &= " and DisputeResponseId is null"
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetRowBySalesReportIdAndBuilderId(ByVal DB As Database, ByVal SalesReportID As Integer, ByVal BuilderId As Integer) As SalesReportDisputeRow
            Dim SQL As String = "SELECT * FROM SalesReportDispute WHERE SalesReportId = " & SalesReportID & " and BuilderId = " & BuilderId
            Dim r As SqlDataReader
            Dim row As SalesReportDisputeRow = New SalesReportDisputeRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function
    End Class

    Public MustInherit Class SalesReportDisputeRowBase
        Private m_DB As Database
        Private m_SalesReportDisputeID As Integer = Nothing
        Private m_SalesReportID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_BuilderTotalAmount As Double = Nothing
        Private m_VendorTotalAmount As Double = Nothing
        Private m_DisputeResponseID As Integer = Nothing
        Private m_DisputeResponseReasonID As Integer = Nothing
        Private m_ResolutionAmount As Double = Nothing
        Private m_BuilderComments As String = Nothing
        Private m_VendorComments As String = Nothing
        Private m_Created As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing


        Public Property SalesReportDisputeID() As Integer
            Get
                Return m_SalesReportDisputeID
            End Get
            Set(ByVal Value As Integer)
                m_SalesReportDisputeID = value
            End Set
        End Property

        Public Property SalesReportID() As Integer
            Get
                Return m_SalesReportID
            End Get
            Set(ByVal Value As Integer)
                m_SalesReportID = value
            End Set
        End Property

        Public Property BuilderID() As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property BuilderTotalAmount() As Double
            Get
                Return m_BuilderTotalAmount
            End Get
            Set(ByVal Value As Double)
                m_BuilderTotalAmount = value
            End Set
        End Property

        Public Property VendorTotalAmount() As Double
            Get
                Return m_VendorTotalAmount
            End Get
            Set(ByVal Value As Double)
                m_VendorTotalAmount = value
            End Set
        End Property

        Public Property DisputeResponseID() As Integer
            Get
                Return m_DisputeResponseID
            End Get
            Set(ByVal Value As Integer)
                m_DisputeResponseID = value
            End Set
        End Property

        Public Property DisputeResponseReasonID() As Integer
            Get
                Return m_DisputeResponseReasonID
            End Get
            Set(ByVal Value As Integer)
                m_DisputeResponseReasonID = value
            End Set
        End Property

        Public Property ResolutionAmount() As Double
            Get
                Return m_ResolutionAmount
            End Get
            Set(ByVal Value As Double)
                m_ResolutionAmount = value
            End Set
        End Property

        Public Property BuilderComments() As String
            Get
                Return m_BuilderComments
            End Get
            Set(ByVal Value As String)
                m_BuilderComments = value
            End Set
        End Property

        Public Property VendorComments() As String
            Get
                Return m_VendorComments
            End Get
            Set(ByVal Value As String)
                m_VendorComments = value
            End Set
        End Property

        Public ReadOnly Property Created() As DateTime
            Get
                Return m_Created
            End Get
        End Property
        Public Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
            End Get
            Set(ByVal Value As DateTime)
                m_ModifyDate = Value
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

        Public Sub New(ByVal DB As Database, ByVal SalesReportDisputeID As Integer)
            m_DB = DB
            m_SalesReportDisputeID = SalesReportDisputeID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM SalesReportDispute WHERE SalesReportDisputeID = " & DB.Number(SalesReportDisputeID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_SalesReportDisputeID = Convert.ToInt32(r.Item("SalesReportDisputeID"))
            m_SalesReportID = Convert.ToInt32(r.Item("SalesReportID"))
            m_BuilderID = Convert.ToInt32(r.Item("BuilderID"))
            If IsDBNull(r.Item("BuilderTotalAmount")) Then
                m_BuilderTotalAmount = Nothing
            Else
                m_BuilderTotalAmount = Convert.ToDouble(r.Item("BuilderTotalAmount"))
            End If
            If IsDBNull(r.Item("VendorTotalAmount")) Then
                m_VendorTotalAmount = Nothing
            Else
                m_VendorTotalAmount = Convert.ToDouble(r.Item("VendorTotalAmount"))
            End If
            If IsDBNull(r.Item("DisputeResponseID")) Then
                m_DisputeResponseID = Nothing
            Else
                m_DisputeResponseID = Convert.ToInt32(r.Item("DisputeResponseID"))
            End If
            If IsDBNull(r.Item("DisputeResponseReasonID")) Then
                m_DisputeResponseReasonID = Nothing
            Else
                m_DisputeResponseReasonID = Convert.ToInt32(r.Item("DisputeResponseReasonID"))
            End If
            If IsDBNull(r.Item("ResolutionAmount")) Then
                m_ResolutionAmount = Nothing
            Else
                m_ResolutionAmount = Convert.ToDouble(r.Item("ResolutionAmount"))
            End If
            If IsDBNull(r.Item("BuilderComments")) Then
                m_BuilderComments = Nothing
            Else
                m_BuilderComments = Convert.ToString(r.Item("BuilderComments"))
            End If
            If IsDBNull(r.Item("VendorComments")) Then
                m_VendorComments = Nothing
            Else
                m_VendorComments = Convert.ToString(r.Item("VendorComments"))
            End If
            m_Created = Convert.ToDateTime(r.Item("Created"))
            If IsDBNull(r.Item("ModifyDate")) Then
                m_ModifyDate = Nothing
            Else
                m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO SalesReportDispute (" _
             & " SalesReportID" _
             & ",BuilderID" _
             & ",BuilderTotalAmount" _
             & ",VendorTotalAmount" _
             & ",DisputeResponseID" _
             & ",DisputeResponseReasonID" _
             & ",ResolutionAmount" _
             & ",BuilderComments" _
             & ",VendorComments" _
             & ",Created" _
             & ",ModifyDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(SalesReportID) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.Number(BuilderTotalAmount) _
             & "," & m_DB.Number(VendorTotalAmount) _
             & "," & m_DB.NullNumber(DisputeResponseID) _
             & "," & m_DB.NullNumber(DisputeResponseReasonID) _
             & "," & m_DB.NullNumber(ResolutionAmount) _
             & "," & m_DB.Quote(BuilderComments) _
             & "," & m_DB.Quote(VendorComments) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(ModifyDate) _
             & ")"

            SalesReportDisputeID = m_DB.InsertSQL(SQL)

            Return SalesReportDisputeID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE SalesReportDispute SET " _
             & " SalesReportID = " & m_DB.NullNumber(SalesReportID) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",BuilderTotalAmount = " & m_DB.Number(BuilderTotalAmount) _
             & ",VendorTotalAmount = " & m_DB.Number(VendorTotalAmount) _
             & ",DisputeResponseID = " & m_DB.NullNumber(DisputeResponseID) _
             & ",DisputeResponseReasonID = " & m_DB.NullNumber(DisputeResponseReasonID) _
             & ",ResolutionAmount = " & m_DB.Number(ResolutionAmount) _
             & ",BuilderComments = " & m_DB.Quote(BuilderComments) _
             & ",VendorComments = " & m_DB.Quote(VendorComments) _
             & ",ModifyDate = " & m_DB.Quote(ModifyDate) _
             & " WHERE SalesReportDisputeID = " & m_DB.Quote(SalesReportDisputeID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM SalesReportDispute WHERE SalesReportDisputeID = " & m_DB.Number(SalesReportDisputeID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class SalesReportDisputeCollection
        Inherits GenericCollection(Of SalesReportDisputeRow)
    End Class

End Namespace


