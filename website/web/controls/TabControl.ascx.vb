Imports Components

Partial Class controls_TabControl
    Inherits BaseControl

    Protected Property TabIndex() As Integer
        Get
            Return ViewState("TabIndex")
        End Get
        Set(ByVal value As Integer)
            ViewState("TabIndex") = value
        End Set
    End Property

    Public ReadOnly Property Data() As Generic.Dictionary(Of String, String)
        Get
            If ViewState("Data") Is Nothing Then
                ViewState("Data") = New Generic.Dictionary(Of String, String)
            End If
            Return ViewState("Data")
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        RegisterScripts()
    End Sub

    Private Sub RegisterScripts()
        If Not Page.ClientScript.IsClientScriptBlockRegistered("TabControl") Then
            Dim s As String = _
                  "var TabCtlSelected_" & ID & " = 0;" _
                & "function SelectTab(id,idx) {" _
                & "     var lasttab = $get(id + '_LI_' + TabCtlSelected_" & ID & ");" _
                & "     var newtab = $get(id + '_LI_' + idx);" _
                & "     lasttab.className = 'taboff';" _
                & "     newtab.className = 'tabon';" _
                & "     var lastdiv = $get(id + '_DIV_' + TabCtlSelected_" & ID & ");" _
                & "     var newdiv = $get(id + '_DIV_' + idx);" _
                & "     lastdiv.style.display = 'none';" _
                & "     newdiv.style.display = '';" _
                & "     TabCtlSelected_" & ID & " = idx;" _
                & "}"

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "TabControl", s, True)
        End If
    End Sub

    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()

        ulTabs.Controls.Clear()
        divContent.Controls.Clear()

        Dim cnt As Integer = 0
        For Each item As Generic.KeyValuePair(Of String, String) In Data
            Dim li As New HtmlGenericControl("li")
            li.ID = "LI_" & cnt
            li.InnerHtml = item.Key
            li.Attributes.Add("class", IIf(cnt = 0, "tabon", "taboff"))
            li.Attributes.Add("onclick", "SelectTab('" & ID & "'," & cnt & ")")
            ulTabs.Controls.Add(li)

            Dim div As New HtmlGenericControl("div")
            div.ID = "DIV_" & cnt
            If item.Value <> Nothing Then div.InnerHtml = Server.HtmlEncode(item.Value).Replace(vbCrLf, "<br />").Replace(vbCr, "<br />").Replace(vbLf, "<br />").Replace(vbTab, "&nbsp;&nbsp;&nbsp;&nbsp;")
            div.Attributes.Add("class", "tabcontent")
            divContent.Controls.Add(div)
            If cnt > 0 Then
                div.Style.Add("display", "none")
            End If

            cnt += 1
        Next
    End Sub
End Class
