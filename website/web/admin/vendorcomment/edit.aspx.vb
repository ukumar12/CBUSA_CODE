Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected VendorCommentID As Integer
    Protected VendorID As Integer
    Protected BuilderID As Integer
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("VENDOR_COMMENTS")

        VendorCommentID = Convert.ToInt32(Request("VendorCommentID"))
        VendorID = Convert.ToInt32(Request("VendorID"))
        BuilderID = Convert.ToInt32(Request("BuilderID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If VendorCommentID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbVendorComment As VendorCommentRow = VendorCommentRow.GetRow(DB, VendorCommentID)
        txtComment.Text = dbVendorComment.Comment

        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, dbVendorComment.VendorID)
        ltlVendor.Text = dbVendor.CompanyName

        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbVendorComment.BuilderID)
        ltlBuilder.Text = dbBuilder.CompanyName
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbVendorComment As VendorCommentRow

            If VendorCommentID <> 0 Then
                dbVendorComment = VendorCommentRow.GetRow(DB, VendorCommentID)
            Else
                dbVendorComment = New VendorCommentRow(DB)
            End If
            dbVendorComment.Comment = txtComment.Text

            If VendorCommentID <> 0 Then
                dbVendorComment.Update()
            Else
                VendorCommentID = dbVendorComment.Insert
            End If

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
        Response.Redirect("delete.aspx?VendorCommentID=" & VendorCommentID & "&VendorID=" & VendorID & "&BuilderID=" & BuilderID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

