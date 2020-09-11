Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports Components

Partial Class Email_VendorRebateNotice
    Inherits BasePage

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim sHistoricId = Request.QueryString("HistoricBuilderId").ToString()
        Dim sVendorId = Request.QueryString("VendorId").ToString()

        Dim sVendors As New System.Text.StringBuilder
        sVendors.Append("SELECT bldrid,vndrid,[vndrname], " & vbCrLf)
        sVendors.Append("       [invoice], " & vbCrLf)
        sVendors.Append("       [period] + ' - ' + [year] AS [Qtr/Yr], " & vbCrLf)
        sVendors.Append("       [date], " & vbCrLf)
        sVendors.Append("       [dayspastdue], " & vbCrLf)
        sVendors.Append("       [purchvol], " & vbCrLf)
        sVendors.Append("       [rebaterate], " & vbCrLf)
        sVendors.Append("       [amountdue] " & vbCrLf)
        sVendors.Append("FROM   [aec_arreport] " & vbCrLf)
        Dim sSubmittedVendors As String = sVendors.ToString() & " WHERE  bldrid =  " & sHistoricId & " AND vndrid = " & sVendorId & " ORDER BY vndrid"

        Dim dtVendors As DataTable = GetDataTableFromAccounting(sSubmittedVendors)
        InsertIntoTable(dtVendors)
        rptVendors.DataSource = dtVendors
        rptVendors.DataBind()

        sSubmittedVendors = sVendors.ToString() & " WHERE  bldrid <>  " & sHistoricId & " AND vndrid = " & sVendorId & " ORDER BY vndrid"
        rptOtherBuildersVendors.DataSource = GetDataTableFromAccounting(sSubmittedVendors)
        rptOtherBuildersVendors.DataBind()
    End Sub

    Private Sub InsertIntoTable(ByVal dtVendors As DataTable)

        For Each row As DataRow In dtVendors.Rows
            DB.ExecuteSQL("INSERT INTO RebateEmailSent (BLDRID,VNDRID,VNDRNAME,INVOICE,SubmittedDate) Values (" & row("bldrid") & "," & row("vndrid") & ",'" & row("vndrname") & "','" & row("invoice") & "','" & System.DateTime.Now() & "')")
        Next
    End Sub
    Private Function GetDataTableFromAccounting(ByVal sQuery As String) As DataTable
        Dim AccDB As New Database
        Try
            AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("AccountingConnectionString"), AppSettings("AccountingConnectionStringUsername"), AppSettings("AccountingConnectionStringPassword")))
            Return AccDB.GetDataTable(sQuery)
        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
            Return New DataTable
        Finally
            AccDB.Close()

        End Try
        Return New DataTable
    End Function

End Class
