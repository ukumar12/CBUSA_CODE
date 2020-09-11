Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected BannerId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BANNERS")

        BannerId = Convert.ToInt32(Request("BannerId"))
        If Not IsPostBack Then
            LoadFromDB()
            RefreshPageControls()
        End If
    End Sub

    Private Sub LoadFromDB()
        If BannerId = 0 Then
            rblBannerType.SelectedValue = "Image"
            btnDelete.Visible = False
            ltlDimensions.Text = "Dimensions will be automatically calculated"
            Exit Sub
        End If

        Dim dbBanner As BannerRow = BannerRow.GetRow(DB, BannerId)
        txtName.Text = dbBanner.Name
        txtLink.Text = dbBanner.Link
        txtAltText.Text = dbBanner.AltText
        txtHTML.Text = dbBanner.HTML
        drpTarget.Text = dbBanner.Target
        fuFilename.CurrentFileName = dbBanner.FileName
        fuFilename.DisplayImage = True
        chkIsActive.Checked = dbBanner.IsActive
        chkIsOrderTracking.Checked = dbBanner.IsOrderTracking
        rblBannerType.SelectedValue = dbBanner.BannerType
        txtWidth.Text = dbBanner.Width
        txtHeight.Text = dbBanner.Height
        ltlDimensions.Text = dbBanner.Width & " x " & dbBanner.Height
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbBanner As BannerRow
            If BannerId <> 0 Then
                dbBanner = BannerRow.GetRow(DB, BannerId)
            Else
                dbBanner = New BannerRow(DB)
            End If
            dbBanner.Name = txtName.Text
            dbBanner.Link = txtLink.Text
            If fuFilename.NewFileName <> String.Empty Then
                fuFilename.SaveNewFile()
                dbBanner.FileName = fuFilename.NewFileName
            ElseIf fuFilename.MarkedToDelete Then
                dbBanner.FileName = Nothing
            End If
            dbBanner.IsActive = chkIsActive.Checked
            dbBanner.IsOrderTracking = chkIsOrderTracking.Checked

            Dim WidthChanged As Boolean = False
            Select Case rblBannerType.SelectedValue
                Case "Image"
                    If fuFilename.NewFileName <> String.Empty Then
                        Dim w, h As Integer
                        Core.GetImageSize(Server.MapPath(fuFilename.Folder & "/" & fuFilename.NewFileName), w, h)
                        If dbBanner.Width <> w Then WidthChanged = True
                        dbBanner.Width = w
                        dbBanner.Height = h
                    End If
                    dbBanner.Target = drpTarget.SelectedValue
                    dbBanner.AltText = txtAltText.Text
                    dbBanner.HTML = String.Empty
                Case "Flash"
                    dbBanner.AltText = String.Empty
                    dbBanner.HTML = String.Empty
                    dbBanner.Target = String.Empty
                    If dbBanner.Width <> txtWidth.Text Then WidthChanged = True
                    dbBanner.Width = txtWidth.Text
                    dbBanner.Height = txtHeight.Text
                Case "Custom"
                    dbBanner.AltText = String.Empty
                    dbBanner.HTML = txtHTML.Text
                    dbBanner.Target = String.Empty
                    If dbBanner.Width <> txtWidth.Text Then WidthChanged = True
                    dbBanner.Width = txtWidth.Text
                    dbBanner.Height = txtHeight.Text
            End Select
            dbBanner.BannerType = rblBannerType.SelectedValue

            Dim Redirect As String = String.Empty
            If BannerId <> 0 Then
                dbBanner.Update()
                Redirect = "default.aspx?" & GetPageParams(FilterFieldType.All)
            Else
                BannerId = dbBanner.Insert
                Redirect = "groups.aspx?F_BannerId=" & BannerId & "&" & GetPageParams(FilterFieldType.All, "F_BannerId")
            End If

            'Remove all associations with groups that don't fit with the new width anymore
            If WidthChanged Then BannerBannerGroupRow.RemoveNotMatchingByBannerWidth(DB, dbBanner.BannerId, dbBanner.Width)

            DB.CommitTransaction()

            If fuFilename.NewFileName <> String.Empty Or fuFilename.MarkedToDelete Then fuFilename.RemoveOldFile()

            Response.Redirect(Redirect)

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?BannerId=" & BannerId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub rblBannerType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblBannerType.SelectedIndexChanged
        RefreshPageControls()
    End Sub

    Private Sub RefreshPageControls()
        Select Case rblBannerType.SelectedValue
            Case "Image"
                trHTML.Visible = False
                trAlt.Visible = True
                trDimensionsAuto.Visible = True
                trFile.Visible = True
                trTarget.Visible = True
            Case "Flash"
                trHTML.Visible = False
                trAlt.Visible = False
                trDimensionsAuto.Visible = False
                trFile.Visible = True
                trTarget.Visible = False
            Case "Custom"
                trHTML.Visible = True
                trAlt.Visible = False
                trDimensionsAuto.Visible = False
                trFile.Visible = False
                trTarget.Visible = False
        End Select
        trDimensions.Visible = Not trDimensionsAuto.Visible
    End Sub
End Class

