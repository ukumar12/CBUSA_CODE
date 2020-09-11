Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class EDIT
    Inherits AdminPage

    Protected AdminId As Integer
    Private PasswordEx As Boolean

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("USERS")
        PasswordEx = SysParam.GetValue(DB, "PasswordEx")

        If Not PasswordEx Then
            PASSWORDVALIDATOREX1.Visible = False
            PASSWORDVALIDATOREX2.Visible = False
            PasswordLengthValidatorEx.Visible = False
            trPasswordEx.Visible = False
            pnlPasswordEx.Visible = False
        End If

        AdminId = Convert.ToInt32(Request("AdminId"))
        If AdminId <> 0 Then
            'If Edit Mode, then turn off initially validators
            PASSWORDVALIDATOR1.Enabled = False
            PASSWORDVALIDATOR2.Enabled = False
            PasswordLengthValidator.Enabled = False
            trPassword.Visible = True

            'If Edit Mode, then turn off initially validators
            If PasswordEx Then
                PASSWORDVALIDATOREX1.Enabled = False
                PASSWORDVALIDATOREX2.Enabled = False
                PasswordLengthValidatorEx.Enabled = False
                trPasswordEx.Visible = True
            End If
        Else
            'If Add mode, then don't display delete and password row
            Delete.Visible = False
            trPassword.Visible = False
            trPasswordEx.Visible = False
        End If

        If Not IsPostBack Then
            If AdminId <> 0 Then
                Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, AdminId)
                Username.Text = dbAdmin.Username
                FIRSTNAME.Text = dbAdmin.FirstName
                LASTNAME.Text = dbAdmin.LastName
                EMAIL.Text = dbAdmin.Email
                chkIsLocked.Checked = dbAdmin.IsLocked
            End If
            BindPrivileges()
        Else
            PASSWORD1.Attributes.Add("value", Request("PASSWORD1"))
            PASSWORD2.Attributes.Add("value", Request("PASSWORD2"))

            If PasswordEx = True Then
                PASSWORDEX1.Attributes.Add("value", Request("PASSWORDEX1"))
                PASSWORDEX2.Attributes.Add("value", Request("PASSWORDEX2"))
            End If
        End If
    End Sub

    Private Sub BindPrivileges()
        'Bind Left List
        Dim dtLeft As DataTable = AdminAdminGroupRow.LoadGroupsWithoutPrivileges(DB, AdminId)
        lbLeft.DataSource = dtLeft.DefaultView
        lbLeft.DataValueField = "GroupId"
        lbLeft.DataTextField = "Description"
        lbLeft.DataBind()

        'This stores the excluded privillege groups
        Dim lbLeftListItem As String = String.Empty
        Dim leftcount As Integer = 0
        While leftcount < lbLeft.Items.Count
            lbLeftListItem &= lbLeft.Items(leftcount).Text
            leftcount += 1
        End While
        ViewState("LeftListItems") = lbLeftListItem

        'Bind Right List
        If AdminId <> 0 Then
            Dim dtRight As DataTable = AdminAdminGroupRow.LoadGroupsWithPrivileges(DB, AdminId)
            lbRight.DataSource = dtRight.DefaultView
            lbRight.DataValueField = "GroupId"
            lbRight.DataTextField = "Description"
            lbRight.DataBind()
        End If
    End Sub

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save.Click

        'If the user entered anything in the password fields
        'then turn on password validators
        If Not PASSWORD1.Text = String.Empty Or Not PASSWORD2.Text = String.Empty Then
            PASSWORDVALIDATOR1.Enabled = True
            PASSWORDVALIDATOR2.Enabled = True
            PasswordLengthValidator.Enabled = True
        End If

        If PasswordEx Then
            If Not PASSWORDEX1.Text = String.Empty Or Not PASSWORDEX2.Text = String.Empty Then
                PASSWORDVALIDATOREX1.Enabled = True
                PASSWORDVALIDATOREX2.Enabled = True
                PasswordLengthValidatorEx.Enabled = True
            End If
        End If
        Page.Validate()

        If Not IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            If AdminId <> 0 Then
                Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, AdminId)
                If Not Username.Text = dbAdmin.Username Then
                    Core.LogEvent("Username for admin user """ & dbAdmin.Username & """ was modified by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
                End If

                dbAdmin.Username = Username.Text
                If Not DB.IsEmpty(PASSWORD1.Text) Then
                    dbAdmin.Password = PASSWORD1.Text
                    Core.LogEvent("Primary password for admin user """ & dbAdmin.Username & """ was changed by """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
                End If
                If Not DB.IsEmpty(PASSWORDEX1.Text) Then
                    dbAdmin.PasswordEx = PASSWORDEX1.Text
                    Core.LogEvent("Secondary password for admin user """ & dbAdmin.Username & """ was changed by """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
                End If

                'Get the goups that are excluded
                Dim lbLeftNewListItem As String = String.Empty
                Dim leftcount As Integer = 0
                While leftcount < lbLeft.Items.Count
                    lbLeftNewListItem &= lbLeft.Items(leftcount).Text
                    leftcount += 1
                End While

                'This record should be only added when an update is done to the user groups
                If Not lbLeftNewListItem = ViewState("LeftListItems") Then
                    Core.LogEvent("Groups were updated for admin user """ & dbAdmin.Username & """ by user """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
                End If

                dbAdmin.FirstName = FIRSTNAME.Text
                dbAdmin.LastName = LASTNAME.Text
                dbAdmin.Email = EMAIL.Text
                If chkIsLocked.Checked = True And dbAdmin.IsLocked = False Then Core.LogEvent("Acount was locked for admin user """ & dbAdmin.Username & """ by """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
                dbAdmin.IsLocked = chkIsLocked.Checked
                dbAdmin.Update()
            Else
                Dim dbAdmin As AdminRow = New AdminRow(DB)
                dbAdmin.Username = Username.Text
                dbAdmin.Password = PASSWORD1.Text
                dbAdmin.PasswordEx = PASSWORDEX1.Text
                dbAdmin.FirstName = FIRSTNAME.Text
                dbAdmin.LastName = LASTNAME.Text
                dbAdmin.Email = EMAIL.Text
                dbAdmin.IsLocked = chkIsLocked.Checked
                AdminId = dbAdmin.AutoInsert()
                If AdminId > 0 Then Core.LogEvent("New admin with username """ & dbAdmin.Username & """ was created by """ & Session("Username") & """", Diagnostics.EventLogEntryType.Information)
            End If

            AdminAdminGroupRow.RemoveByAdmin(DB, AdminId)

            'INSERT ONLY SELECTED PRIVILEGES
            For Each item As ListItem In lbRight.Items
                Dim dbAdminAdminGroup As AdminAdminGroupRow = New AdminAdminGroupRow(DB)
                dbAdminAdminGroup.AdminId = AdminId
                dbAdminAdminGroup.GroupId = CInt(item.Value)
                dbAdminAdminGroup.Insert()
            Next
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub Delete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete.Click
        If AdminId <> 0 Then
            Response.Redirect("delete.aspx?AdminId=" & AdminId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

    Private Sub btnRight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRight.Click
        Dim item As ListItem
        Dim i As Integer = 0

        While i < lbLeft.Items.Count
            If lbLeft.Items(i).Selected Then
                item = lbLeft.Items(i)
                item.Selected = False
                Dim bInserted As Boolean = False
                For j As Integer = 0 To lbRight.Items.Count - 1
                    If lbRight.Items(j).Text > item.Text Then
                        lbRight.Items.Insert(j, item)
                        bInserted = True
                        Exit For
                    End If
                Next
                If Not bInserted Then
                    lbRight.Items.Add(item)
                End If
                lbLeft.Items.RemoveAt(i)
            Else
                i += 1
            End If
        End While
    End Sub

    Private Sub btnLeft_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLeft.Click
        Dim item As ListItem
        Dim i As Integer = 0

        While i < lbRight.Items.Count
            If lbRight.Items(i).Selected Then
                item = lbRight.Items(i)
                item.Selected = False
                Dim bInserted As Boolean = False
                For j As Integer = 0 To lbLeft.Items.Count - 1
                    If lbLeft.Items(j).Text > item.Text Then
                        lbLeft.Items.Insert(j, item)
                        bInserted = True
                        Exit For
                    End If
                Next
                If Not bInserted Then
                    lbLeft.Items.Add(item)
                End If
                lbRight.Items.RemoveAt(i)
            Else
                i += 1
            End If
        End While
    End Sub

End Class
