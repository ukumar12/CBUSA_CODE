Option Strict Off
Imports System.Configuration.ConfigurationManager
Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Services
Imports System.IO

Partial Class modules_Statements_RebateBuilderStatement
    Inherits ModuleControl
    Dim dtStatementRecords As DataTable = Nothing
    Dim dtBuilderStmtVolumeFee As DataTable = Nothing
    Dim dtBuilderStmtRebatePayment As DataTable = Nothing
    Dim dtBuilderStmtDistribution As DataTable = Nothing
    Dim dtBuilderStmtBeginBalance As DataTable = Nothing
    Dim HistoricID As Integer
    Dim InitialBalance As Decimal
    Dim drrecords As DataRow
    Dim isCustomSearch As Boolean = False
    Dim Validcols() As String = {"RowNumber", "DisplayReportingYearQtr", "DocumentDate", "DisplayDate", "TransactionType", "ReportingYear", "ReportingQtr", "BuilderID", "BuilderName", "VendorID", "VendorName", "PVFeeRate", "PurchaseVolume", "Amount", "Balance", "ProductCategory", "DocumentType", "DocumentNumber", "OrigDocNumber", "TransactionTypeOrder"}
    Dim RowNum As Integer = 1
    Dim TransactionTypeOrder As Integer = 1
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsAdminDisplay Then Exit Sub
        HistoricID = DB.ExecuteScalar("SELECT Top 1 HistoricID FROM Builder WHERE BuilderID =" & Session("BuilderID"))

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        If Not IsPostBack Then

        Core.DataLog("Rebate Account Statement", PageURL, CurrentUserId, "Builder Rebates Left Menu Click", "", "", "", "", UserName)

            Try

                Dim dtprogramsBuilder As DataTable = GetDataTableForPrograms("RG_BuilderStmtBeginBalance", HistoricID, "01/01/2001")
                'Dim dt As DataTable = GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value)
                'BindDropDowns(dt)
                'BindGridView(dt)
                BindProgramDropdown(dtprogramsBuilder)
                dpDateLbound.Value = Now.AddDays(-30).Date
                If dpDateLbound.Value < "11/1/2015" Then
                    dpDateLbound.Value = "11/1/2015"
                End If
                dpDateUbound.Value = Now.Date
                BindList()
            Catch ex As Exception

            End Try

        End If

    End Sub

    Private Sub BindList()
        BindGridView(GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value))
        BindDropDowns(GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value))
        Session("DateFrom") = dpDateLbound.Value
        Session("DateTo") = dpDateUbound.Value
    End Sub

    Protected Sub TransactionTypeChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlTransactionType As DropDownList = DirectCast(sender, DropDownList)
        ViewState("TransactionType") = ddlTransactionType.SelectedValue
        BindGridView(GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value), ddlTransactionType.SelectedValue, ViewState("VendorName"), ViewState("ReportingYearQtr"))

    End Sub
    Protected Sub BindProgramDropdown(ByVal dtPrograms As DataTable)
        ddlPrograms.DataSource = dtPrograms.DefaultView.ToTable
        ddlPrograms.DataTextField = "ProductCategory"
        ddlPrograms.DataValueField = "ProductCategory"
        ddlPrograms.DataBind()

    End Sub
    Protected Sub VendorNameChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlVendorName As DropDownList = DirectCast(sender, DropDownList)
        ViewState("VendorName") = ddlVendorName.SelectedValue
        BindGridView(GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value), ViewState("TransactionType"), ddlVendorName.SelectedValue, ViewState("ReportingYearQtr"))
    End Sub
    Protected Sub ReportingYearQtrChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim ddlVendorName As DropDownList = DirectCast(sender, DropDownList)
        ViewState("ReportingYearQtr") = ddlReportingYearQtr.SelectedValue
        BindGridView(GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value), ViewState("TransactionType"), ViewState("VendorName"), ddlReportingYearQtr.SelectedValue)
    End Sub

    Protected Sub BindDropDowns(ByVal dt As DataTable)
        ddlVendorName.DataSource = GetSortedDatatable(dt, "VendorName")
        ddlVendorName.DataTextField = "VendorName"
        ddlVendorName.DataValueField = "VendorName"
        ddlVendorName.DataBind()
        ddlVendorName.Items.Insert(0, New ListItem("--ALL --", ""))
        ddlReportingYearQtr.DataSource = dt.DefaultView.ToTable(True, "DisplayReportingYearQtr")
        ddlReportingYearQtr.DataTextField = "DisplayReportingYearQtr"
        ddlReportingYearQtr.DataValueField = "DisplayReportingYearQtr"
        ddlReportingYearQtr.DataBind()
        ddlReportingYearQtr.Items.Insert(0, New ListItem("--ALL --", ""))
        ddlTransactionType.SelectedValue = ""
    End Sub
    Private Function GetSortedDatatable(ByVal dt As DataTable, Optional ByVal ColumnName As String = "") As DataTable
        dt.DefaultView.RowFilter = ColumnName & " IS NOT NULL "
        dt.DefaultView.Sort = ColumnName & " ASC"
        Return dt.DefaultView.ToTable(True, ColumnName)
    End Function

    'Protected Function GetDatatableWithBalance(ByVal dt As DataTable, ByVal dtBuilderStmtBeginBalance As DataTable, ByVal HistoricID As Integer, ByVal DateFrom As Date) As DataTable
    '   . Dim InitialBalance As Double = dtBuilderStmtBeginBalance.DefaultView .
    '    For Each dr In dt.Rows
    '        dr("balance") = InitialBalance + dr("Amount")
    '        InitialBalance = dr("balance")
    '    Next
    '    dt.AcceptChanges()
    '    Return dt
    'End Function

    Protected Sub BindGridView(ByVal dt As DataTable, Optional ByVal TransactionType As String = "", Optional ByVal VendorName As String = "", Optional ByVal DisplayReportingYearQtr As String = "")

        Dim filters As New List(Of String)
        isCustomSearch = False
        If Not String.IsNullOrEmpty(TransactionType) Then
            filters.Add("TransactionType LIKE " & DB.FilterQuote(TransactionType.Trim))
            isCustomSearch = True
        End If
        If Not String.IsNullOrEmpty(VendorName) Then
            filters.Add("VendorName LIKE " & DB.FilterQuote(VendorName.Trim))
            isCustomSearch = True
        End If
        If Not String.IsNullOrEmpty(DisplayReportingYearQtr) Then
            filters.Add("DisplayReportingYearQtr LIKE " & DB.FilterQuote(DisplayReportingYearQtr.Trim))
            isCustomSearch = True
        End If


        dt.DefaultView.RowFilter = String.Join(" AND ", filters.ToArray)
        'For Each dr As DataRow In dt.Rows
        '    If dr.Item("TransactionType") = "Volume Fee" Then
        '        dr.Item("TransactionTypeOrder") = 1
        '    ElseIf dr.Item("TransactionType") = "Rebate Pmt" Then
        '        dr.Item("TransactionTypeOrder") = 2
        '    ElseIf dr.Item("TransactionType") = "Distribution" Then
        '        dr.Item("TransactionTypeOrder") = 3
        '    ElseIf dr.Item("TransactionType") = "Volume Fee Adj" Then
        '        dr.Item("TransactionTypeOrder") = 4
        '    Else
        '        dr.Item("TransactionTypeOrder") = 0
        '    End If
        'Next

        dt.DefaultView.Sort = "DisplayDate,TransactionTypeOrder DESC"
        dt = dt.DefaultView.ToTable




        For Each dr As DataRow In dt.Rows
            dr.Item("RowNumber") = RowNum
            RowNum = RowNum + 1
            Dim Balance As Decimal = dr("Amount") + InitialBalance
            InitialBalance = Balance
            dr.Item("Balance") = Balance
        Next

        dt.DefaultView.Sort = "RowNumber DESC"
        rptList.DataSource = dt.DefaultView.ToTable()
        rptList.DataBind()


    End Sub
    Protected Sub ExportGridView(ByVal dtExport As DataTable, Optional ByVal TransactionType As String = "", Optional ByVal VendorName As String = "", Optional ByVal DisplayReportingYearQtr As String = "")

        Dim filters As New List(Of String)
        isCustomSearch = False
        If Not String.IsNullOrEmpty(TransactionType) Then
            filters.Add("TransactionType LIKE " & DB.FilterQuote(TransactionType.Trim))
            isCustomSearch = True
        End If
        If Not String.IsNullOrEmpty(VendorName) Then
            filters.Add("VendorName LIKE " & DB.FilterQuote(VendorName.Trim))
            isCustomSearch = True
        End If
        If Not String.IsNullOrEmpty(DisplayReportingYearQtr) Then
            filters.Add("DisplayReportingYearQtr LIKE " & DB.FilterQuote(DisplayReportingYearQtr.Trim))
            isCustomSearch = True
        End If

        filters.Add("BuilderID IS NOT NULL")



        dtExport.DefaultView.RowFilter = String.Join(" AND ", filters.ToArray)
        dtExport.DefaultView.Sort = "DisplayDate,TransactionTypeOrder DESC"



        dtExport = dtExport.DefaultView.ToTable
        For Each dr As DataRow In dtExport.Rows
            dr.Item("RowNumber") = RowNum
            RowNum = RowNum + 1
            Dim Balance As Decimal = dr("Amount") + InitialBalance
            InitialBalance = Balance
            dr.Item("Balance") = Balance
        Next

        dtExport.DefaultView.Sort = "RowNumber DESC"

        Dim dttoexport As DataTable = dtExport.DefaultView.ToTable


        Dim res As DataTable = dttoexport
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("DocumentDate , TransactionType , ReportingYearQtr,VendorName , PurchaseVolume , PVFeeRate,Amount ")

        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows

                Dim DocumentDateE As String = String.Empty
                If Not IsDBNull(row("DisplayDate")) Then
                    DocumentDateE = row("DisplayDate")
                End If

                Dim TransactionTypeE As String = String.Empty
                If Not IsDBNull(row("TransactionType")) Then
                    TransactionTypeE = row("TransactionType")
                End If

                Dim DisplayReportingYearQtrE As String = String.Empty
                If Not IsDBNull(row("DisplayReportingYearQtr")) Then
                    DisplayReportingYearQtrE = row("DisplayReportingYearQtr")
                End If

                Dim VendorNameE As String = String.Empty
                If Not IsDBNull(row("VendorName")) Then
                    VendorNameE = row("VendorName")
                End If

                Dim PurchaseVolumeE As String = String.Empty
                If Not IsDBNull(row("PurchaseVolume")) Then
                    PurchaseVolumeE = row("PurchaseVolume")
                End If

                Dim PVFeeRateE As String = String.Empty
                If Not IsDBNull(row("PVFeeRate")) Then
                    PVFeeRateE = row("PVFeeRate")
                End If


                Dim AmountE As String = String.Empty
                If Not IsDBNull(row("Amount")) Then
                    AmountE = FormatCurrency(row("Amount"))
                End If


                sw.WriteLine(Core.QuoteCSV(DocumentDateE) & "," & Core.QuoteCSV(TransactionTypeE) & "," & Core.QuoteCSV(DisplayReportingYearQtrE) & "," & Core.QuoteCSV(VendorNameE) & "," & Core.QuoteCSV(PurchaseVolumeE) & "," & Core.QuoteCSV(PVFeeRateE) & "," & Core.QuoteCSV(AmountE))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub
    Private Function GetDataTableForPrograms(ByVal StoredProcedureName As String, ByVal BuilderID As Integer, ByVal DateFrom As Date) As DataTable
        Dim ResDb As New Database
        Dim dt As New DataTable
        Dim prams(1) As SqlParameter
        prams(0) = New SqlParameter("@BUILDERID", BuilderID)
        prams(1) = New SqlParameter("@DATEFROM ", DateFrom)

        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            ResDb.RunProc(StoredProcedureName, prams, dt)
            Return dt
        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
        Finally
            ResDb.Close()
        End Try
        Return Nothing
    End Function


    Private Function GetDataTableFromResGroup(ByVal StoredProcedureName As String, ByVal BuilderID As Integer, ByVal DateFrom As Date, ByVal Dateto As Date, ByVal TransactionType As String) As DataTable
        Dim ResDb As New Database
        Dim dt As New DataTable
        Dim prams(2) As SqlParameter


        prams(0) = New SqlParameter("@DATEFROM", DateFrom)
        prams(1) = New SqlParameter("@DATETO", Dateto)
        prams(2) = New SqlParameter("@BUILDERID", BuilderID)

        Try

            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            ResDb.RunProc(StoredProcedureName, prams, dt)


            Return CleanDatatable(dt, TransactionType)
        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
        Finally
            ResDb.Close()
        End Try
        Return Nothing
    End Function
    Private Function CleanDatatable(ByVal dt As DataTable, ByVal TransactionType As String) As DataTable
        Dim dttemp As DataTable = DB.GetDataTable("Select TOP 0 * FROM RebateStatements")
        Dim filters As New List(Of String)
        If Not String.IsNullOrEmpty(ddlPrograms.SelectedValue) Then
            filters.Add("ProductCategory LIKE " & FilterQuoteExact(ddlPrograms.SelectedValue.Trim))
        End If
        dt.DefaultView.RowFilter = String.Join(" AND ", filters.ToArray)
        dt = dt.DefaultView.ToTable()
        dt.AcceptChanges()

        If TransactionType = "VolumeFee" Then
            dt.Columns("StatementType").ColumnName = "TransactionType"
            dt.Columns("DocumentDate").ColumnName = "DisplayDate"
            dt.Columns("PVFEE").ColumnName = "Amount"
            dt = MergeData(dttemp, dt, Validcols)
        ElseIf TransactionType = "RebatePmt" Then
            dt.Columns("StatementType").ColumnName = "TransactionType"
            dt.Columns("PaymentDate").ColumnName = "DisplayDate"
            dt.Columns("RebatePmt").ColumnName = "Amount"
            dt = MergeData(dttemp, dt, Validcols)
        ElseIf TransactionType = "Distribution" Then
            dt.Columns("StatementType").ColumnName = "TransactionType"
            dt.Columns("CheckDate").ColumnName = "DisplayDate"
            dt.Columns("BuilderDistribution").ColumnName = "Amount"
            dt.Columns("PayToVendorName").ColumnName = "VendorName"
            dt.Columns("PayToVendorID").ColumnName = "VendorID"
            dt = MergeData(dttemp, dt, Validcols)
        End If
        Dim finalamount As Double = 0.0

        For Each dr As DataRow In dt.Rows
            If dr.Item("TransactionType").ToString.ToLower = "distribution" Then
                dr.Item("DisplayReportingYearQtr") = String.Empty
            Else
                dr.Item("DisplayReportingYearQtr") = dr.Item("ReportingYear") & " Q" & dr.Item("ReportingQtr") '2015 Q1 
            End If

            If dr.Item("TransactionType").ToString.ToLower = "volume fee" Then
                dr.Item("TransactionTypeOrder") = 1
            ElseIf dr.Item("TransactionType").ToString.ToLower = "distribution" Then
                dr.Item("TransactionTypeOrder") = 2
            ElseIf dr.Item("TransactionType").ToString.ToLower = "volume fee adj" Then
                dr.Item("TransactionTypeOrder") = 3
            ElseIf dr.Item("TransactionType").ToString.ToLower = "rebate pmt" Then
                dr.Item("TransactionTypeOrder") = 4
            Else
                dr.Item("TransactionTypeOrder") = 0
            End If

        Next
        dt.AcceptChanges()
        Return dt
    End Function

    Private Function MergeData(ByVal tblA As DataTable, ByVal tblB As DataTable, ByVal colsA() As String) As DataTable
        Dim mergedtbl As New DataTable
        Dim col As DataColumn
        Dim sColumnName As String
        For Each sColumnName In colsA
            col = tblA.Columns(sColumnName)
            mergedtbl.Columns.Add(New DataColumn(col.ColumnName, col.DataType))
        Next
        For Each row As DataRow In tblA.Rows
            mergedtbl.ImportRow(row)
        Next row

        For Each row As DataRow In tblB.Rows
            mergedtbl.ImportRow(row)
        Next row
        Return mergedtbl
    End Function

    Private Function RemoveInvalidColumns(ByVal dt As DataTable, ByVal Col As String) As DataTable

        Dim dcCollection As DataColumnCollection = dt.Columns
        If Not dcCollection.Contains(Col) Then
            dt.Columns.Remove(Col)
        End If
        Return dt
    End Function
    Protected Function GetDatatable(ByVal HistoricID As Integer, ByVal DateFrom As Date, ByVal Dateto As Date) As DataTable
        'To do Caching
        Dim dtHugeFinalDatatable As DataTable = Nothing


        dtBuilderStmtVolumeFee = GetDataTableFromResGroup("RG_BuilderStmtVolumeFee", HistoricID, DateFrom, Dateto, "VolumeFee")

        dtBuilderStmtRebatePayment = GetDataTableFromResGroup("RG_BuilderStmtRebatePayment", HistoricID, DateFrom, Dateto, "RebatePmt")
        dtBuilderStmtDistribution = GetDataTableFromResGroup("RG_BuilderStmtDistribution", HistoricID, DateFrom, Dateto, "Distribution")
        dtHugeFinalDatatable = MergeData(dtBuilderStmtVolumeFee, dtBuilderStmtRebatePayment, Validcols)

        dtHugeFinalDatatable.Merge(dtBuilderStmtDistribution)
        dtBuilderStmtBeginBalance = GetDataTableForPrograms("RG_BuilderStmtBeginBalance", HistoricID, DateFrom)

        '*************  Fix for VSO#9721 *************
        dtBuilderStmtBeginBalance.DefaultView.RowFilter = "ProductCategory like '" & ddlPrograms.SelectedValue.Trim & "'"
        'dtBuilderStmtBeginBalance.DefaultView.RowFilter = "ProductCategory like '%" & ddlPrograms.SelectedValue.Trim & "%'"
        ' dtBuilderStmtBeginBalance.DefaultView.RowFilter = "ProductCategory = " & DB.NQuote (Local Vendor Rebates    " 
        Dim dtf As DataTable = dtBuilderStmtBeginBalance.DefaultView.ToTable
        InitialBalance = dtf.Rows(0).Item("NetBalance")

        Dim BalanceRow As DataRow = dtHugeFinalDatatable.NewRow
        BalanceRow("DisplayDate") = dtf.Rows(0).Item("BalanceAsOfDate")
        BalanceRow("TransactionType") = dtf.Rows(0).Item("StatementType")
        BalanceRow("Amount") = 0
        'BalanceRow("ReportItem") = "Select All"
        dtHugeFinalDatatable.Rows.InsertAt(BalanceRow, 0)

        dtHugeFinalDatatable.AcceptChanges()

        Return dtHugeFinalDatatable
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles btnSubmit.Click
        If Not Page.IsValid Then Exit Sub
        BindList()
    End Sub
    Protected Sub cvDate_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvDate.ServerValidate
        If (DateTime.Compare(dpDateLbound.Value, dpDateUbound.Value) > 0) Then
            args.IsValid = False
        Else
            args.IsValid = True
        End If
    End Sub
    Protected Sub lnkExport_Click(sender As Object, e As System.EventArgs) Handles lnkExport.Click
        If Not Page.IsValid Then Exit Sub
        ExportGridView(GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value), ddlTransactionType.SelectedValue, ddlVendorName.SelectedValue, ddlReportingYearQtr.SelectedValue)
        ' BindDropDowns(GetDatatable(HistoricID, dpDateLbound.Value, dpDateUbound.Value))
        'Session("DateFrom") = dpDateLbound.Value
        'Session("DateTo") = dpDateUbound.Value
    End Sub
    Protected Sub rptList_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptList.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim ltlBalance As Literal = e.Item.FindControl("ltlBalance")

            Dim ltlPVFeeRate As Literal = e.Item.FindControl("ltlPVFeeRate")
            Dim ltlPurchaseVolume As Literal = e.Item.FindControl("ltlPurchaseVolume")
            Dim ltlVendorName As Literal = e.Item.FindControl("ltlVendorName")
            Dim spnVendorName As HtmlGenericControl = e.Item.FindControl("spnVendorName")
            Dim ltlDisplayReportingYearQtr As Literal = e.Item.FindControl("ltlDisplayReportingYearQtr")
            Dim ltlAmount As Literal = e.Item.FindControl("ltlAmount")

            If Not IsDBNull(e.Item.DataItem("OrigDocNumber")) Then
                spnVendorName.Attributes.Add("onclick", "GetRebateLineDetails(" & Core.Escape(e.Item.DataItem("BuilderID")) & "," & Core.Escape(e.Item.DataItem("VendorID")) & "," & Core.Escape(e.Item.DataItem("VendorName")) & "," & Core.Escape(e.Item.DataItem("DisplayReportingYearQtr")) & "," & Core.Escape(Core.GetDate(dpDateLbound.Value)) & "," & Core.Escape(Core.GetDate(dpDateUbound.Value)) & "," & Core.Escape(e.Item.DataItem("OrigDocNumber")) & "," & Core.Escape(e.Item.DataItem("RowNumber")) & ")")
            End If
            If Not isCustomSearch Then
                '  Dim Balance As Decimal = e.Item.DataItem("Amount") + InitialBalance
                ' InitialBalance = Balance
                'ltlBalance.Text = FormatCurrency(Balance, 2, TriState.True, TriState.True)
                ltlBalance.Text = FormatCurrency(e.Item.DataItem("Balance"))
            End If
            If Not IsDBNull(e.Item.DataItem("PurchaseVolume")) Then
                ltlPurchaseVolume.Text = FormatCurrency(e.Item.DataItem("PurchaseVolume"), 2, TriState.True, TriState.True)
            End If



            If Not IsDBNull(e.Item.DataItem("PVFeeRate")) Then
                ltlPVFeeRate.Text = FormatPercent(e.Item.DataItem("PVFeeRate"), 2)
            End If
            If Not IsDBNull(e.Item.DataItem("Amount")) Then
                ltlAmount.Text = FormatCurrency(e.Item.DataItem("Amount"), 2, TriState.True, TriState.True)
            End If
            If IsDBNull(e.Item.DataItem("BuilderID")) Then
                ltlAmount.Visible = False
            End If

            If Not IsDBNull(e.Item.DataItem("DisplayReportingYearQtr")) Then
                ltlDisplayReportingYearQtr.Text = e.Item.DataItem("DisplayReportingYearQtr")
            Else
                ltlAmount.Visible = False
            End If
            If Not IsDBNull(e.Item.DataItem("VendorName")) Then
                ltlVendorName.Text = e.Item.DataItem("VendorName")
            End If


        End If
    End Sub

    'Added by Apala (Medullus) for fixing VSO#9721
    Private Function FilterQuoteExact(ByVal input As String) As String
        If DB.IsEmpty(input) Then
            Return "NULL"
        Else
            Return "'" + input.Replace("'", "''") + "'"
        End If
    End Function


   ' Protected Sub btnDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnDashBoard.Click
        'Response.Redirect("/builder/default.aspx")
   ' End Sub

End Class
