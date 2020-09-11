Imports Components
Imports DataLayer
Imports System.Data.SqlClient

Public Class Member_Addressbook_Edit
    Inherits AdminPage

    Private m_AddressId As Integer
    Protected MemberId As Integer
    Private dbMember As MemberRow

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        CheckAccess("MEMBERS")
        Try
            m_AddressId = Request.QueryString("AddressId")
            MemberId = Convert.ToInt32(Request("MemberId"))
            dbMember = MemberRow.GetRow(DB, MemberId)
            If Request("mode") = "Add" Then
                btnDelete.Visible = False
            End If
        Catch ex As Exception
            AddError(ex.Message)
        End Try
        If Not IsPostBack Then
            LoadDropdowns()
            If m_AddressId > 0 Then LoadFormData()
        End If

        If drpCountry.SelectedValue = "US" Then
            rfvState.Enabled = True
            rfvRegion.Enabled = False
        Else
            rfvState.Enabled = False
            rfvRegion.Enabled = True
        End If
    End Sub

    Private Sub LoadFormData()
        Dim dbMemberAddress As MemberAddressRow
        If m_AddressId > 0 Then dbMemberAddress = MemberAddressRow.GetRow(DB, m_AddressId) Else Exit Sub
        txtLabel.Text = dbMemberAddress.Label
        txtFirstName.Text = dbMemberAddress.FirstName
        txtLastName.Text = dbMemberAddress.LastName
        txtCompany.Text = dbMemberAddress.Company
        txtAddress1.Text = dbMemberAddress.Address1
        txtAddress2.Text = dbMemberAddress.Address2
        txtCity.Text = dbMemberAddress.City
        txtZip.Text = dbMemberAddress.Zip
        txtPhone.Text = dbMemberAddress.Phone
        drpState.SelectedValue = dbMemberAddress.State
        drpCountry.SelectedValue = dbMemberAddress.Country
        txtRegion.Text = dbMemberAddress.Region
        txtLabel.Text = dbMemberAddress.Label
    End Sub

    Private Sub LoadDropdowns()
        drpState.DataSource = StateRow.GetStateList(DB)
        drpState.DataValueField = "StateCode"
        drpState.DataTextField = "StateName"
        drpState.DataBind()
        drpState.Items.Insert(0, New ListItem("", ""))

        drpCountry.DataSource = CountryRow.GetCountryList(DB)
        drpCountry.DataValueField = "CountryCode"
        drpCountry.DataTextField = "CountryName"
        drpCountry.DataBind()
        drpCountry.Items.Insert(0, New ListItem("", ""))
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsValid Then Exit Sub

        ' CHECK THE SHIPPING ADDRESS
        If SysParam.GetValue(DB, "UPSAddressVerification") AndAlso drpCountry.SelectedValue = "US" Then
            Dim ErrorDesc As String = String.Empty
            If Not UPS.ValidateAddress(txtCity.Text, drpState.SelectedValue, txtZip.Text, drpCountry.SelectedValue, ErrorDesc) Then
                AddError("Entered Address is not recognized. Please verify the City, State, Zipcode and Country are correct. Make sure to spell out your full City name.")
                Exit Sub
            End If
        End If

        Try
            DB.BeginTransaction()
            Dim dbMemberAddress As MemberAddressRow
            If m_AddressId > 0 Then dbMemberAddress = MemberAddressRow.GetRow(DB, m_AddressId) Else dbMemberAddress = New MemberAddressRow(DB)
            dbMemberAddress.MemberId = dbMember.MemberId
            dbMemberAddress.AddressType = "AddressBook"
            dbMemberAddress.IsDefault = False
            dbMemberAddress.FirstName = txtFirstName.Text
            dbMemberAddress.LastName = txtLastName.Text
            dbMemberAddress.Company = txtCompany.Text
            dbMemberAddress.Address1 = txtAddress1.Text
            dbMemberAddress.Address2 = txtAddress2.Text
            dbMemberAddress.City = txtCity.Text
            dbMemberAddress.Zip = txtZip.Text
            dbMemberAddress.Phone = txtPhone.Text
            dbMemberAddress.State = drpState.SelectedValue
            dbMemberAddress.Country = drpCountry.SelectedValue
            dbMemberAddress.Region = txtRegion.Text
            dbMemberAddress.Label = txtLabel.Text

            If m_AddressId > 0 Then dbMemberAddress.Update() Else dbMemberAddress.Insert()
            DB.CommitTransaction()

            Response.Redirect("/admin/members/addressbook/default.aspx?MemberId=" & MemberId)

        Catch ex As SqlException
            If Not DB Is Nothing AndAlso Not DB.Transaction Is Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Response.Redirect("/admin/members/addressbook/delete.aspx?AddressId=" & m_AddressId & "&MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect("/admin/members/addressbook/default.aspx?MemberId=" & MemberId & "&" & GetPageParams(FilterFieldType.All))
    End Sub
End Class