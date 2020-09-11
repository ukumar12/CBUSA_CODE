Imports Components
Imports DataLayer

Partial Class StoreCheckoutSteps
    Inherits BaseControl

    Protected MultipleShipToEnabled As Boolean = False
    Public Property CurrentStep() As CheckouStepEnum
        Get
            Return IIf(ViewState("CurrentStep") Is Nothing, CheckouStepEnum.Billing, ViewState("CurrentStep"))
        End Get
        Set(ByVal value As CheckouStepEnum)
            ViewState("CurrentStep") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MultipleShipToEnabled = SysParam.GetValue(DB, "MultipleShipToEnabled")
    End Sub

End Class

Public Enum CheckouStepEnum
    Billing = 1
    Shipping = 2
    Payment = 3
    Confirmation = 4
End Enum
