Imports Components

Partial Class controls_BuilderRegistrationSteps
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
        tdPayment.InnerHtml = "<span class=""larger"">2</span> - Payment Information"

        Dim isComplete As Boolean = (RegistrationStep = 5)

        If RegistrationStep > 1 And Not isComplete Then
            tdAccount.InnerHtml = "<a href=""/forms/builder-registration/default.aspx" & qstring & """>" & tdAccount.InnerHtml & "</a>"
        ElseIf RegistrationStep = 1 Then
            tdAccount.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdAccount.Attributes("class") = tdAccount.Attributes("class") & " bgltblue larger"
        Else
            tdAccount.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If
       

       
        If RegistrationStep > 4 And Not isComplete Then
            tdPayment.InnerHtml = "<a href=""/forms/builder-registration/payment.aspx" & qstring & """>" & tdPayment.InnerHtml & "</a>"
        ElseIf RegistrationStep = 4 Then
            tdPayment.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdPayment.Attributes("class") = tdAccount.Attributes("class") & " bgltblue larger"
        Else
            tdPayment.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If

        If RegistrationStep > 5 Then
            tdConfirm.InnerHtml = "<a href=""/forms/builder-registration/thankyou.aspx" & qstring & """>" & tdConfirm.InnerHtml & "</a>"
        ElseIf RegistrationStep = 5 Then
            tdConfirm.Style(HtmlTextWriterStyle.BackgroundColor) = ""
            tdConfirm.Attributes("class") = tdAccount.Attributes("class") & " bgltblue larger"
        Else
            tdConfirm.Style(HtmlTextWriterStyle.Color) = "#aaa"
        End If

    End Sub
End Class
