Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected BannerGroupId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BANNERS")

        BannerGroupId = Convert.ToInt32(Request("BannerGroupId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If BannerGroupId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbBannerGroup As BannerGroupRow = BannerGroupRow.GetRow(DB, BannerGroupId)
        txtName.Text = dbBannerGroup.Name
        txtMinWidth.Text = dbBannerGroup.MinWidth
        txtMaxWidth.Text = dbBannerGroup.MaxWidth
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbBannerGroup As BannerGroupRow

            If BannerGroupId <> 0 Then
                dbBannerGroup = BannerGroupRow.GetRow(DB, BannerGroupId)
            Else
                dbBannerGroup = New BannerGroupRow(DB)
            End If
            dbBannerGroup.Name = txtName.Text

            Dim WidthChanged As Boolean = False
            If dbBannerGroup.MinWidth <> txtMinWidth.Text Or dbBannerGroup.MaxWidth <> txtMaxWidth.Text Then
                WidthChanged = True
            End If

            dbBannerGroup.MinWidth = txtMinWidth.Text
            dbBannerGroup.MaxWidth = txtMaxWidth.Text

            Dim Redirect As String = String.Empty
            If BannerGroupId <> 0 Then
                dbBannerGroup.Update()
                Redirect = "default.aspx?" & GetPageParams(FilterFieldType.All)
            Else
                BannerGroupId = dbBannerGroup.Insert
                Redirect = "banners.aspx?F_BannerGroupId=" & BannerGroupId & "&" & GetPageParams(FilterFieldType.All, "F_BannerGroupId")
            End If

            'Remove all associations with banners that don't fit with the new width anymore
            If WidthChanged Then BannerBannerGroupRow.RemoveNotMatchingByMinMaxWidth(DB, dbBannerGroup.BannerGroupId, dbBannerGroup.MinWidth, dbBannerGroup.MaxWidth)

            DB.CommitTransaction()

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
        Response.Redirect("delete.aspx?BannerGroupId=" & BannerGroupId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

