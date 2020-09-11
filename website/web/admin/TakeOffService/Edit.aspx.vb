Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Controls

Public Class Edit
    Inherits AdminPage

    Protected TakeOffServiceID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BUILDERS")

        TakeOffServiceID = Convert.ToInt32(Request("TakeOffServiceID"))
        mudUpload.TakeOffServiceId = TakeOffServiceID

        If TakeOffServiceID > 0 Then
            trDocUpload.Visible = True
            btnSaveAndUpload.Visible = False
        Else
            trDocUpload.Visible = False
            btnSaveAndUpload.Visible = True
        End If

        If Not IsPostBack Then
            LoadFromDB()
            BindDocuments()

        End If
    End Sub

 

    Private Sub LoadFromDB()

        cblLLC.DataSource = LLCRow.GetList(DB, "LLC")
        cblLLC.DataTextField = "LLC"
        cblLLC.DataValueField = "LLCID"
        cblLLC.DataBind()
        If TakeOffServiceID = 0 Then
            rblIsActive.SelectedValue = True
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbTakeOffService As TakeOffServiceRow = TakeOffServiceRow.GetRow(DB, TakeOffServiceID)
        txtTitle.Text = dbTakeOffService.Title
        txtDescription.Value = dbTakeOffService.Description
        cblLLC.SelectedValues = dbTakeOffService.GetSelectedLLCs
        rblIsActive.SelectedValue = dbTakeOffService.IsActive
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbTakeOffService As TakeOffServiceRow

            If TakeOffServiceID <> 0 Then
                dbTakeOffService = TakeOffServiceRow.GetRow(DB, TakeOffServiceID)
            Else
                dbTakeOffService = New TakeOffServiceRow(DB)
            End If
            dbTakeOffService.Title = txtTitle.Text
            dbTakeOffService.Description = txtDescription.Value

            dbTakeOffService.IsActive = rblIsActive.SelectedValue

            If TakeOffServiceID <> 0 Then
                dbTakeOffService.Update()
            Else
                TakeOffServiceID = dbTakeOffService.Insert
            End If

            dbTakeOffService.DeleteFromAllLLCs()
            dbTakeOffService.InsertToLLCs(cblLLC.SelectedValues)

            DB.CommitTransaction()


            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?TakeOffServiceID=" & TakeOffServiceID & "&" & GetPageParams(FilterFieldType.All))
    End Sub


    Protected Sub rptDocuments_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptDocuments.ItemDataBound

        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If


        Dim lnkMessageTitle As HtmlAnchor = e.Item.FindControl("lnkMessageTitle")

        Dim lnkDelete As ConfirmImageButton = e.Item.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Item.DataItem("DocumentId")

        lnkMessageTitle.HRef = "/assets/takeoffservice/" & e.Item.DataItem("FileName").ToString
    End Sub

    Protected Sub rptDocuments_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptDocuments.ItemCommand
        Select Case e.CommandName
            Case Is = "Remove"
                Try
                    DB.BeginTransaction()
                    Dim FileInfo As System.IO.FileInfo
                    Try
                        FileInfo = New System.IO.FileInfo(Server.MapPath("/assets/takeoffservice/" & TakeOffServiceDocumentRow.GetRow(DB, e.CommandArgument).FileName))
                        FileInfo.Delete()
                    Catch ex As Exception

                    End Try
                    TakeOffServiceDocumentRow.RemoveRow(DB, e.CommandArgument)
                    DB.CommitTransaction()
                    BindDocuments()
                Catch ex As SqlException
                    If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
                    AddError(ErrHandler.ErrorText(ex))
                End Try
        End Select
    End Sub
    Protected Sub BindDocuments()

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From  TakeoffserviceDocument  where TakeOffServiceID = " & DB.Number(TakeOffServiceID)

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)

            Me.rptDocuments.DataSource = dt
            Me.rptDocuments.DataBind()

            If dt.Rows.Count > 0 Then
                divNoCurrentDocuments.Visible = False
            End If
        End If
    End Sub
    Protected Sub btnTransferDocs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTransferDocs.Click
        BindDocuments()
    End Sub
End Class
