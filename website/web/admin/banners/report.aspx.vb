Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Report
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BANNERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_CreateDateLbound.Text = Request("F_CreateDateLBound")
            F_CreateDateUbound.Text = Request("F_CreateDateUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "FullName"

            LoadFromDB()
            BindList()
        End If
    End Sub

    Private Sub LoadFromDB()
        F_BannerId.DataSource = BannerRow.GetActiveBanners(DB)
        F_BannerId.DataTextField = "Name"
        F_BannerId.DataValueField = "BannerId"
        F_BannerId.DataBind()
        F_BannerId.Items.Insert(0, New ListItem("-- ALL --", ""))

        F_BannerGroupId.DataSource = BannerGroupRow.GetList(DB)
        F_BannerGroupId.DataTextField = "Name"
        F_BannerGroupId.DataValueField = "BannerGroupId"
        F_BannerGroupId.DataBind()
        F_BannerGroupId.Items.Insert(0, New ListItem("-- ALL --", ""))
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        SQLFields = "select *, Name + ' (' + filename + ')' as FullName, case when ImpressionCount = 0 then 0 else round(cast(ClickCount as float) * 100 / ImpressionCount,2) end as ClickRate, CASE WHEN ClickCount = 0 THEN 0 ELSE ROUND(NofOrders * 100 / ClickCount,2) END AS Conversion "

        SQL = String.Empty
        SQL &= " from ( "
        SQL &= " select b.BannerId,b.Name, b.link, b.AltText, b.Filename, sum(ImpressionCount) as ImpressionCount, sum(ClickCount) as ClickCount,"
        SQL &= " COALESCE((SELECT SUM(1/coalesce(cast(NofBanners as float),1)) FROM StoreOrder so, (select OrderId, count(BannerId) as NofBanners from BannerOrderTracking group by OrderId) bt WHERE so.OrderId = bt.orderid and so.ProcessDate IS NOT NULL AND so.OrderId in (select OrderId from BannerOrderTracking where BannerId = b.BannerId)),0) AS NofOrders, "
        SQL &= " COALESCE((SELECT SUM(Total/coalesce(NofBanners,1)) FROM StoreOrder so, (select OrderId, count(BannerId) as NofBanners from BannerOrderTracking group by OrderId) bt  WHERE so.OrderId = bt.orderid and ProcessDate IS NOT NULL AND so.OrderId in (select OrderId from BannerOrderTracking where BannerId = b.BannerId)),0) AS Total,"
        SQL &= " COALESCE((SELECT AVG(Total/coalesce(NofBanners,1)) FROM StoreOrder so, (select OrderId, count(BannerId) as NofBanners from BannerOrderTracking group by OrderId) bt  WHERE so.OrderId = bt.orderid and ProcessDate IS NOT NULL AND so.OrderId in (select OrderId from BannerOrderTracking where BannerId = b.BannerId)),0) AS AvgOrder "
        SQL &= " from BannerTracking btr, Banner b "
        SQL &= " where	b.BannerId = btr.BannerId "

        If Not F_CreateDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "btr.CreateDate >= " & DB.Quote(F_CreateDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "btr.CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_BannerId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "btr.BannerId = " & DB.Number(F_BannerId.SelectedValue)
            Conn = " AND "
        End If
        If Not F_BannerGroupId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "btr.BannerId in (select BannerId from BannerBannerGroup where btr.BannerId = BannerId and BannerGroupId = " & DB.Number(F_BannerGroupId.SelectedValue) & ")"
            Conn = " AND "
		End If
		If Not F_IsActive.SelectedValue = String.Empty Then
			SQL &= Conn & "b.isactive = " & DB.Number(F_IsActive.SelectedValue)
			Conn = " AND "
		End If
        SQL &= " group by b.BannerId, b.Name, b.link, b.filename, b.AltText "
        SQL &= " ) as tmp"

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataSet = DB.GetDataSet(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.Tables(0).DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = ListItemType.Item Or e.Row.RowType = ListItemType.AlternatingItem) Then
            Exit Sub
        End If
        Dim img As Literal = CType(e.Row.FindControl("img"), Literal)
        Dim imglink As Label = CType(e.Row.FindControl("imglink"), Label)
        Dim noimg As Label = CType(e.Row.FindControl("noimg"), Label)
        If Convert.ToString(e.Row.DataItem("FileName")) <> String.Empty Then
            img.Text = "<img src=""/assets/banner/" & e.Row.DataItem("FileName") & """>"
            imglink.Visible = True
            noimg.Visible = False
        Else
            imglink.Visible = False
            noimg.Visible = True
        End If
    End Sub
End Class

