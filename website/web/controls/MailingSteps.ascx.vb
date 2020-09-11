Imports Components
Imports Utility
Imports System.Data

Partial Class MailingSteps
    Inherits BaseControl

    Public Property MessageId() As Integer
        Get
            Return ViewState("MessageId")
        End Get
        Set(ByVal value As Integer)
            ViewState("MessageId") = value
        End Set
    End Property

    Public Property CurrentStep() As Integer
        Get
            If ViewState("CurrentStep") Is Nothing Then ViewState("CurrentStep") = 1
            Return ViewState("CurrentStep")
        End Get
        Set(ByVal value As Integer)
            ViewState("CurrentStep") = value
        End Set
    End Property

    Public Property Step1() As Boolean
        Get
            If ViewState("Step1") Is Nothing Then ViewState("Step1") = False
            Return ViewState("Step1")
        End Get
        Set(ByVal value As Boolean)
            ViewState("Step1") = value
        End Set
    End Property

    Public Property Step2() As Boolean
        Get
            If ViewState("Step2") Is Nothing Then ViewState("Step2") = False
            Return ViewState("Step2")
        End Get
        Set(ByVal value As Boolean)
            ViewState("Step2") = value
        End Set
    End Property

    Public Property Step3() As Boolean
        Get
            If ViewState("Step3") Is Nothing Then ViewState("Step3") = False
            Return ViewState("Step3")
        End Get
        Set(ByVal value As Boolean)
            ViewState("Step3") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub
End Class
