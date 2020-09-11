Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected PIQAccountID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("PIQ")

        PIQAccountID = Convert.ToInt32(Request("PIQAccountID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpPIQID.Datasource = PIQRow.GetList(DB, "CompanyName")
        drpPIQID.DataValueField = "PIQID"
        drpPIQID.DataTextField = "CompanyName"
        drpPIQID.Databind()
        drpPIQID.Items.Insert(0, New ListItem("", ""))

        If PIQAccountID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbPIQAccount As PIQAccountRow = PIQAccountRow.GetRow(DB, PIQAccountID)
        txtFirstName.Text = dbPIQAccount.FirstName
        txtLastName.Text = dbPIQAccount.LastName
        txtUsername.Text = dbPIQAccount.Username
        drpPIQID.SelectedValue = dbPIQAccount.PIQID
        rblIsPrimary.SelectedValue = dbPIQAccount.IsPrimary
        rblRequirePasswordUpdate.SelectedValue = dbPIQAccount.RequirePasswordUpdate
        txtPhone.Text = dbPIQAccount.Phone
        txtMobile.Text = dbPIQAccount.Mobile
        txtFax.Text = dbPIQAccount.Fax
        txtEmail.Text = dbPIQAccount.Email
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If PIQAccountID <> 0 Then
            rfvPassword.Enabled = True
            rfvPasswordVerify.Enabled = True
        Else
            rfvPassword.Enabled = False
            rfvPasswordVerify.Enabled = False
        End If

        If PIQAccountID = 0 OrElse PIQAccountRow.GetRow(DB, PIQAccountID).Username <> txtUsername.Text Then
            If Not PIQAccountRow.CheckUsernameAvailability(DB, txtUsername.Text) Then
                AddError("Entered Username is not available")
                Exit Sub
            End If
        End If

        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbPIQAccount As PIQAccountRow

            If PIQAccountID <> 0 Then
                dbPIQAccount = PIQAccountRow.GetRow(DB, PIQAccountID)
            Else
                dbPIQAccount = New PIQAccountRow(DB)
            End If
            dbPIQAccount.FirstName = txtFirstName.Text
            dbPIQAccount.LastName = txtLastName.Text
            dbPIQAccount.Username = txtUsername.Text
            If Not txtPassword.Text = String.Empty Then
                dbPIQAccount.Password = txtPassword.Text
            End If
            dbPIQAccount.PIQID = drpPIQID.SelectedValue
            dbPIQAccount.IsPrimary = rblIsPrimary.SelectedValue
            dbPIQAccount.RequirePasswordUpdate = rblRequirePasswordUpdate.SelectedValue
            dbPIQAccount.Phone = txtPhone.Text
            dbPIQAccount.Mobile = txtMobile.Text
            dbPIQAccount.Fax = txtFax.Text
            dbPIQAccount.Email = txtEmail.Text
            If PIQAccountID <> 0 Then
                dbPIQAccount.Update()
            Else
                PIQAccountID = dbPIQAccount.Insert
                Dim PIQName As String = DB.ExecuteScalar("select CompanyName from Piq where PIQId = " & drpPIQID.SelectedValue)

                SendNotification(dbPIQAccount.Email, PIQName, txtFirstName.Text & " " & txtLastName.Text, txtUsername.Text)
            End If

            DB.CommitTransaction()


            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
    Private Sub SendNotification(ByVal Email As String, PIQName As String, ByVal Name As String, ByVal Username As String)
        Dim msg As String = PIQName & " has added a New account for " & Name & " ( " & Username & ", " & Email & " ) "

        Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        Core.SendSimpleMail(FromAddress, FromName, Convert.ToString(ConfigurationManager.AppSettings("piqAddMailId")), Name, "New PIQ Account Added", msg)


        Page.ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "window.setTimeout(""location.href='/default.aspx';"",30000);", True)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?PIQAccountID=" & PIQAccountID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

