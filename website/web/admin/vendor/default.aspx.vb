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
            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()

            F_State.DataSource = StateRow.GetStateList(DB)
            F_State.DataValueField = "StateCode"
            F_State.DataTextField = "StateCode"
            F_State.DataBind()
            F_State.Items.Insert(0, New ListItem("-- ALL --", ""))
            F_HistoricId.Text = Request("F_HistoricId")
            F_CompanyName.Text = Request("F_CompanyName")
            F_City.Text = Request("F_City")
            F_Email.Text = Request("F_Email")
            F_WebsiteURL.Text = Request("F_WebsiteURL")
            F_State.SelectedValue = Request("F_State")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorID"

            BindList()
        End If
    End Sub
    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub
        ExportReport()

    End Sub

    Public Sub ExportReport()
        gvList.PageSize = 5000
        Dim res As DataTable = Nothing
        Dim SqlEXPORT As String = String.Empty
        SqlEXPORT = "Select * from Vendor v  "
        Dim CONN As String = " where "
        If Not F_HistoricId.Text = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "v.HistoricId = " & DB.Quote(F_HistoricId.Text)
            CONN = " AND "
        End If

        If Not F_WebsiteURL.Text = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "WebsiteURL = " & DB.Quote(F_WebsiteURL.Text)
            CONN = " AND "
        End If
        If Not F_State.SelectedValue = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "State = " & DB.Quote(F_State.SelectedValue)
            CONN = " AND "
        End If
        If Not F_CompanyName.Text = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
            CONN = " AND "
        End If
        If Not F_City.Text = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "City LIKE " & DB.FilterQuote(F_City.Text)
            CONN = " AND "
        End If
        If Not F_Email.Text = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "Email LIKE " & DB.FilterQuote(F_Email.Text)
            CONN = " AND "
        End If
        If Not F_LLC.SelectedValues = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & " VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
            CONN = " AND "
        End If

        If Not F_IsActive.SelectedValue = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "v.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            CONN = " AND "
        End If

        If Not F_IsExcluded.SelectedValue = String.Empty Then
            SqlEXPORT = SqlEXPORT & CONN & "v.ExcludedVendor  = " & DB.Number(F_IsExcluded.SelectedValue)
            CONN = " AND "
        End If



        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Vendor,HistoricID,Markets, Address, Address2, City, State, Zip, Phone, Email, WebsiteURL, IsActive  ")


        res = DB.GetDataTable(SqlEXPORT)
        Dim sql As String = "Select LLCVendor .*, LLC.LLC as llcName  from LLCVendor inner join LLC on LLC.LLCID = LLCVendor .LLCID  "
        Dim dt As DataTable = DB.GetDataTable(sql)

        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows
                Dim CompanyName As String = String.Empty
                If Not IsDBNull(row("CompanyName")) Then
                    CompanyName = row("CompanyName")
                End If
                Dim HistoricID As String = String.Empty
                If Not IsDBNull(row("HistoricID")) Then
                    HistoricID = row("HistoricID")
                End If
                Dim Markets As String = String.Empty
                Dim dLLC As DataRow() = Nothing

                dLLC = dt.Select("VendorID=" & row("VendorID"))
                For Each drow As DataRow In dLLC
                    Markets &= drow("LLCName") & ","
                Next
                Markets = Markets.TrimEnd(",")
                Dim Address As String = String.Empty
                If Not IsDBNull(row("Address")) Then
                    Address = row("Address")
                End If
                Dim Address2 As String = String.Empty
                If Not IsDBNull(row("Address2")) Then
                    Address2 = row("Address2")
                End If

                Dim City As String = String.Empty
                If Not IsDBNull(row("City")) Then
                    City = row("City")
                End If
                Dim State As String = String.Empty
                If Not IsDBNull(row("State")) Then
                    State = row("State")
                End If

                Dim Zip As String = String.Empty
                If Not IsDBNull(row("Zip")) Then
                    Zip = row("Zip")
                End If

                Dim Phone As String = String.Empty
                If Not IsDBNull(row("Phone")) Then
                    Phone = row("Phone")
                End If

                Dim Email As String = String.Empty
                If Not IsDBNull(row("Email")) Then
                    Email = row("Email")
                End If
                Dim WebsiteURL As String = String.Empty
                If Not IsDBNull(row("WebsiteURL")) Then
                    WebsiteURL = row("WebsiteURL")
                End If
                Dim IsActive As String = String.Empty
                If Convert.ToBoolean(row("IsActive")) = True Then
                    IsActive = "Yes"
                Else
                    IsActive = "No"
                End If

                sw.WriteLine(Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(HistoricID) & "," & Core.QuoteCSV(Markets) & "," & Core.QuoteCSV(Address) & "," & Core.QuoteCSV(Address2) & "," & Core.QuoteCSV(City) & "," & Core.QuoteCSV(State) & "," & Core.QuoteCSV(Zip) & "," & Core.QuoteCSV(Phone) & "," & Core.QuoteCSV(Email) & "," & Core.QuoteCSV(WebsiteURL) & "," & Core.QuoteCSV(IsActive))
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
            strLLCPricing &= row("LLC") & " ,"
        Next
        If strLLCPricing <> String.Empty AndAlso strLLCPricing.EndsWith(" ,") Then strLLCPricing = strLLCPricing.Substring(0, strLLCPricing.Length - 1)
        Return strLLCPricing
    End Function
    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " *, Coalesce((SELECT Top 1 'True' FROM VendorRegistration vr WHERE vr.CompleteDate IS NOT NULL AND DateDiff(yyyy, vr.CompleteDate, GetDate()) = 0 AND vr.VendorId = v.VendorId), 'False') As IsRegistrationCompleted, Coalesce((SELECT Top 1 'True' FROM SalesReport sr WHERE sr.Submitted IS NOT NULL AND sr.PeriodYear = " & LastYear & " AND sr.PeriodQuarter = " & LastQuarter & " AND sr.VendorId = v.VendorId), 'False') As IsReportSubmitted "
        SQL = " FROM Vendor v "

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "v.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If


        If Not F_WebsiteURL.Text = String.Empty Then
            SQL = SQL & Conn & "WebsiteURL = " & DB.Quote(F_WebsiteURL.Text)
            Conn = " AND "
        End If
        If Not F_State.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "State = " & DB.Quote(F_State.SelectedValue)
            Conn = " AND "
        End If
        If Not F_CompanyName.Text = String.Empty Then
            SQL = SQL & Conn & "CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
            Conn = " AND "
        End If
        If Not F_City.Text = String.Empty Then
            SQL = SQL & Conn & "City LIKE " & DB.FilterQuote(F_City.Text)
            Conn = " AND "
        End If
        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "Email LIKE " & DB.FilterQuote(F_Email.Text)
            Conn = " AND "
        End If
        If Not F_LLC.SelectedValues = String.Empty Then
            SQL = SQL & Conn & " VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
            Conn = " AND "
        End If

        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "v.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If

        If Not F_IsExcluded.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "v.ExcludedVendor  = " & DB.Number(F_IsExcluded.SelectedValue)
            Conn = " AND "
        End If


        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandArgument Is Nothing OrElse Not IsNumeric(e.CommandArgument) Then Exit Sub


        Dim id As Integer = e.CommandArgument
        Dim vendor As VendorRow = VendorRow.GetRow(DB, id)
        Session("BuilderAccountId") = Nothing
        Session("BuilderId") = Nothing
        Session("VendorId") = id
        Session("VendorAccountId") = Nothing
        Session("PIQId") = Nothing
        Response.Redirect("/vendor/default.aspx")
    End Sub

    Private ReadOnly Property CurrentQuarter() As Integer
        Get
            Return Math.Ceiling(DatePart(DateInterval.Month, Now) / 3)
        End Get
    End Property

    Private ReadOnly Property CurrentYear() As Integer
        Get
            Return DatePart(DateInterval.Year, Now)
        End Get
    End Property

    Private ReadOnly Property LastQuarter() As Integer
        Get
            Return IIf(CurrentQuarter - 1 = 0, 4, CurrentQuarter - 1)
        End Get
    End Property

    Private ReadOnly Property LastYear() As Integer
        Get
            Return IIf(LastQuarter = 4, CurrentYear - 1, CurrentYear)
        End Get
    End Property


End Class
