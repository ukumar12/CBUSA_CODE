Imports Components
Imports Utility
Imports System.Data

Partial Class MailingLists
    Inherits BaseControl

    Private Total As Integer = 0

    Public Property IsPermanent() As Boolean
        Get
            Return IIf(ViewState("IsPermanent") Is Nothing, False, ViewState("IsPermanent"))
        End Get
        Set(ByVal value As Boolean)
            ViewState("IsPermanent") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindControls()
        End If
    End Sub

    Private Sub BindControls()
        Dim Conn As String = " WHERE "
        Dim SQL As String = ""

        SQL = "SELECT ml.Name, (SELECT COUNT(MemberId) AS Total FROM MailingListMember WHERE ListId = ml.ListId) AS Total FROM MailingList ml "
        If IsPermanent Then
            SQL &= Conn & " ml.IsPermanent = 1"
        Else
            SQL &= Conn & " ml.IsPermanent = 0"
        End If
        SQL &= " ORDER BY ml.Name ASC"

        Dim dt As DataTable = DB.GetDataTable(SQL)
        rptLists.DataSource = dt
        rptLists.DataBind()

        If Total > 0 Then
            ltlHeader.Text = "There are " & Total & " members subscribed to the following lists: "
        Else
            ltlHeader.Text = "There are no lists available for this recipient type"
        End If
    End Sub

    Protected Sub rptLists_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLists.ItemDataBound
        If Not e.Item.ItemType = ListItemType.Item And Not e.Item.ItemType = ListItemType.AlternatingItem Then
            Exit Sub
        End If

        Total += e.Item.DataItem("Total")
    End Sub
End Class
