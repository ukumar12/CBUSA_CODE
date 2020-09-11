Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO
Imports System.Data.SqlClient

Partial Class Index
    Inherits AdminPage

    Private PageAvgPrice As Double = 0
    Private PageQuantity As Integer = 0
    Private PageTotal As Double = 0
    Private AvgPrice As Double = 0
    Private Quantity As Integer = 0
    Private Total As Double = 0

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("REPORTS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_Status.DataSource = StoreOrderStatusRow.GetAllStoreOrderStatuss(DB)
            F_Status.DataValueField = "Code"
            F_Status.DataTextField = "Name"
            F_Status.DataBind()
            F_Status.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_DiscoverySources.DataSource = HowHeardRow.GetAllHowHeard(DB)
            F_DiscoverySources.DataTextField = "HowHeard"
            F_DiscoverySources.DataValueField = "HowHeardId"
            F_DiscoverySources.DataBind()
            F_DiscoverySources.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_ProcessDateLbound.Text = Request("F_ProcessDateLBound")
            F_ProcessDateUbound.Text = Request("F_ProcessDateUBound")
            F_SKU.Text = Request("F_SKU")
            F_ItemName.Text = Request("F_ItemName")

			F_ReferralCode.Text = Request("F_ReferralCode")
			F_PromotionCode.Text = Request("F_PromotionCode")

            F_DepartmentId.SelectedValue = Request("F_DepartmentId")
            BindDepartmentsDropDown(DB, F_DepartmentId, 5, "-- ALL --")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "ItemName"

            BindList()
        End If
    End Sub

    Private Sub BindDepartmentsDropDown(ByVal DB As Database, ByVal DepartmentId As DropDownList, ByVal Level As Integer, ByVal FirstField As String)
        Dim SQL As String = String.Empty
        If Level = Nothing Then
            SQL = "SELECT * FROM StoreDepartment ORDER BY NAME ASC"
        Else
            SQL = " SELECT P1.DepartmentId, REPLICATE('   ', COUNT(P2.DepartmentId)-2) + p1.NAME AS NAME" _
                & " FROM StoreDepartment P1, StoreDepartment P2" _
                & " WHERE P1.lft BETWEEN P2.lft AND P2.rgt" _
                & " GROUP BY P1.DepartmentId, P1.lft, p1.rgt, p1.NAME" _
                & " HAVING COUNT(P2.DepartmentId) > 1 AND COUNT(P2.DepartmentId) <= " & DB.Quote((Level + 1).ToString) _
                & " ORDER BY P1.lft"
        End If
        Dim ds As DataTable = DB.GetDataTable(SQL)
        DepartmentId.DataSource = ds
        DepartmentId.DataTextField = "Name"
        DepartmentId.DataValueField = "DepartmentId"
        DepartmentId.DataBind()
        DepartmentId.Items.Insert(0, New ListItem(FirstField, ""))
    End Sub

    Private Function BuildQuery() As String
        Dim Conn As String = " where "

        Dim SQL As String = " FROM StoreSalesView ssv"
        If Not F_SKU.Text = String.Empty Then
            SQL = SQL & Conn & "SKU LIKE " & DB.StartsWith(F_SKU.Text)
            Conn = " AND "
        End If
        If Not F_ItemName.Text = String.Empty Then
            SQL = SQL & Conn & "ItemName LIKE " & DB.FilterQuote(F_ItemName.Text)
            Conn = " AND "
        End If
        If Not F_ProcessDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "ProcessDate >= " & DB.Quote(F_ProcessDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_ProcessDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "ProcessDate < " & DB.Quote(DateAdd("d", 1, F_ProcessDateUbound.Text))
            Conn = " AND "
		End If

		If Not F_ReferralCode.Text = String.Empty Then
			SQL &= Conn & "ReferralCode = " & DB.Quote(F_ReferralCode.Text)
			Conn = " AND "
		End If

		If Not F_PromotionCode.Text = String.Empty Then
			SQL &= Conn & "PromotionCode = " & DB.Quote(F_PromotionCode.Text)
            Conn = " AND "
        End If

        If Not DB.IsEmpty(F_DiscoverySources.SelectedValue) Then
            SQL = SQL & Conn & "ssv.HowHeardId=" & DB.Number(F_DiscoverySources.SelectedValue)
            Conn = " AND "
        End If

        If Not DB.IsEmpty(F_DepartmentId.SelectedValue) Then
            SQL &= Conn & " DepartmentId in ("
            SQL &= " select sd.DepartmentId from StoreDepartment sd, StoreDepartment p "
            SQL &= " where p.DepartmentId = " & DB.Number(F_DepartmentId.SelectedValue)
            SQL &= " and p.lft <= sd.Lft and p.rgt >= sd.rgt"
            SQL &= " )"
            Conn = " AND "
        End If
        If F_Status.SelectedValue <> "" Then
            SQL = SQL & Conn & "Status = " & DB.Quote(F_Status.SelectedValue)
            Conn = " AND "
        End If
        Return SQL
    End Function

    Private Sub ExportList()
        Dim SQLFields, SQL As String

        SQLFields = "SELECT ItemName, SKU, SUM(Price * Quantity)/SUM(Quantity) AS AvgPrice, SUM(Quantity) AS Quantity, SUM(Price * Quantity) AS TotalPrice"

        SQL = BuildQuery()
        SQL &= " GROUP BY SKU, ItemName "

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        Dim Folder As String = "/assets/sales/"
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Sales Report ")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine(String.Empty)
        sw.WriteLine("Search Criteria")
        sw.WriteLine("Order Date From:," & F_ProcessDateLbound.Text)
        sw.WriteLine("Order Date To:," & F_ProcessDateUbound.Text)
        sw.WriteLine("Item #:," & F_SKU.Text)
        sw.WriteLine("Item Name:," & F_ItemName.Text)
        sw.WriteLine("Department:," & F_DepartmentId.SelectedItem.Text)
        sw.WriteLine("Status:," & F_Status.SelectedItem.Text)
        sw.WriteLine("Discovery Source:," & F_DiscoverySources.SelectedItem.Text)
        sw.WriteLine(String.Empty)
        sw.WriteLine(String.Empty)

        sw.WriteLine("Item Number,Item Name,Avg Price,Units,Total,Discovery Source")
        For Each dr As DataRow In res.Rows
            Dim SKU As String = IIf(IsDBNull(dr("SKU")), String.Empty, "'" & dr("SKU"))
            Dim ItemName As String = IIf(IsDBNull(dr("ItemName")), String.Empty, dr("ItemName"))
            Dim AvgPrice As String = IIf(IsDBNull(dr("AvgPrice")), String.Empty, dr("AvgPrice"))
            Dim Quantity As String = IIf(IsDBNull(dr("Quantity")), String.Empty, dr("Quantity"))
            Dim TotalPrice As String = IIf(IsDBNull(dr("TotalPrice")), String.Empty, dr("TotalPrice"))
            sw.WriteLine(Core.QuoteCSV(SKU) & "," & Core.QuoteCSV(ItemName) & "," & Core.QuoteCSV(AvgPrice) & "," & Core.QuoteCSV(Quantity) & "," & Core.QuoteCSV(TotalPrice))
        Next
        sw.Flush()
        sw.Close()

        lnkDownload.NavigateUrl = Folder & FileName
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " SKU, ItemName, SUM(Price * Quantity)/SUM(Quantity) AS AvgPrice, SUM(Quantity) AS Quantity, SUM(Price * Quantity) AS TotalPrice "

        SQL = BuildQuery()

        Dim dr As SqlDataReader = DB.GetReader("select SUM(Price * Quantity)/SUM(Quantity) AS AvgPrice, SUM(Quantity) AS Quantity, SUM(Price * Quantity) AS TotalPrice " & SQL)
        If dr.Read Then
            AvgPrice = IIf(IsDBNull(dr("AvgPrice")), 0, dr("AvgPrice"))
            Quantity = IIf(IsDBNull(dr("Quantity")), 0, dr("Quantity"))
            Total = IIf(IsDBNull(dr("TotalPrice")), 0, dr("TotalPrice"))
        End If
        dr.Close()

        SQL &= " GROUP BY SKU, ItemName"

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT count(*) from (select SKU, ItemName " & SQL & ") as tmp")


        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0

        If F_OutputAs.SelectedValue = "Excel" Then
            divDownload.Visible = True
            gvList.Visible = False
            ExportList()
        Else
            divDownload.Visible = False
            gvList.Visible = True
            BindList()
        End If
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            PageAvgPrice += e.Row.DataItem("AvgPrice") * e.Row.DataItem("Quantity")
            PageQuantity += e.Row.DataItem("Quantity")
            PageTotal += e.Row.DataItem("TotalPrice")
        End If
        If e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(1).Text = "Page Total:<br/>Total:"
            e.Row.Cells(2).Text = FormatCurrency(PageAvgPrice / PageQuantity) & "<br />" & FormatCurrency(AvgPrice)
            e.Row.Cells(3).Text = PageQuantity & "<br />" & Quantity
            e.Row.Cells(4).Text = FormatCurrency(PageTotal) & "<br />" & FormatCurrency(Total)
        End If
    End Sub
End Class

