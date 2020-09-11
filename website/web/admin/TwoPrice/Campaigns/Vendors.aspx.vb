Imports Components
Imports Controls
Imports DataLayer
Imports TwoPrice.DataLayer
Imports System.Linq
Imports System.Web.Services
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

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
            'Dim dtVendors As DataTable = DB.GetDataTable("SELECT v.VendorID, v.CompanyName, llcv.LLCID, llc.LLC" _
            '                                & " FROM Vendor v" _
            '                                & " INNER JOIN LLCVendor llcv" _
            '                                & " ON v.VendorId =  llcv.VendorId" _
            '                                & " INNER JOIN LLC" _
            '                                & " ON llcv.LLCID = llcv.LLCID" _
            '                                & " WHERE llcv.LLCID IN " & DB.NumberMultiple(currLLCs) _
            '                                & " ORDER BY LLC.LLC")
            Dim dtVendors As DataTable = DB.GetDataTable("SELECT v.VendorID, v.CompanyName, 'Vendors' as vend" _
                                           & " FROM Vendor v" _
                                           & " WHERE v.VendorId IN (SELECT VendorId FROM LLCVendor WHERE LLCID IN " & DB.NumberMultiple(currLLCs) & ")" _
                                           & " AND v.IsActive = 1" _
                                           & " ORDER BY v.CompanyName ASC")
            drpVendors.DataSource = dtVendors
            drpVendors.DataValueField = "VendorId"
            drpVendors.DataTextField = "CompanyName"
            drpVendors.DataGroupField = "vend"
            drpVendors.DataBind()
            drpVendors.ClearSelection()
            hdnVendors.Value = dbTwoPriceCampaign.GetSelectedCampaignVendors

            Dim dtBidDeadLineDate = If(dbTwoPriceCampaign.VendorBidDeadline = Nothing, DateTime.Now.AddDays(10).ToString("d").Replace("-", "/"), dbTwoPriceCampaign.VendorBidDeadline.ToString("d").Replace("-", "/"))
            dtBidDeadLine.Text = dtBidDeadLineDate
            dtBidDeadLine.Value = dtBidDeadLineDate
            ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", "<script type = 'text/javascript'>window.onload=function(){ document.getElementById('" + dtBidDeadLine.ClientID + "_txtDatePicker').value='" + dtBidDeadLineDate + "';} </script>")
            dtBidDeadLine.Attributes.Add("value", dtBidDeadLineDate)


        End If
    End Sub
    Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles btnSubmit1.Click, btnSubmit2.Click
        Dim bValid As Boolean = True
        If hdnVendors.Value = Nothing Then
            AddError("At least one Vendor must be selected.")
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
                dbTwoPriceCampaign.DeleteFromAllCampaignVendors()
                dbTwoPriceCampaign.InsertToCampaignVendors(hdnVendors.Value)
                dbTwoPriceCampaign.AddUpdateVendorBidDeadline(dtBidDeadLine.Value)
                ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType, "ChangesSaved", "alert('Changes have been saved!');", True)

                LoadMessaging()

                DB.CommitTransaction()
            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError(ErrHandler.ErrorText(ex))
            End Try
        End If

    End Sub
    Public Sub LoadMessaging()
        pnlMain.Visible = False
        pnlSendMessage.Visible = True
        Dim dbAutoMessage As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignRequest")
        ltlAutoMessage.Text = dbAutoMessage.Message.Replace(vbCrLf, "<br>")
    End Sub

    Protected Sub btnSendMessage_Click(sender As Object, e As System.EventArgs) Handles btnSendMessage.Click
        Try
            DB.BeginTransaction()

            Dim dbAutoMessage As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignRequest")
            Dim dbTwoPriceCampaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, TwoPriceCampaignId)
            Dim vendors As String = dbTwoPriceCampaign.GetSelectedCampaignVendors()

            If vendors <> Nothing Then
                'Update status
                Using sqlr As SqlDataReader = DB.GetReader("SELECT * FROM TwoPriceStatus WHERE Value = 'BiddingInProgress'")
                    If sqlr.Read Then
                        dbTwoPriceCampaign.Status = sqlr("Value")
                    End If
                End Using

                dbTwoPriceCampaign.Status = "BiddingInProgress"
                dbTwoPriceCampaign.Update()
                Dim dtVendors As DataTable = DB.GetDataTable("select VendorId, CompanyName FROM Vendor WHERE VendorId in " & DB.NumberMultiple(vendors))

                Dim colVendors As List(Of VendorRow) = VendorRow.GetCollection(DB, vendors)

                For Each dbVendor As VendorRow In colVendors
                    Dim sBody As String = FormatMessage(DB, dbAutoMessage, dbVendor, txtMessage.Text)
                    ' SendEmail(DB, row, sBody)
                    sBody = sBody.Replace("%%CampaignName%%", dbTwoPriceCampaign.Name)
                    sBody = sBody.Replace("%%BidListLink%%", AppSettings("GlobalRefererName") & "/vendor/twoprice/sku-price.aspx?TwoPriceTakeOffId=" & DB.ExecuteScalar("SELECT TOP 1 TwoPriceTakeOffId FROM TwoPriceTakeOff WHERE TwoPriceCampaignId = " & dbTwoPriceCampaign.TwoPriceCampaignId))
                    dbAutoMessage.CCList = txtCC.Text().Trim()
                    'Add ops manager to CC
                    Dim Conn As String = ""
                    If dbAutoMessage.CCList <> Nothing Then Conn = ","
                    For Each LLCID As String In dbVendor.GetSelectedLLCs.Split(",")
                        Dim LLC As LLCRow = LLCRow.GetRow(DB, LLCID)
                        dbAutoMessage.CCList &= Conn & LLC.OperationsManager
                    Next
                    dbAutoMessage.Send(dbVendor, sBody, True)
                Next
            End If
            pnlMain.Visible = False
            pnlSendMessage.Visible = False
            pnlComplete.Visible = True
            DB.CommitTransaction()
        Catch ex As Exception
            pnlMain.Visible = False
            pnlSendMessage.Visible = True
            pnlComplete.Visible = False
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
    Private Function FormatMessage(ByVal DB As Database, ByVal automessage As AutomaticMessagesRow, ByVal drVendor As VendorRow, ByVal Msg As String) As String
        Dim tempMsg As String
        tempMsg = automessage.Message
        tempMsg = tempMsg.Replace("%%Vendor%%", drVendor.CompanyName)
        tempMsg = tempMsg.Replace("%%AdditionalMessage%%", Msg)

        Return tempMsg
    End Function

    Protected Sub btnSaveDeadlineDate_Click(sender As Object, e As System.EventArgs) Handles btnSaveDeadlineDate.Click
        Dim bValid As Boolean = True
        If hdnVendors.Value = Nothing Then
            AddError("At least one Vendor must be selected.")
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

                dbTwoPriceCampaign.AddUpdateVendorBidDeadline(dtBidDeadLine.Value)
                ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType, "DeadlineSaved", "alert(' Bid-Submission Reminder Deadline Date have been saved!');", True)

                DB.CommitTransaction()
            Catch ex As Exception
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError(ErrHandler.ErrorText(ex))
            End Try
        End If

    End Sub
End Class
