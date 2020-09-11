Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class View
    Inherits AdminPage

    Protected MessageId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        MessageId = Request("MessageId")
        MailingSummaryCtrl.MessageId = MessageId

        If Not IsPostBack Then
            LoadFromDb()
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
        ltlName.Text = dbMailingMessage.Name
    End Sub

End Class