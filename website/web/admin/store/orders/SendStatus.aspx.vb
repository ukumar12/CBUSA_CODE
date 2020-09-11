Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.Net
Imports Utility

Partial Class SendStatus
    Inherits AdminPage

    Private OrderId As Integer

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("ORDERS")

        Try
            OrderId = Convert.ToInt32(Request("OrderId"))

        Catch ex As Exception

        End Try

        If Not IsPostBack Then
            Dim dbAdmin As AdminRow = AdminRow.GetRow(DB, LoggedInAdminId)
            Me.txtBCC.Text = dbAdmin.Email
            Me.txtFromEmail.Text = dbAdmin.Email
            Me.txtFromName.Text = dbAdmin.FirstName & " " & dbAdmin.LastName

            Dim dbORder As StoreOrderRow = StoreOrderRow.GetRow(DB, OrderId)
            Me.txtRecipientEmail.Text = dbORder.Email
            Me.txtRecipientName.Text = Trim(dbORder.BillingFirstName & " " & dbORder.BillingLastName)


            Me.txtSubject.Text = SysParam.GetValue(DB, "StatusUpdateSubject")
            'Me.txtMessage.Value = SysParam.GetValue(DB, "StatusUpdateBody")

            LoadStatus()

        End If


    End Sub

    Private Sub LoadStatus()
        Dim qs As New URLParameters(Request.QueryString, "OrderId")
        Dim URL As String = AppSettings("GlobalRefererName") & "/admin/store/orders/view.aspx?OrderItemId=" & Request("OrderItemId") & "&RecipientId=" & Request("RecipientId") & "&OrderId=" & HttpUtility.UrlEncode(Crypt.EncryptTripleDes(OrderId)) & "&" & qs.ToString
        Dim r As HttpWebRequest = WebRequest.Create(URL)
        Dim myCache As New System.Net.CredentialCache()
        myCache.Add(New Uri(URL), "Basic", New System.Net.NetworkCredential("ameagle", "design"))
        r.Credentials = myCache

        'Get the data as an HttpWebResponse object
        Dim resp As System.Net.HttpWebResponse = r.GetResponse()
        Dim sr As New System.IO.StreamReader(resp.GetResponseStream())
        Dim HTML As String = sr.ReadToEnd()
        sr.Close()

        HTML = Replace(HTML, "href=""/", "href=""" & AppSettings("GlobalRefererName") & "/")
        HTML = Replace(HTML, "src=""/", "src=""" & AppSettings("GlobalRefererName") & "/")
        HTML = Replace(HTML, "background:url(/assets", "background:url(" & AppSettings("GlobalRefererName") & "/assets")

        HTML = Replace(HTML, "<!-- MESSAGE -->", SysParam.GetValue(DB, "StatusUpdateBody"))

        Me.txtMessage.Value = HTML

    End Sub


    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        If Not Page.IsValid Then Exit Sub
        If SendIt() Then
            Page.ClientScript.RegisterStartupScript(Me.GetType, "CloseWindow", "parent.closeSend();", True)
        End If
    End Sub

    Public Function SendIt() As Boolean

        Try
            Dim FromEmail As String = Me.txtFromEmail.Text
            Dim FromName As String = Me.txtFromName.Text

            Core.SendHTMLMail(FromEmail, FromName, txtRecipientEmail.Text, txtRecipientName.Text, txtSubject.Text, txtMessage.Value)
            Dim dbResponseCapture As New DataLayer.ResponseCaptureRow(DB)
            dbResponseCapture.OrderId = OrderId
            dbResponseCapture.ResponseCapture = Me.txtMessage.Value
            Dim ResponseCaptureId As Integer = dbResponseCapture.Insert()

            Dim dbNotes As New StoreOrderNoteRow(DB)
            dbNotes.AdminId = LoggedInAdminId
            dbNotes.Note = "Sent status email: <a href=""/admin/responsecapture/view.aspx?ResponseCaptureId=" & ResponseCaptureId & """ target=""_blank"">View Email</a>"
            dbNotes.OrderId = OrderId
            dbNotes.Insert()


            Return True
        Catch wex As System.Net.WebException
            AddError("Error sending email.")
            Return False
        Catch sex As System.Data.SqlClient.SqlException
            AddError("Email Sent. Error Capturing HTML.")
            Return False
        End Try
    End Function
End Class

