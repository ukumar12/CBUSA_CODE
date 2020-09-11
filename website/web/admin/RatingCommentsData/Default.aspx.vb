Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("RATING_CATEGORIES")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
           
            F_BuilderID.DataSource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.DataBind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))
            F_LLCId.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLCId.DataTextField = "LLC"
            F_LLCId.DataValueField = "LLCID"
            F_LLCId.DataBind()
            F_LLCId.Items.Insert(0, New ListItem("-- ALL --", ""))


            F_LastName.Text = Request("F_LastName")
            F_CompanyName.Text = Request("F_CompanyName")
            F_Email.Text = Request("F_Email")
            F_WebsiteURL.Text = Request("F_WebsiteURL")
            
            F_LLCId.SelectedValue = Request("F_LLCId")

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


        SQLFields = "select b.BuilderID ,b.IsActive ,b.companyname, vc.comments ,vcrc.ratings "
        SQL = " from Builder b  "
        SQL &= " left outer join  (select COUNT(vendorcommentid) as comments ,BuilderID from VendorComment group by VendorComment .BuilderID )vc "
        SQL &= " on b.BuilderID = vc.builderid  "
        SQL &= " left outer join ( select  COUNT(builderid) as ratings,BuilderID  FROM VendorRatingCategoryRating group by VendorRatingCategoryRating.BuilderID )vcrc"
        SQL &= " on b.BuilderID = vcrc.BuilderID where vc.comments > 0 "


        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
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
        
        If Not F_LastName.Text = String.Empty Then
            SQL = SQL & Conn & "ba.LastName LIKE " & DB.FilterQuote(F_LastName.Text)
            Conn = " AND "
        End If
        If Not F_CompanyName.Text = String.Empty Then
            SQL = SQL & Conn & "b.CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text)
            Conn = " AND "
        End If
        If Not F_Email.Text = String.Empty Then
            SQL = SQL & Conn & "ba.Email LIKE " & DB.FilterQuote(F_Email.Text)
            Conn = " AND "
        End If
        
        If Not F_IsActive.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "b.IsActive  = " & DB.Number(F_IsActive.SelectedValue)
            Conn = " AND "
        End If
      
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

  

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub

    
End Class

