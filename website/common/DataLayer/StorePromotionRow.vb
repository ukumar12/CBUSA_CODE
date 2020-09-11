Option Explicit On

Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Diagnostics
Imports System.Text
Imports Components

Namespace DataLayer

    Public Class StorePromotionRow
        Inherits StorePromotionRowBase

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Sub New(ByVal DB As Database)
            MyBase.New(DB)
        End Sub 'New

        Public Sub New(ByVal DB As Database, ByVal PromotionId As Integer)
            MyBase.New(DB, PromotionId)
        End Sub 'New

        'Shared function to get one row
        Public Shared Function GetRow(ByVal DB As Database, ByVal PromotionId As Integer) As StorePromotionRow
            Dim row As StorePromotionRow

            row = New StorePromotionRow(DB, PromotionId)
            row.Load()

            Return row
        End Function

        Public Shared Function GetRowByCode(ByVal DB As Database, ByVal PromotionCode As String) As StorePromotionRow
            Dim SQL As String = "SELECT top 1 * FROM StorePromotion WHERE PromotionCode = " & DB.Quote(PromotionCode)
            Dim r As SqlDataReader
            Dim row As StorePromotionRow = New StorePromotionRow(DB)
            r = DB.GetReader(SQL)
            If r.Read Then
                row.Load(r)
            End If
            r.Close()
            Return row
        End Function

        Public Shared Sub RemoveRow(ByVal DB As Database, ByVal PromotionId As Integer)
            Dim row As StorePromotionRow

            row = New StorePromotionRow(DB, PromotionId)
            row.Remove()
        End Sub

        'Custom Methods
        Public Sub InsertPromotionItem(ByVal Record As String)
            Dim aTmp() As String = Split(Record, "-")
            If aTmp(1) = "Item" Then
                DB.ExecuteSQL("INSERT INTO StorePromotionItem (PromotionId, ItemId) VALUES (" & PromotionId & "," & DB.Quote(aTmp(0)) & ")")
            ElseIf aTmp(1) = "Department" Then
                DB.ExecuteSQL("INSERT INTO StorePromotionItem (PromotionId, DepartmentId) VALUES (" & PromotionId & "," & DB.Quote(aTmp(0)) & ")")
            End If
        End Sub

        Public Sub RemovePromotionItem(ByVal Id As Integer)
            DB.ExecuteSQL("DELETE FROM StorePromotionItem WHERE Id=" & DB.Number(Id))
        End Sub

        'Custom Methods
        Public Shared Function GetAllPromotions(ByVal DB As Database) As DataTable
            Dim dt As DataTable = DB.GetDataTable("select * from StorePromotion order by PromotionName")
            Return dt
        End Function
    End Class

    Public MustInherit Class StorePromotionRowBase
        Private m_DB As Database
        Private m_PromotionId As Integer = Nothing
        Private m_PromotionName As String = Nothing
        Private m_PromotionCode As String = Nothing
        Private m_PromotionType As String = Nothing
        Private m_Message As String = Nothing
        Private m_Discount As Double = Nothing
        Private m_StartDate As DateTime = Nothing
        Private m_EndDate As DateTime = Nothing
        Private m_MinimumPurchase As Double = Nothing
        Private m_MaximumPurchase As Double = Nothing
        Private m_IsItemSpecific As Boolean = Nothing
        Private m_IsFreeShipping As Boolean = Nothing
        Private m_IsActive As Boolean = Nothing
        Private m_NumberSent As Integer = Nothing
        Private m_DeliveryMethod As String = Nothing


        Public Property PromotionId() As Integer
            Get
                Return m_PromotionId
            End Get
            Set(ByVal Value As Integer)
                m_PromotionId = value
            End Set
        End Property


        Public Property PromotionName() As String
            Get
                Return m_PromotionName
            End Get
            Set(ByVal Value As String)
                m_PromotionName = value
            End Set
        End Property

        Public Property PromotionCode() As String
            Get
                Return m_PromotionCode
            End Get
            Set(ByVal Value As String)
                m_PromotionCode = value
            End Set
        End Property

        Public Property PromotionType() As String
            Get
                Return m_PromotionType
            End Get
            Set(ByVal Value As String)
                m_PromotionType = value
            End Set
        End Property

        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal Value As String)
                m_Message = value
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

        Public Property MinimumPurchase() As Double
            Get
                Return m_MinimumPurchase
            End Get
            Set(ByVal Value As Double)
                m_MinimumPurchase = value
            End Set
        End Property

        Public Property MaximumPurchase() As Double
            Get
                Return m_MaximumPurchase
            End Get
            Set(ByVal Value As Double)
                m_MaximumPurchase = value
            End Set
        End Property


        Public Property IsFreeShipping() As Boolean
            Get
                Return m_IsFreeShipping
            End Get
            Set(ByVal Value As Boolean)
                m_IsFreeShipping = value
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
        Public Property IsItemSpecific() As Boolean
            Get
                Return m_IsItemSpecific
            End Get
            Set(ByVal Value As Boolean)
                m_IsItemSpecific = Value
            End Set
        End Property
        Public Property NumberSent() As Integer
            Get
                Return m_NumberSent
            End Get
            Set(ByVal value As Integer)
                m_NumberSent = value
            End Set
        End Property

        Public Property DeliveryMethod() As String
            Get
                Return m_DeliveryMethod
            End Get
            Set(ByVal value As String)
                m_DeliveryMethod = value
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

        Public Sub New(ByVal DB As Database, ByVal PromotionId As Integer)
            m_DB = DB
            m_PromotionId = PromotionId
        End Sub 'New

        Protected Overridable Sub Load()
            Dim r As SqlDataReader
            Dim SQL As String

            SQL = "SELECT * FROM StorePromotion WHERE PromotionId = " & DB.Number(PromotionId)
            r = m_DB.GetReader(SQL)
            If r.Read Then
                Me.Load(r)
            End If
            r.Close()
        End Sub


        Protected Overridable Sub Load(ByVal r As sqlDataReader)
            m_PromotionId = Convert.ToInt32(r.Item("PromotionId"))
            If IsDBNull(r.Item("PromotionName")) Then
                m_PromotionName = Nothing
            Else
                m_PromotionName = Convert.ToString(r.Item("PromotionName"))
            End If
            If IsDBNull(r.Item("PromotionCode")) Then
                m_PromotionCode = Nothing
            Else
                m_PromotionCode = Convert.ToString(r.Item("PromotionCode"))
            End If
            If IsDBNull(r.Item("PromotionType")) Then
                m_PromotionType = Nothing
            Else
                m_PromotionType = Convert.ToString(r.Item("PromotionType"))
            End If
            If IsDBNull(r.Item("Message")) Then
                m_Message = Nothing
            Else
                m_Message = Convert.ToString(r.Item("Message"))
            End If
            If IsDBNull(r.Item("Discount")) Then
                m_Discount = Nothing
            Else
                m_Discount = Convert.ToDouble(r.Item("Discount"))
            End If
            m_StartDate = Convert.ToDateTime(r.Item("StartDate"))
            m_EndDate = Convert.ToDateTime(r.Item("EndDate"))
            If IsDBNull(r.Item("MinimumPurchase")) Then
                m_MinimumPurchase = Nothing
            Else
                m_MinimumPurchase = Convert.ToDouble(r.Item("MinimumPurchase"))
            End If
            If IsDBNull(r.Item("MaximumPurchase")) Then
                m_MaximumPurchase = Nothing
            Else
                m_MaximumPurchase = Convert.ToDouble(r.Item("MaximumPurchase"))
            End If
            m_IsItemSpecific = Convert.ToBoolean(r.Item("IsItemSpecific"))
            m_IsFreeShipping = Convert.ToBoolean(r.Item("IsFreeShipping"))
            m_IsActive = Convert.ToBoolean(r.Item("IsActive"))
            If IsDBNull(r.Item("DeliveryMethod")) Then
                m_DeliveryMethod = Nothing
            Else
                m_DeliveryMethod = Convert.ToString(r.Item("DeliveryMethod"))
            End If
            If IsDBNull(r.Item("NumberSent")) Then
                m_NumberSent = Nothing
            Else
                m_NumberSent = Convert.ToInt32(r.Item("NumberSent"))
            End If
        End Sub 'Load
        Public Overridable Function Insert() As Integer
            Dim SQL As String


            SQL = " INSERT INTO StorePromotion (" _
             & " PromotionName" _
             & ",PromotionCode" _
             & ",PromotionType" _
             & ",Message" _
             & ",Discount" _
             & ",StartDate" _
             & ",EndDate" _
             & ",MinimumPurchase" _
             & ",MaximumPurchase" _
             & ",IsItemSpecific" _
             & ",IsFreeShipping" _
             & ",IsActive" _
             & ",NumberSent" _
             & ",DeliveryMethod" _
             & ") VALUES (" _
             & m_DB.Quote(PromotionName) _
             & "," & m_DB.Quote(PromotionCode) _
             & "," & m_DB.Quote(PromotionType) _
             & "," & m_DB.Quote(Message) _
             & "," & m_DB.Number(Discount) _
             & "," & m_DB.NullQuote(StartDate) _
             & "," & m_DB.NullQuote(EndDate) _
             & "," & m_DB.Number(MinimumPurchase) _
             & "," & m_DB.Number(MaximumPurchase) _
             & "," & CInt(IsItemSpecific) _
             & "," & CInt(IsFreeShipping) _
             & "," & CInt(IsActive) _
             & "," & m_DB.NullNumber(NumberSent) _
             & "," & m_DB.Quote(DeliveryMethod) _
             & ")"

            PromotionId = m_DB.InsertSQL(SQL)

            Return PromotionId
        End Function

        Public Overridable Sub Update()
            Dim SQL As String

            SQL = " UPDATE StorePromotion SET " _
             & " PromotionName = " & m_DB.Quote(PromotionName) _
             & ",PromotionCode = " & m_DB.Quote(PromotionCode) _
             & ",PromotionType = " & m_DB.Quote(PromotionType) _
             & ",Message = " & m_DB.Quote(Message) _
             & ",Discount = " & m_DB.Number(Discount) _
             & ",StartDate = " & m_DB.NullQuote(StartDate) _
             & ",EndDate = " & m_DB.NullQuote(EndDate) _
             & ",MinimumPurchase = " & m_DB.Number(MinimumPurchase) _
             & ",MaximumPurchase = " & m_DB.Number(MaximumPurchase) _
             & ",IsItemSpecific = " & CInt(IsItemSpecific) _
             & ",IsFreeShipping = " & CInt(IsFreeShipping) _
             & ",IsActive = " & CInt(IsActive) _
             & ",NumberSent = " & m_DB.NullNumber(NumberSent) _
             & ",DeliveryMethod = " & m_DB.Quote(DeliveryMethod) _
             & " WHERE PromotionId = " & m_DB.Quote(PromotionId)

            m_DB.ExecuteSQL(SQL)

        End Sub 'Update

        Public Sub Remove()
            Dim SQL As String

            SQL = "DELETE FROM StorePromotionItem WHERE PromotionId = " & m_DB.Quote(PromotionId)
            m_DB.ExecuteSQL(SQL)

            SQL = "DELETE FROM StorePromotion WHERE PromotionId = " & m_DB.Quote(PromotionId)
            m_DB.ExecuteSQL(SQL)
        End Sub 'Remove
    End Class

    Public Class StorePromotionCollection
        Inherits GenericCollection(Of StorePromotionRow)
    End Class

End Namespace

