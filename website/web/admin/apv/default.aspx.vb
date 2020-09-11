Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.IO

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_DisputeResponse.DataSource = DisputeResponseRow.GetList(DB, "DisputeResponse")
            F_DisputeResponse.DataValueField = "DisputeResponse"
            F_DisputeResponse.DataTextField = "DisputeResponse"
            F_DisputeResponse.DataBind()
            F_DisputeResponse.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_DisputeResponseReason.DataSource = DisputeResponseReasonRow.GetList(DB, "DisputeResponseReason")
            F_DisputeResponseReason.DataValueField = "DisputeResponseReason"
            F_DisputeResponseReason.DataTextField = "DisputeResponseReason"
            F_DisputeResponseReason.DataBind()
            F_DisputeResponseReason.Items.Insert(0, New ListItem("-- ALL --", ""))

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
            F_HasReported.SelectedValue = Request("F_HasReported")
            F_DisputeResponse.SelectedValue = Request("F_DisputeResponse")
            F_DisputeResponseReason.SelectedValue = Request("F_DisputeResponseReason")
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
        SQL = " FROM vAPVWithDisputes  "

        If Not F_HasReported.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "HasReported = " & DB.Quote(F_HasReported.SelectedValue)
            Conn = " AND "
        End If
        If Not F_DisputeResponse.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "DisputeResponse = " & DB.Quote(F_DisputeResponse.SelectedValue)
            Conn = " AND "
        End If
        If Not F_DisputeResponseReason.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "DisputeResponseReason = " & DB.Quote(F_DisputeResponseReason.SelectedValue)
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
        Integer.TryParse(F_StartPeriodYear.Text, StartYear)
        Integer.TryParse(F_StartPeriodQuarter.Text, StartQuarter)
        Integer.TryParse(F_EndPeriodYear.Text, EndYear)
        Integer.TryParse(F_EndPeriodQuarter.Text, EndQuarter)
        'if period spans years
        If StartYear > 0 AndAlso StartYear <> EndYear Then
            SQL = SQL & Conn & "("
            ' first do start and end condition
            SQL &= "(PeriodQuarter >= " & DB.Number(StartQuarter) & " AND PeriodYear = " & DB.Number(StartYear)
            SQL &= ") OR ("
            SQL &= "PeriodQuarter <= " & DB.Number(EndQuarter) & " AND PeriodYear = " & DB.Number(EndYear) & ")"
            ' do inbetween conditions if needed
            If StartYear + 1 <> EndYear Then
                For i As Integer = StartYear + 1 To EndYear - 1
                    SQL &= " OR (PeriodYear = " & DB.Number(i) & ")"
                Next
            End If
            SQL &= ")"
            Conn = " AND "
            ' if period within same year
        ElseIf StartYear > 0 AndAlso StartYear = EndYear Then
            SQL = SQL & Conn & "PeriodQuarter >= " & DB.Number(StartQuarter)
            Conn = " AND "
            SQL = SQL & Conn & "PeriodQuarter <= " & DB.Number(EndQuarter)
            Conn = " AND "
            SQL = SQL & Conn & "PeriodYear = " & DB.Number(StartYear)
            Conn = " AND "
        End If
        'If Not F_PeriodYear.Text = String.Empty Then
        '    SQL = SQL & Conn & "PeriodYear >= " & DB.Number(F_PeriodYear.Text)
        '    Conn = " AND "
        'End If
        'If Not F_PeriodQuarter.Text = String.Empty Then
        '    SQL = SQL & Conn & "PeriodQuarter >= " & DB.Number(F_PeriodQuarter.Text)
        '    Conn = " AND "
        'End If
        If Not F_VendorID.Text = String.Empty Then
            SQL = SQL & Conn & "VendorID >= " & DB.Number(F_VendorID.Text)
            Conn = " AND "
        End If
        'gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(1) " & SQL, 600)

        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder, 600)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BuildQuery()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        If F_StartPeriodYear.Text = "" Or F_StartPeriodQuarter.Text = "" Or F_EndPeriodYear.Text = "" Or F_EndPeriodQuarter.Text = "" Then
            AddError("Please enter both Start and End Period Year and Period Quarter")
            Exit Sub
        End If
        gvList.PageIndex = 0
        BindList()
    End Sub
    Private Sub btnExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExport.Click
        If Not IsValid Then Exit Sub
        If F_StartPeriodYear.Text = "" Or F_StartPeriodQuarter.Text = "" Or F_EndPeriodYear.Text = "" Or F_EndPeriodQuarter.Text = "" Then
            AddError("Please enter both Start and End Period Year and Period Quarter")
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
        sw.WriteLine("Builder Name , Historic Vendor ID , Vendor Name ,Has Reported , LLC , Period Year,Period Quarter,Vendor ID,Dispute Response,Dispute Response Reason,Initial Vendor Amount,Initial Builder Amount,Builder Amount In Dispute, Vendor Amount In Dispute,Resolution Amount, Final Amount")

        If res.Rows.Count > 0 Then

            For Each row As DataRow In res.Rows


                Dim BuilderName As String = String.Empty
                If Not IsDBNull(row("BuilderName")) Then
                    BuilderName = row("BuilderName")
                End If
                Dim HistoricVendorID As String = String.Empty
                If Not IsDBNull(row("HistoricVendorID")) Then
                    HistoricVendorID = row("HistoricVendorID")
                End If
                Dim VendorName As String = String.Empty
                If Not IsDBNull(row("VendorName")) Then
                    VendorName = row("VendorName")
                End If
                Dim HasReported As String = String.Empty
                If Not IsDBNull(row("HasReported")) Then
                    HasReported = row("HasReported")
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

                Dim VendorID As String = String.Empty
                If Not IsDBNull(row("VendorID")) Then
                    VendorID = row("VendorID")
                End If

                Dim DisputeResponse As String = String.Empty
                If Not IsDBNull(row("DisputeResponse")) Then
                    DisputeResponse = row("DisputeResponse")
                End If

                Dim DisputeResponseReason As String = String.Empty
                If Not IsDBNull(row("DisputeResponseReason")) Then
                    DisputeResponseReason = row("DisputeResponseReason")
                End If
                Dim InitialVendorAmount As String = String.Empty
                If Not IsDBNull(row("InitialVendorAmount")) Then
                    InitialVendorAmount = FormatCurrency(row("InitialVendorAmount"))
                End If

                Dim InitialBuilderAmount As String = String.Empty
                If Not IsDBNull(row("InitialBuilderAmount")) Then
                    InitialBuilderAmount = FormatCurrency(row("InitialBuilderAmount"))
                End If
                Dim BuilderAmountInDispute As String = String.Empty
                If Not IsDBNull(row("BuilderAmountInDispute")) Then
                    BuilderAmountInDispute = FormatCurrency(row("BuilderAmountInDispute"))
                End If


                Dim VendorAmountInDispute As String = String.Empty
                If Not IsDBNull(row("VendorAmountInDispute")) Then
                    VendorAmountInDispute = FormatCurrency(row("VendorAmountInDispute"))
                End If


                Dim ResolutionAmount As String = String.Empty
                If Not IsDBNull(row("ResolutionAmount")) Then
                    ResolutionAmount = FormatCurrency(row("ResolutionAmount"))
                End If


                Dim FinalAmount As String = String.Empty
                If Not IsDBNull(row("FinalAmount")) Then
                    FinalAmount = FormatCurrency(row("FinalAmount"))
                End If


                sw.WriteLine(Core.QuoteCSV(BuilderName) & "," & Core.QuoteCSV(HistoricVendorID) & "," & Core.QuoteCSV(VendorName) & "," & Core.QuoteCSV(HasReported) & "," & Core.QuoteCSV(LLC) & "," & Core.QuoteCSV(PeriodYear) & "," & Core.QuoteCSV(PeriodQuarter) & "," & Core.QuoteCSV(VendorID) & "," & Core.QuoteCSV(DisputeResponse) & "," & Core.QuoteCSV(DisputeResponseReason) & "," & Core.QuoteCSV(InitialVendorAmount) & "," & Core.QuoteCSV(InitialBuilderAmount) & "," & Core.QuoteCSV(BuilderAmountInDispute) & "," & Core.QuoteCSV(VendorAmountInDispute) & "," & Core.QuoteCSV(ResolutionAmount) & "," & Core.QuoteCSV(FinalAmount))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub
End Class

