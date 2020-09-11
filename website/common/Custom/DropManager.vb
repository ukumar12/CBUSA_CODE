Imports Components
Imports DataLayer
Imports System.Web.UI
Imports System.Text
Imports System.Collections.Specialized

Namespace Controls
    Public Class DropManager
        Inherits ScriptControl
        Implements ICallbackEventHandler

        Public OrderId As Integer
        Public IsTwoPriceOrder As Boolean

        Protected ReadOnly Property DB() As Database
            Get
                Return CType(Page, BasePage).DB
            End Get
        End Property

        Private m_DropItems As StringDictionary
        Public Property DropItems() As StringDictionary
            Get
                If m_DropItems Is Nothing Then
                    m_DropItems = New StringDictionary
                End If
                Return m_DropItems
            End Get
            Set(ByVal value As StringDictionary)
                m_DropItems = value
            End Set
        End Property

        Private m_OpenDropForm As String
        Public Property OpenDropForm() As String
            Get
                Return m_OpenDropForm
            End Get
            Set(ByVal value As String)
                m_OpenDropForm = value
            End Set
        End Property


        Protected Overrides Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor)
            Dim s As New ScriptControlDescriptor("AE.DropManager", ClientID)

            Dim json As New System.Web.Script.Serialization.JavaScriptSerializer()

            Dim dtItems As DataTable

            If IsTwoPriceOrder Then
                dtItems = GetTwoPriceOrderProducts(DB, OrderId)
            Else
                dtItems = OrderRow.GetOrderProducts(DB, OrderId, "SortOrder")
            End If

            'Dim dtItems As DataTable = OrderRow.GetOrderProducts(DB, OrderId, "SortOrder")
            'If dtItems.Rows.Count = 0 Then
            '    'Check for twoprice if not in regular order drops
            '    'Referencing TwoPrice.DataLayer would cause circular dependency here - just gonna copy the function into this file
            '    dtItems = GetTwoPriceOrderProducts(DB, OrderId)
            'End If

            Dim sItems As New StringBuilder("[")
            Dim sConn As String = String.Empty

            For Each row As DataRow In dtItems.Rows
                sItems.Append(sConn & "{'id':" & json.Serialize(row("OrderProductID")) & ",'name':" & json.Serialize("<b>" & row("Quantity") & "&nbsp;&nbsp;-&nbsp;&nbsp;</b>" & row("ProductName") & "&nbsp;<b>(#" & row("ProductSku") & ")</b>") & ",'dropId':" & IIf(IsDBNull(row("DropId")), 0, row("DropId")) & ",'sortorder':" & row("SortOrder") & "}")
                sConn = ","
            Next
            sItems.Append("]")
            s.AddScriptProperty("initItems", sItems.ToString)


            s.AddScriptProperty("initDrops", GetDropsJson)

            s.AddScriptProperty("openDropFunction", OpenDropForm)
            s.AddProperty("callback", Page.ClientScript.GetCallbackEventReference(Me, "arg", "null", "null"))
            'CSS
            s.AddProperty("itemNormalClass", "dropmgritem")
            s.AddProperty("itemDragClass", "dropmgrdrag")
            s.AddProperty("dropNormalClass", "dropmgrdrop")
            s.AddProperty("dropHoverClass", "drpmgrhover")

            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetDropsJson() As String
            Dim json As New System.Web.Script.Serialization.JavaScriptSerializer
            Dim dtDrops As DataTable = OrderRow.GetOrderDrops(DB, OrderId)
            If dtDrops.Rows.Count = 0 Then
                'Check for twoprice if not in regular order drops
                'Referencing TwoPrice.DataLayer would cause circular dependency here - just gonna copy the function into this file
                dtDrops = GetTwoPriceOrderDrops(DB, OrderId)
            End If
            Dim sDrops As New StringBuilder("[")
            Dim sConn As String = String.Empty
            For Each row As DataRow In dtDrops.Rows
                Dim jsonDrop As New DropDetails()
                With jsonDrop
                    .id = row("OrderDropID")
                    .name = row("DropName")
                    .delivery = IIf(IsDBNull(row("RequestedDelivery")), String.Empty, row("RequestedDelivery"))
                    .instructions = Core.GetString(row("DeliveryInstructions"))
                    .notes = Core.GetString(row("Notes"))
                End With

                sDrops.Append(sConn & json.Serialize(jsonDrop))
                sConn = ","
            Next
            sDrops.Append("]")
            Return sDrops.ToString
        End Function

        Public Function GetTwoPriceOrderDrops(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = "select * from TwoPriceOrderDrop where OrderId=" & DB.Number(OrderId)
            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Public Shared Function GetTwoPriceOrderProducts(ByVal DB As Database, ByVal OrderId As Integer, Optional ByVal SortBy As String = "", Optional ByVal SortOrder As String = "ASC") As DataTable
            Dim sql As String = _
                  " select o.*, o.TwoPriceOrderProductId AS OrderProductId, p.Product as ProductName, p.*, coalesce(o.VendorSku,p.SKU) as ProductSku, o.DropId, o.VendorPrice as Price " _
                & " from TwoPriceOrderProduct o inner join Product p on o.ProductId=p.ProductId " _
                & " where o.TwoPriceOrderId=" & DB.Number(OrderId)

            If SortBy <> String.Empty Then
                sql &= " order by " & Core.ProtectParam(SortBy & " " & SortOrder)
            End If
            Return DB.GetDataTable(sql)
        End Function

        Protected Overrides Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference)
            Dim s As New ScriptReference("/includes/controls/DropManager.js")
            Return New ScriptReference() {s}
        End Function

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            writer.AddAttribute("id", Me.ClientID)
            writer.AddAttribute("name", Me.UniqueID)
            writer.AddAttribute("type", "hidden")
            If DropItems.Count > 0 Then
                Dim json As New System.Web.Script.Serialization.JavaScriptSerializer()
                writer.AddAttribute("value", json.Serialize(DropItems))
            End If
            writer.RenderBeginTag("input")

            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
        End Sub

        Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
            Return String.Empty
        End Function

        Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent
            If eventArgument <> String.Empty Then
                Dim json As New System.Web.Script.Serialization.JavaScriptSerializer
                Dim data As Object = json.DeserializeObject(eventArgument)
                DropItems.Clear()
                For Each drop As Generic.KeyValuePair(Of String, Object) In data
                    DropItems.Add(drop.Key, drop.Value)
                    If drop.Value <> String.Empty Then
                        'OrderDropRow.AddProductsToDrop(DB, drop.Key, drop.Value)
                        Dim items As String() = drop.Value.Split(",")
                        For i As Integer = 0 To items.Length - 1
                            Dim parts As String() = items(i).Split("|")
                            If drop.Key = -1 Then
                                If parts.Length <> 2 OrElse Not UpdateProductDrop(DB, parts(0), Nothing, parts(1)) Then
                                    Logger.Error("Error deserializing OrderProducts in DropManager.vb")
                                End If
                            Else
                                If parts.Length <> 2 OrElse Not UpdateProductDrop(DB, parts(0), drop.Key, parts(1)) Then
                                    Logger.Error("Error deserializing OrderProducts in DropManager.vb")
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End Sub

        Private Function UpdateProductDrop(ByVal DB As Database, ByVal OrderProductID As Integer, ByVal OrderDropID As Integer, ByVal SortOrder As Integer) As Boolean
            Dim dtDrops As DataTable = OrderRow.GetOrderDrops(DB, OrderId)
            Dim Prefix As String = ""
            If dtDrops.Rows.Count = 0 Then
                Prefix = "TwoPrice"
            End If

            Dim sql As String = "update " & Prefix & "OrderProduct set DropId=" & DB.Number(OrderDropID) & ", SortOrder=" & DB.Number(SortOrder) & " where " & Prefix & "OrderProductID=" & DB.Number(OrderProductID)
            Return DB.ExecuteSQL(sql)
        End Function
    End Class

    Public Class DropDetails
        Public id As Integer
        Public name As String
        Public delivery As String
        Public instructions As String
        Public notes As String
    End Class
End Namespace
