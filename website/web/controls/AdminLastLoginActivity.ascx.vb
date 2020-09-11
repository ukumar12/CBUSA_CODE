Imports DataLayer
Imports System.Data
Imports Components

Partial Class AdminLastLoginActivity
    Inherits BaseControl

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        BindLastActivityRepeater()
    End Sub

    Private Sub BindLastActivityRepeater()
        Dim LoggedInIsInternal As Boolean = False
        If TypeOf Page Is BasePage Then
            LoggedInIsInternal = CType(Me.Page, AdminPage).LoggedInIsInternal
        End If
        Dim dt As DataTable = AdminLogRow.GetLast10Logins(DB, LoggedInIsInternal)
        LastActivity.DataSource = dt
        LastActivity.DataBind()

        If dt.Rows.Count = 0 Then
            LastActivity.Visible = False
            NoRecords.Visible = True
        Else
            LastActivity.Visible = True
            NoRecords.Visible = False
        End If
    End Sub
End Class
