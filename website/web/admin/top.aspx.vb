Imports Components

Partial Class TopPage
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If "admin" <> LoggedInUsername Then
            ltlChangePassword.Text = AdminMenu.MenuTopEmptyRoot("/admin/password/", "Change Password")
        End If
        If HasRights("USERS") Then
            ltlSystemParameters.Text = AdminMenu.MenuTopEmptyRoot("/admin/settings/", "System Parameters")
        End If
        ltlLogout.Text = AdminMenu.MenuTopEmptyRoot("/admin/logout.aspx", "Logout")
    End Sub

End Class
