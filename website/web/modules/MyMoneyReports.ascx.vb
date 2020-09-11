Option Strict Off

Imports Components
Imports DataLayer
Imports Controls
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.Linq

Partial Class _default
    Inherits ModuleControl

    Private AccDB As New Database
    Private qVendorInvoices As IEnumerable
    Private dtVendors As DataTable

    Private ReadOnly Property BuilderRebatesTable() As String
        Get
            Return SysParam.GetValue(DB, "ArmstrongBuilderRebatesTable")
        End Get
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        LoadVendors()
    End Sub


    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        

        If Not IsPostBack Then

            For var = 1 To 4 Step 1
                Me.drpStartQuarter.Items.Insert(var - 1, (New ListItem(var, var)))
                Me.drpEndQuarter.Items.Insert(var - 1, (New ListItem(var, var)))
            Next

            For var = 1 To 10 Step 1
                Me.drpStartYear.Items.Insert(var - 1, (New ListItem(Now.Year - 8 + var, Now.Year - 8 + var)))
                Me.drpEndYear.Items.Insert(var - 1, (New ListItem(Now.Year - 8 + var, Now.Year - 8 + var)))
            Next

            Me.drpStartQuarter.SelectedValue = ((Now.AddMonths(-3).Month - 1) \ 3) + 1
            Me.drpEndQuarter.SelectedValue = ((Now.AddMonths(3).Month - 1) \ 3) + 1

            Me.drpStartYear.SelectedValue = Now.AddMonths(-3).Year
            Me.drpEndYear.SelectedValue = Now.AddMonths(3).Year


            'Me.F_StartDate.Value = Now.Subtract(New TimeSpan(180, 0, 0, 0)).ToString("MM/dd/yyyy")
            'Me.F_EndDate.Value = Now.ToString("MM/dd/yyyy")

            LoadBuilderRebateGrids()

        End If


    End Sub

    Protected Sub LoadVendors()
        F_Vendor.WhereClause = "VendorID in (select l.VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID where b.BuilderID=" & DB.Number(Session("BuilderId")) & ")"

        Dim SQL As String = "SELECT v.VendorID, v.HistoricID, v.CompanyName FROM Vendor v JOIN LLCVendor lv ON v.VendorID = lv.VendorID JOIN Builder b ON lv.LLCID = b.LLCID WHERE b.BuilderID = " & DB.Number(Session("BuilderId")) & " order by CompanyName"
        dtVendors = DB.GetDataTable(SQL)

        'Dim item As ListItem

        ''Response.Write(SQL)


        'Me.F_Vendor.DataSource = dtVendors
        'Me.F_Vendor.DataValueField = "HistoricID"
        'Me.F_Vendor.DataTextField = "CompanyName"
        'Me.F_Vendor.DataBind()

        'If Not IsPostBack Then
        '    For Each item In Me.F_Vendor.Items
        '        item.Selected = True
        '    Next
        'End If

    End Sub

    Protected Sub LoadBuilderRebateGrids()

        Dim SQL As String = String.Empty
        Dim BufferSQL As String = String.Empty
        Dim dt As DataTable
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, Session("BuilderId"))

        If Me.drpEndYear.SelectedValue < Me.drpStartYear.SelectedValue Then
            Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue
            Me.drpEndQuarter.SelectedValue = Me.drpStartQuarter.SelectedValue
        ElseIf Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue And Me.drpEndQuarter.SelectedValue < Me.drpStartQuarter.SelectedValue Then
            Me.drpEndYear.SelectedValue = Me.drpStartYear.SelectedValue
            Me.drpEndQuarter.SelectedValue = Me.drpStartQuarter.SelectedValue
        End If

        Dim StartDate As DateTime
        If Me.drpStartQuarter.SelectedValue = "1" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 1, 1)
        ElseIf Me.drpStartQuarter.SelectedValue = "2" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 4, 1)
        ElseIf Me.drpStartQuarter.SelectedValue = "3" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 7, 1)
        ElseIf Me.drpStartQuarter.SelectedValue = "4" Then
            StartDate = New DateTime(drpStartYear.SelectedValue, 10, 1)
        Else
            StartDate = New DateTime(Year(Now()), 10, 1)
        End If

        Dim EndDate As DateTime
        If Me.drpEndQuarter.SelectedValue = "1" Then
            EndDate = New DateTime(drpEndYear.SelectedValue, 4, 1)
        ElseIf Me.drpEndQuarter.SelectedValue = "2" Then
            EndDate = New DateTime(drpEndYear.SelectedValue, 7, 1)
        ElseIf Me.drpEndQuarter.SelectedValue = "3" Then
            EndDate = New DateTime(drpEndYear.SelectedValue, 10, 1)
        ElseIf Me.drpEndQuarter.SelectedValue = "4" Then
            EndDate = New DateTime(drpEndYear.SelectedValue + 1, 1, 1)
        Else
            EndDate = New DateTime(Year(Now()) + 1, 1, 1)
        End If
        EndDate = EndDate.AddDays(-1)

        'Me.AccDB.BeginTransaction()

        Try
            AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))

            'Builder Rebates grid

            SQL = _
                " select " _
                & "     VNDRID as VendorHistoricID," _
                & "     RBATEINVOICE as InvoiceNumber," _
                & "     BLDYEAR AS PeriodYear," _
                & "     BLDPERIOD AS PeriodQuarter," _
                & "     SUM(COALESCE(PURCHVOLCUR,0)) as PurchaseVolume," _
                & "     SUM(COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0)) AS TotalInvoiced," _
                & "     SUM(COALESCE(RBATPAID,0)) AS TotalCollected," _
                & "     SUM(COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0)) - SUM(COALESCE(RBATPAID,0)) AS TotalDue" _
                & " from " _
                & "     cbusa_BuilderRebates" _
                & " where " _
                & "     LLCID=(SELECT TOP 1 STRING1 FROM VBCloud_Param WHERE CODE=1 AND LONGINT1=" & DB.Number(dbBuilder.LLCID) & ")" _
                & " and " _
                & "     CAST((CAST((BLDPERIOD - 1) * 3 + 1 AS varchar) + '/1/' + CAST(BLDYEAR AS varchar)) as DateTime) >= " & DB.Quote(StartDate) _
                & " and " _
                & "     CAST((CAST((BLDPERIOD - 1) * 3 + 1 AS varchar) + '/1/' + CAST(BLDYEAR AS varchar)) as DateTime) <= " & DB.Quote(EndDate) _
                & " and " _
                & "     BLDRID=" & DB.Number(dbBuilder.HistoricID)

            If F_Vendor.Value <> Nothing Then
                SQL &= " and VNDRID = " & DB.NumberMultiple(F_Vendor.Value)
            End If

            SQL &= " group by RBATEINVOICE, VNDRID, BLDYEAR, BLDPERIOD"

            dt = AccDB.GetDataTable(SQL)

            qVendorInvoices = (From dr As DataRow In dt.AsEnumerable Join dv As DataRow In dtVendors.AsEnumerable On Core.GetInt(dr("VendorHistoricID")) Equals Core.GetInt(dv("HistoricID")) Order By dv("CompanyName") _
                               Select New With { _
                                    .CompanyName = dv("CompanyName"), _
                                    .InvoiceNumber = dr("InvoiceNumber"), _
                                    .PeriodYear = dr("PeriodYear"), _
                                    .PeriodQuarter = dr("PeriodQuarter"), _
                                    .PurchaseVolume = dr("PurchaseVolume"), _
                                    .TotalInvoiced = dr("TotalInvoiced"), _
                                    .TotalCollected = dr("TotalCollected"), _
                                    .TotalDue = dr("TotalDue") _
                                } _
                               )

            SQL = _
                " select " _
                & "     BLDYEAR AS PeriodYear,"

            SQL &= "    BLDPERIOD AS PeriodQuarter,"

            SQL &= _
                  "     SUM(COALESCE(PURCHVOLCUR,0)) as PurchaseVolume," _
                & "     SUM(COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0)) AS TotalInvoiced," _
                & "     SUM(COALESCE(RBATPAID,0)) AS TotalCollected," _
                & "     SUM(COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0)) - SUM(COALESCE(RBATPAID,0)) AS TotalDue" _
                & " from " _
                & "     cbusa_BuilderRebates" _
                & " where " _
                & "     LLCID=(SELECT TOP 1 STRING1 FROM VBCloud_Param WHERE CODE=1 AND LONGINT1=" & DB.Number(dbBuilder.LLCID) & ")" _
                & " and " _
                & "     CAST((CAST((BLDPERIOD - 1) * 3 + 1 AS varchar) + '/1/' + CAST(BLDYEAR AS varchar)) as DateTime) >= " & DB.Quote(StartDate) _
                & " and " _
                & "     CAST((CAST((BLDPERIOD - 1) * 3 + 1 AS varchar) + '/1/' + CAST(BLDYEAR AS varchar)) as DateTime) <= " & DB.Quote(EndDate) _
                & " and " _
                & "     BLDRID=" & DB.Number(dbBuilder.HistoricID)


            If F_Vendor.Value <> Nothing Then
                SQL &= " and VNDRID in " & DB.NumberMultiple(F_Vendor.Value)
            End If

            SQL &= " group by BLDYEAR, BLDPERIOD"

            dt = Me.AccDB.GetDataTable(SQL)

            rptQuarters.DataSource = dt
            rptQuarters.DataBind()


            'All LLC Rebates grid

            SQL = _
                " select " _
                & "     VNDRID as VendorHistoricID," _
                & "     RBATEINVOICE as InvoiceNumber," _
                & "     RBATEINVDATE as InvoiceDate," _
                & "     BLDYEAR as PeriodYear," _
                & "     BLDPERIOD as PeriodQuarter," _
                & "     SUM(COALESCE(PURCHVOLCUR,0)) as PurchaseVolume," _
                & "     SUM(COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0)) as TotalInvoiced," _
                & "     SUM(COALESCE(RBATPAID,0)) as TotalCollected," _
                & "     SUM(COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0)) - SUM(COALESCE(RBATPAID,0)) as TotalDue" _
                & " from " _
                & "     cbusa_BuilderRebates" _
                & " where " _
                & "     LLCID=(SELECT TOP 1 STRING1 FROM VBCloud_Param WHERE CODE=1 AND LONGINT1=" & DB.Number(dbBuilder.LLCID) & ")" _
                & " and " _
                & "     (COALESCE(REBATEAMT,0) + COALESCE(CUSTRBATAMT,0) + COALESCE(RBATADJAMT,0) - COALESCE(RBATPAID,0)) > 0" _
                & " group by " _
                & "     VNDRID, RBATEINVDATE, RBATEINVOICE, BLDYEAR, BLDPERIOD" _
                & " order by BLDYEAR, BLDPERIOD"



            dt = AccDB.GetDataTable(SQL)
            gvLLCRebates.DataSource = (From dr As DataRow In dt.AsEnumerable Join dv As DataRow In dtVendors.AsEnumerable On Core.GetInt(dr("VendorHistoricID")) Equals Core.GetInt(dv("HistoricID")) Order By dv("CompanyName") _
                                       Select New With { _
                                            .InvoiceNumber = dr("InvoiceNumber"), _
                                            .InvoiceDate = dr("InvoiceDate"), _
                                            .InvoiceDueDate = DateAdd(DateInterval.Day, 30, dr("InvoiceDate")), _
                                            .PeriodQuarter = dr("PeriodQuarter"), _
                                            .PeriodYear = dr("PeriodYear"), _
                                            .TotalDue = dr("TotalDue"), _
                                            .CompanyName = dv("CompanyName") _
                                        } _
                                       )
            gvLLCRebates.DataBind()
        Catch ex As SqlException
            If AccDB IsNot Nothing AndAlso AccDB.Transaction IsNot Nothing Then AccDB.RollbackTransaction()
            Logger.Error(Logger.GetErrorMessage(ex))
            AddError(ErrHandler.ErrorText(ex))
        Finally
            If AccDB IsNot Nothing AndAlso AccDB.IsOpen Then
                AccDB.Close()
            End If
        End Try
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        LoadBuilderRebateGrids()
    End Sub

    Protected Sub rptQuarters_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptQuarters.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim gvBuilderRebates As GridView = e.Item.FindControl("gvBuilderRebates")
        gvBuilderRebates.DataSource = From i In qVendorInvoices Where i.PeriodYear = e.Item.DataItem("PeriodYear") And i.PeriodQuarter = e.Item.DataItem("PeriodQuarter") Select i
        gvBuilderRebates.DataBind()

        gvBuilderRebates.FooterRow.Cells(1).Controls.Add(New LiteralControl("<b>QUARTER TOTALS</b>"))
        gvBuilderRebates.FooterRow.Cells(4).Controls.Add(New LiteralControl(FormatCurrency(e.Item.DataItem("PurchaseVolume"))))
        gvBuilderRebates.FooterRow.Cells(5).Controls.Add(New LiteralControl(FormatCurrency(e.Item.DataItem("TotalInvoiced"))))
        gvBuilderRebates.FooterRow.Cells(6).Controls.Add(New LiteralControl(FormatCurrency(e.Item.DataItem("TotalCollected"))))
        gvBuilderRebates.FooterRow.Cells(7).Controls.Add(New LiteralControl(FormatCurrency(e.Item.DataItem("TotalDue"))))
        gvBuilderRebates.FooterRow.Visible = True
    End Sub
End Class
