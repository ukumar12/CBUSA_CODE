Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Collections.Generic
Imports System.Web.Services
Imports Utility
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Windows.Forms

Partial Class builder_VendorRating
    Inherits SitePage
    Protected VendorId As Integer = 0
    Protected BuilderId As Integer = 1
    Protected Vendor As DataLayer.VendorRow
    Protected Registration As VendorRegistrationRow
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("BuilderId") Is Nothing And Session("PIQId") Is Nothing And Session("AdminID") Is Nothing Then
            Response.Redirect("/default.aspx")
        End If

        Dim SQL As String = String.Empty


        'If VendorId = 0 Then Response.Redirect("/directory/default.aspx")

        'Me.RatingDisplay.VendorId = VendorId
        Vendor = DataLayer.VendorRow.GetRow(DB, VendorId)
        Registration = VendorRegistrationRow.GetRowByVendor(DB, VendorId)

        Me.PageTitle = "Rate A Vendor"
        Me.MetaKeywords = Vendor.CompanyName
        Me.MetaDescription = Vendor.CompanyName

        If Not IsPostBack Then
            'LoadVendor()
            If Session("BuilderId") IsNot Nothing Then
                'slPreferredVendor2.DataBind()
                'slPreferredVendor2.GetScript()
                slPreferredVendor2.WhereClause = "IsActive = 1 and VendorID in (select VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID and b.BuilderID=" & DB.Number(Session("BuilderID")) & ")" ' " IsActive = 1 and VendorID in (select VendorID from LLCVendor l inner join Builder b on l.LLCID=b.LLCID where exists (select * from VendorProductPrice where VendorId=l.VendorId) and b.BuilderID=" & DB.Number(Session("BuilderID")) & ")"
                pnlPreferredVendor.Visible = True
                AddHandler slPreferredVendor2.ValueChanged, AddressOf UpdateVendor
            End If
            Session("CurrentPreferredVendor") = Nothing

            If Request("VendorId") IsNot Nothing Then
                Session("CurrentPreferredVendor") = Convert.ToInt64(Request("VendorId"))
                VendorId = Convert.ToInt64(Request("VendorId"))
                Dim VendorName As String = DB.ExecuteScalar("select companyName from vendor where vendorId = " & Convert.ToInt64(Request("VendorId")) & "")
                slPreferredVendor2.Text = VendorName
                slPreferredVendor2.Value = Request("VendorId")
                'ScriptManager.RegisterClientScriptBlock(Me.Page, Me.GetType, "", "$('#liComment').trigger('click');", True)
            End If
            BindData()
            Dim dbComment As VendorCommentRow = VendorCommentRow.GetRow(DB, VendorId, Session("BuilderId"))
            txtComment.Text = dbComment.Comment

        End If
        If Session("CurrentPreferredVendor") IsNot Nothing Then
            VendorId = Session("CurrentPreferredVendor")
        End If

        'Me.hdnbuilderid.Value = BuilderId
        'Me.hdnvendorid.Value = VendorId

    End Sub
    Private Sub BindData()
        rptRatings.DataSource = VendorRatingCategoryRatingRow.GetVendorRatings(DB, VendorId, Session("BuilderId"))
        rptRatings.DataBind()

        rptComments.DataSource = VendorCommentRow.GetVendorComments(DB, VendorId)
        rptComments.DataBind()

        If rptComments.DataSource.Rows.Count > 0 Then
            Me.divCommentNone.Visible = False
        Else
            Me.divCommentNone.Visible = True
        End If
    End Sub
    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then
            Exit Sub
        End If
        If VendorId = 0 Or slPreferredVendor2.Text.Trim() = "" Or slPreferredVendor2.Text Is Nothing Then
            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.GetType, "alert", "alert('Please select any vendor');", True)
            'ScriptManager.RegisterStartupScript(Page, Me.GetType, "Info", "alert('Please select any vendor')", True)
            Exit Sub
        End If
        'VendorId = 36484
        Dim dbComment As VendorCommentRow = VendorCommentRow.GetRow(DB, VendorId, Session("BuilderId"))
        dbComment.BuilderID = Session("BuilderId")
        dbComment.Comment = txtComment.Text
        dbComment.VendorID = VendorId
        If dbComment.Submitted = Nothing Then
            dbComment.Insert()
        Else
            dbComment.Update()
        End If

        Dim sRatings As New StringBuilder()

        For Each item As RepeaterItem In rptRatings.Items
            Dim hdnRatingCategoryID As HiddenField = item.FindControl("hdnRatingCategoryID")
            Dim ctlRating As Controls.StarRating = item.FindControl("ctlRating")

            Dim CategoryID As Integer = hdnRatingCategoryID.Value
            Dim dbRow As VendorRatingCategoryRatingRow = VendorRatingCategoryRatingRow.GetRow(DB, VendorId, Session("BuilderId"), CategoryID)
            dbRow.Rating = ctlRating.Rating
            Try
                dbRow.Insert()
            Catch ex As SqlClient.SqlException
                dbRow.Update()
            End Try

            Dim dbRating As RatingCategoryRow = RatingCategoryRow.GetRow(DB, dbRow.RatingCategoryID)
            If dbRow.Rating = 0 Then
                sRatings.AppendLine(dbRating.RatingCategory & ":" & vbTab & vbTab & "Not Rated")
            Else
                sRatings.AppendLine(dbRating.RatingCategory & ":" & vbTab & vbTab & dbRow.Rating)
            End If

        Next

        Dim Builder As String = BuilderRow.GetRow(DB, Session("BuilderId")).CompanyName
        Dim sMsg As String = Builder & " has submitted the following vendor rating for your company:" & vbCrLf & vbCrLf & "Ratings:" & vbCrLf & vbCrLf & sRatings.ToString & vbCrLf & vbCrLf & "Comments:" & dbComment.Comment & vbCrLf & vbCrLf & "In order to review all ratings that have been submitted for your company, please login to the CBUSA software and select the ‘Ratings and Comments’ link on your vendor dashboard." & vbCrLf & vbCrLf & "Thank you." & vbCrLf & vbCrLf & "Brian Pavlick" & vbCrLf & "CBUSA" & vbCrLf & "brian@cbusa.us"

        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, VendorId)
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewRating")

        dbMsg.Send(dbVendor, sMsg)

        'Send email to all builders in same llc.

        dbMsg = AutomaticMessagesRow.GetRowByTitle(DB, "NewRatingToBuilders")
        sMsg = Builder & " has completed the following vendor rating for " & slPreferredVendor2.Text & ":" & vbCrLf & vbCrLf & "Ratings:" & vbCrLf & vbCrLf & sRatings.ToString & vbCrLf & vbCrLf & "Comments:" & dbComment.Comment & vbCrLf & vbCrLf & "In order to submit a rating for any preferred vendor or to review other ratings that have been submitted, please login to the CBUSA software and refer to the Member Directory." & vbCrLf & vbCrLf & "Thank you." & vbCrLf & vbCrLf & "Brian Pavlick" & vbCrLf & "CBUSA" & vbCrLf & "brian@cbusa.us"

        Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, dbVendor.GetSelectedLLCs.Split(",")(0))



        For Each row As DataRow In dtBuilders.Rows
            Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))

            dbMsg.Send(dbBuilder, sMsg, CCLLCNotification:=False)
            If SysParam.GetValue(DB, "TestMode") Then Exit For
        Next

        Try
            dbMsg.SendLLCNotificationListAReminder(LLCRow.GetLLCNotificationList(DB, dbVendor.GetSelectedLLCs.Split(",")(0)), sMsg)
        Catch ex As Exception

        End Try


        ScriptManager.RegisterClientScriptBlock(Page, Me.GetType, "OpenSaved", "InitSaved();", True)

        'BindData()
        Response.Redirect(HttpContext.Current.Request.Url.ToString(), True)

    End Sub
    Protected Sub slPreferredVendor2_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles slPreferredVendor2.ValueChanged
        If slPreferredVendor2.Value Is Nothing Or slPreferredVendor2.Value = "" Then
            Session("CurrentPreferredVendor") = Nothing
        Else
            Session("CurrentPreferredVendor") = slPreferredVendor2.Value
            'ctlSearch.CatalogType = controls_SearchSql.CATALOG_TYPE.CATALOG_TYPE_MARKET
        End If
        If slPreferredVendor2.Value = Nothing Or slPreferredVendor2.Value = "" Then
            VendorId = 0

        Else
            VendorId = slPreferredVendor2.Value
        End If
        BindData()
        Dim dbComment As VendorCommentRow = VendorCommentRow.GetRow(DB, VendorId, Session("BuilderId"))
        txtComment.Text = dbComment.Comment
        'RatingPanel.Attributes("class").Replace("PanelDisable", "")
    End Sub
    Protected Sub UpdateVendor(ByVal sender As Object, ByVal e As System.EventArgs)
        If VendorId = Nothing Then
            'ctlSearch.FilterListCallback = Nothing
            'ctlSearch.FilterCacheKey = Nothing
            'ctlSearch.FilterList = Nothing
        Else
            'ctlSearch.FilterListCallback = AddressOf FilterCallback
            'ctlSearch.FilterCacheKey = VendorId
            Dim list As New Generic.List(Of String)
            FilterCallback(list)
            'ctlSearch.FilterList = list
        End If
        ' ctlSearch.SearchProduct(0, True, True)
        'upResults.Update()
    End Sub
    Protected Sub FilterCallback(ByRef list As Generic.List(Of String))
        Dim dt As DataTable = VendorProductPriceRow.GetAllVendorPrices(DB, VendorId)
        list.AddRange(From dr As DataRow In dt.AsEnumerable Where Not IsDBNull(dr("VendorPrice")) And Not Core.GetBoolean(dr("IsSubstitution")) Select Convert.ToString(dr("ProductID")))
        list.AddRange(From dr As DataRow In dt.AsEnumerable Where Not IsDBNull(dr("SubstitutePrice")) And Core.GetBoolean(dr("IsSubstitution")) Select Convert.ToString(dr("ProductID")))
        'list.AddRange(From dr As DataRow In dt.AsEnumerable Select Convert.ToString(dr("ProductID")))
    End Sub
End Class
