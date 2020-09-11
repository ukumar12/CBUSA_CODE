Imports Components
Imports DataLayer
Imports Utilities

Partial Class modules_forms_BuilderRegistration
    Inherits ModuleControl

    Protected BuilderId As Integer
    Protected dbBuilder As BuilderRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        BuilderId = Request("BuilderId")
        dbBuilder = BuilderRow.GetRow(DB, BuilderId)
        If Not IsPostBack Then
            LoadFromDB()
        End If
    End Sub

    Private m_NextStepUrl As String
    Public Property NextStepUrl() As String
        Get
            If m_NextStepUrl = Nothing Then
                m_NextStepUrl = Args(0)
            End If
            Return m_NextStepUrl
        End Get
        Set(ByVal value As String)
            m_NextStepUrl = value
        End Set
    End Property

    Private Sub LoadFromDB()
        If dbBuilder.BuilderID = 0 Then
            Exit Sub
        End If

        With dbBuilder
            txtCompanyName.Text = .CompanyName
            txtAddress.Text = .Address
            txtAddress2.Text = .Address2
            txtCity.Text = .City
            txtState.Text = .State
            txtZip.Text = .Zip
            txtPhone.Text = .Phone
            txtMobile.Text = .Mobile
            txtFax.Text = .Fax
            txtWebsiteUrl.Text = .WebsiteURL
            txtEmail.Text = .Email
        End With
    End Sub

    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click
        Page.Validate("BuilderReg")
        If Not Page.IsValid Then Exit Sub

        Try
            DB.BeginTransaction()

            With dbBuilder
                .CompanyName = txtCompanyName.Text
                .Address = txtAddress.Text
                .Address2 = txtAddress2.Text
                .City = txtCity.Text
                .State = txtState.Text
                .Zip = txtZip.Text
                .Phone = txtPhone.Text
                .Fax = txtFax.Text
                .Mobile = txtMobile.Text
                .WebsiteURL = txtWebsiteUrl.Text
                .Email = txtEmail.Text
            End With

            Dim dbStat As RegistrationStatusRow = RegistrationStatusRow.GetRowByStatus(DB, "Pending")
            dbBuilder.RegistrationStatusID = dbStat.RegistrationStatusID

            If dbBuilder.BuilderID = 0 Then
                dbBuilder.Insert()
            Else
                dbBuilder.Update()

            End If

            DB.CommitTransaction()
            '' Salesforce Integration
            'Dim sfHelper As SalesforceHelper = New SalesforceHelper(DB)
            'If sfHelper.Login() Then
            '    If sfHelper.UpsertBuilder(dbBuilder) = False Then
            '        'throw error
            '    End If
            'End If

            Response.Redirect(NextStepUrl)

        Catch ex As Exception
            If DB IsNot Nothing AndAlso DB.Transaction IsNot Nothing Then DB.RollbackTransaction()
            AddError(ErrHandler.ErrorText(ex))
        End Try
    End Sub
End Class
