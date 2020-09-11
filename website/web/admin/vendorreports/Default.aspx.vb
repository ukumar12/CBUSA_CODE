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



    Private Function BindReportedVendors() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " AND "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        'SQLFields = "Select v.companyname "
        'SQL = " from SalesReport s inner join Vendor v ON s.vendorID = v.vendorID where periodyear = " & LastYear & " and periodQuarter = " & LastQuarter

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " v.VendorID, v.companyname as Vendor,v.phone as VendorPhone," _
           & " Coalesce((SELECT TOP 1 firstname + ' ' + lastname AS PrimaryContact from vendoraccount va  " _
           & " inner join vendoraccountvendorrole r on r.vendoraccountid=va.vendoraccountid inner join vendorrole vr on " _
           & " r.vendorroleid=vr.vendorroleid where vr.vendorroleid=1 and v.vendorid = va.vendorid),'') AS PrimaryContact, " _
           & " Coalesce((SELECT TOP 1 email AS PrimaryContactEmail from vendoraccount va  " _
           & " inner join vendoraccountvendorrole r on r.vendoraccountid=va.vendoraccountid inner join vendorrole vr on " _
           & " r.vendorroleid=vr.vendorroleid where vr.vendorroleid=1 and v.vendorid = va.vendorid),'') AS PrimaryContactEmail,  " _
           & " Coalesce((SELECT TOP 1 firstname + ' ' + lastname AS QuarterlyReportContact from vendoraccount va  " _
           & " inner join vendoraccountvendorrole r on r.vendoraccountid=va.vendoraccountid inner join vendorrole vr on " _
           & " r.vendorroleid=vr.vendorroleid where vr.vendorroleid=2 and v.vendorid = va.vendorid),'') AS QuarterlyReportContact, " _
           & " Coalesce((SELECT TOP 1 email AS QuarterlyReportEmail from vendoraccount va  " _
           & " inner join vendoraccountvendorrole r on r.vendoraccountid=va.vendoraccountid inner join vendorrole vr on " _
           & " r.vendorroleid=vr.vendorroleid where vr.vendorroleid=2 and v.vendorid = va.vendorid),'') AS QuarterlyReportEmail "

        SQL = " FROM vendor v where v.isactive = 1 AND (SELECT sr.Submitted From SalesReport sr WHERE " _
            & " sr.submitted is not null AND sr.PeriodYear = " & LastYear & " AND sr.PeriodQuarter = " & LastQuarter _
            & " AND sr.VendorId = v.VendorId "

        If F_Reported.SelectedValue = 1 Then
            SQL &= ") IS NULL "
        Else
            SQL &= ") IS NOT NULL "
        End If

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL &= " AND v.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
        End If

        If Not F_LLC.SelectedValues = String.Empty Then
            SQL &= " AND v.VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
        End If

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BindReportedVendors()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
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
        Dim res As DataTable = BindReportedVendors()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Vendor Name , Phone , Primary Contact , Primary Contact Email , Quarterly Reporter, Quarterly Reporter Email,VendorLLC")

        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CompanyName As String = row("Vendor")

                Dim VendorPhone As String = String.Empty
                If Not IsDBNull(row("VendorPhone")) Then
                    VendorPhone = row("VendorPhone")
                Else
                    VendorPhone = " "
                End If
                Dim PrimaryContact As String = String.Empty
                If Not IsDBNull(row("PrimaryContact")) Then
                    PrimaryContact = row("PrimaryContact")
                Else
                    PrimaryContact = " "
                End If
                Dim PrimaryContactEmail As String = String.Empty
                If Not IsDBNull(row("PrimaryContactEmail")) Then
                    PrimaryContactEmail = row("PrimaryContactEmail")
                Else
                    PrimaryContactEmail = " "
                End If
                Dim QuarterlyReportContact As String = String.Empty
                If Not IsDBNull(row("QuarterlyReportContact")) Then
                    QuarterlyReportContact = row("QuarterlyReportContact")
                Else
                    QuarterlyReportContact = " "
                End If
               
                Dim QuarterlyReportEmail As String = String.Empty
                If Not IsDBNull(row("QuarterlyReportEmail")) Then
                    QuarterlyReportEmail = row("QuarterlyReportEmail")
                Else
                    QuarterlyReportEmail = " "
                End If
                Dim VendorLLC As String = String.Empty
                If Not IsDBNull(ListLLC(row("VendorID"))) Then
                    VendorLLC = ListLLC(row("VendorID"))
                Else
                    VendorLLC = " "
                End If

                sw.WriteLine(Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(VendorPhone) & "," & Core.QuoteCSV(PrimaryContact) & "," & Core.QuoteCSV(PrimaryContactEmail) & "," & Core.QuoteCSV(QuarterlyReportContact) & "," & Core.QuoteCSV(QuarterlyReportEmail) & "," & Core.QuoteCSV(VendorLLC))
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
            strLLCPricing &= row("LLC") & "|"
        Next

        If strLLCPricing <> String.Empty AndAlso strLLCPricing.EndsWith("|") Then strLLCPricing = strLLCPricing.Substring(0, strLLCPricing.Length - 1)

        Return strLLCPricing

    End Function





End Class

