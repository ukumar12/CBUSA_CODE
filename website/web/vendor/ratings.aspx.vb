Imports Components
Imports DataLayer

Partial Class vendor_ratings
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Private dtRatings As DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureVendorAccess()

        ctlOverallRating.VendorId = Session("VendorId")

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then
         Core.DataLog("Comments and Ratings", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)
            BindData()
        End If

    End Sub

    Private Sub BindData()
        Dim sqlFields As String = "select Row_Number() over(order by c.Submitted desc) as RowNumber, c.*, b.CompanyName"
        Dim sql As String = " from VendorComment c inner join Builder b on c.BuilderID=b.BuilderID where c.VendorID=" & DB.Number(Session("VendorId"))

        Dim cnt As Integer = DB.ExecuteScalar("select count(*) " & sql)
        Dim dt As DataTable = DB.GetDataTable("select * from (" & sqlFields & sql & ") as temp where temp.RowNumber >=" & (ctlNavigator.PageNumber - 1) * ctlNavigator.MaxPerPage & " and temp.RowNumber <" & (ctlNavigator.PageNumber * ctlNavigator.MaxPerPage) & " order by temp.RowNumber")

        dtRatings = DB.GetDataTable("select * from VendorRatingCategoryRating r inner join RatingCategory c on r.RatingCategoryID=c.RatingCategoryID where r.VendorID=" & DB.Number(Session("VendorId")))

        If cnt = 0 Then
            ctlNavigator.Visible = False
        Else
            ctlNavigator.NofRecords = cnt
            ctlNavigator.DataBind()
        End If
        rptComments.DataSource = dt
        rptComments.DataBind()
    End Sub


    Protected Sub ctlNavigator_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigator.NavigatorEvent
        ctlNavigator.PageNumber = e.PageNumber
        BindData()
    End Sub

    Protected Sub rptComments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptComments.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Dim ltlRatings As Literal = e.Item.FindControl("ltlRatings")
        Dim aRatings As DataRow() = dtRatings.Select("BuilderID=" & e.Item.DataItem("BuilderId"), "RatingCategory")
        ltlRatings.Text = "<table cellpadding=""2"" cellspacing=""2"" border=""0"" style=""background-color:#fff;border:1px solid #000;"">"
        For Each row As DataRow In aRatings
            ltlRatings.Text &= "<tr><td class=""bold smaller"">" & row("RatingCategory") & "</td><td style=""white-space:nowrap;"">"
            Dim i As Integer
            If Core.GetInt(row("Rating")) = 0 Then
                ltlRatings.Text &= "<img src=""/images/rating/na-red.gif"" alt=""Not Rated"" />"
                ltlRatings.Text &= "</td><td style=""white-space:nowrap;"" class=""bold smaller"">N/A</td>"
            Else
                For i = 1 To Core.GetInt(row("Rating"))
                    ltlRatings.Text &= "<img src=""/images/rating/star-red-sm.gif"" alt=""" & Core.GetInt(row("Rating")) & """ />"
                Next
                For i = i To 10
                    ltlRatings.Text &= "<img src=""/images/rating/star-gr-sm.gif"" alt=""" & Core.GetInt(row("Rating")) & """ />"
                Next
                ltlRatings.Text &= "</td><td style=""white-space:nowrap;"" class=""bold smaller"">" & Core.GetInt(row("Rating")) & "</td>"
            End If
            ltlRatings.Text &= "</tr>"
        Next
        ltlRatings.Text &= "</table>"

    End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
       ' Response.Redirect("/vendor/default.aspx")
   ' End Sub
End Class
