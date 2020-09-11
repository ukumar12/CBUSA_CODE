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
            F_VendorID.DataSource = VendorRow.GetActiveList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_HistoricId.Text = Request("F_HistoricId")
            F_LastName.Text = Request("F_LastName")
            F_Username.Text = Request("F_Username")
            F_IsPrimary.Text = Request("F_IsPrimary")

            F_VendorID.SelectedValue = Request("F_VendorID")
            F_CreatedLbound.Text = Request("F_CreatedLBound")
            F_CreatedUbound.Text = Request("F_CreatedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorAccountID"

            BindList()
        End If
    End Sub

    Private Function BuildQuery() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " v.CompanyName as VendorName, va.* "
        SQL = " FROM VendorAccount va Inner Join Vendor v On va.VendorId = v.VendorId  "

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "v.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If
        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "va.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_LastName.Text = String.Empty Then
            SQL = SQL & Conn & "va.LastName LIKE " & DB.FilterQuote(F_LastName.Text)
            Conn = " AND "
        End If
        If Not F_Username.Text = String.Empty Then
            SQL = SQL & Conn & "va.Username LIKE " & DB.FilterQuote(F_Username.Text)
            Conn = " AND "
        End If
        If Not F_IsPrimary.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "va.IsPrimary  = " & DB.Number(F_IsPrimary.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "va.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CreatedLbound.Text = String.Empty Then
            SQL = SQL & Conn & "Created >= " & DB.Quote(F_CreatedLbound.Text)
            Conn = " AND "
        End If
        If Not F_CreatedUbound.Text = String.Empty Then
            SQL = SQL & Conn & "Created < " & DB.Quote(DateAdd("d", 1, F_CreatedUbound.Text))
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub
    Private Sub BindList()
        Dim res As DataTable = BuildQuery()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub



    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName <> "Sort" Then
            Dim id As Integer = e.CommandArgument
            Dim account As VendorAccountRow = VendorAccountRow.GetRow(DB, id)
            Session("VendorAccountId") = account.VendorAccountID
            Session("VendorId") = account.VendorID
            Session("BuilderId") = Nothing
            Session("BuilderAccountId") = Nothing
            Session("PIQId") = Nothing
            If e.CommandName = "LoginNoReg" Then
                Session("SkipRegistrationCheck") = True
            Else
                Session("SkipRegistrationCheck") = False
            End If
            Response.Redirect("/vendor/default.aspx")
        End If

    End Sub

    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub
        ExportReport()

    End Sub
    Public Sub ExportReport()
        gvList.PageSize = 5000
        Dim res As DataTable = BuildQuery()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("First Name , Last Name , Vendor Name , LLC Name , Email")

        Dim sql As String = "Select LLCVendor .*, LLC.LLC as llcName  from LLCVendor inner join LLC on LLC.LLCID = LLCVendor .LLCID  "
        Dim dt As DataTable = DB.GetDataTable(sql)


        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows


                Dim FirstName As String = String.Empty
                If Not IsDBNull(row("FirstName")) Then
                    FirstName = row("FirstName")
                End If
                Dim LastName As String = String.Empty
                If Not IsDBNull(row("LastName")) Then
                    LastName = row("LastName")
                End If
                Dim VendorName As String = String.Empty
                If Not IsDBNull(row("VendorName")) Then
                    VendorName = row("VendorName")
                End If

                Dim LLCName As String = String.Empty

                Dim dLLC As DataRow() = Nothing

                dLLC = dt.Select("VendorID=" & row("VendorID"))
                For Each drow As DataRow In dLLC
                    LLCName &= drow("LLCName") & ","
                Next
                LLCName = LLCName.TrimEnd(",")

                Dim VendorEmail As String = String.Empty
                If Not IsDBNull(row("Email")) Then
                    VendorEmail = row("Email")
                End If
                sw.WriteLine(Core.QuoteCSV(FirstName) & "," & Core.QuoteCSV(LastName) & "," & Core.QuoteCSV(VendorName) & "," & Core.QuoteCSV(LLCName) & "," & Core.QuoteCSV(VendorEmail))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If

    End Sub


    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim vendorid As String = gvList.DataKeys(e.Row.RowIndex).Value.ToString()
        Dim LLCName As Literal = e.Row.FindControl("LLCName")
        LLCName.Text = ListLLC(vendorid)

    End Sub


    Private Function ListLLC(ByVal VendorID As String) As String
        Dim dtLLCPricing As DataTable = VendorRow.GetLLCList(DB, VendorID)
        Dim strLLCPricing As String = String.Empty
        For Each row As DataRow In dtLLCPricing.Rows
            strLLCPricing &= row("LLC") & " ,"
        Next
        If strLLCPricing <> String.Empty AndAlso strLLCPricing.EndsWith(" ,") Then strLLCPricing = strLLCPricing.Substring(0, strLLCPricing.Length - 1)
        Return strLLCPricing
    End Function


End Class

