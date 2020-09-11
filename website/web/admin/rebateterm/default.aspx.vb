Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
	Inherits AdminPage

	Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDOR_ACCOUNTS")

		gvList.BindList = AddressOf BindList
		If Not IsPostBack Then
			F_VendorID.Datasource = VendorRow.GetList(DB,"CompanyName")
			F_VendorID.DataValueField = "VendorID"
			F_VendorID.DataTextField = "CompanyName"
			F_VendorID.Databind
			F_VendorID.Items.Insert(0, New ListItem("-- ALL --",""))

            F_HistoricId.Text = Request("F_HistoricId")
            F_VendorID.SelectedValue = Request("F_VendorID")

            F_VendorID.SelectedValue = Request("F_VendorID")
            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()

            If Not Request("F_StartYear") Is Nothing Then
                F_StartYear.Text = Request("F_StartYear")
            Else
                F_StartYear.Text = Now.Year
            End If
            If Not Request("F_StartQuarter") Is Nothing Then
                F_StartQuarter.Text = Request("F_StartQuarter")
            Else
                F_StartQuarter.Text = Math.Ceiling(Now.Month / 3)
            End If

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "RebateTermsID"

            BindList()
        End If
	End Sub

    Private Function BindRebateTerms() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " rt.*, v.CompanyName As Vendor, v.HistoricID As VendorHistoricID"
        SQL = " FROM RebateTerm rt Inner Join Vendor v On rt.VendorId = v.VendorId "

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "v.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "rt.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If

        If Not F_StartYear.Text = String.Empty Then
            SQL = SQL & Conn & "StartYear = " & DB.Quote(F_StartYear.Text)
            Conn = " AND "
        End If

        If Not F_StartQuarter.Text = String.Empty Then
            SQL = SQL & Conn & "StartQuarter = " & DB.Quote(F_StartQuarter.Text)
            Conn = " AND "
        End If

        If Not F_LLC.SelectedValues = String.Empty Then
            SQL = SQL & Conn & " v.VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        Return DB.GetDataTable(SQLFields & SQL & "ORDER BY " & gvList.SortByAndOrder)

       
    End Function

    Private Sub BindList()
        Dim res As DataTable = BindRebateTerms()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

	Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("add.aspx?" & GetPageParams(FilterFieldType.All))
	End Sub

	Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
		If Not IsValid Then Exit Sub
		
		gvList.PageIndex = 0
		BindList()
    End Sub

    Protected Sub export_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Export.Click
        If Not IsValid Then Exit Sub
        ExportReport()
    End Sub
    Public Sub ExportReport()
        gvList.PageSize = 5000
        Dim res As DataTable = BindRebateTerms()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Vendor Name , VendorHistoricID , Start Year , Start Quarter , Rebate Percentage, Created,VendorLLC")

        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim Vendor As String = row("Vendor")

                Dim VendorHistoricID As String = String.Empty
                If Not IsDBNull(row("VendorHistoricID")) Then
                    VendorHistoricID = row("VendorHistoricID")
                Else
                    VendorHistoricID = " "
                End If
                Dim StartYear As String = String.Empty
                If Not IsDBNull(row("StartYear")) Then
                    StartYear = row("StartYear")
                Else
                    StartYear = " "
                End If
                Dim StartQuarter As String = String.Empty
                If Not IsDBNull(row("StartQuarter")) Then
                    StartQuarter = row("StartQuarter")
                Else
                    StartQuarter = " "
                End If
                Dim RebatePercentage As String = String.Empty
                If Not IsDBNull(row("RebatePercentage")) Then
                    RebatePercentage = row("RebatePercentage")
                Else
                    RebatePercentage = " "
                End If

                Dim Created As String = Nothing
                If Not IsDBNull(row("Created")) Then
                    Created = row("Created")
                Else
                    Created = " "
                End If
                Dim VendorLLC As String = String.Empty
                If Not IsDBNull(ListLLC(row("VendorID"))) Then
                    VendorLLC = ListLLC(row("VendorID"))
                Else
                    VendorLLC = " "
                End If

                sw.WriteLine(Core.QuoteCSV(Vendor) & "," & Core.QuoteCSV(VendorHistoricID) & "," & Core.QuoteCSV(StartYear) & "," & Core.QuoteCSV(StartQuarter) & "," & Core.QuoteCSV(RebatePercentage) & "," & Core.QuoteCSV(Created) & "," & Core.QuoteCSV(VendorLLC))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub
    Private Function ListLLC(ByVal VendorID As String) As String

        Dim dtLLCPricing As DataTable = VendorRow.GetLLCList(DB, VendorID)
        Dim strLLCPricing As String = String.Empty

        For Each row As DataRow In dtLLCPricing.Rows
            strLLCPricing &= row("LLC") & "|"
        Next

        If strLLCPricing <> String.Empty AndAlso strLLCPricing.EndsWith("|") Then strLLCPricing = strLLCPricing.Substring(0, strLLCPricing.Length - 1)

        Return strLLCPricing

    End Function
    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim vendorid As String = gvList.DataKeys(e.Row.RowIndex).Value.ToString()
        Dim LLCName As Literal = e.Row.FindControl("LLCName")
        LLCName.Text = ListLLC(vendorid)

    End Sub
End Class
