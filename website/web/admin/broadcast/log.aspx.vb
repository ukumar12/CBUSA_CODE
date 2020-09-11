Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Partial Class Log
    Inherits AdminPage

    Protected MessageId As Integer
    Protected ListId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("BROADCAST")

        MessageId = Request("MessageId")
        ListId = Request("ListId")
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Function StrUnix2HTML(ByVal Text As String) As String
        If Text = String.Empty Then
            Return String.Empty
        End If
        Return Replace(Text, vbLf, "<br />")
    End Function

    Private Sub LoadFromDB()
        Dim dbMailingMessage As MailingMessageRow = MailingMessageRow.GetRow(DB, MessageId)
        ltlName.Text = dbMailingMessage.Name

        Dim ListName As String = String.Empty
        If dbMailingMessage.ListHTMLId = ListId Then
            ListName = dbMailingMessage.ListPrefix & "_" & dbMailingMessage.MessageId & "_HTML"
        End If
        If dbMailingMessage.ListTextId = ListId Then
            ListName = dbMailingMessage.ListPrefix & "_" & dbMailingMessage.MessageId & "_TEXT"
        End If

        Dim Copied As Boolean = False
        Dim Total As Integer = DB.ExecuteScalar("SELECT count(*) FROM Mailing_Lyris_Outmail o WHERE o.listId_ = " & DB.Number(ListId))
        If Total = 0 Then Copied = False Else Copied = True

        Dim Log As String = String.Empty
        If Copied Then
            SQL = "SELECT Transact_ FROM Mailing_Lyris_Outmail WHERE ListId_ = " & DB.Number(ListId)
            Log = DB.ExecuteScalar(SQL)
        Else
            SQL = "SELECT Transact_ FROM outmail_ WHERE type_ = 'list' AND List_ = " & DB.Quote(ListName)
            Dim DBLyris As New Database
            DBLyris.Open(AppSettings("LyrisConnectionString"))
            Log = DBLyris.ExecuteScalar(SQL)
            DBLyris.Close()
        End If
        If Log = String.Empty Then Log = "There is no log information yet. Please check again later."
        ltlLog.Text = StrUnix2HTML(Log)
    End Sub

End Class