Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("DOCUMENTS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_Title.Text = Request("F_Title")
            F_FileName.Text = Request("F_FileName")
            F_CompanyName.Text = Request("F_CompanyName")
            F_Audience.Text = Request("F_Audience")
            F_IsApproved.Text = Request("F_IsApproved")
            F_UploadedLBound.Text = Request("F_UploadedLBound")
            F_UploadedUBound.Text = Request("F_UploadedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "AdminDocumentID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT "
        If Me.F_Audience.SelectedValue <> String.Empty Then SQLFields &= " DISTINCT "
        SQLFields &= " TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM AdminDocument ad "

        If Not F_Audience.SelectedValues = String.Empty Or Me.F_CompanyName.Text <> String.Empty Then
            If Me.F_Audience.SelectedValues.IndexOf("Builder") > -1 Or Me.F_CompanyName.Text <> String.Empty Then
                If Me.F_CompanyName.Text <> String.Empty Then SQL = SQL & " LEFT "
                SQL = SQL & " JOIN AdminDocumentBuilderRecipient adbr ON ad.AdminDocumentID = adbr.AdminDocumentID "
                If Me.F_CompanyName.Text <> String.Empty Then
                    SQL = SQL & " JOIN Builder b ON b.BuilderID = adbr.BuilderID "
                End If
            End If
            If Me.F_Audience.SelectedValues.IndexOf("Vendor") > -1 Or Me.F_CompanyName.Text <> String.Empty Then
                If Me.F_CompanyName.Text <> String.Empty Then SQL = SQL & " LEFT "
                SQL = SQL & " JOIN AdminDocumentVendorRecipient advr ON ad.AdminDocumentID = advr.AdminDocumentID "
                If Me.F_CompanyName.Text <> String.Empty Then
                    SQL = SQL & " JOIN Vendor v ON v.VendorID = advr.VendorID "
                End If
            End If
            If Me.F_Audience.SelectedValues.IndexOf("PIQ") > -1 Or Me.F_CompanyName.Text <> String.Empty Then
                If Me.F_CompanyName.Text <> String.Empty Then SQL = SQL & " LEFT "
                SQL = SQL & " JOIN AdminDocumentPIQRecipient adpr ON ad.AdminDocumentID = adpr.AdminDocumentID "
                If Me.F_CompanyName.Text <> String.Empty Then
                    SQL = SQL & " JOIN PIQ p ON p.PIQID = adpr.PIQID "
                End If
            End If
        End If

        If Not F_Title.Text = String.Empty Then
            SQL = SQL & Conn & "ad.Title LIKE " & DB.FilterQuote(F_Title.Text)
            Conn = " AND "
        End If
        If Not F_FileName.Text = String.Empty Then
            SQL = SQL & Conn & "ad.FileName LIKE " & DB.FilterQuote(F_FileName.Text)
            Conn = " AND "
        End If
        If Not F_CompanyName.Text = String.Empty Then
            SQL = SQL & Conn & "( b.CompanyName LIKE " & DB.FilterQuote(F_FileName.Text) & " OR  v.CompanyName LIKE " & DB.FilterQuote(F_FileName.Text) & " OR p.CompanyName LIKE " & DB.FilterQuote(F_FileName.Text) & " )"
            Conn = " AND "
        End If
        If Not F_IsApproved.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "ad.IsApproved  = " & DB.Number(F_IsApproved.SelectedValue)
            Conn = " AND "
        End If
        If Not F_UploadedLbound.Text = String.Empty Then
            SQL = SQL & Conn & "ad.Uploaded >= " & DB.Quote(F_UploadedLbound.Text)
            Conn = " AND "
        End If
        If Not F_UploadedUbound.Text = String.Empty Then
            SQL = SQL & Conn & "ad.Uploaded < " & DB.Quote(DateAdd("d", 1, F_UploadedUbound.Text))
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY ad." & gvList.SortByAndOrder)
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
End Class
