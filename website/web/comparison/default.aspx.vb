Imports Components
Imports DataLayer
Imports System.Linq
Imports Controls

Partial Class comparison_default2
    Inherits SitePage
    '10/25/2010
    Private IsReadOnly As Boolean = False
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

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

    Protected Property PriceComparisonID() As Integer
        Get
            Return ViewState("PriceComparisonID")
        End Get
        Set(ByVal value As Integer)
            ViewState("PriceComparisonID") = value
        End Set
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
        If Request("IsUpdate") = "Y" Then
            Session("BuilderId") = Request("BuilderId")
        Else
            EnsureBuilderAccess()
        End If

        If Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty Then
            pnlPrint.Visible = False
        End If

        Dim auth As Boolean = True 'Context.User IsNot Nothing AndAlso Context.User.Identity.IsAuthenticated
        Dim dbPriceComparison As PriceComparisonRow
        If Request("PriceComparisonID") IsNot Nothing Then
            dbPriceComparison = PriceComparisonRow.GetRow(DB, Request("PriceComparisonID"))
            Session("CurrentTakeoffId") = dbPriceComparison.TakeoffID
        Else
            dbPriceComparison = PriceComparisonRow.GetRowByTakeoff(DB, Session("CurrentTakeoffId"))
        End If

        IsReadOnly = dbPriceComparison.IsAdminComparison OrElse Not auth

        If Not IsPostBack Then
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

            If dbPriceComparison.PriceComparisonID = Nothing Then
                dbPriceComparison.BuilderID = Session("BuilderID")
                dbPriceComparison.TakeoffID = Session("CurrentTakeoffId")
                dbPriceComparison.Insert()
            Else
                Try
                    dbPriceComparison.Update()
                Catch ex As Exception
                End Try
            End If
            PriceComparisonID = dbPriceComparison.PriceComparisonID

            cbDashboard.Checked = dbPriceComparison.IsDashboard
            cbIsAdminComparison.Checked = dbPriceComparison.IsAdminComparison

            cbIsAdminComparison.Visible = Context.User IsNot Nothing AndAlso Context.User.Identity.IsAuthenticated

            If IsReadOnly Then
                'if comparison is marked admin comparison & logged-in user is not admin, then read-only
                If dbPriceComparison.IsAdminComparison Then
                    lnkTakeoff.Visible = False
                    cbDashboard.Visible = False
                    lsVendors.Visible = False
                    Session("CurrentTakeoffID") = Nothing
                End If
            End If

            Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, dbPriceComparison.TakeoffID)

            ltlTakeoffTitle.Text = IIf(dbTakeoff.Title = Nothing, "(Unsaved)", dbTakeoff.Title)

            If dbTakeoff.ProjectID = Nothing Then
                ltlTakeoffProject.Text = "(No Project)"
            Else
                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbTakeoff.ProjectID)
                ltlTakeoffProject.Text = IIf(dbProject.ProjectName <> Nothing, dbProject.ProjectName, "(No Project)")
            End If

            ltlTakeoffUpdate.Text = FormatDateTime(IIf(dbTakeoff.Saved <> Nothing, dbTakeoff.Saved, dbTakeoff.Created), DateFormat.GeneralDate)

            'lsVendors.DataSource = VendorRow.GetListByLLC(DB, dbBuilder.LLCID, "CompanyName")
            lsVendors.DataSource = VendorRow.GetTakeoffVendors(DB, dbPriceComparison.TakeoffID)
            lsVendors.DataTextField = "Label"
            lsVendors.DataValueField = "VendorId"
            lsVendors.DataBind()

            If lsVendors.DataSource.rows.count = 0 Then
                ltlNoVendors.Text = "<b class=""larger red"">No Vendors have supplied prices for the selected items</b>"
                lsVendors.Visible = False
            Else
                Dim vendors As PriceComparisonVendorSummaryCollection = PriceComparisonVendorSummaryRow.GetVendors(DB, PriceComparisonID)
                If vendors.Count > 0 Then
                    Dim s As New System.Text.StringBuilder()
                    For Each v As PriceComparisonVendorSummaryRow In vendors
                        s.Append(IIf(s.Length = 0, v.VendorID, "," & v.VendorID))
                    Next
                    'lsVendors.SelectedValues = (From r As PriceComparisonVendorSummaryRow In vendors Select Convert.ToString(r.VendorID)).Aggregate(Function(sum, app) IIf(sum = "", app, sum & "," & app))
                    lsVendors.SelectedValues = s.ToString
                End If
            End If


            BuildComparison()

        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Protected Sub frmSubstitute_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmSubstitute.Callback
        Dim json As New Web.Script.Serialization.JavaScriptSerializer
        Dim ret As String = json.Serialize(args.Data)
        frmSubstitute.CallbackResult = ret
    End Sub

    Protected Sub cbDashboard_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbDashboard.CheckedChanged
        Dim dbPriceComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, PriceComparisonID)
        dbPriceComparison.IsDashboard = cbDashboard.Checked
        dbPriceComparison.Update()
    End Sub

    Protected Sub cbIsAdminComparison_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbIsAdminComparison.CheckedChanged
        Dim dbPriceComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, PriceComparisonID)
        dbPriceComparison.IsAdminComparison = cbIsAdminComparison.Checked
        dbPriceComparison.Update()
    End Sub

    Private Sub SubstituteProducts(ByRef pVendorRow As PriceComparisonVendorProductPriceRow, _
                                        ByRef pTableCell As HtmlTableCell, _
                                        ByRef pTableNumber As Integer, _
                                        ByRef pHighPrice As Double, _
                                        ByRef pLowPrice As Double, _
                                        ByRef pPrices As ArrayList, _
                                        ByRef pTempTotals As Double(), _
                                        ByRef pFullyPriced As Boolean(), _
                                        ByRef pSubsIncomplete As Boolean(), _
                                        ByRef pPricedCounts As Integer(), _
                                        ByVal pSubs As DataRow, _
                                        ByVal pStateRow As DataRowView, _
                                        ByVal pTakeOffRow As DataRow, _
                                        ByVal pIndex As Integer)

        If pTableNumber < TableTypes.Subtotal Then
            pTableNumber = TableTypes.Subtotal
        End If
        Dim extendedPrice As Double = pSubs("VendorPrice") * pVendorRow.Quantity
        pTableCell.InnerHtml = FormatCurrency(extendedPrice)
        If Not IsReadOnly Then
            pTableCell.Attributes("onclick") = "OpenSubForm(" & pTakeOffRow("TakeoffProductID") & "," & pStateRow("VendorID") & "," & Core.Escape(pSubs("SubstituteProduct")) & "," & Core.Escape(Core.GetString(pSubs("VendorSku"))) & ",'" & FormatCurrency(Core.GetDouble(pSubs("VendorPrice"))) & "'," & pSubs("RecommendedQuantity") & ");"
        End If
        pPrices.Add(extendedPrice)
        pPricedCounts(pIndex) += 1

        TestPrices(extendedPrice, pHighPrice, pLowPrice)

        Select Case Core.GetInt(pStateRow("State"))
            Case ProductState.Init
                pTableCell.Attributes("class") = "sub"
                pTempTotals(pIndex) = extendedPrice
                pFullyPriced(pIndex) = False
                pSubsIncomplete(pIndex) = True
            Case ProductState.Accepted
                pTableCell.Attributes("class") = "subaccpt"
                pTempTotals(pIndex) = extendedPrice
            Case ProductState.Omit
                pTableCell.Attributes("class") = "subreject"
                pTempTotals(pIndex) = 0
        End Select

        ComposeVenderRow(pVendorRow, Core.GetInt(pSubs("RecommendedQuantity")), _
                            pSubs("SubProductID"), pSubs("VendorPrice"))
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
        Dim dbComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, PriceComparisonID)
        Dim oldVendors As String = PriceComparisonRow.GetSavedVendors(DB, PriceComparisonID)
        Dim newVendors As String = IIf(IsReadOnly, oldVendors, lsVendors.SelectedValues)
        Dim aOld As New ArrayList(oldVendors.Split(","))
        Dim aNew As New ArrayList(newVendors.Split(","))
        For Each oldVendor In aOld
            If IsNumeric(oldVendor) And Not aNew.Contains(oldVendor) Then
                'If Not aNew.Contains(oldVendor) Then
                PriceComparisonVendorSummaryRow.RemoveRow(DB, PriceComparisonID, oldVendor)
                'Else
                '    aNew.Remove(oldVendor)
                'End If
            End If
        Next

        For Each id As String In aNew
            If IsNumeric(id) Then
                PriceComparisonRow.InitVendor(DB, PriceComparisonID, id)
            End If
        Next

        Dim dtVendors As DataTable = DB.GetDataTable("select * from Vendor where IsActive = 1 and VendorID in " & DB.NumberMultiple(newVendors) & " order by VendorID")

        Dim dtTakeoffProducts As DataTable = TakeOffRow.GetTakeoffProducts(DB, dbComparison.TakeoffID)
        Dim dtPriced As DataTable = TakeOffRow.GetTakeoffProductPrices(DB, dbComparison.TakeoffID, newVendors)
        Dim dtSubs As DataTable = TakeOffRow.GetTakeoffSubstitutions(DB, dbComparison.TakeoffID, newVendors)
        Dim dtAvgs As DataTable = TakeOffRow.GetTakeoffAveragePrices(DB, dbComparison.TakeoffID)
        Dim dtSpecial As DataTable = TakeOffRow.GetTakeoffSpecialPrices(DB, dbComparison.TakeoffID, newVendors)
        Dim dtState As DataTable = PriceComparisonVendorProductPriceRow.GetVendorProductState(DB, dbComparison.PriceComparisonID)

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

            Dim dv As New DataView(dtState, "TakeoffProductID=" & TakeOffRow("TakeoffProductID"), "VendorID", DataViewRowState.CurrentRows)

            'index to current vendor in various structures ordered by VendorID
            Dim idx As Integer = 0
            For Each stateRow As DataRowView In dv
                Dim dbVendorProductPrice As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(DB, PriceComparisonID, stateRow("VendorID"), TakeOffRow("TakeoffProductID"))
                td = New HtmlTableCell
                tr.Cells.Add(td)

                Dim sql As String = "VendorID=" & stateRow("VendorID") & " and TakeoffProductID=" & TakeOffRow("TakeoffProductID")

                If Not IsDBNull(TakeOffRow("SpecialOrderProductID")) Then
                    'Special Order products

                    tblNumber = TableTypes.Special

                    Dim subs As DataRow() = dtSubs.Select(sql)
                    If subs.Length > 0 Then
                        SubstituteProducts(dbVendorProductPrice, td, tblNumber, highPrice, lowPrice, aPrices, aTempTotals, aFullyPriced, aSubsIncomplete, aPricedCounts, subs(0), stateRow, TakeOffRow, idx)
                    Else

                        Dim s As DataRow() = dtSpecial.Select(sql)
                        If s.Length > 0 Then
                            'if priced

                            Dim extendedPrice As Double = s(0)("VendorPrice") * dbVendorProductPrice.Quantity
                            td.InnerHtml = FormatCurrency(extendedPrice)
                            td.Attributes("class") = "norm"
                            aPrices.Add(extendedPrice)
                            aTempTotals(idx) = extendedPrice
                            aPricedCounts(idx) += 1

                            TestPrices(extendedPrice, highPrice, lowPrice)

                            dbVendorProductPrice.UnitPrice = s(0)("VendorPrice")
                            dbVendorProductPrice.Total = dbVendorProductPrice.UnitPrice * dbVendorProductPrice.Quantity

                            If Not IsReadOnly Then
                                td.Attributes("onclick") = "OpenSpecialForm(" & TakeOffRow("TakeoffProductID") & "," & stateRow("VendorID") & "," & Core.Escape(Core.GetString(TakeOffRow("Product"))) & ",'" & FormatCurrency(s(0)("VendorPrice")) & "');"
                            End If
                        Else
                            'if not priced

                            td.InnerHtml = "N/A"

                            aCanRequest(idx) = True
                            Select Case Core.GetInt(stateRow("State"))
                                Case ProductState.Accepted
                                    td.Attributes("class") = "noneaccpt"
                                Case ProductState.Omit
                                    td.Attributes("class") = "noneomit"
                                Case ProductState.Pending
                                    td.Attributes("class") = "nonepend"
                                    aFullyPriced(idx) = False
                                Case Else
                                    If IsDBNull(stateRow("VendorProductPriceRequestID")) Then
                                        td.Attributes("class") = "none"
                                        aFullyPriced(idx) = False
                                    Else
                                        td.Attributes("class") = "nonepend"
                                        aFullyPriced(idx) = False
                                        aCanRequest(idx) = False

                                    End If


                            End Select

                            If Not IsReadOnly Then
                                td.Attributes("onclick") = "OpenSpecialForm(" & TakeOffRow("TakeoffProductID") & "," & stateRow("VendorID") & "," & Core.Escape(Core.GetString(TakeOffRow("Product"))) & ",'N/A');"
                            End If
                        End If
                    End If

                    dbVendorProductPrice.SpecialOrderProductID = TakeOffRow("SpecialOrderProductID")
                Else
                    'Non-Special Products

                    Dim p As DataRow() = dtPriced.Select(sql)
                    If p.Length > 0 Then
                        'Priced Products

                        Dim extendedPrice As Double = p(0)("VendorPrice") * dbVendorProductPrice.Quantity
                        td.InnerHtml = FormatCurrency(extendedPrice)
                        td.Attributes("class") = "norm"
                        aPrices.Add(extendedPrice)
                        aTempTotals(idx) = extendedPrice
                        aPricedCounts(idx) += 1

                        TestPrices(extendedPrice, highPrice, lowPrice)

                        dbVendorProductPrice.ProductID = p(0)("ProductID")
                        dbVendorProductPrice.UnitPrice = p(0)("VendorPrice")
                        dbVendorProductPrice.Total = dbVendorProductPrice.Quantity * dbVendorProductPrice.UnitPrice
                    Else
                        Dim subs As DataRow() = dtSubs.Select(sql)
                        If subs.Length > 0 Then
                            SubstituteProducts(dbVendorProductPrice, td, tblNumber, highPrice, lowPrice, aPrices, aTempTotals, aFullyPriced, aSubsIncomplete, aPricedCounts, subs(0), stateRow, TakeOffRow, idx)
                        Else
                            'Un-priced Products

                            Dim a As DataRow() = dtAvgs.Select("TakeoffProductID=" & TakeOffRow("TakeoffProductID"))
                            If a.Length > 0 Then
                                If tblNumber < TableTypes.Average Then
                                    tblNumber = TableTypes.Average
                                End If
                                If Core.GetDouble(a(0)("AvgPrice")) > 0 Then
                                    'LLC Average Price available

                                    Dim extendedPrice As Double = a(0)("AvgPrice") * dbVendorProductPrice.Quantity
                                    td.InnerHtml = FormatCurrency(extendedPrice)
                                    aPrices.Add(extendedPrice)

                                    TestPrices(extendedPrice, highPrice, lowPrice)

                                    If Core.GetInt(stateRow("State")) <> ProductState.Omit Then
                                        aTempTotals(idx) = extendedPrice
                                    Else
                                        aTempTotals(idx) = 0
                                    End If
                                Else
                                    'LLC Average price not available

                                    td.InnerHtml = "N/A"
                                    aPrices.Add(-1)
                                End If


                                aCanRequest(idx) = True
                                Select Case Core.GetInt(stateRow("State"))
                                    Case ProductState.Omit
                                        td.Attributes("class") = "noneomit"
                                    Case ProductState.Accepted
                                        td.Attributes("class") = "noneaccpt"
                                    Case ProductState.Pending
                                        td.Attributes("class") = "nonepend"
                                        aFullyPriced(idx) = False
                                    Case Else
                                        If IsDBNull(stateRow("VendorProductPriceRequestID")) Then
                                            td.Attributes("class") = "none"
                                            aFullyPriced(idx) = False
                                        Else
                                            td.Attributes("class") = "nonepend"
                                            aFullyPriced(idx) = False
                                            aCanRequest(idx) = False

                                        End If

                                End Select

                                Dim price As String = IIf(Core.GetDouble(a(0)("AvgPrice")) > 0, FormatCurrency(Core.GetDouble(a(0)("AvgPrice"))), "N/A")
                                If Not IsReadOnly Then
                                    td.Attributes("onclick") = "OpenSpecialForm(" & TakeOffRow("TakeoffProductID") & "," & stateRow("VendorID") & "," & Core.Escape(Core.GetString(TakeOffRow("Product"))) & ",'" & price & "');"
                                End If

                                dbVendorProductPrice.ProductID = a(0)("ProductID")
                                If IsNumeric(price) And Core.GetInt(stateRow("State")) = ProductState.Accepted Then
                                    dbVendorProductPrice.UnitPrice = a(0)("AvgPrice")
                                    dbVendorProductPrice.Total = dbVendorProductPrice.UnitPrice * dbVendorProductPrice.Quantity
                                Else
                                    dbVendorProductPrice.UnitPrice = Nothing
                                    dbVendorProductPrice.Total = Nothing
                                End If
                            End If
                        End If
                    End If
                End If

                dbVendorProductPrice.Update()
                idx += 1
            Next
            Dim cell As HtmlTableCell
            Dim attr As String
            For i As Integer = 0 To aPrices.Count - 1
                cell = tr.Cells(i + 3)
                attr = cell.Attributes("class")
                If attr Is Nothing OrElse Not attr.Contains("omit") Then
                    If lowPrice > 0 And lowPrice = aPrices(i) Then
                        cell.Attributes("class") = IIf(attr = "norm", "low", attr & "low")
                    ElseIf aPrices(i) > 0 Then
                        Dim pct As Double = Math.Round(100 * (aPrices(i) - lowPrice) / lowPrice)
                        cell.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                    End If
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

        td = New HtmlTableCell()
        td.InnerHtml = "Subtotal: Priced Substitutions Included"
        td.Attributes("class") = "subt"
        td.ColSpan = 3
        trSubSubtotalTop.Cells.Add(td)

        trSubSubtotal.Cells.Add(New HtmlTableCell() With {.InnerHtml = "Subtotal: Priced Substitutions Included"})

        trAvgSubtotal.Cells.Add(New HtmlTableCell() With {.InnerHtml = "Subtotal: Averages Used if Pricing Missing"})

        td = New HtmlTableCell()
        td.InnerHtml = "Subtotal: Averages Used if Pricing Missing"
        td.Attributes("class") = "subt"
        td.ColSpan = 3
        trAvgSubtotalTop.Cells.Add(td)

        trSpecSubtotal.Cells.Add(New HtmlTableCell() With {.InnerHtml = "Subtotal: Special Order Products"})

        td = New HtmlTableCell
        td.InnerHtml = "Subtotal: Special Order Products"
        td.Attributes("class") = "subt"
        td.ColSpan = 3
        trSpecSubtotalTop.Cells.Add(td)

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
            'when exporting to excel 2/2 is converted to 2-feb so added a space as work around
            td.InnerHtml = " " & aPricedCounts(i) & "/" & productCount
            trComparedRow.Cells.Add(td)

            AddFieldToTableRow(trPricedSubtotal, minPriced, maxPriced, aPricedTotals(i))
            AddFieldToTableRow(trSubSubtotal, minSub, maxSub, aSubTotals(i))
            AddFieldToTableRow(trAvgSubtotal, minAvg, maxAvg, aAvgTotals(i))
            AddFieldToTableRow(trSpecSubtotal, minSpec, maxSpec, aSpecTotals(i))

            AddFieldToTableRow(trPricedSubtotalTop, minPriced, maxPriced, aPricedTotals(i))
            AddFieldToTableRow(trSubSubtotalTop, minSub, maxSub, aSubTotals(i))
            AddFieldToTableRow(trAvgSubtotalTop, minAvg, maxAvg, aAvgTotals(i))
            AddFieldToTableRow(trSpecSubtotalTop, minSpec, maxSpec, aSpecTotals(i))

            totals(i) += aPricedTotals(i)
            totals(i) += aSubTotals(i)
            totals(i) += aAvgTotals(i)
            totals(i) += aSpecTotals(i)

            'total
            td = New HtmlTableCell
            trTotalTotal.Cells.Add(td)

            'ensure all vendors have accurate summaries; build button row
            If dtVendors.Rows.Count > i Then
                If Not IsReadOnly Then
                    If aFullyPriced(i) Then
                        td = New HtmlTableCell
                        td.InnerHtml = "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnOrder, dtVendors.Rows(i)("VendorID")) & """ style=""white-space:nowrap;"">Place Order</a>"
                        td.Attributes("class") = "plcordr"
                    ElseIf aCanRequest(i) Then
                        td = New HtmlTableCell
                        td.InnerHtml = "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(btnRequestPricing, dtVendors.Rows(i)("VendorID")) & """ class=""btnblue"" style=""white-space:nowrap;padding-left:10px;padding-right:10px;"">Request Pricing</a>"
                    Else
                        td = New HtmlTableCell
                        td.InnerHtml = "<b>Prices Pending</b>"

                        If aSubsIncomplete(i) Then
                            td.InnerHtml &= "<br/><b>Substitutions Incomplete</b>"
                        End If
                    End If

                    trBtnRow.Cells.Add(td)
                End If

                Dim dbSummary As PriceComparisonVendorSummaryRow = PriceComparisonVendorSummaryRow.GetRow(DB, PriceComparisonID, dtVendors.Rows(i)("VendorID"))
                dbSummary.PriceComparisonID = PriceComparisonID
                dbSummary.VendorID = dtVendors.Rows(i)("VendorID")
                dbSummary.Subtotal = totals(i)

                'tax & total irrelevant until order placed
                dbSummary.Tax = 0
                dbSummary.Total = dbSummary.Subtotal

                If dbSummary.CreateDate = Nothing Then
                    dbSummary.Insert()
                Else
                    dbSummary.Update()
                End If
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

    Protected Sub BuildExportFromComparison()
        Dim fName As String = Core.GenerateFileID & ".csv"
        Dim f As New System.IO.FileStream(Server.MapPath("/assets/comparison/" & fName), IO.FileMode.Create)
        Dim out As New System.IO.StreamWriter(f)
        Dim dbComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, PriceComparisonID)
        Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, dbComparison.TakeoffID)
        out.WriteLine("Price Comparison Export")
        out.WriteLine("Takeoff:," & Core.QuoteCSV(dbTakeoff.Title))
        out.WriteLine("Date:," & Core.QuoteCSV(Now) & vbCrLf & vbCrLf)

        Dim priceRE As New Regex("(N/A)|(\$[\d,]*\.[\d]*)")

        OutputVisibleTable(tblPriced, priceRE, "Products all vendors priced:", out)
        OutputVisibleTable(tblSub, priceRE, "Products for which some vendors provided substitutes:", out)
        OutputVisibleTable(tblAvg, priceRE, "Products some vendors did not price:", out)
        OutputVisibleTable(tblSpec, priceRE, "Special Order Products:", out)

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

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenExportForm", "Sys.Application.add_load(OpenExportForm);", True)
        'log Export Comparision
        Core.DataLog("Price Comparision", PageURL, CurrentUserId, "Export Comparision", PriceComparisonID, "", "", "", UserName)
        'end log
    End Sub

    Protected Sub lsVendors_SelectedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lsVendors.SelectedChanged
        BuildComparison()
    End Sub

    Public Sub RequestPricing(ByVal PriceComparisonId As Integer, ByVal VendorId As Integer)
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "MissingPrice")
        'Dim dbRecip As New automatic
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim msg As New StringBuilder

        msg.Append(dbBuilder.CompanyName & " is requesting pricing for the following items:" & vbCrLf & vbCrLf)

        Dim dtProducts As DataTable = PriceComparisonRow.GetVendorProducts(DB, PriceComparisonId, VendorId, True)
        'Dim q = From product As DataRow In dtProducts.AsEnumerable Where (IsDBNull(product("UnitPrice")) OrElse product("UnitPrice") < 0) OrElse (product("State") <> Controls.ProductState.Accepted And product("State") <> Controls.ProductState.Omit And IsDBNull(product("SubstituteProductID"))) Select product
        For Each row As DataRow In dtProducts.Rows
            If (IsDBNull(row("UnitPrice")) OrElse row("UnitPrice") <= 0) Then
                Dim dbRequest As VendorProductPriceRequestRow = VendorProductPriceRequestRow.GetRow(DB, dbBuilder.BuilderID, dbVendor.VendorID, row("TakeoffProductID"))
                dbRequest.BuilderID = dbBuilder.BuilderID
                If dbRequest.Created = Nothing Then
                    dbRequest.CreatorBuilderAccountID = Session("BuilderAccountId")
                End If
                If Not IsDBNull(row("ProductID")) Then
                    dbRequest.ProductID = row("ProductID")
                End If
                dbRequest.TakeoffProductID = row("TakeoffProductID")
                dbRequest.VendorID = dbVendor.VendorID
                If Not IsDBNull(row("SpecialOrderProductID")) Then
                    dbRequest.SpecialOrderProductID = Core.GetInt(row("SpecialOrderProductID"))
                End If
                If dbRequest.Created = Nothing Then
                    dbRequest.Insert()
                Else
                    dbRequest.Update()
                End If

                Try
                    If Not IsDBNull(row("CBUSASKU")) Then
                        msg.Append("CBUSA Sku # " & row("CBUSASKU") & " - " & row("Product"))
                    Else
                        msg.Append(row("Product"))
                    End If
                Catch ex As Exception
                    Logger.Error("Price Comparision : " & PriceComparisonId & "  " & ex.ToString)
                End Try

                If Not IsDBNull(row("SpecialOrderProductId")) Then
                    msg.Append("(Special Order)")
                End If
                'msg.Append(vbTab & row("Description"))
                msg.AppendLine()
            End If
        Next
        msg.AppendLine()
        msg.Append("Please click the link below to access the CBUSA software")
        msg.AppendLine()
        msg.Append("https://app.custombuilders-usa.com/default.aspx")
        'msg.AppendLine(dbMsg.Message)

        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        Dim FromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")

        Dim sMsg As String = msg.ToString

        'FOR TESTING
        If SysParam.GetValue(DB, "TestMode") = True Then
            dbVendor.Email = FromEmail
        End If

        'log Mail Sent For Request Pricing 
        Core.DataLog("Price Comparision", PageURL, CurrentUserId, "Mail Sent For Request Pricing", "", "", "", "", UserName)
        'end log
        If dbMsg.IsEmail Then
            dbMsg.Send(dbVendor, msg.ToString)
            'Core.SendSimpleMail(FromEmail, FromName, dbVendor.Email, dbVendor.CompanyName, dbMsg.Subject, sMsg)
            'If dbMsg.CCList <> String.Empty Then
            '    Dim aEmails() As String = dbMsg.CCList.Split(",")
            '    For Each email As String In aEmails
            '        Core.SendSimpleMail(FromEmail, FromName, email, email, dbMsg.Subject, sMsg)
            '    Next
            'End If
        End If
    End Sub

    Protected Sub btnRequestPricing_Command(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRequestPricing.Postback
        Dim VendorID As Integer = Request("__EVENTARGUMENT")

        'log Request Pricing 
        Core.DataLog("Price Comparision", PageURL, CurrentUserId, "Btn Request Pricing", "", "", "", "", UserName)
        'end log
        RequestPricing(PriceComparisonID, VendorID)

        BuildComparison()
    End Sub

    Protected Sub btnOrder_Command(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOrder.Postback
        Dim VendorID As Integer = Request("__EVENTARGUMENT")
        'log Place Order
        Core.DataLog("Price Comparision", PageURL, CurrentUserId, "Place Order", PriceComparisonID, "", "", "", UserName)
        'end log
        Response.Redirect("/order/default.aspx?PriceComparisonID=" & PriceComparisonID & "&VendorID=" & Server.UrlEncode(VendorID))
    End Sub

    Protected Sub frmSubstitute_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSubstitute.Postback
        If Not IsNumeric(txtSubQuantity.Text) Then
            AddError("Invalid Quantity")
            Exit Sub
        End If

        Dim btn As Button = sender
        Dim TakeoffProductID As Integer = hdnSubTakeoffProductID.Value
        Dim VendorID As Integer = hdnSubVendorID.Value
        Dim dbPrice As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(DB, PriceComparisonID, VendorID, TakeoffProductID)
        Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, TakeoffProductID)

        Select Case btn.CommandName
            Case "Accept"
                dbPrice.Quantity = txtSubQuantity.Text
                dbPrice.State = ProductState.Accepted
            Case "Reject"
                dbPrice.Quantity = dbTakeoffProduct.Quantity
                dbPrice.State = ProductState.Omit
        End Select

        dbPrice.Update()
        BuildComparison()
    End Sub

    Protected Sub frmAverage_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmAverage.Postback
        Dim btn As Button = sender
        Dim takeoffProductID As Integer = hdnAvgTakeoffProductID.Value
        Dim vendorID As Integer = hdnAvgVendorID.Value
        Dim dbPrice As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(DB, PriceComparisonID, vendorID, takeoffProductID)
        Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, takeoffProductID)

        Select Case btn.CommandName
            Case "Accept"
                dbPrice.State = ProductState.Accepted
            Case "Omit"
                dbPrice.State = ProductState.Omit
            Case "Request"
                dbPrice.State = ProductState.Pending

                Dim dbRequest As VendorProductPriceRequestRow = VendorProductPriceRequestRow.GetRow(DB, Session("BuilderId"), vendorID, takeoffProductID)
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
                Dim dbVendor As VendorRow = VendorRow.GetRow(DB, vendorID)
                If dbRequest.Created = Nothing Then
                    dbRequest.BuilderID = Session("BuilderId")
                    dbRequest.CreatorBuilderAccountID = Session("BuilderAccountId")
                    dbRequest.ProductID = dbTakeoffProduct.ProductID
                    dbRequest.SpecialOrderProductID = dbTakeoffProduct.SpecialOrderProductID
                    dbRequest.TakeoffProductID = takeoffProductID
                    dbRequest.VendorID = vendorID
                    dbRequest.Insert()

                    Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "MissingPrice")
                    Dim msg As New StringBuilder

                    msg.Append(dbBuilder.CompanyName & " is requesting pricing for the following item:" & vbCrLf & vbCrLf)
                    If dbTakeoffProduct.ProductID <> Nothing Then
                        Dim dbProduct As ProductRow = ProductRow.GetRow(DB, dbTakeoffProduct.ProductID)
                        If dbProduct.SKU = String.Empty Then
                            msg.AppendLine(dbProduct.Product)
                        Else
                            msg.AppendLine("CBUSA Sku # " & dbProduct.SKU & " - " & dbProduct.Product)
                        End If

                    ElseIf dbTakeoffProduct.SpecialOrderProductID <> Nothing Then
                        Dim dbProduct As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, dbTakeoffProduct.SpecialOrderProductID)
                        msg.AppendLine(dbProduct.SpecialOrderProduct)
                    End If

                    dbMsg.Send(dbVendor, msg.ToString)
                End If
        End Select

        dbPrice.Update()

        BuildComparison()
    End Sub

    Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
        BuildComparison()
        BuildExportFromComparison()
    End Sub
End Class
