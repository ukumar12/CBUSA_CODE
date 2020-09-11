Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreOrderRecipientRow
        Inherits StoreOrderRecipientRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal RecipientId As Integer)
            MyBase.New(DB, RecipientId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal RecipientId As Integer) As StoreOrderRecipientRow
            Dim row As StoreOrderRecipientRow

            row = New StoreOrderRecipientRow(DB, RecipientId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal RecipientId As Integer)
            Dim row As StoreOrderRecipientRow

            row = New StoreOrderRecipientRow(DB, RecipientId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetOrderRecipients(ByVal DB As Database, ByVal OrderId As Integer) As DataTable
            Return DB.GetDataTable("SELECT RecipientId, IsShippingIndividually, Status, TrackingNr, ShippedDate, Label, FirstName, LastName, Company, Address1, Address2, City, State, Zip, Country, Phone, BaseSubtotal, Subtotal, GiftMessage, GiftMessageLabel, (SELECT Name FROM StoreShippingMethod sm WHERE sm.MethodId = sor.ShippingMethodId) As ShippingMethod, Discount, Shipping, GiftWrapping, Tax, Total, (SELECT Name FROM StoreOrderStatus WHERE StoreOrderStatus.Code = sor.STatus ) as StatusName FROM StoreOrderRecipient sor WHERE OrderId=" & OrderId)
        End Function

        Public Shared Function GetOrderRecipientsOrderByLabel(ByVal DB As Database, ByVal OrderId As Integer) As DataTable
            Return DB.GetDataTable("SELECT RecipientId, Status, TrackingNr, ShippedDate, Label, FirstName, LastName, Company, Address1, Address2, City, State, Zip, Country, Phone, BaseSubtotal, Subtotal, GiftMessage, GiftMessageLabel, (SELECT Name FROM StoreShippingMethod sm WHERE sm.MethodId = sor.ShippingMethodId) As ShippingMethod, Discount, Shipping, GiftWrapping, Tax, Total FROM StoreOrderRecipient sor WHERE OrderId=" & OrderId & " and IsCustomer = 0 order by Label")
        End Function

        Public Shared Function GetDefaultRecipient(ByVal DB As Database, ByVal OrderId As Integer) As StoreOrderRecipientRow
            Dim SQL As String = "SELECT * FROM StoreOrderRecipient WHERE OrderId = " & OrderId & " and IsCustomer = 1"
            Dim r As SqlDataReader
            Dim row As StoreOrderRecipientRow = New StoreOrderRecipientRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Function GetCartItemsById(ByVal DB As Database, ByVal CartItems As String) As DataTable
            Dim dt As DataTable = DB.GetDataTable("SELECT * FROM StoreOrderItem WHERE OrderId=" & DB.Number(Me.OrderId) & " AND RecipientId=" & DB.Number(Me.RecipientId) & " AND OrderItemId IN " & DB.NumberMultiple(CartItems))
            Return dt
        End Function
    End Class

    Public MustInherit Class StoreOrderRecipientRowBase
        Private m_DB As Database
        Private m_RecipientId As Integer = Nothing
        Private m_OrderId As Integer = Nothing
        Private m_AddressId As Integer = Nothing
        Private m_Label As String = Nothing
        Private m_IsCustomer As Boolean = Nothing
        Private m_IsSameAsBilling As Boolean = Nothing
        Private m_FirstName As String = Nothing
        Private m_MiddleInitial As String = Nothing
        Private m_LastName As String = Nothing
        Private m_Company As String = Nothing
        Private m_Address1 As String = Nothing
        Private m_Address2 As String = Nothing
        Private m_City As String = Nothing
        Private m_State As String = Nothing
        Private m_Region As String = Nothing
        Private m_Country As String = Nothing
        Private m_Zip As String = Nothing
        Private m_Phone As String = Nothing
        Private m_GiftMessage As String = Nothing
        Private m_GiftMessageLabel As String = Nothing
        Private m_BaseSubtotal As Double = Nothing
        Private m_Discount As Double = Nothing
        Private m_Subtotal As Double = Nothing
        Private m_Shipping As Double = Nothing
        Private m_GiftWrapping As Double = Nothing
        Private m_Tax As Double = Nothing
        Private m_Total As Double = Nothing
        Private m_CountryName As String
        Private m_Status As String = Nothing
        Private m_ShippedDate As String = Nothing
        Private m_TrackingNr As String = Nothing
        Private m_ShippingMethodId As Integer = Nothing
        Private m_IsShippingIndividually As Boolean = Nothing


        Public Property IsShippingIndividually() As Boolean
            Get
                Return m_IsShippingIndividually
            End Get
            Set(ByVal value As Boolean)
                m_IsShippingIndividually = value
            End Set
        End Property

        Public Property RecipientId() As Integer
            Get
                Return m_RecipientId
            End Get
            Set(ByVal Value As Integer)
                m_RecipientId = Value
            End Set
        End Property

        Public Property OrderId() As Integer
            Get
                Return m_OrderId
            End Get
            Set(ByVal Value As Integer)
                m_OrderId = value
            End Set
        End Property

        Public Property AddressId() As Integer
            Get
                Return m_AddressId
            End Get
            Set(ByVal Value As Integer)
                m_AddressId = value
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

        Public Property IsCustomer() As Boolean
            Get
                Return m_IsCustomer
            End Get
            Set(ByVal Value As Boolean)
                m_IsCustomer = value
            End Set
        End Property

        Public Property IsSameAsBilling() As Boolean
            Get
                Return m_IsSameAsBilling
            End Get
            Set(ByVal Value As Boolean)
                m_IsSameAsBilling = value
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

        Public Property Company() As String
            Get
                Return m_Company
            End Get
            Set(ByVal Value As String)
                m_Company = value
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

        Public Property Country() As String
            Get
                Return m_Country
            End Get
            Set(ByVal Value As String)
                m_Country = Value
                m_CountryName = Nothing
            End Set
        End Property

        Public Property CountryName() As String
            Get
                If m_CountryName Is Nothing Then
                    m_CountryName = DataLayer.CountryRow.GetRow(DB, Country).CountryName
                End If
                Return m_Country
            End Get
            Set(ByVal Value As String)
                m_Country = Value
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

        Public Property GiftMessage() As String
            Get
                Return m_GiftMessage
            End Get
            Set(ByVal Value As String)
                m_GiftMessage = Value
            End Set
        End Property

        Public Property GiftMessageLabel() As String
            Get
                Return m_GiftMessageLabel
            End Get
            Set(ByVal Value As String)
                m_GiftMessageLabel = Value
            End Set
        End Property

        Public Property BaseSubtotal() As Double
            Get
                Return m_BaseSubtotal
            End Get
            Set(ByVal Value As Double)
                m_BaseSubtotal = value
            End Set
        End Property

        Public Property Discount() As Double
            Get
                Return m_Discount
            End Get
            Set(ByVal Value As Double)
                m_Discount = value
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

        Public Property Shipping() As Double
            Get
                Return m_Shipping
            End Get
            Set(ByVal Value As Double)
                m_Shipping = value
            End Set
        End Property

        Public Property GiftWrapping() As Double
            Get
                Return m_GiftWrapping
            End Get
            Set(ByVal Value As Double)
                m_GiftWrapping = Value
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
                m_Total = Value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal value As String)
                m_Status = value
            End Set
        End Property

        Public Property TrackingNr() As String
            Get
                Return m_TrackingNr
            End Get
            Set(ByVal value As String)
                m_TrackingNr = value
            End Set
        End Property

        Public Property ShippedDate() As String
            Get
                Return m_ShippedDate
            End Get
            Set(ByVal value As String)
                m_ShippedDate = value
            End Set
        End Property

        Public Property ShippingMethodId() As Integer
            Get
                Return m_ShippingMethodId
            End Get
            Set(ByVal value As Integer)
                m_ShippingMethodId = value
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

        Public Sub New(ByVal DB As Database, ByVal RecipientId As Integer)
            m_DB = DB
            m_RecipientId = RecipientId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreOrderRecipient WHERE RecipientId = " & DB.Number(RecipientId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_RecipientId = Convert.ToInt32(r.Item("RecipientId"))
            m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            m_Status = Convert.ToString(r.Item("Status"))

            If IsDBNull(r.Item("AddressId")) Then
                m_AddressId = Nothing
            Else
                m_AddressId = Convert.ToInt32(r.Item("AddressId"))
            End If
            m_Label = Convert.ToString(r.Item("Label"))
            m_IsCustomer = Convert.ToBoolean(r.Item("IsCustomer"))
            m_IsSameAsBilling = Convert.ToBoolean(r.Item("IsSameAsBilling"))
            If IsDBNull(r.Item("FirstName")) Then
                m_FirstName = Nothing
            Else
                m_FirstName = Convert.ToString(r.Item("FirstName"))
            End If
            If IsDBNull(r.Item("MiddleInitial")) Then
                m_MiddleInitial = Nothing
            Else
                m_MiddleInitial = Convert.ToString(r.Item("MiddleInitial"))
            End If
            If IsDBNull(r.Item("LastName")) Then
                m_LastName = Nothing
            Else
                m_LastName = Convert.ToString(r.Item("LastName"))
            End If
            If IsDBNull(r.Item("Company")) Then
                m_Company = Nothing
            Else
                m_Company = Convert.ToString(r.Item("Company"))
            End If
            If IsDBNull(r.Item("Address1")) Then
                m_Address1 = Nothing
            Else
                m_Address1 = Convert.ToString(r.Item("Address1"))
            End If
            If IsDBNull(r.Item("Address2")) Then
                m_Address2 = Nothing
            Else
                m_Address2 = Convert.ToString(r.Item("Address2"))
            End If
            If IsDBNull(r.Item("City")) Then
                m_City = Nothing
            Else
                m_City = Convert.ToString(r.Item("City"))
            End If
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
            If IsDBNull(r.Item("Country")) Then
                m_Country = Nothing
            Else
                m_Country = Convert.ToString(r.Item("Country"))
            End If
            If IsDBNull(r.Item("Zip")) Then
                m_Zip = Nothing
            Else
                m_Zip = Convert.ToString(r.Item("Zip"))
            End If
            If IsDBNull(r.Item("Phone")) Then
                m_Phone = Nothing
            Else
                m_Phone = Convert.ToString(r.Item("Phone"))
            End If
            If IsDBNull(r.Item("GiftMessage")) Then
                m_GiftMessage = Nothing
            Else
                m_GiftMessage = Convert.ToString(r.Item("GiftMessage"))
            End If
            If IsDBNull(r.Item("GiftMessageLabel")) Then
                m_GiftMessageLabel = Nothing
            Else
                m_GiftMessageLabel = Convert.ToString(r.Item("GiftMessageLabel"))
            End If
            m_BaseSubtotal = Convert.ToDouble(r.Item("BaseSubtotal"))
            m_Discount = Convert.ToDouble(r.Item("Discount"))
            m_Subtotal = Convert.ToDouble(r.Item("Subtotal"))
            m_Shipping = Convert.ToDouble(r.Item("Shipping"))
            m_GiftWrapping = Convert.ToDouble(r.Item("GiftWrapping"))
            m_Tax = Convert.ToDouble(r.Item("Tax"))
            If IsDBNull(r.Item("TrackingNr")) Then
                m_TrackingNr = Nothing
            Else
                m_TrackingNr = Convert.ToString(r.Item("TrackingNr"))
            End If
            If IsDBNull(r.Item("ShippedDate")) Then
                m_ShippedDate = Nothing
            Else
                m_ShippedDate = Convert.ToString(r.Item("ShippedDate"))
            End If
            m_Total = Convert.ToDouble(r.Item("Total"))
            If IsDBNull(r.Item("ShippingMethodId")) Then
                m_ShippingMethodId = Nothing
            Else
                m_ShippingMethodId = Convert.ToInt32(r.Item("ShippingMethodId"))
            End If
            m_IsShippingIndividually = Convert.ToBoolean(r.Item("IsShippingIndividually"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreOrderRecipient (" _
             & " OrderId" _
             & ",AddressId" _
             & ",Label" _
             & ",IsCustomer" _
             & ",IsSameAsBilling" _
             & ",FirstName" _
             & ",MiddleInitial" _
             & ",LastName" _
             & ",Company" _
             & ",Address1" _
             & ",Address2" _
             & ",City" _
             & ",State" _
             & ",Region" _
             & ",Country" _
             & ",Zip" _
             & ",Phone" _
             & ",GiftMessage" _
             & ",GiftMessageLabel" _
             & ",BaseSubtotal" _
             & ",Discount" _
             & ",Subtotal" _
             & ",Shipping" _
             & ",GiftWrapping" _
             & ",Tax" _
             & ",Total" _
             & ",Status" _
             & ",TrackingNr" _
             & ",ShippedDate" _
             & ",ShippingMethodId" _
             & ",IsShippingIndividually" _
             & ") VALUES (" _
             & m_DB.NullNumber(OrderId) _
             & "," & m_DB.NullNumber(AddressId) _
             & "," & m_DB.Quote(Label) _
             & "," & CInt(IsCustomer) _
             & "," & CInt(IsSameAsBilling) _
             & "," & m_DB.Quote(FirstName) _
             & "," & m_DB.Quote(MiddleInitial) _
             & "," & m_DB.Quote(LastName) _
             & "," & m_DB.Quote(Company) _
             & "," & m_DB.Quote(Address1) _
             & "," & m_DB.Quote(Address2) _
             & "," & m_DB.Quote(City) _
             & "," & m_DB.Quote(State) _
             & "," & m_DB.Quote(Region) _
             & "," & m_DB.Quote(Country) _
             & "," & m_DB.Quote(Zip) _
             & "," & m_DB.Quote(Phone) _
             & "," & m_DB.Quote(GiftMessage) _
             & "," & m_DB.Quote(GiftMessageLabel) _
             & "," & m_DB.Number(BaseSubtotal) _
             & "," & m_DB.Number(Discount) _
             & "," & m_DB.Number(Subtotal) _
             & "," & m_DB.Number(Shipping) _
             & "," & m_DB.Number(GiftWrapping) _
             & "," & m_DB.Number(Tax) _
             & "," & m_DB.Number(Total) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.Quote(TrackingNr) _
             & "," & m_DB.Quote(ShippedDate) _
             & "," & m_DB.NullNumber(ShippingMethodId) _
             & "," & CInt(IsShippingIndividually) _
             & ")"

            RecipientId = m_DB.InsertSQL(SQL)

            Return RecipientId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreOrderRecipient SET " _
             & " OrderId = " & m_DB.NullNumber(OrderId) _
             & ",AddressId = " & m_DB.NullNumber(AddressId) _
             & ",Label = " & m_DB.Quote(Label) _
             & ",IsCustomer = " & CInt(IsCustomer) _
             & ",IsSameAsBilling = " & CInt(IsSameAsBilling) _
             & ",FirstName = " & m_DB.Quote(FirstName) _
             & ",MiddleInitial = " & m_DB.Quote(MiddleInitial) _
             & ",LastName = " & m_DB.Quote(LastName) _
             & ",Company = " & m_DB.Quote(Company) _
             & ",Address1 = " & m_DB.Quote(Address1) _
             & ",Address2 = " & m_DB.Quote(Address2) _
             & ",City = " & m_DB.Quote(City) _
             & ",State = " & m_DB.Quote(State) _
             & ",Region = " & m_DB.Quote(Region) _
             & ",Country = " & m_DB.Quote(Country) _
             & ",Zip = " & m_DB.Quote(Zip) _
             & ",Phone = " & m_DB.Quote(Phone) _
             & ",GiftMessage = " & m_DB.Quote(GiftMessage) _
             & ",GiftMessageLabel = " & m_DB.Quote(GiftMessageLabel) _
             & ",BaseSubtotal = " & m_DB.Number(BaseSubtotal) _
             & ",Discount = " & m_DB.Number(Discount) _
             & ",Subtotal = " & m_DB.Number(Subtotal) _
             & ",Shipping = " & m_DB.Number(Shipping) _
             & ",GiftWrapping = " & m_DB.Number(GiftWrapping) _
             & ",Tax = " & m_DB.Number(Tax) _
             & ",Total = " & m_DB.Number(Total) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",TrackingNr = " & m_DB.Quote(TrackingNr) _
             & ",ShippedDate = " & m_DB.Quote(ShippedDate) _
             & ",ShippingMethodId = " & m_DB.NullNumber(ShippingMethodId) _
             & ",IsShippingIndividually = " & CInt(IsShippingIndividually) _
             & " WHERE RecipientId = " & m_DB.Quote(RecipientId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StoreOrderRecipient WHERE RecipientId = " & m_DB.Quote(RecipientId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreOrderRecipientCollection
        Inherits GenericCollection(Of StoreOrderRecipientRow)
    End Class

End Namespace


