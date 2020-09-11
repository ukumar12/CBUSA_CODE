Imports Components
Imports DataLayer

Partial Class admin_eagleone_edit
    Inherits AdminPage

    Protected PriceComparisonID As Integer
    Protected dbPriceComparison As PriceComparisonRow

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("EAGLE_ONE")
        PriceComparisonID = Request("PriceComparisonID")

        If PriceComparisonID = Nothing Then
            dbPriceComparison = New PriceComparisonRow(DB)
        Else
            dbPriceComparison = PriceComparisonRow.GetRow(DB, PriceComparisonID)
        End If

        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private Sub BindVendors()
        If slTakeoff.Value <> Nothing And slLLC.Value <> Nothing Then
            lsVendors.DataSource = VendorRow.GetListByLLC(DB, slLLC.Value, "CompanyName")
            lsVendors.DataTextField = "CompanyName"
            lsVendors.DataValueField = "VendorID"
            lsVendors.DataBind()
            lsVendors.Visible = True
            ltlSelect.Visible = False
        Else
            lsVendors.Visible = False
            ltlSelect.Visible = True
        End If
    End Sub

    Private Sub LoadFromDB()
        If dbPriceComparison.TakeoffID <> Nothing Then
            Dim dbTakeoff As TakeOffRow = TakeOffRow.GetRow(DB, dbPriceComparison.TakeoffID)
            slTakeoff.Text = dbTakeoff.Title
            slTakeoff.Value = dbTakeoff.TakeOffID
        End If
        If dbPriceComparison.BuilderID <> Nothing Then
            Dim dbLLC As LLCRow = LLCRow.GetBuilderLLC(DB, dbPriceComparison.BuilderID)
            slLLC.Text = dbLLC.LLC
            slLLC.Value = dbLLC.LLCID
        End If

        BindVendors()

        If dbPriceComparison.Created = Nothing Then
            Exit Sub
        End If

        lsVendors.SelectedValues = PriceComparisonRow.GetSavedVendors(DB, PriceComparisonID)
    End Sub

    Protected Sub slLLC_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles slLLC.ValueChanged
        BindVendors()
    End Sub

    Protected Sub slTakeoff_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles slTakeoff.ValueChanged
        BindVendors()
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then
            Exit Sub
        End If

        DB.BeginTransaction()
        Try

            Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, slLLC.Value)
            Dim builderId As Integer = Nothing
            If dtBuilders.Rows.Count > 0 Then
                builderId = Core.GetInt(dtBuilders.Rows(0)("BuilderId"))
            End If

            dbPriceComparison.AdminID = LoggedInAdminId
            dbPriceComparison.IsAdminComparison = True
            dbPriceComparison.BuilderID = builderId
            dbPriceComparison.IsDashboard = True
            dbPriceComparison.TakeoffID = slTakeoff.Value
            If dbPriceComparison.Created = Nothing Then
                dbPriceComparison.Insert()
            Else
                dbPriceComparison.Update()
            End If

            Dim dtProducts As DataTable = TakeOffRow.GetTakeoffProductAverages(DB, dbPriceComparison.TakeoffID, slLLC.Value)

            For Each id As String In lsVendors.SelectedValues.Split(",")
                Dim dtPrices As DataTable = TakeOffProductRow.GetTakeoffVendorPricing(DB, dbPriceComparison.TakeoffID, id)

                PriceComparisonRow.UpdateVendor(DB, dbPriceComparison.PriceComparisonID, id, dtProducts, dtPrices)
            Next

            DB.CommitTransaction()
            Response.Redirect("default.aspx")
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
