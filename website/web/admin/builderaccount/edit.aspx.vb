Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Utilities

Public Class Edit
    Inherits AdminPage

    Protected BuilderAccountID As Integer
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private UpdatedBuilderAccountId As String = ""
    Private NewBuilderAccountId As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDER_ACCOUNTS")

        BuilderAccountID = Convert.ToInt32(Request("BuilderAccountID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("AdminId")
        UserName = Session("Username")
        UpdatedBuilderAccountId = BuilderAccountID
    End Sub
    Private Sub LoadFromDB()

        drpBuilderID.DataSource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.DataBind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))
        F_Role.DataSource = BuilderRoleRow.GetList(DB, "BuilderRoleID")
        F_Role.DataTextField = "BuilderRole"
        F_Role.DataValueField = "BuilderRoleID"
        F_Role.DataBind()
        If BuilderAccountID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, BuilderAccountID)
        txtCRMID.Text = dbBuilderAccount.CRMID
        'txtCRMID.Enabled = False
        txtHistoricID.Text = dbBuilderAccount.HistoricID
        txtFirstName.Text = dbBuilderAccount.FirstName
        txtLastName.Text = dbBuilderAccount.LastName
        txtTitle.Text = dbBuilderAccount.Title
        txtPhone.Text = dbBuilderAccount.Phone
        txtMobile.Text = dbBuilderAccount.Mobile
        txtFax.Text = dbBuilderAccount.Fax
        txtUsername.Text = dbBuilderAccount.Username
        txtHistoricPasswordHash.Text = dbBuilderAccount.HistoricPasswordHash
        txtHistoricPasswordSalt.Text = dbBuilderAccount.HistoricPasswordSalt
        txtHistoricPasswordSha1.Text = dbBuilderAccount.HistoricPasswordSha1
        txtEmail.Text = dbBuilderAccount.Email
        drpBuilderID.SelectedValue = dbBuilderAccount.BuilderID
        rblIsPrimary.SelectedValue = dbBuilderAccount.IsPrimary
        rblIsActive.SelectedValue = dbBuilderAccount.IsActive
        rblSendNCPReminder.SelectedValue = dbBuilderAccount.SendNCPReminder

        F_Role.SelectedValues = dbBuilderAccount.GetSelectedRoles


    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If BuilderAccountID <> 0 Then
            If Not txtPassword.Text = String.Empty Then
                rfvPassword.Enabled = True
                rfvPasswordVerify.Enabled = True

            Else
                rfvPassword.Enabled = False
                rfvPasswordVerify.Enabled = False
            End If

        Else
            rfvPassword.Enabled = False
            rfvPasswordVerify.Enabled = False
        End If

        If BuilderAccountID = 0 OrElse BuilderAccountRow.GetRow(DB, BuilderAccountID).Username <> txtUsername.Text Then
            If Not BuilderAccountRow.CheckUsernameAvailability(DB, txtUsername.Text) Then
                AddError("Entered Username is not available")
                Exit Sub
            End If
        End If

        Dim strCRMIDCheckStatus As String = ""
        If CheckIfCRMIDExists(strCRMIDCheckStatus) = False Then
            AddError(strCRMIDCheckStatus)
            Exit Sub
        End If

        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbBuilderAccount As BuilderAccountRow

            If BuilderAccountID <> 0 Then
                dbBuilderAccount = BuilderAccountRow.GetRow(DB, BuilderAccountID)
            Else
                dbBuilderAccount = New BuilderAccountRow(DB)
            End If
            If txtHistoricID.Text <> Nothing Then
                dbBuilderAccount.HistoricID = txtHistoricID.Text
            End If

            dbBuilderAccount.CRMID = IIf(txtCRMID.Text = "", String.Empty, txtCRMID.Text)

            dbBuilderAccount.FirstName = txtFirstName.Text
            dbBuilderAccount.LastName = txtLastName.Text
            dbBuilderAccount.Title = txtTitle.Text
            dbBuilderAccount.Phone = txtPhone.Text
            dbBuilderAccount.Mobile = txtMobile.Text
            dbBuilderAccount.Fax = txtFax.Text
            dbBuilderAccount.Username = txtUsername.Text
            dbBuilderAccount.HistoricPasswordHash = txtHistoricPasswordHash.Text
            dbBuilderAccount.HistoricPasswordSalt = txtHistoricPasswordSalt.Text
            dbBuilderAccount.HistoricPasswordSha1 = txtHistoricPasswordSha1.Text
            dbBuilderAccount.Email = txtEmail.Text
            If Not txtPassword.Text = String.Empty Then
                'log password change
                If Not dbBuilderAccount.Password = txtPassword.Text Then
                    Core.DataLog("Admin", PageURL, CurrentUserId, "Builder Password Update", UpdatedBuilderAccountId, "", "", "", UserName)
                End If
                'end log
                dbBuilderAccount.Password = txtPassword.Text
            End If
            dbBuilderAccount.BuilderID = drpBuilderID.SelectedValue
            dbBuilderAccount.IsPrimary = rblIsPrimary.SelectedValue
            'log If IsActive value Changes
            If rblIsActive.SelectedValue = dbBuilderAccount.IsActive Then
                dbBuilderAccount.IsActive = rblIsActive.SelectedValue
            Else
                Core.DataLog("Admin", PageURL, CurrentUserId, "Update Builder Active Status", UpdatedBuilderAccountId, "", "", "", UserName)
                dbBuilderAccount.IsActive = rblIsActive.SelectedValue
            End If
            'End loggig activity
            dbBuilderAccount.SendNCPReminder = rblSendNCPReminder.SelectedValue

            If BuilderAccountID <> 0 Then
                dbBuilderAccount.Update()

                'log update builder account
                Core.DataLog("Admin", PageURL, CurrentUserId, "Update Builder Account", UpdatedBuilderAccountId, "", "", "", UserName)
                'end log

                '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.UpsertBuilderAccount(dbBuilderAccount) = False Then
                '        'throw error
                '    End If
                'End If
                '===============================================================================================

            Else
                BuilderAccountID = dbBuilderAccount.Insert()

                'log add builder account 
                NewBuilderAccountId = dbBuilderAccount.BuilderAccountID
                Core.DataLog("Admin", PageURL, CurrentUserId, "Add Builder Account", NewBuilderAccountId, "", "", "", UserName)
                'end log

                '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.InsertBuilderAccount(dbBuilderAccount, BuilderRow.GetRow(DB, dbBuilderAccount.BuilderID).CRMID) = False Then
                '        'throw error
                '    End If
                'End If
                '===============================================================================================
            End If

            If dbBuilderAccount.IsPrimary <> False Then
                SQL = ""
                SQL = "  Update BuilderAccount set IsPrimary = 0  "
                SQL &= " where BuilderID = " & DB.Quote(dbBuilderAccount.BuilderID)
                SQL &= " and BuilderAccountID not in  ( " & DB.Quote(dbBuilderAccount.BuilderAccountID) & ") "

                DB.ExecuteSQL(SQL)
            End If

            dbBuilderAccount.DeleteFromBuilderRoles()
            dbBuilderAccount.InsertToBuilderAccountBuilderRoles(F_Role.SelectedValues)

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
        Response.Redirect("delete.aspx?BuilderAccountID=" & BuilderAccountID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Function CheckIfCRMIDExists(ByRef strReturnMessage As String) As Boolean

        Dim strCRMID As String = txtCRMID.Text.Trim()

        If strCRMID = "" Then Return True

        Dim dbBuilderAccRow As BuilderAccountRow = BuilderAccountRow.GetBuilderAccountByCRMID(DB, strCRMID)
        Dim dbVendorAccRow As VendorAccountRow = VendorAccountRow.GetVendorAccountByCRMID(DB, strCRMID)

        If Not dbBuilderAccRow Is Nothing Then
            If dbBuilderAccRow.BuilderAccountID <> 0 AndAlso dbBuilderAccRow.BuilderAccountID <> BuilderAccountID Then

                Dim dbBuilderRow As BuilderRow = BuilderRow.GetRow(DB, dbBuilderAccRow.BuilderID)
                Dim dbLLC As LLCRow = LLCRow.GetRow(DB, dbBuilderRow.LLCID)

                Dim strAccountStatus As String = IIf(dbBuilderAccRow.IsActive, "Active", "Inactive")
                Dim strAccountName As String = String.Concat(dbBuilderAccRow.FirstName, " ", dbBuilderAccRow.LastName)

                strReturnMessage = String.Concat("This CRM ID is already assigned to the ", strAccountStatus, " account of ", strAccountName, " with ", dbBuilderRow.CompanyName, " in the ", dbLLC.LLC, " market. You cannot assign the same CRM ID to multiple accounts. Please determine why the other account is using this CRM ID and correct it in this app and/or in Insightly as necessary.")

                Return False

            ElseIf dbVendorAccRow.VendorAccountID <> 0 Then

                Dim dbVendorRow As VendorRow = VendorRow.GetRow(DB, dbVendorAccRow.VendorID)
                Dim dbLLC As LLCRow = LLCRow.GetRow(DB, dbVendorRow.LLCID)

                Dim strAccountStatus As String = IIf(dbVendorAccRow.IsActive, "Active", "Inactive")
                Dim strAccountName As String = String.Concat(dbVendorAccRow.FirstName, " ", dbVendorAccRow.LastName)

                strReturnMessage = String.Concat("This CRM ID is already assigned to the ", strAccountStatus, " account of ", strAccountName, " with ", dbVendorRow.CompanyName, " in the ", dbLLC.LLC, " market. You cannot assign the same CRM ID to multiple accounts. Please determine why the other account is using this CRM ID and correct it in this app and/or in Insightly as necessary.")

                Return False

            End If
        End If

        Return True

    End Function

End Class
