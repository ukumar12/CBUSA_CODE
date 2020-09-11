Imports TwoPrice.DataLayer
Imports DataLayer
Imports Components
Imports System.Configuration.ConfigurationManager

Public Class TwoPriceBidRequestReminders

    Public Shared Sub Run(ByVal DB As Database)

        Dim dtActiveCampaigns As DataTable = DB.GetDataTable("SELECT * " & _
                                                             " FROM TwoPriceCampaign " & _
                                                             " WHERE EndDate > GETDATE() " & _
                                                             " AND AwardedVendorId IS NULL " & _
                                                             " AND IsActive = 1")

        For Each dr As DataRow In dtActiveCampaigns.Rows
            Dim Campaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, dr("TwoPriceCampaignId"))

            If (Campaign.EndDate - DateTime.Now).Days <= 5 Then
                For Each VID As String In Campaign.GetCampaignVendorswithNotDeclined().Split()
                    Dim VendorId As Integer
                    If Integer.TryParse(VID, VendorId) Then
                        Dim Vendor As VendorRow = VendorRow.GetRow(DB, VID)

                        Dim dtVendorBids As DataTable = DB.GetDataTable("SELECT * " & _
                                                                        " FROM TwoPriceVendorProductPrice " & _
                                                                        " WHERE TwoPriceCampaignID = " & Campaign.TwoPriceCampaignId & _
                                                                        " AND VendorID = " & Vendor.VendorID & _
                                                                        " AND Submitted = 1")
                        If dtVendorBids.Rows.Count = 0 Then
                            'If no submitted bids
                            Dim dbAutoMessage As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "CampaignBidReminder")

                            Dim DaysLeft As String = (Campaign.EndDate - DateTime.Now).Days
                            Dim BidLink As String = AppSettings("GlobalRefererName") & "/vendor/twoprice/edit.aspx?TwoPriceTakeOffId=" & Campaign.TwoPriceCampaignId

                            Dim sBody As String = FormatMessage(DB, dbAutoMessage, Vendor, Campaign.Name, DaysLeft, BidLink)
                            dbAutoMessage.SendAdmin("", sBody)
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    Private Shared Function FormatMessage(ByVal DB As Database, ByVal automessage As AutomaticMessagesRow, ByVal drVendor As VendorRow, ByVal CampaignName As String, ByVal DaysLeft As String, ByVal BidLink As String) As String
        Dim tempMsg As String
        tempMsg = automessage.Message
        tempMsg = tempMsg.Replace("%%Vendor%%", drVendor.CompanyName)
        tempMsg = tempMsg.Replace("%%Campaign%%", CampaignName)
        tempMsg = tempMsg.Replace("%%days%%", DaysLeft)
        tempMsg = tempMsg.Replace("%%bidlink%%", BidLink)

        Return tempMsg
    End Function

End Class
