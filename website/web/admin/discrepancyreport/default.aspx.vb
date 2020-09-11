Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'CheckAccess("PRICE_COMPARISONS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            'F_BuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
            'F_BuilderID.DataValueField = "BuilderID"
            'F_BuilderID.DataTextField = "CompanyName"
            'F_BuilderID.Databind()
            'F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            'F_BuilderID.SelectedValue = Request("F_BuilderID")
            'F_CreatedLBound.Text = Request("F_CreatedLBound")
            'F_CreatedUBound.Text = Request("F_CreatedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "BuilderName"

            BindList()
        End If
    End Sub

    Function GetSQL(ByVal BuilderId As Integer, ByVal LastYear As Integer, ByVal LastQuarter As Integer) As String
        Dim SQL As String
        SQL = " Select b.HistoricId BuilderHistoricId, b.CompanyName As BuilderName, r.*, dr.DisputeResponse, drr.DisputeResponseReason from Builder b Inner Join ( "
        SQL &= " select v.HistoricId As VendorHistoricId, ptmp.BuilderId, ptmp.PurchasesReportID,ptmp.Modified,coalesce(ptmp.TotalAmount,0) as BuilderTotal, v.VendorID, v.CompanyName as VendorCompany, "
        SQL &= " coalesce(stmp.VendorTotal,0) as VendorTotal, stmp.SalesReportID, stmp.SalesReportDisputeID, stmp.DisputeResponseID, stmp.DisputeResponseReasonID, "
        SQL &= " stmp.ResolutionAmount, stmp.VendorTotalAmount, stmp.BuilderTotalAmount, stmp.BuilderComments, stmp.VendorComments, stmp.CreatorVendorAccountId"
        SQL &= " from  Vendor v left outer join ("
        SQL &= " select pr.builderid, pr.PurchasesReportID, pvt.TotalAmount,pvt.VendorID,pvt.Modified "
        SQL &= " from PurchasesReport pr inner join PurchasesReportVendorTotalAmount pvt on pr.PurchasesReportID=pvt.PurchasesReportID "
        SQL &= " where"
        SQL &= " pr.BuilderID=" & DB.Number(BuilderId) & " and "
        SQL &= " pr.PeriodQuarter=" & DB.Number(LastQuarter) & " and pr.PeriodYear=" & DB.Number(LastYear) & ") as ptmp     on v.VendorID = ptmp.VendorID full outer join  ("
        SQL &= " select sr.VendorID, srt.TotalAmount as VendorTotal, sr.SalesReportID, srd.SalesReportDisputeID, srd.DisputeResponseID, "
        SQL &= " srd.DisputeResponseReasonID, srd.ResolutionAmount, srd.BuilderComments, srd.VendorComments, srd.VendorTotalAmount, srd.BuilderTotalAmount, srt.CreatorVendorAccountId"
        SQL &= " from          ("
        SQL &= " select * from SalesReport where PeriodYear=" & DB.Number(LastYear) & " and PeriodQuarter=" & DB.Number(LastQuarter) & ") as sr     left outer join          ("
        SQL &= " select * from SalesReportBuilderTotalAmount where "
        SQL &= " BuilderID = " & DB.Number(BuilderId)
        SQL &= " ) as srt on srt.SalesReportID=sr.SalesReportID     left outer join          ("
        SQL &= " select * from SalesReportDispute where "
        SQL &= " BuilderID = " & DB.Number(BuilderId)
        SQL &= " ) as srd on sr.SalesReportID=srd.SalesReportID ) as stmp on v.VendorID = stmp.VendorID "
        SQL &= " where(ptmp.Modified Is Not null Or coalesce(ptmp.TotalAmount, 0) <> coalesce(stmp.VendorTotal, 0) Or stmp.SalesReportDisputeID Is Not null)"
        SQL &= " and (ptmp.VendorID is not null or stmp.VendorID is not null)"
        SQL &= " and coalesce(ptmp.TotalAmount,0) <> coalesce(stmp.VendorTotal,0)"
        SQL &= " ) r On b.BuilderId = r.BuilderId Left Outer Join DisputeResponse dr On r.DisputeResponseID = dr.DisputeResponseID Left Outer Join DisputeResponseReason drr On r.DisputeResponseReasonID = drr.DisputeResponseReasonID "
        Return SQL
    End Function

    Private Sub BindList()
        Dim SQL As String
        Dim LastYear As Integer = 2009
        Dim LastQuarter As Integer = 2

        SQL = "Select BuilderId From Builder Where IsActive = 1 Order By CompanyName"

        Dim dt As DataTable = DB.GetDataTable(SQL)
        Dim dtReport As DataTable = DB.GetDataTable(GetSQL(0, LastYear, LastQuarter))
        For Each row As DataRow In dt.Rows

            
            Dim dtBuilder As DataTable = DB.GetDataTable(GetSQL(row("BuilderId"), LastYear, LastQuarter))
            For Each r As DataRow In dtBuilder.Rows
                dtReport.ImportRow(r)
            Next
        Next

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        gvList.DataSource = dtReport.DefaultView
        gvList.DataBind()
    End Sub

    'Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
    '    If Not IsValid Then Exit Sub

    '    gvList.PageIndex = 0
    '    BindList()
    'End Sub
End Class

