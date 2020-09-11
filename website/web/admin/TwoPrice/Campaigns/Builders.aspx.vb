Imports Components
Imports Controls
Imports DataLayer
Imports TwoPrice.DataLayer
Imports System.Linq
Imports System.Web.Services

Partial Class admin_TwoPrice_Campaigns_Builders
    Inherits AdminPage

    Protected TwoPriceCampaignId As Integer
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CheckAccess("TWO_PRICE_CAMPAIGNS")
        TwoPriceCampaignId = Convert.ToInt32(Request("TwoPriceCampaignId"))
        If TwoPriceCampaignId = 0 Then
            AddError("A campaign is not selected.  Please select a Campaign from the admin.")
            Exit Sub
        End If
        pnlMain.Visible = True
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        Dim dbTwoPriceCampaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
        ltlCamapignName.Text = dbTwoPriceCampaign.Name
        Dim currLLCs As String = dbTwoPriceCampaign.GetSelectedLLCs
        If currLLCs <> Nothing Then
            Dim dt As DataTable = DB.GetDataTable("SELECT LLC from LLC where LLCID in" & DB.NumberMultiple(currLLCs))
            Dim sLLC As String = String.Empty
            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1 Step 1
                    sLLC &= dt(i)("LLC") & IIf(i < dt.Rows.Count - 1, ", ", "")
                Next
            End If
            ltlLLCS.Text = sLLC
            Dim dtBuilders As DataTable = DB.GetDataTable("SELECT BuilderID, Companyname, builder.LLCID,LLC  FROM Builder " & _
                                                    " INNER JOIN LLC " & _
                                                    " ON Builder.LLCID = LLC.LLCID " & _
                                                    " WHERE builder.LLCID IN" & DB.NumberMultiple(currLLCs) & _
                                                    " AND builder.IsActive = 1")
            drpbuilders.DataSource = dtBuilders
            drpbuilders.DataGroupField = "LLC"
            drpbuilders.DataValueField = "BuilderId"
            drpbuilders.DataTextField = "CompanyName"
            drpbuilders.DataBind()
            drpbuilders.ClearSelection()
            hdnBuilders.Value = dbTwoPriceCampaign.GetSelectedCampaignBuilders
        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles btnSubmit1.Click, btnSubmit2.Click
        Dim bValid As Boolean = True
        If hdnBuilders.Value = Nothing Then
            AddError("At least one builder must be selected.")
            bValid = False
            Exit Sub
        End If
        If TwoPriceCampaignId = Nothing Then
            AddError("A Campaign must first be selected.  Please go to the Campaign Management to associate builders.")
            bValid = False
        End If
        If Page.IsValid AndAlso bValid Then
            Try
                DB.BeginTransaction()
                Dim dbTwoPriceCampaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
                dbTwoPriceCampaign.DeleteFromAllCampaignBuilders()
                'Remove duplicates if they exist
                dbTwoPriceCampaign.InsertToCampaignBuilders(hdnBuilders.Value)
                ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType, "ChangesSaved", "alert('Changes have been saved!');", True)
                DB.CommitTransaction()
            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError(ErrHandler.ErrorText(ex))
            End Try
        End If

    End Sub
End Class
