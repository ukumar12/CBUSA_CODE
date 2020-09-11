Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Utilities

Public Class Edit
    Inherits AdminPage

    Protected VendorAccountID As Integer
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private UpdatedVendorAccountId As String = ""
    Private NewVendorAccountId As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_ACCOUNTS")

        VendorAccountID = Convert.ToInt32(Request("VendorAccountID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("AdminId")
        UserName = Session("Username")
        UpdatedVendorAccountId = VendorAccountID
    End Sub

    Private Sub LoadFromDB()
        If VendorAccountID = 0 Then
            rfvPassword.Enabled = True
            rfvPasswordVerify.Enabled = True
        Else
            rfvPassword.Enabled = False
            rfvPasswordVerify.Enabled = False
        End If

        drpVendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
        drpVendorID.DataValueField = "VendorID"
        drpVendorID.DataTextField = "CompanyName"
        drpVendorID.DataBind()
        drpVendorID.Items.Insert(0, New ListItem("", ""))

        drpVendorBranchOfficeID.Visible = False

        If VendorAccountID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        drpVendorBranchOfficeID.Visible = True

        Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, VendorAccountID)

        drpVendorBranchOfficeID.DataSource = VendorBranchOfficeRow.GetListByVendor(DB, dbVendorAccount.VendorID, "Address")
        drpVendorBranchOfficeID.DataValueField = "VendorBranchOfficeID"
        drpVendorBranchOfficeID.DataTextField = "Address"
        drpVendorBranchOfficeID.DataBind()
        drpVendorBranchOfficeID.Items.Insert(0, New ListItem("", ""))

        txtCRMID.Text = dbVendorAccount.CRMID
        txtCRMID.Enabled = False
        txtHistoricID.Text = dbVendorAccount.HistoricID
        txtFirstName.Text = dbVendorAccount.FirstName
        txtLastName.Text = dbVendorAccount.LastName
        txtTitle.Text = dbVendorAccount.Title
        txtPhone.Text = dbVendorAccount.Phone
        txtMobile.Text = dbVendorAccount.Mobile
        txtFax.Text = dbVendorAccount.Fax
        txtEmail.Text = dbVendorAccount.Email
        txtUsername.Text = dbVendorAccount.Username
        txtHistoricPasswordHash.Text = dbVendorAccount.HistoricPasswordHash
        txtHistoricPasswordSalt.Text = dbVendorAccount.HistoricPasswordSalt
        txtHistoricPasswordSha1.Text = dbVendorAccount.HistoricPasswordSha1
        ltlCreate.Text = FormatDateTime(dbVendorAccount.Created, DateFormat.ShortDate)
        ltlUpdate.Text = FormatDateTime(dbVendorAccount.Updated, DateFormat.ShortDate)
        drpVendorID.SelectedValue = dbVendorAccount.VendorID
        drpVendorBranchOfficeID.SelectedValue = dbVendorAccount.VendorBranchOfficeID
        rblIsPrimary.SelectedValue = dbVendorAccount.IsPrimary
        rblIsActive.SelectedValue = dbVendorAccount.IsActive
        rblRequirePasswordUpdate.SelectedValue = dbVendorAccount.RequirePasswordUpdate
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        If VendorAccountID = 0 OrElse VendorAccountRow.GetRow(DB, VendorAccountID).Username <> txtUsername.Text Then
            If Not VendorAccountRow.CheckUsernameAvailability(DB, txtUsername.Text) Then
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

            Dim dbVendorAccount As VendorAccountRow

            If VendorAccountID <> 0 Then
                dbVendorAccount = VendorAccountRow.GetRow(DB, VendorAccountID)
            Else
                dbVendorAccount = New VendorAccountRow(DB)
            End If

            dbVendorAccount.CRMID = txtCRMID.Text
            If txtHistoricID.Text <> Nothing Then dbVendorAccount.HistoricID = txtHistoricID.Text
            dbVendorAccount.FirstName = txtFirstName.Text
            dbVendorAccount.LastName = txtLastName.Text
            dbVendorAccount.Title = txtTitle.Text
            dbVendorAccount.Phone = txtPhone.Text
            dbVendorAccount.Mobile = txtMobile.Text
            dbVendorAccount.Fax = txtFax.Text
            dbVendorAccount.Email = txtEmail.Text
            dbVendorAccount.Username = txtUsername.Text
            dbVendorAccount.HistoricPasswordHash = txtHistoricPasswordHash.Text
            dbVendorAccount.HistoricPasswordSalt = txtHistoricPasswordSalt.Text
            dbVendorAccount.HistoricPasswordSha1 = txtHistoricPasswordSha1.Text
            If Not txtPassword.Text = String.Empty Then
                'log password change
                If Not dbVendorAccount.Password = txtPassword.Text Then
                    Core.DataLog("Admin", PageURL, CurrentUserId, "Update Vendor Password", UpdatedVendorAccountId, "", "", "", UserName)
                End If
                'end log
                dbVendorAccount.Password = txtPassword.Text
            End If
            dbVendorAccount.VendorID = drpVendorID.SelectedValue
            If drpVendorBranchOfficeID.SelectedValue <> Nothing Then
                dbVendorAccount.VendorBranchOfficeID = drpVendorBranchOfficeID.SelectedValue
            End If
            dbVendorAccount.IsPrimary = rblIsPrimary.SelectedValue
            'log If IsActive Changes
            If rblIsActive.SelectedValue = dbVendorAccount.IsActive Then
                dbVendorAccount.IsActive = rblIsActive.SelectedValue
            Else
                Core.DataLog("Admin", PageURL, CurrentUserId, "Update Vendor Active Status", UpdatedVendorAccountId, "", "", "", UserName)
                dbVendorAccount.IsActive = rblIsActive.SelectedValue
            End If
            'End loggig activity
            dbVendorAccount.RequirePasswordUpdate = rblRequirePasswordUpdate.SelectedValue
            If dbVendorAccount.Password = Nothing Then
                dbVendorAccount.RequirePasswordUpdate = True
            End If

            If dbVendorAccount.IsActive = False Then        'DELETE ALL ROLES FOR VENDOR ACCOUNT IF DEACTIVATED
                DB.ExecuteSQL("DELETE FROM VendorAccountVendorRole where VendorAccountID = " & dbVendorAccount.VendorAccountID)
            End If

            If VendorAccountID <> 0 Then
                dbVendorAccount.Update()

                'log If Vendor account update
                Core.DataLog("Admin", PageURL, CurrentUserId, "Update Vendor Account", UpdatedVendorAccountId, "", "", "", UserName)
                'end log

                Dim dtVendorAccountVendorRole = DB.ExecuteScalar("Select * from VendorAccount t1 join VendorAccountVendorRole t2  on t1.VendorAccountId=t2.VendorAccountId where t2.VendorRoleId in (1 ) and t1.VendorAccountId= " & DB.Number(VendorAccountID) & "")
                If Not UpdatedVendorAccountId = dtVendorAccountVendorRole Then
                    If rblIsPrimary.SelectedValue <> False Then
                        Dim dbVendorAccountVendorRole As VendorRoleRow
                        dbVendorAccountVendorRole = New VendorRoleRow(DB)
                        Dim VendorRoleId As Integer = 1
                        'VendorRoleRow.AssignVendorRole(DB, Session("VendorId"), UpdatedVendorAccountId, VendorRoleId)
                        SQL = "insert into VendorAccountVendorRole (VendorAccountID,VendorRoleID) values ( " & DB.Number(VendorAccountID) & " ,1)"
                        DB.ExecuteSQL(SQL)
                    End If
                ElseIf UpdatedVendorAccountId = dtVendorAccountVendorRole Then
                    'If rblIsPrimary.SelectedValue = False Then
If rblIsPrimary.SelectedValue = False OrElse rblIsActive.SelectedValue = False Then
                        SQL = "DELETE FROM VendorAccountVendorRole WHERE vendorRoleId=1 and VendorAccountID = " & DB.Number(VendorAccountID)
                        DB.ExecuteSQL(SQL)
                    End If
                End If

                    '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
                    '' Salesforce Integration
                    'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                    'If sfHelper.Login() Then
                    '    If sfHelper.UpsertVendorAccount(dbVendorAccount) = False Then
                    '        'throw error
                    '    End If
                    'End If
                    '===============================================================================================

                Else
                    VendorAccountID = dbVendorAccount.Insert()
                'log If Add Vendor account 
                NewVendorAccountId = dbVendorAccount.VendorAccountID
                    Core.DataLog("Admin", PageURL, CurrentUserId, "Add Vendor Account", NewVendorAccountId, "", "", "", UserName)
                'end log
                If rblIsPrimary.SelectedValue <> False Then
                    Dim dbVendorAccountVendorRole As VendorRoleRow
                    dbVendorAccountVendorRole = New VendorRoleRow(DB)
                    Dim VendorRoleId As Integer = 1
                    VendorRoleRow.InsertVendorRole(DB, Session("VendorId"), NewVendorAccountId, VendorRoleId)
                End If

                '=================== Apala (Medullus) - Sales Force related code 11.08.2017 ===================
                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.InsertVendorAccount(dbVendorAccount, VendorRow.GetRow(DB, dbVendorAccount.VendorID).CRMID) = False Then
                '        'throw error
                '    End If
                'End If
                '===============================================================================================
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
        Response.Redirect("delete.aspx?VendorAccountID=" & VendorAccountID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Function CheckIfCRMIDExists(ByRef strReturnMessage As String) As Boolean

        Dim strCRMID As String = txtCRMID.Text.Trim()

        If strCRMID = "" Then Return True

        Dim dbVendorAccRow As VendorAccountRow = VendorAccountRow.GetVendorAccountByCRMID(DB, strCRMID)
        Dim dbBuilderAccRow As BuilderAccountRow = BuilderAccountRow.GetBuilderAccountByCRMID(DB, strCRMID)

        If Not dbVendorAccRow Is Nothing Then
            If dbVendorAccRow.VendorAccountID <> 0 AndAlso dbVendorAccRow.VendorAccountID <> VendorAccountID Then

                Dim dbVendorRow As VendorRow = VendorRow.GetRow(DB, dbVendorAccRow.VendorID)
                Dim dbLLC As LLCRow = LLCRow.GetRow(DB, dbVendorRow.LLCID)

                Dim strAccountStatus As String = IIf(dbVendorAccRow.IsActive, "Active", "Inactive")
                Dim strAccountName As String = String.Concat(dbVendorAccRow.FirstName, " ", dbVendorAccRow.LastName)

                strReturnMessage = String.Concat("This CRM ID is already assigned to the ", strAccountStatus, " account of ", strAccountName, " with ", dbVendorRow.CompanyName, " in the ", dbLLC.LLC, " market. You cannot assign the same CRM ID to multiple accounts. Please determine why the other account is using this CRM ID and correct it in this app and/or in Insightly as necessary.")

                Return False

            ElseIf dbBuilderAccRow.BuilderAccountID <> 0 Then

                Dim dbBuilderRow As BuilderRow = BuilderRow.GetRow(DB, dbBuilderAccRow.BuilderID)
                Dim dbLLC As LLCRow = LLCRow.GetRow(DB, dbBuilderRow.LLCID)

                Dim strAccountStatus As String = IIf(dbBuilderAccRow.IsActive, "Active", "Inactive")
                Dim strAccountName As String = String.Concat(dbBuilderAccRow.FirstName, " ", dbBuilderAccRow.LastName)

                strReturnMessage = String.Concat("This CRM ID is already assigned to the ", strAccountStatus, " account of ", strAccountName, " with ", dbBuilderRow.CompanyName, " in the ", dbLLC.LLC, " market. You cannot assign the same CRM ID to multiple accounts. Please determine why the other account is using this CRM ID and correct it in this app and/or in Insightly as necessary.")

                Return False

            End If
        End If

        Return True

    End Function

End Class

