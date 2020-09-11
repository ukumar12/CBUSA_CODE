Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDER_ACCOUNTS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_BuilderID.DataSource = BuilderRow.GetActiveList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.DataBind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_HistoricId.Text = Request("F_HistoricId")
            F_LastName.Text = Request("F_LastName")
            F_Username.Text = Request("F_Username")
            F_Email.Text = Request("F_Email")
            F_IsPrimary.Text = Request("F_IsPrimary")

            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_CreatedLbound.Text = Request("F_CreatedLBound")
            F_CreatedUbound.Text = Request("F_CreatedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "BuilderAccountID"

            If Request("F_BuilderID") IsNot Nothing Then
                lnkEditBuilder.NavigateUrl = "/admin/builders/edit.aspx?BuilderId=" & Request("F_BuilderID")
            Else
                lnkEditBuilder.Visible = False
            End If
            lnkReturn.NavigateUrl = "/admin/builders/default.aspx?" & GetPageParams(FilterFieldType.All, "BuilderAccountId")
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " a.*, b.CompanyName, b.HistoricID As HID "
        SQL = " FROM BuilderAccount a inner join Builder b on a.BuilderID=b.BuilderID  "

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "b.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If

        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "a.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_LastName.Text = String.Empty Then
            SQL = SQL & Conn & "a.LastName LIKE " & DB.FilterQuote(F_LastName.Text)
            Conn = " AND "
        End If
        If Not F_Username.Text = String.Empty Then
            SQL = SQL & Conn & "a.Username LIKE " & DB.FilterQuote(F_Username.Text)
            Conn = " AND "
        End If
        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "a.Email LIKE " & DB.FilterQuote(F_Email.Text)
            Conn = " AND "
        End If
        If Not F_IsPrimary.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "a.IsPrimary  = " & DB.Number(F_IsPrimary.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "a.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CreatedLbound.Text = String.Empty Then
            SQL = SQL & Conn & "a.Created >= " & DB.Quote(F_CreatedLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreatedUbound.Text = String.Empty Then
            SQL = SQL & Conn & "a.Created < " & DB.Quote(DateAdd("d", 1, F_CreatedUbound.Text))
            Conn = " AND "
        End If

        SQL = SQL & Conn & " FlagForExistingUser is null"

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    'Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
    '    Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    'End Sub
    Private Function GetBuilderAccountData() As DataTable

        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " a.*, b.CompanyName, b.HistoricID As HID "
        SQL = " FROM BuilderAccount a inner join Builder b on a.BuilderID=b.BuilderID  "

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "b.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If

        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "a.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_LastName.Text = String.Empty Then
            SQL = SQL & Conn & "a.LastName LIKE " & DB.FilterQuote(F_LastName.Text)
            Conn = " AND "
        End If
        If Not F_Username.Text = String.Empty Then
            SQL = SQL & Conn & "a.Username LIKE " & DB.FilterQuote(F_Username.Text)
            Conn = " AND "
        End If
        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "a.Email LIKE " & DB.FilterQuote(F_Email.Text)
            Conn = " AND "
        End If
        If Not F_IsPrimary.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "a.IsPrimary  = " & DB.Number(F_IsPrimary.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "a.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CreatedLbound.Text = String.Empty Then
            SQL = SQL & Conn & "a.Created >= " & DB.Quote(F_CreatedLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreatedUbound.Text = String.Empty Then
            SQL = SQL & Conn & "a.Created < " & DB.Quote(DateAdd("d", 1, F_CreatedUbound.Text))
            Conn = " AND "
        End If
        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function
    Public Sub ExportList()
        gvList.PageSize = 5000
        Dim res As DataTable = GetBuilderAccountData()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("CompanyName ,FirstName,LastName, Email, IsPrimary, IsActive, Phone ,Mobile , Fax ")
        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CompanyName As String = row("CompanyName")
                Dim FirstName As String = String.Empty
                Dim LastName As String = String.Empty
                Dim Email As String = String.Empty
                Dim IsPrimary As String = String.Empty
                Dim IsActive As String = String.Empty
                Dim Phone As String = String.Empty
                Dim Mobile As String = String.Empty
                Dim Fax As String = String.Empty

                If Not IsDBNull(row("FirstName")) Then
                    FirstName = row("FirstName")
                Else
                    FirstName = ""
                End If
                If Not IsDBNull(row("LastName")) Then
                    LastName = row("LastName")
                Else
                    LastName = ""
                End If

                If row("IsActive") Then
                    IsActive = "YES"
                Else
                    IsActive = "NO"
                End If


                If row("IsPrimary") Then
                    IsPrimary = "YES"
                Else
                    IsPrimary = "NO"
                End If

                If Not IsDBNull(row("Email")) Then
                    Email = row("Email")
                End If
                If Not IsDBNull(row("Phone")) Then
                    Phone = row("Phone")
                End If

                If Not IsDBNull(row("Mobile")) Then
                    Mobile = row("Mobile")
                End If
                If Not IsDBNull(row("Fax")) Then
                    Fax = row("Fax")
                End If
                sw.WriteLine(Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(FirstName) & "," & Core.QuoteCSV(LastName) & "," & Core.QuoteCSV(Email) & "," & Core.QuoteCSV(IsPrimary) & "," & Core.QuoteCSV(IsActive) & "," & Core.QuoteCSV(Phone) & "," & Core.QuoteCSV(Mobile) & "," & Core.QuoteCSV(Fax))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub
    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        ExportList()
    End Sub
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName <> "Login" And e.CommandName <> "LoginNoReg" Then
            Exit Sub
        End If
        Dim id As Integer = e.CommandArgument
        Dim account As BuilderAccountRow = BuilderAccountRow.GetRow(DB, id)
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, account.BuilderID)
        Session("BuilderAccountId") = account.BuilderAccountID
        Session("BuilderId") = account.BuilderID
        If dbBuilder.Guid = Nothing Then
            dbBuilder.UpdateGuid(Core.GenerateFileID)
            'dbBuilder.Guid = Core.GenerateFileID
            'dbBuilder.Update()
        End If
        'Session("BuilderGuid") = dbBuilder.Guid
        Session("VendorId") = Nothing
        Session("VendorAccountId") = Nothing
        Session("PIQId") = Nothing
        Session("CurrentTakeoffID") = Nothing
        Session("TakeoffForID") = Nothing
        If e.CommandName = "Login" Then
            Session("SkipRegistrationCheck") = False
        Else
            Session("SkipRegistrationCheck") = True
        End If
        Response.Redirect("/builder/default.aspx")
    End Sub
End Class
