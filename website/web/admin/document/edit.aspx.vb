Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected AdminDocumentID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("DOCUMENTS")

        AdminDocumentID = Convert.ToInt32(Request("AdminDocumentID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If AdminDocumentID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        cblAudienceType.DataSource = DB.GetDataTable("SELECT * FROM DocumentAudienceType")
        cblAudienceType.DataTextField = "AudienceType"
        cblAudienceType.DataValueField = "DocumentAudienceTypeID"
        cblAudienceType.DataBind()
        F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
        F_LLC.DataValueField = "LLCID"
        F_LLC.DataTextField = "LLC"
        F_LLC.DataBind()
        Dim dbAdminDocument As AdminDocumentRow = AdminDocumentRow.GetRow(DB, AdminDocumentID)
        txtTitle.Text = dbAdminDocument.Title
        ltlUploaded.Text = dbAdminDocument.Uploaded.ToString("MM/dd/yyyy")
        rblIsApproved.SelectedValue = dbAdminDocument.IsApproved
        txtDocumentHistoryNotes.Text = dbAdminDocument.DocumentHistoryNotes
        cblAudienceType.SelectedValues = dbAdminDocument.DocumentAudiences
        F_LLC.SelectedValues = dbAdminDocument.DocumentLLCAudiences
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbAdminDocument As AdminDocumentRow

            If AdminDocumentID <> 0 Then
                dbAdminDocument = AdminDocumentRow.GetRow(DB, AdminDocumentID)
            Else
                dbAdminDocument = New AdminDocumentRow(DB)
            End If
            dbAdminDocument.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
            dbAdminDocument.Title = txtTitle.Text
            dbAdminDocument.IsApproved = rblIsApproved.SelectedValue
            dbAdminDocument.DocumentHistoryNotes = txtDocumentHistoryNotes.Text
            If AdminDocumentID <> 0 Then
                dbAdminDocument.Update()
            Else
                AdminDocumentID = dbAdminDocument.Insert
            End If

            DB.CommitTransaction()

            Dim SQL As String
            Dim DeleteSQL As String
            dbAdminDocument.DeleteFromAllLists()
            dbAdminDocument.InsertToLists(cblAudienceType.SelectedValues)
            dbAdminDocument.DeleteFromAdminDocumentLLCLists()
            dbAdminDocument.InsertToAdminDocumentLLCLists(F_LLC.SelectedValues)

            AdminDocumentRow.ClearAllRecipients(DB, AdminDocumentID)

            Dim Audience As String() = cblAudienceType.SelectedTexts.Split(",")
            For Each s As String In Audience

                Select Case s.Trim.ToLower
                    Case "builder"
                        SQL = "INSERT INTO AdminDocument" & s & "Recipient(AdminDocumentID, " & s & "ID) SELECT   " & AdminDocumentID & " , BuilderID From Builder Where HasDocumentsAccess =1 AND BuilderID IN ( Select BuilderID From Builder WHERE LLCID IN (" & F_LLC.SelectedValues & " ))"
                        Try
                            Me.DB.ExecuteSQL(SQL)
                        Catch ex As SqlClient.SqlException
                        End Try
                    Case "vendor"
                        SQL = "INSERT INTO AdminDocument" & s & "Recipient(AdminDocumentID, " & s & "ID) SELECT   " & AdminDocumentID & " , VendorID From Vendor Where HasDocumentsAccess =1 AND   VendorID IN ( Select  Distinct VendorID  from LLCVendor WHERE LLCID IN (" & F_LLC.SelectedValues & " ))"
                        Try
                            Me.DB.ExecuteSQL(SQL)
                        Catch ex As SqlClient.SqlException
                        End Try
                    Case "piq"
                        SQL = "INSERT INTO AdminDocument" & s & "Recipient(AdminDocumentID, " & s & "ID) SELECT    " & AdminDocumentID & " , PIQID From PIQ Where HasDocumentsAccess =1 "
                        Try
                            Me.DB.ExecuteSQL(SQL)
                        Catch ex As SqlClient.SqlException
                        End Try
                End Select


            Next




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
        Response.Redirect("delete.aspx?AdminDocumentID=" & AdminDocumentID & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnRecipients_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRecipients.Click
        Response.Redirect("recipient.aspx?AdminDocumentID=" & AdminDocumentID)
    End Sub
End Class
