Imports Components
Imports DataLayer
Imports System.Net.Mail

Partial Class _default
    Inherits SitePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        If Not IsPostBack Then
            '' commented to hide the monthly data reminder pop-up

            ' CheckMonthlyData()

        End If
    End Sub

    Private Sub CheckMonthlyData()
        If Session("MonthlyDataNagScreen") Is Nothing AndAlso Not BuilderMonthlyStatsRow.MonthlyDataIsCurrent(DB, Session("BuilderId")) Then
            'If Not BuilderMonthlyStatsRow.MonthlyDataIsCurrent(DB, Session("BuilderId")) Then
            ScriptManager.RegisterStartupScript(Page, Me.GetType, "MonthlyDataPopup", "Sys.Application.add_load(OpenDataReminder);", True)
            Session("MonthlyDataNagScreen") = True
        End If
    End Sub

End Class
