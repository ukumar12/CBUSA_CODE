Imports Components
Imports DataLayer
Imports System.Net.Mail
Imports System.Linq
Imports System.Data
Imports Controls

Partial Class vendor
    Inherits SitePage

    Protected VendorId As Integer = 0
    Protected BuilderId As Integer = 1
    Protected Vendor As DataLayer.VendorRow
    Protected Registration As VendorRegistrationRow
    Protected countAdminEmails As Integer = 0
    Private m_dvRoles As DataView
    Public ReadOnly Property dvRoles() As DataView
        Get
            If m_dvRoles Is Nothing Then
                Dim dt As DataTable = VendorRoleRow.GetVendorRoles(DB, VendorId)
                m_dvRoles = dt.DefaultView
                m_dvRoles.Sort = "VendorAccountID"
            End If
            Return m_dvRoles
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("BuilderId") Is Nothing And Session("PIQId") Is Nothing And Session("AdminID") Is Nothing Then
            Response.Redirect("/default.aspx")
        End If

        Dim SQL As String = String.Empty

        VendorId = Convert.ToInt32(Request("VendorId"))
        If VendorId = 0 Then Response.Redirect("/directory/default.aspx")

        Me.RatingDisplay.VendorId = VendorId
        Vendor = DataLayer.VendorRow.GetRow(DB, VendorId)
        Registration = VendorRegistrationRow.GetRowByVendor(DB, VendorId)

        Me.PageTitle = Vendor.CompanyName
        Me.MetaKeywords = Vendor.CompanyName
        Me.MetaDescription = Vendor.CompanyName

        If Not IsPostBack Then
            LoadVendor()

            'BindData()

            Dim dbComment As VendorCommentRow = VendorCommentRow.GetRow(DB, VendorId, Session("BuilderId"))
            'txtComment.Text = dbComment.Comment

        End If
        ltlVendorRating.Text = "<a class='VendorButton' href='../builder/VendorRating.aspx?VendorId=" & Request("VendorId") & "'>Rate this vendor</a>"
        'Me.hdnbuilderid.Value = BuilderId
        'Me.hdnvendorid.Value = VendorId

    End Sub

    'Private Sub BindData()
    '    rptRatings.DataSource = VendorRatingCategoryRatingRow.GetVendorRatings(DB, VendorId, Session("BuilderId"))
    '    rptRatings.DataBind()

    '    rptComments.DataSource = VendorCommentRow.GetVendorComments(DB, VendorId)
    '    rptComments.DataBind()

    '    If rptComments.DataSource.Rows.Count > 0 Then
    '        Me.divCommentNone.Visible = False
    '    Else
    '        Me.divCommentNone.Visible = True
    '    End If
    'End Sub

    Private Sub LoadVendor()

        Me.ltrCompanyName.Text = Vendor.CompanyName

        If Vendor.WebsiteURL = String.Empty OrElse Vendor.WebsiteURL.ToLower = "http://" OrElse Vendor.WebsiteURL.ToLower = "https://" Then
            Me.trWebsiteURL.Visible = False
        Else
            Me.lnkWebsiteURL.NavigateUrl = Vendor.WebsiteURL
            Me.lnkWebsiteURL.Text = Vendor.WebsiteURL
            Me.trWebsiteURL.Visible = True
        End If

        If Vendor.Address <> String.Empty Then
            Me.divAddressInfo.InnerHtml = Vendor.Address
        End If

        If Vendor.Address2 <> String.Empty Then
            If Me.divAddressInfo.InnerHtml <> String.Empty Then Me.divAddressInfo.InnerHtml &= "<br/>"
            Me.divAddressInfo.InnerHtml &= Vendor.Address2
        End If

        If Me.divAddressInfo.InnerHtml <> String.Empty Then Me.divAddressInfo.InnerHtml &= "<br/>"
        Me.divAddressInfo.InnerHtml &= Vendor.City & ", " & Vendor.State & " " & Vendor.Zip

        If Vendor.Phone <> String.Empty Then
            If Me.divContactInfo.InnerHtml <> String.Empty Then Me.divContactInfo.InnerHtml &= "<br/>"
            Me.divContactInfo.InnerHtml &= "p: " & Vendor.Phone
        End If

        If Vendor.Fax <> String.Empty Then
            If Me.divContactInfo.InnerHtml <> String.Empty Then Me.divContactInfo.InnerHtml &= "<br/>"
            Me.divContactInfo.InnerHtml &= "f: " & Vendor.Fax
        End If

        If Vendor.BillingAddress <> String.Empty Then
            divBillingInfo.InnerHtml = "<b>Billing:</b><br/>"
            divBillingInfo.InnerHtml &= Vendor.BillingAddress & "<br/>"
        End If

        If Vendor.BillingCity <> String.Empty Then
            divBillingInfo.InnerHtml &= Vendor.City & IIf(Vendor.State <> String.Empty, ", ", "") & Vendor.State & " " & Vendor.Zip
        End If

        Dim dbBusinessType As BusinessTypeRow = BusinessTypeRow.GetRow(DB, Registration.BusinessType)
        If dbBusinessType.BusinessTypeID <> Nothing Then
            ltlBusinessType.Text = dbBusinessType.BusinessType
        End If

        ltlNumEmployees.Text = Registration.Employees
        ltlYearStarted.Text = Registration.YearStarted
        ltlAreasSupplied.Text = Registration.SupplyArea

        If Registration.SubsidiaryExplanation <> String.Empty Then
            ltlSubsidiary.Text = Registration.SubsidiaryExplanation
        Else
            trSubsidiary.Visible = False
        End If

        Dim sSupplyPhases As String = VendorCategoryRow.GetNames(DB, Vendor.GetSelectedVendorCategories, ",")

        'If sSupplyPhases <> String.Empty Then
        '    tdSupplyPhase.InnerHtml = Replace(sSupplyPhases, ",", ", ")
        'End If

        'If tdSupplyPhase.InnerHtml = String.Empty Then
        '    trSupplyPhase.Visible = False
        'End If

        ctlTabs.Data.Add("Supply Phases", Replace(sSupplyPhases, ",", vbCrLf))
        ctlTabs.Data.Add("Services Offered", Vendor.ServicesOffered)
        ctlTabs.Data.Add("Discounts", Vendor.Discounts)
        ctlTabs.Data.Add("Additional Rebate Program", Vendor.RebateProgram)
        ctlTabs.Data.Add("Payment Terms", Vendor.PaymentTerms)
        ctlTabs.Data.Add("Specialty Services", Vendor.SpecialtyServices)
        ctlTabs.Data.Add("Manufacturers / Brands", Vendor.BrandsSupplied)
        If Vendor.AcceptedCards <> String.Empty Then
            ctlTabs.Data.Add("Accepted Credit Cards", Vendor.AcceptedCards)
        Else
            ctlTabs.Data.Add("Accepted Credit Cards", "None")
        End If

        Dim dtRebateTerms As DataTable
        dtRebateTerms = RebateTermRow.GetCurrentTerms(DB, Vendor.VendorID)
        Dim sRebates As String = String.Empty
        Dim Conn As String = String.Empty
        If Not dtRebateTerms.Rows.Count = 0 Then
            For Each row As DataRow In dtRebateTerms.Rows
                sRebates &= Conn & "Starting Period: Quarter " & row("StartQuarter") & ", " & row("StartYear") & vbCrLf & vbTab & " - Rebate Percentage: " & row("RebatePercentage") & "%"
                Conn = vbCrLf
            Next
        End If

        ctlTabs.Data.Add("Rebate Terms", sRebates)

        Me.rptContacts.DataSource = DB.GetDataTable("SELECT va.VendorAccountID, va.FirstName, va.LastName, va.Title, va.Email, va.Phone, va.Mobile, va.Fax FROM VendorAccount AS va INNER JOIN Vendor AS vndr ON va.VendorID = vndr.VendorID WHERE va.IsActive = 1 AND va.VendorID=" & DB.Number(VendorId) & " ORDER BY FirstName ASC, LastName ASC")
        Me.rptContacts.DataBind()

    End Sub

    Protected Sub rptContacts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptContacts.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If
        Dim ltlPrimary As Literal = CType(e.Item.FindControl("ltlPrimary"), Literal)
        Dim ltlContactFullName As Literal = CType(e.Item.FindControl("ltlContactFullName"), Literal)
        Dim trTitle As HtmlTableRow = CType(e.Item.FindControl("trTitle"), HtmlTableRow)
        Dim ltlTitle As Literal = CType(e.Item.FindControl("ltlTitle"), Literal)
        Dim trPhone As HtmlTableRow = CType(e.Item.FindControl("trPhone"), HtmlTableRow)
        Dim ltlPhone As Literal = CType(e.Item.FindControl("ltlPhone"), Literal)
        Dim trMobile As HtmlTableRow = CType(e.Item.FindControl("trMobile"), HtmlTableRow)
        Dim ltlMobile As Literal = CType(e.Item.FindControl("ltlMobile"), Literal)
        Dim trFax As HtmlTableRow = CType(e.Item.FindControl("trFax"), HtmlTableRow)
        Dim ltlFax As Literal = CType(e.Item.FindControl("ltlFax"), Literal)
        Dim trEmail As HtmlTableRow = CType(e.Item.FindControl("trEmail"), HtmlTableRow)
        Dim ltlEmail As Literal = CType(e.Item.FindControl("ltlEmail"), Literal)


        Dim sSQL As String = "SELECT vavr.VendorAccountID FROM VendorAccountVendorRole AS vavr INNER JOIN VendorRole AS vr ON vr.VendorRoleID = vavr.VendorRoleID WHERE vr.VendorRole = 'Primary Contact' AND vavr.VendorAccountID = " & e.Item.DataItem("VendorAccountID")
        Dim IsPrimary As Boolean = Not (DB.ExecuteScalar(sSQL) = Nothing)

        If IsPrimary Then
            ltlPrimary.Text = "Primary "
        End If

        ltlContactFullName.Text = Core.BuildFullName(e.Item.DataItem("FirstName"), " ", e.Item.DataItem("LastName"))

        If Not IsDBNull(e.Item.DataItem("Title")) AndAlso Not e.Item.DataItem("Title").ToString = String.Empty Then
            ltlTitle.Text = e.Item.DataItem("Title")
        Else
            trTitle.Visible = False
        End If

        If Not IsDBNull(e.Item.DataItem("Phone")) AndAlso Not e.Item.DataItem("Phone").ToString = String.Empty Then
            ltlPhone.Text = e.Item.DataItem("Phone")
        Else
            trPhone.Visible = False
        End If

        If Not IsDBNull(e.Item.DataItem("Mobile")) AndAlso Not e.Item.DataItem("Mobile").ToString = String.Empty Then
            ltlMobile.Text = e.Item.DataItem("Mobile")
        Else
            trMobile.Visible = False
        End If

        If Not IsDBNull(e.Item.DataItem("Fax")) AndAlso Not e.Item.DataItem("Fax").ToString = String.Empty Then
            ltlFax.Text = e.Item.DataItem("Fax")
        Else
            trFax.Visible = False
        End If

        If Not IsDBNull(e.Item.DataItem("Email")) AndAlso Not e.Item.DataItem("Email").ToString = String.Empty Then
            ltlEmail.Text = "<a href=""mailto:" & e.Item.DataItem("Email") & """>" & e.Item.DataItem("Email") & "</a>"
        Else
            trEmail.Visible = False
        End If

        'Dim roles As String = (From r In dvRoles Where Not IsDBNull(r("VendorAccountID")) AndAlso r("VendorAccountID") = e.Item.DataItem("VendorAccountID") Select r("VendorRole")).DefaultIfEmpty.Aggregate(Function(sum, app) IIf(sum = String.Empty, app, sum & "<br/>" & app))
        Dim roles As New StringBuilder
        For Each r As DataRowView In dvRoles
            If Not IsDBNull(r("VendorAccountID")) AndAlso r("VendorAccountID") = e.Item.DataItem("VendorAccountID") Then
                roles.Append(r("VendorRole") & "<br/>")
            End If
        Next
        If roles.Length = 0 Then
            Dim trRoles As HtmlTableRow = e.Item.FindControl("trRoles")
            trRoles.Visible = False
        Else
            Dim ltlRoles As Literal = e.Item.FindControl("ltlRoles")
            ltlRoles.Text = roles.ToString
        End If
    End Sub

    'Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
    '    If Not IsValid Then
    '        Exit Sub
    '    End If

    '    Dim dbComment As VendorCommentRow = VendorCommentRow.GetRow(DB, VendorId, Session("BuilderId"))
    '    dbComment.BuilderID = Session("BuilderId")
    '    dbComment.Comment = txtComment.Text
    '    dbComment.VendorID = VendorId
    '    If dbComment.Submitted = Nothing Then
    '        dbComment.Insert()
    '    Else
    '        dbComment.Update()
    '    End If

    '    Dim sRatings As New StringBuilder()

    '    For Each item As RepeaterItem In rptRatings.Items
    '        Dim hdnRatingCategoryID As HiddenField = item.FindControl("hdnRatingCategoryID")
    '        Dim ctlRating As Controls.StarRating = item.FindControl("ctlRating")

    '        Dim CategoryID As Integer = hdnRatingCategoryID.Value
    '        Dim dbRow As VendorRatingCategoryRatingRow = VendorRatingCategoryRatingRow.GetRow(DB, VendorId, Session("BuilderId"), CategoryID)
    '        dbRow.Rating = ctlRating.Rating
    '        Try
    '            dbRow.Insert()
    '        Catch ex As SqlClient.SqlException
    '            dbRow.Update()
    '        End Try

    '        Dim dbRating As RatingCategoryRow = RatingCategoryRow.GetRow(DB, dbRow.RatingCategoryID)
    '        If dbRow.Rating = 0 Then
    '            sRatings.AppendLine(dbRating.RatingCategory & ":" & vbTab & vbTab & "Not Rated")
    '        Else
    '            sRatings.AppendLine(dbRating.RatingCategory & ":" & vbTab & vbTab & dbRow.Rating)
    '        End If

    '    Next

    '    Dim Builder As String = BuilderRow.GetRow(DB, Session("BuilderId")).CompanyName
    '    Dim sMsg As String = Builder & " has submitted the following vendor rating for your company:" & vbCrLf & vbCrLf & "Ratings:" & vbCrLf & vbCrLf & sRatings.ToString & vbCrLf & vbCrLf & "Comments:" & dbComment.Comment & vbCrLf & vbCrLf & "In order to review all ratings that have been submitted for your company, please login to the CBUSA software and select the ‘Ratings and Comments’ link on your vendor dashboard." & vbCrLf & vbCrLf & "Thank you." & vbCrLf & vbCrLf & "Brian Pavlick" & vbCrLf & "CBUSA" & vbCrLf & "brian@cbusa.us"

    '    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
    '    Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewRating")

    '    dbMsg.Send(dbVendor, sMsg)

    '    'Send email to all builders in same llc.

    '    dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NewRatingToBuilders")
    '    sMsg = Builder & " has completed the following vendor rating for " & Vendor.CompanyName & ":" & vbCrLf & vbCrLf & "Ratings:" & vbCrLf & vbCrLf & sRatings.ToString & vbCrLf & vbCrLf & "Comments:" & dbComment.Comment & vbCrLf & vbCrLf & "In order to submit a rating for any preferred vendor or to review other ratings that have been submitted, please login to the CBUSA software and refer to the Member Directory." & vbCrLf & vbCrLf & "Thank you." & vbCrLf & vbCrLf & "Brian Pavlick" & vbCrLf & "CBUSA" & vbCrLf & "brian@cbusa.us"

    '    Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, dbVendor.GetSelectedLLCs.Split(",")(0))



    '    For Each row As DataRow In dtBuilders.Rows
    '        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))

    '        dbMsg.Send(dbBuilder, sMsg, CCLLCNotification:=False)
    '        If SysParam.GetValue(DB, "TestMode") Then Exit For
    '    Next

    '    Try
    '        dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB, dbVendor.GetSelectedLLCs.Split(",")(0)), sMsg)
    '    Catch ex As Exception

    '    End Try


    '    ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenSaved", "InitSaved();", True)
    '    BindData()
    'End Sub
End Class
