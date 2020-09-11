Imports Components
Imports DataLayer

Partial Class surveys_start
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ResponseId As Integer = 0
		Dim SurveyId As Integer = 0
		Dim OrderId As Integer = 0
		Dim dbOrder As StoreOrderRow
		Dim dbSurvey As SurveyRow
		Dim RedirectUrl As String = Nothing
        Try

            If Request("ResponseId") <> "" Then
                ResponseId = Convert.ToInt32(Utility.Crypt.DecryptTripleDES(Request.QueryString("ResponseId")))
            Else
                SurveyId = Convert.ToInt32(Request.QueryString("SurveyId"))
            End If


            If ResponseId > 0 Then
                SurveyId = DataLayer.SurveyResponseRow.GetSurveyId(DB, ResponseId)
                If Not SurveyRow.IsLive(DB, SurveyId) Then
                    'Survey is not live
					RedirectUrl = "/404.aspx"
					Exit Try
				Else
					dbSurvey = SurveyRow.GetRow(DB, SurveyId)
					Dim dbSurveyResponse As SurveyResponseRow = SurveyResponseRow.GetRow(DB, ResponseId)
					If dbSurveyResponse.Status = 0 Then
						'Survey already started 0 = not started, 1 = started, 2 = completed
						'mark survey as started and note date/time
						If dbSurveyResponse.OrderId = Nothing AndAlso dbSurvey.IsFollowUp Then Response.Redirect("/404.aspx")
						dbSurveyResponse.Status = 1
						dbSurveyResponse.StartDate = Now
						dbSurveyResponse.Update()
					End If
				End If
				If SurveyId = 0 Then
					RedirectUrl = "/404.aspx"
					Exit Try
				End If
			ElseIf SurveyId > 0 Then
				If Not SurveyRow.IsLive(DB, SurveyId) Then
					'Survey is not live
					RedirectUrl = "/404.aspx"
					Exit Try
				Else
					dbSurvey = SurveyRow.GetRow(DB, SurveyId)
					If dbSurvey.IsFollowUp AndAlso Not Request.QueryString("OrderId") = Nothing Then
						OrderId = Convert.ToInt32(Utility.Crypt.DecryptTripleDes(Request.QueryString("OrderId")))
						dbOrder = StoreOrderRow.GetRow(DB, OrderId)
						If dbOrder.ProcessDate = Nothing OrElse Not DB.ExecuteScalar("select top 1 orderid from surveyresponse where status = 2 and orderid = " & OrderId & " and surveyid = " & DB.Number(SurveyId)) = Nothing Then
							RedirectUrl = "/surveys/notfound.aspx"
							Exit Try
						End If
					End If

					Dim dbSurveyResponse As New SurveyResponseRow(DB)

					If OrderId = Nothing AndAlso dbSurvey.IsFollowUp Then
						RedirectUrl = "/404.aspx"
						Exit Try
					End If
					dbSurveyResponse.OrderId = OrderId
					dbSurveyResponse.Status = 1
					dbSurveyResponse.StartDate = Now
					dbSurveyResponse.RemoteIP = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR").ToString
					dbSurveyResponse.SurveyId = SurveyId
					dbSurveyResponse.CompleteDate = Nothing

					ResponseId = dbSurveyResponse.Insert()
				End If
			Else
				RedirectUrl = "/404.aspx"
				Exit Try
            End If

            'Make sure user type has permission to access survey.
            If dbSurvey.IsBuilder Then
                EnsureBuilderAccess()
            ElseIf dbSurvey.IsVendor Then
                EnsureVendorAccess()
            ElseIf dbSurvey.IsPIQ Then
                EnsurePIQAccess()
            Else
                RedirectUrl = "/"
                Exit Try
            End If


        Catch ex As Exception
			RedirectUrl = "/404.aspx"
			Exit Try
		End Try
		If RedirectUrl <> Nothing Then Response.Redirect(RedirectUrl)
		Response.Redirect("/surveys/default.aspx?ResponseId=" & Server.UrlEncode(Utility.Crypt.EncryptTripleDes(ResponseId).ToString))
    End Sub
End Class
