Imports Components
Imports Utility
Imports System.Data

Public Class LimitTextBox
    Inherits BaseControl

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ctrl.Columns = Size
        ctrl.Attributes("style") = "width: " & Width.ToString & "px;"
        ctrl.MaxLength = MaxLength
        If Not TextMode = Nothing Then ctrl.TextMode = TextMode
        If Rows <> 0 Then ctrl.Rows = Rows
        If Columns <> 0 Then ctrl.Columns = Columns
        ctrl.Attributes("onKeyUp") = "limit('" & ClientID & "', " & Width & "," & Limit & ")"
        ctrl.Attributes("onFocus") = "limit('" & ClientID & "', " & Width & "," & Limit & ")"
    End Sub

    Public Property CssClass() As String
        Get
            Return ctrl.CssClass
        End Get
        Set(ByVal Value As String)
            ctrl.CssClass = Value
        End Set
    End Property

    Public Property TextMode() As TextBoxMode
        Get
            Return ViewState("TextMode")
        End Get
        Set(ByVal Value As TextBoxMode)
            ViewState("TextMode") = Value
        End Set
    End Property

    Public Property Rows() As Integer
        Get
            Return ViewState("Rows")
        End Get
        Set(ByVal Value As Integer)
            ViewState("Rows") = Value
        End Set
    End Property

    Public Property Columns() As Integer
        Get
            Return ViewState("Columns")
        End Get
        Set(ByVal Value As Integer)
            ViewState("Columns") = Value
        End Set
    End Property


    Public Property Width() As Integer
        Get
            Return ViewState("Width")
        End Get
        Set(ByVal Value As Integer)
            ViewState("Width") = Value
        End Set
    End Property

    Public Property MaxLength() As Integer
        Get
            Return ViewState("MaxLength")
        End Get
        Set(ByVal Value As Integer)
            ViewState("MaxLength") = Value
        End Set
    End Property

    Public Property Size() As Integer
        Get
            Return ViewState("Size")
        End Get
        Set(ByVal Value As Integer)
            ViewState("Size") = Value
        End Set
    End Property

    Public Property Text() As String
        Get
            Return ctrl.Text
        End Get
        Set(ByVal Value As String)
            ctrl.Text = Value
        End Set
    End Property

    Public Property Limit() As Integer
        Get
            Return ViewState("Limit")
        End Get
        Set(ByVal Value As Integer)
            ViewState("Limit") = Value
        End Set
    End Property

End Class

