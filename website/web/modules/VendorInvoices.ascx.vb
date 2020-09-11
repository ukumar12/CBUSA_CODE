Option Strict Off

Imports Components
Imports DataLayer
Imports TwoPrice.DataLayer

Partial Class _default
    Inherits ModuleControl

    Protected m_ReturnCount As Integer = 0
    Protected m_DisplayViewAllLink As Boolean = True

    Private m_dtStatuses As DataTable
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Private ReadOnly Property dtStatuses() As DataTable
        Get
            If m_dtStatuses Is Nothing Then
                m_dtStatuses = OrderStatusRow.GetList(DB, "StatusCode")
            End If
            Return m_dtStatuses
        End Get
    End Property

    Public Property ReturnCount() As Integer
        Get
            If IsAdminDisplay And IsDesignMode Then
                If drpReturnCount.SelectedValue = "" Then
                    Return 0
                Else
                    Return CType(drpReturnCount.SelectedValue, Integer)
                End If
            Else
                Return m_ReturnCount
            End If
        End Get
        Set(ByVal value As Integer)
            If IsAdminDisplay And IsDesignMode Then
                drpReturnCount.SelectedValue = value
            Else
                m_ReturnCount = value
            End If
        End Set
    End Property

    Public Property DisplayViewAllLink() As Boolean
        Get
            If IsAdminDisplay And IsDesignMode Then
                If drpDisplayViewAllLink.SelectedValue = "" Then
                    Return False
                Else
                    Return CType(drpDisplayViewAllLink.SelectedValue, Boolean)
                End If
            Else
                Return m_ReturnCount
            End If
        End Get
        Set(ByVal value As Boolean)
            If IsAdminDisplay And IsDesignMode Then
                drpReturnCount.SelectedValue = IIf(value, 1, 0)
            Else
                m_DisplayViewAllLink = value
            End If
        End Set
    End Property

    Public Overrides Property Args() As String
        Get
            Return "ReturnCount=" & ReturnCount & "&DisplayViewAllLink=" & DisplayViewAllLink
        End Get
        Set(ByVal Value As String)
            If Value = String.Empty Then Exit Property
            Dim Pairs() As String = Value.Split("&"c)

            If Pairs.Length >= 1 Then
                Dim ReturnCountValues() As String = Pairs(0).Split("="c)
                If ReturnCountValues.Length > 0 Then
                    ReturnCount = CType(ReturnCountValues(1), Integer)
                End If
            End If

            If Pairs.Length >= 2 Then
                Dim DisplayViewAllLinkValues() As String = Pairs(1).Split("="c)
                If DisplayViewAllLinkValues.Length > 0 Then
                    DisplayViewAllLink = CType(DisplayViewAllLinkValues(1), Boolean)
                End If
            End If

        End Set
    End Property

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        divDesigner.Visible = IsDesignMode

        If IsDesignMode Then
            If IsPostBack Then
                CheckPostData(Controls)
            End If
            hdnField.Value = Args
        End If

    End Sub

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim SQL As String = String.Empty
        Dim dt As DataTable

        gvNewOrders.BindList = AddressOf BindData

       PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then
            Core.DataLog("Orders", PageURL, CurrentUserId, "Vendor Top Menu Click", "", "", "", "", UserName)

            gvNewOrders.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvNewOrders.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvNewOrders.SortBy = String.Empty Then
                gvNewOrders.SortBy = "Created"
                gvNewOrders.SortOrder = "Desc"
            End If

            BindData()
            'SQL = GridSQL()

            'If SQL <> String.Empty Then

            '    dt = DB.GetDataTable(SQL)
            '    gvNewOrders.DataKeyNames = New String() {"OrderID"}
            '    Me.gvNewOrders.DataSource = dt
            '    Me.gvNewOrders.DataBind()

            '    If DisplayViewAllLink Then
            '        Me.lnkViewAll.Visible = True
            '    Else
            '        Me.lnkViewAll.Visible = False
            '    End If

            'End If
        End If

        'If IsAdminDisplay And IsDesignMode Then
        '    drpReturnCount.Items.Insert(0, New ListItem("5", "5"))
        '    drpReturnCount.Items.Insert(0, New ListItem("10", "10"))
        '    drpReturnCount.Items.Insert(0, New ListItem("15", "15"))
        '    drpReturnCount.Items.Insert(0, New ListItem("25", "25"))
        '    drpReturnCount.Items.Insert(0, New ListItem("ALL", "0"))

        '    drpDisplayViewAllLink.Items.Insert(0, New ListItem("True", "1"))
        '    drpDisplayViewAllLink.Items.Insert(0, New ListItem("False", "0"))
        'End If

    End Sub

    Private Sub BindData()
        Dim sqlFields As String = "select top " & (gvNewOrders.PageIndex + 1) * gvNewOrders.PageSize & " o.*, b.CompanyName as Builder, s.OrderStatus"

        Dim orderTable As String = " SELECT *, NULL As TwoPriceCampaignId, NULL As ImportedTakeOffId FROM [Order] " & _
                            " UNION ALL " & _
                            " Select * From TwoPriceOrder "

        Dim sql As String = _
              " from (" & orderTable & ") o inner join Builder b on o.BuilderID=b.BuilderID left outer join OrderStatus s on o.OrderStatusID=s.OrderStatusID" _
            & " where o.VendorID=" & DB.Number(Session("VendorId")) _
            & " and (SELECT TOP 1 StatusCode from OrderStatus WHERE OrderStatusID=o.OrderStatusID) > (SELECT MIN(StatusCode) FROM OrderStatus) and s.OrderStatus != 'Unsaved' "
        If DisplayViewAllLink Then
            sql &= " and s.StatusCode != 5 "
        End If
        gvNewOrders.Pager.NofRecords = DB.ExecuteScalar("select count(*)" & sql)

        gvNewOrders.DataKeyNames = New String() {"OrderID"}
        gvNewOrders.DataSource = DB.GetDataTable(sqlFields & sql & " order by " & gvNewOrders.SortByAndOrder).DefaultView
        gvNewOrders.DataBind()
    End Sub

    Private Function GridSQL() As String

        Dim SQL As String = String.Empty

        SQL = "SELECT " & IIf(ReturnCount <> 0, " TOP " & ReturnCount.ToString, "") & vbCrLf
        SQL &= "  o.*," & vbCrLf
        SQL &= "  b.CompanyName Builder," & vbCrLf
        SQL &= "  o.Created as Submitted" & vbCrLf
        SQL &= "FROM" & vbCrLf
        SQL &= "  [ORDER] o" & vbCrLf
        SQL &= "  JOIN Builder b ON o.BuilderID = b.BuilderID" & vbCrLf
        SQL &= "WHERE" & vbCrLf
        SQL &= "  VendorID = " & Session("VendorId") & vbCrLf
        SQL &= "AND" & vbCrLf
        SQL &= "  (SELECT TOP 1 StatusCode from OrderStatus WHERE OrderStatusID=o.OrderStatusID) > (SELECT MIN(StatusCode) FROM OrderStatus)"
        SQL &= "ORDER BY" & vbCrLf
        SQL &= "  o.Created" & vbCrLf

        Return SQL

    End Function

    Protected Sub gvNewOrders_PageIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvNewOrders.PageIndexChanged
        BindData()
    End Sub

    Protected Sub gvNewOrders_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvNewOrders.RowCreated
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim frm As PopupForm.PopupForm = e.Row.FindControl("frmStatus")
        AddHandler frm.TemplateLoaded, AddressOf frmStatus_TemplateLoaded
        AddHandler frm.Postback, AddressOf frmStatus_Postback
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(frm)
    End Sub

    Protected Sub frmStatus_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim frmStatus As PopupForm.PopupForm = sender
        Dim row As GridViewRow = frmStatus.NamingContainer
        Dim drpStatus As DropDownList = frmStatus.FindControl("drpStatus")
        Dim txtComments As TextBox = frmStatus.FindControl("txtComments")
        Dim ltlOrderTitle As Literal = frmStatus.FindControl("ltlOrderTitle")

        drpStatus.DataSource = dtStatuses
        drpStatus.DataTextField = "OrderStatus"
        drpStatus.DataValueField = "OrderStatusID"
        drpStatus.DataBind()
        If row.DataItem IsNot Nothing Then
            drpStatus.SelectedValue = Core.GetInt(row.DataItem("OrderStatusID"))
            txtComments.Text = Core.GetString(row.DataItem("VendorNotes"))
            ltlOrderTitle.Text = row.DataItem("Title") & " (#" & row.DataItem("OrderNumber") & ")"
            hdnIsTwoPrice.Value = (row.DataItem("TwoPriceCampaignId") IsNot DBNull.Value).ToString
        End If
    End Sub

    Protected Sub frmStatus_Postback(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim frmStatus As PopupForm.PopupForm = CType(sender, Control).NamingContainer
        Dim row As GridViewRow = frmStatus.NamingContainer
        Dim drpStatus As DropDownList = frmStatus.FindControl("drpStatus")
        Dim txtComments As TextBox = frmStatus.FindControl("txtComments")
        Dim hdnIsTwoPrice As HiddenField = frmStatus.FindControl("hdnIsTwoPrice")
        Dim OrderID As Integer = gvNewOrders.DataKeys(row.RowIndex)(0)
        Dim dbOrder As Object

        If CBool(hdnIsTwoPrice.Value) Then
            dbOrder = TwoPriceOrderRow.GetRow(DB, OrderID)
        Else
            dbOrder = OrderRow.GetRow(DB, OrderID)
        End If

        If dbOrder.OrderStatusID <> drpStatus.SelectedValue Then
            Dim dbHistory As New OrderStatusHistoryRow(DB)
            dbHistory.OrderID = dbOrder.OrderID
            dbHistory.CreatorVendorAccountID = Session("VendorAccountId")
            dbHistory.OrderStatusID = drpStatus.SelectedValue
            dbHistory.Insert()
        End If

        dbOrder.OrderStatusID = drpStatus.SelectedValue
        dbOrder.VendorNotes = txtComments.Text
        dbOrder.Update()
        Core.DataLog("Order", PageURL, CurrentUserId, "Update Order Status",dbOrder.OrderID , "", "", "", UserName)

        Dim dbStatus As OrderStatusRow = OrderStatusRow.GetRow(DB, dbOrder.OrderStatusID)
        Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbOrder.BuilderID)
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "OrderStatusUpdate")
        Dim msg As String = vbCrLf & vbCrLf _
            & "Order #:         " & dbOrder.OrderNumber & vbCrLf _
            & "Title:           " & dbOrder.Title & vbCrLf _
            & "Current Status:  " & dbStatus.OrderStatus & vbCrLf
        If dbOrder.VendorNotes <> String.Empty Then
            msg &= "Vendor Comments:    " & dbOrder.VendorNotes & vbCrLf
        End If

        dbMsg.Send(dbBuilder, msg)

        Dim s As String =
              " function() {" _
            & "     Sys.Application.remove_loaded(arguments.callee);" _
            & "     window.frm = $get('" & frmStatus.ClientID & "').control;" _
            & "     window.frm.Close(); alert('hhhh');" _
            & "     window.frm = undefined;" _
            & "     Sys.Application.add_loaded(OpenInvoicesConfirm);" _
            & " }"

        BindData()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "InvoiceConfirm", s, True)
    End Sub

    Protected Sub gvNewOrders_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvNewOrders.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim ltlStatus As Literal = e.Row.FindControl("ltlStatus")
        ltlStatus.Text = Core.GetString(e.Row.DataItem("OrderStatus"))

        Dim btnOpenStatus As ImageButton = e.Row.FindControl("btnOpenStatus")
        Dim form As PopupForm.PopupForm = e.Row.FindControl("frmStatus")

        btnOpenStatus.OnClientClick = "javascript:return ShowDetails('" & form.ClientID & "_window_" & e.Row.RowIndex & "');"

        Dim btnCancelStatus As Button = form.FindControl("btnCancelStatus")
        btnCancelStatus.OnClientClick = "javascript:return HideDetails('" & form.ClientID & "_window_" & e.Row.RowIndex & "');"

    End Sub

    Private Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPageSize.SelectedIndexChanged
        gvNewOrders.PageSize = CInt(ddlPageSize.SelectedValue)

        gvNewOrders.SortBy = Core.ProtectParam(Request("F_SortBy"))
        gvNewOrders.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
        If gvNewOrders.SortBy = String.Empty Then
            gvNewOrders.SortBy = "Created"
            gvNewOrders.SortOrder = "Desc"
        End If

        BindData()
    End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
    ' Response.Redirect("/vendor/default.aspx")
    ' End Sub

End Class
