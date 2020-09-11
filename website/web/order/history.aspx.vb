Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data
Imports TwoPrice.DataLayer
Imports System.Collections.Generic

Partial Class order_history
    Inherits SitePage

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Private m_dtProducts As DataTable
    Private ReadOnly Property dvProducts(ByVal OrderId As Integer, ByVal IsTwoPrice As Boolean) As DataView
        Get
            If IsTwoPrice Then
                m_dtProducts = TwoPriceOrderRow.GetOrderProducts(DB, OrderId, "SortOrder")
            Else
                m_dtProducts = OrderRow.GetOrderProducts(DB, OrderId, "SortOrder")
            End If
            Return m_dtProducts.DefaultView
        End Get
    End Property

    Private m_dtStatus As DataTable
    Private ReadOnly Property dvStatus() As DataView
        Get
            If m_dtStatus Is Nothing Then
                m_dtStatus = OrderRow.GetBuilderOrderStatuses(DB, Session("BuilderID"))
            End If
            Return m_dtStatus.DefaultView
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'EnsureBuilderAccess()

        gvList.BindList = AddressOf BindData

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")

        If Not IsPostBack Then

            If Session("BuilderId") IsNot Nothing Then
                Core.DataLog("Order", PageURL, CurrentUserId, "Builder Top Menu Click", "", "", "", "", UserName)
            End If

            F_ProjectID.DataSource = ProjectRow.GetBuilderOrderProjects(DB, Session("BuilderID"), "ProjectName")
            F_ProjectID.DataTextField = "ProjectName"
            F_ProjectID.DataValueField = "ProjectID"
            F_ProjectID.DataBind()
            F_ProjectID.Items.Insert(0, New ListItem("Select Project", ""))

            F_ProjectID.SelectedValue = Request("F_ProjectID")

            F_TwoPriceCampaignID.Items.Insert(0, New ListItem("Select Campaign", ""))

            gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
            gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
            If gvList.SortBy = String.Empty Then
                gvList.SortBy = "Created"
                gvList.SortOrder = "DESC"
            End If

            gvList.PageSize = CInt(ddlPageSize.SelectedValue) '10
            gvList.Pager.NofRecords = OrderRow.GetBuilderOrderCount(DB, Session("BuilderId"))
            BindData()
        End If

    End Sub

    Private Sub BindData()
        ViewState("F_SortBy") = gvList.SortBy
        ViewState("F_SortOrder") = gvList.SortOrder

        Dim SQLFields As String = "SELECT TOP " & (gvList.PageIndex + 1) * gvList.PageSize & " o.*, p.ProjectName as Project, s.OrderStatus, v.CompanyName as VendorCompany "

        Dim orderTable As String = " SELECT OrderID,HistoricID,OrderNumber,VendorID,BuilderID,ProjectID,HistoricVendorID,HistoricBuilderID,HistoricProjectID," & _
                                    "Title,PONumber,OrdererFirstName,OrdererLastName,OrdererEmail,OrdererPhone,SuperFirstName,SuperLastName,SuperEmail,SuperPhone," & _
                                    "Subtotal,Tax,Total,RequestedDelivery,OrderStatusID,HistoricOrderStatusID,DeliveryInstructions,Notes,RemoteIP,Created,CreatorBuilderID," & _
                                     "VendorNotes,TakeoffID,SalesRepVendorAccountID,Updated,NULL As TwoPriceCampaignId,TaxRate" & _
                                     "  FROM [Order] " & _
                                    " UNION ALL " & _
                                    " Select TwoPriceOrderID,HistoricID,OrderNumber,VendorID,BuilderID,ProjectID,HistoricVendorID,HistoricBuilderID,HistoricProjectID, " & _
                                    "Title,PONumber,OrdererFirstName,OrdererLastName,OrdererEmail,OrdererPhone,SuperFirstName,SuperLastName,SuperEmail,SuperPhone," & _
                                    "Subtotal,Tax,Total,RequestedDelivery,OrderStatusID,HistoricOrderStatusID,DeliveryInstructions,Notes,RemoteIP,Created,CreatorBuilderID," & _
                                    "VendorNotes,TwoPriceTakeoffID,SalesRepVendorAccountID,Updated,TwoPriceCampaignId,TaxRate" & _
                                    " From TwoPriceOrder "

        Dim sql As String = _
              " from (" & orderTable & ") o left outer join Project p on o.ProjectID=p.ProjectID " _
            & "     left outer join OrderStatus s on o.OrderStatusID=s.OrderStatusID" _
            & "     inner join Vendor v on o.VendorId=v.VendorId" _
            & " where o.BuilderID=" & DB.Number(Session("BuilderId"))

        If F_ProjectID.SelectedValue <> Nothing Then
            sql &= " and o.ProjectID=" & DB.Number(F_ProjectID.SelectedValue)
        End If

        If F_StartDate.Value <> Nothing Then
            sql &= " and o.Created >= " & DB.Quote(F_StartDate.Value)
        End If

        If F_EndDate.Value <> Nothing Then
            sql &= " and o.Created <= " & DB.Quote(F_EndDate.Value)
        End If

        If F_TwoPriceCampaignID.SelectedValue <> Nothing Then
            sql &= " and o.TwoPriceCampaignId = " & DB.Quote(F_TwoPriceCampaignID.SelectedValue)
        End If

        sql &= " and s.OrderStatus != 'Unsaved' "

        gvList.Pager.NofRecords = DB.ExecuteScalar("select count(*) " & sql)
        'Dim res As DataTable = OrderRow.GetBuilderOrders(DB, Session("BuilderId"), F_ProjectID.SelectedValue, gvList.SortBy, gvList.SortOrder, gvList.PageIndex, gvList.PageSize)
        If gvList.SortBy <> Nothing Then
            sql &= " order by " & Core.ProtectParam(gvList.SortByAndOrder)
        End If
        gvList.DataSource = DB.GetDataTable(SQLFields & sql)
        gvList.DataBind()
    End Sub

    Protected Sub frmDetails_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim form As PopupForm.PopupForm = sender
        Dim row As GridViewRow = CType(form.NamingContainer, GridViewRow)

        If row IsNot Nothing And row.RowType = DataControlRowType.DataRow AndAlso row.DataItem IsNot Nothing Then
            CType(form.FindControl("ltlOrderDate"), Literal).Text = FormatDateTime(Core.GetDate(row.DataItem("Created")), DateFormat.ShortDate)
            CType(form.FindControl("ltlVendor"), Literal).Text = Core.GetString(row.DataItem("VendorCompany"))
            CType(form.FindControl("ltlProject"), Literal).Text = Core.GetString(row.DataItem("Project"))
            CType(form.FindControl("ltlOrderTitle"), Literal).Text = Core.GetString(row.DataItem("Title"))
            CType(form.FindControl("ltlOrderPlacerName"), Literal).Text = Core.BuildFullName(Core.GetString(row.DataItem("OrdererFirstName")), String.Empty, Core.GetString(row.DataItem("OrdererLastName")))
            CType(form.FindControl("ltlOrderPlacerPhone"), Literal).Text = Core.GetString(row.DataItem("OrdererPhone"))
            CType(form.FindControl("ltlOrderPlacerEmail"), Literal).Text = Core.GetString(row.DataItem("OrdererEmail"))
            CType(form.FindControl("ltlSuperName"), Literal).Text = Core.BuildFullName(Core.GetString(row.DataItem("SuperFirstName")), String.Empty, Core.GetString(row.DataItem("SuperLastName")))
            CType(form.FindControl("ltlSuperPhone"), Literal).Text = Core.GetString(row.DataItem("SuperPhone"))
            CType(form.FindControl("ltlSuperEmail"), Literal).Text = Core.GetString(row.DataItem("SuperEmail"))
            If Not IsDBNull(row.DataItem("Subtotal")) Then
                CType(form.FindControl("ltlSubtotal"), Literal).Text = FormatCurrency(row.DataItem("Subtotal"))
            End If
            If Not IsDBNull(row.DataItem("Tax")) Then
                CType(form.FindControl("ltlTax"), Literal).Text = FormatCurrency(row.DataItem("Tax"))
            End If
            If Not IsDBNull(row.DataItem("Total")) Then
                CType(form.FindControl("ltlTotal"), Literal).Text = FormatCurrency(row.DataItem("Total"))
            End If

            Dim gvItems As GridView = form.FindControl("gvItems")
            gvItems.DataSource = dvProducts(row.DataItem("OrderId"), CBool(form.Attributes("IsTwoPrice")))
            gvItems.DataBind()


        End If
    End Sub

    Protected Sub gvList_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowCreated
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim frmDetails As PopupForm.PopupForm = e.Row.FindControl("frmDetails")

        If e.Row.DataItem IsNot Nothing Then
            If e.Row.DataItem("TwoPriceCampaignId") IsNot DBNull.Value Then
                frmDetails.Attributes.Add("IsTwoPrice", "True")

                trCampaign.Visible = True
                Dim dbTwoPriceCampaign As TwoPriceCampaignRow = TwoPriceCampaignRow.GetRow(DB, e.Row.DataItem("TwoPriceCampaignId"))
                Dim li As New ListItem(dbTwoPriceCampaign.Name, dbTwoPriceCampaign.TwoPriceCampaignId)
                If Not F_TwoPriceCampaignID.Items.Contains(li) Then F_TwoPriceCampaignID.Items.Add(li)
            Else
                frmDetails.Attributes.Add("IsTwoPrice", "False")
            End If
        End If

        AddHandler frmDetails.TemplateLoaded, AddressOf frmDetails_TemplateLoaded
    End Sub


    Protected Sub gvList_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvList.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then
            Exit Sub
        End If

        Dim lnkShowDetails As ImageButton = e.Row.FindControl("lnkShowDetails")
        Dim form As PopupForm.PopupForm = e.Row.FindControl("frmDetails")

        'lnkShowDetails.OnClientClick = "javascript:return ShowDetails('" & form.ClientID & "_window');"

        'Dim spnClose As HtmlControls.HtmlGenericControl = form.FindControl("spanClose")
        'spnClose.Attributes.Add("onclick", "javascript:return HideDetails('" & form.ClientID & "_window');")

        lnkShowDetails.OnClientClick = "javascript:return ShowDetails('" & form.ClientID & "_window_" & e.Row.RowIndex & "');"

        Dim spnClose As HtmlControls.HtmlGenericControl = form.FindControl("spanClose")
        spnClose.Attributes.Add("onclick", "javascript:return HideDetails('" & form.ClientID & "_window_" & e.Row.RowIndex & "');")

        Dim lnkDetails As HyperLink = e.Row.FindControl("lnkDetails")

        Dim unprocessedCode As Integer = (From s In dvStatus Where Core.GetInt(s("StatusCode")) = Convert.ToInt32(OrderStatus.Unprocessed) Select s("StatusCode")).FirstOrDefault
        Dim rowStatusId As Integer = Core.GetInt(e.Row.DataItem("OrderStatusID"))
        If rowStatusId = unprocessedCode Then
            lnkDetails.Text = "Edit Order"
            lnkDetails.NavigateUrl = "/order/default.aspx?OrderID=" & e.Row.DataItem("OrderID")
        Else
            lnkDetails.Text = "Order Details"
            lnkDetails.NavigateUrl = "/order/summary.aspx?OrderID=" & e.Row.DataItem("OrderID")
        End If

        If e.Row.DataItem("TwoPriceCampaignId") IsNot DBNull.Value Then
            lnkDetails.NavigateUrl &= "&twoprice=y"
        End If
    End Sub

    Protected Sub btnFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFilter.Click
        BindData()
    End Sub

    Protected Sub frmDelete_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmDelete.Postback

        Dim OrderID As Integer = hdnOrderId.Value
        Dim IsTwoPrice As Boolean = hdnIsTwoPrice.Value
        Dim dbOrder As Object

        If IsTwoPrice Then
            dbOrder = TwoPriceOrderRow.GetRow(DB, OrderID)
        Else
            dbOrder = OrderRow.GetRow(DB, OrderID)
        End If
        Dim dbVendor As VendorRow = VendorRow.GetRow(DB, dbOrder.VendorID)
        Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "OrderDeleted")
        dbMsg.Send(dbVendor)
        dbOrder.Remove()
        BindData()
    End Sub

    Private Sub ddlPageSize_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlPageSize.SelectedIndexChanged
        gvList.SortBy = Core.ProtectParam(Request("F_SortBy"))
        gvList.SortOrder = Core.ProtectParam(Request("F_SortOrder"))
        If gvList.SortBy = String.Empty Then
            gvList.SortBy = "Created"
            gvList.SortOrder = "DESC"
        End If

        gvList.PageSize = CInt(ddlPageSize.SelectedValue) '10
        gvList.Pager.NofRecords = OrderRow.GetBuilderOrderCount(DB, Session("BuilderId"))
        BindData()
    End Sub

End Class
