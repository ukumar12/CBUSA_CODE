Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class MemberAddressRow
        Inherits MemberAddressRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal AddressId As Integer)
            MyBase.New(DB, AddressId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal AddressId As Integer) As MemberAddressRow
            Dim row As MemberAddressRow

            row = New MemberAddressRow(DB, AddressId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal AddressId As Integer)
            Dim row As MemberAddressRow

            row = New MemberAddressRow(DB, AddressId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetDefaultBillingRow(ByVal DB As Database, ByVal MemberId As Integer) As MemberAddressRow
            Dim SQL As String = "SELECT * FROM MemberAddress WHERE AddressType = 'Billing' and IsDefault = 1 and MemberId = " & MemberId
            Dim r As SqlDataReader
            Dim row As MemberAddressRow = New MemberAddressRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then row.Load(r)
            r.Close()
            Return row
        End Function

        Public Shared Function GetDefaultShippingRow(ByVal DB As Database, ByVal MemberId As Integer) As MemberAddressRow
            Dim SQL As String = "SELECT * FROM MemberAddress WHERE AddressType = 'Shipping' and IsDefault = 1 and MemberId = " & MemberId
            Dim r As SqlDataReader
            Dim row As MemberAddressRow = New MemberAddressRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then row.Load(r)
            r.Close()
            Return row
        End Function

        Public Shared Function IsMemberAddressValid(ByVal DB As Database, ByVal MemberId As Integer, ByVal AddressId As Integer) As Boolean
            Dim dbAddress As MemberAddressRow = MemberAddressRow.GetRow(DB, AddressId)
            If dbAddress.MemberId = MemberId Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function GetAddressBookRecipients(ByVal DB As Database, ByVal MemberId As Integer, ByVal OrderId As Integer) As DataTable
            Return DB.GetDataTable("SELECT AddressId, Label FROM MemberAddress ma WHERE AddressType = 'AddressBook' AND MemberId = " & MemberId & " AND Label NOT IN ( SELECT Label FROM StoreOrderRecipient WHERE Label = ma.Label AND OrderId = " & OrderId & ")")
        End Function

        Public ReadOnly Property StateName() As String
            Get
                Dim dbState As StateRow = StateRow.GetRowByCode(DB, State)
                If Not dbState Is Nothing Then
                    Return dbState.StateName
                End If
                Return String.Empty
            End Get
        End Property

        Public ReadOnly Property CountryName() As String
            Get
                Dim dbCountry As CountryRow = CountryRow.GetRowByCode(DB, Country)
                If Not dbCountry Is Nothing Then
                    Return dbCountry.CountryName
                End If
                Return String.Empty
            End Get
        End Property

    End Class

    Public MustInherit Class MemberAddressRowBase
        Private m_DB As Database
        Private m_AddressId As Integer = Nothing
        Private m_MemberId As Integer = Nothing
        Private m_AddressType As String = Nothing
        Private m_Label As String = Nothing
        Private m_Company As String = Nothing
        Private m_FirstName As String = Nothing
        Private m_MiddleInitial As String = Nothing
        Private m_LastName As String = Nothing
        Private m_Address1 As String = Nothing
        Private m_Address2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Region As String = Nothing
        Private m_Zip As String = Nothing
        Private m_Country As String = Nothing
        Private m_Email As String = Nothing
        Private m_Phone As String = Nothing
        Private m_IsDefault As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ModifyDate As DateTime = Nothing


        Public Property AddressId() As Integer
            Get
                Return m_AddressId
            End Get
            Set(ByVal Value As Integer)
                m_AddressId = value
            End Set
        End Property

        Public Property MemberId() As Integer
            Get
                Return m_MemberId
            End Get
            Set(ByVal Value As Integer)
                m_MemberId = value
            End Set
        End Property

        Public Property AddressType() As String
            Get
                Return m_AddressType
            End Get
            Set(ByVal Value As String)
                m_AddressType = value
            End Set
        End Property

        Public Property Label() As String
            Get
                Return m_Label
            End Get
            Set(ByVal Value As String)
                m_Label = value
            End Set
        End Property

        Public Property Company() As String
            Get
                Return m_Company
            End Get
            Set(ByVal Value As String)
                m_Company = value
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

        Public Property MiddleInitial() As String
            Get
                Return m_MiddleInitial
            End Get
            Set(ByVal Value As String)
                m_MiddleInitial = value
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

        Public Property Address1() As String
            Get
                Return m_Address1
            End Get
            Set(ByVal Value As String)
                m_Address1 = value
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

        Public Property Region() As String
            Get
                Return m_Region
            End Get
            Set(ByVal Value As String)
                m_Region = value
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

        Public Property Country() As String
            Get
                Return m_Country
            End Get
            Set(ByVal Value As String)
                m_Country = value
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

        Public Property Phone() As String
            Get
                Return m_Phone
            End Get
            Set(ByVal Value As String)
                m_Phone = value
            End Set
        End Property

        Public Property IsDefault() As Boolean
            Get
                Return m_IsDefault
            End Get
            Set(ByVal Value As Boolean)
                m_IsDefault = value
            End Set
        End Property

        Public ReadOnly Property CreateDate() As DateTime
            Get
                Return m_CreateDate
            End Get
        End Property

        Public ReadOnly Property ModifyDate() As DateTime
            Get
                Return m_ModifyDate
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

        Public Sub New(ByVal DB As Database, ByVal AddressId As Integer)
            m_DB = DB
            m_AddressId = AddressId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM MemberAddress WHERE AddressId = " & DB.Quote(AddressId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_AddressId = Convert.ToInt32(r.Item("AddressId"))
            m_MemberId = Convert.ToInt32(r.Item("MemberId"))
            m_AddressType = Convert.ToString(r.Item("AddressType"))
            m_Label = Convert.ToString(r.Item("Label"))
            If IsDBNull(r.Item("Company")) Then
                m_Company = Nothing
            Else
                m_Company = Convert.ToString(r.Item("Company"))
            End If
            m_FirstName = Convert.ToString(r.Item("FirstName"))
            If IsDBNull(r.Item("MiddleInitial")) Then
                m_MiddleInitial = Nothing
            Else
                m_MiddleInitial = Convert.ToString(r.Item("MiddleInitial"))
            End If
            m_LastName = Convert.ToString(r.Item("LastName"))
            m_Address1 = Convert.ToString(r.Item("Address1"))
            If IsDBNull(r.Item("Address2")) Then
                m_Address2 = Nothing
            Else
                m_Address2 = Convert.ToString(r.Item("Address2"))
            End If
            m_City = Convert.ToString(r.Item("City"))
            If IsDBNull(r.Item("State")) Then
                m_State = Nothing
            Else
                m_State = Convert.ToString(r.Item("State"))
            End If
            If IsDBNull(r.Item("Region")) Then
                m_Region = Nothing
            Else
                m_Region = Convert.ToString(r.Item("Region"))
            End If
            If IsDBNull(r.Item("Zip")) Then
                m_Zip = Nothing
            Else
                m_Zip = Convert.ToString(r.Item("Zip"))
            End If
            If IsDBNull(r.Item("Country")) Then
                m_Country = Nothing
            Else
                m_Country = Convert.ToString(r.Item("Country"))
            End If
            If IsDBNull(r.Item("Email")) Then
                m_Email = Nothing
            Else
                m_Email = Convert.ToString(r.Item("Email"))
            End If
            If IsDBNull(r.Item("Phone")) Then
                m_Phone = Nothing
            Else
                m_Phone = Convert.ToString(r.Item("Phone"))
            End If
            m_IsDefault = Convert.ToBoolean(r.Item("IsDefault"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            m_ModifyDate = Convert.ToDateTime(r.Item("ModifyDate"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String
            SQL = " INSERT INTO MemberAddress (" _
             & " MemberId" _
             & ",AddressType" _
             & ",Label" _
             & ",Company" _
             & ",FirstName" _
             & ",MiddleInitial" _
             & ",LastName" _
             & ",Address1" _
             & ",Address2" _
             & ",City" _
             & ",State" _
             & ",Region" _
             & ",Zip" _
             & ",Country" _
             & ",Email" _
             & ",Phone" _
             & ",IsDefault" _
             & ",CreateDate" _
             & ",ModifyDate" _
             & ") VALUES (" _
             & m_DB.Quote(MemberId) _
             & "," & m_DB.Quote(AddressType) _
             & "," & m_DB.Quote(Label) _
             & "," & m_DB.Quote(Company) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(MiddleInitial) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(Address1) _
             & "," & m_DB.Quote(Address2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Region) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(Country) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Quote(Phone) _
             & "," & CInt(IsDefault) _
             & "," & m_DB.Quote(Now) _
             & "," & m_DB.Quote(Now) _
             & ")"
            AddressId = m_DB.InsertSQL(SQL)
            Return AddressId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String
            SQL = " UPDATE MemberAddress SET " _
             & " MemberId = " & m_DB.Quote(MemberId) _
             & ",AddressType = " & m_DB.Quote(AddressType) _
             & ",Label = " & m_DB.Quote(Label) _
             & ",Company = " & m_DB.Quote(Company) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",MiddleInitial = " & m_DB.Quote(MiddleInitial) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",Address1 = " & m_DB.Quote(Address1) _
             & ",Address2 = " & m_DB.Quote(Address2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Region = " & m_DB.Quote(Region) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",Country = " & m_DB.Quote(Country) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",IsDefault = " & CInt(IsDefault) _
             & ",ModifyDate = " & m_DB.Quote(Now) _
             & " WHERE AddressId = " & m_DB.Quote(AddressId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String
            SQL = "DELETE FROM MemberAddress WHERE AddressId = " & m_DB.Quote(AddressId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class MemberAddressCollection
        Inherits GenericCollection(Of MemberAddressRow)
    End Class
End Namespace


