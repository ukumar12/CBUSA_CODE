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

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub
    Private Sub LoadFromDB()
        cblAudienceType.DataSource = DB.GetDataTable("SELECT * FROM DocumentAudienceType")
        cblAudienceType.DataTextField = "AudienceType"
        cblAudienceType.DataValueField = "DocumentAudienceTypeID"
        cblAudienceType.DataBind()

        F_LLC.DataSource = LLCRow.GetList(DB, "LLC")
        F_LLC.DataValueField = "LLCID"
        F_LLC.DataTextField = "LLC"
        F_LLC.DataBind()

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        Dim AdminDocument As New DataLayer.AdminDocumentRow

        Dim FileInfo As System.IO.FileInfo
        Dim OriginalExtension As String
        Dim NewFileName As String

        Try
            DB.BeginTransaction()

            If Me.fulDocument.NewFileName <> String.Empty Then

                AdminDocument = New DataLayer.AdminDocumentRow(Me.DB)
                AdminDocument.AdminID = IIf(AdminRow.GetRowByUsername(DB, Session("Username")).AdminId = 0, 1, AdminRow.GetRowByUsername(DB, Session("Username")).AdminId)
                AdminDocument.FileGUID = Guid.NewGuid.ToString
                AdminDocument.IsApproved = CType(Me.rblIsApproved.Text, Boolean)
                AdminDocument.Title = Me.txtTitle.Text
                AdminDocument.Uploaded = Now
                AdminDocument.DocumentHistoryNotes = txtDocumentHistoryNotes.Text
                Me.fulDocument.Folder = "/assets/document/"
                Me.fulDocument.SaveNewFile()

                FileInfo = New System.IO.FileInfo(Server.MapPath(Me.fulDocument.Folder & Me.fulDocument.NewFileName))
                OriginalExtension = System.IO.Path.GetExtension(Me.fulDocument.MyFile.FileName)
                NewFileName = Replace(AdminDocument.FileGUID, "-", "")
                FileInfo.CopyTo(Server.MapPath(Me.fulDocument.Folder & NewFileName & OriginalExtension))

                AdminDocument.FileName = NewFileName & OriginalExtension
                AdminDocumentID = AdminDocument.Insert()
                AdminDocument.DeleteFromAllLists()
                AdminDocument.InsertToLists(cblAudienceType.SelectedValues)
                AdminDocument.DeleteFromAdminDocumentLLCLists()
                AdminDocument.InsertToAdminDocumentLLCLists(F_LLC.SelectedValues)
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

                FileInfo.Delete()

            End If

            DB.CommitTransaction()

            Response.Redirect("default.aspx")

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx")
    End Sub

End Class
