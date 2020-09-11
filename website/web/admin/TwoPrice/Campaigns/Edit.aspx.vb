Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports TwoPrice.DataLayer
Imports Controls

Public Class Edit
    Inherits AdminPage

    Protected TwoPriceCampaignId As Integer
    Private dbTwoprice As TwoPriceCampaignRow


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("TWO_PRICE_CAMPAIGNS")

        TwoPriceCampaignId = Convert.ToInt32(Request("TwoPriceCampaignId"))


        mudUpload.TwoPriceCampaignId = TwoPriceCampaignId

        If TwoPriceCampaignId > 0 Then
            trDocUpload.Visible = True
            btnSaveAndUpload.Visible = False
        Else
            trDocUpload.Visible = False
            btnSaveAndUpload.Visible = True
        End If

        If Not IsPostBack Then
            LoadFromDB()
        End If

       

    End Sub

    Private Sub LoadFromDB()
        drpStatus.DataSource = TwoPriceStatusRow.GetList(DB, "SortOrder")
        drpStatus.DataValueField = "Value"
        drpStatus.DataTextField = "Name"
        drpStatus.DataBind()
        cblLLC.DataSource = LLCRow.GetList(DB, "LLC")
        cblLLC.DataTextField = "LLC"
        cblLLC.DataValueField = "LLCID"
        cblLLC.DataBind()

        If TwoPriceCampaignId = 0 Then
            rblIsActive.SelectedValue = True
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbTwoPriceCampaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)

        txtName.Text = dbTwoPriceCampaign.Name
        drpStatus.SelectedValue = dbTwoPriceCampaign.Status
        dtStartDate.Value = dbTwoPriceCampaign.StartDate
        dtEndDate.Value = dbTwoPriceCampaign.EndDate
        rblIsActive.SelectedValue = dbTwoPriceCampaign.IsActive
        rblPriceUpdate.SelectedValue = dbTwoPriceCampaign.IsUpdatePrice
        cblLLC.SelectedValues = dbTwoPriceCampaign.GetSelectedLLCs
        If dbTwoPriceCampaign.Status <> "Awarded" Then
            rblPriceUpdate.Enabled = False
        Else
            rblPriceUpdate.Enabled = True
        End If

        BindDocuments()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click, btnSaveAndUpload.Click

        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbTwoPriceCampaign As TwoPriceCampaignRow

            If TwoPriceCampaignId <> 0 Then
                dbTwoPriceCampaign = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
            Else
                dbTwoPriceCampaign = New TwoPriceCampaignRow(DB)
            End If
            dbTwoPriceCampaign.Name = txtName.Text
            dbTwoPriceCampaign.Status = drpStatus.SelectedValue
            dbTwoPriceCampaign.StartDate = dtStartDate.Value
            dbTwoPriceCampaign.EndDate = dtEndDate.Value
            dbTwoPriceCampaign.IsActive = rblIsActive.SelectedValue
            dbTwoPriceCampaign.IsUpdatePrice = rblPriceUpdate.SelectedValue

            If TwoPriceCampaignId <> 0 Then
                dbTwoPriceCampaign.Update()
            Else
                TwoPriceCampaignId = dbTwoPriceCampaign.Insert
                'mudUpload.TwoPriceCampaignId

            End If
            'mudUpload.PrepareBulkUpload();
            dbTwoPriceCampaign.DeleteFromAllLLCs()
            dbTwoPriceCampaign.InsertToLLCs(cblLLC.SelectedValues)

            DB.CommitTransaction()

            If sender.ID = "btnSaveAndUpload" Then
                Response.Redirect("edit.aspx?TwoPriceCampaignId=" & TwoPriceCampaignId)
            Else

                Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
            End If


        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?TwoPriceCampaignId=" & TwoPriceCampaignId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub rptDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If


        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")

        Dim lnkDelete As ConfirmImageButton = e.Item.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Item.DataItem("DocumentId")

        lnkMessageTitle.HRef = "/assets/Twoprice/" & e.Item.DataItem("FileName").ToString

    End Sub

    Protected Sub rptDocuments_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptDocuments.ItemCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    Dim FileInfo As System.IO.FileInfo
                    Try
                        FileInfo = New System.IO.FileInfo(Server.MapPath("/assets/Twoprice/" & TwoPriceDocumentRow.GetRow(DB, e.CommandArgument).FileName))
                        FileInfo.Delete()
                    Catch ex As Exception

                    End Try
                    TwoPriceDocumentRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindDocuments()
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub

    Protected Sub BindDocuments()

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From  TwoPriceDocument  where TwoPriceCampaignId = " & DB.Number(TwoPriceCampaignId)

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)

            Me.rptDocuments.DataSource = dt
            Me.rptDocuments.DataBind()

            If dt.Rows.Count > 0 Then
                divNoCurrentDocuments.Visible = False
            End If

        End If
    End Sub
    Protected Sub btnTransferDocs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTransferDocs.Click
        BindDocuments()
    End Sub
End Class