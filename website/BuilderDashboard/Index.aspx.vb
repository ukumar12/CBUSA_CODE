Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Net.Mail
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration.ConfigurationManager
Imports Components

Partial Class Index
    Inherits System.Web.UI.Page

    Private Enum SHOW_IN_DASHBOARD_CATEGORIES
        DASHBOARD_CATEGORY_DRYWALL = 9
        DASHBOARD_CATEGORY_FLOORING = 19
        DASHBOARD_CATEGORY_GARAGE_DOOR = 21
        DASHBOARD_CATEGORY_HVAC = 26
        DASHBOARD_CATEGORY_INSULATION = 27
        DASHBOARD_CATEGORY_KITCHEN_AND_BATH = 29
        DASHBOARD_CATEGORY_LIGHTING = 31
        DASHBOARD_CATEGORY_ROOFING = 37
        DASHBOARD_CATEGORY_SUPPLY_HOUSE = 45
    End Enum

    Private cReportingQuarter As Int32
    Private cReportingYear As Int32
    Private cBuilderID As Int32

    Private cStrConn As String

    Private Sub Index_Load(sender As Object, e As EventArgs) Handles Me.Load

        cBuilderID = CInt(Request.QueryString("BuilderId"))
        cStrConn = DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), "", "")

        GetLastReportingQuarterYear()
        PopulateKPIs()
        PopulateBuilderPurchaseVolumeHistory()
        PopulateVendorUtilizationSection()

        PopulateAllBuildersInLLC()
        PopulateVendorUseTable()

        If Not Page.IsPostBack Then
            PopulateVendorCategories()
            PopulateBuilderList()
        Else
            hdnPostBack.Value = "true"
            FilterBuilderAndCategory()
        End If

    End Sub

    Private Sub GetLastReportingQuarterYear()
        Dim CurrentQuarter As Integer = Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        Dim CurrentYear As Integer = DatePart(DateInterval.Year, Now)

        Dim LastQuarter As Integer = IIf(CurrentQuarter - 1 = 0, 4, CurrentQuarter - 1)
        Dim LastYear As Integer = IIf(LastQuarter = 4, CurrentYear - 1, CurrentYear)

        Dim LastQuarterEnd As DateTime = New Date(LastYear, (LastQuarter * 3), Date.DaysInMonth(LastYear, LastQuarter * 3)).Date

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlGetReportingDeadline As String = "sp_SysParamsReportingDeadlines"

        Dim sqlComm As New SqlCommand(sqlGetReportingDeadline, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@LastQtrEndDate", LastQuarterEnd)

        Dim FinalInvoiceDate As Date = Now.Date

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                FinalInvoiceDate = DR("FinalInvoiceDate")
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

        If DateDiff(DateInterval.Day, FinalInvoiceDate, Now.Date) > 0 Then
            cReportingQuarter = LastQuarter
            cReportingYear = LastYear
        Else
            cReportingYear = IIf(LastQuarter = 1, CurrentYear - 1, CurrentYear)
            cReportingQuarter = IIf(LastQuarter = 1, 4, LastQuarter - 1)
        End If

    End Sub

    Private Sub PopulateVendorCategories()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlVendorCategoryList As String = "SELECT DashboardCategoryID, DashboardCategory FROM VendorDashboardCategory ORDER BY DashboardCategory"

        Dim sqlComm As New SqlCommand(sqlVendorCategoryList, sqlConn)
        sqlComm.CommandType = CommandType.Text

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        mulselCategory.Items.Clear()

        Try
            If DR.HasRows Then
                While DR.Read()
                    Dim liCategory As New ListItem()
                    liCategory.Text = DR("DashboardCategory")
                    liCategory.Value = DR("DashboardCategoryID")

                    mulselCategory.Items.Add(liCategory)
                End While
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateBuilderList()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlBuilderList As String = "SELECT BuilderID, CompanyName FROM Builder WHERE LLCId IN (SELECT LLCId FROM Builder WHERE BuilderId = @BuilderId) AND IsActive = 1 ORDER BY CompanyName"

        Dim sqlComm As New SqlCommand(sqlBuilderList, sqlConn)
        sqlComm.CommandType = CommandType.Text
        sqlComm.Parameters.AddWithValue("@BuilderId", cBuilderID)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        mulselBuilder.Items.Clear()

        Try
            If DR.HasRows Then
                While DR.Read()
                    Dim liBuilder As New ListItem()
                    liBuilder.Text = DR("CompanyName")
                    liBuilder.Value = DR("BuilderId")

                    mulselBuilder.Items.Add(liBuilder)
                End While
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateKPIs()
        PopulateLastQuarterPurchasesKPI()
        PopulateFourQuarterPurchasesKPI()
        PopulatePurchaseVolumeRankKPI()
        PopulateVendorUsagePercentKPI()
        PopulateFourQuarterVendorUseKPI()
        PopulateVendorUsageRankKPI()
    End Sub

    Private Sub PopulateLastQuarterPurchasesKPI()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlLastQtrPurchasesIndicator As String = "sp_BuilderLastQtrPurchasesIndicator"

        Dim sqlComm As New SqlCommand(sqlLastQtrPurchasesIndicator, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                Dim LastQtrPurchases As Double = Math.Round(DR("ReportingQtrPurchaseTotal") / 1000, 0)
                Dim PrevQtrPurchases As Double = Math.Round(DR("PrevQtrPurchaseTotal") / 1000, 0)

                Dim Indicator As String = DR("Indicator")
                Dim UpDownPercent As Int32 = DR("UpDownPercent")

                lblReportingQtrTotalPurchase.Text = String.Concat("$", LastQtrPurchases.ToString(), "K")

                If DR("UpDownPercent") > 1000 Then
                    UpDownPercent = Math.Round((DR("UpDownPercent") / 1000), 0)
                    lblReportingQtrUpDownIndicator.Text = String.Concat((UpDownPercent).ToString(), "K", "%")
                Else
                    lblReportingQtrUpDownIndicator.Text = String.Concat(UpDownPercent.ToString(), "%")
                End If

                lblReportingQtrTotalPurchase.Text = String.Concat("$", LastQtrPurchases.ToString(), "K")
                lblReportingQtrYear.Text = String.Concat(cReportingYear, "Q", cReportingQuarter)

                If Indicator = "UP" Then
                    spnReportingQtrUpDownIndicator.InnerHtml = "<i class=""fa fa-caret-up""></i>"
                    spnReportingQtrProfitLoss.Attributes.Add("class", "profit")
                Else
                    spnReportingQtrUpDownIndicator.InnerHtml = "<i class=""fa fa-caret-down""></i>"
                    spnReportingQtrProfitLoss.Attributes.Add("class", "loss")
                End If

                'spnReportingQtrUpDownIndicator.InnerHtml = "<i class=""fa fa-caret-down""></i>"
                'spnReportingQtrProfitLoss.Attributes.Add("class", "loss")
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateFourQuarterPurchasesKPI()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlLast4QtrPurchasesIndicator As String = "sp_BuilderLast4QtrPurchasesIndicator"

        Dim sqlComm As New SqlCommand(sqlLast4QtrPurchasesIndicator, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                Dim Last4QtrPurchases As Double = Math.Round(DR("BuilderLast4QtrTotalPurchases") / 1000, 0)
                Dim Prev4QtrPurchases As Double = Math.Round(DR("BuilderPrev4QtrTotalPurchases") / 1000, 0)
                Dim Indicator As String = DR("Indicator")
                Dim UpDownPercent As Int16 = DR("UpDownPercent")

                lblFourQtrTotalPurchase.Text = String.Concat("$", Last4QtrPurchases.ToString(), "K")
                lblFourQtrUpDownIndicator.Text = String.Concat(UpDownPercent.ToString(), "%")

                If Indicator = "UP" Then
                    spnFourQtrUpDownIndicator.InnerHtml = "<i class=""fa fa-caret-up""></i>"
                    spnFourQtrProfitLoss.Attributes.Add("class", "profit")
                Else
                    spnFourQtrUpDownIndicator.InnerHtml = "<i class=""fa fa-caret-down""></i>"
                    spnFourQtrProfitLoss.Attributes.Add("class", "loss")
                End If
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulatePurchaseVolumeRankKPI()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlPurchaseVolumeRank As String = "sp_BuilderPurchaseVolumeRank"

        Dim sqlComm As New SqlCommand(sqlPurchaseVolumeRank, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                Dim PurchaseVolumeRank As Int16 = DR("PurchaseVolumeRank")
                Dim TotalRanks As Int16 = DR("RankChartMax")

                lblPurchaseVolumeRank.Text = String.Concat(PurchaseVolumeRank.ToString(), " of ", TotalRanks.ToString())
            Else
                lblPurchaseVolumeRank.Text = "N/A"
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateVendorUsagePercentKPI()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlVendorUsagePercent As String = "sp_BuilderQuarterVendorUsagePercent"

        Dim sqlComm As New SqlCommand(sqlVendorUsagePercent, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                lblVendorUsagePercentQtrYear.Text = String.Concat(cReportingYear.ToString(), " ", "Q", cReportingQuarter.ToString())

                Dim VendorUsagePercent As Int16 = DR("VendorUsagePercent")
                Dim Indicator As String = DR("Indicator")
                Dim UpDownPercent As Int32 = DR("UpDownPercent")

                lblVendorUsagePercent.Text = String.Concat(VendorUsagePercent.ToString(), "%")

                If DR("UpDownPercent") > 1000 Then
                    UpDownPercent = Math.Round((DR("UpDownPercent") / 1000), 0)
                    lblVendorUsagePercentUpDownIndicator.Text = String.Concat((UpDownPercent).ToString(), "K", "%")
                Else
                    lblVendorUsagePercentUpDownIndicator.Text = String.Concat(UpDownPercent.ToString(), "%")
                End If

                If Indicator = "UP" Then
                    spnVendorUsagePercentIndicator.InnerHtml = "<i class=""fa fa-caret-up""></i>"
                    spnVendorUsagePercentProfitLoss.Attributes.Add("class", "profit")
                Else
                    spnVendorUsagePercentIndicator.InnerHtml = "<i class=""fa fa-caret-down""></i>"
                    spnVendorUsagePercentProfitLoss.Attributes.Add("class", "loss")
                End If
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateFourQuarterVendorUseKPI()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sql4QtrVendorUse As String = "sp_4QuarterVendorUse"

        Dim sqlComm As New SqlCommand(sql4QtrVendorUse, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                Dim FourQtrVendorUse As Int16 = DR("FourQtrVendorUse")
                Dim Indicator As String = DR("Indicator")
                Dim UpDownPercent As Int32 = DR("UpDownPercent")

                lblFourQtrVendorUse.Text = FourQtrVendorUse.ToString()

                If DR("UpDownPercent") > 1000 Then
                    UpDownPercent = Math.Round((DR("UpDownPercent") / 1000), 0)
                    lblFourQtrVendorUsePercentUpDownIndicator.Text = String.Concat((UpDownPercent).ToString(), "K", "%")
                Else
                    lblFourQtrVendorUsePercentUpDownIndicator.Text = String.Concat(UpDownPercent.ToString(), "%")
                End If

                If Indicator = "UP" Then
                    spnFourQtrVendorUsePercentIndicator.InnerHtml = "<i class=""fa fa-caret-up""></i>"
                    spnFourQtrVendorUseProfitLoss.Attributes.Add("class", "profit")
                Else
                    spnFourQtrVendorUsePercentIndicator.InnerHtml = "<i class=""fa fa-caret-down""></i>"
                    spnFourQtrVendorUseProfitLoss.Attributes.Add("class", "loss")
                End If
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateVendorUsageRankKPI()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlVendorUsageRank As String = "sp_BuilderQuarterVendorUseRank"

        Dim sqlComm As New SqlCommand(sqlVendorUsageRank, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                Dim VendorUseRank As Double = DR("VendorUseRank")
                Dim TotalRanksInLLC As Double = DR("RankChartMax")

                lblVendorUseRank.Text = String.Concat(VendorUseRank.ToString(), " of ", TotalRanksInLLC.ToString())
            Else
                lblVendorUseRank.Text = "N/A"
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateBuilderPurchaseVolumeHistory()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlBuilderPurchaseVolumeHistory As String = "sp_BuilderPurchaseVolumeHistory"

        Dim sqlComm As New SqlCommand(sqlBuilderPurchaseVolumeHistory, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then

                Dim strQtrYears As String = String.Empty
                Dim strBuilderPurchases As String = String.Empty
                Dim strLLCAvgPurchases As String = String.Empty

                While DR.Read()
                    strQtrYears = String.Concat(strQtrYears, CStr(DR("QtrYear")).Trim(), ",")
                    strBuilderPurchases = String.Concat(strBuilderPurchases, IIf(IsDBNull(DR("QtrTotalPurchase")), "0", (DR("QtrTotalPurchase") / 1000).ToString()), ",")
                    strLLCAvgPurchases = String.Concat(strLLCAvgPurchases, IIf(IsDBNull(DR("QtrLLCAverage")), "0", (DR("QtrLLCAverage") / 1000).ToString()), ",")
                End While

                hdnQuarterYearValues.Value = strQtrYears.Substring(0, strQtrYears.Length - 1)
                hdnBuilderPurchases.Value = strBuilderPurchases.Substring(0, strBuilderPurchases.Length - 1)
                hdnLLCAvgPurchases.Value = strLLCAvgPurchases.Substring(0, strLLCAvgPurchases.Length - 1)

            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateVendorUtilizationSection()

        For Each iDashboardCategory As SHOW_IN_DASHBOARD_CATEGORIES In System.Enum.GetValues(GetType(SHOW_IN_DASHBOARD_CATEGORIES))
            PopulateCategoryTopVendors(iDashboardCategory)
            PopulateCategoryMarketSpend(iDashboardCategory)
        Next

    End Sub

    Private Sub PopulateCategoryTopVendors(ByVal DashboardCategory As SHOW_IN_DASHBOARD_CATEGORIES)

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlTopVendorsInLLCCategory As String = "sp_BuildersVendorInLLCCategoryQuarter"

        Dim sqlComm As New SqlCommand(sqlTopVendorsInLLCCategory, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)
        sqlComm.Parameters.AddWithValue("@DashboardCategoryID", DashboardCategory)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then

                Dim strTopVendorsULTag As String = ""

                While DR.Read()
                    Dim VendorName As String = DR("VendorName")
                    Dim TotalSalesAmount As Double = DR("TotalSalesAmount")

                    If TotalSalesAmount > 0 Then
                        strTopVendorsULTag = String.Concat(strTopVendorsULTag, "<li>", VendorName, " ($", TotalSalesAmount.ToString("N0"), ")", "</li>")
                    End If

                End While

                Select Case DashboardCategory
                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_DRYWALL
                        ulDrywallTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_FLOORING
                        ulFlooringTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_GARAGE_DOOR
                        ulGarageDoorsTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_HVAC
                        ulHVACTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_INSULATION
                        ulInsulationTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_KITCHEN_AND_BATH
                        ulKitchenBathTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_LIGHTING
                        ulLightingTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_ROOFING
                        ulRoofingTopVendors.InnerHtml = strTopVendorsULTag

                    Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_SUPPLY_HOUSE
                        ulSupplyHouseTopVendors.InnerHtml = strTopVendorsULTag
                End Select

            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateCategoryMarketSpend(ByVal DashboardCategory As SHOW_IN_DASHBOARD_CATEGORIES)

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlVendorUsageRank As String = "sp_VendorUtilizationForLLCCategoryQuarter"

        Dim sqlComm As New SqlCommand(sqlVendorUsageRank, sqlConn)

        sqlComm.CommandType = CommandType.StoredProcedure

        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)
        sqlComm.Parameters.AddWithValue("@DashboardCategoryID", DashboardCategory)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                DR.Read()

                Dim CategoryTotalInMarket As Double = DR("BuilderCategoryTotal")
                Dim MarketSharePercent As Double = DR("MarketSharePercent")
                Dim LLCCategoryVendorCount As Integer = DR("LLCCategoryVendorCount")

                If LLCCategoryVendorCount > 0 Then
                    Dim strMarketSharePercent As String
                    Dim strMarketCategoryTotal As String
                    Dim strDivCssClass As String = "quarter-block"
                    Dim boolShowDimmedImage As Boolean = False

                    If CategoryTotalInMarket > 0 Then
                        strMarketSharePercent = String.Concat(MarketSharePercent.ToString(), "% of")
                        strMarketCategoryTotal = String.Concat("$", CategoryTotalInMarket.ToString("N0"))
                    Else
                        strDivCssClass = String.Concat(strDivCssClass, " decibel-block")
                        strMarketSharePercent = "No contribution to"
                        strMarketCategoryTotal = ""
                        boolShowDimmedImage = True
                    End If

                    Select Case DashboardCategory
                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_DRYWALL
                            lblDrywallMarketSpendPercent.Text = strMarketSharePercent
                            lblDrywallTotalSalesVolume.Text = strMarketCategoryTotal
                            divDrywall.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_FLOORING
                            lblFlooringMarketSpendPercent.Text = strMarketSharePercent
                            lblFlooringTotalSalesVolume.Text = strMarketCategoryTotal
                            divFlooring.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_GARAGE_DOOR
                            lblGarageDoorsMarketSpendPercent.Text = strMarketSharePercent
                            lblGarageDoorsTotalSalesVolume.Text = strMarketCategoryTotal
                            divGarageDoor.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_HVAC
                            lblHVACMarketSpendPercent.Text = strMarketSharePercent
                            lblHVACTotalSalesVolume.Text = strMarketCategoryTotal
                            divHVAC.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_INSULATION
                            lblInsulationMarketSpendPercent.Text = strMarketSharePercent
                            lblInsulationTotalSalesVolume.Text = strMarketCategoryTotal
                            divInsulation.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_KITCHEN_AND_BATH
                            lblKitchenBathMarketSpendPercent.Text = strMarketSharePercent
                            lblKitchenBathTotalSalesVolume.Text = strMarketCategoryTotal
                            divKitchenAndBath.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_LIGHTING
                            lblLightingMarketSpendPercent.Text = strMarketSharePercent
                            lblLightingTotalSalesVolume.Text = strMarketCategoryTotal
                            divLighting.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_ROOFING
                            lblRoofingMarketSpendPercent.Text = strMarketSharePercent
                            lblRoofingTotalSalesVolume.Text = strMarketCategoryTotal
                            divRoofing.Attributes.Item("class") = strDivCssClass

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_SUPPLY_HOUSE
                            lblSupplyHouseMarketSpendPercent.Text = strMarketSharePercent
                            lblSupplyHouseTotalSalesVolume.Text = strMarketCategoryTotal
                            divSupplyHouse.Attributes.Item("class") = strDivCssClass
                    End Select
                Else

                    Select Case DashboardCategory
                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_DRYWALL
                            divDrywallWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_FLOORING
                            divFlooringWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_GARAGE_DOOR
                            divGarageDoorWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_HVAC
                            divHVACWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_INSULATION
                            divInsulationWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_KITCHEN_AND_BATH
                            divKitchenAndBathWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_LIGHTING
                            divLightingWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_ROOFING
                            divRoofingWrapper.Style.Add("display", "none")

                        Case SHOW_IN_DASHBOARD_CATEGORIES.DASHBOARD_CATEGORY_SUPPLY_HOUSE
                            divSupplyHouseWrapper.Style.Add("display", "none")
                    End Select

                End If

            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateAllBuildersInLLC(Optional ByVal pStrSelectedBuilders As String = "")

        tblVendorUse.Rows.Clear()

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlBuildersInLLC As String = ""

        If pStrSelectedBuilders <> "" Then
            sqlBuildersInLLC = "SELECT BuilderId, CompanyName FROM Builder WHERE IsActive = 1 AND BuilderID IN (" & pStrSelectedBuilders & ") ORDER BY CompanyName"
        Else
            sqlBuildersInLLC = "Select BuilderId, CompanyName FROM Builder WHERE IsActive = 1 AND LLCId = (Select LLCId FROM Builder WHERE BuilderId = @BuilderId) ORDER BY CompanyName"
        End If

        Dim sqlComm As New SqlCommand(sqlBuildersInLLC, sqlConn)
        sqlComm.CommandType = CommandType.Text
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                Dim trHeaderBuilderRow As New HtmlTableRow

                Dim thVendorColumnHeader As New HtmlTableCell("th")
                thVendorColumnHeader.InnerText = ""
                thVendorColumnHeader.Attributes.Add("class", "top-left-corner-cell")
                trHeaderBuilderRow.Cells.Add(thVendorColumnHeader)

                While DR.Read()
                    Dim BuilderId As Int32 = DR("BuilderId")
                    Dim BuilderName As String = DR("CompanyName")

                    Dim hrefBuilderDirectory As New HtmlAnchor
                    hrefBuilderDirectory.HRef = ConfigurationManager.AppSettings("GlobalRefererName") & "/directory/default.aspx?builder=true&company=" & BuilderName
                    hrefBuilderDirectory.Target = "_blank"
                    hrefBuilderDirectory.InnerText = BuilderName
                    hrefBuilderDirectory.Style.Add("color", "black")

                    Dim pBuilderName As New HtmlGenericControl
                    pBuilderName.TagName = "p"
                    pBuilderName.Attributes.Add("class", "builder-column-header")
                    pBuilderName.Controls.Add(hrefBuilderDirectory)
                    'pBuilderName.Attributes.Add("style", "padding:10px 10px 10px 10px; writing-mode:vertical-rl")
                    'pBuilderName.InnerText = BuilderName

                    Dim thBuilder As New HtmlTableCell
                    thBuilder.Attributes.Add("style", "vertical-align: middle;")
                    thBuilder.Controls.Add(pBuilderName)

                    trHeaderBuilderRow.Cells.Add(thBuilder)
                End While

                tblVendorUse.Rows.Add(trHeaderBuilderRow)
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub PopulateVendorUseTable(Optional ByVal pStrSelectedCategories As String = "", Optional ByVal pStrSelectedBuilders As String = "")

        Dim sqlConn As New SqlConnection(cStrConn)
        sqlConn.Open()

        Dim sqlLLCVendorsByCategory As String = "sp_MarketWideVendorUtilization"


        Dim sqlComm As New SqlCommand(sqlLLCVendorsByCategory, sqlConn)
        sqlComm.CommandType = CommandType.StoredProcedure
        sqlComm.Parameters.AddWithValue("@BuilderID", cBuilderID)
        sqlComm.Parameters.AddWithValue("@ReportingQtr", cReportingQuarter)
        sqlComm.Parameters.AddWithValue("@ReportingYear", cReportingYear)

        If pStrSelectedCategories <> "" Then
            sqlComm.Parameters.AddWithValue("@SelectedCategoryIDList", pStrSelectedCategories)
        End If

        If pStrSelectedBuilders <> "" Then
            sqlComm.Parameters.AddWithValue("@SelectedBuilderIDList", pStrSelectedBuilders)
        End If

        Dim DR As SqlDataReader = sqlComm.ExecuteReader()

        Try
            If DR.HasRows Then
                Dim PrevVendorId As Int32 = 0
                Dim strPrevVendorName As String = String.Empty

                Dim PrevCategoryId As Int32 = 0
                Dim strPrevCategoryName As String = String.Empty

                Dim iRowIndex As Int16 = 0

                While DR.Read()
                    Dim DashboardCategoryId As Int16 = DR("DashboardCategoryId")
                    Dim DashboardCategory As String = DR("DashboardCategory")

                    Dim VendorId As Int32 = DR("VendorId")
                    Dim VendorName As String = DR("VendorName")

                    Dim BuilderId As Int32 = DR("BuilderId")
                    Dim BuilderName As String = DR("BuilderName")

                    Dim BuilderPurchaseFinalAmount As Double = DR("FinalAmount")

                    'Dim strToolTipText As String = String.Concat("$", String.Format("{0:n}", BuilderPurchaseFinalAmount))
                    Dim strToolTipText As String = ""

                    If DashboardCategoryId <> PrevCategoryId Then
                        'Add a new row for Category
                        'Dim iFontAwesomeCaret As New HtmlGenericControl("i")
                        'iFontAwesomeCaret.Attributes.Add("class", "fa fa-caret-up row-toggle")
                        'iFontAwesomeCaret.Attributes.Add("data-category", DashboardCategory)

                        'Dim divCaretIcon As New HtmlGenericControl("div")
                        'divCaretIcon.Controls.Add(iFontAwesomeCaret)

                        'Dim divCategory As New HtmlGenericControl("div")
                        'divCategory.InnerText = DashboardCategory

                        Dim divWrapper As New HtmlGenericControl("div")
                        divWrapper.Attributes.Add("class", "row-toggle")
                        divWrapper.Attributes.Add("data-category", DashboardCategory)
                        'divWrapper.Controls.Add(divCaretIcon)
                        'divWrapper.Controls.Add(divCategory)
                        divWrapper.InnerHtml = String.Concat("<i class=""fa fa-caret-up row-toggle"" data-category=", DashboardCategory, "></i>&nbsp;", DashboardCategory)

                        Dim thCategory As New HtmlTableCell("th")
                        thCategory.Controls.Add(divWrapper)

                        thCategory.ColSpan = tblVendorUse.Rows(0).Cells.Count
                        thCategory.Attributes.Add("class", "fixed-side dashboard-category")
                        thCategory.Attributes.Add("data-category", DashboardCategory)

                        Dim trCategory As New HtmlTableRow
                        trCategory.Attributes.Add("data-category", DashboardCategory)
                        trCategory.Cells.Add(thCategory)

                        tblVendorUse.Rows.Add(trCategory)

                        iRowIndex = iRowIndex + 1
                    End If

                    If Not VendorName.Equals(strPrevVendorName) Then
                        'Start a new row for next Vendor record
                        Dim thVendor As New HtmlTableCell("th")
                        thVendor.Attributes.Add("class", "fixed-side vendor-name")
                        thVendor.Attributes.Add("data-category", DashboardCategory)
                        thVendor.InnerText = VendorName

                        Dim tdPurchaseIndicator As New HtmlTableCell
                        tdPurchaseIndicator.Attributes.Add("class", "purchase-indicator")
                        tdPurchaseIndicator.InnerHtml = IIf(BuilderPurchaseFinalAmount > 0.0, "<i class=""fa fa-circle purchase-value"" aria-hidden=""True"" title=" & strToolTipText & "></i>", "")

                        Dim trVendorCol As New HtmlTableRow
                        trVendorCol.Attributes.Add("data-category", DashboardCategory)
                        trVendorCol.Attributes.Add("class", "vendor-row")

                        trVendorCol.Cells.Add(thVendor)
                        trVendorCol.Cells.Add(tdPurchaseIndicator)

                        tblVendorUse.Rows.Add(trVendorCol)

                        iRowIndex = iRowIndex + 1
                    Else
                        Dim tdPurchaseIndicator As New HtmlTableCell
                        tdPurchaseIndicator.Attributes.Add("class", "purchase-indicator")
                        tdPurchaseIndicator.InnerHtml = IIf(BuilderPurchaseFinalAmount > 0.0, "<i class=""fa fa-circle purchase-value"" aria-hidden=""True"" title=" & strToolTipText & "></i>", "")

                        tblVendorUse.Rows(iRowIndex).Cells.Add(tdPurchaseIndicator)
                    End If

                    PrevCategoryId = DashboardCategoryId
                    PrevVendorId = VendorId
                    strPrevVendorName = VendorName
                End While
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            sqlComm.Dispose()
            sqlComm = Nothing

            sqlConn.Close()
            sqlConn.Dispose()
            sqlConn = Nothing
        End Try

    End Sub

    Private Sub FilterBuilderAndCategory()

        Dim strSelectedCategories As String = ""
        If hdnSelectedCategories.Value <> "" Then
            strSelectedCategories = CStr(hdnSelectedCategories.Value).Substring(0, hdnSelectedCategories.Value.Length - 1)
        End If

        Dim strSelectedBuilders As String = ""
        If hdnSelectedBuilders.Value <> "" Then
            strSelectedBuilders = CStr(hdnSelectedBuilders.Value).Substring(0, hdnSelectedBuilders.Value.Length - 1)
        End If

        PopulateAllBuildersInLLC(strSelectedBuilders)
        PopulateVendorUseTable(strSelectedCategories, strSelectedBuilders)

    End Sub

    Private Sub btnBuilderCategoryFilter_Click(sender As Object, e As EventArgs) Handles btnBuilderCategoryFilter.Click
        'FilterBuilderAndCategory()
    End Sub

End Class
