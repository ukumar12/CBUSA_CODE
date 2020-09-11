Imports Components
Imports Controls
Imports System.Data
Imports DataLayer

Partial Class Index
    Inherits AdminPage

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckAccess("ORDERS")

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

            F_ProjectID.Datasource = ProjectRow.GetList(DB, "ProjectName")
            F_ProjectID.DataValueField = "ProjectID"
            F_ProjectID.DataTextField = "ProjectName"
            F_ProjectID.Databind()
            F_ProjectID.Items.Insert(0, New ListItem("-- ALL --", ""))

            F_OrderNumber.Text = Request("F_OrderNumber")
            F_Title.Text = Request("F_Title")
            F_PONumber.Text = Request("F_PONumber")
            F_OrdererLastName.Text = Request("F_OrdererLastName")
            F_OrdererEmail.Text = Request("F_OrdererEmail")
            F_SuperLastName.Text = Request("F_SuperLastName")
            F_SuperEmail.Text = Request("F_SuperEmail")
            F_VendorID.SelectedValue = Request("F_VendorID")
            F_BuilderID.SelectedValue = Request("F_BuilderID")
            F_ProjectID.SelectedValue = Request("F_ProjectID")
            F_RequestedDeliveryLBound.Text = Request("F_RequestedDeliveryLBound")
            F_RequestedDeliveryUBound.Text = Request("F_RequestedDeliveryUBound")

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then gvList.SortBy = "OrderID"

            BindList()
        End If
    End Sub

    Private Sub BindList()
        Dim SQLFields, SQL As String
        Dim Conn As String = " where "

        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        SQLFields = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " * "
        SQL = " FROM Order  "

        If Not F_VendorID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "VendorID = " & DB.Quote(F_VendorID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_BuilderID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "BuilderID = " & DB.Quote(F_BuilderID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_ProjectID.SelectedValue = String.Empty Then
            SQL = SQL & Conn & "ProjectID = " & DB.Quote(F_ProjectID.SelectedValue)
            Conn = " AND "
        End If
        If Not F_OrderNumber.Text = String.Empty Then
            SQL = SQL & Conn & "OrderNumber LIKE " & DB.FilterQuote(F_OrderNumber.Text)
            Conn = " AND "
        End If
        If Not F_Title.Text = String.Empty Then
            SQL = SQL & Conn & "Title LIKE " & DB.FilterQuote(F_Title.Text)
            Conn = " AND "
        End If
        If Not F_PONumber.Text = String.Empty Then
            SQL = SQL & Conn & "PONumber LIKE " & DB.FilterQuote(F_PONumber.Text)
            Conn = " AND "
        End If
        If Not F_OrdererLastName.Text = String.Empty Then
            SQL = SQL & Conn & "OrdererLastName LIKE " & DB.FilterQuote(F_OrdererLastName.Text)
            Conn = " AND "
        End If
        If Not F_OrdererEmail.Text = String.Empty Then
            SQL = SQL & Conn & "OrdererEmail LIKE " & DB.FilterQuote(F_OrdererEmail.Text)
            Conn = " AND "
        End If
        If Not F_SuperLastName.Text = String.Empty Then
            SQL = SQL & Conn & "SuperLastName LIKE " & DB.FilterQuote(F_SuperLastName.Text)
            Conn = " AND "
        End If
        If Not F_SuperEmail.Text = String.Empty Then
            SQL = SQL & Conn & "SuperEmail LIKE " & DB.FilterQuote(F_SuperEmail.Text)
            Conn = " AND "
        End If
        If Not F_RequestedDeliveryLBound.Text = String.Empty Then
            SQL = SQL & Conn & "RequestedDelivery >= " & DB.Quote(F_RequestedDeliveryLBound.Text)
            Conn = " AND "
        End If
        If Not F_RequestedDeliveryUBound.Text = String.Empty Then
            SQL = SQL & Conn & "RequestedDelivery < " & DB.Quote(DateAdd("d", 1, F_RequestedDeliveryUBound.Text))
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

