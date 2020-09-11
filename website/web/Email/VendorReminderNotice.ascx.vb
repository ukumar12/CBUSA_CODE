 
Option Strict Off
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports Components

Partial Class Email_VendorReminderNotice
    Inherits ModuleControl

    Dim sHistoricVendorId As String
    Protected sBuilderName As String
    Protected sVendorName As String
    Protected sumBuilder As String
    Protected sumAllBuilder As String

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsAdminDisplay Then
            Dim sHistoricVendorId As String = Request.QueryString("HistoricVendorId").ToString()
            Dim HistoricBuilderIdByGroup As String = Request.QueryString("HistoricBuilderIdByGroup").ToString()
            sVendorName = DB.ExecuteScalar("SELECT Top 1 CompanyName FROM Vendor WHERE historicId=" & sHistoricVendorId)
            VendorName.Text = sVendorName
            If Not IsPostBack Then
                BindList()
            End If
        End If

    End Sub

    Private Sub BindList()
        Dim sHistoricVendorId As String = Request.QueryString("HistoricVendorId").ToString()
        Dim HistoricBuilderIdByGroup As String = Request.QueryString("HistoricBuilderIdByGroup").ToString()
        'Dim sVendors As New System.Text.StringBuilder
        'sVendors.Append("SELECT bldrid,vndrid,[vndrname], " & vbCrLf)
        'sVendors.Append("       [invoice], " & vbCrLf)
        'sVendors.Append("       [period] + ' - ' + [year] AS [Qtr/Yr], " & vbCrLf)
        'sVendors.Append("       [date], " & vbCrLf)
        'sVendors.Append("       [Period], " & vbCrLf)
        'sVendors.Append("       [Year], " & vbCrLf)
        'sVendors.Append("       [dayspastdue], " & vbCrLf)
        'sVendors.Append("       [purchvol], " & vbCrLf)
        'sVendors.Append("       [rebaterate], " & vbCrLf)
        'sVendors.Append("       [amountdue] " & vbCrLf)
        'sVendors.Append("FROM   [aec_arreport] " & vbCrLf)
        'Dim sSubmittedVendors As String = sVendors.ToString() & " WHERE    vndrid = " & sHistoricVendorId & " AND BLDRID in (" & HistoricBuilderIdByGroup & ")"

        Dim sVendors As New System.Text.StringBuilder
        sVendors.Append("SELECT " & vbCrLf)
        sVendors.Append("       [invoice], " & vbCrLf)
        sVendors.Append("       [PERIOD], " & vbCrLf)
        sVendors.Append("       [YEAR], " & vbCrLf)
        sVendors.Append("       [DATE], " & vbCrLf)
        sVendors.Append("       [dayspastdue], " & vbCrLf)
        sVendors.Append("       SUM(AmountDue) AS AmountDue " & vbCrLf)
        sVendors.Append("FROM   [RG_ARReport] " & vbCrLf)
        Dim sSubmittedVendors As String = sVendors.ToString() & " WHERE dayspastdue > 0 AND INVOICE IN (SELECT INVOICE FROM RG_ARReport WHERE bldrid in (  " & HistoricBuilderIdByGroup & " ) AND vndrid = " & sHistoricVendorId & " ) GROUP BY INVOICE,PERIOD,YEAR,DATE,DaysPastDue"


        Dim dtVendors As DataTable = GetDataTableFromAccounting(sSubmittedVendors)

        sumBuilder = Math.Round(dtVendors.Compute("SUM(amountdue)", ""), 2)


        'If Request.QueryString("isPreview") Is Nothing Then
        '    '  InsertIntoTable(dtVendors)

        'End If

        rptVendors.DataSource = dtVendors
        rptVendors.DataBind()

      
    End Sub

    Private Sub InsertIntoTable(ByVal dtVendors As DataTable)
        Dim sHistoricVendorId As String = Request.QueryString("HistoricVendorId").ToString()
        Dim sVendorId = DB.ExecuteScalar("SELECT vendorid FROM Vendor WHERE historicId=" & sHistoricVendorId)
        Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 WHERE t1.VendorID = t2.VendorID FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + t2.Email from Vendor t2 where t2.VendorID = " & sVendorId)

        For Each row As DataRow In dtVendors.Rows
            DB.ExecuteSQL("INSERT INTO RebateEmailSent (BLDRID,VNDRID,INVOICE,AECReportDate,Period,Year,daysPastDue,purchVol,RebateRate,AmountDue,SubmittedDate,isAutoEmailTask, Email) Values (" & row("bldrid") & "," & row("vndrid") & ",'" & row("invoice") & "','" & row("Date") & "','" & row("Period") & "','" & row("Year") & "','" & row("DaysPastDue") & "','" & row("PurchVol") & "','" & row("RebateRate") & "','" & row("AmountDue") & "','" & System.DateTime.Now() & "','" & True & "','" & sVendorEmails & "')")
        Next
    End Sub
    Private Function GetDataTableFromAccounting(ByVal sQuery As String) As DataTable
        Dim AccDB As New Database
        Try
            AccDB.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
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
