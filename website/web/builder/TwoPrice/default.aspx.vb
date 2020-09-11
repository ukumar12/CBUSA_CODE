Imports Components
Imports DataLayer
Imports TwoPrice.DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Partial Class takeoffs_default
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private dbTakeoff As TakeOffRow

    Private m_dtComparisons As DataTable
    Private ReadOnly Property dtComparisons() As DataTable
        Get
            If m_dtComparisons Is Nothing Then
                m_dtComparisons = PriceComparisonRow.GetSavedComparisons(DB, Session("BuilderId"))
            End If
            Return m_dtComparisons
        End Get
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        dbTakeoff = TakeOffRow.GetRow(DB, Session("CurrentTakeoffId"))
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        gvList.BindList = AddressOf BindData


        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        If Not IsPostBack Then
            Core.DataLog("Committed Purchase", PageURL, CurrentUserId, "Builder Top Menu Click", "", "", "", "", UserName)

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "Saved"
                gvList.SortOrder = "Desc"
            End If

            ' BindData()
            ViewState("F_SortBy") = gvList.SortBy
            ViewState("F_SortOrder") = gvList.SortOrder

            gvList.DataKeyNames = New String() {"TwoPriceTakeoffId"}

            gvList.AllowPaging = False
            gvList.DataSource = GetAwardedEvents().DefaultView
            gvList.DataBind()
            BindPendingCpData()
        End If

    End Sub
    Private Function GetAwardedEvents() As DataTable
        Dim dt As DataTable = New DataTable
        Dim con As SqlConnection = New SqlConnection(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        Dim sda As SqlDataAdapter = New SqlDataAdapter
        '       Dim strQuery As String = (
        '               "SELECT tpc.Name, tpc.StartDate,tpc.EndDate,tpc.TwoPriceCampaignId ,'Participating (Legacy)' as DisplayValue,'5' as TwoPriceBuilderParticipationTypeId,tpbv.BuilderId,tpto.TwoPriceTakeOffID " _
        '               & "From TwoPriceCampaign tpc " _
        '               & "Join TwoPriceTakeOff tpto ON tpc.TwoPriceCampaignId = tpto.TwoPriceCampaignId " _
        '               & "Join TwoPriceCampaignBuilder_Rel tpbv ON tpc.TwoPriceCampaignId = tpbv.TwoPriceCampaignId " _
        '               & "Where tpc.Status = 'Awarded' AND  tpbv.BuilderId = " & Session("BuilderId") & " and " _
        '               & " tpc.IsActive = 1 AND tpc.StartDate <= GETDATE() AND tpc.EndDate >= GETDATE() " _
        '               & " AND tpc.TwoPriceCampaignId NOT IN (SELECT TwoPriceCampaignId from twopricebuilderinvitation) " _
        '               & " UNION" _
        '               & " Select t2.name, t2.startdate,t2.enddate,t1.TwoPriceCampaignId,t3.DisplayValue,t3.TwoPriceBuilderParticipationTypeId ,t1.BuilderId ,tpto.TwoPriceTakeOffID " _
        '               & "From TwoPriceBuilderParticipation t1 " _
        '               & "Join twopricecampaign t2 on t1.TwoPriceCampaignId = t2.TwoPriceCampaignId " _
        '               & "JOIN TwoPriceTakeOff tpto ON t2.TwoPriceCampaignId = tpto.TwoPriceCampaignId " _
        '               & "Join TwoPriceBuilderParticipationType t3 on t1.ParticipationType = t3.TwoPriceBuilderParticipationTypeid " _
        '               & " Join twopricebuilderinvitation t4 on t4.TwoPriceCampaignId = t2.TwoPriceCampaignId  " _
        '               & "Where t1.builderid = " & Session("BuilderId") & " And " _
        '               & "t2.Status ='Awarded' and t4.invitationstatus=1  AND t2.IsActive=1 AND  t2.StartDate <= GETDATE() AND t2.EndDate >= GETDATE() " _
        '               & " UNION " _
        '                & "Select tp.name, tp.startdate, tp.enddate,tp.TwoPriceCampaignId, 'Participation Not Specified' AS DisplayValue, 4 as TwoPriceBuilderParticipationTypeId," _
        '               & " TPLLC.BuilderID, tpto.TwoPriceTakeOffID as TwoPriceTakeOffID  FROM TwoPriceCampaign tp  Join TwoPriceTakeOff tpto ON tp.TwoPriceCampaignId = tpto.TwoPriceCampaignId" _
        '& " Join TwoPriceCampaignBuilder_Rel TPLLC on TPLLC.TwoPriceCampaignId = tp.TwoPriceCampaignId     Join " _
        ' & " twopricebuilderinvitation TPBI on TPBI.TwoPriceCampaignId = tp.TwoPriceCampaignId  where tp.Status ='Awarded'" _
        '  & " And  tp.isactive = 1  And tp.StartDate <= GETDATE() And tp.EndDate >= GETDATE()  And " & Session("BuilderId") & " Not IN" _
        '  & " (select builderid from TwoPriceBuilderParticipation where TwoPriceCampaignId = tp.TwoPriceCampaignId)  And TPLLC.BuilderID = " & Session("BuilderId") & ""
        '       )
        'query changed for upgrade the awarded list regardless of invitationstatus.
        'AND TPBI.invitationstatus=1 and t4.invitationstatus=1

        Dim strQuery As String = String.Concat("EXEC sp_GetBuilderAwardedCPEvents ", Session("BuilderId"))
        Dim cmd As SqlCommand = New SqlCommand(strQuery)
        cmd.Connection = con
        con.Open()
        sda.SelectCommand = cmd
        sda.Fill(dt)

        Return dt

    End Function
    Private Sub BindData()
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        gvList.DataKeyNames = New String() {"TwoPriceTakeoffId"}

        gvList.AllowPaging = False


        'Dim res As DataTable = TwoPriceCampaignRow.GetTwoPriceTakeOffList(DB, Status:="Awarded", BuilderId:=Session("BuilderId"), RestrictByDate:=True, GetActiveOnly:=True)
        Dim res As DataTable = GetAwardedEvents()
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        Select Case e.CommandName
            Case "ChangeTakeoff"
                Response.Redirect("edit.aspx?TwoPriceTakeOffId=" & DB.Number(e.CommandArgument))
        End Select
    End Sub
    Protected Sub gvList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub

        Dim lnkCommitted As LinkButton = e.Row.FindControl("lnkCommitted")

        lnkCommitted.Text = e.Row.DataItem("Name")
        lnkCommitted.CommandArgument = e.Row.DataItem("TwoPriceTakeoffID")

    End Sub
    Private Sub BindPendingCpData()
        Dim objDB As New Database
        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        Dim strQuery As String = ("Select Name,StartDate,EndDate,CreatedOn,isnull(Deadline,'') Deadline,DisplayValue,TwoPriceCampaignId" + " From TwoPriceBuilderParticipation")
        Dim cmd As SqlCommand = New SqlCommand(strQuery)
        gvpendinglist.DataSource = GetPendingCpData()
        gvpendinglist.DataBind()
    End Sub
    Private Function GetPendingCpData() As DataTable
        Dim dt As DataTable = New DataTable
        Dim con As SqlConnection = New SqlConnection(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        Dim sda As SqlDataAdapter = New SqlDataAdapter
        'Dim strQuery As String = ("select t2.name,t2.startdate,t2.enddate,t1.CreatedOn,t3.DisplayValue,t1.TwoPriceCampaignId from TwoPriceBuilderParticipation t1 join twopricecampaign t2 on t1.TwoPriceCampaignId = t2.TwoPriceCampaignId join TwoPriceBuilderParticipationType t3 on t1.ParticipationType = t3.TwoPriceBuilderParticipationTypeid")
        Dim strQuery As String = "select distinct tc.name,tc.startDate,tc.EndDate,tbp.CreatedOn,convert(varchar,isnull(ti.ResponseDeadline,''),101) ResponseDeadline,                                   " &
"isnull(tbpt.DisplayValue,'Participation Not Specified') DisplayValue, ti.TwoPriceCampaignId,tbpt.TwoPriceBuilderParticipationTypeId,                                                                    " &
" CASE WHEN cast(ti.ResponseDeadline As Date) < cast(GETDATE() As Date) Then 0 Else 1 End As EnableLink,                                                                                                  " &
" isnull(AdminPermissionForPostDeadLine,0)  AdminPermissionForPostDeadLine                                                                                                 " &
" FROM TwoPriceCampaign tc                                                                                                                                                                         " &
" join twopricebuilderinvitation    ti              on          tc.twopricecampaignId       = ti.twopricecampaignId and ti.InvitationStatus = 1     " &
" join TwoPriceCampaignLLC_Rel      tpLLC           on          tpllc.twopricecampaignId    = ti.twopricecampaignId                                 " &
" Join Builder                      b               on          tpllc.LLCId                 = b.LLcId left " &
" join TwoPriceBuilderParticipation tbp             on          ti.twopricecampaignId       = tbp.twopricecampaignId And  b.BuilderId = tbp.BuilderId                                                                                  " &
" Left Join TwoPriceBuilderParticipationType tbpt   on          tbp.participationType       = tbpt.twopricebuilderparticipationTypeId " &
" where tc.Status != 'Awarded' and b.BuilderId = " & Session("BuilderId") & " and tc.isactive =1  " &
" and cast(tc.enddate As Date) > cast(getdate() As Date)"

        ' commented and  for resolve pending CP event list issue
        Dim cmd As SqlCommand = New SqlCommand(strQuery)
        cmd.Connection = con
        con.Open()
        sda.SelectCommand = cmd
        sda.Fill(dt)
        Return dt

    End Function

    Private Sub gvpendinglist_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvpendinglist.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub

        Dim lnkTPCampaign As LinkButton = e.Row.FindControl("lnkbtnCPParticipation")

        Dim hdnAdminPermissionForPostDeadLine As HiddenField = e.Row.FindControl("hdnAdminPermissionForPostDeadLine")


        'log and auditing
        Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Open Event List", e.Row.DataItem("TwoPriceCampaignId"), "", "", "", UserName, "CBUSA_CPM")
        'end logging
        lnkTPCampaign.Enabled = False
        'lnkTPCampaign.PostBackUrl = "/builder/CpEvent/DataEntry.aspx?Tcam=" & e.Row.DataItem("TwoPriceCampaignId")
        Dim Participation As String = e.Row.DataItem("DisplayValue")
        If Participation = "Participation Not Specified" Then
            e.Row.ForeColor = Color.Red
        ElseIf Participation = "Not Participating" Then
            e.Row.ForeColor = Color.Gray
        ElseIf Participation.ToString().ToLower() = "participate with specific projects" And DateTime.Now.Date > DateTime.Parse(e.Row.DataItem("ResponseDeadline")).Date And Convert.ToBoolean(hdnAdminPermissionForPostDeadLine.Value) = False Then
            lnkTPCampaign.Enabled = True


            lnkTPCampaign.CommandName = "OpenPopUp"

            'AddHandler lnkTPCampaign.Command, AddressOf Me.GetPendingProjectDetails


        End If
        If DateTime.Parse(e.Row.DataItem("ResponseDeadline")).Date >= DateTime.Now.Date Then
            lnkTPCampaign.Enabled = True
            lnkTPCampaign.CommandName = "OpenEditPage"
        ElseIf DateTime.Parse(e.Row.DataItem("ResponseDeadline")).Date < DateTime.Now.Date And Convert.ToBoolean(hdnAdminPermissionForPostDeadLine.Value) = True Then
            lnkTPCampaign.Enabled = True
            lnkTPCampaign.CommandName = "OpenEditPage"
        End If
        lnkTPCampaign.CommandArgument = e.Row.DataItem("TwoPriceCampaignId")

    End Sub

    Private Function GetPtfData(CampaignId As String) As DataTable
        Dim dt As DataTable = New DataTable
        Dim con As SqlConnection = New SqlConnection(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        Dim sda As SqlDataAdapter = New SqlDataAdapter
        Dim strQuery As String = ("Select TwoPriceBuilderProject.BuilderProjectId, TwoPriceBuilderProject.Community, TwoPriceBuilderProject.ProjectName, TwoPriceBuilderProject.FloorPlan, TwoPriceBuilderProjectStatus.DisplayValue, TwoPriceBuilderProjectStatus.TwoPriceBuilderProjectStatusId from TwoPriceBuilderProject join TwoPriceBuilderProjectStatus On TwoPriceBuilderProject.Status = TwoPriceBuilderProjectStatus.TwoPriceBuilderProjectStatusId  where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID") & " And ConstructionType=1 ")
        Dim cmd As SqlCommand = New SqlCommand(strQuery)
        cmd.Connection = con
        con.Open()
        sda.SelectCommand = cmd
        sda.Fill(dt)
        Return dt
    End Function
    Protected Sub GetProjectDetails(ByVal sender As Object, ByVal e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        Dim TwoPriceCampaignId As Integer = CType(sender, LinkButton).CommandArgument
        Dim CommandName As String = CType(sender, LinkButton).CommandName
        If CommandName.ToString().ToLower() = "openeditpage" Then
            Response.Redirect("/builder/CpEvent/DataEntry.aspx?Tcam=" & DB.Number(TwoPriceCampaignId))
        ElseIf CommandName.ToString().ToLower() = "openpopup" Then
            AddPtfProject.Attributes.Add("style", "display:block")


            'TwoPriceCampaignId = 442
            Dim CampaignType As Integer = DB.ExecuteScalar("SELECT TOP 1 ISNULL(Id,0)  FROM TwoPriceProjectData WHERE TwoPriceCampaignId =" & TwoPriceCampaignId & "")
            If CampaignType > 0 Then
                Dim dt As New DataTable

                Dim strQuery As String = "exec Proc_TwoPriceCampaignProjectData_horizontal " & TwoPriceCampaignId & ", " & Session("BuilderID") & ""
                dt = DB.GetDataTable(strQuery)
                grdProjList.DataSource = dt
                grdProjList.DataBind()
                OldStyleProjects.Attributes.Add("style", "display:none;")
                NewStyleProjects.Attributes.Add("style", "display:block;")
            Else

                NewStyleProjects.Attributes.Add("style", "display:none;")
                OldStyleProjects.Attributes.Add("style", "display:block;")
                gvPtfList.DataSource = GetPtfData(TwoPriceCampaignId)
                gvPtfList.DataBind()

                gvCustList.DataSource = GetCustData(TwoPriceCampaignId)
                gvCustList.DataBind()

            End If

        End If


    End Sub

    Private Function GetCustData(CampaignId As String) As DataTable
        Dim dt As DataTable = New DataTable
        Dim con As SqlConnection = New SqlConnection(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        Dim sda As SqlDataAdapter = New SqlDataAdapter
        'Dim strQuery As String = ("Select TwoPriceBuilderProject.BuilderProjectId, TwoPriceBuilderProject.ProjectName, CASE TwoPriceBuilderProject.TakeOffInSystem WHEN 0 THEN 'No' ELSE 'Yes' END AS TakeOffInSystem, TwoPriceBuilderProject.SquareFeet, TwoPriceBuilderProject.Stories, TwoPriceBuilderProject.SpecialMaterials, TwoPriceBuilderProjectStatus.DisplayValue,TwoPriceBuilderProjectStatus.TwoPriceBuilderProjectStatusId from TwoPriceBuilderProject join TwoPriceBuilderProjectStatus on TwoPriceBuilderProject.Status = TwoPriceBuilderProjectStatus.TwoPriceBuilderProjectStatusId where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID") & " and ConstructionType=2 ")
        Dim strQuery As String = ("Select t1.BuilderProjectId, t1.ProjectName, CASE t1.TakeOffInSystem WHEN 0  THEN 'No' when 1 then 'Yes' ELSE '' END AS TakeOffInSystem, t1.SquareFeet, t1.Stories, t1.SpecialMaterials, t2.DisplayValue,t2.TwoPriceBuilderProjectStatusId from TwoPriceBuilderProject t1 join TwoPriceBuilderProjectStatus t2 on t1.Status = t2.TwoPriceBuilderProjectStatusId  where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID") & " and ConstructionType=2 ")
        Dim cmd As SqlCommand = New SqlCommand(strQuery)
        cmd.Connection = con
        con.Open()
        sda.SelectCommand = cmd
        sda.Fill(dt)
        Return dt
    End Function

    Protected Sub grdProjList_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.Cells.Count > 3 Then
            e.Row.Cells(0).Visible = False
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
        End If
    End Sub
End Class
