  
Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_BuilderID.DataSource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.DataBind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))


            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "b.CompanyName"
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

        SQLcount = "Select  DISTINCT   pr.*,b.companyname ,b.llcID" _
         & " FROM   PurchasesReport AS pr  INNER JOIN dbo.PurchasesReportVendorTotalAmount AS prvta " _
          & " ON pr.PurchasesReportid = prvta.PurchasesReportid INNER JOIN Builder b ON b.BuilderID = pr.BuilderID " _
          & " AND pr.submitted IS NULL  "


        SQL = "Select  DISTINCT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " pr.*,b.companyname,b.llcID " _
          & " FROM   PurchasesReport AS pr  INNER JOIN dbo.PurchasesReportVendorTotalAmount AS prvta " _
           & " ON pr.PurchasesReportid = prvta.PurchasesReportid INNER JOIN Builder b ON b.BuilderID = pr.BuilderID " _
           & " AND pr.submitted IS NULL  "


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
        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL &= " AND b.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            SQLcount &= " AND b.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
        End If

        If Not F_LLC.SelectedValues = String.Empty Then
            SQL &= " AND b.llcID in  " & DB.NumberMultiple(F_LLC.SelectedValues)
            SQLcount &= " AND b.llcID in  " & DB.NumberMultiple(F_LLC.SelectedValues)
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

        Dim PurchasesReportID As Integer = 0
        Dim SQL As String

        For i = 0 To gvList.Rows.Count - 1
            row = gvList.Rows(i)
            isChecked = CType(row.FindControl("chkSelect"), CheckBox).Checked

            If isChecked Then
                PurchasesReportID = CType(row.FindControl("ltlPurchasesReportID"), Label).Text
                Dim LastUpdatedDate As Date = DB.ExecuteScalar("SELECT MAX(created) from PurchasesReportVendorTotalAmount where PurchasesReportID = " & DB.Number(PurchasesReportID))
                Dim LastUpdatedByVendorAccount As Integer = DB.ExecuteScalar("SELECT CreatorBuilderACCOUNTID FROM PurchasesReportVendorTotalAmount WHERE created = (Select MAX(created) from PurchasesReportVendorTotalAmount where PurchasesReportID = " & DB.Number(PurchasesReportID) & ")")
                DB.ExecuteSQL("UPDATE PurchasesReport SET SUBMITTED = " & DB.NQuote(LastUpdatedDate) & " , SUBMITTERBUILDERACCOUNTID = " & DB.Number(LastUpdatedByVendorAccount) & " ,UpdateAdminId  =" & DB.Number(LoggedInAdminId) & ", UpdatedDate = " & DB.NQuote(Now) & "where PurchasesReportID = " & DB.Number(PurchasesReportID))
            End If
        Next

        Response.Redirect("NonReportedBuilders.aspx")

    End Sub
    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName <> "SubmitPurchasesReport" Then
            Exit Sub
        End If
        Dim id As Integer = e.CommandArgument

        Dim LastUpdatedDate As Date = DB.ExecuteScalar("SELECT MAX(created) from PurchasesReportVendorTotalAmount where PurchasesReportID = " & DB.Number(id))
        Dim LastUpdatedByBuilderAccount As Integer = DB.ExecuteScalar("SELECT CreatorBuilderAccountID FROM PurchasesReportVendorTotalAmount WHERE created = (Select MAX(created) from SalesReportBuilderTotalAmount where SalesReportID = " & DB.Number(id) & ")")


        If e.CommandName = "SubmitPurchasesReport" Then
            DB.ExecuteSQL("UPDATE PurchasesReport SET SUBMITTED = " & DB.NQuote(LastUpdatedDate) & " , SUBMITTERBuilderACCOUNTID = " & DB.Number(LastUpdatedByBuilderAccount) & " ,UpdateAdminId  =" & DB.Number(LoggedInAdminId) & ", UpdatedDate = " & DB.NQuote(Now) & "where PurchasesReportID = " & DB.Number(id))
        End If
        BindList()

    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If



        Dim ltlUpdateDate As Literal = e.Row.FindControl("ltlUpdateDate")



        ltlUpdateDate.Text = DB.ExecuteScalar("SELECT MAX(created) from PurchasesReportVendorTotalAmount where PurchasesReportID = " & DB.Number(e.Row.DataItem("PurchasesReportID")))

    End Sub
End Class













