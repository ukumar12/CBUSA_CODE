Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Partial Class forgot
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsLoggedIn() Then
            Response.Redirect("/members/")
		End If
		lnk.InnerText = SysParam.GetValue(DB, "ContactUsEmail")
		lnk.HRef = "mailto:" & lnk.InnerText
	End Sub

    Protected Sub btnRetrieve_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRetrieve.Click
        If Not Page.IsValid Then Exit Sub
        Try
            Dim dbMember As MemberRow = MemberRow.GetRowByUsername(DB, txtUsername.Text)
            Dim dbBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, dbMember.MemberId)

            Dim sMsg As String

            sMsg = "Dear " & Core.BuildFullName(dbBilling.FirstName, String.Empty, dbBilling.LastName) & vbCrLf & vbCrLf
            sMsg = sMsg & "Welcome to the " & AppSettings("GlobalWebsiteName") & " Member section, below you will find your password." & vbCrLf
            sMsg = sMsg & "Please keep this password in a safe place." & vbCrLf & vbCrLf
            sMsg = sMsg & "Password : " & dbMember.Password & vbCrLf & vbCrLf
            sMsg = sMsg & "Your password is case sensitive, be sure to enter exactly how you see it above." & vbCrLf
            sMsg = sMsg & "Additionally, you can change your User ID and password by clicking on the 'Update my membership information' link after logging in." & vbCrLf
            sMsg = sMsg & vbCrLf
            sMsg = sMsg & "Sincerely," & vbCrLf
            sMsg = sMsg & AppSettings("GlobalWebsiteName") & " Administrator" & vbCrLf

            Call Core.SendSimpleMail(SysParam.GetValue(DB, "ContactUsEmail"), SysParam.GetValue(DB, "ContactUsName"), dbBilling.Email, dbBilling.Email, AppSettings("GlobalWebsiteName") & " password", sMsg)

            Session("Confirmation") = "<p>Thank you for your request.<p>You will be receiving your password momentarily via e-mail to the e-mail address on your account."

            Response.Redirect("confirmation.aspx")

        Catch ex As Exception
            AddError("The username you entered could not be found in our system. Please try again, or you may also create a new account")
        End Try
    End Sub
End Class
