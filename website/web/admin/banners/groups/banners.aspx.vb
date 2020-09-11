Imports Components
Imports Controls
Imports System.Data.Sqlclient
Imports DataLayer

Partial Class Banners
    Inherits AdminPage

    Protected BannerGroupId As Integer
    Protected dbBannerGroup As BannerGroupRow

    Private dtBannerGroups As DataTable

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BANNERS")

        BannerGroupId = Request("F_BannerGroupId")
        dbBannerGroup = BannerGroupRow.GetRow(DB, BannerGroupId)
        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            BindList()
        End If
        RefreshValidators()
    End Sub

    Private Sub BindList()
        dtBannerGroups = BannerGroupRow.GetSelectedBannerListByBannerGroup(DB, dbBannerGroup.BannerGroupId, dbBannerGroup.MinWidth, dbBannerGroup.MaxWidth)

        gvList.DataSource = BannerRow.GetList(DB, dbBannerGroup.MinWidth, dbBannerGroup.MaxWidth)
        gvList.DataBind()
    End Sub

    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub

        dtBannerGroups.DefaultView.RowFilter = "BannerId = " & e.Row.DataItem("BannerId")
        Dim chkGroup As CheckBox = e.Row.FindControl("chkGroup")
        Dim DateLbound As DatePicker = e.Row.FindControl("DateLbound")
        Dim DateUbound As DatePicker = e.Row.FindControl("DateUbound")
        Dim txtWeight As TextBox = e.Row.FindControl("txtWeight")
        Dim ltlTotal As Literal = e.Row.FindControl("ltlTotal")
        Dim UniqueId As HtmlInputHidden = e.Row.FindControl("UniqueId")
        If dtBannerGroups.DefaultView.Count > 0 Then
            Dim row As DataRowView = dtBannerGroups.DefaultView.Item(0)

            chkGroup.Checked = True
            DateLbound.Text = IIf(IsDBNull(row("DateFrom")), String.Empty, row("DateFrom"))
            DateUbound.Text = IIf(IsDBNull(row("DateTo")), String.Empty, row("DateTo"))
            txtWeight.Text = IIf(IsDBNull(row("Weight")), String.Empty, row("Weight"))
            ltlTotal.Text = IIf(IsDBNull(row("TotalWeight")), 0, row("TotalWeight"))
            UniqueId.Value = IIf(IsDBNull(row("UniqueId")), 0, row("UniqueId"))
        Else
            chkGroup.Checked = False
            DateLbound.Text = String.Empty
            DateUbound.Text = String.Empty
            txtWeight.Text = String.Empty
            ltlTotal.Text = 0
            UniqueId.Value = String.Empty
        End If
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All, "F_BannerGroupId"))
    End Sub

    Private Sub RefreshValidators()
        For Each row As GridViewRow In gvList.Rows
            Dim chkGroup As CheckBox = row.FindControl("chkGroup")
            Dim vDateLBound As DateValidator = row.FindControl("vDateLBound")
            Dim vDateUBound As DateValidator = row.FindControl("vDateUBound")
            Dim ivtxtWeight As IntegerValidator = row.FindControl("ivtxtWeight")
            Dim rvtxtWeight As RequiredFieldValidator = row.FindControl("rvtxtWeight")

            vDateLBound.Enabled = chkGroup.Checked
            vDateUBound.Enabled = chkGroup.Checked
            ivtxtWeight.Enabled = chkGroup.Checked
            rvtxtWeight.Enabled = chkGroup.Checked
        Next
    End Sub


    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()
            For Each row As GridViewRow In gvList.Rows
                Dim BannerId As HtmlInputHidden = row.FindControl("BannerId")
                Dim chkGroup As CheckBox = row.FindControl("chkGroup")
                Dim DateLbound As DatePicker = row.FindControl("DateLbound")
                Dim DateUbound As DatePicker = row.FindControl("DateUbound")
                Dim txtWeight As TextBox = row.FindControl("txtWeight")
                Dim ltlTotal As Literal = row.FindControl("ltlTotal")
                Dim UniqueId As HtmlInputHidden = row.FindControl("UniqueId")

                If chkGroup.Checked Then
                    Dim dbBannerBannerGroup As BannerBannerGroupRow = BannerBannerGroupRow.GetRowByBannerAndGroup(DB, BannerId.Value, BannerGroupId)
                    dbBannerBannerGroup.DateFrom = DateLbound.Value
                    dbBannerBannerGroup.DateTo = DateUbound.Value
                    dbBannerBannerGroup.Weight = txtWeight.Text
                    dbBannerBannerGroup.BannerGroupId = BannerGroupId
                    dbBannerBannerGroup.BannerId = BannerId.Value
                    If dbBannerBannerGroup.UniqueId = 0 Then
                        dbBannerBannerGroup.Insert()
                    Else
                        dbBannerBannerGroup.Update()
                    End If
                Else
                    If Not UniqueId.Value = String.Empty Then BannerBannerGroupRow.RemoveRow(DB, UniqueId.Value)
                End If
            Next
            DB.CommitTransaction()
            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All, "F_BannerGroupId"))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

End Class

