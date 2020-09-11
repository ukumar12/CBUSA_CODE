Option Strict Off

Imports System.Drawing
Imports System.Security.Cryptography

Imports Components
Imports DataLayer
Imports InfoSoftGlobal
Imports System.Configuration.ConfigurationManager

Partial Class MyMoneyOld
    Inherits ModuleControl

    Protected StartDate As DateTime
    Protected EndDate As DateTime
    Protected FlashString As String
    Dim ColorArray As ArrayList = New ArrayList()



    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If IsAdminDisplay Or DesignMode Then
            Exit Sub
        End If

        If Not CType(Me.Page, SitePage).IsLoggedInVendor Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
        If dbVendor.EnableMarketShare = False Then
            pnlMain.Visible = False
            pnlAccessDenied.Visible = True
            Exit Sub
        Else
            pnlMain.Visible = True
            pnlAccessDenied.Visible = False
        End If

        Dim SQL As String = String.Empty
        Dim i As Integer = 1


        If Not IsPostBack Then

            For i = 1 To 4
                Me.drpStartQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
                Me.drpEndQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
            Next

            Dim StartYear As Integer = 2008
            Dim iCount As Integer = 0
            While StartYear <= Now.Year + 1
                Me.drpStartYear.Items.Insert(iCount, (New ListItem(StartYear, StartYear)))
                Me.drpEndYear.Items.Insert(iCount, (New ListItem(StartYear, StartYear)))
                StartYear += 1
                iCount += 1
            End While



            Me.drpStartQuarter.SelectedValue = ((Now.AddMonths(-3).Month - 1) \ 3) + 1
            Me.drpEndQuarter.SelectedValue = ((Now.AddMonths(3).Month - 1) \ 3) + 1

            Me.drpStartYear.SelectedValue = Now.AddMonths(-3).Year
            Me.drpEndYear.SelectedValue = Now.AddMonths(3).Year

            lsSupplyPhases.DataSource = VendorCategoryRow.GetList(DB, "SortOrder")
            lsSupplyPhases.DataTextField = "Category"
            lsSupplyPhases.DataValueField = "VendorCategoryID"
            lsSupplyPhases.DataBind()
            lsSupplyPhases.SelectedValues = VendorRow.GetRow(DB, Session("VendorId")).GetSelectedVendorCategories

            LoadVendors()
        End If

    End Sub
    Protected Sub LoadVendors()
        If Not Session("VendorId") = Nothing Then
            Dim SQL As String
            SQL = "select VendorId,HistoricID, CompanyName from Vendor where IsActive = 1 And EnableMarketShare = 1 And VendorId <> " & Session("VendorId")
            If Not lsSupplyPhases.SelectedValues = String.Empty Then
                SQL &= " and VendorId in (select VendorID from VendorCategoryVendor where VendorCategoryId in (" & lsSupplyPhases.SelectedValues & "))"
            End If
            SQL &= " and VendorId in (select VendorId from LLCVendor where LLCID =" & VendorRow.GetLLCList(DB, Session("VendorId")) & " ) Order By CompanyName"
            lsVendors.DataSource = DB.GetDataTable(SQL).DefaultView
            lsVendors.DataTextField = "CompanyName"
            lsVendors.DataValueField = "HistoricID"
            lsVendors.DataBind()
        End If
    End Sub

    Protected Sub btnGenerateChart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGenerateChart.Click
        Dim FlashString As String = GetChartFlashString()
        ltlChart.Text = "<object classid=""clsid:d27cdb6e-ae6d-11cf-96b8-444553540000"" codebase=""http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0"" width=""700"" height=""400"" name=""PieChart"">" & _
                            "<param name=""allowScriptAccess"" value=""always"" />" & _
                            "<param name=""movie"" value=""/FusionCharts/FCF_Pie3D.swf"" />" & _
                            "<param name=""FlashVars"" value=""&chartWidth=700&chartHeight=400&debugMode=0&dataXML=" & FlashString & """ />" & _
                            "<param name=""quality"" value=""high"" />" & _
                            "<param name=""wmode"" value=""transparent"" />" & _
                            "<embed src=""/FusionCharts/FCF_Pie3D.swf"" flashVars=""&chartWidth=700&chartHeight=400&debugMode=0&dataXML=" & FlashString & """ wmode=""transparent"" quality=""high"" width=""700"" height=""400"" name=""PieChart"" type=""application/x-shockwave-flash"" pluginspage=""http://www.macromedia.com/go/getflashplayer"" />" & _
                        "</object>"

    End Sub

    Public Sub InitializeColorArray()
        ColorArray.Add("AFD8F8")
        ColorArray.Add("F6BD0F")
        ColorArray.Add("8BBA00")
        ColorArray.Add("FF8E46")
        ColorArray.Add("008E8E")
        ColorArray.Add("D64646")
        ColorArray.Add("8E468E")
        ColorArray.Add("588526")
        ColorArray.Add("B3AA00")
        ColorArray.Add("008ED6")
        ColorArray.Add("9D080D")
        ColorArray.Add("A186BE")
        ColorArray.Add("CC6600")
        ColorArray.Add("FDC689")
        ColorArray.Add("ABA000")
        ColorArray.Add("F26D7D")
        ColorArray.Add("FFF200")
        ColorArray.Add("0054A6")
        ColorArray.Add("F7941C")
        ColorArray.Add("CC3300")
        ColorArray.Add("006600")
        ColorArray.Add("663300")
        ColorArray.Add("6DCFF6")
    End Sub




    Protected Function GetChartFlashString() As String
        If IsAdminDisplay Then
            Return String.Empty
        End If
        Dim XML As String = String.Empty
        Dim SQL As String = String.Empty
        'Dim dt As DataTable

        If Me.drpEndYear.SelectedValue < Me.drpStartYear.SelectedValue Then
            Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue
            Me.drpEndQuarter.SelectedValue = Me.drpStartQuarter.SelectedValue
        ElseIf Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue And Me.drpEndQuarter.SelectedValue < Me.drpStartQuarter.SelectedValue Then
            Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue
            Me.drpEndQuarter.SelectedValue = Me.drpStartQuarter.SelectedValue
        End If


        Dim arDB As New Database()
        arDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))
        Try

            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
            SQL = "select sum(PURCHVOLCUR+ PURCHVOLADJ) as PurchaseVolume from cbusa_BuilderRebates where VNDRID=" & dbVendor.HistoricID _
                & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"


            Dim BuilderPurchases As Double = Core.GetDouble(arDB.ExecuteScalar(SQL))

            SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID <> " & DB.Number(dbVendor.HistoricID) _
                & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"

            Dim NonBuilderPurchases As Double = Core.GetDouble(arDB.ExecuteScalar(SQL))
            Dim dtOtherVendors As DataTable = Nothing

            If Not lsVendors.SelectedValues = Nothing Then
                SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID <> " & DB.Number(dbVendor.HistoricID) & " and VNDRID not IN(" & lsVendors.SelectedValues & ") " _
                & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"
                NonBuilderPurchases = Core.GetDouble(arDB.ExecuteScalar(SQL))

                SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) as Sum, VNDRID as HistoricId from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID in(" & DB.Number(dbVendor.HistoricID) & "," & lsVendors.SelectedValues & ") " _
                & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))" _
                & " GROUP BY VNDRID ORDER BY Sum DESC"
                dtOtherVendors = arDB.GetDataTable(SQL)
            End If


            'build the xml to drive the pie chart
            If BuilderPurchases = 0 And NonBuilderPurchases = 0 Then
                XML = XML & "<graph caption='Vendor Comparison' decimalPrecision='0' showPercentageValues='0' showNames='0' numberPrefix='$' showValues='0' showPercentageInLabel='0' pieYScale='60' pieBorderAlpha='40' pieFillAlpha='70' pieSliceDepth='20' pieRadius='200'>"
                XML = XML & "</graph>"
            ElseIf Not dtOtherVendors Is Nothing Then
                If dtOtherVendors.Rows.Count > 0 Then
                    Dim dbVendorRow As VendorRow = Nothing
                    XML = XML & "<graph caption='Vendor Comparison' decimalPrecision='1' showPercentageValues='1' showNames='1' numberPrefix='$' showValues='1' showPercentageInLabel='0' pieYScale='60' pieBorderAlpha='60' pieFillAlpha='70' pieSliceDepth='30' pieRadius='200'>"
                    'XML = XML & "<set name='Other LLC Vendors' value='" & NonBuilderPurchases & "' color='AFD8F8' />"
                    Dim colorCounter As Integer = 1
                    Dim drIndex As Integer = 1

                    InitializeColorArray()
                    'XML = XML & "<set name='My Vendor' value='" & BuilderPurchases & "' color='" & ColorArray(0) & "' />"
                    For Each dr As DataRow In dtOtherVendors.Rows
                        If colorCounter >= ColorArray.Count Then
                            colorCounter = 0
                        End If
                        dbVendorRow = VendorRow.GetRowByHistoricId(DB, dr("HistoricId"))
                        XML = XML & "<set name='" & drIndex & "' value='" & Core.GetDouble(dr("Sum")) & "' color='" & ColorArray(colorCounter) & "' hoverText='" & dbVendorRow.CompanyName & "' />"
                        Dim tr As TableRow = New TableRow
                        Dim td As TableCell = New TableCell

                        td.Text = drIndex & "."
                        drIndex = drIndex + 1
                        tr.Cells.Add(td)

                        td = New TableCell
                        td.Height = 10
                        td.Width = 15
                        td.VerticalAlign = VerticalAlign.Middle
                        td.Text = "<div style=""background-color:#" & ColorArray(colorCounter) & ";"" class=""gridColorDIV"">&nbsp;</div>"
                        tr.Cells.Add(td)

                        td = New TableCell
                        td.HorizontalAlign = HorizontalAlign.Left
                        td.Text = dbVendorRow.CompanyName
                        tr.Cells.Add(td)

                        td = New TableCell
                        td.HorizontalAlign = HorizontalAlign.Right
                        td.Text = FormatCurrency(Core.GetDouble(dr("Sum")), 2, TriState.UseDefault, TriState.True, TriState.True)
                        tr.Cells.Add(td)

                        tblChartLegend.Rows.Add(tr)

                        colorCounter = colorCounter + 1
                    Next

                    XML = XML & "</graph>"
                End If
            Else
                XML = XML & "<graph caption='Vendor Comparison' decimalPrecision='1' showPercentageValues='1' showNames='1' numberPrefix='$' showValues='1' showPercentageInLabel='1' pieYScale='60' pieBorderAlpha='40' pieFillAlpha='70' pieSliceDepth='30' pieRadius='150'>"
                XML = XML & "<set name='My Vendor' value='" & BuilderPurchases & "' color='AFD8F8' />"
                XML = XML & "<set name='Other LLC Vendors' value='" & NonBuilderPurchases & "' color='F6BD0F' />"
                XML = XML & "</graph>"
            End If

            'create the chart
            Return Server.UrlEncode(XML)

        Finally
            If arDB IsNot Nothing AndAlso arDB.IsOpen Then arDB.Close()
        End Try
    End Function

    Protected Sub lsSupplyPhases_SelectedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lsSupplyPhases.SelectedChanged
        LoadVendors()
    End Sub

    Protected Sub btnOptOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOptOut.Click
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
        dbVendor.EnableMarketShare = False
        dbVendor.Update()
        Response.Redirect("/vendor/vendorcharts.aspx")
    End Sub
End Class
