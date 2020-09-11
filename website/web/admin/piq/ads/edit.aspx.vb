Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected PIQAdID As Integer
    Public PIQID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PIQ")

        PIQID = Convert.ToInt32(Request("PIQID"))

        If IsDBNull(PIQID) OrElse PIQID = 0 Then Response.Redirect("/admin/piq/default.aspx?" & GetPageParams(FilterFieldType.All))

        ltrPIQ.Text = PIQRow.GetRow(DB, PIQID).CompanyName

        PIQAdID = Convert.ToInt32(Request("PIQAdID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If PIQAdID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPIQAd As PIQAdRow = PIQAdRow.GetRow(DB, PIQAdID)
        PIQID = dbPIQAd.PIQID
        txtAltText.Text = dbPIQAd.AltText
        txtLinkURL.Text = dbPIQAd.LinkURL
        dtStartDate.Value = dbPIQAd.StartDate
        dtEndDate.Value = dbPIQAd.EndDate
        fuAdFile.CurrentFileName = dbPIQAd.AdFile
        rblIsActive.SelectedValue = dbPIQAd.IsActive
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbPIQAd As PIQAdRow

            If PIQAdID <> 0 Then
                dbPIQAd = PIQAdRow.GetRow(DB, PIQAdID)
            Else
                dbPIQAd = New PIQAdRow(DB)
            End If
            dbPIQAd.PIQID = PIQID
            dbPIQAd.AltText = txtAltText.Text
            dbPIQAd.LinkURL = txtLinkURL.Text
            dbPIQAd.StartDate = dtStartDate.Value
            dbPIQAd.EndDate = dtEndDate.Value
            If fuAdFile.NewFileName <> String.Empty Then
                fuAdFile.SaveNewFile()
                dbPIQAd.AdFile = fuAdFile.NewFileName
                Core.ResizeImageWithQuality(Server.MapPath("/assets/piq/ads/" & dbPIQAd.AdFile), Server.MapPath("/assets/piq/ads/" & dbPIQAd.AdFile), SysParam.GetValue(Me.DB, "PIQAdsImageWidth"), SysParam.GetValue(Me.DB, "PIQAdsImageHeight"), 100)
                Core.ResizeImageWithQuality(Server.MapPath("/assets/piq/ads/" & dbPIQAd.AdFile), Server.MapPath("/assets/piq/ads/thumbnails/" & dbPIQAd.AdFile), SysParam.GetValue(Me.DB, "PIQAdsThumbnailWidth"), SysParam.GetValue(Me.DB, "PIQAdsThumbnailHeight"), 100)
            ElseIf fuAdFile.MarkedToDelete Then
                dbPIQAd.AdFile = Nothing
            End If
            dbPIQAd.IsActive = rblIsActive.SelectedValue

            If PIQAdID <> 0 Then
                dbPIQAd.Update()
            Else
                PIQAdID = dbPIQAd.Insert
            End If

            DB.CommitTransaction()

            If fuAdFile.NewFileName <> String.Empty OrElse fuAdFile.MarkedToDelete Then fuAdFile.RemoveOldFile()

            Response.Redirect("default.aspx?PIQID=" & PIQID & "&" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?PIQID=" & PIQID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?PIQID=" & PIQID & "&PIQAdID=" & PIQAdID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

