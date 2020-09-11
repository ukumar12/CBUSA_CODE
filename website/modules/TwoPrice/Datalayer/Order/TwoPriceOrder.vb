Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

Public Class TwoPriceOrderRow
        Inherits TwoPriceOrderRowBase

        'Pointer to twoprice property for compatibilities sake
        Public Property OrderID() As Integer
            Get
                Return TwoPriceOrderID
            End Get
            Set(value As Integer)
                TwoPriceOrderID = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, TwoPriceOrderID As Integer)
            MyBase.New(DB, TwoPriceOrderID)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal TwoPriceOrderID As Integer) As TwoPriceOrderRow
            Dim row As TwoPriceOrderRow

            row = New TwoPriceOrderRow(DB, TwoPriceOrderID)
            row.Load()

            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal TwoPriceOrderID As Integer)
            Dim SQL As String

            SQL = "DELETE FROM TwoPriceOrder WHERE TwoPriceOrderID = " & DB.Number(TwoPriceOrderID)
            DB.ExecuteSQL(SQL)
        End Sub

        Public Sub Remove()
            RemoveRow(DB, TwoPriceOrderID)
        End Sub

        Public Shared Function GetList(ByVal DB As Database, Optional ByVal SortBy As String = "", Optional SortOrder As String = "ASC") As DataTable
            Dim SQL As String = "select * from TwoPriceOrder"
            If Not SortBy = String.Empty Then
                SortBy = Core.ProtectParam(SortBy)
                SortOrder = Core.ProtectParam(SortOrder)

                SQL &= " order by " & SortBy & " " & SortOrder
            End If
            Return DB.GetDataTable(SQL)
        End Function

        'Custom Methods

        Public Shared Function GetOrderProducts(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            'Dim sql As String = _
            '      " select o.*, p.Product as ProductName, p.*, coalesce(o.VendorSku,p.SKU) as ProductSku, o.DropId, o.VendorPrice as Price " _
            '    & " from TwoPriceOrderProduct o inner join Product p on o.ProductId=p.ProductId " _
            '    & " where o.TwoPriceOrderId=" & DB.Number(OrderId)

            '************  Changed by Apala (Medullus) on 07.02.2018 for mGuard#T-1086
            'Dim sql As String =
            '      " select o.TwoPriceOrderProductID, o.TwoPriceOrderID, o.Quantity, p.ProductId, p.Product, p.Product as ProductName, coalesce(o.VendorSku,p.SKU) as ProductSku, o.DropId, o.VendorPrice, o.VendorPrice as Price, o.SortOrder " _
            '    & " from TwoPriceOrderProduct o inner join Product p on o.ProductId=p.ProductId " _
            '    & " where o.TwoPriceOrderId=" & DB.Number(OrderId) _
            '    & " union " _
            '    & " select o.TwoPriceOrderProductID, o.TwoPriceOrderID, o.Quantity, p.SpecialOrderProductId as ProductId, p.SpecialOrderProduct as Product, p.SpecialOrderProduct as ProductName, '' as ProductSku, o.DropId, o.VendorPrice, o.VendorPrice as Price, o.SortOrder " _
            '    & " from TwoPriceOrderProduct o inner join SpecialOrderProduct p on o.SpecialOrderProductId=p.SpecialOrderProductId " _
            '    & " where o.TwoPriceOrderId=" & DB.Number(OrderId)

            '************  Changed by Subhra (Medullus) on 13.03.2018 for mGuard#T-1120
            Dim sql As String =
                  " select o.TwoPriceOrderProductID, o.TwoPriceOrderID, o.Quantity, p.ProductId, p.Product, p.Product as ProductName, p.SKU as ProductSku, " _
                & " o.DropId, o.VendorPrice, o.VendorPrice as Price, o.SortOrder " _
                & " from TwoPriceOrderProduct o inner join Product p on o.ProductId=p.ProductId " _
                & " where o.TwoPriceOrderId=" & DB.Number(OrderId) _
                & " union " _
                & " select o.TwoPriceOrderProductID, o.TwoPriceOrderID, o.Quantity, p.SpecialOrderProductId as ProductId, p.SpecialOrderProduct as Product, " _
                & " p.SpecialOrderProduct as ProductName, '' as ProductSku, o.DropId, o.VendorPrice, o.VendorPrice as Price, o.SortOrder " _
                & " from TwoPriceOrderProduct o inner join SpecialOrderProduct p on o.SpecialOrderProductId=p.SpecialOrderProductId " _
                & " where o.TwoPriceOrderId=" & DB.Number(OrderId)

            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(Sql)
        End Function
        'Adding cbusa sku seperately
        Public Shared Function GetOrderProductsForDisplay(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            'Dim sql As String = _
            '      " select o.*, p.Product as ProductName, p.*, o.VendorSku as ProductSku, p.sku as cbusasku ,o.DropId, o.VendorPrice as Price " _
            '    & " from TwoPriceOrderProduct o inner join Product p on o.ProductId=p.ProductId " _
            '    & " where o.TwoPriceOrderId=" & DB.Number(OrderId)

            '************  Changed by Apala (Medullus) on 07.02.2018 for mGuard#T-1086
            Dim sql As String =
                  " select o.*, p.Product as ProductName, o.VendorSku as ProductSku, p.sku as cbusasku ,o.DropId, o.VendorPrice as Price " _
                & " from TwoPriceOrderProduct o inner join Product p on o.ProductId=p.ProductId " _
                & " where o.TwoPriceOrderId=" & DB.Number(OrderId) & " UNION " _
                & "select o.*, p.SpecialOrderProduct as ProductName, o.VendorSku as ProductSku, '' as cbusasku ,o.DropId, o.VendorPrice as Price " _
                & " from TwoPriceOrderProduct o inner join SpecialOrderProduct p on o.SpecialOrderProductId=p.SpecialOrderProductId " _
                & " where o.TwoPriceOrderId=" & DB.Number(OrderId)

            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function
        Public Shared Function GetOrderDrops(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = "select * from TwoPriceOrderDrop where OrderId=" & DB.Number(OrderId)
            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

    End Class

    Public MustInherit Class TwoPriceOrderRowBase
        Private m_DB As Database
        Private m_TwoPriceOrderID As Integer = Nothing
        Private m_HistoricID As Integer = Nothing
        Private m_OrderNumber As String = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_ProjectID As Integer = Nothing
        Private m_HistoricVendorID As Integer = Nothing
        Private m_HistoricBuilderID As Integer = Nothing
        Private m_HistoricProjectID As Integer = Nothing
        Private m_Title As String = Nothing
        Private m_PONumber As String = Nothing
        Private m_OrdererFirstName As String = Nothing
        Private m_OrdererLastName As String = Nothing
        Private m_OrdererEmail As String = Nothing
        Private m_OrdererPhone As String = Nothing
        Private m_SuperFirstName As String = Nothing
        Private m_SuperLastName As String = Nothing
        Private m_SuperEmail As String = Nothing
        Private m_SuperPhone As String = Nothing
        Private m_Subtotal As Double = Nothing
        Private m_Tax As Double = Nothing
        Private m_Total As Double = Nothing
        Private m_RequestedDelivery As DateTime = Nothing
        Private m_OrderStatusID As Integer = Nothing
        Private m_HistoricOrderStatusID As Integer = Nothing
        Private m_DeliveryInstructions As String = Nothing
        Private m_Notes As String = Nothing
        Private m_RemoteIP As String = Nothing
        Private m_Created As DateTime = Nothing
        Private m_CreatorBuilderID As Integer = Nothing
        Private m_VendorNotes As String = Nothing
        Private m_TwoPriceTakeoffID As Integer = Nothing
        Private m_SalesRepVendorAccountID As Integer = Nothing
        Private m_Updated As DateTime = Nothing
        Private m_TwoPriceCampaignId As Integer = Nothing
        Private m_TaxRate As Double = Nothing
        Private m_ImportedTakeOffID As String = Nothing

        Public Property TwoPriceOrderID As Integer
            Get
                Return m_TwoPriceOrderID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceOrderID = value
            End Set
        End Property

        Public Property HistoricID As Integer
            Get
                Return m_HistoricID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricID = value
            End Set
        End Property

        Public Property OrderNumber As String
            Get
                Return m_OrderNumber
            End Get
            Set(ByVal Value As String)
                m_OrderNumber = value
            End Set
        End Property

        Public Property VendorID As Integer
            Get
                Return m_VendorID
            End Get
            Set(ByVal Value As Integer)
                m_VendorID = value
            End Set
        End Property

        Public Property BuilderID As Integer
            Get
                Return m_BuilderID
            End Get
            Set(ByVal Value As Integer)
                m_BuilderID = value
            End Set
        End Property

        Public Property ProjectID As Integer
            Get
                Return m_ProjectID
            End Get
            Set(ByVal Value As Integer)
                m_ProjectID = value
            End Set
        End Property

        Public Property HistoricVendorID As Integer
            Get
                Return m_HistoricVendorID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricVendorID = value
            End Set
        End Property

        Public Property HistoricBuilderID As Integer
            Get
                Return m_HistoricBuilderID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricBuilderID = value
            End Set
        End Property

        Public Property HistoricProjectID As Integer
            Get
                Return m_HistoricProjectID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricProjectID = value
            End Set
        End Property

        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(ByVal Value As String)
                m_Title = value
            End Set
        End Property

        Public Property PONumber As String
            Get
                Return m_PONumber
            End Get
            Set(ByVal Value As String)
                m_PONumber = value
            End Set
        End Property

        Public Property OrdererFirstName As String
            Get
                Return m_OrdererFirstName
            End Get
            Set(ByVal Value As String)
                m_OrdererFirstName = value
            End Set
        End Property

        Public Property OrdererLastName As String
            Get
                Return m_OrdererLastName
            End Get
            Set(ByVal Value As String)
                m_OrdererLastName = value
            End Set
        End Property

        Public Property OrdererEmail As String
            Get
                Return m_OrdererEmail
            End Get
            Set(ByVal Value As String)
                m_OrdererEmail = value
            End Set
        End Property

        Public Property OrdererPhone As String
            Get
                Return m_OrdererPhone
            End Get
            Set(ByVal Value As String)
                m_OrdererPhone = value
            End Set
        End Property

        Public Property SuperFirstName As String
            Get
                Return m_SuperFirstName
            End Get
            Set(ByVal Value As String)
                m_SuperFirstName = value
            End Set
        End Property

        Public Property SuperLastName As String
            Get
                Return m_SuperLastName
            End Get
            Set(ByVal Value As String)
                m_SuperLastName = value
            End Set
        End Property

        Public Property SuperEmail As String
            Get
                Return m_SuperEmail
            End Get
            Set(ByVal Value As String)
                m_SuperEmail = value
            End Set
        End Property

        Public Property SuperPhone As String
            Get
                Return m_SuperPhone
            End Get
            Set(ByVal Value As String)
                m_SuperPhone = value
            End Set
        End Property

        Public Property Subtotal As Double
            Get
                Return m_Subtotal
            End Get
            Set(ByVal Value As Double)
                m_Subtotal = value
            End Set
        End Property

        Public Property Tax As Double
            Get
                Return m_Tax
            End Get
            Set(ByVal Value As Double)
                m_Tax = value
            End Set
        End Property
        Public Property TaxRate() As Double
            Get
                Return m_TaxRate
            End Get
            Set(ByVal Value As Double)
                m_TaxRate = Value
            End Set
        End Property
        Public Property Total As Double
            Get
                Return m_Total
            End Get
            Set(ByVal Value As Double)
                m_Total = value
            End Set
        End Property

        Public Property RequestedDelivery As DateTime
            Get
                Return m_RequestedDelivery
            End Get
            Set(ByVal Value As DateTime)
                m_RequestedDelivery = value
            End Set
        End Property

        Public Property OrderStatusID As Integer
            Get
                Return m_OrderStatusID
            End Get
            Set(ByVal Value As Integer)
                m_OrderStatusID = value
            End Set
        End Property

        Public Property HistoricOrderStatusID As Integer
            Get
                Return m_HistoricOrderStatusID
            End Get
            Set(ByVal Value As Integer)
                m_HistoricOrderStatusID = value
            End Set
        End Property

        Public Property DeliveryInstructions As String
            Get
                Return m_DeliveryInstructions
            End Get
            Set(ByVal Value As String)
                m_DeliveryInstructions = value
            End Set
        End Property

        Public Property Notes As String
            Get
                Return m_Notes
            End Get
            Set(ByVal Value As String)
                m_Notes = value
            End Set
        End Property

        Public Property RemoteIP As String
            Get
                Return m_RemoteIP
            End Get
            Set(ByVal Value As String)
                m_RemoteIP = value
            End Set
        End Property

        Public Property Created As DateTime
            Get
                Return m_Created
            End Get
            Set(ByVal Value As DateTime)
                m_Created = value
            End Set
        End Property

        Public Property CreatorBuilderID As Integer
            Get
                Return m_CreatorBuilderID
            End Get
            Set(ByVal Value As Integer)
                m_CreatorBuilderID = value
            End Set
        End Property

        Public Property VendorNotes As String
            Get
                Return m_VendorNotes
            End Get
            Set(ByVal Value As String)
                m_VendorNotes = value
            End Set
        End Property

        Public Property TwoPriceTakeoffID As Integer
            Get
                Return m_TwoPriceTakeoffID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceTakeoffID = value
            End Set
        End Property

        Public Property SalesRepVendorAccountID As Integer
            Get
                Return m_SalesRepVendorAccountID
            End Get
            Set(ByVal Value As Integer)
                m_SalesRepVendorAccountID = value
            End Set
        End Property

        Public Property Updated As DateTime
            Get
                Return m_Updated
            End Get
            Set(ByVal Value As DateTime)
                m_Updated = value
            End Set
        End Property

        Public Property TwoPriceCampaignId As Integer
            Get
                Return m_TwoPriceCampaignId
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceCampaignId = value
            End Set
        End Property

        Public Property ImportedTakeOffID As String
            Get
                Return m_ImportedTakeOffID
            End Get
            Set(ByVal Value As String)
                m_ImportedTakeOffID = Value
            End Set
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

        Public Sub New(ByVal DB As Database, TwoPriceOrderID As Integer)
            m_DB = DB
            m_TwoPriceOrderID = TwoPriceOrderID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceOrder WHERE TwoPriceOrderID = " & DB.Number(TwoPriceOrderID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_TwoPriceOrderID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceOrderID = Core.GetInt(r.Item("TwoPriceOrderID"))
            m_HistoricID = Core.GetInt(r.Item("HistoricID"))
            m_OrderNumber = Core.GetString(r.Item("OrderNumber"))
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_BuilderID = Core.GetInt(r.Item("BuilderID"))
            m_ProjectID = Core.GetInt(r.Item("ProjectID"))
            m_HistoricVendorID = Core.GetInt(r.Item("HistoricVendorID"))
            m_HistoricBuilderID = Core.GetInt(r.Item("HistoricBuilderID"))
            m_HistoricProjectID = Core.GetInt(r.Item("HistoricProjectID"))
            m_Title = Core.GetString(r.Item("Title"))
            m_PONumber = Core.GetString(r.Item("PONumber"))
            m_OrdererFirstName = Core.GetString(r.Item("OrdererFirstName"))
            m_OrdererLastName = Core.GetString(r.Item("OrdererLastName"))
            m_OrdererEmail = Core.GetString(r.Item("OrdererEmail"))
            m_OrdererPhone = Core.GetString(r.Item("OrdererPhone"))
            m_SuperFirstName = Core.GetString(r.Item("SuperFirstName"))
            m_SuperLastName = Core.GetString(r.Item("SuperLastName"))
            m_SuperEmail = Core.GetString(r.Item("SuperEmail"))
            m_SuperPhone = Core.GetString(r.Item("SuperPhone"))
            m_Subtotal = Core.GetDouble(r.Item("Subtotal"))
            m_Tax = Core.GetDouble(r.Item("Tax"))
            m_Total = Core.GetDouble(r.Item("Total"))
            m_RequestedDelivery = Core.GetDate(r.Item("RequestedDelivery"))
            m_OrderStatusID = Core.GetInt(r.Item("OrderStatusID"))
            m_HistoricOrderStatusID = Core.GetInt(r.Item("HistoricOrderStatusID"))
            m_DeliveryInstructions = Core.GetString(r.Item("DeliveryInstructions"))
            m_Notes = Core.GetString(r.Item("Notes"))
            m_RemoteIP = Core.GetString(r.Item("RemoteIP"))
            m_Created = Core.GetDate(r.Item("Created"))
            m_CreatorBuilderID = Core.GetInt(r.Item("CreatorBuilderID"))
            m_VendorNotes = Core.GetString(r.Item("VendorNotes"))
            m_TwoPriceTakeoffID = Core.GetInt(r.Item("TwoPriceTakeoffID"))
            m_SalesRepVendorAccountID = Core.GetInt(r.Item("SalesRepVendorAccountID"))
            m_Updated = Core.GetDate(r.Item("Updated"))
            m_TwoPriceCampaignId = Core.GetInt(r.Item("TwoPriceCampaignId"))
            m_TaxRate = Core.GetDouble(r.Item("TaxRate"))
            m_ImportedTakeOffID = Core.GetString(r.Item("ImportedTakeOffID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String = String.Empty

            Created = Now
            Updated = Now

            SQL = " INSERT INTO TwoPriceOrder (" _
                & " HistoricID" _
                & ",OrderNumber" _
                & ",VendorID" _
                & ",BuilderID" _
                & ",ProjectID" _
                & ",HistoricVendorID" _
                & ",HistoricBuilderID" _
                & ",HistoricProjectID" _
                & ",Title" _
                & ",PONumber" _
                & ",OrdererFirstName" _
                & ",OrdererLastName" _
                & ",OrdererEmail" _
                & ",OrdererPhone" _
                & ",SuperFirstName" _
                & ",SuperLastName" _
                & ",SuperEmail" _
                & ",SuperPhone" _
                & ",Subtotal" _
                & ",Tax" _
                & ",Total" _
                & ",RequestedDelivery" _
                & ",OrderStatusID" _
                & ",HistoricOrderStatusID" _
                & ",DeliveryInstructions" _
                & ",Notes" _
                & ",RemoteIP" _
                & ",Created" _
                & ",CreatorBuilderID" _
                & ",VendorNotes" _
                & ",TwoPriceTakeoffID" _
                & ",Updated" _
                & ",TaxRate" _
                & ",TwoPriceCampaignId" _
                & ",ImportedTakeOffID" _
                & ") VALUES (" _
                & m_DB.NullNumber(HistoricID) _
                & "," & m_DB.Quote(OrderNumber) _
                & "," & m_DB.NullNumber(VendorID) _
                & "," & m_DB.NullNumber(BuilderID) _
                & "," & m_DB.NullNumber(ProjectID) _
                & "," & m_DB.NullNumber(HistoricVendorID) _
                & "," & m_DB.NullNumber(HistoricBuilderID) _
                & "," & m_DB.NullNumber(HistoricProjectID) _
                & "," & m_DB.Quote(Title) _
                & "," & m_DB.Quote(PONumber) _
                & "," & m_DB.Quote(OrdererFirstName) _
                & "," & m_DB.Quote(OrdererLastName) _
                & "," & m_DB.Quote(OrdererEmail) _
                & "," & m_DB.Quote(OrdererPhone) _
                & "," & m_DB.Quote(SuperFirstName) _
                & "," & m_DB.Quote(SuperLastName) _
                & "," & m_DB.Quote(SuperEmail) _
                & "," & m_DB.Quote(SuperPhone) _
                & "," & m_DB.Number(Subtotal) _
                & "," & m_DB.Number(Tax) _
                & "," & m_DB.Number(Total) _
                & "," & m_DB.NullQuote(RequestedDelivery) _
                & "," & m_DB.NullNumber(OrderStatusID) _
                & "," & m_DB.NullNumber(HistoricOrderStatusID) _
                & "," & m_DB.Quote(DeliveryInstructions) _
                & "," & m_DB.Quote(Notes) _
                & "," & m_DB.Quote(RemoteIP) _
                & "," & m_DB.NullQuote(Created) _
                & "," & m_DB.NullNumber(CreatorBuilderID) _
                & "," & m_DB.Quote(VendorNotes) _
                & "," & m_DB.NullNumber(TwoPriceTakeoffID) _
                & "," & m_DB.NullQuote(Updated) _
                & "," & m_DB.Number(TaxRate) _
                & "," & m_DB.NullNumber(TwoPriceCampaignId) _
                & "," & m_DB.Quote(ImportedTakeOffID) _
                & ")"

            TwoPriceOrderID = m_DB.InsertSQL(SQL)

            Return TwoPriceOrderID
        End Function

        Public Overridable Sub Update()

            Updated = Now

            Dim SQL As String

            SQL = " UPDATE TwoPriceOrder WITH (ROWLOCK) SET " _
                & " HistoricID = " & m_DB.NullNumber(HistoricID) _
                & ",OrderNumber = " & m_DB.Quote(OrderNumber) _
                & ",VendorID = " & m_DB.NullNumber(VendorID) _
                & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
                & ",ProjectID = " & m_DB.NullNumber(ProjectID) _
                & ",HistoricVendorID = " & m_DB.NullNumber(HistoricVendorID) _
                & ",HistoricBuilderID = " & m_DB.NullNumber(HistoricBuilderID) _
                & ",HistoricProjectID = " & m_DB.NullNumber(HistoricProjectID) _
                & ",Title = " & m_DB.Quote(Title) _
                & ",PONumber = " & m_DB.Quote(PONumber) _
                & ",OrdererFirstName = " & m_DB.Quote(OrdererFirstName) _
                & ",OrdererLastName = " & m_DB.Quote(OrdererLastName) _
                & ",OrdererEmail = " & m_DB.Quote(OrdererEmail) _
                & ",OrdererPhone = " & m_DB.Quote(OrdererPhone) _
                & ",SuperFirstName = " & m_DB.Quote(SuperFirstName) _
                & ",SuperLastName = " & m_DB.Quote(SuperLastName) _
                & ",SuperEmail = " & m_DB.Quote(SuperEmail) _
                & ",SuperPhone = " & m_DB.Quote(SuperPhone) _
                & ",Subtotal = " & m_DB.Number(Subtotal) _
                & ",Tax = " & m_DB.Number(Tax) _
                & ",Total = " & m_DB.Number(Total) _
                & ",RequestedDelivery = " & m_DB.NullQuote(RequestedDelivery) _
                & ",OrderStatusID = " & m_DB.NullNumber(OrderStatusID) _
                & ",HistoricOrderStatusID = " & m_DB.NullNumber(HistoricOrderStatusID) _
                & ",DeliveryInstructions = " & m_DB.Quote(DeliveryInstructions) _
                & ",Notes = " & m_DB.Quote(Notes) _
                & ",RemoteIP = " & m_DB.Quote(RemoteIP) _
                & ",Created = " & m_DB.NullQuote(Created) _
                & ",CreatorBuilderID = " & m_DB.NullNumber(CreatorBuilderID) _
                & ",VendorNotes = " & m_DB.Quote(VendorNotes) _
                & ",TwoPriceTakeoffID = " & m_DB.NullNumber(TwoPriceTakeoffID) _
                & ",SalesRepVendorAccountID = " & m_DB.NullNumber(SalesRepVendorAccountID) _
                & ",Updated = " & m_DB.NullQuote(Updated) _
                & ",TwoPriceCampaignId = " & m_DB.NullNumber(TwoPriceCampaignId) _
                & ",TaxRate = " & m_DB.Number(TaxRate) _
                & ",ImportedTakeOffID = " & m_DB.Quote(ImportedTakeOffID) _
                & " WHERE TwoPriceOrderID = " & m_DB.Quote(TwoPriceOrderID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TwoPriceOrderCollection
        Inherits GenericCollection(Of TwoPriceOrderRow)
    End Class

End Namespace

