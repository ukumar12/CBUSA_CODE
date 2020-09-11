Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_Addressbook_Default
    Inherits SitePage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureSSL()
        EnsureMemberAccess()

        Dim dbMember As MemberRow = MemberRow.GetRow(DB, Session("MemberId"))
        Dim ds As DataTable = dbMember.GetAddressBook()
        rptAddressBook.DataSource = ds
        rptAddressBook.DataBind()
        If ds.DefaultView.Count = 0 Then
            divItems.Visible = False
        Else
            divItems.Visible = True
        End If
        divNoItems.Visible = Not divItems.Visible
    End Sub

    Protected Sub rptAddressBook_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptAddressBook.ItemCommand

        If e.CommandName = "Remove" Then
            If MemberAddressRow.IsMemberAddressValid(DB, Session("MemberId"), e.CommandArgument) Then
                MemberAddressRow.RemoveRow(DB, e.CommandArgument)
            End If
            Response.Redirect("/members/addressbook/")

        ElseIf e.CommandName = "Edit" Then
            If MemberAddressRow.IsMemberAddressValid(DB, Session("MemberId"), e.CommandArgument) Then
                Response.Redirect("/members/addressbook/edit.aspx?AddressId=" & e.CommandArgument)
            End If

        End If
    End Sub

    Protected Sub rptAddressBook_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAddressBook.ItemDataBound
        If Not e.Item.ItemType = ListItemType.AlternatingItem And Not e.Item.ItemType = ListItemType.Item Then
            Exit Sub
        End If

        Dim btnEdit As Button = CType(e.Item.FindControl("btnEdit"), Button)
        Dim btnDelete As Button = CType(e.Item.FindControl("btnDelete"), Button)
        Dim ltlLabel As Literal = CType(e.Item.FindControl("ltlLabel"), Literal)
        Dim ltlName As Literal = CType(e.Item.FindControl("ltlName"), Literal)
        Dim ltlAddress As Literal = CType(e.Item.FindControl("ltlAddress"), Literal)

        btnDelete.CommandArgument = e.Item.DataItem("AddressId")
        btnEdit.CommandArgument = e.Item.DataItem("AddressId")

        Dim FirstName As String = IIf(IsDBNull(e.Item.DataItem("FirstName")), String.Empty, e.Item.DataItem("FirstName"))
        Dim LastName As String = IIf(IsDBNull(e.Item.DataItem("LastName")), String.Empty, e.Item.DataItem("LastName"))
        Dim LabelName As String = IIf(IsDBNull(e.Item.DataItem("Label")), String.Empty, e.Item.DataItem("Label"))
        Dim FullAddress As String = String.Empty

        Dim Address As String = String.Empty
        If Not IsDBNull(e.Item.DataItem("Address1")) Then Address &= e.Item.DataItem("Address1")
        If Not IsDBNull(e.Item.DataItem("Address2")) Then Address &= "<br>" & e.Item.DataItem("Address2")
        If Not IsDBNull(e.Item.DataItem("City")) Then Address &= "<br>" & e.Item.DataItem("City")
        If Not IsDBNull(e.Item.DataItem("State")) Then Address &= ", " & StateRow.GetRowByCode(DB, e.Item.DataItem("State")).StateName
        If Not IsDBNull(e.Item.DataItem("Region")) Then Address &= ", " & e.Item.DataItem("Region")
        If Not IsDBNull(e.Item.DataItem("Zip")) Then Address &= " " & e.Item.DataItem("Zip")
        If Not IsDBNull(e.Item.DataItem("Country")) Then Address &= "<br>" & CountryRow.GetRowByCode(DB, e.Item.DataItem("Country")).CountryName

        ltlLabel.Text = LabelName
        ltlName.Text = Core.BuildFullName(FirstName, String.Empty, LastName)
        ltlAddress.Text = address
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Response.Redirect("/members/addressbook/edit.aspx")
    End Sub
End Class