Imports Components
Imports DataLayer


Partial Class controls_VendorRegistrationSteps
    Inherits BaseControl

    Public Property RegistrationStep() As Integer
        Get
            If ViewState("RegistrationStep") Is Nothing Then
                ViewState("RegistrationStep") = 1
            End If
            Return ViewState("RegistrationStep")
        End Get
        Set(ByVal value As Integer)
            ViewState("RegistrationStep") = value
        End Set
    End Property

    Protected ReadOnly Property qstring() As String
        Get
            Return IIf(Request("id") Is Nothing, String.Empty, "?id=" & Request("id"))
        End Get
    End Property

    Protected Sub Page_PreRender1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        tdAccount.InnerHtml = "<span class=""larger"">1</span> - Account Information"
        tdCompany.InnerHtml = "<span class=""larger"">2</span> - Company Profile"
        tdDirectory.InnerHtml = "<span class=""larger"">3</span> - Directory Information"
        tdUsers.InnerHtml = "<span class=""larger"">4</span> - User Accounts & Roles"
        tdTerms.InnerHtml = "<span class=""larger"">5</span> - Rebate Terms"
        tdPhases.InnerHtml = "<span class=""larger"">6</span> - Supply Phases"

        Dim isComplete As Boolean = False

        If RegistrationStep > 1 And Not isComplete Then
            tdAccount.InnerHtml = "<a href=""/forms/vendor-registration/default.aspx" & qstring & IIf(qstring = Nothing, "?", "&") & "redir=false"">" & tdAccount.InnerHtml & "</a>"
        ElseIf RegistrationStep = 1 Then
            tdAccount.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdAccount.Attributes("class") = tdAccount.Attributes("class") & " bgltblue larger"
        Else
            tdAccount.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If
        If RegistrationStep > 2 And Not isComplete Then
            tdCompany.InnerHtml = "<a href=""/forms/vendor-registration/companyprofile.aspx" & qstring & """>" & tdCompany.InnerHtml & "</a>"
        ElseIf RegistrationStep = 2 Then
            tdCompany.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdCompany.Attributes("class") = tdCompany.Attributes("class") & " bgltblue larger"
        Else
            tdCompany.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If

        If RegistrationStep > 3 And Not isComplete Then
            tdDirectory.InnerHtml = "<a href=""/forms/vendor-registration/register.aspx" & qstring & """>" & tdDirectory.InnerHtml & "</a>"
        ElseIf RegistrationStep = 3 Then
            tdDirectory.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdDirectory.Attributes("class") = tdCompany.Attributes("class") & " bgltblue larger"
        Else
            tdDirectory.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If


        If RegistrationStep > 4 And Not isComplete Then
            tdUsers.InnerHtml = "<a href=""/forms/vendor-registration/users.aspx" & qstring & """>" & tdUsers.InnerHtml & "</a>"
        ElseIf RegistrationStep = 4 Then
            tdUsers.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdUsers.Attributes("class") = tdAccount.Attributes("class") & " bgltblue larger"
        Else
            tdUsers.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If

        If RegistrationStep > 5 And Not isComplete Then
            tdTerms.InnerHtml = "<a href=""/rebates/terms.aspx" & qstring & """>" & tdTerms.InnerHtml & "</a>"
        ElseIf RegistrationStep = 5 Then
            tdTerms.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdTerms.Attributes("class") = tdAccount.Attributes("class") & " bgltblue larger"
        Else
            tdTerms.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If

        If RegistrationStep > 6 And Not isComplete Then
            tdPhases.InnerHtml = "<a href=""/forms/vendor-registration/supplyphase.aspx" & qstring & """>" & tdPhases.InnerHtml & "</a>"
        ElseIf RegistrationStep = 6 Then
            tdPhases.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdPhases.Attributes("class") = tdAccount.Attributes("class") & " bgltblue larger"
        Else
            tdPhases.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If

    End Sub
End Class
