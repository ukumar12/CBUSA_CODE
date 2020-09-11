Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_supplyphase
    Inherits SitePage

    Private CurrentUserId As String = ""
    Private UserName As String = ""
    Private PageURL As String = ""
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("VendorId") Is Nothing Or Session("VendorAccountId") Is Nothing Then
            Response.Redirect("/default.aspx")
        End If

        Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"))
        If dbRegistration.CompleteDate <> Nothing Then
            ctlSteps.Visible = False
        End If

        PageURL = Request.Url.ToString()
        CurrentUserId = Session("VendorId")
        UserName = Session("Username")

        If Not IsPostBack Then

         Core.DataLog("Edit Supply Phases", PageURL, CurrentUserId, "Vendor Left Menu Click", "", "", "", "", UserName)

            'Dim root As SupplyPhaseRow = SupplyPhaseRow.GetRootSupplyPhase(DB)
            'lsSupplyPhases.DataSource = SupplyPhaseRow.GetChildren(DB, root.SupplyPhaseID, VendorRow.GetLLCList(DB, Session("VendorId")))
            lsSupplyPhases.DataSource = VendorCategoryRow.GetList(DB, "SortOrder")
            lsSupplyPhases.DataTextField = "Category"
            lsSupplyPhases.DataValueField = "VendorCategoryID"
            lsSupplyPhases.DataBind()
            'lsSupplyPhases.SelectedValues = SupplyPhaseRow.GetVendorSupplyPhaseString(DB, Session("VendorId"), True)
            lsSupplyPhases.SelectedValues = VendorRow.GetRow(DB, Session("VendorId")).GetSelectedVendorCategories
        End If

    End Sub

    Private Sub Process(ByVal Redirect As String)
        If lsSupplyPhases.SelectedValues = String.Empty Then
            AddError("Please select at least one Supply Phase")
            Exit Sub
        End If

        DB.BeginTransaction()
        Try
            'Dim sql As String = "delete from VendorSupplyPhase where VendorID=" & DB.Number(Session("VendorId"))
            'DB.ExecuteSQL(sql)

            'dim sPhases as string = lsSupplyPhases.SelectedValues
            'SQL = "with Parents(SupplyPhaseId, ParentSupplyPhaseId) as (" _
            '        & "     select SupplyPhaseId, ParentSupplyPhaseId from SupplyPhase where SupplyPhaseId in " & DB.NumberMultiple(sPhases) _
            '        & " union all " _
            '        & "     select sp.SupplyPhaseId, sp.ParentSupplyPhaseId from SupplyPhase sp inner join Parents p on sp.ParentSupplyPhaseId=p.SupplyPhaseID " _
            '        & " )" _
            '        & " insert into VendorSupplyPhase(VendorID,SupplyPhaseID) Select Distinct " _
            '        & DB.Number(Session("VendorId")) _
            '        & ",SupplyPhaseId" _
            '        & " from Parents union select " & DB.Number(Session("VendorId")) & ", SupplyPhaseId from SupplyPhase where ParentSupplyPhaseID is null"

            'DB.ExecuteSQL(SQL)

            Dim dbVendor As VendorRow = VendorRow.GetRow(DB, Session("VendorId"))

            dbVendor.DeleteFromAllVendorCategories()
            dbVendor.InsertToVendorCategories(lsSupplyPhases.SelectedValues)

            Dim dbRegistration As VendorRegistrationRow = VendorRegistrationRow.GetRowByVendor(DB, Session("VendorId"))
            Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetStatusByStep(DB, 5)
            Dim bSendNotifications As Boolean = False
            If dbRegistration.CompleteDate = Nothing Then
                dbRegistration.RegistrationStatusID = dbStat.RegistrationStatusID
                dbRegistration.CompleteDate = Now
                dbRegistration.Update()
                bSendNotifications = True
            End If

            DB.CommitTransaction()

            If bSendNotifications Then
                Dim dtBuilders As DataTable = BuilderRow.GetListByLLC(DB, dbVendor.GetSelectedLLCs.Split(",")(0))

                Dim dbMsg As AutomaticMessagesRow = AutomaticMessagesRow.GetRowByTitle(DB, "NewVendorForBuilders")
                For Each row As DataRow In dtBuilders.Rows
                    Dim dbBuilder As BuilderRow = BuilderRow.GetRow(DB, row("BuilderId"))

                    dbMsg.Send(dbBuilder, "New Vendor: " & vbTab & dbVendor.CompanyName & vbCrLf & vbCrLf & GlobalRefererName, CCLLCNotification:=False)
                Next
                dbMsg.SendAdmin("New Vendor: " & vbTab & dbVendor.CompanyName & vbCrLf & vbCrLf & GlobalRefererName, String.Empty)
            End If

            Response.Redirect(Redirect)
        Catch ex As SqlClient.SqlException
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnSKUPrice_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSKUPrice.Click
        'log Btn Save & Input Pricing clicked
        Core.DataLog("Edit Supply Phases", PageURL, CurrentUserId, "Btn Save & Input Pricing", "", "", "", "", UserName)
        'end log
        Process("sku-price.aspx")
    End Sub

    Protected Sub btnDashboard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDashboard.Click
        'log Btn Save & Input Pricing clicked
        Core.DataLog("Edit Supply Phases", PageURL, CurrentUserId, "Btn Save & Return To Dashboard", "", "", "", "", UserName)
        'end log
        Process("/vendor/default.aspx")
    End Sub

    'Protected Sub btnGoToDashBoard_Click(sender As Object, e As System.EventArgs) Handles btnGoToDashBoard.Click
        'Response.Redirect("/vendor/default.aspx")
    'End Sub

End Class
