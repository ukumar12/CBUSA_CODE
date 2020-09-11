Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected SlotId As Integer
    Protected dbTemplate As MailingTemplateRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BROADCAST")

        SlotId = Convert.ToInt32(Request("SlotId"))
        dbTemplate = MailingTemplateRow.GetRow(DB, Convert.ToInt32(Request("F_TemplateId")))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        If SlotId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbMailingTemplateSlot As MailingTemplateSlotRow = MailingTemplateSlotRow.GetRow(DB, SlotId)
        txtSlotName.Text = dbMailingTemplateSlot.SlotName
        fuImageName.CurrentFileName = dbMailingTemplateSlot.ImageName
        fuImageName.DisplayImage = True
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            Dim dbMailingTemplateSlot As MailingTemplateSlotRow
            If SlotId <> 0 Then
                dbMailingTemplateSlot = MailingTemplateSlotRow.GetRow(DB, SlotId)
            Else
                dbMailingTemplateSlot = New MailingTemplateSlotRow(DB)
            End If
            dbMailingTemplateSlot.SlotName = txtSlotName.Text
            dbMailingTemplateSlot.TemplateId = dbTemplate.TemplateId
            If fuImageName.NewFileName <> String.Empty Then
                fuImageName.SaveNewFile()
                dbMailingTemplateSlot.ImageName = fuImageName.NewFileName
            ElseIf fuImageName.MarkedToDelete Then
                dbMailingTemplateSlot.ImageName = Nothing
            End If

            If SlotId <> 0 Then
                dbMailingTemplateSlot.Update()
            Else
                SlotId = dbMailingTemplateSlot.Insert
            End If

            DB.CommitTransaction()

            If fuImageName.NewFileName <> String.Empty Or fuImageName.MarkedToDelete Then fuImageName.RemoveOldFile()

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
        Response.Redirect("delete.aspx?SlotId=" & SlotId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
