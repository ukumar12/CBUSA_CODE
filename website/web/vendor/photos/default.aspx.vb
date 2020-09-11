Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class VendorPhotos
    Inherits SitePage
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        EnsureVendorAccess()

        If Not CType(Me.Page, SitePage).IsLoggedInVendor Then
            Response.Redirect("/default.aspx")
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName= Session("UserName")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            BindList()
         Core.DataLog("Photo Gallery", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        SQLFields = "SELECT * "
        SQL = " FROM VendorPhoto Where VendorId = " & DB.Number(Session("VendorId"))

        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()

        If res.Rows.Count > 0 Then
            Preview.NavigateUrl = "photos.aspx?VendorId=" & Session("VendorId")
        Else
            Preview.Visible = False
        End If

        'If res.Rows.Count > 9 Then
        '    AddNew.Visible = False
        'End If

    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        'log Btn Add photos Click
        Core.DataLog("Photo Gallery", PageURL, CurrentUserId, "Btn Add Photo", "", "", "", "", UserName)
        'end log
        Response.Redirect("edit.aspx")
    End Sub


   ' Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
       ' Response.Redirect("/vendor/default.aspx")
   ' End Sub

End Class

