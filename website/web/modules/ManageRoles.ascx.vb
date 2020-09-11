Imports System.Web.Services
Imports Components
Imports DataLayer


Partial Class modules_ManageRoles
    Inherits ModuleControl
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private dtAccounts As DataTable


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            dtAccounts = DB.GetDataTable("select Username as FullName, VendorAccountID from VendorAccount where VendorID=" & DB.Number(Session("VendorID")) & " and IsActive = 1")
            BindData()
        End If
        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorAccountId")
        UserName = Session("Username")
    End Sub

    Private Sub BindData()
        Dim dtRoles As DataTable = DB.GetDataTable("select * from VendorRole")

        rptRoles.DataSource = dtRoles ' VendorRoleRow.GetVendorRoles(DB, Session("VendorId"))
        rptRoles.DataBind()
    End Sub



    Protected Sub rptRoles_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRoles.ItemDataBound
        If e.Item.ItemType <> ListItemType.Item And e.Item.ItemType <> ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Dim hdnRoleId As HiddenField = e.Item.FindControl("hdnRoleID")
        hdnRoleId.Value = e.Item.DataItem("VendorRoleID")

        Dim drpUsers As ListBox = e.Item.FindControl("drpUsers")
        drpUsers.DataSource = dtAccounts
        drpUsers.DataTextField = "FullName"
        drpUsers.DataValueField = "VendorAccountID"
        drpUsers.DataBind()

        Dim dtVendorRoleAccounts As DataTable = DB.GetDataTable(" SELECT * from VendorAccountVendorRole where VendorAccountID IN (select VendorAccountID from VendorAccount where VendorID=" & DB.Number(Session("VendorId")) & ") AND VendorRoleID= " & hdnRoleId.Value & "")
        If dtVendorRoleAccounts.Rows.Count > 0 Then
            For Each itm As ListItem In drpUsers.Items
                If dtVendorRoleAccounts.Select("VendorAccountId=" & itm.Value & "").Count > 0 Then
                    itm.Selected = True
                End If
            Next
        End If

    End Sub

    Private Sub drpSaveRole(ByVal sender As Object, ByVal e As EventArgs)
        Dim drpUsers As DropDownList = sender
        Dim item As RepeaterItem = drpUsers.NamingContainer
        Dim ltlSaved As Literal = item.FindControl("ltlSaved")
        Dim VendorRoleId As Integer = drpUsers.Attributes("VendorRoleId")
        Dim Change_VendorAccountId As Integer = drpUsers.Attributes("VendorAccountId")

        Dim hdnPrevVendorAccountId As HiddenField = item.FindControl("hdnPrevVendorAccountId")

        Dim PrevVendorAccountId As Integer = 0

        If hdnPrevVendorAccountId.Value <> "0" Then
            PrevVendorAccountId = CInt(hdnPrevVendorAccountId.Value)
        End If

        If drpUsers.SelectedValue = Nothing Then
            VendorRoleRow.ClearVendorRole(DB, Session("VendorID"), VendorRoleId)
        Else
            If Change_VendorAccountId = Nothing Then
                VendorRoleRow.InsertVendorRole(DB, Session("VendorId"), drpUsers.SelectedValue, VendorRoleId)
                ltlSaved.Text = "Update Saved"
                Core.DataLog("Vendor", PageURL, CurrentUserId, "Edit User Account And Roles", "Insert Isprimary Contact= " & drpUsers.SelectedValue, "", "", "", UserName)
                Dim Sql As String = "UPDATE VendorAccount SET IsPrimary=1 WHERE VendorAccountId=" & drpUsers.SelectedValue
                DB.ExecuteSQL(Sql)
                Core.DataLog("Vendor", PageURL, CurrentUserId, "Edit User Account And Roles", "Update Isprimary Value In admin Section=" & drpUsers.SelectedValue, "", "", "", UserName)
            Else

                Dim dtVendorAccountVendorRole = DB.ExecuteScalar("Select * from VendorAccountVendorRole where VendorAccountId=" & drpUsers.SelectedValue & " and VendorRoleId=1 ")
                If VendorRoleId <> 1 Then
                    VendorRoleRow.UpdateVendorRole(DB, Session("VendorId"), drpUsers.SelectedValue, PrevVendorAccountId, VendorRoleId)
                    ltlSaved.Text = "Update Saved"
                Else
                    If dtVendorAccountVendorRole Is Nothing Then
                        VendorRoleRow.UpdateVendorRole(DB, Session("VendorId"), drpUsers.SelectedValue, PrevVendorAccountId, VendorRoleId)
                        ltlSaved.Text = "Update Saved"
                        Core.DataLog("Vendor", PageURL, CurrentUserId, "Edit User Account And Roles", "Update Isprimary Contact= " & Change_VendorAccountId, "", "", "", UserName)
                        Dim Sql As String = "UPDATE VendorAccount SET IsPrimary=1 WHERE VendorAccountId=" & drpUsers.SelectedValue
                        Dim IsprimarySql As String = "UPDATE VendorAccount SET IsPrimary=0 WHERE VendorAccountId=" & PrevVendorAccountId
                        DB.ExecuteSQL(Sql)
                        DB.ExecuteSQL(IsprimarySql)
                        Core.DataLog("Vendor", PageURL, CurrentUserId, "Edit User Account And Roles", "Update Isprimary Value In admin Section=" & drpUsers.SelectedValue & "," & Change_VendorAccountId, "", "", "", UserName)
                    Else
                        ltlSaved.Text = "Duplicate Primary Contact Is Not Allowed."
                        Exit Sub
                    End If
                End If
            End If
        End If

        'ltlSaved.Text = "Update Saved"
        Core.DataLog("Vendor", PageURL, CurrentUserId, "Manage User Roles", "", "", "", "", UserName)
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "ClearResult", "window.setTimeout('ClearResult(""" & item.FindControl("spanResult").ClientID & """);',3000);", True)
    End Sub

    Private Sub SaveRole(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim list As Controls.SearchList = sender
        Dim item As RepeaterItem = list.NamingContainer
        Dim ltlSaved As Literal = item.FindControl("ltlSaved")
        Dim VendorRoleID As Integer = 0 'Keys(item.ItemIndex)

        'BuilderRoleRow.AssignBuilderRole(DB, Session("BuilderId"), list.Value, BuilderRoleID)
        If list.Value = Nothing Then
            VendorRoleRow.ClearVendorRole(DB, Session("VendorID"), VendorRoleID)
        Else
            VendorRoleRow.AssignVendorRole(DB, Session("VendorId"), list.Value, VendorRoleID)
        End If


        ltlSaved.Text = "Update Saved"
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "ClearResult", "window.setTimeout('ClearResult(""" & item.FindControl("spanResult").ClientID & """);',3000);", True)
    End Sub

    Public Sub Refresh()
        BindData()
    End Sub


End Class
