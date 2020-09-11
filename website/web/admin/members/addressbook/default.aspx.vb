Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_Addressbook_Default
    Inherits AdminPage

    Protected MemberId As Integer
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        MemberId = Convert.ToInt32(Request("MemberId"))
        Dim dbMember As MemberRow = MemberRow.GetRow(DB, MemberId)
        Dim dbMemberBilling As MemberAddressRow = MemberAddressRow.GetDefaultBillingRow(DB, MemberId)
        txtMemberName.Text = "<b>" & Core.BuildFullName(dbMemberBilling.FirstName, dbMemberBilling.MiddleInitial, dbMemberBilling.LastName) & "(" & dbMember.Username & ")</b>"
        lnkBack.HRef = "/admin/members/view.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All)
        Dim ds As DataTable = dbMember.GetAddressBook()
        gvAddressBook.DataSource = ds
        gvAddressBook.Pager.NofRecords = ds.Rows.Count
        gvAddressBook.DataBind()

    End Sub
    Protected Sub gvAddressBook_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvAddressBook.RowDataBound
        If Not e.Row.RowType = ListItemType.AlternatingItem And Not e.Row.RowType = ListItemType.Item Then
            Exit Sub
        End If

        Dim ltlLabel As Literal = CType(e.Row.FindControl("ltlLabel"), Literal)
        Dim ltlName As Literal = CType(e.Row.FindControl("ltlName"), Literal)
        Dim ltlAddress As Literal = CType(e.Row.FindControl("ltlAddress"), Literal)

        Dim FirstName As String = IIf(IsDBNull(e.Row.DataItem("FirstName")), String.Empty, e.Row.DataItem("FirstName"))
        Dim LastName As String = IIf(IsDBNull(e.Row.DataItem("LastName")), String.Empty, e.Row.DataItem("LastName"))
        Dim LabelName As String = IIf(IsDBNull(e.Row.DataItem("Label")), String.Empty, e.Row.DataItem("Label"))
        Dim FullAddress As String = String.Empty

        Dim Address As String = String.Empty
        If Not IsDBNull(e.Row.DataItem("Address1")) Then Address &= e.Row.DataItem("Address1")
        If Not IsDBNull(e.Row.DataItem("Address2")) Then Address &= "<br>" & e.Row.DataItem("Address2")
        If Not IsDBNull(e.Row.DataItem("City")) Then Address &= "<br>" & e.Row.DataItem("City")
        If Not IsDBNull(e.Row.DataItem("State")) Then Address &= ", " & StateRow.GetRowByCode(DB, e.Row.DataItem("State")).StateName
        If Not IsDBNull(e.Row.DataItem("Region")) Then Address &= ", " & e.Row.DataItem("Region")
        If Not IsDBNull(e.Row.DataItem("Zip")) Then Address &= " " & e.Row.DataItem("Zip")
        If Not IsDBNull(e.Row.DataItem("Country")) Then Address &= "<br>" & CountryRow.GetRowByCode(DB, e.Row.DataItem("Country")).CountryName

        ltlLabel.Text = LabelName
        ltlName.Text = Core.BuildFullName(FirstName, String.Empty, LastName)
        ltlAddress.Text = Address
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Response.Redirect("/admin/members/addressbook/edit.aspx?mode=Add&MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class