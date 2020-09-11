Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.Linq
Imports System.Data
Imports System.IO
Imports Controls
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Collections.Generic
Imports System.Web.Services
Imports Utility
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Windows.Forms
Imports System.Net.Mail

Partial Class BuilderDataEntry
    Inherits SitePage
    Protected CampaignId As String
    Protected Takeoffvalue As String
    Protected VarOpt As String
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not (IsLoggedInBuilder() Or Session("AdminId") IsNot Nothing) Then
            Response.Redirect("/default.aspx")
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        CampaignId = Request.QueryString("Tcam")
        VarOpt = Request.QueryString("Opt")
        HdnParticipationType.Value = DB.ExecuteScalar("Select ParticipationType from TwoPriceBuilderParticipation where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        Dim PurchaseSql As String = DB.ExecuteScalar("select * from TwoPriceBuilderProject where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        If PurchaseSql IsNot Nothing Then
            HdnPurchases.Value = "True"
        Else
            HdnPurchases.Value = "False"
        End If

        If Not IsPostBack Then

            Dim BuilderEventSql As String = " Select TwoPriceCampaignId, Name from TwoPriceCampaign where IsActive=1 And TwoPriceCampaignId in ( " &
                                        "    SELECT distinct TwoPriceCampaignId from TwoPriceBuilderProjectData WHERE TwoPriceCampaignId <> " & CampaignId & " AND BuilderId=" & Session("BuilderID") & ")"
            ddlBuilderEvents.DataSource = DB.GetDataTable(BuilderEventSql)
            ddlBuilderEvents.DataTextField = "Name"
            ddlBuilderEvents.DataValueField = "TwoPriceCampaignId"
            ddlBuilderEvents.DataBind()
            ddlBuilderEvents.Items.Insert(0, "-Select-")

            PopulateProjectCopyNumericDropdowns()

            Me.BindPtfData()
            GetTwoPriceBuilderInvitation()
            LoadRblCheck()

            Dim strTwoPriceCampaignPrticipationOptions As String = DB.ExecuteScalar("SELECT ParticipationOptions FROM TwoPriceCampaign WHERE TwoPriceCampaignId = " & CampaignId)

            Dim arrTwoPriceCampaignPrticipationOptions() As String = strTwoPriceCampaignPrticipationOptions.Split(",")

            liSpecificProjects.Attributes.Add("style", "display:none")
            liNoProjects.Attributes.Add("style", "display:none")
            liNonParticipant.Attributes.Add("style", "display:none")
            If arrTwoPriceCampaignPrticipationOptions.Length > 0 Then
                If arrTwoPriceCampaignPrticipationOptions.Contains("0") Then
                    liSpecificProjects.Attributes.Add("style", "display:block")
                End If
                If arrTwoPriceCampaignPrticipationOptions.Contains("1") Then
                    liNoProjects.Attributes.Add("style", "display:block")
                End If
                If arrTwoPriceCampaignPrticipationOptions.Contains("2") Then
                    liNonParticipant.Attributes.Add("style", "display:block")
                End If
            End If

            Dim ParticipationType As String = DB.ExecuteScalar("select ParticipationType from TwoPriceBuilderParticipation where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
            If Deadline.InnerHtml <> "" Then
                Dim currentDate As Date = Now.Date
                Dim CPEventDeadline As Date = Convert.ToDateTime(Deadline.InnerHtml)

                Dim dd As String = Now.ToShortDateString
                If CPEventDeadline.Date < currentDate.Date Then                        'for what hppens after deadline 
                    spnDeadLine.Attributes.Add("style", "display:none")
                    If ParticipationType = 3 Then 'if optout option is selected
                        RblSpecificProjects.Enabled = False

                        RblNoProjects.Enabled = False
                    ElseIf (ParticipationType = 1) Or (ParticipationType = 2) Then 'if any optin is selected
                        RblNonParticipant.Enabled = False
                        If (ParticipationType = 1) And (PurchaseSql IsNot Nothing) Then 'if optin is selctd and purchase is specified i.e 1st optin
                            RblNoProjects.Enabled = False
                        End If
                        If (ParticipationType = 2) And (PurchaseSql Is Nothing) Then
                            RblNoProjects.Enabled = False
                        End If
                    ElseIf ParticipationType Is Nothing Then
                        RblSpecificProjects.Enabled = False

                        RblNoProjects.Enabled = False
                        RblNonParticipant.Enabled = False
                    End If
                    If VarOpt IsNot Nothing Then                '---if dealine is crossed and someone clicked on optin link in mail
                        Dim ChkSessionBuilderSql As String = DB.ExecuteScalar("Select bldr.builderid from builder bldr join twopricecampaignllc_rel llcrel on bldr.llcid = llcrel.llcid " _
                        & " where llcrel.twopricecampaignid =  " & CampaignId & " And builderid = " & Session("BuilderID"))
                        If ChkSessionBuilderSql = Session("BuilderID") Then

                            If HdnParticipationType.Value = "" Then
                                divPtf.Attributes.Add("style", "display:none")
                                RblNoProjects.Checked = False
                                RblSpecificProjects.Checked = False
                                RblNonParticipant.Checked = False
                            Else
                                If HdnParticipationType.Value = 1 Then
                                    Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Enroll Invitation", CampaignId, "", "", "", UserName, "CBUSA_CPM")

                                    RblSpecificProjects.Checked = True

                                    divPtf.Attributes.Add("style", "display:block")

                                ElseIf HdnParticipationType.Value = 2 Then
                                    RblNoProjects.Checked = True
                                    RblSpecificProjects.Checked = False
                                    RblSpecificProjects.Enabled = False

                                    divPtf.Attributes.Add("style", "display:none")
                                ElseIf HdnParticipationType.Value = 3 Then
                                    RblNonParticipant.Checked = True
                                    RblSpecificProjects.Checked = False
                                    RblSpecificProjects.Enabled = False
                                    RblNoProjects.Checked = False
                                    RblNoProjects.Enabled = False
                                    divPtf.Attributes.Add("style", "display:none")
                                End If
                            End If
                        Else
                            Response.Redirect("~/builder/default.aspx")
                            Exit Sub
                        End If
                    End If
                Else
                    OptValue()  'for what hppens before deadline 
                End If
            Else
                OptValue()   'if response deadline is empty
            End If
        End If

        Dim PtfProjectSql As String = DB.ExecuteScalar("select count (twopricecampaignid) from twopricebuilderproject  where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID") & " And ConstructionType = 1 ")
        HdnPtfProject.Value = PtfProjectSql


    End Sub

    Private Sub OptValue()
        If VarOpt IsNot Nothing Then
            Dim ChkSessionBuilderSql As String = DB.ExecuteScalar("Select bldr.builderid from builder bldr join twopricecampaignllc_rel llcrel On bldr.llcid = llcrel.llcid " _
            & " where llcrel.twopricecampaignid =  " & CampaignId & " And builderid = " & Session("BuilderID"))
            If ChkSessionBuilderSql = Session("BuilderID") Then

                Dim ParticipationcheckSql As String = DB.ExecuteScalar("Select * from TwoPriceBuilderParticipation where TwoPriceCampaignId = " & CampaignId & "And BuilderId = " & Session("BuilderID"))

                If VarOpt = 1 Then
                    If ParticipationcheckSql Is Nothing Then
                        Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Enroll Invitation", CampaignId, "", "", "", UserName, "CBUSA_CPM")
                        Dim InsertBldrParticipationSql As String = "Insert into TwoPriceBuilderParticipation( TwoPriceCampaignId " _
                 & ", BuilderId " _
                 & ", ParticipationType " _
                 & ", PortfolioHomes " _
                 & ", CustomHomes " _
                 & ", RecordState " _
                 & ", CreatedOn " _
                 & ", CreatedBy " _
                 & ", Modifiedon " _
                 & ", ModifiedBy)values( " _
                 & CampaignId _
                 & "," & Session("BuilderID") _
                 & ", 1 " _
                 & ", 1 " _
                 & ", 1 " _
                 & ", 1 " _
                 & "," & "'" & DateTime.Now & "'" _
                 & "," & Session("BuilderAccountId") _
                 & "," & "'" & DateTime.Now & "'" _
                 & "," & Session("BuilderAccountId") _
                 & ")"
                        DB.ExecuteSQL(InsertBldrParticipationSql)
                        divPtf.Attributes.Add("style", "display:block")

                        Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-In Projects", CampaignId, "", "", "", UserName, "CBUSA_CPM")
                        SendConfirmationMail(CampaignId)
                    Else
                        Dim UpdateBldrParticipationSql As String = "Update TwoPriceBuilderParticipation set ParticipationType = 1 where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID")
                        DB.ExecuteSQL(UpdateBldrParticipationSql)

                    End If
                    RblSpecificProjects.Checked = True
                ElseIf VarOpt = 3 Then
                    If ParticipationcheckSql Is Nothing Then
                        Dim InsertOptOutBldrParticipationSql As String = "Insert into TwoPriceBuilderParticipation( TwoPriceCampaignId " _
         & ",BuilderId " _
         & ",ParticipationType " _
         & ",PortfolioHomes " _
         & ",CustomHomes " _
         & ",RecordState " _
         & ",CreatedOn " _
         & ",CreatedBy " _
         & ",Modifiedon " _
         & ",ModifiedBy)values( " _
         & CampaignId _
         & "," & Session("BuilderID") _
         & ", 3 " _
         & ", 0 " _
         & ", 0 " _
         & ", 1 " _
         & "," & "'" & DateTime.Now & "'" _
         & "," & Session("BuilderAccountId") _
         & "," & "'" & DateTime.Now & "'" _
         & "," & Session("BuilderAccountId") _
         & ")"
                        DB.ExecuteSQL(InsertOptOutBldrParticipationSql)

                        Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Decline Invitation", CampaignId, "", "", "", UserName, "CBUSA_CPM")
                        Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-Out", CampaignId, "", "", "", UserName, "CBUSA_CPM")
                    Else
                        Dim UpdateOptOutBldrParticipationSql As String = "Update TwoPriceBuilderParticipation set ParticipationType = 3 where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID")
                        DB.ExecuteSQL(UpdateOptOutBldrParticipationSql)
                    End If
                    RblNonParticipant.Checked = True
                    divPtf.Attributes.Add("style", "display:none")

                End If

                Dim SqlGetTwoPriceCampaignBuilder_Rel As String = DB.ExecuteScalar("select * from TwoPriceCampaignBuilder_Rel where twopricecampaignid = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
                If SqlGetTwoPriceCampaignBuilder_Rel Is Nothing Then
                    Dim InsertTwoPriceCampaignBuilder_RelSql As String = "Insert into TwoPriceCampaignBuilder_Rel(Twopricecampaignid " _
                   & ",Builderid) values (" & CampaignId & "," & Session("BuilderID") & ")"
                    DB.ExecuteSQL(InsertTwoPriceCampaignBuilder_RelSql)
                End If

            Else
                Response.Redirect("~/builder/default.aspx")
                Exit Sub
            End If
        End If
    End Sub

    Private Sub BindPtfData()
        Dim objDB As New Database
        'objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        'Dim strQuery As String = ("Select BuilderProjectId, Community, ProjectName, FloorPlan, Status, StausValue" + " from TwoPriceBuilderProject")
        'Dim cmd As SqlCommand = New SqlCommand(strQuery)
        If Not ViewState("ProjectData") Is Nothing Then
            gvPtfList.DataSource = ViewState("ProjectData")
            gvPtfList.DataBind()
        Else
            gvPtfList.DataSource = GetPtfData()
            gvPtfList.DataBind()
        End If





    End Sub



    Private Function GetPtfData() As DataTable

        Dim dt As New DataTable
        Try
            Dim strQuery As String = "exec Proc_TwoPriceCampaignProjectData_horizontal " & CampaignId & ", " & Session("BuilderID") & ""
            dt = DB.GetDataTable(strQuery)
            ViewState("ProjectData") = dt
            Return dt
        Catch ex As Exception
            Return dt
        End Try
    End Function



    Private Sub GetTwoPriceBuilderInvitation()
        Dim Dr As SqlDataReader
        Dim sr As DataTable
        Dim sd As DataTable
        Dim objDB As New Database
        Dim SQL As String
        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))
        SQL = "Select * FROM TwoPriceBuilderInvitation "
        Dr = objDB.GetReader(SQL)
        sr = objDB.GetDataTable("Select * FROM TwoPriceCampaign where TwoPriceCampaignId = " & CampaignId)
        sd = objDB.GetDataTable("Select * FROM TwoPriceBuilderInvitation where TwoPriceCampaignId =" & CampaignId)
        If sr.Rows.Count > 0 Then
            StartDate.InnerHtml = sr.Rows(0)("StartDate").ToShortDateString()
            EndDate.InnerHtml = sr.Rows(0)("EndDate").ToShortDateString()
            SpnCpevent.InnerHtml = sr.Rows(0)("Name").ToString()
        Else
            StartDate.InnerHtml = ""
            EndDate.InnerHtml = ""
            SpnCpevent.InnerHtml = ""
        End If
        If sd.Rows.Count > 0 Then
            Deadline.InnerHtml = sd.Rows(0)("ResponseDeadline").ToShortDateString()
            Dim EventDescHTML As String = sd.Rows(0)("EventDescription").ToString()
            EventDescHTML = EventDescHTML.Replace("&lt;", "<").Replace("&gt;", ">").Replace("nbsp;", " ").Replace("&amp;", "").Replace("rsquo;", "'")
            TxtEventDiscription.InnerHtml = EventDescHTML
        Else
            Deadline.InnerHtml = ""
            TxtEventDiscription.InnerHtml = ""
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnCloseProject.Click
        Response.Redirect(Request.RawUrl)
    End Sub
    Private Sub btnCopy_Click(sender As Object, e As EventArgs) Handles btnCopy.Click
        'Dim IdList As List(Of String) = New List(Of String)()
        If grdProjList.Rows.Count > 0 Then

            For Each row As GridViewRow In grdProjList.Rows
                If row.RowType = DataControlRowType.DataRow Then
                    Dim chkRow As WebControls.CheckBox = TryCast(row.Cells(0).FindControl("chkProject"), WebControls.CheckBox)
                    If chkRow.Checked Then
                        'Dim name As String = row.Cells(1).Text
                        Dim ProjectId As String = TryCast(row.FindControl("hdnProjectId"), WebControls.HiddenField).Value

                        Dim sqlParams(3) As SqlParameter
                        sqlParams(0) = New SqlParameter("@ProjectId", ProjectId)
                        sqlParams(1) = New SqlParameter("@SourceTwoPriceCampaignId", ddlBuilderEvents.SelectedValue)
                        sqlParams(2) = New SqlParameter("@TargetTwoPriceCampaignId", CampaignId)
                        sqlParams(3) = New SqlParameter("@BuilderAccountId", Session("BuilderAccountId"))

                        DB.RunProc("sp_CopyTwoPriceCampaignProject", sqlParams)

                    End If
                End If
            Next
        End If

        Response.Redirect(Request.RawUrl)
    End Sub
    'Private Sub btnSavePtfProject_Click(sender As Object, e As EventArgs) Handles btnSavePtfProject.Click
    '    If Not IsValid Then Exit Sub
    '    Dim SqlPtf = "Insert into TwoPriceBuilderProject(" _
    '  & " TwoPriceCampaignId" _
    '  & ", BuilderId" _
    '  & ", ProjectName" _
    '  & ", Status" _
    '  & ", Community" _
    '  & ", FloorPlan" _
    '  & ", ConstructionType" _
    '  & ", RecordState" _
    '  & ", CreatedOn" _
    '  & ", CreatedBy" _
    '  & ", Modifiedon" _
    '  & ", ModifiedBy" _
    '  & ") VALUES (" _
    '  & HdnTwoPriceCampaignId.Value _
    '  & "," & Session("BuilderID") _
    '  & ", ''" _
    '  & ",''" _
    '  & ", ''" _
    '  & ", ''" _
    '  & ", 1 " _
    '  & ", 1 " _
    '  & "," & "'" & DateTime.Now & "'" _
    '  & "," & Session("BuilderID") _
    '  & "," & "'" & DateTime.Now & "'" _
    '  & "," & Session("BuilderID") _
    '  & ")"
    '    Dim SqlEditPtf = "Update TwoPriceBuilderProject set" _
    '  & "  ProjectName= ''" _
    '  & ",Status=''" _
    '  & ",Community= ''" _
    '  & ",FloorPlan= ''" _
    '  & ",CreatedOn=" & "'" & DateTime.Now & "'" _
    '  & ",CreatedBy=" & Session("BuilderAccountId") _
    '  & ",Modifiedon=" & "'" & DateTime.Now & "'" _
    '  & ",ModifiedBy=" & Session("BuilderAccountId") _
    '  & "where BuilderProjectId=" & HdnPtfBuilderProjectId.Value

    '    gvPtfList.DataSource = Me.GetPtfData()
    '    gvPtfList.DataBind()
    '    divPtf.Attributes.Add("style", "display:block")

    '    Dim PurchaseSql As String = DB.ExecuteScalar("select * from TwoPriceBuilderProject where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
    '    If PurchaseSql IsNot Nothing Then
    '        HdnPurchases.Value = "True"
    '    Else
    '        HdnPurchases.Value = "False"
    '    End If

    '    Dim PtfProjectSql As String = DB.ExecuteScalar("select count (twopricecampaignid) from twopricebuilderproject  where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID") & " And ConstructionType = 1 ")
    '    HdnPtfProject.Value = PtfProjectSql


    '    ' Two Price Campaign Question Data
    '    Dim strProjectDataSql As String = "SELECT  ISNULL(A.Id,0) Id  , BuilderId  , B.TwoPriceCampaignId  , B.QuestionId  ,B.QuestionLabel, B.HintText,  ISNULL( QuestionAns,'') AS QuestionAns  , BuilderProjectId " &
    '                                        " FROM         TwoPriceProjectData          B " &
    '                                       "  Left JOIN    TwoPriceBuilderProjectData   A ON A.TwoPriceCampaignId = B.TwoPriceCampaignId AND A.QuestionId=B.QuestionId  " &
    '                                       "  WHERE  B.TwoPriceCampaignId      =       " & CampaignId & " " &
    '                                       "  And   (a.BuilderId               =       " & Session("BuilderID") & "     Or a.BuilderId Is NULL) " &
    '                                       "  ORDER BY B.SortOrder ASC"
    '    Dim dtProjectData As DataTable = DB.GetDataTable(strProjectDataSql)
    '    gvProjectData.DataSource = dtProjectData
    '    gvProjectData.DataBind()

    '    ltrlPfTitle.Text = "Project Data"
    '    AddPtfProject.Attributes.Add("style", "display:block")
    '    tblPtProjectQuestions.Attributes.Add("style", "display:block")
    'End Sub

    'Public Sub BindCpDropdown()
    '    Dim dtCp = DB.GetDataTable("select distinct tc.TwoPriceCampaignId,tc.Name from TwoPriceCampaign tc join twoPriceBuilderProject tbp on tc.TwoPriceCampaignId = tbp.TwoPriceCampaignId where  tc.TwoPriceCampaignId !=" & Request.QueryString("Tcam") & " and tbp.BuilderId =" & Session("BuilderId") &
    '                               " Union " &
    '                               " select '0','Select Event'")
    '    ddlCpEvent.DataSource = dtCp
    '    ddlCpEvent.DataTextField = "Name"
    '    ddlCpEvent.DataValueField = "TwoPriceCampaignId"
    '    ddlCpEvent.DataBind()
    'End Sub


    Protected Sub EditPtf(ByVal sender As Object, ByVal e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        Dim BuilderProjetId As Integer = CType(sender, LinkButton).CommandArgument
        Dim PtfStatus As String = row.Cells(4).Text
        HdnPtfBuilderProjectId.Value = BuilderProjetId
        AddPtfProject.Attributes.Add("style", "display:block")
        HdnCheckPtfNewEntry.Value = 2
    End Sub



    Protected Sub DeleteProject(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsValid Then Exit Sub

        Dim BuilderProjetId As Integer = CType(sender, LinkButton).CommandArgument
        If HdnCnfDelete.Value = "true" Then

            Dim DeletePtfProjectSql = " EXEC PROC_TWO_PRICE_CAMPAIGN_DELETE_PROJECT " & BuilderProjetId
            DB.ExecuteSQL(DeletePtfProjectSql)
            ProjectCount()
        End If
        gvPtfList.DataSource = GetPtfData()
        gvPtfList.DataBind()
    End Sub


    Private Sub BtnAddPtfProjects_ServerClick(sender As Object, e As EventArgs) Handles BtnAddPtfProjects.ServerClick
        ltrlPfTitle.Text = "Add Project"

        '    ' Two Price Campaign Question Data
        Dim strProjectDataSql As String = "SELECT  ISNULL(A.Id,0) Id  , BuilderId  , B.TwoPriceCampaignId  , B.QuestionId  ,B.QuestionLabel, B.HintText,  ISNULL( QuestionAns,'') AS QuestionAns  , BuilderProjectId  ,B.IsRequired" &
                                            " FROM         TwoPriceProjectData          B " &
                                           "  Left JOIN    TwoPriceBuilderProjectData   A ON A.TwoPriceCampaignId = B.TwoPriceCampaignId AND A.QuestionId=B.QuestionId  AND a.BuilderProjectId = 0 " &
                                           "  WHERE  B.TwoPriceCampaignId      =       " & CampaignId & " " &
                                           "  And   (a.BuilderId               =       " & Session("BuilderID") & "     Or a.BuilderId Is NULL) " &
                                           "  ORDER BY B.SortOrder ASC"
        Dim dtProjectData As DataTable = DB.GetDataTable(strProjectDataSql)
        gvProjectData.DataSource = dtProjectData
        gvProjectData.DataBind()

        AddPtfProject.Attributes.Add("style", "display:block")
        tblPtProjectQuestions.Attributes.Add("style", "display:block")
        HdnCheckPtfNewEntry.Value = 1
    End Sub

    Private Sub RblSpecificProjects_CheckedChanged(sender As Object, e As EventArgs) Handles RblSpecificProjects.CheckedChanged
        Dim SqlParticipation As String = DB.ExecuteScalar("Select * from TwoPriceBuilderParticipation where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        If SqlParticipation IsNot Nothing Then
            Dim SpecificProjectSql As String = "Update TwoPriceBuilderParticipation set ParticipationType=1 where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID")
            DB.ExecuteSQL(SpecificProjectSql)
            RblNonParticipant.Checked = False
            RblNoProjects.Checked = False
            HdnParticipationType.Value = 1
            Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-In Projects", CampaignId, "", "", "", UserName, "CBUSA_CPM")
        Else
            Dim InsertSpecificProjectSql As String = "Insert into TwoPriceBuilderParticipation( TwoPriceCampaignId " _
      & ",BuilderId " _
      & ",ParticipationType " _
      & ",PortfolioHomes " _
      & ",CustomHomes " _
      & ",RecordState " _
      & ",CreatedOn " _
      & ",CreatedBy " _
      & ",Modifiedon " _
      & ",ModifiedBy)values( " _
      & HdnTwoPriceCampaignId.Value _
      & "," & Session("BuilderID") _
      & ", 1 " _
      & ", 0 " _
      & ", 0 " _
      & ", 1 " _
      & "," & "'" & DateTime.Now & "'" _
      & "," & Session("BuilderAccountId") _
      & "," & "'" & DateTime.Now & "'" _
      & "," & Session("BuilderAccountId") _
      & ")"
            DB.ExecuteSQL(InsertSpecificProjectSql)
            RblNonParticipant.Checked = False
            RblNoProjects.Checked = False
            HdnParticipationType.Value = 1
            Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-In Projects", CampaignId, "", "", "", UserName, "CBUSA_CPM")
        End If

        Dim SqlGetTwoPriceCampaignBuilder_Rel As String = DB.ExecuteScalar("select * from TwoPriceCampaignBuilder_Rel where twopricecampaignid = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        If SqlGetTwoPriceCampaignBuilder_Rel Is Nothing Then
            Dim InsertTwoPriceCampaignBuilder_RelSql As String = "Insert into TwoPriceCampaignBuilder_Rel(Twopricecampaignid " _
            & ",Builderid) values (" & CampaignId & "," & Session("BuilderID") & ")"
            DB.ExecuteSQL(InsertTwoPriceCampaignBuilder_RelSql)
        End If
        BindPtfData()
        divPtf.Attributes.Add("style", "display:block")
    End Sub

    Private Sub RblNoProjects_CheckedChanged(sender As Object, e As EventArgs) Handles RblNoProjects.CheckedChanged
        Dim SqlParticipation As String = DB.ExecuteScalar("Select * from TwoPriceBuilderParticipation where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        If SqlParticipation IsNot Nothing Then
            Dim NoProjectSql As String = "update TwoPriceBuilderParticipation set ParticipationType=2,PortfolioHomes=0,CustomHomes=0 where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID")
            Dim DeleteSpecificProjectSql As String = "Delete from TwoPriceBuilderProject where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID")

            DB.ExecuteSQL(NoProjectSql)
            DB.ExecuteSQL(DeleteSpecificProjectSql)
            Dim DeleteProjectData As String = "  Delete FROM TwoPriceBuilderProjectData WHERE TwoPriceCampaignId=" & CampaignId & " AND BuilderId=" & Session("BuilderID") & " "
            DB.ExecuteSQL(DeleteProjectData)
            RblNonParticipant.Checked = False
            RblSpecificProjects.Checked = False
            HdnParticipationType.Value = 2
            Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-In Ad Hoc", CampaignId, "", "", "", UserName, "CBUSA_CPM")
        Else
            Dim InsertSpecificProjectSql As String = "Insert into TwoPriceBuilderParticipation( TwoPriceCampaignId " _
      & ",BuilderId " _
      & ",ParticipationType " _
      & ",PortfolioHomes " _
      & ",CustomHomes " _
      & ",RecordState " _
      & ",CreatedOn " _
      & ",CreatedBy " _
      & ",Modifiedon " _
      & ",ModifiedBy)values( " _
      & HdnTwoPriceCampaignId.Value _
      & "," & Session("BuilderID") _
      & ", 2 " _
      & ", 0 " _
      & ", 0 " _
      & ", 1 " _
      & "," & "'" & DateTime.Now & "'" _
      & "," & Session("BuilderAccountId") _
      & "," & "'" & DateTime.Now & "'" _
      & "," & Session("BuilderAccountId") _
      & ")"
            DB.ExecuteSQL(InsertSpecificProjectSql)
            RblNonParticipant.Checked = False
            RblSpecificProjects.Checked = False
            HdnParticipationType.Value = 2
            Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-In Ad Hoc", CampaignId, "", "", "", UserName, "CBUSA_CPM")
        End If
        Dim SqlGetTwoPriceCampaignBuilder_Rel As String = DB.ExecuteScalar("select * from TwoPriceCampaignBuilder_Rel where twopricecampaignid = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        If SqlGetTwoPriceCampaignBuilder_Rel Is Nothing Then
            Dim InsertTwoPriceCampaignBuilder_RelSql As String = "Insert into TwoPriceCampaignBuilder_Rel(Twopricecampaignid " _
            & ",Builderid) values (" & CampaignId & "," & Session("BuilderID") & ")"
            DB.ExecuteSQL(InsertTwoPriceCampaignBuilder_RelSql)
        End If
        gvPtfList.DataSource = GetPtfData()
        gvPtfList.DataBind()
        divPtf.Attributes.Add("style", "display:none")
    End Sub

    Private Sub RblNonParticipant_CheckedChanged(sender As Object, e As EventArgs) Handles RblNonParticipant.CheckedChanged
        Dim SqlParticipation As String = DB.ExecuteScalar("Select * from TwoPriceBuilderParticipation where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        If SqlParticipation IsNot Nothing Then
            Dim NonParticipantSql As String = "update TwoPriceBuilderParticipation set ParticipationType=3,PortfolioHomes=0,CustomHomes=0 where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID")
            Dim DeleteSpecificProjectSql As String = "Delete from TwoPriceBuilderProject where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID")
            DB.ExecuteSQL(NonParticipantSql)
            DB.ExecuteSQL(DeleteSpecificProjectSql)

            Dim DeleteProjectData As String = "  Delete FROM TwoPriceBuilderProjectData WHERE TwoPriceCampaignId=" & CampaignId & " AND BuilderId=" & Session("BuilderID") & " "
            DB.ExecuteSQL(DeleteProjectData)

            RblSpecificProjects.Checked = False
            RblNoProjects.Checked = False
            HdnParticipationType.Value = 3
            Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-Out", CampaignId, "", "", "", UserName, "CBUSA_CPM")
        Else
            Dim InsertSpecificProjectSql As String = "Insert into TwoPriceBuilderParticipation( TwoPriceCampaignId " _
      & ",BuilderId " _
      & ",ParticipationType " _
      & ",PortfolioHomes " _
      & ",CustomHomes " _
      & ",RecordState " _
      & ",CreatedOn " _
      & ",CreatedBy " _
      & ",Modifiedon " _
      & ",ModifiedBy)values( " _
      & HdnTwoPriceCampaignId.Value _
      & "," & Session("BuilderID") _
      & ", 3 " _
      & ", 0 " _
      & ", 0 " _
      & ", 1 " _
      & "," & "'" & DateTime.Now & "'" _
      & "," & Session("BuilderAccountId") _
      & "," & "'" & DateTime.Now & "'" _
      & "," & Session("BuilderAccountId") _
      & ")"
            DB.ExecuteSQL(InsertSpecificProjectSql)
            RblSpecificProjects.Checked = False
            RblNoProjects.Checked = False
            HdnParticipationType.Value = 3
            Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Opt-Out", CampaignId, "", "", "", UserName, "CBUSA_CPM")
        End If
        Dim SqlGetTwoPriceCampaignBuilder_Rel As String = DB.ExecuteScalar("select * from TwoPriceCampaignBuilder_Rel where twopricecampaignid = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
        If SqlGetTwoPriceCampaignBuilder_Rel IsNot Nothing Then
            Dim InsertTwoPriceCampaignBuilder_RelSql As String = ("Delete from TwoPriceCampaignBuilder_Rel where twopricecampaignid = " & CampaignId & " And BuilderId = " & Session("BuilderID"))
            DB.ExecuteSQL(InsertTwoPriceCampaignBuilder_RelSql)
        End If
        gvPtfList.DataSource = GetPtfData()
        gvPtfList.DataBind()
        divPtf.Attributes.Add("style", "display:none")
    End Sub



    Private Sub LoadRblCheck()
        If HdnParticipationType.Value = "" Then       'if immediate send is done for cp event
            RblNoProjects.Checked = False
            RblSpecificProjects.Checked = False
            RblNonParticipant.Checked = False
            divPtf.Attributes.Add("style", "display:none")
        Else
            If (VarOpt = 1) Or (HdnParticipationType.Value = 1) Then     'by defalut Select options depending upon whatbuilder have choosed In link

                RblSpecificProjects.Checked = True
                divPtf.Attributes.Add("style", "display:block")
            Else
                If (HdnParticipationType.Value = 2) Then
                    RblNoProjects.Checked = True
                    divPtf.Attributes.Add("style", "display:none")

                Else
                    RblNonParticipant.Checked = True
                    divPtf.Attributes.Add("style", "display:none")
                End If
            End If
        End If
    End Sub

    Private Sub ProjectCount()
        Dim PtfProjectSql As String = DB.ExecuteScalar("select count (twopricecampaignid) from twopricebuilderproject  where TwoPriceCampaignId = " & CampaignId & " And BuilderId = " & Session("BuilderID") & " And ConstructionType = 1 ")
        HdnPtfProject.Value = PtfProjectSql
    End Sub




    Private Sub SendConfirmationMail(ByVal CampaignId As Integer)
        Dim objDB As New Database
        Dim dtConformationMaildata As DataTable
        objDB.Open(DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword")))

        dtConformationMaildata = objDB.GetDataTable("Select tpC.Name as NameWdMonth, " _
        & " bldr.CompanyName, " _
        & " FORMAT(tpinvi.ResponseDeadline, 'MMMM dd, yyyy') AS ResponseDeadlineFull, " _
        & " ba.Email as RecipientEmail, " _
        & " ba.FirstName +' '+ba.Lastname AS RecipientFullName, " _
        & " amd.Email as AdminEmail, " _
        & " amd.FirstName +' '+amd.Lastname AS AdminFullName, tpinvi.AdditionalContact AS AdditionalContact, " _
        & " ba.BuilderAccountId" _
        & " from Twopricebuilderinvitation tpinvi join Twopricecampaign tpc  " _
        & " on tpc.TwopricecampaignId = tpinvi.TwopricecampaignId join TwoPriceCampaignLLC_Rel  llcRel ON llcRel.TwopricecampaignId =tpc.TwopricecampaignId " _
        & " join builder bldr on bldr.llcid = llcRel.llcid join builderaccount ba on ba.builderid = bldr.BuilderID " _
        & " JOIN Admin amd ON tpinvi.PrimaryContact=amd.AdminId " _
        & " where tpinvi.twopricecampaignid = " & CampaignId & " and bldr.builderid =" & Session("BuilderID") & " and ba.isactive=1 and bldr.isactive=1 and ba.email is not null")
        If dtConformationMaildata.Rows.Count <> 0 Then
            Dim SNameWdMonth, sOptInLink, sOptOutLink, SResponseDeadline, sAdminEmail, sBuilderAccountId, sAdminFullName, sBuilderName As String
            SNameWdMonth = dtConformationMaildata.Rows(0)("NameWdMonth").ToString()
            SResponseDeadline = dtConformationMaildata.Rows(0)("ResponseDeadlineFull").ToString()
            sBuilderAccountId = dtConformationMaildata.Rows(0)("BuilderAccountId").ToString()
            sAdminFullName = dtConformationMaildata.Rows(0)("AdminFullName").ToString()
            sBuilderName = dtConformationMaildata.Rows(0)("CompanyName").ToString()

            Dim OptInLink As String = AppSettings("GlobalRefererName") & "/default.aspx?mod=tpc&Tcam=" & CampaignId & "&Opt=1"
            sOptInLink = "<a style="" border: 2px solid #282927; font-size:12px; line-height:12px;font - weight:400; color:#FFF; background:#073e6a;text-decoration:none;"" href=""" & OptInLink & """> Provide Enrollment Details </a>"

            Dim OptOutLink As String = AppSettings("GlobalRefererName") & "/cpmodule/OptOut.aspx?Mod=tpc&Tcam=" & CampaignId & "&Opt=3&BuilderId=" & Session("BuilderID") & "&BuilderAccountID=" & sBuilderAccountId
            sOptOutLink = "<a style=""text-decoration: underline; color: #073e6a;"" href=""" & OptOutLink & """> opt-out</a>"

            sAdminEmail = dtConformationMaildata.Rows(0)("AdminEmail").ToString()
            Dim RSpnNameWdMonth, RspnName, RSpnOptInLink, ROptOutLink, RSpnResponseDeadline, RSpnBuilderName As String
            RSpnNameWdMonth = "<span ID=""SpnNameWdMonth"">" & SNameWdMonth & "</span>"
            RspnName = "<span ID=""spnName"" style=""font-weight:700;"">" & SNameWdMonth & "</span>"
            RSpnOptInLink = "<span ID=""SpnOptInLink"">" & sOptInLink & "</span>"
            ROptOutLink = "<span id=""SpnOptOutLink"">" & sOptOutLink & "</span>"
            RSpnResponseDeadline = "<span id=""SpnResponseDeadline"">" & SResponseDeadline & "</span>"
            RSpnBuilderName = "<span ID=""SpnBuilderName"">" & sBuilderName & "</span>"

            Dim MailBody As String = DivComformation.InnerHtml
            MailBody = Replace(MailBody, "<span ID=""SpnNameWdMonth""></span>", RSpnNameWdMonth)
            MailBody = Replace(MailBody, "<span ID=""spnName"" style=""font-weight:700;""></span>", RspnName)
            MailBody = Replace(MailBody, "<span ID=""SpnOptInLink""></span>", RSpnOptInLink)
            MailBody = Replace(MailBody, "<span id=""SpnOptOutLink""></span>", ROptOutLink)
            MailBody = Replace(MailBody, "<span id=""SpnResponseDeadline""></span>", RSpnResponseDeadline)
            MailBody = Replace(MailBody, "<span ID=""SpnBuilderName""></span>", RSpnBuilderName)
            Dim ReqMailBody As String = MailBody.Replace("&lt;", "<").Replace("&gt;", ">")
            Dim MailSubject As String = "Welcome to the " & SNameWdMonth & " committed-purchase event!"
            Dim AdminMailSubject As String = sBuilderName & " has opted in for the " & SNameWdMonth
            Dim AdminMailBody As String = sBuilderName & " has opted in for the " & SNameWdMonth
            Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA Customer Service", sAdminEmail, sAdminFullName, AdminMailSubject, AdminMailBody, "", "")

            '============= SEND MAIL TO ADDITIONAL CONTACT =====================
            Dim sAdditionalContactEmail As String = dtConformationMaildata.Rows(0)("AdditionalContact").ToString()
            If sAdditionalContactEmail <> String.Empty Then
                For Each temp As String In sAdditionalContactEmail.Split(",")
                    If temp.Length > 1 And temp <> "" Then
                        Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA Customer Service", temp, "", AdminMailSubject, AdminMailBody, "", "")
                    End If
                Next
            End If
            '===================================================================

            If dtConformationMaildata.Rows.Count > 0 Then
                Dim i As Integer = 0
                For Each row As DataRow In dtConformationMaildata.Rows
                    Dim sRecipientFullName As String = dtConformationMaildata.Rows(i)("RecipientFullName").ToString()
                    Dim sRecipientEmail As String = dtConformationMaildata.Rows(i)("RecipientEmail").ToString()
                    Core.SendSimpleMail(sAdminEmail, "CBUSA Customer Service", sRecipientEmail, sRecipientFullName, MailSubject, ReqMailBody, "", "")
                    i = i + 1
                Next
            End If

        End If

        '''''''''''''''''   mail to Additional Contact
        'Dim RetVal As DataTable = DB.GetDataTable("select a.Name,c.companyname,d.AdditionalContact from TwoPriceCampaign a" &
        '                                          " inner join TwoPriceCampaignBuilder_Rel b on a.TwoPriceCampaignID=b.TwoPriceCampaignID" &
        '                                          " inner join Builder c on c.BuilderID=b.BuilderID" &
        '                                          " inner join TwoPriceBuilderInvitation d on d.TwoPriceCampaignID=a.TwoPriceCampaignID" &
        '                                          " where a.TwoPriceCampaignId=" & CampaignId & " and b.BuilderID=" & Session("BuilderID"))

        'Dim AdditionalmsgTo As MailAddress
        'Dim AdditionalmsgPri As MailMessage

        'Dim MailSubjectPri As String = RetVal.Rows(0)("companyname") & " has opted In For the " & RetVal.Rows(0)("Name")
        'Dim MailBodyPri As String = RetVal.Rows(0)("companyname") & " has opted In For the " & RetVal.Rows(0)("Name")
        'Dim msgFromPri As MailAddress = New MailAddress("customerservice@cbusa.us", "CBUSA Customer Service")

        'For Each temp As String In RetVal.Rows(0)("AdditionalContact").ToString().Split(",")
        '    If temp.Length > 1 And temp <> "" Then

        '        AdditionalmsgTo = New MailAddress(temp, "")
        '        AdditionalmsgPri = New MailMessage(msgFromPri, AdditionalmsgTo)
        '        AdditionalmsgPri.IsBodyHtml = False
        '        AdditionalmsgPri.Subject = MailSubjectPri
        '        AdditionalmsgPri.Body = MailBodyPri
        '        Core.SendMail(AdditionalmsgPri, "", "")
        '    End If
        'Next

    End Sub

    Private Sub PopulateProjectCopyNumericDropdowns()

        ddlCopyProjectCounter.Items.Clear()
        ddlBlankProjectCounter.Items.Clear()

        Dim iCounter As Integer

        For iCounter = 1 To 10
            ddlCopyProjectCounter.Items.Add(iCounter.ToString())
            ddlBlankProjectCounter.Items.Add(iCounter.ToString())
        Next

    End Sub

    'Private Sub gvPtfList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gvPtfList.RowDataBound

    '    e.Row.Cells(1).Visible = False
    '    e.Row.Cells(2).Visible = False
    '    e.Row.Cells(3).Visible = False


    '    If e.Row.RowType = DataControlRowType.DataRow Then

    '        Dim lnkBtnCopy As New LinkButton
    '        lnkBtnCopy = e.Row.FindControl("lnkBtnCopyPortfolioProject")

    '        lnkBtnCopy.OnClientClick = "return ShowCopyProjectDialog('" & e.Row.DataItem("BuilderProjectId") & "');"

    '        If e.Row.RowIndex = gvPtfList.EditIndex Then
    '            Dim divEditDeleteCopy As New HtmlControls.HtmlGenericControl("div")
    '            divEditDeleteCopy = e.Row.FindControl("divPtfEditDeleteCopy")
    '            divEditDeleteCopy.Style.Item("display") = "none"

    '            Dim divUpdateCancel As New HtmlControls.HtmlGenericControl("div")
    '            divUpdateCancel = e.Row.FindControl("divPtfUpdateCancel")
    '            divUpdateCancel.Style.Item("display") = "block"

    '        End If
    '    End If

    'End Sub

    Protected Sub gvPtfList_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles gvPtfList.RowEditing
        gvPtfList.EditIndex = e.NewEditIndex
        gvPtfList.DataSource = ViewState("ProjectData")
        gvPtfList.DataBind()
    End Sub

    Private Sub gvPtfList_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles gvPtfList.RowCancelingEdit
        gvPtfList.EditIndex = -1
        gvPtfList.DataSource = ViewState("ProjectData")
        gvPtfList.DataBind()
    End Sub

    Private Sub gvPtfList_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles gvPtfList.RowUpdating


        Dim TwoPriceCampaignId As String = e.NewValues("TwoPriceCampaignId")
        Dim BuilderId As String = e.NewValues("BuilderId")

        For Each val As DictionaryEntry In e.NewValues
            If val.Key <> "TwoPriceCampaignId" And val.Key <> "BuilderId" Then
                'Dim QuestionId As String = DB.ExecuteScalar("select QuestionId from TwoPriceProjectData where  TwoPriceCampaignId=" & TwoPriceCampaignId & " and QuestionLabel= '" & val.Key & "'")
                'Dim strSql As String = "UPDATE TwoPriceBuilderProjectData SET QuestionAns='" & val.Value & "' WHERE QuestionId=" & QuestionId & " And TwoPriceCampaignId =" & TwoPriceCampaignId & " And BuilderProjectId   = '" & gvPtfList.DataKeys(e.RowIndex).Item("BuilderProjectId") & "'"
                'DB.ExecuteSQL(strSql)
                Dim sqlParams(4) As SqlParameter

                sqlParams(0) = New SqlParameter("@ProjectId", gvPtfList.DataKeys(e.RowIndex).Item("BuilderProjectId"))
                sqlParams(1) = New SqlParameter("@TwoPriceCampaignId", TwoPriceCampaignId)
                sqlParams(2) = New SqlParameter("@BuilderId", BuilderId)
                sqlParams(3) = New SqlParameter("@QuestionLabel", SqlDbType.NVarChar, 250)
                sqlParams(3).Value = val.Key
                sqlParams(4) = New SqlParameter("@Answer", SqlDbType.NVarChar, 250)
                sqlParams(4).Value = IIf(val.Value Is Nothing, "", val.Value)

                DB.RunProc("PROC_UpdateTwoPriceProjectData", sqlParams)
            End If
        Next


        gvPtfList.EditIndex = -1
        gvPtfList.DataSource = GetPtfData()
        gvPtfList.DataBind()
    End Sub



    Private Sub btnCreateProjectCopy_Click(sender As Object, e As EventArgs) Handles btnCreateProjectCopy.Click
        CreateProjectCopies(Me.hdnSelectedProjectId.Value, Me.ddlCopyProjectCounter.SelectedValue)
        'Response.Redirect(Request.RawUrl)
    End Sub

    Private Sub CreateProjectCopies(ByVal pProjectId As Integer, ByVal pNumberOfCopies As Integer)
        Dim sqlParams(2) As SqlParameter

        sqlParams(0) = New SqlParameter("@ProjectId", pProjectId)
        sqlParams(1) = New SqlParameter("@NumberOfCopies", pNumberOfCopies)
        sqlParams(2) = New SqlParameter("@BuilderAccountId", Session("BuilderAccountId"))

        DB.RunProc("sp_CreateTwoPriceProjectCopies", sqlParams)

        'Re-populate GridViews
        gvPtfList.DataSource = GetPtfData()
        gvPtfList.DataBind()
    End Sub

    Private Sub btnCreateBlankProjects_Click(sender As Object, e As EventArgs) Handles btnCreateBlankProjects.Click
        CreateBlankProjects(Me.hdnConstructionType.Value, Me.ddlBlankProjectCounter.SelectedValue)
    End Sub

    Private Sub CreateBlankProjects(ByVal pConstructionType As Short, ByVal pNumberOfCopies As Integer)

        Dim sqlParams(4) As SqlParameter

        sqlParams(0) = New SqlParameter("@TwoPriceCampaignId", Convert.ToInt32(CampaignId))
        sqlParams(1) = New SqlParameter("@BuilderId", Session("BuilderId"))
        sqlParams(2) = New SqlParameter("@ConstructionType", pConstructionType)
        sqlParams(3) = New SqlParameter("@NumberOfCopies", pNumberOfCopies)
        sqlParams(4) = New SqlParameter("@BuilderAccountId", Session("BuilderAccountId"))

        DB.RunProc("sp_CreateTwoPriceBlankProjects", sqlParams)

        If pConstructionType = 1 Then
            gvPtfList.EditIndex = gvPtfList.Rows.Count
            gvPtfList.DataSource = GetPtfData()
            gvPtfList.DataBind()
        End If

    End Sub

    Function PtfLogProjectData() As String
        Dim jsonPtf As String = "TwoPriceCampaignId: " & HdnTwoPriceCampaignId.Value & ",BuilderId: " & Session("BuilderID") & ",ProjectName: ''," &
      " ConstructionType: 1,RecordState: 1,CreatedOn: " _
      & "'" & DateTime.Now & "',CreatedBy: '" & DateTime.Now & "',Modifiedon: '" & DateTime.Now & "',ModifiedBy: " & Session("BuilderID")
        Return jsonPtf
    End Function

    Protected Sub btnSavePtfProjectData_Click(sender As Object, e As EventArgs) Handles btnSavePtfProjectData.Click

        If Not IsValid Then Exit Sub

        If HdnCheckPtfNewEntry.Value = 1 Then
            Dim SqlPtf = "Insert into TwoPriceBuilderProject(" _
              & " TwoPriceCampaignId" _
              & ", BuilderId" _
              & ", ProjectName" _
              & ", Status" _
              & ", Community" _
              & ", FloorPlan" _
              & ", ConstructionType" _
              & ", RecordState" _
              & ", CreatedOn" _
              & ", CreatedBy" _
              & ", Modifiedon" _
              & ", ModifiedBy" _
              & ") VALUES (" _
              & HdnTwoPriceCampaignId.Value _
              & "," & Session("BuilderID") _
              & ", ''" _
              & ",'1'" _
              & ", ''" _
              & ", ''" _
              & ", 1 " _
              & ", 1 " _
              & "," & "'" & DateTime.Now & "'" _
              & "," & Session("BuilderID") _
              & "," & "'" & DateTime.Now & "'" _
              & "," & Session("BuilderID") _
              & ")"


            Dim NewPtfJsonData As String = PtfLogProjectData()

            HdnPtfBuilderProjectId.Value = DB.InsertSQL(SqlPtf)
            Core.DataLog("Committed Purchase Builder", PageURL, CurrentUserId, "Add Portfolio", CampaignId, "", NewPtfJsonData, "", UserName, "CBUSA_CPM")
        End If


        If gvProjectData.Rows.Count > 0 Then
            For Each gvr As GridViewRow In gvProjectData.Rows
                Dim hdnId As HiddenField = gvr.Cells(0).FindControl("hdnpfId")
                Dim hdnQuestionId As HiddenField = gvr.Cells(0).FindControl("hdnpfQuestionId")
                Dim txtAnswer As WebControls.TextBox = gvr.Cells(1).FindControl("txtpfAnswer")

                Dim strAnswer As String = txtAnswer.Text

                If Convert.ToInt32(hdnId.Value) > 0 Then
                    Dim strSql As String = "UPDATE TwoPriceBuilderProjectData SET QuestionAns='" & strAnswer.Trim() & "' WHERE Id=" & hdnId.Value & " And TwoPriceCampaignId =" & CampaignId & " And BuilderProjectId   = '" & HdnPtfBuilderProjectId.Value & "'"
                    DB.ExecuteSQL(strSql)
                Else
                    Dim strSql As String = "INSERT INTO TwoPriceBuilderProjectData VALUES(" & Session("BuilderID") & "," & CampaignId & "," & hdnQuestionId.Value & ",'" & strAnswer.Trim() & "','" & HdnPtfBuilderProjectId.Value & "')"
                    DB.ExecuteSQL(strSql)
                End If
            Next
        End If
        AddPtfProject.Attributes.Add("style", "display:none")
        tblPtProjectQuestions.Attributes.Add("style", "display:none")

        gvPtfList.DataSource = GetPtfData()
        gvPtfList.DataBind()
    End Sub

    Private Sub btnCancelPtfProjectData_Click(sender As Object, e As EventArgs) Handles btnCancelPtfProjectData.Click
        AddPtfProject.Attributes.Add("style", "display:none")
        tblPtProjectQuestions.Attributes.Add("style", "display:none")

    End Sub


    'Protected Sub UpdatePfProjectData(ByVal sender As Object, ByVal e As EventArgs)
    '    If Not IsValid Then Exit Sub



    'End Sub


    Protected Sub gvPtfList_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.Cells.Count > 3 Then
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
        End If

        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim lnkBtnCopy As New LinkButton
            lnkBtnCopy = e.Row.FindControl("lnkBtnCopyPortfolioProject")

            lnkBtnCopy.OnClientClick = "return ShowCopyProjectDialog('" & e.Row.DataItem("BuilderProjectId") & "');"

            If e.Row.RowIndex = gvPtfList.EditIndex Then
                Dim divEditDeleteCopy As New HtmlControls.HtmlGenericControl("div")
                divEditDeleteCopy = e.Row.FindControl("divPtfEditDeleteCopy")
                divEditDeleteCopy.Style.Item("display") = "none"

                Dim divUpdateCancel As New HtmlControls.HtmlGenericControl("div")
                divUpdateCancel = e.Row.FindControl("divPtfUpdateCancel")
                divUpdateCancel.Style.Item("display") = "block"

            End If
        End If

    End Sub
    Protected Sub gvPtfList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If e.CommandName = "EDIT" Then

            Dim BuilderProjectId As String = e.CommandArgument
            'ltrlPfTitle.Text = "Project Data"
            'Dim strProjectDataSql As String = "SELECT  ISNULL(A.Id,0) Id  , BuilderId  , B.TwoPriceCampaignId  , B.QuestionId  ,B.QuestionLabel, B.HintText,  ISNULL( QuestionAns,'') AS QuestionAns  , BuilderProjectId " &
            '                                 " FROM         TwoPriceProjectData          B " &
            '                                "  Left JOIN    TwoPriceBuilderProjectData   A ON A.TwoPriceCampaignId = B.TwoPriceCampaignId AND A.QuestionId=B.QuestionId  " &
            '                                "  WHERE  B.TwoPriceCampaignId      =      " & CampaignId & " " &
            '                                "  And   (a.BuilderId               =       " & Session("BuilderID") & "     Or a.BuilderId Is NULL) " &
            '                                "  AND      A.BuilderProjectId      =      " & BuilderProjectId & "" &
            '                                "  ORDER    BY B.QuestionId ASC              "
            'Dim dtProjectData As DataTable = DB.GetDataTable(strProjectDataSql)
            'gvProjectData.DataSource = dtProjectData
            'gvProjectData.DataBind()

            AddPtfProject.Attributes.Add("style", "display:none")
            tblPtProjectQuestions.Attributes.Add("style", "display:none")
            HdnPtfBuilderProjectId.Value = BuilderProjectId
            HdnCheckPtfNewEntry.Value = 2

        End If

    End Sub
    Protected Sub grdProjList_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.Cells.Count > 3 Then
            e.Row.Cells(1).Visible = False
            e.Row.Cells(2).Visible = False
            e.Row.Cells(3).Visible = False
        End If
    End Sub

    Protected Sub ddlBuilderEvents_OnSelectedIndexChanged(sender As Object, e As EventArgs)

        If ddlBuilderEvents.SelectedValue = "-Select-" Then
            Dim strQuery As String = "exec Proc_TwoPriceCampaignProjectData_horizontal " & 0 & ", " & Session("BuilderID") & ""
            grdProjList.DataSource = DB.GetDataTable(strQuery)
            grdProjList.DataBind()

            grdProjList.Visible = False
        Else

            Dim TPC_Id As String = ddlBuilderEvents.SelectedValue

            Dim strQuery As String = "exec Proc_TwoPriceCampaignProjectData_horizontal " & TPC_Id & ", " & Session("BuilderID") & ""
            grdProjList.DataSource = DB.GetDataTable(strQuery)
            grdProjList.DataBind()
            grdProjList.Visible = True
        End If

    End Sub
End Class
