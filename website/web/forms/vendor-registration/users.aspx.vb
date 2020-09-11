Imports Components
Imports DataLayer
Imports System.Web.Services
Imports Utility
Imports System.Linq
Imports System.Data
Imports Utilities
Imports System.Configuration.ConfigurationManager
Imports System.Net
Imports System.IO

Partial Class forms_vendor_registration_terms
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorRegistration As VendorRegistrationRow

    Private m_dvRoles As DataView
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private UpdatedVendorAccountId As String = ""
    Private NewVendorAccountId As String = ""

    Private ReadOnly Property dvRoles() As DataView
        Get
            If m_dvRoles Is Nothing Then
                m_dvRoles = VendorAccountRow.GetAllVendorUserRoles(DB, VendorId).DefaultView
            End If
            Return m_dvRoles
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("VendorId") <> 0 Then
            VendorId = CType(Session("VendorId"), Integer)
            dbVendor = VendorRow.GetRow(Me.DB, VendorId)
            dbVendorRegistration = VendorRegistrationRow.GetRowByVendor(Me.DB, VendorId, Now.Year)
        Else
            Response.Redirect("/default.aspx")
        End If

        If Session("VendorAccountId") <> 0 Then
            VendorAccountId = CType(Session("VendorAccountId"), Integer)
            dbVendorAccount = VendorAccountRow.GetRow(Me.DB, VendorAccountId)
        Else
            Response.Redirect("/default.aspx")
        End If

        If dbVendorRegistration.CompleteDate <> Nothing AndAlso dbVendorRegistration.CompleteDate.Year = Now.Year Then
            ctlSteps.Visible = False
            btnContinue.Visible = False
            btnBack.Visible = False
        Else
            btnDashboard.Visible = False
            btnBack.Visible = True
        End If

        Dim bChanged As Boolean = False

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorAccountId")
        UserName = dbVendorAccount.Username

        If Not IsPostBack And Not ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
            LoadFromDB()
            LoadFromDBRequestedUser()
            Core.DataLog("Edit Account Information", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)

            Try
                If Not Session("errVendorUserRoles") Is Nothing AndAlso Session("errVendorUserRoles") <> String.Empty Then
                    AddError(Session("errVendorUserRoles"))
                    Session("errVendorUserRoles") = Nothing
                End If
            Catch ex As Exception
            End Try
        End If

    End Sub

    Private Sub LoadFromDB()
        Dim dt As DataTable

        'Users
        dt = VendorAccountRow.GetListByVendor(Me.DB, VendorId, ShowActiveOnly:=True)
        Me.rptUsers.DataSource = dt
        Me.rptUsers.DataBind()

    End Sub

    Private Sub LoadFromDBRequestedUser()
        Dim dt As DataTable

        'Users
        dt = VendorAccountRow.GetListByVendor(Me.DB, VendorId)
        Me.rptRequestedUser.DataSource = dt
        Me.rptRequestedUser.DataBind()
    End Sub


    Protected Sub rptUsers_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptUsers.ItemCommand
        Dim VendorAccountID As Integer = e.CommandArgument
        Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, VendorAccountID)

        txtFirstName.Text = dbVendorAccount.FirstName
        txtLastName.Text = dbVendorAccount.LastName

        'ScriptManager.RegisterStartupScript(Me, Me.GetType, "OpenForm", "window.setTimeout(OpenForm,1000);", True)
    End Sub

    Protected Sub rptUsers_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUsers.ItemCreated
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        'Dim btnEditUser As Button = e.Item.FindControl("btnEditUser")
        'ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnEditUser)
    End Sub


    Protected Sub rptUsers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUsers.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        'Dim btnEditUser As Button = e.Item.FindControl("btnEditUser")
        'btnEditUser.CommandArgument = e.Item.DataItem("VendorAccountID")

        'Dim ltrUserRoles As Literal = e.Item.FindControl("ltrUserRoles")
        'dvRoles.RowFilter = "VendorAccountID=" & e.Item.DataItem("VendorAccountID")
        'If dvRoles.Count = 0 Then
        '    ltrUserRoles.Text = "None"
        'Else
        '    ltrUserRoles.Text = String.Empty
        '    For Each dr As DataRowView In dvRoles
        '        ltrUserRoles.Text &= dr("VendorRole") & "<br/>"
        '    Next
        'End If
    End Sub

    'Protected Sub cvtxtUsername_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvftxtUsername.ServerValidate
    '    Dim Username As String = DB.ExecuteScalar("SELECT Username FROM VendorAccount WHERE Username = " & DB.Quote(txtUsername.Text))
    '    args.IsValid = (Username = String.Empty)
    'End Sub

    Protected Sub frmEdit_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmEdit.Postback
        frmEdit.Validate()

        Dim VendorAccountID As Integer = hdnVendorAccountID.Value
        Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, VendorAccountID)

        Dim bValid As Boolean = frmEdit.IsValid

        If Not bValid Then
            Exit Sub
        End If

        DB.BeginTransaction()
        Try
            dbVendorAccount.VendorID = Session("VendorID")
            dbVendorAccount.FirstName = txtFirstName.Text
            dbVendorAccount.LastName = txtLastName.Text
            Dim sendInvite As Boolean = (dbVendorAccount.Email <> txtEmailAddress.Text)
            dbVendorAccount.Email = txtEmailAddress.Text
            If dbVendorAccount.Username = String.Empty Then
                dbVendorAccount.Username = VendorAccountRow.GetUniqueUserName(DB, txtFirstName.Text, txtLastName.Text)
                sendInvite = True
            End If
            If dbVendorAccount.Password = String.Empty Then
                dbVendorAccount.Password = Core.GenerateFileID().Substring(0, 7)
                dbVendorAccount.RequirePasswordUpdate = False
                sendInvite = True
            End If

            If dbVendorAccount.VendorAccountID > 0 Then
                ''dbVendorAccount.Update()

                'Log Update User
                UpdatedVendorAccountId = dbVendorAccount.VendorAccountID
                Core.DataLog("Vendor Registration", PageURL, CurrentUserId, "Update User", UpdatedVendorAccountId, "", "", "", UserName)

                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.UpsertVendorAccount(dbVendorAccount) = False Then
                '        'throw error
                '    End If
                'End If

            Else
                dbVendorAccount.IsPrimary = False
                dbVendorAccount.IsActive = True
                dbVendorAccount.FlagForExistingUser = True
                ''dbVendorAccount.Insert()

                'Log Create User
                NewVendorAccountId = dbVendorAccount.VendorAccountID
                Core.DataLog("Vendor Registration", PageURL, CurrentUserId, "Create User", NewVendorAccountId, "", "", "", UserName)
                'End logging activity

                '' Salesforce Integration
                'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
                'If sfHelper.Login() Then
                '    If sfHelper.InsertVendorAccount(dbVendorAccount, VendorRow.GetRow(DB, Session("VendorID")).CRMID) = False Then
                '        'throw error
                '    End If
                'End If
            End If

            DB.CommitTransaction()

            'If sendInvite Then SendInvitation(dbVendorAccount)



            ''''''''''''''' indranil added
            '

            Dim iVendorID As Integer = Session("VendorID")
            Dim sVendorCompanyName As String = DB.ExecuteScalar("select CompanyName from Vendor where VendorID = " & iVendorID)

            Dim sMsg As String =
                  sVendorCompanyName & " has requested to add the following account user" & ":" & vbCrLf & vbCrLf _
                & "First Name :" & vbTab & dbVendorAccount.FirstName & vbCrLf _
                & "Last Name :" & vbTab & dbVendorAccount.LastName & vbCrLf _
                & "Email ID :" & vbTab & dbVendorAccount.Email & vbCrLf

            Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "arrowdev@medullus.com", "Test", "Request to Add a New Vendor User Account", sMsg)


            Response.Redirect("users.aspx")

            LoadFromDB()
            LoadFromDBRequestedUser()
            upResults.Update()
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            Logger.Error(Logger.GetErrorMessage(ex))
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Private Sub SendInvitation(ByVal dbVendorAccount As VendorAccountRow)
        Dim name As String = Core.BuildFullName(dbVendorAccount.FirstName, "", dbVendorAccount.LastName)
        Dim msg As String = name & "," & vbCrLf & vbCrLf
        msg &= "A new account has been created for you at " & GlobalRefererName & " .  Your username and password are shown below." & vbCrLf & vbCrLf
        msg &= "Log in to finish setting up your account and to access the CBUSA website." & vbCrLf & vbCrLf
        msg &= "Username: " & dbVendorAccount.Username & vbCrLf
        msg &= "Password: " & dbVendorAccount.Password & vbCrLf & vbCrLf

        Dim FromAddress As String = SysParam.GetValue(DB, "ContactUsEmail")
        Dim FromName As String = SysParam.GetValue(DB, "ContactUsName")
        Core.SendSimpleMail(FromAddress, FromName, dbVendorAccount.Email, name, "CBUSA Vendor Account Registration", msg)

        'Log user creation mail
        NewVendorAccountId = dbVendorAccount.VendorAccountID
        Core.DataLog("Vendor Registration", PageURL, CurrentUserId, "Mail Sent To Created User", NewVendorAccountId, "", "", "", UserName)
        'End logging activity
    End Sub

    Protected Sub frmEdit_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmEdit.TemplateLoaded

    End Sub

    <WebMethod()>
    Public Shared Function LoadAccount(ByVal VendorAccountID As Integer) As String
        Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetRow(GlobalDB.DB, VendorAccountID)
        Dim out As String =
            "{'ID':'" & dbVendorAccount.VendorAccountID & "'" _
            & ",'FirstName':'" & Replace(dbVendorAccount.FirstName, "'", "\'") & "'" _
            & ",'LastName':'" & Replace(dbVendorAccount.LastName, "'", "\'") & "'" _
            & ",'Email':'" & Replace(dbVendorAccount.Email, "'", "\'") & "'" _
            & "}"

        Return out
    End Function

    Private Function Process() As Boolean
        Dim dtRoles As DataTable = VendorRoleRow.GetVendorRoles(DB, Session("VendorID"))
        Dim bValid As Boolean = True
        For Each row As DataRow In dtRoles.Rows
            If IsDBNull(row("VendorAccountID")) Then
                bValid = False
                AddError("Please assign a user to role '" & row("VendorRole") & "'")
            End If
        Next

        If Not bValid Then
            AddError("Please select a user for every role.")
            Return False
        End If

        Try
            If dbVendorRegistration.CompleteDate = Nothing Then
                Dim dbStatus As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 3)
                dbVendorRegistration.RegistrationStatusID = dbStatus.RegistrationStatusID
                dbVendorRegistration.Update()
            End If
            Return True
        Catch ex As SqlClient.SqlException
            AddError(ErrHandler.ErrorText(ex))
            Return False
        End Try
    End Function

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        If Process() Then
            Response.Redirect("/rebates/terms.aspx?guid=" & dbVendor.GUID)
        End If
    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        If Process() Then
            Response.Redirect("/vendor/default.aspx")
        End If
    End Sub

    Protected Sub frmDelete_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmDelete.Postback
        Dim id As Integer = hdnConfirmID.Value
        If id <> Nothing Then
            Dim dbAcountVendor As VendorAccountRow = VendorAccountRow.GetRow(DB, id)
            dbAcountVendor.IsActive = False
            dbAcountVendor.Update()
            DB.ExecuteSQL("DELETE FROM VendorAccountVendorRole where VendorAccountID = " & DB.Number(id))
            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If sfHelper.DeleteAccountVendor(dbAcountVendor) = False Then
            '        'throw error
            '    End If
            'End If

            VendorAccountRow.RemoveRow(DB, id)

            'log user delete activity
            UpdatedVendorAccountId = dbAcountVendor.VendorAccountID
            Core.DataLog("Vendor Registration", PageURL, CurrentUserId, "Delete User", UpdatedVendorAccountId, "", "", "", UserName)
            'End delete Activity
        End If
        LoadFromDB()
        ctlRoles.Refresh()

    End Sub

    Private Sub frmRemoveRequest_Postback(sender As Object, e As EventArgs) Handles frmRemoveRequest.Postback

        Try
            Dim VendorID As Integer = Session("VendorId")
            Dim VendorCompanyName As String = DB.ExecuteScalar("SELECT CompanyName FROM Vendor WHERE VendorID = " & VendorID)
            Dim HasAccountSelected As Boolean = False
            Dim SelectAccounts As String = String.Empty

            For Each chkBoxVendorAccount As ListItem In chkbxLstVendorAccounts.Items
                If chkBoxVendorAccount.Selected = True Then
                    HasAccountSelected = True
                    SelectAccounts = SelectAccounts & chkBoxVendorAccount.Text & vbCrLf
                End If
            Next

            If HasAccountSelected Then
                Dim sMsg As String = String.Concat(VendorCompanyName, " has requested to remove the following account(s): ", vbCrLf & vbCrLf, SelectAccounts)

                Core.SendSimpleMail("customerservice@cbusa.us", "CBUSA", "pdas@medullus.com", "pdas@medullus.com", "Request to Remove Vendor Account", sMsg)
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub

    Private Sub btnRemoveUser_Click(sender As Object, e As EventArgs) Handles btnRemoveUser.Click

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

    Private Sub frmRemoveRequest_TemplateLoaded(sender As Object, e As EventArgs) Handles frmRemoveRequest.TemplateLoaded

        chkbxLstVendorAccounts.Items.Clear()

        Dim dtVendorAccount As DataTable = VendorAccountRow.GetListByVendor(Me.DB, VendorId, ShowActiveOnly:=True)

        Dim qVendorAccounts As IEnumerable
        qVendorAccounts = (From dr As DataRow In dtVendorAccount.AsEnumerable
                           Select New With {
                                    .VendorAccountId = dr("VendorAccountId"),
                                    .VendorAccountName = String.Concat(dr("FirstName"), " ", dr("LastName"))
                            })

        Dim dtCustomVendorAccount As DataTable = EQToDataTable(qVendorAccounts)

        chkbxLstVendorAccounts.DataSource = dtCustomVendorAccount
        chkbxLstVendorAccounts.DataTextField = "VendorAccountName"
        chkbxLstVendorAccounts.DataValueField = "VendorAccountId"
        chkbxLstVendorAccounts.DataBind()

    End Sub

    <WebMethod()>
    Public Shared Function UpdateAndSyncVendorRoles(ByVal RoleId As String, ByVal AccountIDs As String) As String
        Dim DB As New Database
        Try
            Dim Connectionstring As String = DBConnectionString.GetConnectionString(AppSettings("ConnectionString"), AppSettings("ConnectionStringUsername"), AppSettings("ConnectionStringPassword"))

            DB.Open(Connectionstring)

            Dim VendorID As Integer = DB.Number(HttpContext.Current.Session("VendorID"))
            If (Not String.IsNullOrEmpty(RoleId)) Then

                VendorRoleRow.ClearVendorRole(DB, VendorID, RoleId)
                If Not String.IsNullOrEmpty(AccountIDs) Then
                    For Each itm As String In AccountIDs.Split(",")
                        VendorRoleRow.InsertVendorRole(DB, VendorID, itm, RoleId)
                    Next
                End If


                Dim sql As String = "   SELECT va.CRMID,Isnull(vr.VendorRole,'No Role') VendorRole FROM VendorAccountVendorRole vavr " &
                            "   INNER JOIN VendorRole	  vr	ON vavr.VendorRoleID		=		vr.VendorRoleID " &
                            "   right outer join   VendorAccount va	ON vavr.VendorAccountID		=		va.VendorAccountID " &
                            "   WHERE va.VendorID		    =	" & VendorID & " " &
                            "   AND   va.IsActive			=	1"
                Dim dtVendorRole As DataTable = DB.GetDataTable(sql)
                Dim vendorRoleList As New List(Of VendorRoleCrm)()
                If dtVendorRole.Rows.Count > 0 Then
                    For Each row As DataRow In dtVendorRole.Rows

                        If vendorRoleList.Count <= 0 Then
                            vendorRoleList.Add(New VendorRoleCrm With {
                        .CRMID = row("CRMID").ToString(),
                        .VendorRoleName = row("VendorRole").ToString()
                    })
                        Else
                            Dim result = vendorRoleList.Where(Function(x) x.CRMID = row("CRMID").ToString()).FirstOrDefault()
                            If (Not result Is Nothing) Then
                                result.VendorRoleName = result.VendorRoleName + ";" + row("VendorRole").ToString()
                            Else
                                vendorRoleList.Add(New VendorRoleCrm With {
                                                            .CRMID = row("CRMID").ToString(),
                                                            .VendorRoleName = row("VendorRole")
                                                        })
                            End If
                        End If
                    Next
                    ''Sync records in CRM
                    For Each itm In vendorRoleList
                        Dim strBody As String = "{""CONTACT_ID"":" & itm.CRMID & ",""CUSTOMFIELDS"":[{""FIELD_NAME"":""Contact_Roles2__c"",""FIELD_VALUE"":""" & itm.VendorRoleName & """}]}"
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3
                        Dim req1 As WebRequest = WebRequest.Create("https://api.insightly.com/v3.1/Contacts")
                        req1.Method = "PUT"
                        req1.Headers("Authorization") = "Basic ZDgyNDdjNzAtYWIyZC00NDlkLTllMGMtNzViODAxODBkZTkyOg=="
                        req1.ContentLength = strBody.Length

                        If (Not strBody Is Nothing) Then
                            Dim postBytes = Encoding.ASCII.GetBytes(strBody)
                            req1.ContentLength = postBytes.Length
                            Dim requestStream As Stream = req1.GetRequestStream()
                            requestStream.Write(postBytes, 0, postBytes.Length)
                        End If
                        Using resp As HttpWebResponse = TryCast(req1.GetResponse(), HttpWebResponse)
                            If (resp.StatusCode = HttpStatusCode.OK) Then
                            End If
                        End Using
                    Next
                End If
            End If
        Catch ex As Exception
            Return "false"
        Finally
            If Not DB Is Nothing Then DB.Close()
        End Try
        Return "true"

    End Function



End Class

Public Class VendorRoleCrm
    Public Property CRMID As String
    Public Property VendorRoleName As String

End Class
