Imports Components
Imports DataLayer
Imports TwoPrice.DataLayer

Partial Class takeoffs_vendor
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Private m_dtComparisons As DataTable
    Private ReadOnly Property dtComparisons() As DataTable
        Get
            If m_dtComparisons Is Nothing Then
                m_dtComparisons = PriceComparisonRow.GetSavedComparisons(DB, Session("BuilderId"))
            End If
            Return m_dtComparisons
        End Get
    End Property

    Private Function BidSubmitted(ByVal TwoPriceCampaignId As Integer) As Boolean
        Dim returnval As Boolean = DB.ExecuteScalar("SELECT COUNT(*) FROM TwoPriceVendorProductPrice WHERE VendorId= " & Session("VendorId") & " AND TwoPriceCampaignId = " & TwoPriceCampaignId & " AND Submitted = 1") > 0
        Return returnval
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (IsLoggedInBuilder() Or IsLoggedInVendor() Or Session("AdminId") IsNot Nothing) Then
            Response.Redirect("/default.aspx")
        End If

        gvList.BindList = AddressOf BindData

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")
        If Not IsPostBack Then
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "Saved"
                gvList.SortOrder = "Desc"
            End If

            BindData()

         Core.DataLog("Committed Purchase", PageURL, CurrentUserId, "Vendor Top Menu Click", "", "", "", "", UserName)

        End If
    End Sub

    Private Sub BindData()
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        Dim res As DataTable = TwoPriceCampaignRow.GetTwoPriceTakeOffList(DB, VendorId:=Session("VendorId"), RestrictByDate:=True, GetActiveOnly:=True)
        'res.DefaultView.RowFilter = "AwardedVendorID IS NULL OR   AwardedVendorID = " & Session("VendorId")

        gvList.Pager.NofRecords = res.Rows.Count
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName = "ChangeTakeoffPrices" Then
            Response.Redirect("sku-price.aspx?TwoPriceTakeOffId=" & e.CommandArgument)
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub

        Dim ltlStatus As Literal = e.Row.FindControl("ltlStatus")
        Dim lnkCommitted As LinkButton = e.Row.FindControl("lnkCommitted")
        Dim Status As String = ""

        Status = e.Row.DataItem("Status")
        lnkCommitted.Text = e.Row.DataItem("Name")
        lnkCommitted.CommandArgument = e.Row.DataItem("TwoPriceTakeoffID")

        'If BidSubmitted(e.Row.DataItem("TwoPriceCampaignId")) Then
        '    Status = "Bid Submitted"
        'End If


        If Status = "Awarded" And Not IsDBNull(e.Row.DataItem("AwardedVendorId")) Then
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, e.Row.DataItem("AwardedVendorId"))
            Status &= " to " & dbVendor.CompanyName
        ElseIf BidSubmitted(e.Row.DataItem("TwoPriceCampaignId")) Then
            Status = "Bid Submitted"
        ElseIf e.Row.DataItem("HasDeclinedToBid") Then
            Status = "Declined To Bid"
        End If

        ltlStatus.Text = Status

    End Sub

   ' Protected Sub btnDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnDashBoard.Click
        'Response.Redirect("/builder/default.aspx")
   ' End Sub

End Class
