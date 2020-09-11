
Imports Components
Partial Class homestarts_vendorsurvey
    Inherits BasePage

    Public Property C_Month As Integer
        Get
            Return If(ViewState("C_Month") IsNot Nothing, CInt(ViewState("C_Month")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("C_YC_Monthear") = value
        End Set
    End Property

    Public Property C_Year As Integer
        Get
            Return If(ViewState("C_Year") IsNot Nothing, CInt(ViewState("C_Year")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("C_Year") = value
        End Set
    End Property
    Public Property VendorID As Integer
        Get
            Return If(ViewState("VendorID") IsNot Nothing, CInt(ViewState("VendorID")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("VendorID") = value
        End Set
    End Property

    Public Property VendorAccountID As Integer
        Get
            Return If(ViewState("VendorAccountID") IsNot Nothing, CInt(ViewState("VendorAccountID")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("VendorAccountID") = value
        End Set
    End Property

    Public Property SurveyId As Integer
        Get
            Return If(ViewState("SurveyId") IsNot Nothing, CInt(ViewState("SurveyId")), 0)
        End Get
        Set(ByVal value As Integer)
            ViewState("SurveyId") = value
        End Set
    End Property

    Public Property sReportUrl As String
        Get
            Return If(ViewState("sReportUrl") IsNot Nothing, ViewState("sReportUrl"), "")
        End Get
        Set(ByVal value As String)
            ViewState("sReportUrl") = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'btnSubmitSurvey.Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(btnSubmitSurvey, Nothing) + ";")
            'btnUpdate.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(btnUpdate, Nothing) + ";")

            If Not IsPostBack Then
                Dim OrganisationID As Integer = Request.QueryString("oid")
                Dim ContactId As Integer = Request.QueryString("cid")
                ViewState("sReportUrl") = "oid=" + OrganisationID.ToString() + "&cid=" + ContactId.ToString()
                Dim CompanyName As String = String.Empty
                ddlBestPersons.Enabled = False

                If OrganisationID <= 0 Or ContactId <= 0 Then

                    ltrlException.Text = "An error occurred. Please try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
                    btnSubmitSurvey.Enabled = False
                    pnlException.Visible = True
                    pnlBestPersion.Visible = False
                    btnSubmitSurvey.Visible = False
                    Return
                End If

                ViewState("C_Month") = Val(DateTime.Now.ToString("MM"))
                ViewState("C_Year") = Val(DateTime.Now.ToString("yyyy"))
                Dim CompanyInfo As DataTable = DB.GetDataTable("select CompanyName, VendorID from  Vendor   where CRMID=" & OrganisationID & " and IsActive =1 ")
                If CompanyInfo.Rows.Count > 0 Then
                    CompanyName = CompanyInfo.Rows(0)("CompanyName")
                    ViewState("VendorID") = CompanyInfo.Rows(0)("VendorID")
                    lblCompanyName.Text = CompanyName
                Else
                    ltrlException.Text = "An error occurred. Please try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
                    btnSubmitSurvey.Enabled = False
                    pnlException.Visible = True
                    btnSubmitSurvey.Visible = False
                    pnlBestPersion.Visible = False
                End If
                Dim VendorAccount As DataTable = DB.GetDataTable("select FirstName+' '+ LastName as UserName,VendorAccountID from VendorAccount where CRMID=" & ContactId & " and VendorID=" & VendorID & "")

                If VendorAccount.Rows.Count > 0 Then
                    ViewState("VendorAccountID") = VendorAccount.Rows(0)("VendorAccountID")
                    lblUserName.Text = VendorAccount.Rows(0)("UserName")
                Else
                    ltrlException.Text = "An error occurred. Please try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
                    btnSubmitSurvey.Visible = False
                    btnSubmitSurvey.Enabled = False
                    pnlException.Visible = True
                    pnlBestPersion.Visible = False
                End If

                Dim SurveyData As DataTable = DB.GetDataTable("select top 2 isnull(SurveyMonth,0) As SurveyMonth,ISNULL(SurveyData,0) SurveyData from VendorNPS where VendorID=" & VendorID & "  AND SurveyYear=" & C_Year & " order by SurveyMonth desc")
                If SurveyData.Rows.Count > 0 Then

                    If Val(SurveyData.Rows(0)("SurveyMonth")) = C_Month Then
                        pnlAlreadySubmittedSurvayData.Visible = True
                        btnSubmitSurvey.Visible = False
                        Response.Redirect("vendor_survey_report.aspx?" & sReportUrl)
                    Else
                        pnlAlreadySubmittedSurvayData.Visible = False
                        If CInt(SurveyData.Rows(0)("SurveyData")) > 0 And CInt(SurveyData.Rows(0)("SurveyData")) > 0 Then
                            btnSubmitSurvey.Enabled = True
                        End If
                    End If

                End If

                '' Load Best Person Drop down list
                'ddlBestPersons.DataSource = DB.GetDataTable("SELECT FirstName+' '+ LastName as UserName,VendorAccountID FROM VendorAccount WHERE  VendorID=" & VendorID.ToString() & " AND VendorAccountID <> " + VendorAccountID.ToString() + "")
                'ddlBestPersons.DataValueField = "VendorAccountID"
                'ddlBestPersons.DataTextField = "UserName"
                'ddlBestPersons.DataBind()
                'ddlBestPersons.Items.Insert(0, New ListItem("-- select names --", "-1"))

            End If
        Catch ex As Exception
            ltrlException.Text = "An error occurred. Please try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
            pnlException.Visible = True
            btnSubmitSurvey.Visible = False
            pnlBestPersion.Visible = False
        End Try



    End Sub

    Protected Sub btnSubmitSurvey_Click(sender As Object, e As EventArgs)
        If Page.IsValid Then


            Try

                Dim SQL As String

                Dim NetPromoterScore As Integer = 0
                If rb_0.Checked Then
                    NetPromoterScore = 0
                ElseIf rb_1.Checked Then
                    NetPromoterScore = 1
                ElseIf rb_2.Checked Then
                    NetPromoterScore = 2
                ElseIf rb_3.Checked Then
                    NetPromoterScore = 3
                ElseIf rb_4.Checked Then
                    NetPromoterScore = 4
                ElseIf rb_5.Checked Then
                    NetPromoterScore = 5
                ElseIf rb_6.Checked Then
                    NetPromoterScore = 6
                ElseIf rb_7.Checked Then
                    NetPromoterScore = 7
                ElseIf rb_8.Checked Then
                    NetPromoterScore = 8
                ElseIf rb_9.Checked Then
                    NetPromoterScore = 9
                ElseIf rb_10.Checked Then
                    NetPromoterScore = 10
                End If

                rb_0.Enabled = False
                rb_1.Enabled = False
                rb_2.Enabled = False
                rb_3.Enabled = False
                rb_4.Enabled = False
                rb_5.Enabled = False
                rb_6.Enabled = False
                rb_7.Enabled = False
                rb_8.Enabled = False
                rb_9.Enabled = False
                rb_10.Enabled = False

                'check if survay already submitted by vendor
                Dim sqlQuery = "SELECT VendorID FROM VendorNPS WHERE VendorID=" & VendorID & " AND SurveyYear=" & C_Year & " AND SurveyMonth=" & C_Month & ""

                Dim dtdata As DataTable = DB.GetDataTable(sqlQuery)
                If dtdata.Rows.Count <= 0 Then
                    SQL = " INSERT INTO [dbo].[VendorNPS] (" _
                         & " [VendorID]" _
                         & ",[VendorAccountId]" _
                         & ",[SurveyMonth]" _
                         & ",[SurveyYear]" _
                         & ",[NetPromoterScore]" _
                         & ",[DateCreated]" _
                         & ") VALUES (" _
                         & VendorID _
                         & "," & VendorAccountID _
                         & "," & C_Month _
                         & "," & C_Year _
                         & "," & NetPromoterScore _
                         & ",GetDate() " _
                         & ")"
                    ViewState("SurveyId") = DB.InsertSQL(SQL)
                    If SurveyId > 0 Then
                        btnSubmitSurvey.Enabled = False
                        pnlSuccess.Visible = True
                        AlertMessage("Thanks for taking the survey!")
                    Else
                        pnlException.Visible = True
                    End If

                    pnlBestPersion.Visible = False
                Else
                    Response.Redirect("vendor_survey_report.aspx?" & sReportUrl)
                End If
            Catch ex As Exception
                ltrlException.Text = "An error occurred, and your data couldn't be saved. Please try again. If the problem persists, please contact <a href='mailto:customerservice@cbusa.us'>customerservice@cbusa.us</a>."
                pnlException.Visible = True
            End Try
        End If



    End Sub
    Protected Sub rbBestPersion_CheckedChanged(sender As Object, e As EventArgs) Handles rbBestPersionY.CheckedChanged
        If rbBestPersionY.Checked Then
            ddlBestPersons.Items.Clear()
            ddlBestPersons.Enabled = False
        Else
            ddlBestPersons.Enabled = True
            ddlBestPersons.DataSource = DB.GetDataTable("SELECT FirstName+' '+ LastName as UserName,VendorAccountID FROM VendorAccount WHERE  VendorID=" & VendorID.ToString() & " AND VendorAccountID <> " + VendorAccountID.ToString() + "")
            ddlBestPersons.DataValueField = "VendorAccountID"
            ddlBestPersons.DataTextField = "UserName"
            ddlBestPersons.DataBind()
            ddlBestPersons.Items.Insert(0, New ListItem("-- select names --", ""))
            rb_0.Checked = False
            rb_1.Checked = False
            rb_2.Checked = False
            rb_3.Checked = False
            rb_4.Checked = False
            rb_5.Checked = False
            rb_6.Checked = False
            rb_7.Checked = False
            rb_8.Checked = False
            rb_9.Checked = False
            rb_10.Checked = False


            ' pnlBestPersion.Visible = True
        End If

        rb_0.Enabled = rbBestPersionY.Checked
        rb_1.Enabled = rbBestPersionY.Checked
        rb_2.Enabled = rbBestPersionY.Checked
        rb_3.Enabled = rbBestPersionY.Checked
        rb_4.Enabled = rbBestPersionY.Checked
        rb_5.Enabled = rbBestPersionY.Checked
        rb_6.Enabled = rbBestPersionY.Checked
        rb_7.Enabled = rbBestPersionY.Checked
        rb_8.Enabled = rbBestPersionY.Checked
        rb_9.Enabled = rbBestPersionY.Checked
        rb_10.Enabled = rbBestPersionY.Checked

        ClientScript.RegisterClientScriptBlock(Me.GetType(), "change", "<script type = 'text/javascript'> NetPromoterChange(); </script>")
        ddlBestPersons.Focus()

    End Sub
    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        '  If SurveyId > 0 Then

        'Dim SQL As String
        Dim sBestPersonIds As String = String.Empty
        sBestPersonIds = If(rbBestPersionY.Checked, "Y", ddlBestPersons.SelectedValue.ToString())

        '        SQL = "UPDATE [dbo].[VendorNPS] " _
        '         & "        SET   [IsBestPersonIds]   = '" & sBestPersonIds & "' " _
        '         & "            , [DateUpdated]       = GETDATE() " _
        '         & " WHERE ID = " & SurveyId & " "

        '        If DB.ExecuteSQL(SQL) > 0 Then
        If rbBestPersionY.Checked Then
            AlertMessage("Please fill survey stats..!")
            rb_0.Focus()
            Return
        Else
            ddlBestPersons.Enabled = True
            btnSubmitSurvey.Enabled = False
            AlertMessage("Thanks for the feedback! We will remove you from the distribution list in the future.")
        End If

        Try

            Dim sMailBody As String = "Dear Team, </br></br><B>" + lblUserName.Text + "</B> from <b>" + lblCompanyName.Text + "</b> would like to be removed from the vendor NPS survey distribution list.</br></br>"
            If Val(ddlBestPersons.SelectedValue) > 0 Then
                sMailBody += " <b>" + ddlBestPersons.SelectedItem.Text + "</b> was identified as the person best suited to provide that data.</br></br>"
            End If
            sMailBody += "Sincerely,<br><br>"
            Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "arrowdev@medullus.com", lblUserName.Text, "Vendor NPS Survey", sMailBody)

            '' Need CC Code
            Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "cwagner@medullus.com", lblUserName.Text, "Vendor NPS Survey", sMailBody)

            'cc mail to cwagner@medullus.com
            rbBestPersionN.Enabled = False
            rbBestPersionY.Enabled = False
            btnUpdate.Enabled = False
            ddlBestPersons.Enabled = False
        Catch ex As Exception

        End Try
        '    End If
        'End If

    End Sub

    'Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
    '    If ddlBestPersons.SelectedValue > 0 Then

    '        AlertMessage("Thanks for the update! We will remove you from the distribution list.")
    '        Dim sMailBody As String = "Dear Team, </br></br><B>" + lblUserName.Text + "</B> from <b>" + lblCompanyName.Text + "</b> would like to be removed from the monthly home starts survey distribution list.</br></br>"
    '        If Val(ddlBestPersons.SelectedValue) > 0 Then
    '            sMailBody += " <b>" + ddlBestPersons.SelectedItem.Text + "</b> was identified as the person best suited to provide that data.</br></br>"
    '        End If
    '        sMailBody += "Sincerely,<br><br>"
    '        Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "arrowdev@medullus.com", lblUserName.Text, "Home Starts Survey", sMailBody)

    '        'rbBestPersionN.Enabled = False
    '        'rbBestPersionY.Enabled = False
    '        ddlBestPersons.Enabled = False
    '        txtActualStart.Enabled = False
    '        txtProjectedStart.Enabled = False
    '        btnSubmitSurvey.Enabled = False
    '        btnUpdate.Enabled = False
    '    End If
    'End Sub


    Protected Sub NetPromoter_ServerValidate(ByVal source As Object, ByVal args As ServerValidateEventArgs)
        args.IsValid = rb_0.Checked OrElse rb_1.Checked OrElse rb_2.Checked OrElse rb_3.Checked OrElse rb_4.Checked OrElse rb_5.Checked OrElse rb_6.Checked OrElse rb_7.Checked OrElse rb_8.Checked OrElse rb_9.Checked OrElse rb_10.Checked
    End Sub
    Public Sub AlertMessage(ByVal Message As String)
        Dim Msg As String = Message
        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type = 'text/javascript'>")
        'sb.Append("window.onload=function(){")
        'sb.Append("alert('")
        'sb.Append(Msg)
        'sb.Append("')};")
        sb.Append("alert('" + Msg + "');")
        sb.Append("</script>")
        ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
    End Sub

End Class
