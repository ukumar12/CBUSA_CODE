Imports Components
Imports DataLayer
Imports PopupForm
Imports System.Linq
Imports System.Data
Imports System.Web.Services
Imports System.Configuration.ConfigurationManager

Partial Class RebateNotificationHistory
    Inherits SitePage

    Protected sHistoricId As String = "0"
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()
        sHistoricId = DB.ExecuteScalar("SELECT historicId FROM Builder WHERE builderId=" & Session("BuilderId"))
        '  sHistoricId = "2204"
        If Not IsPostBack Then
            BindVendors()
        End If

    End Sub

    Private Sub BindVendors()
        Dim sVendors As New System.Text.StringBuilder
        sVendors.Append("SELECT DISTINCT([vndrid]) AS vndrid,vndrname FROM RebateEmailSent   " & vbCrLf)
        sVendors.Append("WHERE IsAutoEmailTask <> 1 AND   bldrid =  " & sHistoricId & " ORDER BY vndrid")
        rptVendors.DataSource = DB.GetDataTable(sVendors.ToString())
        rptVendors.DataBind()

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

    Protected Sub rptVendors_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptVendors.ItemDataBound
        
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

            Dim sVendorId As String = CType(e.Item.FindControl("litVendorId"), Literal).Text
            Dim rptSubmittedDate As Repeater = CType(e.Item.FindControl("rptSubmittedDate"), Repeater)
            'Dim rptGroup As Repeater = CType(e.Item.FindControl("rptGroup"), Repeater)
            rptSubmittedDate.DataSource = DB.GetDataTable("select DISTINCT Convert(nvarchar, SubmittedDate,110) AS SubmittedDate from RebateEmailSent where bldrid =  " & sHistoricId & " AND IsAutoEmailTask <> 1 AND VNDRID = " & sVendorId & " ORDER BY SubmittedDate")
            rptSubmittedDate.DataBind()

            For Each item As RepeaterItem In rptSubmittedDate.Items
                '   rptGroup.DataSource = DB.GetDataTable("select DISTINCT GroupID from RebateEmailSent where bldrid =  " & sHistoricId & " AND VNDRID = " & sVendorId & " ORDER BY SubmittedDate")
                '  rptGroup.DataBind()
                Dim sSubmittedDate As String = CType(item.FindControl("litSubmittedDate"), Literal).Text
                Dim rptReportByGroup As Repeater = CType(item.FindControl("rptReportByGroup"), Repeater)


                rptReportByGroup.DataSource = DB.GetDataTable("select DISTINCT GroupId from RebateEmailSent where bldrid =  " & sHistoricId & " AND VNDRID = " & sVendorId & " AND IsAutoEmailTask <> 1 AND convert(nvarchar, submittedDate,110) = '" & sSubmittedDate & "'")
                rptReportByGroup.DataBind()

                For Each groupItem As RepeaterItem In rptReportByGroup.Items
                    
                    Dim sGroupId As String = CType(groupItem.FindControl("litGroupId"), Literal).Text

                    Dim rptReport As Repeater = CType(groupItem.FindControl("rptReport"), Repeater)
                    rptReport.DataSource = DB.GetDataTable("select * from RebateEmailSent where bldrid =  " & sHistoricId & " AND VNDRID = " & sVendorId & " AND IsAutoEmailTask <> 1 AND convert(nvarchar, submittedDate,110) = '" & sSubmittedDate & "' AND GroupId = " & sGroupId)
                    rptReport.DataBind()
                Next
            Next
        End If
    End Sub
    Protected Sub btnBack_Click(sender As Object, e As System.EventArgs) Handles btnBack.Click
        Response.Redirect("Rebate-Notification.aspx")
    End Sub

End Class
