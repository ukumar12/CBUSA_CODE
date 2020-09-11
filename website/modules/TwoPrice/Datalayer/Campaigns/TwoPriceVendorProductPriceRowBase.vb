Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public MustInherit Class TwoPriceVendorProductPriceRowBase
        Private m_DB As Database
        Private m_TwoPriceProductPriceID As Integer = Nothing
        Private m_VendorID As Integer = Nothing
        Private m_ProductID As Integer = Nothing
        Private m_TwoPriceCampaignID As Integer = Nothing
        Private m_Price As Double = Nothing
        Private m_LastUpdated As DateTime = Nothing
        Private m_Comments As String = Nothing
        Private m_Submitted As Boolean = Nothing

        Public Property TwoPriceProductPriceID As Integer
            Get
                Return m_TwoPriceProductPriceID
            End Get
            Set(ByVal Value As Integer)
                m_TwoPriceProductPriceID = value
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

        Public Property ProductID As Integer
            Get
                Return m_ProductID
            End Get
            Set(ByVal Value As Integer)
                m_ProductID = value
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

        Public Property Price As Double
            Get
                Return m_Price
            End Get
            Set(ByVal Value As Double)
                m_Price = value
            End Set
        End Property

        Public Property LastUpdated As DateTime
            Get
                Return m_LastUpdated
            End Get
            Set(ByVal Value As DateTime)
                m_LastUpdated = value
            End Set
        End Property

        Public Property Comments As String
            Get
                Return m_Comments
            End Get
            Set(ByVal Value As String)
                m_Comments = value
            End Set
        End Property

        Public Property Submitted As Boolean
            Get
                Return m_Submitted
            End Get
            Set(ByVal Value As Boolean)
                m_Submitted = value
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

        Public Sub New(ByVal DB As Database, TwoPriceProductPriceID As Integer)
            m_DB = DB
            m_TwoPriceProductPriceID = TwoPriceProductPriceID
        End Sub 'New

        Public Sub New(ByVal DB As Database, VendorID As Integer, ByVal ProductID As Integer, ByVal TwoPriceCampaignId As Integer)
            m_DB = DB
            m_VendorID = VendorID
            m_ProductID = ProductID
            m_TwoPriceCampaignID = TwoPriceCampaignId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM TwoPriceVendorProductPrice WHERE TwoPriceProductPriceID = " & DB.Number(TwoPriceProductPriceID)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            Else
                m_TwoPriceProductPriceID = Nothing
            End If
            r.Close()
        End Sub

        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_TwoPriceProductPriceID = Core.GetInt(r.Item("TwoPriceProductPriceID"))
            m_VendorID = Core.GetInt(r.Item("VendorID"))
            m_ProductID = Core.GetInt(r.Item("ProductID"))
            m_TwoPriceCampaignID = Core.GetInt(r.Item("TwoPriceCampaignID"))
            m_Price = Core.GetDouble(r.Item("Price"))
            m_LastUpdated = Core.GetDate(r.Item("LastUpdated"))
            m_Comments = Core.GetString(r.Item("Comments"))
            m_Submitted = Core.GetBoolean(r.Item("Submitted"))
        End Sub 'Load

        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO TwoPriceVendorProductPrice (" _
                & " VendorID" _
                & ",ProductID" _
                & ",TwoPriceCampaignID" _
                & ",Price" _
                & ",LastUpdated" _
                & ",Comments" _
                & ",Submitted" _
                & ") VALUES (" _
                & m_DB.NullNumber(VendorID) _
                & "," & m_DB.NullNumber(ProductID) _
                & "," & m_DB.NullNumber(TwoPriceCampaignID) _
                & "," & m_DB.Number(Price) _
                & "," & m_DB.NullQuote(LastUpdated) _
                & "," & m_DB.Quote(Comments) _
                & "," & CInt(Submitted) _
                & ")"

            TwoPriceProductPriceID = m_DB.InsertSQL(SQL)

            Return TwoPriceProductPriceID
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE TwoPriceVendorProductPrice WITH (ROWLOCK) SET " _
                & " VendorID = " & m_DB.NullNumber(VendorID) _
                & ",ProductID = " & m_DB.NullNumber(ProductID) _
                & ",TwoPriceCampaignID = " & m_DB.NullNumber(TwoPriceCampaignID) _
                & ",Price = " & m_DB.Number(Price) _
                & ",LastUpdated = " & m_DB.NullQuote(LastUpdated) _
                & ",Comments = " & m_DB.Quote(Comments) _
                & ",Submitted = " & CInt(Submitted) _
                & " WHERE TwoPriceProductPriceID = " & m_DB.quote(TwoPriceProductPriceID)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update
    End Class

    Public Class TwoPriceVendorProductPriceCollection
        Inherits GenericCollection(Of TwoPriceVendorProductPriceRow)
    End Class

End Namespace
