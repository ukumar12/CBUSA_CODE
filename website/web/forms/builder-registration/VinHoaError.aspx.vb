
Imports Components
Imports DataLayer

Partial Class VinHoaError
    Inherits SitePage

    Private dbBuilder As BuilderRow
    Private initializedWebsession As Vindicia.WebSession


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Making Sure We get Session id parameter from vindicia
        If (Request("Session_Id") Is Nothing) Then Response.Redirect("default.aspx")

        'Load Builder
        dbBuilder = BuilderRow.GetRow(DB, Session("BuilderId"))

        'No builder found
        If dbBuilder Is Nothing OrElse dbBuilder.BuilderID = 0 Then
            Response.Redirect("default.aspx")
        End If

        'ONLY DO PROCESSING FOR GET REQUEST
        If Not IsPostBack Then
            LogWebSessionAndRedirect()
        End If



        'THIS WILL HAPPEN IF WE GET A POST, THIS SHOULD'NT HAPPEN
        Response.Redirect("default.aspx?vinHoaErr=ln33", True)

    End Sub

    Private Sub LogWebSessionAndRedirect()
        Dim sessionId As String = Request("Session_Id")
        Dim vinWebSessionReturn As Vindicia.Return
        Dim postedValues() As Vindicia.NameValuePair
        Dim requestRedirectFromUpdatePage As Boolean = False
        initializedWebsession = New Vindicia.WebSession
        Try
            vinWebSessionReturn = New Vindicia.WebSession().fetchByVid("", sessionId, initializedWebsession)

            requestRedirectFromUpdatePage = (From sPair In postedValues
                                             Where sPair.name = "ae_Requested_from_page"
                                             Select sPair.value).FirstOrDefault().ToLower = "true"

            Dim webSession As New VindiciaWebSession(DB, dbBuilder, VindiciaWebSessionMethods.Account_updatePaymentMethod, HttpContext.Current)
            Dim pageName As String = CType(IIf(requestRedirectFromUpdatePage, "updates", "payment"), String)
            webSession.LogReturn(initializedWebsession.apiReturn, "Error --> Account_updatePaymentMethod --> " & pageName)

        Catch ex As Exception
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try


        If requestRedirectFromUpdatePage Then
            Response.Redirect("/builder/update.aspx?vinHoaErr=ln64", True)
        Else
            Response.Redirect("/forms/builder-registration/payment.aspx?vinHoaErr=ln66", True)
        End If


    End Sub
End Class
