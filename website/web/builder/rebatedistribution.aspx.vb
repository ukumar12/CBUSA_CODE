Option Strict Off
Imports System.Configuration.ConfigurationManager
Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Services
Imports System.IO
Imports System.IO.Compression
Imports System.Net

Partial Class builder_rebatedistribution
    Inherits SitePage
    Private ResDb As New Database
    Private DtFrom As Date
    Private DtTo As Date
    Private BuilderId As Int64
    Private DtRebate As DataTable
    Private Property SortDirection As String
        Get
            Return IIf(ViewState("SortDirection") IsNot Nothing, Convert.ToString(ViewState("SortDirection")), "ASC")
        End Get
        Set(value As String)
            ViewState("SortDirection") = value
        End Set
    End Property


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'LoadVendors()
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            dpDateLbound.Value = DateAdd(DateInterval.Day, -60, DateTime.Now).Date
            dpDateUbound.Value = DateTime.Now.Date
            BindGrid()
        End If

    End Sub
    Public Function BindGridDataSource(ByVal DB As Database, ByVal BuilderID As Integer, ByVal DateFrom As Date, ByVal DateTo As Date) As DataTable
        'Dim ResDb As New Database
        'Response.Write(BuilderID.ToString() & " | " & DateFrom.ToString() & " | " & DateTo.ToString())
        Dim dt As New DataTable
        Dim prams(2) As SqlParameter
        prams(0) = New SqlParameter("@BUILDERID", BuilderID)
        prams(1) = New SqlParameter("@DATEFROM ", DateFrom)
        prams(2) = New SqlParameter("@DATETO ", DateTo)

        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            ResDb.RunProc("RG_RebateDistributionReport", prams, dt)
            dt.DefaultView.Sort = "DocumentDate Desc"
            dt = dt.DefaultView.ToTable()
            Return dt
        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
        Finally
            ResDb.Close()
        End Try
        Return Nothing
    End Function

    Public ReadOnly Property GetSelectedLLCs() As String
        Get
            Dim dr As SqlDataReader = DB.GetReader("select LLCID from LLCVendor where VendorID = " & Session("VendorId"))
            Dim Conn As String = String.Empty
            Dim Result As String = String.Empty

            While dr.Read()
                Result &= Conn & dr("LLCID")
                Conn = ","
            End While
            dr.Close()
            Return Result
        End Get
    End Property
    Public Function GetVendorList(ByVal DB As Database) As DataTable
        Dim SQL As String = "select * from Vendor where VendorId = " & DB.Number(Session("VendorId")) & ""
        Return DB.GetDataTable(SQL)
    End Function
    Public Function GetHistoricIdByBuilderId(ByVal BuilderId As Int32) As Int32
        Dim SQL As String = "select HistoricID from Builder where BuilderID =" & BuilderId
        Return DB.ExecuteScalar(SQL)
    End Function
    Public Function GetVendorRole(ByVal DB As Database) As DataTable
        Dim sql As String = "Select distinct v.VendorRoleID, VendorRole from VendorAccount a inner join VendorAccountVendorRole r " &
            "on a.VendorAccountID=r.VendorAccountID inner join VendorRole v on r.VendorRoleID=v.VendorRoleID And VendorID = " & Session("VendorId") & ""
        Return DB.GetDataTable(sql)
    End Function

    Protected Function LoadBuilderRebateGridsRecords() As DataTable
        'Try
        '    Dim SQL As String = String.Empty
        '    Dim BufferSQL As String = String.Empty
        '    Dim dt As DataTable
        '    ViewState("F_SortBy") = gvList.SortBy
        '    ViewState("F_SortOrder") = gvList.SortOrder

        '    ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
        '    Dim ZeroValuFilter As String = ""

        '    'If F_ZeroDue.Checked = True Then
        '    '    ZeroValuFilter = "isnull(AmountDue,0) = 0 "
        '    'Else
        '    '    ZeroValuFilter = "isnull(AmountDue,0) <> 0 OR DocType = 'Return' OR DocType = 'Credit' "
        '    'End If
        '    SQL = " SELECT VendorHistoricID, Phone, InvoiceDate, case when AmountDue = 0 then '' else DayspastDue end AS DayspastDue, RebateRate," _
        '        & " VendorName, invoice, PeriodQuarter, PeriodYear, PurchaseVolume, AmountDue, DocType " _
        '        & " From (" _
        '        & " SELECT vndrid                      AS VendorHistoricID, Phone ,Date as InvoiceDate, DayspastDue, RebateRate," _
        '        & " vndrname                    AS VendorName," _
        '        & " Document invoice, CASE  WHEN (DocType = 'Return' or DocType = 'Credit') THEN 'Credit' ELSE 'Invoice' END AS DocType , " _
        '        & " period                      AS PeriodQuarter," _
        '        & " year                        AS PeriodYear," _
        '        & " Sum(COALESCE(purchvol, 0))  AS PurchaseVolume," _
        '        & " Sum(COALESCE(amountdue, 0)) AS AmountDue" _
        '        & " FROM RG_ARReportInvAndRet rg " _
        '        & " GROUP  BY vndrid,vndrname,Document, period, year, DayspastDue, RebateRate ,Date, Phone, DocType " _
        '        & " ) Qry " _
        '        & " Where " & ZeroValuFilter '' isnull(AmountDue,0) <> 0 

        '    dt = ResDb.GetDataTable(SQL)

        '    qVendorInvoices = (From dr As DataRow In dt.AsEnumerable Join dv As DataRow In dtVendors.AsEnumerable On Core.GetInt(dr("VendorHistoricID")) Equals Core.GetInt(dv("HistoricID"))
        '                       Select New With {
        '                                .VendorName = dr("VendorName"),
        '                                .VendorHistoricID = dr("VendorHistoricID"),
        '                                .invoice = dr("invoice"),
        '                                .Phone = dr("Phone"),
        '                                .DocType = dr("DocType"),
        '                                .InvoiceDate = dr("InvoiceDate"),
        '                                .DayspastDue = dr("DayspastDue"),
        '                                .RebateRate = dr("RebateRate"),
        '                                .PeriodQuarter = dr("PeriodQuarter"),
        '                                .PeriodYear = dr("PeriodYear"),
        '                                .PurchaseVolume = dr("PurchaseVolume"),
        '                                .AmountDue = dr("AmountDue"),
        '                                .VendorID = dv("VendorID")
        '                        }
        '                          )

        '    Dim dtVendorInvoices As DataTable = EQToDataTable(qVendorInvoices)

        '    SQL = "Select * from RG_ARReportInvAndRet Where " & ZeroValuFilter
        '    dtInv = ResDb.GetDataTable(SQL)
        '    qBuilderInvoices = (From dr As DataRow In dtInv.AsEnumerable Join dv As DataRow In dtBuilders.AsEnumerable On Core.GetInt(dr("BldrID")) Equals Core.GetInt(dv("HistoricID"))
        '                        Select New With {
        '                              .BldrID = dr("BldrID"),
        '                              .invoice = dr("Document"),
        '                              .BuilderName = dv("CompanyName"),
        '                              .DayspastDue = dr("DayspastDue"),
        '                              .RebateRate = dr("RebateRate"),
        '                              .PurchVol = dr("PurchVol"),
        '                              .AmountDue = dr("AmountDue")
        '                              }
        '                       )
        '    dtInv = EQToDataTable(qBuilderInvoices)

        '    Dim filters As New List(Of String)
        '    'If F_Invoice.Text <> String.Empty Then
        '    '    filters.Add("Invoice Like " & DB.FilterQuote(F_Invoice.Text))
        '    'End If
        '    'If F_PeriodQuarter.SelectedValue <> String.Empty Then
        '    '    filters.Add("PeriodQuarter IN " & DB.NumberMultiple(F_PeriodQuarter.SelectedValue))
        '    'End If
        '    'If F_PeriodYear.SelectedValue <> String.Empty Then
        '    '    filters.Add("PeriodYear IN " & DB.NumberMultiple(F_PeriodYear.SelectedValue))
        '    'End If
        '    'If F_ZeroDue.Checked = True Then
        '    '    filters.Add("AmountDue = 0 ")
        '    'End If

        '    Dim dvVendorInvoices As DataView = dtVendorInvoices.DefaultView
        '    'dvVendorInvoices.Sort = gvList.SortByAndOrder
        '    If dvVendorInvoices.Count > 0 Then
        '        dvVendorInvoices.RowFilter = String.Join(" AND ", filters.ToArray)
        '    End If

        '    res = dvVendorInvoices.ToTable

        '    dtVendorInvoices = dtVendorInvoices 'dtVendorInvoices.AsEnumerable.Take((gvList.PageIndex + 1) * gvList.PageSize).CopyToDataTable

        '    dtVendorInvoices.RejectChanges()

        'Catch ex As SqlException
        '    If ResDb IsNot Nothing AndAlso ResDb.Transaction IsNot Nothing Then ResDb.RollbackTransaction()
        '    Logger.Error(Logger.GetErrorMessage(ex))
        '    AddError(ErrHandler.ErrorText(ex))
        'Finally
        '    If ResDb IsNot Nothing AndAlso ResDb.IsOpen Then
        '        ResDb.Close()
        '    End If
        'End Try
        'Return res
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

    Protected Function getNamesAccountPayables(ByVal db As Database, ByVal VendorId As Integer) As DataTable
        Dim sql1 As String = " select va.phone, va.Email, va.FirstName + ' ' + va.LastName as FullName from VendorAccountVendorRole vavr inner join " &
                                 " VendorAccount va on vavr.VendorAccountID=va.VendorAccountID   where va.VendorID= " & db.Number(VendorId) & " and vavr.VendorRoleID = 5 "
        Return db.GetDataTable(sql1)

    End Function
    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        'If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub
        'Dim gvBuilderRebates As GridView = e.Row.FindControl("gvBuilderRebates")
        'Dim LLCName As Literal = e.Row.FindControl("LLCName")
        'Dim apContactName As Literal = e.Row.FindControl("apContactName")
        'Dim apEmail As Literal = e.Row.FindControl("apEmail")
        'Dim apPhone As Literal = e.Row.FindControl("apPhone")
        'Dim Subfilters As New List(Of String)
        'If Not IsDBNull(e.Row.DataItem("Invoice")) Then
        '    Subfilters.Add("Invoice IN " & DB.NumberMultiple(e.Row.DataItem("Invoice")))
        'End If
        'If dtInv.Columns.Count > 0 Then
        '    dtInv.DefaultView.RowFilter = String.Join(" AND ", Subfilters.ToArray)
        'End If

        'Dim conc As String = String.Empty
        'For Each id As String In VendorRow.GetLLCList(DB, e.Row.DataItem("VendorID")).Split(",")
        '    Try
        '        If LLCRow.GetRow(DB, id).IsActive Then
        '            LLCName.Text &= conc & LLCRow.GetRow(DB, id).LLC
        '            conc = ","
        '        End If
        '    Catch ex As Exception

        '    End Try

        'Next

        'Try
        '    apContactName.Text = getNamesAccountPayables(DB, e.Row.DataItem("VendorID")).Rows(0)("FullName")
        'Catch ex As Exception
        '    apContactName.Text = String.Empty

        'End Try
        'Try
        '    If Not IsDBNull(e.Row.DataItem("Phone")) Then
        '        apPhone.Text = e.Row.DataItem("Phone")
        '    Else
        '        apPhone.Text = getNamesAccountPayables(DB, e.Row.DataItem("VendorID")).Rows(0)("Phone")
        '    End If

        'Catch ex As Exception
        '    apPhone.Text = String.Empty

        'End Try
        'Try
        '    apEmail.Text = getNamesAccountPayables(DB, e.Row.DataItem("VendorID")).Rows(0)("Email")
        'Catch ex As Exception
        '    apEmail.Text = String.Empty

        'End Try

        'gvBuilderRebates.DataSource = dtInv
        'gvBuilderRebates.DataBind()

        'Dim imgBtnViewFile As ImageButton = e.Row.FindControl("BtnViewFileARRebate")
        'imgBtnViewFile.OnClientClick = "javascript: return ViewPDF('" & e.Row.DataItem("invoice") & "');"

        '_OperationType = "Viewed (Expanded)"
        '_CurrentUserId = Session("VendorId")
        'UserName = Session("Username")

        'Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
        'Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)

    End Sub
    Protected Sub btnSend_Click(sender As Object, e As System.EventArgs) Handles btnSend.Click

        'Try
        '    Dim EmailCompare As New StringBuilder
        '    Dim VendorIdCompare As New StringBuilder
        '    Dim InvoiceNo As New List(Of String)
        '    Dim MessageStr As String = ""
        '    Dim ErrorMessageStr As String = ""
        '    Dim VendorName As String = ""
        '    Dim ZipFolderCreation As String = ""
        '    Dim Email As String = ""
        '    Dim DestinationFilePath As String = System.Configuration.ConfigurationManager.AppSettings("DestinationFilePath").ToString()

        '    For Each gvRow As GridViewRow In gvList.Rows
        '        Dim IsChecked As Boolean = CType(gvRow.FindControl("ChkSelectRow"), CheckBox).Checked
        '        VendorName = gvList.Rows(gvRow.RowIndex).Cells(4).Text.ToString().Trim()
        '        If IsChecked Then
        '            If Not VendorIdCompare.ToString().Contains(Session("VendorId").ToString()) Then
        '                VendorIdCompare.Append(Session("VendorId").ToString())
        '                InvoiceNo.Add(gvList.Rows(gvRow.RowIndex).Cells(9).Text.ToString().Trim())
        '            Else
        '                ZipFolderCreation = DestinationFilePath & Session("VendorId")
        '                InvoiceNo.Add(gvList.Rows(gvRow.RowIndex).Cells(9).Text.ToString().Trim())
        '                If Not Directory.Exists(ZipFolderCreation) Then
        '                    Directory.CreateDirectory(ZipFolderCreation)
        '                End If
        '                For Each Inv As String In InvoiceNo
        '                    If Not File.Exists(DestinationFilePath & Inv & ".pdf") Then
        '                        ErrorMessage("This file does not exist." & vbCrLf & "(" & DestinationFilePath & "\" & Inv & ".pdf" & ")")
        '                    Else
        '                        If Not File.Exists(ZipFolderCreation & "\" & Inv & ".pdf") Then
        '                            File.Copy(DestinationFilePath & Inv & ".pdf", ZipFolderCreation & "\" & Inv & ".pdf")
        '                        End If
        '                    End If
        '                Next
        '                If File.Exists(DestinationFilePath & "Invoice.zip") Then
        '                    File.Delete(DestinationFilePath & "Invoice.zip")
        '                End If
        '                ZipFile.CreateFromDirectory(ZipFolderCreation, DestinationFilePath & "Invoice.zip", CompressionLevel.Fastest, True)
        '            End If
        '        End If
        '    Next
        '    If Not InvoiceNo.Count > 1 Then
        '        If Not File.Exists(DestinationFilePath & InvoiceNo.FirstOrDefault() & ".pdf") Then
        '            ErrorMessageStr = "This file does not exist." & vbCrLf & "(" & DestinationFilePath & "\" & InvoiceNo.FirstOrDefault() & ".pdf" & ")"
        '        Else
        '            For Each Li As ListItem In chkVendorRole.Items
        '                If Li.Selected = True Then
        '                    If Not EmailCompare.ToString().Contains(GetVendorRoleByVendorId(DB, DB.Number(Session("VendorId")), Li.Value)) Then
        '                        EmailCompare.Append(GetVendorRoleByVendorId(DB, DB.Number(Session("VendorId")), Li.Value))
        '                        Email = GetVendorRoleByVendorId(DB, DB.Number(Session("VendorId")), Li.Value)
        '                        If Not Email = "" Then
        '                            Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
        '                            Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        '                            Core.SendSimpleMail(FromAddress, FromName, Email, "", "CBUSA Invoice " & InvoiceNo.FirstOrDefault(), "CBUSA Preferred Vendor - Please click on the attachment to view all outstanding rebate invoices. If you have any questions please contact the CBUSA team.", InvoiceNo.FirstOrDefault() & ".pdf", DestinationFilePath)
        '                            MessageStr = "Invoices have been sent" & HtmlNewLine
        '                        Else
        '                            ErrorMessageStr += "Role - " & Li.Text & " is not Available for the Vendor - " & VendorName & " " & HtmlNewLine
        '                        End If
        '                    End If
        '                End If
        '            Next
        '        End If
        '    Else
        '        For Each Li As ListItem In chkVendorRole.Items
        '            If Li.Selected = True Then
        '                If Not EmailCompare.ToString().Contains(GetVendorRoleByVendorId(DB, DB.Number(Session("VendorId")), Li.Value)) Then
        '                    EmailCompare.Append(GetVendorRoleByVendorId(DB, DB.Number(Session("VendorId")), Li.Value))
        '                    Email = GetVendorRoleByVendorId(DB, DB.Number(Session("VendorId")), Li.Value)
        '                    If Not Email = "" Then
        '                        Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
        '                        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        '                        Core.SendSimpleMail(FromAddress, FromName, Email, "", "CBUSA Invoices", "CBUSA Preferred Vendor - Please click on the attachment to view all outstanding rebate invoices. If you have any questions please contact the CBUSA team.", "Invoice.zip", DestinationFilePath)
        '                        MessageStr = "Invoices have been sent" & HtmlNewLine
        '                    Else
        '                        ErrorMessageStr += "Role - " & Li.Text & " is not Available for the Vendor - " & VendorName & " " & HtmlNewLine
        '                    End If
        '                End If
        '            End If
        '        Next
        '    End If

        '    _OperationType = "Email Generate"
        '    _CurrentUserId = Session("VendorId")
        '    UserName = Session("Username")
        '    _PageURL = Request.Url.ToString()

        '    lblErrorMsg.Text = MessageStr & HtmlNewLine & ErrorMessageStr
        '    Dim script As String = "window.onload = function() { OpenInvoiceForm(); };"
        '    ClientScript.RegisterStartupScript(Me.GetType(), lblErrorMsg.Text, script, True)

        '    Dim Obj As LogAudit = New LogAudit(_AuditTrailID, _ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, "Insert", StorageAccount, FileName, UserName)
        '    Obj.CallAuditAzureFunction(_ProjectName, _OperationDate, _ModuleName, _PageURL, _CurrentUserId, _OperationType, _ColumnName, _OldValue, _NewValue, FileName, UserName)

        '    For Each Li As ListItem In chkVendorRole.Items
        '        If Li.Selected = True Then
        '            Li.Selected = False
        '        End If
        '    Next

        '    CheckBoxAll.Checked = False
        '    Dim chkall As CheckBox = CType(gvList.HeaderRow.FindControl("ChkSelectAll"), CheckBox)
        '    chkall.Checked = False
        '    For Each gvRow As GridViewRow In gvList.Rows
        '        Dim IsChecked As CheckBox = CType(gvRow.FindControl("ChkSelectRow"), CheckBox)
        '        IsChecked.Checked = False
        '    Next
        'Catch ex As Exception
        '    LblFilePath.InnerText = ex.Message.ToString()
        'End Try
    End Sub
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
    Public Function GetVendorRoleByVendorId(ByVal DB As Database, ByVal VendorId As Integer, ByVal VendorRoleId As Integer) As String
        Dim sql As String = "select A.Email from VendorAccount a inner join VendorAccountVendorRole r " &
            "on a.VendorAccountID=r.VendorAccountID inner join VendorRole v on r.VendorRoleID=v.VendorRoleID where a.VendorID = " &
            DB.Number(VendorId) & " and v.VendorRoleID = " & DB.Number(VendorRoleId) & ""
        Dim Email As String = ""
        Dim sdr As SqlDataReader = DB.GetReader(sql)
        If sdr.Read Then
            Email = sdr("Email")
        End If
        sdr.Close()
        Return Email
    End Function
    Protected Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        BindGrid()
        gvList.PageIndex = 0

        'BindBuilderRebateGrids()
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnExport.Click
        'If Not IsValid Then Exit Sub
        ExportReport()
    End Sub
    Public Sub ExportReport()
        gvList.PageSize = 5000
        'Dim res As DataTable = LoadBuilderRebateGridsRecords()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Date, Transaction Type ,Rebate Program , Company Name , Amount")
        If dpDateLbound.Text Is "" And dpDateUbound.Text Is "" Then
            DtTo = DateTime.Now().Date
            DtFrom = DateAdd(DateInterval.Day, -60, DateTime.Now()).Date
        Else
            DtTo = dpDateUbound.Value
            DtFrom = dpDateLbound.Value
        End If
        If Session("BuilderId") Is Nothing Then
            BuilderId = 0
        Else
            BuilderId = Session("BuilderId")
        End If
        Dim HistoricId As Int32 = GetHistoricIdByBuilderId(BuilderId)
        DtRebate = BindGridDataSource(ResDb, HistoricId, DtFrom, DtTo)
        If DtRebate IsNot Nothing Then


            If DtRebate.Rows.Count > 0 Then

                For Each row As DataRow In DtRebate.Rows
                    Dim _Date As String = String.Empty
                    Dim TransactionType As String = String.Empty
                    Dim RebateProgram As String = String.Empty
                    Dim CompanyName As String = String.Empty
                    Dim Amount As String = String.Empty


                    Dim conc As String = String.Empty
                    'For Each id As String In VendorRow.GetLLCList(DB, row("VendorID")).Split(",")
                    '    If LLCRow.GetRow(DB, id).IsActive Then
                    '        Market &= conc & LLCRow.GetRow(DB, id).LLC
                    '        conc = ","
                    '    End If
                    'Next

                    'If Not IsDBNull(row("HistoricVendorID")) Then
                    '    Market = row("HistoricVendorID")
                    'End If
                    'If Not IsDBNull(row("VendorHistoricID")) Then
                    '    VendorID = row("VendorHistoricID")
                    'End If
                    'If Not IsDBNull(row("VendorName")) Then
                    '    VendorName = row("VendorName")
                    'End If

                    'Try
                    '    APContact = getNamesAccountPayables(DB, row("VendorID")).Rows(0)("FullName")
                    'Catch ex As Exception
                    '    APContact = String.Empty

                    'End Try

                    Try
                        If Not IsDBNull(row("DocumentDate")) Then
                            _Date = row("DocumentDate")
                        Else
                            _Date = ""
                        End If

                    Catch ex As Exception
                        _Date = String.Empty

                    End Try
                    Try
                        If Not IsDBNull(row("RebateType")) Then
                            TransactionType = row("RebateType")
                        Else
                            TransactionType = ""
                        End If

                    Catch ex As Exception
                        TransactionType = String.Empty

                    End Try
                    Try
                        If Not IsDBNull(row("ProductCategory")) Then
                            RebateProgram = row("ProductCategory")
                        Else
                            RebateProgram = ""
                        End If

                    Catch ex As Exception
                        RebateProgram = String.Empty

                    End Try
                    Try
                        If Not IsDBNull(row("CustomerName")) Then
                            CompanyName = row("CustomerName")
                        Else
                            CompanyName = ""
                        End If

                    Catch ex As Exception
                        CompanyName = String.Empty

                    End Try
                    Try
                        If Not IsDBNull(row("DocumentDate")) Then
                            _Date = row("DocumentDate")
                        Else
                            _Date = ""
                        End If

                    Catch ex As Exception
                        _Date = String.Empty

                    End Try
                    Try
                        If Not IsDBNull(row("TrxAmount")) Then
                            Amount = row("TrxAmount")
                        Else
                            Amount = ""
                        End If

                    Catch ex As Exception
                        Amount = String.Empty

                    End Try

                    sw.WriteLine(Core.QuoteCSV(_Date) & "," & Core.QuoteCSV(TransactionType) & "," & Core.QuoteCSV(RebateProgram) & "," & Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(Amount))
                Next
            End If
        End If
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)

    End Sub
    Protected Sub BtnViewFileARRebate_Click(sender As Object, e As ImageClickEventArgs)
        MsgBox("View clicked")
    End Sub
    Protected Sub BtnDownloadFileARRebate_Click(sender As Object, e As ImageClickEventArgs)
        MsgBox("Download clicked")
    End Sub


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
    'Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvList.PageIndexChanging
    '    gvList.PageIndex = e.NewPageIndex
    '    BindGrid()
    'End Sub
    Public Sub BindGrid(Optional ByVal sortExpression As String = Nothing)
        If (dpDateUbound.Text IsNot "" And dpDateLbound.Text IsNot "") Then
            DtTo = dpDateUbound.Value
            DtFrom = dpDateLbound.Value
        Else
            DtTo = DateTime.Now.Date
            DtFrom = DateAdd(DateInterval.Day, -60, DateTime.Now.Date).Date
        End If


        'If Convert.ToString(dpDateLbound.Value) Is Nothing Then

        'Else
        '    DtFrom = dpDateLbound.Value
        '    DtTo = dpDateUbound.Value
        'End If
        If Session("BuilderId") Is Nothing Then
            BuilderId = 0
        Else
            BuilderId = Session("BuilderId")
        End If
        Dim HistoricId As Int32 = GetHistoricIdByBuilderId(BuilderId)
        DtRebate = BindGridDataSource(ResDb, HistoricId, DtFrom, DtTo)
        'HttpContext.Current.Cache["secret key"] = DtRebate
        If (Not (sortExpression) Is Nothing) Then
            Dim dv As DataView = DtRebate.AsDataView
            Me.SortDirection = IIf(Me.SortDirection = "ASC", "DESC", "ASC")
            dv.Sort = sortExpression & " " & Me.SortDirection
            gvList.DataSource = dv
        Else
            gvList.DataSource = DtRebate
        End If
        'gvList.DataSource = DtRebate
        gvList.Pager.NofRecords = DtRebate.Rows.Count
        gvList.DataBind()

    End Sub

    Protected Sub gvList_Sorting(sender As Object, e As GridViewSortEventArgs)
        Me.BindGrid(e.SortExpression)
    End Sub
End Class
