Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SalesReportDisputeID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("SALES_REPORT_DISPUTES")

		SalesReportDisputeID = Convert.ToInt32(Request("SalesReportDisputeID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
        drpDisputeResponseID.Datasource = DisputeResponseRow.GetList(DB, "DisputeResponse")
        drpDisputeResponseID.DataValueField = "DisputeResponseID"
        drpDisputeResponseID.DataTextField = "DisputeResponse"
        drpDisputeResponseID.Databind()
        drpDisputeResponseID.Items.Insert(0, New ListItem("", ""))

        drpDisputeResponseReasonID.Datasource = DisputeResponseReasonRow.GetList(DB, "DisputeResponseReason")
        drpDisputeResponseReasonID.DataValueField = "DisputeResponseReasonID"
        drpDisputeResponseReasonID.DataTextField = "DisputeResponseReason"
        drpDisputeResponseReasonID.Databind()
        drpDisputeResponseReasonID.Items.Insert(0, New ListItem("", ""))

        If SalesReportDisputeID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbSalesReportDispute As SalesReportDisputeRow = SalesReportDisputeRow.GetRow(DB, SalesReportDisputeID)
        txtSalesReportID.Text = dbSalesReportDispute.SalesReportID
        txtBuilderTotalAmount.Text = dbSalesReportDispute.BuilderTotalAmount
        txtVendorTotalAmount.Text = dbSalesReportDispute.VendorTotalAmount
        txtResolutionAmount.Text = dbSalesReportDispute.ResolutionAmount
        txtBuilderComments.Text = dbSalesReportDispute.BuilderComments
        txtVendorComments.Text = dbSalesReportDispute.VendorComments
        drpDisputeResponseID.SelectedValue = dbSalesReportDispute.DisputeResponseID
        drpDisputeResponseReasonID.SelectedValue = dbSalesReportDispute.DisputeResponseReasonID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbSalesReportDispute As SalesReportDisputeRow

            If SalesReportDisputeID <> 0 Then
                dbSalesReportDispute = SalesReportDisputeRow.GetRow(DB, SalesReportDisputeID)
            Else
                dbSalesReportDispute = New SalesReportDisputeRow(DB)
            End If
            dbSalesReportDispute.SalesReportID = txtSalesReportID.Text
            dbSalesReportDispute.BuilderTotalAmount = txtBuilderTotalAmount.Text
            dbSalesReportDispute.VendorTotalAmount = txtVendorTotalAmount.Text
            dbSalesReportDispute.ResolutionAmount = txtResolutionAmount.Text
            dbSalesReportDispute.BuilderComments = txtBuilderComments.Text
            dbSalesReportDispute.VendorComments = txtVendorComments.Text
            dbSalesReportDispute.DisputeResponseID = drpDisputeResponseID.SelectedValue
            dbSalesReportDispute.DisputeResponseReasonID = drpDisputeResponseReasonID.SelectedValue

            If SalesReportDisputeID <> 0 Then
                dbSalesReportDispute.Update()
            Else
                SalesReportDisputeID = dbSalesReportDispute.Insert
            End If

            DB.CommitTransaction()


            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

	Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
		Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub

	Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
		Response.Redirect("delete.aspx?SalesReportDisputeID=" & SalesReportDisputeID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
