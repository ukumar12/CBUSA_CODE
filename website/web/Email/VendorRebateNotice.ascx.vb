Option Strict Off
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports Components
 
Partial Class Email_VendorRebateNotice
    Inherits ModuleControl

    Dim sHistoricVendorId, sHistoricBuilderId As String
    Protected sBuilderName As String
    Protected sVendorName As String
    Protected sumBuilder As String
    Protected sumAllBuilder, sBuilderID As String
    Protected sBuilderAccountEmail As String
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        

        If Not IsAdminDisplay Then
           

            sHistoricBuilderId = Request.QueryString("HistoricBuilderId").ToString()
            sBuilderName = DB.ExecuteScalar("SELECT Top 1 CompanyName FROM Builder WHERE historicId=" & sHistoricBuilderId)
            sVendorName = DB.ExecuteScalar("SELECT Top 1 CompanyName FROM Vendor WHERE historicId=" & Request.QueryString("VendorId").ToString())
            VendorName.Text = sVendorName
            sBuilderID = DB.ExecuteScalar("SELECT Top 1 BuilderID FROM Builder WHERE historicId=" & sHistoricBuilderId)
            Dim dbLLC As LLCRow = LLCRow.GetBuilderLLC(DB, sBuilderID)
            litllcOperationsManager.Text = dbLLC.OperationsManager
            ltlBuilderGroup.Text = sBuilderName
            If Not IsPostBack Then

                BindList()
            End If

            If Not Request("isPreview") = String.Empty Then
                btnSend.Visible = True
            End If


        End If

    End Sub

    Private Sub BindList()
        Dim sHistoricBuilderId = Request.QueryString("HistoricBuilderId").ToString()
        sHistoricVendorId = Request.QueryString("VendorId").ToString()

        Dim sVendors As New System.Text.StringBuilder
        sVendors.Append("SELECT " & vbCrLf)
        sVendors.Append("       [invoice], " & vbCrLf)
        sVendors.Append("       [PERIOD], " & vbCrLf)
        sVendors.Append("       [YEAR], " & vbCrLf)
        sVendors.Append("       [DATE], " & vbCrLf)
        sVendors.Append("       [dayspastdue], " & vbCrLf)
        sVendors.Append("       SUM(AmountDue) AS AmountDue " & vbCrLf)
        sVendors.Append("FROM   [RG_ARReport] " & vbCrLf)
        Dim sSubmittedVendors As String = sVendors.ToString() & " WHERE  DaysPastDue >= 30 AND INVOICE IN (SELECT INVOICE FROM RG_ARReport WHERE bldrid =  " & sHistoricBuilderId & " AND vndrid = " & sHistoricVendorId & " ) GROUP BY INVOICE,PERIOD,YEAR,DATE,DaysPastDue"

        Dim dtVendors As DataTable = GetDataTableFromAccounting(sSubmittedVendors)

        sumBuilder = Math.Round(dtVendors.Compute("SUM(amountdue)", ""), 2)


        If Request.QueryString("isPreview") Is Nothing Then
            InsertIntoTable(dtVendors)
        Else

            rptOtherBuildersVendors.Visible = False
        End If

        rptVendors.DataSource = dtVendors
        rptVendors.DataBind()
        'sSubmittedVendors = sVendors.ToString() & " WHERE INVOICE IN (SELECT INVOICE FROM RG_ARReport WHERE bldrid <>  " & sHistoricBuilderId & " AND vndrid = " & sHistoricVendorId & " ) GROUP BY INVOICE,PERIOD,YEAR,DATE,DaysPastDue"
        '' sSubmittedVendors = sVendors.ToString() & " WHERE  bldrid <>  " & sHistoricBuilderId & " AND vndrid = " & sHistoricVendorId & " ORDER BY vndrid"
        'Dim dtOtherBuilders As DataTable = GetDataTableFromAccounting(sSubmittedVendors)
        'If dtOtherBuilders.Rows.Count > 0 Then
        '    sumAllBuilder = Math.Round(dtOtherBuilders.Compute("SUM(amountdue)", ""), 2)
        '    rptOtherBuildersVendors.DataSource = dtOtherBuilders
        '    rptOtherBuildersVendors.DataBind()
        'End If

    End Sub

    Private Sub InsertIntoTable(ByVal dtVendors As DataTable)
        Dim sVendorId = DB.ExecuteScalar("SELECT vendorid FROM Vendor WHERE historicId=" & sHistoricVendorId)
        Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 WHERE t1.VendorID = t2.VendorID AND t1.IsActive = 1 FOR XML PATH('')),1,1,'') as varchar(max)) from Vendor t2 where t2.VendorID = " & sVendorId)
        Dim MaxGroupID As Integer = DB.ExecuteScalar("select top 1 GroupID from RebateEmailSent order by GroupID desc")
        MaxGroupID += 1
        For Each row As DataRow In dtVendors.Rows
            DB.ExecuteSQL("INSERT INTO RebateEmailSent (BLDRID,VNDRID,VNDRNAME,INVOICE,AECReportDate,Period,Year,daysPastDue,AmountDue,SubmittedDate, GroupID,Email) Values (" & sHistoricBuilderId & "," & sHistoricVendorId & "," & DB.Quote(sVendorName) & ",'" & row("invoice") & "','" & row("Date") & "','" & row("Period") & "','" & row("Year") & "','" & row("DaysPastDue") & "','" & row("AmountDue") & "','" & System.DateTime.Now() & "','" & MaxGroupID & "','" & sVendorEmails & "')")
        Next
    End Sub
    Private Function GetDataTableFromAccounting(ByVal sQuery As String) As DataTable
        Dim resdb As New Database
        Try
            ResDb.Open(DBConnectionString.GetConnectionString(AppSettings("ResgroupConnectionString"), AppSettings("ResgroupConnectionStringUsername"), AppSettings("ResgroupConnectionStringPassword")))
            Return ResDb.GetDataTable(sQuery)
        Catch ex As Exception
            AddError(ErrHandler.ErrorText(ex))
            Return New DataTable
        Finally
            ResDb.Close()

        End Try
        Return New DataTable
    End Function

   
    Protected Sub btnSend_Click(sender As Object, e As System.EventArgs) Handles btnSend.Click
        If Session("BuilderId") Is Nothing Or Session("BuilderAccountId") Is Nothing Then
            Response.Redirect(AppSettings("GlobalSecureName") & "/default.aspx")
        End If
        Dim sHistoricBuilderId = Request.QueryString("HistoricBuilderId").ToString()
        sHistoricVendorId = Request.QueryString("VendorId").ToString()
        sBuilderAccountEmail = DB.ExecuteScalar("SELECT Email FROM BuilderAccount WHERE BuilderAccountId=" & Session("BuilderAccountID"))
        Dim Url As String = IIf(AppSettings("GlobalSecureName") = Nothing, AppSettings("GlobalRefererName"), AppSettings("GlobalSecureName")) & "/Email/VendorRebateNotice.aspx?HistoricBuilderId=" & sHistoricBuilderId & "&VendorId=" & sHistoricVendorId

        Dim sMsg As String = Core.GetRenderedHtml(Url)
        'sMsg = Replace(sMsg, "arTotal", "arTotal1")

        Dim sSubject As String = "Past Due CBUSA Rebates"

        Dim sVendorIds = DB.ExecuteScalar("SELECT vendorid FROM Vendor WHERE historicId=" & sHistoricVendorId)
        'Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 WHERE t1.isactive = 1 and t1.VendorID = t2.VendorID FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + t2.Email from Vendor t2 where t2.VendorID = " & sVendorIds)
        'Dim sVendorEmails As String = DB.ExecuteScalar("select CAST(stuff((SELECT ',' + t1.Email FROM VendorAccount t1 join VendorAccountVendorRole t3 on t1.VendorAccountid= t3.vendoraccountid WHERE t1.VendorID = t2.VendorID AND t1.IsActive = 1 and t3.VendorRoleid in (1,5) FOR XML PATH('')),1,1,'') as varchar(max)) + ', ' + t2.Email from Vendor t2 where t2.VendorID =  " & sVendorIds)
        Dim sVendorEmails As String = DB.ExecuteScalar("SELECT COALESCE(STUFF((SELECT ',' + VendorEmail.Email FROM(SELECT DISTINCT VA.Email FROM VendorAccount VA JOIN VendorAccountVendorRole VAVR ON VA.VendorAccountID = VAVR.VendorAccountID WHERE VA.VendorID = " & sVendorIds & " AND VA.IsActive = 1 AND VAVR.VendorRoleID IN (1, 5)) AS VendorEmail FOR XML PATH('')),1,1,''), 'NA') VendorEmails")

        Dim llcBuilderRow As LLCRow = LLCRow.GetBuilderLLC(DB, Session("BuilderId"))
        Dim addr As String = SysParam.GetValue(DB, "RebateNotificationList")
        Dim addrCC As String = SysParam.GetValue(DB, "RebateNotificationListCC")

        If sVendorEmails <> "NA" Then
            Dim arr As String() = Split(sVendorEmails, ",")
            Dim iCounter As Integer = 0

            For Each sEmail As String In arr
                If iCounter = 0 Then
                    Core.SendHTMLMail(sBuilderAccountEmail, sBuilderName, sEmail, sEmail, sSubject, sMsg, llcBuilderRow.NotificationEmailList & IIf(addrCC <> String.Empty, "," & addrCC, ""), addr)
                Else
                    Core.SendHTMLMail(sBuilderAccountEmail, sBuilderName, sEmail, sEmail, sSubject, sMsg)
                End If

                iCounter = iCounter + 1
            Next
        End If

        ' Core.SendHTMLMailToMultipleRecipient(sBuilderAccountEmail, sBuilderName, sVendorEmails, sVendorEmails, sSubject, sMsg, IIf(llcBuilderRow.NotificationEmailList <> Nothing, llcBuilderRow.NotificationEmailList, "") & IIf(addrCC <> String.Empty, "," & addrCC, ""), addr)

        Response.Redirect("~/rebates/Rebate-Notification.aspx?c=y")

    End Sub
End Class
