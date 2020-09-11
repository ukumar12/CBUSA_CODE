Imports Components
Imports DataLayer
Imports Controls
Imports System.Linq
Imports System.Data.SqlClient
Imports TwoPrice.DataLayer
Imports System.Web.Services
Imports Utility

Partial Class order_default
    Inherits SitePage
    Protected TakeOffName As String = String.Empty
    Protected OrderId As Integer
    Protected Property PriceComparisonId() As Integer
        Get
            Return ViewState("PriceComparisonId")
        End Get
        Set(ByVal value As Integer)
            ViewState("PriceComparisonId") = value
        End Set
    End Property

    Protected Property VendorId() As Integer
        Get
            Return ViewState("VendorId")
        End Get
        Set(ByVal value As Integer)
            ViewState("VendorId") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureBuilderAccess()

        OrderId = Request("OrderID")

        If Not IsPostBack Then
            PriceComparisonId = Request("PriceComparisonId")
            VendorId = Request("VendorId")
            If OrderId = Nothing And (PriceComparisonId = Nothing Or VendorId = Nothing) Then
                Response.Redirect("/comparison/default.aspx")
            End If

            Dim dbTakeOff As TakeOffRow = TakeOffRow.GetRow(DB, PriceComparisonRow.GetRow(DB, PriceComparisonId).TakeoffID)
            TakeOffName = dbTakeOff.Title
            If dbTakeOff.ProjectID <> Nothing Then
                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbTakeOff.ProjectID)
                acProject.Text = dbProject.ProjectName
                acProject.Value = dbProject.ProjectID
                Dim tax As Double = 0

                Dim taxRate As Double = GetTaxRate(DB, dbProject.Zip)
                txtTaxRate.Text = Math.Round(100 * taxRate, 4)
            End If

            LoadFromDB()

            acVendorAccount.WhereClause = "VendorID=" & DB.Number(VendorId)
            acProject.WhereClause = "BuilderID=" & DB.Number(Session("BuilderId"))
        End If
    End Sub

    Protected Sub frmProject_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmProject.TemplateLoaded
        If Not IsPostBack Then
            BindProjectForm()
        End If
    End Sub

    Private Sub LoadFromDB()
        Dim dbOrder As Object

        If Request("twoprice") = "y" Then
            dbOrder = TwoPriceOrderRow.GetRow(DB, OrderId)
        Else
            dbOrder = OrderRow.GetRow(DB, OrderId)
        End If

        If dbOrder.OrderID = Nothing Then
            If Not Session("BuilderAccountId") Is Nothing Then
                Dim BuilderAccount As BuilderAccountRow = BuilderAccountRow.GetRow(DB, Session("BuilderAccountId"))
                txtOrdererEmail.Text = BuilderAccount.Email
                txtOrdererFirstName.Text = BuilderAccount.FirstName
                txtOrdererLastName.Text = BuilderAccount.LastName
                txtOrdererPhone.Text = BuilderAccount.Phone
                txtOrderTitle.Text = TakeOffName
            End If
            Exit Sub
        End If

        VendorId = dbOrder.VendorID
        PriceComparisonId = Nothing

        With dbOrder
            txtDeliveryInstructions.Text = .DeliveryInstructions
            txtNotes.Text = .Notes
            txtOrdererEmail.Text = .OrdererEmail
            txtOrdererFirstName.Text = .OrdererFirstName
            txtOrdererLastName.Text = .OrdererLastName
            txtOrdererPhone.Text = .OrdererPhone
            txtOrderTitle.Text = .Title
            txtPONumber.Text = .PONumber
            txtSuperEmail.Text = .SuperEmail
            txtSuperFirstName.Text = .SuperFirstName
            txtSuperLastName.Text = .SuperLastName
            txtSuperPhone.Text = .SuperPhone
            txtTaxRate.Text = .TaxRate

            If .SalesRepVendorAccountID <> Nothing Then
                Dim dbVendorAccount As VendorAccountRow = VendorAccountRow.GetRow(DB, .SalesRepVendorAccountID)
                acVendorAccount.Text = dbVendorAccount.LastName & ", " & dbVendorAccount.FirstName
                acVendorAccount.Value = dbVendorAccount.VendorAccountID
            End If
            If .ProjectID <> Nothing Then
                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, .ProjectID)
                acProject.Text = dbProject.ProjectName
                acProject.Value = dbProject.ProjectID

            End If
        End With

        btnSubmit.Text = "Edit Drops"

        sfDrops.Visible = False
        trRbDrops.Visible = False
    End Sub

    Private Sub BindProjectForm()
        drpProjectState.DataSource = StateRow.GetStateList(DB)
        drpProjectState.DataTextField = "StateName"
        drpProjectState.DataValueField = "StateCode"
        drpProjectState.DataBind()

        drpProjectPortfolio.DataSource = PortfolioRow.GetList(DB, "Portfolio")
        drpProjectPortfolio.DataTextField = "Portfolio"
        drpProjectPortfolio.DataValueField = "PortfolioID"
        drpProjectPortfolio.DataBind()
        drpProjectPortfolio.Items.Insert(0, New ListItem("", ""))

        drpProjectStatus.DataSource = ProjectStatusRow.GetList(DB, "SortOrder")
        drpProjectStatus.DataTextField = "ProjectStatus"
        drpProjectStatus.DataValueField = "ProjectStatusID"
        drpProjectStatus.DataBind()
    End Sub

    'Protected Sub acBuilder_ValueUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles acBuilder.ValueUpdated
    '    If acBuilder.Value <> String.Empty Then
    '        acProject.WhereClause = " BuilderId=" & DB.Number(acBuilder.Value)
    '        acProject.Visible = True
    '        spanNoBuilder.Visible = False
    '    Else
    '        acProject.Visible = False
    '        spanNoBuilder.Visible = True
    '    End If
    'End Sub

    Protected Sub sfDrops_ValidateRow(ByVal sender As Object, ByVal args As Controls.SubForm.SubFormEventArguments) Handles sfDrops.ValidateRow
        args.IsValid = True
        If args.DataRow("txtDropName") = String.Empty Then
            args.IsValid = False
        End If
        If args.DataRow("dpRequestedDelivery") = String.Empty Then
            AddError("Delivery date is invalid.")
            args.IsValid = False
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not IsValid Then Exit Sub
        Try
            DB.BeginTransaction()

            Dim dbOrder As OrderRow = OrderRow.GetRow(DB, OrderId)
            dbOrder.VendorID = VendorId
            dbOrder.BuilderID = Session("BuilderId")
            dbOrder.CreatorBuilderID = Session("BuilderAccountId")
            dbOrder.DeliveryInstructions = txtDeliveryInstructions.Text
            dbOrder.Notes = txtNotes.Text
            dbOrder.OrdererEmail = txtOrdererEmail.Text
            dbOrder.OrdererFirstName = txtOrdererFirstName.Text
            dbOrder.OrdererLastName = txtOrdererLastName.Text
            dbOrder.OrdererPhone = txtOrdererPhone.Text
            dbOrder.OrderNumber = OrderRow.GetOrderNumber(DB)
            dbOrder.OrderStatusID = OrderStatusRow.GetDefaultStatusId(DB)
            dbOrder.PONumber = txtPONumber.Text
            If acProject.Value <> Nothing Then
                dbOrder.ProjectID = acProject.Value

            End If
            dbOrder.RemoteIP = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
            If rbDropNo.Checked Then
                dbOrder.RequestedDelivery = dpRequestedDelivery.Value
            End If
            dbOrder.SuperEmail = txtSuperEmail.Text
            dbOrder.SuperFirstName = txtSuperFirstName.Text
            dbOrder.SuperLastName = txtSuperLastName.Text
            dbOrder.SuperPhone = txtSuperPhone.Text
            dbOrder.Title = txtOrderTitle.Text
            dbOrder.SalesRepVendorAccountID = acVendorAccount.Value
            dbOrder.TaxRate = txtTaxRate.Text
            dbOrder.Created = Now
            If dbOrder.OrderID <> Nothing Then
                Dim UpdatedTax As Double = dbOrder.Subtotal * dbOrder.TaxRate / 100
                dbOrder.Tax = UpdatedTax
                dbOrder.Total = dbOrder.Subtotal + UpdatedTax
                dbOrder.Update()
            Else
                dbOrder.Insert()

                Dim aDrops As DataRow() = sfDrops.GetData()
                For Each row As DataRow In aDrops
                    If Not sfDrops.InvalidRows.Contains(row("RowIndex")) Then
                        Dim dbDrop As New OrderDropRow(DB)
                        dbDrop.OrderID = dbOrder.OrderID
                        dbDrop.CreatorBuilderID = Session("BuilderAccountID")
                        dbDrop.DropName = row("txtDropName")
                        If row("dpRequestedDelivery") <> Nothing Then dbDrop.RequestedDelivery = row("dpRequestedDelivery")
                        dbDrop.Insert()
                    End If
                Next

                Dim dbProject As ProjectRow = ProjectRow.GetRow(DB, dbOrder.ProjectID)

                Dim subTotal As Double = 0
                Dim total As Double = 0
                Dim tax As Double = 0
                'Dim taxRate As Double = GetTaxRate(dbProject.City, dbProject.County, dbProject.State)
                Dim taxRate As Double = txtTaxRate.Text / 100


                Dim dtItems As DataTable = PriceComparisonRow.GetVendorProducts(DB, PriceComparisonId, VendorId, True)
                For Each row As DataRow In dtItems.Rows
                    If Not IsDBNull(row("State")) Then
                        Dim dbProduct As New OrderProductRow(DB)
                        dbProduct.OrderID = dbOrder.OrderID
                        If Not IsDBNull(row("SubstituteProductID")) Then
                            dbProduct.ProductID = row("SubstituteProductID")
                        ElseIf Not IsDBNull(row("ProductID")) Then
                            dbProduct.ProductID = row("ProductID")
                        End If
                        If Not IsDBNull(row("SpecialOrderProductId")) Then
                            dbProduct.SpecialOrderProductID = row("SpecialOrderProductId")
                        End If
                        dbProduct.Quantity = row("Quantity")
                        dbProduct.VendorSku = Core.GetString(row("VendorSku"))
                        dbProduct.VendorPrice = Core.GetDouble(row("UnitPrice"))
                        dbProduct.Insert()

                        Dim lineSub = dbProduct.Quantity * dbProduct.VendorPrice
                        subTotal += lineSub
                        tax += taxRate * lineSub
                    End If

                Next
                dbOrder.Total = subTotal + tax
                dbOrder.Subtotal = subTotal
                dbOrder.Tax = tax

                Dim dbComparison As PriceComparisonRow = PriceComparisonRow.GetRow(DB, PriceComparisonId)
                dbOrder.TakeoffID = dbComparison.TakeoffID

                dbOrder.Update()

                'dbComparison.Remove()

                Session("PriceComparisonId") = Nothing

            End If

            DB.CommitTransaction()

            Response.Redirect("drops.aspx?OrderID=" & dbOrder.OrderID)
        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    <Web.Services.WebMethod(EnableSession:=True)> _
    Public Shared Function GetTaxRate(ByVal Zip As String) As Double
        Return Math.Round(100 * GetTaxRate(Utility.GlobalDB.DB, Zip), 4)

    End Function
    <WebMethod()> _
    Public Shared Function GetTaxRateFromProjectZip(ByVal ProjectID As String) As Double
        If ProjectID <> String.Empty Then
            Dim Project As ProjectRow = ProjectRow.GetRow(GlobalDB.DB, ProjectID)

            Return Math.Round(100 * GetTaxRate(Utility.GlobalDB.DB, Project.Zip), 4)
        Else
            Return String.Empty

        End If
         End Function

    Public Shared Function GetTaxRate(ByVal DB As Database, ByVal Zip As String) As Double
        Dim f As New FastTax.DOTSFastTax()
        Dim info As FastTax.TaxInfo = f.GetTaxInfo(Zip, "sales", SysParam.GetValue(DB, "FastTaxLicenseKey"))
        Return info.TotalTaxRate
    End Function

    Private Function GetTaxRate(ByVal City As String, ByVal County As String, ByVal State As String) As Double
        Dim f As New FastTax.DOTSFastTax()
        Dim info As FastTax.TaxInfo = f.GetTaxInfoByCityCountyState(City, County, State, "total", SysParam.GetValue(DB, "FastTaxLicenseKey"))
        Return info.TotalTaxRate
    End Function

    Protected Sub frmProject_Callback(ByVal sender As Object, ByVal args As PopupForm.PopupFormEventArgs) Handles frmProject.Callback
        If Not frmProject.IsValid Then
            Exit Sub
        End If

        Dim dbProject As New ProjectRow(DB)
        dbProject.Address = txtProjectAddress1.Text
        dbProject.Address2 = txtProjectAddress2.Text
        dbProject.BuilderID = Session("BuilderId")
        dbProject.City = txtProjectCity.Text
        dbProject.IsArchived = rblProjectArchive.SelectedValue
        dbProject.LotNumber = txtProjectLotNo.Text
        If drpProjectPortfolio.SelectedValue <> Nothing Then
            dbProject.PortfolioID = drpProjectPortfolio.SelectedValue
        End If
        dbProject.ProjectName = txtProjectName.Text
        dbProject.ProjectStatusID = drpProjectStatus.SelectedValue
        dbProject.StartDate = dpProjectStartDate.Value
        dbProject.State = drpProjectState.SelectedValue
        dbProject.Subdivision = txtProjectSubdivision.Text
        dbProject.Zip = txtProjectZip.Text
        dbProject.Insert()

        acProject.WhereClause = "BuilderID=" & DB.Number(Session("BuilderId"))

    End Sub





End Class
