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

            F_HistoricId.Text = Request("F_HistoricId")
            F_VendorID.SelectedValue = Request("F_VendorID")
            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()
            If Request("F_TotalRequestsLBound") <> String.Empty Then
                F_TotalRequestsLBound.Text = Request("F_TotalRequestsLBound")
            Else
                F_TotalRequestsLBound.Text = "1"
            End If
            F_TotalRequestsUBound.Text = Request("F_TotalRequestsUBound")
            F_ActiveBidsLBound.Text = Request("F_ActiveBidsLBound")
            F_ActiveBidsUBound.Text = Request("F_ActiveBidsUBound")
            F_TotalPendingBidsLBound.Text = Request("F_TotalPendingBidsLBound")
            F_TotalPendingBidsUBound.Text = Request("F_TotalPendingBidsUBound")
            F_TotalBidsLBound.Text = Request("F_TotalBidsLBound")
            F_TotalBidsUBound.Text = Request("F_TotalBidsUBound")
            F_TotalBidAmountLBound.Text = Request("F_TotalBidAmountLBound")
            F_TotalBidAmountUBound.Text = Request("F_TotalBidAmountUBound")
            F_TotalAwardedBidsLBound.Text = Request("F_TotalAwardedBidsLBound")
            F_TotalAwardedBidsUBound.Text = Request("F_TotalAwardedBidsUBound")
            F_TotalAwardedBidsAmountLBound.Text = Request("F_TotalAwardedBidsAmountLBound")
            F_TotalAwardedBidsAmountUBound.Text = Request("F_TotalAwardedBidsAmountUBound")
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "v.CompanyName"
            BindList()
        End If
    End Sub

    Private Function BindVendorBidData() As DataTable
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "    select V.CompanyName, V.VendorID, p.TotalRequests ,p.ActiveBids ,p.PendingBids   ,p.TotalBids    ,p.TotalBidsAmount  ,p.TotalAwardedBids  ,p.TotalAwardedBidsAmount"
        SQL = "  from Vendor v"
        SQL &= " left outer join ( select POQuoteRequest .VendorId , COUNT( (POQuoteRequest . quoteid) ) as TotalRequests "
        SQL &= " , COUNT (CASE WHEN  POQuoteRequest.RequestStatus In ('New') THEN POQuoteRequest.QuoteId ELSE NULL END) AS ActiveBids"
        SQL &= " , COUNT( CASE When POQuoteRequest.RequestStatus In ('New') and  ( POQuoteRequest.QuoteTotal  =0 or  POQuoteRequest.QuoteTotal  = null)  THEN POQuoteRequest.QuoteId ELSE NULL END)  as PendingBids"
        SQL &= " , COUNT(case when POQuoteRequest.RequestStatus In ('New') and  ( POQuoteRequest.QuoteTotal > 0)    THEN POQuoteRequest.QuoteId ELSE NULL END) AS TotalBids"
        SQL &= " , sum (case WHEN POQuoteRequest .RequestStatus in ('new') and ( POQuoteRequest.QuoteTotal  > 0) then quotetotal else 0 end)  as TotalBidsAmount "
        SQL &= " , COUNT (case WHEN POQuoteRequest .RequestStatus in  ('Awarded') THEN POQuoteRequest.QuoteId ELSE NULL END  ) as TotalAwardedBids"
        SQL &= " ,SUM (CASE when   POQuoteRequest .RequestStatus in ('Awarded') then quotetotal else 0 end ) as TotalAwardedBidsAmount"
        SQL &= "  from POQuoteRequest group by POQuoteRequest .VendorId )p on v .VendorId = p .VendorID"

        If Not F_HistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "v.HistoricId = " & DB.Quote(F_HistoricId.Text)
            Conn = " AND "
        End If
        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "v.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_TotalRequestsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalRequests >= " & DB.Number(F_TotalRequestsLBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalRequestsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalRequests <= " & DB.Number(F_TotalRequestsUBound.Text)
            Conn = " AND "
        End If
        If Not F_ActiveBidsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "ActiveBids >= " & DB.Number(F_ActiveBidsLBound.Text)
            Conn = " AND "
        End If
        If Not F_ActiveBidsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "ActiveBids <= " & DB.Number(F_ActiveBidsUBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalPendingBidsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "PendingBids >= " & DB.Number(F_TotalPendingBidsLBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalPendingBidsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "PendingBids <= " & DB.Number(F_TotalPendingBidsUBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalBidsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalBids >= " & DB.Number(F_TotalBidsLBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalBidsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalBids <= " & DB.Number(F_TotalBidsUBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalBidAmountLBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalBidsAmount >= " & DB.Number(F_TotalBidAmountLBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalBidAmountUBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalBidsAmount <= " & DB.Number(F_TotalBidAmountUBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalAwardedBidsLBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalAwardedBids >= " & DB.Number(F_TotalAwardedBidsLBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalAwardedBidsUBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalAwardedBids <= " & DB.Number(F_TotalAwardedBidsUBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalAwardedBidsAmountLBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalAwardedBidsAmount >= " & DB.Number(F_TotalAwardedBidsAmountLBound.Text)
            Conn = " AND "
        End If
        If Not F_TotalAwardedBidsAmountUBound.Text = String.Empty Then
            SQL = SQL & Conn & "TotalAwardedBidsAmount <= " & DB.Number(F_TotalAwardedBidsAmountUBound.Text)
            Conn = " AND "
        End If
        If Not F_LLC.SelectedValues = String.Empty Then
            SQL = SQL & Conn & " v.VendorID in (select VendorID from LLCVendor where LLCID in " & DB.NumberMultiple(F_LLC.SelectedValues) & ")"
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)
        Return DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)

    End Function

    Private Sub BindList()
        Dim res As DataTable = BindVendorBidData()
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
        Dim res As DataTable = BindVendorBidData()
        Dim Folder As String = "/assets/catalogs/"
        Dim FileName As String = Core.GenerateFileID & ".csv"
        Dim sw As StreamWriter = New StreamWriter(Server.MapPath(Folder & FileName), False)
        sw.WriteLine("Vendor Name , Total Requests , Active Bids , Total Pending Bids , TotalBids, Total Bid Amount($),Total Awarded Bids,Total Awarded Bids Amount($) ")

        If res.Rows.Count > 0 Then
            For Each row As DataRow In res.Rows
                Dim CompanyName As String = row("CompanyName")

                Dim TotalRequests As String = String.Empty
                If Not IsDBNull(row("TotalRequests")) Then
                    TotalRequests = row("TotalRequests")
                Else
                    TotalRequests = "0"
                End If
                Dim ActiveBids As String = String.Empty
                If Not IsDBNull(row("ActiveBids")) Then
                    ActiveBids = row("ActiveBids")
                Else
                    ActiveBids = "0"
                End If
                Dim PendingBids As String = String.Empty
                If Not IsDBNull(row("PendingBids")) Then
                    PendingBids = row("PendingBids")
                Else
                    PendingBids = "0"
                End If
                Dim TotalBids As String = String.Empty
                If Not IsDBNull(row("TotalBids")) Then
                    TotalBids = row("TotalBids")
                Else
                    TotalBids = "0"
                End If
                Dim TotalBidsAmount As String = String.Empty
                If Not IsDBNull(row("TotalBidsAmount")) Then
                    TotalBidsAmount = row("TotalBidsAmount")
                Else
                    TotalBidsAmount = "0"
                End If
                Dim TotalAwardedBids As String = String.Empty
                If Not IsDBNull(row("TotalAwardedBids")) Then
                    TotalAwardedBids = row("TotalAwardedBids")
                Else
                    TotalAwardedBids = "0"
                End If
                Dim TotalAwardedBidsAmount As String = String.Empty
                If Not IsDBNull(row("TotalAwardedBidsAmount")) Then
                    TotalAwardedBidsAmount = row("TotalAwardedBidsAmount")
                Else
                    TotalAwardedBidsAmount = "0"
                End If

                sw.WriteLine(Core.QuoteCSV(CompanyName) & "," & Core.QuoteCSV(TotalRequests) & "," & Core.QuoteCSV(ActiveBids) & "," & Core.QuoteCSV(PendingBids) & "," & Core.QuoteCSV(TotalBids) & "," & Core.QuoteCSV(TotalBidsAmount) & "," & Core.QuoteCSV(TotalAwardedBids) & "," & Core.QuoteCSV(TotalAwardedBidsAmount))
            Next
            sw.Flush()
            sw.Close()
            sw.Dispose()
            Response.Redirect(Folder & FileName)
        End If
    End Sub


End Class

