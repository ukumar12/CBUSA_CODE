Imports Components
Imports Controls
Imports DataLayer
Imports System.Linq
Imports TwoPrice.DataLayer
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager

Partial Class Edit
    Inherits AdminPage

    Private selectedVendors As String
    '10/25/2010
    Private IsReadOnly As Boolean = False

    Protected ReadOnly Property TwoPriceCampaignId As Integer
        Get
            Return Request.QueryString("TwoPriceCampaignId")
        End Get
    End Property

    Private m_IsAwardedCampaign As Boolean = Nothing
    Private ReadOnly Property IsAwardedCampaign As Boolean
        Get
            If m_IsAwardedCampaign = Nothing Then
                m_IsAwardedCampaign = dbTwoPriceCampaign.Status = "Awarded"
            End If
            Return m_IsAwardedCampaign
        End Get
    End Property

    Private m_dbTwoPriceCampaign As TwoPriceCampaignRow = Nothing
    Private ReadOnly Property dbTwoPriceCampaign As TwoPriceCampaignRow
        Get
            If m_dbTwoPriceCampaign Is Nothing Then
                m_dbTwoPriceCampaign = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
            End If

            Return m_dbTwoPriceCampaign
        End Get
    End Property

    Private m_dbTwoPriceTakeOff As TwoPriceTakeOffRow = Nothing
    Private ReadOnly Property dbTwoPriceTakeOff As TwoPriceTakeOffRow
        Get
            If m_dbTwoPriceTakeOff Is Nothing Then
                m_dbTwoPriceTakeOff = TwoPriceTakeOffRow.GetRowByTwoPriceCampaignId(DB, TwoPriceCampaignId)
            End If

            Return m_dbTwoPriceTakeOff
        End Get
    End Property

    Protected ReadOnly Property PrintUrl() As String
        Get
            Dim Output As String = Request.ServerVariables("URL").ToString.Trim & "?"
            If Not Request.ServerVariables("QUERY_STRING") Is Nothing AndAlso Request.ServerVariables("QUERY_STRING").ToString.Trim <> String.Empty Then
                Dim TempArray As String() = Split(Request.ServerVariables("QUERY_STRING").ToString.Trim, "&")
                For Each Param As String In TempArray
                    Dim TempArray2 As String() = Split(Param.ToString.Trim, "=")
                    If UBound(TempArray2) = 1 Then
                        If TempArray2(0).ToString.Trim <> String.Empty AndAlso TempArray2(0).ToString.ToLower.Trim <> "print" Then
                            If Right(Output, 1) <> "?" Then Output &= "&"
                            Output &= TempArray2(0).ToString.Trim & "=" & TempArray2(1).ToString.Trim
                        End If
                    End If
                Next
                If Right(Output, 1) <> "?" Then Output &= "&"
            End If
            Output &= "print=y&" & GetPageParams(Components.FilterFieldType.All)
            Return Output
        End Get
    End Property

    Public ReadOnly Property LoadedVendors() As Generic.List(Of String)
        Get
            If Session("LoadedVendorComparisons") Is Nothing Then
                Session("LoadedVendorComparisons") = New Generic.List(Of String)
            End If
            Return Session("LoadedVendorComparisons")
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim auth As Boolean = True 'Context.User IsNot Nothing AndAlso Context.User.Identity.IsAuthenticated


        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

        'lsVendors.DataSource = VendorRow.GetListByLLC(DB, dbBuilder.LLCID, "CompanyName")
        selectedVendors = dbTwoPriceCampaign.GetSelectedCampaignVendors
        If selectedVendors <> Nothing Then
            Dim dtVendors As DataTable = DB.GetDataTable("SELECT * from Vendor where VendorId in" & DB.NumberMultiple(selectedVendors))

            For Each row As DataRow In dtVendors.Rows
                ltlVendors.Text &= row.Item("CompanyName") & "<br />"
            Next
        Else
            ltlVendors.Text = "<b class=""larger red"">No Vendors have supplied prices for the selected items</b>"
        End If

        BuildComparison()
    End Sub

    Private Sub TestPrices(ByVal pExtendedPrice As Double, ByRef pHighPrice As Double, ByRef pLowPrice As Double)
        If pExtendedPrice < pLowPrice OrElse pLowPrice = -1 Then
            pLowPrice = pExtendedPrice
        End If
        If pExtendedPrice > pHighPrice OrElse pHighPrice = -1 Then
            pHighPrice = pExtendedPrice
        End If
    End Sub

    Private Sub ComposeVenderRow(ByRef pVenderRow As PriceComparisonVendorProductPriceRow, ByVal pRecommendedQuantity As Integer, ByVal pSubProductId As Integer, ByVal pVendorPrice As Double)
        pVenderRow.RecommendedQuantity = pRecommendedQuantity
        If pVenderRow.RecommendedQuantity = 0 Then
            pVenderRow.RecommendedQuantity = pVenderRow.Quantity
        End If
        pVenderRow.SubstituteProductID = pSubProductId
        pVenderRow.UnitPrice = pVendorPrice
        pVenderRow.Total = pVenderRow.UnitPrice * pVenderRow.Quantity
    End Sub

    Private Enum TableTypes As Integer
        Priced = 0
        Subtotal = 1
        Average = 2
        Special = 3
    End Enum

    Private Sub BuildComparison()

        Dim Vendors As String = selectedVendors
        Dim avend As New ArrayList(Vendors.Split(","))


        Dim dtVendors As DataTable = DB.GetDataTable("select * from Vendor where VendorID in " & DB.NumberMultiple(Vendors) & " order by VendorID DESC")

        Dim dtTakeoffProducts As DataTable = TwoPriceTakeOffRow.GetTwoPriceTakeOffProducts(DB, dbTwoPriceTakeOff.TwoPriceTakeOffID)
        Dim dtPriced As DataTable = TwoPriceTakeOffRow.GetTwoPriceTakeOffProductPrices(DB, dbTwoPriceTakeOff.TwoPriceTakeOffID, Vendors)
        'Dim dtSubs As DataTable = TwoPriceTakeOffRow.GetTwoPriceTakeOffSubstitutions(DB, dbTwoPriceTakeOff.TwoPriceTakeOffID, newVendors)
        Dim dtSubs As New DataTable
        Dim dtAvgs As DataTable = TwoPriceTakeOffRow.GetTwoPriceTakeOffAveragePrices(DB, dbTwoPriceTakeOff.TwoPriceTakeOffID)
        Dim dtSpecial As DataTable = TwoPriceTakeOffRow.GetTwoPriceTakeOffSpecialPrices(DB, dbTwoPriceTakeOff.TwoPriceTakeOffID, Vendors)
        Dim dtState As DataTable = TwoPriceCampaignRow.GetVendorProductState(DB, TwoPriceCampaignId)

        Dim productCount As Integer = dtTakeoffProducts.Rows.Count
        Dim vendorCount As Integer = dtVendors.Rows.Count - 1
        Dim aPricedCounts(vendorCount) As Integer
        Dim aPricedTotals(vendorCount) As Double
        Dim aSubTotals(vendorCount) As Double
        Dim aAvgTotals(vendorCount) As Double
        Dim aSpecTotals(vendorCount) As Double
        Dim aFullyPriced(vendorCount) As Boolean
        Dim aCanRequest(vendorCount) As Boolean
        Dim aSubsIncomplete(vendorCount) As Boolean

        For i As Integer = 0 To vendorCount
            aPricedCounts(i) = 0
            aFullyPriced(i) = True
            aCanRequest(i) = False
            aSubsIncomplete(i) = False
        Next

        Dim td As HtmlTableCell
        For Each row As DataRow In dtVendors.Rows
            trPricedHeader.Cells.Add(New HtmlTableCell("th") With {.InnerHtml = row("CompanyName")})
            trSubHeader.Cells.Add(New HtmlTableCell("th") With {.InnerHtml = row("CompanyName")})
            trAvgHeader.Cells.Add(New HtmlTableCell("th") With {.InnerHtml = row("CompanyName")})
            trSpecHeader.Cells.Add(New HtmlTableCell("th") With {.InnerHtml = row("CompanyName")})
            trSumHeader.Cells.Add(New HtmlTableCell("th") With {.InnerHtml = row("CompanyName")})
        Next

        Dim aPrices As New ArrayList
        For Each TakeOffRow As DataRow In dtTakeoffProducts.Rows
            Dim tr As New HtmlTableRow
            tr.Cells.Add(New HtmlTableCell() With {.InnerHtml = TakeOffRow("Quantity")})
            tr.Cells.Add(New HtmlTableCell() With {.InnerHtml = Core.GetString(TakeOffRow("SKU"))})

            td = New HtmlTableCell
            If Not IsDBNull(TakeOffRow("Product")) Then
                td.InnerHtml = TakeOffRow("Product")
            ElseIf Not IsDBNull(TakeOffRow("SpecialOrderProduct")) Then
                td.InnerHtml = TakeOffRow("SpecialOrderProduct")
            End If
            tr.Cells.Add(td)

            Dim lowPrice As Double = -1
            Dim highPrice As Double = -1
            Dim tblNumber As Integer = TableTypes.Priced
            Dim aTempTotals(vendorCount) As Double
            aPrices.Clear()

            Dim dv As DataView
            If dtState.Rows.Count > 0 Then
                dv = New DataView(dtState, "ProductID=" & TakeOffRow("ProductID"), "VendorID DESC", DataViewRowState.CurrentRows)
            Else
                dv = dtState.DefaultView
            End If


            'index to current vendor in various structures ordered by VendorID
            Dim idx As Integer = 0
            Dim vendorIdx As Integer = 0
            For Each stateRow As DataRowView In dtVendors.DefaultView
                Dim dbVendorProductPrice As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(DB, 0, stateRow("VendorID"), TakeOffRow("TwoPriceTakeoffProductID"))
                td = New HtmlTableCell
                tr.Cells.Add(td)

                Dim sql As String = "VendorID=" & stateRow("VendorID") & " and TwoPriceTakeoffProductID=" & TakeOffRow("TwoPriceTakeoffProductID")


                'Non-Special Products

                Dim p As DataRow() = dtPriced.Select(sql)
                If p.Length > 0 Then
                    'Priced Products

                    Dim extendedPrice As Double = p(0)("VendorPrice") * TakeOffRow("Quantity") '* dbVendorProductPrice.Quantity
                    td.InnerHtml = FormatCurrency(extendedPrice)
                    td.Attributes("class") = "norm"
                    If Core.GetString(p(0)("Comments")) <> Nothing Then
                        'td.Attributes("class").ToString().Concat(" commentbackgroud")
                        td.Style.Add(HtmlTextWriterStyle.BackgroundColor, "#EBEBEC")
                        td.InnerHtml = "<a class='tooltip'><span class='fa fa-comment'></span>" & FormatCurrency(extendedPrice) & "</a>"
                        td.InnerHtml &= "<div class='adminToolTipWrpr'>" &
                                            "<div class='adminToolTipShadow'>" &
                                                Core.GetString(p(0)("Comments")) &
                                            "</div>" &
                                            "<div class='adminToolTopShadowBottom'>&nbsp;</div>" &
                                        "</div>"
                    End If
                    aPrices.Add(extendedPrice)
                    aTempTotals(idx) = extendedPrice
                    aPricedCounts(idx) += 1

                    TestPrices(extendedPrice, highPrice, lowPrice)

                    dbVendorProductPrice.ProductID = p(0)("ProductID")
                    dbVendorProductPrice.UnitPrice = p(0)("VendorPrice")
                    dbVendorProductPrice.Total = dbVendorProductPrice.Quantity * dbVendorProductPrice.UnitPrice
                Else
                    'Un-priced Products
                    aPrices.Add(0)
                    td.InnerHtml = ""
                End If

                'dbVendorProductPrice.Update()
                idx += 1
                vendorIdx += 1
            Next
            Dim cell As HtmlTableCell
            Dim attr As String
            For i As Integer = 0 To aPrices.Count - 1
                cell = tr.Cells(i + 3)
                attr = cell.Attributes("class")
                If attr Is Nothing OrElse Not attr.Contains("omit") Then
                    If lowPrice > 0 And lowPrice = aPrices(i) Then

                        cell.Attributes("class") = IIf(attr = "norm", "low lowTwo", attr & "low lowTwo")
                    ElseIf aPrices(i) > 0 Then
                        Dim pct As Double = Math.Round(100 * (aPrices(i) - lowPrice) / lowPrice)
                        cell.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                    End If
                    attr = cell.Attributes("class")
                    Try
                        If highPrice > 0 And highPrice = aPrices(i) And Not attr.Contains("low") Then
                            cell.Attributes("class") = IIf(attr = "norm", "high", attr & "high")
                        End If
                    Catch ex As Exception

                    End Try
                End If
            Next

            Dim TotalsArray As Double()
            Select Case tblNumber
                Case TableTypes.Priced
                    tblPriced.Rows.Add(tr)
                    TotalsArray = aPricedTotals
                Case TableTypes.Subtotal
                    tblSub.Rows.Add(tr)
                    TotalsArray = aSubTotals
                Case TableTypes.Average
                    tblAvg.Rows.Add(tr)
                    TotalsArray = aAvgTotals
                Case TableTypes.Special
                    tblSpec.Rows.Add(tr)
                    TotalsArray = aSpecTotals
            End Select

            If TotalsArray IsNot Nothing Then
                For i As Integer = 0 To aTempTotals.Length - 1
                    TotalsArray(i) += aTempTotals(i)
                Next
            End If

        Next

        Dim trComparedRow As New HtmlTableRow
        trComparedRow.Attributes("class") = "ttl1"
        tblSum.Rows.Add(trComparedRow)

        Dim trPricedSubtotalTop As New HtmlTableRow
        trPricedSubtotalTop.Attributes("class") = "sbttl"
        tblPriced.Rows.Add(trPricedSubtotalTop)

        Dim trPricedSubtotal As New HtmlTableRow
        trPricedSubtotal.Attributes("class") = "ttl2"
        tblSum.Rows.Add(trPricedSubtotal)

        Dim trSubSubtotalTop As New HtmlTableRow
        trSubSubtotalTop.Attributes("class") = "sbttl"
        tblSub.Rows.Add(trSubSubtotalTop)

        Dim trSubSubtotal As New HtmlTableRow
        trSubSubtotal.Attributes("class") = "ttl3"
        tblSum.Rows.Add(trSubSubtotal)

        Dim trAvgSubtotalTop As New HtmlTableRow
        trAvgSubtotalTop.Attributes("class") = "sbttl"
        tblAvg.Rows.Add(trAvgSubtotalTop)

        Dim trAvgSubtotal As New HtmlTableRow
        trAvgSubtotal.Attributes("class") = "ttl2"
        tblSum.Rows.Add(trAvgSubtotal)

        Dim trSpecSubtotalTop As New HtmlTableRow
        trSpecSubtotalTop.Attributes("class") = "sbttl"
        tblSpec.Rows.Add(trSpecSubtotalTop)

        Dim trSpecSubtotal As New HtmlTableRow
        trSpecSubtotal.Attributes("class") = "ttl3"
        tblSum.Rows.Add(trSpecSubtotal)

        Dim trTotalTotal As New HtmlTableRow
        trTotalTotal.Attributes("class") = "ttl1"
        tblSum.Rows.Add(trTotalTotal)

        Dim trBtnRow As New HtmlTableRow
        tblSum.Rows.Add(trBtnRow)

        trComparedRow.Cells.Add(New HtmlTableCell() With {.InnerHtml = "Number of Prices Compared"})

        td = New HtmlTableCell
        td.InnerHtml = "Subtotal: Products All Vendors Priced"
        td.Attributes("class") = "subt"
        td.ColSpan = 3

        trPricedSubtotalTop.Cells.Add(td)

        trPricedSubtotal.Cells.Add(New HtmlTableCell() With {.InnerHtml = "Subtotal: Products All Vendors Priced"})

        trTotalTotal.Cells.Add(New HtmlTableCell() With {.InnerHtml = "Total Price"})
        trBtnRow.Cells.Add(New HtmlTableCell() With {.InnerHtml = "&nbsp;"})

        Dim minPriced As Double = Aggregate t As Double In aPricedTotals.DefaultIfEmpty Into Min()
        Dim maxPriced As Double = Aggregate t As Double In aPricedTotals.DefaultIfEmpty Into Max()
        Dim minSub As Double = Aggregate t As Double In aSubTotals.DefaultIfEmpty Into Min()
        Dim maxSub As Double = Aggregate t As Double In aSubTotals.DefaultIfEmpty Into Max()
        Dim minAvg As Double = Aggregate t As Double In aAvgTotals.DefaultIfEmpty Into Min()
        Dim maxAvg As Double = Aggregate t As Double In aAvgTotals.DefaultIfEmpty Into Max()
        Dim minSpec As Double = Aggregate t As Double In aSpecTotals.DefaultIfEmpty Into Min()
        Dim maxSpec As Double = Aggregate t As Double In aSpecTotals.DefaultIfEmpty Into Max()

        Dim totals(vendorCount) As Double

        For i As Integer = 0 To aPricedCounts.Count - 1
            Dim totalTotal As Double = 0
            td = New HtmlTableCell
            td.InnerHtml = " " & aPricedCounts(i) & "/" & productCount
            trComparedRow.Cells.Add(td)

            AddFieldToTableRow(trPricedSubtotal, minPriced, maxPriced, aPricedTotals(i))

            AddFieldToTableRow(trPricedSubtotalTop, minPriced, maxPriced, aPricedTotals(i))

            totals(i) += aPricedTotals(i)
            totals(i) += aSubTotals(i)
            totals(i) += aAvgTotals(i)
            totals(i) += aSpecTotals(i)

            'total
            td = New HtmlTableCell
            trTotalTotal.Cells.Add(td)
            Dim HasDeclinedBid As Boolean = DB.ExecuteScalar("select HasDeclinedToBid from  TwoPriceCampaignVendor_Rel where VendorId = " & dtVendors.Rows(i)("VendorID") & " and TwoPriceCampaignId = " & dbTwoPriceTakeOff.TwoPriceCampaignId & "")
            'ensure all vendors have accurate summaries; build button row
            If dtVendors.Rows.Count > i Then
                If (Not HasDeclinedBid) Then

                    If aFullyPriced(i) And Not IsAwardedCampaign Then
                        td = New HtmlTableCell
                        td.InnerHtml = "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnAward, dtVendors.Rows(i)("VendorID")) & """ style=""white-space:nowrap;"">Award Vendor</a>"
                        td.Attributes("class") = "plcordr"
                    ElseIf IsAwardedCampaign Then
                        If dtVendors.Rows(i)("VendorID") = dbTwoPriceCampaign.AwardedVendorId Then
                            td = New HtmlTableCell
                            td.InnerHtml = "Awarded"
                            td.Attributes("style") = "background-color:#4CC417;padding:3px;text-align:center"
                        Else
                            td = New HtmlTableCell
                            td.InnerHtml = "Award Vendor"
                            td.Attributes("style") = "background-color:#D7D7D7;padding:3px;text-align:center"
                        End If
                    End If


                Else
                    td = New HtmlTableCell
                    td.InnerHtml = "Bid Declined"
                    td.Attributes("style") = "background-color:red;color:white;padding:3px;text-align:center"
                End If
                trBtnRow.Cells.Add(td)
            End If
        Next

        If totals.Length > 0 AndAlso totals.Max > 0 Then
            Dim totalMinAmt As Double = Aggregate t As Double In totals Where t > 0 Into Min()
            Dim totalMaxAmt As Double = Aggregate t As Double In totals Into Max()

            For i As Integer = 0 To totals.Length - 1
                td = trTotalTotal.Cells(i + 1)
                If totalMinAmt > 0 AndAlso totalMinAmt = totals(i) Then
                    td.Attributes("class") = "low"
                    td.InnerHtml = FormatCurrency(totals(i))
                ElseIf totalMaxAmt > 0 AndAlso totalMaxAmt = totals(i) Then
                    td.Attributes("class") = "high"
                    If totalMinAmt > 0 Then
                        td.InnerHtml = FormatCurrency(totals(i)) & "<br/><span class=""smaller"">" & Math.Round(100 * (totals(i) - totalMinAmt) / totalMinAmt) & "% from low</span>"
                    End If
                Else
                    td.Attributes("class") = "norm"
                    If totalMinAmt > 0 Then
                        td.InnerHtml = FormatCurrency(totals(i)) & "<br/><span class=""smaller"">" & Math.Round(100 * (totals(i) - totalMinAmt) / totalMinAmt) & "% from low</span>"
                    End If
                End If
            Next
        End If

        'hide empty tables
        tblPriced.Visible = tblPriced.Rows.Count > 4
        tblSub.Visible = tblSub.Rows.Count > 4
        tblAvg.Visible = tblAvg.Rows.Count > 4
        tblSpec.Visible = tblSpec.Rows.Count > 4
    End Sub

    Protected Sub AddFieldToTableRow(ByVal pRow As HtmlTableRow, ByVal pMin As Double, ByVal pMax As Double, ByVal pArrayElement As Double)
        Dim td = New HtmlTableCell
        td.InnerHtml = FormatCurrency(pArrayElement)
        If pMin > 0 And pArrayElement = pMin Then
            td.Attributes("class") = "low"
        Else
            Dim pct As Double = Math.Round((pArrayElement - pMin) / pMin * 100)
            If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
            End If
            If pMax > 0 AndAlso pArrayElement = pMax Then
                td.Attributes("class") = "high"
            Else
                td.Attributes("class") = "norm"
            End If
        End If
        pRow.Cells.Add(td)
    End Sub

    Protected Sub btnAward_Command(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAward.Postback
        Dim VendorID As Integer = Request("__EVENTARGUMENT")

        btnSendMessage.CommandArgument = VendorID

        pnMain.Visible = False
        pnlSendMessage.Visible = True

        Dim dbVendorAutoMessage As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignAwardedVendor")
        ltlVendorAutoMessage.Text = dbVendorAutoMessage.Message.Replace(vbCrLf, "<br>")

        Dim dbBuilderAutoMessage As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignAwardedBuilder")
        ltlBuilderAutoMessage.Text = dbBuilderAutoMessage.Message.Replace(vbCrLf, "<br>")

        'Response.Redirect("/order/default.aspx?PriceComparisonID=" & PriceComparisonID & "&VendorID=" & Server.UrlEncode(VendorID))
    End Sub

    Protected Sub btnSendMessage_Click(sender As Object, e As System.EventArgs) Handles btnSendMessage.Click
        Try
            DB.BeginTransaction()
            Dim dbTwoPriceCampaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
            Dim AwardedVendorId As Integer = btnSendMessage.CommandArgument

            If AwardedVendorId <> Nothing Then
                'Update status/awarded vendor
                dbTwoPriceCampaign.Status = "Awarded"
                dbTwoPriceCampaign.AwardedVendorId = AwardedVendorId
                dbTwoPriceCampaign.Update()

                DB.CommitTransaction()

                Dim dbVendor As VendorRow = VendorRow.GetRow(DB, AwardedVendorId)

                ' SendEmail(DB, row, sBody)
                For Each xID In dbTwoPriceCampaign.GetSelectedCampaignVendors().Split(",")
                    Dim VendorId As Integer = CInt(xID)
                    Dim dbAutoMessage As New AutomaticMessagesRow
                    Dim dbxVendor As VendorRow = VendorRow.GetRow(DB, VendorId)

                    If VendorId = AwardedVendorId Then
                        dbAutoMessage = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignAwardedVendor")
                    Else
                        dbAutoMessage = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignAwardedLosingVendor")
                    End If

                    Dim sBody As String = FormatMessage(DB, dbAutoMessage, dbxVendor, txtMessage.Text, dbTwoPriceCampaign.Name)

                    'Add ops manager to CC
                    Dim Conn As String = ""
                    If dbAutoMessage.CCList <> Nothing Then Conn = ","
                    For Each LLCID As String In dbVendor.GetSelectedLLCs.Split(",")
                        Dim LLC As LLCRow = LLCRow.GetRow(DB, LLCID)
                        dbAutoMessage.CCList &= Conn & LLC.OperationsManager
                    Next

                    dbAutoMessage.Send(dbxVendor, sBody, True)
                Next

                Dim Builders As String = dbTwoPriceCampaign.GetSelectedCampaignBuilders()
                For Each builderId As String In Builders.Split(",")
                    If builderId <> "" Then
                        Dim dbAutoMessage As New AutomaticMessagesRow
                        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, builderId)
                        dbAutoMessage = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignAwardedBuilder")
                        Dim sBody As String = FormatMessage(DB, dbAutoMessage, dbVendor, txtMessageBuilder.Text, dbTwoPriceCampaign.Name, dbBuilder.CompanyName, AppSettings("GlobalRefererName") & "/builder/twoprice/edit.aspx?TwoPriceTakeOffId=" & DB.ExecuteScalar("SELECT TOP 1 TwoPriceTakeOffId FROM TwoPriceTakeOff WHERE TwoPriceCampaignId = " & dbTwoPriceCampaign.TwoPriceCampaignId))
                        dbAutoMessage.CCList = String.Join(",", (From dr In BuilderAccountRow.GetBuilderAccounts(DB, dbBuilder.BuilderID, True) Where Not Core.GetString(dr.Item("Email")) Is Nothing And Core.GetString(dr.Item("Email")) <> dbBuilder.Email Select CStr(dr.Item("Email"))).ToArray())
                        dbAutoMessage.Send(dbBuilder, sBody, True)
                    End If
                Next
            End If
            pnMain.Visible = False
            pnlSendMessage.Visible = False
            pnComplete.Visible = True

        Catch ex As Exception
            pnMain.Visible = False
            pnlSendMessage.Visible = True
            pnComplete.Visible = False
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
    Private Function FormatMessage(ByVal DB As Database, ByVal automessage As AutomaticMessagesRow, ByVal drVendor As VendorRow, ByVal Msg As String, ByVal CampaignName As String, Optional ByVal BuilderName As String = "", Optional ByVal BidListLink As String = "") As String
        Dim tempMsg As String
        tempMsg = automessage.Message
        tempMsg = tempMsg.Replace("%%Vendor%%", drVendor.CompanyName)
        tempMsg = tempMsg.Replace("%%Campaign%%", CampaignName)
        tempMsg = tempMsg.Replace("%%AdditionalMessage%%", Msg)
        tempMsg = tempMsg.Replace("%%Builder%%", BuilderName)
        tempMsg = tempMsg.Replace("%%BidListLink%%", BidListLink)

        Return tempMsg
    End Function

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        BuildExportFromComparison()

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenExportForm", "Sys.Application.add_load(OpenExportForm);", True)
    End Sub

    Protected Sub BuildExportFromComparison()
        Dim fName As String = Core.GenerateFileID & ".csv"
        Dim f As New System.IO.FileStream(Server.MapPath("/assets/comparison/" & fName), IO.FileMode.Create)
        Dim out As New System.IO.StreamWriter(f)
        out.WriteLine("Price Comparison Export")
        out.WriteLine("Campaign:," & Core.QuoteCSV(dbTwoPriceTakeOff.Title))
        out.WriteLine("Date:," & Core.QuoteCSV(Now) & vbCrLf & vbCrLf)

        Dim priceRE As New Regex("(N/A)|(\$[\d,]*\.[\d]*)")

        OutputVisibleTable(tblPriced, priceRE, "Priced Products", out)

        out.WriteLine()
        For i As Integer = 2 To tblSum.Rows.Count - 2
            For j = 0 To tblSum.Rows(i).Cells.Count - 1
                If i > 2 And j > 0 Then
                    out.Write(IIf(j > 0, ",", "") & Core.QuoteCSV(priceRE.Match(tblSum.Rows(i).Cells(j).InnerHtml).Value))
                Else
                    out.Write(IIf(j > 0, ",", "") & Core.QuoteCSV(tblSum.Rows(i).Cells(j).InnerHtml))
                    If j = 0 Then
                        'skip to cells to line up with prices above
                        out.Write(",,")
                    End If
                End If
            Next
            out.WriteLine()
        Next

        out.WriteLine()

        lnkExportFile.HRef = "/assets/comparison/" & fName

        out.Close()
        f.Close()
    End Sub

    Protected Sub OutputVisibleTable(ByVal pTable As HtmlTable, ByVal pPriceRegex As Regex, ByVal pFirstLine As String, ByVal pOutput As System.IO.StreamWriter)
        If pTable.Visible Then
            pOutput.WriteLine(pFirstLine)
            Dim row As HtmlTableRow = Nothing
            For i As Integer = 2 To pTable.Rows.Count - 2
                row = pTable.Rows(i)
                pOutput.Write(Core.QuoteCSV(row.Cells(0).InnerHtml))
                pOutput.Write("," & Core.QuoteCSV(row.Cells(1).InnerHtml))
                pOutput.Write("," & Core.QuoteCSV(row.Cells(2).InnerHtml))
                For j As Integer = 3 To row.Cells.Count - 1
                    If i = 2 Then
                        pOutput.Write("," & Core.QuoteCSV(row.Cells(j).InnerHtml))
                    Else
                        pOutput.Write("," & Core.QuoteCSV(pPriceRegex.Match(row.Cells(j).InnerHtml).Value))
                    End If
                Next
                pOutput.WriteLine()
            Next
            pOutput.WriteLine()
        End If
    End Sub

End Class
