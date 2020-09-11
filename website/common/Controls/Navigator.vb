Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Text

Namespace Controls

    Public Class NavigatorEventArgs
        Inherits EventArgs

        Private m_PageNumber As Integer = 1

        Public Property PageNumber() As Integer
            Get
                Return m_PageNumber
            End Get
            Set(ByVal Value As Integer)
                m_PageNumber = Value
            End Set
        End Property

        Public Sub New(ByVal iPage As Integer)
            PageNumber = iPage
        End Sub
    End Class

    Public Delegate Sub NavigatorEventHandler(ByVal sender As Object, ByVal e As NavigatorEventArgs)

    <ParseChildren(True)> _
    Public Class Navigator
        Inherits Control
        Implements INamingContainer

        Private m_PagerSize As Integer = 10
        Private m_cssClass As String = String.Empty
        Private m_AddHiddenField As Boolean = False

        Public Property PagerSize() As Integer
            Get
                Return m_PagerSize
            End Get
            Set(ByVal Value As Integer)
                m_PagerSize = Value
            End Set
        End Property

        Public Property AddHiddenField() As Boolean
            Get
                Return m_AddHiddenField
            End Get
            Set(ByVal value As Boolean)
                m_AddHiddenField = value
            End Set
        End Property

        Public Property CssClass() As String
            Get
                Return m_cssClass
            End Get
            Set(ByVal value As String)
                m_cssClass = value
            End Set
        End Property

        Public Event NavigatorEvent As NavigatorEventHandler

        Protected Overridable Sub OnNavigatorEvent(ByVal e As NavigatorEventArgs)
        End Sub

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            MyBase.LoadViewState(savedState)

            'If our child controls have already been created, we need to call
            'CreateControlHierarchy to give them a chance to bind from
            'ViewState. 
            If ChildControlsCreated Then
                CreateChildControls()
            End If
        End Sub

        Public Property PageNumber() As Integer
            Get
                If Not ViewState("PageNumber") Is Nothing Then
                    Return CInt(ViewState("PageNumber"))
                Else
                    Return 1
                End If
            End Get
            Set(ByVal Value As Integer)
                ViewState("PageNumber") = Value
            End Set
        End Property

        Public Property NofRecords() As Integer
            Get
                If Not ViewState("NofRecords") Is Nothing Then
                    Return CInt(ViewState("NofRecords"))
                Else
                    Return 0
                End If
            End Get
            Set(ByVal Value As Integer)
                ViewState("NofRecords") = Value
            End Set
        End Property

        Public ReadOnly Property NofPages() As Integer
            Get
                Return CInt(Math.Round(NofRecords / MaxPerPage + 0.499999))
            End Get
        End Property

        Public Property MaxPerPage() As Integer
            Get
                If Not ViewState("MaxPerPage") Is Nothing Then
                    Return CInt(ViewState("MaxPerPage"))
                Else
                    Return 10
                End If
            End Get
            Set(ByVal Value As Integer)
                ViewState("MaxPerPage") = Value
            End Set
        End Property

        Private Sub AddText(ByVal s As String)
            Dim ltl As LiteralControl = New LiteralControl(s)
            ltl.EnableViewState = False
            Controls.Add(ltl)
        End Sub

        Protected Overrides Sub CreateChildControls()
            Dim iHowMany As Integer, iTotal As Integer, iPage As Integer = PageNumber, iMaxPerPage As Integer = MaxPerPage
            Dim iStartNumber As Integer, iStopNumber As Integer
            Dim iNumOfLinks As Integer = PagerSize
            Dim iStartLoop As Integer, iEndLoop As Integer

            iHowMany = NofRecords
            iTotal = iHowMany
            iHowMany = CType(Math.Round(CType(iHowMany / iMaxPerPage + 0.499999, Single)), Integer)
            iPage = Math.Min(iPage, iHowMany)
            iStartNumber = (iPage - 1) * iMaxPerPage + 1
            iStopNumber = iPage * iMaxPerPage
            If iStopNumber > iTotal Then
                iStopNumber = iTotal
            End If

            Controls.Clear()

            Dim text As StringBuilder = New StringBuilder()
            If AddHiddenField Then
                text.Append("<input type=""hidden"" name=""" & UniqueID & """ id=""" & ClientID & """ value="""" />")
            End If
            text.Append("<table width=""100%"" border=0 cellspacing=0 cellpadding=0>")
            text.Append("<tr>")
            text.Append("<td width=""100%"" align=left")

            If CssClass = String.Empty Then
                text.Append(">")
            Else
                text.Append(" class=" & CssClass & ">")
            End If

            text.Append("<nobr><b>Rows:</b> " & iStartNumber & " - " & iStopNumber & " of " & iTotal & "</nobr>")
            AddText(text.ToString())

            If iHowMany > 1 Then
                AddText("<p></P>")

                If iPage > 1 Then
                    Dim link As LinkButton = New LinkButton()
                    link.Text = "&laquo; Previous " & iMaxPerPage
                    AddHandler link.Click, AddressOf Link_Click
                    link.CommandArgument = "prev"
                    link.CausesValidation = False
                    link.EnableViewState = False
                    Controls.Add(link)
                    AddText(" | ")
                Else
                    AddText("&laquo; Previous " & iMaxPerPage & " | ")
                End If

                iStartLoop = Math.Max(iPage - iNumOfLinks, 1)
                iEndLoop = Math.Min(iPage + iNumOfLinks, iHowMany)

                Dim i As Integer
                For i = iStartLoop To iEndLoop
                    If iPage = i Then
                        AddText(" <b>" & i & "</b>")
                    Else
                        AddText(" ")
                        Dim link As LinkButton = New LinkButton()
                        link.Text = i.ToString()
                        AddHandler link.Click, AddressOf Link_Click
                        link.CommandArgument = i.ToString()
                        link.CausesValidation = False
                        link.EnableViewState = False
                        Controls.Add(link)
                    End If
                Next

                If iPage - iHowMany < 0 Then
                    iMaxPerPage = Math.Min(iMaxPerPage, iTotal - iMaxPerPage * iPage)
                    AddText(" | ")

                    Dim link As LinkButton = New LinkButton()
                    link.Text = "Next " & iMaxPerPage & " &raquo;"
                    AddHandler link.Click, AddressOf Link_Click
                    link.CommandArgument = "next"
                    link.CausesValidation = False
                    link.EnableViewState = False
                    Controls.Add(link)
                Else
                    AddText(" | Next " & iTotal - iMaxPerPage * iPage + iMaxPerPage & " &raquo;")
                End If
            End If
            text.Remove(0, text.Length)
            text.Append("</td><td valign=top align=right")

            If CssClass = String.Empty Then
                text.Append(">")
            Else
                text.Append(" class=" & CssClass & ">")
            End If

            text.Append("&nbsp;&nbsp;&nbsp;<nobr><b>Page:</b> " & iPage & " of " & iHowMany & "</nobr>")
            text.Append("</td></tr>")
            text.Append("</table>")
            AddText(text.ToString())
        End Sub

        Private Sub Link_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim iPage As Integer
            Dim link As LinkButton = CType(sender, LinkButton)

            Select Case link.CommandArgument.ToString()
                Case "prev"
                    iPage = PageNumber - 1
                Case "next"
                    iPage = PageNumber + 1
                Case Else
                    iPage = Int32.Parse(link.CommandArgument)
            End Select
            RaiseEvent NavigatorEvent(Me, New NavigatorEventArgs(iPage))
        End Sub

        Public Overrides Sub DataBind()
            CreateChildControls()
            MyBase.DataBind()
        End Sub

    End Class

End Namespace
