Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Configuration.ConfigurationManager
Imports System.IO


Partial Class admin_TwoPrice_Campaigns_PostDeadlineEnrollment
    Inherits AdminPage


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        CheckAccess("TWO_PRICE_CAMPAIGNS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            gvList.PageIndex = IIf(Request("F_pg") = String.Empty, 0, Core.ProtectParam(Request("F_pg")))

            LoadCPEvent()

            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "TPCBR.TwoPriceCampaignId"
                gvList.SortOrder = "DESC"
            End If
            BindList()
        End If


    End Sub


    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder
        ViewState("F_pg") = gvList.PageIndex.ToString

        SQL = "SELECT distinct B.BuilderID,B.CompanyName,TPBI.ResponseDeadline," &
            " 'Opt Out' as ResponseStatus from TwoPriceBuilderParticipation A" &
            " inner Join Builder B on B.BuilderID=A.BuilderID" &
            " inner Join TwoPriceBuilderInvitation TPBI on TPBI.TwoPriceCampaignId=A.TwoPriceCampaignId" &
            " where a.TwoPriceCampaignId = " & Request.QueryString("TwoPriceCampaignId") & "  And TPBI.ResponseDeadline <= convert(date,getdate(),103)" &
            " And (A.ParticipationType=3 or A.ParticipationType=4) AND b.IsActive=1 " &
            " UNION" &
            " Select distinct B.BuilderID,B.CompanyName,TPBI.ResponseDeadline," &
            " 'No Response' as ResponseStatus from TwoPriceBuilderDistribution A" &
            " inner Join Builder B on A.BuilderID=B.BuilderID" &
            " inner Join TwoPriceBuilderInvitation TPBI on TPBI.TwoPriceCampaignId=A.TwoPriceCampaignId" &
            " where a.TwoPriceCampaignId = " & Request.QueryString("TwoPriceCampaignId") & "  And TPBI.ResponseDeadline <= convert(date,getdate(),103)" &
            " And a.BuilderID Not in (select BuilderID from TwoPriceBuilderParticipation where TwoPriceCampaignId = " & Request.QueryString("TwoPriceCampaignId") & ") AND b.IsActive=1 "




        'gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQL)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub LoadCPEvent()
        Dim SQL As String = "select * from TwoPriceCampaign where TwoPriceCampaignId=" & Request.QueryString("TwoPriceCampaignId")
        Dim dtCampaignId As DataTable = DB.GetDataTable(SQL)

        If (dtCampaignId.Rows.Count > 0) Then
            lblEventName.Text = dtCampaignId.Rows(0)("Name").ToString()
        End If
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvList.RowCommand
        If (e.CommandName = "Grant") Then


        End If
    End Sub

    Private Sub UpdateParticipation(ByVal BuilderID As Integer)
        Dim sqlparams(1) As SqlParameter

        sqlparams(0) = New SqlParameter("@BuilderID", BuilderID)
        sqlparams(1) = New SqlParameter("@TwoPriceCampaignId", Request.QueryString("TwoPriceCampaignId"))

        DB.RunProc("usp_UpdateBuilderParticipation", sqlparams)

        BindList()



    End Sub

    Protected Sub rblYesNo_SelectedIndexChanged(sender As Object, e As EventArgs)


        Dim dtBuilder As DataTable

        For i As Integer = 0 To gvList.Rows.Count - 1
            Dim row As GridViewRow = gvList.Rows(i)
            Dim rdb As RadioButtonList = CType(row.FindControl("rblYesNo"), RadioButtonList)

            If rdb.SelectedValue = "Yes" Then
                Dim btn As RadioButtonList = CType(sender, RadioButtonList)
                Dim gvr As GridViewRow = CType(btn.NamingContainer, GridViewRow)
                Dim cellValue As String = row.Cells(0).Text


                dtBuilder = DB.GetDataTable("select companyname from Builder where BuilderID=" & cellValue)

                ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Access given to " & dtBuilder.Rows(0)("companyname") & " to participate in : " & lblEventName.Text & "');", True)

                UpdateParticipation(cellValue)
                Exit Sub
            End If
        Next





    End Sub
End Class
