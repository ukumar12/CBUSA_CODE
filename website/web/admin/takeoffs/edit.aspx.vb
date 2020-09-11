Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected TakeOffID As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("TAKE_OFFS")

        TakeOffID = Convert.ToInt32(Request("TakeOffID"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        drpBuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
        drpBuilderID.DataValueField = "BuilderID"
        drpBuilderID.DataTextField = "CompanyName"
        drpBuilderID.Databind()
        drpBuilderID.Items.Insert(0, New ListItem("", ""))

        If TakeOffID = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbTakeOff As TakeOffRow = TakeOffRow.GetRow(DB, TakeOffID)
        txtTitle.Text = dbTakeOff.Title
        dtArchived.Value = dbTakeOff.Archived
        drpBuilderID.SelectedValue = dbTakeOff.BuilderID
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbTakeOff As TakeOffRow

            If TakeOffID <> 0 Then
                dbTakeOff = TakeOffRow.GetRow(DB, TakeOffID)
            Else
                dbTakeOff = New TakeOffRow(DB)
            End If
            dbTakeOff.Title = txtTitle.Text
            dbTakeOff.Archived = dtArchived.Value
            dbTakeOff.BuilderID = drpBuilderID.SelectedValue

            If TakeOffID <> 0 Then
                dbTakeOff.Update()
            Else
                TakeOffID = dbTakeOff.Insert
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
        Response.Redirect("delete.aspx?TakeOffID=" & TakeOffID & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class

