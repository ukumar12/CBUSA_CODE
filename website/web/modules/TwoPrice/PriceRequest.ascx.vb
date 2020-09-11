Imports Components
Imports DataLayer
Imports System.Linq
Imports System.Data
Imports TwoPrice.DataLayer

Partial Class modules_TwoPrice_PriceRequest
    Inherits ModuleControl

    Private Property EditIndex() As Integer
        Get
            Return ViewState("EditIndex") - 1
        End Get
        Set(ByVal value As Integer)
            ViewState("EditIndex") = value + 1
        End Set
    End Property

    Private ReadOnly Property Keys() As Generic.List(Of String)
        Get
            If ViewState("Keys") Is Nothing Then
                ViewState("Keys") = New Generic.List(Of String)
            End If
            Return ViewState("Keys")
        End Get
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If IsAdminDisplay Then
            ctlSearch.Visible = False
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsAdminDisplay Then
            Exit Sub
        End If
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(ctlNavigator)
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(rblPricedOnly)

        If Not IsPostBack Then
            BindData()
        Else
            If rblPricedOnly.SelectedValue = True Then
                'ctlSearch.FilterListCallback = AddressOf FilterListCallback
                'ctlSearch.FilterCacheKey = Session("VendorId")
                Dim list As New Generic.List(Of String)
                FilterListCallback(list)
                ctlSearch.FilterList = list
            Else
                'ctlSearch.FilterListCallback = Nothing
                'ctlSearch.FilterCacheKey = Nothing
                ctlSearch.FilterList = Nothing
            End If
            If hdnRequestId.Value <> Nothing Then
                Dim dbRequest As VendorProductPriceRequestRow = VendorProductPriceRequestRow.GetRow(DB, hdnRequestId.Value)
                If dbRequest.ProductID <> Nothing Then
                    Dim Requested As ProductRow = ProductRow.GetRow(DB, dbRequest.ProductID)
                    spanSubHeaderProduct.InnerHtml = Requested.Product
                ElseIf dbRequest.SpecialOrderProductID <> Nothing Then
                    Dim Requested As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, dbRequest.SpecialOrderProductID)
                    spanSubHeaderProduct.InnerHtml = Requested.SpecialOrderProduct
                End If
            End If
        End If
    End Sub

    Private Sub BindData()
        Dim res As DataTable = TwoPriceVendorProductPriceRequestRow.GetPriceRequests(DB, Session("VendorId"), "Created", "Asc")
        If res.Rows.Count = 0 Then
            ltlNone.Text = "There are no pending builder price requests"
        Else
            ltlNone.Text = Nothing
        End If
        Keys.Clear()
        rptRequests.DataSource = res.DefaultView
        rptRequests.DataBind()

        'ctlSearch.FilterListCallback = AddressOf FilterListCallback
        'ctlSearch.FilterCacheKey = Session("VendorId")
        Dim list As New Generic.List(Of String)
        FilterListCallback(list)
        ctlSearch.FilterList = list

        ctlSearch.SearchProduct()
    End Sub

    Protected Sub frmSelect_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim form As PopupForm.PopupForm = sender
        Dim item As RepeaterItem = form.NamingContainer
        If item.DataItem Is Nothing Then Exit Sub

        CType(form.FindControl("ltlProductName"), Literal).Text = item.DataItem.ProductName
    End Sub

    Protected Sub frmSelect_Postback(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSelect.Postback
        Dim form As PopupForm.PopupForm = CType(sender, Control).NamingContainer

        Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, hdnRequestId.Value)
        Dim bAlways As Boolean = CType(form.FindControl("rbAppliesAlways"), RadioButton).Checked
        Dim bConfirm As Boolean = False
        If dbRequest.SpecialOrderProductID <> Nothing Then
            DB.BeginTransaction()
            Try
                Dim dbPrice As VendorSpecialOrderProductPriceRow = VendorSpecialOrderProductPriceRow.GetRow(DB, Session("VendorId"), dbRequest.SpecialOrderProductID)

                If dbRequest.TakeoffProductID <> Nothing Then
                    Dim dbSubstitute As VendorTakeOffProductSubstituteRow = VendorTakeOffProductSubstituteRow.GetRow(DB, Session("VendorId"), dbRequest.TakeoffProductID)
                    dbSubstitute.CreatorVendorAccountID = Session("VendorAccountId")
                    dbSubstitute.RecommendedQuantity = txtQuantity.Text
                    dbSubstitute.SubstituteProductID = hdnSubstituteID.Value
                    dbSubstitute.TakeOffProductID = dbRequest.TakeoffProductID
                    dbSubstitute.VendorID = Session("VendorId")

                    If dbSubstitute.Created = Nothing Then
                        dbSubstitute.Insert()
                    Else
                        dbSubstitute.Update()
                    End If

                    dbPrice.IsSubstitution = True
                    dbPrice.SpecialOrderProductID = dbRequest.SpecialOrderProductID
                    dbPrice.SubmitterVendorAccountID = Session("VendorId")
                    dbPrice.VendorID = Session("VendorId")
                    If dbPrice.Submitted = Nothing Then
                        dbPrice.Insert()
                    Else
                        dbPrice.Update()
                    End If

                    dbRequest.Remove()
                    DB.CommitTransaction()
                    bConfirm = True

                Else
                    '**************************************         mGuard#T-1086       ****************************************************
                    DB.BeginTransaction()
                    Try
                        Dim dbTwoPriceBuilderTakeOff As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                        Dim dbTwoPriceSubstitute As TwoPriceBuilderTakeOffProductSubstituterow = TwoPriceBuilderTakeOffProductSubstituterow.GetRow(DB, Session("VendorID"), dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                        ' Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, dbRequest.ProductID)

                        Dim dbSubstitutePrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), hdnSubstituteID.Value)
                        If dbTwoPriceBuilderTakeOff.TwoPriceBuilderTakeOffProductPendingID > 0 Then
                            dbTwoPriceBuilderTakeOff.PriceRequestState = PriceRequestState.SubstitutionAvailable
                            dbTwoPriceBuilderTakeOff.VendorPrice = dbSubstitutePrice.VendorPrice
                            dbTwoPriceBuilderTakeOff.Update()
                        End If

                        dbTwoPriceSubstitute.CreatorVendorAccountID = Session("VendorAccountId")
                        dbTwoPriceSubstitute.RecommendedQuantity = Convert.ToInt32(txtQuantity.Text) * dbTwoPriceBuilderTakeOff.Quantity
                        dbTwoPriceSubstitute.SubstituteProductID = hdnSubstituteID.Value
                        'dbPrice.TakeOffProductID = dbTakeoffProduct.TakeOffProductID
                        dbTwoPriceSubstitute.VendorID = Session("VendorId")
                        If dbTwoPriceSubstitute.Created = Nothing Then
                            dbTwoPriceSubstitute.Insert()
                        Else
                            dbTwoPriceSubstitute.Update()
                        End If


                        dbRequest.Remove()
                        DB.CommitTransaction()
                        bConfirm = True
                    Catch ex As SqlClient.SqlException
                        If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                        AddError("There was an error processing this substitution.  Please try again.")
                    End Try
                    '******************************************************************************************************************************

                End If

            Catch ex As SqlClient.SqlException
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError("There was an error processing this substitution.  Please try again.")
            End Try
        ElseIf bAlways Then
            DB.BeginTransaction()
            Try
                Dim dbPrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), dbRequest.ProductID)
                Dim dbSubstitute As VendorProductSubstituteRow = VendorProductSubstituteRow.GetRow(DB, Session("VendorId"), dbRequest.ProductID)
                Dim dbSubstituteProductPrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), hdnSubstituteID.Value)
                dbPrice.IsSubstitution = True
                dbPrice.IsUpload = False
                dbPrice.ProductID = dbRequest.ProductID
                dbPrice.SubmitterVendorAccountID = Session("VendorAccountId")
                dbPrice.UpdaterVendorAccountID = Session("VendorAccountId")
                dbPrice.VendorID = Session("VendorId")
                If dbPrice.Submitted = Nothing Then
                    dbPrice.Insert()
                Else
                    dbPrice.Update()
                End If

                dbSubstitute.CreatorVendorAccountID = Session("VendorAccountId")
                dbSubstitute.ProductID = dbRequest.ProductID
                dbSubstitute.QuantityMultiplier = CType(form.FindControl("txtQuantity"), TextBox).Text
                dbSubstitute.SubstituteProductID = hdnSubstituteID.Value
                dbSubstitute.VendorID = Session("VendorId")
                If dbSubstitute.Created = Nothing Then
                    dbSubstitute.Insert()
                Else
                    dbSubstitute.Update()
                End If

                dbRequest.Remove()

                Dim dbSubstitutePrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), dbSubstitute.SubstituteProductID)
                Dim dbHistory As New VendorProductPriceHistoryRow(DB)
                dbHistory.IsSubstitution = True
                dbHistory.IsUpload = False
                dbHistory.ProductID = dbPrice.ProductID
                dbHistory.SubmitterVendorAccountID = Session("VendorAccountId")
                dbHistory.VendorID = Session("VendorId")
                dbHistory.VendorSKU = dbSubstitutePrice.VendorSKU
                dbHistory.VendorPrice = dbSubstitutePrice.VendorPrice
                dbHistory.Insert()

                DB.CommitTransaction()
                bConfirm = True
            Catch ex As SqlClient.SqlException
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError("There was an error processing this substitution.  Please try again.")
            End Try
        Else
            'Add substitute -- Only for this request  
            DB.BeginTransaction()
            Try
                Dim dbTwoPriceBuilderTakeOff As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                Dim dbPrice As TwoPriceBuilderTakeOffProductSubstituterow = TwoPriceBuilderTakeOffProductSubstituterow.GetRow(DB, Session("VendorID"), dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                ' Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, dbRequest.ProductID)

                Dim dbSubstitutePrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), hdnSubstituteID.Value)
                If dbTwoPriceBuilderTakeOff.TwoPriceBuilderTakeOffProductPendingID > 0 Then
                    dbTwoPriceBuilderTakeOff.PriceRequestState = PriceRequestState.SubstitutionAvailable
                    dbTwoPriceBuilderTakeOff.VendorPrice = dbSubstitutePrice.VendorPrice
                    dbTwoPriceBuilderTakeOff.Update()
                End If

                dbPrice.CreatorVendorAccountID = Session("VendorAccountId")
                dbPrice.RecommendedQuantity = Convert.ToInt32(txtQuantity.Text) * dbTwoPriceBuilderTakeOff.Quantity
                dbPrice.SubstituteProductID = hdnSubstituteID.Value
                'dbPrice.TakeOffProductID = dbTakeoffProduct.TakeOffProductID
                dbPrice.VendorID = Session("VendorId")
                If dbPrice.Created = Nothing Then
                    dbPrice.Insert()
                Else
                    dbPrice.Update()
                End If


                dbRequest.Remove()
                DB.CommitTransaction()
                bConfirm = True
            Catch ex As SqlClient.SqlException
                If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                AddError("There was an error processing this substitution.  Please try again.")
            End Try
        End If
        BindData()
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "twoPriceCloseForm", "Sys.Application.add_load(twoPriceCloseSubForm);", True)
        upRequests.Update()

        If bConfirm Then
            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PriceUpdate")
            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
            Dim ProductName As String = ""
            If dbRequest.ProductID <> Nothing Then
                Dim dbProduct As ProductRow = ProductRow.GetRow(DB, dbRequest.ProductID)
                ProductName = dbProduct.Product
            ElseIf dbRequest.SpecialOrderProductID <> Nothing Then
                Dim dbProduct As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, dbRequest.SpecialOrderProductID)
                ProductName = dbProduct.SpecialOrderProduct
            End If
            Dim sMsg As String = dbVendor.CompanyName & " updated pricing on the following product: " & ProductName & vbCrLf & vbCrLf & System.Configuration.ConfigurationManager.AppSettings("GlobalRefererName")

            If bAlways Then
                Dim dtRequests As DataTable = VendorProductPriceRequestRow.GetRequestsByProduct(DB, Session("VendorId"), dbRequest.ProductID)
                For Each row As DataRow In dtRequests.Rows
                    dbMsg.Send(BuilderRow.GetRow(DB, row("BuilderID")), sMsg)
                    VendorProductPriceRequestRow.RemoveRow(DB, row("TwoPriceVendorProductPriceRequestID"))
                Next
            Else
                Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbRequest.BuilderID)
                dbMsg.Send(dbBuilder, sMsg)

            End If
        End If
    End Sub

    Protected Sub rptRequests_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptRequests.ItemCommand
        Dim rbFormAppliesAlways As RadioButton = e.Item.FindControl("rbFormAppliesAlways")
        Dim bAllRequest As Boolean = rbFormAppliesAlways.Checked
        Select Case e.CommandName
            Case "Remove"
                DB.BeginTransaction()
                Try
                    Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, e.CommandArgument)
                    Dim dbAutoMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PriceDelete")
                    Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, dbRequest.BuilderID)
                    Dim MsgBody As String = String.Empty
                    MsgBody = dbVendor.CompanyName & " has deleted your Price Request for the product: " & ProductRow.GetName(DB, dbRequest.ProductID) & " because they do not carry this product or a suitable subsitute. " & vbCrLf & vbCrLf & "Please contact them directly for clarification."
                    dbAutoMsg.Send(dbBuilder, MsgBody)
                    dbRequest.Remove()
                    DB.CommitTransaction()
                Catch ex As SqlClient.SqlException
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    Logger.Error(Logger.GetErrorMessage(ex))
                    AddError("An error was encountered while deleting.  Please try again.")
                End Try
            Case "Update"
                EditIndex = e.Item.ItemIndex
            Case "Cancel"
                EditIndex = -1
            Case "Save"

                Try
                    Dim txtPrice As TextBox = e.Item.FindControl("txtPrice")
                    Dim txtSku As TextBox = e.Item.FindControl("txtVendorSku")

                    Dim price As String = Regex.Replace(txtPrice.Text, "[^\d.]", "")
                    If price = String.Empty OrElse Double.Parse(price) <= 0 Then
                        ctlError.AddError("Entered price is invalid.")
                        ctlError.Visible = True
                        Exit Sub
                    End If

                    If Not bAllRequest Then
                        Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, Keys(e.Item.ItemIndex))
                        If dbRequest.ProductID <> Nothing Then
                            Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                            TwoPriceBuilderTakeOffProductPending.VendorPrice = Math.Round(Double.Parse(price), 2)
                            TwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.VendorPriced
                            TwoPriceBuilderTakeOffProductPending.VendorPrice = Math.Round(Double.Parse(price), 2)
                            TwoPriceBuilderTakeOffProductPending.VendorSku = txtSku.Text
                            TwoPriceBuilderTakeOffProductPending.Update()
                            TwoPriceOrderProductRow.AddProduct(DB, TwoPriceBuilderTakeOffProductPending.TwoPriceOrderID, TwoPriceBuilderTakeOffProductPending.ProductID, TwoPriceBuilderTakeOffProductPending.VendorPrice, TwoPriceBuilderTakeOffProductPending.Quantity, False, TwoPriceBuilderTakeOffProductPending.VendorSku, PriceRequestState.VendorPriced)
                            TwoPriceBuilderTakeOffProductPendingRow.RemoveRow(DB, dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PriceUpdate")
                            dbMsg.Send(BuilderRow.GetRow(DB, dbRequest.BuilderID))
                            TwoPriceVendorProductPriceRequestRow.RemoveRow(DB, dbRequest.TwoPriceVendorProductPriceRequestID)
                            ctlError.Visible = False

                        ElseIf dbRequest.SpecialOrderProductID <> Nothing Then
                            Dim TwoPriceBuilderTakeOffProductPending As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                            TwoPriceBuilderTakeOffProductPending.VendorPrice = Math.Round(Double.Parse(price), 2)
                            TwoPriceBuilderTakeOffProductPending.PriceRequestState = PriceRequestState.VendorPriced
                            TwoPriceBuilderTakeOffProductPending.VendorPrice = Math.Round(Double.Parse(price), 2)
                            TwoPriceBuilderTakeOffProductPending.VendorSku = txtSku.Text
                            TwoPriceBuilderTakeOffProductPending.Update()
                            TwoPriceOrderProductRow.AddSpecialOrderProduct(DB, TwoPriceBuilderTakeOffProductPending.TwoPriceOrderID, TwoPriceBuilderTakeOffProductPending.SpecialOrderProductID, TwoPriceBuilderTakeOffProductPending.VendorPrice, TwoPriceBuilderTakeOffProductPending.Quantity, False, TwoPriceBuilderTakeOffProductPending.VendorSku, PriceRequestState.VendorPriced)
                            TwoPriceBuilderTakeOffProductPendingRow.RemoveRow(DB, dbRequest.TwoPriceBuilderTakeOffProductPendingID)
                            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PriceUpdate")
                            dbMsg.Send(BuilderRow.GetRow(DB, dbRequest.BuilderID))
                            TwoPriceVendorProductPriceRequestRow.RemoveRow(DB, dbRequest.TwoPriceVendorProductPriceRequestID)
                            ctlError.Visible = False

                        End If

                    Else
                        Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, Keys(e.Item.ItemIndex))

                        If dbRequest.ProductID <> Nothing Then
                            Dim dbOtherPendingPricingRequest As DataTable = VendorProductPriceRequestRow.GetRequestsByProduct(DB, Session("VendorId"), dbRequest.ProductID)
                            Dim dbPrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, Session("VendorId"), dbRequest.ProductID)
                            dbPrice.IsSubstitution = False
                            dbPrice.IsUpload = False
                            dbPrice.ProductID = dbRequest.ProductID
                            dbPrice.SubmitterVendorAccountID = Session("VendorAccountId")
                            dbPrice.UpdaterVendorAccountID = Session("VendorAccountId")
                            dbPrice.VendorID = Session("VendorId")
                            dbPrice.VendorPrice = Math.Round(Double.Parse(price), 2)
                            dbPrice.VendorSKU = txtSku.Text
                            If dbPrice.Submitted = Nothing Then
                                dbPrice.Insert()
                            Else
                                dbPrice.Update()
                            End If
                            dbRequest.Remove()

                            Dim dbHistory As New VendorProductPriceHistoryRow(DB)
                            dbHistory.IsSubstitution = False
                            dbHistory.IsUpload = False
                            dbHistory.ProductID = dbPrice.ProductID
                            dbHistory.SubmitterVendorAccountID = Session("VendorAccountId")
                            dbHistory.VendorID = Session("VendorId")
                            dbHistory.VendorPrice = dbPrice.VendorPrice
                            dbHistory.VendorSKU = dbPrice.VendorSKU
                            dbHistory.Insert()

                            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PriceUpdate")
                            Dim dtRequests As DataTable = VendorProductPriceRequestRow.GetRequestsByProduct(DB, Session("VendorId"), dbRequest.ProductID)
                            For Each row As DataRow In dtRequests.Rows
                                dbMsg.Send(BuilderRow.GetRow(DB, row("BuilderID")))
                                VendorProductPriceRequestRow.RemoveRow(DB, row("VendorProductPriceRequestID"))
                            Next

                            Dim dttwopricePricingRequests As DataTable = TwoPriceVendorProductPriceRequestRow.GetTwoPriceRequestsByProduct(DB, Session("VendorID"), dbRequest.ProductID)
                            If dbRequest.ProductID <> Nothing Then
                                'Update regular pricing table for vendor ' RefreshNotPricedProducts function in twoprice/edit.aspx.vb will refresh items there.
                                ' AddProduct(TwoPriceBuilderTakeOffProductPending.ProductID, TwoPriceBuilderTakeOffProductPending.TwoPriceOrderID, TwoPriceBuilderTakeOffProductPending.VendorPrice, TwoPriceBuilderTakeOffProductPending.Quantity, False, TwoPriceBuilderTakeOffProductPending.VendorSku, PriceRequestState.VendorPriced)

                                For Each row As DataRow In dttwopricePricingRequests.Rows
                                    dbMsg.Send(BuilderRow.GetRow(DB, row("BuilderID")))
                                    TwoPriceVendorProductPriceRequestRow.RemoveRow(DB, row("TwoPriceVendorProductPriceRequestID"))
                                Next
                            End If

                            ctlError.Visible = False

                            '**************** Following (else) part added by Apala (Medullus) for mGuard#T-1086
                        ElseIf dbRequest.SpecialOrderProductID <> Nothing Then
                            Dim dbOtherPendingPricingRequest As DataTable = VendorProductPriceRequestRow.GetRequestsBySpecialOrderProduct(DB, Session("VendorId"), dbRequest.SpecialOrderProductID)
                            Dim dbPrice As VendorSpecialOrderProductPriceRow = VendorSpecialOrderProductPriceRow.GetRow(DB, Session("VendorId"), dbRequest.SpecialOrderProductID)
                            dbPrice.IsSubstitution = False
                            dbPrice.SpecialOrderProductID = dbRequest.SpecialOrderProductID
                            dbPrice.SubmitterVendorAccountID = Session("VendorAccountId")
                            dbPrice.VendorID = Session("VendorId")
                            dbPrice.VendorPrice = Math.Round(Double.Parse(price), 2)
                            dbPrice.VendorSKU = txtSku.Text
                            If dbPrice.Submitted = Nothing Then
                                dbPrice.Insert()
                            Else
                                dbPrice.Update()
                            End If
                            dbRequest.Remove()

                            Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "PriceUpdate")
                            Dim dtRequests As DataTable = VendorProductPriceRequestRow.GetRequestsBySpecialOrderProduct(DB, Session("VendorId"), dbRequest.SpecialOrderProductID)
                            For Each row As DataRow In dtRequests.Rows
                                dbMsg.Send(BuilderRow.GetRow(DB, row("BuilderID")))
                                VendorProductPriceRequestRow.RemoveRow(DB, row("VendorProductPriceRequestID"))
                            Next

                            Dim dttwopricePricingRequests As DataTable = TwoPriceVendorProductPriceRequestRow.GetTwoPriceRequestsBySpecialOrderProduct(DB, Session("VendorID"), dbRequest.SpecialOrderProductID)
                            If dbRequest.SpecialOrderProductID <> Nothing Then
                                'Update regular pricing table for vendor ' RefreshNotPricedProducts function in twoprice/edit.aspx.vb will refresh items there.
                                ' AddProduct(TwoPriceBuilderTakeOffProductPending.ProductID, TwoPriceBuilderTakeOffProductPending.TwoPriceOrderID, TwoPriceBuilderTakeOffProductPending.VendorPrice, TwoPriceBuilderTakeOffProductPending.Quantity, False, TwoPriceBuilderTakeOffProductPending.VendorSku, PriceRequestState.VendorPriced)

                                For Each row As DataRow In dttwopricePricingRequests.Rows
                                    dbMsg.Send(BuilderRow.GetRow(DB, row("BuilderID")))
                                    TwoPriceVendorProductPriceRequestRow.RemoveRow(DB, row("TwoPriceVendorProductPriceRequestID"))
                                Next
                            End If

                            ctlError.Visible = False
                        End If
                    End If
                Catch ex As SqlClient.SqlException
                    If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
                    Logger.Error(Logger.GetErrorMessage(ex))
                    AddError("An error was encountered while saving.  Please try again.")
                End Try
                EditIndex = -1
        End Select
        BindData()
    End Sub

    Private Sub SendMessage(ByVal req As VendorProductPriceRequestRow)
        'auto message here
    End Sub
    'Private Sub AddProduct(ProductId As Integer, TwoPriceOrderID As Integer, Price As Double, Quantity As Integer, Optional ByVal AddToQuantity As Boolean = True, Optional VendorSku As String = Nothing, Optional PriceState As Integer = 0)
    '    Dim TwoPriceOrderProduct As TwoPriceOrderProductRow
    '    'Per ticket 205529 - customer doesnot want merge products - insert the same way as in takeoff

    '    'Product is not part of this order, add it.
    '    TwoPriceOrderProduct = New TwoPriceOrderProductRow(DB)
    '    TwoPriceOrderProduct.VendorPrice = Price
    '    TwoPriceOrderProduct.Quantity = Quantity
    '    TwoPriceOrderProduct.ProductID = ProductId
    '    TwoPriceOrderProduct.TwoPriceOrderID = TwoPriceOrderID
    '    TwoPriceOrderProduct.PriceRequestState = PriceState
    '    TwoPriceOrderProduct.VendorSku = VendorSku

    '    TwoPriceOrderProduct.TwoPriceOrderProductID = TwoPriceOrderProduct.Insert()

    'End Sub
    Protected Sub rptResults_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptResults.ItemCreated
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        'Dim form As PopupForm.PopupForm = e.Item.FindControl("frmSelect")
        'AddHandler form.TemplateLoaded, AddressOf frmSelect_TemplateLoaded
        'AddHandler form.Postback, AddressOf frmSelect_Postback
        Dim btnSelect As Button = e.Item.FindControl("btnSelect")
        AddHandler btnSelect.Click, AddressOf btnSelect_Click
        ScriptManager.GetCurrent(Page).RegisterAsyncPostBackControl(btnSelect)
    End Sub

    Protected Sub btnSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ProductID As Integer = CType(sender, Button).CommandArgument
        Dim dbRequest As TwoPriceVendorProductPriceRequestRow = TwoPriceVendorProductPriceRequestRow.GetRow(DB, hdnRequestId.Value)
        Dim dbTwoPricePendingProduct As TwoPriceBuilderTakeOffProductPendingRow = TwoPriceBuilderTakeOffProductPendingRow.GetRow(DB, dbRequest.TwoPriceBuilderTakeOffProductPendingID)
        Dim dbSubstitute As ProductRow = ProductRow.GetRow(DB, ProductID)

        If dbTwoPricePendingProduct.SpecialOrderProductID <> Nothing Then
            Dim dbRequested As SpecialOrderProductRow = SpecialOrderProductRow.GetRow(DB, dbTwoPricePendingProduct.SpecialOrderProductID)
            ltlProductName.Text = dbSubstitute.Product
            ltlRequestedProduct.Text = dbRequested.SpecialOrderProduct
            spanQuantity.InnerHtml = dbTwoPricePendingProduct.Quantity
            txtQuantity.Text = dbTwoPricePendingProduct.Quantity
            trApplies.Visible = False
            trRecommendedQty.Visible = False
            hdnSubstituteID.Value = dbSubstitute.ProductID
        Else
            Dim dbRequested As ProductRow = ProductRow.GetRow(DB, dbRequest.ProductID)
            ltlProductName.Text = dbSubstitute.Product
            ltlRequestedProduct.Text = dbRequested.Product
            spanQuantity.InnerHtml = dbTwoPricePendingProduct.Quantity
            txtQuantity.Text = 1
            txtQuantity.Attributes.Add("onchange", "UpdateTotal(event);")
            spanRecommendedQty.InnerHtml = spanQuantity.InnerHtml
            trRecommendedQty.Visible = True
            trApplies.Visible = True
            hdnSubstituteID.Value = dbSubstitute.ProductID
        End If
        ScriptManager.RegisterStartupScript(Page, Me.GetType, "twoPriceOpenSelectForm", "Sys.Application.add_load(twoPriceSwapForms)", True)
        upSelect.Update()
    End Sub

    Protected Sub rptRequests_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptRequests.ItemDataBound
        If e.Item.ItemType <> ListItemType.AlternatingItem And e.Item.ItemType <> ListItemType.Item Then
            Exit Sub
        End If

        Keys.Add(e.Item.DataItem("TwoPriceVendorProductPriceRequestId"))

        Dim ProductType As String = e.Item.DataItem("ProductType")

        Dim btnUpdate As Button = e.Item.FindControl("btnUpdate")
        Dim btnSubsitute As Button = e.Item.FindControl("btnSubstitute")
        Dim trInfo As HtmlTableRow = e.Item.FindControl("trInfo")
        Dim trForm As HtmlTableRow = e.Item.FindControl("trForm")
        Dim trFormApplies As HtmlTableRow = e.Item.FindControl("trFormApplies")
        If e.Item.ItemIndex = EditIndex Then
            trForm.Visible = True
            btnUpdate.Visible = False
            trFormApplies.Visible = True
            btnSubsitute.Visible = False
        Else
            trForm.Visible = False
            btnUpdate.Visible = True
            trFormApplies.Visible = False
            btnSubsitute.Visible = True
        End If

        Dim rbFormAppliesAlways As RadioButton = e.Item.FindControl("rbFormAppliesAlways")
        Dim rbFormAppliesOnce As RadioButton = e.Item.FindControl("rbFormAppliesOnce")

        If ProductType = "Special" Then
            rbFormAppliesAlways.Checked = False
            rbFormAppliesOnce.Checked = True
            rbFormAppliesAlways.Enabled = False
        Else
            rbFormAppliesAlways.Checked = True
            rbFormAppliesOnce.Checked = False
            rbFormAppliesAlways.Enabled = True
        End If

    End Sub

    Private m_VendorProducts As DataView
    Private ReadOnly Property VendorProducts() As DataView
        Get
            If m_VendorProducts Is Nothing Then
                m_VendorProducts = VendorProductPriceRow.GetAllVendorPrices(DB, Session("VendorId")).DefaultView
                m_VendorProducts.Sort = "ProductId"
            End If
            Return m_VendorProducts
        End Get
    End Property

    Private Function GetValue(ByVal drv As DataRowView, ByVal field As String) As Object
        If drv Is Nothing Then
            Return Nothing
        Else
            Return drv(field)
        End If
    End Function

    Private Sub BindSearchData(ByVal tbl As DataTable, ByVal count As Integer)
        'If rblPricedOnly.SelectedValue = True Then
        '    Dim q = From sr As DataRow In tbl.AsEnumerable
        '            Join vp As DataRowView In VendorProducts
        '            On Convert.ToInt32(sr("ProductId")) Equals Convert.ToInt32(vp("ProductId"))
        '            Select New With {
        '                .ProductID = sr("ProductId"),
        '                .VendorSku = vp("VendorSku"),
        '                .ProductSku = sr("SKU"),
        '                .Product = sr("Product"),
        '                .Description = sr("Description"),
        '                .UnitOfMeasure = sr("SizeUnitOfMeasureText")
        '            }
        '    rptResults.DataSource = q
        '    ctlNavigator.NofRecords = count
        'Else
        '    Dim q = From sr As DataRow In tbl.AsEnumerable Group Join vp As DataRowView In VendorProducts On Convert.ToInt32(sr("ProductId")) Equals Convert.ToInt32(vp("ProductId")) Into grp = Group
        '            From vp As DataRowView In grp.DefaultIfEmpty Select New With {.ProductID = sr("ProductId"), .VendorSku = GetValue(vp, "VendorSku"), .ProductSku = sr("SKU"), .ProductName = sr("ProductName"), .Description = sr("Description"), .UnitOfMeasure = sr("SizeUnitOfMeasureText")}
        '    rptResults.DataSource = q
        'End If

        Dim q = From sr As DataRow In tbl.AsEnumerable Group Join vp As DataRowView In VendorProducts On Convert.ToInt32(sr("ProductId")) Equals Convert.ToInt32(vp("ProductId")) Into grp = Group
                From vp As DataRowView In grp.DefaultIfEmpty Select New With {.ProductID = sr("ProductId"), .VendorSku = GetValue(vp, "VendorSku"), .ProductSku = sr("SKU"), .Product = sr("Product")}

        rptResults.DataSource = q
        rptResults.DataBind()

        ctlNavigator.NofRecords = count
        ctlNavigator.PageNumber = IIf(ctlSearch.PageNumber > 0, ctlSearch.PageNumber, 1)
        ctlNavigator.DataBind()

    End Sub

    Protected Sub ctlSearch_OnTreeNodeSelect(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.OnTreeNodeSelect
        If rblPricedOnly.SelectedValue = True Then
            Dim list As New Generic.List(Of String)
            FilterListCallback(list)
            ctlSearch.FilterList = list
        End If
    End Sub

    Protected Sub ctlSearch_ResultsUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlSearch.ResultsUpdated
        BindSearchData(ctlSearch.SearchResults, ctlSearch.SearchResults.Rows(0).Item(3))
        ltlBreadcrumbs.Text = ctlSearch.Breadcrumbs
        spanSubHeaderProduct.InnerHtml = hdnSubstituteProductName.Value
        upResults.Update()
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        ctlSearch.SearchProduct(0, False, False)
    End Sub

    Protected Sub FilterListCallback(ByRef list As Generic.List(Of String))
        list.AddRange(From vp As DataRowView In VendorProducts Where Not IsDBNull(vp("VendorPrice")) And Not Core.GetBoolean(vp("IsSubstitution")) Select Convert.ToString(vp.Item("ProductId")))
    End Sub

    Protected Sub rblPricedOnly_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblPricedOnly.SelectedIndexChanged
        If rblPricedOnly.SelectedValue = False Then
            'ctlSearch.FilterListCallback = Nothing
            ctlSearch.FilterList = Nothing
        Else
            'ctlSearch.FilterListCallback = AddressOf FilterListCallback
            Dim list As New Generic.List(Of String)
            FilterListCallback(list)
            ctlSearch.FilterList = list
        End If
        ctlSearch.SearchProduct(0, False, False)
    End Sub

    Protected Sub ctlNavigator_NavigatorEvent(ByVal sender As Object, ByVal e As Controls.NavigatorEventArgs) Handles ctlNavigator.NavigatorEvent
        ctlNavigator.PageNumber = e.PageNumber
        ctlSearch.PageNumber = e.PageNumber
        ctlSearch.SearchProduct()
    End Sub

    'Protected Sub frmSelect_Postback1(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim ProductID As Integer = hdnSubstituteID.Value
    '    Dim dbRequest As VendorProductPriceRequestRow = VendorProductPriceRequestRow.GetRow(DB, hdnRequestId.Value)
    '    Dim dbTakeoffProduct As TakeOffProductRow = TakeOffProductRow.GetRow(DB, dbRequest.TakeoffProductID)
    '    Dim dbSubstitute As ProductRow = ProductRow.GetRow(DB, ProductID)
    '    Dim dbRequestPrice As VendorProductPriceRow = VendorProductPriceRow.GetRow(DB, dbRequest.VendorID, dbRequest.ProductID)
    '    Dim delete As Boolean = False

    '    If dbRequest.SpecialOrderProductID <> Nothing Then
    '        Dim dbPrice As VendorSpecialOrderProductPriceRow = VendorSpecialOrderProductPriceRow.GetRow(DB, Session("VendorId"), dbRequest.SpecialOrderProductID)
    '        dbPrice.IsSubstitution = True
    '        dbPrice.SpecialOrderProductID = dbRequest.SpecialOrderProductID
    '        dbPrice.SubmitterVendorAccountID = Session("VendorAccountId")
    '        dbPrice.VendorID = Session("VendorId")
    '        dbPrice.VendorPrice = Nothing
    '        dbPrice.VendorSKU = Nothing
    '        If dbPrice.Submitted = Nothing Then
    '            dbPrice.SubmitterVendorAccountID = Session("VendorAccountId")
    '            dbPrice.Insert()
    '        Else
    '            dbPrice.Update()
    '        End If
    '        delete = True
    '    Else
    '        If rbAppliesOnce.Checked Then
    '            Dim dbPrice As VendorTakeOffProductSubstituteRow = VendorTakeOffProductSubstituteRow.GetRow(DB, Session("VendorId"), dbTakeoffProduct.TakeOffProductID)
    '            dbPrice.RecommendedQuantity = spanRecommendedQty.InnerHtml
    '            dbPrice.SubstituteProductID = dbSubstitute.ProductID
    '            dbPrice.TakeOffProductID = dbTakeoffProduct.TakeOffProductID
    '            dbPrice.VendorID = Session("VendorId")
    '            If dbPrice.Created = Nothing Then
    '                dbPrice.CreatorVendorAccountID = Session("VendorAccountId")
    '                dbPrice.Insert()
    '            Else
    '                dbPrice.Update()
    '            End If
    '            delete = True
    '        Else
    '            Dim dbPrice As VendorProductSubstituteRow = VendorProductSubstituteRow.GetRow(DB, Session("VendorId"), dbRequest.ProductID)
    '            dbPrice.ProductID = dbRequest.ProductID
    '            dbPrice.QuantityMultiplier = txtQuantity.Text
    '            dbPrice.SubstituteProductID = dbSubstitute.ProductID
    '            dbPrice.VendorID = Session("VendorId")
    '            If dbPrice.Created = Nothing Then
    '                dbPrice.CreatorVendorAccountID = Session("VendorAccountId")
    '                dbPrice.Insert()
    '            Else
    '                dbPrice.Update()
    '            End If
    '            delete = True
    '        End If

    '        dbRequestPrice.IsSubstitution = True
    '        dbRequestPrice.ProductID = dbRequest.ProductID
    '        dbRequestPrice.SubstituteQuantityMultiplier = txtQuantity.Text
    '        dbRequestPrice.VendorID = dbRequest.VendorID
    '        dbRequestPrice.VendorPrice = Nothing
    '        dbRequestPrice.VendorSKU = Nothing
    '        dbRequestPrice.UpdaterVendorAccountID = Session("VendorAccountId")
    '        If dbRequestPrice.Submitted <> Nothing Then
    '            dbRequestPrice.Update()
    '        Else
    '            dbRequestPrice.SubmitterVendorAccountID = Session("VendorAccountId")
    '            dbRequestPrice.Insert()
    '        End If
    '    End If
    '    If delete Then
    '        dbRequest.Remove()
    '    End If
    '    ScriptManager.RegisterStartupScript(Page, Me.GetType, "CloseSelect", "Sys.Application.add_load(CloseSelectForm)", True)
    '    BindData()
    '    upRequests.Update()
    'End Sub

End Class
