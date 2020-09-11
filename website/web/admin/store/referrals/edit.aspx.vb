Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected ReferralId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MARKETING_TOOLS")

        ReferralId = Convert.ToInt32(Request("ReferralId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpPromotion.DataSource = DB.GetDataTable("SELECT PromotionId, PromotionCode FROM StorePromotion ORDER BY PromotionCode ASC")
        drpPromotion.DataTextField = "PromotionCode"
        drpPromotion.DataValueField = "PromotionId"
        drpPromotion.DataBind()
        drpPromotion.Items.Insert(0, New ListItem("Select optional promotion", ""))

        If ReferralId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If
        
        Dim dbReferral As ReferralRow = ReferralRow.GetRow(DB, ReferralId)
        If dbReferral.PromotionId <> Nothing Then drpPromotion.SelectedValue = dbReferral.PromotionId
        txtName.Text = dbReferral.Name
        txtCode.Text = dbReferral.Code
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbReferral As ReferralRow

            If ReferralId <> 0 Then
                dbReferral = ReferralRow.GetRow(DB, ReferralId)
            Else
                dbReferral = New ReferralRow(DB)
            End If
            dbReferral.Name = txtName.Text
            dbReferral.Code = txtCode.Text
            If drpPromotion.SelectedValue <> "" Then dbReferral.PromotionId = drpPromotion.SelectedValue Else dbReferral.PromotionId = 0

            If ReferralId <> 0 Then
                dbReferral.Update()
            Else
                ReferralId = dbReferral.Insert
            End If

            DB.CommitTransaction()


            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?ReferralId=" & ReferralId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

