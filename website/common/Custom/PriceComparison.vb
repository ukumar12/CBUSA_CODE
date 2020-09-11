Imports Components
Imports DataLayer
Imports System.Web
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization

Namespace Controls
    Public Class PriceComparison
        Inherits Control
        Implements IScriptControl
        Implements IPostBackDataHandler
        Implements IPostBackEventHandler


        Public Delegate Sub PriceComparisonDelegate(ByVal sender As Object, ByVal e As PriceComparisonEventArgs)
        Public Event Postback As PriceComparisonDelegate

        Public Sub New()
            CssClasses = New JsonCssClasses
            With CssClasses
                .normal = "norm"
                .subInit = "sub"
                .noPrice = "none"
                .noPricePending = "nonepend"
                .noPriceAccepted = "noneaccpt"
                .noPriceOmit = "noneomit"
                .subAccepted = "subaccpt"
                .subRejected = "subreject"
            End With
        End Sub

        Protected ReadOnly Property DB() As Database
            Get
                Return CType(Page, BasePage).DB
            End Get
        End Property

        Private m_Results As DataTable
        Public ReadOnly Property Results() As DataTable
            Get
                If m_Results Is Nothing Then
                    m_Results = GetDataTable()
                End If
                Return m_Results
            End Get
        End Property

        Public Property PriceComparisonId() As Integer
            Get
                Return ViewState("PriceComparisonId")
            End Get
            Set(ByVal value As Integer)
                ViewState("PriceComparisonId") = value
            End Set
        End Property

        Private dbPriceComparison As PriceComparisonRow

        Private m_TakeoffProducts As DataTable
        Public ReadOnly Property TakeoffProducts() As DataTable
            Get
                If m_TakeoffProducts Is Nothing Then
                    m_TakeoffProducts = TakeOffRow.GetTakeoffProducts(DB, TakeoffId)
                End If
                Return m_TakeoffProducts
            End Get
        End Property

        Private m_TakeoffId As Integer
        Public Property TakeoffId() As Integer
            Get
                Return m_TakeoffId
            End Get
            Set(ByVal value As Integer)
                m_TakeoffId = value
            End Set
        End Property

        Private m_SubFormOpenFunction As String
        Public Property SubFormOpenFunction() As String
            Get
                Return m_SubFormOpenFunction
            End Get
            Set(ByVal value As String)
                m_SubFormOpenFunction = value
            End Set
        End Property

        Private m_SpecialFormOpenFunction As String
        Public Property SpecialFormOpenFunction() As String
            Get
                Return m_SpecialFormOpenFunction
            End Get
            Set(ByVal value As String)
                m_SpecialFormOpenFunction = value
            End Set
        End Property

        Public CssClasses As JsonCssClasses

        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AE.PriceComparison", Me.ClientID)
            Dim dtProducts As DataTable = TakeOffRow.GetTakeoffProducts(DB, TakeoffId)
            Dim sProducts As String = "["
            Dim sConn As String = String.Empty
            Dim json As New JavaScriptSerializer()
            For Each row As DataRow In dtProducts.Rows
                If Not IsDBNull(row("SpecialOrderProductId")) Then
                    sProducts &= sConn & "{'qty':" & json.Serialize(row("Quantity")) & ",'sku':" & json.Serialize(row("SKU")) & ",'name':" & json.Serialize(row("SpecialOrderProduct")) & ",'id':" & json.Serialize(row("TakeoffProductId")) & ",'productid':" & json.Serialize(row("ProductId")) & "}"
                Else
                    sProducts &= sConn & "{'qty':" & json.Serialize(row("Quantity")) & ",'sku':" & json.Serialize(row("SKU")) & ",'name':" & json.Serialize(row("Product")) & ",'id':" & json.Serialize(row("TakeoffProductId")) & ",'productid':" & json.Serialize(row("ProductId")) & "}"
                End If
                sConn = ","
            Next
            sProducts &= "]"
            s.AddScriptProperty("products", sProducts)
            s.AddScriptProperty("classes", json.Serialize(CssClasses))
            s.AddProperty("priceComparisonId", PriceComparisonId)
            s.AddProperty("takeoffId", TakeoffId)
            s.AddProperty("pbTemplate", Page.ClientScript.GetPostBackEventReference(Me, "##VID##"))
            s.AddScriptProperty("openSubForm", SubFormOpenFunction)
            s.AddScriptProperty("openSpecialForm", SpecialFormOpenFunction)

            If PriceComparisonId <> Nothing Then
                s.AddProperty("initVendors", PriceComparisonRow.GetSavedVendors(DB, PriceComparisonId))
            End If

            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim a As New List(Of ScriptReference)
            a.Add(New ScriptReference("/includes/controls/PriceComparison.js"))
            Return a.ToArray
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            ScriptManager.GetCurrent(Page).RegisterScriptControl(Me)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
            writer.AddAttribute("id", Me.ClientID)
            writer.AddAttribute("name", Me.UniqueID)
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("value", "")
            writer.RenderBeginTag("input")
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            Page.RegisterRequiresRaiseEvent(Me)
            If postCollection(postDataKey) <> Nothing Then
                Dim re As New Regex("VID([\d]+)\|PID([\d]+)$", Text.RegularExpressions.RegexOptions.Compiled)
                Dim json As New JavaScriptSerializer()

                For Each key As String In Page.Request.Form.AllKeys
                    If key <> String.Empty Then
                        Dim m As Match = re.Match(key)
                        If m.Success Then
                            Dim value As Controls.JsonVendorProduct = json.Deserialize(Of JsonVendorProduct)(Page.Request.Form.Item(key))
                            Dim row As DataRow = Results.NewRow

                            row("TakeoffProductID") = m.Groups(2).Value
                            row("VendorID") = m.Groups(1).Value

                            row("ProductID") = value.ProductId
                            row("SubstituteProductID") = value.SubProductId
                            row("Quantity") = value.Qty
                            row("UnitPrice") = value.UnitPrice
                            row("Total") = value.Price
                            row("RecommendedQuantity") = value.Qty * value.Multiply
                            row("SpecialOrderProductID") = value.SpecialOrderProductId
                            row("State") = value.State

                            Results.Rows.Add(row)
                        End If
                    End If
                Next
                Return True
            Else
                Return False
            End If
        End Function

        Private Function GetDataTable() As DataTable
            Dim dt As New DataTable
            dt.Columns.Add(New DataColumn("TakeoffProductId", GetType(Integer)))
            dt.Columns.Add(New DataColumn("ProductID", GetType(Integer)))
            dt.Columns.Add(New DataColumn("VendorID", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Quantity", GetType(Integer)))
            dt.Columns.Add(New DataColumn("RecommendedQuantity", GetType(Integer)))
            dt.Columns.Add(New DataColumn("SubstituteProductID", GetType(Integer)))
            dt.Columns.Add(New DataColumn("SpecialOrderProductID", GetType(Integer)))
            dt.Columns.Add(New DataColumn("SupplyPhaseID", GetType(Integer)))
            dt.Columns.Add(New DataColumn("UnitPrice", GetType(Double)))
            dt.Columns.Add(New DataColumn("Total", GetType(Double)))
            dt.Columns.Add(New DataColumn("State", GetType(Int32)))
            Return dt
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent
        End Sub

        'Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
        '    Dim VendorId As Integer = eventArgument
        '    Dim result As IEnumerable = (From res As DataRow In Results.Rows Where res.Item("VendorId") = VendorId Select res)
        '    For Each row As DataRow In result
        '        Dim item As New PriceComparisonVendorProductPriceRow(DB)
        '        item.PriceComparisonID = PriceComparison.PriceComparisonID
        '        item.ProductID = row("ProductId")
        '        item.Quantity = row("Quantity")
        '        item.SubstituteProductID = row("SubstituteProductID")
        '        item.Total = row("Total")
        '        item.UnitPrice = row("UnitPrice")
        '        item.VendorID = row("VendorId")
        '        item.Insert()
        '    Next
        'End Sub

        Private Sub PriceComparison_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack And Not ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
                'PriceComparisonId = HttpContext.Current.Request("PriceComparisonId")
                If PriceComparisonId = Nothing Then
                    Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, TakeoffId)
                    dbPriceComparison = New PriceComparisonRow(DB)
                    dbPriceComparison.TakeoffID = TakeoffId
                    dbPriceComparison.BuilderID = dbTakeoff.BuilderID
                    PriceComparisonId = dbPriceComparison.Insert()
                Else
                    dbPriceComparison = PriceComparisonRow.GetRow(DB, PriceComparisonId)
                    TakeoffId = dbPriceComparison.TakeoffID
                End If
            End If
        End Sub

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            If eventArgument = String.Empty Then
                eventArgument = HttpContext.Current.Request.Form("__EVENTARGUMENT")
            End If
            If eventArgument <> String.Empty Then
                If eventArgument = "update" Then
                    RaiseEvent Postback(Me, New PriceComparisonEventArgs(True, Nothing))
                Else
                    RaiseEvent Postback(Me, New PriceComparisonEventArgs(False, eventArgument))
                End If
            End If
        End Sub
    End Class

    <Serializable()> _
    Public Class JsonVendorProduct
        Public ProductId As Integer
        Public SubProductId As Integer
        Public SpecialOrderProductId As Integer
        Public UnitPrice As Double
        Public Price As Double
        Public IsAvg As Boolean
        Public IsSub As Boolean
        Public IsSpecial As Boolean
        Public Sku As String
        Public Multiply As Double
        Public Qty As Integer
        Public State As ProductState
        Public Name As String
    End Class

    <Serializable()> _
    Public Class JsonCssClasses
        Public normal
        Public high
        Public low
        Public subInit
        Public subAccepted
        Public subRejected
        Public noPrice
        Public noPricePending
        Public noPriceAccepted
        Public noPriceRejected
        Public noPriceOmit
    End Class

    Public Enum ProductState
        Init = 0
        Pending = 1
        Accepted = 2
        Omit = 3
    End Enum

    Public Class PriceComparisonEventArgs
        Inherits System.EventArgs
        Public Sub New(ByVal IsUpdate As Boolean, ByVal VendorID As Integer)
            Me.IsUpdate = IsUpdate
            Me.VendorID = VendorID
        End Sub
        Public IsUpdate As Boolean
        Public VendorID As Integer
    End Class
End Namespace