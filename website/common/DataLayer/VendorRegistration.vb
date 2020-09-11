Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class VendorRegistrationRow
        Inherits VendorRegistrationRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal VendorRegistrationID As Integer)
            MyBase.New(DB, VendorRegistrationID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal VendorRegistrationID As Integer) As VendorRegistrationRow
            Dim row As VendorRegistrationRow

            row = New VendorRegistrationRow(DB, VendorRegistrationID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal VendorRegistrationID As Integer)
            Dim row As VendorRegistrationRow

            row = New VendorRegistrationRow(DB, VendorRegistrationID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from VendorRegistration"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods
        Public Shared Function GetRowByVendor(ByVal DB As Database, ByVal VendorID As Integer, Optional ByVal CurrentYear As Integer = Nothing) As VendorRegistrationRow
            Dim row As New VendorRegistrationRow(DB)
            Dim sql As String = "select top 1 * from VendorRegistration where RegistrationStatusId > 0 And VendorID=" & DB.Number(VendorID)
            If CurrentYear <> Nothing Then
                sql &= " and datepart(yy,CompleteDate) = " & DB.Number(CurrentYear)
            End If
            sql &= " order by Updated desc"

            Dim sdr As SqlDataReader = DB.GetReader(sql)
            If sdr.Read Then
                row.Load(sdr)
            End If
            sdr.Close()

            Return row
        End Function

    End Class

    Public MustInherit Class VendorRegistrationRowBase
        Private m_DB As Database
        Private m_VendorRegistrationID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_HistoricVendorID As Integer = Nothing
        Private m_YearStarted As Integer = Nothing
        Private m_Employees As Integer = Nothing
        Private m_ProductsOffered As String = Nothing
        Private m_CompanyMemberships As String = Nothing
        Private m_IsSubsidiary As Boolean = Nothing
        Private m_SubsidiaryExplanation As String = Nothing
        Private m_HadLawsuit As Boolean = Nothing
        Private m_LawsuitExplanation As String = Nothing
        Private m_SupplyArea As String = Nothing
        Private m_PrimarySupplyPhaseID As Integer = Nothing
        Private m_SecondarySupplyPhaseID As Integer = Nothing
        Private m_PreparerFirstName As String = Nothing
        Private m_PreparerLastName As String = Nothing
        Private m_AcceptsTerms As Boolean = Nothing
        Private m_Notes As String = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_RegistrationStatusID As Integer = Nothing
        Private m_ApprovingAdminID As Integer = Nothing
        Private m_Approved As DateTime = Nothing
        Private m_BusinessType As Integer = Nothing
        Private m_CompleteDate As DateTime = Nothing

        Public Property VendorRegistrationID() As Integer
            Get
                Return m_VendorRegistrationID
            End Get
            Set(ByVal Value As Integer)
                m_VendorRegistrationID = value
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

        Public Property HistoricVendorID() As Integer
            Get
                Return m_HistoricVendorID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricVendorID = value
            End Set
        End Property

        Public Property YearStarted() As Integer
            Get
                Return m_YearStarted
            End Get
            Set(ByVal Value As Integer)
                m_YearStarted = value
            End Set
        End Property

        Public Property Employees() As Integer
            Get
                Return m_Employees
            End Get
            Set(ByVal Value As Integer)
                m_Employees = value
            End Set
        End Property

        Public Property ProductsOffered() As String
            Get
                Return m_ProductsOffered
            End Get
            Set(ByVal Value As String)
                m_ProductsOffered = value
            End Set
        End Property

        Public Property CompanyMemberships() As String
            Get
                Return m_CompanyMemberships
            End Get
            Set(ByVal Value As String)
                m_CompanyMemberships = value
            End Set
        End Property

        Public Property IsSubsidiary() As Boolean
            Get
                Return m_IsSubsidiary
            End Get
            Set(ByVal Value As Boolean)
                m_IsSubsidiary = value
            End Set
        End Property

        Public Property SubsidiaryExplanation() As String
            Get
                Return m_SubsidiaryExplanation
            End Get
            Set(ByVal Value As String)
                m_SubsidiaryExplanation = value
            End Set
        End Property

        Public Property HadLawsuit() As Boolean
            Get
                Return m_HadLawsuit
            End Get
            Set(ByVal Value As Boolean)
                m_HadLawsuit = value
            End Set
        End Property

        Public Property LawsuitExplanation() As String
            Get
                Return m_LawsuitExplanation
            End Get
            Set(ByVal Value As String)
                m_LawsuitExplanation = value
            End Set
        End Property

        Public Property SupplyArea() As String
            Get
                Return m_SupplyArea
            End Get
            Set(ByVal Value As String)
                m_SupplyArea = value
            End Set
        End Property

        Public Property PrimarySupplyPhaseID() As Integer
            Get
                Return m_PrimarySupplyPhaseID
            End Get
            Set(ByVal Value As Integer)
                m_PrimarySupplyPhaseID = value
            End Set
        End Property

        Public Property SecondarySupplyPhaseID() As Integer
            Get
                Return m_SecondarySupplyPhaseID
            End Get
            Set(ByVal Value As Integer)
                m_SecondarySupplyPhaseID = value
            End Set
        End Property

        Public Property PreparerFirstName() As String
            Get
                Return m_PreparerFirstName
            End Get
            Set(ByVal Value As String)
                m_PreparerFirstName = value
            End Set
        End Property

        Public Property PreparerLastName() As String
            Get
                Return m_PreparerLastName
            End Get
            Set(ByVal Value As String)
                m_PreparerLastName = value
            End Set
        End Property

        Public Property AcceptsTerms() As Boolean
            Get
                Return m_AcceptsTerms
            End Get
            Set(ByVal Value As Boolean)
                m_AcceptsTerms = value
            End Set
        End Property

        Public Property Notes() As String
            Get
                Return m_Notes
            End Get
            Set(ByVal Value As String)
                m_Notes = value
            End Set
        End Property

        Public Property BusinessType() As Integer
            Get
                Return m_BusinessType
            End Get
            Set(ByVal value As Integer)
                m_BusinessType = value
            End Set
        End Property

        Public ReadOnly Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
        End Property

        Public ReadOnly Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
        End Property

        Public Property RegistrationStatusID() As Integer
            Get
                Return m_RegistrationStatusID
            End Get
            Set(ByVal Value As Integer)
                m_RegistrationStatusID = value
            End Set
        End Property

        Public Property ApprovingAdminID() As Integer
            Get
                Return m_ApprovingAdminID
            End Get
            Set(ByVal Value As Integer)
                m_ApprovingAdminID = value
            End Set
        End Property

        Public Property Approved() As DateTime
            Get
                Return m_Approved
            End Get
            Set(ByVal Value As DateTime)
                m_Approved = value
            End Set
        End Property

        Public Property CompleteDate() As DateTime
            Get
                Return m_CompleteDate
            End Get
            Set(ByVal value As DateTime)
                m_CompleteDate = value
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

        Public Sub New(ByVal DB As Database, ByVal VendorRegistrationID As Integer)
            m_DB = DB
            m_VendorRegistrationID = VendorRegistrationID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM VendorRegistration WHERE VendorRegistrationID = " & DB.Number(VendorRegistrationID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_VendorRegistrationID = Convert.ToInt32(r.Item("VendorRegistrationID"))
            m_VendorID = Convert.ToInt32(r.Item("VendorID"))
            If IsDBNull(r.Item("HistoricVendorID")) Then
                m_HistoricVendorID = Nothing
            Else
                m_HistoricVendorID = Convert.ToInt32(r.Item("HistoricVendorID"))
            End If
            m_YearStarted = Convert.ToInt32(r.Item("YearStarted"))
            m_Employees = Convert.ToInt32(r.Item("Employees"))
            m_ProductsOffered = Convert.ToString(r.Item("ProductsOffered"))
            m_CompanyMemberships = Convert.ToString(r.Item("CompanyMemberships"))
            m_IsSubsidiary = Convert.ToBoolean(r.Item("IsSubsidiary"))
            m_SubsidiaryExplanation = Convert.ToString(r.Item("SubsidiaryExplanation"))
            m_HadLawsuit = Convert.ToBoolean(r.Item("HadLawsuit"))
            m_LawsuitExplanation = Convert.ToString(r.Item("LawsuitExplanation"))
            m_SupplyArea = Convert.ToString(r.Item("SupplyArea"))
            If IsDBNull(r.Item("PrimarySupplyPhaseID")) Then
                m_PrimarySupplyPhaseID = Nothing
            Else
                m_PrimarySupplyPhaseID = Convert.ToInt32(r.Item("PrimarySupplyPhaseID"))
            End If
            If IsDBNull(r.Item("SecondarySupplyPhaseID")) Then
                m_SecondarySupplyPhaseID = Nothing
            Else
                m_SecondarySupplyPhaseID = Convert.ToInt32(r.Item("SecondarySupplyPhaseID"))
            End If
            m_PreparerFirstName = Convert.ToString(r.Item("PreparerFirstName"))
            m_PreparerLastName = Convert.ToString(r.Item("PreparerLastName"))
            m_AcceptsTerms = Convert.ToBoolean(r.Item("AcceptsTerms"))
            m_Notes = Convert.ToString(r.Item("Notes"))
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Convert.ToDateTime(r.Item("Updated"))
            End If
            m_RegistrationStatusID = Convert.ToInt32(r.Item("RegistrationStatusID"))
            m_ApprovingAdminID = Core.GetInt(r.Item("ApprovingAdminID"))
            If IsDBNull(r.Item("Approved")) Then
                m_Approved = Nothing
            Else
                m_Approved = Convert.ToDateTime(r.Item("Approved"))
            End If
            m_BusinessType = Core.GetInt(r.Item("BusinessType"))
            m_CompleteDate = Core.GetDate(r.Item("CompleteDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO VendorRegistration (" _
             & " VendorID" _
             & ",HistoricVendorID" _
             & ",YearStarted" _
             & ",Employees" _
             & ",ProductsOffered" _
             & ",CompanyMemberships" _
             & ",IsSubsidiary" _
             & ",SubsidiaryExplanation" _
             & ",HadLawsuit" _
             & ",LawsuitExplanation" _
             & ",SupplyArea" _
             & ",PrimarySupplyPhaseID" _
             & ",SecondarySupplyPhaseID" _
             & ",PreparerFirstName" _
             & ",PreparerLastName" _
             & ",AcceptsTerms" _
             & ",Notes" _
             & ",Submitted" _
             & ",Updated" _
             & ",RegistrationStatusID" _
             & ",ApprovingAdminID" _
             & ",Approved" _
             & ",BusinessType" _
             & ",CompleteDate" _
             & ") VALUES (" _
             & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(HistoricVendorID) _
             & "," & m_DB.Number(YearStarted) _
             & "," & m_DB.Number(Employees) _
             & "," & m_DB.Quote(ProductsOffered) _
             & "," & m_DB.Quote(CompanyMemberships) _
             & "," & CInt(IsSubsidiary) _
             & "," & m_DB.Quote(SubsidiaryExplanation) _
             & "," & CInt(HadLawsuit) _
             & "," & m_DB.Quote(LawsuitExplanation) _
             & "," & m_DB.Quote(SupplyArea) _
             & "," & m_DB.NullNumber(PrimarySupplyPhaseID) _
             & "," & m_DB.NullNumber(SecondarySupplyPhaseID) _
             & "," & m_DB.Quote(PreparerFirstName) _
             & "," & m_DB.Quote(PreparerLastName) _
             & "," & CInt(AcceptsTerms) _
             & "," & m_DB.Quote(Notes) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullNumber(RegistrationStatusID) _
             & "," & m_DB.NullNumber(ApprovingAdminID) _
             & "," & m_DB.NullQuote(Approved) _
             & "," & m_DB.NullNumber(BusinessType) _
             & "," & m_DB.NullQuote(CompleteDate) _
             & ")"

            VendorRegistrationID = m_DB.InsertSQL(SQL)

            Return VendorRegistrationID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE VendorRegistration SET " _
             & " VendorID = " & m_DB.NullNumber(VendorID) _
             & ",HistoricVendorID = " & m_DB.NullNumber(HistoricVendorID) _
             & ",YearStarted = " & m_DB.Number(YearStarted) _
             & ",Employees = " & m_DB.Number(Employees) _
             & ",ProductsOffered = " & m_DB.Quote(ProductsOffered) _
             & ",CompanyMemberships = " & m_DB.Quote(CompanyMemberships) _
             & ",IsSubsidiary = " & CInt(IsSubsidiary) _
             & ",SubsidiaryExplanation = " & m_DB.Quote(SubsidiaryExplanation) _
             & ",HadLawsuit = " & CInt(HadLawsuit) _
             & ",LawsuitExplanation = " & m_DB.Quote(LawsuitExplanation) _
             & ",SupplyArea = " & m_DB.Quote(SupplyArea) _
             & ",PrimarySupplyPhaseID = " & m_DB.NullNumber(PrimarySupplyPhaseID) _
             & ",SecondarySupplyPhaseID = " & m_DB.NullNumber(SecondarySupplyPhaseID) _
             & ",PreparerFirstName = " & m_DB.Quote(PreparerFirstName) _
             & ",PreparerLastName = " & m_DB.Quote(PreparerLastName) _
             & ",AcceptsTerms = " & CInt(AcceptsTerms) _
             & ",Notes = " & m_DB.Quote(Notes) _
             & ",Updated = " & m_DB.NullQuote(Now) _
             & ",RegistrationStatusID = " & m_DB.NullNumber(RegistrationStatusID) _
             & ",ApprovingAdminID = " & m_DB.NullNumber(ApprovingAdminID) _
             & ",Approved = " & m_DB.NullQuote(Approved) _
             & ",BusinessType = " & m_DB.NullNumber(BusinessType) _
             & ",CompleteDate = " & m_DB.NullQuote(CompleteDate) _
             & " WHERE VendorRegistrationID = " & m_DB.Quote(VendorRegistrationID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM VendorRegistration WHERE VendorRegistrationID = " & m_DB.Number(VendorRegistrationID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class VendorRegistrationCollection
        Inherits GenericCollection(Of VendorRegistrationRow)
    End Class

End Namespace


