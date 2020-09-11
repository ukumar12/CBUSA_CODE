Imports Components
Imports Utility
Imports System.Configuration.ConfigurationManager

Partial Class StarRatingDisplay
    Inherits BaseControl

    Protected m_VendorId As Integer = 0

    Public Property VendorId() As Integer
        Get
            Return m_VendorId
        End Get
        Set(ByVal value As Integer)
            m_VendorId = value
        End Set
    End Property

    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '    BindData()
    'End Sub

    Protected Sub Page_PreRender1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        BindData()
    End Sub

    Private Sub BindData()

        Dim OverallAvg As Double = Core.GetDouble(DB.ExecuteScalar("select Avg(cast(coalesce(Rating,0) as float)) from VendorRatingCategoryRating where VendorID=" & DB.Number(m_VendorId) & " and Coalesce(Rating,0) > 0"))
        ltlOverallAverage.Text = Math.Round(OverallAvg, 1)
        Dim i As Integer
        For i = 1 To Math.Ceiling(OverallAvg)
            ltlOverallStars.Text &= "<img alt=""" & OverallAvg & """ src=""/images/rating/star-red-sm.gif"" />"
        Next

        For i = i To 10
            ltlOverallStars.Text &= "<img alt=""" & OverallAvg & """ src=""/images/rating/star-gr-sm.gif"" />"
        Next


        Dim SQL As String = ""
        Dim Ratings As Integer = 0
        Dim Comments As Integer = 0
        Dim AverageStars As Integer = 0

        Dim dt As DataTable

        SQL = "SELECT" & vbCrLf
        SQL &= "  COUNT(*) Ratings" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  (" & vbCrLf
        SQL &= "    SELECT" & vbCrLf
        SQL &= "      VendorId, BuilderID" & vbCrLf
        SQL &= "    FROM" & vbCrLf
        SQL &= "      VendorRatingCategoryRating vrcr" & vbCrLf
        SQL &= "    WHERE" & vbCrLf
        SQL &= "      VendorId = " & VendorId & vbCrLf
        SQL &= "    GROUP BY" & vbCrLf
        SQL &= "      VendorId, BuilderID" & vbCrLf
        SQL &= ") r " & vbCrLf

        Ratings = CType(DB.ExecuteScalar(SQL), Integer)

        SQL = "SELECT" & vbCrLf
        SQL &= "  COUNT(*) Comments" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  VendorComment" & vbCrLf
        SQL &= "WHERE" & vbCrLf
        SQL &= "  VendorId = " & VendorId & vbCrLf

        Comments = CType(DB.ExecuteScalar(SQL), Integer)

        SQL = "SELECT" & vbCrLf
        SQL &= "  rc.RatingCategoryID," & vbCrLf
        SQL &= "  rc.RatingCategory," & vbCrLf
        SQL &= "  AVG(CAST(ISNULL(r.Rating, 0) AS FLOAT)) Rating" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  RatingCategory rc " & vbCrLf
        SQL &= "  LEFT JOIN" & vbCrLf
        SQL &= "  (" & vbCrLf
        SQL &= "    SELECT" & vbCrLf
        SQL &= "	  VendorID," & vbCrLf
        SQL &= "	  RatingCategoryID," & vbCrLf
        SQL &= "	  ROUND(Rating, 0, 1) Rating" & vbCrLf
        SQL &= "    FROM" & vbCrLf
        SQL &= "      VendorRatingCategoryRating vrcr" & vbCrLf
        SQL &= "    WHERE" & vbCrLf
        SQL &= "      VendorId = " & VendorId & vbCrLf
        SQL &= "    AND" & vbCrLf
        SQL &= "      COALESCE(Rating,0) > 0" & vbCrLf
        SQL &= "   ) r ON rc.RatingCategoryID = r.RatingCategoryID" & vbCrLf
        SQL &= "GROUP BY" & vbCrLf
        SQL &= "  rc.RatingCategoryID, rc.RatingCategory" & vbCrLf
        SQL &= "ORDER BY" & vbCrLf
        SQL &= "  rc.RatingCategory"

        dt = DB.GetDataTable(SQL)
        rptStarDisplay.DataSource = dt
        rptStarDisplay.DataBind()

        Me.ltrRatings.Text = Ratings.ToString
        Me.ltrComments.Text = Comments.ToString

    End Sub

    Protected Sub rptStarDisplay_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptStarDisplay.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlStars As Literal = e.Item.FindControl("ltlStars")
        Dim ltlAverage As Literal = e.Item.FindControl("ltlAverage")
        ltlStars.Text = String.Empty

        Dim i As Integer
        For i = 1 To Math.Ceiling(Core.GetDouble(e.Item.DataItem("Rating")))
            ltlStars.Text &= "<img alt=""" & e.Item.DataItem("Rating") & """ src=""/images/rating/star-red-sm.gif"" />"
        Next

        For i = i To 10
            ltlStars.Text &= "<img alt=""" & e.Item.DataItem("Rating") & """ src=""/images/rating/star-gr-sm.gif"" />"
        Next

        ltlAverage.Text = "<b>" & IIf(Math.Round(Core.GetDouble(e.Item.DataItem("Rating")), 1) > 0, Math.Round(Core.GetDouble(e.Item.DataItem("Rating")), 1), "N/A") & "</b>"
    End Sub
End Class
