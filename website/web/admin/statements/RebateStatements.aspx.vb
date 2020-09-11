Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Security.Policy
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Net
Imports System.Diagnostics
Imports System.Security
Imports System.IO.Compression

Partial Class Index
    Inherits AdminPage
    Private ResDb As New Database
    Private dtVendors As DataTable
    Private dtBuilders As DataTable
    Private qVendorInvoices As IEnumerable
    Private qBuilderInvoices As IEnumerable
    Private InvoiceDetails As IEnumerable
    Private dtInv As DataTable
    Private res As DataTable = Nothing

    Private _AuditTrailID As Integer = 0
    Private _ProjectName As String = "CBUSA_Legacy Application"
    Private _OperationDate As DateTime = DateAndTime.Now()
    Private _ModuleName As String = "Rebate Document"
    Private _PageURL As String = ""
    Private _CurrentUserId As String = ""
    Private _OperationType As String = ""
    Private _ColumnName As String = ""
    Private _OldValue As String = ""
    Private _NewValue As String = ""
    Private StorageAccount As String = "UseDevelopmentStorage = True;"
    Private FileName As String = ""
    Private UserName As String = ""
    Public Const HtmlNewLine As String = "<br />"


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        LoadVendors()
    End Sub

    Protected Sub LoadVendors()


        Dim varname1 As New System.Text.StringBuilder
        varname1.Append("SELECT Cast(Stuff((SELECT ',' + t1.email FROM vendoraccount t1 WHERE t1.vendorid " & vbCrLf)
        varname1.Append("       = " & vbCrLf)
        varname1.Append("       t2.vendorid FOR xml path('')), 1, 1, '') AS VARCHAR(max)) " & vbCrLf)
        varname1.Append("       + ', ' + t2.email AS Email, " & vbCrLf)
        varname1.Append("       t2.phone, " & vbCrLf)
        varname1.Append("       t2.historicid,t2.VendorID " & vbCrLf)
        varname1.Append("FROM   vendor t2")

        dtVendors = DB.GetDataTable(varname1.ToString)


        dtBuilders = BuilderRow.GetList(DB)


    End Sub
    Protected Sub Page_Load1(sender As Object, e As System.EventArgs) Handles Me.Load

        gvList.BindList = AddressOf BindBuilderRebateGrids
        Dim i As Integer = 1
        If Not IsPostBack Then
            F_PeriodQuarter.Items.Insert(0, New ListItem("", ""))
            F_PeriodYear.Items.Insert(0, New ListItem("", ""))
            For i = 1 To 4
                Me.F_PeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))
            Next

            For i = 1 To 15
                Me.F_PeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))
            Next


            Me.F_PeriodQuarter.SelectedValue = ""


            Me.F_PeriodYear.SelectedValue = ""

            F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()
            F_HistoricId.Text = Request("F_HistoricId")
            F_CompanyName.Text = Request("F_CompanyName")
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorHistoricID"
            BindBuilderRebateGrids()

            Dim dtVendorRole As DataTable = GetVendorRole(DB)
            chkVendorRole.DataSource = dtVendorRole
            chkVendorRole.DataBind()
        End If
    End Sub

    Protected Function LoadBuilderRebateGridsRecords() As DataTable
        Try
            Dim SQL As String = String.Empty
            Dim BufferSQL As String = String.Empty
            Dim dt As DataTable
            ViewState("F_SortBy") = gvList.SortBy
            ViewState("F_SortOrder") = gvList.SortOrder

            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            Dim ZeroValuFilter As String = ""

            If F_ZeroDue.Checked = True Then
                ZeroValuFilter = "isnull(AmountDue,0) = 0 "
            Else
                ZeroValuFilter = "isnull(AmountDue,0) <> 0 OR DocType = 'Return' OR DocType = 'Credit' "
            End If

            SQL = " SELECT VendorHistoricID, Phone, InvoiceDate, case when AmountDue = 0 then '' else DayspastDue end AS DayspastDue, RebateRate," _
                & " VendorName, invoice, PeriodQuarter, PeriodYear, PurchaseVolume, AmountDue, DocType " _
                & " From (" _
                & " SELECT vndrid                      AS VendorHistoricID, Phone ,Date as InvoiceDate, DayspastDue, RebateRate," _
                & " vndrname                    AS VendorName," _
                & " Document invoice, CASE  WHEN REPLACE(RebateDocType, ' ', '') = 'Writeoff' THEN 'Write-Off' WHEN REPLACE(RebateDocType, ' ', '') = 'Return' THEN 'Credit'" _
                & " WHEN REPLACE(RebateDocType, ' ', '') = 'Adjustment' THEN 'Invoice Adjustment' ELSE 'Invoice' END AS DocType, " _
                & " period                      AS PeriodQuarter," _
                & " year                        AS PeriodYear," _
                & " Sum(COALESCE(purchvol, 0))  AS PurchaseVolume," _
                & " Sum(COALESCE(amountdue, 0)) AS AmountDue" _
                & " FROM RG_ARReportInvAndRet " _
                & " GROUP  BY vndrid,vndrname,RebateDocType, document, period, year, DayspastDue, RebateRate ,Date, Phone, DocType " _
                & " ) Qry " _
                & " Where " & ZeroValuFilter '' isnull(AmountDue,0) <> 0 

            dt = ResDb.GetDataTable(SQL)

            qVendorInvoices = (From dr As DataRow In dt.AsEnumerable Join dv As DataRow In dtVendors.AsEnumerable On Core.GetInt(dr("VendorHistoricID")) Equals Core.GetInt(dv("HistoricID"))
                               Select New With {
                                        .VendorName = dr("VendorName"),
                                        .VendorHistoricID = dr("VendorHistoricID"),
                                        .invoice = dr("invoice"),
                                        .Phone = dr("Phone"),
                                        .DocType = dr("DocType"),
                                        .InvoiceDate = dr("InvoiceDate"),
                                        .DayspastDue = dr("DayspastDue"),
                                        .RebateRate = dr("RebateRate"),
                                        .PeriodQuarter = dr("PeriodQuarter"),
                                        .PeriodYear = dr("PeriodYear"),
                                        .PurchaseVolume = dr("PurchaseVolume"),
                                        .AmountDue = dr("AmountDue"),
                                        .VendorID = dv("VendorID")
                                }
                                  )

            Dim dtVendorInvoices As DataTable = EQToDataTable(qVendorInvoices)



            SQL = "Select * from RG_ARReportInvAndRet Where " & ZeroValuFilter
            dtInv = ResDb.GetDataTable(SQL)
            qBuilderInvoices = (From dr As DataRow In dtInv.AsEnumerable Join dv As DataRow In dtBuilders.AsEnumerable On Core.GetInt(dr("BldrID")) Equals Core.GetInt(dv("HistoricID"))
                                Select New With {
                                      .BldrID = dr("BldrID"),
                                      .invoice = dr("document"),
                                      .BuilderName = dv("CompanyName"),
                                      .DayspastDue = dr("DayspastDue"),
                                      .RebateRate = dr("RebateRate"),
                                      .PurchVol = dr("PurchVol"),
                                      .AmountDue = dr("AmountDue")
                                      }
                               )
            dtInv = EQToDataTable(qBuilderInvoices)

            Dim filters As New List(Of String)
            If F_HistoricId.Text <> String.Empty Then
                filters.Add("VendorHistoricID IN " & DB.NumberMultiple(F_HistoricId.Text))
            End If
            If F_CompanyName.Text <> String.Empty Then
                filters.Add("VendorName Like " & DB.FilterQuote(F_CompanyName.Text))
            End If
            If Not F_VendorID.SelectedValue = String.Empty Then
                filters.Add("VendorID = " & DB.Quote(F_VendorID.SelectedValue))
            End If
            If Not F_LLC.SelectedValues = String.Empty Then
                Dim cmap As New List(Of String)
                Dim reader As SqlDataReader = DB.GetReader("SELECT DISTINCT VendorID FROM LLCVendor WHERE LLCID IN " & DB.NumberMultiple(F_LLC.SelectedValues))
                While reader.Read
                    cmap.Add(reader("VendorID"))
                End While
                reader.Close()
                filters.Add("VendorID IN " & DB.NumberMultiple(String.Join(",", cmap.ToArray)))
            End If
            If F_Invoice.Text <> String.Empty Then
                filters.Add("Invoice Like " & DB.FilterQuote(F_Invoice.Text))
            End If
            If F_PeriodQuarter.SelectedValue <> String.Empty Then
                filters.Add("PeriodQuarter IN " & DB.NumberMultiple(F_PeriodQuarter.SelectedValue))
            End If
            If F_PeriodYear.SelectedValue <> String.Empty Then
                filters.Add("PeriodYear IN " & DB.NumberMultiple(F_PeriodYear.SelectedValue))
            End If
            If F_ZeroDue.Checked = True Then
                filters.Add("AmountDue = 0 ")
            End If
            Dim dvVendorInvoices As DataView = dtVendorInvoices.DefaultView
            dvVendorInvoices.Sort = gvList.SortByAndOrder
            dvVendorInvoices.RowFilter = String.Join(" AND ", filters.ToArray)

            res = dvVendorInvoices.ToTable

            ' dtVendorInvoices = dtVendorInvoices 'dtVendorInvoices.AsEnumerable.Take((gvList.PageIndex + 1) * gvList.PageSize).CopyToDataTable

            dtVendorInvoices.RejectChanges()

        Catch ex As SqlException
            Response.Write(ex.Message)
            If ResDb IsNot Nothing AndAlso ResDb.Transaction IsNot Nothing Then ResDb.RollbackTransaction()
            Logger.Error(Logger.GetErrorMessage(ex))
            AddError(ErrHandler.ErrorText(ex))
        Finally
            If ResDb IsNot Nothing AndAlso ResDb.IsOpen Then
                ResDb.Close()
            End If
        End Try

        Return res

    End Function

    Protected Sub BindBuilderRebateGrids()
        Dim BuilderRebateGrids As DataTable = LoadBuilderRebateGridsRecords()
        If res.Rows.Count > 0 Then
            gvList.DataSource = BuilderRebateGrids.AsEnumerable.Take((gvList.PageIndex + 1) * gvList.PageSize).CopyToDataTable
            gvList.Pager.NofRecords = BuilderRebateGrids.Rows.Count
            gvList.DataBind()
        Else
            gvList.DataSource = BuilderRebateGrids
            gvList.Pager.NofRecords = BuilderRebateGrids.Rows.Count
            gvList.DataBind()
        End If

        _OperationType = "View"
        _CurrentUserId = Session("AdminId")
        UserName = Session("Username")
        _PageURL = Request.Url.ToString()

        Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
        Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)
    End Sub

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
    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub
        Dim gvBuilderRebates As GridView = e.Row.FindControl("gvBuilderRebates")
        Dim LLCName As Literal = e.Row.FindControl("LLCName")
        Dim apContactName As Literal = e.Row.FindControl("apContactName")
        Dim apEmail As Literal = e.Row.FindControl("apEmail")
        Dim apPhone As Literal = e.Row.FindControl("apPhone")
        Dim Subfilters As New List(Of String)
        If Not IsDBNull(e.Row.DataItem("Invoice")) Then
            Subfilters.Add("Invoice IN " & DB.NumberMultiple(e.Row.DataItem("Invoice")))
        End If
        If dtInv.Columns.Count > 0 Then
            dtInv.DefaultView.RowFilter = String.Join(" AND ", Subfilters.ToArray)
        End If

        Dim conc As String = String.Empty
        For Each id As String In VendorRow.GetLLCList(DB, e.Row.DataItem("VendorID")).Split(",")
            Try
                If LLCRow.GetRow(DB, id).IsActive Then
                    LLCName.Text &= conc & LLCRow.GetRow(DB, id).LLC
                    conc = ","
                End If
            Catch ex As Exception

            End Try

        Next

        Try
            apContactName.Text = getNamesAccountPayables(DB, e.Row.DataItem("VendorID")).Rows(0)("FullName")
        Catch ex As Exception
            apContactName.Text = String.Empty

        End Try
        Try
            If Not IsDBNull(e.Row.DataItem("Phone")) Then
                apPhone.Text = e.Row.DataItem("Phone")
            Else
                apPhone.Text = getNamesAccountPayables(DB, e.Row.DataItem("VendorID")).Rows(0)("Phone")
            End If

        Catch ex As Exception
            apPhone.Text = String.Empty

        End Try
        Try
            apEmail.Text = getNamesAccountPayables(DB, e.Row.DataItem("VendorID")).Rows(0)("Email")
        Catch ex As Exception
            apEmail.Text = String.Empty

        End Try

        gvBuilderRebates.DataSource = dtInv
        gvBuilderRebates.DataBind()

        Dim imgBtnViewFile As ImageButton = e.Row.FindControl("BtnViewFileARRebate")
        imgBtnViewFile.OnClientClick = "javascript: return ViewPDF('" & e.Row.DataItem("invoice") & "');"

        _OperationType = "Viewed (Expanded)"
        _CurrentUserId = Session("AdminId")
        UserName = Session("Username")
        _PageURL = Request.Url.ToString()

        Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
        Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)

    End Sub

    Protected Function getNamesAccountPayables(ByVal db As Database, ByVal VendorId As Integer) As DataTable
        Dim sql1 As String = " select va.phone, va.Email, va.FirstName + ' ' + va.LastName as FullName from VendorAccountVendorRole vavr inner join " &
                                 " VendorAccount va on vavr.VendorAccountID=va.VendorAccountID   where va.VendorID= " & db.Number(VendorId) & " and vavr.VendorRoleID = 5 "
        Return db.GetDataTable(sql1)

    End Function
    Public Function GetVendorByVendorId(ByVal DB As Database, ByVal VendorHistoricID As Integer) As Integer
        Dim sql As String = "select VendorID from Vendor where HistoricID=" & DB.Number(VendorHistoricID)
        Dim VendorId As Integer
        Dim sdr As SqlDataReader = DB.GetReader(sql)
        If sdr.Read Then
            VendorId = sdr("VendorID")
        End If
        sdr.Close()
        Return VendorId
    End Function
    Public Function GetVendorRole(ByVal DB As Database) As DataTable
        Dim sql As String = "select * from VendorRole"
        Return DB.GetDataTable(sql)
    End Function
    Public Function GetVendorRoleByVendorId(ByVal DB As Database, ByVal VendorId As Integer, ByVal VendorRoleId As Integer) As String
        Dim sql As String = "select A.Email from VendorAccount a inner join VendorAccountVendorRole r " &
            "on a.VendorAccountID=r.VendorAccountID inner join VendorRole v on r.VendorRoleID=v.VendorRoleID where a.VendorID = " &
            DB.Number(VendorId) & " and v.VendorRoleID = " & DB.Number(VendorRoleId) & ""
        Dim Email As String = ""
        Dim sdr As SqlDataReader = DB.GetReader(sql)
        If sdr.HasRows() Then
            While sdr.Read()
                Email = String.Concat(Email, sdr("Email"), ";")
            End While
        End If
        sdr.Close()
        Return Email
    End Function

    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub
        ExportReport()
    End Sub
    Public Sub ExportReport()
        gvList.PageSize = 5000
        Dim res As DataTable = LoadBuilderRebateGridsRecords()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Market, VendorID ,VendorName , AP Contact , Email,Phone,Invoice,QTR,YEAR,IDATE,Days,Purchase Vol,RebateRate,Due")

        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows
                Dim Market As String = String.Empty
                Dim VendorID As String = String.Empty
                Dim VendorName As String = String.Empty
                Dim APContact As String = String.Empty
                Dim Email As String = String.Empty
                Dim Phone As String = String.Empty
                Dim Invoice As String = String.Empty
                Dim QTR As String = String.Empty
                Dim YEAR As String = String.Empty
                Dim IDATE As String = String.Empty
                Dim Days As String = String.Empty
                Dim PurchaseVol As String = String.Empty
                Dim RebateRate As String = String.Empty
                Dim Due As String = String.Empty

                Dim conc As String = String.Empty
                For Each id As String In VendorRow.GetLLCList(DB, row("VendorID")).Split(",")
                    If LLCRow.GetRow(DB, id).IsActive Then
                        Market &= conc & LLCRow.GetRow(DB, id).LLC
                        conc = ","
                    End If
                Next

                'If Not IsDBNull(row("HistoricVendorID")) Then
                '    Market = row("HistoricVendorID")
                'End If
                If Not IsDBNull(row("VendorHistoricID")) Then
                    VendorID = row("VendorHistoricID")
                End If
                If Not IsDBNull(row("VendorName")) Then
                    VendorName = row("VendorName")
                End If

                Try
                    APContact = getNamesAccountPayables(DB, row("VendorID")).Rows(0)("FullName")
                Catch ex As Exception
                    APContact = String.Empty

                End Try

                Try
                    If Not IsDBNull(row("Phone")) Then
                        Phone = row("Phone")
                    Else
                        Phone = getNamesAccountPayables(DB, row("VendorID")).Rows(0)("Phone")
                    End If

                Catch ex As Exception
                    Phone = String.Empty

                End Try
                Try
                    Email = getNamesAccountPayables(DB, row("VendorID")).Rows(0)("Email")
                Catch ex As Exception
                    Email = String.Empty
                End Try


                If Not IsDBNull(row("invoice")) Then
                    Invoice = row("invoice")
                End If
                If Not IsDBNull(row("PeriodQuarter")) Then
                    QTR = row("PeriodQuarter")
                End If
                If Not IsDBNull(row("PeriodYear")) Then
                    YEAR = row("PeriodYear")
                End If
                If Not IsDBNull(row("InvoiceDate")) Then
                    IDATE = row("InvoiceDate")
                End If
                If Not IsDBNull(row("DaysPastDue")) Then
                    Days = row("DaysPastDue")
                End If
                If Not IsDBNull(row("PurchaseVolume")) Then
                    PurchaseVol = FormatCurrency(row("PurchaseVolume"), 2)
                End If
                If Not IsDBNull(row("RebateRate")) Then
                    RebateRate = row("RebateRate")
                End If
                If Not IsDBNull(row("AmountDue")) Then
                    Due = FormatCurrency(row("AmountDue"), 2)
                End If




                sw.WriteLine(Core.QuoteCSV(Market) & "," & Core.QuoteCSV(VendorID) & "," & Core.QuoteCSV(VendorName) & "," & Core.QuoteCSV(APContact) & "," & Core.QuoteCSV(Email) & "," & Core.QuoteCSV(Phone) & "," & Core.QuoteCSV(Invoice) & "," & Core.QuoteCSV(QTR) & "," & Core.QuoteCSV(YEAR) & "," & Core.QuoteCSV(IDATE) & "," & Core.QuoteCSV(Days) & "," & Core.QuoteCSV(PurchaseVol) & "," & Core.QuoteCSV(RebateRate) & "," & Core.QuoteCSV(Due))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub

    'Private Sub ActionButtonARRebate_Click(sender As Object, e As EventArgs) Handles ActionButtonARRebate.Click
    '    gvList.Enabled = False
    '    'Find the button
    '    For Each myRow In gvList.Rows
    '        Dim myButton As ImageButton = myRow.Cells(0).Controls(1)
    '        If (Not IsNothing(myButton)) Then
    '            myButton.ImageUrl = "disabled.gif"
    '        End If
    '    Next
    'End Sub
    Protected Sub BtnViewFileARRebate_Click(sender As Object, e As ImageClickEventArgs)
        MsgBox("View clicked")
    End Sub
    Protected Sub BtnDownloadFileARRebate_Click(sender As Object, e As ImageClickEventArgs)
        MsgBox("Download clicked")
    End Sub

    Protected Sub gvList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvList.SelectedIndexChanged

    End Sub

    Private Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName = "ViewFile" Then
            'Determine the RowIndex of the Row whose Button was clicked.
            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            'Reference the GridView Row.
            Dim row As GridViewRow = gvList.Rows(rowIndex)
            'Fetch value of Name.
            Dim InvoiceNo As String = row.Cells(9).Text.Trim
            Dim InvoicePDFPath As String = AppSettings("DestinationFilePath")
            'Dim dir As DirectoryInfo = New DirectoryInfo(InvoicePDFPath)

            'For Each File As FileInfo In dir.GetFiles()

            '    'fi.CopyTo("C:\temp\" + fi.Name)
            '    If File.Name = InvoiceNo & ".pdf" Then

            '        If File.Exists Then
            '            Dim client As New WebClient()
            '            Dim buffer As [Byte]() = client.DownloadData(InvoicePDFPath & InvoiceNo & ".pdf")

            '            If buffer IsNot Nothing Then
            '                Response.ContentType = "application/pdf"
            '                Response.AddHeader("content-length", buffer.Length.ToString())
            '                Response.BinaryWrite(buffer)
            '                Exit For
            '            End If
            '        Else
            '            'Response.Write("This file does not exist." & vbCrLf & "(" & InvoicePDFPath & "\" & InvoiceNo & ".pdf" & ")")
            '        End If
            '    End If
            'Next

            Dim File As System.IO.FileInfo = New System.IO.FileInfo(InvoicePDFPath & "\" & InvoiceNo & ".pdf")
            If File.Exists Then
                Dim client As New WebClient()

                Dim buffer As [Byte]() = client.DownloadData(InvoicePDFPath & "\" & InvoiceNo & ".pdf")

                If buffer IsNot Nothing Then
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("content-length", buffer.Length.ToString())
                    Response.BinaryWrite(buffer)

                    Context.Response.Write("<script language='javascript'>window.open('ViewPDF.aspx?invoice=" & InvoiceNo & "','_blank');</script>")

                    'Page.ClientScript.RegisterStartupScript(Me.GetType(), "OpenWindow", "window.open('" + +"','_newtab');", True)

                End If

                _OperationType = "View pdf File"
                _CurrentUserId = Session("AdminId")
                UserName = Session("Username")
                _PageURL = Request.Url.ToString()

                Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
                Dim Result = Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)

            Else
                ErrorMessage("This file does not exist." & HtmlNewLine & "(" & InvoicePDFPath & "\" & InvoiceNo & ".pdf" & ")")
            End If

        End If
        If e.CommandName = "DownloadFile" Then
            'Determine the RowIndex of the Row whose Button was clicked.
            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            'Reference the GridView Row.
            Dim row As GridViewRow = gvList.Rows(rowIndex)
            'Fetch value of Name.
            Dim InvoiceNo As String = row.Cells(9).Text.Trim
            Dim InvoicePDFPath As String = AppSettings("DestinationFilePath")
            Dim File As System.IO.FileInfo = New System.IO.FileInfo(InvoicePDFPath & "\" & InvoiceNo & ".pdf")
            If File.Exists Then
                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & File.Name)
                Response.AddHeader("Content-Length", File.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(File.FullName)
                Response.End()

                _OperationType = "Download File"
                _CurrentUserId = Session("AdminId")
                UserName = Session("Username")
                _PageURL = Request.Url.ToString()

                Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
                Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)


            Else
                ErrorMessage("This file does not exist." & HtmlNewLine & "(" & InvoicePDFPath & "\" & InvoiceNo & ".pdf" & ")")
            End If
        End If
    End Sub
    Protected Sub btnSend_Click(sender As Object, e As System.EventArgs) Handles btnSend.Click

        Try
            Dim EmailCompare As New StringBuilder
            Dim InvoiceNo As New List(Of InvoiceList)
            Dim VendorIdList As New List(Of Integer)
            Dim VendorIdListDistinct As New List(Of Integer)
            Dim Email As String = ""
            Dim MessageStr As String = ""
            Dim ErrorMessageStr As String = ""
            Dim DestinationFilePath As String = System.Configuration.ConfigurationManager.AppSettings("DestinationFilePath").ToString()

            For Each gvRow As GridViewRow In gvList.Rows
                Dim IsChecked As Boolean = CType(gvRow.FindControl("ChkSelectRow"), CheckBox).Checked
                If IsChecked Then
                    Dim VendorHistoricID As String = gvList.Rows(gvRow.RowIndex).Cells(3).Text
                    Dim VendorId As Integer = GetVendorByVendorId(DB, VendorHistoricID)
                    VendorIdList.Add(GetVendorByVendorId(DB, VendorHistoricID))
                    InvoiceNo.Add(New InvoiceList(gvList.Rows(gvRow.RowIndex).Cells(9).Text.ToString().Trim(), VendorId, gvList.Rows(gvRow.RowIndex).Cells(4).Text.ToString().Trim()))
                End If
            Next
            VendorIdListDistinct = VendorIdList.ToList().Distinct().ToList()
            For Each VendorId As Integer In VendorIdListDistinct
                If VendorIdList.FindAll(Function(p) p = VendorId).Count = 1 Then
                    Dim VInvoiceNo As String = InvoiceNo.Where(Function(x) x.VVendorId = VendorId).Select(Function(s) s.VInvoiceNo).FirstOrDefault()
                    Dim VVendorName As String = InvoiceNo.Where(Function(x) x.VVendorId = VendorId).Select(Function(s) s.VVendorName).FirstOrDefault()
                    If Not File.Exists(DestinationFilePath & VInvoiceNo & ".pdf") Then
                        ErrorMessageStr += "This file does not exist. for the Vendor Name - " & VVendorName & " " & HtmlNewLine & "(" & DestinationFilePath & "\" & VInvoiceNo & ".pdf" & ")"
                    Else
                        For Each Li As ListItem In chkVendorRole.Items
                            If Li.Selected = True Then
                                Dim strEmailsForSelectedRole As String = GetVendorRoleByVendorId(DB, VendorId, Li.Value)
                                Dim arrEmailsForSelectedRole As String() = Nothing

                                If Not EmailCompare.ToString().Contains(strEmailsForSelectedRole) Then
                                    EmailCompare.Append(strEmailsForSelectedRole)
                                    'Email = GetVendorRoleByVendorId(DB, VendorId, Li.Value)

                                    If String.IsNullOrEmpty(strEmailsForSelectedRole) Then
                                        ErrorMessageStr += "Role - " & Li.Text & " is not Available for the Vendor - " & VVendorName & " " & HtmlNewLine
                                    Else
                                        If strEmailsForSelectedRole.Contains(";") Then
                                            arrEmailsForSelectedRole = strEmailsForSelectedRole.Split(";")
                                        Else
                                            arrEmailsForSelectedRole(0) = strEmailsForSelectedRole
                                        End If

                                        For Each strEmail As String In arrEmailsForSelectedRole
                                            If Not strEmail = "" Then
                                                Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
                                                Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")

                                                Core.SendSimpleMail(FromAddress, FromName, strEmail, "", "CBUSA Invoice " & VInvoiceNo, "CBUSA Preferred Vendor - Please click on the attachment to view all outstanding rebate invoices. If you have any questions please contact the CBUSA team.", VInvoiceNo & ".pdf", DestinationFilePath)

                                                MessageStr = "Invoices have been sent" & HtmlNewLine
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        Next
                    End If
                Else
                    Dim VVendorName As String = InvoiceNo.Where(Function(x) x.VVendorId = VendorId).Select(Function(s) s.VVendorName).FirstOrDefault()
                    Dim ZipFolderCreation As String = DestinationFilePath & VendorId
                    If Not Directory.Exists(ZipFolderCreation) Then
                        Directory.CreateDirectory(ZipFolderCreation)
                    End If
                    For Each Inv As String In InvoiceNo.Where(Function(x) x.VVendorId = VendorId).Select(Function(s) s.VInvoiceNo)
                        If Not File.Exists(DestinationFilePath & Inv & ".pdf") Then
                            ErrorMessageStr += "This file does not exist. for the Vendor Name - " & VVendorName & " " & HtmlNewLine & "(" & DestinationFilePath & "\" & Inv & ".pdf" & ")"
                        Else
                            If Not File.Exists(ZipFolderCreation & "\" & Inv & ".pdf") Then
                                File.Copy(DestinationFilePath & Inv & ".pdf", ZipFolderCreation & "\" & Inv & ".pdf")
                            End If
                            If File.Exists(DestinationFilePath & "Invoice.zip") Then
                                File.Delete(DestinationFilePath & "Invoice.zip")
                            End If
                            ZipFile.CreateFromDirectory(ZipFolderCreation, DestinationFilePath & "Invoice.zip", CompressionLevel.Fastest, True)
                        End If
                    Next

                    EmailCompare = New StringBuilder()
                    For Each Li As ListItem In chkVendorRole.Items
                        If Li.Selected = True Then
                            Dim strEmailsForSelectedRole As String = GetVendorRoleByVendorId(DB, VendorId, Li.Value)
                            Dim arrEmailsForSelectedRole As String() = Nothing

                            If Not EmailCompare.ToString().Contains(strEmailsForSelectedRole) Then
                                EmailCompare.Append(strEmailsForSelectedRole)
                                'Email = GetVendorRoleByVendorId(DB, VendorId, Li.Value)

                                If String.IsNullOrEmpty(strEmailsForSelectedRole) Then
                                    ErrorMessageStr += "Role - " & Li.Text & " is not Available for the Vendor - " & VVendorName & " " & HtmlNewLine
                                Else
                                    If strEmailsForSelectedRole.Contains(";") Then
                                        arrEmailsForSelectedRole = strEmailsForSelectedRole.Split(";")
                                    Else
                                        arrEmailsForSelectedRole(0) = strEmailsForSelectedRole
                                    End If

                                    For Each strEmail As String In arrEmailsForSelectedRole
                                        If Not strEmail = "" Then
                                            Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
                                            Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
                                            Core.SendSimpleMail(FromAddress, FromName, strEmail, "", "CBUSA Invoices", "CBUSA Preferred Vendor - Please click on the attachment to view all outstanding rebate invoices. If you have any questions please contact the CBUSA team.", "Invoice.zip", DestinationFilePath)
                                            MessageStr = "Invoices have been sent" & HtmlNewLine
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    Next
                End If
            Next

            _OperationType = "Email Generate"
            _CurrentUserId = Session("AdminId")
            UserName = Session("Username")
            _PageURL = Request.Url.ToString()

            Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
            Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)

            lblErrorMsg.Text = MessageStr & HtmlNewLine & ErrorMessageStr
            Dim script As String = "window.onload = function() { OpenInvoiceForm(); };"
            ClientScript.RegisterStartupScript(Me.GetType(), lblErrorMsg.Text, script, True)

            For Each Li As ListItem In chkVendorRole.Items
                If Li.Selected = True Then
                    Li.Selected = False
                End If
            Next
            CheckBoxAll.Checked = False
            Dim chkall As CheckBox = CType(gvList.HeaderRow.FindControl("ChkSelectAll"), CheckBox)
            chkall.Checked = False
            For Each gvRow As GridViewRow In gvList.Rows
                Dim IsChecked As CheckBox = CType(gvRow.FindControl("ChkSelectRow"), CheckBox)
                IsChecked.Checked = False
            Next

        Catch ex As Exception
            LblFilePath.InnerText = ex.Message.ToString()
        End Try
    End Sub
    Public Class InvoiceList
        Public Property VInvoiceNo As String
        Public Property VVendorId As Integer
        Public Property VVendorName As String
        Public Sub New(InvoiceNo As String, VendorId As Integer, VendorName As String)
            Me.VInvoiceNo = InvoiceNo
            Me.VVendorId = VendorId
            Me.VVendorName = VendorName
        End Sub
    End Class
    Public Sub ErrorMessage(ByVal Message As String)
        Dim Msg As String = Message
        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type = 'text/javascript'>")
        sb.Append("window.onload=function(){")
        sb.Append("alert('")
        sb.Append(Msg)
        sb.Append("')};")
        sb.Append("</script>")
        ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
    End Sub
    Protected Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindBuilderRebateGrids()
    End Sub
End Class
