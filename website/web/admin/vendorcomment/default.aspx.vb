﻿Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("VENDOR_COMMENTS")

        gvList.BindList = AddressOf BindList
        If Not IsPostBack Then
            F_VendorID.Datasource = VendorRow.GetList(DB, "CompanyName")
            F_VendorID.DataValueField = "VendorID"
            F_VendorID.DataTextField = "CompanyName"
            F_VendorID.Databind()
            F_VendorID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_BuilderID.Datasource = BuilderRow.GetList(DB, "CompanyName")
            F_BuilderID.DataValueField = "BuilderID"
            F_BuilderID.DataTextField = "CompanyName"
            F_BuilderID.Databind()
            F_BuilderID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_VendorID.SelectedValue = Request("F_VendorID")
            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_SubmittedLBound.Text = Request("F_SubmittedLBound")
            F_SubmittedUBound.Text = Request("F_SubmittedUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "VendorCommentID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " c.*, b.CompanyName as BuilderCompany, v.CompanyName as VendorCompany "
        SQL = " FROM VendorComment c inner join Builder b on c.BuilderID=b.BuilderID inner join Vendor v on v.VendorID=c.VendorID "

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "c.VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "c.BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_SubmittedLBound.Text = String.Empty Then
            SQL = SQL & Conn & "c.Submitted >= " & DB.Quote(F_SubmittedLbound.Text)
            Conn = " AND "
        End If
        If Not F_SubmittedUBound.Text = String.Empty Then
            SQL = SQL & Conn & "c.Submitted < " & DB.Quote(DateAdd("d", 1, F_SubmittedUbound.Text))
            Conn = " AND "
        End If
        gvList.Pager.NofRecords = DB.ExecuteScalar("SELECT COUNT(*) " & SQL)

        Dim res As DataTable = DB.GetDataTable(SQLFields & SQL & " ORDER BY " & gvList.SortByAndOrder)
        gvList.DataSource = res.DefaultView
        gvList.DataBind()
    End Sub

    Private Sub AddNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddNew.Click
        Response.Redirect("edit.aspx?" & GetPageParams(FilterFieldType.All))
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Not IsValid Then Exit Sub

        gvList.PageIndex = 0
        BindList()
    End Sub
End Class

