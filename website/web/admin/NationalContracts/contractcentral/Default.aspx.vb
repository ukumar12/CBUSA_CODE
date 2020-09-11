Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            PopulateLLCList()
        End If

    End Sub

    Private Sub PopulateLLCList()
        ddlLLC.DataSource = LLCRow.GetList(DB, "LLC")
        ddlLLC.DataValueField = "LLCId"
        ddlLLC.DataTextField = "LLC"
        ddlLLC.DataBind()
        ddlLLC.Items.Insert(0, New ListItem("-- SELECT --", ""))
    End Sub

End Class
