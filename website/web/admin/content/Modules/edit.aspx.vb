Imports Components
Imports DataLayer
Imports System.Data
Imports System.Data.SqlClient

Partial Class edit
    Inherits AdminPage

    Private ModuleId As Integer
    Private dbModule As ContentToolModuleRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
		CheckInternalAccess("CONTENT_TOOL")

        ModuleId = Convert.ToInt32(Request("ModuleId"))
        trSkipIndexing.Visible = SysParam.GetValue(DB, "IdevSearchEnabled")
        If Not IsPostBack Then
            If ModuleId = 0 Then
                Delete.Visible = False
            Else
                LoadFromDB()
            End If
        End If
    End Sub

    Private Sub LoadFromDB()
        dbModule = ContentToolModuleRow.GetRow(DB, ModuleId)

        Name.text = dbModule.Name
        Args.text = dbModule.Args
        MinWidth.text = dbModule.MinWidth
        MaxWidth.Text = dbModule.MaxWidth
        HTML.text = dbModule.HTML
        ControlURL.text = dbModule.ControlURL
        chkSkipIndexing.Checked = dbModule.SkipIndexing
    End Sub

    Private Sub Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Save.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbModule As ContentToolModuleRow
            If ModuleId <> 0 Then
                dbModule = ContentToolModuleRow.GetRow(DB, ModuleId)
            Else
                dbModule = New ContentToolModuleRow(DB)
            End If
            dbModule.Name = Name.Text
            dbModule.Args = Args.text
            dbModule.MinWidth = MinWidth.text
            dbModule.MaxWidth = MaxWidth.Text
            dbModule.SkipIndexing = chkSkipIndexing.Checked
            dbModule.HTML = HTML.text
            dbModule.ControlURL = ControlURL.text
            If ModuleId <> 0 Then
                dbModule.Update()
            Else
                ModuleId = dbModule.AutoInsert()
            End If
            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        Finally
            If Not DB Is Nothing Then DB.Close()
        End Try
    End Sub

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub Delete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Delete.Click
        If ModuleId <> 0 Then
            Response.Redirect("delete.aspx?ModuleId=" & ModuleId & "&" & GetPageParams(FilterFieldType.All))
        End If
    End Sub

End Class
