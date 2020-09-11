Imports Components
Imports DataLayer
Imports System.Linq
Imports Controls

Partial Class comparison_default2
    Inherits SitePage

    Private IsReadOnly As Boolean = False

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
        EnsureBuilderAccess()
        If Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty Then
            pnlPrint.Visible = False
        End If

        Dim dbPriceComparison As PriceComparisonRow
        If Request("PriceComparisonID") IsNot Nothing Then
            dbPriceComparison = PriceComparisonRow.GetRow(DB, Request("PriceComparisonID"))
            Session("CurrentTakeoffId") = dbPriceComparison.TakeoffID
        Else
            dbPriceComparison = PriceComparisonRow.GetRowByTakeoff(DB, Session("CurrentTakeoffId"))
        End If

        If Not dbPriceComparison.IsAdminComparison OrElse (Context.User IsNot Nothing AndAlso Context.User.Identity.IsAuthenticated) Then
            IsReadOnly = False
        Else
            IsReadOnly = True
        End If

        If Not IsPostBack Then
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

            If dbPriceComparison.PriceComparisonID = Nothing Then
                dbPriceComparison.BuilderID = Session("BuilderID")
                dbPriceComparison.TakeoffID = Session("CurrentTakeoffId")
                dbPriceComparison.Insert()
            End If
            PriceComparisonID = dbPriceComparison.PriceComparisonID

            cbDashboard.Checked = dbPriceComparison.IsDashboard
            cbIsAdminComparison.Checked = dbPriceComparison.IsAdminComparison

            If Context.User IsNot Nothing AndAlso Context.User.Identity.IsAuthenticated Then
                cbIsAdminComparison.Visible = True
            Else
                cbIsAdminComparison.Visible = False
            End If

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

            If dbTakeoff.Title = Nothing Then
                ltlTakeoffTitle.Text = "(Unsaved)"
            Else
                ltlTakeoffTitle.Text = dbTakeoff.Title
            End If

            If dbTakeoff.ProjectID = Nothing Then
                ltlTakeoffProject.Text = "(No Project)"
            Else
                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbTakeoff.ProjectID)
                If dbProject.ProjectName <> Nothing Then
                    ltlTakeoffProject.Text = dbProject.ProjectName
                Else
                    ltlTakeoffProject.Text = "(No Project)"
                End If
            End If

            If dbTakeoff.Saved <> Nothing Then
                ltlTakeoffUpdate.Text = FormatDateTime(dbTakeoff.Saved, DateFormat.GeneralDate)
            Else
                ltlTakeoffUpdate.Text = FormatDateTime(dbTakeoff.Created, DateFormat.GeneralDate)
            End If

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

        Dim dtVendors As DataTable = DB.GetDataTable("select * from Vendor where VendorID in " & DB.NumberMultiple(newVendors) & " order by VendorID")

        Dim dtTakeoffProducts As DataTable = TakeOffRow.GetTakeoffProducts(DB, dbComparison.TakeoffID)
        Dim dtPriced As DataTable = TakeOffRow.GetTakeoffProductPrices(DB, dbComparison.TakeoffID, newVendors)
        Dim dtSubs As DataTable = TakeOffRow.GetTakeoffSubstitutions(DB, dbComparison.TakeoffID, newVendors)
        Dim dtAvgs As DataTable = TakeOffRow.GetTakeoffAveragePrices(DB, dbComparison.TakeoffID)
        Dim dtSpecial As DataTable = TakeOffRow.GetTakeoffSpecialPrices(DB, dbComparison.TakeoffID, newVendors)
        Dim dtState As DataTable = PriceComparisonVendorProductPriceRow.GetVendorProductState(DB, dbComparison.PriceComparisonID)

        Dim productCount As Integer = dtTakeoffProducts.Rows.Count
        Dim aPricedCounts(dtVendors.Rows.Count - 1) As Integer
        Dim aPricedTotals(dtVendors.Rows.Count - 1) As Double
        Dim aSubTotals(dtVendors.Rows.Count - 1) As Double
        Dim aAvgTotals(dtVendors.Rows.Count - 1) As Double
        Dim aSpecTotals(dtVendors.Rows.Count - 1) As Double
        Dim aFullyPriced(dtVendors.Rows.Count - 1) As Boolean
        For i As Integer = 0 To aFullyPriced.Length - 1
            aFullyPriced(i) = True
        Next
        Dim aCanRequest(dtVendors.Rows.Count - 1) As Boolean
        For i As Integer = 0 To aCanRequest.Length - 1
            aCanRequest(i) = False
        Next
        Dim aSubsIncomplete(dtVendors.Rows.Count - 1) As Boolean
        For i As Integer = 0 To aSubsIncomplete.Length - 1
            aSubsIncomplete(i) = False
        Next

        Dim td As HtmlTableCell
        For Each row As DataRow In dtVendors.Rows
            td = New HtmlTableCell("th")
            td.InnerHtml = row("CompanyName")
            trPricedHeader.Cells.Add(td)

            td = New HtmlTableCell("th")
            td.InnerHtml = row("CompanyName")
            trSubHeader.Cells.Add(td)

            td = New HtmlTableCell("th")
            td.InnerHtml = row("CompanyName")
            trAvgHeader.Cells.Add(td)

            td = New HtmlTableCell("th")
            td.InnerHtml = row("CompanyName")
            trSpecHeader.Cells.Add(td)

            td = New HtmlTableCell("th")
            td.InnerHtml = row("CompanyName")
            trSumHeader.Cells.Add(td)
        Next

        Dim aPrices As New ArrayList
        For Each TakeOffRow As DataRow In dtTakeoffProducts.Rows
            Dim tr As New HtmlTableRow

            td = New HtmlTableCell
            td.InnerHtml = TakeOffRow("Quantity")
            tr.Cells.Add(td)

            td = New HtmlTableCell
            td.InnerHtml = Core.GetString(TakeOffRow("SKU"))
            tr.Cells.Add(td)

            td = New HtmlTableCell
            If Not IsDBNull(TakeOffRow("Product")) Then
                td.InnerHtml = TakeOffRow("Product")
            ElseIf Not IsDBNull(TakeOffRow("SpecialOrderProduct")) Then
                td.InnerHtml = TakeOffRow("SpecialOrderProduct")
            End If
            tr.Cells.Add(td)

            Dim lowPrice As Double = -1
            Dim highPrice As Double = -1
            aPrices.Clear()
            Dim tblNumber As Integer = 0
            Dim aTempTotals(dtVendors.Rows.Count - 1) As Double

            Dim dv As New DataView(dtState, "TakeoffProductID=" & TakeOffRow("TakeoffProductID"), "VendorID", DataViewRowState.CurrentRows)

            'index to current vendor in various structures ordered by VendorID
            Dim idx As Integer = 0
            For Each stateRow As DataRowView In dv
                Dim dbVendorProductPrice As PriceComparisonVendorProductPriceRow = PriceComparisonVendorProductPriceRow.GetRow(DB, PriceComparisonID, stateRow("VendorID"), TakeOffRow("TakeoffProductID"))
                td = New HtmlTableCell
                tr.Cells.Add(td)


                If Not IsDBNull(TakeOffRow("SpecialOrderProductID")) Then
                    'Special Order products

                    tblNumber = 3
                    Dim subs As DataRow() = dtSubs.Select("VendorID=" & stateRow("VendorID") & " and TakeoffProductID=" & TakeOffRow("TakeoffProductID"))
                    If subs.Length > 0 Then
                        'Special order substitutes

                        If tblNumber < 1 Then
                            tblNumber = 1
                        End If
                        Dim extendedPrice As Double = subs(0)("VendorPrice") * dbVendorProductPrice.Quantity
                        td.InnerHtml = FormatCurrency(extendedPrice)
                        If Not IsReadOnly Then
                            td.Attributes("onclick") = "OpenSubForm(" & TakeOffRow("TakeoffProductID") & "," & stateRow("VendorID") & "," & Core.Escape(subs(0)("SubstituteProduct")) & "," & Core.Escape(Core.GetString(subs(0)("VendorSku"))) & ",'" & FormatCurrency(Core.GetDouble(subs(0)("VendorPrice"))) & "'," & subs(0)("RecommendedQuantity") & ");"
                        End If
                        aPrices.Add(extendedPrice)
                        If lowPrice = -1 OrElse extendedPrice < lowPrice Then
                            lowPrice = extendedPrice
                        End If
                        If highPrice = -1 OrElse extendedPrice > highPrice Then
                            highPrice = extendedPrice
                        End If

                        If aPricedCounts(idx) = Nothing Then
                            aPricedCounts(idx) = 1
                        Else
                            aPricedCounts(idx) += 1
                        End If

                        Select Case Core.GetInt(stateRow("State"))
                            Case ProductState.Init
                                td.Attributes("class") = "sub"
                                aTempTotals(idx) = extendedPrice
                                aFullyPriced(idx) = False
                                aSubsIncomplete(idx) = True
                            Case ProductState.Accepted
                                td.Attributes("class") = "subaccpt"
                                aTempTotals(idx) = extendedPrice
                            Case ProductState.Omit
                                td.Attributes("class") = "subreject"
                                aTempTotals(idx) = 0
                        End Select

                        dbVendorProductPrice.RecommendedQuantity = Core.GetInt(subs(0)("RecommendedQuantity"))
                        If dbVendorProductPrice.RecommendedQuantity = 0 Then
                            dbVendorProductPrice.RecommendedQuantity = dbVendorProductPrice.Quantity
                        End If
                        dbVendorProductPrice.SubstituteProductID = subs(0)("SubProductID")
                        dbVendorProductPrice.UnitPrice = subs(0)("VendorPrice")
                        dbVendorProductPrice.Total = dbVendorProductPrice.UnitPrice * dbVendorProductPrice.Quantity

                    Else
                        'Special order non-subtitutes

                        Dim s As DataRow() = dtSpecial.Select("VendorID=" & stateRow("VendorID") & " and TakeoffProductID=" & TakeOffRow("TakeoffProductID"))
                        If s.Length > 0 Then
                            'if priced

                            Dim extendedPrice As Double = s(0)("VendorPrice") * dbVendorProductPrice.Quantity
                            td.InnerHtml = FormatCurrency(extendedPrice)
                            td.Attributes("class") = "norm"
                            aPrices.Add(extendedPrice)
                            If lowPrice = -1 OrElse extendedPrice < lowPrice Then
                                lowPrice = extendedPrice
                            End If
                            If highPrice = -1 OrElse extendedPrice > highPrice Then
                                highPrice = extendedPrice
                            End If

                            aTempTotals(idx) = extendedPrice

                            If aPricedCounts(idx) = Nothing Then
                                aPricedCounts(idx) = 1
                            Else
                                aPricedCounts(idx) += 1
                            End If

                            dbVendorProductPrice.UnitPrice = s(0)("VendorPrice")
                            dbVendorProductPrice.Total = dbVendorProductPrice.UnitPrice * dbVendorProductPrice.Quantity

                            If Not IsReadOnly Then
                                td.Attributes("onclick") = "OpenSpecialForm(" & TakeOffRow("TakeoffProductID") & "," & stateRow("VendorID") & "," & Core.Escape(Core.GetString(TakeOffRow("Product"))) & ",'" & FormatCurrency(s(0)("VendorPrice")) & "');"
                            End If
                        Else
                            'if not priced

                            td.InnerHtml = "N/A"
                            If Not IsDBNull(stateRow("VendorProductPriceRequestID")) Then
                                td.Attributes("class") = "nonepend"
                                aFullyPriced(idx) = False
                            Else
                                aCanRequest(idx) = True
                                Select Case Core.GetInt(stateRow("State"))
                                    Case ProductState.Accepted
                                        td.Attributes("class") = "noneaccpt"
                                    Case ProductState.Omit
                                        td.Attributes("class") = "noneomit"
                                    Case Else
                                        td.Attributes("class") = "none"
                                        aFullyPriced(idx) = False
                                End Select
                            End If
                            If Not IsReadOnly Then
                                td.Attributes("onclick") = "OpenSpecialForm(" & TakeOffRow("TakeoffProductID") & "," & stateRow("VendorID") & "," & Core.Escape(Core.GetString(TakeOffRow("Product"))) & ",'N/A');"
                            End If
                        End If
                    End If

                    dbVendorProductPrice.SpecialOrderProductID = TakeOffRow("SpecialOrderProductID")
                Else
                    'Non-Special Products

                    Dim p As DataRow() = dtPriced.Select("VendorID=" & stateRow("VendorID") & " and TakeoffProductID=" & TakeOffRow("TakeoffProductID"))
                    If p.Length > 0 Then
                        'Priced Products

                        Dim extendedPrice As Double = p(0)("VendorPrice") * dbVendorProductPrice.Quantity
                        td.InnerHtml = FormatCurrency(extendedPrice)
                        td.Attributes("class") = "norm"
                        aPrices.Add(extendedPrice)
                        If lowPrice = -1 OrElse extendedPrice < lowPrice Then
                            lowPrice = extendedPrice
                        End If
                        If highPrice = -1 OrElse extendedPrice > highPrice Then
                            highPrice = extendedPrice
                        End If

                        aTempTotals(idx) = extendedPrice

                        If aPricedCounts(idx) = Nothing Then
                            aPricedCounts(idx) = 1
                        Else
                            aPricedCounts(idx) += 1
                        End If

                        dbVendorProductPrice.ProductID = p(0)("ProductID")
                        dbVendorProductPrice.UnitPrice = p(0)("VendorPrice")
                        dbVendorProductPrice.Total = dbVendorProductPrice.Quantity * dbVendorProductPrice.UnitPrice
                    Else
                        'Substituted Products

                        Dim s As DataRow() = dtSubs.Select("VendorID=" & stateRow("VendorID") & " and TakeoffProductID=" & TakeOffRow("TakeoffProductID"))
                        If s.Length > 0 Then
                            If tblNumber < 1 Then
                                tblNumber = 1
                            End If
                            Dim extendedPrice As Double = s(0)("VendorPrice") * dbVendorProductPrice.Quantity
                            td.InnerHtml = FormatCurrency(extendedPrice)
                            If Not IsReadOnly Then
                                td.Attributes("onclick") = "OpenSubForm(" & TakeOffRow("TakeoffProductID") & "," & stateRow("VendorID") & "," & Core.Escape(s(0)("SubstituteProduct")) & "," & Core.Escape(Core.GetString(s(0)("VendorSku"))) & ",'" & FormatCurrency(Core.GetDouble(s(0)("VendorPrice"))) & "'," & s(0)("RecommendedQuantity") & ");"
                            End If
                            aPrices.Add(extendedPrice)
                            If lowPrice = -1 OrElse extendedPrice < lowPrice Then
                                lowPrice = extendedPrice
                            End If
                            If highPrice = -1 OrElse extendedPrice > highPrice Then
                                highPrice = extendedPrice
                            End If

                            If aPricedCounts(idx) = Nothing Then
                                aPricedCounts(idx) = 1
                            Else
                                aPricedCounts(idx) += 1
                            End If

                            Select Case Core.GetInt(stateRow("State"))
                                Case ProductState.Init
                                    td.Attributes("class") = "sub"
                                    aTempTotals(idx) = extendedPrice
                                    aFullyPriced(idx) = False
                                    aSubsIncomplete(idx) = True
                                Case ProductState.Accepted
                                    td.Attributes("class") = "subaccpt"
                                    aTempTotals(idx) = extendedPrice
                                Case ProductState.Omit
                                    td.Attributes("class") = "subreject"
                                    aTempTotals(idx) = 0
                            End Select

                            dbVendorProductPrice.RecommendedQuantity = Core.GetInt(s(0)("RecommendedQuantity"))
                            If dbVendorProductPrice.RecommendedQuantity = 0 Then
                                dbVendorProductPrice.RecommendedQuantity = dbVendorProductPrice.Quantity
                            End If
                            dbVendorProductPrice.SubstituteProductID = s(0)("SubProductID")
                            dbVendorProductPrice.UnitPrice = s(0)("VendorPrice")
                            dbVendorProductPrice.Total = dbVendorProductPrice.UnitPrice * dbVendorProductPrice.Quantity
                        Else
                            'Un-priced Products

                            Dim a As DataRow() = dtAvgs.Select("TakeoffProductID=" & TakeOffRow("TakeoffProductID"))
                            If a.Length > 0 Then
                                If tblNumber < 2 Then
                                    tblNumber = 2
                                End If
                                If Core.GetDouble(a(0)("AvgPrice")) > 0 Then
                                    'LLC Average Price available

                                    Dim extendedPrice As Double = a(0)("AvgPrice") * dbVendorProductPrice.Quantity
                                    td.InnerHtml = FormatCurrency(extendedPrice)
                                    aPrices.Add(extendedPrice)
                                    If lowPrice = -1 OrElse extendedPrice < lowPrice Then
                                        lowPrice = extendedPrice
                                    End If
                                    If highPrice = -1 OrElse extendedPrice > highPrice Then
                                        highPrice = extendedPrice
                                    End If

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
                                td.Attributes("class") = "none"
                                If Not IsDBNull(stateRow("VendorProductPriceRequestID")) Then
                                    td.Attributes("class") = "nonepend"
                                    aFullyPriced(idx) = False
                                Else
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
                                            td.Attributes("class") = "none"
                                            aFullyPriced(idx) = False
                                    End Select
                                End If
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
            For i As Integer = 0 To aPrices.Count - 1
                If tr.Cells(i + 3).Attributes("class") Is Nothing OrElse Not tr.Cells(i + 3).Attributes("class").Contains("omit") Then
                    If lowPrice > 0 And lowPrice = aPrices(i) Then
                        tr.Cells(i + 3).Attributes("class") = IIf(tr.Cells(i + 3).Attributes("class") = "norm", "low", tr.Cells(i + 3).Attributes("class") & "low")
                    ElseIf aPrices(i) > 0 Then
                        Dim pct As Double = Math.Round(100 * (aPrices(i) - lowPrice) / lowPrice)
                        tr.Cells(i + 3).InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                    End If
                    Try
                        If highPrice > 0 And highPrice = aPrices(i) And Not tr.Cells(i + 3).Attributes("class").Contains("low") Then
                            tr.Cells(i + 3).Attributes("class") = IIf(tr.Cells(i + 3).Attributes("class") = "norm", "high", tr.Cells(i + 3).Attributes("class") & "high")
                        End If
                    Catch ex As Exception

                    End Try
                End If
            Next

            Dim TotalsArray As Double()
            Select Case tblNumber
                Case 0
                    tblPriced.Rows.Add(tr)
                    TotalsArray = aPricedTotals
                Case 1
                    tblSub.Rows.Add(tr)
                    TotalsArray = aSubTotals
                Case 2
                    tblAvg.Rows.Add(tr)
                    TotalsArray = aAvgTotals
                Case 3
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

        td = New HtmlTableCell()
        td.InnerHtml = "Number of Prices Compared"
        trComparedRow.Cells.Add(td)

        td = New HtmlTableCell
        td.InnerHtml = "Subtotal: Products All Vendors Priced"
        td.Attributes("class") = "subt"
        td.ColSpan = 3
        trPricedSubtotalTop.Cells.Add(td)

        td = New HtmlTableCell()
        td.InnerHtml = "Subtotal: Products All Vendors Priced"
        trPricedSubtotal.Cells.Add(td)

        td = New HtmlTableCell()
        td.InnerHtml = "Subtotal: Priced Substitutions Included"
        td.Attributes("class") = "subt"
        td.ColSpan = 3
        trSubSubtotalTop.Cells.Add(td)

        td = New HtmlTableCell()
        td.InnerHtml = "Subtotal: Priced Substitutions Included"
        trSubSubtotal.Cells.Add(td)

        td = New HtmlTableCell()
        td.InnerHtml = "Subtotal: Averages Used if Pricing Missing"
        trAvgSubtotal.Cells.Add(td)

        td = New HtmlTableCell()
        td.InnerHtml = "Subtotal: Averages Used if Pricing Missing"
        td.Attributes("class") = "subt"
        td.ColSpan = 3
        trAvgSubtotalTop.Cells.Add(td)

        td = New HtmlTableCell
        td.InnerHtml = "Subtotal: Special Order Products"
        trSpecSubtotal.Cells.Add(td)

        td = New HtmlTableCell
        td.InnerHtml = "Subtotal: Special Order Products"
        td.Attributes("class") = "subt"
        td.ColSpan = 3
        trSpecSubtotalTop.Cells.Add(td)

        td = New HtmlTableCell
        td.InnerHtml = "Total Price"
        trTotalTotal.Cells.Add(td)

        td = New HtmlTableCell
        td.InnerHtml = "&nbsp;"
        trBtnRow.Cells.Add(td)

        Dim minPriced As Double = Aggregate t As Double In aPricedTotals.DefaultIfEmpty Into Min()
        Dim maxPriced As Double = Aggregate t As Double In aPricedTotals.DefaultIfEmpty Into Max()
        Dim minSub As Double = Aggregate t As Double In aSubTotals.DefaultIfEmpty Into Min()
        Dim maxSub As Double = Aggregate t As Double In aSubTotals.DefaultIfEmpty Into Max()
        Dim minAvg As Double = Aggregate t As Double In aAvgTotals.DefaultIfEmpty Into Min()
        Dim maxAvg As Double = Aggregate t As Double In aAvgTotals.DefaultIfEmpty Into Max()
        Dim minSpec As Double = Aggregate t As Double In aSpecTotals.DefaultIfEmpty Into Min()
        Dim maxSpec As Double = Aggregate t As Double In aSpecTotals.DefaultIfEmpty Into Max()

        Dim totals(dtVendors.Rows.Count - 1) As Double

        For i As Integer = 0 To aPricedCounts.Count - 1
            Dim totalTotal As Double = 0
            td = New HtmlTableCell
            td.InnerHtml = aPricedCounts(i) & "/" & productCount
            trComparedRow.Cells.Add(td)

            'priced products
            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aPricedTotals(i))
            If minPriced > 0 And aPricedTotals(i) = minPriced Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aPricedTotals(i) - minPriced) / minPriced * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxPriced > 0 AndAlso aPricedTotals(i) = maxPriced Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
            trPricedSubtotal.Cells.Add(td)

            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aPricedTotals(i))
            If minPriced > 0 And aPricedTotals(i) = minPriced Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aPricedTotals(i) - minPriced) / minPriced * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxPriced > 0 AndAlso aPricedTotals(i) = maxPriced Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
                trPricedSubtotalTop.Cells.Add(td)

            'totalTotal += aPricedTotals(i)
            totals(i) += aPricedTotals(i)

                'substitutes
            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aSubTotals(i))
            If minSub > 0 And aSubTotals(i) = minSub Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aSubTotals(i) - minSub) / minSub * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxSub > 0 AndAlso aSubTotals(i) = maxSub Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
            trSubSubtotal.Cells.Add(td)

            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aSubTotals(i))
            If minSub > 0 And aSubTotals(i) = minSub Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aSubTotals(i) - minSub) / minSub * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxSub > 0 AndAlso aSubTotals(i) = maxSub Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
            trSubSubtotalTop.Cells.Add(td)

            'totalTotal += aSubTotals(i)
            totals(i) += aSubTotals(i)

            'averages
            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aAvgTotals(i))
            If minAvg > 0 And aAvgTotals(i) = minAvg Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aAvgTotals(i) - minAvg) / minAvg * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxAvg > 0 AndAlso aAvgTotals(i) = maxAvg Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
            trAvgSubtotal.Cells.Add(td)

            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aAvgTotals(i))
            If minAvg > 0 And aAvgTotals(i) = minAvg Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aAvgTotals(i) - minAvg) / minAvg * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxAvg > 0 AndAlso aAvgTotals(i) = maxAvg Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
            trAvgSubtotalTop.Cells.Add(td)

            'totalTotal += aAvgTotals(i)
            totals(i) += aAvgTotals(i)

            'special order
            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aSpecTotals(i))
            If minSpec > 0 And aSpecTotals(i) = minSpec Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aSpecTotals(i) - minSpec) / minSpec * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxSpec > 0 AndAlso aSpecTotals(i) = maxSpec Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
            trSpecSubtotal.Cells.Add(td)

            td = New HtmlTableCell
            td.InnerHtml = FormatCurrency(aSpecTotals(i))
            If minSpec > 0 And aSpecTotals(i) = minSpec Then
                td.Attributes("class") = "low"
            Else
                Dim pct As Double = Math.Round((aSpecTotals(i) - minSpec) / minSpec * 100)
                If Not Double.IsNaN(pct) And Not Double.IsInfinity(pct) Then
                    td.InnerHtml &= "<br/><span class=""smaller"">" & pct & "% from low</span>"
                End If
                If maxSpec > 0 AndAlso aSpecTotals(i) = maxSpec Then
                    td.Attributes("class") = "high"
                Else
                    td.Attributes("class") = "norm"
                End If
            End If
            trSpecSubtotalTop.Cells.Add(td)

            'totalTotal += aSpecTotals(i)
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

        If tblPriced.Visible Then
            out.WriteLine("Products all vendors priced:")
            For i As Integer = 2 To tblPriced.Rows.Count - 2
                out.Write(Core.QuoteCSV(tblPriced.Rows(i).Cells(0).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblPriced.Rows(i).Cells(1).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblPriced.Rows(i).Cells(2).InnerHtml))
                For j As Integer = 3 To tblPriced.Rows(i).Cells.Count - 1
                    If i = 2 Then
                        out.Write("," & Core.QuoteCSV(tblPriced.Rows(i).Cells(j).InnerHtml))
                    Else
                        out.Write("," & Core.QuoteCSV(priceRE.Match(tblPriced.Rows(i).Cells(j).InnerHtml).Value))
                    End If
                Next
                out.WriteLine()
            Next
            out.WriteLine()
        End If

        If tblSub.Visible Then
            out.WriteLine("Products for which some vendors provided substitutes:")
            For i As Integer = 2 To tblSub.Rows.Count - 2
                out.Write(Core.QuoteCSV(tblSub.Rows(i).Cells(0).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblSub.Rows(i).Cells(1).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblSub.Rows(i).Cells(2).InnerHtml))
                For j As Integer = 3 To tblSub.Rows(i).Cells.Count - 1
                    If i = 2 Then
                        out.Write("," & Core.QuoteCSV(tblSub.Rows(i).Cells(j).InnerHtml))
                    Else
                        out.Write("," & Core.QuoteCSV(priceRE.Match(tblSub.Rows(i).Cells(j).InnerHtml).Value))
                    End If

                Next
                out.WriteLine()
            Next
            out.WriteLine()
        End If

        If tblAvg.Visible Then
            out.WriteLine("Products some vendors did not price:")
            For i As Integer = 2 To tblAvg.Rows.Count - 2
                out.Write(Core.QuoteCSV(tblAvg.Rows(i).Cells(0).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblAvg.Rows(i).Cells(1).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblAvg.Rows(i).Cells(2).InnerHtml))
                For j As Integer = 3 To tblAvg.Rows(i).Cells.Count - 1
                    If i = 2 Then
                        out.Write("," & Core.QuoteCSV(tblAvg.Rows(i).Cells(j).InnerHtml))
                    Else
                        out.Write("," & Core.QuoteCSV(priceRE.Match(tblAvg.Rows(i).Cells(j).InnerHtml).Value))
                    End If
                Next
                out.WriteLine()
            Next
            out.WriteLine()
        End If

        If tblSpec.Visible Then
            out.WriteLine("Special Order Products:")
            For i As Integer = 2 To tblSpec.Rows.Count - 2
                out.Write(Core.QuoteCSV(tblSpec.Rows(i).Cells(0).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblSpec.Rows(i).Cells(1).InnerHtml))
                out.Write("," & Core.QuoteCSV(tblSpec.Rows(i).Cells(2).InnerHtml))
                For j As Integer = 3 To tblSpec.Rows(i).Cells.Count - 1
                    If i = 2 Then
                        out.Write("," & Core.QuoteCSV(tblSpec.Rows(i).Cells(j).InnerHtml))
                    Else
                        out.Write("," & Core.QuoteCSV(priceRE.Match(tblSpec.Rows(i).Cells(j).InnerHtml).Value))
                    End If
                Next
                out.WriteLine()
            Next
            out.WriteLine()
        End If

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
    End Sub

    Protected Sub lsVendors_SelectedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lsVendors.SelectedChanged
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

                    msg.Append("Builder " & dbBuilder.CompanyName & " is requesting pricing for the following item:" & vbCrLf & vbCrLf)
                    If dbTakeoffProduct.ProductID <> Nothing Then
                        Dim dbProduct As ProductRow = ProductRow.GetRow(DB, dbTakeoffProduct.ProductID)
                        msg.AppendLine(dbProduct.Product)
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


    Public Sub RequestPricing(ByVal PriceComparisonId As Integer, ByVal VendorId As Integer)
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "MissingPrice")
        'Dim dbRecip As New automatic
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))
        Dim msg As New StringBuilder

        msg.Append("Builder " & dbBuilder.CompanyName & " is requesting pricing for the following items:" & vbCrLf & vbCrLf)

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

                msg.Append(row("Product"))
                If Not IsDBNull(row("SpecialOrderProductId")) Then
                    msg.Append("(Special Order)")
                End If
                'msg.Append(vbTab & row("Description"))
                msg.AppendLine()
            End If
        Next
        msg.AppendLine()
        'msg.AppendLine(dbMsg.Message)

        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        Dim FromEmail As String = SysParam.GetValue(DB, "ContactUsEmail")

        Dim sMsg As String = msg.ToString

        'FOR TESTING
        If SysParam.GetValue(DB, "TestMode") = True Then
            dbVendor.Email = FromEmail
        End If


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

        RequestPricing(PriceComparisonID, VendorID)

        BuildComparison()
    End Sub

    Protected Sub btnOrder_Command(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOrder.Postback
        Dim VendorID As Integer = Request("__EVENTARGUMENT")

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

    Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
        BuildComparison()
        BuildExportFromComparison()
    End Sub
End Class
