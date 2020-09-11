Imports Components

<ParseChildren(True, "Content")> _
<PersistChildren(False)> _
Partial Class controls_InfoBox
    Inherits BaseControl

    Private m_Content As String
    <PersistenceMode(PersistenceMode.InnerDefaultProperty)> _
    Public Property Content() As String
        Get
            Return m_Content
        End Get
        Set(ByVal value As String)
            m_Content = value
        End Set
    End Property

    Public Property Width() As Integer
        Get
            Return IIf(ViewState("Width") Is Nothing, 300, ViewState("Width"))
        End Get
        Set(ByVal value As Integer)
            ViewState("Width") = value
        End Set
    End Property

    Public Property Height() As Integer
        Get
            Return IIf(ViewState("Height") Is Nothing, 150, ViewState("Height"))
        End Get
        Set(ByVal value As Integer)
            ViewState("Height") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        frmInfo.Width = Width
        frmInfo.Height = Height
        If Not Page.ClientScript.IsClientScriptBlockRegistered("InfoBox") Then
            Dim s As String = _
                "function OpenInfo(e,id) {" _
                & " var frm = $get(id);" _
                & " if(frm && frm.control) {" _
                & "     frm.control._doMoveToClick(e); " _
                & "     frm.control.Open();" _
                & " }" _
                & "}"

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "InfoBox", s, True)
        End If
    End Sub

    Protected Sub frmInfo_TemplateLoaded(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmInfo.TemplateLoaded
        ltlContent.Text = Content
    End Sub
End Class
