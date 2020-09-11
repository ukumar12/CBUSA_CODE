Imports System.Web
Imports System.Data.SqlClient
Imports System.Text
Imports System.Data
Imports DataLayer
Imports Components

Public Class Edit
    Inherits AdminPage

    Protected MemberId As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("BROADCAST")

        MemberId = Convert.ToInt32(Request("MemberId"))
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()
        cblLists.DataSource = MailingListRow.GetLists(DB)
        cblLists.DataTextField = "name"
        cblLists.DataValueField = "listid"
        cblLists.DataBind()

        If MemberId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        Dim dbMailingMember As MailingMemberRow = MailingMemberRow.GetRow(DB, MemberId)
        txtEmail.Text = dbMailingMember.Email
        txtName.Text = dbMailingMember.Name
        rblMimeType.SelectedValue = dbMailingMember.MimeType
        cblLists.SelectedValues = dbMailingMember.SubscribedLists

        Dim SQL As String = " select NumBounces, mm.SentDate, mm.Name from (" _
          & " select List_Id, sum(Num_Bounces) as NumBounces from Mailing_Lyris_Member where email = " & DB.Quote(dbMailingMember.Email) & " group by List_Id" _
          & " ) as tmp, MailingMessage mm where tmp.List_Id IN (ListHTMLId, ListTextId)" _
          & " order by SentDate"

        gvList.DataSource = DB.GetDataTable(SQL)
        gvList.DataBind()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbMailingMember As MailingMemberRow

            If MemberId <> 0 Then
                dbMailingMember = MailingMemberRow.GetRow(DB, MemberId)
            Else
                dbMailingMember = MailingMemberRow.GetRowByEmail(DB, txtEmail.Text)
                MemberId = dbMailingMember.MemberId
            End If
            dbMailingMember.Email = txtEmail.Text
            dbMailingMember.Name = txtName.Text
            dbMailingMember.MimeType = rblMimeType.SelectedValue
            dbMailingMember.Status = "ACTIVE"
            If MemberId <> 0 Then
                dbMailingMember.Update()
            Else
                MemberId = dbMailingMember.Insert
            End If
            dbMailingMember.DeleteFromAllLists()
            dbMailingMember.InsertToLists(cblLists.SelectedValues)

            DB.CommitTransaction()

            Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))

        Catch ex As SqlException
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("default.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("delete.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class
