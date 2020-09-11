Imports Components
Imports DataLayer
Imports Utility
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data
Imports System.Collections.Generic
Imports System.Linq


<WebService(Namespace:="http://www.cbusa.us/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Script.Services.ScriptService()> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class PriceComparison
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Sub RemoveVendor(ByVal PriceComparisonId As Integer, ByVal VendorId As Integer)
        PriceComparisonRow.RemoveVendor(GlobalDB.DB, PriceComparisonId, VendorId)
    End Sub

    <WebMethod(EnableSession:=True)> _
    Public Function LoadVendor(ByVal PriceComparisonId As Integer, ByVal VendorID As Integer, ByVal TakeoffID As Integer) As String
        Try
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(GlobalDB.DB, IIf(Session("TakeoffForId") Is Nothing, Session("BuilderId"), Session("TakeoffForId")))
            Dim dtPrices As DataTable = TakeOffProductRow.GetTakeoffVendorPricing(GlobalDB.DB, TakeoffID, VendorID)
            'Dim dtPrices As DataTable = TakeOffProductRow.GetTakeoffVendorPricingOld(GlobalDB.DB, TakeoffID, VendorID)
            'Dim dtProducts As DataTable = TakeOffRow.GetTakeoffProducts(GlobalDB.DB, TakeoffID)
            Dim dtProducts As DataTable = TakeOffRow.GetTakeoffProductAverages(GlobalDB.DB, TakeoffID, dbBuilder.LLCID)
            Dim dtRequests As DataTable = VendorProductPriceRequestRow.GetTakeoffPriceRequests(GlobalDB.DB, VendorID, TakeoffID)
            Dim dvStates As DataView = PriceComparisonRow.GetVendorProducts(GlobalDB.DB, PriceComparisonId, VendorID, False).DefaultView
            dvStates.Sort = "TakeoffProductID"

            'Dim dtSpecial As DataTable = TakeOffRow.GetSpecialVendorPricing(GlobalDB.DB, TakeoffID, VendorID)

            Dim dbVendor As VendorRow = VendorRow.GetRow(GlobalDB.DB, VendorID)

            PriceComparisonRow.UpdateVendor(GlobalDB.DB, PriceComparisonId, VendorID, dtProducts, dtPrices)

            Dim sOut As New System.Text.StringBuilder()
            Dim sConn As String = String.Empty

            Dim js As New System.Web.Script.Serialization.JavaScriptSerializer()
            sOut.Append("{'id':" & dbVendor.VendorID & ",'name':" & js.Serialize(dbVendor.CompanyName) & ",'products':{")
            'add product objects
            Dim reqIdx As Integer = 0
            For Each row As DataRow In dtPrices.Rows
                Dim tmp As DataRow = row
                Dim vrow As New Controls.JsonVendorProduct()
                Dim qty As Integer = Core.GetInt(row("Quantity"))
                vrow.Multiply = 1
                Dim TakeoffProductID As Integer = Core.GetInt(row("TakeoffProductID"))
                vrow.State = (From s As DataRowView In dvStates Where s("TakeoffProductID") = TakeoffProductID Select s("State")).FirstOrDefault
                'Dim isSpecial As Boolean = Not IsDBNull((From dr As DataRow In dtProducts.AsEnumerable Where dr("TakeoffProductId") = tmp.Item("TakeoffProductId") Select dr("SpecialOrderProductId")).FirstOrDefault)
                If IsDBNull(row("SpecialOrderProductId")) Then
                    vrow.ProductId = IIf(IsDBNull(row("ProductId")), Nothing, row("ProductId"))
                    vrow.SubProductId = IIf(IsDBNull(row("SubProductId")), Nothing, row("SubProductId"))
                    If vrow.SubProductId <> Nothing Then
                        vrow.Name = ProductRow.GetName(GlobalDB.DB, vrow.SubProductId)
                    Else
                        vrow.Name = ProductRow.GetName(GlobalDB.DB, vrow.ProductId)
                    End If
                    vrow.Sku = IIf(IsDBNull(row("VendorSku")), Nothing, row("VendorSku"))
                    vrow.IsSpecial = False
                    If IsDBNull(row("VendorPrice")) Then
                        vrow.UnitPrice = (From dr As DataRow In dtProducts.AsEnumerable() Where dr("TakeoffProductId") = tmp("TakeoffProductId") Select dr("AvgPrice")).FirstOrDefault
                        vrow.Price = qty * vrow.UnitPrice

                        If reqIdx < dtRequests.Rows.Count AndAlso Convert.ToString(dtRequests.Rows(reqIdx)("TakeoffProductID")) = Convert.ToString(row("TakeoffProductId")) Then
                            vrow.State = Controls.ProductState.Pending
                            reqIdx += 1
                        Else
                            vrow.State = Controls.ProductState.Init
                        End If
                        vrow.IsAvg = True
                        vrow.IsSub = False
                    ElseIf row("IsSubstitution") Then
                        vrow.UnitPrice = row("VendorPrice")
                        vrow.Price = qty * row("VendorPrice")
                        vrow.IsAvg = False
                        vrow.IsSub = True
                        vrow.Multiply = IIf(IsDBNull(row("SubstituteQuantityMultiplier")), 1, row("SubstituteQuantityMultiplier"))
                    Else
                        vrow.UnitPrice = row("VendorPrice")
                        vrow.Price = qty * row("VendorPrice")
                        vrow.IsAvg = False
                        vrow.IsSub = False
                    End If
                Else
                    If reqIdx < dtRequests.Rows.Count AndAlso Convert.ToString(dtRequests.Rows(reqIdx)("TakeoffProductID")) = Convert.ToString(row("TakeoffProductId")) Then
                        vrow.State = Controls.ProductState.Pending
                        reqIdx += 1
                    Else
                        vrow.State = Controls.ProductState.Init
                    End If
                    vrow.SpecialOrderProductId = row("SpecialOrderProductID")
                    If Not IsDBNull(row("SubProductID")) Then
                        vrow.SubProductId = row("SubProductID")
                        vrow.Name = ProductRow.GetName(GlobalDB.DB, vrow.SubProductId)
                        vrow.Sku = Core.GetString("VendorSku")
                        vrow.UnitPrice = Core.GetDouble(row("VendorPrice"))
                        'vrow.Price = Core.GetDouble(row("VendorPrice"))
                        vrow.IsSub = True
                        vrow.Multiply = Core.GetInt(row("SubstituteQuantityMultiplier"))
                    Else
                        vrow.IsSub = False
                        vrow.UnitPrice = IIf(IsDBNull(row("VendorSpecialPrice")), -1, row("VendorSpecialPrice"))
                        'vrow.Price = IIf(IsDBNull(row("VendorSpecialPrice")), -1, row("VendorSpecialPrice"))
                        'If vrow.Price > 0 Then vrow.Price *= qty
                        vrow.Sku = IIf(IsDBNull(row("VendorSpecialSku")), String.Empty, row("VendorSpecialSku"))
                        vrow.Multiply = 1
                    End If
                    vrow.Price = qty * vrow.UnitPrice
                    vrow.IsSpecial = True
                    vrow.IsAvg = False

                End If
                sOut.Append(sConn & "'" & row("TakeoffProductId") & "':" & js.Serialize(vrow))
                sConn = ","
            Next
            sOut.Append("}") 'close products array

            sConn = String.Empty

            'add special order product objects
            'sOut.Append(",'specials':{")
            'For Each row As DataRow In dtPrices.Rows
            '    sOut.Append(sConn & "'" & row("TakeoffProductId") & "':{")
            '    sOut.Append("'price':" & row("VendorPrice") & ",'vendorSku':'" & row("VendorSku") & "'")
            '    sOut.Append("}") 'close special obj
            '    sConn = ","
            'Next
            'sOut.Append("}") 'close specials array
            sOut.Append("}") 'close vendor object
            Return sOut.ToString
        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Function

    Private Sub UpdateCurrentComparison(Optional ByVal RemoveVendorID As Integer = Nothing)
        Dim dbOldComparison As PriceComparisonRow = PriceComparisonRow.GetRow(GlobalDB.DB, HttpContext.Current.Session("PriceComparisonID"))
        Dim vendors As PriceComparisonVendorSummaryCollection = PriceComparisonVendorSummaryRow.GetVendors(GlobalDB.DB, dbOldComparison.PriceComparisonID)
        Dim ids As New Generic.List(Of String)
        For Each row As PriceComparisonVendorSummaryRow In vendors
            If row.VendorID = RemoveVendorID Then
                row.Remove()
            Else
                ids.Add(row.VendorID)
            End If
        Next
        'If Not (dbOldComparison.IsSaved Or dbOldComparison.IsDashboard) Then
        '    dbOldComparison.Remove()
        'End If
        'Dim dbNewComparison As PriceComparisonRow = PriceComparisonRow.GetRow(GlobalDB.DB, dbOldComparison.TakeoffID, ids)
        'If dbNewComparison.Created = Nothing Then
        '    dbNewComparison.AdminID = Session("AdminId")
        '    dbNewComparison.BuilderID = Session("BuilderId")
        '    dbNewComparison.Insert()
        'End If
        'Session("PriceComparisonId") = dbNewComparison.PriceComparisonID

    End Sub

    <WebMethod(EnableSession:=True)> _
    Public Sub RemoveVendor(ByVal VendorID As Integer)
        UpdateCurrentComparison(VendorID)
    End Sub

    <WebMethod(EnableSession:=True)> _
    Public Function UpdateVendorProduct(ByVal PriceComparisonId As Integer, ByVal VendorId As Integer, ByVal TakeoffProductId As Integer, ByVal vprod As Controls.JsonVendorProduct) As Controls.JsonVendorProduct
        Dim dbProduct As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(GlobalDB.DB, PriceComparisonId, VendorId, TakeoffProductId)
        dbProduct.State = vprod.State
        If vprod.Qty <> Nothing Then dbProduct.Quantity = vprod.Qty
        dbProduct.UnitPrice = vprod.Price

        If vprod.IsSub Then
            Dim dbSub As VendorProductPriceRow = VendorProductPriceRow.GetRow(GlobalDB.DB, VendorId, vprod.SubProductId)
            vprod.Price = dbSub.VendorPrice * vprod.Qty
        End If

        If dbProduct.Quantity >= 0 And dbProduct.UnitPrice >= 0 Then
            dbProduct.Total = dbProduct.Quantity * dbProduct.UnitPrice
        End If
        dbProduct.Update()

        Return vprod
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Sub RequestPricing(ByVal PriceComparisonId As Integer, ByVal VendorId As Integer)
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(GlobalDB.DB, "MissingPrice")
        'Dim dbRecip As New automatic
        Dim dbVendor As VendorRow = VendorRow.GetRow(GlobalDB.DB, VendorId)
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(GlobalDB.DB, Session("BuilderId"))
        Dim msg As New StringBuilder

        msg.Append("Builder " & dbBuilder.CompanyName & " is requesting pricing for the following items:" & vbCrLf & vbCrLf)

        Dim dtProducts As DataTable = PriceComparisonRow.GetVendorProducts(GlobalDB.DB, PriceComparisonId, VendorId, True)
        'Dim q = From product As DataRow In dtProducts.AsEnumerable Where (IsDBNull(product("UnitPrice")) OrElse product("UnitPrice") < 0) OrElse (product("State") <> Controls.ProductState.Accepted And product("State") <> Controls.ProductState.Omit And IsDBNull(product("SubstituteProductID"))) Select product
        For Each row As DataRow In dtProducts.Rows
            If (IsDBNull(row("UnitPrice")) OrElse row("UnitPrice") < 0) OrElse (row("State") <> Controls.ProductState.Accepted And row("State") <> Controls.ProductState.Omit And IsDBNull(row("SubstituteProductID"))) Then
                Dim dbRequest As VendorProductPriceRequestRow = VendorProductPriceRequestRow.GetRow(GlobalDB.DB, dbBuilder.BuilderID, dbVendor.VendorID, row("TakeoffProductID"))
                dbRequest.BuilderID = dbBuilder.BuilderID
                If dbRequest.Created = Nothing Then
                    dbRequest.CreatorBuilderAccountID = Session("BuilderAccountId")
                End If
                If Not IsDBNull(row("ProductID")) Then
                    dbRequest.ProductID = row("ProductID")
                End If
                dbRequest.TakeoffProductID = row("TakeoffProductID")
                dbRequest.VendorID = dbVendor.VendorID
                dbRequest.SpecialOrderProductID = Core.GetInt(row("SpecialOrderProductID"))
                If dbRequest.Created = Nothing Then
                    dbRequest.Insert()
                Else
                    dbRequest.Update()
                End If

                msg.Append(row("Product"))
                If Not IsDBNull(row("SpecialOrderProductId")) Then
                    msg.Append("(Special Order)")
                End If
                msg.Append(vbTab & row("Description"))
                msg.AppendLine()
            End If
        Next
        msg.AppendLine()
        msg.AppendLine(dbMsg.Message)

        Dim FromName As String = SysParam.GetValue(GlobalDB.DB, "ContactUsName")
        Dim FromEmail As String = SysParam.GetValue(GlobalDB.DB, "ContactUsEmail")

        Dim sMsg As String = msg.ToString

        'FOR TESTING
        If SysParam.GetValue(GlobalDB.DB, "TestMode") = True Then
            dbVendor.Email = FromEmail
        End If


        If dbMsg.IsEmail Then
            Core.SendSimpleMail(FromEmail, FromName, dbVendor.Email, dbVendor.CompanyName, dbMsg.Subject, sMsg)
            If dbMsg.CCList <> String.Empty Then
                Dim aEmails() As String = dbMsg.CCList.Split(",")
                For Each email As String In aEmails
                    Core.SendSimpleMail(FromEmail, FromName, email, email, dbMsg.Subject, sMsg)
                Next
            End If
        End If
    End Sub

    <WebMethod(Enablesession:=True)> _
    Public Sub AddMarketIndicator(ByVal ProductID As Integer)
        Dim PriceComparisonID As Integer = Session("PriceComparisonId")
        Dim dt As DataTable = GlobalDB.DB.GetDataTable("select * from PriceComparisonVendorProductPrice where PriceComparisonID=" & GlobalDB.DB.Number(PriceComparisonID) & " and ProductID=" & GlobalDB.DB.Number(ProductID))

        Dim dbProduct As New BuilderIndicatorProductRow(GlobalDB.DB)
        dbProduct.BuilderID = Session("BuilderId")
        dbProduct.ProductID = ProductID
        dbProduct.Insert()
        For Each row As DataRow In dt.Rows
            Dim dbVendor As New BuilderIndicatorVendorRow(GlobalDB.DB)
            dbVendor.BuilderID = Session("BuilderId")
            dbVendor.VendorID = row("VendorID")
            dbVendor.BuilderIndicatorProductID = dbProduct.BuilderIndicatorProductID
            dbVendor.Insert()
        Next
    End Sub
End Class
