Imports Components
Imports DataLayer
Imports System.Configuration.ConfigurationManager

Partial Class SendToFriend
    Inherits SitePage

    Protected SiteName As String = String.Empty
    Protected Display2 As String
    Protected Display3 As String
    Protected DisplayAddMore As String

    Private ReadOnly Property Url() As String
        Get
            Return Request("url")
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SiteName = AppSettings("GlobalWebsiteName")

        If FriendName2.Text = String.Empty And FriendEmail2.Text = String.Empty Then
            Display2 = "none"
        Else
            Display2 = "block"
        End If
        If FriendName3.Text = String.Empty And FriendEmail3.Text = String.Empty Then
            Display3 = "none"
        Else
            Display3 = "block"
        End If
        If Display2 = "block" And Display3 = "block" Then
            DisplayAddMore = "none"
        Else
            DisplayAddMore = "block"
        End If

        If Not Request("c") = String.Empty Then
            pnlResult.Visible = True
            pnlFields.Visible = False
        Else
            pnlResult.Visible = False
            pnlFields.Visible = True
        End If
    End Sub

    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        If Not IsValid Then Exit Sub

        Dim Msg As String = String.Empty
        Dim Recipients As String = FriendEmail1.Text

        Msg &= "Your Friend, " & YourName.Text & ", has recommended the following page on " & AppSettings("GlobalWebsiteName") & vbCrLf & vbCrLf
        Msg &= "Title: " & Subject.Text & vbCrLf
        Msg &= "URL: " & AppSettings("GlobalRefererName") & Url & vbCrLf & vbCrLf

        If Not Message.Text = String.Empty Then
            Msg &= "Your friend also included this personal message:" & vbCrLf & vbCrLf & Message.Text & vbCrLf & vbCrLf
        End If
        Msg &= "----------" & vbCrLf & "NOTE: If your e-mail account doesn't automatically turn the URL above into a link, you can copy and paste it into your browser."

        Core.SendSimpleMail(YourEmail.Text, YourName.Text, FriendEmail1.Text, FriendName1.Text, Subject.Text, Msg)

        If Not FriendEmail2.Text = String.Empty Then
            Core.SendSimpleMail(YourEmail.Text, YourName.Text, FriendEmail2.Text, FriendName2.Text, Subject.Text, Msg)
            Recipients &= "," & FriendEmail2.Text
        End If

        If Not FriendEmail3.Text = String.Empty Then
            Core.SendSimpleMail(YourEmail.Text, YourName.Text, FriendEmail3.Text, FriendName3.Text, Subject.Text, Msg)
            Recipients &= "," & FriendEmail3.Text
        End If

        Session("Result") = "An e-mail has been sent to the following recipient(s):  " & Recipients & "."

        Response.Redirect("friend.aspx?c=y")
    End Sub

    Private Function EmailNameXor(ByVal Email As Boolean, ByVal Name As Boolean) As Boolean
        Return Not (Email Or Name And Not (Email And Name))
    End Function

    Protected Sub valCustom2_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valCustom2.ServerValidate
        args.IsValid = EmailNameXor(String.Empty = FriendEmail2.Text, String.Empty = FriendName2.Text)
    End Sub

    Protected Sub valCustom3_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valCustom3.ServerValidate
        args.IsValid = EmailNameXor(String.Empty = FriendEmail3.Text, String.Empty = FriendName3.Text)
    End Sub

End Class
