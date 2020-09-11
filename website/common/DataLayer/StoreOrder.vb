Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StoreOrderRow
        Inherits StoreOrderRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal OrderId As Integer)
            MyBase.New(DB, OrderId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal OrderId As Integer) As StoreOrderRow
            Dim row As StoreOrderRow

            row = New StoreOrderRow(DB, OrderId)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal OrderId As Integer)
            Dim row As StoreOrderRow

            row = New StoreOrderRow(DB, OrderId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Shared Function GetOrderSummary(ByVal DB As Database, ByVal OrderId As Integer) As StoreOrderSummary
            Dim SQL As String
            Dim os As New StoreOrderSummary

            SQL = "select coalesce(SUM(Quantity),0) as Quantity, coalesce(SUM(Price * Quantity),0) AS SubTotal FROM StoreOrderItem WITH (NOLOCK) WHERE OrderId = " & OrderId
            Dim dr As SqlDataReader = DB.GetReader(SQL)
            If dr.Read Then
                os.Quantity = dr("Quantity")
                os.SubTotal = dr("SubTotal")
            End If
            dr.Close()
            dr = Nothing
            Return os
        End Function
    End Class

    Public Class StoreOrderSummary
        Public Quantity As Integer = 0
        Public SubTotal As Double = 0
    End Class


    Public MustInherit Class StoreOrderRowBase
        Private m_DB As Database
        Private m_OrderId As Integer = Nothing
        Private m_OrderNo As String = Nothing
        Private m_HowHeardId As Integer = Nothing
        Private m_HowHeardName As String = Nothing
        Private m_MemberId As Integer = Nothing
        Private m_BillingFirstName As String = Nothing
        Private m_BillingMiddleInitial As String = Nothing
        Private m_BillingLastName As String = Nothing
        Private m_BillingCompany As String = Nothing
        Private m_BillingAddress1 As String = Nothing
        Private m_BillingAddress2 As String = Nothing
        Private m_BillingCity As String = Nothing
        Private m_BillingState As String = Nothing
        Private m_BillingRegion As String = Nothing
        Private m_BillingCountry As String = Nothing
        Private m_BillingZip As String = Nothing
        Private m_BillingPhone As String = Nothing
        Private m_CardNumber As String = Nothing
        Private m_CIDNumber As String = Nothing
        Private m_CardTypeId As Integer = Nothing
        Private m_ExpirationDate As String = Nothing
        Private m_CardholderName As String = Nothing
        Private m_Email As String = Nothing
        Private m_BaseSubtotal As Double = Nothing
        Private m_Discount As Double = Nothing
        Private m_Subtotal As Double = Nothing
        Private m_Shipping As Double = Nothing
        Private m_GiftWrapping As Double = Nothing
        Private m_Tax As Double = Nothing
        Private m_Total As Double = Nothing
        Private m_PaymentReference As String = Nothing
        Private m_IsFreeShipping As Boolean = Nothing
        Private m_CreateDate As DateTime = Nothing
        Private m_ProcessDate As DateTime = Nothing
        Private m_ExportDate As DateTime = Nothing
        Private m_ReferralCode As String = Nothing
        Private m_RemoteIP As String = Nothing
        Private m_PromotionCode As String = Nothing
        Private m_Status As String = Nothing
        Private m_Guid As String = Nothing
        Private m_Comments As String = Nothing

        Public Property OrderId() As Integer
            Get
                Return m_OrderId
            End Get
            Set(ByVal Value As Integer)
                m_OrderId = Value
            End Set
        End Property

        Public ReadOnly Property GiftCardCount() As Integer
            Get
                Return DB.ExecuteScalar("SELECT COALESCE(SUM(Quantity),0) FROM StoreOrderItem WHERE IsGiftCard = 1 AND OrderId=" & Me.OrderId)
            End Get
        End Property

        Public ReadOnly Property RegularItemCount() As Integer
            Get
                Return DB.ExecuteScalar("SELECT COALESCE(SUM(Quantity),0) FROM StoreOrderItem WHERE IsGiftCard = 0 AND OrderId=" & Me.OrderId)
            End Get
        End Property


        Public ReadOnly Property TotalItemCount() As Integer
            Get
                Return DB.ExecuteScalar("SELECT COUNT(*) FROM StoreOrderItem WHERE OrderId=" & Me.OrderId)
            End Get
        End Property

        Public Property HowHeardName() As String
            Get
                If m_HowHeardName Is Nothing Then
                    m_HowHeardName = HowHeardRow.GetRow(DB, HowHeardId).HowHeard
                End If
                Return m_HowHeardName
            End Get
            Set(ByVal value As String)
                m_HowHeardName = value
            End Set
        End Property

        Public Property OrderNo() As String
            Get
                Return m_OrderNo
            End Get
            Set(ByVal Value As String)
                m_OrderNo = Value
            End Set
        End Property

        Public Property HowHeardId() As Integer
            Get
                Return m_HowHeardId
            End Get
            Set(ByVal Value As Integer)
                m_HowHeardId = Value
                m_HowHeardName = Nothing
            End Set
        End Property

        Public Property MemberId() As Integer
            Get
                Return m_MemberId
            End Get
            Set(ByVal Value As Integer)
                m_MemberId = Value
            End Set
        End Property

        Public Property BillingFirstName() As String
            Get
                Return m_BillingFirstName
            End Get
            Set(ByVal Value As String)
                m_BillingFirstName = Value
            End Set
        End Property

        Public Property BillingMiddleInitial() As String
            Get
                Return m_BillingMiddleInitial
            End Get
            Set(ByVal Value As String)
                m_BillingMiddleInitial = Value
            End Set
        End Property

        Public Property BillingLastName() As String
            Get
                Return m_BillingLastName
            End Get
            Set(ByVal Value As String)
                m_BillingLastName = Value
            End Set
        End Property

        Public Property BillingCompany() As String
            Get
                Return m_BillingCompany
            End Get
            Set(ByVal Value As String)
                m_BillingCompany = Value
            End Set
        End Property

        Public Property BillingAddress1() As String
            Get
                Return m_BillingAddress1
            End Get
            Set(ByVal Value As String)
                m_BillingAddress1 = Value
            End Set
        End Property

        Public Property BillingAddress2() As String
            Get
                Return m_BillingAddress2
            End Get
            Set(ByVal Value As String)
                m_BillingAddress2 = Value
            End Set
        End Property

        Public Property BillingCity() As String
            Get
                Return m_BillingCity
            End Get
            Set(ByVal Value As String)
                m_BillingCity = Value
            End Set
        End Property

        Public Property BillingState() As String
            Get
                Return m_BillingState
            End Get
            Set(ByVal Value As String)
                m_BillingState = Value
            End Set
        End Property

        Public Property BillingRegion() As String
            Get
                Return m_BillingRegion
            End Get
            Set(ByVal Value As String)
                m_BillingRegion = Value
            End Set
        End Property

        Public Property BillingCountry() As String
            Get
                Return m_BillingCountry
            End Get
            Set(ByVal Value As String)
                m_BillingCountry = Value
            End Set
        End Property

        Public Property BillingZip() As String
            Get
                Return m_BillingZip
            End Get
            Set(ByVal Value As String)
                m_BillingZip = Value
            End Set
        End Property

        Public Property BillingPhone() As String
            Get
                Return m_BillingPhone
            End Get
            Set(ByVal Value As String)
                m_BillingPhone = Value
            End Set
        End Property

        Public ReadOnly Property StarredCardNumber() As String
            Get
                Return Right(CardNumber, 4).PadLeft(16, "*")
            End Get
        End Property

        Public Property CardNumber() As String
            Get
                If m_CardNumber = String.Empty Then Return String.Empty
                Return Utility.Crypt.DecryptTripleDesEx(m_CardNumber, Guid)
            End Get
            Set(ByVal Value As String)
                m_CardNumber = Utility.Crypt.EncryptTripleDesEx(Value, Guid)
            End Set
        End Property

        Public Property CIDNumber() As String
            Get
                If m_CIDNumber = String.Empty Then Return String.Empty
                Return Utility.Crypt.DecryptTripleDesEx(m_CIDNumber, Guid)
            End Get
            Set(ByVal Value As String)
                m_CIDNumber = Utility.Crypt.EncryptTripleDesEx(Value, Guid)
            End Set
        End Property

        Public ReadOnly Property StarredCIDNumber() As String
            Get
                Return "".PadLeft(CIDNumber.Length, "*")
            End Get
        End Property

        Public Property CardTypeId() As Integer
            Get
                Return m_CardTypeId
            End Get
            Set(ByVal Value As Integer)
                m_CardTypeId = Value
            End Set
        End Property

        Public Property ExpirationDate() As String
            Get
                If m_ExpirationDate = String.Empty Then Return String.Empty
                Return Utility.Crypt.DecryptTripleDesEx(m_ExpirationDate, Guid)
            End Get
            Set(ByVal Value As String)
                m_ExpirationDate = Utility.Crypt.EncryptTripleDesEx(Value, Guid)
            End Set
        End Property

        Public Property CardholderName() As String
            Get
                If m_CardholderName = String.Empty Then Return String.Empty
                Return Utility.Crypt.DecryptTripleDesEx(m_CardholderName, Guid)
            End Get
            Set(ByVal Value As String)
                m_CardholderName = Utility.Crypt.EncryptTripleDesEx(Value, Guid)
            End Set
        End Property

        Public Property Email() As String
            Get
                Return m_Email
            End Get
            Set(ByVal Value As String)
                m_Email = Value
            End Set
        End Property

        Public Property BaseSubtotal() As Double
            Get
                Return m_BaseSubtotal
            End Get
            Set(ByVal Value As Double)
                m_BaseSubtotal = Value
            End Set
        End Property

        Public Property Discount() As Double
            Get
                Return m_Discount
            End Get
            Set(ByVal Value As Double)
                m_Discount = Value
            End Set
        End Property

        Public Property Subtotal() As Double
            Get
                Return m_Subtotal
            End Get
            Set(ByVal Value As Double)
                m_Subtotal = Value
            End Set
        End Property

        Public Property Shipping() As Double
            Get
                Return m_Shipping
            End Get
            Set(ByVal Value As Double)
                m_Shipping = Value
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
                m_Tax = Value
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

        Public Property IsFreeShipping() As Boolean
            Get
                Return m_IsFreeShipping
            End Get
            Set(ByVal Value As Boolean)
                m_IsFreeShipping = Value
            End Set
        End Property

        Public Property ProcessDate() As DateTime
            Get
                Return m_ProcessDate
            End Get
            Set(ByVal Value As DateTime)
                m_ProcessDate = Value
            End Set
        End Property

        Public Property ExportDate() As DateTime
            Get
                Return m_ExportDate
            End Get
            Set(ByVal Value As DateTime)
                m_ExportDate = Value
            End Set
        End Property

        Public Property ReferralCode() As String
            Get
                Return m_ReferralCode
            End Get
            Set(ByVal Value As String)
                m_ReferralCode = Value
            End Set
        End Property

        Public Property RemoteIP() As String
            Get
                Return m_RemoteIP
            End Get
            Set(ByVal Value As String)
                m_RemoteIP = Value
            End Set
        End Property

        Public Property PromotionCode() As String
            Get
                Return m_PromotionCode
            End Get
            Set(ByVal Value As String)
                m_PromotionCode = Value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return m_Status
            End Get
            Set(ByVal Value As String)
                m_Status = Value
            End Set
        End Property

        Public Property PaymentReference() As String
            Get
                Return m_PaymentReference
            End Get
            Set(ByVal value As String)
                m_PaymentReference = value
            End Set
        End Property

        Public Property Guid() As String
            Get
                Return m_Guid
            End Get
            Set(ByVal Value As String)
                m_Guid = Value
            End Set
        End Property

        Public Property Comments() As String
            Get
                Return m_Comments
            End Get
            Set(ByVal Value As String)
                m_Comments = Value
            End Set
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
            Set(ByVal Value As Database)
                m_DB = Value
            End Set
        End Property

        Public Sub New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            m_DB = DB
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal OrderId As Integer)
            m_DB = DB
            m_OrderId = OrderId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StoreOrder WHERE OrderId = " & DB.Number(OrderId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As SqlDataReader)
            m_OrderId = Convert.ToInt32(r.Item("OrderId"))
            If IsDBNull(r.Item("OrderNo")) Then
                m_OrderNo = Nothing
            Else
                m_OrderNo = Convert.ToString(r.Item("OrderNo"))
            End If
            If IsDBNull(r.Item("HowHeardId")) Then
                m_HowHeardId = Nothing
            Else
                m_HowHeardId = Convert.ToInt32(r.Item("HowHeardId"))
            End If
            If IsDBNull(r.Item("HowHeardName")) Then
                m_HowHeardName = Nothing
            Else
                m_HowHeardName = Convert.ToString(r.Item("HowHeardName"))
            End If
            If IsDBNull(r.Item("MemberId")) Then
                m_MemberId = Nothing
            Else
                m_MemberId = Convert.ToInt32(r.Item("MemberId"))
            End If
            If IsDBNull(r.Item("BillingFirstName")) Then
                m_BillingFirstName = Nothing
            Else
                m_BillingFirstName = Convert.ToString(r.Item("BillingFirstName"))
            End If
            If IsDBNull(r.Item("BillingMiddleInitial")) Then
                m_BillingMiddleInitial = Nothing
            Else
                m_BillingMiddleInitial = Convert.ToString(r.Item("BillingMiddleInitial"))
            End If
            If IsDBNull(r.Item("BillingLastName")) Then
                m_BillingLastName = Nothing
            Else
                m_BillingLastName = Convert.ToString(r.Item("BillingLastName"))
            End If
            If IsDBNull(r.Item("BillingCompany")) Then
                m_BillingCompany = Nothing
            Else
                m_BillingCompany = Convert.ToString(r.Item("BillingCompany"))
            End If
            If IsDBNull(r.Item("BillingAddress1")) Then
                m_BillingAddress1 = Nothing
            Else
                m_BillingAddress1 = Convert.ToString(r.Item("BillingAddress1"))
            End If
            If IsDBNull(r.Item("BillingAddress2")) Then
                m_BillingAddress2 = Nothing
            Else
                m_BillingAddress2 = Convert.ToString(r.Item("BillingAddress2"))
            End If
            If IsDBNull(r.Item("BillingCity")) Then
                m_BillingCity = Nothing
            Else
                m_BillingCity = Convert.ToString(r.Item("BillingCity"))
            End If
            If IsDBNull(r.Item("BillingState")) Then
                m_BillingState = Nothing
            Else
                m_BillingState = Convert.ToString(r.Item("BillingState"))
            End If
            If IsDBNull(r.Item("BillingRegion")) Then
                m_BillingRegion = Nothing
            Else
                m_BillingRegion = Convert.ToString(r.Item("BillingRegion"))
            End If
            If IsDBNull(r.Item("BillingCountry")) Then
                m_BillingCountry = Nothing
            Else
                m_BillingCountry = Convert.ToString(r.Item("BillingCountry"))
            End If
            If IsDBNull(r.Item("BillingZip")) Then
                m_BillingZip = Nothing
            Else
                m_BillingZip = Convert.ToString(r.Item("BillingZip"))
            End If
            If IsDBNull(r.Item("BillingPhone")) Then
                m_BillingPhone = Nothing
            Else
                m_BillingPhone = Convert.ToString(r.Item("BillingPhone"))
            End If
            If IsDBNull(r.Item("CardNumber")) Then
                m_CardNumber = Nothing
            Else
                m_CardNumber = Convert.ToString(r.Item("CardNumber"))
            End If
            If IsDBNull(r.Item("CIDNumber")) Then
                m_CIDNumber = Nothing
            Else
                m_CIDNumber = Convert.ToString(r.Item("CIDNumber"))
            End If
            If IsDBNull(r.Item("CardTypeId")) Then
                m_CardTypeId = Nothing
            Else
                m_CardTypeId = Convert.ToInt32(r.Item("CardTypeId"))
            End If
            If IsDBNull(r.Item("ExpirationDate")) Then
                m_ExpirationDate = Nothing
            Else
                m_ExpirationDate = Convert.ToString(r.Item("ExpirationDate"))
            End If
            If IsDBNull(r.Item("CardholderName")) Then
                m_CardholderName = Nothing
            Else
                m_CardholderName = Convert.ToString(r.Item("CardholderName"))
            End If
            If IsDBNull(r.Item("Email")) Then
                m_Email = Nothing
            Else
                m_Email = Convert.ToString(r.Item("Email"))
            End If
            m_BaseSubtotal = Convert.ToDouble(r.Item("BaseSubtotal"))
            m_Discount = Convert.ToDouble(r.Item("Discount"))
            m_Subtotal = Convert.ToDouble(r.Item("Subtotal"))
            m_Shipping = Convert.ToDouble(r.Item("Shipping"))
            m_GiftWrapping = Convert.ToDouble(r.Item("GiftWrapping"))
            m_Tax = Convert.ToDouble(r.Item("Tax"))
            m_Total = Convert.ToDouble(r.Item("Total"))
            m_IsFreeShipping = Convert.ToBoolean(r.Item("IsFreeShipping"))
            m_CreateDate = Convert.ToDateTime(r.Item("CreateDate"))
            If IsDBNull(r.Item("ProcessDate")) Then
                m_ProcessDate = Nothing
            Else
                m_ProcessDate = Convert.ToDateTime(r.Item("ProcessDate"))
            End If

            If IsDBNull(r.Item("ExportDate")) Then
                m_ExportDate = Nothing
            Else
                m_ExportDate = Convert.ToDateTime(r.Item("ExportDate"))
            End If
            If IsDBNull(r.Item("ReferralCode")) Then
                m_ReferralCode = Nothing
            Else
                m_ReferralCode = Convert.ToString(r.Item("ReferralCode"))
            End If
            m_RemoteIP = Convert.ToString(r.Item("RemoteIP"))
            If IsDBNull(r.Item("PromotionCode")) Then
                m_PromotionCode = Nothing
            Else
                m_PromotionCode = Convert.ToString(r.Item("PromotionCode"))
            End If
            If IsDBNull(r.Item("Status")) Then
                m_Status = Nothing
            Else
                m_Status = Convert.ToString(r.Item("Status"))
            End If
            If IsDBNull(r.Item("PaymentReference")) Then
                m_PaymentReference = Nothing
            Else
                m_PaymentReference = Convert.ToString(r.Item("PaymentReference"))
            End If
            If IsDBNull(r.Item("Guid")) Then
                m_Guid = Nothing
            Else
                m_Guid = Convert.ToString(r.Item("Guid"))
            End If
            If IsDBNull(r.Item("Comments")) Then
                m_Comments = Nothing
            Else
                m_Comments = Convert.ToString(r.Item("Comments"))
            End If
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StoreOrder (" _
             & " OrderNo" _
             & ",HowHeardId" _
             & ",HowHeardName" _
             & ",MemberId" _
             & ",BillingFirstName" _
             & ",BillingMiddleInitial" _
             & ",BillingLastName" _
             & ",BillingCompany" _
             & ",BillingAddress1" _
             & ",BillingAddress2" _
             & ",BillingCity" _
             & ",BillingState" _
             & ",BillingRegion" _
             & ",BillingCountry" _
             & ",BillingZip" _
             & ",BillingPhone" _
             & ",CardNumber" _
             & ",CIDNumber" _
             & ",CardTypeId" _
             & ",ExpirationDate" _
             & ",CardholderName" _
             & ",Email" _
             & ",BaseSubtotal" _
             & ",Discount" _
             & ",Subtotal" _
             & ",Shipping" _
             & ",GiftWrapping" _
             & ",Tax" _
             & ",Total" _
             & ",IsFreeShipping" _
             & ",CreateDate" _
             & ",ProcessDate" _
             & ",ExportDate" _
             & ",ReferralCode" _
             & ",RemoteIP" _
             & ",PromotionCode" _
             & ",Status" _
             & ",PaymentReference" _
             & ",Guid" _
             & ",Comments" _
             & ") VALUES (" _
             & m_DB.Quote(OrderNo) _
             & "," & m_DB.NullNumber(HowHeardId) _
             & "," & m_DB.Quote(HowHeardName) _
             & "," & m_DB.NullNumber(MemberId) _
             & "," & m_DB.Quote(BillingFirstName) _
             & "," & m_DB.Quote(BillingMiddleInitial) _
             & "," & m_DB.Quote(BillingLastName) _
             & "," & m_DB.Quote(BillingCompany) _
             & "," & m_DB.Quote(BillingAddress1) _
             & "," & m_DB.Quote(BillingAddress2) _
             & "," & m_DB.Quote(BillingCity) _
             & "," & m_DB.Quote(BillingState) _
             & "," & m_DB.Quote(BillingRegion) _
             & "," & m_DB.Quote(BillingCountry) _
             & "," & m_DB.Quote(BillingZip) _
             & "," & m_DB.Quote(BillingPhone) _
             & "," & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(CardNumber, Guid)) _
             & "," & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(CIDNumber, Guid)) _
             & "," & m_DB.NullNumber(CardTypeId) _
             & "," & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(ExpirationDate, Guid)) _
             & "," & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(CardholderName, Guid)) _
             & "," & m_DB.Quote(Email) _
             & "," & m_DB.Number(BaseSubtotal) _
             & "," & m_DB.Number(Discount) _
             & "," & m_DB.Number(Subtotal) _
             & "," & m_DB.Number(Shipping) _
             & "," & m_DB.Number(GiftWrapping) _
             & "," & m_DB.Number(Tax) _
             & "," & m_DB.Number(Total) _
             & "," & CInt(IsFreeShipping) _
             & "," & m_DB.NullQuote(Now) _
             & "," & m_DB.NullQuote(ProcessDate) _
             & "," & m_DB.NullQuote(ExportDate) _
             & "," & m_DB.Quote(ReferralCode) _
             & "," & m_DB.Quote(RemoteIP) _
             & "," & m_DB.Quote(PromotionCode) _
             & "," & m_DB.Quote(Status) _
             & "," & m_DB.Quote(PaymentReference) _
             & "," & m_DB.Quote(Guid) _
             & "," & m_DB.Quote(Comments) _
             & ")"

            OrderId = m_DB.InsertSQL(SQL)

            Return OrderId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StoreOrder SET " _
             & " OrderNo = " & m_DB.Quote(OrderNo) _
             & ",HowHeardId = " & m_DB.NullNumber(HowHeardId) _
             & ",HowHeardName = " & m_DB.Quote(HowHeardName) _
             & ",MemberId = " & m_DB.NullNumber(MemberId) _
             & ",BillingFirstName = " & m_DB.Quote(BillingFirstName) _
             & ",BillingMiddleInitial = " & m_DB.Quote(BillingMiddleInitial) _
             & ",BillingLastName = " & m_DB.Quote(BillingLastName) _
             & ",BillingCompany = " & m_DB.Quote(BillingCompany) _
             & ",BillingAddress1 = " & m_DB.Quote(BillingAddress1) _
             & ",BillingAddress2 = " & m_DB.Quote(BillingAddress2) _
             & ",BillingCity = " & m_DB.Quote(BillingCity) _
             & ",BillingState = " & m_DB.Quote(BillingState) _
             & ",BillingRegion = " & m_DB.Quote(BillingRegion) _
             & ",BillingCountry = " & m_DB.Quote(BillingCountry) _
             & ",BillingZip = " & m_DB.Quote(BillingZip) _
             & ",BillingPhone = " & m_DB.Quote(BillingPhone) _
             & ",CardNumber = " & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(CardNumber, Guid)) _
             & ",CIDNumber = " & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(CIDNumber, Guid)) _
             & ",CardTypeId = " & m_DB.NullNumber(CardTypeId) _
             & ",ExpirationDate = " & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(ExpirationDate, Guid)) _
             & ",CardholderName = " & m_DB.Quote(Utility.Crypt.EncryptTripleDesEx(CardholderName, Guid)) _
             & ",Email = " & m_DB.Quote(Email) _
             & ",BaseSubtotal = " & m_DB.Number(BaseSubtotal) _
             & ",Discount = " & m_DB.Number(Discount) _
             & ",Subtotal = " & m_DB.Number(Subtotal) _
             & ",Shipping = " & m_DB.Number(Shipping) _
             & ",GiftWrapping = " & m_DB.Number(GiftWrapping) _
             & ",Tax = " & m_DB.Number(Tax) _
             & ",Total = " & m_DB.Number(Total) _
             & ",IsFreeShipping = " & CInt(IsFreeShipping) _
             & ",ProcessDate = " & m_DB.NullQuote(ProcessDate) _
             & ",ExportDate = " & m_DB.NullQuote(ExportDate) _
             & ",ReferralCode = " & m_DB.Quote(ReferralCode) _
             & ",RemoteIP = " & m_DB.Quote(RemoteIP) _
             & ",PromotionCode = " & m_DB.Quote(PromotionCode) _
             & ",Status = " & m_DB.Quote(Status) _
             & ",PaymentReference = " & m_DB.Quote(PaymentReference) _
             & ",Guid = " & m_DB.Quote(Guid) _
             & ",Comments = " & m_DB.Quote(Comments) _
             & " WHERE OrderId = " & m_DB.Quote(OrderId)

            m_DB.ExecuteSQL(SQL)
        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String
            SQL = "DELETE FROM StoreOrder WHERE OrderId = " & m_DB.Quote(OrderId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StoreOrderCollection
        Inherits GenericCollection(Of StoreOrderRow)
    End Class

End Namespace


