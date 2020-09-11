Imports System.Configuration.ConfigurationManager
Imports Components

Partial Class Navigation
    Inherits System.Web.UI.UserControl
    Implements IPostBackEventHandler

    Const NofLinks As Integer = 10

    Public Event Navigate As EventHandler

    Public Property URL() As String
        Get
            Return IIf(ViewState("URL") Is Nothing, "/store/default.aspx", ViewState("URL"))
        End Get
        Set(ByVal value As String)
            ViewState("URL") = value
        End Set
    End Property

    Public Property Sort() As String
        Get
            If ViewState("sort") Is Nothing Then ViewState("sort") = "score"
            Return ViewState("sort")
        End Get
        Set(ByVal value As String)
            ViewState("sort") = value
        End Set
    End Property

    Public Property Pg() As Integer
        Get
            If ViewState("pg") Is Nothing Then ViewState("pg") = 1
            If ViewState("pg") = 0 Then ViewState("pg") = 1
            Return ViewState("pg")
        End Get
        Set(ByVal value As Integer)
            ViewState("pg") = value
        End Set
    End Property

    Public Property MaxPerPage() As Integer
        Get
            Return ViewState("MaxPerPage")
        End Get
        Set(ByVal value As Integer)
            ViewState("MaxPerPage") = value
        End Set
    End Property

    Public Property NofRecords() As Integer
        Get
            Return ViewState("NofRecords")
        End Get
        Set(ByVal value As Integer)
            ViewState("NofRecords") = value
        End Set
    End Property

    Public ReadOnly Property NofPages() As Integer
        Get
            If MaxPerPage = 0 Then Return 0
            Return CType(Math.Round(CType(NofRecords / MaxPerPage + 0.499999, Single)), Integer)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ltl.Text = String.Empty

        Dim StartLoop As Integer = Math.Max(Pg - NofLinks, 1)
        Dim EndLoop As Integer = Math.Min(Pg + NofLinks, NofPages)
        Dim bEnd As Boolean = False
        While EndLoop - StartLoop >= NofLinks
            If EndLoop - Pg > Pg - StartLoop Then
                EndLoop -= 1
            Else
                StartLoop += 1
            End If
        End While

        For i As Integer = StartLoop To EndLoop
            If i = Pg Then
                ltl.Text &= "<span class=""pageon"">" & i & "</span>" & vbCrLf
            Else
                ltl.Text &= "<a href=""" & Page.ClientScript.GetPostBackClientHyperlink(Me, i) & """>" & i & "</a>" & vbCrLf
            End If
        Next

        lnkPrev2.Visible = True
        lnkPrev2.HRef = Page.ClientScript.GetPostBackClientHyperlink(Me, Pg - 1)
        If Pg = 1 Then lnkPrev2.Visible = False

        lnkNext1.HRef = Page.ClientScript.GetPostBackClientHyperlink(Me, Pg + 1)
        If Pg = NofPages Then lnkNext1.Visible = False

        If NofPages = 1 Then ltl.Visible = False

        Dim m As Integer = Nothing
        If Pg = NofPages Then
            m = NofRecords Mod MaxPerPage
        End If
        If m = 0 Then
            m = MaxPerPage
        End If
        ltlShowing.Text = "<span class=""blue"">Showing " & ((Pg - 1) * MaxPerPage + 1) & "-" & ((Pg - 1) * MaxPerPage + m) & " of " & NofRecords & "</span>"
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If NofRecords = 0 Then Visible = False
    End Sub

    Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
        Pg = eventArgument
        RaiseEvent Navigate(Me, EventArgs.Empty)
    End Sub
End Class
