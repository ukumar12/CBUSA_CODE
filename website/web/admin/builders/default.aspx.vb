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
            F_RegistrationStatusID.DataSource = RegistrationStatusRow.GetList(DB)
            F_RegistrationStatusID.DataValueField = "RegistrationStatusID"
            F_RegistrationStatusID.DataTextField = "RegistrationStatus"
            F_RegistrationStatusID.DataBind()
            F_RegistrationStatusID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_LLCId.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLCId.DataTextField = "LLC"
            F_LLCId.DataValueField = "LLCID"
            F_LLCId.DataBind()
            F_LLCId.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_HistoricId.Text = Request("F_HistoricId")
            F_CompanyName.Text = Request("F_CompanyName")
            F_WebsiteURL.Text = Request("F_WebsiteURL")
            F_RegistrationStatusID.SelectedValue = Request("F_RegistrationStatusID")
            F_SubmittedLbound.Text = Request("F_SubmittedLBound")
            F_SubmittedUbound.Text = Request("F_SubmittedUBound")
            F_LLCId.SelectedValue = Request("F_LLCId")
            F_drpSkipEntitlement.SelectedValue = Request("F_drpSkipEntitlement")
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "CompanyName"

            BindList()

            If LoggedInIsInternal Then
                btnSubmitA.Visible = True
            End If

        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " Where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " b.*, (Select RegistrationStatus From RegistrationStatus Where RegistrationStatusId = b.RegistrationStatusid) As RegistrationStatus, llc.LLC, Coalesce((SELECT Top 1 'True' FROM BuilderRegistration br WHERE br.CompleteDate IS NOT NULL AND DateDiff(yyyy, br.CompleteDate, GetDate()) >= 0 AND br.BuilderId = b.BuilderId), 'False') As IsRegistrationCompleted, Coalesce((SELECT Top 1 'True' FROM PurchasesReport pr WHERE pr.Submitted IS NOT NULL AND pr.PeriodYear = " & LastYear & " AND pr.PeriodQuarter = " & LastQuarter & " AND pr.BuilderId = b.BuilderId), 'False') As IsReportSubmitted "
        SQL = " FROM Builder b Left Outer Join LLC llc On b.LLCId = llc.LLCId "

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "b.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If

        If Not F_CrmId.Text = String.Empty Then
            SQL = SQL & Conn & "b.CRMID = " & DB.Quote(F_CrmId.Text)
            Conn = " AND "
        End If

        If Not F_LLCId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.LLCID = " & DB.Number(F_LLCId.SelectedValue)
            Conn = " AND "
        End If

        If Not F_WebsiteURL.Text = String.Empty Then
            SQL = SQL & Conn & "b.WebsiteURL = " & DB.Quote(F_WebsiteURL.Text)
            Conn = " AND "
        End If
        If Not F_RegistrationStatusID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.RegistrationStatusID = " & DB.Quote(F_RegistrationStatusID.SelectedValue)
            Conn = " AND "
        End If
        'If Not F_LastName.Text = String.Empty Then
        '    SQL = SQL & Conn & "ba.LastName LIKE " & DB.FilterQuote(F_LastName.Text)
        '    Conn = " AND "
        'End If
        If Not F_CompanyName.Text = String.Empty Then
            SQL = SQL & Conn & "b.CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
            Conn = " AND "
        End If
        'If Not F_Email.Text = String.Empty Then
        '    SQL = SQL & Conn & "ba.Email LIKE " & DB.FilterQuote(F_Email.Text)
        '    Conn = " AND "
        'End If
        If Not F_SubmittedLbound.Text = String.Empty Then
            SQL = SQL & Conn & "b.Submitted >= " & DB.Quote(F_SubmittedLbound.Text)
            Conn = " AND "
        End If
        If Not F_SubmittedUbound.Text = String.Empty Then
            SQL = SQL & Conn & "b.Submitted < " & DB.Quote(DateAdd("d", 1, F_SubmittedUbound.Text))
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsNew.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.IsNew  = " & DB.Number(F_IsNew.SelectedValue)
            Conn = " AND "
        End If

        If Not F_drpSkipEntitlement.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.SkipEntitlementCheck  = " & DB.Number(F_drpSkipEntitlement.SelectedValue)
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
    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        ExportList()
    End Sub
    Public Sub ExportList()
        gvList.PageSize = 5000
        Dim res As DataTable = GetBuilderData()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("HistoricID, Company Name, LLC, Address, City, State, Zip, Email, Phone,  Active ")
        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CompanyName As String = row("CompanyName")
                Dim HistoricID As String = String.Empty
                Dim Address As String = String.Empty
                Dim City As String = String.Empty
                Dim Email As String = String.Empty
                Dim Phone As String = String.Empty
                Dim State As String = String.Empty
                Dim Zip As String = String.Empty
                If Not IsDBNull(row("HistoricID")) Then
                    HistoricID = row("HistoricID")
                Else
                    HistoricID = "0"
                End If

                Dim Active As String = String.Empty
                If row("IsActive") Then
                    Active = "YES"
                Else
                    Active = "NO"
                End If

                Dim LLC As String = String.Empty
                If Not IsDBNull(row("LLC")) Then
                    LLC = row("LLC")
                Else
                    LLC = "-"
                End If

                If Not IsDBNull(row("Address")) Then
                    Address = row("Address")
                End If
                If Not IsDBNull(row("City")) Then
                    City = row("City")
                End If
                If Not IsDBNull(row("Email")) Then
                    Email = row("Email")
                End If
                If Not IsDBNull(row("Phone")) Then
                    Phone = row("Phone")
                End If
                If Not IsDBNull(row("State")) Then
                    State = row("State")
                End If
                If Not IsDBNull(row("Zip")) Then
                    Zip = row("Zip")
                End If
                sw.WriteLine(Core.QuoteCSV(HistoricID) & "," & Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(LLC) & "," & Core.QuoteCSV(Address) & "," & Core.QuoteCSV(City) & "," & Core.QuoteCSV(State) & "," & Core.QuoteCSV(Zip) & "," & Core.QuoteCSV(Email) & "," & Core.QuoteCSV(Phone) & "," & Core.QuoteCSV(Active))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub

    Private Function GetBuilderData() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " Where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT distinct b.historicID, b.CompanyName, b.IsActive, b.Address, b.SkipEntitlementCheck, b.City, b.state, b.Zip, b.Email, b.Phone, b.CRMID, b.LLCID, b.WebsiteURL, b.RegistrationStatusID, b.Submitted, "
        SQLFields &= " b.IsNew , (Select RegistrationStatus From RegistrationStatus Where RegistrationStatusId = b.RegistrationStatusid) As RegistrationStatus, "
        SQLFields &= " llc.LLC, Coalesce((SELECT Top 1 'True' FROM BuilderRegistration br WHERE br.CompleteDate IS NOT NULL AND DateDiff(yyyy, br.CompleteDate, GetDate()) = 0 "
        SQLFields &= " AND br.BuilderId = b.BuilderId), 'False') As IsRegistrationCompleted, Coalesce((SELECT Top 1 'True' FROM PurchasesReport pr WHERE pr.Submitted IS NOT NULL AND pr.PeriodYear = " & LastYear & " AND pr.PeriodQuarter = " & LastQuarter & " AND pr.BuilderId = b.BuilderId), 'False') As IsReportSubmitted "
        SQL = " FROM Builder b Left Outer Join LLC llc On b.LLCId = llc.LLCId "

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "b.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If

        If Not F_CrmId.Text = String.Empty Then
            SQL = SQL & Conn & "b.CRMID = " & DB.Quote(F_CrmId.Text)
            Conn = " AND "
        End If

        If Not F_LLCId.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.LLCID = " & DB.Number(F_LLCId.SelectedValue)
            Conn = " AND "
        End If

        If Not F_WebsiteURL.Text = String.Empty Then
            SQL = SQL & Conn & "b.WebsiteURL = " & DB.Quote(F_WebsiteURL.Text)
            Conn = " AND "
        End If
        If Not F_RegistrationStatusID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.RegistrationStatusID = " & DB.Quote(F_RegistrationStatusID.SelectedValue)
            Conn = " AND "
        End If

        If Not F_CompanyName.Text = String.Empty Then
            SQL = SQL & Conn & "b.CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
            Conn = " AND "
        End If

        If Not F_SubmittedLbound.Text = String.Empty Then
            SQL = SQL & Conn & "b.Submitted >= " & DB.Quote(F_SubmittedLbound.Text)
            Conn = " AND "
        End If
        If Not F_SubmittedUbound.Text = String.Empty Then
            SQL = SQL & Conn & "b.Submitted < " & DB.Quote(DateAdd("d", 1, F_SubmittedUbound.Text))
            Conn = " AND "
        End If
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
        If Not F_IsNew.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.IsNew  = " & DB.Number(F_IsNew.SelectedValue)
            Conn = " AND "
        End If

        If Not F_drpSkipEntitlement.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.SkipEntitlementCheck  = " & DB.Number(F_drpSkipEntitlement.SelectedValue)
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

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

    Protected Sub btnSubmitA_Click(sender As Object, e As System.EventArgs) Handles btnSubmitA.Click
        Try
            Dim dtBuilders As DataTable = DB.GetDataTable("Select * from Builder Where   builderid = " & F_HistoricId.Text & " and IsActive =1 ")
            Dim p As New VindiciaPaymentProcessor(DB)


            p.IsTestMode = DataLayer.SysParam.GetValue(DB, "TestMode")
            For Each builder As DataRow In dtBuilders.Rows
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, builder("BuilderID"))
                Dim LLCAffliateID As String = LLCRow.GetRow(DB, dbBuilder.LLCID).AffiliateID.ToString("D3")
                Try
                    p.IsTestMode = SysParam.GetValue(DB, "TestMode")
                    If p.EnsureVindiciaAccount(dbBuilder) Then
                        Dim autoBills() As Vindicia.AutoBill = p.GetAutobills(dbBuilder)
                        For Each ab As Vindicia.AutoBill In autoBills
                            p.UpdateMerchantAffID(ab.VID, dbBuilder, ab, LLCAffliateID)
                        Next
                    End If
                Catch ex As Exception
                    Console.WriteLine("Error  CancelAutoBill  . " & dbBuilder.BuilderID)
                End Try
            Next
        Catch ex As Exception
            Console.WriteLine("Error  CancelAutoBill ")
        End Try
    End Sub

End Class
