Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.IO

Partial Class Index
    Inherits AdminPage
 
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDORS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_VendorID.DataSource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.DataBind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))


            F_VendorID.SelectedValue = Request("F_VendorID")
 

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "v.CompanyName"
            BindList()
        End If
    End Sub

    Private Sub BindList()
        If F_VendorID.SelectedValue <> String.Empty Then
            Dim res As DataTable = VendorProductPriceRequestRow.GetPriceRequests(DB, F_VendorID.SelectedValue, "Created", "Asc")
            gvList.DataSource = res.DefaultView
            gvList.DataBind()
        End If
        
    End Sub
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub
        BindList()
    End Sub
    Protected Sub gvList_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvList.RowCommand
        If e.CommandName <> "DeletePriceRequest" Then
            Exit Sub
        End If
        Dim id As Integer = e.CommandArgument

        VendorProductPriceRequestRow.RemoveRow(DB, id)
        
        BindList()

    End Sub
    Protected Sub btnSubmitAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitAll.Click

        Dim i As Integer = 0
        Dim row As GridViewRow
        Dim isChecked As Boolean = False

        Dim VendorProductPriceRequestId As Integer = 0
        Dim SQL As String

        For i = 0 To gvList.Rows.Count - 1
            row = gvList.Rows(i)
            isChecked = CType(row.FindControl("chkSelect"), CheckBox).Checked

            If isChecked Then
                VendorProductPriceRequestId = CType(row.FindControl("ltlVendorProductPriceRequestId"), Label).Text
                VendorProductPriceRequestRow.RemoveRow(DB, VendorProductPriceRequestId)
            End If
        Next

        BindList()

    End Sub

    Protected Sub gvList_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If Not (e.Row.RowType = DataControlRowType.DataRow) Then
            Exit Sub
        End If
        Dim ltlVendorName As Literal = e.Row.FindControl("ltlVendorName")
        ltlVendorName.Text = DB.ExecuteScalar("SELECT CompanyName from Vendor where VendorID = " & DB.Number(F_VendorID.SelectedValue))

    End Sub
    End Class
