Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class PIQRow
        Inherits PIQRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PIQID As Integer)
            MyBase.New(DB, PIQID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PIQID As Integer) As PIQRow
            Dim row As PIQRow

            row = New PIQRow(DB, PIQID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PIQID As Integer)
            Dim row As PIQRow

            row = New PIQRow(DB, PIQID)
            row.Remove()
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from PIQ"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public ReadOnly Property GetSelectedVendors() As String
            Get
                Dim dr As SqlDataReader = DB.GetReader("select PreferredVendorID from PIQPreferredVendor where PIQID = " & PIQID)
                Dim Conn As String = String.Empty
                Dim Result As String = String.Empty

                While dr.Read()
                    Result &= Conn & dr("PreferredVendorID")
                    Conn = ","
                End While
                dr.Close()
                Return Result
            End Get
        End Property

        Public Sub DeleteFromAllVendors()
            DB.ExecuteSQL("delete from PIQPreferredVendor where PIQID = " & PIQID)
        End Sub

        Public Sub InsertToVendors(ByVal Elements As String)
            If Elements = String.Empty Then Exit Sub

            Dim aElements As String() = Elements.Split(",")
            For Each Element As String In aElements
                If Element <> "" Then
                    InsertToVendor(DB, Element, PIQID)
                End If
            Next
        End Sub

        Public Shared Sub InsertToVendor(ByVal DB As Database, ByVal VendorID As Integer, ByVal PIQID As Integer)
            Dim SQL As String = "insert into PIQPreferredVendor (PIQID, PreferredVendorID) values (" & DB.Number(PIQID) & "," & DB.Number(VendorID) & ")"
            DB.ExecuteSQL(SQL)
        End Sub

        Public Shared Sub RemoveVendor(ByVal DB As Database, ByVal VendorId As Integer, ByVal PIQID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM PIQPreferredVendor WHERE PIQID = " & DB.Number(PIQID) & " AND PreferredVendorId = " & DB.Number(VendorId)
            DB.ExecuteSQL(SQL)
        End Sub 'RemoveVendor

        Public Sub UpdateVendors(ByVal dt As DataTable)
            DB.ExecuteSQL("Delete from PIQPreferredVendor where PIQID = " & DB.Number(Me.PIQID))
            For i As Integer = 0 To dt.Rows.Count - 1
                If Not IsDBNull(dt.Rows(i)("VendorId")) AndAlso CStr(dt.Rows(i)("VendorId")).Trim() <> String.Empty Then
                    InsertToVendor(DB, dt.Rows(i)("VendorId"), PIQID)
                End If
            Next
        End Sub

        Public Shared Function GetPreferredVendorDetails(ByVal DB As Database, ByVal PIQId As Integer) As DataTable
            Dim sql As String = "select * from Vendor where VendorId in (select PreferredVendorId from PIQPreferredVendor where PIQId=" & DB.Number(PIQId) & ") order by CompanyName"
            Return DB.GetDataTable(sql)
        End Function
    End Class

    Public MustInherit Class PIQRowBase
        Private m_DB As Database
        Private m_PIQID As Integer = Nothing
        Private m_CompanyName As String = Nothing
        Private m_Address As String = Nothing
        Private m_Address2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Zip As String = Nothing
        Private m_Phone As String = Nothing
        Private m_Mobile As String = Nothing
        Private m_Fax As String = Nothing
        Private m_WebsiteURL As String = Nothing
        Private m_FirstName As String = Nothing
        Private m_LastName As String = Nothing
        Private m_Email As String = Nothing
        Private m_LogoFile As String = Nothing
        Private m_LogoGUID As String = Nothing
        Private m_IncentivePrograms As String = Nothing
        Private m_Submitted As DateTime = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_HasCatalogAccess As Boolean = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_HasDocumentsAccess As Boolean = Nothing

        Public Property PIQID() As Integer
            Get
                Return m_PIQID
            End Get
            Set(ByVal Value As Integer)
                m_PIQID = value
            End Set
        End Property

        Public Property CompanyName() As String
            Get
                Return m_CompanyName
            End Get
            Set(ByVal Value As String)
                m_CompanyName = value
            End Set
        End Property

        Public Property Address() As String
            Get
                Return m_Address
            End Get
            Set(ByVal Value As String)
                m_Address = value
            End Set
        End Property

        Public Property Address2() As String
            Get
                Return m_Address2
            End Get
            Set(ByVal Value As String)
                m_Address2 = value
            End Set
        End Property

        Public Property City() As String
            Get
                Return m_City
            End Get
            Set(ByVal Value As String)
                m_City = value
            End Set
        End Property

        Public Property State() As String
            Get
                Return m_State
            End Get
            Set(ByVal Value As String)
                m_State = value
            End Set
        End Property

        Public Property Zip() As String
            Get
                Return m_Zip
            End Get
            Set(ByVal Value As String)
                m_Zip = value
            End Set
        End Property

        Public Property Phone() As String
            Get
                Return m_Phone
            End Get
            Set(ByVal Value As String)
                m_Phone = value
            End Set
        End Property

        Public Property Mobile() As String
            Get
                Return m_Mobile
            End Get
            Set(ByVal Value As String)
                m_Mobile = value
            End Set
        End Property

        Public Property Fax() As String
            Get
                Return m_Fax
            End Get
            Set(ByVal Value As String)
                m_Fax = value
            End Set
        End Property

        Public Property WebsiteURL() As String
            Get
                Return m_WebsiteURL
            End Get
            Set(ByVal Value As String)
                m_WebsiteURL = value
            End Set
        End Property

        Public Property FirstName() As String
            Get
                Return m_FirstName
            End Get
            Set(ByVal Value As String)
                m_FirstName = value
            End Set
        End Property

        Public Property LastName() As String
            Get
                Return m_LastName
            End Get
            Set(ByVal Value As String)
                m_LastName = value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = value
            End Set
        End Property

        Public Property LogoFile() As String
            Get
                Return m_LogoFile
            End Get
            Set(ByVal Value As String)
                m_LogoFile = value
            End Set
        End Property

        Public Property LogoGUID() As String
            Get
                Return m_LogoGUID
            End Get
            Set(ByVal Value As String)
                m_LogoGUID = value
            End Set
        End Property

        Public Property IncentivePrograms() As String
            Get
                Return m_IncentivePrograms
            End Get
            Set(ByVal Value As String)
                m_IncentivePrograms = value
            End Set
        End Property

        Public Property Submitted() As DateTime
            Get
                Return m_Submitted
            End Get
            Set(ByVal Value As DateTime)
                m_Submitted = value
            End Set
        End Property

        Public Property Updated() As DateTime
            Get
                Return m_Updated
            End Get
            Set(ByVal Value As DateTime)
                m_Updated = value
            End Set
        End Property

        Public Property IsActive() As Boolean
            Get
                Return m_IsActive
            End Get
            Set(ByVal Value As Boolean)
                m_IsActive = value
            End Set
        End Property
        Public Property HasCatalogAccess() As Boolean
            Get
                Return m_HasCatalogAccess
            End Get
            Set(ByVal Value As Boolean)
                m_HasCatalogAccess = value
            End Set
        End Property

        Public Property HasDocumentsAccess() As Boolean
            Get
                Return m_HasDocumentsAccess
            End Get
            Set(ByVal Value As Boolean)
                m_HasDocumentsAccess = Value
            End Set
        End Property


        Public Property StartDate() As DateTime
            Get
                Return m_StartDate
            End Get
            Set(ByVal Value As DateTime)
                m_StartDate = value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return m_EndDate
            End Get
            Set(ByVal Value As DateTime)
                m_EndDate = value
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

        Public Sub New(ByVal DB As Database, ByVal PIQID As Integer)
            m_DB = DB
            m_PIQID = PIQID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM PIQ WHERE PIQID = " & DB.Number(PIQID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PIQID = Convert.ToInt32(r.Item("PIQID"))
            m_CompanyName = Convert.ToString(r.Item("CompanyName"))
            m_Address = Convert.ToString(r.Item("Address"))
            If IsDBNull(r.Item("Address2")) Then
                m_Address2 = Nothing
            Else
                m_Address2 = Convert.ToString(r.Item("Address2"))
            End If
            m_City = Convert.ToString(r.Item("City"))
            m_State = Convert.ToString(r.Item("State"))
            m_Zip = Convert.ToString(r.Item("Zip"))
            m_Phone = Convert.ToString(r.Item("Phone"))
            If IsDBNull(r.Item("Mobile")) Then
                m_Mobile = Nothing
            Else
                m_Mobile = Convert.ToString(r.Item("Mobile"))
            End If
            If IsDBNull(r.Item("Fax")) Then
                m_Fax = Nothing
            Else
                m_Fax = Convert.ToString(r.Item("Fax"))
            End If
            If IsDBNull(r.Item("WebsiteURL")) Then
                m_WebsiteURL = Nothing
            Else
                m_WebsiteURL = Convert.ToString(r.Item("WebsiteURL"))
            End If
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_Email = Convert.ToString(r.Item("Email"))
            If IsDBNull(r.Item("LogoFile")) Then
                m_LogoFile = Nothing
            Else
                m_LogoFile = Convert.ToString(r.Item("LogoFile"))
            End If
            If IsDBNull(r.Item("LogoGUID")) Then
                m_LogoGUID = Nothing
            Else
                m_LogoGUID = Convert.ToString(r.Item("LogoGUID"))
            End If
            If IsDBNull(r.Item("IncentivePrograms")) Then
                m_IncentivePrograms = Nothing
            Else
                m_IncentivePrograms = Convert.ToString(r.Item("IncentivePrograms"))
            End If
            m_Submitted = Convert.ToDateTime(r.Item("Submitted"))
            If IsDBNull(r.Item("Updated")) Then
                m_Updated = Nothing
            Else
                m_Updated = Convert.ToDateTime(r.Item("Updated"))
            End If
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            m_HasCatalogAccess = Convert.ToBoolean(r.Item("HasCatalogAccess"))
            m_HasDocumentsAccess = Convert.ToBoolean(r.Item("HasDocumentsAccess"))
            m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            If IsDBNull(r.Item("EndDate")) Then
                m_EndDate = Nothing
            Else
                m_EndDate = Convert.ToDateTime(r.Item("EndDate"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO PIQ (" _
             & " CompanyName" _
             & ",Address" _
             & ",Address2" _
             & ",City" _
             & ",State" _
             & ",Zip" _
             & ",Phone" _
             & ",Mobile" _
             & ",Fax" _
             & ",WebsiteURL" _
             & ",FirstName" _
             & ",LastName" _
             & ",Email" _
             & ",LogoFile" _
             & ",LogoGUID" _
             & ",IncentivePrograms" _
             & ",Submitted" _
             & ",Updated" _
             & ",IsActive" _
             & ",HasCatalogAccess" _
             & ",StartDate" _
             & ",EndDate" _
             & ",HasDocumentsAccess" _
             & ") VALUES (" _
             & m_DB.Quote(CompanyName) _
             & "," & m_DB.Quote(Address) _
             & "," & m_DB.Quote(Address2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(Mobile) _
             & "," & m_DB.Quote(Fax) _
             & "," & m_DB.Quote(WebsiteURL) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(LogoFile) _
             & "," & m_DB.Quote(LogoGUID) _
             & "," & m_DB.Quote(IncentivePrograms) _
             & "," & m_DB.NullQuote(Submitted) _
             & "," & m_DB.NullQuote(Updated) _
             & "," & CInt(IsActive) _
             & "," & CInt(HasCatalogAccess) _
             & "," & m_DB.NullQuote(StartDate) _
             & "," & m_DB.NullQuote(EndDate) _
             & "," & CInt(HasDocumentsAccess) _
             & ")"

            PIQID = m_DB.InsertSQL(SQL)

            Return PIQID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE PIQ SET " _
             & " CompanyName = " & m_DB.Quote(CompanyName) _
             & ",Address = " & m_DB.Quote(Address) _
             & ",Address2 = " & m_DB.Quote(Address2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",Mobile = " & m_DB.Quote(Mobile) _
             & ",Fax = " & m_DB.Quote(Fax) _
             & ",WebsiteURL = " & m_DB.Quote(WebsiteURL) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",LogoFile = " & m_DB.Quote(LogoFile) _
             & ",LogoGUID = " & m_DB.Quote(LogoGUID) _
             & ",IncentivePrograms = " & m_DB.Quote(IncentivePrograms) _
             & ",Submitted = " & m_DB.NullQuote(Submitted) _
             & ",Updated = " & m_DB.NullQuote(Updated) _
             & ",IsActive = " & CInt(IsActive) _
             & ",HasCatalogAccess = " & CInt(HasCatalogAccess) _
             & ",StartDate = " & m_DB.NullQuote(StartDate) _
             & ",EndDate = " & m_DB.NullQuote(EndDate) _
             & ",HasDocumentsAccess = " & CInt(HasDocumentsAccess) _
             & " WHERE PIQID = " & m_DB.Quote(PIQID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM PIQAccountLLC WHERE PIQAccountID IN (SELECT PIQAccountID FROM PIQAccount WHERE PIQID = " & m_DB.Number(PIQID) & ")"
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM PIQAccount WHERE PIQID = " & m_DB.Number(PIQID)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM PIQ WHERE PIQID = " & m_DB.Number(PIQID)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove

        
    End Class

    Public Class PIQCollection
        Inherits GenericCollection(Of PIQRow)
    End Class

End Namespace


