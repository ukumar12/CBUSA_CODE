Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data
Imports TwoPrice.DataLayer

Partial Class order_drops
    Inherits SitePage

    Protected OrderId As Integer
    Private IsTwoPrice As Boolean
    Private dbOrder As Object
    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        OrderId = Request("OrderID")
        IsTwoPrice = Request("twoprice") = "y"

        If OrderId = Nothing Then
            Response.Redirect("default.aspx")
        End If
        If IsTwoPrice Then
            dbOrder = TwoPriceOrderRow.GetRow(DB, OrderId)
        Else
            dbOrder = OrderRow.GetRow(DB, OrderId)
        End If

        If dbOrder.OrderID = Nothing OrElse dbOrder.BuilderID <> Session("BuilderId") Then
            Response.Redirect("default.aspx")
        End If
        Dim dbStatus As OrderStatusRow = OrderStatusRow.GetRow(DB, OrderStatus.Unprocessed)
        If dbOrder.OrderStatusID = Nothing OrElse dbOrder.OrderStatusID = dbStatus.OrderStatusID Then
            btnReturnOrders.Visible = False
        Else
            btnCancelOrder.Visible = False
            btnSubmitOrder.Visible = False
        End If

        ctlDrops.OrderId = OrderId
        If IsTwoPrice Then
            ctlDrops.IsTwoPriceOrder = True
        Else
            ctlDrops.IsTwoPriceOrder = False
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("BuilderId")
        UserName = Session("Username")
    End Sub

    Protected Sub frmDrop_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmDrop.Callback
        Dim form As PopupForm.PopupForm = sender
        If Not form.IsValid Then Exit Sub


        Dim ItemIdx As Integer = args.Data("hdnItemIdx")
        Dim OrderDropId As Integer = IIf(args.Data("hdnOrderDropId") = String.Empty, Nothing, args.Data("hdnOrderDropId"))

        If IsTwoPrice Then
            Dim dbTwoPriceDrop As TwoPriceOrderDropRow = TwoPriceOrderDropRow.GetRow(DB, OrderDropId)
            dbTwoPriceDrop.DeliveryInstructions = args.Data("txtInstructions")
            dbTwoPriceDrop.DropName = args.Data("txtName")
            dbTwoPriceDrop.Notes = args.Data("txtNotes")
            dbTwoPriceDrop.OrderID = OrderId
            dbTwoPriceDrop.RequestedDelivery = args.Data("dpDelivery")

            If dbTwoPriceDrop.Created = Nothing Then
                dbTwoPriceDrop.CreatorBuilderID = Session("BuilderId")
                dbTwoPriceDrop.Insert()
                Dim drop As New Controls.DropDetails()
                drop.id = dbTwoPriceDrop.OrderDropID
                drop.instructions = dbTwoPriceDrop.DeliveryInstructions
                drop.name = dbTwoPriceDrop.DropName
                drop.notes = dbTwoPriceDrop.Notes
                Dim json As New System.Web.Script.Serialization.JavaScriptSerializer
                form.CallbackResult = "{'drop':" & json.Serialize(drop) & ",'itemIdx':" & ItemIdx & "}"
            Else
                dbTwoPriceDrop.Update()
                'log Edit/Save Drops
                Core.DataLog("Order", PageURL, CurrentUserId, "Edit And Save Drop", ctlDrops.OrderId, "", "", "", UserName)
                'end log
            End If

        Else
            Dim dbDrop As OrderDropRow = OrderDropRow.GetRow(DB, OrderDropId)
            dbDrop.DeliveryInstructions = args.Data("txtInstructions")
            dbDrop.DropName = args.Data("txtName")
            dbDrop.Notes = args.Data("txtNotes")
            dbDrop.OrderID = OrderId
            dbDrop.RequestedDelivery = args.Data("dpDelivery")

            If dbDrop.Created = Nothing Then
                dbDrop.CreatorBuilderID = Session("BuilderId")
                dbDrop.Insert()
                Dim drop As New Controls.DropDetails()
                drop.id = dbDrop.OrderDropID
                drop.instructions = dbDrop.DeliveryInstructions
                drop.name = dbDrop.DropName
                drop.notes = dbDrop.Notes
                Dim json As New System.Web.Script.Serialization.JavaScriptSerializer
                form.CallbackResult = "{'drop':" & json.Serialize(drop) & ",'itemIdx':" & ItemIdx & "}"
            Else
                dbDrop.Update()
            End If
        End If



      
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click


        Dim dtItems As DataTable = OrderProductRow.GetOrderProducts(DB, dbOrder.OrderID)


        Dim bValid As Boolean = True
        If dbOrder.RequestedDelivery = Nothing Then
            For Each item As DataRow In dtItems.Rows
                If IsDBNull(item("DropID")) Then
                    bValid = False
                End If
            Next
            If Not bValid Then
                AddError("All items must be assigned to a drop.")
                Exit Sub
            End If
        End If
    End Sub

    Protected Sub btnSubmitOrder_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitOrder.Click
        DB.BeginTransaction()
        Try
            'Dim dbStatus As OrderStatusRow = OrderStatusRow.GetStatusByName(DB, "Submitted")
            'dbOrder.OrderStatusID = dbStatus.OrderStatusID

            'dbOrder.Update()

            'Dim dbHistory As New OrderStatusHistoryRow(DB)
            'dbHistory.OrderID = dbOrder.OrderID
            'dbHistory.OrderStatusID = dbOrder.OrderStatusID
            'dbHistory.CreatorBuilderAccountID = Session("BuilderAccountId")
            'dbHistory.Insert()

            DB.CommitTransaction()

            'log Drops Btn Continue Click
            Core.DataLog("Order", PageURL, CurrentUserId, "Continue To Order Summary Page From Drop", ctlDrops.OrderId, "", "", "", UserName)
            'end log

            Response.Redirect("summary.aspx?OrderId=" & dbOrder.OrderID & "&twoprice=" & Request("twoprice"))
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            Logger.Error(Logger.GetErrorMessage(ex))
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnReturnOrders_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReturnOrders.Click
        Response.Redirect("/order/history.aspx")
    End Sub
End Class
