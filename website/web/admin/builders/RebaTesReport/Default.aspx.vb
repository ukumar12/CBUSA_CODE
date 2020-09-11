Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BUILDERS")
        'gvList.Attributes.Add("style", "word-break:break-all;word-wrap:break-word")
        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_BuilderID.DataSource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "HistoricID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.DataBind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "HistoricID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
            F_LLC.DataTextField = "LLC"
            F_LLC.DataValueField = "LLCID"
            F_LLC.DataBind()
            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_VendorID.SelectedValue = Request("F_VendorID")

            F_BuilderHistoricId.Text = Request("F_BuilderHistoricId")
            

            F_SubmittedLbound.Text = Request("F_SubmittedLBound")
            F_SubmittedUbound.Text = Request("F_SubmittedUBound")
            F_VendorHistoricId.Text = Request("F_VendorHistoricId")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "SubmittedDAte"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL, SqlCount As String
        Dim Conn As String = " Where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * , b.CompanyName As BuilderName, b.HistoricID as BuilderHistoricID , v.CompanyName , v.HistoricID as VendorHistoricID, b.LLCID as builderLLCID  ,l.LLC  "
        SQL = " FROM RebateEmailSent res inner join       Builder b on res.BLDRID = b.HistoricID  inner join Vendor v on res .VNDRID =  v.HistoricID inner join LLC l on b.LLCID = l.LLCID "


        SqlCount = "SELECT   * , b.CompanyName as BuilderName, b.HistoricID as BuilderHistoricID , v.CompanyName as VendorName , v.HistoricID as VendorHistoricID, b.LLCID as builderLLCID  ,l.LLC "
        If Not F_BuilderHistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "BLDRID = " & DB.Quote(F_BuilderHistoricId.Text)
            Conn = " AND "
        End If

       

        If Not F_VendorHistoricId.Text = String.Empty Then
            SQL = SQL & Conn & "VNDRID = " & DB.Quote(F_VendorHistoricId.Text)
            Conn = " AND "
        End If

        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & " b.HistoricID  = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & " v.HistoricID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
        
        If Not F_SubmittedLbound.Text = String.Empty Then
            SQL = SQL & Conn & "SubmittedDate >= " & DB.Quote(F_SubmittedLbound.Text)
            Conn = " AND "
        End If
        If Not F_SubmittedUbound.Text = String.Empty Then
            SQL = SQL & Conn & "SubmittedDate < " & DB.Quote(DateAdd("d", 1, F_SubmittedUbound.Text))
            Conn = " AND "
        End If
        If Not F_LLC.SelectedValues = String.Empty Then
            SQL = SQL & Conn & " b.LLCID  in  " & DB.NumberMultiple(F_LLC.SelectedValues)
            Conn = " AND "
        End If
        '  gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        Dim resCount As DataTable = DB.GetDataTable(SqlCount & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.Pager.NofRecords = resCount.Rows.Count

        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

     

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
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
