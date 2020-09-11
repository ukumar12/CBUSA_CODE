Imports Components
Imports Controls
Imports System.Data
Imports DataLayer
Imports System.Data.SqlClient

Partial Class Photos
    Inherits SitePage
    Protected VendorId As Integer
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        VendorId = Request("VendorId")
        If Not IsPostBack Then
            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ltlVendor.Text = VendorRow.GetRow(DB, VendorId).CompanyName

        SQLFields = "SELECT * "
        SQL = " FROM VendorPhoto Where VendorId = " & DB.Number(VendorId)

        Dim dt As DataTable = DB.GetDataTable(SQLFields & SQL)

        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                ltlPhotos.Text &= "<img src=""/assets/vendorphoto/" & row("Photo") & """ alt=""" & IIf(Not IsDBNull(row("AltText")), row("AltText"), "") & """ name=""" & IIf(Not IsDBNull(row("Caption")), row("Caption"), "") & """ />"
            Next
        Else
            'divPhotos.Visible = False
            divNoPhotos.Visible = True
        End If
        
    End Sub


End Class

