Imports System.IO
Imports Components
Imports DataLayer

Partial Class _default
    Inherits SitePage

    Private LLCID As Integer = 0
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
   Private OperationType As String = ""

    Private Property LoggedInType() As String
        Get
            Return ViewState("LoggedInType")
        End Get
        Set(ByVal value As String)
            ViewState("LoggedInType") = value
        End Set
    End Property

    Protected ReadOnly Property ExportUrl() As String
        Get
            Dim Output As String = Request.ServerVariables("URL").ToString.Trim & "?"
            If Not Request.ServerVariables("QUERY_STRING") Is Nothing AndAlso Request.ServerVariables("QUERY_STRING").ToString.Trim <> String.Empty Then
                Dim TempArray As String() = Split(Request.ServerVariables("QUERY_STRING").ToString.Trim, "&")
                For Each Param As String In TempArray
                    Dim TempArray2 As String() = Split(Param.ToString.Trim, "=")
                    If UBound(TempArray2) = 1 Then
                        If TempArray2(0).ToString.Trim <> String.Empty AndAlso TempArray2(0).ToString.ToLower.Trim <> "export" AndAlso TempArray2(0).ToString.ToLower.Trim <> "print" Then
                            If Right(Output, 1) <> "?" Then Output &= "&"
                            Output &= TempArray2(0).ToString.Trim & "=" & TempArray2(1).ToString.Trim
                        End If
                    End If
                Next
                If Right(Output, 1) <> "?" Then Output &= "&"
            End If
            Output &= "export=y&" & GetPageParams(Components.FilterFieldType.All)
            Return Output
        End Get
    End Property

    Protected ReadOnly Property PrintUrl() As String
        Get
            Dim Output As String = Request.ServerVariables("URL").ToString.Trim & "?"
            If Not Request.ServerVariables("QUERY_STRING") Is Nothing AndAlso Request.ServerVariables("QUERY_STRING").ToString.Trim <> String.Empty Then
                Dim TempArray As String() = Split(Request.ServerVariables("QUERY_STRING").ToString.Trim, "&")
                For Each Param As String In TempArray
                    Dim TempArray2 As String() = Split(Param.ToString.Trim, "=")
                    If UBound(TempArray2) = 1 Then
                        If TempArray2(0).ToString.Trim <> String.Empty AndAlso TempArray2(0).ToString.ToLower.Trim <> "print" Then
                            If Right(Output, 1) <> "?" Then Output &= "&"
                            Output &= TempArray2(0).ToString.Trim & "=" & TempArray2(1).ToString.Trim
                        End If
                    End If
                Next
                If Right(Output, 1) <> "?" Then Output &= "&"
            End If
            Output &= "print=y&" & GetPageParams(Components.FilterFieldType.All)
            Return Output
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("VendorId") Is Nothing And Session("BuilderId") Is Nothing And Session("PIQId") Is Nothing And Session("AdminID") Is Nothing Then
            Response.Redirect("/default.aspx")
        End If

        If Not Session("BuilderId") = Nothing Then LLCID = BuilderRow.GetRow(DB, Session("BuilderId")).LLCID

        If Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty Then
            pnlPrint.Visible = False
        End If

        PageURL = Request.Url.ToString()
        If Session("BuilderId") IsNot Nothing Then
            CurrentUserId = Session("BuilderId")
            OperationType = "Builder Top Menu Click"
        Else
            CurrentUserId = Session("VendorId")
            OperationType = "Vendor Top Menu Click"
        End If
        UserName = Session("Username")

        If Not IsPostBack Then

            Core.DataLog("Directory", PageURL, CurrentUserId, OperationType, "", "", "", "", UserName)

            LoadSupplyPhases()

            F_MemberType.Items.Add(New ListItem("Builder", "Builder"))

            Dim strSearchBuilder As String = Request.QueryString("builder")
            Dim strCompanyName As String = Request.QueryString("company")

            Try
                If CType(Me.Page, SitePage).IsLoggedInBuilder Then
                    LoggedInType = "builder"
                    F_MemberType.Items.Add(New ListItem("Vendor", "Vendor"))
                    F_MemberType.Items.Add(New ListItem("PIQ", "PIQ"))

                    If strSearchBuilder = "true" Then
                        Me.F_MemberType.SelectedValue = "Builder"
                        Me.F_CompanyName.Text = strCompanyName
                    Else
                        Me.F_MemberType.SelectedValue = "Vendor"
                    End If

                ElseIf CType(Me.Page, SitePage).IsLoggedInVendor Then
                    LoggedInType = "vendor"
                    tdSupplyPhase.InnerHtml = Nothing
                    F_SupplyPhase.Visible = False
                    Me.F_MemberType.SelectedValue = "Builder"
                ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ Then
                    LoggedInType = "piq"
                    F_MemberType.Items.Add(New ListItem("Vendor", "Vendor"))
                    F_MemberType.Items.Add(New ListItem("PIQ", "PIQ"))
                    Me.F_MemberType.SelectedValue = "Builder"
                Else
                    Me.F_MemberType.SelectedValue = "Builder"
                End If

            Catch ex As Exception
                Me.F_MemberType.SelectedValue = "Builder"
            End Try

            If (Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty) _
                Or (Not Request("export") Is Nothing AndAlso Request("export").ToString.Trim <> String.Empty) Then
                If Not Request("F_MemberType") Is Nothing Then
                    F_MemberType.SelectedValue = Request("F_MemberType").ToString.Trim
                End If
                If F_SupplyPhase.Visible AndAlso Not Request("F_SupplyPhase") Is Nothing Then
                    F_SupplyPhase.SelectedValue = Request("F_SupplyPhase").ToString.Trim
                End If
                If Not Request("F_ContactFirstName") Is Nothing Then
                    F_ContactFirstName.Text = Request("F_ContactFirstName").ToString.Trim
                End If
                If Not Request("F_ContactLastName") Is Nothing Then
                    F_ContactLastName.Text = Request("F_ContactLastName").ToString.Trim
                End If
                If Not Request("F_CompanyName") Is Nothing Then
                    F_CompanyName.Text = Request("F_CompanyName").ToString.Trim
                End If
            End If

            LoadDirectory()
        End If

        SupplyPhase.Visible = F_MemberType.SelectedValue = "Vendor"
        F_SupplyPhase.Visible = F_MemberType.SelectedValue = "Vendor"

        If (Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty) Then
            Select Case F_MemberType.SelectedValue
                Case "Builder", "PIQ"
                    divWrapper.Style.Remove("width")
                Case "Vendor"
                    divWrapper.Style("width") = "650"
                    'divWrapper2.Style.Remove("width")
            End Select
        Else
            divWrapper.Style("width") = "80%"
            'Select Case F_MemberType.SelectedValue
            '    Case "Builder", "PIQ"
            '        divWrapper.Style("width") = "60%"
            '    Case "Vendor"
            '        divWrapper.Style.Remove("width")
            'End Select
        End If

        If Not Request("export") Is Nothing AndAlso Request("export").ToString.Trim <> String.Empty Then
            'Response.Clear()
            'Response.Write("Creating export file.  Please wait...")
            'Response.Flush()
            ExportList()
        End If

    End Sub
    Private Sub LoadVendorRating(VendorId As Int32)
        Dim OverallAvg As Double = Core.GetDouble(DB.ExecuteScalar("select Avg(cast(coalesce(Rating,0) as float)) from VendorRatingCategoryRating where VendorID=" & VendorId & " and Coalesce(Rating,0) > 0"))

    End Sub
    Private Sub LoadSupplyPhases()
        Dim RootSupplyPhaseId = SupplyPhaseRow.GetRootSupplyPhase(DB).SupplyPhaseID

        'Me.F_SupplyPhase.DataSource = DB.GetDataTable("SELECT SupplyPhase, SupplyPhaseId FROM SupplyPhase WHERE ParentSupplyPhaseId = " & DB.Number(RootSupplyPhaseId) & " AND SupplyPhaseId NOT IN (SELECT SupplyPhaseId FROM SupplyPhaseLLCExclusion WHERE LLCID = " & DB.Number(LLCID) & ") ORDER BY SupplyPhase ASC")
        Me.F_SupplyPhase.DataSource = VendorCategoryRow.GetList(DB, "SortOrder") 'SupplyPhaseRow.GetChildren(DB, RootSupplyPhaseId)
        Me.F_SupplyPhase.DataValueField = "VendorCategoryId"
        Me.F_SupplyPhase.DataTextField = "Category"
        Me.F_SupplyPhase.DataBind()
        Me.F_SupplyPhase.Items.Insert(0, New ListItem("-- ALL --", ""))

    End Sub

    Private Sub LoadDirectory()

        Dim EntityTable As String = ""
        Dim EntityTableAccount As String = ""
        Dim EntityTableId As String = ""
        Dim EntityRegistration As String = ""
        Dim SQL As String = ""
        Dim WhereClause As String = ""
        Dim JoinClause As String = ""

        'select the correct entity table and 
        Select Case Me.F_MemberType.SelectedValue
            Case "Builder"
                EntityTable = Me.F_MemberType.SelectedValue
                EntityTableAccount = Me.F_MemberType.SelectedValue & "Account"
                EntityTableId = Me.F_MemberType.SelectedValue & "ID"
                EntityRegistration = Me.F_MemberType.SelectedValue & "Registration"
            Case "Vendor"
                EntityTable = Me.F_MemberType.SelectedValue
                EntityTableAccount = Me.F_MemberType.SelectedValue & "Account"
                EntityTableId = Me.F_MemberType.SelectedValue & "ID"
                EntityRegistration = Me.F_MemberType.SelectedValue & "Registration"
            Case "PIQ"
                EntityTable = Me.F_MemberType.SelectedValue
                EntityTableAccount = Me.F_MemberType.SelectedValue & "Account"
                EntityTableId = Me.F_MemberType.SelectedValue & "ID"
                EntityRegistration = Me.F_MemberType.SelectedValue & "Registration"
        End Select
        'If Me.F_MemberType.SelectedValue = "Vendor" Then
        '    JoinClause = EntityTable & " et " & vbCrLf & " join VendorRatingCategoryRating vr on et." & EntityTableId & " = vr.VendorID"
        'Else
        '    JoinClause = EntityTable & " et " & vbCrLf
        'End If

        If Me.F_MemberType.SelectedValue = "Vendor" Then
            SQL = "SELECT" & vbCrLf
            SQL = SQL & " Row_Number() Over(order by CompanyName) as Rank, et." & EntityTableId & " ID, count(distinct vr.BuilderId) as NoOfReviews,isnull(Avg(case when rating > 0 then cast(coalesce(Rating,0) as float) end),0) Rating, et.CompanyName, et.WebsiteURL, et.Address, et.Address2, et.City, et.State, et.Zip, et.Phone, et.Fax "
            SQL = SQL & "  , et.LogoFile as LogoFile" & vbCrLf
        Else
            SQL = "SELECT" & vbCrLf
            SQL = SQL & " Row_Number() Over(order by CompanyName) as Rank, et." & EntityTableId & " ID, et.CompanyName, et.WebsiteURL, et.Address, et.Address2, et.City, et.State, et.Zip, et.Phone, et.Fax "
            SQL = SQL & "  , NULL as LogoFile" & vbCrLf
        End If
        ' Added For rating Control By Susobhan
        SQL &= "FROM" & vbCrLf
        If Me.F_MemberType.SelectedValue = "Vendor" Then
            JoinClause = EntityTable & " et " & vbCrLf & " left join VendorRatingCategoryRating vr on et." & EntityTableId & " = vr.VendorID"
        Else
            JoinClause = EntityTable & " et " & vbCrLf
        End If



        'join clause is changed by the supply phase
        If Me.F_MemberType.SelectedValue = "Vendor" AndAlso Me.F_SupplyPhase.SelectedValue <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " And "
            WhereClause = WhereClause & " et." & EntityTableId & " In (Select " & EntityTableId & " from VendorCategoryVendor where VendorCategoryID = " & DB.Number(Me.F_SupplyPhase.SelectedValue) & ")" & vbCrLf
        End If

        SQL = SQL & JoinClause

        If Me.F_ContactFirstName.Text <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " And "
            WhereClause = WhereClause & " et." & EntityTableId & " In (Select " & EntityTableId & " from " & EntityTableAccount & " where FirstName Like " & DB.FilterQuote(Me.F_ContactFirstName.Text) & ")" & vbCrLf
        End If

        If Me.F_ContactLastName.Text <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " And "
            WhereClause = WhereClause & " et." & EntityTableId & " In (Select " & EntityTableId & " from " & EntityTableAccount & " where LastName Like " & DB.FilterQuote(Me.F_ContactLastName.Text) & ")" & vbCrLf
        End If

        If Me.F_CompanyName.Text <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " And "
            WhereClause = WhereClause & " et.CompanyName Like " & DB.FilterQuote(Me.F_CompanyName.Text) & vbCrLf
        End If

        If Me.F_MemberType.SelectedValue = "Builder" AndAlso Not Session("BuilderId") Is Nothing Then
            If WhereClause <> "" Then WhereClause = WhereClause & " And "
            WhereClause = WhereClause & " et.LLCID = " & DB.Number(LLCID) & vbCrLf
        ElseIf F_MemberType.SelectedValue = "Builder" AndAlso Not Session("VendorId") Is Nothing Then
            If WhereClause <> "" Then WhereClause = WhereClause & " And "
            WhereClause = WhereClause & " et.LLCID In (Select LLCID from LLCVendor where VendorID=" & DB.Number(Session("VendorId")) & ")"
        End If

        If Me.F_MemberType.SelectedValue = "Vendor" AndAlso Not Session("BuilderId") Is Nothing Then
            If WhereClause <> "" Then WhereClause = WhereClause & " And "
            WhereClause = WhereClause & " et." & EntityTableId & " In (Select " & EntityTableId & " from LLCVendor where IsActive = 1 And LLCID = " & DB.Number(LLCID) & ")" & vbCrLf
        End If

        If LoggedInType = "piq" AndAlso PIQAccountRow.GetLLCCount(DB, Session("PiqAccountId")) > 0 Then
            If F_MemberType.SelectedValue = "Vendor" Then
                If WhereClause <> String.Empty Then WhereClause &= " And "
                WhereClause &= " et.VendorId In (Select VendorId from LLCVendor lv inner join PiqAccountLLC lp On lv.LLCId=lp.LLCId where lp.PiqAccountId=" & DB.Number(Session("PiqAccountId")) & ")"
            ElseIf F_MemberType.SelectedValue = "Builder" Then
                If WhereClause <> String.Empty Then WhereClause &= " And "
                WhereClause &= " et.LLCId In (Select LLCID from LLC where isactive =1)"
                If WhereClause <> String.Empty Then WhereClause &= " And "
                WhereClause &= " et.LLCId In (Select LLCId from PiqAccountLLC where PiqAccountId=" & DB.Number(Session("PiqAccountId")) & ")"
            End If
        End If

        If WhereClause <> "" Then WhereClause = WhereClause & " And "
        WhereClause = WhereClause & " et.IsActive = 1"

        'add where clause to the main SQL string
        If WhereClause <> "" Then
            SQL = SQL & " WHERE " & vbCrLf & WhereClause & vbCrLf
        End If
        If Me.F_MemberType.SelectedValue = "Vendor" Then
            SQL = SQL & " group by et." & EntityTableId & ",et.CompanyName, et.WebsiteURL, et.Address, et.Address2, et.City, et.State, et.Zip, et.Phone, et.Fax,et.LogoFile"
        End If

        Dim curPage As Integer = ctlNavigate.PageNumber
        Dim perPage As Integer = ctlNavigate.MaxPerPage
        Dim cnt As Integer = DB.ExecuteScalar("Select count(*) from (" & SQL & ") As tmp")

        If Not cnt = 0 Then
            ctlNavigate.NofRecords = cnt
            ctlNavigate.DataBind()
        Else
            ctlNavigate.Visible = False
            pnlNoResults.Visible = True
        End If

        If Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty Then
            ctlNavigate.Visible = False
            curPage = 1
            perPage = 32000
        End If

        SQL = "Select top " & perPage & " * from (" & SQL & ") As tmp where Rank > " & DB.Number(perPage * (curPage - 1))

        ' order by clause, sort by company name
        SQL = SQL & " ORDER BY Rank " & vbCrLf



        Dim res As DataTable = DB.GetDataTable(SQL)
        rptDirectory.DataSource = res.DefaultView
        rptDirectory.DataBind()

    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        LoadDirectory()
    End Sub

    Protected Sub rptDirectory_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDirectory.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim ltlLogo As Literal = e.Item.FindControl("ltlLogo")
        Dim divCompanyName As HtmlGenericControl = CType(e.Item.FindControl("divCompanyName"), HtmlGenericControl)
        Dim ltlCompanyName As Literal = e.Item.FindControl("ltlCompanyName")
        Dim btnPreferredVendorDetails As LinkButton = CType(e.Item.FindControl("btnPreferredVendorDetails"), LinkButton)
        Dim btnPreferredVendorPhotos As LinkButton = CType(e.Item.FindControl("btnPreferredVendorPhotos"), LinkButton)
        Dim btnPiqDetails As LinkButton = CType(e.Item.FindControl("btnPiqDetails"), LinkButton)
        Dim hrefWebsiteURL As HtmlAnchor = CType(e.Item.FindControl("hrefWebsiteURL"), HtmlAnchor)
        Dim tdCompanyName As HtmlTableCell = e.Item.FindControl("tdCompanyName")
        Dim tdWebsiteURL As HtmlTableCell = e.Item.FindControl("tdWebsiteURL")
        Dim tdAddressInfo As Literal = e.Item.FindControl("tdAddressInfo")
        Dim spanSupplyPhase As HtmlGenericControl = e.Item.FindControl("spanSupplyPhase")
        Dim tdSupplyPhase As HtmlTableCell = e.Item.FindControl("tdSupplyPhase")
        Dim tdMarkets As HtmlTableCell = e.Item.FindControl("tdMarkets")
        Dim spanMarkets As HtmlGenericControl = e.Item.FindControl("spanMarkets")
        ' Added For rating Control By Susobhan
        Dim ltlReviewDetails As Literal = e.Item.FindControl("ltlReviewDetails")
        Dim ltlStarRating As Literal = e.Item.FindControl("ltlStarRating")


        Dim table As HtmlTable = e.Item.FindControl("table")
        'Dim trContacts As HtmlTableRow = CType(e.Item.FindControl("trContacts"), HtmlTableRow)
        Dim tdPrimaryContact As HtmlTableCell = CType(e.Item.FindControl("tdPrimaryContact"), HtmlTableCell)
        Dim divWrapper2 As HtmlGenericControl = CType(e.Item.FindControl("divWrapper2"), HtmlGenericControl)

        If (Not Request("print") Is Nothing AndAlso Request("print").ToString.Trim <> String.Empty) Then
            Select Case F_MemberType.SelectedValue
                Case "Builder", "PIQ"
                    '
                Case "Vendor"
                    divWrapper2.Style.Remove("width")
            End Select
        End If

        If IsDBNull(e.Item.DataItem("LogoFile")) Then
            ltlLogo.Text = "&nbsp;"
        Else
            ltlLogo.Text = "<img src=""/assets/vendor/logo/standard/" & e.Item.DataItem("LogoFile") & """ alt=""" & e.Item.DataItem("CompanyName") & """ style=""border:none;""/>"
            If Not IsDBNull(e.Item.DataItem("WebsiteURL")) Then
                ltlLogo.Text = "<a href=""" & e.Item.DataItem("WebsiteURL") & """ target=""_blank"">" & ltlLogo.Text & "</a>"
            End If
        End If

        If Me.F_MemberType.SelectedValue = "Builder" Then
            ltlCompanyName.Text = e.Item.DataItem("CompanyName")
            btnPreferredVendorDetails.Visible = False
            btnPreferredVendorPhotos.Visible = False
            btnPiqDetails.Visible = False
        ElseIf Me.F_MemberType.SelectedValue = "Vendor" Then
            divCompanyName.Style.Add("height", "33px")
            ltlCompanyName.Text = "<a class='ComapnyName' href=""vendor.aspx?vendorid=" & e.Item.DataItem("ID") & """>" & e.Item.DataItem("CompanyName") & "</a>"
            ltlReviewDetails.Text = "Avg " & Math.Round(e.Item.DataItem("Rating"), 2) & "/ 10 | " & e.Item.DataItem("NoOfReviews") & " ratings"
            Dim i As Integer
            ltlStarRating.Text = "<a  target='_blank' href=""vendor.aspx?vendorid=" & e.Item.DataItem("ID") & """>"
            For i = 1 To Math.Ceiling(e.Item.DataItem("Rating"))
                ltlStarRating.Text &= "<img alt=""" & e.Item.DataItem("Rating") & """ src=""/images/rating/star-red-sm.gif"" />"
            Next

            For i = i To 10
                ltlStarRating.Text &= "<img alt=""" & e.Item.DataItem("Rating") & """ src=""/images/rating/star-gr-sm.gif"" />"
            Next
            ltlStarRating.Text &= "</a>"
            btnPreferredVendorDetails.PostBackUrl = "vendor.aspx?vendorid=" & e.Item.DataItem("ID")
            If DB.GetDataTable("Select PhotoId From VendorPhoto Where VendorId = " & DB.Number(e.Item.DataItem("ID"))).Rows.Count > 0 Then
                btnPreferredVendorPhotos.PostBackUrl = "/vendor/photos/photos.aspx?vendorid=" & e.Item.DataItem("ID")
            Else
                btnPreferredVendorPhotos.Visible = False
            End If

            btnPiqDetails.Visible = False
        ElseIf Me.F_MemberType.SelectedValue = "PIQ" Then
            divCompanyName.Style.Add("height", "33px")
            ltlCompanyName.Text = "<a style=""float:left;"" href=""piq.aspx?piqid=" & e.Item.DataItem("ID") & """>" & e.Item.DataItem("CompanyName") & "</a>"
            btnPreferredVendorDetails.Visible = False
            btnPreferredVendorPhotos.Visible = False
            btnPiqDetails.PostBackUrl = "piq.aspx?piqid=" & e.Item.DataItem("ID") & "&" & GetPageParams(FilterFieldType.All)
        End If

        If IsDBNull(e.Item.DataItem("WebsiteURL")) OrElse e.Item.DataItem("WebsiteURL") = String.Empty Then
            hrefWebsiteURL.Visible = False
        Else
            hrefWebsiteURL.HRef = IIf(e.Item.DataItem("WebsiteURL").ToString.StartsWith("http"), e.Item.DataItem("WebsiteURL"), "http://" & e.Item.DataItem("WebsiteURL"))
            hrefWebsiteURL.InnerText = hrefWebsiteURL.HRef
        End If

        'Address Block
        tdAddressInfo.Text = "<b class=""smaller nopad"">Mailing Address:</b><br/>"
        tdAddressInfo.Text &= e.Item.DataItem("Address").ToString

        If e.Item.DataItem("Address2").ToString <> String.Empty Then
            If Trim(tdAddressInfo.Text) <> String.Empty Then tdAddressInfo.Text &= "<br/>"
            tdAddressInfo.Text &= e.Item.DataItem("Address2").ToString
        End If

        If Trim(tdAddressInfo.Text) <> String.Empty Then tdAddressInfo.Text &= "<br/>"
        tdAddressInfo.Text &= e.Item.DataItem("City").ToString & ", " & e.Item.DataItem("State").ToString & " " & e.Item.DataItem("Zip").ToString

        If Not IsDBNull(e.Item.DataItem("Phone")) Then
            tdAddressInfo.Text &= "<br/><br/><b class=""smaller nopad"">Telephone:</b><br/>" & e.Item.DataItem("Phone")
        End If

        If Me.F_MemberType.SelectedValue = "Vendor" Then
            Dim sSupplyPhases As String = VendorCategoryRow.GetNames(DB, VendorRow.GetRow(DB, e.Item.DataItem("ID")).GetSelectedVendorCategories, ",")
            If sSupplyPhases <> String.Empty Then
                spanSupplyPhase.InnerHtml = Replace(sSupplyPhases, ",", ", ")
            End If

            If spanSupplyPhase.InnerHtml = String.Empty Then
                tdSupplyPhase.InnerHtml = String.Empty
            End If




        ElseIf F_MemberType.SelectedValue = "PIQ" Then
            Dim dtPreferred As DataTable = PIQRow.GetPreferredVendorDetails(DB, e.Item.DataItem("ID"))
            Dim sPreferred As New StringBuilder
            If dtPreferred.Rows.Count > 0 Then
                sPreferred.Append("<b class=""smaller"">Preferred Vendors:</b><br/>")
                For Each row As DataRow In dtPreferred.Rows
                    sPreferred.Append(row("CompanyName") & "<br/>")
                Next
            End If
            tdSupplyPhase.InnerHtml = sPreferred.ToString
        Else
            tdSupplyPhase.InnerHtml = String.Empty
        End If
        Dim sMarkets As String = String.Empty
        Dim conc As String = String.Empty
        If Me.F_MemberType.SelectedValue = "Vendor" Then
            For Each id As String In VendorRow.GetLLCList(DB, e.Item.DataItem("ID")).Split(",")
                If LLCRow.GetRow(DB, id).IsActive Then
                    sMarkets &= conc & LLCRow.GetRow(DB, id).LLC
                    conc = ","
                End If
            Next
            If sMarkets <> String.Empty Then
                spanMarkets.InnerHtml = sMarkets
            End If

            If spanMarkets.InnerHtml = String.Empty Then
                tdMarkets.InnerHtml = String.Empty
            End If
        ElseIf F_MemberType.SelectedValue = "PIQ" Then
            spanMarkets.InnerHtml = String.Empty
            tdMarkets.InnerHtml = String.Empty
        Else
            sMarkets = LLCRow.GetRow(DB, BuilderRow.GetRow(DB, e.Item.DataItem("ID")).LLCID).LLC
            If sMarkets <> String.Empty Then
                spanMarkets.InnerHtml = sMarkets
            End If

            If spanMarkets.InnerHtml = String.Empty Then
                tdMarkets.InnerHtml = String.Empty
            End If
        End If


        Dim dtContacts As DataTable
        Dim sSQL As String = String.Empty
        Select Case Me.F_MemberType.SelectedValue
            Case "Builder"
                sSQL = "SELECT ba.FirstName, ba.LastName, ba.Title, ba.Email, ba.Phone, ba.Mobile, ba.Fax,ba.IsPrimary FROM BuilderAccount AS ba INNER JOIN Builder AS bldr ON ba.BuilderID = bldr.BuilderID WHERE ba.IsActive = 1 AND ba.BuilderID=" & DB.Number(e.Item.DataItem("ID")) & " ORDER BY ba.IsPrimary DESC, ba.FirstName ASC, ba.LastName ASC "
            Case "Vendor"
                Dim VendorRoleId As Integer
                VendorRoleId = DB.ExecuteScalar("SELECT Top 1 VendorRoleId FROM VendorRole WHERE VendorRole = 'Primary Contact'")
                sSQL = "SELECT Top 1 va.VendorAccountID, va.FirstName, va.LastName, va.Title, va.Email, va.Phone, va.Mobile, va.Fax FROM VendorAccount AS va INNER JOIN Vendor AS vndr ON va.VendorID = vndr.VendorID INNER JOIN VendorAccountVendorRole AS vavr ON vavr.VendorAccountId = va.VendorAccountId WHERE va.IsActive = 1 AND va.VendorID=" & DB.Number(e.Item.DataItem("ID")) & " AND vavr.VendorRoleID = " & DB.Number(VendorRoleId) & " ORDER BY va.FirstName ASC, va.LastName ASC"
            Case "PIQ"
                sSQL = "SELECT pa.FirstName, pa.LastName, NULL AS Title, pa.Email, pa.Phone, pa.Mobile, pa.Fax, pa.IsPrimary FROM PIQAccount AS pa INNER JOIN PIQ AS piq ON pa.PIQID = piq.PIQID WHERE piq.IsActive = 1 AND pa.IsPrimary = 0 AND pa.PIQID=" & DB.Number(e.Item.DataItem("ID"))
                If LoggedInType = "vendor" Then
                    sSQL &= " and pa.PiqAccountId in (select pal.PiqAccountId from PiqAccountLLC pal inner join LLCVendor lv on pal.LLCId=lv.LLCId where lv.VendorId=" & DB.Number(Session("VendorId")) & ")"
                ElseIf LoggedInType = "builder" Then
                    sSQL &= " and pa.PiqAccountId in (select pal.PiqAccountId from PiqAccountLLC pal inner join Builder b on pal.LLCId=b.LLCId where b.BuilderId=" & DB.Number(Session("BuilderId")) & ")"
                End If
                sSQL &= " order by IsPrimary, LastName"
        End Select

        Dim trOtherContacts As HtmlTableCell = e.Item.FindControl("trOtherContacts")


        'For PIQ get primary contact then populate all other contacts
        If F_MemberType.SelectedValue = "PIQ" Then
            Dim sSQLPrimary As String = "SELECT Top 1 pa.FirstName, pa.LastName, NULL AS Title, pa.Email, pa.Phone, pa.Mobile, pa.Fax, pa.IsPrimary FROM PIQAccount AS pa INNER JOIN PIQ AS piq ON pa.PIQID = piq.PIQID WHERE pa.IsPrimary = 1 AND piq.IsActive = 1 AND pa.PIQID=" & DB.Number(e.Item.DataItem("ID"))
            dtContacts = DB.GetDataTable(sSQLPrimary)
            If dtContacts.Rows.Count = 0 Then
                'trContacts.Visible = False
                trOtherContacts.Visible = False
            Else
                Dim v As DataRow = dtContacts.Rows(0)
                tdPrimaryContact.InnerHtml = String.Empty
                tdPrimaryContact.InnerHtml = "<b class=""smaller nopad"">Primary Contact:</b><br/>"
                tdPrimaryContact.InnerHtml &= Core.BuildFullName(v("FirstName"), String.Empty, v("LastName"))

                If Not IsDBNull(v("Title")) AndAlso Not v("Title") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Title: " & v("Title")
                End If

                If F_MemberType.SelectedValue = "Vendor" Then
                    Dim dbVendorAccount As VendorAccountRow
                    dbVendorAccount = VendorAccountRow.GetRow(DB, v("VendorAccountID"))

                    Dim sVendorAccountRoles As String = dbVendorAccount.GetSelectedVendorRoleLabels()
                    If Not sVendorAccountRoles = String.Empty Then
                        tdPrimaryContact.InnerHtml &= " <br/>Roles: " & sVendorAccountRoles
                    End If
                End If

                If Not IsDBNull(v("Email")) AndAlso Not v("Email") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Email: <a href=""mailto:" & v("Email") & """ >" & v("Email") & "</a>"
                End If

                If Not IsDBNull(v("Phone")) AndAlso Not v("Phone") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Phone: " & v("Phone")
                End If

                If Not IsDBNull(v("Mobile")) AndAlso Not v("Mobile") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Mobile: " & v("Mobile")
                End If

                If Not IsDBNull(v("Fax")) AndAlso Not v("Fax") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Fax: " & v("Fax")
                End If
            End If

            dtContacts = DB.GetDataTable(sSQL)

            If dtContacts.Rows.Count = 0 Then
                trOtherContacts.Visible = False
            Else
                Dim divContacts As HtmlGenericControl = e.Item.FindControl("divContacts")

                trOtherContacts.Visible = True

                For i As Integer = 0 To dtContacts.Rows.Count - 1
                    Dim v As DataRow = dtContacts.Rows(i)

                    divContacts.InnerHtml &= "<b>" & Core.BuildFullName(v("FirstName"), String.Empty, v("LastName")) & "</b>"

                    If Not IsDBNull(v("Title")) AndAlso Not v("Title") = String.Empty Then
                        divContacts.InnerHtml &= "<br/>Title: " & v("Title")
                    End If

                    If F_MemberType.SelectedValue = "Vendor" Then
                        Dim dbVendorAccount As VendorAccountRow
                        dbVendorAccount = VendorAccountRow.GetRow(DB, v("VendorAccountID"))

                        Dim sVendorAccountRoles As String = dbVendorAccount.GetSelectedVendorRoleLabels()
                        If Not sVendorAccountRoles = String.Empty Then
                            divContacts.InnerHtml &= " <br/>Roles: " & sVendorAccountRoles
                        End If
                    End If

                    If Not IsDBNull(v("Email")) AndAlso Not v("Email") = String.Empty Then
                        divContacts.InnerHtml &= "<br/>Email: <a href=""mailto:" & v("Email") & """ >" & v("Email") & "</a>"
                    End If

                    If Not IsDBNull(v("Phone")) AndAlso Not v("Phone") = String.Empty Then
                        divContacts.InnerHtml &= "<br/>Phone: " & v("Phone")
                    End If

                    If Not IsDBNull(v("Mobile")) AndAlso Not v("Mobile") = String.Empty Then
                        divContacts.InnerHtml &= "<br/>Mobile: " & v("Mobile")
                    End If

                    If Not IsDBNull(v("Fax")) AndAlso Not v("Fax") = String.Empty Then
                        divContacts.InnerHtml &= "<br/>Fax: " & v("Fax")
                    End If

                    divContacts.InnerHtml &= "<br/><br/>"
                Next
            End If
        Else
            'For Builders, primary contact is first contact on the list then populate all other conatct starting from the second row.
            dtContacts = DB.GetDataTable(sSQL)

            If dtContacts.Rows.Count = 0 Then
                trOtherContacts.Visible = False
            Else

                Dim v As DataRow = dtContacts.Rows(0)
                tdPrimaryContact.InnerHtml = String.Empty
                tdPrimaryContact.InnerHtml = "<b class=""smaller nopad"">Primary Contact:</b><br/>"
                tdPrimaryContact.InnerHtml &= Core.BuildFullName(v("FirstName"), String.Empty, v("LastName"))

                If Not IsDBNull(v("Title")) AndAlso Not v("Title") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Title: " & v("Title")
                End If

                If F_MemberType.SelectedValue = "Vendor" Then
                    Dim dbVendorAccount As VendorAccountRow
                    dbVendorAccount = VendorAccountRow.GetRow(DB, v("VendorAccountID"))

                    Dim sVendorAccountRoles As String = dbVendorAccount.GetSelectedVendorRoleLabels()
                    If Not sVendorAccountRoles = String.Empty Then
                        tdPrimaryContact.InnerHtml &= " <br/>Roles: " & sVendorAccountRoles
                    End If
                End If

                If Not IsDBNull(v("Email")) AndAlso Not v("Email") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Email: <a href=""mailto:" & v("Email") & """ >" & v("Email") & "</a>"
                End If

                If Not IsDBNull(v("Phone")) AndAlso Not v("Phone") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Phone: " & v("Phone")
                End If

                If Not IsDBNull(v("Mobile")) AndAlso Not v("Mobile") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Mobile: " & v("Mobile")
                End If

                If Not IsDBNull(v("Fax")) AndAlso Not v("Fax") = String.Empty Then
                    tdPrimaryContact.InnerHtml &= "<br/>Fax: " & v("Fax")
                End If

                If F_MemberType.SelectedValue = "Builder" And dtContacts.Rows.Count > 0 Then

                    Dim divContacts As HtmlGenericControl = e.Item.FindControl("divContacts")

                    trOtherContacts.Visible = True

                    For i As Integer = 1 To dtContacts.Rows.Count - 1
                        v = dtContacts.Rows(i)

                        divContacts.InnerHtml &= "<b>" & Core.BuildFullName(v("FirstName"), String.Empty, v("LastName")) & "</b>"

                        If Not IsDBNull(v("Title")) AndAlso Not v("Title") = String.Empty Then
                            divContacts.InnerHtml &= "<br/>Title: " & v("Title")
                        End If

                        If F_MemberType.SelectedValue = "Vendor" Then
                            Dim dbVendorAccount As VendorAccountRow
                            dbVendorAccount = VendorAccountRow.GetRow(DB, v("VendorAccountID"))

                            Dim sVendorAccountRoles As String = dbVendorAccount.GetSelectedVendorRoleLabels()
                            If Not sVendorAccountRoles = String.Empty Then
                                divContacts.InnerHtml &= " <br/>Roles: " & sVendorAccountRoles
                            End If
                        End If

                        If Not IsDBNull(v("Email")) AndAlso Not v("Email") = String.Empty Then
                            divContacts.InnerHtml &= "<br/>Email: <a href=""mailto:" & v("Email") & """ >" & v("Email") & "</a>"
                        End If

                        If Not IsDBNull(v("Phone")) AndAlso Not v("Phone") = String.Empty Then
                            divContacts.InnerHtml &= "<br/>Phone: " & v("Phone")
                        End If

                        If Not IsDBNull(v("Mobile")) AndAlso Not v("Mobile") = String.Empty Then
                            divContacts.InnerHtml &= "<br/>Mobile: " & v("Mobile")
                        End If

                        If Not IsDBNull(v("Fax")) AndAlso Not v("Fax") = String.Empty Then
                            divContacts.InnerHtml &= "<br/>Fax: " & v("Fax")
                        End If

                        divContacts.InnerHtml &= "<br/><br/>"
                    Next
                Else
                    trOtherContacts.Visible = False
                End If
            End If
        End If


    End Sub

    Protected Sub ctlNavigate_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigate.NavigatorEvent
        ctlNavigate.PageNumber = e.PageNumber
        LoadDirectory()
    End Sub

    Protected Sub F_MemberType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles F_MemberType.SelectedIndexChanged
        SupplyPhase.Visible = F_MemberType.SelectedValue = "Vendor"
        F_SupplyPhase.Visible = F_MemberType.SelectedValue = "Vendor"
    End Sub

    Protected Sub btnExport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
        If Convert.ToInt32(rptDirectory.Items.Count) > 5000 Then
            ScheduleExport()
        Else
            Response.Redirect(ExportUrl)
        End If
    End Sub

    Private Sub ScheduleExport()
        Dim IdValue As Integer = 0
        Dim IdField As String = String.Empty
        Dim NotificationEmail As String = String.Empty
        If CType(Me.Page, SitePage).IsLoggedInBuilder AndAlso Not Session("BuilderAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("BuilderAccountId"))
            IdField = "BuilderAccountId"
            Dim dbAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, IdValue)
            NotificationEmail = dbAccount.Email.ToString.Trim
        ElseIf CType(Me.Page, SitePage).IsLoggedInVendor AndAlso Not Session("VendorAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("VendorAccountId"))
            IdField = "VendorAccountId"
            Dim dbAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, IdValue)
            NotificationEmail = dbAccount.Email.ToString.Trim
        ElseIf CType(Me.Page, SitePage).IsLoggedInPIQ AndAlso Not Session("PIQAccountId") Is Nothing Then
            IdValue = Convert.ToInt32(Session("PIQAccountId"))
            IdField = "PIQAccountId"
            NotificationEmail = String.Empty
        End If
        If IdValue <= 0 Then Exit Sub

        Dim Parameters As String = "NotificationEmail=" & NotificationEmail.ToString.Trim
        If Not Session("VendorId") Is Nothing AndAlso Session("VendorId").ToString.Trim <> String.Empty Then
            If Parameters.Trim <> String.Empty Then Parameters &= "&"
            Parameters &= "VendorId=" & Session("VendorId").ToString.Trim
        End If
        If Not Session("BuilderId") Is Nothing AndAlso Session("BuilderId").ToString.Trim <> String.Empty Then
            If Parameters.Trim <> String.Empty Then Parameters &= "&"
            Parameters &= "BuilderId=" & Session("BuilderId").ToString.Trim
        End If
        If LLCID <> Nothing AndAlso LLCID.ToString.Trim <> String.Empty Then
            If Parameters.Trim <> String.Empty Then Parameters &= "&"
            Parameters &= "LLCID=" & LLCID.ToString.Trim
        End If
        If Parameters.Trim <> String.Empty Then Parameters &= "&"
        Parameters &= GetPageParams(Components.FilterFieldType.All)

        ExportQueueRow.Add(DB, IdValue, IdField, "member_directory", Parameters)

        divNotification.Visible = True
    End Sub

    Private Sub ExportList()
        Dim EntityTable As String = ""
        Dim EntityTableAccount As String = ""
        Dim EntityTableId As String = ""
        Dim EntityRegistration As String = ""
        Dim SQL As String = ""
        Dim WhereClause As String = ""
        Dim JoinClause As String = ""
        Dim OrderByClause As String = ""

        'select the correct entity table and 
        Select Case F_MemberType.SelectedValue
            Case "Builder"
                EntityTable = F_MemberType.SelectedValue
                EntityTableAccount = F_MemberType.SelectedValue & "Account"
                EntityTableId = F_MemberType.SelectedValue & "ID"
                EntityRegistration = F_MemberType.SelectedValue & "Registration"
            Case "Vendor"
                EntityTable = F_MemberType.SelectedValue
                EntityTableAccount = F_MemberType.SelectedValue & "Account"
                EntityTableId = F_MemberType.SelectedValue & "ID"
                EntityRegistration = F_MemberType.SelectedValue & "Registration"
            Case "PIQ"
                EntityTable = F_MemberType.SelectedValue
                EntityTableAccount = F_MemberType.SelectedValue & "Account"
                EntityTableId = F_MemberType.SelectedValue & "ID"
                EntityRegistration = F_MemberType.SelectedValue & "Registration"
            Case Else
                Exit Sub
        End Select

        OrderByClause = "order by CompanyName "


        If F_MemberType.SelectedValue = "Builder" Then
            OrderByClause = " order by LLC ,CompanyName "
        ElseIf F_MemberType.SelectedValue = "Vendor" Then
            OrderByClause = " order by CompanyName "
        End If



        SQL = "SELECT" & vbCrLf
        SQL = SQL & " Row_Number() Over( " & OrderByClause & ") as Rank, et." & EntityTableId & " ID, et.CompanyName, et.WebsiteURL, et.Address, et.Address2, et.City, et.State, et.Zip, et.Phone, et.Fax "

        If F_MemberType.SelectedValue = "Vendor" Then
            SQL = SQL & "  , et.LogoFile as LogoFile ,  STUFF((SELECT ','+ a.LLC  FROM LLC a JOIN LLCVendor  ar ON ar.LLCID   =  a.LLCID   WHERE a.isactive =1 AND  ar.VendorID    = et.VendorID order by a.LLC  FOR XML PATH(''), TYPE).value('.','VARCHAR(max)'), 1, 1, '') AS LLC " & vbCrLf
        ElseIf F_MemberType.SelectedValue = "Builder" Then
            SQL = SQL & "  , NULL as LogoFile , LLC " & vbCrLf
        Else
            SQL = SQL & "  , NULL as LogoFile" & vbCrLf
        End If

        SQL &= "FROM" & vbCrLf

        JoinClause = EntityTable & " et " & vbCrLf


        If F_MemberType.SelectedValue = "Builder" Then
            JoinClause &= " LEFT JOIN LLC on LLC.LLCID = et.LLCID "
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " LLC.Isactive =1 "
        End If
        If LoggedInType = "piq" AndAlso PIQAccountRow.GetLLCCount(DB, Session("PiqAccountId")) > 0 Then
            If F_MemberType.SelectedValue = "Vendor" Then
                If WhereClause <> String.Empty Then WhereClause &= " AND "
                WhereClause &= " et.VendorId in (select VendorId from LLCVendor lv inner join PiqAccountLLC lp on lv.LLCId=lp.LLCId where lp.PiqAccountId=" & DB.Number(Session("PiqAccountId")) & ")"
            ElseIf F_MemberType.SelectedValue = "Builder" Then
                If WhereClause <> String.Empty Then WhereClause &= " AND "
                WhereClause &= " et.LLCId in (Select LLCID from LLC where isactive =1)"
                If WhereClause <> String.Empty Then WhereClause &= " AND "
                WhereClause &= " et.LLCId in (select LLCId from PiqAccountLLC where PiqAccountId=" & DB.Number(Session("PiqAccountId")) & ")"
            End If
        End If

        'join clause is changed by the supply phase
        If F_MemberType.SelectedValue = "Vendor" AndAlso F_SupplyPhase.SelectedValue <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from VendorCategoryVendor where VendorCategoryID = " & DB.Number(F_SupplyPhase.SelectedValue) & ")" & vbCrLf
        End If

        SQL = SQL & JoinClause



        If F_ContactFirstName.Text <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from " & EntityTableAccount & " where FirstName LIKE " & DB.FilterQuote(F_ContactFirstName.Text) & ")" & vbCrLf
        End If

        If F_ContactLastName.Text <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from " & EntityTableAccount & " where LastName LIKE " & DB.FilterQuote(F_ContactLastName.Text) & ")" & vbCrLf
        End If

        If F_CompanyName.Text <> "" Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et.CompanyName LIKE " & DB.FilterQuote(F_CompanyName.Text) & vbCrLf
        End If

        If F_MemberType.SelectedValue = "Builder" AndAlso Not Session("BuilderId") Is Nothing Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et.LLCID = " & DB.Number(LLCID) & vbCrLf
        ElseIf F_MemberType.SelectedValue = "Builder" AndAlso Not Session("VendorID") Is Nothing Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et.LLCID in (select LLCID from LLCVendor where VendorID=" & DB.Number(Session("VendorID")) & ")"
        End If

        If F_MemberType.SelectedValue = "Vendor" AndAlso Not Session("BuilderId") Is Nothing Then
            If WhereClause <> "" Then WhereClause = WhereClause & " AND "
            WhereClause = WhereClause & " et." & EntityTableId & " in (select " & EntityTableId & " from LLCVendor where IsActive = 1 and LLCID = " & DB.Number(LLCID) & ")" & vbCrLf
        End If

        If WhereClause <> "" Then WhereClause = WhereClause & " AND "
        WhereClause = WhereClause & " et.IsActive = 1"

        'add where clause to the main SQL string
        If WhereClause <> "" Then
            SQL = SQL & " WHERE " & vbCrLf & WhereClause & vbCrLf
        End If

        Dim curPage As Integer = 1
        Dim perPage As Integer = 5000

        SQL = "select top " & perPage & " * from (" & SQL & ") as tmp where Rank > " & DB.Number(perPage * (curPage - 1))

        ' order by clause, sort by company name
        SQL = SQL & " ORDER BY Rank " & vbCrLf

        Dim dt As DataTable = DB.GetDataTable(SQL)

        If F_MemberType.SelectedValue = "Vendor" Then
            Dim dataView As New DataView(dt)
            dataView.Sort = " LLC, companyname"
            dt = dataView.ToTable()
        End If
        

        Dim dv As DataView = dt.DefaultView 

        Dim Folder As String = "/assets/directories/"
        Dim FolderPath As String = Server.MapPath(Folder)
        Dim i As Integer = 0
        Dim FileName As String = Core.GenerateFileID & ".csv"

        Dim sw As StreamWriter = New StreamWriter((FolderPath & FileName), False)

        sw.WriteLine("CBUSA Directory")
        sw.WriteLine("Report generated on " & DateTime.Now.ToString("d"))
        sw.WriteLine(String.Empty)

        If F_MemberType.SelectedValue <> String.Empty Then
            sw.WriteLine("Member Type," & F_MemberType.SelectedValue)
        End If
        If F_CompanyName.Text <> String.Empty Then
            sw.WriteLine("Company Name," & F_CompanyName.Text)
        End If
        If F_ContactFirstName.Text <> String.Empty Then
            sw.WriteLine("Contact First Name," & F_ContactFirstName.Text)
        End If
        If F_ContactLastName.Text <> String.Empty Then
            sw.WriteLine("Contact Last Name," & F_ContactLastName.Text)
        End If
        If F_SupplyPhase.SelectedValue <> String.Empty Then
            Dim dbSupplyPhase As VendorCategoryRow = VendorCategoryRow.GetRow(DB, Convert.ToInt32(F_SupplyPhase.SelectedValue))
            If Not dbSupplyPhase Is Nothing Then
                sw.WriteLine("Supply Phase," & dbSupplyPhase.Category)
            End If
        End If
        sw.WriteLine(String.Empty)

        If F_MemberType.SelectedValue = "Vendor" Then
            sw.WriteLine("Markets, Company Name,Website URL,Address 1,Address 2,City,State,Zip,Supply Phases,Primary Contact First Name,Primary Contact Last Name,Title,Roles,Email,Phone,Mobile,Fax")
        ElseIf F_MemberType.SelectedValue = "Builder" Then
            sw.WriteLine("Markets, Company Name,Website URL,Address 1,Address 2,City,State,Zip,Primary Contact First Name,Primary Contact Last Name,Title,Email,Phone,Mobile,Fax")
        Else
            sw.WriteLine("Company Name,Website URL,Address 1,Address 2,City,State,Zip,Primary Contact First Name,Primary Contact Last Name,Title,Email,Phone,Mobile,Fax")
        End If
        If dv.Count > 0 Then
            For jLoop As Integer = 0 To (dv.Count - 1)
                Dim WebsiteURL As String = String.Empty
                Dim Markets As String = String.Empty
                If Not Core.GetString(dv(jLoop)("WebsiteURL")) Is Nothing Then
                    WebsiteURL = Core.GetString(dv(jLoop)("WebsiteURL")).Trim
                End If
                If Not WebsiteURL.StartsWith("http") And WebsiteURL <> String.Empty Then
                    WebsiteURL = "http://" & WebsiteURL
                End If
                If F_MemberType.SelectedValue = "Builder" Then
                    If Not Core.GetString(dv(jLoop)("LLC")) Is Nothing Then
                        Markets = Core.GetString(dv(jLoop)("LLC")).Trim
                    End If
                ElseIf F_MemberType.SelectedValue = "Vendor" Then
                    If Not Core.GetString(dv(jLoop)("LLC")) Is Nothing Then
                        Markets = Core.GetString(dv(jLoop)("LLC")).Trim
                    End If


                End If

                Dim dtContacts As DataTable
                Dim sSQL As String = String.Empty
                Select Case F_MemberType.SelectedValue
                    Case "Builder"
                        'sSQL = "SELECT ba.FirstName, ba.LastName, ba.Title, ba.Email, ba.Phone, ba.Mobile, ba.Fax FROM BuilderAccount AS ba INNER JOIN Builder AS bldr ON ba.BuilderID = bldr.BuilderID WHERE ba.IsPrimary = 1 AND ba.IsActive = 1 AND ba.BuilderID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY ba.FirstName ASC, ba.LastName ASC "
                        sSQL = "SELECT Distinct ba.FirstName, ba.LastName, ba.Title, ba.Email, ba.Phone, ba.Mobile, ba.Fax FROM BuilderAccount AS ba INNER JOIN Builder AS bldr ON ba.BuilderID = bldr.BuilderID WHERE ba.IsActive = 1 AND ba.BuilderID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY ba.FirstName ASC, ba.LastName ASC "
                    Case "Vendor"
                        'Dim VendorRoleId As Integer
                        'VendorRoleId = DB.ExecuteScalar("SELECT Top 1 VendorRoleId FROM VendorRole WHERE VendorRole = 'Primary Contact'")
                        'sSQL = "SELECT Top 1 va.VendorAccountID, va.FirstName, va.LastName, va.Title, va.Email, va.Phone, va.Mobile, va.Fax FROM VendorAccount AS va INNER JOIN Vendor AS vndr ON va.VendorID = vndr.VendorID INNER JOIN VendorAccountVendorRole AS vavr ON vavr.VendorAccountId = va.VendorAccountId WHERE va.IsActive = 1 AND va.VendorID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " AND vavr.VendorRoleID = " & DB.Number(VendorRoleId) & " ORDER BY va.FirstName ASC, va.LastName ASC"
                        sSQL = "SELECT Distinct va.VendorAccountID, va.FirstName, va.LastName, va.Title, va.Email, va.Phone, va.Mobile, va.Fax FROM VendorAccount AS va INNER JOIN Vendor AS vndr ON va.VendorID = vndr.VendorID Left Outer JOIN VendorAccountVendorRole AS vavr ON vavr.VendorAccountId = va.VendorAccountId WHERE va.IsActive = 1 AND va.VendorID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY va.FirstName ASC, va.LastName ASC"
                    Case "PIQ"
                        'sSQL = "SELECT Top 1 pa.FirstName, pa.LastName, NULL AS Title, piq.Email, piq.Phone, piq.Mobile, piq.Fax FROM PIQAccount AS pa INNER JOIN PIQ AS piq ON pa.PIQID = piq.PIQID WHERE pa.IsPrimary = 1 AND piq.IsActive = 1 AND pa.PIQID=" & DB.Number(Core.GetInt(dv(jLoop)("ID"))) & " ORDER BY newid() "
                        sSQL = "SELECT Distinct pa.FirstName, pa.LastName, NULL AS Title, pa.Email, piq.Phone, piq.Mobile, piq.Fax FROM PIQAccount AS pa INNER JOIN PIQ AS piq ON pa.PIQID = piq.PIQID WHERE piq.IsActive = 1 AND pa.PIQID=" & DB.Number(Core.GetInt(dv(jLoop)("ID")))
                        If LoggedInType = "builder" Then
                            sSQL &= " and pa.PiqAccountId in (select PiqAccountId from PiqAccountLLC pal inner join Builder b on pal.LLCId=b.LLCId where b.BuilderId=" & DB.Number(Session("BuilderId"))
                        ElseIf LoggedInType = "vendor" Then
                            sSQL &= " and pa.PiqAccountId in (select PiqAccountId from PiqAccountLLC pal inner join LLCVendor lv on pal.LLCId=pv.LLCId where LLCVendor.VendorId=" & DB.Number(Session("VendorId"))
                        End If

                End Select
                dtContacts = DB.GetDataTable(sSQL)

                If dtContacts.Rows.Count = 0 AndAlso F_MemberType.SelectedValue = "PIQ" Then
                    sSQL = "SELECT Distinct pa.FirstName, pa.LastName, NULL AS Title, pa.Email, piq.Phone, piq.Mobile, piq.Fax FROM PIQAccount AS pa INNER JOIN PIQ AS piq ON pa.PIQID = piq.PIQID WHERE piq.IsActive = 1 AND pa.PIQID=" & DB.Number(Core.GetInt(dv(jLoop)("ID")))
                    dtContacts = DB.GetDataTable(sSQL)
                End If

                For Each v As DataRow In dtContacts.Rows
                    Dim FirstName As String = String.Empty
                    Dim LastName As String = String.Empty
                    Dim Title As String = String.Empty
                    Dim sVendorAccountRoles As String = String.Empty
                    Dim sVendorSupplyPhases As String = String.Empty
                    Dim Email As String = String.Empty
                    Dim Phone As String = String.Empty
                    Dim Mobile As String = String.Empty
                    Dim Fax As String = String.Empty

                    FirstName = Core.GetString(v("FirstName"))
                    LastName = Core.GetString(v("LastName"))
                    Title = Core.GetString(v("Title"))
                    If F_MemberType.SelectedValue = "Vendor" Then
                        Dim dbVendorAccount As VendorAccountRow
                        dbVendorAccount = VendorAccountRow.GetRow(DB, v("VendorAccountID"))
                        sVendorAccountRoles = dbVendorAccount.GetSelectedVendorRoleLabels()
                        sVendorSupplyPhases = VendorCategoryRow.GetNames(DB, VendorRow.GetRow(DB, Core.GetInt(dv(jLoop)("ID"))).GetSelectedVendorCategories, ",")

                        ' Dim dtLLC As DataTable = DB.GetDataTable("select * from LLCVendor  LV INNER JOIN LLC ON LLC.LLCID=LV.LLCID WHERE LV.VendorID = " & Core.GetInt(dv(jLoop)("ID")) & "ORDER BY LLC")


                    End If
                    Email = Core.GetString(v("Email"))
                    Phone = Core.GetString(v("Phone"))
                    Mobile = Core.GetString(v("Mobile"))
                    Fax = Core.GetString(v("Fax"))



                    If F_MemberType.SelectedValue = "Vendor" Then
                        sw.WriteLine(Core.QuoteCSV(Markets) & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("CompanyName"))) _
                                     & "," & Core.QuoteCSV(WebsiteURL) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address2"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("City"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("State"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Zip"))) _
                                     & "," & Core.QuoteCSV(sVendorSupplyPhases) _
                                     & "," & Core.QuoteCSV(FirstName) _
                                     & "," & Core.QuoteCSV(LastName) _
                                     & "," & Core.QuoteCSV(Title) _
                                     & "," & Core.QuoteCSV(sVendorAccountRoles) _
                                     & "," & Core.QuoteCSV(Email) _
                                     & "," & Core.QuoteCSV(Phone) _
                                     & "," & Core.QuoteCSV(Mobile) _
                                     & "," & Core.QuoteCSV(Fax))
                    ElseIf F_MemberType.SelectedValue = "Builder" Then
                        sw.WriteLine(Core.QuoteCSV(Markets) & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("CompanyName"))) _
                                     & "," & Core.QuoteCSV(WebsiteURL) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address2"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("City"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("State"))) _
                                     & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Zip"))) _
                                     & "," & Core.QuoteCSV(FirstName) _
                                     & "," & Core.QuoteCSV(LastName) _
                                     & "," & Core.QuoteCSV(Title) _
                                     & "," & Core.QuoteCSV(Email) _
                                     & "," & Core.QuoteCSV(Phone) _
                                     & "," & Core.QuoteCSV(Mobile) _
                                     & "," & Core.QuoteCSV(Fax))
                    Else
                        sw.WriteLine(Core.QuoteCSV(Core.GetString(dv(jLoop)("CompanyName"))) _
                                   & "," & Core.QuoteCSV(WebsiteURL) _
                                   & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address"))) _
                                   & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Address2"))) _
                                   & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("City"))) _
                                   & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("State"))) _
                                   & "," & Core.QuoteCSV(Core.GetString(dv(jLoop)("Zip"))) _
                                   & "," & Core.QuoteCSV(FirstName) _
                                   & "," & Core.QuoteCSV(LastName) _
                                   & "," & Core.QuoteCSV(Title) _
                                   & "," & Core.QuoteCSV(Email) _
                                   & "," & Core.QuoteCSV(Phone) _
                                   & "," & Core.QuoteCSV(Mobile) _
                                   & "," & Core.QuoteCSV(Fax))
                    End If
                Next
            Next
        Else
            sw.WriteLine("There are no results that match your search criteria.")
        End If

        sw.Flush()
        sw.Close()
        sw.Dispose()

        Response.Redirect(Folder & FileName)
    End Sub

    Protected Sub btnClose_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btnClose.Click
        divNotification.Visible = False
    End Sub

End Class
