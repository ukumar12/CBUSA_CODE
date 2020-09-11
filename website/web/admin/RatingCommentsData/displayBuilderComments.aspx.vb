Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private BuilderID As Integer
    Private vendorID As Integer
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("RATING_CATEGORIES")
        BuilderID = Request("BuilderID")
        vendorID = Request("VendorID")
        If BuilderID = Nothing Then Response.Redirect("/admin/default.aspx")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then

            F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))
            

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "CompanyName"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " And "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        'SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " b.*, ba.FirstName, ba.LastName, (Select RegistrationStatus From RegistrationStatus Where RegistrationStatusId = b.RegistrationStatusid) As RegistrationStatus, llc.LLC, Coalesce((SELECT Top 1 'True' FROM BuilderRegistration br WHERE br.CompleteDate IS NOT NULL AND DateDiff(yyyy, br.CompleteDate, GetDate()) = 0 AND br.BuilderId = b.BuilderId), 'False') As IsRegistrationCompleted, Coalesce((SELECT Top 1 'True' FROM PurchasesReport pr WHERE pr.Submitted IS NOT NULL AND pr.PeriodYear = " & LastYear & " AND pr.PeriodQuarter = " & LastQuarter & " AND pr.BuilderId = b.BuilderId), 'False') As IsReportSubmitted "
        'SQL = " FROM Builder b Left Outer Join BuilderAccount ba On b.BuilderId = ba.BuilderId Left Outer Join LLC llc On b.LLCId = llc.LLCId "


        SQLFields = "select vc.*,b.companyName as Builder,v.CompanyName as Vendor "
        SQL = " from VendorComment vc inner join Builder b on vc.BuilderID = b.builderid inner join Vendor v on vc.VendorID = v.VendorID  where vc.builderID = " & Builderid

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "v.vendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
       

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

   
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub


End Class

