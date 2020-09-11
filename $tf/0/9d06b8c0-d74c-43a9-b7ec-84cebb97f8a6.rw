Imports DataLayer
Imports Components
Imports Vindicia
Imports NameValuePair = Vindicia.NameValuePair

Partial Class controls_VindiciaHostedPayment
    Inherits ModuleControl

    Protected Property BuilderBillingAddress() As String
    Public Property RequestedFromUpdatePage As Boolean = False

    Private Sub controls_VindiciaHostedPayment_Load(sender As Object, e As EventArgs) Handles Me.Load
        If (Me._dbBuilderAccount Is Nothing) Then
            Throw New ArgumentException("Builder account not provided")
        End If

        Dim builderAddress As BuilderAccountRow = BuilderAccountRow.GetPrimaryAccount(DB, Builder.BuilderID)


        InitializeWebSession()
    End Sub

    Private Sub InitializeWebSession()
        vindiciaWebSession = New VindiciaWebSession(DB, Builder, VindiciaWebSessionMethods.Account_updatePaymentMethod, HttpContext.Current)

        If Request("vinHoaErr") IsNot Nothing Then vindiciaWebSession.VinWebSessionVid = Nothing

        If Not (vindiciaWebSession.CheckIfSessionIdIsActiveInVindicia(vindiciaWebSession.VinWebSessionVid)) Then
            vindiciaWebSession.UpdateCreditCardInformation()
        End If
    End Sub


    Public Property Builder As BuilderRow
        Get
            Return _dbBuilderAccount
        End Get
        Set(value As BuilderRow)
            _dbBuilderAccount = value
        End Set
    End Property

    Private _dbBuilderAccount As BuilderRow


    Protected Property vindiciaWebSession As VindiciaWebSession
        Get
            Return _vindiciaWebSession
        End Get
        Set(value As VindiciaWebSession)
            _vindiciaWebSession = value
        End Set
    End Property

    Private _vindiciaWebSession As VindiciaWebSession
End Class
