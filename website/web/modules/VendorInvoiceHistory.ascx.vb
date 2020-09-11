Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Partial Class modules_VendorInvoiceHistory
    Inherits ModuleControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        gvInvoices.BindList = AddressOf BindData
        If Not IsPostBack And Session("VendorId") IsNot Nothing Then
            BindData()
        End If
    End Sub

    Private Sub BindData()
        Dim arDB As New Database()
        Try
            arDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))

            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))

            Dim sqlFields As String = _
                " select " _
                & "DATEEDITED as EditDate," _
                & "RBATEINVOICE as InvoiceNumber," _
                & "Ceiling(BLDPERIOD / 3) as PeriodQuarter," _
                & "BLDYEAR as PeriodYear," _
                & "(REBATEAMT + CUSTRBATAMT + RBATADJAMT) as TotalAmount," _
                & "RBATPAID as AmountPaid"
            Dim sql As String = _
                  " from " _
                & " cbusa_BuilderRebates " _
                & " where " _
                & " VNDRID=" & DB.Number(dbVendor.HistoricID)

            Dim cnt As Integer = arDB.ExecuteScalar("select count(*) " & sql)
            Dim sortby As String = IIf(gvInvoices.SortBy = String.Empty, "EditDate", gvInvoices.SortBy)
            Dim sortorder As String = IIf(gvInvoices.SortOrder = String.Empty, "Desc", gvInvoices.SortOrder)

            Dim dt As DataTable = arDB.GetDataTable(sqlFields & sql & " order by " & sortby & " " & sortorder)
            gvInvoices.DataSource = dt
            gvInvoices.Pager.NofRecords = cnt
            gvInvoices.DataBind()

        Finally
            If arDB IsNot Nothing AndAlso arDB.IsOpen Then
                arDB.Close()
            End If
        End Try
    End Sub
End Class
