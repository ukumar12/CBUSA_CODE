Imports Components
Imports DataLayer
Imports Utilities

Partial Class modules_BuilderUsers
    Inherits ModuleControl
    Implements ICallbackEventHandler

    Private CallbackResult As String
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private UpdatedBuilderAccountId As String = ""
    Private NewBuilderAccountId As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ScriptManager.GetCurrent(Page).RegisterPostBackControl(frmEdit)
        ScriptManager.GetCurrent(Page).RegisterPostBackControl(frmRemoveRequest)

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderAccountId")
        UserName = Session("Username")

        If Not IsPostBack Then
            BindData()
            'BindRequestedData()
            Core.DataLog("Manage Users", PageURL, CurrentUserId, "Builder Left Menu Click", "", "", "", "", UserName)
        End If
    End Sub

    Private Sub BindRequestedData()
        gvRequestedUserAccounts.DataSource = BuilderAccountRow.GetBuilderAccounts(DB, Session("BuilderId"))
        gvRequestedUserAccounts.DataBind()
    End Sub

    Private Sub BindData()
        gvAccounts.DataSource = BuilderAccountRow.GetBuilderAccounts(DB, Session("BuilderId"), True)
        gvAccounts.DataBind()
    End Sub

    Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
        Return CallbackResult
    End Function

    Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent
        Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, eventArgument)
        Dim out As String = _
            "{'ID':'" & dbBuilderAccount.BuilderAccountID & "'" _
            & ",'FirstName':'" & dbBuilderAccount.FirstName & "'" _
            & ",'LastName':'" & dbBuilderAccount.LastName & "'" _
            & ",'Email':'" & dbBuilderAccount.Email & "'" _
            & "}"

        CallbackResult = out
    End Sub

    Protected Sub frmDelete_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmDelete.Postback
        Dim id As Integer = hdnConfirmID.Value
        If id <> Nothing Then

            DB.BeginTransaction()
            Dim dbBuilderAccountRow As BuilderAccountRow = BuilderAccountRow.GetRow(DB, id)
            If dbBuilderAccountRow.BuilderAccountID > 0 Then
                dbBuilderAccountRow.IsActive = False
                dbBuilderAccountRow.Update()
                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.DeleteBuilderAccount(dbBuilderAccountRow) = False Then
                '        'throw error
                '    End If
                'End If

                'log delete user
                UpdatedBuilderAccountId = dbBuilderAccountRow.BuilderAccountID
                Core.DataLog("Manage User", PageURL, CurrentUserId, "Delete Builder User Account", UpdatedBuilderAccountId, "", "", "", UserName)
                'end log
            End If
            DB.CommitTransaction()

            BuilderAccountRow.RemoveRow(DB, id)
        End If
        BindData()
    End Sub

    Protected Sub frmEdit_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmEdit.TemplateLoaded
        If Session("BuilderId") IsNot Nothing Then
            F_Role.DataSource = BuilderRoleRow.GetList(DB, "BuilderRoleID")
            F_Role.DataTextField = "BuilderRole"
            F_Role.DataValueField = "BuilderRoleID"
            F_Role.DataBind()
        End If
    End Sub

    Protected Sub frmEdit_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmEdit.Postback

        Page.Validate("UserReg")
        frmEdit.Validate()
        If Not Page.IsValid Or Not frmEdit.IsValid Then
            spanError.InnerHtml = CType(Page, SitePage).ErrorPlaceHolder.Message
            Exit Sub
        End If

        Dim BuilderAccountID As Integer = IIf(hdnBuilderAccountID.Value = String.Empty, 0, hdnBuilderAccountID.Value)
        Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, BuilderAccountID)

        DB.BeginTransaction()
        Try
            dbBuilderAccount.BuilderID = Session("BuilderId")
            dbBuilderAccount.FirstName = txtFirstName.Text
            dbBuilderAccount.LastName = txtLastName.Text
            Dim sendInvite As Boolean = (dbBuilderAccount.Email <> txtEmailAddress.Text)
            dbBuilderAccount.Email = txtEmailAddress.Text
            dbBuilderAccount.IsActive = True
            dbBuilderAccount.FlagForExistingUser = True

            If dbBuilderAccount.Username = String.Empty Then
                dbBuilderAccount.Username = VendorAccountRow.GetUniqueUserName(DB, txtFirstName.Text, txtLastName.Text)
                sendInvite = True
            End If
            If dbBuilderAccount.Password = String.Empty Then
                dbBuilderAccount.Password = Core.GenerateFileID().Substring(0, 7)
                dbBuilderAccount.RequirePasswordUpdate = True
                sendInvite = True
            End If

            If dbBuilderAccount.BuilderAccountID > 0 Then

                ''dbBuilderAccount.Update()

                'log update builder
                UpdatedBuilderAccountId = dbBuilderAccount.BuilderAccountID
                Core.DataLog("Manage User", PageURL, CurrentUserId, "Update Builder User Account", UpdatedBuilderAccountId, "", "", "", UserName)
                'end log

                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.UpsertBuilderAccount(dbBuilderAccount) = False Then
                '        'throw error
                '    End If
                'End If
            Else
                ''Dim BuilderID As Integer = dbBuilderAccount.Insert()

                'log builder account update
                NewBuilderAccountId = dbBuilderAccount.BuilderAccountID
                Core.DataLog("Manage User", PageURL, CurrentUserId, "Add Builder Account", NewBuilderAccountId, "", "", "", UserName)
                'end log

                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.InsertBuilderAccount(dbBuilderAccount, BuilderRow.GetRow(DB, Session("BuilderId")).CRMID) = False Then
                '        'throw error
                '    End If
                'End If
            End If

            ''dbBuilderAccount.DeleteFromBuilderRoles()
            ''dbBuilderAccount.InsertToBuilderAccountBuilderRoles(F_Role.SelectedValues)
            DB.CommitTransaction()


            ''''''''''''' indranil added '
            Dim iBuilderID As Integer = Session("BuilderId")
            Dim sBuilderCompanyName As String = DB.ExecuteScalar("select CompanyName from Builder where BuilderID = " & iBuilderID)
            Response.Write(F_Role.SelectedValues)
            If (F_Role.SelectedValues = "") Then
                Dim sMsg As String =
                 sBuilderCompanyName & " has requested to add the following account user" & ":" & vbCrLf & vbCrLf _
               & "First Name :" & vbTab & dbBuilderAccount.FirstName & vbCrLf _
               & "Last Name :" & vbTab & dbBuilderAccount.LastName & vbCrLf _
               & "Email ID :" & vbTab & dbBuilderAccount.Email & vbCrLf

                Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "arrowdev@medullus.com", "Test", "Request to Add a New Account User", sMsg)
                'Core.se
            Else
                Dim sMsg As String =
                 sBuilderCompanyName & " has requested to add the following account user" & ":" & vbCrLf & vbCrLf _
               & "First Name :" & vbTab & dbBuilderAccount.FirstName & vbCrLf _
               & "Last Name :" & vbTab & dbBuilderAccount.LastName & vbCrLf _
               & "Email ID :" & vbTab & dbBuilderAccount.Email & vbCrLf _
               & "Role :" & vbTab & F_Role.SelectedItem.Text & vbCrLf

                Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "arrowdev@medullus.com", "Test", "Request to Add a New Builder User Account", sMsg)
            End If

            BindData()
            BindRequestedData()
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            Logger.Error(Logger.GetErrorMessage(ex))
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Sub SendInvitation(ByVal dbBuilderAccount As BuilderAccountRow)
        Dim name As String = Core.BuildFullName(dbBuilderAccount.FirstName, "", dbBuilderAccount.LastName)
        Dim msg As String = name & "," & vbCrLf & vbCrLf
        msg &= "A new account has been created for you at " & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName") & " .  Your username and password are shown below." & vbCrLf & vbCrLf
        msg &= "Log in to finish setting up your account and to access the CBUSA website." & vbCrLf & vbCrLf
        msg &= "Username: " & dbBuilderAccount.Username & vbCrLf
        msg &= "Password: " & dbBuilderAccount.Password & vbCrLf & vbCrLf

        Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        Core.SendSimpleMail(FromAddress, FromName, dbBuilderAccount.Email, name, "CBUSA Builder Account Registration", msg)
    End Sub

    Protected Sub btnEditUser_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim BuilderAccountID As Integer = CType(sender, ImageButton).CommandArgument
        Dim dbBuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, BuilderAccountID)

        txtEmailAddress.Text = dbBuilderAccount.Email
        txtFirstName.Text = dbBuilderAccount.FirstName
        txtLastName.Text = dbBuilderAccount.LastName
        hdnBuilderAccountID.Value = dbBuilderAccount.BuilderAccountID
        F_Role.ClearSelection()
        F_Role.SelectedValues = dbBuilderAccount.GetSelectedRoles


        upEdit.Update()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenEditForm", "Sys.Application.add_load(OpenEditForm);", True)
    End Sub

    Protected Sub gvAccounts_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvAccounts.RowCreated
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        'Dim btnEdit As ImageButton = e.Row.FindControl("btnEdit")
        'ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnEdit)
        'AddHandler btnEdit.Click, AddressOf btnEditUser_Click
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As System.EventArgs) Handles btnAddUser.Click


        txtEmailAddress.Text = String.Empty
        txtFirstName.Text = String.Empty
        txtLastName.Text = String.Empty
        hdnBuilderAccountID.Value = 0
        F_Role.ClearSelection()
        '  F_Role.SelectedValues = dbBuilderAccount.GetSelectedRoles
        upEdit.Update()

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenEditForm", "Sys.Application.add_load(OpenEditForm);", True)
    End Sub

    Protected Sub gvAccounts_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvAccounts.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub
        Dim ltlBuilderRoles As Literal = CType(e.Row.FindControl("ltlBuilderRoles"), Literal)

        ltlBuilderRoles.Text = BuilderAccountRow.GetBuilderAccountRoles(DB, e.Row.DataItem("BuilderAccountID"))

    End Sub

    Protected Sub gvRequestedUserAccounts_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvRequestedUserAccounts.RowDataBound
        If Not e.Row.RowType = DataControlRowType.DataRow Then Exit Sub
        Dim ltlBuilderRoles As Literal = CType(e.Row.FindControl("ltlBuilderRoles"), Literal)

        ltlBuilderRoles.Text = BuilderAccountRow.GetBuilderAccountRoles(DB, e.Row.DataItem("BuilderAccountID"))

    End Sub

    Private Sub btnRemoveUser_Click(sender As Object, e As EventArgs) Handles btnRemoveUser.Click

        Dim dtBuilderAccount As DataTable = BuilderAccountRow.GetBuilderAccounts(DB, Session("BuilderId"), True)

        Dim qBuilderAccounts As IEnumerable
        qBuilderAccounts = (From dr As DataRow In dtBuilderAccount.AsEnumerable
                            Select New With {
                                        .BuilderAccountId = dr("BuilderAccountId"),
                                        .BuilderAccountName = String.Concat(dr("FirstName"), " ", dr("LastName"))
                                })

        Dim dtCustomBuilderAccount As DataTable = EQToDataTable(qBuilderAccounts)

        chkbxLstBuilderAccounts.DataSource = dtCustomBuilderAccount
        chkbxLstBuilderAccounts.DataTextField = "BuilderAccountName"
        chkbxLstBuilderAccounts.DataValueField = "BuilderAccountId"
        chkbxLstBuilderAccounts.DataBind()

        upRemoveRequest.Update()

        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenRemoveForm", "Sys.Application.add_load(OpenRemoveForm);", True)

    End Sub

    Public Function EQToDataTable(ByVal parIList As System.Collections.IEnumerable) As System.Data.DataTable
        Dim ret As New System.Data.DataTable()
        Try
            Dim ppi As System.Reflection.PropertyInfo() = Nothing
            If parIList Is Nothing Then Return ret
            For Each itm In parIList
                If ppi Is Nothing Then
                    ppi = DirectCast(itm.[GetType](), System.Type).GetProperties()
                    For Each pi As System.Reflection.PropertyInfo In ppi
                        Dim colType As System.Type = pi.PropertyType
                        If (colType.IsGenericType) AndAlso (colType.GetGenericTypeDefinition() Is GetType(System.Nullable(Of ))) Then colType = colType.GetGenericArguments()(0)
                        ret.Columns.Add(New System.Data.DataColumn(pi.Name, colType))
                    Next
                End If
                Dim dr As System.Data.DataRow = ret.NewRow
                For Each pi As System.Reflection.PropertyInfo In ppi
                    dr(pi.Name) = If(pi.GetValue(itm, Nothing) Is Nothing, DBNull.Value, pi.GetValue(itm, Nothing))
                Next
                ret.Rows.Add(dr)
            Next
            For Each c As System.Data.DataColumn In ret.Columns
                c.ColumnName = c.ColumnName.Replace("_", " ")
            Next
        Catch ex As Exception
            ret = New System.Data.DataTable()
        End Try
        Return ret
    End Function

    Private Sub frmRemoveRequest_Postback(sender As Object, e As EventArgs) Handles frmRemoveRequest.Postback

        Try
            Dim BuilderID As Integer = Session("BuilderId")
            Dim BuilderCompanyName As String = DB.ExecuteScalar("SELECT CompanyName FROM Builder WHERE BuilderID = " & BuilderID)
            Dim HasAccountSelected As Boolean = False
            Dim SelectAccounts As String = String.Empty

            For Each chkBoxBuilderAccount As ListItem In chkbxLstBuilderAccounts.Items
                If chkBoxBuilderAccount.Selected = True Then
                    HasAccountSelected = True
                    SelectAccounts = SelectAccounts & chkBoxBuilderAccount.Text & vbCrLf
                End If
            Next

            If HasAccountSelected Then
                Dim sMsg As String = String.Concat(BuilderCompanyName, " has requested to remove the following account(s): ", vbCrLf & vbCrLf, SelectAccounts)

                Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "abasu@medullus.com", "abasu@medullus.com", "Request to Remove Builder Account", sMsg)
            End If

            BindData()

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub
End Class
