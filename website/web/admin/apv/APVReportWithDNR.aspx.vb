Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.IO

Partial Class Index
    Inherits AdminPage
    
    Private m_GetAllVendorUserRoles As DataTable
    Private ReadOnly Property dtGetAllVendorUserRoles() As DataTable
        Get
            If m_GetAllVendorUserRoles Is Nothing Then
                m_GetAllVendorUserRoles = GetAllVendorUserRoles(DB)
            End If
            Return m_GetAllVendorUserRoles
        End Get
    End Property

    Public Shared Function GetAllVendorUserRoles(ByVal DB As Database) As DataTable
        Dim sql As String = _
              " select  a.FirstName + ' ' +  a.LastName as Name , a.Email , a.VendorID, a.Phone ,  r.VendorAccountID, v.VendorRole, v.VendorRoleID " _
            & " from VendorAccount a inner join VendorAccountVendorRole r on a.VendorAccountID=r.VendorAccountID " _
            & "     inner join VendorRole v on r.VendorRoleID=v.VendorRoleID where v.VendorRoleID = 2 " _
            & " order by VendorAccountID"

        Return DB.GetDataTable(sql)
    End Function

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        gvList.BindList = AddressOf BindList
        Dim i As Integer = 1
        If Not IsPostBack Then

            For i = 1 To 4
                Me.F_StartPeriodQuarter.Items.Insert(i - 1, (New ListItem(i, i)))

            Next

            For i = 1 To 15
                Me.F_StartPeriodYear.Items.Insert(i - 1, (New ListItem(Now.Year - 15 + i, Now.Year - 15 + i)))

            Next
            Me.F_StartPeriodQuarter.SelectedValue = ((Now.AddMonths(-3).Month - 1) \ 3) + 1
            Me.F_StartPeriodYear.SelectedValue = Now.AddMonths(-3).Year

            F_BuilderName.DataSource = BuilderRow.GetList(DB, "CompanyName", "ASC")
            F_BuilderName.DataValueField = "CompanyName"
            F_BuilderName.DataTextField = "CompanyName"
            F_BuilderName.DataBind()
            F_BuilderName.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorName.DataSource = VendorRow.GetList(DB, "CompanyName", "ASC")
            F_VendorName.DataValueField = "VendorId"
            F_VendorName.DataTextField = "CompanyName"
            F_VendorName.DataBind()
            F_VendorName.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_LLC.DataSource = LLCRow.GetList(DB, "LLC", "ASC")
            F_LLC.DataValueField = "LLC"
            F_LLC.DataTextField = "LLC"
            F_LLC.DataBind()
            F_LLC.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BuilderName.SelectedValue = Request("F_BuilderName")
            F_VendorName.SelectedValue = Request("F_VendorName")
            F_LLC.Text = Request("F_LLC")
            F_HasReportedVendor.SelectedValue = Request("F_HasReportedVendor")
           
            F_HistoricVendorID.Text = Request("F_HistoricVendorID")
            'F_PeriodYear.Text = Request("F_PeriodYear")
            'F_PeriodQuarter.Text = Request("F_PeriodQuarter")
            F_VendorID.Text = Request("F_VendorID")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "PeriodYear, PeriodQuarter"
                gvList.SortOrder = "ASC"
            End If
        End If
    End Sub

    Private Function BuildQuery() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM vAPVforReport_2  "

        If Not F_HasReportedVendor.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "HasReportedVendor = " & DB.Quote(F_HasReportedVendor.SelectedValue)
            Conn = " AND "
        End If
        
        If Not F_BuilderName.Text = String.Empty Then
            SQL = SQL & Conn & "BuilderName = " & DB.Quote(F_BuilderName.Text)
            Conn = " AND "
        End If
        If Not F_VendorName.Text = String.Empty Then
            SQL = SQL & Conn & "VendorId = " & DB.Number(F_VendorName.Text)
            Conn = " AND "
        End If
        ' if ALL not selected, determine values to include
        If Not F_LLC.Items(0).Selected Then
            Dim LLSString As String = String.Empty
            For Each li As ListItem In F_LLC.Items
                If li.Selected Then
                    LLSString &= DB.Quote(li.Value) & ","
                End If
            Next
            If Not String.IsNullOrEmpty(LLSString) Then
                ' trim last comma
                LLSString = LLSString.Substring(0, LLSString.Length - 1)
                SQL = SQL & Conn & "LLC in (" & LLSString & ")"
                Conn = " AND "
            End If
        End If
        If Not F_HistoricVendorID.Text = String.Empty Then
            SQL = SQL & Conn & "HistoricVendorID = " & DB.Number(F_HistoricVendorID.Text)
            Conn = " AND "
        End If

        Dim StartYear As Integer = 0
        Dim EndYear As Integer = 0
        Dim StartQuarter As Integer = 0
        Dim EndQuarter As Integer = 0
        Integer.TryParse(F_StartPeriodYear.SelectedValue, StartYear)
        Integer.TryParse(F_StartPeriodQuarter.SelectedValue, StartQuarter)
        
       
        If StartYear > 0 Then
            SQL = SQL & Conn & "PeriodYear = " & DB.Number(StartYear)
            Conn = " AND "
        End If
        If StartQuarter > 0 Then
            SQL = SQL & Conn & "PeriodQuarter = " & DB.Number(StartQuarter)
            Conn = " AND "
        End If
        If Not F_VendorID.Text = String.Empty Then
            SQL = SQL & Conn & "VendorID = " & DB.Number(F_VendorID.Text)
            Conn = " AND "
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("Select Count(*)  " & SQL)
        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

        Return res

    End Function

    Private Sub BindList()
        Dim res As DataTable = BuildQuery()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub



    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        If F_StartPeriodYear.Text = "" Or F_StartPeriodQuarter.Text = "" Then
            AddError("Please enter both Start  Period Year and Period Quarter")
            Exit Sub
        End If
        gvList.PageIndex = 0
        BindList()
    End Sub
    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub
        If F_StartPeriodYear.Text = "" Or F_StartPeriodQuarter.Text = "" Then
            AddError("Please enter both Start  Period Year and Period Quarter")
            Exit Sub
        End If
        ExportReport()

    End Sub
    Public Sub ExportReport()
        gvList.PageSize = 5000
        Dim res As DataTable = BuildQuery()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Historic Vendor ID , Vendor Name,Vendor Quarterly Reporter Contact Name,Email address,Phone Number,BuilderName , LLC , Period Year,Period Quarter  ,Initial Vendor Amount,Initial Builder Amount,Builder Amount In Dispute, Vendor Response Amount, Final Amount, Submitted Date")

        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows


                
                Dim HistoricVendorID As String = String.Empty
                If Not IsDBNull(row("HistoricVendorID")) Then
                    HistoricVendorID = row("HistoricVendorID")
                End If
                Dim VendorName As String = String.Empty
                If Not IsDBNull(row("VendorName")) Then
                    VendorName = row("VendorName")
                End If

                Dim vendorQTRReporterName As String = String.Empty
                Dim vendorQTRReporterEmail As String = String.Empty
                Dim vendorQTRReporterPhone As String = String.Empty
                Dim rows As DataRow() = dtGetAllVendorUserRoles.Select("VendorID =" & row("VendorId"))
                If rows.Length > 0 Then
                    If Not IsDBNull(rows(0)("Name")) Then
                        vendorQTRReporterName = rows(0)("Name")
                    End If
                    If Not IsDBNull(rows(0)("Email")) Then
                        vendorQTRReporterEmail = rows(0)("Email")
                    End If
                    If Not IsDBNull(rows(0)("Phone")) Then
                        vendorQTRReporterPhone = rows(0)("Phone")
                    End If
                End If


                Dim BuilderName As String = String.Empty
                If Not IsDBNull(row("BuilderName")) Then
                    BuilderName = row("BuilderName")
                End If

                Dim LLC As String = String.Empty
                If Not IsDBNull(row("LLC")) Then
                    LLC = row("LLC")
                End If

                Dim PeriodYear As String = String.Empty
                If Not IsDBNull(row("PeriodYear")) Then
                    PeriodYear = row("PeriodYear")
                End If
                Dim PeriodQuarter As String = String.Empty
                If Not IsDBNull(row("PeriodQuarter")) Then
                    PeriodQuarter = row("PeriodQuarter")
                End If


                Dim InitialVendorAmount As String = String.Empty

                If row("HasReportedVendor") = "Yes" Then
                    If Not IsDBNull(row("InitialVendorAmount")) Then
                        InitialVendorAmount = FormatCurrency(row("InitialVendorAmount"))
                    Else
                        'InitialVendorAmount = FormatCurrency(0, 2)
                        InitialVendorAmount = "DNR"
                    End If
                Else
                    InitialVendorAmount = "DNR"
                End If
                Dim InitialBuilderAmount As String = String.Empty
                If row("HasReportedBuilder") = "Yes" Then
                    If Not IsDBNull(row("InitialBuilderAmount")) Then
                        InitialBuilderAmount = FormatCurrency(row("InitialBuilderAmount"))
                    Else
                        'InitialBuilderAmount = FormatCurrency(0, 2)
                        InitialBuilderAmount = "DNR"
                    End If
                Else
                    InitialBuilderAmount = "DNR"
                End If





                Dim BuilderAmountInDispute As String = String.Empty
                If Not IsDBNull(row("BuilderAmountInDispute")) Then
                    BuilderAmountInDispute = FormatCurrency(row("BuilderAmountInDispute"))
                End If



                Dim ResolutionAmount As String = String.Empty
                If Not IsDBNull(row("ResolutionAmount")) Then
                    ResolutionAmount = FormatCurrency(row("ResolutionAmount"))
                End If


                Dim FinalAmount As String = String.Empty
                If Not IsDBNull(row("FinalAmount")) Then
                    FinalAmount = FormatCurrency(row("FinalAmount"))
                End If

                Dim SubmittedDate As String = String.Empty
                If Not IsDBNull(row("SubmittedDate")) Then
                    SubmittedDate = row("SubmittedDate")
                End If

                sw.WriteLine(Core.QuoteCSV(HistoricVendorID) & "," & Core.QuoteCSV(VendorName) & "," & Core.QuoteCSV(vendorQTRReporterName) & "," & Core.QuoteCSV(vendorQTRReporterEmail) & "," & Core.QuoteCSV(vendorQTRReporterPhone) & "," & Core.QuoteCSV(BuilderName) & "," & Core.QuoteCSV(LLC) & "," & Core.QuoteCSV(PeriodYear) & "," & Core.QuoteCSV(PeriodQuarter) & "," & Core.QuoteCSV(InitialVendorAmount) & "," & Core.QuoteCSV(InitialBuilderAmount) & "," & Core.QuoteCSV(BuilderAmountInDispute) & "," & Core.QuoteCSV(ResolutionAmount) & "," & Core.QuoteCSV(FinalAmount) & "," & Core.QuoteCSV(SubmittedDate))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If
        Dim ltlInitialBuilderAmount As Literal = e.Row.FindControl("ltlInitialBuilderAmount")
        Dim ltlInitialVendorAmount As Literal = e.Row.FindControl("ltlInitialVendorAmount")

        Dim ltlVendorQuarterlyReporter As Literal = e.Row.FindControl("ltlVendorQuarterlyReporter")
        
        Dim rows As DataRow() = dtGetAllVendorUserRoles.Select("VendorID =" & e.Row.DataItem("VendorId"))
        If rows.Length > 0 Then
            ltlVendorQuarterlyReporter.Text = "Qtr Reporter : " & rows(0)("Name")
            ltlVendorQuarterlyReporter.Text &= "</br> Email : " & rows(0)("Email")
            ltlVendorQuarterlyReporter.Text &= "</br> Phone: " & rows(0)("Phone")
        End If
       
        If e.Row.DataItem("HasReportedBuilder") = "Yes" Then
            If Not IsDBNull(e.Row.DataItem("InitialBuilderAmount")) Then
                ltlInitialBuilderAmount.Text = FormatCurrency(e.Row.DataItem("InitialBuilderAmount"))
            Else
                'ltlInitialBuilderAmount.Text = FormatCurrency(0.0)
                ltlInitialBuilderAmount.Text = "DNR"
            End If
        Else
            ltlInitialBuilderAmount.Text = "DNR"
        End If

        If e.Row.DataItem("HasReportedVendor") = "Yes" Then
            If Not IsDBNull(e.Row.DataItem("InitialVendorAmount")) Then
                ltlInitialVendorAmount.Text = FormatCurrency(e.Row.DataItem("InitialVendorAmount"))
            Else
                'ltlInitialVendorAmount.Text = FormatCurrency(0.0)
                ltlInitialVendorAmount.Text = "DNR"
            End If

        Else
            ltlInitialVendorAmount.Text = "DNR"
        End If

    End Sub
End Class

