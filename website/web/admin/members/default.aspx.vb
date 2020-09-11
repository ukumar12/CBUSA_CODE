Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("MEMBERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Username.Text = Core.ProtectParam(Request("F_Username"))
            F_Email.Text = Core.ProtectParam(Request("F_Email"))
            F_IsActive.Text = Core.ProtectParam(Request("F_IsActive"))
            F_CreateDateLbound.Text = Core.ProtectParam(Request("F_CreateDateLbound"))
            F_CreateDateUbound.Text = Core.ProtectParam(Request("F_CreateDateUbound"))
            F_LastName.Text = Core.ProtectParam(Request("F_LastName"))
            F_Zip.Text = Core.ProtectParam(Request("F_Zip"))
            F_OrderTotalUbound.Text = Core.ProtectParam(Request("F_OrderTotalUbound"))
            F_OrderTotalLBound.Text = Core.ProtectParam(Request("F_OrderTotalLbound"))
            F_NumOrdersUbound.Text = Core.ProtectParam(Request("F_NumOrdersUbound"))
            F_NumOrdersLBound.Text = Core.ProtectParam(Request("F_NumOrdersLbound"))
            F_AvgOrderSizeUBound.Text = Core.ProtectParam(Request("F_AvgOrderSizeUbound"))
            F_AvgOrderSizeLBound.Text = Core.ProtectParam(Request("F_AvgOrderSizeLbound"))
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "m.CreateDate"
                gvList.SortOrder = " DESC"
            End If
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " and "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " m.*,ma.Email, ma.LastName, ma.Zip, (select Count(OrderNo) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId) as OrderCount, (select SUM(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId = m.memberid group by o.MemberId ) as OrderTotal,(select Avg(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId = m.memberid group by o.MemberId ) as AverageOrderSize"
        SQL = BuildQuery()
       
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub ExportList()
        Dim SQLFields, SQL As String

        SQLFields = "SELECT m.*, ma.Email,ma.LastName, ma.Zip, (select Count(OrderNo) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId) as OrderCount, (select SUM(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId = m.memberid group by o.MemberId ) as OrderTotal,(select AVG(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId = m.memberid group by o.MemberId ) as AverageOrderSize"
        SQL = BuildQuery()

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        Dim Folder As String = "/assets/members/"
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Members Report")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine(String.Empty)
        sw.WriteLine("Search Criteria")
        sw.WriteLine("UserName:," & F_Username.Text)
        sw.WriteLine("Email:," & F_Email.Text)
        sw.WriteLine("IsActive:," & F_IsActive.SelectedItem.Text)
        sw.WriteLine("Member Since From:," & F_CreateDateLbound.Text)
        sw.WriteLine("Member Since To:," & F_CreateDateUbound.Text)
        sw.WriteLine("LastName:," & F_LastName.Text)
        sw.WriteLine("Zip:," & F_Zip.Text)
        sw.WriteLine("# Orders From:," & F_NumOrdersLBound.Text)
        sw.WriteLine("# Orders To:," & F_NumOrdersUbound.Text)
        sw.WriteLine("Total Dollars Spent From:," & F_OrderTotalLBound.Text)
        sw.WriteLine("Total Dollars Spent To:," & F_OrderTotalUbound.Text)
		sw.WriteLine("Average Order Size From:," & F_AvgOrderSizeLBound.Text)
		sw.WriteLine("Average Order Size To:," & F_AvgOrderSizeUBound.Text)
        sw.WriteLine(String.Empty)
        sw.WriteLine(String.Empty)

        sw.WriteLine("UserName,Last Name,Email,Zip Code,# Orders,Total Dollars Spent,Average Order Size,MemberSince,IsActive")
        For Each dr As DataRow In res.Rows
            Dim Username As String = IIf(IsDBNull(dr("Username")), String.Empty, dr("Username"))
            Dim LastName As String = IIf(IsDBNull(dr("LastName")), String.Empty, dr("LastName"))
            Dim Email As String = IIf(IsDBNull(dr("Email")), String.Empty, dr("Email"))
            Dim ZipCode As String = IIf(IsDBNull(dr("Zip")), String.Empty, dr("Zip"))
            Dim NumOrders As String = IIf(IsDBNull(dr("OrderCount")), String.Empty, dr("OrderCount"))
            Dim OrderTotal As String = IIf(IsDBNull(dr("OrderTotal")), String.Empty, dr("Ordertotal"))
            Dim AvgOrderSize As String = IIf(IsDBNull(dr("AverageOrderSize")), String.Empty, dr("AverageOrderSize"))
            Dim memberSince As String = IIf(IsDBNull(dr("CreateDate")), String.Empty, Convert.ToString(dr("CreateDate")))
            Dim IsActive As String = String.Empty
            If Convert.ToBoolean(dr("IsActive")) = True Then IsActive = "Yes" Else IsActive = "No"
            sw.WriteLine(Core.QuoteCSV(Username) & "," & Core.QuoteCSV(LastName) & _
                         "," & Core.QuoteCSV(Email) & "," & Core.QuoteCSV(ZipCode) & _
                         "," & Core.QuoteCSV(NumOrders) & "," & Core.QuoteCSV(OrderTotal) & "," & Core.QuoteCSV(AvgOrderSize) & "," & Core.QuoteCSV(memberSince) & "," & Core.QuoteCSV(IsActive))
        Next
        sw.Flush()
        sw.Close()

        lnkDownload.NavigateUrl = Folder & FileName
    End Sub
    Private Function BuildQuery() As String
        Dim Conn As String = " and "

        SQL = " FROM Member m, MemberAddress ma where m.MemberId = ma.MemberId and ma.AddressType = 'Billing'"
        If Not F_Username.Text = String.Empty Then
            SQL = SQL & Conn & "m.Username LIKE " & DB.FilterQuote(F_Username.Text)
            Conn = " AND "
        End If
        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "ma.Email = " & DB.Quote(F_Email.Text)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "m.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CreateDateLbound.Text = String.Empty Then
            SQL = SQL & Conn & "m.CreateDate >= " & DB.Quote(F_CreateDateLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreateDateUbound.Text = String.Empty Then
            SQL = SQL & Conn & "m.CreateDate < " & DB.Quote(DateAdd("d", 1, F_CreateDateUbound.Text))
            Conn = " AND "
        End If
        If Not F_LastName.Text = String.Empty Then
            SQL = SQL & Conn & "ma.LastName like " & DB.StartsWith(F_LastName.Text)
            Conn = " AND "
        End If
        If Not F_Zip.Text = String.Empty Then
            SQL = SQL & Conn & "ma.Zip like " & DB.StartsWith(F_Zip.Text)
            Conn = " AND "
        End If
        If Not F_NumOrdersUbound.Text = String.Empty Then
            SQL = SQL & Conn & "coalesce((select Count(OrderNo) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId),0)<=" & DB.Number(F_NumOrdersUbound.Text)
            Conn = " AND "
        End If
        If Not F_NumOrdersLBound.Text = String.Empty Then
            SQL = SQL & Conn & "coalesce((select Count(OrderNo) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId),0)>=" & DB.Number(F_NumOrdersLBound.Text)
            Conn = " AND "
        End If
        If Not F_OrderTotalUbound.Text = String.Empty Then
            SQL = SQL & Conn & "coalesce((select sum(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId),0)<=" & DB.Number(F_OrderTotalUbound.Text)
            Conn = " AND "
        End If
        If Not F_OrderTotalLBound.Text = String.Empty Then
            SQL = SQL & Conn & "coalesce((select sum(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId),0)>=" & DB.Number(F_OrderTotalLBound.Text)
            Conn = " AND "
        End If
        If Not F_AvgOrderSizeUBound.Text = String.Empty Then
            SQL = SQL & Conn & "coalesce((select avg(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId),0)<=" & DB.Number(F_AvgOrderSizeUBound.Text)
            Conn = " AND "
        End If
        If Not F_AvgOrderSizeLBound.Text = String.Empty Then
            SQL = SQL & Conn & "coalesce((select avg(Total) from StoreOrder o where o.ProcessDate is Not Null and o.MemberId =m.memberid group by o.MemberId),0)>=" & DB.Number(F_AvgOrderSizeLBound.Text)
            Conn = " AND "
        End If
        Return SQL
    End Function
    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        DB.Close()
        Response.Redirect("add.aspx?" & GetPageParams(FilterFieldType.All))
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

    Private Sub gvList_RowDataBound(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim AvgOrderSize As Double = 0.0
            Dim ltlAverageOrderSize As Literal = e.Row.FindControl("ltlAvgOrderSize")
            Dim ltlOrderTotal As Literal = e.Row.FindControl("ltlOrderTotal")
            If Not IsDBNull(e.Row.DataItem("AverageOrderSize")) Then
                ltlAverageOrderSize.Text = FormatCurrency(e.Row.DataItem("AverageOrderSize"))
            Else
                ltlAverageOrderSize.Text = FormatCurrency("0")
            End If
            If Not IsDBNull(e.Row.DataItem("OrderTotal")) Then
                ltlOrderTotal.Text = FormatCurrency(e.Row.DataItem("OrderTotal"))
            Else
                ltlOrderTotal.Text = FormatCurrency("0")
            End If
        End If
	End Sub

	Private Sub btnPlaceOrder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPlaceOrder.Click
		Utility.CookieUtil.SetTripleDESEncryptedCookie("MemberId", Nothing)
		Session("MemberId") = Nothing
		Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"))
	End Sub

	Private Sub gvList_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvList.RowCommand
		Select Case e.CommandName
			Case "Login"
				Session("MemberId") = e.CommandArgument
				Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName"))
		End Select
    End Sub
End Class
