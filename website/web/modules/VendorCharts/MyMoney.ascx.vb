Option Strict Off

Imports System.Drawing
Imports System.Security.Cryptography

Imports Components
Imports DataLayer
Imports InfoSoftGlobal
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Collections.Generic

Partial Class MyMoney
    Inherits ModuleControl
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
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

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

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
            'lsSupplyPhases.SelectedValues = VendorRow.GetRow(DB, Session("VendorId")).GetSelectedVendorCategories

            LoadVendors()

         Core.DataLog("Market Share Report", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)
        End If

    End Sub
    Protected Sub LoadVendors()
        If Not Session("VendorId") = Nothing Then
            Dim SQL As String
            SQL = "select VendorId,HistoricID, CompanyName from Vendor where IsActive = 1 And EnableMarketShare = 1 And VendorId <> " & Session("VendorId")
            If Not lsSupplyPhases.SelectedValues = String.Empty Then
                SQL &= " and VendorId in (select VendorID from VendorCategoryVendor where VendorCategoryId in (" & lsSupplyPhases.SelectedValues & "))"
            Else
                SQL &= " And 1=2 "
            End If
            SQL &= " and VendorId in (select VendorId from LLCVendor where LLCID In (" & VendorRow.GetLLCList(DB, Session("VendorId")) & " )) Order By CompanyName"
            Dim dt As DataTable = DB.GetDataTable(SQL)

            ltlVendors.Text = ""

            If dt.Rows.Count > 1 Then
                rptVendors.DataSource = dt
                'lsVendors.DataTextField = "CompanyName"
                'lsVendors.DataValueField = "HistoricID"
            ElseIf dt.Rows.Count = 1 Then
                ltlVendors.Text = "There is only one vendor in the selected supply phase(s) and therefore that vendor's name is left anonymous."
            ElseIf Not lsSupplyPhases.SelectedValues = String.Empty Then
                ltlVendors.Text = "There are no vendors in the selected supply phase(s). Please select more supply phases or press the ""Generate Chart"" button to see your number compared to all other vendors in your LLC."
            Else
                ltlVendors.Text = "Please select a supply phase or press the ""Generate Chart"" button to see your number compared to all other vendors in your LLC."
            End If

            rptVendors.DataBind()
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


        Dim dtMarketShareReport As DataTable
        Dim qVendorInvoices As IEnumerable
        Dim VendorsSql As New System.Text.StringBuilder
        VendorsSql.Append("SELECT " & vbCrLf)
        VendorsSql.Append("  v.VendorID, " & vbCrLf)
        VendorsSql.Append("  v.HistoricID, " & vbCrLf)
        VendorsSql.Append("  l.affiliateID, " & vbCrLf)
        VendorsSql.Append("  l.LLC, l.LLCID, " & vbCrLf)
        VendorsSql.Append("  v.EnableMarketShare " & vbCrLf)
        VendorsSql.Append("FROM Vendor v " & vbCrLf)
        VendorsSql.Append("INNER JOIN LLCVendor lv " & vbCrLf)
        VendorsSql.Append("  ON v.VendorID = lv.VendorID " & vbCrLf)
        VendorsSql.Append("INNER JOIN LLC l " & vbCrLf)
        VendorsSql.Append("  ON l.LLCID = lv.LLCID")

        Dim dtVendors As DataTable = DB.GetDataTable(VendorsSql.ToString)


        dtMarketShareReport = GetMarketShareReport("RG_MarketShareReport", drpStartYear.SelectedValue, drpEndYear.SelectedValue, drpStartQuarter.SelectedValue, drpEndQuarter.SelectedValue)
        dtMarketShareReport.DefaultView.RowFilter = " MarketID <> ''"
        dtMarketShareReport = dtMarketShareReport.DefaultView.ToTable





        qVendorInvoices = (From dr As DataRow In dtMarketShareReport.AsEnumerable Join dv As DataRow In dtVendors.AsEnumerable On Core.GetInt(dr("VendorID")) Equals Core.GetInt(dv("HistoricID")) _
                               Select New With { _
                                     .VendorID = Core.GetInt(dv("VendorID")), _
                                      .HistoricID = Core.GetInt(dv("HistoricID")), _
                                      .LLCID = Core.GetInt(dv("LLCID")), _
                                       .LLC = Core.GetString(dv("LLC")), _
                                      .CompanyName = dr("VendorName"), _
                                       .EnableMarketShare = Core.GetBoolean(dv("EnableMarketShare")), _
                                     .affiliateID = Core.GetInt(dv("affiliateID")), _
                                     .PurchaseVolume = Core.GetDouble(dr("PurchaseVolume")) _
                                } _
                               )

        Dim dt As DataTable = EQToDataTable(qVendorInvoices)

        'Dim arDB As New Database()
        'arDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))

        If dt.Rows.Count > 0 Then


            Try
                'Refer Ali's 11/9/2011 version for original incase of any issues.

                Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
                'SQL = "select sum(PURCHVOLCUR+ PURCHVOLADJ) as PurchaseVolume from cbusa_BuilderRebates where VNDRID=" & dbVendor.HistoricID _
                '    & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                '    & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"


                'Dim BuilderPurchases As Double = Core.GetDouble(arDB.ExecuteScalar(SQL))

                Dim BuilderPurchases As Double = Core.GetDouble(dt.Compute("SUM(PurchaseVolume)", "VendorID = " & dbVendor.VendorID))

                'SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID <> " & DB.Number(dbVendor.HistoricID) _
                '    & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                '    & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"




                'Dim NonBuilderPurchases As Double = Core.GetDouble(arDB.ExecuteScalar(SQL))

                Dim NonBuilderPurchases As Double = Core.GetDouble(dt.Compute("SUM(PurchaseVolume)", String.Format("LLCID  = {0} AND VendorID <> {1} ", dbVendor.LLCID, dbVendor.VendorID)))




                Dim dtOtherVendors As DataTable = Nothing
                Dim dtVendorList As DataTable = Nothing
                Dim VendorLists As String = String.Empty
                Dim conn As String = String.Empty

                If Not lsSupplyPhases.SelectedValues = String.Empty Then


                    SQL = "select HistoricId from Vendor where IsActive = 1 And EnableMarketShare = 1 And VendorId <> " & Session("VendorId")
                    'If Not lsSupplyPhases.SelectedValues = String.Empty Then
                    SQL &= " and VendorId in (select VendorID from VendorCategoryVendor where VendorCategoryId in (" & lsSupplyPhases.SelectedValues & "))"
                    'End If
                    SQL &= " and VendorId in (select VendorId from LLCVendor where LLCID In (" & VendorRow.GetLLCList(DB, Session("VendorId")) & " )) Order By CompanyName"

                    dtVendorList = DB.GetDataTable(SQL)

                    For Each dr As DataRow In dtVendorList.Rows
                        VendorLists &= conn & dr("HistoricID")
                        conn = ","
                    Next
                    If VendorLists <> String.Empty Then
                        'SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID <> " & DB.Number(dbVendor.HistoricID) & " and VNDRID not IN(" & VendorLists & ") " _
                        '& " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                        '& " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"
                        'NonBuilderPurchases = Core.GetDouble(arDB.ExecuteScalar(SQL))

                        NonBuilderPurchases = Core.GetDouble(dt.Compute("SUM(PurchaseVolume)", String.Format("LLCID  = {0} AND VendorID <> {1} AND HistoricID NOT IN (" & VendorLists & ") ", dbVendor.LLCID, dbVendor.VendorID)))



                        'SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) as Sum, VNDRID as HistoricId from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID in(" & DB.Number(dbVendor.HistoricID) & "," & VendorLists & ") " _
                        '& " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                        '& " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))" _
                        '& " GROUP BY VNDRID ORDER BY Sum DESC"
                        'dtOtherVendors = arDB.GetDataTable(SQL)
                        Dim filters As New List(Of String)
                        filters.Add("HistoricID IN  (" & VendorLists & "," & dbVendor.HistoricID & ")")
                        '  filters.Add("HistoricID IN " & DB.Quote(VendorLists))
                        dt.DefaultView.RowFilter = String.Join(" AND ", filters.ToArray)
                        dtOtherVendors = dt.DefaultView.ToTable
                    End If

                End If

                'If Not lsVendors.SelectedValues = Nothing Then
                '    SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID <> " & DB.Number(dbVendor.HistoricID) & " and VNDRID not IN(" & lsVendors.SelectedValues & ") " _
                '    & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                '    & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))"
                '    NonBuilderPurchases = Core.GetDouble(arDB.ExecuteScalar(SQL))

                '    SQL = "select sum(PURCHVOLCUR + PURCHVOLADJ) as Sum, VNDRID as HistoricId from cbusa_BuilderRebates where LLCID=(select top 1 STRING1 from VBCloud_Param where CODE=1 and LONGINT1=" & DB.Number(dbVendor.LLCID) & ") and VNDRID in(" & DB.Number(dbVendor.HistoricID) & "," & lsVendors.SelectedValues & ") " _
                '    & " and (BLDYEAR < " & drpEndYear.SelectedValue & " or (BLDYEAR = " & drpEndYear.SelectedValue & " and BLDPERIOD <=" & drpEndQuarter.SelectedValue & "))" _
                '    & " and (BLDYEAR > " & drpStartYear.SelectedValue & " or (BLDYEAR = " & drpStartYear.SelectedValue & " and BLDPERIOD >=" & drpStartQuarter.SelectedValue & "))" _
                '    & " GROUP BY VNDRID ORDER BY Sum DESC"
                '    dtOtherVendors = arDB.GetDataTable(SQL)
                'End If


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
                        Dim bIsMyVendor As Boolean = False

                        InitializeColorArray()
                        'XML = XML & "<set name='My Vendor' value='" & BuilderPurchases & "' color='" & ColorArray(0) & "' />"
                        For Each dr As DataRow In dtOtherVendors.Rows
                            If colorCounter >= ColorArray.Count Then
                                colorCounter = 0
                            End If

                            dbVendorRow = VendorRow.GetRowByHistoricId(DB, dr("HistoricId"))

                            If dbVendorRow.VendorID = Session("VendorId") Then
                                XML = XML & "<set name='" & drIndex & "' value='" & Core.GetDouble(dr("PurchaseVolume")) & "' color='" & ColorArray(colorCounter) & "' hoverText='" & dbVendorRow.CompanyName.Replace("'", "") & "' />"
                            Else
                                XML = XML & "<set name='" & drIndex & "' value='" & Core.GetDouble(dr("PurchaseVolume")) & "' color='" & ColorArray(colorCounter) & "' hoverText='Vendor " & drIndex & "' />"
                            End If


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
                            If dbVendorRow.VendorID = Session("VendorId") Then
                                td.Text = dbVendorRow.CompanyName
                            Else
                                td.Text = "Vendor " & dtOtherVendors.Rows.IndexOf(dr) + 1
                            End If

                            tr.Cells.Add(td)

                            td = New TableCell
                            td.HorizontalAlign = HorizontalAlign.Right
                            td.Text = FormatCurrency(Core.GetDouble(dr("PurchaseVolume")), 2, TriState.UseDefault, TriState.True, TriState.True)
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
            Catch ex As Exception
              
            End Try
        Else
            XML = "<graph caption='Vendor Comparison' decimalPrecision='0' showPercentageValues='0' showNames='0' numberPrefix='$' showValues='0' showPercentageInLabel='0' pieYScale='60' pieBorderAlpha='40' pieFillAlpha='70' pieSliceDepth='20' pieRadius='200'>"
            XML = XML & "</graph>"
            Return Server.UrlEncode(XML)
        End If
    End Function

    Protected Sub lsSupplyPhases_SelectedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lsSupplyPhases.SelectedChanged
        LoadVendors()
    End Sub

    Private Shared Function GetMarketShareReport(ByVal StoredProcedureName As String, ByVal StartYear As Integer, ByVal EndYear As Integer, ByVal StartQuarter As Integer, ByVal EndQuarter As Integer) As DataTable
        Dim ResDb As New Database
        Dim dt As New DataTable
        Dim prams(3) As SqlParameter
        prams(0) = New SqlParameter("@FromYear", StartYear)
        prams(1) = New SqlParameter("@ToYear", EndYear)
        prams(2) = New SqlParameter("@FromQuarter", StartQuarter)
        prams(3) = New SqlParameter("@ToQuarter", EndQuarter)

        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            ResDb.RunProc(StoredProcedureName, prams, dt)
            'Dim CacheKeySuffix As String = "_"
            'Dim CacheKey As String = "MarketShareReport" & StartYear & CacheKeySuffix & EndYear & CacheKeySuffix & StartQuarter & CacheKeySuffix & EndQuarter
            'System.Web.HttpContext.Current.Cache.Insert(CacheKey, dt, Nothing, Date.UtcNow.AddSeconds(600), TimeSpan.Zero)
            Return dt
        Catch ex As Exception
        Finally
            If ResDb IsNot Nothing AndAlso ResDb.IsOpen Then ResDb.Close()
        End Try
        Return Nothing
    End Function

    Public Function EQToDataTable(ByVal parIList As System.Collections.IEnumerable) As System.Data.DataTable
        Dim ret As New System.Data.DataTable()
        Try
            Dim ppi As System.Reflection.PropertyInfo() = Nothing
            If parIList Is Nothing Then Return ret
            For Each itm In parIList
                If ppi Is Nothing Then
                    ppi = DirectCast(itm.[GetType](), System.Type).GetProperties()
                    For Each pi As System.Reflection.PropertyInfo In ppi
                        Dim colType As System.Type = pi.PropertyType
                        If (colType.IsGenericType) AndAlso (colType.GetGenericTypeDefinition() Is GetType(System.Nullable(Of ))) Then colType = colType.GetGenericArguments()(0)
                        ret.Columns.Add(New System.Data.DataColumn(pi.Name, colType))
                    Next
                End If
                Dim dr As System.Data.DataRow = ret.NewRow
                For Each pi As System.Reflection.PropertyInfo In ppi
                    dr(pi.Name) = If(pi.GetValue(itm, Nothing) Is Nothing, DBNull.Value, pi.GetValue(itm, Nothing))
                Next
                ret.Rows.Add(dr)
            Next
            For Each c As System.Data.DataColumn In ret.Columns
                c.ColumnName = c.ColumnName.Replace("_", " ")
            Next
        Catch ex As Exception
            ret = New System.Data.DataTable()
        End Try
        Return ret
    End Function
    'Protected Sub btnOptOut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOptOut.Click
    '    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
    '    dbVendor.EnableMarketShare = False
    '    dbVendor.Update()
    '    Response.Redirect("/vendor/vendorcharts.aspx")
    'End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
        'Response.Redirect("/vendor/default.aspx")
    'End Sub
End Class
