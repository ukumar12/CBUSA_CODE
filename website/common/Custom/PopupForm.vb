Imports Components
Imports DataLayer
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Collections.Specialized

Namespace PopupForm
    <ParseChildren(True)> _
    <PersistChildren(False)> _
    Public Class PopupForm
        Inherits CompositeControl
        Implements IScriptControl
        Implements IPostBackEventHandler
        Implements IPostBackDataHandler
        Implements ICallbackEventHandler

        Public Event TemplateLoaded As EventHandler
        Public Event Postback As EventHandler
        Public Event Callback As PopupFormCallbackHandler
        Public Delegate Sub PopupFormCallbackHandler(ByVal sender As Object, ByVal args As PopupFormEventArgs)

        Private m_FormTemplate As ITemplate
        <PersistenceMode(PersistenceMode.InnerDefaultProperty)> _
        <TemplateInstance(TemplateInstance.Single)> _
        Public Property FormTemplate() As ITemplate
            Get
                Return m_FormTemplate
            End Get
            Set(ByVal value As ITemplate)
                m_FormTemplate = value
            End Set
        End Property

        Private m_Buttons As Generic.List(Of PopupFormButton)
        <PersistenceMode(PersistenceMode.InnerProperty)> _
        Public ReadOnly Property Buttons() As Generic.List(Of PopupFormButton)
            Get
                If m_Buttons Is Nothing Then
                    m_Buttons = New Generic.List(Of PopupFormButton)
                End If
                Return m_Buttons
            End Get
        End Property

        Private m_OpenMode As PopupFormOpenMode = PopupFormOpenMode.None
        Public Property OpenMode() As PopupFormOpenMode
            Get
                Return m_OpenMode
            End Get
            Set(ByVal value As PopupFormOpenMode)
                m_OpenMode = value
            End Set
        End Property

        Private m_Animate As Boolean
        Public Property Animate() As Boolean
            Get
                Return m_Animate
            End Get
            Set(ByVal value As Boolean)
                m_Animate = value
            End Set
        End Property

        Private m_OpenTriggerId As String
        Public Property OpenTriggerId() As String
            Get
                Return m_OpenTriggerId
            End Get
            Set(ByVal value As String)
                m_OpenTriggerId = value
            End Set
        End Property

        Private m_CloseTriggerId As String
        Public Property CloseTriggerId() As String
            Get
                Return m_CloseTriggerId
            End Get
            Set(ByVal value As String)
                m_CloseTriggerId = value
            End Set
        End Property

        Private m_ErrorPlaceholderId As String
        Public Property ErrorPlaceholderId() As String
            Get
                Return m_ErrorPlaceholderId
            End Get
            Set(ByVal value As String)
                m_ErrorPlaceholderId = value
            End Set
        End Property

        Private m_ShowVeil As Boolean
        Public Property ShowVeil() As Boolean
            Get
                Return m_ShowVeil
            End Get
            Set(ByVal value As Boolean)
                m_ShowVeil = value
            End Set
        End Property

        Private m_VeilCloses As Boolean
        Public Property VeilCloses() As Boolean
            Get
                Return m_VeilCloses
            End Get
            Set(ByVal value As Boolean)
                m_VeilCloses = value
            End Set
        End Property

        Private m_ValidateCallback As Boolean
        Public Property ValidateCallback() As Boolean
            Get
                Return m_ValidateCallback
            End Get
            Set(ByVal value As Boolean)
                m_ValidateCallback = value
            End Set
        End Property


        Private m_Inputs As Generic.List(Of Control)
        Private ReadOnly Property Inputs() As Generic.List(Of Control)
            Get
                If m_Inputs Is Nothing Then
                    m_Inputs = New Generic.List(Of Control)
                End If
                Return m_Inputs
            End Get
        End Property

        Private m_Validators As Generic.List(Of Control)
        Private ReadOnly Property Validators() As Generic.List(Of Control)
            Get
                If m_Validators Is Nothing Then
                    m_Validators = New Generic.List(Of Control)
                End If
                Return m_Validators
            End Get
        End Property

        Private m_CallbackResult As String
        Public Property CallbackResult() As String
            Get
                Return m_CallbackResult
            End Get
            Set(ByVal value As String)
                m_CallbackResult = value
            End Set
        End Property

        Private m_IsValid As Boolean
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return m_IsValid
            End Get
        End Property

        Public Sub EnsureChildrenCreated()
            EnsureChildControls()
        End Sub

        Protected Overrides Sub CreateChildControls()
            MyBase.CreateChildControls()

            Dim container As New HtmlControls.HtmlGenericControl("div")
            container.Attributes.Add("class", CssClass)
            For Each key As String In Attributes.Keys
                container.Attributes.Add(key, Attributes(key))
            Next
            For Each key As String In Style.Keys
                container.Style.Add(key, Style(key))
            Next
            If Width <> Nothing And container.Style("width") = Nothing Then
                container.Style.Add("width", Width.ToString)
            End If
            If Height <> Nothing And container.Style("height") = Nothing Then
                container.Style.Add("height", Height.ToString)
            End If
            container.Style.Add("display", "none")
            Controls.Add(container)
            container.ID = "window"
            FormTemplate.InstantiateIn(container)
            RaiseEvent TemplateLoaded(Me, EventArgs.Empty)

            Inputs.Clear()
            Validators.Clear()
            FindInputsAndValidators(container)
        End Sub

        Private Sub FindInputsAndValidators(ByVal container As Control)
            If TypeOf container Is IPostBackDataHandler Then
                Inputs.Add(container)
            End If
            If TypeOf container Is BaseValidator Then
                Validators.Add(container)
                'If CType(container, BaseValidator).EnableClientScript Then
                '    CType(container, BaseValidator).Enabled = False
                'End If
            End If
            If container.HasControls Then
                For Each ctl As Control In container.Controls
                    FindInputsAndValidators(ctl)
                Next
            End If
        End Sub

        Public Function GetScriptDescriptors() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptDescriptor) Implements System.Web.UI.IScriptControl.GetScriptDescriptors
            Dim s As New ScriptControlDescriptor("AE.PopupForm", ClientID)

            Select Case OpenMode
                Case PopupFormOpenMode.MoveToCenter
                    s.AddScriptProperty("moveToCenter", "true")
                Case PopupFormOpenMode.MoveToClick
                    s.AddScriptProperty("moveToClick", "true")
            End Select

            s.AddScriptProperty("animate", Animate.ToString.ToLower())

            If OpenTriggerId <> String.Empty Then
                Dim ctl As Control = FindControl(OpenTriggerId)
                If ctl Is Nothing Then
                    If Me.NamingContainer IsNot Nothing Then
                        ctl = NamingContainer.FindControl(OpenTriggerId)
                    End If
                    If ctl Is Nothing Then
                        ctl = Page.FindControl(OpenTriggerId)
                    End If
                End If
                s.AddElementProperty("openTrigger", ctl.ClientID)
            End If

            If CloseTriggerId <> String.Empty Then
                Dim ctl As Control = FindControl(CloseTriggerId)
                If ctl Is Nothing Then
                    If NamingContainer IsNot Nothing Then
                        ctl = NamingContainer.FindControl(CloseTriggerId)
                    End If
                    If ctl Is Nothing Then
                        ctl = Page.FindControl(CloseTriggerId)
                    End If
                End If
                s.AddElementProperty("closeTrigger", ctl.ClientID)
            End If

            If ErrorPlaceholderId <> String.Empty Then
                Dim ctl As Control = FindControl(ErrorPlaceholderId)
                s.AddElementProperty("errorPlaceholder", ctl.ClientID)
            End If

            s.AddScriptProperty("showVeil", ShowVeil.ToString.ToLower)
            s.AddScriptProperty("veilCloses", VeilCloses.ToString.ToLower)

            Dim sInputs As New Text.StringBuilder
            Dim conn As String = String.Empty
            sInputs.Append("[")
            For Each ctl As Control In Inputs
                sInputs.Append(conn & "{'serverId':'" & ctl.ID & "','clientId':'" & ctl.ClientID & IIf(TypeOf ctl Is Controls.DatePicker, "_cal", "") & "'}")
                conn = ","
            Next
            sInputs.Append("]")

            Dim sValidators As New Text.StringBuilder
            conn = String.Empty
            sValidators.Append("[")
            For Each ctl As Control In Validators
                sValidators.Append(conn & "'" & ctl.ClientID & "'")
                conn = ","
            Next
            sValidators.Append("]")

            Dim sButtons As New Text.StringBuilder
            sButtons.Append("[")
            conn = String.Empty
            Dim q = From btn As PopupFormButton In Buttons Select New With {.button = btn, .control = FindControl(btn.ControlId)}
            For Each btn As Object In q
                sButtons.Append(conn & "{'id':'" & btn.control.ClientID & "','serverId':'" & btn.control.ID & "'")
                Select Case btn.button.ButtonType
                    Case PopupFormButtonType.Callback
                        sButtons.Append(",'callback':""" & Page.ClientScript.GetCallbackEventReference(Me, "callbackArg", "callbackResultDel", "ctxt", "function(res,ctxt) {alert(res);}", False) & """")
                        If btn.button.ClientCallback <> String.Empty Then
                            sButtons.Append(",'clientCallback':" & btn.button.ClientCallback)
                        End If
                    Case PopupFormButtonType.Postback
                        sButtons.Append(",'postback':""" & Page.ClientScript.GetPostBackEventReference(Me, btn.control.UniqueID) & """")
                End Select
                If btn.control.Attributes("onclick") <> Nothing Then
                    sButtons.Append(",'onclick':""" & btn.control.Attributes("onclick") & """")
                End If
                sButtons.Append("}")
                conn = ","
            Next
            sButtons.Append("]")

            s.AddScriptProperty("buttons", sButtons.ToString)
            s.AddScriptProperty("inputs", sInputs.ToString)
            s.AddScriptProperty("validators", sValidators.ToString)
            Return New ScriptDescriptor() {s}
        End Function

        Public Function GetScriptReferences() As System.Collections.Generic.IEnumerable(Of System.Web.UI.ScriptReference) Implements System.Web.UI.IScriptControl.GetScriptReferences
            Dim r As New ScriptReference("/includes/controls/PopupForm.js")
            Return New ScriptReference() {r}
        End Function

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            ScriptManager.GetCurrent(Page).RegisterScriptControl(Me)
        End Sub

        Public Overrides Sub RenderBeginTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        End Sub

        Public Overrides Sub RenderEndTag(ByVal writer As System.Web.UI.HtmlTextWriter)
        End Sub

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            MyBase.Render(writer)

            writer.AddAttribute("id", ClientID)
            writer.AddAttribute("name", UniqueID)
            writer.AddAttribute("type", "hidden")
            writer.AddAttribute("value", ID)
            writer.RenderBeginTag("input")
            writer.RenderEndTag()

            ScriptManager.GetCurrent(Page).RegisterScriptDescriptors(Me)
        End Sub

        Public Overrides Sub DataBind()
            EnsureChildControls()
            MyBase.DataBind()
        End Sub

        Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            If eventArgument = String.Empty Then
                eventArgument = System.Web.HttpContext.Current.Request("__EVENTARGUMENT")
            End If
            If Page.IsCallback Or eventArgument = String.Empty Then Exit Sub
            EnsureChildControls()
            Dim ctl As Control = Page.FindControl(eventArgument)
            RaiseEvent Postback(ctl, EventArgs.Empty)
        End Sub

        Public Function GetCallbackResult() As String Implements System.Web.UI.ICallbackEventHandler.GetCallbackResult
            Return CallbackResult
        End Function

        Public Sub RaiseCallbackEvent(ByVal eventArgument As String) Implements System.Web.UI.ICallbackEventHandler.RaiseCallbackEvent
            Dim args As String() = Regex.Split(eventArgument, "(?<!\\)\|")
            Dim json As New Web.Script.Serialization.JavaScriptSerializer()
            Dim ctl As Control = FindControl(args(0))
            Dim data As Object = json.DeserializeObject(args(1))
            Dim temp As New StringDictionary
            For Each item As KeyValuePair(Of String, Object) In data
                temp.Add(item.Key, item.Value)
                Dim input As Object = FindControl(item.Key)
                If input IsNot Nothing Then
                    If TypeOf input Is TextBox Then
                        DirectCast(input, TextBox).Text = item.Value
                    ElseIf TypeOf input Is DropDownList Then
                        DirectCast(input, DropDownList).SelectedValue = item.Value
                    ElseIf TypeOf input Is HiddenField Then
                        DirectCast(input, HiddenField).Value = item.Value
                    ElseIf TypeOf input Is Controls.DatePicker Then
                        DirectCast(input, Controls.DatePicker).Value = IIf(IsDate(item.Value), item.Value, Nothing)
                    End If
                End If
            Next

            m_IsValid = True
            Dim errorIds As New Generic.List(Of String)
            If ValidateCallback Then
                EnsureChildControls()
                Dim eph As MasterPages.ErrorMessage = DirectCast(Page, BasePage).ErrorPlaceHolder
                For Each v As BaseValidator In Validators
                    v.Validate()
                    If Not v.IsValid Then
                        m_IsValid = False
                        errorIds.Add(v.ControlToValidate)
                        eph.AddError(v.ErrorMessage)
                    End If
                Next
                eph.UpdateVisibility()
            End If
            If m_IsValid Then
                RaiseEvent Callback(Me, New PopupFormEventArgs(ctl, temp))
            Else
                CallbackResult = "{'errors':" & json.Serialize(errorIds) & ",'errorMsg':'" & GetControlHtml(CType(Page, BasePage).ErrorPlaceHolder).Replace("'", "\'") & "'}"
                RaiseEvent Callback(Me, New PopupFormEventArgs(ctl, temp))
            End If
        End Sub

        Public Function GetControlHtml(ByVal ctl As Control) As String
            Dim out As New Text.StringBuilder
            Dim sw As New IO.StringWriter(out)
            Dim hw As New HtmlTextWriter(sw)

            ctl.RenderControl(hw)
            Return out.ToString
        End Function

        Public Sub Validate()
            m_IsValid = True
            For Each v As BaseValidator In Validators
                v.Validate()

                If Not v.IsValid Then
                    m_IsValid = False
                End If
            Next
        End Sub

        Public Function LoadPostData(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData
            'Page.RegisterRequiresRaiseEvent(Me)
            If postCollection(postDataKey) <> Nothing Then
                EnsureChildControls()
                For Each i As Control In Inputs
                    If postCollection.AllKeys.Contains(i.UniqueID) Then
                        CType(i, IPostBackDataHandler).LoadPostData(i.UniqueID, postCollection)
                    End If
                Next
            End If
        End Function

        Public Sub RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub
    End Class

    Public Class PopupFormButton
        Public ControlId As String
        Public ButtonType As PopupFormButtonType
        Public ClientCallback As String
    End Class

    Public Enum PopupFormButtonType
        Postback
        Callback
        ScriptOnly
    End Enum

    Public Enum PopupFormOpenMode
        None
        MoveToClick
        MoveToCenter
    End Enum

    Public Class PopupFormEventArgs
        Public Sub New(ByVal Source As Control, ByVal Data As StringDictionary)
            Me.Source = Source
            Me.Data = Data
        End Sub
        Public Source As Control
        Public Data As StringDictionary
    End Class
End Namespace
