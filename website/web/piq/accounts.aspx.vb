Imports Components
Imports DataLayer

Partial Class piq_accounts
    Inherits SitePage


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsurePIQAccess()

        ScriptManager.GetCurrent(Page).RegisterPostBackControl(frmEdit)
        If Not IsPostBack Then

            BindData()
        End If
    End Sub

    Private Sub BindData()

        gvAccounts.DataSource = PIQAccountRow.GetListByPIQ(DB, Session("PIQId"), "LastName")
        gvAccounts.DataBind()
    End Sub

    Protected Sub gvAccounts_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvAccounts.RowCreated
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim btnEdit As ImageButton = e.Row.FindControl("btnEdit")
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnEdit)
        AddHandler btnEdit.Click, AddressOf btnEdit_Click
    End Sub

    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Dim PIQAccountID As Integer = CType(sender, ImageButton).CommandArgument
        Dim dbAccount As PIQAccountRow = PIQAccountRow.GetRow(DB, PIQAccountID)

        txtEmailAddress.Text = dbAccount.Email
        txtFirstName.Text = dbAccount.FirstName
        txtLastName.Text = dbAccount.LastName
        hdnAccountID.Value = dbAccount.PIQAccountID
        cblLLC.ClearSelection()
        cblLLC.SelectedValues = dbAccount.GetSelectedLLCs

        upEdit.Update()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "OpenEditForm", "Sys.Application.add_load(OpenEditForm);", True)
    End Sub

    Protected Sub btnConfirmDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmDelete.Click
        Dim PIQAccountID As Integer = hdnConfirmID.Value

        DB.ExecuteSQL("DELETE FROM PIQAccountLLC WHERE PIQAccountId = " & PIQAccountID)

        PIQAccountRow.RemoveRow(DB, PIQAccountID)
        BindData()
    End Sub

    Private Sub SendNotification(ByVal Email As String, PIQName As String, ByVal Name As String, ByVal Username As String)
        Dim msg As String = PIQName & " has added a New account for " & Name & " ( " & Username & ", " & Email & " ) "

        Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        Core.SendSimpleMail(FromAddress, FromName, Convert.ToString(ConfigurationManager.AppSettings("piqAddMailId")), Name, "New PIQ Account Added", msg)


        Page.ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "window.setTimeout(""location.href='/default.aspx';"",30000);", True)
    End Sub

    Private Sub btnEditSave_Click(sender As Object, e As EventArgs) Handles btnEditSave.Click

        If sender.ID = "btnEditCancel" Then
            Response.Redirect("~/piq/accounts.aspx")
            Exit Sub
        End If

        Dim PIQAccountID As Integer = IIf(hdnAccountID.Value = String.Empty, 0, hdnAccountID.Value)
        Dim dbAccount As PIQAccountRow = PIQAccountRow.GetRow(DB, PIQAccountID)

        Try
            DB.BeginTransaction()

            dbAccount.PIQID = Session("PIQId")

            Dim sendInvite As Boolean = False
            If dbAccount.Username = String.Empty Then
                dbAccount.Username = VendorAccountRow.GetUniqueUserName(DB, txtFirstName.Text, txtLastName.Text)
                sendInvite = True
            End If
            If dbAccount.Password = String.Empty Then
                dbAccount.Password = Core.GenerateFileID().Substring(0, 7)
                'Removed RequirePasswordUpdate for new accounts  - #214042 
                'dbAccount.RequirePasswordUpdate = True
                sendInvite = True
            End If
            If txtEmailAddress.Text <> dbAccount.Email Then
                sendInvite = True
            End If

            dbAccount.FirstName = txtFirstName.Text
            dbAccount.LastName = txtLastName.Text
            dbAccount.Email = txtEmailAddress.Text

            If dbAccount.PIQAccountID = Nothing Then
                dbAccount.Insert()
                Dim PIQName As String = DB.ExecuteScalar("select CompanyName from Piq where PIQId = " & dbAccount.PIQID)
                SendNotification(dbAccount.Email, PIQName, txtFirstName.Text & " " & txtLastName.Text, dbAccount.Username)
                'Convert.ToString(ConfigurationManager.AppSettings("piqAddMailId"))
            Else
                dbAccount.Update()
            End If

            dbAccount.DeleteFromAllLLCs()
            If cblLLC.SelectedValues.Length > 0 Then
                dbAccount.InsertToLLCs(cblLLC.SelectedValues)
            End If

            DB.CommitTransaction()

            If sendInvite Then
                Dim sMsg As String = _
                      "A User Account has been created for you at " & GlobalRefererName & ":" & vbCrLf & vbCrLf _
                    & "Username:" & vbTab & dbAccount.Username & vbCrLf _
                    & "Password:" & vbTab & dbAccount.Password & vbCrLf

                Core.SendSimpleMail(SysParam.GetValue(DB, "ContactUsEmail"), SysParam.GetValue(DB, "ContactUsFrom"), dbAccount.Email, Core.BuildFullName(dbAccount.FirstName, "", dbAccount.LastName), "Your CBUSA Account", sMsg)
            End If

            BindData()
        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            Logger.Error(Logger.GetErrorMessage(ex))
        End Try
    End Sub



    Protected Sub frmEdit_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmEdit.TemplateLoaded
        cblLLC.DataSource = DB.GetDataTable("Select * from LLC where Isactive =1 Order by LLC")
        cblLLC.DataTextField = "LLC"
        cblLLC.DataValueField = "LLCId"
        cblLLC.DataBind()
    End Sub


End Class
