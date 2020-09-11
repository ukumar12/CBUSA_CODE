
Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class TwoPriceBuilderTakeOffProductPendingRowBase
        Private m_DB As Database
        Private m_TwoPriceBuilderTakeOffProductPendingID As Integer = Nothing
        Private m_TwoPriceOrderID As Integer = Nothing
        Private m_TwoPriceCampaignID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_SpecialOrderProductID As Integer = Nothing
        Private m_Quantity As Integer = Nothing
        Private m_SortOrder As Integer = Nothing
        Private m_VendorSku As String = Nothing
        Private m_VendorPrice As Double = Nothing
        Private m_BuilderID As Integer = Nothing
        Private m_PriceRequestState As Integer = Nothing
        Private m_SubstituteProductID As Integer = Nothing
        Public Property TwoPriceBuilderTakeOffProductPendingID As Integer
            Get
                Return m_TwoPriceBuilderTakeOffProductPendingID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceBuilderTakeOffProductPendingID = value
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
        Public Property TwoPriceOrderID As Integer
            Get
                Return m_TwoPriceOrderID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceOrderID = value
            End Set
        End Property

        Public Property TwoPriceCampaignID As Integer
            Get
                Return m_TwoPriceCampaignID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceCampaignID = value
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
                m_BuilderID = Value
            End Set
        End Property
        Public Property PriceRequestState As Integer
            Get
                Return m_PriceRequestState
            End Get
            Set(ByVal Value As Integer)
                m_PriceRequestState = Value
            End Set
        End Property
        Public Property ProductID As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = value
            End Set
        End Property

        Public Property SpecialOrderProductID As Integer
            Get
                Return m_SpecialOrderProductID
            End Get
            Set(ByVal Value As Integer)
                m_SpecialOrderProductID = value
            End Set
        End Property

        Public Property Quantity As Integer
            Get
                Return m_Quantity
            End Get
            Set(ByVal Value As Integer)
                m_Quantity = value
            End Set
        End Property

        Public Property SortOrder As Integer
            Get
                Return m_SortOrder
            End Get
            Set(ByVal Value As Integer)
                m_SortOrder = value
            End Set
        End Property

        Public Property VendorSku As String
            Get
                Return m_VendorSku
            End Get
            Set(ByVal Value As String)
                m_VendorSku = value
            End Set
        End Property

        Public Property VendorPrice As Double
            Get
                Return m_VendorPrice
            End Get
            Set(ByVal Value As Double)
                m_VendorPrice = value
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

        Public Sub New(ByVal DB As Database, TwoPriceBuilderTakeOffProductPendingID As Integer)
            m_DB = DB
            m_TwoPriceBuilderTakeOffProductPendingID = TwoPriceBuilderTakeOffProductPendingID
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceBuilderTakeOffProductPending WHERE TwoPriceBuilderTakeOffProductPendingID = " & DB.Number(TwoPriceBuilderTakeOffProductPendingID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_TwoPriceBuilderTakeOffProductPendingID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceBuilderTakeOffProductPendingID = Core.GetInt(r.Item("TwoPriceBuilderTakeOffProductPendingID"))
            m_TwoPriceOrderID = Core.GetInt(r.Item("TwoPriceOrderID"))
            m_TwoPriceCampaignID = Core.GetInt(r.Item("TwoPriceCampaignID"))
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_ProductID = Core.GetInt(r.Item("ProductID"))
            m_SpecialOrderProductID = Core.GetInt(r.Item("SpecialOrderProductID"))
            m_Quantity = Core.GetInt(r.Item("Quantity"))
            m_VendorSku = Core.GetString(r.Item("VendorSku"))
            m_VendorPrice = Core.GetDouble(r.Item("VendorPrice"))
            m_BuilderID = Core.GetInt(r.Item("BuilderID"))
            m_PriceRequestState = Core.GetInt(r.Item("PriceRequestState"))
            m_SubstituteProductID = Core.GetInt(r.Item("SubstituteProductID"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String

            Dim MaxSortOrder As Integer = DB.ExecuteScalar("select top 1 SortOrder from TwoPriceBuilderTakeOffProductPending order by SortOrder desc")
            MaxSortOrder += 1

            SQL = " INSERT INTO TwoPriceBuilderTakeOffProductPending (" _
             & " TwoPriceOrderID" _
             & ",TwoPriceCampaignID" _
             & ",VendorID" _
             & ",ProductID" _
             & ",SpecialOrderProductID" _
             & ",Quantity" _
             & ",SortOrder" _
             & ",VendorSku" _
             & ",VendorPrice" _
             & ",BuilderID" _
             & ",PriceRequestState" _
             & ",SubstituteProductID" _
             & ") VALUES (" _
             & m_DB.NullNumber(TwoPriceOrderID) _
             & "," & m_DB.NullNumber(TwoPriceCampaignID) _
             & "," & m_DB.NullNumber(VendorID) _
             & "," & m_DB.NullNumber(ProductID) _
             & "," & m_DB.NullNumber(SpecialOrderProductID) _
             & "," & m_DB.Number(Quantity) _
             & "," & MaxSortOrder _
             & "," & m_DB.Quote(VendorSku) _
             & "," & m_DB.Number(VendorPrice) _
             & "," & m_DB.NullNumber(BuilderID) _
             & "," & m_DB.NullNumber(PriceRequestState) _
             & "," & m_DB.NullNumber(SubstituteProductID) _
             & ")"

            TwoPriceBuilderTakeOffProductPendingID = m_DB.InsertSQL(SQL)

            Return TwoPriceBuilderTakeOffProductPendingID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceBuilderTakeOffProductPending WITH (ROWLOCK) SET " _
             & " TwoPriceOrderID = " & m_DB.NullNumber(TwoPriceOrderID) _
             & ",TwoPriceCampaignID = " & m_DB.NullNumber(TwoPriceCampaignID) _
             & ",VendorID = " & m_DB.NullNumber(VendorID) _
             & ",ProductID = " & m_DB.NullNumber(ProductID) _
             & ",SpecialOrderProductID = " & m_DB.NullNumber(SpecialOrderProductID) _
             & ",Quantity = " & m_DB.Number(Quantity) _
             & ",VendorSku = " & m_DB.Quote(VendorSku) _
             & ",VendorPrice = " & m_DB.Number(VendorPrice) _
             & ",BuilderID = " & m_DB.NullNumber(BuilderID) _
             & ",PriceRequestState = " & m_DB.NullNumber(PriceRequestState) _
             & ",SubstituteProductID = " & m_DB.NullNumber(SubstituteProductID) _
             & " WHERE TwoPriceBuilderTakeOffProductPendingID = " & m_DB.Quote(TwoPriceBuilderTakeOffProductPendingID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TwoPriceBuilderTakeOffProductPendingCollection
        Inherits GenericCollection(Of TwoPriceBuilderTakeOffProductPendingRow)
    End Class

    Public Enum PriceRequestState
        Priced = 0
        Init = 1
        RequestPending = 2
        SubstitutionAvailable = 3
        VendorPriced = 4
        UnknownPriceAccepted = 5
        SubstituteAccepted = 6
        Omit = 7
    End Enum

End Namespace

