Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components
Imports Controls

Public Class Edit
    Inherits AdminPage

    Protected NCPContentID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        NCPContentID = Convert.ToInt32(Request("NCPContentID"))

        mudUpload.ncpContentID = NCPContentID


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

        cblContracts.DataSource = NationalContractRow.GetList(DB, "Title")
        cblContracts.DataTextField = "Title"
        cblContracts.DataValueField = "ContractID"
        cblContracts.DataBind()

        If NCPContentID = 0 Then
            rblIsActive.SelectedValue = True
            btnDelete.Visible = False
            trDocUpload.Visible = False
            Exit Sub
        End If


        Dim dbNCPContent As NCPContentRow = NCPContentRow.GetRow(DB, NCPContentID)
        txtName.Text = dbNCPContent.Name
        txtDescription.Value = dbNCPContent.Description
        rblIsActive.SelectedValue = dbNCPContent.IsActive
        cblLLC.SelectedValues = dbNCPContent.GetSelectedLLCs
        cblContracts.SelectedValues = dbNCPContent.GetSelectedContracts

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbNCPContent As NCPContentRow

            If NCPContentID <> 0 Then
                dbNCPContent = NCPContentRow.GetRow(DB, NCPContentID)
            Else
                dbNCPContent = New NCPContentRow(DB)
            End If
            dbNCPContent.Name = txtName.Text
            dbNCPContent.Description = txtDescription.Value
            dbNCPContent.IsActive = rblIsActive.SelectedValue

            If NCPContentID <> 0 Then
                dbNCPContent.Update()
            Else
                NCPContentID = dbNCPContent.Insert
            End If

            dbNCPContent.DeleteFromAllLLCs()
            dbNCPContent.InsertToLLCs(cblLLC.SelectedValues)

            dbNCPContent.DeleteFromAllContracts()
            dbNCPContent.InsertToContentContracts(cblContracts.SelectedValues)
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
        Response.Redirect("delete.aspx?NCPContentID=" & NCPContentID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

   
    Protected Sub BindDocuments()

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        SQL = "Select * From  NCPDocument  where NCPContentID = " & DB.Number(NCPContentID)

        If SQL <> String.Empty Then
            dt = DB.GetDataTable(SQL)
            gvDocuments.Pager.NofRecords = dt.Rows.Count
            gvDocuments.DataSource = dt
            gvDocuments.DataBind()
            If dt.Rows.Count > 0 Then
                divNoCurrentDocuments.Visible = False
            End If
        End If
    End Sub
    Protected Sub btnTransferDocs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTransferDocs.Click
        BindDocuments()
    End Sub
    Protected Sub gvDocuments_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvDocuments.RowCreated
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkDelete As ConfirmImageButton = e.Row.FindControl("lnkDelete")
        '  CType(Me.Page.Master.FindControl("AjaxManager"), ScriptManager).RegisterAsyncPostBackControl(lnkDelete)
    End Sub

    Protected Sub gvDocuments_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvDocuments.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim lnkDelete As ConfirmImageButton = e.Row.FindControl("lnkDelete")
        lnkDelete.CommandArgument = e.Row.DataItem("DocumentID")
        Dim LnkDocument As HyperLink = e.Row.FindControl("LnkDocument")
        LnkDocument.NavigateUrl = "/assets/NCPDocuments/" & e.Row.DataItem("FileName").ToString
    End Sub

    Protected Sub gvDocuments_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvDocuments.RowCommand
        Select Case e.CommandName
            Case Is = "Remove"
                DB.BeginTransaction()
                Dim dbNCPDocument As NCPDocumentRow = NCPDocumentRow.GetRow(DB, e.CommandArgument)
                NCPDocumentRow.RemoveRow(DB, e.CommandArgument)
                DB.CommitTransaction()
                BindDocuments()
        End Select
    End Sub
    'Protected Sub gvDocuments_RowDeleting(sender As Object, e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gvDocuments.RowDeleting
    '    Dim row As GridViewRow = CType(gvDocuments.Rows(e.RowIndex), GridViewRow)


    '    Dim DocumentID As String = gvDocuments.DataKeys(gvDocuments.EditIndex).Item("DocumentID").ToString()
    '    Dim DocumentTitle As String = CType(row.Cells(2).Controls(0), TextBox).Text

    '    Dim dbNCPDocument As NCPDocumentRow = NCPDocumentRow.GetRow(DB, DocumentID)


    '    NCPDocumentRow.RemoveRow(DB, DocumentID)


    '    gvDocuments.EditIndex = -1
    '    BindDocuments()
    'End Sub
    Protected Sub gvDocuments_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gvDocuments.RowEditing
        gvDocuments.EditIndex = e.NewEditIndex
        BindDocuments()
    End Sub
    Protected Sub gvDocuments_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gvDocuments.RowUpdating
        Dim row As GridViewRow = CType(gvDocuments.Rows(e.RowIndex), GridViewRow)

        If CType(row.Cells(2).Controls(0), TextBox).Text <> "" Then
            Dim DocumentID As String = gvDocuments.DataKeys(gvDocuments.EditIndex).Item("DocumentID").ToString()
            Dim DocumentTitle As String = CType(row.Cells(2).Controls(0), TextBox).Text

            Dim dbNCPDocument As NCPDocumentRow = NCPDocumentRow.GetRow(DB, DocumentID)
            dbNCPDocument.Title = DocumentTitle

            dbNCPDocument.Update()
        End If

        gvDocuments.EditIndex = -1
        BindDocuments()
    End Sub



    Protected Sub gvDocuments_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gvDocuments.RowCancelingEdit
        gvDocuments.EditIndex = -1
        BindDocuments()
    End Sub
End Class
