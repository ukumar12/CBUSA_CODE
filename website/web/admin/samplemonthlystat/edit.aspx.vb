Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
	Inherits AdminPage

	Protected SampleMonthlyStatID As Integer

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckAccess("SAMPLE_MONTHLY_STATS")

		SampleMonthlyStatID = Convert.ToInt32(Request("SampleMonthlyStatID"))
		If Not IsPostBack Then
			LoadFromDB()
		End If
	End Sub

	Private Sub LoadFromDB()
		If SampleMonthlyStatID = 0 Then
			btnDelete.Visible = False
			Exit Sub
		End if

		Dim dbSampleMonthlyStat As SampleMonthlyStatRow = SampleMonthlyStatRow.GetRow(DB, SampleMonthlyStatID)
        txtStartedUnits.Text = dbSampleMonthlyStat.StartedUnits
		txtSoldUnits.Text = dbSampleMonthlyStat.SoldUnits
		txtClosingUnits.Text = dbSampleMonthlyStat.ClosingUnits
		txtUnsoldUnits.Text = dbSampleMonthlyStat.UnsoldUnits
		dtTimePeriodDate.Value = dbSampleMonthlyStat.TimePeriodDate
	End Sub

	Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
		If Not IsValid Then Exit Sub
		Try
			DB.BeginTransaction()

			Dim dbSampleMonthlyStat As SampleMonthlyStatRow

			If SampleMonthlyStatID <> 0 Then
				dbSampleMonthlyStat = SampleMonthlyStatRow.GetRow(DB, SampleMonthlyStatID)
			Else
				dbSampleMonthlyStat = New SampleMonthlyStatRow(DB)
			End If
            dbSampleMonthlyStat.Year = dtTimePeriodDate.Value.Year
            dbSampleMonthlyStat.Month = dtTimePeriodDate.Value.Month
			dbSampleMonthlyStat.StartedUnits = txtStartedUnits.Text
			dbSampleMonthlyStat.SoldUnits = txtSoldUnits.Text
			dbSampleMonthlyStat.ClosingUnits = txtClosingUnits.Text
			dbSampleMonthlyStat.UnsoldUnits = txtUnsoldUnits.Text
			dbSampleMonthlyStat.TimePeriodDate = dtTimePeriodDate.Value
	
			If SampleMonthlyStatID <> 0 Then
				dbSampleMonthlyStat.Update()
			Else
				SampleMonthlyStatID = dbSampleMonthlyStat.Insert
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
		Response.Redirect("delete.aspx?SampleMonthlyStatID=" & SampleMonthlyStatID & "&" & GetPageParams(FilterFieldType.All))
	End Sub
End Class
