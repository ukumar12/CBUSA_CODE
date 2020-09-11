Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDORS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))


            F_VendorID.SelectedValue = Request("F_VendorID")
            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "v.CompanyName"
            BindList()
        End If
    End Sub

    Private ReadOnly Property LastQuarter() As Integer
        Get
            Dim ret As Integer = Math.Ceiling(Now.Month / 3) - 1
            Return IIf(ret = 0, 4, ret)
        End Get
    End Property

    Private ReadOnly Property LastYear() As Integer
        Get
            Return IIf(LastQuarter = 4, Now.Year - 1, Now.Year)
        End Get
    End Property



    Private Function BindVendors() As DataTable
        Dim SQLcount, SQL As String
        Dim Conn As String = " AND "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        'SQLFields = "Select v.companyname "
        'SQL = " from SalesReport s inner join Vendor v ON s.vendorID = v.vendorID where periodyear = " & LastYear & " and periodQuarter = " & LastQuarter

        SQLcount = "Select  DISTINCT   sr.*,v.companyname " _
         & " FROM   salesreport AS sr  INNER JOIN dbo.salesreportbuildertotalamount AS srbta " _
          & " ON sr.salesreportid = srbta.salesreportid INNER JOIN vendor v ON v.vendorid = sr.vendorid " _
          & " AND sr.submitted IS NULL  "


        SQL = "Select  DISTINCT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " sr.*,v.companyname " _
          & " FROM   salesreport AS sr  INNER JOIN dbo.salesreportbuildertotalamount AS srbta " _
           & " ON sr.salesreportid = srbta.salesreportid INNER JOIN vendor v ON v.vendorid = sr.vendorid " _
           & " AND sr.submitted IS NULL  "


        If Not F_PeriodQuarter.Text = String.Empty Then
            SQL &= " AND PeriodQuarter = " & DB.Quote(F_PeriodQuarter.Text)
            SQLcount &= " AND PeriodQuarter = " & DB.Quote(F_PeriodQuarter.Text)
        Else
            SQL &= " AND PeriodQuarter = " & DB.Quote(LastQuarter)
            SQLcount &= " AND PeriodQuarter = " & DB.Quote(LastQuarter)
        End If


        If Not F_PeriodYear.Text = String.Empty Then
            SQL &= " AND PeriodYear = " & DB.Quote(F_PeriodYear.Text)
            SQLcount &= " AND PeriodYear = " & DB.Quote(F_PeriodYear.Text)
        Else
            SQL &= " AND PeriodYear = " & DB.Quote(LastYear)
            SQLcount &= " AND PeriodYear = " & DB.Quote(LastYear)
        End If
        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL &= " AND v.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            SQLcount &= " AND v.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
        End If

        If Not F_LLC.SelectedValues = String.Empty Then
            SQL &= " AND v.VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
            SQLcount &= " AND v.VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
        End If


        gvList.Pager.NofRecords = DB.GetDataTable(SQLcount).Rows.Count
        Return DB.GetDataTable(SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BindVendors()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
    Protected Sub btnSubmitAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitAll.Click

        Dim i As Integer = 0
        Dim row As GridViewRow
        Dim isChecked As Boolean = False

        Dim SalesReportID As Integer = 0
        Dim SQL As String

        For i = 0 To gvList.Rows.Count - 1
            row = gvList.Rows(i)
            isChecked = CType(row.FindControl("chkSelect"), CheckBox).Checked

            If isChecked Then
                SalesReportID = CType(row.FindControl("ltlSalesReportID"), Label).Text
                Dim LastUpdatedDate As Date = DB.ExecuteScalar("SELECT MAX(created) from SalesReportBuilderTotalAmount where SalesReportID = " & DB.Number(SalesReportID))
                Dim LastUpdatedByVendorAccount As Integer = DB.ExecuteScalar("SELECT CreatorVENDORACCOUNTID FROM SalesReportBuilderTotalAmount WHERE created = (Select MAX(created) from SalesReportBuilderTotalAmount where SalesReportID = " & DB.Number(SalesReportID) & ")")
                DB.ExecuteSQL("UPDATE SALESREPORT SET SUBMITTED = " & DB.NQuote(LastUpdatedDate) & " , SUBMITTERVENDORACCOUNTID = " & DB.Number(LastUpdatedByVendorAccount) & " ,UpdateAdminId  =" & DB.Number(LoggedInAdminId) & ", UpdatedDate = " & DB.NQuote(Now) & "where SalesReportID = " & DB.Number(SalesReportID))
            End If
        Next

        Response.Redirect("NonReportedVendors.aspx")

    End Sub
    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName <> "SubmitSalesReport" Then
            Exit Sub
        End If
        Dim id As Integer = e.CommandArgument

        Dim LastUpdatedDate As Date = DB.ExecuteScalar("SELECT MAX(created) from SalesReportBuilderTotalAmount where SalesReportID = " & DB.Number(id))
        Dim LastUpdatedByVendorAccount As Integer = DB.ExecuteScalar("SELECT CreatorVENDORACCOUNTID FROM SalesReportBuilderTotalAmount WHERE created = (Select MAX(created) from SalesReportBuilderTotalAmount where SalesReportID = " & DB.Number(id) & ")")

        
        If e.CommandName = "SubmitSalesReport" Then
            DB.ExecuteSQL("UPDATE SALESREPORT SET SUBMITTED = " & DB.NQuote(LastUpdatedDate) & " , SUBMITTERVENDORACCOUNTID = " & DB.Number(LastUpdatedByVendorAccount) & " ,UpdateAdminId  =" & DB.Number(LoggedInAdminId) & ", UpdatedDate = " & DB.NQuote(Now) & "where SalesReportID = " & DB.Number(id))
        End If
        BindList()

    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If



        Dim ltlUpdateDate As Literal = e.Row.FindControl("ltlUpdateDate")



        ltlUpdateDate.Text = DB.ExecuteScalar("SELECT MAX(created) from SalesReportBuilderTotalAmount where SalesReportID = " & DB.Number(e.Row.DataItem("SalesReportID")))

    End Sub
End Class

 
     



 





 
