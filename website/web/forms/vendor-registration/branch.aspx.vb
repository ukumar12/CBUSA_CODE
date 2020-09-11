Imports Components
Imports DataLayer

Partial Class forms_vendor_registration_branch
    Inherits SitePage

    Protected VendorId As Integer
    Protected VendorAccountId As Integer
    Protected VendorBranchOfficeId As Integer
    Protected dbVendor As VendorRow
    Protected dbVendorAccount As VendorAccountRow
    Protected dbVendorBranchOffice As VendorBranchOfficeRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("VendorId") <> 0 Then
            VendorId = CType(Session("VendorId"), Integer)
            dbVendor = VendorRow.GetRow(Me.DB, VendorId)
        Else
            Response.Redirect("/default.aspx")
        End If

        If Session("VendorAccountId") <> 0 Then
            VendorAccountId = CType(Session("VendorAccountId"), Integer)
            dbVendorAccount = VendorAccountRow.GetRow(Me.DB, VendorAccountId)
        Else
            Response.Redirect("/default.aspx")
        End If

        If Request("VendorBranchOfficeId") <> String.Empty Then
            If IsNumeric(Request("VendorBranchOfficeId")) Then
                VendorBranchOfficeId = CType(Request("VendorBranchOfficeId"), Integer)
                dbVendorBranchOffice = VendorBranchOfficeRow.GetRow(Me.DB, VendorBranchOfficeId)
            Else
                dbVendorBranchOffice = New VendorBranchOfficeRow(Me.DB)
            End If
        Else
            dbVendorBranchOffice = New VendorBranchOfficeRow(Me.DB)
        End If

        If Not IsPostBack Then

            drpBranchState.DataSource = DataLayer.StateRow.GetStateList(DB)
            drpBranchState.DataTextField = "StateName"
            drpBranchState.DataValueField = "StateCode"
            drpBranchState.DataBind()
            drpBranchState.Items.Insert(0, New ListItem("", ""))

            LoadFromDB()
        End If
    End Sub

    Private Sub LoadFromDB()

        If VendorBranchOfficeId = 0 Then
            btnDelete.Visible = False
            Exit Sub
        End If

        With dbVendorBranchOffice
            txtBranchAddress.Text = dbVendorBranchOffice.Address
            txtBranchAddress2.Text = dbVendorBranchOffice.Address2
            txtBranchCity.Text = dbVendorBranchOffice.City
            drpBranchState.SelectedValue = dbVendorBranchOffice.State
            txtBranchZip.Text = dbVendorBranchOffice.Zip
        End With
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If dbVendorBranchOffice.VendorBranchOfficeID <> 0 Then
            dbVendorBranchOffice.Remove()
        End If
        Response.Redirect("register.aspx")
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Page.Validate("VendorReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbVendorBranchOffice
                .Address = txtBranchAddress.Text
                .Address2 = txtBranchAddress2.Text
                .City = txtBranchCity.Text
                .State = drpBranchState.SelectedValue
                .Zip = txtBranchZip.Text
                .VendorID = VendorId
            End With

            If dbVendorBranchOffice.VendorBranchOfficeID = 0 Then
                dbVendorBranchOffice.Insert()
            Else
                dbVendorBranchOffice.Update()
            End If

            DB.CommitTransaction()

            Response.Redirect("register.aspx")

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try

    End Sub

End Class
