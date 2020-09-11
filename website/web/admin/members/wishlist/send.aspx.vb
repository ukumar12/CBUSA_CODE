Imports Components
Imports DataLayer
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager

Public Class Member_Wishlist_Send
    Inherits AdminPage

    Protected MemberId As Integer

    Protected WebsiteName As String
    Protected RefererName As String

    Protected dbMember As MemberRow
    Protected dbMemberAddress As MemberAddressRow
    Protected FullName As String

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")

        MemberId = Convert.ToInt32(Request.QueryString("MemberId"))

        WebsiteName = AppSettings("GlobalWebsiteName")
        RefererName = AppSettings("GlobalRefererName")

        dbMember = MemberRow.GetRow(DB, MemberId)
        dbMemberAddress = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)

        FullName = Core.BuildFullName(dbMemberAddress.FirstName, dbMemberAddress.MiddleInitial, dbMemberAddress.LastName)
    End Sub

    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        If Not IsValid Then Exit Sub

        Dim Message As String = String.Empty
        Dim Subject As String = "A " & WebsiteName & " Wish List for you from " & FullName & "!"

        Message = "Greetings from " & WebsiteName & "!" & vbCrLf & vbCrLf
        Message &= FullName & " has sent you a Wish List from " & WebsiteName & " website." & vbCrLf & vbCrLf
        Message &= "Click on the link below to see what items are on the list." & vbCrLf
        Message &= RefererName & "/wishlist.aspx?w=" & dbMember.Guid & vbCrLf & vbCrLf
        Message &= "Now you don't have to read " & dbMemberAddress.FirstName & "'s mind "
        Message &= "-- buy a wished-for item and make " & dbMemberAddress.FirstName & "'s day!" & vbCrLf & vbCrLf

        If Not txtMessage.Text = String.Empty Then
            Message &= "--------------------------------------------------------------" & vbCrLf & vbCrLf
            Message &= dbMemberAddress.FirstName & "'s message to you:" & vbCrLf & vbCrLf
            Message &= txtMessage.Text
        End If
        Core.SendSimpleMail(dbMemberAddress.Email, FullName, txtEmail.Text, txtEmail.Text, Subject, Message)

        Response.Redirect("/admin/members/wishlist/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("/admin/members/wishlist/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class